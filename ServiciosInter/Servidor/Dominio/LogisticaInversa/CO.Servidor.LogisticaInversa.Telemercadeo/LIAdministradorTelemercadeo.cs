using CO.Servidor.LogisticaInversa.Telemercadeo.Custodia;
using CO.Servidor.LogisticaInversa.Telemercadeo.Reexpedicion;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using System;
using System.Collections.Generic;

namespace CO.Servidor.LogisticaInversa.Telemercadeo
{
    public class LIAdministradorTelemercadeo
    {
        private static readonly LIAdministradorTelemercadeo instancia = new LIAdministradorTelemercadeo();

        public static LIAdministradorTelemercadeo Instancia
        {
            get { return LIAdministradorTelemercadeo.instancia; }
        }

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
        public LIGestionesGuiaDC ObtenerGestionesGuias(ADEnumEstadoGuia estado, long idCentroServicio, long numeroGuia, bool esCol)
        {
            return LIConfiguradorTelemercadeo.Instancia.ObtenerGestionesGuias(estado, idCentroServicio, numeroGuia, esCol);
        }

        /// <summary>
        /// Método para obtener los archivos de evidencia de una guia en telemercadeo
        /// </summary>
        /// <param name="IdEstadoGuiaLog"></param>
        /// <returns></returns>
        public List<LIEvidenciaDevolucionDC> ObtenerArchivosEvidenciaDevolucion(long IdEstadoGuiaLog)
        {
            return LIConfiguradorTelemercadeo.Instancia.ObtenerArchivosEvidenciaDevolucion(IdEstadoGuiaLog);
        }

        /// <summary>
        /// Método para obtener el detalle de una guia con el número de gestiones
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="idEstado"></param>
        /// <param name="localidad"></param>
        /// <returns></returns>
        public LIGestionesGuiaDC ObtenerGuiaGestiones(long numeroGuia, short idEstado, string localidad)
        {
            return LIConfiguradorTelemercadeo.Instancia.ObtenerGuiaGestiones(numeroGuia, idEstado, localidad);
        }

        /// <summary>
        /// Método encargado de obtener gestiones de una guía
        /// </summary>
        /// <param name="idTrazaguia"></param>
        /// <returns></returns>
        public IList<LIGestionesDC> ObtenerGestionesGuia(long idTrazaguia)
        {
            return LIConfiguradorTelemercadeo.Instancia.ObtenerGestionesGuia(idTrazaguia);
        }

        /// <summary>
        /// Metodo para obtener el estado de la guia en telemercadeo
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public LIEstadoYMotivoGuiaDC ObtenerEstadoYMotivoGuia(long numeroGuia)
        {
            return LIConfiguradorTelemercadeo.Instancia.ObtenerEstadoYMotivoGuia(numeroGuia);
        }

        /// <summary>
        /// Metodo para obtener el flujo de la guia en telemercadeo
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns>lista con las gestiones en laguia</returns>
        public List<LIFlujoGuiaDC> ObtenerFlujoGuia(long numeroGuia)
        {
            return LIConfiguradorTelemercadeo.Instancia.ObtenerFlujoGuia(numeroGuia);
        }


        /// <summary>
        /// Método para obtener los resultados posibles de una gestión de telemercadeo
        /// </summary>
        /// <returns></returns>
        public IList<LIResultadoTelemercadeoDC> ObtenerResultados()
        {
            return LIConfiguradorTelemercadeo.Instancia.ObtenerResultados();
        }

        /// <summary>
        /// Método encargado de obtener la información de la guía en admisión
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns>guía de admision</returns>
        public ADGuia ObtenerInfoAdmision(long numeroGuia)
        {
            return LIConfiguradorTelemercadeo.Instancia.ObtenerInfoAdmision(numeroGuia);
        }

        /// <summary>
        /// Método para obtener los posibles estados de transición
        /// </summary>
        /// <param name="estadoGuia"></param>
        /// <returns></returns>
        public IList<ADEnumEstadoGuia> ObtenerEstados(ADEnumEstadoGuia estadoGuia)
        {
            return LIConfiguradorTelemercadeo.Instancia.ObtenerEstados(estadoGuia);
        }

        public List<ADEstadoGuia> ObtenerEstadosParaDevolver()
        {
            return LIConfiguradorTelemercadeo.Instancia.ObtenerEstadosParaDevolver();
        }

        /// <summary>
        /// Método para obtener las razones de borrado de una gestión
        /// </summary>
        /// <returns></returns>
        public IList<LIGestionMotivoBorradoDC> ObtenerMotivosBorrado()
        {
            return LIConfiguradorTelemercadeo.Instancia.ObtenerMotivosBorrado();
        }

