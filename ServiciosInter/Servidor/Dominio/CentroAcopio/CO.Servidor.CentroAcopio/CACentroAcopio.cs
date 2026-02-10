using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Transactions;
using Framework.Servidor.Comun;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using Framework.Servidor.Excepciones;
using CO.Servidor.CentroAcopio.Datos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.CentroAcopio;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.AdmEstadosGuia;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using CO.Servidor.Dominio.Comun.Suministros;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using Framework.Servidor.Comun.Util;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.MensajeriaNN;
using CO.Servidor.CentroServicios;
using CO.Servidor.CentroServicios.Datos;
using CO.Servidor.Raps.Comun.Integraciones;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.RAPS.Reglas.Parametros;
using CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes;
using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;
using CO.Servidor.Raps.Comun;

namespace CO.Servidor.CentroAcopio
{
    public class CACentroAcopio : MarshalByRefObject
    {
        private IPUFachadaCentroServicios fachadaCentroServicio = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();
        private static readonly CACentroAcopio instancia = (CACentroAcopio)FabricaInterceptores.GetProxy(new CACentroAcopio(), COConstantesModulos.MODULO_CENTRO_ACOPIO);
        public static CACentroAcopio Instancia
        {
            get { return CACentroAcopio.instancia; }
        }

        private IADFachadaAdmisionesMensajeria fachadaMensajeria = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>();
        private IPUFachadaCentroServicios fachadaCentroServicios = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();
        private ISUFachadaSuministros fachadaSuministros = COFabricaDominio.Instancia.CrearInstancia<ISUFachadaSuministros>();


        public CAAsignacionGuiaDC Asignar_a_ReclameOficina(CAAsignacionGuiaDC AsignacionGuia)
        {

            // Validacion 0. Guia Exista
            ADGuia Guia;
            #region obtener Guia

            Guia = fachadaMensajeria.ObtenerGuiaXNumeroGuia(AsignacionGuia.NumeroGuia);

            #endregion


            using (TransactionScope transaccion = new TransactionScope())
            {

                // Validacion 1. la Guia debe ser Tipo de entrega (Reclame en Oficina)(2)
                if (Guia.IdTipoEntrega.Trim() != "2")
                {
                    AsignacionGuia.Respuesta = OUEnumValidacionDescargue.Error;
                    AsignacionGuia.Mensaje = "Guia debe ser Tipo de entrega (RECLAME EN OFICINA)";
                    return AsignacionGuia;
                }

                // Validacion 2. Guia Esta en Centro de Acopio del COL del Login
                if (Guia.TrazaGuiaEstado.IdEstadoGuia.Value != (short)ADEnumEstadoGuia.CentroAcopio)
                {
                    AsignacionGuia.Respuesta = OUEnumValidacionDescargue.Error;
                    AsignacionGuia.Mensaje = "La Guia debe estar en Centro de Acopio.";
                    return AsignacionGuia;
                }
                // Validacion 3. Guia Esta en Centro de Acopio del COL del Login
                if (Guia.TrazaGuiaEstado.IdCentroServicioEstado != AsignacionGuia.IdCentroServicioCOL)
                {
                    AsignacionGuia.Respuesta = OUEnumValidacionDescargue.Error;
                    AsignacionGuia.Mensaje = "La Guia se encuentra en un Centro de Acopio diferente.";
                    return AsignacionGuia;
                }


                //Movimiento de bodega SALIDA del COL
                AsignacionGuia.MovimientoInventario = new PUMovimientoInventario();
                AsignacionGuia.MovimientoInventario.TipoMovimiento = PUEnumTipoMovimientoInventario.Salida;
                AsignacionGuia.MovimientoInventario.IdCentroServicioOrigen = ControllerContext.Current.IdCentroServicio;
                AsignacionGuia.MovimientoInventario.Bodega = new PUCentroServiciosDC() { IdCentroServicio = AsignacionGuia.IdCentroServicioCOL };
                AsignacionGuia.MovimientoInventario.NumeroGuia = Guia.NumeroGuia;
                AsignacionGuia.MovimientoInventario.FechaGrabacion = DateTime.Now;
                AsignacionGuia.MovimientoInventario.FechaEstimadaIngreso = DateTime.Now;
                AsignacionGuia.MovimientoInventario.CreadoPor = ControllerContext.Current.Usuario;
                fachadaCentroServicios.AdicionarMovimientoInventario(AsignacionGuia.MovimientoInventario);


                //Movimiento de bodega ASIGNACION al PRO                                                  
                AsignacionGuia.MovimientoInventario = new PUMovimientoInventario();
                AsignacionGuia.MovimientoInventario.TipoMovimiento = PUEnumTipoMovimientoInventario.Asignacion;
                AsignacionGuia.MovimientoInventario.IdCentroServicioOrigen = ControllerContext.Current.IdCentroServicio;
                AsignacionGuia.MovimientoInventario.Bodega = new PUCentroServiciosDC() { IdCentroServicio = Guia.IdCentroServicioDestino };
                AsignacionGuia.MovimientoInventario.NumeroGuia = Guia.NumeroGuia;
                AsignacionGuia.MovimientoInventario.FechaGrabacion = DateTime.Now;
                AsignacionGuia.MovimientoInventario.FechaEstimadaIngreso = DateTime.Now;
                AsignacionGuia.MovimientoInventario.CreadoPor = ControllerContext.Current.Usuario;
                fachadaCentroServicios.AdicionarMovimientoInventario(AsignacionGuia.MovimientoInventario);

                AsignacionGuia.Respuesta = OUEnumValidacionDescargue.Exitosa;
                AsignacionGuia.Mensaje = "OK";              

                transaccion.Complete();
            }



            return AsignacionGuia;
        }

