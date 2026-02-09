using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.ParametrosOperacion;
using CO.Servidor.Dominio.Comun.Rutas;
using CO.Servidor.OperacionNacional.Datos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.OperacionNacional;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion;
using CO.Servidor.Servicios.ContratoDatos.Rutas;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.OperacionNacional
{
    /// <summary>
    /// Administrador para la operacion nacional
    /// </summary>
    public class ONAdministradorOperacionNacional
    {
        private static readonly ONAdministradorOperacionNacional instancia = new ONAdministradorOperacionNacional();

        public static ONAdministradorOperacionNacional Instancia
        {
            get { return ONAdministradorOperacionNacional.instancia; }
        }

        /// <summary>
        /// constructor
        /// </summary>
        private ONAdministradorOperacionNacional() { }

        /// <summary>
        /// Obtiene  los manifiestos de la operacion nacional
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consult</param>
        /// <param name="idCentroServiciosManifiesta">Centro de servicios que manifiesta</param>
        /// <returns>Lista con los vehiculos</returns>
        public IList<ONManifiestoOperacionNacional> ObtenerManifiestosOperacionNacional(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina, long idCentroServiciosManifiesta, bool incluyeFecha)
        {
            return ONOperacionNacional.Instancia.ObtenerManifiestosOperacionNacional(filtro, indicePagina, registrosPorPagina, idCentroServiciosManifiesta, incluyeFecha);
        }
        /// <summary>
        /// Indica si una guía, dado su número y el id de la agencia, ya ha sido ingresada a centro de acopio pero no había sido creada la guía como tal al momento del ingreso
        /// </summary>
        /// <param name="numeroGuia"></param>.
        /// <param name="idAgencia"></param>
        /// <returns>True si la guía ya fué ingresada</returns>
        public bool GuiaYaFueIngresadaACentroDeAcopio(long numeroGuia, long idAgencia)
        {
            return ONOperacionNacional.Instancia.GuiaYaFueIngresadaACentroDeAcopio(numeroGuia, idAgencia);
        }

        /// <summary>
        /// Indica si una guía, dado su número ya ha sido ingresasda a centro de copio antes de haberla creado en el sistema.
        /// </summary>
        /// <param name="numeroGuia">Retorna el id del centro de servicio que ingresó a centro de acopio la guía</param>
        /// <returns></returns>
        public long GuiaYaFueIngresadaACentroDeAcopioRetornaCS(long numeroGuia)
        {
            return ONOperacionNacional.Instancia.GuiaYaFueIngresadaACentroDeAcopioRetornaCentroServicio(numeroGuia);
        }

        /// <summary>
        /// Obtiene todas las rutas filtradas a partir de una localidad de origen
        /// </summary>
        /// <param name="IdLocalidad"></param>
        /// <returns></returns>
        public IList<RURutaDC> ObtenerRutasXLocalidadOrigen(string idLocalidad)
        {
            return COFabricaDominio.Instancia.CrearInstancia<IFachadaRutas>().ObtenerRutasXLocalidadOrigen(idLocalidad);
        }

        /// <summary>
        /// Obtiene las rutas a las cuales pertenece una estación
        /// </summary>
        /// <param name="idLocalidadEstacion"></param>
        /// <returns></returns>
        public List<RURutaDC> ObtenerRutasPerteneceEstacion(string idLocalidadEstacion)
        {
            return COFabricaDominio.Instancia.CrearInstancia<IFachadaRutas>().ObtenerRutasPerteneceEstacion(idLocalidadEstacion);
        }

        /// <summary>
        /// Obtiene las rutas a las cuales pertenece una estacion, incluye las rutas en las que la estacion es origen y destino
        /// </summary>
        /// <param name="idLocalidadEstacion"></param>
        /// <returns></returns>
        public List<RURutaDC> ObtenerRutasPerteneceEstacionOrigDest(string idLocalidadEstacion)
        {
            return COFabricaDominio.Instancia.CrearInstancia<IFachadaRutas>().ObtenerRutasPerteneceEstacionOrigDest(idLocalidadEstacion);
        }

        /// <summary>
        /// Obtiene las empresas transportadoras asociadas a una ruta
        /// </summary>
        /// <param name="idRuta">id de la ruta</param>
        /// <returns></returns>
        public IList<RUEmpresaTransportadora> ObtenerEmpresasTransportadorasXRuta(int idRuta)
        {
            return COFabricaDominio.Instancia.CrearInstancia<IFachadaRutas>().ObtenerEmpresasTransportadorasXRuta(idRuta);
        }

        /// <summary>
        /// Obtiene las empresas transportadoras asociadas a una racol
        /// </summary>
        /// <param name="idRuta">id de la ruta</param>
        /// <returns></returns>
        public IList<RUEmpresaTransportadora> ObtenerEmpresasTransportadorasXRacol(int idRacol)
        {
            return COFabricaDominio.Instancia.CrearInstancia<IFachadaRutas>().ObtenerEmpresasTransportadorasXRacol(idRacol);
        }

        /// <summary>
        /// Obtiene una lista de los vehiculos activos que pertenecen a un racol y que tienen ingreso a un col
        /// </summary>
        /// <param name="idRacol">Id del racol al que pertenece el vehiculo</param>
        /// <param name="idCentroServicios">Id del centro de servicios al que ingreso el vehiculo</param>
        /// <returns></returns>
        public IList<POVehiculo> ObtenerVehiculosIngresoCentroServicioXRacol(long idRacol, long idCentroServicios, bool esCol)
        {
            return ONOperacionNacional.Instancia.ObtenerVehiculosIngresoCentroServicioXRacol(idRacol, idCentroServicios, esCol);
        }

        /// <summary>
        /// <summary>
        /// Obtiene una lista de los conductores activos y con pase vigente asignados a un vehiculo
        /// </summary>
        /// <param name="idVehiculo"></param>
        /// <returns></returns>
        public IList<POConductores> ObtenerConductoresActivosVehiculos(int idVehiculo)
        {
            return COFabricaDominio.Instancia.CrearInstancia<IPOFachadaParametrosOperacion>().ObtenerConductoresActivosVehiculos(idVehiculo);
        }

        /// <summary>
        /// Inserta, actualiza un manifiesto nacional
        /// </summary>
        /// <param name="manifiesto"></param>
        public ONManifiestoOperacionNacional ActualizarManifiestoOperacionNacional(ONManifiestoOperacionNacional manifiesto)
        {
            return ONOperacionNacional.Instancia.ActualizarManifiestoOperacionNacional(manifiesto);
        }

        /// <summary>
        /// Obtiene las estaciones y localidades adicionales de una ruta
        /// </summary>
        /// <param name="idRuta"></param>
        /// <returns>Lista con las estaciones de la ruta</returns>
        public IList<RUEstacionRuta> ObtenerEstacionesYLocalidadesAdicionalesRuta(int idRuta)
        {
            return COFabricaDominio.Instancia.CrearInstancia<IFachadaRutas>().ObtenerEstacionesYLocalidadesAdicionalesRuta(idRuta);
        }

        /// <summary>
        /// Obtiene las estaciones-ruta de un Manifiesto
        /// </summary>
        /// <param name="idManifiesto"></param>
        /// <returns>Lista con las estaciones-ruta de un Manifiesto</returns>
        public IList<RUEstacionRuta> ObtenerEstacionesRutaDeManifiesto(long idManifiesto)
        {
            return COFabricaDominio.Instancia.CrearInstancia<IFachadaRutas>().ObtenerEstacionesRutaDeManifiesto(idManifiesto);
        }

        /// <summary>
        /// Obtiene los consolidados de un manifiesto de la operacion nacional
        /// </summary>
        /// <param name="idManifiestoOperacionNacional">Identificador del manifiesto de la operacion nacional</param>
        /// <returns></returns>
        public IList<ONConsolidado> ObtenerConsolidadosManifiesto(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long idManifiestoOperacionNacional, string idLocalidad)
        {
            return ONOperacionNacional.Instancia.ObtenerConsolidadosManifiesto(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idManifiestoOperacionNacional, idLocalidad);
        }

        /// <summary>
        /// Obtiene todos los tipos de consolidado
        /// </summary>
        /// <returns></returns>
        public IList<ONTipoConsolidado> ObtenerTipoConsolidado()
        {
            return ONOperacionNacional.Instancia.ObtenerTipoConsolidado();
        }

        // TODO:ID Obtener Lista Tipos de Novedad Estacion-Ruta
        public IList<ONTipoNovedadEstacionRutaDC> ObtenerTiposNovedadEstacionRuta()
        {
            return ONOperacionNacional.Instancia.ObtenerTiposNovedadEstacionRuta();
        }

        /// <summary>
        /// Obtiene todos los detalles de los tipos de consolidado
        /// </summary>
        /// <returns></returns>
        public IList<ONTipoConsolidadoDetalle> ObtenerTipoConsolidadoDetalle(int idTipoConsolidado)
        {
            return ONOperacionNacional.Instancia.ObtenerTipoConsolidadoDetalle(idTipoConsolidado);
        }

        /// <summary>
        /// Inserta un consolidado
        /// </summary>
        /// <param name="consolidado"></param>
        /// <returns></returns>
        public ONConsolidado AdicionarConsolidado(ONConsolidado consolidado, long idCentroServicios)
        {
            return ONOperacionNacional.Instancia.AdicionarConsolidado(consolidado, idCentroServicios);
        }


        /// <summary>
        /// Modifica un consolidado
        /// </summary>
        /// <param name="consolidado"></param>
        /// <returns></returns>
        public void EditarConsolidado(ONConsolidado consolidado, long idCentroServicios)
        {
            ONOperacionNacional.Instancia.EditarConsolidado(consolidado, idCentroServicios);
        }

        /// <summary>
        /// Inserta un envio al consolidado
        /// </summary>
        /// <param name="consolidadoDetalle"></param>
        /// <returns></returns>
        public ONConsolidadoDetalle AdicionarConsolidadoDetalle(ONConsolidadoDetalle consolidadoDetalle)
        {
            return ONOperacionNacional.Instancia.AdicionarConsolidadoDetalle(consolidadoDetalle);
        }

        /// <summary>
        /// Obtiene las guias de un consolidado
        /// </summary>
        /// <param name="idConsolidado"></param>
        /// <returns></returns>
        public IList<ONConsolidadoDetalle> ObtenerGuiasConsolidado(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina, long idConsolidado)
        {
            return ONOperacionNacional.Instancia.ObtenerGuiasConsolidado(filtro, indicePagina, registrosPorPagina, idConsolidado);
        }

        /// <summary>
        /// Obtiene todas las guias manifestadas, incluye guias sueltas y guias consolidadas
        /// </summary>
        /// <returns></returns>
        public IList<ONManifiestoGuia> ObtenerTodasGuiasManifiesto(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long idManifiestoOperacionNacional)
        {
            return ONOperacionNacional.Instancia.ObtenerTodasGuiasManifiesto(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idManifiestoOperacionNacional);
        }

        /// <summary>
        /// Inserta una guia suelta a un manifiesto
        /// </summary>
        /// <param name="guia"></param>
        public ONManifiestoGuia AdicionarGuiaSuelta(ONManifiestoGuia guia)
        {
            return ONOperacionNacional.Instancia.AdicionarGuiaSuelta(guia);
        }

        /// Obtiene los consolidados y las guias sueltas para la impresion del manifiesto por ruta
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <param name="idCentroServicios"></param>
        /// <returns></returns>
        public List<ONImpresionManifiestoDC> ObtenerImpresionManifiestoRuta(long idManifiesto, List<string> ciudadesAManifestar)
        {
            return ONOperacionNacional.Instancia.ObtenerImpresionManifiestoRuta(idManifiesto, ciudadesAManifestar);
        }

        /// <summary>
        /// Obtiene todas las guias sueltas de un manifiesto nacional
        /// </summary>
        /// <param name="idManifiestoOpeNacional"></param>
        /// <returns></returns>
        public IList<ONManifiestoGuia> ObtenerGuiasSueltas(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina, long idManifiestoOpeNacional)
        {
            return ONOperacionNacional.Instancia.ObtenerGuiasSueltas(filtro, indicePagina, registrosPorPagina, idManifiestoOpeNacional);
        }

        /// <summary>
        /// Obtiene todos los motivos de eliminacion de una guia
        /// </summary>
        /// <returns></returns>
        public IList<ONTipoMotivoElimGuiaMani> ObtenerTodosMotivosEliminacionGuia()
        {
            return ONOperacionNacional.Instancia.ObtenerTodosMotivosEliminacionGuia();
        }

        /// <summary>
        /// Elimina una guia suelta de un manifiesto o una guia de un consolidado de un manifiesto
        /// </summary>
        /// <param name="motivo"></param>
        public void EliminarGuiaManifiesto(ONMotivoElimGuiaMani motivo)
        {
            ONOperacionNacional.Instancia.EliminarGuiaManifiesto(motivo);
        }

        /// <summary>
        /// Cierra un manifiesto
        /// </summary>
        /// <param name="idManifiesto"></param>
        public void CerrarManifiesto(long idManifiesto, DateTime fechaSalida)
        {
            ONOperacionNacional.Instancia.CerrarManifiesto(idManifiesto, fechaSalida);
        }

        /// <summary>
        /// Reabre un manifiesto
        /// </summary>
        /// <param name="idManifiesto"></param>
        public void AbrirManifiesto(ONManifiestoOperacionNacional manifiesto)
        {
            ONOperacionNacional.Instancia.AbrirManifiesto(manifiesto);
        }

        /// <summary>
        /// Genera el reporte del manifiesto de carga
        /// </summary>
        /// <param name="idManifiesto"></param>
        public ONReporteManifiestoCarga GenerarReporteManifiestoCarga(long idManifiesto, long IdCentroServiciosManifiesta)
        {
            return ONOperacionNacional.Instancia.GenerarReporteManifiestoCarga(idManifiesto, IdCentroServiciosManifiesta);
        }
        /// <summary>
        /// para obtener parametros guias en novedades de ruta cuando se requiera asignar novedad a guias especificas
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="guiaxNovIndv"></param>
        /// <returns></returns>
        public List<ONParametrosGuiaInvNovedadRuta> ObtenerParametrosGuiaIndvPorNovedadRuta(long numeroGuia, ONEnumGuiaIndvNovedadRuta guiaxNovIndv, string tipoUbicacion)
        {
            return ONOperacionNacional.Instancia.ObtenerParametrosGuiaIndvPorNovedadRuta(numeroGuia, guiaxNovIndv, tipoUbicacion);
        }
        /// <summary>
        /// obtiene una lista de guias en el centro de acopio, que debieron haber sido agregadas al manifiesto
        /// </summary>
        /// <param name="manifiesto"></param>
        /// <returns></returns>
        public List<ADGuia> ObtenerEnviosPendientesPorManifestar(ONManifiestoOperacionNacional manifiesto)
        {
            return ONOperacionNacional.Instancia.ObtenerEnviosPendientesPorManifestar(manifiesto);
        }

        #region Ingreso COL por ruta

        /// <summary>
        /// Valida el vehiculo seleccionado para poder hacer el ingreso a col
        /// </summary>
        public bool ValidacionVehiculoIngreso(ONIngresoOperativoDC ingreso)
        {
            return ONManejadorIngresoRuta.Instancia.ValidacionVehiculoIngreso(ingreso);
        }

        /// <summary>
        /// Obtiene los envios consolidados de los manifiestos asociados al vehiculo
        /// </summary>
        /// <param name="idVehiculo"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        public List<ONManifiestoOperacionNacional> ObtenerEnviosConsolidadosManifiestoVehiculo(int idVehiculo, int idRuta, int indicePagina, int registrosPorPagina)
        {
            return ONManejadorIngresoRuta.Instancia.ObtenerEnviosConsolidadosManifiestoVehiculo(idVehiculo, idRuta, indicePagina, registrosPorPagina);
        }

        /// <summary>
        /// Obtiene los consolidados de los manifiestos abiertos asociados al vehiculo
        /// </summary>
        /// <param name="idVehiculo">id del vehiculo</param>
        /// <returns></returns>
        public List<ONConsolidado> ObtenerConsolidadosManifiestoVehiculo(int idVehiculo, int idRuta, int indicePagina, int registrosPorPagina)
        {
            return ONManejadorIngresoRuta.Instancia.ObtenerConsolidadosManifiestoVehiculo(idVehiculo, idRuta, indicePagina, registrosPorPagina);
        }

        /// <summary>
        /// Consulta todos los envios de un consolidado
        /// </summary>
        /// <param name="idVehiculo"></param>
        /// <param name="idIngresoOperativo"></param>
        /// <returns></returns>
        public List<ONConsolidadoDetalle> ObtenerTodosEnviosSueltosVehiculo(int idVehiculo, int idRuta)
        {
            return ONManejadorIngresoRuta.Instancia.ObtenerTodosEnviosSueltosVehiculo(idVehiculo, idRuta);
        }

        /// <summary>
        /// Consulta todos los envios consolidados del manifiesto
        /// </summary>
        /// <param name="idVehiculo"></param>
        /// <returns></returns>
        public List<ONConsolidadoDetalle> ObtenerTodosEnviosConsolidadoManifiesto(int idVehiculo, int idRuta)
        {
            return ONManejadorIngresoRuta.Instancia.ObtenerTodosEnviosConsolidadoManifiesto(idVehiculo, idRuta);
        }

        /// <summary>
        /// Obtiene los estados del empaque para el parametro de peso especificado
        /// </summary>
        /// <param name="idParametro"></param>
        /// <returns></returns>
        public List<PAEstadoEmpaqueDC> ObtenerEstadosEmpaqueParametroPeso(string idParametro)
        {
            return ONManejadorIngresoRuta.Instancia.ObtenerEstadosEmpaqueParametroPeso(idParametro);
        }

        /// <summary>
        /// Guarda el ingreso a la agencia del vehiculo del operativo
        /// </summary>
        /// <param name="ingresoOperativo"></param>
        /// <returns></returns>
        public long GuardarIngresoAgenciaRuta(ONIngresoOperativoDC ingresoOperativo)
        {
            return ONManejadorIngresoRuta.Instancia.GuardarIngresoAgenciaRuta(ingresoOperativo);
        }

        /// <summary>
        /// Guarda la novedad del consolidado
        /// </summary>
        /// <param name="idConsolidado">id del consolidado </param>
        /// <param name="descripcion">Descripcion de la novedad</param>
        /// <param name="numeroPrecintoIngreso">numero del precinto de ingreso</param>
        /// <param name="numeroTulaContenedor">número de tula o contendor de ingreso</param>
        public void GuardarNovedadConsolidado(ONConsolidado consolidado, string descripcion)
        {
            ONManejadorIngresoRuta.Instancia.GuardarNovedadConsolidado(consolidado, descripcion);
        }

        /// <summary>
        /// Guarda el envio ingresado
        /// </summary>
        /// <param name="ingreso"></param>
        public ONIngresoOperativoDC GuardaIngresoEnvioAgencia(ONIngresoOperativoDC ingreso)
        {
            return ONManejadorIngresoRuta.Instancia.GuardaIngresoEnvioAgencia(ingreso);
        }

        /// <summary>
        /// Retorna los envios del consolidado seleccionado
        /// </summary>
        /// <param name="idConsolidado"></param>
        /// <returns></returns>
        public List<ONConsolidadoDetalle> ObtenerEnviosConsolidado(long idConsolidado)
        {
            return ONManejadorIngresoRuta.Instancia.ObtenerEnviosConsolidado(idConsolidado);
        }

        /// <summary>
        /// Obtiene los manifiestos sin descargar de un vehiculo
        /// </summary>
        /// <param name="idVehiculo"></param>
        /// <returns></returns>
        public List<ONManifiestoOperacionNacional> ObtenerManifiestosVehiculo(int idVehiculo)
        {
            return ONManejadorIngresoRuta.Instancia.ObtenerManifiestosVehiculo(idVehiculo);
        }

        /// <summary>
        /// Obtiene los envios sueltos del manifiesto
        /// </summary>
        /// <param name="idVehiculo">id del vehiculo</param>
        /// <param name="idIngresoOperativo">id del ingreso a la agencia del vehiculo</param>
        /// <param name="estaDescargada">bit para saber si esta descargado o no el manifiesto</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize">tamaño de la pagina</param>
        /// <returns></returns>
        public List<ONConsolidadoDetalle> ObtenerEnviosSueltosManifiestoVehiculo(int idVehiculo, int idRuta, long idIngresoOperativo, int pageIndex, int pageSize)
        {
            return ONManejadorIngresoRuta.Instancia.ObtenerEnviosSueltosManifiestoVehiculo(idVehiculo, idRuta, idIngresoOperativo, pageIndex, pageSize);
        }

        /// <summary>
        /// Obtiene el total de los envios manifestados
        /// </summary>
        /// <param name="idVehiculo"></param>
        /// <param name="idLocalidadDestino"></param>
        /// <returns></returns>
        public int ObtenerTotalEnviosManifestadosVehiculoLocalidad(int idVehiculo, string idLocalidadDestino)
        {
            return ONManejadorIngresoRuta.Instancia.ObtenerTotalEnviosManifestadosVehiculoLocalidad(idVehiculo, idLocalidadDestino);
        }

        /// <summary>
        /// Obtiene el total de los envios sobrantes
        /// </summary>
        /// <param name="idVehiculo"></param>
        /// <param name="idLocalidadDestino"></param>
        /// <returns></returns>
        public int ObtenerTotalEnviosSobrantesVehiculoLocalidad(long idOperativo, string idLocalidadDestino)
        {
            return ONManejadorIngresoRuta.Instancia.ObtenerTotalEnviosSobrantesVehiculoLocalidad(idOperativo, idLocalidadDestino);
        }

        /// <summary>
        /// Obtiene el total de los envios descargados
        /// </summary>
        /// <param name="idVehiculo"></param>
        /// <param name="idLocalidadDestino"></param>
        /// <returns></returns>
        public int ObtenerTotalEnviosDescargadosVehiculoLocalidad(long idOperativo, string idLocalidadDestino)
        {
            return ONManejadorIngresoRuta.Instancia.ObtenerTotalEnviosDescargadosVehiculoLocalidad(idOperativo, idLocalidadDestino);
        }

        /// <summary>
        /// Guarda y valida los envios transito
        /// </summary>
        /// <param name="idGuiaInterna"></param>
        /// <param name="ingreso"></param>
        public void GuardaIngresoEnviosTransito(ONIngresoOperativoDC ingreso)
        {
            ONManejadorIngresoRuta.Instancia.GuardaIngresoEnviosTransito(ingreso);
        }

        #endregion Ingreso COL por ruta

        /// <summary>
        /// Obtiene las rutas de una localidad
        /// </summary>
        /// <param name="idLocalidad"></param>
        public List<RURutaDC> ObtenerRutasPorLocalidad(string idLocalidad)
        {
            return COFabricaDominio.Instancia.CrearInstancia<IFachadaRutas>().ObtenerRutasPorLocalidad(idLocalidad);
        }

        /// <summary>
        /// Obtiene el ingreso o salida del transportador por los parametros de entrada
        /// </summary>
        /// <param name="placa"></param>
        /// <param name="idRuta"></param>
        /// <param name="fechaInicio"></param>
        /// <param name="fechaFin"></param>
        /// <param name="identificadorConductor"></param>
        /// <param name="nombreConductor"></param>
        /// <param name="incluyeFecha"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<ONIngresoSalidaTransportadorDC> ObtenerIngresoSalidaTransportador(ONFiltroTransportadorDC filtro, int indicePagina, int registrosPorPagina, bool incluyeFecha)
        {
            return ONOperacionNacional.Instancia.ObtenerIngresoSalidaTransportador(filtro, indicePagina, registrosPorPagina, incluyeFecha);
        }

        /// <summary>
        /// Obtiene todos los conductores Activos
        /// </summary>
        public IList<POConductores> ObtenerTodosConductores()
        {
            return COFabricaDominio.Instancia.CrearInstancia<IPOFachadaParametrosOperacion>().ObtenerTodosConductores();
        }

        /// <summary>
        /// Obtiene la informacion de ingreso y salida del transportador por ID
        /// </summary>
        /// <param name="idIngrsoSalidaTransportado"></param>
        /// <returns></returns>
        public ONIngresoSalidaTransportadorDC ObtenerIngresoSalidaTransportadorPorId(long idIngrsoSalidaTransportado)
        {
            return ONOperacionNacional.Instancia.ObtenerIngresoSalidaTransportadorPorId(idIngrsoSalidaTransportado);
        }

        /// <summary>
        /// Obtiene todas las novedades operativas
        /// </summary>
        public IList<ONNovedadOperativoDC> ObtenerNovedadOperativo()
        {
            return ONOperacionNacional.Instancia.ObtenerNovedadOperativo();
        }

        /// <summary>
        /// Obtiene las estaciones y localidades adicionales de una ruta sin importar si son conolidado o no
        /// </summary>
        /// <param name="idRuta"></param>
        /// <returns>Lista con las estaciones de la ruta</returns>
        public IList<RUEstacionRuta> ObtenerEstacionesRuta(int idRuta)
        {
            return COFabricaDominio.Instancia.CrearInstancia<IFachadaRutas>().ObtenerEstacionesRuta(idRuta);
        }

        /// <summary>
        /// Ingreso en la tabla de ingreso salida transportador
        /// </summary>
        /// <param name="ingresoSalidaTrans"></param>
        public void IngresarIngresosSalidasTrasnportador(ONIngresoSalidaTransportadorDC ingresoSalidaTrans)
        {
            ONOperacionNacional.Instancia.IngresarIngresosSalidasTrasnportador(ingresoSalidaTrans);
        }

        /// <summary>
        /// Consultar la última ruta de un vehículo que ha sido manifestado,
        /// el vehículo se consulta por su placa
        /// </summary>
        public ONRutaConductorDC ObtenerUltimaRutaConduPlaca(string placa)
        {
            return ONOperacionNacional.Instancia.ObtenerUltimaRutaConduPlaca(placa);
        }

        /// <summary>
        /// Metodo para Obtener las
        /// Regionales Administrativas
        /// </summary>
        /// <returns>lista de RACOL</returns>
        public List<PURegionalAdministrativa> ObtenerRegionalAdministrativa()
        {
            IPUFachadaCentroServicios fachadaCentroServicios = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();
            return fachadaCentroServicios.ObtenerRegionalAdministrativa();
        }

        /// <summary>
        /// Metodo para Obtener los Centros de
        /// Servicios y Agencias por RACOL
        /// </summary>
        /// <param name="idRacol"> es el id del RACOL</param>
        /// <returns>lista de centros de servicio</returns>
        public List<PUCentroServiciosDC> ObtenerCentrosServicios(long idRacol)
        {
            IPUFachadaCentroServicios fachadaCentroServicios = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();
            return fachadaCentroServicios.ObtenerCentrosServicios(idRacol);
        }

        /// <summary>
        /// Obtener los consolidados a partir de la guia interna
        /// </summary>
        /// <param name="idGuiaInterna"></param>
        public ONConsolidado ObtenerConsolidadoPorIdGuia(long idGuiaInterna)
        {
            return ONOperacionNacional.Instancia.ObtenerConsolidadoPorIdGuia(idGuiaInterna);
        }

        /// <summary>
        /// Consulta las estaciones de una ruta y la cantidad de envíos manifestados en un manifiesto específico x cada estación
        /// </summary>
        /// <param name="idRuta"></param>
        /// <param name="idManifiesto"></param>
        /// <returns></returns>
        public List<ONCantEnviosManXEstacionDC> ConsultarCantEnviosManXEstacion(int idRuta, long idManifiesto)
        {
            return ONOperacionNacional.Instancia.ConsultarCantEnviosManXEstacion(idRuta, idManifiesto);
        }

        /// <summary>
        /// Cierra el ingreso del operativo para el vehiculo
        /// </summary>
        public void CerrarIngresoOperativoAgencia(ONIngresoOperativoDC ingreso)
        {
            ONManejadorIngresoRuta.Instancia.CerrarIngresoOperativoAgencia(ingreso);
        }


        /// <summary>
        /// Obtener Novedades de Trasnporte de la guia seleccionada
        /// </summary>
        /// <param name="numeroGuia"></param>
        public List<ONNovedadesTransporteDC> ObtenerNovedadesTransporteGuia(long numeroGuia)
        {
            return ONOperacionNacional.Instancia.ObtenerNovedadesTransporteGuia(numeroGuia);
        }

        // Se revisa si el Casillero es AEREO (Ciudades de la Costa en la Tabla TrayectoCasilleroServicio_OPN)
        public bool ValidarTrayectoServicioAereo(string idLocalidadOrigen, string idLocalidadDestino, int idServicio)
        {
            return ONOperacionNacional.Instancia.ValidarTrayectoServicioAereo(idLocalidadOrigen, idLocalidadDestino, idServicio);
        }

        #region Operativo Ciudad

        /// <summary>
        /// Guarda el envio ingresado
        /// </summary>
        /// <param name="ingreso"></param>
        public ONIngresoOperativoCiudadDC GuardaIngresoEnvioOperativoCiudad(ONIngresoOperativoCiudadDC ingreso)
        {
            return ONOperacionNacional.Instancia.GuardaIngresoEnvioOperativoCiudad(ingreso);
        }

        /// <summary>
        /// Guarda  el ingreso al operativo por ciudad
        /// </summary>
        /// <param name="ingreso"></param>
        /// <returns></returns>
        public long GuardarOperativoAgenciaCiudad(ONIngresoOperativoCiudadDC ingreso)
        {
            return ONOperacionNacional.Instancia.GuardarOperativoAgenciaCiudad(ingreso);
        }

        /// <summary>
        /// Cierra el ingreso del operativo para el vehiculo
        /// </summary>
        public void TerminaDescargueOperativo(long idIngresoOperativo)
        {
            ONOperacionNacional.Instancia.TerminarDescargueOperativo(idIngresoOperativo);
        }

        #endregion Operativo Ciudad

        #region Operativo por manifiesto

        /// <summary>
        /// Obtiene los manifietos donde la ciudad origen, destino o estacion de ruta sea la ciudad
        /// donde se esta haciendo el descargue
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <returns></returns>
        public List<ONManifiestoOperacionNacional> ObtenerManifiestosXLocalidad(IDictionary<string, string> filtro, string idLocalidad, int indicePagina, int registrosPorPagina)
        {
            return ONManejadorIngresoManifiesto.Instancia.ObtenerManifiestosXLocalidad(filtro, idLocalidad, indicePagina, registrosPorPagina);
        }

        /// <summary>
        /// Consulta el detalle del manifiesto
        /// </summary>
        /// <param name="idManifiesto"></param>
        /// <returns></returns>
        public ONIngresoOperativoDC ObtenerDetalleManifiesto(ONIngresoOperativoDC ingreso)
        {
            return ONManejadorIngresoManifiesto.Instancia.ObtenerDetalleManifiesto(ingreso);
        }

        /// <summary>
        /// Obtiene los envios de un manifiesto por numero de manifesto
        /// </summary>
        /// <param name="idManifiesto"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <returns></returns>
        public List<ONConsolidado> ObtenerEnviosXManifiesto(long idManifiesto, int indicePagina, int registrosPorPagina)
        {
            return ONManejadorIngresoManifiesto.Instancia.ObtenerEnviosXManifiesto(idManifiesto, indicePagina, registrosPorPagina);
        }

        /// <summary>
        /// Retorna o asigan los consolidados del manifiesto
        /// </summary>
        /// <param name="idManifiesto"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <returns></returns>
        public List<ONConsolidado> ObtenerDetalleConsolidadosManifiesto(long idManifiesto, int indicePagina, int registrosPorPagina)
        {
            return ONManejadorIngresoManifiesto.Instancia.ObtenerDetalleConsolidadosManifiesto(idManifiesto, indicePagina, registrosPorPagina);
        }

        /// <summary>
        /// Obtiene los envios de consolidados de un manifiesto
        /// </summary>
        /// <param name="idManifiesto"></param>
        public List<ONEnviosDescargueRutaDC> ObtenerEnviosConsolidadoXManifiesto(long idManifiesto)
        {
            return ONManejadorIngresoManifiesto.Instancia.ObtenerEnviosConsolidadoXManifiesto(idManifiesto);
        }

        /// <summary>
        /// Obtiene los envios sueltos de un manifiesto
        /// </summary>
        /// <param name="idManifiesto"></param>
        public List<ONEnviosDescargueRutaDC> ObtenerEnviosSueltosXManifiesto(long idManifiesto)
        {
            return ONManejadorIngresoManifiesto.Instancia.ObtenerEnviosSueltosXManifiesto(idManifiesto);
        }

        /// <summary>
        /// Obtiene los totales de manifiesto, total sobrantes, total faltantes
        /// </summary>
        /// <param name="idManifiesto"></param>
        /// <param name="idOperativo"></param>
        public ONCierreDescargueManifiestoDC ObtenerTotalCierreManifiesto(long idManifiesto, long idOperativo)
        {
            return ONManejadorIngresoManifiesto.Instancia.ObtenerTotalCierreManifiesto(idManifiesto, idOperativo);
        }

        #endregion Operativo por manifiesto

        #region Ingreso centro de acopio nacional

        /// <summary>
        /// Método para ingresar los consolidados en el ingreso a centro de acopio nacional
        /// </summary>
        /// <param name="controlTrans"></param>
        /// <param name="noPrecinto"></param>
        /// <param name="noConsolidado"></param>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public List<ONConsolidado> IngresarManifiestoConsolidado(long controlTrans, long noPrecinto, string noConsolidado, long idCentroServicio)
        {
            return ONIngresoCentroAcopio.Instancia.IngresarManifiestoConsolidado(controlTrans, noPrecinto, noConsolidado, idCentroServicio);
        }

        /// <summary>
        ///  Método para ingresar una guía suelta a centro de acopio nacional
        /// </summary>
        /// <param name="guia"></param>
        /// <returns></returns>
        public ONEnviosDescargueRutaDC IngresarGuiaSuelta(ONEnviosDescargueRutaDC guia, List<OUNovedadIngresoDC> listaNovedades)
        {
            return ONIngresoCentroAcopio.Instancia.IngresarGuiaSuelta(guia, listaNovedades);
        }

        #endregion Ingreso centro de acopio nacional

        #region Descargue Consolidados

        /// <summary>
        /// Obtiene los Consolidados de Guias para el descarge Urbano - Regional - Nacional
        /// </summary>
        /// <param name="ingresoConsolidado">info a consultar</param>
        /// <returns>info de las guias descargadas</returns>
        public ONDescargueConsolidadosUrbRegNalDC ObtenerIngresoConsolidado(ONDescargueConsolidadosUrbRegNalDC ingresoConsolidado)
        {
            return ONOperacionNacional.Instancia.ObtenerIngresoConsolidado(ingresoConsolidado);
        }

        /// <summary>
        /// Obtiene las novedades delos ingresos de la Guia
        /// </summary>
        /// <returns></returns>
        public List<ONNovedadesEnvioDC> ObtenerNovedadesIngresosGuia()
        {
            return ONOperacionNacional.Instancia.ObtenerNovedadesIngresosGuia();
        }

        /// <summary>
        ///Obtiene las novedades de Descargue Consolidado
        /// </summary>
        /// <returns></returns>
        public List<ONNovedadesConsolidadoDC> ObtenerNovedadesDescargueConsolidado()
        {
            return ONOperacionNacional.Instancia.ObtenerNovedadesDescargueConsolidado();
        }

        /// <summary>
        /// Obtiene la info inicial de la pantalla de Descargue Consolidados
        /// </summary>
        /// <returns>Listas de info Inicial pantalla</returns>
        public ONDescargueConsolidadosInfoInicialDC ObtenerInfoInicialDescargueConsolidados()
        {
            return ONOperacionNacional.Instancia.ObtenerInfoInicialDescargueConsolidados();
        }

        /// <summary>
        /// Adiciona el Proceso de Descargue Consolidado
        /// </summary>
        /// <param name="nuevoDescargue"></param>
        public bool AdicionarProcesoDescargueConsolidado(ONDescargueConsolidadosUrbRegNalDC nuevoDescargue)
        {
            return ONOperacionNacional.Instancia.AdicionarProcesoDescargueConsolidado(nuevoDescargue);
        }

        #endregion Descargue Consolidados


    }
}