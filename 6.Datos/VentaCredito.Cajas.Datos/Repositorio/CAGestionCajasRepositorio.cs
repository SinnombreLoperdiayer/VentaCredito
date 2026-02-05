using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Servicio.Entidades.Cajas;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using Servicios.Entidades.Cajas;

namespace VentaCredito.Cajas.Datos.Repositorio
{
    public class CAGestionCajasRepositorio
    {

        private string conexionStringController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;

        #region Singleton

        private static CAGestionCajasRepositorio instancia = new CAGestionCajasRepositorio();

        public static CAGestionCajasRepositorio Instancia
        {
            get
            {
                return CAGestionCajasRepositorio.instancia;
            }
        }

        #endregion

        #region Metodos

        public void AdicionarMovCentroSrvCajaCasaMatriz(CajaCasaMatrizCentroSvcMovDC transCSCasaMatriz)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paCajaCasaMatrizCentroSvcMov_CAJ", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CEM_IdOperacionCajaCasaMatriz", transCSCasaMatriz.CEM_IdOperacionCajaCasaMatriz);
                cmd.Parameters.AddWithValue("@CEM_IdCentroServiciosMov", transCSCasaMatriz.CEM_IdCentroServiciosMov);
                cmd.Parameters.AddWithValue("@CEM_NombreCentroServiciosMov", transCSCasaMatriz.CEM_NombreCentroServiciosMov);
                cmd.Parameters.AddWithValue("@CEM_IdRegistroTranscaccion", transCSCasaMatriz.CEM_IdRegistroTranscaccion);
                cmd.Parameters.AddWithValue("@CEM_CreadoPor", transCSCasaMatriz.CEM_CreadoPor);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }




        #endregion


    }

}
