using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using CO.Servidor.Cajas.Apertura;
using CO.Servidor.Cajas.CACierreCaja;
using CO.Servidor.Cajas.CajaVenta;
using CO.Servidor.Dominio.Comun.Area;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.Suministros;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros;
using CO.Servidor.Servicios.ContratoDatos.Area;
using CO.Servidor.Servicios.ContratoDatos.Cajas;
using CO.Servidor.Servicios.ContratoDatos.Cajas.Cierres;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using CO.Servidor.Servicios.ContratoDatos.GestionCajas;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using Framework.Servidor.Servicios.ContratoDatos.Parametros.Consecutivos;
using Framework.Servidor.Servicios.ContratoDatos.Seguridad;
using Framework.Servidor.Excepciones;
using CO.Servidor.Dominio.Comun.Tarifas;
using CO.Servidor.Cajas.Datos;

namespace CO.Servidor.Cajas
{
    /// <summary>
    /// Fachada para controles visuales de cajas
    /// </summary>
    public class CAAdministradorCajas
    {
        #region Atributos

        /// <summary>
        /// creo la Instancia
        /// </summary>
        private static CAAdministradorCajas instancia = new CAAdministradorCajas();
        private CATransaccionCaja registroTransaccion;
        #endregion Atributos

        #region Instancia

        /// <summary>
        /// Obtengo la instancia.
        /// </summary>
        public static CAAdministradorCajas Instancia
        {
            get { return CAAdministradorCajas.instancia; }
        }

        private CAAdministradorCajas()
        {
            registroTransaccion=new CATransaccionCaja();
        }

        #endregion Instancia

        #region Caja Auxiliar

        /// <summary>
        /// Obtener información complementaria para el cierre de una caja
        /// </summary>
        /// <param name="idCierre">Identificacdor del cierre de caja</param>
        /// <returns>Información complementaria del cierre de caja</returns>
        public CAInfoComplementariaCierreCajaDC ObtenerInfoComplementariaCierreCaja(long idCierreCaja)
        {
            return CACierreCaja.CACierreCaja.Instancia.ObtenerInfoComplementariaCierreCaja(idCierreCaja);
        }

        /// <summary>
        /// Cierra la caja y Totaliza Procesos.
        /// </summary>
        /// <param name="idApertura">Es el id de la apertura.</param>
        /// <param name="idPunto">Es el id del punto.</param>
        public void CerrarCaja(long idCodigoUsuario, long idApertura, long idPunto, int idCaja)
        {
            CACierreCaja.CACierreCaja.Instancia.CerrarCaja(idCodigoUsuario, idApertura, idPunto, idCaja);
        }

        /// <summary>
        /// Obtener la info de la caja para cierre.
        /// </summary>
        /// <param name="idCodigoUsuario">The id codigo usuario.</param>
        /// <param name="idFormaPago">The id forma pago.</param>
        /// <param name="operador">The operador.</param>
        /// <returns></returns>
        public List<CACierreCajaDC> ObtenerInfoCierreCaja(long idUsuario, short idFormaPago, string operador, long idCentroServicio, int idCaja)
        {
            return CACaja.Instancia.ObtenerInfoCierreCaja(idUsuario, idFormaPago, operador, idCentroServicio, idCaja);
        }
      

        /// <summary>
        /// Obtener Cierres del Cajero.
        /// </summary>
        /// <param name="idCodigoUsuario">The id codigo usuario.</param>
        /// <returns></returns>
        public List<CACierreCajaDC> ObtenerCierresCajero(long idCodigoUsuario, long idCentroServicio, DateTime fechaCierre, int indicePagina, int tamanoPagina)
        {
            return CACierreCaja.CACierreCaja.Instancia.ObtenerCierresCajero(idCodigoUsuario, idCentroServicio, fechaCierre, indicePagina, tamanoPagina);
        }

        /// <summary>
        /// Obtiene las Cajas Cerradas y Pendientes
        /// para Cerrar de un punto o centro de Servicio
        /// </summary>
        /// <param name="idCentroSrv"></param>
        /// <param name="idFormaPago"></param>
        /// <param name="operador"></param>
        /// <returns></returns>
        public List<CACierreCajaDC> ObtenerCajasAReportarCajeroPrincipal(long idCentroSrv, short idFormaPago, string operador)
        {
            return CACierreCaja.CACierreCaja.Instancia.ObtenerCajasAReportarCajeroPrincipal(idCentroSrv, idFormaPago, operador);
        }

        #endregion Caja Auxiliar

        #region Caja Principal

