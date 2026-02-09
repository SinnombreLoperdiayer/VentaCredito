

using CO.Servidor.Raps;
using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;
using CO.Servidor.Servicios.ContratoDatos.Raps.Consultas;
using CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes;
using CO.Servidor.Servicios.Contratos;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.Servicios.ContratoDatos.Raps.Escalonamiento;

namespace CO.Servidor.Servicios.Implementacion.Raps
{
    /// <summary>
    /// Clase para los servicios de Configuracion de Raps
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class RAConfiguracionRapsSvc : IRAConfiguracionRapsSvc
    {

        #region Accion
        /// <summary>
        /// Listar Acciones
        /// </summary>
        /// <returns></returns>
        public List<RAAccionDC> ListarAccion()
        {
            return RAConfiguracion.Instancia.ListarAccion();
        }

        /// <summary>
        /// obtener una accion
        /// </summary>
        /// <param name="idAccion"></param>
        /// <returns></returns>
        public RAAccionDC ObtenerAccion(short idAccion)
        {
            return RAConfiguracion.Instancia.ObtenerAccion(idAccion);
        }

        /// <summary>
        /// Crear una accion
        /// </summary>
        /// <param name="accion"></param>
        /// <returns></returns>
        public bool CrearAccion(RAAccionDC accion)
        {
            return RAConfiguracion.Instancia.CrearAccion(accion);
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
            return RAConfiguracion.Instancia.CrearAccionPlantillaParametrizacionRaps(accionPlantillaParametrizacionRaps);
        }

        /// <summary>
        /// Listar acciones plantilla parametrizacion raps
        /// </summary>
        /// <returns></returns>
        public List<RAAccionPlantillaParametrizacionRapsDC> ListarAccionPlantillaParametrizacionRaps()
        {
            return RAConfiguracion.Instancia.ListarAccionPlantillaParametrizacionRaps();
        }

        /// <summary>
        /// Obtiene una plantilla de parametrizacionRaps para una accion
        /// </summary>
        /// <param name="idAccionPlantilla"></param>
        /// <returns></returns>
        public RAAccionPlantillaParametrizacionRapsDC ObtenerAccionPlantillaParametrizacionRaps(long idAccionPlantilla)
        {
            return RAConfiguracion.Instancia.ObtenerAccionPlantillaParametrizacionRaps(idAccionPlantilla);
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
            return RAConfiguracion.Instancia.CrearCargo(cargo);
        }

        /// <summary>
        /// Listar los cargos
        /// </summary>
        /// <param name="idCargo"></param>
        /// <returns></returns>
        public List<RACargoDC> ObtenerCargos(int idCargo)
        {
            return RAConfiguracion.Instancia.ObtenerCargos(idCargo);
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
            return RAConfiguracion.Instancia.CrearClasificacion(clasificacion);
        }

        /// <summary>
        /// Lista las clasificaciones
        /// </summary>
        /// <returns>Lista la clasificacion</returns>
        public List<RAClasificacionDC> ListarClasificacion()
        {
            return RAConfiguracion.Instancia.ListarClasificacion();
        }

        /// <summary>
        /// Obtiene una clasificacion
        /// </summary>
        /// <param name="idClasificacion"></param>
        /// <returns>Clasificacion</returns>
        public RAClasificacionDC ObtenerClasificacion(int idClasificacion)
        {
            return RAConfiguracion.Instancia.ObtenerClasificacion(idClasificacion);
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
            return RAConfiguracion.Instancia.CrearEscalonamiento(escalonamiento);
        }

        /// <summary>
        /// Lista los registros de escalonamiento para una parametrizacion
        /// </summary>
        /// <param name="idParametrizacionRap"></param>
        /// <returns>Lista de escalonamiento</returns>
        public List<RAEscalonamientoDC> ListarEscalonamiento(long idParametrizacionRap)
        {
            return RAConfiguracion.Instancia.ListarEscalonamiento(idParametrizacionRap);
        }

        /// <summary>
        /// consulta un item de escalonamiento
        /// </summary>
        /// <param name="idEscalonamiento"></param>
        /// <returns>Escalonamiento</returns>
        //public RAEscalonamientoDC ObtenerEscalonamiento(long idEscalonamiento)
        //{
        //    return RAConfiguracion.Instancia.ObtenerEscalonamiento(idEscalonamiento);
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
            return RAConfiguracion.Instancia.CrearEstados(estado);
        }

