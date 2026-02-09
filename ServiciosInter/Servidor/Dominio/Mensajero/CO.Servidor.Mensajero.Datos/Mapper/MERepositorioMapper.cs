using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.Mensajero;
using System.Data.SqlClient;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion;

namespace CO.Servidor.Mensajero.Datos.Mapper
{
    public class MERepositorioMapper
    {
        public static List<MECargo> ToListCargo(SqlDataReader reader)
        {
            List<MECargo> cargos = null;
            while (reader.Read())
            {
                if (cargos == null)
                {
                    cargos = new List<MECargo>();
                }
                cargos.Add(new MECargo {
                    IdCargo =reader["CAR_IdCargo"]==DBNull.Value ? string.Empty : reader["CAR_IdCargo"].ToString(),
                    Descripcion = reader["CAR_Descripcion"] == DBNull.Value ? string.Empty : reader["CAR_Descripcion"].ToString(),
                    IdCargoReporta = reader["CAR_IdCargoReporta"] == DBNull.Value ? string.Empty : reader["CAR_IdCargoReporta"].ToString()
                });
            }
            return cargos;
        }

        public static List<METipoContrato> ToListTipoContrato(SqlDataReader reader)
        {
            List<METipoContrato> tipoContrato = new List<METipoContrato>();
            while (reader.Read()) {
                tipoContrato.Add(new METipoContrato
                {
                    IdTipoContrato = Convert.ToInt16(reader["TIC_IdTipoContrato"]),
                    Descripcion = reader["TIC_Descripcion"] == DBNull.Value ? string.Empty : reader["TIC_Descripcion"].ToString()
                });
            }
            return tipoContrato;
        }

        public static List<METipoMensajero> ToListTipoMensajero(SqlDataReader reader)
        {
            List<METipoMensajero> tipoMensajero = new List<METipoMensajero>();
            while (reader.Read())
            {
                tipoMensajero.Add(new METipoMensajero
                {
                    IdTipoMensajero = Convert.ToInt16(reader["TIM_IdTipoMensajero"]),
                    Descripcion = reader["TIM_Descripcion"] == DBNull.Value ? string.Empty : reader["TIM_Descripcion"].ToString()
                });
            }
            return tipoMensajero;
        }

        public static List<MEEstadoMensajero> ToListEstadoMensajero(SqlDataReader reader)
        {
            List<MEEstadoMensajero> estadosMensajero = new List<MEEstadoMensajero>();
            while (reader.Read())
            {
                estadosMensajero.Add(new MEEstadoMensajero
                {
                    IdEstado = Convert.ToInt16(reader["MEN_IdEstado"]),
                    Estado = reader["MEN_Estado"] == DBNull.Value ? string.Empty : reader["MEN_Estado"].ToString()
                });
            }
            return estadosMensajero;
        }

        internal static List<MEGruposLiquidacion> ToListGruposLiquidacion(SqlDataReader reader)
        {
            List<MEGruposLiquidacion> gruposLiquidacion = new List<MEGruposLiquidacion>();
            while (reader.Read())
            {
                gruposLiquidacion.Add(new MEGruposLiquidacion
                {
                    IdGrupoLiq = Convert.ToInt64(reader["GLQ_IdGrupoLiq"]),
                    GrupoLiq = reader["GLQ_GrupoLiq"].ToString(),
                    FechaInicio= Convert.ToDateTime(reader["GLQ_FechaInicio"]),
                    FechaFinal = Convert.ToDateTime(reader["GLQ_FechaFinal"]),
                    Estado = Convert.ToBoolean(reader["GLQ_Estado"]),
                    TotalPaginas = Convert.ToInt32(reader["TotalPaginas"])
                });
            }
            return gruposLiquidacion;
        }

        internal static List<MEGruposLiquidacion> ToListGruposLiquidacionUnico(SqlDataReader reader)
        {
            List<MEGruposLiquidacion> gruposLiquidacion = new List<MEGruposLiquidacion>();
            while (reader.Read())
            {
                gruposLiquidacion.Add(new MEGruposLiquidacion
                {
                    IdGrupoLiq = Convert.ToInt64(reader["GLQ_IdGrupoLiq"]),
                    GrupoLiq = reader["GLQ_GrupoLiq"].ToString(),
                    FechaInicio = Convert.ToDateTime(reader["GLQ_FechaInicio"]),
                    FechaFinal=Convert.ToDateTime(reader["GLQ_FechaFinal"])
                });
            }
            return gruposLiquidacion;
        }

