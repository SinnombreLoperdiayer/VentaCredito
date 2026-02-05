using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Servicio.Entidades.Clientes;
using Servicios.Entidades.Clientes;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace VentaCredito.Clientes.Datos.Repositorio
{
    public class CLClienteContadoRepositorio
    {

        private static string CadCnxController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;

        private static CLClienteContadoRepositorio instancia = new CLClienteContadoRepositorio();

        public static CLClienteContadoRepositorio Instancia { get
            {
                return instancia;

            }  }

        


        /// <summary>
        /// Adiciona un cliente si este ya existe lo actualiza si ha cambiado la informacion
        /// </summary>
        /// <param name="clienteContado"></param>
        /// <returns>IDcliente si lo creo</returns>
        public long? AdicionarClienteContado(CLClienteContadoDC clienteContado, string usuarioCreacion)
        {
            CLClienteContadoDC cliente = null;
            if (!string.IsNullOrEmpty(clienteContado.Identificacion))
            {
                if (clienteContado.IdClienteContado == 0) //Busca al cliente por tipo y numero de documento
                {
                    using (SqlConnection conn = new SqlConnection(CadCnxController))
                    {
                        conn.Open();
                        SqlCommand comm = new SqlCommand("paObtenerClienteContadoPorDocumento_CLI", conn);
                        comm.CommandType = System.Data.CommandType.StoredProcedure;
                        comm.Parameters.Add(new SqlParameter("@TipoId", clienteContado.TipoId));
                        comm.Parameters.Add(new SqlParameter("@IdIdentificacion", clienteContado.Identificacion));
                        SqlDataReader lector = comm.ExecuteReader();

                        while (lector.Read())
                        {
                            cliente = new CLClienteContadoDC()
                            {
                                IdClienteContado = Convert.ToInt64(lector["CLC_IdClienteContado"]),
                                Apellido1 = Convert.ToString(lector["CLC_Apellido1"]),
                                Apellido2 = Convert.ToString(lector["CLC_Apellido2"]),
                                Direccion = Convert.ToString(lector["CLC_Direccion"]),
                                Email = Convert.ToString(lector["CLC_Email"]),
                                Nombre = Convert.ToString(lector["CLC_Nombre"]),
                                Identificacion = Convert.ToString(lector["CLC_Identificacion"]),
                                //Ocupacion =  lector["CLC_Ocupacion"],
                                Ocupacion = new PAOcupacionDC
                                {
                                    IdOcupacion = Convert.ToInt16(lector["CLC_Ocupacion"])
                                },
                                Telefono = Convert.ToString(lector["CLC_Telefono"]),
                                TipoId = Convert.ToString(lector["CLC_TipoId"]),
                                UltimaCedulaEscaneada = lector["CLC_UltimaCedulaEscaneada"] == DBNull.Value ? 0 : Convert.ToInt64(lector["CLC_UltimaCedulaEscaneada"])
                            };
                        }
                        conn.Close();
                    }
                    if (cliente != null)
                        clienteContado.IdClienteContado = cliente.IdClienteContado;
                }
                else //busca al cliente contado por id
                {
                    using (SqlConnection conn = new SqlConnection(CadCnxController))
                    {
                        conn.Open();
                        SqlCommand comm = new SqlCommand("paObtenerClienteContadoPorId_CLI", conn);
                        comm.CommandType = System.Data.CommandType.StoredProcedure;
                        comm.Parameters.Add(new SqlParameter("@IdClienteContado", clienteContado.IdClienteContado));
                        SqlDataReader lector = comm.ExecuteReader();

                        while (lector.Read())
                        {
                            cliente = new CLClienteContadoDC();

                            cliente.IdClienteContado = Convert.ToInt64(lector["CLC_IdClienteContado"]);
                            cliente.Apellido1 = Convert.ToString(lector["CLC_Apellido1"]);
                            cliente.Apellido2 = Convert.ToString(lector["CLC_Apellido2"]);
                            cliente.Direccion = Convert.ToString(lector["CLC_Direccion"]);
                            cliente.Email = Convert.ToString(lector["CLC_Email"]);
                            cliente.Nombre = Convert.ToString(lector["CLC_Nombre"]);
                            cliente.Identificacion = Convert.ToString(lector["CLC_Identificacion"]);
                            cliente.Ocupacion = new PAOcupacionDC
                            {
                                IdOcupacion = Convert.ToInt16(lector["CLC_Ocupacion"])
                            };
                            cliente.Telefono = Convert.ToString(lector["CLC_Telefono"]);
                            cliente.TipoId = Convert.ToString(lector["CLC_TipoId"]);
                            if (lector["CLC_UltimaCedulaEscaneada"] != DBNull.Value)
                                cliente.UltimaCedulaEscaneada = Convert.ToInt64(lector["CLC_UltimaCedulaEscaneada"]);

                        }
                        conn.Close();
                    }
                }
                if (cliente == null)
                {
                    using (SqlConnection conn = new SqlConnection(CadCnxController))
                    {
                        conn.Open();
                        SqlCommand comm = new SqlCommand("paInsertarClienteContado_CLI", conn);
                        comm.CommandType = System.Data.CommandType.StoredProcedure;
                        comm.Parameters.AddWithValue("@Apellido1", clienteContado.Apellido1.ToUpper().Trim());
                        comm.Parameters.AddWithValue("@Apellido2", clienteContado.Apellido2.ToUpper().Trim());
                        comm.Parameters.AddWithValue("@CreadoPor", usuarioCreacion);
                        comm.Parameters.AddWithValue("@Direccion", clienteContado.Direccion.ToUpper().Trim());
                        comm.Parameters.AddWithValue("@Email", clienteContado.Email == null ? string.Empty : clienteContado.Email.ToUpper().Trim());
                        comm.Parameters.AddWithValue("@Nombre", clienteContado.Nombre.ToUpper().Trim());
                        comm.Parameters.AddWithValue("@idIdentificacion", clienteContado.Identificacion.ToUpper().Trim());
                        comm.Parameters.AddWithValue("@Ocupacion", clienteContado.Ocupacion != null ? clienteContado.Ocupacion.IdOcupacion : (short)0);
                        comm.Parameters.AddWithValue("@Telefono", clienteContado.Telefono.ToUpper().Trim());
                        comm.Parameters.AddWithValue("@TipoId", clienteContado.TipoId);
                        comm.Parameters.AddWithValue("@UltimaCedulaEscaneada", clienteContado.UltimaCedulaEscaneada == 0 ? null : clienteContado.UltimaCedulaEscaneada);
                        comm.Parameters.AddWithValue("@FechaGrabacion", DateTime.Now);
                        Int64 num = Convert.ToInt64(comm.ExecuteScalar());
                        conn.Close();
                        return num;
                    }
                }
                else
                {
                    if (string.Compare(cliente.TipoId, clienteContado.TipoId, true) != 0) //valida si se cambio el tipo de id del cliente
                    {
                        clienteContado.ClienteModificado = true;
                    }

                    if (clienteContado.Ocupacion != null && cliente.Ocupacion.IdOcupacion != clienteContado.Ocupacion.IdOcupacion) //valida si se cambio la ocupacion del cliente
                    {
                        clienteContado.ClienteModificado = true;
                    }
                    if (clienteContado.ClienteModificado)
                    {
                        using (SqlConnection conn = new SqlConnection(CadCnxController))
                        {
                            conn.Open();
                            SqlCommand comm = new SqlCommand("paActualizarClienteContado_CLI", conn);
                            comm.CommandType = System.Data.CommandType.StoredProcedure;
                            comm.Parameters.AddWithValue("@IdClienteContado", clienteContado.IdClienteContado);
                            comm.Parameters.AddWithValue("@Apellido1", clienteContado.Apellido1.ToUpper().Trim());
                            comm.Parameters.AddWithValue("@Apellido2", clienteContado.Apellido2.ToUpper().Trim());
                            comm.Parameters.AddWithValue("@Direccion", clienteContado.Direccion.ToUpper().Trim());
                            comm.Parameters.AddWithValue("@Email", clienteContado.Email == null ? string.Empty : clienteContado.Email.ToUpper().Trim());
                            comm.Parameters.AddWithValue("@Telefono", clienteContado.Telefono.ToUpper().Trim());
                            comm.Parameters.AddWithValue("@Ocupacion", clienteContado.Ocupacion != null ? clienteContado.Ocupacion.IdOcupacion : (short)0);
                            comm.Parameters.AddWithValue("@Nombre", clienteContado.Nombre.ToUpper().Trim());
                            SqlParameter paramUltimaCedulaEscaneada = new SqlParameter();
                            paramUltimaCedulaEscaneada.ParameterName = "@UltimaCedulaEscaneada";
                            if (cliente.UltimaCedulaEscaneada.HasValue)
                            {
                                paramUltimaCedulaEscaneada.Value = cliente.UltimaCedulaEscaneada.Value;
                            }
                            else
                            {
                                paramUltimaCedulaEscaneada.Value = DBNull.Value;
                            }
                            comm.Parameters.Add(paramUltimaCedulaEscaneada);
                            comm.ExecuteNonQuery();
                            conn.Close();
                            AuditoriaRepositorio.MapearAuditModificarClienteContado(clienteContado, usuarioCreacion);
                        }
                    }
                }
            }
            return null;

        }

        /// <summary>
        /// Adicionar destinarios frecuentes al cliente remitente
        /// </summary>
        /// <param name="clienteContadoRemitente"></param>
        /// <param name="clienteContadoDestinatario"></param>
        public void AdicionarDestinatariosFrecuentes(CLClienteContadoDC clienteContadoRemitente, CLClienteContadoDC clienteContadoDestinatario, string descUltimoCentroServDestino, long idUltimoCentroServDestino, string usuarioCreacion)
        {
            var cliente = ObtenerClienteContadoDestFrecuente(clienteContadoRemitente.IdClienteContado, clienteContadoDestinatario.IdClienteContado);

            if (cliente != null)
            {
                using (SqlConnection conn = new SqlConnection(CadCnxController))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("paActualizarUltimoCentroServicioDestino_CLI", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdClienteContadoRemitente", clienteContadoRemitente.IdClienteContado);
                    cmd.Parameters.AddWithValue("@IdClienteContadoDestinatario", clienteContadoDestinatario.IdClienteContado);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        

        private List<ClienteContadoDestFrecuenDC> ObtenerClienteContadoDestFrecuente(long IdClienteContadoRemit, long IdClienteContadoDest)
        {
            List<ClienteContadoDestFrecuenDC> listaDestinatarios = new List<ClienteContadoDestFrecuenDC>();

            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerDestinatariosFrecuentes_CLI", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdClienteContadoRemitente", IdClienteContadoRemit);
                cmd.Parameters.AddWithValue("@IdDestinatarioFrecuente", IdClienteContadoDest);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var destinatario = new ClienteContadoDestFrecuenDC()
                        {
                            IdDestinatarioFrecuente = Convert.ToInt64(reader["DEF_IdDestinatarioFrecuente"]),
                            IdUltimoCentroServDestino = Convert.ToInt64(reader["DEF_IdUltimoCentroServDestino"]),
                            DescUltimoCentroServDestino = reader["DEF_DescUltimoCentroServDestino"].ToString(),
                            IdClienteContado = Convert.ToInt64(reader["DEF_IdClienteContado"]),
                            TipoId = reader["CLC_TipoId"].ToString(),
                            Identificacion = reader["CLC_Identificacion"].ToString(),
                            Nombre = reader["CLC_Nombre"].ToString(),
                            Apellido1 = reader["CLC_Apellido1"].ToString(),
                            Apellido2 = reader["CLC_Apellido2"].ToString(),
                            Telefono = reader["CLC_Telefono"].ToString(),
                            Direccion = reader["CLC_Direccion"].ToString(),
                            Email = reader["CLC_Email"].ToString(),
                            Ocupacion = Convert.ToInt32(reader["CLC_Ocupacion"]),
                            TipoIDClienteRemitente = reader["TipoIDClienteRemitente"].ToString(),
                            IDClienteRemitente = reader["IDClienteRemitente"].ToString(),
                            FechaGrabacion = Convert.ToDateTime(reader["DEF_FechaGrabacion"]),
                            IdMunicipio = reader["CES_IdMunicipio"].ToString()
                        };
                        listaDestinatarios.Add(destinatario);
                    }
                }
            }
            return listaDestinatarios;
        }
    }
}
