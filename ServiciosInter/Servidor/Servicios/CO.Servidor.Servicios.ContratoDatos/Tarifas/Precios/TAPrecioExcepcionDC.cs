using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Cliente.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Precios;
using Framework.Servidor.Comun;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Servicios.ContratoDatos.Tarifas.Precios
{
  /// <summary>
  /// Clase que contiene la información de las excepciones de trayectos
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class TAPrecioExcepcionDC : DataContractBase
  {
    // private string idCiudadOrigen;

    [DataMember]
    public long IdPrecioExcepcionTrayecto { get; set; }

    [DataMember]
    public int IdListaPrecio { get; set; }

    [DataMember]
    public int IdServicio { get; set; }

    //[DataMember]
    //[Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    //public string IdCiudadOrigen { get; set; }


    private PALocalidadDC ciudadOrigen;
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "LocalidadOrigen", Description = "ToolTipLocalidadOrigen")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [DataMember]
      public PALocalidadDC CiudadOrigen
    {
        get { return ciudadOrigen; }
        set
        {
            ciudadOrigen = value;
            OnPropertyChanged("CiudadOrigen");

        }
    }


    [DataMember]
    public PALocalidadDC PaisOrigen { get; set; }


    //[DataMember]
    //[Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    //public string IdCiudadDestino { get; set; }

    private PALocalidadDC ciudadDestino;
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "LocalidadDestino", Description = "ToolTipLocalidadDestino")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [DataMember]
      public PALocalidadDC CiudadDestino
    {
        get { return ciudadDestino; }
        set
        {
            ciudadDestino = value;
            OnPropertyChanged("CiudadDestino");
        }
    }

    [DataMember]
    public PALocalidadDC PaisDestino { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public TATipoTrayecto TipoTrayecto { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ValorKiloInicial")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public decimal ValorKiloInicial { get; set; }


    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ValorKiloAdicional")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public decimal ValorKiloAdicional { get; set; }

    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }
    [DataMember]
    public bool? EsDestinoTodoElPais { get; set; }

    [DataMember]
    public bool? EsOrigenTodoElPais { get; set; }

   
    private decimal pesoInicial;
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "PesoInicial")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public decimal PesoInicial
    {
        get { return pesoInicial; }
        set {

            if (value != 0)
            {
                if(value<PesoFinal)
                    pesoInicial = value;
            }
            else
                if(PesoFinal==0)
                    pesoInicial = value;

        OnPropertyChanged("PesoInicial");
        }
    }


    private decimal pesoFinal;
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "PesoFinal")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public decimal PesoFinal
    {
        get { return pesoFinal; }
        set {

            if (value != 0)
            {
                if(value>PesoInicial)
                    pesoFinal = value;
            }
            else
                if (PesoInicial == 0)
                   pesoFinal = value;
            
            OnPropertyChanged("PesoFinal"); }
    }

  }
}