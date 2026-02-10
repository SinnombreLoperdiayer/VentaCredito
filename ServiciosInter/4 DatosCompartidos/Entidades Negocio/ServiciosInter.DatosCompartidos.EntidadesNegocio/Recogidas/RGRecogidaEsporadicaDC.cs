using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiciosInter.DatosCompartidos.EntidadesNegocio.Recogidas
{
    public class RGRecogidaEsporadicaDC
    {        
        public long? IdSolRecogida { get; set; }
     
        public DateTime FechaHoraRecogida { get; set; }
     
        public DateTime FechaGrabacion { get; set; }
        
        public string DireccionRecogida { get; set; }
        
        public string EstadoRecogida { get; set; }
        
        public string DescripcionEstado { get; set; }
        
        public string Mensajero { get; set; }
        
        public string IdLocalidad { get; set; }
        
        public string Longitud { get; set; }
        
        public string Latitud { get; set; }
        
        public string UrlImage { get; set; }

        public int IdEstadoRecogida { get; set; }
    }
}