        public List<CAAsignacionGuiaDC> ConsultarGuias_EnCenAco_ParaREO(long IdCentroServicio)
        {
            return CARepositorioCentroAcopio.Instancia.ConsultarGuias_EnCenAco_ParaREO(IdCentroServicio);
        }
        public List<CAManifiestoREO> ConsultarManifiestosREO(DateTime? Fecha, long IdManifiesto)
        {
            Fecha = Fecha == null ? DateTime.Now.Date : Fecha;
            return CARepositorioCentroAcopio.Instancia.ConsultarManifiestosREO(Fecha.Value, IdManifiesto);
        }

        public MovimientoConsolidado MovimientoConsolidadoVigente(string numeroConsolidado, CACEnumTipoConsolidado tipoConsolidado)
        {
            return CARepositorioCentroAcopio.Instancia.MovimientoConsolidadoVigente(numeroConsolidado, tipoConsolidado);
        }

        public List<TipoConsolidado> ObtenerTipoConsolidado()
        {
            return CARepositorioCentroAcopio.Instancia.ObtenerTipoConsolidado();
        }

        public void InsertarMovimientoConsolidado(MovimientoConsolidado movimientoConsolidado)
        {
            using (TransactionScope transaction = new TransactionScope())
            {
                movimientoConsolidado.IdCentroServicioDestino = fachadaCentroServicios.ObtenerAgenciaLocalidad(movimientoConsolidado.IdLocalidadDestino).IdCentroServicio;
                movimientoConsolidado.IdMovimiento = CARepositorioCentroAcopio.Instancia.InsertarMovimientoConsolidado(movimientoConsolidado);
                if (movimientoConsolidado.Novedad != null && 
                    !string.IsNullOrEmpty(movimientoConsolidado.Novedad.Descripcion) && 
                    movimientoConsolidado.Novedad.IdTipoNovedad != 0)
                {
                    CARepositorioCentroAcopio.Instancia.InsertarNovedadConsolidado(movimientoConsolidado.Novedad, movimientoConsolidado.NumeroConsolidado, movimientoConsolidado.NumeroPrecinto);
                }
                transaction.Complete();
            }

        }

        public List<CAAsignacionGuiaDC> ConsultarManifiestoREO_Guias(long IdManifiesto)
        {
            return CARepositorioCentroAcopio.Instancia.ConsultarManifiestoREO_Guias(IdManifiesto);
        }

