using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace Framework.Servidor.Servicios.ContratoDatos.Parametros
{
  /// <summary>
  /// Clase que contiene la informacion de las ocupaciones
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class PAOcupacionDC : DataContractBase
  {
    [DataMember]
    [Display(ResourceType = typeof(Etiquetas), Name = "Ocupacion", Description = "TooltipOcupacion")]
    public short IdOcupacion { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Etiquetas), Name = "Ocupacion", Description = "TooltipOcupacion")]
    public string DescripcionOcupacion { get; set; }
  }
}