using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.LogisticaInversa
{
    [DataContract(Namespace="http://contrologis.com")]
    public class LIAsignacionMensajero : DataContractBase
    {
        [DataMember]
        public string EstadoGuia { get; set; }

        [DataMember]
        public long PlanillaAsignacion { get; set; }

        [DataMember]
        public string CedulaMensajero { get; set; }

        [DataMember]
        public string UsuarioDescargue { get; set; }

        [DataMember]
        public DateTime FechaDescargue { get; set; }

        [DataMember]
        public string NombreMensajero { get; set; }

        [DataMember]
        public string UsuarioAsigna { get; set; }

        [DataMember]
        public DateTime FechaAsigna { get; set; }

        [DataMember]
        public long NumeroGuia { get; set; }

        [DataMember]
        public string MotivoGuia { get; set; }

        [DataMember]
        public bool EstaDescargada { get; set; }
        [DataMember]
        public long? NumeroAuditoria { get; set; }

        [DataMember]
        public string Auditor { get; set; }

        [DataMember]
        public DateTime? FechaAuditoria { get; set; }
    }
}
