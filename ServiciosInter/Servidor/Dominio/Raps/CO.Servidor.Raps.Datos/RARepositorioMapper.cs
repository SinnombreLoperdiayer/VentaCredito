
using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;
using CO.Servidor.Servicios.ContratoDatos.Raps.Consultas;
using CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
namespace CO.Servidor.Raps.Datos
{
    public class RARepositorioMapper
    {

        internal static List<RAAccionDC> MapperListaAccion(SqlDataReader reader)
        {
            List<RAAccionDC> resultado = null;
            while (reader.Read())
            {
                if (resultado == null)
                {
                    resultado = new List<RAAccionDC>();
                }

                var r = new RAAccionDC
                {
                    IdAccion = Convert.ToByte(reader["IdAccion"]),
                    Descripcion = Convert.ToString(reader["Descripcion"]),
                    Estado = Convert.ToBoolean(reader["estado"]),
                };

                resultado.Add(r);
            }

            return resultado;
        }

        internal static List<RACargoDC> MapperACargo(SqlDataReader reader)
        {
            List<RACargoDC> resultado = null;
            while (reader.Read())
            {
                if (resultado == null)
                {
                    resultado = new List<RACargoDC>();
                }
                var r = new RACargoDC
                {
                    IdCargo = reader["IdCargo"].ToString(),
                    Descripcion = Convert.ToString(reader["Descripcion"]),
                    Estado = Convert.ToBoolean(reader["estado"]),
                    Procedimiento = reader["IdProcedimiento"].ToString(),
                    CargoNovasoft = Convert.ToString(reader["CargoNovasoft"]),
                    EnteControl = reader["EnteControl"] != DBNull.Value ? Convert.ToBoolean(reader["EnteControl"]) : false,
                    Regional = reader["Regional"] != DBNull.Value ? Convert.ToBoolean(reader["Regional"]) : false
                };

                resultado.Add(r);
            }
            return resultado;
        }


        internal static RAAccionDC MapperAAccion(SqlDataReader reader)
        {
            RAAccionDC resultado = null;
            if (reader.Read())
            {
                var r = new RAAccionDC
                {
                    IdAccion = Convert.ToByte(reader["IdAccion"]),
                    Descripcion = Convert.ToString(reader["Descripcion"]),
                    Estado = Convert.ToBoolean(reader["estado"]),
                };

                resultado = r;
            }
            return resultado;
        }

        internal static List<RAAccionPlantillaParametrizacionRapsDC> MapperListaAccionPlantillaParametrizacionRaps(SqlDataReader reader)
        {
            List<RAAccionPlantillaParametrizacionRapsDC> resultado = null;
            while (reader.Read())
            {
                if (resultado == null)
                {
                    resultado = new List<RAAccionPlantillaParametrizacionRapsDC>();
                }
                var r = new RAAccionPlantillaParametrizacionRapsDC
                {
                    IdAccionPlantilla = Convert.ToInt64(reader["IdAccionPlantilla"]),
                    IdParametrizacionRap = Convert.ToInt64(reader["IdParametrizacionRap"]),
                    IdAccion = Convert.ToByte(reader["IdAccion"]),
                    IdPlantilla = Convert.ToInt64(reader["IdPlantilla"]),
                    Estado = Convert.ToBoolean(reader["Estado"]),
                };

                resultado.Add(r);
            }

            return resultado;
        }


        internal static RAAccionPlantillaParametrizacionRapsDC MapperAAccionPlantillaParametrizacionRaps(SqlDataReader reader)
        {
            RAAccionPlantillaParametrizacionRapsDC resultado = null;
            if (reader.Read())
            {
                var r = new RAAccionPlantillaParametrizacionRapsDC
                {
                    IdAccionPlantilla = Convert.ToInt64(reader["IdAccionPlantilla"]),
                    IdParametrizacionRap = Convert.ToInt64(reader["IdParametrizacionRap"]),
                    IdAccion = Convert.ToByte(reader["IdAccion"]),
                    IdPlantilla = Convert.ToInt64(reader["IdPlantilla"]),
                    Estado = Convert.ToBoolean(reader["Estado"]),
                };

                resultado = r;
            }
            return resultado;
        }


        internal static List<RAClasificacionDC> MapperListaClasificacion(SqlDataReader reader)
        {
            List<RAClasificacionDC> resultado = null;
            while (reader.Read())
            {
                if (resultado == null)
                {
                    resultado = new List<RAClasificacionDC>();
                }
                var r = new RAClasificacionDC
                {
                    IdClasificacion = Convert.ToInt32(reader["IdClasificacion"]),
                    Descripcion = Convert.ToString(reader["Descripcion"]),
                    Estado = Convert.ToBoolean(reader["Estado"]),
                };

                resultado.Add(r);
            }

            return resultado;
        }

        internal static RAClasificacionDC MapperAClasificacion(SqlDataReader reader)
        {
            RAClasificacionDC resultado = null;
            if (reader.Read())
            {
                var r = new RAClasificacionDC
                {
                    IdClasificacion = Convert.ToInt32(reader["IdClasificacion"]),
                    Descripcion = Convert.ToString(reader["Descripcion"]),
                    Estado = Convert.ToBoolean(reader["Estado"]),
                };

                resultado = r;
            }
            return resultado;
        }

        internal static List<RAEscalonamientoDC> MapperListaEscalonamiento(SqlDataReader reader)
        {
            List<RAEscalonamientoDC> resultado = null;
            while (reader.Read())
            {
                if (resultado == null)
                {
                    resultado = new List<RAEscalonamientoDC>();
                }
                var r = new RAEscalonamientoDC
                {

                    IdParametrizacionRap = Convert.ToInt64(reader["IdParametrizacionRap"]),
                    idCargo = reader["idCargo"].ToString(),
                    Orden = Convert.ToByte(reader["orden"]),
                    IdTipoHora = Convert.ToInt32(reader["IdTipoHora"]),
                    HorasEscalar = Convert.ToByte(reader["HorasEscalar"]),
                };

                resultado.Add(r);
            }
            return resultado;
        }


