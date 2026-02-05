using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Servicio.Entidades.Admisiones.Mensajeria;
using System.Data.SqlClient;
using VentaCredito.Transversal.Entidades.Clientes;

namespace VentaCredito.Datos.Repositorio
{
    public class ADCentroAcopioRepositorio
    {

        private string CadCnxController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;

        private static ADCentroAcopioRepositorio instancia = new ADCentroAcopioRepositorio();
        private string conexionStringController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;

        public static ADCentroAcopioRepositorio Instancia { get { return instancia; } }

        /// <summary>
        /// Método para ingresar una guía a centro de acopio
        /// </summary>
        /// <param name="guia"></param>
        public void IngresarGuiaManualCentroAcopio(ADGuia guia)
        {

            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("paIngresarGuiaNoRegCentroAcopio_MEN", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroGuia", guia.NumeroGuia);
                cmd.Parameters.AddWithValue("@IdAdmision", guia.IdAdmision);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Obtiene los mensajes homologados para el estado 2
        /// </summary>
        /// <returns></returns>
        public List<MensajeEstadoDos> ObtenerMensajesEstadoDos()
        {
            List<MensajeEstadoDos> mensajes = new List<MensajeEstadoDos>();

            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerMensajesEstadoDos", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader read = cmd.ExecuteReader();

                while (read.Read())
                {
                    MensajeEstadoDos mensaje = new MensajeEstadoDos
                    {
                        IdMensaje = Convert.ToInt32(read["MCA_Id"]),
                        Mensaje = read["MCA_Mensaje"].ToString()
                    };
                    mensajes.Add(mensaje);
                }
            }

            return mensajes;
        }
    }
}
