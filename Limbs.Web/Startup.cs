using Limbs.Web.Repositories;
using Microsoft.Owin;
using Owin;
using Microsoft.Extensions.DependencyInjection;

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
            services.AddSingleton<IOrdersRepository, OrdersRepository>();
            services.AddSingleton<IUsersRepository, UsersRepository>();
            services.AddSingleton<IAmbassadorsRepository, AmbassadorsRepository>();
            services.AddSingleton<IProductsRepository, ProductsRepository>();
        }
    }


}
