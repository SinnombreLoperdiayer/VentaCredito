using CO.Servidor.Servicios.ContratoDatos.Clientes;
using CO.Servidor.Servicios.ContratoDatos.Recogidas;
using Framework.Servidor.Comun.Util;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Text;

namespace CO.Servidor.Recogidas.Datos
{
    /// <summary>
    /// Clase para consultar y persistir informacion en la base de datos para los procesos de rutas
    /// </summary>
    public class RGRepositorio
    {
        private static readonly RGRepositorio instancia = new RGRepositorio();
        private string CadCnxController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;
        private Dictionary<RGEnumClaseSolicitud, string> ClasesSolicitud;

        /// <summary>
        /// Retorna la instancia de la clase TARepositorio
        /// </summary>
        public static RGRepositorio Instancia
        {
            get { return RGRepositorio.instancia; }
        }

        /// <summary>
        /// constructor
        /// </summary>
        private RGRepositorio()
        {
            CreaClasesSolicitudes();
        }
        /// <summary>
        /// Obtiene las recogidas que no se han asignado
        /// </summary>
        /// <param name="idCiudad"></param>
        /// <param name="idCol"></param>
        /// <param name="idClienteCredito"></param>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>

        public List<RGRecogidasDC> ObtenerRecogidasPorAsignar(string idCiudad, string idCol, int? idClienteCredito, long? idCentroServicio, int numeroPagina, int tamanioPagina)
        {
            List<RGRecogidasDC> lstRecogidas = new List<RGRecogidasDC>();

            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerRecogidasFijasSinProgramar_REC", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdLocalidad", idCiudad);
                cmd.Parameters.AddWithValue("@IdCentroLogistico", idCol);
                cmd.Parameters.AddWithValue("@IdClienteCredito", idClienteCredito);
                cmd.Parameters.AddWithValue("@IdCentroServicio", idCentroServicio);
                cmd.Parameters.AddWithValue("@PageIndex", numeroPagina);
                cmd.Parameters.AddWithValue("@PageSize", tamanioPagina);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    RGRecogidasDC recogida = new RGRecogidasDC()
                    {
                        Id = Convert.ToInt64(reader["id"]),
                        Nombre = reader["CES_Nombre"].ToString(),
                        Direccion = reader["CES_Direccion"].ToString(),
                        Ciudad = reader["LOC_Nombre"].ToString(),
                        idCliente = Convert.ToInt32(reader["idCliente"]),
                        IdHorarioCentroServicio = Convert.ToInt64(reader["idProgramacionCes"]),
                        IdHorarioSucursal = Convert.ToInt64(reader["idProgramacionSuc"]),
                        Hora = Convert.ToDateTime(reader["hora"]),
                        DiaSemana = Convert.ToInt32(reader["dia"]),
                        IdSucursal = Convert.ToInt32(reader["IdCredit"]),
                        IdCentroServicio = Convert.ToInt32(reader["IdPunto"]),
                        Paginas = Convert.ToInt64(reader["Paginas"])
                    };
                    CultureInfo ci = new CultureInfo("Es-Es");
                    recogida.DiaSemana = recogida.DiaSemana == 7 ? 0 : recogida.DiaSemana;
                    recogida.DescripcionHorario = ci.DateTimeFormat.GetDayName((DayOfWeek)recogida.DiaSemana).ToString() + ' ' + recogida.Hora.Hour.ToString() + ':' + (recogida.Hora.Minute <= 9 ? '0' + recogida.Hora.Minute.ToString() : recogida.Hora.Minute.ToString());
                    lstRecogidas.Add(recogida);
                }
                conn.Close();
            }
            return lstRecogidas;
        }

        public long ObtenerCantidadFijasPorAsignar(String idCentroServicio)
        {
            long resultado = 0;
            using (var conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerCantidadRecogidasFijas_REC", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdCentroLogistico", idCentroServicio);
                conn.Open();
                resultado = Convert.ToInt64(cmd.ExecuteScalar());
            }
            return resultado;
        }

        public void CancelarSolicitudRecogida(RGAsignacionSolicitudRecogidaDC asignacion)
        {
            using (var conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paCancelarSolicitudRecogida_REC", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdSolicitud", asignacion.IdSolicitudRecogida);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void BorrarAsignacionSolicitudRecogida(RGAsignacionSolicitudRecogidaDC asignacion)
        {
            using (var conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paBorrarAsignacionSolicitudRecogida_REC", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdSolicitud", asignacion.IdSolicitudRecogida);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// obtiene todos los empleados que sean mensajeros conductores y auxiliares de zona sin importar que esten activos en la empresa
        /// </summary>
        /// <returns></returns>
        public List<RGEmpleadoDC> ObtenerEmpleadosParaReAsignarRecogidas(string idLocalidad)
        {
            List<RGEmpleadoDC> lstEmpleado = new List<RGEmpleadoDC>();
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerEmpleadosParaReAsignarRecogidas_REC", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idLocalidad", idLocalidad);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    RGEmpleadoDC empleado = new RGEmpleadoDC
                    {
                        idEmpleado = reader["Identificacion"].ToString(),
                        nombreEmpleado = reader["NombreEmpleado"].ToString(),
                        idTipoIdentificacion = reader["TipoIdentificacion"].ToString(),
                        email = string.IsNullOrEmpty(reader["e_mail"].ToString()) ? "N/A" : reader["e_mail"].ToString(),
                        idCargo = reader["cod_car"].ToString(),
                        descripcionCargo = reader["nom_car"].ToString(),
                        direccion = reader["dir_res"].ToString(),
                        telefonoFijo = reader["tel_res"].ToString(),
                        ciudadEmpleado = reader["LOC_Nombre"].ToString(),
                        racolEmpleado = reader["REA_Descripcion"].ToString(),
                        estadoEmpleado = Convert.ToBoolean(reader["estado"])
                    };
                    lstEmpleado.Add(empleado);
                }
                conn.Close();
            }
            return lstEmpleado;
        }

        /// <summary>
        /// Obtener nuevo estado motivo solicitud
        /// </summary>
        /// <param name="idMotivo"></param>
        /// <param name="idActor"></param>
        /// <returns></returns>
        public EnumEstadoSolicitudRecogida ObtenerNuevoEstadoMotivosSolicitud(int idMotivo, int idActor)
        {
            EnumEstadoSolicitudRecogida resultado = EnumEstadoSolicitudRecogida.None;

            using (var conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerNuevoEstadoMotivosSolicitud_REC", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdMotivo", idMotivo);
                cmd.Parameters.AddWithValue("@IdActorMotivo", idActor);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                resultado = MapperRepositorio.MapperToEnumEstadoSolicitudRecogida(reader);
            }

            return resultado;

        }


        /// <summary>
        /// Obtener Motivos Estado Solicitud Recogida X Actor
        /// </summary>
        /// <param name="idActor"></param>
        /// <returns></returns>
        public List<RGMotivoEstadoSolRecogidaDC> ObtenerMotivoEstadoSolRecogidaXActor(long idActor)
        {
            List<RGMotivoEstadoSolRecogidaDC> resultado = null;
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerMotivosActor_REC", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdActorMotivo", idActor);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                resultado = MapperRepositorio.MapperToListaMotivoEstadoSolRecogida(reader);
            }
            return resultado;
        }

        /// <summary>
        /// Inserta la programacion de recogidas fijas a un mensajero
        /// </summary>
        public long InsertarProgramacionRecogidasFijas(RGProgramacionRecogidaFijaDC programacion, RGRecogidasDC recogidas)
        {
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paInsertarProgramacionRecogidasFijas_REC", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idScursal", recogidas.idCliente);//TODO RANCID: ya no se deben guardar
                cmd.Parameters.AddWithValue("@idCentroServicios", recogidas.Id);//TODO RANCID: ya no se deben guardar
                cmd.Parameters.AddWithValue("@estadoProgramacion", programacion.estadoProgramacion);
                cmd.Parameters.AddWithValue("@docPersonaResponsable", programacion.docPersonaResponsable);
                cmd.Parameters.AddWithValue("@placaVehiculo", programacion.placaVehiculo);
                cmd.Parameters.AddWithValue("@asignacionTemporal", programacion.asignacionTemporal);
                cmd.Parameters.AddWithValue("@docPersonaRespTemporal", programacion.docPersonaRespTemporal);
                cmd.Parameters.AddWithValue("@FechaInicialAsignacionTemp", programacion.FechaInicialAsignacionTemp);
                cmd.Parameters.AddWithValue("@fechaFinalAsignacionTemp", programacion.FechaFinalAsignacionTemp);
                cmd.Parameters.AddWithValue("@HorarioCentroServicio", recogidas.IdHorarioCentroServicio);//TODO RANCID: Modificar sp para traer el id de horario sucursal o idhorario ces
                cmd.Parameters.AddWithValue("@HorarioCliente", recogidas.IdHorarioSucursal);//TODO RANCID: Modificar sp para traer el id de horario sucursal o idhorario ces
                cmd.Parameters.AddWithValue("@creadoPor", ControllerContext.Current.Usuario);
                conn.Open();
                var id = cmd.ExecuteScalar();
                if (id != null)
                {
                    return Convert.ToInt64(id);
                }
                else
                {
                    return 0;
                }
                conn.Close();
            }
        }

        /// <summary>
        /// Obtiene los horarios para sucursal 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<string> ObtenerHorarioRecogidaSucursal(int? idCliente)
        {
            List<string> lstHorarios = new List<string>();
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerHorarioRecogidaConsolidadoPorSucursal_REC", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idSucursal", idCliente);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string horario = "";
                    horario = reader["Horario"].ToString();
                    lstHorarios.Add(horario);
                }
                conn.Close();
            }
            return lstHorarios;
        }

        /// <summary>
        /// Obtiene la ultima solicitud registrada
        /// </summary>
        /// <returns></returns>
        public RGRecogidasDC ObtenerUltimaSolicitud(string numeroDocumento)
        {
            RGRecogidasDC recogida = new RGRecogidasDC();
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paConsultarUltimaSolicitud_REC", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroDocumento", numeroDocumento);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    recogida.Nombre = reader["SRP_NombreCompleto"].ToString();
                    recogida.Direccion = reader["SRP_DireccionRecogida"].ToString();
                    recogida.Correo = reader["SRP_CorreoElectronico"].ToString();
                    recogida.Ciudad = reader["SRE_IdLocalidadRecogida"].ToString();
                    recogida.Latitud = reader["SRE_Latitud"].ToString();
                    recogida.Longitud = reader["SRE_Longitud"].ToString();
                    recogida.NombreCiudad = reader["LOC_Nombre"].ToString();
                    recogida.Id = Convert.ToInt64(reader["SRP_NumeroDocumento"]);
                    recogida.NumeroTelefono = reader["SRP_NumeroTelefonico"].ToString();
                    recogida.FechaRecogida = Convert.ToDateTime(reader["SRE_FechaHoraRecogida"]);
                    recogida.PreguntarPor = reader["SRP_PreguntarPor"].ToString();
                    recogida.DescripcionEnvios = reader["SRP_DescripcionEnvios"].ToString();
                    recogida.TotalPiezas = Convert.ToInt32(reader["SRP_TotalPiezas"]);
                    recogida.PesoAproximado = Convert.ToInt32(reader["SRP_PesoAproximado"]);
                    recogida.TipoDocumento = reader["SRP_TipoDocumento"].ToString();
                }
                conn.Close();
            }
            return recogida;
        }

        public void InsertaTelemercadeo(RGTelemercadeo telemercadeo)
        {
            using (var clientConn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand();
                clientConn.Open();
                cmd = new SqlCommand("paInsertarTelemercadeos_REC", clientConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdSolRecogida", telemercadeo.IdSolRecogida);
                cmd.Parameters.AddWithValue("@Observacion", telemercadeo.Observacion);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);

                cmd.ExecuteNonQuery();
            }

        }

        public RGGraficaDonnaPie ObtenerEstadisticasPorFecha(DateTime fechaInicial, Nullable<DateTime> fechaFinal, string idCiudad, string idTerritorial, string idRegional, bool sendFlag)
        {

            RGGraficaDonnaPie result = new RGGraficaDonnaPie
            {
                Axis = new List<RGDictionary>(),
                Titulo = string.Empty
            };

            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerEstadisticasPorFecha_REC", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FechaInicial", fechaInicial);
                if (fechaFinal != null) cmd.Parameters.AddWithValue("@FechaFinal", fechaFinal);
                cmd.Parameters.AddWithValue("@IdCiudad", idCiudad);
                cmd.Parameters.AddWithValue("@IdTerritorial", idTerritorial);
                cmd.Parameters.AddWithValue("@IdRegional", idRegional);
                cmd.Parameters.AddWithValue("@Recogidas", sendFlag ? 0 : 1);
                cmd.Parameters.AddWithValue("@Entregas", sendFlag ? 1 : 0);
                conn.Open();

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);



                for (int i = 0; i <= dt.Columns.Count - 1; i++)
                {
                    result.Axis.Add(new RGDictionary()
                    {
                        Name = dt.Columns[i].ColumnName,
                        Value = dt.Rows[0][dt.Columns[i].ColumnName].ToString()
                    });

                }
                conn.Close();
            }

            return result;
        }

        public void ModificarRecogidaEsporadica(RGRecogidasDC recogida)
        {
            using (var conn = new SqlConnection(CadCnxController))
            {

                SqlCommand cmd = new SqlCommand("paModificarInformacionSolicitudRecogida", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdSolRecogida", recogida.Id);
                cmd.Parameters.AddWithValue("@FechaHoraRecogida", recogida.FechaRecogida);
                cmd.Parameters.AddWithValue("@NombreCompleto", recogida.Nombre);
                cmd.Parameters.AddWithValue("@PreguntarPor", recogida.PreguntarPor);
                cmd.Parameters.AddWithValue("@DireccionRecogida", recogida.Direccion);
                cmd.Parameters.AddWithValue("@DescripcionEnvios", recogida.DescripcionEnvios);
                cmd.Parameters.AddWithValue("@PesoAproximado", recogida.PesoAproximado);
                cmd.Parameters.AddWithValue("@TotalPiezas", recogida.TotalPiezas);
                cmd.Parameters.AddWithValue("@Usuario", ControllerContext.Current.Usuario);

                conn.Open();
                var resultado = cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        public void ModificarCoordenadasRecogidaEsporadica(RGRecogidaEsporadicaDC recogida)
        {
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paModificarCoordenadasRecogidaEsporadica", conn);
                conn.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdSolRecogida", recogida.IdSolRecogida);
                cmd.Parameters.AddWithValue("@DireccionRecogida", recogida.DireccionRecogida);
                cmd.Parameters.AddWithValue("@Latitud", recogida.Latitud);
                cmd.Parameters.AddWithValue("@Longitud", recogida.Longitud);
                cmd.Parameters.AddWithValue("@Usuario", ControllerContext.Current.Usuario);

                cmd.ExecuteNonQuery();
            }
        }

        public List<RGDictionary> ObtenerComportamientoEntregaHoras(DateTime fechaInicial, Nullable<DateTime> fechaFinal, string idCiudad, string idTerritorial, string idRegional)
        {
            List<RGDictionary> balance = new List<RGDictionary>();

            /*
             @FechaInicial DATETIME,
             @FechaFinal DATETIME  = NULL,
             @Ciudad VARCHAR(60) = NULL,
             @IdTerritorial VARCHAR(10) = NULL,
             @IdRegional VARCHAR(5) = NULL
             */
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerComportamientoEntregaHoras_REC", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FechaInicial", fechaInicial);
                if (fechaFinal != null) cmd.Parameters.AddWithValue("@FechaFinal", fechaFinal.Value);
                cmd.Parameters.AddWithValue("@IdCiudad", idCiudad);
                cmd.Parameters.AddWithValue("@IdTerritorial", idTerritorial);
                cmd.Parameters.AddWithValue("@IdRegional", idRegional);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    RGDictionary dupla = new RGDictionary
                    {
                        Name = reader["HORA"].ToString(),
                        Value = reader["NUMERO"].ToString()
                    };

                    balance.Add(dupla);
                }
                conn.Close();
            }

            return balance;
        }

        public List<RGDictionary> ObtenerComportamientoRecogidasHoras(DateTime fechaInicial, Nullable<DateTime> fechaFinal, string idCiudad, string idTerritorial, string idRegional)
        {
            List<RGDictionary> balance = new List<RGDictionary>();
            /*
             @FechaInicial DATETIME,
             @FechaFinal DATETIME  = NULL,
             @Ciudad VARCHAR(60) = NULL,
             @IdTerritorial VARCHAR(10) = NULL,
             @IdRegional VARCHAR(5) = NULL
             */
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerComportamientoRecogidaHoras_REC", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FechaInicial", fechaInicial);
                if (fechaFinal != null) cmd.Parameters.AddWithValue("@FechaFinal", fechaFinal);
                cmd.Parameters.AddWithValue("@idCiudad", idCiudad);
                cmd.Parameters.AddWithValue("@IdTerritorial", idTerritorial);
                cmd.Parameters.AddWithValue("@IdRegional", idRegional);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    RGDictionary dupla = new RGDictionary
                    {
                        Name = reader["HORA"].ToString(),
                        Value = reader["NUMERO"].ToString()
                    };

                    balance.Add(dupla);
                }
                conn.Close();
            }

            return balance;
        }

        public RGBestSellingChart ObtenerValoresTopPorCiudad(DateTime fechaInicial, Nullable<DateTime> fechaFinal, bool sendFlag)
        {
            RGBestSellingChart result = new RGBestSellingChart
            {
                Axis = new List<List<RGDictionary>>(),
                Titulo = string.Empty
            };

            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerValoresTopPorCiudad_REC", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FechaInicial", fechaInicial);
                if (fechaFinal != null) cmd.Parameters.AddWithValue("@FechaFinal", fechaFinal.Value);
                cmd.Parameters.AddWithValue("@Recogidas", sendFlag ? 0 : 1);
                cmd.Parameters.AddWithValue("@Entregas", sendFlag ? 1 : 0);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    int count = 0;
                    List<RGDictionary> currentRow = new List<RGDictionary>();
                    while (count <= reader.FieldCount - 1)
                    {
                        RGDictionary dupla = new RGDictionary
                        {
                            Name = reader.GetName(count),
                            Value = reader[reader.GetName(count)].ToString()
                        };

                        currentRow.Add(dupla);
                        count++;
                    }

                    result.Axis.Add(currentRow);

                }
                conn.Close();
            }

            return result;
        }

        public List<RGDispositivoMensajeroDC> ObtenerMensajerosForzarRecogida(string ubicaciones)
        {
            List<RGDispositivoMensajeroDC> resultado = new List<RGDispositivoMensajeroDC>();
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerMensajerosForzarRecogida_REC", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ubicaciones", ubicaciones);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                resultado = MapperRepositorio.MapperToRGDispositivoMensajeroDC(reader);
            }
            return resultado;
        }

        public List<RGDetalleMensajeroBalance.RGDetalleInfoMensajero> ObtenerMensajerosActivosPorAplicacion(DateTime fechaInicial, Nullable<DateTime> fechaFinal, string idCiudad, string idTerritorial, string idRegional)
        {
            List<RGDetalleMensajeroBalance.RGDetalleInfoMensajero> result = new List<RGDetalleMensajeroBalance.RGDetalleInfoMensajero>();

            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerMensajerosActivosPorAplicacion_REC", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FechaInicial", fechaInicial);
                if (fechaFinal != null)
                {
                    fechaFinal = fechaFinal.Value.AddDays(1);
                    cmd.Parameters.AddWithValue("@FechaFinal", fechaFinal);
                }
                if (!string.IsNullOrEmpty(idCiudad))
                    cmd.Parameters.AddWithValue("@IdCiudad", idCiudad);
                if (!string.IsNullOrEmpty(idTerritorial))
                    cmd.Parameters.AddWithValue("@IdTerritorial", idTerritorial);
                if (!string.IsNullOrEmpty(idRegional))
                    cmd.Parameters.AddWithValue("@IdRegional", idRegional);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    RGDetalleMensajeroBalance.RGDetalleInfoMensajero currentRow = new RGDetalleMensajeroBalance.RGDetalleInfoMensajero()
                    {
                        IdLocalidad = reader["IdLocalidad"].ToString(),
                        IdMensajero = Convert.ToInt64(reader["IdMensajero"]),
                        Nombre = reader["Nombre"].ToString(),
                        //Latitud = reader["Latitud"].ToString(),
                        //Longitud = reader["Longitud"].ToString(),
                        //Foto = Convert.ToBase64String((byte[])reader["Foto"]),
                        Entregas = reader["Entregas"].ToString(),
                        Recogidas = reader["Recogidas"].ToString()
                        //,Placa = reader["Placa"].ToString(),
                        //TipoVehiculo = reader["TipoVehiculo"].ToString(),
                        //ColorVehiculo = reader["ColorVehiculo"].ToString(),
                        //TipoContrato = reader["TipoContrato"].ToString()
                    };

                    result.Add(currentRow);
                }
                conn.Close();
            }

            return result;

        }

        /// <summary>
        /// inserta la auditoria de la reasignacion de recogidas fijas ya programadas en la app
        /// </summary>
        /// <param name="p"></param>
        public void InsertarAuditoriaRecogidasFijas(RGRecogidasDC p)
        {
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paInsertarAuditoriaAsignacionRecogidasFijas_REC", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdProgramacionRecogidas", p.IdProgramacion);
                cmd.Parameters.AddWithValue("@CambiadoPor", ControllerContext.Current.Usuario);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        /// <summary>
        /// Metodo para insertar las fotografias de la recogida
        /// </summary>
        public void InsertarFotografiasRecogida(RGAsignarRecogidaDC recogida)
        {
            string rutaImagenes;
            string carpetaDestino;

            using (SqlConnection cnn = new SqlConnection(CadCnxController))
            {
                int file = 0;
                rutaImagenes = PAParametros.Instancia.ConsultarParametrosFramework("FoldImgRecogidaIL");
                carpetaDestino = Path.Combine(rutaImagenes + "\\" + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year);
                if (!Directory.Exists(carpetaDestino))
                {
                    Directory.CreateDirectory(carpetaDestino);
                }
                if (recogida.FotografiasRecogida != null)
                {
                    foreach (String imagen in recogida.FotografiasRecogida)
                    {
                        file++;
                        byte[] bytebuffer = Convert.FromBase64String(imagen);
                        MemoryStream memoryStream = new MemoryStream(bytebuffer);
                        var image = Image.FromStream(memoryStream);
                        ImageCodecInfo jpgEncoder = UtilidadesFW.GetEncoder(ImageFormat.Jpeg);
                        string ruta = carpetaDestino + "\\" + recogida.IdSolicitudRecogida + "-" + file + ".jpg";
                        System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                        EncoderParameters myEncoderParameters = new EncoderParameters(1);
                        EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 180L);
                        myEncoderParameters.Param[0] = myEncoderParameter;
                        image.Save(ruta, jpgEncoder, myEncoderParameters);

                        SqlCommand cmd = new SqlCommand("paInsertarImagenesEvidenciaRecogidas_REC", cnn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@IdSolicitudRecogida", recogida.IdSolicitudRecogida);
                        cmd.Parameters.AddWithValue("@RutaImagen", ruta);
                        cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);
                        cnn.Open();
                        cmd.ExecuteNonQuery();
                        cnn.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Obtiene los adjuntos de una cita
        /// </summary>
        /// <param name="idSolicitudRecogida"></param>
        /// <returns></returns>
        public List<string> ObtenerEvidenciasRecogida(long idSolicitudRecogida)
        {
            List<string> lstImagenes = new List<string>();
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerImagenesRecogidas_REC", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdSolicitud", idSolicitudRecogida);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                //while (reader.Read())
                //{
                //    string imagen;
                //    using (FileStream fs = File.OpenRead(reader["IER_RutaImagen"].ToString()))
                //    {
                //        byte[] bytes = new byte[fs.Length];
                //        fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
                //        fs.Close();
                //        imagen = Convert.ToBase64String(bytes);
                //    }
                //    lstImagenes.Add(imagen);
                //}
                while (reader.Read())
                {
                    byte[] imagen = File.ReadAllBytes(reader["IER_RutaImagen"].ToString());
                    lstImagenes.Add(Convert.ToBase64String(imagen));
                }
                conn.Close();
            }
            return lstImagenes;
        }

        /// <summary>
        /// Metodo para actualizar numero piezas de recogida fija 
        /// </summary>
        /// <param name="recogida"></param>
        public void ActualizarNumeroPiezasRecogidaFija(RGAsignarRecogidaDC recogida)
        {
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paActualizarNumeroPiezasRecogidaFija_REC", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdSolicitudRecogda", recogida.IdSolicitudRecogida);
                cmd.Parameters.AddWithValue("@NumeroPiezas", recogida.NumeroPiezas);
                conn.Open();
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                conn.Close();
            }
        }

        /// <summary>
        /// Metodo para obtener posicion de mensajeros activos 
        /// </summary>
        /// <param name="fechaInicial"></param>
        /// <param name="fechaFinal"></param>
        /// <param name="idCiudad"></param>
        /// <param name="idTerritorial"></param>
        /// <param name="idRegional"></param>
        /// <returns></returns>
        public List<RGDetalleMensajeroBalance.RGDetalleInfoMensajero> ObtenerPosicionesMensajerosActivos(DateTime fechaInicial, Nullable<DateTime> fechaFinal, string idCiudad, string idTerritorial, string idRegional)
        {
            List<RGDetalleMensajeroBalance.RGDetalleInfoMensajero> result = new List<RGDetalleMensajeroBalance.RGDetalleInfoMensajero>();

            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerPosicionesMensajerosActivos_REC", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FechaInicial", DateTime.Now);
                if (fechaFinal != null) cmd.Parameters.AddWithValue("@FechaFinal", DateTime.Now);
                if (!string.IsNullOrEmpty(idCiudad))
                    cmd.Parameters.AddWithValue("@IdCiudad", idCiudad);
                if (!string.IsNullOrEmpty(idTerritorial))
                    cmd.Parameters.AddWithValue("@IdTerritorial", idTerritorial);
                if (!string.IsNullOrEmpty(idRegional))
                    cmd.Parameters.AddWithValue("@IdRegional", idRegional);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        RGDetalleMensajeroBalance.RGDetalleInfoMensajero currentRow = new RGDetalleMensajeroBalance.RGDetalleInfoMensajero()
                        {
                            //Telefono = ObtenerNumeroCorporativoPorusuario(reader["PEI_Identificacion"].ToString()),
                            Telefono = reader["Telefono"].ToString(),
                            IdLocalidad = reader["IdLocalidad"].ToString(),
                            IdMensajero = Convert.ToInt64(reader["IdMensajero"]),
                            Nombre = reader["Nombre"].ToString(),
                            Latitud = reader["Latitud"].ToString(),
                            Longitud = reader["Longitud"].ToString(),
                            Placa = reader["Placa"].ToString(),
                            TipoVehiculo = reader["TipoVehiculo"].ToString(),
                            ColorVehiculo = reader["ColorVehiculo"].ToString(),
                            TipoContrato = reader["TipoContrato"].ToString(),
                            FechaGrabacion = reader["FechaGrabacion"] == null ? new DateTime() : Convert.ToDateTime(reader["FechaGrabacion"])
                        };

                        result.Add(currentRow);
                    }
                }
                conn.Close();
            }

            return result;
        }

        public string ObtenerNumeroCorporativoPorusuario(string idUsuario)
        {
            string numeroTelefonico = "";
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerNumeroCorporativo_REC", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Idusuario", idUsuario);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    numeroTelefonico = reader["usuario"].ToString();
                }
                conn.Close();
            }
            return numeroTelefonico;
        }

        public List<RGDetalleMensajeroBalance.RGDetalleRutaMensajero> ObtenerDetalleRutaMensajero(DateTime fechaInicial, Nullable<DateTime> fechaFinal, string idMensajero)
        {
            List<RGDetalleMensajeroBalance.RGDetalleRutaMensajero> result = new List<RGDetalleMensajeroBalance.RGDetalleRutaMensajero>();

            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerDetalleRutaMensajero_REC", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FechaInicial", fechaInicial);
                if (fechaFinal != null) cmd.Parameters.AddWithValue("@FechaFinal", fechaFinal.Value);
                cmd.Parameters.AddWithValue("@IdMensajero", idMensajero);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var currentRow = new RGDetalleMensajeroBalance.RGDetalleRutaMensajero
                    {
                        EsEntrega = Convert.ToBoolean(reader["EsEntrega"]),
                        EsRecogida = Convert.ToBoolean(reader["EsRecogida"]),
                        FechaGrabacion = Convert.ToDateTime(reader["FechaGrabacion"]),
                        Latitud = reader["Latitud"].ToString(),
                        Longitud = reader["Longitud"].ToString(),
                        EsDevolucion = Convert.ToBoolean(reader["EsDevolucion"]),
                        Guia = reader["Guia"].ToString()
                    };

                    result.Add(currentRow);
                }
                conn.Close();
            }

            return result;

        }

        public RGDetalleMensajeroBalance.RGDetalleInfoMensajero ObtenerDetalleInfoMensajero(DateTime fechaInicial, Nullable<DateTime> fechaFinal, string idMensajero)
        {
            RGDetalleMensajeroBalance.RGDetalleInfoMensajero result = new RGDetalleMensajeroBalance.RGDetalleInfoMensajero();

            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerDetalleMensajeroIndicadores_REC", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FechaInicial", fechaInicial);
                if (fechaFinal != null) cmd.Parameters.AddWithValue("@FechaFinal", fechaFinal.Value);
                cmd.Parameters.AddWithValue("@IdMensajero", idMensajero);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    result = new RGDetalleMensajeroBalance.RGDetalleInfoMensajero()
                    {
                        Email = reader["Email"].ToString(),
                        IdLocalidad = reader["IdLocalidad"].ToString(),
                        Entregas = reader["Entregas"].ToString(),
                        Nombre = reader["Nombre"].ToString(),
                        Recogidas = reader["Recogidas"].ToString(),
                        Telefono = reader["Telefono"].ToString(),
                        Placa = reader["Placa"].ToString(),
                        TipoVehiculo = reader["TipoVehiculo"].ToString(),
                        ColorVehiculo = reader["ColorVehiculo"].ToString(),
                        TipoContrato = reader["TipoContrato"].ToString(),
                        Foto = reader["Foto"] != DBNull.Value ? Convert.ToBase64String((byte[])reader["Foto"]) : null
                    };


                }
                conn.Close();
            }

            return result;
        }
        /// <summary>
        /// inserta un cambio de estado de una solicitud
        /// </summary>
        /// <param name="solicitud"></param>
        public void InsertarEstadoSolRecogidaTraza(RGAsignarRecogidaDC solicitud, EnumEstadoSolicitudRecogida nuevoEstado)
        {
            using (SqlConnection sqlconn = new SqlConnection(CadCnxController))
            {
                var cmd = new SqlCommand("paInsertarEstadoSolRecogidaTraza_REC", sqlconn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdSolicitudRecogida", solicitud.IdSolicitudRecogida);
                cmd.Parameters.AddWithValue("@IdEstado", nuevoEstado);
                cmd.Parameters.AddWithValue("@LocalidadCambio", solicitud.LocalidadCambio);
                cmd.Parameters.AddWithValue("@Longitud", solicitud.Longitud);
                cmd.Parameters.AddWithValue("@Latitud", solicitud.Latitud);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);
                if (solicitud.IdMotivo != 0)
                {
                    cmd.Parameters.AddWithValue("@IdMotivo", solicitud.IdMotivo);
                    cmd.Parameters.AddWithValue("@DescripcionMotivo", solicitud.DescripcionMotivo);
                }
                cmd.Parameters.AddWithValue("@DocPersonaResponsable", solicitud.DocPersonaResponsable);
                sqlconn.Open();

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// inserta una solicitud con el estado de reservada
        /// </summary>
        /// <param name="solicitud"></param>
        public void InsertarAsignacionSolicitud(RGRecogidasDC recogida)
        {
            using (SqlConnection sqlconn = new SqlConnection(CadCnxController))
            {
                var cmd = new SqlCommand("paInsertarSolicitudesRecogidasFijasIndividual_REC", sqlconn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdSucursal", recogida.IdSucursal);
                cmd.Parameters.AddWithValue("@DocResponsable", recogida.NumeroDocumento);
                cmd.Parameters.AddWithValue("@HoraRecogida", recogida.Hora);
                cmd.Parameters.AddWithValue("@IdCentroServicios", recogida.IdCentroServicio);
                cmd.Parameters.AddWithValue("@IdProgramacionRecogidas", recogida.IdProgramacion);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);
                sqlconn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void InsertarAsignacionSolicitudRecogida_REC(RGAsignacionSolicitudRecogidaDC asignacion)
        {


            using (var clientConn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand();
                clientConn.Open();
                cmd = new SqlCommand("paInsertarAsignacionSolicitudRecogidaREC", clientConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DocPersonaResponsable", asignacion.DocPersonaResponsable);
                cmd.Parameters.AddWithValue("@IdSolicitudRecogida", asignacion.IdSolicitudRecogida);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);
                cmd.Parameters.AddWithValue("@PlacaVehiculo", asignacion.PlacaVehiculo);

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Inserta las recogidas esporadicas
        /// </summary>
        /// <param name="recogida"></param>
        /// <returns></returns>
        public long InsertarSolicitudRecogida(RGRecogidasDC recogida)
        {
            long idRecogida = 0;
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paInsertarSolicitudRecogida_REC", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TipoSolRecogida", recogida.TipoRecogida);
                cmd.Parameters.AddWithValue("@IdOrigenSolRecogida", ControllerContext.Current.IdAplicativoOrigen);
                cmd.Parameters.AddWithValue("@IdEstadoSolicitud", EnumEstadoSolicitudRecogida.Creado);
                cmd.Parameters.AddWithValue("@IdClienteContado", recogida.idCliente);
                cmd.Parameters.AddWithValue("@FechaHoraRecogida", recogida.FechaRecogida);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);
                cmd.Parameters.AddWithValue("@Longitud", recogida.Longitud);
                cmd.Parameters.AddWithValue("@Latitud", recogida.Latitud);
                cmd.Parameters.AddWithValue("@IdLocalidadRecogida", recogida.Ciudad);
                cmd.Parameters.AddWithValue("@EsEsporadicaCliente", 1);
                if (recogida.IdSucursal > 0)
                {
                    cmd.Parameters.AddWithValue("@IdSucursal", recogida.IdSucursal);
                }
                cmd.Parameters.AddWithValue("@IdCentroServicios", recogida.IdCentroServicio);
                cmd.Parameters.AddWithValue("@TipoDocumento", recogida.TipoDocumento);
                cmd.Parameters.AddWithValue("@NumeroDocumento", recogida.NumeroDocumento);
                cmd.Parameters.AddWithValue("@NombreCompleto", recogida.Nombre);
                cmd.Parameters.AddWithValue("@DireccionRecogida", recogida.Direccion);
                cmd.Parameters.AddWithValue("@CorreoElectronico", recogida.Correo);
                cmd.Parameters.AddWithValue("@NumeroTelefonico", recogida.NumeroTelefono);
                cmd.Parameters.AddWithValue("@PreguntarPor", recogida.PreguntarPor);
                cmd.Parameters.AddWithValue("@DescripcionEnvios", recogida.DescripcionEnvios);
                cmd.Parameters.AddWithValue("@TotalPiezas", recogida.TotalPiezas);
                cmd.Parameters.AddWithValue("@PesoAproximado", recogida.PesoAproximado);
                conn.Open();
                var resultado = cmd.ExecuteScalar();
                conn.Close();
                if (resultado != null)
                {
                    idRecogida = Convert.ToInt64(resultado);
                }
            }
            return idRecogida;
        }


        /// <summary>
        /// Obtiene las recogidas programadas a cierto empleado
        /// </summary>
        /// <returns></returns>
        public List<RGRecogidasDC> ObtenerRecogidasPorEmpleado(string idEmpleado)
        {
            List<RGRecogidasDC> lstRecogidas = new List<RGRecogidasDC>();
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerRecogidasAsignadasPorEmpleado_REC", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idempleado", idEmpleado);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    RGRecogidasDC recogida = new RGRecogidasDC
                    {
                        Id = Convert.ToInt64(reader["id"]),
                        Nombre = reader["CES_Nombre"].ToString(),
                        Direccion = reader["CES_Direccion"].ToString(),
                        Ciudad = reader["LOC_Nombre"].ToString(),
                        idCliente = Convert.ToInt32(reader["idCliente"]),
                        IdAsignacion = Convert.ToInt64(reader["ARF_IdAsignacion"]),
                        IdProgramacion = Convert.ToInt64(reader["ARF_IdProgramacionRecogida"]),
                        Hora = Convert.ToDateTime(reader["hora"]),
                        DiaSemana = Convert.ToInt32(reader["dia"]),
                        Paginas = Convert.ToInt32(reader["Paginas"])
                    };
                    CultureInfo ci = new CultureInfo("Es-Es");
                    recogida.DiaSemana = recogida.DiaSemana == 7 ? 0 : recogida.DiaSemana;
                    recogida.DescripcionHorario = ci.DateTimeFormat.GetDayName((DayOfWeek)recogida.DiaSemana).ToString() + ' ' + recogida.Hora.Hour.ToString() + ':' + (recogida.Hora.Minute <= 9 ? '0' + recogida.Hora.Minute.ToString() : recogida.Hora.Minute.ToString());
                    lstRecogidas.Add(recogida);
                }
                conn.Close();
            }
            return lstRecogidas;
        }

        /// <summary>
        /// Edita las recogidas asignadas a un empleado con el fin de asignarselas a un nuevo empleado
        /// </summary>
        /// <param name="programacion"></param>
        public void EditarProgramacionRecogidasFijas(RGProgramacionRecogidaFijaDC programacion)
        {
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paModificarAsignacionRecogidasFijas_REC", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PlacaVehiculo", programacion.placaVehiculo);
                cmd.Parameters.AddWithValue("@EsAsignacionTemporal", programacion.asignacionTemporal);
                cmd.Parameters.AddWithValue("@DocPersonaRespTemporal", programacion.docPersonaRespTemporal);
                cmd.Parameters.AddWithValue("@FechaInicialAsignacionTemp", programacion.FechaInicialAsignacionTemp);
                cmd.Parameters.AddWithValue("@FechaFinalAsignacionTemp", programacion.FechaFinalAsignacionTemp);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);
                cmd.Parameters.AddWithValue("@IdAsignacion", programacion.IdAsignacion);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        /// <summary>
        /// Edita las recogidas DIARIA asignadas a un empleado con el fin de asignarselas a un nuevo empleado
        /// </summary>
        /// <param name="programacion"></param>
        public void EditarRecogidasFijasPendientesDeRecoger(RGProgramacionRecogidaFijaDC programacion, RGRecogidasDC recogida)
        {
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paModificarAsignacionDiariaSolicitudesFijas_REC", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdProgramacionRecogidas", recogida.IdProgramacion);
                cmd.Parameters.AddWithValue("@IdResponsable", programacion.docPersonaRespTemporal);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        /// <summary>
        /// Obtiene los horarios para sucursal 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<string> ObtenerHorarioRecogidaCentroServicio(int? idCentroServicio)
        {
            List<string> lstHorarios = new List<string>();
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerHorarioRecogidaConsolidadoPorCentroServicio_REC", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idCentroServicio", idCentroServicio);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string horario = "";
                    horario = reader["Horario"].ToString();
                    lstHorarios.Add(horario);
                }
                conn.Close();
            }
            return lstHorarios;
        }

        public List<RGRecogidasDC> ObtenerCentrosDeServicio(string idCol, string idCiudad)
        {
            List<RGRecogidasDC> lstCentrosDeServicio = new List<RGRecogidasDC>();
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerPuntosYAgenciasConRecogidasPorCiudad_REC", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdCentroLogistico", idCol);
                if (idCiudad.ToLower() != "null")
                {
                    cmd.Parameters.AddWithValue("@IdLocalidad", idCiudad);
                }
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    RGRecogidasDC centroServicio = new RGRecogidasDC
                    {
                        Nombre = reader["CES_Nombre"].ToString(),
                        Id = Convert.ToInt32(reader["CES_IdCentroServicios"])
                    };
                    lstCentrosDeServicio.Add(centroServicio);
                }
                conn.Close();
            }
            return lstCentrosDeServicio;
        }



        /// <summary>
        /// obtiene todos los empleados que sean mensajeros conductores y auxiliares de zona
        /// </summary>
        /// <returns></returns>
        public List<RGEmpleadoDC> ObtenerEmpleadosParaAsignarRecogidas(string idLocalidad)
        {
            List<RGEmpleadoDC> lstEmpleado = new List<RGEmpleadoDC>();
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerEmpleadosParaAsignarRecogidas_REC", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idLocalidad", idLocalidad);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    RGEmpleadoDC empleado = new RGEmpleadoDC
                    {
                        idEmpleado = reader["Identificacion"].ToString(),
                        nombreEmpleado = reader["NombreEmpleado"].ToString(),
                        idTipoIdentificacion = reader["TipoIdentificacion"].ToString(),
                        email = string.IsNullOrEmpty(reader["e_mail"].ToString()) ? "N/A" : reader["e_mail"].ToString(),
                        idCargo = reader["cod_car"].ToString(),
                        descripcionCargo = reader["nom_car"].ToString(),
                        direccion = reader["dir_res"].ToString(),
                        telefonoFijo = reader["tel_res"].ToString(),
                        telefonoCelular = reader["tel_cel"].ToString(),
                        ciudadEmpleado = reader["LOC_Nombre"].ToString(),
                        racolEmpleado = reader["REA_Descripcion"].ToString(),
                        estadoEmpleado = Convert.ToBoolean(reader["estado"])
                    };
                    lstEmpleado.Add(empleado);
                }
                conn.Close();
            }
            return lstEmpleado;
        }


        /// <summary>
        /// obtiene todos los empleados que sean mensajeros conductores y auxiliares de zona
        /// </summary>
        /// <returns></returns>
        public RGEmpleadoDC ObtenerDatosDeEmpleadoPorCedula(string idEmpleado)
        {
            RGEmpleadoDC empleado = new RGEmpleadoDC();
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerEmpleadosParaAsignarRecogidas_REC", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Idusuario", idEmpleado);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    empleado = new RGEmpleadoDC
                    {
                        idEmpleado = reader["Identificacion"].ToString(),
                        nombreEmpleado = reader["NombreEmpleado"].ToString(),
                        idTipoIdentificacion = reader["TipoIdentificacion"].ToString(),
                        email = reader["e_mail"].ToString(),
                        idCargo = reader["cod_car"].ToString(),
                        descripcionCargo = reader["nom_car"].ToString(),
                        direccion = reader["dir_res"].ToString(),
                        telefonoFijo = reader["tel_res"].ToString(),
                        telefonoCelular = reader["tel_cel"].ToString(),
                        ciudadEmpleado = reader["LOC_Nombre"].ToString(),
                        racolEmpleado = reader["REA_Descripcion"].ToString(),
                        estadoEmpleado = Convert.ToBoolean(reader["estado"])
                    };
                }
                conn.Close();
            }
            return empleado;
        }

        /// <summary>
        /// Obtiene las recogidas realizadas de cliente peaton segun el token del dispositivo.
        /// </summary>
        /// <param name="tokenDispositivo"></param>
        /// <returns></returns>
        public List<RGRecogidaEsporadicaDC> ObtenerMisRecogidasClientePeaton(string idUsuario)
        {
            List<RGRecogidaEsporadicaDC> misRecogidas = new List<RGRecogidaEsporadicaDC>();
            using (SqlConnection con = new SqlConnection(CadCnxController))
            {

                con.Open();
                SqlCommand cmd = new SqlCommand("paObtenerMisRecogidasClientePeaton_REC", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdUsuario", idUsuario));
                SqlDataReader lector = cmd.ExecuteReader();

                while (lector.Read())
                {
                    misRecogidas.Add(new RGRecogidaEsporadicaDC
                    {
                        IdSolRecogida = (long)lector["SRE_IdSolRecogida"],
                        FechaHoraRecogida = Convert.ToDateTime(lector["SRE_FechaHoraRecogida"].ToString()),
                        FechaGrabacion = Convert.ToDateTime(lector["SRE_FechaGrabacion"].ToString()),
                        DireccionRecogida = lector["SRP_DireccionRecogida"].ToString(),
                        DescripcionEstado = lector["SRE_DescripcionEstadoSol"].ToString(),
                        Mensajero = lector["Mensajero"].ToString(),
                        IdLocalidad = lector["SRE_IdLocalidadRecogida"].ToString()
                    });
                }
                con.Close();
                con.Dispose();
            }

            return misRecogidas;

        }

        /// <summary>
        /// obtiene la lista de clientes credito
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <returns></returns>
        public List<RGRecogidasDC> ObtenerClientesCredito(string idCol)
        {
            List<RGRecogidasDC> lstClientes = new List<RGRecogidasDC>();
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerClientesCreditoConRecogidasPorCiudad_REC", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdCentroLogistico", idCol);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    RGRecogidasDC cliente = new RGRecogidasDC
                    {
                        idCliente = Convert.ToInt32(reader["SUC_IdSucursal"]),
                        Nombre = reader["SUC_Nombre"].ToString()
                    };
                    lstClientes.Add(cliente);
                }
                conn.Close();
            }
            return lstClientes;
        }

        public RGDetalleSolicitudRecogidaDC obtenerDetalleSolRecogida(long idSolicitudRecogida, RGEnumClaseSolicitud claseSolicitud)
        {
            RGDetalleSolicitudRecogidaDC resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                sqlConn.Open();
                if (claseSolicitud != RGEnumClaseSolicitud.SinSolicitud)
                {
                    var cmd = new SqlCommand(IdentificaClaseSolicitud(claseSolicitud), sqlConn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdSolRecogida", idSolicitudRecogida);
                    var reader = cmd.ExecuteReader();
                    resultado = MapperRepositorio.MapperToDetalleSolicitudRegcogida(reader, claseSolicitud);
                }
            }
            return resultado;
        }

        private void CreaClasesSolicitudes()
        {
            this.ClasesSolicitud = new Dictionary<RGEnumClaseSolicitud, string>();
            ClasesSolicitud.Add(RGEnumClaseSolicitud.Exporadica, "paObtenerDetSolRecogidaExporadica");
            ClasesSolicitud.Add(RGEnumClaseSolicitud.FijaCliente, "paObtenerDetSolRecogidaFijaCliente");
            ClasesSolicitud.Add(RGEnumClaseSolicitud.ExporadicaClienteFijo, "paObtenerDetSolRecogidaExpoCliente");
            ClasesSolicitud.Add(RGEnumClaseSolicitud.FijaCentroServicio, "paObtenerDetSolRecogidaFijaCentroServicio");
            ClasesSolicitud.Add(RGEnumClaseSolicitud.SinSolicitud, "");
        }

        private string IdentificaClaseSolicitud(RGEnumClaseSolicitud claseSolicitud)
        {
            if (ClasesSolicitud.ContainsKey(claseSolicitud))
            {
                return Convert.ToString(ClasesSolicitud[claseSolicitud]);
            }

            return "";
        }


        /// <summary>
        /// Metodo para consultar el estado de la solicitud recogida
        /// </summary>
        /// <param name="solicitud"></param>
        /// <returns></returns>
        public EnumEstadoSolicitudRecogida ObtenerEstadoSolicitudRecogida(RGAsignarRecogidaDC solicitud)
        {
            EnumEstadoSolicitudRecogida resultado;
            using (var conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerEstadoSolicitudRecogida_REC", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdSolicitud", solicitud.IdSolicitudRecogida);
                conn.Open();
                resultado = (EnumEstadoSolicitudRecogida)cmd.ExecuteScalar();
                conn.Close();
            }
            return resultado;
        }



        #region Recogidas Controller App

        /// <summary>
        /// Metodo para obtener las recogidas disponibles por mensajero 
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public List<RGRecogidasDC> ObtenerRecogidasDisponibles(string idLocalidad)
        {
            List<RGRecogidasDC> recogidas = new List<RGRecogidasDC>();
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("paObtRecogidasDisponiblesLocalidadCtrApp_RG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdLocalidad", idLocalidad);
                var reader = cmd.ExecuteReader();
                recogidas = MapperRepositorio.MapperRecogidaDisponible(reader);
                return recogidas;
            }
        }

        /// <summary>
        /// Metodo para obtener las recogidas reservadas 
        /// </summary>
        /// <param name="numIdentificacion"></param>
        /// <returns></returns>
        public List<RGRecogidasDC> ObtenerRecogidasReservadasMensajero(string numIdentificacion)
        {
            List<RGRecogidasDC> recogidas;
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtRecogidasReservadasMensajeroCtrApp_RG", conn);
                cmd.Parameters.AddWithValue("@numIdentificacion", numIdentificacion);
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                var reader = cmd.ExecuteReader();
                recogidas = MapperRepositorio.MapperRecogidasReserva(reader);
                return recogidas;
            }
        }
        /// <summary>
        /// Metodo para obtener las 
        /// </summary>
        /// <param name="numIdentificacion"></param>
        /// <returns></returns>
        public List<RGRecogidasDC> ObtenerRecogidasEfectivasMensajero(string numIdentificacion)
        {
            List<RGRecogidasDC> recogidas;
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtRecogidasEfectivasMensajeroCtrApp_RG", conn);
                cmd.Parameters.AddWithValue("@numIdentificacion", numIdentificacion);
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                var reader = cmd.ExecuteReader();
                recogidas = MapperRepositorio.MapperRecogidasEfectivas(reader);
                return recogidas;
            }
        }
        #endregion

        #region IVR

        public string ObtenerNombreyDireccionCliente(string telefono)
        {
            string correo = string.Empty;
            CLClientesDC cliente = new CLClientesDC();
            PATipoIdentificacion TipoIdentificacion = new PATipoIdentificacion();
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                string validacion = string.Empty;
                SqlCommand cmd = new SqlCommand("paObtenerDatosSolicitudRecogidaEsporadica_CLI", conn);
                if (telefono.Length == 8)
                {
                    string indicativo = telefono.Substring(0, 1);
                    StringBuilder aStringBuilder = new StringBuilder(telefono);
                    aStringBuilder.Remove(0, 1);
                    telefono = aStringBuilder.ToString();
                    cmd.Parameters.AddWithValue("@NumeroTelefonico", telefono);
                    cmd.Parameters.AddWithValue("@Indicativo", indicativo);
                }
                else if (telefono.Length == 10)
                {
                    cmd.Parameters.AddWithValue("@NumeroTelefonico", telefono);
                    cmd.Parameters.AddWithValue("@Indicativo", null);
                }
                else
                {
                    return validacion;
                }

                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    TipoIdentificacion.IdTipoIdentificacion = Convert.ToString(reader["SRP_TipoDocumento"]);
                    cliente.Nit = Convert.ToString(reader["SRP_NumeroDocumento"]);
                    cliente.RazonSocial = Convert.ToString(reader["SRP_NombreCompleto"]);
                    cliente.Direccion = Convert.ToString(reader["SRP_DireccionRecogida"]);
                    cliente.Telefono = Convert.ToString(reader["SRP_NumeroTelefonico"]);
                    cliente.IdCiudad = Convert.ToString(reader["SRE_IdLocalidadRecogida"]);
                    correo = Convert.ToString(reader["SRP_CorreoElectronico"]);
                    correo = String.IsNullOrEmpty(correo) ? "NA" : correo;
                    string longitud = "0", latitud = "0";
                    if (reader["SRE_Longitud"] != System.DBNull.Value)
                    {
                        longitud = Convert.ToString(reader["SRE_Longitud"]);
                    }
                    if (reader["SRE_Latitud"] != System.DBNull.Value)
                    {
                        latitud = Convert.ToString(reader["SRE_Latitud"]);
                    }
                    validacion = string.Join(",", new string[] { TipoIdentificacion.IdTipoIdentificacion, cliente.Nit, cliente.RazonSocial, cliente.Direccion, cliente.Telefono, correo, cliente.IdCiudad, longitud, latitud });
                }
                conn.Close();
                return validacion;
            }
        }

        #endregion


    }

}
