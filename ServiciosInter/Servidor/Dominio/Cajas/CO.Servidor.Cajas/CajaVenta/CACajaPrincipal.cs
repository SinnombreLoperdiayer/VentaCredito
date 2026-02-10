using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Transactions;
using CO.Servidor.Cajas.Apertura;
using CO.Servidor.Cajas.CajaFinanciera;
using CO.Servidor.Cajas.Comun;
using CO.Servidor.Cajas.Datos;
using CO.Servidor.Cajas.Datos.Modelo;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.Suministros;
using CO.Servidor.Dominio.Comun.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros.Pagos;
using CO.Servidor.Servicios.ContratoDatos.Cajas;
using CO.Servidor.Servicios.ContratoDatos.Cajas.Cierres;
using CO.Servidor.Servicios.ContratoDatos.GestionCajas;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;

namespace CO.Servidor.Cajas.CajaVenta
{
    /// <summary>
    /// Clase que contiene los metodos de consulta, cierre
    /// envio de dinero de la caja Ppal del un punto..
    /// </summary>
    internal class CACajaPrincipal : ControllerBase
    {
        #region Atributo

        private static readonly CACajaPrincipal instancia = (CACajaPrincipal)FabricaInterceptores.GetProxy(new CACajaPrincipal(), COConstantesModulos.CAJA);

        #endregion Atributo

        #region Instancia

        public static CACajaPrincipal Instancia
        {
            get { return CACajaPrincipal.instancia; }
        }

        #endregion Instancia

        /// <summary>
        /// Obtener las cajas para cerrar.
        /// </summary>
        /// <param name="idPuntoCentroServicio">Es el id punto centro servicio.</param>
        /// <returns>Las cajas que se pueden Cerrar</returns>
        public List<CACierreCajaDC> ObtenerCajasParaCerrar(long idPuntoCentroServicio, short idFormaPago, string operador)
        {
            return CARepositorioCaja.Instancia.ObtenerCajasParaCerrar(idPuntoCentroServicio, idFormaPago, operador);
        }

        public CAConsolidadoCierreDC ObtenerResumenCierreCajaPrincipal(long idCentroServicios, long idAperturaCaja, long idCierrePuntoAsociado)
        {
            decimal baseInicialCentro = CACaja.Instancia.ObtenerCentroServicio(idCentroServicios).BaseInicialCaja;

            CAConsolidadoCierreDC resumen = CARepositorioCaja.Instancia.ObtenerResumenConsolidadoCierre(idAperturaCaja,
              idCentroServicios,
              idCierrePuntoAsociado,
              baseInicialCentro);

            resumen.InfoGirosNoPagosCentroSvc = CACaja.Instancia.ObtenerGirosNoPagosCentroSvc(idCentroServicios);
            resumen.InfoAlCobrosSinCancelar = CACaja.Instancia.ObtenerTotalesAlCobrosSinPagar(idCentroServicios);
            return resumen;
        }

        /// <summary>
        /// Obtiene el resumen por concepto de caja de un punto o centro de
        /// servicio para cerrar.
        /// </summary>
        /// <param name="idPuntoCentroServicio">Es el id del punto ó centro de servicio.</param>
        /// <returns>la lista de los conceptos agrupados y totalizados para cerrar el punto</returns>
        public List<CAResumeCierrePuntoDC> ObtenerResumenCierrePunto(long idPuntoCentroServicio, long idCierrePuntoAsociado, bool cierreAutomatico = false)
        {
            decimal BaseInicialCentro = CACaja.Instancia.ObtenerCentroServicio(idPuntoCentroServicio).BaseInicialCaja;
            List<CAResumeCierrePuntoDC> resumen = null;
            if (cierreAutomatico)
            {
                resumen = CARepositorioCaja.Instancia.ObtenerResumenCierrePunto(BaseInicialCentro, idPuntoCentroServicio, idCierrePuntoAsociado, cierreAutomatico);
            }
            else
            {
                resumen = CARepositorioCaja.Instancia.ObtenerResumenCierrePunto(BaseInicialCentro, idPuntoCentroServicio, idCierrePuntoAsociado);
            }

            //PGTotalPagosDC infoGiros = CACaja.Instancia.ObtenerGirosNoPagosCentroSvc(idPuntoCentroServicio);

            //Consulto los giros no pagos por el centro de Servicio
            resumen.First().InfoGirosNoPagosCentroSvc = CACaja.Instancia.ObtenerGirosNoPagosCentroSvc(idPuntoCentroServicio);
            resumen.First().InfoAlCobrosSinCancelar = CACaja.Instancia.ObtenerTotalesAlCobrosSinPagar(idPuntoCentroServicio);
            return resumen;
        }

