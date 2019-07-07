using System.ComponentModel;
using Limbs.Web.Entities.Resources;
using Limbs.Web.Entities.Filters;

namespace Limbs.Web.Entities.Models
{
    public enum AmputationType
    {
        //Mano, No hay diseño
        [LocalizedDescription("AmputationType_A", typeof(ModelTexts))]
        A = 0,
        //Mano, No hay diseño
        [LocalizedDescription("AmputationType_B", typeof(ModelTexts))]
        B = 1,
        //Mano
        [LocalizedDescription("AmputationType_C", typeof(ModelTexts))]
        C = 2,
        //Mano
        [LocalizedDescription("AmputationType_D", typeof(ModelTexts))]
        D = 3,
        //Brazo
        [LocalizedDescription("AmputationType_E", typeof(ModelTexts))]
        E = 4,
        //Brazo
        [LocalizedDescription("AmputationType_F", typeof(ModelTexts))]
        F = 5,
        //Brazo, No hay diseño
        [LocalizedDescription("AmputationType_G", typeof(ModelTexts))]
        G = 6,
        //Brazo, No hay diseño
        [LocalizedDescription("AmputationType_H", typeof(ModelTexts))]
        H = 7
    }
}