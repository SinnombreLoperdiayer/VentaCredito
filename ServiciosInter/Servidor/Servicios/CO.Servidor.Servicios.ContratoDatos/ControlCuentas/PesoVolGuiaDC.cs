using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.ControlCuentas
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class PesoVolGuiaDC : DataContractBase
    {   
        [DataMember]
        public decimal LargoVolumetricoAuditoria { get; set; }
        [DataMember]
        public decimal AnchoVolumetricoAuditoria { get; set; }
        [DataMember]
        public decimal AltoVolumetricoAuditoria { get; set; }
        [DataMember]
        public decimal PesoVolumetricoTotalAuditoria { get; set; }
        [DataMember]
        public DateTime FechaAuditoria { get; set; }
        [DataMember]
        public string CreadoPor { get; set; }
        [DataMember]
        public string ObservacionesAuditoria { get; set; }
        [DataMember]
        public decimal PesoBasculaAuditoria { get; set; }
        
    }
}
