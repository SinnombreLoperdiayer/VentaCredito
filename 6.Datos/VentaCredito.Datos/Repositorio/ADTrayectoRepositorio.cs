using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Servicio.Entidades.Admisiones.Mensajeria;
using System.Data.SqlClient;
using System.Data;


namespace VentaCredito.Datos.Repositorio
{
    public class ADTrayectoRepositorio
    {

        private string CadCnxController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;


        private static ADTrayectoRepositorio instancia = new ADTrayectoRepositorio();
        private string conexionStringController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;

        public static ADTrayectoRepositorio Instancia { get { return instancia; } }

        /// <summary>
        /// Retorna todo el listado de casilleros establecidos por trayecto
        /// </summary>
        /// <returns></returns>
        public HashSet<ADRangoTrayecto> ObtenerCasillerosTrayectos()
        {
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                List<ADRangoTrayecto> casilleros = new List<ADRangoTrayecto>();

                conn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerCasillerosTrayectos_OPN", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    var listaLocalidades = new List<ADRangoTrayecto>();
                    var listaCasilleros = new List<ADRangoCasillero>();
                    while (reader.Read())
                    {
                        listaLocalidades.Add(new ADRangoTrayecto
                        {
                            IdLocalidadDestino = reader["TRC_IdLocalidadDestino"].ToString(),
                            IdLocalidadOrigen = reader["TRC_IdLocalidadOrigen"].ToString()
                        });

                        listaCasilleros.Add(new ADRangoCasillero
                        {
                            Casillero = reader["TCP_IdCasillero"].ToString(),
                            RangoInicial = Convert.ToDecimal(reader["RPC_RangoInicial"]),
                            RangoFinal = Convert.ToDecimal(reader["RPC_RangoFinal"])
                        });
                    }
                    var listaAgrupada = listaLocalidades.GroupBy(cas => new { cas.IdLocalidadDestino, cas.IdLocalidadOrigen }).ToList();

                    listaAgrupada.ConvertAll<ADRangoTrayecto>(r =>
                   new ADRangoTrayecto
                   {
                       IdLocalidadDestino = r.First().IdLocalidadDestino,
                       IdLocalidadOrigen = r.First().IdLocalidadOrigen,
                       Rangos = listaCasilleros
                   });
                    var lista = listaAgrupada.SelectMany(g => g);
                    casilleros = lista.ToList();

                }
                return new HashSet<ADRangoTrayecto>(casilleros);
            }
        }
    }
}

