using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Servidor.Servicios.ContratoDatos.Seguridad
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class SEUsuarioCentroServicioDC : DataContractBase
    {
        /// <summary>
        /// Es el Id de l centro de Servicio.
        /// </summary>
        /// <value>
        /// The id centro servicio.
        /// </value>
        [DataMember]
        public long IdCentroServicio { get; set; }

        /// <summary>
        /// Es el Nombre del centro de Servicio
        /// </summary>
        /// <value>
        /// The nombre centro servicio.
        /// </value>
        [DataMember]
        public string NombreCentroServicio { get; set; }

        /// <summary>
        /// Es el Id del Usuario Consultado
        /// </summary>
        /// <value>
        /// The id usuario.
        /// </value>
        [DataMember]
        public long IdUsuario { get; set; }

        /// <summary>
        /// Id de Caja
        /// </summary>
        /// <value>
        /// The id caja.
        /// </value>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Caja")]
        public int IdCaja { get; set; }

        /// <summary>
        /// Nombre del cajero
        /// </summary>
        /// <value>
        /// The nombre cajero.
        /// </value>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Nombre")]
        public string NombreCajero { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Documento")]
        public string NumeroDocumento { get; set; }

        /// <summary>
        /// Es el perfil que tiene el usuario Asignado
        /// </summary>
        [DataMember]
        public string Perfil { get; set; }

        [DataMember]
        public string Usuario { get; set; }
    }
}