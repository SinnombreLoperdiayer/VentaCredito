using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;
using System.Collections.ObjectModel;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;

namespace CO.Servidor.Servicios.ContratoDatos.Cajas
{
  /// <summary>
  /// Clase que contiene las clases
  /// de Registro de Caja, Cta Mensajero,Registro Mensajero
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class CARecibirDineroMensajeroDC : DataContractBase
  {
    /// <summary>
    /// Es el registro de la Transaccion del
    /// mensajero en la caja
    /// </summary>
    /// <value>
    /// The registro en caja mensajero.
    /// </value>
    [DataMember]
    public CARegistroTransacCajaDC RegistroEnCajaMensajero { get; set; }

    /// <summary>
    /// Se registra la Transaccion del mensajero en un Historico
    /// </summary>
    /// <value>
    /// The reporte mensajero.
    /// </value>
    [DataMember]
    public CAReporteMensajeroCajaDC ReporteMensajero { get; set; }

    /// <summary>
    /// Se afecta la Cuenta del mensajero
    /// </summary>
    /// <value>
    /// The cuenta mensajero.
    /// </value>
    [DataMember]
    public CACuentaMensajeroDC CuentaMensajero { get; set; }

    /// <summary>
    /// Si el reporte de dinero se hizo con un numero de autorización esta valor será mayor a 0
    /// </summary>
    [DataMember]
    public long NumeroAutorizacion { get; set; }

    /// <summary>
    /// Listado de al cobros descargados
    /// </summary>
    [DataMember]
    public List<OUEnviosPendMensajerosDC> AlcobrosDescargados { get; set; }      
  }
}