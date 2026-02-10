using CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes;
using System.Collections.Generic;

namespace CO.Servidor.Servicios.WebApi.ModeloResponse.Raps
{
    public class DestalleSolicitud
    {
        public RASolicitudConsultaDC Solicitud { get; set; }
        public List<RAGestionDC> Gestiones { get; set; }
    }
}