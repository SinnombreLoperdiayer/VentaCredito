using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Framework.Servidor.Excepciones.Session
{
    public class SERepositorioSesion
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
        /// Valida la sesion sel usuario
        /// </summary>
        /// <param name="credencial"></param>
        /// <returns></returns>
        public string ValidaSesionUsuario(string token)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringAuditoria))
            {
                SqlCommand cmd = new SqlCommand("paValidarSesionUsuario_SEG", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@USE_NomUsuario", ControllerContext.Current.Usuario);
                cmd.Parameters.AddWithValue("@USE_Token", token);
                cmd.Parameters.AddWithValue("@USE_IdentificadorMaquina", ControllerContext.Current.IdentificadorMaquina);
                cmd.Parameters.AddWithValue("@USE_IdentificadorAplicacion", ControllerContext.Current.IdAplicativoOrigen);
                cmd.Parameters.AddWithValue("@USE_IdCentroServicios", ControllerContext.Current.IdCentroServicio);
                cmd.Parameters.AddWithValue("@USE_IdCaja", ControllerContext.Current.IdCaja);
                cmd.Parameters.AddWithValue("@USE_CreadoPor", ControllerContext.Current.Usuario);
                conn.Open();
                string rst = cmd.ExecuteScalar().ToString();
                conn.Close();
                return rst;
            }
        }

        /// <summary>
        /// Acualiza los datos de centro servicio y caja para el usuario
        /// </summary>
        /// <param name="credencia"></param>
        public void ActualizaSesion()
        {
            using (SqlConnection conn = new SqlConnection(conexionStringAuditoria))
            {
                SqlCommand cmd = new SqlCommand("paActualizarSesionUsuario_SEG", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@USE_NomUsuario", ControllerContext.Current.Usuario);
                cmd.Parameters.AddWithValue("@USE_Token", ControllerContext.Current.TokenSesion);
                cmd.Parameters.AddWithValue("@USE_IdentificadorMaquina", ControllerContext.Current.IdentificadorMaquina);
                cmd.Parameters.AddWithValue("@USE_IdentificadorAplicacion", ControllerContext.Current.IdAplicativoOrigen);
                cmd.Parameters.AddWithValue("@USE_IdCaja", ControllerContext.Current.IdCaja);
                cmd.Parameters.AddWithValue("@USE_IdCentroServicios", ControllerContext.Current.IdCentroServicio);
                

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }


    }

    public class ConstantesRespuestaValidacionSesion
    {

        public const string FALLO_VALIDAR = "FAL";
        public const string PERMITIDO = "PER";
        //public const string SESION_CREADA = "CRE";
        public const string MULTIPLES_SESIONES = "MUL";
        //public const string SESION_CERRADA = "CER";
        //public const string SESION_ACTUALIZADA = "UPD";
    }
}
