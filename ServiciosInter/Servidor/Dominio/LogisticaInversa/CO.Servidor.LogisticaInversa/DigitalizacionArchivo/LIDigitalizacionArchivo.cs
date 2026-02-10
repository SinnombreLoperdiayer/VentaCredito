using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Transactions;
using CO.Servidor.Dominio.Comun.AdmEstadosGuia;
using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.LogisticaInversa.Comun;
using CO.Servidor.LogisticaInversa.Datos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros.Pagos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using Framework.Servidor.Servicios.ContratoDatos.Parametros.Consecutivos;
using CO.Servidor.Dominio.Comun.OperacionUrbana;
using System.Data.SqlClient;
using CO.Servidor.LogisticaInversa.Datos.Modelo;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using System.Threading.Tasks;
using CO.Servidor.Servicios.ContratoDatos.Comisiones;
using CO.Servidor.Dominio.Comun.Cajas;
using CO.Servidor.Dominio.Comun.Comisiones;
using CO.Servidor.Servicios.ContratoDatos.Cajas;
using CO.Servidor.Dominio.Comun.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria.AdmMasiva;

namespace CO.Servidor.LogisticaInversa.DigitalizacionArchivo
{
    /// <summary>
    /// Clase de negocio para las operaciones de digitalización y archivo
    /// </summary>
    internal class LIDigitalizacionArchivo : ControllerBase
    {
        private static readonly LIDigitalizacionArchivo instancia = (LIDigitalizacionArchivo)FabricaInterceptores.GetProxy(new LIDigitalizacionArchivo(), COConstantesModulos.DIGITALIZACION_Y_ARCHIVO);

        private IADFachadaAdmisionesMensajeria fachadaMensajeria = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>();

        private IOUFachadaOperacionUrbana fachadaOperacionUrbana = COFabricaDominio.Instancia.CrearInstancia<IOUFachadaOperacionUrbana>();

        private ICAFachadaCajas fachadaCajas = COFabricaDominio.Instancia.CrearInstancia<ICAFachadaCajas>();

        private IPUFachadaCentroServicios fachadaCentroServicios = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();

        private ICMFachadaComisiones fachadaComisiones = COFabricaDominio.Instancia.CrearInstancia<ICMFachadaComisiones>();


        /// <summary>
        /// Path almacena imagenes scanneadas
        /// </summary>
        private string filePath = ConfigurationManager.AppSettings["Controller.RutaDescargaArchivos"];

        public static LIDigitalizacionArchivo Instancia
        {
            get { return LIDigitalizacionArchivo.instancia; }
        }

        public LIDigitalizacionArchivo()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(PAAdministrador.Instancia.ConsultarParametrosFramework("Cultura"));
        }

        /// <summary>
        /// Retorna el comprobante de pago de un giro
        /// </summary>
        /// <param name="idAdmisionGiro">Identificador admisión giro</param>
        /// <returns>Archivo</returns>
        public string ObtenerComprobantePagoGiro(long idAdmisionGiro)
        {
            return LIRepositorioDigitalizacion.Instancia.ObtenerComprobantePagoGiro(idAdmisionGiro);
        }



        /// <summary>
        /// Retorna Volantes de una guía
        /// </summary>
        /// <param name="numeroGuia">Identificador admisión guía</param>
        /// <returns>Archivo</returns>
        public List<string> ObtenerVolantesGuia(long numeroGuia)
        {
            return LIRepositorioDigitalizacion.Instancia.ObtenerVolantesGuia(numeroGuia);
        }

        #region Archivo Giro

        /// <summary>
        /// Válida si existe el giro y si tiene un archivo
        /// </summary>
        /// <param name="imagenes">Imágenes</param>
        /// <returns>Imágenes</returns>
        public List<LIArchivoGiroDC> ExisteGiroArchivo(List<LIArchivoGiroDC> imagenes)
        {
            imagenes.ToList().ForEach(imagen =>
            {
                Int64 valorDecodificado;
                if (Int64.TryParse(imagen.ValorDecodificado, out valorDecodificado))
                {
                    imagen.IdAdmisionGiro = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesGiros>().ObtenerIdentificadorAdmisionGiro(valorDecodificado);
                    if (imagen.IdAdmisionGiro != 0)
                    {
                        imagen.ExisteGiro = true;
                        if (LIRepositorioDigitalizacion.Instancia.ConsultarArchivoGiroExiste(imagen))
                            imagen.ResultadoEscaner = ADEnumResultadoEscaner.Duplicada;
                        else
                            imagen.ResultadoEscaner = ADEnumResultadoEscaner.Exitosa;
                    }
                    else
                        imagen.ResultadoEscaner = ADEnumResultadoEscaner.NoAdmitida;
                }
                else
                    imagen.ResultadoEscaner = ADEnumResultadoEscaner.NoAdmitida;
            });

            return imagenes;
        }

        /// <summary>
        /// Adiciona un archivo giro
        /// </summary>
        /// <param name="imagen">Objeto imágen</param>
        public LIArchivoGiroDC AdicionarArchivoGiro(LIArchivoGiroDC imagen)
        {
            if (!string.IsNullOrEmpty(imagen.ValorDecodificado) && imagen.ValorDecodificado.Count() >= LOIConstantesLogisticaInversa.DIGITALIZACION_NUMERO_MINIMO_CARACTERES_VALOR_DECODIFICADO)
            {
                imagen.IdAdmisionGiro = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesGiros>().ObtenerIdentificadorAdmisionGiro(System.Convert.ToInt64(imagen.ValorDecodificado));

                if (imagen.IdAdmisionGiro != 0)
                {
                    string rutaArchivo = Path.Combine(this.filePath, COConstantesModulos.DIGITALIZACION_Y_ARCHIVO, imagen.RutaServidor);
                    imagen.RutaServidor = rutaArchivo;
                    imagen.ExisteGiro = true;
                    imagen.ExisteArchivo = LIRepositorioDigitalizacion.Instancia.ConsultarArchivoGiroExiste(imagen);

                    if (imagen.ExisteArchivo == false)
                        LIRepositorioDigitalizacion.Instancia.AdicionarArchivoGiro(imagen);
                    else
                        LIRepositorioDigitalizacion.Instancia.EditarArchivoGiro(imagen);
                }
            }
            return imagen;
        }

