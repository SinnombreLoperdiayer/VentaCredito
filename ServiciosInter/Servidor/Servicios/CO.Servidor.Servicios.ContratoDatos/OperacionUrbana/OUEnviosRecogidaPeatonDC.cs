using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Servicios.ContratoDatos.OperacionUrbana
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class OUEnviosRecogidaPeatonDC : DataContractBase
  {
    [DataMember]
    public TATipoEnvio TipoEnvio { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "CiudadDestino", Description = "CiudadDestino")]
    public PALocalidadDC MunicipioDestino { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "CantidadEnvios", Description = "CantidadEnvios")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public short? CantidadEnvios { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "PesoAproximado", Description = "PesoAproximado")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public decimal? PesoAproximado { get; set; }
  }
}