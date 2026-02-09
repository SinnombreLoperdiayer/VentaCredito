using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.OperacionUrbana
{
    [DataContract(Namespace = "http://contrologis.com")]
    public enum OUEnumTipoOrigenRecogida
    {
        /// <summary>
        /// Pagina web 
        /// </summary>
        [EnumMember]
        WEB,
        /// <summary>
        /// Telefono
        /// </summary>
        [EnumMember]
        IVR,
        /// <summary>
        /// Aplicacion movil
        /// </summary>
        [EnumMember]
        APP,
        /// <summary>
        /// Controller
        /// </summary>
        [EnumMember]        
        CON
    }    

}
