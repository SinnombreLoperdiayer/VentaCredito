
using System.Runtime.Serialization;
namespace CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RAUsuarioDeGrupoDC
    {
        [DataMember]
        public int IdCargo { get; set; }

        [DataMember]
        public int idUsuarioGrupo { get; set; }
    } 
}