        internal static RAEscalonamientoDC MapperAEscalonamiento(SqlDataReader reader)
        {
            RAEscalonamientoDC resultado = null;
            if (reader.Read())
            {
                var r = new RAEscalonamientoDC
                {
                    // IdEscalonamiento = Convert.ToInt64(reader["IdEscalonamiento"]),
                    IdParametrizacionRap = Convert.ToInt64(reader["IdParametrizacionRap"]),
                    idCargo = reader["idCargo"].ToString(),
                    Orden = Convert.ToByte(reader["orden"]),
                    IdTipoHora = Convert.ToInt32(reader["IdTipoHora"]),
                    HorasEscalar = Convert.ToByte(reader["HorasEscalar"]),
                };

                resultado = r;
            }
            return resultado;
        }

        internal static List<RAEstadosDC> MapperListaEstados(SqlDataReader reader)
        {
            List<RAEstadosDC> resultado = new List<RAEstadosDC>(); ;
            while (reader.Read())
            {
                var r = new RAEstadosDC
                {
                    IdEstado = Convert.ToInt32(reader["IdEstado"]),
                    Descripcion = Convert.ToString(reader["Descripcion"]),
                    Estado = Convert.ToBoolean(reader["Estado"]),
                };

                resultado.Add(r);
            }

            return resultado;
        }


        internal static RAEstadosDC MapperAEstados(SqlDataReader reader)
        {
            RAEstadosDC resultado = null;
            if (reader.Read())
            {
                var r = new RAEstadosDC
                {
                    IdEstado = Convert.ToInt32(reader["IdEstado"]),
                    Descripcion = Convert.ToString(reader["Descripcion"]),
                    Estado = Convert.ToBoolean(reader["Estado"]),
                };

                resultado = r;
            }

            return resultado;
        }

        internal static List<RAFlujoAccionEstadoDC> MapperListaFlujoAccionEstado(SqlDataReader reader)
        {
            List<RAFlujoAccionEstadoDC> resultado = null;
            while (reader.Read())
            {
                if (resultado == null)
                {
                    resultado = new List<RAFlujoAccionEstadoDC>();
                }
                var r = new RAFlujoAccionEstadoDC
                {
                    IdFlujo = Convert.ToInt32(reader["IdFlujo"]),
                    IdAccion = Convert.ToByte(reader["IdAccion"]),
                    IdEstado = Convert.ToInt32(reader["idEstado"]),
                    IdCargo = Convert.ToInt32(reader["IdCargo"]),
                    IdEstadoFinal = Convert.ToInt32(reader["IdEstadoFinal"]),
                    IdCargoFinal = Convert.ToInt32(reader["IdCargoFinal"]),
                };

                resultado.Add(r);
            }

            return resultado;
        }


        internal static RAFlujoAccionEstadoDC MapperAFlujoAccionEstado(SqlDataReader reader)
        {
            RAFlujoAccionEstadoDC resultado = null;
            if (reader.Read())
            {
                var r = new RAFlujoAccionEstadoDC
                {
                    IdFlujo = Convert.ToInt32(reader["IdFlujo"]),
                    IdAccion = Convert.ToByte(reader["IdAccion"]),
                    IdEstado = Convert.ToInt32(reader["idEstado"]),
                    IdCargo = Convert.ToInt32(reader["IdCargo"]),
                    IdEstadoFinal = Convert.ToInt32(reader["IdEstadoFinal"]),
                    IdCargoFinal = Convert.ToInt32(reader["IdCargoFinal"]),
                };

                resultado = r;
            }

            return resultado;
        }

        internal static List<RAFormatoDC> MapperListaFormato(SqlDataReader reader)
        {
            List<RAFormatoDC> resultado = null;
            while (reader.Read())
            {
                if (resultado == null)
                {
                    resultado = new List<RAFormatoDC>();
                }
                var r = new RAFormatoDC
                {
                    IdFormato = Convert.ToInt32(reader["IdFormato"]),
                    Descripcion = Convert.ToString(reader["Descripcion"]),
                    Estado = Convert.ToBoolean(reader["Estado"]),
                    IdSistemaFormato = Convert.ToInt32(reader["IdSistemaFormato"]),
                };

                resultado.Add(r);
            }

            return resultado;
        }


        internal static RAFormatoDC MapperAFormato(SqlDataReader reader)
        {
            RAFormatoDC resultado = null;
            if (reader.Read())
            {
                var r = new RAFormatoDC
                {

                    IdFormato = Convert.ToInt32(reader["IdFormato"]),
                    Descripcion = Convert.ToString(reader["Descripcion"]),
                    Estado = Convert.ToBoolean(reader["Estado"]),
                    IdSistemaFormato = Convert.ToInt32(reader["IdSistemaFormato"]),

                };

                resultado = r;
            }

            return resultado;
        }

        internal static List<RAGrupoUsuarioDC> MapperListaGrupoUsuario(SqlDataReader reader)
        {
            List<RAGrupoUsuarioDC> resultado = null;
            while (reader.Read())
            {
                if (resultado == null)
                {
                    resultado = new List<RAGrupoUsuarioDC>();
                }
                var r = new RAGrupoUsuarioDC
                {
                    IdGrupoUsuario = Convert.ToInt32(reader["IdGrupoUsuario"]),
                    Descripcion = Convert.ToString(reader["Descripcion"]),
                    Estado = Convert.ToBoolean(reader["estado"]),
                };

                resultado.Add(r);
            }

            return resultado;
        }


        internal static RAGrupoUsuarioDC MapperAGrupoUsuario(SqlDataReader reader)
        {
            RAGrupoUsuarioDC resultado = null;
            if (reader.Read())
            {
                var r = new RAGrupoUsuarioDC
                {

                    IdGrupoUsuario = Convert.ToInt32(reader["IdGrupoUsuario"]),
                    Descripcion = Convert.ToString(reader["Descripcion"]),
                    Estado = Convert.ToBoolean(reader["estado"]),
                };

                resultado = r;
            }

            return resultado;
        }

        internal static List<RAPantillaAccionCorreoDC> MapperListaPantillaAccionCorreo(SqlDataReader reader)
        {
            List<RAPantillaAccionCorreoDC> resultado = null;
            while (reader.Read())
            {
                if (resultado == null)
                {
                    resultado = new List<RAPantillaAccionCorreoDC>();
                }
                var r = new RAPantillaAccionCorreoDC
                {
                    IdPlantilla = Convert.ToInt64(reader["IdPlantilla"]),
                    IdAccion = Convert.ToByte(reader["IdAccion"]),
                    Asunto = Convert.ToString(reader["Asunto"]),
                    Cuerpo = Convert.ToString(reader["Cuerpo"]),
                    EsPredeterminada = Convert.ToBoolean(reader["EsPredeterminada"]),
                    Estado = Convert.ToBoolean(reader["Estado"]),
                };

                resultado.Add(r);
            }

            return resultado;
        }

