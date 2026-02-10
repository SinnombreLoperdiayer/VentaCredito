using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Cliente.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.OperacionNacional
{
  /// <summary>
  /// Contiene la informacion del ingreso y salida del transportador
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class ONIngresoSalidaTransportadorDC : DataContractBase
  {
    [DataMember]
    public long IdIngrsoSalidaTransportado { get; set; }

    [DataMember]
    public string IdLocalidadIngresoSalida { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Ciudad", Description = "ToolTipCiudad")]
    public string LocalidadDescripcion { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Novedad", Description = "Novedad")]
    public short IdNovedadOperativo { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Novedad", Description = "Novedad")]
    public string NovedadDescripcion { get; set; }

    [DataMember]
    public ONNovedadOperativoDC Novedad { get; set; }

    [DataMember]
    public long IdAgenciaIngresoSalida { get; set; }

    [DataMember]
    public int IdVehiculo { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Placa", Description = "ToolTipPlaca")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public string Placa { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Identificacion", Description = "TooltipIdentificacion")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public string IdentificacionConductor { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NombreConductor", Description = "TooltipNombreConductor")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public string NombreConductor { get; set; }

    [DataMember]
    public long IdConductor { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "EntradaSalida", Description = "ToolTipEntradaSalidaTransportador")]
    public bool EsIngreso { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Ruta", Description = "ToolTipIdRuta")]
    public int IdRuta { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Ruta", Description = "ToolTipIdRuta")]
    public string RutaDescripcion { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [RegularExpression(@"[0-9]*", ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NumeroPrecintoVehiculo", Description = "TooltipNumPrecintoVehiculo")]
    public long? NumeroPrecinto { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FechaIngreso", Description = "ToolTipFechaIngreso")]
    public DateTime FechaIngresoSalida { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Observaciones", Description = "TooltipObservaciones")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public string Observacion { get; set; }

    [DataMember]
    public int FechaInicial { get; set; }

    [DataMember]
    public int FechaFinal { get; set; }
  }
}