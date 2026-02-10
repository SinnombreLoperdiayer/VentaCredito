using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.LogisticaInversa
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class LIFlujoGuiaDC : DataContractBase
    {
        [DataMember]
        public string IdLocalidad { get; set; }

        [DataMember]
        public string Ciudad { get; set; }

        [DataMember]
        public string Usuario { get; set; }
        
        [DataMember]
        public DateTime FechaEstado { get; set; }

        [DataMember]
        public long IdEstado { get; set; }

        [DataMember]
        public string DescripcionEstado { get; set; }

        [DataMember]
        public long NumeroGuia { get; set; }

        [DataMember]
        public long IdEstadoLog { get; set; }
        
    }
}