        /// <summary>
        /// Obtener las cajas para cerrar.
        /// </summary>
        /// <param name="idPuntoCentroServicio">Es el id punto centro servicio.</param>
        /// <returns>Las cajas que se pueden Cerrar</returns>
        public List<CACierreCajaDC> ObtenerCajasParaCerrar(long idPuntoCentroServicio, short idFormaPago, string operador)
        {
            return CACajaPrincipal.Instancia.ObtenerCajasParaCerrar(idPuntoCentroServicio, idFormaPago, operador);
        }

        /// <summary>
        /// Obtiene el resumen por concepto de caja de un punto o centro de
        /// servicio para cerrar.
        /// </summary>
        /// <param name="idPuntoCentroServicio">Es el id del punto ó centro de servicio.</param>
        /// <returns>la lista de los conceptos agrupados y totalizados para cerrar el punto</returns>
        public List<CAResumeCierrePuntoDC> ObtenerResumenCierrePunto(long idPuntoCentroServicio, long idCierrePuntoAsociado)
        {
            return CACajaPrincipal.Instancia.ObtenerResumenCierrePunto(idPuntoCentroServicio, idCierrePuntoAsociado);
        }

        /// <summary>
        /// Obtener el resumen para el cierre de de caja principal
        /// </summary>
        /// <param name="idCentroServicios">Identificador del centro de servicios</param>
        /// <param name="idAperturaCaja">Identificador de la apertura de caja</param>
        /// <param name="idCierrePuntoAsociado">Identificador del cierre de centro de servicios, para el caso de consulta de un cierre ya hecho</param>
        /// <returns>Resumen del cierre o transacciones de una apertura</returns>
        public CAConsolidadoCierreDC ObtenerResumenCierreCajaPrincipal(long idCentroServicios, long idAperturaCaja, long idCierrePuntoAsociado)
        {
            return CACajaPrincipal.Instancia.ObtenerResumenCierreCajaPrincipal(idCentroServicios, idAperturaCaja, idCierrePuntoAsociado);
        }

        /// <summary>
        /// Obtiene el ultimo cierre del punto.
        /// </summary>
        /// <returns></returns>
        public CACierreCentroServicioDC ObtenerUltimoCierrePunto(long idPuntoCentroServicio)
        {
            return CACajaPrincipal.Instancia.ObtenerUltimoCierrePunto(idPuntoCentroServicio);
        }

        /// <summary>
        /// Obtienes the cierres punto.
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
            return CACajaPrincipal.Instancia.ObtenerCierresPunto(filtro, campoOrdenamiento, indicePagina, registrosPorPagina,
                                                                   ordenamientoAscendente, out totalRegistros, idPuntoCentroServicio);
        }

        /// <summary>
        /// Cierra la Caja del Punto ó Centro de Servicio y
        /// actualiza el Id de Cierre de Punto en las Cajas respectivas
        /// como reportado.
        /// </summary>
        /// <param name="cajasPuntoCentroServicio">Informacion de Cierre de punto + lista de cajas del punto</param>
        public long CerrarCajaPuntoCentroServcio(CACierreCentroServicioDC cierrePuntoCentroServicio)
        {
            return CACierreCaja.CACierreCaja.Instancia.CerrarCajaPuntoCentroServcio(cierrePuntoCentroServicio);
        }

        /// <summary>
        /// Obtiene el Valor a Enviar a la Empresa
        /// </summary>
        /// <param name="idCentroServicio">The id centro servicio.</param>
        public decimal ObtenerValorEmpresa(long idCentroServicio)
        {
            return CACajaPrincipal.Instancia.ObtenerValorEmpresa(idCentroServicio);
        }

        /// <summary>
        /// Inserta el dinero envio empresa.
        /// </summary>
        /// <param name="EnvioDineroAgencia">El envio dinero agencia.</param>
        public void InsertarDineroEnvioEmpresa(CARecoleccionDineroPuntoDC EnvioDineroAgencia)
        {
            CACajaPrincipal.Instancia.InsertarDineroEnvioEmpresa(EnvioDineroAgencia);
        }

        /// <summary>
        /// Adiciona el dinero recibido por la Empresa
        /// de una agencio o punto.
        /// </summary>
        /// <param name="dineroRecibido">The dinero recibido.</param>
        public void AdicionarDineroAgencia(CARecoleccionDineroPuntoDC dineroRecibido)
        {
            CACajaPrincipal.Instancia.AdicionarDineroAgencia(dineroRecibido);
        }

        /// <summary>
        /// Obtiene el dinero puntos reportados.
        /// </summary>
        /// <param name="idCentroServicio">es el id centro servicio.</param>
        /// <returns>la lista de los puntos con el dinero reportado</returns>
        public List<CARecoleccionDineroPuntoDC> ObtenerDineroReportadoPuntos(IDictionary<string, string> filtro, long idCentroServicio)
        {
            return CACajaPrincipal.Instancia.ObtenerDineroReportadoPuntos(filtro, idCentroServicio);
        }

