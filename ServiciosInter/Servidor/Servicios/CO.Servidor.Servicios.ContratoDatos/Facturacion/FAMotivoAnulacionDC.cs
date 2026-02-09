using CO.Cliente.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Facturacion
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class FAMotivoAnulacionDC
  {
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Motivo", Description = "Motivo")]
    public short IdMotivo { get; set; }

    [DataMember]
    public string DescripcionMotivo { get; set; }
  }
}