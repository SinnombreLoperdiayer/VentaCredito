using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Produccion
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class PRProgramacionLiquidacionDCDeprecated : DataContractBase
  {
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "IdProgramacion")]
    [CamposOrdenamiento("PRL_IdProgramacion")]
    [Filtrable("PRL_IdProgramacion", "Id progamación:", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 250)]
    public long IdProgramacion { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "CantidadAgencias")]
    public int CantidadAgencias { get; set; }

    [DataMember]
    [Filtrable("PRI_FechaProgramacion", "Fecha programación:", COEnumTipoControlFiltro.DatePicker)]
    [CamposOrdenamiento("PRI_FechaProgramacion")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FechaProgramacion")]
    public DateTime FechaProgramacion { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FechaCorte")]
    public DateTime FechaCorte { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FechaGrabacion")]
    public DateTime FechaGrabacion { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FechaEjecucion")]
    public DateTime FechaEjecucion { get; set; }

    [DataMember]
    [Filtrable("PRI_CreadoPor", "Usuario:", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 250)]
    [CamposOrdenamiento("PRI_CreadoPor")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Usuario")]
    public string Usuario { get; set; }

    [DataMember]
    public int IdEstado { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Estado")]
    public string DescripcionEstado { get; set; }

    [DataMember]
    public List<PUAgenciaDeRacolDC> AgenciasIncluidas { get; set; }

    [DataMember]
    public List<PUAgenciaDeRacolDC> AgenciasNoIncluidas { get; set; }

    [DataMember]
    public long IdRacolLiquidacion { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "PuntoCentroServicio")]
    public long IdCentroServicios { get; set; }

    [DataMember]
    public string NombreCentroServicios { get; set; }

    [DataMember]
    public bool EstaEjecutada { get; set; }

    [DataMember]
    public DateTime HoraProgramada { get; set; }

    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }
  }
}