using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.Threading.Tasks;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Raps.Comun.Integraciones.Datos;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes;
using Framework.Servidor.Comun.Util;
using RestSharp;
using CO.Servidor.Servicios.ContratoDatos.Raps;
using Framework.Servidor.Comun;
using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using Framework.Servidor.MotorReglas;
using CO.Servidor.Raps.Datos;
using Framework.Servidor.Excepciones;

namespace CO.Servidor.Raps.Comun.Integraciones
{
    public class RAIntegracionRaps
    {
        #region singleton
        private static readonly RAIntegracionRaps instancia = new RAIntegracionRaps();

        public static RAIntegracionRaps Instancia
        {
            get
            {
                return instancia;
            }
        }
        #endregion
        #region Fachadas
        private IPUFachadaCentroServicios fachadaCes = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();
        #endregion        

        private RAIntegracionRaps()
        {

        }

        /// <summary>
        /// Crea una solicitud acumulativa
        /// </summary>
        [System.Obsolete()]
        public void CrearSolicitudAcumulativaRaps(EnumTipoNovedadRaps tipoNovedad, Dictionary<string, object> parametrosParametrizacion, string idLocalidad, string usuario)
        {

            Task.Factory.StartNew(() =>
            {
                try
                {
                    EnviarSolicitudAcumulativaRaps(tipoNovedad, parametrosParametrizacion, idLocalidad, usuario);
                }
                catch (Exception ex)
                {

                    string archivo = @"c:\logExcepciones\logExcepciones.txt";
                    FileInfo f = new FileInfo(archivo);
                    StreamWriter writer;
                    if (!f.Exists) // si no existe entonces lo crea, si ya existe NO lo crea
                    {
                        writer = f.CreateText();
                        writer.Close();
                    }

                    writer = f.AppendText();
                    writer.WriteLine("********ERROR INTEGRANDO ***** " + ex.Message);
                    writer.Close();

                    UtilidadesFW.AuditarExcepcion(ex, true);
                }
            });
        }



        /// <summary>
        /// Crea una solicitud acumulativa
        /// </summary>
        [System.Obsolete()]
        private void EnviarSolicitudAcumulativaRaps(EnumTipoNovedadRaps tipoNovedad, Dictionary<string, object> parametrosParametrizacion, string idLocalidad, string usuario)
        {
            string urlApi = ObtenerUrlApi();

            //idSistema = 1 => controller
            var objSignal = new { parametrosParametrizacion = parametrosParametrizacion, idCiudad = idLocalidad, idSistema = 1, idTipoNovedad = tipoNovedad };
            var restClient = new RestClient(urlApi);
            var restRequest = new RestRequest("api/SolicitudesRaps/CrearSolicitudAcumulativa", Method.POST);
            restRequest.AddJsonBody(objSignal);
            restRequest.AddHeader("usuario", usuario);
            restClient.Execute(restRequest);

        }
       
        /// <summary>
        /// Consulta el parametro de la url de controller api
        /// </summary>
        /// <returns></returns>
        public string ObtenerUrlApi()
        {
            return RARepositorioIntegracion.Instancia.ObtenerUrlApi();

        }

        /// <summary>
        /// Obtiene los parametros por tipo de integracion
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        [System.Obsolete()]
        public List<LIParametrizacionIntegracionRAPSDC> ObtenerParametrosPorIntegracion(string tipoParametro)
        {
            return RARepositorioIntegracion.Instancia.ObtenerParametrosPorIntegracion(tipoParametro);
        }
                 

        /// <summary>
        /// Obtiene los parametros por id novedad
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public List<LIParametrizacionIntegracionRAPSDC> ObtenerParametrosPorIntegracionPorNovedad(int IdNovedad, RAEnumOrigenRaps origenRaps)
        {
            return RARepositorioIntegracion.Instancia.ObtenerParametrosPorIntegracionPorNovedad(IdNovedad, origenRaps.GetHashCode());
        }

        /// <summary>
        /// Obtener parametros fallas personalizadas
        /// </summary>
        /// <param name="idTipoNovedad"></param>
        /// <returns></returns>
        public List<LIParametrizacionIntegracionRAPSDC> ListaParametrosPersonalizacionPorNovedad(int idTipoNovedad)
        {
            return RARepositorioIntegracion.Instancia.ListaParametrosPersonalizacionPorNovedad(idTipoNovedad);
        }

