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
    public class GIMotivosInactivacion
    {
        [DataMember]
        public string DescMotivoInactivacion {get;set;}
        [DataMember]

        public short IdMotivoInactivacion { get; set; }
    }
}
