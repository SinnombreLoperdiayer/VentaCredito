using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using CO.Servidor.Clientes.Comun;
using CO.Servidor.Clientes.Datos.Modelo;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Servicios;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System.Data.SqlClient;

namespace CO.Servidor.Clientes.Datos
{
    public partial class CLRepositorio
    {
        /// <summary>
        /// Consulta la informacion de un cliente Contado a partir de un tipo de documento y un numero de documento
        /// </summary>
        /// <param name="tipoDocumento">Tipo de documento del cliente a consultar</param>
        /// <param name="numeroDocumento">Numéro del documento del cliente a consultar </param>
        /// <returns>Cliente Contado</returns>
        public CLClienteContadoDC ConsultarClienteContado(string tipoDocumento, string numeroDocumento)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                List<CLClienteContado_CLI> clienteContado = contexto.paObtenerClienteContado_CLI(numeroDocumento, tipoDocumento, null).ToList();
                if (clienteContado == null || clienteContado.Count == 0)
                {
                    return null;
                }

                return new CLClienteContadoDC()
                {
                    IdClienteContado = clienteContado.First().CLC_IdClienteContado,
                    Apellido1 = clienteContado.First().CLC_Apellido1 != null ? clienteContado.First().CLC_Apellido1.ToUpper().Trim() : string.Empty,
                    Apellido2 = clienteContado.First().CLC_Apellido2 != null ? clienteContado.First().CLC_Apellido2.ToUpper().Trim() : string.Empty,
                    Direccion = clienteContado.First().CLC_Direccion != null ? clienteContado.First().CLC_Direccion.ToUpper().Trim() : string.Empty,
                    Email = clienteContado.First().CLC_Email != null ? clienteContado.First().CLC_Email.ToUpper().Trim() : string.Empty,
                    Identificacion = clienteContado.First().CLC_Identificacion != null ? clienteContado.First().CLC_Identificacion.ToUpper().Trim() : string.Empty,
                    Nombre = clienteContado.First().CLC_Nombre.ToUpper() != null ? clienteContado.First().CLC_Nombre.ToUpper().Trim() : string.Empty,
                    Ocupacion = new PAOcupacionDC() { IdOcupacion = clienteContado.First().CLC_Ocupacion },
                    Telefono = clienteContado.First().CLC_Telefono != null ? clienteContado.First().CLC_Telefono.ToUpper().Trim() : string.Empty,
                    TipoId = clienteContado.First().CLC_TipoId,
                    ClienteModificado = false
                };
            }
        }

        /// <summary>
        /// Obtiene los clientes activos que tengan una sucursal activa por municipio
        /// </summary>
        public List<CLClientesDC> ObtenerClientesSucursalesActivas(string idLocalidad)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ClienteSucursalContrato_VCLI.Where(r => r.SUC_Municipio == idLocalidad &&
                                                                  r.CLI_Estado == ConstantesFramework.ESTADO_ACTIVO
                                                                  && r.SUC_Estado == ConstantesFramework.ESTADO_ACTIVO)
                  .ToList()
                  .ConvertAll(r => new CLClientesDC()
                  {
                      IdCliente = r.CLI_IdCliente,
                      Nit = r.CLI_Nit,
                      RazonSocial = r.CLI_RazonSocial,
                      Direccion = r.CLI_Direccion,
                      Telefono = r.CLI_Telefono,
                  });
            }
        }

        /// <summary>
        /// Consulta la información de un cliente a partir de su número telefónico
        /// </summary>
        /// <param name="numTelefono"></param>
        /// <returns></returns>
        public List<CLClienteContadoDC> ConsultarClienteContado(string numTelefono)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                List<CLClienteContado_CLI> clienteContado = contexto.paObtenerClienteContado_CLI(null, null, numTelefono).ToList();

                if (clienteContado == null || clienteContado.Count == 0)
                {
                    return null;
                }

                return clienteContado.ConvertAll(cli => new CLClienteContadoDC
                {
                    Apellido1 = cli.CLC_Apellido1 != null ? cli.CLC_Apellido1.ToUpper().Trim() : string.Empty,
                    Apellido2 = cli.CLC_Apellido2 != null ? cli.CLC_Apellido2.ToUpper().Trim() : string.Empty,
                    Direccion = cli.CLC_Direccion != null ? cli.CLC_Direccion.ToUpper().Trim() : string.Empty,
                    Email = cli.CLC_Email != null ? cli.CLC_Email.ToUpper().Trim() : string.Empty,
                    IdClienteContado = cli.CLC_IdClienteContado,
                    Identificacion = cli.CLC_Identificacion != null ? cli.CLC_Identificacion.ToUpper().Trim() : string.Empty,
                    Nombre = cli.CLC_Nombre != null ? cli.CLC_Nombre.ToUpper().Trim() : string.Empty,
                    Telefono = cli.CLC_Telefono != null ? cli.CLC_Telefono.ToUpper().Trim() : string.Empty,
                    TipoId = cli.CLC_TipoId
                });
            }
        }

        /// <summary>
        /// Consulta los destinatarios frecuentes de un cliente
        /// </summary>
        /// <param name="tipoDocumento">Tipo de documento del cliente</param>
        /// <param name="numeroDocumento">Numero de documento del cliente</param>
        /// <returns>Lista con los ultimos 3 destinatarios frecuentes.</returns>
        public IList<CLDestinatarioFrecuenteDC> ConsultarDestinatarioFrecuente(string tipoDocumento, string numeroDocumento, string idMunicipioDestino)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                List<CLDestinatarioFrecuenteDC> destinatarios;
                if (idMunicipioDestino != null)
                {
                    destinatarios = contexto.ClienteContadoDestFrecuen_VCLI.Where(
                      cc => cc.TipoIDClienteRemitente == tipoDocumento && cc.CES_IdMunicipio == idMunicipioDestino &&
                        cc.IDClienteRemitente == numeroDocumento &&
                        cc.CLC_Identificacion != numeroDocumento
                        ).OrderByDescending(fec => fec.DEF_FechaGrabacion).ToList().ConvertAll(
                          df => new CLDestinatarioFrecuenteDC()
                          {
                              IdUltimoCentroServDestino = df.DEF_IdUltimoCentroServDestino,
                              ClienteContado = new CLClienteContadoDC()
                              {
                                  IdClienteContado = df.DEF_IdDestinatarioFrecuente,
                                  Apellido1 = df.CLC_Apellido1.ToUpper().Trim(),
                                  Apellido2 = df.CLC_Apellido2.ToUpper().Trim(),
                                  Direccion = df.CLC_Direccion.ToUpper().Trim(),
                                  Email = df.CLC_Email.ToUpper().Trim(),
                                  Identificacion = df.CLC_Identificacion.ToUpper().Trim(),
                                  Nombre = df.CLC_Nombre.ToUpper().Trim(),
                                  Ocupacion = new PAOcupacionDC() { IdOcupacion = df.CLC_Ocupacion },
                                  Telefono = df.CLC_Telefono.ToUpper().Trim(),
                                  TipoId = df.CLC_TipoId,
                                  ClienteModificado = false
                              }
                          }).Distinct().ToList();
                }
                else
                {
                    destinatarios = contexto.ClienteContadoDestFrecuen_VCLI.Where(
                           cc => cc.TipoIDClienteRemitente == tipoDocumento &&
                           cc.IDClienteRemitente == numeroDocumento &&
                           cc.CLC_Identificacion != numeroDocumento
                           ).OrderByDescending(fec => fec.DEF_FechaGrabacion).ToList().ConvertAll(
                             df => new CLDestinatarioFrecuenteDC()
                             {
                                 IdUltimoCentroServDestino = df.DEF_IdUltimoCentroServDestino,
                                 ClienteContado = new CLClienteContadoDC()
                                 {
                                     IdClienteContado = df.DEF_IdDestinatarioFrecuente,
                                     Apellido1 = df.CLC_Apellido1.ToUpper().Trim(),
                                     Apellido2 = df.CLC_Apellido2.ToUpper().Trim(),
                                     Direccion = df.CLC_Direccion.ToUpper().Trim(),
                                     Email = df.CLC_Email.ToUpper().Trim(),
                                     Identificacion = df.CLC_Identificacion.ToUpper().Trim(),
                                     Nombre = df.CLC_Nombre.ToUpper().Trim(),
                                     Ocupacion = new PAOcupacionDC() { IdOcupacion = df.CLC_Ocupacion },
                                     Telefono = df.CLC_Telefono.ToUpper().Trim(),
                                     TipoId = df.CLC_TipoId,
                                     ClienteModificado = false
                                 }
                             }).Distinct().ToList();
                }

                // Validar listas restrictivas
                List<CLDestinatarioFrecuenteDC> destinatariosSinListas = destinatarios.Where(
                  dest =>
                     PAAdministrador.Instancia.ValidarListaRestrictiva(dest.ClienteContado.Identificacion) == false).Take(3).ToList();

                return destinatariosSinListas;
            }
        }

        /// <summary>
        /// Adicionar destinarios frecuentes al cliente remitente
        /// </summary>
        /// <param name="clienteContadoRemitente"></param>
        /// <param name="clienteContadoDestinatario"></param>
        public  void AdicionarDestinatariosFrecuentes(CLClienteContadoDC clienteContadoRemitente, CLClienteContadoDC clienteContadoDestinatario, string descUltimoCentroServDestino, long idUltimoCentroServDestino, string usuarioCreacion)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ClienteContadoDestFrecuen_VCLI cliente = contexto.ClienteContadoDestFrecuen_VCLI.Where(des => des.DEF_IdClienteContado == clienteContadoRemitente.IdClienteContado && des.DEF_IdDestinatarioFrecuente == clienteContadoDestinatario.IdClienteContado).FirstOrDefault();

                // Si el cliente destinatario ya es un cliente frecuente
                if (cliente != null)
                {
                    DestinatarioFrecuente_CLI destinatario = contexto.DestinatarioFrecuente_CLI.FirstOrDefault(dest =>
                     dest.DEF_IdClienteContado == clienteContadoRemitente.IdClienteContado &&
                     dest.DEF_IdDestinatarioFrecuente == clienteContadoDestinatario.IdClienteContado);
                    if (destinatario != null)
                    {
                        destinatario.DEF_IdUltimoCentroServDestino = idUltimoCentroServDestino;
                        contexto.SaveChanges();
                    }
                    return;
                }

                // S el cliente tiene 3 o mas destinatario remueve el mas viejo
                if (contexto.ClienteContadoDestFrecuen_VCLI.Where(des => des.DEF_IdClienteContado == clienteContadoRemitente.IdClienteContado).Count() > 2)
                {
                    DestinatarioFrecuente_CLI clienteEliminar = contexto.DestinatarioFrecuente_CLI
                      .Where(des => des.DEF_IdClienteContado == clienteContadoRemitente.IdClienteContado)
                      .OrderBy(df => df.DEF_FechaGrabacion).First();
                    contexto.DestinatarioFrecuente_CLI.Remove(clienteEliminar);
                }

                if (descUltimoCentroServDestino == null)
                    descUltimoCentroServDestino = "";
                if (idUltimoCentroServDestino == null)
                    idUltimoCentroServDestino = 0;


               DestinatarioFrecuente_CLI desFrecuente =  contexto.DestinatarioFrecuente_CLI.Where(d => d.DEF_IdClienteContado ==  clienteContadoRemitente.IdClienteContado && d.DEF_IdDestinatarioFrecuente == clienteContadoDestinatario.IdClienteContado).FirstOrDefault();

               if (desFrecuente == null)
               {

                   DestinatarioFrecuente_CLI nuevoDestinatario = new DestinatarioFrecuente_CLI()
                    {
                        DEF_IdClienteContado = clienteContadoRemitente.IdClienteContado,
                        DEF_IdDestinatarioFrecuente = clienteContadoDestinatario.IdClienteContado,
                        DEF_FechaGrabacion = DateTime.Now,
                        DEF_CreadoPor = usuarioCreacion,
                        DEF_DescUltimoCentroServDestino = descUltimoCentroServDestino,
                        DEF_IdUltimoCentroServDestino = idUltimoCentroServDestino
                    };
                   contexto.DestinatarioFrecuente_CLI.Add(nuevoDestinatario);
               }
               else
               {
                   desFrecuente.DEF_DescUltimoCentroServDestino = descUltimoCentroServDestino;
                   desFrecuente.DEF_IdUltimoCentroServDestino = idUltimoCentroServDestino;
               }                
                contexto.SaveChanges();
            }
        }

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
                            CLRepositorioAudit.MapearAuditModificarClienteContado(clienteContado, usuarioCreacion);
                        }
                    }
                }
            }
            return null;

            //using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{

            //    ClienteContado_CLI cliente = null;
            //    if (!string.IsNullOrEmpty(clienteContado.Identificacion))
            //    {
            //        if (clienteContado.IdClienteContado == 0) //Busca al cliente por tipo y numero de documento
            //        {
            //            cliente = contexto.ClienteContado_CLI.FirstOrDefault(cc => cc.CLC_TipoId == clienteContado.TipoId && cc.CLC_Identificacion == clienteContado.Identificacion);
            //            if (cliente != null)
            //                clienteContado.IdClienteContado = cliente.CLC_IdClienteContado;
            //        }
            //        else //busca al cliente contado por id
            //        {
            //            cliente = contexto.ClienteContado_CLI.FirstOrDefault(cc => cc.CLC_IdClienteContado == clienteContado.IdClienteContado);
            //        }
            //        if (cliente == null)
            //        {

            //            ClienteContado_CLI nuevoClienteContado = new ClienteContado_CLI()
            //            {
            //                CLC_Apellido1 = clienteContado.Apellido1.ToUpper().Trim(),
            //                CLC_Apellido2 = clienteContado.Apellido2.ToUpper().Trim(),
            //                CLC_CreadoPor = usuarioCreacion,
            //                CLC_Direccion = clienteContado.Direccion.ToUpper().Trim(),
            //                CLC_Email = clienteContado.Email == null ? string.Empty : clienteContado.Email.ToUpper().Trim(),
            //                CLC_Nombre = clienteContado.Nombre.ToUpper().Trim(),
            //                CLC_Identificacion = clienteContado.Identificacion.ToUpper().Trim(),
            //                CLC_Ocupacion = clienteContado.Ocupacion != null ? clienteContado.Ocupacion.IdOcupacion : (short)0,
            //                CLC_Telefono = clienteContado.Telefono.ToUpper().Trim(),
            //                CLC_TipoId = clienteContado.TipoId,
            //                CLC_UltimaCedulaEscaneada = clienteContado.UltimaCedulaEscaneada == 0 ? null : clienteContado.UltimaCedulaEscaneada,
            //                CLC_FechaGrabacion = DateTime.Now
            //            };

            //            contexto.ClienteContado_CLI.Add(nuevoClienteContado);
            //            contexto.SaveChanges();
            //            return nuevoClienteContado.CLC_IdClienteContado;
            //        }
            //        else
            //        {
            //            if (string.Compare(cliente.CLC_TipoId, clienteContado.TipoId, true) != 0) //valida si se cambio el tipo de id del cliente
            //            {
            //                clienteContado.ClienteModificado = true;
            //            }

            //            if (clienteContado.Ocupacion != null && cliente.CLC_Ocupacion != clienteContado.Ocupacion.IdOcupacion) //valida si se cambio la ocupacion del cliente
            //            {
            //                clienteContado.ClienteModificado = true;
            //            }
            //            if (clienteContado.ClienteModificado)
            //            {                            
            //                cliente.CLC_Apellido1 = clienteContado.Apellido1.ToUpper().Trim();
            //                cliente.CLC_Apellido2 = clienteContado.Apellido2.ToUpper().Trim();
            //                cliente.CLC_Direccion = clienteContado.Direccion.ToUpper().Trim();
            //                cliente.CLC_Email = clienteContado.Email == null ? string.Empty : clienteContado.Email.ToUpper().Trim();
            //                cliente.CLC_Telefono = clienteContado.Telefono.ToUpper().Trim();
            //                cliente.CLC_Ocupacion = clienteContado.Ocupacion != null ? clienteContado.Ocupacion.IdOcupacion : (short)0;
            //                cliente.CLC_Nombre = clienteContado.Nombre.ToUpper().Trim();
            //                //if (clienteContado.UltimaCedulaEscaneada != null)
            //                //{
            //                //    cliente.CLC_UltimaCedulaEscaneada = clienteContado.UltimaCedulaEscaneada;
            //                //}
            //                cliente.CLC_UltimaCedulaEscaneada = clienteContado.UltimaCedulaEscaneada;
            //                CLRepositorioAudit.MapearAuditModificarClienteContado(contexto, usuarioCreacion);
            //                contexto.SaveChanges();
            //            }
            //        }
            //    }
            //    return null;
            //}
        }

        /// <summary>
        /// Adiciona el acumulado de dinero al cliente
        /// </summary>
        /// <param name="clienteContado"></param>
        /// <returns>Valor acumulado del giro</returns>
        private decimal AcumuladoGiro(CLClienteContadoDC clienteContado, decimal valorGiro)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                decimal valorAcumulado = valorGiro;
                DateTime fechaHoy = DateTime.Now.Date;
                AcumuladoGirosCliente_GIR acumuladoGiros = contexto.AcumuladoGirosCliente_GIR.FirstOrDefault(ag => ag.AGC_IdClienteContado == clienteContado.IdClienteContado);
                if (acumuladoGiros != null)
                {
                    if (acumuladoGiros.AGC_FechaUltimoAcumGiros < fechaHoy)
                        acumuladoGiros.AGC_AcumuladoDiarioGiros = valorGiro;
                    else
                    {
                        acumuladoGiros.AGC_AcumuladoDiarioGiros += valorGiro;
                        valorAcumulado = acumuladoGiros.AGC_AcumuladoDiarioGiros;
                    }

                    acumuladoGiros.AGC_FechaUltimoAcumGiros = DateTime.Now;
                    acumuladoGiros.AGC_FechaGrabacion = DateTime.Now;
                    contexto.SaveChanges();
                    return acumuladoGiros.AGC_AcumuladoDiarioGiros;
                }
                else
                {
                    AcumuladoGirosCliente_GIR nuevoAcumuladoGiros = new AcumuladoGirosCliente_GIR()
                    {
                        AGC_IdClienteContado = clienteContado.IdClienteContado,
                        AGC_CreadoPor = ControllerContext.Current.Usuario,
                        AGC_FechaGrabacion = DateTime.Now,
                        AGC_AcumuladoDiarioGiros = valorGiro,
                        AGC_FechaUltimoAcumGiros = DateTime.Now,
                        AGC_FechaUltimoAcumPagos = DateTime.Now
                    };
                    contexto.AcumuladoGirosCliente_GIR.Add(nuevoAcumuladoGiros);
                    contexto.SaveChanges();
                    return valorAcumulado;
                }
            }
        }

        /// <summary>
        /// Adiciona el acumulado de pagos dinero al cliente
        /// </summary>
        /// <param name="clienteContado"></param>
        /// <returns>Valor acumulado de los pagos</returns>
        public decimal AcumuladoPagos(CLClienteContadoDC clienteContado, decimal valorPago)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                decimal valorAcumulado = valorPago;
                DateTime fechaHoy = DateTime.Now.Date;

                long idCliente = contexto.ClienteContado_CLI.Where(cliente => cliente.CLC_Identificacion == clienteContado.Identificacion && cliente.CLC_TipoId == clienteContado.TipoId).FirstOrDefault().CLC_IdClienteContado;
                AcumuladoGirosCliente_GIR acumuladoPagos = contexto.AcumuladoGirosCliente_GIR.FirstOrDefault(ag => ag.AGC_IdClienteContado == idCliente);
                if (acumuladoPagos != null)
                {
                    if (acumuladoPagos.AGC_FechaUltimoAcumGiros < fechaHoy)
                        acumuladoPagos.AGC_AcumuladoDiarioPagos = valorPago;
                    else
                    {
                        acumuladoPagos.AGC_AcumuladoDiarioPagos += valorPago;
                        valorAcumulado = acumuladoPagos.AGC_AcumuladoDiarioPagos;
                    }

                    acumuladoPagos.AGC_FechaUltimoAcumGiros = DateTime.Now;
                    acumuladoPagos.AGC_FechaGrabacion = DateTime.Now;
                    contexto.SaveChanges();
                    return acumuladoPagos.AGC_AcumuladoDiarioPagos;
                }
                else
                {
                    AcumuladoGirosCliente_GIR nuevoAcumuladoGiros = new AcumuladoGirosCliente_GIR()
                    {
                        AGC_IdClienteContado = idCliente,
                        AGC_CreadoPor = ControllerContext.Current.Usuario,
                        AGC_FechaGrabacion = DateTime.Now,
                        AGC_AcumuladoDiarioPagos = valorPago,
                        AGC_FechaUltimoAcumGiros = DateTime.Now,
                        AGC_FechaUltimoAcumPagos = DateTime.Now
                    };
                    contexto.AcumuladoGirosCliente_GIR.Add(nuevoAcumuladoGiros);
                    contexto.SaveChanges();
                    return valorAcumulado;
                }
            }
        }

        /// <summary>
        /// Adiciona el cliente remitente y destinatario, adiciona los destinatarios frecuentes
        /// y el acumulado a cada clilente
        /// </summary>
        /// <param name="clienteContado"></param>
        /// <param name="valorGiro"></param>
        public decimal AdmGuardarClienteContado(CLClienteContadoDC clienteContadoRemitente, CLClienteContadoDC clienteContadoDestinatario, decimal valorGiro, string descUltimoCentroServDestino, long idUltimoCentroServDestino)
        {
            RegistrarClienteContado(clienteContadoRemitente, clienteContadoDestinatario, descUltimoCentroServDestino, idUltimoCentroServDestino, ControllerContext.Current.Usuario);

            return AcumuladoGiro(clienteContadoRemitente, valorGiro);
        }

        /// <summary>
        /// Almacenar a un cliente y acumular los valores
        /// </summary>
        /// <param name="clienteContadoRemitente"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        public decimal GuardarClienteAcumularValores(CLClienteContadoDC clienteContado, decimal valorGiro)
        {
            long? idCliente = AdicionarClienteContado(clienteContado, ControllerContext.Current.Usuario);
            if (idCliente != null)
            {
                clienteContado.IdClienteContado = idCliente.Value;
            }

            return AcumuladoGiro(clienteContado, valorGiro);
        }

        /// <summary>
        /// Registra destinatarios frecuentes
        /// </summary>
        /// <param name="clienteContadoRemitente"></param>
        /// <param name="clienteContadoDestinatario"></param>
        /// <param name="descUltimoCentroServDestino"></param>
        /// <param name="idUltimoCentroServDestino"></param>
        public void RegistrarClienteContado(CLClienteContadoDC clienteContadoRemitente, CLClienteContadoDC clienteContadoDestinatario, string descUltimoCentroServDestino, long idUltimoCentroServDestino, string usuarioCreacion)
        {
            if (clienteContadoRemitente.Identificacion != null)
            {
                long? idClienteRemitente = AdicionarClienteContado(clienteContadoRemitente, usuarioCreacion);

                if (idClienteRemitente != null)
                    clienteContadoRemitente.IdClienteContado = idClienteRemitente.GetValueOrDefault();
            }

            if (!string.IsNullOrWhiteSpace(clienteContadoDestinatario.Identificacion) && clienteContadoDestinatario.Identificacion != "0")
            {
                long? idClienteDestinatario = AdicionarClienteContado(clienteContadoDestinatario, usuarioCreacion);

                if (idClienteDestinatario != null)
                    clienteContadoDestinatario.IdClienteContado = idClienteDestinatario.GetValueOrDefault();
            }

            if (!string.IsNullOrWhiteSpace(clienteContadoDestinatario.Identificacion) && !string.IsNullOrWhiteSpace(clienteContadoRemitente.Identificacion) && clienteContadoDestinatario.Identificacion != "0")
            {
                AdicionarDestinatariosFrecuentes(clienteContadoRemitente, clienteContadoDestinatario, descUltimoCentroServDestino, idUltimoCentroServDestino, usuarioCreacion);
            }
        }

        /// <summary>
        /// Retorna los servicios asignados a la sucursal por el contrato
        /// </summary>
        /// <param name="idSucursal">Identificador de la sucursal del cliente</param>
        public IEnumerable<CLServiciosSucursal> ObtenerServiciosSucursalPorUnidadNegocio(int idSucursal, string idUnidadMensajeria, int idListaPrecios)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                List<paObtenerServActPorSucLisPrecio_CLI_Result> sSucursal = contexto.paObtenerServActPorSucLisPrecio_CLI((int)idSucursal, idListaPrecios, idUnidadMensajeria, null).ToList();
                List<CLServiciosSucursal> serviciosSucursal = sSucursal
                                                                    .ConvertAll<CLServiciosSucursal>(servSuc => new CLServiciosSucursal
                                                                    {
                                                                        Contrato = new CLContratosDC
                                                                        {
                                                                            IdContrato = servSuc.CON_IdContrato,
                                                                            NombreContrato = servSuc.CON_NombreContrato,
                                                                            NumeroContrato = servSuc.CON_NumeroContrato,
                                                                            ListaPrecios = servSuc.LIP_IdListaPrecios
                                                                        },
                                                                        Servicio = new Servicios.ContratoDatos.Tarifas.TAServicioDC
                                                                        {
                                                                            IdServicio = servSuc.SER_IdServicio,
                                                                            Nombre = servSuc.SER_Nombre,
                                                                            IdConceptoCaja = servSuc.SER_IdConceptoCaja,
                                                                            PesoMaximo = servSuc.SME_PesoMaximo,
                                                                            PesoMinimo = servSuc.SME_PesoMínimo,
                                                                            IdUnidadNegocio = servSuc.SER_IdUnidadNegocio
                                                                        },
                                                                        IdSucursal = servSuc.SUC_IdSucursal
                                                                    });

                if (serviciosSucursal.Count > 0)
                {
                    return serviciosSucursal;
                }
                else
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.CLIENTES, ETipoErrorFramework.EX_SUCURSAL_SIN_SERVICIOS.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_SUCURSAL_SIN_SERVICIOS));
                    throw new FaultException<ControllerException>(excepcion);
                }
            }
        }

        /// <summary>
        /// Obtiene los servicios de un contrato
        /// </summary>
        /// <param name="idContrato"></param>
        /// <returns></returns>
        public List<CLServiciosSucursal> ObtenerServiciosContrato(int idContrato)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ContratosServicios_VCLI.
                  Where(c => c.CON_IdContrato == idContrato && c.LPS_Estado == ConstantesFramework.ESTADO_ACTIVO && c.SER_Estado == ConstantesFramework.ESTADO_ACTIVO)
                  .ToList().OrderBy(s => s.UNE_Nombre).ToList()
                  .ConvertAll<CLServiciosSucursal>(s =>
                  new CLServiciosSucursal()
                  {
                      Servicio = new Servicios.ContratoDatos.Tarifas.TAServicioDC()
                      {
                          UnidadNegocio = s.UNE_Nombre,
                          Nombre = s.SER_Nombre,
                      },
                      ListaPrecio = new Servicios.ContratoDatos.Tarifas.TAListaPrecioDC()
                      {
                          PrimaSeguro = s.LPS_PrimaSeguros.ToString("##"),
                          TarifaPlena = s.LIP_EsTarifaPlena,
                          EsListaCliente = s.LIP_EsListaCliente
                      }
                  });
            }
        }

        /// <summary>
        /// Obtiene los centros de correspondencia de un contrato
        /// </summary>
        /// <param name="idContrato"></param>
        /// <returns></returns>
        public List<TAServicioCentroDeCorrespondenciaDC> ObtenerCentrosCorrespondenciaContrato(int idContrato)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ContratoCentrosCorrespondencia_VCLI.
                  Where(c => c.CON_IdContrato == idContrato && c.LIP_Estado == ConstantesFramework.ESTADO_ACTIVO && c.LPS_Estado == ConstantesFramework.ESTADO_ACTIVO && c.SER_Estado == ConstantesFramework.ESTADO_ACTIVO)
                  .ToList().OrderBy(s => s.UNE_Nombre).ToList()
                  .ConvertAll<TAServicioCentroDeCorrespondenciaDC>(s =>
                  new TAServicioCentroDeCorrespondenciaDC()
                  {
                      Descripcion = s.SCC_Descripcion,
                      Valor = s.POP_Valor
                  });
            }
        }

        /// <summary>
        /// Obtiene las facturas de un contrato
        /// </summary>
        /// <param name="idContrato"></param>
        /// <returns></returns>
        public List<CLFacturaDC> ObtenerFacturacionContrato(int idContrato)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ContratoFacturacion_VCLI.Where(c => c.PAF_Contrato == idContrato).ToList().
                   ConvertAll<CLFacturaDC>(c =>
                     new CLFacturaDC()
                     {
                         FinalCorte = c.PAF_DiaFinalCorte,
                         IdFactura = c.PAF_IdParFacturacion,
                         PlazoPago = c.PAF_PlazoPago,
                         DescFormaPago = c.PAF_FormaPago,
                         DiaPagoFactura = new CLFacturaNotacionDC()
                         {
                             IdNotacion = c.NOF_IdNotacion_DiaPagoFacturacion
                         },
                         DiaFacturacion = new CLFacturaNotacionDC()
                         {
                             IdNotacion = c.NOF_IdNotacion_DiaFacturacion
                         },

                         DiaRadicacion = new CLFacturaNotacionDC()
                         {
                             IdNotacion = c.NOF_IdNotacion_DiaRadicacion
                         },
                         LocalidadRadicacion = new PALocalidadDC()
                         {
                             Nombre = c.LOC_NombreLocRadicacion
                         },
                         NombreFactura = c.PAF_NombreFactura
                     });
            }
        }

        /// <summary>
        /// Valida que un cliente crédito pueda realizar venta de servicios y retorna  la lista de servicios habilitados
        /// </summary>
        /// <param name="idCentroServicios">Identificador del centro de servicios</param>
        /// <param name="unidadNegocio">Unidad de negocio a consultar</param>
        /// <returns>Lista de servicios ofrecidos</returns>
        public IEnumerable<CLServiciosSucursal> ObtenerServiciosPorUnidadesDeNegocio(long idSucursal, string idUnidadMensajeria, string idUnidadCarga, int idListaPrecios)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                List<paObtenerServActPorSucLisPrecio_CLI_Result> sSucursal = contexto.paObtenerServActPorSucLisPrecio_CLI((int)idSucursal, idListaPrecios, idUnidadMensajeria, idUnidadCarga).ToList();
                List<CLServiciosSucursal> serviciosSucursal = sSucursal
                                                                    .ConvertAll<CLServiciosSucursal>(servSuc => new CLServiciosSucursal
                                                                    {
                                                                        Contrato = new CLContratosDC
                                                                        {
                                                                            IdContrato = servSuc.CON_IdContrato,
                                                                            NombreContrato = servSuc.CON_NombreContrato,
                                                                            NumeroContrato = servSuc.CON_NumeroContrato,
                                                                            ListaPrecios = servSuc.LIP_IdListaPrecios
                                                                        },
                                                                        Servicio = new Servicios.ContratoDatos.Tarifas.TAServicioDC
                                                                        {
                                                                            IdServicio = servSuc.SER_IdServicio,
                                                                            Nombre = servSuc.SER_Nombre,
                                                                            IdConceptoCaja = servSuc.SER_IdConceptoCaja,
                                                                            PesoMaximo = servSuc.SME_PesoMaximo,
                                                                            PesoMinimo = servSuc.SME_PesoMínimo,
                                                                            IdUnidadNegocio = servSuc.SER_IdUnidadNegocio
                                                                        },
                                                                        IdSucursal = servSuc.SUC_IdSucursal
                                                                    });

                if (serviciosSucursal.Count > 0)
                {
                    return serviciosSucursal;
                }
                else
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.CLIENTES, ETipoErrorFramework.EX_SUCURSAL_SIN_SERVICIOS.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_SUCURSAL_SIN_SERVICIOS));
                    throw new FaultException<ControllerException>(excepcion);
                }
            }
        }

        /// <summary>
        /// Consultar la ultima cedula escaneada
        /// </summary>
        /// <returns> archivo del cliente</returns>
        public string ConsultarDocumentoCliente(string tipoId, string identificacion)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                string archivo = string.Empty;
                var archivoCliente = contexto.CliContadoArchivoCedulas_VCLI.FirstOrDefault(cliente => cliente.CLC_TipoId == tipoId && cliente.CLC_Identificacion == identificacion);
                if (archivoCliente != null)
                {
                    Byte[] docuemento = archivoCliente.ACC_Adjunto;
                    archivo = Convert.ToBase64String(docuemento);
                }

                return archivo;
            }
        }

        /// <summary>
        /// Almacenar la cedula del cliente que reclama el giro
        /// </summary>
        ///<param name="pagosGiros">informacion del pago</param>
        public void AlmacenarCedulaCliente(CLClienteContadoDC clienteContado, string archivoCedulaClientePago)
        {
            //string rutaArchivo = Path.Combine(this.filePathGiros, COConstantesModulos.GIROS, Path.GetFileName(archivoCedulaClientePago));
            //using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //    ArchivoCedulasClientes_CLI clienteArchivo = new ArchivoCedulasClientes_CLI()
            //    {
            //        //ACC_Adjunto = System.IO.File.ReadAllBytes(rutaArchivo),
            //        ACC_NombreAdjunto = CLMensajesClientes.CargarMensaje(CLEnumTipoErrorCliente.IN_ARCHIVO_CEDULA_DESTINATARIO),
            //        ACC_Descripcion = CLMensajesClientes.CargarMensaje(CLEnumTipoErrorCliente.IN_ARCHIVO_DESC_CEDULA_DESTINATARIO),
            //        ACC_IdAdjunto = Guid.NewGuid(),
            //        ACC_Usuario = ControllerContext.Current.Usuario,
            //        ACC_FechaCargaArchivo = DateTime.Now
            //    };

            //    using (FileStream fs = File.OpenRead(rutaArchivo))
            //    {
            //        byte[] bytes = new byte[fs.Length];
            //        fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
            //        fs.Close();
            //        clienteArchivo.ACC_Adjunto = bytes;
            //    }

            //    contexto.ArchivoCedulasClientes_CLI.Add(clienteArchivo);
            //    contexto.SaveChanges();
            //    clienteContado.UltimaCedulaEscaneada = clienteArchivo.ACC_IdArchivo;
            //    clienteContado.ClienteModificado = true;
            //    AdicionarClienteContado(clienteContado, ControllerContext.Current.Usuario);
            //}
        }

        /// <summary>
        /// Obtiene las sucursales activas existentes en el sistema
        /// </summary>
        /// <returns></returns>
        public List<CLSucursalDC> ObtenerSucursalesActivas()
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.SucursalesContrato_VCLI.Where(r => r.SUC_Estado == ConstantesFramework.ESTADO_ACTIVO)
                  .ToList()
                  .ConvertAll(r => new CLSucursalDC()
                  {
                      IdSucursal = r.SUC_IdSucursal,
                      Nombre = r.SUC_Nombre,
                      Ciudad = new PALocalidadDC()
                      {
                          IdLocalidad = r.SUC_Municipio,
                          Nombre = r.LOC_Nombre
                      },
                      Direccion = r.SUC_Direccion,
                      Telefono = r.SUC_Telefono,
                      Email = r.SUC_Email,
                      Agencia = r.SUC_AgenciaEncargada,
                      IdCliente = r.SUC_ClienteCredito,
                      NombreAgencia = r.SUC_AgenciaEncargada.ToString()
                  });
            }
        }

        /// <summary>
        /// Obtiene las sucursales activas de un cliente
        /// </summary>
        /// <param name="idClienteCredito"></param>
        /// <returns></returns>
        public List<CLSucursalDC> ObtenerSucursalesActivasCliente(int idClienteCredito)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.SucursalesContrato_VCLI.Where(r => r.SUC_ClienteCredito == idClienteCredito && r.SUC_Estado == ConstantesFramework.ESTADO_ACTIVO)
                  .ToList()
                  .ConvertAll(r => new CLSucursalDC()
                  {
                      IdSucursal = r.SUC_IdSucursal,
                      Nombre = r.SUC_Nombre,
                      Ciudad = new PALocalidadDC()
                      {
                          IdLocalidad = r.SUC_Municipio,
                          Nombre = r.LOC_Nombre
                      },
                      Direccion = r.SUC_Direccion,
                      Telefono = r.SUC_Telefono,
                      Email = r.SUC_Email,
                      Agencia = r.SUC_AgenciaEncargada,
                      IdCliente = r.SUC_ClienteCredito,
                      NombreAgencia = r.SUC_AgenciaEncargada.ToString()
                  });
            }
        }

        /// <summary>
        /// Obtiene las sucursales activas de un cliente por ciudad de sucursal
        /// </summary>
        /// <param name="idClienteCredito"></param>
        /// <returns></returns>
        public List<CLSucursalDC> ObtenerSucursalesXCiudadActivasCliente(int idClienteCredito, string idLocalidad)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.SucursalesContrato_VCLI.Where(r => r.SUC_ClienteCredito == idClienteCredito && r.SUC_Municipio == idLocalidad && r.SUC_Estado == ConstantesFramework.ESTADO_ACTIVO)
                  .ToList()
                  .ConvertAll(r => new CLSucursalDC()
                  {
                      IdSucursal = r.SUC_IdSucursal,
                      Nombre = r.SUC_Nombre,
                      Ciudad = new PALocalidadDC()
                      {
                          IdLocalidad = r.SUC_Municipio,
                          Nombre = r.LOC_Nombre
                      },
                      Direccion = r.SUC_Direccion,
                      Telefono = r.SUC_Telefono,
                      Email = r.SUC_Email,
                      Agencia = r.SUC_AgenciaEncargada,
                      IdCliente = r.SUC_ClienteCredito,
                      NombreAgencia = r.SUC_AgenciaEncargada.ToString() + " - " + r.CES_Nombre,
                      IdBodega = r.SUC_IdBodega,
                      IdCentroCostoAgencia = long.Parse(r.CES_IdCentroCostos)
                  });
            }
        }

        /// <summary>
        /// Obtiene las Sucursales por
        /// contrato
        /// </summary>
        /// <param name="idContrato"></param>
        /// <returns></returns>
        public List<CLSucursalDC> ObtenerSucursalesPorContrato(int idContrato)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.SucursalesContrato_VCLI.Where(r => r.SUC_Contrato == idContrato && r.SUC_Estado == ConstantesFramework.ESTADO_ACTIVO)
                  .ToList()
                  .ConvertAll(r => new CLSucursalDC()
                  {
                      IdSucursal = r.SUC_IdSucursal,
                      IdSucursalContrato = r.SUC_IdSucursalContrato,
                      Nombre = r.SUC_Nombre,
                      Ciudad = new PALocalidadDC()
                      {
                          IdLocalidad = r.SUC_Municipio,
                          Nombre = r.LOC_Nombre
                      },
                      Direccion = r.SUC_Direccion,
                      Telefono = r.SUC_Telefono,
                      Contacto = r.SUC_NombreContacto,
                      Email = r.SUC_Email,
                      Agencia = r.SUC_AgenciaEncargada,
                      IdCliente = r.SUC_ClienteCredito,
                      NombreAgencia = r.SUC_AgenciaEncargada.ToString() + " - " + r.CES_Nombre,
                      IdBodega = r.SUC_IdBodega,
                      IdCentroCostoAgencia = long.Parse(r.CES_IdCentroCostos),
                      Fecha = r.SUC_FechaGrabacion,
                      ContratoSucursal = new CLContratosDC()
                      {
                          IdSucursalContrato = r.SUC_IdSucursalContrato,
                          IdContrato = r.SUC_Contrato,
                      }
                  });
            }
        }

        /// <summary>
        /// Obtiene lo horarios de recogida de las sucursales de un contrato
        /// </summary>
        /// <param name="idSucursalContrato"></param>
        /// <returns></returns>
        public List<CLSucursalHorarioDC> ObtenerHorariosRecogidaSucursalContrato(int idSucursalContrato)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.HorarioRecogidasSucursalContrato_VCLI.Where(h => h.HRS_SucursalContrato == idSucursalContrato).ToList().ConvertAll<CLSucursalHorarioDC>(h =>
                  new CLSucursalHorarioDC()
                {
                    Hora = h.HRS_Hora,
                    IdDia = h.HRS_Dia,
                    NombreDia = h.DIA_NombreDia
                });
            }
        }

        /// <summary>
        /// Obtiene las Sucursales por
        /// contrato y ciudad
        /// </summary>
        /// <param name="idContrato"></param>
        /// <returns>lista de Sucursales</returns>
        public List<CLSucursalDC> ObtenerSucursalesPorContratoCiudad(int idContrato, string idCiudad)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.SucursalesContrato_VCLI.Where(r => r.SUC_Contrato == idContrato &&
                                                                    r.SUC_Estado == ConstantesFramework.ESTADO_ACTIVO &&
                                                                    r.SUC_Municipio == idCiudad)
                  .ToList()
                  .ConvertAll(r => new CLSucursalDC()
                  {
                      IdSucursal = r.SUC_IdSucursal,
                      Nombre = r.SUC_Nombre,
                      Ciudad = new PALocalidadDC()
                      {
                          IdLocalidad = r.SUC_Municipio,
                          Nombre = r.LOC_Nombre
                      },
                      Direccion = r.SUC_Direccion,
                      Telefono = r.SUC_Telefono,
                      Email = r.SUC_Email,
                      Agencia = r.SUC_AgenciaEncargada,
                      IdCliente = r.SUC_ClienteCredito,
                      NombreAgencia = r.SUC_AgenciaEncargada.ToString() + " - " + r.CES_Nombre,
                      IdBodega = r.SUC_IdBodega,
                      IdCentroCostoAgencia = long.Parse(r.CES_IdCentroCostos),
                      ContratoSucursal = new CLContratosDC()
                      {
                          IdSucursalContrato = r.SUC_IdSucursalContrato,
                          IdContrato = r.SUC_Contrato,
                      }
                  });
            }
        }

        /// <summary>
        /// Retorna la lista de clientes crédito que aplican a admisión peatón - convenio dadas la ciudad de origen y destino del envío.
        /// Retorna las sucursales que tienen contratos vigentes para la ciudad de destino y valida que la ciudad de origen sea permitida
        /// para el cliente crédito dado. Además valida que la lista de precios asociada al contrato esté vigente.
        /// </summary>
        /// <param name="idCiudadOrigen">Id de la ciudad de origen</param>
        /// <param name="idCiudadDestino">Id de la ciudad de destino</param>
        /// <returns></returns>
        public List<CLClienteSucConvenioNal> ObtenerClientesAplicanConvenio(string idCiudadOrigen, string idCiudadDestino)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paObtenerClientesConvenioNal_CLI1(idCiudadOrigen, idCiudadDestino)
                                  .ToList()
                                  .ConvertAll(c => new CLClienteSucConvenioNal
                                  {
                                      IdCliente = c.CLI_IdCliente,
                                      RazonSocial = c.CLI_RazonSocial,
                                      IdSucursal = c.SUC_IdSucursal,
                                      DigitoVerificacion = c.CLI_DigitoVerificacion,
                                      DireccionSucursal = c.SUC_Direccion,
                                      IdListaPrecios = c.CON_ListaPrecios,
                                      Nit = c.CLI_Nit,
                                      NombreContrato = c.CON_NombreContrato,
                                      NombreSucursal = c.SUC_Nombre,
                                      NumeroContrato = c.CON_NumeroContrato,
                                      TelefonoSucursal = c.SUC_Telefono,
                                      IdContrato = c.CON_IdContrato
                                  });
            }
        }

        #region Cliente Condiciones Giro

        /// <summary>
        /// Adiciona las condiciones para el servicio de giros para un cliente
        /// </summary>
        public void AdicionarClienteCondicionGiro(CLContratosDC contrato)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ClienteCondicionGiro_CLI nuevaCondicionGiro = new ClienteCondicionGiro_CLI()
                {
                    CCG_IdContrato = contrato.ClienteCondicionGiro.IdContrato,
                    CCG_ConvenioPagaPorte = contrato.ClienteCondicionGiro.ConvenioPagaPorte,
                    CCG_PermiteDispersion = contrato.ClienteCondicionGiro.PermiteDispersion,
                    CCG_PermiteGiroConvenio = contrato.ClienteCondicionGiro.PermiteGiroConvenio,
                    CCG_FechaGrabacion = DateTime.Now,
                    CCG_CreadoPor = ControllerContext.Current.Usuario
                };

                contexto.ClienteCondicionGiro_CLI.Add(nuevaCondicionGiro);

                if (contrato.ClienteCondicionGiro.PermiteDispersion)
                {
                    CupoDispersionCliente_CLI cupo = new CupoDispersionCliente_CLI()
                    {
                        CDC_CupoDispersionAprobado = contrato.CupoDispersionCliente.CupoDispersionAprobado,
                        CDC_IdCliente = contrato.IdCliente,
                        CDC_IdContrato = contrato.IdContrato,
                        CON_CreadoPor = ControllerContext.Current.Usuario,
                        CON_FechaGrabacion = DateTime.Now
                    };

                    contexto.CupoDispersionCliente_CLI.Add(cupo);
                }

                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Edita las condiciones para el servicio de giros para un cliente
        /// </summary>
        public void EditarClienteCondicionGiro(CLContratosDC contrato)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ClienteCondicionGiro_CLI editarCondicionGiro = contexto.ClienteCondicionGiro_CLI.Where(r => r.CCG_IdContrato == contrato.ClienteCondicionGiro.IdContrato)
                  .FirstOrDefault();

                editarCondicionGiro.CCG_ConvenioPagaPorte = contrato.ClienteCondicionGiro.ConvenioPagaPorte;
                editarCondicionGiro.CCG_PermiteDispersion = contrato.ClienteCondicionGiro.PermiteDispersion;
                editarCondicionGiro.CCG_PermiteGiroConvenio = contrato.ClienteCondicionGiro.PermiteGiroConvenio;

                CupoDispersionCliente_CLI cupoDispersionCliente = contexto.CupoDispersionCliente_CLI.Where(r => r.CDC_IdCliente == contrato.IdCliente && r.CDC_IdContrato == contrato.IdContrato).FirstOrDefault();
                if (cupoDispersionCliente != null) // Actualiza
                {
                    cupoDispersionCliente.CDC_CupoDispersionAprobado = contrato.CupoDispersionCliente.CupoDispersionAprobado;
                }
                else
                { // Inserta
                    CupoDispersionCliente_CLI cupo = new CupoDispersionCliente_CLI()
                    {
                        CDC_CupoDispersionAprobado = contrato.CupoDispersionCliente.CupoDispersionAprobado,
                        CDC_IdCliente = contrato.IdCliente,
                        CDC_IdContrato = contrato.IdContrato,
                        CON_CreadoPor = ControllerContext.Current.Usuario,
                        CON_FechaGrabacion = DateTime.Now
                    };

                    contexto.CupoDispersionCliente_CLI.Add(cupo);
                }

                contexto.SaveChanges();
            }
        }

        #endregion Cliente Condiciones Giro

        #region Sucursales

        /// <summary>
        /// Obtener las sucursales de una Agencia
        /// </summary>
        /// <param name="idAgencia">id de la agencia encargada de las sucursales</param>
        /// <returns>Sucursales de la Agencia</returns>
        public List<CLSucursalDC> ObtenerSucursalesPorIdAgencia(long idAgencia)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.SucursalesAgencia_VCLI.Where(sucursal => sucursal.SUC_AgenciaEncargada == idAgencia).ToList()
                  .ConvertAll(suc => new CLSucursalDC()
                  {
                      IdSucursal = suc.SUC_IdSucursal,
                      Nombre = suc.SUC_Nombre
                  });
            }
        }

        #endregion Sucursales
    }
}