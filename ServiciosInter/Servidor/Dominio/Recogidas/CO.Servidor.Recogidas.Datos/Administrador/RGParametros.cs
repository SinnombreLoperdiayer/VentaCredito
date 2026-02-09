using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.Recogidas;

namespace CO.Servidor.Recogidas.Datos.Administrador
{
    public class RGParametros
    {
        private static readonly RGParametros instancia = new RGParametros();
        private string CadCnxController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;

        /// <summary>
        /// Retorna la instancia de la clase TARepositorio
        /// </summary>
        public static RGParametros Instancia
        {
            get { return RGParametros.instancia; }
        }

        /// <summary>
        /// constructor
        /// </summary>
        public RGParametros() { }


        /// <summary>
        /// Consulta las territoriales de los centros de servicios
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public List<RGTerritorialDC> ObtenerTerritoriales(string idCentroServicio)
        {
            List<RGTerritorialDC> resultado = null;

            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerCentroLogisticoRacol", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                if (idCentroServicio.ToLower() != "null")
                {
                    cmd.Parameters.AddWithValue("@IdCentroServicios", (string)idCentroServicio);
                }
                conn.Open();
                var reader = cmd.ExecuteReader();

                resultado = MapperParametros.MapperTerritoriales(reader);
            }

            return resultado;
        }

        /// <summary>
        /// Consulta agencias de las territoriales de los centros de servicios
        /// </summary>
        /// <returns></returns>
        public List<RGAgenciaDC> ObtenerAgencias()
        {
            List<RGAgenciaDC> resultado = null;

            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerCentroLogisticoAgencias", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                var reader = cmd.ExecuteReader();

                resultado = MapperParametros.MapperAgencias(reader);
            }

            return resultado;
        }
    }
}
