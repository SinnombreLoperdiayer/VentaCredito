using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using System.Configuration;


namespace VentaCredito.Tarifas.Datos.Repositorio
{
    public class Parametros
    {

        private string conexionStringController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;



        private static Parametros instancia = new Parametros();

        public static Parametros Instancia
        {
            get
            {
                return instancia;
            }
        }

        public List<TATipoEnvio> ObtenerServicioTipoEnvio(int idTipoEnvio, int idServicio)
        {
            List<TATipoEnvio> servicios = null;

            using (SqlConnection cnx = new SqlConnection(conexionStringController))
            {

                cnx.Open();
                SqlCommand cmd = new SqlCommand("paObtenerServiciosXTipoEnvio", cnx);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TipoEnvio", idTipoEnvio);
                SqlDataReader lector = cmd.ExecuteReader();
                if (lector.HasRows)
                {
                    servicios = new List<TATipoEnvio>();

                    while (lector.Read())
                    {
                        int servicio = Convert.ToInt32(lector["SER_IdServicio"]);
                        if (idServicio == servicio)
                        {
                            servicios.Add(new TATipoEnvio
                            {
                                IdTipoEnvio = Convert.ToInt16(lector["TEN_IdTipoEnvio"]),
                                Nombre = Convert.ToString(lector["TEN_Nombre"]),
                                Descripcion = Convert.ToString(lector["TEN_Descripcion"]),
                                PesoMinimo = Convert.ToDecimal(lector["TEN_PesoMinimo"]),
                                PesoMaximo = Convert.ToDecimal(lector["TEN_PesoMaximo"]),
                                CodigoMinisterio = Convert.ToDecimal(lector["TEN_CodigoMinisterio"]),
                            });
                        }

                    }
                }

            }

            return servicios;
        }

        /// <summary>
        /// Obtiene los Servicios de la DB
        /// </summary>
        /// <returns>Lista con los servicios de la DB</returns>
        public IEnumerable<TAServicioDC> ObtenerServicios()
        {
            using (SqlConnection cnx = new SqlConnection(conexionStringController))
            {
                List<TAServicioDC> servicios = new List<TAServicioDC>();
                cnx.Open();
                SqlCommand cmd = new SqlCommand("paObtenerServicios_TAR", cnx);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                SqlDataReader lector = cmd.ExecuteReader();
                while (lector.Read())
                {
                    servicios.Add(new TAServicioDC
                    {
                        IdServicio = int.Parse(lector["SER_IdServicio"].ToString()),
                        Nombre = lector["SER_Nombre"].ToString(),
                        Descripcion = lector["SER_Descripcion"].ToString(),
                        UnidadNegocio = lector["UNE_IdUnidad"].ToString(),
                        IdConceptoCaja = int.Parse(lector["SER_IdConceptoCaja"].ToString()),
                        TiempoEntrega = 0
                    });
                }
                return servicios;
            }
        }
    }
}
