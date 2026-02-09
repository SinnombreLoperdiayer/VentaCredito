using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using CO.Servidor.Adminisiones.Mensajeria.Datos;
using CO.Servidor.Dominio.Comun.Area;
using CO.Servidor.Dominio.Comun.Cajas;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Comisiones;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria.AdmMasiva;
using CO.Servidor.Servicios.ContratoDatos.Area;
using CO.Servidor.Servicios.ContratoDatos.Cajas;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Comisiones;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Parametros.Consecutivos;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Dominio.Comun.OperacionUrbana;
using System.ServiceModel;
using CO.Servidor.Dominio.Comun.AdmEstadosGuia;
using Framework.Servidor.Comun.Util;

namespace CO.Servidor.Adminisiones.Mensajeria
{
    public class ADAdmisionMensajeria : CO.Servidor.Adminisiones.Mensajeria.Servicios.ADAdmisionServicio
    {
        #region Singleton

        private static readonly ADAdmisionMensajeria instancia = (ADAdmisionMensajeria)FabricaInterceptores.GetProxy(new ADAdmisionMensajeria(), COConstantesModulos.MENSAJERIA);

        /// <summary>
        /// Retorna una instancia de Consultas de admision guía interna
        /// /// </summary>
        public static ADAdmisionMensajeria Instancia
        {
            get { return ADAdmisionMensajeria.instancia; }
        }

        #endregion Singleton

        #region Consultas

        /// <summary>
        /// Calcula las comisiones de entrega para la admisión registrada
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="fachadaComisiones"></param>
        /// <returns></returns>
        public CMComisionXVentaCalculadaDC CalcularComisiones(ADGuia guia, CMEnumTipoComision tipoComision, ICMFachadaComisiones fachadaComisiones)
        {
            CMComisionXVentaCalculadaDC comision = fachadaComisiones.CalcularComisionesxVentas(
              new Servidor.Servicios.ContratoDatos.Comisiones.CMConsultaComisionVenta()
              {
                  IdCentroServicios = guia.IdCentroServicioOrigen,
                  IdServicio = guia.IdServicio,
                  TipoComision = tipoComision,
                  ValorBaseComision = guia.ValorServicio,
                  NumeroOperacion = guia.NumeroGuia,
              });
            return comision;
        }
        #endregion Consultas

        #region Actualizar

        /// <summary>
        /// Actualiza como pagada una guía de mensajeria
        /// </summary>
        /// <param name="idAdmisionMensajeria"></param>
        public void ActualizarPagadoGuia(long idAdmisionMensajeria, bool estaPagada = true)
        {
            using (TransactionScope trans = new TransactionScope())
            {
                ADRepositorio.Instancia.ActualizarPagadoGuia(idAdmisionMensajeria, estaPagada);
                trans.Complete();
            }
        }

        public List<ADGuia> ObtenerReporteCajaGuiasMensajeroApp(long idMensajero)
        {
            return ADRepositorio.Instancia.ObtenerReporteCajaGuiasMensajeroApp(idMensajero);            
        }

        /// <summary>
        /// Actualiza como pagada una guía de mensajeria
        /// </summary>
        /// <param name="idAdmisionMensajeria"></param>
        public void ActualizarReintentoEntregasAdmision(long idAdmisionMensajeria)
        {
            using (TransactionScope trans = new TransactionScope())
            {
                ADRepositorio.Instancia.ActualizarReintentosEntregaAdmisionAdo(idAdmisionMensajeria);
                trans.Complete();
            }
        }

        /// <summary>
        /// Actualiza en supervision una guía de mensajeria
        /// </summary>
        /// <param name="idAdmisionMensajeria"></param>
        public void ActualizarSupervisionGuia(long idAdmisionMensajeria)
        {
            using (TransactionScope trans = new TransactionScope())
            {
                ADRepositorio.Instancia.ActualizarSupervisionGuia(idAdmisionMensajeria);
                trans.Complete();
            }
        }

