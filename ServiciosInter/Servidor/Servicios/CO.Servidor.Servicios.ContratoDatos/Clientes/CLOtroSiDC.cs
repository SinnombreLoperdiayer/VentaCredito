using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Clientes
{
  /// <summary>
  /// Clase con el DataContract de otrosi de contrato
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class CLOtroSiDC : DataContractBase
  {

    public event EventHandler  OnIdModalidadOtroSiCambio;
    [DataMember]
    [CamposOrdenamiento("OSC_IdOtroSi")]
    [Filtrable("OSC_IdOtroSi", "Código:", COEnumTipoControlFiltro.TextBox)]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Codigo", Description = "TooltipCodigoOtrosi")]
    public int IdOtroSi { get; set; }

   
    private short idModalidadOtroSi;
     [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Modalidad", Description = "TooltipModalidad")]
public short IdModalidadOtroSi
{
  get { return idModalidadOtroSi; }
  set { 
    idModalidadOtroSi = value; 
    if(OnIdModalidadOtroSiCambio !=null)
      OnIdModalidadOtroSiCambio(idModalidadOtroSi, null);
  }
}

    [DataMember]
    [CamposOrdenamiento("OSC_NumeroOtroSi")]
    [Filtrable("OSC_NumeroOtroSi", "Número interno:", COEnumTipoControlFiltro.TextBox)]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NumeroInterno", Description = "TooltipNumeroInterno")]
    [StringLength(15, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
    public string NumeroOtroSi { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public int IdContrato { get; set; }

    [DataMember]
    [CamposOrdenamiento("OSC_ValorOtroSi")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Valor", Description = "TooltipValorContrato")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public decimal Valor { get; set; }

    [DataMember]
    [CamposOrdenamiento("OSC_FechaFin")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FechaFin", Description = "TooltipFechaFin")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public DateTime FechaFinal { get; set; }

    [DataMember]
    [Filtrable("OSC_Descripcion", "Observación:", COEnumTipoControlFiltro.TextBox)]
    [CamposOrdenamiento("OSC_Descripcion")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Descripcion", Description = "ToolTipDocuCentrosServicio")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [StringLength(250, MinimumLength = 5, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
    public string Descripcion { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public IEnumerable<CLTipoOtroSiDC> ListaTipos { get; set; }

    [DataMember]
    [CamposOrdenamiento("OSC_IdModalidadOtroSi")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Modalidad", Description = "TooltipModalidad")]
    public string DescripcionModalidad { get; set; }

    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }


    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NumeroRegistroDisponibilidad", Description = "ToolTipNumeroRegistroDisponibilidad")]
    [StringLength(100, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
    public string NumeroRegistroDisponibilidad { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ValorDisponibilidad", Description = "ToolTipValorDisponibilidad")]
    public decimal? ValorDisponibilidad { get; set; }

    [DataMember]
    public List<CLContratosArchivosDC> ArchivosOtroSi { get; set; }
  }
}