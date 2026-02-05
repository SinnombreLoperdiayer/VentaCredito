using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VentaCredito.Transversal.Entidades.AdmisionPreenvio
{
    public class TAPreciosAgrupadosDC
    {
        public int IdServicio { get; set; }
        public TAPrecioMensajeriaDC Precio { get; set; }
        public TAPrecioCargaDC PrecioCarga { get; set; }
        public string Mensaje { get; set; }
        public string NombreServicio { get; set; }
        public string TiempoEntrega { get; set; }
        public TAFormaPagoServicio FormaPagoServicio { get; set; }
        public DateTime fechaEntrega { get; set; }
    }
}
