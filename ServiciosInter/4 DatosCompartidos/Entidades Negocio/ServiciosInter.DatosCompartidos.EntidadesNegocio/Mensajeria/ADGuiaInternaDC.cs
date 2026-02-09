using ServiciosInter.DatosCompartidos.EntidadesNegocio.Areas;
using ServiciosInter.DatosCompartidos.EntidadesNegocio.Parametros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiciosInter.DatosCompartidos.EntidadesNegocio.Mensajeria
{
    public class ADGuiaInternaDC
    {
        
        public long IdAdmisionGuia { get; set; }
        
        public long NumeroGuia { get; set; }
        
        public ARGestionDC GestionOrigen { get; set; }
        
        public ARGestionDC GestionDestino { get; set; }
        
        public string NombreRemitente { get; set; }
        
        public string TelefonoRemitente { get; set; }
        
        public string DireccionRemitente { get; set; }

        
        public long IdCentroServicioOrigen { get; set; }

        
        public string NombreCentroServicioOrigen { get; set; }
        
        public PALocalidadDC LocalidadOrigen { get; set; }
        
        public PALocalidadDC LocalidadDestino { get; set; }
        
        public PALocalidadDC PaisDefault { get; set; }
        
        public string NombreDestinatario { get; set; }
        
        public string TelefonoDestinatario { get; set; }
        
        public string DireccionDestinatario { get; set; }
        
        public string DiceContener { get; set; }

        
        public long IdCentroServicioDestino { get; set; }

        
        public string NombreCentroServicioDestino { get; set; }

        
        public bool EsManual { get; set; }

        
        public bool EsOrigenGestion { get; set; }

        
        public bool EsDestinoGestion { get; set; }

        
        public string CreadoPor { get; set; }

        
        public DateTime FechaGrabacion { get; set; }

        
        public int TiempoEntrega { get; set; }

        
        public string GuidDeChequeo { get; set; }

        
        public string IdentificacionRemitente { get; set; }

        
        public string IdentificacionDestinatario { get; set; }
        
        public string TipoIdentificacionRemitente { get; set; }
        
        public string TipoIdentificacionDestinatario { get; set; }

        
        public string EmailRemitente { get; set; }

        
        public string EmailDestinatario { get; set; }

        
        public string Observaciones { get; set; }

        
        public ADTipoEntrega TipoEntrega { get; set; }

        
        public DateTime FechaEstimadaEntrega { get; set; }

        
        public string InfoCasillero { get; set; }
    }
}
