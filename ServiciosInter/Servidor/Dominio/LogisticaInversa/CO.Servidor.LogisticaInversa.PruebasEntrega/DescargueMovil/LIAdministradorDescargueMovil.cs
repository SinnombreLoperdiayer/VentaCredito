using CO.Controller.Servidor.Integraciones;
using CO.Servidor.Dominio.Comun.AdmEstadosGuia;
using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Dominio.Comun.LogisticaInversa;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.OperacionUrbana;
using CO.Servidor.LogisticaInversa.Comun;
using CO.Servidor.LogisticaInversa.Datos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Servicios;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Transactions;

namespace CO.Servidor.LogisticaInversa.PruebasEntrega.DescargueMovil
{
    public class LIAdministradorDescargueMovil : ControllerBase
    {
        /// <summary>
        /// Singleton 
        /// </summary>
        private static readonly LIAdministradorDescargueMovil instancia = (LIAdministradorDescargueMovil)FabricaInterceptores.GetProxy(new LIAdministradorDescargueMovil(), COConstantesModulos.LOGISTICA_INVERSA);

        public static LIAdministradorDescargueMovil Instancia
        {
            get { return LIAdministradorDescargueMovil.instancia; }
        }

        private IADFachadaAdmisionesMensajeria fachadaMensajeria;
        private ILIFachadaLogisticaInversa fachadaLogisticaInversa;



        private LIAdministradorDescargueMovil()
        {
            fachadaMensajeria = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>();
            fachadaLogisticaInversa = COFabricaDominio.Instancia.CrearInstancia<ILIFachadaLogisticaInversa>();
        }

        /// <summary>
        /// Descargue guias mensajero controller App
        /// </summary>
        /// <param name="descargue"></param>
        /// <returns></returns>
        public LIEstadoDescargueControllerAppDC DescargueMensajeroControllerApp(LIDescargueControllerAppDC descargue)
        {

            LIEstadoDescargueControllerAppDC respuesta = new LIEstadoDescargueControllerAppDC();
            respuesta.Resultado = OUEnumValidacionDescargue.ErrorEstado;
            respuesta.Mensaje = "La guía no se encuentra en reparto";

            /*************************** Estado guia ********************************/
            ADTrazaGuia ultimoEstadoGuia = EstadosGuia.ObtenerTrazaUltimoEstadoXNumGuia(descargue.NumeroGuia);
            /***************************** Validacion en reparto ********************************************/
            if (ultimoEstadoGuia.IdEstadoGuia == (short)ADEnumEstadoGuia.EnReparto || ultimoEstadoGuia.IdEstadoGuia == (short)ADEnumEstadoGuia.IntentoEntrega)
            {
                short ultimoEst = ultimoEstadoGuia.IdEstadoGuia.Value;

                /********************************* Informacion de guia para recibido notificacion******************************************/
                ultimoEstadoGuia.IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.Entregada;
                ultimoEstadoGuia.Observaciones = "Entregado desde APP";
                ultimoEstadoGuia.Modulo = COConstantesModulos.PRUEBAS_DE_ENTREGA;
                ultimoEstadoGuia.Latitud = descargue.Latitud;
                ultimoEstadoGuia.Longitud = descargue.Longitud;
                ultimoEstadoGuia.FechaGrabacion = descargue.FechaGrabacion;


                using (TransactionScope transaction = new TransactionScope())
                {
                    /********************* Se valida si la guia está planilla y corresponde al mensajero ************************/
                    if (LIRepositorioPruebasEntrega.Instancia.ValidarPlanillaDescargueMensajero(descargue.NumeroGuia, descargue.IdMensajero))
                    {
                        /******************** INSERTAR ESTADO ENTREGA ENTREGADO ******************************/
                        long idEstagoGuiaLog = EstadosGuia.InsertaEstadoGuiaFecha(ultimoEstadoGuia);

                        /***************************** ACTUALIZA ESTADO GUIA ****************************/
                        fachadaMensajeria.ActualizarEntregadoGuia(descargue.NumeroGuia);

                        // Traza impreso asocia el cambio de estado con el documento que lo genero
                        ADTrazaGuiaImpresoDC trazaImpreso = new ADTrazaGuiaImpresoDC
                        {
                            IdTrazaGuia = idEstagoGuiaLog,
                            NumeroImpreso = descargue.IdPlanilla,
                            TipoImpreso = ADEnumTipoImpreso.Planilla,
                            Usuario = ControllerContext.Current.Usuario,
                            FechaGrabacion = DateTime.Now,
                        };

                        EstadosGuia.InsertarEstadoGuiaImpreso(trazaImpreso);


                        if (LIRepositorioPruebasEntrega.Instancia.ValidarGuiaMensajeTextoEntrega(descargue.NumeroGuia))
                        {
                            /***************************** ENVIA MENSAJE TEXTO  ****************************/
                            ADMensajeriaTipoCliente remitenteDestinatario = fachadaMensajeria.ObtenerRemitenteDestinatarioGuia(descargue.NumeroGuia);

                            if (!string.IsNullOrEmpty(remitenteDestinatario.PeatonRemitente.Telefono) && remitenteDestinatario.PeatonRemitente.Telefono.Length == 10)
                            {
                                AdministradorMensajesTexto.Instancia.EnviarMensajeTexto(LOIEnumMensajesTexto.EntregaMovil, remitenteDestinatario.PeatonRemitente.Telefono, descargue.NumeroGuia, "");
                            }

                        }



                        /******************* INSERCION IMAGENES PRUEBA ENTREGA Y FACHADA PARA NOTIFICACIONES *****************************/
                        descargue.IdCiudad = ultimoEstadoGuia.IdCiudad;
                        descargue.NombreCiudad = ultimoEstadoGuia.Ciudad;
                        LIRepositorioPruebasEntrega.Instancia.InsertarImagenesPruebaEntrega(descargue);
                        LIRepositorioPruebasEntrega.Instancia.InsertarConteoDescarguesYDevolucionesPorApp(descargue.IdPlanilla, 1, 0);
                        if(ultimoEst == (short)ADEnumEstadoGuia.IntentoEntrega)
                        {
                            LIRepositorioPruebasEntrega.Instancia.InsertarConteoDescarguesYDevolucionesPorApp(descargue.IdPlanilla,0, -1);
                        }
                        respuesta.Resultado = OUEnumValidacionDescargue.Exitosa;
                        respuesta.Mensaje = "Descargue satisfactorio";

                        LIGestionAuditorDC gestionAuditor = new LIGestionAuditorDC()
                        {
                            NumeroGuia = descargue.NumeroGuia,
                            IdEstadoGuiaLog = idEstagoGuiaLog,
                            IdPlanillaAsignacion = descargue.IdPlanilla,
                            DescripcionInmueble = "Pendiente",
                            NombreReceptorAuditoria = "Pendiente",
                            CedulaReceptorAuditoria = "Pendiente"
                        };

                        LIRepositorioPruebasEntrega.Instancia.InsertarGestionAuditor(gestionAuditor);

                        transaction.Complete();
                        return respuesta;
                    }
                    else
                    {
                        respuesta.Resultado = OUEnumValidacionDescargue.SinAsignar;
                        respuesta.Mensaje = "La guía no está planillada al mensajero";
                        transaction.Complete();
                        return respuesta;
                    }
                }
            }


            return respuesta;
        }