        public CAAsignacionGuiaDC Asignar_a_ConfirmacionesyDev(CAAsignacionGuiaDC AsignacionGuia)
        {

            //(Validaciones)

            // 0. Guia Exista
            ADGuia Guia;
            #region obtener Guia

            Guia = fachadaMensajeria.ObtenerGuiaXNumeroGuia(AsignacionGuia.NumeroGuia);

            #endregion


            // Guia Esta en Centro de Acopio del COL del Login
            if (Guia.TrazaGuiaEstado.IdEstadoGuia.Value != (short)ADEnumEstadoGuia.CentroAcopio)
            {
                AsignacionGuia.Respuesta = OUEnumValidacionDescargue.Error;
                AsignacionGuia.Mensaje = "La Guia debe estar en Centro de Acopio.";
                return AsignacionGuia;
            }
            // Guia Esta en Centro de Acopio del COL del Login
            if (Guia.TrazaGuiaEstado.IdCentroServicioEstado != AsignacionGuia.IdCentroServicioCOL)
            {
                AsignacionGuia.Respuesta = OUEnumValidacionDescargue.Error;
                AsignacionGuia.Mensaje = "La Guia se encuentra en un Centro de Acopio diferente.";
                return AsignacionGuia;
            }

            //// la Guia No debe ser Tipo de entrega (Reclame en Oficina)(2)
            //if (Guia.IdTipoEntrega.Trim() == "2")
            //{
            //    AsignacionGuia.Respuesta = OUEnumValidacionDescargue.Error;
            //    AsignacionGuia.Mensaje = "Guia es Tipo de entrega (RECLAME EN OFICINA)!";
            //    return AsignacionGuia;
            //}


            PUCentroServiciosDC CenServicioMov = fachadaCentroServicios.ObtenerCentroConfirmacionesDevoluciones(new PALocalidadDC() { IdLocalidad = AsignacionGuia.IdMunicipioCOL });
            if (CenServicioMov == null)
            {
                AsignacionGuia.Respuesta = OUEnumValidacionDescargue.Error;
                AsignacionGuia.Mensaje = "No se Encuentra la Bodega para Confirmaciones y Devoluciones.";
                return AsignacionGuia;
            }

            // Crear el Movimiento de bodega
            AsignacionGuia.MovimientoInventario = new PUMovimientoInventario();
            AsignacionGuia.MovimientoInventario.TipoMovimiento = PUEnumTipoMovimientoInventario.Asignacion;
            AsignacionGuia.MovimientoInventario.IdCentroServicioOrigen = ControllerContext.Current.IdCentroServicio;
            AsignacionGuia.MovimientoInventario.Bodega = CenServicioMov;
            AsignacionGuia.MovimientoInventario.NumeroGuia = Guia.NumeroGuia;
            AsignacionGuia.MovimientoInventario.FechaGrabacion = DateTime.Now;
            AsignacionGuia.MovimientoInventario.FechaEstimadaIngreso = DateTime.Now;
            AsignacionGuia.MovimientoInventario.CreadoPor = ControllerContext.Current.Usuario;
            fachadaCentroServicios.AdicionarMovimientoInventario(AsignacionGuia.MovimientoInventario);

            AsignacionGuia.Respuesta = OUEnumValidacionDescargue.Exitosa;
            AsignacionGuia.Mensaje = "OK";


            return AsignacionGuia;
        }

        public List<CAAsignacionGuiaDC> ConsultarGuias_EnConfirmacionesyDev(long IdCentroServicio, string pIdLocalidad)
        {
            // Primero obtenemos el dato de la Bodega de Confirmaciones y Devoluciones para hacer la consulta posteriormente
            PUCentroServiciosDC BodegaConfirDev = fachadaCentroServicios.ObtenerCentroConfirmacionesDevoluciones(new PALocalidadDC() { IdLocalidad = pIdLocalidad });

            return CARepositorioCentroAcopio.Instancia.ConsultarGuias_EnConfirmacionesyDev(IdCentroServicio, BodegaConfirDev.IdCentroServicio);
        }



        public CAAsignacionGuiaDC Asignar_a_Custodia(CAAsignacionGuiaDC AsignacionGuia)
        {

            //(Validaciones)

            // 0. Guia Exista
            ADGuia Guia;
            #region obtener Guia

            Guia = fachadaMensajeria.ObtenerGuiaXNumeroGuia(AsignacionGuia.NumeroGuia);

            #endregion


            // Guia Esta en Centro de Acopio del COL del Login
            if (Guia.TrazaGuiaEstado.IdEstadoGuia.Value != (short)ADEnumEstadoGuia.CentroAcopio)
            {
                AsignacionGuia.Respuesta = OUEnumValidacionDescargue.Error;
                AsignacionGuia.Mensaje = "La Guia debe estar en Centro de Acopio.";
                return AsignacionGuia;
            }
            // Guia Esta en Centro de Acopio del COL del Login
            if (Guia.TrazaGuiaEstado.IdCentroServicioEstado != AsignacionGuia.IdCentroServicioCOL)
            {
                AsignacionGuia.Respuesta = OUEnumValidacionDescargue.Error;
                AsignacionGuia.Mensaje = "La Guia se encuentra en un Centro de Acopio diferente.";
                return AsignacionGuia;
            }

            //// la Guia No debe ser Tipo de entrega (Reclame en Oficina)(2)
            //if (Guia.IdTipoEntrega.Trim() == "2")
            //{
            //    AsignacionGuia.Respuesta = OUEnumValidacionDescargue.Error;
            //    AsignacionGuia.Mensaje = "Guia es Tipo de entrega (RECLAME EN OFICINA)!";
            //    return AsignacionGuia;
            //}

            PUCentroServiciosDC BodegaCustodia = fachadaCentroServicios.ObtenerBodegaCustodia();
            if (BodegaCustodia == null)
            {
                AsignacionGuia.Respuesta = OUEnumValidacionDescargue.Error;
                AsignacionGuia.Mensaje = "No se Encuentro la Bodega para Indemnizaciones y Custodia.";
                return AsignacionGuia;
            }

            // Crear el Movimiento de bodega
            AsignacionGuia.MovimientoInventario = new PUMovimientoInventario();
            AsignacionGuia.MovimientoInventario.TipoMovimiento = PUEnumTipoMovimientoInventario.Asignacion;
            AsignacionGuia.MovimientoInventario.IdCentroServicioOrigen = ControllerContext.Current.IdCentroServicio;
            AsignacionGuia.MovimientoInventario.Bodega = BodegaCustodia;
            AsignacionGuia.MovimientoInventario.NumeroGuia = Guia.NumeroGuia;
            AsignacionGuia.MovimientoInventario.FechaGrabacion = DateTime.Now;
            AsignacionGuia.MovimientoInventario.FechaEstimadaIngreso = DateTime.Now;
            AsignacionGuia.MovimientoInventario.CreadoPor = ControllerContext.Current.Usuario;
            fachadaCentroServicios.AdicionarMovimientoInventario(AsignacionGuia.MovimientoInventario);

            AsignacionGuia.Respuesta = OUEnumValidacionDescargue.Exitosa;
            AsignacionGuia.Mensaje = "OK";



            return AsignacionGuia;
        }

