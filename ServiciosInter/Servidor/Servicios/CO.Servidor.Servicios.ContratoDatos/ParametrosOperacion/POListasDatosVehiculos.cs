using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion
{
  /// <summary>
  /// Clase que contiene las listas requeridas para la creacion de un vehiculo tipo carro o moto
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class POListasDatosVehiculos : DataContractBase
  {
    [DataMember]
    public ObservableCollection<POColor> LstColor { get; set; }

    [DataMember]
    public ObservableCollection<POConfiguracionVehiculo> LstConfiguracionVehiculo { get; set; }

    [DataMember]
    public ObservableCollection<POTipoCarroceria> LstTipoCarroceria { get; set; }

    [DataMember]
    public ObservableCollection<POMarca> LstMarca { get; set; }

    [DataMember]
    public ObservableCollection<POTipoContrato> LstTipoContrato { get; set; }

    [DataMember]
    public ObservableCollection<POTipoCombustibleDC> LstTipoCombustible { get; set; }
  }
}