using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.OperacionNacional
{
    /// <summary>
    /// Contiene la Informacion de la tabla de
    /// Novedades de Consolidados
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class ONNovedadesConsolidadoDC
    {
        [DataMember]
        public long NumCtrolTransMan { get; set; }

        [DataMember]
        public short IdNovedadConsolidado { get; set; }

        [DataMember]
        public string DescripcionNovedadConsolidado { get; set; }

        [DataMember]
        public bool NovedadSeleccionada { get; set; }
    }
}