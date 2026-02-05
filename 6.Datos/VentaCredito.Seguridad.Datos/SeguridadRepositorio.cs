using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VentaCredito.Transversal;
using VentaCredito.Transversal.Entidades.Clientes;

namespace VentaCredito.Seguridad.Datos
{
    public class SeguridadRepositorio
    {
        private static SeguridadRepositorio instancia = new SeguridadRepositorio();

        private string conexionStringSeguridad;

        public static SeguridadRepositorio Instancia
        {
            get { return instancia; }
        }

        public SeguridadRepositorio()
        {
            conexionStringSeguridad = ConfigurationManager.ConnectionStrings["ConnectionSeguridad"].ConnectionString;
        }

        public Autenticacion ValidarAutorizacionServicio()
        {
            var autenticacion = new Autenticacion();
            using (SqlConnection sqlconn = new SqlConnection(conexionStringSeguridad))
            {
                SqlCommand cmd = new SqlCommand(@"pa_ObtenerAutorizacionMetodoServicioTercero_SEG", sqlconn);
                cmd.CommandType = CommandType.StoredProcedure;                
                cmd.Parameters.AddWithValue("@IdUsuario", ContextoSitio.Current.Usuario);
                cmd.Parameters.AddWithValue("@Metodo", ContextoSitio.Current.Metodo);
                cmd.Parameters.AddWithValue("@Token", ContextoSitio.Current.Token);

                sqlconn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        autenticacion = new Autenticacion
                        {
                            ErrorAutenticacion = reader["SERMET_Metodo"] != DBNull.Value ? Convert.ToString(reader["SERMET_Metodo"]) : string.Empty,
                            EstaAutorizado = true
                        };
                    }
                }
                else
                {
                    autenticacion = new Autenticacion
                    {
                        ErrorAutenticacion = "El cliente no se encuentra autorizado para consumir el servicio solicitado",
                        EstaAutorizado = true
                    };
                }
                sqlconn.Close();
            }
            return autenticacion;
        }

        public Autenticacion ValidarUsuarioServicio()
        {
            var autenticacion = new Autenticacion();
            using (SqlConnection sqlconn = new SqlConnection(conexionStringSeguridad))
            {
                SqlCommand cmd = new SqlCommand(@"pa_ObtenerAutorizacionServicioTercero_SEG", sqlconn);
                cmd.CommandType = CommandType.StoredProcedure;                
                cmd.Parameters.AddWithValue("@IdUsuario", ContextoSitio.Current.Usuario);
                cmd.Parameters.AddWithValue("@Clave", ContextoSitio.Current.Password);

                sqlconn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        autenticacion = new Autenticacion
                        {
                            ErrorAutenticacion = reader["UIN_IdUsuarioIntegracion"] != DBNull.Value ? Convert.ToString(reader["UIN_IdUsuarioIntegracion"]) : string.Empty,
                            EstaAutorizado = true
                        };
                    }
                }
                else
                {
                    autenticacion = new Autenticacion
                    {
                        ErrorAutenticacion = "El usuario o el password no son correctos",
                        EstaAutorizado = false
                    };
                }
                sqlconn.Close();
            }
            return autenticacion;
        }

        /// <summary>
        /// Servicio que obtiene el usuario y el token asociado a un cliente credito
        /// Hevelin Dayana Diaz Susa - 22/09/2022
        /// </summary>
        /// <param name="IdClienteCredito"></param>
        public UsuarioIntegracion ObtenerUsuarioIntegracionPorIdCliente(int IdClienteCredito)
        {
            UsuarioIntegracion usuario = new UsuarioIntegracion();
            using (SqlConnection conn = new SqlConnection(conexionStringSeguridad))
            {
                SqlCommand cmd = new SqlCommand("paObtenerUsuarioIntegracionPorCliente", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdCliente", IdClienteCredito);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    usuario =  new UsuarioIntegracion
                    {
                        UsuarioCliente = Convert.ToString(reader["UIN_IdUsuarioIntegracion"]),
                        Token = Convert.ToString(reader["UIN_Token"])
                    };
                }
                conn.Close();
               
            }
            return usuario;
        }

        /// <summary>
        /// Servicio que obtiene el token registrado en tabla usuarios integracion, filtrado por el id del usuario integracion
        /// Hevelin Dayana Diaz Susa - 01/11/2022
        /// </summary>
        /// <param name="UsuarioIntegracion"></param>
        public UsuarioIntegracion ObtenerTokenUsuarioIntegracionPorUsuario(string UsuarioIntegracion)
        {
            UsuarioIntegracion usuario = new UsuarioIntegracion();
            using (SqlConnection conn = new SqlConnection(conexionStringSeguridad))
            {
                SqlCommand cmd = new SqlCommand("paObtenerTokenUsuarioIntegracionPorUsuario", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UsuarioIntegracion", UsuarioIntegracion);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    usuario = new UsuarioIntegracion
                    {
                        IdClienteCredito = Convert.ToInt32(reader["UIN_IdClienteCredito"]),
                        UsuarioCliente = Convert.ToString(reader["UIN_IdUsuarioIntegracion"]),
                        Token = Convert.ToString(reader["UIN_Token"])
                    };
                }
                conn.Close();

            }
            return usuario;
        }
        
        /// método que consulta columna verificación de contenido de la tabla clientescredito_seg
        /// Hevelin Dayana Diaz Susa - 12/10/2022
        /// </summary>
        /// <param name="IdCliente"></param>
        public bool ConsultarVeriContenidoClienteCredito(int IdCliente)
        {
            bool existeVerificacion;
            using (SqlConnection con = new SqlConnection(conexionStringSeguridad))
            {
                SqlCommand cmd = new SqlCommand("paConsultarVeriContenidoClienteCre_CLI", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdClienteCredito", IdCliente);
                con.Open();
                var existe = cmd.ExecuteScalar();
                con.Close();
                existeVerificacion = Convert.ToBoolean(existe);
            }
            return existeVerificacion;
        }

    }
}