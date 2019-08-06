using AutoMapper;
using Limbs.Web.Entities.Models;

namespace Limbs.Web.ViewModels.Configs
{
    public class EventProfile : Profile
    {
        public EventProfile()
        {
            CreateMap<EventModel, EventViewModel>()
                .ForMember(p => p.EventTypeName, opt => opt.MapFrom(src => src.EventType == null ? string.Empty : src.EventType.Name))
                .ForMember(p => p.StartDate, opt => opt.MapFrom(src => src.StartDate.ToString("dd/MM/yyyy hh:mm tt")))
                .ForMember(p => p.EndDate, opt => opt.MapFrom(src => src.EndDate.ToString("dd/MM/yyyy hh:mm tt")))
                .ForMember(p => p.CountParticipants, opt => opt.MapFrom(src => src.EventOrders == null ? 0 : src.EventOrders.Count));
        }
    }
}