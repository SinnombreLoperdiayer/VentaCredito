using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using CO.Cliente.Servicios.ContratoDatos;


namespace CO.Servidor.Servicios.ContratoDatos.OperacionUrbana
{
  [DataContract(Namespace = "http://interrapidisimo.com")]
  public class OUTipoConsolidadoDC
  {
    [DataMember]
    public short IdTipoConsolidado { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Tipo")]
    public string Descripcion { get; set; }

    [DataMember]
    public List<OUTipoConsolidadoDetalleDC> Detalles { get; set; }
  }
}
