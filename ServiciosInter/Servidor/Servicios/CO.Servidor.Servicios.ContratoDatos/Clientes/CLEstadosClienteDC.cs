using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Clientes
{
  /// <summary>
  /// Clase con el DataContract de los estados del cliente
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class CLEstadosClienteDC : DataContractBase
  {
    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public int IdEstadoCliente { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public int IdCliente { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public string Estado { get; set; }

    [DataMember]
    [CamposOrdenamiento("ESC_Estado")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Estado", Description = "TooltipEstado")]
    public string EstadoDescripcion { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public short Motivo { get; set; }

    [DataMember]
    [CamposOrdenamiento("ESC_Motivo")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Motivo", Description = "TooltipMotivo")]
    public string MotivoDescripcion { get; set; }

    [DataMember]
    [CamposOrdenamiento("ESC_Observaciones")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [StringLength(100, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Observaciones", Description = "TooltipObservaciones")]
    public string Observaciones { get; set; }

    [DataMember]
    [CamposOrdenamiento("ESC_FechaGrabacion")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Fecha", Description = "TooltipFecha")]
    public DateTime Fecha { get; set; }

    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }
  }
}