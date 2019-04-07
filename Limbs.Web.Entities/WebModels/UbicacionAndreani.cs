using System.Data.Entity.Spatial;

namespace Limbs.Web.Entities.WebModels
{
    public class UbicacionAndreani
    {
        public string Descripcion { get; set; }
        public string Direccion { get; set; }
        public string HoradeTrabajo { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public string Mail { get; set; }
        public string Numero { get; set; }
        public string Responsable { get; set; }
        public string Resumen { get; set; }
        public string Sucursal { get; set; }
        public string Telefono1 { get; set; }
        public string Telefono2 { get; set; }
        public string Telefono3 { get; set; }
        public string TipoSucursal { get; set; }
        public string TipoTelefono1 { get; set; }
        public string TipoTelefono2 { get; set; }
        public string TipoTelefono3 { get; set; }
        public DbGeography Location { get; set; }
    }
}