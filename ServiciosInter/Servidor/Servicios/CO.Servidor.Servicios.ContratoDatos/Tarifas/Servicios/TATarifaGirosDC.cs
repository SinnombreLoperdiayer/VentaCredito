using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Precios;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Tarifas.Servicios
{
  /// <summary>
  /// Clase que contiene la información de servicio giro
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class TATarifaGirosDC : DataContractBase
  {
    [DataMember]
    public int IdListaPrecio { get; set; }

    [DataMember]
    public ObservableCollection<TAFormaPago> FormasPago { get; set; }

    [DataMember]
    public ObservableCollection<TAPrecioRangoDC> PrecioRango { get; set; }

    [DataMember]
    public ObservableCollection<TAImpuestosDC> Impuestos { get; set; }
  }
}