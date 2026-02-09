using CO.Controller.Servidor.Integraciones;
using CO.Servidor.Dominio.Comun.AdmEstadosGuia;
using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Dominio.Comun.Cajas;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Comisiones;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.OperacionUrbana;
using CO.Servidor.Dominio.Comun.Suministros;
using CO.Servidor.Dominio.Comun.Tarifas;
using CO.Servidor.LogisticaInversa.Datos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Cajas;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Comisiones;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.ServiceModel;
using System.Threading;
using System.Transactions;
using CO.Servidor.LogisticaInversa.Comun;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using CO.Servidor.Dominio.Comun.Util;
using Framework.Servidor.Comun.Util;
using CO.Servidor.LogisticaInversa.PruebasEntrega.Descargue;

namespace CO.Servidor.LogisticaInversa.PruebasEntrega.ReclameEnOficina
{
    public class LIReclameEnOficina : ControllerBase
    {
        #region Instancia

        private static readonly LIReclameEnOficina instancia = (LIReclameEnOficina)FabricaInterceptores.GetProxy(new LIReclameEnOficina(), COConstantesModulos.PRUEBAS_DE_ENTREGA);

        public static LIReclameEnOficina Instancia
        {
            get { return LIReclameEnOficina.instancia; }
        }

        public LIReclameEnOficina()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(PAAdministrador.Instancia.ConsultarParametrosFramework("Cultura"));
        }

        #endregion Instancia

        #region Fachadas

        private ISUFachadaSuministros fachadaSuministros = COFabricaDominio.Instancia.CrearInstancia<ISUFachadaSuministros>();

        private IADFachadaAdmisionesMensajeria fachadaMensajeria = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>();

        private ICMFachadaComisiones fachadaComisiones = COFabricaDominio.Instancia.CrearInstancia<ICMFachadaComisiones>();

        private IOUFachadaOperacionUrbana fachadaOpUrbana = COFabricaDominio.Instancia.CrearInstancia<IOUFachadaOperacionUrbana>();

        private IPUFachadaCentroServicios fachadaCentroServicios = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();

        #endregion Fachadas



        /// <summary>
        /// Metodo para consultar los envios reclame en oficina por punto y tipoMovimiento
        /// </summary>
        /// <param name="ReclameEnOficina"></param>
        /// <returns></returns>
        public List<LIReclameEnOficinaDC> ConsultarGuiasReclameEnOficina(long idCentroServicio, int idTipoMovimiento, bool filtroDevolucion)
        {
            List<LIReclameEnOficinaDC> listaRetorno = new List<LIReclameEnOficinaDC>();
            listaRetorno = LIRepositorioPruebasEntrega.Instancia.ConsultarGuiasReclameEnOficina(idCentroServicio, idTipoMovimiento, filtroDevolucion);
            if (idTipoMovimiento == 2)
            {
                listaRetorno.ForEach(obj =>
                   {
                       if (obj.DiasTranscurridos == 25 && LIRepositorioPruebasEntrega.Instancia.ValidarMensajeReo_LOI(obj.NumeroGuia))
                       {
                           AdministradorMensajesTexto.Instancia.EnviarMensajeTexto(LOIEnumMensajesTexto.Vencimientoreo, obj.TelefonoDestinatario, obj.NumeroGuia);
                       }
                   });
            }

            return listaRetorno;
        }

        /// <summary>
        /// consulta las guias reclame en oficina utilizando filtros
        /// </summary>
        /// <param name="ReclameEnOficina"></param>
        /// <returns></returns>
        public List<LIReclameEnOficinaDC> ConsultarGuiasReclameEnOficinaFiltros(long idCentroServicio, Dictionary<string, string> filtros)
        {
            return LIRepositorioPruebasEntrega.Instancia.ConsultarGuiasReclameEnOficinaFiltros(idCentroServicio, filtros);
        }

        /// <summary>
        /// Metodo para consultar los totales de los  envios Asignados, Ingresados y para Devolucion de reclame en oficina por punto
        /// </summary>
        /// <param name="ReclameEnOficina"></param>
        /// <returns></returns>
        public Dictionary<string, int> ConsultarContadoresGuiasReclameEnOficina(long idCentroServicio)
        {
            return LIRepositorioPruebasEntrega.Instancia.ConsultarContadoresGuiasReclameEnOficina(idCentroServicio);
        }

