using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Tarifas
{
  /// <summary>
  /// Clase que contiene la información de las unidades de negocio
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class TAUnidadNegocio : DataContractBase
  {
    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "IdUnidadNegocio", Description = "TooltipUniNegocio")]
    public string IdUnidadNegocio { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "IdUnidadNegocio", Description = "TooltipUniNegocio")]

    public string Nombre { get; set; }

    [DataMember]
    public string Descripcion { get; set; }

    [DataMember]
    public bool Seleccionado { get; set; }
  }
}