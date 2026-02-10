using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Area;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Servicios.ContratoDatos.Suministros
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class SURemisionSuministroDC : DataContractBase
  {
    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NumeroRemision", Description = "NumeroRemision")]
    [Filtrable("RES_IdRemisionSuministros", "Id Remision: ", COEnumTipoControlFiltro.TextBox)]
    [CamposOrdenamiento("RES_IdRemisionSuministros")]
    public long IdRemision { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Fecha", Description = "Fecha")]
    [CamposOrdenamiento("RES_FechaGrabacion")]
    [Filtrable("RES_FechaGrabacion", "Fecha remisión: ", COEnumTipoControlFiltro.DatePicker)]
    public DateTime FechaRemision { get; set; }

    /// <summary>
    /// Ciudad destino de la remision
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Ciudad", Description = "Ciudad")]
    public PALocalidadDC CiudadDestino { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NumeroGuiaInternaDespacho", Description = "NumeroGuiaInternaDespacho")]
    [CamposOrdenamiento("RES_NumeroGuiaInternaDespacho")]
    [Filtrable("RES_NumeroGuiaInternaDespacho", "No. guía interna despacho: ", COEnumTipoControlFiltro.TextBox)]
    public long? NumeroGuiaDespacho { get; set; }

    [DataMember]
    [Filtrable("RES_NombreDestinatario", "Destinatario: ", COEnumTipoControlFiltro.TextBox)]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Destinatario", Description = "Destinatario")]
    public string Destinatario { get; set; }

    [DataMember]
    public SUGrupoSuministrosDC GrupoSuministros { get; set; }

    [DataMember]
    public int IdCasaMatriz { get; set; }

    private PURegionalAdministrativa racolAsignacionSuministro;

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "RegionalAdministrativa", Description = "RegionalAdministrativa")]
    public PURegionalAdministrativa RacolAsignacionSuministro
    {
      get { return racolAsignacionSuministro; }
      set
      {
        racolAsignacionSuministro = value;
        if (OnCambioRegional != null)
          OnCambioRegional(null, null);
      }
    }

    private PUCentroServiciosDC centroServicioAsignacion;

    [DataMember]
    public PUCentroServiciosDC CentroServicioAsignacion
    {
      get { return centroServicioAsignacion; }
      set
      {
        centroServicioAsignacion = value;
      }
    }

    [DataMember]
    public POMensajero MensajeroAsignacion { get; set; }

    [DataMember]
    public ARProcesoDC ProcesoAsignacion { get; set; }

    [DataMember]
    public PUCentroServiciosDC CentroServiciosGeneraRemision { get; set; }

    /// <summary>
    /// Informacion de la ciudad donde se crea la remision
    /// </summary>
    [DataMember]
    public PALocalidadDC CiudadRemisionSuministro { get; set; }

    [DataMember]
    public long? IdGuiaInternaRemision { get; set; }

    [DataMember]
    [CamposOrdenamiento("SUC_Nombre")]
    public CLSucursalDC Sucursal { get; set; }

    [DataMember]
    public CLClientesDC ClienteRemision { get; set; }

    [DataMember]
    public SUEnumGrupoSuministroDC GrupoSuministroDestino { get; set; }

    [DataMember]
    public DateTime RangoFechaInicial { get; set; }

    [DataMember]
    public DateTime RangoFechaFinal { get; set; }

    public event EventHandler OnCambioRegional;

    [DataMember]
    public ADGuiaInternaDC GuiaInterna { get; set; }

    [DataMember]
    public ARCasaMatrizDC CasaMatrizGeneraRemision { get; set; }

    [DataMember]
    public ARCasaMatrizDC CasaMatrizDestinoRemision { get; set; }

    [DataMember]
    public PAEnumEstados Estado { get; set; }

    [DataMember]
    public CLContratosDC Contrato { get; set; }

    [DataMember]
    [Display(Name = "Genera Guía Interna" , Description="Genera Guía Interna")]
    public bool GeneraGuiaInterna { get; set; }
  }
}