using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Cajas
{
  /// <summary>
  /// Clase que contiene la Información del
  /// registro del dinero a la agencia
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class CARegistroDineroAgenciaDC
  {
    /// <summary>
    /// Gets or sets the bolsa seguridad.
    /// </summary>
    /// <value>
    /// Es el numero de la Bolsa de Seguridad.
    /// </value>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "BolsaSeguridad", Description = "TooltipBolsaSeguridad")]
    public string BolsaSeguridad { get; set; }

    /// <summary>
    /// Gets or sets the saldo anterior efectivo.
    /// </summary>
    /// <value>
    /// Es el saldo en Efectivo del Cierre Anterior.
    /// </value>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TotalAEmpresa", Description = "ToolTipTotalAEmpresa")]
    public decimal TotalAEmpresa { get; set; }

    /// <summary>
    /// Gets or sets the id punto atencion.
    /// </summary>
    /// <value>
    /// Es el punto de atencion al que corresponde la caja.
    /// </value>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoAgencia", Description = "ToolTipTipoAgencia")]
    public long IdPuntoAtencion { get; set; }

    /// <summary>
    /// Gets or sets the doc mensajero.
    /// </summary>
    /// <value>
    /// Es el documento del mensajero.
    /// </value>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Identificacion", Description = "TooltipIdentificacion")]
    public long? DocMensajero { get; set; }

    /// <summary>
    /// Gets or sets the nombre mensajero.
    /// </summary>
    /// <value>
    /// Es el nombre del mensajero.
    /// </value>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NombreMensajero")]
    public string NombreMensajero { get; set; }
  }
}