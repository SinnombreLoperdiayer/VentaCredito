using CO.Servidor.Servicios.ContratoDatos.Recogidas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CO.Servidor.Servicios.WebApi.ModelosRequest.recogidas
{
    public class RecogidaRequest
    {
        public RGAsignarRecogidaDC Recogida { get; set; }
        public int IdSistema { get; set; }
        public int TipoNovedad { get; set; }
        public Dictionary<string, object> Parametros { get; set; }
        public string IdCiudad { get; set; }
    }
}