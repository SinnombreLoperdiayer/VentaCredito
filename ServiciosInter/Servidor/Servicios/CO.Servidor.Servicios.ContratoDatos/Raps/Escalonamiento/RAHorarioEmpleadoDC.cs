using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Escalonamiento
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RAHorarioEmpleadoDC
    {
        [DataMember]
        public int IdDia { get; set; }

        [DataMember]
        public DateTime HoraEntrada { get; set; }

        [DataMember]
        public DateTime HoraSalida { get; set; }

    }
}
