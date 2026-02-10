using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Transactions;
using CO.Servidor.Dominio.Comun.AdmEstadosGuia;
using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.Suministros;
using CO.Servidor.LogisticaInversa.Comun;
using CO.Servidor.LogisticaInversa.Datos;
using CO.Servidor.LogisticaInversa.Datos.Modelo;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Area;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using Framework.Servidor;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.Reglas;
using Framework.Servidor.Excepciones;
using Framework.Servidor.MotorReglas;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using Framework.Servidor.Servicios.ContratoDatos.Parametros.Consecutivos;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using System.Text;
using CO.Servidor.Dominio.Comun.CentroServicios;
using System.Net;
using CO.Servidor.Dominio.Comun.Cajas;


namespace CO.Servidor.LogisticaInversa.Telemercadeo.Custodia
{
    internal class LIConfiguradorCustodia : ControllerBase
    {
        private static readonly LIConfiguradorCustodia instancia = (LIConfiguradorCustodia)FabricaInterceptores.GetProxy(new LIConfiguradorCustodia(), COConstantesModulos.TELEMERCADEO);

        public static LIConfiguradorCustodia Instancia
        {
            get { return LIConfiguradorCustodia.instancia; }
        }

        private ISUFachadaSuministros fachadaSuministros = COFabricaDominio.Instancia.CrearInstancia<ISUFachadaSuministros>();
        private IPUFachadaCentroServicios fachadaCes = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();
        private IADFachadaAdmisionesMensajeria fachadaMensajeria = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>();
        private ICAFachadaCajas fachadaCajas = COFabricaDominio.Instancia.CrearInstancia<ICAFachadaCajas>();
        static string ftpEvidencias = PAAdministrador.Instancia.ConsultarParametrosFramework("ftpEvidenciaCustodia");
        static string passFTPGlobal = PAAdministrador.Instancia.ConsultarParametrosFramework("passFtpDigitalizaci");
        static string userFTPGloblal = PAAdministrador.Instancia.ConsultarParametrosFramework("UserFtpDigitalizaci");
        private NetworkCredential ftpCredential = new NetworkCredential(userFTPGloblal, passFTPGlobal);
        private string ftpPath = string.Concat(ftpEvidencias, DateTime.Now.Date.ToString("u").Substring(0, 10));

        #region Consultas

        /// <summary>
        /// Consulta las guias de acuerdo a un  estado y una localidad
        /// </summary>
        /// <param name="idEstado"></param>
        /// <param name="localidad"></param>
        /// <param name="numeroPagina"></param>
        /// <param name="tamanoPagina"></param>
        /// <returns></returns>
        public List<LIGuiaCustodiaDC> ObtenerGuiasEstado(ADEnumEstadoGuia estado, string localidad, int numeroPagina, int tamanoPagina)
        {
            return LIRepositorioTelemercadeo.Instancia.ObtenerGuiasEstado((short)estado, localidad, numeroPagina, tamanoPagina);
        }

        /// <summary>
        /// Método para obtener una guía en custodia
        /// </summary>
        /// <param name="idEstado"></param>
        /// <param name="numeroGuia"></param>
        /// <param name="localidad"></param>
        /// <returns></returns>
        public LIGuiaCustodiaDC ObtenerGuiaCustodia(short idEstado, long numeroGuia, string localidad)
        {
            return LIRepositorioTelemercadeo.Instancia.ObtenerGuiaCustodia((short)idEstado, numeroGuia, localidad);
        }

