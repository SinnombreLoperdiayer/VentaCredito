using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RARegionalSuculsalDC
    {
        [DataMember]
        public string CodSucursal { get; set; }

        [DataMember]
        public string NombreSucursal { get; set; }

        [DataMember]
        public int IdCargo { get; set; }

        [DataMember]
        public string Descripcion { get; set; }

        [DataMember]
        public bool Estado { get; set; }

        [DataMember]
        public int IdProcedimiento { get; set; }

        [DataMember]
        public string CargoNovasoft { get; set; }

        [DataMember]
        public string CodigoSucursal { get; set; }

        [DataMember]
        public string CodigoMunicipio { get; set; }

        [DataMember]
        public long CodigoDepartamento { get; set; }

        [DataMember]
        public bool EstadoSucursal { get; set; }

    }
}
