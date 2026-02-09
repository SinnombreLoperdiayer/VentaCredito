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
  public class OUMotivoDescargueRecogidasDC : DataContractBase
  {
    /// <summary>
    /// id del motivo de descargue de la recogida
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "MotivoDescargue", Description = "MotivoDescargue")]
    public int IdMotivo { get; set; }

    /// <summary>
    /// descripcion del motivo de descargue de la recogida
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "MotivoDescargue", Description = "MotivoDescargue")]
    public string DescripcionMotivo { get; set; }

    /// <summary>
    /// Indica si el motivo permite realizar reprogramacion o no
    /// </summary>
    [DataMember]
    public bool PermiteReprogramar { get; set; }

    [DataMember]
    public bool VisibleMensajero { get; set; }
  }
}