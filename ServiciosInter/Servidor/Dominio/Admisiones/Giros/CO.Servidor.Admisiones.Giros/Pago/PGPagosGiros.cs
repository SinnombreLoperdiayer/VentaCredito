using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Transactions;
using CO.Servidor.Admisiones.Giros.Comun;
using CO.Servidor.Admisiones.Giros.Datos;
using CO.Servidor.Dominio.Comun.Cajas;
using CO.Servidor.Dominio.Comun.Clientes;
using CO.Servidor.Dominio.Comun.Comisiones;
using CO.Servidor.Dominio.Comun.GestionGiros;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.Suministros;
using CO.Servidor.Dominio.Comun.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros.Pagos;
using CO.Servidor.Servicios.ContratoDatos.Cajas;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Comisiones;
using CO.Servidor.Servicios.ContratoDatos.Solicitudes;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System.IO;
using System.Configuration;
using CO.Servidor.Dominio.Comun.Util;
using RestSharp;
using CO.Servidor.Servicios.Contratos;
using CO.Servidor.Dominio.Comun.Admisiones;

namespace CO.Servidor.Admisiones.Giros.Pago
{
    public class PGPagosGiros : ControllerBase
    {
        #region CrearInstancia

        private static readonly PGPagosGiros instancia = (PGPagosGiros)FabricaInterceptores.GetProxy(new PGPagosGiros(), COConstantesModulos.GIROS);

        /// <summary>
        /// Retorna una instancia de centro Servicios
        /// /// </summary>
        public static PGPagosGiros Instancia
        {
            get { return PGPagosGiros.instancia; }
        }

        #endregion CrearInstancia


        /// <summary>
        /// Path almacena imagenes scanneadas
        /// </summary>
        private string filePath = ConfigurationManager.AppSettings["Controller.RutaDescargaArchivos"];


        #region Metodos

        #region Consultas de Giros a Pagar

        /// <summary>
        /// Consultar la cantidad de pagos y la sumatoria total de los mismos
        /// por agencia
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public PGTotalPagosDC ConsultarPagosAgencia(long idCentroServicio)
        {
            return PGRepositorio.Instancia.ConsultarPagosAgencia(idCentroServicio);
        }

        /// <summary>
        /// Consultar el giro por numero de giro
        /// </summary>
        /// <param name="idGiro"></param>
        public GIAdmisionGirosDC ConsultarGiroXNumGiro(long idGiro)
        {
            GIAdmisionGirosDC giro = PGRepositorio.Instancia.ConsultarGiroXNumGiro(idGiro);

            // Se comentario por decision de no traer la cedula del cliente
            //if (giro != null)
            //  giro.DocumentoDestinatario = COFabricaDominio.Instancia.CrearInstancia<ICLFachadaClientes>().ConsultarDocumentoCliente(giro.GirosPeatonPeaton.ClienteDestinatario.TipoId, giro.GirosPeatonPeaton.ClienteDestinatario.Identificacion);

            return giro;
        }

        /// <summary>
        /// Consultar el giro por numero de giro y la ciudad de destino
        /// </summary>
        /// <param name="idGiro"></param>
        public GIAdmisionGirosDC ConsultarGiroNumGiroCiudadDestino(long idGiro, string idCiudadDestino)
        {
            return PGRepositorio.Instancia.ConsultarGiroNumGiroCiudadDestino(idGiro, idCiudadDestino);
        }

        /// <summary>
        /// Consultar el giro por numero de giro
        /// y centro Servicio destino
        /// </summary>
        /// <param name="idGiro"></param>
        public GIAdmisionGirosDC ConsultarGiroXNumGiroCentroServicio(long idGiro, long idCentroSvc)
        {

            return PGRepositorio.Instancia.ConsultarGiroXNumGiroCentroServicio(idGiro, idCentroSvc);
        }

        /// <summary>
        /// Consultar el giro por tipo y numero de identificacion del destinatario
        /// </summary>
        /// <param name="tipoId"></param>
        /// <param name="identificacion"></param>
        public List<GIAdmisionGirosDC> ConsultarGiroXIdentificacion(string tipoId, string identificacion)
        {
            return PGRepositorio.Instancia.ConsultarGiroXIdentificacion(tipoId, identificacion);
        }

