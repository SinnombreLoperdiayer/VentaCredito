using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using CO.Servidor.Servicios.ContratoDatos.Seguridad;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;

namespace CO.Servidor.Dominio.Seguridad
{
    public class SEGSeguridad
    {
        #region Instancia
        private static readonly SEGSeguridad instancia = new SEGSeguridad();
        private string ConnectionSeguridad = ConfigurationManager.ConnectionStrings["ConnectionSeguridad"].ConnectionString + " ";

        /// <summary>
        /// Retorna una instancia de centro Servicios
        /// /// </summary>
        public static SEGSeguridad Instancia
        {
            get { return SEGSeguridad.instancia; }
        }

        public List<SEGUsuarioDC> ObtenerInfoUsuarioPorDocumento(string creadoPor)
        {
            List<SEGUsuarioDC> resultado = null;
            using(SqlConnection conn = new SqlConnection(ConnectionSeguridad))
            {
                conn.Open();
                var cmd = new SqlCommand("paConsultaUsuarios_SEG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@tipo", 2);
                cmd.Parameters.AddWithValue("@idApp", 0);
                cmd.Parameters.AddWithValue("@TextoUsuario", creadoPor);
                cmd.Parameters.AddWithValue("@NRegistros", 1);
                cmd.Parameters.AddWithValue("@Pagina", 1);

                var reader = cmd.ExecuteReader();
                resultado = MapperSEGSeguridad.ToUsuarioDC(reader);
            }

            return resultado;
        }
        #endregion

        #region Metodos

        #endregion
    }
}
