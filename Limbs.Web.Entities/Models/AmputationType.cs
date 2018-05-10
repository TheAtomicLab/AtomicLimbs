using System.ComponentModel;

namespace Limbs.Web.Entities.Models
{
    public enum AmputationType
    {
        //Mano, No hay diseño
        [Description("Perdí UNA falange de cualquier dedo")]
        A,
        //Mano, No hay diseño
        [Description("Perdí DOS falanges de cualquier dedo")]
        B,
        //Mano
        [Description("Perdí mis cuatro dedos y tengo un pulgar")]
        C,
        //Mano
        [Description("Perdí el pulgar y no tengo los dedos. (Poseo hueso carpal)")]
        D,
        //Brazo
        [Description("Perdí la mano, no tengo muñeca. (Poseo hueso cúbito y radio pero no carpal)")]
        E,
        //Brazo
        [Description("Tengo un muñón a partir del codo y tengo un antebrazo desarrollado. (Huesos cúbito y radio presentes)")]
        F,
        //Brazo, No hay diseño
        [Description("Perdí el codo y tengo el húmero")]
        G,
        //Brazo, No hay diseño
        [Description("Tengo el húmero pero muy poco desarrollado")]
        H
    }

    public static class AmputationTypeExtensions
    {
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