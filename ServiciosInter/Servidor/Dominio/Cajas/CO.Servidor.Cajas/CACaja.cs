using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Transactions;
using CO.Servidor.Cajas.Comun;
using CO.Servidor.Cajas.Datos;
using CO.Servidor.Cajas.Datos.Modelo;
using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Dominio.Comun.Area;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Clientes;
using CO.Servidor.Dominio.Comun.Comisiones;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.OperacionUrbana;
using CO.Servidor.Dominio.Comun.Produccion;
using CO.Servidor.Dominio.Comun.Suministros;
using CO.Servidor.Dominio.Comun.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros.Pagos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Area;
using CO.Servidor.Servicios.ContratoDatos.Cajas;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using CO.Servidor.Servicios.ContratoDatos.Comisiones;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.Produccion;
using CO.Servidor.Servicios.ContratoDatos.Solicitudes;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Precios;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Seguridad;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using Framework.Servidor.Servicios.ContratoDatos.Parametros.Consecutivos;
using Framework.Servidor.Servicios.ContratoDatos.Seguridad;
using CO.Servidor.Cajas.Apertura;
using CO.Servidor.Cajas.CajaVenta;
using CO.Servidor.Servicios.ContratoDatos.GestionCajas;


namespace CO.Servidor.Cajas
{
    /// <summary>
    /// Clase que contiene los metodos de consulta de otros
    /// modulos para completar los procesos de validacion
    /// y obtencion de datos.
    /// </summary>
    internal class CACaja : ControllerBase
    {
        #region Atributos
        private CATransaccionCaja registroTransaccion;
        private static readonly CACaja instancia = (CACaja)FabricaInterceptores.GetProxy(new CACaja(), COConstantesModulos.CAJA);
        IADFachadaAdmisionesMensajeria fachadaMensajeria = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>();
        #endregion Atributos

        #region Instancia

        public static CACaja Instancia
        {
            get { return CACaja.instancia; }
        }

        private CACaja()
        {
            registroTransaccion = new CATransaccionCaja();
        }

        #endregion Instancia

        #region Centro Svc

        /// <summary>
        /// Obtiene el centro servicio.
        /// </summary>
        /// <param name="idCentroServicio">El idcentroservicio.</param>
        /// <returns>El centro de Servicio con la BaseInicial</returns>
        public PUCentroServiciosDC ObtenerCentroServicio(long idCentroServicio)
        {
            IPUFachadaCentroServicios fachadaCentroServicios = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();
            return fachadaCentroServicios.ObtenerCentroServicio(idCentroServicio);
        }

        /// <summary>
        /// Metodo para Obtener los Centros de
        /// Servicios y Agencias por RACOL
        /// </summary>
        /// <param name="idRacol"> es el id del RACOL</param>
        /// <returns>lista de centros de servicio</returns>
        public List<PUCentroServiciosDC> ObtenerCentrosServicios(long idRacol)
        {
            IPUFachadaCentroServicios fachadaCentroServicios = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();
            return fachadaCentroServicios.ObtenerCentrosServicios(idRacol);
        }


        /// <summary>
        /// Obtiene los Centros de Servicios Activos e Inactivos de una Racol
        /// </summary>
        /// <param name="idRacol"></param>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerCentrosServiciosTodos(long idRacol)
        {
            IPUFachadaCentroServicios fachadaCentrosServiciosTodos = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();
            return fachadaCentrosServiciosTodos.ObtenerCentrosServiciosTodos(idRacol);
        }

        /// <summary>
        /// Obtiene la agencia responsable punto.
        /// </summary>
        /// <param name="idPunto">es el id del punto.</param>
        /// <returns></returns>
        public PUAgenciaDeRacolDC ObtenerAgenciaResponsablePunto(long idPunto)
        {
            IPUFachadaCentroServicios fachadaCentroServicios = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();
            return fachadaCentroServicios.ObtenerAgenciaResponsable(idPunto);
        }

        /// <summary>
        /// Obtiene los puntosde atencion
        /// de una agencia centro Servicio.
        /// </summary>
        /// <param name="idCentroServicio">The id centro servicio.</param>
        /// <returns>Lista de Puntos de Una Agencia</returns>
        public List<PUCentroServiciosDC> ObtenerPuntosDeAgencia(long idCentroServicio)
        {
            IPUFachadaCentroServicios fachadaCentroServicios = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();
            return fachadaCentroServicios.ObtenerPuntosDeAgencia(idCentroServicio);
        }

        /// <summary>
        /// Obtener los Tipos Observ punto.
        /// </summary>
        /// <returns></returns>
        public List<CATipoObsPuntoAgenciaDC> ObtenerTiposObservPunto()
        {
            return CARepositorioCaja.Instancia.ObtenerTiposObservPunto();
        }

        public CAAperturaCajaDC ObtenerDatosCajeroAutomatico()
        {
            return null;
        }

        /// <summary>
        /// Obtiene el responsable segun el tipo del Cerntro de Svr
        /// </summary>
        /// <param name="idCentroSrv"></param>
        /// <param name="tipoCentroSrv"></param>
        /// <returns>info del centro de servicio responsable</returns>
        public PUAgenciaDeRacolDC ObtenerResponsableCentroSrvSegunTipo(long idCentroSrv, string tipoCentroSrv)
        {
            IPUFachadaCentroServicios fachadaCentroServicios = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();
            return fachadaCentroServicios.ObtenerResponsableCentroSrvSegunTipo(idCentroSrv, tipoCentroSrv);
        }

        /// <summary>
        /// Obtiene todas la Agencias-ptos-col-Racol
        /// </summary>
        /// <returns>listaCentrosServicio</returns>
        public IList<PUCentroServiciosDC> ObtenerTodosCentrosServicios()
        {
            IPUFachadaCentroServicios fachadaCentroServicios = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();
            return fachadaCentroServicios.ObtenerCentrosServiciosActivos();
        }

        #endregion Centro Svc

        #region Mensajero

