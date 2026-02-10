using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.LogisticaInversa
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class LIHistorialEntregaDC : DataContractBase
    {
        [DataMember]
        public long IdDireccion { get; set; }

        [DataMember]
        public string Direccion { get; set; }

        [DataMember]
        public string Idlocalidad { get; set; }

        [DataMember]
        public bool EsVerificada { get; set; }

        [DataMember]
        public string CiudadDestino { get; set; }

        [DataMember]
        public long NumeroGuia { get; set; }

        [DataMember]
        public string Estado { get; set; }

        [DataMember]
        public DateTime FechaEntrega { get; set; }



    }
}
