using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.CentroAcopio
{
    [DataContract]
    public class AdjuntoConsolidado
    {
        [DataMember]
        public int IdAdjunto { get; set; }

        [DataMember]
        public string Ruta { get; set; }

        [DataMember]
        public string Imagen { get; set; }

        [DataMember]
        public int IdMovimiento { get; set; }

        [DataMember]
        public DateTime FechaGrabacion { get; set; }
    }
}
