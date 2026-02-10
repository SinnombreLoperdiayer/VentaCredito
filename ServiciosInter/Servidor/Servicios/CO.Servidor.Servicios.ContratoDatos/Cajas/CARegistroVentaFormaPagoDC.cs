using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Cajas
{
  /// <summary>
  /// Clase que contiene las formas de pago de
  /// una venta
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class CARegistroVentaFormaPagoDC : DataContractBase
  {
    /// <summary>
    /// Gets or sets the numero asociado.
    /// </summary>
    /// <value>
    /// Es el numero que puede estar asociado
    /// a una transaccion de credito o prepago ó cheque
    /// aqui se registra el numero cheque ó autorización.
    /// </value>
    [DataMember]
    public string NumeroAsociado { get; set; }

    /// <summary>
    /// Gets or sets the campo01.
    /// </summary>
    /// <value>
    /// Campo variable dependiendo la
    /// forma de pago
    /// </value>
    [DataMember]
    public string Campo01 { get; set; }

    /// <summary>
    /// Gets or sets the campo02.
    /// </summary>
    /// <value>
    /// Campo variable dependiendo la
    /// forma de pago
    /// </value>
    [DataMember]
    public string Campo02 { get; set; }

    /// <summary>
    /// Gets or sets the id forma pago.
    /// </summary>
    /// <value>
    /// Es le Id de la Forma de Pago.
    /// </value>
    [DataMember]
    public short IdFormaPago { get; set; }

    /// <summary>
    /// Gets or sets the descripcion.
    /// </summary>
    /// <value>
    /// Es la descripcion de la Forma de Pago
    /// </value>
    [DataMember]
    public string Descripcion { get; set; }

    /// <summary>
    /// Gets or sets the valor.
    /// </summary>
    /// <value>
    /// Es el valor de la forma de pago
    /// </value>
    [DataMember]
    public decimal Valor { get; set; }
  }
}