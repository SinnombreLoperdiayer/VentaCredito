using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Servidor.Servicios.ContratoDatos.ConsultasExternas
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class COConsultaComcelResponse
    {

        [DataMember]
        public string Cuenta { get; set; }

        [DataMember]
        public string NombreApellido { get; set; }

        [DataMember]
        public string Direccion { get; set; }

        [DataMember]
        public string NumeroGuia { get; set; }


        [DataMember]
        public string Imagen { get; set; }
        
        [DataMember]
        public string MotivoDevolucion { get; set; }
        
        [DataMember]
        public string Estado { get; set; }

        [DataMember]
        public string Producto { get; set; }

        [DataMember]
        public string Mensaje { get; set; }

    }
}
