using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Suministros
{
  /// <summary>
  /// Realizar la configuración de las cuentas de correo de los destinatarios
  /// de la alerta del fallo en la sincronización a Novasoft de la salida ó
  /// traslado de suministros asignados desde Controller
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class SUCorreoNotificacionesSumDC : DataContractBase
  {
    /// <summary>
    /// Id de Identificaion del Mail asociado
    /// </summary>
    [DataMember]
    public int IdCorreoNotificacion { get; set; }

    /// <summary>
    /// Correo electrónico al cual se le envía la
    /// notificación de la falla de sincronización
    /// </summary>
    [DataMember]
    [StringLength(50, MinimumLength = 5, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
    [RegularExpression(@"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}", ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "EmailNoValido")]
    public string Email { get; set; }

    /// <summary>
    /// Enumeracion que indica el estado del objeto dentro de una lista
    /// </summary>
    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }
  }
}