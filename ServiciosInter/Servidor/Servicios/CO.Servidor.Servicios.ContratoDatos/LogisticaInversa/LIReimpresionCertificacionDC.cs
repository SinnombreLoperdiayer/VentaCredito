using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.LogisticaInversa
{

    [DataContract(Namespace = "http://contrologis.com")]
    public class LIReimpresionCertificacionDC : DataContractBase
    {
        [DataMember]
        public long NumeroGuia { get; set; }

        [DataMember]
        public string NombreSolicitante { get; set; }

        [DataMember]
        public string IdentificacionSolicitante { get; set; }

        [DataMember]
        public string TelefonoSolicitante { get; set; }

        [DataMember]
        public int IdCentroServicio { get; set; }

        [DataMember]
        public string DireccionCentroServicio { get; set; }
    }
}
