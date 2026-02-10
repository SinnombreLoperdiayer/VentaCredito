using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;

namespace Framework.Servidor.Servicios.ContratoDatos.Parametros
{
    /// <summary>
    /// Clase que contiene la informacion de un responsable legal
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class PAResponsableLegal : DataContractBase
    {
        public PAResponsableLegal()
        {
            this.Pais = new PALocalidadDC()
            {
                IdLocalidad = "057"
            };
        }

        [DataMember]
        [Filtrable("PRL_IdPersonaExterna", "Id:", COEnumTipoControlFiltro.TextBox)]
        [CamposOrdenamiento("PRL_IdPersonaExterna")]
        [Display(ResourceType = typeof(Etiquetas), Name = "Id", Description = "ToolTipId")]
        public long IdResponsable { get; set; }

        /// <summary>
        /// Pais de la factura
        /// </summary>
        [DataMember]
        public PALocalidadDC Pais { get; set; }

        [DataMember]
        public PAPersonaExterna PersonaExterna { get; set; }

        [DataMember]
        [CamposOrdenamiento("PRL_IdPersonaExterna")]
        [Display(ResourceType = typeof(Etiquetas), Name = "Nombre", Description = "ToolTipNombre")]
        public string NombreCompuesto { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Etiquetas), Name = "Ocupacion", Description = "TooltipOcupacion")]
        [StringLength(100, ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        public string Ocupacion { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Etiquetas), Name = "EmpresaEmpleador", Description = "TooltipEmpresaEmpleador")]
        [StringLength(100, ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        public string EmpresaEmpleador { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Etiquetas), Name = "Ingresos", Description = "TooltipIngresosActuales")]
        public decimal IngresosEmpleoActual { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Etiquetas), Name = "PoseeFincaRaiz", Description = "TooltipPoseeFincaRaiz")]
        public bool PoseeFincaRaiz { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Etiquetas), Name = "Telefono", Description = "TooltipTelefono")]
        [StringLength(25, ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        public string Telefono { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Etiquetas), Name = "Fax", Description = "TooltipFax")]
        [StringLength(25, ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        public string Fax { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [RegularExpression(@"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}", ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "EmailNoValido")]
        [Display(ResourceType = typeof(Etiquetas), Name = "Email", Description = "Tooltipemail")]
        [StringLength(100, ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        public string Email { get; set; }

        /// <summary>
        /// Enumeración que indica el estado del objeto dentro de una lista
        /// </summary>
        [DataMember]
        public EnumEstadoRegistro EstadoRegistro { get; set; }

        [DataMember]
        [Filtrable("PEE_Identificacion", typeof(Etiquetas), "Identificacion", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 25)]
        [Display(ResourceType = typeof(Etiquetas), Name = "Identificacion", Description = "TooltipIdentificacion")]
        public string Identificacion { get; set; }

        [DataMember]
        [Filtrable("PEE_PrimerNombre", typeof(Etiquetas), "PrimerNombre", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 50)]
        [Display(ResourceType = typeof(Etiquetas), Name = "PrimerNombre", Description = "TooltipPrimerNombre")]
        public string PrimerNombre { get; set; }

        [DataMember]
        [Filtrable("PEE_PrimerApellido", typeof(Etiquetas), "PrimerApellido", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 50)]
        [Display(ResourceType = typeof(Etiquetas), Name = "PrimerApellido", Description = "TooltipPrimerApellidoPersona")]
        public string PrimerApellido { get; set; }
    }
}