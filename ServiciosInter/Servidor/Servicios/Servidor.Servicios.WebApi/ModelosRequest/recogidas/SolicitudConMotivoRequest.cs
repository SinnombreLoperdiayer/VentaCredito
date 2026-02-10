using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CO.Servidor.Servicios.ContratoDatos.Recogidas;

namespace CO.Servidor.Servicios.WebApi.ModelosRequest.recogidas
{
    public class SolicitudConMotivoRequest
    {
        public int IdActor { get; set; }
        public int IdMotivo { get; set; }
        public RGAsignarRecogidaDC Solicitud { get; set; }
    }
}