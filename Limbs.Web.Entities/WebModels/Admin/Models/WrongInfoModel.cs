namespace Limbs.Web.Entities.WebModels.Admin.Models
{
    public class WrongInfoModel
    {
        public int? Order_Id { get; set; }
        public string Fullname_Requestor { get; set; }
        public string Email_Requestor { get; set; }
        public bool IsWrongImages { get; set; }
        public string Comments { get; set; }
    }
}