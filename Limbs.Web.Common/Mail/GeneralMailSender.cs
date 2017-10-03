using System;
using System.Net;
using System.Net.Mail;

namespace Limbs.Web.Common.Mail
{
	/// <summary>
	/// Sender General de mails.
	/// </summary>
	public class GeneralMailSender: IMailSender
	{
	    private readonly string _mailserver;
        private readonly string _userName;
	    private readonly string _password;

        public GeneralMailSender(string mailserver, string userName, string password)
        {
            if (string.IsNullOrEmpty(mailserver))
            {
                throw new ArgumentNullException(nameof(mailserver));
            }
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentNullException(nameof(userName));
            }
            if (string.IsNullOrEmpty(password))
			{
				throw new ArgumentNullException(nameof(password));
		    }
		    _mailserver = mailserver;
		    _userName = userName;
            _password = password;
		}

		public void Send(MailMessage mail)
		{
			var client = new SmtpClient { Host = _mailserver, EnableSsl = true, Credentials = new NetworkCredential(_userName, _password) };
			client.Send(mail);
		}
	}
}