        /// <summary>
        /// Retorna el stream de un archivo de envidencia de devolución dado su id
        /// </summary>
        /// <param name="idArchivo"></param>
        /// <returns></returns>
        public string ObtenerArchivoEvidenciaAdjunto(long idArchivo)
        {
            return LIConfiguradorTelemercadeo.Instancia.ObtenerArchivoEvidenciaAdjunto(idArchivo);
        }

        /// <summary>
        /// Consulta la informacion del flujo de la guia para telemercadeo
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public LIAdmisionGuiaFlujoDC ObtenerAdmisionGuiaFlujo(long numeroGuia)
        {
            return LIConfiguradorTelemercadeo.Instancia.ObtenerAdmisionGuiaFlujo(numeroGuia);
        }

        /// <summary>
        /// Consulta la informacion de los ingresos a centro de acopio nacional
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<LIIngresoCentroAcopioNacionalDC> ObtenerIngresoAcopioNacional(long numeroGuia)
        {
            return LIConfiguradorTelemercadeo.Instancia.ObtenerIngresoAcopioNacional(numeroGuia);
        }

        /// <summary>
        /// Consulta la informacion de los ingresos a centro de acopio urbano
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<LIIngresoCentroAcopioUrbanoDC> ObtenerIngresoCentroAcopioUrbano(long numeroGuia)
        {
            return LIConfiguradorTelemercadeo.Instancia.ObtenerIngresoCentroAcopioUrbano(numeroGuia);
        }

        /// <summary>
        /// Consulta de asignaciones a mensajero para telemercadeo
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<LIAsignacionMensajero> ObtenerAsignacionMensajero(long numeroGuia)
        {
            return LIConfiguradorTelemercadeo.Instancia.ObtenerAsignacionMensajero(numeroGuia);
        }

        /// <summary>
        /// Consulta la informacion del manifiestó para telemercadeo
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<LIManifiestoMercadeoDC> ObtenerManifiesto(long numeroGuia)
        {
            return LIConfiguradorTelemercadeo.Instancia.ObtenerManifiesto(numeroGuia);
        }

        /// <summary>
        /// Consulta la informacion del archivo de la prueba de entrega
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public LIArchivoEntregaDC ObtenerArchivoPruebaEntrega(long numeroGuia)
        {
            return LIConfiguradorTelemercadeo.Instancia.ObtenerArchivoPruebaEntrega(numeroGuia);
        }

        /// <summary>
        /// Consulta la última gestión del telemercadeo
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public LIDetalleTelemercadeoDC ObtenerUltimaGestionTelemercadeoGuia(long numeroGuia)
        {
            return LIConfiguradorTelemercadeo.Instancia.ObtenerUltimaGestionTelemercadeoGuia(numeroGuia);
        }

        /// <summary>
        /// Retorna las guías asociadas a la localidad pasada en el estado solicitado
        /// </summary>
        /// <param name="estado"></param>
        /// <param name="idCentroServicio"></param>
        /// <param name="numeroGuia"></param>
        /// <param name="esCol"></param>
        /// <returns></returns>
        public LIGestionesGuiaDC ObtenerGestionesGuiasTelemercadeo(ADEnumEstadoGuia estado, long idCentroServicio, long numeroGuia, bool esCol)
        {
            return LIConfiguradorTelemercadeo.Instancia.ObtenerGestionesGuiasTelemercadeo(estado, idCentroServicio, numeroGuia, esCol);
        }

        /// <summary>
        /// Consulta La gestion realizada por el usuario conectado el dia de hoy
        /// </summary>
        /// <returns>objeto estadistica telemercadeo</returns>
        public LIEstadisticaTelemercadeoDC ObtenerEstadisticaGestion()
        {
            return LIConfiguradorTelemercadeo.Instancia.ObtenerEstadisticaGestion();
        }

        /// <summary>
        /// Obtener detalle de Telemercadeo al observar el flujo de la guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<LIDetalleTelemercadeoDC> ObtenerDetalleTelemercadeoGuia(long numeroGuia)
        {
            return LIConfiguradorTelemercadeo.Instancia.ObtenerDetalleTelemercadeoGuia(numeroGuia);
        }
        /// <summary>
        /// Obtiene el detalle de los motivos de la guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<LIDetalleMotivoGuiaDC> ObtenerDetalleMotivoGuia(long numeroGuia)
        {
            return LIConfiguradorTelemercadeo.Instancia.ObtenerDetalleMotivoGuia(numeroGuia);
        }

