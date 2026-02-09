using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Comun;

namespace Framework.Servidor.Servicios.ContratoDatos.Seguridad
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class SEMenuAccion : DataContractBase
  {
    [DataMember]
    public int IdMenuRol { get; set; }

    [DataMember]
    public int IdMenu { get; set; }

    [DataMember]
    public string Etiqueta { get; set; }

    //Recibir y enviar Acciones de un Menu Rol
    [DataMember]
    public List<SEAccion> Acciones { get; set; }

    /// <summary>
    /// Enumeracion que indica el estado del objeto dentro de una lista
    /// </summary>
    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }

  }
}