        /// <summary>
        /// INGRESAR Guia REO a PRO
        /// </summary>
        /// <param name="guiaReclameOficina"></param>
        /// <returns></returns>
        public LIReclameEnOficinaDC AdicionarReclameEnOficina(LIReclameEnOficinaDC guiaReclameOficina)
        {

            ADTrazaGuia estadoGuia;
            PUMovimientoInventario tipoMovimientoActual = LIRepositorioPruebasEntrega.Instancia.ConsultaUltimoMovimientoBodegaGuia(guiaReclameOficina.MovimientoInventario.NumeroGuia);

            if (!(tipoMovimientoActual.TipoMovimiento == PUEnumTipoMovimientoInventario.Asignacion
                && tipoMovimientoActual.Bodega.IdCentroServicio == ControllerContext.Current.IdCentroServicio))
            {
                guiaReclameOficina.Respuesta = OUEnumValidacionDescargue.Notificacion;
                guiaReclameOficina.Mensaje = "La guía no se encuentra asignada para ingreso a reclame en oficina";
                return guiaReclameOficina;
            }



            ADGuia Guia = fachadaMensajeria.ObtenerGuiaXNumeroGuia(guiaReclameOficina.MovimientoInventario.NumeroGuia);
            guiaReclameOficina.DocumentoDestinatario = Guia.Destinatario.Identificacion;
            guiaReclameOficina.NombreDestinatario = Guia.Destinatario.NombreYApellidos;
            guiaReclameOficina.EsAlCobro = Guia.EsAlCobro;
            guiaReclameOficina.Peso = Guia.Peso;
            guiaReclameOficina.DiceContener = Guia.DiceContener;

            //ingresar a bodega        
            guiaReclameOficina.MovimientoInventario.IdCentroServicioOrigen = ControllerContext.Current.IdCentroServicio;
            guiaReclameOficina.MovimientoInventario.NumeroGuia = Guia.NumeroGuia;
            guiaReclameOficina.MovimientoInventario.FechaGrabacion = DateTime.Now;
            guiaReclameOficina.MovimientoInventario.FechaEstimadaIngreso = DateTime.Now;
            guiaReclameOficina.MovimientoInventario.CreadoPor = ControllerContext.Current.Usuario;

            using (TransactionScope transaccion = new TransactionScope())
            {

                guiaReclameOficina.IdReclameEnOficina = fachadaCentroServicios.AdicionarMovimientoInventario(guiaReclameOficina.MovimientoInventario);
                //ingresa reclame en oficina
                LIRepositorioPruebasEntrega.Instancia.AdicionarReclameEnOficina(guiaReclameOficina);


                //segunda transicion de estado pasa a ingreso reclame en oficina
                estadoGuia = new ADTrazaGuia
                {
                    Ciudad = guiaReclameOficina.MovimientoInventario.Bodega.CiudadUbicacion.Nombre,
                    IdCiudad = guiaReclameOficina.MovimientoInventario.Bodega.CiudadUbicacion.IdLocalidad,
                    IdAdmision = Guia.IdAdmision,
                    IdEstadoGuia = (short)ADEnumEstadoGuia.TransitoUrbano,
                    IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.ReclameEnOficina,
                    Modulo = COConstantesModulos.PRUEBAS_DE_ENTREGA,
                    NumeroGuia = guiaReclameOficina.MovimientoInventario.NumeroGuia,
                    Observaciones = string.Empty,
                    FechaGrabacion = DateTime.Now
                };
                estadoGuia.IdTrazaGuia = EstadosGuia.InsertaEstadoGuia(estadoGuia);
                guiaReclameOficina.Respuesta = OUEnumValidacionDescargue.Exitosa;
                guiaReclameOficina.Mensaje = "Se ingreso la guía satisfactoriamente";

                //se adiciona los 30 dias para reclame oficina
                EGTipoNovedadGuia.CambiarFechaEntregaCalendario(new Servicios.ContratoDatos.Comun.COCambioFechaEntregaDC
                {
                    Guia = new ADGuia { NumeroGuia = guiaReclameOficina.MovimientoInventario.NumeroGuia },
                    TiempoAfectacion = 30
                });

                if (!string.IsNullOrEmpty(Guia.Destinatario.Telefono))
                {
                    PUCentroServiciosDC prodestino = fachadaCentroServicios.ObtenerCentroServicio(Guia.IdCentroServicioDestino);
                    string mensaje = "INTERRAPIDISIMO RECLAME ENVIO " + Guia.NumeroGuia.ToString() + " EN " + prodestino.Direccion.ToUpper() + "-" + prodestino.CiudadUbicacion.Nombre.ToUpper();
                    mensaje = mensaje.Replace("#", " ");

                    //Envio de mensaje al destinatario para avisar el ingreso del envio a el pro
                    System.Threading.Tasks.Task.Factory.StartNew(() =>
                    {
                        bool rtaParametroEsPruebas =  true;
                        try
                        {
                            bool.TryParse(PAAdministrador.Instancia.ConsultarParametrosFramework("EsAmbientePruebas"), out rtaParametroEsPruebas);
                                

                            if (!rtaParametroEsPruebas)
                            {
                                MensajesTexto.Instancia.EnviarMensajeTexto(Guia.Destinatario.Telefono, mensaje);
                            }
                        }
                        catch (Exception exc)
                        {
                            AuditoriaTrace.EscribirAuditoria(ETipoAuditoria.Error, exc.ToString(), COConstantesModulos.MODULO_OPERACION_NACIONAL);
                        }

                    }, System.Threading.Tasks.TaskCreationOptions.PreferFairness);
                }


                transaccion.Complete();
                return guiaReclameOficina;
            }

        }