        internal static List<MEGrupoBasico> ToListGruposBasicos(SqlDataReader reader)
        {
            List<MEGrupoBasico> gruposBasicos = new List<MEGrupoBasico>();
            while (reader.Read()) {
                gruposBasicos.Add(new MEGrupoBasico
                {
                    IdGrupoBasico = Convert.ToInt64(reader["GBP_IdGrupoBasico"]),
                    GrupoBasico = reader["GBP_GrupoBasico"].ToString(),
                    ValorInicial = Convert.ToDouble(reader["GBP_ValorInicial"]),
                    ValorFinal = Convert.ToDouble(reader["GBP_ValorFinal"]),
                    NumeroCuotas = Convert.ToInt32(reader["GBP_NumeroCuotas"]),
                    FechaInicial=Convert.ToDateTime(reader["GBP_FechaInicial"]),
                    FechaFinal = Convert.ToDateTime(reader["GBP_FechaFinal"]),
                    Estado=Convert.ToBoolean(reader["GBP_Estado"]),
                    TotalPaginas = Convert.ToInt32(reader["TotalPaginas"])
                });
            }
            return gruposBasicos;
        }

        internal static List<MEGrupoRodamiento> ToListGruposRodamiento(SqlDataReader reader)
        {
            List<MEGrupoRodamiento> gruposRodamiento = new List<MEGrupoRodamiento>();
            while (reader.Read()) {
                gruposRodamiento.Add(new MEGrupoRodamiento
                {
                    Id = Convert.ToInt32(reader["RME_IdRodamiento"]),
                    RodamientoMensajero = reader["RME_RodamientoMensajero"].ToString(),
                    IdCiudad = reader["RME_IdTipoCiudad"].ToString(),
                    IdZona = Convert.ToInt32(reader["RME_IdZona"]),
                    //IdTipoTransporte = Convert.ToInt32(reader["RME_IdTipoTransporte"]),
                    NombreTipoCiudad = reader["CGC_TipoCiudad"].ToString(),
                    NombreZona = reader["ZON_Descripcion"].ToString(),
                    FechaInicial = Convert.ToDateTime(reader["RME_FechaInical"]),
                    FechaFinal = Convert.ToDateTime(reader["RME_FechaFinal"]),
                    Estado = Convert.ToBoolean(reader["RME_Estado"]),
                    IdTipoCiudad=Convert.ToInt32(reader["LTC_IdTipoCiudad"]),
                    //TipoTransporte = reader["TTP_TipoTransporte"].ToString(),
                    //Valor = Convert.ToDouble(reader["TTP_Valor"]),
                    //MinimoVital=Convert.ToDouble(reader["TTP_MinimoVital"])
                    TotalPaginas = Convert.ToInt32(reader["TotalPaginas"]),
                });
            }
            return gruposRodamiento;
        }

        internal static List<MEBasicoLiquidacion> ToListBasicoLiquidacion(SqlDataReader reader)
        {
            List<MEBasicoLiquidacion> basicoLiquidacion = new List<MEBasicoLiquidacion>();
            while (reader.Read())
            {
                basicoLiquidacion.Add(new MEBasicoLiquidacion
                {
                    IdGrupoBasico = Convert.ToInt64(reader["BLQ_IdGrupoBasico"]),
                    Mes = Convert.ToInt32(reader["BLQ_Mes"]),
                    Valor = Convert.ToDouble(reader["BLQ_Valor"])
                });
            }
            return basicoLiquidacion;
        }

        internal static List<METipoTransporte> ToListTipoTransporte(SqlDataReader reader) {
            List<METipoTransporte> tipotransporte = new List<METipoTransporte>();

            while (reader.Read())
            {
                tipotransporte.Add(new METipoTransporte
                {
                    IdTipoTransporte = reader["TTP_IdTipoTransporte"].ToString(),
                    TipoTranpsorte = reader["TTP_TipoTransporte"].ToString(),
                    //MinimoVital=Convert.ToDouble(reader["TTP_MinimoVital"]),
                    //FechaCreado=Convert.ToDateTime(reader["TTP_FechaCreado"]),
                    //CreadoPor=reader["TTP_CreadoPor"].ToString()
                });
            }
            return tipotransporte;
        }

