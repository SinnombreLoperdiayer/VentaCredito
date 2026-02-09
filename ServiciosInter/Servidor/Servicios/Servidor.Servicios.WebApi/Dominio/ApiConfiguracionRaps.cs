using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;
using CO.Servidor.Servicios.ContratoDatos.Raps.Consultas;
using CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes;
using CO.Servidor.Servicios.WebApi.Comun;
using CO.Servidor.Servicios.WebApi.NotificacionesSignalR;
using CO.Servidor.Servicios.WebApiHub;
using Framework.Servidor.Comun;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CO.Servidor.Servicios.ContratoDatos.Raps.Escalonamiento;

namespace CO.Servidor.Servicios.WebApi.Dominio
{
    public class ApiConfiguracionRaps : ApiDominioBase
    {
        private static ApiConfiguracionRaps instancia = (ApiConfiguracionRaps)FabricaInterceptorApi.GetProxy(new ApiConfiguracionRaps(), COConstantesModulos.MODULO_RAPS);

        #region contructor
        public static ApiConfiguracionRaps Instancia
        {
            get { return ApiConfiguracionRaps.instancia; }
        }

        private ApiConfiguracionRaps() { }
        #endregion

        #region Accion
        /// <summary>
        /// Listar Acciones
        /// </summary>
        /// <returns></returns>
        public List<RAAccionDC> ListarAccion()
        {
            return FabricaServicios.ServicioConfiguracionRaps.ListarAccion();
        }

        /// <summary>
        /// obtener una accion
        /// </summary>
        /// <param name="idAccion"></param>
        /// <returns></returns>
        public RAAccionDC ObtenerAccion(short idAccion)
        {
            return FabricaServicios.ServicioConfiguracionRaps.ObtenerAccion(idAccion);
        }

        /// <summary>
        /// Crear una accion
        /// </summary>
        /// <param name="accion"></param>
        /// <returns></returns>
        public bool CrearAccion(RAAccionDC accion)
        {
            return FabricaServicios.ServicioConfiguracionRaps.CrearAccion(accion);
        }
        #endregion

        #region plantillaparametrizacion

        /// <summary>
        /// Crear accion plantilla parametrizacion raps
        /// </summary>
        /// <param name="accionPlantillaParametrizacionRaps"></param>
        /// <returns></returns>
        public bool CrearAccionPlantillaParametrizacionRaps(RAAccionPlantillaParametrizacionRapsDC accionPlantillaParametrizacionRaps)
        {
            return FabricaServicios.ServicioConfiguracionRaps.CrearAccionPlantillaParametrizacionRaps(accionPlantillaParametrizacionRaps);
        }

        /// <summary>
        /// Listar acciones plantilla parametrizacion raps
        /// </summary>
        /// <returns></returns>
        public List<RAAccionPlantillaParametrizacionRapsDC> ListarAccionPlantillaParametrizacionRaps()
        {
            return FabricaServicios.ServicioConfiguracionRaps.ListarAccionPlantillaParametrizacionRaps();
        }

        /// <summary>
        /// Obtiene una plantilla de parametrizacionRaps para una accion
        /// </summary>
        /// <param name="idAccionPlantilla"></param>
        /// <returns></returns>
        public RAAccionPlantillaParametrizacionRapsDC ObtenerAccionPlantillaParametrizacionRaps(long idAccionPlantilla)
        {
            return FabricaServicios.ServicioConfiguracionRaps.ObtenerAccionPlantillaParametrizacionRaps(idAccionPlantilla);
        }

        #endregion

        #region cargo

        /// <summary>
        /// Crea un nuevo cargo
        /// </summary>
        /// <param name="cargo"></param>
        /// <returns></returns>
        public bool CrearCargo(RACargoDC cargo)
        {
            return FabricaServicios.ServicioConfiguracionRaps.CrearCargo(cargo);
        }

        /// <summary>
        /// Listar los cargos
        /// </summary>
        /// <param name="idCargo"></param>
        /// <returns></returns>
        public List<RACargoDC> ObtenerCargos(int idCargo)
        {
            return FabricaServicios.ServicioConfiguracionRaps.ObtenerCargos(idCargo);
        }
        #endregion

        #region clasificacion

        /// <summary>
        /// Crea una nueva clasificacion
        /// </summary>
        /// <param name="clasificacion"></param>
        /// <returns>Verdadero al grabar </returns>
        public bool CrearClasificacion(RAClasificacionDC clasificacion)
        {
            return FabricaServicios.ServicioConfiguracionRaps.CrearClasificacion(clasificacion);
        }

        /// <summary>
        /// Lista las clasificaciones
        /// </summary>
        /// <returns>Lista la clasificacion</returns>
        public List<RAClasificacionDC> ListarClasificacion()
        {
            return FabricaServicios.ServicioConfiguracionRaps.ListarClasificacion();
        }

        /// <summary>
        /// Obtiene una clasificacion
        /// </summary>
        /// <param name="idClasificacion"></param>
        /// <returns>Clasificacion</returns>
        public RAClasificacionDC ObtenerClasificacion(int idClasificacion)
        {
            return FabricaServicios.ServicioConfiguracionRaps.ObtenerClasificacion(idClasificacion);
        }

        #endregion

        #region escalonamiento

        /// <summary>
        /// Crea un registro de escalonamiento
        /// </summary>
        /// <param name="escalonamiento"></param>
        /// <returns></returns>
        public bool CrearEscalonamiento(RAEscalonamientoDC escalonamiento)
        {
            return FabricaServicios.ServicioConfiguracionRaps.CrearEscalonamiento(escalonamiento);
        }

