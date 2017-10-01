using LightInject;
using Limbs.Web.Repositories;
using Microsoft.Owin;
using Owin;
using Limbs.Web.Services;
using Limbs.Web.Repositories.Interfaces;
using Limbs.Web.Storage.Azure;
using Limbs.Web.Storage.Azure.BlobStorage;
using Microsoft.WindowsAzure.Storage.Blob;

[assembly: OwinStartupAttribute(typeof(Limbs.Web.Startup))]
namespace Limbs.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            ConfigureServices();
            ConfigureStorage();
        }

        public void ConfigureServices()
        {
            var container = new ServiceContainer();
            
            container.Register<IOrdersRepository, OrdersRepository>();
            container.Register<IUsersRepository, UsersRepository>();
            container.Register<IAmbassadorsRepository, AmbassadorsRepository>();
            container.Register<IProductsRepository, ProductsRepository>();
            container.Register<IUserFiles, UserFilesInAzureStorage>();

            container.RegisterControllers();
            container.EnableMvc();
        }


        public void ConfigureStorage()
        {
            var storageAccount = AzureStorageAccount.DefaultAccount;
            var blobClient = storageAccount.CreateCloudBlobClient();

            //Container creation
            blobClient.GetContainerReference(AzureStorageContainer.UserFiles.ToString()).CreateIfNotExists(BlobContainerPublicAccessType.Blob);
        }
    }


}
