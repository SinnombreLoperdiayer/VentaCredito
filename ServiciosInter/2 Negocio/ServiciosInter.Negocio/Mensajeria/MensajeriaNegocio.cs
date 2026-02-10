using iTextSharp.text;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using RestSharp;
using ServiciosInter.DatosCompartidos.Comun;
using ServiciosInter.DatosCompartidos.EntidadesNegocio.CentrosServicio;
using ServiciosInter.DatosCompartidos.EntidadesNegocio.Clientes;
using ServiciosInter.DatosCompartidos.EntidadesNegocio.Comun;
using ServiciosInter.DatosCompartidos.EntidadesNegocio.Facturacion;
using ServiciosInter.DatosCompartidos.EntidadesNegocio.Giros;
using ServiciosInter.DatosCompartidos.EntidadesNegocio.Integraciones;
using ServiciosInter.DatosCompartidos.EntidadesNegocio.LogisticaInversa;
using ServiciosInter.DatosCompartidos.EntidadesNegocio.Mensajeria;
using ServiciosInter.DatosCompartidos.EntidadesNegocio.OperacionNacional;
using ServiciosInter.DatosCompartidos.EntidadesNegocio.Parametros;
using ServiciosInter.DatosCompartidos.EntidadesNegocio.ParametrosOperacion;
using ServiciosInter.DatosCompartidos.EntidadesNegocio.Tarifas;
using ServiciosInter.DatosCompartidos.Wrappers.Preenvios;
using ServiciosInter.Infraestructura.AccesoDatos.Repository.Mensajeria;
using ServiciosInter.Infraestructura.AccesoDatos.Repository.Tarifas;
using ServiciosInter.Integraciones;
using ServiciosInter.Negocio.Parametros;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Web;
using WPF.Comun.EntidadesNegocio.ServiciosInter;

namespace ServiciosInter.Negocio.Mensajeria
{
    public class MensajeriaNegocio
    {
        private static readonly MensajeriaNegocio instancia = new MensajeriaNegocio();//(ApiAdmisionMensajeria)FabricaInterceptorApi.GetProxy(new ApiAdmisionMensajeria(), COConstantesModulos.MENSAJERIA);
        private const string strCentroLogisticoOrigen = "En Centro Logístico";
        public Dictionary<int, string> mensajesEstadoDos = new Dictionary<int, string>();

        public static MensajeriaNegocio Instancia
        {
            get { return MensajeriaNegocio.instancia; }
        }

        public HashSet<ADRangoTrayecto> TrayectosCasillero
        {
            get;
            set;
        }

        public Dictionary<int, string> MensajesEstadoDos
        {
            get
            {
                if (mensajesEstadoDos.Count == 0)
                {
                    mensajesEstadoDos = MensajeriaRepository.Instancia
                        .ObtenerMensajesEstadoDos()
                        .ToDictionary(m => m.IdMensaje, m => m.Mensaje);
                }
                return mensajesEstadoDos;
            }
        }

        private MensajeriaNegocio()
        {
        }

        /// <summary>
        /// Metodo para obtener el rastreo de las guias solicitadas
        /// </summary>
        /// <param name="guias"></param>
        /// <returns></returns>
        public List<ADRastreoGuiaDC> ObtenerRastreoGuias(string guiasAConsultar)
        {
            List<ADRastreoGuiaDC> LstRastreoGuias = new List<ADRastreoGuiaDC>();
            List<long> lstGuias = new List<long>();

            bool existeGuia;

            List<long> lstGuiasVal = guiasAConsultar.Split(',').ToList().ConvertAll<long>(l =>
            {
                long n = 0;
                long.TryParse(l, out n);
                return n;
            });

            lstGuiasVal.ForEach(f =>
            {
                if (f.ToString().Length > 12 && f.ToString().StartsWith("8"))
                {
                    lstGuias.Add(f);
                }
                else
                {
                    existeGuia = MensajeriaRepository.Instancia.VerificarSiGuiaExiste(f);

                    if (existeGuia)
                    {
                        lstGuias.Add(f);
                    }
                }
            });

            lstGuias.ForEach(e =>
            {
                if (e != 0)
                {
                    ADRastreoGuiaDC rastreoGuia = new ADRastreoGuiaDC();

                    #region Sispostal

                    if (e.ToString().Length > 12 && e.ToString().StartsWith("8"))
                    {
                        // Es guía sispostal
                        rastreoGuia.EsSispostal = true;
                        //Ultima gestion del envío
                        rastreoGuia.TrazaGuia = MensajeriaRepository.Instancia.ObtenerTrazaUltimoEstadoXNumGuiaSispostal(e);

                        //Estados del envío
                        List<INEstadosSWSispostal> EstadosSisPostal = null;

                        if (rastreoGuia.TrazaGuia.IdEstadoGuia != (short)INEnumSispostalGuia.ANULADA
                        && rastreoGuia.TrazaGuia.IdEstadoGuia != (short)INEnumSispostalGuia.RETENCION
                        && rastreoGuia.TrazaGuia.IdEstadoGuia != (short)INEnumSispostalGuia.GUIASINFISICO)
                        {
                            EstadosSisPostal = IntegracionSisPostal.Instancia.ObtenerEstadosGuiaSispostal(e);

                        }
                        if (EstadosSisPostal != null)
                        {
                            List<ADEstadoGuiaMotivoDC> estadosMotivoGuia = new List<ADEstadoGuiaMotivoDC>();
                            foreach (var item in EstadosSisPostal)
                            {
                                //Causales GUIA SIN FISICO, ANULADO, SINIESTRO, RETENCION NO DEBEN SER VISIBLES
                                if (!item.Estado.ToUpper().Contains("GUIA SIN FISICO") && !item.Estado.ToUpper().Contains("ANULADO") && !item.Estado.ToUpper().Contains("RETENCION"))
                                {
                                    estadosMotivoGuia.Add(new ADEstadoGuiaMotivoDC
                                    {
                                        IdTrazaGuia = long.Parse(item.Guia),
                                        EstadoGuia = new ADTrazaGuia
                                        {
                                            DescripcionEstadoGuia = item.Estado,
                                            NumeroGuia = long.Parse(item.Guia),
                                            Ciudad = item.Ciudad,
                                            FechaGrabacion = item.Fecha
                                        }
                                    });
                                }
                            }
                            rastreoGuia.EstadosGuia = estadosMotivoGuia;
                        }

                        //Remitente - Destinatario (Información de la guía)
                        rastreoGuia.Guia = MensajeriaRepository.Instancia.ObtenerGuiaSispostalXNumeroGuia(e);

                        //Imagen guía solo para entregas
                        if (rastreoGuia.EstadosGuia != null)
                        {
                            foreach (ADEstadoGuiaMotivoDC a in rastreoGuia.EstadosGuia)
                            {
                                // Si la guía se encuentra entregada, se consulta la imagen 
                                if (a.EstadoGuia.DescripcionEstadoGuia == "Entrega Exitosa")
                                {
                                    rastreoGuia.ImagenGuia = ObtenerImagenGuia(e);
                                }
                            }
                        }
                    }
                    #endregion

                    #region Controller
                    /*************************************** Guias Controller **********************************************************/
                    else
                    {

                        // Remitente - Destinatario (Información de la gúia
                        rastreoGuia.Guia = MensajeriaRepository.Instancia.ObtenerGuiaXNumeroGuia(e);
                        // Estados de la guía
                        rastreoGuia.TrazaGuia = MensajeriaRepository.Instancia.ObtenerTrazaUltimoEstadoXNumGuia(e);
                        // ultimo estado de la guia
                        List<ADEstadoGuiaMotivoDC> EstadosMotivoGuia = MensajeriaRepository.Instancia.ObtenerEstadosMotivosGuia(e);
                        if (EstadosMotivoGuia.Count > 0)
                        {
                            //validar fecha entrega
                            var estadoGuiaEntregado = EstadosMotivoGuia.FirstOrDefault(o => o.EstadoGuia.IdEstadoGuia == 11);
                            if (estadoGuiaEntregado != null)
                            {
                                //comparar fecha de entrega si fecha capturada manual menor a descarga actualizar
                                if (rastreoGuia.Guia.FechaEntrega < estadoGuiaEntregado.EstadoGuia.FechaGrabacion)
                                {
                                    estadoGuiaEntregado.EstadoGuia.FechaGrabacion = rastreoGuia.Guia.FechaEntrega;
                                }
                            }

                            // Estado motivo guia : Incauto
                            var estadoGuiaIncautado = EstadosMotivoGuia.FirstOrDefault(o => o.Motivo.IdMotivoGuia == 159);
                            if (estadoGuiaIncautado != null)
                            {
                                /************************ Se filtra por incautado y se elimina el ultimo centro de acopio ***************************************/
                                var listaElementosMotivo = EstadosMotivoGuia.ToList().FindAll(x => x.EstadoGuia.IdEstadoGuia == (short)ADEnumEstadoGuia.CentroAcopio).OrderByDescending(y => y.EstadoGuia.FechaGrabacion).ToList();
                                if (listaElementosMotivo.Count > 0)
                                {
                                    EstadosMotivoGuia.Remove(listaElementosMotivo[0]);
                                }
                            }

                            /********************************************** ACTUALIZAR RESIDENTE AUSENTE / MENSAJERO NO ALCANZÓ *******************************************************/
                            EstadosMotivoGuia.ToList().FindAll(x => x.Motivo.IdMotivoGuia == 122).ForEach(a =>
                            {
                                a.Motivo.Descripcion = a.Motivo.Descripcion.Split('/')[0] + "/" + a.Motivo.Descripcion.Split('/')[1] + "/";
                            });

                            /********************************** ELIMINAR TRANSITO URBANO *****************************************************************/
                            var estadoGuiaTransitoUrbano = EstadosMotivoGuia.Where(w => w.EstadoGuia.IdEstadoGuia == (short)ADEnumEstadoGuia.TransitoUrbano);
                            if (estadoGuiaTransitoUrbano != null)
                            {
                                EstadosMotivoGuia.Remove(estadoGuiaTransitoUrbano.FirstOrDefault());
                            }

                            rastreoGuia.EstadosGuia = EstadosMotivoGuia;

                            /******************************** Ultimo estado de la guia ***********************************/
                            ADEstadoGuiaMotivoDC ultimoEstado = rastreoGuia.EstadosGuia.LastOrDefault();

                            /******************** Validacion ultimo estado reparto ******************/
                            if (ultimoEstado.EstadoGuia.IdEstadoGuia == (short)ADEnumEstadoGuia.EnReparto)
                            {
                                /************************ Obtene posicion mensajero por numero de guia (segun la ultima planilla generada) **************************/
                                POUbicacionMensajero poUbicacionMensajero = MensajeriaRepository.Instancia.ObtenerUltimaPosicionMensajeroPorNumeroGuia(e);
                                if (poUbicacionMensajero != null && poUbicacionMensajero.Latitud != 0 && poUbicacionMensajero.Longitud != 0)
                                {
                                    ADTrazaGuia adTrazaGuia = new ADTrazaGuia()
                                    {
                                        Latitud = poUbicacionMensajero.Latitud.ToString(),
                                        Longitud = poUbicacionMensajero.Longitud.ToString()
                                    };
                                    /***************Actualización del Estado guia traza ***************/
                                    ultimoEstado.EstadoGuia.Latitud = adTrazaGuia.Latitud;
                                    ultimoEstado.EstadoGuia.Longitud = adTrazaGuia.Longitud;
                                }
                            }
                        }


                        //Telemercadeo 
                        rastreoGuia.Telemercadeo = MensajeriaRepository.Instancia.ObtenerInformacionTelemercadeoGuia(e);
                        //Volantes de devolucion
                        rastreoGuia.Volantes = MensajeriaRepository.Instancia.ObtenerVolantesGuia(e);
                        //Novedades de transporte
                        rastreoGuia.NovedadesTransporte = MensajeriaRepository.Instancia.ObtenerNovedadesTransporteGuia(e);
                        //Imagen
                        rastreoGuia.ImagenGuia = ObtenerImagenGuia(e);

                        rastreoGuia.GuiaDevolucion = MensajeriaRepository.Instancia.obtieneEstadoGestionGuia(e);

                        //Obtener informacion guía interna radicado radicado
                        if (rastreoGuia.Guia.IdServicio == 16)
                        {
                            var numeroGuiaInterna = MensajeriaRepository.Instancia.ObtenerAdmisionRapiradicado(rastreoGuia.Guia.NumeroGuia, false);
                            rastreoGuia.NumeroGuiaInternaRadicado = numeroGuiaInterna;
                        }
                        if (rastreoGuia.Guia.EstadoGuia == ADEnumEstadoGuia.Archivada || rastreoGuia.Guia.EstadoGuia == ADEnumEstadoGuia.Digitalizada)
                        {

                            ADEstadoGuiaMotivoDC estado = null;
                            if (rastreoGuia.EstadosGuia != null)
                            {
                                estado = rastreoGuia.EstadosGuia.Where(l => l.EstadoGuia.IdEstadoGuia == (short)ADEnumEstadoGuia.DevolucionRatificada || l.EstadoGuia.IdEstadoGuia == (short)ADEnumEstadoGuia.Entregada).FirstOrDefault();

                                if (estado != null)
                                {
                                    rastreoGuia.TrazaGuia.IdEstadoGuia = estado.EstadoGuia.IdEstadoGuia;
                                    rastreoGuia.TrazaGuia.DescripcionEstadoGuia = estado.EstadoGuia.DescripcionEstadoGuia;
                                    rastreoGuia.Guia.EstadoGuia = (ADEnumEstadoGuia)estado.EstadoGuia.IdEstadoGuia;
                                    rastreoGuia.Guia.DescripcionEstado = estado.EstadoGuia.DescripcionEstadoGuia;
                                }

                                rastreoGuia.EstadosGuia.RemoveAll(guia => guia.EstadoGuia.IdEstadoGuia == (short)ADEnumEstadoGuia.Digitalizada || guia.EstadoGuia.IdEstadoGuia == (short)ADEnumEstadoGuia.Archivada);
                            }
                        }
                    }
                    #endregion
                    LstRastreoGuias.Add(rastreoGuia);
                }
            });

            return LstRastreoGuias;
        }


