using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Transactions;
using System.Web;
using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Dominio.Comun.Cajas;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.OperacionUrbana;
using CO.Servidor.Dominio.Comun.ParametrosOperacion;
using CO.Servidor.OperacionUrbana.Comun;
using CO.Servidor.OperacionUrbana.Datos;
using CO.Servidor.OperacionUrbana.PruebaEntrega;
using CO.Servidor.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Cajas;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using CO.Servidor.Dominio.Comun.AdmEstadosGuia;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.LogisticaInversa.Datos;
using CO.Servidor.Dominio.Comun.CentroAcopio;

namespace CO.Servidor.OperacionUrbana
{
    /// <summary>
    /// Fachada para la administración de operación urbana
    /// </summary>
    public class OUAdministradorOperacionUrbana
    {
        #region Campos

        private static readonly OUAdministradorOperacionUrbana instancia = new OUAdministradorOperacionUrbana();

        #endregion Campos

        #region Propiedades

        /// <summary>
        /// Retorna una instancia del administrador de operacion urbana
        /// </summary>
        public static OUAdministradorOperacionUrbana Instancia
        {
            get { return OUAdministradorOperacionUrbana.instancia; }
        }


        private IADFachadaAdmisionesMensajeria fachadaMensajeria = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>();

        private IPUFachadaCentroServicios fachadaCes = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();

        private ICAFachadaCentroAcopio fachadaCentroAcopio = COFabricaDominio.Instancia.CrearInstancia<ICAFachadaCentroAcopio>();

        #endregion Propiedades

        #region Mensajeros

        /// <summary>
        /// Adiciona, edita o elimina un mensajero
        /// </summary>
        /// <param name="mensajero"></param>
        public void ActualizarMensajero(OUMensajeroDC mensajero)
        {
            OUMensajero.Instancia.ActualizarMensajero(mensajero);
        }

        /// <summary>
        /// Obtiene la lista de tipos de mensajero
        /// </summary>
        /// <returns></returns>
        public IEnumerable<OUTipoMensajeroDC> ObtenerTiposMensajero()
        {
            return OUMensajero.Instancia.ObtenerTiposMensajero();
        }

        /// <summary>
        /// Retorna los estados de los mensajeros
        /// </summary>
        /// <returns></returns>
        public IList<OUEstadosMensajeroDC> ObtenerEstadosMensajero()
        {
            return OUMensajero.Instancia.ObtenerEstadosMensajero();
        }

        /// <summary>
        /// Consulta si existe la persona en la bd de nova soft y retorna la informacion de la persona
        /// </summary>
        /// <param name="identificacion">Documento de identificacion</param>
        /// <param name="contratista">Tipo de vinculacion (Contratista o tercero)</param>
        /// <returns></returns>
        public OUMensajeroDC ConsultaExisteMensajero(string identificacion, bool contratista)
        {
            return OUMensajero.Instancia.ConsultaExisteMensajero(identificacion, contratista);
        }

        /// <summary>
        /// Retorna los mensajeros del centro logistico
        /// </summary>
        /// <param name="idCentroLogistico"></param>
        /// <returns></returns>
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
            return OURepositorio.Instancia.ObtenerNombreMensajeroAgencia(idAgencia);
        }

