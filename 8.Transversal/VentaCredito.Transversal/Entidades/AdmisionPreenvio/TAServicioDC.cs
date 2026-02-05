using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VentaCredito.Transversal.Entidades.AdmisionPreenvio
{
    public class TAServicioDC
    {
        public int IdServicio {get; set;}
        public string Nombre { get; set; }
        public double PesoMinimo { get; set; }
        public double PesoMaximo { get; set; }
        public TAPrecioServicioDC ValorServicio { get; set; }
        public bool AplicaContraPago { get; set; }
        
    }
}
