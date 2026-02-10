using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Servidor.Servicios.ContratoDatos.Seguridad
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class SEUsuarioPorCodigoDC : DataContractBase
    {
        /// <summary>
        /// Es el Id del codigo del Usuario.
        /// </summary>
        [DataMember]
        public long IdCodigoUsuario { get; set; }

        /// <summary>
        /// Nombre del Usuario
        /// </summary>
        [DataMember]
        public string NombreUsuario { get; set; }

        /// <summary>
        /// Apellidos del Usuario
        /// </summary>
        [DataMember]
        public string PrimerApellido { get; set; }

        /// <summary>
        /// Apellidos del Usuario
        /// </summary>
        [DataMember]
        public string SegundoApellido { get; set; }

        /// <summary>
        /// Numero de Documento del Usuario
        /// </summary>
        [DataMember]
        public string Documento { get; set; }

        /// <summary>
        /// Id Caja Centro
        /// </summary>
        [DataMember]
        public int idCaja { get; set; }

        /// <summary>
        /// Es el Nombre Con Apellidos
        /// </summary>
        [DataMember]
        public string NombreCompleto { get; set; }

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
    }
}