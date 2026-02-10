using System;
using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Consultas
{
    public class RAObtenerListaSolicitudesRaps
    {
       [DataMember]
        public int IdEstado { get; set; }

       [DataMember]
        public string Descripcion { get; set; }

       [DataMember]
       public DateTime FechaCreacion { get; set; }

       [DataMember]
       public DateTime FechaVencimiento { get; set; }       
    }
}
