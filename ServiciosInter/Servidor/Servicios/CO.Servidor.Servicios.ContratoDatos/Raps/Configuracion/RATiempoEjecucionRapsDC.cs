using System;
using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion
{


    [DataContract(Namespace = "http://contrologis.com")]
    public class RATiempoEjecucionRapsDC
    {
        [DataMember]
        public long NumeroEjecucion { get; set; }

        [DataMember]
        public long IdParametrizacionRap { get; set; }

        [DataMember]
        public int idTipoPeriodo { get; set; }

        [DataMember]
        public int DiaPeriodo { get; set; }

        [DataMember]
        //public TimeSpan Hora { get; set; }
        public DateTime Hora { get; set; }
    } 
}
