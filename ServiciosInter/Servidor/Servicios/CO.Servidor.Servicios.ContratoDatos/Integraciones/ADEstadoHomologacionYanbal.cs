using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Integraciones
{
    [DataContract(Namespace = "")]
    public class ADEstadoHomologacionYanbal
    {
        [DataMember]
        public string ADM_DescripcionEstado { get; set; }
        [DataMember]
        public DateTime ADM_FechaAdmision { get; set; }
        [DataMember]
        public string ADM_NoPedido { get; set; }
        [DataMember]
        public string ADM_Observaciones { get; set; }
        [DataMember]
        public string ADM_NumeroGuia { get; set; }
        [DataMember]
        public string ADM_IdDestinatario { get; set; }
        [DataMember]
        public DateTime EGT_FechaGrabacion { get; set; }
        [DataMember]
        public string HY_CodEventoYanbal { get; set; }
        [DataMember]
        public string HY_TipoEventoYanbal { get; set; }
        [DataMember]
        public string  HY_DescripcionYanbal { get; set; }
        [DataMember]
        public string HY_MotivoYanbal { get; set; }
        [DataMember]
        public string HY_DescripcionMotivo { get; set; }
        [DataMember]
        public string CES_Nombre { get; set; }
        [DataMember]
        public string ADM_NumeroBolsaSeguridad { get; set; }
        
    }
}
