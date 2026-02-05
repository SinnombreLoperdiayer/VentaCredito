using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VentaCredito.Clientes.Datos.Repositorio
{
    public class CLContratoRepositorio
    {
        private static string CadCnxController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;

        #region Singleton

        private static CLContratoRepositorio instancia = new CLContratoRepositorio();

        public static CLContratoRepositorio Instancia
        {
            get { return instancia; }
        }

        #endregion

        #region Metodos

        /// <summary>
        /// Retorna el porcentaje de aviso para un contrato dado.
        /// </summary>
        /// <param name="idContrato"></param>
        /// <returns></returns>
        public decimal ObtenerPorcentajeAvisoContrato(int idContrato)
        {
            decimal porcentaje = 0;
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerPorcentajeAvisoContrato_CLI", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdContrato", idContrato);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        porcentaje = Convert.ToDecimal(reader["CON_PorcentajeAviso"]);
                    }
                }
            }
            return porcentaje;
        }

        /// <summary>
        /// Metodo que obtiene el acumulado del consumo del contrato
        /// </summary>
        /// <param name="idContrato"></param>
        /// <returns>decimal con el valor</returns>
        public decimal ObtenerConsumoContrato(int idContrato)
        {
            decimal acumulado = 0;
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerConsumoContrato_CLI", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdContrato", idContrato);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        acumulado = Convert.ToDecimal(reader["CON_AcumuladoVentas"]);
                    }
                }
            }
            return acumulado;
        }

        /// <summary>
        /// Obtiene el valor de las adiciones en dinero de un otro si
        /// </summary>
        /// <param name="idContrato"></param>
        /// <returns></returns>
        public decimal ObtenerValorPresupuestoContrato(int idContrato)
        {
            decimal presupuesto = 0;
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerValorPresupuestoContrato_CLI", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdContrato", idContrato);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        presupuesto = Convert.ToDecimal(reader["CON_ValorContrato"]);
                    }
                }
            }
            return presupuesto;
        }

        /// <summary>
        /// Obtiene el valor de las adiciones en dinero de un otro si
        /// </summary>
        /// <param name="idContrato"></param>
        /// <returns></returns>
        public decimal ObtenerValorPresupuestoOtrosi(int idContrato)
        {
            decimal presupuestoOtroSi = 0;
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerValorPresupuestoOtrosi_CLI", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdContrato", idContrato);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        presupuestoOtroSi = Convert.ToDecimal(reader["OSC_ValorotroSi"]);
                    }
                }
            }
            return presupuestoOtroSi;
        }

        #endregion

    }
}
