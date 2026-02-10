using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Seguridad;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Framework.Servidor.Seguridad.Datos
{
    public  class SERepositorioSesion
    {

        #region Campos

        private string conexionStringAuditoria = ConfigurationManager.ConnectionStrings["ControllerExcepciones"].ConnectionString;

        private static readonly SERepositorioSesion instancia = new SERepositorioSesion();

        public static SERepositorioSesion Instancia
        {
            get { return SERepositorioSesion.instancia; }
        }

        private SERepositorioSesion()
        { }


      




        #endregion
        /// <summary>
        /// Crea sesion de usuario 
        /// </summary>
        /// <param name="credencial"></param>
        public SECredencialUsuario CrearSesionUsuario(SECredencialUsuario credencial)
        {            

            using (SqlConnection conn = new SqlConnection (conexionStringAuditoria))
            {
                SqlCommand cmd = new SqlCommand("paCrearSesionUsuario_SEG", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@USE_NomUsuario",credencial.Usuario);                
                cmd.Parameters.AddWithValue("@USE_IdentificadorAplicacion", credencial.IdAplicativoOrigen);
                cmd.Parameters.AddWithValue("@USE_IdentificadorMaquina", credencial.IdentificadorMaquina);
                cmd.Parameters.AddWithValue("@USE_TiempoVida",12); //en horas
                cmd.Parameters.AddWithValue("@USE_CreadoPor",credencial.Usuario);
                conn.Open();
                credencial.TokenSesion = cmd.ExecuteScalar().ToString();
                conn.Close();
            }
            return credencial;

        }
      
        /// <summary>
        /// Acualiza los datos de centro servicio y caja para el usuario
        /// </summary>
        /// <param name="credencia"></param>
        public void ActualizaSesion(SECredencialUsuario credencial)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringAuditoria))
            {
                SqlCommand cmd = new SqlCommand("paActualizarSesionUsuario_SEG", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@USE_NomUsuario", ControllerContext.Current.Usuario);
                cmd.Parameters.AddWithValue("@USE_Token", ControllerContext.Current.TokenSesion);
                cmd.Parameters.AddWithValue("@USE_IdentificadorMaquina", ControllerContext.Current.IdentificadorMaquina);
                cmd.Parameters.AddWithValue("@USE_IdentificadorAplicacion", ControllerContext.Current.IdAplicativoOrigen);
                cmd.Parameters.AddWithValue("@USE_IdCentroServicios", ControllerContext.Current.IdCaja);
                
                conn.Open();
                cmd.ExecuteNonQuery();                
            }
        }


    }

  
}
