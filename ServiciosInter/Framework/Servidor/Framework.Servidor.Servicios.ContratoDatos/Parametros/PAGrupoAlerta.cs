using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Servidor.Servicios.ContratoDatos.Parametros
{
  /// <summary>
  /// Clase que contiene la informacion de los grupos de destinatarios utilizado para las alertas
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class PAGrupoAlerta : DataContractBase
  {
    [DataMember]
    public string IdGrupo { get; set; }

    [DataMember]
    public string Descripcion { get; set; }

    [DataMember]
    public bool Seleccionado { get; set; }

    [DataMember]
    public string CorrerosDestinatarios { get; set; }
  }
}