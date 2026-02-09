using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.LogisticaInversa
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class LISalidaCustodia : DataContractBase
    {
        #region Members

        [DataMember]
        public long NumeroGuia { get; set; }

        [DataMember]
        public string Clasificacion { get; set; }

        [DataMember]
        public string Pago { get; set; }

        [DataMember]
        public string Contenido { get; set; }

        [DataMember]
        public string Destino { get; set; }

        #endregion
    }
}
