using System;
using System.Data.SqlClient;
using CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes;
using CO.Servidor.Servicios.ContratoDatos.Raps;
using System.Collections.Generic;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;

namespace CO.Servidor.Raps.Comun.Integraciones.Datos.Datos
{
    public class MapperRepositorioIntegracion
    {
        internal static RAInformacionUsuarioRAP ToRaInformacionUsuarioRAP(SqlDataReader reader)
        {
            RAInformacionUsuarioRAP resultado = null;

            if (reader.HasRows)
            {
                if (reader.Read())
                {
                    resultado = new RAInformacionUsuarioRAP
                    {
                        Identificacion = Convert.ToInt64(reader["Identificacion"]),
                        Correo = Convert.ToString(reader["Correo"]),
                        IdCargo = Convert.ToString(reader["IdCargo"]),
                    };
                }
            }

            return resultado;
        }


        internal static List<LIParametrizacionIntegracionRAPSDC> ToParametrizacionReglasRaps(SqlDataReader reader)
        {
            List<LIParametrizacionIntegracionRAPSDC> resultado = null;

            if (reader.HasRows)
            {
                LIParametrizacionIntegracionRAPSDC parametro = new LIParametrizacionIntegracionRAPSDC();
                resultado = new List<LIParametrizacionIntegracionRAPSDC>();

                while (reader.Read())
                {
                    resultado.Add(new LIParametrizacionIntegracionRAPSDC
                    {

                        NombreParametro = reader["MPI_NombreParametro"].ToString(),
                        IdParametro = Convert.ToInt32(reader["MPI_IdParametroparametrizacion"]),
                        TipoDato = Convert.ToInt32(reader["IdTipoDato"]),
                        Longitud = Convert.ToInt32(reader["Longitud"]),
                        DescripcionParametro = reader["DescripcionParametro"].ToString(),
                        ClaveTipoFalla = reader["MPI_ModuloParametro"].ToString(),
                        EsArray = reader["MPI_EsArray"] != DBNull.Value ? Convert.ToBoolean(reader["MPI_EsArray"]) : false,
                        NombreObjeto = reader["MPI_Objeto"] != DBNull.Value ? reader["MPI_Objeto"].ToString() : "",
                        NombrePropiedad = reader["MPI_Propiedad"] != DBNull.Value ? reader["MPI_Propiedad"].ToString() : "",
                        PosicionEnArray = reader["MPI_PosicionEnArray"] != DBNull.Value ? Convert.ToInt32(reader["MPI_PosicionEnArray"]) : 0,
                        IdMotivoController = reader["MPI_IdMotivoController"] == DBNull.Value ? Convert.ToInt16(0) : Convert.ToInt16(reader["MPI_IdMotivoController"]),
                        IdParametrizacion = reader["MPI_IdParametrizacion"] == DBNull.Value ? 0 : Convert.ToInt64(reader["MPI_IdParametrizacion"]),
                        IdFuncion = reader["MPI_IdFuncion"] == DBNull.Value ? 0 : Convert.ToInt32(reader["MPI_IdFuncion"]),
                        DescripcionFuncion = reader["MPI_DescripcionFuncion"] == DBNull.Value ? String.Empty : reader["MPI_DescripcionFuncion"].ToString(),
                        IdTipoNovedad = reader["MPI_IdTipoNovedad"] == DBNull.Value ? 0 : Convert.ToInt32(reader["MPI_IdTipoNovedad"]),
                        EsAgrupamiento = reader["EsAgrupamiento"] == DBNull.Value ? false : Convert.ToBoolean(reader["EsAgrupamiento"]),
                    });
                    //parametro.IdParametrizacionReglasEstados = Convert.ToInt64(reader["IdParametrizacionReglasEstados"]);
                    //parametro.IdMotivoController = Convert.ToInt16(reader["IdMotivoController"]);
                    //parametro.IdParametrizacion = Convert.ToInt64(reader["IdParametrizacion"]);
                    //parametro.IdParametroAsociado = Convert.ToInt32(reader["IdParametroAsociado"]);
                    //parametro.IdFuncion = Convert.ToInt32(reader["IdFuncion"]);
                    //parametro.DescripcionFuncion = reader["DescripcionFuncion"].ToString();
                    //parametro.DescripcionParametrizacion = reader["MPI_ModuloParametro"].ToString();

                    //   resultado.Add(parametro);
                }
            }
            return resultado;
        }


