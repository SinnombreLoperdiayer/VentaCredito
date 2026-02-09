using CO.Servidor.Raps.Datos;
using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;
using CO.Servidor.Servicios.ContratoDatos.Raps.Consultas;
using CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes;
using Framework.Servidor.Comun;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using System;
using Framework.Servidor.Excepciones;
using CO.Servidor.Servicios.ContratoDatos.Raps.Escalonamiento;

namespace CO.Servidor.Raps
{
    public class RAConfiguracion : ControllerBase
    {
        private static readonly RAConfiguracion instancia = (RAConfiguracion)FabricaInterceptores.GetProxy(new RAConfiguracion(), COConstantesModulos.MODULO_RAPS);

        #region singleton

        public static RAConfiguracion Instancia
        {
            get { return RAConfiguracion.instancia; }
        }

        public RAConfiguracion() { }

        #endregion

        #region Insertar
        /// <summary>
        /// Crea el origen Raps
        /// </summary>
        /// <param name="origenRaps"></param>
        /// <returns></returns>
        public bool CrearOrigenRaps(RAOrigenRapsDC origenRaps)
        {
            return RARepositorio.Instancia.CrearOrigenRaps(origenRaps);
        }

        /// <summary>
        /// Crear tipo de incumplimiento
        /// </summary>
        /// <param name="tipoIncumplimiento"></param>
        /// <returns></returns>
        public bool CrearTipoIncumplimiento(RATipoIncumplimientoDC tipoIncumplimiento)
        {
            return RARepositorio.Instancia.CrearTipoIncumplimiento(tipoIncumplimiento);
        }

        /// <summary>
        /// Crear Hora Escalar
        /// </summary>
        /// <param name="horaEscalar"></param>
        /// <returns></returns>
        public bool CrearHoraEscalar(RAHoraEscalarDC horaEscalar)
        {
            return RARepositorio.Instancia.CrearHoraEscalar(horaEscalar);
        }

        /// <summary>
        /// Insertar Parametrizacion, Escalonamiento y Tiempo de ejecución
        /// </summary>
        /// <param name="parametrizacionRaps"></param>
        /// <param name="listEscalonamiento"></param>
        /// <param name="listaTiempoEjecucion"></param>
        /// <returns></returns>
        public long CrearParametrizacionRaps(RAParametrizacionRapsDC parametrizacionRaps, List<RAEscalonamientoDC> listEscalonamiento, List<RATiempoEjecucionRapsDC> listaTiempoEjecucion, List<RAParametrosParametrizacionDC> listaParametros)
        {
            long IdParametrizacion = 0;
            using (TransactionScope transaccion = new TransactionScope())
            {

                //insertar parametrizacion raps

                IdParametrizacion = RARepositorio.Instancia.CrearParametrizacionRaps(parametrizacionRaps);


                if (listEscalonamiento != null && listEscalonamiento.Any())
                {

                    listEscalonamiento.ForEach(escalonamiento =>
                    {
                        escalonamiento.IdParametrizacionRap = IdParametrizacion;
                        RARepositorio.Instancia.CrearEscalonamiento(escalonamiento);
                    });
                }

                if (listaTiempoEjecucion != null && listaTiempoEjecucion.Any())
                {
                    int count = 0;
                    listaTiempoEjecucion.ForEach(tiempos =>
                    {
                        tiempos.IdParametrizacionRap = IdParametrizacion;
                        tiempos.NumeroEjecucion = count;
                        RARepositorio.Instancia.CrearTiempoEjecucionRaps(tiempos);
                        count++;
                    });
                }

                if (listaParametros != null && listaParametros.Any() && listaParametros[0].idTipoDato != 0)
                {
                    listaParametros.ForEach(parametros =>
                    {
                        //int? idTipoNovedad = null;
                        parametros.idParametrizacionRap = IdParametrizacion;

                        //if (parametros.idTipoDato == (int)RAEnumTipoDato.TIPONOVEDAD)
                        // {
                        ///se debe cambiar ya que no debe insertar la novedad sino el id que el cliente envia
                        //  idTipoNovedad = RARepositorio.Instancia.InsertarTipoNovedad(parametros.descripcionParametro);
                        // }
                        //parametros.idTipoNovedad = idTipoNovedad;
                        RARepositorio.Instancia.InsertarParametrosParametrizacion(parametros);
                    }
                        );
                }

                transaccion.Complete();
            }

            return IdParametrizacion;
        }

        /// <summary>
        /// Crear una accion
        /// </summary>
        /// <param name="accion"></param>
        /// <returns></returns>
        public bool CrearAccion(RAAccionDC accion)
        {
            return RARepositorio.Instancia.CrearAccion(accion);
        }

        /// <summary>
        /// Crear accion plantilla parametrizacion raps
        /// </summary>
        /// <param name="accionPlantillaParametrizacionRaps"></param>
        /// <returns></returns>
        public bool CrearAccionPlantillaParametrizacionRaps(RAAccionPlantillaParametrizacionRapsDC accionPlantillaParametrizacionRaps)
        {
            return RARepositorio.Instancia.CrearAccionPlantillaParametrizacionRaps(accionPlantillaParametrizacionRaps);
        }

        /// <summary>
        /// Crea un nuevo cargo
        /// </summary>
        /// <param name="cargo"></param>
        /// <returns></returns>
        public bool CrearCargo(RACargoDC cargo)
        {
            return RARepositorio.Instancia.CrearCargo(cargo);
        }

        #region Clasificacion

        /// <summary>
        /// Crea una nueva clasificacion
        /// </summary>
        /// <param name="clasificacion"></param>
        /// <returns>Verdadero al grabar </returns>
        public bool CrearClasificacion(RAClasificacionDC clasificacion)
        {
            return RARepositorio.Instancia.CrearClasificacion(clasificacion);
        }
        #endregion

