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
  /// Clase que contien la información de precio de los servicio
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class TAPrecioCentroCorrespondenciaDC : DataContractBase
  {
    [DataMember]
    public IEnumerable<TAImpuestosDC> Impuestos { get; set; }

    [DataMember]
    public IEnumerable<TAValorAdicional> ValoresAdicionales { get; set; }

    [DataMember]
    public IEnumerable<TAServicioCentroDeCorrespondenciaDC> PrecioCentrosCorrespondencia { get; set; }
  }
}