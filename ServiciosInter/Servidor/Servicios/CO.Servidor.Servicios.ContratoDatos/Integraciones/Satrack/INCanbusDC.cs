using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Integraciones.Satrack
{
    [DataContract(Namespace = "")]
    public class INCanbusDC
    {
        [DataMember]
        public string Canpeso { get; set; }

    }
}