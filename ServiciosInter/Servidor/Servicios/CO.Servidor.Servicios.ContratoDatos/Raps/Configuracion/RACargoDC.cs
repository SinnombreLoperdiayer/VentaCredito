using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RACargoDC
    {
        [DataMember]
        public string IdCargo { get; set; }

        [DataMember]
        public string Descripcion { get; set; }

        [DataMember]
        public bool Estado { get; set; }

        [DataMember]
        public string Procedimiento { get; set; }

        [DataMember]
        public string NombreProcedimiento { get; set; }

        [DataMember]
        public string CargoNovasoft { get; set; }
        
        [DataMember]
        public bool EnteControl { get; set; }

        [DataMember]
        public bool Regional { get; set; }

        [DataMember]
        public string CorreoCorporativo { get; set; }
        
        [DataMember]
        public string CodSucursal { get; set; }

        [DataMember]
        public string IdProceso { get; set; }

        [DataMember]
        public string NombreProceso { get; set; }

        [DataMember]
        public string CodigoCargo { get; set; }

        [DataMember]
        public string Identificacion { get; set; }

        [DataMember]
        public string NombrePersona { get; set; }

        [DataMember]
        public int IdTerritorial { get; set; }

        [DataMember]
        public string IdRegional { get; set; }
    } 
}
