using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Produccion
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class PRMotivoNovedadDCDeprecated
  {
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Novedad", Description = "Novedad")]
    public int IdMotivoNovedad { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Novedad")]
    public string DescripcionMotivoNovedad { get; set; }

    [DataMember]
    public short IdRetencion { get; set; }

    [DataMember]
    public List<PRRetencionMotivoNovedadDCDeprecated> Retenciones { get; set; }
  }
}