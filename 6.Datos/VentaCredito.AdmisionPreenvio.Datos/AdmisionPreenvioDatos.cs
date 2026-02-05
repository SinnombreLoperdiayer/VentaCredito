using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VentaCredito.Transversal.Entidades.AdmisionPreenvio;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace VentaCredito.AdmisionPreenvio.Datos
{
    public class AdmisionPreenvioDatos
    {
        #region Singleton

        private static AdmisionPreenvioDatos instancia = new AdmisionPreenvioDatos();
        private string CnxController;
        private string CnxPreenvios;
        public static AdmisionPreenvioDatos Instancia
        {
            get
            {
                return instancia;
            }
        }
        #endregion
        public AdmisionPreenvioDatos()
        {
            CnxController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;
            CnxPreenvios = ConfigurationManager.ConnectionStrings["Preenvios"].ConnectionString;
        }

        /// <summary>
        /// Valida que el preenvio exista y pertenezca a el cliente credito especifico
        /// </summary>
        /// <param name="numeroPreenvio"></param>
        /// <param name="idCliente"></param>
        /// <param name="idSucursal"></param>
        /// <returns></returns>
        public bool ValidarExistenciaPreenvio(long numeroPreenvio, long idCliente, long idSucursal)
        {
            bool existePreenvio;
            using (SqlConnection con = new SqlConnection(CnxPreenvios))
            {
                SqlCommand cmd = new SqlCommand("pa_ValidarExistenciaPreenvio", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdClienteCredito", idCliente);
                cmd.Parameters.AddWithValue("@IdSucursal", idSucursal);
                cmd.Parameters.AddWithValue("@NumeroPreenvio", numeroPreenvio);
                con.Open();
                var existe = cmd.ExecuteScalar();
                con.Close();
                existePreenvio = Convert.ToBoolean(existe);   
            }
                return existePreenvio;
        }

        /// <summary>
        /// Consulta las preguias por rango de fecha, cliente y sucursal
        /// </summary>
        /// <param name="requestPreeGuias"></param>
        /// <returns></returns>
        public List<long> ConsultarPreGuiasPorFechas(RequestImpresionPreguia requestPreeGuias)
        {
            List<long> ltsPreenvios = new List<long>();
            using (SqlConnection con = new SqlConnection(CnxPreenvios))
            {
                SqlCommand cmd = new SqlCommand("pa_ObtenerPreenviosClientePorFecha", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdClienteCredito", requestPreeGuias.IdCliente);
                cmd.Parameters.AddWithValue("@IdSucursal", requestPreeGuias.IdSucursal);
                cmd.Parameters.AddWithValue("@FechaInicio", requestPreeGuias.FechaInicio);
                cmd.Parameters.AddWithValue("@FechaFinal", requestPreeGuias.FechaFinal);
                con.Open();
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ltsPreenvios.Add((long)(reader["PRA_NumeroPreenvio"]));
                    }

                }
                con.Close();
            }
            return ltsPreenvios;
        }

        /// <summary>
        /// Obtiene todos los estados que puede tener una guia
        /// Hevelin Dayana Diaz - 05/10/2021
        /// </summary>
        /// <returns>Lista de estados mensajeria</returns>
        public List<EstadoGuia_MEN> ObtenerEstadosLogisticosGuias()
        {
            List<EstadoGuia_MEN> ltsEstados = new List<EstadoGuia_MEN>();
            using (SqlConnection con = new SqlConnection(CnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerTodosLosEstadosGias_MEN", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var estado = new EstadoGuia_MEN()
                        {
                            ESG_IdEstadoGuia = Convert.ToInt64(reader["ESG_IdEstadoGuia"]),
                            ESG_Descripcion = Convert.ToString(reader["ESG_Descripcion"])
                        };
                        ltsEstados.Add(estado);
                    }

                }
                con.Close();
            }
            return ltsEstados;
        }

        /// <summary>
        /// Método que consulta por localidad de destino si retorna formato en base 64 normal o base 64 simplificado
        /// Hevelin Dayana Diaz - 08/06/2022
        /// </summary>
        /// <param name="IdLocalidadDes"></param>
        /// <returns>1 si es base 64 simplificado, 2 si es base 64 normal</returns>
        public int ConsultarFormatoGuiaSimpliLocDest(string IdLocalidadDes)
        {
            int EsFormatoSimpli;
            using (SqlConnection con = new SqlConnection(CnxController))
            {
                SqlCommand cmd = new SqlCommand("paConsultarFormatoGuiaSimpliLocDestino_CLI", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdLocalidadDest", IdLocalidadDes);
                con.Open();
                EsFormatoSimpli = Convert.ToInt32(cmd.ExecuteScalar());
                con.Close();
            }
            return EsFormatoSimpli;
        }


        /// <summary>
        /// Método que consulta por localidad de destino si se puede georreferenciar la ciudad y si tiene codigo postal
        /// Hevelin Dayana Diaz - 17/08/2022
        /// </summary>
        /// <param name="IdLocalidadDes"></param>
        /// <returns>1 si es base 64 simplificado, 2 si es base 64 normal</returns>
        public Localidad_PAR ConsultarZonayGeoPorIdLacalidad_CLI(string IdLocalidadDes)
        {
            Localidad_PAR localidad = new Localidad_PAR();
            using (SqlConnection conn = new SqlConnection(CnxController))
            {
                SqlCommand command = new SqlCommand("paConsultarZonayGeoPorIdLacalidad_CLI", conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@IdLocalidad", IdLocalidadDes);

                conn.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    localidad.LOC_IdLocalidad = Convert.ToString(reader["LOC_IdLocalidad"]);
                    localidad.LOC_CodigoPostal = Convert.ToString(reader["LOC_CodigoPostal"]);
                    localidad.LOC_SeGeorreferencia = Convert.ToBoolean(reader["LOC_SeGeorreferencia"]);
                }
                conn.Close();
            }
            return localidad;
        }

        /// <summary>
        /// Método que consulta un cliente frecuente por numero de documento
        /// Hevelin Dayana Diaz - 17/08/2022
        /// </summary>
        /// <param name="NumeroDocumento"></param>
        /// <returns>1 si es base 64 simplificado, 2 si es base 64 normal</returns>
        public DestinatarioFrecuente_CLI ConsultarClienteFrecuentePorIdentificacion_CLI(string NumeroDocumento, string TipoId)
        {
            DestinatarioFrecuente_CLI destinatario = new DestinatarioFrecuente_CLI();
            using (SqlConnection conn = new SqlConnection(CnxController))
            {
                SqlCommand command = new SqlCommand("paObtenerClienteContadoPorDocumento_CLI", conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@IdIdentificacion", NumeroDocumento);
                command.Parameters.AddWithValue("@TipoId", TipoId);

                conn.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    destinatario.CLC_IdClienteContado = Convert.ToInt64(reader["CLC_IdClienteContado"]);
                    destinatario.CLC_Identificacion = Convert.ToString(reader["CLC_Identificacion"]);
                    destinatario.CLC_ZonaPostal = Convert.ToString(reader["CLC_ZonaPostal"]);
                    destinatario.CLC_Direccion = Convert.ToString(reader["CLC_Direccion"]);
                    destinatario.CLC_Longitude = Convert.ToString(reader["CLC_Longitude"]);
                    destinatario.CLC_Latitude = Convert.ToString(reader["CLC_Latitude"]);
                    destinatario.CLC_IdLocalidad = Convert.ToString(reader["CLC_IdLocalidad"]);
                    destinatario.CLC_Zona1 = Convert.ToString(reader["CLC_Zona1"]);
                    destinatario.CLC_Barrio = Convert.ToString(reader["CLC_Barrio"]);
                    destinatario.CLC_DirNormalizada = Convert.ToString(reader["CLC_DirNormalizada"]);
                    destinatario.CLC_Localidad = Convert.ToString(reader["CLC_Localidad"]);
                    destinatario.CLC_EstadoGeo = Convert.ToString(reader["CLC_EstadoGeo"]);
                    destinatario.CLC_MacroZona = Convert.ToString(reader["CLC_MacroZona"]);
                    destinatario.CLC_MicroZona = Convert.ToString(reader["CLC_MicroZona"]);
                }
                conn.Close();
            }
            return destinatario;
        }

        /// <summary>
        /// Método que insertar la verificacion de contenido asociado al preenvio creado
        /// Hevelin Dayanana Diaz - 14/10/2022
        /// </summary>
        public void InsertarVerificacionContenidoNumeroGuia(VerificacionContenido_ADM verificacion)
        {
            using (SqlConnection connection = new SqlConnection(CnxController))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("paInsertarVerificacionContenidoNumeroGuia", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@NumeroGuia", verificacion.ADM_NumeroGuia));
                cmd.Parameters.Add(new SqlParameter("@VerificacionContenido", verificacion.ADM_VerificacionContenido));
                cmd.ExecuteNonQuery();

                connection.Close();
            }
        }

        /// <summary>
        /// Método que insertar la verificacion de contenido asociado al preenvio creado en la base de datos de preenvios
        /// Jonathan Contreras - 04/01/2023
        /// </summary>
        public void InsertarVerificacionContenidoNumeroGuiaPreenvio(VerificacionContenido_ADM verificacion)
        {
            using (SqlConnection connection = new SqlConnection(CnxPreenvios))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("paInsertarVerificacionContenidoNumeroGuia", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@NumeroGuia", verificacion.ADM_NumeroGuia));
                cmd.Parameters.Add(new SqlParameter("@VerificacionContenido", verificacion.ADM_VerificacionContenido));
                cmd.ExecuteNonQuery();

                connection.Close();
            }
        }

        /// <summary>
        /// Obtiene la info del preenvio asociada a un numero de preenvio
        /// Mauricio Hernández Cabrera - 18/04/2023
        /// </summary>
        /// <param name="numero">Numero de preenvio</param>
        public PreenvioAdmisionCL ObtenerPreenvioClienteCredito(long numero)
        {
            PreenvioAdmisionCL preenvio = new PreenvioAdmisionCL();
            using (SqlConnection conn = new SqlConnection(CnxPreenvios))
            {
                SqlCommand command = new SqlCommand("paObtenerAdmisionClienteCreditoPorNumero", conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@NumeroPreenvio", numero);

                conn.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    preenvio.IdPreenvio = Convert.ToInt64(reader["PRA_IdPreenvio"]);
                    preenvio.NumeroPreenvio = Convert.ToInt64(reader["PRA_NumeroPreenvio"]);
                    preenvio.IdEstadoPreenvio = Convert.ToInt16(reader["PRA_IdEstadoPreenvio"]);
                    preenvio.DescripcionEstado = Convert.ToString(reader["PRA_DescripcionEstado"]);
                    preenvio.Peso = Convert.ToDecimal(reader["PRA_Peso"]);
                    preenvio.NumeroPieza = Convert.ToInt16(reader["PRA_NumeroPieza"]);
                    preenvio.IdClienteCredito = Convert.ToInt64(reader["PRA_IdClienteCredito"]);
                    preenvio.CodigoConvenioRemitente = Convert.ToInt64(reader["PRA_CodigoConvenioRemitente"]);
                    preenvio.FechaGrabacionEstado = Convert.ToDateTime(reader["PRA_FechaGrabacionEstado"]);
                    preenvio.IdCiudadDestino = Convert.ToString(reader["PRA_IdCiudadDestino"]);
                    preenvio.NombreCiudadDestino = Convert.ToString(reader["NombreCiudadDestino"]);
                    preenvio.IdUnidadNegocio = Convert.ToString(reader["PRA_IdUnidadNegocio"]);
                    preenvio.IdServicio = Convert.ToInt32(reader["PRA_IdServicio"]);
                    preenvio.NombreServicio = Convert.ToString(reader["PRA_NombreServicio"]);
                    preenvio.IdTipoEntrega = Convert.ToString(reader["PRA_IdTipoEntrega"]);
                    preenvio.IdCentroServicioDestino = Convert.ToInt64(reader["PRA_IdCentroServicioDestino"]);
                    preenvio.IdPaisOrigen = Convert.ToString(reader["PRA_IdPaisOrigen"]);
                    preenvio.IdCiudadOrigen = Convert.ToString(reader["PRA_IdCiudadOrigen"]);
                    preenvio.CiudadOrigen = Convert.ToString(reader["NombreCiudadOrigen"]);
                    preenvio.CodigoPostalOrigen = Convert.ToString(reader["PRA_CodigoPostalOrigen"]);
                    preenvio.IdPaisDestino = Convert.ToString(reader["PRA_IdPaisDestino"]);
                    preenvio.CodigoPostalDestino = Convert.ToString(reader["PRA_CodigoPostalDestino"]);
                    preenvio.TelefonoDestinatario = Convert.ToString(reader["PRA_TelefonoDestinatario"]);
                    preenvio.DireccionDestinatario = Convert.ToString(reader["PRA_DireccionDestinatario"]);
                    preenvio.NombreDestinatario = Convert.ToString(reader["PRA_NombreDestinatario"]);
                    preenvio.ApellidoDestinatario = Convert.ToString(reader["PRA_ApellidoDestinatario"]);
                    preenvio.EmailDestinatario = Convert.ToString(reader["PRA_EmailDestinatario"]);
                    preenvio.TipoCliente = Convert.ToString(reader["PRA_TipoCliente"]);
                    preenvio.ValorTotal = Convert.ToDecimal(reader["PRA_ValorTotal"]);
                    preenvio.ValorDeclarado = Convert.ToDecimal(reader["PRA_ValorDeclarado"]);
                    preenvio.FechaPreenvio = Convert.ToDateTime(reader["PRA_FechaPreenvio"]);
                    preenvio.IdTipoEnvio = Convert.ToInt16(reader["PRA_IdTipoEnvio"]);
                    preenvio.NombreTipoEnvio = Convert.ToString(reader["PRA_NombreTipoEnvio"]);
                    preenvio.IdentificacionRemitente = Convert.ToString(reader["PRA_IdentificacionRemitente"]);
                    preenvio.NombreRemitente = Convert.ToString(reader["PRA_NombreRemitente"]);
                    preenvio.IdTipoIdentificacionDestinatario = Convert.ToString(reader["PRA_IdTipoIdentificacionDestinatario"]);
                    preenvio.IdentificacionDestinatario = Convert.ToString(reader["PRA_IdentificacionDestinatario"]);
                    preenvio.DescripcionTipoEntrega = Convert.ToString(reader["PRA_DescripcionTipoEntrega"]);
                    preenvio.NombreCiudadOrigen = Convert.ToString(reader["PRA_NombreCiudadOrigen"]);
                    preenvio.ValorAdmision = Convert.ToDecimal(reader["PRA_ValorAdmision"]);
                    preenvio.Observaciones = Convert.ToString(reader["PRA_Observaciones"]);
                    preenvio.IdCentroServicioOrigen = Convert.ToInt64(reader["PRA_IdCentroServicioOrigen"]);
                    preenvio.NombreCentroServicioOrigen = Convert.ToString(reader["PRA_NombreCentroServicioOrigen"]);
                    preenvio.IdContrato = Convert.ToInt32(reader["PRA_IdContrato"]);
                    preenvio.IdListaPrecios = Convert.ToInt32(reader["PRA_IdListaPrecios"]);
                    preenvio.IdRecogida = reader["PRA_IdRecogida"] != DBNull.Value ? Convert.ToInt64(reader["PRA_IdRecogida"]) : 0;
                    preenvio.IdFormaPago = Convert.ToInt16(reader["PRA_IdFormaPago"]);
                    preenvio.NombreFormaPago = Convert.ToString(reader["FOP_Descripcion"]);
                }
                conn.Close();
            }
            return preenvio;
        }

    }
}
