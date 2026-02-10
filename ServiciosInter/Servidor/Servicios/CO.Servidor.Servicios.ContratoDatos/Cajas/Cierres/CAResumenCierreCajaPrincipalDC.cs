using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Cajas.Cierres
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class CAResumenCierreCajaPrincipalDC
  {
    [DataMember]
    public long IdApertura { get; set; }

    [DataMember]
    public long IdCierreAsociado { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Id")]
    public int IdConceptoCaja { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Concepto")]
    public string NombreConceptoCaja { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Cantidad")]
    public int Cantidad { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Ingreso", Description = "ToolTipIngreso")]
    public decimal ValorIngreso { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Egreso", Description = "ToolTipEgreso")]
    public decimal ValorEgreso { get; set; }
  }
}