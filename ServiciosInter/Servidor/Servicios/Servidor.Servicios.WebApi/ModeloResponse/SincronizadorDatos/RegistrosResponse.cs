using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CO.Servidor.Servicios.WebApi.ModeloResponse.SincronizadorDatos
{
    public class RegistrosResponse
    {
        
        public string InsertUpdate { get; set; }
        
        public string NombreTabla { get; set; }
        
        public string ActualAnchor { get; set; }
        
        public int BatchActual { get; set; }
        
        public int TotalBatch { get; set; }

        
        public string Error { get; set; }
    }
}
