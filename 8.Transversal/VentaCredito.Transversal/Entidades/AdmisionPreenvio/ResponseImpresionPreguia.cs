using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VentaCredito.Transversal.Entidades.AdmisionPreenvio
{
    public class ResponseImpresionPreguia
    {
        public DateTime FechaEjcucion { get; set; }
        public byte[] PdfGuias { get; set; }
        public string MsjError { get; set; }
        public List<long> LtsPreenviosNoIncluidos { get; set; }

    }
}
