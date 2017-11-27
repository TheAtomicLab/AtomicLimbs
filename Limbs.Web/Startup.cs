using System;
using System.Reflection;
using System.Web.Mvc;
using LightInject;
using Limbs.Web.Common.Mail;
using Limbs.Web.Repositories;
using Microsoft.Owin;
using Owin;
using Limbs.Web.Services;
using Limbs.Web.Repositories.Interfaces;
using Limbs.Web.Storage.Azure;
using Microsoft.AspNet.SignalR;
using Resources;

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
            DefaultModelBinder.ResourceClassKey = typeof(LimbsResources).Name;
            ClientDataTypeModelValidatorProvider.ResourceClassKey = typeof(LimbsResources).Name;

            var ass = Assembly.Load("Microsoft.AspNet.Identity.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
            var hack = ass.GetType("Microsoft.AspNet.Identity.Resources");
            var field = hack.GetField("resourceMan", BindingFlags.Static | BindingFlags.NonPublic);
            if (field == null) return;

            field.SetValue(null, new System.Resources.ResourceManager(typeof(LimbsResources).FullName ?? throw new InvalidOperationException(), typeof(LimbsResources).Assembly));
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
