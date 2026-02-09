using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;

namespace Framework.Servidor.Servicios.ContratoDatos.Seguridad
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class SEMenu : DataContractBase
  {
    [DataMember]
    [CamposOrdenamiento("MEN_IdMenu")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "IdMenu")]
    public int IdMenu { get; set; }

    [DataMember]
    [Filtrable("MEN_Etiqueta", "Descripción:", COEnumTipoControlFiltro.TextBox)]
    [CamposOrdenamiento("MEN_Etiqueta")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Descripcion")]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public string Etiqueta { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Assembly", Description = "TooltipAssemblyMenu")]
    public string Assembly { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NameSpace", Description = "TooltipNameSpaceMenu")]
    public string NameSpace { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public string UserControl { get; set; }

    [DataMember]
    [CamposOrdenamiento("MEN_Comentarios")]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Comentarios")]
    public string Comentarios { get; set; }

    /// <summary>
    /// Enumeracion que indica el estado del objeto dentro de una lista
    /// </summary>
    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }

    [DataMember]
    [CamposOrdenamiento("MEN_UrlRelativa")]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "UrlRelativa", Description = "TooltipUrlRelativaMenu")]
    public string UrlRelativa { get; set; }

    [DataMember]
    public List<string> Acciones { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Modulo")]
    public string IdModulo { get; set; }

    [DataMember]
    [Filtrable("MEN_IdModulo", "Módulo:", COEnumTipoControlFiltro.TextBox)]
    [CamposOrdenamiento("MEN_IdModulo")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Modulo")]
    public string NomModulo { get; set; }

    [DataMember]
    public bool AplicaPunto { get; set; }

    [DataMember]
    public bool AplicaAgencia { get; set; }

    [DataMember]
    public bool AplicaCol { get; set; }

    [DataMember]
    public bool AplicaRacol { get; set; }

    [DataMember]
    public bool AplicaGestion { get; set; }

    [DataMember]
    public bool AplicaServidorCredito { get; set; }

    [DataMember]
    public string MenuWPF { get; set; }

    [DataMember]
    public string Imagen { get; set; }

    [DataMember]
    public bool AplicaClienteCredito { get; set; }
    }
}