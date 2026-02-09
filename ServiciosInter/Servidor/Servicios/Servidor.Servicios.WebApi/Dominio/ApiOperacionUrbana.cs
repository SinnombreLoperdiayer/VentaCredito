
using CO.Servidor.OperacionUrbana.Comun;
using CO.Servidor.Servicios.ContratoDatos.Comun;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion;
using CO.Servidor.Servicios.WebApi.Comun;
using CO.Servidor.Servicios.WebApi.ModeloResponse.OperacionUrbana;
using CO.Servidor.Servicios.WebApi.ModelosRequest.OperacionUrbana;
using CO.Servidor.Servicios.WebApi.NotificacionesPush;
using CO.Servidor.Servicios.WebApi.ProcesosAutomaticos;
using Framework.Servidor.Comun;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CO.Servidor.Servicios.WebApi.Dominio
{
    public class ApiOperacionUrbana : ApiDominioBase
    {

        private static readonly ApiOperacionUrbana instancia = (ApiOperacionUrbana)FabricaInterceptorApi.GetProxy(new ApiOperacionUrbana(), COConstantesModulos.MODULO_OPERACION_URBANA);

        public static ApiOperacionUrbana Instancia
        {
            get { return ApiOperacionUrbana.instancia; }
        }

        private ApiOperacionUrbana()
        {

        }



        /// <summary>
        /// Inserta la relacion entre un dispositivo movil y una recogida
        /// </summary>
        /// <param name="idRecogida"></param>
        /// <param name="idDispositivoMovil"></param>       
        public void RegistrarSolicitudRecogidaMovil(SolicitudRecogidaPushMovilRequest solicitudRecogida)
        {
            PADispositivoMovil dispositivo = FabricaServicios.ServicioParametros.ObtenerDispositivoMovilTokenOs(solicitudRecogida.TokenDispositivo, solicitudRecogida.SistemaOperativo);

            if (dispositivo == null)
            {
                PADispositivoMovil disp = new PADispositivoMovil()
                {
                    IdCiudad = solicitudRecogida.IdLocalidad,
                    SistemaOperativo = solicitudRecogida.SistemaOperativo,
                    TipoDispositivo = PAEnumTiposDispositivos.DPE,
                    TokenDispositivo = solicitudRecogida.TokenDispositivo
                };

                long idDispositivo = FabricaServicios.ServicioParametros.RegistrarDispositivoMovil(disp);

                dispositivo = new PADispositivoMovil()
                {
                    IdDispositivo = idDispositivo
                };
            }

            FabricaServicios.ServicioOperacionUrbana.RegistrarSolicitudRecogidaMovil(solicitudRecogida.IdRecogida, dispositivo.IdDispositivo);
            ///Notifica a los mensajeros que hay una nueva recogida disponible en la ciudad
            //PushGeneral.Instancia.NotificarRecogidaMensajerosMovilPAM(solicitudRecogida.IdLocalidad);

        }





        /// <summary>
        /// Obtiene el id del mensajero filtrando por el nombre de usuario
        /// </summary>
        /// <param name="nomUsuario"></param>
        /// <returns></returns>       
        public long ObtenerIdMensajeroNomUsuario(string nomUsuario)
        {
            return FabricaServicios.ServicioParametrosOperacion.ObtenerIdMensajeroNomUsuario(nomUsuario);
        }


        /// <summary>
        /// Obtiene todas las recogidas asignadas a un mensajero en un dia
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>     
        public RecogidasMensajeroResponse ObtenerRecogidasMensajerosDia(long idMensajero)
        {

            List<OURecogidasDC> lstPendientes = new List<OURecogidasDC>();
            List<OURecogidasDC> lstEfectuadas = new List<OURecogidasDC>();
            List<OURecogidasDC> lstCanceladasMensajero = new List<OURecogidasDC>();
            List<OURecogidasDC> lstCanceladasCliente = new List<OURecogidasDC>();
            List<OURecogidasDC> lstVencidas = new List<OURecogidasDC>();


            List<OURecogidasDC> recogidas = FabricaServicios.ServicioOperacionUrbana.ObtenerRecogidasMensajerosDia(idMensajero);


            lstCanceladasCliente = recogidas.Where(r => r.MotivoDescargue != null && r.MotivoDescargue.IdMotivo == (int)OUEnumMotivoDescargueRecogida.CANCELADA_POR_CLIENTE).ToList();
            lstCanceladasMensajero = recogidas.Where(r => r.MotivoDescargue != null && r.MotivoDescargue.VisibleMensajero && r.MotivoDescargue.IdMotivo != (short)OUEnumMotivoDescargueRecogida.RECOGIDA_REALIZADA).ToList();
            lstEfectuadas = recogidas.Where(r => r.MotivoDescargue != null && r.MotivoDescargue.IdMotivo == (int)OUEnumMotivoDescargueRecogida.RECOGIDA_REALIZADA).ToList();
            lstVencidas = recogidas.Where(r => r.MotivoDescargue != null && r.MotivoDescargue.IdMotivo == (int)OUEnumMotivoDescargueRecogida.VENCIDA).ToList();
            //lstPendientes = recogidas.Where(r => r.EstadoRecogida.IdEstado == 1).ToList();
            lstPendientes = recogidas.Where(r => r.EstadoRecogida.IdEstado == (short)OUEnumEstadoSolicitudRecogidas.IN_PROGRAMADA && r.MotivoDescargue == null).ToList();

            RecogidasMensajeroResponse retorno = new RecogidasMensajeroResponse();
            retorno.LstCanceladasCliente = lstCanceladasCliente;
            retorno.LstCanceladasMensajero = lstCanceladasMensajero;
            retorno.LstEfectuadas = lstEfectuadas;
            retorno.LstPendientes = lstPendientes;
            retorno.LstVencidas = lstVencidas;

            return retorno;
        }



        /// <summary>
        /// Obtiene todas las recogidas creadas por un cliente movil en un dia
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>     
        public RecogidasClienteMovilResponse ObtenerRecogidasClienteMovilDia(string tokenDispositivo)
        {


            List<OURecogidasDC> lstEfectuadas = new List<OURecogidasDC>();
            List<OURecogidasDC> lstCanceladasCliente = new List<OURecogidasDC>();
            List<OURecogidasDC> lstProgramadas = new List<OURecogidasDC>();
            List<OURecogidasDC> lstPendientesPorProgramar = new List<OURecogidasDC>();
            List<OURecogidasDC> lstPendientesPorReProgramar = new List<OURecogidasDC>();



            List<OURecogidasDC> recogidas = FabricaServicios.ServicioOperacionUrbana.ObtenerRecogidasClienteMovilDia(tokenDispositivo);



            lstCanceladasCliente = recogidas.Where(r => r.MotivoDescargue != null && r.MotivoDescargue.IdMotivo == (short)OUEnumMotivoDescargueRecogida.CANCELADA_POR_CLIENTE).ToList();
            //lstEfectuadas = recogidas.Where(r => r.EstadoRecogida.IdEstado != 1 && r.EstadoRecogida.IdEstado != 2 && r.MotivoDescargue != null && !r.MotivoDescargue.PermiteReprogramar).ToList();//&& r.MotivoDescargue.IdMotivo == (short)MotivoDescargueSolicitud.RecogidaRealizada).ToList();            
            lstEfectuadas = recogidas.Where(r => r.EstadoRecogida.IdEstado != (short)OUEnumEstadoSolicitudRecogidas.IN_PROGRAMADA
                && r.EstadoRecogida.IdEstado != (short)OUEnumEstadoSolicitudRecogidas.IN_PENDIENTE_PROGRAMAR
                && r.MotivoDescargue != null
                && !r.MotivoDescargue.PermiteReprogramar).ToList();//&& r.MotivoDescargue.IdMotivo == (short)MotivoDescargueSolicitud.RecogidaRealizada).ToList();            
            //lstProgramadas = recogidas.Where(r => r.EstadoRecogida.IdEstado == 1).ToList();
            lstProgramadas = recogidas.Where(r => r.EstadoRecogida.IdEstado == (short)OUEnumEstadoSolicitudRecogidas.IN_PROGRAMADA).ToList();
            //lstPendientesPorProgramar = recogidas.Where(r => r.EstadoRecogida.IdEstado == 2).ToList();
            lstPendientesPorProgramar = recogidas.Where(r => r.EstadoRecogida.IdEstado == (short)OUEnumEstadoSolicitudRecogidas.IN_PENDIENTE_PROGRAMAR).ToList();
            lstPendientesPorReProgramar = recogidas.Where(r => r.MotivoDescargue != null && r.MotivoDescargue.PermiteReprogramar).ToList();

            RecogidasClienteMovilResponse retorno = new RecogidasClienteMovilResponse();
            retorno.LstCanceladasCliente = lstCanceladasCliente;
            retorno.LstEfectuadas = lstEfectuadas;
            retorno.LstPendientesPorProgramar = lstPendientesPorProgramar;
            retorno.LstProgramadas = lstProgramadas;
            retorno.LstPendientesPorReProgramar = lstPendientesPorReProgramar;

            return retorno;
        }


        /// <summary>
        /// Descarga la recogida de la planillla, teniendo en cuenta el motivo de descargue
        /// </summary>
        /// <param name="recogidaRequest"></param>
        /// <returns></returns>
        public bool DescargarRecogida(DescargarRecogidaRequest recogidaRequest)
        {

            if (recogidaRequest.IdMotivo == 7)
            {
                recogidaRequest.DescripcionMotivo = "RECOGIDA REALIZADA";
            }

            OUDescargueRecogidaMensajeroDC descargue = new OUDescargueRecogidaMensajeroDC()
            {
                IdAsignacion = recogidaRequest.IdAsignacion,
                IdRecogida = recogidaRequest.IdRecogida,
                MotivoDescargue = new OUMotivoDescargueRecogidasDC()
                {
                    IdMotivo = recogidaRequest.IdMotivo,
                    PermiteReprogramar = recogidaRequest.MotivoPermiteReprogramar,
                    DescripcionMotivo = recogidaRequest.DescripcionMotivo,
                },
                Novedad = recogidaRequest.Observaciones
            };

            FabricaServicios.ServicioOperacionUrbana.GuardarDescargueRecogidaPeaton(descargue);

            if (!recogidaRequest.MotivoPermiteReprogramar)
            {
                OUAsignacionRecogidaMensajeroDC asignacion = new OUAsignacionRecogidaMensajeroDC()
                {
                    IdSolicitudRecogida = recogidaRequest.IdRecogida
                };

                NotificarClienteRecogidaDescargueRecogida(asignacion, recogidaRequest.IdMotivo);
            }

            return true;

        }
        /// <summary>
        /// Metodo para obtener guias mensajero en zona 
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public List<OUGuiaIngresadaAppDC> ObtenerGuiasMensajeroEnZona(long idMensajero)
        {
            return FabricaServicios.ServicioOperacionUrbana.ObtenerGuiasMensajeroEnZona(idMensajero);
        }

        /// <summary>
        /// Metodo para obtener las guias entregadas en planilla de mensajero 
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public List<OUGuiaIngresadaAppDC> ObtenerGuiasEntregadasMensajero(long idMensajero)
        {
            return FabricaServicios.ServicioOperacionUrbana.ObtenerGuiasEntregadasMensajero(idMensajero);
        }

        /// <summary>
        /// Metodo para consultar las guias en devolucion del mensajero 
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public List<OUGuiaIngresadaAppDC> ObtenerGuiasDevolucionMensajero(long idMensajero)
        {
            return FabricaServicios.ServicioOperacionUrbana.ObtenerGuiasDevolucionMensajero(idMensajero);
        }

        /// <summary>
        /// Metodo para obtener las guias en zona del auditor
        /// </summary>
        /// <param name="idAuditor"></param>
        /// <returns></returns>
        public List<OUGuiaIngresadaAppDC> ObtenerGuiasEnZonaAuditor(long idAuditor)
        {
            //Guias planilladas para auditoria
            List<OUGuiaIngresadaAppDC> guiasEnZonaAuditor = FabricaServicios.ServicioOperacionUrbana.ObtenerGuiasEnZonaAuditor(idAuditor);

            //Intentos de entrega por numero de guia
            guiasEnZonaAuditor.ForEach(e =>
           {
               //e.IntentosEntrega = LIAdministradorMovil.Instancia.ConsultarIntentosEntregaPorNumeroGuia(e);
           });

            return guiasEnZonaAuditor;
        }

        /// <summary>
        /// Metodo para obtener las guias entregadas por auditor en zona
        /// </summary>
        /// <param name="idAuditor"></param>
        /// <returns></returns>
        public List<OUGuiaIngresadaAppDC> ObtenerGuiasEntregadasAuditor(long idAuditor)
        {
            return FabricaServicios.ServicioOperacionUrbana.ObtenerGuiasEntregadasAuditor(idAuditor);
        }

        /// <summary>
        /// Metodo para obtener las guias devolucion del auditor
        /// </summary>
        /// <param name="idAuditor"></param>
        /// <returns></returns>
        public List<OUGuiaIngresadaAppDC> ObtenerGuiasDevolucionAuditor(long idAuditor)
        {
            return FabricaServicios.ServicioOperacionUrbana.ObtenerGuiasDevolucionAuditor(idAuditor);
        }

        /// <summary>
        /// Metodo para obtener la guia planillada para descargue controller app
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public OUGuiaIngresadaAppDC ObtenerGuiaEnPlanillaDescargue(long numeroGuia, long idMensajero)
        {
            return FabricaServicios.ServicioOperacionUrbana.ObtenerGuiaEnPlanillaDescargue(numeroGuia, idMensajero);
        }

        /// <summary>
        /// Metodo para traer la guia planilla al auditor para descargar con controller app
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public OUGuiaIngresadaAppDC ObtenerGuiaEnPlanillaAuditorDescargue(long numeroGuia, long idMensajero)
        {
            return FabricaServicios.ServicioOperacionUrbana.ObtenerGuiaEnPlanillaAuditorDescargue(numeroGuia, idMensajero);
        }

        /// <summary>
        /// cancela una recogida de peaton
        /// </summary>
        /// <param name="descargue"></param>
        public void CancelarRecogidaPeaton(OUDescargueRecogidaMensajeroDC descargue)
        {
            FabricaServicios.ServicioOperacionUrbana.CancelarRecogidaPeaton(descargue);
        }




        /// <summary>
        /// Asigna una recogida a un mensajero
        /// </summary>
        /// <param name="recogidaMensajero"></param>
        /// <returns></returns>
        public bool ProgramarRecogidaMensajero(RecogidaMensajeroRequest recogidaMensajero)
        {
            lock (this)
            {
                OURecogidasDC recogida = FabricaServicios.ServicioOperacionUrbana.ObtenerRecogidaPeaton(recogidaMensajero.IdRecogida);
                if (recogida == null)
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized) { Content = new StringContent("Recogida no existe") });

                if (recogida.EstadoRecogida.IdEstado != (short)OUEnumEstadoSolicitudRecogidas.IN_PROGRAMADA)
                {
                    OUMensajeroDC mensajero = FabricaServicios.ServicioParametrosOperacion.ObtenerMensajeroIdMensajeroPAM(recogidaMensajero.IdMensajero);
                    if (mensajero == null)
                        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized) { Content = new StringContent("Mensajero no existe para la asignación de la recogida.") });

                    OUAsignacionRecogidaMensajeroDC asignacion = new OUAsignacionRecogidaMensajeroDC()
                    {
                        IdMensajero = mensajero.IdMensajero,
                        IdSolicitudRecogida = recogidaMensajero.IdRecogida,
                        DireccionRecogida = recogida.Direccion,
                        IdLocalidadRecogida = recogida.LocalidadRecogida.IdLocalidad,
                        NumeroDocumentoCliente = recogida.RecogidaPeaton.DocumentoCliente,
                        NombreApellidoMensajero = mensajero.NombreCompleto,
                        IdentificacionMensajero = mensajero.PersonaInterna.Identificacion
                    };

                    asignacion.IdAsignacion = FabricaServicios.ServicioOperacionUrbana.AsignarRecogidaMensajero(asignacion);
                    NotificarClienteRecogidaAceptadaPorMensajero(asignacion);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


        /// <summary>
        /// Notifica al cliente que solicitó la recogia, los datos del mensajero que la realizará
        /// </summary>
        /// <param name="programacionRecogida"></param>
        private void NotificarClienteRecogidaAceptadaPorMensajero(OUAsignacionRecogidaMensajeroDC asignacion)
        {
            try
            {
                POVehiculo vehiculo = FabricaServicios.ServicioParametrosOperacion.ObtenerVehiculoMensajero(asignacion.IdMensajero);

                if (vehiculo == null)
                {
                    vehiculo = new POVehiculo()
                    {
                        Placa = "-"
                    };
                }

                PADispositivoMovil dispositivo = FabricaServicios.ServicioOperacionUrbana.ObtenerdispositivoMovilClienteRecogida(asignacion.IdSolicitudRecogida);
                string mensaje = "{\"title\":\"Recogida Confirmada\",\"message\":\"Su recogida fue confirmada.\", \"nombreMensajero\": \"" + asignacion.NombreApellidoMensajero
                    + "\",\"cedula\":\"" + asignacion.IdentificacionMensajero
                    + "\",\"Placa\":\"" + vehiculo.Placa
                    + "\",\"codigoSeg\":\"" + asignacion.NumeroDocumentoCliente.Substring(asignacion.NumeroDocumentoCliente.Length - 2, 2)
                    + "\",\"Direccion\":\"" + asignacion.DireccionRecogida
                    + "\",\"tiempoRecogida\":\"1 hora\",\"IdRecogida\":\"" + asignacion.IdSolicitudRecogida
                    + "\",\"IdProgramacionSolicitudRecogida\":\"" + asignacion.IdAsignacion
                    + "\",\"IdCiudad\":\"" + asignacion.IdLocalidadRecogida + "\" }";
                PushGeneral.Instancia.NotificarRecogidaClienteRecogidasMovil(dispositivo.TokenDispositivo, mensaje, dispositivo.SistemaOperativo);
            }
            catch
            {
            }

        }

        /// <summary>
        /// Notificacion para los clientes cuando se realice el descargue de la solicitud de recogida.
        /// </summary>
        /// <param name="asignacion"></param>
        private void NotificarClienteRecogidaDescargueRecogida(OUAsignacionRecogidaMensajeroDC asignacion, int idMotivo)
        {
            string mensaje = string.Empty;
            try
            {
                PADispositivoMovil dispositivo = FabricaServicios.ServicioOperacionUrbana.ObtenerdispositivoMovilClienteRecogida(asignacion.IdSolicitudRecogida);
                //TODO: Exitosa
                if (idMotivo == 7)
                {
                    mensaje = "{\"title\":\"Su recogida ha finalizado!\",\"message\":\"Gracias por preferirnos. Lo invitamos a calificar nuestro servicio.\", \"idMotivo \" : \"" + idMotivo + "\" }";
                }
                //TODO: Cancelada 
                else if (idMotivo == 1)
                {
                    mensaje = "{\"title\":\"Su recogida ha sido cancelada!\",\"message\":\"En breve será asignada a uno de nuestros mensajeros.\" , \"idMotivo \" : \"" + idMotivo + "\" }";
                }
                PushGeneral.Instancia.NotificarRecogidaClienteRecogidasMovil(dispositivo.TokenDispositivo, mensaje, dispositivo.SistemaOperativo);
            }
            catch
            {

            }
        }

        /// <summary>
        /// Obtiene todas las recogidas pendientes de peaton del dia actual, filtradas por un rango de tiempo para la hora de recogida
        /// metodo utilizado en el proceso automatico que envia las notificaciones a los pam
        /// </summary>
        /// <returns></returns>
        public List<OURecogidasDC> ObtenerRecogidasPeatonPendientesDia()
        {
            return FabricaServicios.ServicioOperacionUrbana.ObtenerRecogidasPeatonPendientesDia();
        }



        /// <summary>
        /// Obtiene todas las recogidas pendientes de peaton del dia actual, filtradas por un rango de tiempo para la hora de recogida y la posicion del mensajero que está consultando
        /// </summary>
        /// <returns></returns>
        public List<OURecogidasDC> ObtenerRecogidasPeatonPendientesCercanasMensajeroDia(string longitud, string latitud, string localidad)
        {

            if (!string.IsNullOrWhiteSpace(longitud))
                longitud = longitud.Replace('.', Convert.ToChar(CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator));
            else
                longitud = "0";
            if (!string.IsNullOrWhiteSpace(latitud))
                latitud = latitud.Replace('.', Convert.ToChar(CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator));
            else
                latitud = "0";
            //TODO: Alejandro, Cambio de metodo para listar recogidas disponibles
            //List<OURecogidasDC> recogidasPendientes = FabricaServicios.ServicioOperacionUrbana.ObtenerRecogidasDisponiblesPeatonDia(localidad);
            List<OURecogidasDC> recogidasPendientes = FabricaServicios.ServicioOperacionUrbana.ObtenerRecogidasPeatonPendientesPorProgramarDia(localidad);
            List<OURecogidasDC> recogidasPendientesCercanas = new List<OURecogidasDC>();

            int radioCiudad = FabricaServicios.ServicioParametros.ObtenerRadioBusquedaRecogidaLocalidad(localidad);

            recogidasPendientes.ForEach(r =>
            {
                if (string.IsNullOrWhiteSpace(r.LongitudRecogida))
                    r.LongitudRecogida = "0";
                if (string.IsNullOrWhiteSpace(r.LatitudRecogida))
                    r.LatitudRecogida = "0";
            });


            //Sin coordenadas
            recogidasPendientesCercanas.AddRange(recogidasPendientes.Where(r => r.LongitudRecogida == "0"));

            recogidasPendientes.Where(r => r.LongitudRecogida != "0").ToList().ForEach(r =>
            {
                r.LongitudRecogida = r.LongitudRecogida.Replace('.', Convert.ToChar(CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator));
                r.LatitudRecogida = r.LatitudRecogida.Replace('.', Convert.ToChar(CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator));

                if (radioCiudad >= Util.CalcularDistanciaMetros(Convert.ToDouble(latitud), Convert.ToDouble(longitud), Convert.ToDouble(r.LatitudRecogida), Convert.ToDouble(r.LongitudRecogida)))
                {
                    recogidasPendientesCercanas.Add(r);
                }

            });


            return recogidasPendientesCercanas.OrderBy(r => r.IdRecogida).ToList();
        }


        /// <summary>
        /// Obtiene todas las recogidas de peaton pendientes por programas
        /// </summary>
        /// <returns></returns>
        public List<OURecogidaResponse> ObtenerTodasRecogidasPeatonPendientesPorProgramar()
        {

            List<OURecogidasDC> recogidas = FabricaServicios.ServicioOperacionUrbana.ObtenerTodasRecogidasPeatonPendientesPorProgramar();

            List<OURecogidaResponse> recoResponse = recogidas.ConvertAll<OURecogidaResponse>(r =>
            {
                OURecogidaResponse reco = new OURecogidaResponse()
                {
                    CantidadEnvios = r.CantidadEnvios.Value,
                    ComplementoDireccion = r.ComplementoDireccion,
                    Contacto = r.Contacto,
                    Direccion = r.Direccion,
                    DispositivoMovil = r.DispositivoMovil,
                    EstadoRecogida = r.EstadoRecogida,
                    FechaRecogida = r.FechaRecogida,
                    Fotografias = r.Fotografias,
                    IdRecogida = r.IdRecogida.Value,
                    LatitudRecogida = r.LatitudRecogida,
                    LocalidadRecogida = r.LocalidadRecogida,
                    LongitudRecogida = r.LongitudRecogida,
                    MinutosTranscurridos = r.MinutosTranscurridos,
                    NombreCliente = r.NombreCliente,
                    PersonaSolicita = r.PersonaSolicita,
                    PesoAproximado = r.PesoAproximado.Value,
                    RecogidaPeaton = r.RecogidaPeaton,
                    TipoOrigenRecogida = r.TipoOrigenRecogida,
                    AsignacionMensajero = r.AsignacionMensajero != null ? new OUAsignacionRecogidaMensajeroDC()
                    {
                        IdAsignacion = r.AsignacionMensajero.IdAsignacion,
                        IdentificacionMensajero = r.AsignacionMensajero.IdentificacionMensajero,
                        NombreApellidoMensajero = r.AsignacionMensajero.NombreApellidoMensajero,
                        Telefono = r.AsignacionMensajero.Telefono,
                        Usuario = r.AsignacionMensajero.Usuario,
                        PlacaVehiculo = r.AsignacionMensajero.PlacaVehiculo,
                        TipoVehiculo = r.AsignacionMensajero.TipoVehiculo
                    } : new OUAsignacionRecogidaMensajeroDC()

                };

                return reco;
            });

            return recoResponse;
        }

        /// <summary>
        /// Obtiene los parametros de la operacion urbana
        /// </summary>
        /// <param name="idParametro"></param>
        /// <returns></returns>
        public string ObtenerParametroOperacionUrbana(string idParametro)
        {
            return FabricaServicios.ServicioOperacionUrbana.ObtenerValorParametro(idParametro);
        }

        /// <summary>
        /// Guarda las notificaciones enviadas por cada recogida
        /// </summary>
        /// <param name="idSolicitudRecogida"></param>
        public void GuardarNotificacionRecogida(long idSolicitudRecogida)
        {
            FabricaServicios.ServicioOperacionUrbana.GuardarNotificacionRecogida(idSolicitudRecogida);
        }

        /// <summary> 
        /// Obtiene los datos del usuario que está solicitando 
        /// la recogida si ya se ha registrado antes.
        /// </summary>
        /// <param name="tipoid"></param>
        /// <param name="identificacion"></param>
        /// <returns></returns>
        public PAPersonaInternaDC ObtenerInfoUsuarioRecogida(string identificacion)
        {
            string tipoid = identificacion.Split(';')[0];
            identificacion = identificacion.Split(';')[1];

            return FabricaServicios.ServicioOperacionUrbana.ObtenerInfoUsuarioRecogida(tipoid, identificacion);
        }

        /// <summary>
        /// Actualiza el estado de la solicitud de uan recogida e inserta el estado traza
        /// </summary>
        public void ActualizarEstadoSolicitudRecogida(long idRecogida, OUEnumEstadoSolicitudRecogidas estado)
        {
            FabricaServicios.ServicioOperacionUrbana.ActualizarEstadoSolicitudRecogida(idRecogida, estado);
        }

        /// <summary>
        /// Retorna los motivos para cancelar una recogida
        /// </summary>
        /// <returns></returns>
        public List<OUMotivoDescargueRecogidasDC> ObtenerMotivosCancelacionRecogidas(bool programada)
        {
            List<OUMotivoDescargueRecogidasDC> lstMotivos = FabricaServicios.ServicioOperacionUrbana.ObtenerMotivosDescargueRecogidas();

            lstMotivos = lstMotivos.Where(m => m.IdMotivo != (int)OUEnumMotivoDescargueRecogida.RECOGIDA_REALIZADA &&
                m.IdMotivo != (int)OUEnumMotivoDescargueRecogida.CANCELADA_POR_MENSAJERO && m.IdMotivo != (int)OUEnumMotivoDescargueRecogida.VENCIDA).ToList();

            if (!programada)
                lstMotivos = lstMotivos.Where(m => m.IdMotivo == (int)OUEnumMotivoDescargueRecogida.CANCELADA ||
                    m.IdMotivo == (int)OUEnumMotivoDescargueRecogida.CANCELADA_POR_CLIENTE).ToList();


            return lstMotivos;
        }

        /// <summary>
        /// Obtiene las recogidas segun el token del dispositivo del cliente peaton
        /// </summary>
        /// <param name="tokenDispositivo"></param>
        /// <returns></returns>
        public List<OURecogidasDC> ObtenerMisRecogidasClientePeaton(string tokenDispositivo)
        {
            return FabricaServicios.ServicioOperacionUrbana.ObtenerMisRecogidasClientePeaton(tokenDispositivo);
        }

        /// <summary>
        /// Obtiene las imagenes de la solicitud recogida
        /// </summary>
        /// <param name="idSolicitudRecogida"></param>
        /// <returns></returns>
        public List<string> ObtenerImagenesSolicitudRecogida(long idSolicitudRecogida)
        {
            return FabricaServicios.ServicioOperacionUrbana.ObtenerImagenesSolicitudRecogida(idSolicitudRecogida);
        }


        /// <summary>
        /// Obtiene toda la informacion del mensajero
        /// </summary>
        /// <param name="nomUsuario"></param>
        /// <returns></returns>
        public OUMensajeroDC ObtenerInformacionMensajeroNomUsuarioPAM(string nomUsuario)
        {
            return FabricaServicios.ServicioParametrosOperacion.ObtenerInformacionMensajeroNomUsuarioPAM(nomUsuario);
        }

        /// <summary>
        /// Metodo que obtiene las guias planilladas por auditor
        /// </summary>
        /// <param name="idAuditor"></param>
        /// <returns></returns>
        public IList<OUGuiaIngresadaDC> ObtenerGuiasAuditor(long idAuditor)
        {
            return FabricaServicios.ServicioOperacionUrbana.ObtenerGuiasAuditor(idAuditor);
        }

        /// <summary>
        /// Metodo que obtiene las guias planilladas para mensajero
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public IList<OUGuiaIngresadaDC> ObtenerGuiasMensajero(long idMensajero)
        {
            return FabricaServicios.ServicioOperacionUrbana.ObtenerGuiasMensajero(idMensajero);
        }

        /// <summary>
        /// Metodo para obtener la informacion del mensajero / auditor 
        /// </summary>
        /// <param name="numIdentificacion"></param>
        /// <returns></returns>
        public OUMensajeroDC ObtenerInformacionUsuarioControllerApp(string numIdentificacion)
        {
            return FabricaServicios.ServicioOperacionUrbana.ObtenerInformacionUsuarioControllerApp(numIdentificacion);
        }

        #region TulasyContenedores

        public List<COTipoNovedadGuiaDC> ObtenerTiposNovedadGuia(COEnumTipoNovedad tipoNovedad)
        {
            return FabricaServicios.ServicioOperacionUrbana.ObtenerTiposNovedadGuia(tipoNovedad);
        }

        #endregion

        #region Sispostal - Modulo de Masivos

        /// <summary>
        /// Metodo para obtener guias por estado Sispostal
        /// </summary>
        /// <param name="idMensajero"></param>
        /// /// <param name="estado"></param>pa
        /// <returns></returns>
        public List<OUGuiaIngresadaAppDC> ObtenerGuiasMensajeroEnZonaMasivos(long idMensajero, short estado)
        {
            return FabricaServicios.ServicioOperacionUrbana.ObtenerGuiasMensajeroEnZonaMasivos(idMensajero, estado);
        }

        #endregion
    }

}
