using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CO.Servidor.Servicios.ContratoDatos.ReversionEstados
{

    [DataContract]
    public class ReversionEstado
    {
        [DataMember]
        public long NumeroGuia { get; set; }
        [DataMember]
        public long NumeroTicket { get; set; }
        [DataMember]
        public int IdEstadoOrigen { get; set; }
        [DataMember]
        public string Observacion { get; set; }
        [DataMember]
        public int IdEstadoSolicitado { get; set; }
        [DataMember]
        public bool GeneraRAP { get; set; }
        [DataMember]
        public string CreadoPor { get; set; }
        [DataMember]
        public string SolicitadoPor { get; set; }
    }
}

