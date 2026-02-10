using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Produccion
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class PRRetencionProduccionDC : DataContractBase
    {
        [DataMember]
        public PRRetencionDC Retencion { get; set; }       

        [DataMember]
        public string TextoEnLaProduccion { get; set; }

        [DataMember]
        public decimal BaseRetencion { get; set; }

        [DataMember]
        public decimal PorcRetencion { get; set; }

        [DataMember]
        public string CuentaContableNovasoft { get; set; }

        [DataMember]
        public DateTime FechaGrabacion { get; set; }

        [DataMember]
        public string CreadoPor { get; set; }

        [DataMember]
        public List<PRConceptoRetencionDC> BasesRetencion { get; set; }
    }
}