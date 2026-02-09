using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros.Pagos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;

namespace CO.Servidor.Servicios.ContratoDatos.Cajas.Cierres
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class CAConsolidadoCierreDC
    {
        [DataMember]
        public List<CAResumenCierreCajaPrincipalDC> ConceptosAgrupados { get; set; }

        [DataMember]
        public List<CAResumenCierreCajaPrincipalFormaPagoDC> FormasPagoAgrupadas { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "BaseInicial", Description = "ToolTipBaseInicial")]
        public decimal SaldoInicial { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "SaldoAnteriorEfectivo", Description = "ToolTipSaldoAnteriorEfectivo")]
        public decimal SaldoAnteriorEfectivo { get; set; }

        /// <summary>
        /// Es la info de los giros no pagos por un centro de Srv.
        /// </summary>
        [DataMember]
        public PGTotalPagosDC InfoGirosNoPagosCentroSvc { get; set; }

        /// <summary>
        /// Retorna o asigna los identificadores de cierre de caja auxiliar consolidados en el resumen de caja
        /// </summary>
        [DataMember]
        public List<long> CierresCajaAuxiliaresReportadas { get; set; }

        /// <summary>
        /// esla informacion de los alcobros sin cancelar y de los
        /// que estan en transito
        /// </summary>
        [DataMember]
        public ADAlCobrosSinCancelarDC InfoAlCobrosSinCancelar { get; set; }
    }
}