        /// <summary>
        /// Crea un registro de escalonamiento
        /// </summary>
        /// <param name="escalonamiento"></param>
        /// <returns></returns>
        public bool CrearEscalonamiento(RAEscalonamientoDC escalonamiento)
        {
            return RARepositorio.Instancia.CrearEscalonamiento(escalonamiento);
        }

        /// <summary>
        /// consulta un item de escalonamiento
        /// </summary>
        /// <param name="idEscalonamiento"></param>
        /// <returns>Escalonamiento</returns>
        //public RAEscalonamientoDC ObtenerEscalonamiento(long idEscalonamiento)
        //{
        //    return RARepositorio.Instancia.ObtenerEscalonamiento(idEscalonamiento);
        //}

        /// <summary>
        /// Crear estado
        /// </summary>
        /// <param name="estado"></param>
        /// <returns></returns>
        public bool CrearEstados(RAEstadosDC estado)
        {
            return RARepositorio.Instancia.CrearEstado(estado);
        }

        /// <summary>
        /// Crea un nuevo item en la tabla Flujo Accion Estado
        /// </summary>
        /// <param name="flujoAccionEstado"></param>
        /// <returns>Verdaro si todo correcto</returns>
        public bool CrearFlujoAccionEstado(RAFlujoAccionEstadoDC flujoAccionEstado)
        {
            return RARepositorio.Instancia.CrearFlujoAccionEstado(flujoAccionEstado);
        }

        /// <summary>
        /// Crea un formato
        /// </summary>
        /// <param name="formato"></param>
        /// <returns></returns>
        public bool CrearFormato(RAFormatoDC formato)
        {
            return RARepositorio.Instancia.CrearFormato(formato);
        }

        /// <summary>
        /// Crea un nuevo grupo usuario
        /// </summary>
        /// <param name="grupoUsuario"></param>
        /// <returns></returns>
        public bool CrearGrupoUsuario(RAGrupoUsuarioDC grupoUsuario)
        {
            return RARepositorio.Instancia.CrearGrupoUsuario(grupoUsuario);
        }

        /// <summary>
        /// Crea una nueva planilla de correo para una accion
        /// </summary>
        /// <param name="plantillaAccionCorreo"></param>
        /// <returns></returns>
        public bool CrearPantillaAccionCorreo(RAPantillaAccionCorreoDC plantillaAccionCorreo)
        {
            return RARepositorio.Instancia.CrearPantillaAccionCorreo(plantillaAccionCorreo);
        }

        ///// <summary>
        ///// Modifac la parametrizacion Raps del parametros
        ///// </summary>
        ///// <param name="parametrizacionRaps"></param>
        ///// <returns></returns>
        //public bool ModificarParametrizacionRaps(RAParametrizacionRapsDC parametrizacionRaps)
        //{
        //    return RARepositorio.Instancia.ModificarParametrizacionRaps(parametrizacionRaps);
        //}

        /// <summary>
        /// Crea un registro de proceso
        /// </summary>
        /// <param name="proceso"></param>
        /// <returns></returns>
        public bool CrearProceso(RAProcesoDC proceso)
        {
            return RARepositorio.Instancia.CrearProceso(proceso);
        }

        /// <summary>
        /// Crea un registro de quien cierra
        /// </summary>
        /// <param name="tipoCierre"></param>
        /// <returns></returns>
        public bool CrearTipoCierre(RATipoCierreDC tipoCierre)
        {
            return
                RARepositorio.Instancia.CrearTipoCierre(tipoCierre);
        }

        /// <summary>
        /// crea un nuevo registro de sistema formato
        /// </summary>
        /// <param name="sistemaFormato"></param>
        /// <returns></returns>
        public bool CrearSistemaFormato(RASistemaFormatoDC sistemaFormato)
        {
            return RARepositorio.Instancia.CrearSistemaFormato(sistemaFormato);
        }

        /// <summary>
        /// Crea una nueva subclasificacion
        /// </summary>
        /// <param name="subClasificacion"></param>
        /// <returns></returns>
        public bool CrearSubClasificacion(RASubClasificacionDC subClasificacion)
        {
            return RARepositorio.Instancia.CrearSubClasificacion(subClasificacion);
        }

        /// <summary>
        /// Crea un nuevo tiempo de ejecucion
        /// </summary>
        /// <param name="tiempoEjecucionRaps"></param>
        /// <returns></returns>
        public bool CrearTiempoEjecucionRaps(RATiempoEjecucionRapsDC tiempoEjecucionRaps)
        {
            return RARepositorio.Instancia.CrearTiempoEjecucionRaps(tiempoEjecucionRaps);
        }

        /// <summary>
        /// Crea un tipo de hora
        /// </summary>
        /// <param name="tipoHora"></param>
        /// <returns></returns>
        public bool CrearTipoHora(RATipoHoraDC tipoHora)
        {
            return RARepositorio.Instancia.CrearTipoHora(tipoHora);
        }

        /// <summary>
        /// Crea un nuevo tipo de periodo
        /// </summary>
        /// <param name="tipoPeriodo"></param>
        /// <returns></returns>
        public bool CrearTipoPeriodo(RATipoPeriodoDC tipoPeriodo)
        {
            return RARepositorio.Instancia.CrearTipoPeriodo(tipoPeriodo);
        }

        /// <summary>
        /// Crea un nuevo tipo de Rap
        /// </summary>
        /// <param name="tipoRap"></param>
        /// <returns></returns>
        public bool CrearTipoRap(RATipoRapDC tipoRap)
        {
            return RARepositorio.Instancia.CrearTipoRap(tipoRap);
        }

        /// <summary>
        /// Inserta las notificaciones que no pudieron ser enviadas al usuario
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        public void InsertarNotificacionPendiente(string mensaje, long idUsuario, long idSolicitud)
        {
            RARepositorio.Instancia.InsertarNotificacionPendiente(mensaje, idUsuario, idSolicitud);
        }

