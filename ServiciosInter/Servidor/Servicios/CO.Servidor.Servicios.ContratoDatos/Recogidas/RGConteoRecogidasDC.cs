using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Recogidas.Recogidas
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RGConteoRecogidasDC
    {
        [DataMember]
        public long Todas { get; set; }

        [DataMember]
        public long Nuevas { get; set; }

        [DataMember]
        public long Reservadas { get; set; }

        [DataMember]
        public long Canceladas { get; set; }

        [DataMember]
        public long Ejecutadas { get; set; }

        [DataMember]
        public long Forzar { get; set; }

        [DataMember]
        public long FijasAsignar { get; set; }

        [DataMember]
        public long Telemercadeo { get; set; }

        [DataMember]
        public long ForzadaFueraTiempo { get; set; }
    }
}