using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CO.Servidor.Servicios.WebApi.ModeloResponse.Raps
{
    public class DetalleParametrizacion
    {
        public RAParametrizacionRapsDC ParametrizacionRaps {get; set; }
        public List<RAEscalonamientoDC> ListarCargos { get; set; }
        public List<RATiempoEjecucionRapsDC> ListarTiempoEjecucionRaps { get; set; }
        public List<RAParametrosParametrizacionDC> lstParametros { get; set; }
    }
}