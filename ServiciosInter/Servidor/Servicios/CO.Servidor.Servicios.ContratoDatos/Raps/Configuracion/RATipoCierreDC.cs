
using System.Runtime.Serialization;
namespace CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RATipoCierreDC
    {
        [DataMember]
        public int IdTipoCierre { get; set; }

        [DataMember]
        public string Descripcion { get; set; }

        [DataMember]
        public bool Estado { get; set; }
    } 
}