        /// <summary>
        /// Editar un archivo giro
        /// </summary>
        /// <param name="imagen">Objeto imágen</param>
        public void EditarArchivoGiro(LIArchivoGiroDC imagen)
        {
            string rutaArchivo = Path.Combine(this.filePath, COConstantesModulos.DIGITALIZACION_Y_ARCHIVO, imagen.RutaServidor);
            imagen.RutaServidor = rutaArchivo;

            LIRepositorioDigitalizacion.Instancia.EditarArchivoGiro(imagen);
        }

        /// <summary>
        /// Adicionar archivos de giro
        /// </summary>
        /// <param name="imagenes">Objeto imagenes</param>
        public List<LIArchivoGiroDC> AdicionarArchivosGiro(List<LIArchivoGiroDC> imagenes)
        {
            imagenes.ForEach(imagen =>
              {
                  AdicionarArchivoGiro(imagen);
              });

            return imagenes;
        }

        #endregion Archivo Giro

        #region Archivo Comprobante Pago Giro

        /// <summary>
        /// Válida si existe el giro y si tiene un archivo
        /// </summary>
        /// <param name="imagenes">Imágenes</param>
        /// <returns>Imágenes</returns>
        public List<LIArchivoComprobantePagoDC> ExisteComprobanteGiroArchivo(List<LIArchivoComprobantePagoDC> imagenes)
        {
            imagenes.ToList().ForEach(imagen =>
            {
                if (imagen.ValorDecodificado != null && imagen.ValorDecodificado != string.Empty && imagen.ValorDecodificado.Count() >= LOIConstantesLogisticaInversa.DIGITALIZACION_NUMERO_MINIMO_CARACTERES_VALOR_DECODIFICADO)
                {
                    Int64 valorDecodificado;
                    if (Int64.TryParse(imagen.ValorDecodificado, out valorDecodificado))
                    {
                        PGPagosGirosDC pagos = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesGiros>().ObtenerGiroPorComprobantePago(valorDecodificado);
                        if (pagos != null)
                        {
                            imagen.IdAdmisionGiro = pagos.IdAdmisionGiro;
                            imagen.NumeroGiro = pagos.IdGiro;
                            if (LIRepositorioDigitalizacion.Instancia.ConsultarArchivoComprobantePagoGuia(imagen))
                                imagen.ResultadoEscaner = ADEnumResultadoEscaner.Duplicada;
                            else
                                imagen.ResultadoEscaner = ADEnumResultadoEscaner.Exitosa;
                        }
                        else
                            imagen.ResultadoEscaner = ADEnumResultadoEscaner.NoAdmitida;
                    }
                }
                else
                    imagen.ResultadoEscaner = ADEnumResultadoEscaner.NoAdmitida;
            });

            return imagenes;
        }

        /// <summary>
        /// Adiciona un archivo giro
        /// </summary>
        /// <param name="imagen">Objeto imágen</param>
        public LIArchivoComprobantePagoDC AdicionarComprobanteArchivoGiro(LIArchivoComprobantePagoDC imagen)
        {
            if (!string.IsNullOrEmpty(imagen.ValorDecodificado) && imagen.ValorDecodificado.Count() >= LOIConstantesLogisticaInversa.DIGITALIZACION_NUMERO_MINIMO_CARACTERES_VALOR_DECODIFICADO)
            {
                PGPagosGirosDC pagos = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesGiros>().ObtenerGiroPorComprobantePago(System.Convert.ToInt64(imagen.ValorDecodificado));

                if (pagos != null)
                {
                    string rutaArchivo = Path.Combine(this.filePath, COConstantesModulos.DIGITALIZACION_Y_ARCHIVO, imagen.RutaServidor);
                    imagen.RutaServidor = rutaArchivo;
                    imagen.IdAdmisionGiro = pagos.IdAdmisionGiro;
                    imagen.NumeroGiro = pagos.IdGiro;
                    imagen.ExisteGiro = true;
                    imagen.ExisteArchivo = LIRepositorioDigitalizacion.Instancia.ConsultarArchivoComprobantePagoGuia(imagen);

                    if (imagen.ExisteArchivo == false)
                        LIRepositorioDigitalizacion.Instancia.AdicionarArchivoComprobantePago(imagen);
                    else
                        LIRepositorioDigitalizacion.Instancia.EditarArchivoComprobantePago(imagen);
                }
            }

            return imagen;
        }

        /// <summary>
        /// Editar un archivo giro
        /// </summary>
        /// <param name="imagen">Objeto imágen</param>
        public void EditarComprobanteArchivoGiro(LIArchivoComprobantePagoDC imagen)
        {
            string rutaArchivo = Path.Combine(this.filePath, COConstantesModulos.DIGITALIZACION_Y_ARCHIVO, imagen.RutaServidor);
            imagen.RutaServidor = rutaArchivo;
            LIRepositorioDigitalizacion.Instancia.EditarArchivoComprobantePago(imagen);
        }

        /// <summary>
        /// Adicionar comprobantes de pago de gito
        /// </summary>
        /// <param name="imagenes">Objeto imagenes</param>
        public List<LIArchivoComprobantePagoDC> AdicionarComprobanteArchivosGiro(List<LIArchivoComprobantePagoDC> imagenes)
        {
            imagenes.ForEach(imagen =>
            {
                AdicionarComprobanteArchivoGiro(imagen);
            });

            return imagenes;
        }

        public bool ConsultarArchivoComprobantePago(long idAdmisionGiro)
        {
            return LIRepositorioDigitalizacion.Instancia.ConsultarArchivoComprobantePago(idAdmisionGiro);
        }

        #endregion Archivo Comprobante Pago Giro

        #region Archivo Guías Mensajería