        /// <summary>
        /// Lista los estados existentes
        /// </summary>
        /// <returns>Lista estado</returns>
        public List<RAEstadosDC> ListarEstados(bool esResponsable, int idEstadoActual, int idCargoSolicita, long idSolicitud)
        {
            return RAConfiguracion.Instancia.ListarEstados(esResponsable, idEstadoActual, idCargoSolicita, idSolicitud);
        }

        /// <summary>
        /// Consulta un estado
        /// </summary>
        /// <param name="IdEstado"></param>
        /// <returns>objeto estado</returns>
        public RAEstadosDC ObtenerEstado(int IdEstado)
        {
            return RAConfiguracion.Instancia.ObtenerEstado(IdEstado);
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
            return RAConfiguracion.Instancia.CrearFlujoAccionEstado(flujoAccionEstado);
        }

        /// <summary>
        /// lista los item de un flujo
        /// </summary>
        /// <param name="idFlujo"></param>
        /// <returns></returns>
        public List<RAFlujoAccionEstadoDC> ListarFlujoAccionEstado(int idFlujo)
        {
            return RAConfiguracion.Instancia.ListarFlujoAccionEstado(idFlujo);
        }

        /// <summary>
        /// Obtiene un registro de flujo Accion Estado
        /// </summary>
        /// <param name="idFlujo"></param>
        /// <returns></returns>
        public RAFlujoAccionEstadoDC ObtenerFlujoAccionEstado(int idFlujo, byte idAccion, int idEstado, int idCargo)
        {
            return RAConfiguracion.Instancia.ObtenerFlujoAccionEstado(idFlujo, idAccion, idEstado, idCargo);
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
            return RAConfiguracion.Instancia.CrearFormato(formato);
        }

        /// <summary>
        /// lista los formatos
        /// </summary>
        /// <returns></returns>
        public List<RAFormatoDC> ListarFormato()
        {
            return RAConfiguracion.Instancia.ListarFormato();
        }

        /// <summary>
        /// obitiene un registro de formato
        /// </summary>
        /// <param name="idFormato"></param>
        /// <returns></returns>
        public RAFormatoDC ObtenerFormato(int idFormato)
        {
            return RAConfiguracion.Instancia.ObtenerFormato(idFormato);
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
            return RAConfiguracion.Instancia.CrearGrupoUsuario(grupoUsuario);
        }

        /// <summary>
        /// Lista los grupos de usuario
        /// </summary>
        /// <returns></returns>
        public List<RAGrupoUsuarioDC> ListarGrupoUsuario()
        {
            return RAConfiguracion.Instancia.ListarGrupoUsuario();
        }

        /// <summary>
        /// Obtiene un registro de grupo usuario
        /// </summary>
        /// <param name="idGrupoUsuario"></param>
        /// <returns></returns>
        public RAGrupoUsuarioDC ObtenerGrupoUsuario(int idGrupoUsuario)
        {
            return RAConfiguracion.Instancia.ObtenerGrupoUsuario(idGrupoUsuario);
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
            return RAConfiguracion.Instancia.CrearPantillaAccionCorreo(plantillaAccionCorreo);
        }

        /// <summary>
        /// Lista las plantillas para una accion
        /// </summary>
        /// <returns></returns>
        public List<RAPantillaAccionCorreoDC> ListarPantillaAccionCorreo(byte idAccion)
        {
            return RAConfiguracion.Instancia.ListarPantillaAccionCorreo(idAccion);
        }

        #endregion

        #region parametrizacion


        /// <summary>
        /// Modifica la parametrizacion Raps del parametro
        /// </summary>        
        /// <param name="parametrizacionRaps"></param>
        /// <returns></returns>
        public bool ModificarParametrizacionRaps(RAParametrizacionRapsDC parametrizacionRaps)
        {
            return RAConfiguracion.Instancia.ModificarParametrizacionRaps(parametrizacionRaps);
        }

        /// <summary>
        /// Lista de Parametrizaciones de solicitudes
        /// </summary>
        /// <returns></returns>
        public List<RAParametrizacionRapsDC> ListarParametrizacionRaps()
        {
            return RAConfiguracion.Instancia.ListarParametrizacionRaps();
        }


        /// <summary>
        /// Lista de Parametrizaciones activas manuales
        /// </summary>
        /// <returns></returns>
        public List<RAParametrizacionRapsDC> ListarParametrizacionRapsManuales()
        {
            return RAConfiguracion.Instancia.ListarParametrizacionRapsManuales();
        }

