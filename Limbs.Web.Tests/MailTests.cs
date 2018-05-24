using System;
using System.Configuration;
using Limbs.QueueConsumers;
using Limbs.Web.Common.Mail;
using Limbs.Web.Storage.Azure.QueueStorage;
using Limbs.Web.Storage.Azure.QueueStorage.Messages;
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
            var mailMessage = new System.Net.Mail.MailMessage(username,
                adminmail?.Split(',')[0] ?? throw new InvalidOperationException(), "Limb.Web.Tests", "TEST MESSAGE");

            // si el mensaje es null significa que el maker controló algunas situaciones y no hay nada para enviar y el mensaje se puede remover de la queue
            mailSender.Send(mailMessage);
        }

        [TestMethod]
        public void StartConsumigMailsMessagesSender()
        {
            QueueConsumerFor<MailMessage>.WithinCurrentThread.Using(new MailsMessagesSender())
                .With(PoolingFrequencer.For(MailsMessagesSender.EstimatedTime))
                .StartConsimung();
        }
        
        [TestMethod]
        public void EnqueueMailMessage()
        {
            AzureQueue.Enqueue(new MailMessage
            {
                From = "no-reply@atomiclab.org",
                To = "alebanzas@outlook.com",
                Subject = "TEST MSJ",
                Body = "Test body",
            });
        }
    }
}