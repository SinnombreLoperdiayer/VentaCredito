using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using Framework.Servidor.Excepciones;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace CO.Servidor.Servicios.Contratos
{
    [ServiceContract(Namespace = "http://contrologis.com")]
    public interface ILILogisticaInversaTelemercadeoSvc
    {
        #region Telemercadeo

        #region consultas

        /// <summary>
        /// Retorna las guías asociadas a la localidad pasada en el estado solicitado
        /// </summary>
        /// <param name="idEstado"></param>
        /// <param name="localidad"></param>
        /// <param name="numeroPagina"></param>
        /// <param name="tamanoPagina"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        LIGestionesGuiaDC ObtenerGestionesGuias(ADEnumEstadoGuia estado, long idCentroServicio, long numeroGuia, bool esCol);

        /// <summary>
        /// Método para obtener los archivos de evidencia de una guia en telemercadeo
        /// </summary>
        /// <param name="IdEstadoGuiaLog"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<LIEvidenciaDevolucionDC> ObtenerArchivosEvidenciaDevolucion(long IdEstadoGuiaLog);

        /// <summary>
        /// Método para obtener el detalle de una guia con el número de gestiones
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="idEstado"></param>
        /// <param name="localidad"></param>
        /// <returns></returns>
        //[OperationContract]
        //[FaultContract(typeof(ControllerException))]
        //LIGestionesGuiaDC ObtenerGuiaGestiones(long numeroGuia, short idEstado, string localidad);

        /// <summary>
        /// Método encargado de obtener gestiones de una guía
        /// </summary>
        /// <param name="idTrazaguia"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<LIGestionesDC> ObtenerGestionesGuia(long idTrazaguia);

        /// <summary>
        /// Método para obtener los resultados posibles de una gestión de telemercadeo
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<LIResultadoTelemercadeoDC> ObtenerResultados();

        /// <summary>
        /// Método encargado de obtener la información de la guía en admisión
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns>guía de admision</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ADGuia ObtenerInfoAdmision(long numeroGuia);

        /// <summary>
        /// Método para obtener los posibles estados de transición
        /// </summary>
        /// <param name="estadoGuia"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<ADEnumEstadoGuia> ObtenerEstados(ADEnumEstadoGuia estadoGuia);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ADEstadoGuia> ObtenerEstadosParaDevolver();


        /// <summary>
        /// Método para obtener los posibles estados de transición
        /// </summary>
        /// <param name="estadoGuia"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<LIGestionMotivoBorradoDC> ObtenerMotivosBorrado();

        /// <summary>
        /// Retorna el stream de un archivo de envidencia de devolución dado su id
        /// </summary>
        /// <param name="idArchivo"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        string ObtenerArchivoEvidenciaAdjunto(long idArchivo);

        /// <summary>
        /// Retorna lista de registros de las gestiones de la guia 
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns>Lista con el flujo de la guia</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<LIFlujoGuiaDC> ObtenerFlujoGuia(long numeroGuia);

        /// <summary>
        /// Consulta la informacion del flujo de la guia para telemercadeo
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        LIAdmisionGuiaFlujoDC ObtenerAdmisionGuiaFlujo(long numeroGuia);

        /// <summary>
        /// Consulta la informacion de los ingresos a centro de acopio nacional
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<LIIngresoCentroAcopioNacionalDC> ObtenerIngresoAcopioNacional(long numeroGuia);

        /// <summary>
        /// Consulta la informacion de los ingresos a centro de acopio urbano
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns>lista</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<LIIngresoCentroAcopioUrbanoDC> ObtenerIngresoCentroAcopioUrbano(long numeroGuia);

        /// <summary>
        /// Consulta de asignaciones a mensajero para telemercadeo
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns>lista</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<LIAsignacionMensajero> ObtenerAsignacionMensajero(long numeroGuia);

        /// <summary>
        /// Consulta la informacion del manifiestó para telemercadeo
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<LIManifiestoMercadeoDC> ObtenerManifiesto(long numeroGuia);

        /// <summary>
        /// Retorna las guías asociadas a la localidad pasada en el estado solicitado
        /// </summary>
        /// <param name="estado"></param>
        /// <param name="idCentroServicio"></param>
        /// <param name="numeroGuia"></param>
        /// <param name="esCol"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        LIGestionesGuiaDC ObtenerGestionesGuiaTelemercadeo(ADEnumEstadoGuia estado, long idCentroServicio, long numeroGuia, bool esCol);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        LIArchivoEntregaDC ObtenerArchivoPruebaEntrega(long numeroGuia);

        /// <summary>
        /// Consulta la última gestión del telemercadeo
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        LIDetalleTelemercadeoDC ObtenerUltimaGestionTelemercadeoGuia(long numeroGuia);

        /// <summary>
        /// Consulta La gestion realizada por el usuario conectado el dia de hoy
        /// </summary>
        /// <returns>objeto estadistica telemercadeo</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        LIEstadisticaTelemercadeoDC ObtenerEstadisticaGestion();

        /// <summary>
        /// Obtener detalle de Telemercadeo al observar el flujo de la guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<LIDetalleTelemercadeoDC> ObtenerDetalleTelemercadeoGuia(long numeroGuia);

        /// <summary>
        /// Obtiene el detalle de los motivos de la guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<LIDetalleMotivoGuiaDC> ObtenerDetalleMotivoGuia(long numeroGuia);

        /// <summary>
        /// Consulta el historial de entregas de una direccion para una localidad
        /// </summary>
        /// <param name="direccion"></param>
        /// <param name="idLocalidad"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<LIHistorialEntregaDC> ObtenerHistorialEntregas(string direccion, string idLocalidad);

        /// <summary>
        /// Consulta las reclamaciondes de la guia
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<LIReclamacionesGuiaDC> ObtenerReclamacionesGuia(long numeroGuia);
        #endregion consultas

        #region Inserciones

        /// <summary>
        /// Método para insertar una gestión de telemercadeo
        /// </summary>
        /// <param name="?"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        int InsertarGestion(LIGestionesDC gestion);

        /// <summary>
        /// Método para insertar un estado de una guia
        /// </summary>
        /// <param name="trazaGuia"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        long CambiarEstadoGuia(ADTrazaGuia trazaGuia, LIGestionesDC gestion, ADMotivoGuiaDC motivo);


        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void CambiarDevolverEstadoGuia(long IdNumeroGuia, long IdEstado, string pObservaciones, string Usuario);


        /// <summary>
        /// Método para insertar un estado de una guia
        /// </summary>
        /// <param name="TrazaGuia"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        long CambiarEstadoGuiaTraza(ADTrazaGuia TrazaGuia);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ADEnumEstadoGuia InsertarGestionWpf(LIGestionesDC gestion);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ADEnumEstadoGuia InsertarGestionAgenciaWpf(LIGestionesDC gestion);

        #endregion Inserciones

        #region Eliminar

        /// <summary>
        /// Método para guardar
        /// </summary>
        /// <param name="idGestion"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void EliminarGestion(LIGestionesDC Gestion);

        #endregion Eliminar

        #endregion Telemercadeo

        #region Rapiradicados

        #region Consultas

        /// <summary>
        /// Método  para obtener las guias en estado rapiradicado y en estado supervision
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ADRapiRadicado> ObtenerGuiasRapiradicados(IDictionary<string, string> filtro);

        /// <summary>
        /// Método para obtener información de los rapiradicados asociados a una admision
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ADRapiRadicado> ObtenerRapiradicadosGuia(long numeroGuia);

        #endregion Consultas

        #region Inserciones

        /// <summary>
        /// Método para generar una guía interna de un rapiradicado
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ADRapiRadicado> GenerarGuiasInternas(List<ADRapiRadicado> listaRadicados);

        /// <summary>
        /// Genera una guía interna y la actualiza en los radicados asociados
        /// </summary>
        /// <param name="listaRadicados"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ADRapiRadicado> GenerarGuiasInternasConsolidado(List<ADRapiRadicado> listaRadicados);

        #endregion Inserciones

        #endregion Rapiradicados

        #region Planillas

        #region Consultas

        /// <summary>
        /// Método para obtener planillas
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<LIPlanillaDC> ObtenerPlanillas(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina, ADEnumTipoImpreso tipoImpreso);

        /// <summary>
        /// Método para obtener las guias de una planilla
        /// </summary>
        /// <param name="numeroPlanilla"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<LIPlanillaDetalleDC> ObtenerGuiasPlanilla(LIPlanillaDC planilla);

        #endregion Consultas

        #region Inserciones

        /// <summary>
        /// Método para insertar admisiones en una planilla de nuevas facturas
        /// </summary>
        /// <param name="numeroPlanilla"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        LIPlanillaDetalleDC AdicionarGuiaPlanilla(LIPlanillaDetalleDC guia);

        /// <summary>
        /// Método para insertar las guias internas en una planilla de cliente contado
        /// </summary>
        /// <param name="numeroPlanilla"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        LIPlanillaDetalleDC AdicionarGuiaPlanillaContado(LIPlanillaDetalleDC guia);

        /// <summary>
        /// Método para insertar las guias internas en una planilla de cliente credito
        /// </summary>
        /// <param name="numeroPlanilla"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        LIPlanillaDetalleDC AdicionarGuiaPlanillaCredito(LIPlanillaDetalleDC guia);

        /// <summary>
        /// Método para insertar una planilla de guías internas
        /// </summary>
        /// <param name="planilla"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        LIPlanillaDC AdicionarPlanilla(LIPlanillaDC planilla);


         /// <summary>
        /// Crea la planilla de devolucion para cliente credito o contado y adiciona la primer guia
        /// </summary>
        /// <param name="planilla"></param>
        /// <param name="guia"></param>
        /// <param name="planillaCredito"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        LIPlanillaDetalleDC CrearPlanillaAdicionarGuia(LIPlanillaDC planilla, LIPlanillaDetalleDC guia, bool planillaCredito);

        #endregion Inserciones

        #region Eliminaciones

        /// <summary>
        /// Método para eliminar una guia de una planilla con auditoria
        /// </summary>
        /// <param name="guia"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void EliminarGuiaPLanilla(LIPlanillaDetalleDC guia);

        #endregion Eliminaciones

        #endregion Planillas

        #region Rexpedion

        /// <summary>
        /// Realiza las validaciones de la guia para hacer la reexpedicion
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        LIReexpedicionEnvioDC ValidaGuiaParaReexpedicion(LIReexpedicionEnvioDC reexpedicion);

        /// <summary>
        /// Registra la reexpedicion del envio
        /// </summary>
        /// <param name="reexpedicion"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ADGuia GuardaReexpedicionEnvio(LIReexpedicionEnvioDC reexpedicion);

        #endregion Rexpedion

        #region Custodia

        #region Consultas

        /// <summary>
        /// Consulta las guias de acuerdo a un  estado y una localidad
        /// </summary>
        /// <param name="idEstado"></param>
        /// <param name="localidad"></param>
        /// <param name="numeroPagina"></param>
        /// <param name="tamanoPagina"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<LIGuiaCustodiaDC> ObtenerGuiasEstado(ADEnumEstadoGuia estado, string localidad, int numeroPagina, int tamanoPagina);

        /// <summary>
        /// Método para obtener una guía en custodia
        /// </summary>
        /// <param name="idEstado"></param>
        /// <param name="numeroGuia"></param>
        /// <param name="localidad"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        LIGuiaCustodiaDC ObtenerGuiaCustodia(short idEstado, long numeroGuia, string localidad);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<LISalidaCustodia> ObtenerSalidasCustodiaPorDia(long idCentroServicio, DateTime fechaConsulta);

        /// <summary>
        /// retorna la cantidad de envios en cierta ubicacion
        /// </summary>
        /// <param name="tipoUbicacion"></param>
        /// <param name="ubicacion"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        int ObtenerNumeroDeEnviosEnUbicacion(int tipoUbicacion, int ubicacion);

        #endregion Consultas

        #region Inserciones

        /// <summary>
        /// Método que almacena el cambio de estado y el motivo de salida de custodia
        /// </summary>
        /// <param name="traza"></param>
        /// <param name="motivo"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        long GuardarCambioEstadoWeb(LICambioEstadoCustodia ceCustodia);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void IngresoCustodia(PUCustodia custodia);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void SalidaCustodia(PUCustodia custodia);

        #endregion Inserciones

        #endregion Custodia

    }
}