using System.Collections.Generic;
using System.Transactions;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.Suministros;
using CO.Servidor.Dominio.Comun.Tarifas;
using CO.Servidor.OperacionUrbana.Datos;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Dominio.Comun.AdmEstadosGuia;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using System.ServiceModel;
using CO.Servidor.OperacionUrbana.Comun;
using System.Linq.Expressions;
using System.IO;
using System.Linq;
using System;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Dominio.Comun.CentroServicios;
using System.Threading.Tasks;


namespace CO.Servidor.OperacionUrbana
{
    internal class OUIngresoCentroAcopio : ControllerBase
    {
        private static readonly OUIngresoCentroAcopio instancia = (OUIngresoCentroAcopio)FabricaInterceptores.GetProxy(new OUIngresoCentroAcopio(), COConstantesModulos.MODULO_OPERACION_URBANA);

        private IADFachadaAdmisionesMensajeria fachadaMensajeria = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>();

        /// <summary>
        /// Retorna una instancia de OUManejadorIngreso
        /// /// </summary>
        public static OUIngresoCentroAcopio Instancia
        {
            get { return OUIngresoCentroAcopio.instancia; }
        }

        #region Novedades de ingreso

        /// <summary>
        /// Método para obtener las novedades de ingreso
        /// </summary>
        /// <returns></returns>
        public List<OUNovedadIngresoDC> ObtenerNovedadesIngreso()
        {
            return OURepositorio.Instancia.ObtenerNovedadesIngreso();
        }

        #endregion

        #region Ingreso a centro de acopio

        /// <summary>
        /// Método para obtener las asignaciones
        /// </summary>
        /// <param name="controlTrans"></param>
        /// <param name="noPrecinto"></param>
        /// <param name="noConsolidado"></param>
        /// <returns></returns>
        public List<OUAsignacionDC> ObtenerAsignaciones(long controlTrans, long noPrecinto, string noConsolidado, OUMensajeroDC mensajero, long idCentroServicio)
        {

            List<OUAsignacionDC> asignaciones = new List<OUAsignacionDC>();
            //Consultar una lista con las asignaciones de acuerdo al filtro
            asignaciones = OURepositorio.Instancia.ObtenerAsignaciones(controlTrans, noPrecinto, noConsolidado);
            if (asignaciones.Any())
                using (TransactionScope scope = new TransactionScope())
                {
                    {
                        asignaciones.ForEach(asg =>
                        {
                            asg.Mensajero = mensajero;
                            //ToDo validar que no se encuentre asignado en un manifiesto nacional
                            OURepositorio.Instancia.CambiarEstadoAsignacionTulaPunto(asg.IdAsignacion, OUConstantesOperacionUrbana.ESTADO_INGRESADO);
                            OURepositorio.Instancia.IngresoConsolidado(asg, mensajero, idCentroServicio);
                        });
                    }
                    scope.Complete();
                }
            return asignaciones;
        }

