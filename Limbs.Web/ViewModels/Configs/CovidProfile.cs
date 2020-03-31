using AutoMapper;
using Limbs.Web.Entities.Models;

namespace Limbs.Web.ViewModels.Configs
{
    public class CovidProfile : Profile
    {
        public CovidProfile()
        {
            CreateMap<CreateCovidOrganizationViewModel, CovidOrganizationModel>()
                .ForMember(p => p.CovidOrganization, src => src.MapFrom(p => p.CovidOrganizationEnum))
                .ForMember(p => p.CovidOrganizationName, src => src.MapFrom(p => p.CovidOrganizationName))
                .ForMember(p => p.Email, src => src.MapFrom(p => p.Email))
                .ForMember(p => p.Name, src => src.MapFrom(p => p.Name))
                .ForMember(p => p.PersonalPhone, src => src.MapFrom(p => p.PersonalPhone))
                .ForMember(p => p.OrganizationPhone, src => src.MapFrom(p => p.OrganizationPhone))
                .ForMember(p => p.OrganizationPhoneIntern, src => src.MapFrom(p => p.OrganizationPhoneIntern))
                .ForMember(p => p.Dni, src => src.MapFrom(p => p.Dni))
                .ForMember(p => p.Surname, src => src.MapFrom(p => p.Surname))
                .ForMember(p => p.Country, src => src.MapFrom(p => p.Country))
                .ForMember(p => p.State, src => src.MapFrom(p => p.State))
                .ForMember(p => p.City, src => src.MapFrom(p => p.City))
                .ForMember(p => p.Address, src => src.MapFrom(p => p.Address))
                .ForMember(p => p.Address2, src => src.MapFrom(p => p.Address2))
                .ForMember(p => p.Quantity, src => src.MapFrom(p => p.Quantity))
                .ForMember(p => p.Token, src => src.MapFrom(p => p.Token))
                .ForMember(p => p.Location, src => src.MapFrom(p => p.Location));

            CreateMap<CovidOrganizationModel, EditCovidOrganizationViewModel>()
                .ForMember(p => p.CovidOrganizationEnum, src => src.MapFrom(p => p.CovidOrganization))
                .ForMember(p => p.CovidOrganizationName, src => src.MapFrom(p => p.CovidOrganizationName))
                .ForMember(p => p.Email, src => src.MapFrom(p => p.Email))
                .ForMember(p => p.Name, src => src.MapFrom(p => p.Name))
                .ForMember(p => p.Quantity, src => src.MapFrom(p => p.Quantity))
                .ForMember(p => p.PersonalPhone, src => src.MapFrom(p => p.PersonalPhone))
                .ForMember(p => p.OrganizationPhone, src => src.MapFrom(p => p.OrganizationPhone))
                .ForMember(p => p.OrganizationPhoneIntern, src => src.MapFrom(p => p.OrganizationPhoneIntern))
                .ForMember(p => p.Surname, src => src.MapFrom(p => p.Surname))
                .ForMember(p => p.Country, src => src.MapFrom(p => p.Country))
                .ForMember(p => p.State, src => src.MapFrom(p => p.State))
                .ForMember(p => p.City, src => src.MapFrom(p => p.City))
                .ForMember(p => p.Address, src => src.MapFrom(p => p.Address))
                .ForMember(p => p.Address2, src => src.MapFrom(p => p.Address2))
                .ForMember(p => p.Token, src => src.MapFrom(p => p.Token))
                .ForMember(p => p.Location, src => src.MapFrom(p => p.Location))
                .ReverseMap();

            CreateMap<CovidEmbajadorEntregableViewModel, COVIDEmbajadorEntregable>()
                .ForPath(p => p.Ambassador.Id, src => src.MapFrom(p => p.AmbassadorId))
                .ForMember(p => p.CantEntregable, src => src.MapFrom(p => p.CantEntregable))
                .ForMember(p => p.TipoEntregable, src => src.MapFrom(p => p.TipoEntregable))
                .ForMember(p => p.Id, src => src.MapFrom(p => p.Id));

            CreateMap<COVIDEmbajadorEntregable, CovidEmbajadorEntregableViewModel>()
                .ForMember(p => p.CantEntregable, src => src.MapFrom(p => p.CantEntregable))
                .ForMember(p => p.TipoEntregable, src => src.MapFrom(p => p.TipoEntregable))
                .ForMember(p => p.Id, src => src.MapFrom(p => p.Id));
        }
    }
}