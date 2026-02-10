using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.OperacionNacional
{
    /// <summary>
    /// Contiene la Informacion de la tabla Novedades Envios Sueltos
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class ONNovedadesEnvioDC
    {
        /// <summary>
        /// Es el id de la novedad Envio Suelto
        /// </summary>
        [DataMember]
        public int IdNovedadEnvioSuelto { get; set; }

        /// <summary>
        /// la descripcion de la Novedad
        /// </summary>
        [DataMember]
        public string DescripcionNovedad { get; set; }

        /// <summary>
        /// Novedad seleccionada
        /// </summary>
        [DataMember]
        public bool NovedadSeleccionada { get; set; }
    }
}