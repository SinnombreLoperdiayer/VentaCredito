using Framework.Servidor.Comun;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using Servicio.Entidades.Clientes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VentaCredito.Transversal.Entidades.AdmisionPreenvio;
using VentaCredito.Transversal.Entidades.Clientes;

namespace VentaCredito.Clientes.Datos.Repositorio
{
    public class CLClienteCreditoRepositorio
    {

        #region Singleton

        private static CLClienteCreditoRepositorio instancia = new CLClienteCreditoRepositorio();
        private string CnxController;
        private string CnxIseguridad;

        public static CLClienteCreditoRepositorio Instancia
        {
            get
            {
                return instancia;
            }
        }

        #endregion  

        public CLClienteCreditoRepositorio()
        {
            CnxController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;
            CnxIseguridad = ConfigurationManager.ConnectionStrings["ConnectionSeguridad"].ConnectionString;
        }


        /// <summary>
        /// Obtiene un cliente a partir del id
        /// </summary>
        /// <returns>Cliente</returns>
        public CLClientesDC ObtenerCliente(long idCliente)
        {

            using (SqlConnection sqlConn = new SqlConnection(CnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerClientePorIdCliente_CLI", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdCliente", idCliente);
                sqlConn.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    return new CLClientesDC
                    {
                        IdCliente = Convert.ToInt32(reader["CLI_IdCliente"]),
                        DigitoVerificacion = Convert.ToString(reader["CLI_DigitoVerificacion"]),
                        Direccion = Convert.ToString(reader["CLI_Direccion"]),
                        Fax = Convert.ToString(reader["CLI_Fax"]),
                        FechaConstitucion = Convert.ToDateTime(reader["CLI_FechaConstitucion"]),
                        FechaVinculacion = Convert.ToDateTime(reader["CLI_FechaVinculacion"]),
                        IdRepresentanteLegal = Convert.ToInt64(reader["CLI_IdRepresentanteLegal"]),
                        Localidad = Convert.ToString(reader["CLI_Municipio"]),
                        Nit = Convert.ToString(reader["CLI_Nit"]),
                        NombreGerente = Convert.ToString(reader["CLI_NombreGerenteGeneral"]),
                        RazonSocial = Convert.ToString(reader["CLI_RazonSocial"]),
                        Telefono = Convert.ToString(reader["CLI_Telefono"])
                    };
                }
                else
                    return new CLClientesDC();
            }
        }

