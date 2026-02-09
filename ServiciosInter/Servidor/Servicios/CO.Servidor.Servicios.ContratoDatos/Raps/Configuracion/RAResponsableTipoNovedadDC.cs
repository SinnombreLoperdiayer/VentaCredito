using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RAResponsableTipoNovedadDC
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int IdTipoNovedadHijo { get; set; }

        [DataMember]
        public string Nombre { get; set; }

        [DataMember]
        public int EstadoOrigen { get; set; }

        [DataMember]
        public bool EsUnicoEnvio { get; set; }

        [DataMember]
        public int IdOrigenRaps { get; set; }
    }
}
