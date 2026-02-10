using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading;
using CO.Servidor.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.Contratos;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using CO.Servidor.Servicios.ContratoDatos.Comun;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.CentroServicios;
using CO.Servidor.Dominio.Comun.AdmEstadosGuia;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.OperacionUrbana.Comun;

namespace CO.Servidor.Servicios.Implementacion.OperacionUrbana
{
    /// <summary>
    /// Clase para los servicios de administración de Tarifas
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class OUOperacionUrbanaSvc : IOUOperacionUrbanaSvc
    {
        #region Constructor

        public OUOperacionUrbanaSvc()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(PAAdministrador.Instancia.ConsultarParametrosFramework("Cultura"));
        }

        #endregion Constructor

        #region Mensajero

        //public GenericoConsultasFramework<OUMensajeroDC> ObtenerMensajero(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente)
        //{
        //  int totalRegistros = 0;
        //  return new GenericoConsultasFramework<OUMensajeroDC>()
        //  {
        //    Lista = OUAdministradorOperacionUrbana.Instancia.ObtenerMensajero(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros),
        //    TotalRegistros = totalRegistros
        //  };
        //}

        public void ActualizarMensajero(OUMensajeroDC mensajero)
        {
            OUAdministradorOperacionUrbana.Instancia.ActualizarMensajero(mensajero);
        }

