using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Tarifas
{
  /// <summary>
  /// Clase que contiene la información de tarifa rapicarga
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class TATarifaRapiCargaDC : DataContractBase
  {
    [DataMember]
    public ObservableCollection<TAPrecioTrayectoDC> ServicioRapiCarga { get; set; }

    [DataMember]
    public ObservableCollection<TAFormaPago> FormasPago { get; set; }

    [DataMember]
    public TAServicioPesoDC ServicioPeso { get; set; }

    [DataMember]
    public TAListaPrecioServicioParametrosDC ListaPrecioParametros { get; set; }

    [DataMember]
    public ObservableCollection<TAImpuestosDC> Impuestos { get; set; }
  }
}