        /// <summary>
        /// Lista de Parametrizaciones activas tipo tarea
        /// </summary>
        /// <returns></returns>
        public List<RAParametrizacionRapsDC> ListarParametrizacionRapsTareas()
        {
            return RAConfiguracion.Instancia.ListarParametrizacionRapsTareas();
        }



        /// <summary>
        /// consulta la informacion de una parametrizacion de solicitudes 
        /// </summary>
        /// <param name="idParametrizacionRap"></param>
        /// <returns></returns>
        public RAParametrizacionRapsDC ObtenerParametrizacionRaps(long idParametrizacionRap)
        {
            return RAConfiguracion.Instancia.ObtenerParametrizacionRaps(idParametrizacionRap);
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
            return RAConfiguracion.Instancia.CrearProceso(proceso);
        }

        /// <summary>
        /// Lista los procesos
        /// </summary>
        /// <returns></returns>
        public List<RAProcesoDC> ListarProceso()
        {
            return RAConfiguracion.Instancia.ListarProceso();
        }

        /// <summary>
        /// Consulta un proceso
        /// </summary>
        /// <param name="idProceso"></param>
        /// <returns></returns>
        public RAProcesoDC ObtenerProceso(int idProceso)
        {
            return RAConfiguracion.Instancia.ObtenerProceso(idProceso);
        }

        #endregion

        #region TipoCierre

        /// <summary>
        /// Crea un registro de quien cierra
        /// </summary>
        /// <param name="quienCierra"></param>
        /// <returns></returns>
        public bool CrearTipoCierre(RATipoCierreDC quienCierra)
        {
            return
                RAConfiguracion.Instancia.CrearTipoCierre(quienCierra);
        }

        /// <summary>
        /// Lista registros quien cierra
        /// </summary>
        /// <returns></returns>
        public List<RATipoCierreDC> ListarTipoCierre()
        {
            return RAConfiguracion.Instancia.ListarTipoCierre();
        }

        /// <summary>
        /// consulta la informacion un registro Quien Cierra
        /// </summary>
        /// <param name="idQuienCierra"></param>
        /// <returns></returns>
        public RATipoCierreDC ObtenerTipoCierre(int idQuienCierra)
        {
            return RAConfiguracion.Instancia.ObtenerTipoCierre(idQuienCierra);
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
            return RAConfiguracion.Instancia.CrearSistemaFormato(sistemaFormato);
        }

        /// <summary>
        /// lista los registros de sistema formato
        /// </summary>
        /// <returns></returns>
        public List<RASistemaFormatoDC> ListarSistemaFormato()
        {
            return RAConfiguracion.Instancia.ListarSistemaFormato();
        }

        /// <summary>
        /// Consulta un registro de sistema formato
        /// </summary>
        /// <param name="idSistemaFormato"></param>
        /// <returns></returns>
        public RASistemaFormatoDC ObtenerSistemaFormato(int idSistemaFormato)
        {
            return RAConfiguracion.Instancia.ObtenerSistemaFormato(idSistemaFormato);
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
            return RAConfiguracion.Instancia.CrearSubClasificacion(subClasificacion);
        }

        /// <summary>
        /// Lista las sub clasificaciones de una clasificacion
        /// </summary>
        /// <returns></returns>
        public List<RASubClasificacionDC> ListarSubClasificacion(int idClasificacion)
        {
            return RAConfiguracion.Instancia.ListarSubClasificacion(idClasificacion);
        }

        /// <summary>
        /// Consulta una subclasificacion
        /// </summary>
        /// <param name="IdSubclasificacion"></param>
        /// <returns></returns>
        public RASubClasificacionDC ObtenerSubClasificacion(int IdSubclasificacion)
        {
            return RAConfiguracion.Instancia.ObtenerSubClasificacion(IdSubclasificacion);
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
            return RAConfiguracion.Instancia.CrearTiempoEjecucionRaps(tiempoEjecucionRaps);
        }

        /// <summary>
        /// Lista los tiempo de ejecucion para una parametrizacion de Raps
        /// </summary>
        /// <param name="idParametrizacionRap"></param>
        /// <returns></returns>
        public List<RATiempoEjecucionRapsDC> ListarTiempoEjecucionRaps(long idParametrizacionRap)
        {
            return RAConfiguracion.Instancia.ListarTiempoEjecucionRaps(idParametrizacionRap);
        }

