using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Cliente.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Precios;
using Framework.Servidor.Comun;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Servicios.ContratoDatos.Tarifas.Precios
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class TAPrecioMensajeriaDC : DataContractBase
  {
    [DataMember]
    public List<TAImpuestosDC> Impuestos { get; set; }

    [DataMember]
    public decimal ValorKiloInicial { get; set; }

    [DataMember]
    public decimal ValorKiloAdicional { get; set; }

    [DataMember]
    public decimal Valor { get; set; }

    [DataMember]
    public decimal ValorContraPago { get; set; }

    [DataMember]
    public decimal ValorPrimaSeguro { get; set; }
  }
}