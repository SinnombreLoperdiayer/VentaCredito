using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RANivelFallaDC
    {
        [DataMember]
        public int IdNivelFalla { get; set; }

        [DataMember]
        public string Descripcion { get; set; }
    }
}
