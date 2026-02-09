using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.OperacionNacional
{
    /// <summary>
    /// Novedad asignada a una Estacion-Ruta
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class ONTipoNovedadEstacionRutaDC
    {
        /// <summary>
        /// Es el id de la Novedad Estacion-Ruta
        /// </summary>
        [DataMember]
        public short IdNovedad { get; set; }

        /// <summary>
        /// la descripcion de la Novedad
        /// </summary>
        [DataMember]
        public string Descripcion { get; set; }

    }
}