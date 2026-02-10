using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Text.RegularExpressions;
using ServiciosInter.DatosCompartidos.Comun;
using ServiciosInter.DatosCompartidos.EntidadesNegocio.CentrosServicio;
using ServiciosInter.DatosCompartidos.EntidadesNegocio.Mensajeria;
using ServiciosInter.DatosCompartidos.EntidadesNegocio.Parametros;
using ServiciosInter.DatosCompartidos.EntidadesNegocio.Tarifas;

namespace ServiciosInter.Infraestructura.AccesoDatos.Repository.Parametros
{
    public class ParametrosRepository
    {
        private string CadCnxController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;
        private static Dictionary<string, string> ParametrosFrameworkCache = null;
        private static DateTime fechaHoraCacheParametros = DateTime.Now;

        private static readonly ParametrosRepository instancia = new ParametrosRepository();

        public static ParametrosRepository Instancia
        {
            get
            {
                return instancia;
            }
        }

        private ParametrosRepository()
        {
        }

        /// <summary>
        /// Consulta los parametros generales del Framework
        /// </summary>
        /// <param name="llave">Nombre del parametro</param>
        /// <returns>Valor el parametro</returns>
        public string ConsultarParametrosFramework(string llave)
        {
            PrepararParametrosFrameworkCache();

            if (ParametrosRepository.ParametrosFrameworkCache != null)
            {
                if (ParametrosRepository.ParametrosFrameworkCache.ContainsKey(llave))
                {
                    return ParametrosRepository.ParametrosFrameworkCache[llave];
                }
                else
                {
                    /*   ControllerException excepcion = new ControllerException(COConstantesModulos.PARAMETROS_GENERALES,
                                              ETipoErrorFramework.EX_NO_EXISTE_PARAMETRO_CONFIGURADO.ToString(),
                                               MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_NO_EXISTE_PARAMETRO_CONFIGURADO));
                       throw new FaultException<ControllerException>(excepcion);*/
                    throw new FaultException<Exception>(new Exception("No existe el parámetro"));
                }
            }
            else
            {
                using (SqlConnection conn = new SqlConnection(CadCnxController))
                {
                    SqlCommand cmd = new SqlCommand("paObtenerParametrosFramework_PAR", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PAR_IdParametro", llave);
                    conn.Open();
                    var parametro = cmd.ExecuteScalar();
                    conn.Close();

                    if (parametro != null)
                    {
                        return Convert.ToString(parametro);
                    }
                    else
                    {
                        /* ControllerException excepcion = new ControllerException(COConstantesModulos.PARAMETROS_GENERALES,
                                             ETipoErrorFramework.EX_NO_EXISTE_PARAMETRO_CONFIGURADO.ToString(),
                                              MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_NO_EXISTE_PARAMETRO_CONFIGURADO));
                         throw new FaultException<ControllerException>(excepcion);
                         */
                        throw new FaultException<Exception>(new Exception("No existe el parámetro"));
                    }
                }
            }
        }

        /// <summary>
        /// Levanta diccionario a memoria con todos los parametrosFramework
        /// </summary>
        private void PrepararParametrosFrameworkCache()
        {
            ///Contrala que cada 10 minutos se refresque la cache
            if (Math.Abs((ParametrosRepository.fechaHoraCacheParametros - DateTime.Now).TotalMinutes) > 10)
            {
                lock (this)
                {
                    if (ParametrosRepository.ParametrosFrameworkCache != null)
                    {
                        ParametrosRepository.ParametrosFrameworkCache = null;
                    }
                }
            }

            if (ParametrosRepository.ParametrosFrameworkCache == null)
            {
                lock (this)
                {
                    if (ParametrosRepository.ParametrosFrameworkCache == null)
                    {
                        try
                        {
                            ParametrosRepository.ParametrosFrameworkCache = new Dictionary<string, string>();
                            ParametrosRepository.fechaHoraCacheParametros = DateTime.Now;
                            using (SqlConnection conn = new SqlConnection(CadCnxController))
                            {
                                SqlCommand cmd = new SqlCommand("paObtenerTodosParametrosFramework_PAR", conn);
                                cmd.CommandType = CommandType.StoredProcedure;
                                conn.Open();
                                SqlDataReader reader = cmd.ExecuteReader();

                                while (reader.Read())
                                {
                                    ParametrosRepository.ParametrosFrameworkCache.Add(reader["PAR_IdParametro"].ToString().Trim(), reader["PAR_ValorParametro"].ToString().Trim());
                                }
                                conn.Close();
                            }
                        }
                        catch
                        {
                            ParametrosRepository.ParametrosFrameworkCache = null;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Retorna la lista de localidad que no son países ni departamentos.
        /// </summary>
        /// <param name="idTipoPais"></param>
        /// <param name="idTipoDepartamento"></param>
        /// <param name="idLocalidPais"></param>
        /// <returns></returns>
        public List<PALocalidadDC> ObtenerMunicipiosCorregimientoInspeccionCaserioPais(int idTipoPais, int idTipoDepartamento, string idPais)
        {
            string tipoPais = idTipoPais.ToString();
            string tipoDepartamento = idTipoDepartamento.ToString();
            string _idTipoPais = ((int)PAEnumTipoLocalidad.PAIS).ToString();
            string _idTipoDepartamento = ((int)PAEnumTipoLocalidad.DEPARTAMENTO).ToString();
            string _idTipoMunicipio = ((int)PAEnumTipoLocalidad.MUNICIPIO).ToString();
            string _idTipoCorregimiento = ((int)PAEnumTipoLocalidad.CORREGIMIENTO).ToString();
            string _idTipoCaserio = ((int)PAEnumTipoLocalidad.CASERIO).ToString();
            string _idTipoInspeccion = ((int)PAEnumTipoLocalidad.INSPECCION).ToString();

            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerTodasLocalidadesAncestros_PAR", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                conn.Open();
                da.Fill(dt);
                conn.Close();

                return dt.AsEnumerable().Where(r => r.Field<string>("LOC_IdTipo") != tipoPais && r.Field<string>("LOC_IdTipo") != tipoDepartamento &&
                      ((r.Field<string>("LOC_IdTipo") == _idTipoDepartamento && r.Field<string>("LOC_IdAncestroPrimerGrado") == idPais) ||
                                r.Field<string>("LOC_IdTipo") == _idTipoMunicipio && r.Field<string>("LOC_IdAncestroSegundoGrado") == idPais ||
                               (r.Field<string>("LOC_IdTipo") == _idTipoCorregimiento || r.Field<string>("LOC_IdTipo") == _idTipoInspeccion
                                 || r.Field<string>("LOC_IdTipo") == _idTipoCaserio && r.Field<string>("LOC_IdAncestroTercerGrado") == idPais)))
                            .ToList().ConvertAll<PALocalidadDC>(localidad => new PALocalidadDC
                            {
                                Nombre = localidad.Field<string>("NombreCompleto"),
                                IdLocalidad = localidad.Field<string>("LOC_IdLocalidad")
                            }).ToList();
            }
        }

        /// <summary>
        /// Obtiene el nombre de un servicio seleccionado
        /// </summary>
        /// <param name="idServicio"></param>
        /// <returns></returns>
        public TAServicioDC ObtenerNombreServicioPorIdServicio(int idServicio)
        {
            TAServicioDC nombreServicio = new TAServicioDC();

            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerNombreServicioPorIdServicio_PUA", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdServicio", idServicio);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while(reader.Read())
                {
                    nombreServicio.Nombre = reader["SER_Nombre"] == null ? string.Empty : reader["SER_Nombre"].ToString();
                    nombreServicio.Descripcion = reader["SER_Descripcion"] == null ? string.Empty : reader["SER_Descripcion"].ToString();
                    nombreServicio.UnidadNegocio = reader["SER_IdUnidadNegocio"] == null ? string.Empty : reader["SER_IdUnidadNegocio"].ToString();
                }
            }

            return nombreServicio;
        }

        /// <summary>
        /// Obtener la agencia a partir de la localidad
        /// </summary>
        ///// <param name="localidad"></param>
        public PUCentroServiciosDC ObtenerAgenciaLocalidad(string localidad)
        {
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerAgenciaLocalidad_PUA", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdLocalidad", localidad);

                SqlParameter paramOut = new SqlParameter("@NombreLocalidad", SqlDbType.VarChar, 100);
                paramOut.Direction = ParameterDirection.Output;
                paramOut.IsNullable = true;
                cmd.Parameters.Add(paramOut);
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                conn.Open();
                da.Fill(dt);
                conn.Close();

                var dr = dt.AsEnumerable().FirstOrDefault();

                switch (dr.Field<string>("Tipo"))
                {
                    case "NOEXISTE":
                        /*  ControllerException excepcion = new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, EnumTipoErrorCentroServicios.EX_LOCALIDAD_SIN_AGENCIAS.ToString(), string.Format(MensajesCentroServicios.CargarMensaje(EnumTipoErrorCentroServicios.EX_LOCALIDAD_SIN_AGENCIAS), paramOut.Value.ToString()));
                          throw new FaultException<ControllerException>(excepcion);*/
                        throw new FaultException<Exception>(new Exception("La localidad no tiene agencia."));
                        break;

                    case "AGE":

                        return new PUCentroServiciosDC()
                        {
                            IdCentroServicio = dr.Field<long>("AGE_IdAgencia"),
                            Nombre = dr.Field<string>("CES_Nombre"),
                            IdMunicipio = dr.Field<string>("LOC_IdLocalidad"),
                            NombreMunicipio = dr.Field<string>("LOC_Nombre"),
                            Telefono1 = dr.Field<string>("CES_Telefono1"),
                            Direccion = dr.Field<string>("CES_Direccion"),
                            Tipo = dr.Field<string>("CES_Tipo"),
                            TipoSubtipo = dr.Field<string>("AGE_IdTipoAgencia"),
                            Sistematizado = dr.Field<bool>("CES_Sistematizada")
                        };

                        break;

                    case "COL":
                        return new PUCentroServiciosDC()
                        {
                            IdCentroServicio = dr.Field<long>("MCL_IdCentroLogistico"),
                            Nombre = dr.Field<string>("CES_Nombre"),
                            IdMunicipio = dr.Field<string>("MCL_IdLocalidad"),
                            NombreMunicipio = dr.Field<string>("LOC_Nombre"),
                            Telefono1 = dr.Field<string>("CES_Telefono1"),
                            Direccion = dr.Field<string>("CES_Direccion"),
                            Tipo = dr.Field<string>("CES_Tipo"),
                            Sistematizado = dr.Field<bool>("CES_Sistematizada")
                        };

                        break;

                    default:
                        throw new FaultException<Exception>(new Exception("La localidad no tiene agencia."));
                }

                //throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, EnumTipoErrorCentroServicios.EX_LOCALIDAD_SIN_AGENCIAS.ToString(), string.Format(MensajesCentroServicios.CargarMensaje(EnumTipoErrorCentroServicios.EX_LOCALIDAD_SIN_AGENCIAS), "DESCONOCIDO")));
            }
        }

        /// <summary>
        /// Valida si un municipio permite forma de pago alcobro.
        /// </summary>
        /// <param name="idMunicipio"></param>
        /// <returns> true => Si tiene forma de pago alcobro,    false => No tiene forma de pago alcobro </returns>
        public bool ValidarMunicipioPermiteAlcobro(string idMunicipio)
        {
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paValidarMunicipioSinAlcobro_PUA", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdLocalidad", idMunicipio);
                conn.Open();
                bool rst = Convert.ToInt32(cmd.ExecuteScalar()) > 0 ? false : true;
                conn.Close();
                conn.Dispose();
                return rst;
            }
        }

