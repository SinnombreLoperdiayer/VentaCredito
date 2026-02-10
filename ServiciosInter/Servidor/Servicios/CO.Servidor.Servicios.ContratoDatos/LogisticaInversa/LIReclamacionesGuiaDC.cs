using Framework.Servidor.Servicios.ContratoDatos;
using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.LogisticaInversa
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class LIReclamacionesGuiaDC : DataContractBase
    {
        [DataMember]
        public long idsolicitud { get; set; }

        [DataMember]
        public short IdMedioRecepSolicitud { get; set; }

        [DataMember]
        public string MedioRecepSolicitud { get; set; }

        [DataMember]
        public short IdTipoSolicitud { get; set; }

        [DataMember]
        public string TipoSolicitud { get; set; }

        [DataMember]
        public short IdSubtipoSolicitud { get; set; }

        [DataMember]
        public string SubtipoSolicitud { get; set; }

        [DataMember]
        public string Reclamante { get; set; }

        [DataMember]
        public string Descripcion { get; set; }


    }
}
