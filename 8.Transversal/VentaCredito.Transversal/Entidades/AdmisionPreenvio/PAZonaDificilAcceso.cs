using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VentaCredito.Transversal.Entidades.AdmisionPreenvio
{
    public class PAZonaDificilAcceso
    {
        public long IdLocalidad { get; set; }

        public string ZonaDescripcion { get; set; }

        public string CreadoPor { get; set; }

        public bool Opcion { get; set; }
    }
}
