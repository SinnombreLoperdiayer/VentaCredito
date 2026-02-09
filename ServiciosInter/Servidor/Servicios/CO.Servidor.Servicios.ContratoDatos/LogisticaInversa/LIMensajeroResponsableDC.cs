using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.LogisticaInversa
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class LIMensajeroResponsableDC : DataContractBase
    {
        [DataMember]
        public string IdMensajero { get; set; }

        [DataMember]
        public string Nombremensajero { get; set; }

        [DataMember]
        public long IdPlanilla { get; set; }

        [DataMember]
        public DateTime FechaGrabacion { get; set; }
    }
}
