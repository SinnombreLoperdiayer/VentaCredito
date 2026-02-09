using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Tarifas.Precios
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class TAPreciosAgrupadosDC
    {
        [DataMember]
        public int IdServicio { get; set; }

        [DataMember]
        public TAPrecioMensajeriaDC Precio { get; set; }

        [DataMember]
        public TAPrecioCargaDC PrecioCarga { get; set; }

        [DataMember]
        public string Mensaje { get; set; }

        [DataMember]
        public string NombreServicio { get; set; }

        [DataMember]
        public string TiempoEntrega { get; set; }

        [DataMember]
        public TAFormaPagoServicio FormaPagoServicio { get; set; }
    }
}
