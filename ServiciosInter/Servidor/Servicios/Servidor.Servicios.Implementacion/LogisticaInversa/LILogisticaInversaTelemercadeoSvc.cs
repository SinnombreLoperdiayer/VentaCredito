using CO.Servidor.LogisticaInversa.Telemercadeo;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.Servicios.Contratos;
using Framework.Servidor.ParametrosFW;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Threading;

namespace CO.Servidor.Servicios.Implementacion.LogisticaInversa
{
    /// <summary>
    /// Clase para los servicios de administración de logistica inversa telemercadeo
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class LILogisticaInversaTelemercadeoSvc : ILILogisticaInversaTelemercadeoSvc
    {
        #region Telemercadeo

        #region Constructor

        public LILogisticaInversaTelemercadeoSvc()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(PAAdministrador.Instancia.ConsultarParametrosFramework("Cultura"));
        }

        #endregion Constructor

        #region consultas

        /// <summary>
        /// Retorna las guías asociadas a la localidad pasada en el estado solicitado
        /// </summary>
        /// <param name="idEstado"></param>
        /// <param name="localidad"></param>
        /// <param name="numeroPagina"></param>
        /// <param name="tamanoPagina"></param>
        /// <returns></returns>
        public LIGestionesGuiaDC ObtenerGestionesGuias(ADEnumEstadoGuia estado, long idCentroServicio, long numeroGuia, bool esCol)
        {
            return LIAdministradorTelemercadeo.Instancia.ObtenerGestionesGuias(estado, idCentroServicio, numeroGuia, esCol);
        }

        /// <summary>
        /// Método para obtener los archivos de evidencia de una guia en telemercadeo
        /// </summary>
        /// <param name="IdEstadoGuiaLog"></param>
        /// <returns></returns>
        public List<LIEvidenciaDevolucionDC> ObtenerArchivosEvidenciaDevolucion(long IdEstadoGuiaLog)
        {
            return LIAdministradorTelemercadeo.Instancia.ObtenerArchivosEvidenciaDevolucion(IdEstadoGuiaLog);
        }

        /// <summary>
        /// Método para obtener el detalle de una guia con el número de gestiones
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="idEstado"></param>
        /// <param name="localidad"></param>
        /// <returns></returns>
        //public LIGestionesGuiaDC ObtenerGuiaGestiones(long numeroGuia, short idEstado, string localidad)
        //{
        //  return LIAdministradorTelemercadeo.Instancia.ObtenerGuiaGestiones(numeroGuia, idEstado, localidad);
        //}

        /// <summary>
        /// Método encargado de obtener gestiones de una guía
        /// </summary>
        /// <param name="idTrazaguia"></param>
        /// <returns></returns>
        public IList<LIGestionesDC> ObtenerGestionesGuia(long idTrazaguia)
        {
            return LIAdministradorTelemercadeo.Instancia.ObtenerGestionesGuia(idTrazaguia);
        }

        /// <summary>
        /// Método para obtener los resultados posibles de una gestión de telemercadeo
        /// </summary>
        /// <returns></returns>
        public IList<LIResultadoTelemercadeoDC> ObtenerResultados()
        {
            return LIAdministradorTelemercadeo.Instancia.ObtenerResultados();
        }

        /// <summary>
        /// Método encargado de obtener la información de la guía en admisión
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns>guía de admision</returns>
        public ADGuia ObtenerInfoAdmision(long numeroGuia)
        {
            return LIAdministradorTelemercadeo.Instancia.ObtenerInfoAdmision(numeroGuia);
        }

        /// <summary>
        /// Método para obtener los posibles estados de transición
        /// </summary>
        /// <param name="estadoGuia"></param>
        /// <returns></returns>
        public IList<ADEnumEstadoGuia> ObtenerEstados(ADEnumEstadoGuia estadoGuia)
        {
            return LIAdministradorTelemercadeo.Instancia.ObtenerEstados(estadoGuia);
        }


        // todo:id
        /// <summary>
        /// Método para obtener los posibles estados para Devolver
        /// </summary>
        /// <param name="estadoGuia"></param>
        /// <returns></returns>
        public List<ADEstadoGuia> ObtenerEstadosParaDevolver()
        {
            return LIAdministradorTelemercadeo.Instancia.ObtenerEstadosParaDevolver();
        }

