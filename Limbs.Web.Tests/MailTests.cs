using System;
using System.Configuration;
using Limbs.Web.Common.Mail;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Limbs.Web.Tests
{
    [TestClass]
    public class MailTests
    {
        [TestMethod]
        public void WhenSendEmailThenIsSent()
        {
            var mailserver = ConfigurationManager.AppSettings["Mail.Server"];
            var username = ConfigurationManager.AppSettings["Mail.Username"];
            var password = ConfigurationManager.AppSettings["Mail.Password"];
            var adminmail = ConfigurationManager.AppSettings["AdminEmails"];

            var mailSender = new GeneralMailSender(mailserver, username, password);
            var mailMessage = new System.Net.Mail.MailMessage(username, adminmail?.Split(',')[0] ?? throw new InvalidOperationException(), "Limb.Web.Tests" ,"TEST MESSAGE");

            // si el mensaje es null significa que el maker controló algunas situaciones y no hay nada para enviar y el mensaje se puede remover de la queue
            mailSender.Send(mailMessage);

        }
    }
}
