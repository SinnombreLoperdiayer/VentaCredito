using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Comun
{
     [DataContract(Namespace = "http://contrologis.com")]
    public class COCambioFechaEntregaDC : DataContractBase
    {
        [DataMember]
        public int IdCambioFechaEntrega{ get; set; }

        [DataMember]
        public ADGuia Guia { get; set; }

        [DataMember]
        public int TiempoAfectacion { get; set; }

        [DataMember]
        public int TiempoAfectacionDigitalizacion { get; set; }

        [DataMember]
        public int TiempoAfectacionArchivo { get; set; }

        [DataMember]
        public DateTime FechaEntrega { get; set; }

        [DataMember]
        public DateTime FechaNuevaEntrega { get; set; }

    }
}