        /// <summary>
        /// Lista los registros de escalonamiento para una parametrizacion
        /// </summary>
        /// <param name="idParametrizacionRap"></param>
        /// <returns>Lista de escalonamiento</returns>
        public List<RAEscalonamientoDC> ListarEscalonamiento(long idParametrizacionRap)
        {
            return FabricaServicios.ServicioConfiguracionRaps.ListarEscalonamiento(idParametrizacionRap);
        }

        /// <summary>
        /// consulta un item de escalonamiento
        /// </summary>
        /// <param name="idEscalonamiento"></param>
        /// <returns>Escalonamiento</returns>
        //public RAEscalonamientoDC ObtenerEscalonamiento(long idEscalonamiento)
        //{
        //    return FabricaServicios.ServicioConfiguracionRaps.ObtenerEscalonamiento(idEscalonamiento);
        //}

        #endregion

        #region Estado

        /// <summary>
        /// Crear estado
        /// </summary>
        /// <param name="estado"></param>
        /// <returns></returns>
        public bool CrearEstados(RAEstadosDC estado)
        {
            return FabricaServicios.ServicioConfiguracionRaps.CrearEstados(estado);
        }

        /// <summary>
        /// Lista los estados existentes
        /// </summary>
        /// <returns>Lista estado</returns>
        public List<RAEstadosDC> ListarEstados(bool esResponsable, int idEstadoActual, int idCargoSolicita, long idSolicitud)
        {
            return FabricaServicios.ServicioConfiguracionRaps.ListarEstados(esResponsable, idEstadoActual, idCargoSolicita, idSolicitud);
        }

        /// <summary>
        /// Consulta un estado
        /// </summary>
        /// <param name="IdEstado"></param>
        /// <returns>objeto estado</returns>
        public RAEstadosDC ObtenerEstado(int IdEstado)
        {
            return FabricaServicios.ServicioConfiguracionRaps.ObtenerEstado(IdEstado);
        }

        #endregion

        #region flujoaccionestado

        /// <summary>
        /// Crea un nuevo item en la tabla Flujo Accion Estado
        /// </summary>
        /// <param name="flujoAccionEstado"></param>
        /// <returns>Verdaro si todo correcto</returns>
        public bool CrearFlujoAccionEstado(RAFlujoAccionEstadoDC flujoAccionEstado)
        {
            return FabricaServicios.ServicioConfiguracionRaps.CrearFlujoAccionEstado(flujoAccionEstado);
        }

        /// <summary>
        /// lista los item de un flujo
        /// </summary>
        /// <param name="idFlujo"></param>
        /// <returns></returns>
        public List<RAFlujoAccionEstadoDC> ListarFlujoAccionEstado(int idFlujo)
        {
            return FabricaServicios.ServicioConfiguracionRaps.ListarFlujoAccionEstado(idFlujo);
        }

        /// <summary>
        /// Obtiene un registro de flujo Accion Estado
        /// </summary>
        /// <param name="idFlujo"></param>
        /// <returns></returns>
        public RAFlujoAccionEstadoDC ObtenerFlujoAccionEstado(int idFlujo, byte idAccion, int idEstado, int idCargo)
        {
            return FabricaServicios.ServicioConfiguracionRaps.ObtenerFlujoAccionEstado(idFlujo, idAccion, idEstado, idCargo);
        }

        #endregion

        #region Formato

        /// <summary>
        /// Crea un formato
        /// </summary>
        /// <param name="formato"></param>
        /// <returns></returns>
        public bool CrearFormato(RAFormatoDC formato)
        {
            return FabricaServicios.ServicioConfiguracionRaps.CrearFormato(formato);
        }

        /// <summary>
        /// lista los formatos
        /// </summary>
        /// <returns></returns>
        public List<RAFormatoDC> ListarFormato()
        {
            return FabricaServicios.ServicioConfiguracionRaps.ListarFormato();
        }

        /// <summary>
        /// obitiene un registro de formato
        /// </summary>
        /// <param name="idFormato"></param>
        /// <returns></returns>
        public RAFormatoDC ObtenerFormato(int idFormato)
        {
            return FabricaServicios.ServicioConfiguracionRaps.ObtenerFormato(idFormato);
        }

        #endregion

        #region Grupo Usuario

        /// <summary>
        /// Crea un nuevo grupo usuario
        /// </summary>
        /// <param name="grupoUsuario"></param>
        /// <returns></returns>
        public bool CrearGrupoUsuario(RAGrupoUsuarioDC grupoUsuario)
        {
            return FabricaServicios.ServicioConfiguracionRaps.CrearGrupoUsuario(grupoUsuario);
        }

        /// <summary>
        /// Lista los grupos de usuario
        /// </summary>
        /// <returns></returns>
        public List<RAGrupoUsuarioDC> ListarGrupoUsuario()
        {
            return FabricaServicios.ServicioConfiguracionRaps.ListarGrupoUsuario();
        }

        /// <summary>
        /// Obtiene un registro de grupo usuario
        /// </summary>
        /// <param name="idGrupoUsuario"></param>
        /// <returns></returns>
        public RAGrupoUsuarioDC ObtenerGrupoUsuario(int idGrupoUsuario)
        {
            return FabricaServicios.ServicioConfiguracionRaps.ObtenerGrupoUsuario(idGrupoUsuario);
        }

        #endregion

        #region plantillacorreo

        /// <summary>
        /// Crea una nueva planilla de correo para una accion
        /// </summary>
        /// <param name="plantillaAccionCorreo"></param>
        /// <returns></returns>
        public bool CrearPantillaAccionCorreo(RAPantillaAccionCorreoDC plantillaAccionCorreo)
        {
            return FabricaServicios.ServicioConfiguracionRaps.CrearPantillaAccionCorreo(plantillaAccionCorreo);
        }

