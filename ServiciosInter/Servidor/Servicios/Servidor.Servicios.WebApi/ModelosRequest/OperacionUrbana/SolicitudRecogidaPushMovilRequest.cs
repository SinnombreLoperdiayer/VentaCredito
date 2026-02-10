using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CO.Servidor.Servicios.WebApi.ModelosRequest.OperacionUrbana
{
    public class SolicitudRecogidaPushMovilRequest
    {
        public long IdRecogida { get; set; }

        public string TokenDispositivo { get; set; }

        public PAEnumOsDispositivo  SistemaOperativo {get;set;}

        public string IdLocalidad { get; set; }
    }
}
