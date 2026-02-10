using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using CO.Servidor.Suministros.Administracion;
using CO.Servidor.Suministros.PreImpresion;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;

namespace CO.Servidor.Suministros
{
  /// <summary>
  /// Fachada para la administración de suministros
  /// </summary>
  public class SUAdministradorSuministros : ControllerBase
  {
    private static readonly SUAdministradorSuministros instancia = (SUAdministradorSuministros)FabricaInterceptores.GetProxy(new SUAdministradorSuministros(), COConstantesModulos.MODULO_SUMINISTROS);

    /// <summary>
    /// Retorna una instancia de administrador de suministros
    /// /// </summary>
    public static SUAdministradorSuministros Instancia
    {
      get { return SUAdministradorSuministros.instancia; }
    }

    /// <summary>
    /// Retorna instancia del configurador de suministros
    /// </summary>
    private CO.Servidor.Suministros.Administracion.SUConfigurador Configurador
    {
      get
      {
        return new SUConfigurador();
      }
    }

    /// <summary>
    /// Retorna la instancia de la pre impresion de suministros
    /// </summary>
    internal SUPreImpresion PreImpresion
    {
      get
      {
        return new SUPreImpresion();
      }
    }

    /// <summary>
    /// Obtiene todos los sumnistros configurados
    /// </summary>
    /// <returns></returns>
    public List<SUSuministro> ObtenerSuministros(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
    {
      return Configurador.ObtenerSuministros(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
    }

    /// <summary>
    /// Retorna las categorias de los suministros
    /// </summary>
    /// <returns></returns>
    public List<SUCategoriaSuministro> ObtenerCategoriasSuministros()
    {
      return Configurador.ObtenerCategoriasSuministros();
    }

    /// <summary>
    /// Realiza la administracion del suministro
    /// </summary>
    /// <param name="suministro"></param>
    public void AdministrarSuministro(SUSuministro suministro)
    {
      Configurador.AdministrarSuministro(suministro);
    }

    /// <summary>
    /// Obtiene todos los suministros
    /// </summary>
    /// <returns></returns>
    public List<SUSuministro> ObtenerSuministros()
    {
      return Configurador.ObtenerSuministros();
    }

    /// <summary>
    /// Obtiene los grupos de suministros configurados
    /// </summary>
    /// <param name="filtro"></param>
    /// <param name="campoOrdenamiento"></param>
    /// <param name="indicePagina"></param>
    /// <param name="registrosPorPagina"></param>
    /// <param name="ordenamientoAscendente"></param>
    /// <param name="totalRegistros"></param>
    /// <returns></returns>
    public List<SUGrupoSuministrosDC> ObtenerGrupoSuministrosConSuminGrupo(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
    {
      return Configurador.ObtenerGrupoSuministrosConSuminGrupo(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
    }

    /// <summary>
    /// Obtener los suministros activos y que sean para preimprimir
    /// </summary>
    /// <returns></returns>
    public List<SUSuministro> ObtenerSuministrosPreImpresion()
    {
      return Configurador.ObtenerSuministrosPreImpresion();
    }

    /// <summary>
    /// Almacena el consumo de un suministro en la base de datos
    /// </summary>
    /// <param name="consumoSuministro"></param>
    public void GuardarConsumoSuministro(SUConsumoSuministroDC consumoSuministro)
    {
      SUSuministros.Instancia.GuardarConsumoSuministro(consumoSuministro);
    }

    #region ResolucionSuministro

    /// <summary>
    /// Retorna las resoluciones de suministros
    /// </summary>
    /// <param name="filtro"></param>
    /// <param name="campoOrdenamiento"></param>
    /// <param name="indicePagina"></param>
    /// <param name="registrosPorPagina"></param>
    /// <param name="ordenamientoAscendente"></param>
    /// <param name="totalRegistros"></param>
    /// <returns></returns>
    public List<SUNumeradorPrefijo> ObtenerResolucionesSuministro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
    {
      return Configurador.ObtenerResolucionesSuministro(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
    }

    /// <summary>
    /// Administra la resulucion del suministro
    /// </summary>
    /// <param name="numerador"></param>
    public void AdministrarResolucion(SUNumeradorPrefijo numerador)
    {
      Configurador.AdministrarResolucion(numerador);
    }

    /// <summary>
    /// Obtiene los suministros q aplican resolucion
    /// </summary>
    /// <returns></returns>
    public List<SUSuministro> ObtenerSuministrosResolucion()
    {
      return Configurador.ObtenerSuministrosResolucion();
    }

    #endregion ResolucionSuministro

    #region Grupo suministro

    /// <summary>
    /// Obtiene los suministros de un grupo
    /// </summary>
    /// <param name="idGrupo"></param>
    /// <returns></returns>
    public List<SUSuministro> ObtenerSuministrosAsignadosGrupo(string idGrupo)
    {
      return Configurador.ObtenerSuministrosAsignadosGrupo(idGrupo);
    }

    /// <summary>
    /// Obtiene los grupos de suministros configurados
    /// </summary>
    /// <param name="filtro"></param>
    /// <param name="campoOrdenamiento"></param>
    /// <param name="indicePagina"></param>
    /// <param name="registrosPorPagina"></param>
    /// <param name="ordenamientoAscendente"></param>
    /// <param name="totalRegistros"></param>
    /// <returns></returns>
    public List<SUGrupoSuministrosDC> ObtenerGrupoSuministros(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
    {
      return Configurador.ObtenerGrupoSuministros(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
    }

    /// <summary>
    /// Administra el grupo de suministros
    /// </summary>
    /// <param name="grupo"></param>
    public void AdministrarGrupoSuministros(SUGrupoSuministrosDC grupo)
    {
      Configurador.AdministrarGrupoSuministros(grupo);
    }

    /// <summary>
    /// Obtener suministros de grupo
    /// </summary>
    /// <param name="idGrupo"></param>
    /// <returns></returns>
    public List<SUSuministro> ObtenerSuministrosGrupo(string idGrupo, long idMensajero)
    {
      return Configurador.ObtenerSuministrosGrupo(idGrupo, idMensajero);
    }

    /// <summary>
    /// Obtiene los suministros que no estan incluidos en ningun grupo, ni en en el grupo seleccionado
    /// </summary>
    /// <param name="idGrupo"></param>
    /// <returns></returns>
    public List<SUSuministro> ObtenerSuministrosNoIncluidosEnGrupo(IDictionary<string, string> filtro, string idGrupo, long idMensajero)
    {
      return Configurador.ObtenerSuministrosNoIncluidosEnGrupo(filtro, idGrupo, idMensajero);
    }

    /// <summary>
    /// Obtiene los mensajeros y conductores configurados
    /// </summary>
    /// <param name="filtro"></param>
    /// <param name="campoOrdenamiento"></param>
    /// <param name="indicePagina"></param>
    /// <param name="registrosPorPagina"></param>
    /// <param name="ordenamientoAscendente"></param>
    /// <param name="totalRegistros"></param>
    /// <returns></returns>
    public IEnumerable<POMensajero> ObtenerMensajerosConductores(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina,
                                                                          int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
    {
      return Configurador.ObtenerMensajerosConductores(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
    }

    #endregion Grupo suministro

    #region Suministros mensajero

    /// <summary>
    /// Administra la informacion del mensajero
    /// </summary>
    /// <param name="grupoMensajero"></param>
    public void AdministrarSuministroMensajero(SUGrupoSuministrosDC grupoMensajero)
    {
      Configurador.AdministrarSuministroMensajero(grupoMensajero);
    }

    #endregion Suministros mensajero

    #region Aprovisionamiento de suministros

    /// <summary>
    /// Obtiene las ultimas remisiones realizadas
    /// </summary>
    public List<SURemisionSuministroDC> ObtenerUltimasRemisiones(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
    {
      return Configurador.ObtenerUltimasRemisiones(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
    }

        public SUDatosResponsableSuministroDC ObtenerResponsableSuministro(long numeroGuia)
        {
            return Configurador.ObtenerResponsableSuministro(numeroGuia);
        }

        /// <summary>
        /// Obtiene los suministros aprabados para realizar la remision al mensajero
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public List<SUSuministro> ObtenerSuministrosAsignadosMensajero(long idMensajero)
    {
      return Configurador.ObtenerSuministrosAsignadosMensajero(idMensajero);
    }

    /// <summary>
    /// Consulta los mensajeros de la agencia
    /// </summary>
    /// <param name="idAgencia"></param>
    /// <returns></returns>
    public IEnumerable<POMensajero> ObtenerMensajerosAgencia(long idAgencia)
    {
      return Configurador.ObtenerMensajerosAgencia(idAgencia);
    }

    #endregion Aprovisionamiento de suministros

    #region Aprovisionar suministros de un Centro de servicio

    /// <summary>
    /// Guardar los suministros que posee un centro de servicio
    /// </summary>
    /// <param name="suministroCentroServicio"></param>
    public void GuardarSuministroCentroServicio(SUSuministroCentroServicioDC suministroCentroServicio)
    {
      Configurador.GuardarSuministroCentroServicio(suministroCentroServicio);
    }

    /// <summary>
    /// Obtener suministros de grupo de un centro de servicio
    /// </summary>
    /// <param name="idGrupo">CSV -PUA -racol</param>
    /// <returns></returns>
    public List<SUSuministro> ObtenerSuministrosGrupoCentroServicio(string idGrupo, long idCentroServicio)
    {
      return Configurador.ObtenerSuministrosGrupoCentroServicio(idGrupo, idCentroServicio);
    }

    /// <summary>
    /// Obtiene los suministros que no estan en el grupo seleccionado ni asignados en el centro de servicio
    /// </summary>
    /// <param name="filtro">codigo erp(novasoft) y descripcion del suministro</param>
    /// <param name="idGrupo">id del grupo</param>
    /// <param name="idCentroServicio">id del centro de servicio</param>
    /// <returns>Lista de suministro</returns>
    public List<SUSuministro> ObtenerSuministrosCentroServicioNoIncluidosEnGrupo(IDictionary<string, string> filtro, string idGrupo, long idCentroServicio)
    {
      return Configurador.ObtenerSuministrosCentroServicioNoIncluidosEnGrupo(filtro, idGrupo, idCentroServicio);
    }

    #endregion Aprovisionar suministros de un Centro de servicio

    #region PreImpresion de Suministros

    /// <summary>
    /// Obtener las provisiones de un racol si estas tienen rangos consecutivos
    /// </summary>
    /// <param name="fechaInicial">fecha inicial de la consulta</param>
    /// <param name="fechaFinal">fecha final de la consulta</param>
    /// <param name="idRacol">Id del RACOL</param>
    /// <param name="idSuministro">id del suministro a consultar</param>
    /// <param name="pageIndex">indice de la pagina</param>
    /// <param name="pageSize">Tamaño de la pagina</param>
    /// <returns>Lista con las provisiones de suministro de todos los centros de servicio, mensajeros y agencias</returns>
    public List<SUImpresionSuministrosDC> ObtenerProvisionesSuministrosPorRACOL(DateTime? fechaInicial, DateTime? fechaFinal, long idRacol, int idSuministro, int pageIndex, int pageSize, bool consultaIncluyeFecha)
    {
      return PreImpresion.ObtenerProvisionesSuministrosPorRACOL(fechaInicial, fechaFinal, idRacol, idSuministro, pageIndex, pageSize,consultaIncluyeFecha);
    }

    /// <summary>
    /// Obtener las provisiones de un racol si estas tienen rangos consecutivos por ciudad destino
    /// </summary>
    /// <param name="fechaInicial">fecha inicial de la consulta</param>
    /// <param name="fechaFinal">fecha final de la consulta</param>
    /// <param name="idRacol">Id del RACOL</param>
    /// <param name="idSuministro">id del suministro a consultar</param>
    /// <param name="pageIndex">indice de la pagina</param>
    /// <param name="pageSize">Tamaño de la pagina</param>
    /// <returns>Lista con las provisiones de suministro de todos los centros de servicio, mensajeros y agencias por ciudad de destino</returns>
    public List<SUImpresionSuministrosDC> ObtenerProvisionesSuministrosCiudadDestino(DateTime? fechaInicial, DateTime? fechaFinal, string idCiudadDestino, int idSuministro, int pageIndex, int pageSize, bool consultaIncluyeFecha)
    {
      return PreImpresion.ObtenerProvisionesSuministrosCiudadDestino(fechaInicial, fechaFinal, idCiudadDestino, idSuministro, pageIndex, pageSize, consultaIncluyeFecha);
    }

    /// <summary>
    /// Obtener las provisiones de un Centro de servicio si estas tienen rangos consecutivos
    /// </summary>
    /// <param name="fechaInicial">fecha inicial de la consulta</param>
    /// <param name="fechaFinal">fecha final de la consulta</param>
    /// <param name="idCentroServicio">Id Centro Servicio</param>
    /// <param name="idSuministro">id del suministro a consultar</param>
    /// <param name="pageIndex">indice de la pagina</param>
    /// <param name="pageSize">Tamaño de la pagina</param>
    /// <returns>Lista con las provisiones de suministro de todos los centros de servicio, mensajeros y agencias</returns>
    public SUImpresionSuministrosDC ObtenerProvisionesSuministrosPorCentroServicio(DateTime? fechaInicial, DateTime? fechaFinal, long idCentroServicio, int idSuministro, int pageIndex, int pageSize)
    {
      return PreImpresion.ObtenerProvisionesSuministrosPorCentroServicio(fechaInicial, fechaFinal, idCentroServicio, idSuministro, pageIndex, pageSize);
    }

    /// <summary>
    /// Obtiene las provisiones de suministros en los rangos ingresados por parametros
    /// en un rango de fechas
    /// </summary>
    /// <param name="fechaInicial">fecha inicial de la consulta por defecto deja la actual menos 3 dias</param>
    /// <param name="fechaFinal">fecha final de la consulta por defecto deja la actual</param>
    /// <param name="rangoInicial">rango inicial</param>
    /// <param name="rangoFinal">rango final</param>
    /// <param name="pageIndex">indice de la pagina</param>
    /// <param name="pageSize">tamaño de la paginacion</param>
    /// <returns>Lista con la informacion para impresión los suministro</returns>
    public SUImpresionSuministrosDC ObtenerProvisionSuministroPorRango(SUFiltroSuministroPorRangoDC filtroPorRango)
    {
      return PreImpresion.ObtenerProvisionSuministroPorRango(filtroPorRango);
    }

     /// <summary>
    /// Obtiene las provisiones de suministros realizados por un usuario
    /// en un rango de fechas
    /// </summary>
    /// <param name="fechaInicial">fecha inicial de la consulta por defecto deja la actual menos 3 dias</param>
    /// <param name="fechaFinal">fecha final de la consulta por defecto deja la actual</param>
    /// <param name="rangoInicial">rango inicial</param>
    /// <param name="rangoFinal">rango final</param>
    /// <param name="pageIndex">indice de la pagina</param>
    /// <param name="pageSize">tamaño de la paginacion</param>
    /// <returns>Lista con la informacion para impresión los suministro</returns>
    public SUImpresionSuministrosDC ObtenerProvisionSuministroPorUsuario(SUFiltroSuministroPorUsuarioDC filtroPorUsuario)
    {
      return PreImpresion.ObtenerProvisionSuministroPorUsuario(filtroPorUsuario);
    }

    /// <summary>
    /// Obtiene informacion de las provisiones de un suministro por el numero de remision
    /// </summary>
    /// <param name="fechaInicial">fecha inicial de la consulta por defecto deja la actual menos 3 dias</param>
    /// <param name="fechaFinal">fecha final de la consulta por defecto deja la actual</param>
    /// <param name="usuario">Usuario que realizo la remision</param>
    /// <param name="remisionInicial">numero de la remision inicial</param>
    /// <param name="remisionFinal">numero de la remision final</param>
    /// <param name="pageIndex">indice de la pagina</param>
    /// <param name="pageSize">tamaño de la paginacion</param>
    /// <returns>Lista con la informacion para impresion los suministro de un Centro Servicio</returns>
    public SUImpresionSuministrosDC ObtenerProvisionSuministroPorRemision(SUFiltroSuministroPorRemisionDC filtroPorRemision)
    {
      return PreImpresion.ObtenerProvisionSuministroPorRemision(filtroPorRemision);
    }

    /// <summary>
    /// Obtiene informacion de las provisiones de un suministro por el numero de remision
    /// </summary>
    /// <param name="fechaInicial">fecha inicial de la consulta por defecto deja la actual menos 3 dias</param>
    /// <param name="fechaFinal">fecha final de la consulta por defecto deja la actual</param>
    /// <param name="usuario">Usuario que realizo la remision</param>
    /// <param name="remisionInicial">numero de la remision inicial</param>
    /// <param name="remisionFinal">numero de la remision final</param>
    /// <param name="pageIndex">indice de la pagina</param>
    /// <param name="pageSize">tamaño de la paginacion</param>
    /// <returns>Lista con la informacion para impresion los suministro de un Centro Servicio</returns>
    public List<SURemisionSuministroDC> ObtenerRemisionesGuiasInternas(SUFiltroSuministroPorRemisionDC filtroPorRemision)
    {
      return PreImpresion.ObtenerRemisionesGuiasInternas(filtroPorRemision);
    }

    #endregion PreImpresion de Suministros

    #region Sucursal Suministro

    /// <summary>
    /// Obtiene los suministros de una sucursal
    /// </summary>
    /// <param name="idSucursal"></param>
    /// <returns></returns>
    public List<SUSuministroSucursalDC> ObtenerSuministrosSucursal(int idSucursal)
    {
      return SUSuministros.Instancia.ObtenerSuministrosSucursal(idSucursal);
    }

    /// <summary>
    /// agrega o modifica un suministro de una sucursal
    /// </summary>
    /// <param name="sumSuc"></param>
    public void AgregarModificarSuministroSucursal(List<SUSuministroSucursalDC> sumSuc)
    {
      SUSuministros.Instancia.AgregarModificarSuministroSucursal(sumSuc);
    }

    /// <summary>
    /// Obtiene los suministros que no estan en el grupo seleccionado ni asignados en la sucursal
    /// </summary>
    /// <param name="idGrupo">id del grupo</param>
    /// <returns>Lista de suministro</returns>
    public List<SUSuministro> ObtenerSuministrosSucursalNoIncluidosEnGrupo(string idGrupo, int idSucursal)
    {
      return SUSuministros.Instancia.ObtenerSuministrosSucursalNoIncluidosEnGrupo(idGrupo, idSucursal);
    }

    #endregion Sucursal Suministro

    #region Procesos Suministro

    /// <summary>
    /// Obtiene los grupos de suministros configurados para los procesos
    /// </summary>
    /// <param name="filtro"></param>
    /// <param name="campoOrdenamiento"></param>
    /// <param name="indicePagina"></param>
    /// <param name="registrosPorPagina"></param>
    /// <param name="ordenamientoAscendente"></param>
    /// <param name="totalRegistros"></param>
    /// <returns></returns>
    public List<SUSuministrosProcesoDC> ObtenerProcesosSuministros(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long CodProceso)
    {
      return SUSuministros.Instancia.ObtenerProcesosSuministros(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, CodProceso);
    }

    /// <summary>
    /// Obtiene los suministros de un proceso
    /// </summary>
    /// <param name="idSucursal"></param>
    /// <returns></returns>
    public List<SUSuministrosProcesoDC> ObtenerSuministrosProceso(long codProceso)
    {
      return SUSuministros.Instancia.ObtenerSuministrosProceso(codProceso);
    }

    /// <summary>
    /// agrega o modifica un suministro de un proceso
    /// </summary>
    /// <param name="sumPro"></param>
    public void ActualizarSuministroProceso(List<SUSuministrosProcesoDC> sumPro)
    {
      SUSuministros.Instancia.AgregarModificarSuministroProceso(sumPro);
    }

    /// <summary>
    /// Obtiene los suministros que no estan en el grupo seleccionado ni asignados a un proceso
    /// </summary>
    /// <param name="idGrupo">id del grupo</param>
    /// <returns>Lista de suministro</returns>
    public List<SUSuministro> ObtenerSuministrosProcesoNoIncluidosEnGrupo(string idGrupo, long codProceso, Dictionary<string, string> filtro)
    {
      return SUSuministros.Instancia.ObtenerSuministrosProcesoNoIncluidosEnGrupo(idGrupo, codProceso, filtro);
    }

    #endregion Procesos Suministro

    /// <summary>
    /// Consulta los parametros de suministros
    /// </summary>
    /// <param name="IdParametro"></param>
    /// <returns></returns>
    public string ObtenerParametrosSuministro(string idParametro)
    {
      return SUSuministros.Instancia.ObtenerParametrosSuministro(idParametro);
    }

    /// <summary>
    /// Obtiene los suministros aprabados para realizar la remision al proceso
    /// </summary>
    /// <param name="idProceso"></param>
    /// <returns></returns>
    public List<SUSuministro> ObtenerSuministrosAsignadosProceso(long idProceso)
    {
      return SUSuministros.Instancia.ObtenerSuministrosAsignadosProceso(idProceso);
    }

    /// <summary>
    /// Obtiene los Correos a notificar la alerta del fallo en
    /// la sincronización a Novasoft de la salida ó
    /// traslado de suministros asignados desde Controller
    /// </summary>
    /// <returns>Lista de Correos</returns>
    public IList<SUCorreoNotificacionesSumDC> ObtenerCorreosNotificacionesSuministro()
    {
      return SUSuministros.Instancia.ObtenerCorreosNotificacionesSuministro();
    }

    /// <summary>
    /// Gestiona los mail a la lista de notificaciones
    /// de sincronizacion de Novasoft para Adicionar ó Borrar
    /// </summary>
    /// <param name="email">Correo a Adicionar</param>
    public void GestionarCorreoNotificacionSuministro(ObservableCollection<SUCorreoNotificacionesSumDC> correosGestionar)
    {
      SUSuministros.Instancia.GestionarCorreoNotificacionSuministro(correosGestionar);
    }

        /// <summary>
        /// Obtiene el numero  prefijo + valorActual
        /// </summary>
        /// <param name="idSuministro">id del suministro</param>
        /// <returns>numero del giro</returns>
        public SUNumeradorPrefijo ObtenerNumeroPrefijoValor(SUEnumSuministro idSuministro)
        {
            return SUSuministros.Instancia.ObtenerNumeroPrefijoValor(idSuministro);
        }


        /// <summary>
        /// Retorna el consecutivo dle suministro dado
        /// </summary>
        /// <param name="idSuministro"></param>
        /// <returns></returns>
        public long ObtenerConsecutivoSuministro(SUEnumSuministro idSuministro)
        {
            return SUSuministros.Instancia.ObtenerConsecutivoSuministro(idSuministro);
        }
    }
}