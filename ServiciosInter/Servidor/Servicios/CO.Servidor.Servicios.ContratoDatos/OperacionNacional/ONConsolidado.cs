using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Servicios.ContratoDatos.OperacionNacional
{
  /// <summary>
  /// Clase que contiene la informacion de los consolidados de los manifietos de la operacion nacional
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class ONConsolidado : DataContractBase
  {
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NumeroConsolidado", Description = "NumeroConsolidado")]
    public long IdManfiestoConsolidado { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NumeroManifiesto", Description = "NumeroManifiesto")]
    public long IdManifiestoOperacionNacional { get; set; }

    [DataMember]
    public int ConsecutivoConsolidado { get; set; }

    /// <summary>
    /// Ciudad estacion destino
    /// </summary>
    [DataMember]
    [Filtrable("RUT_NombreLocalidadDestino", typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), "LocalidadDestino", COEnumTipoControlFiltro.TextBox)]
    public PALocalidadDC LocalidadManifestada { get; set; }

    [DataMember]
    public string IdLocalidadDespacha { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoConsolidado", Description = "TipoConsolidado")]    
    public int IdTipoConsolidado { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoConsolidado", Description = "TipoConsolidado")]
    public string NombreTipoConsolidado { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "DetalleTipoConsolidado", Description = "DetalleTipoConsolidado")]    
    public int IdTipoConsolidadoDetalle { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoConsolidado", Description = "TipoConsolidado")]
    public string NombreTipoConsolidadoDetalle { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "DetalleConsolidado", Description = "DetalleConsolidado")]
    public string DescripcionConsolidadoDetalle { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NumTulaContendedor", Description = "NumTulaContendedor")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public string NumeroContenedorTula { get; set; }

    /// <summary>
    /// Retorna o asigna el numero de tula o contenedor en la ingreso al col
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NumTulaContendedor", Description = "NumTulaContendedor")]
    public string NumeroContenedorTulaLlegada { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NumeroGuia", Description = "TooltipNumeroGuia")]    
    public long? IdGuiaInterna { get; set; }

    [DataMember]    
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "GuiaConsolidado", Description = "GuiaConsolidado")]
    public long? NumeroGuiaInterna { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NumeroPrecintoRetorno", Description = "NumeroPrecintoRetorno")]
    public long? NumeroPrecintoRetorno { get; set; }

    [DataMember]    
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NumeroPrecintoSalida", Description = "NumeroPrecintoSalida")]
    public long? NumeroPrecintoSalida { get; set; }

    /// <summary>
    /// retorna o asigna el numero de precinto al ingreso al col
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NumeroPrecinto", Description = "NumeroPrecinto")]
    public long? NumeroPrecintoIngreso { get; set; }

    [DataMember]
    public DateTime FechaGrabacion { get; set; }

    /// <summary>
    /// Enumeración que indica el estado del objeto dentro de una lista
    /// </summary>
    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }

    /// <summary>
    /// Detalle de un consolidado
    /// </summary>
    [DataMember]
    public ONConsolidadoDetalle DetalleConsolidado { get; set; }

    /// <summary>
    /// Lista Envios de un consolidado
    /// </summary>
    [DataMember]
    public List<ONConsolidadoDetalle> EnviosConsolidados { get; set; }

    /// <summary>
    /// Retorna o asigna el total de los envios consolidados
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TotalEnvios", Description = "TotalEnvios")]
    public decimal TotalEnviosConsolidado { get; set; }

    /// <summary>
    /// Retorna o asigna el total de los envios consolidados
    /// </summary>    
    private bool estaActivo;
    /// <summary>
    /// Retorna o asigna el total de los envios consolidados
    /// </summary>
    [DataMember]
    public bool EstaActivo
    {
        get { return estaActivo; }
        set { estaActivo = value; OnPropertyChanged("EstaActivo"); }
    }
    
    [DataMember]
    public long NumControlTransManIda { get; set; }
     
    [DataMember]
    public long? NumControlTransManRet { get; set; }
    [DataMember]
    public int IdRutaDespacho { get; set; }

    [DataMember]
    public string NumeroGuiaRotulo { get; set; }

  }
}