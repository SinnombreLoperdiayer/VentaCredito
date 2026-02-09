using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Clase que contiene la informacion de la localidad
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class PALocalidadDC : DataContractBase
    {
        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Codigo", Description = "ToolTipCodigoLocalidad")]
        public string IdLocalidad { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoLocalidad", Description = "ToolTipTipoLocalidad")]
        public string IdTipoLocalidad { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Padre", Description = "ToolTipPadre")]
        public string IdAncestroPGrado { get; set; }

        [DataMember]
        public string IdAncestroSGrado { get; set; }

        [DataMember]
        public string NombreAncestroSGrado { get; set; }

        [DataMember]
        public string IdAncestroTGrado { get; set; }

        [DataMember]
        public string NombreAncestroTGrado { get; set; }

        [DataMember]
        [CamposOrdenamiento("LOC_Nombre")]
        [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Filtrable("LOC_Nombre", "Nombre Localidad", COEnumTipoControlFiltro.TextBox)]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Municipio", Description = "ToolTipNombreLocalidad")]
        [StringLength(100, ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        public string Nombre { get; set; }

        [DataMember]
        [CamposOrdenamiento("LOC_NombreCorto")]
        [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Filtrable("LOC_NombreCorto", "Nombre Corto Localidad", COEnumTipoControlFiltro.TextBox)]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NombreCorto", Description = "ToolTipNombreCortoLocalidad")]
        [StringLength(4, ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        public string NombreCorto { get; set; }

        [DataMember]
        [CamposOrdenamiento("LOC_Nombre")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NombrePadre", Description = "ToolTipNombrePadreLocalidad")]
        public string NombreAncestroPGrado { get; set; }

        [DataMember]
        [CamposOrdenamiento("NombreCompleto")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Nombre", Description = "TooltipNombreLocalConPadre")]
        public string NombreCompleto { get; set; }

        [DataMember]
        [CamposOrdenamiento("TLO_Descripcion")]
        //[Filtrable("TLO_Descripcion", "Tipo Localidad", COEnumTipoControlFiltro.ComboBox)]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoLocalidad", Description = "ToolTipTipoLocalidad")]
        public string NombreTipoLocalidad { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Asignado")]
        public bool AsignadoEnZona { get; set; }

        [DataMember]
        public bool AsignadoEnZonaOrig { get; set; }

        [DataMember]
        public bool DispoLocalidad { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ZonaAsignada")]
        public string NombreZona { get; set; }

        public bool HabilitarCheck
        {
            get
            {
                if (DispoLocalidad || (AsignadoEnZonaOrig && !DispoLocalidad))
                    return true;
                else
                    return false;
            }
        }

        [DataMember]
        [CamposOrdenamiento("LOC_CodigoPostal")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "CodigoPostal")]
        [StringLength(25, ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        public string CodigoPostal { get; set; }

        [DataMember]
        public string Indicativo { get; set; }

        [DataMember]
        public long IdCentroServicio { get; set; }
        /// <summary>
        /// Enumeracion que indica el estado del objeto dentro de una lista
        /// </summary>
        [DataMember]
        public EnumEstadoRegistro EstadoRegistro { get; set; }

        public ObservableCollection<PATipoLocalidad> TiposLocalidades { get; set; }
    }
}