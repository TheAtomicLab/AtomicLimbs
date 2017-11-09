using LightInject;
using Limbs.Web.Common.Mail;
using Limbs.Web.Models;
using Limbs.Web.Repositories;
using Microsoft.Owin;
using Owin;
using Limbs.Web.Services;
using Limbs.Web.Repositories.Interfaces;
using Limbs.Web.Storage.Azure;
using Microsoft.AspNet.SignalR;

[assembly: OwinStartupAttribute(typeof(Limbs.Web.Startup))]
namespace Limbs.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            ConfigureServices(app);

            FullStorageInitializer.Initialize();
            MailTemplatesRegistration.Initialize();
        }

        public void ConfigureServices(IAppBuilder app)
        {
            var container = new ServiceContainer();
            
            container.Register<IOrdersRepository, OrdersRepository>();
            container.Register<IUsersRepository, UsersRepository>();
            container.Register<IAmbassadorsRepository, AmbassadorsRepository>();
            container.Register<IUserFiles, UserFilesInAzureStorage>();
            container.Register<IOrderNotificationService, OrderMailNotificationService>();
            container.Register<IMessageService, MessagesService>();

            GlobalHost.DependencyResolver.Register(typeof(MessagesHub), () => new MessagesHub(new MessagesService()));
            app.MapSignalR();

            container.RegisterControllers();
            container.EnableMvc();
        }
    }
}
