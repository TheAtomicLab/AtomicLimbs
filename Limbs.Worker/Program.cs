using LightInject;
using Limbs.Web.Common.Mail;
using Limbs.Web.Logic.Services;
using Microsoft.Azure.WebJobs;

namespace Limbs.Worker
{
    // To learn more about Microsoft Azure WebJobs SDK, please see https://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main()
        {
            MailTemplatesRegistration.Initialize();

            var container = new ServiceContainer();
            container.Register<IOrderNotificationService, OrderMailNotificationService>();
            container.Register<Functions>();

            var config = new JobHostConfiguration()
            {
                JobActivator = new SimpleInjectorJobActivator(container)
            };

            config.UseTimers(); 

            if (config.IsDevelopment)
            {
                config.UseDevelopmentSettings();
            }

            var host = new JobHost(config);
            host.RunAndBlock();
        }
    }
}
