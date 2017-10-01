using System;
using System.Linq;
using Microsoft.WindowsAzure.Storage;

namespace Limbs.Web.Storage.Azure
{
	/// <summary>
	/// Initialize a queue storage specific for a message type.
	/// </summary>
	/// <typeparam name="TMessage">The typeof the message</typeparam>
	public class QueueStorageInitializer<TMessage> : IStorageInitializer where TMessage : class
	{
		private readonly CloudStorageAccount _account;
		private readonly string _queueName = typeof(TMessage).Name.ToLowerInvariant();

		public QueueStorageInitializer(CloudStorageAccount account)
		{
		    _account = account ?? throw new ArgumentNullException(nameof(account));
		}

		public void Initialize()
		{
			var queueClient = _account.CreateCloudQueueClient();
			var queue = queueClient.GetQueueReference(_queueName);
			queue.CreateIfNotExists();
		}

		public void Drop()
		{
			var queueClient = _account.CreateCloudQueueClient();
		    if (!queueClient.ListQueues().Select(c => c.Name).Contains(_queueName)) return;
		    var queue = queueClient.GetQueueReference(_queueName);
		    queue.Delete();
		}
	}
}