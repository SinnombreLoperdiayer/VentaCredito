using CO.Servidor.Dominio.Comun.AdmEstadosGuia;
using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Dominio.Comun.Cajas;
using CO.Servidor.Dominio.Comun.CentroAcopio;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Comisiones;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.OperacionUrbana;
using CO.Servidor.Dominio.Comun.Suministros;
using CO.Servidor.Dominio.Comun.Tarifas;
using CO.Servidor.LogisticaInversa.Datos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Comun;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW;
using CO.Servidor.RAPS.Reglas.Parametros;
using CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes;
using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;
using System.ServiceModel;
using System.Threading;
using System.Globalization;
using System.Transactions;
using System;
using CO.Servidor.LogisticaInversa.Comun;
using CO.Servidor.Raps.Comun.Integraciones;
using System.Collections.Generic;
using Framework.Servidor.Comun.Util;
using System.Linq;

namespace CO.Servidor.LogisticaInversa.PruebasEntrega.Descargue
{
    public class LIDescarguePruebasEntrega : ControllerBase
    {
        #region Instancia

        private static readonly LIDescarguePruebasEntrega instancia = (LIDescarguePruebasEntrega)FabricaInterceptores.GetProxy(new LIDescarguePruebasEntrega(), COConstantesModulos.PRUEBAS_DE_ENTREGA);

        public static LIDescarguePruebasEntrega Instancia
        {
            get { return LIDescarguePruebasEntrega.instancia; }
        }

        public LIDescarguePruebasEntrega()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(PAAdministrador.Instancia.ConsultarParametrosFramework("Cultura"));
        }

        #endregion Instancia

        #region Fachadas

        private ICAFachadaCajas fachadaCajas = COFabricaDominio.Instancia.CrearInstancia<ICAFachadaCajas>();

        private IADFachadaAdmisionesMensajeria fachadaAdmisiones = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>();

        private ISUFachadaSuministros fachadaSuministros = COFabricaDominio.Instancia.CrearInstancia<ISUFachadaSuministros>();

        private IADFachadaAdmisionesMensajeria fachadaMensajeria = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>();

        private ICMFachadaComisiones fachadaComisiones = COFabricaDominio.Instancia.CrearInstancia<ICMFachadaComisiones>();

        private IOUFachadaOperacionUrbana fachadaOpUrbana = COFabricaDominio.Instancia.CrearInstancia<IOUFachadaOperacionUrbana>();

        private IPUFachadaCentroServicios fachadaCes = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();

        private ICAFachadaCentroAcopio fachadaCentroAcopio = COFabricaDominio.Instancia.CrearInstancia<ICAFachadaCentroAcopio>();


        #endregion Fachadas

        #region Campos        
        RAParametrosSolicitudAcumulativaDC parametrosSolicitudAcumulativa = new RAParametrosSolicitudAcumulativaDC();
        RAFallaMapper fallaMapper = null;

        #endregion Campos

        #region Metodos

        /// <summary>
        /// MÃ©todo para obtener la informacion de una guia en el descargue de devoluciones por mensajero
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public OUGuiaIngresadaDC ObtenerInfoGuiaDevolucion(long numeroGuia, long idMensajero)
        {
            return new OUGuiaIngresadaDC();
            //OUPlanillaAsignacionMensajero planillaGuia = OURecogidas.Instancia.ObtenerUltimaPlanillaMensajeroGuia(NumeroGuia)
        }

        public LIDescargueGuiaDC GuardarDevolucionAgencia(OUGuiaIngresadaDC guia)
        {
            LIDescargueGuiaDC respuesta = new LIDescargueGuiaDC();
            ADTrazaGuia estadoGuia;

            try
            {
                if (guia.Motivo == null)
                {
                    using (TransactionScope transaccion = new TransactionScope())
                    {

                        respuesta.Resultado = RegistrarNovedadSinMotivo(guia);
                        respuesta.Estado = ADEnumEstadoGuia.SinEstado;
                        PUMovimientoInventario movimiento = new Servicios.ContratoDatos.CentroServicios.PUMovimientoInventario
                        {
                            TipoMovimiento = PUEnumTipoMovimientoInventario.Asignacion,
                            IdCentroServicioOrigen = guia.IdCentroLogistico,
                            Bodega = new PUCentroServiciosDC
                            {
                                IdCentroServicio = ControllerContext.Current.IdCentroServicio,
                            },
                            NumeroGuia = guia.NumeroGuia.Value,
                            FechaGrabacion = DateTime.Now,
                            FechaEstimadaIngreso = DateTime.Now,
                            CreadoPor = ControllerContext.Current.Usuario,
                        };
                        fachadaCes.AdicionarMovimientoInventario(movimiento);

                        fachadaCentroAcopio.CambiarTipoEntregaTelemercadeo_REO((long)guia.NumeroGuia, guia.IdCentroServicioDestino);

                        transaccion.Complete();
                    }

                }
                else
                {
                    using (TransactionScope transaccion = new TransactionScope())
                    {

                        string observaciones = guia.Observaciones != null ? guia.Observaciones : string.Empty;

                        guia.NuevoEstadoGuia = "DEV";
                        guia.EstaDescargada = true;
                        guia.EstadoGuiaPlanilla = "DEV";
                        guia.TipoImpreso = ADEnumTipoImpreso.Planilla;

                        //cambio de estado pasa a intento de entrega
                        estadoGuia = new ADTrazaGuia
                        {
                            Ciudad = guia.CiudadDestino.Nombre,
                            IdCiudad = guia.CiudadDestino.IdLocalidad,
                            IdAdmision = guia.IdAdmision,
                            IdEstadoGuia = (short)(EstadosGuia.ObtenerUltimoEstado(guia.IdAdmision)),
                            IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.IngresoABodega,
                            Modulo = COConstantesModulos.PRUEBAS_DE_ENTREGA,
                            NumeroGuia = guia.NumeroGuia,
                            Observaciones = observaciones,
                            FechaGrabacion = guia.FechaMotivoDevolucion
                        };

                        estadoGuia.IdTrazaGuia = EstadosGuia.InsertaEstadoGuia(estadoGuia);

                        if (estadoGuia.IdTrazaGuia == 0)
                        {
                            respuesta.Resultado = OUEnumValidacionDescargue.ErrorEstado;
                            return respuesta;
                        }


                        //Actualiza estado de guia en planilla de mensajero
                        fachadaOpUrbana.ActualizarGuiaMensajero(guia);

                        if (guia.Motivo.IntentoEntrega)
                        {
                            //Actualiza intentos de entrega
                            fachadaMensajeria.ActualizarReintentosEntrega(guia.IdAdmision);
                        }

                        // Traza impreso asocia el cambio de estado con el documento que lo genero
                        ADTrazaGuiaImpresoDC trazaImpreso = new ADTrazaGuiaImpresoDC
                        {
                            IdTrazaGuia = estadoGuia.IdTrazaGuia,
                            NumeroImpreso = guia.Planilla,
                            TipoImpreso = guia.TipoImpreso,
                            Usuario = ControllerContext.Current.Usuario,
                            FechaGrabacion = DateTime.Now,
                        };

                        EstadosGuia.InsertarEstadoGuiaImpreso(trazaImpreso);

                        ADEstadoGuiaMotivoDC estadoMotivoGuia = new ADEstadoGuiaMotivoDC
                        {
                            IdTrazaGuia = estadoGuia.IdTrazaGuia,
                            Motivo = guia.Motivo,
                            Observaciones = string.Empty,
                            FechaMotivo = guia.FechaMotivoDevolucion
                        };

                        //inserta el estado motivo de la devoluciÃ³n
                        EstadosGuia.InsertaEstadoGuiaMotivo(estadoMotivoGuia);

                        /// Se guarda la evidencia de intento de entrega
                        if (guia.Motivo.EsEscaneo && guia.EvidenciaDevolucion.NumeroEvidencia != 0)
                        {
                            if (LIRepositorioPruebasEntrega.Instancia.ValidarEvidenciaDevolucion(
                                guia.EvidenciaDevolucion.NumeroEvidencia, guia.EvidenciaDevolucion.TipoEvidencia.IdTipo))
                            {
                                respuesta.Resultado = OUEnumValidacionDescargue.Error;
                                respuesta.Mensaje = "Numero de volante de devoluciÃ³n ya existe";
                                transaccion.Dispose();
                                return respuesta;
                            }
                            else
                            {
                                // obtener el tipo de evidencia de acuerdo al motivo y el intento de entrega
                                guia.EvidenciaDevolucion.IdEstadoGuialog = estadoGuia.IdTrazaGuia.Value;
                                guia.EvidenciaDevolucion.EstaDigitalizado = false;
                                guia.EvidenciaDevolucion.IdEvidenciaDevolucion = guia.EvidenciaDevolucion.NumeroEvidencia;
                                guia.EvidenciaDevolucion.TipoEvidencia = EGMotivosGuia.ObtenerMotivosEvidencia(guia.Motivo, guia.CantidadReintentosEntrega);

                                LIRepositorioPruebasEntrega.Instancia.AdicionarEvidenciaDevolucion(guia.EvidenciaDevolucion);
                            }
                        }

                        //segunda transicion de estado obtiene el estado a partir del motivo, estado actual e intento de entrega
                        estadoGuia = new ADTrazaGuia
                        {
                            Ciudad = guia.CiudadDestino.Nombre,
                            IdCiudad = guia.CiudadDestino.IdLocalidad,
                            IdAdmision = guia.IdAdmision,
                            IdEstadoGuia = (short)ADEnumEstadoGuia.IntentoEntrega,
                            Modulo = COConstantesModulos.PRUEBAS_DE_ENTREGA,
                            NumeroGuia = guia.NumeroGuia,
                            Observaciones = observaciones,
                            FechaGrabacion = DateTime.Now.AddMilliseconds(500)
                        };

                        estadoGuia.IdNuevoEstadoGuia = EGMotivosGuia.ObtenerEstadoMotivo(guia.Motivo.IdMotivoGuia, (short)(ADEnumEstadoGuia.IntentoEntrega), guia.CantidadReintentosEntrega);

                        if (estadoGuia.IdNuevoEstadoGuia == 0)
                        {
                            estadoGuia.IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.DevolucionRegional;
                        }

                        estadoGuia.IdTrazaGuia = EstadosGuia.InsertaEstadoGuia(estadoGuia);
                        respuesta.Estado = (ADEnumEstadoGuia)estadoGuia.IdNuevoEstadoGuia;

                        if (estadoGuia.IdEstadoGuia == (short)ADEnumEstadoGuia.PendienteIngresoaCustodia)
                        {
                            ADGuia guiaAdmision = fachadaAdmisiones.ObtenerGuiaXNumeroGuia(guia.NumeroGuia.Value);
                            if (!string.IsNullOrEmpty(guiaAdmision.Destinatario.Telefono) && guiaAdmision.Destinatario.Telefono.Length == 10 && guia.TipoCliente == ADEnumTipoCliente.PPE.ToString())
                            {
                                AdministradorMensajesTexto.Instancia.EnviarMensajeTexto(LOIEnumMensajesTexto.Custodia, guiaAdmision.TelefonoDestinatario, guia.NumeroGuia.Value, "");
                            }

                        }

                        PUMovimientoInventario movimiento = new Servicios.ContratoDatos.CentroServicios.PUMovimientoInventario
                        {
                            TipoMovimiento = PUEnumTipoMovimientoInventario.Ingreso,
                            IdCentroServicioOrigen = guia.IdCentroLogistico,
                            Bodega = new PUCentroServiciosDC
                            {
                                IdCentroServicio = ControllerContext.Current.IdCentroServicio,
                            },
                            NumeroGuia = guia.NumeroGuia.Value,
                            FechaGrabacion = DateTime.Now,
                            FechaEstimadaIngreso = DateTime.Now,
                            CreadoPor = ControllerContext.Current.Usuario,
                        };
                        fachadaCes.AdicionarMovimientoInventario(movimiento);


                        //Se valida si el motivo causa supervision, se actualiza la admisiÃ³n
                        if (estadoGuia.IdEstadoGuia == (short)ADEnumEstadoGuia.Auditoria)
                            fachadaMensajeria.ActualizarSupervisionGuia(guia.IdAdmision);

                        if (guia.Novedad != null)
                        {
                            CONovedadGuiaDC novedad = new CONovedadGuiaDC { NumeroGuia = guia.NumeroGuia.Value, TipoNovedad = guia.Novedad };
                            LIRepositorioPruebasEntrega.Instancia.IngresarNovedad(novedad);
                        }

                        if (guia.Motivo.TiempoAfectacion != 0)
                        {
                            EGTipoNovedadGuia.CambiarFechaEntrega(new Servicios.ContratoDatos.Comun.COCambioFechaEntregaDC
                            {
                                Guia = new ADGuia { NumeroGuia = guia.NumeroGuia.Value },
                                TiempoAfectacion = guia.Motivo.TiempoAfectacion
                            });
                        }
                        respuesta.Resultado = OUEnumValidacionDescargue.Exitosa;
                        transaccion.Complete();

                    }
                }

                IntegrarDevolucionAgencia(guia);
                return respuesta;

            }
            catch (Exception ex)
            {
                var ex1 = (System.ServiceModel.FaultException)ex;
                if (ex1.Code.Name == "Raps0001")
                {
                    respuesta.Resultado = OUEnumValidacionDescargue.ErrorSolicitudRaps;
                }
                else
                {
                    respuesta.Resultado = OUEnumValidacionDescargue.Error;
                }
                respuesta.Mensaje = ex.Message;
                return respuesta;
            }
        }

