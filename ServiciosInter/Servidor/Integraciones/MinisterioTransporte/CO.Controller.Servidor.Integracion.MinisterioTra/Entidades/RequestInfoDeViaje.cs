using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Controller.Servidor.Integracion.MinTransporte.Entidades
{
    public class RequestInfoDeViaje
    {
        public InformacionDeViaje root { get; set; }
    }

    public class InformacionDeViaje 
    {
        public Acceso acceso { get; set; }

        public Solicitud solicitud { get; set; }

        public VariablesInformacionDeViaje variables { get; set; }
    }

    public class VariablesInformacionDeViaje 
    {
        public long NUMNITEMPRESATRANSPORTE { get; set; }
        public string CONSECUTIVOINFORMACIONVIAJE { get; set; }
        public string CODIDCONDUCTOR { get; set; }
        public long NUMIDCONDUCTOR { get; set; }
        public string NUMPLACA { get; set; }
        public string CODMUNICIPIOORIGENINFOVIAJE { get; set; }
        public string CODMUNICIPIODESTINOINFOVIAJE { get; set; }
        public string OBSERVACIONES { get; set; }
        public long VALORFLETEPACTADOVIAJE { get; set; }
        public PreRemesa PREREMESAS { get; set; }
    }


    public class PreRemesa
    {
        public List<ManPreRemesa> MANPREREMESA { get; set; }
    }

    public class ManPreRemesa
    {
        public string CONSECUTIVOINFORMACIONCARGA { get; set; }
    }

}
