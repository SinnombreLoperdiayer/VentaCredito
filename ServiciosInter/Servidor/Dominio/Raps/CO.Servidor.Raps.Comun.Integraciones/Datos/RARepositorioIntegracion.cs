using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.ServiceModel;
using CO.Servidor.Raps.Comun.Integraciones.Datos.Datos;
using CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes;
using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;

namespace CO.Servidor.Raps.Comun.Integraciones.Datos
{
    internal class RARepositorioIntegracion
    {

        private static readonly RARepositorioIntegracion instancia = new RARepositorioIntegracion();

        internal static RARepositorioIntegracion Instancia
        {
            get
            {
                return instancia;
            }
        }


        string conStr = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;
        private string conexionStringRaps = ConfigurationManager.ConnectionStrings["rapsTransaccional"].ConnectionString;

        private RARepositorioIntegracion()
        {

        }


        /// <summary>
        /// Consulta el parametro de la url de controller api
        /// </summary>
        /// <returns></returns>
        internal string ObtenerUrlApi()
        {

            using (SqlConnection conn = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("paObtenerParametrosFramework_PAR", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PAR_IdParametro", "UrlControllerApi");
                conn.Open();
                var parametro = cmd.ExecuteScalar();
                conn.Close();

                if (parametro != null)
                {
                    return Convert.ToString(parametro);
                }
                else
                {

                    throw new FaultException<Exception>(new Exception("No existe el parametro 'UrlControllerApi'"));
                }
            }
        }

        /// <summary>
        /// Obtiene los parametros por tipo de integracion
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public List<LIParametrizacionIntegracionRAPSDC> ObtenerParametrosPorIntegracion(string tipoParametro)
        {
            List<LIParametrizacionIntegracionRAPSDC> lstParametros = new List<LIParametrizacionIntegracionRAPSDC>();
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paObtenerParametrosIntegracion_RAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Tipo", tipoParametro);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    LIParametrizacionIntegracionRAPSDC parametro = new LIParametrizacionIntegracionRAPSDC
                    {
                        NombreParametro = reader["MPI_NombreParametro"].ToString(),
                        IdParametro = Convert.ToInt32(reader["MPI_IdParametroparametrizacion"]),
                        TipoDato = Convert.ToInt32(reader["IdTipoDato"]),
                        Longitud = Convert.ToInt32(reader["Longitud"]),
                        DescripcionParametro = reader["DescripcionParametro"].ToString(),
                        ClaveTipoFalla = reader["MPI_ModuloParametro"].ToString(),
                        EsArray = reader["MPI_EsArray"] != DBNull.Value ? Convert.ToBoolean(reader["MPI_EsArray"]) : false,
                        NombreObjeto = reader["MPI_Objeto"] != DBNull.Value ? reader["MPI_Objeto"].ToString() : "",
                        NombrePropiedad = reader["MPI_Propiedad"] != DBNull.Value ? reader["MPI_Propiedad"].ToString() : "",
                        PosicionEnArray = reader["MPI_PosicionEnArray"] != DBNull.Value ? Convert.ToInt32(reader["MPI_PosicionEnArray"]) : 0

                    };
                    lstParametros.Add(parametro);
                }
                conn.Close();
            }
            return lstParametros;
        }

