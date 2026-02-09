using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RAOrigenRapsDC
    {
        [DataMember]
        public int IdOrigenRaps { get; set; }

        [DataMember]
        public string descripcion { get; set; }

        [DataMember]
        public bool Estado { get; set; }
    } 
}
