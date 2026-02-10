using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace Framework.Servidor.Servicios.ContratoDatos.Parametros
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class PAParienteDC
  {
    [DataMember]
    [Display(ResourceType = typeof(Etiquetas), Name = "Parentesco", Description = "ToolTipParentesco")]
    public short IdPariente { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Etiquetas), Name = "Parentesco", Description = "ToolTipParentesco")]
    public string NombrePariente { get; set; }
  }
}