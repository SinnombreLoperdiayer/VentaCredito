using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using System;
using System.Collections.Generic;

namespace CO.Servidor.Dominio.Comun.OperacionUrbana
{
    /// <summary>
    /// Interfaz para acceso a la fachada de Operacion Urbana
    /// </summary>
    public interface IOUFachadaOperacionUrbana
    {
        /// <summary>
        /// Obtiene los nombres, id y cedula de mensajeros COL.
        /// </summary>
        /// <param name="idCentroLogistico">The id centro logistico.</param>
        /// <returns>retorna el nombre de los mensajeros que pertenecen al centro logistico</returns>
        IEnumerable<OUNombresMensajeroDC> ObtenerNombreMensajerosCOL(long idCentroLogistico);

        /// <summary>
        /// Obtene los datos del mensajero de la agencia.
        /// </summary>
        /// <param name="idAgencia">Es el id agencia.</param>
        /// <returns>la lista de mensajeros de una agencia</returns>
        IEnumerable<OUNombresMensajeroDC> ObtenerNombreMensajeroAgencia(long idAgencia);

        /// <summary>
        /// Nivela a cero todas las cuentas de mensajeros asociadas a un número de factura específico
        /// </summary>
        /// <param name="numeroDocumento"></param>
        /// <param name="observaciones"></param>
        /// <param name="usuario"></param>
        void NivelarCuentasMensajerosACeroXFactura(long noFactura, string observaciones, int idConcepto);

        /// <summary>
        /// Obtiene los mensajeros dependientes de un centro de servicio, es decir, no solo trae los pertenecientes al centro de servicio dado, sino también
        /// de aquellos de quienes el centro de servicio pasado como parámetro es responsable
        /// </summary>
        /// <param name="idCentroServicio">Id del centro de servicio de quien se desean obtener los mensajeros</param>
        /// <returns></returns>
        List<OUNombresMensajeroDC> ObtenerMensajerosDependientesCentroServicio(long idCentroServicio);

        /// <summary>
        /// Obtene los datos del mensajero de la agencia.
        /// </summary>
        /// <param name="idAgencia">Es el id agencia.</param>
        /// <returns>la lista de mensajeros de una agencia con filtro</returns>
        IEnumerable<OUNombresMensajeroDC> ObtenerNombreMensajeroAgenciaPag(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina,
                                                                                int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros,
                                                                                long idAgencia);

        /// <summary>
        /// Obtiene las entregas del mensajero.
        /// </summary>
        /// <param name="idMensajero">The id mensajero.</param>
        /// <returns>Lista de Guias entregadas de alcobro  por mensajero</returns>
        List<OUEnviosPendMensajerosDC> ObtenerEnviosPendMensajero(long idMensajero, long idComprobante);

        /// <summary>
        /// Actualiza las guias al cobro planilladas asociadas a un mensajero como ya reportadas en caja
        /// </summary>
        /// <param name="idMensajero"></param>
        void ActualizarAlCobrosEntregaMensajero(long idMensajero, long idComprobante, List<OUEnviosPendMensajerosDC> alcobrosDescargados);

        /// <summary>
        /// Retorna el último mensajero que tuvo asignada una guía dada
        /// </summary>
        /// <param name="idGuia"></param>
        /// <returns></returns>
        OUNombresMensajeroDC ConsultarUltimoMensajeroGuia(long idGuia);

        /// <summary>
        /// Retorna el número de la última planilla y el mensajero asociado dado el número de guía
        /// </summary>
        OUPlanillaAsignacionMensajero ObtenerUltimaPlanillaMensajeroGuia(long numeroGuia);

        /// <summary>
        /// Guardar el ingreso de una guía la centro de acopio, y realiza las validaciones necesarias asi como el
        /// envio de las fallas
        /// </summary>
        /// <param name="numeroGuia"></param>
        OUGuiaIngresadaDC IngresoGuiaCentroAcopio(OUGuiaIngresadaDC guiaIngresada);

        /// <summary>
        /// retorna el peso maximo de la mensajeria
        /// </summary>
        /// <param name="idParametro"></param>
        /// <returns></returns>
        string ObtenerPesoMaximoMensajeria();

        /// <summary>
        /// Indica si una guía, dado su número y el id de la agencia, ya ha sido ingresada a centro de acopio pero no había sido creada la guía como tal al momento del ingreso
        /// </summary>
        /// <param name="numeroGuia"></param>.
        /// <param name="idAgencia"></param>
        /// <returns>True si la guía ya fué ingresada</returns>
        bool GuiaYaFueIngresadaACentroDeAcopio(long numeroGuia, long idAgencia);

        /// <summary>
        /// Indica si una guía, dado su número ya ha sido ingresada a centro de acopio pero no habiá sido creada en el sistema
        /// </summary>
        /// <param name="numeroguia"></param>
        /// <returns>Retorna el número de la agencia uqe hizo el ingreso</returns>
        long GuiaYaFueIngresadaACentroDeAcopioRetornaCentroAcopio(long numeroGuia);

        /// <summary>
        /// Obtiene las planillas donde esta asignado un envio
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <returns></returns>
        List<OUGuiaIngresadaDC> ObtenerPlanillasGuia(long idAdmision);

        /// <summary>
        /// Inserta el nuevo estado de la guía
        /// </summary>
        /// <param name="guia"></param>
        // void CambiarEstado(OUGuiaIngresadaDC guia, ADEnumEstadoGuia estadoNuevo);

