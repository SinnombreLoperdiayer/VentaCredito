using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Citas
{
    [DataContract(Namespace = "http://controllogis.com")]
    public class RAIntegranteCitaDC
    {
        [DataMember]
        public long DocumentoIntegrante { get; set; }

        [DataMember]
        public long IdCita { get; set; }

        [DataMember]
        public RAEnumTipoIntegrante IdTipoIntegrante { get; set; }

        [DataMember]
        public string DescripcionTipo { get; set; }

        [DataMember]
        public bool EsModerador { get; set; }

        [DataMember]
        public string TipoIntegrante { get; set; }

    }
}
