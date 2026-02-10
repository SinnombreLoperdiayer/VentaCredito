using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Integraciones
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class INTArchivoPruebaEntrega : DataContractBase
    {
        [DataMember]
        public long IdRegistroTransEcapture { get; set; }

        [DataMember]
        public DateTime FechaRegistro { get; set; }

        [DataMember]
        public string IdCodigoProceso { get; set; }

        [DataMember]
        public DateTime FechaRecepcion { get; set; }

        [DataMember]
        public DateTime FechaLeida { get; set; }

        [DataMember]
        public bool EstadoEnvio { get; set; }

        [DataMember]
        public long NumeroFormato { get; set; }
    }
}
