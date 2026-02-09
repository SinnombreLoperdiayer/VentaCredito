using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.CentroAcopio
{
    [DataContract]
    public class NovedadConsolidado
    {
        [DataMember]
        public int IdNovedad { get; set; }

        [DataMember]
        public string Descripcion { get; set; }

        [DataMember]
        public DateTime FechaGrabacion { get; set; }

        [DataMember]
        public string CreadoPor { get; set; }

        [DataMember]
        public int IdTipoNovedad { get; set; }

        [DataMember]
        public int IdMovimiento { get; set; }

        [DataMember]
        public int IdConsolidado { get; set; }

        [DataMember]
        public List<AdjuntoConsolidado> AdjuntosConsolidado { get; set; }
    }
}
