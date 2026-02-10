using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Cajas
{
  /// <summary>
  /// Clase que contiene la informacion del
  /// detalle de la compra del pin Prepago
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class CAPinPrepagoDtllCompraDC : DataContractBase
  {
    /// <summary>
    /// Es el numero del PinPrepago
    /// </summary>
    /// <value>
    /// The pin prepago.
    /// </value>
    [DataMember]
    [Display(ResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Etiquetas), Name = "PinPrepago")]
    public long PinPrepago { get; set; }

    /// <summary>
    /// puede ser el numero de la factura ó numero de la Guia asociada.
    /// </summary>
    /// <value>
    /// The numero documento.
    /// </value>
    [DataMember]
    [Display(ResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Etiquetas), Name = "Numero")]
    public long NumeroDocumento { get; set; }

    /// <summary>
    /// Es el Concepto de Caja.
    /// </summary>
    /// <value>
    /// The descripcion concepto caja.
    /// </value>
    [DataMember]
    [Display(ResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Etiquetas), Name = "Concepto")]
    public string DescripcionConceptoCaja { get; set; }

    /// <summary>
    /// Es la Cantidad de Productos
    /// </summary>
    /// <value>
    /// The cantidad.
    /// </value>
    [DataMember]
    [Display(ResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Etiquetas), Name = "Cantidad")]
    public int Cantidad { get; set; }

    /// <summary>
    /// Es le valor del servicio comprado.
    /// </summary>
    /// <value>
    /// The valor servicio.
    /// </value>
    [DataMember]
    [Display(ResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Etiquetas), Name = "Valor")]
    public decimal ValorServicio { get; set; }

    /// <summary>
    /// Es la fecha en la que se adquirio el Servicio
    /// </summary>
    /// <value>
    /// The fecha compra.
    /// </value>
    [DataMember]
    [Display(ResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Etiquetas), Name = "FechaTransaccion")]
    public DateTime FechaCompra { get; set; }
  }
}