using CO.Servidor.Dominio.Comun.AdmEstadosGuia.Datos.Modelo;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Comun;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;


namespace CO.Servidor.Dominio.Comun.AdmEstadosGuia.Datos
{
    /// <summary>
    /// Contiene la parte de acceso a datos para el administrador de estados de guía
    /// </summary>
    internal class EGRepositorio
    {
        private string CadCnxController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;
        private string CadCnxSispostalController = ConfigurationManager.ConnectionStrings["Sispostal"].ConnectionString;

        #region Atributos

        private string NombreModelo = "EstadosGuiaEntities";

        #endregion

        #region Instancia singleton de la clase

        private static readonly EGRepositorio instancia = new EGRepositorio();

        public static EGRepositorio Instancia
        {
            get
            {
                return instancia;
            }
        }

        #endregion Instancia singleton de la clase

        #region Consultas

        /// <summary>
        /// Retorna los estados de la guía de la base de datos
        /// </summary>
        /// <returns></returns>
        internal List<EGEstadoGuia> ObtenerEstadosGuia()
        {
            List<EGEstadoGuia> lst = new List<EGEstadoGuia>();
            using (SqlConnection cnn = new SqlConnection(CadCnxController))
            {

                SqlCommand cmd = new SqlCommand("paObtenerEstadosGuiasPredecesor_MEN", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                cnn.Open();
                da.Fill(dt);
                cnn.Close();
                //TODO: PROBAR FUNCIONAMIENTO
                List<EGEstadoGuia> lstEstados = dt.AsEnumerable().ToList().GroupBy(p => p.Field<short>("PEG_IdEstadoGuia")).ToList()
                      .ConvertAll<EGEstadoGuia>(estado =>
                          {
                              EGEstadoGuia r = new EGEstadoGuia();
                              r.Id = estado.First().Field<short>("PEG_IdEstadoGuia");
                              r.Precedesores = estado.ToList().ConvertAll(predecesor =>
                                      new EGEstadoGuia
                                      {
                                          Id = predecesor.Field<short>("PEG_IdEstadoGuiaPredecesor")
                                      });
                              return r;
                          });
                return lstEstados;
            }
        }


        /// <summary>
        /// Obtiene los estados  de una guia en una localidad
        /// </summary>
        /// <returns></returns>
        internal List<ADTrazaGuia> ObtenerEstadosGuia(long numeroGuia)
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
        /// Obtiene los Estados y Motivos de la Guia seleccionada
        /// </summary>
        /// <returns></returns>
        internal List<ADEstadoGuiaMotivoDC> ObtenerEstadosMotivosGuia(long numeroGuia)
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
                        estado.FechaMotivo = reader["EGM_FechaMotivo"] == DBNull.Value ? Convert.ToDateTime(reader["EGT_FechaGrabacion"]) : Convert.ToDateTime(reader["EGM_FechaMotivo"]) ;
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
        /// Obtiene los estados  de una guia filtrando por IdAdmisión
        /// </summary>
        /// <returns></returns>
        internal List<ADTrazaGuia> ObtenerEstadosGuiaxIdAdmision(long idAdmision)
        {

            List<ADTrazaGuia> lstGuiaxAdm = new List<ADTrazaGuia>();
            using (SqlConnection cnn = new SqlConnection(CadCnxController))
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerEstadosGuiaxIdAdmision_MEN", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdAdmMensajeria", idAdmision);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    lstGuiaxAdm.Add(new ADTrazaGuia()
                    {
                        Ciudad = reader["EGT_NombreLocalidad"].ToString(),
                        IdAdmision = Convert.ToInt64(reader["EGT_IdAdminisionMensajeria"]),
                        IdCiudad = reader["EGT_IdLocalidad"].ToString(),
                        IdEstadoGuia = Convert.ToInt16(reader["EGT_IdEstadoGuia"]),
                        Modulo = reader["EGT_IdModulo"].ToString(),
                        NumeroGuia = Convert.ToInt64(reader["EGT_NumeroGuia"]),
                        Observaciones = reader["EGT_Observacion"].ToString(),
                        FechaGrabacion = Convert.ToDateTime(reader["EGT_FechaGrabacion"]),
                    });
                }
            }
            return lstGuiaxAdm;
        }


        /// <summary>
        /// Retorna el ultimo estado de una guia por el id de admision
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <returns></returns>
        internal ADEnumEstadoGuia ObtenerUltimoEstado(long idAdmision)
        {
            return (ADEnumEstadoGuia)ObtenerTrazaUltimoEstadoGuia(idAdmision).IdEstadoGuia;
        }

        /// <summary>
        /// Retorna el ultimo estado de una guia por el numero
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <returns></returns>
        internal ADEnumEstadoGuia ObtenerUltimoEstadoxNumero(long numeroGuia)
        {

            return (ADEnumEstadoGuia)ObtenerTrazaUltimoEstadoXNumGuia(numeroGuia).IdEstadoGuia;
        }


        /// <summary>
        /// Si una Guia en Estado Centro de Acopio, se Puede anular
        /// </summary>
        /// <param name="NroGuia"></param>
        /// <returns></returns>
        internal bool EnCentrodeAcopio_Automatico(long NroGuia)
        {
            bool RtaValidacion = true;
            using (SqlConnection cnn = new SqlConnection(CadCnxController))
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand("paValidarEnCentrodeAcopio", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@numeroGuia", NroGuia);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                    RtaValidacion = false;
            }
            return RtaValidacion;
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

                traza = Mapper.EGRepositorioMapper.ToListEstadoGuia(reader);

            }
            return traza;
        }



        internal IList<ADTrazaGuia> ObtenerTrazaGuiasLocalidad(long numeroGuia, string idLocalidad)
        {
            IList<ADTrazaGuia> listaTraza = new List<ADTrazaGuia>();
            using (SqlConnection cnn = new SqlConnection(CadCnxController))
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerTrazaGuiaLocalidad_MEN", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EGT_NumeroGuia", numeroGuia);
                cmd.Parameters.AddWithValue("EGT_IdLocalidad", idLocalidad);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    listaTraza.Add(
                        new ADTrazaGuia
                        {
                            Ciudad = reader["EGT_NombreLocalidad"].ToString(),
                            IdAdmision = (Convert.ToInt64(reader["EGT_IdAdminisionMensajeria"])),
                            IdCiudad = reader["EGT_IdLocalidad"].ToString(),
                            IdEstadoGuia = (Convert.ToInt16(reader["EGT_IdEstadoGuia"])),
                            NumeroGuia = (Convert.ToInt64(reader["EGT_NumeroGuia"])),
                            Modulo = reader["EGT_IdModulo"].ToString(),
                            Observaciones = reader["EGT_Observacion"].ToString(),
                            FechaGrabacion = (Convert.ToDateTime(reader["EGT_FechaGrabacion"]))
                        });

                }
            }
            return listaTraza;
        }



        /// <summary>
        /// Metodo para obtener el proximo estado de una guía de devolución
        /// </summary>
        /// <param name="idMotivo"></param>
        /// <param name="estadoActual"></param>
        /// <returns></returns>
        public short ObtenerEstadoMotivo(short idMotivo, short estadoActual, short intentoEntrega)
        {
            short estadosGuia = 0;
            using (SqlConnection cnn = new SqlConnection(CadCnxController))
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerMotivoGuiaTransicionEstado_MEN", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdEstadoGuiaDesde", estadoActual);
                cmd.Parameters.AddWithValue("@IdMotivoGuia", idMotivo);
                cmd.Parameters.AddWithValue("@IntentoEntrega", intentoEntrega);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    estadosGuia = (Convert.ToInt16(reader["MGT_IdEstadoGuiaHasta"]));
                }
            }
            return estadosGuia;
        }



        /// <summary>
        /// Método para validar si una guia paso por un estado especifico
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="estadoGuia"></param>
        /// <returns></returns>
        internal bool ValidarEstadoGuia(long numeroGuia, short estadoGuia)
        {
            using (SqlConnection cnn = new SqlConnection(CadCnxController))
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand("paValidarEstadoGuia_MEN", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@numeroGuia", numeroGuia);
                cmd.Parameters.AddWithValue("@estadoGuia", (int)estadoGuia);

                SqlParameter outPutVal = new SqlParameter("@respuesta", SqlDbType.Int);
                outPutVal.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outPutVal);
                cmd.ExecuteReader();

                if (outPutVal.Value != DBNull.Value && (int)outPutVal.Value == 0)
                    return false;
                else
                    return true;
            }
        }

        #endregion Consultas

        #region Inserciones


        /// <summary>
        /// Guarda la traza de la guia ingresada
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="estadoGuia"></param>
        internal long InsertaEstadoGuia(ADTrazaGuia guia)
        {
            using (SqlConnection cnn = new SqlConnection(CadCnxController))
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand("PaCrearEstadoGuiaTraza_MEN", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idAdmisionMensajeria", guia.IdAdmision);
                cmd.Parameters.AddWithValue("@numeroGuia", guia.NumeroGuia);
                cmd.Parameters.AddWithValue("@idEstadoGuia", guia.IdNuevoEstadoGuia);
                cmd.Parameters.AddWithValue("@observacion", guia.Observaciones == null ? string.Empty : guia.Observaciones);
                cmd.Parameters.AddWithValue("@idLocalidad", guia.IdCiudad);
                cmd.Parameters.AddWithValue("@nombreLocalidad", guia.Ciudad);
                cmd.Parameters.AddWithValue("@idModulo", !string.IsNullOrWhiteSpace(guia.Modulo) ? guia.Modulo : "MEN");
                cmd.Parameters.AddWithValue("@fechaGrabacion", DateTime.Now);
                cmd.Parameters.AddWithValue("@creadoPor", ControllerContext.Current.Usuario);
                cmd.Parameters.AddWithValue("@idCentroServicio", ControllerContext.Current.IdCentroServicio);
                cmd.Parameters.AddWithValue("@NombreCentroServicio", ControllerContext.Current.NombreCentroServicio);
                cmd.Parameters.AddWithValue("@Latitud", guia.Latitud == null ? string.Empty : guia.Latitud);
                cmd.Parameters.AddWithValue("@Longitud", guia.Longitud == null ? string.Empty : guia.Longitud);
                /************************* Id Aplicacion Origen***********************************/
                if (ControllerContext.Current.IdAplicativoOrigen > 0)
                {
                    cmd.Parameters.AddWithValue("@IdAplicacionOrigen", ControllerContext.Current.IdAplicativoOrigen);
                }
                long num = Convert.ToInt64(cmd.ExecuteScalar());
                cnn.Close();

                return num;
            }
        }


        /// <summary>
        /// Guarda la traza de la guia ingresada
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="estadoGuia"></param>
        internal long InsertaEstadoGuiaCentroServicio(ADTrazaGuia guia)
        {
            using (SqlConnection cnn = new SqlConnection(CadCnxController))
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand("PaCrearEstadoGuiaTraza_MEN", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idAdmisionMensajeria", guia.IdAdmision);
                cmd.Parameters.AddWithValue("@numeroGuia", guia.NumeroGuia);
                cmd.Parameters.AddWithValue("@idEstadoGuia", guia.IdNuevoEstadoGuia);
                cmd.Parameters.AddWithValue("@observacion", guia.Observaciones == null ? string.Empty : guia.Observaciones);
                cmd.Parameters.AddWithValue("@idLocalidad", guia.IdCiudad);
                cmd.Parameters.AddWithValue("@nombreLocalidad", guia.Ciudad);
                cmd.Parameters.AddWithValue("@idModulo", !string.IsNullOrWhiteSpace(guia.Modulo) ? guia.Modulo : "MEN");
                cmd.Parameters.AddWithValue("@fechaGrabacion", DateTime.Now);
                cmd.Parameters.AddWithValue("@creadoPor", ControllerContext.Current.Usuario);
                cmd.Parameters.AddWithValue("@idCentroServicio", guia.IdCentroServicioEstado);
                cmd.Parameters.AddWithValue("@NombreCentroServicio", guia.NombreCentroServicioEstado);
                cmd.Parameters.AddWithValue("@Latitud", guia.Latitud == null ? string.Empty : guia.Latitud);
                cmd.Parameters.AddWithValue("@Longitud", guia.Longitud == null ? string.Empty : guia.Longitud);

                long num = Convert.ToInt64(cmd.ExecuteScalar());
                cnn.Close();

                return num;
            }
        }



        /// <summary>
        /// Guarda la traza de la guia ingresada con fecha de estado
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="estadoGuia"></param>
        internal long InsertaEstadoGuiaFecha(ADTrazaGuia guia)
        {
            using (SqlConnection cnn = new SqlConnection(CadCnxController))
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand("PaCrearEstadoGuiaTrazaFecha_MEN", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idAdmisionMensajeria", guia.IdAdmision);
                cmd.Parameters.AddWithValue("@numeroGuia", guia.NumeroGuia);
                cmd.Parameters.AddWithValue("@idEstadoGuia", guia.IdNuevoEstadoGuia);
                cmd.Parameters.AddWithValue("@observacion", guia.Observaciones == null ? string.Empty : guia.Observaciones);
                cmd.Parameters.AddWithValue("@idLocalidad", guia.IdCiudad);
                cmd.Parameters.AddWithValue("@nombreLocalidad", guia.Ciudad);
                cmd.Parameters.AddWithValue("@idModulo", !string.IsNullOrWhiteSpace(guia.Modulo) ? guia.Modulo : "MEN");
                cmd.Parameters.AddWithValue("@fechaGrabacion", guia.FechaGrabacion.Year != 1 ? guia.FechaGrabacion:DateTime.Now);
                cmd.Parameters.AddWithValue("@creadoPor", ControllerContext.Current.Usuario);
                cmd.Parameters.AddWithValue("@idCentroServicio", ControllerContext.Current.IdCentroServicio);
                cmd.Parameters.AddWithValue("@NombreCentroServicio", ControllerContext.Current.NombreCentroServicio);
                cmd.Parameters.AddWithValue("@Latitud", guia.Latitud == null ? string.Empty : guia.Latitud);
                cmd.Parameters.AddWithValue("@Longitud", guia.Longitud == null ? string.Empty : guia.Longitud);
                /************************* Id Aplicacion Origen***********************************/
                if (ControllerContext.Current.IdAplicativoOrigen > 0)
                {
                    cmd.Parameters.AddWithValue("@IdAplicacionOrigen", ControllerContext.Current.IdAplicativoOrigen);
                }
                long num = Convert.ToInt64(cmd.ExecuteScalar());
                cnn.Close();

                return num;
            }
        }

        /// <summary>
        /// Guarda el estado motivo de una guía al realizar un cambio de estado
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="estadoGuia"></param>
        internal void InsertaEstadoGuiaMotivo(ADEstadoGuiaMotivoDC guia)
        {
            if (guia.FechaMotivo == DateTime.MinValue)
                guia.FechaMotivo = DateTime.Now.Date;

            using (SqlConnection cnn = new SqlConnection(CadCnxController))
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand("PaCrearEstadoGuiaMotivo_MEN", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idEstadoGuiaLog", guia.IdTrazaGuia);
                cmd.Parameters.AddWithValue("@idMotivoGuia", guia.Motivo.IdMotivoGuia);
                cmd.Parameters.AddWithValue("@observacion", guia.Observaciones);
                cmd.Parameters.AddWithValue("@fechaGrabacion", guia.FechaMotivo);
                cmd.Parameters.AddWithValue("@creadoPor", ControllerContext.Current.Usuario);
                cmd.ExecuteNonQuery();
                cnn.Close();
            }
        }


        /// <summary>
        /// Guarda el estado motivo de una guía al realizar un cambio de estado
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="estadoGuia"></param>
        internal void InsertaEstadoGuiaMotivoFecha(ADEstadoGuiaMotivoDC guia)
        {
            if (guia.FechaMotivo == DateTime.MinValue)
                guia.FechaMotivo = DateTime.Now.Date;

            using (SqlConnection cnn = new SqlConnection(CadCnxController))
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand("PaCrearEstadoGuiaMotivoFecha_MEN", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idEstadoGuiaLog", guia.IdTrazaGuia);
                cmd.Parameters.AddWithValue("@idMotivoGuia", guia.Motivo.IdMotivoGuia);
                cmd.Parameters.AddWithValue("@observacion", guia.Observaciones);
                cmd.Parameters.AddWithValue("@fechaGrabacion", guia.FechaMotivo);
                cmd.Parameters.AddWithValue("@creadoPor", ControllerContext.Current.Usuario);
                cmd.Parameters.AddWithValue("@EGM_TipoPredio", guia.TipoPredio);
                cmd.Parameters.AddWithValue("@EGM_DescripcionPredio", guia.DescripcionPredio);
                cmd.Parameters.AddWithValue("@EGM_TipoContador", guia.TipoContador);
                cmd.Parameters.AddWithValue("@EGM_NumeroContador", guia.NumeroContador);
                cmd.ExecuteNonQuery();
                cnn.Close();
            }
        }

        /// <summary>
        /// Guarda el estado motivo de una guía al realizar un cambio de estado
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="estadoGuia"></param>
        internal void InsertaEstadoGuiaMotivoParametros(ADEstadoGuiaMotivoDC guia)
        {
            if (guia.FechaMotivo == DateTime.MinValue)
                guia.FechaMotivo = DateTime.Now.Date;

            using (SqlConnection cnn = new SqlConnection(CadCnxController))
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand("PaCrearEstadoGuiaMotivoFecha_MEN", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idEstadoGuiaLog", guia.IdTrazaGuia);
                cmd.Parameters.AddWithValue("@idMotivoGuia", guia.Motivo.IdMotivoGuia);
                cmd.Parameters.AddWithValue("@observacion", guia.Observaciones);
                cmd.Parameters.AddWithValue("@fechaGrabacion", guia.FechaMotivo);
                cmd.Parameters.AddWithValue("@creadoPor", ControllerContext.Current.Usuario);
                cmd.ExecuteNonQuery();
                cnn.Close();
            }
        }

        /// <summary>
        /// Actualiza el estado motivo de una guía al realizar un cambio de estado
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="estadoGuia"></param>
        internal void ActualizarEstadoGuiaMotivo(ADEstadoGuiaMotivoDC guia)
        {
            using (SqlConnection cnn = new SqlConnection(CadCnxController))
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand("paModificarMotivoGuia_LOI", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idEstadoGuiaLog", guia.IdTrazaGuia);
                cmd.Parameters.AddWithValue("@idMotivoGuia", guia.Motivo.IdMotivoGuia);
                cmd.Parameters.AddWithValue("@fechaMotivo", guia.FechaMotivo);
                cmd.ExecuteNonQuery();
                cnn.Close();
            }
        }




        /// <summary>
        /// Método para insertar un registro de estado guía relacionado a un impreso
        /// </summary>
        /// <param name="trazaImpreso"></param>
        internal void InsertarEstadoGuiaImpreso(ADTrazaGuiaImpresoDC trazaImpreso)
        {
            using (SqlConnection cnn = new SqlConnection(CadCnxController))
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand("PaCrearEstadoGuiaTipoImpreso_MEN", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EGI_IdEstadoGuiaLog", trazaImpreso.IdTrazaGuia);
                cmd.Parameters.AddWithValue("@EGI_IdTipoImpreso", (short)trazaImpreso.TipoImpreso);
                cmd.Parameters.AddWithValue("@EGI_NumeroImpreso", trazaImpreso.NumeroImpreso);
                cmd.Parameters.AddWithValue("@EGI_FechaGrabacion", DateTime.Now);
                cmd.Parameters.AddWithValue("@EGI_CreadoPor", ControllerContext.Current.Usuario);
                //  cmd.Parameters.AddWithValue("@EGI_IdTercero", trazaImpreso.IdTercero);
                cmd.ExecuteNonQuery();
                cnn.Close();
            }
        }

        #endregion Inserciones

        #region Devolver Estados
        // todo:id
        internal List<ADEstadoGuia> ObtenerEstadosParaDevolver()
        {
            List<ADEstadoGuia> LstEstados = new List<ADEstadoGuia>();

            try
            {
                using (SqlConnection cnn = new SqlConnection(CadCnxController))
                {
                    cnn.Open();
                    string cadSQL = "SELECT ESG_IdEstadoGuia, ESG_Descripcion FROM EstadoGuia_MEN WHERE ESG_PermiteDevolver = 1";

                    SqlCommand comando = new SqlCommand(cadSQL, cnn);
                    comando.CommandType = CommandType.Text;

                    SqlDataReader reader = comando.ExecuteReader();

                    while (reader.Read())
                    {
                        LstEstados.Add(new ADEstadoGuia() { Id = Convert.ToInt16(reader["ESG_IdEstadoGuia"]), Descripcion = reader["ESG_Descripcion"].ToString() });
                    }
                }

                return LstEstados;
            }
            catch (Exception exc)
            {
                ControllerException excepcion = new ControllerException(COConstantesModulos.MENSAJERIA,
                EGEnumTipoError.EX_ERROR_MOTIVO_ESTADO.ToString(), exc.Message);
                throw new FaultException<ControllerException>(excepcion);
            }
        }

        // todo:id
        internal void CambiarDevolverEstadoGuia(long IdNumeroGuia, long pNuevoIdEstado, string pObservaciones, string Usuario)
        {
            List<ADTrazaGuia> arrEstadosGuia = ObtenerEstadosGuia(IdNumeroGuia);

            if (arrEstadosGuia != null && arrEstadosGuia.Count > 0)
            {
                ADTrazaGuia EstadoACambiar = arrEstadosGuia.Where(dd => dd.IdEstadoGuia == pNuevoIdEstado).FirstOrDefault();

                if (EstadoACambiar != null)
                {
                    long IdUltimoEstado = (long)ObtenerUltimoEstadoxNumero(IdNumeroGuia);

                    if (EstadoACambiar.IdEstadoGuia.Value != IdUltimoEstado)
                    {

                        SqlTransaction objtran = null;
                        try
                        {
                            //1. Cambiar ADM_IdEstadoGuiaLog en AdmisionMensajeria_MEN. Si su ultimo Estado es Entregada, se quita el Bit en (ADM_EstaEntregada)
                            using (SqlConnection cnn = new SqlConnection(CadCnxController))
                            {
                                cnn.Open();
                                objtran = cnn.BeginTransaction();
                                string cadSQL = "UPDATE AdmisionMensajeria_MEN WITH(ROWLOCK) SET ADM_EstaEntregada = 0 "
                                            + " , ADM_IdEstadoGuiaLog = " + EstadoACambiar.IdTrazaGuia.ToString()
                                            + " WHERE ADM_NumeroGuia = " + IdNumeroGuia.ToString()

                                + " INSERT INTO AuditoriaCambioEstadoGuia_AUD VALUES(" + IdNumeroGuia.ToString() + ","
                                            + IdUltimoEstado.ToString() + "," + pNuevoIdEstado.ToString() + ",'" + pObservaciones + "',getdate(),'" + Usuario + "')";

                                SqlCommand comando = new SqlCommand();
                                comando.CommandType = CommandType.Text;
                                comando.CommandText = cadSQL;
                                comando.Transaction = objtran;
                                comando.Connection = cnn;

                                comando.ExecuteNonQuery();
                                objtran.Commit();
                            }


                            //2. Llenar Nueva Tabla de Auditoria


                        }
                        catch (Exception exc)
                        {
                            objtran.Rollback();
                            ControllerException excepcion = new ControllerException(COConstantesModulos.MENSAJERIA,
                            EGEnumTipoError.EX_ERROR_MOTIVO_ESTADO.ToString(), exc.Message);
                            throw new FaultException<ControllerException>(excepcion);
                        }
                    }
                    else
                    {
                        ControllerException excepcion = new ControllerException(COConstantesModulos.MENSAJERIA,
                            EGEnumTipoError.EX_ERROR_MOTIVO_ESTADO.ToString(), "La Guía ya se encuentra en el Estado Seleccionado.");
                        throw new FaultException<ControllerException>(excepcion);
                    }
                }
                else
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.MENSAJERIA,
                        EGEnumTipoError.EX_ERROR_MOTIVO_ESTADO.ToString(), "La Guía no ha tenido el Estado Seleccionado.");
                    throw new FaultException<ControllerException>(excepcion);
                }

            }
            else
            {
                ControllerException excepcion = new ControllerException(COConstantesModulos.MENSAJERIA,
                        EGEnumTipoError.EX_ERROR_MOTIVO_ESTADO.ToString(), "No Existe Guía con número: " + IdNumeroGuia.ToString());
                throw new FaultException<ControllerException>(excepcion);
            }

        }

        #endregion

        #region Novedades


        /// <summary>
        /// Obtiene los tipos de novedad de una guia de acuerdo al tipo
        /// </summary>
        /// <returns></returns>
        internal List<COTipoNovedadGuiaDC> ObtenerTiposNovedadGuia(COEnumTipoNovedad tipoNovedad)
        {
            List<COTipoNovedadGuiaDC> listaNovedades = new List<COTipoNovedadGuiaDC>();
            using (SqlConnection cnn = new SqlConnection(CadCnxController))
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerTipoNovedadGuia_MEN", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdTipoNovedad", (short)tipoNovedad);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    listaNovedades.Add(new COTipoNovedadGuiaDC()
                    {
                        IdTipoNovedadGuia = Convert.ToInt16(reader["TNG_IdTipoNovedadGuia"]),
                        Descripcion = reader["TNG_Descripcion"].ToString(),
                        TipoNovedad = tipoNovedad,
                        TiempoAfectacion = Convert.ToInt32(reader["TNG_TiempoAfectacion"]),
                        ToolTip = DBNull.Value.Equals(reader["TNG_ToolTip"]) ? String.Empty : Convert.ToString(reader["TNG_ToolTip"]),
                    });
                }
            }
            return listaNovedades;
        }



        /// <summary>
        /// Método para afectar la fecha estimada de entrega
        /// </summary>
        /// <param name="cambioFecha"></param>
        internal void CambiarFechaEntrega(COCambioFechaEntregaDC cambioFecha)
        {
            //1. Cambiar ADM_IdEstadoGuiaLog en AdmisionMensajeria_MEN. Si su ultimo Estado es Entregada, se quita el Bit en (ADM_EstaEntregada)
            using (SqlConnection cnn = new SqlConnection(CadCnxController))
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand("paInsertarCambioFechaEntrega_MEN", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@numeroGuia", cambioFecha.Guia.NumeroGuia);
                cmd.Parameters.AddWithValue("@fechaEntrega", cambioFecha.FechaEntrega);
                cmd.Parameters.AddWithValue("@fechaEntregaNew", cambioFecha.FechaNuevaEntrega);
                cmd.Parameters.AddWithValue("@fechaDigitalizacion", cambioFecha.Guia.FechaEstimadaDigitalizacion);
                cmd.Parameters.AddWithValue("@fechaArchivo", cambioFecha.Guia.FechaEstimadaArchivo);
                cmd.Parameters.AddWithValue("@creadoPor", ControllerContext.Current.Usuario);
                cmd.ExecuteNonQuery();
                cnn.Close();
            }

        }

        /// <summary>
        /// Metodo para afectar la fecha estimada de entrega a ayer
        /// </summary>
        /// <param name="cambioFecha"></param>
        public void CambiarFechaEntregaDiaAnterior(COCambioFechaEntregaDC cambioFecha)
        {
            using (SqlConnection cnn = new SqlConnection(CadCnxController))
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand("paInsertarCambioFechaEntregaDiaAnterior_MEN", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@numeroGuia", cambioFecha.Guia.NumeroGuia);
                cmd.Parameters.AddWithValue("@tiempoAfectacion", cambioFecha.TiempoAfectacion);
                cmd.Parameters.AddWithValue("@creadoPor", ControllerContext.Current.Usuario);
                cmd.ExecuteNonQuery();
                cnn.Close();
            }

        }

        /// <summary>
        /// Obtiene la fecha estimada de entrega new de determinada guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public CODatosCambioFechasDC ObtenerDatosParaCambiosDeFecha(long numeroGuia)
        {
            CODatosCambioFechasDC tiemposGuia = new CODatosCambioFechasDC();
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerDatosParaCambiosDeFecha_MEN", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@numeroGuia", numeroGuia);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    tiemposGuia.FechaEstimadaEntregaNew = Convert.ToDateTime(reader["ADM_FechaEstimadaEntregaNew"]);
                    tiemposGuia.Tiempos = new TATiempoDigitalizacionArchivo();
                    tiemposGuia.Tiempos.numeroDiasArchivo = Convert.ToDouble(reader["STR_TiempoDigitalizacion"]);
                    tiemposGuia.Tiempos.numeroDiasDigitalizacion = Convert.ToDouble(reader["STR_TiempoArchivo"]);
                }
                conn.Close();
            }
            return tiemposGuia;
        }
        #endregion

        #region Motivos




        /// <summary>
        /// Metodo para obtener los motivos asociados a un tipo de motivo de una guía
        /// </summary>
        /// <param name="tipoMotivo">enumeracion de tipos de motivos posibles </param>
        /// <returns> lista de motivos guia</returns>
        public IList<ADMotivoGuiaDC> ObtenerMotivosGuias(ADEnumTipoMotivoDC tipoMotivo)
        {
            IList<ADMotivoGuiaDC> listaMotivos = new List<ADMotivoGuiaDC>();
            using (SqlConnection cnn = new SqlConnection(CadCnxController))
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerMotivosGuia_MEN", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@tipoMotivo", (short)tipoMotivo);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    listaMotivos.Add(new ADMotivoGuiaDC
                    {
                        Descripcion = reader["MOG_Descripcion"].ToString(),
                        MotivoCRC = (Convert.ToInt16(reader["MOG_IdMotivoGuiaCRC"])),
                        EsVisible = (Convert.ToBoolean(reader["MOG_EsVisible"])),
                        SeReporta = (Convert.ToBoolean(reader["MOG_SeReporta"])),
                        IdMotivoGuia = (Convert.ToInt16(reader["MOG_IdMotivoGuia"])),
                        EsEscaneo = (Convert.ToBoolean(reader["MOG_SolicitaEscaneo"])),
                        Tipo = (ADEnumTipoMotivoDC)((Convert.ToInt16(reader["MOG_IdTipoMotivoGuia"]))),
                        CausaSupervision = (Convert.ToBoolean(reader["MOG_CausaSupervision"])),
                        @namespace = reader["MOG_Namespace"].ToString(),
                        nombreAssembly = reader["MOG_NombreAssembly"].ToString(),
                        nombreClase = reader["MOG_NombreClase"].ToString(),
                        TiempoAfectacion = (Convert.ToInt16(reader["MOG_TiempoAfectacion"])),
                        IntentoEntrega = (Convert.ToBoolean(reader["MOG_IntentoEntrega"])),
                        ObservacionMotivo = DBNull.Value.Equals(reader["MOG_ObservacionMotivo"]) ? string.Empty : reader["MOG_ObservacionMotivo"].ToString(),
                        CapturaPredio = DBNull.Value.Equals(reader["MOG_CapturaPredio"]) ? false : (Convert.ToBoolean(reader["MOG_CapturaPredio"])),
                        CapturaContador = DBNull.Value.Equals(reader["MOG_CapturaContador"]) ? false : (Convert.ToBoolean(reader["MOG_CapturaContador"])),
                        CapturaObservacion = DBNull.Value.Equals(reader["MOG_CapturaObservacion"]) ? false : (Convert.ToBoolean(reader["MOG_CapturaObservacion"]))
                    });
                }
            }
            return listaMotivos;
        }



        /// <summary>
        /// Método para obtener el tipo de evidencia segun el motivo y el intento de entrega
        /// </summary>
        /// <param name="motivo"></param>
        /// <param name="intentoEntrega"></param>
        /// <returns></returns>
        public LITipoEvidenciaDevolucionDC ObtenerMotivosEvidencia(ADMotivoGuiaDC motivo, short intentoEntrega)
        {

            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerMotivoEvidencia_MEN", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idMotivo", (short)motivo.IdMotivoGuia);
                cmd.Parameters.AddWithValue("@intentoEntrega", intentoEntrega);
                cmd.ExecuteNonQuery();
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                sqlConn.Close();
                if (dt.Rows.Count != 0)
                {
                    return new LITipoEvidenciaDevolucionDC
                    {
                        IdTipo = Convert.ToInt16(dt.Rows[0]["MED_IdTipoEvidenciaDevolucion"]),
                        Descripcion = dt.Rows[0]["TIV_Descripcion"].ToString()
                    };
                }
                else
                    throw new Exception("El motivo no tiene configurada el tipo de volante");
            }
        }


        /// <summary>
        /// Método para obtener los tipos de evidencia de mensajeria
        /// </summary>
        /// <returns></returns>
        public IList<LITipoEvidenciaDevolucionDC> ObtenerTiposEvidencia()
        {

            IList<LITipoEvidenciaDevolucionDC> tiposEvidencia = new List<LITipoEvidenciaDevolucionDC>();
            using (SqlConnection cnn = new SqlConnection(CadCnxController))
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerTipoEvidenciaDevolucion_MEN", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    tiposEvidencia.Add(new LITipoEvidenciaDevolucionDC
                    {
                        Descripcion = reader["TIV_Descripcion"].ToString(),
                        IdTipo = (Convert.ToInt16(reader["TIV_IdTipoEvidenciaDevolucion"])),
                        IdSuministro = (Convert.ToInt32(reader["TIV_IdSuministro"])),
                    });
                }
            }
            return tiposEvidencia;
        }


        /// <summLLary>
        /// Metodo encargado de validar la transicion de estado, a partir de la escogencia de un motivo
        /// </summary>
        /// <param name="motivo">motivo escogido para realizar cambio</param>
        /// <param name="estado">enumeracion de estados de la guía</param>
        /// <returns></returns>
        public IList<ADEnumEstadoGuia> ValidarEstadoMotivo(ADMotivoGuiaDC motivo, ADEnumEstadoGuia estado)
        {
            IList<ADEnumEstadoGuia> estadosGuia = new List<ADEnumEstadoGuia>();
            using (SqlConnection cnn = new SqlConnection(CadCnxController))
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerMotivoGuiaTransicionEstado_MEN", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdEstadoGuiaDesde", (int)(estado));
                cmd.Parameters.AddWithValue("@IdMotivoGuia", motivo.IdMotivoGuia);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    estadosGuia.Add((ADEnumEstadoGuia)Enum.Parse(typeof(ADEnumEstadoGuia), reader["MGT_IdEstadoGuiaHasta"].ToString()));
                }
            }
            return estadosGuia;
        }

        /// <summary>
        /// Consulta el estado gestion devolucion o entrega y segun el caso adiciona el motivo
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ADTrazaGuia ObtenerEstadoGestion(long numeroGuia)
        {
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerEstadoGestion_MEN", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroGuia", numeroGuia);
                cmd.ExecuteNonQuery();
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                sqlConn.Close();
                if (dt.Rows.Count != 0)
                    return new ADTrazaGuia
                    {
                        NumeroGuia = Convert.ToInt64(dt.Rows[0]["ADM_NumeroGuia"]),
                        IdTrazaGuia = Convert.ToInt64(dt.Rows[0]["ADM_IdEstadoGuiaLog"]),
                        IdEstadoGuia = Convert.ToInt16(dt.Rows[0]["EGT_IdEstadoGuia"]),
                        DescripcionEstadoGuia = dt.Rows[0]["EGT_DescripcionEstado"].ToString(),
                        IdCiudad = dt.Rows[0]["EGT_IdLocalidad"].ToString(),
                        Ciudad = dt.Rows[0]["EGT_NombreLocalidad"].ToString(),
                        FechaGrabacion = Convert.ToDateTime(dt.Rows[0]["EGT_FechaGrabacion"]),
                        Usuario = dt.Rows[0]["EGT_CreadoPor"].ToString(),
                    };
                else
                    return null;
            }
        }
        /// <summary>
        /// onsulta el motivo de devolucion de acuerdo a el log
        /// </summary>
        /// <param name="idTrazaGuia"></param>
        /// <returns></returns>
        public ADMotivoGuiaDC ObtenerMotivoGestion(long idTrazaGuia)
        {
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerMotivoDevolucionTraza_MEN", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idTraza", idTrazaGuia);
                cmd.ExecuteNonQuery();
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                sqlConn.Close();
                if (dt.Rows.Count != 0)
                    return new ADMotivoGuiaDC
                    {
                        IdMotivoGuia = Convert.ToInt16(dt.Rows[0]["MOG_IdMotivoGuia"]),
                        Descripcion = dt.Rows[0]["MOG_Descripcion"].ToString(),
                    };
                else
                    return null;
            }
        }
        #endregion

        #region Sispostal - Masivos

        /// <summary>
        /// Metodo para traer los motivos de devoluicion en Sispostal
        /// </summary>
        /// <returns></returns>
        /// <returns> lista de motivos de devolucion</returns>
        public IList<ADMotivoGuiaDC> ObtenerMotivosDevolucionGuiasMasivos()
        {
            IList<ADMotivoGuiaDC> listaMotivos = new List<ADMotivoGuiaDC>();
            using (SqlConnection cnn = new SqlConnection(CadCnxSispostalController))
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand("Sp_obtenerMotivos", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (Convert.ToInt16(reader["CODMOT"]) != 100)
                    {
                        listaMotivos.Add(new ADMotivoGuiaDC
                        {
                            Descripcion = reader["MOTIVO"].ToString(),
                            IdMotivoGuia = (Convert.ToInt16(reader["CODMOT"]))
                        });
                    }
                }
            }
            return listaMotivos;
        }

        /// <summary>
        /// Obtiene la traza del ultimo estado de la guia x numero de guia
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <returns></returns>
        public ADTrazaGuia ObtenerTrazaUltimoEstadoXNumGuiaMasivos(long numeroGuia)
        {
            ADTrazaGuia traza = new ADTrazaGuia();
            using (SqlConnection cnn = new SqlConnection(CadCnxSispostalController))
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand("CON_GUIA", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Guia", numeroGuia);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    traza.NumeroGuia = DBNull.Value == reader["Guia"] ? 0 : Convert.ToInt64(reader["Guia"]);
                    traza.IdEstadoGuia = DBNull.Value == reader["estado"] ? (short)0 : Convert.ToInt16(reader["estado"]);
                    traza.IdCiudad = reader["Ciudad"].ToString();
                    traza.IdCentroServicioEstado = DBNull.Value == reader["id_mov_suc"] ? 0 : (Convert.ToInt64(reader["id_mov_suc"]));
                    traza.NombreCentroServicioEstado = reader["SUCURSAL"].ToString();
                    if (reader["fechentre"] != DBNull.Value)
                    {
                        traza.FechaEntrega = Convert.ToDateTime(reader["fechentre"]);
                    }
                }
            }
            return traza;
        }

        /// <summary>
        /// Guarda la traza de la guia ingresada con fecha de estado
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="estadoGuia"></param>
        internal void ActualizarEntregadoGuiaMasivos(ADTrazaGuia guia, bool intentoEntrega)
        {
            using (SqlConnection cnn = new SqlConnection(CadCnxSispostalController))
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand("ENTREGA_A", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Guia", guia.NumeroGuia);
                cmd.Parameters.AddWithValue("@idemp", 1);
                cmd.Parameters.AddWithValue("@RECIBE", "AppMovil");
                cmd.Parameters.AddWithValue("@FECHENTRE", guia.FechaGrabacion.Year != 1 ? guia.FechaGrabacion : DateTime.Now);
                cmd.Parameters.AddWithValue("@ESTADO", guia.IdNuevoEstadoGuia.ToString());
                cmd.Parameters.AddWithValue("@diasm", 0);
                cmd.Parameters.AddWithValue("@operario", guia.Usuario);
                cmd.Parameters.AddWithValue("@aclaracion", guia.Observaciones == null ? string.Empty : guia.Observaciones);
                cmd.Parameters.AddWithValue("@movil", 1);
                cmd.Parameters.AddWithValue("@intentoEntrega", intentoEntrega);
                cmd.ExecuteNonQuery();
                cnn.Close();
            }
        }

        internal void DevolucionGuiaMasivos(ADTrazaGuia guia)
        {
            using (SqlConnection cnn = new SqlConnection(CadCnxSispostalController))
            {
                cnn.Open();
                /*************************** Procedimiento creado en SISPOSTAL para la devolucón *****/
                SqlCommand cmd = new SqlCommand("DEVOLUCION_ACLARA1", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Guia", guia.NumeroGuia);
                cmd.Parameters.AddWithValue("@idemp", 1);
                cmd.Parameters.AddWithValue("@Tipodevo", guia.MotivoGuia.IdMotivoGuia.ToString());
                cmd.Parameters.AddWithValue("@NOVEDAD", guia.MotivoGuia.Descripcion);
                cmd.Parameters.AddWithValue("@fechentre", DateTime.Now);
                cmd.Parameters.AddWithValue("@estado", guia.IdNuevoEstadoGuia.ToString());
                cmd.Parameters.AddWithValue("@diasm", 0);
                cmd.Parameters.AddWithValue("@ACLARACION_DEV", guia.Observaciones == null ? string.Empty : guia.Observaciones);
                cmd.Parameters.AddWithValue("@dia_a_des", 0);
                cmd.Parameters.AddWithValue("@operario", guia.Usuario);
                cmd.ExecuteNonQuery();
                cnn.Close();
            }
        }

        #endregion
    }
}