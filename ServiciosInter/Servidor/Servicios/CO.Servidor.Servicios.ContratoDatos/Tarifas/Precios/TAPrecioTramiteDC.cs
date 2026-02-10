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
  /// Clase que contien la información del servicio trámite
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class TAPrecioTramiteDC : DataContractBase
  {
    [DataMember]
    public IEnumerable<TAValorAdicional> ValoresAdicionales { get; set; }

    [DataMember]
    public decimal Valor { get; set; }

    [DataMember]
    public decimal ValorAdicionalLocal { get; set; }

    [DataMember]
    public decimal ValorAdicionalDocumento { get; set; }

    [DataMember]
    public IEnumerable<TAImpuestosDC> ImpuestosTramite { get; set; }
  }
}