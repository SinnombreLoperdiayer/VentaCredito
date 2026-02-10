using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Controller.Servidor.Integracion.MinTransporte.Entidades
{
    public class RequestAnularRemesaTerrestreCarga
    {
        public AnularRemesaTerrestreCarga root { get; set; }
    }
    public class AnularRemesaTerrestreCarga 
    {
        public Acceso acceso { get; set; }

        public Solicitud solicitud { get; set; }

        public VariablesAnularRemesaTerrestreCarga variables { get; set; }
    }
    public class VariablesAnularRemesaTerrestreCarga
    {
        public long NUMNITEMPRESATRANSPORTE { get; set; }
        public string CONSECUTIVOREMESA { get; set; }
        public string MOTIVOREVERSAREMESA { get; set; }
        public string MOTIVOANULACIONINFOVIAJE { get; set; }
    }
}
