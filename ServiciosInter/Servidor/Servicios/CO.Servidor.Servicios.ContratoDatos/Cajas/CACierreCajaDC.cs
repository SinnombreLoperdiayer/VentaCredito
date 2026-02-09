using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Cajas
{
    /// <summary>
    /// Clase que contiene los Valores
    /// del Cierre de la Caja
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class CACierreCajaDC : DataContractBase
    {
        /// <summary>
        /// Gets or sets the id cierre caja.
        /// </summary>
        /// <value>
        /// Es el id de Cierre de la Caja es el mismo Id de Apertura de Caja.
        /// </value>
        [DataMember]
        public long IdCierreCaja { get; set; }

        /// <summary>
        /// Gets or sets the id caja.
        /// </summary>
        /// <value>
        /// Es el id de la caja que realiza el Cierre.
        /// </value>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "CajaNumero", Description = "TooltipCaja")]
        public int IdCaja { get; set; }

        /// <summary>
        /// Gets or sets the id punto atencion.
        /// </summary>
        /// <value>
        /// Es el punto de atencion al que corresponde la caja.
        /// </value>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoAgencia", Description = "ToolTipTipoAgencia")]
        public long IdPuntoAtencion { get; set; }

        /// <summary>
        /// Gets or sets the total reportar.
        /// </summary>
        /// <value>
        /// Es el valor del ingrso - el valor del egreso.
        /// </value>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TotalReportarEfectivo", Description = "ToolTipTotalReportarEfectivo")]
        public decimal TotalReportar { get; set; }

        /// <summary>
        /// Gets or sets the total ingreso efectivo.
        /// </summary>
        /// <value>
        /// Es el valor del Ingreso del efectivo por parte del cajero
        /// </value>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TotalIngresosEfectivo", Description = "ToolTipTotalEgresosEfectivo")]
        public decimal TotalIngresoEfectivo { get; set; }

        /// <summary>
        /// Gets or sets the total egreso efectivo.
        /// </summary>
        /// <value>
        /// Es el valor del Egreso del efectivo por parte del cajero
        /// </value>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TotalEgresosEfectivo", Description = "ToolTipTotalTotalEgreso")]
        public decimal TotalEgresoEfectivo { get; set; }

        /// <summary>
        /// Gets or sets the total ingreso otras formas.
        /// </summary>
        /// <value>
        /// Son los ingresos diferentes al efectivo que recibio el cajero.
        /// </value>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TotalIngresosOtrasFormas", Description = "ToolTipTotalIngresosOtrasFormas")]
        public decimal TotalIngresoOtrasFormas { get; set; }

        /// <summary>
        /// Gets or sets the total egreso otras formas.
        /// </summary>
        /// <value>
        /// Son los Egresos diferentes al efectivo que recibio el cajero.
        /// </value>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TotalEgresosOtrasFormas", Description = "ToolTipTotalEgresosOtrasFormas")]
        public decimal TotalEgresoOtrasFormas { get; set; }

        /// <summary>
        /// Gets or sets the total comision venta.
        /// </summary>
        /// <value>
        /// Es el valor de comision que le corresponde al punto
        /// o centro de servicio por una venta.
        /// </value>
        [DataMember]
        public decimal TotalComisionVenta { get; set; }

        /// <summary>
        /// Gets or sets the total comision responsable.
        /// </summary>
        /// <value>
        /// Es el valor de comision que le corresponde al
        /// responsable del punto ó centro de servicio por una venta.
        /// </value>
        [DataMember]
        public decimal TotalComisionResponsable { get; set; }

        /// <summary>
        /// Gets or sets the total comision empresa.
        /// </summary>
        /// <value>
        /// Es el valor de comision que le corresponde a la
        /// empresa por una venta.
        /// </value>
        [DataMember]
        public decimal TotalComisionEmpresa { get; set; }

        /// <summary>
        /// Gets or sets the total A reportar.
        /// </summary>
        /// <value>
        /// Es el total a reportar del centro o punto de
        /// servicio
        /// </value>
        [DataMember]
        public decimal TotalAReportar { get; set; }

        /// <summary>
        /// Gets or sets the usuario caja.
        /// </summary>
        /// <value>
        /// Es el usuario que abrio la caja
        /// </value>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Usuario")]
        public string UsuarioCaja { get; set; }

        /// <summary>
        /// Gets or sets the creado por.
        /// </summary>
        /// <value>
        /// Es el usuario que cierra la Caja
        /// </value>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Usuario")]
        public string CreadoPor { get; set; }

        /// <summary>
        /// Es la fecha de Cierre de la Caja
        /// </summary>
        /// <value>
        /// Es la fecha con la que abrio la caja.
        /// </value>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FechaCierre", Description = "ToolTipFechaCierre")]
        public DateTime? FechaCierre { get; set; }

        /// <summary>
        /// Fecha de Apertura de Caja
        /// </summary>
        /// <value>
        /// The fecha apertura.
        /// </value>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FechaInicioVenta", Description = "ToolTipFechaInicioVenta")]
        public DateTime? FechaApertura { get; set; }

        /// <summary>
        /// Es el id del Usuario de Cierre
        /// </summary>
        [DataMember]
        public long idUsuario { get; set; }

        /// <summary>
        /// es el número total de registros 
        /// </summary>
        [DataMember]
        public int? NRegistros { get; set; }
    }
}