using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VentaCredito.Transversal.Entidades.Planilla
{
    public class MensajeroRecogida
    {
        public string TipoIdentificación { get; set; }
        public string NumeroIdentificacion { get; set; } 
        public string Nombre { get; set; }
        public string FotoMensajero { get; set; }
        public string HuellaDactilar { get; set; }
        public string Dedo { get; set; }
        public bool EsManoDerecha{ get; set; }
        public long IdSolicitudRecogida { get; set; }
        public string TelefonoMensajero { get; set; }


    }
}
