using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Controller.Servidor.Integracion.MinTransporte.Entidades
{
    public class RequestSuspenderRemesa
    {
        public SuspenderRemesa root { get; set; }
    }

    public class SuspenderRemesa
    {
        public Acceso acceso { get; set; }

        public Solicitud solicitud { get; set; }

        public VarSuspenderRemesa variables { get; set; }
    }

    public class VarSuspenderRemesa
    {
        public long NUMNITEMPRESATRANSPORTE { get; set; }
        public string CONSECUTIVOREMESA { get; set; }
        public string NUMMANIFIESTOCARGA { get; set; }
        public string CODOPERACIONTRANSPORTECUMPLIDO { get; set; }
        public string MOTIVOSUSPENSIONREMESA { get; set; }
    }
}
