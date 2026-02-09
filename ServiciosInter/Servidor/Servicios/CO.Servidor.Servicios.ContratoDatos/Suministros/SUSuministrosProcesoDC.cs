using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;

namespace CO.Servidor.Servicios.ContratoDatos.Suministros
{
  /// <summary>
  /// Contiene la informacion de los suministros de un proceso
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class SUSuministrosProcesoDC
  {
    [DataMember]
    public int IdSuministroProceso { get; set; }

    [DataMember]
    public long CodProceso { get; set; }
    
    [DataMember]
    public string DescripcionProceso { get; set; }

    [DataMember]
    public int IdSuministro { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "CantidadInicialAutorizada", Description = "ToolTipCantidadAsignadaSuministro")]
    public decimal CantidadInicialAutorizada { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "StockMinimoSuministro", Description = "ToolTipStockMinimoSuministro")]
    public decimal StockMinimo { get; set; }
    [DataMember]
    public SUSuministro Suministro { get; set; }
    
    [DataMember]
    public string Estado { get; set; }

    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }
  }
}
