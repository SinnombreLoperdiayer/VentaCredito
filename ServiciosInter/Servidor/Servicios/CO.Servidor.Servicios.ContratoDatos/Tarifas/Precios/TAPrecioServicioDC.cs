using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Cliente.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Servicios;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Tarifas.Precios
{
  /// <summary>
  /// Clase que contiene la información de precio se los servicios
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class TAPrecioServicioDC : DataContractBase
  {
    [DataMember]
    public IEnumerable<TAImpuestosDC> Impuestos { get; set; }

    [DataMember]
    public IEnumerable<TAValorAdicional> ValoresAdicionales { get; set; }

    /// <summary>
    /// Valor en pesos
    /// </summary>
    [DataMember]
    public decimal Valor { get; set; }

    /// <summary>
    /// Trm Que aplica para la tarifa cuando es internacional el envio
    /// </summary>
    [DataMember]
    public decimal TRM { get; set; }

    /// <summary>
    /// Valor en dolares  cuando es internacional el envio
    /// </summary>
    public decimal ValorDolares
    {
      get
      {
        return Valor / TRM;
      }
    }

    /// <summary>
    /// Prima de seguro
    /// </summary>
    [DataMember]
    public decimal PrimaSeguro { get; set; }
  }
}