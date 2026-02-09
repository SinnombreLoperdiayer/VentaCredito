using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Tarifas
{
  /// <summary>
  /// Clase que contiene la información del estado activo e inactivo
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class TAEstadoActivoInactivo : DataContractBase
  {
    [DataMember]
    public string IdEstado { get; set; }

    [DataMember]
    public string EstadoUsuario { get; set; }
  }
}