using CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CO.Servidor.ParametrosOperacion.Datos.Mapper
{
    public class PORepositorioMapper
    {
        internal static List<POVehiculo> ToListVehiculos(SqlDataReader reader)
        {
            List<POVehiculo> vehiculos = null;
            while (reader.Read())
            {
                if (vehiculos == null)
                {
                    vehiculos = new List<POVehiculo>();
                }
                vehiculos.Add(new POVehiculo
                {
                    IdVehiculo = Convert.ToInt32(reader["VEH_IdVehiculo"]),
                    Placa = reader["VEH_Placa"].ToString(),
                    NombrePropietario=reader["CON_RazonSocial"].ToString(),
                });
            }
            return vehiculos;

        }
    }
}
