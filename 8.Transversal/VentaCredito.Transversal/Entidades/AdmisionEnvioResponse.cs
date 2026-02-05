using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VentaCredito.Transversal.Entidades
{
    public class AdmisionEnvioResponse
    {
        public long NumeroGuia { get; set; }
        public int EstadoGuia { get; set; }
        public DateTime  FechaAdmision { get; set; }
        public byte[] pdfBytes { get; set; }
    }
}
