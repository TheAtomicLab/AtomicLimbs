using Limbs.Web.Repositories;
using Microsoft.Owin;
using Owin;
using Microsoft.Extensions.DependencyInjection;
using Limbs.Web.Services;
using Limbs.Web.Repositories.Interfaces;
using Microsoft.WindowsAzure.Storage;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage.Blob;

[assembly: OwinStartupAttribute(typeof(Limbs.Web.Startup))]
namespace Limbs.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IUserFiles, UserFilesInAzureStorage>();
            services.AddSingleton<IOrdersRepository, OrdersRepository>();
            services.AddSingleton<IUsersRepository, UsersRepository>();
            services.AddSingleton<IAmbassadorsRepository, AmbassadorsRepository>();
            services.AddSingleton<IProductsRepository, ProductsRepository>();

            ConfigureStorage();
        }


        public void ConfigureStorage()
        {
            CloudStorageAccount storageAccount = AzureStorageAccount.DefaultAccount;
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            //Container creation
            blobClient.GetContainerReference(AzureStorageContainer.UserFiles.ToString()).CreateIfNotExists();
        }
    }


}