        /// <summary>
        /// Obtiene el ultimo cierre del punto.
        /// </summary>
        /// <returns></returns>
        public CACierreCentroServicioDC ObtenerUltimoCierrePunto(long idPuntoCentroServicio)
        {
            return CARepositorioCaja.Instancia.ObtenerUltimoCierrePunto(idPuntoCentroServicio);
        }

        /// <summary>
        /// Obtiene los cierres punto.
        /// </summary>
        /// <param name="filtro">Es el filtro.</param>
        /// <param name="campoOrdenamiento">El campo de ordenamiento.</param>
        /// <param name="indicePagina">El indice pagina.</param>
        /// <param name="registrosPorPagina">Los registros por pagina.</param>
        /// <param name="ordenamientoAscendente">Sie s verdadero <c>true</c> es ascendente el ordenamiento.</param>
        /// <param name="totalRegistros">El total de Registros.</param>
        /// <param name="idPuntoCentroServicio">Es el Id del Punto de Servicio.</param>
        /// <returns>La lista de los 10 ultimos Cierres del punto</returns>
        public List<CACierreCentroServicioDC> ObtenerCierresPunto(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina,
                                                                  int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros,
                                                                  long idPuntoCentroServicio)
        {
            return CARepositorioCaja.Instancia.ObtenerCierresPunto(filtro, campoOrdenamiento, indicePagina, registrosPorPagina,
                                                                   ordenamientoAscendente, out totalRegistros, idPuntoCentroServicio);
        }

        /// <summary>
        /// Obtiene el Valor a Enviar a la Empresa
        /// </summary>
        /// <param name="idCentroServicio">The id centro servicio.</param>
        public decimal ObtenerValorEmpresa(long idCentroServicio)
        {
            return CARepositorioCaja.Instancia.ObtenerValorEmpresa(idCentroServicio);
        }

        /// <summary>
        /// Inserta el dinero envio empresa y marcar los cierres asociados como reportados.
        /// </summary>
        /// <param name="envioDineroAgencia">El envio dinero agencia.</param>
        public void InsertarDineroEnvioEmpresa(CARecoleccionDineroPuntoDC envioDineroAgencia)
        {
            using (TransactionScope trans = new TransactionScope())
            {
                //Todo: ValidarGuardarSuministroBolsaSeguridad(envioDineroAgencia.BolsaSeguridad, envioDineroAgencia.IdPuntoServicio);
                CARepositorioCaja.Instancia.InsertarDineroEnvioEmpresa(envioDineroAgencia);

                //Se actualizan los cierres anteriores por centroServicio
                CARepositorioCaja.Instancia.MarcarCierresComoReportados(envioDineroAgencia.IdCierreCentroServicios, envioDineroAgencia.IdPuntoServicio);
                trans.Complete();
            }
        }

        /// <summary>
        /// Valida y Guarda el Suministro
        /// </summary>
        /// <param name="EnvioDineroAgencia"></param>
        private void ValidarGuardarSuministroBolsaSeguridad(string idSuministro, long idAgencia)
        {
            #region bolsa Valida

            //ISUFachadaSuministros fachadaSuministros = COFabricaDominio.Instancia.CrearInstancia<ISUFachadaSuministros>();

            //int i = 0;
            //string prefijo = "";
            //long NumeroBolsaSeguridad = 0;
            //while (true)
            //{
            //    prefijo = idSuministro.Substring(0, i);
            //    if (long.TryParse(idSuministro.Substring(prefijo.Length), out NumeroBolsaSeguridad))
            //        break;
            //    i++;
            //    if (prefijo == idSuministro)

            //        //la bolsa de seguridad no pertenece al punto de servicio que lo quiere utilizar
            //        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.CAJA, CAEnumTipoErrorCaja.ERROR_BOLSA_SEGIRIDAD_ERRADA.ToString(),
            //          CACajaServerMensajes.CargarMensaje(CAEnumTipoErrorCaja.ERROR_BOLSA_SEGIRIDAD_ERRADA)));
            //}

            //SUSuministro suministroAsociado = null;
            //if (prefijo != "")
            //    suministroAsociado = fachadaSuministros.ConsultarSuministroxPrefijo(prefijo);

            //if (suministroAsociado != null)
            //{
            //    //SUPropietarioGuia propietario = fachadaSuministros.ObtenerPropietarioSuministro(NumeroBolsaSeguridad, (SUEnumSuministro)Enum.ToObject(typeof(SUEnumSuministro), suministroAsociado.Id), idAgencia);

            //    SUEnumGrupoSuministroDC grupo = (SUEnumGrupoSuministroDC)Enum.Parse(typeof(SUEnumGrupoSuministroDC), propietario.CentroServicios.Tipo);

            //    SUConsumoSuministroDC consumo = null;

            //    consumo = new SUConsumoSuministroDC()
            //    {
            //        Cantidad = 1,
            //        EstadoConsumo = SUEnumEstadoConsumo.CON,
            //        GrupoSuministro = grupo,
            //        IdDuenoSuministro = idAgencia,
            //        IdServicioAsociado = 0,
            //        NumeroSuministro = NumeroBolsaSeguridad,
            //        Suministro = (SUEnumSuministro)Enum.ToObject(typeof(SUEnumSuministro), suministroAsociado.Id)
            //    };
            //    fachadaSuministros.GuardarConsumoSuministro(consumo);
            //}
            //else
            //{
            //    //la bolsa de seguridad no pertenece al punto de servicio que lo quiere utilizar
            //    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.CAJA, CAEnumTipoErrorCaja.ERROR_BOLSA_SEGIRIDAD_ERRADA.ToString(),
            //      CACajaServerMensajes.CargarMensaje(CAEnumTipoErrorCaja.ERROR_BOLSA_SEGIRIDAD_ERRADA)));
            //}

            #endregion bolsa Valida
        }

