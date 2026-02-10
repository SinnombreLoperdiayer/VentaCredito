using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Servicios.ContratoDatos.Rutas
{
  /// <summary>
  /// Clase que contiene la informacion de las rutas
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class RURutaDC : DataContractBase
  {
      private int idRuta;

      [DataMember]
      [Filtrable("RUT_IdRuta", typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), "IdRuta", COEnumTipoControlFiltro.TextBox)]
      [CamposOrdenamiento("RUT_IdRuta")]
      [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "IdRuta", Description = "ToolTipIdRuta")]
      public int IdRuta
      {
          get { return idRuta; }
          set { idRuta = value; OnPropertyChanged("IdRuta"); }
      }

    [DataMember]
    [Filtrable("RUT_Nombre", typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), "Ruta", COEnumTipoControlFiltro.TextBox)]
    [CamposOrdenamiento("RUT_Nombre")]
    [StringLength(50, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NombreRuta", Description = "ToolTipRuta")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public string NombreRuta { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoRuta", Description = "ToolTipTipoRuta")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Range(1, int.MaxValue, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public int IdTipoRuta { get; set; }

    [DataMember]
    [CamposOrdenamiento("TRU_Descripcion")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoRuta", Description = "ToolTipTipoRuta")]
    public string NombreTipoRuta { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "LocalidadOrigen", Description = "ToolTipLocalidadOrigen")]
    public string IdLocalidadOrigen { get; set; }

    [DataMember]
    [Filtrable("RUT_NombreLocalidadOrigen", typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), "LocalidadOrigen", COEnumTipoControlFiltro.TextBox)]
    [CamposOrdenamiento("RUT_NombreLocalidadOrigen")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "LocalidadOrigen", Description = "ToolTipLocalidadOrigen")]
    public string NombreLocalidadOrigen { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "LocalidadDestino", Description = "ToolTipLocalidadDestino")]
    public string IdLocalidadDestino { get; set; }

    [DataMember]
    [Filtrable("RUT_NombreLocalidadDestino", typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), "LocalidadDestino", COEnumTipoControlFiltro.TextBox)]
    [CamposOrdenamiento("RUT_NombreLocalidadDestino")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "LocalidadDestino", Description = "ToolTipLocalidadDestino")]
    public string NombreLocalidadDestino { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "MedioTransporte", Description = "ToolTipMedioTransporte")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Range(1, int.MaxValue, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public int IdMediotransporte { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "MedioTransporte", Description = "ToolTipMedioTransporte")]
    public string NombreMedioTransporte { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "GeneraManifiesto", Description = "ToolTipGeneraManifiesto")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public bool GeneraManifiesto { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "CostoMensualRuta", Description = "ToolTipCostoMensualRuta")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public decimal CostoMensualRuta { get; set; }

    [DataMember]
    [CamposOrdenamiento("TIT_Descripcion")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoTransporte", Description = "ToolTipTipoTransporte")]
    [Range(1, int.MaxValue, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public int IdTipoTransporte { get; set; }

    [DataMember]
    [CamposOrdenamiento("TIT_Descripcion")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoTransporte", Description = "ToolTipTipoTransporte")]
    public string NombreTipoTransporte { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "LocalidadDestino", Description = "ToolTipLocalidadDestino")]    
    public PALocalidadDC CiudadDestino { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "LocalidadOrigen", Description = "ToolTipLocalidadOrigen")]    
    public PALocalidadDC CiudadOrigen { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoVehiculo", Description = "ToolTipTipoVehiculo")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Range(1, int.MaxValue, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public int IdTipoVehiculo { get; set; }

    [DataMember]
    [Display(Name = "")]
    public PALocalidadDC PaisCiudad { get; set; }

    [DataMember]
    [Display(Name = "")]
    public PALocalidadDC PaisCiudadDestino { get; set; }

    [DataMember]
    public List<RUFrecuenciaRuta> FrecuenciaRuta { get; set; }

    [DataMember]
    public List<RUEstacionRuta> EstacionesHijas { get; set; }

    [DataMember]
    public List<RUEmpresaTransportadora> EmpresasTransportadoras { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Estado", Description = "ToolTipEstadoRuta")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public bool Estado { get; set; }
       [DataMember]
    public bool EsRutaMasivos { get; set; }

    /// <summary>
    /// Enumeración que indica el estado del objeto dentro de una lista
    /// </summary>
    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }
  }
}