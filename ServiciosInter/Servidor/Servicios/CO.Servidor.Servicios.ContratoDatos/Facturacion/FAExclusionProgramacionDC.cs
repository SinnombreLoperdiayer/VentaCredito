using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Facturacion
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class FAExclusionProgramacionDC
  {
    [DataMember]
    public int IdServicio { get; set; }

    [DataMember]
    [Display(Name = "Concepto")]
    public string NombreServicio { get; set; }

    [DataMember]
    [Display(Name = "Movimiento Desde")]
    public long NumeroOperacion { get; set; }

    [DataMember]
    [Display(Name = "Movimiento Hasta")]
    public long NumeroOperacionHasta { get; set; }

    [DataMember]
    public DateTime FechaOperacion { get; set; }

    [DataMember]
    public long IdProgramacion { get; set; }

    [DataMember]
    [Display(Name = "Valor")]
    public decimal Valor { get; set; }

    [DataMember]
    [Display(Name = "Fecha Exclusión")]
    public System.DateTime FechaGrabacion { get; set; }

    [DataMember]
    [Display(Name = "Usuario")]
    public string CreadoPor { get; set; }
  }
}