        public LIDescargueGuiaDC GuardarDevolucionAgenciaV7(OUGuiaIngresadaDC guia)
        {
            LIDescargueGuiaDC respuesta = new LIDescargueGuiaDC();
            ADTrazaGuia estadoGuia;
            //IDictionary<string, object> datos=null;
            bool integraRaps = false;


            try
            {
                if (guia.Motivo == null)
                {
                    using (TransactionScope transaccion = new TransactionScope())
                    {

                        respuesta.Resultado = RegistrarNovedadSinMotivo(guia);
                        respuesta.Estado = ADEnumEstadoGuia.SinEstado;
                        PUMovimientoInventario movimiento = new Servicios.ContratoDatos.CentroServicios.PUMovimientoInventario
                        {
                            TipoMovimiento = PUEnumTipoMovimientoInventario.Asignacion,
                            IdCentroServicioOrigen = guia.IdCentroLogistico,
                            Bodega = new PUCentroServiciosDC
                            {
                                IdCentroServicio = ControllerContext.Current.IdCentroServicio,
                            },
                            NumeroGuia = guia.NumeroGuia.Value,
                            FechaGrabacion = DateTime.Now,
                            FechaEstimadaIngreso = DateTime.Now,
                            CreadoPor = ControllerContext.Current.Usuario,
                        };
                        fachadaCes.AdicionarMovimientoInventario(movimiento);

                        fachadaCentroAcopio.CambiarTipoEntregaTelemercadeo_REO((long)guia.NumeroGuia, guia.IdCentroServicioDestino);
                        integraRaps = true;

                        transaccion.Complete();
                    }

                }
                else
                {
                    using (TransactionScope transaccion = new TransactionScope())
                    {

                        string observaciones = guia.Observaciones != null ? guia.Observaciones : string.Empty;

                        guia.NuevoEstadoGuia = "DEV";
                        guia.EstaDescargada = true;
                        guia.EstadoGuiaPlanilla = "DEV";
                        guia.TipoImpreso = ADEnumTipoImpreso.Planilla;

                        //cambio de estado pasa a intento de entrega
                        estadoGuia = new ADTrazaGuia
                        {
                            Ciudad = guia.CiudadDestino.Nombre,
                            IdCiudad = guia.CiudadDestino.IdLocalidad,
                            IdAdmision = guia.IdAdmision,
                            IdEstadoGuia = (short)(EstadosGuia.ObtenerUltimoEstado(guia.IdAdmision)),
                            IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.IntentoEntrega,
                            Modulo = COConstantesModulos.PRUEBAS_DE_ENTREGA,
                            NumeroGuia = guia.NumeroGuia,
                            Observaciones = observaciones,
                            FechaGrabacion = guia.FechaMotivoDevolucion
                        };

                        estadoGuia.IdTrazaGuia = EstadosGuia.InsertaEstadoGuia(estadoGuia);

                        if (estadoGuia.IdTrazaGuia == 0)
                        {
                            respuesta.Resultado = OUEnumValidacionDescargue.ErrorEstado;
                            return respuesta;
                        }


                        //Actualiza estado de guia en planilla de mensajero
                        fachadaOpUrbana.ActualizarGuiaMensajero(guia);

                        if (guia.Motivo.IntentoEntrega)
                        {
                            //Actualiza intentos de entrega
                            fachadaMensajeria.ActualizarReintentosEntrega(guia.IdAdmision);
                        }

                        // Traza impreso asocia el cambio de estado con el documento que lo genero
                        ADTrazaGuiaImpresoDC trazaImpreso = new ADTrazaGuiaImpresoDC
                        {
                            IdTrazaGuia = estadoGuia.IdTrazaGuia,
                            NumeroImpreso = guia.Planilla,
                            TipoImpreso = guia.TipoImpreso,
                            Usuario = ControllerContext.Current.Usuario,
                            FechaGrabacion = DateTime.Now,
                        };

                        EstadosGuia.InsertarEstadoGuiaImpreso(trazaImpreso);

                        ADEstadoGuiaMotivoDC estadoMotivoGuia = new ADEstadoGuiaMotivoDC
                        {
                            IdTrazaGuia = estadoGuia.IdTrazaGuia,
                            Motivo = guia.Motivo,
                            Observaciones = string.Empty,
                            FechaMotivo = guia.FechaMotivoDevolucion
                        };

                        //inserta el estado motivo de la devoluciÃ³n
                        EstadosGuia.InsertaEstadoGuiaMotivo(estadoMotivoGuia);

                        /// Se guarda la evidencia de intento de entrega
                        if (guia.Motivo.EsEscaneo && guia.EvidenciaDevolucion.NumeroEvidencia != 0)
                        {
                            if (LIRepositorioPruebasEntrega.Instancia.ValidarEvidenciaDevolucion(
                                guia.EvidenciaDevolucion.NumeroEvidencia, guia.EvidenciaDevolucion.TipoEvidencia.IdTipo))
                            {
                                respuesta.Resultado = OUEnumValidacionDescargue.Error;
                                respuesta.Mensaje = "Numero de volante de devoluciÃ³n ya existe";
                                transaccion.Dispose();
                                return respuesta;
                            }
                            else
                            {
                                // obtener el tipo de evidencia de acuerdo al motivo y el intento de entrega
                                guia.EvidenciaDevolucion.IdEstadoGuialog = estadoGuia.IdTrazaGuia.Value;
                                guia.EvidenciaDevolucion.EstaDigitalizado = false;
                                guia.EvidenciaDevolucion.IdEvidenciaDevolucion = guia.EvidenciaDevolucion.NumeroEvidencia;
                                guia.EvidenciaDevolucion.TipoEvidencia = EGMotivosGuia.ObtenerMotivosEvidencia(guia.Motivo, guia.CantidadReintentosEntrega);

                                LIRepositorioPruebasEntrega.Instancia.AdicionarEvidenciaDevolucion(guia.EvidenciaDevolucion);
                            }
                        }

                        //segunda transicion de estado obtiene el estado a partir del motivo, estado actual e intento de entrega
                        estadoGuia = new ADTrazaGuia
                        {
                            Ciudad = guia.CiudadDestino.Nombre,
                            IdCiudad = guia.CiudadDestino.IdLocalidad,
                            IdAdmision = guia.IdAdmision,
                            IdEstadoGuia = (short)ADEnumEstadoGuia.IntentoEntrega,
                            Modulo = COConstantesModulos.PRUEBAS_DE_ENTREGA,
                            NumeroGuia = guia.NumeroGuia,
                            Observaciones = observaciones,
                            FechaGrabacion = DateTime.Now.AddMilliseconds(500)
                        };

                        estadoGuia.IdNuevoEstadoGuia = EGMotivosGuia.ObtenerEstadoMotivo(guia.Motivo.IdMotivoGuia, (short)(ADEnumEstadoGuia.IntentoEntrega), guia.CantidadReintentosEntrega);

                        if (estadoGuia.IdNuevoEstadoGuia == 0)
                        {
                            estadoGuia.IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.DevolucionRegional;
                        }

                        estadoGuia.IdTrazaGuia = EstadosGuia.InsertaEstadoGuia(estadoGuia);
                        respuesta.Estado = (ADEnumEstadoGuia)estadoGuia.IdNuevoEstadoGuia;

                        if (estadoGuia.IdEstadoGuia == (short)ADEnumEstadoGuia.PendienteIngresoaCustodia)
                        {
                            ADGuia guiaAdmision = fachadaAdmisiones.ObtenerGuiaXNumeroGuia(guia.NumeroGuia.Value);
                            if (!string.IsNullOrEmpty(guiaAdmision.Destinatario.Telefono) && guiaAdmision.Destinatario.Telefono.Length == 10 && guia.TipoCliente == ADEnumTipoCliente.PPE.ToString())
                            {
                                AdministradorMensajesTexto.Instancia.EnviarMensajeTexto(LOIEnumMensajesTexto.Custodia, guiaAdmision.TelefonoDestinatario, guia.NumeroGuia.Value, "");
                            }
                        }

                        PUMovimientoInventario movimiento = new Servicios.ContratoDatos.CentroServicios.PUMovimientoInventario
                        {
                            TipoMovimiento = PUEnumTipoMovimientoInventario.Ingreso,
                            IdCentroServicioOrigen = guia.IdCentroLogistico,
                            Bodega = new PUCentroServiciosDC
                            {
                                IdCentroServicio = ControllerContext.Current.IdCentroServicio,
                            },
                            NumeroGuia = guia.NumeroGuia.Value,
                            FechaGrabacion = DateTime.Now,
                            FechaEstimadaIngreso = DateTime.Now,
                            CreadoPor = ControllerContext.Current.Usuario,
                        };
                        fachadaCes.AdicionarMovimientoInventario(movimiento);

                        //Se valida si el motivo causa supervision, se actualiza la admisiÃ³n
                        if (estadoGuia.IdEstadoGuia == (short)ADEnumEstadoGuia.Auditoria)
                            fachadaMensajeria.ActualizarSupervisionGuia(guia.IdAdmision);

                        if (guia.Novedad != null)
                        {
                            CONovedadGuiaDC novedad = new CONovedadGuiaDC { NumeroGuia = guia.NumeroGuia.Value, TipoNovedad = guia.Novedad };
                            LIRepositorioPruebasEntrega.Instancia.IngresarNovedad(novedad);
                        }

                        if (guia.Motivo.TiempoAfectacion != 0)
                        {
                            EGTipoNovedadGuia.CambiarFechaEntrega(new Servicios.ContratoDatos.Comun.COCambioFechaEntregaDC
                            {
                                Guia = new ADGuia { NumeroGuia = guia.NumeroGuia.Value },
                                TiempoAfectacion = guia.Motivo.TiempoAfectacion
                            });
                        }
                        respuesta.Resultado = OUEnumValidacionDescargue.Exitosa;
                        integraRaps = true;
                        transaccion.Complete();
                    }
                }

                if (integraRaps)
                {
                    //datos.Add("guia", guia);
                    fallaMapper = new RAFallaMapper();
                    RADatosFallaDC datosFalla = fallaMapper.MapperDatosFallaAutomaticaAgenciaGuiaLogistica(guia, RAEnumSistemaOrigen.CONTROLLER.GetHashCode());
                    datosFalla.IdMotivo = datosFalla.IdMotivoGuia;
                    parametrosSolicitudAcumulativa = AdministradorReglaEstadoRAPS.Instancia.IntegrarRapFallas(datosFalla, RAEnumOrigenRaps.Agencias);
                    CrearSolicitudAcumulativa(parametrosSolicitudAcumulativa, datosFalla);
                }
                //IntegrarDevolucionAgencia(guia);
                return respuesta;
            }
            catch (Exception ex)
            {
                var ex1 = (System.ServiceModel.FaultException)ex;
                if (ex1.Code.Name == "Raps0001")
                {
                    respuesta.Resultado = OUEnumValidacionDescargue.ErrorSolicitudRaps;
                }
                else
                {
                    respuesta.Resultado = OUEnumValidacionDescargue.Error;
                }
                respuesta.Mensaje = ex.Message;
                return respuesta;
            }

            IntegrarDevolucionAgencia(guia);

            return respuesta;

        }

