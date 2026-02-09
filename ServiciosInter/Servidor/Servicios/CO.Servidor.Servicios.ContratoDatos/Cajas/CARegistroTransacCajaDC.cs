using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Cajas
{
  /// <summary>
  /// Clase que contiene los campos
  /// del registro de transaccion de
  /// la caja
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class CARegistroTransacCajaDC : DataContractBase
  {
    /// <summary>
    /// Es la Informacion de la Apertura de Caja.
    /// </summary>
    [DataMember]
    public CAAperturaCajaDC InfoAperturaCaja { get; set; }

    /// <summary>
    /// Es el id del centro o punto de Servicio
    /// </summary>
    [DataMember]
    public long IdCentroServiciosVenta { get; set; }

    /// <summary>
    /// El Id del centro de servicios destino de la guia, se utiliza en la anulacion de la guia para poder identificar el CServicios Destino en caso de que la guia tenga forma de pago al cobro.
    /// </summary>
    [DataMember]
    public long IdCentroServiciosDestinoGuia { get; set; }

    /// <summary>
    /// Es el nombre de centro servicio o Punto.
    /// </summary>
    [DataMember]
    public string NombreCentroServiciosVenta { get; set; }

    /// <summary>
    /// Centro de costos del centro de servicio
    /// </summary>
    [DataMember]
    public string CentroCostos { get; set; }

    /// <summary>
    /// En el caso de los Puntos es el Id de la Agencia
    /// reponsable del Punto.
    /// </summary>
    [DataMember]
    public long IdCentroResponsable { get; set; }

    /// <summary>
    /// En el caso de los Puntos es el nombre de la Agencia
    /// reponsable del Punto.
    /// </summary>
    [DataMember]
    public string NombreCentroResponsable { get; set; }

    /// <summary>
    /// Es el valor Total de producto
    /// este valor incluye los valores de VrServicio + VrAdicionales
    /// + VrPrima + VrTerceros.
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Etiquetas), Name = "Valor")]
    public decimal ValorTotal { get; set; }

    /// <summary>
    /// Es el valor del Total de los Impuestos.
    /// </summary>
    [DataMember]
    public decimal TotalImpuestos { get; set; }

    /// <summary>
    /// Es el valor Total de las Retenciones.
    /// </summary>
    [DataMember]
    public decimal TotalRetenciones { get; set; }

    /// <summary>
    /// Son los Tipos CRE(Credito)-PEA(Peaton)-.
    /// </summary>
    [DataMember]
    public CAEnumTipoDatosAdicionales TipoDatosAdicionales { get; set; }

    /// <summary>
    /// Es la lista del detalle de los registros de la Transaccion.
    /// </summary>
    [DataMember]
    public List<CARegistroTransacCajaDetalleDC> RegistrosTransacDetallesCaja { get; set; }

    /// <summary>
    /// es el registro que se llena si se realiza un
    /// proceso de cliente de tipo credito.
    /// </summary>
    [DataMember]
    public CARegistroTransClienteCreditoDC RegistroVentaClienteCredito { get; set; }

    /// <summary>
    /// es la forma de pago con su valor respectivo
    /// </summary>
    [DataMember]
    public List<CARegistroVentaFormaPagoDC> RegistroVentaFormaPago { get; set; }

    /// <summary>
    /// Es el usuario que registra el Proceso.
    /// </summary>
    [DataMember]
    public string Usuario { get; set; }

    /// <summary>
    /// Es usuario gestion para la apertura de Caja
    /// </summary>
    /// <value>
    ///   <c>true</c> if [es usuario gestion]; otherwise, <c>false</c>.
    /// </value>
    [DataMember]
    public bool EsUsuarioGestion { get; set; }

    /// <summary>
    /// Es el Id de la Casa Matriz
    /// </summary>
    [DataMember]
    public short IdCasaMatriz { get; set; }

    /// <summary>
    /// Se Valida que la transaccion sea entre cajas ppal y auxiliar
    /// </summary>
    [DataMember]
    public bool EsTransladoEntreCajas { get; set; }
  }
}