        /// <summary>
        /// Consulta registro tiempo de ejecucion Raps
        /// </summary>
        /// <param name="idEjecucion"></param>
        /// <returns></returns>
        public RATiempoEjecucionRapsDC ObtenerTiempoEjecucionRaps(long idEjecucion)
        {
            return RAConfiguracion.Instancia.ObtenerTiempoEjecucionRaps(idEjecucion);
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
            return RAConfiguracion.Instancia.CrearTipoHora(tipoHora);
        }

        /// <summary>
        /// Lista los tipos de hora
        /// </summary>
        /// <returns></returns>
        public List<RATipoHoraDC> ListarTipoHora()
        {
            return RAConfiguracion.Instancia.ListarTipoHora(); ;
        }

        /// <summary>
        /// Consulta un tipo de hora
        /// </summary>
        /// <param name="idTipoHora"></param>
        /// <returns></returns>
        public RATipoHoraDC ObtenerTipoHora(int idTipoHora)
        {
            return RAConfiguracion.Instancia.ObtenerTipoHora(idTipoHora);
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
            return RAConfiguracion.Instancia.CrearTipoPeriodo(tipoPeriodo);
        }

        /// <summary>
        /// Lista los tipos de periodos
        /// </summary>
        /// <returns></returns>
        public List<RATipoPeriodoDC> ListarTipoPeriodo()
        {
            return RAConfiguracion.Instancia.ListarTipoPeriodo();
        }

        /// <summary>
        /// Consulta un tipo de periodo
        /// </summary>
        /// <param name="idTipoPeriodo"></param>
        /// <returns></returns>
        public RATipoPeriodoDC ObtenerTipoPeriodo(int idTipoPeriodo)
        {
            return RAConfiguracion.Instancia.ObtenerTipoPeriodo(idTipoPeriodo);
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
            return RAConfiguracion.Instancia.CrearTipoRap(tipoRap);
        }

        /// <summary>
        /// Lista los tipos de Raps
        /// </summary>
        /// <returns></returns>
        public List<RATipoRapDC> ListarTipoRap()
        {
            return RAConfiguracion.Instancia.ListarTipoRap();
        }

        /// <summary>
        /// Consulta un tipo de rap
        /// </summary>
        /// <param name="idTipoRap"></param>
        /// <returns></returns>
        public RATipoRapDC ObtenerTipoRap(int idTipoRap)
        {
            return RAConfiguracion.Instancia.ObtenerTipoRap(idTipoRap);
        }

        #endregion

        #region consultas


        /// <summary>
        /// Lista los tipos de Raps Activos
        /// </summary>
        /// <returns></returns>
        public List<RATipoRapDC> ListarTipoRapAct()
        {
            return RAConfiguracion.Instancia.ListaTipoRapAct();
        }

        /// <summary>
        /// Lista los estado activos
        /// </summary>
        /// <returns></returns>
        public List<RAEstadosDC> ListarEstadosAct(bool esResponsable, int idEstadoActual)
        {
            return RAConfiguracion.Instancia.ListarEstadosAct(esResponsable, idEstadoActual);
        }

        #region Origen Raps

        /// <summary>
        /// Crea un nuevo origen de Rap
        /// </summary>
        /// <param name="origenRaps"></param>
        /// <returns></returns>
        public bool CrearOrigenRaps(RAOrigenRapsDC origenRaps)
        {
            return RAConfiguracion.Instancia.CrearOrigenRaps(origenRaps);
        }


        /// <summary>
        /// listar origenes de raps
        /// </summary>
        /// <returns></returns>
        public List<RAOrigenRapsDC> ListarOrigenRaps()
        {
            return RAConfiguracion.Instancia.ListarOrigenRaps();
        }

        #endregion

        /// <summary>
        /// Listar tipo de cierre activos
        /// </summary>
        /// <returns></returns>
        public List<RATipoCierreDC> ListarTipoCierreAct()
        {
            return RAConfiguracion.Instancia.ListarTipoCierreAct();
        }

        /// <summary>
        /// Listar las clasificaciones  activas
        /// </summary>
        /// <returns></returns>
        public List<RAClasificacionDC> ListarClasificacionAct()
        {
            return RAConfiguracion.Instancia.ListarClasificacionAct();
        }

        /// <summary>
        /// Listar las subclasificaciones de la clasificacion dada.
        /// </summary>
        /// <param name="idClasificacion"></param>
        /// <returns></returns>
        public List<RASubClasificacionDC> ListarSubClasificacionAct(int idClasificacion)
        {
            return RAConfiguracion.Instancia.ListarSubClasificacionAct(idClasificacion);
        }

