using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Integraciones;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace CO.Servidor.Servicios.Contratos
{
    [ServiceContract(Namespace = "http://contrologis.com")]
    public interface ILILogisticaInversaSvc
    {
        #region Manifiesto

        #region Consultar

        /// <summary>
        /// Método para consultar los tipos de manifiesto
        /// </summary>
        /// <returns>lista con los tipos de manifiesto</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<LITipoManifiestoDC> ObtenerTiposManifiesto();

        /// <summary>
        /// Metodo encargado de obtener los manifiestos asociados a una agencia
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <returns>Lista con los manifiestos filtrados</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<LIManifiestoDC> ObtenerManifiestosFiltro(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina);

        #endregion Consultar

        #region Adicionar

        /// <summary>
        /// Metodo para adicionar manifiestos
        /// </summary>
        /// <param name="manifiesto"></param>
        /// <returns>id del manifiesto generado</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        long AdicionarManifiesto(LIManifiestoDC manifiesto);

        /// <summary>
        /// Método para insertar guías en un manifiesto
        /// </summary>
        /// <param name="guia">objeto tipo guía</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void AdicionarGuiaManifiesto(LIGuiaDC guia);

        #endregion Adicionar

        #region Eliminar

        /// <summary>
        /// Elimina un manifiesto
        /// </summary>
        /// <param name="manifiesto"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void EliminarManifiesto(LIManifiestoDC manifiesto);

        /// <summary>
        /// Elimina una guia asociadad a un manifiesto
        /// </summary>
        /// <param name="guia"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void EliminarGuiaManifiesto(LIGuiaDC guia);

        #endregion Eliminar

        #endregion Manifiesto

        #region Descarga de Manifiesto

        #region Consultar

        /// <summary>
        /// Metodo encargado de obtener los manifiestos asociados a un Col destino
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<LIManifiestoDC> ObtenerManifiestosDestinoFiltro(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina);

        /// <summary>
        /// Metodo para obtener las guias por manifiesto
        /// </summary>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="idManifiesto"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<LIGuiaDC> ObtenerGuiasManifiestoDescarga(long idManifiesto);

        /// <summary>
        /// Metodo para obtener los motivos asociados a un tipo de motivo de una guía
        /// </summary>
        /// <param name="tipoMotivo">enumeracion de tipos de motivos posibles </param>
        /// <returns> lista de motivos guia</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<ADMotivoGuiaDC> ObtenerMotivosGuias(ADEnumTipoMotivoDC tipoMotivo);

        /// <summary>
        /// Método para obtener los tipos de evidencia de mensajeria
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<LITipoEvidenciaDevolucionDC> ObtenerTiposEvidencia();

        #endregion Consultar

        #region Adicionar

        /// <summary>
        /// Método para insertar guías en un manifiesto
        /// </summary>
        /// <param name="guia">objeto tipo guía</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        OUEnumValidacionDescargue GuardarCambiosGuiaAgencia(LIGuiaDC guia);

        /// <summary>
        /// Método para guardar un manifiesto manual y la guia asociada
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="manifiesto"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        LIManifiestoDC GuardarManifiestoManual(LIGuiaDC guia, LIManifiestoDC manifiesto);

        #endregion Adicionar

        #region Actualizar

        /// <summary>
        /// Método encargado de actualizar el inicio de la fecha de descarga de un manifiesto
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizarManifiesto(long idManifiesto);

        #endregion Actualizar

        #endregion Descarga de Manifiesto

        #region Digitalizacion y Archivo

        /// <summary>
        /// Válida si existe el giro y si tiene un archivo
        /// </summary>
        /// <param name="imagenes">Imágenes</param>
        /// <returns>Imágenes</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<LIArchivoGiroDC> ExisteGiroArchivo(List<LIArchivoGiroDC> imagenes);

        /// <summary>
        /// Adiciona un archivo giro
        /// </summary>
        /// <param name="imagen">Objeto imágen</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        LIArchivoGiroDC AdicionarArchivoGiro(LIArchivoGiroDC imagen);

        /// <summary>
        /// Editar un archivo giro
        /// </summary>
        /// <param name="imagen">Objeto imágen</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void EditarArchivoGiro(LIArchivoGiroDC imagen);

        /// <summary>
        /// Adicionar archivos de giro
        /// </summary>
        /// <param name="imagenes">Objeto imagenes</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<LIArchivoGiroDC> AdicionarArchivosGiro(List<LIArchivoGiroDC> imagenes);

        /// <summary>
        /// Valida si se puede digitalizar la guía de acuerdo al estado
        /// </summary>
        /// <param name="imagenes">Colección de imágenes</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GuardarGuiasCorrectas(List<LIArchivoGuiaMensajeriaDC> imagenes, bool entregaExitosa);

        /// <summary>
        /// Adiciona un archivo guía mensajería
        /// </summary>
        /// <param name="entregaExitosa"></param>
        /// <param name="imagen"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void AdicionarArchivoGuiaMensajeria(bool entregaExitosa, LIArchivoGuiaMensajeriaDC imagen);

        /// <summary>
        /// Edita un archivo guía de mensajería
        /// </summary>
        /// <param name="imagen">Objeto imágen</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void EditarArchivoGuiaMensajeria(LIArchivoGuiaMensajeriaDC imagen);

        /// <summary>
        /// Válida si existe el giro y si tiene un archivo
        /// </summary>
        /// <param name="imagenes">Imágenes</param>
        /// <returns>Imágenes</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<LIArchivoComprobantePagoDC> ExisteComprobanteGiroArchivo(List<LIArchivoComprobantePagoDC> imagenes);

        /// <summary>
        /// Adiciona un archivo giro
        /// </summary>
        /// <param name="imagen">Objeto imágen</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        LIArchivoComprobantePagoDC AdicionarComprobanteArchivoGiro(LIArchivoComprobantePagoDC imagen);

        /// <summary>
        /// Editar un archivo giro
        /// </summary>
        /// <param name="imagen">Objeto imágen</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void EditarComprobanteArchivoGiro(LIArchivoComprobantePagoDC imagen);

        /// <summary>
        /// Adicionar comprobantes de pago de gito
        /// </summary>
        /// <param name="imagenes">Objeto imagenes</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<LIArchivoComprobantePagoDC> AdicionarComprobanteArchivosGiro(List<LIArchivoComprobantePagoDC> imagenes);

        /// <summary>
        /// Archiva una guía
        /// </summary>
        /// <param name="guia">Objeto guía</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        LIArchivoGuiaMensajeriaDC GuardarArchivoGuiaMensajeria(LIArchivoGuiaMensajeriaDC guia);


        /// <summary>
        /// Método para obtener un archivo guia
        /// </summary>
        /// <param name="imagen"></param>
        LIArchivoGuiaMensajeriaDC ObtenerArchivoGuiaFS(LIArchivoGuiaMensajeriaDC archivoGuia);

        /// <summary>
        /// Obtiene las guías archivadas
        /// </summary>
        /// <returns>Colección guías archivadas</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<LIArchivoGuiaMensajeriaDC> ObtenerArchivoGuia(long idCol);

        /// <summary>
        /// Método para validar archivos de guías, para clasificarlas de acuerdo a su posible resultado
        /// </summary>
        /// <param name="entregaExitosa"></param>
        /// <param name="imagenes"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<LIArchivoGuiaMensajeriaDC> ValidarArchivosGuias(bool entregaExitosa, List<LIArchivoGuiaMensajeriaDC> imagenes);

        /// <summary>
        /// Método para validar archivos de guías, para clasificarlas de acuerdo a su posible resultado
        /// </summary>
        /// <param name="entregaExitosa"></param>
        /// <param name="imagenes"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<LIEvidenciaDevolucionDC> ValidarArchivosVolantes(List<LIEvidenciaDevolucionDC> imagenes);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<LIEvidenciaDevolucionDC> ValidarArchivosVolantesWPF(List<LIEvidenciaDevolucionDC> imagenes);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        LIEvidenciaDevolucionDC AsociarNumeroGuiaAVolante(LIEvidenciaDevolucionDC imagenVolante);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<LIEvidenciaDevolucionDC> ObtenerEvidenciaDevolucionxGuia(long NumeroGuia);

        /// <summary>
        /// Guarda un archivo de un volante
        /// </summary>
        /// <param name="imagenes">Colección de imágenes</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GuardarVolantesCorrectos(List<LIEvidenciaDevolucionDC> imagenes);

        /// <summary>
        /// Edita un archivo de un volante
        /// </summary>
        /// <param name="imagen">Objeto imágen</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void EditarArchivoVolante(LIArchivosDC imagen);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ADGuia ObtenerFechaEstimadaEntregaGuia(long numeroguia);



        /// <summary>
        /// Metodo que obtiene los datos de archivo guía de mensajería    
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        LIArchivoGuiaMensajeriaDC ObtenerArchivoGuiaxNumeroGuia(long numeroGuia);


        #endregion Digitalizacion y Archivo

        #region Descargue de planillas

        #region Consulta

        /// <summary>
        /// Obtiene los mensajeros de una agencia especifica
        /// </summary>
        /// <param name="centroLogistico"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<OUMensajeroDC> ObtenerMensajeroDescargueAgencia(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, long puntoServicio);

        /// <summary>
        /// Método para obtener las guías pendientes de un mensajero asignado a una agencia
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns>Lista de guías</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<OUGuiaIngresadaDC> ObtenerGuiasMensajero(long idMensajero);


        /// <summary>
        /// Método para obtener las guías pendientes de un auditor asignado a una COL
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns>Lista de guías</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<OUGuiaIngresadaDC> ObtenerGuiasAuditor(long idAuditor);

        /// <summary>
        /// Método para obtener las guías pendientes del col asignadas a logistica invers
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns>Lista de guías</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<OUGuiaIngresadaDC> ObtenerGuiasCol(long idCol);



        /// <summary>
        /// Método para obtener la última planilla de un mensajero
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        OUPlanillaAsignacionDC ObtenerUltimaPLanillaMensajero(long idMensajer);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        OUMotivosDevolucionMensajeDC ObtenerConteoMotivosDevolucion(long? numeroGuia, string nombreMensajero, long idCol, string NombreMotivo);


        /// <summary>
        /// Obtiene los mensajeros que han estadpos asignados a un planilla de la cual se descargo una guia como no entregada
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        /// 
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<LIMensajeroResponsableDC> ObtenerMensajerosResponsablesDescargue(long numeroGuia);

        #endregion Consulta

        #region Adición

        /// <summary>
        /// Método para modifica o adicionar una guía de una planilla
        /// </summary>
        /// <param name="guia"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        OUEnumValidacionDescargue GuardarCambiosGuia(OUGuiaIngresadaDC guia, bool validaEntrega);

        #endregion Adición

        #region Deshacer

        /// <summary>
        /// Método para deshacer la entrega exitosa de una prueba de entrega
        /// </summary>
        /// <param name="guia"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void DeshacerEntrega(OUGuiaIngresadaDC guia);

        #endregion Deshacer

        #endregion Descargue de planillas

        #region Certificacion de Notificaciones

        #region Consultas

        /// <summary>
        /// Retorna el archivo digitalizado de una guía
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ADArchivoAlmacenGuia ObtenerArchivoAlmacenGuia(long numeroGuia);

        /// <summary>
        /// Retorna la información de una guía dada su forma de pago, en un rango de fechas de admisión, que pertenezcan al cliente dado y al RACOL dado, que
        /// sean del servicio de notificaciones, tipo de envío certificación, que estén descargadas como entrega correcta, que no tengan capturado los datos de
        /// recibido y estén digitalizadas
        /// </summary>
        /// <param name="idFormaPago">Forma de pago</param>
        /// <param name="fechaInicio">Fecha Inicial</param>
        /// <param name="fechaFin">Fecha Final</param>
        /// <param name="idCliente">Id del Cliente</param>
        /// <param name="idRacol">Id del Racol</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ADGuia> ObtenerGuiasParaCapturaAutomatica(short idFormaPago, System.DateTime fechaInicio, System.DateTime fechaFin, int? idCliente, long idRacol);

        /// <summary>
        /// Retorna información de guía que para ser recibida en forma manual en la parte de notificaciones.
        /// La guía debe estar en estado "Devolución" o "Entrega" y la prueba de entrega o de devolución
        /// correspondiente debe estar digitalizada en la aplicación
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ADGuia ObtenerGuiaParaRecibirManualNotificaciones(long numeroGuia);

        /// <summary>
        /// Método para obtener las guías de notificaciones
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ADNotificacion> ObtenerNotificacionesRecibido(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina);

        /// <summary>
        /// Método para generar las guías internas de las notificaciones
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        int GenerarGuiasInternasNotificacion(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina);

        /// <summary>
        /// Método para obtener los id de las guías de notificaciones
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<long> ObtenerIdNotificaciones(IDictionary<string, string> filtro);

        #endregion Consultas

        #region Grabación

        /// <summary>
        /// Registra recibido de guía manual
        /// </summary>
        /// <param name="recibido"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void RegistrarRecibidoGuiaManual(LIRecibidoGuia recibido);

        #endregion Grabación

        #endregion Certificacion de Notificaciones

        #region Planilla Certificacion

        /// <summary>
        /// Obtener planillas de certificacion
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
        GenericoConsultasFramework<LIPlanillaCertificacionesDC> ObtenerPlanillasCertificaciones(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, bool esDevolucion);

        /// <summary>
        /// Obtener planillas de certificacion con ado
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="esDevolucion"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GenericoConsultasFramework<LIPlanillaCertificacionesDC> ObtenerPlanillasCertificacionesAdo(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, bool esDevolucion);


        /// <summary>
        /// Método para obtener las guias de una planilla de certificacion
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ADNotificacion> ObtenerGuiasPlanillaCertificacion(long idPlanilla);

        /// <summary>
        /// Guarda los envios de la planilla de asignacion
        /// </summary>
        /// <param name="planilla"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GuardarLstEnvioPlanillaCertificacion(LIPlanillaCertificacionesDC planilla);

        /// <summary>
        /// Guarda guia en la planilla de certificacion
        /// </summary>
        /// <param name="planilla"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        LIPlanillaCertificacionesDC GuardarGuiaPlanillaCertificacion(LIPlanillaCertificacionesDC planilla);

        /// <summary>
        /// Elimina una guia de la planilla de certificaciones
        /// </summary>
        /// <param name="guiaCertificacion"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ADNotificacion EliminarGuiaPlanillaCertificaciones(ADNotificacion guiaCertificacion);

        /// <summary>
        /// Valida que la guia ingresada cumpla con las condiciones para la certificacion
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ADNotificacion ValidarGuiaCertificacionNotificacion(long numeroGuia);

        /// <summary>
        /// Obtiene las notificaciones del col, con los filtros seleccionados que esten sin planillar
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="idCol"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ADNotificacion> ObtenerNotificacionesFiltroSinPla(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina, long idCol);

        /// <summary>
        /// Retorna las guias internas de las planillas
        /// </summary>
        /// <param name="planillas"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ADGuiaInternaDC> ObtenerGuiasInternasPlanilla(string planillas, bool esPlanilla, long idCol);

        /// <summary>
        /// Descarga la planilla de certificacion
        /// </summary>
        /// <param name="idPlanilla"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void DescargarPlanillaCertificaciones(long idPlanilla);

        /// <summary>
        /// Método que adiciona una planilla de certificación
        /// </summary>
        /// <param name="planilla"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        LIPlanillaCertificacionesDC AdicionarPlanilla(LIPlanillaCertificacionesDC planilla);

        /// <summary>
        /// Método para adicionar una guía a una planilla
        /// </summary>
        /// <param name="guia"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ADNotificacion AdicionarGuiaPlanilla(ADNotificacion guia);

        /// <summary>
        /// Método para cerrar una planilla de notificaciones
        /// </summary>
        /// <param name="numeroPlanilla"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void CerrarPlanillaNotificaciones(long numeroPlanilla);

        /// <summary>
        /// Método para generar las guias de la planilla de entrega
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void AdicionarGuiasPlanillaEntrega(LIPlanillaCertificacionesDC planilla);

        #endregion Planilla Certificacion

        #region GeneracionImagenes

        /// <summary>
        /// Obtiene los numeros de guias y la ruta
        /// </summary>
        /// <returns>Colección numeros de guia y rutas</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<LIArchivoGuiaMensajeriaDC> ObtenerGuiasRuta(string imagenesGenerar, int idCliente, string idCiudad, int idSucursal, DateTime fechaAdmisionInical, DateTime fechaAdmisionFinal, long guiaFacturaInicial, long guiaFacturaFinal, long ordenCompraInicial, long ordenCompraFinal);

        #endregion GeneracionImagenes

        #region Descargue de Devoluciones

        /// <summary>
        /// Método para obtener la informacion de una guia en el descargue de devoluciones por mensajero
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        OUGuiaIngresadaDC ObtenerInfoGuiaDevolucion(long numeroGuia, long idMensajero);


        /// <summary>
        /// Método para descargar las devoluciones de una guia mensajero
        /// </summary>
        /// <param name="guia"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        LIDescargueGuiaDC GuardarDevolucion(OUGuiaIngresadaDC guia);


        /// <summary>
        /// Método para descargar las devoluciones de una guia mensajero
        /// </summary>
        /// <param name="guia"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        LIDescargueGuiaDC GuardarDevolucionAgencia(OUGuiaIngresadaDC guia);

        /// <summary>
        /// Registro de una entrega desde la agencia
        /// </summary>
        /// <param name="guia"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        LIDescargueGuiaDC DescargueEntregaAgencia(OUGuiaIngresadaDC guia);

        /// <summary>
        /// Guarda las devoluciones de auditoria
        /// </summary>
        /// <param name="guia"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        LIDescargueGuiaDC GuardarDevolucionAuditoria(OUGuiaIngresadaDC guia);


        /// <summary>
        /// Guarda las devoluciones de auditoria
        /// </summary>
        /// <param name="guia"></param>
        /// <returns></returns>
        /// 
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        LIDescargueGuiaDC GuardarDevolucionCol(OUGuiaIngresadaDC guia);


        #endregion Descargue de Devoluciones





        /// <summary>
        /// Método para ingresa  una guia a reclame en oficina
        /// </summary>        
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        LIReclameEnOficinaDC AdicionarReclameEnOficina(LIReclameEnOficinaDC reclameOficina);

        /// <summary>
        /// Metodo para consultar los envios reclame en oficina por punto y tipoMovimiento
        /// </summary>
        /// <param name="ReclameEnOficina"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<LIReclameEnOficinaDC> ConsultarGuiasReclameEnOficina(long idCentroServicio, int idTipoMovimiento, bool filtroDevolucion);

        /// <summary>
        /// consulta las guias reclame en oficina utilizando filtros
        /// </summary>
        /// <param name="ReclameEnOficina"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<LIReclameEnOficinaDC> ConsultarGuiasReclameEnOficinaFiltros(long idCentroServicio, Dictionary<string, string> filtros);


        /// <summary>
        /// Metodo para consultar los totales de los  envios Asignados, Ingresados y para Devolucion de reclame en oficina por punto
        /// </summary>
        /// <param name="ReclameEnOficina"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        Dictionary<string, int> ConsultarContadoresGuiasReclameEnOficina(long idCentroServicio);

        /// <summary>
        /// Método ejecutar accion (entrega o devolucion) reclame en oficina
        /// </summary>        
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        LIReclameEnOficinaDC DevolucionReclameOficina(LIReclameEnOficinaDC reclameOficina);

        /// <summary>
        /// Método ejecutar accion (entrega o devolucion) reclame en oficina
        /// </summary>        
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        LIReclameEnOficinaDC EntregaReclameOficina(LIReclameEnOficinaDC reclameOficina);



        /// <summary>
        /// Obtiene Información de Telemercadeo realizado a la Guia seleccionada
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<LIGestionesDC> ObtenerInformacionTelemercadeoGuia(long numeroGuia);


        /// <summary>
        /// Retorna el(los) Volantes de una guia
        /// Realizado: Mauricio Sanchez 20160208
        /// </summary>
        /// <param name="numeroGuia">Identificador admisión guia</param>
        /// <returns>Archivo</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<string> ObtenerVolantesGuia(long numeroGuia);

        /// <summary>
        /// Obtiene las guias que se encuentran en bodega por asignar 
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        OUGuiaIngresadaDC AsignarGuiaCentroAcopio(long numeroGuia, long idCentroServicio);

        /// <summary>
        /// Adiciona la guia a la planilla, auditores en terreno
        /// </summary>
        /// <param name="planilla"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        OUPlanillaAsignacionDC GestionarPlanillaAuditor(OUPlanillaAsignacionDC planilla);

        /// <summary>
        /// Asigna un mensajero a la planilla
        /// </summary>
        /// <param name="planilla"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void AsignaMensajeroPlanilla(OUPlanillaAsignacionDC planilla);

        /// <summary>
        /// Obtiene guia para asignar a una planilla de auditoria
        /// </summary>
        /// <param name="NumeroGuia"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        OUGuiaIngresadaDC ObtenerGuiaBodegaPorAsignarAuditor(long NumeroGuia);

        /// <summary>
        /// Verifica si una guia tiene almenos una tapa por tipo tapa
        /// </summary>
        /// <param name="tapaLogistica"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        bool VerificarGuiaConTapa(LITapaLogisticaDC tapaLogistica);

        /// <summary>
        /// Verifica si un numero de guia esta en la tabla gestionAuditoria para imprimir la tapa
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        bool VerificaGuiaConGestionAuditor(long numeroGuia);


        /// <summary>
        /// Metodo para obtener las planilas asignadas al centro logistico
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="idCentroServicios"></param>
        /// <param name="incluyeFecha"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<OUPlanillaAsignacionDC> ObtenerPlanillasAsignacionCentroLogistico(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, long idCentroServicios, bool incluyeFecha);

        /// <summary>
        /// Obtener mensajero auditores
        /// </summary>
        /// <param name="puntoServicio"></param>
        /// <param name="esAgencia"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<OUMensajeroDC> ObtenerMensajerosAuditores(long puntoServicio, bool esAgencia);

        /// <summary>
        /// Descargue controller app Mensajero
        /// </summary>
        /// <param name="descargue"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        LIEstadoDescargueControllerAppDC DescargueMensajeroControllerApp(LIDescargueControllerAppDC descargue);

        /// <summary>
        /// Metodo para devoluciones mensajero 
        /// </summary>
        /// <param name="descargue"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        LIEstadoDescargueControllerAppDC DevolucionMensajeroControllerApp(LIDescargueControllerAppDC descargue);

        /// <summary>
        /// Metodo para descargue Auditor 
        /// </summary>
        /// <param name="descargue"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        LIEstadoDescargueControllerAppDC DescargueAuditorControllerApp(LIDescargueControllerAppDC descargue);


        /// <summary>
        /// Metodo para la devolucion ratificada del audotir
        /// </summary>
        /// <param name="devolucion"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        LIEstadoDescargueControllerAppDC DevolucionRatificadaAuditorControllerApp(LIDescargueControllerAppDC devolucion);

        /// <summary>
        /// Metodo para descargue entregas maestras auditor controller app
        /// </summary>
        /// <param name="descargue"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        LIEstadoDescargueControllerAppDC DescargueEMAuditorControllerApp(LIDescargueControllerAppDC descargue);

        #region Recibido Guia

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<LIRecibidoGuia> ObtenerRecibidosPendientes(long idCol, DateTime fechaInicial, DateTime fechaFinal);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<LIRecibidoGuia> ObtenerRecibidosPendientesApp(long idCol, DateTime fechaInicial, DateTime fechaFinal, LIEnumOrigenAplicacion idAplicacionOrigen);
        #endregion

        #region Certificaciones Web

        /// <summary>
        /// Valida el Tipo de Certificacion
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="idRemitente"></param>
        /// <param name="idDestinatario"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ADNotificacion ValidarCertificacionWeb(LIReimpresionCertificacionDC reimprecionCertif);


        /// <summary>
        /// Método para validar si la Certificación ya se ha impreso con anterioridad
        /// </summary>
        /// <param name="numeroguia"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        long ValidarNotificacionExisteAuditoria(long numeroguia);
       
       
        /// <summary>
        /// Método que actualiza el campo está devuelta en la tabla Admision Notificaciones
        /// </summary>
        /// <param name="numeroguia"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizaEstaDevueltaADMNotif(long numeroguia);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ADNotificacion ObtenerDatosFacturaReImpresionNotificacion(long numGuia);

        /// <summary>
        /// Realiza la afectación a caja para las Reimpresiones
        /// </summary>
        /// <param name="afectacionCajaReimp"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void AfectacionCajaCertificacion(ADNotificacion afectacionCajaReimp);


        /// <summary>
        /// Audita las guias reimpresas
        /// </summary>
        /// <param name="afectacionCajaReimp"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void InsertarAuditoriaReimpresion(LIReimpresionCertificacionDC auditoriaReimpresion);

        #endregion


        #region Agencias

        /// <summary>
        /// Método para obtener los pendientes de la agencia
        /// </summary>
        /// <param name="idAgencia"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<LIDescargueGuiaAgenciaDC> ObtenerPendientesAgencia(long idAgencia);

         


        #endregion


        #region Digitalizacion de agencias



        /// <summary>
        /// Método para validar archivos de guías, para clasificarlas de acuerdo a su posible resultado
        /// </summary>
        /// <param name="entregaExitosa"></param>
        /// <param name="imagenes"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<LIArchivoGuiaMensajeriaDC> ValidarArchivosAgencias(List<LIArchivoGuiaMensajeriaDC> imagenes);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entregaExitosa"></param>
        /// <param name="imagenes"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        LIArchivoGuiaMensajeriaDC ArchivarGuiaPruebaEntregaWPF(LIArchivoGuiaMensajeriaDC guia);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        InformacionECAPTURE ValidarInformacionECapture(ADGuia guia);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        bool InsertarLecturaEcaptureArchivoPruebaEntrega(INTArchivoPruebaEntrega archivoPruebaEntrega);

        #endregion

        #region Fallas Interlogis / Web
        /// <summary>
        /// Obtener parametros por integracion 
        /// </summary>
        /// <param name="tipoParametro"></param>
        /// <returns></returns>
        [System.Obsolete()]
        List<LIParametrizacionIntegracionRAPSDC> ObtenerParametrosPorIntegracion(string tipoParametro);

        #endregion

    }
}