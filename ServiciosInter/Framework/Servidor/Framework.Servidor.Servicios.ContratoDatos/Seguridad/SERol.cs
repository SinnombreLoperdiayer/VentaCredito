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
  /// <summary>
  /// Clase que representa un rol de seguridad
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class SERol : DataContractBase
  {
    [DataMember]
    [StringLength(10, ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
    [Filtrable("ROL_IdRol", "Id Rol:", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 10)]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "IdRol")]
    [CamposOrdenamiento("ROL_IdRol")]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public string IdRol { get; set; }

    [DataMember]
    [StringLength(50, ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
    [Filtrable("ROL_Descripcion", "Descripción:", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 50)]
    [CamposOrdenamiento("ROL_Descripcion")]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Descripcion")]
    public string Descripcion { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "RequiereIdentificadorMaquina", Description = "TooltipRequiereIdMaquinaUsuario")]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public bool RequiereIdMaquina { get; set; }

    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }
  }
}