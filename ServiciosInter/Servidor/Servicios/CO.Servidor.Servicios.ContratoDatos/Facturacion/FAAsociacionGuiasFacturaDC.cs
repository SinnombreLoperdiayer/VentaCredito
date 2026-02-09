using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Facturacion
{
    /// <summary>
    /// Representa los datos que se deben usar para asociar un conjunto de guías crédito a una factura
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class FAAsociacionGuiasFacturaDC
    {
        [DataMember]
        public long FacturaDesde { get; set; }

        [DataMember]
        public long FacturaHasta { get; set; }

        [DataMember]
        public int IdCliente { get; set; }

        [DataMember]
        public long NumeroFactura { get; set; }

        [DataMember]
        public string CreadoPor { get; set; }
    }
}
