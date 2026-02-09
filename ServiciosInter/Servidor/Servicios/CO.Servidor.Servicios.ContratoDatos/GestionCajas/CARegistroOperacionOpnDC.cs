using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.GestionCajas
{
  /// <summary>
  /// Contrato de datos para las
  /// operaciones sobre caja de Operación Nacional
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class CARegistroOperacionOpnDC : DataContractBase
  {
    /// <summary>
    /// Retorna o asigna indicador para operación de ingreso o egreso
    /// </summary>
    [DataMember]
    public bool EsIngreso { get; set; }

    /// <summary>
    /// Retorna o asigna descripción de la operación
    /// </summary>
    [DataMember]
    public string Descripcion { get; set; }

    /// <summary>
    /// Retorna o asigna observación de la operación
    /// </summary>
    [DataMember]
    public string Observacion { get; set; }

    /// <summary>
    /// Retorna o asigna  el identificador del concepto de caja de la operación
    /// </summary>
    [DataMember]
    public int IdConceptoCaja { get; set; }

    /// <summary>
    /// Descripcion del nombre del concepto de
    /// caja
    /// </summary>
    [DataMember]
    public string NombreConceptoCaja { get; set; }

    /// <summary>
    /// Retorna o asigna  Indicador del origen de la operación
    /// </summary>
    [DataMember]
    public string MovHechoPor { get; set; }

    /// <summary>
    ///  Retorna o asigna el Numero de documento de la operación
    /// </summary>
    [DataMember]
    public string NumeroDocumento { get; set; }

    /// <summary>
    /// Retorna o asigna el Valor de la operación
    /// </summary>
    [DataMember]
    public decimal Valor { get; set; }

    /// <summary>
    /// Retorna o asigna el identificador del centro de servicios desde donde se hace la operación
    /// </summary>
    [DataMember]
    public short IdCasaMatriz { get; set; }

    /// <summary>
    /// Retorna o asigna el código del usuario
    /// </summary>
    [DataMember]
    public long IdCodigoUsuario { get; set; }

    /// <summary>
    /// Retorna o asignal el usuario que hace la operación
    /// </summary>
    [DataMember]
    public string CreadoPor { get; set; }

    /// <summary>
    /// Retorna o asigna la fecha en la cual se hace el movimiento
    /// </summary>
    [DataMember]
    public DateTime FechaMovimiento { get; set; }

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