        /// <summary>
        /// Lista las plantillas para una accion
        /// </summary>
        /// <returns></returns>
        public List<RAPantillaAccionCorreoDC> ListarPantillaAccionCorreo(byte idAccion)
        {
            return FabricaServicios.ServicioConfiguracionRaps.ListarPantillaAccionCorreo(idAccion);
        }

        #endregion

        #region parametrizacion

        /// <summary>
        /// Modifica una parametrizacionRas
        /// </summary>
        /// <param name="parametrizacionRaps"></param>
        /// <returns></returns>
        public bool ModificarParametrizacionRaps(RAParametrizacionRapsDC parametrizacionRaps)
        {
            return FabricaServicios.ServicioConfiguracionRaps.ModificarParametrizacionRaps(parametrizacionRaps);
        }

        /// <summary>
        /// Lista de Parametrizaciones de solicitudes
        /// </summary>
        /// <returns></returns>
        public List<RAParametrizacionRapsDC> ListarParametrizacionRaps()
        {
            return FabricaServicios.ServicioConfiguracionRaps.ListarParametrizacionRaps();
        }


        /// <summary>
        /// Lista de Parametrizaciones activas manuales
        /// </summary>
        /// <returns></returns>
        public List<RAParametrizacionRapsDC> ListarParametrizacionRapsManuales()
        {
            return FabricaServicios.ServicioConfiguracionRaps.ListarParametrizacionRapsManuales();
        }

        /// <summary>
        /// Lista de Parametrizaciones activas tipo tarea
        /// </summary>
        /// <returns></returns>
        public List<RAParametrizacionRapsDC> ListarParametrizacionRapsTareas()
        {
            return FabricaServicios.ServicioConfiguracionRaps.ListarParametrizacionRapsTareas();
        }


        /// <summary>
        /// consulta la informacion de una parametrizacion de solicitudes 
        /// </summary>
        /// <param name="idParametrizacionRap"></param>
        /// <returns></returns>
        public RAParametrizacionRapsDC ObtenerParametrizacionRaps(long idParametrizacionRap)
        {
            return FabricaServicios.ServicioConfiguracionRaps.ObtenerParametrizacionRaps(idParametrizacionRap);
        }

        #endregion

        #region proceso

        /// <summary>
        /// Crea un registro de proceso
        /// </summary>
        /// <param name="proceso"></param>
        /// <returns></returns>
        public bool CrearProceso(RAProcesoDC proceso)
        {
            return FabricaServicios.ServicioConfiguracionRaps.CrearProceso(proceso);
        }

        /// <summary>
        /// Lista los procesos
        /// </summary>
        /// <returns></returns>
        public List<RAProcesoDC> ListarProceso()
        {
            return FabricaServicios.ServicioConfiguracionRaps.ListarProceso();
        }

        /// <summary>
        /// Consulta un proceso
        /// </summary>
        /// <param name="idProceso"></param>
        /// <returns></returns>
        public RAProcesoDC ObtenerProceso(int idProceso)
        {
            return FabricaServicios.ServicioConfiguracionRaps.ObtenerProceso(idProceso);
        }

        #endregion

        #region TipoCierre

        /// <summary>
        /// Crea un registro de quien cierra
        /// </summary>
        /// <param name="tipoCierre"></param>
        /// <returns></returns>
        public bool CrearTipoCierre(RATipoCierreDC tipoCierre)
        {
            return
                FabricaServicios.ServicioConfiguracionRaps.CrearTipoCierre(tipoCierre);
        }

        /// <summary>
        /// Lista registros quien cierra
        /// </summary>
        /// <returns></returns>
        public List<RATipoCierreDC> ListarTipoCierre()
        {
            return FabricaServicios.ServicioConfiguracionRaps.ListarTipoCierre();
        }

        /// <summary>
        /// consulta la informacion un registro Quien Cierra
        /// </summary>
        /// <param name="idTipoCierre"></param>
        /// <returns></returns>
        public RATipoCierreDC ObtenerTipoCierre(int idTipoCierre)
        {
            return FabricaServicios.ServicioConfiguracionRaps.ObtenerTipoCierre(idTipoCierre);
        }

        #endregion

        #region sistemaformato

        /// <summary>
        /// crea un nuevo registro de sistema formato
        /// </summary>
        /// <param name="sistemaFormato"></param>
        /// <returns></returns>
        public bool CrearSistemaFormato(RASistemaFormatoDC sistemaFormato)
        {
            return FabricaServicios.ServicioConfiguracionRaps.CrearSistemaFormato(sistemaFormato);
        }

        /// <summary>
        /// lista los registros de sistema formato
        /// </summary>
        /// <returns></returns>
        public List<RASistemaFormatoDC> ListarSistemaFormato()
        {
            return FabricaServicios.ServicioConfiguracionRaps.ListarSistemaFormato();
        }

        /// <summary>
        /// Consulta un registro de sistema formato
        /// </summary>
        /// <param name="idSistemaFormato"></param>
        /// <returns></returns>
        public RASistemaFormatoDC ObtenerSistemaFormato(int idSistemaFormato)
        {
            return FabricaServicios.ServicioConfiguracionRaps.ObtenerSistemaFormato(idSistemaFormato);
        }

        #endregion

        #region subclasificacion

        /// <summary>
        /// Crea una nueva subclasificacion
        /// </summary>
        /// <param name="subClasificacion"></param>
        /// <returns></returns>
        public bool CrearSubClasificacion(RASubClasificacionDC subClasificacion)
        {
            return FabricaServicios.ServicioConfiguracionRaps.CrearSubClasificacion(subClasificacion);
        }

