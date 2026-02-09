using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Runtime.Serialization;


namespace CO.Servidor.Servicios.ContratoDatos.LogisticaInversa
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class LIEstadoYMotivoGuiaDC : DataContractBase
    {
        [DataMember]
        public DateTime EstadoFechaGravacion { get; set; }
        [DataMember]
        public int IdEstadoGuia { get; set; }
        [DataMember]
        public string DescripcionEstado { get; set; }
        [DataMember]
        public string EstadoLocalidad { get; set; }
        [DataMember]
        public string EstadoModulo { get; set; }
        [DataMember]
        public string EstadoUsuario { get; set; }
        [DataMember]
        public int IdMotivoGuia { get; set; }
        [DataMember]
        public string MotivoDescripcion { get; set; }
        [DataMember]
        public string MotivoUsuario { get; set; }
        [DataMember]
        public int IdTipoMotivoGuia { get; set; }
        [DataMember]
        public string MotivoTipo { get; set; }
        [DataMember]
        public DateTime MotivoFechaGravacion { get; set; }
        [DataMember]
        public int IdMotivoGuiaCRC { get; set; }
        [DataMember]
        public string MotivoCRC { get; set; }

    }
}
