using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Transactions;
using CO.Controller.Servidor.Integraciones.CuatroSieteDos;
using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Dominio.Comun.Cajas;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.Suministros;
using CO.Servidor.Dominio.Comun.Tarifas;
using CO.Servidor.Dominio.Comun.Util;
using CO.Servidor.GestionGiros.Comun;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros.Pagos;
using CO.Servidor.Servicios.ContratoDatos.Cajas;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Solicitudes;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Precios;
using CO.Servidor.Solicitudes.Datos;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Mensajeria;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Seguridad;
using Framework.Servidor.Servicios.ContratoDatos.Mensajeria;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using Framework.Servidor.Servicios.ContratoDatos.Parametros.Consecutivos;
using Framework.Servidor.Servicios.ContratoDatos.Seguridad;
using CO.Servidor.Servicios.ContratoDatos.Integraciones;

namespace CO.Servidor.Solicitudes.Giros.Solicitudes
{
    internal class GISolicitud : ControllerBase
    {
        private GIGirosPeatonPeatonDC datosRemitente;
        private MEMensajeEnviado infoMensaje;
        private static readonly GISolicitud instancia = (GISolicitud)FabricaInterceptores.GetProxy(new GISolicitud(), COConstantesModulos.GIROS);

        public static GISolicitud Instancia
        {
            get { return GISolicitud.instancia; }
        }

        #region Consulta

        /// <summary>
        /// Consulta el estado del giro y de la solicitud para realizar
        /// La creacion de una Solicitud Nueva
        /// </summary>
        /// <param name="idGiro">Identificador del giro.</param>
        /// <returns>Información del giro</returns>
        /// <remarks>Genera excepción si encuentra solicitudes activas para el id giro dado</remarks>
        public GISolicitudGiroDC ObtenerGiro(long idGiro, string digVerif)
        {
            if (GIDigitoChequeo.Instancia.ValidarDigitoChequeo(idGiro, digVerif))
            {
                return GIRepositorioSolicitudes.Instancia.ObtenerGiro(idGiro);
            }
            else
            {
                ControllerException excepcion = new ControllerException(COConstantesModulos.GIROS, GIEnumTipoErrorSolicitud.ERROR_DIGITO_CHEQUEO.ToString(),
                                                GISolicitudesServidorMensajes.CargarMensaje(GIEnumTipoErrorSolicitud.ERROR_DIGITO_CHEQUEO));
                throw new FaultException<ControllerException>(excepcion);
            }
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
            List<GISolicitudGiroDC> lstSolicitudesPorAgencia = GIRepositorioSolicitudes.Instancia.ObtenerSolicitudesPorAgencia(filtro, campoOrdenamiento, indicePagina, registrosPorPagina,
                                                                                        ordenamientoAscendente, out totalRegistros, idRacol, idAgencia);

            //Actualizo el estado del giro
            lstSolicitudesPorAgencia.ForEach(sol =>
            {
                sol.AdmisionGiro.EstadoGiro = ConvertirEstGiro(sol.AdmisionGiro.EstadoGiro).ToString();
            });

            return lstSolicitudesPorAgencia;
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
            List<GISolicitudGiroDC> lstSolicitudesActivas = GIRepositorioSolicitudes.Instancia.ObtenerSolicitudesActivas(filtro, campoOrdenamiento, indicePagina, registrosPorPagina,
                                                                                        ordenamientoAscendente, out totalRegistros, idRegional);

            //Actualizo el estado del giro
            lstSolicitudesActivas.ForEach(sol =>
            {
                sol.AdmisionGiro.EstadoGiro = ConvertirEstGiro(sol.AdmisionGiro.EstadoGiro).ToString();
            });

            return lstSolicitudesActivas;
        }

        /// <summary>
        /// Obtiene el detalle de una solicitud.
        /// </summary>
        /// <param name="idSolicitud">Valor de la solicitud a consultar.</param>
        /// <returns>el detalle de una solicitud</returns>
        public GISolicitudGiroDC ObtenerDetalleSolicitud(long idSolicitud)
        {
            GISolicitudGiroDC detalle = GIRepositorioSolicitudes.Instancia.ObtenerDetalleSolicitud(idSolicitud);
            detalle.CambioDestinatario.LstTipoIdentificacionReclama = new ObservableCollection<PATipoIdentificacion>(PAAdministrador.Instancia.ConsultarTiposIdentificacion());
            return detalle;
        }

