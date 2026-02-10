using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Controller.Servidor.Integracion.MinTransporte.Entidades
{
    public class RequestInformacionCarga
    {
        public InformacionDeCarga root { get; set; }
    }

    public class InformacionDeCarga
    {
        public Acceso acceso { get; set; }

        public Solicitud solicitud { get; set; }

        public VariablesInformacionDeCarga variables { get; set; }
    }

    public class VariablesInformacionDeCarga
    {
        public long NUMNITEMPRESATRANSPORTE { get; set; }
        public string CONSECUTIVOINFORMACIONCARGA { get; set; }
        public string CODOPERACIONTRANSPORTE { get; set; }
        public string CODNATURALEZACARGA { get; set; }
        public long? CANTIDADINFORMACIONCARGA { get; set; }
        public long? UNIDADMEDIDACAPACIDAD { get; set; }
        public long? CODTIPOEMPAQUE { get; set; }
        public string MERCANCIAINFORMACIONCARGA { get; set; }
        public string DESCRIPCIONCORTAPRODUCTO { get; set; }
        public string CODTIPOIDREMITENTE { get; set; }
        public long? NUMIDREMITENTE { get; set; }
        public long? CODSEDEREMITENTE { get; set; }
        public long? PESOCONTENEDORVACIO { get; set; }
        public string CODTIPOIDDESTINATARIO { get; set; }
        public long? NUMIDDESTINATARIO { get; set; }
        public string CODSEDEDESTINATARIO { get; set; }
        public string PACTOTIEMPOCARGUE { get; set; }
        public long? HORASPACTOCARGA { get; set; }
        public long? MINUTOSPACTOCARGA { get; set; }
        public string PACTOTIEMPODESCARGUE { get; set; }
        public long? HORASPACTODESCARGUE { get; set; }
        public long? MINUTOSPACTODESCARGUE { get; set; }
        public long? OBSERVACIONES { get; set; }
        public string FECHACITAPACTADACARGUE { get; set; }
        public string HORACITAPACTADACARGUE { get; set; }
        public string FECHACITAPACTADADESCARGUE { get; set; }
        public string HORACITAPACTADADESCARGUEREMESA { get; set; }
    }
}
