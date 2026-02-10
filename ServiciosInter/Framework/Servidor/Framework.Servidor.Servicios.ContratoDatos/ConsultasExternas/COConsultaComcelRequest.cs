using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Servidor.Servicios.ContratoDatos.ConsultasExternas
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class COConsultaComcelRequest
    {

        
        [DataMember]
        public string Cuenta { get; set; }        
        /// <summary>
        /// periodo de en el que se envio la guia, YYYYMM
        /// </summary>
        [DataMember]
        public string Fecha { get; set; }

        [DataMember]
        public string Producto { get; set; }

        [DataMember]
        public string NomUsuario { get; set; }
        [DataMember]
        public string Password { get; set; }
    }
}