        /// <summary>
        /// Retorna la lista del horario de determinado centro de servicio
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public List<TAHorarioRecogidaCsvDC> ObtenerHorarioRecogidaDeCsv(long idCentroServicio)
        {
            using (SqlConnection cnx = new SqlConnection(CadCnxController))
            {
                List<TAHorarioRecogidaCsvDC> horario = new List<TAHorarioRecogidaCsvDC>();
                cnx.Open();
                SqlCommand cmd = new SqlCommand("paObtenerHorarioRecogidasCSV_TAR", cnx);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@idcentroServicios", idCentroServicio));
                SqlDataReader lector = cmd.ExecuteReader();
                while (lector.Read())
                {
                    horario.Add(new TAHorarioRecogidaCsvDC
                    {
                        DiaDeLaSemana = int.Parse(lector["HRC_Dia"].ToString()),
                        HoraRecogida = Convert.ToDateTime(lector["HRC_Hora"].ToString())
                    });
                }
                return horario;
            }
        }

        /// <summary>
        /// Retorna el número de dias del trayecto si existe
        /// </summary>
        /// <param name="municipioOrigen">Municipio de Origen</param>
        /// <param name="municipioDestino">Municipio de Destino</param>
        /// <param name="servicio">Servicio</param>
        /// <returns>Duración en días</returns>
        public TATiempoDigitalizacionArchivo ValidarServicioTrayectoDestino(PALocalidadDC municipioOrigen, PALocalidadDC municipioDestino, TAServicioDC servicio, decimal peso)
        {
            TATiempoDigitalizacionArchivo tiempos = new TATiempoDigitalizacionArchivo();
            SqlCommand cmd = null;
            TAServicioDC Servicio = ObtenerUnidadNegocio(servicio.IdServicio);

            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                if (Servicio.IdUnidadNegocio == TAConstantesServicios.UNIDAD_MENSAJERIA && peso >= 3 && servicio.IdServicio != TAConstantesServicios.SERVICIO_RAPI_HOY && servicio.IdServicio != TAConstantesServicios.SERVICIO_RAPI_AM)
                {
                    cmd = new SqlCommand("paValidarDiasAdicionalesTrayectoDestino_TAR", conn);
                }
                else
                {
                    cmd = new SqlCommand("paValidarServicioTrayectoDestino_TAR", conn);
                    cmd.Parameters.AddWithValue("@IdServicio", servicio.IdServicio);
                }
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdLocalidadOrigen", municipioOrigen.IdLocalidad);
                cmd.Parameters.AddWithValue("@IdLocalidadDestino", municipioDestino.IdLocalidad);
                
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    tiempos.numeroDiasArchivo = Convert.ToDouble(reader["STR_TiempoArchivo"]);
                    tiempos.numeroDiasDigitalizacion = Convert.ToDouble(reader["STR_TiempoDigitalizacion"]);
                    tiempos.numeroDiasEntrega = Convert.ToInt32(reader["STR_TiempoEntrega"]);
                }
                conn.Close();
            }
            return tiempos;
        }

        /// <summary>
        /// Retorna el número de dias del trayecto si existe
        /// </summary>
        /// <param name="municipioOrigen">Municipio de Origen</param>
        /// <param name="municipioDestino">Municipio de Destino</param>
        /// <param name="servicio">Servicio</param>
        /// <param name="idListaPrecios">Lista de precios</param>
        /// <returns>Duración en días</returns>
        public TATiempoDigitalizacionArchivo ValidarServicioTrayectoDestinoExcepcion(PALocalidadDC municipioOrigen, PALocalidadDC municipioDestino, TAServicioDC servicio, decimal peso, int idListaPrecios)
        {
            TATiempoDigitalizacionArchivo tiempos = new TATiempoDigitalizacionArchivo();
            SqlCommand cmd = null;
            TAServicioDC Servicio = ObtenerUnidadNegocio(servicio.IdServicio);

            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                if (Servicio.IdUnidadNegocio == TAConstantesServicios.UNIDAD_MENSAJERIA && peso >= 3 && servicio.IdServicio != TAConstantesServicios.SERVICIO_RAPI_HOY && servicio.IdServicio != TAConstantesServicios.SERVICIO_RAPI_AM)
                {
                    cmd = new SqlCommand("paValidarDiasAdicionalesTrayectoDestino_TAR", conn);
                }
                else
                {
                    cmd = new SqlCommand("paValidarServicioTrayectoDestinoExcepcion_TAR", conn);
                    cmd.Parameters.AddWithValue("@IdServicio", servicio.IdServicio);
                    cmd.Parameters.AddWithValue("@IdListaPrecios", idListaPrecios);
                }
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdLocalidadOrigen", municipioOrigen.IdLocalidad);
                cmd.Parameters.AddWithValue("@IdLocalidadDestino", municipioDestino.IdLocalidad);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    tiempos.numeroDiasArchivo = Convert.ToDouble(reader["STR_TiempoArchivo"]);
                    tiempos.numeroDiasDigitalizacion = Convert.ToDouble(reader["STR_TiempoDigitalizacion"]);
                    tiempos.numeroDiasEntrega = Convert.ToInt32(reader["STR_TiempoEntrega"]);
                }
                conn.Close();
            }
            return tiempos;
        }

        /// <summary>
        /// Obtiene
        /// </summary>
        /// <param name="municipioOrigen"></param>
        /// <param name="municipioDestino"></param>
        /// <returns></returns>
        public int ObtenerIdServicioDeMayorTiempoEntrega(PALocalidadDC municipioOrigen, PALocalidadDC municipioDestino)
        {
 
                int idServicioMaxtiempo = 0;
                using (SqlConnection conn = new SqlConnection(CadCnxController))
                {
                    SqlCommand cmd = new SqlCommand("paObtenerIdServicioConMaximoTiempoEntrega", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdLocalidadOrigen", municipioOrigen.IdLocalidad);
                    cmd.Parameters.AddWithValue("@IdLocalidadDestino", municipioDestino.IdLocalidad);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            idServicioMaxtiempo = Convert.ToInt32(reader["STR_IdServicio"]);
                        }
                    }
                    conn.Close();
                }
                return idServicioMaxtiempo;
        }

        private TAServicioDC ObtenerUnidadNegocio(int idServicio)
        {
            TAServicioDC servicio = null;

            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerUnidadServicio_TAR", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdServicio", idServicio);
                conn.Open();
                SqlDataReader lector = cmd.ExecuteReader();
                while (lector.Read())
                {
                    servicio = new TAServicioDC()
                    {
                        IdUnidadNegocio = lector["UNE_IdUnidad"].ToString()

                    };
                }
                conn.Close();
            }
            return servicio;
        }

        /// <summary>
        /// Método para obtener los agregar los dias laborales entre dos fechas
        /// </summary>
        /// <param name="fechaOriginal"></param>
        /// <param name="diasLaborables"></param>
        /// <returns></returns>
        public int ConsultarDiasLaborales(DateTime fechaInicial, DateTime fechaFinal)
        {
            int diasRetorna;
            int diasLaborables = fechaFinal.Subtract(fechaInicial).Days;
            List<DateTime> festivos = ObtenerFestivosDesdeFecha(fechaInicial.Date);
            if (fechaInicial < fechaFinal)
            {
                diasRetorna = diasLaborables;
                DateTime fechaTemporal = fechaInicial.Date;
                while (diasLaborables > 0)
                {
                    fechaTemporal = fechaTemporal.AddDays(1);
                    if (fechaTemporal.DayOfWeek == DayOfWeek.Saturday || fechaTemporal.DayOfWeek == DayOfWeek.Sunday || festivos.Contains(fechaTemporal))
                    {
                        diasRetorna--;
                    }
                    diasLaborables--;
                }
                return diasRetorna;
            }
            else
            {
                diasRetorna = diasLaborables;
                DateTime fechaTemporal = fechaInicial.Date;
                while (diasLaborables < 0)
                {
                    fechaTemporal = fechaTemporal.AddDays(1);
                    if (fechaTemporal.DayOfWeek == DayOfWeek.Saturday || fechaTemporal.DayOfWeek == DayOfWeek.Sunday || festivos.Contains(fechaTemporal))
                    {
                        diasRetorna++;
                    }
                    diasLaborables++;
                }
                return diasRetorna;
            }
        }

        /// <summary>
        /// Obtiene lista de festivos desde la fecha de parametro
        /// </summary>
        /// <param name="fechadesde"></param>
        /// <returns></returns>
        public List<DateTime> ObtenerFestivosDesdeFecha(DateTime fechadesde)
        {
            List<DateTime> lstFechas = new List<DateTime>();

            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerFestivosDesde_PAR", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idPais", ConstantesFramework.ID_LOCALIDAD_COLOMBIA);
                cmd.Parameters.AddWithValue("@fechaDesde", DateTime.Now.Date);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    lstFechas.Add(Convert.ToDateTime(reader["CAL_Fecha"]));
                }
                conn.Close();
            }
            return lstFechas;
        }

        /// <summary>
        /// Retorna Validacion si el Servicio-Origen-Destino, debe etiquetarse como AEREO en el campo del casillero de la Guia
        /// </summary>
        /// <param name="municipioOrigen"></param>
        /// <param name="municipioDestino"></param>
        /// <param name="idServicio"></param>
        /// <returns></returns>
        public bool ValidarServicioTrayectoCasilleroAereo(string municipioOrigen, string municipioDestino, int idServicio)
        {
            bool rta = false;

            using (SqlConnection cnx = new SqlConnection(CadCnxController))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand("paValidarServicioTrayectoCasilleroAereo_TAR", cnx);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@municipioOrigen", municipioOrigen));
                cmd.Parameters.Add(new SqlParameter("@municipioDestino", municipioDestino));
                cmd.Parameters.Add(new SqlParameter("@idServicio", idServicio));

                Int32 num = (Int32)cmd.ExecuteScalar();
                if (num == 1)
                    rta = true;
            }

            return rta;
        }

        /// <summary>
        /// Obtiene lista con los dias no habiles
        /// </summary>
        /// <returns></returns>
        public List<DateTime> ObtenerDiasNoHabiles()
        {            
            List<DateTime> lstDias = new List<DateTime>();
            DateTime fechaInicial = DateTime.ParseExact("01/01/" + DateTime.Now.Year, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime fechaFinal = DateTime.ParseExact("31/12/" + DateTime.Now.Year, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerDiasNoHabiles_PAR", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FechaInicio", fechaInicial);
                cmd.Parameters.AddWithValue("@FechaFin", fechaFinal);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    DateTime diaNoHabil = new DateTime();
                    diaNoHabil = Convert.ToDateTime(reader["DNH_Fecha"]);
                    lstDias.Add(diaNoHabil);
                }
                conn.Close();
            }
            return lstDias;
        }

        /// <summary>
        /// Actualiza la informació de validación de dos centros de servicios implicados en un trayecto
        /// </summary>
        /// <param name="localidadDestino">Ciudad o municipio de destino del envío</param>
        /// <param name="validacion">Validación del trayecto</param>
        /// <param name="idCentroServicio">Id del centro de servicios de origen de la transacción</param>
        /// <param name="localidadOrigen">si no se tiene el id centro de servicio origen el metodo lo busca a través de la localidad original</param>
        public void ObtenerInformacionValidacionTrayecto(PALocalidadDC localidadDestino, ADValidacionServicioTrayectoDestino validacion, long idCentroServicio, PALocalidadDC localidadOrigen = null)
        {
            using (var sqlconn = new SqlConnection(CadCnxController))
            {
                sqlconn.Open();
                var cmd = new SqlCommand("paObtInfoAgenciasTrayecto_PUA ", sqlconn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@localidadOrigen", localidadOrigen.IdLocalidad);
                cmd.Parameters.AddWithValue("@localidadDestino", localidadDestino.IdLocalidad);
                var resul = cmd.ExecuteReader();

                if (resul.HasRows)
                {
                    if (resul.Read())
                    {
                        if (DBNull.Value.Equals(resul["CES_IdCentroServiciosOri"]))

                        {
                            //throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, EnumTipoErrorCentroServicios.EX_LOCALIDAD_SIN_AGENCIAS.ToString(), MensajesCentroServicios.CargarMensaje(EnumTipoErrorCentroServicios.EX_LOCALIDAD_SIN_AGENCIAS)));
                            throw new FaultException<Exception>(new Exception("Localidad sin agencia."));
                        }
                        else
                        {
                            validacion.PesoMaximoTrayectoOrigen = Convert.ToDecimal(resul["CES_PesoMaximoOri"]);
                            validacion.IdCentroServiciosOrigen = Convert.ToInt64(resul["CES_IdCentroServiciosOri"]);
                            validacion.NombreCentroServiciosOrigen = Convert.ToString(resul["CES_IdCentroServiciosOri"]);
                            validacion.VolumenMaximoOrigen = Convert.ToDecimal(resul["CES_IdCentroServiciosOri"]);
                        }

                        if (DBNull.Value.Equals(resul["CES_IdCentroServiciosDes"]))

                        {
                            //throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, EnumTipoErrorCentroServicios.EX_TRAYECTO_NO_VALIDO.ToString(), MensajesCentroServicios.CargarMensaje(EnumTipoErrorCentroServicios.EX_TRAYECTO_NO_VALIDO)));
                            throw new FaultException<Exception>(new Exception("Trayecto no válido."));
                        }
                        else
                        {
                            validacion.PesoMaximoTrayectoDestino = Convert.ToDecimal(resul["CES_PesoMaximoDes"]);
                            validacion.VolumenMaximoDestino = Convert.ToDecimal(resul["CES_VolumenMaximoDes"]);
                            validacion.IdCentroServiciosDestino = Convert.ToInt64(resul["CES_IdCentroServiciosDes"]);
                            validacion.DestinoAdmiteFormaPagoAlCobro = Convert.ToBoolean(resul["CES_AdmiteFormaPagoAlCobroDes"]);
                            validacion.NombreCentroServiciosDestino = Convert.ToString(resul["CES_NombreDes"]);
                            validacion.CodigoPostalDestino = Convert.ToString(resul["LOC_CodigoPostalDes"]);
                            validacion.DireccionCentroServiciosDestino = Convert.ToString(resul["CES_DireccionDes"]);
                            validacion.TelefonoCentroServiciosDestino = Convert.ToString(resul["CES_Telefono1Des"]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Obtener información de validación del trayecto
        /// </summary>
        /// <param name="localidadOrigen"></param>
        /// <param name="idCentroServicioOrigen"></param>
        public void ObtenerInformacionValidacionTrayectoOrigen(PALocalidadDC localidadOrigen, ADValidacionServicioTrayectoDestino validacion)
        {
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtInfoAgenciasTrayecto_PUA", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@localidadOrigen", localidadOrigen.IdLocalidad);
                cmd.Parameters.AddWithValue("@localidadDestino", validacion.CodigoPostalDestino);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (!reader.HasRows)
                {
                    //throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, EnumTipoErrorCentroServicios.EX_LOCALIDAD_SIN_AGENCIAS.ToString(), MensajesCentroServicios.CargarMensaje(EnumTipoErrorCentroServicios.EX_LOCALIDAD_SIN_AGENCIAS)));
                    throw new FaultException<Exception>(new Exception("Localidad sin agencias"));
                }

                while (reader.Read())
                {
                    validacion.PesoMaximoTrayectoOrigen = Convert.ToDecimal(reader["CES_PesoMaximoOri"]);
                    validacion.IdCentroServiciosOrigen = Convert.ToInt64(reader["CES_IdCentroServiciosOri"]);
                    validacion.NombreCentroServiciosOrigen = reader["CES_NombreOri"].ToString();
                    validacion.VolumenMaximoOrigen = Convert.ToDecimal(reader["CES_VolumenMaximoOri"]);
                    validacion.PesoMaximoTrayectoDestino = Convert.ToDecimal(reader["CES_PesoMaximoOri"]);
                    validacion.IdCentroServiciosDestino = Convert.ToInt64(reader["CES_IdCentroServiciosDes"]);
                    validacion.NombreCentroServiciosDestino = reader["CES_NombreDes"].ToString();
                    validacion.VolumenMaximoDestino = Convert.ToDecimal(reader["CES_VolumenMaximoDes"]);
                    validacion.DestinoAdmiteFormaPagoAlCobro = false;
                    validacion.DireccionCentroServiciosDestino = reader["CES_DireccionDes"].ToString();
                    validacion.TelefonoCentroServiciosDestino = reader["CES_Telefono1Des"].ToString();
                }
            }
        }
        /// <summary>
        /// Obtiene el id del contrato
        /// </summary>
        /// <param name="idCLiente"></param>
        /// <returns></returns>
        /// Método temporal
        public PAContratoCliente ObtenerContratoCliente(int idCLiente, int idcontrato = 0)
        {
            PAContratoCliente contrato = null;
            using (SqlConnection cnx = new SqlConnection(CadCnxController))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand("paObtenerServiciosContratoCliente_TAR", cnx);
                cmd.Parameters.AddWithValue("@IdCliente", idCLiente.ToString().Trim());
                cmd.Parameters.AddWithValue("@Fecha", DateTime.Now.Date);
                if(idcontrato != 0)
                {
                    cmd.Parameters.AddWithValue("@IdContrato", idcontrato);
                }
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                SqlDataReader lector = cmd.ExecuteReader();
                while (lector.Read())
                {
                    contrato = new PAContratoCliente()
                    {
                        IdCliente = Int32.Parse(lector[0].ToString()),
                        IdListaPrecios = Int32.Parse(lector[1].ToString())
                    };
                }
            }
            return contrato;
        }

        /// <summary>
        /// Obtiene los Servicios de la DB
        /// </summary>
        /// <returns>Lista con los servicios de la DB</returns>
        public List<TAServicioDC> ObtenerServicios()
        {
            using (SqlConnection cnx = new SqlConnection(CadCnxController))
            {
                List<TAServicioDC> servicios = new List<TAServicioDC>();
                cnx.Open();
                SqlCommand cmd = new SqlCommand("paObtenerServicios_TAR", cnx);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                SqlDataReader lector = cmd.ExecuteReader();
                while (lector.Read())
                {
                    TAServicioDC svc = new TAServicioDC
                    {
                        IdServicio = int.Parse(lector["SER_IdServicio"].ToString()),
                        Nombre = lector["SER_Nombre"].ToString(),
                        Descripcion = lector["SER_Descripcion"].ToString(),
                        UnidadNegocio = lector["UNE_IdUnidad"].ToString(),
                        IdConceptoCaja = int.Parse(lector["SER_IdConceptoCaja"].ToString()),
                        TiempoEntrega = 0
                    };
                    if (lector["HSM_HoraFinal"] != null && DateTime.TryParse(lector["HSM_HoraFinal"].ToString(),out DateTime tempTime))
                    {
                        svc.HoraFin = tempTime;
                    }
                    if (lector["HSM_HoraInicial"] != null && DateTime.TryParse(lector["HSM_HoraInicial"].ToString(), out tempTime))
                    {
                        svc.HoraInicio = tempTime;
                    }
                    if (lector["HSM_AplicaTodoDia"] != null && bool.TryParse(lector["HSM_AplicaTodoDia"].ToString(), out bool tempBool))
                    {
                        svc.AplicaTodoElDia = tempBool;
                    }

                    servicios.Add(svc);
                }
                return servicios;
            }
        }

        /// <summary>
        /// Obtiene los servicios asociados al contrato de la DB
        /// </summary>
        /// <param name="idContrato">Identificador del contrato</param>
        /// <returns>Lista de servicios</returns>
        public List<TAServicioDC> ObtenerServiciosContratoCliente(int idContrato )
        {
            using (SqlConnection cnx = new SqlConnection(CadCnxController))
            {
                List<TAServicioDC> servicios = new List<TAServicioDC>();
                cnx.Open();
                SqlCommand cmd = new SqlCommand("paObtenerServiciosContrato_TAR", cnx);
                cmd.Parameters.AddWithValue("@IdContrato", idContrato);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                SqlDataReader lector = cmd.ExecuteReader();
                while (lector.Read())
                {
                    servicios.Add(new TAServicioDC
                    {
                        IdServicio = int.Parse(lector["SER_IdServicio"].ToString()),
                        Nombre = lector["SER_Nombre"].ToString(),
                        Descripcion = lector["SER_Descripcion"].ToString(),
                        UnidadNegocio = lector["SER_IdUnidadNegocio"].ToString(),
                        IdConceptoCaja = int.Parse(lector["SER_IdConceptoCaja"].ToString()),
                        TiempoEntrega = 0
                    });
                }
                return servicios;
            }
        }

        /// <summary>
        /// Verifica si por ese contrato tiene el servicio asignado
        /// </summary>
        /// <param name="idContrato"></param>
        /// <param name="idServicio"></param>
        /// <returns>Retorna 1 si el cliente tiene el servicio asignado y 0 si no</returns>
        public bool ValidarServicioContratoCliente(int idContrato, int idServicio)
        {
            using (SqlConnection cnx = new SqlConnection(CadCnxController))
            {
                TAServicioDC servicio = new TAServicioDC();
                cnx.Open();
                SqlCommand cmd = new SqlCommand("paValidarServicioContratoCredito_TAR", cnx);
                cmd.Parameters.AddWithValue("@IdContrato", idContrato);
                cmd.Parameters.AddWithValue("@IdServicio", idServicio);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                bool tieneServicio = Convert.ToInt32(cmd.ExecuteScalar()) > 0 ? true : false;
                cnx.Close();
                cnx.Dispose();
                return tieneServicio;
            }
        }


        /// <summary>
        /// Obtiene toda la info basica de los centros de servicio
        /// </summary>
        /// <returns></returns>
        public List<PUCentroServicioInfoGeneral> ObtenerInformacionGeneralCentrosServicio()
        {
            var listaCentrosServicios = new List<PUCentroServicioInfoGeneral>();
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerInfoBasicaCentrosServicio_PUA", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                conn.Open();
                da.Fill(dt);
                conn.Close();

                listaCentrosServicios = dt.AsEnumerable().ToList().ConvertAll<PUCentroServicioInfoGeneral>(r =>
                {
                    PUCentroServicioInfoGeneral csGeneral = new PUCentroServicioInfoGeneral()
                    {
                        IdLocalidad = r.Field<string>("LOC_IdLocalidad"),
                        IdCentroServicios = r.Field<long>("CES_IdCentroServicios"),
                        Barrio = r.Field<string>("CES_Barrio"),
                        DireccionCentroServicio = r.Field<string>("CES_Direccion"),
                        Latitud = r["CES_Latitud"] != DBNull.Value ? r.Field<decimal>("CES_Latitud") : 0,
                        Longitud = r["CES_Longitud"] != DBNull.Value ? r.Field<decimal>("CES_Longitud") : 0,
                        LocalidadNombre = r.Field<string>("LOC_Nombre"),
                        LocalidadNombreCompleto = r.Field<string>("NombreCompleto"),
                        NombreCentroServicio = r.Field<string>("CES_Nombre"),
                        Telefono1 = r.Field<string>("CES_Telefono1"),
                        Telefono2 = r.Field<string>("CES_Telefono2"),
                        Tipo = r.Field<string>("CES_Tipo"),
                        EsReclameEnOficina = r.Field<int>("ReclameEnOficina") == 1
                    };

                    return csGeneral;
                });
            }

            var horariosCentrosServicio = ObtenerHorariosCentrosServicios();

            listaCentrosServicios.ForEach(item =>
            {
                item.HorariosCentroServicios = horariosCentrosServicio.ToList().FindAll(x => x.IdLocalidad == item.IdLocalidad && x.IdCentroServicios == item.IdCentroServicios);
            });

            return listaCentrosServicios;
        }

        public List<PUCentroServicioInfoGeneral> ObtenerInformacionGeneralCentrosServicioAPP()
        {
            var listaCentrosServicios = new List<PUCentroServicioInfoGeneral>();
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerInfoBasicaCentrosServicio_PUA", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                conn.Open();
                da.Fill(dt);
                conn.Close();

                listaCentrosServicios = dt.AsEnumerable().ToList().ConvertAll<PUCentroServicioInfoGeneral>(r =>
                {
                    PUCentroServicioInfoGeneral csGeneral = new PUCentroServicioInfoGeneral()
                    {
                        IdLocalidad = r.Field<string>("LOC_IdLocalidad"),
                        IdCentroServicios = r.Field<long>("CES_IdCentroServicios"),
                        Barrio = r.Field<string>("CES_Barrio"),
                        DireccionCentroServicio = r.Field<string>("CES_Direccion"),
                        Latitud = r["CES_Latitud"] != DBNull.Value ? r.Field<decimal>("CES_Latitud") : 0,
                        Longitud = r["CES_Longitud"] != DBNull.Value ? r.Field<decimal>("CES_Longitud") : 0,
                        LocalidadNombre = r.Field<string>("LOC_Nombre"),
                        LocalidadNombreCompleto = r.Field<string>("NombreCompleto"),
                        NombreCentroServicio = r.Field<string>("CES_Nombre"),
                        Telefono1 = r.Field<string>("CES_Telefono1"),
                        Telefono2 = r.Field<string>("CES_Telefono2"),
                        Tipo = r.Field<string>("CES_Tipo"),
                        EsReclameEnOficina = r.Field<int>("ReclameEnOficina") == 1
                    };

                    return csGeneral;
                });
            }           

            return listaCentrosServicios;
        }

        public List<string> ObtenerHorariosCentroServicioAppRecogidas(long idCentroServicio)
        {
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerHorarioCentroServicioAppRecogidas_PUA", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@HCS_IdCentroServicios", idCentroServicio);
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                List<string> horarios = new List<string>();
                while (reader.Read())
                {

                    horarios.Add(reader["Horario"].ToString());
                }
                conn.Close();

                return horarios;
            }
        }

        /// <summary>
        /// Obtiene los horaricos de los centros de servicio
        /// </summary>
        /// <returns></returns>
        private List<PUHorariosCentroServicios> ObtenerHorariosCentrosServicios()
        {
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerHorariosCentrosServicio", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                conn.Open();
                da.Fill(dt);
                conn.Close();

                return dt.AsEnumerable().ToList().ConvertAll<PUHorariosCentroServicios>(r =>
                {
                    PUHorariosCentroServicios csGeneral = new PUHorariosCentroServicios()
                    {
                        IdLocalidad = !r.IsNull("LOC_IdLocalidad") ? r.Field<string>("LOC_IdLocalidad") : string.Empty,
                        IdCentroServicios = !r.IsNull("HCS_IdCentroServicios") ? r.Field<long>("HCS_IdCentroServicios") : 0,
                        IdHorarioCentroServicios = !r.IsNull("HCS_IdHorarioCentroServicio") ? r.Field<int>("HCS_IdHorarioCentroServicio") : 0,
                        IdDia = !r.IsNull("HCS_IdDia") ? r.Field<string>("HCS_IdDia") : string.Empty,
                        NombreDia = !r.IsNull("HCS_NombreDia") ? r.Field<string>("HCS_NombreDia") : string.Empty,
                        HoraInicio = !r.IsNull("HCS_HoraInicio") ? r.Field<DateTime>("HCS_HoraInicio") : new DateTime(),
                        HoraFin = !r.IsNull("HCS_HoraFin") ? r.Field<DateTime>("HCS_HoraFin") : new DateTime(),
                    };
                    return csGeneral;
                });
            }
        }

        /// <summary>
        /// Obtiene los servicios junto con sus horarios de venta de un centro de servicio
        /// </summary>
        /// <param name="idServicio"></param>
        /// <returns></returns>
        public List<PUServicio> ObtenerServiciosCentroServicio(long idCentroServicio)
        {
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerServiciosHorariosCentrosServicio_PUA", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdCentroServicios", idCentroServicio);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                conn.Open();
                da.Fill(dt);
                conn.Close();

                return dt.AsEnumerable().ToList().GroupBy(r => r.Field<int>("SER_IdServicio"))
                    .Select(s => s.First()).ToList().ConvertAll<PUServicio>(r =>
                    {
                        PUServicio servicio = new PUServicio()
                        {
                            NombreServicio = r.Field<string>("SER_Nombre"),
                            IdServicio = r.Field<int>("SER_IdServicio")
                        };

                        var detalleCentroServicioServicio = dt.AsEnumerable().Where(d => d.Field<int>("SER_IdServicio") == r.Field<int>("SER_IdServicio")).ToList();

                        servicio.HorariosServicios = detalleCentroServicioServicio.ConvertAll<PUHorariosServiciosCentroServicios>(h =>
                        {
                            return new PUHorariosServiciosCentroServicios()
                            {
                                HoraFin = h.Field<DateTime>("HoraFinal"),
                                HoraInicio = h.Field<DateTime>("HoraInicial"),
                                NombreDia = h.Field<string>("DIA_NombreDia"),
                                IdCentroServiciosServicio = h.Field<long>("CSS_IdCentroServicioServicio"),
                                IdDia = h.Field<string>("HCS_IdDia"),
                            };
                        });

                        return servicio;
                    });
            }
        }

        /// <summary>
        /// Consulta la fecha del servidor
        /// </summary>
        /// <returns></returns>
        public List<PATipoIdentificacion> ConsultarTiposIdentificacion()
        {
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerTiposIdentificacion_PAR", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                List<PATipoIdentificacion> lstTipoIden = new List<PATipoIdentificacion>();
                while (reader.Read())
                {
                    lstTipoIden.Add(new PATipoIdentificacion()
                    {
                        DescripcionIdentificacion = reader["TII_Descripcion"].ToString(),
                        IdTipoIdentificacion = reader["TII_IdTipoIdentificacion"].ToString()
                    });
                }
                conn.Close();
                return lstTipoIden;
            }
        }

        public DateTime ObtenerFechaRecogidaCiudad(string idCiudad, string fecha)
        {
            DateTime resultado = new DateTime();
            string fechaResul = string.Empty;
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("ObtenerFechaInicialRecogidaporCiudad", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Fecha", fecha);
                cmd.Parameters.AddWithValue("@IdCiudad", idCiudad);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    fechaResul = reader["Fecha"].ToString();
                    try
                    {
                        resultado = DateTime.ParseExact(fechaResul,"dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture );
                    }
                    catch (Exception)
                    {

                        //resultado = new DateTime(int.Parse(fechaResul.Substring(6,4)), int.Parse(fechaResul.Substring(3, 2)), int.Parse(fechaResul.Substring(0, 2)), int.Parse(fechaResul.Substring(11, 2)), int.Parse(fechaResul.Substring(14, 2)),int.Parse(fechaResul.Substring(17, 2)));
                        Regex rx = new Regex(@"\b([0-9]{2})\/([0-9]{2})\/([0-9]{4})\s([0-9]{2}):([0-9]{2}):?([0-9]{2})?\b",
                          RegexOptions.Compiled | RegexOptions.IgnoreCase);
                        MatchCollection matches = rx.Matches(fechaResul);

                        int dia = int.Parse(matches[0].Groups[1].ToString());
                        int mes = int.Parse(matches[0].Groups[2].ToString());
                        int anyo = int.Parse(matches[0].Groups[3].ToString());
                        int hora = int.Parse(matches[0].Groups[4].ToString());
                        int min = int.Parse(matches[0].Groups[5].ToString());
                        //int seg = int.Parse(matches[0].Groups[6].ToString());

                        resultado = new DateTime(anyo, mes, dia, hora, min,0,0);                                                        
                    }                                            
                    
                }
                conn.Close();
                return resultado;
            }
        }

        public IList<string> ObtenerFestivosAnio()
        {
            IList<string> resultado = new List<string>();
            string fechaFestivo = string.Empty;
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerFestivosDesde_PAR", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idPais", "057");
                cmd.Parameters.AddWithValue("@fechaDesde", "01/01/" + DateTime.Now.Year);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    fechaFestivo = Convert.ToDateTime(reader["CAL_Fecha"]).ToShortDateString();
                    resultado.Add(fechaFestivo);
                }
                conn.Close();
                return resultado;
            }
        }

        /// <summary>
        /// Procedimiento que trae el valor comercial minimo y maximo que aplica para contrapago
        /// Hevelin Dayana Diaz Susa - 12/11/2021
        /// </summary>
        public TAPrecioRangoContrapago ValidarValorComercialContrapago(int IdListaPrecio)
        {

            TAPrecioRangoContrapago precio = null;

            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paValidarValorComercialContrapago", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdListaPrecio", IdListaPrecio);
                conn.Open();
                SqlDataReader lector = cmd.ExecuteReader();
                while (lector.Read())
                {
                    if (lector["ValorMinimo"] != DBNull.Value && lector["ValorMaximo"] != DBNull.Value)
                    {
                        precio = new TAPrecioRangoContrapago()
                        {
                            ValorMinimo = Convert.ToInt32(lector["ValorMinimo"]),
                            ValorMaximo = Convert.ToInt32(lector["ValorMaximo"])

                        };
                    }
                    else
                    {
                        precio = null;
                    }

                }
                conn.Close();
            }
            return precio;
        }

        /// <summary>
        /// Servicio que valida si existe un centro de servicioque aplique contrapago en una determinada localidad 
        /// Hevelin Dayana Diaz - 12/11/2021
        /// </summary>
        /// <param name="IdLocalidadOrigen"></param>
        public PUCentroServiciosDC ObtenerCentroServicioContrapagoLocalidad(string IdLocalidadOrigen)
        {
            PUCentroServiciosDC centroServicio = new PUCentroServiciosDC();
            SqlCommand cmd = null;

            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                cmd = new SqlCommand("paObtenerCentroServicioContrapagoLocalidad", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdLocalidad", IdLocalidadOrigen);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    centroServicio.IdCentroServicio = Convert.ToInt64(reader["IdCentroServicio"]);
                    centroServicio.Nombre = Convert.ToString(reader["Nombre"]);
                }
                conn.Close();
            }
            return centroServicio;
        }

        /// <summary>
        /// Retorna el Estado (0 = Inactivo / 1 = Activo) de una localidad para saber si es viable o no para que permita crear preenvios
        /// </summary>
        /// <param name="IdLocalidadDestino"></param>
        /// <returns>Estado de validez de la localidad destino</returns>
        public bool ConsultarValidezDestinoGeneracionGuias(string IdLocalidadDestino)
        {
            bool valido = false;

            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("pa_ConsultarValidezDestinoGeneracionGuias_PAR", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdLocalidadDestino", IdLocalidadDestino);
                sqlConn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    valido = Convert.ToBoolean(reader["DestinoValidoGeneracionGuiaEstadoActivo"].ToString());
                }

                sqlConn.Close();
            }

            return valido;
        }

    }
}