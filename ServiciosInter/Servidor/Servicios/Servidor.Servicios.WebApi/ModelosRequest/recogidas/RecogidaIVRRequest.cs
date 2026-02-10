using CO.Servidor.Servicios.ContratoDatos.Recogidas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CO.Servidor.Servicios.WebApi.ModelosRequest.recogidas
{
    public class RecogidaIVRRequest
    {
        public int Anio { get; set; }
        public int Mes { get; set; }
        public int Dia { get; set; }
        public int Hora { get; set; }
        public int Minuto { get; set; }

        public RGRecogidasDC Recogida { get; set; }
    }
}