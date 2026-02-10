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
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.LogisticaInversa.Datos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Dominio.Comun.Cajas;
using CO.Servidor.Dominio.Comun.Suministros;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using CO.Servidor.Servicios.ContratoDatos.Cajas;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Precios;

namespace CO.Servidor.LogisticaInversa.Notificaciones
{
    public class LOICertificacionWeb : ControllerBase
    {
        private static readonly LOICertificacionWeb instancia = (LOICertificacionWeb)FabricaInterceptores.GetProxy(new LOICertificacionWeb(), COConstantesModulos.NOTIFICACIONES);

        public static LOICertificacionWeb Instancia
        {
            get { return LOICertificacionWeb.instancia; }
        }


        #region Fachadas

        private ICAFachadaCajas fachadaCajas = COFabricaDominio.Instancia.CrearInstancia<ICAFachadaCajas>();

        private IADFachadaAdmisionesMensajeria fachadaAdmisiones = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>();

        private ISUFachadaSuministros fachadaSuministros = COFabricaDominio.Instancia.CrearInstancia<ISUFachadaSuministros>();

        private IPUFachadaCentroServicios fachadaCes = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();


        #endregion Fachadas


        #region Consulta impresion



        /// <summary>
        /// Método para consultar la certificación WEB
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="idRemitente"></param>
        /// <param name="idDestinatario"></param>
        /// <returns></returns>
        public ADNotificacion ValidarCertificacionWeb(LIReimpresionCertificacionDC reimprecionCertif)
        {
            ADNotificacion respuesta = new ADNotificacion();
            ADEstadoGuia idEstadoGuia = new ADEstadoGuia();

            //1.Obtener info x centroservicio
            long idCentroServicio = ControllerContext.Current.IdCentroServicio;
            PUCentroServiciosDC ces = fachadaCes.ObtenerInformacionCentroServicioPorId(idCentroServicio);
            string dircentroservicio = ces.Direccion;

            //2.Validar notificacion
            respuesta = ValidarCertificacionWeb(reimprecionCertif.NumeroGuia);
            respuesta.TipoDestino = new Servicios.ContratoDatos.Tarifas.TATipoDestino();
            respuesta.GuiaInterna = new ADGuiaInternaDC();

            //3.valida si ya fue impresa
            respuesta.ExisteGuiaAuditoria = ValidarNotificacionExisteAuditoria(reimprecionCertif.NumeroGuia);
            idEstadoGuia = instancia.ValidarEntregaoDevolucion(reimprecionCertif.NumeroGuia);

            //4. Obtiene Información Guía
            var idAdmision = fachadaAdmisiones.ObtenerGuiaXNumeroGuia(reimprecionCertif.NumeroGuia);
            var infoGuia = fachadaAdmisiones.ObtenerNotificacionGuia(idAdmision.IdAdmision);
            respuesta.TipoDestino.Id = infoGuia.TipoDestino.Id;
            respuesta.GuiaInterna.NumeroGuia = infoGuia.GuiaInterna.NumeroGuia;

            //5. Suministro

            using (TransactionScope transaccion = new TransactionScope())
            {
                respuesta.NumeroSuministro = fachadaSuministros.ObtenerNumeroPrefijoValor(SUEnumSuministro.IMPRESION_NOTIFICACION_PUNTO);
                transaccion.Complete();
            }

            return respuesta;
        }


        /// <summary>
        /// Audita las guias reimpresas
        /// </summary>
        /// <param name="auditoriaReimpresion"></param>
        public void InsertarAuditoriaReimpresion(LIReimpresionCertificacionDC auditoriaReimpresion)
        {
            LIRepositorioNotificaciones.Instancia.InsertarAuditoriaImpresionNotifPunto(auditoriaReimpresion);
        }

