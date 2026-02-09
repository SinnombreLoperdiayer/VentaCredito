using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Integraciones.Satrack
{
    [DataContract(Namespace = "")]
    public class INTraficoDC
    {
        [DataMember]
        public string Agencia { get; set; }
        [DataMember]
        public string Planviaje { get; set; }
        [DataMember]
        public string Campo1 { get; set; }
        [DataMember]
        public string Nombreconductor { get; set; }
        [DataMember]
        public string Telefonoconductor { get; set; }
    }
}