        /// <summary>
        /// crea el grupo con sus correspondientes cargos
        /// </summary>
        /// <param name="grupo"></param>
        public bool CrearGrupoCargo(RACargoGrupoDC grupo)
        {
            return RARepositorio.Instancia.CrearGrupo(grupo);
        }
        #endregion

        #region consultar

        /// <summary>
        /// Listar acciones plantilla parametrizacion raps
        /// </summary>
        /// <returns></returns>
        public List<RAAccionPlantillaParametrizacionRapsDC> ListarAccionPlantillaParametrizacionRaps()
        {
            return RARepositorio.Instancia.ListarAccionPlantillaParametrizacionRaps();
        }

        /// <summary>
        /// Obtiene una plantilla de parametrizacionRaps para una accion
        /// </summary>
        /// <param name="idAccionPlantilla"></param>
        /// <returns></returns>
        public RAAccionPlantillaParametrizacionRapsDC ObtenerAccionPlantillaParametrizacionRaps(long idAccionPlantilla)
        {
            return RARepositorio.Instancia.ObtenerAccionPlantillaParametrizacionRaps(idAccionPlantilla);
        }

        /// <summary>
        /// Lista los estados existentes
        /// </summary>
        /// <returns>Lista estado</returns>
        public List<RAEstadosDC> ListarEstados(bool esResponsable, int idEstadoActual, int idCargoSolicita, long idSolicitud)
        {
            List<RAEstadosDC> lstEstados = new List<RAEstadosDC>();
            if (idCargoSolicita == 0)
            {
                lstEstados = new List<RAEstadosDC>
                {
                    new RAEstadosDC
                    {
                        IdEstado = 6,
                        Descripcion = "Cerrar",
                        Estado = true
                    }
                };
                return lstEstados;
            }

            bool cerrar = false;
            bool cancelar = false;

            if (!esResponsable && ((RAEnumEstados)idEstadoActual == RAEnumEstados.Rechazado || (RAEnumEstados)idEstadoActual == RAEnumEstados.Escalado))
            {
                List<RAGestionDC> listaGestion = RARepositorioSolicitudes.Instancia.ListarGestion(idSolicitud);
                if (listaGestion.Any(g => g.IdEstado == RAEnumEstados.Respuesta))
                {
                    cerrar = true;
                }
                else
                {
                    cancelar = true;
                }
            }

            lstEstados = RARepositorio.Instancia.ListarEstados(esResponsable, idEstadoActual);

            if (cerrar)
            {
                lstEstados.Add(new RAEstadosDC
                {
                    IdEstado = 6,
                    Descripcion = "Cerrar",
                    Estado = true
                });
            }
            if (cancelar)
            {
                lstEstados.Add(new RAEstadosDC
                {
                    IdEstado = 8,
                    Descripcion = "Cancelar",
                    Estado = true
                });
            }

            return lstEstados;
        }

        /// <summary>
        /// Consulta un estado
        /// </summary>
        /// <param name="IdEstado"></param>
        /// <returns>objeto estado</returns>
        public RAEstadosDC ObtenerEstado(int IdEstado)
        {
            return RARepositorio.Instancia.ObtenerEstado(IdEstado);
        }

        /// <summary>
        /// Lista los cargos
        /// </summary>
        /// <returns></returns>
        public List<RACargoDC> ListarCargos()
        {
            return RARepositorio.Instancia.ListarCargos();
        }

        /// <summary>
        /// Listar los cargos
        /// </summary>
        /// <param name="idCargo"></param>
        /// <returns></returns>
        public List<RACargoDC> ObtenerCargos(int idCargo)
        {
            return RARepositorio.Instancia.ObtenerCargos(idCargo);
        }

        /// <summary>
        /// Lista las clasificaciones
        /// </summary>
        /// <returns>Lista la clasificacion</returns>
        public List<RAClasificacionDC> ListarClasificacion()
        {
            return RARepositorio.Instancia.ListarClasificacion();
        }

        /// <summary>
        /// Obtiene una clasificacion
        /// </summary>
        /// <param name="idClasificacion"></param>
        /// <returns>Clasificacion</returns>
        public RAClasificacionDC ObtenerClasificacion(int idClasificacion)
        {
            return RARepositorio.Instancia.ObtenerClasificacion(idClasificacion);
        }

        /// <summary>
        /// Lista los registros de escalonamiento para una parametrizacion
        /// </summary>
        /// <param name="idParametrizacionRap"></param>
        /// <returns>Lista de escalonamiento</returns>
        public List<RAEscalonamientoDC> ListarEscalonamiento(long idParametrizacionRap)
        {
            return RARepositorio.Instancia.ListarEscalonamiento(idParametrizacionRap);
        }

        /// <summary>
        /// lista los item de un flujo
        /// </summary>
        /// <param name="idFlujo"></param>
        /// <returns></returns>
        public List<RAFlujoAccionEstadoDC> ListarFlujoAccionEstado(int idFlujo)
        {
            return RARepositorio.Instancia.ListarFlujoAccionEstado(idFlujo);
        }

        /// <summary>
        /// Obtiene un registro de flujo Accion Estado
        /// </summary>
        /// <param name="idFlujo"></param>
        /// <returns></returns>
        public RAFlujoAccionEstadoDC ObtenerFlujoAccionEstado(int idFlujo, byte idAccion, int idEstado, int idCargo)
        {
            return RARepositorio.Instancia.ObtenerFlujoAccionEstado(idFlujo, idAccion, idEstado, idCargo);
        }

        /// <summary>
        /// lista los formatos
        /// </summary>
        /// <returns></returns>
        public List<RAFormatoDC> ListarFormato()
        {
            return RARepositorio.Instancia.ListarFormato();
        }