        /// <summary>
        /// Valida si se puede digitalizar la guía de acuerdo al estado
        /// </summary>
        /// <param name="imagenes">Colección de imágenes</param>
        public void GuardarGuiasCorrectas(List<LIArchivoGuiaMensajeriaDC> imagenes, bool entregaExitosa)
        {
            //imagenes.ForEach(imagen =>
            //  {
            //    AdicionarArchivoGuiaMensajeria(entregaExitosa, imagen);
            //  });
        }

        /// <summary>
        /// Adiciona un archivo guía mensajería
        /// </summary>
        /// <param name="entregaExitosa"></param>
        /// <param name="imagen"></param>
        public void AdicionarArchivoGuiaMensajeria(bool entregaExitosa, LIArchivoGuiaMensajeriaDC imagen)
        {
            LIRepositorioDigitalizacion.Instancia.AdicionarArchivoGuiaMensajeria(imagen);
        }

        /// <summary>
        /// Método para validar archivos de guías, para clasificarlas de acuerdo a su posible resultado
        /// </summary>
        /// <param name="entregaExitosa"></param>
        /// <param name="imagenes"></param>
        /// <returns></returns>
        public List<LIArchivoGuiaMensajeriaDC> ValidarArchivosGuias(bool entregaExitosa, List<LIArchivoGuiaMensajeriaDC> imagenes)
        {
            foreach (var imagen in imagenes)
            {
                Int64 valorDecodificado;
                if (Int64.TryParse(imagen.ValorDecodificado, out valorDecodificado))
                {
                    try
                    {

                        ADTrazaGuia ultimoEstadoGuia = EstadosGuia.ObtenerTrazaUltimoEstadoXNumGuia(valorDecodificado);

                        if (!ultimoEstadoGuia.IdEstadoGuia.HasValue || ultimoEstadoGuia.IdEstadoGuia.Value <= 0)
                        {
                            imagen.ResultadoEscaner = ADEnumResultadoEscaner.NoAdmitida;
                            continue;
                        }

                        imagen.IdAdmisionMensajeria = ultimoEstadoGuia.IdAdmision.Value;
                        imagen.NumeroGuia = ultimoEstadoGuia.NumeroGuia.Value;

                        ADGuia guiaAdmision = fachadaMensajeria.ObtenerGuiaXNumeroGuia(valorDecodificado);

                        ADTrazaGuia estadoGuia = new ADTrazaGuia
                        {
                            NumeroGuia = valorDecodificado,
                            IdAdmision = ultimoEstadoGuia.IdAdmision,
                            Observaciones = "Entregado desde digitalizacion",
                            Modulo = COConstantesModulos.PRUEBAS_DE_ENTREGA,
                            IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.Entregada,
                            Ciudad = imagen.NombreCiudad,
                            IdCiudad = imagen.IdCiudad,
                        };


                        if (ultimoEstadoGuia.IdEstadoGuia == (short)ADEnumEstadoGuia.Digitalizada || ultimoEstadoGuia.IdEstadoGuia == (short)ADEnumEstadoGuia.Archivada)
                        {
                            imagen.ResultadoEscaner = ADEnumResultadoEscaner.Duplicada;
                            imagen.RutaServidor = LIRepositorioDigitalizacion.Instancia.ValidarArchivoGuiaExiste(imagen.IdAdmisionMensajeria);
                        }
                        else if (ultimoEstadoGuia.IdEstadoGuia == (short)ADEnumEstadoGuia.Entregada)
                        {
                            using (TransactionScope transaccion = new TransactionScope())
                            {
                                if (fachadaOperacionUrbana.ActualizarGuiaPlanilla(valorDecodificado))
                                {
                                    estadoGuia.IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.Digitalizada;
                                    EstadosGuia.InsertaEstadoGuia(estadoGuia);
                                    //EMRL Validar si se marca como manual para q se envie a ECAPTURE o no
                                    imagen.Manual = LIRepositorioDigitalizacion.Instancia.IsManualGuiaEcapture(valorDecodificado);
                                    LIRepositorioDigitalizacion.Instancia.AdicionarArchivoGuiaMensajeria(imagen);
                                    imagen.ResultadoEscaner = ADEnumResultadoEscaner.Exitosa;
                                }
                                transaccion.Complete();
                            };
                        }
                        else if (ultimoEstadoGuia.IdEstadoGuia == (short)ADEnumEstadoGuia.EnReparto || ultimoEstadoGuia.IdEstadoGuia == (short)ADEnumEstadoGuia.Auditoria)
                        {
                            if (ultimoEstadoGuia.IdCentroServicioEstado != imagen.IdCentroLogistico && ultimoEstadoGuia.IdEstadoGuia == (short)ADEnumEstadoGuia.EnReparto)
                            {
                                imagen.ResultadoEscaner = ADEnumResultadoEscaner.EstadoInvalido;
                                imagen.DescripcionEstado = "Otra Regional";
                                continue;
                            }
                            Task.Factory.StartNew(() =>
                            {
                                ///Confirma direccion de entrega
                                fachadaMensajeria.ConfirmarDireccion(valorDecodificado, ControllerContext.Current.Usuario, true, false);

                            });

                            using (TransactionScope transaccion = new TransactionScope())
                            {
                                ///Se actualiza la planilla del mensajero
                                if (fachadaOperacionUrbana.ActualizarGuiaPlanilla(valorDecodificado))
                                {
                                    EntregarDigitalizar(imagen, estadoGuia);

                                    ///Adiciona las tablas de archivo
                                    LIRepositorioDigitalizacion.Instancia.AdicionarArchivoGuiaMensajeria(imagen);

                                    imagen.ResultadoEscaner = ADEnumResultadoEscaner.Exitosa;

                                }
                                else
                                {
                                    imagen.ResultadoEscaner = ADEnumResultadoEscaner.EstadoInvalido;
                                    imagen.DescripcionEstado = "Error de planilla";
                                }

                                transaccion.Complete();
                            };

                        }
                        else
                        {
                            imagen.ResultadoEscaner = ADEnumResultadoEscaner.EstadoInvalido;
                            imagen.EstadoGuia = ultimoEstadoGuia.DescripcionEstadoGuia;
                        }
                    }
                    catch
                    {
                        imagen.ResultadoEscaner = ADEnumResultadoEscaner.NoAdmitida;
                    }
                }
                else
                {
                    imagen.ResultadoEscaner = ADEnumResultadoEscaner.NoAdmitida;
                }
            };
            return imagenes;
        }