        /// <summary>
        /// Método para obtener las razones de borrado de una gestión
        /// </summary>
        /// <returns></returns>
        public IList<LIGestionMotivoBorradoDC> ObtenerMotivosBorrado()
        {
            return LIAdministradorTelemercadeo.Instancia.ObtenerMotivosBorrado();
        }

        /// <summary>
        /// Retorna el stream de un archivo de envidencia de devolución dado su id
        /// </summary>
        /// <param name="idArchivo"></param>
        /// <returns></returns>
        public string ObtenerArchivoEvidenciaAdjunto(long idArchivo)
        {
            return LIAdministradorTelemercadeo.Instancia.ObtenerArchivoEvidenciaAdjunto(idArchivo);
        }

        /// <summary>
        /// Retorna lista de registros de las gestiones de la guia 
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<LIFlujoGuiaDC> ObtenerFlujoGuia(long numeroGuia)
        {
            return LIAdministradorTelemercadeo.Instancia.ObtenerFlujoGuia(numeroGuia);          
        }

        /// <summary>
        /// Consulta la informacion del flujo de la guia para telemercadeo
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public LIAdmisionGuiaFlujoDC ObtenerAdmisionGuiaFlujo(long numeroGuia)
        {
            return LIAdministradorTelemercadeo.Instancia.ObtenerAdmisionGuiaFlujo(numeroGuia);
        }

        /// <summary>
        /// Consulta la informacion de los ingresos a centro de acopio nacional
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<LIIngresoCentroAcopioNacionalDC> ObtenerIngresoAcopioNacional(long numeroGuia) 
        {
            return LIAdministradorTelemercadeo.Instancia.ObtenerIngresoAcopioNacional(numeroGuia);
        }

        /// <summary>
        /// Consulta la informacion de los ingresos a centro de acopio urbano
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<LIIngresoCentroAcopioUrbanoDC> ObtenerIngresoCentroAcopioUrbano(long numeroGuia)
        {
            return LIAdministradorTelemercadeo.Instancia.ObtenerIngresoCentroAcopioUrbano(numeroGuia);
        }

        /// <summary>
        /// Consulta de asignaciones a mensajero para telemercadeo
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<LIAsignacionMensajero> ObtenerAsignacionMensajero(long numeroGuia)
        {
            return LIAdministradorTelemercadeo.Instancia.ObtenerAsignacionMensajero(numeroGuia);
        }

        /// <summary>
        /// Consulta la informacion del manifiestó para telemercadeo
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<LIManifiestoMercadeoDC> ObtenerManifiesto(long numeroGuia)
        {
            return LIAdministradorTelemercadeo.Instancia.ObtenerManifiesto(numeroGuia);
        }

        /// <summary>
        /// Consulta la informacion del archivo de la prueba de entrega
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public LIArchivoEntregaDC ObtenerArchivoPruebaEntrega(long numeroGuia)
        {
            return LIAdministradorTelemercadeo.Instancia.ObtenerArchivoPruebaEntrega(numeroGuia);
        }

        /// <summary>
        /// Consulta la última gestión del telemercadeo
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public LIDetalleTelemercadeoDC ObtenerUltimaGestionTelemercadeoGuia(long numeroGuia)
        {
            return LIAdministradorTelemercadeo.Instancia.ObtenerUltimaGestionTelemercadeoGuia(numeroGuia);
        }

        /// <summary>
        /// Retorna las guías asociadas a la localidad pasada en el estado solicitado
        /// </summary>
        /// <param name="estado"></param>
        /// <param name="idCentroServicio"></param>
        /// <param name="numeroGuia"></param>
        /// <param name="esCol"></param>
        /// <returns></returns>
        public LIGestionesGuiaDC ObtenerGestionesGuiaTelemercadeo(ADEnumEstadoGuia estado, long idCentroServicio, long numeroGuia, bool esCol)
        {            
            return LIAdministradorTelemercadeo.Instancia.ObtenerGestionesGuiasTelemercadeo(estado, idCentroServicio, numeroGuia, esCol);
        }