        internal static List<METipoAccion> ToListTipoAccion(SqlDataReader reader) {
            List<METipoAccion> tipoAccion = new List<METipoAccion>();

            while (reader.Read()) {
                tipoAccion.Add(new METipoAccion
                {
                    IdTipoAccion = Convert.ToInt32(reader["TAC_IdTipoAccion"]),
                    TipoAccion = reader["TAC_TipoAccion"].ToString()
                });
            }
            return tipoAccion;
        }

        internal static List<METipoPenalidad> ToListTipoPenalidad(SqlDataReader reader) {
            List<METipoPenalidad> tipoPenalidad = new List<METipoPenalidad>();

            while (reader.Read()) {
                tipoPenalidad.Add(new METipoPenalidad
                {
                    IdPenalidad = Convert.ToInt32(reader["TPM_IdPenalidad"]),
                    Penalidad = reader["TPM_Penalidad"].ToString(),
                    IdTipoUsuario = Convert.ToInt32(reader["TPU_IdTipoUsuario"]),
                    TipoUsuario = reader["TPU_TipoUsuario"].ToString(),
                    ValorPorcentual = Convert.ToDouble(reader["TPM_ValorPorcentual"]),
                    Porcentaje=Convert.ToDouble(reader["TPM_Porcentaje"]),
                    FechaInicio=Convert.ToDateTime(reader["TPM_FechaInicio"]),
                    FechaFin=Convert.ToDateTime(reader["TPM_FechaFin"]),
                    Estado=Convert.ToBoolean(reader["TPM_Estado"]),
                    TipoCuenta = new METipoCuenta {
                        IdTipoCuenta=reader["tpm_idTipocuenta"].ToString(),
                        TipoCuenta=reader["TCU_TipoCuenta"].ToString()
                    },
                    IdPenalidadRaps=reader["TPM_IdPenalidadRaps"].ToString(),
                    IdParametroRaps=Convert.ToInt32(reader["TPM_IdParametroRaps"]),
                    TotalPaginas = Convert.ToInt32(reader["TotalPaginas"])
                });
            }
            return tipoPenalidad;
        }

        internal static List<METipoPenalidad> ToListTipoPenalidadesConfig(SqlDataReader reader)
        {
            List<METipoPenalidad> tipoPenalidad = new List<METipoPenalidad>();

            while (reader.Read())
            {
                tipoPenalidad.Add(new METipoPenalidad
                {
                    IdPenalidadRaps = reader["PRA_IdPenalidadRaps"].ToString(),
                    IdParametroRaps = Convert.ToInt32(reader["PRA_IdParametroRaps"]),
                  //  Penalidad =  reader["Nombre"]==DBNull.Value ? string.Empty : reader["Nombre"].ToString()
                });
            }
            return tipoPenalidad;
        }
        internal static List<METipoPenalidad> ToListTipoPenalidadesRaps(SqlDataReader reader)
        {
            List<METipoPenalidad> tipoPenalidad = new List<METipoPenalidad>();

            while (reader.Read())
            {
                tipoPenalidad.Add(new METipoPenalidad
                {
                    IdParametroRaps = Convert.ToInt32(reader["IdParametrizacionRap"]),
                    Penalidad =  reader["Nombre"]==DBNull.Value ? string.Empty : reader["Nombre"].ToString()
                });
            }
            return tipoPenalidad;
        }

        internal static List<METipoUsuario> ToListTipoUsuario(SqlDataReader reader) {
            List<METipoUsuario> tipoUsuario = new List<METipoUsuario>();

            while (reader.Read()) {
                tipoUsuario.Add(new METipoUsuario
                {
                    IdTipoUsuario = Convert.ToInt32(reader["TPU_IdTipoUsuario"]), 
                    TipoUsuario=reader["TPU_TipoUsuario"].ToString()
                });
            }
            return tipoUsuario;
        }

        internal static List<METipoRodamiento> ToListTipoRodamiento(SqlDataReader reader) {
            List<METipoRodamiento> tipoRodamiento = new List<METipoRodamiento>();

            while (reader.Read()) {
                tipoRodamiento.Add(new METipoRodamiento
                {
                    IdGrupoRodamiento=Convert.ToInt32(reader["RMT_IdRodamiento"]),
                    IdTipoTransporte=reader["RMT_IdTipoTransporte"].ToString(),
                    Valor=Convert.ToDouble(reader["RMT_Valor"]),
                    MinimoVital=Convert.ToDouble(reader["TTP_MinimoVital"]),
                    TipoTransporte=reader["TTP_TipoTransporte"].ToString(),
                    EstadoTpoRod=Convert.ToBoolean(reader["RMT_Estado"])
                });
            }
            return tipoRodamiento;
        }

