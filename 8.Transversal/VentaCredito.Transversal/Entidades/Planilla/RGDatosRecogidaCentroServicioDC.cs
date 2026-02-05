using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VentaCredito.Transversal.Entidades.Planilla
{
    public class RGDatosRecogidaCentroServicioDC
    {
        public long IdCentroServicio { get; set; }

        public string NombreCentroServicio { get; set; }

        public DateTime FechaHoraRecogida { get; set; }

        public string IdentificacionResponsable { get; set; }

        public string NombreResponsable { get; set; }

        public long IdMensajero { get; set; }

        public long IdSolicitudRecogida { get; set; }

        public string FotoMensajero { get; set; }

        public string TelefonoMensajero { get; set; }
    }
}
