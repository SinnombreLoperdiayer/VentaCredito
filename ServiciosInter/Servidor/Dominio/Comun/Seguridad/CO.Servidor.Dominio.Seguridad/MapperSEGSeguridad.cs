using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using CO.Servidor.Servicios.ContratoDatos.Seguridad;

namespace CO.Servidor.Dominio.Seguridad
{
    internal class MapperSEGSeguridad
    {
        internal static List<SEGUsuarioDC> ToUsuarioDC(SqlDataReader reader)
        {
            List<SEGUsuarioDC> resultado = null;

            if(reader.HasRows)
            {
                resultado = new List<SEGUsuarioDC>();
                while(reader.Read())
                {
                    var r = new SEGUsuarioDC
                    {
                        IdUsuario = Convert.ToString(reader["USUIdUsuario"]),
                        LoginUsuario = Convert.ToString(reader["USULoginUsuario"]),
                        Estado = Convert.ToBoolean(reader["USUEstado"]),
                        FechaCreacion = Convert.ToDateTime(reader["USUFechaCreacion"]),
                        IdDocumento = Convert.ToString(reader["USUIdDocumento"]),
                        NumeroIdentificacion = Convert.ToString(reader["num_ide"]),
                        Nombre = Convert.ToString(reader["Nombre"]),
                        Ingreso = Convert.ToDateTime(reader["Ingreso"]),
                        Egreso = Convert.ToDateTime(reader["Egreso"]),
                        Correo = Convert.ToString(reader["Correo"]),
                        Celular = Convert.ToString(reader["Celular"]),
                    };

                    resultado.Add(r);
                }
            }

            return resultado;
        }
    }
}