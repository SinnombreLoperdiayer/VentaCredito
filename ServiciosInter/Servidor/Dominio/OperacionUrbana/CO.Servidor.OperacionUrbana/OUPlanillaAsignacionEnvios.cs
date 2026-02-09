using CO.Servidor.Dominio.Comun.AdmEstadosGuia;
using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Dominio.Comun.Cajas;
using CO.Servidor.Dominio.Comun.CentroAcopio;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.OperacionUrbana.Comun;
using CO.Servidor.OperacionUrbana.Datos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Cajas;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Transactions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CO.Servidor.OperacionUrbana
{
    public class OUPlanillaAsignacionEnvios : ControllerBase
    {

        #region Singleton
        private static readonly OUPlanillaAsignacionEnvios instancia = (OUPlanillaAsignacionEnvios)FabricaInterceptores.GetProxy(new OUPlanillaAsignacionEnvios(), COConstantesModulos.MODULO_OPERACION_URBANA);

        /// <summary>
        /// Retorna una instancia de OUManejadorIngreso
        /// /// </summary>
        public static OUPlanillaAsignacionEnvios Instancia
        {
            get { return OUPlanillaAsignacionEnvios.instancia; }
        }

        private OUPlanillaAsignacionEnvios()
        {
        }
        #endregion


        private ICAFachadaCentroAcopio fachadaCentroAcopio = COFabricaDominio.Instancia.CrearInstancia<ICAFachadaCentroAcopio>();


        #region Consultas

        /// <summary>
        /// Retorna las planillas de asignacion del envios del centro logistico
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <param name="idCentroServicios"></param>
        /// <returns></returns>
        public List<OUPlanillaAsignacionDC> ObtenerPlanillasAsignacionCentroLogistico(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, long idCentroServicios, bool incluyeFecha)
        {
            return OURepositorio.Instancia.ObtenerPlanillasAsignacionCentroLogistico(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, idCentroServicios, incluyeFecha);
        }

        /// <summary>
        /// Retorna los mensajeros asociados al col y filtrados por tipo
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <param name="idCentroServicios">Id centro servicios</param>
        /// <param name="idTipoMensajero">Tipo de mensajero</param>
        /// <returns></returns>
        public List<OUMensajeroDC> ObtenerMensejorPorColYTipoMensajero(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long idCentroServicios, int idTipoMensajero)
        {
            return OURepositorio.Instancia.ObtenerMensejorPorColYTipoMensajero(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idCentroServicios, idTipoMensajero);
        }

        /// <summary>
        /// Retorna los envios de la planilla de asignacion enviada
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <returns></returns>
        public List<OUGuiaIngresadaDC> ObtenerEnviosPlanillaAsignacion(long idPlanilla)
        {
            return OURepositorio.Instancia.ObtenerEnviosPlanillaAsignacion(idPlanilla);
        }

        /// <summary>
        /// Obtiene las guias de determinado mensajero en determinado dia
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <param name="fechaPlanilla"></param>
        /// <returns>las guias de determinado mensajero con determinada fecha</returns>
        public List<OUGuiaIngresadaDC> ObtenerGuiasPlanilladasPorDiaYMensajero(long idMensajero, DateTime fechaPlanilla)
        {
            return OURepositorio.Instancia.ObtenerGuiasPlanilladasPorDiaYMensajero(idMensajero, fechaPlanilla);
        }

        /// <summary>
        /// Verifica el soat y la revision tecnomecanica del mensajero
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns>
        /// true = si el soat y la tecnomecanica estan vigentes
        /// false = si el soat y la tecnomecanica estan vencidos
        /// </returns>
        public bool VerificaMensajeroSoatTecnoMecanica(long idMensajero)
        {
            return OUManejadorIngreso.Instancia.VerificaMensajeroSoatTecnoMecanica(idMensajero);
        }

        /// <summary>
        /// Obtiene el total de los envios pendientes del mensajero
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public int ObtenerEnviosPendientesMensajero(long idMensajero)
        {
            return OURepositorio.Instancia.ObtenerEnviosPendientesMensajero(idMensajero);
        }

        /// <summary>
        /// Retorna los estados de la planilla
        /// </summary>
        /// <returns></returns>
        public IEnumerable<OUEstadosPlanillaAsignacionDC> ObtenerEstadosPlanillaAsignacion()
        {
            return OURepositorio.Instancia.ObtenerEstadosPlanillaAsignacion();
        }

        /// <summary>
        /// Indica si una guía, dado su número y el id de la agencia, ya ha sido ingresada a centro de acopio pero no había sido creada la guía como tal al momento del ingreso
        /// </summary>
        /// <param name="numeroGuia"></param>.
        /// <param name="idAgencia"></param>
        /// <returns>True si la guía ya fué ingresada</returns>
        public bool GuiaYaFueIngresadaACentroDeAcopio(long numeroGuia, long idAgencia)
        {
            return OUManejadorIngreso.Instancia.GuiaYaFueIngresadaACentroDeAcopio(numeroGuia, idAgencia);
        }

        /// <summary>
        /// Indica si una guía, dado su número ya ha sido ingresada a centro de acopio pero no habiá sido creada en el sistema
        /// </summary>
        /// <param name="numeroguia"></param>
        /// <returns>Retorna el número de la agencia uqe hizo el ingreso</returns>
        public long GuiaYaFueIngresadaACentroDeAcopioRetornaCentroAcopio(long numeroGuia)
        {
            return OUManejadorIngreso.Instancia.GuiaYaFueIngresadaACentroDeAcopioRetornaCentroAcopio(numeroGuia);
        }

        #endregion Consultas

        #region Inserción

        public OUPlanillaAsignacionDC GuardarPlanillaAsignacionEnvio(OUPlanillaAsignacionDC planilla)
        {
            using (TransactionScope scope = new TransactionScope())
            {  
                planilla.TotalGuias++;
                if (planilla.TotalGuias == 1 && planilla.IdPlanillaAsignacion <= 0)
                {
                    if (string.IsNullOrEmpty(planilla.IdEstadoPlanilla))
                    {
                        planilla.IdEstadoPlanilla = "1";
                    }
                    planilla.IdPlanillaAsignacion = OURepositorio.Instancia.GuardarPlanillaAsignacionEnvio(planilla);                     

                    if (planilla.Mensajero != null && planilla.Mensajero.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
                    {
                        AsignaMensajeroPlanilla(planilla);
                    }
                }

                if (planilla.IdPlanillaAsignacion > 0)
                {
                    //Se obtiene informacion de la guia
                    planilla.Guias = OURepositorio.Instancia.ConsultaGuiaParaPlanilla(planilla.Guias);

                    //Valida si el envio se encuentra registrado en sistema
                    if (!planilla.Guias.GuiaRegistrada)
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, OUEnumTipoErrorOU.EX_GUIA_NO_ESTA_REGISTRADA.ToString(), OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_GUIA_NO_ESTA_REGISTRADA)));

                    //Se obtiene estado guia 
                    ADTrazaGuia ultimoEstadoGuia = EstadosGuia.ObtenerTrazaUltimoEstadoGuia(planilla.Guias.IdAdmision);

                    //Validacion asignacion a mensajero (guias solo en estado centro de acopio)
                    if (!planilla.EsAuditoria && ultimoEstadoGuia.IdEstadoGuia != (short)ADEnumEstadoGuia.CentroAcopio)
                    {
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, OUEnumTipoErrorOU.EX_GUIA_NO_SE_ENCUENTRA_EN_AUDITORIA.ToString(), OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_GUIA_NO_SE_ENCUENTRA_EN_AUDITORIA)));
                    }

                    if (!planilla.EsAuditoria)
                    {
                        if (ultimoEstadoGuia.IdEstadoGuia != (short)ADEnumEstadoGuia.CentroAcopio)
                            throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, OUEnumTipoErrorOU.EX_GUIA_ASIGNADA_A_OTRO_INVENTARIO.ToString(), OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_GUIA_ASIGNADA_A_OTRO_INVENTARIO)));
                    }
                    else
                    {
                        if (ultimoEstadoGuia.IdEstadoGuia != (short)ADEnumEstadoGuia.Auditoria)
                            throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, OUEnumTipoErrorOU.EX_GUIA_NO_SE_ENCUENTRA_EN_AUDITORIA.ToString(), OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_GUIA_NO_SE_ENCUENTRA_EN_AUDITORIA)));
                    }

                    if (fachadaCentroAcopio.validarAsignacionInventario(planilla.Guias.NumeroGuia.Value, ControllerContext.Current.IdCentroServicio))
                        throw new FaultException<ControllerException>
                            (new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA,
                                OUEnumTipoErrorOU.EX_GUIA_ASIGNADA_A_OTRO_INVENTARIO.ToString(),
                                OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_GUIA_ASIGNADA_A_OTRO_INVENTARIO)));

                    //para el caso que las planillas las esté realizando una agencia (ej:Espinal), la propiedad  planilla.IdCentroLogistico debe ser igual al col de apoyo de la agencia
                    PUCentroServiciosDC colCentroSrv = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerCentroLogisticoResponsableCentroServicio(planilla.IdCentroLogistico);
                    if (colCentroSrv.IdColRacolApoyo.HasValue && colCentroSrv.IdColRacolApoyo != planilla.IdCentroLogistico)
                    {
                        planilla.IdCentroLogistico = colCentroSrv.IdColRacolApoyo.Value;
                    }


                    PUCentroServiciosDC col = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerCentroLogisticoResponsableCentroServicio(planilla.Guias.IdCentroServicioDestino);

                    if ((col.IdColRacolApoyo != null && col.IdColRacolApoyo == planilla.IdCentroLogistico) || col.Tipo == ConstantesFramework.TIPO_CENTRO_SERVICIO_RACOL)
                    {
                        //Valida si la ciudad destino es diferente a la ciudad de asignacion del envio
                        if (planilla.IdCiudad != planilla.Guias.DetalleGuia.IdCiudadDestino)
                        {
                            if (!COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ValidarCiudadSeApoyaCOL(planilla.Guias.DetalleGuia.IdCiudadDestino, col.IdColRacolApoyo.Value))
                                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, OUEnumTipoErrorOU.EX_CIUDAD_ASIGNACION_DIFERENTE_CIUDAD_DESTINO_GUIA.ToString(), OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_CIUDAD_ASIGNACION_DIFERENTE_CIUDAD_DESTINO_GUIA)));
                        }
                    }
                    else
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, OUEnumTipoErrorOU.EX_DESTINO_ENVIO_NO_SE_APOYA_COL_ASIGNACION.ToString(), OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_DESTINO_ENVIO_NO_SE_APOYA_COL_ASIGNACION)));

                    //valida si la guia la tiene otro mensajero, no esta en estado devolucion y no pertenece a la misma planilla
                    OURepositorio.Instancia.ObtenerEnviosPlanillaAsignacionGuia(planilla.Guias.IdAdmision, planilla.IdPlanillaAsignacion);
                    OUMensajeroDC mensajeroActual = OURepositorio.Instancia.ConsultarMensajeroPlanilla(planilla.IdPlanillaAsignacion);

                    //Se afecta la cuenta del mensajero asignado a la planilla si el envío es un al cobro
                    if (mensajeroActual != null && planilla.Guias.EsAlCobro)
                    {
                        string observacion = "Al cobro descontado por reasignación a la planilla " + planilla.IdPlanillaAsignacion.ToString();
                        OURepositorio.Instancia.NivelarCuentasMensajerosACeroXFactura((long)planilla.Guias.NumeroGuia, observacion, (int)CAEnumConceptosCaja.TRANS_DESCUENTO_ALCOBRO_DEVUELTO);

                        ICAFachadaCajas fachadaCajas = COFabricaDominio.Instancia.CrearInstancia<ICAFachadaCajas>();
                        CACuentaMensajeroDC transMensajero = new CACuentaMensajeroDC()
                        {
                            ConceptoCajaMensajero = new CAConceptoCajaDC()
                            {
                                IdConceptoCaja = (int)CAEnumConceptosCaja.PAGO_DE_ENVIO_AL_COBRO
                            },
                            ConceptoEsIngreso = true,
                            FechaGrabacion = DateTime.Now,
                            Mensajero = new OUNombresMensajeroDC()
                            {
                                IdPersonaInterna = mensajeroActual.IdMensajero,
                                NombreApellido = mensajeroActual.NombreCompleto
                            },
                            NumeroDocumento = (long)planilla.Guias.NumeroGuia,
                            Observaciones = "Al cobro cargado por asignacion de guía de la planilla " + planilla.IdPlanillaAsignacion.ToString(),
                            UsuarioRegistro = ControllerContext.Current.Usuario,
                            Valor = planilla.Guias.ValorTotal
                        };
                        fachadaCajas.AdicionarTransaccMensajero(transMensajero);
                    }

                    //Si es rapiradicado consulta la cantidad de radicados
                    if (planilla.Guias.DetalleGuia.IdServicio == OUConstantesOperacionUrbana.RAPIRADICADO)
                    {
                        planilla.Guias.CantidadRadicados = OURepositorio.Instancia.ObtenerInfoEnvioRapiRadicado(planilla.Guias.IdAdmision);
                    }
                    else
                        planilla.Guias.CantidadRadicados = 0;
                   
                    planilla.Guias.Consecutivo++;
                    planilla.Guias.CreadoPor = ControllerContext.Current.Usuario;
                    planilla.Guias.EstaVerificada = false;
                    planilla.Guias.FechaDescarga = ConstantesFramework.MinDateTimeController;
                    planilla.Guias.EstaPlanillada = true;
                    planilla.Guias.EstadoGuiaPlanilla = OUConstantesOperacionUrbana.ESTADO_PLANILLADA;

                    ADTrazaGuia trazaGuia = new ADTrazaGuia
                    {
                        Ciudad = planilla.Guias.Ciudad,
                        IdCiudad = planilla.Guias.IdCiudad,
                        IdAdmision = planilla.Guias.IdAdmision,
                        IdEstadoGuia = (short)(EstadosGuia.ObtenerUltimoEstado(planilla.Guias.IdAdmision)),

                        Modulo = COConstantesModulos.PRUEBAS_DE_ENTREGA,
                        NumeroGuia = planilla.Guias.NumeroGuia,
                        Observaciones = string.Empty,
                    };

                    if (ultimoEstadoGuia.IdEstadoGuia == (short)ADEnumEstadoGuia.CentroAcopio)
                    {
                        trazaGuia.IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.EnReparto;
                        trazaGuia.IdTrazaGuia = EstadosGuia.ValidarInsertarEstadoGuia(trazaGuia);

                        if (trazaGuia.IdTrazaGuia == 0)
                        {
                            string descripcionEstado = ", Estado actual " + ((ADEnumEstadoGuia)(trazaGuia.IdEstadoGuia)).ToString();

                            ControllerException excepcion = new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA
                                            , OUEnumTipoErrorOU.EX_CAMBIO_ESTADO_NO_VALIDO.ToString()
                                            , OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_CAMBIO_ESTADO_NO_VALIDO));
                            excepcion.Mensaje = excepcion.Mensaje + descripcionEstado;
                            throw new FaultException<ControllerException>(excepcion);
                        }

                        EnviarMensajeDestinatario(planilla);

                        ADTrazaGuiaImpresoDC trazaImpreso = new ADTrazaGuiaImpresoDC
                        {
                            IdTrazaGuia = trazaGuia.IdTrazaGuia,
                            NumeroImpreso = planilla.IdPlanillaAsignacion,
                            TipoImpreso = ADEnumTipoImpreso.Planilla,
                            Usuario = ControllerContext.Current.Usuario,
                        };
                        EstadosGuia.InsertarEstadoGuiaImpreso(trazaImpreso);
                    }
                    ///Guarda el envio de la planilla
                    OURepositorio.Instancia.GuardarGuiaPlanillaAsignacion(planilla);



                    ///Actualiza el total de las guias de la planilla
                    if (planilla.TotalGuias > 1)
                        ActualizaTotalEnviosPlanillados(planilla.IdPlanillaAsignacion, planilla.TotalGuias);

                    int cantidadDeAsignaciones = OURepositorio.Instancia.ObtenerNumeroDeAsignacionesParaUnaGuia(planilla.Guias.NumeroGuia.Value);
                    if (cantidadDeAsignaciones >= Convert.ToInt32(OURepositorio.Instancia.ObtenerValorParametro("CantidadMaxAsigEnvio")))
                    {
                        planilla.MensajeAdvertenciaCantidadAsignacion = string.Format("Este es el intento número {0} de entrega para este envío", cantidadDeAsignaciones);
                    }
                    else
                    {
                        planilla.MensajeAdvertenciaCantidadAsignacion = null;
                    }

                    scope.Complete();
                }
                return planilla;
            }
        }

        /// <summary>
        /// Lanzar mensaje de texto de cliente crédito si es una guía crédito y el destino requiere lanzamiento de mensaje
        /// </summary>
        /// <param name="planilla"></param>
        private void EnviarMensajeDestinatario(OUPlanillaAsignacionDC planilla)
        {
            if(planilla.Guias.TipoCliente == ADEnumTipoCliente.CPE.ToString())
            {
                var telefono = planilla.Guias.TelefonoDestinatario;
                if(Regex.IsMatch(telefono, "^\\d{10}"))
                {
                    new Task(() => Controller.Servidor.Integraciones.MensajesTexto.Instancia.EnviarMensajeCliente(
                                        planilla.Guias.IdAdmision, 
                                        (long)planilla.Guias.NumeroGuia, 
                                        planilla.Guias.IdCliente, 
                                        planilla.Guias.NumeroPedido, 
                                        planilla.Guias.TelefonoDestinatario,
                                        planilla.Guias.DetalleGuia.IdServicio)
                                    ).Start();
                }
            }

            if (planilla.Guias.EsAlCobro == true && planilla.Guias.EstaDescargada == false)
            {
                new Task(() => Controller.Servidor.Integraciones.MensajesTexto.Instancia.EnviarMensajeNoCliente(
                                        planilla.Guias.IdAdmision,
                                        (long)planilla.Guias.NumeroGuia,
                                        Controller.Servidor.Integraciones.EnumMensajeNocliente.AlCobroSinPagar.ToString(),
                                        planilla.Guias.TelefonoDestinatario
                                        ,planilla.Guias.ValorTotal)
                                    ).Start();
            }
        }


        #endregion Inserción

        #region Actualizacion

        /// <summary>
        /// Modifica una planilla de asignacion de envios
        /// </summary>
        /// <param name="planilla"></param>
        public void ModificarPlanillaAsignacionEnvio(OUPlanillaAsignacionDC planilla)
        {
            OURepositorio.Instancia.ModificarPlanillaAsignacionEnvio(planilla);
        }

        /// <summary>
        /// Actualiza el total de los envios planillados
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <param name="totalGuias"></param>
        public void ActualizaTotalEnviosPlanillados(long idPlanilla, int totalGuias)
        {
            OURepositorio.Instancia.ActualizaTotalEnviosPlanillados(idPlanilla, totalGuias);
        }

        /// <summary>
        /// Actualiza el total de los envios planillados
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <param name="totalGuias"></param>
        public void ActualizaTotalEnviosPlanillados(long idPlanilla)
        {
            OURepositorio.Instancia.ActualizaTotalEnviosPlanillados(idPlanilla);
        }

        /// <summary>
        /// verifica el envio seleccionado
        /// </summary>
        /// <param name="planillaAsignacion"></param>
        /// <param name="guia"></param>
        public void ActualizaEnvioVerificadoPlanillaAsignacion(long planillaAsignacion, long numGuia)
        {
            OUGuiaIngresadaDC guia = OURepositorio.Instancia.ConsultaEnvioPlanillaAsignacionGuia(numGuia);
            //if (guia.Planilla != planillaAsignacion)
            //    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, OUEnumTipoErrorOU.EX_EL_ENVIO_NO_ESTA_ASIGNADO.ToString(), OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_EL_ENVIO_NO_ESTA_ASIGNADO)));

            if (ControllerContext.Current.Usuario.CompareTo(guia.CreadoPor) == 0)
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, OUEnumTipoErrorOU.EX_ERROR_USUARIO_VERIFICA.ToString(), OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_ERROR_USUARIO_VERIFICA)));

            if (!guia.EstaVerificada)
            {
                guia.EstaVerificada = true;
                OURepositorio.Instancia.ActualizaEnvioVerificadoPlanillaAsignacion(guia);
            }
            else
            {
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, OUEnumTipoErrorOU.EX_LA_GUIA_ESTA_VERIFICADA.ToString(), OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_LA_GUIA_ESTA_VERIFICADA)));
            }
        }

        /// <summary>
        /// Cierra la planilla de asignación
        /// </summary>
        /// <param name="idPlanilla"></param>
        public void CerrarPlanillaAsignacion(long idPlanilla)
        {
            OURepositorio.Instancia.CerrarPlanillaAsignacion(idPlanilla);
        }

        /// <summary>
        /// Abrir Planilla de asignacion
        /// </summary>
        /// <param name="idPlanilla"></param>
        public void AbrirPlanillaAsignacion(long idPlanilla)
        {
            OURepositorio.Instancia.AbrirPlanillaAsignacion(idPlanilla);
        }

        /// <summary>
        /// Asigna el mensajero a la planilla seleccionada
        /// </summary>
        /// <param name="planilla">Planilla de asignacion</param>
        public void AsignaMensajeroPlanilla(OUPlanillaAsignacionDC planilla)
        {
            using (TransactionScope transaccion = new TransactionScope())
            {

                OUMensajeroDC mensajero = OURepositorio.Instancia.ConsultarMensajeroPlanilla(planilla.IdPlanillaAsignacion);
                if (mensajero != null && (EnumEstadoRegistro.ADICIONADO.CompareTo(planilla.Mensajero.EstadoRegistro) == 0))
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, OUEnumTipoErrorOU.EX_PLANILLA_YA_ASIGNADA.ToString(), string.Format(OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_PLANILLA_YA_ASIGNADA), mensajero.NombreCompleto)));

                }


                string observacion = "Al cobro descontado por reasignación de la planilla " + planilla.IdPlanillaAsignacion.ToString();
                OURepositorio.Instancia.NivelarCuentasMensajerosACero(planilla.IdPlanillaAsignacion, observacion);
                if (EnumEstadoRegistro.ADICIONADO.CompareTo(planilla.Mensajero.EstadoRegistro) == 0)
                {
                    OURepositorio.Instancia.GuardarMensajeroPlanilla(planilla);
                }

                else if (EnumEstadoRegistro.MODIFICADO.CompareTo(planilla.Mensajero.EstadoRegistro) == 0)
                {
                    OUMensajeroDC mensajeroActual = OURepositorio.Instancia.ConsultarMensajeroPlanilla(planilla.IdPlanillaAsignacion);
                    OUMensajeroDC mensajeroNuevo = planilla.Mensajero;
                    
                    if (mensajeroActual != null)
                    {
                        if (mensajeroActual.IdMensajero != mensajeroNuevo.IdMensajero)
                        {
                            OURepositorio.Instancia.ActualizaMensajeroPlanilla(planilla);
                        }
                    }
                    else
                    {
                        OURepositorio.Instancia.GuardarMensajeroPlanilla(planilla);
                    }

                }
                observacion = "Al cobro cargado por asignación para reparto de la planilla No. " + planilla.IdPlanillaAsignacion.ToString();
                OURepositorio.Instancia.AfectarCuentaMensajeroPorAsignacion(planilla.IdPlanillaAsignacion, planilla.Mensajero.IdMensajero, planilla.Mensajero.NombreCompleto, CAEnumConceptosCaja.PAGO_DE_ENVIO_AL_COBRO, observacion);
                transaccion.Complete();
            }
        }

        #endregion Actualizacion

        #region Eliminacion

        /// <summary>
        /// Elimina un envio de la planilla de asignacion
        /// </summary>
        /// <param name="idAdmisionMensajeria"></param>
        public void EliminarEnvioPlanillaAsignacion(OUPlanillaAsignacionDC planilla, long idAdmisionMensajeria)
        {
            OUMensajeroDC mensajeroActual = OURepositorio.Instancia.ConsultarMensajeroPlanilla(planilla.IdPlanillaAsignacion);
            IADFachadaAdmisionesMensajeria fachadaMensajeria = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>();
            ADGuia guia = fachadaMensajeria.ObtenerGuia(idAdmisionMensajeria);
            using (TransactionScope transaccion = new TransactionScope())
            {
                OURepositorio.Instancia.NivelarCuentasMensajerosACeroXFactura(guia.NumeroGuia, "Al cobro descontado por eliminación de guía de la planilla " + planilla.IdPlanillaAsignacion.ToString(), (int)CAEnumConceptosCaja.TRANS_DESCUENTO_ALCOBRO_DEVUELTO);

                OURepositorio.Instancia.EliminarEnvioPlanillaAsignacion(idAdmisionMensajeria, planilla.IdPlanillaAsignacion);

                //Actualiza la cantidad de envios
                ActualizaTotalEnviosPlanillados(planilla.IdPlanillaAsignacion, planilla.TotalGuias - 1);
                planilla.Guias.IdAdmision = idAdmisionMensajeria;
                planilla.Guias.NumeroGuia = guia.NumeroGuia;

                ADTrazaGuia estadoGuia = new ADTrazaGuia
                {
                    Ciudad = planilla.Guias.Ciudad,
                    IdCiudad = planilla.Guias.IdCiudad,
                    IdAdmision = planilla.Guias.IdAdmision,
                    IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.CentroAcopio,
                    Modulo = COConstantesModulos.PRUEBAS_DE_ENTREGA,
                    NumeroGuia = planilla.Guias.NumeroGuia,
                    Observaciones = string.Empty,
                    FechaGrabacion = DateTime.Now
                };
                //EstadosGuia.InsertaEstadoGuia(estadoGuia);
                EstadosGuia.InsertaEstadoGuia(estadoGuia);
                //if (estadoGuia.IdTrazaGuia == 0)
                //{
                //    string descripcionEstado = ", Estado actual " + ((ADEnumEstadoGuia)(estadoGuia.IdEstadoGuia)).ToString();

                //    ControllerException excepcion = new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA
                //                    , OUEnumTipoErrorOU.EX_CAMBIO_ESTADO_NO_VALIDO.ToString()
                //                    , OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_CAMBIO_ESTADO_NO_VALIDO));
                //    excepcion.Mensaje = excepcion.Mensaje + descripcionEstado;
                //    throw new FaultException<ControllerException>(excepcion);
                //}

                transaccion.Complete();

            }
        }


        /// <summary>
        /// Elimina un envio de la planilla de asignacion para CAC
        /// </summary>
        /// <param name="idAdmisionMensajeria"></param>
        public void EliminarEnvioPlanillaCentroAcopio(long numeroGuia)
        {
            using (TransactionScope transaccion = new TransactionScope())
            {
                ADTrazaGuia estado = EstadosGuia.ObtenerTrazaUltimoEstadoXNumGuia(numeroGuia);
                OUGuiaIngresadaDC planillaValidar = ConsultaEnvioPlanillaAsignacionGuia(numeroGuia);

                if (planillaValidar.Planilla != 0)
                {
                    if (estado.IdEstadoGuia == (short)ADEnumEstadoGuia.EnReparto)
                    {
                        OURepositorio.Instancia.NivelarCuentasMensajerosACeroXFactura(numeroGuia, "Al cobro descontado por eliminación de guía de la planilla ", (int)CAEnumConceptosCaja.TRANS_DESCUENTO_ALCOBRO_DEVUELTO);
                        OURepositorio.Instancia.EliminarEnvioPlanillaAsignacionCac(numeroGuia);

                        estado.IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.CentroAcopio;
                        EstadosGuia.InsertaEstadoGuia(estado);
                    }
                    else
                    {
                        string descripcionEstado = ", Estado actual " + ((ADEnumEstadoGuia)(estado.IdEstadoGuia)).ToString();
                        ControllerException excepcion = new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA
                                        , OUEnumTipoErrorOU.EX_CAMBIO_ESTADO_NO_VALIDO.ToString()
                                        , OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_CAMBIO_ESTADO_NO_VALIDO));
                        excepcion.Mensaje = excepcion.Mensaje + descripcionEstado;
                        throw new FaultException<ControllerException>(excepcion);
                    }
                }
                else
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA
                                         , OUEnumTipoErrorOU.EX_EL_ENVIO_NO_ESTA_ASIGNADO.ToString()
                                         , OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_EL_ENVIO_NO_ESTA_ASIGNADO));
                    throw new FaultException<ControllerException>(excepcion);
                }
                transaccion.Complete();
            }
        }

        /// <summary>
        /// Valida si el envío está asignado a una planilla
        /// </summary>
        /// <param name="idGuia"></param>
        /// <returns></returns>
        public OUGuiaIngresadaDC ConsultaEnvioPlanillaAsignacionGuia(long idGuia)
        {
            return OURepositorio.Instancia.ConsultaEnvioPlanillaAsignacionGuia(idGuia);
        }


        /// <summary>
        /// Reasigna la guia seleccionada
        /// </summary>
        /// <param name="planilla"></param>
        /// <param name="idPlanillaNueva"></param>
        /// <param name="idAdmisionMensajeria"></param>
        public void ReasignarEnvioPlanilla(long planilla, long idPlanillaNueva, long idAdmisionMensajeria, long idAgencia)
        {
            OUMensajeroDC mensajeroPlanillaVieja = OURepositorio.Instancia.ConsultarMensajeroPlanilla(planilla);
            OUMensajeroDC mensajeroPlanillaNueva = OURepositorio.Instancia.ConsultarMensajeroPlanilla(idPlanillaNueva);
            IADFachadaAdmisionesMensajeria fachadaMensajeria = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>();
            ADGuia guia = fachadaMensajeria.ObtenerGuia(idAdmisionMensajeria);

            using (TransactionScope transaccion = new TransactionScope())
            {
                if (mensajeroPlanillaVieja != null && guia.EsAlCobro)
                {
                    ICAFachadaCajas fachadaCajas = COFabricaDominio.Instancia.CrearInstancia<ICAFachadaCajas>();
                    CACuentaMensajeroDC transMensajero = new CACuentaMensajeroDC()
                    {
                        ConceptoCajaMensajero = new CAConceptoCajaDC()
                        {
                            IdConceptoCaja = (int)CAEnumConceptosCaja.TRANS_DESCUENTO_ALCOBRO_DEVUELTO
                        },
                        ConceptoEsIngreso = false,
                        FechaGrabacion = DateTime.Now,
                        Mensajero = new OUNombresMensajeroDC()
                        {
                            IdPersonaInterna = mensajeroPlanillaVieja.IdMensajero,
                            NombreApellido = mensajeroPlanillaVieja.NombreCompleto
                        },
                        NumeroDocumento = guia.NumeroGuia,
                        Observaciones = "Al cobro descontado por reasignación de guía de la planilla " + planilla.ToString(),
                        UsuarioRegistro = ControllerContext.Current.Usuario,
                        Valor = guia.ValorTotal
                    };
                    fachadaCajas.AdicionarTransaccMensajero(transMensajero);
                }
                if (mensajeroPlanillaNueva != null && guia.EsAlCobro)
                {
                    ICAFachadaCajas fachadaCajas = COFabricaDominio.Instancia.CrearInstancia<ICAFachadaCajas>();
                    CACuentaMensajeroDC transMensajero = new CACuentaMensajeroDC()
                    {
                        ConceptoCajaMensajero = new CAConceptoCajaDC()
                        {
                            IdConceptoCaja = (int)CAEnumConceptosCaja.PAGO_DE_ENVIO_AL_COBRO
                        },
                        ConceptoEsIngreso = true,
                        FechaGrabacion = DateTime.Now,
                        Mensajero = new OUNombresMensajeroDC()
                        {
                            IdPersonaInterna = mensajeroPlanillaNueva.IdMensajero,
                            NombreApellido = mensajeroPlanillaNueva.NombreCompleto
                        },
                        NumeroDocumento = guia.NumeroGuia,
                        Observaciones = "Al cobro cargado por asignación de guía de la planilla No. " + idPlanillaNueva.ToString(),
                        UsuarioRegistro = ControllerContext.Current.Usuario,
                        Valor = guia.ValorTotal
                    };
                    fachadaCajas.AdicionarTransaccMensajero(transMensajero);
                }
                OURepositorio.Instancia.ReasignaEnvioPlanillaAsignacion(planilla, idPlanillaNueva, idAdmisionMensajeria, idAgencia);
                transaccion.Complete();
            }
        }

        #endregion Eliminacion




    }
}
