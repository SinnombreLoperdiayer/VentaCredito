using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Transactions;
using CO.Servidor.Adminisiones.Mensajeria.Comun;
using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.LogisticaInversa.Comun;
using CO.Servidor.LogisticaInversa.Datos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Area;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.LogisticaInversa.Notificaciones
{
    public class LINotificaciones : ControllerBase
    {
        private static readonly LINotificaciones instancia = (LINotificaciones)FabricaInterceptores.GetProxy(new LINotificaciones(), COConstantesModulos.NOTIFICACIONES);

        public static LINotificaciones Instancia
        {
            get { return LINotificaciones.instancia; }
        }

        private IADFachadaAdmisionesMensajeria fachadaMensajeria = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>();

        /// <summary>
        /// Indica si el recibido de la guía ha sido registrado previamente
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public bool RecibidoRegistrado(long numeroGuia)
        {
            return LIRepositorioNotificaciones.Instancia.RecibidoRegistrado(numeroGuia);
        }

        /// <summary>
        /// Registra recibido de guía manual
        /// </summary>
        /// <param name="recibido"></param>
        public void RegistrarRecibidoGuiaManual(LIRecibidoGuia recibido)
        {
            if(recibido.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
            LIRepositorioNotificaciones.Instancia.RegistrarRecibidoGuiaManual(recibido);
            else if (recibido.EstadoRegistro == EnumEstadoRegistro.MODIFICADO)
             LIRepositorioNotificaciones.Instancia.ModificarRecibidoGuiaManual(recibido);
        }

        /// <summary>
        /// Método para obtener las guías de notificaciones
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <returns></returns>
        public List<ADNotificacion> ObtenerNotificacionesRecibido(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina)
        {
            return fachadaMensajeria.ObtenerNotificacionesRecibido(filtro, indicePagina, registrosPorPagina);
        }

        /// <summary>
        /// Método para generar las guías internas de las notificaciones
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <returns></returns>
        public int GenerarGuiasInternasNotificacion(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina)
        {
            int contador = 0;
            string esConsolidado;
            string nit;

            filtro.TryGetValue("esConsolidado", out esConsolidado);
            filtro.TryGetValue("nit", out nit);

            if (!string.IsNullOrEmpty(nit) && esConsolidado == true.ToString())
            {
                using (TransactionScope transaccion = new TransactionScope())
                {
                    List<ADNotificacion> listaNotificaciones;
                    listaNotificaciones = fachadaMensajeria.ObtenerNotificacionesRecibido(filtro, indicePagina, registrosPorPagina).ToList();

                    ADGuia guiaAdmision = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>().ObtenerGuia(listaNotificaciones.FirstOrDefault().GuiaAdmision.IdAdmision);
                    ADGuiaInternaDC GuiaInterna = new ADGuiaInternaDC()
                    {
                        GestionOrigen = new ARGestionDC { IdGestion = 0, Descripcion = string.Empty },
                        GestionDestino = new ARGestionDC { IdGestion = 0, Descripcion = string.Empty },
                        DiceContener = guiaAdmision.DiceContener,
                        DireccionDestinatario = listaNotificaciones.FirstOrDefault().DireccionDestinatario,
                        DireccionRemitente = guiaAdmision.DireccionDestinatario,
                        EsManual = false,
                        IdAdmisionGuia = 0,
                        IdCentroServicioOrigen = guiaAdmision.IdCentroServicioDestino,
                        LocalidadDestino = listaNotificaciones.FirstOrDefault().CiudadDestino,
                        LocalidadOrigen = new PALocalidadDC() { IdLocalidad = guiaAdmision.IdCiudadDestino, Nombre = guiaAdmision.NombreCiudadDestino },
                        NombreCentroServicioOrigen = guiaAdmision.NombreCentroServicioDestino,
                        NombreDestinatario = guiaAdmision.NombreCentroServicioDestino,
                        NombreRemitente = listaNotificaciones.FirstOrDefault().NombreDestinatario,
                        NumeroGuia = 0,
                        PaisDefault = listaNotificaciones.FirstOrDefault().PaisDestino,
                        TelefonoDestinatario = listaNotificaciones.FirstOrDefault().TelefonoDestinatario,
                        TelefonoRemitente = guiaAdmision.Remitente.Telefono,
                        EsDestinoGestion = false,
                        EsOrigenGestion = false,
                    };
                    GuiaInterna = fachadaMensajeria.AdicionarGuiaInterna(GuiaInterna);

                    listaNotificaciones.ForEach(g =>
                    {
                        if (g.GuiaInterna.NumeroGuia != 0)
                        {
                            fachadaMensajeria.ActualizarNotificacion(g.GuiaAdmision.IdAdmision, g.GuiaInterna.NumeroGuia);
                        }
                    })
                    ;
                    transaccion.Complete();
                    return 1;
                }
            }
            else
            {
                using (TransactionScope transaccion = new TransactionScope())
                {
                    List<ADNotificacion> listaNotificaciones;
                    listaNotificaciones = fachadaMensajeria.ObtenerNotificacionesRecibido(filtro, indicePagina, registrosPorPagina).ToList();

                    listaNotificaciones.ForEach(g =>
                    {
                        if (g.GuiaInterna.NumeroGuia == 0)
                        {
                            ADGuia guiaAdmision = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>().ObtenerGuia(g.GuiaAdmision.IdAdmision);
                            ADGuiaInternaDC GuiaInterna = new ADGuiaInternaDC()
                            {
                                GestionOrigen = new ARGestionDC { IdGestion = 0, Descripcion = string.Empty },
                                GestionDestino = new ARGestionDC { IdGestion = 0, Descripcion = string.Empty },
                                DiceContener = guiaAdmision.DiceContener,
                                DireccionDestinatario = g.DireccionDestinatario,
                                DireccionRemitente = guiaAdmision.DireccionDestinatario,
                                EsManual = false,
                                IdAdmisionGuia = 0,
                                IdCentroServicioOrigen = guiaAdmision.IdCentroServicioDestino,
                                LocalidadDestino = g.CiudadDestino,
                                LocalidadOrigen = new PALocalidadDC() { IdLocalidad = guiaAdmision.IdCiudadDestino, Nombre = guiaAdmision.NombreCiudadDestino },
                                NombreCentroServicioOrigen = guiaAdmision.NombreCentroServicioDestino,
                                NombreDestinatario = guiaAdmision.NombreCentroServicioDestino,
                                NombreRemitente = g.NombreDestinatario,
                                NumeroGuia = 0,
                                PaisDefault = g.PaisDestino,
                                TelefonoDestinatario = g.TelefonoDestinatario,
                                TelefonoRemitente = guiaAdmision.Remitente.Telefono,
                                EsDestinoGestion = false,
                                EsOrigenGestion = false,
                            };
                            g.GuiaInterna = fachadaMensajeria.AdicionarGuiaInterna(GuiaInterna);
                            fachadaMensajeria.ActualizarNotificacion(g.GuiaAdmision.IdAdmision, g.GuiaInterna.NumeroGuia);
                            contador++;
                        }
                    })
                    ;
                    transaccion.Complete();
                    return contador;
                }
            }
        }

        /// <summary>
        /// Método para obtener los id de las guías de notificaciones
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <returns></returns>
        public List<long> ObtenerIdNotificaciones(IDictionary<string, string> filtro)
        {
            return fachadaMensajeria.ObtenerIdNotificaciones(filtro);
        }

        #region Planilla Certificacion

   

        /// <summary>
        /// Guarda guia en la planilla de certificacion
        /// </summary>
        /// <param name="planilla"></param>
        public LIPlanillaCertificacionesDC GuardarGuiaPlanillaCertificacion(LIPlanillaCertificacionesDC planilla)
        {
            //using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new System.TimeSpan(0, 10, 0)))
            //{
                
            //    List<ADNotificacion> guiasPeaton = new List<ADNotificacion>();
            //    List<ADNotificacion> guiasPeatonNoREP = new List<ADNotificacion>();
            //    List<ADNotificacion> guiasConvenio = new List<ADNotificacion>();

            //    long numeroPlanilla = 0;
            //    ADGuiaInternaDC guiaInterna = null;
            //    planilla.LstGuiasPlanilla.ToList().ForEach(not =>
            //    {
            //        if (not.EstadoRegistro == EnumEstadoRegistro.ADICIONADO &&
            //            (not.GuiaAdmision.TipoCliente == ADEnumTipoCliente.PCO
            //            || not.GuiaAdmision.TipoCliente == ADEnumTipoCliente.PEA
            //            || not.GuiaAdmision.TipoCliente == ADEnumTipoCliente.PPE)
            //            && not.TipoDestino.Id == ADConstantes.TIPO_ENVIO_RECLAMA_EN_PUNTO
            //            )
            //        {
            //            guiasPeaton.Add(not);
            //        }

            //        if (not.EstadoRegistro == EnumEstadoRegistro.ADICIONADO &&
            //            (not.GuiaAdmision.TipoCliente == ADEnumTipoCliente.PCO
            //        || not.GuiaAdmision.TipoCliente == ADEnumTipoCliente.PEA
            //        || not.GuiaAdmision.TipoCliente == ADEnumTipoCliente.PPE)
            //        && not.TipoDestino.Id != ADConstantes.TIPO_ENVIO_RECLAMA_EN_PUNTO)
            //        {
            //            guiasPeatonNoREP.Add(not);
            //        }

            //        if (not.EstadoRegistro == EnumEstadoRegistro.ADICIONADO
            //           && not.GuiaAdmision.TipoCliente == ADEnumTipoCliente.CCO
            //           || not.GuiaAdmision.TipoCliente == ADEnumTipoCliente.CPE)
            //        {
            //            guiasConvenio.Add(not);
            //        }
            //    });

            //    if (guiasPeaton != null && guiasPeaton.Any())
            //    {
            //        ///Obtiene los centros de servicios de las notificacines
            //        List<long> agencias = guiasPeaton.Select(r => r.GuiaAdmision.IdCentroServicioOrigen).Distinct().ToList();

            //        ///Agrupa los envios por centro de servicio
            //        agencias.ForEach(r =>
            //        {
            //            List<ADNotificacion> guiasXPunto = guiasPeaton.Where(guiaP => guiaP.GuiaAdmision.IdCentroServicioOrigen == r).ToList();

            //            PUCentroServiciosDC centro = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerCentroServicio(r);

            //            guiasXPunto.ForEach(guia =>
            //            {
            //                planilla.GuiaPlanilla = guia;
            //                if (planilla.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
            //                {
            //                    planilla.GuiaPlanilla.NombreDestinatario = centro.Nombre;
            //                    planilla.GuiaPlanilla.DireccionDestinatario = centro.Direccion;
            //                    planilla.GuiaPlanilla.TelefonoDestinatario = centro.Telefono1;
            //                    planilla.GuiaPlanilla.CiudadDestino = centro.CiudadUbicacion;

            //                    if (numeroPlanilla <= 0)
            //                    {
            //                        numeroPlanilla = GenerarPlanillaCertificacion(planilla, true);
            //                        planilla.NumeroPlanilla = numeroPlanilla;
            //                    }
            //                    guiaInterna = GenerarGuiaInternaCertificacion(planilla);
            //                    guia.GuiaInterna = guiaInterna;                                
            //                }

            //            });
            //        });
            //    }

            //    ///Agrupa las guias de origen peaton que no son reclame en punto
            //    if (guiasPeatonNoREP != null && guiasPeatonNoREP.Any())
            //    {
            //        guiasPeatonNoREP.ForEach(guia =>
            //        {
            //            planilla.GuiaPlanilla = guia;

            //            ///Genera la planilla
            //            if (planilla.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
            //            {
            //                if (numeroPlanilla <= 0)
            //                    numeroPlanilla = GenerarPlanillaCertificacion(planilla, true);
            //                planilla.NumeroPlanilla = numeroPlanilla;
            //            }

            //            ///se genera guia de correspondencia interna para las certificaciones de entrega o devolucion
            //            guia.GuiaInterna = GenerarGuiaInternaCertificacion(planilla);
            //        });
            //    }

            //    if (guiasConvenio != null && guiasConvenio.Any())
            //    {
            //        var p = guiasConvenio.Select(r => new { r.GuiaAdmision.IdCliente, r.GuiaAdmision.IdSucursal }).Distinct().ToList();

            //        p.ForEach(m =>
            //        {
            //            List<ADNotificacion> guiaConvenioXSucCli = guiasConvenio.Where(g => g.GuiaAdmision.IdSucursal == m.IdSucursal && g.GuiaAdmision.IdCliente == m.IdCliente).ToList();
            //            planilla.NumeroPlanilla = 0;
            //            guiaConvenioXSucCli.ForEach(guia =>
            //            {
            //                if (planilla.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
            //                {
            //                    if (numeroPlanilla <= 0)
            //                        numeroPlanilla = GenerarPlanillaCertificacion(planilla, true);

            //                    planilla.NumeroPlanilla = numeroPlanilla;
            //                }

            //                planilla.GuiaPlanilla.GuiaInterna.IdAdmisionGuia = 0;

            //            });
            //        });
            //    }
            //    scope.Complete();
            //}
            return planilla;
        }

           /// <summary>
        /// Elimina una guia de la planilla de certificaciones
        /// </summary>
        /// <param name="guiaCertificacion"></param>
        /// <returns></returns>
        public ADNotificacion EliminarGuiaPlanillaCertificaciones(ADNotificacion guiaCertificacion)
        {
             LIRepositorioNotificaciones.Instancia.EliminarGuiaPlanillaCertificaciones(guiaCertificacion);
             fachadaMensajeria.ActualizarNotificacionPlanilla(guiaCertificacion.GuiaAdmision.IdAdmision);
             return guiaCertificacion;
        }

        public long GenerarPlanillaCertificacion(LIPlanillaCertificacionesDC planilla, bool generaGuia)
        {
            if (planilla.GuiaInterna == null)
                planilla.GuiaInterna = new ADGuiaInternaDC();
          //  planilla.NumeroPlanilla = LIRepositorioNotificaciones.Instancia.GuardarPlanillaCertificaciones(planilla);
            if (generaGuia)
                planilla.GuiaInterna = GenerarGuiaInternaPlanillaCertificacion(planilla);

            LIRepositorioNotificaciones.Instancia.ActualizarGuiaInternaPlanilla(planilla.NumeroPlanilla, planilla.GuiaInterna.IdAdmisionGuia, planilla.GuiaInterna.NumeroGuia);

            return planilla.NumeroPlanilla;
        }

        /// <summary>
        /// Valida que la guia ingresada cumpla con las condiciones para la certificacion
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ADNotificacion ValidarGuiaCertificacionNotificacion(long numeroGuia)
        {
            LIRepositorioNotificaciones.Instancia.ValidarNotificacionPlanillada(numeroGuia);

            ///Obtiene la notificacion, si la guia ingresada no es del servicio de notificaciones muestra una excepcion
            ADNotificacion notificacion = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>().ObtenerAdmMenNotEntregaDevolucion(numeroGuia);

            ///Para los envios de cliente credito, se obtiene el id de la sucursal y el id el cliente
            if (notificacion.GuiaAdmision.TipoCliente == ADEnumTipoCliente.CCO || notificacion.GuiaAdmision.TipoCliente == ADEnumTipoCliente.CPE)
            {
                ADGuia guia = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>().ObtenerGuiaPorNumeroGuiaConInfoClienteCredito(notificacion.GuiaAdmision.NumeroGuia, notificacion.GuiaAdmision.IdAdmision);
                notificacion.GuiaAdmision.IdCliente = guia.IdCliente;
                notificacion.GuiaAdmision.IdSucursal = guia.IdSucursal;
            }

              return notificacion;
        }

        /// <summary>
        /// Genera la guia interna de la planilla de certificaciones
        /// </summary>
        /// <param name="planilla"></param>
        /// <returns></returns>
        public ADGuiaInternaDC GenerarGuiaInternaPlanillaCertificacion(LIPlanillaCertificacionesDC planilla)
        {
            ADGuiaInternaDC guia = new ADGuiaInternaDC()
            {
                DiceContener = string.Format(LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.IN_CONTENIDO_GUIA_INTERNA_PLANILLA), planilla.NumeroPlanilla),
                EsOrigenGestion = false,
                EsManual = false,
                EsDestinoGestion = false,
                DireccionDestinatario = planilla.GuiaPlanilla.DireccionDestinatario,
                TelefonoDestinatario = planilla.GuiaPlanilla.TelefonoDestinatario,
                NombreDestinatario = planilla.GuiaPlanilla.NombreDestinatario,
                LocalidadDestino = planilla.GuiaPlanilla.CiudadDestino,
                LocalidadOrigen = planilla.CentroServiciosPlanilla.CiudadUbicacion,
                NombreRemitente = planilla.CentroServiciosPlanilla.Nombre,
                DireccionRemitente = planilla.CentroServiciosPlanilla.Direccion,
                TelefonoRemitente = planilla.CentroServiciosPlanilla.Telefono1,
                IdCentroServicioDestino = planilla.GuiaPlanilla.GuiaAdmision.IdCentroServicioOrigen,
                NombreCentroServicioDestino = planilla.GuiaPlanilla.GuiaAdmision.NombreCentroServicioOrigen,
                GestionDestino = new ARGestionDC()
                {
                    IdGestion = 0,
                    Descripcion = string.Empty
                },
                GestionOrigen = new ARGestionDC()
                {
                    IdGestion = 0,
                    Descripcion = string.Empty
                },
                PaisDefault = new PALocalidadDC()
                {
                    IdLocalidad = ConstantesFramework.ID_LOCALIDAD_COLOMBIA,
                    Nombre = ConstantesFramework.DESC_LOCALIDAD_COLOMBIA,
                },
                NombreCentroServicioOrigen = planilla.CentroServiciosPlanilla.Nombre,
                IdCentroServicioOrigen = planilla.CentroServiciosPlanilla.IdCentroServicio,
            };

            return CrearGuiaInternaCertificacion(guia);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="planilla"></param>
        public ADGuiaInternaDC GenerarGuiaInternaCertificacion(LIPlanillaCertificacionesDC planilla)
        {
            ADGuiaInternaDC guia = new ADGuiaInternaDC()
            {
                DiceContener = string.Format(LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.IN_CONTENIDO_GUIA_INTERNA_CERTIFICACION), planilla.GuiaPlanilla.GuiaAdmision.NumeroGuia, DateTime.Now),
                EsOrigenGestion = false,
                EsManual = false,
                EsDestinoGestion = false,
                DireccionDestinatario = planilla.GuiaPlanilla.DireccionDestinatario,
                TelefonoDestinatario = planilla.GuiaPlanilla.TelefonoDestinatario,
                NombreDestinatario = planilla.GuiaPlanilla.NombreDestinatario,
                LocalidadDestino = planilla.GuiaPlanilla.CiudadDestino,
                LocalidadOrigen = planilla.CentroServiciosPlanilla.CiudadUbicacion,
                NombreRemitente = planilla.CentroServiciosPlanilla.Nombre,
                DireccionRemitente = planilla.CentroServiciosPlanilla.Direccion,
                TelefonoRemitente = planilla.CentroServiciosPlanilla.Telefono1,
                GestionDestino = new ARGestionDC()
                {
                    IdGestion = 0,
                    Descripcion = string.Empty
                },
                GestionOrigen = new ARGestionDC()
                {
                    IdGestion = 0,
                    Descripcion = string.Empty
                },
                PaisDefault = new PALocalidadDC()
                {
                    IdLocalidad = ConstantesFramework.ID_LOCALIDAD_COLOMBIA,
                    Nombre = ConstantesFramework.DESC_LOCALIDAD_COLOMBIA
                },

                NombreCentroServicioOrigen = planilla.CentroServiciosPlanilla.Nombre,
                IdCentroServicioOrigen = planilla.CentroServiciosPlanilla.IdCentroServicio,
            };

            ///Si la notificacion tiene el tipo destino, enviar direccion remitente
            ///la agencia destino es la agencia origen de la admision
            if (planilla.GuiaPlanilla.TipoDestino.Id.CompareTo("EDR") == 0)
            {
                PUAgenciaDeRacolDC centro = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerAgenciaResponsable(planilla.GuiaPlanilla.GuiaAdmision.IdCentroServicioOrigen);
                guia.IdCentroServicioDestino = centro.IdResponsable;
                guia.NombreCentroServicioDestino = centro.NombreResponsable;
            }

            ///Si la notificacion tiene el tipo destino, enviar direccion remitente
            ///la agencia destino es la agencia de la ciudad seleccionada en la admision
            else if (planilla.GuiaPlanilla.TipoDestino.Id.CompareTo("ENV") == 0)
            {
                PUCentroServiciosDC centro = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerCentroLogisticoApoyaMunicipio(planilla.GuiaPlanilla.CiudadDestino.IdLocalidad);
                guia.IdCentroServicioDestino = centro.IdCentroServicio;
                guia.NombreCentroServicioDestino = centro.Nombre;
            }

            ///Si la notificacion tiene el tipo destino, reclama en punto
            ///el centro servicios destino es el punto donde se realizo el envio
            else if (planilla.GuiaPlanilla.TipoDestino.Id.CompareTo("REP") == 0)
            {
                PUCentroServiciosDC centro = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerAgenciaLocalidad(planilla.GuiaPlanilla.CiudadDestino.IdLocalidad);
                guia.IdCentroServicioDestino = centro.IdCentroServicio;
                guia.NombreCentroServicioDestino = centro.Nombre;
            }

            return CrearGuiaInternaCertificacion(guia);
        }

        /// <summary>
        /// Crea la guia interna de la remision
        /// </summary>
        /// <param name="guiaInterna"></param>
        /// <returns>NUmero de guia interna</returns>
        public ADGuiaInternaDC CrearGuiaInternaCertificacion(ADGuiaInternaDC guiaInterna)
        {
            return COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>().AdicionarGuiaInterna(guiaInterna);
        }

        /// <summary>
        /// Guarda los envios de la planilla de asignacion
        /// </summary>
        /// <param name="planilla"></param>
        public void GuardarLstEnvioPlanillaCertificacion(LIPlanillaCertificacionesDC planilla)
        {
            if (planilla.NumeroPlanilla <= 0)
            //    planilla.NumeroPlanilla = LIRepositorioNotificaciones.Instancia.GuardarPlanillaCertificaciones(planilla);

            LIRepositorioNotificaciones.Instancia.GuardarLstEnvioPlanillaCertificacion(planilla);
        }

        /// <summary>
        /// Obtiene las notificaciones del col, con los filtros seleccionados que esten sin planillar
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="idCol"></param>
        public List<ADNotificacion> ObtenerNotificacionesFiltroSinPla(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina, long idCol)
        {
            return LIRepositorioNotificaciones.Instancia.ObtenerNotificacionesFiltroSinPla(filtro, indicePagina, registrosPorPagina, idCol);
        }

        /// <summary>
        /// Retorna las guias internas de las planillas
        /// </summary>
        /// <param name="planillas"></param>
        public List<ADGuiaInternaDC> ObtenerGuiasInternasImpresionPlanillas(string planillas, bool EsPlanilla, long idCol)
        {
            List<ADGuiaInternaDC> guiasInt = null;
            if (EsPlanilla)
                guiasInt = LIRepositorioNotificaciones.Instancia.ObtenerGuiasInternasPlanilla(planillas, idCol);
            else
                guiasInt = LIRepositorioNotificaciones.Instancia.ObtenerGuiasInternasCertificacionesPlanilla(planillas, idCol);

            return guiasInt;
        }

        /// <summary>
        /// Descarga la planilla de certificacion
        /// </summary>
        /// <param name="idPlanilla"></param>
        public void DescargarPlanillaCertificaciones(long idPlanilla)
        {
            LIRepositorioNotificaciones.Instancia.DescargarPlanillaCertificaciones(idPlanilla);
        }

        #endregion Planilla Certificacion

    }
}