        /// <summary>
        /// Consulta el historial de entregas de una direccion para una localidad
        /// </summary>
        /// <param name="direccion"></param>
        /// <param name="idLocalidad"></param>
        /// <returns></returns>
        public List<LIHistorialEntregaDC> ObtenerHistorialEntregas(string direccion, string idLocalidad)
        {
            return LIConfiguradorTelemercadeo.Instancia.ObtenerHistorialEntregas(direccion,idLocalidad);
        }
        #endregion consultas

        #region Inserciones

        /// <summary>
        /// Método para insertar una gestión de telemercadeo
        /// </summary>
        /// <param name="?"></param>
        public int InsertarGestion(LIGestionesDC gestion)
        {
            return LIConfiguradorTelemercadeo.Instancia.InsertarGestion(gestion);
        }

        /// <summary>
        /// Método para insertar una gestión de telemercadeo desde Versin WPF
        /// </summary>
        /// <param name="?"></param>
        public ADEnumEstadoGuia InsertarGestionWpf(LIGestionesDC gestion)
        {
            return LIConfiguradorTelemercadeo.Instancia.InsertarGestionWpf(gestion);
        }

        public ADEnumEstadoGuia InsertarGestionAgenciaWpf(LIGestionesDC gestion)
        {
            return LIConfiguradorTelemercadeo.Instancia.InsertarGestionAgenciaWpf(gestion);
        }

        /// <summary>
        /// Método para insertar un estado de una guia
        /// </summary>
        /// <param name="trazaGuia"></param>
        /// <returns></returns>
        public long CambiarEstadoGuia(ADTrazaGuia trazaGuia, LIGestionesDC gestion, ADMotivoGuiaDC motivo)
        {
            return LIConfiguradorTelemercadeo.Instancia.CambiarEstadoGuia(trazaGuia, gestion, motivo);
        }

        /// <summary>
        /// Método para insertar un estado de una guia
        /// </summary>
        /// <param name="TrazaGuia"></param>
        /// <returns></returns>
        public long CambiarEstadoGuia(ADTrazaGuia TrazaGuia)
        {
            return LIConfiguradorTelemercadeo.Instancia.CambiarEstadoGuia(TrazaGuia);
        }

        // todo:id Metodo para Devolver el estado de una Guia
        public void CambiarDevolverEstadoGuia(long IdNumeroGuia, long IdEstado, string pObservaciones, string Usuario)
        {
            CO.Servidor.Dominio.Comun.AdmEstadosGuia.EstadosGuia.CambiarDevolverEstadoGuia(IdNumeroGuia, IdEstado, pObservaciones, Usuario);
        }

        /// <summary>
        /// Consulta las reclamaciondes de la guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<LIReclamacionesGuiaDC> ObtenerReclamacionesGuia(long numeroGuia)
        {
            return LIConfiguradorTelemercadeo.Instancia.ObtenerReclamacionesGuia(numeroGuia);
        }


        #endregion Inserciones

        #region Eliminar

        /// <summary>
        /// Método para guardar
        /// </summary>
        /// <param name="idGestion"></param>
        public void EliminarGestion(LIGestionesDC Gestion)
        {
            LIConfiguradorTelemercadeo.Instancia.EliminarGestion(Gestion);
        }

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
        public List<ADRapiRadicado> ObtenerGuiasRapiradicados(IDictionary<string, string> filtro)
        {
            return LIConfiguradorRapiradicados.Instancia.ObtenerGuiasRapiradicados(filtro);
        }

        /// <summary>
        /// Método para obtener información de los rapiradicados asociados a una admision
        /// </summary>
        /// <returns></returns>
        public List<ADRapiRadicado> ObtenerRapiradicadosGuia(long numeroGuia)
        {
            return LIConfiguradorRapiradicados.Instancia.ObtenerRapiradicadosGuia(numeroGuia);
        }

        #endregion Consultas

        #region Inserciones

        /// <summary>
        /// Método para generar una guía interna de un rapiradicado
        /// </summary>
        /// <returns></returns>
        public List<ADRapiRadicado> GenerarGuiasInternas(List<ADRapiRadicado> listaRadicados)
        {
            return LIConfiguradorRapiradicados.Instancia.GenerarGuiasInternas(listaRadicados);
        }

        /// <summary>
        /// Genera una guía interna y la actualiza en los radicados asociados
        /// </summary>
        /// <param name="listaRadicados"></param>
        /// <returns></returns>
        public List<ADRapiRadicado> GenerarGuiasInternasConsolidado(List<ADRapiRadicado> listaRadicados)
        {
            return LIConfiguradorRapiradicados.Instancia.GenerarGuiasInternasConsolidado(listaRadicados);
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
            return LIConfiguradorPlanillas.Instancia.ObtenerPlanillas(filtro, indicePagina, registrosPorPagina, tipoImpreso);
        }