        public List<CAAsignacionGuiaDC> ConsultarGuias_EnCustodia(long IdCentroServicio)
        {
            // Primero obtenemos el dato de la Bodega de Confirmaciones y Devoluciones para hacer la consulta posteriormente
            PUCentroServiciosDC BodegaCustodia = fachadaCentroServicios.ObtenerBodegaCustodia();

            return CARepositorioCentroAcopio.Instancia.ConsultarGuias_EnCustodia(IdCentroServicio, BodegaCustodia.IdCentroServicio);
        }


        public void CrearManifiesto_REO(long IdCSManif, long IdCSDesti, long IdVehiculo, long IdMensajero)
        {


            using (TransactionScope transaccion = new TransactionScope())
            {
                // 1. Crea el Manifiesto
                long newIdMAnifiesto = CARepositorioCentroAcopio.Instancia.CrearManifiesto_REO(IdCSManif, IdCSDesti, IdVehiculo, IdMensajero, ControllerContext.Current.Usuario);
                CARepositorioCentroAcopio.Instancia.CreaDetalleManifiesto_REO(newIdMAnifiesto, IdCSManif, IdCSDesti);


                // 2. Afecta el Estado de las Guias del Manifiesto (a SalidadeBodega)
                List<CAAsignacionGuiaDC> listaManifiesto_Guia = CARepositorioCentroAcopio.Instancia.ConsultarManifiestoREO_Guias(newIdMAnifiesto);
                PUCentroServiciosDC Ces = fachadaCentroServicios.ObtenerCentroServicio(ControllerContext.Current.IdCentroServicio);
                foreach (CAAsignacionGuiaDC iteGuia in listaManifiesto_Guia)
                {
                    ADTrazaGuia estadoGuia = new ADTrazaGuia
                    {
                        Ciudad = Ces.CiudadUbicacion.Nombre,
                        IdCiudad = Ces.CiudadUbicacion.IdLocalidad,
                        IdAdmision = iteGuia.IdAdmision,
                        IdEstadoGuia = (short)(EstadosGuia.ObtenerUltimoEstado(iteGuia.IdAdmision)),
                        IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.TransitoUrbano,
                        Modulo = COConstantesModulos.MODULO_CENTRO_ACOPIO,
                        NumeroGuia = iteGuia.NumeroGuia,
                        Observaciones = string.Empty,
                        FechaGrabacion = DateTime.Now
                    };
                    estadoGuia.IdTrazaGuia = EstadosGuia.ValidarInsertarEstadoGuia(estadoGuia);
                    if (estadoGuia.IdTrazaGuia == 0)
                    {
                        string descripcionEstado = ", Estado actual " + ((ADEnumEstadoGuia)(estadoGuia.IdEstadoGuia)).ToString();
                        ControllerException excepcion = new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL
                                        , "0"
                                        , "Cambio de Estado no válido");
                        excepcion.Mensaje = excepcion.Mensaje + descripcionEstado;
                        throw new FaultException<ControllerException>(excepcion);
                    }

                }

                transaccion.Complete();
            }

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="numeroguia"></param>
        /// <param name="IdCSDestino"></param>
        /// <returns></returns>
        public CAAsignacionGuiaDC CambiarTipoEntrega_REO(long numeroguia, long IdCSDestino)
        {
            CAAsignacionGuiaDC respuesta = new CAAsignacionGuiaDC();
            ADGuia Guia = new ADGuia();
            #region obtener Guia

            Guia = fachadaMensajeria.ObtenerGuiaXNumeroGuia(numeroguia);

            #endregion


            //1- La Guia ya se encuentra en Reclame en Oficina
            if (Guia.IdTipoEntrega.Trim() == "2")
            {
                if (IdCSDestino != Guia.IdCentroServicioDestino)
                {
                    CARepositorioCentroAcopio.Instancia.CambiarTipoEntrega_REO(numeroguia, IdCSDestino);
                    respuesta.Respuesta = OUEnumValidacionDescargue.Exitosa;
                    return respuesta;
                }
                else
                {
                    respuesta.Respuesta = OUEnumValidacionDescargue.Error;
                    respuesta.Mensaje = "La Guia ya es Reclame en Oficina";
                    return respuesta;
                }
            }

            //2- La Guia tiene intentos de Entrega (Intento de Entrega (7))
            // Se quita el 22/03/2016 por solicitud de Oscar debido a que no permitia asignar a REO por haber retirado por fuera de Zona
            //if (EstadosGuia.ObtenerEstadosGuia(Guia.NumeroGuia).FirstOrDefault(es => es.IdEstadoGuia == (Int16)ADEnumEstadoGuia.EnReparto) != null)
            //{
            //    respuesta.Respuesta = OUEnumValidacionDescargue.Error;
            //    respuesta.Mensaje = "La Guia ya estuvo en reparto y no se puede cambiar el tipo de entrega";
            //    return respuesta;
            //}

            ADTrazaGuia traza = EstadosGuia.ObtenerTrazaUltimoEstadoXNumGuia(Guia.NumeroGuia);
            if (traza.IdEstadoGuia == (short)ADEnumEstadoGuia.CentroAcopio && traza.IdCentroServicioEstado == ControllerContext.Current.IdCentroServicio)
            {
                CARepositorioCentroAcopio.Instancia.CambiarTipoEntrega_REO(numeroguia, IdCSDestino);
                respuesta.Respuesta = OUEnumValidacionDescargue.Exitosa;
                return respuesta;
            }
            else
            {
                respuesta.Respuesta = OUEnumValidacionDescargue.Error;
                respuesta.Mensaje = "La Guia no se encuentra en centro de acopio o esta en otro centro de servicio";
                return respuesta;
            }

        }

