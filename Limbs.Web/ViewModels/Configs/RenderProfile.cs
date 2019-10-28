using AutoMapper;
using Limbs.Web.Entities.Models;
using Limbs.Web.ViewModels.Admin;

namespace Limbs.Web.ViewModels.Configs
{
    public class RenderProfile : Profile
    {
        public RenderProfile()
        {
            CreateMap<RenderModel, RenderIndexViewModel>()
                .ForMember(p => p.AmputationImage, src => src.MapFrom(p => p.AmputationType.PrimaryUrlImage))
                .ForMember(p => p.RenderImage, src => src.MapFrom(p => p.PrimaryUrlImage))
                .ForMember(p => p.TotalPieces, src => src.MapFrom(p => p.Pieces.Count));
        }
    }
}