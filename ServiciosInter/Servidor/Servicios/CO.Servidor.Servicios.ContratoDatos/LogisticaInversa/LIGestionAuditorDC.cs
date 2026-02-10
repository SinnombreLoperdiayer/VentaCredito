using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;



namespace CO.Servidor.Servicios.ContratoDatos.LogisticaInversa
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class LIGestionAuditorDC : DataContractBase
    {
        [DataMember]
        public int IdAuditoria { get; set; }
        [DataMember]
        public long NumeroGuia { get; set; }
        [DataMember]
        public long IdEstadoGuiaLog { get; set; }
        [DataMember]
        public long IdPlanillaAsignacion { get; set; }
        [DataMember]
        public string DescripcionInmueble { get; set; }
        [DataMember]
        public string NombreReceptorAuditoria { get; set; }
        [DataMember]
        public string CedulaReceptorAuditoria { get; set; }
                
    }
}