        /// <summary>
        /// obitiene un registro de formato
        /// </summary>
        /// <param name="idFormato"></param>
        /// <returns></returns>
        public RAFormatoDC ObtenerFormato(int idFormato)
        {
            return RARepositorio.Instancia.ObtenerFormato(idFormato);
        }

        /// <summary>
        /// Lista los grupos de usuario
        /// </summary>
        /// <returns></returns>
        public List<RAGrupoUsuarioDC> ListarGrupoUsuario()
        {
            return RARepositorio.Instancia.ListarGrupoUsuario();
        }

        /// <summary>
        /// Obtiene un registro de grupo usuario
        /// </summary>
        /// <param name="idGrupoUsuario"></param>
        /// <returns></returns>
        public RAGrupoUsuarioDC ObtenerGrupoUsuario(int idGrupoUsuario)
        {
            return RARepositorio.Instancia.ObtenerGrupoUsuario(idGrupoUsuario);
        }

        /// <summary>
        /// Lista de Parametrizaciones de solicitudes
        /// </summary>
        /// <returns></returns>
        public List<RAParametrizacionRapsDC> ListarParametrizacionRaps()
        {
            return RARepositorio.Instancia.ListarParametrizacionRaps();
        }


        /// <summary>
        /// Lista de Parametrizaciones activas manuales
        /// </summary>
        /// <returns></returns>
        public List<RAParametrizacionRapsDC> ListarParametrizacionRapsManuales()
        {
            return RARepositorio.Instancia.ListarParametrizacionRapsManuales();
        }

        /// <summary>
        /// Lista de Parametrizaciones activas tipo tarea
        /// </summary>
        /// <returns></returns>
        public List<RAParametrizacionRapsDC> ListarParametrizacionRapsTareas()
        {
            return RARepositorio.Instancia.ListarParametrizacionRapsTareas();
        }


        /// <summary>
        /// consulta la informacion de una parametrizacion de solicitudes 
        /// </summary>
        /// <param name="idParametrizacionRap"></param>
        /// <returns></returns>
        public RAParametrizacionRapsDC ObtenerParametrizacionRaps(long idParametrizacionRap)
        {
            return RARepositorio.Instancia.ObtenerParametrizacionRaps(idParametrizacionRap);
        }


        /// <summary>
        /// Lista las plantillas para una accion
        /// </summary>
        /// <returns></returns>
        public List<RAPantillaAccionCorreoDC> ListarPantillaAccionCorreo(byte idAccion)
        {
            return RARepositorio.Instancia.ListarPantillaAccionCorreo(idAccion);
        }

        /// <summary>
        /// Lista los procesos
        /// </summary>
        /// <returns></returns>
        public List<RAProcesoDC> ListarProceso()
        {
            return RARepositorio.Instancia.ListarProceso();
        }

        /// <summary>
        /// Consulta un proceso
        /// </summary>
        /// <param name="idProceso"></param>
        /// <returns></returns>
        public RAProcesoDC ObtenerProceso(int idProceso)
        {
            return RARepositorio.Instancia.ObtenerProceso(idProceso);
        }

        /// <summary>
        /// Lista registros quien cierra
        /// </summary>
        /// <returns></returns>
        public List<RATipoCierreDC> ListarTipoCierre()
        {
            return RARepositorio.Instancia.ListarTipoCierre();
        }

        /// <summary>
        /// consulta la informacion un registro Quien Cierra
        /// </summary>
        /// <param name="idTipoCierre"></param>
        /// <returns></returns>
        public RATipoCierreDC ObtenerTipoCierre(int idTipoCierre)
        {
            return RARepositorio.Instancia.ObtenerTipoCierre(idTipoCierre);
        }

        /// <summary>
        /// lista los registros de sistema formato
        /// </summary>
        /// <returns></returns>
        public List<RASistemaFormatoDC> ListarSistemaFormato()
        {
            return RARepositorio.Instancia.ListarSistemaFormato();
        }

        /// <summary>
        /// Consulta un registro de sistema formato
        /// </summary>
        /// <param name="idSistemaFormato"></param>
        /// <returns></returns>
        public RASistemaFormatoDC ObtenerSistemaFormato(int idSistemaFormato)
        {
            return RARepositorio.Instancia.ObtenerSistemaFormato(idSistemaFormato);
        }

        /// <summary>
        /// Lista las sub clasificaciones de una clasificacion
        /// </summary>
        /// <returns></returns>
        public List<RASubClasificacionDC> ListarSubClasificacion(int idClasificacion)
        {
            return RARepositorio.Instancia.ListarSubClasificacion(idClasificacion);
        }

        /// <summary>
        /// Consulta una subclasificacion
        /// </summary>
        /// <param name="IdSubclasificacion"></param>
        /// <returns></returns>
        public RASubClasificacionDC ObtenerSubClasificacion(int IdSubclasificacion)
        {
            return RARepositorio.Instancia.ObtenerSubClasificacion(IdSubclasificacion);
        }

        /// <summary>
        /// Lista los tiempo de ejecucion para una parametrizacion de Raps
        /// </summary>
        /// <param name="idParametrizacionRap"></param>
        /// <returns></returns>
        public List<RATiempoEjecucionRapsDC> ListarTiempoEjecucionRaps(long idParametrizacionRap)
        {
            return RARepositorio.Instancia.ListarTiempoEjecucionRaps(idParametrizacionRap);
        }

        /// <summary>
        /// Consulta registro tiempo de ejecucion Raps
        /// </summary>
        /// <param name="idEjecucion"></param>
        /// <returns></returns>
        public RATiempoEjecucionRapsDC ObtenerTiempoEjecucionRaps(long idEjecucion)
        {
            return RARepositorio.Instancia.ObtenerTiempoEjecucionRaps(idEjecucion);
        }

        /// <summary>
        /// Lista los tipos de hora
        /// </summary>
        /// <returns></returns>
        public List<RATipoHoraDC> ListarTipoHora()
        {
            return RARepositorio.Instancia.ListarTipoHora(); ;
        }

