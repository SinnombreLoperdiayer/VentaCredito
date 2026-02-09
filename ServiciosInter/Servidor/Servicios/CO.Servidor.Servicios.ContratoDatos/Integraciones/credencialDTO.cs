using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Integraciones
{
    [DataContract(Namespace = "")]
    public class credencialDTO
    {
        [DataMember(Order = 1)]
        public string usuario { get; set; }

        [DataMember(Order=2)]
        public string clave { get; set; }        
        
    }
}
