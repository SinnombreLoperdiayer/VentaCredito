using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiciosInter.DatosCompartidos.EntidadesNegocio.Recogidas
{
    public class SolicitudConMotivoRequest
    {
        public int IdActor { get; set; }
        public int IdMotivo { get; set; }

        public long IdSolicitudRecogida { get; set; }

        public string LocalidadCambio { get; set; }

        public RGAsignarRecogidaDC Solicitud { get; set; }
    }
}
