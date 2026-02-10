using CO.Servidor.Servicios.ContratoDatos.Raps.Escalonamiento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion
{
    //Mod
    [DataContract(Namespace = "http://contrologis.com")]
    public class RAResponsableDC
    {
        [DataMember]
        public int IdResponsableFalla { get; set; }

        [DataMember]
        public string IdentificacionResponsable { get; set; }

        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Nombre { get; set; }

        [DataMember]
        public RACargoEscalarDC CargoEscalona { get; set; }

    }
}
