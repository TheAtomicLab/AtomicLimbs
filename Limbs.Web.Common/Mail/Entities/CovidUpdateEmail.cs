namespace Limbs.Web.Common.Mail.Entities
{
    public class CovidUpdateEmail
    {
        public string FullName { get; set; }
        public string Url { get; set; }
        public string PreviousEmail { get; set; }
        public string NewEmail { get; set; }
    }
}
