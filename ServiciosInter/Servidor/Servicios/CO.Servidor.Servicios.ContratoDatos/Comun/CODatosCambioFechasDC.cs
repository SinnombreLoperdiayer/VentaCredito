using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Comun
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class CODatosCambioFechasDC
    {
        [DataMember]
        public DateTime FechaEstimadaEntregaNew { get; set; }

        [DataMember]
        public TATiempoDigitalizacionArchivo Tiempos { get; set; }
    }
}
