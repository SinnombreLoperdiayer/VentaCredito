using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Controller.Servidor.Integracion.MinTransporte.Entidades
{
    class RequestConsultaCumplidoRemesa
    {
        public ConsultaCumplidoRemesa root { get; set; }
    }

    public class ConsultaCumplidoRemesa
    {
        public Acceso acceso { get; set; }

        public Solicitud solicitud { get; set; }

        public string variables { get; set; }

        public DocumentoConsultaCumplidoRemesa documento { get; set; }
    }

    public class DocumentoConsultaCumplidoRemesa
    {
        public string NUMNITEMPRESATRANSPORTE { get; set; }
        public string INGRESOID { get; set; }
    }
}
