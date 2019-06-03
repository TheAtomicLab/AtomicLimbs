using AutoMapper;
using Limbs.Web.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Limbs.Web.ViewModels.Configs
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<OrderModel, OrderUpdateModel>()
                .ForMember(p => p.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(p => p.AmputationTypeFkId, opt => opt.MapFrom(src => src.AmputationTypeFkId))
                .ForMember(p => p.ColorFkId, opt => opt.MapFrom(src => src.ColorFkId))
                .ForMember(p => p.Comments, opt => opt.MapFrom(src => src.Comments))
                .ForMember(p => p.ProductType, opt => opt.MapFrom(src => src.ProductType))
                .ForMember(p => p.TotalImages, opt => opt.MapFrom(src => GetTotalImages(src.IdImage)))
                .ForMember(p => p.Images, opt => opt.MapFrom(src => GetImages(src.IdImage)))
                .ReverseMap()
                .ForAllOtherMembers(p => p.Ignore());

            CreateMap<NewOrder, OrderModel>()
                .ForMember(p => p.Status, opt => opt.MapFrom(src => OrderStatus.NotAssigned))
                .ForMember(p => p.StatusLastUpdated, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(p => p.Date, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(p => p.RenderPieces, opt => opt.MapFrom(src => new List<OrderRenderPieceModel>()));

            CreateMap<RenderModel, RenderViewModel>();
            CreateMap<AmputationTypeModel, AmputationTypeDetailsViewModel>();
            CreateMap<ColorModel, ColorDetailsViewModel>();

            CreateMap<OrderRenderPieceModel, OrderRenderPieceViewModel>()
                .ForMember(p => p.PieceName, opt => opt.MapFrom(src => src.RenderPiece.Render.Pieces.FirstOrDefault(p => p.Id == src.RenderPieceId).Name));

            CreateMap<OrderRenderPieceViewModel, OrderRenderPieceModel>();

            CreateMap<UserModel, OrderRequesterDetailsViewModel>()
                .ForMember(p => p.FullName, opt => opt.MapFrom(src => src.FullName()))
                .ForMember(p => p.FullDni, opt => opt.MapFrom(src => src.FullDni()))
                .ForMember(p => p.Birth, opt => opt.MapFrom(src => src.Birth.ToString("dd/MM/yyyy")))
                .ForMember(p => p.FullAddress, opt => opt.MapFrom(src => src.FullAddress()));

            CreateMap<OrderModel, OrderDetailsViewModel>()
                .ForMember(p => p.RenderPiecesGroupBy, opt => opt.MapFrom(src => GetRenderAndPieces(src.RenderPieces)))
                .ForMember(p => p.Images, opt => opt.MapFrom(src => GetImages(src.IdImage)))
                .ForMember(p => p.Color, opt => opt.MapFrom(src => Mapper.Map<ColorDetailsViewModel>(src.ColorFk)))
                .ForMember(p => p.AmputationType, opt => opt.MapFrom(src => Mapper.Map<AmputationTypeDetailsViewModel>(src.AmputationTypeFk)))
                .ForMember(p => p.OrderRequester, opt => opt.MapFrom(src => Mapper.Map<OrderRequesterDetailsViewModel>(src.OrderRequestor)))
                .ForMember(p => p.PercentagePrinted, opt => opt.MapFrom(src => GetPercentagePrinted(src.RenderPieces)));
        }

        private List<RenderPieceGroupByViewModel> GetRenderAndPieces(IEnumerable<OrderRenderPieceModel> orderRenderPieceModels)
        {
            if (orderRenderPieceModels == null || !orderRenderPieceModels.Any()) return new List<RenderPieceGroupByViewModel>();

            return orderRenderPieceModels.GroupBy(
                p => p.RenderPiece.Render,
                p => p,
                (key, g) => new RenderPieceGroupByViewModel { Render = Mapper.Map<RenderViewModel>(key), OrderRenderPieces = Mapper.Map<List<OrderRenderPieceViewModel>>(g) }).ToList();
        }

        private string[] GetImages(string imageStr)
        {
            if (string.IsNullOrEmpty(imageStr)) return Array.Empty<string>();
            return imageStr.Split(',');
        }

        private int GetTotalImages(string imageStr)
        {
            if (string.IsNullOrEmpty(imageStr)) return 0;
            return imageStr.Split(',').Length;
        }
        private int GetPercentagePrinted(List<OrderRenderPieceModel> orderRenderPieces)
        {
            if (orderRenderPieces == null || !orderRenderPieces.Any(p => p.Printed)) return 0;

            var total = orderRenderPieces.Count;
            var totalPrinted = orderRenderPieces.Count(p => p.Printed);

            return (int)((float)totalPrinted / total * 100);
        }
    }
}