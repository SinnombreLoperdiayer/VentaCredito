using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.WebApi.Controllers
{
    
    public class RAObtenerDetalleSolicitudDC
    {
        [DataMember]
        public int IdSolicitud { get; set; }

        [DataMember]
        public DateTime FechaCreacion { get; set; } 
        
        [DataMember]
        public string NombreSolicitante {get; set; }

        [DataMember]
        public int IdSubclasificacion { get; set; }

        [DataMember]
        public int Regional { get; set; }

        [DataMember]
        public DateTime FechaVencimiento { get; set; }

        [DataMember]
        public string NombreSolicitadoa { get; set; }

        [DataMember]
        public DateTime Fecha { get; set; }

        [DataMember]
        public String Comentario { get; set; }

        [DataMember]
        public string UbicacionNombre { get; set; }

    }
}
