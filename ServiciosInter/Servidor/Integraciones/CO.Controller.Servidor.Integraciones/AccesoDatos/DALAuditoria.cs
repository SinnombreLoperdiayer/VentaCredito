using CO.Servidor.Servicios.ContratoDatos.Integraciones;
using Framework.Servidor.Excepciones;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CO.Controller.Servidor.Integraciones.AccesoDatos
{   
    public class DALAuditoria
    {
        private string CadCnxController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;
        public static readonly DALAuditoria Instancia = new DALAuditoria();

        public DALAuditoria()
        {

        }

        /// <summary>
        /// Graba auditoria de integración
        /// </summary>
        /// <param name="tipoIntegracion"></param>
        /// <param name="usuario"></param>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public void AuditarIntegracion(string tipoIntegracion, string request, string response)
        {
            using (SqlConnection cnx = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ControllerExcepciones"].ConnectionString))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand("paAuditarIntegracion_AUD", cnx);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("AUI_TipoIntegraciOn", tipoIntegracion));
                cmd.Parameters.Add(new SqlParameter("AUI_Request", request));
                cmd.Parameters.Add(new SqlParameter("AUI_Response", response));
                cmd.ExecuteNonQuery();
                cnx.Close();
                cnx.Dispose();
            }
        }

        /// <summary>
        /// Obtiene el horario del dia solicitado
        /// </summary>
        /// <param name="dayOfWeek"></param>
        public HorarioMensajesTextoDC ObtenerHorarioParaMensajes(int dia)
        {
            HorarioMensajesTextoDC horario = new HorarioMensajesTextoDC();
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerHorarioMensajesPorDia", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idDia",dia);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    horario.HoraInicio = Convert.ToDateTime(reader["HPM_HoraInicio"]);
                    horario.HoraFin = Convert.ToDateTime(reader["HPM_HoraFin"]);
                }
                conn.Close();
            }
            return horario;
        }

        /// <summary>
        /// Inserta los mensajes que no fueron enviados por estar fuera del horario permitido
        /// </summary>
        /// <param name="numeroCelular"></param>
        /// <param name="mensaje"></param>
        public void InsertarMensajesNoEnviados(string numeroCelular, string mensaje)
        {
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paInsertarMensajesNoEnviados", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroCelular", numeroCelular);
                cmd.Parameters.AddWithValue("@Mensaje", mensaje);
                cmd.Parameters.AddWithValue("@CreadoPor", "Sistema"); //ControllerContext.Current.Usuario
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }
    }
}