        /// <summary>
        /// Integracion para registrar los raps inmediatos en devolucion agencias
        /// </summary>
        /// <param name="guia"></param>
        private void IntegrarDevolucionAgencia(OUGuiaIngresadaDC guia)
        {
            if (guia.Novedad.IdTipoNovedadGuia == (short)LIEnumTipoNovedadGuia.El_envío_no_llegó || guia.Novedad.IdTipoNovedadGuia == (short)LIEnumTipoNovedadGuia.El_envío_llegó_averiado || guia.Novedad.IdTipoNovedadGuia == (short)LIEnumTipoNovedadGuia.El_envío_llegó_saqueado)
            {
                LIEnumTipoNovedadGuia novedad = (LIEnumTipoNovedadGuia)Enum.ToObject(typeof(LIEnumTipoNovedadGuia), guia.Novedad.IdTipoNovedadGuia);
                try
                {
                    RAIntegracionRaps.Instancia.RegistrarSolicitudRapsInmediata(guia, ControllerContext.Current.Usuario, ControllerContext.Current.Identificacion.ToString(), novedad, ControllerContext.Current.IdCentroServicio);
                }
                catch (Exception ex)
                {
                    UtilidadesFW.AuditarExcepcion(ex);
                }
            }
        }

        private OUEnumValidacionDescargue RegistrarNovedadSinMotivo(OUGuiaIngresadaDC guia)
        {
            if (guia.Novedad.IdTipoNovedadGuia == (short)LIEnumTipoNovedadGuia.El_envío_es_para_reclamar_en_oficina)
            {
                using (TransactionScope transaccion = new TransactionScope())
                {
                    this.fachadaCentroAcopio.CambiarTipoEntrega_REO((long)guia.NumeroGuia, guia.IdCentroServicioDestino);
                    EGTipoNovedadGuia.CambiarFechaEntregaCalendario(new Servicios.ContratoDatos.Comun.COCambioFechaEntregaDC
                    {
                        Guia = new ADGuia { NumeroGuia = (long)guia.NumeroGuia },
                        TiempoAfectacion = (30)
                    });
                    transaccion.Complete();

                }
                return OUEnumValidacionDescargue.Exitosa;
            }

            if (guia.Novedad.IdTipoNovedadGuia == (short)LIEnumTipoNovedadGuia.El_envío_no_llegó)
            {
                // TODO: Crear rap por envio no llego 
                return OUEnumValidacionDescargue.Exitosa;
            }

            return OUEnumValidacionDescargue.ErrorEstado;

        }


        /// <summary>
        /// Crea los raps de las fallas mensajero
        /// </summary>
        /// <param name="guia"></param>
        private static void IntegrarRapFallasMensajero(OUGuiaIngresadaDC guia)
        {
            Dictionary<string, object> parametrosParametrizacion = new Dictionary<string, object>();
            Comun.EnumTipoNovedadRaps motivoRaps = Comun.EnumTipoNovedadRaps.Pordefecto;
            if (!ParametrizacionRaps.Instancia.CreaParametrosRaps(guia, out motivoRaps, out parametrosParametrizacion))
            {
                return;
            }

            if (motivoRaps != Comun.EnumTipoNovedadRaps.Pordefecto)
                RAIntegracionRaps.Instancia.CrearSolicitudAcumulativaRaps((Raps.Comun.Integraciones.EnumTipoNovedadRaps)motivoRaps.GetHashCode(), parametrosParametrizacion, guia.IdCiudad.Substring(0, 5), ControllerContext.Current.Usuario);
        }

        /// <summary>
        /// Guarda las devoluciones de mensajero
        /// </summary>
        /// <param name="guia"></param>
        /// <returns></returns>
        public LIDescargueGuiaDC GuardarDevolucion(OUGuiaIngresadaDC guia)
        {
            LIDescargueGuiaDC respuesta = new LIDescargueGuiaDC();
            ADTrazaGuia estadoGuia;

            try
            {

                string observaciones = guia.Observaciones != null ? guia.Observaciones : string.Empty;
                guia.NuevoEstadoGuia = "DEV";
                guia.EstaDescargada = true;
                guia.EstadoGuiaPlanilla = "DEV";
                guia.TipoImpreso = ADEnumTipoImpreso.Planilla;


                ADTrazaGuia estadoActual = EstadosGuia.ObtenerTrazaUltimoEstadoXNumGuia(guia.NumeroGuia.Value);


                //primera transicion de estado pasa a intento de entrega
                estadoGuia = new ADTrazaGuia
                {

                    Ciudad = guia.Ciudad,
                    IdCiudad = guia.IdCiudad,
                    IdAdmision = guia.IdAdmision,
                    IdEstadoGuia = (short)estadoActual.IdEstadoGuia,
                    IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.IntentoEntrega,
                    Modulo = COConstantesModulos.PRUEBAS_DE_ENTREGA,
                    NumeroGuia = guia.NumeroGuia,
                    Observaciones = observaciones,
                    FechaGrabacion = guia.FechaMotivoDevolucion
                };
                bool integraRaps = false;
                using (TransactionScope transaccion = new TransactionScope())
                {
                    ADEstadoGuiaMotivoDC estadoMotivoGuia = new ADEstadoGuiaMotivoDC
                    {
                        Motivo = guia.Motivo,
                        Observaciones = string.Empty,
                        FechaMotivo = guia.FechaMotivoDevolucion
                    };

                    if (estadoActual.IdEstadoGuia != (short)ADEnumEstadoGuia.IntentoEntrega)
                    {
                        estadoGuia.IdTrazaGuia = EstadosGuia.ValidarInsertarEstadoGuia(estadoGuia);

                        if (estadoGuia.IdTrazaGuia == 0)
                        {
                            respuesta.Resultado = OUEnumValidacionDescargue.ErrorEstado;
                            return respuesta;
                        }

                        estadoMotivoGuia.IdTrazaGuia = estadoGuia.IdTrazaGuia;

                        // Traza impreso asocia el cambio de estado con el documento que lo genero
                        ADTrazaGuiaImpresoDC trazaImpreso = new ADTrazaGuiaImpresoDC
                        {
                            IdTrazaGuia = estadoGuia.IdTrazaGuia,
                            NumeroImpreso = guia.Planilla,
                            TipoImpreso = guia.TipoImpreso,
                            Usuario = ControllerContext.Current.Usuario,
                            FechaGrabacion = DateTime.Now,
                        };

                        EstadosGuia.InsertarEstadoGuiaImpreso(trazaImpreso);

                        //inserta el estado motivo de la devoluciÃ³n
                        EstadosGuia.InsertaEstadoGuiaMotivo(estadoMotivoGuia);

                        /// Se guarda la evidencia de intento de entrega
                        if (guia.Motivo.EsEscaneo && guia.EvidenciaDevolucion.NumeroEvidencia != 0)
                        {
                            guia.EvidenciaDevolucion.TipoEvidencia = EGMotivosGuia.ObtenerMotivosEvidencia(guia.Motivo, guia.CantidadReintentosEntrega);

                            if (LIRepositorioPruebasEntrega.Instancia.ValidarEvidenciaDevolucion(guia.EvidenciaDevolucion.NumeroEvidencia, guia.EvidenciaDevolucion.TipoEvidencia.IdTipo))
                            {
                                respuesta.Resultado = OUEnumValidacionDescargue.Error;
                                respuesta.Mensaje = "Numero de volante de devoluciÃ³n ya existe";
                                transaccion.Dispose();
                                return respuesta;
                            }
                            else
                            {
                                // obtener el tipo de evidencia de acuerdo al motivo y el intento de entrega
                                guia.EvidenciaDevolucion.IdEstadoGuialog = estadoGuia.IdTrazaGuia.Value;
                                guia.EvidenciaDevolucion.EstaDigitalizado = false;
                                guia.EvidenciaDevolucion.IdEvidenciaDevolucion = guia.EvidenciaDevolucion.NumeroEvidencia;
                                LIRepositorioPruebasEntrega.Instancia.AdicionarEvidenciaDevolucion(guia.EvidenciaDevolucion);
                            }
                        }

                    }
                    else
                    {
                        // los datos de descargue del app fueron cambiados
                        if (guia.DescargueSupervisado)
                        {
                            estadoMotivoGuia.IdTrazaGuia = guia.EvidenciaDevolucion.IdEstadoGuialog;
                            //editar motivo 
                            EstadosGuia.ActualizarEstadoGuiaMotivo(estadoMotivoGuia);

                            if (guia.Motivo.EsEscaneo && guia.EvidenciaDevolucion.NumeroEvidencia != 0)
                            {
                                if (guia.EvidenciaDevolucion.TipoEvidencia.IdTipo == 0)
                                {
                                    guia.EvidenciaDevolucion.TipoEvidencia = EGMotivosGuia.ObtenerMotivosEvidencia(guia.Motivo, guia.CantidadReintentosEntrega);
                                }
                                if (!LIRepositorioPruebasEntrega.Instancia.ValidarEvidenciaDevolucion(guia.EvidenciaDevolucion.NumeroEvidencia, guia.EvidenciaDevolucion.TipoEvidencia.IdTipo))
                                {
                                    // obtener el tipo de evidencia de acuerdo al motivo y el intento de entrega
                                    guia.EvidenciaDevolucion.IdEstadoGuialog = guia.EvidenciaDevolucion.IdEstadoGuialog;
                                    guia.EvidenciaDevolucion.IdEvidenciaDevolucion = guia.EvidenciaDevolucion.NumeroEvidencia;
                                    LIRepositorioPruebasEntrega.Instancia.ModificarEvidenciaDevolucion(guia.EvidenciaDevolucion);
                                }
                            }
                        }
                    }


                    //Actualiza intentos de entrega
                    if (guia.Motivo.IntentoEntrega)
                    {
                        fachadaMensajeria.ActualizarReintentosEntrega(guia.IdAdmision);
                    }

                    //Actualiza estado de guia en planilla de mensajero
                    fachadaOpUrbana.ActualizarGuiaMensajero(guia);


                    //segunda transicion de estado obtiene el estado a partir del motivo, estado actual e intento de entrega
                    estadoGuia = new ADTrazaGuia
                    {
                        Ciudad = guia.Ciudad,
                        IdCiudad = guia.IdCiudad,
                        IdAdmision = guia.IdAdmision,
                        IdEstadoGuia = (short)ADEnumEstadoGuia.IntentoEntrega,
                        Modulo = COConstantesModulos.PRUEBAS_DE_ENTREGA,
                        NumeroGuia = guia.NumeroGuia,
                        Observaciones = observaciones,
                        FechaGrabacion = DateTime.Now.AddMilliseconds(500)
                    };

                    estadoGuia.IdNuevoEstadoGuia = EGMotivosGuia.ObtenerEstadoMotivo(guia.Motivo.IdMotivoGuia, (short)(ADEnumEstadoGuia.IntentoEntrega), guia.CantidadReintentosEntrega);

                    if (estadoGuia.IdNuevoEstadoGuia == 0)
                    {
                        estadoGuia.IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.Auditoria;
                    }


                    if (guia.TipoCliente == ADEnumTipoCliente.INT.ToString() || (estadoGuia.IdNuevoEstadoGuia == (short)ADEnumEstadoGuia.DevolucionRatificada && guia.EsAlCobro && !guia.EstaPagada))
                    {
                        estadoGuia.IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.Telemercadeo;
                    }

                    estadoGuia.IdTrazaGuia = EstadosGuia.InsertaEstadoGuia(estadoGuia);
                    respuesta.Estado = (ADEnumEstadoGuia)estadoGuia.IdNuevoEstadoGuia;


                    PUMovimientoInventario movimiento = new Servicios.ContratoDatos.CentroServicios.PUMovimientoInventario
                    {
                        TipoMovimiento = PUEnumTipoMovimientoInventario.Ingreso,
                        IdCentroServicioOrigen = guia.IdCentroLogistico,
                        Bodega = new PUCentroServiciosDC
                        {
                            IdCentroServicio = ControllerContext.Current.IdCentroServicio,
                        },
                        NumeroGuia = guia.NumeroGuia.Value,
                        FechaGrabacion = DateTime.Now,
                        FechaEstimadaIngreso = DateTime.Now,
                        CreadoPor = ControllerContext.Current.Usuario,
                    };
                    fachadaCes.AdicionarMovimientoInventario(movimiento);



                    // si el motivo es incautado envia mensaje de texto
                    if (guia.Motivo.IdMotivoGuia == 138)
                    {
                        ADGuia guiaAdmision = fachadaAdmisiones.ObtenerGuiaXNumeroGuia(guia.NumeroGuia.Value);
                        if (!string.IsNullOrEmpty(guiaAdmision.Destinatario.Telefono) && guiaAdmision.Destinatario.Telefono.Length == 10)
                        {
                            AdministradorMensajesTexto.Instancia.EnviarMensajeTexto(LOIEnumMensajesTexto.Incautado, guiaAdmision.TelefonoDestinatario, guia.NumeroGuia.Value, "");

                        }
                    }

                    // si el motivo es hurto envia mensaje de texto
                    else if (guia.Motivo.IdMotivoGuia == 139)
                    {
                        ADGuia guiaAdmision = fachadaAdmisiones.ObtenerGuiaXNumeroGuia(guia.NumeroGuia.Value);
                        if (!string.IsNullOrEmpty(guiaAdmision.Destinatario.Telefono) && guiaAdmision.Destinatario.Telefono.Length == 10)
                        {
                            AdministradorMensajesTexto.Instancia.EnviarMensajeTexto(LOIEnumMensajesTexto.Hurto, guiaAdmision.TelefonoDestinatario, guia.NumeroGuia.Value, "");

                        }
                    }

                    if (estadoGuia.IdEstadoGuia == (short)ADEnumEstadoGuia.PendienteIngresoaCustodia)
                    {
                        ADGuia guiaAdmision = fachadaAdmisiones.ObtenerGuiaXNumeroGuia(guia.NumeroGuia.Value);
                        if (!string.IsNullOrEmpty(guiaAdmision.Destinatario.Telefono) && guiaAdmision.Destinatario.Telefono.Length == 10 && guia.TipoCliente == ADEnumTipoCliente.PPE.ToString())
                        {
                            AdministradorMensajesTexto.Instancia.EnviarMensajeTexto(LOIEnumMensajesTexto.Custodia, guiaAdmision.TelefonoDestinatario, guia.NumeroGuia.Value, "");

                        }

                    }

                    //Se valida si el motivo causa supervision, se actualiza la admisiÃ³n
                    if (estadoGuia.IdEstadoGuia == (short)ADEnumEstadoGuia.Auditoria)
                        fachadaMensajeria.ActualizarSupervisionGuia(guia.IdAdmision);

                    if (guia.Novedad != null)
                    {
                        CONovedadGuiaDC novedad = new CONovedadGuiaDC { NumeroGuia = guia.NumeroGuia.Value, TipoNovedad = guia.Novedad };
                        LIRepositorioPruebasEntrega.Instancia.IngresarNovedad(novedad);
                    }


                    if (guia.Motivo.TiempoAfectacion != 0)
                    {
                        EGTipoNovedadGuia.CambiarFechaEntrega(new Servicios.ContratoDatos.Comun.COCambioFechaEntregaDC
                        {
                            Guia = new ADGuia { NumeroGuia = guia.NumeroGuia.Value },
                            TiempoAfectacion = guia.Motivo.TiempoAfectacion
                        });
                    }

                    integraRaps = true;

                    respuesta.Resultado = OUEnumValidacionDescargue.Exitosa;
                    transaccion.Complete();

                }

                if (integraRaps)
                {
                    IntegrarRapFallasMensajero(guia);
                }
                return respuesta;
            }
            catch (Exception ex)
            {
                respuesta.Resultado = OUEnumValidacionDescargue.Error;
                respuesta.Mensaje = ex.Message;
                return respuesta;
            }
        }


