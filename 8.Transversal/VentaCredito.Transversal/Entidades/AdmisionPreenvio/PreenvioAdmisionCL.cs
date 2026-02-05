using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VentaCredito.Transversal.Entidades.AdmisionPreenvio
{
    public class PreenvioAdmisionCL
    {
        public long IdPreenvio { get; set; }
        public long NumeroPreenvio { get; set; }
        public short IdEstadoPreenvio { get; set; }
        public string DescripcionEstado { get; set; }
        public decimal Peso { get; set; }
        public short NumeroPieza { get; set; }
        public long IdClienteCredito { get; set; }
        public long CodigoConvenioRemitente { get; set; }
        public DateTime FechaGrabacionEstado { get; set; }
        public string IdCiudadDestino { get; set; }
        public string IdUnidadNegocio { get; set; }
        public int IdServicio { get; set; }
        public string NombreServicio { get; set; }
        public string IdTipoEntrega { get; set; }
        public long IdCentroServicioDestino { get; set; }
        public string IdPaisOrigen { get; set; }
        public string IdCiudadOrigen { get; set; }
        public string CiudadOrigen { get; set; }
        public string CodigoPostalOrigen { get; set; }
        public string IdPaisDestino { get; set; }
        public string NombreCiudadDestino { get; set; }
        public string CodigoPostalDestino { get; set; }
        public string TelefonoDestinatario { get; set; }
        public string DireccionDestinatario { get; set; }
        public string NombreDestinatario { get; set; }
        public string ApellidoDestinatario { get; set; }
        public string EmailDestinatario { get; set; }
        public string TipoCliente { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal ValorDeclarado { get; set; }
        public DateTime FechaPreenvio { get; set; }
        public short IdTipoEnvio { get; set; }
        public string NombreTipoEnvio { get; set; }
        public string IdentificacionRemitente { get; set; }
        public string NombreRemitente { get; set; }
        public string IdTipoIdentificacionDestinatario { get; set; }
        public string IdentificacionDestinatario { get; set; }
        public string DescripcionTipoEntrega { get; set; }
        public string NombreCiudadOrigen { get; set; }
        public decimal ValorAdmision { get; set; }
        public string Observaciones { get; set; }
        public long IdCentroServicioOrigen { get; set; }
        public string NombreCentroServicioOrigen { get; set; }
        public int IdContrato { get; set; }
        public int IdListaPrecios { get; set; }
        public long IdRecogida { get; set; }
        public short IdFormaPago { get; set; }
        public string NombreFormaPago { get; set; }
    }
}