        /// <summary>
        /// Cierre automatico de Cajas Auxiliares ejecuta por el
        /// Cajero ppal para cerrar la Caja del Punto o centro de Servicio.
        /// </summary>
        /// <param name="idCentroServicio">The id centro servicio.</param>
        /// <param name="idUsuario">The id usuario.</param>
        public void CerrarCajasAutomaticamentePorCentroSvc(CARegistroTransacCajaDC movimientoCaja)
        {
            CACierreCaja.CACierreCaja.Instancia.CerrarCajasAutomaticamentePorCentroSvc(movimientoCaja);
        }

        /// <summary>
        /// Proceso de Cierre automatico de Puntos
        /// Centros de Servicio
        /// </summary>
        public void CierreAutomaticoCajasAgencias()
        {
            CACierresAutomaticos.Instancia.CierreAutomaticoCajasAgencias();
        }

        /// <summary>
        /// Proceso de Apertura Automatica de Puntos que
        /// no tuvieron Movimientos
        /// </summary>
        //public void ProcesoCierreAutomatico()
        //{
        //    CACierresAutomaticos.Instancia.ProcesoCierreAutomatico();
        //}

        /// <summary>
        /// Metodo que realiza la transaccion de
        /// obtener la dupla y realiza la transaccion
        /// entre Caja Ppal y Caja Auxiliar
        /// </summary>
        /// <param name="infoTransaccion"></param>
        public void TransladarDineroEntreCajas(CAOperaRacolBancoEmpresaDC infoTransaccion)
        {
            CACajaPrincipal.Instancia.TransladarDineroEntreCajas(infoTransaccion);
        }

        /// <summary>
        /// Obtiene la Info de la Pantalla de
        /// RegistroEnvioAgencia
        /// </summary>
        /// <param name="idCentroServicio">idcentroServicio</param>
        /// <returns>info de registroEnvio</returns>
        public CARegistrarEnvioAgenciaDC ObtenerInfoRegistroEnvioAgencia(long idCentroServicio)
        {
            return CACajaPrincipal.Instancia.ObtenerInfoRegistroEnvioAgencia(idCentroServicio);
        }

        #endregion Caja Principal

        #region Movimientos Caja

        /// <summary>
        /// Adicionar el movimiento caja.
        /// </summary>
        /// <param name="movimientoCaja">The movimiento caja.</param>
        /// <param name="tipoComprobante">Indica el tipo de comprobante que quiere agregar a la operación, si lo pasa nulo, no se tiene en cuenta</param>
        public CAIdTransaccionesCajaDC AdicionarMovimientoCaja(CARegistroTransacCajaDC movimientoCaja, PAEnumConsecutivos? tipoComprobante)
        {
            return CAApertura.Instancia.RegistrarVentaRequiereTipoComprobante(movimientoCaja, tipoComprobante);
        }

        #endregion Movimientos Caja

        #region Caja Mensajero

        /// <summary>
        /// Obtiene los mensajeros de apoyo para el envio de dinero.
        /// </summary>
        /// <param name="idCentroServicio">The id centro servicio.</param>
        /// <returns></returns>
        public List<OUNombresMensajeroDC> ObtenerMensajerosPuntoCentroServicio(long idCentroServicio)
        {
            return CACaja.Instancia.ObtenerMensajerosPuntoCentroServicio(idCentroServicio);
        }

        /// <summary>
        /// Obtiene los mensajeros de apoyo para el envio de dinero.
        /// </summary>
        /// <param name="idCentroServicio">The id centro servicio.</param>
        /// <returns></returns>
        public IEnumerable<OUNombresMensajeroDC> ObtenerMensajerosPuntoCentroServicioPag(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina,
                                                                                int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros,
                                                                                long idCentroServicio)
        {
            return CACaja.Instancia.ObtenerMensajerosPuntoCentroServicioPag(filtro, campoOrdenamiento, indicePagina, registrosPorPagina,
                                                                              ordenamientoAscendente, out totalRegistros, idCentroServicio);
        }

        /// <summary>
        /// Adiciona las Transacciones de un Mensajero.
        /// </summary>
        /// <param name="registroMensajero">Clase Cuenta Mensajero.</param>
        public long AdicionarTransaccMensajero(CACuentaMensajeroDC registroMensajero)
        {
            return CAMensajero.Instancia.AdicionarTransaccMensajero(registroMensajero);
        }

