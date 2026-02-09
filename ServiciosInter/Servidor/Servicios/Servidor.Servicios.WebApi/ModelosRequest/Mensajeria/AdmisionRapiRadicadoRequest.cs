using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CO.Servidor.Servicios.WebApi.ModelosRequest.Mensajeria
{
    public class AdmisionRapiRadicadoRequest
    {
        public short NumeroFolios { get; set; }
        public string CodigoRapiRadicado { get; set; }
        public string IdTipoDestinoDestinatario { get; set; }
        public string DescripcionTipoDestinoDestinatario { get; set; }
        public string IdCiudadDestino { get; set; }
        public string NombreCiudadDestino { get; set; }
        public string TipoIdDestinatario { get; set; }
        public string IdDestinatario { get; set; }
        public string NombreDestinatario { get; set; }
        public string Apellido1Destinatario { get; set; }
        public string Apellido2Destinatario { get; set; }
        public string TelefonoDestinatario { get; set; }
        public string DireccionDestinatario { get; set; }
        public string EmailDestinatario { get; set; }
    }
}
