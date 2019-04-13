using AutoMapper;
using Limbs.Web.Entities.Models;

namespace Limbs.Web.ViewModels.Configs
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<OrderModel, OrderUpdateModel>()
                .ForMember(p => p.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(p => p.AmputationType, opt => opt.MapFrom(src => src.AmputationType))
                .ForMember(p => p.Color, opt => opt.MapFrom(src => src.Color))
                .ForMember(p => p.Comments, opt => opt.MapFrom(src => src.Comments))
                .ForMember(p => p.ProductType, opt => opt.MapFrom(src => src.ProductType))
                .ReverseMap()
                .ForAllOtherMembers(p => p.Ignore());
        }
    }
}