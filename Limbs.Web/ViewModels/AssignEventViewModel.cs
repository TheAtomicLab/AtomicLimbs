namespace Limbs.Web.ViewModels
{
    public class AssignEventViewModel
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int? EventOrderId { get; set; }
        public string EventTypeName { get; set; }
        public string Title { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public bool IsAssigned { get; set; }
    }
}