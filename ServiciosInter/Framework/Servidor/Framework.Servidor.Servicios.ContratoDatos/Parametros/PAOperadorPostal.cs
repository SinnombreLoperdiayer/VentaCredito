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
  [DataContract(Namespace = "http://contrologis.com")]
  public class PAOperadorPostal : DataContractBase
  {
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "OperadorPostal", Description = "TooltipOperadorPostal")]
    public int Id { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Filtrable("OPO_Nombre", "Operador Postal", COEnumTipoControlFiltro.TextBox)]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "OperadorPostal", Description = "TooltipOperadorPostal")]
    public string Nombre { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Descripcion", Description = "TooltipOperadorPostal")]
    public string Descripcion { get; set; }

    [DataMember]
    public string IdZona { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FormulaPesoVolumetrico", Description = "FormulaPesoVolumetrico")]
    public string FormulaPesoVolumetrico { get; set; }

    /// <summary>
    /// Tiempo de entrega para la zona designada
    /// </summary>
    [DataMember]
    public int TiempoEntrega { get; set; }

    /// <summary>
    /// Porcentaje al recargo de combustible
    /// </summary>
    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "RecargoCombustible", Description = "RecargoCombustible")]
    public decimal PorcentajeCombustible { get; set; }

    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }
  }
}