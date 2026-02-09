using CO.Servidor.Servicios.ContratoDatos.Integraciones;
using Framework.Servidor.Comun;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CO.Controller.Servidor.Integraciones.AccesoDatos.DALIntegraciones
{
    public class INRepositorioIntegraciones
    {
        private string conexionStringController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;

        private static readonly INRepositorioIntegraciones instancia = new INRepositorioIntegraciones();
        public static INRepositorioIntegraciones Instancia
        {
            get
            {
                return instancia;
            }
        }

        private INRepositorioIntegraciones()
        {
        }

        #region Metodos

        public int ValidarCredencial(credencialDTO credencial)
        {
            string usuario = Encoding.UTF8.GetString(Convert.FromBase64String(credencial.usuario));
            string clave = Encoding.UTF8.GetString(Convert.FromBase64String(credencial.clave));
            //string usuario = credencial.usuario;
            //string clave = credencial.clave;
            clave = COEncripcion.ObtieneHash(clave);
            int idCliente = 0;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                
                  SqlCommand cmd = new SqlCommand("paConsultarUsuarioIntegracion_SEG", sqlConn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Usuario", usuario);
                    cmd.Parameters.AddWithValue("@Contrasena", clave);
                    sqlConn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        idCliente = Convert.ToInt32(reader["USI_IdCliente"]);
                    }
                    sqlConn.Close();
                    
                }
            return idCliente;
        }

        #endregion
        #region ConsultaMovimientosYanbal
        /// <summary>
        /// Metodo que llama al procedimiento almacenado y mapea en una lista de Estados y motivos homologados
        /// </summary>
        /// <returns>Lista con Estados y motivos homologados </returns>
        public List<ADEstadoHomologacionYanbal> ConsultarMovimientosYanbal()
        {
            List<ADEstadoHomologacionYanbal> Respuesta = new List<ADEstadoHomologacionYanbal>();
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paGenerarArchivoYanbalHomologacion_MEN", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 180;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ADEstadoHomologacionYanbal EstadoGuia = new ADEstadoHomologacionYanbal();
                    EstadoGuia.ADM_DescripcionEstado = reader["ADM_DescripcionEstado"].ToString();
                    EstadoGuia.ADM_FechaAdmision = Convert.ToDateTime(reader["ADM_FechaAdmision"]);
                    EstadoGuia.ADM_IdDestinatario = reader["ADM_IdDestinatario"].ToString();
                    EstadoGuia.ADM_NoPedido = reader["ADM_NoPedido"].ToString();
                    EstadoGuia.ADM_NumeroBolsaSeguridad = reader["ADM_NumeroBolsaSeguridad"].ToString();
                    EstadoGuia.ADM_NumeroGuia = reader["ADM_NumeroGuia"].ToString();
                    EstadoGuia.ADM_Observaciones = reader["ADM_Observaciones"].ToString();
                    EstadoGuia.CES_Nombre = reader["CES_Nombre"].ToString();
                    EstadoGuia.EGT_FechaGrabacion = Convert.ToDateTime(reader["EGT_FechaGrabacion"]);
                    EstadoGuia.HY_CodEventoYanbal = reader["HY_CodEventoYanbal"].ToString();
                    EstadoGuia.HY_DescripcionYanbal = reader["HY_DescripcionYanba"].ToString();
                    EstadoGuia.HY_TipoEventoYanbal = reader["HY_TipoEventoYanbal"].ToString();
                    EstadoGuia.HY_MotivoYanbal = reader["HY_MotivoYanbal"].ToString();
                    EstadoGuia.HY_DescripcionMotivo = reader["HY_DescripcionMotivo"].ToString();
                    Respuesta.Add(EstadoGuia);
                }
            }
            return Respuesta;

        }
        /// <summary>
        /// Consulta en Parametros Framework la frecuencia de ejecucion de la consola
        /// </summary>
        /// <returns>Numero de minutos </returns>
        public int FrecuenciaEjecucion() {
            int Respuesta = 0;
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerParametrosFramework_PAR", conn);
                cmd.Parameters.AddWithValue("@PAR_IdParametro", "FrecEnvioEventYanbal"); 
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ADEstadoHomologacionYanbal EstadoGuia = new ADEstadoHomologacionYanbal();
                    Respuesta = Convert.ToInt32(reader["PAR_ValorParametro"]);
                }
            }
            return Respuesta;
        }
        /// <summary>
        /// Acutaliza la fecha de la ultima ejecucion
        /// </summary>
        /// <param name="valor"></param>
        public int ActualizarParametroIntegracionYanbal()
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paActualizarParametroFrmIntegracionYanbal_PAR", conn);
                cmd.Parameters.AddWithValue("@PAR_IdParametro", "UltEjecYanbal");                
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
            }
            return 0;
        }
        #endregion

    }
}
