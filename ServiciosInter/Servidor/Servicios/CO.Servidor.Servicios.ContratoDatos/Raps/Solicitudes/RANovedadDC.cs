using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes
{
    [DataContract(Namespace="http://contrologis.com")]
    public class RANovedadDC
    {
        [DataMember]
        public int idTipoNovedad { get; set; }

        [DataMember]
        public string descripcionNovedad { get; set; }

        [DataMember]
        public string ClaveParametro { get; set; }

        [DataMember]
        public Int16 Cantidad { get; set; }
    }
}
