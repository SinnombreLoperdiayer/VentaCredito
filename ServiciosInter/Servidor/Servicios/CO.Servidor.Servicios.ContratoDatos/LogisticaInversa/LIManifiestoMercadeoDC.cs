using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.LogisticaInversa
{
    /// <summary>
    /// Clase con el datacontract Manifiesto para mercadeo
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class LIManifiestoMercadeoDC : DataContractBase
    {
        [DataMember]
        public long? GuiaInterna { get; set; }

        [DataMember]
        public long NumeroGuia { get; set; }

        [DataMember]
        public bool EstaDescargada { get; set; }

        [DataMember]
        public DateTime FechaGrabacion { get; set; }

        [DataMember]
        public string UsuarioManifesto { get; set; }

        [DataMember]
        public long IdManfiestoOperaNacioConso { get; set; }

        [DataMember]
        public long IdAdminisionMensajeria { get; set; }

        [DataMember]
        public long IdManifiestoOperacionNacio { get; set; }

        [DataMember]
        public string LocalidadManifestada { get; set; }

        [DataMember]
        public int TipoConsolidadoDetalle { get; set; }

        [DataMember]
        public string DescpConsolidadoDetalle { get; set; }

        [DataMember]
        public string NumeroContenedorTula { get; set; }

        [DataMember]
        public long? IdGuiaInterna { get; set; }

        [DataMember]
        public long? NumeroPrecintoSalida { get; set; }

        [DataMember]
        public string NombreLocalidadDespacho { get; set; }

        [DataMember]
        public long MON_IdManifiestoOperacionNacional { get; set; }

        [DataMember]
        public DateTime FechaDescargue { get; set; }

        [DataMember]
        public bool EstaDescargado { get; set; }

        [DataMember]
        public string Placa { get; set; }

        [DataMember]
        public string NombreConductor { get; set; }

        [DataMember]
        public long NumeroManifiestoCarga { get; set; }

        [DataMember]
        public int IdEmpresaTransportadora { get; set; }

        [DataMember]
        public string ETR_Nombre { get; set; }

        [DataMember]
        public string RUT_Nombre { get; set; }

        [DataMember]
        public long? MOT_IdManifiestoOperacionNacio { get; set; }

    }
}