        /// <summary>
        /// Metodo para descargue de entregas auditor
        /// </summary>
        /// <param name="descargue"></param>
        /// <returns></returns>
        public LIEstadoDescargueControllerAppDC DescargueAuditorControllerApp(LIDescargueControllerAppDC descargue)
        {
            LIEstadoDescargueControllerAppDC respuesta = new LIEstadoDescargueControllerAppDC();
            respuesta.Resultado = OUEnumValidacionDescargue.ErrorEstado;
            respuesta.Mensaje = "La guía no se encuentra en auditoria";
            /*************************** Estado guia ********************************/
            ADTrazaGuia ultimoEstadoGuia = EstadosGuia.ObtenerTrazaUltimoEstadoXNumGuia(descargue.NumeroGuia);
            /***************************** Validacion en reparto ********************************************/
            if (ultimoEstadoGuia.IdEstadoGuia == (short)ADEnumEstadoGuia.Auditoria || ultimoEstadoGuia.IdEstadoGuia == (short)ADEnumEstadoGuia.IntentoEntrega)
            {
                short ultimoEst = ultimoEstadoGuia.IdEstadoGuia.Value;
                /********************************* Informacion de guia para recibido notificacion******************************************/
                ultimoEstadoGuia.IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.Entregada;
                ultimoEstadoGuia.Observaciones = "Entregado desde APP";
                ultimoEstadoGuia.Modulo = COConstantesModulos.PRUEBAS_DE_ENTREGA;
                ultimoEstadoGuia.Latitud = descargue.Latitud;
                ultimoEstadoGuia.Longitud = descargue.Longitud;
                ultimoEstadoGuia.FechaGrabacion = descargue.FechaGrabacion;

                using (TransactionScope transaction = new TransactionScope())
                {
                    /********************* Se valida si la guia está planilla y corresponde al mensajero ************************/
                    if (LIRepositorioPruebasEntrega.Instancia.ValidarPlanillaDescargueMensajero(descargue.NumeroGuia, descargue.IdMensajero))
                    {
                        /******************** INSERTAR ESTADO ENTREGA ENTREGADO ******************************/
                        long idEstagoGuiaLog = EstadosGuia.InsertaEstadoGuiaFecha(ultimoEstadoGuia);

                        // Traza impreso asocia el cambio de estado con el documento que lo genero
                        ADTrazaGuiaImpresoDC trazaImpreso = new ADTrazaGuiaImpresoDC
                        {
                            IdTrazaGuia = idEstagoGuiaLog,
                            NumeroImpreso = descargue.IdPlanilla,
                            TipoImpreso = ADEnumTipoImpreso.Planilla,
                            Usuario = ControllerContext.Current.Usuario,
                            FechaGrabacion = DateTime.Now,
                        };

                        EstadosGuia.InsertarEstadoGuiaImpreso(trazaImpreso);

                        /*********************************** Registrar recibido guia para servicio notificaciones *********************************************/
                        if (descargue.IdServicio == (short)TAEnumServiciosDC.Notificaciones && descargue.RecibidoGuia != null)
                        {
                            descargue.RecibidoGuia.FechaEntrega = descargue.FechaEntrega;
                            descargue.RecibidoGuia.IdGuia = ultimoEstadoGuia.IdAdmision.Value;
                            descargue.RecibidoGuia.Otros = "Descargue desde APP";
                            fachadaLogisticaInversa.RegistrarRecibidoGuiaManual(descargue.RecibidoGuia);
                        }

                        /******************* INSERCION IMAGENES PRUEBA ENTREGA *****************************/
                        descargue.IdCiudad = ultimoEstadoGuia.IdCiudad;
                        descargue.NombreCiudad = ultimoEstadoGuia.Ciudad;
                        LIRepositorioPruebasEntrega.Instancia.InsertarImagenesPruebaEntrega(descargue);
                        LIRepositorioPruebasEntrega.Instancia.InsertarConteoDescarguesYDevolucionesPorApp(descargue.IdPlanilla, 1, 0);
                        if (ultimoEst == (short)ADEnumEstadoGuia.IntentoEntrega)
                        {
                            LIRepositorioPruebasEntrega.Instancia.InsertarConteoDescarguesYDevolucionesPorApp(descargue.IdPlanilla, 0, -1);
                        }
                        respuesta.Resultado = OUEnumValidacionDescargue.Exitosa;
                        respuesta.Mensaje = "Descargue exitoso";
                    }
                    else
                    {
                        respuesta.Resultado = OUEnumValidacionDescargue.SinAsignar;
                        respuesta.Mensaje = "La guía no está asignada al auditor";
                    }

                    transaction.Complete();
                    return respuesta;
                }
            }
            return respuesta;
        }