        internal static List<METiposLiquidacion> ToListTipoLiquidacion(SqlDataReader reader) {
            List<METiposLiquidacion> tiposLiquidacion = new List<METiposLiquidacion>();

            while (reader.Read())
            {
                tiposLiquidacion.Add(new METiposLiquidacion
                {
                    IdTipoLiq = Convert.ToInt32(reader["TLQ_IdTipoLiq"]),
                    IdGrupoLiq=Convert.ToInt32(reader["TLQ_IdGrupoLiq"]),
                    IdUnidad=reader["TLQ_IdUnidad"].ToString(),
                    UnidadNegocio=reader["UnidadNegocio"].ToString(),
                    IdTipoAccion=Convert.ToInt32(reader["TAC_IdTipoAccion"]),
                    TipoAccion=reader["TipoAccion"].ToString(),
                    FormaPago=reader["FormaPago"].ToString(),
                    ValorPorcentual=Convert.ToDouble(reader["VFP_ValorPorcentual"]),
                    TotalPaginas = Convert.ToInt32(reader["TotalPaginas"]),
                    IdFormaPago=Convert.ToInt32(reader["FOP_IdFormaPago"]),
          
                });
            }
            return tiposLiquidacion;
        }

        internal static List<MEUnidadNegocioFormaPago> ToListUnidadNegocioFormaPago(SqlDataReader reader)
        {
            List<MEUnidadNegocioFormaPago> unidadNegocioFormaPago = new List<MEUnidadNegocioFormaPago>();

            while (reader.Read())
            {
                unidadNegocioFormaPago.Add(new MEUnidadNegocioFormaPago
                {
                    IdUnidadForma=Convert.ToInt32(reader["UNF_Id"].ToString()),
                    IdTipoAccion = Convert.ToInt32(reader["TAC_IdTipoAccion"]),
                    TipoAccion = reader["TAC_TipoAccion"].ToString(),
                    IdUnidad = reader["UNF_IdUnidad"].ToString(),
                    UnidadNegocio = reader["UNE_Nombre"].ToString(),
                    Descripcion = reader["UNE_Descripcion"].ToString(),
                    IdFormaPago = Convert.ToInt16(reader["UNF_IdFormaPago"]),
                    FormaPago = reader["FOP_Descripcion"].ToString(),
                    TotalPaginas=Convert.ToInt32(reader["TotalPaginas"]),
                    TipoLiquidacion = new METiposLiquidacion {
                        IdTipoLiq = 0,
                        IdGrupoLiq = 0,
                        IdFormaPago=Convert.ToInt32(reader["AFP_IdFormaPago"]),
                        IdTipoAccion = Convert.ToInt32(reader["AFP_IdTipoAccion"]),
                        ValorPorcentual = 0
                    },
                });
            }
            return unidadNegocioFormaPago;
        }
        internal static List<MEUnidadNegocioFormaPago> ToListUnidadNegocio(SqlDataReader reader)
        {
            List<MEUnidadNegocioFormaPago> unidadNegocio = new List<MEUnidadNegocioFormaPago>();

            while (reader.Read())
            {
                unidadNegocio.Add(new MEUnidadNegocioFormaPago
                {
                    IdUnidad = reader["UNE_IdUnidad"].ToString(),
                    UnidadNegocio = reader["UNE_Nombre"].ToString(),
                    Descripcion = reader["UNE_Descripcion"].ToString(),
                });
            }
            return unidadNegocio;
        }

