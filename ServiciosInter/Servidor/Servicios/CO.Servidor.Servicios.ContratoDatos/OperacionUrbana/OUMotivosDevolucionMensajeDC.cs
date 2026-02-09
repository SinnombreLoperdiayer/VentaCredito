using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos;


namespace CO.Servidor.Servicios.ContratoDatos.OperacionUrbana
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class OUMotivosDevolucionMensajeDC : DataContractBase
    {
        [DataMember]
        public int cantidadMotivos { get; set; }

        [DataMember]
        public string historicoMotivos { get; set; }
    }
}