        /// <summary>
        /// Metodo para las devoluciones mensajero controller app
        /// </summary>
        /// <param name="devolucion"></param>
        /// <returns></returns>
        public LIEstadoDescargueControllerAppDC DevolucionMensajeroControllerApp(LIDescargueControllerAppDC devolucion)
        {
            LIEstadoDescargueControllerAppDC respuesta = new LIEstadoDescargueControllerAppDC();
            respuesta.Resultado = OUEnumValidacionDescargue.ErrorEstado;
            respuesta.Mensaje = "La guía no se encuentra en reparto";

            ADTrazaGuia ultimoEstadoGuia = EstadosGuia.ObtenerTrazaUltimoEstadoXNumGuia(devolucion.NumeroGuia);
            /***************************** Validacion en reparto ********************************************/
            if (ultimoEstadoGuia.IdEstadoGuia == (short)ADEnumEstadoGuia.EnReparto)
            {
                /********************************* Informacion de guia para recibido notificacion******************************************/
                ultimoEstadoGuia.IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.IntentoEntrega;
                ultimoEstadoGuia.Observaciones = "Descargado desde APP";
                ultimoEstadoGuia.Modulo = COConstantesModulos.PRUEBAS_DE_ENTREGA;
                ultimoEstadoGuia.Latitud = devolucion.Latitud;
                ultimoEstadoGuia.Longitud = devolucion.Longitud;
                ultimoEstadoGuia.FechaGrabacion = devolucion.FechaGrabacion;

                /*********WALTER************** SE NECESITA PARA TRAER NUMERO INTENTOS DE ENTREGA ***********************************************/
                ADGuia guia = fachadaMensajeria.ObtenerGuiaXNumeroGuia(devolucion.NumeroGuia);
                using (TransactionScope transaccion = new TransactionScope())
                {
                    /********************* Se valida si la guia está planilla y corresponde al mensajero ************************/
                    if (LIRepositorioPruebasEntrega.Instancia.ValidarPlanillaDescargueMensajero(devolucion.NumeroGuia, devolucion.IdMensajero))
                    {
                        /******************************** ESTADO INTENTO DE ENTREGA  *******************************************/
                        ultimoEstadoGuia.IdTrazaGuia = EstadosGuia.ValidarInsertarEstadoGuiaFecha(ultimoEstadoGuia);

                        if (ultimoEstadoGuia.IdTrazaGuia == 0)
                        {
                            respuesta.Resultado = OUEnumValidacionDescargue.ErrorEstado;
                            respuesta.Mensaje = "No se pude cambiar el estado";
                            transaccion.Dispose();
                            return respuesta;
                        }

                        if (LIRepositorioPruebasEntrega.Instancia.ValidarGuiaMensajeTextoEntrega(devolucion.NumeroGuia))
                        {
                            /***************************** ENVIA MENSAJE TEXTO  ****************************/
                            ADMensajeriaTipoCliente remitenteDestinatario = fachadaMensajeria.ObtenerRemitenteDestinatarioGuia(devolucion.NumeroGuia);

                            if (!string.IsNullOrEmpty(remitenteDestinatario.PeatonRemitente.Telefono))
                            {
                                AdministradorMensajesTexto.Instancia.EnviarMensajeTexto(LOIEnumMensajesTexto.DevolucionMovil, remitenteDestinatario.PeatonRemitente.Telefono, devolucion.NumeroGuia, devolucion.MotivoGuia.Descripcion);
                            }

                            if (!string.IsNullOrEmpty(remitenteDestinatario.PeatonDestinatario.Telefono))
                            {
                                AdministradorMensajesTexto.Instancia.EnviarMensajeTexto(LOIEnumMensajesTexto.DevolucionMovil, remitenteDestinatario.PeatonDestinatario.Telefono, devolucion.NumeroGuia, devolucion.MotivoGuia.Descripcion);
                            }

                        }


                        ADEstadoGuiaMotivoDC estadoMotivoGuia = new ADEstadoGuiaMotivoDC
                        {
                            IdTrazaGuia = ultimoEstadoGuia.IdTrazaGuia,
                            Motivo = devolucion.MotivoGuia,
                            Observaciones = devolucion.Observaciones,
                            FechaMotivo = devolucion.FechaGrabacion,
                        };

                        //INSERTA EL ESTADO MOTIVO DEVOLUCION
                        EstadosGuia.InsertaEstadoGuiaMotivoFecha(estadoMotivoGuia);

                        // Traza impreso asocia el cambio de estado con el documento que lo genero
                        ADTrazaGuiaImpresoDC trazaImpreso = new ADTrazaGuiaImpresoDC
                        {
                            IdTrazaGuia = ultimoEstadoGuia.IdTrazaGuia,
                            NumeroImpreso = devolucion.IdPlanilla,
                            TipoImpreso = ADEnumTipoImpreso.Planilla,
                            Usuario = ControllerContext.Current.Usuario,
                            FechaGrabacion = DateTime.Now,
                        };

                        EstadosGuia.InsertarEstadoGuiaImpreso(trazaImpreso);

                        /**************** Aumentar fecha por tiempo de novedad ***********************************************/
                        if (devolucion.MotivoGuia.TiempoAfectacion != 0)
                        {
                            EGTipoNovedadGuia.CambiarFechaEntrega(new Servicios.ContratoDatos.Comun.COCambioFechaEntregaDC
                            {
                                Guia = new ADGuia { NumeroGuia = guia.NumeroGuia },
                                TiempoAfectacion = devolucion.MotivoGuia.TiempoAfectacion
                            });
                        }

                        /***************************************INSERCION DE LAS IMAGENES EVIDENCIA ************************************************/
                        devolucion.IdCiudad = ultimoEstadoGuia.IdCiudad;
                        devolucion.NombreCiudad = ultimoEstadoGuia.Ciudad;
                        LIRepositorioPruebasEntrega.Instancia.InsertarImagenesPruebaEntrega(devolucion);
                        LIRepositorioPruebasEntrega.Instancia.InsertarConteoDescarguesYDevolucionesPorApp(devolucion.IdPlanilla, 0, 1);
                        /****************** Si motivo guia EsEscaneo es TRUE***************************/
                        if (devolucion.MotivoGuia.EsEscaneo)
                        {
                            guia.CantidadIntentosEntrega++;
                            /************************** Se calcula el TipoEvidencia segun el IdMotivo *****************************/
                            var tipoEvidencia = EGMotivosGuia.ObtenerMotivosEvidencia(devolucion.MotivoGuia, (short)guia.CantidadIntentosEntrega);
                            /// Se valida si el numero de volante de devolucion ya está creado
                            if (LIRepositorioPruebasEntrega.Instancia.ValidarEvidenciaDevolucion(devolucion.NumeroIntentoFallidoEntrega, tipoEvidencia.IdTipo))
                            {
                                respuesta.Resultado = OUEnumValidacionDescargue.Error;
                                respuesta.Mensaje = "Numero de volante de devolución ya existe";
                                transaccion.Dispose();
                            }
                            else
                            {
                                var evidenciaDevolucion = new LIEvidenciaDevolucionDC();
                                evidenciaDevolucion.IdEstadoGuialog = ultimoEstadoGuia.IdTrazaGuia.Value;
                                evidenciaDevolucion.EstaDigitalizado = false;
                                evidenciaDevolucion.NumeroEvidencia = devolucion.NumeroIntentoFallidoEntrega;
                                evidenciaDevolucion.TipoEvidencia = tipoEvidencia;
                                LIRepositorioPruebasEntrega.Instancia.AdicionarEvidenciaDevolucion(evidenciaDevolucion);
                                respuesta.Resultado = OUEnumValidacionDescargue.Exitosa;
                                respuesta.Mensaje = "Descargue satisfactorio";
                                transaccion.Complete();
                            }
                        }
                        else
                        {
                            /******************* Existoso sin volante devolucion ********************************/
                            respuesta.Resultado = OUEnumValidacionDescargue.Exitosa;
                            respuesta.Mensaje = "Descargue satisfactorio";
                            transaccion.Complete();
                        }


                    }
                    else
                    {
                        respuesta.Resultado = OUEnumValidacionDescargue.SinAsignar;
                        respuesta.Mensaje = "La guía no está asignada al mensajero";
                        transaccion.Complete();
                    }
                }
            }
            return respuesta;
        }

