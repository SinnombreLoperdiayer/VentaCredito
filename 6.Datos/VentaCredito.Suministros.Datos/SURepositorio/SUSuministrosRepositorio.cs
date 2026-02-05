using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using Servicio.Entidades.Suministros;
using Framework.Servidor.Excepciones;
using VentaCredito.Transversal;

namespace VentaCredito.Suministros.Datos.Repositorio
{
    public class SUSuministros
    {
        private string cadenaTransaccional = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;

        #region CrearInstancia

        private static readonly SUSuministros instancia = new SUSuministros();

        /// <summary>
        /// Retorna una instancia de centro Servicios
        /// /// </summary>
        public static SUSuministros Instancia
        {
            get { return SUSuministros.instancia; }
        }

        #endregion CrearInstancia

        /// <summary>
        /// Almacena el consumo de un suministro en la base de datos
        /// </summary>
        /// <param name="consumoSuministro"></param>
        /// <param name="datosCentroServicio"></param>
        /// <param name="datosServicio"></param>
        public void GuardarConsumoSuministro(SUConsumoSuministroDC consumoSuministro)
        {
            using (SqlConnection conn = new SqlConnection(cadenaTransaccional))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("paCrearConsumoSuministro_SUM", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdSuministro", (int)consumoSuministro.Suministro);
                cmd.Parameters.AddWithValue("@IdGrupoSuministro", consumoSuministro.GrupoSuministro.ToString());
                cmd.Parameters.AddWithValue("@IdPropietarioSuministro", consumoSuministro.IdDuenoSuministro);
                cmd.Parameters.AddWithValue("@NumeroSuministro", consumoSuministro.NumeroSuministro);
                cmd.Parameters.AddWithValue("@CantidadConsumida", consumoSuministro.Cantidad);
                cmd.Parameters.AddWithValue("@EstadoConsumo", consumoSuministro.EstadoConsumo.ToString());
                cmd.Parameters.AddWithValue("@IdServicio", consumoSuministro.IdServicioAsociado);
                cmd.Parameters.AddWithValue("@CreadoPor", ContextoSitio.Current.Usuario);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Obtiene el numero de suministro prefijo + valorActual
        /// </summary>
        /// <param name="idSuministro">id del suministro</param>
        /// <returns>numero del giro</returns>

        public SUNumeradorPrefijo ObtenerNumeroPrefijoValor(SUEnumSuministro Suministro)
        {
            SUNumeradorPrefijo numPre = new SUNumeradorPrefijo();
            int idSuministro = (int)Suministro;

            using (SqlConnection conn = new SqlConnection(cadenaTransaccional))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerSuministrosValorActualNumerador_SUM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter paramOut = new SqlParameter("@ACTUAL", SqlDbType.BigInt);
                paramOut.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(paramOut);
                cmd.Parameters.AddWithValue("@IDSUMINISTROS", idSuministro);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);

                if (ds.Tables.Count > 0)
                {
                    numPre = new SUNumeradorPrefijo()
                    {
                        Prefijo =  ds.Tables[0].Rows[0].Field<string>("Prefijo"),
                        ValorActual = ds.Tables[0].Rows[0].Field<long>("ValorActual")
                    };
                }
                return numPre;
            }
        }
    }
}