        /// <summary>
        /// Listar los sistemas activos
        /// </summary>
        /// <returns></returns>
        public List<RASistemaFormatoDC> ListarSistemaFormatoAct()
        {
            return RAConfiguracion.Instancia.ListarSistemaFormatoAct();
        }

        /// <summary>
        /// Listar los formatos activos de un sistema
        /// </summary>
        /// <param name="idSistemaFormato"></param>
        /// <returns></returns>

        public List<RAFormatoDC> ListarFormatoAct(int idSistemaFormato)
        {
            return RAConfiguracion.Instancia.ListarFormatoAct(idSistemaFormato);
        }

        /// <summary>
        /// Lista los grupos de usuario activos
        /// </summary>
        /// <returns></returns>
        public List<RAGrupoUsuarioDC> ListarGrupoUsuarioAct()
        {
            return RAConfiguracion.Instancia.ListarGrupoUsuarioAct();
        }

        /// <summary>
        /// Listar los procesos activos
        /// </summary>
        /// <returns></returns>
        public List<RAProcesoDC> ListarProcesosAct()
        {
            return RAConfiguracion.Instancia.ListarProcesosAct();
        }
        #endregion

        /// <summary>
        /// Lista los cargos
        /// </summary>
        /// <returns></returns>
        public List<RACargoDC> ListarCargos()
        {
            return RAConfiguracion.Instancia.ListarCargos();
        }

        /// <summary>
        /// Cambia el estado de una parametrizacion Raps
        /// </summary>
        /// <param name="idParametrizacionRap"></param>

        /// <param name="estado"></param>
        /// <returns></returns>
        public bool CambiaEstadoParametrizacionRaps(long idParametrizacionRap, bool estado)
        {
            return RAConfiguracion.Instancia.CambiaEstadoParametrizacionRaps(idParametrizacionRap, estado);
        }

        /// <summary>
        /// lista los tipos de periodo activos
        /// </summary>
        /// <returns></returns>
        public List<RATipoPeriodoDC> ListarTipoPeriodoAct()
        {
            return RAConfiguracion.Instancia.ListarTipoPeriodoAct();
        }

        public RAOrigenRapsDC ObtenerOrigenRaps(int idOrigenRaps)
        {
            return RAConfiguracion.Instancia.ObtenerOrigenRaps(idOrigenRaps);
        }


        /// <summary>
        /// Listar Tipo Incumplimiento
        /// </summary>
        /// <returns></returns>
        public List<RATipoIncumplimientoDC> ListarTipoIncumplimiento()
        {
            return RAConfiguracion.Instancia.ListarTipoIncumplimiento();
        }

        public List<RARegionalSuculsalDC> ListarRegionalCargo()
        {
            return RAConfiguracion.Instancia.ListarRegionalCargo();
        }

        /// <summary>
        /// Listar Tipo Incumplimiento Activos
        /// </summary>
        /// <returns></returns>
        public List<RATipoIncumplimientoDC> ListarTipoIncumplimientoAct()
        {
            return RAConfiguracion.Instancia.ListarTipoIncumplimientoAct();
        }


        /// <summary>
        /// Crear tipo de incumplimiento
        /// </summary>
        /// <param name="tipoIncumplimiento"></param>
        /// <returns></returns>
        public bool CrearTipoIncumplimiento(RATipoIncumplimientoDC tipoIncumplimiento)
        {
            return RAConfiguracion.Instancia.CrearTipoIncumplimiento(tipoIncumplimiento);
        }

        /// <summary>
        /// Obtener Tipo Incumplimiento
        /// </summary>
        /// <param name="idTipoIncumplimiento"></param>
        /// <returns></returns>
        public RATipoIncumplimientoDC ObtenerTipoIncumplimiento(int idTipoIncumplimiento)
        {
            return RAConfiguracion.Instancia.ObtenerTipoIncumplimiento(idTipoIncumplimiento);

        }

        public List<RAPaginaParametrizacionRapsDC> ListarParametrizacionRapsPaginada(int pagina, int registrosXPagina, int tipoRap, string ordenaPor)
        {
            return RAConfiguracion.Instancia.ListarParametrizacionRapsPaginada(pagina, registrosXPagina, tipoRap, ordenaPor);
        }

