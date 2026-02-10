using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Recogidas
{
    public class RGRecogidaMotorDC
    {

        public long IdSolicitudRecogida { get; set; }

        public string IdLocalidadRecogida { get; set; }       

        public string Direccionrecogida { get; set; }
        public EnumAccionMotor Accion { get; set; }

        public bool EsNueva { get; set; }
    }
}
