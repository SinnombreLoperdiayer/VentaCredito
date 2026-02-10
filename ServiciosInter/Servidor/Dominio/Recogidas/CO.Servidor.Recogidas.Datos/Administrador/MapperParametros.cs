using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using CO.Servidor.Servicios.ContratoDatos.Recogidas;

namespace CO.Servidor.Recogidas.Datos.Administrador
{
    internal class MapperParametros
    {
        internal static List<RGTerritorialDC> MapperTerritoriales(SqlDataReader reader)
        {
            List<RGTerritorialDC> resultado = null;
            if (reader.HasRows)
            {
                while (reader.Read())
                {

                    var r = new RGTerritorialDC
                    {
                        IdCentroLogistico=Convert.ToInt64(reader["CEL_IdCentroLogistico"]) ,
                        Nombre=Convert.ToString(reader["CES_Nombre"]) ,
                        NombreTerritorial=Convert.ToString(reader["TER_NombreTerritorial"]) ,
                        Descripcion=Convert.ToString(reader["REA_Descripcion"]) ,
                        IdRegionalAdm=Convert.ToString(reader["REA_IdRegionalAdm"]),
                        IdMunicipio=Convert.ToString(reader["CES_IdMunicipio"])
                    };

                    r.latitud=Convert.ToDecimal(DBNull.Value.Equals(reader["CES_latitud"]) ? 0 : reader["CES_latitud"]);
                    r.Longitud=Convert.ToDecimal(DBNull.Value.Equals(reader["CES_Longitud"]) ? 0 : reader["CES_Longitud"]);

                    if (resultado==null)
                    {
                        resultado=new List<RGTerritorialDC>();
                    }

                    resultado.Add(r);
                }
            }
            return resultado;
        }

        internal static List<RGAgenciaDC> MapperAgencias(SqlDataReader reader)
        {
            List<RGAgenciaDC> resultado = null;
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    var r = new RGAgenciaDC
                    {

                        IdCentroLogistico=Convert.ToInt64(reader["AGE_IdCentroLogistico"]) ,
                        IdAgencia=Convert.ToInt64(reader["AGE_IdAgencia"]) ,
                        IdPropietario=Convert.ToInt32(reader["CES_IdPropietario"]) ,
                        NOMBRE=Convert.ToString(reader["CES_NOMBRE"]) ,
                        Direccion=Convert.ToString(reader["CES_Direccion"]) ,
                        IdMunicipio=Convert.ToString(reader["CES_IdMunicipio"]) ,
                        IdPersonaResponsable=Convert.ToInt64(reader["CES_IdPersonaResponsable"]) ,                        
                        Email=Convert.ToString(reader["CES_Email"]) ,
                        IdCentroCostos=Convert.ToString(reader["CES_IdCentroCostos"]) ,
                        PesoMaximo=Convert.ToDecimal(reader["CES_PesoMaximo"]) ,
                        VolumenMaximo=Convert.ToDecimal(reader["CES_VolumenMaximo"]) ,
                        AdmiteFormaPagoAlCobro=Convert.ToBoolean(reader["CES_AdmiteFormaPagoAlCobro"]) ,
                        CodigoBodega=Convert.ToString(reader["CES_CodigoBodega"]) ,
                        IdClasificadorCanalVenta=Convert.ToInt16(reader["CES_IdClasificadorCanalVenta"])
                    };

                    r.Latitud=Convert.ToDecimal(DBNull.Value.Equals(reader["CES_Latitud"]) ? 0 : reader["CES_Latitud"]);
                    r.Longitud=Convert.ToDecimal(DBNull.Value.Equals(reader["CES_Longitud"]) ? 0 : reader["CES_Longitud"]);

                    if (resultado==null)
                    {
                        resultado=new List<RGAgenciaDC>();
                    }

                    resultado.Add(r);
                }
            }
            return resultado;
        }
    }
}