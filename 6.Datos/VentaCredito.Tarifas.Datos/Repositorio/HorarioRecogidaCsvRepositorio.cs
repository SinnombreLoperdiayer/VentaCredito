using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Servicio.Entidades.Tarifas;

namespace VentaCredito.Tarifas.Datos.Repositorio
{
    public class HorarioRecogidaCsv
    {

        private string cadenaTransaccional = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;

        private static HorarioRecogidaCsv instancia = new HorarioRecogidaCsv();

        public static HorarioRecogidaCsv Instancia
        {
            get
            {
                return instancia;
            }
        }

        /// <summary>
        /// Retorna la lista del horario de determinado centro de servicio
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public List<TAHorarioRecogidaCsvDC> ObtenerHorarioRecogidaDeCsv(long idCentroServicio)
        {
            using (SqlConnection cnx = new SqlConnection(cadenaTransaccional))
            {
                List<TAHorarioRecogidaCsvDC> horario = new List<TAHorarioRecogidaCsvDC>();
                cnx.Open();
                SqlCommand cmd = new SqlCommand("paObtenerHorarioRecogidasCSV_TAR", cnx);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@idcentroServicios", idCentroServicio));
                SqlDataReader lector = cmd.ExecuteReader();
                while (lector.Read())
                {
                    horario.Add(new TAHorarioRecogidaCsvDC
                    {
                        DiaDeLaSemana = int.Parse(lector["HRC_Dia"].ToString()),
                        HoraRecogida = Convert.ToDateTime(lector["HRC_Hora"].ToString())
                    });
                }
                return horario;
            }

        }
    }
}