        /// <summary>
        /// Lista las sub clasificaciones de una clasificacion
        /// </summary>
        /// <returns></returns>
        public List<RASubClasificacionDC> ListarSubClasificacion(int idClasificacion)
        {
            return FabricaServicios.ServicioConfiguracionRaps.ListarSubClasificacion(idClasificacion);
        }

        /// <summary>
        /// Consulta una subclasificacion
        /// </summary>
        /// <param name="IdSubclasificacion"></param>
        /// <returns></returns>
        public RASubClasificacionDC ObtenerSubClasificacion(int IdSubclasificacion)
        {
            return FabricaServicios.ServicioConfiguracionRaps.ObtenerSubClasificacion(IdSubclasificacion);
        }

        #endregion

        #region tiempoejecucion

        /// <summary>
        /// Crea un nuevo tiempo de ejecucion
        /// </summary>
        /// <param name="tiempoEjecucionRaps"></param>
        /// <returns></returns>
        public bool CrearTiempoEjecucionRaps(RATiempoEjecucionRapsDC tiempoEjecucionRaps)
        {
            return FabricaServicios.ServicioConfiguracionRaps.CrearTiempoEjecucionRaps(tiempoEjecucionRaps);
        }

        /// <summary>
        /// Lista los tiempo de ejecucion para una parametrizacion de Raps
        /// </summary>
        /// <param name="idParametrizacionRap"></param>
        /// <returns></returns>
        public List<RATiempoEjecucionRapsDC> ListarTiempoEjecucionRaps(long idParametrizacionRap)
        {
            return FabricaServicios.ServicioConfiguracionRaps.ListarTiempoEjecucionRaps(idParametrizacionRap);
        }

        public List<RAParametrosParametrizacionDC> ListarParametros(long idParametrizacion)
        {
            return FabricaServicios.ServicioConfiguracionRaps.ListarParametros(idParametrizacion);
        }

        /// <summary>
        /// Consulta registro tiempo de ejecucion Raps
        /// </summary>
        /// <param name="idEjecucion"></param>
        /// <returns></returns>
        public RATiempoEjecucionRapsDC ObtenerTiempoEjecucionRaps(long idEjecucion)
        {
            return FabricaServicios.ServicioConfiguracionRaps.ObtenerTiempoEjecucionRaps(idEjecucion);
        }

        #endregion

        #region tipohora

        /// <summary>
        /// Crea un tipo de hora
        /// </summary>
        /// <param name="tipoHora"></param>
        /// <returns></returns>
        public bool CrearTipoHora(RATipoHoraDC tipoHora)
        {
            return FabricaServicios.ServicioConfiguracionRaps.CrearTipoHora(tipoHora);
        }

        /// <summary>
        /// Lista los tipos de hora
        /// </summary>
        /// <returns></returns>
        public List<RATipoHoraDC> ListarTipoHora()
        {
            return FabricaServicios.ServicioConfiguracionRaps.ListarTipoHora(); ;
        }

        /// <summary>
        /// Consulta un tipo de hora
        /// </summary>
        /// <param name="idTipoHora"></param>
        /// <returns></returns>
        public RATipoHoraDC ObtenerTipoHora(int idTipoHora)
        {
            return FabricaServicios.ServicioConfiguracionRaps.ObtenerTipoHora(idTipoHora);
        }

        #endregion

        #region tipoperiodo

        /// <summary>
        /// Crea un nuevo tipo de periodo
        /// </summary>
        /// <param name="tipoPeriodo"></param>
        /// <returns></returns>
        public bool CrearTipoPeriodo(RATipoPeriodoDC tipoPeriodo)
        {
            return FabricaServicios.ServicioConfiguracionRaps.CrearTipoPeriodo(tipoPeriodo);
        }

        /// <summary>
        /// Lista los tipos de periodos
        /// </summary>
        /// <returns></returns>
        public List<RATipoPeriodoDC> ListarTipoPeriodo()
        {
            return FabricaServicios.ServicioConfiguracionRaps.ListarTipoPeriodo();
        }

        /// <summary>
        /// Consulta un tipo de periodo
        /// </summary>
        /// <param name="idTipoPeriodo"></param>
        /// <returns></returns>
        public RATipoPeriodoDC ObtenerTipoPeriodo(int idTipoPeriodo)
        {
            return FabricaServicios.ServicioConfiguracionRaps.ObtenerTipoPeriodo(idTipoPeriodo);
        }

        #endregion

        #region tiporap

        /// <summary>
        /// Crea un nuevo tipo de Rap
        /// </summary>
        /// <param name="tipoRap"></param>
        /// <returns></returns>
        public bool CrearTipoRap(RATipoRapDC tipoRap)
        {
            return FabricaServicios.ServicioConfiguracionRaps.CrearTipoRap(tipoRap);
        }

        /// <summary>
        /// Lista los tipos de Raps
        /// </summary>
        /// <returns></returns>
        public List<RATipoRapDC> ListarTipoRap()
        {
            return FabricaServicios.ServicioConfiguracionRaps.ListarTipoRap();
        }

        /// <summary>
        /// Consulta un tipo de rap
        /// </summary>
        /// <param name="idTipoRap"></param>
        /// <returns></returns>
        public RATipoRapDC ObtenerTipoRap(int idTipoRap)
        {
            return FabricaServicios.ServicioConfiguracionRaps.ObtenerTipoRap(idTipoRap);
        }

        #endregion

        #region activos

        /// <summary>
        /// Listar solicitudes Activas
        /// </summary>
        /// <returns></returns>
        public List<RATipoRapDC> ListarTipoRapAct()
        {
            return FabricaServicios.ServicioConfiguracionRaps.ListarTipoRapAct();
        }