        /// <summary>
        /// Consulta La gestion realizada por el usuario conectado el dia de hoy
        /// </summary>
        /// <returns>objeto estadistica telemercadeo</returns>
        public LIEstadisticaTelemercadeoDC ObtenerEstadisticaGestion()
        {
            return LIAdministradorTelemercadeo.Instancia.ObtenerEstadisticaGestion();
        }

        /// <summary>
        /// Obtener detalle de Telemercadeo al observar el flujo de la guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<LIDetalleTelemercadeoDC> ObtenerDetalleTelemercadeoGuia(long numeroGuia)
        {
            return LIAdministradorTelemercadeo.Instancia.ObtenerDetalleTelemercadeoGuia(numeroGuia);
        }

        /// <summary>
        /// Obtiene el detalle de los motivos de la guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<LIDetalleMotivoGuiaDC> ObtenerDetalleMotivoGuia(long numeroGuia)
        {
            return LIAdministradorTelemercadeo.Instancia.ObtenerDetalleMotivoGuia(numeroGuia);
        }

        #endregion consultas

        #region Inserciones

        /// <summary>
        /// Método para insertar una gestión de telemercadeo
        /// </summary>
        /// <param name="?"></param>
        public int InsertarGestion(LIGestionesDC gestion)
        {
            return LIAdministradorTelemercadeo.Instancia.InsertarGestion(gestion);
        }

        /// <summary>
        /// Método para insertar un estado de una guia
        /// </summary>
        /// <param name="trazaGuia"></param>
        /// <returns></returns>
        public long CambiarEstadoGuia(ADTrazaGuia trazaGuia, LIGestionesDC gestion, ADMotivoGuiaDC motivo)
        {
            return LIAdministradorTelemercadeo.Instancia.CambiarEstadoGuia(trazaGuia, gestion, motivo);
        }

        // todo:id Metodo para Devolver el estado de una Guia
        public void CambiarDevolverEstadoGuia(long IdNumeroGuia, long IdEstado, string pObservaciones, string Usuario)
        {
            LIAdministradorTelemercadeo.Instancia.CambiarDevolverEstadoGuia(IdNumeroGuia, IdEstado, pObservaciones, Usuario);
        }


        /// <summary>
        /// Método para insertar un estado de una guia
        /// </summary>
        /// <param name="TrazaGuia"></param>
        /// <returns></returns>
        public long CambiarEstadoGuiaTraza(ADTrazaGuia TrazaGuia)
        {
            return LIAdministradorTelemercadeo.Instancia.CambiarEstadoGuia(TrazaGuia);
        }

        public ADEnumEstadoGuia InsertarGestionWpf(LIGestionesDC gestion)
        {
           return LIAdministradorTelemercadeo.Instancia.InsertarGestionWpf(gestion);
        }

        public ADEnumEstadoGuia InsertarGestionAgenciaWpf(LIGestionesDC gestion)
        {
            return LIAdministradorTelemercadeo.Instancia.InsertarGestionAgenciaWpf(gestion);
        }

        /// <summary>
        /// Consulta el historial de entregas de una direccion para una localidad
        /// </summary>
        /// <param name="direccion"></param>
        /// <param name="idLocalidad"></param>
        /// <returns></returns>
        public List<LIHistorialEntregaDC> ObtenerHistorialEntregas(string direccion, string idLocalidad)
        {
            return LIAdministradorTelemercadeo.Instancia.ObtenerHistorialEntregas(direccion, idLocalidad);
        }   

        /// <summary>
        /// Consulta las reclamaciondes de la guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<LIReclamacionesGuiaDC> ObtenerReclamacionesGuia(long numeroGuia)
        {
            return LIAdministradorTelemercadeo.Instancia.ObtenerReclamacionesGuia(numeroGuia);
        }



        #endregion Inserciones

        #region Eliminar

        /// <summary>
        /// Método para guardar
        /// </summary>
        /// <param name="idGestion"></param>
        public void EliminarGestion(LIGestionesDC Gestion)
        {
            LIAdministradorTelemercadeo.Instancia.EliminarGestion(Gestion);
        }

