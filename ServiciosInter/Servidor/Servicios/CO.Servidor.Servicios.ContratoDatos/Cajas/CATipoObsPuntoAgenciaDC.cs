using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;

namespace CO.Servidor.Servicios.ContratoDatos.Cajas
{
    /// <summary>
    /// Clase Tipos de Observacion Puntos
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class CATipoObsPuntoAgenciaDC
    {
        /// <summary>
        /// Gets or sets the id tipo observacion.
        /// </summary>
        /// <value>
        /// Es el id tipo observacion.
        /// </value>
        [DataMember]
        [Filtrable("TOP_IdTipoObservacion", typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), "Id", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 5)]
        [CamposOrdenamiento("TOP_IdTipoObservacion")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Etiquetas), Name = "Id", Description = "ToolTipTipoObsevPunto")]
        public short idTipoObservacion { get; set; }

        /// <summary>
        /// Gets or sets the descripcion.
        /// </summary>
        /// <value>
        /// Es la descripcion del tipo de Observacion.
        /// </value>
        [DataMember]
        [Filtrable("TOP_Descripcion", typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), "Descripcion", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 30)]
        [CamposOrdenamiento("TOP_Descripcion")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Etiquetas), Name = "Descripcion", Description = "ToolTipDescTipObsPun")]
        public string Descripcion { get; set; }

        /// <summary>
        /// Gets or sets the usuario.
        /// </summary>
        /// <value>
        /// El usuario que guarda el regitro.
        /// </value>
        [DataMember]
        public string Usuario { get; set; }

        /// <summary>
        /// Enumeración que indica el estado del objeto dentro de una lista
        /// </summary>
        [DataMember]
        public EnumEstadoRegistro EstadoRegistro { get; set; }
    }
}