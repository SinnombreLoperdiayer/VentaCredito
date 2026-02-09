using ServiciosInter.DatosCompartidos.EntidadesNegocio.Comun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiciosInter.DatosCompartidos.EntidadesNegocio.Areas
{
    public class ARGestionDC
    {
        public long IdGestion { get; set; }
        
        public string Descripcion { get; set; }
        
        public int IdCasaMatriz { get; set; }
                
        public string IdMacroProceso { get; set; }
        
        public string CentroCostos { get; set; }
        
        public string CodigoBodegaERP { get; set; }
        
        public string IdGestionExterno { get; set; }

        
        public EnumEstadoRegistro EstadoRegistro { get; set; }
    }
}