        #region Pendientes Agencia
        /// <summary>
        /// Integracion para registrar los raps inmediatos en devolucion agencias
        /// </summary>
        /// <param name="guia"></param>
        public void RegistrarSolicitudRapsInmediata(OUGuiaIngresadaDC guia, string usuario, string identificacion, LIEnumTipoNovedadGuia novedad, long idCentroServicio)
        {
            string urlApi = ObtenerUrlApi();
            RASolicitudDC solicitud = new RASolicitudDC();
            solicitud.IdParametrizacionRap = 0;
            solicitud.Descripcion = guia.Novedad.Descripcion;
            solicitud.IdCiudad = guia.IdCiudad;

            long IdParametrizacionRap = IdentificaParametrizacionPorNovedad(novedad);
            string idciudadCol = fachadaCes.ObtenerCOLResponsable(idCentroServicio).IdMunicipio;

            var destinatario = RARepositorioSolicitudes.Instancia.ObtenerPersonaAsignarRap(IdParametrizacionRap, idciudadCol);

            if(destinatario == null)
            {
                //throw new FaultException(

                //    string.Format("Imposible registrar Solicitud Raps Inmediata validar parametrizacion = {0} en la ciudad codigo = {1}",
                //    IdParametrizacionRap, 
                //    idciudadCol) , new FaultCode("Raps0001")
                //    );

                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.LOGISTICA_INVERSA, "001",
                  string.Format("Imposible registrar Solicitud Raps Inmediata validar parametrizacion = {0} en la ciudad codigo = {1}",
                  IdParametrizacionRap,
                  idciudadCol)));
          
        }                                                            

            var InformacionGestiona = RARepositorioIntegracion.Instancia.ObtenerInformacionUsuarioPorIdentificacion(identificacion);