        /// <summary>
        /// Guarda las devoluciones de mensajero
        /// </summary>
        /// <param name="guia"></param>
        /// <returns></returns>
        public LIDescargueGuiaDC GuardarDevolucionV7(OUGuiaIngresadaDC guia)
        {
            LIDescargueGuiaDC respuesta = new LIDescargueGuiaDC();
            ADTrazaGuia estadoGuia;
            IDictionary<string, object> datos = null;

            try
            {

                string observaciones = guia.Observaciones != null ? guia.Observaciones : string.Empty;
                guia.NuevoEstadoGuia = "DEV";
                guia.EstaDescargada = true;
                guia.EstadoGuiaPlanilla = "DEV";
                guia.TipoImpreso = ADEnumTipoImpreso.Planilla;


                ADTrazaGuia estadoActual = EstadosGuia.ObtenerTrazaUltimoEstadoXNumGuia(guia.NumeroGuia.Value);


                //primera transicion de estado pasa a intento de entrega
                estadoGuia = new ADTrazaGuia
                {
                    Ciudad = guia.Ciudad,
                    IdCiudad = guia.IdCiudad,
                    IdAdmision = guia.IdAdmision,
                    IdEstadoGuia = (short)estadoActual.IdEstadoGuia,
                    IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.IntentoEntrega,
                    Modulo = COConstantesModulos.PRUEBAS_DE_ENTREGA,
                    NumeroGuia = guia.NumeroGuia,
                    Observaciones = observaciones,
                    FechaGrabacion = guia.FechaMotivoDevolucion
                };

                bool integraRaps = false;
                using (TransactionScope transaccion = new TransactionScope())
                {
                    ADEstadoGuiaMotivoDC estadoMotivoGuia = new ADEstadoGuiaMotivoDC
                    {
                        Motivo = guia.Motivo,
                        Observaciones = string.Empty,
                        FechaMotivo = guia.FechaMotivoDevolucion
                    };

                    if (estadoActual.IdEstadoGuia != (short)ADEnumEstadoGuia.IntentoEntrega)
                    {
                        estadoGuia.IdTrazaGuia = EstadosGuia.ValidarInsertarEstadoGuia(estadoGuia);

                        if (estadoGuia.IdTrazaGuia == 0)
                        {
                            respuesta.Resultado = OUEnumValidacionDescargue.ErrorEstado;
                            return respuesta;
                        }

                        estadoMotivoGuia.IdTrazaGuia = estadoGuia.IdTrazaGuia;

                        // Traza impreso asocia el cambio de estado con el documento que lo genero
                        ADTrazaGuiaImpresoDC trazaImpreso = new ADTrazaGuiaImpresoDC
                        {
                            IdTrazaGuia = estadoGuia.IdTrazaGuia,
                            NumeroImpreso = guia.Planilla,
                            TipoImpreso = guia.TipoImpreso,
                            Usuario = ControllerContext.Current.Usuario,
                            FechaGrabacion = DateTime.Now,
                        };

                        EstadosGuia.InsertarEstadoGuiaImpreso(trazaImpreso);

                        //inserta el estado motivo de la devoluciÃ³n
                        EstadosGuia.InsertaEstadoGuiaMotivo(estadoMotivoGuia);

                        /// Se guarda la evidencia de intento de entrega
                        if (guia.Motivo.EsEscaneo && guia.EvidenciaDevolucion.NumeroEvidencia != 0)
                        {
                            guia.EvidenciaDevolucion.TipoEvidencia = EGMotivosGuia.ObtenerMotivosEvidencia(guia.Motivo, guia.CantidadReintentosEntrega);

                            if (LIRepositorioPruebasEntrega.Instancia.ValidarEvidenciaDevolucion(guia.EvidenciaDevolucion.NumeroEvidencia, guia.EvidenciaDevolucion.TipoEvidencia.IdTipo))
                            {
                                respuesta.Resultado = OUEnumValidacionDescargue.Error;
                                respuesta.Mensaje = "Numero de volante de devoluciÃ³n ya existe";
                                transaccion.Dispose();
                                return respuesta;
                            }
                            else
                            {
                                // obtener el tipo de evidencia de acuerdo al motivo y el intento de entrega
                                guia.EvidenciaDevolucion.IdEstadoGuialog = estadoGuia.IdTrazaGuia.Value;
                                guia.EvidenciaDevolucion.EstaDigitalizado = false;
                                guia.EvidenciaDevolucion.IdEvidenciaDevolucion = guia.EvidenciaDevolucion.NumeroEvidencia;
                                LIRepositorioPruebasEntrega.Instancia.AdicionarEvidenciaDevolucion(guia.EvidenciaDevolucion);
                            }
                        }

                    }
                    else
                    {
                        // los datos de descargue del app fueron cambiados
                        if (guia.DescargueSupervisado)
                        {
                            estadoMotivoGuia.IdTrazaGuia = guia.EvidenciaDevolucion.IdEstadoGuialog;
                            //editar motivo 
                            EstadosGuia.ActualizarEstadoGuiaMotivo(estadoMotivoGuia);

                            if (guia.Motivo.EsEscaneo && guia.EvidenciaDevolucion.NumeroEvidencia != 0)
                            {
                                if (guia.EvidenciaDevolucion.TipoEvidencia.IdTipo == 0)
                                {
                                    guia.EvidenciaDevolucion.TipoEvidencia = EGMotivosGuia.ObtenerMotivosEvidencia(guia.Motivo, guia.CantidadReintentosEntrega);
                                }
                                if (!LIRepositorioPruebasEntrega.Instancia.ValidarEvidenciaDevolucion(guia.EvidenciaDevolucion.NumeroEvidencia, guia.EvidenciaDevolucion.TipoEvidencia.IdTipo))
                                {
                                    // obtener el tipo de evidencia de acuerdo al motivo y el intento de entrega
                                    guia.EvidenciaDevolucion.IdEstadoGuialog = guia.EvidenciaDevolucion.IdEstadoGuialog;
                                    guia.EvidenciaDevolucion.IdEvidenciaDevolucion = guia.EvidenciaDevolucion.NumeroEvidencia;
                                    LIRepositorioPruebasEntrega.Instancia.ModificarEvidenciaDevolucion(guia.EvidenciaDevolucion);
                                }
                            }
                        }
                    }


                    //Actualiza intentos de entrega
                    if (guia.Motivo.IntentoEntrega)
                    {
                        fachadaMensajeria.ActualizarReintentosEntrega(guia.IdAdmision);
                    }

                    //Actualiza estado de guia en planilla de mensajero
                    fachadaOpUrbana.ActualizarGuiaMensajero(guia);


                    //segunda transicion de estado obtiene el estado a partir del motivo, estado actual e intento de entrega
                    estadoGuia = new ADTrazaGuia
                    {
                        Ciudad = guia.Ciudad,
                        IdCiudad = guia.IdCiudad,
                        IdAdmision = guia.IdAdmision,
                        IdEstadoGuia = (short)ADEnumEstadoGuia.IntentoEntrega,
                        Modulo = COConstantesModulos.PRUEBAS_DE_ENTREGA,
                        NumeroGuia = guia.NumeroGuia,
                        Observaciones = observaciones,
                        FechaGrabacion = DateTime.Now.AddMilliseconds(500)
                    };

                    estadoGuia.IdNuevoEstadoGuia = EGMotivosGuia.ObtenerEstadoMotivo(guia.Motivo.IdMotivoGuia, (short)(ADEnumEstadoGuia.IntentoEntrega), guia.CantidadReintentosEntrega);

                    if (estadoGuia.IdNuevoEstadoGuia == 0)
                    {
                        estadoGuia.IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.Auditoria;
                    }


                    if (guia.TipoCliente == ADEnumTipoCliente.INT.ToString() || (estadoGuia.IdNuevoEstadoGuia == (short)ADEnumEstadoGuia.DevolucionRatificada && guia.EsAlCobro && !guia.EstaPagada))
                    {
                        estadoGuia.IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.Telemercadeo;
                    }

                    estadoGuia.IdTrazaGuia = EstadosGuia.InsertaEstadoGuia(estadoGuia);
                    respuesta.Estado = (ADEnumEstadoGuia)estadoGuia.IdNuevoEstadoGuia;


                    PUMovimientoInventario movimiento = new Servicios.ContratoDatos.CentroServicios.PUMovimientoInventario
                    {
                        TipoMovimiento = PUEnumTipoMovimientoInventario.Ingreso,
                        IdCentroServicioOrigen = guia.IdCentroLogistico,
                        Bodega = new PUCentroServiciosDC
                        {
                            IdCentroServicio = ControllerContext.Current.IdCentroServicio,
                        },
                        NumeroGuia = guia.NumeroGuia.Value,
                        FechaGrabacion = DateTime.Now,
                        FechaEstimadaIngreso = DateTime.Now,
                        CreadoPor = ControllerContext.Current.Usuario,
                    };
                    fachadaCes.AdicionarMovimientoInventario(movimiento);



                    // si el motivo es incautado envia mensaje de texto
                    if (guia.Motivo.IdMotivoGuia == 138)
                    {
                        ADGuia guiaAdmision = fachadaAdmisiones.ObtenerGuiaXNumeroGuia(guia.NumeroGuia.Value);
                        if (!string.IsNullOrEmpty(guiaAdmision.Destinatario.Telefono) && guiaAdmision.Destinatario.Telefono.Length == 10)
                        {
                            AdministradorMensajesTexto.Instancia.EnviarMensajeTexto(LOIEnumMensajesTexto.Incautado, guiaAdmision.TelefonoDestinatario, guia.NumeroGuia.Value, "");

                        }
                    }

                    // si el motivo es hurto envia mensaje de texto
                    else if (guia.Motivo.IdMotivoGuia == 139)
                    {
                        ADGuia guiaAdmision = fachadaAdmisiones.ObtenerGuiaXNumeroGuia(guia.NumeroGuia.Value);
                        if (!string.IsNullOrEmpty(guiaAdmision.Destinatario.Telefono) && guiaAdmision.Destinatario.Telefono.Length == 10)
                        {
                            AdministradorMensajesTexto.Instancia.EnviarMensajeTexto(LOIEnumMensajesTexto.Hurto, guiaAdmision.TelefonoDestinatario, guia.NumeroGuia.Value, "");

                        }
                    }

                    if (estadoGuia.IdEstadoGuia == (short)ADEnumEstadoGuia.PendienteIngresoaCustodia)
                    {
                        ADGuia guiaAdmision = fachadaAdmisiones.ObtenerGuiaXNumeroGuia(guia.NumeroGuia.Value);
                        if (!string.IsNullOrEmpty(guiaAdmision.Destinatario.Telefono) && guiaAdmision.Destinatario.Telefono.Length == 10 && guia.TipoCliente == ADEnumTipoCliente.PPE.ToString())
                        {
                            AdministradorMensajesTexto.Instancia.EnviarMensajeTexto(LOIEnumMensajesTexto.Custodia, guiaAdmision.TelefonoDestinatario, guia.NumeroGuia.Value, "");

                        }

                    }

                    //Se valida si el motivo causa supervision, se actualiza la admisiÃ³n
                    if (estadoGuia.IdEstadoGuia == (short)ADEnumEstadoGuia.Auditoria)
                        fachadaMensajeria.ActualizarSupervisionGuia(guia.IdAdmision);

                    if (guia.Novedad != null)
                    {
                        CONovedadGuiaDC novedad = new CONovedadGuiaDC { NumeroGuia = guia.NumeroGuia.Value, TipoNovedad = guia.Novedad };
                        LIRepositorioPruebasEntrega.Instancia.IngresarNovedad(novedad);
                    }


                    if (guia.Motivo.TiempoAfectacion != 0)
                    {
                        EGTipoNovedadGuia.CambiarFechaEntrega(new Servicios.ContratoDatos.Comun.COCambioFechaEntregaDC
                        {
                            Guia = new ADGuia { NumeroGuia = guia.NumeroGuia.Value },
                            TiempoAfectacion = guia.Motivo.TiempoAfectacion
                        });
                    }

                    integraRaps = true;

                    respuesta.Resultado = OUEnumValidacionDescargue.Exitosa;
                    transaccion.Complete();

                }

                if (integraRaps)
                {
                    fallaMapper = new RAFallaMapper();
                    RADatosFallaDC datosFalla = fallaMapper.MapperDatosFallaAutomaticaMensajeroGuiaLogistica(guia, RAEnumSistemaOrigen.CONTROLLER.GetHashCode());
                    datosFalla.IdMotivo = datosFalla.IdMotivoGuia;
                    parametrosSolicitudAcumulativa = AdministradorReglaEstadoRAPS.Instancia.IntegrarRapFallas(datosFalla, RAEnumOrigenRaps.Mensajero);
                    CrearSolicitudAcumulativa(parametrosSolicitudAcumulativa, datosFalla);
                }
                return respuesta;

            }
            catch (Exception ex)
            {
                respuesta.Resultado = OUEnumValidacionDescargue.Error;
                respuesta.Mensaje = ex.Message;
                return respuesta;
            }
        }

