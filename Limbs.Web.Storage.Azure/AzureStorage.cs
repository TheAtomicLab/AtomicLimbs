using Microsoft.WindowsAzure.Storage;
using System.Configuration;

namespace Limbs.Web.Storage.Azure
{
    public sealed class AzureStorageAccount
    {
        private const string DefaultDataConnectionString = "Storage.ConnectionString";

        public static CloudStorageAccount DefaultAccount => CloudStorageAccount.TryParse(ConfigurationManager.AppSettings[DefaultDataConnectionString], out var account) ?
            account :
            CloudStorageAccount.DevelopmentStorageAccount;
    }
}