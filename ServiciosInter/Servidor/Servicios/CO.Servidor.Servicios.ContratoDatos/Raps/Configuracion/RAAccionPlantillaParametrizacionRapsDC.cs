using System;
using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RAAccionPlantillaParametrizacionRapsDC
    {
        [DataMember]
        public long IdAccionPlantilla { get; set; }

        [DataMember]
        public long IdParametrizacionRap { get; set; }

        [DataMember]
        public Byte IdAccion { get; set; }

        [DataMember]
        public long IdPlantilla { get; set; }

        [DataMember]
        public bool Estado { get; set; }
    }
}