        /// <summary>
        /// adiciona reclame en oficina
        /// </summary>                
        public LIReclameEnOficinaDC DevolucionReclameOficina(LIReclameEnOficinaDC guiaReclameOficina)
        {

            PUMovimientoInventario tipoMovimientoActual = LIRepositorioPruebasEntrega.Instancia.ConsultaUltimoMovimientoBodegaGuia(guiaReclameOficina.MovimientoInventario.NumeroGuia);

            if (!(tipoMovimientoActual.TipoMovimiento == PUEnumTipoMovimientoInventario.Ingreso
                && tipoMovimientoActual.Bodega.IdCentroServicio == ControllerContext.Current.IdCentroServicio))
            {
                guiaReclameOficina.Respuesta = OUEnumValidacionDescargue.Error;
                guiaReclameOficina.Mensaje = "La Guía no se encuentra Ingresada al PRO";
                return guiaReclameOficina;
            }

            ADGuia Guia = new ADGuia();
            Guia = fachadaMensajeria.ObtenerGuiaXNumeroGuia(guiaReclameOficina.MovimientoInventario.NumeroGuia);
            if (Guia.Entregada)
            {
                guiaReclameOficina.Respuesta = OUEnumValidacionDescargue.Error;
                guiaReclameOficina.Mensaje = string.Format("La Guia {0} ya se encuentra Entregada", guiaReclameOficina.MovimientoInventario.NumeroGuia);
                return guiaReclameOficina;
            }


            using (TransactionScope transaccion = new TransactionScope())
            {

                //cambio estado entregado por reclame en oficina
                ADTrazaGuia estadoGuia;
                estadoGuia = new ADTrazaGuia
                {
                    Ciudad = guiaReclameOficina.MovimientoInventario.Bodega.CiudadUbicacion.Nombre,
                    IdCiudad = guiaReclameOficina.MovimientoInventario.Bodega.CiudadUbicacion.IdLocalidad,
                    IdAdmision = Guia.IdAdmision,
                    IdEstadoGuia = (short)EstadosGuia.ObtenerUltimoEstadoxNumero(Guia.NumeroGuia),
                    IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.TransitoUrbano,
                    Modulo = COConstantesModulos.PRUEBAS_DE_ENTREGA,
                    NumeroGuia = guiaReclameOficina.MovimientoInventario.NumeroGuia,
                    Observaciones = "Guía devuelta desde reclame en oficina",
                    FechaGrabacion = DateTime.Now
                };
                estadoGuia.IdTrazaGuia = EstadosGuia.ValidarInsertarEstadoGuia(estadoGuia);


                //salida de la bodega
                guiaReclameOficina.MovimientoInventario.IdCentroServicioOrigen = ControllerContext.Current.IdCentroServicio;
                guiaReclameOficina.MovimientoInventario.TipoMovimiento = PUEnumTipoMovimientoInventario.Salida;
                guiaReclameOficina.MovimientoInventario.FechaGrabacion = DateTime.Now;
                guiaReclameOficina.MovimientoInventario.FechaEstimadaIngreso = DateTime.Now;
                guiaReclameOficina.MovimientoInventario.CreadoPor = ControllerContext.Current.Usuario;
                fachadaCentroServicios.AdicionarMovimientoInventario(guiaReclameOficina.MovimientoInventario);

                guiaReclameOficina.Respuesta = OUEnumValidacionDescargue.Exitosa;

                transaccion.Complete();
                return guiaReclameOficina;
            }


        }


