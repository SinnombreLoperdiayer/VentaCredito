using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceModel;
using CO.Servidor.LogisticaInversa.Comun;
using CO.Servidor.LogisticaInversa.Datos.Modelo;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Parametros.Consecutivos;

using System.Data;
using System.Data.SqlClient;

namespace CO.Servidor.LogisticaInversa.Datos
{
    /// <summary>
    ///  Repositorio para digitalización y archivo
    /// </summary>
    public class LIRepositorioDigitalizacionArchivo
    {
        #region Campos

        private static readonly LIRepositorioDigitalizacionArchivo instancia = new LIRepositorioDigitalizacionArchivo();
        private const string NombreModelo = "ModeloLogisticaInversa";
        private string CadCnxController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;


        public static LIRepositorioDigitalizacionArchivo Instancia
        {
            get { return LIRepositorioDigitalizacionArchivo.instancia; }
        }
        #endregion

        #region Volantes de devolución

        /// <summary>
        /// Método para validar la digitalización de los volantes de devolución
        /// </summary>
        /// <param name="imagen"></param>
        /// <returns></returns>
        public LIEvidenciaDevolucionDC ValidarArchivosVolantes(LIEvidenciaDevolucionDC imagen)
        {
            using (EntidadesLogisticaInversa contexto = new EntidadesLogisticaInversa(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                long numeroVolante = 0;
                long.TryParse(imagen.Archivo.NumeroVolante, out numeroVolante);
                EvidenciaDevolucion_MEN evidencia = contexto.EvidenciaDevolucion_MEN
                  .Where(w => w.VOD_NumeroEvidencia == numeroVolante)
                  .FirstOrDefault();

                if (evidencia == null)
                {
                    imagen.Archivo.ResultadoEscaner = ADEnumResultadoEscaner.NoAdmitida;
                }
                else
                {
                    imagen.IdEvidenciaDevolucion = evidencia.VOD_IdEvidenciaDevolucion;
                    if (evidencia.VOD_EstaDigitalizado)
                        imagen.Archivo.ResultadoEscaner = ADEnumResultadoEscaner.Duplicada;
                    else
                        imagen.Archivo.ResultadoEscaner = ADEnumResultadoEscaner.Exitosa;
                }
                return imagen;
            }
        }


        public LIEvidenciaDevolucionDC ObtenerEvidenciaDevolucion(long numeroVolante)
        {
            LIEvidenciaDevolucionDC evidencia = null;
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                conn.Open();
                SqlCommand comando = new SqlCommand("paObtenerEvidenciaDevolucion_LOI", conn);
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.AddWithValue("@numeroVolante", numeroVolante);
                SqlDataReader reader = comando.ExecuteReader();
                if (reader.Read())
                {
                    evidencia = new LIEvidenciaDevolucionDC()
                    {
                        IdEvidenciaDevolucion = Convert.ToInt64(reader["VOD_IdEvidenciaDevolucion"]),
                        EstaDigitalizado = Convert.ToBoolean(reader["VOD_EstaDigitalizado"]),
                        NumeroGuia = Convert.ToInt64(reader["NumeroGuia"]),
                    };
                }
            }
            return evidencia;
        }