        #endregion Eliminar

        #endregion Telemercadeo

        #region Rapiradicados

        #region Consultas

        /// Método  para obtener las guias en estado rapiradicado y en estado supervision
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <returns></returns>
        public List<ADRapiRadicado> ObtenerGuiasRapiradicados(IDictionary<string, string> filtro)
        {
            return LIAdministradorTelemercadeo.Instancia.ObtenerGuiasRapiradicados(filtro);
        }

        /// <summary>
        /// Método para obtener información de los rapiradicados asociados a una admision
        /// </summary>
        /// <returns></returns>
        public List<ADRapiRadicado> ObtenerRapiradicadosGuia(long numeroGuia)
        {
            return LIAdministradorTelemercadeo.Instancia.ObtenerRapiradicadosGuia(numeroGuia);
        }

        #endregion Consultas

        #region Inserciones

        /// <summary>
        /// Método para generar una guía interna de un rapiradicado
        /// </summary>
        /// <returns></returns>
        public List<ADRapiRadicado> GenerarGuiasInternas(List<ADRapiRadicado> listaRadicados)
        {
            return LIAdministradorTelemercadeo.Instancia.GenerarGuiasInternas(listaRadicados);
        }

        /// <summary>
        /// Genera una guía interna y la actualiza en los radicados asociados
        /// </summary>
        /// <param name="listaRadicados"></param>
        /// <returns></returns>
        public List<ADRapiRadicado> GenerarGuiasInternasConsolidado(List<ADRapiRadicado> listaRadicados)
        {
            return LIAdministradorTelemercadeo.Instancia.GenerarGuiasInternasConsolidado(listaRadicados);
        }

        #endregion Inserciones

        #endregion Rapiradicados

        #region Planillas

        #region Consultas

        /// <summary>
        /// Método para obtener planillas de guias internas
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <returns></returns>
        public IList<LIPlanillaDC> ObtenerPlanillas(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina, ADEnumTipoImpreso tipoImpreso)
        {
            return LIAdministradorTelemercadeo.Instancia.ObtenerPlanillas(filtro, indicePagina, registrosPorPagina, tipoImpreso);
        }

        /// <summary>
        /// Método para obtener las guias de una planilla
        /// </summary>
        /// <param name="numeroPlanilla"></param>
        /// <returns></returns>
        public IList<LIPlanillaDetalleDC> ObtenerGuiasPlanilla(LIPlanillaDC planilla)
        {
            return LIAdministradorTelemercadeo.Instancia.ObtenerGuiasPlanilla(planilla);
        
        }

        public IList<LISalidaCustodia> ObtenerSalidasCustodiaPorDia(long idCentroServicio, DateTime fechaConsulta)
        {
            return LIAdministradorTelemercadeo.Instancia.ObtenerSalidasCustodiaPorDia(idCentroServicio, fechaConsulta);
        }

        #endregion Consultas

        #region Inserciones

        /// <summary>   
        /// Método para insertar admisiones en una planilla de nuevas facturas
        /// </summary>
        /// <param name="numeroPlanilla"></param>
        /// <returns></returns>
        public LIPlanillaDetalleDC AdicionarGuiaPlanilla(LIPlanillaDetalleDC guia)
        {
            return LIAdministradorTelemercadeo.Instancia.AdicionarGuiaPlanilla(guia);
        }

        /// <summary>
        /// Método para insertar las guias internas en una planilla de cliente contado
        /// </summary>
        /// <param name="numeroPlanilla"></param>
        /// <returns></returns>
        public LIPlanillaDetalleDC AdicionarGuiaPlanillaContado(LIPlanillaDetalleDC guia)
        {
            return LIAdministradorTelemercadeo.Instancia.AdicionarGuiaPlanillaContado(guia);
        }

        /// <summary>
        /// Método para insertar las guias internas en una planilla de cliente credito
        /// </summary>
        /// <param name="numeroPlanilla"></param>
        /// <returns></returns>
        public LIPlanillaDetalleDC AdicionarGuiaPlanillaCredito(LIPlanillaDetalleDC guia)
        {
            return LIAdministradorTelemercadeo.Instancia.AdicionarGuiaPlanillaCredito(guia);
        }