        /// <summary>
        /// Adiciona el dinero recibido por la Empresa
        /// de una agencio o punto.
        /// </summary>
        /// <param name="dineroRecibido">el dinero recibido.</param>
        public void AdicionarDineroAgencia(CARecoleccionDineroPuntoDC dineroRecibido)
        {
            // si adjuntan el despacho manual para novasof se realiza la validación y guardado de la información
            //este campo no es requerido para realizar la transacción
            if (dineroRecibido.PlanillaDespachoManual != 0)
            {
                ValidarGuardarSuministroBolsaSeguridad(dineroRecibido.BolsaSeguridad,
                                                       dineroRecibido.RegistroCajaPuntoReporta.IdCentroServiciosVenta);
            }

            //cierro y reporto la caja a la caja principal en caso
            //de ser un reporte de dinero manual
            if (dineroRecibido.RegistroManual)
            {


                if (CARepositorioCaja.Instancia.ValidarReporteDineroPuntoNoDescargado(dineroRecibido.RegistroCajaPuntoReporta.IdCentroServiciosVenta))
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.CAJA, CAEnumTipoErrorCaja.ERROR_REPORTE_DINERO_DE_PUNTO_PENDIENTE_POR_DESCARGAR.ToString(),
                                 CACajaServerMensajes.CargarMensaje(CAEnumTipoErrorCaja.ERROR_REPORTE_DINERO_DE_PUNTO_PENDIENTE_POR_DESCARGAR));
                    throw new FaultException<ControllerException>(excepcion);

                }


                CACierreCentroServicioDC infoCierreCentroSvc = null;
                List<CACierreCajaDC> cajasParaCerrar = null;

                //consulto ultimo cierre NO reportado
                CACierreCentroServicioDC cierreNoReport = CARepositorioCaja.Instancia
                                                          .ObtenerUltimoCierreNoReportadoAgencia(dineroRecibido.RegistroCajaAgencia.IdCentroServiciosVenta,
                                                                                                  dineroRecibido.RegistroCajaPuntoReporta.IdCentroServiciosVenta);

