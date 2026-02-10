using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Integraciones
{
    [DataContract]
    public class ArchivoVolante
    {
        [DataMember()]
        public long IdArchivo { get; set; }

        [DataMember()]
        public long IdCentroLogistico { get; set; }

        [DataMember()]
        public string RutaArchivo { get; set; }

        [DataMember()]
        public bool ImagenSincronizada { get; set; }

        [DataMember()]
        public long NumeroVolante { get; set; }

        [DataMember()]
        public DateTime FechaGrabacion { get; set; }

        [DataMember()]
        public string CreadoPor { get; set; }
    }
}
