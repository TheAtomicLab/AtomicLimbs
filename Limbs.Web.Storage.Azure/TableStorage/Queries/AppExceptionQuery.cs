using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table.DataServices;

namespace Limbs.Web.Storage.Azure.TableStorage.Queries
{
	public class AppExceptionQuery
	{
		private readonly TableServiceContext _tableContext;

		public AppExceptionQuery(CloudStorageAccount account)
		{
		    var client = account.CreateCloudTableClient();
            _tableContext = new TableServiceContext(client);
		}
        
		public IEnumerable<AppExceptionData> GetExceptions(DateTime date)
		{
            var queryable = _tableContext.CreateQuery<AppExceptionData>(typeof(AppExceptionData).AsTableStorageName());

		    var query = queryable.Where(x => x.PartitionKey == date.ToString("yyyyMMdd"));

			var result = query.AsTableServiceQuery(_tableContext).Execute();

		    return result;
		}
	}
}