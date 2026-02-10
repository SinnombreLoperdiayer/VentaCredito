using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.LogisticaInversa
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class LIEstadoDatosGuiaDC : DataContractBase
  {
    [DataMember]
    public string IdEstadoDato { get; set; }

    [DataMember]
    public string Descripcion { get; set; }
  }
}