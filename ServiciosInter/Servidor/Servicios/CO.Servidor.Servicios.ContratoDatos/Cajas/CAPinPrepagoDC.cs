using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Servicios.ContratoDatos.Cajas
{
  /// <summary>
  /// Clase que contiene la Info del
  /// pin prepago
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class CAPinPrepagoDC : DataContractBase
  {
    /// <summary>
    /// Es el identity pinprepago
    /// </summary>
    /// <value>
    /// The id pin prepago.
    /// </value>
    [DataMember]
    public long IdPinPrepago { get; set; }

    private long pin;

    /// <summary>
    /// Es el id del pin Prepago
    /// </summary>
    /// <value>
    /// The pin.
    /// </value>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "PinPrepago", Description = "TootipPinPrepago")]
    [Filtrable("PIP_Pin", "Numero Pin", COEnumTipoControlFiltro.TextBox)]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido", AllowEmptyStrings = false)]
    public long Pin
    {
      get { return pin; }
      set
      {
        pin = value;
        if (value != 0 && OnIngresoNumeroPinEvent != null)
        {
          OnIngresoNumeroPinEvent();
        }
      }
    }

    private decimal valorPin;

    /// <summary>
    /// Valor del pin prepago
    /// </summary>
    /// <value>
    /// The valor pin.
    /// </value>
    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido", AllowEmptyStrings = false)]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Valor")]
    public decimal ValorPin
    {
      get { return valorPin; }
      set
      {
        valorPin = value;
        if (value != 0 && OnValidarTopePinEvent != null)
        {
          OnValidarTopePinEvent();
        }
      }
    }

    /// <summary>
    /// Es el Saldo pend del pin Prepago.
    /// </summary>
    /// <value>
    /// The saldo pin prepago.
    /// </value>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "SaldoPrepago")]
    public decimal SaldoPinPrepago { get; set; }

    /// <summary>
    /// Es la fecha en la que se registro
    /// el ultimo descuento
    /// </summary>
    /// <value>
    /// The fecha actualizacion.
    /// </value>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FechaActualizacion")]
    public DateTime FechaActualizacion { get; set; }

    /// <summary>
    /// Es el nombre del comprador
    /// </summary>
    /// <value>
    /// The nombre comprador.
    /// </value>
    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido", AllowEmptyStrings = false)]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NombreCliente")]
    public string NombreComprador { get; set; }

    /// <summary>
    /// Es el tipo de Id del comprador.
    /// </summary>
    /// <value>
    /// The tipo id.
    /// </value>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoIdentificacion")]
    public PATipoIdentificacion TipoId { get; set; }

    private string identificacion;

    /// <summary>
    /// Es la identificacion del comprador.
    /// </summary>
    /// <value>
    /// The identificacion.
    /// </value>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Identificacion")]
    [Filtrable("PIP_Identificacion", "Identificación", COEnumTipoControlFiltro.TextBox)]
    [RegularExpression(@"[0-9]*", ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "IdentificacionNoValida")]
    public string Identificacion
    {
      get { return identificacion; }
      set
      {
        identificacion = value;
        if (value != null && OnValidacionCedulaEvent != null)
        {
          OnValidacionCedulaEvent();
        }
      }
    }

    /// <summary>
    /// Es la direccion del comprador.
    /// </summary>
    /// <value>
    /// The direccion.
    /// </value>
    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido", AllowEmptyStrings = false)]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Direccion")]
    public string Direccion { get; set; }

    /// <summary>
    /// Es el id de la localidad
    /// </summary>
    /// <value>
    /// The id localidad.
    /// </value>
    [DataMember]
    public string IdLocalidad { get; set; }

    /// <summary>
    /// Es el id del centro de servicio que
    /// vendio el prepago
    /// </summary>
    /// <value>
    /// The id centro servicio vende.
    /// </value>
    [DataMember]
    public long IdCentroServicioVende { get; set; }

    /// <summary>
    /// Usuario que registra la transaccion
    /// </summary>
    /// <value>
    /// The usuario.
    /// </value>
    [DataMember]
    public string Usuario { get; set; }

    /// <summary>
    /// Fecha de Grabacion del registro
    /// </summary>
    /// <value>
    /// The fecha grabacion.
    /// </value>
    [DataMember]
    public DateTime FechaGrabacion { get; set; }

    #region Eventos

    /// <summary>
    /// Evento de Cambio de Numero Pin
    /// </summary>
    public delegate void OnIngresoNumeroPin();
    public event OnIngresoNumeroPin OnIngresoNumeroPinEvent;

    /// <summary>
    /// Evento para l aValidacion de la Cedula en las listas
    /// negras de venta de pin prepago
    /// </summary>
    public delegate void OnValidacionCedula();
    public event OnValidacionCedula OnValidacionCedulaEvent;

    /// <summary>
    /// Evento de Validacion del Valor del tope del pin
    /// Prepago
    /// </summary>
    public delegate void OnValidarTopePin();
    public event OnValidarTopePin OnValidarTopePinEvent;

    #endregion Eventos
  }
}