        internal static List<MEUnidadNegocioFormaPago> ToListFormaPago(SqlDataReader reader)
        {
            List<MEUnidadNegocioFormaPago> formaPago = new List<MEUnidadNegocioFormaPago>();

            while (reader.Read())
            {
                formaPago.Add(new MEUnidadNegocioFormaPago
                {
                    IdFormaPago = Convert.ToInt16(reader["FOP_IdFormaPago"]),
                    FormaPago = reader["FOP_Descripcion"].ToString(),
                    IdTipoAccion = Convert.ToInt32(reader["AFP_IdTipoAccion"]),
                    TipoLiquidacion = new METiposLiquidacion
                    {
                        IdTipoAccion = Convert.ToInt32(reader["AFP_IdTipoAccion"]),
                    },
                });
            }
            return formaPago;
        }
        internal static List<METipoCuenta> ToListTipoCuenta(SqlDataReader reader)
        {
            List<METipoCuenta> tipoCuenta = new List<METipoCuenta>();

            while (reader.Read())
            {
                tipoCuenta.Add(new METipoCuenta
                {
                    IdTipoCuenta = reader["TCU_IdTipocuenta"].ToString(),
                    TipoCuenta = reader["TCU_TipoCuenta"].ToString(),
                });
            }
            return tipoCuenta;
        }
        internal static List<MEMensajero> ToListMensajero(SqlDataReader reader)
        {
            List<MEMensajero> mensajeros = null;
            while (reader.Read())
            {
                if (mensajeros == null)
                {
                    mensajeros = new List<MEMensajero>();
                }
                mensajeros.Add(new MEMensajero
                {
                        IdDocumento = reader["NumeroIdentificacion"] == DBNull.Value ? string.Empty : reader["NumeroIdentificacion"].ToString(),
                        Nombre=reader["ApellidosNombre"]==DBNull.Value ?string.Empty:reader["ApellidosNombre"].ToString()
                });
            }
            return mensajeros;
        }

      

