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

namespace CO.Servidor.Servicios.ContratoDatos.Rutas
{
  /// <summary>
  /// Clase que contiene la informacion de las estaciones de la ruta
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class RUEstacionRuta : DataContractBase
  {
    public event EventHandler OnEstacionRutaCambio;
    public event EventHandler OnCiudadEstacionCambio;

    private int idEstacionRuta;

    [DataMember]
    public int IdEstacionRuta
    {
      get { return idEstacionRuta; }
      set
      {
        idEstacionRuta = value;
        if (OnEstacionRutaCambio != null)
          OnEstacionRutaCambio(null, null);
      }
    }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "IdRuta", Description = "ToolTipIdRuta")]
    public int IdRuta { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "CiudadManifestar", Description = "TooltipCiudadManifestar")]
    public string IdLocalidadEstacion { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "CiudadEstacion", Description = "ToolTipCiudadEstacion")]
    [CamposOrdenamiento("ESR_NombreLocalidadEstacion")]
    public string NombreLocalidadEstacion { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "OrdenEnRuta", Description = "ToolTipOrdenEnRuta")]
    public int OrdenEnRuta { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "OrdenEnCasillero", Description = "ToolTipOrdenEnCasillero")]
    public int OrdenEnCasillero { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TiempoParada", Description = "ToolTipTiempoParada")]
    [Range(1, int.MaxValue, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public decimal TiempoParada { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TiempoViajeDesdeAnterior", Description = "ToolTipTiempoViajeDesdeAnterior")]
    [Range(1, int.MaxValue, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public decimal TiempoViajeDesdeAnterior { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "PermiteEnganche", Description = "ToolTipPermiteEnganche")]
    public bool PermiteEnganche { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "RequierePrecintoRetorno", Description = "ToolTipRequierePrecintoRetorno")]
    public bool RequierePrecintoRetorno { get; set; }

    [IgnoreDataMember]
    public PALocalidadDC CiudadHijaEstacion { get; set; }

    private PALocalidadDC ciudadEstacion;

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "CiudadEstacion", Description = "ToolTipCiudadEstacion")]
    public PALocalidadDC CiudadEstacion
    {
      get { return ciudadEstacion; }
      set
      {
        ciudadEstacion = value;
        if (OnCiudadEstacionCambio != null)
          OnCiudadEstacionCambio(null, null);
      }
    }

    [IgnoreDataMember]
    public PALocalidadDC PaisCiudadHijaEstacion { get; set; }

    [DataMember]
    public List<RUCoberturaEstacion> CiudadesHijas { get; set; }

    [DataMember]
    public List<RUFrecuenciaParadaEstacion> FrecuenciaLlegada { get; set; }

    [DataMember]
    public bool GeneraConsolidado { get; set; }

    [IgnoreDataMember]
    public bool CambiarOrden { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Seleccionar", Description = "ToolTipSeleccionar")]
    public bool Seleccionada { get; set; }

    /// <summary>
    /// Enumeración que indica el estado del objeto dentro de una lista
    /// </summary>
    [DataMember]
    public EnumEstadoRegistro EstadoRegistro
    {
      get { return estadoRegistro; }
      set { estadoRegistro = value; }
    }
    [DataMember]
    public string TipoAgencia { get; set; }
    [DataMember]
    public string SubTipoAgencia { get; set; }

    private EnumEstadoRegistro estadoRegistro;


    [DataMember]
    public int CantConsolidados { get; set; }

    [DataMember]
    public int CantSueltos { get; set; }

    [DataMember]
    public string NomTabla { get; set; }

    [DataMember]
    public string StrNovedades { get; set; }

  }
}