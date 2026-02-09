using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Produccion
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class PRRetencionMotivoNovedadDCDeprecated : DataContractBase
  {
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "IdProgramacion")]
    public int IdRetencion { get; set; }

    [DataMember]
    public decimal ValorFijo { get; set; }

    [DataMember]
    public decimal TarifaPorcentual { get; set; }

    [DataMember]
    public decimal ValorBase { get; set; }
  }
}