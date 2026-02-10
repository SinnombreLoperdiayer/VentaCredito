
using System.Runtime.Serialization;
namespace CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RAEstadosDC
    {
        [DataMember]
        public int IdEstado { get; set; }

        [DataMember]
        public string Descripcion { get; set; }

        [DataMember]
        public bool Estado { get; set; }
    }
}