        /// <summary>
        /// Consultar el giro por tipo y numero de identificacion del destinatario
        /// y por la ciudad de donde se realiza la consulta
        /// </summary>
        /// <param name="tipoId"></param>
        /// <param name="identificacion"></param>
        /// <param name="idCiudadConsulta"></param>
        /// <returns>Lista de Giros por la ciudad consultada</returns>
        public List<GIAdmisionGirosDC> ConsultarGiroXIdentificacionCiudad(string tipoId, string identificacion, string idCiudadConsulta)
        {
            return PGRepositorio.Instancia.ConsultarGiroXIdentificacionCiudad(tipoId, identificacion, idCiudadConsulta);
        }

        /// <summary>
        /// Consultar el giro por tipo y numero de identificacion del destinatario
        /// y por la ciudad de donde se realiza la consulta
        /// </summary>
        /// <param name="tipoId"></param>
        /// <param name="identificacion"></param>
        /// <param name="idCiudadConsulta"></param>
        /// <returns>Lista de Giros por la ciudad consultada</returns>
        public List<GIAdmisionGirosDC> ConsultarGiroXIdentificacionCentroServiciosDestino(string tipoId, string identificacion, long idCentroServicio)
        {
            return PGRepositorio.Instancia.ConsultarGiroXIdentificacionCentroServiciosDestino(tipoId, identificacion, idCentroServicio);
        }

        /// <summary>
        /// Consulta la informacion de un pago
        /// </summary>
        /// <param name="idAdmisionGiro"></param>
        public PGPagosGirosDC ConsultarInformacionPago(long idAdmisionGiro)
        {
            PGPagosGirosDC pago = PGRepositorio.Instancia.ConsultarInformacionPago(idAdmisionGiro);
            if (pago != null)
            {
                pago.DocumentoDestinatario = ObtenerImagenDocumentoDestinatarioPagoGiro(GIEnumTipoDocumentoPagoGiro.DOCUMENTOIDENTIDAD, idAdmisionGiro);
            }

            return pago;
        }

        /// <summary>
        /// obtiene el la imagen del documento adjunto al pago de
        /// un giro en el formato de Comprobante de Pago
        /// </summary>
        /// <param name="tipoDocPago">Enumeracion del tipo Doc</param>
        /// <param name="idAdmisionGiro">Id de Admision del Giro</param>
        /// <returns>Imagen</returns>
        public string ObtenerImagenDocumentoDestinatarioPagoGiro(GIEnumTipoDocumentoPagoGiro tipoDocPago, long idAdmisionGiro)
        {
            return PGRepositorio.Instancia.ObtenerImagenDocumentoDestinatarioPagoGiro(tipoDocPago, idAdmisionGiro);
        }

        /// <summary>
        /// Obtiene el identificador de pagos giro
        /// </summary>
        /// <param name="idAdmisionGiro">Identificador admisión</param>
        /// <returns>Identificador</returns>
        public long ObtenerIdentificadorPagosGiro(long idAdmisionGiro)
        {
            return PGRepositorio.Instancia.ObtenerIdentificadorPagosGiro(idAdmisionGiro);
        }

        /// <summary>
        /// Obtiene la informacion de un giro por el numero de comprobante de pago
        /// </summary>
        /// <param name="idComprobantePago"></param>
        /// <returns></returns>
        public PGPagosGirosDC ObtenerGiroPorComprobantePago(long idComprobantePago)
        {
            return PGRepositorio.Instancia.ObtenerGiroPorComprobantePago(idComprobantePago);
        }

        #endregion Consultas de Giros a Pagar

        #region Realizar el proceso de pago

        /// <summary>
        /// Valida que el giro este disponible para pagar, no tenga solicitudes
        /// </summary>
        public void ValidacionPago(long idAdmisiongiro)
        {
            PGRepositorio.Instancia.ValidacionPago(idAdmisiongiro);
        }

        /// <summary>
        /// Consulta si el pago se realizo exitosamente
        /// </summary>
        /// <param name="idAdmisiongiro"></param>
        public void ConsultarPago(long idAdmisiongiro)
        {
            PGRepositorio.Instancia.ConsultarPago(idAdmisiongiro);
        }