        /// <summary>
        /// Realiza la afectación a caja para las reimpresiones
        /// </summary>
        /// <param name="afectacionCajaReimp"></param>
        public void AfectacionCajaCertificacion(ADNotificacion afectacionCajaReimp)
        {
            ADNotificacion respuesta = new ADNotificacion();

            int valorReimpresion = Convert.ToInt32(fachadaAdmisiones.ObtenerParametrosAdmisiones().ValorReimpresionCertificacion);
            long idCentroServicio = ControllerContext.Current.IdCentroServicio;
            PUCentroServiciosDC ces = fachadaCes.ObtenerInformacionCentroServicioPorId(idCentroServicio);

            using (TransactionScope transaccion = new TransactionScope())
            {
                respuesta.NumeroSuministro = fachadaSuministros.ObtenerNumeroPrefijoValor(SUEnumSuministro.IMPRESION_NOTIFICACION_PUNTO);
                transaccion.Complete();
            }
            respuesta.ExisteGuiaAuditoria = ValidarNotificacionExisteAuditoria(afectacionCajaReimp.NumeroGuia);

            AdicionarMovimientoCaja(ces, respuesta, valorReimpresion);
        }


        /// <summary>
        /// Método para consultar la certificación WEB
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="idRemitente"></param>
        /// <param name="idDestinatario"></param>
        /// <returns></returns>
        public ADNotificacion ValidarCertificacionWeb(long numeroGuia)
        {
            ADNotificacion respuesta = new ADNotificacion();
            respuesta.TipoCertificacion = ValidarDatosCertificacion(numeroGuia);

            return respuesta;
        }


