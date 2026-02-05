using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VentaCredito.Transversal.Entidades.AdmisionPreenvio
{
    public class RequestImpresionPreguia
    {
        public long IdCliente { get; set; }
        public long IdSucursal { get; set; }
        public bool PorRangoFecha { get; set; }
        public List<long> LtsPreguias { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFinal { get; set; }
        public long Formato { get; set; }
    }
}
