using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Servicios.ContratoDatos.Tarifas.Trayectos
{
  /// <summary>
  /// Clase que contiene la información de trayectos de mensajería
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class TATrayectoMensajeriaDC : DataContractBase
  {
    public event EventHandler OnCambioPorcentaje;

    [DataMember]
    public long IdPrecioTrayectoSubTrayecto { get; set; }

    [DataMember]
    public int IdListaPrecio { get; set; }

    [DataMember]
    public int IdServicio { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public TATipoTrayecto Trayecto { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ValorKiloInicial")]
    public decimal KiloInicial { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ValorKiloInicial")]
    public decimal ValorBaseKiloInicial { get; set; }

    [DataMember]
    public ObservableCollection<TAPrecioTrayectoMensajeriaDC> PrecioTrayecto { get; set; }

    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }

    private decimal porcentajeIncremento;

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Incremento")]
    public decimal PorcentajeIncremento
    {
      get { return porcentajeIncremento; }
      set
      {
        porcentajeIncremento = value;
        if (OnCambioPorcentaje != null)
          OnCambioPorcentaje(null, null);
      }
    }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ValorPropuestoKiloInicial")]
    public decimal ValorPropuestoKiloInicial { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ValorFijadoKiloInicial")]
    public decimal ValorFijadoKiloInicial { get; set; }
  }
}