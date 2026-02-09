using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiciosInter.DatosCompartidos.EntidadesNegocio.Mensajeria
{
    public class ADArchivoRadicadoDC
    {        
        public long IdArchivo { get; set; }
             
        public string NombreAdjunto { get; set; }
                
        public DateTime FechaGrabacion { get; set; }
                
        public string NombreCompleto { get; set; }
                
        public string NombreServidor { get; set; }
    }
}
