using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Tarifas.Servicios
{
  /// <summary>
  /// Clase que contiene la información de la tarifa del servicio trámites
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class TATarifaTramitesDC : DataContractBase
  {
    [DataMember]
    public int IdListaPrecio { get; set; }

    [DataMember]
    public int IdServicio { get; set; }

    [DataMember]
    public ObservableCollection<TAServicioTramiteDC> ServicioTramites { get; set; }

    [DataMember]
    public ObservableCollection<TAFormaPago> FormasPago { get; set; }
  }
}