        /// <summary>
        /// Guarda las devoluciones de auditoria
        /// </summary>
        /// <param name="guia"></param>
        /// <returns></returns>
        public LIDescargueGuiaDC GuardarDevolucionCol(OUGuiaIngresadaDC guia)
        {
            LIDescargueGuiaDC respuesta = new LIDescargueGuiaDC();
            ADTrazaGuia estadoGuia;

            try
            {
                ADTrazaGuia estadoActual = EstadosGuia.ObtenerTrazaUltimoEstadoXNumGuia(guia.NumeroGuia.Value);
                guia.IdAdmision = estadoActual.IdAdmision.Value;

                if (estadoActual.IdEstadoGuia != (short)ADEnumEstadoGuia.CentroAcopio || guia.IdCentroLogistico != estadoActual.IdCentroServicioEstado)
                {
                    respuesta.Resultado = OUEnumValidacionDescargue.ErrorEstado;
                    respuesta.Mensaje = "La guia no se encuentra en centro de acopio";
                    return respuesta;
                }

                using (TransactionScope transaccion = new TransactionScope())
                {
                    ADGuia guiaAdmision = fachadaAdmisiones.ObtenerGuiaXNumeroGuia(guia.NumeroGuia.Value);

                    if (!string.IsNullOrEmpty(guiaAdmision.Destinatario.Telefono) && guiaAdmision.Destinatario.Telefono.Length == 10)
                    {
                        if (guia.Motivo.IdMotivoGuia == 159)
                        {
                            AdministradorMensajesTexto.Instancia.EnviarMensajeTexto(LOIEnumMensajesTexto.Incautado, guiaAdmision.TelefonoDestinatario, guia.NumeroGuia.Value, "");

                        }
                        else if (guia.Motivo.IdMotivoGuia == 160)
                        {
                            AdministradorMensajesTexto.Instancia.EnviarMensajeTexto(LOIEnumMensajesTexto.Hurto, guiaAdmision.TelefonoDestinatario, guia.NumeroGuia.Value, "");

                        }
                    }

                    string observaciones = guia.Observaciones != null ? guia.Observaciones : string.Empty;



                    //Afectar caja de destino si al cobro fue pagado automaticamente por vencimiento
                    if (guiaAdmision.EsAlCobro && guiaAdmision.EstaPagada)
                    {
                        if (fachadaCajas.InsertarDescuentoAlCobroDevuelto(guiaAdmision.NumeroGuia, guiaAdmision.IdCentroServicioDestino, guiaAdmision.IdAdmision))
                        {
                            guiaAdmision.EstaPagada = false;
                        }
                    }

                    //primera transicion de estado
                    estadoGuia = new ADTrazaGuia
                    {
                        Ciudad = estadoActual.Ciudad,
                        IdCiudad = estadoActual.IdCiudad,
                        IdAdmision = guia.IdAdmision,
                        Modulo = COConstantesModulos.PRUEBAS_DE_ENTREGA,
                        NumeroGuia = guia.NumeroGuia,
                        Observaciones = observaciones,
                        FechaGrabacion = guia.FechaMotivoDevolucion
                    };


                    estadoGuia.IdNuevoEstadoGuia = EGMotivosGuia.ObtenerEstadoMotivo(guia.Motivo.IdMotivoGuia, (short)(ADEnumEstadoGuia.IntentoEntrega), guia.CantidadReintentosEntrega);

                    if (estadoGuia.IdNuevoEstadoGuia == 0 && guiaAdmision.TipoCliente != ADEnumTipoCliente.INT)
                    {
                        if ((guiaAdmision.EsAlCobro && !guiaAdmision.EstaPagada) || (!guiaAdmision.EsAlCobro && guiaAdmision.Peso > 2 && guiaAdmision.TipoCliente == ADEnumTipoCliente.PPE))
                        {
                            estadoGuia.IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.PendienteIngresoaCustodia;
                            //Se crea tapa de logistica inversa
                            LITapaLogisticaDC tapaLogistica = new LITapaLogisticaDC
                            {
                                NumeroGuia = guiaAdmision.NumeroGuia,
                                NumeroTapaLogistica = null,
                                Tipo = LIEnumTipoTapaLogisticaDC.Custodia,
                                Impresa = false,
                            };
                            LITapasLogisticaInversa.Instancia.AdicionarTapaLogistica(tapaLogistica);
                        }
                        else
                        {
                            estadoGuia.IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.DevolucionRatificada;
                        }
                    }
                    else if (estadoGuia.IdNuevoEstadoGuia == 0 && guiaAdmision.TipoCliente == ADEnumTipoCliente.INT)
                    {
                        estadoGuia.IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.PendienteIngresoaCustodia;
                    }
                    else if (estadoGuia.IdNuevoEstadoGuia == (short)ADEnumEstadoGuia.DevolucionRatificada && guiaAdmision.EsAlCobro && !guiaAdmision.EstaPagada)
                    {
                        estadoGuia.IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.Telemercadeo;
                    }


                    estadoGuia.IdTrazaGuia = EstadosGuia.InsertaEstadoGuia(estadoGuia);

                    ADEstadoGuiaMotivoDC estadoMotivoGuia = new ADEstadoGuiaMotivoDC
                    {
                        IdTrazaGuia = estadoGuia.IdTrazaGuia,
                        Motivo = guia.Motivo,
                        Observaciones = string.Empty,
                        FechaMotivo = guia.FechaMotivoDevolucion
                    };


                    //inserta el estado motivo de la devoluciÃ³n
                    EstadosGuia.InsertaEstadoGuiaMotivo(estadoMotivoGuia);

                    /// Se guarda la evidencia de intento de entrega
                    if (guia.Motivo.EsEscaneo && guia.EvidenciaDevolucion.NumeroEvidencia != 0)
                    {
                        guia.EvidenciaDevolucion.TipoEvidencia = EGMotivosGuia.ObtenerMotivosEvidencia(guia.Motivo, guia.CantidadReintentosEntrega);

                        if (LIRepositorioPruebasEntrega.Instancia.ValidarEvidenciaDevolucion(guia.EvidenciaDevolucion.NumeroEvidencia, guia.EvidenciaDevolucion.TipoEvidencia.IdTipo))
                        {
                            respuesta.Resultado = OUEnumValidacionDescargue.Error;
                            respuesta.Mensaje = "Numero de volante de devoluciÃ³n ya existe";
                            transaccion.Dispose();
                            return respuesta;
                        }
                        else
                        {
                            // obtener el tipo de evidencia de acuerdo al motivo y el intento de entrega
                            guia.EvidenciaDevolucion.IdEstadoGuialog = estadoGuia.IdTrazaGuia.Value;
                            guia.EvidenciaDevolucion.EstaDigitalizado = false;
                            guia.EvidenciaDevolucion.IdEvidenciaDevolucion = guia.EvidenciaDevolucion.NumeroEvidencia;

                            LIRepositorioPruebasEntrega.Instancia.AdicionarEvidenciaDevolucion(guia.EvidenciaDevolucion);
                        }
                    }

                    if (guia.Motivo.TiempoAfectacion != 0)
                    {
                        EGTipoNovedadGuia.CambiarFechaEntrega(new Servicios.ContratoDatos.Comun.COCambioFechaEntregaDC
                        {
                            Guia = new ADGuia { NumeroGuia = guia.NumeroGuia.Value },
                            TiempoAfectacion = guia.Motivo.TiempoAfectacion
                        });
                    }


                    if (guia.Motivo.IntentoEntrega)
                    {
                        //Actualiza intentos de entrega
                        fachadaMensajeria.ActualizarReintentosEntrega(guia.IdAdmision);
                    }


                    PUMovimientoInventario movimiento = new Servicios.ContratoDatos.CentroServicios.PUMovimientoInventario
                    {
                        TipoMovimiento = PUEnumTipoMovimientoInventario.Ingreso,
                        IdCentroServicioOrigen = guia.IdCentroLogistico,
                        Bodega = new PUCentroServiciosDC
                        {
                            IdCentroServicio = ControllerContext.Current.IdCentroServicio,
                        },
                        NumeroGuia = guia.NumeroGuia.Value,
                        FechaGrabacion = DateTime.Now,
                        FechaEstimadaIngreso = DateTime.Now,
                        CreadoPor = ControllerContext.Current.Usuario,
                    };

                    fachadaCes.AdicionarMovimientoInventario(movimiento);


                    //Se valida si el motivo causa supervision, se actualiza la admisiÃ³n
                    if (estadoGuia.IdEstadoGuia == (short)ADEnumEstadoGuia.Auditoria)
                        fachadaMensajeria.ActualizarSupervisionGuia(guia.IdAdmision);


                    if (guia.Novedad != null)
                    {
                        CONovedadGuiaDC novedad = new CONovedadGuiaDC { NumeroGuia = guia.NumeroGuia.Value, TipoNovedad = guia.Novedad };
                        LIRepositorioPruebasEntrega.Instancia.IngresarNovedad(novedad);
                    }

                    respuesta.Estado = (ADEnumEstadoGuia)estadoGuia.IdNuevoEstadoGuia;
                    respuesta.Resultado = OUEnumValidacionDescargue.Exitosa;
                    transaccion.Complete();
                    return respuesta;
                }
            }
            catch (Exception ex)
            {
                respuesta.Resultado = OUEnumValidacionDescargue.Error;
                respuesta.Mensaje = ex.Message;
                return respuesta;
            }

        }

