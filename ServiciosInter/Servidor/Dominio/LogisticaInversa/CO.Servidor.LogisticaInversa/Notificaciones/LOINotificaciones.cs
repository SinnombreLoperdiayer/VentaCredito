using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Clientes;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Transactions;

namespace CO.Servidor.LogisticaInversa.Notificaciones
{
    public class LOINotificaciones : ControllerBase
    {
        private static readonly LOINotificaciones instancia = (LOINotificaciones)FabricaInterceptores.GetProxy(new LOINotificaciones(), COConstantesModulos.NOTIFICACIONES);

        private IPUFachadaCentroServicios fachadaCentroServicios = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();

        private ICLFachadaClientes fachadaClientes = COFabricaDominio.Instancia.CrearInstancia<ICLFachadaClientes>();

        private IADFachadaAdmisionesMensajeria fachadaMensajeria = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>();

        private static int numeroRegistrosGuiasInternas = 150;

        public static LOINotificaciones Instancia
        {
            get { return LOINotificaciones.instancia; }
        }

        #region Planillas

        /// <summary>
        ///
        /// </summary>
        /// <param name="guia"></param>
        /// <returns></returns>
        public ADNotificacion AdicionarGuiaPlanilla(ADNotificacion guia)
        {
            PUCentroServiciosDC centroServicioDestino = new PUCentroServiciosDC();
            LIPlanillaCertificacionesDC planilla = LIRepositorioNotificaciones.Instancia.ObtenerPlanillaCertificaciones(guia.IdPlanillaCertificacionGuia);

            using (TransactionScope transaccion = new TransactionScope())
            {
                ADTipoEntrega tipoEntrega = new ADTipoEntrega { Id = "1", Descripcion = "ENTREGA EN DIRECCION" };

                if (planilla != null && planilla.NumeroPlanilla != 0)
                {
                    if (planilla.TipoPlanilla == LIEnumTipoPlanillaNotificacion.CRE)
                    {
                        ADGuia guiaAdmision = fachadaMensajeria.ObtenerGuiaXNumeroGuiaCredito(guia.GuiaAdmision.NumeroGuia);
                        if (planilla.IdDestinatario == guiaAdmision.IdSucursal)
                        {
                            fachadaMensajeria.ValidarNotificacionDevolucion(guiaAdmision.IdAdmision);
                            guia.GuiaInterna = planilla.GuiaInterna;
                            guia.GuiaAdmision = guiaAdmision;
                            guia.CentroServicioDestino = new PUCentroServiciosDC
                            {
                                IdCentroServicio = guia.GuiaInterna.IdCentroServicioDestino,
                                Nombre = guia.GuiaInterna.NombreCentroServicioDestino
                            };
                            LIRepositorioNotificaciones.Instancia.GuardarGuiaPlanillaCertificacion(guia);
                        }
                        else
                        {
                            ControllerException excepcion =
                                new ControllerException
                                    (COConstantesModulos.NOTIFICACIONES,
                                    LOIEnumTipoErrorLogisticaInversa.EX_GUIA_NO_CREDITO.ToString(),
                                    LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.EX_GUIA_NO_CREDITO));
                            throw new FaultException<ControllerException>(excepcion);
                        }
                    }
                    else if (planilla.TipoPlanilla == LIEnumTipoPlanillaNotificacion.CES)
                    {
                        ADGuia guiaAdmision = fachadaMensajeria.ObtenerGuiaXNumeroGuia(guia.GuiaAdmision.NumeroGuia);
                        guia = fachadaMensajeria.ValidarNotificacionDevolucion(guiaAdmision.IdAdmision);
                        guia.GuiaAdmision = guiaAdmision;
                        guia.IdPlanillaCertificacionGuia = planilla.NumeroPlanilla;

                        if (guia.TipoDestino.Id == "EDR")
                        {
                            // TODO: WALTER REVISAR PORQUE SI ES "ENV" no está funcionando bien!!!
                            tipoEntrega = new ADTipoEntrega { Id = "1", Descripcion = "ENTREGA EN DIRECCION" };
                            centroServicioDestino = fachadaCentroServicios.ObtenerCOLResponsable(guiaAdmision.IdCentroServicioOrigen);
                        }
                        else if (guia.TipoDestino.Id == "REP")
                        {
                            tipoEntrega = new ADTipoEntrega { Id = "2", Descripcion = "RECLAME EN OFICINA" };
                            centroServicioDestino = fachadaCentroServicios.ObtenerCOLResponsable(guiaAdmision.IdCentroServicioOrigen);
                        }
                        else if (guia.TipoDestino.Id == "ENV")
                        {
                            tipoEntrega = new ADTipoEntrega { Id = "1", Descripcion = "ENTREGA EN  NUEVA DIRECCION" };
                            centroServicioDestino = fachadaCentroServicios.ObtenerCentroLogisticoApoyaMunicipio(guia.CiudadDestino.IdLocalidad);
                        }

                        if (planilla.IdDestinatario == centroServicioDestino.IdCentroServicio)
                        {
                            guia.CentroServicioDestino = centroServicioDestino;
                        }
                        else
                        {
                            ControllerException excepcion =
                         new ControllerException
                             (COConstantesModulos.NOTIFICACIONES,
                             LOIEnumTipoErrorLogisticaInversa.EX_ERROR_GUIA_DESTINO.ToString(),
                             LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.EX_ERROR_GUIA_DESTINO));
                            throw new FaultException<ControllerException>(excepcion);
                        }
                        guia.GuiaInterna = GenerarGuiaInternaCertificacion(planilla, guia, tipoEntrega);
                        LIRepositorioNotificaciones.Instancia.GuardarGuiaPlanillaCertificacion(guia);
                    }
                    fachadaMensajeria.ActualizarPLanilladaNotificacion(guia);
                }
                transaccion.Complete();
            }
            return guia;
        }

        /// <summary>
        /// Método para generar las guias de la planilla de entrega
        /// </summary>
        /// <param name="planilla"></param>
        public void AdicionarGuiasPlanillaEntrega(LIPlanillaCertificacionesDC planilla)
        {
            ADTipoEntrega tipoEntrega;
            if (planilla.TipoPlanilla == LIEnumTipoPlanillaNotificacion.CES)
            {
                if (planilla.LstGuiasPlanilla != null && planilla.LstGuiasPlanilla.Any())
                {
                    planilla.LstGuiasPlanilla.ToList().ForEach(guia =>
                    {
                        ADGuia GuiaAdmision = fachadaMensajeria.ObtenerGuiaXNumeroGuia(guia.GuiaAdmision.NumeroGuia);
                        guia = fachadaMensajeria.ObtenerNotificacionGuia(GuiaAdmision.IdAdmision);
                        guia.GuiaAdmision = GuiaAdmision;
                        using (TransactionScope transaccion = new TransactionScope())
                        {
                            guia.IdPlanillaCertificacionGuia = planilla.NumeroPlanilla;
                            switch (guia.TipoDestino.Id)
                            {
                                case "EDR":
                                    guia.CentroServicioDestino = fachadaCentroServicios.ObtenerCOLResponsable(guia.GuiaAdmision.IdCentroServicioOrigen);
                                    tipoEntrega = new ADTipoEntrega { Id = "1", Descripcion = "ENTREGA EN DIRECCION" };
                                    break;

                                case "REP":
                                    guia.CentroServicioDestino = new PUCentroServiciosDC { IdCentroServicio = guia.GuiaAdmision.IdCentroServicioOrigen, Nombre = guia.GuiaAdmision.NombreCentroServicioOrigen, IdMunicipio = guia.GuiaAdmision.IdCiudadOrigen, NombreMunicipio = guia.GuiaAdmision.NombreCiudadOrigen };
                                    tipoEntrega = new ADTipoEntrega { Id = "2", Descripcion = "RECLAME EN OFICINA" };
                                    break;

                                case "ENV":
                                    guia.CentroServicioDestino = fachadaCentroServicios.ObtieneCOLPorLocalidad(guia.CiudadDestino.IdLocalidad);
                                    tipoEntrega = new ADTipoEntrega { Id = "1", Descripcion = "ENTREGA EN DIRECCION" };
                                    break;

                                default:
                                    guia.CentroServicioDestino = new PUCentroServiciosDC { IdCentroServicio = guia.GuiaAdmision.IdCentroServicioOrigen, Nombre = guia.GuiaAdmision.NombreCentroServicioOrigen, IdMunicipio = guia.GuiaAdmision.IdCiudadOrigen, NombreMunicipio = guia.GuiaAdmision.NombreCiudadOrigen };
                                    tipoEntrega = new ADTipoEntrega { Id = "1", Descripcion = "ENTREGA EN DIRECCION" };
                                    break;
                            }
                            guia.GuiaInterna = GenerarGuiaInternaCertificacion(planilla, guia, tipoEntrega);
                            LIRepositorioNotificaciones.Instancia.GuardarGuiaPlanillaCertificacion(guia);
                            fachadaMensajeria.ActualizarPLanilladaNotificacion(guia);
                            transaccion.Complete();
                        }
                    });
                    CerrarPlanillaNotificaciones(planilla.NumeroPlanilla);
                }
            }
            else if (planilla.TipoPlanilla == LIEnumTipoPlanillaNotificacion.CRE)
            {
                int i = 0;
                int j = 0;
                int numeroCiclos = 0;
                if (planilla.LstGuiasPlanilla != null && planilla.LstGuiasPlanilla.Any())
                {
                    numeroCiclos = (planilla.LstGuiasPlanilla.Count / numeroRegistrosGuiasInternas) + 1;
                }
                while (i < numeroCiclos)
                {

                    ADGuiaInternaDC GuiaInterna = fachadaMensajeria.ObtenerGuiaInterna(planilla.GuiaInterna.NumeroGuia);
                    if (i > 0)
                    {
                        GuiaInterna.PaisDefault = new PALocalidadDC
                        {
                            IdLocalidad = ConstantesFramework.ID_LOCALIDAD_COLOMBIA,
                            Nombre = ConstantesFramework.DESC_LOCALIDAD_COLOMBIA
                        };
                        GuiaInterna = fachadaMensajeria.AdicionarGuiaInterna(GuiaInterna);
                    }


                    while (j < (numeroRegistrosGuiasInternas * (i + 1)))
                    {
                        using (TransactionScope transaccion = new TransactionScope())
                        {
                            if (planilla.LstGuiasPlanilla.Count > j)
                            {
                                planilla.LstGuiasPlanilla[j].GuiaInterna = GuiaInterna;
                                planilla.LstGuiasPlanilla[j].IdPlanillaCertificacionGuia = planilla.NumeroPlanilla;
                                planilla.LstGuiasPlanilla[j].CentroServicioDestino = planilla.CentroServiciosPlanilla;
                                LIRepositorioNotificaciones.Instancia.GuardarGuiaPlanillaCertificacion(planilla.LstGuiasPlanilla[j]);
                                fachadaMensajeria.ActualizarPLanilladaNotificacion(planilla.LstGuiasPlanilla[j]);
                                transaccion.Complete();
                            }
                            else
                                break;
                        }
                        j++;
                    }
                    i++;
                    j = i * numeroRegistrosGuiasInternas;
                };
            }
        }

        /// <summary>
        /// Método que adiciona una planilla de certificación
        /// </summary>
        /// <param name="planilla"></param>
        /// <returns></returns>
        public LIPlanillaCertificacionesDC AdicionarPlanilla(LIPlanillaCertificacionesDC planilla)
        {
            using (TransactionScope transaccion = new TransactionScope())
            {
                if (planilla.TipoPlanilla == LIEnumTipoPlanillaNotificacion.CRE)
                {
                    planilla.GuiaInterna = GenerarGuiaInternaPlanillaCertificacion(planilla);
                }
                else
                {
                    planilla.GuiaInterna = new ADGuiaInternaDC { NumeroGuia = 0, IdAdmisionGuia = 0 };
                }

                planilla = LIRepositorioNotificaciones.Instancia.GuardarPlanillaCertificacionesAdo(planilla);
                transaccion.Complete();
                return planilla;
            }
        }

        /// <summary>
        /// Método para cerrar una planilla de notificaciones
        /// </summary>
        /// <param name="numeroPlanilla"></param>
        public void CerrarPlanillaNotificaciones(long numeroPlanilla)
        {
            using (TransactionScope transaccion = new TransactionScope())
            {
                LIRepositorioNotificaciones.Instancia.CerrarPlanillaNotificaciones(numeroPlanilla);
                transaccion.Complete();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="planilla"></param>
        public ADGuiaInternaDC GenerarGuiaInternaCertificacion(LIPlanillaCertificacionesDC planilla, ADNotificacion guia, ADTipoEntrega tipoEntrega)
        {
            ADGuiaInternaDC guiaInterna = new ADGuiaInternaDC()
            {
                DiceContener = string.Format(LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.IN_CONTENIDO_GUIA_INTERNA_CERTIFICACION), guia.GuiaAdmision.NumeroGuia, DateTime.Now),
                EsOrigenGestion = false,
                EsManual = false,
                EsDestinoGestion = false,
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
                TipoEntrega = tipoEntrega,
                DireccionDestinatario = guia.DireccionDestinatario,
                TelefonoDestinatario = guia.TelefonoDestinatario == null ? string.Empty : guia.TelefonoDestinatario,
                NombreDestinatario = guia.NombreDestinatario,
                LocalidadOrigen = planilla.CentroServiciosPlanilla.CiudadUbicacion,
                LocalidadDestino = guia.CiudadDestino,
                NombreRemitente = planilla.CentroServiciosPlanilla.Nombre,
                DireccionRemitente = planilla.CentroServiciosPlanilla.Direccion,
                TelefonoRemitente = planilla.CentroServiciosPlanilla.Telefono1,

                NombreCentroServicioDestino = guia.CentroServicioDestino.Nombre,
                IdCentroServicioDestino = guia.CentroServicioDestino.IdCentroServicio,

                NombreCentroServicioOrigen = planilla.CentroServiciosPlanilla.Nombre,
                IdCentroServicioOrigen = planilla.CentroServiciosPlanilla.IdCentroServicio,
            };

            return fachadaMensajeria.AdicionarGuiaInterna(guiaInterna);
        }

        /// <summary>
        /// Método para obtener las guias de una planilla de certificación
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <returns></returns>
        public List<ADNotificacion> ObtenerGuiasPlanillaCertificacion(long idPlanilla)
        {
            return LIRepositorioNotificaciones.Instancia.ObtenerGuiasPlanillaCertificacion(idPlanilla);
        }

        /// <summary>
        /// Obtener planillas de certificación
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <returns></returns>
        public List<LIPlanillaCertificacionesDC> ObtenerPlanillasCertificaciones(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, bool esDevolucion, out int totalRegistros)
        {
            return LIRepositorioNotificaciones.Instancia.ObtenerPlanillasCertificaciones(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, esDevolucion, out totalRegistros);
        }

        /// <summary>
        /// Obtener planillas de certificacion con Ado
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="esDevolucion"></param>
        /// <param name="totalPaginas"></param>
        /// <returns></returns>
        public IEnumerable<LIPlanillaCertificacionesDC> ObtenerPlanillasCertificacionesAdo(IDictionary<String, String> filtro, String campoOrdenamiento, Int32 indicePagina, Int32 registrosPorPagina, Boolean ordenamientoAscendente, Boolean esDevolucion, out Int32 totalPaginas)
        {
            return LIRepositorioNotificaciones.Instancia.ObtenerPlanillasCertificacionesAdo(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, esDevolucion, out totalPaginas);
        }

        /// <summary>
        /// Genera la guia interna de la planilla de certificaciones
        /// </summary>
        /// <param name="planilla"></param>
        /// <returns></returns>
        private ADGuiaInternaDC GenerarGuiaInternaPlanillaCertificacion(LIPlanillaCertificacionesDC planilla)
        {
            ADGuiaInternaDC guia = new ADGuiaInternaDC()
            {
                DiceContener = string.Empty,
                EsOrigenGestion = false,
                EsManual = false,
                EsDestinoGestion = false,

                GestionDestino = new ARGestionDC()
                {
                    IdGestion = 0,
                    Descripcion = string.Empty
                },
                GestionOrigen = new ARGestionDC()
                {
                    IdCasaMatriz = 1,
                    IdGestion = 0,
                    Descripcion = string.Empty
                },
                PaisDefault = new PALocalidadDC()
                {
                    IdLocalidad = ConstantesFramework.ID_LOCALIDAD_COLOMBIA,
                    Nombre = ConstantesFramework.DESC_LOCALIDAD_COLOMBIA,
                },
                NombreRemitente = planilla.CentroServiciosPlanilla.Nombre,
                DireccionRemitente = planilla.CentroServiciosPlanilla.Direccion,
                TelefonoRemitente = planilla.CentroServiciosPlanilla.Telefono1,
                NombreCentroServicioOrigen = planilla.CentroServiciosPlanilla.Nombre,
                IdCentroServicioOrigen = planilla.CentroServiciosPlanilla.IdCentroServicio,
                LocalidadOrigen = planilla.CentroServiciosPlanilla.CiudadUbicacion,
                IdentificacionRemitente = planilla.CentroServiciosPlanilla.IdCentroServicio.ToString()
            };
            if (planilla.TipoPlanilla == LIEnumTipoPlanillaNotificacion.CRE)
            {
                guia.DireccionDestinatario = planilla.DireccionDestinatario;
                guia.TelefonoDestinatario = planilla.TelefonoDestinatario;
                guia.NombreDestinatario = planilla.NombreDestinatario;
                guia.LocalidadDestino = planilla.LocalidadDestino;
                guia.NombreCentroServicioDestino = planilla.CentroServiciosPlanilla.Nombre;
                guia.IdCentroServicioDestino = planilla.CentroServiciosPlanilla.IdCentroServicio;
                guia.IdentificacionDestinatario = planilla.IdDestinatario.ToString();
            }
            else if (planilla.TipoPlanilla == LIEnumTipoPlanillaNotificacion.CES)
            {
                PUCentroServiciosDC centroServicioDestino = fachadaCentroServicios.ObtenerAgenciaLocalidad(planilla.LocalidadDestino.IdLocalidad);

                guia.DireccionDestinatario = planilla.DireccionDestinatario;
                guia.TelefonoDestinatario = planilla.TelefonoDestinatario;
                guia.NombreDestinatario = planilla.NombreDestinatario;
                guia.LocalidadDestino = planilla.LocalidadDestino;
                guia.IdCentroServicioDestino = centroServicioDestino.IdCentroServicio;
                guia.NombreCentroServicioDestino = centroServicioDestino.Nombre;
                guia.IdentificacionDestinatario = centroServicioDestino.IdCentroServicio.ToString();
            }

            return fachadaMensajeria.AdicionarGuiaInterna(guia);
        }

        #endregion Planillas


        #region Recibido Guia

        /// <summary>
        /// Método para obtener guias pendientes de captura datos de recibido
        /// </summary>
        /// <param name="idCol"></param>
        /// <returns></returns>
        public List<LIRecibidoGuia> ObtenerRecibidosPendientes(long idCol, DateTime fechaInicial, DateTime fechaFinal)
        {
            return LIRepositorioNotificaciones.Instancia.ObtenerRecibidosPendientes(idCol, fechaInicial, fechaFinal);
        }

        public List<LIRecibidoGuia> ObtenerRecibidosPendientesApp(long idCol, DateTime fechaInicial, DateTime fechaFinal, LIEnumOrigenAplicacion idAplicacionOrigen)
        {
            return LIRepositorioNotificaciones.Instancia.ObtenerRecibidosPendientesApp(idCol, fechaInicial, fechaFinal, idAplicacionOrigen);
        }


        #endregion
    }
}