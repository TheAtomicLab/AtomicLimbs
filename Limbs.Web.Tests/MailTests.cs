using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Configuration;
using System.Dynamic;
using System.IO;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Web.Razor;
using Limbs.Web.Common.Mail;
using Microsoft.CSharp;
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
            var mailMessage = new MailMessage(username,
                adminmail?.Split(',')[0] ?? throw new InvalidOperationException(), "Limb.Web.Tests", "TEST MESSAGE");

            // si el mensaje es null significa que el maker controló algunas situaciones y no hay nada para enviar y el mensaje se puede remover de la queue
            mailSender.Send(mailMessage);
        }

        [TestMethod]
        

        private static string GetStringFromSomewhere()
        {
            return "<b>Well done @DynModel.Who !!</b>";
        }
    }
}