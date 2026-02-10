using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Servicios.ContratoDatos.Cajas
{
    /// <summary>
    /// Clase que contiene los campos del detalle
    /// de la caja
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class CARegistroTransacCajaDetalleDC : DataContractBase
    {
        /// <summary>
        /// Es el concepto de caja o item cobrado.
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Etiquetas), Name = "Concepto")]
        public CAConceptoCajaDC ConceptoCaja { get; set; }

        /// <summary>
        /// Cantidad de Items Cobrados.
        /// </summary>
        [DataMember]
        public short Cantidad { get; set; }

        /// <summary>
        /// Es el valor que le envia un remitente a un
        /// destinatario. y
        /// </summary>
        [DataMember]
        public decimal ValorTercero { get; set; }

        /// <summary>
        /// Es el valor de los impuestos del servicio
        /// </summary>
        /// <value>
        /// Es el valor de los impuestos del servicio
        /// </value>
        [DataMember]
        public decimal ValorImpuestos { get; set; }

        /// <summary>
        /// Es el valor de las retenciones del servicio.
        /// </summary>
        [DataMember]
        public decimal ValorRetenciones { get; set; }

        /// <summary>
        /// Es el numero del giro ó numero de guia.
        /// </summary>
        [DataMember]
        public long Numero { get; set; }

        /// <summary>
        /// Es la Observación referente al detalle de la
        /// transaccion
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Etiquetas), Name = "Observaciones")]
        public string Observacion { get; set; }

        /// <summary>
        /// Descripción de la transacción, es un texto libre
        /// que adiciona una descripción a la transacción de caja
        /// aplica para las transacciones entre RACOL y centros de Servicio
        /// pueden ir Consignacion, Retiro ó Abastecimientos y otros segun
        /// se configure
        /// </summary>
        [DataMember]
        public string Descripcion { get; set; }

        /// <summary>
        /// Es el valor de la prima de seguro sobre el envio.
        /// </summary>
        [DataMember]
        public decimal ValorPrimaSeguros { get; set; }

        /// <summary>
        /// Es el valor del servicio.
        /// </summary>
        [DataMember]
        public decimal ValorServicio { get; set; }

        /// <summary>
        /// Es la sumatoria de los Valores Adicionales como:
        /// Son los valores adicionales al envio de un paquete que el
        /// cliente cancela
        /// </summary>
        [DataMember]
        public decimal ValoresAdicionales { get; set; }

        /// <summary>
        /// Es el valor por el que se declara el paquete
        /// </summary>
        [DataMember]
        public decimal ValorDeclarado { get; set; }

        /// <summary>
        /// Es le numero de la Factura
        /// </summary>
        [DataMember]
        public string NumeroFactura { get; set; }

        /// <summary>
        /// Son los estados de facturacion de una venta
        /// PEN(PENDIENTE)-FACT(FACTURADO)-ANU(ANULADO)
        /// </summary>
        [DataMember]
        public CAEnumEstadoFacturacion EstadoFacturacion { get; set; }

        /// <summary>
        /// Es la fecha de la Facturacon de la venta
        /// </summary>
        [DataMember]
        public DateTime FechaFacturacion { get; set; }

        /// <summary>
        /// <c>verdadero</c> si [es una entrada a la caja]; de lo contrario es<c>false</c>.
        /// </summary>
        [DataMember]
        public bool ConceptoEsIngreso { get; set; }

        /// <summary>
        /// Es la lista de impuestos asociados a un detalle.
        /// </summary>
        [DataMember]
        public List<CARegistroTranscDtllImpuestoDC> LtsImpuestos { get; set; }

        /// <summary>
        /// Id del registro de la transaccion
        /// </summary>
        [DataMember]
        public long IdRegistroTranscaccion { get; set; }

        /// <summary>
        /// Número de comprobante de la transacción si aplica.
        /// </summary>
        [DataMember]
        public string NumeroComprobante { get; set; }

        /// <summary>
        /// Clase que contiene los campos del detalle
        /// de la caja Adicionales generalmente para las transacciones
        /// de las cajas principales
        /// </summary>
        [DataMember]
        public CARegistroTransacCajaDetallAdicionalDC DetalleAdicional { get; set; }
    }
}