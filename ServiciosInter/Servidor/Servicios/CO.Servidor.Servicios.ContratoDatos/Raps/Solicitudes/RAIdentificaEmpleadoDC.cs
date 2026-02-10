using CO.Servidor.Servicios.ContratoDatos.Raps.Citas;
using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RAIdentificaEmpleadoDC
    {
        [DataMember]
        public string CodigoSucursal { get; set; }

        [DataMember]
        public string CodigoEmpleado { get; set; }

        [DataMember]
        public string NumeroIdentificacion { get; set; }

        [DataMember]
        public string Nombre { get; set; }

        [DataMember]
        public string email { get; set; }

        [DataMember]
        public string IdCargo { get; set; }
        [DataMember]
        public string DescripcionCargo { get; set; }

        [DataMember]
        public RAEnumTipoIntegrante IdTipoIntegrante { get; set; }

        [DataMember]
        public string CodigoPlanta { get; set; }

        [DataMember]
        public string TipoIntegrante { get; set; }
    }
}
