using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;

namespace CO.Servidor.Servicios.ContratoDatos.Tarifas
{
  /// <summary>
  /// Clase que contiene la información del precio trayecto rango
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class TAPrecioTrayectoDC : INotifyPropertyChanged
  {
    private ObservableCollection<TATipoSubTrayecto> subTrayectosDisponibles;

    private ObservableCollection<TATipoTrayecto> totalTrayectos;

    private TATipoTrayecto tipoTrayecto;

    private TATipoSubTrayecto tipoSubTrayecto { get; set; }

    [DataMember]
    public long IdPrecioTrayectoSubTrayecto { get; set; }

    [DataMember]
    public int IdTrayectoSubTrayecto { get; set; }

    [DataMember]
    public int IdServicio { get; set; }

    [DataMember]
    public int IdListaPrecio { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public TATipoTrayecto TipoTrayecto
    {
      get { return tipoTrayecto; }
      set
      {
        if (this.tipoTrayecto != value)
        {
          this.tipoTrayecto = value;

          if (TotalTrayectosSubTrayectos != null)
          {
            if (SubTrayectosDisponibles == null)
            {
              SubTrayectosDisponibles = new ObservableCollection<TATipoSubTrayecto>();
            }
            else
            {
              SubTrayectosDisponibles.Clear();
            }

            foreach (TATrayectoSubTrayectoDC trayectoSubtrayectoItem in TotalTrayectosSubTrayectos
              .Where(std => std.IdTipoTrayecto == TipoTrayecto.IdTipoTrayecto)
              .OrderBy(o => o.DescripcionTipoSubTrayecto))
            {
              SubTrayectosDisponibles.Add(new TATipoSubTrayecto
              {
                IdTipoSubTrayecto = trayectoSubtrayectoItem.IdTipoSubTrayecto,
                Descripcion = trayectoSubtrayectoItem.DescripcionTipoSubTrayecto,
                EstadoRegistro = Framework.Servidor.Comun.EnumEstadoRegistro.SIN_CAMBIOS
              });
            }
          }

          if (PropertyChanged != null)
          {
            PropertyChanged(this, new PropertyChangedEventArgs("TipoTrayecto"));
          }
        }
      }
    }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public TATipoSubTrayecto TipoSubTrayecto
    {
      get { return this.tipoSubTrayecto; }
      set
      {
        if (this.tipoSubTrayecto != value)
        {
          this.tipoSubTrayecto = value;
          if (PropertyChanged != null)
          {
            PropertyChanged(this, new PropertyChangedEventArgs("TipoSubTrayecto"));
          }
        }
      }
    }

    [DataMember]
    public string IdTipoValorAdicional { get; set; }

    [DataMember]
    [CamposOrdenamiento("KiloAdicional")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "KiloAdicional")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public decimal KiloAdicional { get; set; }

    [DataMember]
    [CamposOrdenamiento("ServicioRetorno")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ServicioRetorno")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public decimal ServicioRetorno { get; set; }

    [DataMember]
    public ObservableCollection<TAPrecioTrayectoRangoDC> Precio { get; set; }

    [IgnoreDataMember]
    public TAPrecioTrayectoRangoDC PrecioSeleccionado { get; set; }

    public ObservableCollection<TATipoSubTrayecto> SubTrayectosDisponibles
    {
      get { return subTrayectosDisponibles; }
      set
      {
        if (this.subTrayectosDisponibles != value)
        {
          this.subTrayectosDisponibles = value;

          if (PropertyChanged != null)
          {
            PropertyChanged(this, new PropertyChangedEventArgs("SubTrayectosDisponibles"));
          }
        }
      }
    }

    public ObservableCollection<TATrayectoSubTrayectoDC> TotalTrayectosSubTrayectos { get; set; }

    public ObservableCollection<TATipoTrayecto> TotalTrayectos
    {
      get { return totalTrayectos; }
      set
      {
        if (this.totalTrayectos != value)
        {
          this.totalTrayectos = value;

          if (PropertyChanged != null)
          {
            PropertyChanged(this, new PropertyChangedEventArgs("TotalTrayectos"));
          }
        }
      }
    }

    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
  }
}