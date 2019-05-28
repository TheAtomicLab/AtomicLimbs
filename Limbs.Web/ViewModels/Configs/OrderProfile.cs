using AutoMapper;
using Limbs.Web.Entities.Models;
using System;
using System.Collections.Generic;

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
                .ForMember(p => p.TotalImages, opt => opt.MapFrom(src => (src.IdImage == null) ? 0 : src.IdImage.Split(',').Length))
                .ForMember(p => p.Images, opt => opt.MapFrom(src => src.IdImage.Split(',')))
                .ReverseMap()
                .ForAllOtherMembers(p => p.Ignore());

            CreateMap<NewOrder, OrderModel>()
                .ForMember(p => p.Status, opt => opt.MapFrom(src => OrderStatus.NotAssigned))
                .ForMember(p => p.StatusLastUpdated, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(p => p.Date, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(p => p.RenderPieces, opt => opt.MapFrom(src => new List<OrderRenderPieceModel>()));
        }
    }
}