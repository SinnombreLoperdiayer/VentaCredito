using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VentaCredito.Transversal.Entidades.AdmisionPreenvio
{
    public class MotivoDevolucionGuia_MEN
    {
        public long IdEstadoGuiaLog { get; set; }
        public int IdMotivoGuia { get; set; }
        public DateTime FechaMotivo { get; set; }
        public string MotivoDevolucion { get; set; }
    }
}
