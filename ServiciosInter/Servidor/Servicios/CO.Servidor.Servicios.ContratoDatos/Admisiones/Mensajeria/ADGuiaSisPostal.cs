using Framework.Servidor.Servicios.ContratoDatos;
using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class ADGuiaSisPostal : DataContractBase
    {
        [DataMember]
        public long NumeroGuia { get; set; }

        [DataMember]
        public string IdLocalidadOrigen { get; set; }

        [DataMember]
        public string IdLocalidadDestino { get; set; }

        [DataMember]
        public string TelefonoDestinatario { get; set; }

        [DataMember]
        public string TelefonoRemitente { get; set; }

    }
}
