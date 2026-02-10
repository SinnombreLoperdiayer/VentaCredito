using Framework.Servidor.Servicios.ContratoDatos;
using System.Runtime.Serialization;
namespace CO.Servidor.Servicios.ContratoDatos.LogisticaInversa
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class LIEstadisticaTelemercadeoDC : DataContractBase
    {
        [DataMember]
        public long Auditoria { get; set; }

         [DataMember]
        public long Custodia { get; set; }

        [DataMember]
        public long DevolucionRatificada { get; set; }

        [DataMember]
        public long NuevaDireccion { get; set; }

        [DataMember]
        public long ReclameOficina { get; set; }

        [DataMember]
        public long Reenvio { get; set; }

        [DataMember]
        public long Telemercadeo { get; set; }

        [DataMember]
        public long DevolverALaRacol { get; set; }
    }
}