        /// <summary>
        /// Cambia el Tipo de Entrega desde el Modulo de Telemercadeo en Controller
        /// </summary>
        /// <param name="numeroguia"></param>
        /// <param name="IdCSDestino"></param>
        /// <returns></returns>
        public void CambiarTipoEntregaTelemercadeo_REO(long numeroguia, long IdCSDestino)
        {
            CARepositorioCentroAcopio.Instancia.CambiarTipoEntrega_REO(numeroguia, IdCSDestino);
        }



        /// <summary>
        /// Valida si la guia ya está asignada.
        /// </summary>
        /// <param name="numeroguia"></param>
        /// <param name="COL"></param>
        /// <returns></returns>
        public bool validarAsignacionMovimientoInventario(long numeroGuia, long idCSAsigna)
        {
            return CARepositorioCentroAcopio.Instancia.ObtieneIdAsignacionMovInventario(numeroGuia, idCSAsigna);
        }

        #region Envios NN
        /// <summary>
        /// Inserta Envio NN .
        /// </summary>
        /// <param name="numeroguia"></param>
        /// <param name="COL"></param>
        /// <returns></returns>
        public long InsertarEnvioNN(ADEnvioNN envioNN)
        {
            SUNumeradorPrefijo numeroSuministro = new SUNumeradorPrefijo();

            using (TransactionScope transaccion = new TransactionScope())
            {
                // Se obtiene el número de guía
                numeroSuministro = fachadaSuministros.ObtenerNumeroPrefijoValor(SUEnumSuministro.ADMISION_NN);
                transaccion.Complete();
            }

            using (TransactionScope transaccion = new TransactionScope())
            {

                envioNN.NumeroMensajeriaNN = numeroSuministro.ValorActual;
                CARepositorioCentroAcopio.Instancia.InsertarEnvioNN(envioNN);

                envioNN.Imagenes.ForEach(i =>
                {

                    string rutaImagenes = Framework.Servidor.ParametrosFW.PAParametros.Instancia.ConsultarParametrosFramework("FolderImagenNN");
                    string carpetaDestino = Path.Combine(rutaImagenes + "\\" + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year);
                    if (!Directory.Exists(carpetaDestino))
                    {
                        Directory.CreateDirectory(carpetaDestino);
                    }
                    byte[] bytebuffer = Convert.FromBase64String(i);
                    MemoryStream memoryStream = new MemoryStream(bytebuffer);
                    var image = Image.FromStream(memoryStream);
                    ImageCodecInfo jpgEncoder = UtilidadesFW.GetEncoder(ImageFormat.Jpeg);
                    string ruta = carpetaDestino + "\\" + Guid.NewGuid() + ".jpg";
                    System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                    EncoderParameters myEncoderParameters = new EncoderParameters(1);
                    EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 180L);
                    myEncoderParameters.Param[0] = myEncoderParameter;
                    image.Save(ruta, jpgEncoder, myEncoderParameters);
                    CARepositorioCentroAcopio.Instancia.InsertarImagenAdmisionNN(ruta, envioNN.NumeroMensajeriaNN);

                });

                transaccion.Complete();
            }
            return envioNN.NumeroMensajeriaNN;
        }
        /// <summary>
        /// Obtiene Envio NN .
        /// </summary>
        /// <param name="numeroguia"></param>
        /// <param name="COL"></param>
        /// <returns></returns>
        public List<ADEnvioNN> ObtieneEnvioNN(AdEnvioNNFiltro envioNNFiltro)
        {
            return CARepositorioCentroAcopio.Instancia.ObtieneEnvioNN(envioNNFiltro);
        }
        /// <summary>
        /// Asigna guia a envío NN
        /// </summary>
        /// <param name="numeroEnvioNN"></param>
        /// <param name="idGuia"></param>
        /// <param name="creadoPor"></param>
        public bool AsignacionGuiaAEnvioNN(long numeroEnvioNN, long idGuia, string creadoPor)
        {
            using (TransactionScope trans = new TransactionScope())
            {               
                bool respuesta = CARepositorioCentroAcopio.Instancia.AsignacionGuiaAEnvioNN(numeroEnvioNN, idGuia, creadoPor);
                ADGuia guia = fachadaMensajeria.ObtenerGuiaXNumeroGuia(idGuia);
               
                if (respuesta && guia.EstadoGuia == ADEnumEstadoGuia.Admitida && guia.TipoCliente == ADEnumTipoCliente.INT)
                {
                    ADTrazaGuia trazaGuia = EstadosGuia.ObtenerTrazaUltimoEstadoXNumGuia(idGuia);
                    trazaGuia.IdNuevoEstadoGuia = (Int16)ADEnumEstadoGuia.PendienteIngresoaCustodia;
                    EstadosGuia.InsertaEstadoGuia(trazaGuia);
                    PUMovimientoInventario movimiento = new PUMovimientoInventario
                    {
                        IdCentroServicioOrigen = trazaGuia.IdCentroServicioEstado,
                        Bodega = fachadaCentroServicios.ObtenerBodegaCustodia(),
                        TipoMovimiento = PUEnumTipoMovimientoInventario.Asignacion,
                        NumeroGuia = idGuia,
                        FechaEstimadaIngreso = DateTime.Now,
                        FechaGrabacion = DateTime.Now
                    };
                    fachadaCentroServicios.AdicionarMovimientoInventario(movimiento);
                }
                trans.Complete();
            }
            return true;
        }

