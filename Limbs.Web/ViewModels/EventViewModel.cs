namespace Limbs.Web.ViewModels
{
    public class EventViewModel
    {
        public int Id { get; set; }
        public string EventTypeName { get; set; }
        public string Title { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int CountParticipants { get; set; }
    }
}