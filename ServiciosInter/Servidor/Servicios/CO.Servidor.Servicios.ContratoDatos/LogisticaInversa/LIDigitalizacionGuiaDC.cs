using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.LogisticaInversa
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class LIDigitalizacionGuiaDC : DataContractBase
    {
        [DataMember]
        public long IdArchivo { get; set; }

        [DataMember]
        public string RutaArchivo { get; set; }

        [DataMember]
        public bool EsSincronizada { get; set; }

        [DataMember]
        public long IdAdminisionMensajeria { get; set; }

        [DataMember]
        public long NumeroGuia { get; set; }

        [DataMember]
        public DateTime FechaGrabacion { get; set; }

        [DataMember]
        public string CreadoPor { get; set; }

        [DataMember]
        public string NombreLocalidad { get; set; }

        [DataMember]
        public string NombreCompleto { get; set; }

        [DataMember]
        public long IdCentroLogistico { get; set; }


    }
}
