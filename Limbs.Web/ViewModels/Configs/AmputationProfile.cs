using AutoMapper;
using Limbs.Web.Entities.Models;
using Limbs.Web.ViewModels.Admin;

namespace Limbs.Web.ViewModels.Configs
{
    public class AmputationProfile : Profile
    {
        public AmputationProfile()
        {
            CreateMap<AmputationTypeModel, AmputationViewModel>()
                .ForMember(p => p.UrlImage, src => src.MapFrom(p => p.PrimaryUrlImage));

            CreateMap<AmputationTypeModel, EditAmputationViewModel>()
                .ForMember(p => p.UrlImage, src => src.MapFrom(p => p.PrimaryUrlImage));

            CreateMap<EditAmputationViewModel, AmputationTypeModel>();
        }
    }
}