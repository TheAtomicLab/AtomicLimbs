using System.ComponentModel;

namespace Limbs.Web.Entities.Models
{
    public enum AmputationType
    {
        //Mano, No hay diseño
        [Description("Perdí UNA falange de cualquier dedo")]
        A = 0,
        //Mano, No hay diseño
        [Description("Perdí DOS falanges de cualquier dedo")]
        B = 1,
        //Mano
        [Description("Perdí mis cuatro dedos y tengo un pulgar")]
        C = 2,
        //Mano
        [Description("Perdí el pulgar y no tengo los dedos. (Poseo hueso carpal)")]
        D = 3,
        //Brazo
        [Description("Perdí la mano, no tengo muñeca. (Poseo hueso cúbito y radio pero no carpal)")]
        E = 4,
        //Brazo
        [Description("Tengo un muñón a partir del codo y tengo un antebrazo desarrollado. (Huesos cúbito y radio presentes)")]
        F = 5,
        //Brazo, No hay diseño
        [Description("Perdí el codo y tengo el húmero")]
        G = 6,
        //Brazo, No hay diseño
        [Description("Tengo el húmero pero muy poco desarrollado")]
        H = 7
    }

    public static class AmputationTypeExtensions
    {
        public static string AmputationDescription(this AmputationType source)
        {
            if (EsBrazo(source))
            { return "Brazo"; }
            else if (EsMano(source))
            { return "Mano"; }
            else
                return "Sin diseño";
        }

        public static bool EsBrazo(this AmputationType source)
        {
            return source == AmputationType.E ||
                   source == AmputationType.F ||
                   source == AmputationType.G ||
                   source == AmputationType.H;
        }

        public static bool EsMano(this AmputationType source)
        {
            return source == AmputationType.A ||
                   source == AmputationType.B ||
                   source == AmputationType.C ||
                   source == AmputationType.D;
        }

        public static bool HayDiseno(this AmputationType source)
        {
            return EsBrazo(source) || EsMano(source);
        }
    }
}