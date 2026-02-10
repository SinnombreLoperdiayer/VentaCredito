using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Recogidas
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RGTelemercadeo
    {
        [DataMember]
        public long IdTelemercadeo { get; set; }

        [DataMember]
        public long IdSolRecogida { get; set; }

        [DataMember]
        public string Observacion { get; set; }
    }
}