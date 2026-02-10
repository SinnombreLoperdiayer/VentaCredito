using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Produccion
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class PRTipoNovedadDCDeprecated : DataContractBase
  {
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoNovedad", Description = "TipoNovedad")]
    public int IdTipoNovedad { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoNovedad")]
    public string DescripcionTipoNovedad { get; set; }
  }
}