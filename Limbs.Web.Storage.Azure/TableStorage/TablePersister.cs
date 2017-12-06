using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Limbs.Web.Storage.Azure.TableStorage
{
	public class TablePersister<TDataRow> : IPersister<TDataRow> where TDataRow : TableEntity
	{
		private readonly CloudTable table;
		private readonly string entityTableName = typeof(TDataRow).AsTableStorageName();

        public TablePersister(CloudTableClient tableClient)
		{
            if (tableClient == null)
			{
                throw new ArgumentNullException(nameof(tableClient));
			}
            table = tableClient.GetTableReference(entityTableName);
		}

		public TDataRow Get(string partitionKey, string rowKey)
		{
		    try
		    {
                var retrieveOperation = TableOperation.Retrieve<TDataRow>(partitionKey, rowKey);

                var retrievedResult = table.Execute(retrieveOperation);

                return (TDataRow)retrievedResult.Result;
		    }
		    catch (StorageException e)
		    {
                if (e.RequestInformation.HttpStatusCode == (int)HttpStatusCode.NotFound)
		        {
		            return null;
		        }
		        throw;
		    }
		}

        public void AddBatch(TableBatchOperation tableOperation)
        {
            table.ExecuteBatch(tableOperation);
        }
        public async Task AddBatchAsync(TableBatchOperation tableOperation)
        {
            await table.ExecuteBatchAsync(tableOperation);
        }

        public void Add(TDataRow dataRow)
        {
            var op = TableOperation.Insert(dataRow);
            table.Execute(op);
        }

        public async Task AddAsync(TDataRow dataRow)
        {
            var op = TableOperation.Insert(dataRow);
            await table.ExecuteAsync(op);
        }

        public void Update(TDataRow dataRow)
        {
            var op = TableOperation.Replace(dataRow);
            table.Execute(op);
        }

        public async Task UpdateAsync(TDataRow dataRow)
        {
            var op = TableOperation.Replace(dataRow);
            await table.ExecuteAsync(op);
        }

        public void Delete(string partitionKey, string rowKey)
		{
			var entity = Get(partitionKey, rowKey);
		    if (entity == null) return;

		    var op = TableOperation.Delete(entity);
		    table.Execute(op);
		}

		public void Delete(TDataRow dataRow)
		{
			Delete(dataRow.PartitionKey, dataRow.RowKey);
		}
	}
}