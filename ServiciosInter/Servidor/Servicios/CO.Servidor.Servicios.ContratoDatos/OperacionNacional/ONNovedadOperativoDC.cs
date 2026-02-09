using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.OperacionNacional
{
    /// <summary>
    /// Contiene la informacion de la novedad operativo
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class ONNovedadOperativoDC
    {
        [DataMember]
        public short IdNovedadOperativo { get; set; }

        [DataMember]
        public string Descripcion { get; set; }

        [DataMember]
        public bool NovedadSeleccionada { get; set; }
    }
}