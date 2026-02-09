using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using CO.Servidor.Facturacion;
using CO.Servidor.Servicios.ContratoDatos.Area;
using CO.Servidor.Servicios.ContratoDatos.Facturacion;
using CO.Servidor.Servicios.Contratos;

namespace CO.Servidor.Servicios.Implementacion.Facturacion
{
    /// <summary>
    ///Implementacion de la interfaz de facturación
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class FAFacturacionSvc : IFAFacturacionSvc
    {
        /// <summary>
        /// Almacena una factura nueva manual
        /// </summary>
        /// <param name="facturaManual"></param>
        public long GuardarFacturaManual(FAFacturaClienteDC facturaManual)
        {
            return FAFacturacionManual.Instancia.GuardarFacturaManual(facturaManual);
        }

        /// <summary>
        /// Consulta las programaciones que existen ya creadas en el sistema según los criterios de búsqueda establecidos
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <returns></returns>
        public IEnumerable<FAProgramacionFacturaDC> ConsultarProgramacionesFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return FAProgramacionFactura.Instancia.ConsultarProgramacionesFiltro(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out  totalRegistros);
        }

        /// <summary>
        /// Consulta las programaciones no ejecutadas de un cliente y un rango de fechas
        /// </summary>        
        /// <returns></returns>
        public IEnumerable<FAProgramacionFacturaDC> ConsultarProgramacionesNoEjecutadas(DateTime fechaDesde, DateTime fechaHasta, int idCliente)
        {
            return FAProgramacionFactura.Instancia.ConsultarProgramacionesNoEjecutadas(fechaDesde, fechaHasta, idCliente);
        }

        /// <summary>
        /// Inserta una nueva exclusión asociada a una programación
        /// </summary>
        /// <param name="exclusion"></param>
        public List<FAExclusionProgramacionDC> ExcluirMovimientoDeProgramacion(FAExclusionProgramacionDC exclusion)
        {
            return FAProgramacionFactura.Instancia.ExcluirMovimientoDeProgramacion(exclusion);
        }

        /// <summary>
        /// Permite deshacer la exclusión de un movimiento ya excluido previamente
        /// </summary>
        /// <param name="exclusion">Exclusión que se desea deshacer</param>
        public void DeshacerExclusionDeMovimiento(FAExclusionProgramacionDC exclusion)
        {
            FAProgramacionFactura.Instancia.DeshacerExclusionDeMovimiento(exclusion);
        }

        public long GenerarFacturaProgramada(FAProgramacionFacturaDC programacion)
        {
            return FAProgramacionFactura.Instancia.GenerarFacturaProgramada(programacion);
        }

        /// <summary>
        /// Agrega un concepto nuevo a una programación de una factura.
        /// </summary>
        /// <param name="concepto"></param>
        public int GuardaConceptoProgramado(FAConceptoProgramadoDC concepto)
        {
            return FAProgramacionFactura.Instancia.GuardaConceptoProgramado(concepto);
        }

        /// <summary>
        /// Elimina un concepto de una programación
        /// </summary>
        /// <param name="concepto"></param>
        public void EliminarConceptoProgramacion(FAConceptoProgramadoDC concepto)
        {
            FAProgramacionFactura.Instancia.EliminarConceptoProgramacion(concepto);
        }

        /// <summary>
        /// Cambia la programación de una factura a una nueva fecha
        /// </summary>
        /// <param name="nuevaProgramacion"></param>
        public void CambiarProgramacionFactura(FAProgramacionFacturaDC nuevaProgramacion, DateTime nuevaFecha)
        {
            FAProgramacionFactura.Instancia.CambiarProgramacionFactura(nuevaProgramacion, nuevaFecha);
        }

