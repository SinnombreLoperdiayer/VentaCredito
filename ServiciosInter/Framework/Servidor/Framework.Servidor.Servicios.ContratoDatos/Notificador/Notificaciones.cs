using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Framework.Servidor.Servicios.ContratoDatos.Notificador
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class CentrosServiciosLinea
    {
        [DataMember]
        public string NombreCentroServicios { get; set; }
        [DataMember]
        public int IdCodUsuario { get; set; }
        [DataMember]
        public string NombreUsuario {get;set;}
        [DataMember]
        public int IdCaja { get; set; }
        [DataMember]
        public string IdNotificador { get; set; }
        [DataMember]
        public long IdCentroServicios { get; set; }

        [DataMember]
        public bool EnLinea { get; set; }

        [DataMember]
        public string NombrePersona { get; set; }

        [DataMember]
        public string IdentificacionPersona { get; set; }
        [DataMember]
        public string TipoCentoServicios { get; set; }
        
        [DataMember]
        public string NombreCol { get; set; }
    }
}
