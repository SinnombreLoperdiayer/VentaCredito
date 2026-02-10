using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Servidor.Servicios.ContratoDatos.Seguridad
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class SEValidacionUsuarioYPersonaInternaDC : DataContractBase
    {
        /// <summary>
        /// Valida si el Usuario existe pero
        /// esta inactivo
        /// </summary>
        [DataMember]
        public bool UsuarioExiste { get; set; }

        /// <summary>
        /// Valida que la persona interna Existe pero
        /// tiene un usuario inactivo
        /// </summary>
        [DataMember]
        public bool PersonaInternaExiste { get; set; }
    }
}