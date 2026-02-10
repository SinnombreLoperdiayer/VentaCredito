using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using System.Data.SqlClient;

namespace CO.Servidor.CentroServicios.Datos.Mapper
{
    public class MERepositorioMapper
    {
        public static List<PUCentroServiciosDC> ToListCentrosServicio(SqlDataReader reader)
        {
            List<PUCentroServiciosDC> CentrosServicio = null;
            while (reader.Read())
            {
                if (CentrosServicio == null)
                {
                    CentrosServicio = new List<PUCentroServiciosDC>();
                }
                CentrosServicio.Add(new PUCentroServiciosDC
                {
                    IdRegionalAdministrativa=Convert.ToInt32(reader["MRA_IdLocalidadRegionalAdm"]),
                    IdMunicipio = reader["LOC_IdLocalidad"].ToString(),
                    Nombre =reader["NombreCompleto"].ToString()
                });
            }
            return CentrosServicio;
        }
        public static List<PUCentroServiciosDC> ToListCentrosServicioTipo(SqlDataReader reader)
        {
            List<PUCentroServiciosDC> CentrosServicio = null;
            while (reader.Read())
            {
                if (CentrosServicio == null)
                {
                    CentrosServicio = new List<PUCentroServiciosDC>();
                }
                CentrosServicio.Add(new PUCentroServiciosDC
                {
                    IdRegionalAdministrativa = Convert.ToInt32(reader["MRA_IdLocalidadRegionalAdm"]),
                    IdMunicipio = reader["LOC_IdLocalidad"].ToString(),
                    Nombre = reader["NombreCompleto"].ToString(),
                    IdTipoCiudad = reader["LTC_IdTipoCiudad"]==DBNull.Value ? 0 : Convert.ToInt32(reader["LTC_IdTipoCiudad"]) ,

                });
            }
            return CentrosServicio;
        }

        internal static List<PUTipoCiudad> ToListTipoCiudad(SqlDataReader reader)
        {
            List<PUTipoCiudad> tiposCiudades = null;
            while (reader.Read())
            {
                if (tiposCiudades == null)
                {
                    tiposCiudades = new List<PUTipoCiudad>();
                }
                tiposCiudades.Add(new PUTipoCiudad
                {
                    IdTipoCiudad = Convert.ToInt32(reader["CGC_IdTipoCiudad"]),
                    TipoCiudad = reader["CGC_TipoCiudad"].ToString(),
                });
            }
            return tiposCiudades;
          }

        internal static List<PUTipoZona> ToListTipoZona(SqlDataReader reader) {
            List<PUTipoZona> tiposZonas = null;
            while (reader.Read())
            {
                if (tiposZonas == null)
                {
                    tiposZonas = new List<PUTipoZona>();
                }
                tiposZonas.Add(new PUTipoZona
                {
                    IdTipoZona=Convert.ToInt32(reader["TZO_IdTipoZona"]),
                    Descripcion=reader["TZO_Descripcion"].ToString()
                });
            }
            return tiposZonas;
        }
    }
}
