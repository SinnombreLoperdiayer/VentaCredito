using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RAReglasIngrecionesManualDC
    {
        [DataMember]
        public int IdRegla { get; set; }

        [DataMember]
        public string NombreRegla { get; set; }

        [DataMember]
        public int IdEstado { get; set; }

        [DataMember]
        public string Assembly { get; set; }

        [DataMember]
        public string NameSpace { get; set; }

        [DataMember]
        public string Clase { get; set; }

        [DataMember]
        public int IdTipoEscalonamiento { get; set; }

    }
}