        /// <summary>
        /// Realiza el pago del giro
        /// </summary>
        public PGComprobantePagoDC PagarGiro(PGPagosGirosDC pagosGiros)
        {
            long numeroPago;
            PGComprobantePagoDC comprobante;

            using (TransactionScope scope = new TransactionScope())
            {
                ISUFachadaSuministros fachadaSuministros = COFabricaDominio.Instancia.CrearInstancia<ISUFachadaSuministros>();
                numeroPago = fachadaSuministros.ObtenerNumeroPrefijoValor(SUEnumSuministro.COMPROBANTE_PAGO_GIRO_POSTAL_AUTOMATICO).ValorActual;
                scope.Complete();
            }

            pagosGiros = PGRepositorio.Instancia.ObtenerCorreos(pagosGiros);

            using (TransactionScope scope = new TransactionScope())
            {
                comprobante = PGRepositorio.Instancia.PagarGiro(pagosGiros, numeroPago);
                comprobante.ObligaDeclaracionVoluntariaFondos = false;

                if (pagosGiros.EstadoGiro == GIEnumEstadosGiro.DEV.ToString())
                {
                    IGIFachadaGestionGiros fachadaGestionGiros = COFabricaDominio.Instancia.CrearInstancia<IGIFachadaGestionGiros>();
                    comprobante.PagoPorDevolucion = fachadaGestionGiros.DevolverGiroCaja(pagosGiros.IdAdmisionGiro, pagosGiros.IdCaja, pagosGiros.IdCentroServiciosPagador);
                }
                else
                {
                    EnviarTransaccionCaja(pagosGiros, comprobante);
                }
                if (PGRepositorio.Instancia.ValidarDeclaracionFondos(pagosGiros.ValorPagado))
                {
                    comprobante.ObligaDeclaracionVoluntariaFondos = true;
                }


                if (pagosGiros.ArchivoCedulaClientePago != null)
                {
                    pagosGiros.ArchivoCedulaClientePago = GuardarImagenCarpeta(pagosGiros.ArchivoCedulaClientePago);

                    //Consume el servicio de Tercero y envía la ruta dónde queda almacenado el documento de pago para el Giro
                    EnviarRutaCedulaClientePago(pagosGiros);


                    PGRepositorio.Instancia.AlmacenarArchivoPagoGiroPago(pagosGiros, pagosGiros.ArchivoCedulaClientePago, GIEnumTipoDocumentoPagoGiro.DOCUMENTOIDENTIDAD);

                }

                if (pagosGiros.PagoAutorizadoPeaton)
                {
                    pagosGiros.ArchivoAutorizacionPago = GuardarImagenCarpeta(pagosGiros.ArchivoAutorizacionPago);
                    PGRepositorio.Instancia.AlmacenarArchivoPagoGiroPago(pagosGiros, pagosGiros.ArchivoAutorizacionPago, GIEnumTipoDocumentoPagoGiro.AUTORIZACIONPAGO);

                }

                if (pagosGiros.PagoAutorizadoEmpresarial)
                {
                    pagosGiros.ArchivoAutorizacionPago = GuardarImagenCarpeta(pagosGiros.ArchivoAutorizacionPago);
                    PGRepositorio.Instancia.AlmacenarArchivoPagoGiroPago(pagosGiros, pagosGiros.ArchivoAutorizacionPago, GIEnumTipoDocumentoPagoGiro.AUTORIZACIONPAGO);
                    pagosGiros.ArchivoCertificadoEmpresa = GuardarImagenCarpeta(pagosGiros.ArchivoCertificadoEmpresa);
                    PGRepositorio.Instancia.AlmacenarArchivoPagoGiroPago(pagosGiros, pagosGiros.ArchivoCertificadoEmpresa, GIEnumTipoDocumentoPagoGiro.CERTIFICADOEMPRESARIAL);


                }

                scope.Complete();
            }

            /*Envia mensaje remitente por pago giro*/

            string tipoNotificacion = "EntregaGiro";
            var tipo = (GIEnumMensajeTexto)Enum.Parse(typeof(GIEnumMensajeTexto), tipoNotificacion.ToString());
            if (!string.IsNullOrEmpty(pagosGiros.TelefonoRemitente))
            {
                GIMensajesTexto.Instancia.EnviarMensajeTexto(tipo, pagosGiros.TelefonoRemitente, pagosGiros.IdAdmisionGiro);
            }
            /**/

            ///Envia correo al remitente avisando pago del giro
            if (pagosGiros.EnviaCorreo && !string.IsNullOrEmpty(pagosGiros.CorreoRemitente))
                EnviarCorreo(pagosGiros);

            return comprobante;
        }


