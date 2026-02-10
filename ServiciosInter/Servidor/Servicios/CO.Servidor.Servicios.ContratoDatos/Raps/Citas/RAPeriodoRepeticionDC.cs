using System;
using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Citas
{
    [DataContract(Namespace = "http://controllogis.com")]
    public class RAPeriodoRepeticionDC
    {

        [DataMember]
        public long IdPeriodoRepeticion { get; set; }

        [DataMember]
        public RAEnumTipoPeriodo IdTipoPeriodo { get; set; }

        [DataMember]
        public int Intervalo { get; set; }

        [DataMember]
        public DateTime? Lunes { get; set; }

        [DataMember]
        public DateTime? Martes { get; set; }

        [DataMember]
        public DateTime? Miercoles { get; set; }

        [DataMember]
        public DateTime? Jueves { get; set; }

        [DataMember]
        public DateTime? Viernes { get; set; }

        [DataMember]
        public DateTime? Sabado { get; set; }

        [DataMember]
        public int DuracionHoras { get; set; }


    }
}