        /// <summary>
        /// Adicionar Reporte del Mensajero.
        /// </summary>
        /// <param name="reportMensajero">Clase report mensajero.</param>
        public void AdicionarReporteMensajero(CAReporteMensajeroCajaDC reportMensajero)
        {
            CAMensajero.Instancia.AdicionarReporteMensajero(reportMensajero);
        }

        /// <summary>
        /// Obtiene todos los reportes de caja de un mensajero
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public List<CAReporteMensajeroCajaDC> ObtenerReportesMensajeros(long idMensajero)
        {
            return CAMensajero.Instancia.ObtenerReportesMensajeros(idMensajero);
        }
        
        /// <summary>
        /// Obtiene todos los reportes de caja de un mensajero por comprobante para imprimir
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <param name="numComprobante"></param>
        /// <returns></returns>
        public CADatosImpCompMensajeroDC ObtenerReportesMensajerosImprimir(long idMensajero, string numComprobante)
        {
            return CAMensajero.Instancia.ObtenerReportesMensajerosImprimir(idMensajero, numComprobante);
        }


        /// <summary>
        /// Procedimientos para el descuento y generacion de la informacion
        /// para el formato de descuento de nomina
        /// </summary>
        /// <param name="registroMensajero">datos del mensajero a descontar</param>
        /// <returns>info de impresion formato</returns>
        public CADatosImpCompMensajeroDC DescuentoNominaMensajero(CACuentaMensajeroDC registroMensajero)
        {
            long consecutivoComprobante = 0;
            consecutivoComprobante = CACaja.Instancia.ObtenerConsecutivoComprobateCajaEgreso();

            registroMensajero.NumeroDocumento = consecutivoComprobante;
            //Agregar el movimiento en la cuenta del mensajero
            AdicionarTransaccMensajero(registroMensajero);

            if (registroMensajero.CentroLogContrapartida != null)
            {
                //Agregar el movimiento en la caja del centrol logistico
                CAConceptoCajaDC conceptoCS = new CAConceptoCajaDC()
                {
                    IdConceptoCaja = registroMensajero.ConceptoCajaMensajero.IdConceptoCaja,
                    Descripcion = registroMensajero.ConceptoCajaMensajero.Descripcion,
                    Nombre = registroMensajero.ConceptoCajaMensajero.Nombre,
                    EsIngreso = !registroMensajero.ConceptoCajaMensajero.EsIngreso,
                    EsEgreso = !registroMensajero.ConceptoCajaMensajero.EsEgreso
                };
                CARegistroTransacCajaDC registroCajaCol = ArmarTransaccionCS(registroMensajero.CentroLogContrapartida, conceptoCS, registroMensajero.Valor, registroMensajero.Observaciones);
                registroCajaCol.RegistrosTransacDetallesCaja.FirstOrDefault().NumeroComprobante = consecutivoComprobante.ToString();
                CAIdTransaccionesCajaDC idTransCol= registroTransaccion.AdicionarMovimientoCaja(registroCajaCol);

                //Registra operacion que relaciona la transacción entre el col y el mensajero
                CAReporteMensajeroCajaDC transMensajeroCS = new CAReporteMensajeroCajaDC()
                {
                    FechaGrabacion = DateTime.Now,
                    IdRegistroTransDetalleCaja = idTransCol.IdTransaccionCajaDtll.FirstOrDefault(),
                    Mensajero = registroMensajero.Mensajero,
                    UsuarioRegistro = ControllerContext.Current.Usuario
                };

                CARepositorioCaja.Instancia.AdicionarReporteMensajero(transMensajeroCS);
            }

            IARFachadaAreas fachadaEmpresa = COFabricaDominio.Instancia.CrearInstancia<IARFachadaAreas>();
            AREmpresaDC datosEmpresa = fachadaEmpresa.ObtenerDatosEmpresa();
            CADatosImpCompMensajeroDC datosImpresion = new CADatosImpCompMensajeroDC()
            {
                ConsecutivoComprobante = consecutivoComprobante,
                CedulaMensajero = registroMensajero.Mensajero.Identificacion,
                NombrMensajero = registroMensajero.Mensajero.NombreApellido,
                NitEmpresa = datosEmpresa.Nit,
                NombreEmpresa = datosEmpresa.NombreEmpresa,
            };

            datosImpresion.MovmientosMensajero = new List<CADatosMovimientoDC>();

            datosImpresion.MovmientosMensajero.Add(new CADatosMovimientoDC()
            {
                IdConceptoCaja = registroMensajero.ConceptoCajaMensajero.IdConceptoCaja,
                NombreConceptoCaja = registroMensajero.ConceptoCajaMensajero.Nombre,
                ValorOperacion = registroMensajero.Valor
            });

            datosImpresion.FechaActual = DateTime.Now;
            return datosImpresion;
        }

