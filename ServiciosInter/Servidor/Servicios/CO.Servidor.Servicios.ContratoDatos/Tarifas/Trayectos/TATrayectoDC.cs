using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Servicios.ContratoDatos.Tarifas.Trayectos
{
  /// <summary>
  /// Clase que contiene la información de trayectos
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class TATrayectoDC : DataContractBase
  {
    [DataMember]
    public long IdTrayecto { get; set; }

    [DataMember]
    public string IdLocalidadOrigen { get; set; }

    [DataMember]
    public string NombreLocalidadOrigen { get; set; }

    [DataMember]
    public string NombreLocalidadDestino { get; set; }

    [DataMember]
    public PALocalidadDC PaisOrigen { get; set; }

    [DataMember]
    public string IdLocalidadDestino { get; set; }

    [DataMember]
    public PALocalidadDC PaisDestino { get; set; }

    [DataMember]
    public TATipoTrayecto Trayecto { get; set; }

    [DataMember]
    public TATipoSubTrayecto SubTrayecto { get; set; }

    [DataMember]
    public int IdTrayectoSubTrayecto { get; set; }

    [DataMember]
    public List<TAServicioDC> Servicios { get; set; }

    [DataMember]
    public bool Replica { get; set; }

    [DataMember]
    public bool CiudadDestinoEditable { get; set; }

    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }
  }
}