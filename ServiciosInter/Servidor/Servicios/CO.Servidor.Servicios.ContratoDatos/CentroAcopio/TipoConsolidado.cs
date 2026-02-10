using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.CentroAcopio
{
    [DataContract]
    public class TipoConsolidado
    {
        [DataMember]
        public int IdTipoConsolidado { get; set; }

        [DataMember]
        public string Descripcion { get; set; }

        [DataMember]
        public string CreadoPor { get; set; }

        [DataMember]
        public DateTime FechaGrabacion { get; set; }
    }
}