        /// <summary>
        /// Método para insertar una planilla de guías internas
        /// </summary>
        /// <param name="planilla"></param>
        /// <returns></returns>
        public LIPlanillaDC AdicionarPlanilla(LIPlanillaDC planilla)
        {
            return LIAdministradorTelemercadeo.Instancia.AdicionarPlanilla(planilla);
        }

        /// <summary>
        /// Crea la planilla de devolucion para cliente credito o contado y adiciona la primer guia
        /// </summary>
        /// <param name="planilla"></param>
        /// <param name="guia"></param>
        /// <param name="planillaCredito"></param>
        /// <returns></returns>
        public LIPlanillaDetalleDC CrearPlanillaAdicionarGuia(LIPlanillaDC planilla, LIPlanillaDetalleDC guia, bool planillaCredito)
        {
            return LIAdministradorTelemercadeo.Instancia.CrearPlanillaAdicionarGuia(planilla, guia, planillaCredito);
        }

        #endregion Inserciones

        #region Eliminaciones

        /// <summary>
        /// Método para eliminar una guia de una planilla con auditoria
        /// </summary>
        /// <param name="guia"></param>
        public void EliminarGuiaPLanilla(LIPlanillaDetalleDC guia)
        {
            LIAdministradorTelemercadeo.Instancia.EliminarGuiaPLanilla(guia);
        }

        #endregion Eliminaciones

        #endregion Planillas

        #region Rexpedion

        /// <summary>
        /// Realiza las validaciones de la guia para hacer la reexpedicion
        /// </summary>
        public LIReexpedicionEnvioDC ValidaGuiaParaReexpedicion(LIReexpedicionEnvioDC reexpedicion)
        {
            return LIAdministradorTelemercadeo.Instancia.ValidaGuiaParaReexpedicion(reexpedicion);
        }

        /// <summary>
        /// Registra la reexpedicion del envio
        /// </summary>
        /// <param name="reexpedicion"></param>
        public ADGuia GuardaReexpedicionEnvio(LIReexpedicionEnvioDC reexpedicion)
        {
            return LIAdministradorTelemercadeo.Instancia.GuardaReexpedicionEnvio(reexpedicion);
        }

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
        public List<LIGuiaCustodiaDC> ObtenerGuiasEstado(ADEnumEstadoGuia estado, string localidad, int numeroPagina, int tamanoPagina)
        {
            return LIAdministradorTelemercadeo.Instancia.ObtenerGuiasEstado(estado, localidad, numeroPagina, tamanoPagina);
        }

        /// <summary>
        /// Método para obtener una guía en custodia
        /// </summary>
        /// <param name="idEstado"></param>
        /// <param name="numeroGuia"></param>
        /// <param name="localidad"></param>
        /// <returns></returns>
        public LIGuiaCustodiaDC ObtenerGuiaCustodia(short idEstado, long numeroGuia, string localidad)
        {
            return LIAdministradorTelemercadeo.Instancia.ObtenerGuiaCustodia((short)idEstado, numeroGuia, localidad);
        }


        public long GuardarCambioEstadoWeb(LICambioEstadoCustodia ceCustodia)
        {
            return LIAdministradorTelemercadeo.Instancia.GuardarCambioEstado(ceCustodia);
        }
        
        public int ObtenerNumeroDeEnviosEnUbicacion(int tipoUbicacion,  int ubicacion)
        {
            return LIAdministradorTelemercadeo.Instancia.ObtenerNumeroDeEnviosEnUbicacion(tipoUbicacion, ubicacion);
        }

        #endregion Consultas

        #region Inserciones

        public void IngresoCustodia(PUCustodia custodia)
        {
            LIAdministradorTelemercadeo.Instancia.IngresoCustodia(custodia);
        }

        public void SalidaCustodia(PUCustodia custodia)
        {
            LIAdministradorTelemercadeo.Instancia.SalidaCustodia(custodia);
        }

        #endregion Inserciones

        
        #endregion Custodia        

 

    }
}