using CO.Servidor.Servicios.ContratoDatos.Comun;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.WebApi.Comun;
using CO.Servidor.Servicios.WebApi.Dominio;
using CO.Servidor.Servicios.WebApi.ModeloResponse.OperacionUrbana;
using CO.Servidor.Servicios.WebApi.ModelosRequest.OperacionUrbana;
using CO.Servidor.Servicios.WebApi.NotificacionesPush;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace CO.Servidor.Servicios.WebApi.Controllers
{
    [RoutePrefix("api/OperacionUrbanaController")]
    ///Clase que expone los servicios REST para el modulo de OperacionUrbana PUA
    public class OperacionUrbanaController : ApiController
    {
        #region Constructor
        public OperacionUrbanaController()
        {

        }
        #endregion

        #region Controller

        /// <summary>
        /// Inserta la relacion entre un dispositivo movil y una recogida
        /// </summary>
        /// <param name="idRecogida"></param>
        /// <param name="idDispositivoMovil"></param>
        /* [HttpPost]
         [Route("RegistrarSolicitudRecogidaMovil")]
         [SeguridadWebApi]
         public void RegistrarSolicitudRecogidaMovil([FromBody]SolicitudRecogidaPushMovilRequest solicitudRecogida)
         {
             ApiOperacionUrbana.Instancia.RegistrarSolicitudRecogidaMovil(solicitudRecogida);

         }*/





        /// <summary>
        /// Asigna una recogida a un mensajero
        /// </summary>
        /// <param name="recogidaMensajero"></param>
        [HttpPost]
        //[Route("ProgramarRecogidaMensajero/{idRecogida}/{idMensajero}")]
        [Route("ProgramarRecogidaMensajero")]
        [SeguridadWebApi]
        public bool ProgramarRecogidaMensajero([FromBody]RecogidaMensajeroRequest recogidaMensajero)
        {
            return ApiOperacionUrbana.Instancia.ProgramarRecogidaMensajero(recogidaMensajero);
        }

        /// <summary>
        /// Obtiene el id del mensajero filtrando por el nombre de usuario
        /// </summary>
        /// <param name="nomUsuario"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerIdMensajeroNomUsuario")]
        public long ObtenerIdMensajeroNomUsuario([FromUri]string nomUsuario)
        {
            return ApiOperacionUrbana.Instancia.ObtenerIdMensajeroNomUsuario(nomUsuario);
        }

        /// <summary>
        /// Obtiene todas las recogidas asignadas a un mensajero en un dia
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerRecogidasMensajerosDia/{idMensajero}")]
        public RecogidasMensajeroResponse ObtenerRecogidasMensajerosDia([FromUri]long idMensajero)
        {
            return ApiOperacionUrbana.Instancia.ObtenerRecogidasMensajerosDia(idMensajero);
        }

        /// <summary>
        /// Obtiene todas las recogidas creadas por un cliente movil en un dia
        /// </summary>
        /// <param name="tokenDispositivo"></param>
        /// <returns></returns>     
        [HttpGet]
        [Route("ObtenerRecogidasClienteMovilDia")]
        public RecogidasClienteMovilResponse ObtenerRecogidasClienteMovilDia([FromUri]string tokenDispositivo)
        {
            return ApiOperacionUrbana.Instancia.ObtenerRecogidasClienteMovilDia(tokenDispositivo);
        }


        /// <summary>
        /// Descarga la recogida de la planillla, teniendo en cuenta el motivo de descargue
        /// </summary>
        /// <param name="recogidaRequest"></param>
        /// <returns></returns>

        [HttpPost]
        [Route("DescargarRecogida")]
        [SeguridadWebApi]
        public bool DescargarRecogida([FromBody]DescargarRecogidaRequest recogidaRequest)
        {
            return ApiOperacionUrbana.Instancia.DescargarRecogida(recogidaRequest);
        }

        /// <summary>
        /// cancela una recogida de peaton
        /// </summary>
        /// <param name="descargue"></param>
        [HttpPost]
        [Route("CancelarRecogidaPeaton")]
        [SeguridadWebApi]
        public void CancelarRecogidaPeaton([FromBody]OUDescargueRecogidaMensajeroDC descargue)
        {
            ApiOperacionUrbana.Instancia.CancelarRecogidaPeaton(descargue);
        }

        /// <summary>
        /// Obtiene todas las recogidas pendientes de peaton del dia actual, filtradas por un rango de tiempo para la hora de recogida y la posicion del mensajero que está consultando
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerRecogidasPeatonPendientesCercanasMensajeroDia")]
        //[SeguridadWebApi]
        public List<OURecogidasDC> ObtenerRecogidasPeatonPendientesCercanasMensajeroDia([FromUri]string longitud, [FromUri]string latitud, [FromUri]string localidad)
        {
            try
            {
                return ApiOperacionUrbana.Instancia.ObtenerRecogidasPeatonPendientesCercanasMensajeroDia(longitud, latitud, localidad);

            }
            catch (Exception ex)
            {
                Util.AuditarExcepcion(ex);
                throw;

            }
        }

        /// <summary>
        /// Obtiene todas las recogidas de peaton pendientes por programar
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerTodasRecogidasPeatonPendientesPorProgramar")]
        //[SeguridadWebApi]
        public List<OURecogidaResponse> ObtenerTodasRecogidasPeatonPendientesPorProgramar()
        {
            return ApiOperacionUrbana.Instancia.ObtenerTodasRecogidasPeatonPendientesPorProgramar();
        }

        /// <summary>
        /// Obtiene todas las recogidas de peaton pendientes por programas
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("ObtenerInfoUsuarioRecogida")]
        //[SeguridadWebApi]
        public PAPersonaInternaDC ObtenerInfoUsuarioRecogida([FromUri]string identificacion)
        {
            return ApiOperacionUrbana.Instancia.ObtenerInfoUsuarioRecogida(identificacion);
        }


        /// <summary>
        /// Retorna los motivos de cancelacion de una recogida
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("ObtenerMotivosCancelacionRecogidas")]
        //[SeguridadWebApi]
        public List<OUMotivoDescargueRecogidasDC> ObtenerMotivosCancelacionRecogidas([FromUri]bool programada)
        {
            return ApiOperacionUrbana.Instancia.ObtenerMotivosCancelacionRecogidas(programada);
        }

        /// <summary>
        /// Obtiene las recogidas del cliente peaton segun el token del dispositivo
        /// </summary>
        /// <param name="tokenDispositivo"></param>
        /// <returns></returns>
        [HttpGet]
        [SeguridadWebApi]
        [Route("ObtenerMisRecogidasClientePeaton")]
        public List<OURecogidasDC> ObtenerMisRecogidasClientePeaton([FromUri]string tokenDispositivo)
        {
            return ApiOperacionUrbana.Instancia.ObtenerMisRecogidasClientePeaton(tokenDispositivo);
        }

        [HttpGet]
        [SeguridadWebApi]
        [Route("ObtenerImagenesSolicitudRecogida")]
        public List<string> ObtenerImagenesSolicitudRecogida([FromUri]long idSolicitudRecogida)
        {
            return ApiOperacionUrbana.Instancia.ObtenerImagenesSolicitudRecogida(idSolicitudRecogida);
        }

        /// <summary>
        /// Metodo para obtener parametro operacion urbana
        /// </summary>
        /// <param name="parametro"></param>
        /// <returns></returns>
        [HttpGet]
        [SeguridadWebApi]
        [Route("ObtenerParametroOperacionUrbana")]
        public string ObtenerParametroOperacionUrbana(string parametro)
        {
            return ApiOperacionUrbana.Instancia.ObtenerParametroOperacionUrbana(parametro);
        }

        /// <summary>
        /// Metodo para obtener la informacion del mensajero
        /// </summary>
        /// <param name="nomUsuario"></param>
        /// <returns></returns>
        [HttpGet]
        [SeguridadWebApi]
        [Route("ObtenerInformacionMensajeroNomUsuarioPAM")]
        public OUMensajeroDC ObtenerInformacionMensajeroNomUsuarioPAM([FromUri] string nomUsuario)
        {
            return ApiOperacionUrbana.Instancia.ObtenerInformacionMensajeroNomUsuarioPAM(nomUsuario);
        }

        /// <summary>
        /// Metodo para consultar las guias planilladas al auditor
        /// </summary>
        /// <param name="idAuditor"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerGuiasAuditor")]
        [SeguridadWebApi]
        public IList<OUGuiaIngresadaDC> ObtenerGuiasAuditor([FromUri]long idAuditor)
        {
            return ApiOperacionUrbana.Instancia.ObtenerGuiasAuditor(idAuditor);
        }

        /// <summary>
        /// Metodo para obtener las guias planilladas por mensajero
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerGuiasMensajero")]
        [SeguridadWebApi]
        public IList<OUGuiaIngresadaDC> ObtenerGuiasMensajero([FromUri]long idMensajero)
        {
            return ApiOperacionUrbana.Instancia.ObtenerGuiasMensajero(idMensajero);

        }

        #region Auditoria Mensajero/Auditor Controller App
        /// <summary>
        /// Metodo para obtener guias planillas en zona
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerGuiasMensajeroEnZona/{idMensajero}")]
        public List<OUGuiaIngresadaAppDC> ObtenerGuiasMensajeroEnZona([FromUri] long idMensajero)
        {
            return ApiOperacionUrbana.Instancia.ObtenerGuiasMensajeroEnZona(idMensajero);
        }

        /// <summary>
        /// Metodo para obtener guias entregadas por mensajero en zona
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerGuiasEntregadasMensajero/{idMensajero}")]
        public List<OUGuiaIngresadaAppDC> ObtenerGuiasEntregadasMensajero([FromUri] long idMensajero)
        {
            return ApiOperacionUrbana.Instancia.ObtenerGuiasEntregadasMensajero(idMensajero);
        }

        /// <summary>
        /// Metodo para obtener las guias de devolución del mensajero 
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerGuiasDevolucionMensajero/{idMensajero}")]
        public List<OUGuiaIngresadaAppDC> ObtenerGuiasDevolucionMensajero([FromUri] long idMensajero)
        {
            return ApiOperacionUrbana.Instancia.ObtenerGuiasDevolucionMensajero(idMensajero);
        }

        /// <summary>
        /// Metodo para obtener las guias en zona auditor
        /// </summary>
        /// <param name="idAuditor"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerGuiasEnZonaAuditor/{idAuditor}")]
        public List<OUGuiaIngresadaAppDC> ObtenerGuiasEnZonaAuditor([FromUri] long idAuditor)
        {
            return ApiOperacionUrbana.Instancia.ObtenerGuiasEnZonaAuditor(idAuditor);

        }

        /// <summary>
        /// Metodo para obtener las guias entregadas por auditor en zona
        /// </summary>
        /// <param name="idAuditor"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerGuiasEntregadasAuditor/{idAuditor}")]
        public List<OUGuiaIngresadaAppDC> ObtenerGuiasEntregadasAuditor([FromUri] long idAuditor)
        {
            return ApiOperacionUrbana.Instancia.ObtenerGuiasEntregadasAuditor(idAuditor);
        }

        /// <summary>
        /// Metodo para obtener las devoluciones por auditor en zona
        /// </summary>
        /// <param name="idAuditor"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerGuiasDevolucionAuditor/{idAuditor}")]
        public List<OUGuiaIngresadaAppDC> ObtenerGuiasDevolucionAuditor([FromUri] long idAuditor)
        {
            return ApiOperacionUrbana.Instancia.ObtenerGuiasDevolucionAuditor(idAuditor);
        }

        /// <summary>
        /// Metodo para obtener la guia planillada para descargue
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerGuiaEnPlanillaDescargue/{numeroGuia}/{idMensajero}")]
        public OUGuiaIngresadaAppDC ObtenerGuiaEnPlanillaDescargue([FromUri] long numeroGuia, [FromUri] long idMensajero)
        {
            return ApiOperacionUrbana.Instancia.ObtenerGuiaEnPlanillaDescargue(numeroGuia, idMensajero);
        }

        [HttpGet]
        [Route("ObtenerGuiaEnPlanillaAuditorDescargue/{numeroGuia}/{idMensajero}")]
        public OUGuiaIngresadaAppDC ObtenerGuiaEnPlanillaAuditorDescargue([FromUri] long numeroGuia, [FromUri] long idMensajero)
        {
            return ApiOperacionUrbana.Instancia.ObtenerGuiaEnPlanillaAuditorDescargue(numeroGuia, idMensajero);
        }

        /// <summary>
        /// Metodo para traer la informacion del usuario (mensajero o auditor)
        /// </summary>
        /// <param name="numIdentificacion"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerInformacionUsuarioControllerApp/{numIdentificacion}")]
        public OUMensajeroDC ObtenerInformacionUsuarioControllerApp([FromUri] string numIdentificacion)
        {
            return ApiOperacionUrbana.Instancia.ObtenerInformacionUsuarioControllerApp(numIdentificacion);
        }


        #region TulasyContenedores

        [HttpGet]
        [Route("ObtenerTiposNovedadGuia/{tipoNovedadRequest}")]
        [SeguridadWebApi]
        public List<COTipoNovedadGuiaDC> ObtenerTiposNovedadGuia(int tipoNovedadRequest)
        {
            var tipoNovedad = (COEnumTipoNovedad)tipoNovedadRequest;
            return ApiOperacionUrbana.Instancia.ObtenerTiposNovedadGuia(tipoNovedad);
        }

        #endregion

        #endregion

        #endregion

        #region Sispostal - Modulo de Masivos

        /// <summary>
        /// Metodo para obtener guias por estado Sispostal 
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <param name="estado"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerGuiasMensajeroEnZonaMasivos/{idMensajero}/{estado}")]
        [SeguridadWebApi]
        public List<OUGuiaIngresadaAppDC> ObtenerGuiasMensajeroEnZonaMasivos([FromUri] long idMensajero, [FromUri] short estado)
        {
            return ApiOperacionUrbana.Instancia.ObtenerGuiasMensajeroEnZonaMasivos(idMensajero, estado);
        }

        #endregion
    }
}