        /// <summary>
        /// Asigna guia a envío NN
        /// </summary>
        /// <param name="numeroEnvioNN"></param>
        /// <param name="idGuia"></param>
        /// <param name="creadoPor"></param>
        public bool AsignacionGuiaAEnvioNNV7(long numeroEnvioNN, long idGuia, string creadoPor)
        {
            using (TransactionScope trans = new TransactionScope())
            {
                bool respuesta = CARepositorioCentroAcopio.Instancia.AsignacionGuiaAEnvioNN(numeroEnvioNN, idGuia, creadoPor);
                ADGuia guia = fachadaMensajeria.ObtenerGuiaXNumeroGuia(idGuia);

                if (respuesta && guia.EstadoGuia == ADEnumEstadoGuia.Admitida && guia.TipoCliente == ADEnumTipoCliente.INT)
                {
                    ADTrazaGuia trazaGuia = EstadosGuia.ObtenerTrazaUltimoEstadoXNumGuia(idGuia);
                    trazaGuia.IdNuevoEstadoGuia = (Int16)ADEnumEstadoGuia.PendienteIngresoaCustodia;
                    EstadosGuia.InsertaEstadoGuia(trazaGuia);
                    PUMovimientoInventario movimiento = new PUMovimientoInventario
                    {
                        IdCentroServicioOrigen = trazaGuia.IdCentroServicioEstado,
                        Bodega = fachadaCentroServicios.ObtenerBodegaCustodia(),
                        TipoMovimiento = PUEnumTipoMovimientoInventario.Asignacion,
                        NumeroGuia = idGuia,
                        FechaEstimadaIngreso = DateTime.Now,
                        FechaGrabacion = DateTime.Now
                    };
                    fachadaCentroServicios.AdicionarMovimientoInventario(movimiento);

                }
                IntegrarRapsAsociacionGuiaNN(guia, numeroEnvioNN);
                trans.Complete();
            }
            return true;
        }

