using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion
{
    [DataContract (Namespace="http://contrologis.com")]
    public class RAParametrosParametrizacionDC
    {
        [DataMember]
        public int idParametro { get; set; }

        [DataMember]
        public long idParametrizacionRap { get; set; }

        [DataMember]
        public string descripcionParametro { get; set; }

        [DataMember]
        public int idTipoDato { get; set; }

        [DataMember]
        public int longitud { get; set; }

        [DataMember]
        public int? idTipoNovedad { get; set; }

        [DataMember]
        public string Valor { get; set; }

        [DataMember]
        public bool EsAgrupamiento { get; set; }

        [DataMember]
        public long IdSolicitud { get; set; }

        [DataMember]
        public string DescripcionSolicitud { get; set; }

        [DataMember]
        public bool EsEncabezadoDescripcion { get; set; }

        [DataMember]
        public bool DescripcionReporte { get; set; }
    }
}
