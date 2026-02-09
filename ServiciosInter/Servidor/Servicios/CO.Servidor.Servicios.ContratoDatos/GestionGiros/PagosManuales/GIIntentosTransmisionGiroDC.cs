using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Cliente.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using Framework.Servidor.Comun.DataAnnotations;

namespace CO.Servidor.Servicios.ContratoDatos.GestionGiros.PagosManuales
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class GIIntentosTransmisionGiroDC
  {
    [DataMember]
    public long IdAdminGiro { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NumeroIntentoTransmision", Description = "ToolTipNumIntentoTransmision")]
    public long IdIntentoTransGiro { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Observaciones", Description = "TooltipObservaciones")]
    public string Observaciones { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FechaTrasmision", Description = "FechaTrasmision")]
    public DateTime FechaGrabacion { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "UsuarioTrasmite", Description = "UsuarioTrasmite")]
    public string CreadoPor { get; set; }

    /// <summary>
    /// Resultado del intento de transmisión del giro TRA(Transmitido)-FAL(Fallido)
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ResultadoTransmision", Description = "ToolTipResultadoTransmision")]
    public GIEnumTipoTransmisionDC TipoTransmision { get; set; }

    /// <summary>
    /// NOmbre de quien recibe la transmisión en la agencia destino
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Nombre", Description = "ToolTipNombreRecibeTransmision")]
    public string NombreContacto { get; set; }

    /// <summary>
    /// Identificacion de quien recibe la transmision
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Identificacion", Description = "ToolTipIdentiRecibeTransmi")]
    public string IdentificacionContacto { get; set; }

    /// <summary>
    /// Identificacion de quien recibe la transmision
    /// </summary>
    [DataMember]
    [CamposOrdenamiento("ITG_NoPlanillaTransmision")]
    [Filtrable("ITG_NoPlanillaTransmision", "Id Planilla: ", COEnumTipoControlFiltro.TextBox)]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Planilla", Description = "Planilla")]
    public long? IdPlanilla { get; set; }

    /// <summary>
    /// Fecha de la planilla de trasmision
    /// </summary>
    [DataMember]
    [Filtrable("FechaPlanilla", "Fecha Planilla: ", COEnumTipoControlFiltro.DatePicker)]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Planilla", Description = "Planilla")]
    [CamposOrdenamiento("FechaPlanilla")]
    public DateTime FechaPlanilla { get; set; }

    [DataMember]
    public PUCentroServiciosDC AgenciaDestino { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoPlanilla", Description = "TipoPlanilla")]
    public GIEnumTipoPlanillaTrasmisionDC TipoPlanilla { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoPlanilla", Description = "TipoPlanilla")]
    public string DescripcionTipoPlanilla { get; set; }

    [DataMember]
    public List<GIAdmisionGirosDC> GirosTransmision { get; set; }
  }
}