        /// <summary>
        /// Método que almacena el cambio de estado y el motivo de salida de custodia
        /// </summary>
        /// <param name="traza"></param>
        /// <param name="motivo"></param>
        /// <returns></returns>
        public long GuardarCambioEstado(LICambioEstadoCustodia ceCustodia)
        {
            ADGuia guia = fachadaMensajeria.ObtenerGuiaXNumeroGuia((long)ceCustodia.IdTrazaGuia);
            PUCentroServiciosDC ces = fachadaCes.ObtenerCentroServicio(ceCustodia.IdCentroServicioEstado);
            ADTrazaGuiaImpresoDC trazaImpreso = new ADTrazaGuiaImpresoDC();

            ADTrazaGuia traza = new ADTrazaGuia
            {
                NumeroGuia = ceCustodia.IdTrazaGuia,
                IdAdmision = guia.IdAdmision,
                IdNuevoEstadoGuia = ceCustodia.IdNuevoEstadoGuia,
                IdCiudad = ces.IdMunicipio,
                Ciudad = ces.CiudadUbicacion.Nombre,
                Modulo = ceCustodia.Modulo,
                FechaGrabacion = ceCustodia.FechaGrabacion,
                Usuario = ceCustodia.Usuario,
                IdCentroServicioEstado = ceCustodia.IdCentroServicioEstado,
                NombreCentroServicioEstado = ces.Nombre
            };

            traza.IdEstadoGuia = (short)EstadosGuia.ObtenerUltimoEstadoxNumero((long)traza.NumeroGuia);

            ADMotivoGuiaDC motivo = new ADMotivoGuiaDC()
            {
                IdMotivoGuia = ceCustodia.IdMotivoGuia,
                Descripcion = ceCustodia.DescripcionGuia,
                IdTercero = ceCustodia.IdTercero
            };


            long numerador = 0;
            using (TransactionScope transaccion = new TransactionScope())
            {
                numerador = Framework.Servidor.ParametrosFW.PAAdministrador.Instancia.ObtenerConsecutivo(PAEnumConsecutivos.Acta_disposicion_final);
                transaccion.Complete();
            }

            using (TransactionScope transaccion = new TransactionScope())
            {
                if (traza.IdNuevoEstadoGuia == (short)(ADEnumEstadoGuia.DisposicionFinal))
                {
                    if (motivo != null && motivo.IdMotivoGuia != 0)
                    {
                        traza.IdTrazaGuia = EstadosGuia.ValidarInsertarEstadoGuia(traza);
                        if (traza.IdTrazaGuia == 0)
                        {
                            string descripcionEstado = ", Estado actual " + ((ADEnumEstadoGuia)(traza.IdEstadoGuia)).ToString();

                            //no pudo realizar el cambio de estado
                            ControllerException excepcion = new ControllerException(COConstantesModulos.PRUEBAS_DE_ENTREGA,
                            LOIEnumTipoErrorLogisticaInversa.EX_CAMBIO_ESTADO_NO_VALIDO.ToString(),
                            LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.EX_CAMBIO_ESTADO_NO_VALIDO));
                            excepcion.Mensaje = excepcion.Mensaje + descripcionEstado;
                            throw new FaultException<ControllerException>(excepcion);
                        }

                        //ToDo Movimiento invetario salida custodia

                        trazaImpreso = new ADTrazaGuiaImpresoDC
                        {
                            IdTrazaGuia = traza.IdTrazaGuia,
                            NumeroImpreso = numerador,
                            TipoImpreso = ADEnumTipoImpreso.ActaDisposicionFinal,
                            Usuario = ControllerContext.Current.Usuario,
                            FechaGrabacion = DateTime.Now,
                            IdTercero = motivo.IdTercero
                        };
                        EstadosGuia.InsertarEstadoGuiaImpreso(trazaImpreso);
                        ADEstadoGuiaMotivoDC estadoMotivoGuia = new ADEstadoGuiaMotivoDC
                        {
                            IdTrazaGuia = traza.IdTrazaGuia,
                            Motivo = motivo,
                            Observaciones = string.Empty
                        };
                        EstadosGuia.InsertaEstadoGuiaMotivo(estadoMotivoGuia);

                        if (ceCustodia.Evidencias.Any())
                        {
                            PUCustodia custodia = new PUCustodia
                            {
                                AdjuntosPrecargue = ceCustodia.Evidencias,
                                MovimientoInventario = new PUMovimientoInventario
                                {
                                    NumeroGuia = (long)ceCustodia.IdTrazaGuia,
                                    IdBodega = ceCustodia.IdBodega                             
                                }                                
                            };
                            SubirImagenesEvidencia(custodia, (long)traza.IdTrazaGuia, false);
                        }

                        LIRepositorioTelemercadeo.Instancia.EliminarCustodiaPorDisposicionFinal((long)ceCustodia.IdTrazaGuia);

                    }
                }
                else
                {
                    traza.IdTrazaGuia = EstadosGuia.ValidarInsertarEstadoGuia(traza);
                    if (traza.IdTrazaGuia == 0)
                    {
                        string descripcionEstado = ", Estado actual " + ((ADEnumEstadoGuia)(traza.IdEstadoGuia)).ToString();

                        //no pudo realizar el cambio de estado
                        ControllerException excepcion = new ControllerException(COConstantesModulos.PRUEBAS_DE_ENTREGA,
                        LOIEnumTipoErrorLogisticaInversa.EX_CAMBIO_ESTADO_NO_VALIDO.ToString(),
                        LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.EX_CAMBIO_ESTADO_NO_VALIDO));
                        excepcion.Mensaje = excepcion.Mensaje + descripcionEstado;
                        throw new FaultException<ControllerException>(excepcion);
                    }

                }

