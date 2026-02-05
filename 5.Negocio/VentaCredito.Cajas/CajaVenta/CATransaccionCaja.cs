using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Framework.Servidor.ParametrosFW;
using Servicio.Entidades.Cajas;
using Servicio.Entidades.GestionCajas;
using Servicio.Entidades.Tarifas.Precios;
using VentaCredito.Cajas.CajaFinaciera;
using VentaCredito.Cajas.Datos.Repositorio;
using VentaCredito.Cajas.Comun;
using System.ServiceModel;
using System.Diagnostics;
using Servicio.Entidades.Cajas.IntegracionApi;
using RestSharp;
using Newtonsoft.Json;
using Framework.Servidor.Excepciones;
using Servicios.Entidades.Cajas;
using VentaCredito.Tarifas;
using Servicio.Entidades.Admisiones.Mensajeria;
using VentaCredito.Transversal;

using Framework.Servidor.Servicios.ContratoDatos.Seguridad;
using CO.Servidor.Dominio.Comun.Util;
using Framework.Servidor.Comun.Util;

namespace VentaCredito.Cajas.CajaVenta
{
    public class CATransaccionCaja
    {

        private static CATransaccionCaja instancia = new CATransaccionCaja();


        public static CATransaccionCaja Instancia
        {
            get
            {
                return CATransaccionCaja.instancia;
            }
        }

        /// <summary>
        /// Adicionar el movimiento caja.
        /// </summary>
        /// <param name="movimientoCaja">The movimiento caja.</param>
        /// <param name="anulacionGuia">indica si se esta anulando una guia</param>
        public CAIdTransaccionesCajaDC AdicionarMovimientoCaja(ADGuia guia,bool anulacionGuia = false)
        {
            CAIdTransaccionesCajaDC caja = new CAIdTransaccionesCajaDC();
            var resultadoMapper = MapperCAIdTransaccionesCajaDC(guia);
            AdicionarMovimientoCajaApiAsync(resultadoMapper, anulacionGuia);
            caja.EnumOrigenCaja = CAEnumOrigenCaja.POS_WEB;

            return caja;
        }

