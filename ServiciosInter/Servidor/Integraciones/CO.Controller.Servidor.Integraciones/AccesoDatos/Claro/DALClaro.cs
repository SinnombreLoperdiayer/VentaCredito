using CO.Controller.Servidor.Integraciones.AccesoDatos.Claro.Mapper;
using CO.Servidor.Servicios.ContratoDatos.Integraciones;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CO.Controller.Servidor.Integraciones.AccesoDatos.Claro
{
    public class DALClaro
    {
        public static readonly DALClaro Instancia = new DALClaro();

        public DALClaro()
        { }

        public List<GuiasClaro> ConsultarEntregasClaro()
        {
            List<GuiasClaro> GuiasClaro = new List<GuiasClaro>();

            using (SqlConnection cnx = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Sispostal"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("paConsultarGuiaTmpEntrega_TBE", cnx);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cnx.Open();

                SqlDataReader resultado=cmd.ExecuteReader();

                if (resultado.HasRows)
                {
                    GuiasClaro = MapperClaro.ToListGuiasClaro(resultado);
                }
             }

            
            return GuiasClaro;

        }

        public void ModificarGuiaEnvioMsj(decimal guia)
        {

            using (SqlConnection cnx = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Sispostal"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("paModificarGuiaEnvioMsj_TBE", cnx);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Guia",guia);
                cnx.Open();

                cmd.ExecuteNonQuery();
            }
        }
    }
}
