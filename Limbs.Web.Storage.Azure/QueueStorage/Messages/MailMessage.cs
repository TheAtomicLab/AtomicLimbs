namespace Limbs.Web.Storage.Azure.QueueStorage.Messages
{
	public class MailMessage
	{
		public string From { get; set; }
        public string To { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
	}
}