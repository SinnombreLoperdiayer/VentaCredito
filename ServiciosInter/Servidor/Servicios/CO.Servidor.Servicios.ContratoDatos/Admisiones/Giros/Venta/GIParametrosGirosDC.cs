using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros.Venta
{

    [DataContract(Namespace = "http://contrologis.com")]
    public class GIParametrosGirosDC : DataContractBase
    {
        [DataMember]

        public string IdParametro { get; set; }

        [DataMember]
        public string ValorParametro { get; set; }

        [DataMember]
        public string Descripcion { get; set; }

        [DataMember]
        public DateTime FechaGrabacion { get; set; }

        [DataMember]
        public string CreadoPor { get; set; }
    }
}
