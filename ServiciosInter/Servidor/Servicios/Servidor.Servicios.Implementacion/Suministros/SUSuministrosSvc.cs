using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Threading;
using CO.Servidor.Dominio.Comun.Suministros;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using CO.Servidor.Servicios.Contratos;
using CO.Servidor.Suministros;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Servicios.ContratoDatos;
//using System.Transactions;

namespace CO.Servidor.Servicios.Implementacion.Suministros
{
    /// <summary>
    /// Clase para los servicios de suministros
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class SUSuministrosSvc : ISUSuministrosSvc
    {
        public SUSuministrosSvc()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(PAAdministrador.Instancia.ConsultarParametrosFramework("Cultura"));
        }

        /// <summary>
        /// Retorna los suministros asignados a un centro de servicio
        /// </summary>
        /// <param name="centroServicio"></param>
        /// <returns></returns>
        public IEnumerable<SUSuministro> ObtenerSuministrosCentroServicio(PUCentroServiciosDC centroServicio)
        {
            ISUFachadaSuministros fachada = new SUFachadaSuministros();
            return fachada.ObtenerSuministrosCentroServicio(centroServicio);
        }

        /// <summary>
        /// Retorna la lista de los suministros confugurados
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <returns></returns>
        public GenericoConsultasFramework<SUSuministro> ObtenerSuministros(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente)
        {
            int totalRegistros = 0;
            return new GenericoConsultasFramework<SUSuministro>()
            {
                Lista = SUAdministradorSuministros.Instancia.ObtenerSuministros(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Realiza la administracion del suministro
        /// </summary>
        /// <param name="suministro"></param>
        public void AdministrarSuministro(SUSuministro suministro)
        {
            SUAdministradorSuministros.Instancia.AdministrarSuministro(suministro);
        }

        ///// <summary>
        ///// Obtiene todos los suministros
        ///// </summary>
        ///// <returns></returns>
        public List<SUSuministro> ObtenerTodosSuministros()
        {
            return SUAdministradorSuministros.Instancia.ObtenerSuministros();
        }

        /// <summary>
        /// Obtener los suministros activos y que sean para preimprimir
        /// </summary>
        /// <returns></returns>
        public List<SUSuministro> ObtenerSuministrosPreImpresion()
        {
            return SUAdministradorSuministros.Instancia.ObtenerSuministrosPreImpresion();
        }

        /// <summary>
        /// Obtiene el numero del suministro segun el tipo
        /// </summary>
        /// <param name="idSuministro"></param>
        /// <returns></returns>
        public SUNumeradorPrefijo ObtenerNumeroSuministroActual(SUEnumSuministro idSuministro)
        {
            return SUAdministradorSuministros.Instancia.ObtenerNumeroPrefijoValor(idSuministro);              
            
        }

        
        /// <summary>
        /// Retorna las categorias de los suministros
        /// </summary>
        /// <returns></returns>
        public List<SUCategoriaSuministro> ObtenerCategoriasSuministros()
        {
            return SUAdministradorSuministros.Instancia.ObtenerCategoriasSuministros();
        }

        public List<SUSuministro> ObtenerSuministrosSucursal(int idSucursal)
        {
            return SUSuministros.Instancia.ObtenerSuministroSucursal(idSucursal);
        }

        /// <summary>
        /// Obtiene las Sucursales por
        /// contrato y ciudad
        /// </summary>
        /// <param name="idContrato"></param>
        /// <returns>lista de Sucursales</returns>
        public List<CLSucursalDC> ObtenerSucursalesPorContratoCiudad(int idContrato, string idCiudad)
        {
            return SUAdministradorRemisiones.Instancia.ObtenerSucursalesPorContratoCiudad(idContrato, idCiudad);
        }

        #region Aprovisionar suministros de un Centro de servicio

        /// <summary>
        /// Obtener suministros de grupo de un centro de servicio
        /// </summary>
        /// <param name="idGrupo">CSV -PUA -racol</param>
        /// <returns></returns>
        public List<SUSuministro> ObtenerSuministrosGrupoCentroServicio(string idGrupo, long idCentroServicio)
        {
            return SUAdministradorSuministros.Instancia.ObtenerSuministrosGrupoCentroServicio(idGrupo, idCentroServicio);
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
            return SUAdministradorSuministros.Instancia.ObtenerSuministrosCentroServicioNoIncluidosEnGrupo(filtro, idGrupo, idCentroServicio);
        }

        #endregion Aprovisionar suministros de un Centro de servicio

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
        public GenericoConsultasFramework<SUNumeradorPrefijo> ObtenerResolucionesSuministro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente)
        {
            int totalRegistros = 0;
            return new GenericoConsultasFramework<SUNumeradorPrefijo>()
            {
                Lista = SUAdministradorSuministros.Instancia.ObtenerResolucionesSuministro(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Administra la resulucion del suministro
        /// </summary>
        /// <param name="numerador"></param>
        public void AdministrarResolucion(SUNumeradorPrefijo numerador)
        {
            SUAdministradorSuministros.Instancia.AdministrarResolucion(numerador);
        }

        /// <summary>
        /// Obtiene los suministros q aplican resolucion
        /// </summary>
        /// <returns></returns>
        public List<SUSuministro> ObtenerSuministrosResolucion()
        {
            return SUAdministradorSuministros.Instancia.ObtenerSuministrosResolucion();
        }

        #endregion ResolucionSuministro

        #region Grupo suministros

        /// <summary>
        /// Obtiene los suministros de un grupo
        /// </summary>
        /// <param name="idGrupo"></param>
        /// <returns></returns>
        public List<SUSuministro> ObtenerSuministrosAsignadosGrupo(string idGrupo)
        {
            return SUAdministradorSuministros.Instancia.ObtenerSuministrosAsignadosGrupo(idGrupo);
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
        public GenericoConsultasFramework<SUGrupoSuministrosDC> ObtenerGrupoSuministros(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente)
        {
            int totalRegistros = 0;
            return new GenericoConsultasFramework<SUGrupoSuministrosDC>()
            {
                Lista = SUAdministradorSuministros.Instancia.ObtenerGrupoSuministros(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros),
                TotalRegistros = totalRegistros
            };
        }

         public SUDatosResponsableSuministroDC ObtenerResponsableSuministro(long numeroGuia)
        {
            return SUAdministradorSuministros.Instancia.ObtenerResponsableSuministro(numeroGuia);
        }

        /// <summary>
        /// Administra el grupo de suministros
        /// </summary>
        /// <param name="grupo"></param>
        public void AdministrarGrupoSuministros(SUGrupoSuministrosDC grupo)
        {
            SUAdministradorSuministros.Instancia.AdministrarGrupoSuministros(grupo);
        }

        /// <summary>
        /// Obtener suministros de grupo
        /// </summary>
        /// <param name="idGrupo"></param>
        /// <returns></returns>
        public List<SUSuministro> ObtenerSuministrosGrupo(string idGrupo, long idMensajero)
        {
            return SUAdministradorSuministros.Instancia.ObtenerSuministrosGrupo(idGrupo, idMensajero);
        }

        /// <summary>
        /// Obtiene los suministros que no estan incluidos en ningun grupo, ni en en el grupo seleccionado
        /// </summary>
        /// <param name="idGrupo"></param>
        /// <returns></returns>
        public List<SUSuministro> ObtenerSuministrosNoIncluidosEnGrupo(IDictionary<string, string> filtro, string idGrupo, long idMensajero)
        {
            return SUAdministradorSuministros.Instancia.ObtenerSuministrosNoIncluidosEnGrupo(filtro, idGrupo, idMensajero);
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
        public GenericoConsultasFramework<POMensajero> ObtenerMensajerosConductores(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina,
                                                                              int registrosPorPagina, bool ordenamientoAscendente)
        {
            int totalRegistros = 0;
            return new GenericoConsultasFramework<POMensajero>()
            {
                Lista = SUAdministradorSuministros.Instancia.ObtenerMensajerosConductores(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros),
                TotalRegistros = totalRegistros
            };
        }

        #endregion Grupo suministros

        #region Suministros mensajero

        /// <summary>
        /// Administra la informacion del mensajero
        /// </summary>
        /// <param name="grupoMensajero"></param>
        public void AdministrarSuministroMensajero(SUGrupoSuministrosDC grupoMensajero)
        {
            SUAdministradorSuministros.Instancia.AdministrarSuministroMensajero(grupoMensajero);
        }

        #region Aprovisionamiento de suministros

        /// <summary>
        /// Obtiene las ultimas remisiones realizadas
        /// </summary>
        public GenericoConsultasFramework<SURemisionSuministroDC> ObtenerUltimasRemisiones(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente)
        {
            int totalRegistros;
            return new GenericoConsultasFramework<SURemisionSuministroDC>()
            {
                Lista = SUAdministradorSuministros.Instancia.ObtenerUltimasRemisiones(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Obtiene los suministros aprabados para realizar la remision al mensajero
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public List<SUSuministro> ObtenerSuministrosAsignadosMensajero(long idMensajero)
        {
            return SUAdministradorSuministros.Instancia.ObtenerSuministrosAsignadosMensajero(idMensajero);
        }

        /// <summary>
        /// Consulta los mensajeros de la agencia
        /// </summary>
        /// <param name="idAgencia"></param>
        /// <returns></returns>
        public IEnumerable<POMensajero> ObtenerMensajerosAgencia(long idAgencia)
        {
            return SUAdministradorSuministros.Instancia.ObtenerMensajerosAgencia(idAgencia);
        }

        /// <summary>
        /// Valida la asignacion del suministro
        /// </summary>
        /// <param name="suministro"></param>
        public void ValidaSuministroAsignacion(SUSuministro suministro)
        {
            SUAdministradorRemisiones.Instancia.ValidaSuministroAsignacion(suministro);
        }

        /// <summary>
        /// Genera la remision de suministros para el mensajero
        /// </summary>
        /// <param name="remision"></param>
        public long AdminRemisionMensajero(SURemisionSuministroDC remision)
        {
            return SUAdministradorRemisiones.Instancia.AdminRemisionMensajero(remision);
        }

        #endregion Aprovisionamiento de suministros

        #endregion Suministros mensajero

        #region Canal de venta

        /// <summary>
        /// Obtiene los suministros asignados al canal de venta
        /// </summary>
        /// <param name="idCentroServicios"></param>
        /// <returns></returns>
        public List<SUSuministro> ObtenerSuministrosAsignadosCanalVenta(long idCentroServicios)
        {
            return SUAdministradorRemisiones.Instancia.ObtenerSuministrosAsignadosCanalVenta(idCentroServicios);
        }

        /// <summary>
        /// Genera la remision de los suministros para el canal de ventas
        /// </summary>
        /// <param name="remision"></param>
        public long AdminRemisionCanalVenta(SURemisionSuministroDC remision)
        {
            return SUAdministradorRemisiones.Instancia.AdminRemisionCanalVenta(remision);
        }

        #endregion Canal de venta

        #region Clientes

        /// <summary>
        /// Obtiene los clientes activos con una sucursal activa por localidad
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <returns></returns>
        public List<CLClientesDC> ObtenerClientesConSucursalActiva(string idLocalidad)
        {
            return SUAdministradorRemisiones.Instancia.ObtenerClientesConSucursalActiva(idLocalidad);
        }

        /// <summary>
        /// Obtiene las sucursales activas del cliente seleccionado
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public List<CLSucursalDC> ObtenerSucursalesActivasCliente(int idCliente, string idLocalidad)
        {
            return SUAdministradorRemisiones.Instancia.ObtenerSucursalesActivasCliente(idCliente, idLocalidad);
        }

        /// <summary>
        /// Obtiene los suministros asignados a la sucursal
        /// </summary>
        /// <param name="idSucursal"></param>
        public List<SUSuministro> ObtenerSuministrosAsignadoSucursal(int idSucursal)
        {
            return SUAdministradorRemisiones.Instancia.ObtenerSuministrosAsignadoSucursal(idSucursal);
        }

        /// <summary>
        /// Genera la remision de suministros para el mensajero
        /// </summary>
        /// <param name="remision"></param>
        public long AdminRemisionCliente(SURemisionSuministroDC remision)
        {
            return SUAdministradorRemisiones.Instancia.AdminRemisionCliente(remision);
        }

        /// <summary>
        /// Obtiene los contratos activos por cliente
        /// </summary>
        /// <param name="idCliente">lista con los contratios activos por cliente</param>
        /// <returns>lista de contratos</returns>
        public IEnumerable<CLContratosDC> ObtenerContratosActivosClientes(int idCliente)
        {
            return SUAdministradorRemisiones.Instancia.ObtenerContratosActivosClientes(idCliente);
        }

        /// <summary>
        /// Obtiene las Sucursales por
        /// contrato
        /// </summary>
        /// <param name="idContrato">id del contrato</param>
        /// <returns>lista de Sucursales</returns>
        public List<CLSucursalDC> ObtenerSucursalesPorContrato(int idContrato)
        {
            return SUAdministradorRemisiones.Instancia.ObtenerSucursalesPorContrato(idContrato);
        }

        /// <summary>
        /// Obtiene la lista de los contratos de las
        /// sucursales Activas
        /// </summary>
        /// <returns>lista de Contratos</returns>
        public List<CLContratosDC> ObtenerContratosActivosDeSucursales()
        {
            return SUAdministradorRemisiones.Instancia.ObtenerContratosActivosDeSucursales();
        }

        /// <summary>
        /// obtiene el contrato de un cliente en una
        /// ciudad
        /// </summary>
        /// <param name="idCliente">id del cliente</param>
        /// <param name="idCiudad">id de la ciudad</param>
        /// <returns>lista de contratos del cliente en esa ciudad</returns>
        public IEnumerable<CLContratosDC> ObtenerContratosClienteCiudad(int idCliente, string idCiudad)
        {
            return SUAdministradorRemisiones.Instancia.ObtenerContratosClienteCiudad(idCliente, idCiudad);
        }

        #endregion Clientes

        #region desasignacion de suministros

        /// <summary>
        /// Obtiene los grupos de suministros
        /// </summary>
        /// <returns></returns>
        public List<SUGrupoSuministrosDC> ObtenerGruposSuministros()
        {
            return SUAdministradorRemisiones.Instancia.ObtenerGruposSuministros();
        }

        /// <summary>
        /// Realiza la desasignacion de suministros de un mensajero
        /// </summary>
        public void DesasignarSuministrosRemision(SURemisionSuministroDC remision)
        {
            SUAdministradorRemisiones.Instancia.DesasignarSuministrosRemision(remision);
        }

        /// <summary>
        /// Retorna los suministros que esten en el rango de fecha seleccionado
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <returns></returns>
        public List<SURemisionSuministroDC> ObtenerSuministrosRemisionXRangoFecha(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina, string idGrupo)
        {
            return SUAdministradorRemisiones.Instancia.ObtenerSuministrosRemisionXRangoFecha(filtro, indicePagina, registrosPorPagina, idGrupo);
        }

        #endregion desasignacion de suministros

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
            return SUAdministradorSuministros.Instancia.ObtenerProvisionesSuministrosPorRACOL(fechaInicial, fechaFinal, idRacol, idSuministro, pageIndex, pageSize, consultaIncluyeFecha);
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
        /// <returns>Lista con las provisiones de suministro de todos los centros de servicio, mensajeros y agencias por ciudad destino</returns>
        public List<SUImpresionSuministrosDC> ObtenerProvisionesSuministrosCiudadDestino(DateTime? fechaInicial, DateTime? fechaFinal, string idCiudadDestino, int idSuministro, int pageIndex, int pageSize, bool consultaIncluyeFecha)
        {
            return SUAdministradorSuministros.Instancia.ObtenerProvisionesSuministrosCiudadDestino(fechaInicial, fechaFinal, idCiudadDestino, idSuministro, pageIndex, pageSize, consultaIncluyeFecha);
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
            return SUAdministradorSuministros.Instancia.ObtenerProvisionesSuministrosPorCentroServicio(fechaInicial, fechaFinal, idCentroServicio, idSuministro, pageIndex, pageSize);
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
            return SUAdministradorSuministros.Instancia.ObtenerProvisionSuministroPorRango(filtroPorRango);
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
            return SUAdministradorSuministros.Instancia.ObtenerProvisionSuministroPorUsuario(filtroPorUsuario);
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
            return SUAdministradorSuministros.Instancia.ObtenerProvisionSuministroPorRemision(filtroPorRemision);
        }

        /// <summary>
        /// Obtiene los suministros que no estan en el grupo seleccionado ni asignados en la sucursal
        /// </summary>
        /// <param name="idGrupo">id del grupo</param>
        /// <returns>Lista de suministro</returns>
        public List<SUSuministro> ObtenerSuministrosSucursalNoIncluidosEnGrupo(string idGrupo, int idSucursal)
        {
            return SUAdministradorSuministros.Instancia.ObtenerSuministrosSucursalNoIncluidosEnGrupo(idGrupo, idSucursal);
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
            return SUAdministradorSuministros.Instancia.ObtenerRemisionesGuiasInternas(filtroPorRemision);
        }

        #endregion PreImpresion de Suministros

        #region Modificacion suministros

        /// <summary>
        /// Administra la remision del suministro
        /// </summary>
        /// <param name="remisionDestino"></param>
        /// <param name="remisionOrigen"></param>
        public long AdministrarModificacionSuministro(SURemisionSuministroDC remisionDestino, SURemisionSuministroDC remisionOrigen)
        {
            return SUAdministradorRemisiones.Instancia.AdministrarModificacionSuministro(remisionDestino, remisionOrigen);
        }

        #endregion Modificacion suministros

        #region Proceso Suministro

        /// <summary>
        /// Obtiene los grupos de suministros configurados en un proceso
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <returns></returns>

        public GenericoConsultasFramework<SUSuministrosProcesoDC> ObtenerProcesosSuministros(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, long codProceso)
        {
            int totalRegistros = 0;
            return new GenericoConsultasFramework<SUSuministrosProcesoDC>()
            {
                Lista = SUAdministradorSuministros.Instancia.ObtenerProcesosSuministros(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, codProceso),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Obtiene los suministros de un proceso
        /// </summary>
        /// <param name="idSucursal"></param>
        /// <returns></returns>
        public List<SUSuministrosProcesoDC> ObtenerSuministrosProceso(long codProceso)
        {
            return SUAdministradorSuministros.Instancia.ObtenerSuministrosProceso(codProceso);
        }

        /// <summary>
        /// agrega o modifica un suministro de una gestion
        /// </summary>
        /// <param name="sumSuc"></param>
        public void ActualizarSuministroProceso(List<SUSuministrosProcesoDC> sumPro)
        {
            SUAdministradorSuministros.Instancia.ActualizarSuministroProceso(sumPro);
        }

        /// <summary>
        /// Obtiene los suministros que no estan en el grupo seleccionado ni asignados a una gestion
        /// </summary>
        /// <param name="idGrupo">id del grupo</param>
        /// <returns>Lista de suministro</returns>
        public List<SUSuministro> ObtenerSuministrosProcesoNoIncluidosEnGrupo(string idGrupo, long codProceso, Dictionary<string, string> filtro)
        {
            return SUAdministradorSuministros.Instancia.ObtenerSuministrosProcesoNoIncluidosEnGrupo(idGrupo, codProceso, filtro);
        }

        #endregion Proceso Suministro

        /// <summary>
        /// Consulta los parametros de suministros
        /// </summary>
        /// <param name="IdParametro"></param>
        /// <returns></returns>
        public string ObtenerParametrosSuministro(string idParametro)
        {
            return SUAdministradorSuministros.Instancia.ObtenerParametrosSuministro(idParametro);
        }

        /// <summary>
        /// Obtiene los suministros aprabados para realizar la remision al proceso
        /// </summary>
        /// <param name="idProceso"></param>
        /// <returns></returns>
        public List<SUSuministro> ObtenerSuministrosAsignadosProceso(long idProceso)
        {
            return SUAdministradorSuministros.Instancia.ObtenerSuministrosAsignadosProceso(idProceso);
        }

        /// <summary>
        /// Genera la remision de suministros para un proceso
        /// </summary>
        /// <param name="remision"></param>
        public long AdminRemisionProceso(SURemisionSuministroDC remision)
        {
            return SUAdministradorRemisiones.Instancia.AdminRemisionProceso(remision);
        }

        /// <summary>
        /// Almacena el consumo de un suministro en la base de datos
        /// </summary>
        /// <param name="consumoSuministro"></param>
        public void GuardarConsumoSuministro(SUConsumoSuministroDC consumoSuministro)
        {
            SUAdministradorSuministros.Instancia.GuardarConsumoSuministro(consumoSuministro);
        }

        /// <summary>
        /// Retorna el consecutivo del suministro dado
        /// </summary>
        /// <param name="idSuministro"></param>
        /// <returns></returns>
        public long ObtenerConsecutivoSuministro(SUEnumSuministro idSuministro)
        {
            return SUAdministradorSuministros.Instancia.ObtenerConsecutivoSuministro(idSuministro);
        }

        /// <summary>
        /// Obtiene los Correos a notificar la alerta del fallo en
        /// la sincronización a Novasoft de la salida ó
        /// traslado de suministros asignados desde Controller
        /// </summary>
        /// <returns>Lista de Correos</returns>
        public IList<SUCorreoNotificacionesSumDC> ObtenerCorreosNotificacionesSuministro()
        {
            return SUAdministradorSuministros.Instancia.ObtenerCorreosNotificacionesSuministro();
        }

        /// <summary>
        /// Gestiona los mail a la lista de notificaciones
        /// de sincronizacion de Novasoft para Adicionar ó Borrar
        /// </summary>
        /// <param name="email">Correo a Adicionar</param>
        public void GestionarCorreoNotificacionSuministro(ObservableCollection<SUCorreoNotificacionesSumDC> correosGestionar)
        {
            SUAdministradorSuministros.Instancia.GestionarCorreoNotificacionSuministro(correosGestionar);
        }


        /// <summary>
        /// Retorna el propietario de una guía  dada
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public SUPropietarioGuia ObtenerPropietarioSuministro(long numeroSuministro, SUEnumSuministro idSuministro, long idPropietario)
        {
            return SUSuministros.Instancia.ObtenerPropietarioSuministro(numeroSuministro, idSuministro, idPropietario);
        }


        /// <summary>
        /// Retorna el propietario de una guía  dada
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public SUPropietarioGuia ObtenerPropietarioGuia(long numeroSuministro)
        {
            return SUSuministros.Instancia.ObtenerPropietarioGuia(numeroSuministro);
        }


        /// <summary>
        /// Retorna el propietario de un suministro sin validar 
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public SUPropietarioGuia ObtenerPropietarioSuministroSinValidar(long numeroSuministro, SUEnumSuministro idSuministro)
        {
            return SUSuministros.Instancia.ObtenerPropietarioSuministroSinValidar(numeroSuministro, idSuministro);

        }


        /// <summary>
        /// Genera la remision de los suministros para el canal de ventas
        /// </summary>
        /// <param name="remision"></param>
        public SURemisionSuministroDC GenerarRangoGuiaManualOffline(long idCentroServicio, int cantidad)
        {
            return SUAdministradorRemisiones.Instancia.GenerarRangoGuiaManualOffline(idCentroServicio, cantidad);
        }

        /// <summary>
        /// Metodo para obtener los suministros de determinado mensajero
        /// </summary>
        /// <param name="IdMensajero"></param>
        /// <param name="IdSuministro"></param>
        /// <returns></returns>
        public List<long> GenerarSuministrosDisponiblesMensajero(long IdMensajero, long IdSuministro)
        {
            return SUSuministros.Instancia.GenerarSuministrosDisponiblesMensajero(IdMensajero, IdSuministro);
        }

        #region Consolidados
        /// <summary>
        /// Indica si un consolidado dado está activo o inactivo
        /// </summary>
        /// <returns></returns>
        /// <param name="codigo">Código del consolidado</param>
        public string ObtenerEstadoActivoConsolidado(string codigo)
        {
            return SUAdministradorConsolidados.Instancia.ObtenerEstadoActivoConsolidado(codigo);
        }

        /// <summary>
        /// Retorna los tipos de consolidado
        /// </summary>
        /// <returns></returns>
        public List<OUTipoConsolidadoDC> ObtenerTiposConsolidado()
        {
            return SUAdministradorConsolidados.Instancia.ObtenerTiposConsolidado();
        }

        /// <summary>
        /// Retorna los tamaños de la tula
        /// </summary>
        /// <returns></returns>
        public List<SUTamanoTulaDC> ObtenerTamanosTula()
        {
            return SUAdministradorConsolidados.Instancia.ObtenerTamanosTula();
        }

        /// <summary>
        /// Retorna los motivos de cambios de un contenedor
        /// </summary>
        /// <returns></returns>
        public List<SUMotivoCambioDC> ObtenerMotivosCambioContenedor()
        {
            return SUAdministradorConsolidados.Instancia.ObtenerMotivosCambioContenedor();
        }

        /// <summary>
        /// Registra un nuevo contenedor en la base de datos
        /// </summary>
        /// <param name="contenedor"></param>
        public void RegistrarNuevoContenedor(SUConsolidadoDC contenedor)
        {
            SUAdministradorConsolidados.Instancia.RegistrarNuevoContenedor(contenedor);
        }

        /// <summary>
        /// Registra una modificación de un contendor
        /// </summary>
        /// <param name="consolidado"></param>
        public void RegistrarModificacionContenedor(SUModificacionConsolidadoDC consolidado)
        {
            SUAdministradorConsolidados.Instancia.RegistrarModificacionContenedor(consolidado);
        }


        /// <summary>
        /// Retorna Lista Tulas-Contenedores
        /// </summary>
        /// <returns></returns>
        public IList<SUConsolidadoDC> ObtenerListaConsolidados(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina)
        {
            return SUAdministradorConsolidados.Instancia.ObtenerListaConsolidados(filtro, indicePagina, registrosPorPagina);
        }




        #endregion
    }
}