        internal static MEMensajero ToListDetalleEmpleadoNovasoft(SqlDataReader reader)
        {
            MEMensajero mensajeros = null;
            if (reader.Read())
            {
                mensajeros = new MEMensajero
                {
                    //TipoDocumento=reader[""]
                    TipoDocumento = reader["TipoIdentificacion"] == DBNull.Value ? string.Empty : reader["TipoIdentificacion"].ToString(),
                    IdDocumento = reader["NumeroIdentificacion"] == DBNull.Value ? string.Empty : reader["NumeroIdentificacion"].ToString(),
                    Nombre = reader["ApellidosNombre"] == DBNull.Value ? string.Empty : reader["ApellidosNombre"].ToString(),
                    CodigoContrato = reader["CodigoContrato"] == DBNull.Value ? string.Empty : reader["CodigoContrato"].ToString(),
                    NumeroPase = reader["NumeroPase"] == DBNull.Value ? string.Empty : reader["NumeroPase"].ToString(),
                    FechaVenciminetoPase = reader["FechaVencimientoPase"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["FechaVencimientoPase"]),
                    PersonaInterna = new OUPersonaInternaDC()
                    {
                        IdPersonaInterna = reader["IdPersona"] == DBNull.Value ? 0 : Convert.ToInt64(reader["IdPersona"]),
                        IdCargo = reader["Cargo"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Cargo"]),
                        Cargo = reader["DescripcionCargo"] == DBNull.Value ? string.Empty : reader["DescripcionCargo"].ToString(),
                        Direccion = reader["DireccionResidencia"] == DBNull.Value ? string.Empty : reader["DireccionResidencia"].ToString(),
                        Telefono = reader["TelefonoFijoDomicilio"] == DBNull.Value ? string.Empty : reader["TelefonoFijoDomicilio"].ToString(),
                        Email = reader["CorreoCorporativo"] == DBNull.Value ? string.Empty : reader["CorreoCorporativo"].ToString(),
                        Municipio = reader["MunicipioResidencia"] == DBNull.Value ? string.Empty : reader["MunicipioResidencia"].ToString(),
                        Nombre = reader["Nombre"] == DBNull.Value ? string.Empty : reader["Nombre"].ToString(),
                        PrimerApellido = reader["PrimerApellido"] == DBNull.Value ? string.Empty : reader["PrimerApellido"].ToString(),
                        SegundoApellido = reader["SegundoApellido"] == DBNull.Value ? string.Empty : reader["SegundoApellido"].ToString(),
                        FechaInicioContrato = reader["FechaIngreso"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["FechaIngreso"]),
                        FechaTerminacionContrato = reader["FechaTerminacionContrato"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["FechaTerminacionContrato"]),

                    },
                    Estado = new OUEstadosMensajeroDC()
                    {
                        IdEstado = reader["Estado"] == DBNull.Value ? string.Empty : reader["Estado"].ToString(),
                    },
                    EsContratista = reader["EsContratista"] == DBNull.Value ? false : Convert.ToBoolean(reader["EsContratista"]),
                    TipoContrato = new POTipoContrato() {
                        IdTipoContrato = reader["TipoContrato"] == DBNull.Value ? 0 : Convert.ToInt32(reader["TipoContrato"]),
                    },
                    EsMensajeroUrbano = reader["EsMensajeroUrbano"] == DBNull.Value ? false : Convert.ToBoolean(reader["EsMensajeroUrbano"]),

                    IdTipoMensajero = reader["IdTipoMensajero"] == DBNull.Value ? Convert.ToInt16(0) : Convert.ToInt16(reader["IdTipoMensajero"]),

                    LocalidadMensajero = new Framework.Servidor.Servicios.ContratoDatos.Parametros.PALocalidadDC() {
                        IdLocalidad = reader["IdMunicipio"] == DBNull.Value ? string.Empty : reader["IdMunicipio"].ToString(),
                    },
                    CargoMensajero = new Framework.Servidor.Servicios.ContratoDatos.Seguridad.SECargo()
                    {
                        IdCargo = reader["IdCargo"] == DBNull.Value ? 0 : Convert.ToInt32(reader["IdCargo"]),
                    },
                    Celular = reader["CelularCorporativo"] == DBNull.Value ? string.Empty : reader["CelularCorporativo"].ToString(),
                    Agencia = new PUAgenciaDeRacolDC()
                    {
                        IdCentroServicio = reader["IdAgencia"] == DBNull.Value ? 0 : Convert.ToInt64(reader["IdAgencia"]),
                        NombreCentroServicio = reader["NombreRacol"] == DBNull.Value ? string.Empty : reader["NombreRacol"].ToString(),
                    },
                    Placa = reader["PlacaVehiculo"] == DBNull.Value ? string.Empty : reader["PlacaVehiculo"].ToString(),
                    TipoVehiculo = reader["TipoVehiculo"] == DBNull.Value ? string.Empty : reader["TipoVehiculo"].ToString(),
                    IdTipoCiudad = reader["LTC_IdTipoCiudad"] == DBNull.Value ? 0 : Convert.ToInt32(reader["LTC_IdTipoCiudad"]),
                    Foto = reader["Foto"] == DBNull.Value ? null : (byte[])reader["Foto"],
                    TipoPersona = reader["TipoPersona"] == DBNull.Value ? null : reader["TipoPersona"].ToString()
                };
            }
            return mensajeros;
        }

        internal static List<MEMensajero> ToListDetalleEmpleadoNovasoftConfig(SqlDataReader reader)
        {
            List<MEMensajero> mensajeros = new List<MEMensajero>();
            while(reader.Read())
            {
                mensajeros.Add(new MEMensajero
                {
                    //TipoDocumento=reader[""]
                    IdDocumento = reader["NumeroIdentificacion"] == DBNull.Value ? string.Empty : reader["NumeroIdentificacion"].ToString(),
                    Nombre = reader["Nombre"] == DBNull.Value ? string.Empty : reader["Nombre"].ToString(),
                    PersonaInterna = new OUPersonaInternaDC()
                    {
                        IdPersonaInterna = reader["IdPersona"] == DBNull.Value ? 0 : Convert.ToInt64(reader["IdPersona"]),
                        Nombre = reader["Nombre"] == DBNull.Value ? string.Empty : reader["Nombre"].ToString(),
                        PrimerApellido = reader["PrimerApellido"] == DBNull.Value ? string.Empty : reader["PrimerApellido"].ToString(),
                        SegundoApellido = reader["SegundoApellido"] == DBNull.Value ? string.Empty : reader["SegundoApellido"].ToString(),
                   
                    },
                  TipoPersona=reader["TipoPersona"] ==DBNull.Value ? string.Empty : reader["TipoPersona"].ToString()
                });
            }
            return mensajeros;
        }


        internal static MEMensajero ToEmpleadoNovasoft(SqlDataReader reader)
        {
            MEMensajero mensajeros = null;
            if (reader.Read())
            {
                mensajeros = new MEMensajero
                {
                    //TipoDocumento=reader[""]
                    TipoDocumento = reader["TipoIdentificacion"] == DBNull.Value ? string.Empty : reader["TipoIdentificacion"].ToString(),
                    IdDocumento = reader["NumeroIdentificacion"] == DBNull.Value ? string.Empty : reader["NumeroIdentificacion"].ToString(),
                    PersonaInterna = new OUPersonaInternaDC()
                    {
                        Telefono = reader["TelefonoResidencia"] == DBNull.Value ? string.Empty : reader["TelefonoResidencia"].ToString(),
                        IdCargo = reader["Cargo"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Cargo"]),
                        Cargo = reader["NombreCargo"] == DBNull.Value ? string.Empty : reader["NombreCargo"].ToString(),
                        Direccion = reader["DireccionResidencia"] == DBNull.Value ? string.Empty : reader["DireccionResidencia"].ToString(),
                        Email = reader["CorreoCorporativo"] == DBNull.Value ? string.Empty : reader["CorreoCorporativo"].ToString(),
                        Nombre = reader["Nombre"] == DBNull.Value ? string.Empty : reader["Nombre"].ToString(),
                        PrimerApellido = reader["PrimerApellido"] == DBNull.Value ? string.Empty : reader["PrimerApellido"].ToString(),
                        SegundoApellido = reader["SegundoApellido"] == DBNull.Value ? string.Empty : reader["SegundoApellido"].ToString(),
                        FechaInicioContrato = reader["FechaIngreso"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["FechaIngreso"]),
                        FechaTerminacionContrato = reader["FechaTerminacionContrato"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["FechaTerminacionContrato"]),
                    },
                    CargoMensajero = new Framework.Servidor.Servicios.ContratoDatos.Seguridad.SECargo()
                    {
                        IdCargo = reader["Cargo"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Cargo"]),
                    },
                    TipoContrato = new POTipoContrato()
                    {
                        IdTipoContrato = reader["TipoContrato"] == DBNull.Value ? 0 : Convert.ToInt32(reader["TipoContrato"]),
                    },
                    Celular = reader["CelularCorporativo"] == DBNull.Value ? string.Empty : reader["CelularCorporativo"].ToString(),
                    Foto = reader["Foto"] == DBNull.Value ? null : (byte[])reader["Foto"],
                };
            }
            return mensajeros;
        }

        internal static List<POVehiculo> ToListVehiculo(SqlDataReader reader)
        {
            List<POVehiculo> vehiculos = new List<POVehiculo>();

            while (reader.Read())
            {
                vehiculos.Add(new POVehiculo
                {
                    TipoVehiculo = reader["TipoVehiculo"].ToString(),
                    Placa = reader["PlacaVehiculo"].ToString(),
                    IdentificacionPropietario = reader["CedulaPropietario"].ToString()

                });
            }
            return vehiculos;
        }

        internal static List<METipoPAM> ToListTipoPAM(SqlDataReader reader)
        {
            List<METipoPAM> TipoPAM = new List<METipoPAM>();

            while (reader.Read())
            {
                TipoPAM.Add(new METipoPAM
                {
                    IdPAM = Convert.ToInt32(reader["TPP_IdTipoPAM"]),
                    DescripcionPAM = reader["TPP_TipoPAM"].ToString(),
               });
            }
            return TipoPAM;
        }

        internal static MEConfigComisiones ToListConfigComisiones(SqlDataReader reader)
        {
            MEConfigComisiones mensajeros = null;
            if(reader.Read())
            {
                mensajeros = new MEConfigComisiones
                {
                    //TipoDocumento=reader[""]
                    IdPersona = reader["IdPersona"] == DBNull.Value ? 0 : Convert.ToInt32(reader["IdPersona"]),
                    GrupoBasico = new MEGrupoBasico()
                    {
                        IdGrupoBasico = reader["IdGrupoBasico"] == DBNull.Value ? 0 : Convert.ToInt64(reader["IdGrupoBasico"])
                    },
                    GrupoLiquidacion = new MEGruposLiquidacion()
                    {
                        IdGrupoLiq = reader["IdGrupoLiquidacion"] == DBNull.Value ? 0 : Convert.ToInt64(reader["IdGrupoLiquidacion"])
                    },
                    GrupoPenalidad = new METipoPenalidad()
                    {
                        Penalidad = reader["IdGrupoPenalidad"] == DBNull.Value ? "0" : reader["IdGrupoPenalidad"].ToString()
                    },
                    GrupoRodamiento = new MEGrupoRodamiento()
                    {
                        Id = reader["IdGrupoRodamiento"] == DBNull.Value ? 0 : Convert.ToInt32(reader["IdGrupoRodamiento"])
                    }

                };
            }
            return mensajeros;
        }


    }
}