        /// <summary>
        /// Método para obtener las guias de una planilla
        /// </summary>
        /// <param name="numeroPlanilla"></param>
        /// <returns></returns>
        public IList<LIPlanillaDetalleDC> ObtenerGuiasPlanilla(LIPlanillaDC planilla)
        {
            return LIConfiguradorPlanillas.Instancia.ObtenerGuiasPlanilla(planilla);
        }

        public IList<LISalidaCustodia> ObtenerSalidasCustodiaPorDia(long idCentroServicio, DateTime fechaConsulta)
        {
            return LIConfiguradorPlanillas.Instancia.ObtenerSalidasCustodiaPorDia(idCentroServicio, fechaConsulta);
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
            return LIConfiguradorPlanillas.Instancia.AdicionarGuiaPlanilla(guia);
        }

        /// <summary>
        /// Método para insertar las guias internas en una planilla de cliente contado
        /// </summary>
        /// <param name="numeroPlanilla"></param>
        /// <returns></returns>
        public LIPlanillaDetalleDC AdicionarGuiaPlanillaContado(LIPlanillaDetalleDC guia)
        {
            return LIConfiguradorPlanillas.Instancia.AdicionarGuiaPlanillaContado(guia);
        }

        /// <summary>
        /// Método para insertar las guias internas en una planilla de cliente credito
        /// </summary>
        /// <param name="numeroPlanilla"></param>
        /// <returns></returns>
        public LIPlanillaDetalleDC AdicionarGuiaPlanillaCredito(LIPlanillaDetalleDC guia)
        {
            return LIConfiguradorPlanillas.Instancia.AdicionarGuiaPlanillaCredito(guia);
        }

        /// <summary>
        /// Método para insertar una planilla de guías internas
        /// </summary>
        /// <param name="planilla"></param>
        /// <returns></returns>
        public LIPlanillaDC AdicionarPlanilla(LIPlanillaDC planilla)
        {
            return LIConfiguradorPlanillas.Instancia.AdicionarPlanilla(planilla);
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
            return LIConfiguradorPlanillas.Instancia.CrearPlanillaAdicionarGuia(planilla, guia, planillaCredito);
        }

        #endregion Inserciones

        #region Eliminaciones

        /// <summary>
        /// Método para eliminar una guia de una planilla con auditoria
        /// </summary>
        /// <param name="guia"></param>
        public void EliminarGuiaPLanilla(LIPlanillaDetalleDC guia)
        {
            LIConfiguradorPlanillas.Instancia.EliminarGuiaPLanilla(guia);
        }

        #endregion Eliminaciones

        #endregion Planillas

        #region Reexpedion

        /// <summary>
        /// Realiza las validaciones de la guia para hacer la reexpedicion
        /// </summary>
        public LIReexpedicionEnvioDC ValidaGuiaParaReexpedicion(LIReexpedicionEnvioDC reexpedicion)
        {
            return LIConfiguradorReexpedicion.Instancia.ValidaGuiaParaReexpedicion(reexpedicion);
        }

        /// <summary>
        /// Registra la reexpedicion del envio
        /// </summary>
        /// <param name="reexpedicion"></param>
        public ADGuia GuardaReexpedicionEnvio(LIReexpedicionEnvioDC reexpedicion)
        {
            return LIConfiguradorReexpedicion.Instancia.GuardaReexpedicionEnvio(reexpedicion);
        }

        #endregion Reexpedion

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
            return LIConfiguradorCustodia.Instancia.ObtenerGuiasEstado(estado, localidad, numeroPagina, tamanoPagina);
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
            return LIConfiguradorCustodia.Instancia.ObtenerGuiaCustodia((short)idEstado, numeroGuia, localidad);
        }

        public long GuardarCambioEstado(LICambioEstadoCustodia ceCustodia)
        {
            return LIConfiguradorCustodia.Instancia.GuardarCambioEstado(ceCustodia);

        }

        public int ObtenerNumeroDeEnviosEnUbicacion(int tipoUbicacion, int ubicacion)
        {
            return LIConfiguradorCustodia.Instancia.ObtenerNumeroDeEnviosEnUbicacion(tipoUbicacion, ubicacion);
        }

        #endregion Consultas

        #region Inserciones

        public void IngresoCustodia(PUCustodia custodia)
        {
            LIConfiguradorCustodia.Instancia.IngresoCustodia(custodia);
        }

        //Obsolete
        public void SalidaCustodia(PUCustodia custodia)
        {
           //LIConfiguradorCustodia.Instancia.SalidaCustodia(custodia);
        }

        #endregion Inserciones

        #endregion Custodia



    }
}