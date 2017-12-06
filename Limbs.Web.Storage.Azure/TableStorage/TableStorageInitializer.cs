using System;
using System.Linq;
using System.Reflection;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Limbs.Web.Storage.Azure.TableStorage
{
	public class TableStorageInitializer<TTableEntity> : IStorageInitializer where TTableEntity : class
	{
		private readonly CloudStorageAccount account;
		private readonly string entityTableName = typeof (TTableEntity).AsTableStorageName();

		public TableStorageInitializer(CloudStorageAccount account)
		{
            this.account = account ?? throw new ArgumentNullException(nameof(account));
			var properties =
				typeof (TTableEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(pi => pi.Name).
					ToList();
			if (!properties.Contains("PartitionKey") || !properties.Contains("RowKey") || !properties.Contains("Timestamp"))
			{
				throw new ArgumentOutOfRangeException("TTableEntity",
				                                      "The type of the entity is not a valid Azure entity type.(it should contain at least the three required public properties: PartitionKey,RowKey,Timestamp");
			}
		}

		public void Initialize()
		{
			var client = new CloudTableClient(account.TableEndpoint, account.Credentials);
            var table = client.GetTableReference(entityTableName);
            table.CreateIfNotExists();
			// Execute conditionally for development storage only
            // AB: not necessary for SDK 2.0
			//if (client.BaseUri.IsLoopback)
			//{
			//	var instance = Activator.CreateInstance(typeof (TTableEntity), true);
			//	client.InitializeTableSchemaFromEntity(entityTableName, instance);
			//}
		}

		public void Drop()
		{
			var client = new CloudTableClient(account.TableEndpoint, account.Credentials);
		    var table = client.GetTableReference(entityTableName);
			table.DeleteIfExists();
		}
	}
}