        private CARegistroTransacCajaDC MapperCAIdTransaccionesCajaDC(ADGuia guia)
        {
            return new CARegistroTransacCajaDC
            {
                RegistrosTransacDetallesCaja = new List<CARegistroTransacCajaDetalleDC>
                {
                    new CARegistroTransacCajaDetalleDC
                    {
                        IdRegistroTranscaccion = 0,
                        ConceptoCaja = new CAConceptoCajaDC
                        {
                            IdConceptoCaja = TATrayecto.Instancia.ObtenerConceptoCaja(3),
                            ContraPartidaCasaMatriz = false,
                            ContraPartidaCS = false,
                            CuentaExterna = new CACuentaExterna
                            {
                                CodCtaExterna = "5454",
                                Descripcion = "",
                                Id = 5
                            },
                            Descripcion = "",
                            EsEgreso = true,
                            EsIngreso = false,
                            EsServicio = true,
                            GruposCategorias = new System.Collections.ObjectModel.ObservableCollection<CAConceptoCajaCategoriaDC>(),
                            IdCategoriaAnterior = 1,
                            IdCuentaExterna = 5,
                            IdDuplaConceptoCaja = 3,
                            Nombre = "",
                            RequiereNoDocumento = false
                        },
                        
                        ValorServicio = guia.ValorServicio,
                        ValorTercero = 0,
                        ValorImpuestos = 0,
                        ValorRetenciones = 0,
                        ValorPrimaSeguros = guia.ValorPrimaSeguro,
                        NumeroFactura = guia.NumeroGuia.ToString(),
                        NumeroComprobante = "0",
                        Descripcion = guia.Observaciones,
                        Cantidad = 1,
                        ConceptoEsIngreso = true,
                        DetalleAdicional = new CARegistroTransacCajaDetallAdicionalDC
                        {
                            Adicional01 = "",
                            Adicional02 = "",
                            Adicional03 = "",
                            IdRegistroTransDetalle = 124548,
                            IdSucursal = 2154
                        },
                        EstadoFacturacion = CAEnumEstadoFacturacion.FAC,
                        FechaFacturacion = DateTime.Now,
                        LtsImpuestos = new List<CARegistroTranscDtllImpuestoDC>
                        {
                            new CARegistroTranscDtllImpuestoDC
                            {
                                InfoImpuesto = new Servicio.Entidades.Tarifas.TAImpuestoDelServicio
                                {
                                    DescripcionImpuesto = "",
                                    EstadoRegistro  = Framework.Servidor.Comun.EnumEstadoRegistro.MODIFICADO,
                                    IdImpuesto = 1,
                                    IdServicio = 3,
                                    ValorImpuesto = 5000,
                                    ValorImpuestoAplicado = 1000
                                },
                                ValorImpuestoLiquidado = 1000,
                            }
                        },
                        Numero = 598798,
                        Observacion = "",
                        ValorDeclarado = 5000,
                        ValoresAdicionales = 5000
                    },
                },
                IdCentroServiciosDestinoGuia = guia.IdCentroServicioDestino,
                IdCentroServiciosVenta = guia.IdCentroServicioOrigen,
                ValorTotal = guia.ValorTotal,
                CentroCostos = "1287",
                NombreCentroResponsable = "PRUEBA ADMISION VENTA CREDITO",
                NombreCentroServiciosVenta = "PRUEBA ADMISION VENTA CREDITO",
                IdCasaMatriz = 1,
                IdCentroResponsable = guia.IdCentroServicioOrigen,
                InfoAperturaCaja = new CAAperturaCajaDC
                {
                    IdAperturaCaja = 0,
                    CreadoPor = "admin",
                    BaseInicialApertura = 100000,
                    DocumentoUsuario = "PRUEBA VENTA CREDITO",
                    EstaAbierta = true,
                    IdCaja = 0,
                    IdCodigoUsuario = 1,
                    NombresUsuario = "admin"
                },
                Usuario = "admin",
                RegistroVentaClienteCredito = new CARegistroTransClienteCreditoDC
                {
                    IdCliente = 1101,
                    IdContrato = guia.IdContrato,
                    IdSucursal = 1245,
                    NitCliente = "800125487",
                    NombreCliente = "PRUEBA VENTA CREDITO",
                    NombreSucursal = "PRUEBA VENTA CREDITO",
                    NumeroContrato = "025487"
                },
                EsTransladoEntreCajas = true,
                EsUsuarioGestion = true,
                TipoDatosAdicionales = CAEnumTipoDatosAdicionales.CRE,
                TotalImpuestos = 1000,
                TotalRetenciones = 1000,
                RegistroVentaFormaPago = new List<CARegistroVentaFormaPagoDC>
                {
                    new CARegistroVentaFormaPagoDC
                    {
                        IdFormaPago = 1,
                        Campo01 = "",
                        Campo02 = "",
                        Descripcion = "",
                        NumeroAsociado = "54545",
                        Valor = 5000
                    },
                }
            };
        }

        /// <summary>
        /// Ejecuta de forma asíncrona el método Movimiento Caja API
        /// </summary>
        /// <returns></returns>
        [DebuggerStepThrough]
        internal static void AdicionarMovimientoCajaApiAsync(CARegistroTransacCajaDC movimientoCaja, bool anulacionGuia = false)
        {
            string usuario = ContextoSitio.Current.Usuario;
            long codigoUsuario = 1;
            System.Threading.Tasks.Task t = new System.Threading.Tasks.Task(() =>
            {
                try
                {
                    AdicionarMovimientoCajaApi(movimientoCaja, usuario, codigoUsuario, anulacionGuia);
                }
                catch (Exception e)
                {
                    UtilidadesFW.AuditarExcepcion(e);
                    var respuesta = string.Format("Error procesando Ingreso:\nMensaje: {0}\n StackTrace: {1}", e.Message, e.StackTrace);
                    AuditoriaTrace.AuditarIntegracion("FinancINGRERR", movimientoCaja.RegistrosTransacDetallesCaja.FirstOrDefault().NumeroComprobante, respuesta);
                    throw e;
                }
            });

            t.Start();
        }

        /// <summary>
        /// Adiciona el movimiento caja Integracion API
        /// </summary>
        /// <param name="movimientoCaja"></param>