        private CARegistroTransacCajaDC ArmarTransaccionCS(PUCentroServiciosDC cs, CAConceptoCajaDC conceptoCaja, decimal valor, string observaciones)
        {
            CARegistroTransacCajaDC transaccion = new CARegistroTransacCajaDC()
            {
                EsTransladoEntreCajas = false,
                EsUsuarioGestion = false,
                IdCentroServiciosVenta = cs.IdCentroServicio,
                NombreCentroServiciosVenta = cs.Nombre,
                IdCentroResponsable = cs.IdCentroServicio,
                NombreCentroResponsable = cs.Nombre,
                RegistrosTransacDetallesCaja = new List<CARegistroTransacCajaDetalleDC>(),
                Usuario = ControllerContext.Current.Usuario,
                ValorTotal = valor,
            };

            CARegistroTransacCajaDetalleDC transDetelle = new CARegistroTransacCajaDetalleDC()
            {
                ConceptoCaja = conceptoCaja,
                ConceptoEsIngreso = conceptoCaja.EsIngreso,
                Observacion = observaciones,
                Descripcion = observaciones,
                ValorServicio = valor,
                Numero = 0,
                NumeroFactura = "0",
                EstadoFacturacion = CAEnumEstadoFacturacion.FAC,
                NumeroComprobante = "0",
                FechaFacturacion = DateTime.Now
            };

            transaccion.InfoAperturaCaja = new CAAperturaCajaDC()
            {
                IdCaja = 0,
                IdCodigoUsuario = ControllerContext.Current.CodigoUsuario
            };

            transaccion.InfoAperturaCaja.IdAperturaCaja = CAApertura.Instancia.ValidarAperturaCajaCentroServicios(transaccion.InfoAperturaCaja, transaccion.IdCentroServiciosVenta, transaccion.IdCentroServiciosVenta);

            transaccion.RegistroVentaFormaPago = new List<CARegistroVentaFormaPagoDC>();

            CARegistroVentaFormaPagoDC formaPago = new CARegistroVentaFormaPagoDC()
            {
                Valor = valor,
                IdFormaPago = TAConstantesServicios.ID_FORMA_PAGO_CONTADO,
                Descripcion = TAConstantesServicios.DESCRIPCION_FORMA_PAGO_CONTADO,
                NumeroAsociado = "0"
            };

            transaccion.RegistroVentaFormaPago.Add(formaPago);
            transaccion.TipoDatosAdicionales = CAEnumTipoDatosAdicionales.PEA;
            transaccion.RegistrosTransacDetallesCaja.Add(transDetelle);

            return transaccion;
        }        

        /// <summary>
        /// Recibir el dinero del mensajero.
        /// </summary>
        /// <param name="transaccionMensajero">Lista de Transacciones para el registro del dinero.</param>
        public CADatosImpCompMensajeroDC RecibirDineroMensajero(List<CARecibirDineroMensajeroDC> transaccionMensajero)
        {
            long consecutivoComprobante = 0;
            consecutivoComprobante = CACaja.Instancia.ObtenerConsecutivoComprobateCajaIngreso();

            foreach (CARecibirDineroMensajeroDC operacion in transaccionMensajero)
            {
                operacion.RegistroEnCajaMensajero.RegistrosTransacDetallesCaja.ForEach(op =>
                {
                    op.NumeroComprobante = consecutivoComprobante.ToString();
                    op.Numero = operacion.NumeroAutorizacion;
                });
                operacion.CuentaMensajero.NumeroDocumento = consecutivoComprobante;
            }
            CAMensajero.Instancia.RecibirDineroMensajero(transaccionMensajero);
            IARFachadaAreas fachadaEmpresa = COFabricaDominio.Instancia.CrearInstancia<IARFachadaAreas>();
            AREmpresaDC datosEmpresa = fachadaEmpresa.ObtenerDatosEmpresa();
            CADatosImpCompMensajeroDC datosImpresion = new CADatosImpCompMensajeroDC()
            {
                ConsecutivoComprobante = consecutivoComprobante,
                CedulaMensajero = transaccionMensajero.FirstOrDefault().CuentaMensajero.Mensajero.Identificacion,
                NombrMensajero = transaccionMensajero.FirstOrDefault().CuentaMensajero.Mensajero.NombreApellido,
                NitEmpresa = datosEmpresa.Nit,
                NombreEmpresa = datosEmpresa.NombreEmpresa,
            };

            datosImpresion.MovimientosAgencia = new List<CADatosMovimientoDC>();
            datosImpresion.MovmientosMensajero = new List<CADatosMovimientoDC>();
            transaccionMensajero.ForEach(t =>
            {
                datosImpresion.MovimientosAgencia.Add(new CADatosMovimientoDC()
                {
                    IdConceptoCaja = t.RegistroEnCajaMensajero.RegistrosTransacDetallesCaja.FirstOrDefault().ConceptoCaja.IdConceptoCaja,
                    NombreConceptoCaja = t.RegistroEnCajaMensajero.RegistrosTransacDetallesCaja.FirstOrDefault().ConceptoCaja.Nombre,
                    NumeroOperacion = t.NumeroAutorizacion == 0 ? "" : "Aut. Des.:" + t.NumeroAutorizacion.ToString(),
                    ValorOperacion = t.RegistroEnCajaMensajero.RegistrosTransacDetallesCaja.FirstOrDefault().ValorServicio
                });

                datosImpresion.MovmientosMensajero.Add(new CADatosMovimientoDC()
                {
                    IdConceptoCaja = t.CuentaMensajero.ConceptoCajaMensajero.IdConceptoCaja,
                    NombreConceptoCaja = t.CuentaMensajero.ConceptoCajaMensajero.Nombre,
                    NumeroOperacion = t.NumeroAutorizacion == 0 ? "" : "Aut. Des.:" + t.NumeroAutorizacion.ToString(),
                    ValorOperacion = t.CuentaMensajero.Valor
                });
            });

            datosImpresion.FechaActual = DateTime.Now;
            return datosImpresion;
        }

