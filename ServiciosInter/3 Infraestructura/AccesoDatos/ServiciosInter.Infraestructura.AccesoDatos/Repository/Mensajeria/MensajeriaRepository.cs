using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.ServiceModel;
using ServiciosInter.DatosCompartidos.Comun;
using ServiciosInter.DatosCompartidos.EntidadesNegocio.Clientes;
using ServiciosInter.DatosCompartidos.EntidadesNegocio.LogisticaInversa;
using ServiciosInter.DatosCompartidos.EntidadesNegocio.Mensajeria;
using ServiciosInter.DatosCompartidos.EntidadesNegocio.OperacionNacional;
using ServiciosInter.DatosCompartidos.EntidadesNegocio.Parametros;
using ServiciosInter.DatosCompartidos.EntidadesNegocio.ParametrosOperacion;
using ServiciosInter.DatosCompartidos.EntidadesNegocio.Tarifas;
using ServiciosInter.Infraestructura.AccesoDatos.Repository.Mapper;
using ServiciosInter.DatosCompartidos.EntidadesNegocio.Comun;
using WPF.Comun.EntidadesNegocio.ServiciosInter;

namespace ServiciosInter.Infraestructura.AccesoDatos.Repository.Mensajeria
{
    public class MensajeriaRepository
    {
        private string CadCnxController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;
        private string CadCnxSispostalController = ConfigurationManager.ConnectionStrings["Sispostal"].ConnectionString;

        private static readonly MensajeriaRepository instancia = new MensajeriaRepository();

        public static MensajeriaRepository Instancia
        {
            get
            {
                return instancia;
            }
        }

