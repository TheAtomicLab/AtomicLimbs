using System.Net.Mail;

namespace Limbs.Web.Common.Mail
{
    public interface IMailSender
    {
        void Send(MailMessage mail);
    }
}