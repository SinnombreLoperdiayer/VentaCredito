using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using VentaCredito.Cajas.Datos.RepositorioCasaMatriz;
using Servicio.Entidades.Cajas;
using Servicios.Entidades.Cajas;

namespace VentaCredito.Cajas.Datos.RepositorioCasaMatriz
{
    public class OperacionCajaCasaMatrizRepositorio
    {

        private string conexionStringController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;

        private static OperacionCajaCasaMatrizRepositorio instancia = new OperacionCajaCasaMatrizRepositorio();
        public static OperacionCajaCasaMatrizRepositorio Instancia
        {
            get
            {
                return instancia;
            }
        }

        
        public long AdicionarOperacionCajaCasaMatriz(OperacionCajaCasaMatrizDC operacion)
        {
            long idOperacion = 0;
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paInsertarOperacionCajaCasaMatriz_CAJ", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CAE_IdAperturaCaja", operacion.IdAperturaCaja);
                cmd.ExecuteNonQuery();
            }

            return idOperacion;
        }
    }
}