        /// <summary>
        /// Listar Tipo Hora Actual
        /// </summary>
        /// <returns></returns>
        public List<RATipoHoraDC> ListarTipoHoraAct()
        {
            return RAConfiguracion.Instancia.ListarTipoHoraAct();
        }


        /// <summary>
        /// Listar Origen Raps Activos
        /// </summary>
        /// <returns></returns>
        public List<RAOrigenRapsDC> ListarOrigenRapsAct()
        {
            return RAConfiguracion.Instancia.ListarOrigenRapsAct();
        }

        /// <summary>
        /// Listar Cargos Activos
        /// </summary>
        /// <returns></returns>
        public List<RACargoDC> ListarCargosAct()
        {
            return RAConfiguracion.Instancia.ListarCargosAct();
        }

        /// <summary>
        /// Listar Personas con cargo de Novasoft
        /// </summary>
        /// <returns></returns>
        public List<RACargoPersonaNovaRapDC> ListarCargoPersonaNova_Rap(RAFiltroCargoPersonaNovaRapDC filtro)
        {
            return RAConfiguracion.Instancia.ListarCargoPersonaNova_Rap(filtro);
        }

        /// <summary>
        /// Listar Cargo Escalonamiento Raps
        /// </summary>
        /// <returns></returns>
        public List<RAListarCargoEscalonamientoRapsDC> ListarCargoEscalonamientoRaps()
        {
            return RAConfiguracion.Instancia.ListarCargoEscalonamientoRaps();
        }

        /// <summary>
        /// ListarCargoEscalonamientoParametrizacionRaps
        /// </summary>
        /// <returns></returns>
        public List<RAEscalonamientoDC> ListarCargoEscalonamientoParametrizacionRaps(int idParametrizacion)
        {
            return RAConfiguracion.Instancia.ListarCargoEscalonamientoParametrizacionRaps(idParametrizacion);

        }


        public bool CrearHoraEscalar(RAHoraEscalarDC horaEscalar)
        {
            return RAConfiguracion.Instancia.CrearHoraEscalar(horaEscalar);
        }



        /// <summary>
        /// Listar Hora Escalar
        /// </summary>
        /// <returns></returns>
        public List<RAHoraEscalarDC> ListarHoraEscalar()
        {
            return RAConfiguracion.Instancia.ListarHoraEscalar();
        }


        /// <summary>
        /// Listar Hora Escalar Activos
        /// </summary>
        /// <returns></returns>
        public List<RAHoraEscalarDC> ListarHoraEscalarAct()
        {
            return RAConfiguracion.Instancia.ListarHoraEscalarAct();
        }


        /// <summary>
        /// Obtener Hora Escalar
        /// </summary>
        /// <param name="idhoraEscalar"></param>
        /// <returns></returns>
        public RAHoraEscalarDC ObtenerHoraEscalar(int idhoraEscalar)
        {
            return RAConfiguracion.Instancia.ObtenerHoraEscalar(idhoraEscalar);
        }


        /// <summary>
        /// Insertar Parametrizacion, Escalonamiento y Tiempo de Ejecucion (esta es la firma, porque aunque es el mismo método varía en la cantidad de parametros que le envío)
        /// </summary>
        /// <param name="parametrizacionRaps"></param>
        /// <param name="listEscalonamiento"></param>
        /// <param name="listaTiempoEjecucion"></param>
        /// <returns></returns>
        public long CrearParametrizacionRaps(RAParametrizacionRapsDC parametrizacionRaps, List<RAEscalonamientoDC> listEscalonamiento, List<RATiempoEjecucionRapsDC> listaTiempoEjecucion, List<RAParametrosParametrizacionDC> listaParametros)
        {
            return RAConfiguracion.Instancia.CrearParametrizacionRaps(parametrizacionRaps, listEscalonamiento, listaTiempoEjecucion, listaParametros);
        }



        /// <summary>
        /// Actualizar Parametrizacion Raps
        /// </summary>
        /// <param name="idParametrizacion"></param>   
        /// <param name="listEscalonamiento"></param>
        /// <param name="listaTiempoEjecucion"></param>
        public void ActualizarParametrizacionRaps(RAParametrizacionRapsDC parametrizacionRaps, List<RAEscalonamientoDC> listEscalonamiento, List<RATiempoEjecucionRapsDC> listaTiempoEjecucion, List<RAParametrosParametrizacionDC> listaParametros)
        {
            RAConfiguracion.Instancia.ActualizarParametrizacionRaps(parametrizacionRaps, listEscalonamiento, listaTiempoEjecucion, listaParametros);
        }

