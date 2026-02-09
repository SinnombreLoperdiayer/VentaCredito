using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Integraciones.Satrack
{
    [DataContract(Namespace = "")]
    public class INParametrosDC

    {
        [DataMember]
        public INTraficoDC Trafico { get; set; }
        [DataMember]
        public INCanbusDC Canbus { get; set; }
        [DataMember]
        public string Disponible { get; set; }
        [DataMember]
        public string Fechadespacho { get; set; }
        [DataMember]
        public INRegionescargueDC Regionescargue { get; set; }
        [DataMember]
        public string Placatemporal { get; set; }

    }
}