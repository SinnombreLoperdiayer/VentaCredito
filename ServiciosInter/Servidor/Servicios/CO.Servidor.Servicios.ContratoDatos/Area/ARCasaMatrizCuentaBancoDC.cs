using System.Collections.Generic;
using System.Runtime.Serialization;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Servicios.ContratoDatos.Area
{
  /// <summary>
  /// Clase que contiene la información de los bancos de una casa
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class ARCasaMatrizCuentaBancoDC : DataContractBase
  {
    /// <summary>
    /// Retorna o asigna las cuentas por banco de una casa matriz
    /// </summary>
    [DataMember]
    public IList<ARCuentaCasaMatrizDC> CuentaBanco { get; set; }

    /// <summary>
    /// Retorna o asigna el identificador de una casa matriz
    /// </summary>
    [DataMember]
    public short IdCasaMatriz { get; set; }

    /// <summary>
    /// Retorna o asigna el nombre de la casa matriz
    /// </summary>
    [DataMember]
    public string NombreCasaMatriz { get; set; }
  }
}