        /// <summary>
        /// Obtiene los mensajeros de apoyo para el envio de dinero.
        /// </summary>
        /// <param name="idCentroServicio">The id centro servicio.</param>
        /// <returns></returns>
        public List<OUNombresMensajeroDC> ObtenerMensajerosPuntoCentroServicio(long idCentroServicio)
        {
            IOUFachadaOperacionUrbana fachadaOperacionUrbana = COFabricaDominio.Instancia.CrearInstancia<IOUFachadaOperacionUrbana>();
            List<OUNombresMensajeroDC> lstMensajeros = new List<OUNombresMensajeroDC>();
            string tipoCentroServicio = ObtenerCentroServicio(idCentroServicio).Tipo;
            List<PUCentroServicioApoyo> LstColDeRacol = new List<PUCentroServicioApoyo>();

            if (tipoCentroServicio == CAConstantesCaja.PUNTO)
            {
                //Consulto por el Id del Punto el id del responsable y lo reemplazo para consultar
                // los mensajeros
                idCentroServicio = ObtenerAgenciaResponsablePunto(idCentroServicio).IdResponsable;

                //Obtengo los Mensajeros Nombre, idMensajero y DocMensajero
                return new List<OUNombresMensajeroDC>(fachadaOperacionUrbana.ObtenerNombreMensajeroAgencia(idCentroServicio));
            }
            else if (tipoCentroServicio == CAConstantesCaja.RACOL)
            {
                //consulto el Col del Racol para tener los Mensajeros Correspondientes
                LstColDeRacol = ObtenerCentrosLogisticosRacol(idCentroServicio);

                List<OUNombresMensajeroDC> lstMensajerosRacol = new List<OUNombresMensajeroDC>();

                LstColDeRacol.ForEach(col =>
                {
                    lstMensajeros = new List<OUNombresMensajeroDC>(fachadaOperacionUrbana.ObtenerNombreMensajeroAgencia(col.IdCentroservicio));
                    lstMensajerosRacol.AddRange(lstMensajeros.Where(car => car.EsMensajeroUrbano == false));
                });

                //Obtengo los Mensajeros Nombre, idMensajero y DocMensajero
                return lstMensajerosRacol;
            }
            else
            {
                //Obtengo los Mensajeros Nombre, idMensajero y DocMensajero
                return new List<OUNombresMensajeroDC>(fachadaOperacionUrbana.ObtenerNombreMensajeroAgencia(idCentroServicio));
            }
        }

        /// <summary>
        /// Obtiene los COL de un Racol
        /// </summary>
        /// <param name="idRacol">id Racol</param>
        /// <returns>lista de Col de un Racol</returns>
        public List<PUCentroServicioApoyo> ObtenerCentrosLogisticosRacol(long idRacol)
        {
            IPUFachadaCentroServicios fachadaCentroServicios = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();
            return fachadaCentroServicios.ObtenerCentrosLogisticosRacol(idRacol);
        }

        /// <summary>
        /// Obtiene los mensajeros de una Agencia Paginado.
        /// </summary>
        /// <param name="idCentroServicio">The id centro servicio.</param>
        /// <returns></returns>
        public IEnumerable<OUNombresMensajeroDC> ObtenerMensajerosPuntoCentroServicioPag(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina,
                                                                                int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros,
                                                                                long idCentroServicio)
        {
            string tipoCentroServicio = ObtenerCentroServicio(idCentroServicio).Tipo;
            if (tipoCentroServicio == CAConstantesCaja.PUNTO)
            {
                //Consulto por el Id del Punto el id del responsable y lo reemplazo para consultar
                // los mensajeros
                idCentroServicio = ObtenerAgenciaResponsablePunto(idCentroServicio).IdResponsable;
            }

            //Obtengo los Mensajeros Nombre, idMensajero y DocMensajero
            IOUFachadaOperacionUrbana fachadaOperacionUrbana = COFabricaDominio.Instancia.CrearInstancia<IOUFachadaOperacionUrbana>();
            return fachadaOperacionUrbana.ObtenerNombreMensajeroAgenciaPag(filtro, campoOrdenamiento, indicePagina, registrosPorPagina,
                                                                   ordenamientoAscendente, out totalRegistros, idCentroServicio);
        }

        #endregion Mensajero

        #region Caja


        /// <summary>
        /// Consulta el centro de servicios que recibió el pago de un al cobro
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public PUCentroServiciosDC ConsultarCentroDeServiciosPagoAlCobro(long numeroGuia, out decimal valorCargado)
        {
            return CARepositorioCaja.Instancia.ConsultarCentroDeServiciosPagoAlCobro(numeroGuia,out valorCargado);
        }

        /// <summary>
        /// Modifica una transacción de caja dado el número de guía, el concepto de caja y el centro de servicios al que pertenece agregando o modificando el número de comprobante de acuerdo al tipo de consecutivo
        /// </summary>
        /// <param name="idCentroServicios"></param>
        /// <param name="idConceptoCaja"></param>
        /// <param name="numeroGuia"></param>
        /// <param name="tipoConsecutivo"></param>
        public long AgregarNumeroComprobanteAMovimientoCaja(long numeroGuia, int idConceptoCaja, long idCentroServicios, PAEnumConsecutivos tipoConsecutivo)
        {
            long idRegistroTransaccionDetalle = CARepositorioCaja.Instancia.ObtenerIdDetalleMovimientoPorCentroServicio(numeroGuia, idConceptoCaja, idCentroServicios);

            long consecutivo = 0;

            // Obtengo consecuttivo de acuerdo al tipo
            if (tipoConsecutivo.Equals(PAEnumConsecutivos.Comprobante_Egreso))
                consecutivo = CACaja.Instancia.ObtenerConsecutivoComprobateCajaEgreso();
            else if (tipoConsecutivo.Equals(PAEnumConsecutivos.Comprobante_Ingreso))
                consecutivo = CACaja.Instancia.ObtenerConsecutivoComprobateCajaIngreso();
            CARepositorioCaja.Instancia.ActualizarNumeroComprobanteTransaccionCaja(idRegistroTransaccionDetalle, consecutivo.ToString());
            return consecutivo;
        }

        /// <summary>
        /// Obtiene los conceptos de Caja por especificacion de
        /// visibilidad para mensajero - punto/Agencia - Racol.
        /// </summary>
        /// <param name="filtroCampoVisible">The filtro campo visible.</param>
        /// <returns>Lista de Conceptos de Caja por el filtro de Columna</returns>
        public List<CAConceptoCajaDC> ObtenerConceptosCajaPorCategoria(int idCategoria)
        {
            return CARepositorioCaja.Instancia.ObtenerConceptosCajaPorCategoria(idCategoria);
        }

        /// <summary>
        /// Obtiene la dupla del concepto.
        /// </summary>
        /// <param name="idConcepto">The id concepto.</param>
        /// <returns>dupla del concepto enviado</returns>
        public CAConceptoCajaDC ObtenerDuplaConcepto(int idConcepto)
        {
            return CARepositorioCaja.Instancia.ObtenerDuplaConcepto(idConcepto);
        }

        /// <summary>
        /// Obtiene el valor del parametro configurado en la tabla de parametrso caja.
        /// </summary>
        /// <param name="idParametro">The id parametro.</param>
        /// <returns></returns>
        public string ObtenerParametroCajas(string idParametro)
        {
            return CARepositorioCaja.Instancia.ObtenerParametroCajas(idParametro);
        }

        /// <summary>
        /// Obtiene el saldo acumulado en caja
        /// </summary>
        /// <param name="idCentroServicios"></param>
        /// <returns></returns>
        public decimal ObtenerSaldoActualCaja(long idCentroServicios)
        {
            return CARepositorioCaja.Instancia.ObtenerSaldoActualCaja(idCentroServicios);
        }

