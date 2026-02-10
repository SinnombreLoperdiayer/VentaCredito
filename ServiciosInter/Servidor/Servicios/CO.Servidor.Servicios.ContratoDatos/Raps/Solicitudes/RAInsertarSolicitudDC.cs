using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes
{
    public class RAInsertarSolicitudDC
    {
        public RASolicitudDC InsertarSolicitud { set; get; }
        public List<RAGestionDC> InsertarListaGestion { get; set; }
        public List<RAAdjuntoDC> InsertarListaAdjuntos { get; set; }
        public Dictionary<string, object> parametrosParametrizacion { get; set; }
    }
}
