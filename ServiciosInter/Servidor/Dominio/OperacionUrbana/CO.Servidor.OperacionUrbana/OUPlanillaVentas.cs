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
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using System.ServiceModel;
using CO.Servidor.OperacionUrbana.Comun;
using CO.Servidor.Dominio.Comun.AdmEstadosGuia;
using System;
using System.Linq;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.AdmEstadosConsolidado;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;

namespace CO.Servidor.OperacionUrbana
{
    internal class OUPlanillaVentas : ControllerBase
    {
        private static readonly OUPlanillaVentas instancia = (OUPlanillaVentas)FabricaInterceptores.GetProxy(new OUPlanillaVentas(), COConstantesModulos.MODULO_OPERACION_URBANA);

        /// <summary>
        /// Retorna una instancia de OUManejadorIngreso
        /// /// </summary>
        public static OUPlanillaVentas Instancia
        {
            get { return OUPlanillaVentas.instancia; }
        }


        //private IPUFachadaCentroServicios fachadaCentroServicios = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();

        /// <summary>
        /// Consultar planillas del mensajero teniendo en cuenta las horas del día
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <param name="idCentroServicios">Id del centro de servicios</param>
        /// <returns>Lista con las planillas del punto</returns>
        public List<OUPlanillaVentaDC> ObtenerPlanillasPorCentroServicios(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long idCentroServicios)
        {
            return OURepositorio.Instancia.ObtenerPlanillasPorCentroServicios(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idCentroServicios);
        }
        /// <summary>
        /// Consulta si un punto de servicio tiene almenos una planilla de recoleccion en punto (planilla ventas)  abierta
        /// </summary>
        /// <returns></returns>
        public bool ValidarPlanillasAbiertasPorPuntoVenta(long idPuntoServicio)
        {
            return OURepositorio.Instancia.ValidarPlanillasAbiertasPorPuntoVenta(idPuntoServicio);
        }

        /// <summary>
        /// Elimina una guia o un rotulo de un consolidado en la planilla de ventas
        /// </summary>
        /// <param name="idAdmisionMensajeria"></param>
        /// <param name="idPlanilla"></param>
        public OUPlanillaVentaGuiasDC EliminarGuiaConsolidadoPlanillaVentas(OUPlanillaVentaGuiasDC guiaPlanilla)
        {
            ADGuia guia = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>().ObtenerGuiaXNumeroGuia(guiaPlanilla.NumeroGuia);

            using (TransactionScope transaccion = new TransactionScope())
            {
                bool cambiarEstadoGuia = false;
                if (guiaPlanilla.TotalPiezasRotulo > 0)
                {
                    ///Solo saca el rotulo del consolidado, por tanto queda como un envio suelto
                    OURepositorio.Instancia.EliminarRotuloDeConsolidadoPlanillaVentas(guia.IdAdmision, guiaPlanilla.IdPlanilla, (short)guiaPlanilla.PiezaActualRotulo);
                }
                else
                {
                    OURepositorio.Instancia.EliminarGuiaConsolidadoPlanillaVentas(guia.IdAdmision, guiaPlanilla.IdPlanilla);
                    cambiarEstadoGuia = true;
                }


                if (cambiarEstadoGuia)
                {
                    ///Cambia estado de guia
                    ADTrazaGuia trazaGuia = new ADTrazaGuia()
                    {
                        IdAdmision = guia.IdAdmision,
                        NumeroGuia = guia.NumeroGuia,
                        Observaciones = string.Empty,
                        IdCiudad = guia.IdCiudadOrigen,
                        Ciudad = guia.NombreCiudadOrigen,
                        Modulo = COConstantesModulos.MODULO_OPERACION_URBANA,
                        FechaGrabacion = DateTime.Now,
                        IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.Admitida
                    };
                    if (EstadosGuia.InsertaEstadoGuia(trazaGuia) == 0)
                    {
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, OUEnumTipoErrorOU.EX_GUIA_NO_ESTA_REGISTRADA.ToString(), OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_CAMBIO_ESTADO_NO_VALIDO)));
                    }
                }

