using LightInject;
using Limbs.Web.Repositories;
using Microsoft.Owin;
using Owin;
using Limbs.Web.Services;
using Limbs.Web.Repositories.Interfaces;
using Limbs.Web.Storage.Azure;

[assembly: OwinStartupAttribute(typeof(Limbs.Web.Startup))]
namespace Limbs.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            ConfigureServices();

            FullStorageInitializer.Initialize();
        }

        public void ConfigureServices()
        {
            var container = new ServiceContainer();
            
            container.Register<IOrdersRepository, OrdersRepository>();
            container.Register<IUsersRepository, UsersRepository>();
            container.Register<IAmbassadorsRepository, AmbassadorsRepository>();
            container.Register<IUserFiles, UserFilesInAzureStorage>();

            container.RegisterControllers();
            container.EnableMvc();
        }
    }
}
