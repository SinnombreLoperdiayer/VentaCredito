using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Tarifas
{
  /// <summary>
  /// Clase que contien la información de la forma de pago por servicio
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class TAFormaPagoServicio : DataContractBase
  {
    [DataMember]
    public int IdServicio { get; set; }

    [DataMember]
    public List<TAFormaPago> FormaPago { get; set; }
  }
}