using System;
using System.Configuration;
using System.Web;
using Limbs.Web.Common.Mail;
using Limbs.Web.Storage.Azure.QueueStorage;
using Limbs.Web.Storage.Azure.QueueStorage.Messages;
using Microsoft.ApplicationInsights;

namespace Limbs.Web.Common.Extensions
{
    public enum ExceptionAction
    {
        Enqueue,
        SendMail,
        SendMailAndEnqueue,
    }

    public static class ExceptionExtensions
    {
        public static void Log(this Exception exception, ExceptionAction action = ExceptionAction.Enqueue)
        {
            Log(exception, null, string.Empty, action);
        }

        public static void Log(this Exception exception, HttpContext context, ExceptionAction action = ExceptionAction.Enqueue)
        {
            Log(exception, context, string.Empty, action);
        }

        public static void Log(this Exception exception, HttpContext context, string customMessage = "", ExceptionAction action = ExceptionAction.Enqueue)
        {

            var url = "Not available";
            var urlreferer = "Not available";

            if (context != null)
            {
                url = context.Request.Url?.ToString();

                if (context.User?.Identity != null && context.User.Identity.IsAuthenticated)
                {
                    customMessage += $"| LoggedUser: {context.User.Identity.Name}";
                }

            }

            if (context != null && context.Request.UrlReferrer != null)
            {
                urlreferer = context.Request.UrlReferrer.ToString();
            }

            var message = new AppException
            {
                CustomMessage = string.IsNullOrWhiteSpace(customMessage) ? "AtomicLimbs.Error" : customMessage,
                Exception = exception,
                Url = url,
                UrlReferrer = urlreferer,
            };

            try
            {
                if (ExceptionAction.Enqueue.Equals(action) || ExceptionAction.SendMailAndEnqueue.Equals(action))
                {
                    AzureQueue.Enqueue(message);
                }
            }
            catch (Exception)
            {
                //estamos en la B, error del error
            }

            try
            {
                if (ExceptionAction.SendMail.Equals(action) || ExceptionAction.SendMailAndEnqueue.Equals(action))
                {
                    var mailserver = ConfigurationManager.AppSettings["Mail.Server"];
                    var username = ConfigurationManager.AppSettings["Mail.Username"];
                    var password = ConfigurationManager.AppSettings["Mail.Password"];
                    var from = ConfigurationManager.AppSettings["Mail.From"];
                    var adminmail = ConfigurationManager.AppSettings["AdminEmails"];

                    var mailSender = new GeneralMailSender(mailserver, username, password);
                    var mailMessage = new System.Net.Mail.MailMessage(from, adminmail?.Split(',')[0] ?? throw new InvalidOperationException(), message.CustomMessage, message.Url + "\n\n" + message.UrlReferrer + "\n\n" + message.Exception);

                    // si el mensaje es null significa que el maker controló algunas situaciones y no hay nada para enviar y el mensaje se puede remover de la queue
                    mailSender.Send(mailMessage);
                }
            }
            catch (Exception e)
            {
                //estamos en la B, error del error
            }

            try
            {
                var ai = new TelemetryClient();
                ai.TrackException(exception);
            }
            catch (Exception)
            {
                //estamos en la B, error del error
            }
        }
    }
}