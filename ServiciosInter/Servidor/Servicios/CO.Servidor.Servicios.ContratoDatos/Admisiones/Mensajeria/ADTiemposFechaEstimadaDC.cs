using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class ADTiemposFechaEstimadaDC
    {
        [DataMember]
        public int DuracionTrayectoEnHoras { get; set; }
        [DataMember]
        public int NumeroHorasDigitalizacion { get; set; }
        [DataMember]
        public int NumeroHorasArchivo { get; set; }


      
    }
}