        /// <summary>
        /// Método para guardar las imagenes en la carpeta compartida
        /// </summary>
        /// <param name="rutaArchivo"></param>
        public string GuardarImagenCarpeta(string rutaArchivo)
        {
            string rutaImagenes = Framework.Servidor.ParametrosFW.PAParametros.Instancia.ConsultarParametrosFramework("FolderImagenGiros");
            string carpetaDestino = Path.Combine(rutaImagenes + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year);
            string rutaDestino = Path.Combine(carpetaDestino, rutaArchivo);
            if (!Directory.Exists(carpetaDestino))
            {
                Directory.CreateDirectory(carpetaDestino);
            }

            lock (this)
            {
                if (!File.Exists(rutaDestino))
                    File.Copy(Path.Combine(this.filePath, COConstantesModulos.GIROS, rutaArchivo), rutaDestino);
            }
            return rutaDestino;
        }

        /// <summary>
        /// envia informacion a los modulos de comision y cajas
        /// </summary>
        /// <param name="pago"></param>
        public void EnviarTransaccionCaja(PGPagosGirosDC pago, PGComprobantePagoDC comprobante, bool esDevolGiro = false, bool esDevolFlete = false)
        {
            ICAFachadaCajas fachadaCajas = COFabricaDominio.Instancia.CrearInstancia<ICAFachadaCajas>();
            ICMFachadaComisiones fachadaComisiones = COFabricaDominio.Instancia.CrearInstancia<ICMFachadaComisiones>();
            int idConceptoCaja = 0;
            decimal valorPagado = 0;
            if (!esDevolFlete && !esDevolGiro)
            {
                idConceptoCaja = PGRepositorio.Instancia.ObtenerConceptoPagos();

                valorPagado = pago.ValorPagado;
            }
            else
            {
                if (esDevolFlete)
                {
                    idConceptoCaja = (int)CAEnumConceptosCaja.DEVOLUCION_PORTE_DE_GIRO;
                    valorPagado = pago.ValorServicio;
                }

                if (esDevolGiro)
                {
                    idConceptoCaja = (int)CAEnumConceptosCaja.DEVOLUCION_GIRO_POSTAL;
                    valorPagado = pago.ValorPagado;
                }
            }

            CMComisionXVentaCalculadaDC comision = fachadaComisiones.CalcularComisionesxVentas(
                  new Servidor.Servicios.ContratoDatos.Comisiones.CMConsultaComisionVenta()
                  {
                      IdCentroServicios = (int)pago.IdCentroServiciosPagador,
                      IdServicio = GIConstantesAdmisionesGiros.SERVICIO_GIRO,
                      TipoComision = Servidor.Servicios.ContratoDatos.Comisiones.CMEnumTipoComision.Pagar,
                      ValorBaseComision = valorPagado,
                      NumeroOperacion = comprobante.IdComprobantePago,
                  });
            ///Cuando es devolucion y se esta afectando solo el flete no se tiene encuenta la comision
            if (!esDevolFlete)
            {
                fachadaComisiones.GuardarComision(comision);
            }

            // Se adiciona el movimiento de caja
            fachadaCajas.AdicionarMovimientoCaja(new Servidor.Servicios.ContratoDatos.Cajas.CARegistroTransacCajaDC()
            {
                InfoAperturaCaja = new CAAperturaCajaDC()
                {
                    IdCaja = pago.IdCaja,
                    IdCodigoUsuario = pago.IdCodigoUsuario
                },
                IdCentroResponsable = comision.IdCentroServicioResponsable,
                IdCentroServiciosVenta = comision.IdCentroServicioVenta,
                NombreCentroResponsable = comision.NombreCentroServicioResponsable,
                NombreCentroServiciosVenta = comision.NombreCentroServicioVenta,
                ValorTotal = valorPagado,
                TotalImpuestos = 0,
                TotalRetenciones = 0,

                RegistrosTransacDetallesCaja = new List<Servidor.Servicios.ContratoDatos.Cajas.CARegistroTransacCajaDetalleDC>()
          {
             new CARegistroTransacCajaDetalleDC()
            {
              ConceptoCaja= new CAConceptoCajaDC()
              {
                IdConceptoCaja= idConceptoCaja,
                EsIngreso=false
              },
              Cantidad=1,
              EstadoFacturacion=CAEnumEstadoFacturacion.FAC,
              FechaFacturacion=DateTime.Now,
              ValorTercero=valorPagado,
              ValorServicio=0,
              ValorImpuestos=0,
              ValorPrimaSeguros=0,
              ValorRetenciones=0,

              //Bug 2288 se ajusta que para el pago del giro se coloca en "Numero" el numero del comprobante
              //antes estaba el numero del giro Rafram 20/12/2012
              Numero= comprobante.IdComprobantePago,
              NumeroFactura= pago.IdGiro.ToString(),
              ValorDeclarado=0,
              ValoresAdicionales=0,
              Observacion=pago.Observaciones,
              ConceptoEsIngreso=false,
              NumeroComprobante = comprobante.IdComprobantePago.ToString()
            }
          },
                Usuario = Framework.Servidor.Excepciones.ControllerContext.Current.Usuario,
                TipoDatosAdicionales = CAEnumTipoDatosAdicionales.PEA,
                RegistroVentaFormaPago = new List<CARegistroVentaFormaPagoDC>()
          {
            new CARegistroVentaFormaPagoDC()
            {
              IdFormaPago=TAConstantesServicios.ID_FORMA_PAGO_CONTADO,
              Descripcion=TAConstantesServicios.DESCRIPCION_FORMA_PAGO_CONTADO,
              Valor=valorPagado
            }
          }
            });
        }


