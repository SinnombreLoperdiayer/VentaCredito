using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros.Pagos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Cajas
{
    /// <summary>
    /// Clase con los campos de
    /// la consulta del resumen
    /// cierre punto
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class CAResumeCierrePuntoDC : DataContractBase
    {
        /// <summary>
        /// Gets or sets the id punto atencion.
        /// </summary>
        /// <value>
        /// Es el Id del Punto de Atencion
        /// </value>
        [DataMember]
        public long IdPuntoAtencion { get; set; }

        /// <summary>
        /// Es el Id de Cierre Efectuado por la Caja
        /// del punto
        /// </summary>
        /// <value>
        /// Es el Id de Cierre Efectuado por la Caja
        /// del punto
        /// </value>
        [DataMember]
        public long IdCierreCaja { get; set; }

        /// <summary>
        /// identifica la Caja
        /// </summary>
        /// <value>
        /// The id caja.
        /// </value>
        [DataMember]
        public int IdCaja { get; set; }

        /// <summary>
        /// Gets or sets the id concepto caja.
        /// </summary>
        /// <value>
        /// Es el Id de Concepto de Caja
        /// </value>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Id")]
        public int IdConceptoCaja { get; set; }

        /// <summary>
        /// Gets or sets the nombre concepto caja.
        /// </summary>
        /// <value>
        /// Es el nombre concepto caja.
        /// </value>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Concepto")]
        public string NombreConceptoCaja { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [es ingreso].
        /// </summary>
        /// <value>
        ///   <c>true</c> si es ingreso]; de lo contrario, <c>false</c>.
        /// </value>
        [DataMember]
        public bool EsIngreso { get; set; }

        /// <summary>
        /// Gets or sets the cantidad.
        /// </summary>
        /// <value>
        /// Es la cantidad por Concepto del servicio.
        /// </value>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Cantidad")]
        public int Cantidad { get; set; }

        /// <summary>
        /// Gets or sets the base comision.
        /// </summary>
        /// <value>
        /// Es la Base de la Comision
        /// </value>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "BaseComision")]
        public decimal BaseComision { get; set; }

        /// <summary>
        /// Gets or sets the ingreso.
        /// </summary>
        /// <value>
        /// Valor total del Servicio incluye Vr Servicio, Vr Adicionales
        /// Vr Prima, Vr tercers, Vr Impuestos,Vr Retenciones se evalua
        /// si es un ingreso</value>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Ingreso", Description = "ToolTipIngreso")]
        public decimal Ingreso { get; set; }

        /// <summary>
        /// Gets or sets the egreso.
        /// </summary>
        /// <value>
        /// Valor total del Servicio incluye Vr Servicio, Vr Adicionales
        /// Vr Prima, Vr tercers, Vr Impuestos,Vr Retenciones se evalua
        /// si es un Egreso</value>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Egreso", Description = "ToolTipEgreso")]
        public decimal Egreso { get; set; }

        /// <summary>
        /// Gets or sets the total comision empresa.
        /// </summary>
        /// <value>Es el Vr de ganancia que le corresponde a la
        /// empresa por la venta de un servicio.</value>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ComisionEmpresa")]
        public decimal TotalComisionEmpresa { get; set; }

        /// <summary>
        /// Gets or sets the total comision centro servicios.
        /// </summary>
        /// <value>Es el Vr de ganancia que le corresponde al
        /// Centro de Servicios por la venta de un servicio.</value>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ComisionAgenciaPunto")]
        public decimal TotalComisionCentroServicios { get; set; }

        /// <summary>
        /// Gets or sets the total comision agencia responsable.
        /// </summary>
        /// <value>Es el Vr de ganancia que le corresponde al
        /// punto ó agencia por la venta de un servicio.</value>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ComisionResponsable")]
        public decimal TotalComisionAgenciaResponsable { get; set; }

        /// <summary>
        /// Gets or sets the id forma pago.
        /// </summary>
        /// <value>
        /// Es el Id de la forma pago.
        /// </value>
        [DataMember]
        public short IdFormaPago { get; set; }

        /// <summary>
        /// Gets or sets the nombre forma pago.
        /// </summary>
        /// <value>
        /// Es el nombre de la forma pago.
        /// </value>
        [DataMember]
        public string NombreFormaPago { get; set; }

        /// <summary>
        /// Gets or sets the valor forma pago.
        /// </summary>
        /// <value>
        /// Es el valor de la forma pago.
        /// </value>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FormaPago")]
        public decimal ValorFormaPago { get; set; }

        /// <summary>
        /// Gets or sets the base inicial.
        /// </summary>
        /// <value>
        /// Base inicial del Centro de Servicio.
        /// </value>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "BaseInicial", Description = "ToolTipBaseInicial")]
        public decimal BaseInicial { get; set; }

        /// <summary>
        /// Gets or sets the saldo anterior efectivo.
        /// </summary>
        /// <value>
        /// Es el saldo en Efectivo del Cierre Anterior.
        /// </value>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "SaldoAnteriorEfectivo", Description = "ToolTipSaldoAnteriorEfectivo")]
        public decimal SaldoAnteriorEfectivo { get; set; }

        /// <summary>
        /// Gets or sets the id cierre asociado.
        /// </summary>
        /// <value>
        /// Es el id del Cierre Asociado si es 0
        /// no tiene un cierre definido.
        /// </value>
        [DataMember]
        public long idCierreAsociado { get; set; }

        /// <summary>
        /// Es la info de los giros no pagos por un centro de Srv.
        /// </summary>
        [DataMember]
        public PGTotalPagosDC InfoGirosNoPagosCentroSvc { get; set; }

        /// <summary>
        /// esla informacion de los alcobros sin cancelar y de los
        /// que estan en transito
        /// </summary>
        [DataMember]
        public ADAlCobrosSinCancelarDC InfoAlCobrosSinCancelar { get; set; }

        /// <summary>
        /// Muestra si la Caja esta Reportada
        /// </summary>
        [DataMember]
        public bool EstaReportado { get; set; }
    }
}