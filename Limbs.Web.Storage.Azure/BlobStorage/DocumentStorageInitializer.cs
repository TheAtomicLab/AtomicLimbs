using System;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Limbs.Web.Storage.Azure
{
	public class DocumentStorageInitializer : IStorageInitializer
	{
		private readonly CloudStorageAccount _account;
		private readonly string _documentsContainerName;

		public DocumentStorageInitializer(CloudStorageAccount account, string documentsContainerName)
		{
		    if (string.IsNullOrWhiteSpace(documentsContainerName))
			{
				throw new ArgumentNullException(nameof(documentsContainerName));
			}
			_account = account ?? throw new ArgumentNullException(nameof(account));
			_documentsContainerName = documentsContainerName.ToLowerInvariant();
		}

		public virtual string DocumentsContainerName => _documentsContainerName;

	    public void Initialize()
        {
            Initialize(BlobContainerPublicAccessType.Off);
        }

	    public void Initialize(BlobContainerPublicAccessType accessType)
		{
			var blobStorageType = _account.CreateCloudBlobClient();
			var container = blobStorageType.GetContainerReference(_documentsContainerName);
			container.CreateIfNotExists();
			var perm = new BlobContainerPermissions
			{
                PublicAccess = accessType
			};
			container.SetPermissions(perm);
		}

		public void Drop()
		{
			var blobStorageType = _account.CreateCloudBlobClient();
		    if (!blobStorageType.ListContainers().Select(c => c.Name).Contains(_documentsContainerName)) return;
		    var container = blobStorageType.GetContainerReference(_documentsContainerName);
		    container.Delete();
		}
	}

	public class DocumentStorageInitializer<TDocument> : DocumentStorageInitializer where TDocument : class
	{
		public DocumentStorageInitializer(CloudStorageAccount account)
			: base(account, typeof(TDocument).Name.ToLowerInvariant())
		{
		}
	}
}