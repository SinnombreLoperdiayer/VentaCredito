using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RACargoPersonaNovaRapDC
    {
        [DataMember]
        public string IdCargo { get; set; }

        [DataMember]
        public string CodigoCargo { get; set; }

        [DataMember]
        public string NombreCargo { get; set; }

        [DataMember]
        public string Identificacion { get; set; }

        [DataMember]
        public string NombrePersona { get; set; }

        [DataMember]
        public int IdTerritorial { get; set; }

        [DataMember]
        public string IdRegional { get; set; }

        [DataMember]
        public string IdProceso { get; set; }

        [DataMember]
        public string NombreProceso { get; set; }

        [DataMember]
        public string IdProcedimiento { get; set; }

        [DataMember]
        public string NombreProcedimiento { get; set; }

        [DataMember]
        public string NombreSucursal { get; set; }

        [DataMember]
        public string DescripcionRol { get; set; }

        [DataMember]
        public int IdRol { get; set; }
    }
}