                transaccion.Complete();
            }
            return guiaPlanilla;
        }
        /// <summary>
        /// Elimina una guia o un rotulo de una planilla de ventas
        /// </summary>
        /// <param name="idAdmisionMensajeria"></param>
        /// <param name="idPlanilla"></param>
        public OUPlanillaVentaGuiasDC EliminarGuiaPlanillaVentas(OUPlanillaVentaGuiasDC guiaPlanilla)
        {
            ADGuia guia = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>().ObtenerGuiaXNumeroGuia(guiaPlanilla.NumeroGuia);

            using (TransactionScope transaccion = new TransactionScope())
            {
                bool cambiarEstadoGuia = false;
                if (guiaPlanilla.TotalPiezasRotulo > 0)
                {
                    OURepositorio.Instancia.EliminarRotuloDePlanillaVentas(guia.IdAdmision, guiaPlanilla.IdPlanilla, (short)guiaPlanilla.PiezaActualRotulo);
                    if (guiaPlanilla.TotalPiezasRotulo == 1)
                    {
                        OURepositorio.Instancia.EliminarGuiaPlanillaVentas(guia.IdAdmision, guiaPlanilla.IdPlanilla);
                        cambiarEstadoGuia = true;
                    }
                    else
                        //Valida si es el ultimo rotulo, si es así elimina la guia de la planilla
                        if (!OURepositorio.Instancia.ValidarExistenciaRotulosEnPlanillaVentas(guiaPlanilla.IdPlanilla, guia.IdAdmision))
                        {
                            OURepositorio.Instancia.EliminarGuiaPlanillaVentas(guia.IdAdmision, guiaPlanilla.IdPlanilla);
                            cambiarEstadoGuia = true;
                        }
                }
                else
                {
                    OURepositorio.Instancia.EliminarGuiaPlanillaVentas(guia.IdAdmision, guiaPlanilla.IdPlanilla);
                    cambiarEstadoGuia = true;
                }

                if (cambiarEstadoGuia)
                {
                    ///Cambia estado de guia
                    ADTrazaGuia trazaGuia = new ADTrazaGuia()
                    {
                        IdAdmision = guia.IdAdmision,
                        NumeroGuia = guia.NumeroGuia,
                        Observaciones = string.Empty,
                        IdCiudad = guia.IdCiudadOrigen,
                        Ciudad = guia.NombreCiudadOrigen,
                        Modulo = COConstantesModulos.MODULO_OPERACION_URBANA,
                        FechaGrabacion = DateTime.Now,
                        IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.Admitida
                    };
                    if (EstadosGuia.InsertaEstadoGuia(trazaGuia) == 0)
                    {
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, OUEnumTipoErrorOU.EX_GUIA_NO_ESTA_REGISTRADA.ToString(), OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_CAMBIO_ESTADO_NO_VALIDO)));
                    }
                }

                transaccion.Complete();

                ///TODO:CED pendiente Ronald en decirme como se hacer el cambio de estado al sacar la guia de la planilla            
                ///ahora, si es una pieza de un rotulo la que sacan, como se maneja ese estado?
                ///[05:04:35 p.m.] christian-velandia: si quedan planilladas, 2 piezas del rotulo, y una sin planillar
                ///[05:04:42 p.m.] christian-velandia: que pasa con esa que se quedo?
                ///[05:05:39 p.m.] christian-velandia: se vuelve a cambiar de estado?
                ///[05:05:48 p.m.] christian-velandia: porque las piezas tienen la misma guia
                ///y si esa pieza se agrega a otra planilla, que pasa con la guia?

            }
            return guiaPlanilla;
        }

        /// <summary>
        /// Obtiene las asignaciones para imprimir y cierra la planilla
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <param name="idCentroServicios"></param>
        /// <returns></returns>
        public List<OUImpresionPlanillaVentasDC> ObtenerCerrarImpresionPlanillaVentasTotal(OUPlanillaVentaDC planilla, long idCentroServicios)
        {
            List<OUImpresionPlanillaVentasDC> Impresion = new List<OUImpresionPlanillaVentasDC>();

            if (planilla.NumeroPlanilla == 0)
                planilla.NumeroPlanilla = OURepositorio.Instancia.GuardarPlanillaVenta(planilla);


            var agenciaResponsable = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerAgenciaResponsable(idCentroServicios);

            string nombreCentroServicioOrigen = agenciaResponsable.NombreCentroServicio;
            string nombreCentroServicioDestino = agenciaResponsable.NombreResponsable;
            string EmpresaTransportadora = "Interrapidisimo";
            ///obtiene todas las asignaciones creadas del punto
            List<OUAsignacionDC> lstAsignacion = OURepositorio.Instancia.ObtenerAsignacionesPlanillaImprimir(planilla.NumeroPlanilla, OUConstantesOperacionUrbana.ESTADO_CREADA);

            long conTransRetorno = 0;
            using (TransactionScope transaccion = new TransactionScope())
            {
                if (lstAsignacion.Count() <= 0)
                {
                    conTransRetorno = Framework.Servidor.ParametrosFW.PAParametros.Instancia.ObtenerConsecutivo(Framework.Servidor.Servicios.ContratoDatos.Parametros.Consecutivos.PAEnumConsecutivos.Control_Transporte_Manifiesto_Urbano_Retorno);
                }

                lstAsignacion.ForEach(a =>
                    {
                        ///Asiga el NumConsTranRetorno, si no existe, se genera y luego se utiliza el mismo numero para todas las asignaciones
                        if ((a.NumContTransRetorno == null || a.NumContTransRetorno == 0) && conTransRetorno == 0)
                        {
                            conTransRetorno = Framework.Servidor.ParametrosFW.PAParametros.Instancia.ObtenerConsecutivo(Framework.Servidor.Servicios.ContratoDatos.Parametros.Consecutivos.PAEnumConsecutivos.Control_Transporte_Manifiesto_Urbano_Retorno);
                        }
                        else if (conTransRetorno == 0)
                        {
                            conTransRetorno = a.NumContTransRetorno.Value;
                        }
                        OURepositorio.Instancia.ActualizarConTransRetornoPlanillaVentas(a.TotalPiezas, planilla.NumeroPlanilla, a.IdAdmisionMensajeria, conTransRetorno, a.IdAsignacion);
                        //cambiar estado de AsignacionTulaPuntoServicio
                        OURepositorio.Instancia.CambiarEstadoAsignacionTulaPunto(a.IdAsignacion, OUConstantesOperacionUrbana.ESTADO_ASIGNADA);



                        ///Agrega a la lista de impresion
                        ///
                        OUImpresionPlanillaVentasDC imp = Impresion.Where(i => i.NumConTransReotorno == conTransRetorno).FirstOrDefault();

                        if (imp != null)
                        {

                            if (imp.Asignaciones == null)
                                imp.Asignaciones = new List<OUAsignacionDC>();
                            if (imp.Planilla == null)
                                imp.Planilla = new OUPlanillaVentaDC();

                            imp.Planilla = planilla;
                            imp.NombreCentroServiciosDestino = nombreCentroServicioDestino;
                            imp.NombreCentroServiciosOrigen = nombreCentroServicioOrigen;
                            imp.NombreEmpresaTransportadora = EmpresaTransportadora;
                            imp.Asignaciones.Add(a);
                        }
                        else
                        {
                            imp = new OUImpresionPlanillaVentasDC();
                            imp.NumConTransReotorno = conTransRetorno;
                            if (imp.Asignaciones == null)
                                imp.Asignaciones = new List<OUAsignacionDC>();
                            if (imp.Planilla == null)
                                imp.Planilla = new OUPlanillaVentaDC();

                            imp.NombreCentroServiciosDestino = nombreCentroServicioDestino;
                            imp.NombreCentroServiciosOrigen = nombreCentroServicioOrigen;
                            imp.NombreEmpresaTransportadora = EmpresaTransportadora;
                            imp.Planilla = planilla;

                            a.TotalPiezas = OURepositorio.Instancia.ObtenerCantidadEnviosEnConsolidadoPorPlanillaNumConTransRetorno(planilla.NumeroPlanilla, conTransRetorno, a.IdAsignacion);
                            imp.Asignaciones.Add(a);




                            Impresion.Add(imp);
                        }
                    });


                //Buscar y adicionar a la planilla como envios sueltos todos los envios en estado admitido, no planillados y admitidos en en el punto de servicio                      
                List<OUPlanillaVentaGuiasDC> lstPendientesPlanilla = OURepositorio.Instancia.ObtenerEnviosEstadoAdmitidoNoPlanilladosPuntoServicio(idCentroServicios);
                lstPendientesPlanilla.ForEach(p =>
                    {
                        p.IdAsignacionTula = -1;

                        p.IdPlanilla = planilla.NumeroPlanilla;
                        p.NumContTransRetorno = conTransRetorno;

                        //grabar guias dentro de la planilla      
                        ///Valida que la guia no haya sido agregada a la planilla
                        if (OURepositorio.Instancia.ValidarGuiaIngresadaPlanillaAsignacionTula(p.IdPlanilla, p.NumeroGuia))
                        {
                            if (p.TotalPiezasRotulo > 0)
                            {
                                if (!OURepositorio.Instancia.ValidarRotuloIngresadoPlanilla(p.IdPlanilla, p.IdAdmision, (short)p.PiezaActualRotulo))
                                {
                                    OURepositorio.Instancia.GuardarRotuloGuiaPlanillaVentas(p);
                                }
                            }
                        }
                        else
                        {
                            OURepositorio.Instancia.GuardarPlanillaVentaGuia(p, null);
                            if (p.TotalPiezasRotulo > 0)
                            {
                                OURepositorio.Instancia.GuardarRotuloGuiaPlanillaVentas(p);
                            }
                        }


                        ///Cambia estado de guia
                        ADTrazaGuia trazaGuia = new ADTrazaGuia()
                        {
                            IdAdmision = p.IdAdmision,
                            NumeroGuia = p.NumeroGuia,
                            Observaciones = string.Empty,
                            IdCiudad = p.IdCiudadOrigen,
                            Ciudad = p.NombreCiudadOrigen,
                            Modulo = COConstantesModulos.MODULO_OPERACION_URBANA,
                            FechaGrabacion = DateTime.Now,
                            IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.TransitoUrbano
                        };
                        if (EstadosGuia.InsertaEstadoGuia(trazaGuia) == 0)
                        {
                            throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, OUEnumTipoErrorOU.EX_GUIA_NO_ESTA_REGISTRADA.ToString(), OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_CAMBIO_ESTADO_NO_VALIDO)));
                        }

                    });



                planilla.TotalEnviosPlanillados = (short)OURepositorio.Instancia.ObtenerCantidadEnviosPorPlanillaNumConTransRetorno(planilla.NumeroPlanilla, conTransRetorno);
                planilla.TotalEnviosSueltosPlanillados = OURepositorio.Instancia.ObtenerCantidadEnviosSueltosPorPlanillaNumConTransRetorno(planilla.NumeroPlanilla, conTransRetorno);


                OUImpresionPlanillaVentasDC imp2 = new OUImpresionPlanillaVentasDC()
                {
                    Asignaciones = new List<OUAsignacionDC>(),
                    NombreCentroServiciosDestino = nombreCentroServicioDestino,
                    NombreCentroServiciosOrigen = nombreCentroServicioOrigen,
                    NombreEmpresaTransportadora = EmpresaTransportadora,
                    Planilla = planilla,
                    NumConTransReotorno = conTransRetorno
                };


                OURepositorio.Instancia.ActualizarTotalEnviosPlanilladosPorPlanillaVentas(planilla.NumeroPlanilla, planilla.TotalEnviosPlanillados);
                OURepositorio.Instancia.CerrarPlanillaVentas(planilla.NumeroPlanilla);

                Impresion.Add(imp2);
                transaccion.Complete();
            };
            return Impresion;
        }



        /// <summary>
        /// Obtiene las asignaciones para imprimir y cierra la planilla parcial
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <param name="idCentroServicios"></param>
        /// <returns></returns>
        public List<OUImpresionPlanillaVentasDC> ObtenerCerrarImpresionPlanillaVentasParcial(long idPlanilla, long idCentroServicios)
        {
            List<OUImpresionPlanillaVentasDC> Impresion = new List<OUImpresionPlanillaVentasDC>();

            OUPlanillaVentaDC planilla = OURepositorio.Instancia.ObtenerPlanillaVentasPorIdPlanilla(idPlanilla);
            var agenciaResponsable = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerAgenciaResponsable(idCentroServicios);

            string nombreCentroServicioOrigen = agenciaResponsable.NombreCentroServicio;
            string nombreCentroServicioDestino = agenciaResponsable.NombreResponsable;
            string EmpresaTransportadora = "Interrapidisimo";
            ///obtiene todas las asignaciones creadas del punto

            long conTransRetorno = 0;
            using (TransactionScope transaccion = new TransactionScope())
            {

                conTransRetorno = Framework.Servidor.ParametrosFW.PAParametros.Instancia.ObtenerConsecutivo(Framework.Servidor.Servicios.ContratoDatos.Parametros.Consecutivos.PAEnumConsecutivos.Control_Transporte_Manifiesto_Urbano_Retorno);


                //Asocia los envios sueltos de la planilla con el numContransRetorno
                List<OUGuiaIngresadaDC> sueltas = OURepositorio.Instancia.ObtenerGuiasSueltasPlanilladas(idPlanilla);
                sueltas.ForEach(a =>
                {
                    OURepositorio.Instancia.ActualizarConTransRetornoPlanillaVentasSueltos(idPlanilla, a.IdAdmision, conTransRetorno);

                    ///Cambia estado de guia
                    ADTrazaGuia trazaGuia = new ADTrazaGuia()
                    {
                        IdAdmision = a.IdAdmision,
                        NumeroGuia = a.NumeroGuia,
                        Observaciones = string.Empty,
                        IdCiudad = a.IdCiudad,
                        Ciudad = a.Ciudad,
                        Modulo = COConstantesModulos.MODULO_OPERACION_URBANA,
                        FechaGrabacion = DateTime.Now,
                        IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.TransitoUrbano
                    };
                    if (EstadosGuia.InsertaEstadoGuia(trazaGuia) == 0)
                    {
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, OUEnumTipoErrorOU.EX_GUIA_NO_ESTA_REGISTRADA.ToString(), OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_CAMBIO_ESTADO_NO_VALIDO)));
                    }
                });


                planilla.TotalEnviosPlanillados = (short)OURepositorio.Instancia.ObtenerCantidadEnviosPorPlanillaNumConTransRetorno(idPlanilla, conTransRetorno);
                planilla.TotalEnviosSueltosPlanillados = OURepositorio.Instancia.ObtenerCantidadEnviosSueltosPorPlanillaNumConTransRetorno(idPlanilla, conTransRetorno);


                OUImpresionPlanillaVentasDC imp2 = new OUImpresionPlanillaVentasDC()
                {
                    Asignaciones = new List<OUAsignacionDC>(),
                    NombreCentroServiciosDestino = nombreCentroServicioDestino,
                    NombreCentroServiciosOrigen = nombreCentroServicioOrigen,
                    NombreEmpresaTransportadora = EmpresaTransportadora,
                    Planilla = planilla,
                    NumConTransReotorno = conTransRetorno
                };


                OURepositorio.Instancia.ActualizarTotalEnviosPlanilladosPorPlanillaVentas(idPlanilla, planilla.TotalEnviosPlanillados);
                OURepositorio.Instancia.CerrarPlanillaVentas(idPlanilla);

                Impresion.Add(imp2);
                transaccion.Complete();
            };

            return Impresion;
        }


        /// <summary>
        /// Obtiene las asignaciones para imprimir la planilla
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <param name="idCentroServicios"></param>
        /// <returns></returns>
        public OUImpresionPlanillaVentasDC ObtenerImpresionManifiestoPlanillaVentas(long idPlanilla, long idCentroServicios)
        {
            OUImpresionPlanillaVentasDC Impresion = new OUImpresionPlanillaVentasDC();

            OUPlanillaVentaDC planilla = OURepositorio.Instancia.ObtenerPlanillaVentasPorIdPlanilla(idPlanilla);
            var agenciaResponsable = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerAgenciaResponsable(idCentroServicios);

            Impresion.NombreCentroServiciosDestino = agenciaResponsable.NombreResponsable;
            Impresion.NombreCentroServiciosOrigen = agenciaResponsable.NombreCentroServicio;
            Impresion.NombreEmpresaTransportadora = "Interrapidisimo";
            Impresion.NombreCiudadDestino = agenciaResponsable.NombreCiudadResponsable;
            long conTransRetorno = 0;

            Impresion.Asignaciones = new List<OUAsignacionDC>();
            Impresion.GuiasRotulosSueltos = new List<string>();

            ///obtiene los envios de cada asignacion asociada a la planilla
            List<long> lstAsignacion = OURepositorio.Instancia.ObtenerIdAsignacionesUtilizadasPlanillaVentas(idPlanilla);
            lstAsignacion.ForEach(a =>
            {
                List<OUAsignacionDC> lstGuiasAsignacion = OURepositorio.Instancia.ObtenerGuiasAsociadasAsignacionPlanillaVentas(idPlanilla, a);
                lstGuiasAsignacion.ForEach(g =>
                     {
                         conTransRetorno = g.NumContTransRetorno != null ? g.NumContTransRetorno.Value : 0;
                         g.IdAsignacion = a;
                         Impresion.Asignaciones.Add(g);

                     });

            });

            List<OUAsignacionDC> lstGuiasSueltas = OURepositorio.Instancia.ObtenerGuiasSueltasAsociadasPlanillaVentas(idPlanilla);
            lstGuiasSueltas.ForEach(g =>
            {
                conTransRetorno = g.NumContTransRetorno != null ? g.NumContTransRetorno.Value : 0;
                Impresion.GuiasRotulosSueltos.Add(g.NumeroGuiaRotulo);

            });



            planilla.TotalEnviosPlanillados = (short)OURepositorio.Instancia.ObtenerCantidadEnviosPorPlanillaNumConTransRetorno(idPlanilla, conTransRetorno);
            planilla.TotalEnviosSueltosPlanillados = OURepositorio.Instancia.ObtenerCantidadEnviosSueltosPorPlanillaNumConTransRetorno(idPlanilla, conTransRetorno);
            planilla.TotalConsolidadosPlanillados = lstAsignacion.Count();
            Impresion.Planilla = planilla;


            return Impresion;
        }

        public List<OUImpresionPlanillaVentasDC> ObtenerImpresionPlanillaVentasSinCerrar(long idPlanilla, long idCentroServicios)
        {
            throw new NotImplementedException("falta CED");
        }



        /// <summary>
        /// Adiciona una guia a una planilla, si la planilla no existe crea una nueva y retorna el numero de la planilla
        /// </summary>
        /// <param name="planilla"></param>
        /// <param name="guiaPlanilla"></param>
        /// <returns></returns>
        public long AdicionarGuiaPlanilla(OUPlanillaVentaDC planilla, OUPlanillaVentaGuiasDC guiaPlanilla)
        {
            long idPlanilla = planilla.NumeroPlanilla;

            using (TransactionScope transaccion = new TransactionScope())
            {
                if (planilla.NumeroPlanilla <= 0)
                {
                    idPlanilla = OURepositorio.Instancia.GuardarPlanillaVenta(planilla);
                    planilla.NumeroPlanilla = idPlanilla;
                }

                //Cambiar el estado de la asignacion
                if (guiaPlanilla.IdAsignacionTula != null)
                {
                    OURepositorio.Instancia.CambiarEstadoAdicionarAsignacion(guiaPlanilla.IdAsignacionTula.Value, OUConstantesOperacionUrbana.ESTADO_INGRESADO);
                    ECAdminEstadosConsolidado.GuardarEstadoConsolidado(new ECEstadoConsolidado { NoTula = guiaPlanilla.NumeroTulaContenedor, Estado = EnumEstadosConsolidados.INU, IdCentroServicios = planilla.IdPuntoServicio, Observaciones = string.Empty });

                }
                ADGuia guia = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>().ObtenerGuiaXNumeroGuia(guiaPlanilla.NumeroGuia);

                if (guia != null)
                {
                    ///Valida que la guia haya sido admitida por el punto que esta planillando
                    if (guia.IdCentroServicioOrigen != planilla.IdPuntoServicio)
                    {
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, OUEnumTipoErrorOU.EX_GUIA_NO_ESTA_REGISTRADA.ToString(), OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_GUIA_NO_PERTENECE_PUNTO)));
                    }

                    bool guiaRotuloIngresada = false;

                    ///Valida que la guia no haya sido agregada a la planilla
                    if (OURepositorio.Instancia.ValidarGuiaIngresadaPlanillaAsignacionTula(planilla.NumeroPlanilla, guiaPlanilla.NumeroGuia))
                    {
                        if (guiaPlanilla.TotalPiezasRotulo > 0)
                        {
                            if (OURepositorio.Instancia.ValidarRotuloIngresadoPlanilla(planilla.NumeroPlanilla, guia.IdAdmision, (short)guiaPlanilla.PiezaActualRotulo))
                            {
                                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, OUEnumTipoErrorOU.EX_PIEZA_ROTULO_YA_PLANILLADA.ToString(), OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_PIEZA_ROTULO_YA_PLANILLADA)));
                            }
                            else
                                guiaRotuloIngresada = true;

                        }
                        else
                            throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, OUEnumTipoErrorOU.EX_GUIA_YA_PLANILLADA_VENTAS.ToString(), OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_GUIA_YA_PLANILLADA_VENTAS)));
                    }

                    ADTrazaGuia trazaGuia = new ADTrazaGuia()
                    {
                        IdAdmision = guia.IdAdmision,
                        NumeroGuia = guia.NumeroGuia,
                        Observaciones = string.Empty,
                        IdCiudad = guia.IdCiudadOrigen,
                        Ciudad = guia.NombreCiudadOrigen,
                        IdEstadoGuia = (short)(EstadosGuia.ObtenerUltimoEstado(guia.IdAdmision)),
                        Modulo = COConstantesModulos.MODULO_OPERACION_URBANA,
                        FechaGrabacion = DateTime.Now,
                        IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.TransitoUrbano,
                        NombreCentroServicioEstado = ControllerContext.Current.NombreCentroServicio
                    };
                    if (EstadosGuia.ValidarInsertarEstadoGuia(trazaGuia) == 0)
                    {
                        string descripcionEstado = ", Estado actual " + ((ADEnumEstadoGuia)(trazaGuia.IdEstadoGuia)).ToString();

                        //no pudo realizar el cambio de estado
                        ControllerException excepcion =
                                      new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, OUEnumTipoErrorOU.EX_CAMBIO_ESTADO_NO_VALIDO.ToString(),
                                      OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_CAMBIO_ESTADO_NO_VALIDO));
                        excepcion.Mensaje = excepcion.Mensaje + descripcionEstado;
                        throw new FaultException<ControllerException>(excepcion);
                    }
                    //}          

                    //logica de grabacion cuando es un rotulo

                    guiaPlanilla.IdAdmision = guia.IdAdmision;
                    guiaPlanilla.IdUnidadNegocio = guia.IdUnidadNegocio;
                    guiaPlanilla.IdServicio = guia.IdServicio;
                    guiaPlanilla.NombreServicio = guia.NombreServicio;
                    guiaPlanilla.DiceContener = guia.DiceContener;
                    guiaPlanilla.Peso = guia.Peso;
                    guiaPlanilla.EsRecomendado = guia.EsRecomendado;
                    guiaPlanilla.IdPlanilla = planilla.NumeroPlanilla;
                    guiaPlanilla.IdCiudadOrigenGuia = guia.IdCiudadOrigen;

                    if (!guiaRotuloIngresada)
                    {
                        //grabar guias dentro de la planilla            
                        OURepositorio.Instancia.GuardarPlanillaVentaGuia(guiaPlanilla, guiaPlanilla.IdAsignacionTula);
                    }

                    if (guiaPlanilla.TotalPiezasRotulo > 0)
                    {
                        OURepositorio.Instancia.GuardarRotuloGuiaPlanillaVentas(guiaPlanilla);
                    }
                }
                else
                    throw new FaultException<ControllerException>
                        (new ControllerException
                            (COConstantesModulos.MODULO_OPERACION_URBANA,
                            OUEnumTipoErrorOU.EX_GUIA_NO_ESTA_REGISTRADA.ToString(),
                            OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_GUIA_NO_ESTA_REGISTRADA)));



                transaccion.Complete();
            }

            return idPlanilla;
        }


        public long CrearPlanillaVentas(OUPlanillaVentaDC planilla, List<OUPlanillaVentaGuiasDC> LstGuias)
        {
            long idPlanilla = 0;

            using (TransactionScope transaccion = new TransactionScope())
            {
                idPlanilla = OURepositorio.Instancia.GuardarPlanillaVenta(planilla);
                planilla.NumeroPlanilla = idPlanilla;


                foreach (OUPlanillaVentaGuiasDC guiaPlanilla in LstGuias)
                {
                    ADGuia guia = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>().ObtenerGuiaXNumeroGuia(guiaPlanilla.NumeroGuia);

                    ADTrazaGuia trazaGuia = new ADTrazaGuia()
                    {
                        IdAdmision = guia.IdAdmision,
                        NumeroGuia = guia.NumeroGuia,
                        Observaciones = string.Empty,
                        IdCiudad = guia.IdCiudadOrigen,
                        Ciudad = guia.NombreCiudadOrigen,
                        IdEstadoGuia = (short)(EstadosGuia.ObtenerUltimoEstado(guia.IdAdmision)),
                        Modulo = COConstantesModulos.MODULO_OPERACION_URBANA,
                        FechaGrabacion = DateTime.Now,
                        IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.TransitoUrbano,
                        NombreCentroServicioEstado = ControllerContext.Current.NombreCentroServicio
                    };
                    if (EstadosGuia.ValidarInsertarEstadoGuia(trazaGuia) == 0)
                    {
                        string descripcionEstado = ", Estado actual " + ((ADEnumEstadoGuia)(trazaGuia.IdEstadoGuia)).ToString();

                        //no pudo realizar el cambio de estado
                        ControllerException excepcion = new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, OUEnumTipoErrorOU.EX_CAMBIO_ESTADO_NO_VALIDO.ToString()
                                                                            , OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_CAMBIO_ESTADO_NO_VALIDO));
                        excepcion.Mensaje = excepcion.Mensaje + descripcionEstado;
                        throw new FaultException<ControllerException>(excepcion);
                    }


                    guiaPlanilla.IdAdmision = guia.IdAdmision;
                    guiaPlanilla.IdPuntoServicio = guia.IdCentroServicioOrigen;
                    guiaPlanilla.IdUnidadNegocio = guia.IdUnidadNegocio;
                    guiaPlanilla.IdServicio = guia.IdServicio;
                    guiaPlanilla.NombreServicio = guia.NombreServicio;
                    guiaPlanilla.DiceContener = guia.DiceContener;
                    guiaPlanilla.Peso = guia.Peso;
                    guiaPlanilla.EsRecomendado = guia.EsRecomendado;
                    guiaPlanilla.IdPlanilla = planilla.NumeroPlanilla;
                    guiaPlanilla.IdCiudadOrigenGuia = guia.IdCiudadOrigen;


                    //Graba guia dentro de la planilla            
                    OURepositorio.Instancia.GuardarPlanillaVentaGuia(guiaPlanilla, guiaPlanilla.IdAsignacionTula);


                    // Movimiento de bodega - Salida del Punto
                    PUMovimientoInventario movInventario = new PUMovimientoInventario();
                    //movInventario.TipoMovimiento = PUEnumTipoMovimientoInventario.Salida;
                    //movInventario.IdCentroServicioOrigen = ControllerContext.Current.IdCentroServicio;
                    //movInventario.Bodega = new PUCentroServiciosDC() { IdCentroServicio = guia.IdCentroServicioOrigen };
                    //movInventario.NumeroGuia = guia.NumeroGuia;
                    //movInventario.FechaGrabacion = DateTime.Now;
                    //movInventario.FechaEstimadaIngreso = DateTime.Now;
                    //movInventario.CreadoPor = ControllerContext.Current.Usuario;
                    //COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().AdicionarMovimientoInventario(movInventario);

                    // Movimiento de bodega - Asignada el COL correspondiente
                    PUCentroServiciosDC COLpuntoOrigen = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerCentroLogisticoApoyaMunicipio(guia.IdCiudadOrigen);
                    movInventario = new PUMovimientoInventario();
                    movInventario.TipoMovimiento = PUEnumTipoMovimientoInventario.Asignacion;
                    movInventario.IdCentroServicioOrigen = ControllerContext.Current.IdCentroServicio;
                    movInventario.Bodega = new PUCentroServiciosDC() { IdCentroServicio = COLpuntoOrigen.IdCentroServicio };
                    movInventario.NumeroGuia = guia.NumeroGuia;
                    movInventario.FechaGrabacion = DateTime.Now;
                    movInventario.FechaEstimadaIngreso = DateTime.Now;
                    movInventario.CreadoPor = ControllerContext.Current.Usuario;
                    COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().AdicionarMovimientoInventario(movInventario);

                }

                transaccion.Complete();
            }


            //{
            //    ControllerException excepcion = new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, "aaaa", exc.Message);
            //    throw new FaultException<ControllerException>(excepcion);
            //}


            return idPlanilla;
        }



        /// <summary>
        /// Adiciona una guia suelta a una planilla, si la planilla no existe crea una nueva y retorna el numero de la planilla
        /// </summary>
        /// <param name="planilla"></param>
        /// <param name="guiaPlanilla"></param>
        /// <returns></returns>
        public long AdicionarGuiaSueltaPlanilla(OUPlanillaVentaDC planilla, OUPlanillaVentaGuiasDC guiaPlanilla)
        {
            long idPlanilla = planilla.NumeroPlanilla;

            using (TransactionScope transaccion = new TransactionScope())
            {
                if (planilla.NumeroPlanilla <= 0)
                {
                    idPlanilla = OURepositorio.Instancia.GuardarPlanillaVenta(planilla);
                    planilla.NumeroPlanilla = idPlanilla;
                }

                ADGuia guia = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>().ObtenerGuiaXNumeroGuia(guiaPlanilla.NumeroGuia);


                ///Valida que la guia haya sido admitida por el punto que esta planillando
                if (guia.IdCentroServicioOrigen != planilla.IdPuntoServicio)
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, OUEnumTipoErrorOU.EX_GUIA_NO_ESTA_REGISTRADA.ToString(), OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_GUIA_NO_PERTENECE_PUNTO)));
                }

                bool guiaRotuloIngresada = false;

                ///Valida que la guia no haya sido agregada a la planilla
                if (OURepositorio.Instancia.ValidarGuiaIngresadaPlanillaAsignacionTula(planilla.NumeroPlanilla, guiaPlanilla.NumeroGuia))
                {
                    if (OURepositorio.Instancia.ValidarRotuloIngresadoPlanilla(planilla.NumeroPlanilla, guia.IdAdmision, (short)guiaPlanilla.PiezaActualRotulo))
                    {
                        throw new FaultException<ControllerException>
                            (new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA,
                                OUEnumTipoErrorOU.EX_PIEZA_ROTULO_YA_PLANILLADA.ToString(),
                                OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_PIEZA_ROTULO_YA_PLANILLADA)));
                    }
                    else
                        guiaRotuloIngresada = true;

                }

                ///Cambia estado de guia
                ADTrazaGuia trazaGuia = new ADTrazaGuia()
                {
                    IdAdmision = guia.IdAdmision,
                    NumeroGuia = guia.NumeroGuia,
                    Observaciones = string.Empty,
                    IdCiudad = guia.IdCiudadOrigen,
                    Ciudad = guia.NombreCiudadOrigen,
                    IdEstadoGuia = (short)(EstadosGuia.ObtenerUltimoEstado(guia.IdAdmision)),
                    Modulo = COConstantesModulos.MODULO_OPERACION_URBANA,
                    FechaGrabacion = DateTime.Now,
                    IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.TransitoUrbano,
                    NumeroPieza = guiaPlanilla.PiezaActualRotulo,
                    TotalPiezas = guiaPlanilla.TotalPiezasRotulo,
                };

                if (EstadosGuia.ValidarInsertarEstadoGuia(trazaGuia) == 0)
                {
                    string descripcionEstado = ", Estado actual " + ((ADEnumEstadoGuia)(trazaGuia.IdEstadoGuia)).ToString();

                    //no pudo realizar el cambio de estado
                    ControllerException excepcion =
                                  new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, OUEnumTipoErrorOU.EX_CAMBIO_ESTADO_NO_VALIDO.ToString(),
                                  OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_CAMBIO_ESTADO_NO_VALIDO));
                    excepcion.Mensaje = excepcion.Mensaje + descripcionEstado;
                    throw new FaultException<ControllerException>(excepcion);
                }


                //logica de grabacion cuando es un rotulo

                guiaPlanilla.IdAdmision = guia.IdAdmision;
                guiaPlanilla.IdUnidadNegocio = guia.IdUnidadNegocio;
                guiaPlanilla.IdServicio = guia.IdServicio;
                guiaPlanilla.NombreServicio = guia.NombreServicio;
                guiaPlanilla.DiceContener = guia.DiceContener;
                guiaPlanilla.Peso = guia.Peso;
                guiaPlanilla.EsRecomendado = guia.EsRecomendado;
                guiaPlanilla.IdPlanilla = planilla.NumeroPlanilla;
                guiaPlanilla.IdCiudadOrigenGuia = guia.IdCiudadOrigen;
                guiaPlanilla.IdAsignacionTula = -1;
                long? idAsignacionTula = null;

                if (!guiaRotuloIngresada)
                {
                    //grabar guias dentro de la planilla            
                    OURepositorio.Instancia.GuardarPlanillaVentaGuia(guiaPlanilla, idAsignacionTula);
                }

                if (guiaPlanilla.TotalPiezasRotulo > 0)
                {
                    OURepositorio.Instancia.GuardarRotuloGuiaPlanillaVentas(guiaPlanilla);
                }


                transaccion.Complete();
            }

            return idPlanilla;
        }

        /// <summary>
        /// Obtiene las guias asignadas a una planilla y a una asignacion tula
        /// </summary>
        /// <param name="idPlanillaVentas"> id de la planilla de ventas</param>
        /// <param name="idAsignacionTula">Id de la asignacion tula</param>
        /// <returns>Lista con las guias asignadas</returns>
        public List<OUGuiaIngresadaDC> ObtenerGuiasPorPlanillaAsignacionTula(long idPlanillaVentas, long idAsignacionTula)
        {
            return OURepositorio.Instancia.ObtenerGuiasPorPlanillaAsignacionTula(idPlanillaVentas, idAsignacionTula);

        }
        /// <summary>
        /// Obtiene todas las guias sueltas planilladas
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <returns></returns>
        public List<OUGuiaIngresadaDC> ObtenerGuiasSueltasPlanilladas(long idPlanilla)
        {
            return OURepositorio.Instancia.ObtenerGuiasSueltasPlanilladas(idPlanilla);
        }


        /// <summary>
        /// Obtiene una lista de las asignaciones de tulas y precintos por punto de servicio,  y por estado
        /// </summary>
        /// <param name="idPuntoServicio"></param>
        /// <returns></returns>
        public List<OUAsignacionDC> ObtenerAsignacionTulaPrecintoPuntoServicio(long idPuntoServicio, string estadoAsignacion)
        {
            return OURepositorio.Instancia.ObtenerAsignacionTulaPrecintoPuntoServicio(idPuntoServicio, estadoAsignacion);
        }

        /// <summary>
        /// Obtiene las guias del punto de servicios
        /// </summary>
        /// <param name="idCentroServicios">Id del centro o punto de servicios</param>
        /// <returns>Lista con las guias del punto sin planillar</returns>
        public List<OUPlanillaVentaGuiasDC> ObtenerGuiasPorPuntoDeServicios(long idCentroServicios)
        {
            return OURepositorio.Instancia.ObtenerGuiasPorPuntoDeServicios(idCentroServicios);
        }

        /// <summary>
        /// Guarda la planilla de venta y las guias de la planilla
        /// </summary>
        /// <param name="planilla">Informacion de la planilla</param>
        /// <param name="guiasPlanilla">Lista con las Guias de la planilla</param>
        public long GuardarPlanillaVentas(OUPlanillaVentaDC planilla, List<OUPlanillaVentaGuiasDC> guiasPlanilla)
        {
            long retorno = 0;
            using (TransactionScope transaccion = new TransactionScope())
            {

                if (!string.IsNullOrWhiteSpace(planilla.BolsaSeguridad))
                    COFabricaDominio.Instancia.CrearInstancia<ISUFachadaSuministros>().GuardarConsumoBolsaSeguridad(planilla.BolsaSeguridad, 0, planilla.IdMensajero);

                retorno = OURepositorio.Instancia.GuardarPlanillaVenta(planilla, guiasPlanilla);
                transaccion.Complete();

            }
            return retorno;
        }

        /// <summary>
        /// Despacha la falla para las guias q no fueron planilladas por el punto de servicios
        /// </summary>
        /// <param name="idCentroServicios"></param>
        public void EnviaFallaConGuiasNoPlanilladas(long idCentroServicios, string nombreCentroServicios, string direccionCentroServicios)
        {
            List<OUPlanillaVentaGuiasDC> guias = new List<OUPlanillaVentaGuiasDC>();
            guias = OURepositorio.Instancia.ObtenerGuiasPorPuntoDeServicios(idCentroServicios);
            if (guias.Count > 0)
            {
                OUManejadorFallas.DespacharFallaPorGuiasNoPlanilladas(idCentroServicios, nombreCentroServicios, direccionCentroServicios, guias.Count, ControllerContext.Current.Usuario);
            }
        }

        /// <summary>
        /// Obtiene los mensajeros del racol
        /// </summary>
        /// <param name="centroLogistico"></param>
        /// <returns></returns>
        public IList<OUMensajeroDC> ObtenerMensajeroPorRegional(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long idRacol)
        {
            return OURepositorio.Instancia.ObtenerMensajeroPorRegional(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idRacol);
        }
    }
}