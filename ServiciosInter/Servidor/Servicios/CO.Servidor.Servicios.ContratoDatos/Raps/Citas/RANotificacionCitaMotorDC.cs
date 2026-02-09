using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Citas
{
    public class RANotificacionCitaMotorDC
    {
        public long IdCita { get;set;}

        public long IdNotificacionCita { get; set; }

        public string CorreoNotificar { get; set; }

        public long DocumentoIntegrante { get; set; }

        public DateTime FechaInicioCita { get; set; }

        public string DescripcionCita { get; set; }

        public string LugarCita { get; set; }
    }
}
