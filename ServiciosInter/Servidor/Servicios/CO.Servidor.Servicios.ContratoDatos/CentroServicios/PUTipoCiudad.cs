using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.CentroServicios
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class PUTipoCiudad : DataContractBase
    {
        [DataMember]
        public int IdTipoCiudad { get; set; }

        [DataMember]
        public string TipoCiudad { get; set; }

    }
}