        private void IntegrarRapsAsociacionGuiaNN(ADGuia guia, long numeroEnvioNN)
        {
            var centroServicio = PURepositorio.Instancia.ObtenerCentroServicio(guia.IdCentroServicioOrigen);
            PUAgenciaDeRacolDC colResponsable = fachadaCentroServicio.ObteneColPropietarioBodega(guia.IdCentroServicioOrigen);
            List<LIParametrizacionIntegracionRAPSDC> lstParametros = new List<LIParametrizacionIntegracionRAPSDC>();
            RADatosFallaDC datosFalla = null;
            RAFallaMapper ma = null;
            RAParametrosSolicitudAcumulativaDC parametrosSolicitudAcumulativa = new RAParametrosSolicitudAcumulativaDC();
            ma = new RAFallaMapper();
            datosFalla = ma.MapperDatosFallaCentroAcopio(centroServicio, Servicios.ContratoDatos.Raps.Configuracion.RAEnumSistemaOrigen.CONTROLLER.GetHashCode(), guia.NumeroGuia, numeroEnvioNN);

            if (centroServicio.Tipo == ConstantesFramework.TIPO_CENTRO_SERVICIO_AGENCIA)
            {
                parametrosSolicitudAcumulativa = AdministradorReglaEstadoRAPS.Instancia.IntegrarRapFallas(datosFalla, RAEnumOrigenRaps.Agencias, CoEnumTipoNovedadRaps.ASOCIACIÓN_GUIAS_NN_AGE_AUTO.GetHashCode());
            }
            else if (centroServicio.Tipo == ConstantesFramework.TIPO_CENTRO_SERVICIO_PUNTO)
            {
                parametrosSolicitudAcumulativa = AdministradorReglaEstadoRAPS.Instancia.IntegrarRapFallas(datosFalla, RAEnumOrigenRaps.Puntos, CoEnumTipoNovedadRaps.ASOCIACIÓN_GUIAS_NN_PTO_AUTO.GetHashCode());
            }

            /*****************************************CREA SOLICITUD ACUMULATIVA********************************************************/
            if (!parametrosSolicitudAcumulativa.EstaEnviado)
            {
                if (parametrosSolicitudAcumulativa.TipoNovedad != CoEnumTipoNovedadRaps.Pordefecto && parametrosSolicitudAcumulativa.Parametrosparametrizacion.Count > 0)
                {
                    RAIntegracionesRaps.Instancia.CrearSolicitudAcumulativaRaps((CoEnumTipoNovedadRaps)parametrosSolicitudAcumulativa.TipoNovedad.GetHashCode(), parametrosSolicitudAcumulativa.Parametrosparametrizacion, datosFalla.IdCiudad.Substring(0, 5), ControllerContext.Current == null ? "MotorRaps" : ControllerContext.Current.Usuario, datosFalla.IdSistema);
                }
            }
            else
            {
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_RAPS, RAEnumTipoErrorClientes.EX_INSERTAR_SOLICITUD.ToString(), RAMensajesRaps.CargarMensaje(RAEnumTipoErrorClientes.EX_FALLA_YA_REGISTRADA_MISMO_RESPONSABLE)));
            }
        }

        /// <summary>
        /// obtiene la ruta de las imagenes relacionadas al envío
        /// </summary>
        /// <param name="numeroEnvioNN"></param>
        /// <returns></returns>
        public List<RutasImagenesEnvioNN> ObtieneRutaImagenesEnvioNN(long numeroEnvioNN)
        {
            return CARepositorioCentroAcopio.Instancia.ObtieneRutaImagenesEnvioNN(numeroEnvioNN);
        }

        public List<ClasificacionEnvioNN> ObtieneClasificacionEnvioNN()
        {
            return CARepositorioCentroAcopio.Instancia.ObtieneClasificacionEnvioNN();
        }
        #endregion


        #region Ingreso a Centro de Acopio Bodegas

        /// <summary>
        /// Obtener Reenvíos enviados desde LOI a Centro de Acopio
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <param name="idCentroServicioOrigen"></param>
        /// <returns></returns>
        public List<CAAsignacionGuiaDC> ObtenerReenviosBodegas_CAC(long idCentroServicioOrigen, ADEnumEstadoGuia idEstado)
        {
            return CARepositorioCentroAcopio.Instancia.ObtenerReenviosBodegas_CAC(idCentroServicioOrigen, idEstado);
        }

        /// <summary>
        /// Ingresar Guia a Centro de Acopio desde Logistica Inversa
        /// </summary>
        /// <param name="movInventario"></param>
        public void IngresaraCentrodeAcopioValidandoEstado(PUMovimientoInventario movInventario, OUNovedadIngresoDC novedad, ADEnumEstadoGuia Estado)
        {

            // Validacion 0. Guia Exista
            ADGuia Guia;
            Guia = fachadaMensajeria.ObtenerGuiaXNumeroGuia(movInventario.NumeroGuia);

            // 1. Validar que el Estado Actual de la Guia sea REENVIO o Admitida
            if (Guia.TrazaGuiaEstado.IdEstadoGuia.Value == (short)Estado || Guia.TrazaGuiaEstado.IdEstadoGuia.Value == (short)ADEnumEstadoGuia.PendienteIngresoaCustodia)
            {

                PUCentroServiciosDC Ces = fachadaCentroServicios.ObtenerCentroServicio(ControllerContext.Current.IdCentroServicio);
                using (TransactionScope transaccion = new TransactionScope())
                {

                    // 2. Insertar en Movimiento INV (como INGRESADA al COL)
                    fachadaCentroServicios.AdicionarMovimientoInventario(movInventario);

                    // 3. Cambiar el Estado de la GUIA a (En Centro de Acopio)

                    ADTrazaGuia estadoGuia = new ADTrazaGuia();
                    {
                        estadoGuia.Ciudad = Ces.CiudadUbicacion.Nombre;
                        estadoGuia.IdCiudad = Ces.CiudadUbicacion.IdLocalidad;
                        estadoGuia.IdAdmision = Guia.IdAdmision;
                        estadoGuia.IdEstadoGuia = (short)(EstadosGuia.ObtenerUltimoEstado(Guia.IdAdmision));
                        estadoGuia.IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.CentroAcopio;
                        estadoGuia.Modulo = COConstantesModulos.MODULO_CENTRO_ACOPIO;
                        estadoGuia.NumeroGuia = Guia.NumeroGuia;
                        estadoGuia.Observaciones = string.Empty;
                        estadoGuia.FechaGrabacion = DateTime.Now;
                    };

                    estadoGuia.IdTrazaGuia = EstadosGuia.ValidarInsertarEstadoGuia(estadoGuia);
                    if (estadoGuia.IdTrazaGuia == 0)
                    {
                        string descripcionEstado = ", Estado actual " + ((ADEnumEstadoGuia)(estadoGuia.IdEstadoGuia)).ToString();
                        ControllerException excepcion = new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL
                                        , "0"
                                        , "Cambio de Estado no válido");
                        excepcion.Mensaje = excepcion.Mensaje + descripcionEstado;
                        throw new FaultException<ControllerException>(excepcion);
                    }



                    long IdIngresoGuia = CARepositorioCentroAcopio.Instancia.GuardarIngresoGuia(estadoGuia);

                    if (novedad.IdNovedad != 0)
                        CARepositorioCentroAcopio.Instancia.GuardarNovedadGuiaIngresada(novedad, IdIngresoGuia);


                    transaccion.Complete();
                }

            }
            else
            {
                ControllerException excepcion = new ControllerException(COConstantesModulos.MODULO_CENTRO_ACOPIO
                             , "0"
                             , "La Guia debe estar en estado " + Estado + " ó Pendiente Ingreso a Custodia.");
                excepcion.Mensaje = excepcion.Mensaje;
                throw new FaultException<ControllerException>(excepcion);
            }



        }

        /// <summary>
        /// Obtiene las Guias que se Eliminan de la planilla desde Centro de Acopio por Envio Fuera de Zona
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns></returns>
        public List<CAAsignacionGuiaDC> ObtenerGuiasEliminadasPlanillaCentroAcopio(string usuario)
        {
            return CARepositorioCentroAcopio.Instancia.ObtenerGuiasEliminadasPlanillaCentroAcopio(usuario);
        }

        #endregion

    }
}
