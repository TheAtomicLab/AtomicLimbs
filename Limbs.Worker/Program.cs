using System;
using System.Threading;
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
            var config = new JobHostConfiguration();

            if (config.IsDevelopment)
            {
                config.UseDevelopmentSettings();
            }

            var host = new JobHost(config);

            while (true)
            {
                host.CallAsync(typeof(Functions).GetMethod("MailsMessagesSender"));

                host.CallAsync(typeof(Functions).GetMethod("ProductGeneratorResult"));
                // The following code ensures that the WebJob will be running continuously

                Console.WriteLine("host.RunAndBlock();");
                host.RunAndBlock();

                Console.WriteLine("MainThread.Sleep(5000);");
                Thread.Sleep(5000);
            }
        }
    }
}