        /// <summary>
        /// Metodo para devolucion auditor controller app
        /// </summary>
        /// <param name="devolucion"></param>
        /// <returns></returns>
        public LIEstadoDescargueControllerAppDC DevolucionRatificadaAuditorControllerApp(LIDescargueControllerAppDC devolucion)
        {
            LIEstadoDescargueControllerAppDC respuesta = new LIEstadoDescargueControllerAppDC();
            respuesta.Resultado = OUEnumValidacionDescargue.ErrorEstado;
            respuesta.Mensaje = "La guía no está en auditoria";

            ADTrazaGuia ultimoEstadoGuia = EstadosGuia.ObtenerTrazaUltimoEstadoXNumGuia(devolucion.NumeroGuia);
            /***************************** Validacion en reparto ********************************************/
            if (ultimoEstadoGuia.IdEstadoGuia == (short)ADEnumEstadoGuia.Auditoria)
            {
                /********************************* Informacion de guia para recibido notificacion******************************************/
                ultimoEstadoGuia.IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.IntentoEntrega;
                ultimoEstadoGuia.Observaciones = "Descargado desde APP";
                ultimoEstadoGuia.Modulo = COConstantesModulos.PRUEBAS_DE_ENTREGA;
                ultimoEstadoGuia.Latitud = devolucion.Latitud;
                ultimoEstadoGuia.Longitud = devolucion.Longitud;
                ultimoEstadoGuia.FechaGrabacion = devolucion.FechaGrabacion;
                /*********WALTER************** SE NECESITA PARA TRAER NUMERO INTENTOS DE ENTREGA ***********************************************/
                ADGuia guia = fachadaMensajeria.ObtenerGuiaXNumeroGuia(devolucion.NumeroGuia);
                using (TransactionScope transaccion = new TransactionScope())
                {
                    /********************* Se valida si la guia está planilla y corresponde al mensajero ************************/
                    if (LIRepositorioPruebasEntrega.Instancia.ValidarPlanillaDescargueMensajero(devolucion.NumeroGuia, devolucion.IdMensajero))
                    {
                        /******************************** ESTADO INTENTO DE ENTREGA  *******************************************/
                        ultimoEstadoGuia.IdTrazaGuia = EstadosGuia.ValidarInsertarEstadoGuiaFecha(ultimoEstadoGuia);

                        if (ultimoEstadoGuia.IdTrazaGuia == 0)
                        {
                            respuesta.Resultado = OUEnumValidacionDescargue.ErrorEstado;
                            respuesta.Mensaje = "No se pude cambiar el estado";
                            transaccion.Dispose();
                            return respuesta;
                        }


                        ADEstadoGuiaMotivoDC estadoMotivoGuia = new ADEstadoGuiaMotivoDC
                        {
                            IdTrazaGuia = ultimoEstadoGuia.IdTrazaGuia,
                            Motivo = devolucion.MotivoGuia,
                            Observaciones = devolucion.Observaciones,
                            FechaMotivo = devolucion.FechaGrabacion,
                            TipoPredio = devolucion.TipoPredio,
                            DescripcionPredio = devolucion.DescripcionPredio,
                            TipoContador = devolucion.TipoContador,
                            NumeroContador = devolucion.NumeroContador
                        };

                        //INSERTA EL ESTADO MOTIVO DEVOLUCION
                        EstadosGuia.InsertaEstadoGuiaMotivoFecha(estadoMotivoGuia);

                        // Traza impreso asocia el cambio de estado con el documento que lo genero
                        ADTrazaGuiaImpresoDC trazaImpreso = new ADTrazaGuiaImpresoDC
                        {
                            IdTrazaGuia = ultimoEstadoGuia.IdTrazaGuia,
                            NumeroImpreso = devolucion.IdPlanilla,
                            TipoImpreso = ADEnumTipoImpreso.Planilla,
                            Usuario = ControllerContext.Current.Usuario,
                            FechaGrabacion = DateTime.Now,
                        };

                        EstadosGuia.InsertarEstadoGuiaImpreso(trazaImpreso);

                        /**************** Aumentar fecha por tiempo de novedad ***********************************************/
                        if (devolucion.MotivoGuia.TiempoAfectacion != 0)
                        {
                            EGTipoNovedadGuia.CambiarFechaEntrega(new Servicios.ContratoDatos.Comun.COCambioFechaEntregaDC
                            {
                                Guia = new ADGuia { NumeroGuia = guia.NumeroGuia },
                                TiempoAfectacion = devolucion.MotivoGuia.TiempoAfectacion
                            });
                        }

                        /********************************** INSERCION DE IMAGENES EVIDENCIA ******************************************************/
                        devolucion.IdCiudad = ultimoEstadoGuia.IdCiudad;
                        devolucion.NombreCiudad = ultimoEstadoGuia.Ciudad;
                        LIRepositorioPruebasEntrega.Instancia.InsertarImagenesPruebaEntrega(devolucion);
                        LIRepositorioPruebasEntrega.Instancia.InsertarConteoDescarguesYDevolucionesPorApp(devolucion.IdPlanilla, 0, 1);
                        /****************** Si motivo guia EsEscaneo es TRUE***************************/
                        if (devolucion.MotivoGuia.EsEscaneo && devolucion.NumeroIntentoFallidoEntrega != 0)
                        {
                            guia.CantidadIntentosEntrega++;
                            /************************** Se calcula el TipoEvidencia segun el IdMotivo *****************************/
                            var tipoEvidencia = EGMotivosGuia.ObtenerMotivosEvidencia(devolucion.MotivoGuia, (short)guia.CantidadIntentosEntrega);
                            /// Se valida si el numero de volante de devolucion ya está creado
                            if (LIRepositorioPruebasEntrega.Instancia.ValidarEvidenciaDevolucion(devolucion.NumeroIntentoFallidoEntrega, tipoEvidencia.IdTipo))
                            {
                                respuesta.Resultado = OUEnumValidacionDescargue.Error;
                                respuesta.Mensaje = "Numero de volante de devolución ya existe";
                                transaccion.Dispose();
                            }
                            else
                            {
                                var evidenciaDevolucion = new LIEvidenciaDevolucionDC();
                                evidenciaDevolucion.IdEstadoGuialog = ultimoEstadoGuia.IdTrazaGuia.Value;
                                evidenciaDevolucion.EstaDigitalizado = false;
                                evidenciaDevolucion.NumeroEvidencia = devolucion.NumeroIntentoFallidoEntrega;
                                evidenciaDevolucion.TipoEvidencia = tipoEvidencia;

                                LIRepositorioPruebasEntrega.Instancia.AdicionarEvidenciaDevolucion(evidenciaDevolucion);
                                respuesta.Resultado = OUEnumValidacionDescargue.Exitosa;
                                respuesta.Mensaje = "Descargue satisfactorio";
                                transaccion.Complete();
                            }
                        }
                        else
                        {
                            /******************* Existoso sin volante devolucion ********************************/
                            respuesta.Resultado = OUEnumValidacionDescargue.Exitosa;
                            respuesta.Mensaje = "Descargue satisfactorio";
                            transaccion.Complete();
                        }
                    }
                    else
                    {
                        respuesta.Resultado = OUEnumValidacionDescargue.SinAsignar;
                        respuesta.Mensaje = "La guía no está asignada al auditor";
                        transaccion.Complete();
                    }
                }
            }
            return respuesta;
        }