        /// <summary>
        /// Listar estados activos
        /// </summary>
        /// <returns></returns>
        public List<RAEstadosDC> ListarEstadosAct(bool esResponsable, int idEstadoActual)
        {
            return FabricaServicios.ServicioConfiguracionRaps.ListarEstadosAct(esResponsable, idEstadoActual);
        }

        /// <summary>
        /// Listar los origen de raps activos
        /// </summary>
        /// <returns></returns>
        public List<RAOrigenRapsDC> ListarOrigenRaps()
        {
            return FabricaServicios.ServicioConfiguracionRaps.ListarOrigenRaps();
        }

        /// <summary>
        /// Listar tipo de cierre activo
        /// </summary>
        /// <returns></returns>
        public List<RATipoCierreDC> ListarTipoCierreAct()
        {
            return FabricaServicios.ServicioConfiguracionRaps.ListarTipoCierreAct();
        }

        /// <summary>
        /// Listar las clasificaciones  activas
        /// </summary>
        /// <returns></returns>
        public List<RAClasificacionDC> ListarClasificacionAct()
        {
            return FabricaServicios.ServicioConfiguracionRaps.ListarClasificacionAct();
        }

        /// <summary>
        /// Listar las subclasificaciones de la clasificacion dada.
        /// </summary>
        /// <param name="idClasificacion"></param>
        /// <returns></returns>
        public List<RASubClasificacionDC> ListarSubClasificacionAct(int idClasificacion)
        {
            return FabricaServicios.ServicioConfiguracionRaps.ListarSubClasificacionAct(idClasificacion);
        }

        /// <summary>
        /// Listar los sistemas activos
        /// </summary>
        /// <returns></returns>
        public List<RASistemaFormatoDC> ListarSistemaFormatoAct()
        {
            return FabricaServicios.ServicioConfiguracionRaps.ListarSistemaFormatoAct();
        }

        /// <summary>
        /// Listar los formatos activos de un sistema
        /// </summary>
        /// <param name="idSistemaFormato"></param>
        /// <returns></returns>
        public List<RAFormatoDC> ListarFormatoAct(int idSistemaFormato)
        {
            return FabricaServicios.ServicioConfiguracionRaps.ListarFormatoAct(idSistemaFormato);
        }

        /// <summary>
        /// Lista los grupos de usuario activos
        /// </summary>
        /// <returns></returns>
        public List<RAGrupoUsuarioDC> ListarGrupoUsuarioAct()
        {
            return FabricaServicios.ServicioConfiguracionRaps.ListarGrupoUsuarioAct();
        }

        /// <summary>
        /// Listar los procesos activos
        /// </summary>
        /// <returns></returns>
        public List<RAProcesoDC> ListarProcesosAct()
        {
            return FabricaServicios.ServicioConfiguracionRaps.ListarProcesosAct();
        }

        #endregion

        /// <summary>
        /// lista los cargos
        /// </summary>
        /// <returns></returns>
        public List<RACargoDC> ListarCargos()
        {
            return FabricaServicios.ServicioConfiguracionRaps.ListarCargos();
        }

        /// <summary>
        /// Cambia el estado de una parametrizacion Raps
        /// </summary>
        /// <param name="idParametrizacionRap"></param>
        /// <param name="estado"></param>
        /// <returns></returns>
        public bool CambiaEstadoParametrizacionRaps(long idParametrizacionRap, bool estado)
        {
            return FabricaServicios.ServicioConfiguracionRaps.CambiaEstadoParametrizacionRaps(idParametrizacionRap, estado);
        }

        /// <summary>
        /// lista los tipos de periodos Activos
        /// </summary>
        /// <returns></returns>
        public List<RATipoPeriodoDC> ListarTipoPeriodoAct()
        {
            return FabricaServicios.ServicioConfiguracionRaps.ListarTipoPeriodoAct();

        }

        #region Crear Origen Raps
        /// <summary>
        /// Crear Origen Raps
        /// </summary>
        /// <param name="origenRaps"></param>
        /// <returns></returns>
        public bool CrearOrigenRaps(RAOrigenRapsDC origenRaps)
        {
            return FabricaServicios.ServicioConfiguracionRaps.CrearOrigenRaps(origenRaps);
        }

        /// <summary>
        /// Obtiene Origen de la Raps.
        /// </summary>
        /// <param name="idOrigenRaps"></param>
        /// <returns></returns>
        public RAOrigenRapsDC ObtenerOrigenRaps(int idOrigenRaps)
        {
            return FabricaServicios.ServicioConfiguracionRaps.ObtenerOrigenRaps(idOrigenRaps);
        }
        #endregion


        public List<RARegionalSuculsalDC> ListarRegionalCargo()
        {
            return FabricaServicios.ServicioConfiguracionRaps.ListarRegionalCargo();
        }

        #region Tipo Incumplimiento
        /// <summary>
        /// Crear Tipo Incumplimineto
        /// </summary>
        /// <param name="tipoIncumplimiento"></param>
        /// <returns></returns>
        public bool CrearTipoIncumplimiento(RATipoIncumplimientoDC tipoIncumplimiento)
        {
            return FabricaServicios.ServicioConfiguracionRaps.CrearTipoIncumplimiento(tipoIncumplimiento);
        }

        /// <summary>
        /// Listar tipo de Incumplimiento
        /// </summary>
        /// <returns></returns>
        public List<RATipoIncumplimientoDC> ListarTipoIncumplimiento()
        {
            return FabricaServicios.ServicioConfiguracionRaps.ListarTipoIncumplimiento();
        }

