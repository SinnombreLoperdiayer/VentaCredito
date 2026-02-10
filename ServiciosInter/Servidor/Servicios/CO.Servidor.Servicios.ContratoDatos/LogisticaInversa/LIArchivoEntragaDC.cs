using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.LogisticaInversa
{
    [DataContract(Namespace="http://contrologis.com")]
    public class LIArchivoEntregaDC: DataContractBase
    {
        [DataMember]
        public long NumeroGuia { get; set; }

        [DataMember]
        public long Caja { get; set; }

        [DataMember]
        public long Lote { get; set; }

        [DataMember]
        public long Posicion { get; set; }

        [DataMember]
        public DateTime FechaEntrega { get; set; }

        [DataMember]
        public DateTime FechaGrabacion { get; set; }

        [DataMember]
        public string CreadoPor { get; set; }

        [DataMember]
        public string IdMunicipio { get; set; }

        [DataMember]
        public string Nombre { get; set; }

        [DataMember]
        public DateTime FechaArchivo { get; set; }

        [DataMember]
        public string UsuarioArchiva { get; set; }

        [DataMember]
        public string DatosdeEdicion { get; set; }

        [DataMember]
        public string DatosdeEntrega { get; set; }

        [DataMember]
        public string EstadoFisico { get; set; }

    }
}
