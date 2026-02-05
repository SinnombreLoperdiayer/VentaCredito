using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VentaCredito.Transversal.Entidades.Planilla
{
    public class ConsultaMensajero
    {
        public string TipoIdentificacion { get; set; }
        public string NumeroIdentificacion { get; set; }
        public long IdCentroServicio { get; set; }
        public string HorarioRecogida { get; set; }
    }
}
