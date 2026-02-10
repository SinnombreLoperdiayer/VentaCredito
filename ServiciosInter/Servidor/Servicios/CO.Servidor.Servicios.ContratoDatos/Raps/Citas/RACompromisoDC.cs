using Framework.Servidor.Comun;
using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Citas
{
    [DataContract(Namespace = "https://wwww.controllogis.com")]
    public class RACompromisoDC
    {
        [DataMember]
        public long IdCompromiso { get; set; }

        [DataMember]
        public string Descripcion { get; set; }

        [DataMember]
        public bool Estado { get; set; }

        [DataMember]
        public long Idresponsable { get; set; }

        [DataMember]
        public long IdCita { get; set; }

        [DataMember]
        public EnumEstadoRegistro EstadoRegistro { get; set; }

        [DataMember]
        public string NombreResponsable { get; set; }
    }
}