        /// <summary>
        /// Edita un archivo guía de mensajería
        /// </summary>
        /// <param name="imagen">Objeto imágen</param>
        public void EditarArchivoGuiaMensajeria(LIArchivoGuiaMensajeriaDC imagen)
        {
            LIRepositorioDigitalizacion.Instancia.EditarArchivoGuiaMensajeria(imagen);
        }

        /// <summary>
        /// Archiva una guía
        /// </summary>
        /// <param name="guia">Objeto guía</param>
        public LIArchivoGuiaMensajeriaDC GuardarArchivoGuiaMensajeria(LIArchivoGuiaMensajeriaDC guia)
        {
            if (!string.IsNullOrEmpty(guia.ValorDecodificado))
            {
                Int64 valorDecodificado;
                ControllerException excepcion;
                ADGuia guiaMensajeria;

                if (Int64.TryParse(guia.ValorDecodificado, out valorDecodificado))
                {
                    var g = LIRepositorioDigitalizacion.Instancia.ObtenerArchivoGuiaFS(guia);

                    if (g == null)
                    {
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.DIGITALIZACION_Y_ARCHIVO, "0", "La guía no se pudo archivar porque la imagen no esta sincronizada"));
                    }
                    guiaMensajeria = fachadaMensajeria.ObtenerGuiaXNumeroGuia(valorDecodificado);
                    guia.IdAdmisionMensajeria = guiaMensajeria.IdAdmision;

                    ADEnumEstadoGuia ultimoEstado = EstadosGuia.ObtenerUltimoEstadoxNumero(valorDecodificado);
                    guia.EstadoDatosEdicion.IdEstadoDato = ObtenerIdentificadorEstadoDatosGuia(guia.EstadoDatosEdicion.Descripcion);
                    guia.EstadoDatosEntrega.IdEstadoDato = ObtenerIdentificadorEstadoDatosGuia(guia.EstadoDatosEntrega.Descripcion);
                    guia.EstadoFisicoGuia.IdEstadoFisico = ObtenerIdentificadorEstadoFisicoGuia(guia.EstadoFisicoGuia.Descripcion);
                    guia.NumeroGuia = valorDecodificado;

                    ADTrazaGuia EstadoGuia = new ADTrazaGuia()
                    {
                        Ciudad = guia.NombreCiudad,
                        DescripcionEstadoGuia = ultimoEstado.ToString(),
                        FechaGrabacion = DateTime.Now,
                        IdAdmision = guia.IdAdmisionMensajeria,
                        IdCiudad = guia.IdCiudad,
                        IdEstadoGuia = (short)ultimoEstado,
                        IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.Archivada,
                        Modulo = COConstantesModulos.DIGITALIZACION_Y_ARCHIVO,
                        NumeroGuia = long.Parse(guia.ValorDecodificado),
                        Observaciones = string.Empty,
                        Usuario = ControllerContext.Current.Usuario
                    };


                    PAConsecutivoDC consecutivo = PAAdministrador.Instancia.ObtenerDatosConsecutivoxCol(PAEnumConsecutivos.Cajas_PruebasEntrega, guia.IdCol);
                    LIRepositorioDigitalizacionArchivo.Instancia.ObtenerCajaLotePosicion(guia);
                    lock (this)
                    {
                        using (TransactionScope transaccion = new TransactionScope())
                        {
                            if (EstadosGuia.ValidarInsertarEstadoGuia(EstadoGuia) == 0)
                                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.DIGITALIZACION_Y_ARCHIVO, "0", "La guía se encuentra en estado " + ultimoEstado.ToString() + " y no puede ser pasada a estado " + ADEnumEstadoGuia.Archivada.ToString() + "."));
                            else
                            {
                                if (consecutivo.Actual < guia.Caja)
                                {
                                    long nuevoConsec = 0;

                                    while (guia.Caja > nuevoConsec)
                                    {
                                        nuevoConsec = PAAdministrador.Instancia.ObtenerConsecutivoPorCol(PAEnumConsecutivos.Cajas_PruebasEntrega, guia.IdCol);
                                    }
                                }

                                if (guia.CajaLlena)
                                {
                                    PAConsecutivoDC consecutivoActual = PAAdministrador.Instancia.ObtenerDatosConsecutivoxCol(PAEnumConsecutivos.Cajas_PruebasEntrega, guia.IdCol);
                                    long idConsecutivoNuevo = PAAdministrador.Instancia.ObtenerConsecutivoPorCol(PAEnumConsecutivos.Cajas_PruebasEntrega, guia.IdCol);
                                    if (idConsecutivoNuevo > consecutivoActual.Actual + 1)
                                    {
                                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.DIGITALIZACION_Y_ARCHIVO, "0", "Se detectó un error de concurrencia al actualizar el número de caja. Intente nuevamente la operación."));
                                    }
                                    guia.Caja = idConsecutivoNuevo;
                                }

                            }
                            LIRepositorioDigitalizacion.Instancia.ArchivarGuia(guia);
                            EnviarMensajeTexto(guiaMensajeria);
                            transaccion.Complete();
                        }
                    }

                }
                else
                {
                    excepcion = new ControllerException(COConstantesModulos.DIGITALIZACION_Y_ARCHIVO, LOIEnumTipoErrorLogisticaInversa.EX_GUIA_NO_EXISTE.ToString(), LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.EX_GUIA_NO_EXISTE));
                    throw new FaultException<ControllerException>(excepcion);
                }
            }
            return guia;
        }

        private void EnviarMensajeTexto(ADGuia guia)
        {
            if (guia.IdServicio == (short)EnumTipoServicio.Notificaciones)
            {
                new Task(() => Controller.Servidor.Integraciones.MensajesTexto.Instancia.EnviarMensajeNoCliente(
                                        guia.IdAdmision,
                                        (long)guia.NumeroGuia,
                                        Controller.Servidor.Integraciones.EnumMensajeNocliente.CertiJudicial.ToString(),
                                        guia.Remitente.Telefono,
                                        guia.ValorTotal)
                                    ).Start();
            }
        }

        /// <summary>
        /// Método para obtener un archivo guia
        /// </summary>
        /// <param name="imagen"></param>
        public LIArchivoGuiaMensajeriaDC ObtenerArchivoGuiaFS(LIArchivoGuiaMensajeriaDC archivoGuia)
        {
            return LIRepositorioDigitalizacion.Instancia.ObtenerArchivoGuiaFS(archivoGuia);
        }


        public LIArchivoGuiaMensajeriaDC ObtenerArchivoGuiaSispostal(LIArchivoGuiaMensajeriaDC archivoGuia)
        {
            return LIRepositorioDigitalizacion.Instancia.ObtenerArchivoGuiaSispostal(archivoGuia);
        }

        /// <summary>
        /// Método para obtener imagen de la Fachada para una guia
        /// </summary>
        /// <param name="imagen"></param>
        public List<LIArchivoGuiaMensajeriaFachadaDC> ObtenerArchivoGuiaFachadaFS(long numeroGuia)
        {
            List<LIArchivoGuiaMensajeriaFachadaDC> lstArchivos = LIRepositorioDigitalizacion.Instancia.ObtenerArchivoGuiaFachadaFS(numeroGuia);
            if (lstArchivos != null)
            {
                lstArchivos.ForEach(ar =>
                {
                    if (!string.IsNullOrEmpty(ar.RutaServidor))
                    {
                        FileStream stream = File.OpenRead(ar.RutaServidor);
                        byte[] fileBytes = new byte[stream.Length];
                        stream.Read(fileBytes, 0, fileBytes.Length);
                        stream.Close();
                        ar.Imagen = Convert.ToBase64String(fileBytes);
                    }
                });
            }
            return lstArchivos;

        }


        /// <summary>
        /// Obtiene el identificador del estado de los datos de la guía
        /// </summary>
        /// <param name="estadoGuia">Estado guia</param>
        /// <returns>Identificador estado</returns>
        public string ObtenerIdentificadorEstadoDatosGuia(string estadoGuia)
        {
            if (estadoGuia == LIEnumEstadoDatosGuia.ILEGIBLE.ToString())
                return LOIConstantesLogisticaInversa.ID_ESTADO_ILEGIBLE;
            else if (estadoGuia == LIEnumEstadoDatosGuia.LEGIBLE.ToString())
                return LOIConstantesLogisticaInversa.ID_ESTADO_LEGIBLE;
            else if (estadoGuia == LIEnumEstadoDatosGuia.INCOMPLETA.ToString())
                return LOIConstantesLogisticaInversa.ID_ESTADO_INCOMPLETO;
            else
                return LOIConstantesLogisticaInversa.ID_ESTADO_NO_CLASIFICADO;
        }

        /// <summary>
        /// Obtiene el identificador del estado físico de la guía
        /// </summary>
        /// <param name="estadoGuia">Estado guia</param>
        /// <returns>Identificador estado</returns>
        public string ObtenerIdentificadorEstadoFisicoGuia(string estadoFisico)
        {
            if (estadoFisico == LIEnumEstadoFisicoGuia.BUENO.ToString())
                return LOIConstantesLogisticaInversa.ID_ESTADO_BUENO;
            else if (estadoFisico == LIEnumEstadoFisicoGuia.MALO.ToString())
                return LOIConstantesLogisticaInversa.ID_ESTADO_MALO;
            else if (estadoFisico == LIEnumEstadoFisicoGuia.REGULAR.ToString())
                return LOIConstantesLogisticaInversa.ID_ESTADO_REGULAR;
            else
                return LOIConstantesLogisticaInversa.ID_ESTADO_NO_CLASIFICADO;
        }

        /// <summary>
        /// Obtiene las guías archivadas
        /// </summary>
        /// <returns>Colección guías archivadas</returns>
        public List<LIArchivoGuiaMensajeriaDC> ObtenerArchivoGuia(long idCol)
        {
            return LIRepositorioDigitalizacion.Instancia.ObtenerArchivoGuia(idCol);
        }


        public ADGuia ObtenerFechaEstimadaEntregaGuia(long numeroguia)
        {
            return LIRepositorioDigitalizacion.Instancia.ObtenerFechaEstimadaEntregaGuia(numeroguia);
        }


        /// <summary>
        /// Metodo que obtiene los datos de archivo guía de mensajería    
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public LIArchivoGuiaMensajeriaDC ObtenerArchivoGuiaxNumeroGuia(long numeroGuia)
        {
            return LIRepositorioDigitalizacion.Instancia.ObtenerArchivoGuiaxNumeroGuia(numeroGuia);
        }


        /// <summary>
        /// Método que afecta la caja destino de una agencia cuando hay afectacion por entrega
        /// </summary>
        /// <param name="guia"></param>
        private void AfectarCajaAgenciaPorEntrega(ADGuia guia)
        {
            PUCentroServiciosDC agenciaDestino = fachadaCentroServicios.ObtenerAgenciaLocalidad(guia.IdCiudadDestino);

            CMComisionXVentaCalculadaDC comisionDestino = fachadaComisiones.CalcularComisionesxVentas(
         new Servidor.Servicios.ContratoDatos.Comisiones.CMConsultaComisionVenta()
         {
             IdCentroServicios = agenciaDestino.IdCentroServicio,
             IdServicio = guia.IdServicio,
             TipoComision = Servidor.Servicios.ContratoDatos.Comisiones.CMEnumTipoComision.Entregar,
             ValorBaseComision = guia.ValorTotal,
             NumeroOperacion = guia.NumeroGuia
         });

            comisionDestino.EsRegistroValido = true;

            List<CARegistroVentaFormaPagoDC> formasDePago = new List<CARegistroVentaFormaPagoDC>();

            formasDePago.Add(new CARegistroVentaFormaPagoDC()
            {
                Valor = guia.ValorTotal,
                IdFormaPago = TAConstantesServicios.ID_FORMA_PAGO_CONTADO,
                Descripcion = TAConstantesServicios.DESCRIPCION_FORMA_PAGO_CONTADO,
                NumeroAsociado = ""
            });

            CARegistroTransacCajaDC registro = new Servidor.Servicios.ContratoDatos.Cajas.CARegistroTransacCajaDC()
            {
                InfoAperturaCaja = new CAAperturaCajaDC()
                {
                    IdCaja = 0,
                    IdCodigoUsuario = ControllerContext.Current.CodigoUsuario,
                },
                TipoDatosAdicionales = CAEnumTipoDatosAdicionales.PEA,
                IdCentroResponsable = comisionDestino.IdCentroServicioResponsable,
                IdCentroServiciosVenta = comisionDestino.IdCentroServicioVenta,
                NombreCentroResponsable = comisionDestino.NombreCentroServicioResponsable,
                NombreCentroServiciosVenta = comisionDestino.NombreCentroServicioVenta,
                RegistrosTransacDetallesCaja = new List<Servidor.Servicios.ContratoDatos.Cajas.CARegistroTransacCajaDetalleDC>()
        {
          new CO.Servidor.Servicios.ContratoDatos.Cajas.CARegistroTransacCajaDetalleDC() {
             Cantidad = 1,
             ConceptoCaja = new CAConceptoCajaDC() { IdConceptoCaja =(int) CAEnumConceptosCaja.PAGO_DE_ENVIO_AL_COBRO },
             ConceptoEsIngreso = true,
             EstadoFacturacion = CAEnumEstadoFacturacion.FAC,
             FechaFacturacion = DateTime.Now,
             Numero = guia.NumeroGuia,
             NumeroFactura = guia.NumeroGuia.ToString(),
             Observacion = guia.Observaciones,
             ValorDeclarado = guia.ValorDeclarado,
             ValoresAdicionales = guia.ValorAdicionales,
             ValorImpuestos = guia.ValorTotalImpuestos,
             ValorPrimaSeguros = guia.ValorPrimaSeguro, ValorRetenciones = guia.ValorTotalRetenciones,
             ValorServicio = guia.ValorServicio,
             ValorTercero = 0
          }
        },
                ValorTotal = guia.ValorTotal,
                TotalImpuestos = guia.ValorTotalImpuestos,
                TotalRetenciones = guia.ValorTotalRetenciones,
                Usuario = ControllerContext.Current.Usuario,
                RegistroVentaFormaPago = formasDePago
            };

            fachadaCajas.AdicionarMovimientoCaja(registro);
        }




        #endregion Archivo Guías Mensajería

        #region Volantes de devolución

        /// <summary>
        /// Método para validar la digitalización de los volantes de devolución
        /// </summary>
        /// <param name="imagen"></param>
        /// <returns></returns>
        public List<LIEvidenciaDevolucionDC> ValidarArchivosVolantes(List<LIEvidenciaDevolucionDC> imagenes)
        {
            imagenes.ForEach(imagen =>
                {
                    LIRepositorioDigitalizacionArchivo.Instancia.ValidarArchivosVolantes(imagen);
                });
            return imagenes;
        }


        public List<LIEvidenciaDevolucionDC> ValidarArchivosVolantesWPF(List<LIEvidenciaDevolucionDC> imagenes)
        {
            foreach (LIEvidenciaDevolucionDC iteImagen in imagenes)
            {
                long valorDecodificado;
                if (long.TryParse(iteImagen.ValorDecodificado, out valorDecodificado))
                {

                    LIEvidenciaDevolucionDC evidenciaBD = LIRepositorioDigitalizacionArchivo.Instancia.ObtenerEvidenciaDevolucion(valorDecodificado);

                    if (evidenciaBD == null)
                    {
                        iteImagen.ResultadoEscaner = ADEnumResultadoEscaner.NoAdmitida;
                    }
                    else
                    {
                        iteImagen.IdEvidenciaDevolucion = evidenciaBD.IdEvidenciaDevolucion;
                        iteImagen.NumeroGuia = evidenciaBD.NumeroGuia;

                        if (evidenciaBD.EstaDigitalizado)
                            iteImagen.ResultadoEscaner = ADEnumResultadoEscaner.Duplicada;
                        else
                        {
                            LIRepositorioDigitalizacionArchivo.Instancia.ActualizaEvidenciaDevolucionADigitalizado(evidenciaBD.IdEvidenciaDevolucion.Value);

                            iteImagen.ResultadoEscaner = ADEnumResultadoEscaner.Exitosa;
                        }
                    }
                }
                else
                {
                    iteImagen.ResultadoEscaner = ADEnumResultadoEscaner.NoIdentificada;
                }

            }

            return imagenes;
        }


        public LIEvidenciaDevolucionDC AsociarNumeroGuiaAVolante(LIEvidenciaDevolucionDC EvidenciaVolante)
        {
            try
            {
                long valorDecodificado;
                if (long.TryParse(EvidenciaVolante.ValorDecodificado, out valorDecodificado))
                {
                    // 1. Actualiza en EvidenciaDevolucion_MEN el numero de Volante para Asociarlo a ala Guia
                    long newIdEvidenciaDevolucion = LIRepositorioDigitalizacionArchivo.Instancia.AsociarGuiaAVolante(EvidenciaVolante, valorDecodificado);

                    // 2. Actualiza a Digitalizado y (si no Existe) crea Registo en ArchivoEvidencia_MEN para la sincronizacion.
                    LIRepositorioDigitalizacionArchivo.Instancia.ActualizaEvidenciaDevolucionADigitalizado(EvidenciaVolante.IdEvidenciaDevolucion.Value);
                    EvidenciaVolante.ResultadoEscaner = ADEnumResultadoEscaner.Exitosa;

                }
                else
                {
                    EvidenciaVolante.ResultadoEscaner = ADEnumResultadoEscaner.NoIdentificada;
                }
                return EvidenciaVolante;

            }
            catch (FaultException<ControllerException> errorControlado)
            {
                ControllerException excepcion = new ControllerException(COConstantesModulos.DIGITALIZACION_Y_ARCHIVO
                                    , "0", errorControlado.Detail.Mensaje);
                throw new FaultException<ControllerException>(excepcion);
            }
            catch (Exception error)
            {
                ControllerException excepcion = new ControllerException(COConstantesModulos.DIGITALIZACION_Y_ARCHIVO
                                    , "0", error.Message);
                throw new FaultException<ControllerException>(excepcion);
            }
        }


        public List<LIEvidenciaDevolucionDC> ObtenerEvidenciaDevolucionxGuia(long NumeroGuia)
        {

            //1. Validar si la Guia Existe
            ADGuia Guia;
            #region Obtener Guia
            try
            {
                Guia = fachadaMensajeria.ObtenerGuiaXNumeroGuia(NumeroGuia);
            }
            catch (FaultException<ControllerException>)
            {
                ControllerException excepcion = new ControllerException(COConstantesModulos.DIGITALIZACION_Y_ARCHIVO
                             , "0", "Numero de Guía no Existe");
                excepcion.Mensaje = excepcion.Mensaje;
                throw new FaultException<ControllerException>(excepcion);
            }
            catch (Exception)
            {
                ControllerException excepcion = new ControllerException(COConstantesModulos.DIGITALIZACION_Y_ARCHIVO
                             , "0", "Numero de Guía no Existe");
                throw new FaultException<ControllerException>(excepcion);
            }
            #endregion

            return LIRepositorioDigitalizacionArchivo.Instancia.ObtenerEvidenciaDevolucionxGuia(NumeroGuia);
        }


        /// <summary>
        ///  Guarda un archivo de un volante
        /// </summary>
        /// <param name="imagenes">Colección de imágenes</param>
        public void GuardarVolantesCorrectos(List<LIEvidenciaDevolucionDC> imagenes)
        {
            imagenes.ForEach(imagen =>
            {
                imagen.Archivo.IdEvidenciaDevolucion = imagen.IdEvidenciaDevolucion;
                fachadaMensajeria.AdicionarArchivo(imagen.Archivo);
                LIRepositorioDigitalizacionArchivo.Instancia.ActualizarDigitalizadoVolante(imagen);
            });

        }

        /// <summary>
        /// Edita un archivo de un volante
        /// </summary>
        /// <param name="imagen">Objeto imágen</param>
        public void EditarArchivoVolante(LIArchivosDC imagen)
        {
            fachadaMensajeria.EditarArchivoEvidencia(imagen);
        }

        #endregion Volantes de devolución

        #region GeneracionImagenes
        /// <summary>
        /// Obtiene los numeros de guias y la ruta
        /// </summary>
        /// <returns>Colección numeros de guia y rutas</returns>
        public List<LIArchivoGuiaMensajeriaDC> ObtenerGuiasRuta(string imagenesGenerar, int idCliente, string idCiudad, int idSucursal, DateTime fechaAdmisionInical, DateTime fechaAdmisionFinal, long guiaFacturaInicial, long guiaFacturaFinal, long ordenCompraInicial, long ordenCompraFinal)
        {
            return LIRepositorioDigitalizacion.Instancia.ObtenerGuiasRuta(imagenesGenerar, idCliente, idCiudad, idSucursal, fechaAdmisionInical, fechaAdmisionFinal, guiaFacturaInicial, guiaFacturaFinal, ordenCompraInicial, ordenCompraFinal);
        }

        #endregion

        #region Digitalizacion de agencias


        /// <summary>
        /// Método para validar archivos de guías, para clasificarlas de acuerdo a su posible resultado
        /// </summary>
        /// <param name="entregaExitosa"></param>
        /// <param name="imagenes"></param>
        /// <returns></returns>
        public List<LIArchivoGuiaMensajeriaDC> ValidarArchivosAgencias(List<LIArchivoGuiaMensajeriaDC> imagenes)
        {
            foreach (var imagen in imagenes)
            {
                Int64 valorDecodificado;
                if (Int64.TryParse(imagen.ValorDecodificado, out valorDecodificado))
                {
                    try
                    {
                        ADTrazaGuia ultimoEstadoGuia = EstadosGuia.ObtenerTrazaUltimoEstadoXNumGuia(valorDecodificado);


                        if (!ultimoEstadoGuia.IdEstadoGuia.HasValue || ultimoEstadoGuia.IdEstadoGuia.Value <= 0)
                        {
                            imagen.ResultadoEscaner = ADEnumResultadoEscaner.NoAdmitida;
                            continue;
                        }
                        else
                        {
                            imagen.IdAdmisionMensajeria = ultimoEstadoGuia.IdAdmision.Value;
                            imagen.NumeroGuia = ultimoEstadoGuia.NumeroGuia.Value;

                        }

                        ADTrazaGuia estadoGuia = new ADTrazaGuia
                        {
                            NumeroGuia = valorDecodificado,
                            IdAdmision = ultimoEstadoGuia.IdAdmision,
                            Observaciones = string.Empty,
                            Modulo = COConstantesModulos.PRUEBAS_DE_ENTREGA,
                            Ciudad = imagen.NombreCiudad,
                            IdCiudad = imagen.IdCiudad,
                        };


                        if (ultimoEstadoGuia.IdEstadoGuia == (short)ADEnumEstadoGuia.Digitalizada || ultimoEstadoGuia.IdEstadoGuia == (short)ADEnumEstadoGuia.Archivada)
                        {
                            imagen.ResultadoEscaner = ADEnumResultadoEscaner.Duplicada;
                            imagen.RutaServidor = LIRepositorioDigitalizacion.Instancia.ValidarArchivoGuiaExiste(imagen.IdAdmisionMensajeria);
                        }
                        else if (ultimoEstadoGuia.IdEstadoGuia == (short)ADEnumEstadoGuia.Entregada || ultimoEstadoGuia.IdEstadoGuia == (short)ADEnumEstadoGuia.DevolucionRatificada || ultimoEstadoGuia.IdEstadoGuia == (short)ADEnumEstadoGuia.Incautado || ultimoEstadoGuia.IdEstadoGuia == (short)ADEnumEstadoGuia.DisposicionFinal || ultimoEstadoGuia.IdEstadoGuia == (short)ADEnumEstadoGuia.CasoFortuito || ultimoEstadoGuia.IdEstadoGuia == (short)ADEnumEstadoGuia.Indemnizacion || ultimoEstadoGuia.IdEstadoGuia == (short)ADEnumEstadoGuia.Custodia)
                        {

                            using (TransactionScope transaccion = new TransactionScope())
                            {
                                estadoGuia.IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.Digitalizada;
                                ///Insertar segunda transicion de estado a digitalizado
                                EstadosGuia.InsertaEstadoGuia(estadoGuia);
                                //EMRL Validar si se marca como manual para q se envie a ECAPTURE o no
                                imagen.Manual = LIRepositorioDigitalizacion.Instancia.IsManualGuiaEcapture(valorDecodificado);
                                ///Adiciona las tablas de archivo
                                LIRepositorioDigitalizacion.Instancia.AdicionarArchivoGuiaMensajeria(imagen);

                                imagen.ResultadoEscaner = ADEnumResultadoEscaner.Exitosa;

                                transaccion.Complete();
                            };
                        }
                        else
                        {

                            ADGuia guiaAdmision = fachadaMensajeria.ObtenerGuiaXNumeroGuia(valorDecodificado);

                            if (!fachadaCentroServicios.ValidarCiudadSeApoyaCOL(guiaAdmision.IdCiudadDestino, imagen.IdCentroLogistico))
                            {
                                imagen.ResultadoEscaner = ADEnumResultadoEscaner.EstadoInvalido;
                                imagen.DescripcionEstado = "Otra Regional";
                                continue;

                            }


                            using (TransactionScope transaccion = new TransactionScope())
                            {

                                if (ultimoEstadoGuia.IdEstadoGuia == (short)ADEnumEstadoGuia.CentroAcopio || ultimoEstadoGuia.IdEstadoGuia == (short)ADEnumEstadoGuia.Reenvio)
                                {
                                    if ((guiaAdmision.IdCentroServicioDestino == guiaAdmision.TrazaGuiaEstado.IdCentroServicioEstado) || (guiaAdmision.IdCiudadDestino == guiaAdmision.TrazaGuiaEstado.IdCiudad))
                                    {
                                        EntregarDigitalizar(imagen, estadoGuia);
                                    }
                                    else
                                    {
                                        imagen.ResultadoEscaner = ADEnumResultadoEscaner.EstadoInvalido;
                                        imagen.DescripcionEstado = "Otra Regional";
                                        transaccion.Dispose();
                                        continue;

                                    }
                                }
                                else if (ultimoEstadoGuia.IdEstadoGuia == (short)ADEnumEstadoGuia.EnReparto || ultimoEstadoGuia.IdEstadoGuia == (short)ADEnumEstadoGuia.Distribucion)
                                {
                                    if (guiaAdmision.IdCentroServicioDestino != imagen.IdCentroLogistico)
                                    {
                                        EntregarDigitalizar(imagen, estadoGuia);
                                    }
                                    else
                                    {
                                        imagen.ResultadoEscaner = ADEnumResultadoEscaner.EstadoInvalido;
                                        imagen.DescripcionEstado = "Descargar Mensajero";
                                        transaccion.Dispose();
                                        continue;

                                    }
                                }
                                else if (ultimoEstadoGuia.IdEstadoGuia == (short)ADEnumEstadoGuia.TransitoRegional || ultimoEstadoGuia.IdEstadoGuia == (short)ADEnumEstadoGuia.TransitoNacional)
                                {

                                    if (string.IsNullOrEmpty(imagen.IdCiudad))
                                    {
                                        PUCentroServiciosDC ces = fachadaCentroServicios.ObtenerCentroServicio(ControllerContext.Current.IdCentroServicio);
                                        estadoGuia.IdCiudad = ces.CiudadUbicacion.IdLocalidad;
                                        estadoGuia.Ciudad = ces.CiudadUbicacion.Nombre;
                                    }
                                    EntregarDigitalizar(imagen, estadoGuia);

                                }
                                else
                                {
                                    imagen.ResultadoEscaner = ADEnumResultadoEscaner.EstadoInvalido;
                                    imagen.EstadoGuia = ultimoEstadoGuia.DescripcionEstadoGuia;
                                    transaccion.Dispose();
                                    continue;
                                }
                                if (guiaAdmision.EsAlCobro && !guiaAdmision.EstaPagada && !fachadaMensajeria.AlCobroCargadoACoordinadorCol(guiaAdmision.IdAdmision))
                                {
                                    fachadaMensajeria.ActualizarPagadoGuia(guiaAdmision.IdAdmision);
                                    AfectarCajaAgenciaPorEntrega(guiaAdmision);
                                }

                                ///Adiciona las tablas de archivo
                                LIRepositorioDigitalizacion.Instancia.AdicionarArchivoGuiaMensajeria(imagen);

                                imagen.ResultadoEscaner = ADEnumResultadoEscaner.Exitosa;
                                transaccion.Complete();
                            }
                        }
                    }
                    catch
                    {
                        imagen.ResultadoEscaner = ADEnumResultadoEscaner.NoAdmitida;
                    }


                }
                else
                {
                    imagen.ResultadoEscaner = ADEnumResultadoEscaner.NoAdmitida;
                }
            }
            return imagenes;
        }

        /// <summary>
        /// Método para entregar y digitalizar una prueba de entrega
        /// </summary>
        /// <param name="imagen"></param>
        /// <param name="estadoGuia"></param>
        private void EntregarDigitalizar(LIArchivoGuiaMensajeriaDC imagen, ADTrazaGuia estadoGuia)
        {
            estadoGuia.Observaciones = "Entregado desde digitalizacion";
            estadoGuia.IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.Entregada;

            ///Insertar segunda transicion entregada
            EstadosGuia.InsertaEstadoGuia(estadoGuia);

            estadoGuia.Ciudad = imagen.NombreCiudad;
            estadoGuia.IdCiudad = imagen.IdCiudad;


            estadoGuia.IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.Digitalizada;

            ///Insertar tercera transicion digitalizada
            EstadosGuia.InsertaEstadoGuia(estadoGuia);
        }
        #endregion


    }
}