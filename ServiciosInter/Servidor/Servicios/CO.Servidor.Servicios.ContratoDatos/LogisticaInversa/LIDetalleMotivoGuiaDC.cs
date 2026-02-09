using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.LogisticaInversa
{
    /// <summary>
    /// Obtiene la informacion de los motivos de la guia
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class LIDetalleMotivoGuiaDC : DataContractBase
    {
        [DataMember]
        public int IdMotivoGuia { get; set; }

        [DataMember]
        public string Motivoguia { get; set; }

        [DataMember]
        public DateTime FechaGrabacion { get; set; }

        [DataMember]
        public string Observacion { get; set; }

        [DataMember]
        public string CreadoPor { get; set; }

    }
}
