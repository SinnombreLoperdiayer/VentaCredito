using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Tarifas.Servicios
{
  /// <summary>
  /// Clase que contiene la información de servicio centro de correspondencia
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class TATarifaCentroDeCorrespondenciaDC : DataContractBase
  {
    [DataMember]
    public int IdListaPrecio { get; set; }

    [DataMember]
    public int IdServicio { get; set; }

    [DataMember]
    public ObservableCollection<TAServicioCentroDeCorrespondenciaDC> ServicioCentroDeCorrespondencia { get; set; }

    [DataMember]
    public ObservableCollection<TAFormaPago> FormasPago { get; set; }

    [DataMember]
    public ObservableCollection<TAImpuestosDC> Impuestos { get; set; }
  }
}