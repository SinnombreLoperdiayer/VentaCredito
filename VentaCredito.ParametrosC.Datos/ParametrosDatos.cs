using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using VentaCredito.Transversal.Entidades.Parametros;
using System.Data.SqlClient;
using System.Data;
using VentaCredito.Transversal.Enumerables;

namespace VentaCredito.ParametrosC.Datos
{
    public class ParametrosDatos
    {
        #region Singleton

        private static ParametrosDatos instancia = new ParametrosDatos();
        private string CnxController;
        public static ParametrosDatos Instancia
        {
            get
            {
                return instancia;
            }
        }

        #endregion  

        public ParametrosDatos()
        {
            CnxController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;
        }

        /// <summary>
        /// Obtiene el valor minimo declarado por id lista de precios del cliente credito filtrado por peso.
        /// Hevelin Dayana Diaz - 14/02/2022
        /// </summary>
        /// <param name="idListaPrecio"></param>
        /// <returns>Valor minimo declarado segun el peso</returns>
        public TAValorPesoDeclaradoDC ObtenerValorPesoDeclarado(int idListaPrecio, decimal peso)
        {
            using (SqlConnection sqlConn = new SqlConnection(CnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerValorPesoDeclarado_Cli", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdListaPrecios", idListaPrecio);
                cmd.Parameters.AddWithValue("@Peso", peso);
                sqlConn.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    return new TAValorPesoDeclaradoDC()
                    {
                        IdValorPesoDeclarado = Convert.ToInt16(reader["IdValorPesoDeclarado"]),
                        IdListaPrecio = Convert.ToInt16(reader["IdListaPrecio"]),
                        PesoInicial = Convert.ToInt64(reader["PesoInicial"]),
                        PesoFinal = Convert.ToInt64(reader["PesoFinal"]),
                        ValorMinimoDeclarado = Convert.ToInt64(reader["ValorMinimoDeclarado"]),
                        ValorMaximoDeclarado = Convert.ToInt64(reader["ValorMaximoDeclarado"]),
                        EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS
                    };
                }
                else
                {
                    return new TAValorPesoDeclaradoDC();
                }
            }
        }
    }
}
