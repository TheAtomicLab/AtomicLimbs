using System.Diagnostics;
using System.Threading;
using Microsoft.WindowsAzure.Storage;

namespace Limbs.Web.Storage.Azure.QueueStorage
{
	public class CurrentThreadQueueMessageBlockConsumer<TMessage> : MessageBlockPoolQueueConsumer<TMessage> where TMessage : class
	{
		public CurrentThreadQueueMessageBlockConsumer(IQueueMessageBlocksConsumer<TMessage> consumer) : base(consumer) { }

		protected override Thread CreateThreadForPolling()
		{
			var consumerName = ConsumerName;
			Trace.WriteLine("Within CurrentThread Start " + consumerName, "Information");

			return null;
		}

		protected override CloudStorageAccount QueueAccount => AzureStorageAccount.DefaultAccount;
	}
}