using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.OperacionNacional
{
  /// <summary>
  /// Clase que contiene la informacion de los tipos de motivo de eliminacion de la guia
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class ONTipoMotivoElimGuiaMani : DataContractBase
  {
    [DataMember]
    public int IdTipoMotivo { get; set; }

    [DataMember]
    public string Descripcion { get; set; }
  }
}