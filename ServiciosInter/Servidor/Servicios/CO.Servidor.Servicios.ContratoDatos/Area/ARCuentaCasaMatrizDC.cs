using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Servicios.ContratoDatos.Area
{
  /// <summary>
  /// Cuentas de un banco de una casa matriz
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class ARCuentaCasaMatrizDC : DataContractBase
  {
    /// <summary>
    /// Retorna o asigna instancia con la información del banco
    /// </summary>
    [DataMember]
    public PABanco Banco { get; set; }

    /// <summary>
    /// Retorna o asigna la identificación de la casa matriz
    /// </summary>
    [DataMember]
    public short IdCasaMatriz { get; set; }

    /// <summary>
    /// Retorna o asigna el numero de la cuenta
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NumeroCuenta", Description = "ToolTipNumeroCuenta")]
    public string NumeroCuenta { get; set; }

    /// <summary>
    /// Retorna o asigna el tipo de cuenta
    /// </summary>
    [DataMember]
    public string TipoCuenta { get; set; }

    /// <summary>
    /// Retorna o asigna la descripción del tipo de cuenta
    /// </summary>
    [DataMember]
    public string DescripcionTipoCuenta { get; set; }
  }
}