                //Valido ultimo cierre no reportado
                if (cierreNoReport == null)
                {
                    using (TransactionScope trans = new TransactionScope())
                    {
                        //consulto Transacciones sin cerrar
                        cajasParaCerrar = CACierreCaja.CACierreCaja.Instancia
                                                                .ObtenerCajasAReportarCajeroPrincipal(dineroRecibido.RegistroCajaPuntoReporta
                                                                                                      .IdCentroServiciosVenta,
                                                                                                      TAConstantesServicios.ID_FORMA_PAGO_DIF_DEMAS,
                                                                                                      CAConstantesCaja.OPERADOR_LOGICO_DIFERENCIA);

                        //Valido Transacciones
                        if (cajasParaCerrar == null)
                        {
                            //Obtengo el ultimo cierre del punto
                            CACierreCentroServicioDC cierreCero = CARepositorioCaja.Instancia.ObtenerUltimoCierrePunto(dineroRecibido.RegistroCajaPuntoReporta.IdCentroServiciosVenta);

                            List<CAResumeCierrePuntoDC> cierreCeroInfo = new List<CAResumeCierrePuntoDC>();

                            //Lleno la info del cierre del punto en Cero
                            infoCierreCentroSvc = LlenarInfoCierrePuntoAutomatico(cierreCeroInfo, dineroRecibido.RegistroCajaPuntoReporta.IdCentroServiciosVenta,
                                                                                     dineroRecibido.RegistroCajaAgencia.InfoAperturaCaja.NombresUsuario,
                                                                                     cierreCero);

                            //lleno info para reportar dinero y cierro caja ppal en Cero
                            dineroRecibido.ValorTotalEnviado = infoCierreCentroSvc.SaldoFinalEfectivo;
                            dineroRecibido.IdCierreCentroServicios = CACierreCaja.CACierreCaja.Instancia.CerrarCajaPuntoCentroServcio(infoCierreCentroSvc);
                        }
                        else
                        {
                            //Se copia la data Inicial para realizar el cierre automatico.
                            CARegistroTransacCajaDC movimientoCaja = dineroRecibido.RegistroCajaPuntoReporta;

                            CACierreCaja.CACierreCaja.Instancia.CerrarCajasAutomaticamentePorCentroSvc(movimientoCaja);

                            //Lleno la info del cierre del punto
                            infoCierreCentroSvc = LlenarInfoCierrePuntoAutomatico(ObtenerResumenCierrePunto(dineroRecibido.RegistroCajaPuntoReporta.IdCentroServiciosVenta, 0),
                                                                                     dineroRecibido.RegistroCajaPuntoReporta.IdCentroServiciosVenta,
                                                                                     dineroRecibido.RegistroCajaAgencia.InfoAperturaCaja.NombresUsuario);

                            //lleno info para reportar dinero y cierro caja ppal
                            dineroRecibido.ValorTotalEnviado = infoCierreCentroSvc.SaldoFinalEfectivo;
                            dineroRecibido.IdCierreCentroServicios = CACierreCaja.CACierreCaja.Instancia.CerrarCajaPuntoCentroServcio(infoCierreCentroSvc);
                        }

                        trans.Complete();
                    }
                }
                else
                {
                    //lleno la Info para Reportar
                    dineroRecibido.ValorTotalEnviado = cierreNoReport.SaldoFinalEfectivo;
                    dineroRecibido.IdCierreCentroServicios = cierreNoReport.IdCierreCentroServicio;
                }

