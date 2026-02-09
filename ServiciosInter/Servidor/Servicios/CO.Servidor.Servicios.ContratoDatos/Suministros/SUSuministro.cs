using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Servicios.ContratoDatos.Suministros
{
  /// <summary>
  /// Contiene información de un suministro
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class SUSuministro : DataContractBase
  {
    /// <summary>
    /// Identificador
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "IdSuministro", Description = "IdSuministro")]
    [CamposOrdenamiento("SUM_IdSuministro")]
    public int Id { get; set; }

    /// <summary>
    /// Descripción
    /// </summary>
    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [CamposOrdenamiento("SUM_Descripcion")]
    [Filtrable("SUM_Descripcion", "Descripción: ", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 25)]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Descripcion", Description = "Descripcion")]
    public string Descripcion { get; set; }

    /// <summary>
    /// Prefijo del suministro
    /// </summary>
    [DataMember]
    public string Prefijo { get; set; }

    /// <summary>
    /// Rangos asignados al suministro
    /// </summary>
    [DataMember]
    public List<SURango> RangosAsignados { get; set; }

    /// <summary>
    /// Suministro en terminos de enumeración
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Suministro", Description = "Suministro")]
    public SUEnumSuministro Suministro { get; set; }

    /// <summary>
    /// Categoria del suministro
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Categoria", Description = "Categoria")]
    public SUEnumCategoria Categoria { get; set; }

    /// <summary>
    /// Codigo de novasoft
    /// </summary>
    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "CodigoNovaSoft", Description = "CodigoNovaSoft")]
    [Filtrable("SUM_CodigoERP", "Código NovaSoft: ", COEnumTipoControlFiltro.TextBox)]
    [CamposOrdenamiento("SUM_CodigoERP")]
    public string CodigoERP { get; set; }

    /// <summary>
    /// Codigo de novasoft
    /// </summary>
    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "CodigoAlterno", Description = "CodigoAlterno")]
    [CamposOrdenamiento("SUM_CodigoAlterno")]
    public string CodigoAlterno { get; set; }

    /// <summary>
    /// Cuenta
    /// </summary>
    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "CuentaGasto", Description = "CuentaGasto")]
    public string CuentaGasto { get; set; }

    /// <summary>
    /// Aplica resolucion
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "AplicaResolucion", Description = "AplicaResolucion")]
    public bool AplicaResolucion { get; set; }

    /// <summary>
    /// indica si se preimprime el suministro
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "SePreimprime", Description = "SePreimprime")]
    [CamposOrdenamiento("SUM_SePreImprime")]
    public bool SePreimprime { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "UnidadMedida", Description = "UnidadMedida")]
    public Framework.Servidor.Servicios.ContratoDatos.Parametros.PAUnidadMedidaDC UnidadMedida { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Activo", Description = "Activo")]
    public bool EstaActivo { get; set; }

    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "SePreimprime", Description = "SePreimprime")]
    public decimal CantidadInicialAutorizada { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "StockMinimoSuministro", Description = "StockMinimoSuministro")]
    public decimal StockMinimo { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "SuministroAutorizado", Description = "SuministroAutorizado")]
    public bool SuministroAutorizado { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Categoria", Description = "Categoria")]
    public SUCategoriaSuministro CategoriaSuministro { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "RangoInicial", Description = "RangoInicial")]
    public long RangoInicial
    {
      get { return rangoInicial; }
      set
      {
        rangoInicial = value;
        if (RangoInicial != 0)
        {
          if (CantidadAsignada != 0)
            RangoFinal = RangoInicial + CantidadAsignada - 1;
          if (CantidadAsignada == 0)
            RangoFinal = 0;
          OnPropertyChanged("RangoInicial");
          OnPropertyChanged("RangoFinal");
        }
      }
    }

    private long rangoInicial;

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "RangoFinal", Description = "RangoFinal")]
    public long RangoFinal { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "RangoFinal", Description = "RangoFinal")]
    public int CantidadAsignada
    {
      get { return cantidadAsignada; }
      set
      {
        cantidadAsignada = value;
        if (RangoInicial != 0)
          RangoFinal = RangoInicial + CantidadAsignada - 1;
        else
          RangoInicial = 0;
        if (CantidadAsignada == 0)
          RangoFinal = 0;
        OnPropertyChanged("RangoInicial");
        OnPropertyChanged("RangoFinal");
      }
    }

    private int cantidadAsignada;

    [DataMember]
    public long IdAsignacionSuministro { get; set; }

    [DataMember]
    public SURango Rango { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FechaInicioResolucion", Description = "FechaInicioResolucion")]
    public DateTime FechaInicialResolucion { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FechaFinalResolucion", Description = "FechaFinalResolucion")]
    public DateTime FechaFinalResolucion { get; set; }

    [DataMember]
    public int IdProvisionSuministroSerial { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "IdentificacionPropietario", Description = "IdentificacionPropietario")]
    public string IdPropietario { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NombrePropietario", Description = "NombrePropietario")]
    public string NombrePropietario { get; set; }

    [DataMember]
    public string IdResolucion { get; set; }

    [DataMember]
    public int IdContrato { get; set; }

    [DataMember]
    public string NombreContrato { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "RangoInicial", Description = "RangoInicial")]
    public long NuevoRangoInicial { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "RangoFinal", Description = "RangoFinal")]
    public long NuevoRangoFinal { get; set; }

    [DataMember]
    public long IdProvisionSuministro { get; set; }


    [DataMember]
    public bool ValidaPropietario { get; set; }
  }
}