using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Comun
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class CONovedadGuiaDC : DataContractBase
    {
        [DataMember]
        public COTipoNovedadGuiaDC TipoNovedad { get; set; }

        [DataMember]
        public long NumeroGuia{ get; set; }
    }
}