        /// <summary>
        /// Guarda las devoluciones de auditoria
        /// </summary>
        /// <param name="guia"></param>
        /// <returns></returns>
        public LIDescargueGuiaDC GuardarDevolucionColV7(OUGuiaIngresadaDC guia)
        {
            LIDescargueGuiaDC respuesta = new LIDescargueGuiaDC();
            ADTrazaGuia estadoGuia;
            bool integraRaps = false;
            IDictionary<string, object> datos = null;

            try
            {
                ADTrazaGuia estadoActual = EstadosGuia.ObtenerTrazaUltimoEstadoXNumGuia(guia.NumeroGuia.Value);
                guia.IdAdmision = estadoActual.IdAdmision.Value;

                if (estadoActual.IdEstadoGuia != (short)ADEnumEstadoGuia.CentroAcopio || guia.IdCentroLogistico != estadoActual.IdCentroServicioEstado)
                {
                    respuesta.Resultado = OUEnumValidacionDescargue.ErrorEstado;
                    respuesta.Mensaje = "La guia no se encuentra en centro de acopio";
                    return respuesta;
                }

                using (TransactionScope transaccion = new TransactionScope())
                {
                    ADGuia guiaAdmision = fachadaAdmisiones.ObtenerGuiaXNumeroGuia(guia.NumeroGuia.Value);
                    guia.IdCentroServicioOrigen = guiaAdmision.IdCentroServicioOrigen;
                    guia.NombreCentroServicioOrigen = guiaAdmision.NombreCentroServicioOrigen;
                    guia.IdCiudad = guiaAdmision.IdCiudadOrigen;



                    if (!string.IsNullOrEmpty(guiaAdmision.Destinatario.Telefono) && guiaAdmision.Destinatario.Telefono.Length == 10)
                    {
                        if (guia.Motivo.IdMotivoGuia == 159)
                        {
                            AdministradorMensajesTexto.Instancia.EnviarMensajeTexto(LOIEnumMensajesTexto.Incautado, guiaAdmision.TelefonoDestinatario, guia.NumeroGuia.Value, "");

                        }
                        else if (guia.Motivo.IdMotivoGuia == 160)
                        {
                            AdministradorMensajesTexto.Instancia.EnviarMensajeTexto(LOIEnumMensajesTexto.Hurto, guiaAdmision.TelefonoDestinatario, guia.NumeroGuia.Value, "");

                        }
                    }

                    string observaciones = guia.Observaciones != null ? guia.Observaciones : string.Empty;



                    //Afectar caja de destino si al cobro fue pagado automaticamente por vencimiento
                    if (guiaAdmision.EsAlCobro && guiaAdmision.EstaPagada)
                    {
                        if (fachadaCajas.InsertarDescuentoAlCobroDevuelto(guiaAdmision.NumeroGuia, guiaAdmision.IdCentroServicioDestino, guiaAdmision.IdAdmision))
                        {
                            guiaAdmision.EstaPagada = false;
                        }
                    }

                    //primera transicion de estado
                    estadoGuia = new ADTrazaGuia
                    {
                        Ciudad = estadoActual.Ciudad,
                        IdCiudad = estadoActual.IdCiudad,
                        IdAdmision = guia.IdAdmision,
                        Modulo = COConstantesModulos.PRUEBAS_DE_ENTREGA,
                        NumeroGuia = guia.NumeroGuia,
                        Observaciones = observaciones,
                        FechaGrabacion = guia.FechaMotivoDevolucion
                    };


                    estadoGuia.IdNuevoEstadoGuia = EGMotivosGuia.ObtenerEstadoMotivo(guia.Motivo.IdMotivoGuia, (short)(ADEnumEstadoGuia.IntentoEntrega), guia.CantidadReintentosEntrega);

                    if (estadoGuia.IdNuevoEstadoGuia == 0 && guiaAdmision.TipoCliente != ADEnumTipoCliente.INT)
                    {
                        if ((guiaAdmision.EsAlCobro && !guiaAdmision.EstaPagada) || (!guiaAdmision.EsAlCobro && guiaAdmision.Peso > 2 && guiaAdmision.TipoCliente == ADEnumTipoCliente.PPE))
                        {
                            estadoGuia.IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.PendienteIngresoaCustodia;
                            //Se crea tapa de logistica inversa
                            LITapaLogisticaDC tapaLogistica = new LITapaLogisticaDC
                            {
                                NumeroGuia = guiaAdmision.NumeroGuia,
                                NumeroTapaLogistica = null,
                                Tipo = LIEnumTipoTapaLogisticaDC.Custodia,
                                Impresa = false,
                            };
                            LITapasLogisticaInversa.Instancia.AdicionarTapaLogistica(tapaLogistica);
                        }
                        else
                        {
                            estadoGuia.IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.DevolucionRatificada;
                        }
                    }
                    else if (estadoGuia.IdNuevoEstadoGuia == 0 && guiaAdmision.TipoCliente == ADEnumTipoCliente.INT)
                    {
                        estadoGuia.IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.PendienteIngresoaCustodia;
                    }
                    else if (estadoGuia.IdNuevoEstadoGuia == (short)ADEnumEstadoGuia.DevolucionRatificada && guiaAdmision.EsAlCobro && !guiaAdmision.EstaPagada)
                    {
                        estadoGuia.IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.Telemercadeo;
                    }


                    estadoGuia.IdTrazaGuia = EstadosGuia.InsertaEstadoGuia(estadoGuia);

                    ADEstadoGuiaMotivoDC estadoMotivoGuia = new ADEstadoGuiaMotivoDC
                    {
                        IdTrazaGuia = estadoGuia.IdTrazaGuia,
                        Motivo = guia.Motivo,
                        Observaciones = string.Empty,
                        FechaMotivo = guia.FechaMotivoDevolucion
                    };


                    //inserta el estado motivo de la devoluciÃ³n
                    EstadosGuia.InsertaEstadoGuiaMotivo(estadoMotivoGuia);

                    /// Se guarda la evidencia de intento de entrega
                    if (guia.Motivo.EsEscaneo && guia.EvidenciaDevolucion.NumeroEvidencia != 0)
                    {
                        guia.EvidenciaDevolucion.TipoEvidencia = EGMotivosGuia.ObtenerMotivosEvidencia(guia.Motivo, guia.CantidadReintentosEntrega);

                        if (LIRepositorioPruebasEntrega.Instancia.ValidarEvidenciaDevolucion(guia.EvidenciaDevolucion.NumeroEvidencia, guia.EvidenciaDevolucion.TipoEvidencia.IdTipo))
                        {
                            respuesta.Resultado = OUEnumValidacionDescargue.Error;
                            respuesta.Mensaje = "Numero de volante de devoluciÃ³n ya existe";
                            transaccion.Dispose();
                            return respuesta;
                        }
                        else
                        {
                            // obtener el tipo de evidencia de acuerdo al motivo y el intento de entrega
                            guia.EvidenciaDevolucion.IdEstadoGuialog = estadoGuia.IdTrazaGuia.Value;
                            guia.EvidenciaDevolucion.EstaDigitalizado = false;
                            guia.EvidenciaDevolucion.IdEvidenciaDevolucion = guia.EvidenciaDevolucion.NumeroEvidencia;

                            LIRepositorioPruebasEntrega.Instancia.AdicionarEvidenciaDevolucion(guia.EvidenciaDevolucion);
                        }
                    }

                    if (guia.Motivo.TiempoAfectacion != 0)
                    {
                        EGTipoNovedadGuia.CambiarFechaEntrega(new Servicios.ContratoDatos.Comun.COCambioFechaEntregaDC
                        {
                            Guia = new ADGuia { NumeroGuia = guia.NumeroGuia.Value },
                            TiempoAfectacion = guia.Motivo.TiempoAfectacion
                        });
                    }


                    if (guia.Motivo.IntentoEntrega)
                    {
                        //Actualiza intentos de entrega
                        fachadaMensajeria.ActualizarReintentosEntrega(guia.IdAdmision);
                    }


                    PUMovimientoInventario movimiento = new Servicios.ContratoDatos.CentroServicios.PUMovimientoInventario
                    {
                        TipoMovimiento = PUEnumTipoMovimientoInventario.Ingreso,
                        IdCentroServicioOrigen = guia.IdCentroLogistico,
                        Bodega = new PUCentroServiciosDC
                        {
                            IdCentroServicio = ControllerContext.Current.IdCentroServicio,
                        },
                        NumeroGuia = guia.NumeroGuia.Value,
                        FechaGrabacion = DateTime.Now,
                        FechaEstimadaIngreso = DateTime.Now,
                        CreadoPor = ControllerContext.Current.Usuario,
                    };

                    fachadaCes.AdicionarMovimientoInventario(movimiento);


                    //Se valida si el motivo causa supervision, se actualiza la admisiÃ³n
                    if (estadoGuia.IdEstadoGuia == (short)ADEnumEstadoGuia.Auditoria)
                        fachadaMensajeria.ActualizarSupervisionGuia(guia.IdAdmision);


                    if (guia.Novedad != null)
                    {
                        CONovedadGuiaDC novedad = new CONovedadGuiaDC { NumeroGuia = guia.NumeroGuia.Value, TipoNovedad = guia.Novedad };
                        LIRepositorioPruebasEntrega.Instancia.IngresarNovedad(novedad);
                    }

                    respuesta.Estado = (ADEnumEstadoGuia)estadoGuia.IdNuevoEstadoGuia;
                    respuesta.Resultado = OUEnumValidacionDescargue.Exitosa;
                    integraRaps = true;
                    transaccion.Complete();

                }

                if (integraRaps)
                {
                    //datos.Add("guia", guia);
                    fallaMapper = new RAFallaMapper();
                    RADatosFallaDC datosFalla = fallaMapper.MapperDatosFallaAutomaticaAgenciaGuiaLogistica(guia, RAEnumSistemaOrigen.CONTROLLER.GetHashCode());
                    datosFalla.IdMotivo = datosFalla.IdMotivoGuia;
                    parametrosSolicitudAcumulativa = AdministradorReglaEstadoRAPS.Instancia.IntegrarRapFallas(datosFalla, RAEnumOrigenRaps.Puntos);
                    CrearSolicitudAcumulativa(parametrosSolicitudAcumulativa, datosFalla);
                }

                return respuesta;
            }
            catch (Exception ex)
            {
                respuesta.Resultado = OUEnumValidacionDescargue.Error;
                respuesta.Mensaje = ex.Message;
                return respuesta;
            }

        }

