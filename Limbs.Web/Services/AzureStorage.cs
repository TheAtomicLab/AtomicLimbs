using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Limbs.Web.Services
{
    public sealed class AzureStorageAccount
    {
        public static CloudStorageAccount DefaultAccount { get { return CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("Storage.ConnectionString")); } private set { } }
    }

    public sealed class AzureStorageContainer
    {
        public static readonly AzureStorageContainer UserFiles = new AzureStorageContainer(1, "userfiles");
        //public static readonly AzureStorageContainer SINGLESIGNON = new AzureStorageContainer(2, "sample");


        private readonly int value;
        private readonly String name;
                
        private AzureStorageContainer(int value, String name)
        {
            this.value = value;
            this.name = name;
        }

        public override String ToString()
        {
            return name;
        }
    }
}