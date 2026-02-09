using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Controller.Servidor.Integracion.MinTransporte.Entidades
{
    class RequestCumplirRemesa
    {
        public CumplirRemesa root { get; set; }
    }

    public class CumplirRemesa
    {
        public Acceso acceso { get; set; }

        public Solicitud solicitud { get; set; }

        public VariablesCumplirRemesa variables { get; set; }
    }

    public class VariablesCumplirRemesa
    {
        public long NUMNITEMPRESATRANSPORTE { get; set; }
        public string CONSECUTIVOREMESA { get; set; }
        public string TIPOCUMPLIDOREMESA { get; set; }
        public string MOTIVOSUSPENSIONREMESA { get; set; }
        public long? CANTIDADCARGADA { get; set; }
        public long? CANTIDADENTREGADA { get; set; }
        public string UNIDADMEDIDACAPACIDAD { get; set; }
        public string FECHALLEGADADESCARGUE { get; set; }
        public string HORALLEGADACARGUEREMESA { get; set; }
        public string FECHAENTRADACARGUE { get; set; }
        public string HORAENTRADACARGUEREMESA { get; set; }
        public string FECHASALIDACARGUE { get; set; }
        public string HORASALIDACARGUEREMESA { get; set; }
        public string HORALLEGADADESCARGUECUMPLIDO { get; set; }
        public string FECHAENTRADADESCARGUE { get; set; }
        public string HORAENTRADADESCARGUECUMPLIDO { get; set; }
        public string FECHASALIDADESCARGUE { get; set; }
        public string HORASALIDADESCARGUECUMPLIDO { get; set; }
        public string OBSERVACIONES { get; set; }
        public string INGRESOID { get; set; }
        
    }
}
