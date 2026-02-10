using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;
using System;

namespace CO.Servidor.Servicios.ContratoDatos.GestionGiros.ExploradorGiros
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class GIMotivosBloqueoDesbloqueo
    {
        [DataMember]
        public string IdIdentificacion { get; set; }
        [DataMember]
        public string Nombre { get; set; }
        [DataMember]
        public short Motivo { get; set; }
        [DataMember]
        public DateTime FechaGrabacion { get; set; }
        [DataMember]
        public long IdAdmisionGiro { get; set; }
        [DataMember]
        public long IdAdmisionGiroIdent { get; set; }
    }
}
