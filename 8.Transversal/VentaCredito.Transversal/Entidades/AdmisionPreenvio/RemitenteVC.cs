using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VentaCredito.Transversal.Entidades.AdmisionPreenvio
{
    public class RemitenteVC
    {
        public long IdRemitente { get; set; }
        public string TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public string Nombre { get; set; }
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public string Correo { get; set; }
        public DateTime FechaGrabacion { get; set; }
        public string MicroZona { get; set; }
        public string MacroZona { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public string ZonaPostal { get; set; }
        public string IdLocalidad { get; set; }
        public string NombreLocalidad { get; set; }
        public string ComplementoDireccion { get; set; }
        public string LugarRecogidaEnvio { get; set; }
        public string FechaTratamientoDatos { get; set; }
    }
}