        public LIDescargueGuiaDC DescargueEntregaAgencia(OUGuiaIngresadaDC guia)
        {
            LIDescargueGuiaDC respuesta = new LIDescargueGuiaDC();
            ADTrazaGuia estadoGuia;

            try
            {

                string observaciones = guia.Observaciones != null ? guia.Observaciones : string.Empty;
                guia.NuevoEstadoGuia = "ENT";
                guia.EstaDescargada = true;
                guia.EstadoGuiaPlanilla = "ENT";
                guia.TipoImpreso = ADEnumTipoImpreso.Planilla;

                //primera transicion de estado pasa a intento de entrega
                estadoGuia = new ADTrazaGuia
                {
                    Ciudad = guia.Ciudad,
                    IdCiudad = guia.IdCiudad,
                    IdAdmision = guia.IdAdmision,
                    IdEstadoGuia = (short)(EstadosGuia.ObtenerUltimoEstado(guia.IdAdmision)),
                    IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.Entregada,
                    Modulo = COConstantesModulos.PRUEBAS_DE_ENTREGA,
                    NumeroGuia = guia.NumeroGuia,
                    Observaciones = observaciones,
                    FechaGrabacion = DateTime.Now
                };

                using (TransactionScope transaccion = new TransactionScope())
                {
                    estadoGuia.IdTrazaGuia = EstadosGuia.ValidarInsertarEstadoGuia(estadoGuia);

                    if (estadoGuia.IdTrazaGuia == 0)
                    {
                        respuesta.Resultado = OUEnumValidacionDescargue.ErrorEstado;
                        return respuesta;
                    }

                    // Traza impreso asocia el cambio de estado con el documento que lo genero
                    ADTrazaGuiaImpresoDC trazaImpreso = new ADTrazaGuiaImpresoDC
                    {
                        IdTrazaGuia = estadoGuia.IdTrazaGuia,
                        NumeroImpreso = guia.Planilla,
                        TipoImpreso = guia.TipoImpreso,
                        Usuario = ControllerContext.Current.Usuario,
                        FechaGrabacion = DateTime.Now,
                    };

                    EstadosGuia.InsertarEstadoGuiaImpreso(trazaImpreso);

                    if (guia.EsAlCobro)
                    {
                        ADGuia adGuia = fachadaMensajeria.ObtenerGuiaXNumeroGuia(guia.NumeroGuia.Value);
                        LIConfiguradorPruebasEntrega.Instancia.AfectarCajaAgenciaPorEntrega(adGuia);
                    }

                    respuesta.Resultado = OUEnumValidacionDescargue.Exitosa;
                    transaccion.Complete();
                    return respuesta;
                }
            }
            catch (Exception ex)
            {
                respuesta.Resultado = OUEnumValidacionDescargue.Error;
                respuesta.Mensaje = ex.Message;
                return respuesta;
            }
        }

