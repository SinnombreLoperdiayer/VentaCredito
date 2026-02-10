using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.CentroAcopio
{
    [DataContract]
    public class MovimientoConsolidado
    {
        [DataMember]
        public int IdMovimiento { get; set; }

        [DataMember]
        public long NumeroPrecinto { get; set; }

        [DataMember]
        public long IdCentroServicioDestino { get; set; }

        [DataMember]
        public int IdTipoMovimiento { get; set; }

        [DataMember]
        public string FechaMovimiento { get; set; }

        [DataMember]
        public string CreadoPor { get; set; }        

        [DataMember]
        public string NumeroConsolidado { get; set; }

        [DataMember]
        public int IdTipoConsolidado { get; set; }

        [DataMember]
        public int IdMovimientoLog { get; set; }

        [DataMember]
        public string IdLocalidadOrigen { get; set; }

        [DataMember]
        public string IdLocalidadDestino { get; set; }

        [DataMember]
        public string IdLocalidadMovimiento { get; set; }

        [DataMember]
        public NovedadConsolidado Novedad { get; set; }

    }
}
