using CO.Servidor.LogisticaInversa.Comun;
using CO.Servidor.LogisticaInversa.Datos.Modelo;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.Util;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Servicios.ContratoDatos.Parametros.Consecutivos;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Transactions;

namespace CO.Servidor.LogisticaInversa.Datos
{
    public class LIRepositorioDigitalizacion
    {
        #region Campos

        private static readonly LIRepositorioDigitalizacion instancia = new LIRepositorioDigitalizacion();
        private string conexionStringArchivo = ConfigurationManager.ConnectionStrings["ControllerArchivo"].ConnectionString;
        private string conexionStringController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;
        private string filePath = ConfigurationManager.AppSettings["Controller.RutaDescargaArchivos"];
        private const string NombreModelo = "ModeloLogisticaInversa";
        private string CadCnxSispostalController = ConfigurationManager.ConnectionStrings["Sispostal"].ConnectionString;

        #endregion Campos

        #region Propiedades

        /// <summary>
        /// Retorna la instancia de la clase LIRepositorioDigitalizacionArchivo
        /// </summary>
        public static LIRepositorioDigitalizacion Instancia
        {
            get { return LIRepositorioDigitalizacion.instancia; }
        }

        #endregion Propiedades

        #region Comprobante de Pago

        /// <summary>
        /// Retorna el comprobante de pago de un giro
        /// </summary>
        /// <param name="idAdmisionGiro">Identificador admisión giro</param>
        /// <returns>Archivo</returns>
        public string ObtenerComprobantePagoGiro(long idAdmisionGiro)
        {
            string query = "SELECT [APG_Adjunto] FROM ArchivoController.dbo.AlmacenArchivoPagoGiro_LOI WITH (NOLOCK) WHERE APG_IdAdmisionGiro = " + idAdmisionGiro.ToString();
            string archivo = string.Empty;

            using (SqlConnection sqlConn = new SqlConnection(conexionStringArchivo))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand(query, sqlConn);

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    Byte[] documento = row["APG_Adjunto"] as Byte[];
                    archivo = Convert.ToBase64String(documento);
                }
                sqlConn.Close();
                return archivo;
            }
        }

        #endregion Comprobante de Pago


        #region Volantes Guia

        /// <summary>
        /// Consulta pruebas de entrega.  Realizado por Mauricio Sanchez 20160205
        /// </summary>
        /// <param name="numeroGuia">objeto de tipo archivo</param>
        public List<string> ObtenerVolantesGuia(long numeroGuia)
        {
            List<string> lstimagenes = new List<string>();

            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {

                SqlCommand cmd = new SqlCommand("paObtenerVolantesGuia_LOI", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroGuia", numeroGuia);
                DataTable dt = new DataTable();
                sqlConn.Open();


                dt.Load(cmd.ExecuteReader());

                foreach (DataRow item in dt.Rows)
                {
                    if (item["ARV_RutaArchivo"] != DBNull.Value)
                    {
                        string ruta = item.Field<string>("ARV_RutaArchivo");
                        FileStream stream = File.OpenRead(ruta);
                        byte[] fileBytes = new byte[stream.Length];
                        stream.Read(fileBytes, 0, fileBytes.Length);
                        stream.Close();
                        lstimagenes.Add(Convert.ToBase64String(fileBytes));
                    }

                }
            }


            if (lstimagenes.Count == 0)
            {
                using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
                {
                    sqlConn.Open();
                    SqlCommand cmd = new SqlCommand("paObtenerVolantesGuiaHistorico_LOI", sqlConn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@NumeroGuia", numeroGuia);

                    DataTable dt = new DataTable();
                    dt.Load(cmd.ExecuteReader());

                    foreach (DataRow item in dt.Rows)
                    {
                        if (item["ARV_Adjunto"] != DBNull.Value)
                        {
                            Byte[] documento = item["ARV_Adjunto"] as Byte[];
                            lstimagenes.Add(Convert.ToBase64String(documento));
                        }
                    }
                }

            }

            return lstimagenes;
        }


        #endregion Volantes Guia




        #region Archivo Giro

        /// <summary>
        /// Obtiene booleano de existencia de imagen
        /// </summary>
        /// <param name="imagen">Objeto imagen</param>
        /// <returns>Booleano</returns>
        public bool ConsultarArchivoGiroExiste(LIArchivoGiroDC imagen)
        {
            string query = "SELECT AAG_IdAdmisionGiro FROM dbo.AlmacenArchivoGiro_LOI WHERE AAG_IdAdmisionGiro = " + imagen.IdAdmisionGiro.ToString();
            string archivo = string.Empty;

            using (SqlConnection sqlConn = new SqlConnection(conexionStringArchivo))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand(query, sqlConn);

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                sqlConn.Close();
                if (dt.Rows.Count > 0)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Adiciona un archivo giro
        /// </summary>
        /// <param name="imagen">Objeto imágen</param>
        public void AdicionarArchivoGiro(LIArchivoGiroDC imagen)
        {
            byte[] archivoImagen;
            using (FileStream fs = new FileStream(imagen.RutaServidor, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
                fs.Close();
                archivoImagen = bytes;
            }

            string query = @"INSERT INTO [AlmacenArchivoGiro_LOI] WITH (ROWLOCK)" +
            " ([AAG_NombreAdjunto] ,[AAG_Adjunto] ,[AAG_IdAdjunto]  ,[AAG_IdAdmisionGiro]  ,[AAG_IdGiro] ,[AAG_FechaGrabacion] ,[AAG_CreadoPor])  " +
         " VALUES(@NombreAdjunto ,@Adjunto ,@IdAdjunto ,@IdAdmisionGiro,@IdGiro ,GETDATE() ,@CreadoPor)";

            using (SqlConnection sqlConn = new SqlConnection(conexionStringArchivo))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand(query, sqlConn);
                cmd.Parameters.Add(new SqlParameter("@NombreAdjunto", string.Concat(LOIConstantesLogisticaInversa.NOMBRE_DIGITALIZACION_FACTURAS_GIROS, "-", imagen.ValorDecodificado)));
                cmd.Parameters.Add(new SqlParameter("@IdAdjunto", Guid.NewGuid()));
                cmd.Parameters.Add(new SqlParameter("@IdAdmisionGiro", imagen.IdAdmisionGiro));
                cmd.Parameters.Add(new SqlParameter("@IdGiro ", Convert.ToInt64(imagen.ValorDecodificado)));
                cmd.Parameters.Add(new SqlParameter("@Adjunto", (object)archivoImagen));
                cmd.Parameters.Add(new SqlParameter("@CreadoPor", ControllerContext.Current.Usuario));
                cmd.ExecuteNonQuery();
                sqlConn.Close();
            }
        }

        /// <summary>
        /// Editar un archivo giro
        /// </summary>
        /// <param name="imagen">Objeto imágen</param>
        public void EditarArchivoGiro(LIArchivoGiroDC imagen)
        {
            byte[] archivoImagen;
            string rutaArchivo = Path.Combine(this.filePath, COConstantesModulos.DIGITALIZACION_Y_ARCHIVO, imagen.RutaServidor);
            imagen.RutaServidor = rutaArchivo;
            using (FileStream fs = new FileStream(imagen.RutaServidor, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
                fs.Close();
                archivoImagen = bytes;
            }

            string query = "UPDATE [AlmacenArchivoGiro_LOI] WITH (ROWLOCK)  SET [AAG_Adjunto] = @Adjunto WHERE AAG_IdAdmisionGiro = " + imagen.IdAdmisionGiro.ToString();

            using (SqlConnection sqlConn = new SqlConnection(conexionStringArchivo))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand(query, sqlConn);
                cmd.Parameters.Add(new SqlParameter("@Adjunto", (object)archivoImagen));
                cmd.ExecuteNonQuery();
                sqlConn.Close();
            }
        }

        #endregion Archivo Giro

        #region Archivo Guias Mensajeria

        /// <summary>
        /// Obtiene las guías archivadas
        /// </summary>
        /// <returns>Colección guías archivadas</returns>
        public List<LIArchivoGuiaMensajeriaDC> ObtenerArchivoGuia(long idCol)
        {
            string query = @"SELECT ARG_NumeroGuia, ARG_FechaEntrega,ARG_DatosEdicion,  ARG_DatosEntrega, ARG_EstadoFisicoGuia, ARG_Caja , ARG_Lote, ARG_Posicion FROM dbo.AlmacenGuia_LOI WHERE ARG_CreadoPor = " + ControllerContext.Current.Usuario + " AND " +
                    "w.ARG_Caja != 0 && w.ARG_Lote != 0 && w.ARG_Posicion != 0 && w.ARG_IdCentroLogistico = " + idCol.ToString() + " ORDER BY" +
                    " ARG_Caja,ARG_Lote, ARG_Posicion ";
            string archivo = string.Empty;

            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand(query, sqlConn);

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                sqlConn.Close();
                if (dt.Rows.Count > 0)
                {
                    return
                            dt.AsEnumerable()
                            .ToList()
                            .ConvertAll(r => new LIArchivoGuiaMensajeriaDC()
                            {
                                ValorDecodificado = r["ARG_NumeroGuia"].ToString(),
                                FechaEntrega = Convert.ToDateTime(r["ARG_FechaEntrega"]),
                                EstadoDatosEdicion = new LIEstadoDatosGuiaDC() { IdEstadoDato = r["ARG_DatosEdicion"] == null ? "" : r["ARG_DatosEdicion"].ToString(), Descripcion = ObtenerDescripcionEstadoDatosGuia(r["ARG_DatosEdicion"] == null ? "" : r["ARG_DatosEdicion"].ToString()) },
                                EstadoDatosEntrega = new LIEstadoDatosGuiaDC() { IdEstadoDato = r["ARG_DatosEntrega"] == null ? "" : r["ARG_DatosEntrega"].ToString(), Descripcion = ObtenerDescripcionEstadoDatosGuia(r["ARG_DatosEntrega"] == null ? "" : r["ARG_DatosEntrega"].ToString()) },
                                EstadoFisicoGuia = new LIEstadoFisicoGuiaDC() { IdEstadoFisico = r["ARG_EstadoFisicoGuia"] == null ? "" : r["ARG_EstadoFisicoGuia"].ToString(), Descripcion = ObtenerDescripcionEstadoFisicoGuia(r["ARG_EstadoFisicoGuia"] == null ? "" : r["ARG_EstadoFisicoGuia"].ToString()) },
                                Caja = Convert.ToInt64(r["ARG_Caja"]),
                                Lote = Convert.ToInt32(r["ARG_Lote"]),
                                Posicion = Convert.ToInt32(r["ARG_Posicion"])
                            });
                }
                else
                    return new List<LIArchivoGuiaMensajeriaDC>();
            }
        }

        /// <summary>
        /// Obtiene la descripcion de los estados de los datos de la guía
        /// </summary>
        /// <param name="estadoDatos">Estado</param>
        /// <returns>Descripcíón</returns>
        public string ObtenerDescripcionEstadoDatosGuia(string estadoDatos)
        {
            if (estadoDatos == LOIConstantesLogisticaInversa.ID_ESTADO_LEGIBLE)
                return LIEnumEstadoDatosGuia.LEGIBLE.ToString();
            if (estadoDatos == LOIConstantesLogisticaInversa.ID_ESTADO_ILEGIBLE)
                return LIEnumEstadoDatosGuia.ILEGIBLE.ToString();
            if (estadoDatos == LOIConstantesLogisticaInversa.ID_ESTADO_INCOMPLETO)
                return LIEnumEstadoDatosGuia.INCOMPLETA.ToString();
            else
                return LOIConstantesLogisticaInversa.ID_ESTADO_NO_CLASIFICADO;
        }

        /// <summary>
        /// Obtiene la descripcion de los estados físicos de la guía
        /// </summary>
        /// <param name="estadoDatos">Estado</param>
        /// <returns>Descripcíón</returns>
        public string ObtenerDescripcionEstadoFisicoGuia(string estadoGuia)
        {
            if (estadoGuia == LOIConstantesLogisticaInversa.ID_ESTADO_BUENO)
                return LIEnumEstadoFisicoGuia.BUENO.ToString();
            if (estadoGuia == LOIConstantesLogisticaInversa.ID_ESTADO_REGULAR)
                return LIEnumEstadoFisicoGuia.REGULAR.ToString();
            if (estadoGuia == LOIConstantesLogisticaInversa.ID_ESTADO_MALO)
                return LIEnumEstadoFisicoGuia.MALO.ToString();
            else
                return LOIConstantesLogisticaInversa.ID_ESTADO_NO_CLASIFICADO;
        }

        /// <summary>
        /// Valida si una admisión ya se encuentra archivada
        /// </summary>
        /// <returns>Colección guías archivadas</returns>
        public string ValidarArchivoGuiaExiste(long idAdmision)
        {
            string retorno;
            string query = "SELECT top (1) ARG_RutaArchivo FROM dbo.AlmacenArchivoGuiaFS_LOI WITH (NOLOCK) WHERE ARG_ImagenSincronizada = 1 AND ARG_IdAdminisionMensajeria = " + idAdmision.ToString();
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand(query, sqlConn);

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                if (dt.Rows.Count == 1)
                    retorno = dt.Rows[0]["ARG_RutaArchivo"].ToString();
                else
                    retorno = string.Empty;
                sqlConn.Close();
                return retorno;
            }
        }

        public bool IsManualGuiaEcapture(long numeroGuia)
        {
            bool manual = false;
            string sp = "pa_ValidarEnvioGuiaEcapture_LOI";
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand(sp, sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroGuia", numeroGuia);

                var reader = cmd.ExecuteReader();
                if (reader.Read())
                    manual = true;

                return manual;
            }
        }

        /// <summary>
        /// Valida si un intento de entrega ya se encuentra archivado
        /// </summary>
        /// <returns>Datos evidencia</returns>
        public bool ValidarArchivoIntentoEntregaExistente(LIArchivoGuiaMensajeriaDC guia)
        {
            bool retorno = false;
            string sp = "pa_ValidarArchivoIntentoEntrega_LOI";
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand(sp, sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@AAI_NumeroEvidencia", guia.NumeroEvidencia);

                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    retorno = true;
                }

                return retorno;
            }

        }

        /// <summary>
        /// Adiciona un archivo guía de mensajería
        /// </summary>
        /// <param name="imagen">Objeto imágen</param>
        public void AdicionarArchivoGuiaMensajeria(LIArchivoGuiaMensajeriaDC imagen)
        {
            int valorDefautl = 0;
            SqlConnection sqlConn = new SqlConnection(conexionStringController);
            SqlTransaction objtran = null;

            try
            {
                if (sqlConn.State != ConnectionState.Open)
                    sqlConn.Open();

                objtran = sqlConn.BeginTransaction();


                string cmdTxt = @"SELECT ARG_IdArchivo 
                                  FROM dbo.AlmacenGuia_LOI WITH(NOLOCK)
                                  WHERE ARG_NumeroGuia =  @numeroGuia";

                SqlCommand cmdConfirmar = new SqlCommand(cmdTxt, sqlConn, objtran);
                cmdConfirmar.Parameters.AddWithValue("@numeroGuia", Convert.ToInt64(imagen.ValorDecodificado));
                var idArchivo = cmdConfirmar.ExecuteScalar();

                if (idArchivo == null)
                {

                    SqlCommand cmd = new SqlCommand("paAdicionarAlmacenGuia_LOI", sqlConn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Transaction = objtran;
                    cmd.Parameters.Add(new SqlParameter("@ARG_IdAdminisionMensajeria", imagen.IdAdmisionMensajeria));
                    cmd.Parameters.Add(new SqlParameter("@ARG_NumeroGuia", (imagen.NumeroGuia)));
                    cmd.Parameters.Add(new SqlParameter("@ARG_Caja", valorDefautl));
                    cmd.Parameters.Add(new SqlParameter("@ARG_Lote", valorDefautl));
                    cmd.Parameters.Add(new SqlParameter("@ARG_Posicion", valorDefautl));
                    cmd.Parameters.Add(new SqlParameter("@ARG_FechaEntrega", ConstantesFramework.MinDateTimeController));
                    cmd.Parameters.Add(new SqlParameter("@ARG_CreadoPor", ControllerContext.Current.Usuario));
                    cmd.Parameters.Add(new SqlParameter("@ARG_DatosEdicion", LOIConstantesLogisticaInversa.ID_ESTADO_NO_CLASIFICADO));
                    cmd.Parameters.Add(new SqlParameter("@ARG_DatosEntrega", LOIConstantesLogisticaInversa.ID_ESTADO_NO_CLASIFICADO));
                    cmd.Parameters.Add(new SqlParameter("@ARG_EstadoFisicoGuia", LOIConstantesLogisticaInversa.ID_ESTADO_NO_CLASIFICADO));
                    cmd.Parameters.Add(new SqlParameter("@ARG_FechaArchivo", ConstantesFramework.MinDateTimeController));
                    cmd.Parameters.Add(new SqlParameter("@ARG_Decodificada", imagen.Decodificada));
                    cmd.Parameters.Add(new SqlParameter("@ARG_Manual", imagen.Manual));
                    cmd.Parameters.Add(new SqlParameter("@ARG_UsuarioArchiva", String.Empty));
                    cmd.Parameters.Add(new SqlParameter("@ARG_IdCentroLogArchiva", imagen.IdCol));
                    cmd.Parameters.Add(new SqlParameter("@ARG_CarpetaImagen", true));
                    cmd.ExecuteNonQuery();

                    SqlCommand cmd2 = new SqlCommand("paAdicionarAlmacenArchivoGuia_LOI", sqlConn);
                    cmd2.CommandType = CommandType.StoredProcedure;
                    cmd2.Transaction = objtran;
                    cmd2.Parameters.Add(new SqlParameter("@ARG_IdCentroLogistico ", imagen.IdCentroLogistico));
                    cmd2.Parameters.Add(new SqlParameter("@ARG_IdAdminisionMensajeria", imagen.IdAdmisionMensajeria));
                    cmd2.Parameters.Add(new SqlParameter("@ARG_NumeroGuia", (imagen.NumeroGuia)));
                    cmd2.Parameters.Add(new SqlParameter("@ARG_CreadoPor", ControllerContext.Current.Usuario));
                    cmd2.ExecuteNonQuery();
                    objtran.Commit();
                }

                sqlConn.Close();
            }
            catch (Exception x)
            {
                objtran.Rollback();
                throw x;
            }
            finally
            {
                if (sqlConn.State != ConnectionState.Closed)
                    sqlConn.Close();
                sqlConn.Dispose();
            }
        }

        /// <summary>
        /// Edita un archivo guía de mensajería
        /// </summary>
        /// <param name="imagen">Objeto imágen</param>
        public void EditarArchivoGuiaMensajeria(LIArchivoGuiaMensajeriaDC imagen)
        {
            //if (ObtenerAlmacenGuia(imagen).EsCarpeta)
            //{
            LIArchivoGuiaMensajeriaDC archivoAnterior = ObtenerArchivoGuiaFS(imagen);
            if (archivoAnterior != null)
            {
                if (archivoAnterior.Sincronizada)
                {
                    string rutaServidor = Path.GetDirectoryName(archivoAnterior.RutaServidor);
                    rutaServidor = rutaServidor + "\\" + imagen.NumeroGuia.ToString() + DateTime.Now.Ticks.ToString() + Path.GetExtension(archivoAnterior.RutaServidor);
                    File.Move(archivoAnterior.RutaServidor, rutaServidor);
                    imagen.RutaServidor = rutaServidor;
                }
                else
                    imagen.RutaServidor = string.Empty;

                using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
                {
                    sqlConn.Open();
                    SqlCommand cmd = new SqlCommand("paAlmacenArchivoGuiaFSHist_LOI", sqlConn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ARG_CambiadoPor", ControllerContext.Current.Usuario);
                    cmd.Parameters.AddWithValue("@ARG_RutaArchivo", imagen.RutaServidor);
                    cmd.Parameters.AddWithValue("@ARG_IdCentroLogistico", imagen.IdCentroLogistico);
                    cmd.Parameters.AddWithValue("@ARG_IdAdmisionMensajeria", imagen.IdAdmisionMensajeria);
                    cmd.ExecuteNonQuery();
                    sqlConn.Close();
                }
            }
        }

        /// <summary>
        /// Método para archivar las guias
        /// </summary>
        /// <param name="imagen"></param>
        public void ArchivarGuia(LIArchivoGuiaMensajeriaDC imagen)
        {
            using (EntidadesLogisticaInversa contexto = new EntidadesLogisticaInversa(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                DateTime fechaEntrega = imagen.FechaEntrega.Date.AddHours(imagen.Hora.Hour).AddMinutes(imagen.Hora.Minute).AddSeconds(0);

                contexto.paActualizarArchivoGuia_LOI(imagen.Caja, imagen.Lote, imagen.Posicion, imagen.EstadoDatosEdicion.IdEstadoDato, imagen.EstadoDatosEntrega.IdEstadoDato,
                    imagen.EstadoFisicoGuia.IdEstadoFisico, fechaEntrega, ControllerContext.Current.Usuario, imagen.IdCol, imagen.NumeroGuia);

                //Actualiza entregado en admision
                contexto.paActualizaEntregado_LOI(imagen.IdAdmisionMensajeria, fechaEntrega);
                contexto.SaveChanges();
            }
        }

        public void ArchivarIntentoEntrega(LIArchivoGuiaMensajeriaDC guia)
        {
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("pa_CrearCajaLotePosicionIntentoEntrega_LOI", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@AAI_NumeroEvidencia", guia.NumeroEvidencia);
                cmd.Parameters.AddWithValue("@AAI_Caja", guia.Caja);
                cmd.Parameters.AddWithValue("@AAI_Lote", guia.Lote);
                cmd.Parameters.AddWithValue("@AAI_Posicion", guia.Posicion);
                cmd.Parameters.AddWithValue("@AAI_FechaEntrega", guia.FechaEntrega);
                cmd.Parameters.AddWithValue("@AAI_CreadoPor", guia.CreadoPor);
                cmd.Parameters.AddWithValue("@AAI_DatosEdicion", guia.EstadoDatosEdicion.IdEstadoDato);
                cmd.Parameters.AddWithValue("@AAI_DatosEntrega", guia.EstadoDatosEntrega.IdEstadoDato);
                cmd.Parameters.AddWithValue("@AAI_EstadoFisicoGuia", guia.EstadoFisicoGuia.IdEstadoFisico);
                cmd.Parameters.AddWithValue("@AAI_FechaArchivo", guia.FechaArchivo);
                cmd.Parameters.AddWithValue("@AAI_UsuarioArchiva", guia.UsuarioArchivo);
                cmd.Parameters.AddWithValue("@AAI_Manual", guia.Manual);
                cmd.Parameters.AddWithValue("@AAI_IdCentroLogArchiva", guia.IdCentroLogistico);
                cmd.ExecuteNonQuery();
                sqlConn.Close();
            }
        }


        /// <summary>
        /// Método para validar la fecha de entrega ingresada en el modulo de archivo
        /// </summary>
        /// <param name="fechaEntrega"></param>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public int ValidarFechaArchivo(DateTime fechaEntrega, long numeroGuia)
        {
            using (EntidadesLogisticaInversa contexto = new EntidadesLogisticaInversa(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                paValidarFechaArchivo_LOI_Result respuesta = contexto.paValidarFechaArchivo_LOI(fechaEntrega, numeroGuia).FirstOrDefault();
                return respuesta.Respuesta.Value;
            }
        }

        /// <summary>
        /// Método para obtener un archivo guia
        /// </summary>
        /// <param name="imagen"></param>
        public LIArchivoGuiaMensajeriaDC ObtenerArchivoGuiaFS(LIArchivoGuiaMensajeriaDC imagen)
        {
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerAlmacenArchivoGuiaFSHist_LOI", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ARG_NumeroGuia", imagen.ValorDecodificado);
                cmd.ExecuteNonQuery();
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                sqlConn.Close();
                if (dt.Rows.Count != 0)
                    return new LIArchivoGuiaMensajeriaDC
                    {
                        NumeroGuia = Convert.ToInt64(dt.Rows[0]["ARG_NumeroGuia"]),
                        IdAdmisionMensajeria = Convert.ToInt64(dt.Rows[0]["ARG_IdAdminisionMensajeria"]),
                        RutaServidor = dt.Rows[0]["ARG_RutaArchivo"].ToString(),
                        Sincronizada = Convert.ToBoolean(dt.Rows[0]["ARG_ImagenSincronizada"]),
                    };
                else
                    return null;
            }
        }

        /// <summary>
        /// Método para obtener un archivo guia
        /// </summary>
        /// <param name="imagen"></param>
        public LIArchivoGuiaMensajeriaDC ObtenerArchivoGuiaPruebaEntrega(LIArchivoGuiaMensajeriaDC imagen)
        {
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("pa_ValidarDigitalizacionIntentoEntrega_LOI", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroEvidencia", imagen.NumeroEvidencia);
                cmd.ExecuteNonQuery();
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                sqlConn.Close();
                if (dt.Rows.Count != 0)
                {
                    return new LIArchivoGuiaMensajeriaDC
                    {
                        NumeroGuia = Convert.ToInt64(dt.Rows[0]["ADM_NumeroGuia"]),
                        IdAdmisionMensajeria = Convert.ToInt64(dt.Rows[0]["ADM_IdAdminisionMensajeria"]),
                        RutaServidor = dt.Rows[0]["ARV_RutaArchivo"].ToString(),
                        Sincronizada = Convert.ToBoolean(dt.Rows[0]["VOD_EstaDigitalizado"]),
                    };
                }
                else
                {
                    return null;
                }
            }
        }

        public LIArchivoGuiaMensajeriaDC ObtenerArchivoGuiaSispostal(LIArchivoGuiaMensajeriaDC imagen)
        {
            using (SqlConnection sqlConn = new SqlConnection(CadCnxSispostalController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerAlmacenArchivoGuiaSispostal_TBE", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdGuia", imagen.ValorDecodificado);
                cmd.ExecuteNonQuery();
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                sqlConn.Close();
                if (dt.Rows.Count != 0)
                    return new LIArchivoGuiaMensajeriaDC
                    {
                        NumeroGuia = Convert.ToInt64(dt.Rows[0]["guia"]),
                        RutaServidor = dt.Rows[0]["imagen"].ToString()
                    };
                else
                    return null;
            }
        }






        /// <summary>
        /// Método para obtener imagen de la Fachada para una guia
        /// </summary>
        /// <param name="imagen"></param>
        public List<LIArchivoGuiaMensajeriaFachadaDC> ObtenerArchivoGuiaFachadaFS(long numeroGuia)
        {
            List<LIArchivoGuiaMensajeriaFachadaDC> resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerImagenesEvidenciaEntregaDevolucion_LOI", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IEGT_NumeroGuia", numeroGuia);
                cmd.ExecuteNonQuery();

                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var r = new LIArchivoGuiaMensajeriaFachadaDC
                        {
                            NumeroGuia = Convert.ToInt64(reader["IEGT_NumeroGuia"]),
                            IdentificacionReciboPor = Convert.ToInt64(reader["IEGT_IdentificacionReciboPor"]),
                            RutaServidor = reader["IEGT_RutaImagen"].ToString(),
                            IdCiudad = Convert.ToInt64(reader["IEGT_IdCiudad"]),
                            Ciudad = reader["IEGT_Ciudad"].ToString(),
                            DescripcionEvidencia = reader["IEGT_DescripcionEvidencia"].ToString(),
                        };

                        decimal number = 0;
                        Decimal.TryParse(Convert.ToString(reader["IEGT_Latitud"]), out number);
                        r.Latitud = number;

                        number = 0;
                        Decimal.TryParse(Convert.ToString(reader["IEGT_Longitud"]), out number);
                        r.Longitud = number;

                        if (resultado == null)
                        {
                            resultado = new List<LIArchivoGuiaMensajeriaFachadaDC>();
                        }
                        resultado.Add(r);
                    }
                }
            }
            return resultado;
        }


        /// <summary>
        /// Obtiene la información de la guía en almacen
        /// </summary>
        /// <returns>Colección guías archivadas</returns>
        public LIArchivoGuiaMensajeriaDC ObtenerAlmacenGuia(LIArchivoGuiaMensajeriaDC imagen)
        {
            string query = @"SELECT ARG_CarpetaImagen , ARG_NumeroGuia, ARG_IdAdminisionMensajeria FROM dbo.AlmacenGuia_LOI WHERE ARG_NumeroGuia = " + imagen.ValorDecodificado;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand(query, sqlConn);
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                sqlConn.Close();
                if (dt.Rows.Count != 0)
                    return new LIArchivoGuiaMensajeriaDC
                    {
                        NumeroGuia = Convert.ToInt64(dt.Rows[0]["ARG_NumeroGuia"]),
                        IdAdmisionMensajeria = Convert.ToInt64(dt.Rows[0]["ARG_IdAdminisionMensajeria"]),
                        EsCarpeta = Convert.ToBoolean(dt.Rows[0]["ARG_CarpetaImagen"]),
                    };
                else
                    return null;
            }
        }

        public ADGuia ObtenerFechaEstimadaEntregaGuia(long numeroguia)
        {
            ADGuia guia = new ADGuia();
            guia.NumeroGuia = numeroguia;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("SELECT ADM_IdAdminisionMensajeria, ADM_FechaEstimadaEntrega,ADM_IdServicio, ADM_FechaAdmision,ADM_FechaEntrega,ADM_TipoCliente,ADM_FechaGrabacion FROM AdmisionMensajeria_MEN WITH(NOLOCK) WHERE ADM_NumeroGuia = @numGuia", sqlConn);
                cmd.Parameters.Add(new SqlParameter("numGuia", numeroguia));

                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {

                    guia.IdAdmision = Convert.ToInt64(reader["ADM_IdAdminisionMensajeria"]);

                    string tipoCliente = reader["ADM_TipoCliente"].ToString();
                    guia.TipoCliente = ((ADEnumTipoCliente)Enum.Parse(typeof(ADEnumTipoCliente), tipoCliente));


                    guia.IdServicio = Convert.ToInt32(reader["ADM_IdServicio"]);

                    var fechaAdmision = reader["ADM_FechaAdmision"].ToString();
                    DateTime fechaAdm;
                    DateTime.TryParse(fechaAdmision, out fechaAdm);
                    guia.FechaAdmision = fechaAdm;

                    var fechaEstimadaEntrega = reader["ADM_FechaEstimadaEntrega"].ToString();
                    DateTime fechaEstim;
                    DateTime.TryParse(fechaEstimadaEntrega, out fechaEstim);
                    guia.FechaEstimadaEntrega = fechaEstim;

                    var fechaEntreg = reader["ADM_FechaEntrega"].ToString();
                    DateTime fechaEntrega;
                    DateTime.TryParse(fechaEntreg, out fechaEntrega);
                    guia.FechaEntrega = fechaEntrega;

                    var fechaGraba = reader["ADM_FechaGrabacion"].ToString();
                    DateTime fechaGrabacion;
                    DateTime.TryParse(fechaGraba, out fechaGrabacion);
                    guia.FechaGrabacion = fechaGrabacion;


                }
                sqlConn.Close();
            }
            return guia;
        }

        /// <summary>
        /// Metodo que obtiene los datos de archivo guía de mensajería    
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public LIArchivoGuiaMensajeriaDC ObtenerArchivoGuiaxNumeroGuia(long numeroGuia)
        {
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerArchivoGuia_LOI", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroGuia", numeroGuia);
                cmd.ExecuteNonQuery();
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                sqlConn.Close();
                if (dt.Rows.Count != 0)
                    return new LIArchivoGuiaMensajeriaDC
                    {
                        NumeroGuia = Convert.ToInt64(dt.Rows[0]["ARG_NumeroGuia"]),
                        IdAdmisionMensajeria = Convert.ToInt64(dt.Rows[0]["ARG_IdAdminisionMensajeria"]),
                        RutaServidor = dt.Rows[0]["ARG_RutaArchivo"].ToString(),
                        Sincronizada = Convert.ToBoolean(dt.Rows[0]["ARG_ImagenSincronizada"]),
                        Caja = Convert.ToInt64(dt.Rows[0]["ARG_Caja"]),
                        Lote = Convert.ToInt32(dt.Rows[0]["ARG_Lote"]),
                        Posicion = Convert.ToInt32(dt.Rows[0]["ARG_Posicion"]),
                        IdCol = Convert.ToInt64(dt.Rows[0]["ARG_IdCentroLogArchiva"]),
                        UsuarioArchivo = dt.Rows[0]["ARG_UsuarioArchiva"].ToString(),
                    };

                else
                    return null;
            }
        }

        #endregion Archivo Guias Mensajeria

        #region Archivo Comprobante Pago Giro

        /// <summary>
        /// Conulta un archivo de comprobante de pago
        /// </summary>
        /// <param name="imagen"></param>
        /// <returns></returns>
        public bool ConsultarArchivoComprobantePagoGuia(LIArchivoComprobantePagoDC imagen)
        {

            bool respuesta;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerImagenArchivosComprobanteGiro_APG", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdAdmisionGiro", imagen.IdAdmisionGiro));
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                if (dt.Rows.Count == 0)
                    respuesta = false;
                else
                    respuesta = true;
                sqlConn.Close();
                return respuesta;
            }
        }


        /// <summary>
        /// Conulta un archivo de comprobante de pago
        /// </summary>
        /// <param name="imagen"></param>
        /// <returns></returns>
        public bool ConsultarArchivoComprobantePago(long idAdmisionGiro)
        {

            bool respuesta;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerImagenArchivosComprobanteGiro_APG", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdAdmisionGiro", idAdmisionGiro));
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                if (dt.Rows.Count == 0)
                    respuesta = false;
                else
                    respuesta = true;
                sqlConn.Close();
                return respuesta;
            }
        }

        /// <summary>
        /// Adiciona un archivo comprobante pago
        /// </summary>
        /// <param name="imagen">Imagen Imagen</param>
        public void AdicionarArchivoComprobantePago(LIArchivoComprobantePagoDC imagen)
        {
            filePath = PAAdministrador.Instancia.ObtenerParametrosAplicacion(PAAdministrador.RutaArchivos);
            string rutaArchivo = Path.Combine(this.filePath, COConstantesModulos.DIGITALIZACION_Y_ARCHIVO, imagen.RutaServidor);
            Image imagenArchivo;
            using (FileStream fs = File.OpenRead(rutaArchivo))
            {
                imagenArchivo = Image.FromStream(fs);
                fs.Close();
            }
            string rutaImagenes = Framework.Servidor.ParametrosFW.PAParametros.Instancia.ConsultarParametrosFramework("FlComprobanteGiros");
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
                SqlCommand cmd = new SqlCommand("paAdicionarImagenArchivosComprobanteGiro_APG", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@NombreAdjunto ", string.Concat(LOIConstantesLogisticaInversa.NOMBRE_DIGITALIZACION_PAGOS_GIROS, "-", imagen.ValorDecodificado)));
                cmd.Parameters.Add(new SqlParameter("@IdComprobantePago", Convert.ToInt64(imagen.ValorDecodificado)));
                cmd.Parameters.Add(new SqlParameter("@RutaAdjunto", ruta));
                cmd.Parameters.Add(new SqlParameter("@IdAdjunto", Guid.NewGuid()));
                cmd.Parameters.Add(new SqlParameter("@IdGiro", imagen.NumeroGiro ?? 0));
                cmd.Parameters.Add(new SqlParameter("@IdAdmisionGiro", imagen.IdAdmisionGiro ?? 0));
                cmd.Parameters.Add(new SqlParameter("@CreadoPor", ControllerContext.Current.Usuario));
                cmd.Parameters.Add(new SqlParameter("@Decodificada", imagen.Decodificada));
                cmd.Parameters.Add(new SqlParameter("@Manual", imagen.Manual));
                cmd.ExecuteNonQuery();
                sqlConn.Close();
            }
        }

        /// <summary>
        /// Edita un archivo comprobante pago
        /// </summary>
        /// <param name="imagen">Objeto Imagen</param>
        public void EditarArchivoComprobantePago(LIArchivoComprobantePagoDC imagen)
        {
            string creadoporAct = "";
            DateTime fechaAsignacionAct = DateTime.Now;

            if (!ConsultarArchivoComprobantePagoGuia(imagen))
            {
                ControllerException excepcion = new ControllerException(COConstantesModulos.DIGITALIZACION_Y_ARCHIVO, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                throw new FaultException<ControllerException>(excepcion);
            }

            byte[] archivoImagen;
            string rutaArchivo = Path.Combine(this.filePath, COConstantesModulos.DIGITALIZACION_Y_ARCHIVO, imagen.RutaServidor);
            imagen.RutaServidor = rutaArchivo;
            using (FileStream fs = new FileStream(imagen.RutaServidor, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
                fs.Close();
                archivoImagen = bytes;
            }

            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerImagenArchivosComprobanteGiro_APG", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@IdAdmisionGiro", imagen.IdAdmisionGiro));
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    creadoporAct = Convert.ToString(reader["APG_CreadoPor"]);
                    fechaAsignacionAct = Convert.ToDateTime(reader["APG_FechaGrabacion"]);
                }
                sqlConn.Close();
            }

            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paAdicionarAlmacenArchivoPagoGiroHist_APG", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@NombreAdjunto ", string.Concat(LOIConstantesLogisticaInversa.NOMBRE_DIGITALIZACION_PAGOS_GIROS, "-", imagen.ValorDecodificado)));
                cmd.Parameters.Add(new SqlParameter("@IdComprobantePago", Convert.ToInt64(imagen.ValorDecodificado)));
                cmd.Parameters.Add(new SqlParameter("@RutaAdjunto", rutaArchivo));
                cmd.Parameters.Add(new SqlParameter("@IdAdjunto", Guid.NewGuid()));
                cmd.Parameters.Add(new SqlParameter("@IdGiro", imagen.NumeroGiro ?? 0));
                cmd.Parameters.Add(new SqlParameter("@IdAdmisionGiro", imagen.IdAdmisionGiro ?? 0));
                cmd.Parameters.Add(new SqlParameter("@CreadoPor", creadoporAct));
                cmd.Parameters.Add(new SqlParameter("@FechaGrabacion", fechaAsignacionAct));
                cmd.Parameters.Add(new SqlParameter("@Decodificada", imagen.Decodificada));
                cmd.Parameters.Add(new SqlParameter("@Manual", imagen.Manual));
                cmd.Parameters.Add(new SqlParameter("@CambiadoPor", ControllerContext.Current.Usuario));
                cmd.ExecuteNonQuery();
                sqlConn.Close();
            }

            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paActualizarAlmacenArchivoPagoGiro_APG", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@RutaAdjunto", rutaArchivo));
                cmd.Parameters.Add(new SqlParameter("@IdAdjunto", Guid.NewGuid()));
                cmd.Parameters.Add(new SqlParameter("@IdAdmisionGiro", imagen.IdAdmisionGiro));
                cmd.ExecuteNonQuery();
                sqlConn.Close();
            }
        }

        #endregion Archivo Comprobante Pago Giro

        #region EvidenciaDevolucion

        /// <summary>
        /// Retorna el stream de un archivo de envidencia de devolución dado su id
        /// </summary>
        /// <param name="idArchivo"></param>
        /// <returns></returns>
        public string ObtenerArchivoEvidenciaAdjunto(long idArchivo)
        {
            ControllerException excepcion;
            string respuesta;
            string query = "SELECT * FROM dbo.ArchivoEvidencia_MEN WHERE ARV_IdArchivoEvidencia = " + idArchivo;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringArchivo))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand(query, sqlConn);
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                if (dt.Rows.Count == 0)
                {
                    excepcion = new ControllerException(COConstantesModulos.TELEMERCADEO, LOIEnumTipoErrorLogisticaInversa.EX_ARCHIVO_NO_ENCONTRADO.ToString(), LOIMensajesLogisticaInversa.CargarMensaje(LOIEnumTipoErrorLogisticaInversa.EX_ARCHIVO_NO_ENCONTRADO));
                    throw new FaultException<ControllerException>(excepcion);
                }
                else
                    respuesta = Convert.ToBase64String(dt.Rows[0]["ARV_Adjunto"] as byte[]);

                sqlConn.Close();
                return respuesta;
            }
        }

        #endregion EvidenciaDevolucion

        #region GeneracionImagenes

        /// <summary>
        /// Retorna las guias y la ruta de las imagenes
        /// </summary>
        /// <param name="idArchivo"></param>
        /// <returns></returns>
        public List<LIArchivoGuiaMensajeriaDC> ObtenerGuiasRuta(string imagenesGenerar, int idCliente, string idCiudad, int idSucursal, DateTime fechaAdmisionInical, DateTime fechaAdmisionFinal, long guiaFacturaInicial, long guiaFacturaFinal, long ordenCompraInicial, long ordenCompraFinal)
        {
            // TODO: LMSA Verificar manejo de fecha en procedimiento almacenado
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                fechaAdmisionInical = fechaAdmisionInical.Date;
                fechaAdmisionFinal = fechaAdmisionFinal.Date;
                SqlCommand cmd = new SqlCommand("paObtenerGuiasRutaImagenes", sqlConn);
                cmd.CommandTimeout = 36000;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("imagenesGenerar", imagenesGenerar);
                cmd.Parameters.AddWithValue("idCliente", idCliente);
                cmd.Parameters.AddWithValue("idCiudad", idCiudad);
                cmd.Parameters.AddWithValue("idSucursal", idSucursal);
                cmd.Parameters.AddWithValue("fechaAdmisionInical", fechaAdmisionInical);
                cmd.Parameters.AddWithValue("fechaAdmisionFinal", fechaAdmisionFinal);
                cmd.Parameters.AddWithValue("guiaFacturaInicial", guiaFacturaInicial);
                cmd.Parameters.AddWithValue("guiaFacturaFinal", guiaFacturaFinal);
                cmd.Parameters.AddWithValue("ordenCompraInicial", ordenCompraInicial);
                cmd.Parameters.AddWithValue("ordenCompraFinal", ordenCompraFinal);

                cmd.ExecuteNonQuery();
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                sqlConn.Close();
                if (dt.Rows.Count > 0)
                {
                    return
                            dt.AsEnumerable()
                            .ToList()
                            .ConvertAll(r => new LIArchivoGuiaMensajeriaDC()
                            {
                                NumeroGuia = Convert.ToInt64(r["ADM_NumeroGuia"]),
                                NombreRemitente = r["ADM_NombreRemitente"].ToString(),
                                CiudadOrigen = r["ADM_NombreCiudadOrigen"].ToString(),
                                TelefonoRemitente = r["ADM_TelefonoRemitente"].ToString(),
                                DireccionRemitente = r["ADM_DireccionRemitente"].ToString(),
                                Peso = r["ADM_Peso"].ToString(),
                                ValorAdmision = r["ADM_ValorAdmision"].ToString(),
                                ValorPrimaSeguro = r["ADM_ValorPrimaSeguro"].ToString(),
                                ValorTotal = r["ADM_ValorTotal"].ToString(),
                                NombreDestinatario = r["ADM_NombreDestinatario"].ToString(),
                                DireccionDestinatario = r["ADM_DireccionDestinatario"].ToString(),
                                TelefonoDestinatario = r["ADM_TelefonoDestinatario"].ToString(),
                                DiceContener = r["ADM_DiceContener"].ToString(),
                                CiudadDestinatario = r["ADM_NombreCiudadDestino"].ToString(),
                                FechaEntrega = Convert.ToDateTime(r["ADM_FechaEntrega"]),
                                Observaciones = r["ADM_Observaciones"].ToString(),
                                DescripcionEstado = r["EGT_DescripcionEstado"].ToString(),
                                RutaServidor = r["ARG_RutaArchivo"].ToString(),
                                EstadoDatosEdicion = new LIEstadoDatosGuiaDC(),
                                EstadoDatosEntrega = new LIEstadoDatosGuiaDC(),
                                EstadoFisicoGuia = new LIEstadoFisicoGuiaDC(),
                                ResultadoEscaner = ADEnumResultadoEscaner.Exitosa,
                            });
                }
                else
                    return new List<LIArchivoGuiaMensajeriaDC>();
            }
        }

        #endregion GeneracionImagenes
    }
}