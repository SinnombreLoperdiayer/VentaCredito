using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Controller.Servidor.Integracion.MinTransporte.Entidades
{
    public class RequestConsultaTerceros
    {
        public ConsultaTerceros root { get; set; }
    }

    public class ConsultaTerceros
    {
        public Acceso acceso { get; set; }

        public Solicitud solicitud { get; set; }

        public string variables { get; set; }

        public DocumentoConsultaTercero documento { get; set; }
    }

    public class DocumentoConsultaTercero
    {
        public string NUMNITEMPRESATRANSPORTE { get; set; }
        public string CODTIPOIDTERCERO { get; set; }
        public string NUMIDTERCERO { get; set; }
    }
}
