using AutoMapper;

namespace Limbs.Web.ViewModels.Configs
{
    public static class AutoMapperLimbsConfig
    {
        public static void Configure()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<OrderProfile>();
                cfg.AddProfile<EventProfile>();
                cfg.AddProfile<AmputationProfile>();
            });
        }
    }
}