using System;
using System.Reflection;
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
            ConfigureLocalization();
            ConfigureAuth(app);
            ConfigureServices(app);

            FullStorageInitializer.Initialize();
            MailTemplatesRegistration.Initialize();
        }

        private void ConfigureLocalization()
        {
            var ass = Assembly.Load("Microsoft.AspNet.Identity.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
            Type hack = ass.GetType("Microsoft.AspNet.Identity.Resources");
            var field = hack.GetField("resourceMan", BindingFlags.Static | BindingFlags.NonPublic);
            //This is where you set your own local resource manager that will read resource files from your own assembly
            field.SetValue(null, new global::System.Resources.ResourceManager("Limbs.Web.Properties.Resources", typeof(Limbs.Web.Properties.Resources).Assembly));
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
