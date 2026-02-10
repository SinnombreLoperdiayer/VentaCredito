using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Produccion
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class PANovedadCentroServicioDCDeprecated : DataContractBase
  {
    [DataMember]
    public long IdCentroServicios { get; set; }

    [DataMember]
    public string NombreCentroServicios { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Valor", Description = "Valor")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [CamposOrdenamiento("NCS_Valor")]
    public decimal Valor { get; set; }

    [IgnoreDataMember]
    public long IdProduccion { get; set; }

    [IgnoreDataMember]
    public DateTime FechaAplicacionPr { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Observaciones", Description = "TooltipObservaciones")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public string Observaciones { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public PRTipoNovedadDCDeprecated TipoNovedad { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public PRMotivoNovedadDCDeprecated MotivoNovedad { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "IdNovedad", Description = "IdNovedad")]
    [CamposOrdenamiento("NCS_IdNovedadCentroServicios")]
    public long IdNovedad { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Usuario", Description = "Usuario")]
    [CamposOrdenamiento("NCS_CreadoPor")]
    public string Usuario { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Fecha")]
    [Filtrable("NCS_FechaGrabacion", "Fecha:", COEnumTipoControlFiltro.DatePicker)]
    [CamposOrdenamiento("NCS_FechaGrabacion")]
    public DateTime FechaNovedad { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "AplicadaPR")]
    [CamposOrdenamiento("NCS_EstaAprobada")]
    public bool AplicadaEnPR { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FechaAplicacionPR")]
    [CamposOrdenamiento("NCS_FechaAplicacionPr")]
    public DateTime FechaAplicacionPR { get; set; }

    [DataMember]
    public PUCentroServiciosDC CentroServicios { get; set; }

    [DataMember]
    public bool EstaAprobada { get; set; }

    [DataMember]
    public DateTime FechaGrabacion { get; set; }
  }
}