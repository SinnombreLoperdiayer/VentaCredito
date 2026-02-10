using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Servicio.Entidades.Admisiones.Mensajeria;
using System.Data;
using System.Configuration;

namespace VentaCredito.Datos.Repositorio
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

        /// <summary>
        /// Retorna la lista de tipos de entrega que pueden aplicar sobre el envío
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ADTipoEntrega> ObtenerTiposEntrega()
        {
            List<ADTipoEntrega> listaTiposEntrega = new List<ADTipoEntrega>();
            ADTipoEntrega tiposEntrega = new ADTipoEntrega();

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerTipoEntrega_MEN", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        tiposEntrega = new ADTipoEntrega()
                        {
                            Id = reader["TIE_IdTipoEntrega"].ToString(),
                            Descripcion = reader["TIE_Descripcion"].ToString()
                        };
                        listaTiposEntrega.Add(tiposEntrega);
                    }
                }
            }
            return listaTiposEntrega;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="IdServicio"></param>
        /// <returns></returns>
        public IEnumerable<HorarioServicioAgil> ObtenerHorariosServiciosAgiles(int? idServicio = null)
        {
            var listaServiciosAgiles = new List<HorarioServicioAgil>();
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerDatosServicioPorIdServicio_MEN", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdServicio", (object)idServicio ?? DBNull.Value);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        int idServicioActual = !EsNulo(reader, "HSM_IdServicio")
                            ? Convert.ToInt32(reader["HSM_IdServicio"])
                            : 0;

                        listaServiciosAgiles.Add(new HorarioServicioAgil()
                        {
                            IdHorario = !EsNulo(reader, "HSM_IdHorario") ? Convert.ToInt32(reader["HSM_IdHorario"]) : idServicioActual,
                            IdServicio = idServicioActual,
                            NombreServicio = !EsNulo(reader, "HSM_NombreFranja") ? reader["HSM_NombreFranja"].ToString() : string.Empty,
                            Alias = !EsNulo(reader, "HSM_Alias") ? reader["HSM_Alias"].ToString() : string.Empty,
                            HoraInicio = !EsNulo(reader, "HSM_HoraInicial") ? (DateTime?)Convert.ToDateTime(reader["HSM_HoraInicial"]) : null,
                            HoraFin = !EsNulo(reader, "HSM_HoraFinal") ? (DateTime?)Convert.ToDateTime(reader["HSM_HoraFinal"]) : null,
                            AplicaTodoDia = !EsNulo(reader, "HSM_AplicaTodoDia") ? (bool?)Convert.ToBoolean(reader["HSM_AplicaTodoDia"]) : null
                        });
                    }
                }
            }
            return listaServiciosAgiles;
        }

        private bool EsNulo(SqlDataReader reader, string columna)
        {
            try
            {
                int ordinal = reader.GetOrdinal(columna);
                return reader.IsDBNull(ordinal);
            }
            catch (IndexOutOfRangeException)
            {
                return true;
            }
        }
    }
}