        /// <summary>
        /// Método para validar una guía suelta a centro de acopio
        /// </summary>
        /// <param name="guia"></param>
        /// <returns></returns>
        OUPlanillaVentaGuiasDC IngresarGuiaSueltaCentroAcopio(OUPlanillaVentaGuiasDC guia, List<OUNovedadIngresoDC> listaNovedades);

        /// <summary>
        /// Valida si el al cobro ya fué reportado en caja por algun mensajero
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns>Numero de Comprobante con que se reporto en caja</returns>
        long AlCobroReportadoEnCaja(long numeroGuia);

        /// <summary>
        /// Retorna la recogida peaton
        /// </summary>
        /// <param name="idSolicitud"></param>
        /// <returns></returns>
        OURecogidasDC ObtenerRecogidaPeaton(long idSolicitud);

        void ActualizarGuiaMensajero(OUGuiaIngresadaDC guia);

        /// <summary>
        /// Metodo para guardar planilla asignacion envio
        /// </summary>
        /// <param name="planilla"></param>
        /// <returns></returns>
        OUPlanillaAsignacionDC GuardarPlanillaAsignacionEnvio(OUPlanillaAsignacionDC planilla);

        /// <summary>
        /// Asignar un mensajero a la planilla
        /// </summary>
        /// <param name="planilla"></param>
        void AsignaMensajeroPlanilla(OUPlanillaAsignacionDC planilla);


        /// <summary>
        /// Metodo para obtener la planilla de centro logistico
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="idCentroServicios"></param>
        /// <param name="incluyeFecha"></param>
        /// <returns></returns>
        List<OUPlanillaAsignacionDC> ObtenerPlanillasAsignacionCentroLogistico(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, long idCentroServicios, bool incluyeFecha);

        /// <summary>
        /// Metodo para obtener Mensajeros Auditores
        /// </summary>
        /// <param name="puntoServicio"></param>
        /// <param name="esAgencia"></param>
        /// <returns></returns>
        IList<OUMensajeroDC> ObtenerMensajerosAuditores(long puntoServicio, bool esAgencia);

        OUDatosMensajeroDC ObtenerDatosMensajeroPorNumeroDeCedula(string identificacionMensajero);

        OUDatosMensajeroDC ObtenerDatosMensajero(long idMensajero);

        #region Descargue de planillas

        #region Consulta

        /// <summary>
        /// Obtiene los mensajeros de una agencia especifica
        /// </summary>
        /// <param name="centroLogistico"></param>
        /// <returns></returns>
        IList<OUMensajeroDC> ObtenerMensajeroDescargueAgencia(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long puntoServicio);

        /// <summary>
        /// Método para obtener las guías pendientes de un mensajero asignado a una agencia
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns>Lista de guías</returns>
        IList<OUGuiaIngresadaDC> ObtenerGuiasMensajero(long idMensajero);


        /// <summary>
        /// Método para obtener las guías pendientes de un auditor asignado a un COL
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns>Lista de guías</returns>
        IList<OUGuiaIngresadaDC> ObtenerGuiasAuditor(long idAuditor);


        /// <summary>
        /// Método para obtener la última planilla de un mensajero
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        OUPlanillaAsignacionDC ObtenerUltimaPLanillaMensajero(long idMensajero);

        /// <summary>
        /// Retorna el numero de motivos diferente porque ha sido devuelta la guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns>numero de motivos diferente porque ha sido devuelta la guia</returns>
        OUMotivosDevolucionMensajeDC ObtenerConteoMotivosDevolucion(long? numeroGuia, string nombreMensajero, long idCol, string NombreMotivo);

        #endregion Consulta

        #region Adición

        /// <summary>
        /// Método para modifica o adicionar una guía de una planilla
        /// </summary>
        /// <param name="guia"></param>
        OUEnumValidacionDescargue GuardarCambiosGuia(OUGuiaIngresadaDC guia, bool validaEntrega);

        /// <summary>
        /// almacena una operación en la caja del mensajero
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="conceptoCaja"></param>
        /// <param name="esIngreso"></param>
        /// <param name="Observaciones"></param>
        /// <param name="Mensajero"></param>
        void GuardarTransaccionMensajero(ADGuia guia, int conceptoCaja, bool esIngreso, string Observaciones, OUNombresMensajeroDC Mensajero);

        /// <summary>
        /// Método para actualizar el estado entregado de una guia de una planilla
        /// </summary>
        /// <param name="guia"></param>
        bool ActualizarGuiaPlanilla(long numeroGuia);

        #endregion Adición

        #region Deshacer

        /// <summary>
        /// Método para deshacer la entrega exitosa de una prueba de entrega
        /// </summary>
        /// <param name="guia"></param>
        /// <returns></returns>
        void DeshacerEntrega(OUGuiaIngresadaDC guia);

        /// <summary>
        /// obtiene los datos del mensajero que tiene asignada una guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        OUMensajeroDC ObtenerAsignacionMensajeroPorNumeroGuia(long numeroGuia);

        /// <summary>
        /// Obtienen datos responsable de guia de manifiesto por número guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        OUMensajeroDC ObtenerResponsableGuiaManifiestoUrbPorNGuia(long numeroGuia);
        
        #endregion Deshacer

        #endregion Descargue de planillas
    }
}