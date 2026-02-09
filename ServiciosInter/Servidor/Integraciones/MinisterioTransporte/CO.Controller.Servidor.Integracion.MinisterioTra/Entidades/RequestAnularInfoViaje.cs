using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Controller.Servidor.Integracion.MinTransporte.Entidades
{
    public class RequestAnularInfoViaje
    {
        public AnularInfoViaje root{ get; set; }
    }
    public class AnularInfoViaje
    {
        public Acceso acceso { get; set; }

        public Solicitud solicitud { get; set; }

        public VariablesAnularInfoViaje variables { get; set; }
    }

    public class VariablesAnularInfoViaje
    {
        public long NUMNITEMPRESATRANSPORTE { get; set; }
        public string CONSECUTIVOINFORMACIONVIAJE { get; set; }
        public string MOTIVOANULACIONINFOVIAJE { get; set; }
    }
}
