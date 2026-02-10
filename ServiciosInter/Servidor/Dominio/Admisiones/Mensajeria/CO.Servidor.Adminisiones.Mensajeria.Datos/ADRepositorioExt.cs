
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
namespace CO.Servidor.Adminisiones.Mensajeria.Datos
{
    public class ADRepositorioExt : ADRepositorio
    {
        #region Atributos        
        private string conexionStringController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;

        #endregion Atributos

        #region Instancia singleton de la clase

        private static readonly ADRepositorioExt instancia = new ADRepositorioExt();

        public static ADRepositorioExt Instancia
        {
            get
            {
                return instancia;
            }
        }

        #endregion Instancia singleton de la clase

        /// <summary>
        /// Obtiene la Lista de Guias a procesar por telemercadeo
        /// </summary>
        /// <param name="idEstadoGuia"></param>
        /// <param name="IdCentroServicioDestino"></param>
        /// <returns></returns>
        public List<ADTrazaGuia> ObtenerGuiasGestion(int idEstadoGuia, long IdCentroServicioDestino)
        {
            List<ADTrazaGuia> resultado = null;

            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerGuiasGestionTelemercadeoCOL_LOI", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdEstadoguia", idEstadoGuia);
                cmd.Parameters.AddWithValue("@IdCentroServicioDestino ", IdCentroServicioDestino);
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        if (resultado == null)
                        {
                            resultado = new List<ADTrazaGuia>();
                        }
                        resultado.Add(
                            new ADTrazaGuia
                            {
                                NumeroGuia = (long)reader["ADM_NumeroGuia"],
                                IdAdmision = (long)reader["ADM_IdAdminisionMensajeria"],
                                FechaGrabacion = (DateTime)reader["ADM_fechaGrabacionEstado"],
                                //Observaciones = (string)reader["Color"],               
                            }
                            );
                    }

                }
            }
            return resultado;
        }

        /// <summary>
        /// Obtiene la Lista de Guias a procesar por telemercadeo
        /// </summary>
        /// <param name="idEstadoGuia"></param>
        /// <param name="IdCentroServicioDestino"></param>
        /// <returns></returns>
        public List<ADTrazaGuiaAgencia> ObtenerGuiasGestionAgencias(int idEstadoGuia, long IdCol)
        {
            List<ADTrazaGuiaAgencia> resultado = null;

            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerGuiasGestionAgenciasTelemercadeoCOL_LOI", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdEstadoguia", idEstadoGuia);
                cmd.Parameters.AddWithValue("@IdCol", IdCol);
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        if (resultado == null)
                        {
                            resultado = new List<ADTrazaGuiaAgencia>();
                        }
                        resultado.Add(
                            new ADTrazaGuiaAgencia
                            {
                                NumeroGuia = (long)reader["NumeroGuia"],
                                IdEstadoGuia = (short)reader["IdEstadoGuia"],
                                IdAdmisionMensajeria = (long)reader["IdAdmisionMensajeria"],
                                FechaGrabacionEstado = (DateTime)reader["FechaGrabacionEstado"],
                                IdCiudadDestino = (string)reader["IdCiudadDestino"],
                                CiudadDestino = (string)reader["NombreCiudadDestino"]
                                //Observaciones = (string)reader["Color"],               
                            }
                            );
                    }

                }
            }
            return resultado;
        }
    }
}
