using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Controller.Servidor.Integracion.MinTransporte.Entidades
{
    public class RequestAnularManifiestoCarga
    {
        public AnularManifiestoCarga root { get; set; }
    }

    public class AnularManifiestoCarga
    {
        public Acceso acceso { get; set; }

        public Solicitud solicitud { get; set; }

        public VariablesAnularManifiestoCarga variables { get; set; }
    }

    public class VariablesAnularManifiestoCarga
    {
        public long NUMNITEMPRESATRANSPORTE { get; set; }
        public string NUMMANIFIESTOCARGA { get; set; }
        public string MOTIVOANULACIONMANIFIESTO { get; set; }
    }

}
