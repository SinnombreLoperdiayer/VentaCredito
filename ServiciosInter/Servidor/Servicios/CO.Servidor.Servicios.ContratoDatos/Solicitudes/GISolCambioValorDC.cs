using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Solicitudes
{
  /// <summary>
  /// Clase que contiene la informacion de la
  /// solicitud del valor del ajuste al
  /// giro y al porte
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class GISolCambioValorDC : DataContractBase
  {
    /// <summary>
    /// Identificador de la solicitud
    /// </summary>
    [DataMember]
    public long IdSolicitud { get; set; }

    /// <summary>
    /// Es el valor sobre el cual se vendió el giro,
    /// este valor puede ser diferente al registrado
    /// en el sistema y a éste valor se
    /// quiere llegar haciendo el ajuste.
    /// </summary>
    [DataMember]
    public decimal ValorRealGiro { get; set; }

    /// <summary>
    /// Diferencia entre lo registrado en el
    /// sistema como valor del giro y el valor real
    /// </summary>
    [DataMember]
    public decimal ValorAAjustar { get; set; }

    /// <summary>
    /// Valor del porte calculado por la venta del
    /// giro en el sistema cuando se vendió el giro
    /// inicialmente, este valor se guarda como
    /// parte del registro de trazabiliad del
    /// ajuste de valor
    /// </summary>
    [DataMember]
    public decimal ValorPorteInicial { get; set; }

    /// <summary>
    /// Valor de la diferencia en entre el porte
    /// inicial y el porte calculado una vez
    /// ajustado el valor del giro
    /// </summary>
    [DataMember]
    public decimal ValorPorteDiferencia { get; set; }
  }
}