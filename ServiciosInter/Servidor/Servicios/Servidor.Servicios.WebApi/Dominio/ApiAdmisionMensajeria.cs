using CO.Servidor.Dominio.Comun.AdmEstadosGuia;
using CO.Servidor.Dominio.Comun.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Area;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Integraciones;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.Servicios.ContratoDatos.OperacionNacional;
using CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Precios;
using CO.Servidor.Servicios.WebApi.Comun;
using CO.Servidor.Servicios.WebApi.ModelosRequest.Mensajeria;
using Framework.Servidor.Comun;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace CO.Servidor.Servicios.WebApi.Dominio
{
    public class ApiAdmisionMensajeria : ApiDominioBase
    {
        private static readonly ApiAdmisionMensajeria instancia = (ApiAdmisionMensajeria)FabricaInterceptorApi.GetProxy(new ApiAdmisionMensajeria(), COConstantesModulos.MENSAJERIA);

        public static ApiAdmisionMensajeria Instancia
        {
            get { return ApiAdmisionMensajeria.instancia; }
        }

        private ApiAdmisionMensajeria()
        {
        }




        /// <summary>
        /// realiza una admision automatica de cliente peaton
        /// </summary>
        /// <param name="guia"></param>
        /// <returns></returns>
        public ADRetornoAdmision RegistrarGuiaAutomatica(AdmisionRequest guia)
        {
            switch (guia.Guia.IdServicio)
            {
                case ConstantesServicios.SERVICIO_NOTIFICACIONES:
                    ADNotificacion notificacion = new ADNotificacion()
                    {
                        Apellido1Destinatario = guia.Notificacion.Apellido1Destinatario,
                        Apellido2Destinatario = guia.Notificacion.Apellido2Destinatario,
                        TipoDestino = new TATipoDestino()
                        {
                            Id = guia.Notificacion.IdTipoDestino,
                            Descripcion = guia.Notificacion.DescripcionTipoDestino
                        },
                        CiudadDestino = new PALocalidadDC()
                        {
                            IdLocalidad = guia.Notificacion.IdLocalidadDestino,
                            Nombre = guia.Notificacion.NombreLocalidadDestino
                        },
                        TipoIdentificacionDestinatario = guia.Notificacion.TipoIdentificacionDestinatario,
                        IdDestinatario = guia.Notificacion.IdDestinatario,
                        NombreDestinatario = guia.Notificacion.NombreDestinatario,
                        TelefonoDestinatario = guia.Notificacion.TelefonoDestinatario,
                        DireccionDestinatario = guia.Notificacion.DireccionDestinatario,
                        EmailDestinatario = guia.Notificacion.EmailDestinatario,
                        ReclamaEnOficina = guia.Notificacion.ReclamaEnOficina
                    };

                    return FabricaServicios.ServicioMensajeria.RegistrarGuiaAutomaticaNotificacion(guia.Guia, guia.idCaja, guia.RemitenteDestinatario, notificacion);
                    break;

                case ConstantesServicios.SERVICIO_INTERNACIONAL:
                    throw new NotImplementedException("No se ha implementado el grabado de internacional en WebApi");
                    break;

                case ConstantesServicios.SERVICIO_RAPIRADICADO:
                    ADRapiRadicado radicado = new ADRapiRadicado()
                    {

                        TipoDestino = new TATipoDestino()
                        {
                            Id = guia.Radicado.IdTipoDestinoDestinatario,
                            Descripcion = guia.Radicado.DescripcionTipoDestinoDestinatario
                        },
                        CiudadDestino = new PALocalidadDC()
                        {
                            IdLocalidad = guia.Radicado.IdCiudadDestino,
                            Nombre = guia.Radicado.NombreCiudadDestino

                        },
                        NombreDestinatario = guia.Radicado.NombreDestinatario,
                        Apellido1Destinatario = guia.Radicado.Apellido1Destinatario,
                        Apellido2Destinatario = guia.Radicado.Apellido2Destinatario,
                        DireccionDestinatario = guia.Radicado.DireccionDestinatario,
                        TelefonoDestinatario = guia.Radicado.TelefonoDestinatario,
                        EmailDestinatario = guia.Radicado.EmailDestinatario,
                        IdDestinatario = guia.Radicado.IdDestinatario,
                        TipoIdentificacionDestinatario = guia.Radicado.TipoIdDestinatario,
                        CodigoRapiRadicado = guia.Radicado.CodigoRapiRadicado,
                        NumeroFolios = guia.Radicado.NumeroFolios

                    };

                    return FabricaServicios.ServicioMensajeria.RegistrarGuiaAutomaticaRapiRadicado(guia.Guia, guia.idCaja, guia.RemitenteDestinatario, radicado);
                    break;

                default:
                    return FabricaServicios.ServicioMensajeria.RegistrarGuiaAutomatica(guia.Guia, guia.idCaja, guia.RemitenteDestinatario);
                    break;
            }

        }


        /// <summary>
        /// Obtiene la informacion de una guia a partir de un numero de guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ADGuia ObtenerGuiaNumeroGuia([FromUri]long numeroGuia)
        {
            ADGuia retorno = null;

            if (numeroGuia.ToString().Length >12 && numeroGuia.ToString().StartsWith("8"))
            {
                retorno = FabricaServicios.ServicioMensajeria.ObtenerGuiaSispostalXNumeroGuia(numeroGuia);
            }
            else {
                retorno = FabricaServicios.ServicioMensajeria.ObtenerGuiaXNumeroGuia(numeroGuia);

            }

            retorno.TipoClienteStr = retorno.TipoCliente.ToString();
            return retorno;

        }


        /// <summary>
        /// Registra una guia manual offline
        /// </summary>
        /// <param name="guia"></param>
        public ADRetornoAdmision RegistrarGuiaManualOffLine(AdmisionRequest guia)
        {

            switch (guia.Guia.IdServicio)
            {
                case ConstantesServicios.SERVICIO_NOTIFICACIONES:
                    ADNotificacion notificacion = new ADNotificacion()
                    {
                        Apellido1Destinatario = guia.Notificacion.Apellido1Destinatario,
                        Apellido2Destinatario = guia.Notificacion.Apellido2Destinatario,
                        TipoDestino = new TATipoDestino()
                        {
                            Id = guia.Notificacion.IdTipoDestino,
                            Descripcion = guia.Notificacion.DescripcionTipoDestino
                        },
                        CiudadDestino = new PALocalidadDC()
                        {
                            IdLocalidad = guia.Notificacion.IdLocalidadDestino,
                            Nombre = guia.Notificacion.NombreDestinatario
                        },
                        TipoIdentificacionDestinatario = guia.Notificacion.TipoIdentificacionDestinatario,
                        IdDestinatario = guia.Notificacion.IdDestinatario,
                        NombreDestinatario = guia.Notificacion.NombreDestinatario,
                        TelefonoDestinatario = guia.Notificacion.TelefonoDestinatario,
                        DireccionDestinatario = guia.Notificacion.DireccionDestinatario,
                        EmailDestinatario = guia.Notificacion.EmailDestinatario,
                        ReclamaEnOficina = guia.Notificacion.ReclamaEnOficina
                    };
                    return FabricaServicios.ServicioMensajeria.RegistrarGuiaManualNotificacion(guia.Guia, guia.idCaja, guia.RemitenteDestinatario, notificacion);
                    break;

                case ConstantesServicios.SERVICIO_INTERNACIONAL:
                    throw new NotImplementedException("No se ha implementado el grabado de internacional en WebApi");
                    break;

                case ConstantesServicios.SERVICIO_RAPIRADICADO:
                    ADRapiRadicado radicado = new ADRapiRadicado()
                    {

                        TipoDestino = new TATipoDestino()
                        {
                            Id = guia.Radicado.IdTipoDestinoDestinatario,
                            Descripcion = guia.Radicado.DescripcionTipoDestinoDestinatario
                        },
                        CiudadDestino = new PALocalidadDC()
                        {
                            IdLocalidad = guia.Radicado.IdCiudadDestino,
                            Nombre = guia.Radicado.NombreCiudadDestino

                        },
                        NombreDestinatario = guia.Radicado.NombreDestinatario,
                        Apellido1Destinatario = guia.Radicado.Apellido1Destinatario,
                        Apellido2Destinatario = guia.Radicado.Apellido2Destinatario,
                        DireccionDestinatario = guia.Radicado.DireccionDestinatario,
                        TelefonoDestinatario = guia.Radicado.TelefonoDestinatario,
                        EmailDestinatario = guia.Radicado.EmailDestinatario,
                        IdDestinatario = guia.Radicado.IdDestinatario,
                        TipoIdentificacionDestinatario = guia.Radicado.TipoIdDestinatario,
                        CodigoRapiRadicado = guia.Radicado.CodigoRapiRadicado,
                        NumeroFolios = guia.Radicado.NumeroFolios

                    };
                    return FabricaServicios.ServicioMensajeria.RegistrarGuiaManualRapiRadicado(guia.Guia, guia.idCaja, guia.RemitenteDestinatario, radicado);
                    break;

                default:
                    return FabricaServicios.ServicioMensajeria.RegistrarGuiaManual(guia.Guia, guia.idCaja, guia.RemitenteDestinatario);
                    break;
            }
        }

        /// <summary>
        /// Obtiene toda la informacion´de admisión a partir de una lista de números de guías
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<ADTrazaGuia> ObtenerListaGuiasSeparadaComas(string listaNumerosGuias)
        {
            return FabricaServicios.ServicioMensajeria.ObtenerListaGuiasSeparadaComas(listaNumerosGuias);
        }

        /// <summary>
        /// Obtiene los estados  de una guia en una localidad
        /// </summary>
        /// <returns></returns>
        public List<ADTrazaGuia> ObtenerEstadosGuia(long numeroGuia)
        {
            return FabricaServicios.ServicioMensajeria.ObtenerEstadosGuia(numeroGuia);
        }

        public List<ADTrazaGuia> ObtenerGuiasGestion(int idEstadoGuia, long IdCentroServicioDestino)
        {
            return FabricaServicios.ServicioMensajeria.ObtenerGuiasGestion(idEstadoGuia, IdCentroServicioDestino);
        }

        /// <summary>
        /// Obtiene los Estados y Motivos de la Guia seleccionada
        /// </summary>
        /// <returns></returns>
        public List<ADEstadoGuiaMotivoDC> ObtenerEstadosMotivosGuia(long numeroGuia)
        {
            List<ADEstadoGuiaMotivoDC> estadosMotivoGuia = new List<ADEstadoGuiaMotivoDC>();
            List<INEstadosSWSispostal> estados = new List<INEstadosSWSispostal>();
            ADEstadoGuiaMotivoDC miEstado = null;

            /******************************* Si la guía es de SISPOSTAL ************************************/
            if (numeroGuia.ToString().Length > 12 && numeroGuia.ToString().StartsWith("8"))
            {
                estados = FabricaServicios.ServicioMotorSispostal.ObtenerEstadosGuiaSispostal(numeroGuia);
                if (estados != null)
                {
                    foreach (var item in estados)
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
            }
            /**************** Guia CONTROLLER **************************/
            else
            {
                estadosMotivoGuia = FabricaServicios.ServicioMensajeria.ObtenerEstadosMotivosGuia(numeroGuia);

                if (estadosMotivoGuia.Count > 0)
                {
                    /*********************************** ESTADO MOTIVO GUIA : INCAUTADO ***********************************************/
                    var estadoGuiaIncautado = estadosMotivoGuia.FirstOrDefault(o => o.Motivo.IdMotivoGuia == 159);
                    if (estadoGuiaIncautado != null)
                    {
                        /************************ Se filtra por incautado y se elimina el ultimo centro de acopio ***************************************/
                        var listaElementosMotivo = estadosMotivoGuia.ToList().FindAll(x => x.EstadoGuia.IdEstadoGuia == (short)ADEnumEstadoGuia.CentroAcopio).OrderByDescending(y => y.EstadoGuia.FechaGrabacion).ToList();
                        if (listaElementosMotivo.Count > 0)
                        {
                            estadosMotivoGuia.Remove(listaElementosMotivo[0]);
                        }
                    }

                    /********************************************** ACTUALIZAR RESIDENTE AUSENTE / MENSAJERO NO ALCANZÓ *******************************************************/
                    estadosMotivoGuia.ToList().FindAll(x => x.Motivo.IdMotivoGuia == 122).ForEach(e =>
                    {
                        e.Motivo.Descripcion = e.Motivo.Descripcion.Split('/')[0] + "/" + e.Motivo.Descripcion.Split('/')[1] + "/";
                    });


                    /********************************** ELIMINAR TRANSITO URBANO *****************************************************************/
                    var estadoGuiaTransitoUrbano = estadosMotivoGuia.Where(w => w.EstadoGuia.IdEstadoGuia == (short)ADEnumEstadoGuia.TransitoUrbano);
                    if (estadoGuiaTransitoUrbano != null)
                    {
                        estadosMotivoGuia.Remove(estadoGuiaTransitoUrbano.FirstOrDefault());
                    }
                }
            }

            /******************************** Ultimo estado de la guia ***********************************/
            ADEstadoGuiaMotivoDC ultimoEstado = estadosMotivoGuia.LastOrDefault();

            /******************** Validacion ultimo estado reparto ******************/
            if (ultimoEstado.EstadoGuia.IdEstadoGuia == (short)ADEnumEstadoGuia.EnReparto)
            {
                /************************ Obtene posicion mensajero por numero de guia (segun la ultima planilla generada) **************************/
                POUbicacionMensajero poUbicacionMensajero = FabricaServicios.ServicioParametrosOperacion.ObtenerUltimaPosicionMensajeroPorNumeroGuia(numeroGuia);
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
            return estadosMotivoGuia;
        }
    
         
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public LIArchivoGuiaMensajeriaDC ObtenerArchivoGuiaxNumeroGuia(long numeroGuia)
        {
            return FabricaServicios.ServicioLogisticaInversa.ObtenerArchivoGuiaxNumeroGuia(numeroGuia);
        }


        ///// <summary>
        ///// Obtiene Informacion de Telemercadeo de la Guia seleccionada
        ///// </summary>
        ///// <returns></returns>
        public List<LIGestionesDC> ObtenerInformacionTelemercadeoGuia(long numeroGuia)
        {
            return FabricaServicios.ServicioLogisticaInversa.ObtenerInformacionTelemercadeoGuia(numeroGuia);
        }

        ///// <summary>
        ///// Obtiene Novedades de Transporte de la Guia seleccionada
        ///// </summary>
        ///// <returns></returns>
        public List<ONNovedadesTransporteDC> ObtenerNovedadesTransporteGuia(long numeroGuia)
        {
            return FabricaServicios.ServicioOperacionNacional.ObtenerNovedadesTransporteGuia(numeroGuia);
        }

        /// <summary>
        /// Retorna volantes de una guia
        /// realizado por Mauricio Sanchez 20160208
        /// </summary>
        /// <param name="numeroGuia">Identificador admisión giro</param>
        /// <returns>Archivo</returns>
        public List<string> ObtenerVolantesGuia(long numeroGuia)
        {
            return FabricaServicios.ServicioLogisticaInversa.ObtenerVolantesGuia(numeroGuia);
        }

        /// <summary>
        /// Registrar guia manual 
        /// </summary>
        /// <param name="guia"></param>
        /// <returns></returns>
        public ADRetornoAdmision RegistrarGuiaManual(AdmisionRequest guia)
        {
            //ADRapiRadicado radicado = new ADRapiRadicado();
            //ADNotificacion notificacion = new ADNotificacion();

            //switch (guia.Guia.IdServicio)
            //{
            //    case ConstantesServicios.SERVICIO_NOTIFICACIONES:
            //         notificacion = new ADNotificacion()
            //        {
            //            Apellido1Destinatario = guia.Notificacion.Apellido1Destinatario,
            //            Apellido2Destinatario = guia.Notificacion.Apellido2Destinatario,
            //            TipoDestino = new TATipoDestino()
            //            {
            //                Id = guia.Notificacion.IdTipoDestino,
            //                Descripcion = guia.Notificacion.DescripcionTipoDestino
            //            },
            //            CiudadDestino = new PALocalidadDC()
            //            {
            //                IdLocalidad = guia.Notificacion.IdLocalidadDestino,
            //                Nombre = guia.Notificacion.NombreDestinatario
            //            },
            //            TipoIdentificacionDestinatario = guia.Notificacion.TipoIdentificacionDestinatario,
            //            IdDestinatario = guia.Notificacion.IdDestinatario,
            //            NombreDestinatario = guia.Notificacion.NombreDestinatario,
            //            TelefonoDestinatario = guia.Notificacion.TelefonoDestinatario,
            //            DireccionDestinatario = guia.Notificacion.DireccionDestinatario,
            //            EmailDestinatario = guia.Notificacion.EmailDestinatario,
            //            ReclamaEnOficina = guia.Notificacion.ReclamaEnOficina
            //        };
            //        break;
            //    case ConstantesServicios.SERVICIO_RAPIRADICADO:
            //         radicado = new ADRapiRadicado()
            //        {

            //            TipoDestino = new TATipoDestino()
            //            {
            //                Id = guia.Radicado.IdTipoDestinoDestinatario,
            //                Descripcion = guia.Radicado.DescripcionTipoDestinoDestinatario
            //            },
            //            CiudadDestino = new PALocalidadDC()
            //            {
            //                IdLocalidad = guia.Radicado.IdCiudadDestino,
            //                Nombre = guia.Radicado.NombreCiudadDestino

            //            },
            //            NombreDestinatario = guia.Radicado.NombreDestinatario,
            //            Apellido1Destinatario = guia.Radicado.Apellido1Destinatario,
            //            Apellido2Destinatario = guia.Radicado.Apellido2Destinatario,
            //            DireccionDestinatario = guia.Radicado.DireccionDestinatario,
            //            TelefonoDestinatario = guia.Radicado.TelefonoDestinatario,
            //            EmailDestinatario = guia.Radicado.EmailDestinatario,
            //            IdDestinatario = guia.Radicado.IdDestinatario,
            //            TipoIdentificacionDestinatario = guia.Radicado.TipoIdDestinatario,
            //            CodigoRapiRadicado = guia.Radicado.CodigoRapiRadicado,
            //            NumeroFolios = guia.Radicado.NumeroFolios

            //        };
            //        break;                
            //    default:
            //        break;
            //}
            //guia.Guia.IdCaja = guia.idCaja; 
            //return FabricaServicios.ServicioMensajeria.RegistrarGuiaManualMovil(guia.Guia, guia.RemitenteDestinatario, notificacion, radicado);
            switch (guia.Guia.IdServicio)
            {
                case ConstantesServicios.SERVICIO_NOTIFICACIONES:
                    ADNotificacion notificacion = new ADNotificacion()
                    {
                        Apellido1Destinatario = guia.Notificacion.Apellido1Destinatario,
                        Apellido2Destinatario = guia.Notificacion.Apellido2Destinatario,
                        TipoDestino = new TATipoDestino()
                        {
                            Id = guia.Notificacion.IdTipoDestino,
                            Descripcion = guia.Notificacion.DescripcionTipoDestino
                        },
                        CiudadDestino = new PALocalidadDC()
                        {
                            IdLocalidad = guia.Notificacion.IdLocalidadDestino,
                            Nombre = guia.Notificacion.NombreDestinatario
                        },
                        TipoIdentificacionDestinatario = guia.Notificacion.TipoIdentificacionDestinatario,
                        IdDestinatario = guia.Notificacion.IdDestinatario,
                        NombreDestinatario = guia.Notificacion.NombreDestinatario,
                        TelefonoDestinatario = guia.Notificacion.TelefonoDestinatario,
                        DireccionDestinatario = guia.Notificacion.DireccionDestinatario,
                        EmailDestinatario = guia.Notificacion.EmailDestinatario,
                        ReclamaEnOficina = guia.Notificacion.ReclamaEnOficina
                    };
                    return FabricaServicios.ServicioMensajeria.RegistrarGuiaManualNotificacion(guia.Guia, guia.idCaja, guia.RemitenteDestinatario, notificacion);
                    break;

                case ConstantesServicios.SERVICIO_INTERNACIONAL:
                    throw new NotImplementedException("No se ha implementado el grabado de internacional en WebApi");
                    break;

                case ConstantesServicios.SERVICIO_RAPIRADICADO:
                    ADRapiRadicado radicado = new ADRapiRadicado()
                    {

                        TipoDestino = new TATipoDestino()
                        {
                            Id = guia.Radicado.IdTipoDestinoDestinatario,
                            Descripcion = guia.Radicado.DescripcionTipoDestinoDestinatario
                        },
                        CiudadDestino = new PALocalidadDC()
                        {
                            IdLocalidad = guia.Radicado.IdCiudadDestino,
                            Nombre = guia.Radicado.NombreCiudadDestino

                        },
                        NombreDestinatario = guia.Radicado.NombreDestinatario,
                        Apellido1Destinatario = guia.Radicado.Apellido1Destinatario,
                        Apellido2Destinatario = guia.Radicado.Apellido2Destinatario,
                        DireccionDestinatario = guia.Radicado.DireccionDestinatario,
                        TelefonoDestinatario = guia.Radicado.TelefonoDestinatario,
                        EmailDestinatario = guia.Radicado.EmailDestinatario,
                        IdDestinatario = guia.Radicado.IdDestinatario,
                        TipoIdentificacionDestinatario = guia.Radicado.TipoIdDestinatario,
                        CodigoRapiRadicado = guia.Radicado.CodigoRapiRadicado,
                        NumeroFolios = guia.Radicado.NumeroFolios

                    };
                    return FabricaServicios.ServicioMensajeria.RegistrarGuiaManualRapiRadicado(guia.Guia, guia.idCaja, guia.RemitenteDestinatario, radicado);
                    break;

                default:
                    return FabricaServicios.ServicioMensajeria.RegistrarGuiaManual(guia.Guia, guia.idCaja, guia.RemitenteDestinatario);
                    break;
            }
            //guia.Guia.IdCaja = guia.idCaja;
          //  return FabricaServicios.ServicioMensajeria.RegistrarGuiaManualMovil(guia.Guia, guia.RemitenteDestinatario, notificacion, radicado);

        }

        /// <summary>
        /// Método para adicionar una guia interna
        /// </summary>
        /// <returns>Identificador de la admisión de la guía interna</returns>
        public ADGuiaInternaDC AdicionarGuiaInterna(AdmisionGuiainternaRequest guiaInterna)
        {
            ADGuiaInternaDC guiaInternaCast = new ADGuiaInternaDC
            {
                NumeroGuia = guiaInterna.NumeroGuia,
                GestionOrigen = new ARGestionDC
                {
                    IdGestion = guiaInterna.idGestionOrigen,
                    Descripcion = guiaInterna.descripcionGestionOrigen
                },
                GestionDestino = new ARGestionDC
                {
                    IdGestion = guiaInterna.idGestionDestino,
                    Descripcion = guiaInterna.descripcionGestionDestino
                },
                NombreRemitente = guiaInterna.NombreRemitente,
                TelefonoRemitente = guiaInterna.TelefonoRemitente,
                DireccionRemitente = guiaInterna.DireccionRemitente,
                IdCentroServicioOrigen = guiaInterna.IdCentroServicioOrigen,
                NombreCentroServicioOrigen = guiaInterna.NombreCentroServicioOrigen,
                LocalidadOrigen = new PALocalidadDC
                {
                    IdLocalidad = guiaInterna.IdLocalidadOrigen,
                    Nombre = guiaInterna.NombreLocalidadOrigen
                },
                LocalidadDestino = new PALocalidadDC
                {
                    IdLocalidad = guiaInterna.IdLocalidadDestino,
                    Nombre = guiaInterna.NombreLocalidadDestino
                },
                PaisDefault = new PALocalidadDC
                {
                    IdLocalidad = guiaInterna.idPaisDefault,
                    Nombre = guiaInterna.nombrePaisDefault
                },
                NombreDestinatario = guiaInterna.NombreDestinatario,
                TelefonoDestinatario = guiaInterna.TelefonoDestinatario,
                DireccionDestinatario = guiaInterna.DireccionDestinatario,
                DiceContener = guiaInterna.DiceContener,
                EsManual = guiaInterna.EsManual,
                EsOrigenGestion = guiaInterna.EsOrigenGestion,
                EsDestinoGestion = guiaInterna.EsDestinoGestion,
                CreadoPor = guiaInterna.CreadoPor,
                FechaGrabacion = guiaInterna.FechaGrabacion == DateTime.MinValue ? DateTime.Now : guiaInterna.FechaGrabacion,
                TiempoEntrega = guiaInterna.TiempoEntrega,
                IdentificacionRemitente = guiaInterna.IdentificacionRemitente,
                IdentificacionDestinatario = guiaInterna.IdentificacionDestinatario,
                TipoIdentificacionRemitente = guiaInterna.TipoIdentificacionRemitente,
                TipoIdentificacionDestinatario = guiaInterna.TipoIdentificacionDestinatario,
                EmailRemitente = guiaInterna.EmailRemitente,
                EmailDestinatario = guiaInterna.EmailDestinatario,
                Observaciones = guiaInterna.Observaciones,
                FechaEstimadaEntrega = guiaInterna.FechaEstimadaEntrega
            };
            return FabricaServicios.ServicioMensajeria.AdicionarGuiaInterna(guiaInternaCast);
        }
        
        #region Rastreo de la guía

        /// <summary>
        /// Metodo para obtener el rastreo de las guias solicitadas
        /// </summary>
        /// <param name="guias"></param>
        /// <returns></returns>
        internal List<ADRastreoGuiaDC> ObtenerRastreoGuias(string guiasAConsultar)
        {
            List<ADRastreoGuiaDC> LstRastreoGuias = new List<ADRastreoGuiaDC>();

            List<long> lstGuias = guiasAConsultar.Split(',').ToList().ConvertAll<long>(l =>
            {
                long n = 0;
                long.TryParse(l, out n);
                return n;
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
                        rastreoGuia.TrazaGuia = EstadosGuia.ObtenerTrazaUltimoEstadoXNumGuiaSispostal(e);

                        //Estados del envío
                        List<INEstadosSWSispostal> EstadosSisPostal = FabricaServicios.ServicioMotorSispostal.ObtenerEstadosGuiaSispostal(e);
                        if (EstadosSisPostal != null)
                        {
                            List<ADEstadoGuiaMotivoDC> estadosMotivoGuia = new List<ADEstadoGuiaMotivoDC>();
                            foreach (var item in EstadosSisPostal)
                            {
                                //Causales GUIA SIN FISICO, ANULADO, SINIESTRO, RETENCION NO DEBEN SER VISIBLES
                                if (item.Estado != "GUIA SIN FISICO" && item.Estado != "ANULADO" && item.Estado != "RETENCION")
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
                        rastreoGuia.Guia = FabricaServicios.ServicioMensajeria.ObtenerGuiaSispostalXNumeroGuia(e);

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
                        //Ultima gestion del envío
                        rastreoGuia.TrazaGuia = EstadosGuia.ObtenerTrazaUltimoEstadoXNumGuia(e);
                        // Estados de la guía
                        List<ADEstadoGuiaMotivoDC> EstadosMotivoGuia = FabricaServicios.ServicioMensajeria.ObtenerEstadosMotivosGuia(e);
                        if (EstadosMotivoGuia.Count > 0)
                        {
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
                                POUbicacionMensajero poUbicacionMensajero = FabricaServicios.ServicioParametrosOperacion.ObtenerUltimaPosicionMensajeroPorNumeroGuia(e);
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

                        // Remitente - Destinatario (Información de la gúia
                        rastreoGuia.Guia = FabricaServicios.ServicioMensajeria.ObtenerGuiaXNumeroGuia(e);
                        //Telemercadeo 
                        rastreoGuia.Telemercadeo = FabricaServicios.ServicioLogisticaInversa.ObtenerInformacionTelemercadeoGuia(e);
                        //Volantes de devolucion
                        rastreoGuia.Volantes = FabricaServicios.ServicioLogisticaInversa.ObtenerVolantesGuia(e);
                        //Novedades de transporte
                        rastreoGuia.NovedadesTransporte = FabricaServicios.ServicioOperacionNacional.ObtenerNovedadesTransporteGuia(e);
                        //Imagen
                        rastreoGuia.ImagenGuia = ObtenerImagenGuia(e);
                    }
                    #endregion  

                    if (rastreoGuia.TrazaGuia.IdEstadoGuia == (short)ADEnumEstadoGuia.Archivada || rastreoGuia.TrazaGuia.IdEstadoGuia == (short)ADEnumEstadoGuia.Digitalizada)
                    {
                        List<ADTrazaGuia> lstEstados = EstadosGuia.ObtenerEstadosGuia(e);
                        var estado = lstEstados.Where(l => l.IdEstadoGuia == (short)ADEnumEstadoGuia.DevolucionRatificada || l.IdEstadoGuia == (short)ADEnumEstadoGuia.Entregada).FirstOrDefault();
                        if (estado != null)
                        {
                            rastreoGuia.TrazaGuia.IdEstadoGuia = estado.IdEstadoGuia;
                            rastreoGuia.TrazaGuia.DescripcionEstadoGuia = estado.DescripcionEstadoGuia;
                        }
                    }
                    LstRastreoGuias.Add(rastreoGuia);
                }
            });

            return LstRastreoGuias;
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
                archivo = FabricaServicios.ServicioLogisticaInversa.ObtenerArchivoGuiaSispostal(archivo);
                imagen = TraerImagenFtp(archivo, numeroGuia, Sftp, SUsuario, SPassword, cont);
            }
            else
            {
                Sftp = "ftpDigitalizacion";
                SPassword = "passFtpDigitalizaci";
                SUsuario = "UserFtpDigitalizaci";
                archivo = FabricaServicios.ServicioLogisticaInversa.ObtenerArchivoGuiaFS(archivo);

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

            ftp = FabricaServicios.ServicioParametros.ConsultarParametrosFramework(SFtp).ValorParametro;
            pass = FabricaServicios.ServicioParametros.ConsultarParametrosFramework(SPassword).ValorParametro;
            user = FabricaServicios.ServicioParametros.ConsultarParametrosFramework(SUsuario).ValorParametro;

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
                byte[] imagenArray = Util.ReadToEnd(stream);
                stream.Close();
                string imgString = Convert.ToBase64String(imagenArray);
                return imgString;
            }
            else
                return "";
        }
        #endregion

        #region Nuevo Cotizador
        /// <summary>
        /// Método para calcular tarifas
        /// </summary>
        /// <param name="idLocalidadOrigen"></param>
        /// <param name="idLocalidadDestino"></param>
        /// <param name="peso"></param>
        /// <param name="valorDeclarado"></param>
        /// <param name="idTipoEntrega"></param>
        /// <returns></returns>

        public List<TAPreciosAgrupadosDC> ResultadoListaCotizar(string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, string idTipoEntrega)
        {
            /***********Calculo de servicios por precios respectivos **********/
            List<TAPreciosAgrupadosDC> preciosCotizacion = FabricaServicios.ServicioMensajeria.ResultadoListaCotizar(idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, idTipoEntrega);
            /************Calculo de valor prima para carga terrestre *************/
            var cargarTerrestre = preciosCotizacion.Where(e => e.IdServicio == TAConstantesServicios.SERVICIO_RAPI_CARGA).FirstOrDefault();
            if (cargarTerrestre != null)
            {
                if (valorDeclarado < 100000)
                {
                    valorDeclarado = 100000;
                }
                cargarTerrestre.Precio.ValorPrimaSeguro = valorDeclarado / 100;
                cargarTerrestre.PrecioCarga.ValorPrimaSeguro = valorDeclarado / 100;
            }
            return preciosCotizacion;
        }

        public List<ADGuia> ObtenerReporteCajaGuiasMensajeroApp(long idMensajero)
        {
            return FabricaServicios.ServicioMensajeria.ObtenerReporteCajaGuiasMensajeroApp(idMensajero);
        }

        #endregion


        #region Calculo fechas Estimada entrega
        /// <summary>
        /// Metodo para validar servicio trayecto destino 
        /// </summary>
        /// <param name="municipioOrigen"></param>
        /// <param name="municipioDestino"></param>
        /// <param name="servicio"></param>
        /// <param name="centroServiciosOrigen"></param>
        /// <param name="fechadmisionEnvio"></param>
        /// <returns></returns>
        public ADValidacionServicioTrayectoDestino ValidarServicioTrayectoDestino(PALocalidadDC municipioOrigen, PALocalidadDC municipioDestino, TAServicioDC servicio, long centroServiciosOrigen, decimal pesoGuia, DateTime? fechadmisionEnvio)
        {

            return FabricaServicios.ServicioMensajeria.ValidarServicioTrayectoDestino(municipioOrigen, municipioDestino, servicio, centroServiciosOrigen, pesoGuia, fechadmisionEnvio); 

        }
        #endregion
    }
}