        /// <summary>
        /// Metodo que consulta las facturas existentes en el sistema utilizando unos parámetros de búsqueda
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <returns></returns>
        public IEnumerable<FAFacturaClienteAutDC> ConsultaFacturasFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return FAConsultafacturas.Instancia.ConsultaFacturasFiltro(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Anular la factura indicada
        /// </summary>
        /// <param name="numeroFactura"></param>
        public void AnularFactura(FAEstadoFacturaDC estadoFactura)
        {
            FAConsultafacturas.Instancia.AnularFactura(estadoFactura);
        }

        /// <summary>
        /// Permite reprogramar una factura anulada para que nuevamente sea generada
        /// </summary>
        /// <param name="FechaNueva"></param>
        public void ReprogramarFactura(DateTime FechaNueva, FAFacturaClienteAutDC factura)
        {
            FAConsultafacturas.Instancia.ReprogramarFactura(FechaNueva, factura);
        }

        /// <summary>
        /// permite aprobar una factura
        /// </summary>
        /// <param name="estadoFactura"></param>
        public void AprobarFactura(FAEstadoFacturaDC estadoFactura)
        {
            FAConsultafacturas.Instancia.AprobarFactura(estadoFactura);
        }

        /// <summary>
        /// Agrega una nueva nota crédito o débito a una factura
        /// </summary>
        /// <param name="nota"></param>
        public FANotaFacturaDC AgregarNotaFactura(FANotaFacturaDC nota)
        {
            return FAConsultafacturas.Instancia.AgregarNotaFactura(nota);
        }

        /// <summary>
        /// Crea las Nvs Notas asociadas a cada Detalle de la Factura
        /// </summary>
        /// <param name="lstConceptosDtll">Lista del detalle que lleva Notas Credito o Debito</param>
        /// <returns>la lista del detalle con las inserciones Realizadas</returns>
        public ObservableCollection<FAConceptoFacturaDC> AgregarNotasAFactura(ObservableCollection<FAConceptoFacturaDC> lstConceptosDtll)
        {
            return FAConsultafacturas.Instancia.AgregarNotasAFactura(lstConceptosDtll);
        }

        /// <summary>
        /// Agrega una nueva nota crédito o débito a una factura
        /// </summary>
        /// <param name="nota"></param>
        public void EliminarNotaFactura(FANotaFacturaDC nota)
        {
            FAConsultafacturas.Instancia.EliminarNotaFactura(nota);
        }

        /// <summary>
        /// Método para obtener los tipos de nota
        /// </summary>
        /// <returns></returns>
        public IList<FATipoNotaDC> ObtenerTiposNota()
        {
            return FAConsultafacturas.Instancia.ObtenerTiposNota();
        }

        /// <summary>
        /// Consulta la lista de guias asociadas a una factura de cliente crédito
        /// </summary>
        /// <param name="numeroFactura"></param>
        /// <returns></returns>
        public List<FAOperacionFacturadaDC> ConsultarOperacionesFactura(long numeroFactura)
        {
            return FAConsultafacturas.Instancia.ConsultarOperacionesFactura(numeroFactura);
        }

        /// <summary>
        /// Asocia un movimento (guía crédito) a una factura específica de un cliente crédito
        /// </summary>
        /// <param name="operacionFac"></param>
        public void AsociarOperacionAFactura(FAAsociacionGuiasFacturaDC datosAsociacion)
        {
            FAConsultafacturas.Instancia.AsociarOperacionAFactura(datosAsociacion);
        }

        /// <summary>
        /// Deasociar un movimiento (guía crédito) de una factura de un cliente crédito
        /// </summary>
        /// <param name="numeroOperacion"></param>
        /// <param name="numeroFactura"></param>
        public void DesasociarOperacionAFactura(long numeroOperacion, long numeroFactura)
        {
            FAConsultafacturas.Instancia.DesasociarOperacionAFactura(numeroOperacion, numeroFactura);
        }

        #region Consultas

        /// <summary>
        /// Consultar los motivos causa de una anualción de una factura
        /// </summary>
        /// <returns></returns>
        public IEnumerable<FAMotivoAnulacionDC> ConsultarMotivosAnulacion()
        {
            return FAConsultafacturas.Instancia.ConsultarMotivosAnulacion();
        }

        /// <summary>
        /// Método para consultar responsables de facturacion
        /// </summary>
        /// <returns></returns>
        public IEnumerable<FAResponsableDC> ConsultarResponsables()
        {
            return FAConsultafacturas.Instancia.ConsultarResponsables();
        }

        /// <summary>
        /// Método de consulta de descripciones de nota credito
        /// </summary>
        /// <returns></returns>
        public IEnumerable<FADescripcionNotaDC> ConsultarDescripcionesNota()
        {
            return FAConsultafacturas.Instancia.ConsultarDescripcionesNota();
        }

        /// <summary>
        /// Método de consulta de estados de nota
        /// </summary>
        /// <returns></returns>
        public IEnumerable<FAEstadoNotaDC> ConsultarEstadosNota()
        {
            return FAConsultafacturas.Instancia.ConsultarEstadosNota();
        }

        /// <summary>
        /// Método para obtener facturas para impresión con filtro
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <returns></returns>
        public IEnumerable<FAFacturaClienteAutDC> ConsultaFacturasImpresion(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina)
        {
            return FAConsultafacturas.Instancia.ConsultaFacturasImpresion(filtro, indicePagina, registrosPorPagina);
        }

        /// <summary>
        /// Método para generar las guías internas
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        public IEnumerable<FAFacturaClienteAutDC> GenerarGuiasFacturas(IDictionary<string, string> filtro, List<long> listaNumFacturas, ARGestionDC gestionOrigen)
        {
            return FAConsultafacturas.Instancia.GenerarGuiasFacturas(filtro, listaNumFacturas, gestionOrigen);
        }

        #endregion Consultas
    }
}