using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using CO.Servidor.Servicios.ContratoDatos.Recogidas;

namespace CO.Servidor.Recogidas.Datos
{
    internal class MapperRepositorio
    {
        internal static RGDetalleSolicitudRecogidaDC MapperToDetalleSolicitudRegcogida(SqlDataReader reader, RGEnumClaseSolicitud claseSolicitud)
        {
            char c = Convert.ToChar(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);

            RGDetalleSolicitudRecogidaDC resultado = null;
            if (reader.HasRows)
            {
                if (reader.Read())
                {
                    var r = new RGDetalleSolicitudRecogidaDC
                    {
                        TipoRecogida = Convert.ToInt64(reader["SRE_TipoSolRecogida"]),
                        IdSolRecogida = Convert.ToInt64(reader["SRE_IdSolRecogida"]),
                        FechaGrabacion = Convert.ToDateTime(reader["SRE_FechaGrabacion"]),
                        FechaHoraRecogida = Convert.ToDateTime(reader["SRE_FechaHoraRecogida"]),                        
                        IdOrigenSolRecogida = Convert.ToInt64(reader["SRE_IdOrigenSolRecogida"]),
                        NombreAplicacion = Convert.ToString(reader["IA_NombreAplicacion"]),
                        Documento = Convert.ToString(reader["Documento"]),
                        NombreCompleto = Convert.ToString(reader["NombreCompleto"]),
                        IdLocalidadRecogida = Convert.ToString(reader["SRE_IdLocalidadRecogida"]),
                        Nombre = Convert.ToString(reader["LOC_Nombre"]),
                        Direccion = Convert.ToString(reader["CLC_Direccion"]),
                        Telefono = Convert.ToString(reader["CLC_Telefono"]),
                        Entrega = Convert.ToString(reader["Entrega"]),
                        Peso = Convert.ToDecimal(reader["Peso"]),
                        Cantidad = Convert.ToDecimal(reader["Cantidad"]),
                        DescripcionEnvios = Convert.ToString(reader["DescripcionEnvios"]),
                        DescripcionMotivo = Convert.ToString(reader["DescripcionMotivo"]),
                        DocPersonaResponsable = Convert.ToString(reader["DocPersonaResponsable"]),
                        NombrePersonaResponsable = Convert.ToString(reader["NombrePersonaResponsable"]),
                        IdEstadoSolicitud = Convert.ToString(reader["IdEstadoSolicitud"]),
                    };

                    decimal number = 0;
                    Decimal.TryParse(Convert.ToString(reader["Latitud_ciudad"]), out number);
                    r.LatitudCiudad = number;

                    number = 0;
                    Decimal.TryParse(Convert.ToString(reader["longitud_ciudad"]), out number);
                    r.LongitudCiudad = number;

                    number = 0;
                    Decimal.TryParse(Convert.ToString(reader["Latitud"]).Replace('.', c), out number);
                    r.Latitud = number == 0 ? r.LatitudCiudad : number;


                    number = 0;
                    Decimal.TryParse(Convert.ToString(reader["longitud"]).Replace('.', c), out number);
                    r.Longitud = number == 0 ? r.LongitudCiudad : number;

                    if (!DBNull.Value.Equals(reader["FechaEjecucion"]))
                    {
                        r.FechaEjecucion = Convert.ToString(reader["FechaEjecucion"]);
                    }

                    if (claseSolicitud == RGEnumClaseSolicitud.FijaCliente)
                    {
                        if (Convert.ToBoolean(reader["EsAsignacionTemporal"]))
                        {
                            if (!(DBNull.Value.Equals(reader["FechaInicialAsignacionTemp"]) && DBNull.Value.Equals(reader["FechaInicialAsignacionTemp"])))
                            {
                                var fechaFin = Convert.ToDateTime(reader["FechaFinalAsignacionTemp"]);
                                var fechaIni = Convert.ToDateTime(reader["FechaInicialAsignacionTemp"]);
                                if (fechaIni >= DateTime.Now.Date || fechaIni <= DateTime.Now.Date)
                                {
                                    r.DocPersonaResponsable = Convert.ToString(reader["DocPersonaRespTemp"]);
                                    r.NombrePersonaResponsable = Convert.ToString(reader["NombrePersonaResponsableTemp"]);
                                }
                            }
                        }
                    }

                    resultado = r;
                }

            }

            return resultado;
        }

