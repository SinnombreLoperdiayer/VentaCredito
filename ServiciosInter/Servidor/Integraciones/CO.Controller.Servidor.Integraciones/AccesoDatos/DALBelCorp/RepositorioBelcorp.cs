using Framework.Servidor.Excepciones;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CO.Controller.Servidor.Integraciones.AccesoDatos.DALBelCorp
{
    public class RepositorioBelcorp
    {

        private static readonly RepositorioBelcorp instancia = new RepositorioBelcorp();

        public static RepositorioBelcorp Instancia
        {
            get
            {
                return instancia;
            }
        }

        private RepositorioBelcorp()
        {
            
        }



        string CadConexionController =  System.Configuration.ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;

        /// <summary>
        /// Valida que la transaccion esté confirmada
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool VerificarTransaccionInventarioDevolucion(Guid token)
        {
            using (SqlConnection cnx = new SqlConnection(CadConexionController))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand("paConfirmaDevolBELCORP", cnx);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@NumGUID", token));
                SqlParameter rst = new SqlParameter("@resul", false);
                rst.Direction = System.Data.ParameterDirection.Output;
                cmd.Parameters.Add(rst);
                cmd.ExecuteNonQuery();
                return Convert.ToBoolean(rst.Value);                
            }
        }

        /// <summary>
        /// Asocia el token con el numero de guia generado para cerrar la transaccion
        /// </summary>
        /// <param name="token"></param>
        /// <param name="numeroGuia"></param>
        public void ActualizarTransaccionInventario(Guid token,long numeroGuia)
        {
            using (SqlConnection cnx = new SqlConnection(CadConexionController))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand("paCierraDevBELCORPController", cnx);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@numGUID", token));
                cmd.Parameters.Add(new SqlParameter("@numGuia", numeroGuia));
                cmd.Parameters.Add(new SqlParameter("@usuario",ControllerContext.Current.Usuario ));
                cmd.Parameters.Add(new SqlParameter("@idCentroServicio", ControllerContext.Current.IdCentroServicio));

                cmd.ExecuteNonQuery();               
            }
        }


    }
}
