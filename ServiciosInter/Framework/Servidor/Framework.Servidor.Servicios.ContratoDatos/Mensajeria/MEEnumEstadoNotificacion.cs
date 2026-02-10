using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Framework.Servidor.Servicios.ContratoDatos.Mensajeria
{
    /// <summary>
    /// Enumeracion con los diferente tipos de estados de un mensaje
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public enum MEEnumEstadoNotificacion
    {
        /// <summary>
        /// Mensaje enviado pero no leido
        /// </summary>
        [EnumMember]
        Enviado,

        /// <summary>
        /// Mensaja enviado y leido
        /// </summary>
        [EnumMember]
        Leido
    }
}