        /// <summary>
        /// Mapper para obtener las recogidas disponibles del mensajero 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static List<RGRecogidasDC> MapperRecogidaDisponible(SqlDataReader reader)
        {
            List<RGRecogidasDC> recogidas = new List<RGRecogidasDC>();
            if (reader.Read())
            {
                do
                {
                    recogidas.Add(new RGRecogidasDC()
                    {
                        Id = Convert.ToInt32(reader["SRE_IdSolRecogida"]),
                        Ciudad = reader["SRE_IdLocalidadRecogida"] == DBNull.Value ? string.Empty : reader["SRE_IdLocalidadRecogida"].ToString(),
                        Direccion = reader["SRP_DireccionRecogida"] == DBNull.Value ? string.Empty : reader["SRP_DireccionRecogida"].ToString(),
                        FechaRecogida = reader["SRE_FechaHoraRecogida"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["SRE_FechaHoraRecogida"]),
                        PreguntarPor = reader["SRP_PreguntarPor"] == DBNull.Value ? string.Empty : reader["SRP_PreguntarPor"].ToString(),
                        Observaciones = reader["SRP_DescripcionEnvios"] == DBNull.Value ? string.Empty : reader["SRP_DescripcionEnvios"].ToString(),
                        CodigoVerificacion = reader["SRP_NumeroDocumento"] == DBNull.Value ? string.Empty : reader["SRP_NumeroDocumento"].ToString(),
                        IdCentroServicio = reader["SRE_IdCentroServicios"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SRE_IdCentroServicios"]),
                        IdSucursal = reader["SRE_IdSucursal"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SRE_IdSucursal"]),
                        TipoRecogida = reader["SRE_TipoSolRecogida"] == DBNull.Value ? RGEnumTipoRecogidaDC.NoDefinida : (RGEnumTipoRecogidaDC)Convert.ToInt32(reader["SRE_TipoSolRecogida"]),
                        TotalPiezas = reader["SRP_TotalPiezas"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SRP_TotalPiezas"]),
                        PesoAproximado = reader["SRP_PesoAproximado"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SRP_PesoAproximado"])
                    });

                } while (reader.Read());
            }
            return recogidas;
        }

        internal static EnumEstadoSolicitudRecogida MapperToEnumEstadoSolicitudRecogida(SqlDataReader reader)
        {
            var resultado = EnumEstadoSolicitudRecogida.None;

            if (reader.HasRows)
            {
                if (reader.Read())
                {
                    resultado = (EnumEstadoSolicitudRecogida)
                        Enum.Parse(typeof(EnumEstadoSolicitudRecogida), Convert.ToString(reader["MER_EstadoCambia"]));
                }
            }
            return resultado;
        }

        internal static List<RGMotivoEstadoSolRecogidaDC> MapperToListaMotivoEstadoSolRecogida(SqlDataReader reader)
        {
            List<RGMotivoEstadoSolRecogidaDC> resultado = null;
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    var r = new RGMotivoEstadoSolRecogidaDC
                    {
                        IdMotivo = Convert.ToInt64(reader["MER_IdMotivo"]),
                        DescripcionMotivo = Convert.ToString(reader["MER_DescripcionMotivo"]),
                        EsFija = Convert.ToBoolean(reader["MER_EsFija"])
                    };

                    if (resultado == null)
                    {
                        resultado = new List<RGMotivoEstadoSolRecogidaDC>();
                    }

                    resultado.Add(r);
                }
            }
            return resultado;
        }

        /// <summary>
        /// Metodo para obtener las recogidas reservadas del mensajero 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static List<RGRecogidasDC> MapperRecogidasReserva(SqlDataReader reader)
        {
            List<RGRecogidasDC> recogidas = new List<RGRecogidasDC>();
            if (reader.Read())
            {
                do
                {
                    recogidas.Add(new RGRecogidasDC()
                    {
                        Id = Convert.ToInt64(reader["IdSolicitud"]),
                        Ciudad = reader["IdLocalidad"] == DBNull.Value ? string.Empty : reader["IdLocalidad"].ToString(),
                        Direccion = reader["DireccionRecogida"] == DBNull.Value ? string.Empty : reader["DireccionRecogida"].ToString(),
                        FechaRecogida = reader["FechaHoraRecogida"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["FechaHoraRecogida"]),
                        PreguntarPor = reader["PreguntarPor"] == DBNull.Value ? string.Empty : reader["PreguntarPor"].ToString(),
                        Observaciones = reader["DescripcionEnvios"] == DBNull.Value ? string.Empty : reader["DescripcionEnvios"].ToString(),
                        CodigoVerificacion = reader["NumeroDocumento"] == DBNull.Value ? string.Empty : reader["NumeroDocumento"].ToString(),
                        IdCentroServicio = reader["IdCentroServicio"] == DBNull.Value ? 0 : Convert.ToInt32(reader["IdCentroServicio"]),
                        IdSucursal = reader["IdSucursal"] == DBNull.Value ? 0 : Convert.ToInt32(reader["IdSucursal"]),
                        TipoRecogida = reader["TipoRecogida"] == DBNull.Value ? RGEnumTipoRecogidaDC.NoDefinida : (RGEnumTipoRecogidaDC)Convert.ToInt32(reader["TipoRecogida"]),
                        EstaForzada = reader["EstaForzada"] == DBNull.Value ? false : Convert.ToBoolean(reader["EstaForzada"]),
                        NombreSucursal = reader["NombreSucursal"] == DBNull.Value ? string.Empty : reader["NombreSucursal"].ToString(),
                        NombreCentroServicio = reader["NombreCentroServicio"] == DBNull.Value ? string.Empty : reader["NombreCentroServicio"].ToString(),
                        NumeroTelefono = reader["Telefono"] == DBNull.Value ? string.Empty : reader["Telefono"].ToString()
                    });
                } while (reader.Read());
            }
            return recogidas;
        }

        /// <summary>
        /// Metodo para obtener las recogidas efectivas del mensajero 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        internal static List<RGRecogidasDC> MapperRecogidasEfectivas(SqlDataReader reader)
        {
            List<RGRecogidasDC> recogidas = new List<RGRecogidasDC>();
            if (reader.Read())
            {
                do
                {
                    recogidas.Add(new RGRecogidasDC()
                    {
                        Id = Convert.ToInt64(reader["IdSolicitud"]),
                        Ciudad = reader["IdLocalidad"] == DBNull.Value ? string.Empty : reader["IdLocalidad"].ToString(),
                        Direccion = reader["DireccionRecogida"] == DBNull.Value ? string.Empty : reader["DireccionRecogida"].ToString(),
                        FechaRecogida = reader["FechaHoraRecogida"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["FechaHoraRecogida"]),
                        PreguntarPor = reader["PreguntarPor"] == DBNull.Value ? string.Empty : reader["PreguntarPor"].ToString(),
                        Observaciones = reader["DescripcionEnvios"] == DBNull.Value ? string.Empty : reader["DescripcionEnvios"].ToString(),
                        CodigoVerificacion = reader["NumeroDocumento"] == DBNull.Value ? string.Empty : reader["NumeroDocumento"].ToString(),
                        IdCentroServicio = reader["IdCentroServicio"] == DBNull.Value ? 0 : Convert.ToInt32(reader["IdCentroServicio"]),
                        IdSucursal = reader["IdSucursal"] == DBNull.Value ? 0 : Convert.ToInt32(reader["IdSucursal"]),
                        TipoRecogida = reader["TipoRecogida"] == DBNull.Value ? RGEnumTipoRecogidaDC.NoDefinida : (RGEnumTipoRecogidaDC)Convert.ToInt32(reader["TipoRecogida"])
                    });
                } while (reader.Read());
            }
            return recogidas;
        }

        internal static List<RGDispositivoMensajeroDC> MapperToRGDispositivoMensajeroDC(SqlDataReader reader)
        {
            List<RGDispositivoMensajeroDC> resultado = null;

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    var r = new RGDispositivoMensajeroDC
                    {
                        IdPosicionMensajero = Convert.ToInt64(reader["UPM_IdPosicionMensajero"]),
                        IdDispositivo = Convert.ToInt64(reader["UPM_IdDispositivo"]),
                        IdLocalidad = Convert.ToString(reader["UPM_IdLocalidad"]),
                        IdMensajero = Convert.ToInt64(reader["MEN_IdMensajero"]),
                        IdTipoMensajero = Convert.ToInt16(reader["TIM_IdTipoMensajero"]),
                        IdAgencia = Convert.ToInt64(reader["MEN_IdAgencia"]),
                        Telefono2 = Convert.ToString(reader["MEN_Telefono2"]),
                        NumeroPase = Convert.ToString(reader["MEN_NumeroPase"]),
                        FechaVencimientoPase = Convert.ToDateTime(reader["MEN_FechaVencimientoPase"]),
                        EsContratista = Convert.ToBoolean(reader["MEN_EsContratista"]),
                        TipoContrato = Convert.ToInt16(reader["MEN_TipoContrato"]),
                        IdPersonaInterna = Convert.ToInt64(reader["MEN_IdPersonaInterna"]),
                        EsMensajeroUrbano = Convert.ToBoolean(reader["MEN_EsMensajeroUrbano"]),
                        FechaGrabacion = Convert.ToDateTime(reader["PME_FechaGrabacion"]),

                        Nombre = Convert.ToString(reader["PEI_Nombre"]),
                        PrimerApellido = Convert.ToString(reader["PEI_PrimerApellido"]),
                        SegundoApellido = DBNull.Value.Equals(reader["PEI_SegundoApellido"]) ? String.Empty : Convert.ToString(reader["PEI_SegundoApellido"]),
                        Telefono = Convert.ToString(reader["PEI_Telefono"]),
                        Descripcion = Convert.ToString(reader["TIM_Descripcion"]),
                        IdVehiculo = DBNull.Value.Equals(reader["MEV_IdVehiculo"]) ? 0 : Convert.ToInt64(reader["MEV_IdVehiculo"]),
                        placa = DBNull.Value.Equals(reader["VEH_placa"]) ? String.Empty : Convert.ToString(reader["VEH_placa"]),
                        IdTipoContrato = DBNull.Value.Equals(reader["VEH_IdTipoContrato"]) ? 0 : Convert.ToInt32(reader["VEH_IdTipoContrato"]),
                        NombreTipoContrato = DBNull.Value.Equals(reader["TIV_Descripcion"]) ? String.Empty : Convert.ToString(reader["TIV_Descripcion"]),
                        Propiedad = Convert.ToString(reader["Propiedad"]),
                    };

                    decimal number = 0;
                    decimal.TryParse(Convert.ToString(reader["UPM_Longitud"]), out number);
                    r.Longitud = number;
                    number = 0;
                    decimal.TryParse(Convert.ToString(reader["UPM_Latitud"]), out number);
                    r.Latitud = number;

                    r.NombreCompleto = r.Nombre + " " + r.PrimerApellido + " " + r.SegundoApellido;

                    if (resultado == null)
                    {
                        resultado = new List<RGDispositivoMensajeroDC>();
                    }

                    resultado.Add(r);
                }
            }

            return resultado;
        }
    }
}