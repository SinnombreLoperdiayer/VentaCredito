using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.ControlCuentas;
using System.Data.SqlClient;
using System.Data;
using Framework.Servidor.Comun.Util;
using Framework.Servidor.Excepciones;
using System.IO;
using System.Drawing.Imaging;
using Framework.Servidor.ParametrosFW;
using System.Drawing;

namespace CO.Servidor.ControlCuentas.Datos
{
    public class CCRepositorioApp
    {

        private static readonly CCRepositorioApp instancia = new CCRepositorioApp();
        private string conexionStringController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;

        #region Singleton
        /// <summary>
        /// Retorna la instancia de la clase CCRepositorioApp
        /// </summary>
        public static CCRepositorioApp Instancia
        {
            get { return CCRepositorioApp.instancia; }
        }

        #endregion

        #region Metodos

        /// <summary>
        /// Metodo para obtener cantidades de guias por auditar
        /// </summary>
        /// <param name="idCentroLogistico"></param>
        /// <returns></returns>
        public CCRespuestaAuditoriaDC ObtenerCantidadGuiasPorAuditar(long idCentroLogistico)
        {
            CCRespuestaAuditoriaDC result = new CCRespuestaAuditoriaDC();
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerCantidadGuiasPorAuditar_CCU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdCentroLogistico", idCentroLogistico);
                cmd.Parameters.AddWithValue("@FechaAuditoria", DateTime.Now);
                conn.Open();
                result.CantidadPorAuditar = Convert.ToInt32(cmd.ExecuteScalar());
                return result;
            }
        }

