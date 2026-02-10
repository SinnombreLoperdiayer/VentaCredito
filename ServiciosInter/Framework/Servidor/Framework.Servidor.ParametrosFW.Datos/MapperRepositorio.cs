using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace Framework.Servidor.ParametrosFW.Datos
{
    internal class MapperRepositorio
    {
        internal static List<PADispositivoMovil> ToDispositivoMovil(SqlDataReader reader)
        {
            List<PADispositivoMovil> resultado = null;
            if(reader.HasRows)
            {
                while(reader.Read())
                {
                    PADispositivoMovil dispositivo = new PADispositivoMovil()
                    {
                        IdDispositivo = Convert.ToInt64(reader["DIM_IdDispositivo"]),
                        SistemaOperativo = (PAEnumOsDispositivo)Enum.Parse(typeof(PAEnumOsDispositivo), Convert.ToString(reader["DIM_SistemaOperativo"])),
                        TipoDispositivo = (PAEnumTiposDispositivos)Enum.Parse(typeof(PAEnumTiposDispositivos), Convert.ToString(reader["DIM_TipoDispositivo"])),
                        TokenDispositivo = Convert.ToString(reader["DIM_TokenDispositivo"]),
                        IdCiudad = Convert.ToString(reader["DIM_IdLocalidad"])

                    };
                    if(resultado == null)
                    {
                        resultado = new List<PADispositivoMovil>();
                    }
                    resultado.Add(dispositivo);
                }
            }
            return resultado;
        }
    }
}