        /// <summary>
        /// Obtiene las entregas del mensajero.
        /// </summary>
        /// <param name="idMensajero">The id mensajero.</param>
        /// <returns>Lista de Guias entregadas de alcobro  por mensajero</returns>
        public List<OUEnviosPendMensajerosDC> ObtenerEnviosPendMensajero(long idMensajero, long idComprobante)
        {
            return CAMensajero.Instancia.ObtenerEnviosPendMensajero(idMensajero, idComprobante);
        }

        /// <summary>
        /// Estados de la Cta mensajero.
        /// </summary>
        /// <param name="idMensajero">The id mensajero.</param>
        /// <param name="fechaConsulta">The fecha consulta.</param>
        /// <returns></returns>
        public List<CACuentaMensajeroDC> ObtenerEstadoCtaMensajero(long idMensajero, DateTime fechaConsulta)
        {
            return CAMensajero.Instancia.ObtenerEstadoCtaMensajero(idMensajero, fechaConsulta);
        }

        /// <summary>
        /// Actualiza la Observacion de trans.
        /// </summary>
        /// <param name="CuentaMensajero">The cuenta mensajero.</param>
        public void ActualizarObservacionEstadoCta(CACuentaMensajeroDC cuentaMensajero)
        {
            CAMensajero.Instancia.ActualizarObservacionEstadoCta(cuentaMensajero);
        }

        #endregion Caja Mensajero

        #region Caja Pin Prepago

        /// <summary>
        /// Obtiene los Prepagos vendidos por un Centro de Servicio.
        /// </summary>
        /// <param name="idCentroServicio">The id centro servicio.</param>
        /// <returns>Lista de Prepagos vendidos</returns>
        public List<CAPinPrepagoDC> ObtenerPrepagosCentroServicio(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina,
                                                                  int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros,
                                                                  long idCentroServicio)
        {
            return CAPinPrepago.Instancia.ObtenerPrepagosCentroServicio(filtro, campoOrdenamiento, indicePagina, registrosPorPagina,
                                                                                          ordenamientoAscendente, out totalRegistros, idCentroServicio);
        }

        /// <summary>
        /// Obtiene el detalle del pin prepago.
        /// </summary>
        /// <param name="pinPrepago">The pin prepago.</param>
        /// <returns>Lista de las compras realizadas con un pin prepago</returns>
        public List<CAPinPrepagoDtllCompraDC> ObtenerDtllCompraPinPrepago(long pinPrepago)
        {
            return CAPinPrepago.Instancia.ObtenerDtllCompraPinPrepago(pinPrepago);
        }

        /// <summary>
        /// Adiciona la venta de un Pin Prepago.
        /// </summary>
        /// <param name="ventaPinPrepago">The venta pin prepago.</param>
        public void AdicionarPinPrepago(CAPinPrepagoDC pinPrepago)
        {
            CAPinPrepago.Instancia.AdicionarPinPrepago(pinPrepago);
        }