        /// <summary>
        /// Guarda las devoluciones de un auditor
        /// </summary>
        /// <param name="guia"></param>
        /// <returns></returns>
        /// <summary>
        /// Guarda las devoluciones de un auditor
        /// </summary>
        /// <param name="guia"></param>
        /// <returns></returns>
        public LIDescargueGuiaDC GuardarDevolucionAuditoria(OUGuiaIngresadaDC guia)
        {

            LIDescargueGuiaDC respuesta = new LIDescargueGuiaDC();
            ADTrazaGuia estadoGuia;
            try
            {


                ADTrazaGuia estadoActual = EstadosGuia.ObtenerTrazaUltimoEstadoXNumGuia(guia.NumeroGuia.Value);

                if (estadoActual.IdEstadoGuia != (short)ADEnumEstadoGuia.Auditoria && estadoActual.IdEstadoGuia != (short)ADEnumEstadoGuia.IntentoEntrega)
                {
                    respuesta.Resultado = OUEnumValidacionDescargue.ErrorEstado;
                    respuesta.Mensaje = "La guia no se encuentra en auditoria";
                    return respuesta;
                }

                string observaciones = "Descargue por auditoria";

                guia.NuevoEstadoGuia = "DEV";
                guia.EstaDescargada = true;
                guia.EstadoGuiaPlanilla = "DEV";
                guia.TipoImpreso = ADEnumTipoImpreso.Planilla;


                //primera transicion de estado pasa a intento de entrega
                estadoGuia = new ADTrazaGuia
                {
                    Ciudad = guia.Ciudad,
                    IdCiudad = guia.IdCiudad,
                    IdAdmision = guia.IdAdmision,
                    IdEstadoGuia = (short)estadoActual.IdEstadoGuia,
                    IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.IntentoEntrega,
                    Modulo = COConstantesModulos.PRUEBAS_DE_ENTREGA,
                    NumeroGuia = guia.NumeroGuia,
                    Observaciones = observaciones,
                    FechaGrabacion = guia.FechaMotivoDevolucion
                };



                using (TransactionScope transaccion = new TransactionScope())
                {
                    ADEstadoGuiaMotivoDC estadoMotivoGuia = new ADEstadoGuiaMotivoDC
                    {
                        Motivo = guia.Motivo,
                        Observaciones = string.Empty,
                        FechaMotivo = guia.FechaMotivoDevolucion
                    };


                    if (estadoActual.IdEstadoGuia != (short)ADEnumEstadoGuia.IntentoEntrega)
                    {

                        estadoGuia.IdTrazaGuia = EstadosGuia.ValidarInsertarEstadoGuia(estadoGuia);

                        if (estadoGuia.IdTrazaGuia == 0)
                        {
                            respuesta.Resultado = OUEnumValidacionDescargue.ErrorEstado;
                            return respuesta;
                        }

                        estadoMotivoGuia.IdTrazaGuia = estadoGuia.IdTrazaGuia;
                        // Traza impreso asocia el cambio de estado con el documento que lo genero
                        ADTrazaGuiaImpresoDC trazaImpreso = new ADTrazaGuiaImpresoDC
                        {
                            IdTrazaGuia = estadoGuia.IdTrazaGuia,
                            NumeroImpreso = guia.Planilla,
                            TipoImpreso = guia.TipoImpreso,
                            Usuario = ControllerContext.Current.Usuario,
                            FechaGrabacion = DateTime.Now,
                        };

                        EstadosGuia.InsertarEstadoGuiaImpreso(trazaImpreso);

                        //inserta el estado motivo de la devoluciÃ³n
                        EstadosGuia.InsertaEstadoGuiaMotivo(estadoMotivoGuia);
                        /// Se guarda la evidencia de intento de entrega
                        if (guia.Motivo.EsEscaneo && guia.EvidenciaDevolucion.NumeroEvidencia != 0)
                        {
                            guia.EvidenciaDevolucion.TipoEvidencia = EGMotivosGuia.ObtenerMotivosEvidencia(guia.Motivo, guia.CantidadReintentosEntrega);

                            if (LIRepositorioPruebasEntrega.Instancia.ValidarEvidenciaDevolucion(guia.EvidenciaDevolucion.NumeroEvidencia, guia.EvidenciaDevolucion.TipoEvidencia.IdTipo))
                            {
                                respuesta.Resultado = OUEnumValidacionDescargue.Error;
                                respuesta.Mensaje = "Numero de volante de devoluciÃ³n ya existe";
                                transaccion.Dispose();
                                return respuesta;
                            }
                            else
                            {
                                // obtener el tipo de evidencia de acuerdo al motivo y el intento de entrega
                                guia.EvidenciaDevolucion.IdEstadoGuialog = estadoGuia.IdTrazaGuia.Value;
                                guia.EvidenciaDevolucion.EstaDigitalizado = false;
                                guia.EvidenciaDevolucion.IdEvidenciaDevolucion = guia.EvidenciaDevolucion.NumeroEvidencia;
                                LIRepositorioPruebasEntrega.Instancia.AdicionarEvidenciaDevolucion(guia.EvidenciaDevolucion);
                            }
                        }
                    }
                    else
                    {
                        // los datos de descargue del app fueron cambiados
                        if (guia.DescargueSupervisado)
                        {
                            estadoMotivoGuia.IdTrazaGuia = guia.EvidenciaDevolucion.IdEstadoGuialog;
                            //editar motivo 
                            EstadosGuia.ActualizarEstadoGuiaMotivo(estadoMotivoGuia);

                            if (guia.Motivo.EsEscaneo && guia.EvidenciaDevolucion.NumeroEvidencia != 0)
                            {
                                if (guia.EvidenciaDevolucion.TipoEvidencia.IdTipo == 0)
                                {
                                    guia.EvidenciaDevolucion.TipoEvidencia = EGMotivosGuia.ObtenerMotivosEvidencia(guia.Motivo, guia.CantidadReintentosEntrega);
                                }
                                if (!LIRepositorioPruebasEntrega.Instancia.ValidarEvidenciaDevolucion(guia.EvidenciaDevolucion.NumeroEvidencia, guia.EvidenciaDevolucion.TipoEvidencia.IdTipo))
                                {
                                    // obtener el tipo de evidencia de acuerdo al motivo y el intento de entrega
                                    guia.EvidenciaDevolucion.IdEstadoGuialog = guia.EvidenciaDevolucion.IdEstadoGuialog;
                                    guia.EvidenciaDevolucion.IdEvidenciaDevolucion = guia.EvidenciaDevolucion.NumeroEvidencia;
                                    LIRepositorioPruebasEntrega.Instancia.ModificarEvidenciaDevolucion(guia.EvidenciaDevolucion);
                                }
                            }
                        }
                    }



                    if (guia.Motivo.IntentoEntrega)
                    {
                        //Actualiza intentos de entrega
                        fachadaMensajeria.ActualizarReintentosEntrega(guia.IdAdmision);
                    }

                    //Actualiza estado de guia en planilla de mensajero
                    fachadaOpUrbana.ActualizarGuiaMensajero(guia);


                    //segunda transicion de estado obtiene el estado a partir peso
                    estadoGuia = new ADTrazaGuia
                    {
                        Ciudad = guia.Ciudad,
                        IdCiudad = guia.IdCiudad,
                        IdAdmision = guia.IdAdmision,
                        Modulo = COConstantesModulos.PRUEBAS_DE_ENTREGA,
                        NumeroGuia = guia.NumeroGuia,
                        Observaciones = observaciones,
                        FechaGrabacion = DateTime.Now.AddMilliseconds(500)
                    };

                    if (guia.TipoCliente == ADEnumTipoCliente.INT.ToString() || (estadoGuia.IdNuevoEstadoGuia == (short)ADEnumEstadoGuia.DevolucionRatificada && guia.EsAlCobro && !guia.EstaPagada))
                    {
                        estadoGuia.IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.Telemercadeo;
                        respuesta.Estado = ADEnumEstadoGuia.Telemercadeo;
                    }
                    else if (guia.Motivo.IdMotivoGuia == 122)
                    {
                        estadoGuia.IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.Auditoria;
                        respuesta.Estado = ADEnumEstadoGuia.Auditoria;
                    }
                    else if (guia.Motivo.IdMotivoGuia == 138)
                    {
                        estadoGuia.IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.Incautado;
                        respuesta.Estado = ADEnumEstadoGuia.Incautado;
                    }
                    else if (guia.Motivo.IdMotivoGuia == 168)
                    {
                        estadoGuia.IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.Telemercadeo;
                        respuesta.Estado = ADEnumEstadoGuia.Telemercadeo;
                    }
                    else if (guia.Motivo.IdMotivoGuia == 139)
                    {
                        estadoGuia.IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.Indemnizacion;
                        respuesta.Estado = ADEnumEstadoGuia.Indemnizacion;
                    }
                    else if (guia.Motivo.IdMotivoGuia == 140 || guia.Motivo.IdMotivoGuia == 136)
                    {
                        estadoGuia.IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.Telemercadeo;
                        respuesta.Estado = ADEnumEstadoGuia.Telemercadeo;
                    }
                    else if (guia.Motivo.IdMotivoGuia == 142)
                    {
                        if (guia.EsAlCobro && !guia.EstaPagada)
                        {
                            estadoGuia.IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.Telemercadeo;
                            respuesta.Estado = ADEnumEstadoGuia.Telemercadeo;
                        }
                        else
                        {
                            estadoGuia.IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.DevolucionRatificada;
                            respuesta.Estado = ADEnumEstadoGuia.DevolucionRatificada;
                        }
                    }
                    else if (guia.Motivo.IdMotivoGuia == 121)
                    {
                        if (guia.TipoCliente == ADEnumTipoCliente.CPE.ToString() || guia.Servicio.IdServicio == TAConstantesServicios.SERVICIO_NOTIFICACIONES)
                        {
                            estadoGuia.IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.DevolucionRatificada;
                            respuesta.Estado = ADEnumEstadoGuia.DevolucionRatificada;

                        }
                        else
                        {
                            estadoGuia.Observaciones = "Envio para reclame en oficina";
                            estadoGuia.IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.Reenvio;
                            respuesta.Estado = ADEnumEstadoGuia.ReclameEnOficina;

                            PUCentroServicioApoyo puntoRO = fachadaCes.ObtenerPuntosREOSegunUbicacionDestino(Convert.ToInt32(guia.CiudadDestino.IdLocalidad)).FirstOrDefault();
                            if (puntoRO == null)
                                puntoRO = fachadaCes.ObtenerPuntosREOSegunUbicacionDestino(Convert.ToInt32(guia.CiudadDestino.IdLocalidad)).FirstOrDefault();

                            fachadaCentroAcopio.CambiarTipoEntregaTelemercadeo_REO(guia.NumeroGuia.Value, puntoRO.IdCentroservicio);

                            LITapasLogisticaInversa.Instancia.AdicionarTapaLogistica(new LITapaLogisticaDC
                            {
                                NumeroGuia = guia.NumeroGuia.Value,
                                Tipo = LIEnumTipoTapaLogisticaDC.ReclameOficina
                            });
                        }

                    }
                    else
                    {
                        if (guia.EsAlCobro || (guia.Peso > 2 && guia.TipoCliente == "PPE"))
                        {
                            estadoGuia.IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.Telemercadeo;
                            respuesta.Estado = ADEnumEstadoGuia.Telemercadeo;

                            if (guia.Motivo.TiempoAfectacion != 0)
                            {
                                EGTipoNovedadGuia.CambiarFechaEntrega(new Servicios.ContratoDatos.Comun.COCambioFechaEntregaDC
                                {
                                    Guia = new ADGuia { NumeroGuia = guia.NumeroGuia.Value },
                                    TiempoAfectacion = guia.Motivo.TiempoAfectacion
                                });
                            }
                        }
                        else
                        {
                            estadoGuia.IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.DevolucionRatificada;
                            respuesta.Estado = ADEnumEstadoGuia.DevolucionRatificada;
                        }
                    }



                    ///inserta transicion segundo estado
                    estadoGuia.IdTrazaGuia = EstadosGuia.InsertaEstadoGuia(estadoGuia);

                    if (estadoGuia.IdEstadoGuia == (short)ADEnumEstadoGuia.PendienteIngresoaCustodia)
                    {
                        ADGuia guiaAdmision = fachadaAdmisiones.ObtenerGuiaXNumeroGuia(guia.NumeroGuia.Value);
                        if (!string.IsNullOrEmpty(guiaAdmision.Destinatario.Telefono) && guiaAdmision.Destinatario.Telefono.Length == 10 && guia.TipoCliente == ADEnumTipoCliente.PPE.ToString())
                        {
                            AdministradorMensajesTexto.Instancia.EnviarMensajeTexto(LOIEnumMensajesTexto.Custodia, guiaAdmision.TelefonoDestinatario, guia.NumeroGuia.Value, "");
                        }

                    }

                    if (guia.Motivo.TiempoAfectacion != 0)
                    {
                        EGTipoNovedadGuia.CambiarFechaEntrega(new Servicios.ContratoDatos.Comun.COCambioFechaEntregaDC
                        {
                            Guia = new ADGuia { NumeroGuia = guia.NumeroGuia.Value },
                            TiempoAfectacion = guia.Motivo.TiempoAfectacion
                        });
                    }



                    // si el motivo es incautado envia mensaje de texto
                    if (guia.Motivo.IdMotivoGuia == 138)
                    {
                        ADGuia guiaAdmision = fachadaAdmisiones.ObtenerGuiaXNumeroGuia(guia.NumeroGuia.Value);
                        if (!string.IsNullOrEmpty(guiaAdmision.Destinatario.Telefono) && guiaAdmision.Destinatario.Telefono.Length == 10)
                        {
                            AdministradorMensajesTexto.Instancia.EnviarMensajeTexto(LOIEnumMensajesTexto.Incautado, guiaAdmision.TelefonoDestinatario, guia.NumeroGuia.Value, "");

                        }
                    }

                    // si el motivo es hurto envia mensaje de texto
                    else if (guia.Motivo.IdMotivoGuia == 139)
                    {
                        ADGuia guiaAdmision = fachadaAdmisiones.ObtenerGuiaXNumeroGuia(guia.NumeroGuia.Value);
                        if (!string.IsNullOrEmpty(guiaAdmision.Destinatario.Telefono) && guiaAdmision.Destinatario.Telefono.Length == 10)
                        {
                            AdministradorMensajesTexto.Instancia.EnviarMensajeTexto(LOIEnumMensajesTexto.Hurto, guiaAdmision.TelefonoDestinatario, guia.NumeroGuia.Value, "");

                        }
                    }

                    if (estadoGuia.IdEstadoGuia == (short)ADEnumEstadoGuia.PendienteIngresoaCustodia)
                    {
                        ADGuia guiaAdmision = fachadaAdmisiones.ObtenerGuiaXNumeroGuia(guia.NumeroGuia.Value);
                        if (!string.IsNullOrEmpty(guiaAdmision.Destinatario.Telefono) && guiaAdmision.Destinatario.Telefono.Length == 10 && guia.TipoCliente == ADEnumTipoCliente.PPE.ToString())
                        {
                            AdministradorMensajesTexto.Instancia.EnviarMensajeTexto(LOIEnumMensajesTexto.Custodia, guiaAdmision.TelefonoDestinatario, guia.NumeroGuia.Value, "");

                        }

                    }

                    if (guia.Novedad != null)
                    {
                        CONovedadGuiaDC novedad = new CONovedadGuiaDC
                        {
                            NumeroGuia = guia.NumeroGuia.Value,
                            TipoNovedad = guia.Novedad
                        };
                        LIRepositorioPruebasEntrega.Instancia.IngresarNovedad(novedad);
                    }

                    respuesta.Guia = guia;
                    respuesta.Resultado = OUEnumValidacionDescargue.Exitosa;
                    transaccion.Complete();
                    return respuesta;
                }
            }
            catch (Exception ex)
            {
                respuesta.Resultado = OUEnumValidacionDescargue.Error;
                respuesta.Mensaje = ex.Message;
                return respuesta;
            }
        }

        public void CrearSolicitudAcumulativa(RAParametrosSolicitudAcumulativaDC parametrosSolicitudAcumulativa, RADatosFallaDC datosFalla)
        {
            /*****************************************CREA SOLICITUD ACUMULATIVA********************************************************/
            if (parametrosSolicitudAcumulativa != null)
            {
                if (!parametrosSolicitudAcumulativa.EstaEnviado)
                {
                    if (parametrosSolicitudAcumulativa.TipoNovedad != CoEnumTipoNovedadRaps.Pordefecto && parametrosSolicitudAcumulativa.Parametrosparametrizacion.Count > 0)
                    {
                        RAIntegracionesRaps.Instancia.CrearSolicitudAcumulativaRaps((CoEnumTipoNovedadRaps)parametrosSolicitudAcumulativa.TipoNovedad.GetHashCode(), parametrosSolicitudAcumulativa.Parametrosparametrizacion, datosFalla.IdCiudad.Substring(0, 5), ControllerContext.Current == null ? "MotorRaps" : ControllerContext.Current.Usuario, datosFalla.IdSistema);
                    }
                }
                else
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_RAPS, Raps.Comun.RAEnumTipoErrorClientes.EX_INSERTAR_SOLICITUD.ToString(), "La falla ya fue registrada para el responsable solicitado"));
                }
            }


        }
        #endregion Metodos

        #region Fallas Interlogis / web

        /// <summary>
        /// Metodo para obtener parametros por integracion 
        /// </summary>
        /// <param name="tipoMotivo"></param>
        /// <returns></returns>
        public List<LIParametrizacionIntegracionRAPSDC> ObtenerParametrosPorIntegracion(string tipoParametro)
        {
            return ParametrizacionRaps.Instancia.ObtenerParametrosPorIntegracion(tipoParametro);
        }

        #endregion
    }
}