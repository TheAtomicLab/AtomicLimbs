using System.ComponentModel;

namespace Limbs.Web.Entities.Models
{
    public enum AmputationType
    {
        //Mano, No hay diseño
        [Description("Perdí una falange de cualquier dedo")]
        A,
        //Mano, No hay diseño
        [Description("Perdí dos falanges de cualquier dedo")]
        B,
        //Mano
        [Description("Perdí mis cuatro dedos y tengo un pulgar")]
        C,
        //Mano
        [Description("Perdí el pulgar y no tengo los dedos (Poseo hueso capital)")]
        D,
        //Brazo
        [Description("Perdí dos falanges de cualquier dedo")]
        E,
        //Brazo
        [Description("Perdí dos falanges de cualquier dedo")]
        F,
        //Brazo, No hay diseño
        [Description("Perdí dos falanges de cualquier dedo")]
        G,
        //Brazo, No hay diseño
        [Description("Perdí dos falanges de cualquier dedo")]
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