        /// <summary>
        /// Consulta un tipo de hora
        /// </summary>
        /// <param name="idTipoHora"></param>
        /// <returns></returns>
        public RATipoHoraDC ObtenerTipoHora(int idTipoHora)
        {
            return RARepositorio.Instancia.ObtenerTipoHora(idTipoHora);
        }

        /// <summary>
        /// Lista los tipos de periodos
        /// </summary>
        /// <returns></returns>
        public List<RATipoPeriodoDC> ListarTipoPeriodo()
        {
            return RARepositorio.Instancia.ListarTipoPeriodo();
        }

        /// <summary>
        /// Consulta un tipo de periodo
        /// </summary>
        /// <param name="idTipoPeriodo"></param>
        /// <returns></returns>
        public RATipoPeriodoDC ObtenerTipoPeriodo(int idTipoPeriodo)
        {
            return RARepositorio.Instancia.ObtenerTipoPeriodo(idTipoPeriodo);
        }

        /// <summary>
        /// Lista los tipos de Raps
        /// </summary>
        /// <returns></returns>
        public List<RATipoRapDC> ListarTipoRap()
        {
            return RARepositorio.Instancia.ListarTipoRap();
        }

        /// <summary>
        /// Consulta un tipo de rap
        /// </summary>
        /// <param name="idTipoRap"></param>
        /// <returns></returns>
        public RATipoRapDC ObtenerTipoRap(int idTipoRap)
        {
            return RARepositorio.Instancia.ObtenerTipoRap(idTipoRap);
        }

        /// <summary>
        /// Lista los tipos de Raps activos
        /// </summary>
        /// <returns></returns>
        public List<RATipoRapDC> ListaTipoRapAct()
        {
            var resultado = RARepositorio.Instancia.ListarTipoRap();

            if (resultado == null)
            {
                return null;
            }

            return resultado.FindAll(x => x.Estado.Equals(true));
        }

        /// <summary>
        /// Lista los estados Activos
        /// </summary>
        /// <returns></returns>
        public List<RAEstadosDC> ListarEstadosAct(bool esResponsable, int idEstadoActual)
        {
            var resultado = RARepositorio.Instancia.ListarEstados(esResponsable, idEstadoActual);

            if (resultado == null || resultado.Count <= 0)
            {
                return null;
            }

            return resultado.FindAll(x => x.Estado.Equals(true));
        }

        /// <summary>
        /// Listar origenes de Raps
        /// </summary>
        /// <returns></returns>
        public List<RAOrigenRapsDC> ListarOrigenRaps()
        {
            return RARepositorio.Instancia.ListarOrigenRaps();
        }

        public List<RATipoCierreDC> ListarTipoCierreAct()
        {
            var resultado = RARepositorio.Instancia.ListarTipoCierre();

            if (resultado == null)
            {
                return null;
            }

            return resultado.FindAll(x => x.Estado.Equals(true));
        }

        /// <summary>
        /// Listar las clasificaciones activas
        /// </summary>
        /// <returns></returns>
        public List<RAClasificacionDC> ListarClasificacionAct()
        {
            var resultado = RARepositorio.Instancia.ListarClasificacion();

            if (resultado == null)
            {
                return null;
            }

            return resultado.FindAll(x => x.Estado.Equals(true));
        }

        /// <summary>
        /// Listar las subclasificaciones de la clasificacion dada.
        /// </summary>
        /// <param name="idClasificacion"></param>
        /// <returns></returns>
        public List<RASubClasificacionDC> ListarSubClasificacionAct(int idClasificacion)
        {
            var resultado = RARepositorio.Instancia.ListarSubClasificacion(idClasificacion);

            return resultado == null ? null : resultado.FindAll(x => x.Estado.Equals(true));
        }

        /// <summary>
        /// Listar los sistemas activos
        /// </summary>
        /// <returns></returns>
        public List<RASistemaFormatoDC> ListarSistemaFormatoAct()
        {
            var resultado = RARepositorio.Instancia.ListarSistemaFormato();

            return resultado == null ? null : resultado.FindAll(x => x.Estado.Equals(true));
        }

        /// <summary>
        /// Listar los formatos activos de un sistema
        /// </summary>
        /// <param name="idSistemaFormato"></param>
        /// <returns></returns>
        public List<RAFormatoDC> ListarFormatoAct(int idSistemaFormato)
        {
            var resultado = RARepositorio.Instancia.ListarFormatoAct(idSistemaFormato);

            return resultado == null ? null : resultado.FindAll(x => x.Estado.Equals(true));
        }

        /// <summary>
        /// Lista los grupos de usuario activos
        /// </summary>
        /// <returns></returns>
        public List<RAGrupoUsuarioDC> ListarGrupoUsuarioAct()
        {
            var resultado = RARepositorio.Instancia.ListarGrupoUsuario();

            return resultado == null ? null : resultado.FindAll(x => x.Estado.Equals(true));
        }

        /// <summary>
        /// LIstar los procesos activos
        /// </summary>
        /// <returns></returns>
        public List<RAProcesoDC> ListarProcesosAct()
        {
            var resultado = RARepositorio.Instancia.ListarProceso();

            return resultado == null ? null : resultado.FindAll(x => x.Estado.Equals(true));
        }

        /// <summary>
        /// obtener una accion
        /// </summary>
        /// <param name="idAccion"></param>
        /// <returns></returns>
        public RAAccionDC ObtenerAccion(short idAccion)
        {
            return RARepositorio.Instancia.ObtenerAccion(idAccion);
        }

        /// <summary>
        /// Listar Acciones
        /// </summary>
        /// <returns></returns>
        public List<RAAccionDC> ListarAccion()
        {
            return RARepositorio.Instancia.ListarAccion();
        }

        public List<RAParametrosParametrizacionDC> ListarParametros(long idParametrizacion)
        {
            return RARepositorio.Instancia.ListarParametros(idParametrizacion);
        }

