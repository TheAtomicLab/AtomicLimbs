using AutoMapper;
using Limbs.Web.Common.Extensions;
using Limbs.Web.Entities.Models;
using Limbs.Web.ViewModels.Admin;
using System;

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

            CreateMap<EventModel, AssignEventViewModel>()
                .ForMember(p => p.EventTypeName, opt => opt.MapFrom(src => src.EventType == null ? string.Empty : src.EventType.Name))
                .ForMember(p => p.StartDate, opt => opt.MapFrom(src => src.StartDate.ToString("dd/MM/yyyy hh:mm tt")))
                .ForMember(p => p.EndDate, opt => opt.MapFrom(src => src.EndDate.ToString("dd/MM/yyyy hh:mm tt")));

            CreateMap<EventOrderModel, OrdersEventViewModel>()
                .ForMember(p => p.OrderId, opt => opt.MapFrom(src => src.Order.Id))
                .ForMember(p => p.OrderDate, opt => opt.MapFrom(src => src.Order.Date.ToFriendlyDateString()))
                .ForMember(p => p.AmputationDescription, opt => opt.MapFrom(src => src.Order.AmputationTypeFk == null ? string.Empty : src.Order.AmputationTypeFk.Short_Description))
                .ForMember(p => p.RequesterEmail, opt => opt.MapFrom(src => src.Order.OrderRequestor == null ? string.Empty : src.Order.OrderRequestor.Email))
                .ForMember(p => p.AmbassadorEmail, opt => opt.MapFrom(src => src.Order.OrderAmbassador == null ? string.Empty : src.Order.OrderAmbassador.Email));

            CreateMap<SponsorModel, SponsorViewModel>()
                .ForMember(p => p.EventDescription, opt => opt.MapFrom(src => src.Event.Description))
                .ForMember(p => p.EventId, opt => opt.MapFrom(src => src.Event.Id));


            CreateMap<SponsorViewModel, SponsorModel>()
                .ForMember(p => p.CreatedDate, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(p => p.CreatedBy, opt => opt.Ignore())
                .ForMember(p => p.Event, opt => opt.Ignore());
        }
    }
}