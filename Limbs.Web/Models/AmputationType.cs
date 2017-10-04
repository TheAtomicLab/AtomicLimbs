using System.ComponentModel;

namespace Limbs.Web.Models
{
    public enum AmputationType
    {
        [Description("Perdí una falange de cualquier dedo")]
        A,
        [Description("Perdí dos falanges de cualquier dedo")]
        B,
        [Description("Perdí mis cuatro dedos y tengo un pulgar")]
        C,
        [Description("Perdí el pulgar y no tengo los dedos (Poseo hueso capital)")]
        D,
        [Description("Perdí dos falanges de cualquier dedo")]
        E,
        [Description("Perdí dos falanges de cualquier dedo")]
        F,
        [Description("Perdí dos falanges de cualquier dedo")]
        G,
        [Description("Perdí dos falanges de cualquier dedo")]
        H
    }
}