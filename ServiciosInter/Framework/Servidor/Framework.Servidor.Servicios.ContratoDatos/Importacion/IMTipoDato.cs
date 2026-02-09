using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Servidor.Servicios.ContratoDatos.Importacion
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class IMTipoDato
  {
    [DataMember]
    public string Nombre { get; set; }

    [DataMember]
    public string Valor { get; set; }
  }
}