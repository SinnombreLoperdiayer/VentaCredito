using CO.Servidor.Servicios.ContratoDatos.Integraciones;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CO.Controller.Servidor.Integraciones.AccesoDatos
{
  public class DALMensajesTexto
  {
    public static readonly DALMensajesTexto Instancia = new DALMensajesTexto();
    public Dictionary<string, string> Parametros = new Dictionary<string, string>();

        /// <summary>
        /// Contiene información de los mensajes para los clientes crédito que tengan habilitado el envío
        /// </summary>
        public List<INTMensajeTextoDC> MensajesClientes = new List<INTMensajeTextoDC>();
        private string ConnStrinTransac;

        public DALMensajesTexto()
        {
            ConnStrinTransac = System.Configuration.ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;
            using(SqlConnection cnx = new SqlConnection(ConnStrinTransac))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand("SELECT PAR_IdParametro,PAR_ValorParametro FROM ParametrosFramework with(Nolock) where PAR_IdParametro in ('UserMensajesTexto', 'UriMensajesTexto', 'PassMensajesTexto')", cnx);
                cmd.CommandType = System.Data.CommandType.Text;
                SqlDataReader lector = cmd.ExecuteReader();
                while(lector.Read())
                {
                    Parametros.Add(lector["PAR_IdParametro"].ToString(), lector["PAR_ValorParametro"].ToString());
                }
                lector.Close();

        // Si no se encuentran parámetros, ponga los siguientes parámetros por defecto.
        if (Parametros.Count < 3)
        {
          if (!Parametros.ContainsKey("UserMensajesTexto"))
          {
            Parametros.Add("UserMensajesTexto", "CSMSINTRP");
          }
          if (!Parametros.ContainsKey("UriMensajesTexto"))
          {
            Parametros.Add("UriMensajesTexto", "http://sms1.signacom.com.co:9090/SignacomWebServer/SendMessage?user={0}&password={1}&destination={2}&message={3}");
          }
          if (!Parametros.ContainsKey("PassMensajesTexto"))
          {
            Parametros.Add("PassMensajesTexto", "QnXEChjx");
          }
        }

                // Obtener formatos de mensajes por clientes crédito
                cmd = new SqlCommand("SELECT CLI_IdCliente,CLI_Formato , CLI_IdServicio FROM FormatoMsjTextoCliente_CLI WITH(NOLOCK)", cnx);
                cmd.CommandType = System.Data.CommandType.Text;
                lector = cmd.ExecuteReader();
                while (lector.Read())
                {
                    MensajesClientes.Add(
                        new INTMensajeTextoDC()
                        {
                            IdCliente = int.Parse(lector["CLI_IdCliente"].ToString()),
                            Mensaje = lector["CLI_Formato"].ToString(),
                            IdServicio = short.Parse(lector["CLI_IdServicio"].ToString()),
                        });
                }

                cnx.Close();
                cnx.Dispose();
            }
        }

        /// <summary>
        /// Inserta la fecha en la que se ejecuto el motor
        /// </summary>
        public void InsertarFechaEjecucion()
        {
            using(SqlConnection conn = new SqlConnection(ConnStrinTransac))
            {
                SqlCommand cmd = new SqlCommand("paInsertarEjecucionMotorMensajesTexto", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }            
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime ObtenerFechaUltimaEjecucionMotorMensajesTexto()
        {
            DateTime fechaUltimaEjecucion = new DateTime();
            using (SqlConnection conn = new SqlConnection(ConnStrinTransac))
            {
                SqlCommand cmd = new SqlCommand("paConsultarUltimaEjecucionMotroMsj", conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    fechaUltimaEjecucion = Convert.ToDateTime(reader["MNE_FechaGrabacion"]);
                }
                conn.Close();  
            }
            return fechaUltimaEjecucion;
        }

        /// <summary>
        /// A ctualiza el estado como enviado del mensaje de texto
        /// </summary>
        public void ActualizarEstadoMensajeTexto(int idMensaje)
        {
            using (SqlConnection conn = new SqlConnection(ConnStrinTransac))
            {
                SqlCommand cmd = new SqlCommand("paModificarEstadoMensajesNoEnviados", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdMensaje", idMensaje);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        /// <summary>
        /// Obtiene y envia mensajes que estan pendientes de enviar
        /// </summary>
        public List<MensajeTextoDC> ObtenerMensajesPendientes()
        {
            List<MensajeTextoDC> lstMensajes = new List<MensajeTextoDC>();
            using (SqlConnection conn = new SqlConnection(ConnStrinTransac))
            {
                SqlCommand cmd = new SqlCommand("paConsultarMensajesNoEnviados", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    MensajeTextoDC mensaje = new MensajeTextoDC()
                    {
                        IdMensajeNoEnviado = Convert.ToInt32(reader["MNE_IdMensaje"]),
                        Mensaje = reader["MNE_Mensaje"].ToString(),
                        NumeroCelular = reader["MNE_NumeroCelular"].ToString()
                    };
                    lstMensajes.Add(mensaje);
                }
                conn.Close();
            }
            return lstMensajes;
        }

        /// <summary>
        /// Método para obtener mensaje de texto de cliente
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="idServicio"></param>
        /// <returns></returns>
        internal string ObtenerMensajeTextoCliente(int idCliente , int idServicio)
        {
            string mensaje = string.Empty;
            mensaje = MensajesClientes.FirstOrDefault(men => men.IdCliente == idCliente && men.IdServicio == idServicio).Mensaje;
            return mensaje;
        }

        public string ObtenerMensajeTexto(string mensaje)
        {
            string result = "";
            using (SqlConnection con = new SqlConnection(ConnStrinTransac))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("paObtenerFormatoMensajeTexto_MEN", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@idMensaje", mensaje));
                result = (string)cmd.ExecuteScalar();
            }

            return result;
        }
        internal string MensajesNoCliente(string mensaje)
        {
            string result = "";
            using(SqlConnection con = new SqlConnection(ConnStrinTransac))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("paObtenerFormatoMensajeTexto_Men", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@idMensaje", mensaje));
                result = (string)cmd.ExecuteScalar();
            }

            return result;
        }

        internal bool HayMensajeEnviadoAdmision(long idAdmision)
        {
            int result = 0;
            using(SqlConnection con = new SqlConnection(ConnStrinTransac))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("paValidarExisteAdmisionEnvioMsjTexto_MEN", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@idAdmision", idAdmision));
                result = (int)cmd.ExecuteScalar();
            }

            return (result == 1);
        }

        internal void InsertarMensajeEnviado(long idAdmision, long numeroGuia, string numeroCelular, string numeroPedido, int idcliente)
        {
            using(SqlConnection cnx = new SqlConnection(ConnStrinTransac))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand("paInsertaMensajeTextoEnviado_MEN", cnx);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdAdminisionMensajeria ", idAdmision);
                cmd.Parameters.AddWithValue("@NumeroGuia			 ", numeroGuia);
                cmd.Parameters.AddWithValue("@NumCelular             ", numeroCelular);
                cmd.Parameters.AddWithValue("@NoPedido				 ", numeroPedido);
                cmd.Parameters.AddWithValue("@IdClienteCredito       ", idcliente);
                cmd.Parameters.AddWithValue("@CreadoPor				 ", "SISTEMA");
                cmd.ExecuteNonQuery();
                cnx.Dispose();
            }
        }


        /// <summary>
        /// Retorna el id de cliente remitente de la guía
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="numPedido"></param>
        /// <returns></returns>
        public int? ConsultarInfoClienteRemitenteGuiaYTiempoEntrega(long numeroGuia, ref string numPedido, ref string fechaEntrega, ref string numCelular)
        {
        using(SqlConnection cnx = new SqlConnection(ConnStrinTransac))
        {
            int? idConvenio = null;
            cnx.Open();
            SqlCommand cmd = new SqlCommand("paObtenerInfoGuiaRemitenteTiempoEntrega_MEN", cnx);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("ADM_NumeroGuia", numeroGuia));
            SqlDataReader lector = cmd.ExecuteReader();
            if(lector.Read())
            {
                numPedido = lector["NumPedido"].ToString();
                int id = 0;
                if(int.TryParse(lector["IdCliente"].ToString(), out id))
                {
                    idConvenio = id;
                }
                fechaEntrega = lector["FechaSalida"].ToString();
                numCelular = lector["NumCelular"].ToString();
            }
            cnx.Close();
            cnx.Dispose();
            return idConvenio;
        }
        }

        /// <summary>
        /// Indica que un mensaje de texto ha sido enviado al cliente
        /// </summary>
        /// <param name="numGuia"></param>
        internal void ActualizarMensajeEnviado(long numGuia)
        {
            using(SqlConnection cnx = new SqlConnection(ConnStrinTransac))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand("paMensajeTextoEnviado_MEN", cnx);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("numGuia", numGuia));
                cmd.ExecuteNonQuery();
                cnx.Close();
                cnx.Dispose();
            }
        }
    }
}
