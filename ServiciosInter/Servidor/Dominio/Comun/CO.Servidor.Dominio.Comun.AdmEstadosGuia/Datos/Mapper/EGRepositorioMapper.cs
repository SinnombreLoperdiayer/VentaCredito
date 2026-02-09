using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CO.Servidor.Dominio.Comun.AdmEstadosGuia.Datos.Mapper
{
    public class EGRepositorioMapper
    {

        internal static ADTrazaGuia ToListEstadoGuia(SqlDataReader reader)
        {
            ADTrazaGuia traza = new ADTrazaGuia();
            if (reader.Read())
            {
                traza.NumeroGuia = reader["Guia"] == DBNull.Value ? 0 : Convert.ToInt64(reader["Guia"]);
                traza.IdEstadoGuia = reader["IdEstado"] == DBNull.Value ? Convert.ToInt16(0) : Convert.ToInt16(reader["IdEstado"]);
                traza.DescripcionEstadoGuia = reader["Estado"] == DBNull.Value ? string.Empty : reader["Estado"].ToString();
                traza.IdCiudad = reader["IdLocalidad"] == DBNull.Value ? string.Empty : reader["IdLocalidad"].ToString();
                traza.Ciudad = reader["NombreLocalidad"] == DBNull.Value ? string.Empty : reader["NombreLocalidad"].ToString();
                traza.FechaAdmisionGuia = Convert.ToDateTime(reader["Fecha"]);
                traza.FechaGrabacion = Convert.ToDateTime(reader["Fecha"]);
                traza.FechaEntrega = reader["FechEntre"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["FechEntre"]);
                traza.IdOrigenApliacion = 0;
                traza.ClaseEstado = reader["ClaseEstado"] == DBNull.Value ? string.Empty : reader["ClaseEstado"].ToString();
            }
            return traza;
        }


    }
}