        public void ActualizaEvidenciaDevolucionADigitalizado(long IdEvidenciaDevolucion)
        {
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                conn.Open();
                SqlCommand comando = new SqlCommand("paActualizaEvidenciaDevolucionADigitalizado_LOI", conn);
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.AddWithValue("@IdEvidenciaDevolucion", IdEvidenciaDevolucion);
                comando.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);
                comando.Parameters.AddWithValue("@IdCentroLogistico", ControllerContext.Current.IdCentroServicio);
                comando.ExecuteNonQuery();
            }
        }

        public long AsociarGuiaAVolante(LIEvidenciaDevolucionDC EvidenciaVolante, long numeroVolante)
        {
            long newIdEvidenciaDevolucion = 0;
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                conn.Open();
                SqlCommand comando = new SqlCommand("paAsociarGuiaAVolante_LOI", conn);
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.AddWithValue("@IdEvidenciaDevolucion", EvidenciaVolante.IdEvidenciaDevolucion);
                comando.Parameters.AddWithValue("@NumeroVolante", numeroVolante);
                comando.ExecuteNonQuery();
            }
            return newIdEvidenciaDevolucion;
        }

        public List<LIEvidenciaDevolucionDC> ObtenerEvidenciaDevolucionxGuia(long NumeroGuia)
        {
            LIEvidenciaDevolucionDC newObj;
            List<LIEvidenciaDevolucionDC> lista = new List<LIEvidenciaDevolucionDC>();
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                conn.Open();
                SqlCommand comando = new SqlCommand("paObtenerEvidenciaDevolucionxGuia_LOI", conn);
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.AddWithValue("@NumeroGuia", NumeroGuia);
                SqlDataReader reader = comando.ExecuteReader();

                while (reader.Read())
                {
                    newObj = new LIEvidenciaDevolucionDC();

                    newObj.IdEvidenciaDevolucion = Convert.ToInt64(reader["VOD_IdEvidenciaDevolucion"]);
                    newObj.NumeroGuia = Convert.ToInt64(reader["EGT_NumeroGuia"]);
                    newObj.NumeroEvidencia = Convert.ToInt64(reader["VOD_NumeroEvidencia"]);
                    newObj.EstaDigitalizado = Convert.ToBoolean(reader["Digitalizado"]);
                    newObj.Descripcion = reader["Descripcion"].ToString();

                    if (reader["EGM_FechaMotivo"] != DBNull.Value)
                        newObj.FechaMotivo = Convert.ToDateTime(reader["EGM_FechaMotivo"]);

                    lista.Add(newObj);
                }

            }
            return lista;
        }


        /// <summary>
        /// Actualiza un volante como digitalizado
        /// </summary>
        /// <param name="imagen"></param>
        public void ActualizarDigitalizadoVolante(LIEvidenciaDevolucionDC imagen)
        {
            using (EntidadesLogisticaInversa contexto = new EntidadesLogisticaInversa(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                long numeroVolante = 0;
                long.TryParse(imagen.Archivo.NumeroVolante, out numeroVolante);
                EvidenciaDevolucion_MEN evidencia = contexto.EvidenciaDevolucion_MEN
                  .Where(w => w.VOD_NumeroEvidencia == numeroVolante)
                  .FirstOrDefault();
                if (evidencia != null)
                {
                    evidencia.VOD_EstaDigitalizado = true;
                    contexto.SaveChanges();
                }
            }
        }
        #endregion Volantes de devolución


        /// <summary>
        /// Obtiene los datos de caja, lote y posición
        /// </summary>
        /// <param name="almacen">Objeto almacen</param>
        /// <returns>Almacen</returns>
        public LIArchivoGuiaMensajeriaDC ObtenerCajaLotePosicion(LIArchivoGuiaMensajeriaDC almacen)
        {
            // TODO: ID, Se cambia el uso de Entityframework por sp

            long Caja = 0;
            int Lote = 0;
            int Posicion = 0;

            #region Busca por Caja_Lote_Posicion

            DataSet dsRes = new DataSet();
            SqlDataAdapter da;
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerDatosdeAlmacenGuia_LOI", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@ARG_UsuarioArchiva", ControllerContext.Current.Usuario));
                cmd.Parameters.Add(new SqlParameter("@ARG_IdCentroLogArchiva", almacen.IdCol));

                da = new SqlDataAdapter(cmd);
                da.Fill(dsRes);
            }
            bool TraeDatos = false;
            if (dsRes.Tables[0].Rows.Count > 0)
            {
                TraeDatos = true;
                DataRow drow = dsRes.Tables[0].Rows[0];
                Caja = Convert.ToInt64(drow["ARG_Caja"]);
                Lote = Convert.ToInt32(drow["ARG_Lote"]);
                Posicion = Convert.ToInt32(drow["ARG_Posicion"]);
            }
            #endregion


            if (TraeDatos && Posicion < LOIConstantesLogisticaInversa.NUMERO_MAXIMO_POSICIONES_POR_LOTE)
            {
                almacen.Caja = (Caja == 0) ? 1 : Caja;
                almacen.Lote = (Lote == 0) ? 1 : Lote;
                almacen.Posicion = Posicion + 1;
            }
            else
            {

                #region Busca por Caja_Lote

                // busca lote disponible LOTE DISPONIBLE
                long Caja_LoteDispo = 0;
                int Lote_LoteDispo = 0;

                DataSet dsLoteDispo = new DataSet();
                SqlDataAdapter da2;
                using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
                {
                    sqlConn.Open();
                    SqlCommand cmd = new SqlCommand("paObtenerDatosdeAlmacenGuia_CajaLote_LOI", sqlConn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@ARG_IdCentroLogArchiva", almacen.IdCol));

                    da2 = new SqlDataAdapter(cmd);
                    da2.Fill(dsLoteDispo);
                }
                bool TraeDatosLoteDispo = false;
                if (dsLoteDispo.Tables[0].Rows.Count > 0)
                {
                    TraeDatosLoteDispo = true;
                    DataRow drow2 = dsLoteDispo.Tables[0].Rows[0];
                    Caja_LoteDispo = Convert.ToInt64(drow2["ARG_Caja"]);
                    Lote_LoteDispo = Convert.ToInt32(drow2["ARG_Lote"]);
                }
                #endregion

                if (TraeDatosLoteDispo)
                {
                    if (Lote_LoteDispo < LOIConstantesLogisticaInversa.NUMERO_MAXIMO_LOTES_POR_CAJA)
                    {
                        almacen.Caja = Caja_LoteDispo;
                        almacen.Lote = Lote_LoteDispo + 1;
                        almacen.Posicion = 1;
                    }
                    else
                    {
                        almacen.Caja = Caja_LoteDispo + 1;
                        almacen.Lote = 1;
                        almacen.Posicion = 1;
                        almacen.CajaLlena = true;
                    }
                }
                else
                {
                    almacen.Caja = 1;
                    almacen.Lote = 1;
                    almacen.Posicion = 1;
                    almacen.CajaLlena = false;
                }
            }

            return almacen;


            #region Codigo Anterior

            //using (EntidadesLogisticaInversa contexto = new EntidadesLogisticaInversa(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //AlmacenGuia_LOI consulta = contexto.AlmacenGuia_LOI
            //  .Where(r => r.ARG_UsuarioArchiva == ControllerContext.Current.Usuario && r.ARG_IdCentroLogArchiva == ControllerContext.Current.IdCentroServicio)
            //  .OrderByDescending(o => o.ARG_Caja)
            //  .ThenByDescending(ol => ol.ARG_Lote)
            //  .ThenByDescending(ot => ot.ARG_Posicion)
            //  .FirstOrDefault();
            //if (consulta != null && consulta.ARG_Posicion < LOIConstantesLogisticaInversa.NUMERO_MAXIMO_POSICIONES_POR_LOTE)
            //{
            //    if (consulta.ARG_Caja == 0)
            //        almacen.Caja = 1;
            //    else
            //        almacen.Caja = consulta.ARG_Caja;
            //    if (consulta.ARG_Lote == 0)
            //        almacen.Lote = 1;
            //    else
            //        almacen.Lote = consulta.ARG_Lote;
            //    almacen.Posicion = consulta.ARG_Posicion + 1;
            //}
            //else
            //{
            //    AlmacenGuia_LOI loteDisponible = contexto.AlmacenGuia_LOI
            //      .Where(ld => ld.ARG_IdCentroLogArchiva == ControllerContext.Current.IdCentroServicio)
            //      .OrderByDescending(c => c.ARG_Caja)
            //      .ThenByDescending(old => old.ARG_Lote)
            //      .FirstOrDefault();

            //    if (loteDisponible != null)
            //    {
            //        if (loteDisponible.ARG_Lote < LOIConstantesLogisticaInversa.NUMERO_MAXIMO_LOTES_POR_CAJA)
            //        {
            //            almacen.Caja = loteDisponible.ARG_Caja;
            //            almacen.Lote = loteDisponible.ARG_Lote + 1;
            //            almacen.Posicion = 1;
            //        }
            //        else
            //        {
            //            almacen.Caja = loteDisponible.ARG_Caja + 1;
            //            almacen.Lote = 1;
            //            almacen.Posicion = 1;
            //            almacen.CajaLlena = true;
            //        }
            //    }
            //    else
            //    {
            //        almacen.Caja = 1;
            //        almacen.Lote = 1;
            //        almacen.Posicion = 1;
            //        almacen.CajaLlena = false;
            //    }

            //}
            //return almacen;
            //}
            #endregion

        }

        public LIArchivoGuiaMensajeriaDC ObtenerCajaLotePosicionIntentoEntrega(ref LIArchivoGuiaMensajeriaDC almacen)
        {
            
            long Caja = 0;
            int Lote = 0;
            int Posicion = 0;

            #region Busca por Caja_Lote_Posicion

            DataSet dsRes = new DataSet();
            SqlDataAdapter da;
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("pa_ObtenerCajaLotePosicionIntentoEntrega_LOI", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@AAI_UsuarioArchiva", ControllerContext.Current.Usuario));
                cmd.Parameters.Add(new SqlParameter("@AAI_IdCentroLogArchiva", almacen.IdCol));

                da = new SqlDataAdapter(cmd);
                da.Fill(dsRes);
            }
            bool TraeDatos = false;
            if (dsRes.Tables[0].Rows.Count > 0)
            {
                TraeDatos = true;
                DataRow drow = dsRes.Tables[0].Rows[0];
                Caja = Convert.ToInt64(drow["AAI_Caja"]);
                Lote = Convert.ToInt32(drow["AAI_Lote"]);
                Posicion = Convert.ToInt32(drow["AAI_Posicion"]);
            }
            #endregion


            if (TraeDatos && Posicion < LOIConstantesLogisticaInversa.NUMERO_MAXIMO_POSICIONES_POR_LOTE)
            {
                almacen.Caja = (Caja == 0) ? 1 : Caja;
                almacen.Lote = (Lote == 0) ? 1 : Lote;
                almacen.Posicion = Posicion + 1;
            }
            else
            {

                #region Busca por Caja_Lote

                // busca lote disponible LOTE DISPONIBLE
                long Caja_LoteDispo = 0;
                int Lote_LoteDispo = 0;

                DataSet dsLoteDispo = new DataSet();
                SqlDataAdapter da2;
                using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
                {
                    sqlConn.Open();
                    SqlCommand cmd = new SqlCommand("pa_ObtenerCajaLotePosicionIntentoEntregaPorBodega_LOI", sqlConn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@AAI_IdCentroLogArchiva", almacen.IdCol));

                    da2 = new SqlDataAdapter(cmd);
                    da2.Fill(dsLoteDispo);
                }
                bool TraeDatosLoteDispo = false;
                if (dsLoteDispo.Tables[0].Rows.Count > 0)
                {
                    TraeDatosLoteDispo = true;
                    DataRow drow2 = dsLoteDispo.Tables[0].Rows[0];
                    Caja_LoteDispo = Convert.ToInt64(drow2["AAI_Caja"]);
                    Lote_LoteDispo = Convert.ToInt32(drow2["AAI_Lote"]);
                }
                #endregion

                if (TraeDatosLoteDispo)
                {
                    if (Lote_LoteDispo < LOIConstantesLogisticaInversa.NUMERO_MAXIMO_LOTES_POR_CAJA)
                    {
                        almacen.Caja = Caja_LoteDispo;
                        almacen.Lote = Lote_LoteDispo + 1;
                        almacen.Posicion = 1;
                    }
                    else
                    {
                        almacen.Caja = Caja_LoteDispo + 1;
                        almacen.Lote = 1;
                        almacen.Posicion = 1;
                        almacen.CajaLlena = true;
                    }
                }
                else
                {
                    almacen.Caja = 1;
                    almacen.Lote = 1;
                    almacen.Posicion = 1;
                }
                //else
                //{
                //    almacen.Caja = 1;
                //    almacen.Lote = 1;
                //    almacen.Posicion = 1;
                //    almacen.CajaLlena = false;

                //    using (SqlConnection cnx = new SqlConnection(CadCnxController))
                //    {
                //        cnx.Open();
                //        SqlCommand cmd = new SqlCommand("pa_CrearCajaLotePosicionIntentoEntrega_LOI", cnx);
                //        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                //        cmd.Parameters.Add(new SqlParameter("@AAI_NumeroEvidencia", almacen.NumeroEvidencia));
                //        cmd.Parameters.Add(new SqlParameter("@AAI_Caja", almacen.Caja));
                //        cmd.Parameters.Add(new SqlParameter("@AAI_Lote", almacen.Lote));
                //        cmd.Parameters.Add(new SqlParameter("@AAI_Posicion", almacen.Posicion));
                //        cmd.Parameters.Add(new SqlParameter("@AAI_DatosEdicion", almacen.EstadoDatosEdicion));
                //        cmd.Parameters.Add(new SqlParameter("@AAI_DatosEntrega", almacen.EstadoDatosEntrega));
                //        cmd.Parameters.Add(new SqlParameter("@AAI_EstadoFisicoGuia", almacen.EstadoFisicoGuia));
                //        cmd.Parameters.Add(new SqlParameter("@AAI_CreadoPor", almacen.CreadoPor));
                //        cmd.Parameters.Add(new SqlParameter("@AAI_FechaEntrega", almacen.FechaEntrega));
                //        cmd.Parameters.Add(new SqlParameter("@AAI_FechaArchivo", almacen.FechaArchivo));
                //        cmd.Parameters.Add(new SqlParameter("@AAI_UsuarioArchiva", ControllerContext.Current.Usuario));
                //        cmd.Parameters.Add(new SqlParameter("@AAI_IdCentroLogArchiva", almacen.IdCol));
                //        cmd.Parameters.Add(new SqlParameter("@AAI_Decodificada", almacen.Decodificada));
                //        cmd.Parameters.Add(new SqlParameter("@AAI_Manual", almacen.Manual));
                //        cmd.Parameters.Add(new SqlParameter("@AAI_CarpetaImagen", almacen.EsCarpeta));

                //        da2 = new SqlDataAdapter(cmd);
                //        da2.Fill(dsLoteDispo);
                //    }                  
                //}
            }

            return almacen;

        }

    }
}