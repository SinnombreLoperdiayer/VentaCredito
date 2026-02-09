using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.LogisticaInversa
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class LITipoEvidenciaControllerAppDC : DataContractBase
    {
        [DataMember]
        public LIEnumTipoEvidenciaDC TipoEvidenciaControllerApp { get; set; }

        [DataMember]
        public string NombreEvidenciaControllerApp { get; set; }

        [DataMember]
        public List<string> Imagenes { get; set; }
    }
}
