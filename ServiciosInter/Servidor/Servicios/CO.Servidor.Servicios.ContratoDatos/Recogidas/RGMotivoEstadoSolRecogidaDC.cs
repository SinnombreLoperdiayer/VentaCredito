using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Recogidas
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RGMotivoEstadoSolRecogidaDC
    {
        [DataMember]
        public long IdMotivo { get; set; }

        [DataMember]
        public string DescripcionMotivo { get; set; }

        [DataMember]
        public bool EsFija { get; set; }
    }
}