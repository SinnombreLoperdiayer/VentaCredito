using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using Framework.Servidor.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;

namespace CO.Servidor.Servicios.ContratoDatos.Cajas
{
  /// <summary>
  /// Clase que contiene los Campos
  /// de la tabla Cuenta Mensajero
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class CACuentaMensajeroDC : DataContractBase
  {
    /// <summary>
    /// Es el Id de la Transaccion
    /// </summary>
    /// <value>
    /// Es el Id de la Transaccion
    /// </value>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Id")]
    public long IdTransaccion { get; set; }

    /// <summary>
    /// Datos del Mensajero el Id a Guardar
    /// es el id de Persona Interna
    /// </summary>
    /// <value>
    /// Datos del Mensajero el Id a Guardar
    /// es el id de Persona Interna
    /// </value>
    [DataMember]
    public OUNombresMensajeroDC Mensajero { get; set; }

    /// <summary>
    /// Si la afectación de la cuenta del mensajero fué por
    /// un concepto de venta o entrega al cobro este
    /// número corresponde al número de la factura asociada
    /// </summary>
    /// <value>
    /// Si la afectación de la cuenta del mensajero fué por
    /// un concepto de venta o entrega al cobro este
    /// número corresponde al número de la factura asociada
    /// </value>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Numero")]
    public long NumeroDocumento { get; set; }

    /// <summary>
    /// Es el valor de la Transacción
    /// </summary>
    /// <value>
    /// Es el valor de la Transacción
    /// </value>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TotalReportar")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido", AllowEmptyStrings = false)]
    public decimal Valor { get; set; }
    

    /// <summary>
    /// Es el Valor agrupado del Ingreso
    /// </summary>
    /// <value>
    /// The valor ingreso.
    /// </value>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Ingreso")]
    public decimal ValorIngreso { get; set; }

    /// <summary>
    /// Es el Valor agrupado de los Egresos
    /// </summary>
    /// <value>
    /// The valor egreso.
    /// </value>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Egreso")]
    public decimal ValorEgreso { get; set; }

    /// <summary>
    /// Si el valor es un Ingreso si no es Un Egreso
    /// </summary>
    /// <value>
    /// <c>true</c> Si el valor es un Ingreso si no es Un Egreso
    /// </value>
    [DataMember]
    public bool ConceptoEsIngreso { get; set; }

    /// <summary>
    /// Es el Valor del saldo acumulado por el mensajero.
    /// se le suma o resta la transaccion segun sea
    /// el ingreso
    /// </summary>
    /// <value>
    /// Es el Valor del saldo acumulado por el mensajero.
    /// se le suma o resta la transaccion segun sea
    /// el ingreso
    /// </value>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Saldo")]
    public decimal SaldoAcumulado { get; set; }

    /// <summary>
    /// Es la fecha en la que se graba el registro.
    /// </summary>
    /// <value>
    /// Es la dfecha en la que se graba el registro.
    /// </value>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Fecha")]
    public DateTime FechaGrabacion { get; set; }

    /// <summary>
    /// Es el Usuario que registra la transacción
    /// </summary>
    /// <value>
    /// Es el Usuario que registra la transacción
    /// </value>
    [DataMember]
    public string UsuarioRegistro { get; set; }

    /// <summary>
    /// Son las Observaciones de la Cta del Mensajero
    /// </summary>
    /// <value>
    /// The observaciones.
    /// </value>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Observaciones")]
    public string Observaciones { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Concepto")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido", AllowEmptyStrings = false)]
    public CAConceptoCajaDC ConceptoCajaMensajero { get; set; }

    /// <summary>
    /// Número de la planilla con la cual el mensajero reporta sus ventas
    /// </summary>
    [DataMember]
    public long NoPlanillaVentas { get; set; }

    /// <summary>
    /// Número de la planilla con la cual el mensajero reporta sus al cobros
    /// </summary>
    [DataMember]
    public long NoPlanillaAlCobros { get; set; }

    [DataMember]
    public PUCentroServiciosDC CentroLogContrapartida { get; set; }
  }
}