        /// <summary>
        /// retorna los tipos de datos disponibles
        /// </summary>
        /// <returns></returns>
        public List<RATipoDatoDC> ObtenerTiposDatos()
        {
            return RARepositorio.Instancia.ObtenerTiposDatos();
        }

        /// <summary>
        /// lista los tipos de periodo activos
        /// </summary>
        /// <returns></returns>
        public List<RATipoPeriodoDC> ListarTipoPeriodoAct()
        {
            var resultado = RARepositorio.Instancia.ListarTipoPeriodo();

            return resultado == null ? null : resultado.FindAll(x => x.Estado.Equals(true));
        }

        public RAOrigenRapsDC ObtenerOrigenRaps(int idOrigenRaps)
        {
            return RARepositorio.Instancia.ObtenerOrigenRaps(idOrigenRaps);
        }


        /// <summary>
        /// Listar Todos los Tipos de Incumplimiento
        /// </summary>
        /// <returns></returns>
        public List<RATipoIncumplimientoDC> ListarTipoIncumplimiento()
        {
            return RARepositorio.Instancia.ListarTipoIncumplimiento();
        }

        public List<RATipoIncumplimientoDC> ListarTipoIncumplimientoAct()
        {
            var resultado = RARepositorio.Instancia.ListarTipoIncumplimiento();

            return resultado == null ? null : resultado.FindAll(x => x.Estado.Equals(true));
        }

        /// Listar sucursales y cargos para definir escalonamiento
        /// </summary>
        /// <returns></returns>
        public List<RARegionalSuculsalDC> ListarRegionalCargo()
        {
            return RARepositorio.Instancia.ListarRegionalCargo();
        }

        /// <summary>
        /// Obtener Tipo Incumplimiento
        /// </summary>
        /// <param name="idTipoIncumplimiento"></param>
        /// <returns></returns>
        public RATipoIncumplimientoDC ObtenerTipoIncumplimiento(int idTipoIncumplimiento)
        {
            return RARepositorio.Instancia.ObtenerTipoIncumplimiento(idTipoIncumplimiento);
        }

        public List<RAPaginaParametrizacionRapsDC> ListarParametrizacionRapsPaginada(int pagina, int registrosXPagina, int tipoRap, string ordenaPor)
        {
            return RARepositorio.Instancia.ListarParametrizacionRapsPaginada(pagina, registrosXPagina, tipoRap, ordenaPor);
        }

        /// <summary>
        /// Listar Tipo Hora Actual
        /// </summary>
        /// <returns></returns>
        public List<RATipoHoraDC> ListarTipoHoraAct()
        {
            var resultado = RARepositorio.Instancia.ListarTipoHora();

            return resultado == null ? null : resultado.FindAll(x => x.Estado.Equals(true));
        }

        /// <summary>
        /// Listar Origen Raps Activos
        /// </summary>
        /// <returns></returns>
        public List<RAOrigenRapsDC> ListarOrigenRapsAct()
        {
            var resultado = RARepositorio.Instancia.ListarOrigenRaps();

            return resultado == null ? null : resultado.FindAll(x => x.Estado.Equals(true));
        }

        /// <summary>
        /// Listar Cargos Activos
        /// </summary>
        /// <returns></returns>
        public List<RACargoDC> ListarCargosAct()
        {
            var resultado = RARepositorio.Instancia.ListarCargos();

            return resultado == null ? null : resultado.FindAll(x => x.Estado.Equals(true));
        }

        /// <summary>
        /// Listar Personas con cargo de Novasoft
        /// </summary>
        /// <returns></returns>
        public List<RACargoPersonaNovaRapDC> ListarCargoPersonaNova_Rap(RAFiltroCargoPersonaNovaRapDC filtro)
        {
            return RARepositorio.Instancia.ListarCargoPersonaNova_Rap(filtro);
        }

        /// <summary>
        /// Listar Cargo Escalonamiento Parametrización de una Raps
        /// </summary>
        /// <returns></returns>
        public List<RAListarCargoEscalonamientoRapsDC> ListarCargoEscalonamientoRaps()
        {
            return RARepositorio.Instancia.ListarCargoEscalonamientoRaps();
        }

        /// <summary>
        /// ListarCargoEscalonamientoParametrizacionRaps
        /// </summary>
        /// <returns></returns>
        public List<RAEscalonamientoDC> ListarCargoEscalonamientoParametrizacionRaps(int idParametrizacion)
        {
            return RARepositorio.Instancia.ListarCargoEscalonamientoParametrizacionRaps(idParametrizacion);
        }

        /// <summary>
        /// Listar Hora Escalar
        /// </summary>
        /// <returns></returns>
        public List<RAHoraEscalarDC> ListarHoraEscalar()
        {
            return RARepositorio.Instancia.ListarHoraEscalar();
        }

        /// <summary>
        /// Listar Hora Escalar Activos
        /// </summary>
        /// <returns></returns>
        public List<RAHoraEscalarDC> ListarHoraEscalarAct()
        {
            var resultado = RARepositorio.Instancia.ListarHoraEscalar();

            return resultado == null ? null : resultado.FindAll(x => x.Estado.Equals(true));
        }

        /// <summary>
        /// Obtener Hora Escalar
        /// </summary>
        /// <param name="idhoraEscalar"></param>
        /// <returns></returns>
        public RAHoraEscalarDC ObtenerHoraEscalar(int idhoraEscalar)
        {
            return RARepositorio.Instancia.ObtenerHoraEscalar(idhoraEscalar);
        }

        /// <summary>
        /// Metodo para Obtener las territoriales
        /// </summary>
        /// <returns></returns>
        public List<RATerritorialDC> ObtenerTerritoriales()
        {
            return RARepositorio.Instancia.ObtenerTerritoriales();

        }

