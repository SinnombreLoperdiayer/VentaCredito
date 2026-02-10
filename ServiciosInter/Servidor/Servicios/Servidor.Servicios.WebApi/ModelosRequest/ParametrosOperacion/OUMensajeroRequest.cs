using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace CO.Servidor.Servicios.WebApi.ModelosRequest.ParametrosOperacion
{
    public class OUMensajeroRequest
    {
        [DataMember]
        public long IdMensajero { get; set; }
        [DataMember]
        public string NombreCompleto { get; set; }
        [DataMember]
        public string Identificacion { get; set; }
        [DataMember]
        public long IdCentroServicio { get; set; }

    }
}