        /// <summary>
        /// Obtiene la lista de valor adicional de la DB
        /// </summary>
        /// <returns></returns>
        public IEnumerable<OUTipoMensajeroDC> ObtenerTiposMensajero()
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerTiposMensajero();
        }

        /// <summary>
        /// obtiene la lista con los estados de los mensajeros
        /// </summary>
        /// <returns></returns>
        public IList<OUEstadosMensajeroDC> ObtenerEstadosMensajero()
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerEstadosMensajero();
        }

        public OUMensajeroDC ConsultaExisteMensajero(string identificacion, bool contratista)
        {
            return OUAdministradorOperacionUrbana.Instancia.ConsultaExisteMensajero(identificacion, contratista);
        }


        /// <summary>
        /// Método para obtener los mensajeros por localidad
        /// </summary>
        /// <param name="Localidad"></param>
        /// <returns></returns>
        public IEnumerable<POMensajero> ObtenerMensajerosLocalidad(string Localidad)
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerMensajerosLocalidad(Localidad);
        }


        #endregion Mensajero

        #region Centro de Acopio

        /// <summary>
        /// Obtiene el total de los envios pendientes asignados por planilla de venta al mensajero
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public int ObtenerTotalEnviosPendientes(long idMensajero)
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerTotalEnviosPendientes(idMensajero);
        }

        /// <summary>
        /// Obtiene los nombres de los mensajeros del centro logistico
        /// </summary>
        /// <returns></returns>
        public OUGuiaIngresadaDC ObtenerInfoMensajero(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, long centroLogistico)
        {
            int totalRegistros = 1;
            return OUAdministradorOperacionUrbana.Instancia.ObtenerInfoMensajero(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, centroLogistico);
        }

        /// <summary>
        /// Obtiene los estados de los empaques para mensajeria y carga
        /// </summary>
        /// <returns></returns>
        public List<OUEstadosEmpaqueDC> ObtenerEstadosEmpaque()
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerEstadosEmpaque();
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
        public GenericoConsultasFramework<OUMensajeroDC> ObtenerMensajeroCentroLogistico(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, long centroLogistico)
        {
            int totalRegistros = 0;
            return new GenericoConsultasFramework<OUMensajeroDC>()
            {
                Lista = OUAdministradorOperacionUrbana.Instancia.ObtenerMensajeroCentroLogistico(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, centroLogistico),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Obtiene todos los mensajeros de un col
        /// </summary>
        /// <param name="idCol"></param>
        /// <returns></returns>
        public List<OUMensajeroDC> ObtenerMensajerosCol(long idCol)
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerMensajerosCol(idCol);
        }

        /// <summary>
        /// Guarda y valida la guia ingresada
        /// </summary>
        /// <param name="guiaIngresada"></param>
        public OUGuiaIngresadaDC GuardarIngreso(OUGuiaIngresadaDC guiaIngresada)
        {
            return OUAdministradorOperacionUrbana.Instancia.GuardarIngreso(guiaIngresada);
        }

        /// <summary>
        /// retorna el total de los envios planillados por mensajero
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public int TotalEnviosPlanillados(long idMensajero)
        {
            return OUAdministradorOperacionUrbana.Instancia.TotalEnviosPlanillados(idMensajero);
        }

        /// <summary>
        /// Retorna la guia consultada a partir del número de guía ingresado
        /// </summary>
        /// <param name="guiaIngresada">Guía Ingresada</param>
        /// <returns>Guía Consultada</returns>
        public OUGuiaIngresadaDC ConsultaGuia(OUGuiaIngresadaDC guiaIngresada)
        {
            return OUAdministradorOperacionUrbana.Instancia.ConsultaGuia(guiaIngresada);
        }

        /// <summary>
        /// Obtener los envios pendientes por descargar
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        GenericoConsultasFramework<OUGuiasPendientesDC> IOUOperacionUrbanaSvc.ObtenerEnviosPendientes(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, long idMensajero)
        {
            int totalRegistros = 0;
            return new GenericoConsultasFramework<OUGuiasPendientesDC>()
            {
                Lista = OUAdministradorOperacionUrbana.Instancia.ObtenerEnviosPendientes(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idMensajero),
                TotalRegistros = totalRegistros
            };
        }



        /// <summary>
        /// Obtener la informacion de un mensajero por medio de su identificador
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns>Datos mensajero</returns>
        public OUDatosMensajeroDC ObtenerDatosMensajero(long idMensajero)
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerDatosMensajero(idMensajero);
        }

        #endregion Centro de Acopio

        #region Planilla ventas

        /// <summary>
        /// Guarda la planilla venta y planilla venta guias
        /// </summary>
        /// <param name="planilla"></param>
        /// <param name="guiasPlanilla"></param>
        public long GuardarPlanillaVentas(OUPlanillaVentaDC planilla, List<OUPlanillaVentaGuiasDC> guiasPlanilla)
        {
            return OUAdministradorOperacionUrbana.Instancia.GuardarPlanillaVentas(planilla, guiasPlanilla);
        }

        /// <summary>
        /// Obtiene las guias asignadas a una planilla y a una asignacion tula
        /// </summary>
        /// <param name="idPlanillaVentas"> id de la planilla de ventas</param>
        /// <param name="idAsignacionTula">Id de la asignacion tula</param>
        /// <returns>Lista con las guias asignadas</returns>
        public List<OUGuiaIngresadaDC> ObtenerGuiasPorPlanillaAsignacionTula(long idPlanillaVentas, long idAsignacionTula)
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerGuiasPorPlanillaAsignacionTula(idPlanillaVentas, idAsignacionTula);

        }

        /// <summary>
        /// Obtiene todas las guias sueltas planilladas
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <returns></returns>
        public List<OUGuiaIngresadaDC> ObtenerGuiasSueltasPlanilladas(long idPlanilla)
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerGuiasSueltasPlanilladas(idPlanilla);
        }

        /// <summary>
        /// Despacha la falla para las guias sin planillar
        /// </summary>
        /// <param name="idCentroServicios"></param>
        public void EnviaFallaConGuiasNoPlanilladas(long idCentroServicios, string nombreCentroServicios, string direccionCentroServicios)
        {
            OUAdministradorOperacionUrbana.Instancia.EnviaFallaConGuiasNoPlanilladas(idCentroServicios, nombreCentroServicios, direccionCentroServicios);
        }

        /// <summary>
        /// Obtiene las planilla por centro de servicios
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="idCentroServicios"></param>
        /// <returns></returns>
        public GenericoConsultasFramework<OUPlanillaVentaDC> ObtenerPlanillasPorCentroServicios(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, long idCentroServicios)
        {
            int totalRegistros = 0;
            return new GenericoConsultasFramework<OUPlanillaVentaDC>()
            {
                Lista = OUAdministradorOperacionUrbana.Instancia.ObtenerPlanillasPorCentroServicios(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idCentroServicios),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Consulta si un punto de servicio tiene almenos una planilla de recoleccion en punto (planilla ventas)  abierta
        /// </summary>
        /// <returns></returns>
        public bool ValidarPlanillasAbiertasPorPuntoVenta(long idPuntoServicio)
        {
            return OUAdministradorOperacionUrbana.Instancia.ValidarPlanillasAbiertasPorPuntoVenta(idPuntoServicio);
        }



        /// <summary>
        /// Adiciona una guia a una planilla, si la planilla no existe crea una nueva y retorna el numero de la planilla
        /// </summary>
        /// <param name="planilla"></param>
        /// <param name="guiaPlanilla"></param>
        /// <returns></returns>
        public long AdicionarGuiaPlanilla(OUPlanillaVentaDC planilla, OUPlanillaVentaGuiasDC guiaPlanilla)
        {
            return OUAdministradorOperacionUrbana.Instancia.AdicionarGuiaPlanilla(planilla, guiaPlanilla);
        }



        public long CrearPlanillaVentas(OUPlanillaVentaDC planilla, List<OUPlanillaVentaGuiasDC> LstGuias)
        {
            return OUAdministradorOperacionUrbana.Instancia.CrearPlanillaVentas(planilla, LstGuias);
        }



        /// <summary>
        /// Adiciona una guia suelta a una planilla, si la planilla no existe crea una nueva y retorna el numero de la planilla
        /// </summary>
        /// <param name="planilla"></param>
        /// <param name="guiaPlanilla"></param>
        /// <returns></returns>
        public long AdicionarGuiaSueltaPlanilla(OUPlanillaVentaDC planilla, OUPlanillaVentaGuiasDC guiaPlanilla)
        {

            return OUAdministradorOperacionUrbana.Instancia.AdicionarGuiaSueltaPlanilla(planilla, guiaPlanilla);
        }

        /// <summary>
        /// Elimina una guia de un consolidado en la planilla de ventas
        /// </summary>
        /// <param name="idAdmisionMensajeria"></param>
        /// <param name="idPlanilla"></param>
        public OUPlanillaVentaGuiasDC EliminarGuiaConsolidadoPlanillaVentas(OUPlanillaVentaGuiasDC guiaPlanilla)
        {
            return OUAdministradorOperacionUrbana.Instancia.EliminarGuiaConsolidadoPlanillaVentas(guiaPlanilla);
        }
        /// <summary>
        /// Elimina una guia o un rotulo de una planilla de ventas
        /// </summary>
        /// <param name="idAdmisionMensajeria"></param>
        /// <param name="idPlanilla"></param>
        public OUPlanillaVentaGuiasDC EliminarGuiaPlanillaVentas(OUPlanillaVentaGuiasDC guiaPlanilla)
        {
            return OUAdministradorOperacionUrbana.Instancia.EliminarGuiaPlanillaVentas(guiaPlanilla);
        }




        /// <summary>
        /// Obtiene las asignaciones para imprimir y cierra la planilla
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <param name="idCentroServicios"></param>
        /// <returns></returns>
        public List<OUImpresionPlanillaVentasDC> ObtenerCerrarImpresionPlanillaVentasTotal(OUPlanillaVentaDC planilla, long idCentroServicios)
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerCerrarImpresionPlanillaVentasTotal(planilla, idCentroServicios);
        }
        /// <summary>
        /// Obtiene las asignaciones para imprimir y cierra la planilla parcial
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <param name="idCentroServicios"></param>
        /// <returns></returns>
        public List<OUImpresionPlanillaVentasDC> ObtenerCerrarImpresionPlanillaVentasParcial(long idPlanilla, long idCentroServicios)
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerCerrarImpresionPlanillaVentasParcial(idPlanilla, idCentroServicios);
        }

        /// <summary>
        /// Obtiene las asignaciones para imprimir la planilla
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <param name="idCentroServicios"></param>
        /// <returns></returns>
        public OUImpresionPlanillaVentasDC ObtenerImpresionManifiestoPlanillaVentas(long idPlanilla, long idCentroServicios)
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerImpresionManifiestoPlanillaVentas(idPlanilla, idCentroServicios);
        }


        /// <summary>
        /// Obtiene las asignaciones para imprimir la planilla
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <param name="idCentroServicios"></param>
        /// <returns></returns>
        public List<OUImpresionPlanillaVentasDC> ObtenerImpresionPlanillaVentasSinCerrar(long idPlanilla, long idCentroServicios)
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerImpresionPlanillaVentasSinCerrar(idPlanilla, idCentroServicios);
        }

        /// <summary>
        /// Obtiene una lista de las asignaciones de tulas y precintos por punto de servicio,  y por estado
        /// </summary>
        /// <param name="idPuntoServicio"></param>
        /// <returns></returns>
        public List<OUAsignacionDC> ObtenerAsignacionTulaPrecintoPuntoServicio(long idPuntoServicio, string estadoAsignacion)
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerAsignacionTulaPrecintoPuntoServicio(idPuntoServicio, estadoAsignacion);
        }

        /// <summary>
        /// Obtiene los mensajeros del racol
        /// </summary>
        /// <param name="centroLogistico"></param>
        /// <returns></returns>
        public GenericoConsultasFramework<OUMensajeroDC> ObtenerMensajeroPorRegional(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, long idRacol)
        {
            int totalRegistros = 0;
            return new GenericoConsultasFramework<OUMensajeroDC>()
            {
                Lista = OUAdministradorOperacionUrbana.Instancia.ObtenerMensajeroPorRegional(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idRacol),
                TotalRegistros = totalRegistros
            };
        }

        public List<OUPlanillaVentaGuiasDC> ObtenerGuiasPorPuntoDeServicios(long idCentroServicios)
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerGuiasPorPuntoDeServicios(idCentroServicios);
        }

        #endregion Planilla ventas

        #region Mensajeros por agencia

        /// <summary>
        /// Obtiene los mensajeros de la agencia
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="puntoServicio"></param>
        /// <returns></returns>
        public GenericoConsultasFramework<OUMensajeroDC> ObtenerMensajeroPorAgencia(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, long puntoServicio)
        {
            int totalRegistros = 0;
            return new GenericoConsultasFramework<OUMensajeroDC>()
            {
                Lista = OUAdministradorOperacionUrbana.Instancia.ObtenerMensajeroPorAgencia(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, puntoServicio),
                TotalRegistros = totalRegistros
            };
        }

        #endregion Mensajeros por agencia

        #region Planilla Asignacion

        /// <summary>
        /// retorna las planillas de asignación de envios del centro logistico
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="idCentroServicios"></param>
        /// <returns></returns>
        public List<OUPlanillaAsignacionDC> ObtenerPlanillasAsignacionCentroLogistico(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, long idCentroServicios, bool incluyeFecha)
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerPlanillasAsignacionCentroLogistico(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, idCentroServicios, incluyeFecha);
        }

        /// <summary>
        /// Retorna los mensajeros asociados al Col, filtrados por tipo de mensajero
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <param name="idCentroServicios"></param>
        /// <param name="idTipoMensajero"></param>
        /// <returns></returns>
        public GenericoConsultasFramework<OUMensajeroDC> ObtenerMensejorPorColYTipoMensajero(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, long idCentroServicios, int idTipoMensajero)
        {
            int totalRegistros = 0;
            return new GenericoConsultasFramework<OUMensajeroDC>()
            {
                Lista = OUAdministradorOperacionUrbana.Instancia.ObtenerMensejorPorColYTipoMensajero(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idCentroServicios, idTipoMensajero),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// REtorna los envios de la planilla de asignación
        /// </summary>
        /// <param name="idPlanilla">Id de la planilla de asignación</param>
        /// <returns>Lista de las guias planilladas</returns>
        public GenericoConsultasFramework<OUGuiaIngresadaDC> ObtenerEnviosPlanillaAsignacion(long idPlanilla)
        {
            return new GenericoConsultasFramework<OUGuiaIngresadaDC>()
            {
                Lista = OUAdministradorOperacionUrbana.Instancia.ObtenerEnviosPlanillaAsignacion(idPlanilla)
            };
        }

        /// <summary>
        /// Obtiene las guias de determinado mensajero en determinado dia
        /// </summary>
        /// <param name="idmensajero"></param>
        /// <param name="fechaPlanilla"></param>
        /// <returns>las guias de determinado mensajero con determinada fecha</returns>
        public GenericoConsultasFramework<OUGuiaIngresadaDC> ObtenerGuiasPlanilladasPorDiaYMensajero(long idMensajero, DateTime fechaPlanilla)
        {
            return new GenericoConsultasFramework<OUGuiaIngresadaDC>()
            {
                Lista = OUAdministradorOperacionUrbana.Instancia.ObtenerGuiasPlanilladasPorDiaYMensajero(idMensajero, fechaPlanilla)
            };
        }

        /// <summary>
        /// Guarda la planilla de asignacion y el envio de esta
        /// </summary>
        /// <param name="planilla"></param>
        public OUPlanillaAsignacionDC GuardarPlanillaAsignacionEnvio(OUPlanillaAsignacionDC planilla)
        {
            return OUAdministradorOperacionUrbana.Instancia.GuardarPlanillaAsignacionEnvio(planilla);
        }

        /// <summary>
        /// Actualiza el total de los envios planillados
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <param name="totalGuias"></param>
        public void ActualizaTotalEnviosPlanillados(long idPlanilla, int totalGuias)
        {
            OUAdministradorOperacionUrbana.Instancia.ActualizaTotalEnviosPlanillados(idPlanilla, totalGuias);
        }

        /// <summary>
        /// Realiza la verificacion del envio seleccionado
        /// </summary>
        /// <param name="planillaAsignacion"></param>
        /// <param name="guia"></param>
        public void ActualizaEnvioVerificadoPlanillaAsignacion(long planillaAsignacion, long numGuia)
        {
            OUAdministradorOperacionUrbana.Instancia.ActualizaEnvioVerificadoPlanillaAsignacion(planillaAsignacion, numGuia);
        }

        /// <summary>
        /// Elimina el envio seleccionado de la planilla de asignacion
        /// </summary>
        /// <param name="idAdmisionMensajeria"></param>
        public void EliminarEnvioPlanillaAsignacion(OUPlanillaAsignacionDC planilla, long idAdmisionMensajeria)
        {
            OUAdministradorOperacionUrbana.Instancia.EliminarEnvioPlanillaAsignacion(planilla, idAdmisionMensajeria);
        }



        /// <summary>
        /// Elimina un envio de la planilla de asignacion
        /// </summary>
        /// <param name="idAdmisionMensajeria"></param>
        public void EliminarEnvioPlanillaCentroAcopio(long numeroGuia)
        {
            OUPlanillaAsignacionEnvios.Instancia.EliminarEnvioPlanillaCentroAcopio(numeroGuia);
        }


        /// <summary>
        /// Valida si el envío está asignado a una planilla
        /// </summary>
        /// <param name="idGuia"></param>
        /// <returns></returns>
        public OUGuiaIngresadaDC ConsultaEnvioPlanillaAsignacionGuia(long idGuia)
        {
            return OUAdministradorOperacionUrbana.Instancia.ConsultaEnvioPlanillaAsignacionGuia(idGuia);
        }

        /// <summary>
        /// Reasigna el envio a otra planilla
        /// </summary>
        /// <param name="idplanilla">id de la planilla del envio</param>
        /// <param name="idPlanillaNueva">id de la planilla a la que se va a reasignar el envio</param>
        /// <param name="idAdmisionMensajeria">id del envio</param>
        public void ReasignarEnvioPlanilla(long idPlanilla, long idPlanillaNueva, long idAdmisionMensajeria, long idAgencia)
        {
            OUAdministradorOperacionUrbana.Instancia.ReasignarEnvioPlanilla(idPlanilla, idPlanillaNueva, idAdmisionMensajeria, idAgencia);
        }

        /// <summary>
        /// Cierra la planilla de asignacion
        /// </summary>
        /// <param name="idPlanilla"></param>
        public void CerrarPlanillaASignacion(long idPlanilla)
        {
            OUAdministradorOperacionUrbana.Instancia.CerrarPlanillaAsignacion(idPlanilla);
        }

        /// <summary>
        /// Abre la planilla de asignacion
        /// </summary>
        /// <param name="idPlanilla"></param>
        public void AbrirPlanillaAsignacion(long idPlanilla)
        {
            OUAdministradorOperacionUrbana.Instancia.AbrirPlanillaAsignacion(idPlanilla);
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
            return OUAdministradorOperacionUrbana.Instancia.VerificaMensajeroSoatTecnoMecanica(idMensajero);
        }

        /// <summary>
        /// Asigna mensajero a la planilla de asignacion
        /// </summary>
        /// <param name="planilla"></param>
        public void AsignaMensajeroPlanilla(OUPlanillaAsignacionDC planilla)
        {
            OUAdministradorOperacionUrbana.Instancia.AsignaMensajeroPlanilla(planilla);
        }

        /// <summary>
        /// Obtiene el total de los envios pendientes del mensajero
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public int ObtenerEnviosPendientesMensajero(long idMensajero)
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerEnviosPendientesMensajero(idMensajero);
        }

        /// <summary>
        /// Retorna los estados de la planilla
        /// </summary>
        /// <returns></returns>
        public IEnumerable<OUEstadosPlanillaAsignacionDC> ObtenerEstadosPlanillaAsignacion()
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerEstadosPlanillaAsignacion();
        }

        /// <summary>
        /// Retorna los mensajeros del centro logistico
        /// </summary>
        /// <param name="idCentroLogistico"></param>
        /// <returns></returns>
        public IEnumerable<OUNombresMensajeroDC> ObtenerNombreMensajerosCOL(long idCentroLogistico)
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerNombreMensajerosCOL(idCentroLogistico);

        }
        /// <summary>
        /// Obtene los datos de los mensajeros de una agencia a partir de un punto de servicio.
        /// </summary>
        /// <param name="idAgencia">Es el id agencia.</param>
        /// <returns>la lista de mensajeros de una agencia</returns>
        public IEnumerable<OUNombresMensajeroDC> ObtenerMensajerosAgenciaDesdePuntoServicio(long idPuntoServicio)
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerMensajerosAgenciaDesdePuntoServicio(idPuntoServicio);
        }

        /// <summary>
        /// Obtene los datos del mensajero de la agencia.
        /// </summary>
        /// <param name="idAgencia">Es el id agencia.</param>
        /// <returns>la lista de mensajeros de una agencia</returns>
        public IEnumerable<OUNombresMensajeroDC> ObtenerNombreMensajeroAgencia(long idAgencia)
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerNombreMensajeroAgencia(idAgencia);
        }

        /// <summary>
        /// Modifica una planilla de asignacion de envios
        /// </summary>
        /// <param name="planilla"></param>
        public void ModificarPlanillaAsignacionEnvio(OUPlanillaAsignacionDC planilla)
        {
            OUAdministradorOperacionUrbana.Instancia.ModificarPlanillaAsignacionEnvio(planilla);
        }


        #endregion Planilla Asignacion

        #region Solicitud Recogidas

        /// <summary>
        /// Obtiene los estados de la solicitud de recogida
        /// </summary>
        /// <returns></returns>
        public List<OUEstadosSolicitudRecogidaDC> ObtenerEstadosRecogida()
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerEstadosRecogida();
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
            return OUAdministradorOperacionUrbana.Instancia.ObtenerRecogidas(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, idAgencia, incluyeFechaAsignacion, incluyeFechaRecogida);
        }

        /// <summary>
        /// Guarda la solicitud de recogida del punto de servicios
        /// </summary>
        /// <param name="recogida"></param>
        public void GuardaSolicitudRecogidaPuntoSvc(OURecogidasDC recogida)
        {
            OUAdministradorOperacionUrbana.Instancia.GuardaSolicitudRecogidaPuntoSvc(recogida);
        }

        /// <summary>
        /// Guarda la solicitud de la recogida por cliente convenio
        /// </summary>
        /// <param name="recogida"></param>
        public void GuardarSolicitudClienteConvenio(OURecogidasDC recogida)
        {
            OUAdministradorOperacionUrbana.Instancia.GuardarSolicitudClienteConvenio(recogida);
        }

        /// <summary>
        /// Guarda la solicitud de recogida del cliente peaton
        /// </summary>
        /// <param name="recogida"></param>
        public long GuardaSolicitudClientePeaton(OURecogidasDC recogida)
        {
            return OUAdministradorOperacionUrbana.Instancia.GuardaSolicitudClientePeaton(recogida);
        }

        public void ActualizaSolicitudClientePeaton(OURecogidasDC recogida)
        {
            OUAdministradorOperacionUrbana.Instancia.ActualizaSolicitudClientePeaton(recogida);
        }

        /// <summary>
        /// consulta direcciones historico recogidas por documento
        /// </summary>
        /// <param name="SolicitudRecogidaPeaton"></param>
        public List<OURecogidasDC> ObtenerDireccionesPeaton(OURecogidaPeatonDC Peaton)
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerDireccionesPeaton(Peaton);
        }

        /// <summary>
        /// Metodo para obtener ultima informacion cliente externo
        /// </summary>
        /// <param name="identificacion"></param>
        /// <returns></returns>
        public OURecogidasDC ObtenerInformacionRecogidaUsuarioExterno(string nomUsuario)
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerInformacionRecogidaUsuarioExterno(nomUsuario);
        }

        /// <summary>
        /// Consulta la solicitud de recogida para un cliente peaton especifico 
        /// </summary>
        /// <returns></returns>
        public OURecogidasDC ObtenerInformacionClientePeaton(OURecogidasDC infoRecogida)
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerInformacionClientePeaton(infoRecogida);
        }


        /// <summary>
        /// Inserta la relacion entre un dispositivo movil y una recogida
        /// </summary>
        /// <param name="idRecogida"></param>
        /// <param name="idDispositivoMovil"></param>
        public void RegistrarSolicitudRecogidaMovil(long idRecogida, long idDispositivoMovil)
        {
            OUAdministradorOperacionUrbana.Instancia.RegistrarSolicitudRecogidaMovil(idRecogida, idDispositivoMovil);
        }
        /// <summary>
        /// retorna el token del dispositivo movil con el que se hizo una recogida
        /// </summary>
        /// <param name="idRecogida"></param>
        public PADispositivoMovil ObtenerdispositivoMovilClienteRecogida(long idRecogida)
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerdispositivoMovilClienteRecogida(idRecogida);
        }



        /// <summary>
        /// Obtiene todas las recogidas pendientes de peaton del dia actual, filtradas por un rango de tiempo para la hora de recogida
        /// </summary>
        /// <returns></returns>
        public List<OURecogidasDC> ObtenerRecogidasPeatonPendientesDia()
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerRecogidasPeatonPendientesDia();
        }


        /// <summary>
        /// Obtiene todas las recogidas pendientes de peaton del dia actual, filtradas por un rango de tiempo para la hora de recogida y localidad de recogida
        /// </summary>
        /// <returns></returns>
        public List<OURecogidasDC> ObtenerRecogidasPeatonPendientesPorProgramarDia(string idLocalidad)
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerRecogidasPeatonPendientesPorProgramarDia(idLocalidad);
        }

        /// <summary>
        /// Obtiene todas las solicitudes de recogida disponibles por localidad.
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <returns></returns>
        public List<OURecogidasDC> ObtenerRecogidasDisponiblesPeatonDia(string idLocalidad)
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerRecogidasDisponiblesPeatonDia(idLocalidad);
        }
        /// <summary>
        /// Obtiene todas las recogidas de peaton pendientes por programas
        /// </summary>
        /// <returns></returns>
        public List<OURecogidasDC> ObtenerTodasRecogidasPeatonPendientesPorProgramar()
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerTodasRecogidasPeatonPendientesPorProgramar();
        }

        /// <summary>
        /// Guarda las notificaciones enviadas por cada recogida
        /// </summary>
        /// <param name="idSolicitudRecogida"></param>
        public void GuardarNotificacionRecogida(long idSolicitudRecogida)
        {
            OUAdministradorOperacionUrbana.Instancia.GuardarNotificacionRecogida(idSolicitudRecogida);
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
            return OUAdministradorOperacionUrbana.Instancia.ObtenerInfoUsuarioRecogida(tipoid, identificacion);
        }

        #endregion Solicitud Recogidas

        #region Programacion de recogidas

        /// <summary>
        /// Retorna los motivos de la Reprogramacion
        /// </summary>
        /// <returns></returns>
        public List<OUMotivosReprogramacionDC> ObtenerMotivosReprogramacion()
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerMotivosReprogramacion();
        }

        /// <summary>
        /// Retorna la recogida por punto de servicio
        /// </summary>
        /// <param name="idSolicitud"></param>
        /// <returns></returns>
        public OURecogidasDC ObtenerRecogidaPuntoServicio(long idSolicitud)
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerRecogidaPuntoServicio(idSolicitud);
        }

        /// <summary>
        /// Retorna la recogida del cliente convenio
        /// </summary>
        /// <param name="idSolicitud"></param>
        /// <returns></returns>
        public OURecogidasDC ObtenerRecogidaConvenio(long idSolicitud)
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerRecogidaConvenio(idSolicitud);
        }

        /// <summary>
        /// Actualiza el reporte de la recogida al mensajero
        /// </summary>
        /// <param name="programacion"></param>
        public void ActualizaReporteMensajero(OUProgramacionSolicitudRecogidaDC programacion)
        {
            OUAdministradorOperacionUrbana.Instancia.ActualizaReporteMensajero(programacion);
        }

        /// <summary>
        /// Retorna la recogida peaton
        /// </summary>
        /// <param name="idSolicitud"></param>
        /// <returns></returns>
        public OURecogidasDC ObtenerRecogidaPeaton(long idSolicitud)
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerRecogidaPeaton(idSolicitud);
        }

        /// <summary>
        /// Retorna el historico de la programacion de la recogida
        /// </summary>
        /// <param name="idSolicitud"></param>
        /// <returns></returns>
        public List<OUProgramacionSolicitudRecogidaDC> ObtenerProgramacionRecogidas(long idSolicitud)
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerProgramacionRecogidas(idSolicitud);
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
            return OUAdministradorOperacionUrbana.Instancia.ObtenerPlanillasRecogidaZonaTipoMenFecha(idZona, idTipoMensajero, fechaRecogida, idCol);
        }

        /// <summary>
        /// Actualiza la solicitud de la recogida y la planilla
        /// </summary>
        /// <param name="programacion"></param>
        public void ActualizaSolicitudRecogida(OUProgramacionSolicitudRecogidaDC programacion)
        {
            OUAdministradorOperacionUrbana.Instancia.ActualizaSolicitudRecogida(programacion);
        }
        /// <summary>
        /// Agrega una programacion y una planilla a una solicitud de recogida
        /// </summary>
        /// <param name="programacion"></param>
        public void AgregarProgramacionSolicitudRecogida(OUProgramacionSolicitudRecogidaDC programacion)
        {
            OUAdministradorOperacionUrbana.Instancia.AgregarProgramacionSolicitudRecogida(programacion);
        }

        /// <summary>
        /// Asigna una solicitud de recogida a un mensajero
        /// </summary>
        /// <returns></returns>
        public long AsignarRecogidaMensajero(OUAsignacionRecogidaMensajeroDC asignacion)
        {
            return OUAdministradorOperacionUrbana.Instancia.AsignarRecogidaMensajero(asignacion);
        }

        /// <summary>
        /// Actualiza la georeferenciacion de una recogida
        /// </summary>
        /// <param name="longitud"></param>
        /// <param name="latitud"></param>
        /// <param name="idRecogida"></param>
        public void ActualizarGeoreferenciacionRecogida(string longitud, string latitud, long idRecogida)
        {
            OUAdministradorOperacionUrbana.Instancia.ActualizarGeoreferenciacionRecogida(longitud, latitud, idRecogida);
        }


        /// <summary>
        /// Obtiene los mensajero por centro logistico y tipo
        /// </summary>
        /// <param name="idCentroLogistico"></param>
        /// <param name="idTipoMensajero"></param>
        /// <returns></returns>
        public List<OUNombresMensajeroDC> ObtenerMensajerosPorTipo(long idCentroLogistico, int idTipoMensajero)
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerMensajerosPorTipo(idCentroLogistico, idTipoMensajero);
        }



        /// <summary>
        /// Obtiene todas las recogidas asignadas a un mensajero en un dia
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public List<OURecogidasDC> ObtenerRecogidasMensajerosDia(long idMensajero)
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerRecogidasMensajerosDia(idMensajero);
        }


        /// <summary>
        /// Obtiene todas las recogidas creadas por un cliente movil en un dia
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public List<OURecogidasDC> ObtenerRecogidasClienteMovilDia(string tokenDispositivo)
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerRecogidasClienteMovilDia(tokenDispositivo);
        }

        /// <summary>
        /// Selecciona todas las recogidas vencidas que fueron asignadas a los usuarioMensajero (usuarios PAM ) en un dia
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public List<OURecogidasDC> ObtenerRecogidasVencidasMensajerosPAMDia()
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerRecogidasVencidasMensajerosPAMDia();
        }


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
            return OUAdministradorOperacionUrbana.Instancia.ObtenerPlanillasRecogidas(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, idCol, incluyeFecha);
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
            return OUAdministradorOperacionUrbana.Instancia.ObtenerProgramacionRecogidasSinPlanillar(idZona, idTipoMensajero, idCol, fechaRecogidas);
        }



        /// <summary>
        /// Guarda la planilla de recogidas con las recogidas seleccionadas
        /// </summary>
        /// <param name="programacion"></param>
        public void GuardaPlanillaRecogidas(OUProgramacionSolicitudRecogidaDC programacion)
        {
            OUAdministradorOperacionUrbana.Instancia.GuardaPlanillaRecogidas(programacion);
        }

        /// <summary>
        /// Obtiene la planilla de programacion de recogidas
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <param name="tipoRecogida"></param>
        /// <returns></returns>
        public OUProgramacionSolicitudRecogidaDC ObtenerPlanillaRecogida(long idPlanilla)
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerPlanillaRecogida(idPlanilla);
        }

        #endregion Programacion de recogidas

        #region Descargue Recogidas

        /// <summary>
        /// Retorna los motivos de descargue de recogidas
        /// </summary>
        /// <returns></returns>
        public List<OUMotivoDescargueRecogidasDC> ObtenerMotivosDescargueRecogidas()
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerMotivosDescargueRecogidas();
        }

        /// <summary>
        /// Retorna los motivos de descargue de recogidas firltrado por idmotivo
        /// </summary>
        /// <returns></returns>
        public OUMotivoDescargueRecogidasDC ObtenerMotivosDescargueRecogidasIdMotivo(int idMotivo)
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerMotivosDescargueRecogidasIdMotivo(idMotivo);
        }

        /// <summary>
        /// Obtiene las recogidas de la planilla
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <param name="tipoRecogida"></param>
        /// <returns></returns>
        public List<OURecogidasDC> ObtenerRecogidasPlanilla(long idPlanilla)
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerRecogidasPlanilla(idPlanilla);
        }

        /// <summary>
        /// Guarda el descargue de una recogida
        /// </summary>
        /// <param name="AdminPlanilla"></param>
        public void GuardarDescargueRecogida(OUProgramacionSolicitudRecogidaDC AdminPlanilla)
        {
            OUAdministradorOperacionUrbana.Instancia.GuardarDescargueRecogida(AdminPlanilla);
        }

        /// <summary>
        /// Guarda el descargue de la recogida peaton
        /// </summary>
        /// <param name="recogida"></param>
        public void GuardarDescargueRecogidaPeaton(OUDescargueRecogidaMensajeroDC descargue)
        {
            OUAdministradorOperacionUrbana.Instancia.GuardarDescargueRecogidaPeaton(descargue);
        }

        /// <summary>
        /// cancela una recogida de peaton
        /// </summary>
        /// <param name="descargue"></param>
        public void CancelarRecogidaPeaton(OUDescargueRecogidaMensajeroDC descargue)
        {
            OUAdministradorOperacionUrbana.Instancia.CancelarRecogidaPeaton(descargue);
        }


        /// <summary>
        /// Actualiza el estado de la solicitud de uan recogida e inserta el estado traza
        /// </summary>
        public void ActualizarEstadoSolicitudRecogida(long idRecogida, OUEnumEstadoSolicitudRecogidas estado)
        {
            OUAdministradorOperacionUrbana.Instancia.ActualizarEstadoSolicitudRecogida(idRecogida, estado);
        }



        #endregion Descargue Recogidas

        #region Apertura Recogidas

        /// <summary>
        /// Obtiene los motivos de apertura de una recogida
        /// </summary>
        /// <returns></returns>
        public List<OUMotivoAperturaDC> ObtenerMotivosAperturaRecogida()
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerMotivosAperturaRecogida();
        }

        /// <summary>
        /// Abre una recogida
        /// </summary>
        /// <param name="recogida"></param>
        public void AbrirRecogida(OUAperturaRecogidaDC recogida)
        {
            OUAdministradorOperacionUrbana.Instancia.AbrirRecogida(recogida);
        }

        #endregion Apertura Recogidas

        /// <summary>
        /// retorna el valor del parametro
        /// </summary>
        /// <param name="idParametro"></param>
        /// <returns></returns>
        public string ObtenerValorParametro(string idParametro)
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerValorParametro(idParametro);
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
        public GenericoConsultasFramework<OUParametrosDC> ObtenerParametrosOperacionUrbana(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente)
        {
            int totalRegistros = 0;
            return new GenericoConsultasFramework<OUParametrosDC>()
            {
                Lista = OUAdministradorOperacionUrbana.Instancia.ObtenerParametrosOperacionUrbana(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Edita el parametro de operacion urbana
        /// </summary>
        /// <param name="parametro"></param>
        public void EditarParametroOperacionUrbana(OUParametrosDC parametro)
        {
            OUAdministradorOperacionUrbana.Instancia.EditarParametroOperacionUrbana(parametro);
        }


        #region Asignacion de tulas y precintos

        /// <summary>
        /// Método para obtener los tipos de asignación posibles
        /// </summary>
        /// <returns></returns>
        public IEnumerable<OUTipoAsignacionDC> ObtenerTiposAsignacion()
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerTiposAsignacion();
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
        public GenericoConsultasFramework<OUAsignacionDC> ObtenerAsignacionCentroServicio(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente)
        {

            int totalRegistros = 0;
            return new GenericoConsultasFramework<OUAsignacionDC>()
            {
                Lista = OUAdministradorOperacionUrbana.Instancia.ObtenerAsignacionCentroServicio(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Método para asignar una tula y un precinto a un centro de servicio
        /// </summary>
        /// <param name="asignacionTula"></param>
        /// <returns></returns>
        public OUAsignacionDC AdicionarAsignacionCentroServicio(OUAsignacionDC asignacion)
        {
            return OUAdministradorOperacionUrbana.Instancia.AdicionarAsignacionCentroServicio(asignacion);
        }


        /// <summary>
        /// Método para eliminar una asignacion de tulas o contenedores
        /// </summary>
        /// <param name="asignacion"></param>
        public void EliminarAsignacionTulaContenedor(OUAsignacionDC asignacion)
        {
            OUAdministradorOperacionUrbana.Instancia.EliminarAsignacionTulaContenedor(asignacion);
        }

        #endregion

        #region Novedades de ingreso

        /// <summary>
        /// Método para obtener las novedades de ingreso
        /// </summary>
        /// <returns></returns>
        public List<OUNovedadIngresoDC> ObtenerNovedadesIngreso()
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerNovedadesIngreso();
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
            return OUAdministradorOperacionUrbana.Instancia.ObtenerAsignaciones(controlTrans, noPrecinto, noConsolidado, mensajero, idCentroServicio);
        }


        /// <summary>
        /// Método para validar una guía suelta a centro de acopio 
        /// </summary>
        /// <param name="guia"></param>
        /// <returns></returns>
        public OUPlanillaVentaGuiasDC IngresarGuiaSuelta(OUPlanillaVentaGuiasDC guia, List<OUNovedadIngresoDC> listaNovedades)
        {
            return OUAdministradorOperacionUrbana.Instancia.IngresarGuiaSuelta(guia, listaNovedades);
        }

        #endregion

        /// <summary>
        /// Aprovisiona guía catalogada como "fantasma", es decir una numeración que debería ser automática
        /// </summary>
        /// <param name="numGUia"></param>
        /// <param name="idCs"></param>
        public bool AprovisionGuiaFantasma(long numGUia, long idCs)
        {
            return OUAdministradorOperacionUrbana.Instancia.AprovisionGuiaFantasma(numGUia, idCs);
        }

        public long CrearAuditoriaAsignacionMensajero(long idCs, long idMensajero)
        {
            return OUAdministradorOperacionUrbana.Instancia.CrearAuditoriaAsignacionMensajero(idCs, idMensajero);
        }

        public void CrearAuditoriaAsignacionMensajeroGuia(long idAuditoria, int esSobrante, long idMensajero, DateTime fecha)
        {
            OUAdministradorOperacionUrbana.Instancia.CrearAuditoriaAsignacionMensajeroGuia(idAuditoria, esSobrante, idMensajero, fecha);
        }

        public void ActualizarAuditoriaAsignacionMensajeroGuia(long idAuditoria, int esSobrante, long numeroGuia)
        {
            OUAdministradorOperacionUrbana.Instancia.ActualizarAuditoriaAsignacionMensajeroGuia(idAuditoria, esSobrante, numeroGuia);
        }

        public List<OUGuiaIngresadaDC> ObtenerAuditoriasPorMensajero(long IdAuditoria, DateTime fechaIni, DateTime fechaFin)
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerAuditoriasPorMensajero(IdAuditoria, fechaIni, fechaFin);
        }

        public List<OUGuiaIngresadaDC> ObtenerGuiasPorAuditoria(long idAuditoria)
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerGuiasPorAuditoria(idAuditoria);
        }

        public List<OUGuiaIngresadaDC> ObtenerDetalleGuiasAuditadas(long idAuditoria)
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerDetalleGuiasAuditadas(idAuditoria);
        }


        public List<COTipoNovedadGuiaDC> ObtenerTiposNovedadGuia(COEnumTipoNovedad tipoNovedad)
        {
            return EGTipoNovedadGuia.ObtenerTiposNovedadGuia(tipoNovedad);
        }

        public PUAgenciaDeRacolDC ObtenerRacolResponsable(long idAgencia)
        {
            return PUCentroServicios.Instancia.ObtenerRacolResponsable(idAgencia);
        }


        public long AdicionarMovimientoInventario(PUMovimientoInventario movimientoInventario)
        {

            return PUCentroServicios.Instancia.AdicionarMovimientoInventario(movimientoInventario);
        }

        public List<OURecogidasDC> ObtenerMisRecogidasClientePeaton(string tokenDispositivo)
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerMisRecogidasClientePeaton(tokenDispositivo);
        }

        public List<string> ObtenerImagenesSolicitudRecogida(long idSolicitudRecogida)
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerImagenesSolicitudRecogida(idSolicitudRecogida);

        }

        public void CalificarSolicitudRecogida(long idSolicitudRecogida, int calificacion, string observaciones)
        {
            OUAdministradorOperacionUrbana.Instancia.CalificarSolicitudRecogida(idSolicitudRecogida, calificacion, observaciones);
        }

        #region Obtener mensajeros pam por centro de servicio
        public List<OUMensajeroPamDC> ObtenerMensajerosPamPorCentroServicio(long IdCentroServicio)
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerMensajerosPamPorCentroServicio(IdCentroServicio);
        }
        #endregion

        #region Obtener lista guias mensajeros pam por centro de servicio
        public List<LIReclameEnOficinaDC> ObtenerGuiasDelPamPorCentroServicio(long IdCentroServicio, string usuario)
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerGuiasDelPamPorCentroServicio(IdCentroServicio, usuario);
        }
        #endregion

        #region ingresa guia pam
        public LIReclameEnOficinaDC IngresoGuiaPam(LIReclameEnOficinaDC GuiasPam)
        {
            return OUAdministradorOperacionUrbana.Instancia.IngresoGuiaPam(GuiasPam);
        }
        #endregion




        /// <summary>
        /// Obtiene los mensajeros de una punto especifico
        /// </summary>
        /// <param name="centroLogistico"></param>
        /// <returns></returns>
        public IList<OUMensajeroDC> ObtenerMensajerosAuditores(long puntoServicio, bool esAgencia)
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerMensajerosAuditores(puntoServicio, esAgencia);
        }
        /// <summary>
        /// metodo para obtener motivo y fecha intento de entrega por numero de guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="idMotivoGuia"></param>
        /// <returns></returns>
        public LIMotivoEvidenciaGuiaDC ObtenerFechaIntentoYMotivoGuia(long numeroGuia, long idPlanilla)
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerFechaIntentoYMotivoGuia(numeroGuia, idPlanilla);
        }
        /// <summary>
        /// Metodo que consulta las guias planilladas al auditor
        /// </summary>
        /// <param name="idAuditor"></param>
        /// <returns></returns>
        public IList<OUGuiaIngresadaDC> ObtenerGuiasAuditor(long idAuditor)
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerGuiasAuditor(idAuditor);
        }

        /// <summary>
        /// Metodo para obtener las guias 
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public IList<OUGuiaIngresadaDC> ObtenerGuiasMensajero(long idMensajero)
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerGuiasMensajero(idMensajero);
        }

        /// <summary>
        /// Metodo para obtener informacion del usuario (mensajero, auditor)
        /// </summary>
        /// <param name="numIdentificacion"></param>
        /// <returns></returns>
        public OUMensajeroDC ObtenerInformacionUsuarioControllerApp(string numIdentificacion)
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerInformacionUsuarioControllerApp(numIdentificacion);
        }

        #region Auditoria Mensajero/Auditor Controller App
        /// <summary>
        /// Metodo para obtener guias planillas por mensajero 
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public List<OUGuiaIngresadaAppDC> ObtenerGuiasMensajeroEnZona(long idMensajero)
        {
            return OUAuditoriaControllerApp.Instancia.ObtenerGuiasMensajeroEnZona(idMensajero);
        }

        /// <summary>
        /// Metodo para obtener las guias entregadas planilladas del mensajero
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public List<OUGuiaIngresadaAppDC> ObtenerGuiasEntregadasMensajero(long idMensajero)
        {
            return OUAuditoriaControllerApp.Instancia.ObtenerGuiasEntregadasMensajero(idMensajero);
        }

        /// <summary>
        /// Metodo para consultar las guias en devolucion del mensajero 
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public List<OUGuiaIngresadaAppDC> ObtenerGuiasDevolucionMensajero(long idMensajero)
        {
            return OUAuditoriaControllerApp.Instancia.ObtenerGuiasDevolucionMensajero(idMensajero);
        }

        /// <summary>
        /// Metodo para obtener las guias en zona auditor
        /// </summary>
        /// <param name="idAuditor"></param>
        /// <returns></returns>
        public List<OUGuiaIngresadaAppDC> ObtenerGuiasEnZonaAuditor(long idAuditor)
        {
            return OUAuditoriaControllerApp.Instancia.ObtenerGuiasEnZonaAuditor(idAuditor);
        }

        /// <summary>
        /// Metodo para obtener las guias entregadas por auditor en zona
        /// </summary>
        /// <param name="idAuditor"></param>
        /// <returns></returns>
        public List<OUGuiaIngresadaAppDC> ObtenerGuiasEntregadasAuditor(long idAuditor)
        {
            return OUAuditoriaControllerApp.Instancia.ObtenerGuiasEntregadasAuditor(idAuditor);
        }

        /// <summary>
        /// Metodo para obtener las devoluciones del auditor
        /// </summary>
        /// <param name="idAuditor"></param>
        /// <returns></returns>
        public List<OUGuiaIngresadaAppDC> ObtenerGuiasDevolucionAuditor(long idAuditor)
        {
            return OUAuditoriaControllerApp.Instancia.ObtenerGuiasDevolucionAuditor(idAuditor);
        }

        /// <summary>
        /// Metodo para obtener la guia planillada para descargue 
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public OUGuiaIngresadaAppDC ObtenerGuiaEnPlanillaDescargue(long numeroGuia, long idMensajero)
        {
            return OUAuditoriaControllerApp.Instancia.ObtenerGuiaEnPlanillaDescargue(numeroGuia, idMensajero);
        }
        /// <summary>
        /// Metodo para obtener la guia planillada al auditor para descargar con controller app
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public OUGuiaIngresadaAppDC ObtenerGuiaEnPlanillaAuditorDescargue(long numeroGuia, long idMensajero)
        {
            return OUAuditoriaControllerApp.Instancia.ObtenerGuiaEnPlanillaAuditorDescargue(numeroGuia, idMensajero);
        }


        #endregion

        #region Sispostal - Modulo de Masivos

        /// <summary>
        /// Metodo para obtener guias por estado Sispostal
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <param name="estado"></param>
        /// <returns></returns>
        public List<OUGuiaIngresadaAppDC> ObtenerGuiasMensajeroEnZonaMasivos(long idMensajero, short estado)
        {
            return OUAuditoriaControllerApp.Instancia.ObtenerGuiasMensajeroEnZonaMasivos(idMensajero, estado);
        }

        #endregion
    }
}