        /// <summary>
        /// Obtiene traza de un envio sispostal
        /// </summary>
        /// <param name="idGuia"></param>
        /// <returns></returns>
        public ADTrazaGuia ObtenerTrazaUltimoEstadoXNumGuiaSispostal(long idGuia)
        {
            ADTrazaGuia traza = new ADTrazaGuia();
            using (SqlConnection cnn = new SqlConnection(CadCnxSispostalController))
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerUltimoEstadoGuiaSispostalxNumero_TBE", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdGuia", idGuia);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    traza.NumeroGuia = reader["Guia"] == DBNull.Value ? 0 : Convert.ToInt64(reader["Guia"]);
                    traza.IdEstadoGuia = reader["IdEstado"] == DBNull.Value ? Convert.ToInt16(0) : Convert.ToInt16(reader["IdEstado"]);
                    traza.DescripcionEstadoGuia = reader["Estado"] == DBNull.Value ? string.Empty : reader["Estado"].ToString();
                    traza.IdCiudad = reader["IdLocalidad"] == DBNull.Value ? string.Empty : reader["IdLocalidad"].ToString();
                    traza.Ciudad = reader["NombreLocalidad"] == DBNull.Value ? string.Empty : reader["NombreLocalidad"].ToString();
                    traza.FechaAdmisionGuia = Convert.ToDateTime(reader["Fecha"]);
                    traza.FechaGrabacion = Convert.ToDateTime(reader["Fecha"]);
                    traza.FechaEntrega = reader["FechEntre"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["FechEntre"]);
                    traza.IdOrigenApliacion = 0;
                    traza.ClaseEstado = reader["ClaseEstado"] == DBNull.Value ? string.Empty : reader["ClaseEstado"].ToString();
                }
            }
            return traza;
        }

        /// <summary>
        /// Obtiene traza de un envio sispostal consulta que viene desde el portal
        /// </summary>
        /// <param name="idGuia"></param>
        /// <returns></returns>
        public ADTrazaGuiaClienteRespuesta ObtenerTrazaUltimoEstadoPorNumGuiaSispostaldelPortal(string idGuia)
        {
            ADTrazaGuiaClienteRespuesta traza = new ADTrazaGuiaClienteRespuesta();
            using (SqlConnection cnn = new SqlConnection(CadCnxSispostalController))
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerUltimoEstadoGuiaSispostalxNumero_TBE", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdGuia", idGuia);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    traza.IdEstadoGuia = reader["IdEstado"] == DBNull.Value ? Convert.ToInt16(0) : Convert.ToInt16(reader["IdEstado"]);
                    traza.DescripcionEstadoGuia = reader["Estado"] == DBNull.Value ? string.Empty : reader["Estado"].ToString();
                    traza.FechaGrabacion = Convert.ToDateTime(reader["Fecha"]);
                }
            }
            return traza;
        }

        /// <summary>
        /// Obtiene guia sispostal por numero de guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
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
                    guia = MensajeriaMapper.ToListGuiaSispostal(reader);
                }
                else
                    throw new Exception("Número de guía no existe.");
                /*throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MENSAJERIA, ADEnumTipoErrorMensajeria.EX_NUMERO_GUIA_NO_EXISTE.ToString(),
              ADMensajesAdmisiones.CargarMensaje(ADEnumTipoErrorMensajeria.EX_NUMERO_GUIA_NO_EXISTE)));
        guia.TrazaGuiaEstado = EstadosGuia.ObtenerTrazaUltimoEstadoGuia(guia.IdAdmision);*/

                return guia;
            }
        }

        /// <summary>
        /// Obtiene guia sispostal por numero de guia consultada desde el portal cuando no pertenece.
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ADGuiaClienteRespuesta ObtenerGuiaSispostalXNumeroGuiaPorPortal(string numeroGuia,bool EncriptaAes=false)
        {
            using (SqlConnection conn = new SqlConnection(CadCnxSispostalController))
            {
                conn.Open();

                SqlCommand comm = new SqlCommand("paObtenerGuiaSispostalFormaPago_TBE", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("@IdGuia", numeroGuia);
                SqlDataReader reader = comm.ExecuteReader();

                ADGuiaClienteRespuesta guia = new ADGuiaClienteRespuesta();

                if (reader.HasRows)
                {
                    guia = MensajeriaMapper.ToListGuiaSispostalPorPortal(reader);
                }
                else
                    throw new Exception("Número de guía no existe.");
                
                return guia;
            }
        }

        /// <summary>
        /// Obtiene guia sispostal por numero de guia consultada desde el portal cuando pertenece.
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ADGuiaClienteRespuesta ObtenerGuiaSispostalPorNumeroGuiaPertenencia(string numeroGuia, bool EncriptaAes = false)
        {
            using (SqlConnection conn = new SqlConnection(CadCnxSispostalController))
            {
                conn.Open();

                SqlCommand comm = new SqlCommand("paObtenerGuiaSispostalFormaPago_TBE", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("@IdGuia", numeroGuia);
                SqlDataReader reader = comm.ExecuteReader();

                ADGuiaClienteRespuesta guia = new ADGuiaClienteRespuesta();

                if (reader.HasRows)
                {
                    guia = MensajeriaMapper.ToListGuiaSispostalPertenencia(reader, EncriptaAes);
                }
                else
                    throw new Exception("Número de guía no existe.");

                return guia;
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
        /// Método para obtener un archivo guia
        /// </summary>
        /// <param name="imagen"></param>
        public LIArchivoGuiaMensajeriaDC ObtenerArchivoGuiaFS(LIArchivoGuiaMensajeriaDC imagen)
        {
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
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
        /// Obtiene los estados  de una guia en una localidad
        /// </summary>
        /// <returns></returns>
        public List<ADTrazaGuia> ObtenerEstadosGuia(long numeroGuia)
        {
            List<ADTrazaGuia> listaestados = new List<ADTrazaGuia>();
            using (SqlConnection cnn = new SqlConnection(CadCnxController))
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerEstadosGuias_MEN", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroGuia", numeroGuia);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    listaestados.Add(new ADTrazaGuia()
                    {
                        Ciudad = reader["EGT_NombreLocalidad"].ToString(),
                        IdAdmision = Convert.ToInt64(reader["EGT_IdAdminisionMensajeria"]),
                        IdCiudad = reader["EGT_IdLocalidad"].ToString(),
                        IdEstadoGuia = Convert.ToInt16(reader["EGT_IdEstadoGuia"]),
                        DescripcionEstadoGuia = reader["EGT_DescripcionEstado"].ToString(),
                        Modulo = reader["EGT_IdModulo"].ToString(),
                        NumeroGuia = Convert.ToInt64(reader["EGT_NumeroGuia"]),
                        Observaciones = reader["EGT_Observacion"].ToString(),
                        FechaGrabacion = Convert.ToDateTime(reader["EGT_FechaGrabacion"]),
                        IdTrazaGuia = Convert.ToInt64(reader["EGT_IdEstadoGuiaLog"]),
                        FechaEntrega = Convert.ToDateTime(@"1900/01/01"),
                        Usuario = reader["EGT_CreadoPor"].ToString(),
                    });
                }
            }

            return listaestados;
        }

        /// <summary>
        /// Obtiene la traza del ultimo estado de la guia x numero de guia
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <returns></returns>
        public ADTrazaGuia ObtenerTrazaUltimoEstadoXNumGuia(long numeroGuia)
        {
            ADTrazaGuia traza = new ADTrazaGuia();
            using (SqlConnection cnn = new SqlConnection(CadCnxController))
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerUltimoEstadoGuiaxNumero_MEN", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@numeroGuia", numeroGuia);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    traza.IdAdmision = (Convert.ToInt64(reader["EGT_IdAdminisionMensajeria"]));
                    traza.NumeroGuia = (Convert.ToInt64(reader["EGT_NumeroGuia"]));
                    traza.IdEstadoGuia = (Convert.ToInt16(reader["EGT_IdEstadoGuia"]));
                    traza.DescripcionEstadoGuia = reader["EGT_DescripcionEstado"].ToString();
                    traza.Observaciones = reader["EGT_Observacion"].ToString();
                    traza.IdCiudad = reader["EGT_IdLocalidad"].ToString();
                    traza.Ciudad = reader["EGT_NombreLocalidad"].ToString();
                    traza.IdCentroServicioEstado = (Convert.ToInt64(reader["EGT_IdCentroServicio"]));
                    traza.NombreCentroServicioEstado = reader["EGT_NombreCentroServicio"].ToString();
                    traza.FechaAdmisionGuia = (Convert.ToDateTime(reader["ADM_FechaAdmision"]));
                    traza.FechaGrabacion = (Convert.ToDateTime(reader["EGT_FechaGrabacion"]));
                    traza.FechaEntrega = (Convert.ToDateTime(reader["ADM_FechaEntrega"]));
                    //traza.IdEstadoGuiaLog = (Convert.ToInt64(reader["EGT_IdEstadoGuiaLog"]));
                }
            }
            return traza;
        }

        /// <summary>
        /// Método para obtener el estado de la guía si fue entregado o devolución ratificada
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public bool ObtenerTrazaEstadoGuiaIVR(long numeroGuia)
        {
            List<ADTrazaGuia> listaEstadosTrazaGuia = new List<ADTrazaGuia>();
            using (SqlConnection cnn = new SqlConnection(CadCnxController))
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerTrazaGuiaEstado_IVR_MEN", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroGuia", numeroGuia);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    ADTrazaGuia estadoTraza = new ADTrazaGuia
                    {
                        NumeroGuia = (Convert.ToInt64(reader["EGT_NumeroGuia"])),
                        IdEstadoGuia = (Convert.ToInt16(reader["EGT_IdEstadoGuia"])),
                        DescripcionEstadoGuia = reader["EGT_DescripcionEstado"].ToString(),
                    };
                    listaEstadosTrazaGuia.Add(estadoTraza);
                }
            }
            return listaEstadosTrazaGuia.Any(e => e.IdEstadoGuia == (short)ADEnumEstadoGuia.Entregada);
        }

        /// <summary>
        /// Obtiene la traza del ultimo estado de la guia x numero de guia consultada desde el portal
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <returns></returns>
        public ADTrazaGuiaClienteRespuesta ObtenerTrazaUltimoEstadoPorNumGuiaPorPortal(string numeroGuia)
        {
            ADTrazaGuiaClienteRespuesta traza = new ADTrazaGuiaClienteRespuesta();
            using (SqlConnection cnn = new SqlConnection(CadCnxController))
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerUltimoEstadoGuiaxNumeroRastreoGuias_MEN", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@numeroGuia", numeroGuia);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    traza.IdEstadoGuia = (Convert.ToInt16(reader["EGT_IdEstadoGuia"]));
                    traza.DescripcionEstadoGuia = reader["EGT_DescripcionEstado"].ToString();
                    traza.FechaGrabacion = (Convert.ToDateTime(reader["EGT_FechaGrabacion"]));
                }
            }
            return traza;
        }

        /// <summary>
        /// Obtiene los Estados y Motivos de la Guia seleccionada
        /// </summary>
        /// <returns></returns>
        public List<ADEstadoGuiaMotivoDC> ObtenerEstadosMotivosGuia(long numeroGuia)
        {
            List<ADEstadoGuiaMotivoDC> listaestadosmotivos = new List<ADEstadoGuiaMotivoDC>();
            using (SqlConnection cnn = new SqlConnection(CadCnxController))
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerEstadosMotivosGuias_MEN", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroGuia", numeroGuia);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    ADEstadoGuiaMotivoDC estado = new ADEstadoGuiaMotivoDC
                    {
                        EstadoGuia = new ADTrazaGuia
                        {
                            IdEstadoGuia = Convert.ToInt16(reader["EGT_IdEstadoGuia"]),
                            Ciudad = reader["EGT_NombreLocalidad"].ToString(),
                            DescripcionEstadoGuia = reader["EGT_DescripcionEstado"].ToString(),
                            FechaGrabacion = Convert.ToDateTime(reader["EGT_FechaGrabacion"]),
                            Latitud = reader["EGT_Latitud"] == DBNull.Value ? string.Empty : reader["EGT_Latitud"].ToString(),
                            Longitud = reader["EGT_Longitud"] == DBNull.Value ? string.Empty : reader["EGT_Longitud"].ToString(),
                        }
                    };

                    if (reader["EGM_IdMotivoGuia"] != DBNull.Value)
                    {
                        estado.Motivo = new ADMotivoGuiaDC
                        {
                            IdMotivoGuia = Convert.ToInt16(reader["EGM_IdMotivoGuia"]),
                            Descripcion = reader["MOG_Descripcion"] == DBNull.Value ? string.Empty : reader["MOG_Descripcion"].ToString()
                        };
                        estado.FechaMotivo = reader["EGM_FechaMotivo"] == DBNull.Value ? Convert.ToDateTime(reader["EGT_FechaGrabacion"]) : Convert.ToDateTime(reader["EGM_FechaMotivo"]);
                    }
                    else
                    {
                        estado.Motivo = new ADMotivoGuiaDC();
                    }

                    listaestadosmotivos.Add(estado);
                }
            }
            return listaestadosmotivos;
        }

        /// <summary>
        /// Obtiene los Estados y Motivos de la Guia seleccionada consultada por el portal.
        /// </summary>
        /// <returns></returns>
        public List<ADEstadoGuiaMotivoClienteRespuesta> ObtenerEstadosMotivosGuiaPorPortal(string numeroGuia)
        {
            List<ADEstadoGuiaMotivoClienteRespuesta> listaestadosmotivos = new List<ADEstadoGuiaMotivoClienteRespuesta>();
            using (SqlConnection cnn = new SqlConnection(CadCnxController))
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerEstadosMotivosGuiasRastreoGuias_MEN", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroGuia", numeroGuia);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    ADEstadoGuiaMotivoClienteRespuesta estado = new ADEstadoGuiaMotivoClienteRespuesta
                    {
                        EstadoGuia = new ADTrazaGuiaEstadoGuia
                        {
                            IdEstadoGuia = Convert.ToInt16(reader["EGT_IdEstadoGuia"]),
                            DescripcionEstadoGuia = reader["EGT_DescripcionEstado"].ToString(),
                            Ciudad = reader["EGT_NombreLocalidad"].ToString(),
                            FechaGrabacion = Convert.ToDateTime(reader["EGT_FechaGrabacion"])
                        }
                    };

                    listaestadosmotivos.Add(estado);
                }
            }
            return listaestadosmotivos;
        }

        /// <summary>
        /// Obtiene los Estados de la Guia seleccionada consultada por el portal.
        /// </summary>
        /// <returns></returns>
        public List<ADEstadoGuiaClienteRespuesta> ObtenerEstadosGuiaPorPortal(string numeroGuia)
        {
            List<ADEstadoGuiaClienteRespuesta> listaestadosmotivos = new List<ADEstadoGuiaClienteRespuesta>();
            using (SqlConnection cnn = new SqlConnection(CadCnxController))
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerEstadosMotivosGuiasRastreoGuias_MEN", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroGuia", numeroGuia);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    ADEstadoGuiaClienteRespuesta estado = new ADEstadoGuiaClienteRespuesta
                    {
                        EstadoGuia = new ADTrazaGuiaEstadoGuiaDetallado
                        {
                            IdEstadoGuia = Convert.ToInt16(reader["EGT_IdEstadoGuia"]),
                            DescripcionEstadoGuia = reader["EGT_DescripcionEstado"].ToString(),
                            Ciudad = reader["EGT_NombreLocalidad"].ToString(),
                            FechaGrabacion = Convert.ToDateTime(reader["EGT_FechaGrabacion"]),
                            IdLocalidadEnCurso = reader["EGT_IdLocalidad"].ToString(),
                            IdLocalidadOrigen = reader["ADM_IdCiudadOrigen"].ToString(),
                            IdLocalidadDestino = reader["ADM_IdCiudadDestino"].ToString()
                        }
                    };

                    listaestadosmotivos.Add(estado);
                }
            }
            return listaestadosmotivos;
        }

        /// <summary>
        /// Metodo para obtener la ultima posicion mensajero por numero de guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public POUbicacionMensajero ObtenerUltimaPosicionMensajeroPorNumeroGuia(long numeroGuia)
        {
            POUbicacionMensajero poUbicacionMensajero = null;
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerUltimaPosicionMensajeroPorNumeroGuia_CPO", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroGuia", numeroGuia);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    poUbicacionMensajero = new POUbicacionMensajero();
                    poUbicacionMensajero.Latitud = reader["UPM_Latitud"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["UPM_Latitud"]);
                    poUbicacionMensajero.Longitud = reader["UPM_Longitud"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["UPM_Longitud"]);
                }
            }
            return poUbicacionMensajero;
        }

        /// <summary>
        /// Obtiene toda la informacion de una guia a partir del numero de guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ADGuia ObtenerGuiaXNumeroGuia(long numeroGuia)
        {
            using (SqlConnection conn = new SqlConnection(CadCnxController))
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
                {

                    // throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NoContent) { Content = new StringContent("El número de guía no existe") });
                    throw new FaultException("El número de guía no existe");

                    //throw new FaultException<Exception>(new Exception("El número de guía no existe"));
                    /*HttpResponseException exc = new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NoContent));
                    exc.Response.Content = new StringContent("El número de guía no existe");

                    throw exc;*/

                }

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
                    FechaEstimadaEntregaNew = adm["ADM_FechaEstimadaEntregaNew"] == DBNull.Value ? Convert.ToDateTime(adm["ADM_FechaEntrega"]) : Convert.ToDateTime(adm["ADM_FechaEstimadaEntregaNew"]),
                    FormasPagoDescripcion = Convert.ToString(adm["FOP_Descripcion"]),
                    EsAutomatico = Convert.ToBoolean(adm["ADM_EsAutomatico"])
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

                guia.TrazaGuiaEstado = ObtenerTrazaUltimoEstadoGuia(guia.IdAdmision);

                return guia;
            }
        }

        /// <summary>
        /// Obtiene toda la informacion de una guia a partir del numero de guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ADGuiaClienteRespuesta ObtenerGuiaPorNumeroGuiaPorPortal(string numeroGuia, bool EncriptaAes = false)
        {
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand comm = new SqlCommand("paObtenerAdmisionMensajeriaRastreoGuias_MEN", conn);
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
                {
                    throw new FaultException("El número de guía no existe");
                }
                List<CLClienteContadoClienteRespuesta> remitenteDestinatario = ObtenerDatosRemitenteDestinatarioProtegidos(numeroGuia, EncriptaAes);
                ADGuiaClienteRespuesta guia = new ADGuiaClienteRespuesta()
                {
                    Remitente = new CLClienteContadoClienteRespuesta()
                    {
                        Nombre = null,
                        Telefono = remitenteDestinatario[0].Telefono,
                        Identificacion = remitenteDestinatario[0].Identificacion,
                        Direccion = null,
                        TipoId = null
                    },
                    Destinatario = new CLClienteContadoClienteRespuesta()
                    {
                        Nombre = null,
                        Telefono = remitenteDestinatario[1].Telefono,
                        Identificacion = remitenteDestinatario[1].Identificacion,
                        Direccion = null,
                        TipoId = null
                    },
                    NumeroGuia = Convert.ToInt64(adm["ADM_NumeroGuia"]),
                    NombreCiudadOrigen = Convert.ToString(adm["ADM_NombreCiudadOrigen"]),
                    NombreCiudadDestino = Convert.ToString(adm["ADM_NombreCiudadDestino"]),
                    FechaEstimadaEntregaNew = adm["ADM_FechaEstimadaEntregaNew"] == DBNull.Value ? Convert.ToDateTime(adm["ADM_FechaEntrega"]) : Convert.ToDateTime(adm["ADM_FechaEstimadaEntregaNew"]),
                    FechaAdmision = Convert.ToDateTime(adm["ADM_FechaAdmision"]),
                    NombreTipoEnvio = Convert.ToString(adm["ADM_NombreTipoEnvio"]),
                    TotalPiezas = Convert.ToInt16(adm["ADM_TotalPiezas"]),
                    Peso = Convert.ToDecimal(adm["ADM_Peso"]),
                    PesoLiqVolumetrico = Convert.ToDecimal(adm["ADM_PesoLiqVolumetrico"]),
                    NumeroBolsaSeguridad = Convert.ToString(adm["ADM_NumeroBolsaSeguridad"]),
                    DiceContener = Convert.ToString(adm["ADM_DiceContener"]),
                    Observaciones = Convert.ToString(adm["ADM_Observaciones"]),
                    ValorTotal = Convert.ToDecimal(adm["ADM_ValorTotal"]),
                    ValorDeclarado = Convert.ToDecimal(adm["ADM_ValorDeclarado"]),
                    IdServicio = Convert.ToInt32(adm["ADM_IdServicio"]),
                    NombreServicio = Convert.ToString(adm["ADM_NombreServicio"]),
                    NumeroPieza= Convert.ToInt16(adm["ADM_NumeroPieza"])
                };

                long IdAdmision = Convert.ToInt64(adm["ADM_IdAdminisionMensajeria"]);

                guia.FormasPago = lstAdm.Where(a => a.Field<long>("ADM_IdAdminisionMensajeria") == IdAdmision).ToList().ConvertAll<ADGuiaFormaPagoClienteRespuesta>(f =>
                new ADGuiaFormaPagoClienteRespuesta
                {
                    IdFormaPago = Convert.ToInt16(f["FOP_IdFormaPago"]),
                    Descripcion = Convert.ToString(f["FOP_Descripcion"]),
                });

                if (guia.FormasPago == null)
                {
                    guia.FormasPago = new List<ADGuiaFormaPagoClienteRespuesta>
                    {
                        new ADGuiaFormaPagoClienteRespuesta { }
                    };
                }

                return guia;
            }
        }

        /// <summary>
        /// Obtiene toda la informacion de una guia a partir del numero de guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ADGuiaClienteRespuesta ObtenerGuiaPorNumeroGuiaPertenencia(string numeroGuia, bool EncriptaAes = false)
        {
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand comm = new SqlCommand("paObtenerAdmisionMensajeriaRastreoGuias_MEN", conn);
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
                {
                    throw new FaultException("El número de guía no existe");
                }
                List<CLClienteContadoClienteRespuesta> remitenteDestinatario = ObtenerDatosRemitenteDestinatarioDesProtegidos(numeroGuia, EncriptaAes);
                ADGuiaClienteRespuesta guia = new ADGuiaClienteRespuesta()
                {
                    Remitente = new CLClienteContadoClienteRespuesta()
                    {
                        Nombre = remitenteDestinatario[0].Nombre,
                        Telefono = remitenteDestinatario[0].Telefono,
                        Identificacion = remitenteDestinatario[0].Identificacion,
                        Direccion = remitenteDestinatario[0].Direccion,
                        TipoId = remitenteDestinatario[0].TipoId,
                        Email = remitenteDestinatario[0].Email
                    },
                    Destinatario = new CLClienteContadoClienteRespuesta()
                    {
                        Nombre = remitenteDestinatario[1].Nombre,
                        Direccion = remitenteDestinatario[1].Direccion,
                        TipoId = remitenteDestinatario[1].TipoId,
                        Telefono = remitenteDestinatario[1].Telefono,
                        Identificacion = remitenteDestinatario[1].Identificacion,
                        Email = remitenteDestinatario[1].Email
                    },
                    NumeroGuia = Convert.ToInt64(adm["ADM_NumeroGuia"]),
                    NombreCiudadOrigen = Convert.ToString(adm["ADM_NombreCiudadOrigen"]),
                    NombreCiudadDestino = Convert.ToString(adm["ADM_NombreCiudadDestino"]),
                    FechaEstimadaEntregaNew = adm["ADM_FechaEstimadaEntregaNew"] == DBNull.Value ? Convert.ToDateTime(adm["ADM_FechaEntrega"]) : Convert.ToDateTime(adm["ADM_FechaEstimadaEntregaNew"]),
                    FechaAdmision = Convert.ToDateTime(adm["ADM_FechaAdmision"]),
                    NombreTipoEnvio = Convert.ToString(adm["ADM_NombreTipoEnvio"]),
                    TotalPiezas = Convert.ToInt16(adm["ADM_TotalPiezas"]),
                    Peso = Convert.ToDecimal(adm["ADM_Peso"]),
                    PesoLiqVolumetrico = Convert.ToDecimal(adm["ADM_PesoLiqVolumetrico"]),
                    NumeroBolsaSeguridad = Convert.ToString(adm["ADM_NumeroBolsaSeguridad"]),
                    DiceContener = Convert.ToString(adm["ADM_DiceContener"]),
                    Observaciones = Convert.ToString(adm["ADM_Observaciones"]),
                    ValorTotal = Convert.ToDecimal(adm["ADM_ValorTotal"]),
                    ValorDeclarado = Convert.ToDecimal(adm["ADM_ValorDeclarado"]),
                    IdServicio = Convert.ToInt32(adm["ADM_IdServicio"]),
                    NombreServicio = Convert.ToString(adm["ADM_NombreServicio"]),
                    NumeroPieza = Convert.ToInt16(adm["ADM_NumeroPieza"])
                   
                };

                long IdAdmision = Convert.ToInt64(adm["ADM_IdAdminisionMensajeria"]);

                guia.FormasPago = lstAdm.Where(a => a.Field<long>("ADM_IdAdminisionMensajeria") == IdAdmision).ToList().ConvertAll<ADGuiaFormaPagoClienteRespuesta>(f =>
                new ADGuiaFormaPagoClienteRespuesta
                {
                    IdFormaPago = Convert.ToInt16(f["FOP_IdFormaPago"]),
                    Descripcion = Convert.ToString(f["FOP_Descripcion"]),
                });

                if (guia.FormasPago == null)
                {
                    guia.FormasPago = new List<ADGuiaFormaPagoClienteRespuesta>
                    {
                        new ADGuiaFormaPagoClienteRespuesta { }
                    };
                }

                return guia;
            }
        }

        /// <summary>
        /// Obtiene la informacion de pertenencia de la guia a partir del numero de guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ADGuiaPertenencia ObtenerPertenenciaGuiaPorNumeroGuia(string numeroGuia)
        {
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand comm = new SqlCommand("usp_ConsultarDatosAdmisionMensajeria", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("@NumeroGuia", numeroGuia);
                SqlDataAdapter da = new SqlDataAdapter(comm);
                DataTable dt = new DataTable();
                conn.Open();
                da.Fill(dt);
                conn.Close();

                var lstAdm = dt.AsEnumerable().ToList();
                var adm = lstAdm.GroupBy(r => r.Field<string>("ADM_IdDestinatario")).Select(s => s.First()).ToList().FirstOrDefault();

                if (adm == null)
                {
                    throw new FaultException("El número de guía no existe");
                }
                ADGuiaPertenencia guiaPertenencia = new ADGuiaPertenencia()
                {
                    NumeroGuia = numeroGuia,
                    IdentificacionRemitente = Convert.ToString(adm["ADM_IdRemitente"]),
                    TelefonoRemitente = Convert.ToString(adm["ADM_TelefonoRemitente"]),
                    IdentificacionDestinatario = Convert.ToString(adm["ADM_IdDestinatario"]),
                    TelefonoDestinatario = Convert.ToString(adm["ADM_TelefonoDestinatario"])
                };

                return guiaPertenencia;
            }
        }

        /// <summary>
        /// Obtiene la informacion protegida de una guia a partir del numero de guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<CLClienteContadoClienteRespuesta> ObtenerDatosRemitenteDestinatarioProtegidos(string numeroGuia, bool EncriptaAes = false)
        {
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand comm = new SqlCommand("usp_ConsultarDatosProtegidosAdmisionMensajeria", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("@NumeroGuia", numeroGuia);
                SqlDataAdapter da = new SqlDataAdapter(comm);
                DataTable dt = new DataTable();
                conn.Open();
                da.Fill(dt);
                conn.Close();

                var lstAdm = dt.AsEnumerable().ToList();
                var adm = lstAdm.GroupBy(r => r.Field<string>("ADM_IdRemitente")).Select(s => s.First()).ToList().FirstOrDefault();

                if (adm == null)
                {
                    throw new FaultException("El número de guía no existe");
                }
                if (!EncriptaAes)
                {
                    List<CLClienteContadoClienteRespuesta> lstGuia = new List<CLClienteContadoClienteRespuesta>();
                    lstGuia.Add(new CLClienteContadoClienteRespuesta()
                    {
                        Telefono = Cifrado.Encrypt(adm["ADM_TelefonoRemitente"].ToString()),
                        Identificacion = Cifrado.Encrypt(adm["ADM_IdRemitente"].ToString())
                    });
                    lstGuia.Add(new CLClienteContadoClienteRespuesta()
                    {
                        Telefono = Cifrado.Encrypt(adm["ADM_TelefonoDestinatario"].ToString()),
                        Identificacion = Cifrado.Encrypt(adm["ADM_IdDestinatario"].ToString()),
                    });
                    return lstGuia;
                }
                else
                {
                    List<CLClienteContadoClienteRespuesta> lstGuia = new List<CLClienteContadoClienteRespuesta>();
                    lstGuia.Add(new CLClienteContadoClienteRespuesta()
                    {
                        Telefono = Cifrado.EncriptarTexto(adm["ADM_TelefonoRemitente"].ToString()),
                        Identificacion = Cifrado.EncriptarTexto(adm["ADM_IdRemitente"].ToString())
                    });
                    lstGuia.Add(new CLClienteContadoClienteRespuesta()
                    {
                        Telefono = Cifrado.EncriptarTexto(adm["ADM_TelefonoDestinatario"].ToString()),
                        Identificacion = Cifrado.EncriptarTexto(adm["ADM_IdDestinatario"].ToString()),
                    });
                    return lstGuia;
                }

                
            }
        }

        /// <summary>
        /// Obtiene la informacion desprotegida de una guia a partir del numero de guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<CLClienteContadoClienteRespuesta> ObtenerDatosRemitenteDestinatarioDesProtegidos(string numeroGuia, bool EncriptaAes=false)
        {
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand comm = new SqlCommand("usp_ConsultarDatosDesprotegidosAdmisionMensajeria", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("@NumeroGuia", numeroGuia);
                SqlDataAdapter da = new SqlDataAdapter(comm);
                DataTable dt = new DataTable();
                conn.Open();
                da.Fill(dt);
                conn.Close();

                var lstAdm = dt.AsEnumerable().ToList();
                var adm = lstAdm.GroupBy(r => r.Field<string>("ADM_IdRemitente")).Select(s => s.First()).ToList().FirstOrDefault();

                if (adm == null)
                {
                    throw new FaultException("El número de guía no existe");
                }

                if (!EncriptaAes)
                {

                    List<CLClienteContadoClienteRespuesta> lstGuia = new List<CLClienteContadoClienteRespuesta>();
                    lstGuia.Add(new CLClienteContadoClienteRespuesta()
                    {
                        Nombre = Cifrado.Encrypt(adm["ADM_NombreRemitente"].ToString()),
                        Telefono = Cifrado.Encrypt(adm["ADM_TelefonoRemitente"].ToString()),
                        Identificacion = Cifrado.Encrypt(adm["ADM_IdRemitente"].ToString()),
                        Direccion = Cifrado.Encrypt(adm["ADM_DireccionRemitente"].ToString()),
                        TipoId = Cifrado.Encrypt(adm["ADM_IdTipoIdentificacionRemitente"].ToString()),
                        Email = Cifrado.Encrypt(adm["ADM_EmailRemitente"].ToString()),
                    });
                    lstGuia.Add(new CLClienteContadoClienteRespuesta()
                    {
                        Nombre = Cifrado.Encrypt(adm["ADM_NombreDestinatario"].ToString()),
                        Telefono = Cifrado.Encrypt(adm["ADM_TelefonoDestinatario"].ToString()),
                        Identificacion = Cifrado.Encrypt(adm["ADM_IdDestinatario"].ToString()),
                        Direccion = Cifrado.Encrypt(adm["ADM_DireccionDestinatario"].ToString()),
                        TipoId = Cifrado.Encrypt(adm["ADM_IdTipoIdentificacionDestinatario"].ToString()),
                        Email = Cifrado.Encrypt(adm["ADM_EmailDestinatario"].ToString())
                    });
                    return lstGuia;

                }
                else
                {
                    List<CLClienteContadoClienteRespuesta> lstGuia = new List<CLClienteContadoClienteRespuesta>();
                    lstGuia.Add(new CLClienteContadoClienteRespuesta()
                    {
                        Nombre = Cifrado.EncriptarTexto(adm["ADM_NombreRemitente"].ToString()),
                        Telefono = Cifrado.EncriptarTexto(adm["ADM_TelefonoRemitente"].ToString()),
                        Identificacion = Cifrado.EncriptarTexto(adm["ADM_IdRemitente"].ToString()),
                        Direccion = Cifrado.EncriptarTexto(adm["ADM_DireccionRemitente"].ToString()),
                        TipoId = Cifrado.EncriptarTexto(adm["ADM_IdTipoIdentificacionRemitente"].ToString()),
                        Email = Cifrado.EncriptarTexto(adm["ADM_EmailRemitente"].ToString()),
                    });
                    lstGuia.Add(new CLClienteContadoClienteRespuesta()
                    {
                        Nombre = Cifrado.EncriptarTexto(adm["ADM_NombreDestinatario"].ToString()),
                        Telefono = Cifrado.EncriptarTexto(adm["ADM_TelefonoDestinatario"].ToString()),
                        Identificacion = Cifrado.EncriptarTexto(adm["ADM_IdDestinatario"].ToString()),
                        Direccion = Cifrado.EncriptarTexto(adm["ADM_DireccionDestinatario"].ToString()),
                        TipoId = Cifrado.EncriptarTexto(adm["ADM_IdTipoIdentificacionDestinatario"].ToString()),
                        Email = Cifrado.EncriptarTexto(adm["ADM_EmailDestinatario"].ToString())
                    });
                    return lstGuia;
                }
               
            }
        }

        /// <summary>
        /// Obtiene la traza del ultimo estado de la guia
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <returns></returns>
        public ADTrazaGuia ObtenerTrazaUltimoEstadoGuia(long idAdmision)
        {
            ADTrazaGuia traza = new ADTrazaGuia();
            using (SqlConnection cnn = new SqlConnection(CadCnxController))
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerUltimoEstadoGuia_MEN", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idAdmisionMensajeria", idAdmision);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    traza.IdAdmision = (Convert.ToInt64(reader["EGT_IdAdminisionMensajeria"]));
                    traza.NumeroGuia = (Convert.ToInt64(reader["EGT_NumeroGuia"]));
                    traza.IdEstadoGuia = (Convert.ToInt16(reader["EGT_IdEstadoGuia"]));
                    traza.DescripcionEstadoGuia = reader["EGT_DescripcionEstado"].ToString();
                    traza.Observaciones = reader["EGT_Observacion"].ToString();
                    traza.IdCiudad = reader["EGT_IdLocalidad"].ToString();
                    traza.Ciudad = reader["EGT_NombreLocalidad"].ToString();
                    traza.IdCentroServicioEstado = (Convert.ToInt64(reader["EGT_IdCentroServicio"]));
                    traza.NombreCentroServicioEstado = reader["EGT_NombreCentroServicio"].ToString();
                    traza.FechaAdmisionGuia = reader["ADM_FechaAdmision"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["ADM_FechaAdmision"]);
                    traza.FechaGrabacion = reader["EGT_FechaGrabacion"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["EGT_FechaGrabacion"]);
                    traza.FechaEntrega = reader["EGT_FechaGrabacion"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["EGT_FechaGrabacion"]);
                    traza.Usuario = reader["EGT_CreadoPor"] == DBNull.Value ? string.Empty : Convert.ToString(reader["EGT_CreadoPor"]);
                }
            }
            return traza;
        }

        /// <summary>
        /// obtiene estado gestión guia
        /// </summary>
        /// <param name="NumeroDeGuia"></param>
        /// <returns></returns>
        public long obtieneEstadoGestionGuia(long NumeroDeGuia)
        {
            long guia = 0;

            using (SqlConnection sqlconn = new SqlConnection(CadCnxController))
            {
                sqlconn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerEstadoGestionGuia_MEN", sqlconn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 500000;
                cmd.Parameters.AddWithValue("@NumeroGuia", NumeroDeGuia);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    guia = reader["PGG_NumeroGuiaNueva"] == DBNull.Value ? 0 : Convert.ToInt64(reader["PGG_NumeroGuiaNueva"].ToString());
                }

                return guia;
            }
        }

        /// <summary>
        /// Permite obtener el listado de imagenes de evidencias
        /// </summary>
        /// <param name="guia">Número de guia filtro</param>
        /// <param name="IdTipoEvidencia">Ide de tipo evidencia filtro</param>
        /// <returns>numero de guia, ruta imagen evidencia</returns>
        public LIImagenesDevolucion ObtenerRutaArchivosGuia(long NumeroGuia)
        {
            LIImagenesDevolucion listaRutas = null;
            using (SqlConnection cnn = new SqlConnection(CadCnxController))
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerRutaDevoluciones_LOI", cnn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@NumeroGuia", NumeroGuia);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    listaRutas = new LIImagenesDevolucion();
                    listaRutas.ListaImagenEvidencia = new List<LIImagenEvidencia>();
                    while (reader.Read())
                    {
                        listaRutas.NumeroGuia = Convert.ToInt64(reader["IEGT_NumeroGuia"]);
                        listaRutas.ListaImagenEvidencia.Add(new LIImagenEvidencia()
                        {
                            Ruta = reader["IEGT_RutaImagen"].ToString(),
                        });
                    }
                }
            }
            return listaRutas;
        }
        ///// <summary>
        ///// Metodo para adicionar Telemercadeo de la guia
        ///// </summary>
        ///// <param name="ObtenerInformacionTelemercadeoGuia"></param>
        ///// <returns></returns>
        public List<LIGestionesDC> ObtenerInformacionTelemercadeoGuia(long numeroGuia)
        {
            List<LIGestionesDC> Lista = new List<LIGestionesDC>();

            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerInformacionTelemercadeoGuia_LOI", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@NumeroGuia", numeroGuia));
                SqlDataReader read = cmd.ExecuteReader();

                while (read.Read())
                {
                    LIGestionesDC newObjGestionDC = new LIGestionesDC
                    {
                        NumeroGuia = Convert.ToInt64(read["GGT_NumeroGuia"]),
                        IdGestion = Convert.ToInt64(read["GGT_IdGestionGuiaTelemercadeo"]),
                        Telefono = read["GGT_TelefonoMarcado"].ToString(),
                        PersonaContesta = read["GGT_PersonaContesta"].ToString(),
                        FechaGestion = Convert.ToDateTime(read["GGT_FechaGrabacion"]),
                        Observaciones = read["GGT_Observacion"].ToString(),
                        NuevaDireccion = read["GGT_NuevaDireccionEnvio"].ToString()
                        /* Resultado = new LIResultadoTelemercadeoDC()
                         {
                             Descripcion = read["EGT_DescripcionEstado"].ToString(),  //Tele
                             Estado = read["RTM_Estado"].ToString(), //act
                             Ciudad = read["NombreCompleto"].ToString()
                         }*/
                    };
                    Lista.Add(newObjGestionDC);
                }
            }

            return Lista;
        }

        /// <summary>
        /// Consulta pruebas de entrega.  Realizado por Mauricio Sanchez 20160205
        /// </summary>
        /// <param name="numeroGuia">objeto de tipo archivo</param>
        public List<string> ObtenerVolantesGuia(long numeroGuia)
        {
            List<string> lstimagenes = new List<string>();

            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
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
                        //ruta = ruta.Replace(@"\\ReporController","Z:");
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
                using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
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

        ///// <summary>
        ///// Metodo para Obtener informacion de las Novedades de Transportede de  la guia seleccionada
        ///// </summary>
        ///// <param name="OObtenerNovedadesTransporteGuia"></param>
        ///// <returns></returns>
        public List<ONNovedadesTransporteDC> ObtenerNovedadesTransporteGuia(long numeroGuia)
        {
            List<ONNovedadesTransporteDC> Lista = new List<ONNovedadesTransporteDC>();

            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerNovedadesRutaGuia_OPN", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@NumeroGuia", numeroGuia));
                SqlDataReader read = cmd.ExecuteReader();

                while (read.Read())
                {
                    ONNovedadesTransporteDC newObjNovedadesTransporteDC = new ONNovedadesTransporteDC
                    {
                        IdManifiestoOperacionNacio = Convert.ToInt64(read["NER_IdManifiestoOperacionNacio"]),
                        NombreNovedad = read["NombreNovedad"].ToString(),
                        LugarIncidente = read["NombreCiudad"].ToString(),
                        Descripcion = read["NER_Observaciones"].ToString(),
                        FechaNovedad = Convert.ToDateTime(read["NER_FechaNovedad"]),
                        Tiempo = read["Tiempo"].ToString(),
                        FechaEstimadaEntrega = Convert.ToDateTime(read["ADM_FechaEstimadaEntregaNew"]),
                    };
                    Lista.Add(newObjNovedadesTransporteDC);
                }
            }

            return Lista;
        }

        /// <summary>
        /// Método para obtener la información de una guía rapiradicado
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public long ObtenerAdmisionRapiradicado(long numeroGuia, bool consultaEstado)
        {
            long numeroGuiaInterna = 0;

            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerAdmRapiradicado_MEN", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 600;
                cmd.Parameters.AddWithValue("@numeroGuia", numeroGuia);
                sqlConn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    if (reader.Read())
                    {

                        numeroGuiaInterna = reader["ARR_NumeroGuiaInterna"] == DBNull.Value ? 0 : Convert.ToInt64(reader["ARR_NumeroGuiaInterna"]);                        
                        
                    }

                }
            }
            return numeroGuiaInterna;
        }

        public Decimal ObtenerPorcentajeDesceuntoCentroServicio(long idCentroServicio, int idServicio)
        {
            decimal descuento = 0;
            using (SqlConnection sqlconn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerPorcentajeDescuentoCentroServicio", sqlconn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdCentroServicio", idCentroServicio);
                cmd.Parameters.AddWithValue("@IdServicio", idServicio);
                sqlconn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        descuento = reader["PorcentajeDescuento"] != DBNull.Value ? Convert.ToDecimal(reader["PorcentajeDescuento"].ToString()) : 0;
                    }
                }
            }
            return descuento;
        }


        /// <summary>
        /// verifica si una guia existe
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public bool VerificarSiGuiaExiste(long numeroGuia)
        {

            using (SqlConnection conn = new SqlConnection(CadCnxController))
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
        /// Obtiene los mensajes para el estado 2
        /// </summary>
        /// <returns></returns>
        public List<MensajeEstadoDos> ObtenerMensajesEstadoDos()
        {
            List<MensajeEstadoDos> mensajes = new List<MensajeEstadoDos>();

            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerMensajesEstadoDos", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader read = cmd.ExecuteReader();

                while (read.Read())
                {
                    MensajeEstadoDos mensaje = new MensajeEstadoDos
                    {
                        IdMensaje = Convert.ToInt32(read["MCA_Id"]),
                        Mensaje = read["MCA_Mensaje"].ToString()
                    };
                    mensajes.Add(mensaje);
                }
            }

            return mensajes;
        }

        #region COTIZADOR

        /// <summary>
        /// Retorna la lista de tipos de entrega que pueden aplicar sobre el envío
        /// </summary>
        /// <returns></returns>
        public List<ADTipoEntrega> ObtenerTiposEntrega()
        {
            List<ADTipoEntrega> Lista = new List<ADTipoEntrega>();

            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerTiposdeEntrega_MEN", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader read = cmd.ExecuteReader();

                while (read.Read())
                {
                    ADTipoEntrega newObjNovedadesTransporteDC = new ADTipoEntrega
                    {
                        Id = read["TIE_IdTipoEntrega"].ToString().Trim(),
                        Descripcion = read["TIE_Descripcion"].ToString()
                    };
                    Lista.Add(newObjNovedadesTransporteDC);
                }
            }

            return Lista.OrderBy(o => o.Descripcion).ToList();
        }

        /// <summary>
        /// Consulta los servicios con los pesos minimos y maximos
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<TAServicioPesoDC> ConsultarServiciosPesosMinimoxMaximos()
        {
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand(@"paConsultarServiciosConPesosMinimosMaximos_TAR", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                sqlConn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                List<TAServicioPesoDC> servicios = new List<TAServicioPesoDC>();

                while (reader.Read())
                {
                    TAServicioPesoDC servicio = new TAServicioPesoDC();

                    servicio.IdServicio = Convert.ToInt32(reader["SER_IdServicio"]);
                    servicio.PesoMaximo = Convert.ToDecimal(reader["SME_PesoMaximo"]);
                    servicio.PesoMinimo = Convert.ToDecimal(reader["SME_PesoMinimo"]);

                    servicios.Insert(0, servicio);
                }
                sqlConn.Close();
                return servicios;
            }
        }

        /// <summary>
        /// Obtiene el porcentaje de la prima seguro por servicio con la lista de precios vigente
        /// </summary>
        /// <param name="idServicio">Identificador del servicio a consultar</param>
        /// /// <param name="idListaPrecio">Identificador de lista de precios</param>
        /// <returns>retorna porcentaje de la prima seguro del servicio</returns>
        public decimal ObtenerPorcentajePrimaSeguro(int idServicio, int idListaPrecio)
        {
            decimal porcentajeServicio = 0;

            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {

                SqlCommand cmd = new SqlCommand(@"paObtenerPrecioSeguroServicio", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdServicio", idServicio);
                cmd.Parameters.AddWithValue("@IdListaPrecio", idListaPrecio);
                sqlConn.Open();
                porcentajeServicio = Convert.ToDecimal(cmd.ExecuteScalar());
                sqlConn.Close();
                return porcentajeServicio;
            }
        }


        /// <summary>
        /// Obtiene los servicios con los pesos minimos y maximos por el identificador de la lista de precios
        /// </summary>
        /// <param name="idListaPrecio">Identificador de lista de precios</param>
        /// <returns>retorna lista de servicios con valores minimos y maximos</returns>
        public List<TAServicioPesoDC> ConsultarServiciosPesosMinimoxMaximosPorListaPrecio(int idListaPrecio)
        {
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand(@"paConsultarServiciosConPesosMinimosMaximosListaPrecios_TAR", sqlConn);
                cmd.Parameters.AddWithValue("@IdListaPrecios", idListaPrecio);
                cmd.CommandType = CommandType.StoredProcedure;
                sqlConn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                List<TAServicioPesoDC> servicios = new List<TAServicioPesoDC>();

                while (reader.Read())
                {
                    TAServicioPesoDC servicio = new TAServicioPesoDC();

                    servicio.IdServicio = Convert.ToInt32(reader["SER_IdServicio"]);
                    servicio.PesoMaximo = Convert.ToDecimal(reader["SME_PesoMaximo"]);
                    servicio.PesoMinimo = Convert.ToDecimal(reader["SME_PesoMinimo"]);

                    servicios.Insert(0, servicio);
                }
                sqlConn.Close();
                return servicios;
            }
        }

        /// <summary>
        /// Obtiene el servicio asignado con su respectivo peso minimo y maximo parametrizado
        /// </summary>
        /// <param name="idListaPrecio"></param>
        /// <returns>Un objeto con la información de precio minimo y maximo</returns>
        public TAServicioPesoDC ConsultarServicioPesosMinimoxMaximosPorListaPrecioCredito(int idListaPrecio, int idServicio, decimal peso)
        {
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                TAServicioPesoDC servicio = new TAServicioPesoDC();
                SqlCommand cmd = new SqlCommand("paConsultarServicioConPesosMinimosMaximosListaPreciosCredito_TAR", sqlConn);
                cmd.Parameters.AddWithValue("@IdListaPrecios", idListaPrecio);
                cmd.Parameters.AddWithValue("@IdServicio", idServicio);
                cmd.Parameters.AddWithValue("@Peso", peso);
                cmd.CommandType = CommandType.StoredProcedure;
                sqlConn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows) {
                    reader.Read();
                    servicio.IdServicio = Convert.ToInt32(reader["SER_IdServicio"]);
                    servicio.PesoMaximo = Convert.ToDecimal(reader["SME_PesoMaximo"]);
                    servicio.PesoMinimo = Convert.ToDecimal(reader["SME_PesoMinimo"]);
                    sqlConn.Close();
                }
                else
                {
                    throw new Exception("No hay servicios asociados que cumpla con los datos ingresados.");
                }
                return servicio;
            }
        }

            /// <summary>
            /// obtiene los valores minimos y maximos declarados al cliente validando según peso y lista de precios configurada
            /// </summary>
            /// param name="peso">peso cotizado</param>
            /// <param name="idListaPrecio">Identificador de lista de precios</param>
            /// <returns>Valor minimo  y maximo declarado</returns>
            public TAValorPesoDeclaradoDC ObtenerValorPesoDeclarado(int idListaPrecio, decimal peso)
        {
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerValorPesoDeclarado_Cli", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdListaPrecios", idListaPrecio);
                cmd.Parameters.AddWithValue("@Peso", peso);
                sqlConn.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    return new TAValorPesoDeclaradoDC()
                    {
                        IdValorPesoDeclarado = Convert.ToInt16(reader["IdValorPesoDeclarado"]),
                        IdListaPrecio = Convert.ToInt16(reader["IdListaPrecio"]),
                        PesoInicial = Convert.ToInt64(reader["PesoInicial"]),
                        PesoFinal = Convert.ToInt64(reader["PesoFinal"]),
                        ValorMinimoDeclarado = Convert.ToInt64(reader["ValorMinimoDeclarado"]),
                        ValorMaximoDeclarado = Convert.ToInt64(reader["ValorMaximoDeclarado"]),
                    };
                }
                else
                {
                    return new TAValorPesoDeclaradoDC();
                }
            }
        }

        /// <summary>
        /// Metodo encargado de devolver el id de la lista de precios vigente
        /// </summary>
        /// <returns>int con el id de la lista de precio</returns>
        public int ObtenerIdListaPrecioVigente()
        {
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {

                SqlCommand cmd = new SqlCommand(@"paObtenerIdListaPreciosVigente_TAR", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                sqlConn.Open();
                int valor = Convert.ToInt32(cmd.ExecuteScalar());
                sqlConn.Close();
                return valor;
            }
        }

        /// <summary>
        /// Consulta si una localidad tiene centros de servicio que acepten pago en casa (Contrapago)
        /// </summary>
        /// <param name="idLocalidad">Id de la localidad que se va a consultar</param>
        /// <returns>Bool, existe o no existe CS con pago en casa para esa localidad</returns>
        public bool LocalidadTieneCSConPagoEnCasa(string idLocalidad)
        {
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {

                SqlCommand cmd = new SqlCommand(@"paLocalidadTieneCSConPagoEnCasa_PAR", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdLocalidad", idLocalidad);
                sqlConn.Open();
                bool valor = Convert.ToBoolean(cmd.ExecuteScalar());
                sqlConn.Close();
                return valor;
            }
        }

        /// <summary>
        /// Consulta los parámetros de configuración de admisiones
        /// </summary>
        /// <returns></returns>
        public string ObtenerParametrosAdmisiones(string parametro)
        {
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerParametrosAdmision_MEN", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@parametro", parametro);
                conn.Open();
                string rst = cmd.ExecuteScalar().ToString();
                conn.Close();
                return rst;
            }
        }

        /// <summary>
        /// Retorna todo el listado de casilleros establecidos por trayecto
        /// </summary>
        /// <returns></returns>
        public HashSet<ADRangoTrayecto> ObtenerCasillerosTrayectos()
        {
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerCasillerosTrayectos_OPN", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                var casilleros = dt.AsEnumerable().ToList().GroupBy(cas => new { TRC_IdLocalidadDestino = cas.Field<string>("TRC_IdLocalidadDestino"), TRC_IdLocalidadOrigen = cas.Field<string>("TRC_IdLocalidadOrigen") })
                  .ToList()
                  .ConvertAll<ADRangoTrayecto>(r =>
                    new ADRangoTrayecto
                    {
                        IdLocalidadDestino = r.First().Field<string>("TRC_IdLocalidadDestino"),
                        IdLocalidadOrigen = r.First().Field<string>("TRC_IdLocalidadOrigen"),
                        Rangos = r.ToList().ConvertAll(rango =>
                          new ADRangoCasillero
                          {
                              Casillero = rango.Field<string>("TCP_IdCasillero"),
                              RangoFinal = rango.Field<decimal>("RPC_RangoFinal"),
                              RangoInicial = rango.Field<decimal>("RPC_RangoInicial")
                          })
                    });

                conn.Close();
                return new HashSet<ADRangoTrayecto>(casilleros);
            }
        }

        /// <summary>
        /// Retorna el id del operador postal de la localidad dada
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <returns></returns>
        public PAOperadorPostal ObtenerOperadorPostalLocalidad(string idLocalidad)
        {
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerOperadorPostalLocalidad_PAR", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdLocalidad", idLocalidad);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                PAOperadorPostal opPostal = new PAOperadorPostal();
                while (reader.Read())
                {
                    opPostal.Id = Convert.ToInt32(reader["OPZ_IdOperadorPostal"]);
                    opPostal.IdZona = reader["IdZona"].ToString();
                    opPostal.TiempoEntrega = Convert.ToInt32(reader["OPZ_TiempoEntrega"]);
                }

                conn.Close();
                return opPostal;
            }
        }

        /// <summary>
        /// Retorna el valor de mensajeria
        /// </summary>
        /// <returns></returns>
        public TAPrecioMensajeriaDC ObtenerPrecioMensajeria(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision, string idTipoEntrega = "-1")
        {
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                TAPrecioMensajeriaDC precio = new TAPrecioMensajeriaDC();

                SqlCommand cmd = new SqlCommand("paPrecioTrayecto_TAR", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdServicio", idServicio);
                cmd.Parameters.AddWithValue("@IdLocalidadOrigen", idLocalidadOrigen);
                cmd.Parameters.AddWithValue("@IdLocalidadDestino", idLocalidadDestino);
                cmd.Parameters.AddWithValue("@IdListaPrecios", idListaPrecio);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dtPrecioT = new DataTable();
                conn.Open();
                da.Fill(dtPrecioT);
                // conn.Close();

                var precioTrayecto = dtPrecioT.AsEnumerable().ToList();

                ///Obtiene las excepciones del trayecto
                cmd = new SqlCommand("paObtenerExTrayecto_TAR", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdServicio", idServicio);
                cmd.Parameters.AddWithValue("@IdLocalidadOrigen", idLocalidadOrigen);
                cmd.Parameters.AddWithValue("@IdLocalidadDestino", idLocalidadDestino);
                cmd.Parameters.AddWithValue("@IdListaPrecio", idListaPrecio);
                SqlDataAdapter daExc = new SqlDataAdapter(cmd);
                DataTable dtExc = new DataTable();
                // conn.Open();
                daExc.Fill(dtExc);
                // conn.Close();

                var excepciones = dtExc.AsEnumerable().ToList().FirstOrDefault();

                if (precioTrayecto.Count() == 0)
                    if (excepciones != null)
                    {
                        DataRow dr = dtPrecioT.NewRow();
                        dr.SetField<string>("TRS_IdTipoSubTrayecto", TAConstantesTarifas.ID_TIPO_SUBTRAYECTO_KILO_INICIAL);
                        precioTrayecto.Add(dr);
                    }
                    else
                    {
                        throw new FaultException<Exception>(new Exception("No existe precio para el trayecto."));
                        //throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.TARIFAS, TAEnumTipoErrorTarifas.EX_NO_EXISTE_PRECIO_PARA_TRAYECTO.ToString(), TAMensajesTarifas.CargarMensaje(TAEnumTipoErrorTarifas.EX_NO_EXISTE_PRECIO_PARA_TRAYECTO)));
                    }

                if ((precioTrayecto.Where(pt => pt.Field<string>("TRS_IdTipoSubTrayecto") == TAConstantesTarifas.ID_TIPO_SUBTRAYECTO_KILO_INICIAL).Count()) == 0)
                {
                    throw new FaultException<Exception>(new Exception("No hay kilo inicial configurado."));
                    //throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.TARIFAS, TAEnumTipoErrorTarifas.EX_KILO_INICIAL_NO_CONFIGURADO.ToString(), TAMensajesTarifas.CargarMensaje(TAEnumTipoErrorTarifas.EX_KILO_INICIAL_NO_CONFIGURADO)));
                }

                precio.ValorKiloInicial = precioTrayecto.Where(pt => pt.Field<string>("TRS_IdTipoSubTrayecto") == TAConstantesTarifas.ID_TIPO_SUBTRAYECTO_KILO_INICIAL)
                  .FirstOrDefault()
                  .Field<decimal>("PTR_ValorFijo");

                precio.ValorKiloAdicional = 0;

                precioTrayecto.Where(r => r.Field<string>("TRS_IdTipoSubTrayecto") != TAConstantesTarifas.ID_TIPO_SUBTRAYECTO_KILO_INICIAL)
                  .ToList()
                  .ForEach(f =>
                  {
                      precio.ValorKiloAdicional += f.Field<decimal>("PTR_ValorFijo");
                  });

                bool aplicaTipoEntrega = false;
                if (idTipoEntrega != "-1")
                {
                    cmd = new SqlCommand("paObtenerPrecioTipoEntregaListaPrecios_TAR", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdListaPrecio", idListaPrecio);
                    cmd.Parameters.AddWithValue("@IdTipoEntrega", idTipoEntrega);
                    cmd.Parameters.AddWithValue("@IdServicio", idServicio);

                    SqlDataAdapter daTipo = new SqlDataAdapter(cmd);
                    DataTable dtTipo = new DataTable();
                    //conn.Open();
                    daTipo.Fill(dtTipo);
                    conn.Close();

                    var precioTipoEntrega = dtTipo.AsEnumerable().ToList().FirstOrDefault();

                    if (precioTipoEntrega != null)
                    {
                        aplicaTipoEntrega = true;
                        precio.ValorKiloInicial = precioTipoEntrega.Field<decimal>("PTE_ValorKiloInicial");
                        precio.ValorKiloAdicional = precioTipoEntrega.Field<decimal>("PTE_ValorKiloAdicional");
                    }
                }

                ///Si hay excepciones obtiene el valor del kilo inicial(Valor configurado en la excepcion)
                ///valor del kilo adicional(valor adicional del trayecto)
                if (!aplicaTipoEntrega && excepciones != null)
                {
                    precio.ValorKiloInicial = excepciones.Field<decimal>("SET_ValorKiloInicial");
                    precio.ValorKiloAdicional = excepciones.Field<decimal>("SET_ValorKiloAdicional");
                }

                decimal totalAdicional = (peso - TAConstantesTarifas.VALOR_KILO_INICIAL_EXCEPCION_NOTIFICACIONES) * precio.ValorKiloAdicional;
                precio.ValorPrimaSeguro = ObtenerPrimaSeguro(idListaPrecio, valorDeclarado, idServicio);

                if (esPrimeraAdmision)
                {
                    precio.Valor = (precio.ValorKiloInicial + totalAdicional);
                }
                else
                {
                    precio.Valor = precio.ValorKiloAdicional + totalAdicional;
                }

                return precio;
            }
        }

        /// <summary>
        /// Obtiene la prima de seguro de una lista de precio servicio
        /// </summary>
        /// <param name="idListaPrecioServicio">Identificador lista de precio servicio</param>
        /// <returns>Prima de seguro</returns>
        public decimal ObtenerPrimaSeguro(int idListaPrecio, decimal valorDeclarado, int idServicio)
        {
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerPrimaSeguro_TAR", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdListaPrecio", idListaPrecio);
                cmd.Parameters.AddWithValue("@IdServicio", idServicio);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (!reader.HasRows)
                {
                    //throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.TARIFAS, TAEnumTipoErrorTarifas.EX_NO_EXISTE_PRIMA_SEGURO_PARA_LISTAPRECIO_SERVICIO.ToString(), TAMensajesTarifas.CargarMensaje(TAEnumTipoErrorTarifas.EX_NO_EXISTE_PRIMA_SEGURO_PARA_LISTAPRECIO_SERVICIO)));
                    throw new FaultException<Exception>(new Exception("No existe la prima de seguro para la lista de precio servicio."));
                }

                while (reader.Read())
                {
                    return (Convert.ToDecimal(reader["LPS_PrimaSeguros"]) / 100) * valorDeclarado;
                }

                throw new FaultException<Exception>(new Exception("No existe la prima de seguro para la lista de precio servicio."));
            }
        }

        /// <summary>
        /// Retorna los impuestos asignados a un servicio
        /// </summary>
        /// <param name="idServicio">Identificador del servicio</param>
        /// <returns>Colección de servicios</returns>
        public List<TAImpuestosDC> ObtenerValorImpuestosServicio(int idServicio)
        {
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerImpuestoServicio_SER", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdServicio", idServicio);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                List<TAImpuestosDC> lstImpuestos = new List<TAImpuestosDC>();
                while (reader.Read())
                {
                    lstImpuestos.Add(new TAImpuestosDC()
                    {
                        Identificador = Convert.ToInt16(reader["SEI_IdImpuesto"]),
                        Descripcion = reader["IMP_Descripcion"].ToString(),
                        Valor = Convert.ToDecimal(reader["IMP_Valor"])
                    });
                }
                conn.Close();
                return lstImpuestos;
            }
        }

        /// <summary>
        /// Obtiene el identificador de ListaPrecioServicio_TAR
        /// </summary>
        /// <param name="idServicio">Identificador Servicio</param>
        /// <param name="idListaPrecio">Identificador Lista Precio</param>
        /// <returns>Identificador ListaPrecioServicio_TAR</returns>
        public string ObtenerIdentificadorListaPrecioServicio(int idServicio, int idListaPrecio)
        {
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerListaPreciosServicio_TAR", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdServicio", idServicio);
                cmd.Parameters.AddWithValue("@IdListaPrecios", idListaPrecio);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (!reader.HasRows)
                {
                    /*ControllerException excepcion = new ControllerException(COConstantesModulos.TARIFAS, TAEnumTipoErrorTarifas.EX_EL_SEERVICIO_SOLICTADO_NO_ESTA_EN_LA_LISTA_DE_PRECIOS.ToString(), TAMensajesTarifas.CargarMensaje(TAEnumTipoErrorTarifas.EX_EL_SEERVICIO_SOLICTADO_NO_ESTA_EN_LA_LISTA_DE_PRECIOS));
                    throw new FaultException<ControllerException>(excepcion);*/
                    throw new FaultException<Exception>(new Exception("El servicio solicitado no está en la lista de precios."));
                }
                string idLista = string.Empty;
                while (reader.Read())
                {
                    idLista = reader["LPS_IdListaPrecioServicio"].ToString();
                }
                conn.Close();

                return idLista;
            }
        }

        /// <summary>
        /// Calcula precio rapicarga, el calculo del precio se realiza de acuerdo al peso ingresado y los rangos configurados
        /// Si el peso ingresado esta en un valor intermedio se aplica la siguiente formula
        /// valor=(valorRango * pesoRangoFinal) +(kilosAdicionales * valorRango)
        /// </summary>
        /// <param name="idServicio">Identificador servicio</param>
        /// <param name="idListaPrecio">Identificador lista de precio</param>
        /// <param name="idLocalidadOrigen">Identificador localidad de origen</param>
        /// <param name="idLocalidadDestino">Identificador localidad de destino</param>
        /// <param name="peso">Peso</param>
        /// <returns>Objeto precio</returns>
        public TAPrecioCargaDC ObtenerPrecioRapiCarga(int idServicio, int idListaPrecio, int idLp, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision, string idTipoEntrega = "-1")
        {
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                bool esMensajeria = false;
                TAPrecioCargaDC precio = new TAPrecioCargaDC();

                SqlCommand cmd = new SqlCommand("paPrecioTrayectoRango_TAR", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdServicio", idServicio);
                cmd.Parameters.AddWithValue("@IdLocalidadOrigen", idLocalidadOrigen);
                cmd.Parameters.AddWithValue("@IdLocalidadDestino", idLocalidadDestino);
                cmd.Parameters.AddWithValue("@IdListaPrecioServicio", idLp);
                conn.Open();
                SqlDataAdapter daPtr = new SqlDataAdapter(cmd);
                DataTable dtPtr = new DataTable();
                daPtr.Fill(dtPtr);

                var precioTrayectoRango = dtPtr.AsEnumerable().ToList();// contexto.paPrecioTrayectoRango_TAR(idServicio, idLocalidadOrigen, idLocalidadDestino, idLp).ToList();

                precioTrayectoRango.ForEach(f =>
                {
                    if (f.Field<string>("TRS_IdTipoSubTrayecto") == TAConstantesTarifas.ID_TIPO_TRAYECTO_ESPECIAL)
                        esMensajeria = true;
                });

                if (esMensajeria == true)
                {
                    TAPrecioMensajeriaDC precioMensajeria = ObtenerPrecioMensajeria(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
                    precio.Valor = precioMensajeria.Valor;
                    precio.ValorKiloAdicional = precioMensajeria.ValorKiloAdicional;
                }
                else if (esMensajeria == false)
                {
                    ///Obtiene las excepciones del trayecto
                    cmd = new SqlCommand("paObtenerExTrayecto_TAR", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdServicio", idServicio);
                    cmd.Parameters.AddWithValue("@IdLocalidadOrigen", idLocalidadOrigen);
                    cmd.Parameters.AddWithValue("@IdLocalidadDestino", idLocalidadDestino);
                    cmd.Parameters.AddWithValue("@IdListaPrecio", idListaPrecio);
                    SqlDataAdapter daExc = new SqlDataAdapter(cmd);
                    DataTable dtExc = new DataTable();
                    // conn.Open();
                    daExc.Fill(dtExc);

                    ///Obtiene las excepciones del trayecto
                    var excepciones = dtExc.AsEnumerable().ToList().FirstOrDefault();
                    if (excepciones != null && precioTrayectoRango.Count() == 0)
                    {
                        DataRow dr = dtPtr.NewRow();
                        precioTrayectoRango.Add(dr);
                    }

                    bool aplicoTipoEntrega = false;
                    if (precioTrayectoRango.Count() > 0)
                    {
                        //Obtiene los precios por tipo de entrega
                        if (idTipoEntrega != "-1")
                        {
                            cmd = new SqlCommand("paObtenerPrecioTipoEntregaListaPrecios_TAR", conn);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@IdListaPrecio", idListaPrecio);
                            cmd.Parameters.AddWithValue("@IdTipoEntrega", idTipoEntrega);
                            cmd.Parameters.AddWithValue("@IdServicio", idServicio);

                            SqlDataAdapter daTipo = new SqlDataAdapter(cmd);
                            DataTable dtTipo = new DataTable();
                            daTipo.Fill(dtTipo);

                            var precioTipoEntrega = dtTipo.AsEnumerable().ToList().FirstOrDefault();

                            if (precioTipoEntrega != null)
                            {
                                aplicoTipoEntrega = true;
                                precioTrayectoRango.ForEach(f =>
                                {
                                    f.SetField<decimal>("PPR_Final", precioTipoEntrega.Field<decimal>("PTR_Final"));
                                    f.SetField<decimal>("PPR_Inicial", precioTipoEntrega.Field<decimal>("PTR_Inicial"));
                                    f.SetField<decimal>("PPR_Valor", precioTipoEntrega.Field<decimal>("PTE_ValorKiloAdicional"));
                                });
                            }
                        }

                        ///Si hay excepciones obtiene el valor del kilo inicial(Valor configurado en la excepcion)
                        ///valor del kilo adicional(valor adicional del trayecto)
                        if (!aplicoTipoEntrega && excepciones != null)
                        {
                            precioTrayectoRango.ForEach(f =>
                            {
                                f.SetField<decimal>("PPR_Final", excepciones.Field<decimal>("PTR_Final"));
                                f.SetField<decimal>("PPR_Inicial", excepciones.Field<decimal>("PTR_Inicial"));
                                f.SetField<decimal>("PPR_Valor", excepciones.Field<decimal>("SET_ValorKiloAdicional"));
                            });
                        }

                        precio.ValorPrimaSeguro = ObtenerPrimaSeguro(idListaPrecio, valorDeclarado, idServicio);

                        // Para rapicarga se debe validar una opción que indica si se debe ignorar la prima de seguro sin importar el valor declarado, esto es para ciertos clientes especiales
                        //var listaPrecioServicio = contexto.ListaPrecioServicio_TAR.FirstOrDefault(l => l.LPS_IdListaPrecioServicio == idLp);

                        if (precioTrayectoRango.Where(p => p.Field<decimal>("PPR_Inicial") <= peso && p.Field<decimal>("PPR_Final") >= peso).Count() > 0)
                        {
                            var consulta = precioTrayectoRango.Where(p => p.Field<decimal>("PPR_Inicial") <= peso && p.Field<decimal>("PPR_Final") >= peso).FirstOrDefault();

                            precio.Valor = consulta.Field<decimal>("PPR_Valor") * consulta.Field<decimal>("PPR_Final");
                            precio.ValorKiloAdicional = consulta.Field<decimal?>("PTR_ValorFijo") ?? 0m;

                            //precio.ValorServicioRetorno = contexto.paObtenerPreTraSubValAdi_TAR(idPrecioTrayectoSubTrayecto).FirstOrDefault().PTV_Valor;
                        }
                        else if (peso < precioTrayectoRango.OrderBy(o => o.Field<decimal>("PPR_Inicial")).First().Field<decimal>("PPR_Inicial"))
                        {
                            TAPrecioMensajeriaDC precioMensajeria = ObtenerPrecioMensajeria(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision);
                            precio.Valor = precioMensajeria.Valor;
                            precio.ValorKiloAdicional = precioMensajeria.ValorKiloAdicional;
                        }
                        else if (peso > precioTrayectoRango.OrderBy(o => o.Field<decimal>("PPR_Final")).Last().Field<decimal>("PPR_Final"))
                        {
                            var consulta = precioTrayectoRango.OrderBy(o => o.Field<decimal>("PPR_Final")).Last();
                            decimal pesoAdicional = peso - (consulta.Field<decimal>("PPR_Final"));
                            //long idPrecioTrayectoSubTrayecto = consulta.Field<long>("PTR_IdPrecioTrayectoSubTrayect");

                            /*  cmd = new SqlCommand("paObtenerPrecioTrayectoSubTrayecto_TAR", conn);
                              cmd.CommandType = CommandType.StoredProcedure;
                              cmd.Parameters.AddWithValue("@IdPrecioTrayectoSubTrayect", idPrecioTrayectoSubTrayecto);

                              SqlDataAdapter daPreTra = new SqlDataAdapter(cmd);
                              DataTable dtPreTra = new DataTable();
                              daPreTra.Fill(dtPreTra);

                              var valorBase = dtPreTra.AsEnumerable().ToList().FirstOrDefault();*/
                            //var valorBase = contexto.PrecioTrayecto_TAR.Where(r => r.PTR_IdPrecioTrayectoSubTrayect == idPrecioTrayectoSubTrayecto).FirstOrDefault();

                            precio.ValorKiloAdicional = consulta.Field<decimal>("PTR_ValorFijo");
                            precio.Valor = (consulta.Field<decimal>("PPR_Valor") * consulta.Field<decimal>("PPR_Final")) + (pesoAdicional * consulta.Field<decimal>("PPR_Valor"));
                        }
                        else
                        {
                            var rangos = precioTrayectoRango.OrderBy(o => o.Field<decimal>("PPR_Inicial")).ToList();
                            bool calculoTarifa = false;
                            for (int i = 0; i < rangos.Count() - 1; i++)
                            {
                                if (peso > rangos[i].Field<decimal>("PPR_Final") && peso < rangos[i + 1].Field<decimal>("PPR_Inicial"))
                                {
                                    if (!calculoTarifa)
                                    {
                                        decimal pesoAdicional = peso - (rangos[i].Field<decimal>("PPR_Final"));
                                        //long idPrecioTrayectoSubTrayecto = rangos[i].Field<long>("PTR_IdPrecioTrayectoSubTrayect");
                                        //var valorBase = contexto.PrecioTrayecto_TAR.Where(r => r.PTR_IdPrecioTrayectoSubTrayect == idPrecioTrayectoSubTrayecto).FirstOrDefault();
                                        precio.ValorKiloAdicional = rangos[i].Field<decimal>("PTR_ValorFijo");
                                        precio.Valor = (rangos[i].Field<decimal>("PTR_ValorFijo") * rangos[i].Field<decimal>("PPR_Final")) + (pesoAdicional * rangos[i].Field<decimal>("PTR_ValorFijo"));
                                        calculoTarifa = true;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        //throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.TARIFAS, TAEnumTipoErrorTarifas.EX_PRECIO_SERVICIO_NO_CONFIGURADO.ToString(), TAMensajesTarifas.CargarMensaje(TAEnumTipoErrorTarifas.EX_PRECIO_SERVICIO_NO_CONFIGURADO)));
                        throw new FaultException<Exception>(new Exception("Precio servicio no configurado."));
                    }
                }

                return precio;
            }
        }

        public bool ValidarTokenNumeroGuia(long numeroGuia, string token)
        {
            bool validacion = false;
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paValidarTokenporNumeroGuia", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroGuia", numeroGuia);
                cmd.Parameters.AddWithValue("@Token", token);
                sqlConn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        validacion = true;
                    }

                }
            }
            return validacion;
        }



        #endregion COTIZADOR
    }
}