        /// <summary>
        /// Obtiene el concepto de Caja por id.
        /// </summary>
        /// <param name="idConcepto">The id concepto.</param>
        /// <returns></returns>
        public CAConceptoCajaDC ObtenerConceptoPorId(int idConcepto)
        {
            return CARepositorioCaja.Instancia.ObtenerConceptoPorId(idConcepto);
        }

        /// <summary>
        /// Obtiene la transaccion completa para anular una guia
        /// dependiendo del id del concepto de la caja y del numero de la guia
        /// en el campo Numero de registro trans detalle Caja
        /// </summary>
        public CARegistroTransacCajaDC ObtenerTransaccionCajaAnulacionGuia(long numero, int idConceptoCaja)
        {
            CARegistroTransacCajaDC transaccion = CARepositorioCaja.Instancia.ObtenerTransaccionCaja(numero, idConceptoCaja);
            if (transaccion != null && transaccion.RegistrosTransacDetallesCaja != null)
            {
                ActualizarConceptoAnulacion(transaccion, CAEnumConceptosCaja.CONCEPTOCAJA_GUIA_ANULADA);
            }
            return transaccion;
        }

        /// <summary>
        /// Obtiene la transaccion completa para anular un giro
        /// dependiendo del id del concepto de la caja y del numero del giro
        /// en el campo Numero de registro trans detalle Caja
        /// </summary>
        public CARegistroTransacCajaDC ObtenerTransaccionCajaAnulacionGiro(long numero, int idConceptoCaja)
        {
            CARegistroTransacCajaDC transaccion = CARepositorioCaja.Instancia.ObtenerTransaccionCaja(numero, idConceptoCaja);

            //Actualizo los datos de Inserción
            transaccion.InfoAperturaCaja.IdCaja = 0;
            transaccion.InfoAperturaCaja.IdCodigoUsuario = ControllerContext.Current.CodigoUsuario;
            transaccion.InfoAperturaCaja.CreadoPor = ControllerContext.Current.Usuario;
            transaccion.Usuario = ControllerContext.Current.Usuario;

            return transaccion;
        }

        /// <summary>
        /// Actualiza el DataContract de Concepto de Caja
        /// para que Aplique la Anulacion
        /// </summary>
        /// <param name="transaccion"></param>
        private static void ActualizarConceptoAnulacion(CARegistroTransacCajaDC transaccion, CAEnumConceptosCaja tipoConceptoAnulacion)
        {
            CAConceptoCajaDC conceptoAnulacion = CARepositorioCaja.Instancia.ObtenerConceptoPorId((int)tipoConceptoAnulacion);

            if (conceptoAnulacion != null && conceptoAnulacion.IdConceptoCaja > 0)
            {
                transaccion.RegistrosTransacDetallesCaja.ForEach(o =>
                {
                    o.ConceptoEsIngreso = conceptoAnulacion.EsIngreso;

                    o.ConceptoCaja = new CAConceptoCajaDC()
                    {
                        EsIngreso = conceptoAnulacion.EsIngreso,
                        IdConceptoCaja = conceptoAnulacion.IdConceptoCaja,
                        IdCuentaExterna = conceptoAnulacion.IdCuentaExterna,
                        Nombre = conceptoAnulacion.Nombre
                    };
                });
            }
            else
            {
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.CAJA, CAEnumTipoErrorCaja.ERROR_CONCEPTO_ANULACIONGUIA_NOEXISTE.ToString(), CACajaServerMensajes.CargarMensaje(CAEnumTipoErrorCaja.ERROR_CONCEPTO_ANULACIONGUIA_NOEXISTE)));
            }
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
            long idAperturaCaja = CARepositorioCaja.Instancia.ObtenerAperturaCajaPorUsuario(idUsuario, idCentroServicio, idCaja);

