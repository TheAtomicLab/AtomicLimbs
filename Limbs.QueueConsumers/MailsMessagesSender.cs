using System;
using System.Configuration;
using Limbs.Web.Common.Extensions;
using Limbs.Web.Common.Mail;
using Limbs.Web.Storage.Azure.QueueStorage;
using Limbs.Web.Storage.Azure.QueueStorage.Messages;

namespace Limbs.QueueConsumers
{
    public class MailsMessagesSender : IQueueMessageConsumer<MailMessage>
    {
        public static readonly TimeSpan EstimatedTime = TimeSpan.FromSeconds(5);
        
        public TimeSpan? EstimatedTimeToProcessMessageBlock { get; }

        public void ProcessMessages(QueueMessage<MailMessage> message)
        {
            try
            {
                var mailserver = ConfigurationManager.AppSettings["Mail.Server"];
                var username = ConfigurationManager.AppSettings["Mail.Username"];
                var password = ConfigurationManager.AppSettings["Mail.Password"];

                var mailSender = new GeneralMailSender(mailserver, username, password);
                var mailMessage = new System.Net.Mail.MailMessage(
                    message.Data.From, message.Data.To,
                    message.Data.Subject, message.Data.Body) {IsBodyHtml = true};
                // si el mensaje es null significa que el maker controló algunas situaciones y no hay nada para enviar y el mensaje se puede remover de la queue
                mailSender.Send(mailMessage);

                Console.WriteLine($"Email sent ({message.Data.Subject}) to: {message.Data.To}");
            }
            catch (Exception e)
            {
                e.Log(null, "Enviando mail key GeneralMailSender");
                var exmsg = $"Enviando mail : {e.Message}\nStackTrace:{e.StackTrace}";
                Console.WriteLine(exmsg, "Error");
                if (message.DequeueCount < 20)
                {
                    throw;
                }
            }
        }
    }
}