        internal static List<RAParametrizacionRapsDC> MapperListaParametrizacionRaps(SqlDataReader reader)
        {
            List<RAParametrizacionRapsDC> resultado = null;
            while (reader.Read())
            {
                if (resultado == null)
                {
                    resultado = new List<RAParametrizacionRapsDC>();
                }
                var r = new RAParametrizacionRapsDC
                {
                    IdParametrizacionRap = Convert.ToInt64(reader["IdParametrizacionRap"]),
                    Nombre = Convert.ToString(reader["Nombre"]),
                    IdSistemaFuente = Convert.ToInt32(reader["IdSistemaFuente"]),
                    IdTipoRap = Convert.ToInt32(reader["IdTipoRap"]),
                    DescripcionRaps = Convert.ToString(reader["DescripcionRaps"]),
                    IdProceso = Convert.ToInt32(reader["IdProceso"]),
                    UtilizaFormato = Convert.ToBoolean(reader["UtilizaFormato"]),
                    IdFormato = Convert.ToInt32(reader["IdFormato"]),

                    IdTipoCierre = Convert.ToInt32(reader["IdTipoCierre"]),
                    IdCargoCierra = reader["IdCargoCierra"].ToString(),
                    IdCargoIncumplimiento = reader["IdCargoIncumplimiento"].ToString(),
                    IdOrigenRaps = Convert.ToInt32(reader["IdOrigenRaps"]),
                    Estado = Convert.ToBoolean(reader["Estado"]),
                    IdGrupoUsuario = Convert.ToInt32(reader["IdGrupoUsuario"]),
                    IdSubclasificacion = Convert.ToInt32(reader["IdSubclasificacion"]),
                    IdTipoPeriodo = Convert.ToInt32(reader["IdTipoPeriodo"]),
                    IdHoraEscalar = Convert.ToInt32(reader["IdHoraEscalar"]),
                    IdTipoHora = Convert.ToInt32(reader["IdTipoHora"]),
                    IdParametrizacionPadre = Convert.ToInt64(reader["IdParametrizacionPadre"]),
                };

                resultado.Add(r);
            }
            return resultado;
        }


        public static RAParametrizacionRapsDC MapperAParametrizacionRaps(SqlDataReader reader)
        {
            RAParametrizacionRapsDC resultado = null;
            if (reader.Read())
            {
                var r = new RAParametrizacionRapsDC
                {
                    IdParametrizacionRap = Convert.ToInt64(reader["IdParametrizacionRap"]),
                    Nombre = Convert.ToString(reader["Nombre"]),
                    IdSistemaFuente = Convert.ToInt32(reader["IdSistemaFuente"]),
                    IdTipoRap = Convert.ToInt32(reader["IdTipoRap"]),
                    DescripcionRaps = Convert.ToString(reader["DescripcionRaps"]),
                    IdProceso = Convert.ToInt32(reader["IdProceso"]),
                    UtilizaFormato = Convert.ToBoolean(reader["UtilizaFormato"]),
                    IdFormato = Convert.ToInt32(reader["IdFormato"]),
                    IdTipoCierre = Convert.ToInt32(reader["IdTipoCierre"]),
                    IdCargoCierra = reader["IdCargoCierra"].ToString(),
                    IdCargoIncumplimiento = reader["IdCargoIncumplimiento"].ToString(),
                    IdOrigenRaps = Convert.ToInt32(reader["IdOrigenRaps"]),
                    Estado = Convert.ToBoolean(reader["Estado"]),
                    IdGrupoUsuario = Convert.ToInt32(reader["IdGrupoUsuario"]),
                    IdSubclasificacion = Convert.ToInt32(reader["IdSubclasificacion"]),
                    IdTipoPeriodo = Convert.ToInt32(reader["IdTipoPeriodo"]),
                    IdHoraEscalar = Convert.ToInt32(reader["IdHoraEscalar"]),
                    IdTipoHora = Convert.ToInt32(reader["IdTipoHora"]),
                    IdTipoIncumplimiento = Convert.ToInt32(reader["IdTipoIncumplimiento"]),
                    IdParametrizacionPadre = Convert.ToInt64(reader["IdParametrizacionPadre"]),
                    IdNivelGravedad = Convert.ToInt32(reader["NivelGravedad"]),
                };
                resultado = r;
            }
            return resultado;
        }

        internal static List<RAProcesoDC> MapperListaProceso(SqlDataReader reader)
        {
            List<RAProcesoDC> resultado = null;
            while (reader.Read())
            {
                if (resultado == null)
                {
                    resultado = new List<RAProcesoDC>();
                }
                var r = new RAProcesoDC
                {
                    IdProceso = reader["IdProceso"].ToString(),
                    Descripcion = Convert.ToString(reader["Descripcion"]),
                    IdMacroProceso = Convert.ToInt32(reader["IdMacroProceso"]),
                    Estado = Convert.ToBoolean(reader["estado"]),
                };

                resultado.Add(r);
            }
            return resultado;
        }


        internal static RAProcesoDC MapperAProceso(SqlDataReader reader)
        {
            RAProcesoDC resultado = null;
            if (reader.Read())
            {
                var r = new RAProcesoDC
                {
                    IdProceso = reader["IdProceso"].ToString(),
                    Descripcion = Convert.ToString(reader["Descripcion"]),
                    IdMacroProceso = Convert.ToInt32(reader["IdMacroProceso"]),
                    Estado = Convert.ToBoolean(reader["Estado"]),
                };
                resultado = r;
            }
            return resultado;
        }

        internal static List<RATipoCierreDC> MapperListaTipoCierre(SqlDataReader reader)
        {
            List<RATipoCierreDC> resultado = null;
            while (reader.Read())
            {
                if (resultado == null)
                {
                    resultado = new List<RATipoCierreDC>();
                }
                var r = new RATipoCierreDC
                {
                    IdTipoCierre = Convert.ToInt32(reader["IdTipoCierre"]),
                    Descripcion = Convert.ToString(reader["Descripcion"]),
                    Estado = Convert.ToBoolean(reader["Estado"]),
                };

                resultado.Add(r);
            }
            return resultado;
        }


