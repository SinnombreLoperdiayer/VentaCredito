using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Integraciones
{
    public class INTMensajeTextoDC : DataContractBase
    {
        [DataMember]
        public int IdCliente { get; set; }

        [DataMember]
        public short IdServicio { get; set; }


        [DataMember]
        public string Mensaje { get; set; }
    }
}