        /// <summary>
        /// Obtener Tipo Incumplimiento
        /// </summary>
        /// <param name="idTipoIncumplimiento"></param>
        /// <returns></returns>
        public RATipoIncumplimientoDC ObtenerTipoIncumplimimento(int idTipoIncumplimiento)
        {
            return FabricaServicios.ServicioConfiguracionRaps.ObtenerTipoIncumplimiento(idTipoIncumplimiento);
        }
        #endregion

        public List<RAPaginaParametrizacionRapsDC> ListarParametrizacionRapsPaginada(int pagina, int registrosXPagina, int tipoRap, string ordenaPor)
        {
            return FabricaServicios.ServicioConfiguracionRaps.ListarParametrizacionRapsPaginada(pagina, registrosXPagina, tipoRap, ordenaPor);
        }

        #region Listar Cargo Escalonamiento Parametrizacion Raps
        /// <summary>
        /// ListarCargoEscalonamientoParametrizacionRaps
        /// </summary>
        /// <returns></returns>
        public List<RAEscalonamientoDC> ListarCargoEscalonamientoParametrizacionRaps(int idParametrizacion)
        {
            return FabricaServicios.ServicioConfiguracionRaps.ListarCargoEscalonamientoParametrizacionRaps(idParametrizacion);
        }

        /// <summary>
        ///  Obtener lista de Solicitudes Raps
        /// </summary>
        /// <returns></returns>
        public List<RAObtenerListaSolicitudesRaps> ObtenerListaSolicitudesRaps(long DocumentoSolicita, int IdEstado)
        {
            return FabricaServicios.ServicioSolicitudesRaps.ObtenerListaSolicitudesRaps(DocumentoSolicita, IdEstado);
        }
        #endregion


        #region Hora Escalar
        /// <summary>
        /// Crear Hora Escalar
        /// </summary>
        /// <param name="horaEscalar"></param>
        /// <returns></returns>
        public bool CrearHoraEscalar(RAHoraEscalarDC horaEscalar)
        {
            return FabricaServicios.ServicioConfiguracionRaps.CrearHoraEscalar(horaEscalar);
        }


        /// <summary>
        /// Listar Hora Escalar
        /// </summary>
        /// <returns></returns>
        public List<RAHoraEscalarDC> ListarHoraEscalar()
        {
            return FabricaServicios.ServicioConfiguracionRaps.ListarHoraEscalar();
        }

        /// <summary>
        /// Obtener Hora Escalar
        /// </summary>
        /// <param name="idhoraEscalar"></param>
        /// <returns></returns>
        public RAHoraEscalarDC ObtenerHoraEscalar(int idhoraEscalar)
        {
            return FabricaServicios.ServicioConfiguracionRaps.ObtenerHoraEscalar(idhoraEscalar);
        }
        #endregion


        #region Lista de registros Activos
        /// <summary>
        /// Listar Tipo de Incumplimiento Activo
        /// </summary>
        /// <returns></returns>
        public List<RATipoIncumplimientoDC> ListarTipoIncumplimientoAct()
        {
            return FabricaServicios.ServicioConfiguracionRaps.ListarTipoIncumplimientoAct();
        }

        /// <summary>
        /// Listar Tipo Hora Activos
        /// </summary>
        /// <returns></returns>
        public List<RATipoHoraDC> ListarTipoHoraAct()
        {
            return FabricaServicios.ServicioConfiguracionRaps.ListarTipoHoraAct();
        }

        /// <summary>
        /// Listar Origen Raps Activos
        /// </summary>
        /// <returns></returns>
        public List<RAOrigenRapsDC> ListarOrigenRapsAct()
        {
            return FabricaServicios.ServicioConfiguracionRaps.ListarOrigenRapsAct();
        }

        /// <summary>
        /// Listar Cargos Activos
        /// </summary>
        /// <returns></returns>
        public List<RACargoDC> ListarCargosAct()
        {
            return FabricaServicios.ServicioConfiguracionRaps.ListarCargosAct();
        }

        /// <summary>
        /// Listar Personas con cargo de Novasoft
        /// </summary>
        /// <returns></returns>
        public List<RACargoPersonaNovaRapDC> ListarCargoPersonaNova_Rap(RAFiltroCargoPersonaNovaRapDC filtro)
        {
            return FabricaServicios.ServicioConfiguracionRaps.ListarCargoPersonaNova_Rap(filtro);
        }

        /// <summary>
        /// Listar Cargo Escalonamiento para Parametrización de una Raps
        /// </summary>
        /// <returns></returns>
        public List<RAListarCargoEscalonamientoRapsDC> ListarCargoEscalonamientoRaps()
        {
            return FabricaServicios.ServicioConfiguracionRaps.ListarCargoEscalonamientoRaps();
        }

        /// <summary>
        /// Listar Hora Escalar Activos
        /// </summary>
        /// <returns></returns>
        public List<RAHoraEscalarDC> ListarHoraEscalarAct()
        {
            return FabricaServicios.ServicioConfiguracionRaps.ListarHoraEscalarAct();
        }
        #endregion


        ///// <summary>
        ///// Obtener Detalle Solicitud Raps 
        ///// </summary>
        ///// <param name="IdSolicitud"></param>
        ///// <returns></returns>       
        //public RAObtenerDetalleSolicitudDC ObtenerDetalleSolicitudRaps(int IdSolicitud)
        //{
        //    return FabricaServicios.ServicioSolicitudesRaps.ObtenerDetalleSolicitudRaps(IdSolicitud);
        //}

        internal ModeloResponse.Raps.DetalleParametrizacion ListaDetalleParametrizacion(int idParametrizacion)
        {
            throw new System.NotImplementedException();
        }



