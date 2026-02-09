using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Servidor.Servicios.ContratoDatos.Parametros
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class PAPropiedadesAplicacionDC : DataContractBase
    {
        [DataMember]
        public string IdUsuario { get; set; }

        [DataMember]
        public string NombreUsuario { get; set; }

        [DataMember]
        public string IdCentroServicio { get; set; }

        [DataMember]
        public string NombreCentroServicio { get; set; }

        [DataMember]
        public int IdCaja { get; set; }

        [DataMember]
        public string IdCiudad { get; set; }

        [DataMember]
        public string NombreCiudad { get; set; }

    }
}
