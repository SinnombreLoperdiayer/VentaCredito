using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.OperacionUrbana;
using CO.Servidor.OperacionUrbana.Comun;
using CO.Servidor.OperacionUrbana.Datos;
using CO.Servidor.OperacionUrbana.PruebaEntrega;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Cajas;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CO.Servidor.OperacionUrbana
{
    /// <summary>
    /// Fachada para la operacion Urbana para interfaz con modulos del dominio
    /// </summary>
    public class OUFachadaOperacionUrbana : IOUFachadaOperacionUrbana
    {
        /// <summary>
        /// Instancia Singleton
        /// </summary>
        private static readonly OUFachadaOperacionUrbana instancia = new OUFachadaOperacionUrbana();

        #region Propiedades

        /// <summary>
        /// Retorna una instancia de la fabrica de Dominio
        /// </summary>
        public static OUFachadaOperacionUrbana Instancia
        {
            get { return OUFachadaOperacionUrbana.instancia; }
        }

        #endregion Propiedades

        /// <summary>
        /// Obtiene los nombres, id y cedula de mensajeros COL.
        /// </summary>
        /// <param name="idCentroLogistico">The id centro logistico.</param>
        /// <returns>retorna el nombre de los mensajeros que pertenecen al centro logistico</returns>
        public IEnumerable<OUNombresMensajeroDC> ObtenerNombreMensajerosCOL(long idCentroLogistico)
        {
            return OURepositorio.Instancia.ObtenerNombreMensajerosCOL(idCentroLogistico);
        }

        /// <summary>
        /// Obtene los datos del mensajero de la agencia.
        /// </summary>
        /// <param name="idAgencia">Es el id agencia.</param>
        /// <returns>la lista de mensajeros de una agencia</returns>
        public IEnumerable<OUNombresMensajeroDC> ObtenerNombreMensajeroAgencia(long idAgencia)
        {
            return OUMensajero.Instancia.ObtenerNombreMensajeroAgencia(idAgencia);
        }

        /// <summary>
        /// Obtiene los mensajeros dependientes de un centro de servicio, es decir, no solo trae los pertenecientes al centro de servicio dado, sino también
        /// de aquellos de quienes el centro de servicio pasado como parámetro es responsable
        /// </summary>
        /// <param name="idCentroServicio">Id del centro de servicio de quien se desean obtener los mensajeros</param>
        /// <returns></returns>
        public List<OUNombresMensajeroDC> ObtenerMensajerosDependientesCentroServicio(long idCentroServicio)
        {
            return OUMensajero.Instancia.ObtenerMensajerosDependientesCentroServicio(idCentroServicio);
        }

        /// <summary>
        /// Obtene los datos del mensajero de la agencia.
        /// </summary>
        /// <param name="idAgencia">Es el id agencia.</param>
        /// <returns>la lista de mensajeros de una agencia con filtro</returns>
        public IEnumerable<OUNombresMensajeroDC> ObtenerNombreMensajeroAgenciaPag(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina,
                                                                                int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros,
                                                                                long idAgencia)
        {
            return OURepositorio.Instancia.ObtenerNombreMensajeroAgenciaPag(filtro, campoOrdenamiento, indicePagina, registrosPorPagina,
                                                                   ordenamientoAscendente, out totalRegistros, idAgencia);
        }

        /// <summary>
        /// Obtiene las entregas del mensajero.
        /// </summary>
        /// <param name="idMensajero">The id mensajero.</param>
        /// <returns>Lista de Guias entregadas de alcobro  por mensajero</returns>
        public List<OUEnviosPendMensajerosDC> ObtenerEnviosPendMensajero(long idMensajero, long idComprobante)
        {
            return OURepositorio.Instancia.ObtenerEnviosPendMensajero(idMensajero, idComprobante);
        }

        /// <summary>
        /// Actualiza las guias al cobro planilladas asociadas a un mensajero como ya reportadas en caja
        /// </summary>
        /// <param name="idMensajero"></param>
        public void ActualizarAlCobrosEntregaMensajero(long idMensajero, long idComprobante, List<OUEnviosPendMensajerosDC> alcobrosDescargados)
        {
            OURepositorio.Instancia.ActualizarAlCobrosEntregaMensajero(idMensajero, idComprobante, alcobrosDescargados);
        }

        /// <summary>
        /// Retorna el último mensajero que tuvo asignada una guía dada
        /// </summary>
        /// <param name="idGuia"></param>
        /// <returns></returns>
        public OUNombresMensajeroDC ConsultarUltimoMensajeroGuia(long idGuia)
        {
            return OUAdministradorOperacionUrbana.Instancia.ConsultarUltimoMensajeroGuia(idGuia);
        }

        /// <summary>
        /// Retorna el número de la última planilla y el mensajero asociado dado el número de guía
        /// </summary>
        public OUPlanillaAsignacionMensajero ObtenerUltimaPlanillaMensajeroGuia(long numeroGuia)
        {
            return OURecogidas.Instancia.ObtenerUltimaPlanillaMensajeroGuia(numeroGuia);
        }

        /// <summary>
        /// Guardar el ingreso de una guía la centro de acopio, y realiza las validaciones necesarias asi como el
        /// envio de las fallas
        /// </summary>
        /// <param name="numeroGuia"></param>
        public OUGuiaIngresadaDC IngresoGuiaCentroAcopio(OUGuiaIngresadaDC guiaIngresada)
        {
            return OUManejadorIngreso.Instancia.GuardarIngreso(guiaIngresada);
        }

        /// <summary>
        /// Método para validar una guía suelta a centro de acopio
        /// </summary>
        /// <param name="guia"></param>
        /// <returns></returns>
        public OUPlanillaVentaGuiasDC IngresarGuiaSueltaCentroAcopio(OUPlanillaVentaGuiasDC guia, List<OUNovedadIngresoDC> listaNovedades)
        {
            return OUIngresoCentroAcopio.Instancia.IngresarGuiaSuelta(guia, listaNovedades);
        }

        /// <summary>
        /// retorna el peso maximo de la mensajeria
        /// </summary>
        /// <param name="idParametro"></param>
        /// <returns></returns>
        public string ObtenerPesoMaximoMensajeria()
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerValorParametro(OUConstantesOperacionUrbana.PARAMETRO_PESO_MAXIMO_MENSAJERIA);
        }

        /// <summary>
        /// Indica si una guía, dado su número y el id de la agencia, ya ha sido ingresada a centro de acopio pero no había sido creada la guía como tal al momento del ingreso
        /// </summary>
        /// <param name="numeroGuia"></param>.
        /// <param name="idAgencia"></param>
        /// <returns>True si la guía ya fué ingresada</returns>
        public bool GuiaYaFueIngresadaACentroDeAcopio(long numeroGuia, long idAgencia)
        {
            return OUAdministradorOperacionUrbana.Instancia.GuiaYaFueIngresadaACentroDeAcopio(numeroGuia, idAgencia);
        }

        /// <summary>
        /// Nivela a cero todas las cuentas de mensajeros asociadas a un número de factura específico
        /// </summary>
        /// <param name="numeroDocumento"></param>
        /// <param name="observaciones"></param>
        /// <param name="usuario"></param>
        public void NivelarCuentasMensajerosACeroXFactura(long noFactura, string observaciones, int idConcepto)
        {
            OURepositorio.Instancia.NivelarCuentasMensajerosACeroXFactura(noFactura, observaciones, idConcepto);
        }

        /// <summary>
        /// Indica si una guía, dado su número ya ha sido ingresada a centro de acopio pero no habiá sido creada en el sistema
        /// </summary>
        /// <param name="numeroguia"></param>
        /// <returns>Retorna el número de la agencia uqe hizo el ingreso</returns>
        public long GuiaYaFueIngresadaACentroDeAcopioRetornaCentroAcopio(long numeroGuia)
        {
            return OUAdministradorOperacionUrbana.Instancia.GuiaYaFueIngresadaACentroDeAcopioRetornaCentroAcopio(numeroGuia);
        }

        ///// <summary>
        ///// Inserta el nuevo estado de la guía
        ///// </summary>
        ///// <param name="guia"></param>
        //public void CambiarEstado(OUGuiaIngresadaDC guia, ADEnumEstadoGuia estadoNuevo)
        //{
        //    OUAdministradorEstadosGuia.Instancia.CambiarEstado(guia, estadoNuevo);
        //}

        /// <summary>
        /// Retorna la recogida peaton
        /// </summary>
        /// <param name="idSolicitud"></param>
        /// <returns></returns>
        public OURecogidasDC ObtenerRecogidaPeaton(long idSolicitud)
        {

            return OURecogidas.Instancia.ObtenerRecogidaPeaton(idSolicitud);
        }

        /// <summary>
        /// Método para actualizar una guia de la planilla de mensajero
        /// </summary>
        /// <param name="guia"></param>
        public void ActualizarGuiaMensajero(OUGuiaIngresadaDC guia)
        {
            guia.NuevoEstadoGuia = OUConstantesOperacionUrbana.ESTADO_DEVOLUCION;
            guia.EstaDescargada = true;
            OURepositorio.Instancia.ActualizarGuiaMensajero(guia);
            if (guia.EsAlCobro)
            {
                OURepositorio.Instancia.NivelarCuentasMensajerosACeroXFactura(guia.NumeroGuia.Value, "Ajuste de al cobro a mensajero por devolución del envio", (int)CAEnumConceptosCaja.TRANS_DESCUENTO_ALCOBRO_DEVUELTO);
                //GuardarTransaccionMensajero(guiaAdmision, (int)CAEnumConceptosCaja.TRANS_DESCUENTO_ALCOBRO_DEVUELTO, false, "Se descuenta al cobro a mensajero por devolución del envio", mensajero);
            }
        }


        #region Descargue de planillas

        #region Consulta

        /// <summary>
        /// Obtiene las planillas donde esta asignado un envio
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <returns></returns>
        public List<OUGuiaIngresadaDC> ObtenerPlanillasGuia(long idAdmision)
        {
            return OUPruebasEntrega.Instancia.ObtenerPlanillasGuia(idAdmision);
        }

        /// <summary>
        /// Valida si el envío está asignado a una planilla
        /// </summary>
        /// <param name="idGuia"></param>
        /// <returns></returns>
        public OUGuiaIngresadaDC ConsultaEnvioPlanillaAsignacionGuia(long idGuia)
        {
            return OUPruebasEntrega.Instancia.ConsultaEnvioPlanillaAsignacionGuia(idGuia);
        }

        /// <summary>
        /// Obtiene los mensajeros de una agencia especifica
        /// </summary>
        /// <param name="centroLogistico"></param>
        /// <returns></returns>
        public IList<OUMensajeroDC> ObtenerMensajeroDescargueAgencia(IDictionary<string, string> filtro,
          string campoOrdenamiento,
          int indicePagina,
          int registrosPorPagina,
          bool ordenamientoAscendente,
          out int totalRegistros, long puntoServicio)
        {
            return OUPruebasEntrega.Instancia.ObtenerMensajeroDescargueAgencia(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, puntoServicio);
        }

        /// <summary>
        /// Retorna el último estado de la guía dada
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <returns></returns>
        //public ADEnumEstadoGuia ObtenerUltimoEstadoGuia(long idAdmision)
        //{
        //    return OUAdministradorEstadosGuia.Instancia.ObtenerUltimoEstadoGuia(idAdmision);
        //}

        /// <summary>
        /// Método para obtener las guías pendientes de un mensajero asignado a una agencia
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns>Lista de guías</returns>
        public IList<OUGuiaIngresadaDC> ObtenerGuiasMensajero(long idMensajero)
        {
            return OUPruebasEntrega.Instancia.ObtenerGuiasMensajero(idMensajero);
        }


        /// <summary>
        /// Método para obtener las guías pendientes de un auditor asignado a un COL
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns>Lista de guías</returns>
        public IList<OUGuiaIngresadaDC> ObtenerGuiasAuditor(long idAuditor)
        {
            return OURepositorio.Instancia.ObtenerGuiasAuditor(idAuditor);
        }


        /// <summary>
        /// Método para obtener la última planilla de un mensajero
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public OUPlanillaAsignacionDC ObtenerUltimaPLanillaMensajero(long idMensajero)
        {
            return OUPruebasEntrega.Instancia.ObtenerUltimaPLanillaMensajero(idMensajero);
        }

        /// <summary>
        /// Retorn el numero de motivos diferente porque ha sido devuelta la guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns>Numero de motivos diferente porque ha sido devuelta la guia</returns>
        public OUMotivosDevolucionMensajeDC ObtenerConteoMotivosDevolucion(long? numeroGuia, string nombreMensajero, long idCol, string NombreMotivo)
        {
            return OUPruebasEntrega.Instancia.ObtenerConteoMotivosDevolucion(numeroGuia, nombreMensajero, idCol, NombreMotivo);
        }

        #endregion Consulta

        #region Adición

        /// <summary>
        /// Método para modifica o adicionar una guía de una planilla
        /// </summary>
        /// <param name="guia"></param>
        public OUEnumValidacionDescargue GuardarCambiosGuia(OUGuiaIngresadaDC guia, bool validaEntrega)
        {
            return OUPruebasEntrega.Instancia.GuardarCambiosGuia(guia, validaEntrega);
        }

        public void GuardarTransaccionMensajero(ADGuia guia, int conceptoCaja, bool esIngreso, string Observaciones, OUNombresMensajeroDC Mensajero)
        {
            OUPruebasEntrega.Instancia.GuardarTransaccionMensajero(guia, conceptoCaja, esIngreso, Observaciones, Mensajero);
        }

        /// <summary>
        /// Valida si el al cobro ya fué reportado en caja por algun mensajero
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns>Numero de Comprobante con que se reporto en caja</returns>
        public long AlCobroReportadoEnCaja(long numeroGuia)
        {
            return OURepositorio.Instancia.AlCobroReportadoEnCaja(numeroGuia);
        }

        /// <summary>
        /// Método para actualizar el estado entregado de una guia de una planilla
        /// </summary>
        /// <param name="guia"></param>
        public bool ActualizarGuiaPlanilla(long numeroGuia)
        {
            return OUPruebasEntrega.Instancia.ActualizarGuiaPlanilla(numeroGuia);
        }

        #endregion Adición

        #region Deshacer

        /// <summary>
        /// Método para deshacer la entrega exitosa de una prueba de entrega
        /// </summary>
        /// <param name="guia"></param>
        /// <returns></returns>
        public void DeshacerEntrega(OUGuiaIngresadaDC guia)
        {
            OUPruebasEntrega.Instancia.DeshacerEntrega(guia);
        }

        #endregion Deshacer

        #endregion Descargue de planillas

        /// <summary>
        /// Metodo para guardar planilla asignacion envio
        /// </summary>
        /// <param name="planilla"></param>
        /// <returns></returns>
        public OUPlanillaAsignacionDC GuardarPlanillaAsignacionEnvio(OUPlanillaAsignacionDC planilla)
        {
            return OUPlanillaAsignacionEnvios.Instancia.GuardarPlanillaAsignacionEnvio(planilla);
        }

        /// <summary>
        /// Asignar mensajero a planilla
        /// </summary>
        /// <param name="planilla"></param>
        public void AsignaMensajeroPlanilla(OUPlanillaAsignacionDC planilla)
        {
            OUAdministradorOperacionUrbana.Instancia.AsignaMensajeroPlanilla(planilla);
        }

        /// <summary>
        /// Obtiene la planilla de asignacion centro logistico
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="idCentroServicios"></param>
        /// <param name="incluyeFecha"></param>
        /// <returns></returns>
        public List<OUPlanillaAsignacionDC> ObtenerPlanillasAsignacionCentroLogistico(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, long idCentroServicios, bool incluyeFecha)
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerPlanillasAsignacionCentroLogistico(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, idCentroServicios, incluyeFecha);
        }

        /// <summary>
        /// Obtener Mensajeros Auditores
        /// </summary>
        /// <param name="puntoServicio"></param>
        /// <param name="esAgencia"></param>
        /// <returns></returns>
        public IList<OUMensajeroDC> ObtenerMensajerosAuditores(long puntoServicio, bool esAgencia)
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerMensajerosAuditores(puntoServicio, esAgencia);
        }


        /// <summary>
        /// Obtiene los datos de determinado mensajero por su numero de cedula
        /// </summary>
        /// <returns></returns>
        public OUDatosMensajeroDC ObtenerDatosMensajeroPorNumeroDeCedula(string identificacionMensajero)
        {
            return OUManejadorIngreso.Instancia.ObtenerDatosMensajeroPorNumeroDeCedula(identificacionMensajero);
        }

        /// <summary>
        /// Obtener la informacion de un mensajero por medio de su identificador
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns>Datos mensajero</returns>
        public OUDatosMensajeroDC ObtenerDatosMensajero(long idMensajero)
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerDatosMensajero(idMensajero);
        }


        /// <summary>
        /// obtiene los datos del mensajero que tiene asignada una guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public OUMensajeroDC ObtenerAsignacionMensajeroPorNumeroGuia(long numeroGuia)
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerAsignacionMensajeroPorNumeroGuia(numeroGuia);
        }

        /// <summary>
        /// Obtienen datos responsable de guia de manifiesto por número guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public OUMensajeroDC ObtenerResponsableGuiaManifiestoUrbPorNGuia(long numeroGuia)
        {
            return OURepositorio.Instancia.ObtenerResponsableGuiaManifiestoUrbPorNGuia(numeroGuia);
        }
    }
}