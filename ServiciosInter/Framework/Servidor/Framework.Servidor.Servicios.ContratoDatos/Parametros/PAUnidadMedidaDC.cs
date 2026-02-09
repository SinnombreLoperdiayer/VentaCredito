using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Comun.DataAnnotations;

namespace Framework.Servidor.Servicios.ContratoDatos.Parametros
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class PAUnidadMedidaDC : DataContractBase
  {
    [DataMember]
    [Display(ResourceType = typeof(Etiquetas), Name = "UnidadMedida", Description = "UnidadMedida")]
    public string IdUnidadMedida { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Etiquetas), Name = "UnidadMedida", Description = "UnidadMedida")]
    [CamposOrdenamiento("UNM_Descripcion")]
    public string Descripcion { get; set; }
  }
}