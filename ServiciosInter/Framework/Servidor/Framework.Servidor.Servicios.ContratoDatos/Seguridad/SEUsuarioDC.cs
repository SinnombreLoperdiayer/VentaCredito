using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Servidor.Servicios.ContratoDatos.Seguridad
{
    /// <summary>
    /// Data de la Tabla de Usuario_SEG
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class SEUsuarioDC : DataContractBase
    {
        /// <summary>
        /// Es el Id del codigo del Usuario.
        /// </summary>
        [DataMember]
        public long IdCodigoUsuario { get; set; }

        /// <summary>
        /// es el usuario del Usuario
        /// </summary>
        [DataMember]
        public string Usuario { get; set; }

        /// <summary>
        /// Es el estado actual del Usuario
        /// </summary>
        [DataMember]
        public string EstadoUsuario { get; set; }

        /// <summary>
        /// Identifica si el usuario es interno
        /// </summary>
        [DataMember]
        public bool UsuarioInterno{ get; set; }
    }
}