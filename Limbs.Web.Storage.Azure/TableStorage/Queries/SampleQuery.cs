using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table.DataServices;

namespace ABServicios.Azure.Storage.DataAccess.TableStorage.Queries
{
	public class SampleQuery
	{
		private readonly TableServiceContext _tableContext;
        private readonly TablePersister<TableSampleData> _tableSamplePersister;

		public SampleQuery(CloudStorageAccount account)
		{
		    var client = account.CreateCloudTableClient();
            _tableSamplePersister = new TablePersister<TableSampleData>(client);
		}

        public TableSampleData GetResultsFromPublicacion(string ex1, string ex2)
		{
            var row = _tableSamplePersister.Get(ex1, ex2);

			return row;
		}

		public void GetResultsFromPackage(string ex1, int ex2)
		{
            var queryable = _tableContext.CreateQuery<TableSampleData>(typeof(TableSampleData).AsTableStorageName());

			var result = (from data in queryable where data.PartitionKey == ex1 select data).AsTableServiceQuery(_tableContext).Execute();
		}
	}
}