        /// <summary>
        /// adiciona reclame en oficina
        /// </summary>                
        public LIReclameEnOficinaDC EntregaReclameOficina(LIReclameEnOficinaDC guiaReclameOficina)
        {

            ADGuia Guia = new ADGuia();
            Guia = fachadaMensajeria.ObtenerGuiaXNumeroGuia(guiaReclameOficina.MovimientoInventario.NumeroGuia);
            if (Guia.Entregada)
            {
                guiaReclameOficina.Respuesta = OUEnumValidacionDescargue.Error;
                guiaReclameOficina.Mensaje = string.Format("La Guia {0} ya se encuentra Entregada", guiaReclameOficina.MovimientoInventario.NumeroGuia);
                return guiaReclameOficina;
            }

            string rutaImagenes = Framework.Servidor.ParametrosFW.PAParametros.Instancia.ConsultarParametrosFramework("FolderImgEviReclOf");
            using (TransactionScope transaccion = new TransactionScope())
            {

                PUMovimientoInventario MovimientoActual = LIRepositorioPruebasEntrega.Instancia.ConsultaUltimoMovimientoBodegaGuia(guiaReclameOficina.MovimientoInventario.NumeroGuia);

                if (!(MovimientoActual.TipoMovimiento == PUEnumTipoMovimientoInventario.Ingreso
                    && MovimientoActual.Bodega.IdCentroServicio == ControllerContext.Current.IdCentroServicio))
                {
                    guiaReclameOficina.Respuesta = OUEnumValidacionDescargue.Error;
                    guiaReclameOficina.Mensaje = "La guia no se encuentra ingresada para reclame en oficina";
                    return guiaReclameOficina;

                }


                guiaReclameOficina.ValorTotal = Guia.ValorTotal;
                guiaReclameOficina.DocumentoDestinatario = Guia.Destinatario.Identificacion;
                guiaReclameOficina.NombreDestinatario = Guia.Destinatario.NombreYApellidos;
                guiaReclameOficina.EsAlCobro = Guia.EsAlCobro;
                guiaReclameOficina.Peso = Guia.Peso;
                guiaReclameOficina.DiceContener = Guia.DiceContener;

                //salida a bodega
                guiaReclameOficina.MovimientoInventario.IdCentroServicioOrigen = ControllerContext.Current.IdCentroServicio;
                guiaReclameOficina.MovimientoInventario.TipoMovimiento = PUEnumTipoMovimientoInventario.Salida;
                guiaReclameOficina.MovimientoInventario.FechaGrabacion = DateTime.Now;
                guiaReclameOficina.MovimientoInventario.FechaEstimadaIngreso = DateTime.Now;
                guiaReclameOficina.MovimientoInventario.CreadoPor = ControllerContext.Current.Usuario;
                fachadaCentroServicios.AdicionarMovimientoInventario(guiaReclameOficina.MovimientoInventario);


                //cambio estado entregado por reclame en oficina
                ADTrazaGuia estadoGuia;
                estadoGuia = new ADTrazaGuia
                {
                    Ciudad = guiaReclameOficina.MovimientoInventario.Bodega.CiudadUbicacion.Nombre,
                    IdCiudad = guiaReclameOficina.MovimientoInventario.Bodega.CiudadUbicacion.IdLocalidad,
                    IdAdmision = Guia.IdAdmision,
                    IdEstadoGuia = (short)EstadosGuia.ObtenerUltimoEstadoxNumero(Guia.NumeroGuia),
                    IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.Entregada,
                    Modulo = COConstantesModulos.PRUEBAS_DE_ENTREGA,
                    NumeroGuia = guiaReclameOficina.MovimientoInventario.NumeroGuia,
                    Observaciones = "Guía entregada desde reclame en oficina",
                    FechaGrabacion = DateTime.Now
                };
                estadoGuia.IdTrazaGuia = EstadosGuia.ValidarInsertarEstadoGuia(estadoGuia);
                if (estadoGuia.IdTrazaGuia == 0)
                {
                    guiaReclameOficina.Respuesta = OUEnumValidacionDescargue.Error;
                    guiaReclameOficina.Mensaje = "Error al Intentar Grabar en EstadoGuiaTraza";
                    return guiaReclameOficina;
                }


                if (Guia.EsAlCobro && Guia.EstaPagada == false)
                {
                    GuardarTransaccion(Guia);
                    fachadaMensajeria.ActualizarPagadoGuia(Guia.IdAdmision, true);
                }
                GuardarComisionEntrega(Guia, CMEnumTipoComision.Entregar);



                
                string carpetaDestino = Path.Combine(rutaImagenes + "\\" + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year);
                if (!Directory.Exists(carpetaDestino))
                {
                    Directory.CreateDirectory(carpetaDestino);
                }

                byte[] bytebuffer = Convert.FromBase64String(guiaReclameOficina.ImagenEvidenciaEntrega);
                MemoryStream memoryStream = new MemoryStream(bytebuffer);
                var image = Image.FromStream(memoryStream);
                ImageCodecInfo jpgEncoder = UtilidadesFW.GetEncoder(ImageFormat.Jpeg);
                string ruta = carpetaDestino + "\\" + guiaReclameOficina.NumeroGuia + "-" + DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") + ".jpg";

                Encoder myEncoder = Encoder.Quality;
                EncoderParameters myEncoderParameters = new EncoderParameters(1);
                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 180L);
                myEncoderParameters.Param[0] = myEncoderParameter;
                image.Save(ruta, jpgEncoder, myEncoderParameters);

                LIRepositorioPruebasEntrega.Instancia.InsertarEvidenciaEntregaReclameEnOficina(guiaReclameOficina, ruta);



                transaccion.Complete();
                guiaReclameOficina.Respuesta = OUEnumValidacionDescargue.Exitosa;
                return guiaReclameOficina;

            }

        }


        #region Cajas y comisiones

        /// <summary>
        /// Método para calcular la comision de entrega de un envio
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="tipoComision"></param>
        private void GuardarComisionEntrega(ADGuia guia, CMEnumTipoComision tipoComision)
        {
            if (guia.ValorServicio > 0)
            {
                CMComisionXVentaCalculadaDC comision = fachadaComisiones.CalcularComisionesxVentas(
             new Servidor.Servicios.ContratoDatos.Comisiones.CMConsultaComisionVenta()
             {
                 IdCentroServicios = guia.IdCentroServicioDestino,
                 IdServicio = guia.IdServicio,
                 TipoComision = tipoComision,
                 ValorBaseComision = guia.ValorServicio,
                 NumeroOperacion = guia.NumeroGuia,
             });
                fachadaComisiones.GuardarComision(comision);
            }
        }


        // <summary>
        /// Método para afectar la cuenta del mensajero
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="conceptoCaja"></param>
        /// <param name="esIngreso"></param>
        public void GuardarTransaccion(ADGuia guia)
        {
            ICAFachadaCajas fachadaCajas = COFabricaDominio.Instancia.CrearInstancia<ICAFachadaCajas>();
            CMComisionXVentaCalculadaDC comision = CalcularComisionesPorEntrega(guia);
            CARegistroTransacCajaDC registro = new Servidor.Servicios.ContratoDatos.Cajas.CARegistroTransacCajaDC()
            {
                InfoAperturaCaja = new CAAperturaCajaDC()
                {
                    IdCodigoUsuario = ControllerContext.Current.CodigoUsuario,
                },
                TipoDatosAdicionales = CAEnumTipoDatosAdicionales.PEA,
                IdCentroResponsable = comision.IdCentroServicioResponsable,
                IdCentroServiciosVenta = comision.IdCentroServicioVenta,
                NombreCentroResponsable = comision.NombreCentroServicioResponsable,
                NombreCentroServiciosVenta = comision.NombreCentroServicioVenta,
                RegistrosTransacDetallesCaja = new List<Servidor.Servicios.ContratoDatos.Cajas.CARegistroTransacCajaDetalleDC>()
        {
          new CO.Servidor.Servicios.ContratoDatos.Cajas.CARegistroTransacCajaDetalleDC() {
             Cantidad = 1,
             ConceptoCaja = new CAConceptoCajaDC() { IdConceptoCaja =  (int)CAEnumConceptosCaja.PAGO_DE_ENVIO_AL_COBRO , EsIngreso = true},
             ConceptoEsIngreso = true,
             EstadoFacturacion = CAEnumEstadoFacturacion.FAC,
             FechaFacturacion = DateTime.Now,
             Numero = guia.NumeroGuia,
             NumeroFactura = guia.NumeroGuia.ToString(),
             ValorDeclarado = guia.ValorDeclarado,
             ValoresAdicionales = guia.ValorAdicionales,
             ValorImpuestos = guia.ValorTotalImpuestos,
             ValorPrimaSeguros = guia.ValorPrimaSeguro, ValorRetenciones = guia.ValorTotalRetenciones,
             ValorServicio = guia.ValorServicio,
             Observacion = "Recaudo de Dinero por guia al cobro",
             ValorTercero = 0
          }
        },
                ValorTotal = guia.ValorTotal,
                TotalImpuestos = guia.ValorTotalImpuestos,
                TotalRetenciones = guia.ValorTotalRetenciones,
                Usuario = ControllerContext.Current.Usuario,
                RegistroVentaFormaPago = guia.FormasPago.ConvertAll(formaPago => new CARegistroVentaFormaPagoDC
                {
                    IdFormaPago = TAConstantesServicios.ID_FORMA_PAGO_CONTADO,
                    Descripcion = TAConstantesServicios.DESCRIPCION_FORMA_PAGO_CONTADO,
                    Valor = guia.ValorTotal,
                    NumeroAsociado = string.Empty,
                    Campo01 = string.Empty,
                    Campo02 = string.Empty,
                }),
            };

            ADRecaudarDineroAlCobroDC guiaAlCobro = new ADRecaudarDineroAlCobroDC();
            guiaAlCobro.MovimientoCaja = registro;
            guiaAlCobro.InfoGuiaRecaudada = guia;
            guiaAlCobro.IdCodigoUsuario = ControllerContext.Current.CodigoUsuario;

            fachadaMensajeria.ActualizarGuiaAlCobro(guiaAlCobro);

        }


        private CMComisionXVentaCalculadaDC CalcularComisionesPorEntrega(ADGuia guia)
        {
            CMComisionXVentaCalculadaDC comision = fachadaComisiones.CalcularComisionesxVentas(
              new Servidor.Servicios.ContratoDatos.Comisiones.CMConsultaComisionVenta()
              {
                  IdCentroServicios = (int)guia.IdCentroServicioDestino,
                  IdServicio = guia.IdServicio,
                  TipoComision = Servidor.Servicios.ContratoDatos.Comisiones.CMEnumTipoComision.Entregar,
                  ValorBaseComision = guia.ValorServicio,
                  NumeroOperacion = guia.NumeroGuia,
              });

            return comision;
        }

    }
    #endregion


}
