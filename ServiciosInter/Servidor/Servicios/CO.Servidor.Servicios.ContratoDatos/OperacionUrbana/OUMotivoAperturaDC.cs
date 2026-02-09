using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.OperacionUrbana
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class OUMotivoAperturaDC
  {
    /// <summary>
    /// id del motivo de descargue de la recogida
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "MotivoApertura", Description = "MotivoApertura")]
    public int IdMotivo { get; set; }

    /// <summary>
    /// descripcion del motivo de descargue de la recogida
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "MotivoApertura", Description = "MotivoApertura")]
    public string DescripcionMotivo { get; set; }
  }
}