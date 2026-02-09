using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria
{
    [DataContract]
    public class DatosAfectacionCaja
    {
        [DataMember]
        public int IdConceptoCaja { get; set; }

        [DataMember]
        public long IdCentroServiciosVenta { get; set; }

        [DataMember]
        public string NombreCentroServiciosVenta { get; set; }

        [DataMember]
        public int Ingreso { get; set; }

        [DataMember]
        public int Egreso { get; set; }

        [DataMember]
        public string NombreConcepto { get; set; }

        [DataMember]
        public string Observacion { get; set; }

        [DataMember]
        public string DescripcionFormaPago { get; set; }

        [DataMember]
        public string NumeroComprobante { get; set; }

        [DataMember]
        public DateTime FechaGrabacion { get; set; }

        [DataMember]
        public string CreadoPor { get; set; }

        [DataMember]
        public string Usuario { get; set; }
    }
}
