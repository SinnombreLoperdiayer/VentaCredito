using CO.Servidor.Adminisiones.Mensajeria.Comun;
using CO.Servidor.Adminisiones.Mensajeria.Datos.Mapper;
using CO.Servidor.Adminisiones.Mensajeria.Datos.Modelo;
using CO.Servidor.Dominio.Comun.AdmEstadosGuia;
using CO.Servidor.Dominio.Comun.Util;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria.AdmMasiva;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.MensajeriaNN;
using CO.Servidor.Servicios.ContratoDatos.Area;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using CO.Servidor.Servicios.ContratoDatos.ControlCuentas;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.Servicios.ContratoDatos.OperacionNacional;
using CO.Servidor.Servicios.ContratoDatos.Recogidas;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Precios;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.Util;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace CO.Servidor.Adminisiones.Mensajeria.Datos
{
    /// <summary>
    /// Contiene las operaciones de acceso a datos de adminisiones de mensajería
    /// </summary>
    public class ADRepositorio
    {
        #region Atributos

        private string NombreModelo = "EntidadesAdmisionesMensajeria";
        private string filePath = string.Empty;
        private CultureInfo cultura = new CultureInfo("es-CO");
        private string conexionStringArchivo = ConfigurationManager.ConnectionStrings["ControllerArchivo"].ConnectionString;
        private string conexionStringController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;
        private string conexionStringAuditoria = ConfigurationManager.ConnectionStrings["ControllerExcepciones"].ConnectionString;
        private string conexionStringPosController = ConfigurationManager.ConnectionStrings["PorController"].ConnectionString;
        private string CadCnxSispostalController = ConfigurationManager.ConnectionStrings["Sispostal"].ConnectionString;


        #endregion Atributos

        #region Instancia singleton de la clase

        private static readonly ADRepositorio instancia = new ADRepositorio();

       
        public static ADRepositorio Instancia
        {
            get
            {
                return instancia;
            }
        }        

        #endregion Instancia singleton de la clase

        #region Consultas

        /// <summary>
        /// Consulta el último estado válido para el cliente final con el fin de responder la llamada via IVR
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public string ConsultarEstadoGuiaIVR(long numeroGuia)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                paObtenerUltimoEstadoIVR_MEN_Result ultimoEstadoIVR = contexto.paObtenerUltimoEstadoIVR_MEN(numeroGuia).FirstOrDefault();

                if (ultimoEstadoIVR != null)
                {
                    if (ultimoEstadoIVR.IdEstadoGuia_LogGestion != 0)
                    {
                        if (ultimoEstadoIVR.IdEstadoGuia_LogGestion == (short)ADEnumEstadoGuia.Entregada
                            || ultimoEstadoIVR.IdEstadoGuia_LogGestion == (short)ADEnumEstadoGuia.DevolucionRatificada)
                        {
                            return ultimoEstadoIVR.DescripcionEstado_LogGestion;
                        }
                    }

                    // Si la Fecha Estimada de entrega ya se cumplio..  no retorna nada (0)
                    if (ultimoEstadoIVR.IdEstadoGuia_Log == (short)ADEnumEstadoGuia.TransitoRegional || ultimoEstadoIVR.IdEstadoGuia_Log == (short)ADEnumEstadoGuia.TransitoNacional)
                        if (DateTime.Now.Date > ultimoEstadoIVR.ADM_FechaEstimadaEntrega.Date)
                            return "0";

                    return ultimoEstadoIVR.DescripcionEstado_Log;
                }

                return "0";
            }
        }

        public DatosAfectacionCaja ObtenerAfectacionCaja(long numeroGuia)
        {
            var datosAfectacion = new DatosAfectacionCaja();

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerAfectacionesCaja_CAJ", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroGuia", numeroGuia);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();                
                if(reader.HasRows)
                {
                    datosAfectacion.IdConceptoCaja = Convert.ToInt32(reader["RTD_IdConceptoCaja"]);
                    datosAfectacion.IdCentroServiciosVenta = Convert.ToInt64(reader["RTC_IdCentroServiciosVenta"]);
                    datosAfectacion.FechaGrabacion = Convert.ToDateTime(reader["RTD_FechaGrabacion"]);
                    datosAfectacion.Ingreso = Convert.ToInt32(reader["Ingreso"]);
                    datosAfectacion.Egreso = Convert.ToInt32(reader["Egreso"]);
                    datosAfectacion.CreadoPor = Convert.ToString(reader["RTD_CreadoPor"]);
                    datosAfectacion.DescripcionFormaPago = Convert.ToString(reader["RVF_DescripcionFormaPago"]);
                    datosAfectacion.NombreCentroServiciosVenta = Convert.ToString(reader["RTC_NombreCentroServiciosVenta"]);
                    datosAfectacion.NombreConcepto = Convert.ToString(reader["RTD_NombreConcepto"]);
                    datosAfectacion.NumeroComprobante = Convert.ToString(reader["RTD_NumeroComprobante"]);
                    datosAfectacion.Observacion = Convert.ToString(reader["RTD_Observacion"]);
                    datosAfectacion.Usuario = Convert.ToString(reader["Usuario"]);
                }
            }

            return datosAfectacion;
        }

        /// <summary>
        /// Consulta Si Existe un Centro de Servicio
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public bool ExisteCentroServicio(long IdCenSvc)
        {

            try
            {
                using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
                {
                    long swCuantos = 0;

                    sqlConn.Open();
                    SqlCommand cmd = new SqlCommand(@"SELECT @Cuantos = COUNT(1) FROM CentroServicios_PUA WHERE CES_IdCentroServicios = @IdCenSvc", sqlConn);

                    SqlParameter paramOut = new SqlParameter("@Cuantos", SqlDbType.BigInt);
                    paramOut.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(paramOut);

                    cmd.Parameters.AddWithValue("@IdCenSvc", IdCenSvc);

                    cmd.ExecuteNonQuery();
                    swCuantos = Convert.ToInt64(paramOut.Value);

                    sqlConn.Close();

                    if (swCuantos > 0)
                        return true;
                    else
                        return false;
                }
            }
            catch
            {
                return false;
            }

        }


        /// <summary>
        /// Obtiene las formas de pago de una guia
        /// </summary>
        /// <param name="idGuia"></param>
        /// <returns></returns>
        public List<ADGuiaFormaPago> ObtenerFormasPagoGuia(long idGuia)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var formasPago = contexto.FormasPagoGuia_VMEN.Where(o => o.AGF_IdAdminisionMensajeria == idGuia);
                if (formasPago != null)
                {
                    return formasPago.ToList().ConvertAll(o =>
                     new ADGuiaFormaPago()
                     {
                         IdFormaPago = o.FOP_IdFormaPago,
                         Descripcion = o.FOP_Descripcion,
                         Valor = o.AGF_Valor
                     });
                }
                else
                    return new List<ADGuiaFormaPago>();
            }
        }

        /// <summary>
        /// Obtiene el estado de la guía
        /// </summary>
        /// <param name="estadoGuia"></param>
        /// <returns></returns>
        public ADEstadoGuia ObtenerEstadoGuia(ADEnumEstadoGuia estadoGuia)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                EstadoGuia_MEN estado = contexto.EstadoGuia_MEN.SingleOrDefault(e => e.ESG_IdEstadoGuia == (int)estadoGuia);
                if (estado != null)
                {
                    return new ADEstadoGuia()
                    {
                        Id = estado.ESG_IdEstadoGuia,
                        Descripcion = estado.ESG_Descripcion,
                        EsVisible = estado.ESG_EsVisible
                    };
                }
                return null;
            }
        }

        /// <summary>
        /// Obtiene la lista de motivos por los cuales no se hizo uso de la bolsa de seguridad
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ADMotivoNoUsoBolsaSeguridad> ObtenerMotivosNoUsoBolsaSeguridad()
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.MotivoNoUsoBolsaSegurid_MEN
                  .OrderBy(motivo => motivo.MAB_Descripcion)
                  .ToList()
                  .ConvertAll<ADMotivoNoUsoBolsaSeguridad>(motivo => new ADMotivoNoUsoBolsaSeguridad
                        {
                            Id = motivo.MAB_IdMotivoNoUsoBolsaSegurida,
                            Descripcion = motivo.MAB_Descripcion
                        });
            }
        }

        /// <summary>
        /// Retorna lista de valores adicionales agregados a una admisión
        /// </summary>
        /// <param name="IdAdmision"></param>
        /// <returns></returns>
        public List<TAValorAdicional> ObtenerValoresAdicionales(long IdAdmision)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paObtenerValAdicioAdm_MEN(IdAdmision)
                  .ToList()
                  .ConvertAll(v =>
                    new TAValorAdicional
                    {
                        IdTipoValorAdicional = v.AVA_IdTipoValorAdicional,
                        Descripcion = v.AVA_Descripcion,
                        PrecioValorAdicional = v.AVA_Valor
                    });
            }
        }

        /// <summary>
        /// Retorna el archivo digitalizado de una guía
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ADArchivoAlmacenGuia ObtenerArchivoAlmacenGuia(long numeroGuia)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                paObtenerArchivoAlmacenAdm_MEN guia = contexto.paObtenerArchivoAlmacenAdm_MEN(numeroGuia, (short)ADEnumEstadoGuia.Entregada, (short)ADEnumEstadoGuia.DevolucionRatificada).FirstOrDefault();
                if (guia != null && guia.ADM_IdAdminisionMensajeria != 0)
                {
                    return new ADArchivoAlmacenGuia
                    {
                        Archivo = Convert.ToBase64String(guia.ARG_Adjunto),
                        Guia = new ADGuia()
                          {
                              IdAdmision = guia.ADM_IdAdminisionMensajeria,
                              NumeroGuia = guia.ADM_NumeroGuia,
                              PrefijoNumeroGuia = guia.ADM_PrefijoNumeroGuia,
                              MotivoEntrega = guia.MOTIVO_GUIA ?? string.Empty
                          }
                    };
                }
                else if (guia != null && guia.ADM_IdAdminisionMensajeria == 0)
                {
                    if (guia.ESTADO == (short)ADEnumEstadoGuia.Entregada || guia.ESTADO == (short)ADEnumEstadoGuia.DevolucionRatificada)
                    {
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MENSAJERIA, ADEnumTipoErrorMensajeria.EX_GUIA_NO_DIGITALIZADA.ToString(), ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.EX_GUIA_NO_DIGITALIZADA)));
                    }
                    else if (guia.ESTADO.HasValue)
                    {
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MENSAJERIA, ADEnumTipoErrorMensajeria.EX_ERROR_ENVIO_NO_ESTA_ENTREGADO_O_DEVOLUCION.ToString(), ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.EX_ERROR_ENVIO_NO_ESTA_ENTREGADO_O_DEVOLUCION)));
                    }
                    else
                    {
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MENSAJERIA, ADEnumTipoErrorMensajeria.EX_NUMERO_GUIA_NO_EXISTE.ToString(), ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.EX_NUMERO_GUIA_NO_EXISTE)));
                    }
                }
                else
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MENSAJERIA, ADEnumTipoErrorMensajeria.EX_NUMERO_GUIA_NO_EXISTE.ToString(), ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.EX_NUMERO_GUIA_NO_EXISTE)));
                }
            }
        }

        /// <summary>
        /// Retorna la información de una guía dada su forma de pago, en un rango de fechas de admisión, que pertenezcan al cliente dado y al RACOL dado, que
        /// sean del servicio de notificaciones, tipo de envío certificación, que estén descargadas como entrega correcta, que no tengan capturado los datos de
        /// recibido y estén digitalizadas
        /// </summary>
        /// <param name="idFormaPago">Forma de pago</param>
        /// <param name="fechaInicio">Fecha Inicial</param>
        /// <param name="fechaFin">Fecha Final</param>
        /// <param name="idCliente">Id del Cliente</param>
        /// <param name="idRacol">Id del Racol</param>
        /// <returns></returns>
        public List<ADGuia> ObtenerGuiasParaCapturaAutomatica(short idFormaPago, DateTime fechaInicio, DateTime fechaFin, int? idCliente, long idRacol, int idServicioNotificaciones)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                DateTime _fechaInicio = fechaInicio.Date;
                DateTime _fechaFin = fechaFin.Date.AddDays(1);
                List<paObtenerGuiasParaCapturaAutomatica_MEN1_Result> guias = contexto.paObtenerGuiasParaCapturaAutomatica_MEN1(idFormaPago, _fechaInicio, _fechaFin, idCliente, idRacol, (short)ADEnumEstadoGuia.Entregada, idServicioNotificaciones, ADConstantes.ID_TIPO_ENVIO_NOTIFICACION).ToList();
                return guias.ConvertAll(g => new ADGuia
                {
                    IdAdmision = g.ADM_IdAdminisionMensajeria.HasValue ? g.ADM_IdAdminisionMensajeria.Value : 0,
                    NumeroGuia = g.ADM_NumeroGuia.HasValue ? g.ADM_NumeroGuia.Value : 0,
                    FechaAdmision = g.ADM_FechaAdmision.HasValue ? g.ADM_FechaAdmision.Value : DateTime.Now,
                    MotivoEntrega = g.MOG_Descripcion
                });
            }
        }

        /// <summary>
        /// Retorna información de guía que para ser recibida en forma manual en la parte de notificaciones.
        /// La guía debe estar en estado "Devolución" o "Entrega" y la prueba de entrega o de devolución
        /// correspondiente debe estar digitalizada en la aplicación
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ADGuia ObtenerGuiaParaRecibirManualNotificaciones(long numeroGuia)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                AD_paAdmisionCapturaManual guia = contexto.paObtenerAdmCapturaNotificacionManual_MEN(numeroGuia, (short)ADEnumEstadoGuia.Entregada, (short)ADEnumEstadoGuia.DevolucionRatificada).FirstOrDefault();
                if (guia != null && guia.ADM_IdAdminisionMensajeria != 0)
                {
                    return new ADGuia
                    {
                        Destinatario = new CLClienteContadoDC
                        {
                            TipoId = guia.ADM_IdTipoIdentificacionDestinatario,
                            Identificacion = guia.ADM_IdDestinatario,
                            Nombre = guia.ADM_NombreDestinatario,
                            Telefono = guia.ADM_NombreDestinatario,
                            Direccion = guia.ADM_DireccionDestinatario
                        },
                        FechaAdmision = guia.ADM_FechaAdmision,
                        IdAdmision = guia.ADM_IdAdminisionMensajeria,
                        IdCiudadDestino = guia.ADM_IdCiudadDestino,
                        IdCiudadOrigen = guia.ADM_IdCiudadOrigen,
                        IdServicio = guia.ADM_IdServicio,
                        MotivoEntrega = guia.MOTIVO_GUIA ?? string.Empty,
                        NombreCiudadDestino = guia.ADM_NombreCiudadDestino,
                        NombreCiudadOrigen = guia.ADM_NombreCiudadOrigen,
                        NumeroGuia = guia.ADM_NumeroGuia,
                        NombreServicio = guia.ADM_NombreServicio,
                        NumeroBolsaSeguridad = guia.ADM_NumeroBolsaSeguridad,
                        PesoLiqMasa = guia.ADM_PesoLiqMasa,
                        PesoLiqVolumetrico = guia.ADM_PesoLiqVolumetrico,
                        Peso = guia.ADM_Peso,
                        Remitente = new CLClienteContadoDC
                        {
                            TipoId = guia.ADM_IdTipoIdentificacionRemitente,
                            Identificacion = guia.ADM_IdRemitente,
                            Nombre = guia.ADM_NombreRemitente,
                            Telefono = guia.ADM_NombreRemitente,
                            Direccion = guia.ADM_DireccionRemitente
                        },
                        NombreTipoEnvio = guia.ADM_NombreTipoEnvio,
                        IdTipoEntrega = guia.ADM_IdTipoEnvio.ToString(),
                        DiceContener = guia.ADM_DiceContener,
                        ValorDeclarado = guia.ADM_ValorDeclarado,
                        Observaciones = guia.ADM_Observaciones,
                        ValorAdmision = guia.ADM_ValorAdmision,
                        ValorPrimaSeguro = guia.ADM_ValorPrimaSeguro,
                        ValorAdicionales = guia.ADM_ValorAdicionales,
                        ValorTotal = guia.ADM_ValorTotal,
                        ValoresAdicionales = this.ObtenerValoresAdicionales(guia.ADM_IdAdminisionMensajeria)
                    };
                }
                else if (guia != null && guia.ADM_IdAdminisionMensajeria == 0)
                {
                    if (guia.ESTADO == (short)ADEnumEstadoGuia.Entregada || guia.ESTADO == (short)ADEnumEstadoGuia.DevolucionRatificada)
                    {
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MENSAJERIA, ADEnumTipoErrorMensajeria.EX_GUIA_NO_DIGITALIZADA.ToString(), ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.EX_GUIA_NO_DIGITALIZADA)));
                    }
                    else
                    {
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MENSAJERIA, ADEnumTipoErrorMensajeria.EX_ERROR_ENVIO_NO_ESTA_ENTREGADO_O_DEVOLUCION.ToString(), ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.EX_ERROR_ENVIO_NO_ESTA_ENTREGADO_O_DEVOLUCION)));
                    }
                }
                else
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MENSAJERIA, ADEnumTipoErrorMensajeria.EX_NUMERO_GUIA_NO_EXISTE.ToString(), ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.EX_NUMERO_GUIA_NO_EXISTE)));
                }
            }
        }

        /// <summary>
        /// Retorna lista de objetos de prohibida circulación
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ADObjetoProhibidaCirculacion> ObtenerObjetosProhibidaCirculacion()
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ObjetoCirculacionProhibida_MEN
                  .OrderBy(objeto => objeto.OCP_Descripcion)
                  .ToList()
                  .ConvertAll<ADObjetoProhibidaCirculacion>(objeto => new ADObjetoProhibidaCirculacion
                      {
                          Id = objeto.OCP_IdObjetoCiruculaProhibida,
                          Descripcion = objeto.OCP_Descripcion
                      });
            }
        }

        /// <summary>
        /// Retorna la lista de tipos de entrega que pueden aplicar sobre el envío
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ADTipoEntrega> ObtenerTiposEntrega()
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TipoEntrega_MEN
                  .OrderBy(tipo => tipo.TIE_Descripcion)
                  .ToList()
                  .ConvertAll(tipo => new ADTipoEntrega
                    {
                        Id = tipo.TIE_IdTipoEntrega.Trim(),
                        Descripcion = tipo.TIE_Descripcion
                    });
            }
        }

        /// <summary>
        /// Consulta los parámetros de configuración de admisiones
        /// </summary>
        /// <returns></returns>
        public ADParametrosAdmisiones ObtenerParametrosAdmisiones()
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ADParametrosAdmisiones parametros = new ADParametrosAdmisiones();
                parametros.PorcentajePrimaSeguro = ObtenerParametro<decimal>(contexto, "PorcPrimaSeguro");
                parametros.PesoPorDefecto = ObtenerParametro<decimal>(contexto, "PesoPorDefecto");
                parametros.TopeMaxValorDeclarado = ObtenerParametro<decimal>(contexto, "TopeMaxValorDeclarad");
                parametros.UnidadMedidaPorDefecto = ObtenerParametro<string>(contexto, "UnidadMedidaPorDefec");
                parametros.TipoMonedaModificable = ObtenerParametro<bool>(contexto, "TipMonedaModificable");
                parametros.TipoMonedaPorDefecto = ObtenerParametro<string>(contexto, "TipoMonedaPorDefecto");
                parametros.PesoMinimoRotulo = ObtenerParametro<int>(contexto, "PesoMinimoRotulo");
                parametros.PorcentajeRecargo = ObtenerParametro<double>(contexto, "PorcentajeRecargo");
                parametros.NumeroPiezasAplicaRotulo = ObtenerParametro<int>(contexto, "NumPiezasApliRotulo");

                // TODO ID: Nuevo parametro para Tope minimo de Vlr Declarado unicamente en tipo de Servicio RAPICARGA
                parametros.TopeMinVlrDeclRapiCarga = ObtenerParametro<decimal>(contexto, "TopeMinVlrDeclRapiCa");

                parametros.ImagenPublicidadGuia = ObtenerParametro<string>(contexto, "ImagenPublicidadGuia");
                parametros.ValorReimpresionCertificacion = ObtenerParametro<string>(contexto, "ValorReimpCertifEnt");
                this.PesoMinimoRotulo = parametros.PesoMinimoRotulo;
                return parametros;
            }
        }

        public int PesoMinimoRotulo { get; set; }

        /// <summary>
        /// Retona un bool que indica si se debe hacer integración con el sistema "Mensajero"
        /// </summary>
        /// <returns></returns>
        public bool ObtenerParametroIntegraConMensajero()
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return ObtenerParametro<bool>(contexto, ConstantesFramework.CACHE_INTEGRA_CON_MENSAJERO);
            }
        }

        /// <summary>
        /// Retona un bool que indica si se debe hacer integración con el sistema "Mensajero"
        /// </summary>
        /// <returns></returns>
        public int ObtenerParametroMaximoRegistros()
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return ObtenerParametro<int>(contexto, ConstantesFramework.CACHE_MAXIMO_REGISTROS);
            }
        }

        /// <summary>
        /// Obtiene el valor de un parámetro de admisiones
        /// </summary>
        /// <typeparam name="T">Tipo del dato que debe retornar</typeparam>
        /// <param name="contexto">Contexto de la base de datos</param>
        /// <param name="parametro">Parámetro a buscar en la tabla</param>
        /// <returns></returns>
        private T ObtenerParametro<T>(EntidadesAdmisionesMensajeria contexto, string parametro)
        {
            ParametrosAdmisiones_MEN porcentaje = contexto.ParametrosAdmisiones_MEN.SingleOrDefault(p => p.PAM_IdParametro == parametro);
            if (porcentaje != null)
            {
                if (typeof(T) == typeof(string))
                {
                    return (T)Convert.ChangeType(porcentaje.PAM_ValorParametro, typeof(T));
                }
                else if (typeof(T) == typeof(decimal))
                {
                    decimal dValor = 0;
                    if (decimal.TryParse(porcentaje.PAM_ValorParametro, out dValor))
                    {
                        return (T)Convert.ChangeType(dValor, typeof(T));
                    }
                    else
                    {
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MENSAJERIA, ADEnumTipoErrorMensajeria.EX_PARAMETRIZACION_INVALIDA.ToString(), ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.EX_PARAMETRIZACION_INVALIDA)));
                    }
                }
                else if (typeof(T) == typeof(int))
                {
                    int iValor = 0;
                    if (int.TryParse(porcentaje.PAM_ValorParametro, out iValor))
                    {
                        return (T)Convert.ChangeType(iValor, typeof(T));
                    }
                    else
                    {
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MENSAJERIA, ADEnumTipoErrorMensajeria.EX_PARAMETRIZACION_INVALIDA.ToString(), ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.EX_PARAMETRIZACION_INVALIDA)));
                    }
                }
                else if (typeof(T) == typeof(bool))
                {
                    bool bValor = false;
                    if (bool.TryParse(porcentaje.PAM_ValorParametro, out bValor))
                    {
                        return (T)Convert.ChangeType(bValor, typeof(T));
                    }
                    else
                    {
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MENSAJERIA, ADEnumTipoErrorMensajeria.EX_PARAMETRIZACION_INVALIDA.ToString(), ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.EX_PARAMETRIZACION_INVALIDA)));
                    }
                }
                else if (typeof(T) == typeof(double))
                {
                    double dValor = 0;
                    if (double.TryParse(porcentaje.PAM_ValorParametro, out dValor))
                    {
                        return (T)Convert.ChangeType(dValor, typeof(T));
                    }
                    else
                    {
                        return (T)Convert.ChangeType(dValor, typeof(T));
                    }
                }
                else
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MENSAJERIA, ADEnumTipoErrorMensajeria.EX_PARAMETRIZACION_INVALIDA.ToString(), ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.EX_PARAMETRIZACION_INVALIDA)));
                }
            }
            else
            {
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MENSAJERIA, ADEnumTipoErrorMensajeria.EX_PARAMETRIZACION_INVALIDA.ToString(), ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.EX_PARAMETRIZACION_INVALIDA)));
            }
        }

        /// <summary>
        /// Obtiene el ultimo estado y ubicacin de la admision mensajeria
        /// </summary>
        ///<param name="idNumeroGuia">Numero de la guia</param>
        public ADGuiaUltEstadoDC ObtenerMensajeriaUltimoEstado(long idNumeroGuia)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                paObtenerAdmiMenUltEstado_MEN_Result mensajeria = contexto.paObtenerAdmiMenUltEstado_MEN(idNumeroGuia).FirstOrDefault();
                if (mensajeria == null)
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MENSAJERIA, ADEnumTipoErrorMensajeria.EX_NUMERO_GUIA_NO_EXISTE.ToString(), ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.EX_NUMERO_GUIA_NO_EXISTE)));
                }

                //if (mensajeria.EGT_IdEstadoGuia == (short)ADEnumEstadoGuia.Entregada)
                //{
                //    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MENSAJERIA, ADEnumTipoErrorMensajeria.EX_GUIA_ENTREGADA.ToString(), ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.EX_GUIA_ENTREGADA)));
                //}

                //if (mensajeria.EGT_IdEstadoGuia == (short)ADEnumEstadoGuia.Custodia)
                //{
                //    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MENSAJERIA, ADEnumTipoErrorMensajeria.EX_GUIA_CUSTODIA.ToString(), ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.EX_GUIA_CUSTODIA)));
                //}

                return new ADGuiaUltEstadoDC()
                {
                    Guia = new ADGuia()
                    {
                        IdMensajero = mensajeria.ADM_IdMensajero,
                        NombreMensajero = mensajeria.NombreMensajero,
                        Entregada = mensajeria.ADM_EstaEntregada,
                        IdAdmision = mensajeria.ADM_IdAdminisionMensajeria,
                        NumeroGuia = mensajeria.ADM_NumeroGuia,
                        IdCentroServicioOrigen = mensajeria.ADM_IdCentroServicioOrigen,
                        NombreCentroServicioOrigen = mensajeria.ADM_NombreCentroServicioOrigen,
                        IdCentroServicioDestino = mensajeria.ADM_IdCentroServicioDestino,
                        NombreCentroServicioDestino = mensajeria.ADM_NombreCentroServicioDestino,
                        IdCiudadOrigen = mensajeria.ADM_IdCiudadOrigen,
                        DiceContener = mensajeria.ADM_DiceContener,
                        Observaciones = mensajeria.ADM_Observaciones,
                        DescripcionTipoEntrega = mensajeria.ADM_DescripcionTipoEntrega,
                        NombreTipoEnvio = mensajeria.ADM_NombreTipoEnvio,
                        NumeroPieza = mensajeria.ADM_NumeroPieza,
                        TotalPiezas = mensajeria.ADM_TotalPiezas,
                        NumeroBolsaSeguridad = mensajeria.ADM_NumeroBolsaSeguridad,
                        Ancho = mensajeria.ADM_Ancho,
                        Largo = mensajeria.ADM_Largo,
                        Alto = mensajeria.ADM_Alto,
                        PesoLiqVolumetrico = mensajeria.ADM_PesoLiqVolumetrico,
                        FormasPagoDescripcion = mensajeria.FormaPago,
                        EsAutomatico = mensajeria.ADM_EsAutomatico,
                        NombreCiudadOrigen = mensajeria.ADM_NombreCiudadOrigen,
                        IdCiudadDestino = mensajeria.ADM_IdCiudadDestino,
                        NombreCiudadDestino = mensajeria.ADM_NombreCiudadDestino,
                        ValorServicio = mensajeria.ADM_ValorAdmision,
                        ValorAdmision = mensajeria.ADM_ValorAdmision,
                        ValorPrimaSeguro = mensajeria.ADM_ValorPrimaSeguro,
                        ValorTotal = mensajeria.ADM_ValorTotal,
                        
                        ValorDeclarado = mensajeria.ADM_ValorDeclarado,
                        ValorTotalImpuestos = mensajeria.ADM_ValorTotalImpuestos,
                        IdServicio = mensajeria.ADM_IdServicio,
                        NombreServicio = mensajeria.ADM_NombreServicio,
                        FechaAdmision = mensajeria.ADM_FechaAdmision,
                        EsPesoVolumetrico = mensajeria.ADM_EsPesoVolumetrico,
                        EsAlCobro = mensajeria.ADM_EsAlCobro,
                        TipoCliente = (ADEnumTipoCliente)Enum.Parse(typeof(ADEnumTipoCliente), mensajeria.ADM_TipoCliente, true),
                        Peso = mensajeria.ADM_Peso,
                        Remitente = new CLClienteContadoDC()
                        {
                            Identificacion = mensajeria.ADM_IdRemitente,
                            Nombre = mensajeria.ADM_NombreRemitente,
                            Telefono = mensajeria.ADM_TelefonoRemitente,
                            Direccion = mensajeria.ADM_DireccionRemitente
                        },
                        Destinatario = new CLClienteContadoDC()
                        {
                            Identificacion = mensajeria.ADM_IdDestinatario,
                            Nombre = mensajeria.ADM_NombreDestinatario,
                            Telefono = mensajeria.ADM_TelefonoDestinatario,
                            Direccion = mensajeria.ADM_DireccionDestinatario
                        },
                    },
                    TrazaGuia = new ADTrazaGuia()
                    {
                        Ciudad = mensajeria.EGT_NombreLocalidad,
                        IdCiudad = mensajeria.EGT_IdLocalidad,
                        IdEstadoGuia = mensajeria.EGT_IdEstadoGuia
                    }
                };
            }
        }

        /// <summary>
        /// Obtener informacion de la guia de mensajeria y las formas de pago
        /// </summary>
        /// <returns></returns>
        public ADGuiaUltEstadoDC ObtenerMensajeriaFormaPago(long idAdmision)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                paObtenerFormasPagoAdmMen mensajeria
                  = contexto.paObtenerFormasPagoAdmMen_MEN(idAdmision).FirstOrDefault();
                if (mensajeria == null)
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MENSAJERIA, ADEnumTipoErrorMensajeria.EX_NUMERO_GUIA_NO_EXISTE.ToString(), ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.EX_NUMERO_GUIA_NO_EXISTE)));
                }

                return
                    new ADGuiaUltEstadoDC
                    {
                        Guia = new ADGuia()
                        {
                            IdAdmision = mensajeria.ADM_IdAdminisionMensajeria,
                            NumeroGuia = mensajeria.ADM_NumeroGuia,
                            Entregada = mensajeria.ADM_EstaEntregada,
                            EstaPagada = mensajeria.ADM_EstaPagada,
                            IdCentroServicioOrigen = mensajeria.ADM_IdCentroServicioOrigen,
                            NombreCentroServicioOrigen = mensajeria.ADM_NombreCentroServicioOrigen,
                            ValorAdicionales = mensajeria.ADM_ValorAdicionales,
                            IdCentroServicioDestino = mensajeria.ADM_IdCentroServicioDestino,
                            NombreCentroServicioDestino = mensajeria.ADM_NombreCentroServicioDestino,
                            ValorPrimaSeguro = mensajeria.ADM_ValorPrimaSeguro,
                            ValorTotalImpuestos = mensajeria.ADM_ValorTotalImpuestos,
                            ValorTotalRetenciones = mensajeria.ADM_ValorTotalRetenciones,
                            IdCiudadOrigen = mensajeria.ADM_IdCiudadOrigen,
                            NombreCiudadOrigen = mensajeria.ADM_NombreCiudadOrigen,
                            IdCiudadDestino = mensajeria.ADM_IdCiudadDestino,
                            ValorAdmision = mensajeria.ADM_ValorAdmision,
                            ValorServicio = mensajeria.ADM_ValorAdmision,
                            NombreCiudadDestino = mensajeria.ADM_NombreCiudadDestino,
                            ValorTotal = mensajeria.ADM_ValorTotal,
                            ValorDeclarado = mensajeria.ADM_ValorDeclarado,
                            IdServicio = mensajeria.ADM_IdServicio,
                            FechaAdmision = mensajeria.ADM_FechaAdmision,
                            Peso = mensajeria.ADM_Peso,
                            EsPesoVolumetrico = mensajeria.ADM_EsPesoVolumetrico,
                            EsAlCobro = mensajeria.ADM_EsAlCobro,
                            EsAutomatico = mensajeria.ADM_EsAutomatico,
                            TipoCliente = (ADEnumTipoCliente)Enum.Parse(typeof(ADEnumTipoCliente), mensajeria.ADM_TipoCliente, true),
                        },
                        FormasPago = new List<ADGuiaFormaPago>()
                        {
                         new ADGuiaFormaPago()
                          {
                              IdFormaPago = mensajeria.AGF_IdFormaPago,
                              Descripcion = mensajeria.FOP_Descripcion,
                              Valor = mensajeria.AGF_Valor
                          }
                        }
                    };
            }
        }

        /// <summary>
        /// Consultar el contrato de n cliente Convenio
        /// </summary>
        /// <param name="tipoCliente"></param>
        /// <param name="idAdmisionMensajeria"></param>
        /// <returns></returns>
        public int ObtenerContratoClienteConvenio(ADEnumTipoCliente tipoCliente, long idAdmisionMensajeria)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                if (tipoCliente == ADEnumTipoCliente.CCO)//cliente convenio convenio
                {
                    MensajeriaConvenioConvenio_MEN convenioConvenio = contexto.MensajeriaConvenioConvenio_MEN.FirstOrDefault(cc => cc.MCC_IdAdminisionMensajeria == idAdmisionMensajeria);
                    if (convenioConvenio == null)
                    {
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MENSAJERIA, ADEnumTipoErrorMensajeria.EX_CLIENTE_CONVENIO_NO_ESTA.ToString(), ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.EX_CLIENTE_CONVENIO_NO_ESTA)));
                    }
                    return convenioConvenio.MCC_IdContratoConvenioRemite;
                }
                else if (tipoCliente == ADEnumTipoCliente.CPE)//cliente convenio peaton
                {
                    MensajeriaConvenioPeaton_MEN convenioPeaton = contexto.MensajeriaConvenioPeaton_MEN.FirstOrDefault(cp => cp.MCP_IdAdminisionMensajeria == idAdmisionMensajeria);
                    if (convenioPeaton == null)
                    {
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MENSAJERIA, ADEnumTipoErrorMensajeria.EX_CLIENTE_CONVENIO_NO_ESTA.ToString(), ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.EX_CLIENTE_CONVENIO_NO_ESTA)));
                    }
                    return convenioPeaton.MCP_IdContratoConvenioRemite;
                }
                else
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MENSAJERIA, ADEnumTipoErrorMensajeria.EX_CLIENTE_CONVENIO_NO_ESTA.ToString(), ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.EX_CLIENTE_CONVENIO_NO_ESTA)));
                }
            }
        }

        /// <summary>
        /// Obteniene una guia ya sea que haya sido modificada o no
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ADGuia ObtenerInformacionGuiaPorNumero(long numeroGuia)
        {
            ADGuia guiaBuscada = new ADGuia();
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand("paObtenerGuiaPorMenOHist_MEN", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("@NumeroGuia", numeroGuia);
                SqlDataReader lector = comm.ExecuteReader();
                while (lector.Read())
                {
                    guiaBuscada = new ADGuia()
                    {
                        //Entregada = Convert.ToBoolean(lector["ADM_EstaEntregada"]),
                        IdAdmision = Convert.ToInt64(lector["ADM_IdAdminisionMensajeria"]),
                        NumeroGuia = Convert.ToInt64(lector["ADM_NumeroGuia"]),
                        IdCiudadOrigen = Convert.ToString(lector["ADM_IdCiudadOrigen"]),
                        NombreCiudadOrigen = Convert.ToString(lector["ADM_NombreCiudadOrigen"]),
                        IdCiudadDestino = Convert.ToString(lector["ADM_IdCiudadDestino"]),
                        NombreCiudadDestino = Convert.ToString(lector["ADM_NombreCiudadDestino"]),
                        ValorTotal = Convert.ToDecimal(lector["ADM_ValorTotal"]),
                        ValorDeclarado = Convert.ToDecimal(lector["ADM_ValorDeclarado"]),
                        IdServicio = Convert.ToInt32(lector["ADM_IdServicio"]),
                        EsAlCobro = Convert.ToBoolean(lector["ADM_EsAlCobro"]),
                        EstaPagada = Convert.ToBoolean(lector["ADM_EstaPagada"]),
                        FechaAdmision = Convert.ToDateTime(lector["ADM_FechaAdmision"]),
                        Peso = Convert.ToDecimal(lector["ADM_Peso"]),
                        EsPesoVolumetrico = Convert.ToBoolean(lector["ADM_EsPesoVolumetrico"]),
                        ValorServicio = Convert.ToDecimal(lector["ADM_ValorAdmision"]),
                        IdCentroServicioOrigen = Convert.ToInt64(lector["ADM_IdCentroServicioOrigen"]),
                        NombreCentroServicioOrigen = Convert.ToString(lector["ADM_NombreCentroServicioOrigen"]),
                        IdCentroServicioDestino = Convert.ToInt64(lector["ADM_IdCentroServicioDestino"]),
                        NombreCentroServicioDestino = Convert.ToString(lector["ADM_NombreCentroServicioDestino"]),
                        NombreServicio = Convert.ToString(lector["ADM_NombreServicio"]),
                        Observaciones = Convert.ToString(lector["ADM_Observaciones"]),
                        FormasPago = new List<ADGuiaFormaPago> 
                        {
                           new ADGuiaFormaPago{
                               IdFormaPago = Convert.ToInt16(lector["FOP_IdFormaPago"]),
                               Descripcion = Convert.ToString(lector["FOP_Descripcion"])
                           }
                        },
                        //contexto.FormasPagoGuia_VMEN.Where(fp => fp.AGF_IdAdminisionMensajeria == admision.ADM_IdAdminisionMensajeria).ToList().ConvertAll(f => new ADGuiaFormaPago { Descripcion = f.FOP_Descripcion, IdFormaPago = f.AGF_IdFormaPago, Valor = f.AGF_Valor }),
                        IdTipoEnvio = Convert.ToInt16(lector["ADM_IdTipoEnvio"]),
                        NombreTipoEnvio = Convert.ToString(lector["ADM_NombreTipoEnvio"]),
                        TotalPiezas = Convert.ToInt16(lector["ADM_TotalPiezas"]),
                        PrefijoNumeroGuia = Convert.ToString(lector["ADM_PrefijoNumeroGuia"]),
                        TelefonoDestinatario = Convert.ToString(lector["ADM_TelefonoDestinatario"]),
                        DireccionDestinatario = Convert.ToString(lector["ADM_DireccionDestinatario"]),
                        TipoCliente = (ADEnumTipoCliente)Enum.Parse(typeof(ADEnumTipoCliente), Convert.ToString(lector["ADM_TipoCliente"]), true),
                        Remitente = new CLClienteContadoDC()
                        {
                            Nombre = Convert.ToString(lector["ADM_NombreRemitente"]),
                            Telefono = Convert.ToString(lector["ADM_TelefonoRemitente"]),
                            Identificacion = Convert.ToString(lector["ADM_IdRemitente"]),
                            Direccion = Convert.ToString(lector["ADM_DireccionRemitente"]),
                            TipoId = Convert.ToString(lector["ADM_IdTipoIdentificacionRemitente"]),
                            Email = Convert.ToString(lector["ADM_EmailRemitente"])
                        },
                        Destinatario = new CLClienteContadoDC()
                        {
                            Nombre = Convert.ToString(lector["ADM_NombreDestinatario"]),
                            Direccion = Convert.ToString(lector["ADM_DireccionDestinatario"]),
                            TipoId = Convert.ToString(lector["ADM_IdTipoIdentificacionDestinatario"]),
                            Telefono = Convert.ToString(lector["ADM_TelefonoDestinatario"]),
                            Identificacion = Convert.ToString(lector["ADM_IdDestinatario"]),
                            Email = Convert.ToString(lector["ADM_EmailDestinatario"])
                        },
                        DiceContener = Convert.ToString(lector["ADM_DiceContener"]),
                        Supervision = Convert.ToBoolean(lector["ADM_EsSupervisada"]),
                        FechaSupervision = Convert.ToDateTime(lector["ADM_FechaSupervision"]),
                        IdMensajero = Convert.ToInt64(lector["ADM_IdMensajero"]),
                        NombreMensajero = Convert.ToString(lector["ADM_NombreMensajero"]),
                        DiasDeEntrega = Convert.ToInt16(lector["ADM_DiasDeEntrega"]),
                        DescripcionTipoEntrega = Convert.ToString(lector["ADM_DescripcionTipoEntrega"]),
                        EsRecomendado = Convert.ToBoolean(lector["ADM_EsRecomendado"]),
                        Alto = Convert.ToDecimal(lector["ADM_Alto"]),
                        Ancho = Convert.ToDecimal(lector["ADM_Ancho"]),
                        Largo = Convert.ToDecimal(lector["ADM_Largo"]),
                        DigitoVerificacion = Convert.ToString(lector["ADM_DigitoVerificacion"]),
                        FechaEstimadaEntrega = Convert.ToDateTime(lector["ADM_FechaEstimadaEntrega"]),
                        NumeroPieza = Convert.ToInt16(lector["ADM_NumeroPieza"]),
                        IdUnidadMedida = Convert.ToString(lector["ADM_IdUnidadMedida"]),
                        IdUnidadNegocio = Convert.ToString(lector["ADM_IdUnidadNegocio"]),
                        ValorTotalImpuestos = Convert.ToDecimal(lector["ADM_ValorTotalImpuestos"]),
                        ValorTotalRetenciones = Convert.ToDecimal(lector["ADM_ValorTotalRetenciones"]),
                        IdTipoEntrega = Convert.ToString(lector["ADM_IdTipoEntrega"]),
                        IdPaisDestino = Convert.ToString(lector["ADM_IdPaisDestino"]),
                        IdPaisOrigen = Convert.ToString(lector["ADM_IdPaisOrigen"]),
                        NombrePaisDestino = Convert.ToString(lector["ADM_NombrePaisDestino"]),
                        NombrePaisOrigen = Convert.ToString(lector["ADM_NombrePaisOrigen"]),
                        CodigoPostalDestino = Convert.ToString(lector["ADM_CodigoPostalDestino"]),
                        CodigoPostalOrigen = Convert.ToString(lector["ADM_CodigoPostalOrigen"]),
                        ValorAdicionales = Convert.ToDecimal(lector["ADM_ValorAdicionales"]),
                        NumeroBolsaSeguridad = Convert.ToString(lector["ADM_NumeroBolsaSeguridad"]),
                        IdMotivoNoUsoBolsaSegurida = lector["ADM_IdMotivoNoUsoBolsaSegurida"] == DBNull.Value ? null : (short?)lector["ADM_IdMotivoNoUsoBolsaSegurida"],
                        MotivoNoUsoBolsaSeguriDesc = Convert.ToString(lector["ADM_MotivoNoUsoBolsaSeguriDesc"]),
                        NoUsoaBolsaSeguridadObserv = Convert.ToString(lector["ADM_NoUsoaBolsaSeguridadObserv"]),
                        PesoLiqMasa = Convert.ToDecimal(lector["ADM_PesoLiqMasa"]),
                        PesoLiqVolumetrico = Convert.ToDecimal(lector["ADM_PesoLiqVolumetrico"]),
                        CreadoPor = Convert.ToString(lector["ADM_CreadoPor"]),
                        ValorPrimaSeguro = Convert.ToDecimal(lector["ADM_ValorPrimaSeguro"]),
                        //NotificarEntregaPorEmail = Convert.ToBoolean(lector["ADM_NotificarEntregaPorEmail"]) == null ? false : Convert.ToBoolean(lector["ADM_NotificarEntregaPorEmail"]),
                        CantidadIntentosEntrega = Convert.ToInt32(lector["ADM_CantidadReintentosEntrega"])
                    };
                }
                conn.Close();

            }
            return guiaBuscada;
        }

        /// <summary>
        /// Obtiene una lista con las guias encontradas sea por numero de guia o por fecha
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="fechaInicio"></param>
        /// <param name="fechaFin"></param>
        /// <returns></returns>
        public List<ADGuia> ObtenerGuiaPorNumeroOFecha(long? numeroGuia, DateTime fechaInicio, DateTime fechaFin, short index, short size)
        {
            List<ADGuia> listaGuias = new List<ADGuia>();
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand("paObtenerGuiaPorIDoFecha_MEN", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("@NumeroGuia", numeroGuia);
                comm.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                comm.Parameters.AddWithValue("@FechaFin", fechaFin);
                comm.Parameters.AddWithValue("@PageIndex", index);
                comm.Parameters.AddWithValue("@PageSize", size);
                SqlDataReader lector = comm.ExecuteReader();
                while (lector.Read())
                {
                    ADGuia guiaBuscada = new ADGuia()
                    {
                        NumeroGuia = Convert.ToInt64(lector["ADM_NumeroGuia"]),
                        NombreCiudadOrigen = Convert.ToString(lector["ADM_NombreCiudadOrigen"]),
                        NombreCiudadDestino = Convert.ToString(lector["ADM_NombreCiudadDestino"]),
                        ValorTotal = Convert.ToDecimal(lector["ADM_ValorTotal"]),
                        Remitente = new CLClienteContadoDC()
                        {
                            Telefono = Convert.ToString(lector["ADM_TelefonoRemitente"])
                        }
                    };
                    listaGuias.Add(guiaBuscada);
                }
                conn.Close();

            }
            return listaGuias;
        }

        /// <summary>
        /// Metodo que obtiene la información de una admisión de mensajeria a partir del numero de la misma
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <returns></returns>
        public ADGuia ObtenerGuia(long idAdmision)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var admision = contexto.paObtenerAdmisionPorId_MEN(idAdmision).FirstOrDefault();
                if (admision == null)
                {
                    throw new FaultException<ControllerException>
                      (new ControllerException(COConstantesModulos.MENSAJERIA,
                        ADEnumTipoErrorMensajeria.EX_NUMERO_GUIA_NO_EXISTE.ToString(),
                        ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.EX_NUMERO_GUIA_NO_EXISTE)));
                }
                else
                {
                    return new ADGuia
                    {
                        IdAdmision = admision.ADM_IdAdminisionMensajeria,
                        NumeroGuia = admision.ADM_NumeroGuia,
                        IdCiudadOrigen = admision.ADM_IdCiudadOrigen,
                        NombreCiudadOrigen = admision.ADM_NombreCiudadOrigen,
                        IdCiudadDestino = admision.ADM_IdCiudadDestino,
                        NombreCiudadDestino = admision.ADM_NombreCiudadDestino,
                        ValorTotal = admision.ADM_ValorTotal,
                        ValorDeclarado = admision.ADM_ValorDeclarado,
                        IdServicio = admision.ADM_IdServicio,
                        EsAlCobro = admision.ADM_EsAlCobro,
                        EstaPagada = admision.ADM_EstaPagada,
                        FechaAdmision = admision.ADM_FechaAdmision,
                        Peso = admision.ADM_Peso,
                        EsPesoVolumetrico = admision.ADM_EsPesoVolumetrico,
                        ValorServicio = admision.ADM_ValorAdmision,
                        IdCentroServicioOrigen = admision.ADM_IdCentroServicioOrigen,
                        NombreCentroServicioOrigen = admision.ADM_NombreCentroServicioOrigen,
                        IdCentroServicioDestino = admision.ADM_IdCentroServicioDestino,
                        NombreCentroServicioDestino = admision.ADM_NombreCentroServicioDestino,
                        ValorPrimaSeguro = admision.ADM_ValorPrimaSeguro,
                        ValorAdicionales = admision.ADM_ValorAdicionales,
                        NombreServicio = admision.ADM_NombreServicio,
                        Observaciones = admision.ADM_Observaciones,
                        FormasPago = new List<ADGuiaFormaPago> { },
                        IdTipoEnvio = admision.ADM_IdTipoEnvio,
                        NombreTipoEnvio = admision.ADM_NombreTipoEnvio,
                        TotalPiezas = admision.ADM_TotalPiezas,
                        PrefijoNumeroGuia = admision.ADM_PrefijoNumeroGuia,
                        TelefonoDestinatario = admision.ADM_TelefonoDestinatario,
                        DireccionDestinatario = admision.ADM_DireccionDestinatario,
                        TipoCliente = (ADEnumTipoCliente)Enum.Parse(typeof(ADEnumTipoCliente), admision.ADM_TipoCliente, true),
                        Remitente = new CLClienteContadoDC() { Nombre = admision.ADM_NombreRemitente, Telefono = admision.ADM_TelefonoRemitente },
                        Destinatario = new CLClienteContadoDC() { Nombre = admision.ADM_NombreDestinatario, Direccion = admision.ADM_DireccionDestinatario, Telefono = admision.ADM_TelefonoDestinatario },
                        DiceContener = admision.ADM_DiceContener,
                        Supervision = admision.ADM_EsSupervisada,
                        FechaSupervision = admision.ADM_FechaSupervision,
                    };
                }
            }
        }

        /// <summary>
        /// Obtiene toda la informacion de una guia a partir del numero de guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ADGuia ObtenerGuiaXNumeroGuia(long numeroGuia)
        {
        
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                
                SqlCommand comm = new SqlCommand("paObtenerAdmisionMensajeriaFormaPago_MEN", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("@numGuia", numeroGuia);
                SqlDataAdapter da = new SqlDataAdapter(comm);
                DataTable dt = new DataTable();
                conn.Open();
                da.Fill(dt);
                conn.Close();

                var lstAdm = dt.AsEnumerable().ToList();
                var adm = lstAdm.GroupBy(r => r.Field<long>("ADM_IdAdminisionMensajeria")).Select(s => s.First()).ToList().FirstOrDefault();

                if (adm == null)
                    throw new FaultException<ControllerException>
                      (new ControllerException(COConstantesModulos.MENSAJERIA, ADEnumTipoErrorMensajeria.EX_NUMERO_GUIA_NO_EXISTE.ToString(),
                      ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.EX_NUMERO_GUIA_NO_EXISTE)));


                ADGuia guia = new ADGuia()
                {
                    EstadoGuia = (ADEnumEstadoGuia)Enum.Parse(typeof(ADEnumEstadoGuia), Convert.ToString(adm["ADM_IdEstadoGuia"]).Trim(), true),
                    IdTrazaGuia = Convert.ToInt64(adm["ADM_IdEstadoGuiaLog"]),
                    Entregada = Convert.ToBoolean(adm["ADM_EstaEntregada"]),
                    IdAdmision = Convert.ToInt64(adm["ADM_IdAdminisionMensajeria"]),
                    NumeroGuia = Convert.ToInt64(adm["ADM_NumeroGuia"]),
                    GuidDeChequeo = Convert.ToString(adm["ADM_GuidDeChequeo"]),
                    IdCiudadOrigen = Convert.ToString(adm["ADM_IdCiudadOrigen"]),
                    NombreCiudadOrigen = Convert.ToString(adm["ADM_NombreCiudadOrigen"]),
                    IdCiudadDestino = Convert.ToString(adm["ADM_IdCiudadDestino"]),
                    NombreCiudadDestino = Convert.ToString(adm["ADM_NombreCiudadDestino"]),
                    ValorTotal = Convert.ToDecimal(adm["ADM_ValorTotal"]),
                    ValorDeclarado = Convert.ToDecimal(adm["ADM_ValorDeclarado"]),
                    IdServicio = Convert.ToInt32(adm["ADM_IdServicio"]),
                    EsAlCobro = Convert.ToBoolean(adm["ADM_EsAlCobro"]),
                    EstaPagada = Convert.ToBoolean(adm["ADM_EstaPagada"]),
                    FechaAdmision = Convert.ToDateTime(adm["ADM_FechaAdmision"]),
                    Peso = Convert.ToDecimal(adm["ADM_Peso"]),
                    EsPesoVolumetrico = Convert.ToBoolean(adm["ADM_EsPesoVolumetrico"]),
                    ValorServicio = Convert.ToDecimal(adm["ADM_ValorAdmision"]),
                    IdCentroServicioOrigen = Convert.ToInt64(adm["ADM_IdCentroServicioOrigen"]),
                    NombreCentroServicioOrigen = Convert.ToString(adm["ADM_NombreCentroServicioOrigen"]),
                    IdCentroServicioDestino = Convert.ToInt64(adm["ADM_IdCentroServicioDestino"]),
                    NombreCentroServicioDestino = Convert.ToString(adm["ADM_NombreCentroServicioDestino"]),
                    NombreServicio = Convert.ToString(adm["ADM_NombreServicio"]),
                    Observaciones = Convert.ToString(adm["ADM_Observaciones"]),
                    IdTipoEnvio = Convert.ToInt16(adm["ADM_IdTipoEnvio"]),
                    NombreTipoEnvio = Convert.ToString(adm["ADM_NombreTipoEnvio"]),
                    TotalPiezas = Convert.ToInt16(adm["ADM_TotalPiezas"]),
                    PrefijoNumeroGuia = Convert.ToString(adm["ADM_PrefijoNumeroGuia"]),
                    TelefonoDestinatario = Convert.ToString(adm["ADM_TelefonoDestinatario"]),
                    DireccionDestinatario = Convert.ToString(adm["ADM_DireccionDestinatario"]),
                    TipoCliente = (ADEnumTipoCliente)Enum.Parse(typeof(ADEnumTipoCliente), Convert.ToString(adm["ADM_TipoCliente"]).Trim(), true),
                    Remitente = new CLClienteContadoDC()
                    {
                        Nombre = Convert.ToString(adm["ADM_NombreRemitente"]),
                        Telefono = Convert.ToString(adm["ADM_TelefonoRemitente"]),
                        Identificacion = Convert.ToString(adm["ADM_IdRemitente"]),
                        Direccion = Convert.ToString(adm["ADM_DireccionRemitente"]),
                        TipoId = Convert.ToString(adm["ADM_IdTipoIdentificacionRemitente"]),
                        Email = Convert.ToString(adm["ADM_EmailRemitente"])
                    },
                    Destinatario = new CLClienteContadoDC()
                    {
                        Nombre = Convert.ToString(adm["ADM_NombreDestinatario"]),
                        Direccion = Convert.ToString(adm["ADM_DireccionDestinatario"]),
                        TipoId = Convert.ToString(adm["ADM_IdTipoIdentificacionDestinatario"]),
                        Telefono = Convert.ToString(adm["ADM_TelefonoDestinatario"]),
                        Identificacion = Convert.ToString(adm["ADM_IdDestinatario"]),
                        Email = Convert.ToString(adm["ADM_EmailDestinatario"])
                    },
                    DiceContener = Convert.ToString(adm["ADM_DiceContener"]),
                    Supervision = Convert.ToBoolean(adm["ADM_EsSupervisada"]),
                    FechaSupervision = Convert.ToDateTime(adm["ADM_FechaSupervision"]),
                    IdMensajero = Convert.ToInt64(adm["ADM_IdMensajero"]),
                    NombreMensajero = adm["ADM_NombreMensajero"] == DBNull.Value ? string.Empty : Convert.ToString(adm["ADM_NombreMensajero"]),
                    DiasDeEntrega = Convert.ToInt16(adm["ADM_DiasDeEntrega"]),
                    DescripcionTipoEntrega = Convert.ToString(adm["ADM_DescripcionTipoEntrega"]),
                    EsRecomendado = Convert.ToBoolean(adm["ADM_EsRecomendado"]),
                    Alto = Convert.ToDecimal(adm["ADM_Alto"]),
                    Ancho = Convert.ToDecimal(adm["ADM_Ancho"]),
                    Largo = Convert.ToDecimal(adm["ADM_Largo"]),
                    DigitoVerificacion = Convert.ToString(adm["ADM_DigitoVerificacion"]),
                    FechaEstimadaEntrega = Convert.ToDateTime(adm["ADM_FechaEstimadaEntrega"]),
                    NumeroPieza = Convert.ToInt16(adm["ADM_NumeroPieza"]),
                    IdUnidadMedida = Convert.ToString(adm["ADM_IdUnidadMedida"]),
                    IdUnidadNegocio = Convert.ToString(adm["ADM_IdUnidadNegocio"]),
                    ValorTotalImpuestos = Convert.ToDecimal(adm["ADM_ValorTotalImpuestos"]),
                    ValorTotalRetenciones = Convert.ToDecimal(adm["ADM_ValorTotalRetenciones"]),
                    IdTipoEntrega = Convert.ToString(adm["ADM_IdTipoEntrega"]),
                    IdPaisDestino = Convert.ToString(adm["ADM_IdPaisDestino"]),
                    IdPaisOrigen = Convert.ToString(adm["ADM_IdPaisOrigen"]),
                    NombrePaisDestino = Convert.ToString(adm["ADM_NombrePaisDestino"]),
                    NombrePaisOrigen = Convert.ToString(adm["ADM_NombrePaisOrigen"]),
                    CodigoPostalDestino = Convert.ToString(adm["ADM_CodigoPostalDestino"]),
                    CodigoPostalOrigen = Convert.ToString(adm["ADM_CodigoPostalOrigen"]),
                    ValorAdicionales = Convert.ToDecimal(adm["ADM_ValorAdicionales"]),
                    NumeroBolsaSeguridad = Convert.ToString(adm["ADM_NumeroBolsaSeguridad"]),
                    IdMotivoNoUsoBolsaSegurida = adm["ADM_IdMotivoNoUsoBolsaSegurida"] == DBNull.Value ? null : (short?)adm["ADM_IdMotivoNoUsoBolsaSegurida"],
                    MotivoNoUsoBolsaSeguriDesc = Convert.ToString(adm["ADM_MotivoNoUsoBolsaSeguriDesc"]),
                    NoUsoaBolsaSeguridadObserv = adm["ADM_NoUsoaBolsaSeguridadObserv"] == DBNull.Value ? string.Empty : Convert.ToString(adm["ADM_NoUsoaBolsaSeguridadObserv"]),
                    PesoLiqMasa = Convert.ToDecimal(adm["ADM_PesoLiqMasa"]),
                    PesoLiqVolumetrico = Convert.ToDecimal(adm["ADM_PesoLiqVolumetrico"]),
                    CreadoPor = Convert.ToString(adm["ADM_CreadoPor"]),
                    ValorPrimaSeguro = Convert.ToDecimal(adm["ADM_ValorPrimaSeguro"]),
                    ValorAdmision = Convert.ToDecimal(adm["ADM_ValorAdmision"]),
                    CantidadIntentosEntrega = Convert.ToInt32(adm["ADM_CantidadReintentosEntrega"]),
                    FechaEntrega = Convert.ToDateTime(adm["ADM_FechaEntrega"]),
                    FechaEstimadaEntregaNew = adm["ADM_FechaEstimadaEntregaNew"]==DBNull.Value ? Convert.ToDateTime(adm["ADM_FechaEntrega"]):  Convert.ToDateTime(adm["ADM_FechaEstimadaEntregaNew"]),
                    FormasPagoDescripcion = Convert.ToString(adm["FOP_Descripcion"]),
                    EsAutomatico = Convert.ToBoolean(adm["ADM_EsAutomatico"]),
                    IdCentroServicioEstado = adm["ADM_IdCentroServicioEstado"] == DBNull.Value ? 0 : Convert.ToInt64(adm["ADM_IdCentroServicioEstado"]),
                    NombreCentroServicioEstado = adm["ADM_NombreCentroServicioEstado"] == DBNull.Value ? string.Empty : adm["ADM_NombreCentroServicioEstado"].ToString(),
                };


                        guia.FormasPago = lstAdm.Where(a => a.Field<long>("ADM_IdAdminisionMensajeria") == guia.IdAdmision).ToList().ConvertAll<ADGuiaFormaPago>(f =>                               
                        new ADGuiaFormaPago
                        {
                               IdFormaPago = Convert.ToInt16(f["FOP_IdFormaPago"]),                               
                            Descripcion = Convert.ToString(f["FOP_Descripcion"]),
                               Valor = f.Field<decimal>("AGF_Valor")                                   
                             });

                if (guia.FormasPago == null)
                {
                    guia.FormasPago = new List<ADGuiaFormaPago>
                                                {
                                                    new ADGuiaFormaPago {
                                                    }
                                                };
                }
                else
                {
                    guia.FormasPagoDescripcion = guia.FormasPago.FirstOrDefault().Descripcion;
                }


                        guia.TrazaGuiaEstado = EstadosGuia.ObtenerTrazaUltimoEstadoGuia(guia.IdAdmision);

           return guia;

                                      
            }


      


        /* 
         using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
         {
             var admision = contexto.paObtenerAdmisionMensajeri_MEN(numeroGuia).FirstOrDefault();

             if (admision == null)
                 throw new FaultException<ControllerException>
                   (new ControllerException(COConstantesModulos.MENSAJERIA, ADEnumTipoErrorMensajeria.EX_NUMERO_GUIA_NO_EXISTE.ToString(),
                   ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.EX_NUMERO_GUIA_NO_EXISTE)));
             else
             {
                 ADGuia objGuia = new ADGuia()
                {
                    Entregada = admision.ADM_EstaEntregada,
                    IdAdmision = admision.ADM_IdAdminisionMensajeria,
                    NumeroGuia = admision.ADM_NumeroGuia,
                    IdCiudadOrigen = admision.ADM_IdCiudadOrigen,
                    NombreCiudadOrigen = admision.ADM_NombreCiudadOrigen,
                    IdCiudadDestino = admision.ADM_IdCiudadDestino,
                    NombreCiudadDestino = admision.ADM_NombreCiudadDestino,
                    ValorTotal = admision.ADM_ValorTotal,
                    ValorDeclarado = admision.ADM_ValorDeclarado,
                    IdServicio = admision.ADM_IdServicio,
                    EsAlCobro = admision.ADM_EsAlCobro,
                    EstaPagada = admision.ADM_EstaPagada,
                    FechaAdmision = admision.ADM_FechaAdmision,
                    Peso = admision.ADM_Peso,
                    EsPesoVolumetrico = admision.ADM_EsPesoVolumetrico,
                    ValorServicio = admision.ADM_ValorAdmision,
                    IdCentroServicioOrigen = admision.ADM_IdCentroServicioOrigen,
                    NombreCentroServicioOrigen = admision.ADM_NombreCentroServicioOrigen,
                    IdCentroServicioDestino = admision.ADM_IdCentroServicioDestino,
                    NombreCentroServicioDestino = admision.ADM_NombreCentroServicioDestino,
                    NombreServicio = admision.ADM_NombreServicio,
                    Observaciones = admision.ADM_Observaciones,
                    FormasPago = contexto.FormasPagoGuia_VMEN.Where(fp => fp.AGF_IdAdminisionMensajeria == admision.ADM_IdAdminisionMensajeria).ToList().ConvertAll(f => new ADGuiaFormaPago { Descripcion = f.FOP_Descripcion, IdFormaPago = f.AGF_IdFormaPago, Valor = f.AGF_Valor }),
                    IdTipoEnvio = admision.ADM_IdTipoEnvio,
                    NombreTipoEnvio = admision.ADM_NombreTipoEnvio,
                    TotalPiezas = admision.ADM_TotalPiezas,
                    PrefijoNumeroGuia = admision.ADM_PrefijoNumeroGuia,
                    TelefonoDestinatario = admision.ADM_TelefonoDestinatario,
                    DireccionDestinatario = admision.ADM_DireccionDestinatario,
                    TipoCliente = (ADEnumTipoCliente)Enum.Parse(typeof(ADEnumTipoCliente), admision.ADM_TipoCliente, true),
                    Remitente = new CLClienteContadoDC()
                    {
                        Nombre = admision.ADM_NombreRemitente,
                        Telefono = admision.ADM_TelefonoRemitente,
                        Identificacion = admision.ADM_IdRemitente,
                        Direccion = admision.ADM_DireccionRemitente,
                        TipoId = admision.ADM_IdTipoIdentificacionRemitente,
                        Email = admision.ADM_EmailRemitente
                    },
                    Destinatario = new CLClienteContadoDC()
                    {
                        Nombre = admision.ADM_NombreDestinatario,
                        Direccion = admision.ADM_DireccionDestinatario,
                        TipoId = admision.ADM_IdTipoIdentificacionDestinatario,
                        Telefono = admision.ADM_TelefonoDestinatario,
                        Identificacion = admision.ADM_IdDestinatario,
                        Email = admision.ADM_EmailDestinatario
                    },
                    DiceContener = admision.ADM_DiceContener,
                    Supervision = admision.ADM_EsSupervisada,
                    FechaSupervision = admision.ADM_FechaSupervision,
                    IdMensajero = admision.ADM_IdMensajero,
                    NombreMensajero = admision.ADM_NombreMensajero,
                    DiasDeEntrega = admision.ADM_DiasDeEntrega,
                    DescripcionTipoEntrega = admision.ADM_DescripcionTipoEntrega,
                    EsRecomendado = admision.ADM_EsRecomendado,
                    Alto = admision.ADM_Alto,
                    Ancho = admision.ADM_Ancho,
                    Largo = admision.ADM_Largo,
                    DigitoVerificacion = admision.ADM_DigitoVerificacion,
                    FechaEstimadaEntrega = admision.ADM_FechaEstimadaEntrega,
                    NumeroPieza = admision.ADM_NumeroPieza,
                    IdUnidadMedida = admision.ADM_IdUnidadMedida,
                    IdUnidadNegocio = admision.ADM_IdUnidadNegocio,
                    ValorTotalImpuestos = admision.ADM_ValorTotalImpuestos,
                    ValorTotalRetenciones = admision.ADM_ValorTotalRetenciones,
                    IdTipoEntrega = admision.ADM_IdTipoEntrega,
                    IdPaisDestino = admision.ADM_IdPaisDestino,
                    IdPaisOrigen = admision.ADM_IdPaisOrigen,
                    NombrePaisDestino = admision.ADM_NombrePaisDestino,
                    NombrePaisOrigen = admision.ADM_NombrePaisOrigen,
                    CodigoPostalDestino = admision.ADM_CodigoPostalDestino,
                    CodigoPostalOrigen = admision.ADM_CodigoPostalOrigen,
                    ValorAdicionales = admision.ADM_ValorAdicionales,
                    NumeroBolsaSeguridad = admision.ADM_NumeroBolsaSeguridad,
                    IdMotivoNoUsoBolsaSegurida = admision.ADM_IdMotivoNoUsoBolsaSegurida,
                    MotivoNoUsoBolsaSeguriDesc = admision.ADM_MotivoNoUsoBolsaSeguriDesc,
                    NoUsoaBolsaSeguridadObserv = admision.ADM_NoUsoaBolsaSeguridadObserv,
                    PesoLiqMasa = admision.ADM_PesoLiqMasa,
                    PesoLiqVolumetrico = admision.ADM_PesoLiqVolumetrico,
                    CreadoPor = admision.ADM_CreadoPor,
                    ValorPrimaSeguro = admision.ADM_ValorPrimaSeguro,
                    NotificarEntregaPorEmail = admision.ADM_NotificarEntregaPorEmail == null ? false : admision.ADM_NotificarEntregaPorEmail.Value,
                    CantidadIntentosEntrega = admision.ADM_CantidadReintentosEntrega                       
                };

                 objGuia.TrazaGuiaEstado = EstadosGuia.ObtenerTrazaUltimoEstadoGuia(objGuia.IdAdmision);
                 return objGuia;

             }
         }
         */
    }

        public ADGuia ObtenerGuiaSispostalXNumeroGuia(long numeroGuia)
        {
            using (SqlConnection conn = new SqlConnection(CadCnxSispostalController))
            {
                conn.Open();

                SqlCommand comm = new SqlCommand("paObtenerGuiaSispostalFormaPago_TBE", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("@IdGuia", numeroGuia);
                SqlDataReader reader = comm.ExecuteReader();
             
                ADGuia guia = new ADGuia();

                if (reader.HasRows)
                {
                    guia = ADRepositorioMapper.ToListGuiaSispostal(reader);
                }
                else
                    throw new FaultException<ControllerException>
                      (new ControllerException(COConstantesModulos.MENSAJERIA, ADEnumTipoErrorMensajeria.EX_NUMERO_GUIA_NO_EXISTE.ToString(),
                      ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.EX_NUMERO_GUIA_NO_EXISTE)));
                guia.TrazaGuiaEstado = EstadosGuia.ObtenerTrazaUltimoEstadoGuia(guia.IdAdmision);

                return guia;


            }
        }
            


        /// <summary>
        /// Obtiene toda la informacion de una guia a partir del numero de guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ADGuia ObtenerGuiaXNumeroGuiaCredito(long numeroGuia)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var admision = contexto.paObtenerAdmisionMensajeriaCliente_MEN(numeroGuia).FirstOrDefault();

                if (admision == null)
                    throw new FaultException<ControllerException>
                      (new ControllerException(COConstantesModulos.MENSAJERIA, ADEnumTipoErrorMensajeria.EX_NUMERO_GUIA_NO_EXISTE.ToString(),
                      ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.EX_NUMERO_GUIA_NO_EXISTE)));
                else
                {
                    return new ADGuia()
                    {
                        Entregada = admision.ADM_EstaEntregada,
                        IdAdmision = admision.ADM_IdAdminisionMensajeria,
                        NumeroGuia = admision.ADM_NumeroGuia,
                        IdCiudadOrigen = admision.ADM_IdCiudadOrigen,
                        NombreCiudadOrigen = admision.ADM_NombreCiudadOrigen,
                        IdCiudadDestino = admision.ADM_IdCiudadDestino,
                        NombreCiudadDestino = admision.ADM_NombreCiudadDestino,
                        ValorTotal = admision.ADM_ValorTotal,
                        ValorDeclarado = admision.ADM_ValorDeclarado,
                        IdServicio = admision.ADM_IdServicio,
                        EsAlCobro = admision.ADM_EsAlCobro,
                        EstaPagada = admision.ADM_EstaPagada,
                        FechaAdmision = admision.ADM_FechaAdmision,
                        Peso = admision.ADM_Peso,
                        EsPesoVolumetrico = admision.ADM_EsPesoVolumetrico,
                        ValorServicio = admision.ADM_ValorAdmision,
                        IdCentroServicioOrigen = admision.ADM_IdCentroServicioOrigen,
                        NombreCentroServicioOrigen = admision.ADM_NombreCentroServicioOrigen,
                        IdCentroServicioDestino = admision.ADM_IdCentroServicioDestino,
                        NombreCentroServicioDestino = admision.ADM_NombreCentroServicioDestino,
                        NombreServicio = admision.ADM_NombreServicio,
                        Observaciones = admision.ADM_Observaciones,
                        FormasPago = contexto.FormasPagoGuia_VMEN.Where(fp => fp.AGF_IdAdminisionMensajeria == admision.ADM_IdAdminisionMensajeria).ToList().ConvertAll(f => new ADGuiaFormaPago { Descripcion = f.FOP_Descripcion, IdFormaPago = f.AGF_IdFormaPago, Valor = f.AGF_Valor }),
                        IdTipoEnvio = admision.ADM_IdTipoEnvio,
                        NombreTipoEnvio = admision.ADM_NombreTipoEnvio,
                        TotalPiezas = admision.ADM_TotalPiezas,
                        PrefijoNumeroGuia = admision.ADM_PrefijoNumeroGuia,
                        TelefonoDestinatario = admision.ADM_TelefonoDestinatario,
                        DireccionDestinatario = admision.ADM_DireccionDestinatario,
                        TipoCliente = (ADEnumTipoCliente)Enum.Parse(typeof(ADEnumTipoCliente), admision.ADM_TipoCliente, true),
                        Remitente = new CLClienteContadoDC()
                        {
                            Nombre = admision.ADM_NombreRemitente,
                            Telefono = admision.ADM_TelefonoRemitente,
                            Identificacion = admision.ADM_IdRemitente,
                            Direccion = admision.ADM_DireccionRemitente,
                            TipoId = admision.ADM_IdTipoIdentificacionRemitente,
                            Email = admision.ADM_EmailRemitente
                        },
                        Destinatario = new CLClienteContadoDC()
                        {
                            Nombre = admision.ADM_NombreDestinatario,
                            Direccion = admision.ADM_DireccionDestinatario,
                            TipoId = admision.ADM_IdTipoIdentificacionDestinatario,
                            Telefono = admision.ADM_TelefonoDestinatario,
                            Identificacion = admision.ADM_IdDestinatario,
                            Email = admision.ADM_EmailDestinatario
                        },
                        DiceContener = admision.ADM_DiceContener,
                        Supervision = admision.ADM_EsSupervisada,
                        FechaSupervision = admision.ADM_FechaSupervision,
                        IdMensajero = admision.ADM_IdMensajero,
                        NombreMensajero = admision.ADM_NombreMensajero,
                        DiasDeEntrega = admision.ADM_DiasDeEntrega,
                        DescripcionTipoEntrega = admision.ADM_DescripcionTipoEntrega,
                        EsRecomendado = admision.ADM_EsRecomendado,
                        Alto = admision.ADM_Alto,
                        Ancho = admision.ADM_Ancho,
                        Largo = admision.ADM_Largo,
                        DigitoVerificacion = admision.ADM_DigitoVerificacion,
                        FechaEstimadaEntrega = admision.ADM_FechaEstimadaEntrega,
                        NumeroPieza = admision.ADM_NumeroPieza,
                        IdUnidadMedida = admision.ADM_IdUnidadMedida,
                        IdUnidadNegocio = admision.ADM_IdUnidadNegocio,
                        ValorTotalImpuestos = admision.ADM_ValorTotalImpuestos,
                        ValorTotalRetenciones = admision.ADM_ValorTotalRetenciones,
                        IdTipoEntrega = admision.ADM_IdTipoEntrega,
                        IdPaisDestino = admision.ADM_IdPaisDestino,
                        IdPaisOrigen = admision.ADM_IdPaisOrigen,
                        NombrePaisDestino = admision.ADM_NombrePaisDestino,
                        NombrePaisOrigen = admision.ADM_NombrePaisOrigen,
                        CodigoPostalDestino = admision.ADM_CodigoPostalDestino,
                        CodigoPostalOrigen = admision.ADM_CodigoPostalOrigen,
                        ValorAdicionales = admision.ADM_ValorAdicionales,
                        NumeroBolsaSeguridad = admision.ADM_NumeroBolsaSeguridad,
                        IdMotivoNoUsoBolsaSegurida = admision.ADM_IdMotivoNoUsoBolsaSegurida,
                        MotivoNoUsoBolsaSeguriDesc = admision.ADM_MotivoNoUsoBolsaSeguriDesc,
                        NoUsoaBolsaSeguridadObserv = admision.ADM_NoUsoaBolsaSeguridadObserv,
                        PesoLiqMasa = admision.ADM_PesoLiqMasa,
                        PesoLiqVolumetrico = admision.ADM_PesoLiqVolumetrico,
                        CreadoPor = admision.ADM_CreadoPor,
                        ValorPrimaSeguro = admision.ADM_ValorPrimaSeguro,
                        IdCliente = admision.MCP_IdConvenioRemitente,
                        NotificarEntregaPorEmail = admision.ADM_NotificarEntregaPorEmail == null ? false : admision.ADM_NotificarEntregaPorEmail.Value,
                        IdSucursal = admision.MCP_IdSucursalRecogida.Value,
                    };
                }
            }
        }

        /// <summary>
        /// Retorna la informacion de la guia, sin lanzar la excepcion
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ADGuia ObtenerInfoGuiaXNumeroGuia(long numeroGuia)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var admision = contexto.paObtenerAdmisionMensajeri_MEN(numeroGuia).FirstOrDefault();

                if (admision != null)
                {
                    return new ADGuia()
                    {
                        IdAdmision = admision.ADM_IdAdminisionMensajeria,
                        NumeroGuia = admision.ADM_NumeroGuia,
                        IdCiudadOrigen = admision.ADM_IdCiudadOrigen,
                        NombreCiudadOrigen = admision.ADM_NombreCiudadOrigen,
                        IdCiudadDestino = admision.ADM_IdCiudadDestino,
                        NombreCiudadDestino = admision.ADM_NombreCiudadDestino,
                        ValorTotal = admision.ADM_ValorTotal,
                        ValorDeclarado = admision.ADM_ValorDeclarado,
                        IdServicio = admision.ADM_IdServicio,
                        EsAlCobro = admision.ADM_EsAlCobro,
                        EstaPagada = admision.ADM_EstaPagada,
                        FechaAdmision = admision.ADM_FechaAdmision,
                        Peso = admision.ADM_Peso,
                        EsPesoVolumetrico = admision.ADM_EsPesoVolumetrico,
                        ValorServicio = admision.ADM_ValorAdmision,
                        IdCentroServicioOrigen = admision.ADM_IdCentroServicioOrigen,
                        NombreCentroServicioOrigen = admision.ADM_NombreCentroServicioOrigen,
                        IdCentroServicioDestino = admision.ADM_IdCentroServicioDestino,
                        NombreCentroServicioDestino = admision.ADM_NombreCentroServicioDestino,
                        NombreServicio = admision.ADM_NombreServicio,
                        Observaciones = admision.ADM_Observaciones,
                        FormasPago = new List<ADGuiaFormaPago> { },
                        IdTipoEnvio = admision.ADM_IdTipoEnvio,
                        NombreTipoEnvio = admision.ADM_NombreTipoEnvio,
                        TotalPiezas = admision.ADM_TotalPiezas,
                        PrefijoNumeroGuia = admision.ADM_PrefijoNumeroGuia,
                        TelefonoDestinatario = admision.ADM_TelefonoDestinatario,
                        DireccionDestinatario = admision.ADM_DireccionDestinatario,
                        TipoCliente = (ADEnumTipoCliente)Enum.Parse(typeof(ADEnumTipoCliente), admision.ADM_TipoCliente, true),
                        Remitente = new CLClienteContadoDC()
                        {
                            Nombre = admision.ADM_NombreRemitente,
                            Telefono = admision.ADM_TelefonoRemitente,
                            Identificacion = admision.ADM_IdRemitente,
                            Direccion = admision.ADM_DireccionRemitente,
                        },
                        Destinatario = new CLClienteContadoDC() { Nombre = admision.ADM_NombreDestinatario, Direccion = admision.ADM_DireccionDestinatario, TipoId = admision.ADM_IdTipoIdentificacionDestinatario, Telefono = admision.ADM_TelefonoDestinatario, Identificacion = admision.ADM_IdDestinatario },
                        DiceContener = admision.ADM_DiceContener,
                        Supervision = admision.ADM_EsSupervisada,
                        FechaSupervision = admision.ADM_FechaSupervision,
                        IdMensajero = admision.ADM_IdMensajero,
                        NombreMensajero = admision.ADM_NombreMensajero,
                        DiasDeEntrega = admision.ADM_DiasDeEntrega,
                        DescripcionTipoEntrega = admision.ADM_DescripcionTipoEntrega,
                        EsRecomendado = admision.ADM_EsRecomendado,
                        Alto = admision.ADM_Alto,
                        Ancho = admision.ADM_Ancho,
                        Largo = admision.ADM_Largo,
                        DigitoVerificacion = admision.ADM_DigitoVerificacion,
                        FechaEstimadaEntrega = admision.ADM_FechaEstimadaEntrega,
                        NumeroPieza = admision.ADM_NumeroPieza,
                        IdUnidadMedida = admision.ADM_IdUnidadMedida,
                        IdUnidadNegocio = admision.ADM_IdUnidadNegocio,
                        ValorTotalImpuestos = admision.ADM_ValorTotalImpuestos,
                        ValorTotalRetenciones = admision.ADM_ValorTotalRetenciones,
                        IdTipoEntrega = admision.ADM_IdTipoEntrega,
                        IdPaisDestino = admision.ADM_IdPaisDestino,
                        IdPaisOrigen = admision.ADM_IdPaisOrigen,
                        NombrePaisDestino = admision.ADM_NombrePaisDestino,
                        NombrePaisOrigen = admision.ADM_NombrePaisOrigen,
                        CodigoPostalDestino = admision.ADM_CodigoPostalDestino,
                        CodigoPostalOrigen = admision.ADM_CodigoPostalOrigen,
                        ValorAdicionales = admision.ADM_ValorAdicionales,
                        NumeroBolsaSeguridad = admision.ADM_NumeroBolsaSeguridad,
                        IdMotivoNoUsoBolsaSegurida = admision.ADM_IdMotivoNoUsoBolsaSegurida,
                        MotivoNoUsoBolsaSeguriDesc = admision.ADM_MotivoNoUsoBolsaSeguriDesc,
                        NoUsoaBolsaSeguridadObserv = admision.ADM_NoUsoaBolsaSeguridadObserv,
                        PesoLiqMasa = admision.ADM_PesoLiqMasa,
                        PesoLiqVolumetrico = admision.ADM_PesoLiqVolumetrico,
                        Entregada = admision.ADM_EstaEntregada
                    };
                }
                else
                    return new ADGuia() { IdAdmision = 0 };
            }
        }

        /// <summary>
        /// Obtiene toda la informacion´de admisión a partir de una lista de números de guías
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<ADGuia> ObtenerListaGuias(List<long> listaNumerosGuias)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                List<ADGuia> listaGuias = new List<ADGuia>();
                if (listaNumerosGuias.Any())
                {
                    listaNumerosGuias.ForEach(numeroGuia =>
                    {
                        var admision = contexto.paObtenerAdmisionFormaPago_MEN(numeroGuia).FirstOrDefault();

                        if (admision != null)
                        {
                            listaGuias.Add(new ADGuia()
                            {
                                IdAdmision = admision.ADM_IdAdminisionMensajeria,
                                NumeroGuia = admision.ADM_NumeroGuia,
                                CodigoPostalDestino = admision.ADM_CodigoPostalDestino,
                                CodigoPostalOrigen = admision.ADM_CodigoPostalOrigen,
                                Destinatario = new Servicios.ContratoDatos.Clientes.CLClienteContadoDC
                                {
                                    Nombre = admision.ADM_NombreDestinatario,
                                    Apellido1 = string.Empty,
                                    Apellido2 = string.Empty,
                                    Direccion = admision.ADM_DireccionDestinatario,
                                    Identificacion = admision.ADM_IdDestinatario,
                                    Telefono = admision.ADM_TelefonoDestinatario,
                                    TipoId = admision.ADM_IdTipoIdentificacionDestinatario
                                },
                                DiasDeEntrega = admision.ADM_DiasDeEntrega,
                                DiceContener = admision.ADM_DiceContener,
                                EsAutomatico = admision.ADM_EsAutomatico,
                                FechaAdmision = admision.ADM_FechaAdmision,
                                IdCentroServicioOrigen = admision.ADM_IdCentroServicioOrigen,
                                IdUnidadNegocio = admision.ADM_IdUnidadNegocio,
                                IdServicio = admision.ADM_IdServicio,
                                NombreCentroServicioDestino = admision.ADM_NombreCentroServicioDestino,
                                NombreCentroServicioOrigen = admision.ADM_NombreCentroServicioOrigen,
                                NombreCiudadDestino = admision.ADM_NombreCiudadDestino,
                                NombreCiudadOrigen = admision.ADM_NombreCiudadOrigen,
                                NombreServicio = admision.ADM_NombreServicio,
                                NombreTipoEnvio = admision.ADM_NombreTipoEnvio,
                                NumeroBolsaSeguridad = admision.ADM_NumeroBolsaSeguridad,
                                NumeroPieza = admision.ADM_NumeroPieza,
                                Observaciones = admision.ADM_Observaciones,
                                PesoLiqMasa = admision.ADM_PesoLiqMasa,
                                PesoLiqVolumetrico = admision.ADM_PesoLiqVolumetrico,
                                Peso = admision.ADM_Peso,
                                PrefijoNumeroGuia = admision.ADM_PrefijoNumeroGuia,
                                Remitente = new Servicios.ContratoDatos.Clientes.CLClienteContadoDC
                                {
                                    Nombre = admision.ADM_NombreRemitente,
                                    Apellido1 = string.Empty,
                                    Apellido2 = string.Empty,
                                    Direccion = admision.ADM_DireccionRemitente,
                                    Identificacion = admision.ADM_IdRemitente,
                                    Telefono = admision.ADM_TelefonoRemitente,
                                    TipoId = admision.ADM_IdTipoIdentificacionRemitente
                                },
                                TipoCliente = (ADEnumTipoCliente)Enum.Parse(typeof(ADEnumTipoCliente), admision.ADM_TipoCliente),
                                TotalPiezas = admision.ADM_TotalPiezas,
                                ValorAdicionales = admision.ADM_ValorAdicionales,
                                ValorAdmision = admision.ADM_ValorAdmision,
                                ValorDeclarado = admision.ADM_ValorDeclarado,
                                ValorPrimaSeguro = admision.ADM_ValorPrimaSeguro,
                                ValorServicio = admision.ADM_ValorAdmision,
                                ValorTotal = admision.ADM_ValorTotal,
                                FormasPagoDescripcion = admision.FormasPago,
                            });
                        }
                    });
                }
                return listaGuias;
            }
        }

        /// <summary>
        /// Obtiene el identificador de la admisión
        /// </summary>
        /// <param name="numeroGuia">Número de guía</param>
        /// <returns>Identificador admisión</returns>
        public long ObtenerIdentificadorAdmision(long numeroGuia)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                paObtenerAdmisionMensajeri_MEN consulta = contexto.paObtenerAdmisionMensajeri_MEN(numeroGuia).FirstOrDefault();

                if (consulta != null)
                    return consulta.ADM_IdAdminisionMensajeria;
                else
                    return 0;
            }
        }

        /// <summary>
        /// Retorna las guías que hayan sido enviadas por el remitente dado y que hayan sido generados el día de hoy (día en que se usa el método)
        /// </summary>
        /// <param name="idDestinatario">Número de identificación del cliente destinatario</param>
        /// <param name="tipoIdDestinatario">Tipo de identificación del cliente destinatario</param>
        /// <param name="centroServiciosOrigen">Id del centro de servicios de origen</param>
        /// <returns></returns>
        public List<ADGuia> ObtenerGuiasPorDestinatarioParaHoy(string idDestinatario, string tipoIdDestinatario, long centroServiciosOrigen)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                List<ADGuiaResumidaHoy_MEN> guias = contexto.paObtenerAdmMenPorDestinatarioHoy_MEN(idDestinatario, tipoIdDestinatario, centroServiciosOrigen, ControllerContext.Current.Usuario).ToList();
                if (guias != null && guias.Count > 0)
                {
                    return guias.ConvertAll(g => new ADGuia()
                    {
                        NumeroGuia = g.ADM_NumeroGuia,
                        TipoCliente = ADEnumTipoCliente.PPE, // Se establece como "peatón peatón" para armar la consulta siempre de esta forma
                        Remitente = new Servicios.ContratoDatos.Clientes.CLClienteContadoDC
                        {
                            Nombre = g.ADM_NombreRemitente,
                            Direccion = g.ADM_DireccionRemitente,
                            Identificacion = g.ADM_IdRemitente,
                            Telefono = g.ADM_TelefonoRemitente,
                            TipoId = g.ADM_IdTipoIdentificacionRemitente
                        },
                        Destinatario = new Servicios.ContratoDatos.Clientes.CLClienteContadoDC
                        {
                            Nombre = g.ADM_NombreDestinatario,
                            Direccion = g.ADM_DireccionDestinatario,
                            Identificacion = g.ADM_IdDestinatario,
                            Telefono = g.ADM_TelefonoDestinatario,
                            TipoId = g.ADM_IdTipoIdentificacionDestinatario
                        },
                        FechaAdmision = g.ADM_FechaAdmision,
                        NombreCentroServicioOrigen = g.ADM_NombreCentroServicioOrigen,
                        NombreCentroServicioDestino = g.ADM_NombreCentroServicioDestino
                    });
                }
                else
                {
                    return new List<ADGuia>();
                }
            }
        }

        /// <summary>
        /// Retorna las guías que hayan sido enviadas por el remitente dado y que hayan sido generados el día de hoy (día en que se usa el método)
        /// </summary>
        /// <param name="idRemitente">Número de identificación del cliente remitente</param>
        /// <param name="tipoIdRemitente">Tipo de identificación del cliente remitente</param>
        /// <returns></returns>
        public List<ADGuia> ObtenerGuiasPorRemitenteParaHoy(string idRemitente, string tipoIdRemitente, long idCentroServiosOrigen)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                List<ADGuiaResumidaHoy_MEN> guias = contexto.paObtenerAdmMenPorRemitenteHoy_MEN(idRemitente, tipoIdRemitente, idCentroServiosOrigen, ControllerContext.Current.Usuario).ToList();
                if (guias != null && guias.Count > 0)
                {
                    return guias.ConvertAll(g => new ADGuia()
                    {
                        NumeroGuia = g.ADM_NumeroGuia,
                        TipoCliente = ADEnumTipoCliente.PPE, // Se establece como "peatón peatón" para armar la consulta siempre de esta forma
                        Remitente = new Servicios.ContratoDatos.Clientes.CLClienteContadoDC
                        {
                            Nombre = g.ADM_NombreRemitente,
                            Direccion = g.ADM_DireccionRemitente,
                            Identificacion = g.ADM_IdRemitente,
                            Telefono = g.ADM_TelefonoRemitente,
                            TipoId = g.ADM_IdTipoIdentificacionRemitente
                        },
                        Destinatario = new Servicios.ContratoDatos.Clientes.CLClienteContadoDC
                        {
                            Nombre = g.ADM_NombreDestinatario,
                            Direccion = g.ADM_DireccionDestinatario,
                            Identificacion = g.ADM_IdDestinatario,
                            Telefono = g.ADM_TelefonoDestinatario,
                            TipoId = g.ADM_IdTipoIdentificacionDestinatario
                        },
                        FechaAdmision = g.ADM_FechaAdmision,
                        NombreCentroServicioOrigen = g.ADM_NombreCentroServicioOrigen,
                        NombreCentroServicioDestino = g.ADM_NombreCentroServicioDestino
                    });
                }
                else
                {
                    return new List<ADGuia>();
                }
            }
        }

        /// <summary>
        /// Obtener guía por número de guía con información de cliente crédito si esta pertenece a un cliente crédito
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="idAdmision"></param>
        /// <returns></returns>
        public ADGuia ObtenerGuiaPorNumeroGuiaConInfoClienteCredito(long numeroGuia, long idAdmision)
        {
            ADGuia guia = ObtenerGuiaXNumeroGuia(numeroGuia);
            ADMensajeriaTipoCliente tipoCliente = new ADMensajeriaTipoCliente();
            switch (guia.TipoCliente)
            {
                case ADEnumTipoCliente.CPE:
                    tipoCliente = ObtenerAdmisionMensajeriaConvenioPeaton(idAdmision);
                    break;

                case ADEnumTipoCliente.CCO:
                    tipoCliente = ObtenerAdmisionMensajeriaConvenioConvenio(idAdmision);
                    break;
            }
            if (tipoCliente.ConvenioRemitente != null)
            {
                guia.IdSucursal = tipoCliente.ConvenioRemitente.IdSucursalRecogida.HasValue ? tipoCliente.ConvenioRemitente.IdSucursalRecogida.Value : 0;
                guia.IdContrato = tipoCliente.ConvenioRemitente.Contrato;
                guia.IdCliente = tipoCliente.ConvenioRemitente.Id;
                guia.IdListaPrecios = tipoCliente.ConvenioRemitente.IdListaPrecios;
            }
            return guia;
        }

        /// <summary>
        /// Retorna la información de una guía completa pero que haya sido generada del día de hoy (día en que se hace la solicitud)
        /// incluyendo la forma como se pagó, se construyó para generar impresión.
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="idCentroServicioOrigen">Id de la agencia uqe solicita la consulta</param>
        /// <returns></returns>
        public ADGuia ObtenerGuiaPorNumeroDeGuiaCompleta(long numeroGuia, long idCentroServicioOrigen)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                List<ADAdmisionNumGuiaHoy> guias = contexto.paObtenerAdmMenPorNumGuiaHoy_MEN(numeroGuia, idCentroServicioOrigen, ControllerContext.Current.Usuario).ToList();
                if (guias != null && guias.Count > 0)
                {
                    return new ADGuia()
                    {
                        CodigoPostalDestino = guias.First().ADM_CodigoPostalDestino,
                        CodigoPostalOrigen = guias.First().ADM_CodigoPostalOrigen,
                        CreadoPor = guias.First().ADM_CreadoPor,
                        Destinatario = new Servicios.ContratoDatos.Clientes.CLClienteContadoDC
                        {
                            Nombre = guias.First().ADM_NombreDestinatario,
                            Apellido1 = string.Empty,
                            Apellido2 = string.Empty,
                            Direccion = guias.First().ADM_DireccionDestinatario,
                            Identificacion = guias.First().ADM_IdDestinatario,
                            Telefono = guias.First().ADM_TelefonoDestinatario,
                            TipoId = guias.First().ADM_IdTipoIdentificacionDestinatario,
                            Email = guias.First().ADM_EmailDestinatario
                        },
                        DiasDeEntrega = guias.First().ADM_DiasDeEntrega,
                        DiceContener = guias.First().ADM_DiceContener,
                        EsAutomatico = guias.First().ADM_EsAutomatico,
                        FechaEstimadaEntrega = guias.First().ADM_FechaEstimadaEntrega,
                        FechaAdmision = guias.First().ADM_FechaAdmision,
                        IdCiudadOrigen = guias.First().ADM_IdCiudadOrigen,
                        IdCiudadDestino = guias.First().ADM_IdCiudadDestino,
                        FormasPago = guias.ConvertAll(f => new ADGuiaFormaPago { Descripcion = f.FOP_Descripcion, IdFormaPago = f.AGF_IdFormaPago }),
                        IdCentroServicioOrigen = guias.First().ADM_IdCentroServicioOrigen,
                        IdUnidadNegocio = guias.First().ADM_IdUnidadNegocio,
                        IdServicio = guias.First().ADM_IdServicio,
                        NombreCentroServicioDestino = guias.First().ADM_NombreCentroServicioDestino,
                        NombreCentroServicioOrigen = guias.First().ADM_NombreCentroServicioOrigen,
                        NombreCiudadDestino = guias.First().ADM_NombreCiudadDestino,
                        NombreCiudadOrigen = guias.First().ADM_NombreCiudadOrigen,
                        NombreServicio = guias.First().ADM_NombreServicio,
                        NombreTipoEnvio = guias.First().ADM_NombreTipoEnvio,
                        NumeroBolsaSeguridad = guias.First().ADM_NumeroBolsaSeguridad,
                        MotivoNoUsoBolsaSeguriDesc = guias.First().ADM_MotivoNoUsoBolsaSeguriDesc,
                        NumeroGuia = guias.First().ADM_NumeroGuia,
                        NumeroPieza = guias.First().ADM_NumeroPieza,
                        Observaciones = guias.First().ADM_Observaciones,
                        PesoLiqMasa = guias.First().ADM_PesoLiqMasa,
                        PesoLiqVolumetrico = guias.First().ADM_PesoLiqVolumetrico,
                        Peso = guias.First().ADM_Peso,
                        PrefijoNumeroGuia = guias.First().ADM_PrefijoNumeroGuia,
                        Remitente = new Servicios.ContratoDatos.Clientes.CLClienteContadoDC
                        {
                            Nombre = guias.First().ADM_NombreRemitente,
                            Apellido1 = string.Empty,
                            Apellido2 = string.Empty,
                            Direccion = guias.First().ADM_DireccionRemitente,
                            Identificacion = guias.First().ADM_IdRemitente,
                            Telefono = guias.First().ADM_TelefonoRemitente,
                            TipoId = guias.First().ADM_IdTipoIdentificacionRemitente,
                            Email = guias.First().ADM_EmailRemitente
                        },
                        TipoCliente = (ADEnumTipoCliente)Enum.Parse(typeof(ADEnumTipoCliente), guias.First().ADM_TipoCliente),
                        TotalPiezas = guias.First().ADM_TotalPiezas,
                        ValorAdicionales = guias.First().ADM_ValorAdicionales,
                        ValorAdmision = guias.First().ADM_ValorAdmision,
                        ValorDeclarado = guias.First().ADM_ValorDeclarado,
                        ValorPrimaSeguro = guias.First().ADM_ValorPrimaSeguro,
                        ValorServicio = guias.First().ADM_ValorAdmision,
                        ValorTotal = guias.First().ADM_ValorTotal,
                        GuidDeChequeo = guias.First().ADM_GuidDeChequeo,
                        NumeroGuiaDHL = guias.First().NumeroGuiaDHL.Value,
                        IdAdmision = guias.First().ADM_IdAdminisionMensajeria,
                        EstaImpresa = guias.First().ADM_Impresa == null ? false : guias.First().ADM_Impresa.Value
                    };
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Obtiene el rapi envio contra pago
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <returns></returns>
        public ADRapiEnvioContraPagoDC ObtenerRapiEnvioContraPago(long idAdmision)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ADRapiEnvioContraPagoDC rapiEnvioContraPago = new ADRapiEnvioContraPagoDC();
                var rapiEnvio = contexto.paObtenerAdmRapiEnvioContraPago_MEN(idAdmision).FirstOrDefault();
                if (rapiEnvio != null)
                {
                    rapiEnvioContraPago.Apellido1Destinatario = rapiEnvio.ARP_Apellido1Destinatario;
                    rapiEnvioContraPago.Apellido2Destinatario = rapiEnvio.ARP_Apellido2Destinatario;
                    rapiEnvioContraPago.ValorARecaudar = rapiEnvio.ARP_ValorARecaudar;
                }
                return rapiEnvioContraPago;
            }
        }

        /// <summary>
        /// Obtiene las guias al cobro no pagas.
        /// </summary>
        /// <param name="numeroGuia">The numero guia.</param>
        /// <param name="fechaInicial">The fecha inicial.</param>
        /// <returns>Lista de Guias al Cobro sin pagar</returns>
        public List<ADGuiaAlCobro> ObtenerGuiasAlCobroNoPagas(int indicePagina, int registrosPorPagina, long numeroGuia, DateTime fechaInicial, DateTime fechaFinal, long idCentroServicio)
        {
            DateTime fechaI = Convert.ToDateTime(fechaInicial, cultura);
            DateTime fechaF = Convert.ToDateTime(fechaFinal, cultura);

            fechaI = new DateTime(fechaI.Year, fechaI.Month, fechaI.Day, 00, 00, 00);
            fechaF = new DateTime(fechaF.Year, fechaF.Month, fechaF.Day, 23, 59, 00);

            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                List<paObtenerGuiaAlCobPorFchaNoPgda_MEN_Result> GuiasNoPagas = contexto.paObtenerGuiaAlCobPorFchaNoPgda_MEN(indicePagina, registrosPorPagina, numeroGuia, fechaI,
                                                                              fechaF, idCentroServicio, (short)ADEnumEstadoGuia.Entregada).ToList();
                List<ADGuiaAlCobro> ltsGuia = new List<ADGuiaAlCobro>();

                if (GuiasNoPagas.Count > 0)
                {
                    ltsGuia = GuiasNoPagas
                                           .ToList()
                                           .ConvertAll<ADGuiaAlCobro>(guia => new ADGuiaAlCobro
                                            {
                                                Guia = new ADGuia()
                                                                      {
                                                                          IdAdmision = guia.ADM_IdAdminisionMensajeria,
                                                                          NumeroGuia = guia.ADM_NumeroGuia.Value,
                                                                          ValorTotal = guia.AGF_Valor.Value,

                                                                          FechaGrabacion = guia.ADM_FechaGrabacion.Value,
                                                                          EsAlCobro = guia.ADM_EsAlCobro.Value,
                                                                          EstaPagada = guia.ADM_EstaPagada.Value,
                                                                          FormasPago = new List<ADGuiaFormaPago>()
                                                            {
                                                              new ADGuiaFormaPago
                                                              {
                                                                IdFormaPago = guia.AGF_IdFormaPago.Value, Descripcion = guia.FOP_Descripcion
                                                              }
                                                            },
                                                                          Destinatario = new CLClienteContadoDC
                                                                          {
                                                                              Nombre = guia.ADM_NombreDestinatario
                                                                          }
                                                                      },
                                                SePuedeDescargar = guia.ADM_EsAlCobro.Value && !guia.ADM_EstaPagada.Value
                                            }).ToList();
                }

                return ltsGuia;
            }
        }

        /// <summary>
        /// Método para obtener una guía de temelercado con sus respectivos valores adicionales
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ADGuia ObtenerGuiaTelemercadeo(long numeroGuia)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ADGuia guiaRetorna = new ADGuia();
                var guiaTelemercadeo = contexto.paObtenerGuiaValorAdicional_MEN(numeroGuia).FirstOrDefault();
                if (guiaTelemercadeo != null)
                {
                    guiaRetorna.Destinatario = new Servicios.ContratoDatos.Clientes.CLClienteContadoDC
                   {
                       Identificacion = guiaTelemercadeo.ADM_IdDestinatario,
                       Nombre = guiaTelemercadeo.ADM_NombreDestinatario,
                       Direccion = guiaTelemercadeo.ADM_DireccionDestinatario,
                       Telefono = guiaTelemercadeo.ADM_TelefonoDestinatario,
                   };
                    guiaRetorna.NombreServicio = guiaTelemercadeo.ADM_NombreServicio;
                    guiaRetorna.IdAdmision = guiaTelemercadeo.ADM_IdAdminisionMensajeria;
                    guiaRetorna.DiceContener = guiaTelemercadeo.ADM_DiceContener;
                    guiaRetorna.FechaAdmision = guiaTelemercadeo.ADM_FechaAdmision;
                    guiaRetorna.IdUnidadNegocio = guiaTelemercadeo.ADM_IdUnidadNegocio;
                    guiaRetorna.NombreCentroServicioDestino = guiaTelemercadeo.ADM_NombreCentroServicioDestino;
                    guiaRetorna.NombreCentroServicioOrigen = guiaTelemercadeo.ADM_NombreCentroServicioOrigen;
                    guiaRetorna.NombreCiudadDestino = guiaTelemercadeo.ADM_NombreCiudadDestino;
                    guiaRetorna.NombreCiudadOrigen = guiaTelemercadeo.ADM_NombreCiudadOrigen;
                    guiaRetorna.NombreTipoEnvio = guiaTelemercadeo.ADM_NombreTipoEnvio;
                    guiaRetorna.NumeroGuia = guiaTelemercadeo.ADM_NumeroGuia;
                    guiaRetorna.Observaciones = guiaTelemercadeo.ADM_Observaciones;
                    guiaRetorna.PrefijoNumeroGuia = guiaTelemercadeo.ADM_PrefijoNumeroGuia;
                    guiaRetorna.FechaGrabacion = guiaTelemercadeo.ADM_FechaAdmision;
                    guiaRetorna.CreadoPor = guiaTelemercadeo.ADM_CreadoPor;
                    guiaRetorna.Remitente = new Servicios.ContratoDatos.Clientes.CLClienteContadoDC
                    {
                        Identificacion = guiaTelemercadeo.ADM_IdRemitente,
                        Nombre = guiaTelemercadeo.ADM_NombreRemitente,
                        Direccion = guiaTelemercadeo.ADM_DireccionRemitente,
                        Telefono = guiaTelemercadeo.ADM_TelefonoRemitente,
                    };
                    guiaRetorna.ValorAdmision = guiaTelemercadeo.ADM_ValorAdmision;
                    guiaRetorna.ValorServicio = guiaTelemercadeo.ADM_ValorAdmision;
                    guiaRetorna.ValorTotal = guiaTelemercadeo.ADM_ValorTotal;
                    guiaRetorna.ValoresAdicionales = new List<TAValorAdicional>();

                    if (!string.IsNullOrEmpty(guiaTelemercadeo.AVA_Descripcion))
                    {
                        guiaRetorna.ValoresAdicionales.Add(new TAValorAdicional
                        {
                            Descripcion = guiaTelemercadeo.AVA_Descripcion,
                            PrecioValorAdicional = guiaTelemercadeo.AVA_Valor.Value,
                        });
                    }

                    guiaRetorna.FormasPago = new List<ADGuiaFormaPago>();

                    if (!string.IsNullOrEmpty(guiaTelemercadeo.FOP_Descripcion))
                    {
                        guiaRetorna.FormasPago.Add
                        (new ADGuiaFormaPago
                        {
                            Descripcion = guiaTelemercadeo.FOP_Descripcion,
                            IdFormaPago = guiaTelemercadeo.AGF_IdFormaPago.Value,
                            Valor = guiaTelemercadeo.AGF_Valor.Value
                        }
                        );
                    }
                }
                else
                {
                    return null;
                }

                return guiaRetorna;
            }
        }

        /// <summary>
        /// Método para obtener un rango de guías internas
        /// </summary>
        /// <param name="numeroInicial"></param>
        /// <param name="numeroFinal"></param>
        /// <returns></returns>
        public List<ADGuiaInternaDC> ObtenerGuiasInternas(long numeroInicial, long numeroFinal, List<long> listaNumerosGuias)
        {
            List<ADGuiaInternaDC> listaGuiasInternas = new List<ADGuiaInternaDC>();

            if (listaNumerosGuias.Any())
            {
                listaNumerosGuias.ForEach(g =>
                {

                    using (SqlConnection conn = new SqlConnection(conexionStringController))
                    {
                        SqlCommand cmd = new SqlCommand("paObtenerRangoGuiaInterna_MEN", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@NumeroInicial", g);
                        cmd.Parameters.AddWithValue("@NumeroFinal", g);
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();

                        conn.Open();
                        da.Fill(dt);
                        conn.Close();

                        var Guia = dt.AsEnumerable().ToList().FirstOrDefault();
                        if (Guia != null)
                        {
                            ADGuiaInternaDC guia = new ADGuiaInternaDC()
                            {
                                NumeroGuia = Guia.Field<long>("ADM_NumeroGuia"),
                                CreadoPor = Guia.Field<string>("AGI_CreadoPor"),
                                DireccionDestinatario = Guia.Field<string>("AGI_DireccionDestinatario"),
                                DireccionRemitente = Guia.Field<string>("AGI_DireccionRemitente"),
                                FechaGrabacion = Guia.Field<DateTime>("AGI_FechaGrabacion"),
                                IdAdmisionGuia = Guia.Field<long>("AGI_IdAdmisionMensajeria"),
                                LocalidadDestino = new PALocalidadDC
                                {
                                    IdLocalidad = Guia.Field<string>("ADM_IdCiudadDestino"),
                                    Nombre = Guia.Field<string>("ADM_NombreCiudadDestino"),
                                    NombreCompleto = Guia.Field<string>("CiudadDestinoNombreCompleto"),

                                },
                                LocalidadOrigen = new PALocalidadDC
                                {
                                    IdLocalidad = Guia.Field<string>("ADM_IdCiudadOrigen"),
                                    Nombre = Guia.Field<string>("ADM_NombreCiudadOrigen"),
                                    NombreCompleto = Guia.Field<string>("CiudadOrigenNombreCompleto"),
                                },
                                GestionOrigen = new ARGestionDC
                                {
                                    IdGestion = Guia.Field<long>("AGI_IdGestionOrigen"),
                                    Descripcion = Guia.Field<string>("AGI_DescripcionGestionOrig")
                                },
                                GestionDestino = new ARGestionDC
                                {
                                    IdGestion = Guia.Field<long>("AGI_IdGestionDestino"),
                                    Descripcion = Guia.Field<string>("AGI_DescripcionGestionDest")
                                },
                                DiceContener = Guia["AGI_Contenido"] == DBNull.Value ? string.Empty : (Guia.Field<string>("AGI_Contenido")),
                                NombreRemitente = Guia.Field<string>("AGI_NombreRemitente"),
                                NombreDestinatario = Guia.Field<string>("AGI_NombreDestinatario"),
                                TelefonoRemitente = Guia.Field<string>("AGI_TelefonoRemitente"),
                                TelefonoDestinatario = Guia.Field<string>("AGI_TelefonoDestinatario"),
                                TiempoEntrega = Guia.Field<short>("ADM_DiasDeEntrega"),
                                IdentificacionDestinatario = Guia.Field<string>("ADM_IdDestinatario"),
                                IdentificacionRemitente = Guia.Field<string>("ADM_IdRemitente"),
                                TipoIdentificacionRemitente = Guia.Field<string>("ADM_IdTipoIdentificacionRemitente"),
                                TipoIdentificacionDestinatario = Guia.Field<string>("ADM_IdTipoIdentificacionDestinatario"),
                                EmailRemitente = Guia.Field<string>("ADM_EmailRemitente"),
                                EmailDestinatario = Guia.Field<string>("ADM_EmailDestinatario"),
                                Observaciones = Guia.Field<string>("ADM_Observaciones"),
                                FechaEstimadaEntrega = Guia.Field<DateTime>("ADM_FechaEstimadaEntrega")

                            };

                            listaGuiasInternas.Add(guia);
                        }



                    }

                });
            }
            else
            {


                using (SqlConnection conn = new SqlConnection(conexionStringController))
                {
                    SqlCommand cmd = new SqlCommand("paObtenerRangoGuiaInterna_MEN", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@NumeroInicial", numeroInicial);
                    cmd.Parameters.AddWithValue("@NumeroFinal", numeroFinal);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();

                    conn.Open();
                    da.Fill(dt);
                    conn.Close();

                    listaGuiasInternas = dt.AsEnumerable().ToList().ConvertAll<ADGuiaInternaDC>(Guia =>
                    {

                        ADGuiaInternaDC g = new ADGuiaInternaDC()
                        {
                            NumeroGuia = Guia.Field<long>("ADM_NumeroGuia"),
                            CreadoPor = Guia.Field<string>("AGI_CreadoPor"),
                            DireccionDestinatario = Guia.Field<string>("AGI_DireccionDestinatario"),
                            DireccionRemitente = Guia.Field<string>("AGI_DireccionRemitente"),
                            FechaGrabacion = Guia.Field<DateTime>("AGI_FechaGrabacion"),
                            IdAdmisionGuia = Guia.Field<long>("AGI_IdAdmisionMensajeria"),
                            LocalidadDestino = new PALocalidadDC
                            {
                                IdLocalidad = Guia.Field<string>("ADM_IdCiudadDestino"),
                                Nombre = Guia.Field<string>("ADM_NombreCiudadDestino")
                            },
                            LocalidadOrigen = new PALocalidadDC
                            {
                                IdLocalidad = Guia.Field<string>("ADM_IdCiudadOrigen"),
                                Nombre = Guia.Field<string>("ADM_NombreCiudadOrigen")
                            },
                            GestionOrigen = new ARGestionDC
                            {
                                IdGestion = Guia.Field<long>("AGI_IdGestionOrigen"),
                                Descripcion = Guia.Field<string>("AGI_DescripcionGestionOrig")
                            },
                            GestionDestino = new ARGestionDC
                            {
                                IdGestion = Guia.Field<long>("AGI_IdGestionDestino"),
                                Descripcion = Guia.Field<string>("AGI_DescripcionGestionDest")
                            },
                            DiceContener = Guia["AGI_Contenido"] == DBNull.Value ? string.Empty : (Guia.Field<string>("AGI_Contenido")),
                            NombreRemitente = Guia.Field<string>("AGI_NombreRemitente"),
                            NombreDestinatario = Guia.Field<string>("AGI_NombreDestinatario"),
                            TelefonoRemitente = Guia.Field<string>("AGI_TelefonoRemitente"),
                            TelefonoDestinatario = Guia.Field<string>("AGI_TelefonoDestinatario"),
                            TiempoEntrega = Guia.Field<short>("ADM_DiasDeEntrega"),
                            IdentificacionDestinatario = Guia.Field<string>("ADM_IdDestinatario"),
                            IdentificacionRemitente = Guia.Field<string>("ADM_IdRemitente"),
                            TipoIdentificacionRemitente = Guia.Field<string>("ADM_IdTipoIdentificacionRemitente"),
                            TipoIdentificacionDestinatario = Guia.Field<string>("ADM_IdTipoIdentificacionDestinatario"),
                            EmailRemitente = Guia.Field<string>("ADM_EmailRemitente"),
                            EmailDestinatario = Guia.Field<string>("ADM_EmailDestinatario"),
                            FechaEstimadaEntrega = Guia.Field<DateTime>("ADM_FechaEstimadaEntrega")

                        };

                        return g;
                    });


                }
            }
            return listaGuiasInternas;

        }

        /// <summary>
        /// Método para obtener una guía interna
        /// </summary>
        /// <param name="numeroInicial"></param>
        /// <param name="numeroFinal"></param>
        /// <returns></returns>
        public ADGuiaInternaDC ObtenerGuiaInterna(long numeroGuia)
        {

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerRangoGuiaInterna_MEN", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroInicial", numeroGuia);
                cmd.Parameters.AddWithValue("@NumeroFinal", numeroGuia);

                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                conn.Open();
                da.Fill(dt);
                conn.Close();

                var guiaInterna = dt.AsEnumerable().ToList().FirstOrDefault();

                if (guiaInterna != null)
                {
                    return new ADGuiaInternaDC()
               {
                   NumeroGuia = guiaInterna.Field<long>("ADM_NumeroGuia"),
                   CreadoPor = guiaInterna.Field<string>("AGI_CreadoPor"),
                   DireccionDestinatario = guiaInterna.Field<string>("AGI_DireccionDestinatario"),
                   DireccionRemitente = guiaInterna.Field<string>("AGI_DireccionRemitente"),
                   FechaGrabacion = guiaInterna.Field<DateTime>("AGI_FechaGrabacion"),
                   IdAdmisionGuia = guiaInterna.Field<long>("AGI_IdAdmisionMensajeria"),
                   LocalidadDestino = new PALocalidadDC
                   {
                       IdLocalidad = guiaInterna.Field<string>("ADM_IdCiudadDestino"),
                       Nombre = guiaInterna.Field<string>("ADM_NombreCiudadDestino")
                   },
                   LocalidadOrigen = new PALocalidadDC
                   {
                       IdLocalidad = guiaInterna.Field<string>("ADM_IdCiudadOrigen"),
                       Nombre = guiaInterna.Field<string>("ADM_NombreCiudadOrigen")
                   },
                   GestionOrigen = new ARGestionDC
                   {
                       IdGestion = guiaInterna.Field<long>("AGI_IdGestionOrigen"),
                       Descripcion = guiaInterna.Field<string>("AGI_DescripcionGestionOrig")
                   },
                   GestionDestino = new ARGestionDC
                   {
                       IdGestion = guiaInterna.Field<long>("AGI_IdGestionDestino"),
                       Descripcion = guiaInterna.Field<string>("AGI_DescripcionGestionDest")
                   },
                   DiceContener = guiaInterna["AGI_Contenido"] == DBNull.Value ? string.Empty : (guiaInterna.Field<string>("AGI_Contenido")),

                   NombreRemitente = guiaInterna.Field<string>("AGI_NombreRemitente"),
                   NombreDestinatario = guiaInterna.Field<string>("AGI_NombreDestinatario"),
                   TelefonoRemitente = guiaInterna.Field<string>("AGI_TelefonoRemitente"),
                   TelefonoDestinatario = guiaInterna.Field<string>("AGI_TelefonoDestinatario"),
                   IdCentroServicioDestino = guiaInterna.Field<long>("ADM_IdCentroServicioDestino"),
                   IdCentroServicioOrigen = guiaInterna.Field<long>("ADM_IdCentroServicioOrigen"),
                   NombreCentroServicioDestino = guiaInterna.Field<string>("ADM_NombreCentroServicioDestino"),
                   NombreCentroServicioOrigen = guiaInterna.Field<string>("ADM_NombreCentroServicioOrigen"),
                   IdentificacionDestinatario = guiaInterna.Field<string>("ADM_IdDestinatario"),
                   IdentificacionRemitente = guiaInterna.Field<string>("ADM_IdRemitente")
               };

                }
                else
                    return null;

            }
        }

        /// <summary>
        /// Método para obtener una guía interna a partir de un numero de guia, si no existe la guia genere excepción
        /// </summary>
        /// <param name="numeroInicial"></param>
        /// <param name="numeroFinal"></param>
        /// <returns></returns>
        public ADGuiaInternaDC ObtenerGuiaInternaNumeroGuia(long numeroGuia)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var guiaInterna = contexto.paObtenerRangoGuiaInterna_MEN(numeroGuia, numeroGuia).FirstOrDefault();

                if (guiaInterna != null)
                {
                    return new ADGuiaInternaDC()
                    {
                        NumeroGuia = guiaInterna.ADM_NumeroGuia,
                        CreadoPor = guiaInterna.AGI_CreadoPor,
                        DireccionDestinatario = guiaInterna.AGI_DireccionDestinatario,
                        DireccionRemitente = guiaInterna.AGI_DireccionRemitente,
                        FechaGrabacion = guiaInterna.AGI_FechaGrabacion,
                        IdAdmisionGuia = guiaInterna.AGI_IdAdmisionMensajeria,
                        LocalidadDestino = new PALocalidadDC
                        {
                            IdLocalidad = guiaInterna.ADM_IdCiudadDestino,
                            Nombre = guiaInterna.ADM_NombreCiudadDestino
                        },
                        LocalidadOrigen = new PALocalidadDC
                        {
                            IdLocalidad = guiaInterna.ADM_IdCiudadOrigen,
                            Nombre = guiaInterna.ADM_NombreCiudadOrigen
                        },
                        GestionOrigen = new ARGestionDC
                        {
                            IdGestion = guiaInterna.AGI_IdGestionOrigen,
                            Descripcion = guiaInterna.AGI_DescripcionGestionOrig
                        },
                        GestionDestino = new ARGestionDC
                        {
                            IdGestion = guiaInterna.AGI_IdGestionDestino,
                            Descripcion = guiaInterna.AGI_DescripcionGestionDest
                        },
                        DiceContener = string.IsNullOrEmpty(guiaInterna.AGI_Contenido) ? string.Empty : (guiaInterna.AGI_Contenido),
                        NombreRemitente = guiaInterna.AGI_NombreRemitente,
                        NombreDestinatario = guiaInterna.AGI_NombreDestinatario,
                        TelefonoRemitente = guiaInterna.AGI_TelefonoRemitente,
                        TelefonoDestinatario = guiaInterna.AGI_TelefonoDestinatario,
                        IdCentroServicioDestino = guiaInterna.ADM_IdCentroServicioDestino,
                        IdCentroServicioOrigen = guiaInterna.ADM_IdCentroServicioOrigen,
                        NombreCentroServicioDestino = guiaInterna.ADM_NombreCentroServicioDestino,
                        NombreCentroServicioOrigen = guiaInterna.ADM_NombreCentroServicioOrigen,
                    };
                }
                else
                {
                    throw new FaultException<ControllerException>
                         (new ControllerException(COConstantesModulos.MENSAJERIA, ADEnumTipoErrorMensajeria.EX_NUMERO_GUIA_NO_EXISTE.ToString(),
                         ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.EX_NUMERO_GUIA_NO_EXISTE)));
                }
            }
        }

        /// <summary>
        /// Obtiene los motivos de anulación de una guía
        /// </summary>
        /// <returns>Colección motivos</returns>
        public List<ADMotivoAnulacionDC> ObtenerMotivosAnulacion()
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                List<MotivoAnulacionGuia_MEN> motivos = contexto.MotivoAnulacionGuia_MEN.ToList();

                if (motivos != null)
                {
                    return motivos.ConvertAll(motivo => new ADMotivoAnulacionDC()
                    {
                        IdMotivoAnulacion = motivo.MAG_IdMotivoAnulaciónGuia,
                        Descripcion = motivo.MAG_Descripcion
                    });
                }
                else
                    return new List<ADMotivoAnulacionDC>();
            }
        }

        /// <summary>
        /// Obtener las notificaciones de una guia
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <returns></returns>
        public ADNotificacion ObtenerNotificacionGuia(long idAdmision)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                AdmisionNotificaciones_MEN notificaciones = contexto.AdmisionNotificaciones_MEN.Where(r => r.ADN_IdAdminisionMensajeria == idAdmision).FirstOrDefault();
                if (notificaciones != null)
                {
                    return new ADNotificacion
                    {
                        Apellido1Destinatario = notificaciones.ADN_Apellido1Destinatario,
                        Apellido2Destinatario = notificaciones.ADN_Apellido2Destinatario,
                        DireccionDestinatario = notificaciones.ADN_DireccionDestinatario,
                        EmailDestinatario = notificaciones.ADN_EmailDestinatario,
                        GuiaAdmision = new ADGuia { IdAdmision = notificaciones.ADN_IdAdminisionMensajeria },
                        CiudadDestino = new PALocalidadDC { IdLocalidad = notificaciones.ADN_IdCiudadDestino, Nombre = notificaciones.ADN_NombreCiudadDestino },
                        IdDestinatario = notificaciones.ADN_IdDestinatario,
                        TipoDestino = new TATipoDestino { Id = notificaciones.ADN_IdTipoDestino },
                        NombreDestinatario = notificaciones.ADN_NombreDestinatario,
                        GuiaInterna = new ADGuiaInternaDC { NumeroGuia = notificaciones.ADN_NumeroGuiaInterna, },
                        ReclamaEnOficina = notificaciones.ADN_ReclamaEnOficina,
                        TipoIdentificacionDestinatario = notificaciones.ADN_TipoIdDestinatario
                    };
                }
                else
                    return null;
            }
        }

        /// <summary>
        /// Método para obtener información de los rapiradicados asociados a una admision
        /// </summary>
        /// <returns></returns>
        public List<ADRapiRadicado> ObtenerRapiradicadosGuia(long numeroGuia)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var listaRadicados = contexto.paObtenerRapiradicadosGuia_MEN(numeroGuia).ToList();

                if (listaRadicados.Any())
                {
                    return listaRadicados.ConvertAll(radicado => new ADRapiRadicado()
                    {
                        NumeroFolios = radicado.ARR_NumeroFolios,
                        CodigoRapiRadicado = radicado.ARR_CodigoRapiRadicado,
                    });
                }
                else
                    return new List<ADRapiRadicado>();
            }
        }

        /// <summary>
        /// Método para obtener las guías de notificaciones
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <returns></returns>
        public List<ADNotificacion> ObtenerNotificacionesRecibido(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                string fechaInicial;
                string fechaFinal;
                string agenciaOrigen;
                string agenciaDestino;
                string nit;
                DateTime fechaI;
                DateTime fechaF;
                ObjectParameter totalRegistros = new ObjectParameter("TotalRegistros", typeof(int));

                filtro.TryGetValue("agenciaOrigen", out agenciaOrigen);
                filtro.TryGetValue("agenciaDestino", out agenciaDestino);
                filtro.TryGetValue("fechaInicial", out fechaInicial);
                filtro.TryGetValue("fechaFinal", out fechaFinal);
                filtro.TryGetValue("nit", out nit);

                if (fechaInicial == null)
                    fechaInicial = ConstantesFramework.MinDateTimeController.ToString();
                if (fechaFinal == null)
                    fechaFinal = DateTime.Now.ToString();

                fechaI = Convert.ToDateTime(fechaInicial, cultura);
                fechaF = Convert.ToDateTime(fechaFinal, cultura);

                fechaI = new DateTime(fechaI.Year, fechaI.Month, fechaI.Day, 00, 00, 00);
                fechaF = new DateTime(fechaF.Year, fechaF.Month, fechaF.Day, 23, 59, 00);

                var notificaciones = contexto.paObtenerNotificacionesRecibido_MEN(indicePagina, registrosPorPagina, Convert.ToInt64(agenciaOrigen), Convert.ToInt64(agenciaDestino), fechaI, fechaF, nit, true, totalRegistros).ToList();

                if (notificaciones.Any())
                {
                    return notificaciones.ConvertAll(r => new ADNotificacion
                    {
                        Apellido1Destinatario = r.ADN_Apellido1Destinatario,
                        Apellido2Destinatario = r.ADN_Apellido2Destinatario,
                        CiudadDestino = new PALocalidadDC
                        {
                            IdLocalidad = r.ADN_IdCiudadDestino,
                            Nombre = r.ADN_NombreCiudadDestino
                        },
                        DireccionDestinatario = r.ADM_DireccionDestinatario,
                        EmailDestinatario = r.ADN_EmailDestinatario,
                        GuiaAdmision = new ADGuia
                        {
                            IdAdmision = r.ADN_IdAdminisionMensajeria,
                            NumeroGuia = r.ADM_NumeroGuia,
                            IdCiudadOrigen = r.ADM_IdCiudadOrigen,
                            NombreCiudadOrigen = r.ADM_NombreCiudadOrigen,
                            IdCiudadDestino = r.ADM_IdCiudadDestino,
                            NombreCiudadDestino = r.ADM_NombreCiudadDestino,
                            IdServicio = r.ADM_IdServicio,
                            IdCentroServicioOrigen = r.ADM_IdCentroServicioOrigen,
                            NombreCentroServicioOrigen = r.ADM_NombreCentroServicioOrigen,
                            IdCentroServicioDestino = r.ADM_IdCentroServicioDestino,
                            NombreCentroServicioDestino = r.ADM_NombreCentroServicioDestino,
                            NombreServicio = r.ADM_NombreServicio,
                            TelefonoDestinatario = r.ADM_TelefonoDestinatario,
                            DireccionDestinatario = r.ADM_DireccionDestinatario,
                        },
                        GuiaInterna = new ADGuiaInternaDC
                        {
                            NumeroGuia = r.ADN_NumeroGuiaInterna
                        },
                        IdDestinatario = r.ADN_IdDestinatario,
                        NombreDestinatario = r.ADN_NombreDestinatario,
                        PaisDestino = new PALocalidadDC
                        {
                            IdLocalidad = r.ADM_IdPaisDestino,
                            Nombre = r.ADM_NombrePaisDestino
                        },
                        RecibidoGuia = new LIRecibidoGuia
                        {
                            FechaEntrega = r.REG_FechaEntrega ?? ConstantesFramework.MinDateTimeController,
                            Identificacion = r.REG_Identificacion ?? string.Empty,
                            IdGuia = r.ADN_IdAdminisionMensajeria,
                            NumeroGuia = r.ADM_NumeroGuia,
                            Otros = r.REG_OtrosDatos ?? string.Empty,
                            RecibidoPor = r.REG_RecibidoPor ?? string.Empty,
                            Telefono = r.REG_Telefono ?? string.Empty
                        },
                        ReclamaEnOficina = r.ADN_ReclamaEnOficina,
                        TelefonoDestinatario = r.ADN_TelefonoDestinatario,
                        TipoDestino = new TATipoDestino
                        {
                            Id = r.ADN_IdTipoDestino,
                            Descripcion = r.ADN_TipoDestino,
                        },
                        TipoIdentificacionDestinatario = r.ADN_TipoIdDestinatario,
                        EstadoGuia = new ADEstadoGuia
                        {
                            Descripcion = ((ADEnumEstadoGuia)(short)r.EGT_IdEstadoGuia).ToString(),
                            Id = (short)r.EGT_IdEstadoGuia,
                        },
                        TotalRegistros = (int)totalRegistros.Value,
                    });
                }
                else
                    return new List<ADNotificacion>();
            }
        }

        /// <summary>
        /// Método para obtener los id de las guías de notificaciones
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <returns></returns>
        public List<long> ObtenerIdNotificaciones(IDictionary<string, string> filtro)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                string fechaInicial;
                string fechaFinal;
                string agenciaOrigen;
                string agenciaDestino;
                string nit;
                DateTime fechaI;
                DateTime fechaF;

                List<long> listaNotificaciones = new List<long>();

                filtro.TryGetValue("agenciaOrigen", out agenciaOrigen);
                filtro.TryGetValue("agenciaDestino", out agenciaDestino);
                filtro.TryGetValue("fechaInicial", out fechaInicial);
                filtro.TryGetValue("fechaFinal", out fechaFinal);
                filtro.TryGetValue("nit", out nit);

                if (fechaInicial == null)
                    fechaInicial = ConstantesFramework.MinDateTimeController.ToString();
                if (fechaFinal == null)
                    fechaFinal = DateTime.Now.ToString();

                fechaI = Convert.ToDateTime(fechaInicial, cultura);
                fechaF = Convert.ToDateTime(fechaFinal, cultura);

                fechaI = new DateTime(fechaI.Year, fechaI.Month, fechaI.Day, 00, 00, 00);
                fechaF = new DateTime(fechaF.Year, fechaF.Month, fechaF.Day, 23, 59, 00);

                var notificaciones = contexto.paObtenerIdNotificaciones_MEN(Convert.ToInt64(agenciaOrigen), Convert.ToInt64(agenciaDestino), fechaI, fechaF, nit).ToList();

                if (notificaciones.Any())
                {
                    notificaciones.ForEach(j =>
                    {
                        listaNotificaciones.Add(j.ADN_NumeroGuiaInterna);
                    });
                }

                return listaNotificaciones;
            }
        }

        /// <summary>
        /// Método para obtener las guías de servicio rapiradicado
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        public List<ADRapiRadicado> ObtenerGuiasRapiradicados(IDictionary<string, string> filtro)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                string fechaInicial;
                string fechaFinal;
                string agencia;
                string nit;
                DateTime fechaI;
                DateTime fechaF;
                List<ADRapiRadicado> listaRetorna = new List<ADRapiRadicado>();
                ADRapiRadicado radicado;

                filtro.TryGetValue("agencia", out agencia);
                filtro.TryGetValue("fechaInicial", out fechaInicial);
                filtro.TryGetValue("fechaFinal", out fechaFinal);
                filtro.TryGetValue("nit", out nit);

                if (fechaInicial == null)
                    fechaInicial = ConstantesFramework.MinDateTimeController.ToString();
                if (fechaFinal == null)
                    fechaFinal = DateTime.Now.ToString();

                fechaI = Convert.ToDateTime(fechaInicial, Thread.CurrentThread.CurrentCulture);
                fechaF = Convert.ToDateTime(fechaFinal, Thread.CurrentThread.CurrentCulture);

                fechaI = new DateTime(fechaI.Year, fechaI.Month, fechaI.Day, 00, 00, 00);
                fechaF = new DateTime(fechaF.Year, fechaF.Month, fechaF.Day, 23, 59, 00);

                var radicados = contexto.paObtenerAdmisionRapiradicados_MEN(Convert.ToInt64(agencia), fechaI, fechaF, (short)ADEnumEstadoGuia.Entregada, nit).ToList();

                if (radicados.Any())
                {
                    radicados.ForEach(r =>
                        {
                            radicado = new ADRapiRadicado
                            {
                                Apellido1Destinatario = r.ARR_Apellido1Destinatario,
                                Apellido2Destinatario = r.ARR_Apellido2Destinatario,
                                CiudadDestino = new PALocalidadDC
                                {
                                    IdLocalidad = r.ARR_IdCiudadDestino,
                                    Nombre = r.ARR_NombreCiudadDestino
                                },
                                CodigoRapiRadicado = r.ARR_CodigoRapiRadicado,
                                DireccionDestinatario = r.ARR_DireccionDestinatario,
                                EmailDestinatario = r.ARR_EmailDestinatario,
                                NumeroGuiaInterna = r.ARR_NumeroGuiaInterna,
                                GuiaAdmision = new ADGuia
                                {
                                    IdAdmision = r.ARR_IdAdminisionMensajeria,
                                    NumeroGuia = r.ADM_NumeroGuia,
                                    IdCiudadOrigen = r.ADM_IdCiudadOrigen,
                                    NombreCiudadOrigen = r.ADM_NombreCiudadOrigen,
                                    IdCiudadDestino = r.ADM_IdCiudadDestino,
                                    NombreCiudadDestino = r.ADM_NombreCiudadDestino,
                                    IdServicio = r.ADM_IdServicio,
                                    IdCentroServicioOrigen = r.ADM_IdCentroServicioOrigen,
                                    NombreCentroServicioOrigen = r.ADM_NombreCentroServicioOrigen,
                                    IdCentroServicioDestino = r.ADM_IdCentroServicioDestino,
                                    NombreCentroServicioDestino = r.ADM_NombreCentroServicioDestino,
                                    NombreServicio = r.ADM_NombreServicio,
                                    TelefonoDestinatario = r.ADM_TelefonoDestinatario,
                                    DireccionDestinatario = r.ADM_DireccionDestinatario,
                                },
                                IdRapiradicado = r.ARR_IdAdminisionRapiRadicado,
                                IdDestinatario = r.ARR_IdDestinatario,
                                NombreDestinatario = r.ARR_NombreDestinatario,
                                NumeroFolios = r.ARR_NumeroFolios,
                                PaisDestino = new PALocalidadDC
                                {
                                    IdLocalidad = r.ADM_IdPaisDestino,
                                    Nombre = r.ADM_NombrePaisDestino
                                },
                                TelefonoDestinatario = r.ARR_TelefonoDestinatario,
                                TipoDestino = new TATipoDestino
                                {
                                    Id = r.ARR_TipoIdDestinatario,
                                    Descripcion = r.ARR_TipoDestino,
                                },
                                TipoIdentificacionDestinatario = r.ARR_TipoIdDestinatario,
                            };
                            if (r.ADM_NumeroGuia != 0)
                                radicado.GuiaInterna = ObtenerGuiaInterna(r.ARR_NumeroGuiaInterna);
                            listaRetorna.Add(radicado);
                        });
                }
                return listaRetorna;
            }
        }

        /// <summary>
        /// Método para obtener la información de una guía rapiradicado
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ADRapiRadicado ObtenerAdmisionRapiradicado(long numeroGuia)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var radicado = contexto.paObtenerAdmRapiradicado_MEN(numeroGuia).FirstOrDefault();

                if (radicado == null)
                {
                    throw new FaultException<ControllerException>
                        (new ControllerException(COConstantesModulos.MENSAJERIA, ADEnumTipoErrorMensajeria.EX_GUIA_NO_EXISTE.ToString(),
                        ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.EX_GUIA_NO_EXISTE)));
                }
                else
                {
                    return new ADRapiRadicado
                    {
                        Apellido1Destinatario = radicado.ARR_Apellido1Destinatario,
                        Apellido2Destinatario = radicado.ARR_Apellido2Destinatario,
                        CiudadDestino = new PALocalidadDC
                        {
                            IdLocalidad = radicado.ARR_IdCiudadDestino,
                            Nombre = radicado.ARR_NombreCiudadDestino
                        },
                        CodigoRapiRadicado = radicado.ARR_CodigoRapiRadicado,
                        DireccionDestinatario = radicado.ARR_DireccionDestinatario,
                        EmailDestinatario = radicado.ARR_EmailDestinatario,
                        NumeroGuiaInterna = radicado.ARR_NumeroGuiaInterna,
                        GuiaAdmision = new ADGuia
                        {
                            IdAdmision = radicado.ARR_IdAdminisionMensajeria,
                            NumeroGuia = radicado.ADM_NumeroGuia,
                            IdCiudadOrigen = radicado.ADM_IdCiudadOrigen,
                            NombreCiudadOrigen = radicado.ADM_NombreCiudadOrigen,
                            IdCiudadDestino = radicado.ADM_IdCiudadDestino,
                            NombreCiudadDestino = radicado.ADM_NombreCiudadDestino,
                            IdServicio = radicado.ADM_IdServicio,
                            IdCentroServicioOrigen = radicado.ADM_IdCentroServicioOrigen,
                            NombreCentroServicioOrigen = radicado.ADM_NombreCentroServicioOrigen,
                            IdCentroServicioDestino = radicado.ADM_IdCentroServicioDestino,
                            NombreCentroServicioDestino = radicado.ADM_NombreCentroServicioDestino,
                            NombreServicio = radicado.ADM_NombreServicio,
                            TelefonoDestinatario = radicado.ADM_TelefonoDestinatario,
                            DireccionDestinatario = radicado.ADM_DireccionDestinatario,
                        },
                        IdRapiradicado = radicado.ARR_IdAdminisionRapiRadicado,
                        IdDestinatario = radicado.ARR_IdDestinatario,
                        NombreDestinatario = radicado.ARR_NombreDestinatario + ' ' + radicado.ARR_Apellido1Destinatario,
                        NumeroFolios = radicado.ARR_NumeroFolios,
                        PaisDestino = new PALocalidadDC
                        {
                            IdLocalidad = radicado.ADM_IdPaisDestino,
                            Nombre = radicado.ADM_NombrePaisDestino
                        },
                        TelefonoDestinatario = radicado.ARR_TelefonoDestinatario,
                        TipoDestino = new TATipoDestino
                        {
                            Id = radicado.ARR_TipoIdDestinatario,
                            Descripcion = radicado.ARR_TipoDestino,
                        },
                        TipoIdentificacionDestinatario = radicado.ARR_TipoIdDestinatario,
                    };
                }
            }
        }


        public List<ADGuia> ObtenerAdmisionMensajeriaSinEntregar(AdEnvioNNFiltro envioNNFiltro)
        {
            List<ADGuia> guias = new List<ADGuia>();

            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                              
                SqlCommand cmd = new SqlCommand("paObtenerAdmisionMensajeriaSinEntregar_MEN", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 600;
                cmd.Parameters.AddWithValue("@FechaIncial", envioNNFiltro.FechaInicio);
                cmd.Parameters.AddWithValue("@FechaFinal", envioNNFiltro.FechaFin);
                cmd.Parameters.AddWithValue("@idCiudadOrigen", envioNNFiltro.CiudadOrigen);
                cmd.Parameters.AddWithValue("@idCiudadDestino", envioNNFiltro.CiudadDestino);
                cmd.Parameters.AddWithValue("@nombreRemitente", envioNNFiltro.NombreRemitente);
                cmd.Parameters.AddWithValue("@nombreDestinatario", envioNNFiltro.NombreDestinatario);
                cmd.Parameters.AddWithValue("@AMN_IdRemitente", envioNNFiltro.IdentificacionRemitente);
                cmd.Parameters.AddWithValue("@AMN_IdDestinatario", envioNNFiltro.IdentificacionDestinatario);
                cmd.Parameters.AddWithValue("@AMN_DiceContener", envioNNFiltro.DiceContener);
                cmd.Parameters.AddWithValue("@AMN_TelefonoDestinatario", envioNNFiltro.TelefonoDestinatario);
                cmd.Parameters.AddWithValue("@AMN_EmailRemitente", envioNNFiltro.CorreoRemitente);
                cmd.Parameters.AddWithValue("@AMN_NumeroBolsaSeguridad", envioNNFiltro.NumeroBolsaSeguridad);

                cmd.Parameters.AddWithValue("@AMN_TelefonoRemitente", envioNNFiltro.TelefonoRemitente);
                cmd.Parameters.AddWithValue("@AMN_DireccionDestinatario", envioNNFiltro.DireccionRemitente);
                cmd.Parameters.AddWithValue("@AMN_DireccionRemitente", envioNNFiltro.DireccionRemitente);
                cmd.Parameters.AddWithValue("@AMN_EmailDestinatario", envioNNFiltro.CorreoDestinatario);
                
                cmd.Parameters.AddWithValue("@AMN_IdOperativo", envioNNFiltro.IdOperativo);
                cmd.Parameters.AddWithValue("@AMN_CantidadPiezas", envioNNFiltro.CantidadPiezas);

                cmd.Parameters.AddWithValue("@NRegistros", envioNNFiltro.NRegistros);
                cmd.Parameters.AddWithValue("@Pagina", envioNNFiltro.Pagina);
                cmd.Parameters.AddWithValue("@Filtro", envioNNFiltro.Filtro);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sqlConn.Open();
                da.Fill(dt);
                sqlConn.Close();

                List<ADGuia> lstGuias = dt.AsEnumerable().ToList().ConvertAll<ADGuia>(row => 
                {
                    ADGuia guia = new ADGuia();

                            guia.IdAdmision = row["IdAdminisionMensajeria"] == DBNull.Value ? 0 : Convert.ToInt64(row["IdAdminisionMensajeria"]);
                            guia.NumeroGuia = row["NumeroGuia"] == DBNull.Value ? 0 : Convert.ToInt64(row["NumeroGuia"]);
                            guia.IdCentroServicioOrigen = row["IdCentroServicioOrigen"] == DBNull.Value ? 0 : Convert.ToInt64(row["IdCentroServicioOrigen"]);
                            guia.IdCentroServicioDestino = row["IdCentroServicioDestino"] == DBNull.Value ? 0 : Convert.ToInt64(row["IdCentroServicioDestino"]);
                            guia.NombreCentroServicioOrigen = row["NombreCentroServicioOrigen"] == DBNull.Value ? string.Empty : row["NombreCentroServicioOrigen"].ToString();
                            guia.NombreCentroServicioDestino = row["NombreCentroServicioDestino"] == DBNull.Value ? string.Empty : row["NombreCentroServicioDestino"].ToString();
                            guia.TelefonoDestinatario = row["TelefonoDestinatario"] == DBNull.Value ? string.Empty : row["TelefonoDestinatario"].ToString();
                            guia.DireccionDestinatario = row["DireccionDestinatario"] == DBNull.Value ? string.Empty : row["DireccionDestinatario"].ToString();
                            guia.DiceContener = row["DiceContener"] == DBNull.Value ? string.Empty : row["DiceContener"].ToString();
                            guia.Remitente = new CLClienteContadoDC
                            {
                                Nombre = row["NombreRemitente"] == DBNull.Value ? string.Empty : row["NombreRemitente"].ToString(),
                                Telefono = row["TelefonoRemitente"] == DBNull.Value ? string.Empty : row["TelefonoRemitente"].ToString(),
                                Identificacion = row["IdRemitente"] == DBNull.Value ? string.Empty : row["IdRemitente"].ToString(),
                                Direccion = row["DireccionRemitente"] == DBNull.Value ? string.Empty : row["DireccionRemitente"].ToString()
                            };
                            guia.Destinatario = new CLClienteContadoDC
                            {
                                Nombre = row["NombreDestinatario"] == DBNull.Value ? string.Empty : row["NombreDestinatario"].ToString(),
                                Telefono = row["TelefonoDestinatario"] == DBNull.Value ? string.Empty : row["TelefonoDestinatario"].ToString(),
                                Identificacion = row["IdDestinatario"] == DBNull.Value ? string.Empty : row["IdDestinatario"].ToString(),
                                Direccion = row["DireccionDestinatario"] == DBNull.Value ? string.Empty : row["DireccionDestinatario"].ToString()
                            };

                            guia.NumeroBolsaSeguridad = row["NumeroBolsaSeguridad"] == DBNull.Value ? string.Empty : row["NumeroBolsaSeguridad"].ToString();
                            guia.NumeroPieza = row["NumeroPieza"] == DBNull.Value ? Convert.ToInt16(0) : Convert.ToInt16(row["NumeroPieza"]);


                            guia.EstadoGuia = (ADEnumEstadoGuia)Enum.Parse(typeof(ADEnumEstadoGuia), Convert.ToString(row["IdEstadoGuia"]).Trim(), true);
                            guia.ObservacionEstadoGuia = row["DescripcionEstado"] == DBNull.Value ? string.Empty : Convert.ToString(row["DescripcionEstado"]);
                            guia.TotalPaginas = row["TotalPaginas"] == DBNull.Value ? 0 : Convert.ToInt32(row["TotalPaginas"]);
                            guia.Peso = row["Peso"] == DBNull.Value ? 0 : Convert.ToDecimal(row["Peso"]);
                            guia.FormasPagoDescripcion = row["DescripcionFormaPago"] == DBNull.Value ? string.Empty : (row["DescripcionFormaPago"]).ToString();


                return guia;
                });

               /* SqlDataReader resultado = cmd.ExecuteReader();

                if (resultado.HasRows)
                {
                    guias = ADRepositorioMapper.ToListAdmisionesSinEntregar(resultado);
                }
                sqlConn.Close();*/
                return guias;
            }
           
        }

       



        /// <summary>
        /// Obtiene todas las guias en estado en centro de acopio en una localidad
        /// </summary>
        /// <param name="idLocalidad"></param>
        public List<ADGuiaUltEstadoDC> ObtenerGuiasEnCentroAcopioLocalidad(string idLocalidad)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paObtenerGuiasEnCentroAcopioDeLocalidad_MEN(idLocalidad).ToList()
                  .ConvertAll<ADGuiaUltEstadoDC>(g =>
                    new ADGuiaUltEstadoDC()
                    {
                        Guia = new ADGuia()
                        {
                            NumeroGuia = g.ADM_NumeroGuia,
                            IdAdmision = g.ADM_IdAdminisionMensajeria,
                            IdCiudadDestino = g.ADM_IdCiudadDestino
                        },
                        TrazaGuia = new ADTrazaGuia()
                        {
                            IdEstadoGuia = g.EGT_IdEstadoGuia,
                            IdCiudad = g.EGT_IdLocalidad
                        }
                    });
            }
        }

        /// <summary>
        /// Valida si un al cobro especifico está asignado a un coordinador de col x vencimiento en el pago
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <returns></returns>
        public bool AlCobroCargadoACoordinadorCol(long idAdmision)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                AlCobrosVencidosxCol_MEN alcobro = contexto.AlCobrosVencidosxCol_MEN.Where(a => a.AVC_IdAlCobro == idAdmision).FirstOrDefault();

                if (alcobro != null)
                    return true;
                return false;
            }
        }

        public void EliminarAlCobroCargadoCoordCol(long idAdmision)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                AlCobrosVencidosxCol_MEN alcobro = contexto.AlCobrosVencidosxCol_MEN.Where(a => a.AVC_IdAlCobro == idAdmision).FirstOrDefault();

                if (alcobro != null)
                    contexto.AlCobrosVencidosxCol_MEN.Remove(alcobro);
            }
        }

        #endregion Consultas

        #region Guias

        /// <summary>
        /// Método para actualizar el campo entregado en la tabla de admisión mensajeria
        /// </summary>
        /// <param name="numeroGuia"></param>
        public void ActualizarEntregadoGuia(long numeroGuia)
        {
            // TODO: Walter Verificar por qué lo borró!!!!!
        }

        /// <summary>
        /// Adiciona una guía anulada. Se usa para la parte de anulación de una guía. Se espera uqe se pase el id del centro de servicio de origen y el número de la guía.
        /// </summary>
        /// <param name="guia"></param>
        public long AdicionarAdmisionAnulada(ADGuia guia)
        {
            guia.TipoCliente = ADEnumTipoCliente.PPE;
            guia.DigitoVerificacion = "0";
            guia.GuidDeChequeo = Guid.NewGuid().ToString();
            guia.EsAutomatico = false;
            guia.IdUnidadNegocio = ADConstantes.ID_UNIDAD_NEGOCIO_MENSAJERIA;
            guia.IdServicio = ADConstantes.ID_SERVICIO_MENSAJERIA_EXPRESA;
            guia.NombreServicio = ADConstantes.NOMBRE_SERVICIO_MENSAJERIA_EXPRESA;
            guia.IdTipoEntrega = ADConstantes.ID_TIPO_ENTREGA;
            guia.DescripcionTipoEntrega = ADConstantes.NOMBRE_TIPO_ENTREGA;
            guia.IdCentroServicioDestino = guia.IdCentroServicioOrigen;
            guia.NombreCentroServicioDestino = guia.NombreCentroServicioOrigen;
            guia.IdPaisOrigen = string.Empty;
            guia.NombrePaisOrigen = string.Empty;
            guia.IdCiudadOrigen = string.Empty;
            guia.NombreCiudadOrigen = string.Empty;
            guia.CodigoPostalOrigen = string.Empty;
            guia.IdPaisDestino = string.Empty;
            guia.NombrePaisDestino = string.Empty;
            guia.IdCiudadDestino = string.Empty;
            guia.NombreCiudadDestino = string.Empty;
            guia.CodigoPostalDestino = string.Empty;

            ADPeaton peaton = new ADPeaton
              {
                  Apellido1 = Comun.ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.IN_ANULADO),
                  Apellido2 = Comun.ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.IN_ANULADO),
                  Direccion = Comun.ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.IN_ANULADO),
                  Identificacion = "0",
                  Nombre = Comun.ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.IN_ANULADO),
                  Telefono = Comun.ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.IN_ANULADO),
                  TipoIdentificacion = "CC"
              };
            guia.TelefonoDestinatario = peaton.Telefono;
            guia.DireccionDestinatario = peaton.Direccion;
            guia.DiasDeEntrega = 0;
            guia.FechaEstimadaEntrega = DateTime.Now.Date;
            guia.ValorAdmision = 0;
            guia.ValorAdicionales = 0;
            guia.ValorDeclarado = 0;
            guia.ValorEmpaque = 0;
            guia.ValorPrimaSeguro = 0;
            guia.ValorServicio = 0;
            guia.ValorTotal = 0;
            guia.ValorTotalImpuestos = 0;
            guia.ValorTotalRetenciones = 0;
            guia.DiceContener = string.Empty;
            guia.Observaciones = Comun.ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.IN_ANULADO);
            guia.NumeroPieza = 0;
            guia.TotalPiezas = 0;
            guia.FechaAdmision = DateTime.Now.Date;
            guia.Peso = 0;
            guia.PesoLiqMasa = 0;
            guia.PesoLiqVolumetrico = 0;
            guia.EsPesoVolumetrico = false;
            guia.NumeroBolsaSeguridad = string.Empty;
            guia.IdUnidadMedida = string.Empty;
            guia.Largo = 0;
            guia.Alto = 0;
            guia.Ancho = 0;
            guia.NombreTipoEnvio = string.Empty;
            guia.IdTipoEntrega = string.Empty;
            guia.AdmisionSistemaMensajero = false;
            guia.EsAlCobro = false;
            guia.EstaPagada = true;
            guia.FechaPago = DateTime.Now.Date;
            guia.Supervision = false;
            guia.CreadoPor = ControllerContext.Current.Usuario;
            guia.EstadoGuia = ADEnumEstadoGuia.Anulada;
            string tipoIdRemitente = string.Empty;
            string idRemitente = string.Empty;
            string nombreRemitente = string.Empty;
            string tipoIdDestinatario = string.Empty;
            string idDestinatario = string.Empty;
            string nombreDestinatario = string.Empty;
            string telefonoRemitente = string.Empty;
            string direccionRemitente = string.Empty;
            string mailRemitente = string.Empty;
            string mailDestinatario = string.Empty;
            tipoIdRemitente = peaton.TipoIdentificacion;
            idRemitente = peaton.Identificacion;
            nombreRemitente = string.Join(" ", peaton.Nombre, peaton.Apellido1, peaton.Apellido2);
            telefonoRemitente = peaton.Telefono;
            direccionRemitente = peaton.Direccion;
            mailRemitente = peaton.Email;
            tipoIdDestinatario = peaton.TipoIdentificacion;
            idDestinatario = peaton.Identificacion;
            nombreDestinatario = string.Join(" ", peaton.Nombre, peaton.Apellido1, peaton.Apellido2);
            mailDestinatario = peaton.Email;
            guia.FormasPago = new List<ADGuiaFormaPago>() {
        new ADGuiaFormaPago
            {
              IdFormaPago = 1, Descripcion = "Contado", Valor = 0
            }
      };
            long? idAdmisionMensajeria = null;
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                idAdmisionMensajeria = contexto.paCrearAdmisionMensajeria_MEN(guia.NumeroGuia, guia.DigitoVerificacion, guia.GuidDeChequeo, guia.EsAutomatico, guia.IdUnidadNegocio,
                  guia.IdServicio, guia.NombreServicio, guia.IdTipoEntrega, guia.DescripcionTipoEntrega, guia.IdCentroServicioOrigen, guia.NombreCentroServicioOrigen,
                  guia.IdCentroServicioDestino, guia.NombreCentroServicioDestino, guia.IdPaisOrigen, guia.NombrePaisOrigen, guia.IdCiudadOrigen, guia.NombreCiudadOrigen,
                  guia.CodigoPostalOrigen, guia.IdPaisDestino, guia.NombrePaisDestino, guia.IdCiudadDestino, guia.NombreCiudadDestino, guia.CodigoPostalDestino, guia.TelefonoDestinatario,
                  guia.DireccionDestinatario, guia.TipoCliente.ToString(), guia.DiasDeEntrega, guia.FechaEstimadaEntrega, guia.ValorServicio, guia.ValorTotal, guia.ValorTotalImpuestos, guia.ValorTotalRetenciones,
                  guia.ValorPrimaSeguro, guia.ValorEmpaque, guia.ValorAdicionales, guia.ValorDeclarado, guia.DiceContener, guia.Observaciones, guia.NumeroPieza,
                  guia.TotalPiezas, guia.FechaAdmision, guia.Peso, guia.PesoLiqVolumetrico, guia.PesoLiqMasa, guia.EsPesoVolumetrico, guia.NumeroBolsaSeguridad, guia.IdMotivoNoUsoBolsaSegurida, guia.MotivoNoUsoBolsaSeguriDesc,
                  guia.NoUsoaBolsaSeguridadObserv, guia.IdUnidadMedida, guia.Largo, guia.Ancho, guia.Alto, guia.EsRecomendado, guia.IdTipoEnvio, guia.NombreTipoEnvio, guia.AdmisionSistemaMensajero, guia.EsAlCobro,
                   ControllerContext.Current.Usuario, (short)guia.EstadoGuia, guia.ObservacionEstadoGuia, tipoIdRemitente, idRemitente, nombreRemitente, tipoIdDestinatario, idDestinatario, nombreDestinatario,
                  telefonoRemitente, direccionRemitente, guia.IdCiudadOrigen, guia.NombreCiudadOrigen, COConstantesModulos.MENSAJERIA, guia.IdMensajero, guia.NombreMensajero, guia.EstaPagada, guia.FechaPago,
                  guia.PrefijoNumeroGuia, false, ConstantesFramework.MinDateTimeController, mailRemitente, mailDestinatario, guia.NotificarEntregaPorEmail, guia.NoPedido).FirstOrDefault();

                if (idAdmisionMensajeria != null)
                {
                    // Se insertan las formas de pago
                    AdicionarGuiaFormasPago(idAdmisionMensajeria.Value, guia.FormasPago, ControllerContext.Current.Usuario);
                }
            }
            return idAdmisionMensajeria.HasValue ? idAdmisionMensajeria.Value : 0;
        }


        /// <summary>
        /// Método para adicionar una guia en la tabla de admisiones. Si "registraIngresoACentroAcopio" es true, entonces adiciona un estado deingreso al centro de acopio
        /// </summary>
        /// <param name="guia">Guía de admisiones con la información de la guía interna</param>
        /// <param name="destinatarioRemitente">Información del destinatario y remitente</param>
        /// <param name="registraIngresoACentroAcopio">Indica si se debe generar ingreso a centro de acopio</param>
        /// <returns>Identificador de la admisión de la guía</returns>
        public long AdicionarAdmision(ADGuia guia, ADMensajeriaTipoCliente destinatarioRemitente)
        {
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                string tipoIdRemitente = string.Empty;
                string idRemitente = string.Empty;
                string nombreRemitente = string.Empty;
                string tipoIdDestinatario = string.Empty;
                string idDestinatario = string.Empty;
                string nombreDestinatario = string.Empty;
                string telefonoRemitente = string.Empty;
                string direccionRemitente = string.Empty;
                string mailRemitente = string.Empty;
                string mailDestinatario = string.Empty;
                switch (guia.TipoCliente)
                {
                    case ADEnumTipoCliente.CCO:
                        tipoIdRemitente = ConstantesFramework.TIPO_DOCUMENTO_NIT;
                        if (destinatarioRemitente.ConvenioRemitente != null)
                        {
                            idRemitente = destinatarioRemitente.ConvenioRemitente.Nit;
                            nombreRemitente = destinatarioRemitente.ConvenioRemitente.RazonSocial;
                            telefonoRemitente = destinatarioRemitente.ConvenioRemitente.Telefono;
                            direccionRemitente = destinatarioRemitente.ConvenioRemitente.Direccion;
                            mailRemitente = destinatarioRemitente.ConvenioRemitente.EMail;
                        }
                        tipoIdDestinatario = ConstantesFramework.TIPO_DOCUMENTO_NIT;
                        if (destinatarioRemitente.ConvenioDestinatario != null)
                        {
                            idDestinatario = destinatarioRemitente.ConvenioDestinatario.Nit;
                            nombreDestinatario = destinatarioRemitente.ConvenioDestinatario.RazonSocial;
                            mailDestinatario = destinatarioRemitente.ConvenioDestinatario.EMail;
                        }
                        break;

                    case ADEnumTipoCliente.CPE:
                        tipoIdRemitente = ConstantesFramework.TIPO_DOCUMENTO_NIT;
                        if (destinatarioRemitente.ConvenioRemitente != null)
                        {
                            idRemitente = destinatarioRemitente.ConvenioRemitente.Nit;
                            nombreRemitente = destinatarioRemitente.ConvenioRemitente.RazonSocial;
                            telefonoRemitente = destinatarioRemitente.ConvenioRemitente.Telefono;
                            direccionRemitente = destinatarioRemitente.ConvenioRemitente.Direccion;
                            mailRemitente = destinatarioRemitente.ConvenioRemitente.EMail;
                        }
                        if (destinatarioRemitente.PeatonDestinatario != null)
                        {
                            tipoIdDestinatario = destinatarioRemitente.PeatonDestinatario.TipoIdentificacion;
                            idDestinatario = destinatarioRemitente.PeatonDestinatario.Identificacion;
                            nombreDestinatario = string.Join(" ", destinatarioRemitente.PeatonDestinatario.Nombre, destinatarioRemitente.PeatonDestinatario.Apellido1, destinatarioRemitente.PeatonDestinatario.Apellido2);
                            mailDestinatario = destinatarioRemitente.PeatonDestinatario.Email;
                        }
                        break;

                    case ADEnumTipoCliente.PCO:
                        if (destinatarioRemitente.PeatonRemitente != null)
                        {
                            tipoIdRemitente = destinatarioRemitente.PeatonRemitente.TipoIdentificacion;
                            idRemitente = destinatarioRemitente.PeatonRemitente.Identificacion;
                            nombreRemitente = string.Join(" ", destinatarioRemitente.PeatonRemitente.Nombre, destinatarioRemitente.PeatonRemitente.Apellido1, destinatarioRemitente.PeatonRemitente.Apellido2);
                            telefonoRemitente = destinatarioRemitente.PeatonRemitente.Telefono;
                            direccionRemitente = destinatarioRemitente.PeatonRemitente.Direccion;
                            mailRemitente = destinatarioRemitente.PeatonRemitente.Email;
                        }
                        tipoIdDestinatario = ConstantesFramework.TIPO_DOCUMENTO_NIT;
                        if (destinatarioRemitente.ConvenioDestinatario != null)
                        {
                            idDestinatario = destinatarioRemitente.ConvenioDestinatario.Nit;
                            nombreDestinatario = destinatarioRemitente.ConvenioDestinatario.RazonSocial;
                            mailDestinatario = destinatarioRemitente.ConvenioDestinatario.EMail;
                        }
                        break;

                    case ADEnumTipoCliente.PPE:
                        if (destinatarioRemitente.PeatonRemitente != null)
                        {
                            tipoIdRemitente = destinatarioRemitente.PeatonRemitente.TipoIdentificacion;
                            idRemitente = destinatarioRemitente.PeatonRemitente.Identificacion;
                            nombreRemitente = string.Join(" ", destinatarioRemitente.PeatonRemitente.Nombre, destinatarioRemitente.PeatonRemitente.Apellido1, destinatarioRemitente.PeatonRemitente.Apellido2);
                            telefonoRemitente = destinatarioRemitente.PeatonRemitente.Telefono;
                            direccionRemitente = destinatarioRemitente.PeatonRemitente.Direccion;
                            mailRemitente = destinatarioRemitente.PeatonRemitente.Email;
                        }
                        if (destinatarioRemitente.PeatonDestinatario != null)
                        {
                            tipoIdDestinatario = destinatarioRemitente.PeatonDestinatario.TipoIdentificacion;
                            idDestinatario = destinatarioRemitente.PeatonDestinatario.Identificacion;
                            nombreDestinatario = string.Join(" ", destinatarioRemitente.PeatonDestinatario.Nombre, destinatarioRemitente.PeatonDestinatario.Apellido1, destinatarioRemitente.PeatonDestinatario.Apellido2);
                            mailDestinatario = destinatarioRemitente.PeatonDestinatario.Email;
                        }
                        break;

                    case ADEnumTipoCliente.INT:
                        nombreRemitente = guia.Remitente.Nombre;
                        telefonoRemitente = guia.Remitente.Telefono;
                        direccionRemitente = guia.Remitente.Direccion;
                        nombreDestinatario = guia.Destinatario.Nombre;
                        mailRemitente = guia.Remitente.Email;
                        mailDestinatario = guia.Destinatario.Email;
                        idRemitente = guia.Remitente.Identificacion;
                        idDestinatario = guia.Destinatario.Identificacion;
                        break;
                }
                var suma = Math.Ceiling(guia.ValorServicio + guia.ValorPrimaSeguro + guia.ValorAdicionales + guia.ValorEmpaque + guia.ValorTotalImpuestos);
                if (suma > 1 && (long)suma != (long)Math.Ceiling(guia.ValorTotal))
                    throw new FaultException<ControllerException>(new ControllerException("MEN", "0", "Verifique el valor total de la guía, ya que se encuentra en cero"));

                SqlCommand cmd = new SqlCommand("paCrearAdmisionMensajeria_MEN", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@ADM_NumeroGuia", guia.NumeroGuia);
                cmd.Parameters.AddWithValue("@ADM_DigitoVerificacion", guia.DigitoVerificacion);
                cmd.Parameters.AddWithValue("@ADM_GuidDeChequeo", guia.GuidDeChequeo);
                cmd.Parameters.AddWithValue("@ADM_EsAutomatico", guia.EsAutomatico);
                cmd.Parameters.AddWithValue("@ADM_IdUnidadNegocio", guia.IdUnidadNegocio);
                cmd.Parameters.AddWithValue("@ADM_IdServicio", guia.IdServicio);
                cmd.Parameters.AddWithValue("@ADM_NombreServicio", guia.NombreServicio);
                cmd.Parameters.AddWithValue("@ADM_IdTipoEntrega", guia.IdTipoEntrega);
                cmd.Parameters.AddWithValue("@ADM_DescripcionTipoEntrega", guia.DescripcionTipoEntrega);
                cmd.Parameters.AddWithValue("@ADM_IdCentroServicioOrigen", guia.IdCentroServicioOrigen);
                cmd.Parameters.AddWithValue("@ADM_NombreCentroServicioOrigen", guia.NombreCentroServicioOrigen);
                cmd.Parameters.AddWithValue("@ADM_IdCentroServicioDestino", guia.IdCentroServicioDestino);
                cmd.Parameters.AddWithValue("@ADM_NombreCentroServicioDestino", guia.NombreCentroServicioDestino);
                cmd.Parameters.AddWithValue("@ADM_IdPaisOrigen", guia.IdPaisOrigen);
                cmd.Parameters.AddWithValue("@ADM_NombrePaisOrigen", guia.NombrePaisOrigen);
                cmd.Parameters.AddWithValue("@ADM_IdCiudadOrigen", guia.IdCiudadOrigen);
                cmd.Parameters.AddWithValue("@ADM_NombreCiudadOrigen", guia.NombreCiudadOrigen);
                cmd.Parameters.AddWithValue("@ADM_CodigoPostalOrigen", guia.CodigoPostalOrigen);
                cmd.Parameters.AddWithValue("@ADM_IdPaisDestino", guia.IdPaisDestino);
                cmd.Parameters.AddWithValue("@ADM_NombrePaisDestino", guia.NombrePaisDestino);
                cmd.Parameters.AddWithValue("@ADM_IdCiudadDestino", guia.IdCiudadDestino);
                cmd.Parameters.AddWithValue("@ADM_NombreCiudadDestino", guia.NombreCiudadDestino);
                cmd.Parameters.AddWithValue("@ADM_CodigoPostalDestino", guia.CodigoPostalDestino);
                cmd.Parameters.AddWithValue("@ADM_TelefonoDestinatario", guia.TelefonoDestinatario);
                cmd.Parameters.AddWithValue("@ADM_DireccionDestinatario", guia.DireccionDestinatario);
                cmd.Parameters.AddWithValue("@ADM_TipoCliente", guia.TipoCliente.ToString());
                cmd.Parameters.AddWithValue("@ADM_DiasDeEntrega", guia.DiasDeEntrega);
                cmd.Parameters.AddWithValue("@ADM_FechaEstimadaEntrega", guia.FechaEstimadaEntrega);
                cmd.Parameters.AddWithValue("@ADM_FechaEstimadaDigitalizacion", guia.FechaEstimadaDigitalizacion);
                cmd.Parameters.AddWithValue("@ADM_FechaEstimadaArchivo", guia.FechaEstimadaArchivo);
                cmd.Parameters.AddWithValue("@ADM_ValorAdmision", guia.ValorAdmision);
                cmd.Parameters.AddWithValue("@ADM_ValorTotal", guia.ValorTotal);
                cmd.Parameters.AddWithValue("@ADM_ValorTotalImpuestos", guia.ValorTotalImpuestos);
                cmd.Parameters.AddWithValue("@ADM_ValorTotalRetenciones", guia.ValorTotalRetenciones);
                cmd.Parameters.AddWithValue("@ADM_ValorPrimaSeguro", guia.ValorPrimaSeguro);
                cmd.Parameters.AddWithValue("@ADM_ValorEmpaque", guia.ValorEmpaque);
                cmd.Parameters.AddWithValue("@ADM_ValorAdicionales", guia.ValorAdicionales);
                cmd.Parameters.AddWithValue("@ADM_ValorDeclarado", guia.ValorDeclarado);
                cmd.Parameters.AddWithValue("@ADM_DiceContener", guia.DiceContener);
                cmd.Parameters.AddWithValue("@ADM_Observaciones", guia.Observaciones);
                cmd.Parameters.AddWithValue("@ADM_NumeroPieza", guia.NumeroPieza);
                cmd.Parameters.AddWithValue("@ADM_TotalPiezas", guia.TotalPiezas);
                cmd.Parameters.AddWithValue("@ADM_FechaAdmision", guia.FechaAdmision.Year != 1 ? guia.FechaAdmision : DateTime.Now);
                cmd.Parameters.AddWithValue("@ADM_Peso", guia.Peso);
                cmd.Parameters.AddWithValue("@ADM_PesoLiqVolumetrico", guia.PesoLiqVolumetrico);
                cmd.Parameters.AddWithValue("@ADM_PesoLiqMasa", guia.PesoLiqMasa);
                cmd.Parameters.AddWithValue("@ADM_EsPesoVolumetrico", guia.EsPesoVolumetrico);
                cmd.Parameters.AddWithValue("@ADM_NumeroBolsaSeguridad", guia.NumeroBolsaSeguridad);
                cmd.Parameters.AddWithValue("@ADM_IdMotivoNoUsoBolsaSegurida", guia.IdMotivoNoUsoBolsaSegurida == null ? 0 : guia.IdMotivoNoUsoBolsaSegurida.Value);
                cmd.Parameters.AddWithValue("@ADM_MotivoNoUsoBolsaSeguriDesc", guia.MotivoNoUsoBolsaSeguriDesc == null ? "" : guia.MotivoNoUsoBolsaSeguriDesc);
                cmd.Parameters.AddWithValue("@ADM_NoUsoaBolsaSeguridadObserv", guia.NoUsoaBolsaSeguridadObserv == null ? "" : guia.NoUsoaBolsaSeguridadObserv);
                cmd.Parameters.AddWithValue("@ADM_IdUnidadMedida", guia.IdUnidadMedida);
                cmd.Parameters.AddWithValue("@ADM_Largo", guia.Largo);
                cmd.Parameters.AddWithValue("@ADM_Ancho", guia.Ancho);
                cmd.Parameters.AddWithValue("@ADM_Alto", guia.Alto);
                cmd.Parameters.AddWithValue("@ADM_EsRecomendado", guia.EsRecomendado);
                cmd.Parameters.AddWithValue("@ADM_IdTipoEnvio", guia.IdTipoEnvio);
                cmd.Parameters.AddWithValue("@ADM_NombreTipoEnvio", guia.NombreTipoEnvio);
                cmd.Parameters.AddWithValue("@ADM_AdmisionSistemaMensajero", guia.AdmisionSistemaMensajero);
                cmd.Parameters.AddWithValue("@ADM_EsAlCobro", guia.EsAlCobro);
                cmd.Parameters.AddWithValue("@ADM_CreadoPor", ControllerContext.Current.Usuario);
                cmd.Parameters.AddWithValue("@EGT_IdEstadoGuia", guia.EstadoGuia);
                cmd.Parameters.AddWithValue("@EGT_ObservacionEstadoGuia", guia.ObservacionEstadoGuia == null ? "" : guia.ObservacionEstadoGuia);
                cmd.Parameters.AddWithValue("@ADM_IdTipoIdentificacionRemitente", tipoIdRemitente);
                cmd.Parameters.AddWithValue("@ADM_IdRemitente", !string.IsNullOrWhiteSpace(idRemitente) ? idRemitente : "0");
                cmd.Parameters.AddWithValue("@ADM_NombreRemitente", nombreRemitente);
                cmd.Parameters.AddWithValue("@ADM_IdTipoIdentificacionDestinatario", tipoIdDestinatario);
                cmd.Parameters.AddWithValue("@ADM_IdDestinatario", idDestinatario);
                cmd.Parameters.AddWithValue("@ADM_NombreDestinatario", nombreDestinatario);
                cmd.Parameters.AddWithValue("@ADM_TelefonoRemitente", telefonoRemitente);
                cmd.Parameters.AddWithValue("@ADM_DireccionRemitente", direccionRemitente);
                cmd.Parameters.AddWithValue("@EGT_IdLocalidad", guia.IdCiudadOrigen);
                cmd.Parameters.AddWithValue("@EGT_NombreLocalidad", guia.NombreCiudadOrigen);
                cmd.Parameters.AddWithValue("@EGT_IdModulo", COConstantesModulos.MENSAJERIA);
                cmd.Parameters.AddWithValue("@ADM_IdMensajero", guia.IdMensajero);
                cmd.Parameters.AddWithValue("@ADM_NombreMensajero", guia.NombreMensajero);
                cmd.Parameters.AddWithValue("@ADM_EstaPagada", guia.EstaPagada);
                cmd.Parameters.AddWithValue("@ADM_FechaPago", guia.FechaPago);
                cmd.Parameters.AddWithValue("@ADM_PrefijoNumeroGuia", guia.PrefijoNumeroGuia == null ? "" : guia.PrefijoNumeroGuia);
                cmd.Parameters.AddWithValue("@AMD_EsSupervisada", false);
                cmd.Parameters.AddWithValue("@AMD_FechaSupervision", ConstantesFramework.MinDateTimeController);
                cmd.Parameters.AddWithValue("@AMD_EmailRemitente", mailRemitente);
                cmd.Parameters.AddWithValue("@AMD_EmailDestinatario", mailDestinatario);
                cmd.Parameters.AddWithValue("@ADM_NotificarEntregaPorEmail", guia.NotificarEntregaPorEmail);
                cmd.Parameters.AddWithValue("@NoPedido", guia.NoPedido);

                sqlConn.Open();
                long? idAdmisionMensajeria = null;
                var idAdm = cmd.ExecuteScalar();
                sqlConn.Close();
                if (idAdm != null)
                {
                    idAdmisionMensajeria = Convert.ToInt64(idAdm);
                    return idAdmisionMensajeria.Value;
                }
                else
                    return 0;
            }
        }

        ////using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
        //using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
        //{
        //    string tipoIdRemitente = string.Empty;
        //    string idRemitente = string.Empty;
        //    string nombreRemitente = string.Empty;
        //    string tipoIdDestinatario = string.Empty;
        //    string idDestinatario = string.Empty;
        //    string nombreDestinatario = string.Empty;
        //    string telefonoRemitente = string.Empty;
        //    string direccionRemitente = string.Empty;
        //    string mailRemitente = string.Empty;
        //    string mailDestinatario = string.Empty;
        //    switch (guia.TipoCliente)
        //    {
        //        case ADEnumTipoCliente.CCO:
        //            tipoIdRemitente = ConstantesFramework.TIPO_DOCUMENTO_NIT;
        //            if (destinatarioRemitente.ConvenioRemitente != null)
        //            {
        //                idRemitente = destinatarioRemitente.ConvenioRemitente.Nit;
        //                nombreRemitente = destinatarioRemitente.ConvenioRemitente.RazonSocial;
        //                telefonoRemitente = destinatarioRemitente.ConvenioRemitente.Telefono;
        //                direccionRemitente = destinatarioRemitente.ConvenioRemitente.Direccion;
        //                mailRemitente = destinatarioRemitente.ConvenioRemitente.EMail;
        //            }
        //            tipoIdDestinatario = ConstantesFramework.TIPO_DOCUMENTO_NIT;
        //            if (destinatarioRemitente.ConvenioDestinatario != null)
        //            {
        //                idDestinatario = destinatarioRemitente.ConvenioDestinatario.Nit;
        //                nombreDestinatario = destinatarioRemitente.ConvenioDestinatario.RazonSocial;
        //                mailDestinatario = destinatarioRemitente.ConvenioDestinatario.EMail;
        //            }
        //            break;

        //        case ADEnumTipoCliente.CPE:
        //            tipoIdRemitente = ConstantesFramework.TIPO_DOCUMENTO_NIT;
        //            if (destinatarioRemitente.ConvenioRemitente != null)
        //            {
        //                idRemitente = destinatarioRemitente.ConvenioRemitente.Nit;
        //                nombreRemitente = destinatarioRemitente.ConvenioRemitente.RazonSocial;
        //                telefonoRemitente = destinatarioRemitente.ConvenioRemitente.Telefono;
        //                direccionRemitente = destinatarioRemitente.ConvenioRemitente.Direccion;
        //                mailRemitente = destinatarioRemitente.ConvenioRemitente.EMail;
        //            }
        //            if (destinatarioRemitente.PeatonDestinatario != null)
        //            {
        //                tipoIdDestinatario = destinatarioRemitente.PeatonDestinatario.TipoIdentificacion;
        //                idDestinatario = destinatarioRemitente.PeatonDestinatario.Identificacion;
        //                nombreDestinatario = string.Join(" ", destinatarioRemitente.PeatonDestinatario.Nombre, destinatarioRemitente.PeatonDestinatario.Apellido1, destinatarioRemitente.PeatonDestinatario.Apellido2);
        //                mailDestinatario = destinatarioRemitente.PeatonDestinatario.Email;
        //            }
        //            break;

        //        case ADEnumTipoCliente.PCO:
        //            if (destinatarioRemitente.PeatonRemitente != null)
        //            {
        //                tipoIdRemitente = destinatarioRemitente.PeatonRemitente.TipoIdentificacion;
        //                idRemitente = destinatarioRemitente.PeatonRemitente.Identificacion;
        //                nombreRemitente = string.Join(" ", destinatarioRemitente.PeatonRemitente.Nombre, destinatarioRemitente.PeatonRemitente.Apellido1, destinatarioRemitente.PeatonRemitente.Apellido2);
        //                telefonoRemitente = destinatarioRemitente.PeatonRemitente.Telefono;
        //                direccionRemitente = destinatarioRemitente.PeatonRemitente.Direccion;
        //                mailRemitente = destinatarioRemitente.PeatonRemitente.Email;
        //            }
        //            tipoIdDestinatario = ConstantesFramework.TIPO_DOCUMENTO_NIT;
        //            if (destinatarioRemitente.ConvenioDestinatario != null)
        //            {
        //                idDestinatario = destinatarioRemitente.ConvenioDestinatario.Nit;
        //                nombreDestinatario = destinatarioRemitente.ConvenioDestinatario.RazonSocial;
        //                mailDestinatario = destinatarioRemitente.ConvenioDestinatario.EMail;
        //            }
        //            break;

        //        case ADEnumTipoCliente.PPE:
        //            if (destinatarioRemitente.PeatonRemitente != null)
        //            {
        //                tipoIdRemitente = destinatarioRemitente.PeatonRemitente.TipoIdentificacion;
        //                idRemitente = destinatarioRemitente.PeatonRemitente.Identificacion;
        //                nombreRemitente = string.Join(" ", destinatarioRemitente.PeatonRemitente.Nombre, destinatarioRemitente.PeatonRemitente.Apellido1, destinatarioRemitente.PeatonRemitente.Apellido2);
        //                telefonoRemitente = destinatarioRemitente.PeatonRemitente.Telefono;
        //                direccionRemitente = destinatarioRemitente.PeatonRemitente.Direccion;
        //                mailRemitente = destinatarioRemitente.PeatonRemitente.Email;
        //            }
        //            if (destinatarioRemitente.PeatonDestinatario != null)
        //            {
        //                tipoIdDestinatario = destinatarioRemitente.PeatonDestinatario.TipoIdentificacion;
        //                idDestinatario = destinatarioRemitente.PeatonDestinatario.Identificacion;
        //                nombreDestinatario = string.Join(" ", destinatarioRemitente.PeatonDestinatario.Nombre, destinatarioRemitente.PeatonDestinatario.Apellido1, destinatarioRemitente.PeatonDestinatario.Apellido2);
        //                mailDestinatario = destinatarioRemitente.PeatonDestinatario.Email;
        //            }
        //            break;

        //        case ADEnumTipoCliente.INT:
        //            nombreRemitente = guia.Remitente.Nombre;
        //            telefonoRemitente = guia.Remitente.Telefono;
        //            direccionRemitente = guia.Remitente.Direccion;
        //            nombreDestinatario = guia.Destinatario.Nombre;
        //            mailRemitente = guia.Remitente.Email;
        //            mailDestinatario = guia.Destinatario.Email;
        //            idRemitente = guia.Remitente.Identificacion;
        //            idDestinatario = guia.Destinatario.Identificacion;
        //            break;
        //    }
        //    var suma = guia.ValorServicio + guia.ValorPrimaSeguro + guia.ValorAdicionales + guia.ValorEmpaque + guia.ValorTotalImpuestos;
        //    if (suma > 1 && (long)suma != (long)guia.ValorTotal)
        //        throw new FaultException<ControllerException>(new ControllerException("MEN", "0", "Verifique el valor total de la guía, ya que se encuentra en cero"));



        //    SqlCommand cmd = new SqlCommand("paCrearAdmisionMensajeria_MEN", sqlConn);
        //    cmd.CommandType = CommandType.StoredProcedure;
        //    cmd.Parameters.Clear();
        //    cmd.Parameters.AddWithValue("@ADM_NumeroGuia", guia.NumeroGuia);
        //    cmd.Parameters.AddWithValue("@ADM_DigitoVerificacion", guia.DigitoVerificacion);
        //    cmd.Parameters.AddWithValue("@ADM_GuidDeChequeo", guia.GuidDeChequeo);
        //    cmd.Parameters.AddWithValue("@ADM_EsAutomatico", guia.EsAutomatico);
        //    cmd.Parameters.AddWithValue("@ADM_IdUnidadNegocio", guia.IdUnidadNegocio);
        //    cmd.Parameters.AddWithValue("@ADM_IdServicio", guia.IdServicio);
        //    cmd.Parameters.AddWithValue("@ADM_NombreServicio", guia.NombreServicio);
        //    cmd.Parameters.AddWithValue("@ADM_IdTipoEntrega", guia.IdTipoEntrega);
        //    cmd.Parameters.AddWithValue("@ADM_DescripcionTipoEntrega", guia.DescripcionTipoEntrega);
        //    cmd.Parameters.AddWithValue("@ADM_IdCentroServicioOrigen", guia.IdCentroServicioOrigen);
        //    cmd.Parameters.AddWithValue("@ADM_NombreCentroServicioOrigen", guia.NombreCentroServicioOrigen);
        //    cmd.Parameters.AddWithValue("@ADM_IdCentroServicioDestino", guia.IdCentroServicioDestino);
        //    cmd.Parameters.AddWithValue("@ADM_NombreCentroServicioDestino", guia.NombreCentroServicioDestino);
        //    cmd.Parameters.AddWithValue("@ADM_IdPaisOrigen", guia.IdPaisOrigen);
        //    cmd.Parameters.AddWithValue("@ADM_NombrePaisOrigen", guia.NombrePaisOrigen);
        //    cmd.Parameters.AddWithValue("@ADM_IdCiudadOrigen", guia.IdCiudadOrigen);
        //    cmd.Parameters.AddWithValue("@ADM_NombreCiudadOrigen", guia.NombreCiudadOrigen);
        //    cmd.Parameters.AddWithValue("@ADM_CodigoPostalOrigen", guia.CodigoPostalOrigen);
        //    cmd.Parameters.AddWithValue("@ADM_IdPaisDestino", guia.IdPaisDestino);
        //    cmd.Parameters.AddWithValue("@ADM_NombrePaisDestino", guia.NombrePaisDestino);
        //    cmd.Parameters.AddWithValue("@ADM_IdCiudadDestino", guia.IdCiudadDestino);
        //    cmd.Parameters.AddWithValue("@ADM_NombreCiudadDestino", guia.NombreCiudadDestino);
        //    cmd.Parameters.AddWithValue("@ADM_CodigoPostalDestino", guia.CodigoPostalDestino);
        //    cmd.Parameters.AddWithValue("@ADM_TelefonoDestinatario", guia.TelefonoDestinatario);
        //    cmd.Parameters.AddWithValue("@ADM_DireccionDestinatario", guia.DireccionDestinatario);
        //    cmd.Parameters.AddWithValue("@ADM_TipoCliente", guia.TipoCliente.ToString());
        //    cmd.Parameters.AddWithValue("@ADM_DiasDeEntrega", guia.DiasDeEntrega);
        //    cmd.Parameters.AddWithValue("@ADM_FechaEstimadaEntrega", guia.FechaEstimadaEntrega);
        //    cmd.Parameters.AddWithValue("@ADM_ValorAdmision", guia.ValorAdmision);
        //    cmd.Parameters.AddWithValue("@ADM_ValorTotal", guia.ValorTotal);
        //    cmd.Parameters.AddWithValue("@ADM_ValorTotalImpuestos", guia.ValorTotalImpuestos);
        //    cmd.Parameters.AddWithValue("@ADM_ValorTotalRetenciones", guia.ValorTotalRetenciones);
        //    cmd.Parameters.AddWithValue("@ADM_ValorPrimaSeguro", guia.ValorPrimaSeguro);
        //    cmd.Parameters.AddWithValue("@ADM_ValorEmpaque", guia.ValorEmpaque);
        //    cmd.Parameters.AddWithValue("@ADM_ValorAdicionales", guia.ValorAdicionales);
        //    cmd.Parameters.AddWithValue("@ADM_ValorDeclarado", guia.ValorDeclarado);
        //    cmd.Parameters.AddWithValue("@ADM_DiceContener", guia.DiceContener);
        //    cmd.Parameters.AddWithValue("@ADM_Observaciones", guia.Observaciones);
        //    cmd.Parameters.AddWithValue("@ADM_NumeroPieza", guia.NumeroPieza);
        //    cmd.Parameters.AddWithValue("@ADM_TotalPiezas", guia.TotalPiezas);
        //    cmd.Parameters.AddWithValue("@ADM_FechaAdmision", guia.FechaAdmision.Year != 1 ? guia.FechaAdmision : DateTime.Now);
        //    cmd.Parameters.AddWithValue("@ADM_Peso", guia.Peso);
        //    cmd.Parameters.AddWithValue("@ADM_PesoLiqVolumetrico", guia.PesoLiqVolumetrico);
        //    cmd.Parameters.AddWithValue("@ADM_PesoLiqMasa", guia.PesoLiqMasa);
        //    cmd.Parameters.AddWithValue("@ADM_EsPesoVolumetrico", guia.EsPesoVolumetrico);
        //    cmd.Parameters.AddWithValue("@ADM_NumeroBolsaSeguridad", guia.NumeroBolsaSeguridad);
        //    cmd.Parameters.AddWithValue("@ADM_IdMotivoNoUsoBolsaSegurida", guia.IdMotivoNoUsoBolsaSegurida == null ? 0 : guia.IdMotivoNoUsoBolsaSegurida.Value);
        //    cmd.Parameters.AddWithValue("@ADM_MotivoNoUsoBolsaSeguriDesc", guia.MotivoNoUsoBolsaSeguriDesc);
        //    cmd.Parameters.AddWithValue("@ADM_NoUsoaBolsaSeguridadObserv", guia.NoUsoaBolsaSeguridadObserv == null ? "" : guia.NoUsoaBolsaSeguridadObserv);
        //    cmd.Parameters.AddWithValue("@ADM_IdUnidadMedida", guia.IdUnidadMedida);
        //    cmd.Parameters.AddWithValue("@ADM_Largo", guia.Largo);
        //    cmd.Parameters.AddWithValue("@ADM_Ancho", guia.Ancho);
        //    cmd.Parameters.AddWithValue("@ADM_Alto", guia.Alto);
        //    cmd.Parameters.AddWithValue("@ADM_EsRecomendado", guia.EsRecomendado);
        //    cmd.Parameters.AddWithValue("@ADM_IdTipoEnvio", guia.IdTipoEnvio);
        //    cmd.Parameters.AddWithValue("@ADM_NombreTipoEnvio", guia.NombreTipoEnvio);
        //    cmd.Parameters.AddWithValue("@ADM_AdmisionSistemaMensajero", guia.AdmisionSistemaMensajero);
        //    cmd.Parameters.AddWithValue("@ADM_EsAlCobro", guia.EsAlCobro);
        //    cmd.Parameters.AddWithValue("@ADM_CreadoPor", ControllerContext.Current.Usuario);
        //    cmd.Parameters.AddWithValue("@EGT_IdEstadoGuia", guia.EstadoGuia);
        //    cmd.Parameters.AddWithValue("@EGT_ObservacionEstadoGuia", guia.ObservacionEstadoGuia == null ? "" : guia.ObservacionEstadoGuia);
        //    cmd.Parameters.AddWithValue("@ADM_IdTipoIdentificacionRemitente", tipoIdRemitente);
        //    cmd.Parameters.AddWithValue("@ADM_IdRemitente", idRemitente);
        //    cmd.Parameters.AddWithValue("@ADM_NombreRemitente", nombreRemitente);
        //    cmd.Parameters.AddWithValue("@ADM_IdTipoIdentificacionDestinatario", tipoIdDestinatario);
        //    cmd.Parameters.AddWithValue("@ADM_IdDestinatario", idDestinatario);
        //    cmd.Parameters.AddWithValue("@ADM_NombreDestinatario", nombreDestinatario);
        //    cmd.Parameters.AddWithValue("@ADM_TelefonoRemitente", telefonoRemitente);
        //    cmd.Parameters.AddWithValue("@ADM_DireccionRemitente", direccionRemitente);
        //    cmd.Parameters.AddWithValue("@EGT_IdLocalidad", guia.IdCiudadOrigen);
        //    cmd.Parameters.AddWithValue("@EGT_NombreLocalidad", guia.NombreCiudadOrigen);
        //    cmd.Parameters.AddWithValue("@EGT_IdModulo", COConstantesModulos.MENSAJERIA);
        //    cmd.Parameters.AddWithValue("@ADM_IdMensajero", guia.IdMensajero);
        //    cmd.Parameters.AddWithValue("@ADM_NombreMensajero", guia.NombreMensajero);
        //    cmd.Parameters.AddWithValue("@ADM_EstaPagada", guia.EstaPagada);
        //    cmd.Parameters.AddWithValue("@ADM_FechaPago", guia.FechaPago);
        //    cmd.Parameters.AddWithValue("@ADM_PrefijoNumeroGuia", guia.PrefijoNumeroGuia == null ? "" : guia.PrefijoNumeroGuia);
        //    cmd.Parameters.AddWithValue("@AMD_EsSupervisada", false);
        //    cmd.Parameters.AddWithValue("@AMD_FechaSupervision", ConstantesFramework.MinDateTimeController);
        //    cmd.Parameters.AddWithValue("@AMD_EmailRemitente", mailRemitente);
        //    cmd.Parameters.AddWithValue("@AMD_EmailDestinatario", mailDestinatario);
        //    cmd.Parameters.AddWithValue("@ADM_NotificarEntregaPorEmail", guia.NotificarEntregaPorEmail);
        //    cmd.Parameters.AddWithValue("@NoPedido", guia.NoPedido);

        //    sqlConn.Open();
        //    long? idAdmisionMensajeria = null;
        //    var idAdm = cmd.ExecuteScalar();
        //    sqlConn.Close();
        //    if (idAdm != null)
        //    {
        //        idAdmisionMensajeria = Convert.ToInt64(idAdm);
        //        return idAdmisionMensajeria.Value;
        //    }
        //    else
        //        return 0;



        //    /* long? idAdmisionMensajeria = contexto.paCrearAdmisionMensajeria_MEN(guia.NumeroGuia, guia.DigitoVerificacion, guia.GuidDeChequeo, guia.EsAutomatico, guia.IdUnidadNegocio,
        //        guia.IdServicio, guia.NombreServicio, guia.IdTipoEntrega, guia.DescripcionTipoEntrega, guia.IdCentroServicioOrigen, guia.NombreCentroServicioOrigen,
        //        guia.IdCentroServicioDestino, guia.NombreCentroServicioDestino, guia.IdPaisOrigen, guia.NombrePaisOrigen, guia.IdCiudadOrigen, guia.NombreCiudadOrigen,
        //        guia.CodigoPostalOrigen, guia.IdPaisDestino, guia.NombrePaisDestino, guia.IdCiudadDestino, guia.NombreCiudadDestino, guia.CodigoPostalDestino, guia.TelefonoDestinatario,
        //        guia.DireccionDestinatario, guia.TipoCliente.ToString(), guia.DiasDeEntrega, guia.FechaEstimadaEntrega, guia.ValorServicio, guia.ValorTotal, guia.ValorTotalImpuestos, guia.ValorTotalRetenciones,
        //        guia.ValorPrimaSeguro, guia.ValorEmpaque, guia.ValorAdicionales, guia.ValorDeclarado, guia.DiceContener, guia.Observaciones, guia.NumeroPieza,
        //        guia.TotalPiezas, guia.FechaAdmision.Year != 1 ? guia.FechaAdmision : DateTime.Now, guia.Peso, guia.PesoLiqVolumetrico, guia.PesoLiqMasa, guia.EsPesoVolumetrico, guia.NumeroBolsaSeguridad, guia.IdMotivoNoUsoBolsaSegurida, guia.MotivoNoUsoBolsaSeguriDesc,
        //        guia.NoUsoaBolsaSeguridadObserv, guia.IdUnidadMedida, guia.Largo, guia.Ancho, guia.Alto, guia.EsRecomendado, guia.IdTipoEnvio, guia.NombreTipoEnvio, guia.AdmisionSistemaMensajero, guia.EsAlCobro,
        //         ControllerContext.Current.Usuario, (short)guia.EstadoGuia, guia.ObservacionEstadoGuia, tipoIdRemitente, idRemitente, nombreRemitente, tipoIdDestinatario, idDestinatario, nombreDestinatario,
        //        telefonoRemitente, direccionRemitente, guia.IdCiudadOrigen, guia.NombreCiudadOrigen, COConstantesModulos.MENSAJERIA, guia.IdMensajero, guia.NombreMensajero, guia.EstaPagada, guia.FechaPago,
        //        guia.PrefijoNumeroGuia, false, ConstantesFramework.MinDateTimeController, mailRemitente, mailDestinatario, guia.NotificarEntregaPorEmail, guia.NoPedido).FirstOrDefault();*/

        // }    
        // }         

        /// <summary>
        /// Método para adicionar una guia en la tabla de admisiones. Si "registraIngresoACentroAcopio" es true, entonces adiciona un estado deingreso al centro de acopio
        /// </summary>
        /// <param name="guia">Guía de admisiones con la información de la guía interna</param>
        /// <param name="destinatarioRemitente">Información del destinatario y remitente</param>
        /// <param name="registraIngresoACentroAcopio">Indica si se debe generar ingreso a centro de acopio</param>
        /// <returns>Identificador de la admisión de la guía</returns>
        public long AdicionarAdmision(ADGuia guia, ADMensajeriaTipoCliente destinatarioRemitente, SqlConnection conexion, SqlTransaction transaccion)
        {
            SqlCommand cmd = new SqlCommand("paCrearAdmisionMensajeria_MEN", conexion, transaccion);
            cmd.CommandType = CommandType.StoredProcedure;

            string tipoIdRemitente = string.Empty;
            string idRemitente = string.Empty;
            string nombreRemitente = string.Empty;
            string tipoIdDestinatario = string.Empty;
            string idDestinatario = string.Empty;
            string nombreDestinatario = string.Empty;
            string telefonoRemitente = string.Empty;
            string direccionRemitente = string.Empty;
            string mailRemitente = string.Empty;
            string mailDestinatario = string.Empty;
            switch (guia.TipoCliente)
            {
                case ADEnumTipoCliente.CCO:
                    tipoIdRemitente = ConstantesFramework.TIPO_DOCUMENTO_NIT;
                    if (destinatarioRemitente.ConvenioRemitente != null)
                    {
                        idRemitente = destinatarioRemitente.ConvenioRemitente.Nit;
                        nombreRemitente = destinatarioRemitente.ConvenioRemitente.RazonSocial;
                        telefonoRemitente = destinatarioRemitente.ConvenioRemitente.Telefono;
                        direccionRemitente = destinatarioRemitente.ConvenioRemitente.Direccion;
                        mailRemitente = destinatarioRemitente.ConvenioRemitente.EMail;
                    }
                    tipoIdDestinatario = ConstantesFramework.TIPO_DOCUMENTO_NIT;
                    if (destinatarioRemitente.ConvenioDestinatario != null)
                    {
                        idDestinatario = destinatarioRemitente.ConvenioDestinatario.Nit;
                        nombreDestinatario = destinatarioRemitente.ConvenioDestinatario.RazonSocial;
                        mailDestinatario = destinatarioRemitente.ConvenioDestinatario.EMail;
                    }
                    break;

                case ADEnumTipoCliente.CPE:
                    tipoIdRemitente = ConstantesFramework.TIPO_DOCUMENTO_NIT;
                    if (destinatarioRemitente.ConvenioRemitente != null)
                    {
                        idRemitente = destinatarioRemitente.ConvenioRemitente.Nit;
                        nombreRemitente = destinatarioRemitente.ConvenioRemitente.RazonSocial;
                        telefonoRemitente = destinatarioRemitente.ConvenioRemitente.Telefono;
                        direccionRemitente = destinatarioRemitente.ConvenioRemitente.Direccion;
                        mailRemitente = destinatarioRemitente.ConvenioRemitente.EMail;
                    }
                    if (destinatarioRemitente.PeatonDestinatario != null)
                    {
                        tipoIdDestinatario = destinatarioRemitente.PeatonDestinatario.TipoIdentificacion;
                        idDestinatario = destinatarioRemitente.PeatonDestinatario.Identificacion;
                        nombreDestinatario = string.Join(" ", destinatarioRemitente.PeatonDestinatario.Nombre, destinatarioRemitente.PeatonDestinatario.Apellido1, destinatarioRemitente.PeatonDestinatario.Apellido2);
                        mailDestinatario = destinatarioRemitente.PeatonDestinatario.Email;
                    }
                    break;

                case ADEnumTipoCliente.PCO:
                    if (destinatarioRemitente.PeatonRemitente != null)
                    {
                        tipoIdRemitente = destinatarioRemitente.PeatonRemitente.TipoIdentificacion;
                        idRemitente = destinatarioRemitente.PeatonRemitente.Identificacion;
                        nombreRemitente = string.Join(" ", destinatarioRemitente.PeatonRemitente.Nombre, destinatarioRemitente.PeatonRemitente.Apellido1, destinatarioRemitente.PeatonRemitente.Apellido2);
                        telefonoRemitente = destinatarioRemitente.PeatonRemitente.Telefono;
                        direccionRemitente = destinatarioRemitente.PeatonRemitente.Direccion;
                        mailRemitente = destinatarioRemitente.PeatonRemitente.Email;
                    }
                    tipoIdDestinatario = ConstantesFramework.TIPO_DOCUMENTO_NIT;
                    if (destinatarioRemitente.ConvenioDestinatario != null)
                    {
                        idDestinatario = destinatarioRemitente.ConvenioDestinatario.Nit;
                        nombreDestinatario = destinatarioRemitente.ConvenioDestinatario.RazonSocial;
                        mailDestinatario = destinatarioRemitente.ConvenioDestinatario.EMail;
                    }
                    break;

                case ADEnumTipoCliente.PPE:
                    if (destinatarioRemitente.PeatonRemitente != null)
                    {
                        tipoIdRemitente = destinatarioRemitente.PeatonRemitente.TipoIdentificacion;
                        idRemitente = destinatarioRemitente.PeatonRemitente.Identificacion;
                        nombreRemitente = string.Join(" ", destinatarioRemitente.PeatonRemitente.Nombre, destinatarioRemitente.PeatonRemitente.Apellido1, destinatarioRemitente.PeatonRemitente.Apellido2);
                        telefonoRemitente = destinatarioRemitente.PeatonRemitente.Telefono;
                        direccionRemitente = destinatarioRemitente.PeatonRemitente.Direccion;
                        mailRemitente = destinatarioRemitente.PeatonRemitente.Email;
                    }
                    if (destinatarioRemitente.PeatonDestinatario != null)
                    {
                        tipoIdDestinatario = destinatarioRemitente.PeatonDestinatario.TipoIdentificacion;
                        idDestinatario = destinatarioRemitente.PeatonDestinatario.Identificacion;
                        nombreDestinatario = string.Join(" ", destinatarioRemitente.PeatonDestinatario.Nombre, destinatarioRemitente.PeatonDestinatario.Apellido1, destinatarioRemitente.PeatonDestinatario.Apellido2);
                        mailDestinatario = destinatarioRemitente.PeatonDestinatario.Email;
                    }
                    break;

                case ADEnumTipoCliente.INT:
                    nombreRemitente = guia.Remitente.Nombre;
                    telefonoRemitente = guia.Remitente.Telefono;
                    direccionRemitente = guia.Remitente.Direccion;
                    nombreDestinatario = guia.Destinatario.Nombre;
                    mailRemitente = guia.Remitente.Email;
                    mailDestinatario = guia.Destinatario.Email;
                    idRemitente = guia.Remitente.Identificacion;
                    idDestinatario = guia.Destinatario.Identificacion;
                    break;
            }
            var suma = guia.ValorServicio + guia.ValorPrimaSeguro + guia.ValorAdicionales + guia.ValorEmpaque + guia.ValorTotalImpuestos;
            if (suma > 1 && (long)suma != (long)guia.ValorTotal)
                throw new FaultException<ControllerException>(new ControllerException("MEN", "0", "Verifique el valor total de la guía, ya que se encuentra en cero"));


            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_NumeroGuia", guia.NumeroGuia));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_DigitoVerificacion", guia.DigitoVerificacion));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_GuidDeChequeo", guia.GuidDeChequeo));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_EsAutomatico", guia.EsAutomatico));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_IdUnidadNegocio", guia.IdUnidadNegocio));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_IdServicio", guia.IdServicio));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_NombreServicio", guia.NombreServicio));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_IdTipoEntrega", guia.IdTipoEntrega));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_DescripcionTipoEntrega", guia.DescripcionTipoEntrega));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_IdCentroServicioOrigen", guia.IdCentroServicioOrigen));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_NombreCentroServicioOrigen", guia.NombreCentroServicioOrigen));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_IdCentroServicioDestino", guia.IdCentroServicioDestino));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_NombreCentroServicioDestino", guia.NombreCentroServicioDestino));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_IdPaisOrigen", guia.IdPaisOrigen));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_NombrePaisOrigen", guia.NombrePaisOrigen));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_IdCiudadOrigen", guia.IdCiudadOrigen));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_NombreCiudadOrigen", guia.NombreCiudadOrigen));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_CodigoPostalOrigen", guia.CodigoPostalOrigen));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_IdPaisDestino", guia.IdPaisDestino));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_NombrePaisDestino", guia.NombrePaisDestino));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_IdCiudadDestino", guia.IdCiudadDestino));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_NombreCiudadDestino", guia.NombreCiudadDestino));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_CodigoPostalDestino", guia.CodigoPostalDestino));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_TelefonoDestinatario", guia.TelefonoDestinatario));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_DireccionDestinatario", guia.DireccionDestinatario));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_TipoCliente", guia.TipoCliente.ToString()));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_DiasDeEntrega", guia.DiasDeEntrega));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_FechaEstimadaEntrega", guia.FechaEstimadaEntrega));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_FechaEstimadaDigitalizacion", guia.FechaEstimadaDigitalizacion));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_FechaEstimadaArchivo", guia.FechaEstimadaArchivo));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_ValorAdmision", guia.ValorServicio));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_ValorTotal", guia.ValorTotal));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_ValorTotalImpuestos", guia.ValorTotalImpuestos));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_ValorTotalRetenciones", guia.ValorTotalRetenciones));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_ValorPrimaSeguro", guia.ValorPrimaSeguro));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_ValorEmpaque", guia.ValorEmpaque));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_ValorAdicionales", guia.ValorAdicionales));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_ValorDeclarado", guia.ValorDeclarado));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_DiceContener", guia.DiceContener));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_Observaciones", guia.Observaciones));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_NumeroPieza", guia.NumeroPieza));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_TotalPiezas", guia.TotalPiezas));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_FechaAdmision", guia.FechaAdmision.Year != 1 ? guia.FechaAdmision : DateTime.Now));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_Peso", guia.Peso));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_PesoLiqVolumetrico", guia.PesoLiqVolumetrico));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_PesoLiqMasa", guia.PesoLiqMasa));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_EsPesoVolumetrico", guia.EsPesoVolumetrico));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_NumeroBolsaSeguridad", guia.NumeroBolsaSeguridad));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_IdMotivoNoUsoBolsaSegurida", guia.IdMotivoNoUsoBolsaSegurida));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_MotivoNoUsoBolsaSeguriDesc", guia.MotivoNoUsoBolsaSeguriDesc));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_NoUsoaBolsaSeguridadObserv", guia.NoUsoaBolsaSeguridadObserv));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_IdUnidadMedida", guia.IdUnidadMedida));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_Largo", guia.Largo));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_Ancho", guia.Ancho));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_Alto", guia.Alto));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_EsRecomendado", guia.EsRecomendado));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_IdTipoEnvio", guia.IdTipoEnvio));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_NombreTipoEnvio", guia.NombreTipoEnvio));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_AdmisionSistemaMensajero", guia.AdmisionSistemaMensajero));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_EsAlCobro", guia.EsAlCobro));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_CreadoPor", ControllerContext.Current.Usuario));
            cmd.Parameters.Add(Utilidades.AddParametro("@EGT_IdEstadoGuia", (short)guia.EstadoGuia));
            cmd.Parameters.Add(Utilidades.AddParametro("@EGT_ObservacionEstadoGuia", guia.ObservacionEstadoGuia));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_IdTipoIdentificacionRemitente", tipoIdRemitente));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_IdRemitente", idRemitente));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_NombreRemitente", nombreRemitente));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_IdTipoIdentificacionDestinatario", tipoIdDestinatario));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_IdDestinatario", idDestinatario));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_NombreDestinatario", nombreDestinatario));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_TelefonoRemitente", telefonoRemitente));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_DireccionRemitente", direccionRemitente));
            cmd.Parameters.Add(Utilidades.AddParametro("@EGT_IdLocalidad", guia.IdCiudadOrigen));
            cmd.Parameters.Add(Utilidades.AddParametro("@EGT_NombreLocalidad", guia.NombreCiudadOrigen));
            cmd.Parameters.Add(Utilidades.AddParametro("@EGT_IdModulo", COConstantesModulos.MENSAJERIA));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_IdMensajero", guia.IdMensajero));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_NombreMensajero", guia.NombreMensajero));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_EstaPagada", guia.EstaPagada));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_FechaPago", guia.FechaPago));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_PrefijoNumeroGuia", guia.PrefijoNumeroGuia));
            cmd.Parameters.Add(Utilidades.AddParametro("@AMD_EsSupervisada", false));
            cmd.Parameters.Add(Utilidades.AddParametro("@AMD_FechaSupervision", ConstantesFramework.MinDateTimeController));
            cmd.Parameters.Add(Utilidades.AddParametro("@AMD_EmailRemitente", mailRemitente));
            cmd.Parameters.Add(Utilidades.AddParametro("@AMD_EmailDestinatario", mailDestinatario));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADM_NotificarEntregaPorEmail", guia.NotificarEntregaPorEmail));
            cmd.Parameters.Add(Utilidades.AddParametro("@NoPedido", guia.NoPedido));


            long? idAdmisionMensajeria = null;
            var idAdm = cmd.ExecuteScalar();
            if (idAdm != null)
                idAdmisionMensajeria = Convert.ToInt64(idAdm);

            AdicionarAdmisionOrigen(guia.NumeroGuia);

            if (idAdmisionMensajeria.HasValue)
                return idAdmisionMensajeria.Value;
            else
            {
                return 0;
            }

        }

        /// <summary>
        /// inserta en la tabla AdmisionMensajeriaOrigen_MEN desde que aplicativo se realizo la admision
        /// </summary>
        public void AdicionarAdmisionOrigen(long numeroGuia)
        {
            string cadenaTransaccional = System.Configuration.ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;
            int idAplicativoOrigen = ControllerContext.Current.IdAplicativoOrigen;
            string usuario = ControllerContext.Current.Usuario;
            Task.Factory.StartNew(() =>
                {
                    try
                    {
                        using (SqlConnection cnx = new SqlConnection(cadenaTransaccional))
                        {
                            cnx.Open();
                            SqlCommand cmd = new SqlCommand("paInsertarAdmisionMensajeriaOrigen_MEN", cnx);
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.Parameters.Add("@numeroGuia", numeroGuia);
                            cmd.Parameters.Add("@idAplicacion", idAplicativoOrigen);
                            cmd.Parameters.Add("@grabadoPor", usuario);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                });
        }


        /// <summary>
        /// Registra los rótulos asociados a una admisión, esto debe aplicar cuando se captura más de una pieza
        /// </summary>
        /// <param name="totalPiezas">Total de piezas que se admitieron</param>
        /// <param name="numeroGuia">Número de guía asociado a la admisión</param>
        /// <param name="idAdmisionMensajeria">Identificador de la guía admitida</param>
        public void AdicionarRotulosAdmision(short totalPiezas, long numeroGuia, long idAdmisionMensajeria)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                for (short i = 1; i <= totalPiezas; i++)
                {
                    contexto.AdmisionRotulos_MEN.Add(new AdmisionRotulos_MEN
                    {
                        ADR_CreadoPor = ControllerContext.Current.Usuario,
                        ADR_FechaGrabacion = DateTime.Now,
                        ADR_IdAdminisionMensajeria = idAdmisionMensajeria,
                        ADR_NumeroGuia = numeroGuia,
                        ADR_NumeroPieza = i,
                        ADR_TotalPiezas = totalPiezas
                    });
                }
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Registra los rótulos asociados a una admisión, esto debe aplicar cuando se captura más de una pieza
        /// </summary>
        /// <param name="totalPiezas">Total de piezas que se admitieron</param>
        /// <param name="numeroGuia">Número de guía asociado a la admisión</param>
        /// <param name="idAdmisionMensajeria">Identificador de la guía admitida</param>
        public void AdicionarRotulosAdmision(short totalPiezas, long numeroGuia, long idAdmisionMensajeria, SqlConnection conexion, SqlTransaction transaccion)
        {

            string strComand = @"INSERT INTO [AdmisionRotulos_MEN]
                                           ([ADR_IdAdminisionMensajeria]
                                           ,[ADR_NumeroPieza]
                                           ,[ADR_NumeroGuia]
                                           ,[ADR_TotalPiezas]
                                           ,[ADR_FechaGrabacion]
                                           ,[ADR_CreadoPor])
                                     VALUES
                                           (@ADR_IdAdminisionMensajeria
                                           ,@ADR_NumeroPieza
                                           ,@ADR_NumeroGuia
                                           ,@ADR_TotalPiezas
                                           ,GETDATE()
                                           ,@ADR_CreadoPor)";
            SqlCommand cmd = new SqlCommand(strComand, conexion, transaccion);
            cmd.CommandType = CommandType.Text;
            for (short i = 1; i <= totalPiezas; i++)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.Add(Utilidades.AddParametro("@ADR_IdAdminisionMensajeria", idAdmisionMensajeria));
                cmd.Parameters.Add(Utilidades.AddParametro("@ADR_NumeroPieza", i));
                cmd.Parameters.Add(Utilidades.AddParametro("@ADR_NumeroGuia", numeroGuia));
                cmd.Parameters.Add(Utilidades.AddParametro("@ADR_TotalPiezas", totalPiezas));
                cmd.Parameters.Add(Utilidades.AddParametro("@ADR_CreadoPor", ControllerContext.Current.Usuario));
                cmd.ExecuteNonQuery();
            }
        }


        /// <summary>
        /// Obtiene la admision de la mensajeria cliente convenio
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <returns></returns>
        public ADMensajeriaTipoCliente ObtenerAdmisionMensajeriaConvenioConvenio(long idAdmision)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ADMensajeriaTipoCliente tipoCliente = new ADMensajeriaTipoCliente();
                var admision = contexto.paObtenerAdmisionConvConv_MEN(idAdmision).FirstOrDefault();
                if (admision != null)
                {
                    tipoCliente.ConvenioRemitente = new ADConvenio()
                    {
                        Id = admision.MCC_IdConvenioRemitente,
                        Nit = admision.MCC_NitConvenioRemitente,
                        RazonSocial = admision.MCC_RazonSocialConvenioRemitente,
                        Contrato = admision.MCC_IdContratoConvenioRemite,
                        IdSucursalRecogida = admision.MCC_IdSucursalRecogida,
                        IdListaPrecios = admision.CON_ListaPrecios
                    };
                    tipoCliente.ConvenioDestinatario = new ADConvenio()
                    {
                        Id = admision.MCC_IdConvenioDestinatario,
                        Nit = admision.MCC_NitConvenioDestinatario,
                        RazonSocial = admision.MCC_RazonSocialConDestinatario,
                        Telefono = admision.MCC_TelefonoDestinatario,
                        EMail = admision.MCC_EmailDestinatario,
                        Direccion = admision.MCC_DireccionDestinatario
                    };
                    tipoCliente.IdContratoConvenioRemitente = admision.MCC_IdContratoConvenioRemite;
                }
                return tipoCliente;
            }
        }

        /// <summary>
        /// Obtiene la admision de la mensajeria cliente convenio
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <returns></returns>
        public ADMensajeriaTipoCliente ObtenerAdmisionMensajeriaConvenioPeaton(long idAdmision)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ADMensajeriaTipoCliente tipoCliente = new ADMensajeriaTipoCliente();
                var admision = contexto.paObtenerAdmisioConvPeaton_MEN(idAdmision).FirstOrDefault();

                if (admision != null)
                {
                    tipoCliente.ConvenioRemitente = new ADConvenio()
                    {
                        Id = admision.MCP_IdConvenioRemitente,
                        Nit = admision.MCP_NitConvenioRemitente,
                        RazonSocial = admision.MCP_RazonSocialConvenioRemitente,
                        IdSucursalRecogida = admision.MCP_IdSucursalRecogida,
                        Contrato = admision.MCP_IdContratoConvenioRemite,
                        IdListaPrecios = admision.CON_ListaPrecios
                    };
                    tipoCliente.PeatonDestinatario = new ADPeaton()
                    {
                        Nombre = admision.MCP_NombreDestinatario,
                        Apellido1 = admision.MCP_Apellido1Destinatario,
                        Apellido2 = admision.MCP_Apellido2Destinatario,
                        Email = admision.MCP_EmailDestinatario,
                        Telefono = admision.MCP_TelefonoDestinatario,
                        Direccion = admision.MCP_DireccionDestinatario,
                        Identificacion = admision.MCP_IdDestinatario,
                        TipoIdentificacion = admision.MCP_IdTipoIdDestinatario
                    };
                    tipoCliente.IdContratoConvenioRemitente = admision.MCP_IdContratoConvenioRemite;
                }

                return tipoCliente;
            }
        }

        /// <summary>
        /// Obtiene la admision de la mensajeria cliente convenio
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <returns></returns>
        public ADMensajeriaTipoCliente ObtenerAdmisionMensajeriaPeatonPeaton(long idAdmision)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ADMensajeriaTipoCliente tipoCliente = new ADMensajeriaTipoCliente();
                var admision = contexto.paObtenerAdmMenPeatonPeaton_MEN(idAdmision).FirstOrDefault();

                if (admision != null)
                {
                    tipoCliente.PeatonRemitente = new ADPeaton()
                    {
                        Nombre = admision.MPP_NombreRemitente,
                        Apellido1 = admision.MPP_Apellido1Remitente,
                        Apellido2 = admision.MPP_Apellido2Remitente,
                        Email = admision.MPP_EmailRemitente,
                        Telefono = admision.MPP_TelefonoRemitente,
                        Direccion = admision.MPP_DireccionRemitente,
                        Identificacion = admision.MPP_IdentificacionRemitente,
                        TipoIdentificacion = admision.MPP_IdTipoIdentificacionRemitente
                    };
                    tipoCliente.PeatonDestinatario = new ADPeaton()
                    {
                        Nombre = admision.MPP_NombreDestinatario,
                        Apellido1 = admision.MPP_Apellido1Destinatario,
                        Apellido2 = admision.MPP_Apellido2Destinatario,
                        Email = admision.MPP_EmailDestinatario,
                        Telefono = admision.MPP_TelefonoDestinatario,
                        Direccion = admision.MPP_DireccionDestinatario,
                        Identificacion = admision.MPP_IdDestinatario
                    };
                }

                return tipoCliente;
            }
        }

        /// <summary>
        /// Obtiene la admision de mensajeria peaton-convenio
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <returns></returns>
        public ADMensajeriaTipoCliente ObtenerAdmisionMensajeriaPeatonConvenio(long idAdmision)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ADMensajeriaTipoCliente tipoCliente = new ADMensajeriaTipoCliente();
                var admision = contexto.paObtenerAdmMenPeatonConvenio_MEN(idAdmision).FirstOrDefault();

                if (admision != null)
                {
                    tipoCliente.PeatonRemitente = new ADPeaton()
                    {
                        Nombre = admision.MPC_NombreRemitente,
                        Apellido1 = admision.MPC_Apellido1Remitente,
                        Apellido2 = admision.MPC_Apellido2Remitente,
                        Email = admision.MPC_EmailRemitente,
                        Telefono = admision.MPC_TelefonoRemitente,
                        Direccion = admision.MPC_DireccionRemitente,
                        Identificacion = admision.MPC_IdRemitente,
                        TipoIdentificacion = admision.MPC_IdTipoIdRemitente
                    };
                    tipoCliente.ConvenioDestinatario = new ADConvenio()
                    {
                        Nit = admision.MPC_NitConvenio,
                        RazonSocial = admision.MPC_RazonSocialConvenio,
                        Id = admision.MPC_IdConvenio,
                        IdSucursalRecogida = admision.MPC_IdSucursal
                    };
                }

                return tipoCliente;
            }
        }

        /// <summary>
        /// Método para adicionar una guia interna
        /// </summary>
        /// <param name="guiaInterna"></param>
        public void AdicionarGuiaInterna(ADGuiaInternaDC guiaInterna)
        {
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paCrearGuiaInterna_MEN", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@AGI_IdAdmisionMensajeria", guiaInterna.IdAdmisionGuia));
                cmd.Parameters.Add(new SqlParameter("@AGI_IdGestionOrigen", guiaInterna.GestionOrigen.IdGestion));
                cmd.Parameters.Add(new SqlParameter("@AGI_DescripcionGestionOrig", guiaInterna.GestionOrigen.Descripcion == null ? "" : guiaInterna.GestionOrigen.Descripcion));
                cmd.Parameters.Add(new SqlParameter("@AGI_IdGestionDestino", guiaInterna.GestionDestino.IdGestion));
                cmd.Parameters.Add(new SqlParameter("@AGI_DescripcionGestionDest", guiaInterna.GestionDestino.Descripcion == null ? "" : guiaInterna.GestionDestino.Descripcion));
                cmd.Parameters.Add(new SqlParameter("@AGI_NombreRemitente", guiaInterna.NombreRemitente));
                cmd.Parameters.Add(new SqlParameter("@AGI_TelefonoRemitente", guiaInterna.TelefonoRemitente));
                cmd.Parameters.Add(new SqlParameter("@AGI_DireccionRemitente", guiaInterna.DireccionRemitente));
                cmd.Parameters.Add(new SqlParameter("@AGI_NombreDestinatario", guiaInterna.NombreDestinatario));
                cmd.Parameters.Add(new SqlParameter("@AGI_TelefonoDestinatario", guiaInterna.TelefonoDestinatario));
                cmd.Parameters.Add(new SqlParameter("@AGI_DireccionDestinatario", guiaInterna.DireccionDestinatario));
                cmd.Parameters.Add(new SqlParameter("@AGI_Contenido", guiaInterna.DiceContener));
                cmd.Parameters.Add(new SqlParameter("@AGI_FechaGrabacion", DateTime.Now));
                cmd.Parameters.Add(new SqlParameter("@AGI_CreadoPor", ControllerContext.Current.Usuario));
                cmd.ExecuteNonQuery();
            }


        }

        /// <summary>
        /// Metodo que obtiene el id y el servicio de la admision a partir del numero de la guía
        /// </summary>
        /// <param name="numeroGuia">Numero de la guía</param>
        /// <returns>Identificador de la admisión de la guía</returns>
        public LIGuiaDC ValidarGuia(long numeroGuia)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                LIGuiaDC guia = new LIGuiaDC();
                var idGuia = contexto.paVerificarNumeroGuia_MEN(numeroGuia).FirstOrDefault();
                if (idGuia != null)
                {
                    guia.IdGuia = idGuia.ADM_IdAdminisionMensajeria;
                    guia.IdServicio = idGuia.ADM_IdServicio;
                }
                return guia;
            }

        }

        /// <summary>
        /// verifica si una guia existe
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public bool VerificarSiGuiaExiste(long numeroGuia)
        {

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paVerificarNumeroGuia_MEN", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@numeroGuia", numeroGuia);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                conn.Open();
                da.Fill(dt);
                conn.Close();

                var row = dt.AsEnumerable().ToList().FirstOrDefault();
                if (row != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


        /// <summary>
        /// Consulta una guía por guid
        /// </summary>
        /// <param name="guid">Valor que identifica la transacción</param>
        /// <returns>Número de guía</returns>
        public ADRetornoAdmision ObtenerGuiaPorGuid(string guid)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ADGuiaPorGuid admisionMensajeria = contexto.paObtenerNumeroGuiaPorGuid_MEN(guid).FirstOrDefault();
                if (admisionMensajeria != null)
                {
                    return new ADRetornoAdmision
                    {
                        NumeroGuia = admisionMensajeria.ADM_NumeroGuia,
                        FechaGrabacion = admisionMensajeria.ADM_FechaAdmision,
                        PrefijoGuia = admisionMensajeria.ADM_PrefijoNumeroGuia
                    };
                }
                else
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MENSAJERIA, ADEnumTipoErrorMensajeria.EX_GUIA_NO_EXISTE.ToString(), ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.EX_GUIA_NO_EXISTE)));
            }
        }

        /// <summary>
        /// Inserta en base de datos las formas de pago de mensajería
        /// </summary>
        /// <param name="formasPagoGuia"></param>
        /// <param name="usuario"></param>
        public void AdicionarGuiaFormasPago(long idAdmisionMensajeria, List<ADGuiaFormaPago> formasPagoGuia, string usuario)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                formasPagoGuia.ForEach(formaPago =>
                    contexto.paCrearAdmiGuiaFormaPago_MEN(idAdmisionMensajeria, formaPago.IdFormaPago, formaPago.Valor, usuario, formaPago.NumeroAsociadoFormaPago)
                  );
            }
        }

        /// <summary>
        /// Inserta en base de datos las formas de pago de mensajería
        /// </summary>
        /// <param name="formasPagoGuia"></param>
        /// <param name="usuario"></param>
        public void AdicionarGuiaFormasPago(long idAdmisionMensajeria, List<ADGuiaFormaPago> formasPagoGuia, string usuario, SqlConnection conexion, SqlTransaction transaccion)
        {
            formasPagoGuia.ForEach(formaPago =>
            {
                SqlCommand cmd = new SqlCommand("paCrearAdmiGuiaFormaPago_MEN", conexion, transaccion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(Utilidades.AddParametro("@AGF_IdAdminisionMensajeria", idAdmisionMensajeria));
                cmd.Parameters.Add(Utilidades.AddParametro("@AGF_IdFormaPago", formaPago.IdFormaPago));
                cmd.Parameters.Add(Utilidades.AddParametro("@AGF_Valor", formaPago.Valor));
                cmd.Parameters.Add(Utilidades.AddParametro("@AGF_CreadoPor", usuario));
                cmd.Parameters.Add(Utilidades.AddParametro("@AGF_NumeroAsociado", formaPago.NumeroAsociadoFormaPago));
                cmd.ExecuteNonQuery();
            }
              );
        }


        /// <summary>
        /// Inserta la información de convenio - convenio
        /// </summary>
        /// <param name="convenio"></param>
        /// <param name="usuario"></param>
        public void AdicionarConvenioConvenio(long idAdmisionesMensajeria, ADMensajeriaTipoCliente convenio, string usuario, int idCliente)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paCrearAdmConvenioConvenio_MEN(idAdmisionesMensajeria, convenio.FacturaRemitente, idCliente, convenio.ConvenioRemitente.Nit,
                  convenio.ConvenioRemitente.RazonSocial, convenio.IdContratoConvenioRemitente, convenio.ConvenioDestinatario.Id, convenio.ConvenioDestinatario.Nit, convenio.ConvenioDestinatario.RazonSocial,
                  convenio.ConvenioDestinatario.Telefono, convenio.ConvenioDestinatario.Direccion, convenio.ConvenioDestinatario.EMail, convenio.ConvenioRemitente.IdSucursalRecogida, usuario);
            }
        }
        /// <summary>
        /// Inserta la información de convenio - convenio
        /// </summary>
        /// <param name="convenio"></param>
        /// <param name="usuario"></param>
        public void AdicionarConvenioConvenio(long idAdmisionesMensajeria, ADMensajeriaTipoCliente convenio, string usuario, int idCliente, SqlConnection conexion, SqlTransaction transaccion)
        {
            SqlCommand cmd = new SqlCommand("paCrearAdmConvenioConvenio_MEN", conexion, transaccion);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(Utilidades.AddParametro("@MCC_IdAdminisionMensajeria", idAdmisionesMensajeria));
            cmd.Parameters.Add(Utilidades.AddParametro("@MCC_FacturaRemitente", convenio.FacturaRemitente));
            cmd.Parameters.Add(Utilidades.AddParametro("@MCC_IdConvenioRemitente", idCliente));
            cmd.Parameters.Add(Utilidades.AddParametro("@MCC_NitConvenioRemitente", convenio.ConvenioRemitente.Nit));
            cmd.Parameters.Add(Utilidades.AddParametro("@MCC_RazonSocialConvenioRemitente", convenio.ConvenioRemitente.RazonSocial));
            cmd.Parameters.Add(Utilidades.AddParametro("@MCC_IdContratoConvenioRemite", convenio.IdContratoConvenioRemitente));
            cmd.Parameters.Add(Utilidades.AddParametro("@MCC_IdConvenioDestinatario", convenio.ConvenioDestinatario.Id));
            cmd.Parameters.Add(Utilidades.AddParametro("@MCC_NitConvenioDestinatario", convenio.ConvenioDestinatario.Nit));
            cmd.Parameters.Add(Utilidades.AddParametro("@MCC_RazonSocialConDestinatario", convenio.ConvenioDestinatario.RazonSocial));
            cmd.Parameters.Add(Utilidades.AddParametro("@MCC_TelefonoDestinatario", convenio.ConvenioDestinatario.Telefono));
            cmd.Parameters.Add(Utilidades.AddParametro("@MCC_DireccionDestinatario", convenio.ConvenioDestinatario.Direccion));
            cmd.Parameters.Add(Utilidades.AddParametro("@MCC_EmailDestinatario", convenio.ConvenioDestinatario.EMail));
            cmd.Parameters.Add(Utilidades.AddParametro("@MCC_IdSucursalRecogida", convenio.ConvenioRemitente.IdSucursalRecogida));
            cmd.Parameters.Add(Utilidades.AddParametro("@MCC_CreadoPor", usuario));
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Inserta la información sde peatón - peatón
        /// </summary>
        /// <param name="peatonPeaton"></param>
        /// <param name="usuario"></param>
        public void AdicionarPeatonPeaton(long idAdmisionesMensajeria, ADMensajeriaTipoCliente peatonPeaton, string usuario, long idCentroServicioOrigen, string nombreCentroServicioOrigen)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paCrearAdmiPeatonPeaton_MEN(idAdmisionesMensajeria, idCentroServicioOrigen, nombreCentroServicioOrigen, peatonPeaton.PeatonRemitente.TipoIdentificacion,
                  peatonPeaton.PeatonRemitente.TipoIdentificacion, peatonPeaton.PeatonRemitente.Nombre, peatonPeaton.PeatonRemitente.Apellido1, peatonPeaton.PeatonRemitente.Apellido2,
                  peatonPeaton.PeatonRemitente.Telefono, peatonPeaton.PeatonRemitente.Direccion, peatonPeaton.PeatonRemitente.Email, peatonPeaton.PeatonDestinatario.TipoIdentificacion,
                  peatonPeaton.PeatonDestinatario.Identificacion, peatonPeaton.PeatonDestinatario.Nombre, peatonPeaton.PeatonDestinatario.Apellido1, peatonPeaton.PeatonDestinatario.Apellido2,
                  peatonPeaton.PeatonDestinatario.Telefono, peatonPeaton.PeatonDestinatario.Direccion, peatonPeaton.PeatonDestinatario.Email, usuario);
            }
        }

        /// <summary>
        /// Inserta la información sde peatón - peatón
        /// </summary>
        /// <param name="peatonPeaton"></param>
        /// <param name="usuario"></param>
        public void AdicionarPeatonPeaton(long idAdmisionesMensajeria, ADMensajeriaTipoCliente peatonPeaton, string usuario, long idCentroServicioOrigen, string nombreCentroServicioOrigen, SqlConnection conexion, SqlTransaction transaccion)
        {

            SqlCommand cmd = new SqlCommand("paCrearAdmiPeatonPeaton_MEN", conexion, transaccion);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(Utilidades.AddParametro("@MPP_IdAdminisionMensajeria", idAdmisionesMensajeria));
            cmd.Parameters.Add(Utilidades.AddParametro("@MPP_IdCentroServicioOrigen", idCentroServicioOrigen));
            cmd.Parameters.Add(Utilidades.AddParametro("@MPP_NombreCentroServicioOrigen", nombreCentroServicioOrigen));
            cmd.Parameters.Add(Utilidades.AddParametro("@MPP_IdTipoIdentificacionRemitente", peatonPeaton.PeatonRemitente.TipoIdentificacion));
            cmd.Parameters.Add(Utilidades.AddParametro("@MPP_IdentificacionRemitente", peatonPeaton.PeatonRemitente.Identificacion));
            cmd.Parameters.Add(Utilidades.AddParametro("@MPP_NombreRemitente", peatonPeaton.PeatonRemitente.Nombre));
            cmd.Parameters.Add(Utilidades.AddParametro("@MPP_Apellido1Remitente", peatonPeaton.PeatonRemitente.Apellido1));
            cmd.Parameters.Add(Utilidades.AddParametro("@MPP_Apellido2Remitente", peatonPeaton.PeatonRemitente.Apellido2));
            cmd.Parameters.Add(Utilidades.AddParametro("@MPP_TelefonoRemitente", peatonPeaton.PeatonRemitente.Telefono));
            cmd.Parameters.Add(Utilidades.AddParametro("@MPP_DireccionRemitente", peatonPeaton.PeatonRemitente.Direccion));
            cmd.Parameters.Add(Utilidades.AddParametro("@MPP_EmailRemitente", peatonPeaton.PeatonRemitente.Email));
            cmd.Parameters.Add(Utilidades.AddParametro("@MPP_TipoIdDestinatario", peatonPeaton.PeatonDestinatario.TipoIdentificacion));
            cmd.Parameters.Add(Utilidades.AddParametro("@MPP_IdDestinatario", peatonPeaton.PeatonDestinatario.Identificacion));
            cmd.Parameters.Add(Utilidades.AddParametro("@MPP_NombreDestinatario", peatonPeaton.PeatonDestinatario.Nombre));
            cmd.Parameters.Add(Utilidades.AddParametro("@MPP_Apellido1Destinatario", peatonPeaton.PeatonDestinatario.Apellido1));
            cmd.Parameters.Add(Utilidades.AddParametro("@MPP_Apellido2Destinatario", peatonPeaton.PeatonDestinatario.Apellido2));
            cmd.Parameters.Add(Utilidades.AddParametro("@MPP_TelefonoDestinatario", peatonPeaton.PeatonDestinatario.Telefono));
            cmd.Parameters.Add(Utilidades.AddParametro("@MPP_DireccionDestinatario", peatonPeaton.PeatonDestinatario.Direccion));
            cmd.Parameters.Add(Utilidades.AddParametro("@MPP_EmailDestinatario", peatonPeaton.PeatonDestinatario.Email));
            cmd.Parameters.Add(Utilidades.AddParametro("@MPP_CreadoPor", usuario));

            cmd.ExecuteNonQuery();

        }

        /// <summary>
        /// Inserta la información de un envío convenio - peatón
        /// </summary>
        /// <param name="idAdmisionesMensajeria"></param>
        /// <param name="convenioPeaton"></param>
        /// <param name="usuario"></param>
        public void AdicionarConvenioPeaton(long idAdmisionesMensajeria, ADMensajeriaTipoCliente convenioPeaton, string usuario, int idCliente)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paCrearAdmiConvenioPeaton_MEN(idAdmisionesMensajeria, idCliente, convenioPeaton.ConvenioRemitente.Nit, convenioPeaton.ConvenioRemitente.RazonSocial,
                  convenioPeaton.IdContratoConvenioRemitente, convenioPeaton.PeatonDestinatario.TipoIdentificacion, convenioPeaton.PeatonDestinatario.Identificacion,
                  convenioPeaton.PeatonDestinatario.Nombre, convenioPeaton.PeatonDestinatario.Apellido1, convenioPeaton.PeatonDestinatario.Apellido2, convenioPeaton.PeatonDestinatario.Telefono,
                  convenioPeaton.PeatonDestinatario.Direccion, convenioPeaton.PeatonDestinatario.Email, convenioPeaton.ConvenioRemitente.IdSucursalRecogida, usuario);
            }
        }
        /// <summary>
        /// Inserta la información de un envío convenio - peatón
        /// </summary>
        /// <param name="idAdmisionesMensajeria"></param>
        /// <param name="convenioPeaton"></param>
        /// <param name="usuario"></param>
        public void AdicionarConvenioPeaton(long idAdmisionesMensajeria, ADMensajeriaTipoCliente convenioPeaton, string usuario, int idCliente, SqlConnection conexion, SqlTransaction transaccion)
        {

            SqlCommand cmd = new SqlCommand("paCrearAdmiConvenioPeaton_MEN", conexion, transaccion);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add(Utilidades.AddParametro("@MCP_IdAdminisionMensajeria", idAdmisionesMensajeria));
            cmd.Parameters.Add(Utilidades.AddParametro("@MCP_IdConvenioRemitente", idCliente));
            cmd.Parameters.Add(Utilidades.AddParametro("@MCP_NitConvenioRemitente", convenioPeaton.ConvenioRemitente.Nit));
            cmd.Parameters.Add(Utilidades.AddParametro("@MCP_RazonSocialConvenioRemitente", convenioPeaton.ConvenioRemitente.RazonSocial));
            cmd.Parameters.Add(Utilidades.AddParametro("@MCP_IdContratoConvenioRemite", convenioPeaton.IdContratoConvenioRemitente));
            cmd.Parameters.Add(Utilidades.AddParametro("@MCP_IdTipoIdDestinatario", convenioPeaton.PeatonDestinatario.TipoIdentificacion));
            cmd.Parameters.Add(Utilidades.AddParametro("@MCP_IdDestinatario", convenioPeaton.PeatonDestinatario.Identificacion));
            cmd.Parameters.Add(Utilidades.AddParametro("@MCP_NombreDestinatario", convenioPeaton.PeatonDestinatario.Nombre));
            cmd.Parameters.Add(Utilidades.AddParametro("@MCP_Apellido1Destinatario", convenioPeaton.PeatonDestinatario.Apellido1));
            cmd.Parameters.Add(Utilidades.AddParametro("@MCP_Apellido2Destinatario", convenioPeaton.PeatonDestinatario.Apellido2));
            cmd.Parameters.Add(Utilidades.AddParametro("@MCP_TelefonoDestinatario", convenioPeaton.PeatonDestinatario.Telefono));
            cmd.Parameters.Add(Utilidades.AddParametro("@MCP_DireccionDestinatario", convenioPeaton.PeatonDestinatario.Direccion));
            cmd.Parameters.Add(Utilidades.AddParametro("@MCP_EmailDestinatario", convenioPeaton.PeatonDestinatario.Email));
            cmd.Parameters.Add(Utilidades.AddParametro("@MCP_IdSucursalRecogida", convenioPeaton.ConvenioRemitente.IdSucursalRecogida));
            cmd.Parameters.Add(Utilidades.AddParametro("@MCP_CreadoPor", usuario));

            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Inserta la información de un envío peatón - convenio
        /// </summary>
        /// <param name="idAdmisionesMensajeria"></param>
        /// <param name="peatonConvenio"></param>
        /// <param name="usuario"></param>
        public void AdicionarPeatonConvenio(int idCliente, long idAdmisionesMensajeria, ADMensajeriaTipoCliente peatonConvenio, string usuario)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paCrearAdmiPeatonConvenio_MEN(idAdmisionesMensajeria, idCliente, peatonConvenio.ConvenioDestinatario.Nit, peatonConvenio.ConvenioDestinatario.RazonSocial,
                  peatonConvenio.PeatonRemitente.TipoIdentificacion, peatonConvenio.PeatonRemitente.Identificacion, peatonConvenio.PeatonRemitente.Nombre, peatonConvenio.PeatonRemitente.Apellido1,
                  peatonConvenio.PeatonRemitente.Apellido2, peatonConvenio.PeatonRemitente.Telefono, peatonConvenio.PeatonRemitente.Direccion, peatonConvenio.ConvenioDestinatario.IdSucursalRecogida,
                  peatonConvenio.ConvenioDestinatario.Contrato, peatonConvenio.PeatonRemitente.Email, usuario);
            }
        }

        /// <summary>
        /// Inserta la información de un envío peatón - convenio
        /// </summary>
        /// <param name="idAdmisionesMensajeria"></param>
        /// <param name="peatonConvenio"></param>
        /// <param name="usuario"></param>
        public void AdicionarPeatonConvenio(int idCliente, long idAdmisionesMensajeria, ADMensajeriaTipoCliente peatonConvenio, string usuario, SqlConnection conexion, SqlTransaction transaccion)
        {

            SqlCommand cmd = new SqlCommand("paCrearAdmiPeatonConvenio_MEN", conexion, transaccion);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(Utilidades.AddParametro("@MPC_IdAdminisionMensajeria", idAdmisionesMensajeria));
            cmd.Parameters.Add(Utilidades.AddParametro("@MPC_IdConvenio", idCliente));
            cmd.Parameters.Add(Utilidades.AddParametro("@MPC_NitConvenio", peatonConvenio.ConvenioDestinatario.Nit));
            cmd.Parameters.Add(Utilidades.AddParametro("@MPC_RazonSocialConvenio", peatonConvenio.ConvenioDestinatario.RazonSocial));
            cmd.Parameters.Add(Utilidades.AddParametro("@MPC_IdTipoIdRemitente", peatonConvenio.PeatonRemitente.TipoIdentificacion));
            cmd.Parameters.Add(Utilidades.AddParametro("@MPC_IdRemitente", peatonConvenio.PeatonRemitente.Identificacion));
            cmd.Parameters.Add(Utilidades.AddParametro("@MPC_NombreRemitente", peatonConvenio.PeatonRemitente.Nombre));
            cmd.Parameters.Add(Utilidades.AddParametro("@MPC_Apellido1Remitente", peatonConvenio.PeatonRemitente.Apellido1));
            cmd.Parameters.Add(Utilidades.AddParametro("@MPC_Apellido2Remitente", peatonConvenio.PeatonRemitente.Apellido2));
            cmd.Parameters.Add(Utilidades.AddParametro("@MPC_TelefonoRemitente", peatonConvenio.PeatonRemitente.Telefono));
            cmd.Parameters.Add(Utilidades.AddParametro("@MPC_DireccionRemitente", peatonConvenio.PeatonRemitente.Direccion));
            cmd.Parameters.Add(Utilidades.AddParametro("@MPC_IdSucursal", peatonConvenio.ConvenioDestinatario.IdSucursalRecogida));
            cmd.Parameters.Add(Utilidades.AddParametro("@MPC_IdContratoConvenio", peatonConvenio.ConvenioDestinatario.Contrato));
            cmd.Parameters.Add(Utilidades.AddParametro("@MPC_EmailRemitente", peatonConvenio.PeatonRemitente.Email));
            cmd.Parameters.Add(Utilidades.AddParametro("@MPC_CreadoPor", usuario));
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Inserta los valores adicionales pasados
        /// </summary>
        /// <param name="idAdmisionMensajeria"></param>
        /// <param name="valoresAdicionales"></param>
        /// <param name="usuario"></param>
        public void AdicionarValoresAdicionales(long idAdmisionMensajeria, List<TAValorAdicional> valoresAdicionales, string usuario)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                if (valoresAdicionales != null && valoresAdicionales.Any())
                {
                    valoresAdicionales.ForEach(valorAdicional =>
                      contexto.paCrearAdmiValorAdicional_MEN(idAdmisionMensajeria, valorAdicional.IdTipoValorAdicional, valorAdicional.Descripcion, valorAdicional.PrecioValorAdicional, usuario));
                }
            }
        }

        /// <summary>
        /// Inserta los valores adicionales pasados
        /// </summary>
        /// <param name="idAdmisionMensajeria"></param>
        /// <param name="valoresAdicionales"></param>
        /// <param name="usuario"></param>
        public void AdicionarValoresAdicionales(long idAdmisionMensajeria, List<TAValorAdicional> valoresAdicionales, string usuario, SqlConnection conexion, SqlTransaction transaccion)
        {

            if (valoresAdicionales != null && valoresAdicionales.Any())
            {
                valoresAdicionales.ForEach(valorAdicional =>
                {
                    SqlCommand cmd = new SqlCommand("paCrearAdmiValorAdicional_MEN", conexion, transaccion);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(Utilidades.AddParametro("@AVA_IdAdminisionMensajeria", idAdmisionMensajeria));
                    cmd.Parameters.Add(Utilidades.AddParametro("@AVA_IdTipoValorAdicional", valorAdicional.IdTipoValorAdicional));
                    cmd.Parameters.Add(Utilidades.AddParametro("@AVA_Descripcion", valorAdicional.Descripcion));
                    cmd.Parameters.Add(Utilidades.AddParametro("@AVA_Valor", valorAdicional.PrecioValorAdicional));
                    cmd.Parameters.Add(Utilidades.AddParametro("@AVA_CreadoPor", usuario));

                    cmd.ExecuteNonQuery();

                });

            }

        }

        /// <summary>
        ///Actualiza los Reintentos de entrega de una admision
        /// </summary>
        /// <param name="idAdmisionMensajeria"></param>
        public void ActualizarReintentosEntregaAdmision(long idAdmisionMensajeria)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paActualizarReintenEntreAdmiMen_MEN(idAdmisionMensajeria);
            }
        }

        public void ActualizarReintentosEntregaAdmisionAdo(long idAdmisionMensajeria)
        {
            using (SqlConnection sqlcon = new SqlConnection(conexionStringController))
            {
                sqlcon.Open();
                SqlCommand cmd = new SqlCommand("paActualizarReintenEntreAdmiMen_MEN", sqlcon);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idAdmision", idAdmisionMensajeria);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Inserta la admisión de una notificación
        /// </summary>
        /// <param name="idAdmisionesMensajeria"></param>
        /// <param name="notificacion"></param>
        /// <param name="usuario"></param>
        public void AdicionarNotificacion(long idAdmisionesMensajeria, ADNotificacion notificacion, string usuario)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paCrearAdmisionNotificacio_MEN(idAdmisionesMensajeria, notificacion.TipoIdentificacionDestinatario, notificacion.IdDestinatario, notificacion.NombreDestinatario,
                   notificacion.Apellido1Destinatario, notificacion.Apellido2Destinatario, notificacion.TelefonoDestinatario, notificacion.DireccionDestinatario, notificacion.EmailDestinatario,
                   notificacion.CiudadDestino.IdLocalidad, notificacion.CiudadDestino.Nombre, notificacion.ReclamaEnOficina, notificacion.TipoDestino.Id, notificacion.TipoDestino.Descripcion, usuario);
            }
        }

        /// <summary>
        /// Inserta la admisión de una notificación
        /// </summary>
        /// <param name="idAdmisionesMensajeria"></param>
        /// <param name="notificacion"></param>
        /// <param name="usuario"></param>
        public void AdicionarNotificacion(long idAdmisionesMensajeria, ADNotificacion notificacion, string usuario, SqlConnection conexion, SqlTransaction transaccion)
        {

            SqlCommand cmd = new SqlCommand("paCrearAdmisionNotificacio_MEN", conexion, transaccion);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(Utilidades.AddParametro("@ADN_IdAdminisionMensajeria", idAdmisionesMensajeria));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADN_TipoIdDestinatario", notificacion.TipoIdentificacionDestinatario));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADN_IdDestinatario", notificacion.IdDestinatario));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADN_NombreDestinatario", notificacion.NombreDestinatario));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADN_Apellido1Destinatario", notificacion.Apellido1Destinatario));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADN_Apellido2Destinatario", notificacion.Apellido2Destinatario));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADN_TelefonoDestinatario", notificacion.TelefonoDestinatario));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADN_DireccionDestinatario", notificacion.DireccionDestinatario));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADN_EmailDestinatario", notificacion.EmailDestinatario));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADN_IdCiudadDestino", notificacion.CiudadDestino.IdLocalidad));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADN_NombreCiudadDestino", notificacion.CiudadDestino.Nombre));
            cmd.Parameters.Add(Utilidades.AddParametro("@ARR_ReclamaEnOficina", notificacion.ReclamaEnOficina));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADN_IdTipoDestino", notificacion.TipoDestino.Id));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADN_TipoDestino", notificacion.TipoDestino.Descripcion));
            cmd.Parameters.Add(Utilidades.AddParametro("@ADN_CreadoPor", usuario));
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Inserta admisión tipo de empaque
        /// </summary>
        /// <param name="idAdmisionesMensajeria"></param>
        /// <param name="tipoEmpaque"></param>
        /// <param name="usuario"></param>
        public void AdicionarAdmisionTipoEmpaque(long idAdmisionesMensajeria, TATipoEmpaque tipoEmpaque, string usuario)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paCrearAdmisionTipoEmpaque_MEN(idAdmisionesMensajeria, tipoEmpaque.IdTipoEmpaque, tipoEmpaque.Descripcion, usuario);
            }
        }


        // todo:id Campos Adicionales en Guia Internacional (Nueva Tabla uno a uno con AdmisionMensajeria_MEN)
        public void AdicionarAdmisionInternacional(ADGuiaInternacionalDC guiaInternacional, string usuario, SqlConnection conexion, SqlTransaction transaccion)
        {

            SqlCommand cmd = new SqlCommand("paInsertarAdmisionInternacional_MEN", conexion, transaccion);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(Utilidades.AddParametro("@IdAdmision", guiaInternacional.IdAdmision));
            cmd.Parameters.Add(Utilidades.AddParametro("@NumeroGuia", guiaInternacional.NumeroGuia));
            cmd.Parameters.Add(Utilidades.AddParametro("@NumeroGuiaDHL", guiaInternacional.NumeroGuiaDHL));
            cmd.Parameters.Add(Utilidades.AddParametro("@FechaEstimadaEntregaDHL", guiaInternacional.FechaEstimadaEntregaDHL));

            cmd.Parameters.Add(Utilidades.AddParametro("@IdPaisDestino", guiaInternacional.IdPaisDestino));
            cmd.Parameters.Add(Utilidades.AddParametro("@NombrePaisDestino", guiaInternacional.NombrePaisDestino));

            cmd.Parameters.Add(Utilidades.AddParametro("@IdDivPolitica", guiaInternacional.IdDivPolitica));
            cmd.Parameters.Add(Utilidades.AddParametro("@NombreDivPolitica", guiaInternacional.NombreDivPolitica));

            cmd.Parameters.Add(Utilidades.AddParametro("@IdCiudadDestino", guiaInternacional.IdCiudadDestino));
            cmd.Parameters.Add(Utilidades.AddParametro("@NombreCiudadDestino", guiaInternacional.NombreCiudadDestino));

            cmd.Parameters.Add(Utilidades.AddParametro("@CodigoPostalDestino", guiaInternacional.CodigoPostalDestino));
            cmd.Parameters.Add(Utilidades.AddParametro("@DireccionDestinatario", guiaInternacional.DireccionDestinatario));
            cmd.Parameters.Add(Utilidades.AddParametro("@IdTipoEmpaque", guiaInternacional.IdTipoEmpaque));
            cmd.Parameters.Add(Utilidades.AddParametro("@TipoEmpaqueNombre", guiaInternacional.TipoEmpaqueNombre));

            cmd.Parameters.Add(Utilidades.AddParametro("@ResponseDHL", guiaInternacional.arrByteXMLresponse));

            cmd.Parameters.Add(Utilidades.AddParametro("@RequestTarifa", guiaInternacional.RequestTarifa));
            cmd.Parameters.Add(Utilidades.AddParametro("@ResponseTarifa", guiaInternacional.ResponseTarifa));
            cmd.Parameters.Add(Utilidades.AddParametro("@RequestGuia", guiaInternacional.RequestGuia));
            cmd.Parameters.Add(Utilidades.AddParametro("@ResponseGuia", guiaInternacional.ResponseGuia));

            cmd.Parameters.Add(Utilidades.AddParametro("@Usuario", usuario));
            cmd.ExecuteNonQuery();

        }


        // todo:id Obtiene Datos Cuando la Guia es Internacion DHL de la
        public ADGuiaInternacionalDC ObtenerDatosGuiaInternacionalDHL(long IdGuiaDHL)
        {
            ADGuiaInternacionalDC GuiaInternacional = null;

            try
            {
                using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
                {
                    sqlConn.Open();

                    string cadSQL = @"SELECT ADI_IdAdmisionMensajeria, ADI_NumeroGuia, ADI_NumeroGuiaDHL"
                            + ", ADI_IdPaisDestino, ADI_NombrePaisDestino"
                            + ", ADI_IdDivPolitica, ADI_NombreDivPolitica"
                            + ", ADI_IdCiudadDestino, ADI_NombreCiudadDestino, ADI_CodigoPostalDestino"
                            + ", ADI_DireccionDestinatario, ADI_IdTipoEmpaque, ADI_TipoEmpaqueNombre"
                            + ", ADI_ResponseDHL"
                            + " FROM AdmisionInternacional_MEN WITH(NOLOCK) WHERE ADI_NumeroGuiaDHL = @IdGuiaDHL";

                    SqlCommand cmd = new SqlCommand(cadSQL, sqlConn);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(Utilidades.AddParametro("@IdGuiaDHL", IdGuiaDHL));

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        GuiaInternacional = new ADGuiaInternacionalDC();
                        GuiaInternacional.IdAdmision = Convert.ToInt64(reader["ADI_IdAdmisionMensajeria"]);
                        GuiaInternacional.NumeroGuia = Convert.ToInt64(reader["ADI_NumeroGuia"]);
                        GuiaInternacional.NumeroGuiaDHL = Convert.ToInt64(reader["ADI_NumeroGuiaDHL"]);

                        GuiaInternacional.CodigoPostalDestino = reader["ADI_IdPaisDestino"].ToString();
                        GuiaInternacional.NombrePaisDestino = reader["ADI_NombrePaisDestino"].ToString();
                        GuiaInternacional.IdDivPolitica = reader["ADI_IdDivPolitica"].ToString();
                        GuiaInternacional.NombreDivPolitica = reader["ADI_NombreDivPolitica"].ToString();
                        GuiaInternacional.IdCiudadDestino = reader["ADI_IdCiudadDestino"].ToString();
                        GuiaInternacional.NombreCiudadDestino = reader["ADI_NombreCiudadDestino"].ToString();
                        GuiaInternacional.CodigoPostalDestino = reader["ADI_CodigoPostalDestino"].ToString();
                        GuiaInternacional.DireccionDestinatario = reader["ADI_DireccionDestinatario"].ToString();
                        GuiaInternacional.IdTipoEmpaque = Convert.ToInt32(reader["ADI_IdTipoEmpaque"]);
                        GuiaInternacional.TipoEmpaqueNombre = reader["ADI_TipoEmpaqueNombre"].ToString();
                        GuiaInternacional.arrByteXMLresponse = (byte[])reader["ADI_ResponseDHL"];
                    }
                    sqlConn.Close();

                    return GuiaInternacional;
                }
            }
            catch (Exception exc)
            {
                return null;
            }

        }



        /// <summary>
        /// Inserta admisión de un rapi envio contra pago
        /// </summary>
        /// <param name="idAdmisionMensajeria"></param>
        /// <param name="rapiEnvioContraPago"></param>
        /// <param name="usuario"></param>
        public void AdicionarRapiEnvioContraPago(long idAdmisionMensajeria, ADRapiEnvioContraPagoDC rapiEnvioContraPago, string usuario)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paCrearAdmiRapiEnvContPago_MEN(idAdmisionMensajeria, rapiEnvioContraPago.TipoIdentificacionDestinatario, rapiEnvioContraPago.IdentificacionDestinatario,
                  rapiEnvioContraPago.NombreDestinatario, rapiEnvioContraPago.Apellido1Destinatario, rapiEnvioContraPago.Apellido2Destinatario, rapiEnvioContraPago.TelefonoDestinatario,
                  rapiEnvioContraPago.DireccionDestinatario, rapiEnvioContraPago.EmailDestinatario, rapiEnvioContraPago.CiudadDestino.IdLocalidad, rapiEnvioContraPago.CiudadDestino.Nombre,
                  rapiEnvioContraPago.TipoDestino.Id, rapiEnvioContraPago.TipoDestino.Descripcion, rapiEnvioContraPago.ValorARecaudar, rapiEnvioContraPago.DescripcionProducto, usuario);
            }
        }

        /// <summary>
        /// Inserta admisión de un rapi envio contra pago
        /// </summary>
        /// <param name="idAdmisionMensajeria"></param>
        /// <param name="rapiEnvioContraPago"></param>
        /// <param name="usuario"></param>
        public void AdicionarRapiEnvioContraPago(long idAdmisionMensajeria, ADRapiEnvioContraPagoDC rapiEnvioContraPago, string usuario, SqlConnection conexion, SqlTransaction transaccion)
        {
            SqlCommand cmd = new SqlCommand("paCrearAdmiRapiEnvContPago_MEN", conexion, transaccion);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(Utilidades.AddParametro("@ARP_IdAdminisionMensajeria", idAdmisionMensajeria));
            cmd.Parameters.Add(Utilidades.AddParametro("@ARP_TipoIdDestinatario", rapiEnvioContraPago.TipoIdentificacionDestinatario));
            cmd.Parameters.Add(Utilidades.AddParametro("@ARP_IdDestinatario", rapiEnvioContraPago.IdentificacionDestinatario));
            cmd.Parameters.Add(Utilidades.AddParametro("@ARP_NombreDestinatario", rapiEnvioContraPago.NombreDestinatario));
            cmd.Parameters.Add(Utilidades.AddParametro("@ARP_Apellido1Destinatario", rapiEnvioContraPago.Apellido1Destinatario));
            cmd.Parameters.Add(Utilidades.AddParametro("@ARP_Apellido2Destinatario", rapiEnvioContraPago.Apellido2Destinatario));
            cmd.Parameters.Add(Utilidades.AddParametro("@ARP_TelefonoDestinatario", rapiEnvioContraPago.TelefonoDestinatario));
            cmd.Parameters.Add(Utilidades.AddParametro("@ARP_DireccionDestinatario", rapiEnvioContraPago.DireccionDestinatario));
            cmd.Parameters.Add(Utilidades.AddParametro("@ARP_EmailDestinatario", rapiEnvioContraPago.EmailDestinatario));
            cmd.Parameters.Add(Utilidades.AddParametro("@ARP_IdCiudadDestino", rapiEnvioContraPago.CiudadDestino.IdLocalidad));
            cmd.Parameters.Add(Utilidades.AddParametro("@ARP_NombreCiudadDestino", rapiEnvioContraPago.CiudadDestino.Nombre));
            cmd.Parameters.Add(Utilidades.AddParametro("@ARP_IdTipoDestino", rapiEnvioContraPago.TipoDestino.Id));
            cmd.Parameters.Add(Utilidades.AddParametro("@ARP_TipoDestino", rapiEnvioContraPago.TipoDestino.Descripcion));
            cmd.Parameters.Add(Utilidades.AddParametro("@ARP_ValorARecaudar", rapiEnvioContraPago.ValorARecaudar));
            cmd.Parameters.Add(Utilidades.AddParametro("@ARP_DescripcionProducto", rapiEnvioContraPago.DescripcionProducto));
            cmd.Parameters.Add(Utilidades.AddParametro("@ARP_CreadoPor", usuario));
            cmd.ExecuteNonQuery();
        }


        /// <summary>
        /// INserta admisión de un rapi radicado, puede tener mucha información de rapiradicado
        /// </summary>
        /// <param name="idAdmisionMensajeria"></param>
        /// <param name="rapiradicado"></param>
        /// <param name="usuario"></param>
        public void AdicionarRapiRadicado(long idAdmisionMensajeria, ADRapiRadicado rapiradicado, string usuario)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                string[] nombreDestinatario = rapiradicado.NombreDestinatario.Split(' ');
                contexto.paCrearAdmiRapiRadicado_MEN(idAdmisionMensajeria,
                  rapiradicado.TipoIdentificacionDestinatario,
                  rapiradicado.IdDestinatario,
                  nombreDestinatario.Count() >= 1 ? nombreDestinatario[0] : string.Empty,
                  nombreDestinatario.Count() >= 2 ? nombreDestinatario[1] : string.Empty,
                  nombreDestinatario.Count() >= 3 ? nombreDestinatario[2] : string.Empty,
                  rapiradicado.TelefonoDestinatario,
                  rapiradicado.DireccionDestinatario,
                  rapiradicado.EmailDestinatario,
                  rapiradicado.CiudadDestino.IdLocalidad,
                  rapiradicado.CiudadDestino.Nombre,
                  rapiradicado.NumeroFolios,
                  1,
                  rapiradicado.CodigoRapiRadicado,
                  rapiradicado.TipoDestino.Id == ADConstantes.TIPO_ENVIO_RECLAMA_EN_PUNTO,
                  usuario,
                  rapiradicado.TipoDestino.Id,
                  rapiradicado.TipoDestino.Descripcion);
            }
        }

        /// <summary>
        /// INserta admisión de un rapi radicado, puede tener mucha información de rapiradicado
        /// </summary>
        /// <param name="idAdmisionMensajeria"></param>
        /// <param name="rapiradicado"></param>
        /// <param name="usuario"></param>
        public void AdicionarRapiRadicado(long idAdmisionMensajeria, ADRapiRadicado rapiradicado, string usuario, SqlConnection conexion, SqlTransaction transaccion)
        {

            SqlCommand cmd = new SqlCommand("paCrearAdmiRapiRadicado_MEN", conexion, transaccion);
            cmd.CommandType = CommandType.StoredProcedure;
            string[] nombreDestinatario = rapiradicado.NombreDestinatario.Split(' ');
            cmd.Parameters.Add(Utilidades.AddParametro("@ARR_IdAdminisionMensajeria", idAdmisionMensajeria));
            cmd.Parameters.Add(Utilidades.AddParametro("@ARR_TipoIdDestinatario", rapiradicado.TipoIdentificacionDestinatario));
            cmd.Parameters.Add(Utilidades.AddParametro("@ARR_IdDestinatario", rapiradicado.IdDestinatario));
            cmd.Parameters.Add(Utilidades.AddParametro("@ARR_NombreDestinatario", nombreDestinatario.Count() >= 1 ? nombreDestinatario[0] : string.Empty));
            cmd.Parameters.Add(Utilidades.AddParametro("@ARR_Apellido1Destinatario", nombreDestinatario.Count() >= 2 ? nombreDestinatario[1] : string.Empty));
            cmd.Parameters.Add(Utilidades.AddParametro("@ARR_Apellido2Destinatario", nombreDestinatario.Count() >= 3 ? nombreDestinatario[2] : string.Empty));
            cmd.Parameters.Add(Utilidades.AddParametro("@ARR_TelefonoDestinatario", rapiradicado.TelefonoDestinatario));
            cmd.Parameters.Add(Utilidades.AddParametro("@ARR_DireccionDestinatario", rapiradicado.DireccionDestinatario));
            cmd.Parameters.Add(Utilidades.AddParametro("@ARR_EmailDestinatario", rapiradicado.EmailDestinatario));
            cmd.Parameters.Add(Utilidades.AddParametro("@ARR_IdCiudadDestino", rapiradicado.CiudadDestino.IdLocalidad));
            cmd.Parameters.Add(Utilidades.AddParametro("@ARR_NombreCiudadDestino", rapiradicado.CiudadDestino.Nombre));
            cmd.Parameters.Add(Utilidades.AddParametro("@ARR_NumeroFolios", rapiradicado.NumeroFolios));
            cmd.Parameters.Add(Utilidades.AddParametro("@ARR_ConsecutivoRadicado", 1));
            cmd.Parameters.Add(Utilidades.AddParametro("@ARR_CodigoRapiRadicado", rapiradicado.CodigoRapiRadicado));
            cmd.Parameters.Add(Utilidades.AddParametro("@ARR_ReclamaEnOficina", rapiradicado.TipoDestino.Id == ADConstantes.TIPO_ENVIO_RECLAMA_EN_PUNTO));
            cmd.Parameters.Add(Utilidades.AddParametro("@ARR_CreadoPor", usuario));
            cmd.Parameters.Add(Utilidades.AddParametro("@ARR_IdTipoDestino", rapiradicado.TipoDestino.Id));
            cmd.Parameters.Add(Utilidades.AddParametro("@ARR_TipoDestino", rapiradicado.TipoDestino.Descripcion));
            cmd.ExecuteNonQuery();

        }



        /// <summary>
        /// Actualizar destino de una guia
        /// </summary>
        /// <param name="centroServicioDestino"></param>
        public void ActualizarDestinoGuia(long idAdmisionMensajeria, PUCentroServiciosDC centroServicioDestino, CCReliquidacionDC valorReliquidado, TAEnumFormaPago? formaPago, string idTipoEntrega, string descripcionTipoEntrega)
        {
            AlmacenarAuditAdmisionMensajeria(idAdmisionMensajeria, EnumEstadoRegistro.MODIFICADO);
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paActualizaDestinoAdmMens_MEN", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdAdminisionMensajeria", idAdmisionMensajeria);
                cmd.Parameters.AddWithValue("@IdCentroServicioDestino", centroServicioDestino.IdCentroServicio);
                cmd.Parameters.AddWithValue("@NombreCentroServicioDestino", centroServicioDestino.Nombre);
                cmd.Parameters.AddWithValue("@IdPaisDestino", centroServicioDestino.IdPais);
                cmd.Parameters.AddWithValue("@NombrePaisDestino", centroServicioDestino.NombrePais);
                cmd.Parameters.AddWithValue("@IdCiudadDestino", centroServicioDestino.IdMunicipio);
                cmd.Parameters.AddWithValue("@NombreCiudadDestino", centroServicioDestino.NombreMunicipio);
                cmd.Parameters.AddWithValue("@CodigoPostalDestino", centroServicioDestino.CodigoPostal);
                cmd.Parameters.AddWithValue("@ValorTransporte", valorReliquidado.ValorTransporte);
                cmd.Parameters.AddWithValue("@ValorPrima", valorReliquidado.ValorPrimaSeguro);
                cmd.Parameters.AddWithValue("@FormaPago", "INTERNA");
                cmd.Parameters.AddWithValue("@IdTipoEntrega", idTipoEntrega);
                cmd.Parameters.AddWithValue("@DescripcionTipoEntrega", descripcionTipoEntrega);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            //using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //    AlmacenarAuditAdmisionMensajeria(idAdmisionMensajeria, EnumEstadoRegistro.MODIFICADO);

            //    contexto.paActualizaDestinoAdmMens_MEN(idAdmisionMensajeria,
            //        centroServicioDestino.IdCentroServicio,
            //        centroServicioDestino.Nombre,
            //        centroServicioDestino.IdPais,
            //        centroServicioDestino.NombrePais,
            //        centroServicioDestino.IdMunicipio,
            //        centroServicioDestino.NombreMunicipio,
            //        centroServicioDestino.CodigoPostal,
            //        valorReliquidado.ValorTransporte,
            //        valorReliquidado.ValorPrimaSeguro, "INTERNA");

            //    contexto.SaveChanges();
            //}
        }

        /// <summary>
        /// Actualizar formas de pago de una guia
        /// </summary>
        public void ActualizarFormaPagoGuia(CCNovedadCambioFormaPagoDC novedadGuia, EnumEstadoRegistro tipoModificacion)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                //Mixta
                if (novedadGuia.FormaPagoNueva.IdFormaPago != -1)
                {
                    contexto.paActualizarFormPagoAdmMen_MEN(
                      novedadGuia.Guia.IdAdmision,
                      novedadGuia.FormaPagoAnterior.IdFormaPago,
                      novedadGuia.FormaPagoNueva.IdFormaPago,
                      novedadGuia.FormaPagoNueva.Valor,
                      DateTime.Now,
                      ControllerContext.Current.Usuario,
                      DateTime.Now,
                      ControllerContext.Current.Usuario,
                      tipoModificacion.ToString());
                }
                else
                {
                    contexto.paActualizarFormPagoAdmMen_MEN(
                        novedadGuia.Guia.IdAdmision,
                        novedadGuia.FormaPagoAnterior.IdFormaPago,
                        (short)TAEnumFormaPago.EFECTIVO,
                        novedadGuia.ValorContadoMixta,
                        DateTime.Now,
                        ControllerContext.Current.Usuario,
                        DateTime.Now,
                        ControllerContext.Current.Usuario,
                        tipoModificacion.ToString());

                    contexto.paActualizarFormPagoAdmMen_MEN(
                        novedadGuia.Guia.IdAdmision,
                        0,
                        (short)TAEnumFormaPago.PREPAGO,
                        novedadGuia.ValorPrepagoMixta,
                        DateTime.Now,
                        ControllerContext.Current.Usuario,
                        DateTime.Now,
                        ControllerContext.Current.Usuario,
                        tipoModificacion.ToString());
                }
            }
        }

        /// <summary>
        /// Actualiza en la base de datos los datos del remitente y del destinatario de una guía o factura
        /// </summary>
        /// <param name="novedadGuia"></param>
        public void ActualizarRemitenteDestinatarioGuia(CCNovedadCambioRemitenteDC novedadGuia)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                AlmacenarAuditAdmisionMensajeria(novedadGuia.Guia.IdAdmision, EnumEstadoRegistro.MODIFICADO);

                contexto.paActualizarDatosBasicosGuia_MEN(novedadGuia.Guia.IdAdmision, novedadGuia.RemitenteNuevo.Identificacion, novedadGuia.RemitenteNuevo.NombreYApellidos, novedadGuia.RemitenteNuevo.Telefono,
                  novedadGuia.RemitenteNuevo.Direccion, novedadGuia.DestinatarioNuevo.Identificacion, novedadGuia.DestinatarioNuevo.NombreYApellidos, novedadGuia.DestinatarioNuevo.Telefono,
                  novedadGuia.DestinatarioNuevo.Direccion);

                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Actualiza el tipo de servicio de una admisión
        /// </summary>
        /// <param name="novedadGuia"></param>
        public void ActualizarTipoServicioGuia(long idAdmisionGuia, int idServicio)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                AlmacenarAuditAdmisionMensajeria(idAdmisionGuia, EnumEstadoRegistro.MODIFICADO);
                contexto.paActualizarServicioAdmMen_MEN(idAdmisionGuia, idServicio);
            }
        }

        /// <summary>
        /// Actualizar el valor total de una guía dada
        /// </summary>
        /// <param name="idAdmisionGuia"></param>
        /// <param name="ValorTotal"></param>
        public void ActualizarValorTotalGuia(CCNovedadCambioValorTotal novedadGuia)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                AlmacenarAuditAdmisionMensajeria(novedadGuia.Guia.IdAdmision, EnumEstadoRegistro.MODIFICADO);
                contexto.paActualizarValorTotalAdmMen_MEN(novedadGuia.Guia.IdAdmision, novedadGuia.NuevoValorTransporte, novedadGuia.NuevoValorPrima, novedadGuia.NuevoValorComercial);
            }
        }

        /// <summary>
        /// Actualizar valores de la guía dada
        /// </summary>
        /// <param name="idAdmisionGuia">nmero de la guia</param>
        /// <param name="valores">valores a modificar</param>
        /// <param name="valorAdicionales">valores adicionales que se agregan al total</param>
        public void ActualizarValoresGuia(long idAdmisionGuia, CCReliquidacionDC valores, decimal valorAdicionales)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                AlmacenarAuditAdmisionMensajeria(idAdmisionGuia, EnumEstadoRegistro.MODIFICADO);
                contexto.paActualizarValoresAdmMen_MEN(idAdmisionGuia, valores.ValorPrimaSeguro, valores.ValorTransporte, valores.ValorTotal + valorAdicionales + valores.ValorImpuestos);
            }
        }

        /// <summary>
        /// Actualiza el Valor del Peso de la Guía dada
        /// </summary>
        /// <param name="idAdmisionGuia">numeor de la Guía</param>
        /// <param name="valorPeso">Valor del peso a actualizar</param>
        public void ActualizarValorPesoGuia(long idAdmisionGuia, decimal valorPeso)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                AlmacenarAuditAdmisionMensajeria(idAdmisionGuia, EnumEstadoRegistro.MODIFICADO);
                contexto.paActualizarPesoAdmMen_MEN(idAdmisionGuia, valorPeso);
            }
        }

        /// <summary>
        /// Actualizar es alcobro
        /// </summary>
        public void ActualizarEsAlCobro(long idAdmisionGuia, bool esAlCobro)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                AlmacenarAuditAdmisionMensajeria(idAdmisionGuia, EnumEstadoRegistro.MODIFICADO);
                contexto.paActualizaEsAlCobroAdmMen_MEN(idAdmisionGuia, esAlCobro);
            }
        }

        /// <summary>
        /// Actualiza como pagada una guía de mensajeria
        /// </summary>
        /// <param name="idAdmisionMensajeria"></param>
        public void ActualizarPagadoGuia(long idAdmisionMensajeria, bool estaPagada = true)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                AlmacenarAuditAdmisionMensajeria(idAdmisionMensajeria, EnumEstadoRegistro.MODIFICADO);
                contexto.paActualizarGuiaComoPagada_MEN(idAdmisionMensajeria, estaPagada);
                //contexto.paActualizarGuiaComoPagada_MEN(idAdmisionMensajeria);
            }
        }

        public List<ADGuia> ObtenerReporteCajaGuiasMensajeroApp(long idMensajero)
        {
            List<ADGuia> lstGuias = new List<ADGuia>();
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerGuiasReporteCajasApp_OPU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdMensajero", idMensajero);                
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                lstGuias = new List<ADGuia>();
                while (reader.Read())
                {
                    ADGuia guiaMensajero = new ADGuia();

                    guiaMensajero.IdMensajero = Convert.ToInt64(reader["PAM_IdMensajero"]);
                    guiaMensajero.NumeroGuia = Convert.ToInt64(reader["PAG_NumeroGuia"]);
                    guiaMensajero.FechaGrabacion = Convert.ToDateTime(reader["PAE_FechaGrabacion"]);
                    guiaMensajero.DireccionDestinatario = Convert.ToString(reader["PAG_DireccionDestinatario"]);                    
                    guiaMensajero.ValorTotal = Convert.ToDecimal(reader["PAG_ValorTotalGuia"]);
                    guiaMensajero.IdServicio = Convert.ToInt32(reader["PAG_IdServicio"]);
                    guiaMensajero.NombreServicio = Convert.ToString(reader["PAG_NombreServicio"]);                    

                    var listaFormaPago = new List<ADGuiaFormaPago>();
                    listaFormaPago.Add(new ADGuiaFormaPago
                    {
                        IdFormaPago = Convert.ToInt16(reader["FOP_IdFormaPago"]),
                        Descripcion = Convert.ToString(reader["FOP_Descripcion"]),
                    });
                    guiaMensajero.FormasPago = listaFormaPago;                                       


                    lstGuias.Add(guiaMensajero);
                }
            }
            return lstGuias;
        }

        /// <summary>
        /// Actualiza en supervision una guía de mensajeria
        /// </summary>
        /// <param name="idAdmisionMensajeria"></param>
        public void ActualizarSupervisionGuia(long idAdmisionMensajeria)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                AlmacenarAuditAdmisionMensajeria(idAdmisionMensajeria, EnumEstadoRegistro.MODIFICADO);
                contexto.paActualizarGuiaSupervision_MEN(idAdmisionMensajeria);
            }
        }

        /// <summary>
        /// Metodo para eliminar una admisión con auditoria
        /// </summary>
        /// <param name="idAdmision"></param>
        public void EliminarAdmision(long idAdmision)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                EliminarTrazaAdmision(idAdmision);
                contexto.paEliminarAdmision_MEN(idAdmision, DateTime.Now, ControllerContext.Current.Usuario, ADConstantes.CAMBIO_ELIMINA);
            }
        }

        /// <summary>
        /// Metodo para eliminar la trazabilidad de estados de una admisión de emnsajeria
        /// </summary>
        /// <param name="idAdmision"></param>
        public void EliminarTrazaAdmision(long idAdmision)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paEliminarTrazaGuia_Men(idAdmision);
            }
        }

        /// <summary>
        /// Eliminar una guía interna
        /// </summary>
        /// <param name="manifiesto"></param>
        public void EliminarGuiaInterna(ADGuiaInternaDC guiaInterna)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paEliminarGuiaInterna_Men(guiaInterna.IdAdmisionGuia, ControllerContext.Current.Usuario, ADConstantes.CAMBIO_ELIMINA);
            }
        }

        /// <summary>
        /// Método para actualizar el número de guía interna
        /// </summary>
        /// <param name="idRadicado"></param>
        /// <param name="numeroGuiaInterna"></param>
        public void ActualizarGuiaRapiradicado(long idRadicado, long numeroGuiaInterna)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paActualizarRapiradicadosGuia_MEN(idRadicado, numeroGuiaInterna);
            }
        }

        /// <summary>
        /// Método para ingresar una guía a centro de acopio
        /// </summary>
        /// <param name="guia"></param>
        public void IngresarGuiaManualCentroAcopio(ADGuia guia)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paIngresarGuiaNoRegCentroAcopio_MEN(guia.NumeroGuia, guia.IdAdmision);
            }

        }

        /// <summary>
        /// Método para ingresar una guía a centro de acopio
        /// </summary>
        /// <param name="guia"></param>
        public void IngresarGuiaManualCentroAcopio(ADGuia guia, SqlConnection conexion, SqlTransaction transaccion)
        {
            SqlCommand cmd = new SqlCommand("paIngresarGuiaNoRegCentroAcopio_MEN", conexion, transaccion);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@NumeroGuia", guia.NumeroGuia));
            cmd.Parameters.Add(new SqlParameter("@IdAdmision", guia.IdAdmision));
            cmd.ExecuteNonQuery();
        }

        #endregion Guias

        #region Reexpedicion

        /// <summary>
        /// Método para actualizar el número de guía interna
        /// </summary>
        /// <param name="idRadicado"></param>
        /// <param name="numeroGuiaInterna"></param>
        public void GuadarRelacionReexpedicionEnvio(long idAdmisionOriginal, long idAdmisionNueva)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                AdminisionMensajeriaGuiaRelacion_MEN relacion = new AdminisionMensajeriaGuiaRelacion_MEN()
                {
                    AGR_IdAdminisionMensajeriaNueva = idAdmisionNueva,
                    AGR_IdAdminisionMensajeriaOriginal = idAdmisionOriginal,
                    AGR_FechaGrabacion = DateTime.Now,
                    AGR_CreadoPor = ControllerContext.Current.Usuario
                };

                contexto.AdminisionMensajeriaGuiaRelacion_MEN.Add(relacion);
                contexto.SaveChanges();
            }
        }

        #endregion Reexpedicion

        #region Novedades

        /// <summary>
        /// Adicionar novedades de una guia
        /// </summary>
        /// <param name="novedad"></param>
        public void AdicionarNovedad(ADNovedadGuiaDC novedad, Dictionary<CCEnumNovedadRealizada, string> datosAdicionalesNovedad)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                string observaciones = "";
                observaciones = novedad.Observaciones;
                if (observaciones == null)
                    observaciones = novedad.Observaciones;

                NovedadGuia_MEN novedadGuia = new NovedadGuia_MEN()
                {
                    NOG_NumeroGuia = novedad.Guia.NumeroGuia,
                    NOG_IdAdminisionMensajeria = novedad.Guia.IdAdmision,
                    NOG_IdentificacionResponsable = novedad.ResponsableNovedad.IdentificadorResponsable != null ? novedad.ResponsableNovedad.IdentificadorResponsable.ToString() : string.Empty,
                    NOG_NombreResponsable = novedad.ResponsableNovedad.DescripcionResponsable,
                    NOG_FechaGrabacion = DateTime.Now,
                    NOG_IdModulo = novedad.IdModulo,
                    NOG_CreadoPor = ControllerContext.Current.Usuario,
                    NOG_IdTipoNovedad = (short)novedad.TipoNovedad,
                    NOG_Observacion = observaciones == null ? "" : observaciones,
                    NOG_QuienSolicita = novedad.QuienSolicita,
                    NOG_CargoResponsable = novedad.ResponsableNovedad.CargoResponsable,
                    NOG_TipoResponsable = novedad.ResponsableNovedad.Responsable.ToString(),
                    NOG_ErrorDocFisico = novedad.ErrorDocFisico
                };

                contexto.NovedadGuia_MEN.Add(novedadGuia);

                foreach (KeyValuePair<CCEnumNovedadRealizada, string> datosAdicionales in datosAdicionalesNovedad)
                {
                    NovedadGuiaDatosAdicionale_MEN novedadesDatosAdd = new NovedadGuiaDatosAdicionale_MEN()
                    {
                        NDA_IdNovedadGuia = novedadGuia.NOG_IdNovedadGuia,
                        NDA_Nombre = datosAdicionales.Key.ToString(),
                        NDA_Valor = datosAdicionales.Value,
                        NDA_FechaGrabacion = DateTime.Now,
                        NDA_CreadoPor = ControllerContext.Current.Usuario
                    };

                    contexto.NovedadGuiaDatosAdicionale_MEN.Add(novedadesDatosAdd);
                }
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Método para actualizar
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <param name="numeroGuia"></param>
        public void ActualizarNotificacion(long idAdmision, long numeroGuia)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paActualizarNotificacion_MEN(idAdmision, numeroGuia);
            }
        }

        /// <summary>
        /// Método para actualizar una notificacion cuando es sacada de una planilla
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <param name="numeroGuia"></param>
        public void ActualizarNotificacionPlanilla(long idAdmision)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                AdmisionNotificaciones_MEN not = contexto.AdmisionNotificaciones_MEN.FirstOrDefault(n => n.ADN_IdAdminisionMensajeria == idAdmision);
                if (not != null)
                {
                    not.ADN_EstaDevuelta = false;
                    not.ADN_NumeroGuiaInterna = 0;
                    contexto.SaveChanges();
                }
            }
        }

        #endregion Novedades

        #region Historico

        /// <summary>
        /// Metodo de auditoria de mensajeria
        /// </summary>
        /// <param name="guia">guia a almacenar</param>
        private void AlmacenarAuditAdmisionMensajeria(long idAdmision, EnumEstadoRegistro tipoModificacion)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paInsertarAdmMenHistorico_MEN(
                  idAdmision,
                  DateTime.Now,
                  ControllerContext.Current.Usuario,
                  tipoModificacion.ToString()
                  );
            }
        }

        #endregion Historico

        #region Adicionar

        /// <summary>
        /// Adiciona archivo de un volante
        /// </summary>
        /// <param name="archivo">objeto de tipo archivo</param>
        public void AdicionarArchivo(LIArchivosDC archivo)
        {
            filePath = PAAdministrador.Instancia.ObtenerParametrosAplicacion(PAAdministrador.RutaArchivos);
            string rutaArchivo = Path.Combine(this.filePath, COConstantesModulos.MENSAJERIA, archivo.NombreServidor);
            byte[] archivoImagen;
            using (FileStream fs = File.OpenRead(rutaArchivo))
            {
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
                fs.Close();
                archivoImagen = bytes;
            }

            string query = "INSERT INTO [ArchivoEvidencia_MEN] WITH (ROWLOCK)" +
            " ([ARV_Adjunto] ,[ARV_IdEvidenciaDevolucion]  ,[ARV_IdAdjunto]  ,[ARV_NombreAdjunto] ,[ARV_FechaGrabacion] ,[ARV_CreadoPor])  " +
           " VALUES(@Adjunto ,@IdEvidenciaDevolucion ,@IdAdjunto,@NombreAdjunto ,GETDATE() ,@CreadoPor)";

            using (SqlConnection sqlConn = new SqlConnection(conexionStringArchivo))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand(query, sqlConn);
                cmd.Parameters.Add(new SqlParameter("@NombreAdjunto", archivo.NombreServidor));
                cmd.Parameters.Add(new SqlParameter("@IdAdjunto", Guid.NewGuid()));
                cmd.Parameters.Add(new SqlParameter("@IdEvidenciaDevolucion", archivo.IdEvidenciaDevolucion.Value));
                cmd.Parameters.Add(new SqlParameter("@Adjunto", (object)archivoImagen));
                cmd.Parameters.Add(new SqlParameter("@CreadoPor", ControllerContext.Current.Usuario));
                cmd.ExecuteNonQuery();
                sqlConn.Close();
            }
        }

        /// <summary>
        /// Adiciona archivo de un radicado
        /// </summary>
        /// <param name="archivo">objeto de tipo archivo</param>
        public void AdicionarArchivoRapiradicado(ADArchivoRadicadoDC archivo, long numeroRadicado)
        {
            
            filePath = PAAdministrador.Instancia.ObtenerParametrosAplicacion(PAAdministrador.RutaArchivos);
            string rutaArchivo = Path.Combine(this.filePath, COConstantesModulos.MENSAJERIA, archivo.NombreServidor);
            Image imagenArchivo;
            using (FileStream fs = File.OpenRead(rutaArchivo))
            {
                imagenArchivo = Image.FromStream(fs);
                fs.Close();
            }                                                                                                                                                                             
            string rutaImagenes = Framework.Servidor.ParametrosFW.PAParametros.Instancia.ConsultarParametrosFramework("FolderArchivoFolio");
            string carpetaDestino = Path.Combine(rutaImagenes + "\\" + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year);
            if (!Directory.Exists(carpetaDestino))
            {
                Directory.CreateDirectory(carpetaDestino);
            }
            ImageCodecInfo jpgEncoder = UtilidadesFW.GetEncoder(ImageFormat.Jpeg);
            string ruta = carpetaDestino + "\\" + Guid.NewGuid() + ".jpg";
            System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 180L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            var im = new Bitmap(imagenArchivo);
            im.Save(ruta, jpgEncoder, myEncoderParameters);

            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paInsertarArchivoFolios", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idAdmisionRapiRadicado", numeroRadicado);
                cmd.Parameters.AddWithValue("@nombreAdjunto", archivo.NombreServidor);
                cmd.Parameters.AddWithValue("@rutaAdjunto", ruta);
                cmd.Parameters.AddWithValue("@idAdjunto", Guid.NewGuid());                
                cmd.Parameters.AddWithValue("@creadoPor", ControllerContext.Current.Usuario);
                cmd.ExecuteNonQuery();
                sqlConn.Close();
            }

           
        }

        /// <summary>
        /// Metodo para guardar un volante de devolución
        /// </summary>
        /// <param name="evidenciaDevolucion"></param>
        /// <returns></returns>
        public long AdicionarEvidenciaDevolucion(LIEvidenciaDevolucionDC evidenciaDevolucion)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                if (evidenciaDevolucion != null)
                {
                    EvidenciaDevolucion_MEN dato = new EvidenciaDevolucion_MEN()
                    {
                        VOD_CreadoPor = ControllerContext.Current.Usuario,
                        VOD_FechaGrabacion = DateTime.Now,
                        VOD_NumeroEvidencia = evidenciaDevolucion.NumeroEvidencia,
                        VOD_IdTipoEvidenciaDevolucion = evidenciaDevolucion.TipoEvidencia.IdTipo,
                        VOD_DescripcionTipoEvidenciaDevolucion = evidenciaDevolucion.TipoEvidencia.Descripcion,
                        VOD_EstaDigitalizado = evidenciaDevolucion.EstaDigitalizado,
                        VOD_IdEstadoGuiaLog = evidenciaDevolucion.IdEstadoGuialog,
                    };
                    contexto.EvidenciaDevolucion_MEN.Add(dato);
                    contexto.SaveChanges();
                    return dato.VOD_IdEvidenciaDevolucion;
                }
                else
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.MENSAJERIA,
                                                                              ADEnumTipoErrorMensajeria.EX_ERROR_GRABAR_ARCHIVO.ToString(),
                                                                              ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.
                                                                              EX_ERROR_GRABAR_ARCHIVO));
                    throw new FaultException<ControllerException>(excepcion);
                }
            }
        }

        /// <summary>
        /// Edita un archivo de evidencia de devolución
        /// </summary>
        /// <param name="imagen">Objeto imágen</param>
        public void EditarArchivoEvidencia(LIArchivosDC imagen)
        {
            filePath = PAAdministrador.Instancia.ObtenerParametrosAplicacion(PAAdministrador.RutaArchivos);
            byte[] archivoImagen;
            string rutaArchivo = Path.Combine(this.filePath, COConstantesModulos.DIGITALIZACION_Y_ARCHIVO, imagen.NombreServidor);

            using (FileStream fs = new FileStream(imagen.NombreServidor, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
                fs.Close();
                archivoImagen = bytes;
            }

            string query = @"UPDATE [ArchivoEvidencia_MEN] WITH (ROWLOCK)" +
                     " SET ARV_Adjunto = @Adjunto, ARV_IdAdjunto = @IdAdjunto " +
                     " WHERE ARV_IdEvidenciaDevolucion = " + imagen.IdEvidenciaDevolucion;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringArchivo))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand(query, sqlConn);
                cmd.Parameters.Add(new SqlParameter("@Adjunto ", (object)archivoImagen));
                cmd.Parameters.Add(new SqlParameter("@IdAdjunto", Guid.NewGuid()));
                cmd.ExecuteNonQuery();
                sqlConn.Close();
            }
        }

        /// <summary>
        /// Actualiza el valor del porcentaje de
        /// recargo
        /// </summary>
        /// <param name="valor">el valor a actualizar</param>
        public void ActualizarParametroPorcentajeRecargo(double porcentaje)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ParametrosAdmisiones_MEN parametroValor = contexto.ParametrosAdmisiones_MEN.SingleOrDefault(p => p.PAM_IdParametro == "PorcentajeRecargo");

                parametroValor.PAM_ValorParametro = porcentaje.ToString();
                parametroValor.PAM_FechaGrabacion = DateTime.Now;
                parametroValor.PAM_CreadoPor = ControllerContext.Current.Usuario;
                contexto.SaveChanges();
            }
        }

        #endregion Adicionar

        #region Orden de Servicio Cargue Masivo

        /// <summary>
        /// Crea una orden de servicio para asociar las guias de un cargue masivo de mensajeria
        /// </summary>
        /// <param name="idOrdenServicio">Retorna el número de orden de servicio creado</param>
        public long CrearOrdenServicioMasivo(ADOrdenServicioMasivoDC datosOrdenServicio)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                OrdenServicioMasivo_MEN ordenServicio = new OrdenServicioMasivo_MEN()
                {
                    OSM_IdCentroServicios = datosOrdenServicio.IdCentroServicios,
                    OSM_NombreCentroServicios = datosOrdenServicio.NombreCentroServicios,
                    OSM_IdSucursalCliente = datosOrdenServicio.IdSucursalCliente,
                    OSM_NombreSucursalCliente = datosOrdenServicio.NombreSucursalCliente,
                    OSM_Estado = "ACT",
                    OSM_CreadoPor = ControllerContext.Current.Usuario,
                    OSM_FechaGrabacion = DateTime.Now
                };

                contexto.OrdenServicioMasivo_MEN.Add(ordenServicio);
                contexto.SaveChanges();

                return ordenServicio.OSM_IdOrdenServicioMasivo;
            }
        }

        /// <summary>
        /// Validar la existencia de una orden de servicio masiva
        /// </summary>
        /// <param name="ordenServicio">número de la orden de servicio a validar</param>
        /// <returns></returns>
        public bool ValidarOrdenServicio(long ordenServicio)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                OrdenServicioMasivo_MEN orden = contexto.OrdenServicioMasivo_MEN.Where(ord => ord.OSM_IdOrdenServicioMasivo == ordenServicio).FirstOrDefault();
                if (orden != null)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Almacena una guía asociada a una orden de servicio
        /// </summary>
        /// <param name="idAdmision">Id de la admisión</param>
        /// <param name="numeroGuia">número de guía</param>
        /// <param name="numeroOrdenServicio">Numero de orden de servicio</param>
        public void GuardarGuiaOrdenServicio(long idAdmision, long numeroGuia, long numeroOrdenServicio)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                OrdenServicioMasivoGuia_MEN ordenGuia = new OrdenServicioMasivoGuia_MEN()
                {
                    OSG_CreadoPor = ControllerContext.Current.Usuario,
                    OSG_FechaGrabacion = DateTime.Now,
                    OSG_IdAdminisionMensajeria = idAdmision,
                    OSG_IdOrdenServicio = numeroOrdenServicio,
                    OSG_NumeroGuia = numeroGuia
                };
                contexto.OrdenServicioMasivoGuia_MEN.Add(ordenGuia);
                contexto.SaveChanges();
            }
            AdicionarAdmisionOrigen(numeroGuia);
        }

        /// <summary>
        /// Consulta los datos de las guías asociadas a una orden de servicio cargada masivamente
        /// </summary>
        /// <param name="idOrdenServicio">Número de la orden de servicio</param>
        /// <returns></returns>
        public List<ADGuia> ConsultarGuiasDeOrdenDeServicio(long? idOrdenServicio, int? pageSize, int? pageIndex)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                List<ADGuia> GuiasOS = null;
                GuiasOS = contexto.paObtenerAdmisionPorOS_MEN(pageIndex, pageSize, idOrdenServicio).ToList().ConvertAll<ADGuia>(admision =>
                  {
                      return new ADGuia
                      {
                          IdAdmision = admision.ADM_IdAdminisionMensajeria,
                          NumeroGuia = admision.ADM_NumeroGuia,
                          IdCiudadOrigen = admision.ADM_IdCiudadOrigen,
                          NombreCiudadOrigen = admision.ADM_NombreCiudadOrigen,
                          DireccionAgenciaCiudadOrigen = admision.CES_ORIGEN_Direccion,
                          DireccionAgenciaCiudadDestino = admision.CES_DESTINO_Direccion,
                          IdCiudadDestino = admision.ADM_IdCiudadDestino,
                          NombreCiudadDestino = admision.ADM_NombreCiudadDestino,
                          ValorTotal = admision.ADM_ValorTotal,
                          ValorDeclarado = admision.ADM_ValorDeclarado,
                          IdServicio = admision.ADM_IdServicio,
                          EsAlCobro = admision.ADM_EsAlCobro,
                          EstaPagada = admision.ADM_EstaPagada,
                          FechaAdmision = admision.ADM_FechaAdmision,
                          DescripcionTipoEntrega = admision.ADM_DescripcionTipoEntrega,
                          Peso = admision.ADM_Peso,
                          ValorPrimaSeguro = admision.ADM_ValorPrimaSeguro,
                          EsPesoVolumetrico = admision.ADM_EsPesoVolumetrico,
                          ValorServicio = admision.ADM_ValorAdmision,
                          IdCentroServicioOrigen = admision.ADM_IdCentroServicioOrigen,
                          NombreCentroServicioOrigen = admision.ADM_NombreCentroServicioOrigen,
                          IdCentroServicioDestino = admision.ADM_IdCentroServicioDestino,
                          NombreCentroServicioDestino = admision.ADM_NombreCentroServicioDestino,
                          NombreServicio = admision.ADM_NombreServicio,
                          Observaciones = admision.ADM_Observaciones,
                          FormasPago = new List<ADGuiaFormaPago> { },
                          IdTipoEnvio = admision.ADM_IdTipoEnvio,
                          NombreTipoEnvio = admision.ADM_NombreTipoEnvio,
                          TotalPiezas = admision.ADM_TotalPiezas,
                          PrefijoNumeroGuia = admision.ADM_PrefijoNumeroGuia,
                          TelefonoDestinatario = admision.ADM_TelefonoDestinatario,
                          DireccionDestinatario = admision.ADM_DireccionDestinatario,
                          TipoCliente = (ADEnumTipoCliente)Enum.Parse(typeof(ADEnumTipoCliente), admision.ADM_TipoCliente, true),
                          Remitente = new CLClienteContadoDC() { Nombre = admision.ADM_NombreRemitente, Telefono = admision.ADM_TelefonoRemitente, Identificacion = admision.ADM_IdRemitente, Direccion = admision.ADM_DireccionRemitente },
                          Destinatario = new CLClienteContadoDC() { Nombre = admision.ADM_NombreDestinatario, Direccion = admision.ADM_DireccionDestinatario, Telefono = admision.ADM_TelefonoDestinatario, Identificacion = admision.ADM_IdDestinatario },
                          DiceContener = admision.ADM_DiceContener,
                          Supervision = admision.ADM_EsSupervisada,
                          FechaSupervision = admision.ADM_FechaSupervision,
                          NoPedido = admision.ADM_NoPedido,
                          DiasDeEntrega = admision.ADM_DiasDeEntrega,
                          MotivoNoUsoBolsaSeguriDesc = admision.ADM_MotivoNoUsoBolsaSeguriDesc,
                          CodigoPostalOrigen = admision.ADM_CodigoPostalOrigen,
                          CodigoPostalDestino = admision.ADM_CodigoPostalDestino,
                          IdentificacionRepLegal = admision.CES_IdentificacionRepLegal,
                          NombreRepLegal = admision.CES_NombreRepLegal
                      };
                  }
                  );

                return GuiasOS;
            }
        }

        /// <summary>
        /// Consulta la cantidad de guias de admisión asociadas a una orden de servicio
        /// </summary>
        /// <param name="idOrdenServicio"></param>
        /// <returns></returns>
        public long ConsultarCantidadGuiasOrdenSerServicio(long idOrdenServicio)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.OrdenServicioMasivoGuia_MEN.Where(ord => ord.OSG_IdOrdenServicio == idOrdenServicio).Count();
            }
        }

        /// <summary>
        /// Obtiene el listado de
        /// las ordenes de servicio por fecha
        /// </summary>
        /// <param name="fechaInicial"></param>
        /// <param name="fechaFinal"></param>
        /// <returns>lista de ordenes por fecha</returns>
        public List<ADOrdenServicioMasivoDC> ObtenerOrdenesServicioPorFecha(DateTime fechaInicial, DateTime fechaFinal)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                List<OrdenServicioMasivo_MEN> ordenMasivo = contexto.OrdenServicioMasivo_MEN.Where(fec => fec.OSM_FechaGrabacion >= fechaInicial && fec.OSM_FechaGrabacion <= fechaFinal).ToList();

                List<ADOrdenServicioMasivoDC> lstOrdenes = new List<ADOrdenServicioMasivoDC>();

                if (ordenMasivo != null && ordenMasivo.Count > 0)
                {
                    lstOrdenes = ordenMasivo.ConvertAll<ADOrdenServicioMasivoDC>(ord => new ADOrdenServicioMasivoDC()
                    {
                        IdOrdenServicioMasivo = ord.OSM_IdOrdenServicioMasivo,
                        FechaOrdenServicio = ord.OSM_FechaGrabacion,
                        IdCentroServicios = ord.OSM_IdCentroServicios,
                        IdSucursalCliente = ord.OSM_IdSucursalCliente == null ? 0 : (int)ord.OSM_IdSucursalCliente,
                        NombreCentroServicios = ord.OSM_NombreCentroServicios,
                        NombreSucursalCliente = ord.OSM_NombreSucursalCliente

                        //EstadoOrdenServicio = ord.OSM_Estado
                    });
                }

                return lstOrdenes;
            }
        }

        #endregion Orden de Servicio Cargue Masivo

        #region Notificaciones

        /// <summary>
        /// Obtiene la admision de mensajeria para el servicio de notificaciones
        /// </summary>
        /// <param name="numeroGuia"></param>
        public ADNotificacion ObtenerAdmisionMensajeriaNotificaciones(long numeroGuia)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var adm = contexto.paObtenerAdmisionGuiaNotificacion_MEN(numeroGuia).FirstOrDefault();

                if (adm == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.MENSAJERIA,
                                                                             ADEnumTipoErrorMensajeria.EX_ERROR_GUIA_NO_EXISTE_NO_ES_NOTIFICACION.ToString(),
                                                                             ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.
                                                                             EX_ERROR_GUIA_NO_EXISTE_NO_ES_NOTIFICACION));
                    throw new FaultException<ControllerException>(excepcion);
                }

                ADNotificacion notificacion = new ADNotificacion
               {
                   GuiaAdmision = new ADGuia()
                   {
                       IdAdmision = adm.ADM_IdAdminisionMensajeria,
                       NumeroGuia = adm.ADM_NumeroGuia,
                       FechaAdmision = adm.ADM_FechaAdmision,
                       Alto = adm.ADM_Alto,
                       Ancho = adm.ADM_Ancho,
                       IdCentroServicioOrigen = adm.ADM_IdCentroServicioOrigen,
                       NombreCentroServicioOrigen = adm.ADM_NombreCentroServicioOrigen,
                       NombreCiudadDestino = adm.ADM_NombreCiudadDestino,
                       NombreCiudadOrigen = adm.ADM_NombreCiudadOrigen,
                       TipoCliente = (ADEnumTipoCliente)Enum.Parse(typeof(ADEnumTipoCliente), adm.ADM_TipoCliente, true),
                       Entregada = adm.ADM_EstaEntregada
                   },
                   Apellido1Destinatario = adm.ADN_Apellido1Destinatario,
                   Apellido2Destinatario = adm.ADN_Apellido2Destinatario,
                   NombreDestinatario = adm.ADN_NombreDestinatario,
                   DireccionDestinatario = adm.ADN_DireccionDestinatario,
                   TelefonoDestinatario = adm.ADM_TelefonoDestinatario,
                   //IdArchivoGuia = adm.ARG_IdArchivo,
                   EstaDevuelta = adm.ADN_EstaDevuelta == null ? false : adm.ADN_EstaDevuelta.Value,
                   CiudadDestino = new PALocalidadDC()
                   {
                       IdLocalidad = adm.ADN_IdCiudadDestino,
                       Nombre = adm.ADN_NombreCiudadDestino
                   },
                   TipoDestino = new TATipoDestino()
                   {
                       Id = adm.ADN_IdTipoDestino
                   },
                   EstadoRegistro = EnumEstadoRegistro.ADICIONADO
               };
                if (adm.ADM_EstaEntregada)
                    notificacion.EstadoGuia = new ADEstadoGuia { Id = (short)ADEnumEstadoGuia.Entregada };
                else
                    notificacion.EstadoGuia = new ADEstadoGuia { Id = (short)ADEnumEstadoGuia.DevolucionRatificada };
                return notificacion;
            }
        }

        /// <summary>
        /// Retorna información de guía que para ser recibida en forma manual en la parte de notificaciones.
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ADGuia ObtenerNotificacion(long numeroGuia)
        {
            ADGuia guiaNotificacion;
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                paObtenerNotificacion_LOI_Result guia = contexto.paObtenerNotificacion_LOI(numeroGuia).FirstOrDefault();
                if (guia != null && guia.ADM_IdAdminisionMensajeria != 0)
                {
                    guiaNotificacion = new ADGuia
                   {
                       Destinatario = new CLClienteContadoDC
                       {
                           TipoId = guia.ADM_IdTipoIdentificacionDestinatario,
                           Identificacion = guia.ADM_IdDestinatario,
                           Nombre = guia.ADM_NombreDestinatario,
                           Telefono = guia.ADM_TelefonoDestinatario,
                           Direccion = guia.ADM_DireccionDestinatario
                       },
                       FechaAdmision = guia.ADM_FechaAdmision,
                       IdAdmision = guia.ADM_IdAdminisionMensajeria,
                       IdCiudadDestino = guia.ADM_IdCiudadDestino,
                       IdCiudadOrigen = guia.ADM_IdCiudadOrigen,
                       IdServicio = guia.ADM_IdServicio,
                       NombreCiudadDestino = guia.ADM_NombreCiudadDestino,
                       NombreCiudadOrigen = guia.ADM_NombreCiudadOrigen,
                       NumeroGuia = guia.ADM_NumeroGuia,
                       NombreServicio = guia.ADM_NombreServicio,
                       PesoLiqMasa = guia.ADM_PesoLiqMasa,
                       PesoLiqVolumetrico = guia.ADM_PesoLiqVolumetrico,
                       Peso = guia.ADM_Peso,
                       Remitente = new CLClienteContadoDC
                       {
                           TipoId = guia.ADM_IdTipoIdentificacionRemitente,
                           Identificacion = guia.ADM_IdRemitente,
                           Nombre = guia.ADM_NombreRemitente,
                           Telefono = guia.ADM_TelefonoRemitente,
                           Direccion = guia.ADM_DireccionRemitente
                       },
                       NombreTipoEnvio = guia.ADM_NombreTipoEnvio,
                       IdTipoEntrega = guia.ADM_IdTipoEnvio.ToString(),
                       DiceContener = guia.ADM_DiceContener,
                       ValorDeclarado = guia.ADM_ValorDeclarado,
                       Observaciones = guia.ADM_Observaciones,
                       ValorAdmision = guia.ADM_ValorAdmision,
                       ValorPrimaSeguro = guia.ADM_ValorPrimaSeguro,
                       ValorAdicionales = guia.ADM_ValorAdicionales,
                       ValorTotal = guia.ADM_ValorTotal,
                       ValoresAdicionales = this.ObtenerValoresAdicionales(guia.ADM_IdAdminisionMensajeria)
                   };

                    RecibidoGuia_LOI recibido = contexto.RecibidoGuia_LOI.Where(rec => rec.REG_NumeroGuia == guia.ADM_NumeroGuia).FirstOrDefault();
                    if (recibido != null)
                    {
                        guiaNotificacion.Recibidoguia = new LIRecibidoGuia
                        {
                            EstadoRegistro = EnumEstadoRegistro.MODIFICADO,
                            FechaEntrega = recibido.REG_FechaEntrega.Date,
                            HoraEntrega = recibido.REG_FechaEntrega,
                            Identificacion = recibido.REG_Identificacion,
                            IdGuia = recibido.REG_IdAdminisionMensajeria,
                            NumeroGuia = recibido.REG_NumeroGuia,
                            Otros = recibido.REG_OtrosDatos,
                            RecibidoPor = recibido.REG_RecibidoPor,
                            Telefono = recibido.REG_Telefono,
                        };
                    }
                    else
                    {
                        guiaNotificacion.Recibidoguia = new LIRecibidoGuia
                       {
                           IdGuia = guia.ADM_IdAdminisionMensajeria,
                           NumeroGuia = guia.ADM_NumeroGuia,
                           FechaEntrega = DateTime.Now,
                           EstadoRegistro = EnumEstadoRegistro.ADICIONADO,
                       };
                    }
                    return guiaNotificacion;
                }
                else
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MENSAJERIA,
                        ADEnumTipoErrorMensajeria.EX_ERROR_GUIA_NO_EXISTE_NO_ES_NOTIFICACION.ToString(),
                        ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.EX_ERROR_GUIA_NO_EXISTE_NO_ES_NOTIFICACION)));
                }
            }
        }

        /// <summary>
        /// Método para validar una guia notificacion en devolucion
        /// </summary>
        /// <param name="idAdmision"></param>
        public ADNotificacion ValidarNotificacionDevolucion(long idAdmision)
        {
            ADTrazaGuia estadoGuia;
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                AdmisionNotificaciones_MEN not = contexto.AdmisionNotificaciones_MEN.FirstOrDefault(n => n.ADN_IdAdminisionMensajeria == idAdmision);
                if (not == null)
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MENSAJERIA,
                            ADEnumTipoErrorMensajeria.EX_GUIA_NO_NOTIFICACION.ToString(),
                            ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.EX_GUIA_NO_NOTIFICACION)));
                }
                else
                    if (not.ADN_EstaDevuelta == true)
                    {
                        //  PlanillaCertificacionesGuia_LOI pla = contexto.PlanillaCertificacionesGuia_LOI.FirstOrDefault(pl => pl.PLG_IdAdminisionMensajeria == idAdmision);
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MENSAJERIA,
                                                         ADEnumTipoErrorMensajeria.EX_GUIA_YA_PLANILLADA.ToString(),
                                                           ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.EX_GUIA_YA_PLANILLADA)));
                    }
                    else
                        estadoGuia = EstadosGuia.ObtenerEstadosGuiaxIdAdmision(idAdmision).FirstOrDefault(est => est.IdEstadoGuia == (short)ADEnumEstadoGuia.DevolucionRatificada);
                if (estadoGuia == null)
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MENSAJERIA,
                     ADEnumTipoErrorMensajeria.EX_GUIA_NO_DEVUELTA.ToString(),
                     ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.EX_GUIA_NO_DEVUELTA)));
                }
                else
                {
                    AlmacenGuia_LOI almacen = contexto.AlmacenGuia_LOI.FirstOrDefault(alm => alm.ARG_IdAdminisionMensajeria == idAdmision);
                    if (almacen == null)
                    {
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MENSAJERIA,
                        ADEnumTipoErrorMensajeria.EX_GUIA_NO_DIGITALIZADA.ToString(),
                        ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.EX_GUIA_NO_DIGITALIZADA)));
                    }
                    else
                    {
                        AdmisionNotificaciones_MEN notificaciones = contexto.AdmisionNotificaciones_MEN.Where(r => r.ADN_IdAdminisionMensajeria == idAdmision).FirstOrDefault();
                        return new ADNotificacion
                        {
                            Apellido1Destinatario = notificaciones.ADN_Apellido1Destinatario,
                            Apellido2Destinatario = notificaciones.ADN_Apellido2Destinatario,
                            DireccionDestinatario = notificaciones.ADN_DireccionDestinatario,
                            EmailDestinatario = notificaciones.ADN_EmailDestinatario,
                            GuiaAdmision = new ADGuia { IdAdmision = notificaciones.ADN_IdAdminisionMensajeria },
                            CiudadDestino = new PALocalidadDC { IdLocalidad = notificaciones.ADN_IdCiudadDestino, Nombre = notificaciones.ADN_NombreCiudadDestino },
                            IdDestinatario = notificaciones.ADN_IdDestinatario,
                            TipoDestino = new TATipoDestino { Id = notificaciones.ADN_IdTipoDestino },
                            NombreDestinatario = notificaciones.ADN_NombreDestinatario,
                            GuiaInterna = new ADGuiaInternaDC { NumeroGuia = notificaciones.ADN_NumeroGuiaInterna, },
                            ReclamaEnOficina = notificaciones.ADN_ReclamaEnOficina,
                            TipoIdentificacionDestinatario = notificaciones.ADN_TipoIdDestinatario,
                            TelefonoDestinatario = notificaciones.ADN_TelefonoDestinatario
                        };
                    }
                }
            }
        }

        /// <summary>
        /// Método para actualizar la tabla notificacion campo ADN_EstaDevuelta , campo ADN_NumeroGuiaInterna
        /// </summary>
        /// <param name="guia"></param>
        public void ActualizarPLanilladaNotificacion(ADNotificacion guia)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                AdmisionNotificaciones_MEN admNotificacion = contexto.AdmisionNotificaciones_MEN.FirstOrDefault(not => not.ADN_IdAdminisionMensajeria == guia.GuiaAdmision.IdAdmision);
                if (admNotificacion != null)
                {
                    admNotificacion.ADN_EstaDevuelta = true;
                    admNotificacion.ADN_NumeroGuiaInterna = guia.GuiaInterna.NumeroGuia;
                    contexto.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Retorna los datos de las notificaciones no planilladas de tipo CES
        /// </summary>
        /// <param name="idAdmision"></param>
        public List<ADNotificacion> ObtenerNotificacionesEntregaCES(long idCentroServicio, long idCentroServicioOrigen, DateTime fechaInicial, DateTime fechaFinal)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.PaObtenerNotificacionesEntregaCes_MEN(idCentroServicio, idCentroServicioOrigen, fechaInicial, fechaFinal)
                    .ToList()
                    .ConvertAll(n => new ADNotificacion
                    {
                        Apellido1Destinatario = n.ADN_Apellido1Destinatario,
                        Apellido2Destinatario = n.ADN_Apellido2Destinatario,
                        DireccionDestinatario = n.ADN_DireccionDestinatario,
                        EmailDestinatario = n.ADN_EmailDestinatario,
                        GuiaAdmision = new ADGuia
                        {
                            IdAdmision = n.ADM_IdAdminisionMensajeria,
                            NumeroGuia = n.ADM_NumeroGuia,
                            FechaEntrega = n.ADM_FechaEntrega
                        },
                        CiudadDestino = new PALocalidadDC { IdLocalidad = n.ADN_IdCiudadDestino, Nombre = n.ADN_NombreCiudadDestino },
                        IdDestinatario = n.ADN_IdDestinatario,
                        TipoDestino = new TATipoDestino { Id = n.ADN_IdTipoDestino, Descripcion = n.ADN_TipoDestino },
                        NombreDestinatario = n.ADN_NombreDestinatario,
                        ReclamaEnOficina = n.ADN_ReclamaEnOficina,
                        TipoIdentificacionDestinatario = n.ADN_TipoIdDestinatario
                    });
            }
        }

        /// <summary>
        /// Retorna los datos de las notificaciones no planilladas de tipo CRE
        /// </summary>
        /// <param name="idAdmision"></param>
        public List<ADNotificacion> ObtenerNotificacionesEntregaCRE(DateTime fechaInicial, DateTime fechaFinal, long idCol, long idSucursal)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.PaObtenerNotificacionesEntregaCre_MEN(fechaInicial, fechaFinal, idCol, idSucursal)
                    .ToList()
                    .ConvertAll(n => new ADNotificacion
                    {
                        Apellido1Destinatario = n.ADN_Apellido1Destinatario,
                        Apellido2Destinatario = n.ADN_Apellido2Destinatario,
                        DireccionDestinatario = n.ADN_DireccionDestinatario,
                        EmailDestinatario = n.ADN_EmailDestinatario,
                        GuiaAdmision = new ADGuia
                        {
                            IdAdmision = n.ADM_IdAdminisionMensajeria,
                            NumeroGuia = n.ADM_NumeroGuia,
                            FechaEntrega = n.ADM_FechaEntrega
                        },
                        CiudadDestino = new PALocalidadDC { IdLocalidad = n.ADN_IdCiudadDestino, Nombre = n.ADN_NombreCiudadDestino },
                        IdDestinatario = n.ADN_IdDestinatario,
                        TipoDestino = new TATipoDestino { Id = n.ADN_IdTipoDestino, Descripcion = n.ADN_TipoDestino },
                        NombreDestinatario = n.ADN_NombreDestinatario,
                        ReclamaEnOficina = n.ADN_ReclamaEnOficina,
                        TipoIdentificacionDestinatario = n.ADN_TipoIdDestinatario
                    });
            }
        }

        /// <summary>
        /// Método para obtener las guías internas de una planilla de notificación
        /// </summary>
        /// <param name="idplanilla"></param>
        /// <returns></returns>
        public List<ADGuiaInternaDC> ObtenerGuiasInternasNotificaciones(long idplanilla)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.PaObtenerGuiasInternasNot_MEN(idplanilla)
                          .ToList()
                          .ConvertAll(n => new ADGuiaInternaDC
                          {
                              NumeroGuia = n.PLG_NumeroGuiaInterna,
                              DireccionDestinatario = n.AGI_DireccionDestinatario,
                              DireccionRemitente = n.AGI_DireccionRemitente,
                              FechaGrabacion = n.AGI_FechaGrabacion,
                              IdAdmisionGuia = n.AGI_IdAdmisionMensajeria,
                              LocalidadDestino = new PALocalidadDC
                              {
                                  IdLocalidad = n.ADM_IdCiudadDestino,
                                  Nombre = n.ADM_NombreCiudadDestino
                              },
                              LocalidadOrigen = new PALocalidadDC
                              {
                                  IdLocalidad = n.ADM_IdCiudadOrigen,
                                  Nombre = n.ADM_NombreCiudadOrigen
                              },
                              GestionOrigen = new ARGestionDC
                              {
                                  IdGestion = n.AGI_IdGestionOrigen,
                                  Descripcion = n.AGI_DescripcionGestionOrig
                              },
                              GestionDestino = new ARGestionDC
                              {
                                  IdGestion = n.AGI_IdGestionDestino,
                                  Descripcion = n.AGI_DescripcionGestionDest
                              },
                              DiceContener = string.IsNullOrEmpty(n.AGI_Contenido) ? string.Empty : (n.AGI_Contenido),
                              NombreRemitente = n.AGI_NombreRemitente,
                              NombreDestinatario = n.AGI_NombreDestinatario,
                              TelefonoRemitente = n.AGI_TelefonoRemitente,
                              TelefonoDestinatario = n.AGI_TelefonoDestinatario
                          });
            }
        }

        /// <summary>
        /// Inserta un registro de una notificacion
        /// </summary>
        /// <param name="numeroGuia"></param>
        public void AdicionarNotificacion(long numeroGuia)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paCrearAdmisionNotificaciones_MEN(numeroGuia);
            }
        }

        #endregion Notificaciones

        public void ValidarSiDestinatarioAutorizaEnvioMensajeTexto(int idCliente, string tipoIdDestinatario, string identificacionDestinatario, long idAdmision, long numeroGuia, string NumPedido)
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paDestDeseaRecibirNotifiMsjTexto_MEN(identificacionDestinatario, tipoIdDestinatario, idCliente, idAdmision, numeroGuia, NumPedido, ControllerContext.Current.Usuario);
            }
        }

        /// <summary>
        /// Retorna todo el listado de casilleros establecidos por trayecto
        /// </summary>
        /// <returns></returns>
        public HashSet<ADRangoTrayecto> ObtenerCasillerosTrayectos()
        {
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var casilleros = contexto.paObtenerCasillerosTrayectos_OPN()
                  .GroupBy(cas => new { cas.TRC_IdLocalidadDestino, cas.TRC_IdLocalidadOrigen })
                  .ToList()
                  .ConvertAll<ADRangoTrayecto>(r =>
                    new ADRangoTrayecto
                    {
                        IdLocalidadDestino = r.First().TRC_IdLocalidadDestino,
                        IdLocalidadOrigen = r.First().TRC_IdLocalidadOrigen,
                        Rangos = r.ToList().ConvertAll(rango =>
                          new ADRangoCasillero
                          {
                              Casillero = rango.TCP_IdCasillero,
                              RangoFinal = rango.RPC_RangoFinal,
                              RangoInicial = rango.RPC_RangoInicial
                          })
                    });
                return new HashSet<ADRangoTrayecto>(casilleros);
            }
        }


        /// <summary>
        /// Obtiene el numero de la factura de venta
        /// </summary>        
        /// <returns>numero de la factura de venta</returns>
        public SUNumeradorPrefijo ObtenerNumeroFacturaAutomatica()
        {
            lock (this)
            {

                using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
                {
                    
                    using (TransactionScope trans = new TransactionScope())
                    {
                        sqlConn.Open();
                        SqlCommand cmd = new SqlCommand(@"paObtenerNumeroFacturaAutomatica_MEN", sqlConn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        DataTable dt = new DataTable();
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);
                        sqlConn.Close();
                        var reg = dt.AsEnumerable().FirstOrDefault();
                        var numGuia = reg["NumeroGuia"];
                        var prefijo = reg["Prefijo"];

                        if (numGuia == null || !(numGuia is long) || (long)numGuia <= 0)
                        {
                            throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MENSAJERIA, ADEnumTipoErrorMensajeria.EX_NO_SE_PUDO_GENERAR_NUMERO_GUIA.ToString(), ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.EX_NO_SE_PUDO_GENERAR_NUMERO_GUIA)));
                        }

                        SUNumeradorPrefijo numero = new SUNumeradorPrefijo()
                        {
                            Prefijo = prefijo.ToString(),
                            ValorActual = (long)numGuia
                        };

                        trans.Complete();
                        return numero;
                    }
                }
            }
        }

        public void GuardarNumeroFacturaFallido(long numeroGuia, string prefijo)
        {
            try
            {
                using (SqlConnection sqlConn = new SqlConnection(conexionStringAuditoria))
                {
                    sqlConn.Open();
                    SqlCommand cmd = new SqlCommand(@"paGrabarNumeroFacturaAutomaticaFallido_MEN", sqlConn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@NumeroGuia", numeroGuia);
                    cmd.Parameters.AddWithValue("@Prefijo", prefijo);
                    cmd.Parameters.AddWithValue("@Usuario", ControllerContext.Current.Usuario);
                    cmd.ExecuteNonQuery();
                    sqlConn.Close();
                }

            }
            catch
            {
                //si falla no debe evitar la continuidad del metodo
            }
        }
        /// <summary>
        /// Audita todas las admisiones automaticas generadas
        /// </summary>
        public void GuardarAuditoriaGrabacionAdmisionMensajeria(int idCaja, string metodoEjecutado, string retorno, string guiaEntrada, string tipoCliente, string usuario, string objetoAdicional = null)
        {
            using (SqlConnection sqlConn = new SqlConnection(conexionStringAuditoria))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand(@"paGrabarAuditoriaAdmisionesAutomaticas_MEN", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@RetornoAdmision", retorno);
                cmd.Parameters.AddWithValue("@IdCaja", idCaja);
                cmd.Parameters.AddWithValue("@GuiaEntrada", guiaEntrada);
                cmd.Parameters.AddWithValue("@RemitenteDestinatario", tipoCliente);
                cmd.Parameters.Add(Utilidades.AddParametro("@ObjetoAdicional", objetoAdicional));
                cmd.Parameters.AddWithValue("@MetodoEjecutado", metodoEjecutado);
                cmd.Parameters.AddWithValue("@CreadoPor", usuario);
                cmd.ExecuteNonQuery();
                sqlConn.Close();
            }
        }


        public void AuditarGuiaGeneradaArchivoTexto(ADGuia guia, int idCaja, ADMensajeriaTipoCliente remitenteDestinatario, object objetoAdicional = null)
        {
            try
            {
            System.Threading.Tasks.Task t = new System.Threading.Tasks.Task(() =>
          {


                  StringBuilder sb = new StringBuilder();

                  string objetoAdicionalSerializado = null;


                  string guiaSerializado = Framework.Servidor.Comun.Util.Serializacion.Serialize(guia);

                  string remitenteDestinatarioSerializado = Framework.Servidor.Comun.Util.Serializacion.Serialize(remitenteDestinatario);

                  string tipoAdmision = "AdmisionMensajeria";

                  if (objetoAdicional != null)
                  {

                      switch (objetoAdicional.GetType().ToString())
                      {
                          case "CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria.ADNotificacion":


                              objetoAdicionalSerializado = Framework.Servidor.Comun.Util.Serializacion.Serialize(objetoAdicional as ADNotificacion);
                              tipoAdmision = "AdmisionNotificaciones";
                              break;

                          case "CO.Servidor.Servicios.ContratoDatos.Tarifas.TATipoEmpaque":
                              objetoAdicionalSerializado = Framework.Servidor.Comun.Util.Serializacion.Serialize(objetoAdicional as TATipoEmpaque);
                              tipoAdmision = "AdmisionInternacional";
                              break;

                          case "CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria.ADRapiEnvioContraPagoDC":
                              objetoAdicionalSerializado = Framework.Servidor.Comun.Util.Serializacion.Serialize(objetoAdicional as ADRapiEnvioContraPagoDC);
                              tipoAdmision = "AdmisionRapienvioContraPago";
                              break;

                          case "CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria.ADRapiRadicado":
                              objetoAdicionalSerializado = Framework.Servidor.Comun.Util.Serializacion.Serialize(objetoAdicional as ADRapiRadicado);
                              tipoAdmision = "AdmisionRapiRadicado";
                              break;
                      }
                  }
                  sb.AppendLine("/******************************************************************/");

                  sb.AppendLine(tipoAdmision + "   FechaAuditoria: " + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + "   Caja: " + idCaja.ToString() + "   NumeroGuia: " + guia.NumeroGuia);
                  sb.AppendLine("-----");
                  sb.AppendLine(guiaSerializado);
                  sb.AppendLine("-----");
                  sb.AppendLine(remitenteDestinatarioSerializado);
                  sb.AppendLine("-----");
                  sb.AppendLine(objetoAdicionalSerializado);
                  sb.AppendLine("/******************************************************************/");

                  string pathAuditoriaAdmisionesController = @"C:\ControllerAuditoriaAdmisiones\";
                  string file = pathAuditoriaAdmisionesController + "auditoria-" + DateTime.Now.ToString("dd-MM-yyyy") + ".txt";
                  File.AppendAllText(file, sb.ToString());


              });
                t.Start();

              }
              catch
              {
              }

        }


        /// <summary>
        /// Método para auditar el numero de guia generado, solo para las pruebas de carga, se debe borrar.
        /// </summary>
        /// <param name="guia"></param>
        public void AuditarNumeroGuiaGenerado(long numeroGuia)
        {
            try
            {
                using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
                {
                    sqlConn.Open();
                    SqlCommand cmd = new SqlCommand(@"INSERT INTO [NumerosGuiasPruebaCarga]
                                                       ([NumeroGuia]
                                                       ,[Confirmado]
                                                       ,Usuario
                                                       ,FechaGrabacion)
                                                 VALUES
                                                       (@NumeroGuia
                                                       ,0
                                                       ,@Usuario
                                                       ,getdate())", sqlConn);
                    cmd.Parameters.AddWithValue("@NumeroGuia", numeroGuia);
                    cmd.Parameters.AddWithValue("@Usuario", ControllerContext.Current.Usuario);
                    cmd.ExecuteNonQuery();
                    sqlConn.Close();
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// Método para auditar el numero de guia generado, solo para las pruebas de carga, se debe borrar.
        /// </summary>
        /// <param name="guia"></param>
        public void ConfirmarAuditoriaNumeroGuiaGenerado(long numeroGuia)
        {
            /*try
            {
                using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
                {
                    sqlConn.Open();
                    SqlCommand cmd = new SqlCommand(@"UPDATE [NumerosGuiasPruebaCarga] with(rowlock)
                                                       SET  [Confirmado] = 1,
                                                            FechaVerificacion = getdate()
                                                     WHERE [NumeroGuia] = @NumeroGuia", sqlConn);
                    cmd.Parameters.AddWithValue("@NumeroGuia", numeroGuia);
                    cmd.ExecuteNonQuery();
                    sqlConn.Close();
                }
            }
            catch
            {

            }*/
        }

        /// <summary>
        /// Retorna el último estado de una guía
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="_idEstado"></param>
        /// <returns></returns>
        public bool ConsultarUltimoEstadoGuia(long numeroGuia, out int _idEstado, out long idAdmisionMensajeria)
        {
            int estado = 0;
            long _idAdmisionMensajeria = 0;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerUltimoEstado_MEN", sqlConn);
                cmd.Parameters.AddWithValue("@NumeroGuia", numeroGuia);
                cmd.CommandType = CommandType.StoredProcedure;
                var dreader = cmd.ExecuteReader();
                if (dreader.Read())
                {
                    string idEstado = dreader["ESG_IdEstadoGuia"].ToString();
                    int.TryParse(idEstado, out estado);
                    string sIdAdmisionMensajeria = dreader["ADM_IdAdminisionMensajeria"].ToString();
                    long.TryParse(sIdAdmisionMensajeria, out _idAdmisionMensajeria);
                }
                sqlConn.Close();
                sqlConn.Dispose();
            }
            _idEstado = estado;
            idAdmisionMensajeria = _idAdmisionMensajeria;
            return (estado > 0);
        }

        /// <summary>
        /// Método para verificar una direccion
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="verificadoPor"></param>
        /// <param name="destinatario"></param>
        public void ConfirmarDireccion(long numeroGuia, string verificadoPor, bool destinatario, bool remitente)
        {
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand(@"paVerificarDireccionesMensajeria_MEN", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@verificadoPor", verificadoPor);
                cmd.Parameters.AddWithValue("@numeroGuia", numeroGuia);
                cmd.Parameters.AddWithValue("@destinatario", destinatario.ToString());
                cmd.Parameters.AddWithValue("@remitente", remitente.ToString());
                cmd.ExecuteNonQuery();
                sqlConn.Close();
            }
        }


        /// <summary>
        /// Obtiene la ubicacion de una guia para la app del cliente
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ADUbicacionGuia ObtenerUbicacionGuia(long numeroGuia)
        {

            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {

                SqlCommand cmd = new SqlCommand(@"paConsultarUbicacionEnvio_MEN", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ADM_NumeroGuia", numeroGuia);
                sqlConn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                sqlConn.Close();

                var guia = dt.AsEnumerable().ToList().FirstOrDefault();

                ADUbicacionGuia ubicacionGuia = new ADUbicacionGuia();

                if (guia != null)
                {
                    ubicacionGuia.DescripcionEstadoGuiaTraza = guia.Field<string>("EGT_DescripcionEstado");
                    ubicacionGuia.DescripcionMotivoGuia = guia.Field<string>("MOG_Descripcion");
                    ubicacionGuia.FechaAdmision = guia.Field<DateTime>("ADM_FechaAdmision");
                    ubicacionGuia.FechaEntrega = guia.Field<DateTime>("ADM_FechaEntrega");
                    ubicacionGuia.IdServicio = guia.Field<int>("ADM_IdServicio");
                    ubicacionGuia.NombreCiudadDestino = guia.Field<string>("ADM_NombreCiudadDestino");
                    ubicacionGuia.NombreCiudadOrigen = guia.Field<string>("ADM_NombreCiudadOrigen");
                    ubicacionGuia.NombreLocalidadGuiatraza = guia.Field<string>("EGT_NombreLocalidad");
                    ubicacionGuia.NumeroGuia = guia.Field<long>("ADM_NumeroGuia");
                    ubicacionGuia.NumeroGuiaDHL = guia["ADI_NumeroGuiaDHL"] != DBNull.Value ? guia.Field<long>("ADI_NumeroGuiaDHL") : 0;
                    ubicacionGuia.NombreRemitente = guia.Field<string>("ADM_NombreRemitente");
                    ubicacionGuia.NombreDestinatario = guia.Field<string>("ADM_NombreDestinatario");
                    ubicacionGuia.DireccionRemitente = guia.Field<string>("ADM_DireccionRemitente");
                    ubicacionGuia.DireccionDestinatario = guia.Field<string>("ADM_DireccionDestinatario");
                    ubicacionGuia.TelefonoRemitente = guia.Field<string>("ADM_TelefonoRemitente");
                    ubicacionGuia.TelefonoDestinatario = guia.Field<string>("ADM_TelefonoDestinatario");
                    ubicacionGuia.NombreServicio = guia.Field<string>("ADM_NombreServicio");
                    ubicacionGuia.NombreTipoEnvio = guia.Field<string>("ADM_NombreTipoEnvio");
                    ubicacionGuia.Peso = guia.Field<decimal>("ADM_Peso");
                    ubicacionGuia.ValorAdmision = guia["ADM_ValorAdmision"] != DBNull.Value ? guia.Field<decimal>("ADM_ValorAdmision") : 0;
                    ubicacionGuia.ValorPrimaSeguro = guia["ADM_ValorPrimaSeguro"] != DBNull.Value ? guia.Field<decimal>("ADM_ValorPrimaSeguro") : 0;
                    ubicacionGuia.ValorAdicionales = guia["ADM_ValorAdicionales"] != DBNull.Value ? guia.Field<decimal>("ADM_ValorAdicionales") : 0;
                    ubicacionGuia.ValorTotal = guia["ADM_ValorTotal"] != DBNull.Value ? guia.Field<decimal>("ADM_ValorTotal") : 0;
                    ubicacionGuia.DiceContener = guia.Field<string>("ADM_DiceContener");
                    ubicacionGuia.DescripcionFormaPago = guia.Field<string>("FOP_Descripcion");
                }

                return ubicacionGuia;

            }


        }


        public void ActualizarGuiaImpresa(long NumeroGuia)
        {
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand(@"paActualizarGuiaImpresa_MEN", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroGuia", NumeroGuia);
                sqlConn.Open();
                cmd.ExecuteNonQuery();
                sqlConn.Close();
            }
        }

        public void AdicionarMovimientoInventario(PUMovimientoInventario MovimientoInventario, SqlConnection conexion, SqlTransaction transaccion)
        {

            SqlCommand cmd = new SqlCommand("paAdicionarMovimientoInventario_PUA", conexion, transaccion);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@INV_IdCentroServicioOrigen", MovimientoInventario.IdCentroServicioOrigen);
            cmd.Parameters.AddWithValue("@INV_IdCentroServicio", MovimientoInventario.Bodega.IdCentroServicio);
            cmd.Parameters.AddWithValue("@INV_IdTipoMovimiento", (short)MovimientoInventario.TipoMovimiento);
            cmd.Parameters.AddWithValue("@INV_NumeroGuia", MovimientoInventario.NumeroGuia);
            cmd.Parameters.AddWithValue("@INV_CreadoPor", MovimientoInventario.CreadoPor);
            cmd.Parameters.AddWithValue("@INV_FechaEstimadaIngreso", MovimientoInventario.FechaEstimadaIngreso);
            cmd.Parameters.AddWithValue("@INV_FechaGrabacion", MovimientoInventario.FechaGrabacion);

            cmd.ExecuteNonQuery();

            // Int64 newId = (Int64)cmd.ExecuteScalar();
            // return (long)newId;
        }

        /// <summary>
        /// Registra la auditoria de las guias de POS que al sincronizarlas ya se encuentran admitidas
        /// </summary>
        /// <param name="guiaSerializada"></param>
        /// <param name="guiaSerializadaServidor"></param>
        /// <param name="objetoAdicionalSerializado"></param>
        /// <param name="numeroGuia"></param>
        /// <param name="idCentroServiciosOrigen"></param>
        public void RegistrarAuditoriaAdmisionesManualesDuplicadas(string guiaSerializada, string guiaSerializadaServidor, string objetoAdicionalSerializado, long numeroGuia, long idCentroServiciosOrigen)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringAuditoria))
            {
                SqlCommand cmd = new SqlCommand("paRegistrarAuditoriaAdmisionesDuplicadas", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroGuiaAuditada", numeroGuia);
                cmd.Parameters.AddWithValue("@IdCentroServicioOrigen", idCentroServiciosOrigen);
                cmd.Parameters.AddWithValue("@AdmisionCliente", guiaSerializada);
                cmd.Parameters.AddWithValue("@AdmisionServidor", guiaSerializadaServidor);

                if(!string.IsNullOrEmpty(objetoAdicionalSerializado))
                cmd.Parameters.AddWithValue("@ObjetoAdicional", objetoAdicionalSerializado);

                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        public string ObtenerParametrosEncabezado(string llave)
        {
            string encabezado;
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerEncabezadoGuia_MEN", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Llave", llave);
                conn.Open();
                var resultado = cmd.ExecuteScalar().ToString();
                conn.Close();
                encabezado = resultado == null ? "" : resultado.ToString();
            }
            return encabezado;
        }

        #region reimpresionesWPF

        /// <summary>
        /// Método para obtener el Tipo de Impresion de una Localidad
        /// </summary>
        /// <param name="IdLocalidad"></param>
        /// <returns></returns>
        public long ObtenerTipoFormatoImpresionLocalidad(long IdLocalidad)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerTipoFormatoImpresionLocalidad_MEN", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CodLocalidad", IdLocalidad);
                conn.Open();
                long resultado = Convert.ToInt64(cmd.ExecuteScalar());
                conn.Close();

                return resultado;
            }
        }


        /// <summary>
        /// Retorna las guías que hayan sido enviadas por el destinatario indicado
        /// </summary>
        /// <param name="tipoDidentificacionDestinatario"></param>
        /// <param name="idDestinatario"></param>
        /// <returns></returns>
        public List<ADGuia> ObtenerGuiasPorDestinatario(string tipoDidentificacionDestinatario, string idDestinatario)
        {
            List<ADGuia> lstGuias = new List<ADGuia>();
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerAdmMensajeriaPorDestinatario_MEN", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ADM_IdTipoIdentificacionDestinatario", tipoDidentificacionDestinatario);
                cmd.Parameters.AddWithValue("@ADM_IdDestinatario", idDestinatario);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                lstGuias = new List<ADGuia>();
                while (reader.Read())
                {
                    ADGuia guiaBuscada = new ADGuia();

                    guiaBuscada.NumeroGuia = Convert.ToInt64(reader["ADM_NumeroGuia"]);
                    guiaBuscada.TipoCliente = ADEnumTipoCliente.PPE;
                    guiaBuscada.Remitente = new CLClienteContadoDC
                    {
                        Nombre = reader["ADM_NombreRemitente"].ToString(),
                        Direccion = reader["ADM_DireccionRemitente"].ToString(),
                        Identificacion = reader["ADM_IdRemitente"].ToString(),
                        Telefono = reader["ADM_TelefonoRemitente"].ToString(),
                        TipoId = reader["ADM_IdTipoIdentificacionRemitente"].ToString()
                    };

                    guiaBuscada.Destinatario = new CLClienteContadoDC
                    {
                        Nombre = reader["ADM_NombreDestinatario"].ToString(),
                        Direccion = reader["ADM_DireccionDestinatario"].ToString(),
                        Identificacion = reader["ADM_IdDestinatario"].ToString(),
                        Telefono = reader["ADM_TelefonoDestinatario"].ToString(),
                        TipoId = reader["ADM_IdTipoIdentificacionDestinatario"].ToString()
                    };
                    guiaBuscada.FechaAdmision = Convert.ToDateTime(reader["ADM_FechaAdmision"]);
                    guiaBuscada.NombreCentroServicioOrigen = reader["ADM_NombreCentroServicioOrigen"].ToString();
                    guiaBuscada.NombreCentroServicioDestino = reader["ADM_NombreCentroServicioDestino"].ToString();

                    lstGuias.Add(guiaBuscada);
                }
            }
            return lstGuias;
        }

        #endregion



        /// <summary>
        /// Consulta la informacion remitente detinatario por numero guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ADMensajeriaTipoCliente ObtenerRemitenteDestinatarioGuia(long numeroGuia)
        {
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {

                SqlCommand cmd = new SqlCommand(@"paObtenerRemitenteDestinatarioGuia_MEN", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@numeroGuia", numeroGuia);
                sqlConn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                sqlConn.Close();

                var remDes = dt.AsEnumerable().ToList().FirstOrDefault();

                ADMensajeriaTipoCliente remitenteDestinatario = new ADMensajeriaTipoCliente() { PeatonDestinatario = new ADPeaton(), PeatonRemitente = new ADPeaton() };

                if (remDes != null)
                {

                    remitenteDestinatario.PeatonRemitente.TipoIdentificacion = remDes.Field<string>("ADM_IdTipoIdentificacionRemitente");
                    remitenteDestinatario.PeatonRemitente.Identificacion = remDes.Field<string>("ADM_IdRemitente");
                    remitenteDestinatario.PeatonRemitente.Nombre = remDes.Field<string>("ADM_NombreRemitente");
                    remitenteDestinatario.PeatonRemitente.Telefono = remDes.Field<string>("ADM_TelefonoRemitente");
                    remitenteDestinatario.PeatonRemitente.Direccion = remDes.Field<string>("ADM_DireccionRemitente");
                    remitenteDestinatario.PeatonRemitente.Email = remDes.Field<string>("ADM_EmailRemitente");
                    remitenteDestinatario.PeatonDestinatario.TipoIdentificacion = remDes.Field<string>("ADM_IdTipoIdentificacionDestinatario");
                    remitenteDestinatario.PeatonDestinatario.Identificacion = remDes.Field<string>("ADM_IdDestinatario");
                    remitenteDestinatario.PeatonDestinatario.Nombre = remDes.Field<string>("ADM_NombreDestinatario");
                    remitenteDestinatario.PeatonDestinatario.Telefono = remDes.Field<string>("ADM_TelefonoDestinatario");
                    remitenteDestinatario.PeatonDestinatario.Direccion = remDes.Field<string>("ADM_DireccionDestinatario");
                    remitenteDestinatario.PeatonDestinatario.Email = remDes.Field<string>("ADM_EmailDestinatario");

                }

                return remitenteDestinatario;

            }

        }
        #region ConsultaGuiaIntegracionxidClienteyNumGuia
        /// <summary>
        /// Consulta la informacion de la guia por el idCliente y por el numero de guia 
        /// <param name="idCliente"/>
        /// <param name="numGuia"/>
        /// <returns>ADGuia</returns>
        /// </summary>
        public ADGuia ConsultarGuia(int idCliente, long numeroGuia)
        {
            ADGuia Respuesta = new ADGuia();
            using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {

                using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
                {
                    SqlCommand cmd = new SqlCommand("paObtenerAdmisionMensajeriaxIdCliente_MEN", sqlConn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@idCliente", idCliente);
                    cmd.Parameters.AddWithValue("@numGuia", numeroGuia);
                    sqlConn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        Respuesta.Entregada = Convert.ToBoolean(reader["ADM_EstaEntregada"]);
                        Respuesta.IdAdmision = Convert.ToInt64(reader["ADM_IdAdminisionMensajeria"]);
                        Respuesta.NumeroGuia = Convert.ToInt64(reader["ADM_NumeroGuia"]);
                        Respuesta.IdCiudadOrigen = Convert.ToString(reader["ADM_IdCiudadOrigen"]);
                        Respuesta.NombreCiudadOrigen = Convert.ToString(reader["ADM_NombreCiudadOrigen"]);
                        Respuesta.IdCiudadDestino = Convert.ToString(reader["ADM_IdCiudadDestino"]);
                        Respuesta.NombreCiudadDestino = Convert.ToString(reader["ADM_NombreCiudadDestino"]);
                        Respuesta.ValorTotal = Convert.ToDecimal(reader["ADM_ValorTotal"]);
                        Respuesta.ValorDeclarado = Convert.ToDecimal(reader["ADM_ValorDeclarado"]);
                        Respuesta.IdServicio = Convert.ToInt32(reader["ADM_IdServicio"]);
                        Respuesta.EsAlCobro = Convert.ToBoolean(reader["ADM_EsAlCobro"]);
                        Respuesta.EstaPagada = Convert.ToBoolean(reader["ADM_EstaPagada"]);
                        Respuesta.FechaAdmision = Convert.ToDateTime(reader["ADM_FechaAdmision"]);
                        Respuesta.Peso = Convert.ToDecimal(reader["ADM_Peso"]);
                        Respuesta.EsPesoVolumetrico = Convert.ToBoolean(reader["ADM_EsPesoVolumetrico"]);
                        Respuesta.ValorServicio = Convert.ToDecimal(reader["ADM_ValorAdmision"]);
                        Respuesta.IdCentroServicioOrigen = Convert.ToInt64(reader["ADM_IdCentroServicioOrigen"]);
                        Respuesta.NombreCentroServicioOrigen = Convert.ToString(reader["ADM_NombreCentroServicioOrigen"]);
                        Respuesta.IdCentroServicioDestino = Convert.ToInt64(reader["ADM_IdCentroServicioDestino"]);
                        Respuesta.NombreCentroServicioDestino = Convert.ToString(reader["ADM_NombreCentroServicioDestino"]);
                        Respuesta.NombreServicio = Convert.ToString(reader["ADM_NombreServicio"]);
                        Respuesta.Observaciones = Convert.ToString(reader["ADM_Observaciones"]);
                        Respuesta.FormasPago = contexto.FormasPagoGuia_VMEN.Where(fp => fp.AGF_IdAdminisionMensajeria == Convert.ToInt64(reader["toADM_IdAdminisionMensajeria"])).ToList().ConvertAll(f => new ADGuiaFormaPago { Descripcion = f.FOP_Descripcion, IdFormaPago = f.AGF_IdFormaPago, Valor = f.AGF_Valor });
                        Respuesta.IdTipoEnvio = Convert.ToInt16(reader["ADM_IdTipoEnvio"]);
                        Respuesta.NombreTipoEnvio = Convert.ToString(reader["ADM_NombreTipoEnvio"]);
                        Respuesta.TotalPiezas = Convert.ToInt16(reader["ADM_TotalPiezas"]);
                        Respuesta.PrefijoNumeroGuia = Convert.ToString(reader["ADM_PrefijoNumeroGuia"]);
                        Respuesta.TelefonoDestinatario = Convert.ToString(reader["ADM_TelefonoDestinatario"]);
                        Respuesta.DireccionDestinatario = Convert.ToString(reader["ADM_DireccionDestinatario"]);
                        Respuesta.TipoCliente = (ADEnumTipoCliente)Enum.Parse(typeof(ADEnumTipoCliente), Convert.ToString(reader["ADM_TipoCliente"]), true);
                        Respuesta.Remitente = new CLClienteContadoDC()
                        {
                            Nombre = Convert.ToString(reader["ADM_NombreRemitente"]),
                            Telefono = Convert.ToString(reader["ADM_TelefonoRemitente"]),
                            Identificacion = Convert.ToString(reader["ADM_IdRemitente"]),
                            Direccion = Convert.ToString(reader["ADM_DireccionRemitente"]),
                            TipoId = Convert.ToString(reader["ADM_IdTipoIdentificacionRemitente"]),
                            Email = Convert.ToString(reader["ADM_EmailRemitente"])
                        };
                        Respuesta.Destinatario = new CLClienteContadoDC()
                        {
                            Nombre = Convert.ToString(reader["ADM_NombreDestinatario"]),
                            Direccion = Convert.ToString(reader["ADM_DireccionDestinatario"]),
                            TipoId = Convert.ToString(reader["ADM_IdTipoIdentificacionDestinatario"]),
                            Telefono = Convert.ToString(reader["ADM_TelefonoDestinatario"]),
                            Identificacion = Convert.ToString(reader["ADM_IdDestinatario"]),
                            Email = Convert.ToString(reader["ADM_EmailDestinatario"])
                        };
                        Respuesta.DiceContener = Convert.ToString(reader["ADM_DiceContener"]);
                        Respuesta.Supervision = Convert.ToBoolean(reader["ADM_EsSupervisada"]);
                        Respuesta.FechaSupervision = Convert.ToDateTime(reader["ADM_FechaSupervision"]);
                        Respuesta.IdMensajero = Convert.ToInt64(reader["ADM_IdMensajero"]);
                        Respuesta.NombreMensajero = Convert.ToString(reader["ADM_NombreMensajero"]);
                        Respuesta.DiasDeEntrega = Convert.ToInt16(reader["ADM_DiasDeEntrega"]);
                        Respuesta.DescripcionTipoEntrega = Convert.ToString(reader["ADM_DescripcionTipoEntrega"]);
                        Respuesta.EsRecomendado = Convert.ToBoolean(reader["ADM_EsRecomendado"]);
                        Respuesta.Alto = Convert.ToDecimal(reader["ADM_Alto"]);
                        Respuesta.Ancho = Convert.ToDecimal(reader["ADM_Ancho"]);
                        Respuesta.Largo = Convert.ToDecimal(reader["ADM_Largo"]);
                        Respuesta.DigitoVerificacion = Convert.ToString(reader["ADM_DigitoVerificacion"]);
                        Respuesta.FechaEstimadaEntrega = Convert.ToDateTime(reader["ADM_FechaEstimadaEntrega"]);
                        Respuesta.NumeroPieza = Convert.ToInt16(reader["ADM_NumeroPieza"]);
                        Respuesta.IdUnidadMedida = Convert.ToString(reader["ADM_IdUnidadMedida"]);
                        Respuesta.IdUnidadNegocio = Convert.ToString(reader["ADM_IdUnidadNegocio"]);
                        Respuesta.ValorTotalImpuestos = Convert.ToDecimal(reader["ADM_ValorTotalImpuestos"]);
                        Respuesta.ValorTotalRetenciones = Convert.ToDecimal(reader["ADM_ValorTotalRetenciones"]);
                        Respuesta.IdTipoEntrega = Convert.ToString(reader["ADM_IdTipoEntrega"]);
                        Respuesta.IdPaisDestino = Convert.ToString(reader["ADM_IdPaisDestino"]);
                        Respuesta.IdPaisOrigen = Convert.ToString(reader["ADM_IdPaisOrigen"]);
                        Respuesta.NombrePaisDestino = Convert.ToString(reader["ADM_NombrePaisDestino"]);
                        Respuesta.NombrePaisOrigen = Convert.ToString(reader["ADM_NombrePaisOrigen"]);
                        Respuesta.CodigoPostalDestino = Convert.ToString(reader["ADM_CodigoPostalDestino"]);
                        Respuesta.CodigoPostalOrigen = Convert.ToString(reader["ADM_CodigoPostalOrigen"]);
                        Respuesta.ValorAdicionales = Convert.ToDecimal(reader["ADM_ValorAdicionales"]);
                        Respuesta.NumeroBolsaSeguridad = Convert.ToString(reader["ADM_NumeroBolsaSeguridad"]);
                        Respuesta.IdMotivoNoUsoBolsaSegurida = Convert.ToInt16(reader["ADM_IdMotivoNoUsoBolsaSegurida"]);
                        Respuesta.MotivoNoUsoBolsaSeguriDesc = Convert.ToString(reader["ADM_MotivoNoUsoBolsaSeguriDesc"]);
                        Respuesta.NoUsoaBolsaSeguridadObserv = Convert.ToString(reader["ADM_NoUsoaBolsaSeguridadObserv"]);
                        Respuesta.PesoLiqMasa = Convert.ToDecimal(reader["ADM_PesoLiqMasa"]);
                        Respuesta.PesoLiqVolumetrico = Convert.ToDecimal(reader["ADM_PesoLiqVolumetrico"]);
                        Respuesta.CreadoPor = Convert.ToString(reader["ADM_CreadoPor"]);
                        Respuesta.ValorPrimaSeguro = Convert.ToDecimal(reader["ADM_ValorPrimaSeguro"]);
                        Respuesta.IdCliente = Convert.ToInt32(reader["MCP_IdConvenioRemitente"]);
                        Respuesta.NotificarEntregaPorEmail = Convert.ToString(reader["ADM_NotificarEntregaPorEmail"]) == null ? false : Convert.ToBoolean(reader["ADM_NotificarEntregaPorEmail"]);
                        Respuesta.IdSucursal = Convert.ToInt32(reader["MCP_IdSucursalRecogida"]);
                        Respuesta.DescripcionEstado = Convert.ToString(reader["ADM_DescripcionEstado"]);
                    }
                    sqlConn.Close();

                }
            }
            return Respuesta;

        }
        #endregion
        
        #region Estados

        public ADEstadoGuia ObtenerEstadoGuiaTrazaPorIdEstado(ADEnumEstadoGuia idEstadoGuia,long numeroGuia)
        {
            ADEstadoGuia estadoGuia = null;
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerTrazaEstadoGuiaPorIdEstado_MEN", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdEstadoGuia", (int)idEstadoGuia);
                cmd.Parameters.AddWithValue("@NumeroGuia", numeroGuia);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    estadoGuia = new ADEstadoGuia();

                    if (reader.Read())
                    {
                        estadoGuia.CreadoPor = reader["EGT_CreadoPor"]==DBNull.Value ? string.Empty : reader["EGT_CreadoPor"].ToString();            
                        estadoGuia.IdCentroServicio = reader["EGT_IdCentroServicio"] == DBNull.Value ? 0 : (long)reader["EGT_IdCentroServicio"];
                        estadoGuia.NombreCentroServicio = reader["EGT_NombreCentroServicio"] == DBNull.Value ? string.Empty : reader["EGT_NombreCentroServicio"].ToString();
                    }
                }
                return estadoGuia;
            }

        }
        #endregion



        public RGEmpleadoDC ObtenerDatosEmpleadoPorIdAdmisionGuiaAdmitida(long idAdmisionGuia)
        {
            RGEmpleadoDC datosEmpleado = new RGEmpleadoDC();
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerDatosDeEmpleadoPorIdAdmisionGuia_RAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroGuia", idAdmisionGuia);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    datosEmpleado.nombreEmpleado = reader["NombreCompleto"].ToString();
                    datosEmpleado.idEmpleado = reader["PEI_Identificacion"].ToString();
                    datosEmpleado.IdCentroServicios = Convert.ToInt64(reader["CES_IdCentroServicios"]);
                    datosEmpleado.TipoCentroServicio = (PUEnumTipoCentroServicioDC)Enum.Parse(typeof(PUEnumTipoCentroServicioDC), reader["CES_Tipo"].ToString());
                }
                conn.Close();
            }
            return datosEmpleado;
        }

    }
}