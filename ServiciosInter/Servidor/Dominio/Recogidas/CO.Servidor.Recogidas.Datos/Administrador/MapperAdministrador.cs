using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using CO.Servidor.Servicios.ContratoDatos.Comun;
using CO.Servidor.Servicios.ContratoDatos.Recogidas;

namespace CO.Servidor.Recogidas.Datos.Administrador
{
    internal class MapperAdministrador
    {
        internal static List<RGDetalleConteoAdminRecogidasDC> ToDetalleConteoAdminRecogida(SqlDataReader reader)
        {
            List<RGDetalleConteoAdminRecogidasDC> resultado = null;
            if(reader.HasRows)
            {
                while(reader.Read())
                {
                    var r = new RGDetalleConteoAdminRecogidasDC
                    {
                        FiltroConteo=Convert.ToString(reader["FiltroConteo"]),
                        IdSolRecogida=Convert.ToInt64(reader["IdSolRecogida"]),
                        FechaGrabacion=Convert.ToDateTime(reader["FechaGrabacion"]),
                        FechaHoraRecogida=Convert.ToDateTime(reader["FechaHoraRecogida"]),
                        IdLocalidadRecogida=Convert.ToString(reader["IdLocalidadRecogida"]),
                        IdOrigenSolRecogida=Convert.ToString(reader["IdOrigenSolRecogida"]),
                        IdAgencia=Convert.ToInt32(reader["IdAgencia"]),
                        IdEstadoSolicitud = (COEnumIdentificadorAplicacion)Enum.Parse(typeof(COEnumIdentificadorAplicacion), Convert.ToString(reader["IdEstadoSolicitud"])),
                        PesoAproximado=Convert.ToInt64(reader["PesoAproximado"]),
                        Nombre=Convert.ToString(reader["Nombre"]),
                        DireccionRecogida=Convert.ToString(reader["DireccionRecogida"]),
                        NumeroDocumento=Convert.ToString(reader["NumeroDocumento"]),
                        EsEsporadicaCliente =Convert.ToBoolean(reader["EsEsporadicaCliente"]),
                        TipoSolRecogida = (RGEnumTipoRecogidaDC)Enum.Parse(typeof(RGEnumTipoRecogidaDC), Convert.ToString(reader["TipoSolRecogida"])),
                        DescripcionMotivo = Convert.ToString(reader["DescripcionMotivo"]),
                    };

                    decimal number = 0;
                    decimal.TryParse(Convert.ToString(reader["Longitud"]), out number);
                    r.Longitud = number;
                    number = 0;
                    decimal.TryParse(Convert.ToString(reader["Latitud"]), out number);
                    r.Latitud = number;

                    if(!DBNull.Value.Equals(reader["IdClienteContado"]))
                    {
                        r.IdClienteContado  =  Convert.ToInt64(reader["IdClienteContado"]);
                    }

                    if(!DBNull.Value.Equals(reader["IdSucursal"]))
                    {
                        r.IdSucursal  = Convert.ToInt64(reader["IdSucursal"]);
                    }

                    if(!DBNull.Value.Equals(reader["IdCentroServicios"]))
                    {
                        r.IdCentroServicios = Convert.ToInt64(reader["IdCentroServicios"]);
                    }

                    if(!DBNull.Value.Equals(reader["LongitudRecogida"]))
                    {
                        r.LongitudRecogida = Convert.ToString(reader["LongitudRecogida"]);
                    }

                    if(!DBNull.Value.Equals(reader["LatitudRecogida"]))
                    {
                        r.LatitudRecogida = Convert.ToString(reader["LatitudRecogida"]);
                    }

                    r.IdentificacionCliente = DBNull.Value.Equals(reader["IdentificacionCliente"]) ? string.Empty : Convert.ToString(reader["IdentificacionCliente"]);


                    r.ClaseSolicitud = IdentificaClasesolicitud(r);

                    if(resultado==null)
                    {
                        resultado=new List<RGDetalleConteoAdminRecogidasDC>();
                    }

                    resultado.Add(r);

                }
            }

            return resultado;

        }

