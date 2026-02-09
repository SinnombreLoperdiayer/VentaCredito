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
  public class LIResultadoTelemercadeoDC : DataContractBase
  {
    [DataMember]
    [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "ResultadoGestion", Description = "ToolTipResultadoGestion")]
    public short IdResultado { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "ResultadoGestion", Description = "ToolTipResultadoGestion")]
    public string Descripcion { get; set; }

    [DataMember]
    public string Estado { get; set; }

    [DataMember]
    public string nombreAssembly { get; set; }

    [DataMember]
    public string @namespace { get; set; }

    [DataMember]
    public string nombreClase { get; set; }

    [DataMember]
    public string Ciudad { get; set; }
  }
}