            return CARepositorioCaja.Instancia.ObtenerInfoCierreCaja(idAperturaCaja, idFormaPago, operador);
        }

        /// <summary>
        /// Obtener el registro de transacción para una númeor de operación
        /// </summary>
        /// <param name="numeroOperacion">Número de operación: Número de guía, Número de giro, etc</param>
        /// <returns>Información del registro detallado de la transacción</returns>
        public CARegistroTransacCajaDetalleDC ObtenerRegistroVentaPorNumeroOperacion(long numeroOperacion)
        {
            return CARepositorioCaja.Instancia.ObtenerRegistroVentaPorNumeroOperacion(numeroOperacion);
        }

        /// <summary>
        /// Obtiene la Transaccion detalle de
        /// un giro
        /// </summary>
        /// <param name="numeroGiro">se valida con RTD_numero</param>
        /// <param name="idConceptoCaja">es el concepto deCaja</param>
        /// <returns></returns>
        public CARegistroTransacCajaDetalleDC ObtenerDetalleTransaccion(long numero, int idConceptoCaja)
        {
            return CARepositorioCaja.Instancia.ObtenerDetalleTransaccion(numero, idConceptoCaja);
        }
        /// <summary>
        /// Ingresa un ajuste a la caja de un punto
        /// </summary>
        /// <param name="ajusteCaja"></param>
        public CAAjustesCajaNovedadesDC IngresarAjusteCajaNovedades(CAAjustesCajaNovedadesDC ajusteCaja)
        {
            using (TransactionScope trans = new TransactionScope())
            {
                long consecutivoComprobante = 0;
                consecutivoComprobante = CACaja.Instancia.ObtenerConsecutivoComprobateCajaEgreso();
                IADFachadaAdmisionesMensajeria fachadaMensajeria = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>();

                if (ajusteCaja.ConceptoCaja.RequiereNoDocumento && fachadaMensajeria.ObtenerGuiaXNumeroGuia(ajusteCaja.NumeroDocumento) == null)
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.CAJA,"0","El número de guía ingresado no es válido"));

                //CARepositorioCaja.Instancia.AdicionarMovimientoCaja(transaccion);
                CARegistroTransacCajaDC transaccionOrigen = ArmarTransaccionAjusteCaja(ajusteCaja.CentroServicio, ajusteCaja.ConceptoCaja, ajusteCaja.Valor, ajusteCaja.Observaciones, ajusteCaja.NumeroDocumento);
                transaccionOrigen.RegistrosTransacDetallesCaja.FirstOrDefault().NumeroComprobante = consecutivoComprobante.ToString();
                CAIdTransaccionesCajaDC idTransOri = registroTransaccion.AdicionarMovimientoCaja(transaccionOrigen);

                if (ajusteCaja.CentroServicioContraPartida != null && ajusteCaja.ConceptoCaja.ContraPartidaCS && !ajusteCaja.ConceptoCaja.ContraPartidaCasaMatriz)
                {
                    CARegistroTransacCajaDC transaccionContrapartida = ArmarTransaccionAjusteCaja(ajusteCaja.CentroServicioContraPartida, ajusteCaja.ConceptoCaja, ajusteCaja.Valor, ajusteCaja.Observaciones,ajusteCaja.NumeroDocumento);
                    transaccionContrapartida.RegistrosTransacDetallesCaja.FirstOrDefault().ConceptoCaja.EsIngreso = !ajusteCaja.ConceptoCaja.EsIngreso;
                    transaccionContrapartida.RegistrosTransacDetallesCaja.FirstOrDefault().ConceptoCaja.EsEgreso = ajusteCaja.ConceptoCaja.EsIngreso;
                    transaccionContrapartida.RegistrosTransacDetallesCaja.FirstOrDefault().ConceptoEsIngreso = !ajusteCaja.ConceptoCaja.EsIngreso;
                    transaccionContrapartida.RegistrosTransacDetallesCaja.FirstOrDefault().NumeroComprobante = consecutivoComprobante.ToString();
                    CAIdTransaccionesCajaDC idTransContraPartida = registroTransaccion.AdicionarMovimientoCaja(transaccionContrapartida);

                    CAMovCentroSvcCentroSvcDC movCsCs = new CAMovCentroSvcCentroSvcDC()
                    {
                        UsuarioRegistra = ControllerContext.Current.Usuario,
                        IdCentroServicioDestino = transaccionContrapartida.IdCentroServiciosVenta,
                        IdCentroServicioOrigen = transaccionOrigen.IdCentroServiciosVenta,
                        IdRegistroTxDestino = idTransContraPartida.IdTransaccionCaja,
                        IdRegistroTxOrigen = idTransOri.IdTransaccionCaja,
                        NombreCentroServicioDestino = transaccionContrapartida.NombreCentroServiciosVenta,
                        NombreCentroServicioOrigen = transaccionOrigen.NombreCentroServiciosVenta
                    };

                    CARepositorioGestionCajas.Instancia.AdiconarMovCentroServiciosACentroServicios(movCsCs);
                }
                trans.Complete();

                ajusteCaja.NumeroComprobante = consecutivoComprobante.ToString();
                ajusteCaja.FechaGrabacion = DateTime.Now;
                return ajusteCaja;
            }
        }

        private CARegistroTransacCajaDC ArmarTransaccionAjusteCaja(PUCentroServiciosDC cs, CAConceptoCajaDC conceptoCaja, decimal valor, string observaciones, long numeroDocumento=0)
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
                Numero = numeroDocumento,
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

        #endregion Caja

        #region Bancos

        /// <summary>
        /// Obtiene todos los bancos
        /// </summary>
        /// <returns>Lista con los bancos</returns>
        public IList<PABanco> ObtenerTodosBancos()
        {
            return PAAdministrador.Instancia.ObtenerTodosBancos();
        }

        /// <summary>
        /// Obtiene los Tipos de Documentos de Banco
        /// </summary>
        /// <returns>lista de los Tipos de Doc Banco</returns>
        public IList<PATipoDocumBancoDC> ObtenerTiposDocumentosBanco()
        {
            return PAAdministrador.Instancia.ObtenerTiposDocumentosBanco();
        }

        #endregion Bancos

        #region Otros Modulos

        /// <summary>
        /// Obtiene los suministros de un punto.
        /// </summary>
        /// <param name="centroServicio">The centro servicio.</param>
        /// <returns></returns>
        public IEnumerable<SUSuministro> ObtenerSuministrosPunto(PUCentroServiciosDC centroServicio)
        {
            ISUFachadaSuministros fachadaSuministros = COFabricaDominio.Instancia.CrearInstancia<ISUFachadaSuministros>();
            return fachadaSuministros.ObtenerSuministrosCentroServicio(centroServicio);
        }

        /// <summary>
        /// Obtiene a la persona de la lista restrictiva por tipo de restriccion.
        /// </summary>
        /// <param name="documento">The documento.</param>
        /// <param name="tipoLista">The tipo lista.</param>
        /// <returns></returns>
        public bool ObtenerPersonaListaRestrictiva(string documento)
        {
            return PAAdministrador.Instancia.ValidarListaRestrictiva(documento);
        }

        /// <summary>
        /// Consultar el id del cliente propietario de una operación
        /// </summary>
        /// <param name="idOperacion">Número de la operación de caja que se quiere consultar</param>
        /// <returns>Id del cliente dueño de la operación, si no pertenece a un cliente retorna null</returns>
        public List<CAOperacionDeClienteDC> ConsultarClientePropDeOperacion(long idOperacionDesde, long idOperacionHasta)
        {
            List<ClienteOperacion_CAJ> operacionesCliente = CARepositorioCaja.Instancia.ConsultarClientePropDeOperacion(idOperacionDesde, idOperacionHasta);

            List<CAOperacionDeClienteDC> operacionesClienteReturn = new List<CAOperacionDeClienteDC>();

            if (operacionesCliente != null)
            {                
                operacionesCliente.ForEach((operacionCliente) =>
                {
                    operacionesClienteReturn.Add(new CAOperacionDeClienteDC()
                    {
                        IdCliente = operacionCliente.IdCliente,
                        IdMovimiento = operacionCliente.IdMovimiento,
                        IdServicio = operacionCliente.IdServicio,
                        NombreServicio = operacionCliente.NombreServicio,
                        ValorOperacion = operacionCliente.ValorOperacion,
                        FechaOperacion = operacionCliente.FechaGrabacion
                    });
                });
            };

            return operacionesClienteReturn;
        }

        /// <summary>
        /// Obtiene a los Cajeros Auxiliares de un Centro de Servicio.
        /// </summary>
        /// <param name="idCentroServicio">The id centro servicio.</param>
        /// <returns></returns>
        public List<SEUsuarioCentroServicioDC> ObtenerCajeroCentroServicio(long idCentroServicio, string idRol)
        {
            return SEAdministradorSeguridad.Instancia.ObtenerCajeroCentroServicio(idCentroServicio, idRol);
        }

        /// <summary>
        /// Obtiene la info de la persona interna por el codigo del
        /// usuario
        /// </summary>
        /// <param name="idCodigoUsuario"></param>
        /// <returns></returns>
        public SEUsuarioPorCodigoDC ObtenerUsuarioPorCodigo(long idCodigoUsuario)
        {
            return SEAdministradorSeguridad.Instancia.ObtenerUsuarioPorCodigo(idCodigoUsuario);
        }

        /// <summary>
        /// Obtiene los consecutivos de ingreso y egreso para el formato
        /// de translado de dinero entre cajas ppal y Auxiliar
        /// </summary>
        /// <returns></returns>
        public PAConsecutivoIngresoEgresoDC ObtenerConsecutivoIngresoEgreso()
        {
            PAConsecutivoIngresoEgresoDC consecutivos = new PAConsecutivoIngresoEgresoDC()
            {
                ConsecutivoIngreso = ObtenerConsecutivoComprobateCajaIngreso(),
                ConsecutivoEgreso = ObtenerConsecutivoComprobateCajaEgreso(),
            };

            return consecutivos;
        }

        /// <summary>
        /// Metodo para Obtener un consecutivo
        /// de Comprobante de Caja de ingreso
        /// </summary>
        /// <returns>El consecutivo de la solicitud</returns>
        public long ObtenerConsecutivoComprobateCajaIngreso()
        {
            using (TransactionScope scope = new TransactionScope())
            {
                long consecutivo = PAAdministrador.Instancia.ObtenerConsecutivo(PAEnumConsecutivos.Comprobante_Ingreso);
                scope.Complete();
                return consecutivo;
            };
        }

        /// <summary>
        /// Metodo para Obtener un consecutivo
        /// de Comprobante de Caja de egreso
        /// </summary>
        /// <returns>El consecutivo de la solicitud</returns>
        public long ObtenerConsecutivoComprobateCajaEgreso()
        {
            using (TransactionScope scope = new TransactionScope())
            {
                long consecutivo = PAAdministrador.Instancia.ObtenerConsecutivo(PAEnumConsecutivos.Comprobante_Egreso);
                scope.Complete();
                return consecutivo;
            };
        }

        /// <summary>
        /// Obtener los datos de la empresa (casa matriz según parámetros del Framework
        /// </summary>
        /// <returns>Información de la empresa</returns>
        public AREmpresaDC ObtenerDatosEmpresa()
        {
            IARFachadaAreas fachadaArea = COFabricaDominio.Instancia.CrearInstancia<IARFachadaAreas>();
            return fachadaArea.ObtenerDatosEmpresa();
        }

        /// <summary>
        /// Obtiene los giros no pagos por centro SVC.
        /// </summary>
        /// <param name="idCentroSrv">The id centro SRV.</param>
        /// <returns>el numero de giros y valor total de los giros no pagos por un centro de servicio </returns>
        public PGTotalPagosDC ObtenerGirosNoPagosCentroSvc(long idCentroSrv)
        {
            IADFachadaAdmisionesGiros fachadaGiros = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesGiros>();
            return fachadaGiros.ConsultarPagosAgencia(idCentroSrv);
        }

        /// <summary>
        /// Obtiene la informacion de los alcobros sin cancelar y de los
        /// que estan en transito de un Centro de Servicio
        /// </summary>
        /// <param name="idCentroSrv">es el Id del Centro de Servicio</param>
        /// <returns>info de Totales</returns>
        public ADAlCobrosSinCancelarDC ObtenerTotalesAlCobrosSinPagar(long idCentroSrv)
        {
            return CARepositorioCaja.Instancia.ObtenerTotalesAlCobrosSinPagar(idCentroSrv);
        }

        /// <summary>
        /// Consultar el giro por numero de giro
        /// </summary>
        /// <param name="idGiro">Numero del giro a consultar</param>
        /// <returns>informacion del giro</returns>
        public GIAdmisionGirosDC ConsultarGiroXNumGiro(long idGiro)
        {
            IADFachadaAdmisionesGiros fachadaGiros = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesGiros>();
            return fachadaGiros.ConsultarGiroXNumGiro(idGiro);
        }

        /// <summary>
        /// Obtiene una lista de los clientes crédito
        /// </summary>
        /// <returns>lista de Clientes</returns>
        public IEnumerable<CLClientesDC> ObtenerClientes()
        {
            ICLFachadaClientes fachadaCLientes = COFabricaDominio.Instancia.CrearInstancia<ICLFachadaClientes>();
            return fachadaCLientes.ObtenerClientes();
        }

        /// <summary>
        /// Retorna todas las casas matriz activas
        /// </summary>
        /// <returns>Colección con todas las casas matrices</returns>
        public IList<ARCasaMatrizDC> ObtenerTodasLasCasaMatriz()
        {
            IARFachadaAreas fachadaArea = COFabricaDominio.Instancia.CrearInstancia<IARFachadaAreas>();
            return fachadaArea.ObtenerTodasLasCasaMatriz();
        }

        /// <summary>
        /// Retorna las cuentas externas del sistema
        /// </summary>
        /// <returns></returns>
        public List<CO.Servidor.Servicios.ContratoDatos.Cajas.CACuentaExterna> ObtenerCuentasExternas()
        {
            return CARepositorioGestionCajas.Instancia.ObtenerCuentasExternas();
        }

        /// <summary>
        /// Se consulta la Comision del Centro de Servicio y del
        /// Responsable para crear las Novedades
        /// </summary>
        /// <param name="giroAnulado">info del giro</param>
        /// <param name="tipoDevolucion">texto de acompañamiento en la observacion</param>
        /// <returns>lista de novedades</returns>
        private List<PANovedadCentroServicioDCDeprecated> ConsultarComisionesCtroSrvYResponsable(CARegistroTransacCajaDC giroAnulado,
                                                                                        string tipoDevolucion, int idMotivoNovedad)
        {
            ICMFachadaComisiones fachadaComisiones = COFabricaDominio.Instancia.CrearInstancia<ICMFachadaComisiones>();

            //Consulta de las Comisiones Relacionadas
            List<CMComisionXVentaCalculadaDC> comisionesRelacionadas = fachadaComisiones.ObtenerComisionPtoYCentroResponsable(giroAnulado.RegistrosTransacDetallesCaja.FirstOrDefault().Numero);

            List<PANovedadCentroServicioDCDeprecated> lstNvNovedades = new List<PANovedadCentroServicioDCDeprecated>();

            if (comisionesRelacionadas != null)
            {
                comisionesRelacionadas.ForEach(com =>
                {
                    //Novedad para el punto
                    if (com.TotalComisionCentroServicioVenta != 0)
                    {
                        lstNvNovedades.Add(CrearDataNovedad(giroAnulado, tipoDevolucion, idMotivoNovedad, com.IdCentroServicioVenta,
                                                     com.NombreCentroServicioVenta, com.TotalComisionCentroServicioVenta));
                    }

                    //Novedad para el Responsable
                    if (com.TotalComisionCentroServicioResponsable != 0)
                    {
                        lstNvNovedades.Add(CrearDataNovedad(giroAnulado, tipoDevolucion, idMotivoNovedad, com.IdCentroServicioResponsable,
                                                                 com.NombreCentroServicioResponsable, com.TotalComisionCentroServicioResponsable));
                    }
                });
            }

            return lstNvNovedades;
        }

        /// <summary>
        /// Crea la Data para generar una
        /// novedad
        /// </summary>
        /// <param name="giroAnulado">info del giro</param>
        /// <param name="tipoDevolucion">es el tipo de devolucion</param>
        /// <param name="idCentroServicio"></param>
        /// <param name="nombreCentroServicio"></param>
        /// <param name="totalComision"></param>
        /// <returns>info de una novedad</returns>
        private PANovedadCentroServicioDCDeprecated CrearDataNovedad(CARegistroTransacCajaDC giroAnulado, string tipoDevolucion, int idmotivoNovedad,
                                                            long idCentroServicio, string nombreCentroServicio, decimal totalComision)
        {
            PANovedadCentroServicioDCDeprecated nvNovedad = new PANovedadCentroServicioDCDeprecated()
            {
                MotivoNovedad = new PRMotivoNovedadDCDeprecated()
                {
                    IdMotivoNovedad = idmotivoNovedad,
                },
                IdCentroServicios = idCentroServicio,
                NombreCentroServicios = nombreCentroServicio,
                Valor = totalComision,
                IdProduccion = 0,
                FechaAplicacionPr = ConstantesFramework.MinDateTimeController,
                EstaAprobada = true,
                Observaciones = string.Format("{0} {1}, giro N° {2}", tipoDevolucion, giroAnulado.RegistrosTransacDetallesCaja.FirstOrDefault().ConceptoCaja.Descripcion,
                                                                                  giroAnulado.RegistrosTransacDetallesCaja.FirstOrDefault().Numero),
                FechaGrabacion = DateTime.Now
            };
            return nvNovedad;
        }

        #endregion Otros Modulos

        #region Conceptos Caja

         /// <summary>
        /// retorna los conceptos filtrados por categoria
        /// </summary>
        /// <param name="categoria"></param>
        /// <returns></returns>
        public List<CAConceptoCajaDC> ObtenerConceptosCaja(CAEnumCategoriasConceptoCaja categoria)
        {
            return CARepositorioGestionCajas.Instancia.ObtenerConceptosCaja(categoria);
        }

        /// <summary>
        /// Consulta los conceptos de caja filtrados, paginas y ordenados
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <returns></returns>
        public List<CAConceptoCajaDC> ObtenerConceptosCaja(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return CARepositorioGestionCajas.Instancia.ObtenerConceptosCaja(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Se aplican cambios realizados sobre un concepto de caja
        /// </summary>
        /// <param name="conceptoCaja"></param>
        public void ActualizarConceptoCaja(CAConceptoCajaDC conceptoCaja)
        {
            using (TransactionScope tran = new TransactionScope())
            {
                CARepositorioGestionCajas.Instancia.ActualizarConceptoCaja(conceptoCaja);
                /// actualiza la tabla de relacion del concepto con la
                /// categoria
                CARepositorioGestionCajas.Instancia.RemoverConceptoCategoria(conceptoCaja);
                foreach (CAConceptoCajaCategoriaDC categoria in conceptoCaja.GruposCategorias)
                {
                    CARepositorioGestionCajas.Instancia.AdicionarConceptoCategoria(categoria, conceptoCaja.IdConceptoCaja);
                }
                tran.Complete();
            }
        }

        /// <summary>
        /// Obtiene las categorias de los conceptos
        /// </summary>
        /// <returns>lista ordenada de Categorias</returns>
        public List<CAConceptoCajaCategoriaDC> ObtenerCategoriaConceptosCaja()
        {
            return CARepositorioGestionCajas.Instancia.ObtenerCategoriaConceptosCaja();
        }

        /// <summary>
        /// Se inserta un concepto de caja nuevo
        /// </summary>
        /// <param name="conceptoCaja"></param>
        public void AdicionarConceptoCaja(CAConceptoCajaDC conceptoCaja)
        {
            using (TransactionScope tran = new TransactionScope())
            {
                CARepositorioGestionCajas.Instancia.AdicionarConceptoCaja(conceptoCaja);
                CARepositorioGestionCajas.Instancia.RemoverConceptoCategoria(conceptoCaja);
                foreach (CAConceptoCajaCategoriaDC categoria in conceptoCaja.GruposCategorias)
                {
                    CARepositorioGestionCajas.Instancia.AdicionarConceptoCategoria(categoria, conceptoCaja.IdConceptoCaja);
                }
                tran.Complete();
            }
        }

        #endregion Conceptos Caja

        #region Consultar caja afectada al cobro
        /// <summary>
        /// Consulta el Centro de servicio cuya caja se afectó por concepto de pago de al cobro
        /// </summary>
        /// <param name="numeroAlCobro"></param>
        /// <returns></returns>
        public PUCentroServiciosDC ConsultarCajaAfectadaPorPagoDeAlCobro(long numeroAlCobro, out decimal valorTotal)
        {
            return CARepositorioCaja.Instancia.ConsultarCajaAfectadaPorPagoDeAlCobro(numeroAlCobro, out valorTotal);
        }
        #endregion

        #region Procesos Solicitudes

        /// <summary>
        /// Realiza el proceso del
        /// calculo d la comision y
        /// registro en caja de la transaccion
        /// </summary>
        /// <param name="numeroGiro">numero del giro</param>
        /// <param name="idConceptoCaja">concepto de caja de giro</param>
        public void AnularGiroCaja(long numeroGiro, int idConceptoCaja)
        {
            //Obtengo la Info de la Transaccion
            CARegistroTransacCajaDC giroAnulado = ObtenerTransaccionCajaAnulacionGiro(numeroGiro, idConceptoCaja);

            ///Actualizo el Concepto de Devolución
            if (giroAnulado != null && giroAnulado.RegistrosTransacDetallesCaja != null)
            {
                ActualizarConceptoAnulacion(giroAnulado, CAEnumConceptosCaja.ANULACION_GIRO);
            }

            List<PANovedadCentroServicioDCDeprecated> lstNovedades = ConsultarComisionesCtroSrvYResponsable(giroAnulado, "Anulación por",
                                                                        (int)PREnumNovedadMotivoDeprecated.NOVEDAD_POR_ANULACION_DEL_GIRO);

            //Se registra la transaccion en Caja por la Anulacion del Giro
            RegistrarTransaccionPorAnulacionDevolucion(giroAnulado, lstNovedades);
        }

        /// <summary>
        /// Proceso de Devolucion en Caja cobrando
        /// el servicio del giro
        /// </summary>
        /// <param name="idGiro">id del giro</param>
        /// <param name="idConcepto">id concepto</param>
        public void DevolucionSinPorteGiro(GIAdmisionGirosDC infoGiro, int idConcepto, long idCentroServicios)
        {
            ICMFachadaComisiones fachadaComisiones = COFabricaDominio.Instancia.CrearInstancia<ICMFachadaComisiones>();

            //Obtengo la Info de la Transaccion
            CARegistroTransacCajaDC giroDevuelto = ObtenerTransaccionCajaAnulacionGiro(infoGiro.IdGiro.Value, idConcepto);

            
            giroDevuelto.ValorTotal = infoGiro.Precio.ValorGiro;
            giroDevuelto.RegistrosTransacDetallesCaja.FirstOrDefault().ValorTercero = infoGiro.Precio.ValorGiro;
            giroDevuelto.RegistrosTransacDetallesCaja.FirstOrDefault().ValorServicio = infoGiro.Precio.ValorServicio;
            giroDevuelto.InfoAperturaCaja.IdCaja = infoGiro.IdCaja;
            giroDevuelto.IdCentroServiciosVenta = idCentroServicios;

            ///Actualizo el Concepto de Devolución
            if (giroDevuelto != null && giroDevuelto.RegistrosTransacDetallesCaja != null)
            {
                ActualizarConceptoAnulacion(giroDevuelto, CAEnumConceptosCaja.DEVOLUCION_GIRO_POSTAL);
            }

            // No se afecta el Valor del porte se deja en Cero 0 ya
            //que es una devolucion solicitada por el Remitente y
            //se cobra el servicio del giro
            giroDevuelto.RegistrosTransacDetallesCaja.ForEach(g =>
            {
                g.ValorServicio = 0;
            });

            // se graba solo el valor del tercero en las formas de pago
            giroDevuelto.RegistroVentaFormaPago.ForEach(val =>
            {
                if (val.IdFormaPago == (int)TAEnumFormaPago.EFECTIVO)
                {
                    val.Valor = giroDevuelto.RegistrosTransacDetallesCaja.FirstOrDefault().ValorTercero;
                }
            });

            List<PANovedadCentroServicioDCDeprecated> lstNovedades = ConsultarComisionesCtroSrvYResponsable(giroDevuelto, "Devolución por",
                                                                        (int)PREnumNovedadMotivoDeprecated.NOVEDAD_POR_DEVOLUCION_DEL_GIRO);

            //Se registra la transaccion en Caja por la devolucion del Giro
            RegistrarTransaccionPorAnulacionDevolucion(giroDevuelto, lstNovedades);
        }

        /// <summary>
        /// Proceso de Devolucion total en Caja
        /// por el giro
        /// </summary>
        /// <param name="idGiro">id del giro</param>
        /// <param name="idConcepto">id concepto</param>
        public void DevolucionConPorteGiro(GIAdmisionGirosDC infoGiro, int idConcepto, long idCentroServicios)
        {
            //Obtengo la Info de la Transaccion
            CARegistroTransacCajaDC giroDevuelto = ObtenerTransaccionCajaAnulacionGiro(infoGiro.IdGiro.Value, idConcepto);

            giroDevuelto.ValorTotal = infoGiro.Precio.ValorGiro;
            giroDevuelto.RegistrosTransacDetallesCaja.FirstOrDefault().ValorTercero = infoGiro.Precio.ValorGiro;
            giroDevuelto.RegistrosTransacDetallesCaja.FirstOrDefault().ValorServicio = infoGiro.Precio.ValorServicio;
            giroDevuelto.InfoAperturaCaja.IdCaja = infoGiro.IdCaja;
            giroDevuelto.IdCentroServiciosVenta = idCentroServicios;

            ///Actualizo el Concepto de Devolución
            if (giroDevuelto != null && giroDevuelto.RegistrosTransacDetallesCaja != null)
            {
                ActualizarConceptoAnulacion(giroDevuelto, CAEnumConceptosCaja.DEVOLUCION_PORTE_DE_GIRO);
            }

            // No se afecta el Valor del porte se deja en Cero 0 ya
            //que es una devolucion solicitada por el Remitente y
            //se cobra el servicio del giro
            giroDevuelto.RegistrosTransacDetallesCaja.ForEach(g =>
            {
                g.ValorTercero = 0;
            });

            // se graba solo el valor del porte ó servicio en las formas de pago
            giroDevuelto.RegistroVentaFormaPago.ForEach(val =>
            {
                if (val.IdFormaPago == (int)TAEnumFormaPago.EFECTIVO)
                {
                    val.Valor = giroDevuelto.RegistrosTransacDetallesCaja.FirstOrDefault().ValorServicio;
                    giroDevuelto.ValorTotal = val.Valor;
                }
            });

            List<PANovedadCentroServicioDCDeprecated> lstNovedades = ConsultarComisionesCtroSrvYResponsable(giroDevuelto, "Devolución por",
                                                                        (int)PREnumNovedadMotivoDeprecated.NOVEDAD_POR_DEVOLUCION_DEL_GIRO);

            //Se registra la transaccion en Caja por la devolucion del Giro
            RegistrarTransaccionPorAnulacionDevolucion(giroDevuelto, lstNovedades);
        }

        /// <summary>
        /// registra la transaccion en Caja y en la tabla de
        /// comisiones por la Anulacion o Devolucion del Giro
        /// </summary>
        /// <param name="giroAnulado">info del giro</param>
        /// <param name="comision">info de la comision</param>
        private static void RegistrarTransaccionPorAnulacionDevolucion(CARegistroTransacCajaDC giroAnulado, List<PANovedadCentroServicioDCDeprecated> lstNovedades)
        {
            IPRFachadaProduccion fachadaProduccion = COFabricaDominio.Instancia.CrearInstancia<IPRFachadaProduccion>();

            using (TransactionScope trans = new TransactionScope())
            {
                //registro la transaccion en Caja
                CAAdministradorCajas.Instancia.AdicionarMovimientoCaja(giroAnulado, null);

                if (lstNovedades != null && lstNovedades.Count > 0)
                {
                    lstNovedades.ForEach(nov =>
                    {
                        //Crea una novedad para ajustar la Comisión del centro de servicio y del Responsable por la Venta
                        fachadaProduccion.AdicionarNovedadCentroServicio(nov);
                    });
                }

                trans.Complete();
            }
        }

        /// <summary>
        /// Metodo para realizar el ajuste del valor del giro en Caja
        /// </summary>
        /// <param name="solicitudAtendida">info de la Solicitud</param>
        public void AjustarValorGiroCaja(GISolicitudGiroDC solicitudAtendida)
        {
            CAConceptoCajaDC conceptoCajaAjusteGiro = ObtenerConceptoPorId((int)CAEnumConceptosCaja.AJUSTE_A_VALOR_DE_GIRO);
            CAConceptoCajaDC conceptoCajaAjustePorte = ObtenerConceptoPorId((int)CAEnumConceptosCaja.RELIQUIDACION_PORTE_EN_AJUSTE_DE_VALOR);

            CARegistroTransacCajaDC transaccionAjusteGiro = null;
            CARegistroTransacCajaDC transaccionAjustePorte = null;

            //Si el valor del giro se debe de aumentar se realiza un ingreso
            //ó egreso segun el ajuste
            if (solicitudAtendida.AjusteAlGiro > 0)
            {
                conceptoCajaAjusteGiro.EsIngreso = true;
                transaccionAjusteGiro = ArmarTransaccionAjuste(solicitudAtendida, conceptoCajaAjusteGiro,
                                                               solicitudAtendida.AjusteAlGiro, CAConstantesCaja.VALOR_CERO_DECIMAL);
            }

            else
            {
                conceptoCajaAjusteGiro.EsIngreso = false;
                transaccionAjusteGiro = ArmarTransaccionAjuste(solicitudAtendida, conceptoCajaAjusteGiro,
                                                               (solicitudAtendida.AjusteAlGiro) * -1, CAConstantesCaja.VALOR_CERO_DECIMAL);
            }

            //Si el valor del porte se debe de aumentar se realiza un ingreso
            //ó egreso segun el ajuste
            if (solicitudAtendida.AjusteAlPorte > 0)
            {
                conceptoCajaAjustePorte.EsIngreso = true;
                transaccionAjustePorte = ArmarTransaccionAjuste(solicitudAtendida, conceptoCajaAjustePorte,
                                                                CAConstantesCaja.VALOR_CERO_DECIMAL, solicitudAtendida.AjusteAlPorte);
            }
            else
            {
                conceptoCajaAjustePorte.EsIngreso = false;
                transaccionAjustePorte = ArmarTransaccionAjuste(solicitudAtendida, conceptoCajaAjustePorte,
                                                                CAConstantesCaja.VALOR_CERO_DECIMAL, (solicitudAtendida.AjusteAlPorte) * -1);
            }

            if (transaccionAjusteGiro != null && transaccionAjusteGiro.ValorTotal != 0)
                CAAdministradorCajas.Instancia.AdicionarMovimientoCaja(transaccionAjusteGiro, null);

            if (transaccionAjustePorte != null && transaccionAjustePorte.ValorTotal != 0)
                CAAdministradorCajas.Instancia.AdicionarMovimientoCaja(transaccionAjustePorte, null);
        }

        /// <summary>
        /// Crea la transaccion del Caja para el ajuste
        /// del giro ó del porte
        /// </summary>
        /// <param name="solicitudAtendida">info de la Solicitud</param>
        /// <param name="conceptoCajaAjusteGiro">tipo de Concepto de Ajuste</param>
        /// <param name="valorAjuste">Valor a Ajustar</param>
        /// <returns>la Transaccion Armada</returns>
        private CARegistroTransacCajaDC ArmarTransaccionAjuste(GISolicitudGiroDC solicitudAtendida, CAConceptoCajaDC conceptoCajaAjusteGiro,
                                                              decimal valorAjusteGiro, decimal valorAjustePorte)
        {
            CARegistroTransacCajaDC registroValorAjuste = null;

            //Creacion de la transaccion caja
            return registroValorAjuste = new CARegistroTransacCajaDC()
            {
                IdCentroServiciosVenta = solicitudAtendida.AdmisionGiro.AgenciaOrigen.IdCentroServicio,
                NombreCentroServiciosVenta = solicitudAtendida.AdmisionGiro.AgenciaOrigen.Nombre,
                IdCentroResponsable = solicitudAtendida.AdmisionGiro.AgenciaOrigen.IdCentroServicio,
                NombreCentroResponsable = solicitudAtendida.AdmisionGiro.AgenciaOrigen.Nombre,
                ValorTotal = valorAjusteGiro + valorAjustePorte,
                TotalImpuestos = CAConstantesCaja.VALOR_CERO_DECIMAL,
                TotalRetenciones = CAConstantesCaja.VALOR_CERO_DECIMAL,
                TipoDatosAdicionales = CAEnumTipoDatosAdicionales.PEA,
                Usuario = ControllerContext.Current.Usuario,
                InfoAperturaCaja = new CAAperturaCajaDC()
                {
                    IdCaja = 0,
                    IdCodigoUsuario = ControllerContext.Current.CodigoUsuario
                },
                RegistrosTransacDetallesCaja = new List<CARegistroTransacCajaDetalleDC>()
          {
            new CARegistroTransacCajaDetalleDC()
            {
              ConceptoCaja= new CAConceptoCajaDC()
              {
                IdConceptoCaja=conceptoCajaAjusteGiro.IdConceptoCaja,
                EsIngreso=conceptoCajaAjusteGiro.EsIngreso
              },
              Cantidad=1,
              EstadoFacturacion= CAEnumEstadoFacturacion.FAC,
              FechaFacturacion= DateTime.Now,
              ValorTercero=valorAjusteGiro,
              ValorImpuestos=CAConstantesCaja.VALOR_CERO_DECIMAL,
              ValorPrimaSeguros=CAConstantesCaja.VALOR_CERO_DECIMAL,
              ValorRetenciones=CAConstantesCaja.VALOR_CERO_DECIMAL,
              Numero=solicitudAtendida.AdmisionGiro.IdGiro.Value,
              NumeroFactura=solicitudAtendida.AdmisionGiro.IdGiro.ToString(),
              ValorDeclarado=CAConstantesCaja.VALOR_CERO_DECIMAL,
              ValoresAdicionales=CAConstantesCaja.VALOR_CERO_DECIMAL,
              ValorServicio=valorAjustePorte,
              Observacion=conceptoCajaAjusteGiro.Nombre,
              ConceptoEsIngreso=conceptoCajaAjusteGiro.EsIngreso
            }
          },
                RegistroVentaFormaPago = new List<CARegistroVentaFormaPagoDC>()
          {
            new CARegistroVentaFormaPagoDC()
            {
              IdFormaPago = TAConstantesServicios.ID_FORMA_PAGO_CONTADO,
              Descripcion = TAConstantesServicios.DESCRIPCION_FORMA_PAGO_CONTADO,
              Valor = valorAjusteGiro + valorAjustePorte,
              NumeroAsociado = string.Empty,
              Campo01 = string.Empty,
              Campo02 = string.Empty,
            }
          }
            };
        }
        /// <summary>
        /// metodo que inserta un movimiento para que se descuente un movimiento al cobro por guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="observacion"></param>
        /// <param name="descripcion"></param>
        /// <param name="idPersonaInterna"></param>
        /// <param name="idCentroServicios"></param>
        public bool InsertarDescuentoAlCobroDevuelto(long numeroGuia, long idCentroServicios, long idAdmisionMensajeria)
        {
            bool respuesta = false;
                CAAperturaCajaDC apertura = new CAAperturaCajaDC
                {
                    IdCaja = 0,
                    IdCodigoUsuario = ControllerContext.Current.CodigoUsuario,
                };

                long idApertura = CAApertura.Instancia.ValidarAperturaCajaCentroServicios(apertura, idCentroServicios);

                using (TransactionScope trans = new TransactionScope())
                {
                if (CARepositorioCaja.Instancia.InsertarDescuentoAlCobroDevuelto(numeroGuia, idCentroServicios, idApertura))
                {
                    fachadaMensajeria.ActualizarPagadoGuia(idAdmisionMensajeria, false);
                    respuesta = true;
                }
                 trans.Complete();
             };

                return respuesta;
        }

        #endregion Procesos Solicitudes
    }
}