        /// <summary>
        /// mapper por responsable tipo novedad
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        internal static List<RAResponsableTipoNovedadDC> MapperObtenerResponsableTipoNovedad(SqlDataReader reader)
        {
            List<RAResponsableTipoNovedadDC> resultado = null;
            if (reader.HasRows)
            {
                resultado = new List<RAResponsableTipoNovedadDC>();

                while (reader.Read())
                {
                    resultado.Add(new RAResponsableTipoNovedadDC()
                    {
                        Id = reader["RTI_Id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["RTI_Id"]),
                        Nombre = reader["RTI_Nombre"] == DBNull.Value ? string.Empty : reader["RTI_Nombre"].ToString(),
                        EstadoOrigen = reader["RTI_EstadoOrigen"] == DBNull.Value ? 0 : Convert.ToInt32(reader["RTI_EstadoOrigen"]),
                        IdOrigenRaps = reader["RTI_IdOrigenRaps"] == DBNull.Value ? 0 : Convert.ToInt32(reader["RTI_IdOrigenRaps"]),
                        IdTipoNovedadHijo = reader["RTI_IdNovedad"] == DBNull.Value ? 0 : Convert.ToInt32(reader["RTI_IdNovedad"]),
                    });

                }

            }
            return resultado;

        }
        /// <summary>
        /// mapper parametros por id responsable 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        internal static List<RAParametrosPersonalizacionRapsDC> MapperObtenerParametrosPorIdResponsable(SqlDataReader reader)
        {
            List<RAParametrosPersonalizacionRapsDC> resultado = null;
            if (reader.HasRows)
            {
                resultado = new List<RAParametrosPersonalizacionRapsDC>();

                while (reader.Read())
                {
                    int idFuncion = reader["MPI_IdFuncion"] == DBNull.Value ? 0 : Convert.ToInt32(reader["MPI_IdFuncion"]);
                    if (idFuncion > 0)
                    {
                        resultado.Add(new RAParametrosPersonalizacionRapsDC()
                        {
                            IdParametro = reader["IdParametro"] == DBNull.Value ? 0 : Convert.ToInt32(reader["IdParametro"]),
                            IdFuncion = idFuncion,
                            Funcion = reader["MPI_DescripcionFuncion"] == DBNull.Value ? 0 : (EnumFuncionesReglasParametrizacionRaps)Enum.Parse(typeof(EnumFuncionesReglasParametrizacionRaps), reader["MPI_DescripcionFuncion"].ToString()),
                            EsAgrupamiento = reader["EsAgrupamiento"] == DBNull.Value ? false : Convert.ToBoolean(reader["EsAgrupamiento"]),
                            DescripcionParametro = reader["Descripcionparametro"] == DBNull.Value ? "" : reader["Descripcionparametro"].ToString(),
                            TipoDato = reader["idTipoDato"] == DBNull.Value ? 0 : Convert.ToInt32(reader["idTipoDato"]),
                            Longitud = reader["Longitud"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Longitud"])
                        });
                    }
                }

            }
            return resultado;

        }
        /// <summary>
        /// mapper parametros por id responsable 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        internal static List<RAParametrosPersonalizacionRapsDC> MapperObtenerParametrosPorIdResponsableGlobales(SqlDataReader reader)
        {
            List<RAParametrosPersonalizacionRapsDC> resultado = null;
            if (reader.HasRows)
            {
                resultado = new List<RAParametrosPersonalizacionRapsDC>();

                while (reader.Read())
                {
                    resultado.Add(new RAParametrosPersonalizacionRapsDC()
                    {
                        IdParametro = reader["PRN_IdFuncion"] == DBNull.Value ? 0 : Convert.ToInt32(reader["PRN_IdFuncion"]),
                        DescripcionParametro = reader["PRN_DescripcionParametro"] == DBNull.Value ? string.Empty : reader["PRN_DescripcionParametro"].ToString(),
                        TipoDato = reader["PRN_IdTipoDato"] == DBNull.Value ? 0 : Convert.ToInt32(reader["PRN_IdTipoDato"]),
                        Longitud = reader["PRN_Longitud"] == DBNull.Value ? 0 : Convert.ToInt32(reader["PRN_Longitud"]),
                        IdFuncion = reader["PRN_IdFuncion"] == DBNull.Value ? 0 : Convert.ToInt32(reader["PRN_IdFuncion"])
                    });

                }

            }
            return resultado;

        }
        /// <summary>
        /// mapper reglas integraciones manual
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        internal static RAReglasIngrecionesManualDC MapperObtenerReglasIntegracionesManual(SqlDataReader reader)
        {
            RAReglasIngrecionesManualDC resultado = null;
            if (reader.HasRows)
            {
                resultado = new RAReglasIngrecionesManualDC();
                if (reader.Read())
                {
                    resultado = new RAReglasIngrecionesManualDC()
                    {
                        IdRegla = Convert.ToInt32(reader["RIM_IdRegla"]),
                        NombreRegla = reader["RIM_DescripcionRegla"].ToString(),
                        IdEstado = Convert.ToInt32(reader["RIM_IdEstado"]),
                        Assembly = reader["RIM_Assembly"].ToString(),
                        NameSpace = reader["RIM_NameSpace"].ToString(),
                        Clase = reader["RIM_Clase"].ToString(),
                    };

                }

            }
            return resultado;
        }
    }
}