        public static void AdicionarMovimientoCajaApi(CARegistroTransacCajaDC movimientoCaja, string usuario, long codigoUsuario, bool anulacionGuia = false)
        {
            var urlCajasApi = CACaja.Instancia.ObtenerParametroCajas("ServicioCajasApi");
            var servicio = string.Empty;
            Method metodo = Method.POST;
            var informacion = new { };
            Dictionary<string, Object> header = new Dictionary<string, object>();

            SEUsuarioPorCodigoDC informacionUsuario = new SEUsuarioPorCodigoDC();//SERepositorio.Instancia.ObtenerUsuarioPorIdUsuarioData(usuario);

            CARegistroTransacionCajaDC ListaTransaccionCaja = new CARegistroTransacionCajaDC
            {
                Uuid = (Guid.NewGuid()).ToString(),
                IdTipoTransaccion = (short)(!anulacionGuia ? EnumTipoTransaccion.INGRESO : EnumTipoTransaccion.EGRESO),
                ValorTotalTransaccion = movimientoCaja.ValorTotal,
                IdAperturaCaja = -1, //controller
                IdCSOrigen = movimientoCaja.IdCentroServiciosVenta,
                DocumentoUsuarioResponsable = informacionUsuario.Documento,
                MediosPago = new List<CAMediosPagoDC>
                {
                    new CAMediosPagoDC
                    {
                        IdMedioPago = 1,
                        IdComprobante = "",
                        Monto = movimientoCaja.ValorTotal,
                    }
                },
                IdUsuarioResponsable = codigoUsuario,
                IdConcepto = (short)movimientoCaja.RegistrosTransacDetallesCaja.FirstOrDefault().ConceptoCaja.IdConceptoCaja,
                EsIngreso = !anulacionGuia,
                IdMensajero = 0,
                Observaciones = "[CONTROLLER] - INGRESO CAJA",
                Detalles = new List<CADetalleTransaccionCajaDC> {
                                    new CADetalleTransaccionCajaDC
                                    {
                                        IdDetalleTransaccion = 0,
                                        IdTransaccion = 0,
                                        IdConceptoCaja = TATrayecto.Instancia.ObtenerConceptoCaja(3),
                                        ValorServicio = movimientoCaja.RegistrosTransacDetallesCaja.FirstOrDefault().ValorServicio,
                                        IngresoParaTercero = movimientoCaja.RegistrosTransacDetallesCaja.FirstOrDefault().ValorTercero,
                                        ValorImpuestos = movimientoCaja.RegistrosTransacDetallesCaja.FirstOrDefault().ValorImpuestos,
                                        ValorRetenciones = movimientoCaja.RegistrosTransacDetallesCaja.FirstOrDefault().ValorRetenciones,
                                        ValorSobreFlete = movimientoCaja.RegistrosTransacDetallesCaja.FirstOrDefault().ValorPrimaSeguros,
                                        ValorNetoTransaccion = movimientoCaja.ValorTotal,
                                        NumeroFactura = movimientoCaja.RegistrosTransacDetallesCaja.FirstOrDefault().NumeroFactura,
                                        NumeroComprobante = "0",
                                        Observacion = movimientoCaja.RegistrosTransacDetallesCaja.FirstOrDefault().Descripcion,
                                        IdCentroServicioDestino = movimientoCaja.IdCentroServiciosDestinoGuia,
                                        IdCentroServiciosOrigen = movimientoCaja.IdCentroServiciosVenta,
                                        IdFormaPago = movimientoCaja.RegistroVentaFormaPago.FirstOrDefault().IdFormaPago,
                                    }

                                },
            };

            var parametros = ListaTransaccionCaja;

            string jsonParametros = JsonConvert.SerializeObject(parametros);

            var solicitud = new
            {
                Url = urlCajasApi,
                Servicio = servicio,
                Metodo = metodo,
                Parametros = jsonParametros,
                Informacion = informacion,
                Header = header
            };
            try
            {
                AuditoriaTrace.AuditarIntegracion("FinancINGR", solicitud.ToString(), "Se consume servicio");
                var resultado = Utilidades.EjecutarServicioRestConRespuesta(urlCajasApi, servicio, metodo, parametros, informacion, header);
                AuditoriaTrace.AuditarIntegracion("FinancINGR", solicitud.ToString(), resultado.Content.ToString());
            }
            catch (Exception e)
            {
                Utilidades.AuditarExcepcion(e);
                var respuesta = string.Format("Error procesando Ingreso:\nMensaje: {0}\n StackTrace: {1}", e.Message, e.StackTrace);
                AuditoriaTrace.AuditarIntegracion("FinancINGR", solicitud.ToString(), respuesta);
                throw e;
            }
        }
    }
}