        internal static List<RGDispositivoMensajeroDC> ToListDispositivoMensajero(SqlDataReader reader)
        {
            List<RGDispositivoMensajeroDC> resultado = null;

            if(reader.HasRows)
            {
                while(reader.Read())
                {
                    var r = new RGDispositivoMensajeroDC
                    {
                        IdPosicionMensajero = Convert.ToInt64(reader["PME_IdPosicionMensajero"]),                       
                        IdDispositivo = Convert.ToInt64(reader["PME_IdDispositivo"]),                       
                        IdLocalidad = Convert.ToString(reader["PME_IdLocalidad"]),
                        IdMensajero = Convert.ToInt64(reader["MEN_IdMensajero"]),
                        IdTipoMensajero = Convert.ToInt16(reader["MEN_IdTipoMensajero"]),
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
                        IdTipoContrato = DBNull.Value.Equals(reader["VEH_placa"]) ? 0 : Convert.ToInt32(reader["VEH_IdTipoContrato"]),
                        NombreTipoContrato = DBNull.Value.Equals(reader["VEH_placa"]) ? String.Empty : Convert.ToString(reader["TIV_Descripcion"]),
                        Propiedad = Convert.ToString(reader["Propiedad"]),
                    };

                    decimal number = 0;
                    decimal.TryParse(Convert.ToString(reader["PME_Longitud"]), out number);
                    r.Longitud = number;
                    number = 0;
                    decimal.TryParse(Convert.ToString(reader["PME_Latitud"]), out number);
                    r.Latitud = number;

                    r.NombreCompleto = r.Nombre + " " + r.PrimerApellido + " " + r.SegundoApellido;

                    if(resultado == null)
                    {
                        resultado = new List<RGDispositivoMensajeroDC>();
                    }

                    resultado.Add(r);
                }
            }

            return resultado;

        }

        internal static List<RGMensajeroLocalidadDC> ToListMensajeroLocalidad(SqlDataReader reader)
        {

            List<RGMensajeroLocalidadDC> resultado = null;

            if(reader.HasRows)
            {
                while(reader.Read())
                {
                    var r = new RGMensajeroLocalidadDC
                    {
                        IdMensajero = Convert.ToInt64(reader["MEN_IdMensajero"]),
                        Nombre = Convert.ToString(reader["PEI_Nombre"]),
                        PrimerApellido = Convert.ToString(reader["PEI_PrimerApellido"]),
                        SegundoApellido = DBNull.Value.Equals(reader["PEI_SegundoApellido"]) ? String.Empty : Convert.ToString(reader["PEI_SegundoApellido"]),
                        Telefono = Convert.ToString(reader["PEI_Telefono"]),
                        IdTipoMensajero = Convert.ToInt32(reader["TIM_IdTipoMensajero"]),
                        Descripcion = Convert.ToString(reader["TIM_Descripcion"]),
                        IdVehiculo = DBNull.Value.Equals(reader["MEV_IdVehiculo"]) ? 0 : Convert.ToInt64(reader["MEV_IdVehiculo"]),
                        placa = DBNull.Value.Equals(reader["VEH_placa"]) ? String.Empty : Convert.ToString(reader["VEH_placa"]),
                        IdTipoContrato = DBNull.Value.Equals(reader["VEH_placa"]) ? 0 : Convert.ToInt32(reader["VEH_IdTipoContrato"]),
                        NombreTipoContrato = DBNull.Value.Equals(reader["VEH_placa"]) ? String.Empty : Convert.ToString(reader["TIV_Descripcion"]),
                        Propiedad = Convert.ToString(reader["Propiedad"]),
                        Identificacion = Convert.ToString(reader["PEI_Identificacion"]),
                        
                    };

                    r.NombreCompleto = r.Nombre + " " + r.PrimerApellido + " " + r.SegundoApellido;
                    if(resultado == null)
                    {
                        resultado = new List<RGMensajeroLocalidadDC>();
                    }

                    resultado.Add(r);
                }
            }

            return resultado;
        }

        private static RGEnumClaseSolicitud IdentificaClasesolicitud(RGDetalleConteoAdminRecogidasDC r)
        {            
            if(r.TipoSolRecogida == RGEnumTipoRecogidaDC.FijaCliente && r.EsEsporadicaCliente)
            {
                return RGEnumClaseSolicitud.ExporadicaClienteFijo;
            }

            if(r.TipoSolRecogida == RGEnumTipoRecogidaDC.Esporadica )
            {
                return RGEnumClaseSolicitud.Exporadica;
            }

            if(r.TipoSolRecogida == RGEnumTipoRecogidaDC.FijaCliente && !r.EsEsporadicaCliente)
            {
                return RGEnumClaseSolicitud.FijaCliente;
            }

            if (r.TipoSolRecogida == RGEnumTipoRecogidaDC.FijaCentroServicio)
            {
                return RGEnumClaseSolicitud.FijaCentroServicio;
            }             

            return RGEnumClaseSolicitud.SinSolicitud;
        }
    }
}