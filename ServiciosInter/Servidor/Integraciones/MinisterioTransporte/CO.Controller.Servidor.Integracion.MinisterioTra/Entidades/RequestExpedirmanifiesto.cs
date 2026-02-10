using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Controller.Servidor.Integracion.MinTransporte.Entidades
{
    public class RequestExpedirmanifiesto
    {
        public ExpedirManifiesto root { get; set; }
    }

    public class ExpedirManifiesto
    {
        public Acceso acceso { get; set; }

        public Solicitud solicitud { get; set; }

        public VariablesExpedirManifiesto variables { get; set; }
    }

    public class VariablesExpedirManifiesto
    {
        public long NUMNITEMPRESATRANSPORTE { get; set; }
        public string NUMMANIFIESTOCARGA { get; set; }
        public string CONSECUTIVOINFORMACIONVIAJE { get; set; }
        public string MANNROMANIFIESTOTRANSBORDO { get; set; }
        public string CODOPERACIONTRANSPORTE { get; set; }
        public string FECHAEXPEDICIONMANIFIESTO { get; set; }
        public string CODMUNICIPIOORIGENMANIFIESTO { get; set; }
        public string CODMUNICIPIODESTINOMANIFIESTO { get; set; }
        public string CODIDTITULARMANIFIESTO { get; set; }
        public long? NUMIDTITULARMANIFIESTO { get; set; }
        public string NUMPLACA { get; set; }
        public string CODIDCONDUCTOR { get; set; }
        public long? NUMIDCONDUCTOR { get; set; }
        public string RETENCIONFUENTEMANIFIESTO { get; set; }
        public string RETENCIONICAMANIFIESTOCARGA { get; set; }
        public long? VALORANTICIPOMANIFIESTO { get; set; }
        public long? VALORFLETEPACTADOVIAJE { get; set; }
        public string CODMUNICIPIOPAGOSALDO { get; set; }
        public string FECHAPAGOSALDOMANIFIESTO { get; set; }
        public string CODRESPONSABLEPAGOCARGUE { get; set; }
        public string CODRESPONSABLEPAGODESCARGUE { get; set; }
        public string OBSERVACIONES { get; set; }

        public RemesaMan REMESASMAN { get; set; }
    }

    public class RemesaMan
    {
        public List<Remesa> REMESA { get; set; }
    }

    public class Remesa
    {
        public string CONSECUTIVOREMESA { get; set; }
    }
}
