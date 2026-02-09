using CO.Servidor.Servicios.ContratoDatos.Area;
using CO.Servidor.Servicios.ContratoDatos.Facturacion;
using Framework.Servidor.Excepciones;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel;

namespace CO.Servidor.Servicios.Contratos
{
    /// <summary>
    ///Contratos WCF de Facturacion
    /// </summary>
    [ServiceContract(Namespace = "http://contrologis.com")]
    public interface IFAFacturacionSvc
    {
        /// <summary>
        /// Almacena una factura nueva manual
        /// </summary>
        /// <param name="facturaManual"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        long GuardarFacturaManual(FAFacturaClienteDC facturaManual);

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
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<FAProgramacionFacturaDC> ConsultarProgramacionesFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros);

        /// <summary>
        /// Consulta las programaciones no ejecutadas de un cliente y un rango de fechas
        /// </summary>        
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<FAProgramacionFacturaDC> ConsultarProgramacionesNoEjecutadas(DateTime fechaDesde, DateTime fechaHasta, int idCliente);        

        /// <summary>
        /// Inserta una nueva exclusión asociada a una programación
        /// </summary>
        /// <param name="exclusion"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<FAExclusionProgramacionDC> ExcluirMovimientoDeProgramacion(FAExclusionProgramacionDC exclusion);

        /// <summary>
        /// Permite deshacer la exclusión de un movimiento ya excluido previamente
        /// </summary>
        /// <param name="exclusion">Exclusión que se desea deshacer</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void DeshacerExclusionDeMovimiento(FAExclusionProgramacionDC exclusion);

        /// <summary>
        /// Genera una factura de forma forzada a partir de los datos de una programación
        /// </summary>
        /// <param name="programacion"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        long GenerarFacturaProgramada(FAProgramacionFacturaDC programacion);

        /// <summary>
        /// Agrega un concepto nuevo a una programación de una factura.
        /// </summary>
        /// <param name="concepto"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        int GuardaConceptoProgramado(FAConceptoProgramadoDC concepto);

        /// <summary>
        /// Elimina un concepto de una programación
        /// </summary>
        /// <param name="concepto"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void EliminarConceptoProgramacion(FAConceptoProgramadoDC concepto);

        /// <summary>
        /// Cambia la programación de una factura a una nueva fecha
        /// </summary>
        /// <param name="nuevaProgramacion"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void CambiarProgramacionFactura(FAProgramacionFacturaDC nuevaProgramacion, DateTime nuevaFecha);

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
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<FAFacturaClienteAutDC> ConsultaFacturasFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros);

        /// <summary>
        /// Anular la factura indicada
        /// </summary>
        /// <param name="numeroFactura"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void AnularFactura(FAEstadoFacturaDC estadoFactura);

        /// <summary>
        /// Consultar los motivos causa de una anualción de una factura
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<FAMotivoAnulacionDC> ConsultarMotivosAnulacion();

        /// <summary>
        /// Permite reprogramar una factura anulada para que nuevamente sea generada
        /// </summary>
        /// <param name="FechaNueva"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ReprogramarFactura(DateTime FechaNueva, FAFacturaClienteAutDC factura);

        /// <summary>
        /// permite aprobar una factura
        /// </summary>
        /// <param name="estadoFactura"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void AprobarFactura(FAEstadoFacturaDC estadoFactura);

        /// <summary>
        /// Agrega una nueva nota crédito o débito a una factura
        /// </summary>
        /// <param name="nota"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        FANotaFacturaDC AgregarNotaFactura(FANotaFacturaDC nota);

        /// <summary>
        /// Consulta la lista de guias asociadas a una factura de cliente crédito
        /// </summary>
        /// <param name="numeroFactura"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<FAOperacionFacturadaDC> ConsultarOperacionesFactura(long numeroFactura);

        /// <summary>
        /// Asocia un movimento (guía crédito) a una factura específica de un cliente crédito
        /// </summary>
        /// <param name="operacionFac"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void AsociarOperacionAFactura(FAAsociacionGuiasFacturaDC datosAsociacion);

        /// <summary>
        /// Deasociar un movimiento (guía crédito) de una factura de un cliente crédito
        /// </summary>
        /// <param name="numeroOperacion"></param>
        /// <param name="numeroFactura"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void DesasociarOperacionAFactura(long numeroOperacion, long numeroFactura);

        /// <summary>
        /// Crea las Nvs Notas asociadas a cada Detalle de la Factura
        /// </summary>
        /// <param name="lstConceptosDtll">Lista del detalle que lleva Notas Credito o Debito</param>
        /// <returns>la lista del detalle con las inserciones Realizadas</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ObservableCollection<FAConceptoFacturaDC> AgregarNotasAFactura(ObservableCollection<FAConceptoFacturaDC> lstConceptosDtll);

        /// <summary>
        /// Agrega una nueva nota crédito o débito a una factura
        /// </summary>
        /// <param name="nota"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void EliminarNotaFactura(FANotaFacturaDC nota);

        /// <summary>
        /// Método para obtener los tipos de nota
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<FATipoNotaDC> ObtenerTiposNota();

        /// <summary>
        /// Método para consultar responsables de facturacion
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<FAResponsableDC> ConsultarResponsables();

        /// <summary>
        /// Método de consulta de descripciones de nota credito
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<FADescripcionNotaDC> ConsultarDescripcionesNota();

        /// <summary>
        /// Método de consulta de estados de nota
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<FAEstadoNotaDC> ConsultarEstadosNota();

        /// <summary>
        /// Método para obtener facturas para impresión con filtro
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<FAFacturaClienteAutDC> ConsultaFacturasImpresion(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina);

        /// <summary>
        /// Método para generar las guías internas
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<FAFacturaClienteAutDC> GenerarGuiasFacturas(IDictionary<string, string> filtro, List<long> listaNumFacturas, ARGestionDC gestionOrigen);
    }
}