using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Comisiones
{
  /// <summary>
  /// Clase que contiene la informacion de los centros de servicio administrados
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class CMCentroServicioAdministrado : DataContractBase
  {
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "IdCentroServicio", Description = "TooltipIdCentroServicio")]
    [Filtrable("CES_IdCentroServicios", "Código Centro Servicio:", COEnumTipoControlFiltro.TextBox, FormatoRegex = ("[0-9]"), MaximaLongitud = 100)]
    public long IdCentroServicios { get; set; }

    [DataMember]

    public long IdCentroServiciosAdministrador { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Nombre", Description = "ToolTipNombreCentroServicio")]
    [Filtrable("CES_Nombre", "Nombre Centro Servicio:", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 250)]
    public string NombreCentroServicios { get; set; }

    [DataMember]
    public string IdTipoAgencia { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Municipio", Description = "TooltipMunicipio")]
    [Filtrable("LOC_Nombre", "Ciudad:", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 100)]
    public string Municipio { get; set; }

    [DataMember]
    public string Estado { get; set; }

    [DataMember]
    public string DescripcionRacol { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Direccion", Description = "TooltipDireccion")]
    [Filtrable("CES_Direccion", "Dirección:", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 250)]
    public string Direccion { get; set; }

    [DataMember]
    public string TipoCentroServicio { get; set; }
  }
}