        /// <summary>
        /// retorna los tipos de datos disponibles
        /// </summary>
        /// <returns></returns>
        public List<RATipoDatoDC> ObtenerTiposDatos()
        {
            return RAConfiguracion.Instancia.ObtenerTiposDatos();
        }

        /// <summary>
        /// Cambia el estado de una parametrizacion
        /// </summary>
        /// <param name="idParametrizacion"></param>
        /// <param name="estaActivo"></param>
        //public void CambiarEstadoParametrizacion(int idParametrizacion, bool estaActivo)
        //{
        //    RAConfiguracion.Instancia.CambiarEstadoParametrizacion(idParametrizacion, estaActivo);
        //}

        public List<RAParametrosParametrizacionDC> ListarParametros(long idParametrizacion)
        {
            return RAConfiguracion.Instancia.ListarParametros(idParametrizacion);
        }

        /// <summary>
        /// Metodo para Obtener las territoriales
        /// </summary>
        /// <returns></returns>
        public List<RATerritorialDC> ObtenerTerritoriales()
        {
            return RAConfiguracion.Instancia.ObtenerTerritoriales();

        }

        /// <summary>
        /// Metodo para obtener las regionales
        /// </summary>
        /// <returns></returns>
        public List<RARegionalSuculsalDC> ObtenerRegionales()
        {
            return RAConfiguracion.Instancia.ObtenerRegionales();
        }

        /// <summary>
        /// Metodo para obtener el personal con informacion novasoft
        /// </summary>
        /// <returns></returns>
        public List<RAPersonaDC> ObtenerPersonal(string cargos, int idParametrizacion, int idSucursal)
        {

            return RAConfiguracion.Instancia.ObtenerPersonal(cargos, idParametrizacion, idSucursal);
        }

        /// <summary>
        /// Obtiene los menus permitidos para cada usuario 
        /// </summary>
        /// <param name="modulosPermitidos"></param>
        /// <returns></returns>
        public List<RAMenusPermitidosDC> ObtenerMenusPermitidos(List<RAModulosDC> modulosPermitidos)
        {
            return RAConfiguracion.Instancia.ObtenerMenusPermitidos(modulosPermitidos);
        }

        /// <summary>
        /// Inserta las notificaciones que no pudieron ser enviadas al usuario
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        public void InsertarNotificacionPendiente(string mensaje, long idUsuario, long idSolicitud)
        {
            RAConfiguracion.Instancia.InsertarNotificacionPendiente(mensaje, idUsuario, idSolicitud);
        }

        /// <summary>
        /// obtiene los procesos
        /// </summary>
        public List<RAProcesoDC> ObtenerProcesos()
        {
            return RAConfiguracion.Instancia.ObtenerProcesos();
        }

        /// <summary>
        /// Obtiene los procedimientos de determinado proceso
        /// </summary>
        /// <param name="idProceso"></param>
        /// <returns></returns>
        public List<RAProcedimientoDC> ObtenerProcedimientoPorproceso(int idProceso)
        {
            return RAConfiguracion.Instancia.ObtenerProcedimientoPorproceso(idProceso);
        }

        /// <summary>
        /// Obtiene los procedimientos de determinados procesos
        /// </summary>
        /// <param name="procesos"></param>
        /// <returns></returns>
        public List<RAProcedimientoDC> ObtenerProcedimientosPorprocesos(string procesos)
        {
            return RAConfiguracion.Instancia.ObtenerProcedimientosPorprocesos(procesos);
        }

        /// <summary>
        /// crea el grupo con sus correspondientes cargos
        /// </summary>
        /// <param name="grupo"></param>
        public bool CrearGrupoCargo(RACargoGrupoDC grupo)
        {
            return RAConfiguracion.Instancia.CrearGrupoCargo(grupo);
        }

        /// <summary>
        /// Lista todos los grupos creados
        /// </summary>
        /// <returns></returns>
        public List<RACargoGrupoDC> ListarGrupos()
        {
            return RAConfiguracion.Instancia.ListarGrupos();
        }

        /// <summary>
        /// Obtiene el cargo con sus respectivos cargos
        /// </summary>
        /// <param name="idGrupo"></param>
        /// <returns></returns>
        public RACargoGrupoDC ObtenerDetallaGrupo(int idGrupo, bool porPersona)
        {
            return RAConfiguracion.Instancia.ObtenerDetalleGrupo(idGrupo, porPersona);
        }

