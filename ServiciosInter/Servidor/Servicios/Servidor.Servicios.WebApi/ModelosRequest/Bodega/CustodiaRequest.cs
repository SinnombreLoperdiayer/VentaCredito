using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CO.Servidor.Servicios.WebApi.ModelosRequest.Bodega
{
    public class CustodiaRequest
    {
        public PUCustodia Custodia { get; set; }
        public List<string> Adjuntos { get; set; }                
    }    
}
