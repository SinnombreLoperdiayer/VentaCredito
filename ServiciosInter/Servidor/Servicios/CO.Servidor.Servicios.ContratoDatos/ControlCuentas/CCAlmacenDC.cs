using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros.Consecutivos;

namespace CO.Servidor.Servicios.ContratoDatos.ControlCuentas
{
  /// <summary>
  /// Clase que contiene la información de almacen de control de cuentas
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class CCAlmacenDC
  {
      [DataMember]      
      public long IdRacol { get; set; }

    [DataMember]
    public string IdAlmacenControlCuentas { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NumeroOperacion", Description = "ToolTipNumeroOperacion")]
    public long IdOperacion { get; set; }

    [DataMember]
    public short IdTipoOperacion { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoOperacion", Description = "TooltipTipoOperacion")]
    public string DescripcionTipoOperacion { get; set; }

    [DataMember]
    public long Caja { get; set; }

    [DataMember]
    public long CajaActual { get; set; }

    [DataMember]
    public int Lote { get; set; }

    [DataMember]
    public int Posicion { get; set; }

    [DataMember]
    public bool CajaLlena { get; set; }

    [DataMember]
    public DateTime Fecha { get; set; }

    [DataMember]
    public PAEnumConsecutivos TipoConsecutivo { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Usuario", Description = "TooltipUsuarioLiquidacion")]
    public string Usuario { get; set; }
  }
}