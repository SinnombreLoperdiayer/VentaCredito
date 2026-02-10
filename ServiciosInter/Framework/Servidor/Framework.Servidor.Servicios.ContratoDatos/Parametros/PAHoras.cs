using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Servidor.Servicios.ContratoDatos.Parametros
{
  /// <summary>
  /// Clase que contiene la informacion de las horas del dia
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class PAHoras : DataContractBase
  {


    public event EventHandler OnErrorRango;

    [DataMember]
    public DateTime HoraInicial { get; set; }

    [DataMember]
    public DateTime HoraFinal { get; set; }

    [IgnoreDataMember]
    private object colorError;

    [IgnoreDataMember]
    public object ColorError
    {
      get { return colorError; }
      set
      {
        colorError = value;
        OnPropertyChanged("ColorError");
        if (OnErrorRango != null)
          OnErrorRango(null, null);
      }
    }

    [IgnoreDataMember]
    private string toolTipError;

    [IgnoreDataMember]
    public string ToolTipError
    {
      get { return toolTipError; }
      set
      {
        toolTipError = value;
        if (OnErrorRango != null)
          OnErrorRango(null, null);
        OnPropertyChanged("ToolTipError");
      }
    }


  }
}