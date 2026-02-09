using Framework.Servidor.Servicios.ContratoDatos;
using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Comun
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class COTipoNovedadGuiaDC : DataContractBase
    {
        [DataMember]
        public short IdTipoNovedadGuia { get; set; }

        [DataMember]
        public string Descripcion { get; set; }

        [DataMember]
        public COEnumTipoNovedad TipoNovedad { get; set; }

        [DataMember]
        public int TiempoAfectacion { get; set; }

        [DataMember]
        public string ToolTip { get; set; }

    }
}
