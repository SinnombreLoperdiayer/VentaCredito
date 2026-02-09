using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Servicios.ContratoDatos.Tarifas.Trayectos
{
  /// <summary>
  /// Clase que contiene la informacion de las excepciones de TrayectoSubTrayecto
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class TATrayectoExcepcionDC : DataContractBase
  {
    [DataMember]
    public long IdTrayectoSubTrayectoExcepcion { get; set; }

    [DataMember]
    public string IdLocalidadOrigen { get; set; }

    [DataMember]
    public string IdLocalidadDestino { get; set; }

    [DataMember]
    public string NombreLocalidadOrigen { get; set; }

    [DataMember]
    public string NombreLocalidadDestino { get; set; }

    [DataMember]
    public PALocalidadDC Pais { get; set; }

    [DataMember]
    public int IdListaPrecio { get; set; }

    [DataMember]
    public int IdTrayectosubTrayecto { get; set; }

    [DataMember]
    public TATipoTrayecto Trayecto { get; set; }

    [DataMember]
    public bool Editable { get; set; }

    [DataMember]
    public TATipoSubTrayecto Subtrayecto { get; set; }

    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }

    [DataMember]
    public IEnumerable<TATrayectoExcepcionServicioDC> Servicios { get; set; }
  }
}