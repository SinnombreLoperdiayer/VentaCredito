
using System.Runtime.Serialization;
namespace CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RAFormatoDC
    {
        [DataMember]
        public int IdFormato { get; set; }

        [DataMember]
        public string Descripcion { get; set; }

        [DataMember]
        public bool Estado { get; set; }

        [DataMember]
        public int IdSistemaFormato { get; set; }
    } 
}
