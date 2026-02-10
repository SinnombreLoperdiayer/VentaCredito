using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CO.Servidor.Servicios.WebApi.ModelosRequest.Mensajeria
{
    public class AdmisionRequest
    {
        public ADGuia Guia { get; set; }
        public int idCaja { get; set; }
        public ADMensajeriaTipoCliente RemitenteDestinatario { get; set; }
        public AdmisionNotificacionRequest Notificacion { get; set; }
        public AdmisionRapiRadicadoRequest Radicado { get; set; }
    }
}
