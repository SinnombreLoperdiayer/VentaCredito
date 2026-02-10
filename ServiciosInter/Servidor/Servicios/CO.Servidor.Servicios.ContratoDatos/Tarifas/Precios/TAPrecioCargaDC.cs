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
  [DataContract(Namespace = "http://contrologis.com")]
  public class TAPrecioCargaDC : DataContractBase
  {
    [DataMember]
    public List<TAImpuestosDC> Impuestos { get; set; }

    [DataMember]
    public decimal ValorServicioRetorno { get; set; }

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