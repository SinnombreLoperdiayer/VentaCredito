using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.OperacionUrbana
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class OUNombresMensajeroDC : DataContractBase
    {
        /// <summary>
        /// Gets or sets the identificacion.
        /// </summary>
        /// <value>
        /// The identificacion.
        /// </value>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Identificacion")]
        [Filtrable("PEI_Identificacion", "Identificacion: ", COEnumTipoControlFiltro.TextBox)]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public string Identificacion { get; set; }

        /// <summary>
        /// Gets or sets the nombre apellido.
        /// </summary>
        /// <value>
        /// The nombre apellido.
        /// </value>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NombreMensajero")]

        //[Filtrable("NombreCompleto", "Nombre: ", COEnumTipoControlFiltro.ComboBox)]
        public string NombreApellido { get; set; }

        /// <summary>
        /// Gets or sets the id persona interna.
        /// </summary>
        /// <value>
        /// The id persona interna.
        /// </value>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Id")]
        public long IdPersonaInterna { get; set; }

        /// <summary>
        /// Gets or sets the id tipo mensajero.
        /// </summary>
        /// <value>
        /// El id tipo mensajero.
        /// </value>
        [DataMember]
        public int IdTipoMensajero { get; set; }

        /// <summary>
        /// Gets or sets the descripcion tipomensajero.
        /// </summary>
        /// <value>
        /// La descripcion tipo Mensajero.
        /// </value>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoMensajero")]
        public string DescripcionTipomensajero { get; set; }

        /// <summary>
        /// Gets or sets the id agencia.
        /// </summary>
        /// <value>
        /// Es el id agencia.
        /// </value>
        [DataMember]
        public long IdAgencia { get; set; }

        /// <summary>
        /// Gets or sets the id centro logistico.
        /// </summary>
        /// <value>
        /// Es el Id del centro Logistico correspondiente
        /// </value>
        [DataMember]
        public long IdCentroLogistico { get; set; }

        /// <summary>
        /// Es el Cargo del Mensajero
        /// </summary>
        [DataMember]
        public string CargoMensajero { get; set; }

        /// <summary>
        /// Bandera de Mensajero Urbano
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "MensajeroUrbano", Description = "ToolTipMensajeroUrbano")]
        public bool EsMensajeroUrbano { get; set; }
        [DataMember]
        public long IdMensajero { get; set; }

        [DataMember]
        public string IdentificacionyNombre { get { return this.Identificacion + " - " + this.NombreApellido; } set {} }
    }
}