        /// <summary>
        /// Obtiene los parametros por id novedad
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public List<LIParametrizacionIntegracionRAPSDC> ObtenerParametrosPorIntegracionPorNovedad(int idNovedad, int origenRaps)
        {
            List<LIParametrizacionIntegracionRAPSDC> lstParametros = new List<LIParametrizacionIntegracionRAPSDC>();
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paObtenerParametrosIntegracionPorIdNovedad_RAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdNovedad", idNovedad);
                cmd.Parameters.AddWithValue("@IdOrigenRaps", origenRaps);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    lstParametros = MapperRepositorioIntegracion.ToParametrizacionReglasRaps(reader);
                }
                conn.Close();
            }
            return lstParametros;
        }

        /// <summary>
        /// Obtener parametros fallas personalizadas
        /// </summary>
        /// <param name="idTipoNovedad"></param>
        /// <returns></returns>

        public List<LIParametrizacionIntegracionRAPSDC> ListaParametrosPersonalizacionPorNovedad(int idTipoNovedad)
        {
            List<LIParametrizacionIntegracionRAPSDC> resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerParametrosNovedadPersonalizada_RAP", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdTipoNovedad", idTipoNovedad));

                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    resultado = new List<LIParametrizacionIntegracionRAPSDC>();
                    while (reader.Read())
                    {
                        resultado.Add(new LIParametrizacionIntegracionRAPSDC()
                        {
                            IdTipoNovedad = reader["IdTipoNovedad"] == DBNull.Value ? 0 : Convert.ToInt32(reader["IdTipoNovedad"]),
                            IdParametro = reader["IdParametro"] == DBNull.Value ? 0 : Convert.ToInt32(reader["IdParametro"]),
                            IdFuncion = reader["IdFuncion"] == DBNull.Value ? 0 : Convert.ToInt32(reader["IdFuncion"]),
                            EsAgrupamiento = Convert.ToBoolean(reader["EsAgrupamiento"]),
                        });
                    }
                }
            }
            return resultado;
        }

        internal RAInformacionUsuarioRAP ObtenerInformacionUsuarioPorIdentificacion(string identificacion)
        {
            RAInformacionUsuarioRAP resultado = null;
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paObtenerInformacionUsuarioPorIdentificacion_RAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Identificacion", identificacion);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                resultado = MapperRepositorioIntegracion.ToRaInformacionUsuarioRAP(reader);
            }

            return resultado;
        }


        #region IntegracionesManualRaps


        /// <summary>
        /// Obtiene los parametros por tipo de integracion
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public List<LIParametrizacionIntegracionRAPSDC> ObtenerParametrosPorTipoNovedad(string tipoParametro)
        {
            List<LIParametrizacionIntegracionRAPSDC> lstParametros = new List<LIParametrizacionIntegracionRAPSDC>();
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paObtenerParametrosIntegracion_RAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Tipo", tipoParametro);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    LIParametrizacionIntegracionRAPSDC parametro = new LIParametrizacionIntegracionRAPSDC
                    {
                        NombreParametro = reader["MPI_NombreParametro"].ToString(),
                        IdParametro = Convert.ToInt32(reader["MPI_IdParametroparametrizacion"]),
                        TipoDato = Convert.ToInt32(reader["IdTipoDato"]),
                        Longitud = Convert.ToInt32(reader["Longitud"]),
                        DescripcionParametro = reader["DescripcionParametro"].ToString(),
                        ClaveTipoFalla = reader["MPI_ModuloParametro"].ToString(),
                        EsArray = reader["MPI_EsArray"] != DBNull.Value ? Convert.ToBoolean(reader["MPI_EsArray"]) : false,
                        NombreObjeto = reader["MPI_Objeto"] != DBNull.Value ? reader["MPI_Objeto"].ToString() : "",
                        NombrePropiedad = reader["MPI_Propiedad"] != DBNull.Value ? reader["MPI_Propiedad"].ToString() : "",
                        PosicionEnArray = reader["MPI_PosicionEnArray"] != DBNull.Value ? Convert.ToInt32(reader["MPI_PosicionEnArray"]) : 0

                    };
                    lstParametros.Add(parametro);
                }
                conn.Close();
            }
            return lstParametros;
        }

        /// <summary>
        /// valida es unico envio para parametrización
        /// </summary>
        /// <param name="idTipoNovedad"></param>
        /// <returns></returns>
        public bool ValidarParametrizacionEsUnicoEnvio(int idTipoNovedad)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paValidarParametrizacionEsEnvioUnicoXIdNovedad_RAPS", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdNovedad", idTipoNovedad);
                conn.Open();
                var result = cmd.ExecuteScalar();

                return result == DBNull.Value ? false : (bool)result;
            }
        }

        public bool ValidarExisteSolicitudParaResponsable(int idParametroAgrupamiento, string valorParametro)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paValidarSolicutudExistePorDiaYResponsable_RAPS", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idParametroAgrupamiento", idParametroAgrupamiento);
                cmd.Parameters.AddWithValue("@valorParametro", valorParametro);
                conn.Open();
                int result = (int)cmd.ExecuteScalar();
                return result > 0 ? true : false;
            }
        }



        /// <summary>
        /// obtiene reglas integraciones manual por id estado guia
        /// </summary>
        /// <param name="idEstadoGuia"></param>
        /// <returns></returns>

        public RAReglasIngrecionesManualDC ObtenerReglasIntegracionesManual(int idEstadoGuia)
        {
            RAReglasIngrecionesManualDC regla = null;
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paObtenerReglaIntegracionManualRaps", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdEstadoGuia", idEstadoGuia);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();


                if (reader.HasRows)
                {
                    regla = MapperRepositorioIntegracion.MapperObtenerReglasIntegracionesManual(reader);
                }


                conn.Close();

                return regla;
            }


        }

        /// <summary>
        /// obtiene responsable tipos novedades generales
        /// </summary>
        /// <returns></returns>
        public List<RAResponsableTipoNovedadDC> ObtenerResponsableTipoNovedad(int idOrigen)
        {
            List<RAResponsableTipoNovedadDC> tiposNovedad = null;

            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paObtenerResponsableTipoNovedad_RAPS", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdOrigenRaps", idOrigen);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                tiposNovedad = MapperRepositorioIntegracion.MapperObtenerResponsableTipoNovedad(reader);

                conn.Close();
                return tiposNovedad;

            }

        }

        /// <summary>
        /// obtener parametros por id responsable y novedad padre
        /// </summary>
        /// <param name="idResponsable"></param>
        /// <param name="idNovedadPadre"></param>
        /// <returns></returns>
        public List<RAParametrosPersonalizacionRapsDC> ObtenerParametrosXIdResponsable(int idResponsable, int idNovedadPadre, int Ejecuta)
        {
            List<RAParametrosPersonalizacionRapsDC> parametros = null;

            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                parametros = new List<RAParametrosPersonalizacionRapsDC>();
                SqlCommand cmd = new SqlCommand("paConsultarParametrosVisibles_RAPS", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdResponsable", idResponsable);
                cmd.Parameters.AddWithValue("@IdNovedadPadre", idNovedadPadre);
                cmd.Parameters.AddWithValue("@Ejecuta", Ejecuta);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                parametros = MapperRepositorioIntegracion.MapperObtenerParametrosPorIdResponsable(reader);
                return parametros;
            }


        }

        /// <summary>
        /// obtener parametros por id  novedad padre
        /// </summary>
        /// <param name="idResponsable"></param>
        /// <param name="idNovedadPadre"></param>
        /// <returns></returns>
        public List<RAParametrosPersonalizacionRapsDC> ObtenerParametrosVisiblesGlobales(int idNovedadPadre)
        {
            List<RAParametrosPersonalizacionRapsDC> parametros = null;

            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                parametros = new List<RAParametrosPersonalizacionRapsDC>();
                SqlCommand cmd = new SqlCommand("paConsultarParametrosVisiblesGlobales_RAPS", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdNovedadPadre", idNovedadPadre);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                parametros = MapperRepositorioIntegracion.MapperObtenerParametrosPorIdResponsableGlobales(reader);
                return parametros;
            }


        }



        /// <summary>
        /// obtiene tipo novedad por reponsable y novedad padre
        /// </summary>
        /// <param name="idNovedadPadre"></param>
        /// <param name="idTipoResponsable"></param>
        /// <returns></returns>
        public int ObtieneTipoNovedad(int idNovedadPadre, int idTipoResponsable)
        {

            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paObtenerTipoNovedadRaps", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdNovedadPadre", idNovedadPadre);
                cmd.Parameters.AddWithValue("@IdTipoResponsable", idTipoResponsable);
                conn.Open();
                var result = cmd.ExecuteScalar();

                return result == null ? 0 : (int)result;
            }
        }


        #endregion







    }
}
