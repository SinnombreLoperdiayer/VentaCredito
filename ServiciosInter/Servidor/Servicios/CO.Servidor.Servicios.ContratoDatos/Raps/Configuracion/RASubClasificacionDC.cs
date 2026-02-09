using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RASubClasificacionDC
    {
        [DataMember]
        public int IdSubclasificacion { get; set; }

        [DataMember]
        public string Descripcion { get; set; }

        [DataMember]
        public int IdClasificacion { get; set; }

        [DataMember]
        public bool Estado { get; set; }
    } 
}
