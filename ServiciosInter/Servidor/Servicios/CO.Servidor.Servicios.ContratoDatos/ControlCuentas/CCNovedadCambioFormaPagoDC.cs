using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;

namespace CO.Servidor.Servicios.ContratoDatos.ControlCuentas
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class CCNovedadCambioFormaPagoDC : ADNovedadGuiaDC
  {
    /// <summary>
    /// Forma de pago a cambiar
    /// </summary>
    [DataMember]
    public ADGuiaFormaPago FormaPagoAnterior { get; set; }

    /// <summary>
    /// Forma de pago nueva
    /// </summary>
    [DataMember]
    public TAFormaPago FormaPagoNueva { get; set; }

    /// <summary>
    /// Valor de contado cuando el cambio es por un pago mixto
    /// </summary>
    [DataMember]
    public decimal ValorContadoMixta { get; set; }

    /// <summary>
    /// Valor prepago cuando el cambio es por un pago mixto
    /// </summary>
    [DataMember]
    public decimal ValorPrepagoMixta { get; set; }
  }
}