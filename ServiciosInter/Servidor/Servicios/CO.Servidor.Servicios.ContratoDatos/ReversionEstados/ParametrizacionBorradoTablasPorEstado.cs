using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.ReversionEstados
{
    [DataContract]
    public class ParametrizacionBorradoTablasPorEstado
    {
        [DataMember]
        public int IdParametroBorradoTabla { get; set; }

        [DataMember]
        public int IdEstadoGuiaDestino { get; set; }

        [DataMember]
        public int IdFuncionRegla { get; set; }

        [DataMember]
        public string DescripcionReglaTabla { get; set; }

        [DataMember]
        public int IdOrdenEjecucion { get; set; }
    }
}
