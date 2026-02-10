using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.Cajas;
using CO.Servidor.Servicios.ContratoDatos.GestionCajas;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.GestionCajas
{
  /// <summary>
  /// Clase que contiene la informacion para aplicar una operación sobre la Caja Banco
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class CACajaBancoDC : DataContractBase
  {
    /// <summary>
    /// Es el Id del Banco a realizar la Transaccion
    /// </summary>
    /// <value>
    /// The id banco.
    /// </value>
    [DataMember]
    public string IdBanco { get; set; }

    /// <summary>
    /// Nombre del banco a Realizar la transaccion
    /// </summary>
    /// <value>
    /// The descripcion banco.
    /// </value>
    [DataMember]
    public string DescripcionBanco { get; set; }

    /// <summary>
    /// Numetro de Cta del banco
    /// </summary>
    /// <value>
    /// The numero cta.
    /// </value>
    [DataMember]
    public string NumeroCta { get; set; }

    /// <summary>
    /// Es la Info del concepto caja.
    /// </summary>
    /// <value>
    /// The concepto caja.
    /// </value>
    [DataMember]
    public CAConceptoCajaDC ConceptoCaja { get; set; }

    /// <summary>
    /// El valor por el que se realiza la transaccion
    /// </summary>
    /// <value>
    /// The valor.
    /// </value>
    [DataMember]
    public decimal Valor { get; set; }

    /// <summary>
    /// Es en numero del documento bancario, an caso de ser cheque o consignacion.
    /// </summary>
    /// <value>
    /// The documento bancario.
    /// </value>
    [DataMember]
    public CADocumentoBancarioDC DocumentoBancario { get; set; }

    /// <summary>
    /// informacion relevante frente a la
    /// transaccion
    /// </summary>
    /// <value>
    /// The observacion.
    /// </value>
    [DataMember]
    public string Observacion { get; set; }

    /// <summary>
    /// Fecha en la que se hizo el movimiento,
    /// valor por defecto: fecha en que se crea el registro (getdate())
    /// </summary>
    /// <value>
    /// The fecha movimiento.
    /// </value>
    [DataMember]
    public DateTime FechaMovimiento { get; set; }

    /// <summary>
    /// El movimiento se hizo por: Un centro de servicios
    /// a la caja banco: CES, Empresa hace el movimiento: EMP
    /// </summary>
    /// <value>
    /// The mov hecho por.
    /// </value>
    [DataMember]
    public string MovHechoPor { get; set; }

    /// <summary>
    /// Es el id del centro o Empresa que realiza la
    /// transaccion
    /// </summary>
    /// <value>
    /// The id centro registra.
    /// </value>
    [DataMember]
    public long IdCentroRegistra { get; set; }

    /// <summary>
    /// es el  nombre del centro ó empresa que realiza la
    /// transaccion.
    /// </summary>
    /// <value>
    /// The nombre centro registra.
    /// </value>
    [DataMember]
    public string NombreCentroRegistra { get; set; }

    /// <summary>
    /// Fecha de Grabacion del registro
    /// </summary>
    /// <value>
    /// The fecha grabacion.
    /// </value>
    [DataMember]
    public DateTime FechaGrabacion { get; set; }

    /// <summary>
    /// Usuario que registra la Operacion
    /// </summary>
    /// <value>
    /// The creado por.
    /// </value>
    [DataMember]
    public string CreadoPor { get; set; }

    /// <summary>
    /// Retorna o asigna el identificador de la casa matriz del banco
    /// </summary>
    [DataMember]
    public short IdCasaMatriz { get; set; }

    /// <summary>
    /// Retorna o asigna la apertura de caja
    /// </summary>
    public long IdAperturaCaja { get; set; }

    /// <summary>
    /// Retorna o asigna el código del usuario que hace la operación
    /// </summary>
    [DataMember]
    public long IdCodigoUsuario { get; set; }

    /// <summary>
    /// Número de comprobante asociado cuando es un ajuste x 
    /// </summary>
    [DataMember]
    public string NumeroComprobante { get; set; }
  }
}