        /// <summary>
        /// Metodo para descargue entrega maestra auditor controller app 
        /// </summary>
        /// <param name="descargue"></param>
        /// <returns></returns>
        public LIEstadoDescargueControllerAppDC DescargueEMAuditorControllerApp(LIDescargueControllerAppDC descargue)
        {
            LIEstadoDescargueControllerAppDC respuesta = new LIEstadoDescargueControllerAppDC();
            respuesta.Resultado = OUEnumValidacionDescargue.ErrorEstado;
            respuesta.Mensaje = "La guía no se encuentra en auditoria";

            /*************************** Estado guia ********************************/
            ADTrazaGuia ultimoEstadoGuia = EstadosGuia.ObtenerTrazaUltimoEstadoXNumGuia(descargue.NumeroGuia);
            /***************************** Validacion en reparto ********************************************/
            if (ultimoEstadoGuia.IdEstadoGuia == (short)ADEnumEstadoGuia.Auditoria || ultimoEstadoGuia.IdEstadoGuia == (short)ADEnumEstadoGuia.IntentoEntrega)
            {
                short ultimoEst = ultimoEstadoGuia.IdEstadoGuia.Value;

                /********************************* Informacion de guia para recibido notificacion******************************************/
                ultimoEstadoGuia.IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.Entregada;
                ultimoEstadoGuia.Observaciones = "Entregado desde APP";
                ultimoEstadoGuia.Modulo = COConstantesModulos.PRUEBAS_DE_ENTREGA;
                ultimoEstadoGuia.Latitud = descargue.Latitud;
                ultimoEstadoGuia.Longitud = descargue.Longitud;
                ultimoEstadoGuia.FechaGrabacion = descargue.FechaGrabacion;

                using (TransactionScope transaction = new TransactionScope())
                {
                    /********************* Se valida si la guia está planilla y corresponde al mensajero ************************/
                    if (LIRepositorioPruebasEntrega.Instancia.ValidarPlanillaDescargueMensajero(descargue.NumeroGuia, descargue.IdMensajero))
                    {
                        /******************** INSERTAR ESTADO ENTREGA ENTREGADO ******************************/
                        long idEstagoGuiaLog = EstadosGuia.InsertaEstadoGuiaFecha(ultimoEstadoGuia);
                        
                        // Traza impreso asocia el cambio de estado con el documento que lo genero
                        ADTrazaGuiaImpresoDC trazaImpreso = new ADTrazaGuiaImpresoDC
                        {
                            IdTrazaGuia = idEstagoGuiaLog,
                            NumeroImpreso = descargue.IdPlanilla,
                            TipoImpreso = ADEnumTipoImpreso.Planilla,
                            Usuario = ControllerContext.Current.Usuario,
                            FechaGrabacion = DateTime.Now,
                        };

                        EstadosGuia.InsertarEstadoGuiaImpreso(trazaImpreso);

                        /*********************************** Registrar recibido guia para servicio notificaciones *********************************************/

                        if (descargue.IdServicio == (short)TAEnumServiciosDC.Notificaciones && descargue.RecibidoGuia != null)
                        {
                            descargue.RecibidoGuia.FechaEntrega = descargue.FechaEntrega;
                            descargue.RecibidoGuia.IdGuia = ultimoEstadoGuia.IdAdmision.Value;
                            descargue.RecibidoGuia.Otros = "Descargue desde APP";
                            fachadaLogisticaInversa.RegistrarRecibidoGuiaManual(descargue.RecibidoGuia);
                        }
                        /******************* INSERCION IMAGENES PRUEBA ENTREGA *****************************/
                        descargue.IdCiudad = ultimoEstadoGuia.IdCiudad;
                        descargue.NombreCiudad = ultimoEstadoGuia.Ciudad;
                        LIRepositorioPruebasEntrega.Instancia.InsertarImagenesPruebaEntrega(descargue);
                        LIRepositorioPruebasEntrega.Instancia.InsertarConteoDescarguesYDevolucionesPorApp(descargue.IdPlanilla, 0, 1);
                        if (ultimoEst == (short)ADEnumEstadoGuia.IntentoEntrega)
                        {
                            LIRepositorioPruebasEntrega.Instancia.InsertarConteoDescarguesYDevolucionesPorApp(descargue.IdPlanilla, 0, -1);
                        }
                        respuesta.Resultado = OUEnumValidacionDescargue.Exitosa;
                        respuesta.Mensaje = "Descargue exitoso";
                    }
                    else
                    {
                        respuesta.Resultado = OUEnumValidacionDescargue.SinAsignar;
                        respuesta.Mensaje = "La guía no está asignada al auditor";
                    }

                    transaction.Complete();
                    return respuesta;
                }
            }
            return respuesta;
        }

