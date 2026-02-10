using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Runtime.Serialization;


namespace CO.Servidor.Servicios.ContratoDatos.LogisticaInversa
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class LIAdmisionGuiaFlujoDC : DataContractBase
    {
        [DataMember]
        public long NumeroGuia { get; set; }
        [DataMember]
        public string TipoCliente { get; set; }
        [DataMember]
        public string Creadopor { get; set; }
        [DataMember]
        public bool EsAutomatico { get; set; }
        [DataMember]
        public int DiasDeEntrega { get; set; }
        [DataMember]
        public DateTime? FechaEstimadaEntregaNew { get; set; }
        [DataMember]
        public string NombreCentroServicioOrigen { get; set; }
        [DataMember]
        public string NombreCentroServicioDestino { get; set; }
        [DataMember]
        public DateTime fechaPago { get; set; }
        [DataMember]
        public string NombreAplicacion { get; set; }

    }
}
