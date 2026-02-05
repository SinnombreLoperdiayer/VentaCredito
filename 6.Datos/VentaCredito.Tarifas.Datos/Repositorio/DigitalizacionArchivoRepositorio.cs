using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CO.Servidor.Dominio.Comun.Tarifas;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using Servicio.Entidades.Tarifas;

namespace VentaCredito.Tarifas.Datos.Repositorio
{
    public class DigitalizacionArchivoRepositorio
    {
        private static DigitalizacionArchivoRepositorio instancia = new DigitalizacionArchivoRepositorio();
        private string cadenaTransaccional = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;
        public static DigitalizacionArchivoRepositorio Instancia { get { return instancia; } }

        /// <summary>
        /// Retorna el número de dias del trayecto si existe
        /// </summary>
        /// <param name="municipioOrigen">Municipio de Origen</param>
        /// <param name="municipioDestino">Municipio de Destino</param>
        /// <param name="servicio">Servicio</param>
        /// <returns>Duración en días</returns>
        public TATiempoDigitalizacionArchivo ValidarServicioTrayectoDestino(PALocalidadDC municipioOrigen, PALocalidadDC municipioDestino, TAServicioDC servicio, decimal peso = 0)
        {
            TATiempoDigitalizacionArchivo tiempos = new TATiempoDigitalizacionArchivo();
            using (SqlConnection conn = new SqlConnection(cadenaTransaccional))
            {
                SqlCommand cmd = null;
                if (servicio.IdServicio == TAConstantesServicios.SERVICIO_MENSAJERIA && peso >= 3)
                {
                    cmd = new SqlCommand("paValidarDiasAdicionalesTrayectoDestino_TAR", conn);
                }
                else
                {
                    cmd = new SqlCommand("paValidarServicioTrayectoDestino_TAR", conn);
                    cmd.Parameters.AddWithValue("@IdServicio", servicio.IdServicio);
                }
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdLocalidadOrigen", municipioOrigen.IdLocalidad);
                cmd.Parameters.AddWithValue("@IdLocalidadDestino", municipioDestino.IdLocalidad);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {                   

                    tiempos.numeroDiasArchivo = Convert.ToDouble(reader["STR_TiempoArchivo"]);
                    tiempos.numeroDiasDigitalizacion = Convert.ToDouble(reader["STR_TiempoDigitalizacion"]);
                    tiempos.numeroDiasEntrega = Convert.ToInt32(reader["STR_TiempoEntrega"]);
                }
                conn.Close();
            }
            return tiempos;
        }
    }
}
