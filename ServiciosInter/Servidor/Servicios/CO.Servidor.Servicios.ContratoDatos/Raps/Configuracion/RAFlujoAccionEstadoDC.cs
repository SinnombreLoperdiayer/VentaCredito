using System;
using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RAFlujoAccionEstadoDC
    {
        [DataMember]
        public int IdFlujo { get; set; }

        [DataMember]
        public Byte IdAccion { get; set; }

        [DataMember]
        public int IdEstado { get; set; }

        [DataMember]
        public int IdCargo { get; set; }

        [DataMember]
        public int IdEstadoFinal { get; set; }

        [DataMember]
        public int IdCargoFinal { get; set; }
    } 
}
