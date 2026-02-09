using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Controller.Servidor.Integracion.MinTransporte.Entidades
{
    class RequestAnularInfoCarga
    {
        public AnularInfoCarga root { get; set; }
    }

    public class AnularInfoCarga
    {
        public Acceso acceso { get; set; }

        public Solicitud solicitud { get; set; }

        public VariablesAnularInfoCarga variables { get; set; }
    }

    public class VariablesAnularInfoCarga
    {
        public long NUMNITEMPRESATRANSPORTE { get; set; }
        public string CONSECUTIVOINFORMACIONCARGA { get; set; }
        public string MOTIVOANULACIONINFOCARGA { get; set; }
    }
}
