using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos.Seguridad;

namespace Framework.Servidor.Servicios.ContratoDatos.Agenda
{
  /// <summary>
  /// Contiene la información de un usuario de la aplicación
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class ASUsuario : DataContractBase
  {
    /// <summary>
    /// Primer apellido del usuario
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "PrimerApellido", Description = "TooltipPrimerApellido")]
    [Filtrable("PEI_PrimerApellido", "Primer Apellido", COEnumTipoControlFiltro.TextBox)]
    [CamposOrdenamiento("PEI_PrimerApellido")]
    public string PrimerApellido { get; set; }

    /// <summary>
    /// Cargo del usuario
    /// </summary>
    [DataMember]
    public ASCargo Cargo { get; set; }

    /// <summary>
    /// Nombre del usuario
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Nombre", Description = "TooltipNombre")]
    [Filtrable("PEI_Nombre", "Nombre", COEnumTipoControlFiltro.TextBox)]
    [CamposOrdenamiento("PEI_Nombre")]
    public string Nombre { get; set; }

    /// <summary>
    /// Segundo apellido del usuario
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "SegundoApellido")]
    [CamposOrdenamiento("PEI_SegundoApellido")]
    public string SegundoApellido { get; set; }

    /// <summary>
    /// Regional a la que pertenece el usuario
    /// </summary>
    [DataMember]
    public SERegional Regional { get; set; }

    /// <summary>
    /// Centro Logístico al que pertenece el usuario
    /// </summary>
    [DataMember]
    public SECentroLogistico CentroLogistico { get; set; }

    [DataMember]
    [Filtrable("USU_IdUsuario", "Usuario", COEnumTipoControlFiltro.TextBox)]
    [CamposOrdenamiento("USU_IdUsuario")]
    public string IdUsuario { get; set; }

    [IgnoreDataMember]
    public string TipoUsuario { get; set; }
  }
}