        /// <summary>
        /// Venta del Pin prepago.
        /// </summary>
        /// <param name="ventaPinPrepago">The venta pin prepago.</param>
        public void VenderPinPrepago(CAVenderPinPrepagoDC ventaPinPrepago)
        {
            CAPinPrepago.Instancia.VenderPinPrepago(ventaPinPrepago);
        }

        /// <summary>
        /// Validar el saldo del Prepago para descontar del
        /// saldo.
        /// </summary>
        /// <param name="idPinPrepago">el id del pin prepago.</param>
        /// <param name="valorCompraPinPrepago">es el valor de la compra con el pin prepago.</param>
        /// <returns></returns>
        public void ValidarSaldoPrepago(long pinPrepago, decimal valorCompraPinPrepago)
        {
            CAPinPrepago.Instancia.ValidarSaldoPrepago(pinPrepago, valorCompraPinPrepago);
        }

        #endregion Caja Pin Prepago

        #region Varios Caja

        /// <summary>
        /// Obtiene el centro servicio.
        /// </summary>
        /// <param name="idCentroServicio">El idcentroservicio.</param>
        /// <returns>El centro de Servicio con la BaseInicial</returns>
        public PUCentroServiciosDC ObtenerCentroServicio(long idCentroServicio)
        {
            return CACaja.Instancia.ObtenerCentroServicio(idCentroServicio);
        }

        /// <summary>
        /// Metodo para Obtener los Centros de
        /// Servicios y Agencias por RACOL
        /// </summary>
        /// <param name="idRacol"> es el id del RACOL</param>
        /// <returns>lista de centros de servicio</returns>
        public List<PUCentroServiciosDC> ObtenerCentrosServicios(long idRacol)
        {
            return CACaja.Instancia.ObtenerCentrosServicios(idRacol);
        }

        /// <summary>
        /// Obtiene los suministros de un punto.
        /// </summary>
        /// <param name="centroServicio">The centro servicio.</param>
        /// <returns></returns>
        public IEnumerable<SUSuministro> ObtenerSuministrosPunto(PUCentroServiciosDC centroServicio)
        {
            return CACaja.Instancia.ObtenerSuministrosPunto(centroServicio);
        }

        /// <summary>
        /// Obtiene los puntosde atencion
        /// de una agencia centro Servicio.
        /// </summary>
        /// <param name="idCentroServicio">The id centro servicio.</param>
        /// <returns>Lista de Puntos de Una Agencia</returns>
        public List<PUCentroServiciosDC> ObtenerPuntosDeAgencia(long idCentroServicio)
        {
            return CACaja.Instancia.ObtenerPuntosDeAgencia(idCentroServicio);
        }

        /// <summary>
        /// Obtener los Tipos Observ punto.
        /// </summary>
        /// <returns></returns>
        public List<CATipoObsPuntoAgenciaDC> ObtenerTiposObservPunto()
        {
            return CACaja.Instancia.ObtenerTiposObservPunto();
        }

        /// <summary>
        /// Obtiene a la persona de la lista restrictiva por tipo de restriccion.
        /// </summary>
        /// <param name="documento">The documento.</param>
        /// <param name="tipoLista">The tipo lista.</param>
        /// <returns></returns>
        public bool ObtenerPersonaListaRestrictiva(string documento)
        {
            return CACaja.Instancia.ObtenerPersonaListaRestrictiva(documento);
        }

        /// <summary>
        /// Obtiene el valor del parametro configurado en la tabla de parametrso caja.
        /// </summary>
        /// <param name="idParametro">The id parametro.</param>
        /// <returns></returns>
        public string ObtenerParametroCajas(string idParametro)
        {
            return CACaja.Instancia.ObtenerParametroCajas(idParametro);
        }

        /// <summary>
        /// Obtiene a los Cajeros Auxiliares de un Centro de Servicio.
        /// </summary>
        /// <param name="idCentroServicio">The id centro servicio.</param>
        /// <returns></returns>
        public List<SEUsuarioCentroServicioDC> ObtenerCajeroCentroServicio(long idCentroServicio, string idRol)
        {
            return CACaja.Instancia.ObtenerCajeroCentroServicio(idCentroServicio, idRol);
        }

        /// <summary>
        /// Obtiene los consecutivos de ingreso y egreso para el formato
        /// de translado de dinero entre cajas ppal y Auxiliar
        /// </summary>
        /// <returns></returns>
        public PAConsecutivoIngresoEgresoDC ObtenerConsecutivoIngresoEgreso()
        {
            return CACaja.Instancia.ObtenerConsecutivoIngresoEgreso();
        }

        /// <summary>
        /// Metodo para Obtener un consecutivo
        /// de Comprobante de Caja ingreso
        /// </summary>
        /// <returns>El consecutivo de la solicitud</returns>
        public long ObtenerConsecutivoComprobateCajaIngreso()
        {
            return CACaja.Instancia.ObtenerConsecutivoComprobateCajaIngreso();
        }