        /// <summary>
        /// Metodo para obtener las regionales
        /// </summary>
        /// <returns></returns>
        public List<RARegionalSuculsalDC> ObtenerRegionales()
        {
            return RARepositorio.Instancia.ObtenerRegionales();

        }

        /// <summary>
        /// Obtiene los empleados con informacion novasoft
        /// </summary>
        /// <returns></returns>
        public List<RAPersonaDC> ObtenerPersonal(string cargos, int idParametrizacion, int idSucursal)
        {
            return RARepositorio.Instancia.ObtenerEmpleadosNovasoft(cargos, idParametrizacion, idSucursal);
        }

        /// <summary>
        /// Obtiene los menus permitidos para cada usuario 
        /// </summary>
        /// <param name="modulosPermitidos"></param>
        /// <returns></returns>
        public List<RAMenusPermitidosDC> ObtenerMenusPermitidos(List<RAModulosDC> modulosPermitidos)
        {
            List<RAMenusPermitidosDC> result = new List<RAMenusPermitidosDC>();
            List<RAMenusPermitidosDC> lstMenus = new List<RAMenusPermitidosDC>();
            List<int> lstIdMenusPermitidos = new List<int>();
            modulosPermitidos.ForEach(modulo =>
            {
                lstIdMenusPermitidos.Add(modulo.IdModulo);
                modulo.Menu.ForEach(menu =>
                {
                    if (menu.AccionesMenu > 0)
                        lstIdMenusPermitidos.Add(menu.IdMenu);
                });
            });
            lstMenus = RARepositorio.Instancia.ObtenerMenusRaps(modulosPermitidos[0].Identificacion);
            foreach (var menu in lstMenus)
            {
                if (lstIdMenusPermitidos.Exists(lm => lm == menu.IdMenu))
                    result.Add(
                        new RAMenusPermitidosDC
                        {
                            Nombre = menu.Nombre,
                            Icon = menu.Icon,
                            Accion = menu.Accion,
                            Activo = menu.Activo,
                            IdMenu = menu.IdMenu,
                            MenuPadre = menu.MenuPadre,
                            NumeroSolicitudes = menu.NumeroSolicitudes
                        }
                        );
            }

            return result;
        }

        /// <summary>
        /// obtiene los procesos
        /// </summary>
        public List<RAProcesoDC> ObtenerProcesos()
        {
            return RARepositorio.Instancia.ObtenerProcesos();
        }

        /// <summary>
        /// Obtiene los procedimientos de determinado proceso
        /// </summary>
        /// <param name="idProceso"></param>
        /// <returns></returns>
        public List<RAProcedimientoDC> ObtenerProcedimientoPorproceso(int idProceso)
        {
            return RARepositorio.Instancia.ObtenerProcedimientoPorproceso(idProceso);
        }

        /// <summary>
        /// Obtiene los procedimientos de determinados procesos
        /// </summary>
        /// <param name="procesos"></param>
        /// <returns></returns>
        public List<RAProcedimientoDC> ObtenerProcedimientosPorprocesos(string procesos)
        {
            return RARepositorio.Instancia.ObtenerProcedimientosPorprocesos(procesos);
        }

        /// <summary>
        /// Obtiene las solicitudes que tienen como fecha de vencimiento hoy y/o mañana
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>        
        public List<RASolicitudDC> ObtenerSolicitudesProximasAVencer(string idUsuario)
        {
            return RARepositorio.Instancia.ObtenerSolicitudesProximasAVencer(idUsuario);
        }

        /// <summary>
        /// Lista todos los grupos creados
        /// </summary>
        /// <returns></returns>
        public List<RACargoGrupoDC> ListarGrupos()
        {
            return RARepositorio.Instancia.ListarGrupos();
        }

        /// <summary>
        /// Obtiene los niveles de falla para los mensajeros
        /// </summary>
        /// <returns></returns>
        public List<RANivelFallaDC> ObtenerNivelesDeFalla()
        {
            return RARepositorio.Instancia.ObtenerNivelesDeFalla();
        }

        /// <summary>
        /// Obtiene los tipos de escalonamiento
        /// </summary>
        /// <returns></returns>
        public List<RATipoEscalonamientoDC> ObtenerTiposEscalonamiento()
        {
            return RARepositorio.Instancia.ObtenerTiposEscalonamiento();
        }

        /// <summary>
        /// Obtiene todos los tipos de novedad disponibles
        /// </summary>
        /// <returns></returns>
        public List<RAResponsableTipoNovedadDC> ObtenerTodoTipoNovedadDisponible()
        {
            return RARepositorio.Instancia.ObtenerTodoTipoNovedadDisponible();
        }

        /// <summary>
        /// Metodo para obtener los parametros integracion por tipo novedad 
        /// </summary>
        /// <param name="idTipoNovedad"></param>
        /// <returns></returns>
        public List<LIParametrizacionIntegracionRAPSDC> ObtenerParametrosIntegracionPorTipoNovedad(int idTipoNovedad)
        {
            return RARepositorio.Instancia.ObtenerParametrosIntegracionPorTipoNovedad(idTipoNovedad);
        }


        /// <summary>
        /// Obtener parametros fallas personalizadas
        /// </summary>
        /// <param name="idTipoNovedad"></param>
        /// <returns></returns>
        [System.Obsolete()]
        public List<RAParametrosPersonalizacionRapsDC> ListaParametrosPersonalizacionPorNovedad(int idTipoNovedad)
        {
            return RARepositorio.Instancia.ListaParametrosPersonalizacionPorNovedad(idTipoNovedad);
        }

        /// <summary>
        /// Obtiene el cargo con sus respectivos cargos
        /// </summary>
        /// <param name="idGrupo"></param>
        /// <param name="porPersona"></param>
        /// <returns></returns>
        public RACargoGrupoDC ObtenerDetalleGrupo(int idGrupo, bool porPersona)
        {
            return RARepositorio.Instancia.ObtenerDetalleGrupo(idGrupo, porPersona);
        }

