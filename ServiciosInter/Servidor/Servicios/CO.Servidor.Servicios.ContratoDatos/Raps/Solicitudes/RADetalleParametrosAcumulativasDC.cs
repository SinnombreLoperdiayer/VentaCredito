using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RADetalleParametrosAcumulativasDC
    {
        [DataMember]
        public string Valor { get; set; }
        [DataMember]
        public int TipoDato { get; set; }
        [DataMember]
        public string Descripcion { get; set; }
    }
}
