using System.Diagnostics;
using Microsoft.WindowsAzure.Storage;

namespace Limbs.Web.Storage.Azure.QueueStorage
{
	public class CurrentThreadQueueMessageConsumer<TMessage> : MessagePoolQueueConsumer<TMessage> where TMessage : class
	{
		public CurrentThreadQueueMessageConsumer(IQueueMessageConsumer<TMessage> consumer) : base(consumer) {}

		protected override System.Threading.Thread CreateThreadForPolling()
		{
			var consumerName = ConsumerName;
			Trace.WriteLine("Within CurrentThread Start " + consumerName, "Information");

			return null;
		}

		protected override CloudStorageAccount QueueAccount => AzureStorageAccount.DefaultAccount;
	}
}