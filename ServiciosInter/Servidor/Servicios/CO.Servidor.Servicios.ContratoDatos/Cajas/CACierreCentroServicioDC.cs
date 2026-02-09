using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.AccessControl;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros.Pagos;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Cajas
{
  /// <summary>
  /// Clase que contiene los campos
  /// del cierre del Centro de Servicio
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class CACierreCentroServicioDC : DataContractBase
  {
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Id")]
    public long IdCierreCentroServicio { get; set; }

    /// <summary>
    /// Es el id del centro de Servcio a Cerrar.
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoAgencia", Description = "ToolTipTipoAgencia")]
    public long IdCentroServicio { get; set; }

    /// <summary>
    /// Es la Base Inicial del Punto ó
    /// centro de servicio
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "BaseInicial", Description = "ToolTipBaseInicial")]
    public decimal BaseInicial { get; set; }

    /// <summary>
    /// Es el Saldo del anterior Cierre
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "SaldoAnteriorEfectivo", Description = "ToolTipSaldoAnteriorEfectivo")]
    public decimal SaldoAnteriorEfectivo { get; set; }

    /// <summary>
    /// Es el total de los Ingresos en efectivo del Cierre Actual.
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TotalIngresosEfectivo", Description = "ToolTipTotalIngresosEfectivo")]
    public decimal TotalIngresosEfectivo { get; set; }

    /// <summary>
    /// Es el total de Egresos en efectivo del cierre actual.
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TotalEgresosEfectivo", Description = "ToolTipTotalEgresosEfectivo")]
    public decimal TotalEgresosEfectivo { get; set; }

    /// <summary>
    ///Es el total de los Ingresos diferentes
    ///al efectico del Cierre Actual.
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TotalIngresosOtrasFormas", Description = "ToolTipTotalIngresosOtrasFormas")]
    public decimal TotalIngresosOtrasFormas { get; set; }

    /// <summary>
    ///Es el total de los Egresos diferentes
    ///al efectico del Cierre Actual.
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TotalEgresosOtrasFormas", Description = "ToolTipTotalEgresosOtrasFormas")]
    public decimal TotalEgresosOtrasFormas { get; set; }

    /// <summary>
    /// Es el saldo final este campo es calculado
    /// en la bd.
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "SaldoFinalEfectivo", Description = "ToolTipSaldoFinalEfectivo")]
    public decimal SaldoFinalEfectivo { get; set; }

    /// <summary>
    /// Es el usuario que cierra el punto.
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "UsuarioCierraPunto", Description = "ToolTipUsuarioCierraPunto")]
    public string UsuarioCierraPunto { get; set; }

    /// <summary>
    /// Lista de las Cajas a reportar por el Punto.
    /// </summary>
    [DataMember]
    public List<CAResumenCierreCajaDC> CajasPuntoReportadas { get; set; }

    /// <summary>
    /// Es la Fecha de Cierre.
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FechaCierreCaja", Description = "ToolTipFechaCierreCaja")]
    [Filtrable("CCS_FechaGrabacion", "Fecha Cierre", COEnumTipoControlFiltro.DatePicker)]
    public DateTime FechaCierre { get; set; }

    /// <summary>
    /// Es la Info de los giros no pagos por
    /// el centro de Servicio
    /// </summary>
    [DataMember]
    public PGTotalPagosDC InfoGirosNoPagosCentroSvc { get; set; }
  }
}