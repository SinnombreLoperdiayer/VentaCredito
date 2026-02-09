using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;

namespace CO.Servidor.Servicios.ContratoDatos.Suministros
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class SUNumeradorPrefijo
  {
    [DataMember]
    public long ValorActual { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Prefijo", Description = "Prefijo")]
    public string Prefijo { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "IdNumerador")]
    [CamposOrdenamiento("NUM_IdNumerador")]
    public string IdNumerador { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Descripcion", Description = "Descripcion")]
    [CamposOrdenamiento("NUM_Descripcion")]
    public string Descripcion { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Resolucion", Description = "Resolucion")]
    [Filtrable("NUM_Resolucion", "Número resolución: ", COEnumTipoControlFiltro.TextBox)]
    [CamposOrdenamiento("NUM_Resolucion")]
    public string Resolucion { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "RangoInicial", Description = "RangoInicial")]
    [CamposOrdenamiento("NUM_Inicio")]
    public long RangoInicial { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "RangoFinal", Description = "RangoFinal")]
    [CamposOrdenamiento("NUM_Fin")]
    public long RangoFinal { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FechaInicio", Description = "FechaInicio")]
    [CamposOrdenamiento("NUM_FechaInicial")]
    public DateTime FechaInicial { get; set; }

    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FechaFin", Description = "FechaFin")]
    [CamposOrdenamiento("NUM_FechaFinal")]
    public DateTime FechaFinal { get; set; }

    [DataMember]
    public bool EstaActivo { get; set; }

    [DataMember]
    public SUSuministro Suministro { get; set; }

    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "RangoActual", Description = "ToolTipRangoActual")]
    public long RangoActual { get; set; }
  }
}