        /// <summary>
        /// Obtiene el detalle de una solicitud.
        /// </summary>
        /// <param name="idSolicitud">id de la solicitud a consultar.</param>
        /// <returns>el detalle de una solicitud</returns>
        public GISolicitudGiroDC ObtenerDetalleSolicitudAprobada(long idSolicitud, long idRacol)
        {
            GISolicitudGiroDC detalle;
            detalle = GIRepositorioSolicitudes.Instancia.ObtenerSolictudAprobada(idSolicitud);

            ///VAlida que el racol enviado sea diferente al racol general para la opcion
            ///de aprobacion de cng, si es diferente del racol se debe validar que la solicitud
            ///pertenezca al racol enviado
            if (idRacol != GIConstantesSolicitudes.VALOR_GRAL_RACOL)
                if (detalle.IdRegionalAdmin != idRacol)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.GIROS, GIEnumTipoErrorSolicitud.ERROR_SOLICITUD_NO_PERTENECE_A_RACOL.ToString(),
                                                  GISolicitudesServidorMensajes.CargarMensaje(GIEnumTipoErrorSolicitud.ERROR_SOLICITUD_NO_PERTENECE_A_RACOL));
                    throw new FaultException<ControllerException>(excepcion);
                }
            return detalle;
        }

        /// <summary>
        /// Consulta la info del Motivo de
        /// una solicitud por el id
        /// </summary>
        /// <param name="idMotivo"></param>
        /// <returns>info del Motivo</returns>
        public GIMotivoSolicitudDC ObtenerMotivoSolicitud(int idMotivo)
        {
            return GIRepositorioSolicitudes.Instancia.ObtenerMotivoSolicitud(idMotivo);
        }

        /// <summary>
        /// Consulta la info de un tipo de Solicitud
        /// por su Id
        /// </summary>
        /// <param name="idTipoSolicitud"></param>
        /// <returns></returns>
        public GITipoSolicitudDC ObtenerTipoSolicitud(int idTipoSolicitud)
        {
            return GIRepositorioSolicitudes.Instancia.ObtenerTipoSolicitud(idTipoSolicitud);
        }

        /// <summary>
        /// Obtiene el archivo adjunto.
        /// </summary>
        /// <param name="idArchivo">Valor del id del archivo adjunto.</param>
        /// <returns>la direccion en bd del archivo</returns>
        public string ObtenerArchivoAdjunto(long idArchivo)
        {
            return GIRepositorioSolicitudes.Instancia.ObtenerArchivoAdjunto(idArchivo);
        }

        /// <summary>
        /// Obtien las solicitudes anteriores.
        /// </summary>
        /// <param name="idAdmisionGiro">Valor del idAdmisionGiro.</param>
        /// <returns>una lista de las solicitudes anteriores</returns>
        public List<GISolicitudGiroDC> ObtenerSolicitudesAnteriores(long idGiro)
        {
            return GIRepositorioSolicitudes.Instancia.ObtenerSolicitudesAnteriores(idGiro);
        }

        /// <summary>
        /// Metodo para Obtener un consecutivo
        /// de una solicitud
        /// </summary>
        /// <param name="tipoConsecutivo">Valor del tipo de consecutivo</param>
        /// <returns>El consecutivo de la solicitud</returns>
        public long ObtenerConsecutivoSolicitud()
        {
            using (TransactionScope scope = new TransactionScope())
            {
                long consecutivo = PAAdministrador.Instancia.ObtenerConsecutivo(PAEnumConsecutivos.Solicitudes_Giros);
                scope.Complete();
                return consecutivo;
            };
        }

        /// <summary>
        /// Metodo para Obtener los Centros de
        /// Servicios y Agencias por RACOL
        /// </summary>
        /// <param name="idRacol"> es el id del RACOL</param>
        /// <returns>lista de centros de servicio</returns>
        public List<PUCentroServiciosDC> ObtenerCentrosServicios(long idRacol)
        {
            return GIRepositorioSolicitudes.Instancia.ObtenerCentroServiciosTransmision(idRacol);
            //IPUFachadaCentroServicios fachadaCentroServicios = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();
            //return fachadaCentroServicios.ObtenerCentrosServicios(idRacol);
        }

        /// <summary>
        /// Metodo para Obtener los Centros de servicios de un racol
        /// Que tengan giros por pagar
        /// </summary>
        /// <param name="idRacol"> es el id del RACOL</param>
        /// <returns>lista de centros de servicio</returns>
        public List<PUCentroServiciosDC> ObtenerCentrosServicioRacol(long idRacol)
        {
            return GIRepositorioSolicitudes.Instancia.ObtenerCentroServiciosTransmision(idRacol);
            //IPUFachadaCentroServicios fachadaCentroServicios = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();
            //return fachadaCentroServicios.ObtenerCentroserviciosGirosATransmitir(idRacol);
        }

        /// <summary>
        /// Obtiene Todas las agencias a
        /// nivel nacional que pueden pagar giros
        /// </summary>
        /// <returns>lista de agencias</returns>
        public IList<PUCentroServiciosDC> ObtenerAgencias()
        {
            IPUFachadaCentroServicios fachadaCentroServicios = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();
            return fachadaCentroServicios.ObtenerAgenciasPuedenPagarGiros();
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
        /// Retorna o asigna las solicitudes de un giro
        /// </summary>
        /// <param name="idAdmisionGiro">Identificador Admisión giro</param>
        /// <returns>Colección de solicitudes</returns>
        public IEnumerable<GISolicitudGiroDC> ObtenerSolicitudesGiros(long idAdmisionGiro)
        {
            return GIRepositorioSolicitudes.Instancia.ObtenerSolicitudesGiros(idAdmisionGiro);
        }

        /// <summary>
        /// Retorna el archivo seleccionado de la solicitud
        /// </summary>
        /// <param name="idAdmisionGiro">Identificador admisión giro</param>
        /// <returns>Archivo</returns>
        public string ObtenerArchivoSolicitud(long idSolicitud, GIArchSolicitudDC archivoSeleccionado)
        {
            return GIRepositorioSolicitudes.Instancia.ObtenerArchivoSolicitud(idSolicitud, archivoSeleccionado);
        }

        /// <summary>
        /// Obtiene posibles estados
        /// </summary>
        /// <returns></returns>
        public List<SEEstadoUsuario> ObtenerTiposEstado()
        {
            return SEAdministradorSeguridad.Instancia.ObtenerTiposEstado();
        }

        /// <summary>
        /// Metodo para consultar el ultimo estado del giro
        /// por el numero del giro
        /// </summary>
        /// <param name="idGiro"></param>
        /// <returns>el estado del giro</returns>
        public string ObtenerUltimoEstadoGiro(long idGiro, long idRacol)
        {
            return GIRepositorioSolicitudes.Instancia.ObtenerGiroAgenciaOrigenRacol(idGiro, idRacol).EstadoGiro;
        }

        /// <summary>
        /// Obtiene la info del centro de servicio consultado
        /// </summary>
        /// <param name="idCentroServicio">id del centro de servicio</param>
        /// <returns>info del centro servicio</returns>
        private void ObtenerInfoCentroServicio(GISolicitudGiroDC solicitudAtendida)
        {
            IPUFachadaCentroServicios fachadaCentroServicios = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();
            PUCentroServiciosDC infoCentroSrv = fachadaCentroServicios.ObtenerCentroServicio(solicitudAtendida.CambioAgenciaPorAgencia.IdCentroServicio);

            solicitudAtendida.CambioAgenciaPorAgencia.CiudadUbicacion = new PALocalidadDC()
            {
                IdLocalidad = infoCentroSrv.CiudadUbicacion.IdLocalidad,
                Nombre = infoCentroSrv.CiudadUbicacion.Nombre
            };

            solicitudAtendida.CambioAgenciaPorAgencia.PaisCiudad = new PALocalidadDC()
            {
                IdLocalidad =  infoCentroSrv.IdPais,
                Nombre =  infoCentroSrv.NombrePais
            };
        }

        /// <summary>
        /// Consulta los tipos de identificacion con los que se reclama el giro
        /// </summary>
        /// <returns>Lista de tipos de identificacion</returns>
        public IList<PATipoIdentificacion> ObtenerTiposIdentificacionReclamaGiros()
        {
            IADFachadaAdmisionesGiros fachadaGiros = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesGiros>();
            return fachadaGiros.ConsultarTiposIdentificacionReclamaGiros();
        }

        /// <summary>
        /// Consulta los tipos de identificacion
        /// </summary>
        /// <returns>Lista de tipos de identificacion</returns>
        public IList<PATipoIdentificacion> ObtenerTiposIdentificacion()
        {
            return PAAdministrador.Instancia.ConsultarTiposIdentificacion();
        }

        /// <summary>
        /// Obtiene la Informacion de Carga inicial
        /// </summary>
        /// <returns></returns>
        public GIInfoCargaInicialSolicitudDC ObtenerInfoCargaInicialSolicitud()
        {
            GIInfoCargaInicialSolicitudDC infoCarga = new GIInfoCargaInicialSolicitudDC();

            infoCarga.LstTipoIdentificacionReclamaGiro = ObtenerTiposIdentificacionReclamaGiros();
            infoCarga.LstTipoIdentificacion = ObtenerTiposIdentificacion();
            infoCarga.LstEstados = ObtenerTiposEstado();

            return infoCarga;
        }

        /// <summary>
        /// Consulta los giros vendidor y pagados por algunos centros de servicio
        /// </summary>
        /// <param name="fechaInicial"></param>
        /// <param name="fechaFinal"></param>
        /// <returns></returns>
        public List<puntoDeAtencion> consultaIngresosEgresosPuntosDeAtencion(string fechaInicial, string fechaFinal)
        {
            return GIRepositorioSolicitudes.Instancia.consultaIngresosEgresosPuntosDeAtencion(fechaInicial, fechaFinal);
        }

        /// <summary>
        /// valida el usuario y contraseña para consumir el servicio por 472
        /// </summary>
        /// <param name="credencial"></param>
        /// <returns></returns>
        public bool ValidarPassword472(credencialDTO credencial)
        {
            return GIRepositorioSolicitudes.Instancia.ValidarPassword472(credencial);
        }

        #endregion Consulta

        #region Insertar

        /// <summary>
        /// Se llenan los Datos Pendientes, Obtiene el
        /// consecutivo de la Solicitud y retorna
        /// Rafram
        /// </summary>
        /// <param name="nvaSolicitud"></param>
        /// <returns>la Info de la Solicitud para Imprimir Formato</returns>
        public GISolicitudGiroDC CrearSolicitudAnulacionComprobante(GISolicitudGiroDC nvaSolicitud)
        {
            nvaSolicitud.IdSolicitud = ObtenerConsecutivoSolicitud();
            nvaSolicitud.MotivoSolicitud.DescripcionMotivo = GIConstantesSolicitudes.MOTIVO_SOLICITUD_ANULACION_COMPROBANTE;
            nvaSolicitud.TipoSolicitud.Descripcion = ObtenerTipoSolicitud((int)nvaSolicitud.TipoSolicitud.Id).Descripcion;

            //valido comprobante de pago
            AdicionarNvaSolicitud(nvaSolicitud);

            return nvaSolicitud;
        }

        /// <summary>
        /// Adicionar una nueva solicitud y envia Correo a
        /// los interesados internos y externos.
        /// </summary>
        /// <param name="nvaSolicitud">Propiedad de la nva solicitud.</param>
        public GISolicitudGiroDC AdicionarNvaSolicitud(GISolicitudGiroDC nvaSol)
        {
            if (nvaSol != null)
            {
                if (nvaSol.IdSolicitud == null)
                    nvaSol.IdSolicitud = ObtenerConsecutivoSolicitud();

                //Realiza los procesos de insercion dependiendo del tipo de solicitud
                InsertarNvaSolicitud(nvaSol);
                return nvaSol;
            }
            else
            {
                ControllerException excepcion = new ControllerException(COConstantesModulos.GIROS, GIEnumTipoErrorSolicitud
                                                                         .ERROR_SOLICITUD_SIN_DATOS.ToString(), GISolicitudesServidorMensajes
                                                                         .CargarMensaje(GIEnumTipoErrorSolicitud.ERROR_SOLICITUD_SIN_DATOS));
                throw new FaultException<ControllerException>(excepcion);
            }
        }

        /// <summary>
        /// Realiza los procesos de insercion dependiendo del tipo de solicitud
        /// </summary>
        /// <param name="nvaSol"></param>
        /// <param name="idSolicitud"></param>
        /// <returns>El id de la Solicitud</returns>
        private void InsertarNvaSolicitud(GISolicitudGiroDC nvaSol)
        {
            //Se valida el numero de la solicitud si es manual y se consulta por Suministros
            //que exista pero no se valida por propietario
            if (nvaSol.ValidoSuministroManual)
            {
                ValidarGuardarSuministroIdSolicitud(nvaSol.IdSolicitud.ToString(), nvaSol.CentroQueSolicita.IdCentroServicio);
            }

            //se clasifica la accion a realizar segun la solicitud
            switch (nvaSol.TipoSolicitud.Id)
            {
                case (int)GIEnumTipoSolicitud.AUTORIZACION_DEVOLUCION:
                    using (TransactionScope scope = new TransactionScope())
                    {
                        GIRepositorioSolicitudes.Instancia.AdicionarNvoRegistroSolicitud(nvaSol);

                        //agrego el cambio de la Agencia por el del origen y lo inserto en la
                        //tbl de cambio de agencia
                        nvaSol.CambioAgenciaPorAgencia = new PUCentroServiciosDC()
                        {
                            IdCentroServicio = nvaSol.AdmisionGiro.AgenciaOrigen.IdCentroServicio,
                            Nombre = nvaSol.AdmisionGiro.AgenciaOrigen.Nombre
                        };
                        scope.Complete();
                    }

                    GIRepositorioSolicitudes.Instancia.AdicionarSolicitudCambioAgencia(nvaSol);
                    if (!nvaSol.EsRacol)

                        //Adiciona los archivo asociados a una solicitud
                        AdicionarArchivosAsociadosSolicitud(nvaSol);
                    break;

                case (int)GIEnumTipoSolicitud.MODIFICACION_DATOS_DESTINATARIO:
                    using (TransactionScope scope = new TransactionScope())
                    {
                        //adiciona una Solicitud por cambio destinatario
                        GIRepositorioSolicitudes.Instancia.AdicionarSolicitudPorCambioDestinatario(nvaSol);
                        scope.Complete();
                    };
                    if (!nvaSol.EsRacol)

                        //Adiciona los archivo asociados a una solicitud
                        AdicionarArchivosAsociadosSolicitud(nvaSol);
                    break;

                case (int)GIEnumTipoSolicitud.CAMBIO_AGENCIA:
                    using (TransactionScope scope = new TransactionScope())
                    {
                        nvaSol.IdSolicitud = GIRepositorioSolicitudes.Instancia.AdicionarNvoRegistroSolicitud(nvaSol);
                        GIRepositorioSolicitudes.Instancia.AdicionarSolicitudCambioAgencia(nvaSol);
                        scope.Complete();
                    };
                    break;

                case (int)GIEnumTipoSolicitud.ANULACION_GIRO:

                    //Se valida que la fecha de creación de la Solicitud
                    //sea la mísma de creación del Giro
                    if (nvaSol.AdmisionGiro.FechaGrabacion.Date == DateTime.Now.Date)
                    {
                        using (TransactionScope scope = new TransactionScope())
                        {
                            nvaSol.IdSolicitud = GIRepositorioSolicitudes.Instancia.AdicionarNvoRegistroSolicitud(nvaSol);
                            scope.Complete();
                        };
                    }
                    else
                    {
                        ControllerException excepcion = new ControllerException(COConstantesModulos.GIROS, GIEnumTipoErrorSolicitud
                                                                     .ERROR_SOLICITUD_FECHA_ANULACION_GIRO.ToString(), string.Format(GISolicitudesServidorMensajes
                                                                     .CargarMensaje(GIEnumTipoErrorSolicitud.ERROR_SOLICITUD_FECHA_ANULACION_GIRO), nvaSol.AdmisionGiro.IdGiro.ToString()).Replace("\\n", "\n"));
                        throw new FaultException<ControllerException>(excepcion);
                    }
                    break;

                case (int)GIEnumTipoSolicitud.AJUSTE_VALOR_GIRO:
                    using (TransactionScope scope = new TransactionScope())
                    {
                        //Se valida que la fecha de creación de la Solicitud
                        //sea la mísma de creación del Giro
                        //if (nvaSol.AdmisionGiro.FechaGrabacion.Date == DateTime.Now.Date)
                        //{
                        nvaSol.IdSolicitud = GIRepositorioSolicitudes.Instancia.AdicionarNvoRegistroSolicitud(nvaSol);
                        //Calcula los Valores con el nuevo valor del giro y el valor de
                        /// los ajustes necesarios
                        CalcularNvaTarifaGiro(nvaSol);

                        //AdicionarValoresNvasTarifas
                        //para consultarlo en la aprobacion
                        AdicionarSolicitudCambioValor(nvaSol);
                        scope.Complete();
                    }
                    if (!nvaSol.EsRacol)

                        //Adiciona los archivo asociados a una solicitud
                        AdicionarArchivosAsociadosSolicitud(nvaSol);

                    //}
                    //else
                    //{
                    //  ControllerException excepcion = new ControllerException(COConstantesModulos.GIROS, GIEnumTipoErrorSolicitud
                    //                                               .ERROR_SOLICITUD_FECHA_AJUSTE_GIRO.ToString(), string.Format(GISolicitudesServidorMensajes
                    //                                               .CargarMensaje(GIEnumTipoErrorSolicitud.ERROR_SOLICITUD_FECHA_AJUSTE_GIRO), nvaSol.AdmisionGiro.IdGiro.ToString()).Replace("\\n", "\n"));
                    //  throw new FaultException<ControllerException>(excepcion);
                    //}
                    break;

                default:
                    using (TransactionScope scope = new TransactionScope())
                    {
                        // Adiciona las Solicitudes por Cambio de Estado y demas
                        nvaSol.IdSolicitud = GIRepositorioSolicitudes.Instancia.AdicionarNvoRegistroSolicitud(nvaSol);
                        scope.Complete();
                    }
                    break;
            }
        }

        /// <summary>
        /// Calcula los Valores con el nuevo
        /// valor del giro y el valor de
        /// los ajustes necesarios
        /// </summary>
        /// <param name="nvaSol"></param>
        private void CalcularNvaTarifaGiro(GISolicitudGiroDC nvaSol)
        {
            ITAFachadaTarifas fachadaTarifas = COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>();
            IEnumerable<TAPrecioRangoDC> RangoPrecios = fachadaTarifas.ObtenerPrecioRango((int)CAEnumConceptosCaja.ADMISION_GIROS);

            if (RangoPrecios != null)
            {
                TAPrecioRangoDC precioRango = RangoPrecios.FirstOrDefault(p =>
                                                                          p.PrecioInicial <= nvaSol.ValorRealGiro &&
                                                                          p.PrecioFinal >= nvaSol.ValorRealGiro);

                if (precioRango != null)
                {
                    nvaSol.AjustesGiroNvasTarifas = new Servicios.ContratoDatos.Tarifas.TAPrecioDC();

                    nvaSol.AjustesGiroNvasTarifas.ValorServicio = decimal.Round(precioRango.Valor + ((precioRango.Porcentaje / 100) * nvaSol.ValorRealGiro), 0);
                    nvaSol.AjustesGiroNvasTarifas.TarifaFijaPorte = precioRango.Valor;
                    nvaSol.AjustesGiroNvasTarifas.TarifaPorcPorte = precioRango.Porcentaje;

                    //Se calculan los las diferencias para ajustar los valores de Giro y Servicio ó Porte
                    nvaSol.AjusteAlGiro = nvaSol.ValorRealGiro - nvaSol.AdmisionGiro.Precio.ValorGiro;
                    nvaSol.AjusteAlPorte = nvaSol.AjustesGiroNvasTarifas.ValorServicio - nvaSol.AdmisionGiro.Precio.ValorServicio;
                }
                else
                {
                    //Valor Fuera de los Rangos
                    ControllerException excepcion = new ControllerException(COConstantesModulos.GIROS, GIEnumTipoErrorSolicitud
                                                                   .ERROR_TARIFA_FUERA_DE_LOS_RANGOS.ToString(), string.Format(GISolicitudesServidorMensajes
                                                                   .CargarMensaje(GIEnumTipoErrorSolicitud.ERROR_TARIFA_FUERA_DE_LOS_RANGOS), nvaSol.AdmisionGiro.IdGiro.ToString()).Replace("\\n", "\n"));
                    throw new FaultException<ControllerException>(excepcion);
                }
            }
            else
            {
                //No hay rango de precios
                ControllerException excepcion = new ControllerException(COConstantesModulos.GIROS, GIEnumTipoErrorSolicitud
                                                                 .ERROR_NO_HAY_RANGOS_PARA_EL_SERVICIO.ToString(), string.Format(GISolicitudesServidorMensajes
                                                                 .CargarMensaje(GIEnumTipoErrorSolicitud.ERROR_NO_HAY_RANGOS_PARA_EL_SERVICIO), nvaSol.AdmisionGiro.IdGiro.ToString()).Replace("\\n", "\n"));
                throw new FaultException<ControllerException>(excepcion);
            }
        }

        /// <summary>
        /// Adiciona los Archivos Asociados a una solicitud
        /// </summary>
        /// <param name="nvaSol">informacion de la solicitud</param>
        private void AdicionarArchivosAsociadosSolicitud(GISolicitudGiroDC nvaSol)
        {
            // Adiciona los Archivos Correspondientes
            if (nvaSol.ArchivosAsociados != null && nvaSol.ArchivosAsociados.Count > 0)
            {
                foreach (GIArchSolicitudDC arch in nvaSol.ArchivosAsociados)
                {
                    GIRepositorioSolicitudes.Instancia.AdicionarArchivosSolicitud(arch);
                };
            }
            else
            {
                ControllerException excepcion = new ControllerException(COConstantesModulos.GIROS, GIEnumTipoErrorSolicitud
                                                                 .ERROR_AL_CARGAR_LOS_ARCHIVOS.ToString(), GISolicitudesServidorMensajes
                                                                 .CargarMensaje(GIEnumTipoErrorSolicitud.ERROR_AL_CARGAR_LOS_ARCHIVOS));
                throw new FaultException<ControllerException>(excepcion);
            }
        }

        /// <summary>
        /// Crear una solicitud de cambio de agencia
        /// </summary>
        /// <param name="solicitudPago"></param>
        public GISolicitudGiroDC AdicionarNvaSolicitudCambioAgencia(GISolicitdPagoDC solicitudPago)
        {
            GISolicitudGiroDC infoSolicitud = null;
            long consecutivo = 0;

            using (TransactionScope scope = new TransactionScope())
            {
                consecutivo = PAAdministrador.Instancia.ObtenerConsecutivo(PAEnumConsecutivos.Solicitudes_Giros);

                GISolicitudGiroDC solicitud = new GISolicitudGiroDC()
                {
                    IdSolicitud = consecutivo,
                    AdmisionGiro = new GIAdmisionGirosDC()
                    {
                        IdAdminGiro = solicitudPago.IdAdminGiro,
                        IdGiro = solicitudPago.IdGiro
                    },
                    MotivoSolicitud = new GIMotivoSolicitudDC()
                    {
                        IdMotivo = (int)GIEnumMotivoSolicitudDC.MOTIVO_SOLICITUD_CAMBIO_AGENCIA,
                        DescripcionMotivo = GIRepositorioSolicitudes.Instancia.ConsultarDesMotivo((int)GIEnumMotivoSolicitudDC.MOTIVO_SOLICITUD_CAMBIO_AGENCIA)
                    },
                    TipoSolicitud = new GITipoSolicitudDC()
                    {
                        Id = GIConstantesSolicitudes.SOL_POR_CAMBIO_AGENCIA,
                        Descripcion = GIRepositorioSolicitudes.Instancia.ConsultarTipoSolicitud(GIConstantesSolicitudes.SOL_POR_CAMBIO_AGENCIA)
                    },

                    CentroQueSolicita = new PUCentroServiciosDC()
                    {
                        IdCentroServicio = solicitudPago.IdCentroSolicita,
                        Nombre = solicitudPago.NombreCentroSolicita
                    },

                    CambioAgenciaPorAgencia = new PUCentroServiciosDC()
                    {
                        IdCentroServicio = solicitudPago.IdCentroSolicita,
                        Nombre = solicitudPago.NombreCentroSolicita,
                    },

                    CentroServicioOrigen = new PUCentroServiciosDC()
                    {
                        IdCentroServicio = solicitudPago.CentroServicioInicial.IdCentroServicio,
                        Nombre = solicitudPago.CentroServicioInicial.Nombre,
                    },

                    IdRegionalAdmin = solicitudPago.IdRacol,
                    RegionalAdminDescripcion = solicitudPago.NombreRacol,

                    ObservSolicitud = string.Empty,
                    idConsecutivoFormato = consecutivo
                };

                AdicionarNvaSolicitud(solicitud);

                scope.Complete();
            };

            infoSolicitud = GIRepositorioSolicitudes.Instancia.ObtenerDataSolicitudGiro(consecutivo);
            return infoSolicitud;
        }

        /// <summary>
        /// Crear una solicitud de cambio de estado
        /// </summary>
        /// <param name="solicitudPago"></param>
        public GISolicitudGiroDC AdicionarNvaSolicitudCambioEstado(GISolicitdPagoDC solicitudPago)
        {
            GISolicitudGiroDC infoSolicitud = null;
            long consecutivo = 0;
            using (TransactionScope scope = new TransactionScope())
            {
                consecutivo = PAAdministrador.Instancia.ObtenerConsecutivo(PAEnumConsecutivos.Solicitudes_Giros);

                GISolicitudGiroDC solicitud = new GISolicitudGiroDC()
                {
                    IdSolicitud = consecutivo,
                    AdmisionGiro = new GIAdmisionGirosDC()
                    {
                        IdAdminGiro = solicitudPago.IdAdminGiro,
                        IdGiro = solicitudPago.IdGiro
                    },
                    MotivoSolicitud = new GIMotivoSolicitudDC()
                    {
                        IdMotivo = (int)GIEnumMotivoSolicitudDC.MOTIVO_SOLICITUD_CAMBIO_ESTADO,
                        DescripcionMotivo = GIRepositorioSolicitudes.Instancia.ConsultarDesMotivo((int)GIEnumMotivoSolicitudDC.MOTIVO_SOLICITUD_CAMBIO_ESTADO)
                    },
                    TipoSolicitud = new GITipoSolicitudDC()
                    {
                        Id = GIConstantesSolicitudes.SOL_POR_CAMBIO_ESTADO,
                        Descripcion = GIRepositorioSolicitudes.Instancia.ConsultarTipoSolicitud(GIConstantesSolicitudes.SOL_POR_CAMBIO_ESTADO)
                    },

                    CentroQueSolicita = new PUCentroServiciosDC()
                    {
                        IdCentroServicio = solicitudPago.IdCentroSolicita,
                        Nombre = solicitudPago.NombreCentroSolicita
                    },

                    IdRegionalAdmin = solicitudPago.IdRacol,
                    RegionalAdminDescripcion = solicitudPago.NombreRacol,
                    ObservSolicitud = string.Empty,
                    idConsecutivoFormato = consecutivo
                };

                AdicionarNvaSolicitud(solicitud);

                scope.Complete();
            };

            infoSolicitud = GIRepositorioSolicitudes.Instancia.ObtenerDataSolicitudGiro(consecutivo);
            return infoSolicitud;
        }

        /// <summary>
        /// Inserta el Cambio del estado del giro
        /// </summary>
        /// <param name="solicitudAtendida"></param>
        /// <param name="contexto"></param>
        public void InsertarCambioEstadoGiro(long idGiro, string estadoGiro)
        {
            long idAdminGiro = 0;
            IADFachadaAdmisionesGiros fachadaAdmisionGiros = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesGiros>();
            idAdminGiro = fachadaAdmisionGiros.ObtenerIdentificadorAdmisionGiro(idGiro).Value;

            GIRepositorioSolicitudes.Instancia.InsertarCambioEstadoGiro(idAdminGiro, estadoGiro);
        }

        /// <summary>
        /// Se adiciona la transaccion a la tabla de Anulaciones
        /// </summary>
        /// <param name="solicitudAtendida"></param>
        private void AdicionarSolicitudAnulacion(GISolicitudGiroDC solicitudAtendida)
        {
            GIRepositorioSolicitudes.Instancia.AdicionarSolicitudAnulacion(solicitudAtendida);
        }

        /// <summary>
        /// Adiciona los Datos del valores y ajustes
        /// para el cambio de una solicitud
        /// </summary>
        /// <param name="solicitudAtendida">info de la nva Sol</param>
        private void AdicionarSolicitudCambioValor(GISolicitudGiroDC solicitudAtendida)
        {
            GIRepositorioSolicitudes.Instancia.AdicionarSolicitudCambioValor(solicitudAtendida);
        }

        #endregion Insertar

        #region Actualizar

        /// <summary>
        /// Actualizar la solicitud.
        /// </summary>
        /// <param name="solicitudAtendida">Propiedad de la solicitud a actualizar.</param>
        public GISolicitudGiroDC ActualizarSolicitud(GISolicitudGiroDC solicitudAtendida)
        {
            IPUFachadaCentroServicios fachadaCentroServicios = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();
            if (fachadaCentroServicios.ObtenerCentroServicioActivo(solicitudAtendida.CentroQueSolicita.IdCentroServicio))
            {
                ProcesarGestionSolicitud(solicitudAtendida);
                EnviarCorreoSolicitud(solicitudAtendida);

                return solicitudAtendida;
            }
            else
            {
                ControllerException excepcion = new ControllerException(COConstantesModulos.GIROS, GIEnumTipoErrorSolicitud
                                                                             .ERROR_AGENCIA_NO_ACTA_PARA_PAGO.ToString(), GISolicitudesServidorMensajes
                                                                             .CargarMensaje(GIEnumTipoErrorSolicitud.ERROR_AGENCIA_NO_ACTA_PARA_PAGO));
                throw new FaultException<ControllerException>(excepcion);
            }
        }

        #endregion Actualizar

        #region Gestion

        /// <summary>
        /// Envia los Correos internos y externos de las solicitudes
        /// </summary>
        /// <param name="solicitudAtendida"></param>
        private void EnviarCorreoSolicitud(GISolicitudGiroDC solicitudAtendida)
        {
            datosRemitente = GIRepositorioSolicitudes.Instancia.ObtenerDatosRemitente(solicitudAtendida);

            //Envio mail externo se valida el mail del cliente para enviar correo interno
            //if (!string.IsNullOrEmpty(datosRemitente.ClienteRemitente.Email))
            //{
            //  PAAdministrador.Instancia.EnviarCorreoUnicoDestinatarioDosVariables(GIConstantesSolicitudes.ASUNTO_TIPO_DE_CONTENIDO_MAIL,
            //    datosRemitente.ClienteRemitente.Email, solicitudAtendida.EstadoSol, solicitudAtendida.IdSolicitud.ToString());
            //}

            //Envio mail interno
            infoMensaje = LlenarInfoMailInterno(solicitudAtendida, solicitudAtendida.EstadoSol);
            MEManejadorMensajeria.Instancia.CrearMensajeNuevo(infoMensaje, ControllerContext.Current.Usuario.ToString());
        }

        /// <summary>
        /// Metodo de llenado
        /// de la propiedad para enviar
        /// mail interno
        /// </summary>
        /// <param name="solicitud"> Solicitud a enviar</param>
        /// <param name="estadoSolicitud">Estado de la solicitud</param>
        /// <returns></returns>
        public MEMensajeEnviado LlenarInfoMailInterno(GISolicitudGiroDC solicitud, string estadoSolicitud)
        {
            MEMensajeEnviado newDatos = new MEMensajeEnviado()
            {
                AceptaRespuestas = false,
                Asunto = GIConstantesSolicitudes.ASUNTO_MAIL_RTA_SOLICITUD + solicitud.IdSolicitud.ToString(),
                FechaCreacion = DateTime.Now,
                UsuarioOrigen = ControllerContext.Current.Usuario,
                CentroServicioDestino = solicitud.CentroQueSolicita.IdCentroServicio,
                DescCentroServicioDestino = solicitud.CentroQueSolicita.Nombre,
                Texto = GIConstantesSolicitudes.CONTENIDO_MAIL_RTA_SOLICITUD + estadoSolicitud,
                Categoria = 2,
                EstadoNotificacion = 0,
            };
            return newDatos;
        }

        /// <summary>
        /// Gestionar los procesos de la solicitud segun su requerimiento.
        /// </summary>
        /// <param name="solicitudAtendida">Propiedad de la solicitud a actualizar.</param>
        /// <returns>el estado de la solicitud</returns>
        public GISolicitudGiroDC ProcesarGestionSolicitud(GISolicitudGiroDC solicitudAtendida)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                //Se valida se es una solicitud rechazada
                if (solicitudAtendida.SolicitudRechazada)
                {
                    solicitudAtendida.EstadoSol = GIConstantesSolicitudes.ESTADO_SOL_REC;
                }
                else
                {
                    //Valido el tipo de solicitud sea anulacion comprobante de pago
                    if (solicitudAtendida.TipoSolicitud.Id == (int)GIEnumTipoSolicitud.ANULACION_COMPROBANTE_PAGO)
                    {
                        //Valido que el comprobante de pago no alla sido utilizado
                        ValidarComprobantePago(solicitudAtendida.AdmisionGiro.IdGiro.Value, solicitudAtendida.CentroQueSolicita.IdCentroServicio, false);

                        //Anulo el consumo de suministro
                        AnularComprobantePagoGiro(solicitudAtendida);

                        //Adiciono la Anulacion a la tbl respectiva
                        AdicionarSolicitudAnulacion(solicitudAtendida);

                        //Actualizo la Solicitud como Aprobada para todos los casos
                        solicitudAtendida.EstadoSol = GIConstantesSolicitudes.ESTADO_SOL_APR;

                        //se realiza la integración con 472 con estado anulado debido a que se retorna el valor del giro y la comision
                        //En el documento de 472 dice que       2.	Giro Anulado: Es todo giro que por error de la empresa prestadora del servicio, error tecnológico o humano del (Front Office canal aliado) no pudo ser efectuado, por lo tanto se reversa toda la operación. (Monto del giro y comisión)
                        //Integracion472.Instancia.IntegrarCuatroSieteDosPagoDevAnul(solicitudAtendida.AdmisionGiro.IdGiro.Value, "2", DateTime.Now);
                        // TODO: RON Prueba de deshabilitación integración
                    }
                    else
                    {
                        // Se valida el estado del giro para aprobar ó rechazar
                        if (GIRepositorioSolicitudes.Instancia.ObtenerEstadoGiro(Convert.ToInt64(solicitudAtendida.AdmisionGiro.IdAdminGiro)))
                        {
                            ////Guarda en las observaciones del giro el tipo de solicitud
                            GIRepositorioSolicitudes.Instancia.ModificarObservacionesAdmisionGiro(solicitudAtendida.IdGiro, solicitudAtendida.TipoSolicitud.Descripcion);

                            //Se tramita las solicitudes aprobadas segun el tipo
                            TramitarSolicitud(solicitudAtendida);
                        }
                        else
                        {
                            ControllerException excepcion = new ControllerException(COConstantesModulos.GIROS, GIEnumTipoErrorSolicitud
                                                                                    .ERROR_GIRO_YA_PAGO.ToString(), GISolicitudesServidorMensajes
                                                                                    .CargarMensaje(GIEnumTipoErrorSolicitud.ERROR_GIRO_YA_PAGO));
                            throw new FaultException<ControllerException>(excepcion);
                        }
                    }
                }

                //Se adiciona en Solicitudes_Atendidas y Actualiza en el estado de la Solicitud segun su gestión
                GIRepositorioSolicitudes.Instancia.AdicionarActualizarSolicitudAtendida(solicitudAtendida);
                scope.Complete();
            }

            //Se retorna el estado de la Solicitud texto
            //return GIRepositorioSolicitudes.Instancia.ConvertirEstadoSolGiro(solicitudAtendida.EstadoSol).ToString();

            return solicitudAtendida;
        }

        /// <summary>
        /// Valida el id del suministro, no valido el
        /// propietario
        /// </summary>
        /// <param name="idSuministro">es el tipo de suministro</param>
        /// <param name="idAgencia">es le centro de servicio</param>
        public void ValidarGuardarSuministroIdSolicitud(string idSuministro, long idAgencia)
        {
            ISUFachadaSuministros fachadaSuministros = COFabricaDominio.Instancia.CrearInstancia<ISUFachadaSuministros>();

            long idSolicitud = Convert.ToInt64(idSuministro);

            SUPropietarioGuia suministroAsociado = null;
            suministroAsociado = ValidarSuministro(idSolicitud, SUEnumSuministro.SOLICITUD_MODIFICACION_GIROS_POSTALES, idAgencia);

            if (suministroAsociado != null && suministroAsociado.CentroServicios.IdCentroServicio == idAgencia)
            {
                SUEnumGrupoSuministroDC grupo = (SUEnumGrupoSuministroDC)Enum.Parse(typeof(SUEnumGrupoSuministroDC), suministroAsociado.CentroServicios.Tipo);

                SUConsumoSuministroDC consumo = null;

                consumo = new SUConsumoSuministroDC()
                {
                    Cantidad = 1,
                    EstadoConsumo = SUEnumEstadoConsumo.CON,
                    GrupoSuministro = grupo,
                    IdDuenoSuministro = idAgencia,
                    IdServicioAsociado = 0,
                    NumeroSuministro = idSolicitud,
                    Suministro = SUEnumSuministro.SOLICITUD_MODIFICACION_GIROS_POSTALES
                };
                fachadaSuministros.GuardarConsumoSuministro(consumo);
            }
            else
            {
                //El id de la Solicitud no pertenece al punto de servicio que lo quiere utilizar
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.GIROS, GIEnumTipoErrorSolicitud
                                                                        .ERROR_NUMERO_SOLICITUD_MANUAL_NO_EXISTE.ToString(), GISolicitudesServidorMensajes
                                                                        .CargarMensaje(GIEnumTipoErrorSolicitud.ERROR_NUMERO_SOLICITUD_MANUAL_NO_EXISTE)));
            }
        }

        /// <summary>
        /// Valida que el comprobante de pago no haya sido usado
        /// aplica solo para pagos Manuales
        /// </summary>
        public void ValidarComprobantePago(long idSuministro, long idAgencia, bool validarSolicitudActiva = true)
        {
            GISolicitudGiroDC solicitudActiva = null;

            if (validarSolicitudActiva)
            {
                solicitudActiva = GIRepositorioSolicitudes.Instancia.ConsultarSolicitudActivaPorNumeroGiro(idSuministro);

                if (solicitudActiva == null)
                {
                    ValidarSuministro(idSuministro, SUEnumSuministro.COMPROBANTE_PAGO_GIRO_MANUAL, idAgencia);
                }
                else
                {
                    //Ya existe una solicitud Activa para ese numero de Comprobante
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.GIROS, GIEnumTipoErrorSolicitud
                                                                            .ERROR_SOLICITUD_NUMERO_COMPROBANTE_YA_CREADA.ToString(),
                                                                            string.Format(GISolicitudesServidorMensajes.
                                                                            CargarMensaje(GIEnumTipoErrorSolicitud.ERROR_SOLICITUD_NUMERO_COMPROBANTE_YA_CREADA).ToString(), idSuministro.ToString(), solicitudActiva.IdSolicitud.ToString())));
                }
            }
            else
            {
                ValidarSuministro(idSuministro, SUEnumSuministro.COMPROBANTE_PAGO_GIRO_MANUAL, idAgencia);
            }
        }

        /// <summary>
        /// Anula el comprobante de pago de un giro
        /// </summary>
        /// <param name="solicitudAtendida"></param>
        private static void AnularComprobantePagoGiro(GISolicitudGiroDC solicitudAtendida)
        {
            //Proceso de aprobacion o rechazo anulación comporbante
            ISUFachadaSuministros fachadaSuministros = COFabricaDominio.Instancia.CrearInstancia<ISUFachadaSuministros>();
            IPUFachadaCentroServicios fachadaCentroServicios = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();

            PUCentroServiciosDC centroServ = fachadaCentroServicios.ObtenerCentroServicio(solicitudAtendida.CentroQueSolicita.IdCentroServicio);
            SUEnumGrupoSuministroDC grupo = (SUEnumGrupoSuministroDC)Enum.Parse(typeof(SUEnumGrupoSuministroDC), centroServ.Tipo);
            SUConsumoSuministroDC consumo = null;

            consumo = new SUConsumoSuministroDC()
            {
                Cantidad = 1,
                EstadoConsumo = SUEnumEstadoConsumo.ANU,
                GrupoSuministro = grupo,
                IdDuenoSuministro = solicitudAtendida.CentroQueSolicita.IdCentroServicio,
                IdServicioAsociado = 0,
                NumeroSuministro = solicitudAtendida.AdmisionGiro.IdGiro.Value,
                Suministro = SUEnumSuministro.COMPROBANTE_PAGO_GIRO_MANUAL
            };
            fachadaSuministros.GuardarConsumoSuministro(consumo);
        }

        /// <summary>
        /// Maneja la Logica de solicitud segun se el caso e inserta en solicitudes atendidas
        /// </summary>
        /// <param name="solicitudAtendida">es la solicitud a Tramitar</param>
        private void TramitarSolicitud(GISolicitudGiroDC solicitudAtendida)
        {
            IADFachadaAdmisionesGiros fachadaAdmisionGiros = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesGiros>();

            //se clasifica la accion a realizar segun la solicitud
            switch (solicitudAtendida.TipoSolicitud.Id)
            {
                case (int)GIEnumTipoSolicitud.AUTORIZACION_DEVOLUCION:

                    //Consulto la info de la nueva ciudad del centro de servicio al que va dirigido el giro
                    ObtenerInfoCentroServicio(solicitudAtendida);

                    GIRepositorioSolicitudes.Instancia.ActualizarSolicitudPorDevolucion(solicitudAtendida);

                    //Actualizo el Estado del Giro a Devolución
                    InsertarCambioEstadoGiro(solicitudAtendida.AdmisionGiro.IdGiro.Value, GIConstantesSolicitudes.ESTADO_GIRO_DEV);

                    //Actualizo la Data del Giro
                    fachadaAdmisionGiros.ActualizarInfoGiro(solicitudAtendida.AdmisionGiro);

                    //Proceso de Anulación en Caja del Giro
                    // DevolverGiroCaja(solicitudAtendida);
                    break;

                case (int)GIEnumTipoSolicitud.MODIFICACION_DATOS_DESTINATARIO:
                    GIRepositorioSolicitudes.Instancia.ActualizarSolicitudPorCambioDestinatario(solicitudAtendida);

                    //Actualizo la Data del Giro
                    fachadaAdmisionGiros.ActualizarInfoGiro(solicitudAtendida.AdmisionGiro);

                    break;

                case (int)GIEnumTipoSolicitud.CAMBIO_AGENCIA:

                    //Consulto la info de la nueva ciudad del centro de servicio al que va dirigido el giro
                    ObtenerInfoCentroServicio(solicitudAtendida);

                    /// Actualiza la Agencia en la Solicitud
                    ActualizarAgenciaGiroPorSolicitud(solicitudAtendida, fachadaAdmisionGiros);

                    break;

                case (int)GIEnumTipoSolicitud.CAMBIO_ESTADO_CUSTODIA_SIN_CANCELAR:
                    InsertarCambioEstadoGiro(solicitudAtendida.AdmisionGiro.IdGiro.Value, GIConstantesSolicitudes.ESTADO_GIRO_ACT);
                    solicitudAtendida.AdmisionGiro = ObtenerInfoGiro(solicitudAtendida.AdmisionGiro.IdAdminGiro.Value);

                    break;

                case (int)GIEnumTipoSolicitud.CAMBIO_ESTADO_REZAGO_SIN_CANCELAR:
                    InsertarCambioEstadoGiro(solicitudAtendida.AdmisionGiro.IdGiro.Value, GIConstantesSolicitudes.ESTADO_GIRO_ACT);
                    solicitudAtendida.AdmisionGiro = ObtenerInfoGiro(solicitudAtendida.AdmisionGiro.IdAdminGiro.Value);
                    break;

                case (int)GIEnumTipoSolicitud.ANULACION_GIRO:

                    //Adiciono la Anulacion a la tbl respectiva
                    AnularGiro(solicitudAtendida);
                    solicitudAtendida.AdmisionGiro = ObtenerInfoGiro(solicitudAtendida.AdmisionGiro.IdAdminGiro.Value);
                    break;

                case (int)GIEnumTipoSolicitud.AJUSTE_VALOR_GIRO:

                    //Se ajusta el Valor del giro en Caja
                    solicitudAtendida.AdmisionGiro = ObtenerInfoGiro(solicitudAtendida.AdmisionGiro.IdAdminGiro.Value);
                    AjustarValorGiroCaja(solicitudAtendida);

                    break;
            }

            //Actualizo la Solicitud como Aprobada para todos los casos
            solicitudAtendida.EstadoSol = GIConstantesSolicitudes.ESTADO_SOL_APR;
        }

        /// <summary>
        /// Actualiza la Agencia en la Solicitud
        /// </summary>
        /// <param name="solicitudAtendida"></param>
        private void ActualizarAgenciaGiroPorSolicitud(GISolicitudGiroDC solicitudAtendida, IADFachadaAdmisionesGiros fachadaAdmisionGiros)
        {
            solicitudAtendida.AdmisionGiro = GIRepositorioSolicitudes.Instancia.ObtenerInfoGiro(solicitudAtendida.AdmisionGiro.IdAdminGiro.Value);

            //Se actualiza la Ciudad
            solicitudAtendida.AdmisionGiro.AgenciaDestino.CiudadUbicacion.IdLocalidad = solicitudAtendida.CambioAgenciaPorAgencia.CiudadUbicacion.IdLocalidad;
            solicitudAtendida.AdmisionGiro.AgenciaDestino.CiudadUbicacion.Nombre = solicitudAtendida.CambioAgenciaPorAgencia.CiudadUbicacion.Nombre;

            //Se actualiza el Centro se Servicio
            solicitudAtendida.AdmisionGiro.AgenciaDestino.IdCentroServicio = solicitudAtendida.CambioAgenciaPorAgencia.IdCentroServicio;
            solicitudAtendida.AdmisionGiro.AgenciaDestino.Nombre = solicitudAtendida.CambioAgenciaPorAgencia.Nombre;

            //Se actualiza el pais
            solicitudAtendida.AdmisionGiro.AgenciaDestino.PaisCiudad.IdLocalidad = solicitudAtendida.CambioAgenciaPorAgencia.PaisCiudad.IdLocalidad;
            solicitudAtendida.AdmisionGiro.AgenciaDestino.PaisCiudad.Nombre = solicitudAtendida.CambioAgenciaPorAgencia.PaisCiudad.Nombre;

            //Por requerimiento si el giro esta transmitido a una agencia lo actualiza para que quede libre para Transmitir
            if (solicitudAtendida.AdmisionGiro.EsTransmitido)
            {
                solicitudAtendida.AdmisionGiro.EsTransmitido = false;
            }

            //Actualizo la Data del Giro
            fachadaAdmisionGiros.ActualizarInfoGiro(solicitudAtendida.AdmisionGiro);
        }

        /// <summary>
        /// Obtiene la Info del Giro
        /// remitente, destinatario,origen,destino
        /// </summary>
        /// <param name="idAdmision">idAdmisiongiro</param>
        /// <returns>info del Giro</returns>
        public GIAdmisionGirosDC ObtenerInfoGiro(long idAdmision)
        {
            return GIRepositorioSolicitudes.Instancia.ObtenerInfoGiro(idAdmision);
        }

        /// <summary>
        /// Valida el suministro
        /// </summary>
        /// <param name="idSuministro">suministro</param>
        /// <param name="tipoSuministro">tipo de Suministro</param>
        /// <returns>info del suministro</returns>
        public SUPropietarioGuia ValidarSuministro(long idSuministro, SUEnumSuministro tipoSuministro, long idPropietario)
        {
            ISUFachadaSuministros fachadaSuministros = COFabricaDominio.Instancia.CrearInstancia<ISUFachadaSuministros>();
            return fachadaSuministros.ObtenerPropietarioSuministro(idSuministro, tipoSuministro, idPropietario);
        }

        /// <summary>
        /// se inserta la solicitud atendida en la tbla solicitud anulación
        /// Numero Comprobante de Pago Guio
        /// </summary>
        /// <param name="solicitudAtendida">la Informacion para insertar la anulación</param>
        public void AnularGiro(GISolicitudGiroDC solicitudAtendida)
        {
            //Se actualiza el estado del giro a anulado
            InsertarCambioEstadoGiro(solicitudAtendida.AdmisionGiro.IdGiro.Value, GIConstantesSolicitudes.ESTADO_GIRO_ANU);

            //Se adiciona la transaccion a la tabla de Anulaciones
            AdicionarSolicitudAnulacion(solicitudAtendida);

            //proceso de anulación del giro en Caja
            AnularGiroEnCaja(solicitudAtendida);
        }

        /// <summary>
        /// Metodo para ejecutar el proceso de anulación de un giro en caja
        /// </summary>
        /// <param name="solicitudAtendida"></param>
        public PGPagoPorDevolucionDC DevolverGiroCaja(long idAdmisionGiro, int idCaja, long idCentroServiciosPagador)
        {
            ICAFachadaCajas fachadaCajas = COFabricaDominio.Instancia.CrearInstancia<ICAFachadaCajas>();
            IADFachadaAdmisionesGiros fachadaAdmisionGiros = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesGiros>();

            GISolicitudGiroDC solicitudAtendida = GIRepositorioSolicitudes.Instancia.ObtenerSolicitudPorIdGiroAdmision(idAdmisionGiro,
                                                                                          (int)GIEnumTipoSolicitud.AUTORIZACION_DEVOLUCION);

            solicitudAtendida.AdmisionGiro = GIRepositorioSolicitudes.Instancia.ObtenerInfoGiro(idAdmisionGiro);

            solicitudAtendida.AdmisionGiro.IdCaja = idCaja;

            PGPagoPorDevolucionDC infoDevolucion = new PGPagoPorDevolucionDC();
            infoDevolucion.MotivoSolicitud = new GIMotivoSolicitudDC();

            infoDevolucion.ValorGiroAPagarPorDevolucion = 0;

            switch (solicitudAtendida.MotivoSolicitud.IdMotivo)
            {
                case (int)GIEnumMotivoSolicitudDC.SOLICITADA_REMITENTE:
                    fachadaCajas.DevolucionSinPorteGiro(solicitudAtendida.AdmisionGiro, (int)CAEnumConceptosCaja.ADMISION_GIROS, idCentroServiciosPagador);

                    infoDevolucion.ValorGiroAPagarPorDevolucion = solicitudAtendida.AdmisionGiro.Precio.ValorGiro;

                    infoDevolucion.MotivoSolicitud.DescripcionMotivo = solicitudAtendida.MotivoSolicitud.DescripcionMotivo;
                    return infoDevolucion;

                default:
                    fachadaCajas.DevolucionConPorteGiro(solicitudAtendida.AdmisionGiro, (int)CAEnumConceptosCaja.ADMISION_GIROS, idCentroServiciosPagador);
                    fachadaCajas.DevolucionSinPorteGiro(solicitudAtendida.AdmisionGiro, (int)CAEnumConceptosCaja.ADMISION_GIROS, idCentroServiciosPagador);

                    solicitudAtendida.AdmisionGiro.Precio.ValorGiro = solicitudAtendida.AdmisionGiro.Precio.ValorGiro + solicitudAtendida.AdmisionGiro.Precio.ValorServicio;
                    solicitudAtendida.AdmisionGiro.Precio.ValorServicio = solicitudAtendida.AdmisionGiro.Precio.ValorServicio;
                    solicitudAtendida.AdmisionGiro.Precio.ValorTotal = solicitudAtendida.AdmisionGiro.Precio.ValorGiro + solicitudAtendida.AdmisionGiro.Precio.ValorServicio +
                                                                      solicitudAtendida.AdmisionGiro.Precio.ValorImpuestos + solicitudAtendida.AdmisionGiro.Precio.ValorAdicionales;

                    infoDevolucion.ValorGiroAPagarPorDevolucion = solicitudAtendida.AdmisionGiro.Precio.ValorGiro;

                    infoDevolucion.MotivoSolicitud.DescripcionMotivo = solicitudAtendida.MotivoSolicitud.DescripcionMotivo;

                    //Actualizo la Data del Giro
                    fachadaAdmisionGiros.ActualizarInfoGiro(solicitudAtendida.AdmisionGiro);
                    return infoDevolucion;

                //case (int)GIEnumMotivoSolicitudDC.AGENCIA_SIN_SERVICIO:
                //  fachadaCajas.DevolucionConPorteGiro(solicitudAtendida.AdmisionGiro.IdGiro.Value, (int)CAEnumConceptosCaja.ADMISION_GIROS);
                //  fachadaCajas.DevolucionSinPorteGiro(solicitudAtendida.AdmisionGiro.IdGiro.Value, (int)CAEnumConceptosCaja.ADMISION_GIROS);

                //  solicitudAtendida.AdmisionGiro.Precio.ValorGiro = solicitudAtendida.AdmisionGiro.Precio.ValorGiro + solicitudAtendida.AdmisionGiro.Precio.ValorServicio;
                //  solicitudAtendida.AdmisionGiro.Precio.ValorServicio = solicitudAtendida.AdmisionGiro.Precio.ValorServicio;
                //  solicitudAtendida.AdmisionGiro.Precio.ValorTotal = solicitudAtendida.AdmisionGiro.Precio.ValorGiro + solicitudAtendida.AdmisionGiro.Precio.ValorServicio +
                //                                                    solicitudAtendida.AdmisionGiro.Precio.ValorImpuestos + solicitudAtendida.AdmisionGiro.Precio.ValorAdicionales;
                //  //Actualizo la Data del Giro
                //  fachadaAdmisionGiros.ActualizarInfoGiro(solicitudAtendida.AdmisionGiro);

                //  break;

                //case (int)GIEnumMotivoSolicitudDC.AGENCIA_SIN_DINERO:
                //  fachadaCajas.DevolucionConPorteGiro(solicitudAtendida.AdmisionGiro.IdGiro.Value, (int)CAEnumConceptosCaja.ADMISION_GIROS);
                //  fachadaCajas.DevolucionSinPorteGiro(solicitudAtendida.AdmisionGiro.IdGiro.Value, (int)CAEnumConceptosCaja.ADMISION_GIROS);
                //  break;

                //case (int)GIEnumMotivoSolicitudDC.ERROR_OPERATIVO:
                //  fachadaCajas.DevolucionConPorteGiro(solicitudAtendida.AdmisionGiro.IdGiro.Value, (int)CAEnumConceptosCaja.ADMISION_GIROS);
                //  fachadaCajas.DevolucionSinPorteGiro(solicitudAtendida.AdmisionGiro.IdGiro.Value, (int)CAEnumConceptosCaja.ADMISION_GIROS);
                //  break;

                //default:
                //  ControllerException excepcion = new ControllerException(COConstantesModulos.GIROS, GIEnumTipoErrorSolicitud.ERROR_MOTIVO_DEVOLUCION_NO_CONFIGURADO.ToString(),
                //                                GISolicitudesServidorMensajes.CargarMensaje(GIEnumTipoErrorSolicitud.ERROR_MOTIVO_DEVOLUCION_NO_CONFIGURADO));
                //  throw new FaultException<ControllerException>(excepcion);
            }
        }

        /// <summary>
        /// Metodo para la anulación de un giro
        /// "la solicitud debe ser de la misma fecha de la solcitud"
        /// </summary>
        /// <param name="anulacionGiro">info de la Anulacion del Giro</param>
        private void AnularGiroEnCaja(GISolicitudGiroDC anulacionGiro)
        {
            ICAFachadaCajas fachadaCajas = COFabricaDominio.Instancia.CrearInstancia<ICAFachadaCajas>();
            fachadaCajas.AnularGiroCaja(anulacionGiro.AdmisionGiro.IdGiro.Value, (int)CAEnumConceptosCaja.ADMISION_GIROS);
        }

        /// <summary>
        /// Metodo para realizar el ajuste del valor del giro en Caja
        /// </summary>
        /// <param name="solicitudAtendida">info de la Solicitud</param>
        public void AjustarValorGiroCaja(GISolicitudGiroDC solicitudAtendida)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                IADFachadaAdmisionesGiros fachadaAdmisionGiros = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesGiros>();

                ICAFachadaCajas fachadaCajas = COFabricaDominio.Instancia.CrearInstancia<ICAFachadaCajas>();
                fachadaCajas.AjustarValorGiroCaja(solicitudAtendida);

                solicitudAtendida.AdmisionGiro.Precio.ValorGiro = solicitudAtendida.AdmisionGiro.Precio.ValorGiro + solicitudAtendida.AjusteAlGiro;
                solicitudAtendida.AdmisionGiro.Precio.ValorServicio = solicitudAtendida.AdmisionGiro.Precio.ValorServicio + solicitudAtendida.AjusteAlPorte;
                solicitudAtendida.AdmisionGiro.Precio.ValorTotal = solicitudAtendida.AdmisionGiro.Precio.ValorGiro + solicitudAtendida.AdmisionGiro.Precio.ValorServicio +
                                                                  solicitudAtendida.AdmisionGiro.Precio.ValorImpuestos + solicitudAtendida.AdmisionGiro.Precio.ValorAdicionales;

                //Actualizo la Data del Giro
                fachadaAdmisionGiros.ActualizarInfoGiro(solicitudAtendida.AdmisionGiro);

                scope.Complete();
            }
        }

        #region Enumerables

        /// <summary>
        /// Convierte el estado del giro.
        /// </summary>
        /// <param name="estado">valor actual del estado.</param>
        /// <returns>la palabra del Estado</returns>
        public GIEnumEstadosGirosDC ConvertirEstGiro(string estado)
        {
            switch (estado)
            {
                case GIConstantesSolicitudes.ESTADO_GIRO_ACT:
                    return GIEnumEstadosGirosDC.ACTIVO;
                case GIConstantesSolicitudes.ESTADO_GIRO_PAG:
                    return GIEnumEstadosGirosDC.PAGADO;
                case GIConstantesSolicitudes.ESTADO_GIRO_CUS:
                    return GIEnumEstadosGirosDC.CUSTODIA;
                case GIConstantesSolicitudes.ESTADO_GIRO_BLQ:
                    return GIEnumEstadosGirosDC.BLOQUEADO;
                case GIConstantesSolicitudes.ESTADO_GIRO_ANU:
                    return GIEnumEstadosGirosDC.ANULADO;
                case GIConstantesSolicitudes.ESTADO_GIRO_DEV:
                    return GIEnumEstadosGirosDC.DEVOLUCION;
                default:
                    return GIEnumEstadosGirosDC.REZAGO;
            }
        }

        /// <summary>
        /// Convierte el estado del giro.
        /// </summary>
        /// <param name="estado">valor actual del estado.</param>
        /// <returns>la palabra del Estado</returns>
        public GIEnumEstadosGirosDC ConvertirEstGiroOriginal(string estado)
        {
            switch (estado)
            {
                case GIConstantesSolicitudes.ESTADO_GIRO_ACTIVO:
                    return GIEnumEstadosGirosDC.ACT;
                case GIConstantesSolicitudes.ESTADO_GIRO_PAGADO:
                    return GIEnumEstadosGirosDC.PAG;
                case GIConstantesSolicitudes.ESTADO_GIRO_CUSTODIA:
                    return GIEnumEstadosGirosDC.CUS;
                case GIConstantesSolicitudes.ESTADO_GIRO_BLOQUEADO:
                    return GIEnumEstadosGirosDC.BLQ;
                case GIConstantesSolicitudes.ESTADO_GIRO_ANULADO:
                    return GIEnumEstadosGirosDC.ANU;
                case GIConstantesSolicitudes.ESTADO_GIRO_DEVOLUCION:
                    return GIEnumEstadosGirosDC.DEV;
                default:
                    return GIEnumEstadosGirosDC.REZ;
            }
        }

        #endregion Enumerables

        #endregion Gestion

        /// <summary>
        /// obtiene el valor real de la caja en los puntos
        /// </summary>
        /// <param name="fechaInicial"></param>
        /// <param name="fechaFinal"></param>
        /// <returns></returns>
        internal List<puntoDeAtencion> consultaValorRealPorPuntosDeAtencion()
        {
            return GIRepositorioSolicitudes.Instancia.consultaValorRealPorPuntosDeAtencion();
        }

                /// <summary>
        /// obtiene el valor real de la caja en el punto
        /// </summary>
        /// <returns></returns>
        public List<puntoDeAtencion> consultaValorRealPorPuntoDeAtencion(string idCentroServicio)
        {
            return GIRepositorioSolicitudes.Instancia.consultaValorRealPorPuntoDeAtencion(idCentroServicio);
        }

        /// <summary>
        /// Inserta en la tabla de [AuditoriaIntegraciones_AUD] cada vez que sesa consumido el servicio por 472
        /// </summary>
        public void RegistrarAuditoria472(string tipoIntegracion, string request, string response)
        {
            GIRepositorioSolicitudes.Instancia.RegistrarAuditoria472(tipoIntegracion, request, response);
        }
    }
}