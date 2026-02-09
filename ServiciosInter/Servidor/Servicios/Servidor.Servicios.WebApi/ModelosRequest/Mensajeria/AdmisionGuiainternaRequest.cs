using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CO.Servidor.Servicios.WebApi.ModelosRequest.Mensajeria
{
    public class AdmisionGuiainternaRequest
    {
        public long NumeroGuia {get;set;}
        public long idGestionOrigen { get; set; } 
        public string descripcionGestionOrigen { get; set; } 
        public long idGestionDestino { get; set; } 
        public string descripcionGestionDestino { get; set; }
        public string NombreRemitente { get; set; } 
        public string TelefonoRemitente { get; set; }
        public string DireccionRemitente { get; set; }
        public long IdCentroServicioOrigen { get; set; }
        public string NombreCentroServicioOrigen { get; set; }
        public string IdLocalidadOrigen { get; set; } 
        public string NombreLocalidadOrigen { get; set; }
        public string IdLocalidadDestino { get; set; }
        public string NombreLocalidadDestino { get; set; }
        public string idPaisDefault { get; set; } 
        public string nombrePaisDefault { get; set; }
        public string NombreDestinatario { get; set; }
        public string TelefonoDestinatario { get; set; }
        public string DireccionDestinatario { get; set; }
        public string DiceContener { get; set; }
        public bool EsManual { get; set; }
        public bool EsOrigenGestion { get; set; }
        public bool EsDestinoGestion { get; set; }
        public string CreadoPor { get; set; } 
        public DateTime FechaGrabacion { get; set; }
        public int TiempoEntrega { get; set; }
        public string IdentificacionRemitente { get; set; }
        public string IdentificacionDestinatario { get; set; }
        public string TipoIdentificacionRemitente { get; set; }
        public string TipoIdentificacionDestinatario { get; set; }
        public string EmailRemitente { get; set; }
        public string EmailDestinatario { get; set; }
        public string Observaciones { get; set; }
        public DateTime FechaEstimadaEntrega { get; set; }
    }
}