using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Cliente.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Facturacion
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class FAOperacionFacturadaDC
  {
    [DataMember]
    public int IdServicio { get; set; }

    [DataMember]
    public string NombreServicio { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NumeroFactura", Description = "TooltipNumeroFactura")]
    public long NoFactura { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NumeroOperacion", Description = "ToolTipNumeroOperacion")]
    public long NoOperacion { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Valor", Description = "Valor")]
    public decimal Valor { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Cliente", Description = "Cliente")]
    public int IdCliente { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "RazonSocial", Description = "TooltipRazonSocial")]
    public string RazonSocialCliente { get; set; }

    [DataMember]
    public DateTime Fecha { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Usuario", Description = "TooltipUsuarioLiquidacion")]
    public string Usuario { get; set; }
  }
}