        /// <summary>
        /// Elimina un grupo por su id
        /// </summary>
        /// <param name="idGrupo"></param>
        /// <returns></returns>
        public bool EliminarGrupo(int idGrupo)
        {
            return RAConfiguracion.Instancia.EliminarGrupo(idGrupo);
        }

        /// <summary>
        /// agrega n cargos a un grupo ya creado
        /// </summary>
        /// <param name="cargos"></param>
        public void EditarCargosDeGrupo(RACargoGrupoDC cargos)
        {
            RAConfiguracion.Instancia.EditarCargosDeGrupo(cargos);
        }

        /// <summary>
        /// Edita la informacion de un grupo
        /// </summary>
        /// <param name="infoGrupo"></param>        
        public void EditarGrupo(RACargoGrupoDC infoGrupo)
        {
            RAConfiguracion.Instancia.EditarGrupo(infoGrupo);
        }

        /// <summary>
        /// obtiene las notificaciones pendientes por usuario
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>        
        public List<RANotificacionDC> ListarNotificacionesPendientes(string idUsuario)
        {
            return RAConfiguracion.Instancia.ListarNotificacionesPendientes(idUsuario);
        }

        /// <summary>
        /// Cambia de estato 0 a 1 las notificaciones vistas por un usuario
        /// </summary>
        /// <param name="idUsuario"></param>
        public void ActualizarestadoNotificacion(string idUsuario)
        {
            RAConfiguracion.Instancia.ActualizarEstadoNotificacion(idUsuario);
        }

        /// <summary>
        /// Obtiene las solicitudes que tienen como fecha de vencimiento hoy y/o mañana
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        public List<RASolicitudDC> ObtenerSolicitudesProximasAVencer(string idUsuario)
        {
            return RAConfiguracion.Instancia.ObtenerSolicitudesProximasAVencer(idUsuario);
        }

        #region Fallas Interlogis / Web

        /// <summary>
        /// Metodo para obtener tipo novedad segun responsable 
        /// </summary>
        /// <param name="idClaseResponsable"></param>
        /// <param name="idSistemaFuente"></param>
        /// <returns></returns>
        public List<RANovedadDC> ObtenerTipoNovedadSegunResponsable(int idClaseResponsable, int idSistemaFuente)
        {
            return RAConfiguracion.Instancia.ObtenerTipoNovedadSegunResponsable(idClaseResponsable, idSistemaFuente);
        }

        /// <summary>
        /// Obtiene los niveles de falla para los mensajeros
        /// </summary>
        /// <returns></returns>
        public List<RANivelFallaDC> ObtenerNivelesDeFalla()
        {
            return RAConfiguracion.Instancia.ObtenerNivelesDeFalla();
        }

        /// <summary>
        /// Obtiene todos los tipos de novedad disponibles
        /// </summary>
        /// <returns></returns>
        public List<RAResponsableTipoNovedadDC> ObtenerTodoTipoNovedadDisponible()
        {
            return RAConfiguracion.Instancia.ObtenerTodoTipoNovedadDisponible();
        }   

        /// <summary>
        /// Obtener parametros integracion por tipo novedad 
        /// </summary>
        /// <param name="idTipoNovedad"></param>
        /// <returns></returns>
        public List<LIParametrizacionIntegracionRAPSDC> ObtenerParametrosIntegracionPorTipoNovedad(int idTipoNovedad)
        {
            return RAConfiguracion.Instancia.ObtenerParametrosIntegracionPorTipoNovedad(idTipoNovedad);
        }

        /// <summary>
        /// Obtener parametros fallas personalizadas
        /// </summary>
        /// <param name="idTipoNovedad"></param>
        /// <returns></returns>
        [System.Obsolete()]
        public List<RAParametrosPersonalizacionRapsDC> ListaParametrosPersonalizacionPorNovedad(int idTipoNovedad)
        {
            return RAConfiguracion.Instancia.ListaParametrosPersonalizacionPorNovedad(idTipoNovedad);
        }

        /// <summary>
        /// Obtiene los tipos de escalonamiento
        /// </summary>
        /// <returns></returns>
        public List<RATipoEscalonamientoDC> ObtenerTiposEscalonamiento()
        {
            return RAConfiguracion.Instancia.ObtenerTiposEscalonamiento();
        }


        #endregion

    }
}