        public List<ONNovedadesTransporteDC> ObtenerNovedadesTransporte(long NumeroGuia)
        {
            return MensajeriaRepository.Instancia.ObtenerNovedadesTransporteGuia(NumeroGuia);

        }
        /// <summary>
        /// Metodo para validar el tratamiendo de la solicitud para el rastreo de las guia.
        /// </summary>
        /// <param name="guias"></param>
        /// <returns></returns>
        public ADRastreoGuiaClienteRespuesta ObtenerRastreoGuiasClientePost(ADRastreoGuiaClienteSolicitud guia)
        {
            ADRastreoGuiaClienteSolicitud guiaSolicitud = ObtenerADRastreoGuiaClienteSolicitud(guia);
            guiaSolicitud.EncriptaAes = guia.EncriptaAes;

            if (guia.NumeroIdentificacion is null && guia.NumeroTelefono is null)
            {
                return Instancia.ObtenerRastreoGuiasDatosProtegidosCliente(guiaSolicitud);
            }
            else
            {
                try
                {
                    ADGuiaPertenencia guiaPertenencia = MensajeriaRepository.Instancia.ObtenerPertenenciaGuiaPorNumeroGuia(guiaSolicitud.NumeroGuia);
                    bool guiaPerteneceConsulta = ObtenerPertenenciaGuia(guia.IdOpcion, guiaPertenencia, guiaSolicitud);

                    return ObtenerRastreoGuiasCliente(guiaSolicitud, guiaPerteneceConsulta);
                }
                catch (FaultException ex)
                {
                    ADGuiaPertenencia guiaPertenencia = ObtenerPreenvioPertenencia(guiaSolicitud.NumeroGuia);
                    bool guiaPerteneceConsulta = ObtenerPertenenciaGuia(guia.IdOpcion, guiaPertenencia, guiaSolicitud);
                    return ObtenerRastreoGuiasCliente(guiaSolicitud, guiaPerteneceConsulta);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guia"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public ADTRespuestaGeneral<ADRastreoGuiaIVRRespuesta> ObtenerRastreoGuiasClienteIVRPost(ADRastreoGuiaIVRSolicitud guia)
        {
            ADTRespuestaGeneral<ADRastreoGuiaIVRRespuesta> respuesta = new ADTRespuestaGeneral<ADRastreoGuiaIVRRespuesta>();

            long numeroGuia;

            if (guia.NumeroGuia == "" || guia.NumeroGuia == null)
            {
                respuesta.resultado = null;
                respuesta.error = true;
                respuesta.mensaje = "Se presentaron errores en la transaccion. Por favor ingrese el número de guía.";
            }

            if (!long.TryParse(guia.NumeroGuia, out numeroGuia))
            {
                respuesta.resultado = null;
                respuesta.error = true;
                respuesta.mensaje = "Se presentaron errores en la transaccion. Por favor ingrese el número de guía.";
            }

            ADGuia guiaPorNumero = MensajeriaRepository.Instancia.ObtenerGuiaXNumeroGuia(numeroGuia);
            ADTrazaGuia trazaGuiaPorNumero = MensajeriaRepository.Instancia.ObtenerTrazaUltimoEstadoXNumGuia(numeroGuia);
            bool isDevolucionRatificada = MensajeriaRepository.Instancia.ObtenerTrazaEstadoGuiaIVR(numeroGuia);

            respuesta = ObtenerResultadosRespuesta(guiaPorNumero, trazaGuiaPorNumero, isDevolucionRatificada);

            return respuesta;
        }

        /// <summary>
        /// Metodo para obtener en respuesta general todos los datos
        /// </summary>
        /// <param name="guiaPorNumero"></param>
        /// <param name="trazaGuiaPorNumero"></param>
        /// <param name="isDevolucionRatificada"></param>
        /// <returns></returns>
        private ADTRespuestaGeneral<ADRastreoGuiaIVRRespuesta> ObtenerResultadosRespuesta(ADGuia guiaPorNumero, ADTrazaGuia trazaGuiaPorNumero, bool isDevolucionRatificada)
        {
            ADTRespuestaGeneral<ADRastreoGuiaIVRRespuesta> respuestaGeneral = new ADTRespuestaGeneral<ADRastreoGuiaIVRRespuesta>();
            try
            {
                DateTime fechaActual = DateTime.Today;
                ADRastreoGuiaIVRRespuesta respuesta = new ADRastreoGuiaIVRRespuesta();
                respuesta.TrazaGuia = new ADTrazaGuiaEstadoGuia();

                respuesta.TrazaGuia.IdEstadoGuia = trazaGuiaPorNumero != null && trazaGuiaPorNumero.IdEstadoGuia != null
                    ? trazaGuiaPorNumero.IdEstadoGuia.GetValueOrDefault()
                    : (short)0;
                respuesta.TrazaGuia.DescripcionEstadoGuia = trazaGuiaPorNumero.DescripcionEstadoGuia;
                respuesta.TrazaGuia.Ciudad = trazaGuiaPorNumero.Ciudad;
                respuesta.TrazaGuia.FechaGrabacion = trazaGuiaPorNumero.FechaGrabacion;

                respuesta.EstadoEvaluacionEnvio = (guiaPorNumero.FechaEstimadaEntregaNew.Date < fechaActual)
                    ? ADEstadoEvaluacionEnvio.Vencido.ToString()
                    : ADEstadoEvaluacionEnvio.A_Tiempo.ToString();
                respuesta.FechaEstimadaEntregaNew = guiaPorNumero.FechaEstimadaEntregaNew;
                respuesta.FechaEstimadaEntrega = guiaPorNumero.FechaEstimadaEntrega;
                respuesta.CentroServicioEstado = trazaGuiaPorNumero.NombreCentroServicioEstado;
                respuesta.TipoEntrega = guiaPorNumero.DescripcionTipoEntrega;
                respuesta.CentroServicioDestino = guiaPorNumero.NombreCentroServicioDestino;
                respuesta.FechaEntrega = !isDevolucionRatificada ? DateTime.MinValue : guiaPorNumero.FechaEntrega;

                respuestaGeneral.resultado = respuesta;
                respuestaGeneral.error = false;
                respuestaGeneral.mensaje = "";

                return respuestaGeneral;
            }
            catch (Exception ex)
            {
                respuestaGeneral.resultado = null;
                respuestaGeneral.error = true;
                respuestaGeneral.mensaje = "Se presentaron errores en la transaccion. " + ex.Message;

                return respuestaGeneral;
            }
        }

        /// <summary>
		/// Metodo para validar el tratamiendo de la solicitud para el rastreo de las guia.
		/// </summary>
		/// <param name="guias"></param>
		/// <returns></returns>
		public ADRastreoGuiaClienteRespuesta ObtenerRastreoGuiasClientePortalPost(ADRastreoGuiaClienteSolicitud guia)
        {
            ADRastreoGuiaClienteSolicitud guiaSolicitud = ObtenerADRastreoGuiaClienteSolicitudPortal(guia);

            if (guia.NumeroIdentificacion is null && guia.NumeroTelefono is null)
            {
                return Instancia.ObtenerRastreoGuiasDatosProtegidosCliente(guiaSolicitud);
            }
            else
            {
                try
                {
                    ADGuiaPertenencia guiaPertenencia = MensajeriaRepository.Instancia.ObtenerPertenenciaGuiaPorNumeroGuia(guiaSolicitud.NumeroGuia);
                    bool guiaPerteneceConsulta = ObtenerPertenenciaGuia(guia.IdOpcion, guiaPertenencia, guiaSolicitud);

                    return ObtenerRastreoGuiasCliente(guiaSolicitud, guiaPerteneceConsulta);
                }
                catch (FaultException ex)
                {
                    ADGuiaPertenencia guiaPertenencia = ObtenerPreenvioPertenencia(guiaSolicitud.NumeroGuia);
                    bool guiaPerteneceConsulta = ObtenerPertenenciaGuia(guia.IdOpcion, guiaPertenencia, guiaSolicitud);
                    return ObtenerRastreoGuiasCliente(guiaSolicitud, guiaPerteneceConsulta);
                }
            }
        }
        /// <summary>
        /// Metodo para obtener el rastreo por numero de guia solo los datos protegidos.
        /// </summary>
        /// <param ADRastreoGuiaClienteSolicitud="guia"></param>
        /// <returns></returns>
        public ADRastreoGuiaClienteRespuesta ObtenerRastreoGuiasDatosProtegidosCliente(ADRastreoGuiaClienteSolicitud guia, bool EncriptaAes = false)
        {
            ADRastreoGuiaClienteRespuesta rastreoGuia = new ADRastreoGuiaClienteRespuesta();

            #region "Es Sispostal"
            if (guia.NumeroGuia.ToString().Length > 12 && guia.NumeroGuia.ToString().StartsWith("8"))
            {
                // Es guía sispostal
                rastreoGuia.EsSispostal = true;
                //Ultima gestion del envío
                rastreoGuia.TrazaGuia = MensajeriaRepository.Instancia.ObtenerTrazaUltimoEstadoPorNumGuiaSispostaldelPortal(guia.NumeroGuia);

                //Estados del envío
                List<INEstadosSWSispostal> EstadosSisPostal = null;

                if (rastreoGuia.TrazaGuia.IdEstadoGuia != (short)INEnumSispostalGuia.ANULADA
                && rastreoGuia.TrazaGuia.IdEstadoGuia != (short)INEnumSispostalGuia.RETENCION
                && rastreoGuia.TrazaGuia.IdEstadoGuia != (short)INEnumSispostalGuia.GUIASINFISICO)
                {
                    EstadosSisPostal = IntegracionSisPostal.Instancia.ObtenerEstadosGuiaSispostal(Convert.ToInt64(guia.NumeroGuia));
                }
                if (EstadosSisPostal != null)
                {
                    List<ADEstadoGuiaMotivoClienteRespuesta> estadosMotivoGuia = new List<ADEstadoGuiaMotivoClienteRespuesta>();
                    foreach (var item in EstadosSisPostal)
                    {
                        //Causales GUIA SIN FISICO, ANULADO, SINIESTRO, RETENCION NO DEBEN SER VISIBLES
                        if (!item.Estado.ToUpper().Contains("GUIA SIN FISICO") && !item.Estado.ToUpper().Contains("ANULADO") && !item.Estado.ToUpper().Contains("RETENCION"))
                        {
                            estadosMotivoGuia.Add(new ADEstadoGuiaMotivoClienteRespuesta
                            {
                                EstadoGuia = new ADTrazaGuiaEstadoGuia
                                {
                                    DescripcionEstadoGuia = item.Estado,
                                    Ciudad = item.Ciudad,
                                    FechaGrabacion = item.Fecha
                                }
                            });
                        }
                    }
                    rastreoGuia.EstadosGuia = estadosMotivoGuia;
                }

                // Remitente - Destinatario (Información de la gúia
                rastreoGuia.Guia = MensajeriaRepository.Instancia.ObtenerGuiaSispostalXNumeroGuiaPorPortal(guia.NumeroGuia, guia.EncriptaAes);
                rastreoGuia.ImagenGuia = null;
            }
            #endregion
            #region "Es Controller"
            else
            {
                try
                {
                    // Remitente - Destinatario (Información de la gúia
                    rastreoGuia.Guia = MensajeriaRepository.Instancia.ObtenerGuiaPorNumeroGuiaPorPortal(guia.NumeroGuia, guia.EncriptaAes);
                    // Estados de la guía
                    rastreoGuia.TrazaGuia = MensajeriaRepository.Instancia.ObtenerTrazaUltimoEstadoPorNumGuiaPorPortal(guia.NumeroGuia);

                    // ultimo estado de la guia
                    List<ADEstadoGuiaClienteRespuesta> EdoMotGuia = MensajeriaRepository.Instancia.ObtenerEstadosGuiaPorPortal(guia.NumeroGuia);
                    List<ADEstadoGuiaClienteRespuesta> EstadosMotivoGuia = UnificarEstadosDos(EdoMotGuia);
                    RemplazarDescripcionEstadosDos(ref EstadosMotivoGuia);

                    if (EstadosMotivoGuia.Count > 0)
                    {
                        /********************************** ELIMINAR TRANSITO URBANO *****************************************************************/
                        var estadoGuiaTransitoUrbano = EstadosMotivoGuia.Where(w => w.EstadoGuia.IdEstadoGuia == (short)ADEnumEstadoGuia.TransitoUrbano);
                        if (estadoGuiaTransitoUrbano != null)
                        {
                            EstadosMotivoGuia.Remove(estadoGuiaTransitoUrbano.FirstOrDefault());
                        }

                        rastreoGuia.EstadosGuia = EstadosMotivoGuia.Select(e => new ADEstadoGuiaMotivoClienteRespuesta
                        {
                            EstadoGuia = new ADTrazaGuiaEstadoGuia
                            {
                                IdEstadoGuia = e.EstadoGuia.IdEstadoGuia,
                                DescripcionEstadoGuia = e.EstadoGuia.DescripcionEstadoGuia,
                                Ciudad = e.EstadoGuia.Ciudad,
                                FechaGrabacion = e.EstadoGuia.FechaGrabacion
                            }
                        }).ToList();

                        /******************************** Ultimo estado de la guia ***********************************/
                        ADEstadoGuiaMotivoClienteRespuesta ultimoEstado = rastreoGuia.EstadosGuia.LastOrDefault();
                    }
                    //Imagen
                    rastreoGuia.ImagenGuia = null;
                }
                catch (FaultException ex)
                {
                    rastreoGuia = obtenerPreenvioProtegido(guia.NumeroGuia, true);
                }
            }
            #endregion

            return rastreoGuia;
        }
        /// <summary>
        /// Metodo para unificar los estado 2, para solo mostrsr origen y destino
        /// </summary>
        /// <param ADEstadoGuiaClienteRespuesta="estados"></param>
        /// <returns></returns>
        public List<ADEstadoGuiaClienteRespuesta> UnificarEstadosDos(List<ADEstadoGuiaClienteRespuesta> estados)
        {
            //Lista de estado de eventos especiales, según matriz Adjuntada en  HU46849
            ADEstadoGuiaClienteRespuesta adEstadoGuiaMotivoClienteRespuesta = new ADEstadoGuiaClienteRespuesta();
            List<int> lstEstadosEspeciales = new List<int> { 9, 14, 21, 24, 35, 40, 11, 10 };
            List<ADEstadoGuiaClienteRespuesta> lstEstadosHomologados = new List<ADEstadoGuiaClienteRespuesta>();
            int index = 0;
            int homologaCentroAcopioDestino = 0;
            foreach (ADEstadoGuiaClienteRespuesta item in estados)
            {
                if (item.EstadoGuia.IdEstadoGuia == 7 && homologaCentroAcopioDestino == 0)
                {
                    homologaCentroAcopioDestino = item.EstadoGuia.IdEstadoGuia;
                    if (estados.Where(x => x.EstadoGuia.IdEstadoGuia == 7).Count() > 0)
                    {
                        adEstadoGuiaMotivoClienteRespuesta = estados.Where(x => x.EstadoGuia.IdEstadoGuia == 7).LastOrDefault();
                        item.EstadoGuia.FechaGrabacion = adEstadoGuiaMotivoClienteRespuesta.EstadoGuia.FechaGrabacion;
                        item.EstadoGuia.Ciudad = adEstadoGuiaMotivoClienteRespuesta.EstadoGuia.Ciudad;
                        item.EstadoGuia.DescripcionEstadoGuia = adEstadoGuiaMotivoClienteRespuesta.EstadoGuia.DescripcionEstadoGuia;
                    }

                    lstEstadosHomologados.Add(item);
                }


                if (lstEstadosEspeciales.Contains(item.EstadoGuia.IdEstadoGuia))
                {
                    lstEstadosHomologados.Add(item);
                    break;
                }

                if (index > 0)
                {

                    if (estados[index].EstadoGuia.IdEstadoGuia == 2)
                    {
                        if (lstEstadosHomologados[lstEstadosHomologados.Count() - 1].EstadoGuia.IdEstadoGuia == 2 && lstEstadosHomologados.Count() >= 2)
                        {
                            lstEstadosHomologados[lstEstadosHomologados.Count() - 1].EstadoGuia.Ciudad = item.EstadoGuia.Ciudad;
                            lstEstadosHomologados[lstEstadosHomologados.Count() - 1].EstadoGuia.DescripcionEstadoGuia = item.EstadoGuia.DescripcionEstadoGuia;
                            lstEstadosHomologados[lstEstadosHomologados.Count() - 1].EstadoGuia.FechaGrabacion = item.EstadoGuia.FechaGrabacion;
                        }
                        else if (lstEstadosHomologados.Count() >= 2)
                        {
                            if (lstEstadosHomologados[lstEstadosHomologados.Count() - 2].EstadoGuia.IdEstadoGuia == 2 && lstEstadosHomologados[lstEstadosHomologados.Count() - 1].EstadoGuia.IdEstadoGuia == 6)
                            {
                                lstEstadosHomologados[lstEstadosHomologados.Count() - 2].EstadoGuia.Ciudad = item.EstadoGuia.Ciudad;
                                lstEstadosHomologados[lstEstadosHomologados.Count() - 2].EstadoGuia.DescripcionEstadoGuia = item.EstadoGuia.DescripcionEstadoGuia;
                                lstEstadosHomologados[lstEstadosHomologados.Count() - 2].EstadoGuia.FechaGrabacion = item.EstadoGuia.FechaGrabacion;
                            }
                            else
                            {
                                lstEstadosHomologados.Add(item);
                            }
                        }
                        else
                        {
                            lstEstadosHomologados.Add(item);
                        }
                    }

                    else if (estados[index].EstadoGuia.IdEstadoGuia == 6)
                    {
                        if (lstEstadosHomologados.Count() >= 2)
                        {
                            if (lstEstadosHomologados[lstEstadosHomologados.Count() - 2].EstadoGuia.IdEstadoGuia == 6)
                            {
                                lstEstadosHomologados[lstEstadosHomologados.Count() - 2].EstadoGuia.DescripcionEstadoGuia = item.EstadoGuia.DescripcionEstadoGuia;
                                lstEstadosHomologados[lstEstadosHomologados.Count() - 2].EstadoGuia.FechaGrabacion = item.EstadoGuia.FechaGrabacion;
                                lstEstadosHomologados[lstEstadosHomologados.Count() - 2].EstadoGuia.Ciudad = item.EstadoGuia.Ciudad;
                            }
                            else
                            {
                                lstEstadosHomologados.Add(item);
                            }
                        }
                        else
                        {
                            lstEstadosHomologados.Add(item);
                        }

                    }
                    else if (estados[index].EstadoGuia.IdEstadoGuia != 2 && estados[index].EstadoGuia.IdEstadoGuia != 7 && estados[index].EstadoGuia.IdEstadoGuia != 6)
                    {
                        lstEstadosHomologados.Add(item);
                    }
                }
                else
                {
                    lstEstadosHomologados.Add(item);
                }


                index++;
            }
            return DepurarEstadosRepetidos(lstEstadosHomologados);
        }
        /// <summary>
        /// Metodo para obtener el rastreo por numero de guia cuando pertenece al destinatario o remitente.
        /// </summary>
        /// <param ADRastreoGuiaClienteSolicitud="guia"></param>
        /// <returns></returns>
        public ADRastreoGuiaClienteRespuesta ObtenerRastreoGuiasClientePertenencia(ADRastreoGuiaClienteSolicitud guia, bool EncriptaAes = false)
        {
            ADRastreoGuiaClienteRespuesta rastreoGuia = new ADRastreoGuiaClienteRespuesta();

            #region "Es Sispostal"
            if (guia.NumeroGuia.ToString().Length > 12 && guia.NumeroGuia.ToString().StartsWith("8"))
            {
                // Es guía sispostal
                rastreoGuia.EsSispostal = true;
                //Ultima gestion del envío
                rastreoGuia.TrazaGuia = MensajeriaRepository.Instancia.ObtenerTrazaUltimoEstadoPorNumGuiaSispostaldelPortal(guia.NumeroGuia);

                //Estados del envío
                List<INEstadosSWSispostal> EstadosSisPostal = null;

                if (rastreoGuia.TrazaGuia.IdEstadoGuia != (short)INEnumSispostalGuia.ANULADA
                && rastreoGuia.TrazaGuia.IdEstadoGuia != (short)INEnumSispostalGuia.RETENCION
                && rastreoGuia.TrazaGuia.IdEstadoGuia != (short)INEnumSispostalGuia.GUIASINFISICO)
                {
                    EstadosSisPostal = IntegracionSisPostal.Instancia.ObtenerEstadosGuiaSispostal(Convert.ToInt64(guia.NumeroGuia));
                }
                if (EstadosSisPostal != null)
                {
                    List<ADEstadoGuiaMotivoClienteRespuesta> estadosMotivoGuia = new List<ADEstadoGuiaMotivoClienteRespuesta>();
                    foreach (var item in EstadosSisPostal)
                    {
                        //Causales GUIA SIN FISICO, ANULADO, SINIESTRO, RETENCION NO DEBEN SER VISIBLES
                        if (!item.Estado.ToUpper().Contains("GUIA SIN FISICO") && !item.Estado.ToUpper().Contains("ANULADO") && !item.Estado.ToUpper().Contains("RETENCION"))
                        {
                            estadosMotivoGuia.Add(new ADEstadoGuiaMotivoClienteRespuesta
                            {
                                EstadoGuia = new ADTrazaGuiaEstadoGuia
                                {
                                    DescripcionEstadoGuia = item.Estado,
                                    Ciudad = item.Ciudad,
                                    FechaGrabacion = item.Fecha
                                }
                            });
                        }
                    }
                    rastreoGuia.EstadosGuia = estadosMotivoGuia;
                }

                // Remitente - Destinatario (Información de la gúia
                rastreoGuia.Guia = MensajeriaRepository.Instancia.ObtenerGuiaSispostalPorNumeroGuiaPertenencia(guia.NumeroGuia, guia.EncriptaAes);
                rastreoGuia.ImagenGuia = ObtenerImagenGuia(Convert.ToInt64(guia.NumeroGuia));
            }
            #endregion
            #region "Es Controller"
            else
            {
                try
                {
                    // Remitente - Destinatario (Información de la gúia
                    rastreoGuia.Guia = MensajeriaRepository.Instancia.ObtenerGuiaPorNumeroGuiaPertenencia(guia.NumeroGuia, EncriptaAes);
                    // Estados de la guía
                    rastreoGuia.TrazaGuia = MensajeriaRepository.Instancia.ObtenerTrazaUltimoEstadoPorNumGuiaPorPortal(guia.NumeroGuia);
                    // ultimo estado de la guia
                    List<ADEstadoGuiaMotivoClienteRespuesta> EstadosMotivoGuia = MensajeriaRepository.Instancia.ObtenerEstadosMotivosGuiaPorPortal(guia.NumeroGuia);
                    if (EstadosMotivoGuia.Count > 0)
                    {
                        /********************************** ELIMINAR TRANSITO URBANO *****************************************************************/
                        var estadoGuiaTransitoUrbano = EstadosMotivoGuia.Where(w => w.EstadoGuia.IdEstadoGuia == (short)ADEnumEstadoGuia.TransitoUrbano);
                        if (estadoGuiaTransitoUrbano != null)
                        {
                            EstadosMotivoGuia.Remove(estadoGuiaTransitoUrbano.FirstOrDefault());
                        }

                        rastreoGuia.EstadosGuia = EstadosMotivoGuia;

                        /******************************** Ultimo estado de la guia ***********************************/
                        ADEstadoGuiaMotivoClienteRespuesta ultimoEstado = rastreoGuia.EstadosGuia.LastOrDefault();
                    }
                    //Imagen
                    rastreoGuia.ImagenGuia = ObtenerImagenGuia(Convert.ToInt64(guia.NumeroGuia));
                }
                catch (FaultException ex)
                {
                    rastreoGuia = obtenerPreenvioProtegido(guia.NumeroGuia, false);
                }
            }
            #endregion

            return rastreoGuia;
        }

        /// <summary>
        /// Metodo para reemplazar la DescripcionEstadoGuia de los estados 2
        /// </summary>
        /// <param ADEstadoGuiaClienteRespuesta="estadosGuia"></param>
        /// <returns></returns>
        public void RemplazarDescripcionEstadosDos(ref List<ADEstadoGuiaClienteRespuesta> estadosGuia)
        {
            Dictionary<int, string> mensajes = MensajesEstadoDos;

            List<ADEstadoGuiaClienteRespuesta> estadosDos = estadosGuia
                .Where(e => e.EstadoGuia.IdEstadoGuia == (short)ADEnumEstadoGuia.CentroAcopio)
                .OrderBy(e => e.EstadoGuia.FechaGrabacion)
                .ToList();

            string ObtenerMensaje(int mensajeInicial, int mensajeAlterno, ref int contador)
            {
                return contador++ == 0 ? mensajes[mensajeInicial] : mensajes[mensajeAlterno];
            }

            int contadorIndiferente = 0, contadorOrigen = 0, contadorDestino = 0;

            estadosDos.ForEach(e =>
            {
                ADTrazaGuiaEstadoGuiaDetallado estadoActual = e.EstadoGuia;

                if (estadoActual.IdLocalidadOrigen == estadoActual.IdLocalidadDestino)
                {
                    estadoActual.DescripcionEstadoGuia = ObtenerMensaje((int)ADEnumMensajeCentroAcopio.EnCentroLogistico, (int)ADEnumMensajeCentroAcopio.RetornoCentroLogistico, ref contadorIndiferente);
                }
                else if (estadoActual.IdLocalidadEnCurso == estadoActual.IdLocalidadOrigen)
                {
                    estadoActual.DescripcionEstadoGuia = ObtenerMensaje((int)ADEnumMensajeCentroAcopio.EnCentroLogisticoOrigen, (int)ADEnumMensajeCentroAcopio.RetornoCentroLogisticoOrigen, ref contadorOrigen);
                }
                else if (estadoActual.IdLocalidadEnCurso == estadoActual.IdLocalidadDestino)
                {
                    estadoActual.DescripcionEstadoGuia = ObtenerMensaje((int)ADEnumMensajeCentroAcopio.EnCentroLogisticoDestino, (int)ADEnumMensajeCentroAcopio.RetornoCentroLogisticoDestino, ref contadorDestino);
                }
                else
                {
                    estadoActual.DescripcionEstadoGuia = mensajes[(int)ADEnumMensajeCentroAcopio.EnCentroLogisticoTransito];
                }
            });
        }

        /// <summary>
        /// Depura estados repetido excepto estados 2
        /// </summary>
        /// <param name="lstGuiasUsuario"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<ADEstadoGuiaClienteRespuesta> DepurarEstadosRepetidos(List<ADEstadoGuiaClienteRespuesta> lstGuiasUsuario)
        {
            try
            {
                List<ADEstadoGuiaClienteRespuesta> lstADEstadoGuiaMotivoClienteRespuesta = new List<ADEstadoGuiaClienteRespuesta>();
                lstADEstadoGuiaMotivoClienteRespuesta.Add(lstGuiasUsuario[0]);
                for (int i = 0; i < lstGuiasUsuario.Count; i++)
                {
                    int contador = i == 0 ? 0 : i - 1;
                    if (lstGuiasUsuario[contador].EstadoGuia.IdEstadoGuia != lstGuiasUsuario[i].EstadoGuia.IdEstadoGuia && lstGuiasUsuario[i].EstadoGuia.IdEstadoGuia != 2)
                    {
                        lstADEstadoGuiaMotivoClienteRespuesta.Add(lstGuiasUsuario[i]);
                    }
                    else if (lstGuiasUsuario[contador].EstadoGuia.IdEstadoGuia == lstGuiasUsuario[i].EstadoGuia.IdEstadoGuia && lstGuiasUsuario[i].EstadoGuia.IdEstadoGuia != 2)
                    {
                        int valor = lstADEstadoGuiaMotivoClienteRespuesta.Count - 1;
                        lstADEstadoGuiaMotivoClienteRespuesta[valor].EstadoGuia.FechaGrabacion = lstGuiasUsuario[i].EstadoGuia.FechaGrabacion;
                        lstADEstadoGuiaMotivoClienteRespuesta[valor].EstadoGuia.Ciudad = lstGuiasUsuario[i].EstadoGuia.Ciudad;
                        lstADEstadoGuiaMotivoClienteRespuesta[valor].EstadoGuia.DescripcionEstadoGuia = lstGuiasUsuario[i].EstadoGuia.DescripcionEstadoGuia;
                    }
                    else if (lstGuiasUsuario[i].EstadoGuia.IdEstadoGuia == 2)
                    {
                        lstADEstadoGuiaMotivoClienteRespuesta.Add(lstGuiasUsuario[i]);
                    }
                }

                return lstADEstadoGuiaMotivoClienteRespuesta;
            }
            catch (Exception ex)
            {
                throw new Exception("Error en apiservinter" + ex.ToString());
            }
        }
        /// <summary>
        /// Metodo para ocultar(*) todos los caracteres de una cadena, a excepción del primer y último.
        /// </summary>
        /// <param string="texto"></param>
        /// <returns></returns>
        public static string OcultarTexto(string texto)
        {
            if (string.IsNullOrEmpty(texto))
            {
                return texto;
            }

            char primerCaracter = texto[0];
            char ultimoCaracter = texto[texto.Length - 1];

            int longitud = texto.Length - 2;
            string asteriscos = new string('*', longitud);

            return $"{primerCaracter}{asteriscos}{ultimoCaracter}";
        }

        /// <summary>
        /// Obtiene la imagen de una guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        public string ObtenerImagenGuia(long numeroGuia)
        {
            LIArchivoGuiaMensajeriaDC archivo = new LIArchivoGuiaMensajeriaDC()
            {
                ValorDecodificado = numeroGuia.ToString()
            };

            string Sftp;
            string SPassword;
            string SUsuario;
            int cont = 0;
            string imagen = "";

            if (numeroGuia.ToString().Length > 12 && numeroGuia.ToString().StartsWith("8"))
            {
                Sftp = "ftpDigSispostal";
                SPassword = "passFtpDigSispostal";
                SUsuario = "UserFtpDigSispostal";
                archivo = MensajeriaRepository.Instancia.ObtenerArchivoGuiaSispostal(archivo);
                imagen = TraerImagenFtp(archivo, numeroGuia, Sftp, SUsuario, SPassword, cont);
            }
            else
            {
                Sftp = "ftpDigitalizacion";
                SPassword = "passFtpDigitalizaci";
                SUsuario = "UserFtpDigitalizaci";
                archivo = MensajeriaRepository.Instancia.ObtenerArchivoGuiaFS(archivo);

                if (archivo != null)
                {
                    foreach (var a in archivo.RutaServidor.ToList())
                    {
                        int convertir = 0;
                        if (int.TryParse(a.ToString(), out convertir))
                        {
                            break;
                        }
                        else
                            cont++;
                    }
                    imagen = TraerImagenFtp(archivo, numeroGuia, Sftp, SUsuario, SPassword, cont);
                }
            }
            return imagen;
        }

        /// <summary>
        /// Metodo para traer imagen FTP
        /// </summary>
        /// <param name="archivo"></param>
        /// <param name="numeroGuia"></param>
        /// <param name="SFtp"></param>
        /// <param name="SUsuario"></param>
        /// <param name="SPassword"></param>
        /// <param name="cont"></param>
        /// <returns></returns>
        private string TraerImagenFtp(LIArchivoGuiaMensajeriaDC archivo, long numeroGuia, string SFtp, string SUsuario, string SPassword, int cont)
        {
            string ftp;
            string pass;
            string user;
            Uri Uriftp = null;

            try
            {
                ftp = ParametrosNegocio.Instancia.ConsultarParametrosFramework(SFtp);
                pass = ParametrosNegocio.Instancia.ConsultarParametrosFramework(SPassword);
                user = ParametrosNegocio.Instancia.ConsultarParametrosFramework(SUsuario);

                if (archivo != null)
                {
                    if (numeroGuia.ToString().Length > 12 && numeroGuia.ToString().StartsWith("8"))
                    {
                        Uriftp = new Uri(ftp + "/" + archivo.RutaServidor.Replace(@"\", "/"));
                    }
                    else
                    {
                        Uriftp = new Uri(ftp + "/" + archivo.RutaServidor.Substring(cont, archivo.RutaServidor.Length - cont).Replace(@"\", "/"));
                    }
                    FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(Uriftp);
                    ftpRequest.Credentials = new NetworkCredential(user, pass);
                    ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;

                    FtpWebResponse ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                    Stream stream = ftpResponse.GetResponseStream();
                    byte[] imagenArray = Utilidades.ReadToEnd(stream);
                    stream.Close();
                    string imgString = Convert.ToBase64String(imagenArray);
                    return imgString;
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Permite obtener el listado de imagenes de evidencias
        /// </summary>
        /// <param name="guia">Número de guia filtro</param>
        /// <param name="IdTipoEvidencia">Ide de tipo evidencia filtro</param>
        /// <returns>numero de guia, ruta imagen evidencia e imagen evidencia base64</returns>
        public LIImagenesDevolucion ObtenerImagenesGuiaController(long NumeroGuia)
        {
            LIImagenesDevolucion InformacionGuia = MensajeriaRepository.Instancia.ObtenerRutaArchivosGuia(NumeroGuia);
            if (InformacionGuia != null)
            {
                int num = 1;
                foreach (LIImagenEvidencia item in InformacionGuia.ListaImagenEvidencia)
                {
                    item.Posicion = num;
                    item.Imagen = ObtenerImagenCompartidaBase64(item.Ruta);
                    item.ExisteImagenServidor = !string.IsNullOrEmpty(item.Imagen);
                    num++;
                }
            }
            return InformacionGuia;
        }

        private string ObtenerImagenCompartidaBase64(string RutaArchivo)
        {
            string imagenbase64 = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(RutaArchivo))
                {
                    FileInfo file = new FileInfo(RutaArchivo);
                    if (file.Exists)
                    {
                        byte[] imagenArray = Utilidades.ReadToEnd(file.OpenRead());
                        string imgString = Convert.ToBase64String(imagenArray);
                        imagenbase64 = imgString;
                    }
                }
                return imagenbase64;
            }
            catch
            {
                return imagenbase64;
            }
        }

        #region COTIZADOR

        /// <summary>
        /// Metodo que obtiene el id lista de precios vigente tarifa plena
        /// Hevelin Dayana Diaz - 14/12/2021
        /// </summary>
        /// <returns></returns>
        public int ObtenerIdListaPrecioVigente()
        {
            return MensajeriaRepository.Instancia.ObtenerIdListaPrecioVigente();
        }

        /// <summary>
        /// Retorna la lista de tipos de entrega que pueden aplicar sobre el envío
        /// </summary>
        /// <returns></returns>
        public List<ADTipoEntrega> ObtenerTiposEntrega()
        {
            return MensajeriaRepository.Instancia.ObtenerTiposEntrega();
        }


        /// <summary>
        /// Método para calcular tarifas
        /// </summary>
        /// <param name="idLocalidadOrigen"></param>
        /// <param name="idLocalidadDestino"></param>
        /// <param name="peso"></param>
        /// <param name="valorDeclarado"></param>
        /// <param name="idTipoEntrega"></param>
        /// <returns></returns>

        public List<TAPreciosAgrupadosDC> ResultadoListaCotizar(string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, string idTipoEntrega, string fechaRecogida)
        {
            /***********Calculo de servicios por precios respectivos **********/
            List<TAPreciosAgrupadosDC> preciosCotizacion = CalculaServicioCotizador(idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, idTipoEntrega, fechaRecogida);
            /************Calculo de valor prima para carga terrestre *************/
            var cargarTerrestre = preciosCotizacion.Where(e => e.IdServicio == TAConstantesServicios.SERVICIO_RAPI_CARGA).FirstOrDefault();

            int idListaPrecios = MensajeriaRepository.Instancia.ObtenerIdListaPrecioVigente();
            decimal topeMinRapiCarga = Convert.ToDecimal(MensajeriaRepository.Instancia.ObtenerParametrosAdmisiones("TopeMinVlrDeclRapiCa"));

            if (cargarTerrestre != null)
            {
                if (valorDeclarado < topeMinRapiCarga)
                {
                    valorDeclarado = topeMinRapiCarga;
                }

                int porcentajePrima = Convert.ToInt32(MensajeriaRepository.Instancia.ObtenerPorcentajePrimaSeguro(cargarTerrestre.IdServicio, idListaPrecios));
                cargarTerrestre.Precio.ValorPrimaSeguro = (valorDeclarado * porcentajePrima) / 100;
                cargarTerrestre.PrecioCarga.ValorPrimaSeguro = (valorDeclarado * porcentajePrima) / 100;
            }
            return preciosCotizacion;
        }

        /// <summary>
        /// Método para calcular tarifas
        /// </summary>
        /// <param name="idLocalidadOrigen">Identificador de la ciudad de origen</param>
        /// <param name="idLocalidadDestino">Identificador de la ciudad de destino</param>
        /// <param name="peso">Peso cubico en kg</param>
        /// <param name="valorDeclarado"> Valor declarado</param>
        /// <param name="idTipoEntrega">Identificador del tipo de entrega</param>
        /// <param name="fechaRecogida">Fecha de recogida</param>
        /// <param name="idCLiente">Identificador del cliente</param>
        /// <returns>Retorna lista de precios por servicios habilitados.</returns>
        public List<TAPreciosAgrupadosDC> ResultadoListaCotizarCliente(string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, string idTipoEntrega, string fechaRecogida, int idCLiente, int idcontrato = 0, bool aplicaContrapago = false, bool esMarketplace = false, bool esReliquidacion = false, bool pagoEnCasa = false)
        {
            int idListaPrecios = 0;
            /***********Calculo de servicios por precios respectivos **********/
            List<TAPreciosAgrupadosDC> preciosCotizacion = CalculaServicioCotizadorCliente(idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, idTipoEntrega, fechaRecogida, idCLiente, idcontrato, aplicaContrapago, esMarketplace, esReliquidacion);
            /************Calculo de valor prima para carga terrestre *************/
            var cargarTerrestre = preciosCotizacion.Where(e => e.IdServicio == TAConstantesServicios.SERVICIO_RAPI_CARGA).FirstOrDefault();

            decimal porcentajePrima = 0;

            decimal topeMinRapiCarga = 0;

            if (esReliquidacion)//Flujo Reliquidacion Bolsa de novedades Credito
            {
                topeMinRapiCarga = Convert.ToDecimal(MensajeriaRepository.Instancia.ObtenerParametrosAdmisiones("TopMinVlrDecRapiCCre"));

                if (cargarTerrestre != null)
                {
                    idListaPrecios = cargarTerrestre.idListaPrecios;

                    if (valorDeclarado < topeMinRapiCarga && !pagoEnCasa)
                    {
                        valorDeclarado = topeMinRapiCarga;
                    }
                    cargarTerrestre.nuevoValorComercial = valorDeclarado;

                    porcentajePrima = MensajeriaRepository.Instancia.ObtenerPorcentajePrimaSeguro(cargarTerrestre.IdServicio, idListaPrecios);

                    cargarTerrestre.Precio.ValorPrimaSeguro = (valorDeclarado * porcentajePrima) / 100;
                    cargarTerrestre.PrecioCarga.ValorPrimaSeguro = (valorDeclarado * porcentajePrima) / 100;
                }
            }
            else
            {
                if (cargarTerrestre != null)
                {
                    if (valorDeclarado < 100000)
                    {
                        valorDeclarado = 100000;
                    }
                    cargarTerrestre.Precio.ValorPrimaSeguro = valorDeclarado / 100;
                    cargarTerrestre.PrecioCarga.ValorPrimaSeguro = valorDeclarado / 100;
                }
            }

            return preciosCotizacion;
        }

        /// <summary>
        /// Realiza cotizacion de cliente credito por servicio
        /// </summary>
        /// <param name="precioServicioDto">Dto que tiene los parametros necesarios para la cotización</param>
        /// <returns>Información de tarifas por servicio</returns>
        public TAPreciosAgrupadosDC ResultadoListaCotizarClienteCredito(TAPrecioServicioDto precioServicioDto) {

            TAPreciosAgrupadosDC preciosCotizacion = CalculaServicioCotizadorClienteCredito(precioServicioDto);

            if (precioServicioDto.IdServicio == TAConstantesServicios.SERVICIO_RAPI_CARGA) {

                if (precioServicioDto.ValorDeclarado < ((decimal)PAEnumTarifas.TOPE_VALIDACION_CARGA))
                {
                    precioServicioDto.ValorDeclarado = (decimal)PAEnumTarifas.TOPE_VALIDACION_CARGA;
                }
                preciosCotizacion.Precio.ValorPrimaSeguro = precioServicioDto.ValorDeclarado / (int)PAEnumTarifas.PORCENTAJE_VALOR_PRIMA_SEGURO;
                preciosCotizacion.PrecioCarga.ValorPrimaSeguro = precioServicioDto.ValorDeclarado / (int)PAEnumTarifas.PORCENTAJE_VALOR_PRIMA_SEGURO;
            }

            return preciosCotizacion;
        }


        public TAPreciosAgrupadosDC CalculaServicioCotizadorClienteCredito(TAPrecioServicioDto precioServicioDto)
        {
            int dia = (int)DateTime.ParseExact(precioServicioDto.Fecha, "dd-MM-yyyy", CultureInfo.InvariantCulture).DayOfWeek; // día de la semana actual
            //Si el peso viene decimal se calcula con su aproximación más alta.
            precioServicioDto.Peso = Math.Ceiling(precioServicioDto.Peso);

            if(!ParametrosNegocio.Instancia.ValidarServicioPorContrato((int)precioServicioDto.IdContrato, precioServicioDto.IdServicio))
            {
                throw new Exception("No hay servicio asociado que cumpla con los datos ingresados.");
            }

            TAServicioPesoDC servicioPeso = MensajeriaRepository.Instancia.ConsultarServicioPesosMinimoxMaximosPorListaPrecioCredito(precioServicioDto.IdListaPrecios, precioServicioDto.IdServicio, precioServicioDto.Peso); //Metodo Mínimos y Máximos

            bool servicioRapicarga = false;

            if (servicioPeso != null && (precioServicioDto.Peso < servicioPeso.PesoMinimo || precioServicioDto.Peso > servicioPeso.PesoMaximo))
            {
                servicioRapicarga = true;
            }

            if (servicioRapicarga)
            {
                decimal topeMinRapiCarga = Convert.ToDecimal(MensajeriaRepository.Instancia.ObtenerParametrosAdmisiones("TopeMinVlrDeclRapiCarga"));
                precioServicioDto.IdServicio = TAConstantesServicios.SERVICIO_RAPI_CARGA;
                precioServicioDto.ValorDeclarado = topeMinRapiCarga;
            }

            TAPreciosAgrupadosDC PrecioServicio = TipoServicioCredito(precioServicioDto);
            bool destinoPermiteAlcobro = ParametrosNegocio.Instancia.ValidarMunicipioPermiteAlcobro(precioServicioDto.IdLocalidadDestino);
            bool costa = false;

            ADValidacionServicioTrayectoDestino validacionTiempo = ValidarServicioTrayectoDestino(
                        new PALocalidadDC() { IdLocalidad = precioServicioDto.IdLocalidadOrigen },
                        new PALocalidadDC() { IdLocalidad = precioServicioDto.IdLocalidadDestino },
                        new TAServicioDC() { IdServicio = precioServicioDto.IdServicio },
                        precioServicioDto.IdCentroServicioOrigen,
                        DateTime.ParseExact(precioServicioDto.Fecha, "dd-MM-yyyy", CultureInfo.InvariantCulture),
                        true,
                        precioServicioDto.Peso,
                        precioServicioDto.IdListaPrecios
                        );

            PrecioServicio.fechaEntrega = validacionTiempo.fechaEntrega;
            PrecioServicio.TiempoEntrega = (validacionTiempo.DuracionTrayectoEnHoras / 24).ToString();

            bool destinoCosta = ParametrosNegocio.Instancia.ValidarServicioTrayectoCasilleroAereo(precioServicioDto.IdLocalidadOrigen, precioServicioDto.IdLocalidadDestino, precioServicioDto.IdServicio);
            
            TAServicioDC servicio = ParametrosNegocio.Instancia.ObtenerNombreServicioPorIdServicio(precioServicioDto.IdServicio);
            PrecioServicio.NombreServicio = servicio.Nombre;

            if (destinoCosta && TAConstantesServicios.SERVICIO_CARGA_EXPRESS == precioServicioDto.IdServicio)
            {
                PrecioServicio.NombreServicio = "Aéreo Costa";
            }

            return PrecioServicio;
        }

        /// <summary>
        /// Nuevo Cotizador
        /// </summary>
        /// <param name="idLocalidadOrigen"></param>
        /// <param name="idLocalidadDestino"></param>
        /// <param name="peso"></param>
        /// <param name="valorDeclarado"></param>
        /// <param name="idTipoEntrega"></param>
        /// <returns></returns>
        private List<TAPreciosAgrupadosDC> CalculaServicioCotizador(string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, string idTipoEntrega, string fechaRecogida)
        {
            int dia = 0;
            //int dia = (int)DateTime.Now.DayOfWeek; // día de la semana actual
            try
            {

                dia = (int)Convert.ToDateTime(fechaRecogida).DayOfWeek; // día de la semana actual
            }
            catch (Exception x)
            {
                var datos = fechaRecogida.Split('-');
                dia = (int)new DateTime(int.Parse(datos[2]), int.Parse(datos[1]), int.Parse(datos[0])).DayOfWeek; // día de la semana actual                
                fechaRecogida = new DateTime(int.Parse(datos[2]), int.Parse(datos[1]), int.Parse(datos[0])).ToString();
            }


            List<TAServicioDC> listaServicios = ParametrosNegocio.Instancia.ObtenerServicios();  // Obtengo todos los servicios habilitados    

            List<TAServicioPesoDC> lstServiciosPesos = MensajeriaRepository.Instancia.ConsultarServiciosPesosMinimoxMaximos(); //Metodo Mínimos y Máximos

            List<TAServicioDC> lstServiciosHabiles = new List<TAServicioDC>();

            TAServicioDC servicioRapicarga = null;
            listaServicios.ForEach(servicio =>
            {

                var servicioPeso = lstServiciosPesos.Where(p => p.IdServicio == servicio.IdServicio).FirstOrDefault();
                if (servicioPeso != null)
                {
                    if (peso >= servicioPeso.PesoMinimo && peso <= servicioPeso.PesoMaximo)
                    {

                        if (servicioPeso.IdServicio != TAConstantesServicios.SERVICIO_RAPI_CARGA)
                            lstServiciosHabiles.Add(servicio);
                        else
                            servicioRapicarga = servicio;
                    }
                }
            });

            int idListaPrecios = MensajeriaRepository.Instancia.ObtenerIdListaPrecioVigente();

            List<TAPreciosAgrupadosDC> lstPreciosAgrupados = CalcularPrecioServicios(lstServiciosHabiles.Select(s => s.IdServicio).ToList(), idListaPrecios, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, idTipoEntrega);

            if (servicioRapicarga != null)
            {
                decimal topeMinRapiCarga = Convert.ToDecimal(MensajeriaRepository.Instancia.ObtenerParametrosAdmisiones("TopeMinVlrDeclRapiCarga"));
                List<int> lstSerRapicarga = new List<int>();
                lstSerRapicarga.Add(servicioRapicarga.IdServicio);
                lstPreciosAgrupados.AddRange(CalcularPrecioServicios(lstSerRapicarga, idListaPrecios, idLocalidadOrigen, idLocalidadDestino, peso, topeMinRapiCarga, idTipoEntrega));
                lstServiciosHabiles.Add(servicioRapicarga);
            }


            PUCentroServiciosDC centroOrigen = ParametrosNegocio.Instancia.ObtenerAgenciaLocalidad(idLocalidadOrigen);

            PALocalidadDC localidadOrigen = new PALocalidadDC()
            {
                IdLocalidad = idLocalidadOrigen
            };
            PALocalidadDC localidadDestino = new PALocalidadDC()
            {
                IdLocalidad = idLocalidadDestino
            };

            bool destinoPermiteAlcobro = ParametrosNegocio.Instancia.ValidarMunicipioPermiteAlcobro(idLocalidadDestino);

            List<Task> lstTareas = new List<Task>();

            bool costa = false;
            lstPreciosAgrupados.ForEach(ps =>
            {

                ADValidacionServicioTrayectoDestino validacionTiempo = ValidarServicioTrayectoDestino(localidadOrigen, localidadDestino, new TAServicioDC() { IdServicio = ps.IdServicio }, centroOrigen.IdCentroServicio, Convert.ToDateTime(fechaRecogida), true, peso);

                ps.fechaEntrega = validacionTiempo.fechaEntrega;

                ps.TiempoEntrega = (validacionTiempo.DuracionTrayectoEnHoras / 24).ToString();

                bool destinoCosta = ParametrosNegocio.Instancia.ValidarServicioTrayectoCasilleroAereo(idLocalidadOrigen, idLocalidadDestino, ps.IdServicio);

                var ser = lstServiciosHabiles.Where(s => s.IdServicio == ps.IdServicio).FirstOrDefault();
                if (ser != null)
                {
                    ps.NombreServicio = ser.Nombre;
                    ps.FranjaServicio = ser.FranjaServicio;
                }

                ps.FormaPagoServicio = new TAFormaPagoServicio()
                {
                    IdServicio = ps.IdServicio,
                    FormaPago = new List<TAFormaPago>()
                };
                ps.FormaPagoServicio.FormaPago.Add(new TAFormaPago() { Descripcion = "Contado" });

                if (destinoPermiteAlcobro && ps.IdServicio != TAConstantesServicios.SERVICIO_NOTIFICACIONES)
                    ps.FormaPagoServicio.FormaPago.Add(new TAFormaPago() { Descripcion = "AlCobro" });

                if (destinoCosta)
                {
                    switch (ps.IdServicio)
                    {
                        case TAConstantesServicios.SERVICIO_RAPI_CARGA:
                            ps.NombreServicio = "Carga Terrestre";
                            costa = true;
                            break;

                        case TAConstantesServicios.SERVICIO_CARGA_EXPRESS:
                            ps.NombreServicio = "Aéreo Costa";
                            costa = true;
                            break;
                    }

                }

                if (ps.IdServicio == TAConstantesServicios.SERVICIO_RAPI_CARGA)
                {
                    ps.NombreServicio = "Carga Terrestre";
                }
            });

            //se revalida si no es costa, y estan los dos servicios de carga (rapiCarga CargaExpress) (ids: 6,17) se deja solo el terrestre(6)
            if (!costa)
            {
                if (lstPreciosAgrupados.Where(s => s.IdServicio == TAConstantesServicios.SERVICIO_RAPI_CARGA || s.IdServicio == TAConstantesServicios.SERVICIO_CARGA_EXPRESS).Count() >= 2)
                {
                    var ser = lstPreciosAgrupados.Where(s => s.IdServicio == TAConstantesServicios.SERVICIO_CARGA_EXPRESS).FirstOrDefault();
                    if (ser != null)
                    {
                        lstPreciosAgrupados.Remove(ser);
                    }
                }
                else //Si hay un solo servicio de carga 6 o 17
                {
                    var ser = lstPreciosAgrupados.Where(s => s.IdServicio == TAConstantesServicios.SERVICIO_RAPI_CARGA || s.IdServicio == TAConstantesServicios.SERVICIO_CARGA_EXPRESS).FirstOrDefault();
                    if (ser != null)
                    {
                        ser.NombreServicio = "Rapi Carga";
                    }
                }

            }

            return lstPreciosAgrupados;
        }

        public ADInformacionEntrega ValidarServicioTrayecto(string idmunicipioOrigen, string idmunicipioDestino, string servicio, string centroServiciosOrigen, string fechadmisionEnvio, string validarTrayecto, string peso)
        {
            PALocalidadDC localidadOrigen = new PALocalidadDC()
            {
                IdLocalidad = idmunicipioOrigen
            };
            PALocalidadDC localidadDestino = new PALocalidadDC()
            {
                IdLocalidad = idmunicipioDestino
            };
            TAServicioDC servicioDC = new TAServicioDC()
            {
                IdServicio = Int32.Parse(servicio)
            };
            long centroServicio = Int64.Parse(centroServiciosOrigen);
            DateTime fechaAdmision = DateTime.ParseExact(fechadmisionEnvio, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            bool validartrayecto = Convert.ToBoolean(validarTrayecto);
            decimal Peso = Decimal.Parse(peso);

            ADValidacionServicioTrayectoDestino result = ValidarServicioTrayectoDestino(localidadOrigen, localidadDestino, servicioDC, centroServicio, fechaAdmision, validartrayecto, Peso);
            ADInformacionEntrega informacionEntrega = new ADInformacionEntrega();
            informacionEntrega.fechaEntrega = result.fechaEntrega;
            informacionEntrega.tiempoEntrega = (result.DuracionTrayectoEnHoras / 24);

            return informacionEntrega;
        }

        private List<TAPreciosAgrupadosDC> CalculaServicioCotizadorCliente(string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, string idTipoEntrega, string fechaRecogida, int idCliente, int idcontrato = 0, bool aplicaContrapago = false, bool esMarketplace = false, bool esReliquidacion = false)
        {
            int dia = (int)DateTime.ParseExact(fechaRecogida, "dd-MM-yyyy", CultureInfo.InvariantCulture).DayOfWeek; // día de la semana actual

            //Si el peso viene decimal se calcula con su aproximación más alta.
            peso = Math.Ceiling(peso);

            PAContratoCliente contrato = ParametrosNegocio.Instancia.ObtenerContratoPorIdCliente(idCliente, idcontrato);
            if (contrato == null)
                throw new Exception("No se encontraron contratos vigentes para el cliente.\n");

            bool estadoValidezLocalidad = ParametrosNegocio.Instancia.ConsultarValidezDestinoGeneracionGuias(idLocalidadDestino);
            if (aplicaContrapago && estadoValidezLocalidad)
                throw new Exception("El destino no es válido para envíos con contra pago y pago en casa.\n");


            List<TAServicioDC> listaServicios = ParametrosNegocio.Instancia.ObtenerServiciosPorContrato(contrato.IdCliente);  // Obtengo todos los servicios habilitados    

            List<TAServicioPesoDC> lstServiciosPesos = MensajeriaRepository.Instancia.ConsultarServiciosPesosMinimoxMaximosPorListaPrecio(contrato.IdListaPrecios); //Metodo Mínimos y Máximos

            TAValorPesoDeclaradoDC valorMinimoDeclarado = new TAValorPesoDeclaradoDC();

            if (contrato.IdListaPrecios != 0)
            {
                TAValorPesoDeclaradoDC valoresDeclaradosCliente = MensajeriaRepository.Instancia.ObtenerValorPesoDeclarado(contrato.IdListaPrecios, peso); // Verificación de valor declarado definido para el cliente
                valorMinimoDeclarado = valoresDeclaradosCliente;
            }

            decimal valorMinimo = valorMinimoDeclarado.ValorMinimoDeclarado;
            decimal valorMaximo = valorMinimoDeclarado.ValorMaximoDeclarado;


            List<TAServicioDC> lstServiciosHabiles = new List<TAServicioDC>();

            TAServicioDC servicioRapicarga = null;
            listaServicios.ForEach(servicio =>
            {

                var servicioPeso = lstServiciosPesos.Where(p => p.IdServicio == servicio.IdServicio).FirstOrDefault();
                if (servicioPeso != null)
                {
                    if (peso >= servicioPeso.PesoMinimo && peso <= servicioPeso.PesoMaximo)
                    {

                        if (servicioPeso.IdServicio != TAConstantesServicios.SERVICIO_RAPI_CARGA)
                            lstServiciosHabiles.Add(servicio);
                        else
                            servicioRapicarga = servicio;
                    }
                }
            });
            int idListaPrecios = 0;

            if (esReliquidacion)
            {
                if (lstServiciosHabiles.Count == 0)  // En caso que el cliente no cuente con línea de servicio, de la nueva liquidación, se toma la lista de precios de tarifa plena. HU56467
                {
                    listaServicios = ParametrosNegocio.Instancia.ObtenerServicios();  // Obtengo todos los servicios habilitados           

                    lstServiciosPesos = MensajeriaRepository.Instancia.ConsultarServiciosPesosMinimoxMaximos(); //Metodo Mínimos y Máximos

                    listaServicios.ForEach(servicio =>
                    {

                        var servicioPeso = lstServiciosPesos.Where(p => p.IdServicio == servicio.IdServicio).FirstOrDefault();
                        if (servicioPeso != null)
                        {
                            if (peso >= servicioPeso.PesoMinimo && peso <= servicioPeso.PesoMaximo)
                            {

                                if (servicioPeso.IdServicio != TAConstantesServicios.SERVICIO_RAPI_CARGA)
                                    lstServiciosHabiles.Add(servicio);
                                else
                                    servicioRapicarga = servicio;
                            }
                        }
                    });

                    idListaPrecios = MensajeriaRepository.Instancia.ObtenerIdListaPrecioVigente();
                }
                else
                {

                    idListaPrecios = contrato.IdListaPrecios;
                }
            }
            else
            {
                idListaPrecios = contrato.IdListaPrecios;
            }

            if (valorDeclarado >= valorMinimo && valorDeclarado <= valorMaximo || esReliquidacion) // Adición Operador OR para : HU69368 - Regla PEC SOLO para Control Pesos
            {
                List<TAPreciosAgrupadosDC> lstPreciosAgrupados = CalcularPrecioServicios(lstServiciosHabiles.Select(s => s.IdServicio).ToList(), idListaPrecios, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, idTipoEntrega);

                if (servicioRapicarga != null)
                {
                    decimal topeMinRapiCarga = Convert.ToDecimal(MensajeriaRepository.Instancia.ObtenerParametrosAdmisiones("TopeMinVlrDeclRapiCarga"));
                    List<int> lstSerRapicarga = new List<int>();
                    lstSerRapicarga.Add(servicioRapicarga.IdServicio);
                    lstPreciosAgrupados.AddRange(CalcularPrecioServicios(lstSerRapicarga, idListaPrecios, idLocalidadOrigen, idLocalidadDestino, peso, topeMinRapiCarga, idTipoEntrega));
                    lstServiciosHabiles.Add(servicioRapicarga);
                }


                PUCentroServiciosDC centroOrigen = ParametrosNegocio.Instancia.ObtenerAgenciaLocalidad(idLocalidadOrigen);

                PALocalidadDC localidadOrigen = new PALocalidadDC()
                {
                    IdLocalidad = idLocalidadOrigen
                };
                PALocalidadDC localidadDestino = new PALocalidadDC()
                {
                    IdLocalidad = idLocalidadDestino
                };

                bool destinoPermiteAlcobro = ParametrosNegocio.Instancia.ValidarMunicipioPermiteAlcobro(idLocalidadDestino);

                List<Task> lstTareas = new List<Task>();

                bool costa = false;
                lstPreciosAgrupados.ForEach(ps =>
                {

                    ADValidacionServicioTrayectoDestino validacionTiempo = ValidarServicioTrayectoDestino(
                        localidadOrigen,
                        localidadDestino,
                        new TAServicioDC() { IdServicio = ps.IdServicio },
                        centroOrigen.IdCentroServicio,
                        DateTime.ParseExact(fechaRecogida, "dd-MM-yyyy", CultureInfo.InvariantCulture),
                        true,
                        peso);

					if (esReliquidacion)
					{
						ps.idListaPrecios = idListaPrecios;
						if (!aplicaContrapago)
						{
							ps.nuevoValorComercial = (valorDeclarado > valorMinimo)? valorDeclarado : valorMinimo;
						}
					}

                    ps.fechaEntrega = validacionTiempo.fechaEntrega;

                    ps.TiempoEntrega = (validacionTiempo.DuracionTrayectoEnHoras / 24).ToString();

                    bool destinoCosta = ParametrosNegocio.Instancia.ValidarServicioTrayectoCasilleroAereo(idLocalidadOrigen, idLocalidadDestino, ps.IdServicio);

                    var ser = lstServiciosHabiles.Where(s => s.IdServicio == ps.IdServicio).FirstOrDefault();
                    if (ser != null)
                    {
                        ps.NombreServicio = ser.Nombre;
                    }

                    ps.FormaPagoServicio = new TAFormaPagoServicio()
                    {
                        IdServicio = ps.IdServicio,
                        FormaPago = new List<TAFormaPago>()
                    };
                    ps.FormaPagoServicio.FormaPago.Add(new TAFormaPago() { Descripcion = "Contado" });

                    if (destinoPermiteAlcobro && ps.IdServicio != TAConstantesServicios.SERVICIO_NOTIFICACIONES)
                        ps.FormaPagoServicio.FormaPago.Add(new TAFormaPago() { Descripcion = "AlCobro" });

                    if (destinoCosta)
                    {
                        switch (ps.IdServicio)
                        {
                            case TAConstantesServicios.SERVICIO_RAPI_CARGA:
                                ps.NombreServicio = "Carga Terrestre";
                                costa = true;
                                break;

                            case TAConstantesServicios.SERVICIO_CARGA_EXPRESS:
                                ps.NombreServicio = "Aéreo Costa";
                                costa = true;
                                break;
                        }

                    }

                    if (ps.IdServicio == TAConstantesServicios.SERVICIO_RAPI_CARGA)
                    {
                        ps.NombreServicio = "Carga Terrestre";
                    }
                });

                //se revalida si no es costa, y estan los dos servicios de carga (rapiCarga CargaExpress) (ids: 6,17) se deja solo el terrestre(6)
                if (!costa)
                {
                    if (!esMarketplace)
                    {
                        if (lstPreciosAgrupados.Where(s => s.IdServicio == TAConstantesServicios.SERVICIO_RAPI_CARGA || s.IdServicio == TAConstantesServicios.SERVICIO_CARGA_EXPRESS).Count() >= 2)
                        {
                            var ser = lstPreciosAgrupados.Where(s => s.IdServicio == TAConstantesServicios.SERVICIO_CARGA_EXPRESS).FirstOrDefault();
                            if (ser != null)
                            {
                                lstPreciosAgrupados.Remove(ser);
                            }
                        }
                        else //Si hay un solo servicio de carga 6 o 17
                        {
                            var ser = lstPreciosAgrupados.Where(s => s.IdServicio == TAConstantesServicios.SERVICIO_RAPI_CARGA || s.IdServicio == TAConstantesServicios.SERVICIO_CARGA_EXPRESS).FirstOrDefault();
                            if (ser != null)
                            {
                                ser.NombreServicio = "Rapi Carga";
                            }
                        }
                    }
                }

                return lstPreciosAgrupados;
            }
            else
            {
                throw new Exception("El valor declarado no se encuentra dentro del rango permitido (" + Convert.ToInt32(valorMinimo) + " - " + Convert.ToInt32(valorMaximo) + "). Favor rectificar los datos ingresados.\n");
            }
        }

        /// <summary>
        /// Calcula el precio por servicio
        /// </summary>
        /// <param name="servicios"></param>
        /// <param name="idListaPrecio"></param>
        /// <param name="idLocalidadOrigen"></param>
        /// <param name="idLocalidadDestino"></param>
        /// <param name="peso"></param>
        /// <param name="valorDeclarado"></param>
        /// <param name="idTipoEntrega"></param>
        /// <returns></returns>
        public List<TAPreciosAgrupadosDC> CalcularPrecioServicios(List<int> servicios, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, string idTipoEntrega)
        {
            List<TAPreciosAgrupadosDC> precios = new List<TAPreciosAgrupadosDC>();

            if (Convert.ToInt16(idTipoEntrega) <= 2)
            {
                foreach (int servicio in servicios)
                {
                    switch (servicio)
                    {
                        case TAConstantesServicios.SERVICIO_CARGA_EXPRESS:
                            TAPreciosAgrupadosDC prec = new TAPreciosAgrupadosDC() { IdServicio = servicio };
                            try
                            {
                                prec.Precio = CalcularPrecioGeneral(TAConstantesServicios.SERVICIO_CARGA_EXPRESS, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, true, idTipoEntrega);
                                //CalcularPrecioCargaExpress(servicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, true, idTipoEntrega);
                                precios.Add(prec);
                            }
                            catch (FaultException<Exception> ex)
                            {
                                prec.Mensaje = ex.Message;
                            }
                            break;

                        case TAConstantesServicios.SERVICIO_CARGA_AEREA:
                            TAPreciosAgrupadosDC precAr = new TAPreciosAgrupadosDC() { IdServicio = servicio };
                            try
                            {
                                precAr.Precio = CalcularPrecioGeneral(TAConstantesServicios.SERVICIO_CARGA_AEREA, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, true, idTipoEntrega);
                                //CalcularPrecioCargaAerea(servicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado);
                                precios.Add(precAr);
                            }
                            catch (FaultException<Exception> ex)
                            {
                                precAr.Mensaje = ex.Message;
                            }
                            break;

                        case TAConstantesServicios.SERVICIO_MENSAJERIA:
                        case TAConstantesServicios.SERVICIO_RAPI_TULAS:
                        case TAConstantesServicios.SERVICIO_RAPI_VALORES_MENSAJERIA:
                        case TAConstantesServicios.SERVICIO_RAPI_VALORES_CARGA:
                        case TAConstantesServicios.SERVICIO_RAPI_CARGA_CONSOLIDADA:
                        case TAConstantesServicios.SERVICIO_RAPI_VALIJAS:
                            TAPreciosAgrupadosDC precMsj = new TAPreciosAgrupadosDC() { IdServicio = servicio };
                            try
                            {
                                precMsj.Precio = CalcularPrecioGeneral(servicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, true, idTipoEntrega);
                                //CalcularPrecioMensajeria(servicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, true, idTipoEntrega);
                                precios.Add(precMsj);
                            }
                            catch (FaultException<Exception> ex)
                            {
                                precMsj.Mensaje = ex.Message;
                            }
                            break;

                        case TAConstantesServicios.SERVICIO_NOTIFICACIONES:
                            TAPreciosAgrupadosDC precNot = new TAPreciosAgrupadosDC() { IdServicio = servicio };
                            try
                            {
                                precNot.Precio = CalcularPrecioGeneral(TAConstantesServicios.SERVICIO_NOTIFICACIONES, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, true, idTipoEntrega);
                                //CalcularPrecioNotificaciones(servicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, true, idTipoEntrega);
                                precios.Add(precNot);
                            }
                            catch (FaultException<Exception> ex)
                            {
                                precNot.Mensaje = ex.Message;
                            }
                            break;

                        case TAConstantesServicios.SERVICIO_RAPI_AM:
                            TAPreciosAgrupadosDC precAm = new TAPreciosAgrupadosDC() { IdServicio = servicio };
                            try
                            {
                                precAm.Precio = CalcularPrecioGeneral(TAConstantesServicios.SERVICIO_RAPI_AM, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, true, idTipoEntrega);
                                //CalcularPrecioRapiAm(servicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, true, idTipoEntrega);
                                precios.Add(precAm);
                            }
                            catch (FaultException<Exception> ex)
                            {
                                precAm.Mensaje = ex.Message;
                            }
                            break;

                        case TAConstantesServicios.SERVICIO_RAPI_CARGA:
                            TAPreciosAgrupadosDC precCar = new TAPreciosAgrupadosDC() { IdServicio = servicio };
                            try
                            {
                                var precio = CalcularPrecioRapiCarga(servicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, true, idTipoEntrega);
                                precCar.PrecioCarga = precio;
                                precCar.Precio = new TAPrecioMensajeriaDC()
                                {
                                    Impuestos = precio.Impuestos,
                                    Valor = precio.Valor,
                                    ValorContraPago = precio.ValorContraPago,
                                    ValorKiloAdicional = precio.ValorKiloAdicional,
                                    ValorPrimaSeguro = precio.ValorPrimaSeguro
                                };
                                precios.Add(precCar);
                            }
                            catch (FaultException<Exception> ex)
                            {
                                precCar.Mensaje = ex.Message;
                            }
                            break;

                        case TAConstantesServicios.SERVICIO_RAPI_CARGA_CONTRA_PAGO:

                            break;

                        case TAConstantesServicios.SERVICIO_RAPI_ENVIOS_CONTRAPAGO:

                            break;

                        case TAConstantesServicios.SERVICIO_RAPI_HOY:
                            TAPreciosAgrupadosDC precHoy = new TAPreciosAgrupadosDC() { IdServicio = servicio };
                            try
                            {
                                precHoy.Precio = CalcularPrecioGeneral(TAConstantesServicios.SERVICIO_RAPI_HOY, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, true, idTipoEntrega);
                                //CalcularPrecioRapiHoy(servicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, true, idTipoEntrega);
                                precios.Add(precHoy);
                            }
                            catch (FaultException<Exception> ex)
                            {
                                precHoy.Mensaje = ex.Message;
                            }
                            break;

                        case TAConstantesServicios.SERVICIO_RAPI_PERSONALIZADO:
                            TAPreciosAgrupadosDC precPer = new TAPreciosAgrupadosDC() { IdServicio = servicio };
                            try
                            {
                                precPer.Precio = CalcularPrecioGeneral(TAConstantesServicios.SERVICIO_RAPI_PERSONALIZADO, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, true, idTipoEntrega);
                                //CalcularPrecioRapiPersonalizado(servicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, true, idTipoEntrega);
                                precios.Add(precPer);
                            }
                            catch (FaultException<Exception> ex)
                            {
                                precPer.Mensaje = ex.Message;
                            }
                            break;

                        case TAConstantesServicios.SERVICIO_RAPI_PROMOCIONAL:
                            break;

                        case TAConstantesServicios.SERVICIO_RAPIRADICADO:
                            TAPreciosAgrupadosDC precRadicado = new TAPreciosAgrupadosDC() { IdServicio = servicio };
                            try
                            {
                                precRadicado.Precio = CalcularPrecioGeneral(TAConstantesServicios.SERVICIO_RAPIRADICADO, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, true, idTipoEntrega);
                                //CalcularPrecioRapiradicado(idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, true, idTipoEntrega);
                                precios.Add(precRadicado);
                            }
                            catch (FaultException<Exception> ex)
                            {
                                precRadicado.Mensaje = ex.Message;
                            }
                            break;
                    }
                }
            }
            else
            {
                foreach (int servicio in servicios)
                {
                    switch (servicio)
                    {
                        case TAConstantesServicios.SERVICIO_CARGA_EXPRESS:
                            TAPreciosAgrupadosDC prec = new TAPreciosAgrupadosDC() { IdServicio = servicio };
                            try
                            {
                                prec.Precio = CalcularPrecioCargaExpress(servicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, true, idTipoEntrega);
                                precios.Add(prec);
                            }
                            catch (FaultException<Exception> ex)
                            {
                                prec.Mensaje = ex.Message;
                            }
                            break;

                        case TAConstantesServicios.SERVICIO_MENSAJERIA:
                            TAPreciosAgrupadosDC precMsj = new TAPreciosAgrupadosDC() { IdServicio = servicio };
                            try
                            {
                                precMsj.Precio = CalcularPrecioGeneral(TAConstantesServicios.SERVICIO_MENSAJERIA, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, true, idTipoEntrega);
                                //CalcularPrecioMensajeria(servicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, true, idTipoEntrega);
                                precios.Add(precMsj);
                            }
                            catch (FaultException<Exception> ex)
                            {
                                precMsj.Mensaje = ex.Message;
                            }
                            break;

                        case TAConstantesServicios.SERVICIO_NOTIFICACIONES:
                            TAPreciosAgrupadosDC precNot = new TAPreciosAgrupadosDC() { IdServicio = servicio };
                            try
                            {
                                precNot.Precio = CalcularPrecioGeneral(TAConstantesServicios.SERVICIO_NOTIFICACIONES, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, true, idTipoEntrega);
                                //CalcularPrecioMensajeria(servicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, true, idTipoEntrega);
                                precios.Add(precNot);
                            }
                            catch (FaultException<Exception> ex)
                            {
                                precNot.Mensaje = ex.Message;
                            }
                            break;

                        case TAConstantesServicios.SERVICIO_RAPIRADICADO:
                            TAPreciosAgrupadosDC precRadicado = new TAPreciosAgrupadosDC() { IdServicio = servicio };
                            try
                            {
                                precRadicado.Precio = CalcularPrecioGeneral(TAConstantesServicios.SERVICIO_RAPIRADICADO, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, true, idTipoEntrega);
                                //CalcularPrecioCargaExpress(servicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, true, idTipoEntrega);
                                precios.Add(precRadicado);
                            }
                            catch (FaultException<Exception> ex)
                            {
                                precRadicado.Mensaje = ex.Message;
                            }
                            break;
                    }
                }
            }
            return precios;
        }

        /// <summary>
        /// Realiza la validación por servicio para consumir su respectivo metodo según las reglas de negocio
        /// </summary>
        /// <param name="precioServicioDto">Dto correspondiente con paremetros de localidad, lista de precios y servicios</param>
        /// <returns>Precio correspondiente por servicio consumido</returns>

        public TAPreciosAgrupadosDC TipoServicioCredito(TAPrecioServicioDto precioServicioDto)
        {
            TAPrecioMensajeriaDC precioServicioGeneral = new TAPrecioMensajeriaDC();
            TAPrecioCargaDC precioServicioCarga = new TAPrecioCargaDC();
            TAPreciosAgrupadosDC precioServicio = new TAPreciosAgrupadosDC();
            int tipoEntrega = Convert.ToInt16(precioServicioDto.IdTipoEntrega);

            if (TAConstantesServicios.SERVICIO_RAPI_CARGA != precioServicioDto.IdServicio)
            {
                if (Convert.ToInt16(precioServicioDto.IdTipoEntrega) > 2 && TAConstantesServicios.SERVICIO_CARGA_EXPRESS == precioServicioDto.IdServicio) {

                    precioServicioGeneral = CalcularPrecioCargaExpress(precioServicioDto.IdServicio, precioServicioDto.IdListaPrecios, precioServicioDto.IdLocalidadOrigen, precioServicioDto.IdLocalidadDestino, precioServicioDto.Peso, precioServicioDto.ValorDeclarado, true, precioServicioDto.IdTipoEntrega);
                }
                else
                {
                    precioServicioGeneral = CalcularPrecioGeneral(precioServicioDto.IdServicio, precioServicioDto.IdListaPrecios, precioServicioDto.IdLocalidadOrigen, precioServicioDto.IdLocalidadDestino, precioServicioDto.Peso, precioServicioDto.ValorDeclarado, true, precioServicioDto.IdTipoEntrega);
                }
                
                precioServicio.Precio = precioServicioGeneral;
            }
            else
            {
                precioServicioCarga = CalcularPrecioRapiCarga(precioServicioDto.IdServicio, precioServicioDto.IdListaPrecios, precioServicioDto.IdLocalidadOrigen, precioServicioDto.IdLocalidadDestino, precioServicioDto.Peso, precioServicioDto.ValorDeclarado, true, precioServicioDto.IdTipoEntrega);
                precioServicio.PrecioCarga = precioServicioCarga;
                precioServicio.Precio = new TAPrecioMensajeriaDC()
                {
                    Impuestos = precioServicioCarga.Impuestos,
                    Valor = precioServicioCarga.Valor,
                    ValorContraPago = precioServicioCarga.ValorContraPago,
                    ValorKiloAdicional = precioServicioCarga.ValorKiloAdicional,
                    ValorPrimaSeguro = precioServicioCarga.ValorPrimaSeguro
                };
            }
            precioServicio.IdServicio = precioServicioDto.IdServicio;
            return precioServicio;
        }

        /// <summary>
        /// Valida si el servicio está habilitado para el trayecto dado por el municipio de origen y el de destino, debe retornar el peso máximo soportado para dicho trayecto
        /// </summary>
        /// <param name="municipioOrigen"></param>
        /// <param name="municipioDestino"></param>
        /// <param name="servicio"></param>
        /// <param name="centroServiciosOrigen"></param>
        /// <returns></returns>
        private ADValidacionServicioTrayectoDestino ValidarServicioTrayectoDestino(PALocalidadDC municipioOrigen, PALocalidadDC municipioDestino, TAServicioDC servicio, long centroServiciosOrigen, DateTime? fechadmisionEnvio = null, bool validarTrayecto = true, decimal peso = 0, int idListaPrecios = 0)
        {
            ADValidacionServicioTrayectoDestino validacionServicioTrayectoDestino = new ADValidacionServicioTrayectoDestino();
            int numeroDias = 0;
            int numeroHoras = 0;
            int idServicioMaxtiempo = 0;

            if (servicio.IdServicio != TAConstantesServicios.SERVICIO_INTERNACIONAL)
            {
                List<DateTime> listaFechasRecogida = new List<DateTime>();
                DateTime fec = Convert.ToDateTime(fechadmisionEnvio);
                DateTime fechaRecogida;
                DateTime fechaDigitalizacion;
                DateTime fechaEntregaSegunHorario;
                int horasRecogida = 0;
                bool fechaValida = false, fechaValidaEntrega = false;
                PUCentroServiciosDC centroSerDestino = ParametrosNegocio.Instancia.ObtenerAgenciaLocalidad(municipioDestino.IdLocalidad);
                long IdCentroServicioDestino = centroSerDestino.IdCentroServicio;
                List<TAHorarioRecogidaCsvDC> horarioCsvDestino = ParametrosNegocio.Instancia.ObtenerHorarioRecogidaDeCsv(IdCentroServicioDestino);
                TATiempoDigitalizacionArchivo tiempos = new TATiempoDigitalizacionArchivo();
                if (municipioOrigen.IdLocalidad == "99999")
                    municipioOrigen.IdLocalidad = "11001000";
                if (municipioDestino.IdLocalidad == "99999")
                    municipioDestino.IdLocalidad = "11001000";

                if (idListaPrecios > 0)
                {
                    tiempos = ParametrosNegocio.Instancia.ValidarServicioTrayectoDestinoExcepcion(municipioOrigen, municipioDestino, servicio, peso, idListaPrecios);
                }
                else
                {
                    tiempos = ParametrosNegocio.Instancia.ValidarServicioTrayectoDestino(municipioOrigen, municipioDestino, servicio, peso);
                }

                /// Se valida que si algun destino tiene 0 horas para entrega se traiga el servicio con mas tiempo de entrega
                if (tiempos.numeroDiasEntrega < 1)
                {
                    idServicioMaxtiempo = ParametrosNegocio.Instancia.ObtenerIdServicioDeMayorTiempoEntrega(municipioOrigen, municipioDestino);
                    tiempos = ParametrosNegocio.Instancia.ValidarServicioTrayectoDestino(municipioOrigen, municipioDestino, new TAServicioDC { IdServicio = idServicioMaxtiempo }, peso);
                }

                fechaRecogida = ParametrosNegocio.Instancia.ObtenerFechaRecogidaCiudad(municipioOrigen.IdLocalidad, fec.ToString("yyyy-MM-dd"));
                horasRecogida = ((fechaRecogida.Date.AddHours(18) - DateTime.Now.Date.AddHours(DateTime.Now.Hour)).Days * 24) + (fechaRecogida.Date.AddHours(18) - DateTime.Now.Date.AddHours(DateTime.Now.Hour)).Hours;

                //if (validarTrayecto)
                ////cambiar por el nuevo objeto
                numeroDias = tiempos.numeroDiasEntrega;

                if (ParametrosNegocio.Instancia.ValidarServicioTrayectoCasilleroAereo(municipioOrigen.IdLocalidad, municipioDestino.IdLocalidad, servicio.IdServicio))
                {
                    ADRangoTrayecto newTrayecto = new ADRangoTrayecto() { IdLocalidadOrigen = municipioOrigen.IdLocalidad, IdLocalidadDestino = municipioDestino.IdLocalidad };
                    newTrayecto.Rangos = new List<ADRangoCasillero>();
                    newTrayecto.Rangos.Add(new ADRangoCasillero() { RangoInicial = 0, RangoFinal = 999, Casillero = "AEREO" });
                    validacionServicioTrayectoDestino.InfoCasillero = newTrayecto;
                }
                else
                    validacionServicioTrayectoDestino.InfoCasillero = ConsultarCasilleroTrayecto(municipioOrigen.IdLocalidad, municipioDestino.IdLocalidad);

                if (servicio.IdServicio == TAConstantesServicios.SERVICIO_RAPI_AM)
                {
                    if (fechadmisionEnvio.HasValue)
                    {
                        double numHabiles = 0;
                        DateTime fechaEntrega;
                        fechaEntrega = ParametrosNegocio.Instancia.ObtenerFechaFinalHabilSinSabados(fechaRecogida, 1, ParametrosNegocio.Instancia.ConsultarParametrosFramework(ConstantesFramework.PARA_PAIS_DEFAULT));
                        // Rapi AM se  en horas al siguiente día antes de 12 M.
                        TimeSpan diferenciaDeFechasAdm = fechaEntrega - fechadmisionEnvio.Value;
                        numHabiles = diferenciaDeFechasAdm.TotalDays * 24;
                        //int numeroHorasRapiAM = Convert.ToInt32(((numHabiles) - DateTime.Now.Hour) + 12);
                        int numeroHorasRapiAM = Convert.ToInt32(((numHabiles) - fechaRecogida.Hour) + 12);
                        validacionServicioTrayectoDestino.DuracionTrayectoEnHoras = numeroHorasRapiAM;
                        fechaDigitalizacion = ObtenerHorasDigitalizacionParaGuia(fechaEntrega, ref validacionServicioTrayectoDestino, tiempos.numeroDiasDigitalizacion, numeroHorasRapiAM);
                        ObtenerHorasArchivioParaGuia(fechaDigitalizacion, ref validacionServicioTrayectoDestino, tiempos.numeroDiasArchivo, validacionServicioTrayectoDestino.NumeroHorasDigitalizacion);
                        validacionServicioTrayectoDestino.fechaEntrega = fechaEntrega;
                    }
                    else
                    {
                        double numDias = 0;
                        DateTime horaEntregaRapiAM = DateTime.Now;
                        numeroDias += 1;
                        horaEntregaRapiAM = ParametrosNegocio.Instancia.ObtenerFechaFinalHabilSinSabados(DateTime.Now, Convert.ToDouble(numeroDias), ParametrosNegocio.Instancia.ConsultarParametrosFramework(ConstantesFramework.PARA_PAIS_DEFAULT));
                        TimeSpan diferenciaDeFechas;
                        diferenciaDeFechas = horaEntregaRapiAM - DateTime.Now;
                        numDias = diferenciaDeFechas.TotalDays * 24;
                        if (numDias > 0)
                        {
                            numeroHoras = Convert.ToInt32(((numDias) - horaEntregaRapiAM.Hour) + 12);
                            numeroHoras = numeroHoras + (horasRecogida >= 20 ? horasRecogida : 0);
                            validacionServicioTrayectoDestino.DuracionTrayectoEnHoras = numeroHoras;
                        }
                        else
                        {
                            numeroHoras = (24 - DateTime.Now.Hour) + 12;
                            numeroHoras = numeroHoras + (horasRecogida >= 20 ? horasRecogida : 0);
                            validacionServicioTrayectoDestino.DuracionTrayectoEnHoras = numeroHoras;
                        }
                        fechaDigitalizacion = ObtenerHorasDigitalizacionParaGuia(horaEntregaRapiAM, ref validacionServicioTrayectoDestino, tiempos.numeroDiasDigitalizacion, numeroHoras);
                        ObtenerHorasArchivioParaGuia(fechaDigitalizacion, ref validacionServicioTrayectoDestino, tiempos.numeroDiasArchivo, validacionServicioTrayectoDestino.NumeroHorasDigitalizacion);
                        validacionServicioTrayectoDestino.fechaEntrega = fechaDigitalizacion;
                    }
                }
                else if (servicio.IdServicio == TAConstantesServicios.SERVICIO_RAPI_HOY)
                {
                    // Rapi HOY se calcula en horas el mismo día antes de las 6 PM.
                    if (!fechadmisionEnvio.HasValue)
                    {
                        validacionServicioTrayectoDestino.DuracionTrayectoEnHoras = 18 - fechaRecogida.Hour;
                        fechaDigitalizacion = ObtenerHorasDigitalizacionParaGuia(fechaRecogida, ref validacionServicioTrayectoDestino, tiempos.numeroDiasDigitalizacion, (18 - fechaRecogida.Hour));
                        ObtenerHorasArchivioParaGuia(fechaDigitalizacion, ref validacionServicioTrayectoDestino, tiempos.numeroDiasArchivo, validacionServicioTrayectoDestino.NumeroHorasDigitalizacion);
                        validacionServicioTrayectoDestino.fechaEntrega = fechaDigitalizacion;
                    }
                    else
                    {
                        DateTime fecha = DateTime.Now;
                        DateTime fechaEntrega;

                        fechaEntrega = (DateTime)fechadmisionEnvio;
                        fechaEntrega = fechaEntrega.Date.AddHours(18);

                        fechaDigitalizacion = ObtenerHorasDigitalizacionParaGuia(fechaEntrega, ref validacionServicioTrayectoDestino, tiempos.numeroDiasDigitalizacion, (18 - fechaRecogida.Hour));
                        ObtenerHorasArchivioParaGuia(fechaDigitalizacion, ref validacionServicioTrayectoDestino, tiempos.numeroDiasArchivo, validacionServicioTrayectoDestino.NumeroHorasDigitalizacion);
                        validacionServicioTrayectoDestino.fechaEntrega = fechaEntrega;

                    }
                }
                else if (servicio.IdServicio == TAConstantesServicios.SERVICIO_RAPI_CARGA_CONSOLIDADA || servicio.IdServicio == TAConstantesServicios.SERVICIO_RAPI_VALIJAS)
                {
                    double numHabiles = 0;
                    DateTime fechaInicial = fechaRecogida.Date.AddHours(18);
                    DateTime fechaEntrega = ParametrosNegocio.Instancia.ObtenerFechaFinalHabilSinSabados(fechaInicial, Convert.ToDouble(numeroDias), ParametrosNegocio.Instancia.ConsultarParametrosFramework(ConstantesFramework.PARA_PAIS_DEFAULT));
                    TimeSpan diferenciaDeFechas = fechaEntrega - fechaInicial;
                    fechaEntrega = CalculoDomingo(numeroDias, diferenciaDeFechas.Days, fechaEntrega);
                    diferenciaDeFechas = fechaEntrega - fechaInicial;
                    numHabiles = diferenciaDeFechas.Days * 24;
                    numeroHoras = Convert.ToInt32((numHabiles));
                    validacionServicioTrayectoDestino.DuracionTrayectoEnHoras = numeroHoras;
                    fechaDigitalizacion = ObtenerHorasDigitalizacionParaGuia(fechaEntrega, ref validacionServicioTrayectoDestino, tiempos.numeroDiasDigitalizacion, numeroHoras);
                    ObtenerHorasArchivioParaGuia(fechaDigitalizacion, ref validacionServicioTrayectoDestino, tiempos.numeroDiasArchivo, validacionServicioTrayectoDestino.NumeroHorasDigitalizacion);
                    validacionServicioTrayectoDestino.fechaEntrega = fechaEntrega;
                }
                else
                {
                    // Si en el rango de fechas hay fines de semana y/o festivos, deben ser omitidos
                    double numHabiles = 0;
                    DateTime fechaEntrega;
                    fechaEntrega = ParametrosNegocio.Instancia.ObtenerFechaFinalHabilSinSabados(fechaRecogida.Date.AddHours(18), Convert.ToDouble(numeroDias), ParametrosNegocio.Instancia.ConsultarParametrosFramework(ConstantesFramework.PARA_PAIS_DEFAULT));
                    //fechaRecogida.Date.AddHours(18);                    
                    TimeSpan diferenciaDeFechas = fechaEntrega - DateTime.Now.Date.AddHours(18);
                    numHabiles = diferenciaDeFechas.Days * 24;
                    //+ (horasRecogida >= 24 ? horasRecogida : 0)
                    numeroHoras = Convert.ToInt32((numHabiles));
                    validacionServicioTrayectoDestino.DuracionTrayectoEnHoras = numeroHoras;
                    fechaDigitalizacion = ObtenerHorasDigitalizacionParaGuia(fechaEntrega, ref validacionServicioTrayectoDestino, tiempos.numeroDiasDigitalizacion, numeroHoras);
                    ObtenerHorasArchivioParaGuia(fechaDigitalizacion, ref validacionServicioTrayectoDestino, tiempos.numeroDiasArchivo, validacionServicioTrayectoDestino.NumeroHorasDigitalizacion);
                    validacionServicioTrayectoDestino.fechaEntrega = fechaEntrega;
                }
                ParametrosNegocio.Instancia.ObtenerInformacionValidacionTrayecto(municipioDestino, validacionServicioTrayectoDestino, centroServiciosOrigen, municipioOrigen);
                validacionServicioTrayectoDestino.ValoresAdicionales = new List<TAValorAdicional>();
            }
            else
            {
                // Se obtiene Operador postal del destino (aplica para internacional)
                PAOperadorPostal operadorPostal = MensajeriaRepository.Instancia.ObtenerOperadorPostalLocalidad(municipioDestino.IdLocalidad);
                if (operadorPostal != null)
                {
                    validacionServicioTrayectoDestino.IdOperadorPostalDestino = operadorPostal.Id;
                    validacionServicioTrayectoDestino.IdZonaOperadorPostalDestino = operadorPostal.IdZona;
                    validacionServicioTrayectoDestino.DuracionTrayectoEnHoras = operadorPostal.TiempoEntrega * 24; // Porque el tiempo de entrega por zona se da en días
                }
                //Cuando es internacional se valida con la ciudad de destino Bogota
                validacionServicioTrayectoDestino.CodigoPostalDestino = "11001000";
                ParametrosNegocio.Instancia.ObtenerInformacionValidacionTrayectoOrigen(municipioOrigen, validacionServicioTrayectoDestino);
                validacionServicioTrayectoDestino.ValoresAdicionales = new List<TAValorAdicional>();
            }
            return validacionServicioTrayectoDestino;
        }
        /// <summary>
        /// Calculo fecha de entrega Domingo
        /// </summary>
        /// <param name="diasConfigurados">dias configurados para el servicio especifico</param>
        /// <param name="diasEstimadoEntrega">dias fecha estimada de entrega antes de ver proximo domingo</param>
        /// <param name="fechaEstimadaDeEntrega">fecha estimada de entrega antes de ver proximo domingo</param>
        /// <returns></returns>
        public DateTime CalculoDomingo(int diasConfigurados, int diasEstimadoEntrega, DateTime fechaEstimadaDeEntrega)
        {
            int diaSemana = (int)fechaEstimadaDeEntrega.DayOfWeek;
            int diasAtrasADomingo = diaSemana;
            int diasAdelanteADomingo = (7 - diaSemana) % 7;
            DateTime domingoAnterior = fechaEstimadaDeEntrega.AddDays(-diasAtrasADomingo);
            int diasConDomingoAnterior = diasEstimadoEntrega - diasAtrasADomingo;
            if (diasConDomingoAnterior >= diasConfigurados)
                return domingoAnterior;
            DateTime domingoSiguiente = fechaEstimadaDeEntrega.AddDays(diasAdelanteADomingo);
            return domingoSiguiente;
        }

        /// <summary>
        /// Consulta si una localidad tiene centros de servicio que acepten pago en casa (Contrapago)
        /// </summary>
        /// <param name="idLocalidad">Id de la localidad que se va a consultar</param>
        /// <returns>Bool, existe o no existe CS con pago en casa para esa localidad</returns>
        public bool LocalidadTieneCSConPagoEnCasa(string idLocalidad)
        {
            return MensajeriaRepository.Instancia.LocalidadTieneCSConPagoEnCasa(idLocalidad);
        }

        /// <summary>
        /// Retorna los rangos de peso y casilleros por un trayecto dado (ciudad origen y ciudad destino)
        /// </summary>
        /// <param name="idLocalidadOrigen"></param>
        /// <param name="idLocalidadDestino"></param>
        /// <returns></returns>
        private ADRangoTrayecto ConsultarCasilleroTrayecto(string idLocalidadOrigen, string idLocalidadDestino)
        {
            if (TrayectosCasillero == null)
            {
                TrayectosCasillero = MensajeriaRepository.Instancia.ObtenerCasillerosTrayectos();
            }
            return TrayectosCasillero.FirstOrDefault(t => t.IdLocalidadOrigen == idLocalidadOrigen && t.IdLocalidadDestino == idLocalidadDestino);

        }

        /// <summary>
        /// Obtiene las horas de digitalizacion para una guia
        /// </summary>
        /// <param name="fechaEntrega"></param>
        /// <param name="validacionServicioTrayectoDestino"></param>
        /// <param name="tiempo"></param>
        /// <param name="numeroHoras"></param>
        /// <returns></returns>
        private DateTime ObtenerHorasDigitalizacionParaGuia(DateTime fechaEntrega, ref ADValidacionServicioTrayectoDestino validacionServicioTrayectoDestino, double tiempo, int numeroHoras)
        {
            int numeroHorasNuevo = 0;
            int numeroDeSabados = 0;
            double numHabilesDigitalizacion = 0;
            DateTime FechaDigitalizacion;
            FechaDigitalizacion = ParametrosNegocio.Instancia.ObtenerFechaFinalHabil(fechaEntrega, tiempo, ParametrosNegocio.Instancia.ConsultarParametrosFramework(ConstantesFramework.PARA_PAIS_DEFAULT));
            TimeSpan diferenciaDeFechas = FechaDigitalizacion - fechaEntrega;
            numHabilesDigitalizacion = (diferenciaDeFechas.Days * 24) + diferenciaDeFechas.Hours;
            numeroDeSabados = ContadorSabados(fechaEntrega, FechaDigitalizacion);
            numHabilesDigitalizacion = numHabilesDigitalizacion + (0.5 * numeroDeSabados);
            numeroHorasNuevo = Convert.ToInt32((numHabilesDigitalizacion) + numeroHoras);
            validacionServicioTrayectoDestino.NumeroHorasDigitalizacion = numeroHorasNuevo;
            return FechaDigitalizacion;
        }

        /// <summary>
        /// Obtiene las horas para archvar una guia
        /// </summary>
        /// <param name="fechaEntrega"></param>
        /// <param name="validacionServicioTrayectoDestino"></param>
        /// <param name="tiempo"></param>
        /// <param name="numeroHoras"></param>

        private void ObtenerHorasArchivioParaGuia(DateTime fechaEntrega, ref ADValidacionServicioTrayectoDestino validacionServicioTrayectoDestino, double tiempo, int numeroHoras)
        {
            int numeroHorasNuevo = 0;
            int numeroDeSabados = 0;
            double numHabilesDigitalizacion = 0;
            DateTime FechaArchivo;
            FechaArchivo = ParametrosNegocio.Instancia.ObtenerFechaFinalHabil(fechaEntrega, tiempo, ParametrosNegocio.Instancia.ConsultarParametrosFramework(ConstantesFramework.PARA_PAIS_DEFAULT));
            TimeSpan diferenciaDeFechas = FechaArchivo - fechaEntrega;
            numeroDeSabados = ContadorSabados(fechaEntrega, FechaArchivo);
            numHabilesDigitalizacion = (diferenciaDeFechas.Days * 24) + diferenciaDeFechas.Hours;
            numHabilesDigitalizacion = FechaArchivo.DayOfWeek == DayOfWeek.Saturday ? numHabilesDigitalizacion - 0.25 : numHabilesDigitalizacion + (0.5 * numeroDeSabados);
            numeroHorasNuevo = Convert.ToInt32((numHabilesDigitalizacion) + numeroHoras);
            validacionServicioTrayectoDestino.NumeroHorasArchivo = numeroHorasNuevo;
        }

        /// <summary>
        /// Obtiene los sabados entre una fecha y otra
        /// </summary>
        /// <returns></returns>
        private int ContadorSabados(DateTime fechaInicio, DateTime fechaFin)
        {
            int cuentaSabados = 0;
            fechaInicio = new DateTime(fechaInicio.Year, fechaInicio.Month, fechaInicio.Day);
            fechaFin = new DateTime(fechaFin.Year, fechaFin.Month, fechaFin.Day);
            while (fechaInicio <= fechaFin)
            {
                if (fechaInicio.DayOfWeek == 0)
                {
                    cuentaSabados++;
                }
                fechaInicio = fechaInicio.AddDays(1);

            }
            return cuentaSabados;
        }


        /// <summary>
        /// Obtiene el precio del servicio carga express
        /// </summary>
        /// <param name="idServicio"></param>
        /// <param name="idListaPrecio"></param>
        /// <param name="idLocalidadOrigen"></param>
        /// <param name="idLocalidadDestino"></param>
        /// <param name="peso"></param>
        /// <param name="valorDeclarado"></param>
        /// <returns></returns>
        public TAPrecioMensajeriaDC CalcularPrecioCargaExpress(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1")
        {

            TAPrecioMensajeriaDC valorPeso = MensajeriaRepository.Instancia.ObtenerPrecioMensajeria(TAConstantesServicios.SERVICIO_CARGA_EXPRESS, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);

            TAPrecioMensajeriaDC precio = new TAPrecioMensajeriaDC()
            {
                Impuestos = MensajeriaRepository.Instancia.ObtenerValorImpuestosServicio(TAConstantesServicios.SERVICIO_CARGA_EXPRESS).ToList(),
                ValorKiloInicial = valorPeso.ValorKiloInicial,
                ValorKiloAdicional = valorPeso.ValorKiloAdicional,
                Valor = valorPeso.Valor,
                ValorPrimaSeguro = valorPeso.ValorPrimaSeguro
            };

            return precio;

        }

        /// <summary>
        /// Método que retorna parametros de la tabla parametrosAdmision_MEN
        /// </summary>
        /// <returns>Valor mínimo declarado para servicio Rapicarga</returns>
        public decimal ObtenerParametrosAdmisiones(string parametro)
        {
            return Convert.ToDecimal(MensajeriaRepository.Instancia.ObtenerParametrosAdmisiones(parametro));
        }

        /// <summary>
        /// Calcula precio
        /// </summary>
        /// <param name="idServicio">Identificador servicio</param>
        /// <param name="idListaPrecio">Identificador id lista de precio</param>
        /// <param name="idLocalidadOrigen">Identificador ciudad de origen</param>
        /// <param name="idLocalidadDestino">Identificador ciudad de destino</param>
        /// <param name="peso">Peso</param>
        /// <returns>Objeto valor</returns>
        public TAPrecioMensajeriaDC CalcularPrecioCargaAerea(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision, string idTipoEntrega = "-1")
        {
            TAPrecioMensajeriaDC valorPeso = MensajeriaRepository.Instancia.ObtenerPrecioMensajeria(TAConstantesServicios.SERVICIO_CARGA_AEREA, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);

            TAPrecioMensajeriaDC precio = new TAPrecioMensajeriaDC()
            {
                Impuestos = MensajeriaRepository.Instancia.ObtenerValorImpuestosServicio(TAConstantesServicios.SERVICIO_CARGA_AEREA).ToList(),
                ValorKiloInicial = valorPeso.ValorKiloInicial,
                ValorKiloAdicional = valorPeso.ValorKiloAdicional,
                Valor = valorPeso.Valor,
                ValorPrimaSeguro = valorPeso.ValorPrimaSeguro
            };

            return precio;
        }


        /// <summary>
        /// Calcula precio
        /// </summary>
        /// <param name="idServicio">Identificador servicio</param>
        /// <param name="idListaPrecio">Identificador id lista de precio</param>
        /// <param name="idLocalidadOrigen">Identificador ciudad de origen</param>
        /// <param name="idLocalidadDestino">Identificador ciudad de destino</param>
        /// <param name="peso">Peso</param>
        /// <returns>Objeto valor</returns>
        public TAPrecioMensajeriaDC CalcularPrecioGeneral(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision, string idTipoEntrega = "-1")
        {
            TAPrecioMensajeriaDC valorPeso = MensajeriaRepository.Instancia.ObtenerPrecioMensajeria(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);

            TAPrecioMensajeriaDC precio = new TAPrecioMensajeriaDC()
            {
                Impuestos = MensajeriaRepository.Instancia.ObtenerValorImpuestosServicio(idServicio).ToList(),
                ValorKiloInicial = valorPeso.ValorKiloInicial,
                ValorKiloAdicional = valorPeso.ValorKiloAdicional,
                Valor = valorPeso.Valor,
                ValorPrimaSeguro = valorPeso.ValorPrimaSeguro
            };

            return precio;
        }

        /// <summary>
        /// Obtiene el precio del servicio rapi carga
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        public TAPrecioCargaDC CalcularPrecioRapiCarga(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision, string idTipoEntrega = "-1")
        {
            string idListaPrecioServicio = MensajeriaRepository.Instancia.ObtenerIdentificadorListaPrecioServicio(idServicio, idListaPrecio);
            int idLp = int.Parse(idListaPrecioServicio);

            TAPrecioCargaDC precioValor = MensajeriaRepository.Instancia.ObtenerPrecioRapiCarga(idServicio, idListaPrecio, idLp, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);

            TAPrecioCargaDC precio = new TAPrecioCargaDC()
            {
                Impuestos = MensajeriaRepository.Instancia.ObtenerValorImpuestosServicio(idServicio).ToList(),
                ValorKiloAdicional = precioValor.ValorKiloAdicional,
                ValorServicioRetorno = precioValor.ValorServicioRetorno,
                Valor = precioValor.Valor,
                ValorPrimaSeguro = precioValor.ValorPrimaSeguro
            };

            return precio;
        }


        #endregion

        #region GENERAR PDF GUIA
        /// <summary>
        /// Descarga el pdf de forma directa por URL
        /// </summary>
        /// <param name="numeroGuia"></param>
        public void DescargarPDFGuia(string numeroGuia)
        {
            ADRastreoGuiaDC rastreoGuia = ObtenerRastreoGuias(numeroGuia).SingleOrDefault();
            if (rastreoGuia != null)
            {
                using (System.IO.MemoryStream memoryStream = new MemoryStream())
                {
                    BaseFont bf = BaseFont.CreateFont();
                    Document document = new Document(PageSize.A5.Rotate(), 0, 0, 0, 0);
                    PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                    //writer.CompressionLevel = PdfStream.BEST_COMPRESSION;
                    //PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(@"C:\tempo\prueba.pdf", FileMode.OpenOrCreate));

                    GenerarPDF(rastreoGuia, writer, bf, document, "PruebaEntrega");
                    document.NewPage();
                    GenerarPDF(rastreoGuia, writer, bf, document, "DESTINATARIO");
                    document.NewPage();
                    GenerarPDF(rastreoGuia, writer, bf, document, "REMITENTE");

                    document.Close();

                    byte[] bytes = memoryStream.ToArray();
                    memoryStream.Close();

                    string nombrePDF = "Prueba";
                    HttpContext.Current.Response.ContentType = "application/force-download";
                    HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename = " + nombrePDF + ".pdf");
                    HttpContext.Current.Response.BinaryWrite(bytes);
                    HttpContext.Current.Response.End();
                }
            }
        }

        /// <summary>
        /// Genera el PDF de la guia y lo guarda en la FTP
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public string GenerarPDFGuia(string numeroGuia, bool obtenerString)
        {
            string ftpAddress = ParametrosNegocio.Instancia.ConsultarParametrosFramework("ftpGeneracionGuias");
            string username = ParametrosNegocio.Instancia.ConsultarParametrosFramework("UserftpGeneracionGuias");
            string password = ParametrosNegocio.Instancia.ConsultarParametrosFramework("PassftpGeneracionGuias");
            string resultado = "";

            string uri = String.Format("{0}/{1}", ftpAddress, numeroGuia + ".pdf");
            FtpWebRequest reqFTP = (FtpWebRequest)FtpWebRequest.Create(uri);
            reqFTP.Credentials = new NetworkCredential(username, password);
            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
            reqFTP.KeepAlive = false;
            reqFTP.UsePassive = false;
            reqFTP.UseBinary = true;

            try
            {
                ADRastreoGuiaDC rastreoGuia = ObtenerRastreoGuias(numeroGuia).SingleOrDefault();
                if (rastreoGuia != null)
                {
                    using (System.IO.MemoryStream memoryStream = new MemoryStream())
                    {
                        BaseFont bf = BaseFont.CreateFont();
                        Document document = new Document(PageSize.A5.Rotate(), 0, 0, 0, 0);
                        PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                        writer.CompressionLevel = PdfStream.BEST_COMPRESSION;

                        GenerarPDF(rastreoGuia, writer, bf, document, "PruebaEntrega");
                        document.NewPage();
                        GenerarPDF(rastreoGuia, writer, bf, document, "DESTINATARIO");
                        document.NewPage();
                        GenerarPDF(rastreoGuia, writer, bf, document, "REMITENTE");
                        document.Close();

                        byte[] bytes = memoryStream.ToArray();
                        memoryStream.Close();

                        Stream requestStream = reqFTP.GetRequestStream();
                        requestStream.Write(bytes, 0, bytes.Length);
                        requestStream.Flush();
                        requestStream.Close();
                    }
                    if (obtenerString)
                    {
                        resultado = DownloadFile(username, password, uri, numeroGuia);
                    }
                    else
                    {
                        resultado = "OK";
                    }
                }
                else
                {
                    resultado = "El numero de Guia no se encuentra registrado";
                }
            }
            catch (Exception ex)
            {
                resultado = "El numero de Guia no se encuentra registrado";
            }

            return resultado;
        }

        public string DownloadFile(string username, string password, string uri, string fileName)
        {
            FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(uri);
            ftpRequest.Credentials = new NetworkCredential(username, password);
            ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
            FtpWebResponse ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
            Stream stream = ftpResponse.GetResponseStream();
            byte[] documentoArray = Utilidades.ReadToEnd(stream);
            stream.Close();
            string documentoString = Convert.ToBase64String(documentoArray);
            return documentoString;

            //Borrado Archivo
            //FtpWebRequest request = (FtpWebRequest)WebRequest.Create(server + document);
            //request.Credentials = credentials;
            //request.Method = WebRequestMethods.Ftp.DeleteFile;
            //request.GetResponse();
        }

        /// <summary>
        /// Genera cada una de las hojas pertenecientes al PDF de la Guia
        /// </summary>
        /// <param name="rastreoGuia"></param>
        /// <param name="writer"></param>
        /// <param name="bf"></param>
        /// <param name="document"></param>
        /// <param name="tipo"></param>
        protected void GenerarPDF(ADRastreoGuiaDC rastreoGuia, PdfWriter writer, BaseFont bf, Document document, string tipo)
        {
            iTextSharp.text.Image imagen = null;
            int coordenadaObservacionX = 0;
            int coordenadaObservacionY = 0;
            int coordenadaGMCX = 0;
            int coordenadaGMCY = 0;
            int coordenadaNGuiaX = 0;
            int coordenadaNGuiaY = 0;
            string textoNumeroGuiaFinal = "";
            string textoGMC = "";

            document.Open();
            PdfContentByte fondo = writer.DirectContentUnder;
            PdfContentByte pcbPE = writer.DirectContent;
            MensajeriaRepository repositorio = new MensajeriaRepository();

            if (tipo == "PruebaEntrega")
            {
                imagen = Image.GetInstance(Recursos.FormatoMCPruebaDeEntrega, System.Drawing.Imaging.ImageFormat.Jpeg);
                coordenadaObservacionX = 500;
                coordenadaObservacionY = 215;
                coordenadaGMCX = 559;
                coordenadaGMCY = 33;
                coordenadaNGuiaX = 568;
                coordenadaNGuiaY = 215;
                textoNumeroGuiaFinal = rastreoGuia.Guia.NumeroGuia.ToString() + "                                                                           " + "PRUEBA DE ENTREGA";
                textoGMC = "GMC-GMC-R-07";
            }
            else
            {
                imagen = Image.GetInstance(Recursos.FormatoMCDestinatario, System.Drawing.Imaging.ImageFormat.Jpeg);
                coordenadaObservacionX = 313;
                coordenadaObservacionY = 16;
                coordenadaGMCX = 561;
                coordenadaGMCY = 33;
                coordenadaNGuiaX = 550;
                coordenadaNGuiaY = 215;
                textoNumeroGuiaFinal = rastreoGuia.Guia.NumeroGuia.ToString();
                textoGMC = "GMC-GMC-R-07                                                                                                                                                                 " + tipo;

            }

            //Recuadro Forma de pago
            pcbPE.SaveState();
            pcbPE.SetColorFill(BaseColor.BLACK);
            pcbPE.Rectangle(215, 325, 10, 75);
            Rectangle figura = new Rectangle(20, 20, 20, 20);
            pcbPE.Fill();
            pcbPE.RestoreState();

            #region Inicio
            iTextSharp.text.Image imagenPE = imagen;
            imagenPE.Rotate();
            imagenPE.RotationDegrees = 90;
            imagenPE.ScaleAbsolute(420, 590);
            imagenPE.SetAbsolutePosition(0, 15);

            fondo.SaveState();
            fondo.AddImage(imagenPE);
            fondo.RestoreState();

            pcbPE.BeginText();

            //Fecha Admision
            pcbPE.SetFontAndSize(bf, 8);
            pcbPE.ShowTextAligned(Element.ALIGN_CENTER, rastreoGuia.Guia.FechaAdmision.ToString(), 73, 124, 90);

            //Tiempo Entrega
            pcbPE.SetFontAndSize(bf, 8);
            pcbPE.ShowTextAligned(Element.ALIGN_CENTER, rastreoGuia.Guia.FechaEstimadaEntrega.ToString(), 87, 124, 90);

            //Mensajeria
            pcbPE.SaveState();
            pcbPE.SetTextRenderingMode(PdfContentByte.TEXT_RENDER_MODE_FILL_STROKE);
            pcbPE.ShowTextAligned(Element.ALIGN_BOTTOM, "MENSAJERÌA", 100, 240, 90);
            pcbPE.RestoreState();

            //Codigo de Barras
            Barcode128 codigoPE = new Barcode128();
            codigoPE.CodeType = Barcode.CODE128;
            codigoPE.Code = rastreoGuia.Guia.NumeroGuia.ToString();
            Image imgCodigoPE = codigoPE.CreateImageWithBarcode(pcbPE, BaseColor.BLACK, BaseColor.BLACK);
            imgCodigoPE.RotationDegrees = 90;
            imgCodigoPE.SetAbsolutePosition(55, 210);
            pcbPE.SaveState();
            pcbPE.AddImage(imgCodigoPE);
            pcbPE.RestoreState();

            //Casillero
            pcbPE.SetFontAndSize(bf, 15);
            pcbPE.ShowTextAligned(Element.ALIGN_BOTTOM, rastreoGuia.Guia.MotivoNoUsoBolsaSeguriDesc.ToString(), 107, 16, 90);
            #endregion

            #region Destinatario
            //CiudadEntrega
            pcbPE.SaveState();
            pcbPE.SetFontAndSize(bf, 12);
            pcbPE.SetTextRenderingMode(PdfContentByte.TEXT_RENDER_MODE_FILL_STROKE);
            pcbPE.ShowTextAligned(Element.ALIGN_BOTTOM, rastreoGuia.Guia.NombreCiudadDestino, 130, 16, 90);
            pcbPE.RestoreState();

            //Nombre Destinatario
            pcbPE.SaveState();
            pcbPE.SetFontAndSize(bf, 10);
            pcbPE.ShowTextAligned(Element.ALIGN_BOTTOM, rastreoGuia.Guia.Destinatario.Nombre, 140, 16, 90);
            pcbPE.RestoreState();

            //Direccion
            pcbPE.SaveState();
            pcbPE.SetFontAndSize(bf, 8);
            pcbPE.ShowTextAligned(Element.ALIGN_BOTTOM, rastreoGuia.Guia.Destinatario.Direccion, 149, 16, 90);
            pcbPE.RestoreState();

            //Celular Origen
            pcbPE.SaveState();
            pcbPE.SetFontAndSize(bf, 8);
            pcbPE.ShowTextAligned(Element.ALIGN_BOTTOM, rastreoGuia.Guia.Destinatario.Telefono, 157, 16, 90);
            pcbPE.RestoreState();

            //Correo Origen
            pcbPE.SaveState();
            pcbPE.SetFontAndSize(bf, 8);
            pcbPE.ShowTextAligned(Element.ALIGN_BOTTOM, rastreoGuia.Guia.Destinatario.Email, 157, 115, 90);
            pcbPE.RestoreState();


            #endregion

            #region Datos del Envio
            //Tipo de empaque
            pcbPE.SaveState();
            pcbPE.SetFontAndSize(bf, 8);
            pcbPE.ShowTextAligned(Element.ALIGN_BOTTOM, rastreoGuia.Guia.NombreTipoEnvio.ToString(), 174, 70, 90);
            pcbPE.RestoreState();

            //Valor Comercial
            pcbPE.SaveState();
            pcbPE.SetFontAndSize(bf, 8);
            pcbPE.ShowTextAligned(Element.ALIGN_BOTTOM, string.Format("{0:n}", rastreoGuia.Guia.ValorDeclarado), 183, 70, 90);
            pcbPE.RestoreState();

            //Numero Pieza
            pcbPE.SaveState();
            pcbPE.SetFontAndSize(bf, 8);
            pcbPE.ShowTextAligned(Element.ALIGN_BOTTOM, rastreoGuia.Guia.NumeroPieza.ToString(), 190, 70, 90);
            pcbPE.RestoreState();

            //Peso por Volumen
            pcbPE.SaveState();
            pcbPE.SetFontAndSize(bf, 8);
            pcbPE.ShowTextAligned(Element.ALIGN_BOTTOM, rastreoGuia.Guia.EsPesoVolumetrico ? string.Format("{0:n}", rastreoGuia.Guia.Peso) : "0", 200, 70, 90);
            pcbPE.RestoreState();

            //Peso en kilos
            pcbPE.SaveState();
            pcbPE.SetFontAndSize(bf, 8);
            pcbPE.ShowTextAligned(Element.ALIGN_BOTTOM, !rastreoGuia.Guia.EsPesoVolumetrico ? string.Format("{0:n}", rastreoGuia.Guia.Peso) : "0", 208, 70, 90);
            pcbPE.RestoreState();

            //Bolsa de Seguridad
            pcbPE.SaveState();
            pcbPE.SetFontAndSize(bf, 8);
            pcbPE.ShowTextAligned(Element.ALIGN_BOTTOM, rastreoGuia.Guia.NumeroBolsaSeguridad.ToString(), 217, 70, 90);
            pcbPE.RestoreState();

            //Dice Contener
            pcbPE.SaveState();
            pcbPE.SetFontAndSize(bf, 8);
            pcbPE.ShowTextAligned(Element.ALIGN_BOTTOM, rastreoGuia.Guia.DiceContener, 227, 70, 90);
            pcbPE.RestoreState();

            #endregion

            #region Liquidacion de Envio
            //Liquidacion
            pcbPE.SaveState();
            pcbPE.SetFontAndSize(bf, 8);
            pcbPE.ShowTextAligned(Element.ALIGN_BOTTOM, "Mensajerìa", 174, 205, 90);
            pcbPE.RestoreState();

            //Valor Transporte
            pcbPE.SaveState();
            pcbPE.SetFontAndSize(bf, 8);
            pcbPE.ShowTextAligned(Element.ALIGN_BOTTOM, string.Format("{0:n}", rastreoGuia.Guia.ValorServicio), 185, 325, 90);
            pcbPE.RestoreState();

            //Valor Flete
            pcbPE.SaveState();
            pcbPE.SetFontAndSize(bf, 8);
            pcbPE.ShowTextAligned(Element.ALIGN_BOTTOM, string.Format("{0:n}", rastreoGuia.Guia.ValorPrimaSeguro), 195, 325, 90);
            pcbPE.RestoreState();

            //Valor otros conceptos
            pcbPE.SaveState();
            pcbPE.SetFontAndSize(bf, 8);
            pcbPE.ShowTextAligned(Element.ALIGN_BOTTOM, "0", 202, 325, 90);
            pcbPE.RestoreState();

            //Valor Total
            pcbPE.SaveState();
            pcbPE.SetFontAndSize(bf, 8);
            pcbPE.ShowTextAligned(Element.ALIGN_BOTTOM, string.Format("{0:n}", rastreoGuia.Guia.ValorTotal), 212, 325, 90);
            pcbPE.RestoreState();

            //Forma de Pago
            //pcbPE.SaveState();
            //pcbPE.SetColorFill(BaseColor.BLACK);
            //pcbPE.Rectangle(215, 325, 10, 75);
            //Rectangle figura = new Rectangle(20, 20, 20, 20);
            //pcbPE.Fill();
            //pcbPE.RestoreState();

            pcbPE.SaveState();
            pcbPE.SetFontAndSize(bf, 9);
            pcbPE.SetColorFill(BaseColor.WHITE);
            pcbPE.ShowTextAligned(Element.ALIGN_BOTTOM, rastreoGuia.Guia.FormasPagoDescripcion.ToUpper(), 222, 325, 90);
            pcbPE.RestoreState();
            #endregion

            #region Remitente
            //Nombre
            pcbPE.SaveState();
            pcbPE.SetFontAndSize(bf, 8);
            pcbPE.ShowTextAligned(Element.ALIGN_BOTTOM, rastreoGuia.Guia.Remitente.Nombre + "         " + "CC" + "     " + rastreoGuia.Guia.Remitente.Identificacion, 245, 16, 90);
            pcbPE.RestoreState();

            //Direccion
            pcbPE.SaveState();
            pcbPE.SetFontAndSize(bf, 8);
            pcbPE.ShowTextAligned(Element.ALIGN_BOTTOM, rastreoGuia.Guia.Remitente.Direccion, 255, 16, 90);
            pcbPE.RestoreState();

            //Telefeono
            pcbPE.SaveState();
            pcbPE.SetFontAndSize(bf, 8);
            pcbPE.ShowTextAligned(Element.ALIGN_BOTTOM, rastreoGuia.Guia.Remitente.Telefono + "            " + rastreoGuia.Guia.Remitente.Email, 265, 16, 90);
            pcbPE.RestoreState();

            //Ciudad
            pcbPE.SaveState();
            pcbPE.SetFontAndSize(bf, 8);
            pcbPE.ShowTextAligned(Element.ALIGN_BOTTOM, rastreoGuia.Guia.IdCiudadOrigen, 275, 16, 90);
            pcbPE.RestoreState();

            #endregion

            #region Campos Finales
            //Observaciones
            pcbPE.SaveState();
            pcbPE.SetFontAndSize(bf, 5);
            pcbPE.ShowTextAligned(Element.ALIGN_BOTTOM, rastreoGuia.Guia.Observaciones, coordenadaObservacionX, coordenadaObservacionY, 90);
            pcbPE.RestoreState();

            //Numero Guia Final
            pcbPE.SaveState();
            pcbPE.SetFontAndSize(bf, 5);
            pcbPE.ShowTextAligned(Element.ALIGN_BOTTOM, textoNumeroGuiaFinal, coordenadaNGuiaX, coordenadaNGuiaY, 90);
            pcbPE.RestoreState();

            //GMC
            pcbPE.SaveState();
            pcbPE.SetFontAndSize(bf, 6);
            pcbPE.ShowTextAligned(Element.ALIGN_BOTTOM, textoGMC, coordenadaGMCX, coordenadaGMCY, 90);
            pcbPE.RestoreState();

            //Imagen Publicidad
            if (tipo != "PruebaEntrega")
            {
                string img = repositorio.ObtenerParametrosAdmisiones("ImagenPublicidadGuia");
                iTextSharp.text.Image imagenPublicidad = Image.GetInstance(img);
                PdfContentByte imgPublicidad = writer.DirectContent;
                imagenPublicidad.Rotate();
                imagenPublicidad.RotationDegrees = 90;
                imagenPublicidad.ScaleToFit(220, 355);
                imagenPublicidad.SetAbsolutePosition(375, 18);

                imgPublicidad.SaveState();
                imgPublicidad.AddImage(imagenPublicidad);
                imgPublicidad.RestoreState();
            }
            #endregion

            pcbPE.EndText();
            writer.Flush();
        }

        public IList<string> ObtenerFestivosAnio()
        {
            return ParametrosNegocio.Instancia.ObtenerFestivosAnio();
        }

        /// <summary>
        /// Obtiene El valor comercial dependiento del peso
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ValorComercialResponse ConsultarValorComercialPeso(int peso)
        {
            return TarifasRepository.Instancia.ConsultarValorComercialPeso(peso);
        }

        public object[] ObtenerPdfImpresion(long numeroGuia)
        {
            FacturacionRequest respuesta = null;
            byte[] pdf = null;
            ADFormatoImpresion formato = null;

            //Consumo de servicio para obtener Guia o Giro
            String URIAdmisionServer = ConfigurationManager.AppSettings.Get("URIApiController");

            if (String.IsNullOrEmpty(URIAdmisionServer))
            {
                throw new Exception("Url servidor admision no encontrado en configuración");
            }

            string hostAdmision = URIAdmisionServer;

            var clientGuia = new RestClient(hostAdmision);
            string uri = "Factura/ObtenerAdmisionoGiro/" + numeroGuia.ToString();
            var requestGuia = new RestRequest(uri, Method.GET);
            requestGuia.AddHeader("Content-Type", "application/json");
            requestGuia.AddHeader("Usuario", "Admin");
            IRestResponse responseMessage = clientGuia.Execute(requestGuia);

            ADGuia guia = null;
            GIAdmisionGirosDC giro = null;
            if (responseMessage.StatusCode.Equals(System.Net.HttpStatusCode.OK) && responseMessage.Content != null)
            {
                respuesta = JsonConvert.DeserializeObject<FacturacionRequest>(responseMessage.Content);
                guia = respuesta.guia;
                giro = respuesta.giro;
            }

            if (guia != null && guia.NumeroGuia > 0)
            {
                guia.PesoKilos = 0;

                //Obtener Resolucion Dian
                DIIntegracionFactura RD = new DIIntegracionFactura()
                {
                    IdServicio = (int)ADEnumTipoServicio.Giro,
                    IdTiposuministro = 0,
                    IdCentroServicio = guia.IdCentroServicioOrigen,
                    IdTercero = null,
                    NumeroIdentificacion = guia.Remitente.Identificacion,
                    IdAplicacion = 1,
                    NumeroDocumento = guia.NumeroGuia,
                    EsManual = !guia.EsAutomatico,
                    NumeroCelular = guia.TelefonoDestinatario,
                    CreadoPor = guia.CreadoPor
                };

                guia.PuertaCasillero = "";

                //Formato de objeto para enviar a servicio de impresion
                formato = new ADFormatoImpresion();
                formato.NameFile = numeroGuia.ToString();
                formato.Template = "MEN-GUIASMS";
                formato.BarCode = numeroGuia.ToString();
                //formato.Print = correo != string.Empty ? 0 : 1;
                formato.Print = 0;
                formato.ItemsFormat = new Utilidades().MapearCamposImpresion(guia);

                pdf = ObtenerPdf(formato);
            }
            else if (giro != null)
            {
                giro.NombreServicio = "GIRO";
                giro.NombreDestinatario = giro.GirosPeatonPeaton.ClienteDestinatario.Nombre;
                giro.NombreRemitente = giro.GirosPeatonPeaton.ClienteRemitente.Nombre;

                DIIntegracionFactura RD = new DIIntegracionFactura()
                {
                    IdServicio = (int)ADEnumTipoServicio.Giro,
                    IdTiposuministro = 0,
                    IdCentroServicio = giro.AgenciaOrigen.IdCentroServicio,
                    IdTercero = null,
                    NumeroIdentificacion = giro.DocumentoRemitente,
                    IdAplicacion = 1,
                    NumeroDocumento = Convert.ToInt32(giro.IdAdminGiro),
                    EsManual = false,
                    NumeroCelular = giro.TelefonoRemitente,
                    CreadoPor = giro.UsuarioCreacionGiro
                };

                giro.FacturaDian = ObtenerResolucionDian(RD);

                //Formato de objeto para enviar a servicio de impresion
                formato = new ADFormatoImpresion();
                formato.NameFile = numeroGuia.ToString();
                formato.Template = "GIR-GUIASMS";
                formato.BarCode = numeroGuia.ToString();
                //formato.Print = correo != string.Empty ? 0 : 1;
                formato.Print = 0;
                formato.ItemsFormat = new Utilidades().MapearCamposImpresion(giro);

                pdf = ObtenerPdf(formato);
            }
            return new object[2] { pdf, respuesta };
        }

        /// <summary>
        /// Metodo para consultar un preenvio por el número.
        /// </summary>
        /// <param string="NumeroPreEnvio"></param>
        /// <returns></returns>
        private PreenvioAdmisionWrapper obtenerPreenvioPorNumero(string NumeroPreEnvio)
        {
            String URIPreenvios = ConfigurationManager.AppSettings.Get("UrlServicioPreEnvio");
            string url = $"{URIPreenvios}Admision/PreEnvioNumero?PreEnvio={NumeroPreEnvio}";

            RestClient client = new RestClient(url);
            RestRequest request = new RestRequest(Method.GET);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("Content-Type", "application/json");
            IRestResponse response = client.Execute(request);
            string content = response.Content;

            if (content == string.Empty)
            {
                throw new FaultException("El número de guía no existe");
            }

            return JsonConvert.DeserializeObject<PreenvioAdmisionWrapper>(content);
        }

        /// <summary>
        /// Metodo para consultar un preenvio y retornarlo como objeto ADGuiaPertenencia.
        /// </summary>
        /// <param string="NumeroPreEnvio"></param>
        /// <returns></returns>
        private ADGuiaPertenencia ObtenerPreenvioPertenencia(string NumeroPreEnvio)
        {
            PreenvioAdmisionWrapper preenvio = obtenerPreenvioPorNumero(NumeroPreEnvio);

            return new ADGuiaPertenencia()
            {
                NumeroGuia = preenvio.NumeroPreenvio.ToString(),
                IdentificacionRemitente = preenvio.IdentificacionRemitente,
                TelefonoRemitente = preenvio.TelefonoRemitente,
                IdentificacionDestinatario = preenvio.IdentificacionDestinatario,
                TelefonoDestinatario = preenvio.TelefonoDestinatario
            };
        }

        /// <summary>
        /// Consulta el preenvio por número, y retorna los datos protegidos/desprotegidos
        /// </summary>
        /// <param name="PreEnvio"></param>
        /// <param name="Proteger"></param>
        private ADRastreoGuiaClienteRespuesta obtenerPreenvioProtegido(string NumeroPreEnvio, bool Proteger)
        {
            PreenvioAdmisionWrapper preenvio = obtenerPreenvioPorNumero(NumeroPreEnvio);

            return new ADRastreoGuiaClienteRespuesta()
            {
                TrazaGuia = new ADTrazaGuiaClienteRespuesta()
                {
                    IdEstadoGuia = preenvio.IdEstadoPreenvio,
                    DescripcionEstadoGuia = preenvio.DescripcionEstado,
                    FechaGrabacion = preenvio.FechaGrabacionEstado
                },
                EstadosGuia = new List<ADEstadoGuiaMotivoClienteRespuesta>()
                {
                    new ADEstadoGuiaMotivoClienteRespuesta()
                    {
                        EstadoGuia = new ADTrazaGuiaEstadoGuia()
                        {
                            IdEstadoGuia = 0,
                            DescripcionEstadoGuia = "ENVÍO PENDIENTE POR ADMITIR",
                            Ciudad = preenvio.CiudadOrigen
                        },
                    }
                },
                Guia = new ADGuiaClienteRespuesta()
                {
                    Remitente = new CLClienteContadoClienteRespuesta()
                    {
                        Nombre = Proteger ? null : Cifrado.EncriptarTexto(preenvio.NombreRemitente),
                        Telefono = Proteger ? Cifrado.EncriptarTexto(OcultarTexto(preenvio.TelefonoRemitente)) : Cifrado.EncriptarTexto(preenvio.TelefonoRemitente),
                        Identificacion = Proteger ? Cifrado.EncriptarTexto(OcultarTexto(preenvio.IdentificacionRemitente)) : Cifrado.EncriptarTexto(preenvio.IdentificacionRemitente),
                        Direccion = Proteger ? null : Cifrado.EncriptarTexto(preenvio.DireccionRemitente),
                        TipoId = Proteger ? null : Cifrado.EncriptarTexto(preenvio.IdTipoIdentificacionRemitente),
                        Email = Proteger ? null : Cifrado.EncriptarTexto(preenvio.EmailRemitente)
                    },
                    Destinatario = new CLClienteContadoClienteRespuesta()
                    {
                        Nombre = Proteger ? null : Cifrado.EncriptarTexto(preenvio.NombreDestinatario),
                        Telefono = Proteger ? Cifrado.EncriptarTexto(OcultarTexto(preenvio.TelefonoDestinatario)) : Cifrado.EncriptarTexto(preenvio.TelefonoDestinatario),
                        Identificacion = Proteger ? Cifrado.EncriptarTexto(OcultarTexto(preenvio.IdentificacionDestinatario)) : Cifrado.EncriptarTexto(preenvio.IdentificacionDestinatario),
                        Direccion = Proteger ? null : Cifrado.EncriptarTexto(preenvio.DireccionDestinatario),
                        TipoId = Proteger ? null : Cifrado.EncriptarTexto(preenvio.IdTipoIdentificacionDestinatario),
                        Email = Proteger ? null : Cifrado.EncriptarTexto(preenvio.EmailDestinatario)
                    },
                    NumeroGuia = preenvio.NumeroPreenvio,
                    NombreCiudadOrigen = preenvio.NombreCiudadOrigen,
                    NombreCiudadDestino = preenvio.NombreCiudadDestino,
                    FechaEstimadaEntregaNew = preenvio.FechaEstimadaEntrega,
                    FechaAdmision = preenvio.FechaPreenvio,
                    NombreTipoEnvio = preenvio.NombreTipoEnvio,
                    TotalPiezas = preenvio.NumeroPieza,
                    Peso = preenvio.Peso,
                    PesoLiqVolumetrico = preenvio.PesoLiqVolumetrico,
                    NumeroBolsaSeguridad = preenvio.NumeroBolsaSeguridad,
                    DiceContener = preenvio.DiceContener,
                    Observaciones = preenvio.Observaciones,
                    IdServicio = preenvio.IdServicio,
                    NombreServicio = preenvio.NombreServicio,
                    NumeroPieza = preenvio.NumeroPieza,
                    FormasPago = new List<ADGuiaFormaPagoClienteRespuesta>()
                    {
                        new ADGuiaFormaPagoClienteRespuesta()
                        {
                            IdFormaPago = preenvio.IdFormaPago,
                            Descripcion = preenvio.NombreFormaPago
                        }
                    }
                },
                ImagenGuia = null
            };
        }

        private byte[] ObtenerPdf(ADFormatoImpresion formato)
        {
            byte[] pdf = null;
            //Consumo de servicio para obtener pdf
            string urlApi = System.Configuration.ConfigurationManager.AppSettings["URIApiImpresion"];
            try
            {
                var client = new RestClient(urlApi);
                var request = new RestRequest(Method.POST);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("Content-Type", "application/json");

                request.AddParameter("undefined", JsonConvert.SerializeObject(formato), ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                var content = response.Content;

                if (!response.IsSuccessful)
                {
                    Utilidades.AuditarExcepcion(new Exception(response.StatusCode.ToString()));
                }

                string result = null;
                pdf = new byte[0];
                object responseobj = new object();
                result = content.Replace("\"", string.Empty);
                pdf = Convert.FromBase64String(result);
                return pdf;

            }
            catch (Exception ex)
            {
                //{
                //    IsSuccess = false,
                //    Message = ex.Message,
                //};
            }
            return pdf;
        }

        private ADFacturaDianDC ObtenerResolucionDian(DIIntegracionFactura data)
        {
            String URIApiIntegracion = ConfigurationManager.AppSettings.Get("URIApiIntegracion");

            ADFacturaDianDC resultado = new ADFacturaDianDC();

            var clientRD = new RestClient(URIApiIntegracion);
            string uriRD = "ObtenerFacturaDian";
            var requestRD = new RestRequest(uriRD, Method.POST);
            requestRD.AddHeader("cache-control", "no-cache");
            requestRD.AddHeader("Content-Type", "application/json");

            requestRD.AddParameter("undefined", JsonConvert.SerializeObject(data), ParameterType.RequestBody);
            IRestResponse responseRD = clientRD.Execute(requestRD);
            var contentRD = responseRD.Content;

            if (!responseRD.IsSuccessful)
            {
                Utilidades.AuditarExcepcion(new Exception(responseRD.StatusCode.ToString()));
            }
            else
            {
                resultado = JsonConvert.DeserializeObject<ADFacturaDianDC>(responseRD.Content);
            }
            return resultado;
        }

        /// <summary>
        /// Metodo para descifrar el objeto de solicitud.
        /// </summary>
        private ADRastreoGuiaClienteSolicitud ObtenerADRastreoGuiaClienteSolicitud(ADRastreoGuiaClienteSolicitud guia)
        {
            if (!guia.EncriptaAes)
            {
                ADRastreoGuiaClienteSolicitud guiaSolicitud = new ADRastreoGuiaClienteSolicitud()
                {
                    NumeroGuia = Cifrado.Decrypt(guia.NumeroGuia),
                    NumeroIdentificacion = Cifrado.Decrypt(guia.NumeroIdentificacion),
                    NumeroTelefono = Cifrado.Decrypt(guia.NumeroTelefono)
                };
                return guiaSolicitud;
            }
            else
            {
                ADRastreoGuiaClienteSolicitud guiaSolicitud = new ADRastreoGuiaClienteSolicitud()
                {
                    NumeroGuia = Cifrado.DesencriptarTexto(guia.NumeroGuia),
                    NumeroIdentificacion = Cifrado.DesencriptarTexto(guia.NumeroIdentificacion),
                    NumeroTelefono = Cifrado.DesencriptarTexto(guia.NumeroTelefono)
                };
                return guiaSolicitud;
            }
        }
        /// <summary>
        /// Metodo para descifrar el objeto de solicitud.
        /// </summary>
        private ADRastreoGuiaClienteSolicitud ObtenerADRastreoGuiaClienteSolicitudPortal(ADRastreoGuiaClienteSolicitud guia)
        {
            ADRastreoGuiaClienteSolicitud guiaSolicitud = new ADRastreoGuiaClienteSolicitud()
            {
                EncriptaAes = guia.EncriptaAes,
                NumeroGuia = Cifrado.DesencriptarTexto(guia.NumeroGuia),
                NumeroIdentificacion = Cifrado.DesencriptarTexto(guia.NumeroIdentificacion),
                NumeroTelefono = Cifrado.DesencriptarTexto(guia.NumeroTelefono)
            };

            return guiaSolicitud;
        }
        /// <summary>
        /// Metodo para obtener la pertenencia de la guia desde sigue tu envio.
        /// </summary>
        private bool ObtenerPertenenciaGuiaSigueTuEnvio(int? opcion, ADGuiaPertenencia guiaPertenencia, ADRastreoGuiaClienteSolicitud guiaSolicitud)
        {
            bool pertenece = false;
            switch (opcion)
            {
                case 1:
                    if (guiaPertenencia.IdentificacionRemitente == guiaSolicitud.NumeroIdentificacion)
                    {
                        pertenece = true;
                    }
                    break;
                case 2:
                    if (guiaPertenencia.TelefonoRemitente == guiaSolicitud.NumeroTelefono)
                    {
                        pertenece = true;
                    }
                    break;
                case 3:
                    if (guiaPertenencia.IdentificacionDestinatario == guiaSolicitud.NumeroIdentificacion)
                    {
                        pertenece = true;
                    }
                    break;
                case 4:
                    if (guiaPertenencia.TelefonoDestinatario == guiaSolicitud.NumeroTelefono)
                    {
                        pertenece = true;
                    }
                    break;
            }

            return pertenece;
        }

        /// <summary>
        /// Metodo para obtener la pertenencia de la guia desde el portal autogestion.
        /// </summary>
        private bool ObtenerPertenenciaGuiaPortalAutogestion(ADGuiaPertenencia guiaPertenencia, ADRastreoGuiaClienteSolicitud guiaSolicitud)
        {
            bool pertenece = false;
            if (guiaPertenencia.IdentificacionRemitente == guiaSolicitud.NumeroIdentificacion
                || guiaPertenencia.IdentificacionDestinatario == guiaSolicitud.NumeroIdentificacion
                || guiaPertenencia.TelefonoRemitente == guiaSolicitud.NumeroTelefono
                || guiaPertenencia.TelefonoDestinatario == guiaSolicitud.NumeroTelefono)
            {
                pertenece = true;
            }

            return pertenece;
        }

        /// <summary>
        /// Metodo para obtener la pertenencia de la guia desde la opcion.
        /// </summary>
        /// <returns></returns>
        private bool ObtenerPertenenciaGuia(int? opcion, ADGuiaPertenencia guiaPertenencia, ADRastreoGuiaClienteSolicitud guiaSolicitud)
        {
            bool guiaPerteneceConsulta = false;

            if (opcion == 0)
            {
                guiaPerteneceConsulta = ObtenerPertenenciaGuiaPortalAutogestion(guiaPertenencia, guiaSolicitud);
            }
            else
            {
                guiaPerteneceConsulta = ObtenerPertenenciaGuiaSigueTuEnvio(opcion, guiaPertenencia, guiaSolicitud);
            }

            return guiaPerteneceConsulta;
        }

        /// <summary>
        /// Metodo para obtener el ratredo de guias
        /// </summary>
        /// <returns></returns>
        private ADRastreoGuiaClienteRespuesta ObtenerRastreoGuiasCliente(ADRastreoGuiaClienteSolicitud guiaSolicitud, bool guiaPerteneceConsulta)
        {
            if (guiaPerteneceConsulta)
            {
                return Instancia.ObtenerRastreoGuiasClientePertenencia(guiaSolicitud, guiaSolicitud.EncriptaAes);
            }
            else
            {
                return Instancia.ObtenerRastreoGuiasDatosProtegidosCliente(guiaSolicitud, guiaSolicitud.EncriptaAes);
            }
        }

        /// <summary>
        /// metodo para el envio de correos.
        /// </summary>
        /// <param name="numero"></param>
        /// <param name="destinatario"></param>
        /// <returns></returns>
        public string EnviarCorreo(long numero, string destinatario)
        {
            object[] resultado = ObtenerPdfImpresion(numero);
            Dictionary<string, string> parametros = new Dictionary<string, string>();
            FacturacionRequest facturacionRequest = (FacturacionRequest)resultado[1];
            DataMail dataMail = null;
            if (facturacionRequest.guia.NumeroGuia > 0)
            {
                dataMail = new DataMail()
                {
                    NombreRemitente = facturacionRequest.guia.Remitente.Nombre,
                    NombreDestinatario = facturacionRequest.guia.Destinatario.Nombre,
                    Servicio = "Envío",
                    FacturaVenta = facturacionRequest.guia.FacturaDian.NumeroFactura.ToString(),
                    FechaAdmision = facturacionRequest.guia.FechaAdmision.ToString(),
                    Origen = facturacionRequest.guia.NombreCiudadOrigen,
                    Destino = facturacionRequest.guia.NombreCiudadDestino,
                    Numero = facturacionRequest.guia.NumeroGuia.ToString()
                };
            }
            else if (facturacionRequest.giro != null)
            {
                dataMail = new DataMail()
                {
                    NombreRemitente = facturacionRequest.giro.GirosPeatonPeaton.ClienteRemitente.Nombre,
                    NombreDestinatario = facturacionRequest.giro.GirosPeatonPeaton.ClienteDestinatario.Nombre,
                    Servicio = "Giro",
                    FacturaVenta = facturacionRequest.giro.FacturaDian.NumeroFactura.ToString(),
                    FechaAdmision = facturacionRequest.giro.FechaGrabacion.ToString(),
                    Origen = facturacionRequest.giro.AgenciaOrigen.NombreMunicipio,
                    Destino = facturacionRequest.giro.AgenciaDestino.NombreMunicipio,
                    Numero = facturacionRequest.giro.IdGiro.ToString()
                };
            }

            if (dataMail != null)
            {
                MailRequest mailRequest = new MailRequest()
                {
                    destinatario = destinatario,
                    adjunto = (byte[])resultado[0],
                    parametrosReplace = new Utilidades().MapearCamposImpresion(dataMail),
                    nombreArchivo = numero.ToString()
                };

                string urlApi = System.Configuration.ConfigurationManager.AppSettings["URIApiController"];
                Dictionary<string, string> parametrosReplace = new Dictionary<string, string>();

                var clientRD = new RestClient(urlApi);
                string uriRD = "Factura/EnviarCorreo";
                var requestRD = new RestRequest(uriRD, Method.POST);
                requestRD.AddHeader("cache-control", "no-cache");
                requestRD.AddHeader("Content-Type", "application/json");

                requestRD.AddParameter("undefined", JsonConvert.SerializeObject(mailRequest), ParameterType.RequestBody);
                IRestResponse responseRD = clientRD.Execute(requestRD);
                var contentRD = responseRD.Content;

                if (!responseRD.IsSuccessful)
                {
                    Utilidades.AuditarExcepcion(new Exception(responseRD.StatusCode.ToString()));
                    return "No fue posible enviar el correo electronico";
                }
                else if (contentRD == "false")
                {
                    return "No fue posible enviar el correo electronico";
                }
                else
                {
                    return "Envió realizado satisfactoriamente";
                }
            }
            else
            {
                return "El número de guia o giro no se encuentra.";
            }
        }

        /// <summary>
        /// Método para validar el token y autorizar usuario
        /// </summary>
        /// <param name="headerValues"></param>
        /// <param name="urlApi"></param>
        /// <exception cref="HttpException"></exception>
        public static void ValidarTokenAutorizacion(IEnumerable<string> headerValues, string urlApi)
        {
            string token = "";
            if (headerValues.FirstOrDefault() != null)
            {
                token = headerValues.FirstOrDefault();
            }
            var idToken = new { IdToken = token };

            var client = new RestClient(urlApi);
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("Content-Type", "application/json");

            request.AddParameter("IdToken", JsonConvert.SerializeObject(idToken), ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            var resultado = JsonConvert.DeserializeObject<dynamic>(response.Content);
            if (resultado.Resultado == null || resultado.message != null)
            {
                throw new HttpException(403, "Internal Server Error");
            }
        }
        #endregion
    }
}