        /// <summary>
        /// obtiene las notificaciones pendientes por usuario
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>        
        public List<RANotificacionDC> ListarNotificacionesPendientes(string idUsuario)
        {
            return RARepositorio.Instancia.ListarNotificacionesPendientes(idUsuario);
        }
        #endregion

        #region modificar

        /// <summary>
        /// Actualizar parametrizacion 
        /// </summary>
        /// <param name="idParametrizacion"></param>
        /// <returns></returns>
        public void ActualizarParametrizacionRaps(RAParametrizacionRapsDC parametrizacionRaps, List<RAEscalonamientoDC> listEscalonamiento, List<RATiempoEjecucionRapsDC> listaTiempoEjecucion, List<RAParametrosParametrizacionDC> listaParametros)
        {
            using (TransactionScope transaccion = new TransactionScope())
            {
                //actualizar parametrizacion
                bool ModificoParametrizacion = RARepositorio.Instancia.ModificarParametrizacionRaps(parametrizacionRaps);
                string idsParametrizacion = "";


                if (listEscalonamiento != null)
                {
                    RARepositorio.Instancia.borrarEscalonamiento(parametrizacionRaps.IdParametrizacionRap);

                    listEscalonamiento.ForEach(escalonamiento =>
                    {
                        escalonamiento.IdParametrizacionRap = parametrizacionRaps.IdParametrizacionRap;
                        RARepositorio.Instancia.CrearEscalonamiento(escalonamiento);
                    });
                }

                if (listaTiempoEjecucion != null)
                {
                    RARepositorio.Instancia.borrarTiempoEjecucion(parametrizacionRaps.IdParametrizacionRap);
                    int contadorNumeroEjecucion = 0;
                    listaTiempoEjecucion.ForEach(tiempo =>
                    {
                        contadorNumeroEjecucion = contadorNumeroEjecucion + 1;
                        tiempo.NumeroEjecucion = contadorNumeroEjecucion;
                        tiempo.IdParametrizacionRap = parametrizacionRaps.IdParametrizacionRap;
                        RARepositorio.Instancia.CrearTiempoEjecucionRaps(tiempo);
                    });
                }

                if (listaParametros != null)
                {
                    listaParametros.ForEach(parametro =>
                    {
                        idsParametrizacion = idsParametrizacion + parametro.idParametro + ",";
                        if (parametro.idParametro != null && parametro.idParametro != 0)
                        {
                            RARepositorio.Instancia.ModificarParametroRaps(parametro);
                        }
                        else
                        {
                            RARepositorio.Instancia.InsertarParametrosParametrizacion(parametro);
                        }
                    });

                    idsParametrizacion = idsParametrizacion.TrimEnd(',');
                    RARepositorio.Instancia.EliminarParametros(idsParametrizacion);
                    //borrar
                }
                transaccion.Complete();
            }
        }

        public bool ModificarParametrizacionRaps(RAParametrizacionRapsDC parametrizacionRaps)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Cambia el estado de una parametrizacion Raps
        /// </summary>
        /// <param name="idParametrizacionRap"></param>
        /// <param name="estado"></param>
        /// <returns></returns>
        public bool CambiaEstadoParametrizacionRaps(long idParametrizacionRap, bool estado)
        {
            return RARepositorio.Instancia.CambiaEstadoParametrizacionRaps(idParametrizacionRap, estado);
        }

        /// <summary>
        /// Cambia el estado de una parametrizacion
        /// </summary>
        /// <param name="idParametrizacion"></param>
        /// <param name="estaActivo"></param>
        //public void CambiarEstadoParametrizacion(int idParametrizacion, bool estaActivo)
        //{
        //    RARepositorio.Instancia.CambiarEstadoParametrizacion(idParametrizacion, estaActivo);
        //}

        /// <summary>
        /// Elimina un grupo por su id
        /// </summary>
        /// <param name="idGrupo"></param>
        /// <returns></returns>        
        public bool EliminarGrupo(int idGrupo)
        {
            return RARepositorio.Instancia.EliminarGrupo(idGrupo);
        }

        /// <summary>
        /// modifica los cargos a un grupo ya creado
        /// </summary>
        /// <param name="cargos"></param>        
        public void EditarCargosDeGrupo(RACargoGrupoDC cargos)
        {
            RARepositorio.Instancia.EliminarCargosDeGrupo(cargos.IdGrupo);
            cargos.lstCargo.ForEach(cargo =>
            {
                RARepositorio.Instancia.InsertarGrupoCargo(cargo.IdCargo, cargos.IdGrupo);
            });
        }

        /// <summary>
        /// Edita la informacion de un grupo
        /// </summary>
        /// <param name="infoGrupo"></param>        
        public void EditarGrupo(RACargoGrupoDC infoGrupo)
        {
            RARepositorio.Instancia.EditarGrupo(infoGrupo);
        }

        /// <summary>
        /// Cambia de estato 0 a 1 las notificaciones vistas por un usuario
        /// </summary>
        /// <param name="idUsuario"></param>
        public void ActualizarEstadoNotificacion(string idUsuario)
        {
            RARepositorio.Instancia.ActualizarEstadoNotificacion(idUsuario);
        }
        #endregion

        #region Fallas Interlogis - Web

        /// <summary>
        /// Metodo para obtener los tipos de novedades segun responsable 
        /// </summary>
        /// <param name="idClaseResponsable"></param>
        /// <param name="idSistemaFuente"></param>
        /// <returns></returns>
        public List<RANovedadDC> ObtenerTipoNovedadSegunResponsable(int idClaseResponsable, int idSistemaFuente)
        {
            return RARepositorio.Instancia.ObtenerTipoNovedadSegunResponsable(idClaseResponsable, idSistemaFuente);
        }
        #endregion

    }
}