                transaccion.Complete();
                return Convert.ToInt64(traza.IdTrazaGuia);
            }
        }

        ///// <summary>
        ///// Método que almacena el cambio de estado y el motivo de salida de custodia
        ///// </summary>
        ///// <param name="traza"></param>
        ///// <param name="motivo"></param>
        ///// <returns></returns>
        //public ADTrazaGuiaImpresoDC GuardarCambioEstado(ADTrazaGuia traza, ADMotivoGuiaDC motivo)
        //{
        //    ADTrazaGuiaImpresoDC trazaImpreso = new ADTrazaGuiaImpresoDC();

        //    using (TransactionScope transaccion = new TransactionScope())
        //    {
        //        long numerador = Framework.Servidor.ParametrosFW.PAAdministrador.Instancia.ObtenerConsecutivo(PAEnumConsecutivos.Acta_disposicion_final);
        //        if (traza.IdNuevoEstadoGuia == (short)(ADEnumEstadoGuia.DisposicionFinal))
        //        {
        //            if (motivo != null && motivo.IdMotivoGuia != 0)
        //            {
        //                traza.IdTrazaGuia = EstadosGuia.ValidarInsertarEstadoGuia(traza);
        //                if (traza.IdTrazaGuia == 0)
        //                {
        //                    string descripcionEstado = ", Estado actual " + ((ADEnumEstadoGuia)(traza.IdEstadoGuia)).ToString();

        //                    //no pudo realizar el cambio de estado
        //                    ControllerException excepcion = new ControllerException(COConstantesModulos.PRUEBAS_DE_ENTREGA,
        //                    LOIEnumTipoErrorLogisticaInversa.EX_CAMBIO_ESTADO_NO_VALIDO.ToString(),
        //                    LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.EX_CAMBIO_ESTADO_NO_VALIDO));
        //                    excepcion.Mensaje = excepcion.Mensaje + descripcionEstado;
        //                    throw new FaultException<ControllerException>(excepcion);
        //                }

        //                //ToDo Movimiento invetario salida custodia

        //                trazaImpreso = new ADTrazaGuiaImpresoDC
        //                {
        //                    IdTrazaGuia = traza.IdTrazaGuia,
        //                    NumeroImpreso = Framework.Servidor.ParametrosFW.PAAdministrador.Instancia.ObtenerConsecutivo(PAEnumConsecutivos.Acta_disposicion_final),
        //                    TipoImpreso = ADEnumTipoImpreso.ActaDisposicionFinal,
        //                    Usuario = ControllerContext.Current.Usuario,
        //                    FechaGrabacion = DateTime.Now,
        //                    IdTercero = motivo.IdTercero
        //                };
        //                EstadosGuia.InsertarEstadoGuiaImpreso(trazaImpreso);
        //                ADEstadoGuiaMotivoDC estadoMotivoGuia = new ADEstadoGuiaMotivoDC
        //                {
        //                    IdTrazaGuia = traza.IdTrazaGuia,
        //                    Motivo = motivo,
        //                    Observaciones = string.Empty
        //                };
        //                EstadosGuia.InsertaEstadoGuiaMotivo(estadoMotivoGuia);
        //            }
        //        }
        //        else
        //        {
        //            traza.IdTrazaGuia = EstadosGuia.ValidarInsertarEstadoGuia(traza);
        //            if (traza.IdTrazaGuia == 0)
        //            {
        //                string descripcionEstado = ", Estado actual " + ((ADEnumEstadoGuia)(traza.IdEstadoGuia)).ToString();

        //                //no pudo realizar el cambio de estado
        //                ControllerException excepcion = new ControllerException(COConstantesModulos.PRUEBAS_DE_ENTREGA,
        //                LOIEnumTipoErrorLogisticaInversa.EX_CAMBIO_ESTADO_NO_VALIDO.ToString(),
        //                LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.EX_CAMBIO_ESTADO_NO_VALIDO));
        //                excepcion.Mensaje = excepcion.Mensaje + descripcionEstado;
        //                throw new FaultException<ControllerException>(excepcion);
        //            }

        //        }

        //        transaccion.Complete();
        //        return trazaImpreso;
        //    }
        //}

            
        public int ObtenerNumeroDeEnviosEnUbicacion(int tipoUbicacion, int ubicacion)
        {
            return LIRepositorioTelemercadeo.Instancia.ObtenerNumeroDeEnviosEnUbicacion(tipoUbicacion, ubicacion);
        }


        #endregion Consultas

        #region API

        public void IngresoCustodia(PUCustodia Custodia)
        {
            ADGuia guia = new ADGuia();

            using (TransactionScope scope = new TransactionScope())
            {
                // VALIDACION ESTADOS  
                guia = fachadaMensajeria.ObtenerGuiaXNumeroGuia(Custodia.MovimientoInventario.NumeroGuia);
                if (!EstadosGuia.ValidarEstadoGuia(guia.NumeroGuia, (short)ADEnumEstadoGuia.PendienteIngresoaCustodia))
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.LOGISTICA_INVERSA,
                       LOIEnumTipoErrorLogisticaInversa.EX_ERROR_NO_ESTADO_PENDIENTE_INGRESO_CUSTODIA.ToString(),
                       LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.EX_ERROR_NO_ESTADO_PENDIENTE_INGRESO_CUSTODIA));
                    excepcion.Mensaje = excepcion.Mensaje;
                    throw new FaultException<ControllerException>(excepcion);
                }
                if (!fachadaCes.AdicionarCustodia(Custodia))
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.LOGISTICA_INVERSA,
                       LOIEnumTipoErrorLogisticaInversa.EX_ERROR_NO_INGRESADO_CUSTODIA.ToString(),
                       LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.EX_ERROR_NO_INGRESADO_CUSTODIA));
                    excepcion.Mensaje = excepcion.Mensaje;
                    throw new FaultException<ControllerException>(excepcion);
                }
                else
                {

                    ///Verifica si la guia tuvo afectacion por 7 dias y la reversa
                    if (guia.EsAlCobro && guia.EstaPagada)
                    {
                        fachadaCajas.InsertarDescuentoAlCobroDevuelto(guia.NumeroGuia, guia.IdCentroServicioDestino, guia.IdAdmision);
                    }

                    ADTrazaGuia estadoGuia = new ADTrazaGuia
                    {
                        Ciudad = Custodia.MovimientoInventario.Bodega.CiudadUbicacion.Nombre,
                        IdCiudad = Custodia.MovimientoInventario.Bodega.CiudadUbicacion.IdLocalidad,
                        IdAdmision = guia.IdAdmision,
                        IdEstadoGuia = (short)ADEnumEstadoGuia.IngresoABodega,
                        IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.Custodia,
                        Modulo = COConstantesModulos.PRUEBAS_DE_ENTREGA,
                        NumeroGuia = Custodia.MovimientoInventario.NumeroGuia,
                        Observaciones = string.Empty,
                        FechaGrabacion = DateTime.Now
                    };
                    estadoGuia.IdTrazaGuia = EstadosGuia.InsertaEstadoGuia(estadoGuia);

                    //GUARDAR ADJUNTOS                        
                    if (Custodia.AdjuntosPrecargue.Any())
                    {                        
                        SubirImagenesEvidencia(Custodia, estadoGuia.IdTrazaGuia.Value, true);
                    }
                }
                scope.Complete();
            }
        }

        public void SubirImagenesEvidencia(PUCustodia custodia, long idEstadoGuiaLog, bool esIngreso)
        {
            ftpPath = string.Concat(ftpEvidencias, DateTime.Now.Date.ToString("u").Substring(0, 10), esIngreso ? "_Ingreso": "_Salida");
            ftpCredential = new NetworkCredential(userFTPGloblal, passFTPGlobal);           
            int NumeroAdjunto = 1; 
            List<FTPObject> evidences = new List<FTPObject>();
            custodia.AdjuntosPrecargue.ForEach(adj =>
            {
                string rutaAdjuntos = Framework.Servidor.ParametrosFW.PAParametros.Instancia.ConsultarParametrosFramework("FolderImagenCustodia");
                string carpetaDestino = Path.Combine(string.Concat(rutaAdjuntos, "\\", esIngreso ? "Ingreso" : "Salida", "\\", DateTime.Now.ToString("s").Substring(0, 10), "\\", custodia.MovimientoInventario.NumeroGuia));
                if (!Directory.Exists(carpetaDestino))
                {
                    Directory.CreateDirectory(carpetaDestino);
                }
                adj = adj.Split(',')[esIngreso ? 0 : 1];
                byte[] bytebuffer = Convert.FromBase64String(adj);
                string ruta = string.Concat(carpetaDestino, "\\", DateTime.Now.ToString("s").Substring(0, 10), "_Evidencia", NumeroAdjunto, ".png");
                AdicionarEvidenciasCustodia(NumeroAdjunto, custodia.MovimientoInventario.IdBodega, ruta, true, idEstadoGuiaLog, esIngreso ? "INGRESO A CUSTODIA" : "SALIDA DE CUSTODIA", esIngreso ? (short)6 : (short)7);
                File.WriteAllBytes(ruta, bytebuffer);
                NumeroAdjunto++;
            });

            //foreach (var item in custodia.AdjuntosPrecargue)
            //{
            //    FTPObject evidence = new FTPObject();
            //    evidence.Path = string.Concat(custodia.MovimientoInventario.NumeroGuia, esIngreso ?"_Evidencia_Ingreso" : "_EvidenciaDisp_", NumeroAdjunto, ".png");
            //    evidence.Data = item.Split(',')[esIngreso ? 0 : 1];
            //    evidences.Add(evidence);
            //    AdicionarEvidenciasCustodia(NumeroAdjunto, custodia.MovimientoInventario.IdBodega, evidence.Path, true, idEstadoGuiaLog, esIngreso ? "INGRESO A CUSTODIA" : "SALIDA DE CUSTODIA", esIngreso ? (short)6 : (short)7);
            //    NumeroAdjunto++;
            //}
            //InsertImagesIntoFTPDirectory(evidences);
        }      

        //public void InsertImagesIntoFTPDirectory(List<FTPObject> data)
        //{            
        //    CreateFTPDirectory(ftpPath);
        //    foreach (FTPObject item in data)
        //    {
        //        FtpWebRequest reqFTP2;
        //        string path = ftpPath + "/" + item.Path.Split('_')[0];
        //        if (!Directory.Exists(path)) CreateFTPDirectory(path);
        //        path = path + "/" + item.Path;
        //        reqFTP2 = (FtpWebRequest)FtpWebRequest.Create(new Uri(path));
        //        reqFTP2.Credentials = ftpCredential;
        //        reqFTP2.KeepAlive = true;
        //        reqFTP2.UseBinary = true;
        //        reqFTP2.Method = WebRequestMethods.Ftp.UploadFile;
        //        byte[] img = Convert.FromBase64String(item.Data);
        //        reqFTP2.ContentLength = img.Length;
        //        Stream reqStream = reqFTP2.GetRequestStream();
        //        reqStream.Write(img, 0, img.Length);
        //        reqStream.Close();
        //    }
        //}

        //private void CreateFTPDirectory(string url)
        //{
        //    try
        //    {
        //        //create the directory
        //        FtpWebRequest requestDir = (FtpWebRequest)FtpWebRequest.Create(new Uri(url));
        //        requestDir.Method = WebRequestMethods.Ftp.MakeDirectory;
        //        requestDir.Credentials = new NetworkCredential(userFTPGloblal, passFTPGlobal);
        //        requestDir.UsePassive = true;
        //        requestDir.UseBinary = true;
        //        requestDir.KeepAlive = false;
        //        FtpWebResponse response = (FtpWebResponse)requestDir.GetResponse();
        //        Stream ftpStream = response.GetResponseStream();
        //        ftpStream.Close();
        //        response.Close();

        //    }
        //    catch (WebException ex)
        //    {
        //        FtpWebResponse response = (FtpWebResponse)ex.Response;
        //        if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
        //        {
        //            response.Close();                    
        //        }
        //        else
        //        {
        //            ControllerException excepcion = new ControllerException(COConstantesModulos.LOGISTICA_INVERSA,
        //                LOIEnumTipoErrorLogisticaInversa.EX_ERROR_FTP.ToString(),
        //                LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.EX_ERROR_FTP));
        //            excepcion.Mensaje = excepcion.Mensaje + " - " + ex;
        //            throw new FaultException<ControllerException>(excepcion);
        //        }
                
        //    }
        //}

        #region adicionar evidencias custodia

        public void AdicionarEvidenciasCustodia(long numeroEvidencia, long idBodega, string ruta, bool sincronizado, long idEstadoGuiaLog, string descripcionTipoEvidencia, Int16 idTipoEvidenciaDevolucion)
        {
            LIRepositorioTelemercadeo.Instancia.AdicionarEvidenciasCustodia(numeroEvidencia, idBodega, ruta, sincronizado, idEstadoGuiaLog, descripcionTipoEvidencia, idTipoEvidenciaDevolucion);
        }

        #endregion

        //public List<PUCustodia> ObtenerGuiasCustodia()
        //{
        //    long idCentroServicio = 0;
        //    return fachadaCes.ObtenerGuiasCustodia(idCentroServicio);
        //}

        #endregion

    }

    public partial class FTPObject
    {
        public string Data { get; set; }
        public string Path { get; set; }
    }
} 