            EjecutarServicioRest(
                urlApi,
                "api/SolicitudesRaps/CrearSolicitud",
                Method.POST,
                new RASolicitudDC
                {
                    IdParametrizacionRap = IdParametrizacionRap,
                    Descripcion = guia.Novedad.Descripcion,
                    IdCiudad = guia.IdCiudad,
                    DocumentoResponsable = destinatario.CargoEscalar.DocumentoEmpleado,
                    DocumentoSolicita = InformacionGestiona.Identificacion.ToString(),
                    FechaInicio = DateTime.Now,
                    IdCargoSolicita = InformacionGestiona.IdCargo,
                    IdCargoResponsable = destinatario.CargoEscalar.IdCargoNovasoft
                },
                guia.IdCiudad.Substring(0, 5),
                new
                {
                    Comentario = CreaComentarioGestion(guia, novedad, usuario),
                    DocumentoGestiona = InformacionGestiona.Identificacion,
                    IdCargoGestiona = InformacionGestiona.IdCargo,
                    CorreoGestiona = InformacionGestiona.Correo,
                    IdCargoDestino = destinatario.CargoEscalar.IdCargoNovasoft,
                    CorreoDestino = destinatario.CorreoEscalar,
                    DocumentoDestino = destinatario.CargoEscalar.DocumentoEmpleado,
                    IdUsuario = usuario,
                },
                new Dictionary<string, object>(),
                usuario);

        }

        private string CreaComentarioGestion(OUGuiaIngresadaDC guia, LIEnumTipoNovedadGuia novedad, string usuario)
        {
            return String.Format(@"Número de Guía {0}\nId Centro de Servicio registra novedad {1} \nNombre Centro de Servicio registra novedad {2}\nNombre de la falla {3}\nFecha y hora registro de falla {4} \nUsuario quien registra la falla {5}",
                guia.NumeroGuia,
                guia.CiudadDestino.IdCentroServicio,
                guia.CiudadDestino.Nombre,
                Enum.GetName(typeof(LIEnumTipoNovedadGuia), novedad).ToString().Replace('_', ' '),
                DateTime.Now.ToString(),
                usuario
            );
        }

        private long IdentificaParametrizacionPorNovedad(LIEnumTipoNovedadGuia novedad)
        {
            long IdParametrizacionRap = 0;
            switch(novedad)
            {
                case LIEnumTipoNovedadGuia.El_envío_no_llegó:
                    IdParametrizacionRap = 507;

                    break;
                case LIEnumTipoNovedadGuia.El_envío_llegó_averiado:
                    IdParametrizacionRap = 509;
                    break;
                case LIEnumTipoNovedadGuia.El_envío_llegó_saqueado:
                    IdParametrizacionRap = 508;
                    break;
                default:
                    IdParametrizacionRap = 0;
                    break;
            }

            return IdParametrizacionRap;
        }

        private void EjecutarServicioRest(string urlApi,
                            string servicio,
                            Method metodo,
                            RASolicitudDC solicitud,
                            string ciudad,
                            dynamic informacion,
                            Dictionary<string, Object> parametrosParam,
                            string usuario)
        {
            var objJSON = new { Solicitud = solicitud, idCiudad = ciudad, idSistema = 1, InformacionGestion = informacion, parametrosParametrizacion = parametrosParam };
            var restClient = new RestClient(urlApi);
            var restRequest = new RestRequest("api/SolicitudesRaps/RegistrarSolicitud", Method.POST);
            restRequest.AddJsonBody(objJSON);
            restRequest.AddHeader("usuario", usuario);
            restClient.Execute(restRequest);
        }

        /// <summary>
        /// valida es unico envio para parametrización
        /// </summary>
        /// <param name="idTipoNovedad"></param>
        /// <returns></returns>

        public bool ValidarParametrizacionEsUnicoEnvio(int idTipoNovedad)
        {
            return RARepositorioIntegracion.Instancia.ValidarParametrizacionEsUnicoEnvio(idTipoNovedad);
        }

        /// <summary>
        /// valida existe solicitud creada para el responsable
        /// </summary>
        /// <param name="idParametroAgrupamiento"></param>
        /// <param name="valorParametro"></param>
        /// <returns></returns>
        public bool ValidarExisteSolicitudParaResponsable(int idParametroAgrupamiento, string valorParametro)
        {
            return RARepositorioIntegracion.Instancia.ValidarExisteSolicitudParaResponsable(idParametroAgrupamiento, valorParametro);
        }

        #endregion


        #region IntegracionesRapsManuales

        /// <summary>
        /// obtiene parametros po responsable
        /// </summary>
        /// <param name="idResposable"></param>
        /// <param name="idNovedadPadre"></param>
        /// <returns></returns>
        public List<RAParametrosPersonalizacionRapsDC> ObtenerParametrosXIdResponsable(int idResposable, int idNovedadPadre,int Ejecuta)
        {
            return RARepositorioIntegracion.Instancia.ObtenerParametrosXIdResponsable(idResposable, idNovedadPadre,Ejecuta);
        }

        /// <summary>
        /// obtener parametros por id  novedad padre
        /// </summary>
        /// <param name="idResponsable"></param>
        /// <param name="idNovedadPadre"></param>
        /// <returns></returns>
        public List<RAParametrosPersonalizacionRapsDC> ObtenerParametrosVisiblesGlobales(int idNovedadPadre, int idOrigen)
        {
            if (idOrigen == 4)
            {
                return RARepositorioIntegracion.Instancia.ObtenerParametrosVisiblesGlobales(idNovedadPadre);
            }
            else
            {
                return RARepositorioIntegracion.Instancia.ObtenerParametrosXIdResponsable(idOrigen, idNovedadPadre, idOrigen);
            }            
        }

        /// <summary>
        /// obtiene resposables con novedad  por id origen
        /// </summary>
        /// <param name="idOrigen"></param>
        /// <returns></returns>
        public List<RAResponsableTipoNovedadDC> ObtenerResponsableTipoNovedad(int idOrigen)
        {
            return RARepositorioIntegracion.Instancia.ObtenerResponsableTipoNovedad(idOrigen);
        }

     
      
        /// <summary>
        /// obtiene reglas por idestadoguia
        /// </summary>
        /// <param name="idEstadoGuia"></param>
        /// <returns></returns>
        public RAReglasIngrecionesManualDC ObtenerReglasIntegracionesManual(int idEstadoGuia)
        {
            return RARepositorioIntegracion.Instancia.ObtenerReglasIntegracionesManual(idEstadoGuia);
        }

        /// <summary>
        /// obtiene tipo novedad
        /// </summary>
        /// <param name="idNovedadPadre"></param>
        /// <param name="idTipoResponsable"></param>
        /// <returns></returns>
        public int ObtieneTipoNovedad(int idNovedadPadre, int idTipoResponsable)
        {
            return RARepositorioIntegracion.Instancia.ObtieneTipoNovedad(idNovedadPadre, idTipoResponsable);
        }

        ///// <summary>
        ///// valida es unico envio para parametrización
        ///// </summary>
        ///// <param name="idTipoNovedad"></param>
        ///// <returns></returns>
        //private bool ValidarParametrizacionEsUnicoEnvio(int idTipoNovedad)
        //{
        //    return RARepositorioSolicitudes.Instancia.ValidarParametrizacionEsUnicoEnvio(idTipoNovedad);
        //}





        #endregion
    }
}