        /// <summary>
        /// Insertar Registro en Parametrizacion, Escalonamiento y Tiempo de Ejecucion
        /// </summary>
        /// <param name="parametrizacionRaps"></param>
        /// <param name="listEscalonamiento"></param>
        /// <param name="listaTiempoEjecucion"></param>
        /// <returns></returns>
        public long CrearParametrizacionRaps(RAParametrizacionRapsDC parametrizacionRaps, List<RAEscalonamientoDC> listEscalonamiento, List<RATiempoEjecucionRapsDC> listaTiempoEjecucion, List<RAParametrosParametrizacionDC> listaParametros)
        {
            return FabricaServicios.ServicioConfiguracionRaps.CrearParametrizacionRaps(parametrizacionRaps, listEscalonamiento, listaTiempoEjecucion, listaParametros);
        }


        /// <summary>
        /// Actualizar Registro en Parametrizacion, Escalonamiento y Tiempo de Ejecucion
        /// </summary>
        /// <param name="rAParametrizacionRapsDC"></param>
        /// <param name="listEscalonamiento"></param>
        /// <param name="listaTiempoEjecucion"></param>
        public void ActualizarParametrizacionRaps(RAParametrizacionRapsDC parametrizacionRaps, List<RAEscalonamientoDC> listEscalonamiento, List<RATiempoEjecucionRapsDC> listaTiempoEjecucion, List<RAParametrosParametrizacionDC> listaParametros)
        {
            FabricaServicios.ServicioConfiguracionRaps.ActualizarParametrizacionRaps(parametrizacionRaps, listEscalonamiento, listaTiempoEjecucion, listaParametros);
        }


        /// <summary>
        /// retorna los tipos de datos disponibles
        /// </summary>
        /// <returns></returns>
        public List<RATipoDatoDC> ObtenerTiposDatos()
        {
            return FabricaServicios.ServicioConfiguracionRaps.ObtenerTiposDatos();
        }

        /// <summary>
        /// Cambia el estado de una parametrizacion
        /// </summary>
        /// <param name="idParametrizacion"></param>
        /// <param name="estaActivo"></param>
        //public void CambiarEstadoParametrizacion(int idParametrizacion, bool estaActivo)
        //{
        //    FabricaServicios.ServicioConfiguracionRaps.CambiarEstadoParametrizacion(idParametrizacion, estaActivo);
        //}

        /// <summary>
        /// Metodo para Obtener las territoriales
        /// </summary>
        /// <returns></returns>
        public List<RATerritorialDC> ObtenerTerritoriales()
        {
            return FabricaServicios.ServicioConfiguracionRaps.ObtenerTerritoriales();
        }

        /// <summary>
        /// Metodo para obtener las regionales 
        /// </summary>
        /// <returns></returns>
        public List<RARegionalSuculsalDC> ObtenerRegionales()
        {
            return FabricaServicios.ServicioConfiguracionRaps.ObtenerRegionales();
        }

        /// <summary>
        /// Metodo para obtener el personal con la respectiva informacion de novasoft
        /// </summary>
        /// <returns></returns>
        public List<RAPersonaDC> ObtenerPersonal(string cargos, int idParametrizacion, int idSucursal)
        {
            return FabricaServicios.ServicioConfiguracionRaps.ObtenerPersonal(cargos, idParametrizacion, idSucursal);
        }

        /// <summary>
        /// Obtiene los menus permitidos para cada usuario 
        /// </summary>
        /// <param name="modulosPermitidos"></param>
        /// <returns></returns>
        public List<RAMenusPermitidosDC> ObtenerMenusPermitidos(List<RAModulosDC> modulosPermitidos)
        {
            return FabricaServicios.ServicioConfiguracionRaps.ObtenerMenusPermitidos(modulosPermitidos);
        }

        public void NotificarSolicitudUsuario(ParametrosSignalR parametro)
        {
            ParametrosSignalR usuario = HubPrincipal.ManejadorUsuarios.ListaUsuarios.Where(e => e.Documento == parametro.Documento).FirstOrDefault();
            if (usuario != null)
            {
                var context = GlobalHost.ConnectionManager.GetHubContext<HubPrincipal>();
                context.Clients.Client(usuario.IdConexion).enviarNotificacionUsuario(parametro);
                return;
            }
            else if (parametro.InsertarNotificacion)
            {
                FabricaServicios.ServicioConfiguracionRaps.InsertarNotificacionPendiente(parametro.Mensaje, parametro.Documento, parametro.IdSolicitud);
            }
        }

        public void NotificarMantenimiento(ParametrosSignalR parametro)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<HubPrincipal>();
            context.Clients.All.enviarNotificarMantenimiento(parametro);
            return;
        }

