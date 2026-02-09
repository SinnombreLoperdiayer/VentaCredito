using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Recogidas
{
    [DataContract(Namespace ="http://contrologis.com")]    
    public class RGProgramacionRecogidaFijaDC
    {

        [DataMember]
        public List<RGRecogidasDC> listaRecogidas { get; set; }

        [DataMember]
        public string estadoProgramacion { get; set; }

        [DataMember]
        public string docPersonaResponsable { get; set; }

        [DataMember]
        public string placaVehiculo { get; set; }

        [DataMember]
        public bool asignacionTemporal { get; set; }

        [DataMember]
        public string docPersonaRespTemporal { get; set; }

        [DataMember]
        public DateTime? FechaInicialAsignacionTemp { get; set; }

        [DataMember]
        public DateTime? FechaFinalAsignacionTemp { get; set; }

        [DataMember]
        public long IdAsignacion { get; set; }


    }
}
