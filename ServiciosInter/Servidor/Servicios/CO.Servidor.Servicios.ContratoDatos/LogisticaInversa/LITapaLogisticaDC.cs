using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.LogisticaInversa
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class LITapaLogisticaDC : DataContractBase
    {
        [DataMember]
        public long IdTapaLogistica { get; set; }


        [DataMember]
        public long? NumeroTapaLogistica { get; set; }

        [DataMember]
        public long NumeroGuia { get; set; }

        
        [DataMember]
        public LIEnumTipoTapaLogisticaDC Tipo { get; set; }

        [DataMember]
        public bool Impresa { get; set; }

        [DataMember]
        public DateTime? FechaImpresion{ get; set; }

        [DataMember]
        public DateTime FechaGrabacion { get; set; }

        [DataMember]
        public string CreadoPor { get; set; }

    }
}
