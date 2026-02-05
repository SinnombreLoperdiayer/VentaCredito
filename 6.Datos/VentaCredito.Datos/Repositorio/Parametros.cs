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
        public IEnumerable<HorarioServicioAgil> ObtenerHorariosServiciosAgiles()
        {
            var listaServiciosAgiles = new List<HorarioServicioAgil>();
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerDatosServicioPorIdServicio_MEN", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        listaServiciosAgiles.Add(new HorarioServicioAgil()
                        {
                            IdHorario = Convert.ToInt32(reader["HSM_IdHorario"]),
                            IdServicio = Convert.ToInt32(reader["HSm_IdServicio"]),
                            Alias = reader["HSM_Alias"].ToString(),
                            HoraInicio = Convert.ToDateTime(reader["HSM_HoraInicial"]),
                            HoraFin = Convert.ToDateTime(reader["HSM_HoraFinal"]),
                            AplicaTodoDia = Convert.ToBoolean(reader["HSM_AplicaTodoDia"])
                        });
                    }
                }
            }
            return listaServiciosAgiles;
        }
    }
}
