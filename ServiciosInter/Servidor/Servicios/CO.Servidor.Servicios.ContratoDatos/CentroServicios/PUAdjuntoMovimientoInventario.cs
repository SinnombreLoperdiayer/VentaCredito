using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.CentroServicios
{
      [DataContract(Namespace = "http://contrologis.com")]
    public class PUAdjuntoMovimientoInventario : DataContractBase
    {
        [DataMember]
        public long IdAdjunto { get; set; }
        [DataMember]
        public PUMovimientoInventario MovimientoInventario { get; set; }
        [DataMember]
        public string RutaAdjunto { get; set; }
        [DataMember]
        public string FormatoAdjunto { get; set; }
        [DataMember]
        public string Adjunto { get; set; }
    }
}
