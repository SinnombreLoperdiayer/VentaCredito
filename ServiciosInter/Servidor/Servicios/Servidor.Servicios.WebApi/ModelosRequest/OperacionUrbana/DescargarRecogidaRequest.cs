using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CO.Servidor.Servicios.WebApi.ModelosRequest.OperacionUrbana
{
    public class DescargarRecogidaRequest
    {

        public long IdRecogida { get; set; }

        public int IdMotivo { get; set; }

        public string DescripcionMotivo { get; set; }

        public bool MotivoPermiteReprogramar { get; set; }

        public string Longitud { get; set; }
        
        public string Latitud  { get; set; }

        public string Observaciones { get; set; }
        
        public long IdAsignacion { get; set; }

        public string IdCiudad { get; set; }

        public string TokenDispositivo { get; set; }

    }
}