        #endregion Realizar el proceso de pago

        #region Crear Solicitudes

        /// <summary>
        /// Crear solicitud de giros
        /// </summary>
        public GISolicitudGiroDC CrearSolicitud(GIEnumMotivoSolicitudDC motivo, GISolicitdPagoDC solicitud)
        {
            PGRepositorio.Instancia.ValidacionPago(solicitud.IdAdminGiro);
            return COFabricaDominio.Instancia.CrearInstancia<IGIFachadaGestionGiros>().AdicionarNvaSolicitud(motivo, solicitud);
        }

        #endregion Crear Solicitudes

        #region Actualizar Datos Giro

        /// <summary>
        /// Actualiza la Informacion del Giro
        /// por una Solicitud
        /// </summary>
        /// <param name="giroUpdate">info del giro a Actualizar</param>
        public void ActualizarInfoGiro(GIAdmisionGirosDC giroUpdate)
        {
            GIRepositorio.Instancia.ActualizarInfoGiro(giroUpdate);
        }

        #endregion Actualizar Datos Giro

        #region Notificacion correo

        /// <summary>
        /// Método para enviar un correo al remitente notificando el cobro del giro
        /// </summary>
        private void EnviarCorreo(PGPagosGirosDC pagoGiro)
        {
            InformacionAlerta informacionAlerta = PAParametros.Instancia.ConsultarInformacionAlerta(PAConstantesParametros.ALERTA_PAGO_GIRO);
            PAEnvioCorreoAsyn.Instancia.EnviarCorreoAsyn(pagoGiro.CorreoRemitente, informacionAlerta.Asunto, string.Format(informacionAlerta.Mensaje, pagoGiro.ClienteCobrador.Nombre + " " + pagoGiro.ClienteCobrador.Apellido1));
        }

        #endregion Notificacion correo

        #region Tercero

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="servicio"></param>
        /// <param name="metodo"></param>
        /// <param name="parametros"></param>
        /// <param name="informacion"></param>
        /// <param name="header"></param>
        public void EnviarRutaCedulaClientePago(PGPagosGirosDC pagosGiros)
        {

            IADFachadaAdmisionesGiros servGiros = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesGiros>();
            var urlTercero = servGiros.ConsultarParametrosGiros("ServicioRestTercero");

            var Url = urlTercero.ValorParametro + "/" + pagosGiros.ClienteCobrador.Identificacion + "/Documentos/";
            var servicio = string.Empty;
            Method metodo = Method.POST;
            dynamic parametros = new { RutaDocumento = pagosGiros.ArchivoCedulaClientePago, CreadoPor = 1, TipoDocumento = pagosGiros.ClienteCobrador.TipoIdentificacion };
            dynamic informacion = new { };
            Dictionary<string, Object> header = new Dictionary<string, object>();
            header.Add("IdDocumento", 1);
            header.Add("RutaDocumento", pagosGiros.ArchivoCedulaClientePago);
            header.Add("FechaDigitalizacion", DateTime.Now);
            header.Add("FechaActualizacion", DateTime.Now);
            header.Add("CreadoPor", 1);
            header.Add("IdTercero", 1);
            header.Add("TipoDocumento", pagosGiros.ClienteCobrador.TipoIdentificacion);

            Utilidades.EjecutarServicioRest(Url, servicio, metodo, parametros, informacion, header);
        }

        #endregion

        #endregion Metodos
    }
}