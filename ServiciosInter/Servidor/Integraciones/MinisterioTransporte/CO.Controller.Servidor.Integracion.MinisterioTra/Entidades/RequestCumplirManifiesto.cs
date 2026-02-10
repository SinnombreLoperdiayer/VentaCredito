using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Controller.Servidor.Integracion.MinTransporte.Entidades
{
    public class RequestCumplirManifiesto
    {
        public CumplirManifiesto root { get; set; }
    }

    public class CumplirManifiesto
    {
        public Acceso acceso { get; set; }

        public Solicitud solicitud { get; set; }

        public VariablesCumplirManifiesto variables { get; set; }
    }

    public class VariablesCumplirManifiesto
    {
        public long NUMNITEMPRESATRANSPORTE { get; set; }
        public string NUMMANIFIESTOCARGA { get; set; }
        public string TIPOCUMPLIDOMANIFIESTO { get; set; }
        public string MOTIVOSUSPENSIONMANIFIESTO { get; set; }
        public string CONSECUENCIASUSPENSION { get; set; }
        public string FECHAENTREGADOCUMENTOS { get; set; }
        public long? VALORADICIONALHORASCARGUE { get; set; }
        public long? VALORADICIONALHORASDESCARGUE { get; set; }
        public long? VALORADICIONALFLETE { get; set; }
        public string MOTIVOVALORADICIONAL { get; set; }
        public long? VALORDESCUENTOFLETE { get; set; }
        public string MOTIVOVALORDESCUENTOMANIFIESTO { get; set; }
        public long? VALORSOBREANTICIPO { get; set; }
        public string OBSERVACIONES { get; set; }
        public string INGRESOID { get; set; }
    }
}
