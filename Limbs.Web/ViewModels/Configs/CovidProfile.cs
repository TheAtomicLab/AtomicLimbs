using AutoMapper;
using Limbs.Web.Entities.Models;

namespace Limbs.Web.ViewModels.Configs
{
    public class CovidProfile : Profile
    {
        public CovidProfile()
        {
            CreateMap<CreateCovidOrganizationViewModel, CovidOrganizationModel>()
                .ForMember(p => p.CovidOrganizationName, src => src.MapFrom(p => p.CovidOrganizationEnum))
                .ForMember(p => p.CovidOrganizationName, src => src.MapFrom(p => p.CovidOrganizationName))
                .ForMember(p => p.Email, src => src.MapFrom(p => p.Email))
                .ForMember(p => p.Name, src => src.MapFrom(p => p.Name))
                .ForMember(p => p.Surname, src => src.MapFrom(p => p.Surname))
                .ForMember(p => p.Country, src => src.MapFrom(p => p.Country))
                .ForMember(p => p.State, src => src.MapFrom(p => p.State))
                .ForMember(p => p.City, src => src.MapFrom(p => p.City))
                .ForMember(p => p.Address, src => src.MapFrom(p => p.Address))
                .ForMember(p => p.Address2, src => src.MapFrom(p => p.Address2))
                .ForMember(p => p.Quantity, src => src.MapFrom(p => p.Quantity))
                .ForMember(p => p.DeliveryDate, src => src.MapFrom(p => p.DeliveryDate))
                .ForMember(p => p.Token, src => src.MapFrom(p => p.Token))
                .ForMember(p => p.Location, src => src.MapFrom(p => p.Location));
        }
    }
}