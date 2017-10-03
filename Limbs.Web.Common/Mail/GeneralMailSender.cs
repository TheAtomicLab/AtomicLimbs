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
	    private readonly int _port;

        public GeneralMailSender(string mailserver, string userName, string password, int port = 25)
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
            _port = port;

            var p = mailserver.Split(':');
            if (p.Length <= 1) return;

            _mailserver = p[0];
            _port = int.Parse(p[1]);
        }

		public void Send(MailMessage mail)
		{
		    var client = new SmtpClient
		    {
		        Host = _mailserver,
		        Port = _port,
                EnableSsl = true,
                Credentials = new NetworkCredential(_userName, _password),
            //Timeout = 2000,
        };
		    client.Send(mail);
		}
	}
}