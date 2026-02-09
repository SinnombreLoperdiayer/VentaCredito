using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.GestionCajas.Cierres
{
  /// <summary>
  /// Contrato de datos para información del cierre de caja de gestión
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class CACierreCajaGestionDC : DataContractBase
  {
    /// <summary>
    /// Retorna o asigna el identificador de la casa matriz
    /// </summary>
    [DataMember]
    public short IdCasaMatriz { get; set; }

    /// <summary>
    /// Retorna o asigna el código del usuario  que hizo el cierre
    /// </summary>
    [DataMember]
    public long IdCodigoUsuario { get; set; }

    /// <summary>
    /// Retorna o asigna el nombre de usuario que hizo el cierre
    /// </summary>
    [DataMember]
    public string Usuario { get; set; }

    /// <summary>
    /// Retorna o asigna el identificador del cierre
    /// </summary>
    [DataMember]
    public long IdCierre { get; set; }

    /// <summary>
    /// Retorna o asigna el identificador del tipo de apertura
    /// </summary>
    [DataMember]
    public string TipoApetura { get; set; }

    /// <summary>
    /// Retorna o asigna la descripción del identificador del tipo de apertura
    /// </summary>
    [DataMember]
    public string DescripcionTipoApetura { get; set; }

    /// <summary>
    /// Retorna o asigna el total de ingresos del cierre
    /// </summary>
    [DataMember]
    public decimal TotalIngresos { get; set; }

    /// <summary>
    /// Retorna o asigna el total de egresos del cierre
    /// </summary>
    [DataMember]
    public decimal TotalEgresos { get; set; }

    /// <summary>
    /// Retorna o asigna la fecha de la apertura
    /// </summary>
    [DataMember]
    public DateTime FechaApertura { get; set; }

    /// <summary>
    /// Retorna o asigna la fecha del cierre
    /// </summary>
    [DataMember]
    public DateTime FechaCierre { get; set; }
  }
}