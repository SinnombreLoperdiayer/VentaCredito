using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace CO.Servidor.Servicios.WebApi.ModelosRequest.CentroServicio
{
    public class PUCentroServicioResponse
    {
        [DataMember]
        public long IdCentroServicio { get; set; }
        [DataMember]
        public string Nombre { get; set; }
        [DataMember]
        public string Tipo { get; set; }
    }
}