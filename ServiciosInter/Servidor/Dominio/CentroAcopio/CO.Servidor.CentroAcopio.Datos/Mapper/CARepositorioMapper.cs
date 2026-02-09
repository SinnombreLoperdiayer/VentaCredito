using CO.Servidor.Servicios.ContratoDatos.Admisiones.MensajeriaNN;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CO.Servidor.CentroAcopio.Datos.Mapper
{
    public class CARepositorioMapper
    {
        internal static List<ClasificacionEnvioNN> ToListClasificacionEnvioNN(SqlDataReader reader)
        {
            List<ClasificacionEnvioNN> clasificaciones = new List<ClasificacionEnvioNN>();

            while (reader.Read()) { 
                clasificaciones.Add(new ClasificacionEnvioNN
                {
                    IdClasificacion = reader["CAM_IdClasificacion"] == DBNull.Value ? 0 : Convert.ToInt16(reader["CAM_IdClasificacion"]),
                    Clasificacion = reader["CAM_NombreClasificacion"] == DBNull.Value ? null : reader["CAM_NombreClasificacion"].ToString()
                });
            }
            return clasificaciones;
        }
    }
}