        public void NotificarSolicitudTodosUsuarios()
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<HubPrincipal>();
            context.Clients.All.updateCounters();
        }

        /// <summary>
        /// obtiene los procesos
        /// </summary>
        public List<RAProcesoDC> ObtenerProcesos()
        {
            return FabricaServicios.ServicioConfiguracionRaps.ObtenerProcesos();
        }

        /// <summary>
        /// Obtiene los procedimientos de determinado proceso
        /// </summary>
        /// <param name="idProceso"></param>
        /// <returns></returns>
        public List<RAProcedimientoDC> ObtenerProcedimientoPorproceso(int idProceso)
        {
            return FabricaServicios.ServicioConfiguracionRaps.ObtenerProcedimientoPorproceso(idProceso);
        }

        /// <summary>
        /// Obtiene los procedimientos de determinados procesos
        /// </summary>
        /// <param name="procesos"></param>
        /// <returns></returns>
        public List<RAProcedimientoDC> ObtenerProcedimientosPorprocesos(string procesos)
        {
            return FabricaServicios.ServicioConfiguracionRaps.ObtenerProcedimientosPorprocesos(procesos);
        }

        /// <summary>
        /// crea el grupo con sus correspondientes cargos
        /// </summary>
        /// <param name="grupo"></param>
        public bool CrearGrupoCargo(RACargoGrupoDC grupo)
        {
            return FabricaServicios.ServicioConfiguracionRaps.CrearGrupoCargo(grupo);
        }

        /// <summary>
        /// Lista todos los grupos creados
        /// </summary>
        /// <returns></returns>
        public List<RACargoGrupoDC> ListarGrupos()
        {
            return FabricaServicios.ServicioConfiguracionRaps.ListarGrupos();
        }

        /// <summary>
        /// Obtiene el cargo con sus respectivos cargos
        /// </summary>
        /// <param name="idGrupo"></param>
        /// <param name="porPersona"></param>
        /// <returns></returns>
        public RACargoGrupoDC ObtenerDetalleGrupo(int idGrupo, bool porPersona)
        {
            return FabricaServicios.ServicioConfiguracionRaps.ObtenerDetallaGrupo(idGrupo, porPersona);
        }

        /// <summary>
        /// Elimina un grupo por su id
        /// </summary>
        /// <param name="idGrupo"></param>
        /// <returns></returns>
        public bool EliminarGrupo(int idGrupo)
        {
            return FabricaServicios.ServicioConfiguracionRaps.EliminarGrupo(idGrupo);
        }

        /// <summary>
        /// agrega n cargos a un grupo ya creado
        /// </summary>
        /// <param name="cargos"></param>
        public void EditarCargosDeGrupo(RACargoGrupoDC cargos)
        {
            FabricaServicios.ServicioConfiguracionRaps.EditarCargosDeGrupo(cargos);
        }

        /// <summary>
        /// Edita la informacion de un grupo
        /// </summary>
        /// <param name="infoGrupo"></param>
        public void EditarGrupo(RACargoGrupoDC infoGrupo)
        {
            FabricaServicios.ServicioConfiguracionRaps.EditarGrupo(infoGrupo);
        }

        /// <summary>
        /// obtiene las notificaciones pendientes por usuario
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        public List<RANotificacionDC> ListarNotificacionesPendientes(string idUsuario)
        {
            return FabricaServicios.ServicioConfiguracionRaps.ListarNotificacionesPendientes(idUsuario);
        }

        /// <summary>
        /// Cambia de estato 0 a 1 las notificaciones vistas por un usuario
        /// </summary>
        /// <param name="idUsuario"></param>
        public void ActualizarEstadoNotificacion(string idUsuario)
        {
            FabricaServicios.ServicioConfiguracionRaps.ActualizarestadoNotificacion(idUsuario);
        }

        /// <summary>
        /// Obtiene las solicitudes que tienen como fecha de vencimiento hoy y/o mañana
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        public List<RASolicitudDC> ObtenerSolicitudesProximasAVencer(string idUsuario)
        {
            return FabricaServicios.ServicioConfiguracionRaps.ObtenerSolicitudesProximasAVencer(idUsuario);
        }

        #region Registro fallas InterLogis App / Fallas Web

        /// <summary>
        /// Metodo para obtener el tipo novedad segun resp
        /// </summary>
        /// <param name="idClaseResponsable"></param>
        /// <param name="idSistemaFuente"></param>
        /// <returns></returns>
        internal List<RANovedadDC> ObtenerTipoNovedadSegunResponsable(int idClaseResponsable, int idSistemaFuente)
        {
            return FabricaServicios.ServicioConfiguracionRaps.ObtenerTipoNovedadSegunResponsable(idClaseResponsable, idSistemaFuente);
        }

        /// <summary>
        /// Obtener parametros por integracion 
        /// </summary>
        /// <param name="tipoParametro"></param>
        /// <returns></returns>
        [System.Obsolete()]

        internal List<LIParametrizacionIntegracionRAPSDC> ObtenerParametrosPorIntegracion(string tipoParametro)
        {
            return FabricaServicios.ServicioLogisticaInversa.ObtenerParametrosPorIntegracion(tipoParametro);
        }

        

        /// <summary>
        /// Obtiene los niveles de falla para los mensajeros
        /// </summary>
        /// <returns></returns>
        public List<RANivelFallaDC> ObtenerNivelesDeFalla()
        {
            return FabricaServicios.ServicioConfiguracionRaps.ObtenerNivelesDeFalla();
        }

        /// <summary>
        /// Obtiene todos los tipos de novedad disponibles
        /// </summary>
        /// <returns></returns>
        public List<RAResponsableTipoNovedadDC> ObtenerTodoTipoNovedadDisponible()
        {
            return FabricaServicios.ServicioConfiguracionRaps.ObtenerTodoTipoNovedadDisponible();
        }

        /// <summary>
        /// Metodo para obtener parametros integracion por tipo novedad 
        /// </summary>
        /// <param name="idTipNovedad"></param>
        /// <returns></returns>
        internal List<LIParametrizacionIntegracionRAPSDC> ObtenerParametrosIntegracionPorTipoNovedad(int idTipNovedad)
        {
            return FabricaServicios.ServicioConfiguracionRaps.ObtenerParametrosIntegracionPorTipoNovedad(idTipNovedad);
        }

        /// <summary>
        /// Obtiene los tipos de escalonamiento
        /// </summary>
        /// <returns></returns>
        public List<RATipoEscalonamientoDC> ObtenerTiposEscalonamiento()
        {
            return FabricaServicios.ServicioConfiguracionRaps.ObtenerTiposEscalonamiento();
        }

        #endregion
    }
}