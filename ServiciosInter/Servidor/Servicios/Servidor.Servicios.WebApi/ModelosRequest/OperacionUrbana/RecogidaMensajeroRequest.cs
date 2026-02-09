using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CO.Servidor.Servicios.WebApi.ModelosRequest.OperacionUrbana
{
    public class RecogidaMensajeroRequest
    {
        public long IdRecogida { get; set; }
        public long IdMensajero { get; set; }
        public long IdProgramacionSolicitudRecogida { get; set; }
        
        public string LatitudMensajero { get; set; }


        public string LongitudMensajero { get; set; }
    }
}
