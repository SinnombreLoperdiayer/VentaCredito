using System;
using System.Collections.Generic;
using CO.Servidor.GestionGiro.ConfiguracionGiros;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros.Pagos;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Solicitudes;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using CO.Servidor.Solicitudes.Giros.Motivos;
using CO.Servidor.Solicitudes.Giros.Solicitudes;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using Framework.Servidor.Servicios.ContratoDatos.Parametros.Consecutivos;
using Framework.Servidor.Servicios.ContratoDatos.Seguridad;

namespace CO.Servidor.Solicitudes.Giros
{
    /// <summary>
    /// Esta es la Fachada de Solicitudes
    /// </summary>
    public class GIAdministradorSolicitudes
    {
        private static GIAdministradorSolicitudes instancia = new GIAdministradorSolicitudes();

        /// <summary>
        /// Instancia de acceso
        /// </summary>
        public static GIAdministradorSolicitudes Instancia
        {
            get { return GIAdministradorSolicitudes.instancia; }
        }

        /// <summary>
        /// Obtiene los Motivos de una solicitud
        /// </summary>
        /// <param name="filtro">Campo por que flitraria.</param>
        /// <param name="campoOrdenamiento">Campo de ordenamiento.</param>
        /// <param name="indicePagina">Campo de indice pagina.</param>
        /// <param name="registrosPorPagina">Campo de registros por pagina.</param>
        /// <param name="ordenamientoAscendente">si es verdadero  ordena ascendentemente <c>true</c> [ordenamiento ascendente].</param>
        /// <param name="totalRegistros">Campo de total registros.</param>
        /// <returns>lista de los motivos</returns>
        public List<GIMotivoSolicitudDC> ObtenerMotivosSol(IDictionary<string, string> filtro, string campoOrdenamiento,
                                                            int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return GIMotivo.Instancia.ObtenerMotivosSolicitud(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Adiciona - Modifica - Elimina el Motivo de la Solicitud Rafael Ramirez 05-01-2011
        /// </summary>
        /// <param name="motivoSolicitud">propiedad de motivo a modificar</param>
        public void ActuzalizarMotivoSolicitudes(GIMotivoSolicitudDC motivoSolicitud)
        {
            GIMotivo.Instancia.ActuzalizarMotivoSolicitudes(motivoSolicitud);
        }

        /// <summary>
        /// Obtiene los Tipos Solicitudes y Motivos
        /// </summary>
        /// <returns>lista de tipos de solicitud</returns>
        public List<GITipoSolicitudDC> ObtenerTiposSolicitudes()
        {
            return GIMotivo.Instancia.ObtenerTiposSolicitud();
        }

        /// <summary>
        /// Obtiene las Solicitudes por Agencia y Giro
        /// </summary>
        /// <param name="idGiro">valor del idgiro</param>
        /// <param name="dgVerif">digito verificacion para la valiodacion</param>
        /// <returns>valore sdel giro</returns>
        public GISolicitudGiroDC ObtenerGiro(long idGiro, string digVerif)
        {
            return GISolicitud.Instancia.ObtenerGiro(idGiro, digVerif);
        }

        /// <summary>
        /// Obtene las solicitudes por agencia.
        /// </summary>
        /// <param name="filtro">Campo por que flitraria.</param>
        /// <param name="campoOrdenamiento">Campo de ordenamiento.</param>
        /// <param name="indicePagina">Campo de indice pagina.</param>
        /// <param name="registrosPorPagina">Campo de registros por pagina.</param>
        /// <param name="ordenamientoAscendente">si es verdadero  ordena ascendentemente <c>true</c> [ordenamiento ascendente].</param>
        /// <param name="totalRegistros">Campo de total registros.</param>
        /// <param name="idRacol">Campo de id racol para filtrar.</param>
        /// <param name="idAgencia">Campo de id agencia para filtrar.</param>
        /// <returns>Lista de Solicitudes por agencia</returns>
        public List<GISolicitudGiroDC> ObtenerSolicitudesPorAgencia(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina,
                                                                          bool ordenamientoAscendente, out int totalRegistros, string idRacol, string idAgencia)
        {
            return GISolicitud.Instancia.ObtenerSolicitudesPorAgencia(filtro, campoOrdenamiento, indicePagina, registrosPorPagina,
                                                                                      ordenamientoAscendente, out totalRegistros, idRacol, idAgencia);
        }

        /// <summary>
        /// Obtiene las solicitudes activas.
        /// </summary>
        /// <param name="filtro">Campo por que filtraria.</param>
        /// <param name="campoOrdenamiento">Campo de ordenamiento.</param>
        /// <param name="indicePagina">Campo de indice pagina.</param>
        /// <param name="registrosPorPagina">Campo de registros por pagina.</param>
        /// <param name="ordenamientoAscendente">si es verdadero  ordena ascendentemente <c>true</c> [ordenamiento ascendente].</param>
        /// <param name="totalRegistros">Campo de total registros.</param>
        /// <returns>LIsta de solicitudes Activas</returns>
        public List<GISolicitudGiroDC> ObtenerSolicitudesActivas(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina,
                                                                         bool ordenamientoAscendente, out int totalRegistros, long idRegional)
        {
            return GISolicitud.Instancia.ObtenerSolicitudesActivas(filtro, campoOrdenamiento, indicePagina, registrosPorPagina,
                                                                                        ordenamientoAscendente, out totalRegistros, idRegional);
        }

        /// <summary>
        /// Obtiene los Tipos de Solicitudes
        /// </summary>
        /// <returns>lista de los tipos de solicitud</returns>
        public List<GITipoSolicitudDC> ObtenerTiposSolicitud()
        {
            return GIMotivo.Instancia.ObtenerTiposSolicitud();
        }

        /// <summary>
        /// Metodo para Obtener el detalle de la
        /// Solicitud
        /// </summary>
        /// <param name="idSolicitud"></param>
        /// <returns>una solicitud</returns>
        public GISolicitudGiroDC ObtenerDetalleSolicitud(long idSolicitud)
        {
            return GISolicitud.Instancia.ObtenerDetalleSolicitud(idSolicitud);
        }

        /// <summary>
        /// Obtiene el detalle de una solicitud.
        /// </summary>
        /// <param name="idSolicitud">id de la solicitud a consultar.</param>
        /// <returns>el detalle de una solicitud</returns>
        public GISolicitudGiroDC ObtenerDetalleSolicitudAprobada(long idSolicitud, long idRacol)
        {
            return GISolicitud.Instancia.ObtenerDetalleSolicitudAprobada(idSolicitud, idRacol);
        }

        /// <summary>
        /// Metodo que Adiciona una nueva Solicitud
        /// dependiendo del tipo de Motivo que tenga
        /// </summary>
        /// <param name="nvaSol">propiedad de la solicitud a agregar</param>
        public GISolicitudGiroDC AdicionarNvaSolicitud(GISolicitudGiroDC nvaSol)
        {
            return GISolicitud.Instancia.AdicionarNvaSolicitud(nvaSol);
        }

        /// <summary>
        /// Metodo poara obtener el archivo
        /// Adjunto
        /// </summary>
        /// <param name="idArchivo"></param>
        /// <returns>el estado de la solicitud</returns>
        public string ObtenerArchivoAdjunto(long idArchivo)
        {
            return GISolicitud.Instancia.ObtenerArchivoAdjunto(idArchivo);
        }

        /// <summary>
        /// Obtien las solicitudes anteriores.
        /// </summary>
        /// <param name="idAdmisionGiro">Valor del idAdmisionGiro.</param>
        /// <returns>una lista de las solicitudes anteriores</returns>
        public List<GISolicitudGiroDC> ObtenerSolicitudesAnteriores(string idGiro)
        {
            return GISolicitud.Instancia.ObtenerSolicitudesAnteriores(Convert.ToInt64(idGiro));
        }

        /// <summary>
        /// Actualizar la solicitud.
        /// </summary>
        /// <param name="solicitudAtendida">Propiedad de la solicitud a actualizar.</param>
        public GISolicitudGiroDC ActualizarSolicitud(GISolicitudGiroDC solicitudAtendida)
        {
            return GISolicitud.Instancia.ActualizarSolicitud(solicitudAtendida);
        }

        /// <summary>
        /// Metodo para Obtener un consecutivo
        /// de una solicitud
        /// </summary>
        /// <param name="tipoConsecutivo">Valor del tipo de consecutivo</param>
        /// <returns>El consecutivo de la solicitud</returns>
        public long ObtenerConsecutivoSolicitud()
        {
            return GISolicitud.Instancia.ObtenerConsecutivoSolicitud();
        }

        /// <summary>
        /// Metodo para Obtener los Centros de
        /// Servicios y Agencias por RACOL
        /// </summary>
        /// <param name="idRacol"> es el id del RACOL</param>
        /// <returns>lista de centros de servicio</returns>
        public List<PUCentroServiciosDC> ObtenerCentrosServicios(long idRacol)
        {
            return GISolicitud.Instancia.ObtenerCentrosServicios(idRacol);
        }

        /// <summary>
        /// Metodo para Obtener los Centros de servicios de un racol
        /// Que tengan giros por pagar
        /// </summary>
        /// <param name="idRacol"> es el id del RACOL</param>
        /// <returns>lista de centros de servicio</returns>
        public List<PUCentroServiciosDC> ObtenerCentrosServicioRacol(long idRacol)
        {
            return GISolicitud.Instancia.ObtenerCentrosServicioRacol(idRacol);
        }

        /// <summary>
        /// Obtiene Todas las agencias a
        /// nivel nacional que pueden pagar Giros
        /// </summary>
        /// <returns>lista de agencias</returns>
        public IList<PUCentroServiciosDC> ObtenerAgencias()
        {
            return GISolicitud.Instancia.ObtenerAgencias();
        }

        /// <summary>
        /// Metodo para Obtener las
        /// Regionales Administrativas
        /// </summary>
        /// <returns>lista de RACOL</returns>
        public List<PURegionalAdministrativa> ObtenerRegionalAdministrativa()
        {
            return GISolicitud.Instancia.ObtenerRegionalAdministrativa();
        }

        /// <summary>
        /// Obtiene posibles estados
        /// </summary>
        /// <returns></returns>
        public List<SEEstadoUsuario> ObtenerTiposEstado()
        {
            return GISolicitud.Instancia.ObtenerTiposEstado();
        }

        /// <summary>
        /// consulta los diferentes clasificaciones que existen para un agencia por el servicio de giros
        /// superhabitarias
        /// deficitarias
        /// compensadas
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public List<PUClasificacionPorIngresosDC> ConsultarClasificacionPorIngreso()
        {
            return GIAdministradorConfiguracionGiros.Instancia.ConsultarClasificacionPorIngreso();
        }

        /// <summary>
        /// Consultar los tipos de motivo  para la inactivacion de un giro
        /// </summary>
        /// <returns></returns>
        public List<string> ConsultarTiposMotivos()
        {
            return GIAdministradorConfiguracionGiros.Instancia.ConsultarTiposMotivos();
        }

        /// <summary>
        /// Guarda la nueva informacion de la configuracion de giros
        /// </summary>
        /// <param name="centroServicio"></param>
        public void GuardarConfiguracionGiro(PUCentroServiciosDC centroServicio)
        {
            GIAdministradorConfiguracionGiros.Instancia.GuardarConfiguracionGiro(centroServicio);
        }

        /// <summary>
        /// Obtener las observaciones de un centro de servicio
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public IList<PUObservacionCentroServicioDC> ObtenerObservacionCentroServicio(long idCentroServicio)
        {
            return GIAdministradorConfiguracionGiros.Instancia.ObtenerObservacionCentroServicio(idCentroServicio);
        }

        /// <summary>
        /// Metodo para consultar el ultimo estado del giro
        /// por el numero del giro
        /// </summary>
        /// <param name="idGiro"></param>
        /// <returns>el estado del giro</returns>
        public string ObtenerUltimoEstadoGiro(long idGiro, long idRacol)
        {
            return GISolicitud.Instancia.ObtenerUltimoEstadoGiro(idGiro, idRacol);
        }

        /// <summary>
        /// Inserta el Cambio del estado del giro
        /// </summary>
        /// <param name="solicitudAtendida"></param>
        /// <param name="contexto"></param>
        public void InsertarCambioEstadoGiro(long idAdmisionGiro, string estadoGiro)
        {
            GISolicitud.Instancia.InsertarCambioEstadoGiro(idAdmisionGiro, estadoGiro);
        }

        /// <summary>
        /// Se llenan los Datos Pendientes, Obtiene el
        /// consecutivo de la Solicitud y retorna
        /// Rafram
        /// </summary>
        /// <param name="nvaSolicitud"></param>
        /// <returns>la Info de la Solicitud para Imprimir Formato</returns>
        public GISolicitudGiroDC CrearSolicitudAnulacionComprobante(GISolicitudGiroDC nvaSolicitud)
        {
            return GISolicitud.Instancia.CrearSolicitudAnulacionComprobante(nvaSolicitud);
        }

        /// <summary>
        /// Valida que el comprobante de pago no haya sido usado
        /// aplica solo para pagos Manuales
        /// </summary>
        public void ValidarComprobantePago(long idSuministro, long idAgencia)
        {
            GISolicitud.Instancia.ValidarComprobantePago(idSuministro, idAgencia);
        }

        /// <summary>
        /// Valida el suministro
        /// </summary>
        /// <param name="idSuministro">suministro</param>
        /// <param name="tipoSuministro">tipo de Suministro</param>
        /// <returns>info del suministro</returns>
        public SUPropietarioGuia ValidarSuministro(long idSuministro, SUEnumSuministro tipoSuministro, long idPropietario)
        {
            return GISolicitud.Instancia.ValidarSuministro(idSuministro, tipoSuministro, idPropietario);
        }

        /// <summary>
        /// Valida el Suministro del numero de la Solicitud
        /// </summary>
        /// <param name="idSolicitud">suministro</param>
        public void ValidarSuministroNumeroSolicitud(long idSolicitud , long idPropietario)
        {
            ValidarSuministro(idSolicitud, SUEnumSuministro.SOLICITUD_MODIFICACION_GIROS_POSTALES, idPropietario);
        }

        /// <summary>
        /// Consulta los tipos de identificacion con los que se reclama el giro
        /// </summary>
        /// <returns>Lista de tipos de identificacion</returns>
        public IList<PATipoIdentificacion> ObtenerTiposIdentificacionReclamaGiros()
        {
            return GISolicitud.Instancia.ObtenerTiposIdentificacionReclamaGiros();
        }

        /// <summary>
        /// Obtiene la Informacion de Carga inicial
        /// </summary>
        /// <returns>info inicial de cargue</returns>
        public GIInfoCargaInicialSolicitudDC ObtenerInfoCargaInicialSolicitud()
        {
            return GISolicitud.Instancia.ObtenerInfoCargaInicialSolicitud();
        }

        /// <summary>
        /// Metodo para ejecutar el proceso de anulación de un giro en caja
        /// </summary>
        /// <param name="solicitudAtendida"></param>
        public PGPagoPorDevolucionDC DevolverGiroCaja(long idAdmisionGiro, int idCaja, long IdCentroServiciosPagador)
        {
            return GISolicitud.Instancia.DevolverGiroCaja(idAdmisionGiro, idCaja, IdCentroServiciosPagador);
        }
    }
}