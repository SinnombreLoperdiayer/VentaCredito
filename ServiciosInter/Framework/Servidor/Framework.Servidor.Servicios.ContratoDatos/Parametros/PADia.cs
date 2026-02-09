using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Servidor.Servicios.ContratoDatos.Parametros
{
  /// <summary>
  /// Clase que contiene la informacion de los dias de la semana
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class PADia : DataContractBase
  {
    [DataMember]
    public string IdDia { get; set; }

    [DataMember]
    public string NombreDia { get; set; }

    [DataMember]
    public ObservableCollection<PAHoras> lstHoras { get; set; }

    [IgnoreDataMember]
    public bool Seleccionado { get; set; }
  }
}