        /// <summary>
        /// Retorna información del cliente crédito,
        /// retornar las  sucursal y
        /// los contrato
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="requiereSucursalContrato"></param>
        /// <returns></returns>
        public List<CLContratosDC> ObtenerContratosSucursal(int idSucursal)
        {
            List<CLContratosDC> resultado = null;

            using (SqlConnection sqlConn = new SqlConnection(CnxController))
            {
                SqlCommand cmd = new SqlCommand("paRteObtenerContratosSucursal_CLI", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idSucursal", idSucursal);
                cmd.Parameters.AddWithValue("@Estado", "ACT");
                sqlConn.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {

                    resultado = new List<CLContratosDC>();

                    while (reader.Read())
                    {
                        var r = new CLContratosDC
                        {
                            IdCliente = Convert.ToInt32(reader["CON_ClienteCredito"]),
                            AcumuladoVentas = Convert.ToInt64(reader["CON_AcumuladoVentas"]),
                            IdSucursalContrato = Convert.ToInt32(reader["SUC_Sucursal"]),
                            IdContrato = Convert.ToInt32(reader["CON_IdContrato"]),
                            NombreContrato = Convert.ToString(reader["CON_NombreContrato"]),
                            NombreInterventor = Convert.ToString(reader["CON_NombreInterventor"]),
                            CiudadInterventor = new PALocalidadDC
                            {
                                CodigoPostal = Convert.ToString(reader["CON_CiudadInterventor"]),
                            },
                            TelefonoInterventor = Convert.ToString(reader["CON_TelefonoInterventor"]),
                            CiudadGestor = new PALocalidadDC
                            {
                                CodigoPostal = Convert.ToString(reader["CON_CiudadGestorPago"]),
                            },
                            ObjetoContrato = Convert.ToString(reader["CON_ObjetoContrato"]),
                            FechaInicial = Convert.ToDateTime(reader["CON_FechaInicio"]),
                            FechaFinal = Convert.ToDateTime(reader["CON_FechaFin"]),
                            FechaFinalExtension = Convert.ToDateTime(reader["CON_FechaFinConExtensiones"]),
                            Valor = Convert.ToDecimal(reader["CON_ValorContrato"]),
                            PorcentajeAviso = Convert.ToDecimal(reader["CON_PorcentajeAviso"]),
                            PresupuestoMensual = Convert.ToDecimal(reader["CON_PresupuestoMensual"]),
                            ListaPrecios = Convert.ToInt32(reader["CON_ListaPrecios"]),
                            EjecutivoCuenta = Convert.ToInt64(reader["CON_EjecutivoCuenta"]),
                            SupervisorCuenta = Convert.ToInt64(reader["CON_SupervisorCuenta"]),
                            NumeroAsignacion = Convert.ToString(reader["CON_NumeroAsignacionPresupuest"]),
                            ValorDisponibilidad = Convert.ToDecimal(reader["CON_ValorDisponibilidad"]),
                            ValidaPeso = Convert.ToBoolean(reader["CON_AplicaValidacionPesoAdmision"]),
                        };

                        resultado.Add(r);
                    }
                }

                return resultado;
            }
        }

        /// <summary>
        /// Metodo utilizado para enviar un correo electronico  para solicitar la cancelacion de una guia.
        /// </summary>
        /// <param name="body"></param>
        public void EnviarCorreoSolicitudCancelacion(string body)
        {
            string CadenaNegocio = ConfigurationManager.ConnectionStrings["INegocio"].ConnectionString;
            string PEmailCancelacion = ConfigurationManager.AppSettings.Get("Parametro_Email_Cancelacion");
            using (SqlConnection conn = new SqlConnection(CadenaNegocio))
            {

                SqlCommand cmd = new SqlCommand("negocio.Pa_RealizarEnvioDeCorreoAUT", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdNombreCorreo", PEmailCancelacion);
                cmd.Parameters.AddWithValue("@CBody", body);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

        }

        /// <summary>
        /// Obtiene clientes creditos activos.
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="idSucursal"></param>
        /// <returns>Retorna un cliente credito activo, con su respectiva sucursal y contrato activo y vegente.</returns>
        public ClienteCreditoVC ObtenerClienteCreditoActivo(long idCliente, long idSucursal)
        {
            using (SqlConnection sqlConn = new SqlConnection(CnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerClienteCreditoActivoPorId_CLI", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdCliente", idCliente);
                cmd.Parameters.AddWithValue("@IdSucursal", idSucursal);
                sqlConn.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    return new ClienteCreditoVC()
                    {
                        IdCliente = Convert.ToInt32(reader["IdCliente"]),
                        Nit = Convert.ToString(reader["Nit"]),
                        RazonSocial = Convert.ToString(reader["RazonSocial"]),
                        Direccion = Convert.ToString(reader["Direccion"]),
                        Email = Convert.ToString(reader["Email"]),
                        Telefono = Convert.ToString(reader["Telefono"]),
                        IdLocalidad = Convert.ToString(reader["IdLocalidad"]),
                        NombreLocalidad = Convert.ToString(reader["NombreLocalidad"]),
                        IdContrato = Convert.ToInt32(reader["IdContrato"]),
                        IdListaPrecios = Convert.ToInt32(reader["IdListaPrecios"]),
                        ValorPresupuesto = Convert.ToInt64(reader["ValorPresupuesto"]),
                        Latitud = Convert.ToString(reader["Latitud"]),
                        Longitud = Convert.ToString(reader["Longitud"]),
                    };
                }
                else
                    return new ClienteCreditoVC();
            }


        }

        /// <summary>
        /// Servicio que inserta un cliente credito y le genera un usuario
        /// Hevelin Dayana Diaz Susa - 12/07/2021
        /// </summary>
        /// <param name="cliente"></param>
        /// <returns>Retorna el id del cliente si se guarda</returns>
        public UsuarioIntegracion InsertarClienteCredito(RequestCLCredito cliente)
        {
            UsuarioIntegracion nombreUsuario = new UsuarioIntegracion();
            using (SqlConnection conn = new SqlConnection(CnxIseguridad))
            {
                SqlCommand cmd = new SqlCommand("InsertarClienteCredito_SEG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idcliente", cliente.IdCliente);
                cmd.Parameters.AddWithValue("@Nit", cliente.NitCliente);
                cmd.Parameters.AddWithValue("@digito", cliente.DigitoCliente);
                cmd.Parameters.AddWithValue("@RazonSocial", cliente.NombreCliente);
                cmd.Parameters.AddWithValue("@localidad", cliente.LocalidadCliente);//ControllerContext.Current.Usuario);
                cmd.Parameters.AddWithValue("@gerente", cliente.RepresentanteCliente);
                cmd.Parameters.AddWithValue("@direccion", cliente.DireccionCliente);
                cmd.Parameters.AddWithValue("@telefono ", cliente.TelefonoCliente);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    return new UsuarioIntegracion()
                    {
                        UsuarioCliente = Convert.ToString(reader["UIN_IdUsuarioIntegracion"]),
                        PasswordCliente = Convert.ToString(reader["UIN_Password"])
                    };
                }
                else
                    return new UsuarioIntegracion();
            }
        }

        /// <summary>
        /// metodo que inserta los servicios seleccionados por un determinado cliente
        /// Hevelin Dayana Diaz Susa - 12/07/2021
        /// </summary>
        /// <param name="usuario">el usuario creado en usuario integracion</param>
        /// <param name="idServicio">id servicio seleccionado por el cliente</param>
        public void InsertarServicioUsuario(string IdUsuarioIntegracion, int IdServicio)
        {
            using (SqlConnection conn = new SqlConnection(CnxIseguridad))
            {
                SqlCommand cmd = new SqlCommand("paInsertarServicioUsuario_SEG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdUsuarioIntegracion ", IdUsuarioIntegracion);
                cmd.Parameters.AddWithValue("@IdServicio", IdServicio);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// metodo que obtiene el listado de todos los servicios creados en base de datos iseguridad
        /// Hevelin Dayana Diaz Susa - 12/07/2021
        /// </summary>
        /// <returns>Lista todos los servicios creados en tabla SERVICIOS_SEG ISEGURIDAD</returns>
        public List<Servicios_SEG> ObtenerServiciosSeguridad()
        {
            List<Servicios_SEG> resultado = null;

            using (SqlConnection sqlConn = new SqlConnection(CnxIseguridad))
            {
                SqlCommand cmd = new SqlCommand("paObtenerServicios", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                sqlConn.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {

                    resultado = new List<Servicios_SEG>();

                    while (reader.Read())
                    {
                        var r = new Servicios_SEG
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Modulo = Convert.ToString(reader["Modulo"]),
                            Url = Convert.ToString(reader["Url"]),
                            Estado = Convert.ToBoolean(reader["Estado"]),
                            Descripcion = Convert.ToString(reader["Descripcion"])
                        };
                        resultado.Add(r);
                    }
                }
                return resultado;
            }
        }

        /// <summary>
        /// Metodo que inserta token en tabla usuarioIntegracion, haciendo una actualizacion a esa misma tabla
        /// Hevelin Dayana Diaz Susa - 16/07/2021
        /// </summary>
        /// <param name="token"></param>
        /// <param name="idUsuarioIntegracion"></param>
        public void InsertarTokenUsuarioIntegracion(string token, string idUsuarioIntegracion)
        {
            using (SqlConnection conn = new SqlConnection(CnxIseguridad))
            {
                SqlCommand cmd = new SqlCommand("paActualizarTokenUsuarioIntegracion_SEG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdUsuarioIntegracion ", idUsuarioIntegracion);
                cmd.Parameters.AddWithValue("@Token", token);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Metodo que activa proceso push para un cliente credito
        /// Hevelin Dayana Diaz - 6/10/2021
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public void ActivarPushClienteCredito(RequestCLCredito cliente)
        {
            using (SqlConnection conn = new SqlConnection(CnxIseguridad))
            {
                SqlCommand cmd = new SqlCommand("paActivarPushClientesCredito_SEG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idcliente", cliente.IdCliente);
                cmd.Parameters.AddWithValue("@urlToken", cliente.urlToken);
                cmd.Parameters.AddWithValue("@urlNotificacion", cliente.urlNotificacion);
                cmd.Parameters.AddWithValue("@key", cliente.key);
                cmd.Parameters.AddWithValue("@Secret", cliente.Secret);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Metodo que Dasactiva proceso push para un cliente credito
        /// Hevelin Dayana Diaz - 8/10/2021
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public void DesactivarPushClienteCredito(RequestCLCredito cliente)
        {
            using (SqlConnection conn = new SqlConnection(CnxIseguridad))
            {
                SqlCommand cmd = new SqlCommand("paDesactivarPushClientesCredito_SEG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idcliente", cliente.IdCliente);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// metodo que inserta los estados seleccionados por un determinado cliente para proceso push
        /// Hevelin Dayana Diaz Susa - 06/10/2021
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="idEstado"></param>
        public void InsertarEstadoGuiaClientes(Int64 idCliente, string idEstados)
        {
            using (SqlConnection conn = new SqlConnection(CnxController))
            {
                SqlCommand cmd = new SqlCommand("paInsertarEstadosGuiasClienteCredito_CLI", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdCliente", idCliente);
                cmd.Parameters.AddWithValue("@IdEstado", idEstados);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }


        

        public string ConsultarNumeroMaximoGuias(string NombreParametro)
        {
            string parametro = null;
            using (SqlConnection conn = new SqlConnection(CnxController))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerParametrosAdmision", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Parametro", NombreParametro);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        parametro = Convert.ToString(reader["PAM_ValorParametro"]);
                    }
                }
                return parametro;
            }
        }

        /// <summary>
        /// Servicio que consulta las sucursales activas asociadas  a un cliente, teniendo en cuenta el contrato actual.
        /// Hevelin Dayana Diaz - 28/04/2023
        /// </summary>
        /// <param name="idCliente">Id cliente credito</param>
        /// <returns>Lista de sucursales asociadas a un cliente credito</returns>
        public List<SucursalCliente_CLI> ObtenerSucursalesActivasPorCliente(int idCliente)
        {
            List<SucursalCliente_CLI> resultado = null;

            using (SqlConnection sqlConn = new SqlConnection(CnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerSucursalesActivasPorIdCliente_CLI", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdCliente", idCliente);
                sqlConn.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    resultado = new List<SucursalCliente_CLI>();
                    while (reader.Read())
                    {
                        var r = new SucursalCliente_CLI
                        {
                            SUC_IdSucursal = Convert.ToInt32(reader["SUC_IdSucursal"]),
                            SUC_Nombre = Convert.ToString(reader["SUC_Nombre"])
                        };
                        resultado.Add(r);
                    }
                }
                return resultado;
            }
        }

        /// <summary>
        /// Actualiza la información de recogida de un cliente crédito
        /// Mauricio Hernandez Cabrera - 18/08/2023 - HU 51407
        /// </summary>
        /// <returns>Mensaje Satisfactorio o Fallido del proceso de actualización de la información de recogida del cliente crédito</returns>
        public string ActualizarInfoRecogidaClienteCredito(RequestCLCredito reqCliente)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(CnxController))
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("pa_ActualizarInfoRecogidaClienteCredito_CLI", connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter("@Direccion", reqCliente.DireccionCliente));
                    cmd.Parameters.Add(new SqlParameter("@Telefono", reqCliente.TelefonoCliente));
                    cmd.Parameters.Add(new SqlParameter("@CodigoMunicipio", reqCliente.LocalidadCliente));
                    cmd.Parameters.Add(new SqlParameter("@IdClienteCredito", reqCliente.IdCliente));
                    cmd.Parameters.Add(new SqlParameter("@IdSucursal", reqCliente.IdSucursal));
                    cmd.Parameters.Add(new SqlParameter("@IdZona", reqCliente.IdZona));
                    int rowsAffected = cmd.ExecuteNonQuery();

                    string mensaje = string.Empty;
                    if (rowsAffected > 0)
                    {
                        mensaje = "Proceso realizado exitosamente.";
                    }

                    connection.Close();
                    return mensaje;
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Proceso FALLIDO. {e.Message}");
            }
        }

    }
}
