namespace Limbs.Web.Entities.WebModels.Admin.Models
{
    public class UserViewModel
    {
        public string Email { get; set; }

        public bool IsEmailConfirmed { get; set; }

        public string[] Roles { get; set; }

        public string[] LoginProviders { get; set; }
    }
}