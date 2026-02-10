using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Controller.Servidor.Integracion.MinTransporte.Entidades
{
    public class RequestConsultaVehiculo
    {
        public ConsultaVehiculo root { get; set; }
    }

    public class ConsultaVehiculo
    {
        public Acceso acceso { get; set; }

        public Solicitud solicitud { get; set; }

        public string variables { get; set; }

        public DocumentoConsultaVehiculo documento { get; set; }
    }

    public class DocumentoConsultaVehiculo
    {
        public string NUMNITEMPRESATRANSPORTE { get; set; }
        public string NUMPLACA { get; set; }
    }
}