        /// <summary>
        /// Metodo para validar la info de la certificacion
        /// </summary>
        /// <returns></returns>
        private LICertificacionWebDC ValidarDatosCertificacion(long numeroGuia)
        {
            ADEstadoGuia idEstadoGuia = new ADEstadoGuia();
            ADGuia Servicio = new ADGuia();
            string imagenEnvio = null;
            long recibido = 0;

            //Validar Servicio
            Servicio = fachadaAdmisiones.ObtenerGuiaXNumeroGuia(numeroGuia);

            if (Servicio.IdServicio == 15)
            {
                //Valida Imagen
                imagenEnvio = instancia.ValidarImagenCertificacionWeb(numeroGuia);

                if (String.IsNullOrEmpty(imagenEnvio))
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.LOGISTICA_INVERSA,
                    LOIEnumTipoErrorLogisticaInversa.EX_GUIA_NO_DIGITALIZADA.ToString(),
                    LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.EX_GUIA_NO_DIGITALIZADA)));
                }
                else
                {
                    idEstadoGuia = instancia.ValidarEntregaoDevolucion(numeroGuia);

                    if (idEstadoGuia.Id == 11)
                    {
                        //Valida Captura Recibido
                        recibido = instancia.ValidarRecibidoCapturado(numeroGuia);

                        if (recibido == 0)
                        {
                            throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.LOGISTICA_INVERSA,
                                LOIEnumTipoErrorLogisticaInversa.EX_ERROR_GUIA_SIN_RECIBIDO_CAPTURADO.ToString(),
                                LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.EX_ERROR_GUIA_SIN_RECIBIDO_CAPTURADO)));
                        }
                        else
                        {
                            return LICertificacionWebDC.Entrega;
                        }
                    }
                    else
                    {
                        if (idEstadoGuia.Id == 10)
                        {
                            return LICertificacionWebDC.Devolucion;
                        }
                        else
                        {
                            return LICertificacionWebDC.SinCertificacion;
                        }
                    }
                }
            }
            else
            {
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.LOGISTICA_INVERSA,
                LOIEnumTipoErrorLogisticaInversa.EX_ERROR_GUIA_NO_ES_NOTIFICACION.ToString(),
                LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.EX_ERROR_GUIA_NO_ES_NOTIFICACION)));
            }
        }




        /// <summary>
        /// Adiciona el movimiento de caja para las reimpresiones desde punto
        /// </summary>
        /// <param name="ces"></param>
        /// <param name="detalle"></param>
        /// <param name="valor"></param>
        private void AdicionarMovimientoCaja(PUCentroServiciosDC ces, ADNotificacion detalle, long valor)
        {
            CARegistroTransacCajaDC registro = new Servidor.Servicios.ContratoDatos.Cajas.CARegistroTransacCajaDC()
            {
                InfoAperturaCaja = new CAAperturaCajaDC()
                {
                    IdCaja = 0,
                    IdCodigoUsuario = ControllerContext.Current.CodigoUsuario,
                },
                TipoDatosAdicionales = CAEnumTipoDatosAdicionales.PEA,
                IdCentroResponsable = ces.IdCentroServicio,
                IdCentroServiciosVenta = ces.IdCentroServicio,
                NombreCentroResponsable = ces.Nombre,
                NombreCentroServiciosVenta = ces.Nombre,
                RegistrosTransacDetallesCaja = new List<Servidor.Servicios.ContratoDatos.Cajas.CARegistroTransacCajaDetalleDC>()
        {
          new CO.Servidor.Servicios.ContratoDatos.Cajas.CARegistroTransacCajaDetalleDC() {
             Cantidad = 1,
             ConceptoCaja = new CAConceptoCajaDC() { IdConceptoCaja = 177 },
             ConceptoEsIngreso = true,
             EstadoFacturacion = CAEnumEstadoFacturacion.FAC,
             FechaFacturacion = DateTime.Now,
             Numero = detalle.ExisteGuiaAuditoria,
             NumeroComprobante = detalle.NumeroSuministro.ValorActual.ToString(),
             NumeroFactura = detalle.ExisteGuiaAuditoria.ToString(),
             Observacion = "Movimiento caja reimpresion certificacion guia No. " + detalle.ExisteGuiaAuditoria.ToString(),
             ValorDeclarado =0,
             ValoresAdicionales =0,
             ValorImpuestos = 0,
             ValorPrimaSeguros = 0,
             ValorServicio = valor,
             ValorTercero = 0
          }
        },
                ValorTotal = valor,
                TotalImpuestos = 0,
                TotalRetenciones = 0,
                Usuario = ControllerContext.Current.Usuario,
                RegistroVentaFormaPago = new List<CARegistroVentaFormaPagoDC>
                {
                 new CARegistroVentaFormaPagoDC
                    {
                        Valor = valor,
                        IdFormaPago = Convert.ToInt16(ConstantesServicios.ID_FORMA_PAGO_CONTADO),
                        Descripcion = ConstantesServicios.DESCRIPCION_FORMA_PAGO_CONTADO,
                        NumeroAsociado = detalle.ExisteGuiaAuditoria.ToString(),
                    }
                },

            };
            fachadaCajas.AdicionarMovimientoCaja(registro);
        }


        /// <summary>
        /// Método para validar si un envío tiene certifiación de entrega o de devolución
        /// </summary>
        /// <param name="NumeroGuia"></param>
        /// <returns></returns>
        public ADEstadoGuia ValidarEntregaoDevolucion(long NumeroGuia)
        {
            return LIRepositorioNotificaciones.Instancia.ValidarEntregaoDevolucion(NumeroGuia);
        }

        /// <summary>
        /// Método que valida si existe la imagen de un envío
        /// </summary>
        /// <param name="NumeroGuia"></param>
        /// <returns></returns>
        public string ValidarImagenCertificacionWeb(long NumeroGuia)
        {
            return LIRepositorioNotificaciones.Instancia.ValidarImagenCertificacionWeb(NumeroGuia);
        }

        /// <summary>
        /// Método para validar el Recibido Capturado
        /// </summary>
        /// <param name="NumeroGuia"></param>
        /// <returns></returns>
        public long ValidarRecibidoCapturado(long NumeroGuia)
        {
            return LIRepositorioNotificaciones.Instancia.ValidarRecibidoCapturado(NumeroGuia);
        }




        /// <summary>
        /// Método para validar si la Certificación ya ha sido impresa con anterioridad por el punto
        /// </summary>
        /// <param name="numeroguia"></param>
        /// <returns></returns>
        public long ValidarNotificacionExisteAuditoria(long numeroguia)
        {
            return LIRepositorioNotificaciones.Instancia.ValidarNotificacionExisteAuditoria(numeroguia);
        }


        /// <summary>
        /// Método que actualiza el campo está devuelta en la tabla Admision Notificaciones
        /// </summary>
        /// <param name="numeroguia"></param>
        public void ActualizaEstaDevueltaADMNotif(long numeroguia)
        {
            LIRepositorioNotificaciones.Instancia.ActualizaEstaDevueltaADMNotif(numeroguia);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="numGuia"></param>
        /// <returns></returns>
        public ADNotificacion ObtenerDatosFacturaReImpresionNotificacion(long numGuia)
        {
            return LIRepositorioNotificaciones.Instancia.ObtenerDatosFacturaReImpresionNotificacion(numGuia);
        }

        #endregion
    }
}
