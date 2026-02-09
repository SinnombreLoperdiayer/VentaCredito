using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CO.Servidor.Servicios.WebApi.ModelosRequest.Mensajeria
{
    public class AdmisionNotificacionRequest
    {
        public string IdTipoDestino { get; set; }
        public string DescripcionTipoDestino { get; set; }
        public string IdLocalidadDestino { get; set; }
        public string NombreLocalidadDestino { get; set; }
        public string TipoIdentificacionDestinatario { get; set; }
        public string IdDestinatario { get; set; }
        public string NombreDestinatario { get; set; }
        public string Apellido1Destinatario { get; set; }
        public string Apellido2Destinatario { get; set; }
        public string TelefonoDestinatario { get; set; }
        public string DireccionDestinatario { get; set; }
        public string EmailDestinatario { get; set; }
        public bool ReclamaEnOficina { get; set; }
    }
}
