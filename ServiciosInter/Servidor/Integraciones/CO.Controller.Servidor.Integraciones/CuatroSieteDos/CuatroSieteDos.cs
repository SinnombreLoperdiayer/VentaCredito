using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Controller.Servidor.Integraciones.CuatroSieteDos
{
    public class CuatroSieteDos
    {
        public decimal IdTransaccionFallida { get; set; }

        public string Usuario { get; set; }

        public string Contrasena { get; set; }

        private string nitCanalAliado = "8002515697";

        public string NitCanalAliado
        {
            get { return nitCanalAliado; }            
        }

        public string NumeroFactura { get; set; }

        public string CodigoPuntoAdmision { get; set; }

        public string FechaAdmision { get; set; }

        public string TipoIdentificacionRemitente { get; set; }

        public string IdentificadorRemitente { get; set; }

        public string PrimerNombreRemitente { get; set; }

        public string SegundoNombreRemitente { get; set; }

        public string PrimerApellidoRemitente { get; set; }

        public string SegundoApellidoRemitente { get; set; }

        public string IdCiudadRemitente { get; set; }

        public string DireccionRemitente { get; set; }

        public string TelefonoRemitente { get; set; }

        public string CelularRemitente { get; set; }

        public string ValorOperacion { get; set; }

        public string ValorPorte { get; set; }

        public string CodigoPuntoDestino { get; set; }

        /// <summary>
        /// 1	Giro Pagado
        /// 2	Giro Anulado
        /// 3	Giro Cancelado
        /// 4	Giro Pendiente Pago
        /// </summary>
        public string EstadoGiro { get; set; }

        public string TipoIdentificacionDestinatario { get; set; }

        public string IdentificadorDestinatario { get; set; }

        public string PrimerNombreDestinatario { get; set; }

        public string SegundoNombreDestinatario { get; set; }

        public string PrimerApellidoDestinatario { get; set; }

        public string SegundoApellidoDestinatario { get; set; }

        public string IdCiudadDestinatario { get; set; }

        public string DireccionDestinatario { get; set; }

        public string TelefonoDestinatario { get; set; }

        public string CelularDestinatario { get; set; }

        public string FechaPago { get; set; }

        public string FechaRecuperacion { get; set; }

        public string Observaciones { get; set; }
    }
}
