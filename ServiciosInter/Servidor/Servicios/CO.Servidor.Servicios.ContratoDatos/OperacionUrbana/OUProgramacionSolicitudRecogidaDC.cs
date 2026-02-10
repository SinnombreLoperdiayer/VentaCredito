using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Servicios.ContratoDatos.OperacionUrbana
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class OUProgramacionSolicitudRecogidaDC : DataContractBase
  {
    //[DataMember]
    //public OUMensajeroDC Mensajero { get; set; }
    
    [DataMember]
    public long IdProgramacionSolicitudRecogida { get; set; }
    
    [DataMember]
    public bool EstaPlanillada { get; set; }
    [DataMember]
    public bool EstaDescargada {get;set;}
      
    [DataMember]
    public DateTime FechaDescarga { get; set; }

    [DataMember]
    public OURecogidasDC Recogida { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Reportada", Description = "Reportada")]
    public bool? ReportadoMensajero { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FechaReporte", Description = "FechaReporte")]
    public DateTime? FechaReporteMensajero { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "MotivoReprogramacion")]
    public OUMotivosReprogramacionDC MotivoReprogramacion { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FechaProgramacion", Description = "FechaProgramacion")]
    public DateTime FechaProgramacion { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Estado", Description = "Estado")]
    public string Estado { get; set; }

    [DataMember]
    public PAZonaDC Zona { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Planilla", Description = "Planilla")]
    public long? IdPlanillaRecogida { get; set; }

    [DataMember]
    public string CreadoPor { get; set; }

    [DataMember]
    public DateTime FechaCreacion { get; set; }

    [DataMember]
    public OUNombresMensajeroDC MensajeroPlanilla { get; set; }

    [DataMember]
    public OUTipoMensajeroDC TipoMensajero { get; set; }

    [DataMember]
    public List<OURecogidasDC> RecogidasPlanilla { get; set; }

    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }

  
  }
}