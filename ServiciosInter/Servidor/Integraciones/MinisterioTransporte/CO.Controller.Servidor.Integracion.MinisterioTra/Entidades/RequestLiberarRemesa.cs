using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Controller.Servidor.Integracion.MinTransporte.Entidades
{
    public class RequestLiberarRemesa
    {
        public liberarRemesa root { get; set; }
    }

    public class liberarRemesa
    {
        public Acceso acceso { get; set; }

        public Solicitud solicitud { get; set; }

        public VariablesLiberarRemesa variables { get; set; }
    }

    public class VariablesLiberarRemesa
    {
        public long NUMNITEMPRESATRANSPORTE { get; set; }
        public string CONSECUTIVOREMESA { get; set; }
        public string NUMMANIFIESTOCARGA { get; set; }
        public string CODOPERACIONTRANSPORTEREVERSA { get; set; }
        public string CODMUNICIPIOTRANSBORDO { get; set; }
        public string MOTIVOTRANSBORDOREMESA { get; set; }
    }
}
