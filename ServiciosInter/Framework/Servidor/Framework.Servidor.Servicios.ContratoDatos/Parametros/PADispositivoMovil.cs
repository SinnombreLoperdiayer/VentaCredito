using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Servidor.Servicios.ContratoDatos.Parametros
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class PADispositivoMovil
    {
        [DataMember]
        public long IdDispositivo { get; set; }
        
        [DataMember]
        public PAEnumOsDispositivo SistemaOperativo { get; set; }

        [DataMember]
        public string TokenDispositivo { get; set; }
        
        [DataMember]
        public PAEnumTiposDispositivos TipoDispositivo { get; set; }

        [DataMember]
        public string IdCiudad { get; set; }

        [DataMember]
        public string NumeroImei { get; set; }
    }
}