        /// <summary>
        /// Adiciona archivo de un radicado
        /// </summary>
        /// <param name="archivo">objeto de tipo archivo</param>
        public void AdicionarArchivosRapiradicado(ADRapiRadicado radicado)
        {
            if (radicado.ListaArchivos.Any())
                using (TransactionScope trans = new TransactionScope())
                {
                    radicado.ListaArchivos.ToList().ForEach(archivo =>
                      {
                          ADRepositorio.Instancia.AdicionarArchivoRapiradicado(archivo, radicado.IdRapiradicado);
                      });
                    trans.Complete();
                }
        }

        /// <summary>
        /// Actualiza la guía y registra el valor en Caja de la transaccion.
        /// </summary>
        /// <param name="guiaAlCobro">The guia al cobro.</param>
        public ADRecaudoAlCobro ActualizarGuiaAlCobro(ADRecaudarDineroAlCobroDC guiaAlCobro)
        {
            ICAFachadaCajas fachadaCajas = COFabricaDominio.Instancia.CrearInstancia<ICAFachadaCajas>();
            CAIdTransaccionesCajaDC movimiento = new CAIdTransaccionesCajaDC();
            IPUFachadaCentroServicios fachadaCs = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();
            string idCentroCostos = string.Empty;

            IOUFachadaOperacionUrbana fachadaOpUrbana = COFabricaDominio.Instancia.CrearInstancia<IOUFachadaOperacionUrbana>();

            decimal valorCaja = 0;
            PUCentroServiciosDC csAfectacionCaja = fachadaCajas.ConsultarCajaAfectadaPorPagoDeAlCobro(guiaAlCobro.InfoGuiaRecaudada.NumeroGuia, out valorCaja);

            if ((csAfectacionCaja == null) || ((csAfectacionCaja != null) && (string.IsNullOrEmpty(csAfectacionCaja.NoComprobante) || string.IsNullOrWhiteSpace(csAfectacionCaja.NoComprobante) || csAfectacionCaja.NoComprobante == "0")))
            {
                using (TransactionScope trans = new TransactionScope())
                {
                    guiaAlCobro.InfoGuiaRecaudada = ADConsultas.Instancia.ObtenerGuia(guiaAlCobro.InfoGuiaRecaudada.IdAdmision);
                    guiaAlCobro.MovimientoCaja.RegistrosTransacDetallesCaja.FirstOrDefault().ValorServicio = guiaAlCobro.InfoGuiaRecaudada.ValorServicio;
                    guiaAlCobro.MovimientoCaja.RegistrosTransacDetallesCaja.FirstOrDefault().ValorPrimaSeguros = guiaAlCobro.InfoGuiaRecaudada.ValorPrimaSeguro;
                    guiaAlCobro.MovimientoCaja.RegistrosTransacDetallesCaja.FirstOrDefault().ValoresAdicionales = guiaAlCobro.InfoGuiaRecaudada.ValorAdicionales;
                    guiaAlCobro.MovimientoCaja.ValorTotal = guiaAlCobro.InfoGuiaRecaudada.ValorServicio + guiaAlCobro.InfoGuiaRecaudada.ValorPrimaSeguro + guiaAlCobro.InfoGuiaRecaudada.ValorAdicionales;

                    PUCentroServiciosDC cs = fachadaCs.ObtenerCentroServicio(guiaAlCobro.InfoGuiaRecaudada.IdCentroServicioDestino);
                    long idCentroServicioResponsable = guiaAlCobro.InfoGuiaRecaudada.IdCentroServicioDestino;
                    string nombreCentroServicioResponsable = cs.Nombre;
                    idCentroCostos = cs.IdCentroCostos;
                    if (cs.Tipo == ConstantesFramework.TIPO_CENTRO_SERVICIO_PUNTO)
                    {
                        PUAgenciaDeRacolDC csResponsable = fachadaCs.ObtenerAgenciaResponsable(guiaAlCobro.InfoGuiaRecaudada.IdCentroServicioDestino);
                        idCentroServicioResponsable = csResponsable.IdResponsable;
                        nombreCentroServicioResponsable = csResponsable.NombreResponsable;
                    }

                    guiaAlCobro.MovimientoCaja.IdCentroResponsable = idCentroServicioResponsable;
                    guiaAlCobro.MovimientoCaja.NombreCentroResponsable = nombreCentroServicioResponsable;

                    ADRepositorio.Instancia.EliminarAlCobroCargadoCoordCol(guiaAlCobro.InfoGuiaRecaudada.IdAdmision);

                    //Se actualiza el Registro de guia recaudada
                    ADRepositorio.Instancia.ActualizarPagadoGuia(guiaAlCobro.InfoGuiaRecaudada.IdAdmision);
                    // Se graba un movimiento de caja de ingreso para el centro de servicio que recauda con concepto “Pago de envío al cobro” y se retorna el número de comprobante de ingreso para impresión
                    movimiento = fachadaCajas.AdicionarMovimientoCaja(guiaAlCobro.MovimientoCaja, PAEnumConsecutivos.Comprobante_Ingreso);

                    //Si el envio esta asignado a un mensajero y au no no se ha descargado se debe aplicar un movimiento en caja del mensajero
                    fachadaOpUrbana.NivelarCuentasMensajerosACeroXFactura(guiaAlCobro.InfoGuiaRecaudada.NumeroGuia, "Descuento/abono de al cobro por pago directo en Centro de Servicio " + guiaAlCobro.InfoGuiaRecaudada.IdCentroServicioDestino.ToString().Trim() + "-" + cs.Nombre.Trim(), (int)CAEnumConceptosCaja.DESCUENTO_POR_DESCARGUE_AL_COBRO_OTRO_CS);


                    //Si el envio ya había afectado una caja por vencimiento se aplica un egreso por el valor de la afectación inicial
                    if (csAfectacionCaja != null && valorCaja > 0)
                    {
                        long idCSQueRecauda = guiaAlCobro.MovimientoCaja.IdCentroServiciosVenta;
                        string nombreCSQueRecauda = guiaAlCobro.MovimientoCaja.NombreCentroServiciosVenta;

                        guiaAlCobro.MovimientoCaja.RegistrosTransacDetallesCaja.FirstOrDefault().ConceptoCaja.IdConceptoCaja = (int)CAEnumConceptosCaja.DESCUENTO_POR_DESCARGUE_AL_COBRO_OTRO_CS;
                        guiaAlCobro.MovimientoCaja.RegistrosTransacDetallesCaja.FirstOrDefault().ConceptoCaja.Descripcion = "Descuento por descargue al cobro en otro centro de servicio";
                        guiaAlCobro.MovimientoCaja.RegistrosTransacDetallesCaja.FirstOrDefault().Observacion = "Descuento por descargue al cobro en centro de servicio " + idCSQueRecauda + "-" + nombreCSQueRecauda;
                        guiaAlCobro.MovimientoCaja.RegistrosTransacDetallesCaja.FirstOrDefault().ValorServicio = valorCaja;
                        guiaAlCobro.MovimientoCaja.RegistrosTransacDetallesCaja.FirstOrDefault().ValorPrimaSeguros = 0;
                        guiaAlCobro.MovimientoCaja.RegistrosTransacDetallesCaja.FirstOrDefault().ValoresAdicionales = 0;
                        guiaAlCobro.MovimientoCaja.RegistrosTransacDetallesCaja.FirstOrDefault().ConceptoEsIngreso = false;
                        guiaAlCobro.MovimientoCaja.IdCentroResponsable = csAfectacionCaja.IdCentroServicio;
                        guiaAlCobro.MovimientoCaja.NombreCentroResponsable = csAfectacionCaja.Nombre;
                        guiaAlCobro.MovimientoCaja.IdCentroServiciosVenta = csAfectacionCaja.IdCentroServicio;
                        guiaAlCobro.MovimientoCaja.NombreCentroServiciosVenta = csAfectacionCaja.Nombre;

                        guiaAlCobro.MovimientoCaja.ValorTotal = valorCaja;

                        movimiento = fachadaCajas.AdicionarMovimientoCaja(guiaAlCobro.MovimientoCaja);
                    }

                    trans.Complete();
                }
            }
            else
            {
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MENSAJERIA, "0", "La factura al cobro que intenta pagar ya se encuentra pagada"));
            }
            AREmpresaDC empresaRacol = COFabricaDominio.Instancia.CrearInstancia<IARFachadaAreas>().ObtenerDatosEmpresa();
            return new ADRecaudoAlCobro
            {
                NitEmpresa = empresaRacol.Nit,
                NumeroConsecutivo = movimiento.NumeroConsecutivo,
                IdCentroCosto = idCentroCostos
            };
        }

        /// <summary>
        /// Valida si un al cobro especifico está asignado a un coordinador de col x vencimiento en el pago
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <returns></returns>
        public bool AlCobroCargadoACoordinadorCol(long idAdmision)
        {
            return ADRepositorio.Instancia.AlCobroCargadoACoordinadorCol(idAdmision);
        }

        /// <summary>
        /// Método para actualizar el número de guía interna
        /// </summary>
        /// <param name="idRadicado"></param>
        /// <param name="numeroGuiaInterna"></param>
        public void ActualizarGuiaRapiradicado(long idRadicado, long numeroGuiaInterna)
        {
            using (TransactionScope trans = new TransactionScope())
            {
                ADRepositorio.Instancia.ActualizarGuiaRapiradicado(idRadicado, numeroGuiaInterna);
                trans.Complete();
            }
        }

        /// <summary>
        /// Actualiza el valor del porcentaje de
        /// recargo
        /// </summary>
        /// <param name="valor">el valor a actualizar</param>
        public void ActualizarParametroPorcentajeRecargo(double porcentaje)
        {
            ADRepositorio.Instancia.ActualizarParametroPorcentajeRecargo(porcentaje);
        }


        /// <summary>
        /// Método para actualizar el campo entregado en la tabla de admisión mensajeria
        /// </summary>
        /// <param name="numeroGuia"></param>
        public void ActualizarEntregadoGuia(long numeroGuia)
        {
            //    ADRepositorio.Instancia.ActualizarEntregadoGuia(numeroGuia);
        }

        #endregion Actualizar

        #region Eliminación

        /// <summary>
        /// Eliminar una guía interna
        /// </summary>
        /// <param name="manifiesto"></param>
        public void EliminarGuiaInterna(ADGuiaInternaDC guiaInterna)
        {
            ADRepositorio.Instancia.EliminarGuiaInterna(guiaInterna);
        }

        #endregion Eliminación

        #region reexpedicion

        /// <summary>
        /// Guarda la relacion de las guias
        /// </summary>
        /// <param name="idRadicado"></param>
        /// <param name="numeroGuiaInterna"></param>
        public void GuadarRelacionReexpedicionEnvio(long idAdmisionOriginal, long idAdmisionNueva)
        {
            ADRepositorio.Instancia.GuadarRelacionReexpedicionEnvio(idAdmisionOriginal, idAdmisionNueva);
        }

        #endregion reexpedicion

        #region Orden de Servicio Cargue Masivo

        /// <summary>
        /// Crea una orden de servicio para asociar las guias de un cargue masivo de mensajeria
        /// </summary>
        /// <param name="idOrdenServicio">Retorna el número de orden de servicio creado</param>
        public long CrearOrdenServicioMasivo(ADOrdenServicioMasivoDC datosOrdenServicio)
        {
            return ADRepositorio.Instancia.CrearOrdenServicioMasivo(datosOrdenServicio);
        }

        /// <summary>
        /// Validar la existencia de una orden de servicio masiva
        /// </summary>
        /// <param name="ordenServicio">número de la orden de servicio a validar</param>
        /// <returns></returns>
        public bool ValidarOrdenServicio(long ordenServicio)
        {
            return ADRepositorio.Instancia.ValidarOrdenServicio(ordenServicio);
        }

        /// <summary>
        /// Almacena una guía asociada a una orden de servicio
        /// </summary>
        /// <param name="idAdmision">Id de la admisión</param>
        /// <param name="numeroGuia">número de guía</param>
        /// <param name="numeroOrdenServicio">Numero de orden de servicio</param>
        public void GuardarGuiaOrdenServicio(long idAdmision, long numeroGuia, long numeroOrdenServicio)
        {
            ADRepositorio.Instancia.GuardarGuiaOrdenServicio(idAdmision, numeroGuia, numeroOrdenServicio);
        }

        /// <summary>
        /// Consulta los datos de las guías asociadas a una orden de servicio cargada masivamente
        /// </summary>
        /// <param name="idOrdenServicio">Número de la orden de servicio</param>
        /// <returns></returns>
        public List<ADGuia> ConsultarGuiasDeOrdenDeServicio(long? idOrdenServicio, int? pageSize, int? pageIndex)
        {
            return ADRepositorio.Instancia.ConsultarGuiasDeOrdenDeServicio(idOrdenServicio, pageSize, pageIndex);
        }

        /// <summary>
        /// Obtiene el listado de
        /// las ordenes de servicio por fecha
        /// </summary>
        /// <param name="fechaInicial"></param>
        /// <param name="fechaFinal"></param>
        /// <returns>lista de ordenes por fecha</returns>
        public List<ADOrdenServicioMasivoDC> ObtenerOrdenesServicioPorFecha(DateTime fechaInicial, DateTime fechaFinal)
        {
            return ADRepositorio.Instancia.ObtenerOrdenesServicioPorFecha(fechaInicial, fechaFinal);
        }

        /// <summary>
        /// Consulta la cantidad de guias de admisión asociadas a una orden de servicio
        /// </summary>
        /// <param name="idOrdenServicio"></param>
        /// <returns></returns>
        public long ConsultarCantidadGuiasOrdenSerServicio(long idOrdenServicio)
        {
            return ADRepositorio.Instancia.ConsultarCantidadGuiasOrdenSerServicio(idOrdenServicio);
        }

        #endregion Orden de Servicio Cargue Masivo

        /// <summary>
        /// Inserta un registro de una notificacion
        /// </summary>
        /// <param name="numeroGuia"></param>
        public void AdicionarNotificacion(long numeroGuia)
        {
            using (TransactionScope trans = new TransactionScope())
            {
                ADRepositorio.Instancia.AdicionarNotificacion(numeroGuia);
                trans.Complete();
            }

        }

        /// <summary>
        /// Retorna los rangos de peso y casilleros por un trayecto dado (ciudad origen y ciudad destino)
        /// </summary>
        /// <param name="idLocalidadOrigen"></param>
        /// <param name="idLocalidadDestino"></param>
        /// <returns></returns>
        public ADRangoTrayecto ConsultarCasilleroTrayecto(string idLocalidadOrigen, string idLocalidadDestino)
        {
            if (TrayectosCasillero == null)
            {
                TrayectosCasillero = ADRepositorio.Instancia.ObtenerCasillerosTrayectos();
            }
            return TrayectosCasillero.FirstOrDefault(t => t.IdLocalidadOrigen == idLocalidadOrigen && t.IdLocalidadDestino == idLocalidadDestino);

        }

        public HashSet<ADRangoTrayecto> TrayectosCasillero
        {
            get;
            set;
        }

        /// <summary>
        /// Realizar un cambio de estado de guías masivo
        /// </summary>
        /// <param name="guias"></param>
        /// <returns></returns>
        public Dictionary<long, string> GrabarCambioEstadoGuias(List<ADGuiaUltEstadoDC> guias)
        {
            Dictionary<long, string> resultado = new Dictionary<long, string>();
            foreach (var guia in guias)
            {
                // Obtener estado actual guía
                int idEstadoGuia = 0;
                string mensaje = string.Empty;
                long idAdmisionMensajeri = 0;
                if (ADConsultas.Instancia.ConsultarUltimoEstadoGuia(guia.Guia.NumeroGuia, out idEstadoGuia, out idAdmisionMensajeri))
                {
                    ADEnumEstadoGuia estadoInicial = ADEnumEstadoGuia.Admitida;
                    if (Enum.TryParse<ADEnumEstadoGuia>(idEstadoGuia.ToString(), out estadoInicial))
                    {
                        if (EstadosGuia.ValidarCambioEstado(estadoInicial, (ADEnumEstadoGuia)Enum.Parse(typeof(ADEnumEstadoGuia), guia.TrazaGuia.IdEstadoGuia.ToString())))
                        {
                            guia.TrazaGuia.IdNuevoEstadoGuia = guia.TrazaGuia.IdEstadoGuia;
                            guia.TrazaGuia.IdEstadoGuia = (short)idEstadoGuia;
                            guia.TrazaGuia.NumeroGuia = guia.Guia.NumeroGuia;
                            guia.TrazaGuia.IdAdmision = idAdmisionMensajeri;

                            EstadosGuia.InsertaEstadoGuia(guia.TrazaGuia);
                            resultado.Add(guia.Guia.NumeroGuia, "OK");
                        }
                        else
                        {
                            resultado.Add(guia.Guia.NumeroGuia, "Cambio de estado NO es válido");
                        }
                    }
                    else
                    {
                        resultado.Add(guia.Guia.NumeroGuia, "Guía no existe o estado NO es válido");
                    }
                }
                else
                {
                    resultado.Add(guia.Guia.NumeroGuia, "Guía no existe o estado NO es válido");
                }
            }
            return resultado;
        }


        /// <summary>
        /// verifica si una guia existe
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public bool VerificarSiGuiaExiste(long numeroGuia)
        {
            return ADRepositorio.Instancia.VerificarSiGuiaExiste(numeroGuia);
        }


        /// <summary>
        /// Registra la auditoria de las guias de POS que al sincronizarlas ya se encuentran admitidas
        /// </summary>
        /// <param name="guiaSerializada"></param>
        /// <param name="objetoAdicionalSerializado"></param>
        /// <param name="numeroGuia"></param>
        /// <param name="idCentroServiciosOrigen"></param>
        public void RegistrarAuditoriaAdmisionesManualesDuplicadas(string guiaSerializada, string objetoAdicionalSerializado, long numeroGuia, long idCentroServiciosOrigen)
        {
            ADGuia guiaServidor = ADFachadaAdmisionesMensajeria.Instancia.ObtenerGuiaXNumeroGuia(numeroGuia);
            string guiaSerializadaServidor = Serializacion.SerializarObjetoDataContract<ADGuia>(guiaServidor);
            ADRepositorio.Instancia.RegistrarAuditoriaAdmisionesManualesDuplicadas(guiaSerializada, guiaSerializadaServidor, objetoAdicionalSerializado, numeroGuia, idCentroServiciosOrigen);
        }
        /// <summary>
        /// obtiene informacion de una guia seleccionada ya sea que haya sido modificada o no
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ADGuia ObtenerInformacionGuiaPorNumero(long numeroGuia)
        {
            return ADRepositorio.Instancia.ObtenerInformacionGuiaPorNumero(numeroGuia);
        }
        /// <summary>
        /// Obtiene una lista con las guias encontradas sea por numero de guia o por fecha
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="fechaInicio"></param>
        /// <param name="fechaFin"></param>
        /// <returns></returns>
        public List<ADGuia> ObtenerGuiaPorNumeroOFecha(long? numeroGuia, DateTime fechaInicio, DateTime fechaFin, short index, short size)
        {
            return ADRepositorio.Instancia.ObtenerGuiaPorNumeroOFecha(numeroGuia, fechaInicio, fechaFin, index, size);
        }

        #region reimpresionesWPF

        /// <summary>
        /// Método para obtener el Tipo de Impresion de una Localidad
        /// </summary>
        /// <param name="IdLocalidad"></param>
        /// <returns></returns>
        public long ObtenerTipoFormatoImpresionLocalidad(long IdLocalidad)
        {
            return ADRepositorio.Instancia.ObtenerTipoFormatoImpresionLocalidad(IdLocalidad);
        }

        /// <summary>
        /// Retorna las guías que hayan sido enviadas por el destinatario indicado
        /// </summary>
        /// <param name="tipoDidentificacionDestinatario"></param>
        /// <param name="idDestinatario"></param>
        /// <returns></returns>
        public List<ADGuia> ObtenerGuiasPorDestinatario(string tipoDidentificacionDestinatario, string idDestinatario)
        {
            return ADRepositorio.Instancia.ObtenerGuiasPorDestinatario(tipoDidentificacionDestinatario, idDestinatario);
        }

        /// <summary>
        /// Obtiene la traza de una guia dependiendo del id unico de su estado en admisionmensajeria
        /// </summary>
        /// <param name="idEstadoGuia"></param>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ADEstadoGuia ObtenerEstadoGuiaTrazaPorIdEstado(ADEnumEstadoGuia idEstadoGuia, long numeroGuia)
        {
            return ADRepositorio.Instancia.ObtenerEstadoGuiaTrazaPorIdEstado(idEstadoGuia, numeroGuia);
        }

        #endregion

    }
}