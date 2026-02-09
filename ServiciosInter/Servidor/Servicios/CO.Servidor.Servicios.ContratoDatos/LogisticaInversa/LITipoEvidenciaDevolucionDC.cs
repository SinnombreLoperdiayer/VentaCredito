using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.LogisticaInversa
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class LITipoEvidenciaDevolucionDC : DataContractBase
  {
    [DataMember]
    [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "Tipo", Description = "ToolTipTipoEvidencia")]
    public short IdTipo { get; set; }

    [DataMember]
    public int IdSuministro { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "Tipo", Description = "ToolTipTipoEvidencia")]
    public string Descripcion { get; set; }
  }
}