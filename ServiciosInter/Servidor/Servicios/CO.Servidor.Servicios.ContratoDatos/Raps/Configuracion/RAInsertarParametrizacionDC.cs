using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion
{
    public class RAInsertarParametrizacionDC 
    {
        public RAParametrizacionRapsDC ParametrizacionRaps { get; set; }
        public List<RAEscalonamientoDC> lstEscalonamiento { get; set; }
        public List<RATiempoEjecucionRapsDC> lstTiempoEjecucion { get; set; }
        public List<RAParametrosParametrizacionDC> lstParametros { get; set; }
    }
}
