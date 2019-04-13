using System;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using LightInject;
using Limbs.Web;
using Limbs.Web.Common.Captcha;
using Limbs.Web.Common.Mail;
using Limbs.Web.Common.Resources;
using Limbs.Web.Helpers;
using Limbs.Web.Logic.Helpers;
using Limbs.Web.Logic.Repositories;
using Limbs.Web.Logic.Repositories.Interfaces;
using Limbs.Web.Logic.Services;
using Limbs.Web.Storage.Azure;
using Limbs.Web.ViewModels.Configs;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace Limbs.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var configuration = new Migrations.Configuration();
            var migrator = new System.Data.Entity.Migrations.DbMigrator(configuration);
            migrator.Update();

            AutoMapperLimbsConfig.Configure();

            ConfigureLocalization();
            ConfigureAuth(app);
            ConfigureServices(app);

            FullStorageInitializer.Initialize();
            MailTemplatesRegistration.Initialize();
        }

        private void ConfigureLocalization()
        {
            var resourcesClass = typeof(LimbsResources).FullName;
            DefaultModelBinder.ResourceClassKey = resourcesClass;
            ValidationExtensions.ResourceClassKey = resourcesClass;

            var field = Assembly.Load("Microsoft.AspNet.Identity.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35")
                .GetType("Microsoft.AspNet.Identity.Resources")
                .GetField("resourceMan", BindingFlags.Static | BindingFlags.NonPublic);
            if (field == null) throw new InvalidOperationException(nameof(field));
            field.SetValue(null, LimbsResources.ResourceManager);
        }

        public void ConfigureServices(IAppBuilder app)
        {
            var container = new ServiceContainer();

            container.Register<ICaptchaValidationService, InvisibleRecaptchaValidationService>();
            container.Register<IOrdersRepository, OrdersRepository>();
            container.Register<IUsersRepository, UsersRepository>();
            container.Register<IAmbassadorsRepository, AmbassadorsRepository>();
            container.Register<IUserFiles, UserFilesInAzureStorage>();
            container.Register<IOrderNotificationService, OrderMailNotificationService>();
            container.Register<IMessageService, MessagesService>();

            container.Register<ConnectionMapping<string>>(new PerContainerLifetime());

            var connectionMapping = container.GetInstance<ConnectionMapping<string>>();
            var messageService = container.GetInstance<IMessageService>();

            GlobalHost.DependencyResolver.Register(typeof(MessagesHub), () => new MessagesHub(messageService, connectionMapping));
            app.MapSignalR();

            container.RegisterControllers();
            container.EnableMvc();
        }
    }
}
