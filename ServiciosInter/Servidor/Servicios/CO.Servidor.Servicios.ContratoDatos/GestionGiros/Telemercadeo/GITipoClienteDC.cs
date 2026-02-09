using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.GestionGiros.Telemercadeo
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class GITipoClienteDC : DataContractBase
  {
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoCliente", Description = "TipoCliente")]
    public string IdTipoCliente { get; set; }

    [DataMember]
    public string Descripcion { get; set; }
  }
}