        internal static RATipoCierreDC MapperAtipoCierre(SqlDataReader reader)
        {
            RATipoCierreDC resultado = null;
            if (reader.Read())
            {
                var r = new RATipoCierreDC
                {
                    IdTipoCierre = Convert.ToInt32(reader["IdTipoCierre"]),
                    Descripcion = Convert.ToString(reader["Descripcion"]),
                    Estado = Convert.ToBoolean(reader["estado"]),
                };

                resultado = r;
            }
            return resultado;
        }

        internal static List<RASistemaFormatoDC> MapperListaSistemaFormato(SqlDataReader reader)
        {
            List<RASistemaFormatoDC> resultado = null;
            while (reader.Read())
            {
                if (resultado == null)
                {
                    resultado = new List<RASistemaFormatoDC>();
                }
                var r = new RASistemaFormatoDC
                {
                    IdSistemaFormato = Convert.ToInt32(reader["IdSistemaFormato"]),
                    Descripcion = Convert.ToString(reader["Descripcion"]),
                    Estado = Convert.ToBoolean(reader["Estado"]),
                };

                resultado.Add(r);
            }
            return resultado;
        }


        internal static RASistemaFormatoDC MapperASistemaFormato(SqlDataReader reader)
        {
            RASistemaFormatoDC resultado = null;
            if (reader.Read())
            {
                var r = new RASistemaFormatoDC
                {
                    IdSistemaFormato = Convert.ToInt32(reader["IdSistemaFormato"]),
                    Descripcion = Convert.ToString(reader["Descripcion"]),
                    Estado = Convert.ToBoolean(reader["Estado"]),
                };

                resultado = r;
            }

            return resultado;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        internal static List<RASubClasificacionDC> MapperListaSubClasificacion(SqlDataReader reader)
        {
            List<RASubClasificacionDC> resultado = null;
            while (reader.Read())
            {
                if (resultado == null)
                {
                    resultado = new List<RASubClasificacionDC>();
                }
                var r = new RASubClasificacionDC
                {
                    IdSubclasificacion = Convert.ToInt32(reader["IdSubclasificacion"]),
                    Descripcion = Convert.ToString(reader["Descripcion"]),
                    IdClasificacion = Convert.ToInt32(reader["IdClasificacion"]),
                    Estado = Convert.ToBoolean(reader["Estado"]),
                };
                resultado.Add(r);
            }
            return resultado;
        }


        internal static RASubClasificacionDC MapperASubClasificacion(SqlDataReader reader)
        {
            RASubClasificacionDC resultado = null;
            if (reader.Read())
            {
                var r = new RASubClasificacionDC
                {
                    IdSubclasificacion = Convert.ToInt32(reader["IdSubclasificacion"]),
                    Descripcion = Convert.ToString(reader["Descripcion"]),
                    IdClasificacion = Convert.ToInt32(reader["IdClasificacion"]),
                    Estado = Convert.ToBoolean(reader["Estado"]),
                };
                resultado = r;
            }
            return resultado;
        }

        internal static List<RATiempoEjecucionRapsDC> MapperListaTiempoEjecucionRaps(SqlDataReader reader)
        {
            List<RATiempoEjecucionRapsDC> resultado = null;
            while (reader.Read())
            {
                if (resultado == null)
                {
                    resultado = new List<RATiempoEjecucionRapsDC>();
                }
                var r = new RATiempoEjecucionRapsDC
                {
                    NumeroEjecucion = Convert.ToInt64(reader["NumeroEjecucion"]),
                    IdParametrizacionRap = Convert.ToInt64(reader["IdParametrizacionRap"]),
                    idTipoPeriodo = Convert.ToInt32(reader["idTipoPeriodo"]),
                    DiaPeriodo = Convert.ToInt32(reader["DiaPeriodo"]),
                };

                //if (!DBNull.Value.Equals(reader[" Hora"]))
                //{
                //    r.Hora = Convert.ToDateTime(reader["FechaGrabacion"]).TimeOfDay;
                //}
                if (!DBNull.Value.Equals(reader["Hora"]))
                {
                    DateTime result = new DateTime();
                    r.Hora = DateTime.Now.AddHours(15);
                    if (DateTime.TryParse(reader["Hora"].ToString(), out result))
                    {
                        r.Hora = result;
                    }
                }
                resultado.Add(r);
            }
            return resultado;
        }


        internal static RATiempoEjecucionRapsDC MapperATiempoEjecucionRaps(SqlDataReader reader)
        {
            RATiempoEjecucionRapsDC resultado = null;
            if (reader.Read())
            {
                var r = new RATiempoEjecucionRapsDC
                {
                    NumeroEjecucion = Convert.ToInt64(reader["IdEjecucion"]),
                    IdParametrizacionRap = Convert.ToInt64(reader["IdParametrizacionRap"]),
                    idTipoPeriodo = Convert.ToInt32(reader["idTipoPeriodo"]),
                    DiaPeriodo = Convert.ToInt32(reader["DiaPeriodo"]),
                };

                if (!DBNull.Value.Equals(reader["Hora"]))
                {
                    //r.Hora = Convert.ToDateTime(reader["FechaGrabacion"]).TimeOfDay;
                    //r.Hora = Convert.ToDateTime(reader["Hora"]).TimeOfDay;
                    DateTime result = new DateTime();
                    r.Hora = DateTime.Now.AddHours(15);
                    if (DateTime.TryParse(reader["Hora"].ToString(), out result))
                    {
                        r.Hora = result;
                    }
                }

                resultado = r;
            }

            return resultado;
        }

        internal static List<RATipoHoraDC> MapperListaTipoHora(SqlDataReader reader)
        {
            List<RATipoHoraDC> resultado = null;
            while (reader.Read())
            {
                if (resultado == null)
                {
                    resultado = new List<RATipoHoraDC>();
                }

                var r = new RATipoHoraDC
                {
                    IdTipoHora = Convert.ToInt32(reader["IdTipoHora"]),
                    Descripcion = Convert.ToString(reader["Descripcion"]),
                    Estado = Convert.ToBoolean(reader["estado"]),
                };

                resultado.Add(r);
            }

            return resultado;
        }


        internal static RATipoHoraDC MapperATipoHora(SqlDataReader reader)
        {
            RATipoHoraDC resultado = null;
            if (reader.Read())
            {
                var r = new RATipoHoraDC
                {
                    IdTipoHora = Convert.ToInt32(reader["IdTipoHora"]),
                    Descripcion = Convert.ToString(reader["Descripcion"]),
                    Estado = Convert.ToBoolean(reader["estado"]),
                };

                resultado = r;
            }
            return resultado;
        }

        internal static List<RATipoPeriodoDC> MapperListaTipoPeriodo(SqlDataReader reader)
        {
            List<RATipoPeriodoDC> resultado = null;
            while (reader.Read())
            {
                if (resultado == null)
                {
                    resultado = new List<RATipoPeriodoDC>();
                }
                var r = new RATipoPeriodoDC
                {
                    IdTipoPeriodo = Convert.ToInt32(reader["idTipoPeriodo"]),
                    Descripcion = Convert.ToString(reader["Descripcion"]),
                    Periodos = Convert.ToInt32(reader["Periodos"]),
                    Estado = Convert.ToBoolean(reader["Estado"]),
                };

                resultado.Add(r);
            }

            return resultado;
        }


        internal static RATipoPeriodoDC MapperATipoPeriodo(SqlDataReader reader)
        {
            RATipoPeriodoDC resultado = null;
            if (reader.Read())
            {
                var r = new RATipoPeriodoDC
                {
                    IdTipoPeriodo = Convert.ToInt32(reader["idTipoPeriodo"]),
                    Descripcion = Convert.ToString(reader["Descripcion"]),
                    Periodos = Convert.ToInt32(reader["Periodos"]),
                    Estado = Convert.ToBoolean(reader["Estado"]),
                };
                resultado = r;
            }
            return resultado;
        }

        internal static List<RATipoRapDC> MapperListaTipoRap(SqlDataReader reader)
        {
            List<RATipoRapDC> resultado = null;
            while (reader.Read())
            {
                if (resultado == null)
                {
                    resultado = new List<RATipoRapDC>();
                }
                var r = new RATipoRapDC
                {
                    IdTipoRap = Convert.ToInt32(reader["IdTipoRap"]),
                    Descripcion = Convert.ToString(reader["Descripcion"]),
                    Estado = Convert.ToBoolean(reader["Estado"]),
                };
                resultado.Add(r);
            }
            return resultado;
        }


        internal static RATipoRapDC MapperATipoRap(SqlDataReader reader)
        {
            RATipoRapDC resultado = null;
            if (reader.Read())
            {
                var r = new RATipoRapDC
                {
                    IdTipoRap = Convert.ToInt32(reader["IdTipoRap"]),
                    Descripcion = Convert.ToString(reader["Descripcion"]),
                    Estado = Convert.ToBoolean(reader["Estado"]),
                };
                resultado = r;
            }
            return resultado;
        }

        internal static List<RAMacroprocesoDC> MapperListaMacroproceso(SqlDataReader reader)
        {
            List<RAMacroprocesoDC> resultado = null;
            while (reader.Read())
            {
                if (resultado == null)
                {
                    resultado = new List<RAMacroprocesoDC>();
                }
                var r = new RAMacroprocesoDC
                {
                    IdMacroProceso = Convert.ToInt32(reader["idMacriProceso"]),
                    Descripcion = Convert.ToString(reader["Descripcion"]),
                    Estado = Convert.ToBoolean(reader["Estado"]),
                };
                resultado.Add(r);
            }
            return resultado;
        }


        internal static RAMacroprocesoDC MapperAMacroproceso(SqlDataReader reader)
        {
            RAMacroprocesoDC resultado = null;
            if (reader.Read())
            {
                var r = new RAMacroprocesoDC
                {
                    IdMacroProceso = Convert.ToInt32(reader["idMacriProceso"]),
                    Descripcion = Convert.ToString(reader["Descripcion"]),
                    Estado = Convert.ToBoolean(reader["Estado"]),
                };
                resultado = r;
            }
            return resultado;
        }

        internal static List<RAProcedimientoDC> MapperListaProcedimiento(SqlDataReader reader)
        {
            List<RAProcedimientoDC> resultado = null;
            while (reader.Read())
            {
                if (resultado == null)
                {
                    resultado = new List<RAProcedimientoDC>();
                }
                var r = new RAProcedimientoDC
                {
                    IdProcedimiento = reader["IdProcedimiento"].ToString(),
                    Descripcion = Convert.ToString(reader["Descripcion"]),
                    Estado = Convert.ToBoolean(reader["Estado"]),
                    IdProceso = Convert.ToInt32(reader["IdProceso"]),
                };

                resultado.Add(r);
            }

            return resultado;
        }


        internal static RAProcedimientoDC MapperAProcedimiento(SqlDataReader reader)
        {
            RAProcedimientoDC resultado = null;
            if (reader.Read())
            {
                var r = new RAProcedimientoDC
                {
                    IdProcedimiento = reader["IdProcedimiento"].ToString(),
                    Descripcion = Convert.ToString(reader["Descripcion"]),
                    Estado = Convert.ToBoolean(reader["Estado"]),
                    IdProceso = Convert.ToInt32(reader["IdProceso"]),
                };


                resultado = r;
            }

            return resultado;
        }

        internal static List<RAUsuarioDeGrupoDC> MapperListaUsuarioDeGrupo(SqlDataReader reader)
        {
            List<RAUsuarioDeGrupoDC> resultado = null;
            while (reader.Read())
            {
                if (resultado == null)
                {
                    resultado = new List<RAUsuarioDeGrupoDC>();
                }
                var r = new RAUsuarioDeGrupoDC
                {

                    IdCargo = Convert.ToInt32(reader["IdCargo"]),
                    idUsuarioGrupo = Convert.ToInt32(reader["idUsuarioGrupo"]),

                };


                resultado.Add(r);
            }
            return resultado;
        }

        internal static RAUsuarioDeGrupoDC MapperAUsuarioDeGrupo(SqlDataReader reader)
        {
            RAUsuarioDeGrupoDC resultado = null;
            if (reader.Read())
            {
                var r = new RAUsuarioDeGrupoDC
                {

                    IdCargo = Convert.ToInt32(reader["IdCargo"]),
                    idUsuarioGrupo = Convert.ToInt32(reader["idUsuarioGrupo"]),

                };


                resultado = r;
            }
            return resultado;
        }

        public static List<RAOrigenRapsDC> MapperListaOrigenRaps(SqlDataReader reader)
        {
            List<RAOrigenRapsDC> resultado = null;
            while (reader.Read())
            {
                if (resultado == null)
                {
                    resultado = new List<RAOrigenRapsDC>();
                }
                var r = new RAOrigenRapsDC
                {
                    IdOrigenRaps = Convert.ToInt32(reader["IdOrigenRaps"]),
                    descripcion = Convert.ToString(reader["descripcion"]),
                    Estado = Convert.ToBoolean(reader["Estado"]),
                };

                resultado.Add(r);
            }

            return resultado;
        }


        public static List<RAFormatoDC> MapperAListarFormato(SqlDataReader reader)
        {
            List<RAFormatoDC> resultado = null;
            while (reader.Read())
            {
                if (resultado == null)
                {
                    resultado = new List<RAFormatoDC>();
                }
                var r = new RAFormatoDC
                {
                    IdFormato = Convert.ToInt32(reader["IdFormato"]),
                    IdSistemaFormato = Convert.ToInt32(reader["IdSistemaFormato"]),
                    Descripcion = Convert.ToString(reader["Descripcion"]),
                    Estado = Convert.ToBoolean(reader["Estado"]),
                };

                resultado.Add(r);
            }

            return resultado;
        }

        public static IEnumerable<RAParametrizacionRapsDC> MapperAListaParametrizacionRaps(SqlDataReader reader)
        {
            List<RAParametrizacionRapsDC> resultado = null;
            while (reader.Read())
            {
                if (resultado == null)
                {
                    resultado = new List<RAParametrizacionRapsDC>();
                }

                var r = new RAParametrizacionRapsDC
                {
                    IdParametrizacionRap = Convert.ToInt64(reader["IdParametrizacionRap"]),
                    Nombre = Convert.ToString(reader["Nombre"]),
                    IdSistemaFuente = Convert.ToInt32(reader["IdSistemaFuente"]),
                    IdTipoRap = Convert.ToInt32(reader["IdTipoRap"]),
                    DescripcionRaps = Convert.ToString(reader["DescripcionRaps"]),
                    IdProceso = Convert.ToInt32(reader["IdProceso"]),
                    UtilizaFormato = Convert.ToBoolean(reader["UtilizaFormato"]),
                    IdFormato = Convert.ToInt32(reader["IdFormato"]),
                    IdTipoCierre = Convert.ToInt32(reader["IdTipoCierre"]),
                    IdCargoCierra = reader["IdCargoCierra"].ToString(),
                    IdCargoIncumplimiento = reader["IdCargoIncumplimiento"].ToString(),
                    IdOrigenRaps = Convert.ToInt32(reader["IdOrigenRaps"]),
                    Estado = Convert.ToBoolean(reader["Estado"]),
                    IdGrupoUsuario = Convert.ToInt32(reader["IdGrupoUsuario"]),
                    IdSubclasificacion = Convert.ToInt32(reader["IdSubclasificacion"]),
                };

                resultado.Add(r);
            }

            return resultado;
        }

        internal static RAOrigenRapsDC MapperAOrigenRaps(SqlDataReader reader)
        {
            RAOrigenRapsDC resultado = null;
            if (reader.Read())
            {
                var r = new RAOrigenRapsDC
                {
                    IdOrigenRaps = Convert.ToInt32(reader["IdOrigenRaps"]),
                    descripcion = Convert.ToString(reader["descripcion"]),
                    Estado = Convert.ToBoolean(reader["Estado"]),
                };

                resultado = r;
            }
            return resultado;
        }

        public static List<RATipoIncumplimientoDC> MapperListaTipoIncumplimiento(SqlDataReader reader)
        {
            List<RATipoIncumplimientoDC> resultado = null;
            while (reader.Read())
            {
                if (resultado == null)
                {
                    resultado = new List<RATipoIncumplimientoDC>();
                }
                var r = new RATipoIncumplimientoDC
                {
                    IdTipoIncumplimiento = Convert.ToInt32(reader["IdTipoIncumplimiento"]),
                    Descripcion = Convert.ToString(reader["Descripcion"]),
                    Estado = Convert.ToBoolean(reader["Estado"]),
                };

                resultado.Add(r);
            }

            return resultado;
        }

        public static List<RARegionalSuculsalDC> MapperListaRegionalSucursal(SqlDataReader reader)
        {
            List<RARegionalSuculsalDC> resultado = null;
            while (reader.Read())
            {
                if (resultado == null)
                {
                    resultado = new List<RARegionalSuculsalDC>();
                }
                var r = new RARegionalSuculsalDC
                {
                    CodSucursal = Convert.ToString(reader["cod_suc"]),
                    NombreSucursal = Convert.ToString(reader["nom_suc"]),
                    IdCargo = Convert.ToInt32(reader["IdCargo"]),
                    Descripcion = Convert.ToString(reader["Descripcion"]),
                    Estado = Convert.ToBoolean(reader["Estado"]),
                    IdProcedimiento = Convert.ToInt32(reader["IdProcedimiento"]),
                    CargoNovasoft = Convert.ToString(reader["CargoNovasoft"]),
                    CodigoSucursal = Convert.ToString(reader["CodigoSucursal"]),
                };
                resultado.Add(r);
            }
            return resultado;
        }

        public static Dictionary<RAEnumAccion, RAEnumEstados> AccionesEstadoFinal
        {
            get
            {
                return new Dictionary<RAEnumAccion, RAEnumEstados>
                    {
                        {RAEnumAccion.None, RAEnumEstados.None},
                        {RAEnumAccion.Crear, RAEnumEstados.Creada    },
                        {RAEnumAccion.Gestionar, RAEnumEstados.Respuesta },
                        {RAEnumAccion.Revisar, RAEnumEstados.Revisado },
                        {RAEnumAccion.Escalar, RAEnumEstados.Escalado },
                        {RAEnumAccion.Asignar, RAEnumEstados.Asignado },
                        {RAEnumAccion.Cerrar,  RAEnumEstados.Cerrado },
                        {RAEnumAccion.Vencer,  RAEnumEstados.Vencido },
                    };
            }

        }


        public static RAEnumEstados NuevoEstado(RAEnumAccion accion)
        {

            RAEnumEstados resultado = RAEnumEstados.None;

            var accionEstadoFinal = RARepositorioMapper.AccionesEstadoFinal;

            if (accionEstadoFinal.ContainsKey(accion))
            {
                resultado = accionEstadoFinal[accion];
            }

            return resultado;
        }

        public static RATipoIncumplimientoDC MapperATipoIncumplimiento(SqlDataReader reader)
        {
            RATipoIncumplimientoDC resultado = null;
            if (reader.Read())
            {
                var r = new RATipoIncumplimientoDC
                {
                    IdTipoIncumplimiento = Convert.ToInt32(reader["IdTipoIncumplimiento"]),
                    Descripcion = Convert.ToString(reader["Descripcion"]),
                    Estado = Convert.ToBoolean(reader["Estado"]),
                };

                resultado = r;
            }

            return resultado;
        }

        public static List<RAPaginaParametrizacionRapsDC> MapperAPaginaParametrizacionRaps(SqlDataReader reader)
        {
            List<RAPaginaParametrizacionRapsDC> resultado = null;
            while (reader.Read())
            {
                if (resultado == null)
                {
                    resultado = new List<RAPaginaParametrizacionRapsDC>();
                }

                var r = new RAPaginaParametrizacionRapsDC
                {
                    Registro = Convert.ToInt64(reader["Registro"]),
                    IdParametrizacionRap = Convert.ToInt64(reader["IdParametrizacionRap"]),
                    Nombre = Convert.ToString(reader["Nombre"]),
                    IdSistemaFuente = Convert.ToInt32(reader["IdSistemaFuente"]),
                    SistemaFuente = Convert.ToString(reader["SistemaFuente"]),
                    IdTipoRap = Convert.ToInt32(reader["IdTipoRap"]),
                    TipoRap = Convert.ToString(reader["TipoRap"]),
                    DescripcionRaps = Convert.ToString(reader["DescripcionRaps"]),
                    IdProceso = Convert.ToInt32(reader["IdProceso"]),
                    Proceso = Convert.ToString(reader["Proceso"]),
                    UtilizaFormato = Convert.ToBoolean(reader["UtilizaFormato"]),
                    IdFormato = Convert.ToInt32(reader["IdFormato"]),
                    Formato = Convert.ToString(reader["Formato"]),
                    IdTipoCierre = Convert.ToInt32(reader["IdTipoCierre"]),
                    TipoCierre = Convert.ToString(reader["TipoCierre"]),
                    IdCargoCierra = Convert.ToInt32(reader["IdCargoCierra"]),
                    CargoCierra = Convert.ToString(reader["CargoCierra"]),
                    IdCargoIncumplimiento = Convert.ToInt32(reader["IdCargoIncumplimiento"]),
                    CargoIncumplimiento = Convert.ToString(reader["CargoIncumplimiento"]),
                    IdOrigenRaps = Convert.ToInt32(reader["IdOrigenRaps"]),
                    OrigenRaps = Convert.ToString(reader["OrigenRaps"]),
                    Estado = Convert.ToBoolean(reader["Estado"]),
                    IdGrupoUsuario = Convert.ToInt32(reader["IdGrupoUsuario"]),
                    GrupoUsuario = Convert.ToString(reader["GrupoUsuario"]),
                    IdClasificacion = Convert.ToInt32(reader["IdClasificacion"]),
                    Clasificacion = Convert.ToString(reader["Clasificacion"]),
                    IdSubclasificacion = Convert.ToInt32(reader["IdSubclasificacion"]),
                    Subclasificacion = Convert.ToString(reader["Subclasificacion"]),
                    TotalPaginas = Convert.ToInt32(reader["TotalPaginas"]),
                };

                resultado.Add(r);
            }
            return resultado;
        }


        /// <summary>
        /// Listar Cargo Escalonamiento Parametrizacion de una Raps
        /// </summary>
        /// <param name="resultReader"></param>
        /// <returns></returns>
        internal static List<RAListarCargoEscalonamientoRapsDC> MapperListarCargoEscalonamientoRaps(SqlDataReader reader)
        {
            List<RAListarCargoEscalonamientoRapsDC> resultado = null;
            while (reader.Read())
            {
                if (resultado == null)
                {
                    resultado = new List<RAListarCargoEscalonamientoRapsDC>();
                }
                var r = new RAListarCargoEscalonamientoRapsDC
                {
                    IdCargo = Convert.ToString(reader["IdCargo"]),
                    Descripcion = Convert.ToString(reader["Descripcion"]),
                    //CodigoSucursal = Convert.ToString(reader["CodigoSucursal"]),
                    //EnteControl = Convert.ToBoolean(reader["EnteControl"]),
                    //Regional = Convert.ToBoolean(reader["Regional"]),

                    //IdProcedimiento = Convert.ToString(reader["IdProcedimiento"]),
                    //NombreProcedimiento = Convert.ToString(reader["NombreProcedimiento"]),
                    //Idproceso = Convert.ToString(reader["IdProceso"]),
                    //NombreProceso = Convert.ToString(reader["NombreProceso"])

                    //CargoNovasoft = reader["CargoNovasoft"].ToString()
                };

                resultado.Add(r);
            }

            return resultado;
        }

        /// <summary>
        /// Listar Cargo Escalonamiento Parametrizacion de una Raps
        /// </summary>
        /// <param name="resultReader"></param>
        /// <returns></returns>
        internal static List<RACargoPersonaNovaRapDC> MapperListarCargoPersonaNova_Rap(SqlDataReader reader, bool porPersona)
        {
            //Modificado
            List<RACargoPersonaNovaRapDC> resultado = null;
            while (reader.Read())
            {
                if (resultado == null)
                {
                    resultado = new List<RACargoPersonaNovaRapDC>();
                }
                var r = new RACargoPersonaNovaRapDC
                {
                    IdCargo = porPersona ? reader["CodigoPlanta"].ToString() : Convert.ToString(reader["CodigoCargo"]),
                    CodigoCargo = Convert.ToString(reader["CodigoCargo"]),
                    NombreCargo = Convert.ToString(reader["NombreCargo"]),
                    Identificacion = porPersona ? reader["Identificacion"].ToString() : "0",
                    NombrePersona = porPersona ? reader["NombrePersona"].ToString() : "0",
                    IdTerritorial = Convert.ToInt16(reader["IdTerritorial"]),
                    IdRegional = Convert.ToString(reader["IdRegional"]),
                    IdProceso = Convert.ToString(reader["IdProceso"]),
                    NombreProceso = reader["nom_suc"].ToString() + " - " + reader["nom_cl5"].ToString(),
                    IdProcedimiento = Convert.ToString(reader["IdProcedimiento"]),
                    NombreProcedimiento = reader["nom_cl1"].ToString(),
                    DescripcionRol = reader["rol"] == DBNull.Value ? "" : reader["rol"].ToString(),
                    IdRol = reader["idRol"] == DBNull.Value ? 0 : Convert.ToInt32(reader["idRol"])
                };

                resultado.Add(r);
            }

            return resultado;
        }



        /// <summary>
        /// ListarCargoEscalonamientoParametrizacionRaps
        /// </summary>
        /// <param name="resultReader"></param>
        /// <returns></returns>
        internal static List<RAEscalonamientoDC> MapperListarCargoEscalonamientoParametrizacionRaps(SqlDataReader reader)
        {
            List<RAEscalonamientoDC> resultado = null;
            while (reader.Read())
            {
                if (resultado == null)
                {
                    resultado = new List<RAEscalonamientoDC>();
                }
                var r = new RAEscalonamientoDC
                {
                    // IdEscalonamiento = Convert.ToInt32(reader["IdEscalonamiento"]),
                    IdParametrizacionRap = Convert.ToInt32(reader["IdParametrizacionRap"]),
                    idCargo = reader["idCargo"].ToString(),
                    Descripcion = Convert.ToString(reader["Descripcion"]),
                    Orden = Convert.ToInt32(reader["orden"]),
                    IdTipoHora = Convert.ToInt32(reader["IdTipoHora"]),
                    HorasEscalar = Convert.ToInt32(reader["HorasEscalar"]),
                    IdProceso = reader["IdProceso"].ToString(),
                    NombreProceso = reader["DescripcionProceso"].ToString(),
                    IdProcedimiento = reader["IdProcedimiento"].ToString(),
                    NombreProcedimiento = reader["DescripcionProcedimiento"].ToString()
                };

                resultado.Add(r);
            }

            return resultado;
        }


        /// <summary>
        /// Listar Hora Escalar
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        internal static List<RAHoraEscalarDC> MapperListarHoraEscalar(SqlDataReader reader)
        {
            List<RAHoraEscalarDC> resultado = null;
            while (reader.Read())
            {
                if (resultado == null)
                {
                    resultado = new List<RAHoraEscalarDC>();
                }
                var r = new RAHoraEscalarDC
                {
                    IdHoraEscalar = Convert.ToInt32(reader["IdHoraEscalar"]),
                    HoraEscalar = Convert.ToInt32(reader["HoraEscalar"]),
                    Estado = Convert.ToBoolean(reader["Estado"])
                };

                resultado.Add(r);
            }

            return resultado;
        }



        internal static RAHoraEscalarDC MapperObtenerHoraEscalar(SqlDataReader reader)
        {
            RAHoraEscalarDC resultado = null;
            if (reader.Read())
            {
                var r = new RAHoraEscalarDC
                {
                    IdHoraEscalar = Convert.ToInt32(reader["IdHoraEscalar"]),
                    HoraEscalar = Convert.ToInt32(reader["HoraEscalar"]),
                    Estado = Convert.ToBoolean(reader["Estado"]),
                };
                resultado = r;
            }
            return resultado;
        }

        /// <summary>
        /// Mapeo Obtener territoriales
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        internal static List<RATerritorialDC> MapperObtenerTerritoriales(SqlDataReader reader)
        {
            List<RATerritorialDC> resultado = new List<RATerritorialDC>();
            while (reader.Read())
            {
                resultado.Add(new RATerritorialDC()
                {

                    IdTerritorial = Convert.ToInt16(reader["IdTerritorial"]),
                    NombreTerritorial = reader["NombreTerritorial"].ToString().Trim()
                });
            }
            return resultado;
        }

        /// <summary>
        /// Mapeo Obtener sucursales
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        internal static List<RARegionalSuculsalDC> MapperObtenerSucursales(SqlDataReader reader)
        {
            List<RARegionalSuculsalDC> resultado = new List<RARegionalSuculsalDC>();
            while (reader.Read())
            {
                resultado.Add(new RARegionalSuculsalDC()
                {
                    CodSucursal = reader["cod_suc"].ToString().Trim(),
                    NombreSucursal = reader["nom_suc"].ToString().Trim(),
                    EstadoSucursal = Convert.ToBoolean(reader["est_suc"]),
                    CodigoMunicipio = reader["cod_pai"].ToString().Trim()
                });
            }
            return resultado;
        }

        /// <summary>
        /// Metodo para obtener los empleados con su respectiva informacion de novasoft
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        internal static List<RAPersonaDC> MapperObtenerPersonalNovasoft(SqlDataReader reader)
        {
            List<RAPersonaDC> resultado = new List<RAPersonaDC>();
            while (reader.Read())
            {
                resultado.Add(new RAPersonaDC()
                {
                    IdCargo = reader["CodigoCargo"].ToString(),
                    IdCargoNovasoft = reader["CodPlantaNova"].ToString().Trim(),
                    Descripcion = reader["Descripcion"].ToString().Trim(),
                    Estado = Convert.ToBoolean(reader["Estado"]),
                    IdProcedimiento = Convert.ToInt32(reader["IdProcedimiento"]),
                    NombreCompleto = reader["NombreEmpleado"].ToString().Trim(),
                    NumeroDocumento = Convert.ToInt64(reader["Identificacion"]),
                    TipoIdentificacion = reader["TipoIdentificacion"].ToString(),
                    Email = reader["e_mail"].ToString().Trim()
                });
            }
            return resultado;
        }

        #region Fallas Interlogis / web 

        /// <summary>
        /// Metodo para obtener los tipos de novedad por responsable y aplicacion origen
        /// </summary>
        /// <param name="resultReader"></param>
        /// <returns></returns>
        internal static List<RANovedadDC> MapperTipoNovedad(SqlDataReader reader)
        {
            List<RANovedadDC> resultado = new List<RANovedadDC>();
            while (reader.Read())
            {
                resultado.Add(new RANovedadDC()
                {
                    idTipoNovedad = reader["TPN_IdTipoNovedad"] == DBNull.Value ? 0 : Convert.ToInt32(reader["TPN_IdTipoNovedad"]),
                    descripcionNovedad = reader["TPN_DescripcionTipoNovedad"] == DBNull.Value ? string.Empty : reader["TPN_DescripcionTipoNovedad"].ToString()
                });
            }
            return resultado;
        }
        #endregion
    }
}