                //registro Transaccion de Reporte Dinero
                using (TransactionScope trans = new TransactionScope())
                {
                    ValidarGuardarSuministroBolsaSeguridad(dineroRecibido.BolsaSeguridad,
                                                            dineroRecibido.RegistroCajaPuntoReporta.IdCentroServiciosVenta);

                    RegistrarMovimientoPorRecoleccionDinero(dineroRecibido);

                    trans.Complete();
                }
            }
            else
            {
                using (TransactionScope trans = new TransactionScope())
                {
                    RegistrarMovimientoPorRecoleccionDinero(dineroRecibido);

                    trans.Complete();
                }
            }
        }

        //procesos de registro en caja de las transacciones de descargue del dinero recibido por parte de la agencia.
        private void RegistrarMovimientoPorRecoleccionDinero(CARecoleccionDineroPuntoDC dineroRecibido)
        {
            //registrar movimiento de dinero en el punto que reportó
            CAIdTransaccionesCajaDC idTranPunto = CAApertura.Instancia.RegistrarVentaConNuevaApertura(dineroRecibido.RegistroCajaPuntoReporta);

            //registrar movimiento en la agencia que está recibiendo el dinero
            CAIdTransaccionesCajaDC idTranAgencia = CAApertura.Instancia.RegistrarVenta(dineroRecibido.RegistroCajaAgencia);

            //registrar Transaccion en CentroSvcCentroSvcMov_CAJ
            RegistrarMovCentroSvcCentroSvc(dineroRecibido, idTranPunto, idTranAgencia);

            //Registrar Transaccion en ReporteMensajeroCaja_CAJ
            RegistrarMovMensajero(dineroRecibido, idTranAgencia);

            //registrar el dinero que recibe la agencia
            CARepositorioCaja.Instancia.AdicionarDineroAgencia(dineroRecibido);

            /*  Cerrar la apertura del punto al cual se le aplicó el egreso  */

            //cerrar la apertura recien hecha para la operación de egreso sobre el punto que reporta
            CACierreCaja.CACierreCaja.Instancia.CerrarCaja(
              dineroRecibido.RegistroCajaPuntoReporta.InfoAperturaCaja.IdCodigoUsuario,
              dineroRecibido.RegistroCajaPuntoReporta.InfoAperturaCaja.IdAperturaCaja,
              dineroRecibido.RegistroCajaPuntoReporta.IdCentroServiciosVenta,
              dineroRecibido.RegistroCajaPuntoReporta.InfoAperturaCaja.IdCaja);

            //Obtener el ultimo cierre del punto que reporta
            CACierreCentroServicioDC dataCierre = CARepositorioCaja.Instancia.ObtenerUltimoCierrePunto(dineroRecibido.RegistroCajaPuntoReporta.IdCentroServiciosVenta);

            //calcular el saldo final descontando el egreso que se acaba de aplicar por el dinero que la agencia le recibe al punto
            decimal saldoFinalEfectivo = dataCierre.SaldoAnteriorEfectivo - dineroRecibido.RegistroCajaPuntoReporta.ValorTotal + dataCierre.BaseInicial;
            decimal saldoAnteriorEfectivo = dataCierre.SaldoAnteriorEfectivo;

            CierreCentroServicios_CAJ cierreDelPunto = new CierreCentroServicios_CAJ()
              {
                  CCS_BaseInicial = dataCierre.BaseInicial,
                  CCS_CreadoPor = dataCierre.UsuarioCierraPunto,
                  CCS_FechaGrabacion = DateTime.Now,
                  CCS_IdCentroServicios = dataCierre.IdCentroServicio,
                  CCS_SaldoAnteriorEfectivo = saldoAnteriorEfectivo,
                  CCS_SaldoFinalEfectivo = saldoFinalEfectivo,
                  CCS_TotalEgresosEfectivo = dineroRecibido.RegistroCajaPuntoReporta.ValorTotal,
                  CCS_TotalEgresosOtrasFormas = 0,
                  CCS_TotalIngresosEfectivo = 0,
                  CCS_TotalIngresosOtrasFormas = 0,
                  CCS_CantidadGirosNoPagados = dataCierre.InfoGirosNoPagosCentroSvc.CantidadPagos,
                  CCS_ValorGirosNoPagados = dataCierre.InfoGirosNoPagosCentroSvc.SumatoriaPagos,
                  CCS_EstaReportado = true       //marcar cierre como reportado
              };

            //adicionar el cierre del punto que hace el reporte de dinero
            long idCierreCentroSvc = CARepositorioCaja.Instancia.AdicionarCierreCentroServcio(cierreDelPunto);

            //marcar el cierre como reportado y asociar el cierre de la caja 0 del punto con el cierre del centro de servicios
            CARepositorioCaja.Instancia.AsociarCierreCajaConCierreCentroSvc(
              dineroRecibido.RegistroCajaPuntoReporta.InfoAperturaCaja.IdAperturaCaja,
              idCierreCentroSvc);
        }

        /// <summary>
        ///Registra la transaccion en la tbla reporte Mensajero
        /// </summary>
        /// <param name="dineroRecibido"></param>
        /// <param name="idTranAgencia"></param>
        private void RegistrarMovMensajero(CARecoleccionDineroPuntoDC dineroRecibido, CAIdTransaccionesCajaDC idTranAgencia)
        {
            dineroRecibido.RegistroMovimentoAReporteMensajero = new CAReporteMensajeroCajaDC()
            {
                IdRegistroTransDetalleCaja = idTranAgencia.IdTransaccionCajaDtll.FirstOrDefault(),
                Mensajero = new Servicios.ContratoDatos.OperacionUrbana.OUNombresMensajeroDC()
                {
                    IdPersonaInterna = dineroRecibido.MensajeroPunto.IdPersonaInterna,
                    NombreApellido = dineroRecibido.MensajeroPunto.NombreApellido,
                }
            };

            //Registra la transaccion en la tbla reporte Mensajero
            CAMensajero.Instancia.AdicionarReporteMensajero(dineroRecibido.RegistroMovimentoAReporteMensajero);
        }

        /// <summary>
        ///registrar Transaccion en CentroSvcCentroSvcMov_CAJ
        /// </summary>
        /// <param name="dineroRecibido"></param>
        /// <param name="idTranPunto"></param>
        /// <param name="idTranAgencia"></param>
        private void RegistrarMovCentroSvcCentroSvc(CARecoleccionDineroPuntoDC dineroRecibido, CAIdTransaccionesCajaDC idTranPunto, CAIdTransaccionesCajaDC idTranAgencia)
        {
            dineroRecibido.RegistroMovEntreCentroSvc = new CAMovCentroSvcCentroSvcDC()
            {
                IdCentroServicioOrigen = dineroRecibido.RegistroCajaPuntoReporta.IdCentroServiciosVenta,
                NombreCentroServicioOrigen = dineroRecibido.RegistroCajaPuntoReporta.NombreCentroServiciosVenta,
                IdCentroServicioDestino = dineroRecibido.RegistroCajaAgencia.IdCentroServiciosVenta,
                NombreCentroServicioDestino = dineroRecibido.RegistroCajaAgencia.NombreCentroServiciosVenta,
                IdRegistroTxOrigen = idTranPunto.IdTransaccionCaja,
                IdRegistroTxDestino = idTranAgencia.IdTransaccionCaja,
                BolsaSeguridad = dineroRecibido.BolsaSeguridad,
                UsuarioRegistra = ControllerContext.Current.Usuario
            };

            // Adicionar los movimientos genrados entre Col y Punto ó Agencia.
            CARepositorioCaja.Instancia.AdicionarMovRacolAgencia(dineroRecibido.RegistroMovEntreCentroSvc);
        }

        /// <summary>
        /// Adjunto la info necesaria para cerrar el punto y
        /// retorno la clase para cerrar el punto
        /// </summary>
        /// <param name="infoCierrePunto"></param>
        /// <param name="idCentroServicio"></param>
        /// <param name="usuario"></param>
        /// <returns>Clase para cerrar el punto </returns>
        public CACierreCentroServicioDC LlenarInfoCierrePuntoAutomatico(List<CAResumeCierrePuntoDC> infoCierrePunto,
                                                                       long idCentroServicio, string usuario,
                                                                        CACierreCentroServicioDC dataCierre = null)
        {
            CASaldosFinalesDC SaldosFinales = new CASaldosFinalesDC()
            {
                BaseInicial = 0,
                SaldoAnteriorEfectivo = 0,
                SaldoFinalEfectivo = 0,
                TotalAEmpresa = 0,
                TotalEgresosEfectivo = 0,
                TotalIngresosEfectivo = 0
            };
            CACierreCentroServicioDC cierreDelPunto = null;
            decimal TotalIngresos = 0;
            decimal TotalEgresos = 0;

            //consolido informacion para cerrar el punto
            if (infoCierrePunto != null && infoCierrePunto.Count > 0)
            {
                List<CAResumeCierrePuntoDC> SaldoFin = infoCierrePunto.Where(forma => forma.IdFormaPago == 1).ToList();
                if (SaldoFin.Count > 0)
                {
                    SaldosFinales = SaldoFin.GroupBy(r => r.IdFormaPago).Select(r => new CASaldosFinalesDC()
                    {
                        BaseInicial = r.First().BaseInicial,
                        SaldoAnteriorEfectivo = r.First().SaldoAnteriorEfectivo,
                        TotalIngresosEfectivo = r.Sum(ingsum => ingsum.Ingreso),
                        TotalEgresosEfectivo = r.Sum(egrsum => egrsum.Egreso),
                        SaldoFinalEfectivo = r.Sum(ingsum => ingsum.Ingreso) - r.Sum(egrsum => egrsum.Egreso) + r.First().SaldoAnteriorEfectivo + r.First().BaseInicial,
                        TotalAEmpresa = r.Sum(ingsum => ingsum.Ingreso) - r.Sum(egrsum => egrsum.Egreso) + r.First().SaldoAnteriorEfectivo
                    }).First();
                }

                TotalIngresos = infoCierrePunto.GroupBy(r => r.IdPuntoAtencion).Select(s => s.Sum(sum => sum.Ingreso)).First();
                TotalEgresos = infoCierrePunto.GroupBy(r => r.IdPuntoAtencion).Select(s => s.Sum(sum => sum.Egreso)).First();

                //lleno data para cerrar punto
                cierreDelPunto = new CACierreCentroServicioDC()
                {
                    BaseInicial = SaldosFinales.BaseInicial,
                    FechaCierre = DateTime.Now,
                    IdCentroServicio = idCentroServicio,

                    // El saldo Anterior a Insertar es el Saldo final efectivo
                    SaldoAnteriorEfectivo = SaldosFinales.SaldoAnteriorEfectivo,
                    SaldoFinalEfectivo = SaldosFinales.SaldoFinalEfectivo,
                    TotalEgresosEfectivo = SaldosFinales.TotalEgresosEfectivo,
                    TotalIngresosEfectivo = SaldosFinales.TotalIngresosEfectivo,
                    TotalIngresosOtrasFormas = TotalIngresos - SaldosFinales.TotalIngresosEfectivo,
                    TotalEgresosOtrasFormas = TotalEgresos - SaldosFinales.TotalEgresosEfectivo,
                    UsuarioCierraPunto = usuario == null ? ControllerContext.Current.Usuario : usuario,
                    CajasPuntoReportadas = infoCierrePunto.GroupBy(r => r.IdCierreCaja).Select(q => new CAResumenCierreCajaDC()
                    {
                        IdCierreCaja = q.First().IdCierreCaja
                    }).ToList(),
                    InfoGirosNoPagosCentroSvc = new PGTotalPagosDC()
                      {
                          CantidadPagos = infoCierrePunto.First().InfoGirosNoPagosCentroSvc.CantidadPagos,
                          SumatoriaPagos = infoCierrePunto.First().InfoGirosNoPagosCentroSvc.SumatoriaPagos
                      },
                };
            }
            else
            {
                cierreDelPunto = new CACierreCentroServicioDC()
                {
                    BaseInicial = dataCierre.BaseInicial,
                    FechaCierre = DateTime.Now,
                    IdCentroServicio = dataCierre.IdCentroServicio,

                    // El saldo Anterior a Insertar es el Saldo final efectivo
                    SaldoAnteriorEfectivo = dataCierre.SaldoAnteriorEfectivo,
                    SaldoFinalEfectivo = dataCierre.SaldoFinalEfectivo,
                    TotalEgresosEfectivo = 0,
                    TotalIngresosEfectivo = 0,
                    TotalIngresosOtrasFormas = 0,
                    TotalEgresosOtrasFormas = 0,
                    UsuarioCierraPunto = dataCierre.UsuarioCierraPunto == null ? usuario : dataCierre.UsuarioCierraPunto,
                    CajasPuntoReportadas = dataCierre.CajasPuntoReportadas,
                    InfoGirosNoPagosCentroSvc = new PGTotalPagosDC()
                    {
                        CantidadPagos = dataCierre.InfoGirosNoPagosCentroSvc.CantidadPagos,
                        SumatoriaPagos = dataCierre.InfoGirosNoPagosCentroSvc.SumatoriaPagos
                    },
                };
            }

            //retorno la info del cierre del punto
            return cierreDelPunto;
        }

        /// <summary>
        /// Obtiene el dinero puntos reportados.
        /// </summary>
        /// <param name="idCentroServicio">es el id centro servicio.</param>
        /// <returns>la lista de los puntos con el dinero reportado</returns>
        public List<CARecoleccionDineroPuntoDC> ObtenerDineroReportadoPuntos(IDictionary<string, string> filtro, long idCentroServicio)
        {
            return CARepositorioCaja.Instancia.ObtenerDineroReportadoPuntos(filtro, idCentroServicio);
        }

        /// <summary>
        /// Arma la informacion necesaria para cerrar el Punto ó Centro de Servicio.
        /// </summary>
        /// <param name="cierrePunto">The cierre punto.</param>
        /// <returns>La info para Cerrar el Punto</returns>
        public CACierreCentroServicioDC ArmarCierreCentroServicio(List<CAResumeCierrePuntoDC> cierrePunto, string usuario)
        {
            CAResumeCierrePuntoDC totalComisiones;
            CASaldosFinalesDC saldosFinales;
            List<CAResumeCierrePuntoDC> infoCierrePunto;
            decimal totalIngresos;
            decimal totalEgresos;

            totalComisiones = cierrePunto.GroupBy(r => r.IdPuntoAtencion).Select(r => new CAResumeCierrePuntoDC()
            {
                TotalComisionAgenciaResponsable = r.Sum(sum => sum.TotalComisionAgenciaResponsable),
                TotalComisionCentroServicios = r.Sum(sum => sum.TotalComisionCentroServicios),
                TotalComisionEmpresa = r.Sum(sum => sum.TotalComisionEmpresa)
            }).First();

            List<CAResumeCierrePuntoDC> SaldoFin = cierrePunto.Where(forma => forma.IdFormaPago == 1).ToList();
            if (SaldoFin.Count > 0)
            {
                saldosFinales = SaldoFin.GroupBy(r => r.IdFormaPago).Select(r => new CASaldosFinalesDC()
                {
                    BaseInicial = r.First().BaseInicial,
                    SaldoAnteriorEfectivo = r.First().SaldoAnteriorEfectivo,
                    TotalIngresosEfectivo = r.Sum(ingsum => ingsum.Ingreso),
                    TotalEgresosEfectivo = r.Sum(egrsum => egrsum.Egreso),
                    SaldoFinalEfectivo = r.Sum(ingsum => ingsum.Ingreso) - r.Sum(egrsum => egrsum.Egreso),
                    TotalAEmpresa = r.Sum(ingsum => ingsum.Ingreso) - r.Sum(egrsum => egrsum.Egreso) - r.First().BaseInicial
                }).First();
            }
            else
            {
                saldosFinales = new CASaldosFinalesDC()
                {
                    BaseInicial = Convert.ToDecimal(CAConstantesCaja.VALOR_CERO),
                    SaldoAnteriorEfectivo = Convert.ToDecimal(CAConstantesCaja.VALOR_CERO),
                    TotalIngresosEfectivo = Convert.ToDecimal(CAConstantesCaja.VALOR_CERO),
                    TotalEgresosEfectivo = Convert.ToDecimal(CAConstantesCaja.VALOR_CERO),
                    SaldoFinalEfectivo = Convert.ToDecimal(CAConstantesCaja.VALOR_CERO),
                    TotalAEmpresa = Convert.ToDecimal(CAConstantesCaja.VALOR_CERO),
                };
            }
            infoCierrePunto = new List<CAResumeCierrePuntoDC>(cierrePunto);
            totalIngresos = cierrePunto.GroupBy(r => r.IdPuntoAtencion).Select(s => s.Sum(sum => sum.Ingreso)).First();
            totalEgresos = cierrePunto.GroupBy(r => r.IdPuntoAtencion).Select(s => s.Sum(sum => sum.Egreso)).First();

            //Llenado de la Info para el Cierre del Punto
            CACierreCentroServicioDC InfoCierreCentroServicio = new CACierreCentroServicioDC()
            {
                BaseInicial = saldosFinales.BaseInicial,
                FechaCierre = DateTime.Now,
                IdCentroServicio = cierrePunto.FirstOrDefault().IdPuntoAtencion,
                SaldoAnteriorEfectivo = saldosFinales.SaldoAnteriorEfectivo,
                SaldoFinalEfectivo = saldosFinales.SaldoFinalEfectivo,
                TotalEgresosEfectivo = saldosFinales.TotalEgresosEfectivo,
                TotalIngresosEfectivo = saldosFinales.TotalIngresosEfectivo,
                TotalIngresosOtrasFormas = totalIngresos - saldosFinales.TotalIngresosEfectivo,
                TotalEgresosOtrasFormas = totalEgresos - saldosFinales.TotalEgresosEfectivo,
                UsuarioCierraPunto = usuario.ToString(),
                CajasPuntoReportadas = infoCierrePunto.GroupBy(r => r.IdCierreCaja).Select(q => new CAResumenCierreCajaDC()
                {
                    IdCierreCaja = q.First().IdCierreCaja,
                }).ToList()
            };

            return InfoCierreCentroServicio;
        }

        /// <summary>
        /// Metodo que realiza la transaccion de
        /// obtener la dupla y realiza la transaccion
        /// entre Caja Ppal y Caja Auxiliar
        /// </summary>
        /// <param name="infoTransaccion"></param>
        public void TransladarDineroEntreCajas(CAOperaRacolBancoEmpresaDC infoTransaccion)
        {
            //Rafram se valida transaccion entre Cajas ppal a Auxiliar para traer la dupla
            if (infoTransaccion.RegistroCentrSvcMenor != null && infoTransaccion.RegistroCentrSvcMenor.EsTransladoEntreCajas)
            {
                infoTransaccion.RegistroCentrSvcMenor.RegistrosTransacDetallesCaja.ForEach(trans =>
                {
                    if (trans.ConceptoCaja == null)
                    {
                        CAConceptoCajaDC valorDupla = CACaja.Instancia.ObtenerDuplaConcepto((int)CAEnumConceptosCaja.TRANS_DINERO_ENTRE_CAJAS);
                        trans.ConceptoCaja = new CAConceptoCajaDC()
                        {
                            IdConceptoCaja = valorDupla.IdConceptoCaja,
                            EsIngreso = valorDupla.EsIngreso,
                        };
                        trans.ConceptoEsIngreso = valorDupla.EsIngreso;
                    }
                });
            }
            CACajaFinanciera.Instancia.TransaccionRacolBancoCasaMatriz(infoTransaccion);
        }

        /// <summary>
        /// Obtiene la Info de la Pantalla de
        /// RegistroEnvioAgencia
        /// </summary>
        /// <param name="idCentroServicio">idcentroServicio</param>
        /// <returns>info de registroEnvio</returns>
        public CARegistrarEnvioAgenciaDC ObtenerInfoRegistroEnvioAgencia(long idCentroServicio)
        {
            CARegistrarEnvioAgenciaDC infoEnvioAgencia = new CARegistrarEnvioAgenciaDC();

            infoEnvioAgencia.ValorAEnviarPorPunto = ObtenerValorEmpresa(idCentroServicio);

            if (Math.Round(infoEnvioAgencia.ValorAEnviarPorPunto, 2) > 0)
            {
                infoEnvioAgencia.InfoUltimoCierrePunto = ObtenerUltimoCierrePunto(idCentroServicio);
                infoEnvioAgencia.ListMensajerosAgencia = CACaja.Instancia.ObtenerMensajerosPuntoCentroServicio(idCentroServicio);
            }

            return infoEnvioAgencia;
        }
    }
}