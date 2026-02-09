using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Tarifas
{
  /// <summary>
  /// Clase que contiene información de los parámetros de lista de precios servicios
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class TAListaPrecioServicioParametrosDC : DataContractBase
  {
    [DataMember]
    public int IdServicio { get; set; }

    [DataMember]
    public int IdListaPrecio { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "PrimaSeguro", Description = "TooltipPrimaSeguro")]
    public decimal PrimaSeguro { get; set; }
  }
}