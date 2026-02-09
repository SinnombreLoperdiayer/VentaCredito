using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RAFiltroCargoPersonaNovaRapDC
    {
        [DataMember]
        public string IdCargos { get; set; }

        [DataMember]
        public string CodigoCargos { get; set; }

        [DataMember]
        public string IdTerritoriales { get; set; }

        [DataMember]
        public string IdRegionales { get; set; }

        [DataMember]
        public string IdProcesos { get; set; }

        [DataMember]
        public string IdProcedimientos { get; set; }
        
        [DataMember]
        public bool PorPersona { get; set; }
    }
}
