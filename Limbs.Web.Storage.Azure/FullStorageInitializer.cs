using Limbs.Web.Storage.Azure.BlobStorage;
using Limbs.Web.Storage.Azure.QueueStorage.Messages;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Limbs.Web.Storage.Azure
{
	public class FullStorageInitializer
	{
		public static void Initialize()
		{
			var account = AzureStorageAccount.DefaultAccount;

		    new QueueStorageInitializer<MailMessage>(account).Initialize();
		    new QueueStorageInitializer<AppException>(account).Initialize();
		    new QueueStorageInitializer<OrderProductGenerator>(account).Initialize();
		    new QueueStorageInitializer<OrderProductGeneratorResult>(account).Initialize();

		    new DocumentStorageInitializer(account, AzureStorageContainer.UserFiles).Initialize(BlobContainerPublicAccessType.Container);
		    new DocumentStorageInitializer(account, AzureStorageContainer.ProductGenerated).Initialize(BlobContainerPublicAccessType.Container);

        }
    }
}