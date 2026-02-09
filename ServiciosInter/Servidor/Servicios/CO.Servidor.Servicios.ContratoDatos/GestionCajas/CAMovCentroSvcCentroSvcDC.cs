using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.GestionCajas
{
  /// <summary>
  /// Clase contiene los datos de la tabla
  /// CentroSvcCentroSvcMov
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class CAMovCentroSvcCentroSvcDC : DataContractBase
  {
    /// <summary>
    /// Es el Id del Centro de servicio que realiza la transacción
    /// </summary>
    /// <value>
    /// The id centro servicio origen.
    /// </value>
    [DataMember]
    public long IdCentroServicioOrigen { get; set; }

    /// <summary>
    /// Es el nombre del Centro de Servicio que realiza la transacción.
    /// </summary>
    /// <value>
    /// The nombre centro servicio origen.
    /// </value>
    [DataMember]
    public string NombreCentroServicioOrigen { get; set; }

    /// <summary>
    /// Es el Id del Centro de servicio que recibe la transacción
    /// </summary>
    /// <value>
    /// The id centro servicio destino.
    /// </value>
    [DataMember]
    public long IdCentroServicioDestino { get; set; }

    /// <summary>
    /// Es el nombre del Centro de Servicio que recibe la transacción.
    /// </summary>
    /// <value>
    /// The nombre centro servicio destino.
    /// </value>
    [DataMember]
    public string NombreCentroServicioDestino { get; set; }

    /// <summary>
    /// Es el id generado en la tabla de la Caja del centro de
    /// servicio origen.
    /// </summary>
    /// <value>
    /// The id registro tx origen.
    /// </value>
    [DataMember]
    public long IdRegistroTxOrigen { get; set; }

    /// <summary>
    /// Es el id generado en la tabla de la Caja del centro de
    /// servicio Destino.
    /// </summary>
    /// <value>
    /// The id registro tx destino.
    /// </value>
    [DataMember]
    public long IdRegistroTxDestino { get; set; }

    /// <summary>
    /// La fecha en la que se realiza la transacción.
    /// </summary>
    /// <value>
    /// The fecha grabacion.
    /// </value>
    [DataMember]
    public DateTime FechaGrabacion { get; set; }

    /// <summary>
    /// Usuario que realiza la Transacción
    /// </summary>
    /// <value>
    /// The usuario registra.
    /// </value>
    [DataMember]
    public string UsuarioRegistra { get; set; }

    /// <summary>
    /// Retorna o asigna el número del precinto
    /// </summary>
    [DataMember]
    public string NumeroPrecinto { get; set; }

    /// <summary>
    /// Retorna o asigna la bolsa de seguridad
    /// </summary>
    [DataMember]
    public string BolsaSeguridad { get; set; }

    /// <summary>
    ///  Retorna o asigna el número de guía
    /// </summary>
    [DataMember]
    public long NumeroGuia { get; set; }

  }
}