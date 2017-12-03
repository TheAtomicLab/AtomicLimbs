using System;
using System.Reflection;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table.DataServices;

namespace Limbs.Web.Storage.Azure.TableStorage
{
	/// <summary>
	///   Development Storage currently requires the schema for an entity stored in a table to have been 
	///   previously defined before you are allowed to query it. To provide the schema, the methods in this 
	///   class create a new table, store a new entity with the required schema and then delete it. 
	///   This workaround is applied only for Development Storage, Azure Storage does not have this restriction.
	/// </summary>
	/// <remarks>
	///   <para>
	///     This class come from p&p samples
	///   </para>
	///   These methods assume that entities derive from TableServiceEntity.
	/// </remarks>
	public static class TableStorageExtensions
	{
		public static void InitializeTableSchemaFromEntity(this CloudTableClient tableStorage, string entityName,
		                                                    object entity)
		{
			TableServiceContext context = tableStorage.GetTableServiceContext();
			DateTime now = DateTime.UtcNow;
			entity.GetType().GetProperty("PartitionKey").SetValue(entity, Guid.NewGuid().ToString(), null);
			entity.GetType().GetProperty("RowKey").SetValue(entity, Guid.NewGuid().ToString(), null);

			//entity.PartitionKey = Guid.NewGuid().ToString();
			//entity.RowKey = Guid.NewGuid().ToString();
			foreach (var p in entity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
			{
				if ((p.Name != "PartitionKey") && (p.Name != "RowKey") && (p.Name != "Timestamp"))
				{
					if (p.PropertyType == typeof(string))
					{
						p.SetValue(entity, Guid.NewGuid().ToString(), null);
					}
					else if (p.PropertyType == typeof(DateTime))
					{
						p.SetValue(entity, now, null);
					}
				}
			}
			context.AddObject(entityName, entity);
			context.SaveChangesWithRetries();
			context.DeleteObject(entity);
			context.SaveChangesWithRetries();
		}

		public static string AsTableStorageName(this Type dataRowType)
		{
			var tableStorageName = dataRowType.Name;
			if(tableStorageName.EndsWith("DataRow"))
			{
				return tableStorageName.Substring(0, tableStorageName.Length - 7);
			}
			if (tableStorageName.EndsWith("Data"))
			{
				return tableStorageName.Substring(0, tableStorageName.Length - 4);
			}
			return tableStorageName;
		}

		public static string ToAzureKey(this string key)
		{
			return key == null ? null : Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(key));
		}
	}
}