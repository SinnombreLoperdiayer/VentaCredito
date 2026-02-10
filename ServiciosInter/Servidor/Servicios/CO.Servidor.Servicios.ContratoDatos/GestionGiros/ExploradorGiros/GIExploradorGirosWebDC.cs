using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros.Venta;

namespace ServiciosInter.DatosCompartidos.Wrappers
{
    public class ExploradorGirosWebDC
    {
        public long NumeroGiro { get; set; }
        public long IdAdmisionGiro { get; set; }
        public string DigitoVerificacion { get; set; }
        public DateTime FechaGrabacion { get; set; }
        public string Estado { get; set; }
        public long ValorGiro { get; set; }
        public long ValorTotal { get; set; }
        public string CreadoPor { get; set; }
        public string IdTipoIdentificacionRemitente { get; set; }
        public string IdRemitente { get; set; }
        public string NombreRemitente { get; set; }
        public string TelefonoRemitente { get; set; }
        public string EmailRemitente { get; set; }
        public long IdCentroServicioOrigen { get; set; }
        public string NombreCentroServicioOrigen { get; set; }
        public string IdTipoIdentificacionDestinatario { get; set; }
        public string IdDestinatario { get; set; }
        public string NombreDestinatario { get; set; }
        public string TelefonoDestinatario { get; set; }
        public string EmailDestinatario { get; set; }
        public long IdCentroServicioDestino { get; set; }
        public string NombreCentroServicioDestino { get; set; }
        private IList<GIEstadosGirosDC> estadosGiro;
        public string ImagenGiro { get; set; }
    }
}
