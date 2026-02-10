using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading;
using CO.Servidor.OperacionNacional;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.OperacionNacional;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion;
using CO.Servidor.Servicios.ContratoDatos.Rutas;
using CO.Servidor.Servicios.Contratos;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using CO.Servidor.Integraciones.Satrack;
using CO.Servidor.Servicios.ContratoDatos.Integraciones.Satrack;

namespace CO.Servidor.Servicios.Implementacion.OperacionNacional
{
    /// <summary>
    /// Clase para los servicios de administración de Tarifas
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class ONOperacionNacionalSvc : IONOperacionNacionalSvc
    {
        #region Constructor

        public ONOperacionNacionalSvc()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(PAAdministrador.Instancia.ConsultarParametrosFramework("Cultura"));
        }

        #endregion Constructor

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
            return ONAdministradorOperacionNacional.Instancia.ObtenerManifiestosOperacionNacional(filtro, indicePagina, registrosPorPagina, idCentroServiciosManifiesta, incluyeFecha);
        }

        public IList<ONManifiestoOperacionNacional> ObtenerManifiestosOpeNal_ConNovRuta(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina, long idCentroServiciosManifiesta)
        {
            return ONOperacionNacional.Instancia.ObtenerManifiestosOpeNal_ConNovRuta(filtro, indicePagina, registrosPorPagina, idCentroServiciosManifiesta);
        }

        public IList<ONManifiestoOperacionNacional> ObtenerManifiestosOpeNal_Disponibles(long idCentroServicios, int idRuta, DateTime fechaInicial, DateTime fechaFinal)
        {
            return ONOperacionNacional.Instancia.ObtenerManifiestosOpeNal_Disponibles(idCentroServicios, idRuta, fechaInicial, fechaFinal);
        }

        public IList<ONNovedadEstacionRutaDC> Obtener_NovedadesEstacionRuta(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina, long idCentroServiciosManifiesta)
        {
            return ONOperacionNacional.Instancia.Obtener_NovedadesEstacionRuta(filtro, indicePagina, registrosPorPagina, idCentroServiciosManifiesta);
        }

        public ONManifiestoOperacionNacional Obtener_ManifiestoxId(long IdManifiesto)
        {
            return ONOperacionNacional.Instancia.Obtener_ManifiestoxId(IdManifiesto);
        }

        /// <summary>
        /// Obtiene todas las rutas filtradas a partir de una localidad de orgen
        /// </summary>
        /// <param name="IdLocalidad"></param>
        /// <returns></returns>
        public IList<RURutaDC> ObtenerRutasXLocalidadOrigen(string idLocalidad)
        {
            return ONAdministradorOperacionNacional.Instancia.ObtenerRutasXLocalidadOrigen(idLocalidad);
        }

        /// <summary>
        /// Obtiene las empresas transportadoras asociadas a una ruta
        /// </summary>
        /// <param name="idRuta">id de la ruta</param>
        /// <returns></returns>
        public IList<RUEmpresaTransportadora> ObtenerEmpresasTransportadorasXRuta(int idRuta)
        {
            return ONAdministradorOperacionNacional.Instancia.ObtenerEmpresasTransportadorasXRuta(idRuta);
        }

        /// <summary>
        /// Obtiene las empresas transportadoras asociadas a una racol
        /// </summary>
        /// <param name="idRuta">id de la ruta</param>
        /// <returns></returns>
        public IList<RUEmpresaTransportadora> ObtenerEmpresasTransportadorasXRacol(int idRacol)
        {
            return ONAdministradorOperacionNacional.Instancia.ObtenerEmpresasTransportadorasXRacol(idRacol);
        }


        /// <summary>
        /// Obtiene una lista de los vehiculos activos que pertenecen a un racol y que tienen ingreso a un col
        /// </summary>
        /// <param name="idRacol">Id del racol al que pertenece el vehiculo</param>
        /// <param name="idCentroServicios">Id del centro de servicios al que ingreso el vehiculo</param>
        /// <returns></returns>
        public IList<POVehiculo> ObtenerVehiculosIngresoCentroServicioXRacol(long idRacol, long idCentroServicios, bool esCol)
        {
            return ONAdministradorOperacionNacional.Instancia.ObtenerVehiculosIngresoCentroServicioXRacol(idRacol, idCentroServicios, esCol);
        }

        /// <summary>
        /// <summary>
        /// Obtiene una lista de los conductores activos y con pase vigente asignados a un vehiculo
        /// </summary>
        /// <param name="idVehiculo"></param>
        /// <returns></returns>
        public IList<POConductores> ObtenerConductoresActivosVehiculos(int idVehiculo)
        {
            return ONAdministradorOperacionNacional.Instancia.ObtenerConductoresActivosVehiculos(idVehiculo);
        }

        /// <summary>
        /// Inserta, actualiza un manifiesto nacional
        /// </summary>
        /// <param name="manifiesto"></param>
        public ONManifiestoOperacionNacional ActualizarManifiestoOperacionNacional(ONManifiestoOperacionNacional manifiesto)
        {
            return ONAdministradorOperacionNacional.Instancia.ActualizarManifiestoOperacionNacional(manifiesto);
        }

        /// <summary>
        /// Obtiene las estaciones y localidades adicionales de una ruta
        /// </summary>
        /// <param name="idRuta"></param>
        /// <returns>Lista con las estaciones de la ruta</returns>
        public IList<RUEstacionRuta> ObtenerEstacionesYLocalidadesAdicionalesRuta(int idRuta)
        {
            return ONAdministradorOperacionNacional.Instancia.ObtenerEstacionesYLocalidadesAdicionalesRuta(idRuta);
        }


        /// <summary>
        /// Obtiene las estaciones-Ruta de Manifiesto
        /// </summary>
        /// <param name="idManifiesto"></param>
        /// <returns>Lista con las estaciones de la ruta</returns>
        public IList<RUEstacionRuta> ObtenerEstacionesRutaDeManifiesto(long idManifiesto)
        {
            return ONAdministradorOperacionNacional.Instancia.ObtenerEstacionesRutaDeManifiesto(idManifiesto);
        }


        /// <summary>
        /// Obtiene los consolidados de un manifiesto de la operacion nacional
        /// </summary>
        /// <param name="idManifiestoOperacionNacional">Identificador del manifiesto de la operacion nacional</param>
        /// <returns></returns>
        public GenericoConsultasFramework<ONConsolidado> ObtenerConsolidadosManifiesto(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, long idManifiestoOperacionNacional, string idLocalidad)
        {
            int totalRegistros = 0;
            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<ONConsolidado>()
            {
                Lista = ONAdministradorOperacionNacional.Instancia.ObtenerConsolidadosManifiesto(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idManifiestoOperacionNacional, idLocalidad),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Obtiene todos los tipos de consolidado
        /// </summary>
        /// <returns></returns>
        public IList<ONTipoConsolidado> ObtenerTipoConsolidado()
        {
            return ONAdministradorOperacionNacional.Instancia.ObtenerTipoConsolidado();
        }

        // TODO:ID Obtener Lista Tipos de Novedad Estacion-Ruta
        public IList<ONTipoNovedadEstacionRutaDC> ObtenerTiposNovedadEstacionRuta()
        {
            return ONAdministradorOperacionNacional.Instancia.ObtenerTiposNovedadEstacionRuta();
        }


        /// <summary>
        /// Obtiene todos los detalles de los tipos de consolidado
        /// </summary>
        /// <returns></returns>
        public IList<ONTipoConsolidadoDetalle> ObtenerTipoConsolidadoDetalle(int idTipoConsolidado)
        {
            return ONAdministradorOperacionNacional.Instancia.ObtenerTipoConsolidadoDetalle(idTipoConsolidado);
        }

        /// <summary>
        /// Inserta un consolidado
        /// </summary>
        /// <param name="consolidado"></param>
        /// <returns></returns>
        public ONConsolidado AdicionarConsolidado(ONConsolidado consolidado, long idCentroServicios)
        {
            return ONAdministradorOperacionNacional.Instancia.AdicionarConsolidado(consolidado, idCentroServicios);
        }



        /// Inserta una Novedad Estacion-Ruta
        /// </summary>
        /// <param name="lista de Novedades"></param>
        /// <returns></returns>
        public void AdicionarNovedadEstacionRuta(List<ONNovedadEstacionRutaDC> lstNovedadesEstacioRuta)
        {
            ONOperacionNacional.Instancia.AdicionarNovedadEstacionRuta(lstNovedadesEstacioRuta);
        }


        /// <summary>
        /// Modifica un consolidado
        /// </summary>
        /// <param name="consolidado"></param>
        /// <returns></returns>
        public void EditarConsolidado(ONConsolidado consolidado, long idCentroServicios)
        {
            ONAdministradorOperacionNacional.Instancia.EditarConsolidado(consolidado, idCentroServicios);
        }

        /// <summary>
        /// Inserta un envio al consolidado
        /// </summary>
        /// <param name="consolidadoDetalle"></param>
        /// <returns></returns>
        public ONConsolidadoDetalle AdicionarConsolidadoDetalle(ONConsolidadoDetalle consolidadoDetalle)
        {
            return ONAdministradorOperacionNacional.Instancia.AdicionarConsolidadoDetalle(consolidadoDetalle);
        }

        /// <summary>
        /// Obtiene las guias de un consolidado
        /// </summary>
        /// <param name="idConsolidado"></param>
        /// <returns></returns>
        public IList<ONConsolidadoDetalle> ObtenerGuiasConsolidado(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina, long idConsolidado)
        {
            return ONAdministradorOperacionNacional.Instancia.ObtenerGuiasConsolidado(filtro, indicePagina, registrosPorPagina, idConsolidado);
        }

        /// <summary>
        /// Obtiene todas las guias manifestadas, incluye guias sueltas y guias consolidadas
        /// </summary>
        /// <returns></returns>
        public IList<ONManifiestoGuia> ObtenerTodasGuiasManifiesto(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long idManifiestoOperacionNacional)
        {
            return ONAdministradorOperacionNacional.Instancia.ObtenerTodasGuiasManifiesto(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idManifiestoOperacionNacional);
        }

        /// <summary>
        /// Inserta una guia suelta a un manifiesto
        /// </summary>
        /// <param name="guia"></param>
        public ONManifiestoGuia AdicionarGuiaSuelta(ONManifiestoGuia guia)
        {
            return ONAdministradorOperacionNacional.Instancia.AdicionarGuiaSuelta(guia);
        }

        /// Obtiene los consolidados y las guias sueltas para la impresion del manifiesto por ruta
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <param name="idCentroServicios"></param>
        /// <returns></returns>
        public List<ONImpresionManifiestoDC> ObtenerImpresionManifiestoRuta(long idManifiesto, List<string> ciudadesAManifestar)
        {
            return ONAdministradorOperacionNacional.Instancia.ObtenerImpresionManifiestoRuta(idManifiesto, ciudadesAManifestar);
        }

        /// <summary>
        /// Obtiene todas las guias sueltas de un manifiesto nacional
        /// </summary>
        /// <param name="idManifiestoOpeNacional"></param>
        /// <returns></returns>
        public IList<ONManifiestoGuia> ObtenerGuiasSueltas(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina, long idManifiestoOpeNacional)
        {
            return ONAdministradorOperacionNacional.Instancia.ObtenerGuiasSueltas(filtro, indicePagina, registrosPorPagina, idManifiestoOpeNacional);
        }

        /// <summary>
        /// Obtiene todos los motivos de eliminacion de una guia
        /// </summary>
        /// <returns></returns>
        public IList<ONTipoMotivoElimGuiaMani> ObtenerTodosMotivosEliminacionGuia()
        {
            return ONAdministradorOperacionNacional.Instancia.ObtenerTodosMotivosEliminacionGuia();
        }

        /// <summary>
        /// Elimina una guia suelta de un manifiesto o una guia de un consolidado de un manifiesto
        /// </summary>
        /// <param name="motivo"></param>
        public void EliminarGuiaManifiesto(ONMotivoElimGuiaMani motivo)
        {
            ONAdministradorOperacionNacional.Instancia.EliminarGuiaManifiesto(motivo);
        }

        /// <summary>
        /// Cierra un manifiesto
        /// </summary>
        /// <param name="idManifiesto"></param>
        public void CerrarManifiesto(long idManifiesto, DateTime fechaSalida)
        {
            ONAdministradorOperacionNacional.Instancia.CerrarManifiesto(idManifiesto, fechaSalida);
        }

        public string ReportarSatrack(INDatosEnvioSatrack infoSatrack)
        {
            //EMRL Si llega con valor se debe enviar a satrack
            return ValidarSatrack(infoSatrack);
        }

        /// <summary>
        /// Valida si se tiene informacion para enviar a satrack y se envia al servicio de satrack
        /// </summary>
        /// <param name="infoSatrack"></param>
        private static string ValidarSatrack(INDatosEnvioSatrack infoSatrack)
        {
            if (infoSatrack != null)
            {
                List<INItinerarioDC> programaciones = new List<INItinerarioDC>();
                INItinerarioDC programacion = new INItinerarioDC()
                {
                    Placa = infoSatrack.Placa,
                    Ruta = infoSatrack.Ruta,
                    Parametros = new INParametrosDC
                    {
                        Fechadespacho = infoSatrack.FechaSalida.Value.ToString("yyyy-MM-ddTHH:mm:ss")
                    }
                };
                programaciones.Add(programacion);
                return IntegracionSatrack.Instancia.ProgramarItinerario(programaciones);
            }
            return
                string.Empty;
        }

        

        /// <summary>
        /// Reabre un manifiesto
        /// </summary>
        /// <param name="idManifiesto"></param>
        public void AbrirManifiesto(ONManifiestoOperacionNacional manifiesto)
        {
            ONAdministradorOperacionNacional.Instancia.AbrirManifiesto(manifiesto);
        }

        /// <summary>
        /// Guarda el envio ingresado
        /// </summary>
        /// <param name="ingreso"></param>
        public ONIngresoOperativoDC GuardaIngresoEnvioAgencia(ONIngresoOperativoDC ingreso)
        {
            return ONAdministradorOperacionNacional.Instancia.GuardaIngresoEnvioAgencia(ingreso);
        }

        /// <summary>
        /// Retorna los envios del consolidado seleccionado
        /// </summary>
        /// <param name="idConsolidado"></param>
        /// <returns></returns>
        public List<ONConsolidadoDetalle> ObtenerEnviosConsolidado(long idConsolidado)
        {
            return ONAdministradorOperacionNacional.Instancia.ObtenerEnviosConsolidado(idConsolidado);
        }

        /// <summary>
        /// Genera el reporte del manifiesto de carga
        /// </summary>
        /// <param name="idManifiesto"></param>
        public ONReporteManifiestoCarga GenerarReporteManifiestoCarga(long idManifiesto, long IdCentroServiciosManifiesta)
        {
            return ONAdministradorOperacionNacional.Instancia.GenerarReporteManifiestoCarga(idManifiesto, IdCentroServiciosManifiesta);
        }

        /// <summary>
        /// Obtiene las rutas a las cuales pertenece una estación
        /// </summary>
        /// <param name="idLocalidadEstacion"></param>
        /// <returns></returns>
        public List<RURutaDC> ObtenerRutasPerteneceEstacion(string idLocalidadEstacion)
        {
            return ONAdministradorOperacionNacional.Instancia.ObtenerRutasPerteneceEstacion(idLocalidadEstacion);
        }

        /// <summary>
        /// Obtiene las rutas a las cuales pertenece una estacion, incluye las rutas en las que la estacion es origen y destino
        /// </summary>
        /// <param name="idLocalidadEstacion"></param>
        /// <returns></returns>
        public List<RURutaDC> ObtenerRutasPerteneceEstacionOrigDest(string idLocalidadEstacion)
        {
            return ONAdministradorOperacionNacional.Instancia.ObtenerRutasPerteneceEstacionOrigDest(idLocalidadEstacion);
        }

        /// <summary>
        /// obtiene una lista de guias en el centro de acopio, que debieron haber sido agregadas al manifiesto
        /// </summary>
        /// <param name="manifiesto"></param>
        /// <returns></returns>
        public List<ADGuia> ObtenerEnviosPendientesPorManifestar(ONManifiestoOperacionNacional manifiesto)
        {
            return ONAdministradorOperacionNacional.Instancia.ObtenerEnviosPendientesPorManifestar(manifiesto);
        }
        /// <summary>
        /// para obtener parametros guias en novedades de ruta cuando se requiera asignar novedad a guias especificas
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="guiaxNovIndv"></param>
        /// <returns></returns>
        public List<ONParametrosGuiaInvNovedadRuta> ObtenerParametrosGuiaIndvPorNovedadRuta(long numeroGuia, ONEnumGuiaIndvNovedadRuta guiaxNovIndv, string tipoUbicacion)
        {
            return ONAdministradorOperacionNacional.Instancia.ObtenerParametrosGuiaIndvPorNovedadRuta(numeroGuia, guiaxNovIndv, tipoUbicacion);
        }
        /// <summary>
        /// Obtiene las novedades de transporte de determinada guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<ONNovedadesTransporteDC> ObtenerNovedadesTransporteGuia(long numeroGuia)
        {
            return ONAdministradorOperacionNacional.Instancia.ObtenerNovedadesTransporteGuia(numeroGuia);
        }

        // Se revisa si el Casillero es AEREO (Ciudades de la Costa en la Tabla TrayectoCasilleroServicio_OPN)
        public bool ValidarTrayectoServicioAereo(string idLocalidadOrigen, string idLocalidadDestino, int idServicio)
        {
            return ONAdministradorOperacionNacional.Instancia.ValidarTrayectoServicioAereo(idLocalidadOrigen, idLocalidadDestino, idServicio);
        }
        #region Ingreso Col Ruta

        /// <summary>
        /// Valida el vehiculo seleccionado para poder hacer el ingreso a col
        /// </summary>
        public bool ValidacionVehiculoIngreso(ONIngresoOperativoDC ingreso)
        {
            return ONAdministradorOperacionNacional.Instancia.ValidacionVehiculoIngreso(ingreso);
        }

        /// <summary>
        /// Obtiene los envios consolidados de los manifiestos asociados al vehiculo
        /// </summary>
        /// <param name="idVehiculo"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        public List<ONManifiestoOperacionNacional> ObtenerEnviosConsolidadosManifiestoVehiculo(int idVehiculo, int idRuta, int indicePagina, int registrosPorPagina)
        {
            return ONAdministradorOperacionNacional.Instancia.ObtenerEnviosConsolidadosManifiestoVehiculo(idVehiculo, idRuta, indicePagina, registrosPorPagina);
        }

        /// <summary>
        /// Obtiene los consolidados de los manifiestos abiertos asociados al vehiculo
        /// </summary>
        /// <param name="idVehiculo">id del vehiculo</param>
        /// <returns></returns>
        public List<ONConsolidado> ObtenerConsolidadosManifiestoVehiculo(int idVehiculo, int idRuta, int indicePagina, int registrosPorPagina)
        {
            return ONAdministradorOperacionNacional.Instancia.ObtenerConsolidadosManifiestoVehiculo(idVehiculo, idRuta, indicePagina, registrosPorPagina);
        }

        /// <summary>
        /// Obtiene los estados del empaque para el parametro de peso especificado
        /// </summary>
        /// <param name="idParametro"></param>
        /// <returns></returns>
        public List<PAEstadoEmpaqueDC> ObtenerEstadosEmpaqueParametroPeso(string idParametro)
        {
            return ONAdministradorOperacionNacional.Instancia.ObtenerEstadosEmpaqueParametroPeso(idParametro);
        }

        /// <summary>
        /// Guarda el ingreso a la agencia del vehiculo del operativo
        /// </summary>
        /// <param name="ingresoOperativo"></param>
        /// <returns></returns>
        public long GuardarIngresoAgenciaRuta(ONIngresoOperativoDC ingresoOperativo)
        {
            return ONAdministradorOperacionNacional.Instancia.GuardarIngresoAgenciaRuta(ingresoOperativo);
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
            ONAdministradorOperacionNacional.Instancia.GuardarNovedadConsolidado(consolidado, descripcion);
        }

        /// <summary>
        /// Obtiene los manifiestos sin descargar de un vehiculo
        /// </summary>
        /// <param name="idVehiculo"></param>
        /// <returns></returns>
        public List<ONManifiestoOperacionNacional> ObtenerManifiestosVehiculo(int idVehiculo)
        {
            return ONAdministradorOperacionNacional.Instancia.ObtenerManifiestosVehiculo(idVehiculo);
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
            return ONAdministradorOperacionNacional.Instancia.ObtenerEnviosSueltosManifiestoVehiculo(idVehiculo, idRuta, idIngresoOperativo, pageIndex, pageSize);
        }

        /// <summary>
        /// Obtiene el total de los envios manifestados
        /// </summary>
        /// <param name="idVehiculo"></param>
        /// <param name="idLocalidadDestino"></param>
        /// <returns></returns>
        public int ObtenerTotalEnviosManifestadosVehiculoLocalidad(int idVehiculo, string idLocalidadDestino)
        {
            return ONAdministradorOperacionNacional.Instancia.ObtenerTotalEnviosManifestadosVehiculoLocalidad(idVehiculo, idLocalidadDestino);
        }

        /// <summary>
        /// Obtiene el total de los envios sobrantes
        /// </summary>
        /// <param name="idVehiculo"></param>
        /// <param name="idLocalidadDestino"></param>
        /// <returns></returns>
        public int ObtenerTotalEnviosSobrantesVehiculoLocalidad(long idOperativo, string idLocalidadDestino)
        {
            return ONAdministradorOperacionNacional.Instancia.ObtenerTotalEnviosSobrantesVehiculoLocalidad(idOperativo, idLocalidadDestino);
        }

        /// <summary>
        /// Obtiene el total de los envios descargados
        /// </summary>
        /// <param name="idVehiculo"></param>
        /// <param name="idLocalidadDestino"></param>
        /// <returns></returns>
        public int ObtenerTotalEnviosDescargadosVehiculoLocalidad(long idOperativo, string idLocalidadDestino)
        {
            return ONAdministradorOperacionNacional.Instancia.ObtenerTotalEnviosDescargadosVehiculoLocalidad(idOperativo, idLocalidadDestino);
        }

        /// <summary>
        /// Guarda y valida los envios transito
        /// </summary>
        /// <param name="idGuiaInterna"></param>
        /// <param name="ingreso"></param>
        public void GuardaIngresoEnviosTransito(ONIngresoOperativoDC ingreso)
        {
            ONAdministradorOperacionNacional.Instancia.GuardaIngresoEnviosTransito(ingreso);
        }

        /// <summary>
        /// Cierra el ingreso del operativo para el vehiculo
        /// </summary>
        public void CerrarIngresoOperativoAgencia(ONIngresoOperativoDC ingreso)
        {
            ONAdministradorOperacionNacional.Instancia.CerrarIngresoOperativoAgencia(ingreso);
        }

        /// <summary>
        /// Consulta todos los envios de un consolidado
        /// </summary>
        /// <param name="idVehiculo"></param>
        /// <param name="idIngresoOperativo"></param>
        /// <returns></returns>
        public List<ONConsolidadoDetalle> ObtenerTodosEnviosSueltosVehiculo(int idVehiculo, int idRuta)
        {
            return ONAdministradorOperacionNacional.Instancia.ObtenerTodosEnviosSueltosVehiculo(idVehiculo, idRuta);
        }

        /// <summary>
        /// Consulta todos los envios consolidados del manifiesto
        /// </summary>
        /// <param name="idVehiculo"></param>
        /// <returns></returns>
        public List<ONConsolidadoDetalle> ObtenerTodosEnviosConsolidadoManifiesto(int idVehiculo, int idRuta)
        {
            return ONAdministradorOperacionNacional.Instancia.ObtenerTodosEnviosConsolidadoManifiesto(idVehiculo, idRuta);
        }

        #endregion Ingreso Col Ruta

        #region Ingreso Salida Transportador

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
            return ONAdministradorOperacionNacional.Instancia.ObtenerIngresoSalidaTransportador(filtro, indicePagina, registrosPorPagina, incluyeFecha);
        }

        /// <summary>
        /// Obtiene las rutas de una localidad
        /// </summary>
        /// <param name="idLocalidad"></param>
        public List<RURutaDC> ObtenerRutasPorLocalidad(string idLocalidad)
        {
            return ONAdministradorOperacionNacional.Instancia.ObtenerRutasPorLocalidad(idLocalidad);
        }

        /// <summary>
        /// Obtiene todos los conductores Activos
        /// </summary>
        public IList<POConductores> ObtenerTodosConductores()
        {
            return ONAdministradorOperacionNacional.Instancia.ObtenerTodosConductores();
        }

        /// <summary>
        /// Obtiene la informacion de ingreso y salida del transportador por ID
        /// </summary>
        /// <param name="idIngrsoSalidaTransportado"></param>
        /// <returns></returns>
        public ONIngresoSalidaTransportadorDC ObtenerIngresoSalidaTransportadorPorId(long idIngrsoSalidaTransportado)
        {
            return ONAdministradorOperacionNacional.Instancia.ObtenerIngresoSalidaTransportadorPorId(idIngrsoSalidaTransportado);
        }

        /// <summary>
        /// Obtiene todas las novedades operativas
        /// </summary>
        public IList<ONNovedadOperativoDC> ObtenerNovedadOperativo()
        {
            return ONAdministradorOperacionNacional.Instancia.ObtenerNovedadOperativo();
        }

        /// <summary>
        /// Obtiene las estaciones y localidades adicionales de una ruta sin importar si son conolidado o no
        /// </summary>
        /// <param name="idRuta"></param>
        /// <returns>Lista con las estaciones de la ruta</returns>
        public IList<RUEstacionRuta> ObtenerEstacionesRuta(int idRuta)
        {
            return ONAdministradorOperacionNacional.Instancia.ObtenerEstacionesRuta(idRuta);
        }

        /// <summary>
        /// Ingreso en la tabla de ingreso salida transportador
        /// </summary>
        /// <param name="ingresoSalidaTrans"></param>
        public void IngresarIngresosSalidasTrasnportador(ONIngresoSalidaTransportadorDC ingresoSalidaTrans)
        {
            ONAdministradorOperacionNacional.Instancia.IngresarIngresosSalidasTrasnportador(ingresoSalidaTrans);
        }

        /// <summary>
        /// Consultar la última ruta de un vehículo que ha sido manifestado,
        /// el vehículo se consulta por su placa
        /// </summary>
        public ONRutaConductorDC ObtenerUltimaRutaConduPlaca(string placa)
        {
            return ONAdministradorOperacionNacional.Instancia.ObtenerUltimaRutaConduPlaca(placa);
        }

        #endregion Ingreso Salida Transportador

        #region Descarga Operativo por ciudad

        /// <summary>
        /// Metodo para Obtener las
        /// Regionales Administrativas
        /// </summary>
        /// <returns>lista de RACOL</returns>
        public List<PURegionalAdministrativa> ObtenerRegionalAdministrativa()
        {
            return ONAdministradorOperacionNacional.Instancia.ObtenerRegionalAdministrativa();
        }

        /// <summary>
        /// Metodo para Obtener los Centros de
        /// Servicios y Agencias por RACOL
        /// </summary>
        /// <param name="idRacol"> es el id del RACOL</param>
        /// <returns>lista de centros de servicio</returns>
        public List<PUCentroServiciosDC> ObtenerCentrosServicios(long idRacol)
        {
            return ONAdministradorOperacionNacional.Instancia.ObtenerCentrosServicios(idRacol);
        }

        /// <summary>
        /// Obtener los consolidados a partir de la guia interna
        /// </summary>
        /// <param name="idGuiaInterna"></param>
        public ONConsolidado ObtenerConsolidadoPorIdGuia(long idGuiaInterna)
        {
            return ONAdministradorOperacionNacional.Instancia.ObtenerConsolidadoPorIdGuia(idGuiaInterna);
        }

        #endregion Descarga Operativo por ciudad

        #region Operativo Ciudad

        /// <summary>
        /// Guarda el envio ingresado
        /// </summary>
        /// <param name="ingreso"></param>
        public ONIngresoOperativoCiudadDC GuardaIngresoEnvioOperativoCiudad(ONIngresoOperativoCiudadDC ingreso)
        {
            return ONAdministradorOperacionNacional.Instancia.GuardaIngresoEnvioOperativoCiudad(ingreso);
        }

        /// <summary>
        /// Guarda  el ingreso al operativo por ciudad
        /// </summary>
        /// <param name="ingreso"></param>
        /// <returns></returns>
        public long GuardarOperativoAgenciaCiudad(ONIngresoOperativoCiudadDC ingreso)
        {
            return ONAdministradorOperacionNacional.Instancia.GuardarOperativoAgenciaCiudad(ingreso);
        }

        /// <summary>
        /// Cierra el ingreso del operativo para la ciudad
        /// </summary>
        public void TerminaDescargueOperativo(long idIngresoOperativo)
        {
            ONAdministradorOperacionNacional.Instancia.TerminaDescargueOperativo(idIngresoOperativo);
        }

        #endregion Operativo Ciudad

        #region Operativo Manifiesto

        /// <summary>
        /// Obtiene los manifietos donde la ciudad origen, destino o estacion de ruta sea la ciudad
        /// donde se esta haciendo el descargue
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <returns></returns>
        public List<ONManifiestoOperacionNacional> ObtenerManifiestosXLocalidad(IDictionary<string, string> filtro, string idLocalidad, int indicePagina, int registrosPorPagina)
        {
            return ONAdministradorOperacionNacional.Instancia.ObtenerManifiestosXLocalidad(filtro, idLocalidad, indicePagina, registrosPorPagina);
        }

        /// <summary>
        /// Consulta el detalle del manifiesto
        /// </summary>
        /// <param name="idManifiesto"></param>
        /// <returns></returns>
        public ONIngresoOperativoDC ObtenerDetalleManifiesto(ONIngresoOperativoDC ingreso)
        {
            return ONAdministradorOperacionNacional.Instancia.ObtenerDetalleManifiesto(ingreso);
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
            return ONAdministradorOperacionNacional.Instancia.ObtenerEnviosXManifiesto(idManifiesto, indicePagina, registrosPorPagina);
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
            return ONAdministradorOperacionNacional.Instancia.ObtenerDetalleConsolidadosManifiesto(idManifiesto, indicePagina, registrosPorPagina);
        }

        /// <summary>
        /// Consulta las estaciones de una ruta y la cantidad de envíos manifestados en un manifiesto específico x cada estación
        /// </summary>
        /// <param name="idRuta"></param>
        /// <param name="idManifiesto"></param>
        /// <returns></returns>
        public List<ONCantEnviosManXEstacionDC> ConsultarCantEnviosManXEstacion(int idRuta, long idManifiesto)
        {
            return ONAdministradorOperacionNacional.Instancia.ConsultarCantEnviosManXEstacion(idRuta, idManifiesto);
        }

        /// <summary>
        /// Obtiene los envios de consolidados de un manifiesto
        /// </summary>
        /// <param name="idManifiesto"></param>
        public List<ONEnviosDescargueRutaDC> ObtenerEnviosConsolidadoXManifiesto(long idManifiesto)
        {
            return ONAdministradorOperacionNacional.Instancia.ObtenerEnviosConsolidadoXManifiesto(idManifiesto);
        }

        /// <summary>
        /// Obtiene los envios sueltos de un manifiesto
        /// </summary>
        /// <param name="idManifiesto"></param>
        public List<ONEnviosDescargueRutaDC> ObtenerEnviosSueltosXManifiesto(long idManifiesto)
        {
            return ONAdministradorOperacionNacional.Instancia.ObtenerEnviosSueltosXManifiesto(idManifiesto);
        }

        /// <summary>
        /// Obtiene los totales de manifiesto, total sobrantes, total faltantes
        /// </summary>
        /// <param name="idManifiesto"></param>
        /// <param name="idOperativo"></param>
        public ONCierreDescargueManifiestoDC ObtenerTotalCierreManifiesto(long idManifiesto, long idOperativo)
        {
            return ONAdministradorOperacionNacional.Instancia.ObtenerTotalCierreManifiesto(idManifiesto, idOperativo);
        }

        #endregion Operativo Manifiesto

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
            return ONAdministradorOperacionNacional.Instancia.IngresarManifiestoConsolidado(controlTrans, noPrecinto, noConsolidado, idCentroServicio);
        }

        /// <summary>
        ///  Método para ingresar una guía suelta a centro de acopio nacional
        /// </summary>
        /// <param name="guia"></param>
        /// <returns></returns>
        public ONEnviosDescargueRutaDC IngresarGuiaSuelta(ONEnviosDescargueRutaDC guia, List<OUNovedadIngresoDC> listaNovedades)
        {
            return ONAdministradorOperacionNacional.Instancia.IngresarGuiaSuelta(guia, listaNovedades);
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
            return ONAdministradorOperacionNacional.Instancia.ObtenerIngresoConsolidado(ingresoConsolidado);
        }

        /// <summary>
        /// Obtiene las novedades delos ingresos de la Guia
        /// </summary>
        /// <returns></returns>
        public List<ONNovedadesEnvioDC> ObtenerNovedadesIngresosGuia()
        {
            return ONAdministradorOperacionNacional.Instancia.ObtenerNovedadesIngresosGuia();
        }

        /// <summary>
        ///Obtiene las novedades de Descargue Consolidado
        /// </summary>
        /// <returns></returns>
        public List<ONNovedadesConsolidadoDC> ObtenerNovedadesDescargueConsolidado()
        {
            return ONAdministradorOperacionNacional.Instancia.ObtenerNovedadesDescargueConsolidado();
        }

        /// <summary>
        /// Obtiene la info inicial de la pantalla de Descargue Consolidados
        /// </summary>
        /// <returns>Listas de info Inicial pantalla</returns>
        public ONDescargueConsolidadosInfoInicialDC ObtenerInfoInicialDescargueConsolidados()
        {
            return ONAdministradorOperacionNacional.Instancia.ObtenerInfoInicialDescargueConsolidados();
        }

        /// <summary>
        /// Adiciona el Proceso de Descargue Consolidado
        /// </summary>
        /// <param name="nuevoDescargue"></param>
        public bool AdicionarProcesoDescargueConsolidado(ONDescargueConsolidadosUrbRegNalDC nuevoDescargue)
        {
            return ONAdministradorOperacionNacional.Instancia.AdicionarProcesoDescargueConsolidado(nuevoDescargue);
        }

        #endregion Descargue Consolidados

        #region TrayectoCasillero

        /// <summary>
        /// Método para obtener los rangos de peso de los casilleros
        /// </summary>
        /// <returns></returns>
        public IList<ONRangoPesoCasilleroDC> ObtenerRangosPesoCasillero()
        {
            return ONOperacionNacional.Instancia.ObtenerRangosPesoCasillero();
        }


        /// <summary>
        /// Método para obtener los trayectos de un origen 
        /// </summary>
        /// <returns></returns>
        public IList<ONTrayectoCasilleroPesoDC> ObtenerTrayectosCasilleroDestino(PALocalidadDC localidadOrigen)
        {
            return ONOperacionNacional.Instancia.ObtenerTrayectosCasilleroDestino(localidadOrigen);
        }

        public void AdicionarNovedadEstacionRutaPorGuia(List<ONNovedadEstacionRutaxGuiaDC> lstNovedadesEstacioRuta)
        {
            ONOperacionNacional.Instancia.AdicionarNovedadEstacionRutaPorGuia(lstNovedadesEstacioRuta);
        }



        /// <summary>
        /// Método para guardar los cambios de  los trayectos casillero
        /// </summary>
        /// <returns></returns>
        public IList<ONTrayectoCasilleroPesoDC> GuardarTrayectoCasillero(List<ONTrayectoCasilleroPesoDC> ListaTrayectosCasillero)
        {
            return ONOperacionNacional.Instancia.GuardarTrayectoCasillero(ListaTrayectosCasillero);
        }


        #endregion

        #region PruebaCarga


        /// <summary>
        /// Prueba de carga para el grafo de rutas
        /// </summary>
        /// <param name="IdCiudadDestino"></param>
        /// <param name="nombreCiudadDestino"></param>
        /// <param name="idCiudadManifiesta"></param>
        /// <param name="nombreciudadManifiesta"></param>
        /// <param name="idRuta"></param>
        /// <returns></returns>
        public Dictionary<string, string> CalcularRutaOptimaOmitiendoCiudadesPruebaCargaGrafo(string IdCiudadDestino, string nombreCiudadDestino, string idCiudadManifiesta, string nombreciudadManifiesta, int idRuta)
        {
            return ONOperacionNacional.Instancia.CalcularRutaOptimaOmitiendoCiudadesPruebaCargaGrafo(IdCiudadDestino, nombreCiudadDestino, idCiudadManifiesta, nombreciudadManifiesta, idRuta);
        }


        #endregion

        /// <summary>
        /// Consultar los horarios de una ruta
        /// </summary>
        /// <param name="idRuta"></param>
        /// <returns>Horarios de los tres dias siguientes al actual y horarios del dia anterior que no superen 8 horas</returns>
        public List<ONHorarioRutaDC> ConsultarHorariosRuta(int idRuta)
        {
            return ONOperacionNacional.Instancia.ConsultarHorariosRuta(idRuta);
        }

    }
}