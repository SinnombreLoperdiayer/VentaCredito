using Framework.Servidor.Comun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RAParametrosSolicitudAcumulativaDC
    {
        [DataMember]
        public Dictionary<string, object> Parametrosparametrizacion { get; set; }

        [DataMember]
        public CoEnumTipoNovedadRaps TipoNovedad { get; set; }

        [DataMember]
        public bool EstaEnviado { get; set; }
    }
}
