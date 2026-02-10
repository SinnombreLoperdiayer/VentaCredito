using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.Cajas;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.GestionCajas
{
  /// <summary>
  /// Clase con la info de la tabla Caja Casa Matriz
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class CACajaCasaMatrizDC : DataContractBase
  {
      [DataMember]
      public long IdTransaccion { get; set; }

    /// <summary>
    /// Es la Info del concepto caja.
    /// </summary>
    /// <value>
    /// The concepto.
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
    [Display(ResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Etiquetas), Name = "Valor")]
    public decimal Valor { get; set; }

    /// <summary>
    /// Es en numero del documento bancario, an caso de ser cheque o consignacion.
    /// </summary>
    /// <value>
    /// The numero documento.
    /// </value>
    [DataMember]
    public string NumeroDocumento { get; set; }

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
    /// Descripción de la transacción sobre
    /// la caja de la empresa o casa matriz
    /// </summary>
    /// <value>
    /// The descripcion.
    /// </value>
    [DataMember]
    [Display(ResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Etiquetas), Name = "Descripcion")]
    public string Descripcion { get; set; }

    /// <summary>
    /// Es el id del centro o Empresa que realiza la
    /// transaccion
    /// </summary>
    /// <value>
    /// The id centro servicio registra.
    /// </value>
    [DataMember]
    public long IdCentroServicioRegistra { get; set; }

    /// <summary>
    /// es el  nombre del centro ó empresa que realiza la
    /// transaccion.
    /// </summary>
    /// <value>
    /// The nombre centro servicio registra.
    /// </value>
    [DataMember]
    [Display(ResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Etiquetas), Name = "CentroServicio")]
    public string NombreCentroServicioRegistra { get; set; }

    /// <summary>
    /// Fecha en la que se hizo el movimiento,
    /// valor por defecto: fecha en que se crea el registro (getdate())
    /// </summary>
    /// <value>
    /// The fecha mov.
    /// </value>
    [DataMember]
    [Display(ResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Etiquetas), Name = "FechaTransaccion")]
    public DateTime FechaMov { get; set; }

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
    /// Retorna o asigna el código de base de datos del usaurio
    /// </summary>
    [DataMember]
    public long IdCodigoUsuario { get; set; }
  }
}