        /// <summary>
        /// Obtene los datos de los mensajeros de una agencia a partir de un punto de servicio.
        /// </summary>
        /// <param name="idAgencia">Es el id agencia.</param>
        /// <returns>la lista de mensajeros de una agencia</returns>
        public IEnumerable<OUNombresMensajeroDC> ObtenerMensajerosAgenciaDesdePuntoServicio(long idPuntoServicio)
        {
            var agencia = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerAgenciaResponsable(idPuntoServicio);

            //la validacion de agencia==null se hace en el metodo ObtenerAgenciaResponsable
            return ObtenerNombreMensajeroAgencia(agencia.IdResponsable);


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
        /// Método para obtener los mensajeros por localidad
        /// </summary>
        /// <param name="Localidad"></param>
        /// <returns></returns>
        public IEnumerable<POMensajero> ObtenerMensajerosLocalidad(string Localidad)
        {
            return OURepositorio.Instancia.ObtenerMensajerosLocalidad(Localidad);
        }

        /// <summary>
        /// Obtiene los mensajeros de una punto especifico
        /// </summary>
        /// <param name="centroLogistico"></param>
        /// <returns></returns>
        public IList<OUMensajeroDC> ObtenerMensajerosAuditores(long puntoServicio, bool esAgencia)
        {
            if (fachadaCes.ObtenerCentroLogistico().Where(ces => ces.IdCentroservicio == puntoServicio).Any())
            {
                esAgencia = false;
            }
            else
            {
                esAgencia = true;
            }
            return OURepositorio.Instancia.ObtenerMensajerosAuditores(puntoServicio, esAgencia);
        }

        #endregion Mensajeros

        #region Centro de Acopio

        /// <summary>
        /// obtener la informacion del mensajero del centro logistico
        /// </summary>
        /// <param name="centroLogistico"></param>
        /// <returns></returns>

        public OUGuiaIngresadaDC ObtenerInfoMensajero(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long centroLogistico)
        {
            return OURepositorio.Instancia.ObtenerInfoMensajero(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, centroLogistico);
        }

        /// <summary>
        /// Obtiene los estados de los empaques para mensajeria y carga
        /// </summary>
        /// <returns></returns>
        public List<OUEstadosEmpaqueDC> ObtenerEstadosEmpaque()
        {
            return OUManejadorIngreso.Instancia.ObtenerEstadosEmpaque();
        }

        /// <summary>
        /// Obtener la informacion de un mensajero por medio de su identificador
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns>Datos mensajero</returns>
        public OUDatosMensajeroDC ObtenerDatosMensajero(long idMensajero)
        {
            return OUManejadorIngreso.Instancia.ObtenerDatosMensajero(idMensajero);
        }

        /// <summary>
        /// obtener id mensajero por identificacion
        /// </summary>
        /// <param name="identificacionMensajero"></param>
        /// <returns></returns>
        public int ObtenerIdMensajeroPorIdentificacion(string identificacionMensajero)
        {
            return OUManejadorIngreso.Instancia.ObtenerIdMensajeroPorIdentificacion(identificacionMensajero);
        }


        /// <summary>
        /// Obtiene los mensajeros de un centro logistico en una lista paginada
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <param name="centroLogistico"></param>
        /// <returns></returns>
        public IList<OUMensajeroDC> ObtenerMensajeroCentroLogistico(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long centroLogistico)
        {
            return OUManejadorIngreso.Instancia.ObtenerMensajeroCentroLogistico(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, centroLogistico);
        }

        /// <summary>
        /// Obtiene todos los mensajeros de un col
        /// </summary>
        /// <param name="idCol"></param>
        /// <returns></returns>
        public List<OUMensajeroDC> ObtenerMensajerosCol(long idCol)
        {
            return OUManejadorIngreso.Instancia.ObtenerMensajerosCol(idCol);
        }

        /// <summary>
        /// Obtiene la lista de los mensajeros de un centro logistico
        /// </summary>
        /// <param name="centroLogistico"></param>
        /// <returns></returns>
        public IList<OUMensajeroDC> ObtenerMensajeroPorAgencia(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long puntoServicio)
        {
            return OURepositorio.Instancia.ObtenerMensajeroPorAgencia(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, puntoServicio);
        }

        /// <summary>
        /// Guarda y valida la guia ingresada
        /// </summary>
        /// <param name="guiaIngresada"></param>
        public OUGuiaIngresadaDC GuardarIngreso(OUGuiaIngresadaDC guiaIngresada)
        {
            return OUManejadorIngreso.Instancia.GuardarIngreso(guiaIngresada);
        }

        /// <summary>
        /// Obtiene el total de los envios pendientes asignados por planilla de venta al mensajero
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public int ObtenerTotalEnviosPendientes(long idMensajero)
        {
            return OUManejadorIngreso.Instancia.ObtenerTotalEnviosPendientes(idMensajero);
        }

        /// <summary>
        /// retorna el total de los envios planillados por mensajero
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public int TotalEnviosPlanillados(long idMensajero)
        {
            return OUManejadorIngreso.Instancia.TotalEnviosPlanillados(idMensajero);
        }

        /// <summary>
        /// Consulta la guía a partir del numero de guía ingresado
        /// </summary>
        /// <param name="guiaIngresada">Guia Ingresada para validaciones</param>
        /// <returns>Guía consultada</returns>
        public OUGuiaIngresadaDC ConsultaGuia(OUGuiaIngresadaDC guiaIngresada)
        {
            return OUManejadorIngreso.Instancia.ConsultaGuia(guiaIngresada);
        }

        /// <summary>
        /// Obtiene las guias pendientes por descargar del mensajero
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public List<OUGuiasPendientesDC> ObtenerEnviosPendientes(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long idMensajero)
        {
            return OURepositorio.Instancia.ObtenerEnviosPendientes(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idMensajero);
        }

        #endregion Centro de Acopio

        #region Planilla ventas

        /// <summary>
        /// Obtiene los mensajeros del racol
        /// </summary>
        /// <param name="centroLogistico"></param>
        /// <returns></returns>
        public IList<OUMensajeroDC> ObtenerMensajeroPorRegional(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long idRacol)
        {
            return OUPlanillaVentas.Instancia.ObtenerMensajeroPorRegional(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idRacol);
        }

        /// <summary>
        /// Obtiene una lista de las asignaciones de tulas y precintos por punto de servicio,  y por estado
        /// </summary>
        /// <param name="idPuntoServicio"></param>
        /// <returns></returns>
        public List<OUAsignacionDC> ObtenerAsignacionTulaPrecintoPuntoServicio(long idPuntoServicio, string estadoAsignacion)
        {
            return OUPlanillaVentas.Instancia.ObtenerAsignacionTulaPrecintoPuntoServicio(idPuntoServicio, estadoAsignacion);
        }

        /// <summary>
        /// Obtiene las planillas por centro de servicios
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <param name="idCentroServicios">Id del centro de servicios</param>
        /// <returns>Lista con las planillas del punto</returns>
        public List<OUPlanillaVentaDC> ObtenerPlanillasPorCentroServicios(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long idCentroServicios)
        {
            return OUPlanillaVentas.Instancia.ObtenerPlanillasPorCentroServicios(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idCentroServicios);
        }

        /// <summary>
        /// Consulta si un punto de servicio tiene almenos una planilla de recoleccion en punto (planilla ventas)  abierta
        /// </summary>
        /// <returns></returns>
        public bool ValidarPlanillasAbiertasPorPuntoVenta(long idPuntoServicio)
        {
            return OUPlanillaVentas.Instancia.ValidarPlanillasAbiertasPorPuntoVenta(idPuntoServicio);
        }

        /// <summary>
        /// Elimina una guia de un consolidado en la planilla de ventas
        /// </summary>
        /// <param name="idAdmisionMensajeria"></param>
        /// <param name="idPlanilla"></param>
        public OUPlanillaVentaGuiasDC EliminarGuiaConsolidadoPlanillaVentas(OUPlanillaVentaGuiasDC guiaPlanilla)
        {
            return OUPlanillaVentas.Instancia.EliminarGuiaConsolidadoPlanillaVentas(guiaPlanilla);
        }

        /// <summary>
        /// Elimina una guia o un rotulo de una planilla de ventas
        /// </summary>
        /// <param name="idAdmisionMensajeria"></param>
        /// <param name="idPlanilla"></param>
        public OUPlanillaVentaGuiasDC EliminarGuiaPlanillaVentas(OUPlanillaVentaGuiasDC guiaPlanilla)
        {
            return OUPlanillaVentas.Instancia.EliminarGuiaPlanillaVentas(guiaPlanilla);
        }

        /// <summary>
        /// Obtiene las asignaciones para imprimir y cierra la planilla
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <param name="idCentroServicios"></param>
        /// <returns></returns>
        public List<OUImpresionPlanillaVentasDC> ObtenerCerrarImpresionPlanillaVentasTotal(OUPlanillaVentaDC planilla, long idCentroServicios)
        {
            return OUPlanillaVentas.Instancia.ObtenerCerrarImpresionPlanillaVentasTotal(planilla, idCentroServicios);
        }

        /// <summary>
        /// Obtiene las asignaciones para imprimir y cierra la planilla parcial
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <param name="idCentroServicios"></param>
        /// <returns></returns>
        public List<OUImpresionPlanillaVentasDC> ObtenerCerrarImpresionPlanillaVentasParcial(long idPlanilla, long idCentroServicios)
        {
            return OUPlanillaVentas.Instancia.ObtenerCerrarImpresionPlanillaVentasParcial(idPlanilla, idCentroServicios);
        }

        /// <summary>
        /// Obtiene las asignaciones para imprimir la planilla
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <param name="idCentroServicios"></param>
        /// <returns></returns>
        public OUImpresionPlanillaVentasDC ObtenerImpresionManifiestoPlanillaVentas(long idPlanilla, long idCentroServicios)
        {
            return OUPlanillaVentas.Instancia.ObtenerImpresionManifiestoPlanillaVentas(idPlanilla, idCentroServicios);
        }

        /// <summary>
        /// Obtiene las asignaciones para imprimir la planilla
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <param name="idCentroServicios"></param>
        /// <returns></returns>
        public List<OUImpresionPlanillaVentasDC> ObtenerImpresionPlanillaVentasSinCerrar(long idPlanilla, long idCentroServicios)
        {
            return OUPlanillaVentas.Instancia.ObtenerImpresionPlanillaVentasSinCerrar(idPlanilla, idCentroServicios);
        }

        /// <summary>
        /// Adiciona una guia a una planilla, si la planilla no existe crea una nueva y retorna el numero de la planilla
        /// </summary>
        /// <param name="planilla"></param>
        /// <param name="guiaPlanilla"></param>
        /// <returns></returns>
        public long AdicionarGuiaPlanilla(OUPlanillaVentaDC planilla, OUPlanillaVentaGuiasDC guiaPlanilla)
        {
            return OUPlanillaVentas.Instancia.AdicionarGuiaPlanilla(planilla, guiaPlanilla);
        }

        public long CrearPlanillaVentas(OUPlanillaVentaDC planilla, List<OUPlanillaVentaGuiasDC> LstGuias)
        {
            return OUPlanillaVentas.Instancia.CrearPlanillaVentas(planilla, LstGuias);
        }

        /// <summary>
        /// Adiciona una guia suelta a una planilla, si la planilla no existe crea una nueva y retorna el numero de la planilla
        /// </summary>
        /// <param name="planilla"></param>
        /// <param name="guiaPlanilla"></param>
        /// <returns></returns>
        public long AdicionarGuiaSueltaPlanilla(OUPlanillaVentaDC planilla, OUPlanillaVentaGuiasDC guiaPlanilla)
        {

            return OUPlanillaVentas.Instancia.AdicionarGuiaSueltaPlanilla(planilla, guiaPlanilla);
        }


        /// <summary>
        /// Obtiene las guias del punto de servicios
        /// </summary>
        /// <param name="idCentroServicios">Id del centro o punto de servicios</param>
        /// <returns>Lista con las guias del punto sin planillar</returns>
        public List<OUPlanillaVentaGuiasDC> ObtenerGuiasPorPuntoDeServicios(long idCentroServicios)
        {
            return OUPlanillaVentas.Instancia.ObtenerGuiasPorPuntoDeServicios(idCentroServicios);
        }

        /// <summary>
        /// Guarda la planilla de venta y las guias de la planilla
        /// </summary>
        /// <param name="planilla">Informacion de la planilla</param>
        /// <param name="guiasPlanilla">Lista con las Guias de la planilla</param>
        public long GuardarPlanillaVentas(OUPlanillaVentaDC planilla, List<OUPlanillaVentaGuiasDC> guiasPlanilla)
        {
            return OUPlanillaVentas.Instancia.GuardarPlanillaVentas(planilla, guiasPlanilla);
        }
        /// <summary>
        /// Obtiene las guias asignadas a una planilla y a una asignacion tula
        /// </summary>
        /// <param name="idPlanillaVentas"> id de la planilla de ventas</param>
        /// <param name="idAsignacionTula">Id de la asignacion tula</param>
        /// <returns>Lista con las guias asignadas</returns>
        public List<OUGuiaIngresadaDC> ObtenerGuiasPorPlanillaAsignacionTula(long idPlanillaVentas, long idAsignacionTula)
        {
            return OUPlanillaVentas.Instancia.ObtenerGuiasPorPlanillaAsignacionTula(idPlanillaVentas, idAsignacionTula);

        }

        /// <summary>
        /// Obtiene todas las guias sueltas planilladas
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <returns></returns>
        public List<OUGuiaIngresadaDC> ObtenerGuiasSueltasPlanilladas(long idPlanilla)
        {
            return OUPlanillaVentas.Instancia.ObtenerGuiasSueltasPlanilladas(idPlanilla);
        }

        /// <summary>
        /// Despacha la falla para las guias q no fueron planilladas por el punto de servicios
        /// </summary>
        /// <param name="idCentroServicios"></param>
        public void EnviaFallaConGuiasNoPlanilladas(long idCentroServicios, string nombreCentroServicios, string direccionCentroServicios)
        {
            OUPlanillaVentas.Instancia.EnviaFallaConGuiasNoPlanilladas(idCentroServicios, nombreCentroServicios, direccionCentroServicios);
        }

        #endregion Planilla ventas

        #region Planilla Asignación

        #region Consultas

        /// <summary>
        /// Retorna las planillas de asignacion del envios del centro logistico
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <param name="idCentroServicios"></param>
        /// <returns></returns>
        public List<OUPlanillaAsignacionDC> ObtenerPlanillasAsignacionCentroLogistico(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, long idCentroServicios, bool incluyeFecha)
        {
            return OUPlanillaAsignacionEnvios.Instancia.ObtenerPlanillasAsignacionCentroLogistico(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, idCentroServicios, incluyeFecha);
        }

        /// <summary>
        /// Retorna los mensajeros asociados al col y filtrados por tipo
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <param name="idCentroServicios">Id centro servicios</param>
        /// <param name="idTipoMensajero">Tipo de mensajero</param>
        /// <returns></returns>
        public List<OUMensajeroDC> ObtenerMensejorPorColYTipoMensajero(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long idCentroServicios, int idTipoMensajero)
        {
            return OUPlanillaAsignacionEnvios.Instancia.ObtenerMensejorPorColYTipoMensajero(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idCentroServicios, idTipoMensajero);
        }

        /// <summary>
        /// Retorna los envios de la planilla de asignacion enviada
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <returns></returns>
        public List<OUGuiaIngresadaDC> ObtenerEnviosPlanillaAsignacion(long idPlanilla)
        {
            return OUPlanillaAsignacionEnvios.Instancia.ObtenerEnviosPlanillaAsignacion(idPlanilla);
        }

        /// <summary>
        /// Obtiene las guias de determinado mensajero en determinado dia
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <param name="fechaPlanilla"></param>
        /// <returns>las guias de determinado mensajero con determinada fecha</returns>
        public List<OUGuiaIngresadaDC> ObtenerGuiasPlanilladasPorDiaYMensajero(long idMensajero, DateTime fechaPlanilla)
        {
            return OUPlanillaAsignacionEnvios.Instancia.ObtenerGuiasPlanilladasPorDiaYMensajero(idMensajero, fechaPlanilla);
        }

        /// <summary>
        /// Verifica el soat y la revision tecnomecanica del mensajero
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns>
        /// true = si el soat y la tecnomecanica estan vigentes
        /// false = si el soat y la tecnomecanica estan vencidos
        /// </returns>
        public bool VerificaMensajeroSoatTecnoMecanica(long idMensajero)
        {
            return OUPlanillaAsignacionEnvios.Instancia.VerificaMensajeroSoatTecnoMecanica(idMensajero);
        }

        /// <summary>
        /// Obtiene el total de los envios pendientes del mensajero
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public int ObtenerEnviosPendientesMensajero(long idMensajero)
        {
            return OUPlanillaAsignacionEnvios.Instancia.ObtenerEnviosPendientesMensajero(idMensajero);
        }

        /// <summary>
        /// Retorna los estados de la planilla
        /// </summary>
        /// <returns></returns>
        public IEnumerable<OUEstadosPlanillaAsignacionDC> ObtenerEstadosPlanillaAsignacion()
        {
            return OUPlanillaAsignacionEnvios.Instancia.ObtenerEstadosPlanillaAsignacion();
        }

        /// <summary>
        /// Indica si una guía, dado su número y el id de la agencia, ya ha sido ingresada a centro de acopio pero no había sido creada la guía como tal al momento del ingreso
        /// </summary>
        /// <param name="numeroGuia"></param>.
        /// <param name="idAgencia"></param>
        /// <returns>True si la guía ya fué ingresada</returns>
        public bool GuiaYaFueIngresadaACentroDeAcopio(long numeroGuia, long idAgencia)
        {
            return OUPlanillaAsignacionEnvios.Instancia.GuiaYaFueIngresadaACentroDeAcopio(numeroGuia, idAgencia);
        }

        /// <summary>
        /// Indica si una guía, dado su número ya ha sido ingresada a centro de acopio pero no habiá sido creada en el sistema
        /// </summary>
        /// <param name="numeroguia"></param>
        /// <returns>Retorna el número de la agencia uqe hizo el ingreso</returns>
        public long GuiaYaFueIngresadaACentroDeAcopioRetornaCentroAcopio(long numeroGuia)
        {
            return OUPlanillaAsignacionEnvios.Instancia.GuiaYaFueIngresadaACentroDeAcopioRetornaCentroAcopio(numeroGuia);
        }

        #endregion Consultas

        #region Inserción

        public OUPlanillaAsignacionDC GuardarPlanillaAsignacionEnvio(OUPlanillaAsignacionDC planilla)
        {
            return OUPlanillaAsignacionEnvios.Instancia.GuardarPlanillaAsignacionEnvio(planilla);
        }

        #endregion Inserción

        #region Actualizacion

        /// <summary>
        /// Modifica una planilla de asignacion de envios
        /// </summary>
        /// <param name="planilla"></param>
        public void ModificarPlanillaAsignacionEnvio(OUPlanillaAsignacionDC planilla)
        {
            OUPlanillaAsignacionEnvios.Instancia.ModificarPlanillaAsignacionEnvio(planilla);
        }

        /// <summary>
        /// Actualiza el total de los envios planillados
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <param name="totalGuias"></param>
        public void ActualizaTotalEnviosPlanillados(long idPlanilla, int totalGuias)
        {
            OUPlanillaAsignacionEnvios.Instancia.ActualizaTotalEnviosPlanillados(idPlanilla, totalGuias);
        }

        /// <summary>
        /// Actualiza el total de los envios planillados
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <param name="totalGuias"></param>
        public void ActualizaTotalEnviosPlanillados(long idPlanilla)
        {
            OUPlanillaAsignacionEnvios.Instancia.ActualizaTotalEnviosPlanillados(idPlanilla);
        }

        /// <summary>
        /// verifica el envio seleccionado
        /// </summary>
        /// <param name="planillaAsignacion"></param>
        /// <param name="guia"></param>
        public void ActualizaEnvioVerificadoPlanillaAsignacion(long planillaAsignacion, long numGuia)
        {
            OUPlanillaAsignacionEnvios.Instancia.ActualizaEnvioVerificadoPlanillaAsignacion(planillaAsignacion, numGuia);
        }

        /// <summary>
        /// Cierra la planilla de asignación
        /// </summary>
        /// <param name="idPlanilla"></param>
        public void CerrarPlanillaAsignacion(long idPlanilla)
        {
            OUPlanillaAsignacionEnvios.Instancia.CerrarPlanillaAsignacion(idPlanilla);
        }

        /// <summary>
        /// Abrir Planilla de asignacion
        /// </summary>
        /// <param name="idPlanilla"></param>
        public void AbrirPlanillaAsignacion(long idPlanilla)
        {
            OUPlanillaAsignacionEnvios.Instancia.AbrirPlanillaAsignacion(idPlanilla);
        }

        /// <summary>
        /// Asigna el mensajero a la planilla seleccionada
        /// </summary>
        /// <param name="planilla">Planilla de asignacion</param>
        public void AsignaMensajeroPlanilla(OUPlanillaAsignacionDC planilla)
        {
            OUPlanillaAsignacionEnvios.Instancia.AsignaMensajeroPlanilla(planilla);
        }

        #endregion Actualizacion

        #region Eliminacion

        /// <summary>
        /// Elimina un envio de la planilla de asignacion
        /// </summary>
        /// <param name="idAdmisionMensajeria"></param>
        public void EliminarEnvioPlanillaAsignacion(OUPlanillaAsignacionDC planilla, long idAdmisionMensajeria)
        {
            OUPlanillaAsignacionEnvios.Instancia.EliminarEnvioPlanillaAsignacion(planilla, idAdmisionMensajeria);
        }

        /// <summary>
        /// Valida si el envío está asignado a una planilla
        /// </summary>
        /// <param name="idGuia"></param>
        /// <returns></returns>
        public OUGuiaIngresadaDC ConsultaEnvioPlanillaAsignacionGuia(long idGuia)
        {
            return OUPlanillaAsignacionEnvios.Instancia.ConsultaEnvioPlanillaAsignacionGuia(idGuia);
        }


        /// <summary>
        /// Reasigna la guia seleccionada
        /// </summary>
        /// <param name="planilla"></param>
        /// <param name="idPlanillaNueva"></param>
        /// <param name="idAdmisionMensajeria"></param>
        public void ReasignarEnvioPlanilla(long planilla, long idPlanillaNueva, long idAdmisionMensajeria, long idAgencia)
        {
            OUPlanillaAsignacionEnvios.Instancia.ReasignarEnvioPlanilla(planilla, idPlanillaNueva, idAdmisionMensajeria, idAgencia);
        }

        #endregion Eliminacion

        #endregion Planilla Asignación

        #region Solicitud Recogidas

        /// <summary>
        /// Obtiene los estados de la solicitud de recogida
        /// </summary>
        /// <returns></returns>
        public List<OUEstadosSolicitudRecogidaDC> ObtenerEstadosRecogida()
        {
            return OURecogidas.Instancia.ObtenerEstadosRecogida();
        }

        /// <summary>
        /// Obtiene las recogidas de la agencia
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="idAgencia"></param>
        /// <param name="incluyeFechaAsignacion"></param>
        /// <param name="incluyeFechaRecogida"></param>
        /// <returns></returns>
        public List<OURecogidasDC> ObtenerRecogidas(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, long idAgencia, bool incluyeFechaAsignacion, bool incluyeFechaRecogida)
        {
            return OURecogidas.Instancia.ObtenerRecogidas(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, idAgencia, incluyeFechaAsignacion, incluyeFechaRecogida);
        }

        /// <summary>
        /// Guarda la solicitud de recogida del punto de servicios
        /// </summary>
        /// <param name="recogida"></param>
        public void GuardaSolicitudRecogidaPuntoSvc(OURecogidasDC recogida)
        {
            OURecogidas.Instancia.GuardaSolicitudRecogidaPuntoSvc(recogida);
        }

        /// <summary>
        /// Guarda la solicitud de la recogida por cliente convenio
        /// </summary>
        /// <param name="recogida"></param>
        public void GuardarSolicitudClienteConvenio(OURecogidasDC recogida)
        {
            OURecogidas.Instancia.GuardarSolicitudClienteConvenio(recogida);
        }

        /// <summary>
        /// Guarda la solicitud de recogida del cliente peaton
        /// </summary>
        /// <param name="recogida"></param>
        public long GuardaSolicitudClientePeaton(OURecogidasDC recogida)
        {
            return OURecogidas.Instancia.GuardaSolicitudClientePeaton(recogida);

        }


        public void ActualizaSolicitudClientePeaton(OURecogidasDC recogida)
        {
            try
            {
                OURecogidas.Instancia.ActualizaSolicitudClientePeaton(recogida);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<OURecogidasDC> ObtenerDireccionesPeaton(OURecogidaPeatonDC Peaton)
        {
            return OURecogidas.Instancia.ObtenerDireccionesPeaton(Peaton);
        }

        /// <summary>
        /// Metodo para obtener ultima informacion del usuario externo
        /// </summary>
        /// <param name="identificacion"></param>
        /// <returns></returns>
        public OURecogidasDC ObtenerInformacionRecogidaUsuarioExterno(string nomUsuario)
        {
            return OURecogidas.Instancia.ObtenerInformacionRecogidaUsuarioExterno(nomUsuario);

        }

        /// <summary>
        /// Consulta la solicitud de recogida para un cliente peaton especifico
        /// </summary>
        /// <returns></returns>
        public OURecogidasDC ObtenerInformacionClientePeaton(OURecogidasDC infoRecogida)
        {
            return OURecogidas.Instancia.ObtenerInformacionClientePeaton(infoRecogida);
        }

        public void RegistrarSolicitudRecogidaMovil(long idRecogida, long idDispositivoMovil)
        {
            OURecogidas.Instancia.RegistrarSolicitudRecogidaMovil(idRecogida, idDispositivoMovil);
        }

        public PADispositivoMovil ObtenerdispositivoMovilClienteRecogida(long idRecogida)
        {
            return OURecogidas.Instancia.ObtenerdispositivoMovilClienteRecogida(idRecogida);
        }


        /// <summary>
        /// Obtiene todas las recogidas pendientes de peaton del dia actual, filtradas por un rango de tiempo para la hora de recogida
        /// </summary>
        /// <returns></returns>
        public List<OURecogidasDC> ObtenerRecogidasPeatonPendientesDia()
        {
            return OURecogidas.Instancia.ObtenerRecogidasPeatonPendientesDia();
        }

        /// <summary>
        /// Obtiene todas las recogidas pendientes de peaton del dia actual, filtradas por un rango de tiempo para la hora de recogida y localidad de recogida
        /// </summary>
        /// <returns></returns>
        public List<OURecogidasDC> ObtenerRecogidasPeatonPendientesPorProgramarDia(string idLocalidad)
        {
            return OURecogidas.Instancia.ObtenerRecogidasPeatonPendientesPorProgramarDia(idLocalidad);
        }

        /// <summary>
        /// Obtiene todas las solicitudes de recogida disponibles por localidad.
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <returns></returns>
        public List<OURecogidasDC> ObtenerRecogidasDisponiblesPeatonDia(string idLocalidad)
        {
            return OURecogidas.Instancia.ObtenerRecogidasDisponiblesPeatonDia(idLocalidad);
        }

        /// <summary>
        /// Obtiene todas las recogidas de peaton pendientes por programas
        /// </summary>
        /// <returns></returns>
        public List<OURecogidasDC> ObtenerTodasRecogidasPeatonPendientesPorProgramar()
        {
            return OURecogidas.Instancia.ObtenerTodasRecogidasPeatonPendientesPorProgramar();
        }

        /// <summary>
        /// Agrega una programacion y una planilla a una solicitud de recogida
        /// </summary>
        /// <param name="programacion"></param>
        public void AgregarProgramacionSolicitudRecogida(OUProgramacionSolicitudRecogidaDC programacion)
        {
            OURecogidas.Instancia.AgregarProgramacionSolicitudRecogida(programacion);
        }

        /// <summary>
        /// Asigna una solicitud de recogida a un mensajero
        /// </summary>
        /// <returns></returns>
        public long AsignarRecogidaMensajero(OUAsignacionRecogidaMensajeroDC asignacion)
        {
            return OURecogidas.Instancia.AsignarRecogidaMensajero(asignacion);
        }

        /// <summary>
        /// Guarda las notificaciones enviadas por cada recogida
        /// </summary>
        /// <param name="idSolicitudRecogida"></param>
        public void GuardarNotificacionRecogida(long idSolicitudRecogida)
        {
            OURecogidas.Instancia.GuardarNotificacionRecogida(idSolicitudRecogida);
        }

        /// <summary>
        /// Obtiene los datos del usuario que está solicitando 
        /// la recogida si ya se ha registrado antes.
        /// </summary>
        /// <param name="tipoid"></param>
        /// <param name="identificacion"></param>
        /// <returns></returns>
        public PAPersonaInternaDC ObtenerInfoUsuarioRecogida(string tipoid, string identificacion)
        {
            return OURecogidas.Instancia.ObtenerInfoUsuarioRecogida(tipoid, identificacion);
        }

        #endregion Solicitud Recogidas

        #region Programacion de recogidas

        #region Consultas

        /// <summary>
        /// Retorna los motivos de la Reprogramacion
        /// </summary>
        /// <returns></returns>
        public List<OUMotivosReprogramacionDC> ObtenerMotivosReprogramacion()
        {
            return OURecogidas.Instancia.ObtenerMotivosReprogramacion();
        }

        /// <summary>
        /// Retorna la recogida por punto de servicio
        /// </summary>
        /// <param name="idSolicitud"></param>
        /// <returns></returns>
        public OURecogidasDC ObtenerRecogidaPuntoServicio(long idSolicitud)
        {
            return OURecogidas.Instancia.ObtenerRecogidaPuntoServicio(idSolicitud);
        }

        /// <summary>
        /// Retorna la recogida del cliente convenio
        /// </summary>
        /// <param name="idSolicitud"></param>
        /// <returns></returns>
        public OURecogidasDC ObtenerRecogidaConvenio(long idSolicitud)
        {
            return OURecogidas.Instancia.ObtenerRecogidaConvenio(idSolicitud);
        }

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
        /// Retorna el historico de la programacion de la recogida
        /// </summary>
        /// <param name="idSolicitud"></param>
        /// <returns></returns>
        public List<OUProgramacionSolicitudRecogidaDC> ObtenerProgramacionRecogidas(long idSolicitud)
        {
            return OURecogidas.Instancia.ObtenerProgramacionRecogidas(idSolicitud);
        }

        /// <summary>
        /// Obtiene las planillas de recogidas creadas para el tipo de mensajero, zona y fecha de recogidas
        /// </summary>
        /// <param name="idZona">id de la zona</param>
        /// <param name="idTipoMensajero">id del tipo de mensajero</param>
        /// <param name="fechaRecogida">fecha de recogida</param>
        /// <returns></returns>
        public List<OUProgramacionSolicitudRecogidaDC> ObtenerPlanillasRecogidaZonaTipoMenFecha(string idZona, int idTipoMensajero, DateTime fechaRecogida, long idCol)
        {
            return OURecogidas.Instancia.ObtenerPlanillasRecogidaZonaTipoMenFecha(idZona, idTipoMensajero, fechaRecogida, idCol);
        }

        /// <summary>
        /// Obtiene los mensajero por centro logistico y tipo
        /// </summary>
        /// <param name="idCentroLogistico"></param>
        /// <param name="idTipoMensajero"></param>
        /// <returns></returns>
        public List<OUNombresMensajeroDC> ObtenerMensajerosPorTipo(long idCentroLogistico, int idTipoMensajero)
        {
            return OURepositorio.Instancia.ObtenerMensajerosPorTipo(idCentroLogistico, idTipoMensajero);
        }

        #endregion Consultas

        #region insert

        /// <summary>
        /// Actualiza el reporte de la recogida al mensajero
        /// </summary>
        /// <param name="programacion"></param>
        public void ActualizaReporteMensajero(OUProgramacionSolicitudRecogidaDC programacion)
        {
            OURecogidas.Instancia.ActualizaReporteMensajero(programacion);
        }

        /// <summary>
        /// Actualiza la solicitud de la recogida y la planilla
        /// </summary>
        /// <param name="programacion"></param>
        public void ActualizaSolicitudRecogida(OUProgramacionSolicitudRecogidaDC programacion)
        {
            OURecogidas.Instancia.ActualizaSolicitudRecogida(programacion);
        }

        #endregion insert

        #region planillas de Recogidas

        /// <summary>
        /// Obtiene las planillas de recogidas por centro logistico
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="idCol"></param>
        /// <param name="incluyeFecha"></param>
        /// <returns></returns>
        public List<OUProgramacionSolicitudRecogidaDC> ObtenerPlanillasRecogidas(Dictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, long idCol, bool incluyeFecha)
        {
            return OURecogidas.Instancia.ObtenerPlanillasRecogidas(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, idCol, incluyeFecha);
        }

        /// <summary>
        /// Obtiene la programacion de la recogida esporadica sin planillar
        /// </summary>
        /// <param name="idZona"></param>
        /// <param name="idTipoMensajero"></param>
        /// <param name="idCol"></param>
        /// <returns></returns>
        public List<OURecogidasDC> ObtenerProgramacionRecogidasSinPlanillar(string idZona, short idTipoMensajero, long idCol, DateTime fechaRecogidas)
        {
            return OURecogidas.Instancia.ObtenerProgramacionRecogidasSinPlanillar(idZona, idTipoMensajero, idCol, fechaRecogidas);
        }

        /// <summary>
        /// Guarda la planilla de recogidas con las recogidas seleccionadas
        /// </summary>
        /// <param name="programacion"></param>
        public void GuardaPlanillaRecogidas(OUProgramacionSolicitudRecogidaDC programacion)
        {
            OURecogidas.Instancia.GuardaPlanillaRecogidas(programacion);
        }

        /// <summary>
        /// Obtiene la planilla de programacion de recogidas
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <param name="tipoRecogida"></param>
        /// <returns></returns>
        public OUProgramacionSolicitudRecogidaDC ObtenerPlanillaRecogida(long idPlanilla)
        {
            return OURecogidas.Instancia.ObtenerPlanillaRecogida(idPlanilla);
        }

        #endregion planillas de Recogidas

        #endregion Programacion de recogidas

        #region Descargue Recogidas

        /// <summary>
        /// Retorna los motivos de descargue de recogidas
        /// </summary>
        /// <returns></returns>
        public List<OUMotivoDescargueRecogidasDC> ObtenerMotivosDescargueRecogidas()
        {
            return OURecogidas.Instancia.ObtenerMotivosDescargueRecogidas();
        }

        /// <summary>
        /// Obtiene las recogidas de la planilla
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <param name="tipoRecogida"></param>
        /// <returns></returns>
        public List<OURecogidasDC> ObtenerRecogidasPlanilla(long idPlanilla)
        {
            return OURecogidas.Instancia.ObtenerRecogidasPlanilla(idPlanilla);
        }

        #region Insert

        /// <summary>
        /// Guarda el descargue de una recogida
        /// </summary>
        /// <param name="AdminPlanilla"></param>
        public void GuardarDescargueRecogida(OUProgramacionSolicitudRecogidaDC AdminPlanilla)
        {
            OURecogidas.Instancia.GuardarDescargueRecogida(AdminPlanilla);
        }


        /// <summary>
        /// Guarda el descargue de la recogida peaton
        /// </summary>
        /// <param name="recogida"></param>
        public void GuardarDescargueRecogidaPeaton(OUDescargueRecogidaMensajeroDC descargue)
        {
            OURecogidas.Instancia.GuardarDescargueRecogidaPeaton(descargue);
        }

        /// <summary>
        /// cancela una recogida de peaton
        /// </summary>
        /// <param name="descargue"></param>
        public void CancelarRecogidaPeaton(OUDescargueRecogidaMensajeroDC descargue)
        {
            OURecogidas.Instancia.CancelarRecogidaPeaton(descargue);
        }


        /// <summary>
        /// Actualiza el estado de la solicitud de uan recogida e inserta el estado traza
        /// </summary>
        public void ActualizarEstadoSolicitudRecogida(long idRecogida, OUEnumEstadoSolicitudRecogidas estado)
        {
            OURecogidas.Instancia.ActualizarEstadoSolicitudRecogida(idRecogida, estado);
        }

        #endregion Insert

        #region Apertura Recogidas

        /// <summary>
        /// Obtiene los motivos de apertura de una recogida
        /// </summary>
        /// <returns></returns>
        public List<OUMotivoAperturaDC> ObtenerMotivosAperturaRecogida()
        {
            return OURecogidas.Instancia.ObtenerMotivosAperturaRecogida();
        }

        /// <summary>
        /// Abre una recogida
        /// </summary>
        /// <param name="recogida"></param>
        public void AbrirRecogida(OUAperturaRecogidaDC recogida)
        {
            OURecogidas.Instancia.AbrirRecogida(recogida);
        }

        #endregion Apertura Recogidas

        #endregion Descargue Recogidas

        #region Generales

        /// <summary>
        /// Retorna el último mensajero que tuvo asignada una guía dada
        /// </summary>
        /// <param name="idGuia"></param>
        /// <returns></returns>
        public OUNombresMensajeroDC ConsultarUltimoMensajeroGuia(long idGuia)
        {
            return OUMensajero.Instancia.ConsultarUltimoMensajeroGuia(idGuia);
        }

        #endregion Generales

        #region Parametro

        /// <summary>
        /// retorna el valor del parametro
        /// </summary>
        /// <param name="idParametro"></param>
        /// <returns></returns>
        public string ObtenerValorParametro(string idParametro)
        {
            return OUManejadorIngreso.Instancia.ObtenerValorParametro(idParametro);
        }

        /// <summary>
        /// Retorna la lista de parametros
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <returns></returns>
        public List<OUParametrosDC> ObtenerParametrosOperacionUrbana(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return OURepositorio.Instancia.ObtenerParametrosOperacionUrbana(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Edita el parametro de operacion urbana
        /// </summary>
        /// <param name="parametro"></param>
        public void EditarParametroOperacionUrbana(OUParametrosDC parametro)
        {
            OURepositorio.Instancia.EditarParametroOperacionUrbana(parametro);
        }

        #endregion Parametro

        #region Asignacion de tulas y precintos

        /// <summary>
        /// Método para obtener los tipos de asignación posibles
        /// </summary>
        /// <returns></returns>
        public IEnumerable<OUTipoAsignacionDC> ObtenerTiposAsignacion()
        {
            return OUAsignacionTulasPrecintos.Instancia.ObtenerTiposAsignacion();
        }

        /// <summary>
        /// Método para obtener las tulas y precintos sin utilizar generadas desde una racol
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <returns></returns>
        public IEnumerable<OUAsignacionDC> ObtenerAsignacionCentroServicio(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return OUAsignacionTulasPrecintos.Instancia.ObtenerAsignacionCentroServicio(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Método para asignar una tula y un precinto a un centro de servicio
        /// </summary>
        /// <param name="asignacionTula"></param>
        /// <returns></returns>
        public OUAsignacionDC AdicionarAsignacionCentroServicio(OUAsignacionDC asignacion)
        {
            return OUAsignacionTulasPrecintos.Instancia.AdicionarAsignacionCentroServicio(asignacion);
        }


        /// <summary>
        /// Método para eliminar una asignacion de tulas o contenedores
        /// </summary>
        /// <param name="asignacion"></param>
        public void EliminarAsignacionTulaContenedor(OUAsignacionDC asignacion)
        {
            OUAsignacionTulasPrecintos.Instancia.EliminarAsignacionTulaContenedor(asignacion);
        }


        #endregion Asignacion de tulas y precintos

        #region Novedades de ingreso

        /// <summary>
        /// Método para obtener las novedades de ingreso
        /// </summary>
        /// <returns></returns>
        public List<OUNovedadIngresoDC> ObtenerNovedadesIngreso()
        {
            return OUIngresoCentroAcopio.Instancia.ObtenerNovedadesIngreso();
        }

        #endregion


        #region Ingreso a centro de acopio

        /// <summary>
        /// Método para obtener las asignaciones
        /// </summary>
        /// <param name="controlTrans"></param>
        /// <param name="noPrecinto"></param>
        /// <param name="noConsolidado"></param>
        /// <returns></returns>
        public List<OUAsignacionDC> ObtenerAsignaciones(long controlTrans, long noPrecinto, string noConsolidado, OUMensajeroDC mensajero, long idCentroServicio)
        {
            return OUIngresoCentroAcopio.Instancia.ObtenerAsignaciones(controlTrans, noPrecinto, noConsolidado, mensajero, idCentroServicio);
        }


        /// <summary>
        /// Método para ingresar una guía suelta a centro de acopio 
        /// </summary>
        /// <param name="guia"></param>
        /// <returns></returns>
        public OUPlanillaVentaGuiasDC IngresarGuiaSuelta(OUPlanillaVentaGuiasDC guia, List<OUNovedadIngresoDC> listaNovedades)
        {
            return OUIngresoCentroAcopio.Instancia.IngresarGuiaSuelta(guia, listaNovedades);
        }

        #endregion

        /// <summary>
        /// Aprovisiona guía catalogada como "fantasma", es decir una numeración que debería ser automática
        /// </summary>
        /// <param name="numGUia"></param>
        /// <param name="idCs"></param>
        public bool AprovisionGuiaFantasma(long numGUia, long idCs)
        {
            return OURecogidas.Instancia.AprovisionGuiaFantasma(numGUia, idCs);
        }

        /// <summary>
        /// Retorna el número de auditoria
        /// </summary>
        /// <param name="idCs"></param>
        /// <returns></returns>
        public long CrearAuditoriaAsignacionMensajero(long idCs, long idMensajero)
        {
            return OURecogidas.Instancia.CrearAuditoriaAsignacionMensajero(idCs, idMensajero);
        }

        /// <summary>
        /// Crear auditoria asignacion mensajero guia
        /// </summary>
        /// <param name="idAuditoria"></param>
        /// <param name="esSobrante"></param>
        /// <param name="idMensajero"></param>
        /// <param name="fecha"></param>
        public void CrearAuditoriaAsignacionMensajeroGuia(long idAuditoria, int esSobrante, long idMensajero, DateTime fecha)
        {
            OURepositorio.Instancia.CrearAuditoriaAsignacionMensajeroGuia(idAuditoria, esSobrante, idMensajero, fecha);
        }

        /// <summary>
        /// Actualizar Auditorias Mensajero Guia
        /// </summary>
        /// <param name="idAuditoria"></param>
        /// <param name="esSobrante"></param>
        /// <param name="numeroGuia"></param>
        public void ActualizarAuditoriaAsignacionMensajeroGuia(long idAuditoria, int esSobrante, long numeroGuia)
        {
            if (esSobrante == 1)
            {
                if (OURepositorio.Instancia.ConsultarNumeroGuiaSobrante(numeroGuia, idAuditoria) == null)
                {
                    OURepositorio.Instancia.InsertarGuiaSobrante(idAuditoria, esSobrante, numeroGuia);
                }
                else
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, OUEnumTipoErrorOU.EX_SOBRANTE_YA_AUDITADO.ToString(), OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_SOBRANTE_YA_AUDITADO)));
                }

            }
            else
                OURepositorio.Instancia.ActualizarAuditoriaAsignacionMensajeroGuia(idAuditoria, esSobrante, numeroGuia);



        }

        /// <summary>
        /// Obtiene las auditorias realizadas a determinado mensajero ennun rango de fecha
        /// </summary>
        /// <param name="IdAuditoria"></param>
        /// <param name="fechaIni"></param>
        /// <param name="fechaFin"></param>
        /// <returns></returns>
        public List<OUGuiaIngresadaDC> ObtenerAuditoriasPorMensajero(long IdAuditoria, DateTime fechaIni, DateTime fechaFin)
        {
            return OURecogidas.Instancia.ObtenerAuditoriasPorMensajero(IdAuditoria, fechaIni, fechaFin);
        }

        /// <summary>
        /// obtiene las guias de determinada auditoria
        /// </summary>
        /// <param name="idAuditoria"></param>
        /// <returns></returns>
        public List<OUGuiaIngresadaDC> ObtenerGuiasPorAuditoria(long idAuditoria)
        {
            return OURecogidas.Instancia.ObtenerGuiasPorAuditoria(idAuditoria);
        }

        /// <summary>
        /// obtiene 
        /// </summary>
        /// <param name="idAuditoria"></param>
        /// <returns></returns>
        public List<OUGuiaIngresadaDC> ObtenerDetalleGuiasAuditadas(long idAuditoria)
        {
            return OURecogidas.Instancia.ObtenerDetalleGuiasAuditadas(idAuditoria);
        }

        /// <summary>
        /// obtiene 
        /// </summary>
        /// <param name="idAuditoria"></param>
        /// <returns></returns>
        //public List<OUGuiaIngresadaDC> IngresoBodegaPam(long idAuditoria)
        //{ 
        //    return OURepositorio.Instancia.ObtenerDetalleGuiasAuditadas(idAuditoria);
        //}


        #region Obtener mensajeros pam por centro de servicio
        public List<OUMensajeroPamDC> ObtenerMensajerosPamPorCentroServicio(long IdCentroServicio)
        {
            return OURepositorio.Instancia.ObtenerMensajerosPamPorCentroServicio(IdCentroServicio);
        }
        #endregion

        #region Obtener lista guias mensajeros pam por centro de servicio
        public List<LIReclameEnOficinaDC> ObtenerGuiasDelPamPorCentroServicio(long IdCentroServicio, string usuario)
        {
            return OURepositorio.Instancia.ObtenerGuiasDelPamPorCentroServicio(IdCentroServicio, usuario);
        }
        #endregion

        #region ingresa guia pam

        public LIReclameEnOficinaDC IngresoGuiaPam(LIReclameEnOficinaDC GuiasPam)
        {
            //todo  luis poner transaccion
            IADFachadaAdmisionesMensajeria fachadaMensajeria = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>();
            IPUFachadaCentroServicios fachadaCentroServicios = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();

            try
            {
                ADTrazaGuia estadoGuia;
                PUMovimientoInventario MovimientoActual = LIRepositorioPruebasEntrega.Instancia.ConsultaUltimoMovimientoBodegaGuia(GuiasPam.MovimientoInventario.NumeroGuia);

                if (MovimientoActual.TipoMovimiento == PUEnumTipoMovimientoInventario.Asignacion)
                {
                    ADGuia Guia = fachadaMensajeria.ObtenerGuiaXNumeroGuia(GuiasPam.MovimientoInventario.NumeroGuia);

                    //ingresar a bodega        
                    GuiasPam.MovimientoInventario.NumeroGuia = Guia.NumeroGuia;
                    GuiasPam.MovimientoInventario.FechaGrabacion = DateTime.Now;
                    GuiasPam.MovimientoInventario.FechaEstimadaIngreso = DateTime.Now;
                    GuiasPam.MovimientoInventario.CreadoPor = ControllerContext.Current.Usuario;
                    GuiasPam.IdReclameEnOficina = fachadaCentroServicios.AdicionarMovimientoInventario(GuiasPam.MovimientoInventario);


                    //primera transicion de estado pasa a ingreso a bodega
                    estadoGuia = new ADTrazaGuia
                    {
                        Ciudad = GuiasPam.MovimientoInventario.Bodega.CiudadUbicacion.Nombre,
                        IdCiudad = GuiasPam.MovimientoInventario.Bodega.CiudadUbicacion.IdLocalidad,
                        IdAdmision = Guia.IdAdmision,
                        IdEstadoGuia = (short)(EstadosGuia.ObtenerUltimoEstado(Guia.IdAdmision)),
                        IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.IngresoABodega,
                        Modulo = COConstantesModulos.PRUEBAS_DE_ENTREGA,
                        NumeroGuia = GuiasPam.MovimientoInventario.NumeroGuia,
                        Observaciones = string.Empty,
                        FechaGrabacion = DateTime.Now
                    };

                    estadoGuia.IdTrazaGuia = EstadosGuia.ValidarInsertarEstadoGuia(estadoGuia);

                    if (estadoGuia.IdTrazaGuia == 0)
                    {
                        GuiasPam.Respuesta = OUEnumValidacionDescargue.ErrorEstado;
                    }
                    else
                    {
                        //estadoGuia.IdTrazaGuia = EstadosGuia.InsertaEstadoGuia(estadoGuia);
                        GuiasPam.Respuesta = OUEnumValidacionDescargue.Exitosa;
                        GuiasPam.Mensaje = "Se ingreso la guía satisfactoriamente";
                    }
                }

                else
                {
                    GuiasPam.Respuesta = OUEnumValidacionDescargue.Notificacion;
                    GuiasPam.Mensaje = "La guía no se encuentra asignada para ingreso a reclame en oficina";
                }


            }
            catch (Exception ex)
            {
                GuiasPam.Respuesta = OUEnumValidacionDescargue.Error;
                GuiasPam.Mensaje = ex.Message;
            }
            return GuiasPam;

        }

        #endregion


        /// <summary>
        /// Obtiene todas las recogidas asignadas a un mensajero en un dia
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public List<OURecogidasDC> ObtenerRecogidasMensajerosDia(long idMensajero)
        {
            return OURecogidas.Instancia.ObtenerRecogidasMensajerosDia(idMensajero);
        }

        /// <summary>
        /// Selecciona todas las recogidas vencidas que fueron asignadas a los usuarioMensajero (usuarios PAM ) en un dia
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public List<OURecogidasDC> ObtenerRecogidasVencidasMensajerosPAMDia()
        {
            return OURecogidas.Instancia.ObtenerRecogidasVencidasMensajerosPAMDia();
        }



        /// <summary>
        /// Actualiza la georeferenciacion de una recogida
        /// </summary>
        /// <param name="longitud"></param>
        /// <param name="latitud"></param>
        /// <param name="idRecogida"></param>
        public void ActualizarGeoreferenciacionRecogida(string longitud, string latitud, long idRecogida)
        {
            OURecogidas.Instancia.ActualizarGeoreferenciacionRecogida(longitud, latitud, idRecogida);
        }



        /// <summary>
        /// Retorna los motivos de descargue de recogidas firltrado por idmotivo
        /// </summary>
        /// <returns></returns>
        public OUMotivoDescargueRecogidasDC ObtenerMotivosDescargueRecogidasIdMotivo(int idMotivo)
        {
            return OURepositorio.Instancia.ObtenerMotivosDescargueRecogidasIdMotivo(idMotivo);
        }

        /// <summary>
        /// Obtiene todas las recogidas creadas por un cliente movil en un dia
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public List<OURecogidasDC> ObtenerRecogidasClienteMovilDia(string tokenDispositivo)
        {
            return OURecogidas.Instancia.ObtenerRecogidasClienteMovilDia(tokenDispositivo);
        }

        /// <summary>
        /// Obtiene las recogidas realizadas segun el token del dispositivo del cliente peaton
        /// </summary>
        /// <param name="tokenDispositivo"></param>
        /// <returns></returns>
        public List<OURecogidasDC> ObtenerMisRecogidasClientePeaton(string tokenDispositivo)
        {
            return OURecogidas.Instancia.ObtenerMisRecogidasClientePeaton(tokenDispositivo);
        }

        /// <summary>
        /// Obtiene las imagenes capturadas de la solicitud recogida.
        /// </summary>
        /// <param name="idSolicitudRecogida"></param>
        /// <returns></returns>
        public List<string> ObtenerImagenesSolicitudRecogida(long idSolicitudRecogida)
        {
            return OURecogidas.Instancia.ObtenerImagenesSolicitudRecogida(idSolicitudRecogida);
        }

        /// <summary>
        /// Metodo para calificar la solicitud de recogida.
        /// </summary>
        /// <param name="idSolicitudRecogida"></param>
        /// <param name="calificacion"></param>
        /// <param name="observaciones"></param>
        public void CalificarSolicitudRecogida(long idSolicitudRecogida, int calificacion, string observaciones)
        {
            OURecogidas.Instancia.CalificarSolicitudRecogida(idSolicitudRecogida, calificacion, observaciones);
        }
        /// <summary>
        /// metodo para obtener motivo y fecha intento de entrega por numero de guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="idMotivoGuia"></param>
        /// <returns></returns>
        public LIMotivoEvidenciaGuiaDC ObtenerFechaIntentoYMotivoGuia(long numeroGuia, long idPlanilla)
        {
            return OURecogidas.Instancia.ObtenerFechaIntentoYMotivoGuia(numeroGuia, idPlanilla);
        }
        /// <summary>
        /// Metodo para obtener las guias planilladas para auditor
        /// </summary>
        /// <param name="idAuditor"></param>
        /// <returns></returns>
        public IList<OUGuiaIngresadaDC> ObtenerGuiasAuditor(long idAuditor)
        {
            return OURecogidas.Instancia.ObtenerGuiasAuditor(idAuditor);
        }

        /// <summary>
        /// Metodo para obtener las guias planilladas para mensajero
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public IList<OUGuiaIngresadaDC> ObtenerGuiasMensajero(long idMensajero)
        {
            return OURecogidas.Instancia.ObtenerGuiasMensajero(idMensajero);
        }

        /// <summary>
        /// Metodo para obtener informacion del mensajero /auditor segun numero de identificacion
        /// </summary>
        /// <param name="numIdentificacion"></param>
        /// <returns></returns>
        public OUMensajeroDC ObtenerInformacionUsuarioControllerApp(string numIdentificacion)
        {
            return OURepositorio.Instancia.ObtenerInformacionUsuarioControllerApp(numIdentificacion);
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

        public OUMensajeroDC ObtenerAsignacionMensajeroPorNumeroGuia(long numeroGuia)
        {
            return OURepositorio.Instancia.ObtenerAsignacionMensajeroPorNumeroGuia(numeroGuia);
        }
    }
}