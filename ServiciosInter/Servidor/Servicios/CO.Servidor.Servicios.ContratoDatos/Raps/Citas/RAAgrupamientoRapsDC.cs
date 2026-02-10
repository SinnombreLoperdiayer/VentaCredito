using System;
using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Citas
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RAAgrupamientoRapsDC
    {
        [DataMember]
        public long IdResponsable { get; set; }

        [DataMember]
        public DateTime FechaInicio { get; set; }

        [DataMember]
        public DateTime FechaFin { get; set; }

        [DataMember]
        public RAEnumTipoAgrupamiento TipoAgrupamiento { get; set; }

        [DataMember]
        public int ConteoRaps { get; set; }

        [DataMember]
        public DateTime FechaRaps { get; set; }

        [DataMember]
        public string DescripcionRaps { get; set; }
    }
}
