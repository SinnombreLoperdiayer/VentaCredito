using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Solicitudes
{
  /// <summary>
  /// Clase que contiene la informacion de la tbl de Tipos de Solicitud
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class GITipoSolicitudDC : DataContractBase
  {
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoSolicitud", Description = "TooltipTipoSolicitud")]
    public short? Id { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "DescripTipoSolicitud", Description = "ToolTipDescripTipoSolicitud")]
    public string Descripcion { get; set; }

    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }

    [DataMember]
    public List<GIMotivoSolicitudDC> MotivosSolicitudes { get; set; }
  }
}