        /// <summary>
        /// Método para validar una guía suelta a centro de acopio 
        /// </summary>
        /// <param name="guia"></param>
        /// <returns></returns>
        public OUPlanillaVentaGuiasDC IngresarGuiaSuelta(OUPlanillaVentaGuiasDC guia, List<OUNovedadIngresoDC> listaNovedades)
        {
            ADGuia guiaAdmision;
            guiaAdmision = fachadaMensajeria.ObtenerInfoGuiaXNumeroGuia(guia.NumeroGuia);
            using (TransactionScope scope = new TransactionScope())
            {
                if (guiaAdmision.IdAdmision != 0)
                {

                    guia.IdAdmision = guiaAdmision.IdAdmision;

                    if (guia.IdCiudadOrigenGuia == guiaAdmision.IdCiudadOrigen)
                    {
                        guia.IdCiudadOrigen = guiaAdmision.IdCiudadOrigen;
                        guia.NombreCiudadOrigen = guiaAdmision.NombreCiudadOrigen;

                        short idEstadoGuia = (short)(EstadosGuia.ObtenerUltimoEstado(guia.IdAdmision));

                        if (idEstadoGuia == (short)ADEnumEstadoGuia.CentroAcopio)
                        {
                            if (OURepositorio.Instancia.ValidarGuiaCentroAcopio(guia.NumeroGuia, guia.IdPuntoServicio))
                            {
                                ControllerException excepcion =
                                                new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, OUEnumTipoErrorOU.EX_GUIA_YA_INGRESADA_CENTRO_ACOPIO.ToString(),
                                                OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_GUIA_YA_INGRESADA_CENTRO_ACOPIO));
                                throw new FaultException<ControllerException>(excepcion);
                            }
                        }

                        PUCentroServiciosDC centroServ = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerAgenciaLocalidad(guia.IdCiudadOrigen);
                        ADTrazaGuia estadoGuia = new ADTrazaGuia
                        {
                            Ciudad = guiaAdmision.NombreCiudadOrigen,
                            IdCiudad = guia.IdCiudadOrigenGuia,
                            IdAdmision = guia.IdAdmision,
                            IdEstadoGuia = idEstadoGuia,
                            IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.CentroAcopio,
                            Modulo = COConstantesModulos.MODULO_OPERACION_URBANA,
                            NumeroGuia = guia.NumeroGuia,
                            Observaciones = string.Empty,
                            NumeroPieza = guia.PiezaActualRotulo,
                            TotalPiezas = guia.TotalPiezasRotulo
                        };
                        estadoGuia.IdTrazaGuia = EstadosGuia.ValidarInsertarEstadoGuia(estadoGuia);
                        if (estadoGuia.IdTrazaGuia == 0)
                        {
                            string descripcionEstado = ", Estado actual " + ((ADEnumEstadoGuia)(estadoGuia.IdEstadoGuia)).ToString();

                            //no pudo realizar el cambio de estado
                            ControllerException excepcion =
                                          new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, OUEnumTipoErrorOU.EX_CAMBIO_ESTADO_NO_VALIDO.ToString(),
                                          OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_CAMBIO_ESTADO_NO_VALIDO));
                            excepcion.Mensaje = excepcion.Mensaje + descripcionEstado;
                            throw new FaultException<ControllerException>(excepcion);
                        }

                        //// Movimiento-Inventario  (Ingreso al COL)
                        PUMovimientoInventario movInventario = new PUMovimientoInventario();
                        movInventario.TipoMovimiento = PUEnumTipoMovimientoInventario.Ingreso;
                        movInventario.IdCentroServicioOrigen = ControllerContext.Current.IdCentroServicio;
                        movInventario.Bodega = new PUCentroServiciosDC() { IdCentroServicio = ControllerContext.Current.IdCentroServicio };
                        movInventario.NumeroGuia = guia.NumeroGuia;
                        movInventario.FechaGrabacion = DateTime.Now;
                        movInventario.FechaEstimadaIngreso = DateTime.Now;
                        movInventario.CreadoPor = ControllerContext.Current.Usuario;
                        COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().AdicionarMovimientoInventario(movInventario);
                        ////////////////////////////////////////////

                        guia.IdIngresoGuia = OURepositorio.Instancia.GuardarIngresoGuiaAgencia(guia);

                        // Lanzar mensaje de texto de cliente crédito si es una guía crédito y el destino requiere lanzamiento de mensaje
                        if (guiaAdmision.TipoCliente == ADEnumTipoCliente.CPE)
                        {
                            new Task(() => CO.Controller.Servidor.Integraciones.MensajesTexto.Instancia.EnviarMensajeTexto(guiaAdmision.NumeroGuia)).Start();
                        }

                        if (listaNovedades != null && listaNovedades.Any())
                        {
                            listaNovedades.ForEach(nov =>
                            {
                                if (nov.Asignado)
                                    OURepositorio.Instancia.GuardarNovedadGuiaIngresada(nov, guia.IdIngresoGuia);
                            });
                        }
                    }
                    else
                    {
                        ControllerException excepcion =
                        new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA,
                       OUEnumTipoErrorOU.EX_GUIA_NO_PERTENECE_PUNTO.ToString(),
                        OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_GUIA_NO_PERTENECE_PUNTO));
                        throw new FaultException<ControllerException>(excepcion);
                    }

                }
                else
                {
                    guia.IdIngresoGuia = OURepositorio.Instancia.GuardarIngresoGuiaNoAgencia(guia);
                    if (listaNovedades != null && listaNovedades.Any())
                    {
                        listaNovedades.ForEach(nov =>
                        {
                            if (nov.Asignado)
                                OURepositorio.Instancia.GuardarNovedadGuiaNoIngresada(nov, guia.IdIngresoGuia);
                        });
                    }
                }

                scope.Complete();
                return guia;
            }
        }

        #endregion
    }
}