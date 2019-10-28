namespace Limbs.Web.ViewModels.Admin
{
    public class AmputationViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string UrlImage { get; set; }
        public string Short_Description { get; set; }
    }

    public class EditAmputationViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string UrlImage { get; set; }
        public string Short_Description { get; set; }
    }
}