using CO.Servidor.Servicios.ContratoDatos.Integraciones;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CO.Controller.Servidor.Integraciones.AccesoDatos.Claro.Mapper
{
    public class MapperClaro
    {
        internal static List<GuiasClaro> ToListGuiasClaro(SqlDataReader reader)
        {
            List <GuiasClaro> GuiasClaro= null;

            while (reader.Read())
            {
                if (GuiasClaro == null)
                {
                    GuiasClaro = new List<GuiasClaro>();
                }

                GuiasClaro.Add(new GuiasClaro
                {
                    Guia=reader["Guia"] ==DBNull.Value ? 0 : Convert.ToDecimal(reader["Guia"]),
                    Orden=reader["orden"]==DBNull.Value ? 0 :Convert.ToDecimal(reader["orden"]),
                    Nombre=reader["nombre"]==DBNull.Value?string.Empty: reader["nombre"].ToString(),
                    Celular=reader["telefono"]==DBNull.Value? string.Empty : reader["telefono"].ToString()
                });
            }
            return GuiasClaro;
        } 
    }
}
