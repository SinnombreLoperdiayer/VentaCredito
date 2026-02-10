using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RAMenusPermitidosDC
    {
        [DataMember]
        public string Nombre { get; set; }

        [DataMember]
        public string Icon { get; set; }

        [DataMember]
        public string Accion { get; set; }

        [DataMember]
        public bool Activo { get; set; }

        [DataMember]
        public int IdMenu {get;set;}

        [DataMember]
        public int MenuPadre { get; set; }

        [DataMember]
        public int NumeroSolicitudes { get; set; }

        [DataMember]
        public List<RAMenusPermitidosDC> Items { get; set; }
    }
}
