using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RATerritorialDC
    {
        [DataMember]
        public int IdTerritorial { get; set; }

        [DataMember]
        public string NombreTerritorial { get; set; }

    }
}