        /// <summary>
        /// Metodo para validar si la guia ha tenido novedades de control de liquidacion
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public bool ConsultarNovedadesControlLiquidacionPorGuia(long numeroGuia)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paConsultarNovedadesLiquidacionGuia_CCU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroGuia", numeroGuia);
                conn.Open();
                var cantidad = cmd.ExecuteScalar();
                return (Convert.ToInt16(cantidad) > 0);
            }
        }


        /// <summary>
        /// Metodo para obtener guia para auditoria 
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public CCRespuestaAuditoriaDC ObtenerGuiaAuditoriaLiquidacion(long numeroGuia)
        {

            CCRespuestaAuditoriaDC respuestaAuditoria = new CCRespuestaAuditoriaDC();
            respuestaAuditoria.EstadoSolicitud = false;
            respuestaAuditoria.MensajeRespuesta = "La guía no existe";
            respuestaAuditoria.NumeroGuia = numeroGuia;

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerGuiaAuditoriaLiquidacion_CCU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroGuia", numeroGuia);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        respuestaAuditoria.GuiaAuditoria = new CCGuiaDC();
                        respuestaAuditoria.GuiaAuditoria.NumeroGuia = reader["ADM_NumeroGuia"] == DBNull.Value ? 0 : Convert.ToInt64(reader["ADM_NumeroGuia"]);
                        respuestaAuditoria.GuiaAuditoria.EsAutomatico = reader["ADM_EsAutomatico"] == DBNull.Value ? false : Convert.ToBoolean(reader["ADM_EsAutomatico"]);
                        respuestaAuditoria.GuiaAuditoria.IdEstadoGuia = reader["ADM_IdEstadoGuia"] == DBNull.Value ? 0 : Convert.ToInt16(reader["ADM_IdEstadoGuia"]);
                        /*** Informacion Sistema ****/
                        respuestaAuditoria.GuiaAuditoria.ValorComercialAdmision = reader["ADM_ValorDeclarado"] == DBNull.Value ? (decimal)0 : Convert.ToDecimal(reader["ADM_ValorDeclarado"]);
                        respuestaAuditoria.GuiaAuditoria.ValorTotalAdmision = reader["ADM_ValorTotal"] == DBNull.Value ? (decimal)0 : Convert.ToDecimal(reader["ADM_ValorTotal"]);
                        respuestaAuditoria.GuiaAuditoria.PesoTotalAdmision = reader["ADM_Peso"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["ADM_Peso"]);
                        respuestaAuditoria.EstadoSolicitud = true;
                        respuestaAuditoria.MensajeRespuesta = "Guía encontrada";
                    }
                }
            }
            return respuestaAuditoria;
        }

        /// <summary>
        /// Metodo para 
        /// </summary>
        /// <param name="guia"></param>
        public int InsertarNovedadControlLiquidacion(CCGuiaDC guia)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paInsertarNovedadControlLiquidacion_CCU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroGuia", guia.NumeroGuia);
                cmd.Parameters.AddWithValue("@EsNovedad", guia.EsNovedad);

                /************** Valores Auditoria ***************/
                cmd.Parameters.AddWithValue("@IdServicioInicial", guia.IdServicioInicial);
                cmd.Parameters.AddWithValue("@NombreServicioInicial", guia.DescripcionServicioInicial == null ? string.Empty : guia.DescripcionServicioInicial);
                cmd.Parameters.AddWithValue("@IdServicioAuditoria", guia.IdServicioAuditoria);
                cmd.Parameters.AddWithValue("@NombreServicioAuditoria", guia.DescripcionServicioAuditoria == null ? string.Empty : guia.DescripcionServicioAuditoria);
                cmd.Parameters.AddWithValue("@IdCiudadDestinoInicial", guia.IdCiudadDestinoInicial);
                cmd.Parameters.AddWithValue("@NombreCiudadDestinoInicial", guia.NombreCiudadDestinoInicial == null ? string.Empty : guia.NombreCiudadDestinoInicial);
                cmd.Parameters.AddWithValue("@IdCiudadDestinoAuditoria", guia.IdCiudadDestinoAuditoria);
                cmd.Parameters.AddWithValue("@NombreCiudadDestinoAuditoria", guia.NombreCiudadDestinoAuditoria == null ? string.Empty : guia.NombreCiudadDestinoAuditoria);
                cmd.Parameters.AddWithValue("@IdFormaPagoInicial", guia.IdFormaPagoInicial);
                cmd.Parameters.AddWithValue("@NombreFormaPagoInicial", guia.DescripcionFormaPagoInicial == null ? string.Empty : guia.DescripcionFormaPagoInicial);
                cmd.Parameters.AddWithValue("@IdFormaPagoAuditoria", guia.IdFormaPagoAuditoria);
                cmd.Parameters.AddWithValue("@NombreFormaPagoAuditoria", guia.DescripcionFormaPagoAuditoria == null ? string.Empty : guia.DescripcionFormaPagoAuditoria);

                /***** Valores Admision ***/
                cmd.Parameters.AddWithValue("@VlrComercialAdmision", guia.ValorComercialAdmision);
                cmd.Parameters.AddWithValue("@VlrTotalAdmision", guia.ValorTotalAdmision);
                cmd.Parameters.AddWithValue("@PesoTotalAdmision", guia.PesoTotalAdmision);

                /***** Valores Prueba de entrega ***/
                cmd.Parameters.AddWithValue("@VlrComercialPruebaEntrega", guia.ValorComercialPruebaEntrega);
                cmd.Parameters.AddWithValue("@VlrTotalPruebaEntrega", guia.ValorTotalPruebaEntrega);
                cmd.Parameters.AddWithValue("@PesoTotalPruebaEntrega", guia.PesoTotalPruebaEntrega);

                /******************** Validacion de pesos ***********************************/
                cmd.Parameters.AddWithValue("@LargoAuditoria", guia.LargoVolumetricoAuditoria);
                cmd.Parameters.AddWithValue("@AnchoAuditoria", guia.AnchoVolumetricoAuditoria);
                cmd.Parameters.AddWithValue("@AltoAuditoria", guia.AltoVolumetricoAuditoria);
                cmd.Parameters.AddWithValue("@PesoBasculaAuditoria", guia.PesoBasculaAuditoria);
                cmd.Parameters.AddWithValue("@PesoVolumetricoAuditoria", guia.PesoVolumetricoTotalAuditoria);
                cmd.Parameters.AddWithValue("@PesoTotalAuditoria", guia.PesoTotalAuditoria);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current == null ? "" : ControllerContext.Current.Usuario);
                cmd.Parameters.AddWithValue("@ObservacionesAuditoria", guia.ObservacionesAuditoria);
                cmd.Parameters.AddWithValue("@FechaAuditoria", DateTime.Now);
                cmd.Parameters.AddWithValue("@IdCentroLogistico", guia.IdCentroServicio);
                conn.Open();

                var respuesta = cmd.ExecuteScalar();

                return Convert.ToInt16(respuesta);
            }
        }

        /// <summary>
        /// METODO PARA INSERTAR TIPOS DE NOVEDADES REGISTRADAS POR AUDITORIA 
        /// </summary>
        /// <param name="pk"></param>
        /// <param name="numeroGuia"></param>
        /// <param name="item"></param>
        public void InsertarNovedadPorNumeroGuia(int pk, long numeroGuia, CCEnumTipoNovedadCtrLiquidacionDC item)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paInsertarNovedadesGuiaControlLiquidacion_CCU", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdAuditoria", pk);
                cmd.Parameters.AddWithValue("@NumeroGuia", numeroGuia);
                cmd.Parameters.AddWithValue("@TipoNovedad", item);

                string detalleTipoNovedad = item == CCEnumTipoNovedadCtrLiquidacionDC.Peso
                    ? "PESO" : item == CCEnumTipoNovedadCtrLiquidacionDC.Servicio
                    ? "SERVICIO" : item == CCEnumTipoNovedadCtrLiquidacionDC.Ciudad_Destino
                    ? "CIUDAD DESTINO" : item == CCEnumTipoNovedadCtrLiquidacionDC.Forma_Pago
                    ? "FORMA PAGO" : item == CCEnumTipoNovedadCtrLiquidacionDC.Valor_Comercial
                    ? "VALOR COMERCIAL" : item == CCEnumTipoNovedadCtrLiquidacionDC.Valor_Total
                    ? "VALOR TOTAL" : item == CCEnumTipoNovedadCtrLiquidacionDC.Sin_Novedades
                    ? "SIN NOVEDADES" : string.Empty;

                cmd.Parameters.AddWithValue("@DescripcionNovedad", detalleTipoNovedad);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current == null ? 0 : ControllerContext.Current.Identificacion);
                cmd.Parameters.AddWithValue("@IdEstadoNovedad", CCEnumEstadoNovedadCtrlLiquidacionDC.CREADA);
                cmd.Parameters.AddWithValue("@DescripcionEstadoNovedad", "CREADA");
                conn.Open();
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                conn.Close();
            }
        }

        /// <summary>
        /// Metodo para insertar traza auditoria de guia 
        /// </summary>
        /// <param name="guia"></param>
        public void InsertarTrazaNovedadControlLiquidacion(int pk, CCGuiaDC guia, CCEnumTipoNovedadCtrLiquidacionDC tipoNovedad, CCEnumEstadoNovedadCtrlLiquidacionDC estadoNovedad)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paInsertarTrazaNovedadCtrlLiquidacion_CCU", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdAuditoria", pk);
                cmd.Parameters.AddWithValue("@NumeroGuia", guia.NumeroGuia);
                cmd.Parameters.AddWithValue("@IdTipoNovedad", (int)tipoNovedad);

                string detalleTipoNovedad = tipoNovedad == CCEnumTipoNovedadCtrLiquidacionDC.Peso
                    ? "PESO" : tipoNovedad == CCEnumTipoNovedadCtrLiquidacionDC.Servicio
                    ? "SERVICIO" : tipoNovedad == CCEnumTipoNovedadCtrLiquidacionDC.Ciudad_Destino
                    ? "CIUDAD DESTINO" : tipoNovedad == CCEnumTipoNovedadCtrLiquidacionDC.Forma_Pago
                    ? "FORMA PAGO" : tipoNovedad == CCEnumTipoNovedadCtrLiquidacionDC.Valor_Comercial
                    ? "VALOR COMERCIAL" : tipoNovedad == CCEnumTipoNovedadCtrLiquidacionDC.Valor_Total
                    ? "VALOR TOTAL" : tipoNovedad == CCEnumTipoNovedadCtrLiquidacionDC.Sin_Novedades
                    ? "SIN NOVEDADES" : string.Empty;

                cmd.Parameters.AddWithValue("@DescripcionNovedad", detalleTipoNovedad);

                string descripcionEstadoNovedad = estadoNovedad == CCEnumEstadoNovedadCtrlLiquidacionDC.CREADA
                    ? "CREADA" : estadoNovedad == CCEnumEstadoNovedadCtrlLiquidacionDC.GESTIONADA
                    ? "GESTIONADA" : estadoNovedad == CCEnumEstadoNovedadCtrlLiquidacionDC.ANULADA
                    ? "ANULADA" : string.Empty;

                cmd.Parameters.AddWithValue("@IdEstadoNovedad", estadoNovedad);
                cmd.Parameters.AddWithValue("@DescripcionEstadoNovedad", descripcionEstadoNovedad);
                cmd.Parameters.AddWithValue("@CreadoPor",  ControllerContext.Current.Usuario);
                conn.Open();
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                conn.Close();
            }
        }

        /// <summary>
        /// Metodo para insertar las imagenes de novedad 
        /// </summary>
        /// <param name="MisImagenes"></param>
        public void InsertarImagenesNovedad(int pk, List<string> MisImagenes, long NumeroGuia)
        {
            string rutaImagenes;
            string carpetaDestino;

            using (SqlConnection cnn = new SqlConnection(conexionStringController))
            {
                int file = 0;
                rutaImagenes = PAParametros.Instancia.ConsultarParametrosFramework("FoldImgAControlLiq");
                carpetaDestino = Path.Combine(rutaImagenes + "\\" + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year);
                if (!Directory.Exists(carpetaDestino))
                {
                    Directory.CreateDirectory(carpetaDestino);
                }

                if (MisImagenes.Count > 0)
                {
                    cnn.Open();
                }

                foreach (string item in MisImagenes)
                {
                    file++;
                    byte[] bytebuffer = Convert.FromBase64String(item);
                    MemoryStream memoryStream = new MemoryStream(bytebuffer);
                    var image = Image.FromStream(memoryStream);
                    ImageCodecInfo jpgEncoder = UtilidadesFW.GetEncoder(ImageFormat.Jpeg);
                    string ruta = carpetaDestino + "\\" + NumeroGuia + "-" + file + ".jpg";
                    System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                    EncoderParameters myEncoderParameters = new EncoderParameters(1);
                    EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 180L);
                    myEncoderParameters.Param[0] = myEncoderParameter;
                    image.Save(ruta, jpgEncoder, myEncoderParameters);

                    SqlCommand cmd = new SqlCommand("paInsertarImagenesAuditoriaCtrlLiquidacion_CCU", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdAuditoria", pk);
                    cmd.Parameters.AddWithValue("@NumeroGuia", NumeroGuia);
                    cmd.Parameters.AddWithValue("@RutaImagen", ruta);
                    cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current == null ? 0 : ControllerContext.Current.Identificacion);
                    cmd.Parameters.AddWithValue("@FechaGrabacion", DateTime.Now);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        /// <summary>
        /// Metodo para consultar lista de guias de novedades control liquidacion 
        /// </summary>
        /// <returns></returns>
        public List<CCGuiaIdAuditoria> ConsultarGuiasNovedadesControlLiquidacion(int indicePagina, int registrosPorPagina)
        {
            List<CCGuiaIdAuditoria> listaTemp = new List<CCGuiaIdAuditoria>();
            using (SqlConnection cnn = new SqlConnection(conexionStringController))
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerGuiasNovedadesControlLiquidacion_CCU", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PageIndex", indicePagina);
                cmd.Parameters.AddWithValue("@PageSize", registrosPorPagina);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    CCGuiaIdAuditoria guia = new CCGuiaIdAuditoria()
                    {
                        IdAuditoriaGuia = Convert.ToInt64(reader["NCL_IdAuditoria"]),
                        NumeroGuia = Convert.ToInt64(reader["NCL_NumeroGuia"])
                    }; 
                    listaTemp.Add(guia);
                }
                reader.Close();

                cnn.Close();
            }
            return listaTemp;
        }
        /// <summary>
        /// metodo para actualizar liquidacion
        /// </summary>
        /// <param name="NumeroGuia"></param>
        /// <param name="tipoNovedad"></param>
        /// <param name="estadoNovedad"></param>
        public void ActualizarNovedadControlLiquidacion(long NumeroGuia, CCEnumTipoNovedadCtrLiquidacionDC tipoNovedad, CCEnumEstadoNovedadCtrlLiquidacionDC estadoNovedad)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paActualizarNovedadControlLiquidacion_CCU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroGuia", NumeroGuia);
                cmd.Parameters.AddWithValue("@TipoNovedad", (int)tipoNovedad);
                cmd.Parameters.AddWithValue("@NuevoEstadoNovedad", (int)estadoNovedad);

                string descripcionEstadoNovedad = estadoNovedad == CCEnumEstadoNovedadCtrlLiquidacionDC.CREADA
                    ? "CREADA" : estadoNovedad == CCEnumEstadoNovedadCtrlLiquidacionDC.GESTIONADA
                    ? "GESTIONADA" : estadoNovedad == CCEnumEstadoNovedadCtrlLiquidacionDC.ANULADA
                    ? "ANULADA" : string.Empty;

                cmd.Parameters.AddWithValue("@DescripcionNuevoEstado", descripcionEstadoNovedad);
                conn.Open();
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                conn.Close();
            }
        }
        /// <summary>
        /// Metodo para obtener una lista con los tipos de novedades de una guia  
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<short> ObtenerTipoNovedadesGuia(long numeroGuia, int IdEstadoNovedad)
        {
            List<short> listaTemp = new List<short>();
            using (SqlConnection cnn = new SqlConnection(conexionStringController))
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerNovedadesPorGuiaControlLiquidacion_CCU", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroGuia", numeroGuia);
                cmd.Parameters.AddWithValue("@IdNovedad", IdEstadoNovedad);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    short tipoNovedad = Convert.ToInt16(reader["NACL_TipoNovedad"]);
                    listaTemp.Add(tipoNovedad);
                }
                reader.Close();

                cnn.Close();
            }
            return listaTemp;
        }
        /// <summary>
        /// Metodo para obtener peso volumetrico de una guia  
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public PesoVolGuiaDC ObtenerPesoVolumetricoGuia(long numeroGuia)
        {
            PesoVolGuiaDC guia = new PesoVolGuiaDC();
            using (SqlConnection cnn = new SqlConnection(conexionStringController))
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerPesoVolumetricoGuia_CCU", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroGuia", numeroGuia);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    guia = new PesoVolGuiaDC()
                    {
                        LargoVolumetricoAuditoria = Convert.ToDecimal(reader["NCL_LargoAuditoria"]),
                        AnchoVolumetricoAuditoria = Convert.ToDecimal(reader["NCL_AnchoAuditoria"]),
                        AltoVolumetricoAuditoria = Convert.ToDecimal(reader["NCL_AltoAuditoria"]),
                        PesoVolumetricoTotalAuditoria = Convert.ToDecimal(reader["NCL_PesoVolumetricoAuditoria"]),
                        FechaAuditoria = Convert.ToDateTime(reader["NCL_FechaGrabacion"]),
                        CreadoPor = Convert.ToString(reader["NCL_CreadoPor"]),
                        ObservacionesAuditoria = Convert.ToString(reader["NCL_Observaciones"]),
                        PesoBasculaAuditoria = Convert.ToDecimal(reader["NCL_PesoBasculaAuditoria"]),
                       
                    };
                }
                reader.Close();

                cnn.Close();
            }
            return guia;
        }
        /// <summary>
        /// Metodo para obtener peso volumetrico de una guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public PesoVolGuiaDC ObtenerPesoVolumetricoParaGuia(long numeroGuia)
        {
            PesoVolGuiaDC guia = new PesoVolGuiaDC();
            using (SqlConnection cnn = new SqlConnection(conexionStringController))
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerPesoVolumetricoGuia_CCU", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroGuia", numeroGuia);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    guia = new PesoVolGuiaDC()
                    {
                        LargoVolumetricoAuditoria = Convert.ToDecimal(reader["NCL_LargoAuditoria"]),
                        AnchoVolumetricoAuditoria = Convert.ToDecimal(reader["NCL_AnchoAuditoria"]),
                        AltoVolumetricoAuditoria = Convert.ToDecimal(reader["NCL_AltoAuditoria"]),
                        PesoVolumetricoTotalAuditoria = Convert.ToDecimal(reader["NCL_PesoVolumetricoAuditoria"]),
                        FechaAuditoria = Convert.ToDateTime(reader["NCL_FechaGrabacion"]),
                        CreadoPor = Convert.ToString(reader["NCL_CreadoPor"]),
                        ObservacionesAuditoria = Convert.ToString(reader["NCL_Observaciones"]),
                        PesoBasculaAuditoria = Convert.ToDecimal(reader["NCL_PesoBasculaAuditoria"])
                    };
                }
                reader.Close();

                cnn.Close();
            }
            return guia;
        }
        /// <summary>
        /// obtiene imagenes por numero de guia
        /// </summary>
        /// <param name="idSolicitudRecogida"></param>
        /// <returns></returns>
        public List<string> ObtenerImagenesNovedadGuia(long numeroGuia)
        {
            List<string> misFotografias = new List<string>();
            using (SqlConnection con = new SqlConnection(conexionStringController))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("paObtenerRutaImagenGuia_CCU", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@NumeroGuia", numeroGuia));
                SqlDataReader lector = cmd.ExecuteReader();

                while (lector.Read())
                {
                    byte[] imagen = File.ReadAllBytes(lector["IACL_RutaImagen"].ToString());
                    misFotografias.Add(Convert.ToBase64String(imagen));
                }
            }

            return misFotografias;

        }
        #endregion

    }
}
