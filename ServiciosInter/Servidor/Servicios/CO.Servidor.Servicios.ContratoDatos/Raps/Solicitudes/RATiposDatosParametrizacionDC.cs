using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes
{
    [DataContract]
    public class RATiposDatosParametrizacionDC
    {
        [DataMember]
        public int IdTipoDato { get; set; }

        [DataMember]
        public string descripcionTipoDato { get; set; }

        [DataMember]
        public string descripcionParametro { get; set; }

        [DataMember]
        public int IdTipoParametro { get; set; }
    }
}