        #region Sispostal

        /// <summary>
        /// Descargue guias mensajero controller App
        /// </summary>
        /// <param name="descargue"></param>
        /// <returns></returns>
        public LIEstadoDescargueControllerAppDC DescargueMasivosControllerApp(LIDescargueControllerAppDC descargue)
        {
            LIEstadoDescargueControllerAppDC respuesta = new LIEstadoDescargueControllerAppDC();
            respuesta.Resultado = OUEnumValidacionDescargue.ErrorEstado;
            respuesta.Mensaje = "La guía no se encuentra en reparto";

            /*************************** Obtiene el ultmimo estado guia ********************************/
            ADTrazaGuia ultimoEstadoGuia = EstadosGuia.ObtenerTrazaUltimoEstadoXNumGuiaMasivos(descargue.NumeroGuia);
            /***************************** Valida que el ultimo estado registrado sea en zona ********************************************/
            if ((ultimoEstadoGuia.IdEstadoGuia == (short)ADEnumEstadoGuiaMasivos.EnZona)
                && (descargue.IdEstado == (short)ADEnumEstadoGuiaMasivos.Entregado
                || descargue.IdEstado == (short)ADEnumEstadoGuiaMasivos.Devuelto
                || descargue.IdEstado == (short)ADEnumEstadoGuiaMasivos.Intento_Etrega))
            {
                /********************************* Informacion de guia para registrar desargue******************************************/
                bool intentoEntrega = descargue.IdEstado == (short)ADEnumEstadoGuiaMasivos.Intento_Etrega ? true : false;
                ultimoEstadoGuia.IdNuevoEstadoGuia = intentoEntrega ? (short)ADEnumEstadoGuiaMasivos.Entregado : descargue.IdEstado;
                ultimoEstadoGuia.Observaciones = descargue.Observaciones;
                ultimoEstadoGuia.Modulo = COConstantesModulos.PRUEBAS_DE_ENTREGA;
                ultimoEstadoGuia.Latitud = descargue.Latitud;
                ultimoEstadoGuia.Longitud = descargue.Longitud;
                ultimoEstadoGuia.FechaGrabacion = descargue.FechaGrabacion;
                ultimoEstadoGuia.Usuario = descargue.Usuario;
                ultimoEstadoGuia.MotivoGuia = descargue.MotivoGuia;

                using (TransactionScope transaction = new TransactionScope())
                {
                    /********************* Se valida si la guia está planilla y corresponde al mensajero ************************/
                    bool planillaValida = true;
                    //Replicar esta consulta con SISPOSTAL
                    //planillaValida = LIRepositorioPruebasEntrega.Instancia.ValidarPlanillaDescargueMensajero(descargue.NumeroGuia, descargue.IdMensajero);
                    if (planillaValida)
                    {
                        /***************************** ACTUALIZA ESTADO GUIA ****************************/
                        if (descargue.IdEstado == (short)ADEnumEstadoGuiaMasivos.Entregado || descargue.IdEstado == (short)ADEnumEstadoGuiaMasivos.Intento_Etrega)
                        {
                            /***************************** (ENTREGA - INTENTO ENTREGA) ****************************/
                            EstadosGuia.ActualizarEntregadoGuiaMasivos(ultimoEstadoGuia, intentoEntrega);
                        }
                        else
                        {
                            /***************************** (DEVOLUCIÓN) ****************************/
                            EstadosGuia.DevolucionGuiaMasivos(ultimoEstadoGuia);
                        }

                        /***************************** INSERTA PRUEBA DE ENTREGA ****************************/
                        string rutaImagen = LIRepositorioPruebasEntrega.Instancia.InsertarPruebasEntregaMasivos(descargue, intentoEntrega);

                        ///******************* INSERCION IMAGENES PRUEBA ENTREGA *****************************/
                        LIRepositorioPruebasEntrega.Instancia.InsertarImagenesPruebaEntregaMasivos(descargue, rutaImagen);

                        respuesta.Resultado = OUEnumValidacionDescargue.Exitosa;
                        respuesta.Mensaje = "Descargue satisfactorio";
                    }
                    else
                    {
                        respuesta.Resultado = OUEnumValidacionDescargue.SinAsignar;
                        respuesta.Mensaje = "La guía no está planillada al mensajero";
                    }
                    transaction.Complete();
                    return respuesta;
                }
            }
            return respuesta;
        }

        #endregion
    }
}