        /// <summary>
        /// Metodo para Obtener un consecutivo
        /// de Comprobante de Caja egreso
        /// </summary>
        /// <returns>El consecutivo de la solicitud</returns>
        public long ObtenerConsecutivoComprobateCajaEgreso()
        {
            return CACaja.Instancia.ObtenerConsecutivoComprobateCajaEgreso();
        }

        /// <summary>
        /// Obtener los datos de la empresa (casa matriz según parámetros del Framework
        /// </summary>
        /// <returns>Información de la empresa</returns>
        public AREmpresaDC ObtenerDatosEmpresa()
        {
            return CACaja.Instancia.ObtenerDatosEmpresa();
        }

        /// <summary>
        /// Actualiza el cierre de caja como Caja Reportada al cajero Ppal
        /// </summary>
        /// <param name="idCierreCajaAux">The id cierre caja aux.</param>
        public void ReportarCajaACajeroPrincipal(CARegistroTransacCajaDC movimientoCaja, long idCierreCaja)
        {
            CACierreCaja.CACierreCaja.Instancia.ReportarCajaACajeroPrincipal(movimientoCaja, idCierreCaja);
        }

        /// <summary>
        /// Obtiene la Ultima apertura activa del usuario por Centro de
        /// servicio
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <param name="idCaja"></param>
        /// <param name="idCodUsuario"></param>
        /// <returns>el id long de la ultima apertura</returns>
        public long ObtenerUltimaAperturaActiva(long idCentroServicio, int idCaja, long idCodUsuario)
        {
            return CAApertura.Instancia.ObtenerUltimaAperturaActiva(idCentroServicio, idCaja, idCodUsuario);
        }

        /// <summary>
        /// Obtiene la info de la persona interna por el codigo del
        /// usuario
        /// </summary>
        /// <param name="idCodigoUsuario"></param>
        /// <returns></returns>
        public SEUsuarioPorCodigoDC ObtenerUsuarioPorCodigo(long idCodigoUsuario)
        {
            return CACaja.Instancia.ObtenerUsuarioPorCodigo(idCodigoUsuario);
        }

        /// <summary>
        /// Obtiene una lista de los clientes crédito
        /// </summary>
        /// <returns>lista de Clientes</returns>
        public IEnumerable<CLClientesDC> ObtenerClientes()
        {
            return CACaja.Instancia.ObtenerClientes();
        }

        /// <summary>
        /// Consultar el giro por numero de giro
        /// </summary>
        /// <param name="idGiro">Numero del giro a consultar</param>
        /// <returns>informacion del giro</returns>
        public GIAdmisionGirosDC ConsultarGiroXNumGiro(long idGiro)
        {
            return CACaja.Instancia.ConsultarGiroXNumGiro(idGiro);
        }

        #endregion Varios Caja

        #region Conceptos Caja

        /// <summary>
        /// Obtiene los conceptos de Caja por especificacion de
        /// visibilidad para mensajero - punto/Agencia - Racol.
        /// </summary>
        /// <param name="filtroCampoVisible">The filtro campo visible.</param>
        /// <returns>Lista de Conceptos de Caja por el filtro de Columna</returns>
        public List<CAConceptoCajaDC> ObtenerConceptosCajaPorCategoria(int idCategoria)
        {
            return CACaja.Instancia.ObtenerConceptosCajaPorCategoria(idCategoria);
        }

        /// <summary>
        /// Obtiene la dupla del concepto.
        /// </summary>
        /// <param name="idConcepto">The id concepto.</param>
        /// <returns>dupla del concepto enviado</returns>
        public CAConceptoCajaDC ObtenerDuplaConcepto(int idConcepto)
        {
            return CACaja.Instancia.ObtenerDuplaConcepto(idConcepto);
        }

        #endregion Conceptos Caja

        /// <summary>
        /// Obtiene todos los valores de los parametros configurado en la tabla de parametros caja.
        /// </summary>        
        /// <returns></returns>
        public IEnumerable<CAParametroCaja> ObtenerParametrosCajas()
        {
            return CARepositorioCaja.Instancia.ObtenerParametrosCajas();
        }

        /// <summary>
        /// Actualiza el valor de un parametro de cajas
        /// </summary>
        /// <param name="idParametro"></param>
        /// <param name="valor"></param>
        public void ActualizarParametroCaja(string idParametro, string valor)
        {
            CARepositorioCaja.Instancia.ActualizarParametroCaja(idParametro,valor);
        }
    }
}