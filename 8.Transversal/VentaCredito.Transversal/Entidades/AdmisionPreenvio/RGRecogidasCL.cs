using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VentaCredito.Transversal.Entidades.AdmisionPreenvio
{
    public class RGRecogidasCL
    {
        public long IdRemitente { get; set; }
        public string NumeroDocumento {get; set; }
        public string Nombre {get; set; }
        public string Direccion {get; set; }
        public string Ciudad {get; set; }
        public string NumeroTelefono {get; set; }
	    public DateTime FechaRecogida { get; set; }
        public int TipoRecogida { get; set; }
	    public string NombreCiudad {get; set; }
        public string Longitud {get; set; }
        public string Latitud {get; set; }
        public string TipoDocumento {get; set; }
        public string NombreCompleto {get; set; }
        public string Correo {get; set; }
        public string PreguntarPor {get; set; }
        public string DescripcionEnvios {get; set; }
        public decimal PesoAproximado { get; set; }
        public int TotalPiezas { get; set; }
        public List<long> NumeroPreenvios { get; set; }
        public int IdClienteCredito { get; set; }
        public int IdSucursalCliente { get; set; }
        public int IdOrigenSolRecogida { get; set; }
    }
}
