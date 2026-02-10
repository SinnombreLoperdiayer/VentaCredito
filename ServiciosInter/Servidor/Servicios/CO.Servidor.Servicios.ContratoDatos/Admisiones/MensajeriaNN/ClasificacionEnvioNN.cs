using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.MensajeriaNN
{
    [DataContract(Namespace = "http://contrologis.com")]

    public class ClasificacionEnvioNN
    {
        [DataMember]
        public int IdClasificacion { get; set; }
        [DataMember]
        public string Clasificacion { get; set; }
        [DataMember]
        public DateTime FechaCreado { get; set; }
        [DataMember]
        public string CreadoPor { get; set; }
    }
}
