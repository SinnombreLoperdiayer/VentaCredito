using CO.Servidor.Clientes.Comun;
using CO.Servidor.Clientes.Datos.Modelo;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.Util;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel;
using System.Transactions;

namespace CO.Servidor.Clientes.Datos
{
    /// <summary>
    /// Clase que representa el repositorio de clientes
    /// </summary>
    public partial class CLRepositorio
    {
        private string CadCnxController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;

        #region Campos

        private static readonly CLRepositorio instancia = new CLRepositorio();
        private const string NombreModelo = "ModeloClientes";
        private string conexionStringArchivo = ConfigurationManager.ConnectionStrings["ControllerArchivo"].ConnectionString;
        private string conexionStringTransaccional = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;
        private string filePath = string.Empty;

        /// <summary>
        /// Path almacena imagenes scanneadas
        /// </summary>
        private string filePathGiros = ConfigurationManager.AppSettings["Controller.RutaDescargaArchivos"];

        #endregion Campos

        #region Propiedades

        /// <summary>
        /// Retorna la instancia de la clase TARepositorio
        /// </summary>
        public static CLRepositorio Instancia
        {
            get { return CLRepositorio.instancia; }
        }

        #endregion Propiedades

        #region Consultas Generales

        /// <summary>
        /// Obtiene una lista de los clientes crédito
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CLClientesDC> ObtenerClientes()
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ClienteCredito_CLI.Where(cl => cl.CLI_Estado == ConstantesFramework.ESTADO_ACTIVO).ToList().ConvertAll(cl => new CLClientesDC
                {
                    IdCliente = cl.CLI_IdCliente,
                    RazonSocial = cl.CLI_RazonSocial,
                    Nit = cl.CLI_Nit
                });
            }
        }



        /// <summary>
        /// Obtiene una lista de los clientes crédito de una agencia específica
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CLClientesDC> ObtenerClientesxAgencia(int idAgencia)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.Sucursal_CLI.Include("ClienteCredito_CLI").Include("CentroServicios_PUA").Where(suc => suc.ClienteCredito_CLI.CLI_Estado == ConstantesFramework.ESTADO_ACTIVO && suc.CentroServicios_PUA.CES_IdCentroServicios == idAgencia).ToList().ConvertAll(cl => new CLClientesDC
                {
                    IdCliente = cl.ClienteCredito_CLI.CLI_IdCliente,
                    RazonSocial = cl.ClienteCredito_CLI.CLI_RazonSocial,
                    Nit = cl.ClienteCredito_CLI.CLI_Nit
                });
            }
        }

        /// <summary>
        /// Obtiene una lista de los clientes crédito de una localidad
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CLClientesDC> ObtenerClientesLocalidad(string idLocalidad)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ClienteCredito_CLI.Where(cli => cli.CLI_Estado == ConstantesFramework.ESTADO_ACTIVO && cli.CLI_Municipio == idLocalidad).ToList().ConvertAll(cl => new CLClientesDC
                {
                    IdCliente = cl.CLI_IdCliente,
                    RazonSocial = cl.CLI_RazonSocial,
                    Nit = cl.CLI_Nit
                });
            }
        }

        /// <summary>
        /// Obtiene una lista de los clientes crédito que van a ser usados para selecciónd e sucursal y contrato
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CLClienteCreditoSucursalContrato> ObtenerClientesCredito()
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ClientesCreditoLocalidad_VCLI.Where(cl => cl.CLI_Estado == ConstantesFramework.ESTADO_ACTIVO)
                    .ToList()
                    .ConvertAll(cl => new CLClienteCreditoSucursalContrato
                    {
                        IdCliente = cl.CLI_IdCliente,
                        RazonSocial = cl.CLI_RazonSocial,
                        Nit = cl.CLI_Nit,
                        Direccion = cl.CLI_Direccion,
                        Telefono = cl.CLI_Telefono,
                        Localidad = new PALocalidadDC { IdLocalidad = cl.CLI_Municipio, Nombre = cl.LOC_Nombre },
                    });
            }
        }

        /// <summary>
        /// Obtiene los clientes y las sucursales con certificaciones de entrega pendientes
        /// </summary>
        /// <param name="idCol"></param>
        /// <returns></returns>
        public List<CLClienteCreditoSucursalContrato> ObtenerClientesSucursalesCertificacionesEntregaPendientes(long idCol)
        {
            using (SqlConnection con = new SqlConnection(conexionStringTransaccional))
            {

                SqlCommand cmd = new SqlCommand("PaObtenerClientesSucursalesCertificaEntregaPendientes_CLI", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 120;
                cmd.Parameters.AddWithValue("@idCol", idCol);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                con.Open();
                DataTable dt = new DataTable();
                da.Fill(dt);
                con.Close();

                List<CLClienteCreditoSucursalContrato> lst = dt.AsEnumerable().GroupBy(g => g.Field<int>("CLI_IdCliente")).OrderBy(o => o.First().Field<string>("CLI_RazonSocial"))
                     .ToList().ConvertAll<CLClienteCreditoSucursalContrato>(cliente =>
                     {
                         var clienteF = cliente.First();
                         CLClienteCreditoSucursalContrato cl = new CLClienteCreditoSucursalContrato()
                         {

                             IdCliente = clienteF.Field<int>("CLI_IdCliente"),
                             NombreCliente = clienteF.Field<string>("CLI_RazonSocial") + "  (" + clienteF.Field<string>("CLI_Nit") + ")",
                             RazonSocial = clienteF.Field<string>("CLI_RazonSocial"),
                             SucursalesCliente = new List<CLSucursalDC>
                                 (
                                 dt.AsEnumerable().Where(suc => suc.Field<int>("CLI_IdCliente") == clienteF.Field<int>("CLI_IdCliente"))
                                 .GroupBy(g => g.Field<int>("SUC_IdSucursal")).ToList()
                                 .ConvertAll<CLSucursalDC>(sucursal =>
                                 {
                                     var sucursalF = sucursal.First();
                                     CLSucursalDC suc = new CLSucursalDC()
                                     {
                                         IdSucursal = sucursalF.Field<int>("SUC_IdSucursal"),
                                         Nombre = sucursalF.Field<string>("SUC_Nombre") + "  ~(" + sucursalF.Field<string>("SUC_Direccion") + ")",
                                         Direccion = sucursalF.Field<string>("SUC_Direccion"),
                                         Telefono = sucursalF.Field<string>("SUC_Telefono"),
                                         Localidad = sucursalF.Field<string>("LOC_IdLocalidad"),
                                         NombreLocalidad = sucursalF.Field<string>("LOC_Nombre"),
                                     };
                                     return suc;
                                 })
                                 )
                         };

                         return cl;
                     });

                return lst;
            }

        }


        /// <summary>
        /// Obtiene los clientes y las sucursales con certificaciones de devolucion pendientes
        /// </summary>
        /// <param name="idCol"></param>
        /// <returns></returns>
        public List<CLClienteCreditoSucursalContrato> ObtenerClientesSucursalesCertificacionesDevolucionPendientes(long idCol)
        {
            using (SqlConnection con = new SqlConnection(conexionStringTransaccional))
            {

                SqlCommand cmd = new SqlCommand("PaObtenerClientesSucursalesCertificaDevolucionPendientes_CLI", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idCol", idCol);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                con.Open();
                DataTable dt = new DataTable();
                da.Fill(dt);
                con.Close();

                List<CLClienteCreditoSucursalContrato> lst = dt.AsEnumerable().GroupBy(g => g.Field<int>("CLI_IdCliente")).OrderBy(o => o.First().Field<string>("CLI_RazonSocial"))
                     .ToList().ConvertAll<CLClienteCreditoSucursalContrato>(cliente =>
                     {
                         var clienteF = cliente.First();
                         CLClienteCreditoSucursalContrato cl = new CLClienteCreditoSucursalContrato()
                         {

                             IdCliente = clienteF.Field<int>("CLI_IdCliente"),
                             NombreCliente = clienteF.Field<string>("CLI_RazonSocial") + "  (" + clienteF.Field<string>("CLI_Nit") + ")",
                             RazonSocial = clienteF.Field<string>("CLI_RazonSocial"),
                             SucursalesCliente = new List<CLSucursalDC>
                                 (
                                 dt.AsEnumerable().Where(suc => suc.Field<int>("CLI_IdCliente") == clienteF.Field<int>("CLI_IdCliente"))
                                 .GroupBy(g => g.Field<int>("SUC_IdSucursal")).ToList()
                                 .ConvertAll<CLSucursalDC>(sucursal =>
                                 {
                                     var sucursalF = sucursal.First();
                                     CLSucursalDC suc = new CLSucursalDC()
                                     {
                                         IdSucursal = sucursalF.Field<int>("SUC_IdSucursal"),
                                         Nombre = sucursalF.Field<string>("SUC_Nombre") + "  ~(" + sucursalF.Field<string>("SUC_Direccion") + ")",
                                         Direccion = sucursalF.Field<string>("SUC_Direccion"),
                                         Telefono = sucursalF.Field<string>("SUC_Telefono"),
                                         Localidad = sucursalF.Field<string>("LOC_IdLocalidad"),
                                         NombreLocalidad = sucursalF.Field<string>("LOC_Nombre"),
                                     };
                                     return suc;
                                 })
                                 )
                         };

                         return cl;
                     });

                return lst;
            }

        }


        /// <summary>
        /// Obtiene una lista de todos los clientes crédito tienen convenio
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CLClientesDC> ObtenerTodosClientesConvenio()
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ClienteCredito_CLI.Include("Contrato_CLI").Include("Sucursal_CLI").Include("Sucursal_CLI.SucursalesContrato_CLI").Where(cl => cl.CLI_Estado == ConstantesFramework.ESTADO_ACTIVO && cl.CLI_AplicaConvenio == true)
                    .ToList()
                    .ConvertAll(cl => new CLClientesDC
                    {
                        IdCliente = cl.CLI_IdCliente,
                        RazonSocial = cl.CLI_RazonSocial,
                        Nit = cl.CLI_Nit,
                        Direccion = cl.CLI_Direccion,
                        Telefono = cl.CLI_Telefono,
                        Contratos = cl.Contrato_CLI.ToList().ConvertAll<CLContratosDC>(con =>
                        {
                            return new CLContratosDC()
                            {
                                IdContrato = con.CON_IdContrato,
                                NombreContrato = con.CON_NombreContrato,
                            };
                        }),
                        Sucursales = cl.Sucursal_CLI.ToList().ConvertAll<CLSucursalDC>(suc =>
                        {
                            return new CLSucursalDC()
                            {
                                IdSucursal = suc.SUC_IdSucursal,
                                Nombre = suc.SUC_Nombre,
                                ContratosSucursal = suc.SucursalesContrato_CLI.Where(s => s.SUC_Sucursal == suc.SUC_IdSucursal).ToList().ConvertAll<CLContratosDC>(p =>
                                {
                                    return new CLContratosDC()
                                    {
                                        IdContrato = p.SUC_Contrato
                                    };
                                })
                            };
                        })
                    });
            }
        }

        /// <summary>
        /// Consulta información de un cliente por su id
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        /*
        public CLClientesDC ObtenerCliente(int idCliente)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ClienteCredito_CLI cliente = contexto.ClienteCredito_CLI.Where(cl => cl.CLI_IdCliente == idCliente).FirstOrDefault();
                if (cliente != null)
                {
                    return new CLClientesDC()
                    {
                        IdCliente = cliente.CLI_IdCliente,
                        Nit = cliente.CLI_Nit,
                        RazonSocial = cliente.CLI_RazonSocial,
                        Direccion = cliente.CLI_Direccion,
                        Telefono = cliente.CLI_Telefono,
                        DigitoVerificacion = cliente.CLI_DigitoVerificacion,
                        Localidad = cliente.CLI_Municipio,
                        NombreGerente = cliente.CLI_NombreGerenteGeneral
                    };
                }
                else
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.CLIENTES, CLEnumTipoErrorCliente.EX_CLIENTE_NO_EXISTE.ToString(), CLMensajesClientes.CargarMensaje(CLEnumTipoErrorCliente.EX_CLIENTE_NO_EXISTE));
                    throw new FaultException<ControllerException>(excepcion);
                }
            }
        }  se elimina este metodo ya que se encuentra repetido */

        /// <summary>
        /// Consulta información de un cliente por su nit
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public CLClientesDC ObtenerClientexNit(string nit)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ClienteCredito_CLI cliente = contexto.ClienteCredito_CLI.Where(cl => cl.CLI_Nit == nit && cl.CLI_Estado == "ACT").FirstOrDefault();
                if (cliente != null)
                {
                    Localidades_VPAR localidad = contexto.Localidades_VPAR.Where(t => t.LOC_IdLocalidad == cliente.CLI_Municipio).FirstOrDefault();
                    return new CLClientesDC()
                    {
                        IdCliente = cliente.CLI_IdCliente,
                        Nit = cliente.CLI_Nit,
                        RazonSocial = cliente.CLI_RazonSocial,
                        Direccion = cliente.CLI_Direccion,
                        Telefono = cliente.CLI_Telefono,
                        DigitoVerificacion = cliente.CLI_DigitoVerificacion,
                        Localidad = cliente.CLI_Municipio,
                        NombreGerente = cliente.CLI_NombreGerenteGeneral,
                        NombreLocalidad = localidad.LOC_Nombre,
                    };
                }
                else
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.CLIENTES, CLEnumTipoErrorCliente.EX_CLIENTE_NO_EXISTE.ToString(), CLMensajesClientes.CargarMensaje(CLEnumTipoErrorCliente.EX_CLIENTE_NO_EXISTE));
                    throw new FaultException<ControllerException>(excepcion);
                }
            }
        }

        /// <summary>
        /// Retorna información del cliente crédito, si se requiere retornar sucursal y contrato se toma la primera sucursal activa y el primer contrato vigente de dicha sucursal
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="requiereSucursalContrato"></param>
        /// <returns></returns>
        public CLClienteCreditoSucursalContrato ObtenerInformacionClienteCredito(int idCliente, bool requiereSucursalContrato)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ClienteCredito_CLI cliente = contexto.ClienteCredito_CLI.Include("Sucursal_CLI.SucursalesContrato_CLI.Contrato_CLI").FirstOrDefault(cl => cl.CLI_IdCliente == idCliente);
                if (cliente != null)
                {
                    if (requiereSucursalContrato)
                    {
                        Sucursal_CLI sucursal = cliente.Sucursal_CLI.FirstOrDefault(suc => suc.SucursalesContrato_CLI != null);

                        if (sucursal != null)
                        {
                            SucursalesContrato_CLI sucursalContrato = sucursal.SucursalesContrato_CLI.FirstOrDefault(suc => suc.SUC_Estado == ConstantesFramework.ESTADO_ACTIVO && suc.Contrato_CLI.CON_FechaInicio <= DateTime.Now && suc.Contrato_CLI.CON_FechaFinConExtensiones >= DateTime.Now);
                            if (sucursalContrato != null)
                            {
                                return new CLClienteCreditoSucursalContrato()
                                {
                                    IdCliente = cliente.CLI_IdCliente,
                                    Nit = cliente.CLI_Nit,
                                    RazonSocial = cliente.CLI_RazonSocial,
                                    Direccion = sucursal.SUC_Direccion,
                                    Telefono = sucursal.SUC_Telefono,
                                    DigitoVerificacion = cliente.CLI_DigitoVerificacion,
                                    IdContrato = sucursalContrato.Contrato_CLI.CON_IdContrato,
                                    IdSucursal = sucursal.SUC_IdSucursal,
                                    IdListaPrecios = sucursalContrato.Contrato_CLI.CON_ListaPrecios
                                };
                            }
                        }

                        ControllerException excepcion = new ControllerException(COConstantesModulos.CLIENTES, CLEnumTipoErrorCliente.EX_SUCURSAL_O_CONTRATO_NO_EXISTEN.ToString(), CLMensajesClientes.CargarMensaje(CLEnumTipoErrorCliente.EX_SUCURSAL_O_CONTRATO_NO_EXISTEN));
                        throw new FaultException<ControllerException>(excepcion);
                    }
                    else
                    {
                        return new CLClienteCreditoSucursalContrato()
                        {
                            IdCliente = cliente.CLI_IdCliente,
                            Nit = cliente.CLI_Nit,
                            RazonSocial = cliente.CLI_RazonSocial,
                            Direccion = cliente.CLI_Direccion,
                            Telefono = cliente.CLI_Telefono,
                            DigitoVerificacion = cliente.CLI_DigitoVerificacion
                        };
                    }
                }
                else
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.CLIENTES, CLEnumTipoErrorCliente.EX_CLIENTE_NO_EXISTE.ToString(), CLMensajesClientes.CargarMensaje(CLEnumTipoErrorCliente.EX_CLIENTE_NO_EXISTE));
                    throw new FaultException<ControllerException>(excepcion);
                }
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
        public CLClienteCreditoSucursalContrato ObtenerInfoClienteCreditoContratos(int idCliente)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ClienteCredito_CLI cliente = contexto.ClienteCredito_CLI.Include("Sucursal_CLI.SucursalesContrato_CLI.Contrato_CLI").FirstOrDefault(cl => cl.CLI_IdCliente == idCliente);
                if (cliente != null)
                {
                    List<CLSucursalDC> sucursal = cliente.Sucursal_CLI.Where(suc => suc.SucursalesContrato_CLI != null && suc.SucursalesContrato_CLI.Count > 0).ToList().ConvertAll
                      (cliSuc => new CLSucursalDC()
                      {
                          IdSucursal = cliSuc.SUC_IdSucursal,
                          Nombre = cliSuc.SUC_Nombre,
                          ContratosSucursal = cliSuc.SucursalesContrato_CLI.Where(suc => suc.SUC_Estado == ConstantesFramework.ESTADO_ACTIVO && suc.Contrato_CLI.CON_FechaInicio <= DateTime.Now && suc.Contrato_CLI.CON_FechaFinConExtensiones >= DateTime.Now).ToList().ConvertAll(contr =>
                          new CLContratosDC()
                          {
                              IdContrato = contr.Contrato_CLI.CON_IdContrato,
                              NombreContrato = contr.Contrato_CLI.CON_NombreContrato,
                              NumeroContrato = contr.Contrato_CLI.CON_NumeroContrato
                          })
                      });

                    if (sucursal == null)
                    {
                        ControllerException excepcion = new ControllerException(COConstantesModulos.CLIENTES, CLEnumTipoErrorCliente.EX_SUCURSAL_O_CONTRATO_NO_EXISTEN.ToString(), CLMensajesClientes.CargarMensaje(CLEnumTipoErrorCliente.EX_SUCURSAL_O_CONTRATO_NO_EXISTEN));
                        throw new FaultException<ControllerException>(excepcion);
                    }

                    return new CLClienteCreditoSucursalContrato()
                    {
                        IdCliente = cliente.CLI_IdCliente,
                        Nit = cliente.CLI_Nit,
                        NombreCliente = cliente.CLI_RazonSocial,
                        SucursalesCliente = sucursal
                    };
                }
                else
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.CLIENTES, CLEnumTipoErrorCliente.EX_CLIENTE_NO_EXISTE.ToString(), CLMensajesClientes.CargarMensaje(CLEnumTipoErrorCliente.EX_CLIENTE_NO_EXISTE));
                    throw new FaultException<ControllerException>(excepcion);
                }
            }
        }

        /// <summary>
        /// Obtiene una lista con los clientes para filtrar
        /// </summary>
        /// <returns>Colección con los clientes configuradas en la base de datos</returns>
        public IEnumerable<CLClientesDC> ObtenerClientesFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            if (filtro.ContainsKey("CLI_RazonSocial"))
            {
                filtro["CLI_RazonSocial"] = filtro["CLI_RazonSocial"].Trim();
            }

            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ConsultarContainsClientesCreditoLocalidad_VCLI(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                .ToList().ConvertAll<CLClientesDC>(r =>
                {
                    Localidades_VPAR localidad = contexto.Localidades_VPAR.Where(t => t.LOC_IdLocalidad == r.CLI_Municipio).Single();

                    CLClientesDC propi = new CLClientesDC
                    {
                        IdCliente = r.CLI_IdCliente,
                        DigitoVerificacion = r.CLI_DigitoVerificacion.Trim(),
                        Direccion = r.CLI_Direccion,
                        NombreEstado = contexto.MotivoEstados_PAR.Where(t => t.MOI_Estado == r.CLI_Estado).First().MOI_DescripcionEstado,
                        Fax = r.CLI_Fax,
                        FechaConstitucion = r.CLI_FechaConstitucion,
                        FechaVinculacion = r.CLI_FechaVinculacion,
                        IdRepresentanteLegal = r.CLI_IdRepresentanteLegal,
                        Localidad = r.CLI_Municipio,
                        NombreLocalidad = r.LOC_Nombre,
                        Nit = r.CLI_Nit,
                        NombreGerente = r.CLI_NombreGerenteGeneral,
                        RazonSocial = r.CLI_RazonSocial,
                        SegmentoMercado = r.CLI_SegmentoMercado,
                        Telefono = r.CLI_Telefono,
                        TipoSociedad = r.CLI_TipoSociedad,
                        TopeMaximoCredito = r.CLI_TopeMaximoCredito,
                        ActividadEconomica = new PATipoActEconomica
                        {
                            IdTipoActEconomica = r.CLI_ActividadEconomica,
                            Descripcion = contexto.TipoActividadEconomica_PAR.Where(t => t.TAE_IdActividad == r.CLI_ActividadEconomica).Single().TAE_Descripcion,
                        },
                        NombreMunicipio = localidad.LOC_Nombre,
                        IdDepto = localidad.LOC_IdAncestroPrimerGrado,
                        NombreDepto = localidad.LOC_NombrePrimero,
                        IdPais = localidad.LOC_IdAncestroSegundoGrado,
                        NombrePais = localidad.LOC_NombreSegundo,
                        NombreRegimenContributivo = contexto.TipoRegimenContributivo_PAR.Where(t => t.TRC_IdRegimen == r.CLI_RegimenContributivo).Single().TRC_Descripcion,
                        NombreTipoSociedad = contexto.TipoSociedad_PAR.Where(t => t.TIS_IdTipoSociedad == r.CLI_TipoSociedad).Single().TIS_Descripcion,
                        NombreSegmentoMercado = contexto.TipoSegmentoMercado_PAR.Where(t => t.TSM_IdSegmento == r.CLI_SegmentoMercado).Single().TSM_Descripcion,
                        NombreRepresentanteLegal = contexto.PersonaResponsableLegal_PAR.Include("PersonaExterna_PAR").Where(t => t.PRL_IdPersonaExterna == r.CLI_IdRepresentanteLegal).Select(t => t.PersonaExterna_PAR.PEE_PrimerNombre + " " + t.PersonaExterna_PAR.PEE_PrimerApellido).Single(),
                        RegimenContributivo = r.CLI_RegimenContributivo,
                        Estado = r.CLI_Estado,
                        IdTipoSectorCliente = r.CLI_IdTipoSectorCliente,
                        ClienteConvenio = !r.CLI_AplicaConvenio.HasValue ? false : r.CLI_AplicaConvenio.Value,
                        IdTipoClienteCredito = r.CLI_IdTipoClienteCredito
                    };
                    return propi;
                }).ToList();
            }
        }

        /// <summary>
        /// Obtiene las sucursales del cliente por racol
        /// </summary>
        /// <param name="idRacol">Id del racol</param>
        /// <param name="idCliente">id del cliente</param>
        /// <returns>Lista de las sucursales del cliente y del racol</returns>
        public List<CLSucursalDC> ObtenerSucursalesClienteRacol(long idRacol, long idCliente)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.SucursalesClienteRacol_VCLI.Where(r => r.CEL_IdRegionalAdm == idRacol && r.SUC_ClienteCredito == idCliente)
                  .ToList()
                  .ConvertAll(r => new CLSucursalDC()
                  {
                      IdSucursal = r.SUC_IdSucursal,
                      Nombre = r.SUC_Nombre,
                      Direccion = r.SUC_Direccion,
                      Telefono = r.SUC_Telefono,
                      Localidad = r.SUC_Municipio,
                      IdBodega = r.SUC_IdBodega,
                      NombreLocalidad = r.LOC_Nombre,
                      IdCliente = Convert.ToInt32(idCliente)
                  });
            }
        }

        public List<CLSucursalDC> ObtenerSucursalesClientesCredito(DateTime fechaInicial, DateTime fechaFinal, string idMensajero,int idEstado)
        {
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerSucursalesClientesCredito_CLI", sqlConn);
                List<CLSucursalDC> sucursales = new List<CLSucursalDC>();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@fechaInicial", fechaInicial);
                cmd.Parameters.AddWithValue("@fechaFinal", fechaFinal);
                cmd.Parameters.AddWithValue("@IdEstado", idEstado);
                if (!string.IsNullOrEmpty(idMensajero))
                {
                    cmd.Parameters.AddWithValue("@IdMensajero", idMensajero);
                }
                sqlConn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    sucursales.Add(new CLSucursalDC()
                    {
                        IdSucursal = Convert.ToInt32(reader["SUC_IdSucursal"]),
                        Nombre = reader["SUC_Nombre"].ToString(),
                        IdCliente = Convert.ToInt32(reader["SUC_ClienteCredito"]),
                        Agencia = Convert.ToInt64(reader["SUC_AgenciaEncargada"]),
                        Ciudad = new PALocalidadDC() { IdLocalidad = reader["SUC_Municipio"].ToString() },
                        IdBodega = reader["SUC_IdBodega"].ToString(),
                        LatitudSucursal= Convert.ToDecimal(reader["SUC_latitud"]),
                        LongitudSucursal= Convert.ToDecimal(reader["SUC_longitud"]),
                        Direccion= reader["SUC_Direccion"].ToString(),
                        Telefono= reader["SUC_Telefono"].ToString(),
                        Fax= reader["SUC_Fax"].ToString(),
                        Email= reader["SUC_Email"].ToString(),
                        Contacto= reader["SUC_NombreContacto"].ToString(),
                        EstadoRecogida = Convert.ToInt32(reader["EstadoRecogida"]),
                        FechaHoraRecogida = Convert.ToDateTime(reader["SRE_FechaHoraRecogida"])
                    });

                }
                sqlConn.Close();
                return sucursales;
            }
        }

        /// <summary>
        /// Retorna la información básica de una sucursal
        /// </summary>
        /// <param name="idSucursal"></param>
        /// <returns></returns>
        //public CLSucursalDC ObtenerSucursalCliente(int idSucursal, CLClientesDC cliente)
        //{
        //    //using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
        //    using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
        //    {
        //        SqlCommand cmd = new SqlCommand("paObtenerSucursalCliente_CLI", sqlConn);
        //        CLSucursalDC sucursal = new CLSucursalDC();
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@idSucursal", idSucursal);
        //        sqlConn.Open();
        //        SqlDataReader reader = cmd.ExecuteReader();
        //        while (reader.Read())
        //        {
        //            cliente.IdCliente = Convert.ToInt32(reader["CLI_IdCliente"]);
        //            cliente.Telefono = reader["CLI_Telefono"].ToString();
        //            cliente.Direccion = reader["CLI_Direccion"].ToString();
        //            cliente.RazonSocial = reader["CLI_RazonSocial"].ToString();
        //            sucursal.IdSucursal = Convert.ToInt32(reader["SUC_IdSucursal"]);
        //            sucursal.Nombre = reader["SUC_Nombre"].ToString();
        //            sucursal.IdCliente = Convert.ToInt32(reader["SUC_ClienteCredito"]);
        //            sucursal.Agencia = Convert.ToInt64(reader["SUC_AgenciaEncargada"]);
        //            sucursal.Ciudad = new PALocalidadDC() { IdLocalidad = reader["SUC_Municipio"].ToString() };
        //            sucursal.IdBodega = reader["SUC_IdBodega"].ToString();

        //        }
        //        sqlConn.Close();
        //        return sucursal;

        //    }
        //}

        /// <summary>
        /// Retorna la información básica de una sucursal
        /// </summary>
        /// <param name="idSucursal"></param>
        /// <returns></returns>
        public CLSucursalDC ObtenerSucursalCliente(int idSucursal, CLClientesDC cliente)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Sucursal_CLI sucursal = contexto.Sucursal_CLI.Include("ClienteCredito_CLI").FirstOrDefault(suc => suc.SUC_IdSucursal == idSucursal);

                if (sucursal != null)
                {
                    cliente.IdCliente = sucursal.ClienteCredito_CLI.CLI_IdCliente;
                    cliente.Telefono = sucursal.ClienteCredito_CLI.CLI_Telefono;
                    cliente.Direccion = sucursal.ClienteCredito_CLI.CLI_Direccion;
                    cliente.RazonSocial = sucursal.ClienteCredito_CLI.CLI_RazonSocial;

                    return new CLSucursalDC()
                    {
                        IdSucursal = sucursal.SUC_IdSucursal,
                        Nombre = sucursal.SUC_Nombre,
                        IdCliente = sucursal.SUC_ClienteCredito,
                        Agencia = sucursal.SUC_AgenciaEncargada,
                        Ciudad = new PALocalidadDC() { IdLocalidad = sucursal.SUC_Municipio },
                        IdBodega = sucursal.SUC_IdBodega
                    };
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Metodo para agregar un nuevo usuario
        /// </summary>
        /// <param name="cliente"></param>
        public int AdicionarCliente(CLClientesDC cliente)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ClienteCredito_CLI dato = new ClienteCredito_CLI()
                {
                    CLI_ActividadEconomica = (short)cliente.ActividadEconomica.IdTipoActEconomica,
                    CLI_DigitoVerificacion = cliente.DigitoVerificacion.Trim(),
                    CLI_Direccion = cliente.Direccion,
                    CLI_Estado = cliente.Estado,
                    CLI_Fax = cliente.Fax,
                    CLI_FechaConstitucion = cliente.FechaConstitucion,
                    CLI_FechaVinculacion = cliente.FechaVinculacion,
                    CLI_IdRepresentanteLegal = cliente.IdRepresentanteLegal,
                    CLI_Municipio = cliente.Localidad,
                    CLI_Nit = cliente.Nit.Trim(),
                    CLI_NombreGerenteGeneral = cliente.NombreGerente,
                    CLI_RazonSocial = cliente.RazonSocial,
                    CLI_RegimenContributivo = cliente.RegimenContributivo,
                    CLI_SegmentoMercado = cliente.SegmentoMercado,
                    CLI_Telefono = cliente.Telefono,
                    CLI_TipoSociedad = cliente.TipoSociedad,
                    CLI_TopeMaximoCredito = cliente.TopeMaximoCredito,
                    CLI_FechaGrabacion = DateTime.Now,
                    CLI_CreadoPor = ControllerContext.Current.Usuario,
                    CLI_IdTipoSectorCliente = (short)cliente.IdTipoSectorCliente,
                    CLI_AplicaConvenio = cliente.ClienteConvenio,
                    CLI_IdTipoClienteCredito = (int)cliente.IdTipoClienteCredito
                };
                EstadosCliente_CLI listaEn = new EstadosCliente_CLI()
                {
                    ESC_Estado = cliente.EstadoCliente.Estado,
                    ESC_Motivo = cliente.EstadoCliente.Motivo,
                    ESC_Observaciones = cliente.EstadoCliente.Observaciones,
                    ESC_FechaGrabacion = DateTime.Now,
                    ESC_CreadoPor = ControllerContext.Current.Usuario
                };
                contexto.ClienteCredito_CLI.Add(dato);
                contexto.SaveChanges();
                return dato.CLI_IdCliente;
            }
        }

        /// <summary>
        /// Metodo para modificar un cliente
        /// </summary>
        /// <param name="cliente"></param>
        public void ModificarCliente(CLClientesDC cliente)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var dato = contexto.ClienteCredito_CLI
               .Where(r => r.CLI_IdCliente == cliente.IdCliente)
               .FirstOrDefault();
                dato.CLI_Nit = cliente.Nit;
                dato.CLI_ActividadEconomica = (short)cliente.ActividadEconomica.IdTipoActEconomica;
                dato.CLI_DigitoVerificacion = cliente.DigitoVerificacion;
                dato.CLI_Direccion = cliente.Direccion;
                dato.CLI_Estado = cliente.Estado;
                dato.CLI_Fax = cliente.Fax;
                dato.CLI_FechaConstitucion = cliente.FechaConstitucion;
                dato.CLI_FechaVinculacion = cliente.FechaVinculacion;
                dato.CLI_IdRepresentanteLegal = cliente.IdRepresentanteLegal;
                dato.CLI_Municipio = cliente.Localidad;
                dato.CLI_NombreGerenteGeneral = cliente.NombreGerente;
                dato.CLI_RazonSocial = cliente.RazonSocial;
                dato.CLI_RegimenContributivo = cliente.RegimenContributivo;
                dato.CLI_SegmentoMercado = cliente.SegmentoMercado;
                dato.CLI_Telefono = cliente.Telefono;
                dato.CLI_TipoSociedad = cliente.TipoSociedad;
                dato.CLI_TopeMaximoCredito = cliente.TopeMaximoCredito;
                dato.CLI_IdTipoSectorCliente = (short)cliente.IdTipoSectorCliente;
                dato.CLI_AplicaConvenio = cliente.ClienteConvenio;
                dato.CLI_IdTipoClienteCredito = cliente.IdTipoClienteCredito;
                CLRepositorioAudit.MapearAuditModificarCliente(contexto);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Metodo para obtener cupo maximo de credito por cliente
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public decimal ObtenerCupoMaximoCliente(int idCliente)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ClienteCredito_CLI
                  .Where(r => r.CLI_IdCliente == idCliente)
                  .Single().CLI_TopeMaximoCredito;
            }
        }


        #endregion Consultas Generales

        #region Estadoscliente

        /// <summary>
        /// Obtiene lista con los estados de un cliente
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CLEstadosClienteDC> ObtenerEstadosCliente(CLClientesDC cliente)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.EstadosCliente_CLI
                  .Where(t => t.ESC_IdCliente == cliente.IdCliente)
                  .OrderByDescending(o => o.ESC_FechaGrabacion)
                  .ToList()
                  .ConvertAll<CLEstadosClienteDC>(r => new CLEstadosClienteDC()
                  {
                      Estado = r.ESC_Estado,
                      EstadoDescripcion = contexto.MotivoEstados_PAR.Where(t => t.MOI_Estado == r.ESC_Estado && t.MOI_Motivo == r.ESC_Motivo).Single().MOI_DescripcionEstado,
                      Fecha = r.ESC_FechaGrabacion,
                      IdCliente = r.ESC_IdCliente,
                      IdEstadoCliente = r.ESC_IdEstado,
                      Motivo = r.ESC_Motivo,
                      MotivoDescripcion = contexto.MotivoEstados_PAR.Where(t => t.MOI_Estado == r.ESC_Estado && t.MOI_Motivo == r.ESC_Motivo).Single().MOI_DescripcionMotivo,
                      Observaciones = r.ESC_Observaciones
                  });
            }
        }

        /// <summary>
        /// Guarda el estado de un cliente
        /// </summary>
        /// <param name="EstadosCliente"></param>
        public void AdicionarEstadosCliente(CLClientesDC cliente)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                EstadosCliente_CLI listaEn = new EstadosCliente_CLI()
                {
                    ESC_Estado = cliente.Estado,
                    ESC_IdCliente = cliente.IdCliente,
                    ESC_Motivo = cliente.EstadoCliente.Motivo,
                    ESC_Observaciones = cliente.EstadoCliente.Observaciones,
                    ESC_FechaGrabacion = DateTime.Now,
                    ESC_CreadoPor = ControllerContext.Current.Usuario
                };
                contexto.EstadosCliente_CLI.Add(listaEn);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Obtiene lista con los estados
        /// </summary>
        /// <returns></returns>
        public IEnumerable<EstadoDC> ObtenerEstados()
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return
                  contexto.MotivoEstados_PAR
                  .ToList()
                  .GroupBy(j => j.MOI_Estado)
                  .ToList()
                  .ConvertAll<EstadoDC>(r => new EstadoDC()
                  {
                      IdEstado = r.Key,
                      EstadoDescripcion = r.First().MOI_DescripcionEstado,
                  });
            }
        }

        /// <summary>
        /// Obtiene lista con los estados y motivos
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CLMotivoEstadosDC> ObtenerMotivosEstados()
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.MotivoEstados_PAR.OrderBy(o => o.MOI_Estado).ToList().ConvertAll<CLMotivoEstadosDC>(r => new CLMotivoEstadosDC()
                {
                    Estado = r.MOI_Estado,
                    EstadoDescripcion = r.MOI_DescripcionEstado,
                    Motivo = r.MOI_Motivo,
                    MotivoDescripcion = r.MOI_DescripcionMotivo
                });
            }
        }

        public IEnumerable<CLTipoClienteCreditoDC> ObtenerTipoClienteCredito()
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TipoClienteCredito_CLI.OrderBy(o => o.TPC_Descripcion).ToList().ConvertAll<CLTipoClienteCreditoDC>(r => new CLTipoClienteCreditoDC()
                {
                    IdTipoClienteCredito = r.TPC_IdTipoClienteCredito,
                    Descripcion = r.TPC_Descripcion,
                    FechaGrabacion = r.TPC_FechaGrabacion,
                    CreadoPor = r.TPC_CreadoPor,
                });
            }

        }

        #endregion Estadoscliente

        #region Archivos

        /// <summary>
        /// Obtiene lista con los archivos de un cliente
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CLArchivosDC> ObtenerArchivosCliente(CLClientesDC cliente)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var listacliente = contexto.DocumentosClienteCredito_VCLI
                  .Where(t => t.ACC_IdClienteCredito == cliente.IdCliente && t.DCC_Estado != ConstantesFramework.ESTADO_INACTIVO)
                  .OrderBy(o => o.DCC_IdDocumento)
                  .ToList()
                  .ConvertAll<CLArchivosDC>(r => new CLArchivosDC()
                  {
                      IdCliente = r.ACC_IdClienteCredito,
                      IdArchivo = r.ACC_IdArchivo,
                      IdDocumento = r.DCC_IdDocumento,
                      NombreDocumento = r.DCC_Nombre,
                      EstadoDocumento = r.DCC_Estado,
                      Fecha = r.ACC_FechaGrabacion,
                      IdAdjunto = r.ACC_IdAdjunto,
                      NombreAdjunto = r.ACC_NombreAdjunto,
                      EstadoRegistro = Framework.Servidor.Comun.EnumEstadoRegistro.SIN_CAMBIOS
                  });

                var listadocumentos = contexto.DocumentosClienteCredito_CLI
                .Where(t => t.DCC_Estado != ConstantesFramework.ESTADO_INACTIVO)
               .OrderBy(o => o.DCC_IdDocumento)
                .ToList()
                .ConvertAll<CLArchivosDC>(r => new CLArchivosDC()
                {
                    IdDocumento = r.DCC_IdDocumento,
                    NombreDocumento = r.DCC_Nombre,
                    EstadoDocumento = r.DCC_Estado,
                });

                foreach (CLArchivosDC documento in listadocumentos)
                {
                    foreach (CLArchivosDC archivo in listacliente)
                    {
                        if (documento.IdDocumento == archivo.IdDocumento)
                        {
                            documento.IdAdjunto = archivo.IdAdjunto;
                            documento.IdArchivo = archivo.IdArchivo;
                            documento.IdCliente = archivo.IdCliente;
                            documento.NombreAdjunto = archivo.NombreAdjunto;
                            documento.Fecha = archivo.Fecha;
                            documento.EstadoRegistro = archivo.EstadoRegistro;
                        }
                    }
                }

                return listadocumentos;
            }
        }

        /// <summary>
        /// Adiciona archivo de un cliente
        /// </summary>
        /// <param name="archivo">objeto de tipo archivo</param>
        public void AdicionarArchivoCliente(CLArchivosDC archivo)
        {
            filePath = PAAdministrador.Instancia.ObtenerParametrosAplicacion(PAAdministrador.RutaArchivos);
            string rutaArchivo = Path.Combine(this.filePath, COConstantesModulos.CLIENTES, archivo.NombreServidor);
            byte[] archivoImagen;
            using (FileStream fs = File.OpenRead(rutaArchivo))
            {
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
                fs.Close();
                archivoImagen = bytes;
            }

            string query = "INSERT INTO [ArchivosClienteCredito_CLI] WITH (ROWLOCK)" +
            " ([ACC_Adjunto] ,[ACC_IdClienteCredito]  ,[ACC_IdAdjunto]  ,[ACC_NombreAdjunto] ,[ACC_FechaGrabacion] ,[ACC_CreadoPor], [ACC_IdDocumento])  " +
           " VALUES(@Adjunto ,@IdClienteCredito ,@IdAdjunto,@NombreAdjunto ,GETDATE() ,@CreadoPor, @IdDocumento)";

            using (SqlConnection sqlConn = new SqlConnection(conexionStringArchivo))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand(query, sqlConn);
                cmd.Parameters.Add(new SqlParameter("@NombreAdjunto", archivo.NombreAdjunto));
                cmd.Parameters.Add(new SqlParameter("@IdAdjunto", Guid.NewGuid()));
                cmd.Parameters.Add(new SqlParameter("@IdClienteCredito", archivo.IdCliente.Value));
                cmd.Parameters.Add(new SqlParameter("@Adjunto", (object)archivoImagen));
                cmd.Parameters.Add(new SqlParameter("@CreadoPor", ControllerContext.Current.Usuario));
                cmd.Parameters.Add(new SqlParameter("@IdDocumento", archivo.IdDocumento));
                cmd.ExecuteNonQuery();
                sqlConn.Close();
            }
        }

        /// <summary>
        /// Elimina archivo de un cliente
        /// </summary>
        /// <param name="archivo">objeto de tipo archivo</param>
        public void EliminarArchivoCliente(CLArchivosDC archivo)
        {
            string query = "DELETE FROM [ArchivosClienteCredito_CLI] WITH (ROWLOCK)" +
        "WHERE  ACC_IdArchivo = " + archivo.IdArchivo;

            using (SqlConnection sqlConn = new SqlConnection(conexionStringArchivo))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand(query, sqlConn);
                cmd.ExecuteNonQuery();
                sqlConn.Close();
            }
        }

        /// <summary>
        ///Metodo para obtener un archivo asociado a un cliente
        /// </summary>
        /// <param name="archivo"></param>
        /// <returns></returns>
        public string ObtenerArchivoCliente(CLArchivosDC archivo)
        {
            string respuesta;
            string query = "SELECT * FROM dbo.ArchivosClienteCredito_CLI WHERE ACC_IdArchivo = " + archivo.IdArchivo;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringArchivo))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand(query, sqlConn);
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                if (dt.Rows.Count == 0)
                {
                    var x = new ControllerException
                         (
                         COConstantesModulos.CLIENTES,
                         CLEnumTipoErrorCliente.EX_FALLO_ADJUNTAR_ARCHIVO.ToString(),
                         CLMensajesClientes.CargarMensaje(CLEnumTipoErrorCliente.EX_FALLO_ADJUNTAR_ARCHIVO)
                         );
                    throw new FaultException<ControllerException>(x);
                }
                else
                    respuesta = Convert.ToBase64String(dt.Rows[0]["ACC_Adjunto"] as byte[]);

                sqlConn.Close();
                return respuesta;
            }
        }

        #endregion Archivos

        #region Divulgación de cliente

        public CLClientesDC ObtenerClienteDivulgacion(int idCliente)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ClienteCredito_CLI cliente = contexto.ClienteCredito_CLI
                  .Where(r => r.CLI_IdCliente == idCliente)
                  .Single();

                if (cliente == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.CLIENTES, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }
                Localidades_VPAR localidad = contexto.Localidades_VPAR.Where(t => t.LOC_IdLocalidad == cliente.CLI_Municipio).Single();

                CLClientesDC Cliente = new CLClientesDC()
                {
                    IdCliente = cliente.CLI_IdCliente,
                    ActividadEconomica = new PATipoActEconomica
                    {
                        IdTipoActEconomica = cliente.CLI_ActividadEconomica,
                        Descripcion = contexto.TipoActividadEconomica_PAR.Where(t => t.TAE_IdActividad == cliente.CLI_ActividadEconomica).Single().TAE_Descripcion,
                    },
                    DigitoVerificacion = cliente.CLI_DigitoVerificacion,
                    Direccion = cliente.CLI_Direccion,
                    NombreEstado = contexto.MotivoEstados_PAR.Where(t => t.MOI_Estado == cliente.CLI_Estado).First().MOI_DescripcionEstado,
                    Fax = cliente.CLI_Fax,
                    FechaConstitucion = cliente.CLI_FechaConstitucion,
                    FechaVinculacion = cliente.CLI_FechaVinculacion,
                    Nit = cliente.CLI_Nit,
                    NombreGerente = cliente.CLI_NombreGerenteGeneral,
                    RazonSocial = cliente.CLI_RazonSocial,
                    Telefono = cliente.CLI_Telefono,
                    TopeMaximoCredito = cliente.CLI_TopeMaximoCredito,
                    NombreMunicipio = localidad.LOC_Nombre,
                    NombreRegimenContributivo = contexto.TipoRegimenContributivo_PAR.Where(t => t.TRC_IdRegimen == cliente.CLI_RegimenContributivo).Single().TRC_Descripcion,
                    NombreTipoSociedad = contexto.TipoSociedad_PAR.Where(t => t.TIS_IdTipoSociedad == cliente.CLI_TipoSociedad).Single().TIS_Descripcion,
                    NombreSegmentoMercado = contexto.TipoSegmentoMercado_PAR.Where(t => t.TSM_IdSegmento == cliente.CLI_SegmentoMercado).Single().TSM_Descripcion,
                    IdRepresentanteLegal = cliente.CLI_IdRepresentanteLegal
                };
                return Cliente;
            }
        }

        public List<CLSucursalDC> ObtenerSucursalesDivulgacion(int idCliente)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.Sucursal_CLI
                  .Where(s => s.SUC_ClienteCredito == idCliente)
                  .ToList()
                  .ConvertAll<CLSucursalDC>(r =>
                  {
                      Localidades_VPAR localidad = contexto.Localidades_VPAR.Where(t => t.LOC_IdLocalidad == r.SUC_Municipio).FirstOrDefault();

                      CLSucursalDC propi = new CLSucursalDC
                      {
                          IdCliente = r.SUC_ClienteCredito,
                          Direccion = r.SUC_Direccion,
                          Telefono = r.SUC_Telefono,
                          Nombre = r.SUC_Nombre,
                          IdSucursal = r.SUC_IdSucursal,
                          Localidad = r.SUC_Municipio,
                          NombreLocalidad = localidad.LOC_Nombre,
                          IdBodega = r.SUC_IdBodega
                      };
                      return propi;
                  });
            }
        }

        public List<CLContratosDC> ObtenerContratosDivulgacion(int idCliente)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.Contrato_CLI.Join(contexto.SucursalesContrato_CLI, con => con.CON_IdContrato, sucCon => sucCon.SUC_Contrato, (con, sucCon) => new { contr = con, sucContra = sucCon }).ToList()
                  .Where(s => s.contr.CON_ClienteCredito == idCliente && s.sucContra.SUC_Estado == ConstantesFramework.ESTADO_ACTIVO)
                  .ToList()
                  .ConvertAll<CLContratosDC>(r =>
                  {
                      CLContratosDC propi = new CLContratosDC
                      {
                          IdCliente = r.contr.CON_ClienteCredito,
                          EjecutivoCuenta = r.contr.CON_EjecutivoCuenta,
                          FechaFinal = r.contr.CON_FechaFin,
                          FechaInicial = r.contr.CON_FechaInicio,
                          IdContrato = r.contr.CON_IdContrato,
                          ListaPrecios = r.contr.CON_ListaPrecios,
                          NombreContrato = r.contr.CON_NombreContrato,
                          NombreGestor = r.contr.CON_NombreGestorPago,
                          NombreInterventor = r.contr.CON_NombreInterventor,
                          NumeroContrato = r.contr.CON_NumeroContrato,
                          ObjetoContrato = r.contr.CON_ObjetoContrato,
                          PorcentajeAviso = r.contr.CON_PorcentajeAviso,
                          PresupuestoMensual = r.contr.CON_PresupuestoMensual,
                          TelefonoGestor = r.contr.CON_TelefonoGestorPago,
                          TelefonoInterventor = r.contr.CON_TelefonoInterventor,
                          SupervisorCuenta = r.contr.CON_SupervisorCuenta,
                          FechaFinalExtension = r.contr.CON_FechaFinConExtensiones,
                          Valor = r.contr.CON_ValorContrato,
                          NombreEjecutivo = PAParametros.Instancia.ObtenerPersonaInterna(r.contr.CON_EjecutivoCuenta).NombreCompleto,
                          NombreSupervisor = PAParametros.Instancia.ObtenerPersonaInterna(r.contr.CON_SupervisorCuenta).NombreCompleto,
                          NumeroAsignacion = r.contr.CON_NumeroAsignacionPresupuest,
                          CertificadoDisponibilidad = r.contr.CON_CertificadoDisponibilidadPropuestal,
                          NumeroRegistroDisponibilidad = r.contr.CON_NumeroRegisroDisponibilidad,
                          ValorDisponibilidad = r.contr.CON_ValorDisponibilidad
                      };
                      return propi;
                  });
            }
        }

        #endregion Divulgación de cliente

        #region Sucursales

        /// <summary>
        /// Retorna el centro de servicios que administra la sucursal pasada como parámetro
        /// </summary>
        /// <param name="idSucursal">Identificador de la sucursal</param>
        /// <returns></returns>
        public CO.Servidor.Servicios.ContratoDatos.CentroServicios.PUCentroServiciosDC ObtenerCentroServiciosAdministraSucursal(int idSucursal)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                SucursalesAgencia_VCLI sucursalAgencia = contexto.SucursalesAgencia_VCLI.FirstOrDefault(sucursal => sucursal.SUC_IdSucursal == idSucursal);
                if (sucursalAgencia != null)
                {
                    return new Servicios.ContratoDatos.CentroServicios.PUCentroServiciosDC()
                    {
                        IdCentroServicio = sucursalAgencia.CES_IdCentroServicios,
                        Nombre = sucursalAgencia.CES_Nombre
                    };
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Obtiene una lista con las sucursales para filtrar a partir de un nit de un cliente
        /// </summary>
        /// <returns>Colección con las sucursales configuradas en la base de datos</returns>
        public IEnumerable<CLSucursalDC> ObtenerSucursalesFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, int idCliente)
        {
            List<CLSucursalDC> sucursales = new List<CLSucursalDC>();

            indicePagina = indicePagina <= 0 ? 1 : indicePagina;

            string nombreSucursal = null, idSucursal = null;


            if (filtro.ContainsKey("SUC_Nombre"))
            {
                nombreSucursal = filtro["SUC_Nombre"];
            }

            if (filtro.ContainsKey("SUC_IdSucursal"))
            {
                idSucursal = filtro["SUC_IdSucursal"];
            }

            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerSucursalesPorNombreIDCliente_CLI", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PageIndex", indicePagina);
                cmd.Parameters.AddWithValue("@PageSize", registrosPorPagina);
                cmd.Parameters.AddWithValue("@OrdenamientoAscendente", ordenamientoAscendente);
                cmd.Parameters.AddWithValue("@CampoOrdenamiento", campoOrdenamiento);

                SqlParameter param = new SqlParameter("@totalRegistros", SqlDbType.Int);
                param.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(param);

                if (!string.IsNullOrEmpty(idSucursal))
                {
                    cmd.Parameters.AddWithValue("@Idsucursal", idSucursal);
                }
                if (!string.IsNullOrEmpty(nombreSucursal))
                {
                    cmd.Parameters.AddWithValue("@Nombre", nombreSucursal);
                }

                cmd.Parameters.AddWithValue("@idCliente", idCliente);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    CLSucursalDC sucursal = new CLSucursalDC
                    {
                        IdCliente = Convert.ToInt32(reader["SUC_ClienteCredito"]),
                        Direccion = Convert.ToString(reader["SUC_Direccion"]),
                        Fax = Convert.ToString(reader["SUC_Fax"]),
                        Ciudad = new PALocalidadDC
                        {
                            IdLocalidad = Convert.ToString(reader["SUC_Municipio"]),
                            Nombre = Convert.ToString(reader["LOC_Nombre"])
                        },
                        Pais = new PALocalidadDC { },
                        Telefono = Convert.ToString(reader["SUC_Telefono"]),
                        Agencia = Convert.ToInt64(reader["SUC_AgenciaEncargada"]),
                        Contacto = Convert.ToString(reader["SUC_NombreContacto"]),
                        Email = Convert.ToString(reader["SUC_Email"]),
                        Nombre = Convert.ToString(reader["SUC_Nombre"]),
                        Zona = Convert.ToString(reader["SUC_Zona"]),
                        IdSucursal = Convert.ToInt32(reader["SUC_IdSucursal"]),
                        NombreAgencia = reader["CES_Nombre"].ToString(),
                        Localidad = Convert.ToString(reader["SUC_Municipio"]),
                        NombreLocalidad = Convert.ToString(reader["LOC_Nombre"]),
                        ListaZonas = ConsultarZonasDeLocalidadXLocalidad(Convert.ToString(reader["SUC_Municipio"])),
                        IdBodega = Convert.ToString(reader["SUC_IdBodega"]),
                        LatitudSucursal = reader["SUC_Latitud"] != DBNull.Value ? Convert.ToDecimal(reader["SUC_Latitud"]) : 0,
                        LongitudSucursal = reader["SUC_Longitud"] != DBNull.Value ? Convert.ToDecimal(reader["SUC_Longitud"]) : 0,
                        CodigoPostal = Convert.ToString(reader["SUC_CodigoPostal"])
                    };

                    decimal lat = 0;

                    decimal.TryParse(reader["SUC_Latitud"].ToString(), out lat);

                    sucursal.LatitudSucursal = lat;

                    decimal longitud = 0;

                    decimal.TryParse(reader["SUC_Longitud"].ToString(), out longitud);

                    sucursal.LongitudSucursal = longitud;


                    switch (reader["LOC_IdTipo"].ToString())
                    {
                        case ConstantesClientes.TIPO_LOCALIDAD_1:
                            sucursal.Pais.IdLocalidad = Convert.ToString(reader["LOC_IdLocalidad"]);
                            sucursal.Pais.Nombre = Convert.ToString(reader["LOC_Nombre"]);
                            break;

                        case ConstantesClientes.TIPO_LOCALIDAD_2:
                            sucursal.Pais.IdLocalidad = Convert.ToString(reader["LOC_IdAncestroPrimerGrado"]);
                            sucursal.Pais.Nombre = Convert.ToString(reader["LOC_NombrePrimero"]);
                            break;

                        case ConstantesClientes.TIPO_LOCALIDAD_3:
                            sucursal.Pais.IdLocalidad = Convert.ToString(reader["LOC_IdAncestroSegundoGrado"]);
                            sucursal.Pais.Nombre = Convert.ToString(reader["LOC_NombreSegundo"]);
                            break;

                        default:
                            sucursal.Pais.IdLocalidad = Convert.ToString(reader["LOC_IdAncestroTercerGrado"]);
                            sucursal.Pais.Nombre = Convert.ToString(reader["LOC_NombreTercero"]);
                            break;
                    }
                    sucursales.Add(sucursal);
                }
                reader.Close();

                sqlConn.Close();

                totalRegistros = (int)cmd.Parameters["@totalRegistros"].Value;
            }

            return sucursales;

        }

        /// <summary>
        /// Consulta las zonas de una localidad incluye la zona general
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <returns>Lista de zonas</returns>
        public IList<PAZonaDC> ConsultarZonasDeLocalidadXLocalidad(string idLocalidad)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var zonas = contexto.ZonaDeLocalidad_PAR.Include("Zona_PAR").Where(obj => obj.ZLO_IdLocalidad == idLocalidad).Select(obj =>
                  new PAZonaDC()
                  {
                      IdZona = obj.ZLO_IdZona,
                      Descripcion = obj.Zona_PAR.ZON_Descripcion
                  }).ToList();

                if (zonas.Count <= 0)//si la localidad no tiene zonas asignadas se agrega la zona general
                {
                    var zonaGeneral = contexto.Zona_PAR.Where(obj => obj.ZON_IdZona == "-1").FirstOrDefault();
                    if (zonaGeneral == null)
                    {
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.PARAMETROS_GENERALES, ETipoErrorFramework.EX_FALTA_ZONA_GENERAL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_FALTA_ZONA_GENERAL)));
                    }
                    zonas.Add(new PAZonaDC() { Descripcion = zonaGeneral.ZON_Descripcion, IdZona = zonaGeneral.ZON_IdZona });
                }
                return zonas;
            }
        }

        /// <summary>
        /// Metodo para insertar sucursales en el repositorio
        /// </summary>
        /// <param name="sucursal">objeto de tipo sucursal</param>
        public int AdicionarSucursal(CLSucursalDC sucursal)
        {
            int idSucursal = 0;
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paInsertarLatitudLongitudSucursal_CLI", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Nombre", sucursal.Nombre);
                cmd.Parameters.AddWithValue("@ClienteCredito", sucursal.IdCliente);
                cmd.Parameters.AddWithValue("@Municipio", sucursal.Ciudad.IdLocalidad);
                cmd.Parameters.AddWithValue("@Zona", sucursal.Zona);
                cmd.Parameters.AddWithValue("@Direccion", sucursal.Direccion);
                cmd.Parameters.AddWithValue("@Telefono", sucursal.Telefono);
                cmd.Parameters.AddWithValue("@Fax", sucursal.Fax);
                cmd.Parameters.AddWithValue("@Email", sucursal.Email);
                cmd.Parameters.AddWithValue("@NombreContacto", sucursal.Contacto);
                cmd.Parameters.AddWithValue("@AgenciaEncargada", sucursal.Agencia);
                cmd.Parameters.AddWithValue("@FechaGrabacion", DateTime.Now);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);
                cmd.Parameters.AddWithValue("@IdBodega", sucursal.IdBodega);
                cmd.Parameters.AddWithValue("@latitud", sucursal.LatitudSucursal.ToString().Replace('.', ','));
                cmd.Parameters.AddWithValue("@longitud", sucursal.LongitudSucursal.ToString().Replace('.', ','));
                cmd.Parameters.AddWithValue("@codigoPostal", sucursal.CodigoPostal.ToString());
                idSucursal = Convert.ToInt32(cmd.ExecuteScalar());
                sqlConn.Close();
            }
            return idSucursal;

        }

        /// <summary>
        /// Metodo para modificar sucursales en el repositorio
        /// </summary>
        /// <param name="sucursal">objeto de tipo sucursal</param>
        public void ModificaSucursal(CLSucursalDC sucursal)
        {
            using (TransactionScope trans = new TransactionScope())
            {
                using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
                {
                    sqlConn.Open();
                    SqlCommand cmd = new SqlCommand("paModificarLatitudLongitudSucursal_CLI", sqlConn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdSucursal", sucursal.IdSucursal);
                    cmd.Parameters.AddWithValue("@Nombre", sucursal.Nombre);
                    cmd.Parameters.AddWithValue("@ClienteCredito", sucursal.IdCliente);
                    cmd.Parameters.AddWithValue("@Municipio", sucursal.Ciudad.IdLocalidad);
                    cmd.Parameters.AddWithValue("@Zona", sucursal.Zona);
                    cmd.Parameters.AddWithValue("@Direccion", sucursal.Direccion);
                    cmd.Parameters.AddWithValue("@Telefono", sucursal.Telefono);
                    cmd.Parameters.AddWithValue("@Fax", sucursal.Fax);
                    cmd.Parameters.AddWithValue("@Email", sucursal.Email);
                    cmd.Parameters.AddWithValue("@NombreContacto", sucursal.Contacto);
                    cmd.Parameters.AddWithValue("@AgenciaEncargada", sucursal.Agencia);
                    cmd.Parameters.AddWithValue("@FechaGrabacion", DateTime.Now);
                    cmd.Parameters.AddWithValue("@CreadoPor", "ASOLANO");
                    cmd.Parameters.AddWithValue("@IdBodega", sucursal.IdBodega);
                    cmd.Parameters.AddWithValue("@latitud", sucursal.LatitudSucursal.ToString().Replace('.', ','));
                    cmd.Parameters.AddWithValue("@longitud", sucursal.LongitudSucursal.ToString().Replace('.', ','));
                    cmd.Parameters.AddWithValue("@codigoPostal", sucursal.CodigoPostal.ToString());
                    cmd.ExecuteNonQuery();

                    cmd = new SqlCommand("paModificarReferenciaUsoGuia_CLI", sqlConn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdSucursal", sucursal.IdSucursal);
                    cmd.Parameters.AddWithValue("@Ciudad", sucursal.Ciudad.IdLocalidad);
                    cmd.Parameters.AddWithValue("@Direccion", sucursal.Direccion);
                    cmd.Parameters.AddWithValue("@Telefono", sucursal.Telefono);
                    cmd.Parameters.AddWithValue("@Nombre", sucursal.Nombre);
                    cmd.ExecuteNonQuery();
                    sqlConn.Close();
                }
                trans.Complete();
            }


        }

        /// <summary>
        /// Cambia la agencia encargada de la sucursal
        /// </summary>
        /// <param name="AnteriorAgencia"></param>
        /// <param name="NuevaAgencia"></param>
        public void ModificarAgenciaResponsableSucursal(long anteriorAgencia, long nuevaAgencia)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                IQueryable<Sucursal_CLI> sucursales = contexto.Sucursal_CLI.Where(s => s.SUC_AgenciaEncargada == anteriorAgencia);
                foreach (Sucursal_CLI sucursal in sucursales)
                {
                    sucursal.SUC_AgenciaEncargada = nuevaAgencia;
                }
                contexto.SaveChanges();
            }
        }

        #endregion Sucursales

        #region Tipo de guia de la sucursal

        /// <summary>
        /// Obtiene el tipo de guia por sucursal
        /// </summary>
        /// <param name="idSucursal"></param>
        /// <returns></returns>
        public CLSucursalGuiasDC ObtenerGuiaPorSucursal(int idSucursal)
        {
            return null;

            /*
            CLSucursalGuiasDC dato = new CLSucursalGuiasDC();
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
              ReferenciaUsoGuia_CLI guia = contexto.ReferenciaUsoGuia_CLI
                .Where(r => r.RUG_IdSucursal == idSucursal)
                .FirstOrDefault();

              if (guia != null)
              {
                Localidades_VPAR ciudad = contexto.Localidades_VPAR.Where(t => t.LOC_IdLocalidad == guia.RUG_CiudadDestino).Single();
                Localidades_VPAR pais = contexto.Localidades_VPAR.Where(t => t.LOC_IdLocalidad == guia.RUG_PaisDestino).Single();
                dato = new CLSucursalGuiasDC()
                  {
                    CiudadDestino = new PALocalidadDC { IdLocalidad = guia.RUG_CiudadDestino, Nombre = ciudad.LOC_Nombre },
                    PaisDestino = new PALocalidadDC { IdLocalidad = guia.RUG_PaisDestino, Nombre = ciudad.LOC_Nombre },
                    Direccion = guia.RUG_DireccionDestinatario,
                    Identificacion = guia.RUG_Identificacion,
                    IdSucursal = guia.SUC_IdSucursal,
                    IdTipoIdentificacion = guia.RUG_TipoIdentificacion,
                    Nombre = guia.RUG_NombreDestinatario,
                    EsOrigenAbierto = guia.RUG_EsOrigenAbierto,
                    Telefono = guia.RUG_TelefonoDestinatario,
                  };
                Sucursal_CLI sucursal = contexto.Sucursal_CLI.Include("ClienteCredito_CLI").FirstOrDefault(suc => suc.SUC_IdSucursal == idSucursal);
                if (sucursal != null)
                {
                  dato.CorreoRemitente = sucursal.SUC_Email;
                  dato.DireccionRemitente = sucursal.SUC_Direccion;
                  dato.NombreRemitente = sucursal.SUC_Nombre;
                  dato.NumeroDocumentoRemitente = sucursal.ClienteCredito_CLI.CLI_Nit;
                  dato.TelefonoRemitente = sucursal.SUC_Telefono;
                }
              }
              return dato;
            }*/
        }

        /// <summary>
        /// Metodo encargado de adicionar un tipo de guia por Sucursal
        /// </summary>
        /// <param name="guiaSucursal"></param>
        public void AdicionarGuiaPorSucursal(CLSucursalGuiasDC guiaSucursal)
        {
            throw new NotImplementedException("Cambiaron la tabla en el repositorio");
            /*
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
              ReferenciaUsoGuia_CLI sucursalGuia = new ReferenciaUsoGuia_CLI()
              {
                RUG_CiudadDestino = guiaSucursal.CiudadDestino.IdLocalidad,
                RUG_PaisDestino = guiaSucursal.PaisDestino.IdLocalidad,
                RUG_DireccionDestinatario = guiaSucursal.Direccion,
                RUG_Identificacion = guiaSucursal.Identificacion,
                RUG_NombreDestinatario = guiaSucursal.Nombre,
                SUC_IdSucursal = guiaSucursal.IdSucursal,
                RUG_TipoIdentificacion = guiaSucursal.IdTipoIdentificacion,
                RUG_EsOrigenAbierto = guiaSucursal.EsOrigenAbierto,
                RUG_TelefonoDestinatario = guiaSucursal.Telefono,
                RUG_CreadoPor = ControllerContext.Current.Usuario,
                RUG_FechaGrabacion = DateTime.Now
              };
              contexto.ReferenciaUsoGuia_CLI.Add(sucursalGuia);
              contexto.SaveChanges();
            }*/
        }

        /// <summary>
        /// Modifica un tipo de guia de una sucursal
        /// </summary>
        /// <param name="guiaSucursal"></param>
        public void ModificarGuiaPorSucursal(CLSucursalGuiasDC guiaSucursal)
        {
            throw new NotImplementedException("Cambiaron la tabla en el repositorio");
            /*
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
              ReferenciaUsoGuia_CLI guia = contexto.ReferenciaUsoGuia_CLI
              .Where(r => r.SUC_IdSucursal == guiaSucursal.IdSucursal)
              .FirstOrDefault();

              if (guia == null)
              {
                ControllerException excepcion = new ControllerException(COConstantesModulos.CLIENTES, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                throw new FaultException<ControllerException>(excepcion);
              }
              guia.RUG_CiudadDestino = guiaSucursal.CiudadDestino.IdLocalidad;
              guia.RUG_PaisDestino = guiaSucursal.PaisDestino.IdLocalidad;
              guia.RUG_DireccionDestinatario = guiaSucursal.Direccion;
              guia.RUG_Identificacion = guiaSucursal.Identificacion;
              guia.RUG_NombreDestinatario = guiaSucursal.Nombre;
              guia.SUC_IdSucursal = guiaSucursal.IdSucursal;
              guia.RUG_TipoIdentificacion = guiaSucursal.IdTipoIdentificacion;
              guia.RUG_EsOrigenAbierto = guiaSucursal.EsOrigenAbierto;
              guia.RUG_TelefonoDestinatario = guiaSucursal.Telefono;
              contexto.SaveChanges();
            }*/
        }

        /// <summary>
        /// Elimina un tipo de guia de una sucursal
        /// </summary>
        /// <param name="guiaSucursal"></param>
        public void EliminarGuiaPorSucursal(CLSucursalGuiasDC guiaSucursal)
        {
            throw new NotImplementedException("Cambiaron la tabla en el repositorio");
            /*
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
              ReferenciaUsoGuia_CLI guia = contexto.ReferenciaUsoGuia_CLI
              .Where(r => r.SUC_IdSucursal == guiaSucursal.IdSucursal)
              .FirstOrDefault();

              if (guia != null)
              {
                contexto.ReferenciaUsoGuia_CLI.Remove(guia);
                contexto.SaveChanges();
              }
            }*/
        }

        #endregion Tipo de guia de la sucursal

        #region Contratos

        /// <summary>
        /// Obtiene una lista con los contratos para filtrar a partir de una identificacion de un cliente
        /// </summary>
        /// <returns>Colección con las sucursales configuradas en la base de datos</returns>
        public List<CLContratosDC> ObtenerContratosFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, int idCliente)
        {
            List<CLContratosDC> lstContratos = new List<CLContratosDC>();
            using (SqlConnection conn = new SqlConnection(conexionStringTransaccional))
            {
                SqlCommand cmd = new SqlCommand("paObtenerDetalleContratos_CLI", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ClienteCredito", idCliente);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    CLContratosDC contrato = new CLContratosDC
                    {
                        IdCliente = Convert.ToInt32(reader["CON_ClienteCredito"]),
                        CiudadGestor = new PALocalidadDC { IdLocalidad = reader["CON_CiudadGestorPago"].ToString(), Nombre = reader["localidadgestor"].ToString() },
                        PaisGestor = new PALocalidadDC { IdLocalidad = reader["idPaisGestor"].ToString(), Nombre = reader["nombrePaisGestor"].ToString() },
                        CiudadInterventor = new PALocalidadDC { IdLocalidad = reader["CON_CiudadInterventor"].ToString(), Nombre = reader["localidadinterventor"].ToString() },
                        PaisInterventor = new PALocalidadDC { IdLocalidad = reader["idPaisInterventor"].ToString(), Nombre = reader["nombrePaisInterventor"].ToString() },
                        EjecutivoCuenta = Convert.ToInt64(reader["CON_EjecutivoCuenta"]),
                        FechaFinal = Convert.ToDateTime(reader["CON_FechaFin"]),
                        FechaInicial = Convert.ToDateTime(reader["CON_FechaInicio"]),
                        IdContrato = Convert.ToInt32(reader["CON_IdContrato"]),
                        ListaPrecios = Convert.ToInt32(reader["CON_ListaPrecios"]),
                        EsListaCliente = Convert.ToBoolean(reader["LIP_EsListaCliente"]),
                        NombreContrato = reader["CON_NombreContrato"].ToString(),
                        NombreGestor = reader["CON_NombreGestorPago"].ToString(),
                        NombreInterventor = reader["CON_NombreInterventor"].ToString(),
                        NumeroContrato = reader["CON_NumeroContrato"].ToString(),
                        ObjetoContrato = reader["CON_ObjetoContrato"].ToString(),
                        PorcentajeAviso = Convert.ToDecimal(reader["CON_PorcentajeAviso"]),
                        PresupuestoMensual = Convert.ToDecimal(reader["CON_PresupuestoMensual"]),
                        TelefonoGestor = reader["CON_TelefonoGestorPago"].ToString(),
                        TelefonoInterventor = reader["CON_TelefonoInterventor"].ToString(),
                        SupervisorCuenta = Convert.ToInt64(reader["CON_SupervisorCuenta"]),
                        FechaFinalExtension = Convert.ToDateTime(reader["CON_FechaFinConExtensiones"]),
                        Valor = Convert.ToDecimal(reader["CON_ValorContrato"]),
                        NombreEjecutivo = reader["personaejecutivo"].ToString(),
                        NombreSupervisor = reader["personasupervisor"].ToString(),
                        NumeroAsignacion = reader["CON_NumeroAsignacionPresupuest"].ToString(),
                        CertificadoDisponibilidad = reader["CON_CertificadoDisponibilidadPropuestal"].ToString(),
                        NumeroRegistroDisponibilidad = reader["CON_NumeroRegisroDisponibilidad"].ToString(),
                        ValorDisponibilidad = Convert.ToDecimal(reader["CON_ValorDisponibilidad"]),
                        ValidaPeso = Convert.ToBoolean(reader["CON_AplicaValidacionPesoAdmision"]),
                        AcumuladoVentas = Convert.ToDecimal(reader["CON_AcumuladoVentas"])
                    };
                    lstContratos.Add(contrato);
                }
                conn.Close();
            }
            return lstContratos;
        }

        /// <summary>
        /// Metodo para insertar contratos en el repositorio
        /// </summary>
        /// <param name="sucursal">objeto de tipo contrato</param>
        public int AdicionarContrato(CLContratosDC contrato)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    Contrato_CLI dato = new Contrato_CLI()
                    {
                        CON_CiudadGestorPago = contrato.CiudadGestor.IdLocalidad,
                        CON_CiudadInterventor = contrato.CiudadInterventor.IdLocalidad,
                        CON_ClienteCredito = contrato.IdCliente,
                        CON_EjecutivoCuenta = contrato.EjecutivoCuenta,
                        CON_FechaFin = contrato.FechaFinal,
                        CON_FechaInicio = contrato.FechaInicial,
                        CON_FechaFinConExtensiones = contrato.FechaFinal,
                        CON_ListaPrecios = contrato.ListaPrecios,
                        CON_NombreContrato = contrato.NombreContrato,
                        CON_NombreGestorPago = contrato.NombreGestor,
                        CON_NombreInterventor = contrato.NombreInterventor,
                        CON_NumeroContrato = contrato.NumeroContrato,
                        CON_ObjetoContrato = contrato.ObjetoContrato,
                        CON_PorcentajeAviso = contrato.PorcentajeAviso,
                        CON_PresupuestoMensual = contrato.PresupuestoMensual,
                        CON_TelefonoGestorPago = contrato.TelefonoGestor,
                        CON_TelefonoInterventor = contrato.TelefonoInterventor,
                        CON_ValorContrato = contrato.Valor,
                        CON_SupervisorCuenta = contrato.SupervisorCuenta,
                        CON_FechaGrabacion = DateTime.Now,
                        CON_CreadoPor = ControllerContext.Current.Usuario,
                        CON_NumeroAsignacionPresupuest = contrato.NumeroAsignacion,
                        CON_CertificadoDisponibilidadPropuestal = contrato.CertificadoDisponibilidad,
                        CON_NumeroRegisroDisponibilidad = contrato.NumeroRegistroDisponibilidad,
                        CON_ValorDisponibilidad = contrato.ValorDisponibilidad
                    };
                    contexto.Contrato_CLI.Add(dato);
                    contexto.SaveChanges();
                    scope.Complete();
                    return dato.CON_IdContrato;
                }
            }
        }

        /// <summary>
        /// Metodo para eliminar contratos en el repositorio
        /// </summary>
        /// <param name="sucursal">objeto de tipo contrato</param>
        public void EliminarContrato(CLContratosDC contrato)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Contrato_CLI dato = contexto.Contrato_CLI
               .Where(r => r.CON_IdContrato == contrato.IdContrato)
               .First();
                contexto.Contrato_CLI.Remove(dato);
                CLRepositorioAudit.MapearAuditModificarContrato(contexto);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Metodo para modificar contratos en el repositorio
        /// </summary>
        /// <param name="sucursal">objeto de tipo contrato</param>
        public void ModificarContrato(CLContratosDC contrato)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var dato = contexto.Contrato_CLI
                .Where(r => r.CON_IdContrato == contrato.IdContrato)
                .FirstOrDefault();
                dato.CON_CiudadGestorPago = contrato.CiudadGestor.IdLocalidad;
                dato.CON_CiudadInterventor = contrato.CiudadInterventor.IdLocalidad;
                dato.CON_ClienteCredito = contrato.IdCliente;
                dato.CON_EjecutivoCuenta = contrato.EjecutivoCuenta;
                dato.CON_FechaFin = contrato.FechaFinal;
                dato.CON_FechaFinConExtensiones = contrato.FechaFinal;
                dato.CON_FechaInicio = contrato.FechaInicial;
                dato.CON_ListaPrecios = contrato.ListaPrecios;
                dato.CON_NombreContrato = contrato.NombreContrato;
                dato.CON_NombreGestorPago = contrato.NombreGestor;
                dato.CON_NombreInterventor = contrato.NombreInterventor;
                dato.CON_NumeroContrato = contrato.NumeroContrato;
                dato.CON_ObjetoContrato = contrato.ObjetoContrato;
                dato.CON_PorcentajeAviso = contrato.PorcentajeAviso;
                dato.CON_PresupuestoMensual = contrato.PresupuestoMensual;
                dato.CON_TelefonoGestorPago = contrato.TelefonoGestor;
                dato.CON_TelefonoInterventor = contrato.TelefonoInterventor;
                dato.CON_ValorContrato = contrato.Valor;
                dato.CON_SupervisorCuenta = contrato.SupervisorCuenta;
                dato.CON_NumeroAsignacionPresupuest = contrato.NumeroAsignacion;
                dato.CON_CertificadoDisponibilidadPropuestal = contrato.CertificadoDisponibilidad;
                dato.CON_NumeroRegisroDisponibilidad = contrato.NumeroRegistroDisponibilidad;
                dato.CON_ValorDisponibilidad = contrato.ValorDisponibilidad;
                if (dato.CON_FechaFinConExtensiones < contrato.FechaFinal)
                    dato.CON_FechaFinConExtensiones = contrato.FechaFinal;
                dato.CON_AplicaValidacionPesoAdmision = contrato.ValidaPeso;
                CLRepositorioAudit.MapearAuditModificarContrato(contexto);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Consultar una lista de precios a partir del id del contrato
        /// </summary>
        /// <param name="idContrato">id contrato</param>
        /// <returns>Lista Orecios</returns>
        public int ObtenerListaPrecioContrato(int idContrato)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Contrato_CLI contratoCliente = contexto.Contrato_CLI.FirstOrDefault(contrato => contrato.CON_IdContrato == idContrato);
                if (contratoCliente == null)
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.CLIENTES, CLEnumTipoErrorCliente.EX_CONTRATO_NO_EXISTE.ToString(), CLMensajesClientes.CargarMensaje(CLEnumTipoErrorCliente.EX_CONTRATO_NO_EXISTE)));
                }

                return contratoCliente.CON_ListaPrecios;
            }
        }

        /// <summary>
        /// Metodo para actualizar la fecha de vigencia con extension de un contrato
        /// </summary>
        /// <param name="fechaVigencia"></param>
        /// <param name="idContrato"></param>
        private void ModificarVigenciaContrato(DateTime fechaVigencia, int idContrato)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var dato = contexto.Contrato_CLI
                .Where(r => r.CON_IdContrato == idContrato)
                .FirstOrDefault();
                dato.CON_FechaFinConExtensiones = fechaVigencia;
                CLRepositorioAudit.MapearAuditModificarContrato(contexto);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Retorna un contrato dado su ID
        /// </summary>
        /// <param name="idContrato"></param>
        /// <returns></returns>
        public CLContratosDC ObtenerContrato(int idContrato)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Contrato_CLI r = contexto.Contrato_CLI.FirstOrDefault(c => c.CON_IdContrato == idContrato);

                return new CLContratosDC
                {
                    FechaFinal = r.CON_FechaFin,
                    FechaInicial = r.CON_FechaInicio,
                    IdContrato = r.CON_IdContrato,
                    ListaPrecios = r.CON_ListaPrecios,
                    NombreContrato = r.CON_NombreContrato,
                    NombreGestor = r.CON_NombreGestorPago,
                    ValidaPeso = r.CON_AplicaValidacionPesoAdmision,
                };
            }
        }

        /// <summary>
        /// Obtiene los contratos activos por cliente
        /// </summary>
        /// <param name="idCliente">lista con los contratios activos por cliente</param>
        /// <returns></returns>
        public IEnumerable<CLContratosDC> ObtenerContratosActivos(int idCliente)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.Contrato_CLI
                .Where(r => DateTime.Now >= r.CON_FechaInicio && DateTime.Now <= r.CON_FechaFin && r.CON_ClienteCredito == idCliente)
                .ToList()
                .ConvertAll<CLContratosDC>(r =>
                {
                    CLContratosDC contrato = new CLContratosDC
                    {
                        NumeroContrato = r.CON_NumeroContrato,
                        FechaFinal = r.CON_FechaFin,
                        FechaInicial = r.CON_FechaInicio,
                        IdContrato = r.CON_IdContrato,
                        ListaPrecios = r.CON_ListaPrecios,
                        NombreContrato = r.CON_NombreContrato,
                        NombreGestor = r.CON_NombreGestorPago,
                        ValidaPeso = r.CON_AplicaValidacionPesoAdmision,
                        ServiciosHabilitados = ObtenerServiciosdeContrato(r.CON_IdContrato)
                    };
                    return contrato;
                }).ToList();
            }
        }



        /// <summary>
        /// obtine el contrato de un cliente en una
        /// ciudad
        /// </summary>
        /// <param name="idCliente">id del cliente</param>
        /// <param name="idCiudad">id de la ciudad</param>
        /// <returns>lista de contratos del cliente en esa ciudad</returns>
        public IEnumerable<CLContratosDC> ObtenerContratosClienteCiudad(int idCliente, string idCiudad)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                IEnumerable<CLContratosDC> contratosCliCiud = null;

                var contratos = (from Contrato in contexto.Contrato_CLI
                                 join Sucursal in contexto.Sucursal_CLI
                                 on Contrato.CON_ClienteCredito equals Sucursal.SUC_ClienteCredito
                                 join estadoContrato in contexto.SucursalesContrato_CLI
                                 on Contrato.CON_IdContrato equals estadoContrato.SUC_Contrato
                                 where Contrato.CON_ClienteCredito == idCliente &&
                                 Sucursal.SUC_Municipio == idCiudad &&
                                 estadoContrato.SUC_Estado == ConstantesFramework.ESTADO_ACTIVO
                                 select new
                                 {
                                     Contrato.CON_IdContrato,
                                     Contrato.CON_NombreContrato,
                                     Contrato.CON_NumeroContrato
                                 }).Distinct().ToList();

                if (contratos != null && contratos.Count > 0)
                {
                    contratosCliCiud = contratos.ConvertAll<CLContratosDC>(cont => new CLContratosDC()
                    {
                        IdContrato = cont.CON_IdContrato,
                        NombreContrato = cont.CON_NombreContrato,
                        NumeroContrato = cont.CON_NumeroContrato,
                    });
                }

                return contratosCliCiud;
            }
        }

        /// <summary>
        /// Obtiene el valor de los contratos vigentes de un cliente
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public decimal ObtenerValorContratos(int idCliente)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                decimal valor = contexto.Contrato_CLI
                  .Where(r => DateTime.Now >= r.CON_FechaInicio && DateTime.Now <= r.CON_FechaFin && r.CON_ClienteCredito == idCliente)
                  .Sum(s => (decimal?)s.CON_ValorContrato) ?? 0;
                return valor;
            }
        }

        /// <summary>
        /// Retorna el porcentaje de aviso para un contrato dado.
        /// </summary>
        /// <param name="idContrato"></param>
        /// <returns></returns>
        public decimal ObtenerPorcentajeAvisoContrato(int idContrato)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Contrato_CLI contrato = contexto.Contrato_CLI
                  .FirstOrDefault(r => r.CON_IdContrato == idContrato);
                if (contrato != null)
                {
                    return contrato.CON_PorcentajeAviso;
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Obtiene el valor de un contrato
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public decimal ObtenerValorPresupuestoContrato(int idContrato)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                decimal valor = contexto.Contrato_CLI
                  .Where(r => r.CON_IdContrato == idContrato)
                  .Single().CON_ValorContrato;

                return valor;
            }
        }

        /// <summary>
        /// Obtiene el valor de las adiciones en dinero de un otro si
        /// </summary>
        /// <param name="idContrato"></param>
        /// <returns></returns>
        public decimal ObtenerValorPresupuestoOtrosi(int idContrato)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                decimal valor = contexto.OtroSiContrato_CLI
                  .Where(r => r.OSC_Contrato == idContrato)
                  .Count() > 0 ?
                  contexto.OtroSiContrato_CLI
                    .Where(r => r.OSC_Contrato == idContrato)
                    .Sum(s => s.OSC_ValorOtroSi)
                    : 0;
                return valor;
            }
        }

        /// <summary>
        /// Obtiene el valor del presupuesto mensual de un contrato
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public decimal ObtenerValorPresupuestoPeriodo(int idContrato)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                decimal valor = contexto.Contrato_CLI
                  .Where(r => r.CON_IdContrato == idContrato)
                  .Single().CON_PresupuestoMensual;

                return valor;
            }
        }

        /// <summary>
        /// Retorna el valor total del contrato
        /// </summary>
        /// <param name="idContrato"></param>
        /// <returns></returns>
        public decimal ObtenerValorPresupuestoTotal(int idContrato)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                decimal valor = contexto.Contrato_CLI
                     .Where(r => r.CON_IdContrato == idContrato)
                     .Single().CON_ValorContrato;

                return valor;
            }
        }

        /// <summary>
        /// Obtiene el valor del consumo de un contrato
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public decimal ObtenerConsumoContrato(int idContrato)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                decimal valor = contexto.Contrato_CLI
                  .Where(r => r.CON_IdContrato == idContrato)
                  .Single().CON_AcumuladoVentas;

                return valor;
            }
        }

        /// <summary>
        /// Metodo para obtener el acumulado de facturación a partir de una fecha de corte y un contrato
        /// </summary>
        /// <param name="fechaInicial"></param>
        /// <returns></returns>
        public decimal ObtenerConsumoPeriodo(int idContrato)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var a = contexto.paObtenerConsumoPeriodo_CLI(idContrato);
                var consumo = a.FirstOrDefault();
                return consumo.HasValue ? consumo.Value : 0M;
            }
        }

        /// <summary>
        /// Metodo para actualizar el acumulado de un contrato
        /// </summary>
        /// <param name="idContrato"></param>
        ///// <param name="valorTransaccion"></param>
        //public void ModificarAcumuladoContrato(int idContrato, decimal valorTransaccion)
        //{
        //  using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
        //    {
        //        SqlCommand cmd = new SqlCommand("paGestionarAcumuladoContrato_CLI", sqlConn);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@CON_IdContrato", idContrato);
        //        cmd.Parameters.AddWithValue("@ACC_ValorTransaccion", valorTransaccion);
        //        cmd.Parameters.AddWithValue("@ACC_CreadoPor", ControllerContext.Current.Usuario);
        //        sqlConn.Open();
        //        cmd.ExecuteNonQuery();
        //        sqlConn.Close();
        //    }
        //}


        /// <summary>
        /// Metodo para actualizar el acumulado de un contrato
        /// </summary>
        /// <param name="idContrato"></param>
        /// <param name="valorTransaccion"></param>
        public void ModificarAcumuladoContrato(int idContrato, decimal valorTransaccion)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paGestionarAcumuladoContrato_CLI(idContrato, valorTransaccion, ControllerContext.Current.Usuario);
            }
        }

        /// <summary>
        /// Valida vigencia de un contrato
        /// </summary>
        /// <param name="idContrato"></param>
        /// <returns></returns>
        public bool ValidarVigenciaContrato(int idContrato)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var contrato = contexto.Contrato_CLI
                  .Where(r => DateTime.Now >= r.CON_FechaInicio && DateTime.Now <= r.CON_FechaFin && r.CON_IdContrato == idContrato)
                  .FirstOrDefault().CON_IdContrato;
                if (contrato == 0)
                    return false;
                else
                    return true;
            }
        }

        /// <summary>
        /// Obtiene la lista de los contratos de las
        /// sucursales Activas
        /// </summary>
        /// <returns>lista de Contratos</returns>
        public List<CLContratosDC> ObtenerContratosActivosDeSucursales()
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                List<CLContratosDC> contratosActivos = new List<CLContratosDC>();

                var contratos = (from Contrato in contexto.Contrato_CLI
                                 join SucursalContrato in contexto.SucursalesContrato_CLI
                                 on Contrato.CON_IdContrato equals SucursalContrato.SUC_Contrato
                                 where SucursalContrato.SUC_Estado == ConstantesFramework.ESTADO_ACTIVO
                                 select new
                                 {
                                     Contrato.CON_IdContrato,
                                     Contrato.CON_NombreContrato,
                                     Contrato.CON_NumeroContrato
                                 }).Distinct().ToList();

                if (contratos != null && contratos.Count > 0)
                {
                    contratosActivos = contratos.ConvertAll<CLContratosDC>(cont => new CLContratosDC()
                    {
                        IdContrato = cont.CON_IdContrato,
                        NombreContrato = cont.CON_NombreContrato,
                        NumeroContrato = cont.CON_NumeroContrato
                    });
                }

                return contratosActivos;
            }
        }

        #endregion Contratos

        #region Personal del contrato

        /// <summary>
        /// Obtiene las personas asociadas a un contrato
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <param name="idContrato"></param>
        /// <returns></returns>
        public IEnumerable<PAPersonaInternaDC> ObtenerPersonalContratoFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, int idContrato)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Dictionary<LambdaExpression, OperadorLogico> where = new Dictionary<LambdaExpression, OperadorLogico>();
                LambdaExpression lamda = contexto.CrearExpresionLambda<PersonaInternaCargoCenSer_VPAR>("PEC_Contrato", idContrato.ToString(), OperadorComparacion.Equal);
                where.Add(lamda, OperadorLogico.And);

                return contexto.ConsultarPersonaInternaCargoCenSer_VPAR(filtro, where, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                .ToList().ConvertAll<PAPersonaInternaDC>(r =>
                {
                    PAPersonaInternaDC persona = new PAPersonaInternaDC
                    {
                        IdPersonaInterna = r.PEI_IdPersonaInterna,
                        NombreCompleto = r.PEI_Nombre + " " + r.PEI_PrimerApellido,
                        Identificacion = r.PEI_Identificacion,
                        IdCargo = r.PEI_IdCargo,
                        IdRegionalAdministrativa = r.PEI_IdRegionalAdm,
                        NombreCargo = r.CAR_Descripcion,
                        NombreRegional = r.REA_Descripcion,
                        idContrato = r.PEC_Contrato,
                    };

                    return persona;
                }).ToList();
            }
        }

        /// <summary>
        /// Metodo para validar si una persona ya se encuentra asignada a un contrato
        /// </summary>
        /// <param name="persona"></param>
        /// <returns></returns>
        public bool ValidarPersonal(PAPersonaInternaDC persona)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.PersonalContrato_CLI
                  .Where(p => p.PEC_PersonaInterna == persona.IdPersonaInterna)
                  .FirstOrDefault() != null;
            }
        }

        /// <summary>
        /// Metodo para agregar personal a un contrato
        /// </summary>
        /// <param name="persona"></param>
        public void AdicionarPersonal(PAPersonaInternaDC persona)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PersonalContrato_CLI dato = new PersonalContrato_CLI()
                {
                    PEC_PersonaInterna = persona.IdPersonaInterna,
                    PEC_Contrato = persona.idContrato,
                    PEC_FechaGrabacion = DateTime.Now,
                    PEC_CreadoPor = ControllerContext.Current.Usuario
                };
                contexto.PersonalContrato_CLI.Add(dato);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Metodo para eliminar personal de un contrato
        /// </summary>
        /// <param name="persona"></param>
        public void EliminarPersonal(PAPersonaInternaDC persona)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PersonalContrato_CLI dato = contexto.PersonalContrato_CLI
              .Where(r => r.PEC_PersonaInterna == persona.IdPersonaInterna)
              .First();
                contexto.PersonalContrato_CLI.Remove(dato);

                contexto.SaveChanges();
            }
        }

        #endregion Personal del contrato

        #region Contactos del contrato

        /// <summary>
        /// Obtiene loc contactos asociadas a un contrato
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <param name="idContrato"></param>
        /// <returns></returns>
        public IEnumerable<CLContactosDC> ObtenerContactosContratoFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, int idContrato)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Dictionary<LambdaExpression, OperadorLogico> where = new Dictionary<LambdaExpression, OperadorLogico>();
                LambdaExpression lamda = contexto.CrearExpresionLambda<ContactosContrato_CLI>("COC_Contrato", idContrato.ToString(), OperadorComparacion.Equal);
                where.Add(lamda, OperadorLogico.And);

                return contexto.ConsultarContactosContrato_CLI(filtro, where, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                .ToList().ConvertAll<CLContactosDC>(r =>
                {
                    CLContactosDC contacto = new CLContactosDC
                    {
                        Apellidos = r.COC_Apellidos,
                        Direccion = r.COC_Direccion,
                        IdContacto = r.COC_IdContacto,
                        IdContrato = r.COC_Contrato,
                        Nombre = r.COC_Nombres,
                        Telefono = r.COC_Telefono,
                        Cargo = r.COC_Cargo,
                    };
                    return contacto;
                }).ToList();
            }
        }

        /// <summary>
        /// Metodo para adicionar contactos a un contrato
        /// </summary>
        /// <param name="contacto"></param>
        public void AdicionarContacto(CLContactosDC contacto)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ContactosContrato_CLI dato = new ContactosContrato_CLI()
                {
                    COC_Apellidos = contacto.Apellidos,
                    COC_Direccion = contacto.Direccion,
                    COC_Contrato = contacto.IdContrato,
                    COC_Nombres = contacto.Nombre,
                    COC_Telefono = contacto.Telefono,
                    COC_Cargo = contacto.Cargo,
                    COC_FechaGrabacion = DateTime.Now,
                    COC_CreadoPor = ControllerContext.Current.Usuario
                };
                contexto.ContactosContrato_CLI.Add(dato);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Metodo para modificar contactos a un contrato
        /// </summary>
        /// <param name="contacto"></param>
        public void ModificarContacto(CLContactosDC contacto)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var dato = contexto.ContactosContrato_CLI
                .Where(r => r.COC_IdContacto == contacto.IdContacto)
               .FirstOrDefault();
                dato.COC_Apellidos = contacto.Apellidos;
                dato.COC_Direccion = contacto.Direccion;
                dato.COC_Nombres = contacto.Nombre;
                dato.COC_Telefono = contacto.Telefono;
                dato.COC_Cargo = contacto.Cargo;
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Metodo para adicionar contactos a un contrato
        /// </summary>
        /// <param name="contacto"></param>
        public void EliminarContacto(CLContactosDC contacto)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                {
                    ContactosContrato_CLI dato = contexto.ContactosContrato_CLI
                   .Where(r => r.COC_IdContacto == contacto.IdContacto)
                   .First();
                    contexto.ContactosContrato_CLI.Remove(dato);
                    contexto.SaveChanges();
                }
            }
        }

        #endregion Contactos del contrato

        #region Deducciones del contrato

        /// <summary>
        /// Obtiene las deducciones asociadas a un contrato
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <param name="idContrato"></param>
        /// <returns></returns>
        public IEnumerable<CLDeduccionesContratoDC> ObtenerDeduccionesContratoFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, int idContrato)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Dictionary<LambdaExpression, OperadorLogico> where = new Dictionary<LambdaExpression, OperadorLogico>();
                LambdaExpression lamda = contexto.CrearExpresionLambda<DeduccionesFacturacion_CLI>("DEF_Contrato", idContrato.ToString(), OperadorComparacion.Equal);
                where.Add(lamda, OperadorLogico.And);

                return contexto.ConsultarDeduccionesFacturacion_CLI(filtro, where, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                .ToList().ConvertAll<CLDeduccionesContratoDC>(r =>
                {
                    CLDeduccionesContratoDC deduccion = new CLDeduccionesContratoDC
                    {
                        IdContrato = r.DEF_Contrato,
                        DescripcionDeduccion = r.DEF_Descripcion,
                        IdDeduccion = r.DEF_IdDeduccion,
                        TarifaPorcentual = r.DEF_TarifaPorcentual,
                        ValorFijo = r.DEF_ValorFijo,
                        ValorPorMonto = r.DEF_ValorPorMonto,
                    };
                    return deduccion;
                }).ToList();
            }
        }

        /// <summary>
        /// Metodo para adicionar deducciones a un contrato
        /// </summary>
        /// <param name="deduccion"></param>
        public void AdicionarDeduccion(CLDeduccionesContratoDC deduccion)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                DeduccionesFacturacion_CLI dato = new DeduccionesFacturacion_CLI()
                {
                    DEF_Contrato = deduccion.IdContrato,
                    DEF_Descripcion = deduccion.DescripcionDeduccion,
                    DEF_TarifaPorcentual = deduccion.TarifaPorcentual,
                    DEF_ValorFijo = deduccion.ValorFijo,
                    DEF_ValorPorMonto = deduccion.ValorPorMonto,
                    DEF_FechaGrabacion = DateTime.Now,
                    DEF_CreadoPor = ControllerContext.Current.Usuario
                };
                contexto.DeduccionesFacturacion_CLI.Add(dato);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Metodo para modificar deducciones de un contrato
        /// </summary>
        /// <param name="deduccion"></param>
        public void ModificarDeduccion(CLDeduccionesContratoDC deduccion)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var dato = contexto.DeduccionesFacturacion_CLI
                .Where(r => r.DEF_IdDeduccion == deduccion.IdDeduccion)
               .FirstOrDefault();
                dato.DEF_Contrato = deduccion.IdContrato;
                dato.DEF_Descripcion = deduccion.DescripcionDeduccion;
                dato.DEF_TarifaPorcentual = deduccion.TarifaPorcentual;
                dato.DEF_ValorFijo = deduccion.ValorFijo;
                dato.DEF_ValorPorMonto = deduccion.ValorPorMonto;
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Metodo para eliminar una deduccion de un contrato
        /// </summary>
        /// <param name="deduccion"></param>
        public void EliminarDeduccion(CLDeduccionesContratoDC deduccion)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                {
                    DeduccionesFacturacion_CLI dato = contexto.DeduccionesFacturacion_CLI
                   .Where(r => r.DEF_IdDeduccion == deduccion.IdDeduccion)
                   .First();
                    contexto.DeduccionesFacturacion_CLI.Remove(dato);
                    contexto.SaveChanges();
                }
            }
        }

        #endregion Deducciones del contrato

        #region Otrosi del contrato

        /// <summary>
        /// obtiene los otro si de un contrato
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <param name="idContrato"></param>
        /// <returns></returns>
        public IEnumerable<CLOtroSiDC> ObtenerOtroSiContratoFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, int idContrato)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Dictionary<LambdaExpression, OperadorLogico> where = new Dictionary<LambdaExpression, OperadorLogico>();
                LambdaExpression lamda = contexto.CrearExpresionLambda<OtroSiContrato_CLI>("OSC_Contrato", idContrato.ToString(), OperadorComparacion.Equal);
                where.Add(lamda, OperadorLogico.And);

                IEnumerable<CLOtroSiDC> listaotrosi = contexto.ConsultarOtroSiContrato_CLI(filtro, where, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                .ToList().ConvertAll<CLOtroSiDC>(r =>
                {
                    CLOtroSiDC otrosi = new CLOtroSiDC
                    {
                        Valor = r.OSC_ValorOtroSi,
                        Descripcion = r.OSC_Descripcion,
                        IdContrato = r.OSC_Contrato,
                        FechaFinal = r.OSC_FechaFin,
                        IdModalidadOtroSi = r.OSC_IdModalidadOtroSi,
                        IdOtroSi = r.OSC_IdOtroSi,
                        NumeroOtroSi = r.OSC_NumeroOtroSi,
                        DescripcionModalidad = contexto.ModalidadOtroSi_CLI.Where(s => s.MOS_IdModalidadOtroSi == r.OSC_IdModalidadOtroSi).FirstOrDefault().MOS_Descripcion,
                        NumeroRegistroDisponibilidad = r.OSC_NumeroRegistroDisponibilidad,
                        ValorDisponibilidad = r.OSC_ValorDisponibilidad
                    };
                    return otrosi;
                }).ToList();
                return listaotrosi;
            }
        }

        /// <summary>
        /// Lista con los tipos de otrosi de un contrato
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CLTipoOtroSiDC> ObtenerListaTiposOtrosi()
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ModalidadOtroSi_CLI.ToList().ConvertAll<CLTipoOtroSiDC>(r =>
                {
                    CLTipoOtroSiDC tipootrosi = new CLTipoOtroSiDC
                    {
                        Id = r.MOS_IdModalidadOtroSi,
                        Descripcion = r.MOS_Descripcion,
                    };
                    return tipootrosi;
                }).ToList();
            }
        }

        /// <summary>
        /// adiciona un otro si a un contrato
        /// </summary>
        /// <param name="otrosi"></param>
        public int AdicionarOtroSi(CLOtroSiDC otrosi)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                OtroSiContrato_CLI dato = new OtroSiContrato_CLI()
                {
                    OSC_ValorOtroSi = otrosi.Valor,
                    OSC_Descripcion = otrosi.Descripcion,
                    OSC_Contrato = otrosi.IdContrato,
                    OSC_FechaFin = otrosi.FechaFinal,
                    OSC_IdModalidadOtroSi = otrosi.IdModalidadOtroSi,
                    OSC_NumeroOtroSi = otrosi.NumeroOtroSi,
                    OSC_FechaGrabacion = DateTime.Now,
                    OSC_CreadoPor = ControllerContext.Current.Usuario,
                    OSC_NumeroRegistroDisponibilidad = otrosi.NumeroRegistroDisponibilidad,
                    OSC_ValorDisponibilidad = otrosi.ValorDisponibilidad
                };
                contexto.OtroSiContrato_CLI.Add(dato);
                contexto.SaveChanges();
                ModificarVigenciaContrato(otrosi.FechaFinal, otrosi.IdContrato);

                return dato.OSC_IdOtroSi;
            }
        }

        /// <summary>
        /// modifica un otrosi de un contrato
        /// </summary>
        /// <param name="otrosi"></param>
        public void ModificarOtroSi(CLOtroSiDC otrosi)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var dato = contexto.OtroSiContrato_CLI
                .Where(r => r.OSC_IdOtroSi == otrosi.IdOtroSi)
               .FirstOrDefault();
                dato.OSC_ValorOtroSi = otrosi.Valor;
                dato.OSC_Contrato = otrosi.IdContrato;
                dato.OSC_Descripcion = otrosi.Descripcion;
                dato.OSC_FechaFin = otrosi.FechaFinal;
                dato.OSC_IdModalidadOtroSi = otrosi.IdModalidadOtroSi;
                dato.OSC_NumeroOtroSi = otrosi.NumeroOtroSi;
                CLRepositorioAudit.MapearAuditModificarOtrosi(contexto);
                contexto.SaveChanges();
                ModificarVigenciaContrato(otrosi.FechaFinal, otrosi.IdContrato);
                dato.OSC_NumeroRegistroDisponibilidad = otrosi.NumeroRegistroDisponibilidad;
                dato.OSC_ValorDisponibilidad = otrosi.ValorDisponibilidad;

                contexto.SaveChanges();
            }
        }

        #endregion Otrosi del contrato

        #region Archivos del contrato

        /// <summary>
        /// Obtiene lista con los archivos de un contrato
        /// </summary>
        /// <returns>objeto de tipo contrato</returns>
        public IEnumerable<CLContratosArchivosDC> ObtenerArchivosContrato(CLContratosDC contrato)
        {
            List<CLContratosArchivosDC> listacliente = new List<CLContratosArchivosDC>();
            List<CLContratosArchivosDC> listaclienteadicionales = new List<CLContratosArchivosDC>();
            //TODO PARRA DIGITALIZACION: comentareo dejando como estaba este metodo
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerRutasImagenArchivosContrato_CLI", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumIdContrato", contrato.IdContrato);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    CLContratosArchivosDC contratoCliente = new CLContratosArchivosDC()
                    {
                        IdContrato = Convert.ToInt32(reader["ARC_IdContrato"]),
                        IdArchivo = Convert.ToInt64(reader["ARC_IdArchivo"]),
                        IdDocumento = Convert.ToInt16(reader["ARC_TipoDocumento"]),
                        Fecha = Convert.ToDateTime(reader["ARC_FechaGrabacion"]),
                        IdAdjunto = new Guid(reader["ARC_IdAdjunto"].ToString()),
                        NombreAdjunto = Convert.ToString(reader["ARC_NombreAdjunto"]),
                        EstadoRegistro = Framework.Servidor.Comun.EnumEstadoRegistro.SIN_CAMBIOS
                    };
                    listacliente.Add(contratoCliente);
                }
                sqlConn.Close();
            }


            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerRutasImagenArchivosContrato_CLI", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumIdContrato", contrato.IdContrato);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    CLContratosArchivosDC contratoCliente = new CLContratosArchivosDC()
                    {
                        IdContrato = Convert.ToInt32(reader["ARC_IdContrato"]),
                        IdArchivo = Convert.ToInt64(reader["ARC_IdArchivo"]),
                        IdDocumento = Convert.ToInt16(reader["ARC_TipoDocumento"]),
                        Fecha = Convert.ToDateTime(reader["ARC_FechaGrabacion"]),
                        IdAdjunto = new Guid(reader["ARC_IdAdjunto"].ToString()),
                        NombreAdjunto = Convert.ToString(reader["ARC_NombreAdjunto"]),
                        EstadoRegistro = Framework.Servidor.Comun.EnumEstadoRegistro.SIN_CAMBIOS
                    };
                    listaclienteadicionales.Add(contratoCliente);
                }
                sqlConn.Close();
            }

            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var listadocumentos = contexto.DocumentosContrato_CLI
                .Where(t => t.DOC_Estado != ConstantesFramework.ESTADO_INACTIVO && t.DOC_IdDocumento != ConstantesClientes.TIPO_ARCHIVO_ADICIONAL)
                .OrderByDescending(o => o.DOC_IdDocumento)
                .ToList()
                .ConvertAll<CLContratosArchivosDC>(r => new CLContratosArchivosDC()
                {
                    IdDocumento = r.DOC_IdDocumento,
                    NombreDocumento = r.DOC_Nombre,
                    EstadoDocumento = r.DOC_Estado,
                });

                foreach (CLContratosArchivosDC documento in listadocumentos)
                {
                    foreach (CLContratosArchivosDC archivo in listacliente)
                    {
                        if (documento.IdDocumento == archivo.IdDocumento)
                        {
                            documento.IdAdjunto = archivo.IdAdjunto;
                            documento.IdArchivo = archivo.IdArchivo;
                            documento.IdContrato = archivo.IdContrato;
                            documento.NombreAdjunto = archivo.NombreAdjunto;
                            documento.Fecha = archivo.Fecha;
                            documento.EstadoRegistro = archivo.EstadoRegistro;
                        }
                    }
                }

                listadocumentos.AddRange(listaclienteadicionales);

                return listadocumentos;
            }


            /*string query;

            query = "SELECT * FROM dbo.ArchivosContrato_CLI WHERE ARC_IdContrato = " + contrato.IdContrato + " AND ARC_TipoDocumento != '" + ConstantesClientes.TIPO_ARCHIVO_ADICIONAL + "'";
            using (SqlConnection sqlConn = new SqlConnection(conexionStringArchivo))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand(query, sqlConn);
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow r in dt.Rows)
                    {
                        listacliente.Add
                            (new CLContratosArchivosDC
                            {
                                IdContrato = Convert.ToInt32(r["ARC_IdContrato"]),
                                IdArchivo = Convert.ToInt64(r["ARC_IdArchivo"]),
                                IdDocumento = Convert.ToInt16(r["ARC_TipoDocumento"]),
                                Fecha = Convert.ToDateTime(r["ARC_FechaGrabacion"]),
                                IdAdjunto = new Guid(r["ARC_IdAdjunto"].ToString()),
                                NombreAdjunto = r["ARC_NombreAdjunto"].ToString(),
                                EstadoRegistro = Framework.Servidor.Comun.EnumEstadoRegistro.SIN_CAMBIOS
                            }
                            );
                    };
                }
            }

            query = "SELECT * FROM dbo.ArchivosContrato_CLI WHERE ARC_IdContrato = " + contrato.IdContrato + " AND ARC_TipoDocumento = '" + ConstantesClientes.TIPO_ARCHIVO_ADICIONAL + "'";
            using (SqlConnection sqlConn2 = new SqlConnection(conexionStringArchivo))
            {
                sqlConn2.Open();
                SqlCommand cmd2 = new SqlCommand(query, sqlConn2);
                DataTable dt2 = new DataTable();
                dt2.Load(cmd2.ExecuteReader());
                if (dt2.Rows.Count > 0)
                {
                    foreach (DataRow r in dt2.Rows)
                    {
                        listaclienteadicionales.Add
                            (new CLContratosArchivosDC
                            {
                                IdContrato = Convert.ToInt32(r["ARC_IdContrato"]),
                                IdArchivo = Convert.ToInt64(r["ARC_IdArchivo"]),
                                IdDocumento = Convert.ToInt16(r["ARC_TipoDocumento"]),
                                Fecha = Convert.ToDateTime(r["ARC_FechaGrabacion"]),
                                IdAdjunto = new Guid(r["ARC_IdAdjunto"].ToString()),
                                NombreAdjunto = r["ARC_NombreAdjunto"].ToString(),
                                NombreDocumento = ConstantesClientes.ARCHIVO_ADICIONAL,
                                EstadoRegistro = Framework.Servidor.Comun.EnumEstadoRegistro.SIN_CAMBIOS
                            }
                            );
                    };
                }
            }

            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var listadocumentos = contexto.DocumentosContrato_CLI
                .Where(t => t.DOC_Estado != ConstantesFramework.ESTADO_INACTIVO && t.DOC_IdDocumento != ConstantesClientes.TIPO_ARCHIVO_ADICIONAL)
                .OrderByDescending(o => o.DOC_IdDocumento)
                .ToList()
                .ConvertAll<CLContratosArchivosDC>(r => new CLContratosArchivosDC()
                {
                    IdDocumento = r.DOC_IdDocumento,
                    NombreDocumento = r.DOC_Nombre,
                    EstadoDocumento = r.DOC_Estado,
                });

                foreach (CLContratosArchivosDC documento in listadocumentos)
                {
                    foreach (CLContratosArchivosDC archivo in listacliente)
                    {
                        if (documento.IdDocumento == archivo.IdDocumento)
                        {
                            documento.IdAdjunto = archivo.IdAdjunto;
                            documento.IdArchivo = archivo.IdArchivo;
                            documento.IdContrato = archivo.IdContrato;
                            documento.NombreAdjunto = archivo.NombreAdjunto;
                            documento.Fecha = archivo.Fecha;
                            documento.EstadoRegistro = archivo.EstadoRegistro;
                        }
                    }
                }

                listadocumentos.AddRange(listaclienteadicionales);

                return listadocumentos;
            }*/
        }

        /// <summary>
        /// Adiciona archivo de un contrato
        /// </summary>
        /// <param name="archivo">objeto de tipo contrato</param>
        public void AdicionarArchivoContrato(CLContratosArchivosDC archivo)
        {
            //TODO PARRA DIGITALIZACION: comentareo dejando como estaba este metodo
            filePath = PAAdministrador.Instancia.ObtenerParametrosAplicacion(PAAdministrador.RutaArchivos);
            string rutaArchivo = Path.Combine(this.filePath, COConstantesModulos.CLIENTES, archivo.NombreServidor); 
            byte[] archivoImagen;
            string extension;
            using (FileStream fs = File.OpenRead(rutaArchivo))
            {
                extension = Path.GetExtension(fs.Name);
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
                fs.Close();
                archivoImagen = bytes;
            }


            string rutaImagenes = Framework.Servidor.ParametrosFW.PAParametros.Instancia.ConsultarParametrosFramework("FolderArchivoContra");
            string carpetaDestino = Path.Combine(rutaImagenes + "\\" + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year);
            if (!Directory.Exists(carpetaDestino))
            {
                Directory.CreateDirectory(carpetaDestino);
            }

            string ruta = carpetaDestino + "\\" + Guid.NewGuid() + extension;

            using (FileStream fs = new FileStream(ruta, FileMode.Create))
            {
                fs.Write(archivoImagen, 0, archivoImagen.Length);
                fs.Close();
            }

            //Image imagenArchivo;
            //using (FileStream fs = File.OpenRead(rutaArchivo))
            //{
            //    imagenArchivo = Image.FromStream(fs);
            //    fs.Close();
            //}
            //ImageCodecInfo jpgEncoder = UtilidadesFW.GetEncoder(ImageFormat.Jpeg);
            //string ruta = carpetaDestino + "\\" + Guid.NewGuid() + ".jpg";
            //System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
            //EncoderParameters myEncoderParameters = new EncoderParameters(1);
            //EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 180L);
            //myEncoderParameters.Param[0] = myEncoderParameter;
            //var im = new Bitmap(imagenArchivo);
            //im.Save(ruta, jpgEncoder, myEncoderParameters);

            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paInsertarArchivosContrato", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idContrato", archivo.IdContrato);
                cmd.Parameters.AddWithValue("@tipoDocumento", archivo.IdDocumento);
                if (archivo.NombreServidor.Length > 35)
                {
                    archivo.NombreServidor = archivo.NombreServidor.Substring(0, 30) + extension;
                }
                cmd.Parameters.AddWithValue("@nombreAdjunto", archivo.NombreServidor);
                cmd.Parameters.AddWithValue("@rutaAdjunto", ruta);
                cmd.Parameters.AddWithValue("@idAdjunto", Guid.NewGuid());
                cmd.Parameters.AddWithValue("@creadoPor", ControllerContext.Current.Usuario);
                cmd.ExecuteNonQuery();
                sqlConn.Close();
            }
        }

        /// <summary>
        /// Elimina archivo de un contrato
        /// </summary>
        /// <param name="archivo">objeto de tipo contrato</param>
        public void EliminarArchivoContrato(CLContratosArchivosDC archivo)
        {
            // string query = "DELETE FROM [ArchivosContrato_CLI] WITH (ROWLOCK)" +
            //"WHERE  ARC_IdArchivo = " + archivo.IdArchivo;
            // Se determina la ruta temporal donde se va a almacenar
            string rutaArhivo = string.Empty;

            int resultado = 0;

            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerRutasImagenArchivosContrato_CLI", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumIdContrato", archivo.IdContrato);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    rutaArhivo = Convert.ToString(reader["ARC_RutaAdjunto"]);
                }
                sqlConn.Close();
            }
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paBorrarRutasImagenArchivosContrato_CLI", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdArchivo", archivo.IdArchivo);
                resultado = cmd.ExecuteNonQuery();
                sqlConn.Close();
            }
            if (resultado > 0)
            {
                File.Delete(rutaArhivo);
            }
        }

        /// <summary>
        ///Metodo para obtener un archivo asociado a un contrato
        /// </summary>
        /// <param name="archivo"></param>
        /// <returns></returns>
        public string ObtenerArchivoContrato(CLContratosArchivosDC archivo)
        {
            string respuesta = "";
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerRutasImagenArchivosContrato_CLI", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumIdContrato", archivo.IdContrato);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    respuesta = Convert.ToString(reader["ARC_RutaAdjunto"]);
                }
                sqlConn.Close();

                if (respuesta == "")
                {
                    var x = new ControllerException
                         (
                         COConstantesModulos.CLIENTES,
                         CLEnumTipoErrorCliente.EX_FALLO_ADJUNTAR_ARCHIVO.ToString(),
                         CLMensajesClientes.CargarMensaje(CLEnumTipoErrorCliente.EX_FALLO_ADJUNTAR_ARCHIVO)
                         );
                    throw new FaultException<ControllerException>(x);
                }
                else
                {
                    return respuesta;
                }
            }


            /*string respuesta;
            string query = "SELECT * FROM dbo.ArchivosContrato_CLI WHERE ARC_IdArchivo = " + archivo.IdArchivo;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringArchivo))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand(query, sqlConn);
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                if (dt.Rows.Count == 0)
                {
                    var x = new ControllerException
                         (
                         COConstantesModulos.CLIENTES,
                         CLEnumTipoErrorCliente.EX_FALLO_ADJUNTAR_ARCHIVO.ToString(),
                         CLMensajesClientes.CargarMensaje(CLEnumTipoErrorCliente.EX_FALLO_ADJUNTAR_ARCHIVO)
                         );
                    throw new FaultException<ControllerException>(x);
                }
                else
                    respuesta = Convert.ToBase64String(dt.Rows[0]["ARC_Adjunto"] as byte[]);

                sqlConn.Close();
                return respuesta;
            }*/
        }

        #endregion Archivos del contrato

        #region Archivos OtroSi

        /// <summary>
        /// Adiciona archivo de un otroSi
        /// </summary>
        /// <param name="archivo">objeto de tipo contrato</param>
        public void AdicionarArchivoOtroSi(CLContratosArchivosDC archivo)
        {
            //TODO PARRA DIGITALIZACION: comentareo dejando como estaba este metodo
            filePath = PAAdministrador.Instancia.ObtenerParametrosAplicacion(PAAdministrador.RutaArchivos);
            string rutaArchivo = Path.Combine(this.filePath, COConstantesModulos.MENSAJERIA, archivo.NombreServidor);
            Image imagenArchivo;
            using (FileStream fs = File.OpenRead(rutaArchivo))
            {
                imagenArchivo = Image.FromStream(fs);
                fs.Close();
            }
            string rutaImagenes = Framework.Servidor.ParametrosFW.PAParametros.Instancia.ConsultarParametrosFramework("FlArchivoOtro");
            string carpetaDestino = Path.Combine(rutaImagenes + "\\" + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year);
            if (!Directory.Exists(carpetaDestino))
            {
                Directory.CreateDirectory(carpetaDestino);
            }
            ImageCodecInfo jpgEncoder = UtilidadesFW.GetEncoder(ImageFormat.Jpeg);
            string ruta = carpetaDestino + "\\" + Guid.NewGuid() + ".jpg";
            System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 180L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            var im = new Bitmap(imagenArchivo);
            im.Save(ruta, jpgEncoder, myEncoderParameters);

            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paInsertarArchivosOtrosSi", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idOtroSi", archivo.IdContrato);
                cmd.Parameters.AddWithValue("@tipoDocumento", archivo.IdDocumento);
                cmd.Parameters.AddWithValue("@nombreAdjunto", archivo.NombreServidor);
                cmd.Parameters.AddWithValue("@rutaAdjunto", ruta);
                cmd.Parameters.AddWithValue("@idAdjunto", Guid.NewGuid());
                //cmd.Parameters.AddWithValue("@adjuntoSincronizado", retorno);
                //cmd.Parameters.AddWithValue("@fechaGrabacion", );
                cmd.Parameters.AddWithValue("@creadoPor", ControllerContext.Current.Usuario);
                cmd.ExecuteNonQuery();
                sqlConn.Close();
            }
            /*filePath = PAAdministrador.Instancia.ObtenerParametrosAplicacion(PAAdministrador.RutaArchivos);
            string rutaArchivo = Path.Combine(this.filePath, COConstantesModulos.CLIENTES, archivo.NombreServidor);
            byte[] archivoImagen;
            using (FileStream fs = File.OpenRead(rutaArchivo))
            {
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
                fs.Close();
                archivoImagen = bytes;
            }

            string query = "INSERT INTO [ArchivosOtrosSi_CLI] WITH (ROWLOCK)" +
            " ([AOS_Adjunto] ,[AOS_IdOtroSi]  ,[AOS_IdAdjunto]  ,[AOS_NombreAdjunto] ,[AOS_FechaGrabacion] ,[AOS_CreadoPor], [AOS_TipoDocumento])  " +
           " VALUES(@Adjunto ,@IdOtroSi ,@IdAdjunto,@NombreAdjunto ,GETDATE() ,@CreadoPor, @TipoDocumento)";

            using (SqlConnection sqlConn = new SqlConnection(conexionStringArchivo))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand(query, sqlConn);
                cmd.Parameters.Add(new SqlParameter("@NombreAdjunto", archivo.NombreAdjunto));
                cmd.Parameters.Add(new SqlParameter("@IdAdjunto", Guid.NewGuid()));
                cmd.Parameters.Add(new SqlParameter("@IdOtroSi", archivo.IdOtroSi.Value));
                cmd.Parameters.Add(new SqlParameter("@Adjunto", (object)archivoImagen));
                cmd.Parameters.Add(new SqlParameter("@CreadoPor", ControllerContext.Current.Usuario));
                cmd.Parameters.Add(new SqlParameter("@TipoDocumento", archivo.IdDocumento));
                cmd.ExecuteNonQuery();
                sqlConn.Close();
            }*/
        }

        /// <summary>
        /// Elimina archivo de un otroSi
        /// </summary>
        /// <param name="archivo">objeto de tipo contrato</param>
        public void EliminarArchivoOtroSi(CLContratosArchivosDC archivo)
        {
            /*string query = "DELETE FROM [ArchivosOtrosSi_CLI] WITH (ROWLOCK)" +
            "WHERE  AOS_IdArchivo = " + archivo.IdArchivo;

            using (SqlConnection sqlConn = new SqlConnection(conexionStringArchivo))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand(query, sqlConn);
                cmd.ExecuteNonQuery();
                sqlConn.Close();
            }*/

            string rutaArhivo = string.Empty;

            int resultado = 0;
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerRutasImagenArchivosOtrosSi_CLI", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdArchivo", archivo.IdArchivo);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    rutaArhivo = Convert.ToString(reader["AOS_RutaAdjunto"]);
                }
                sqlConn.Close();
            }
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paBorrarRutasImagenArchivosOtrosSI_CLI", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdArchivo", archivo.IdArchivo);
                resultado = cmd.ExecuteNonQuery();
                sqlConn.Close();
            }
            if (resultado > 0)
            {
                File.Delete(rutaArhivo);
            }



        }

        /// <summary>
        ///Metodo para obtener un archivo asociado a un otroSi
        /// </summary>
        /// <param name="archivo"></param>
        /// <returns></returns>
        /// Todo implementar no muestra archivo en otor si cliente
        public string ObtenerArchivoOtroSi(CLContratosArchivosDC archivo)
        {

            string respuesta = "";
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerRutasImagenArchivosContrato_CLI", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdArchivo", archivo.IdArchivo);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    respuesta = Convert.ToString(reader["AOS_RutaAdjunto"]);
                }
                sqlConn.Close();

                if (respuesta == "")
                {
                    var x = new ControllerException
                         (
                         COConstantesModulos.CLIENTES,
                         CLEnumTipoErrorCliente.EX_FALLO_ADJUNTAR_ARCHIVO.ToString(),
                         CLMensajesClientes.CargarMensaje(CLEnumTipoErrorCliente.EX_FALLO_ADJUNTAR_ARCHIVO)
                         );
                    throw new FaultException<ControllerException>(x);
                }
                else
                {
                    return respuesta;
                }
            }
            /*string respuesta;
            string query = "SELECT * FROM dbo.ArchivosOtrosSi_CLI WHERE AOS_IdArchivo = " + archivo.IdArchivo;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringArchivo))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand(query, sqlConn);
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                if (dt.Rows.Count == 0)
                {
                    var x = new ControllerException
                         (
                         COConstantesModulos.CLIENTES,
                         CLEnumTipoErrorCliente.EX_FALLO_ADJUNTAR_ARCHIVO.ToString(),
                         CLMensajesClientes.CargarMensaje(CLEnumTipoErrorCliente.EX_FALLO_ADJUNTAR_ARCHIVO)
                         );
                    throw new FaultException<ControllerException>(x);
                }
                else
                    respuesta = Convert.ToBase64String(dt.Rows[0]["AOS_Adjunto"] as byte[]);

                sqlConn.Close();
                return respuesta;
            }*/
        }

        #endregion Archivos OtroSi

        #region Sucursales del contrato

        /// <summary>
        /// Obtiene una lista con las sucursales para filtrar a partir de un contrato de un cliente
        /// </summary>
        /// <returns>Colección con las sucursales configuradas en la base de datos</returns>
        public IEnumerable<CLSucursalContratoDC> ObtenerSucursalesContratoFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, int idContrato)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Dictionary<LambdaExpression, OperadorLogico> where = new Dictionary<LambdaExpression, OperadorLogico>();
                LambdaExpression lamda = contexto.CrearExpresionLambda<SucursalesContrato_VCLI>("SUC_Contrato", idContrato.ToString(), OperadorComparacion.Equal);
                where.Add(lamda, OperadorLogico.And);
                if (!filtro.ContainsKey("SUC_Estado"))
                {
                    LambdaExpression lamda2 = contexto.CrearExpresionLambda<SucursalesContrato_VCLI>("SUC_Estado", ConstantesFramework.ESTADO_ACTIVO, OperadorComparacion.Equal);
                    where.Add(lamda2, OperadorLogico.And);
                }
                return contexto.ConsultarSucursalesContrato_VCLI(filtro, where, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                .ToList()
                .ConvertAll<CLSucursalContratoDC>(r =>
                {
                    List<CLSucursalHorarioDC> horarios = contexto.HorarioRecogidaSucursalContrato_CLI.Where(obj => obj.HRS_SucursalContrato == r.SUC_IdSucursalContrato).ToList().ConvertAll<CLSucursalHorarioDC>(h =>
                        new CLSucursalHorarioDC()
                        {
                            Hora = h.HRS_Hora,
                            IdDia = h.HRS_Dia.Trim(),
                        });

                    CLSucursalContratoDC succontra = new CLSucursalContratoDC
                    {
                        DebeRecoger = r.SUC_QueDebeRecoger,
                        EstadoDescripcion = contexto.MotivoEstados_PAR.Where(t => t.MOI_Estado == r.SUC_Estado).First().MOI_DescripcionEstado,
                        IdContrato = r.SUC_Contrato,
                        IdEstado = r.SUC_Estado,
                        IdSucursal = r.SUC_IdSucursal,
                        Nombre = r.SUC_Nombre,
                        IdSucursalContrato = r.SUC_IdSucursalContrato,
                        Localidad = r.LOC_Nombre,
                        Direccion = r.SUC_Direccion,
                        Telefono = r.SUC_Telefono,
                        Horario = horarios,
                    };
                    return succontra;
                }).ToList();
            }
        }

        /// <summary>
        /// Adiciona una sucursal a un contrato especifico
        /// </summary>
        /// <param name="sucursal"></param>
        public int AdicionarSucursalContrato(CLSucursalContratoDC sucursal)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                SucursalesContrato_CLI dato = new SucursalesContrato_CLI
                {
                    SUC_Contrato = sucursal.IdContrato,
                    SUC_Estado = sucursal.IdEstado,
                    SUC_QueDebeRecoger = sucursal.DebeRecoger,
                    SUC_Sucursal = sucursal.IdSucursal,
                    SUC_FechaGrabacion = DateTime.Now,
                    SUC_CreadoPor = ControllerContext.Current.Usuario
                };
                contexto.SucursalesContrato_CLI.Add(dato);
                contexto.SaveChanges();
                return dato.SUC_IdSucursalContrato;
            }
        }

        /// <summary>
        /// Modificar una sucursal de un contrato especifico
        /// </summary>
        /// <param name="sucursal"></param>
        public void ModificarSucursalContrato(CLSucursalContratoDC sucursal)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var dato = contexto.SucursalesContrato_CLI
               .Where(r => r.SUC_IdSucursalContrato == sucursal.IdSucursalContrato)
               .FirstOrDefault();
                dato.SUC_Contrato = sucursal.IdContrato;
                dato.SUC_Estado = sucursal.IdEstado;
                dato.SUC_IdSucursalContrato = sucursal.IdSucursalContrato;
                dato.SUC_QueDebeRecoger = sucursal.DebeRecoger;
                dato.SUC_Sucursal = sucursal.IdSucursal;
                CLRepositorioAudit.MapearAuditModificarSucursalContrato(contexto);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Elimina una sucursal de un contrato especifico
        /// </summary>
        /// <param name="sucursal"></param>
        public void EliminarSucursalContrato(CLSucursalContratoDC sucursal)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                SucursalesContrato_CLI dato = contexto.SucursalesContrato_CLI
               .Where(r => r.SUC_IdSucursalContrato == sucursal.IdSucursalContrato)
               .First();
                List<SucursalContraCambioEstado_CLI> estados = contexto.SucursalContraCambioEstado_CLI.Where(r => r.SUC_IdSucursalContrato == sucursal.IdSucursalContrato).ToList();
                estados.ForEach(e => contexto.SucursalContraCambioEstado_CLI.Remove(e));
                contexto.SucursalesContrato_CLI.Remove(dato);
                CLRepositorioAudit.MapearAuditModificarSucursalContrato(contexto);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Metodo para obtener los estados de una sucursal asociada a un contrato
        /// </summary>
        /// <param name="IdContrato"></param>
        /// <returns></returns>
        public IEnumerable<CLSucursalEstadosDC> ObtenerSucursalEstados(int IdContrato)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.SucursalContraCambioEstado_CLI
               .Where(t => t.SUC_IdSucursalContrato == IdContrato)
               .OrderBy(o => o.SUC_Estado)
               .ToList()
               .ConvertAll<CLSucursalEstadosDC>(r => new CLSucursalEstadosDC()
               {
                   Estado = r.SUC_Estado,
                   EstadoDescripcion = contexto.MotivoEstados_PAR.Where(t => t.MOI_Estado == r.SUC_Estado).First().MOI_DescripcionEstado,
                   Fecha = r.SUC_FechaGrabacion,
                   IdContrato = r.SUC_IdSucursalContrato,
                   Observaciones = r.SUC_ObservacionEstado
               });
            }
        }

        /// <summary>
        /// Metodo para guardar los cambios de estado de una sucursal asociada a un contrato
        /// </summary>
        /// <param name="Estado"></param>
        public void AdicionarSucursalEstados(CLSucursalContratoDC sucursal)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                SucursalContraCambioEstado_CLI estado = new SucursalContraCambioEstado_CLI()
                {
                    SUC_Estado = sucursal.EstadoDetalle.Estado,
                    SUC_IdSucursalContrato = sucursal.IdSucursalContrato,
                    SUC_ObservacionEstado = sucursal.EstadoDetalle.Observaciones,
                    SUC_FechaGrabacion = DateTime.Now,
                    SUC_CreadoPor = ControllerContext.Current.Usuario
                };
                contexto.SucursalContraCambioEstado_CLI.Add(estado);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Metodo encargado de controlar los horarios de recogida de una sucursal
        /// </summary>
        public void HorariosSucursal(CLSucursalContratoDC sucursal)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var horarios = contexto.HorarioRecogidaSucursalContrato_CLI.Where(obj => obj.HRS_SucursalContrato == sucursal.IdSucursalContrato).ToList();
                for (int i = horarios.Count - 1; i >= 0; i--)
                {
                    contexto.HorarioRecogidaSucursalContrato_CLI.Remove(horarios[i]);
                }

                sucursal.Horario.ToList().ForEach(obj =>
                {
                    contexto.HorarioRecogidaSucursalContrato_CLI.Add(new HorarioRecogidaSucursalContrato_CLI()
                    {
                        HRS_CreadoPor = ControllerContext.Current.Usuario,
                        HRS_FechaGrabacion = DateTime.Now,
                        HRS_Hora = obj.Hora,
                        HRS_SucursalContrato = sucursal.IdSucursalContrato,
                        HRS_Dia = obj.IdDia
                    });
                });

                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Obtiene una lista con las sucursales de un cliente que no esten en un contrato especifico
        /// </summary>
        /// <returns>Colección con las sucursales configuradas en la base de datos</returns>
        public IEnumerable<CLSucursalDC> ObtenerSucursalesFiltroExcepcion(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina, int idContrato, int idCliente)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                string IdSucursal;
                string Nombre;
                filtro.TryGetValue("SUC_IdSucursal", out IdSucursal);
                filtro.TryGetValue("SUC_Nombre", out Nombre);
                var Sucursales = contexto.paObtenerSucContrato_CLI(indicePagina, registrosPorPagina, idCliente, idContrato, Convert.ToInt32(IdSucursal), Nombre).ToList();
                if (Sucursales.Any())
                {
                    return Sucursales
                       .ToList()
                       .ConvertAll<CLSucursalDC>(r =>
                       {
                           Localidades_VPAR localidad = contexto.Localidades_VPAR.Where(t => t.LOC_IdLocalidad == r.SUC_Municipio).FirstOrDefault();

                           CentroServicios_PUA centro = contexto.CentroServicios_PUA.Where(l => l.CES_IdCentroServicios == r.SUC_AgenciaEncargada).FirstOrDefault();

                           CLSucursalDC propi = new CLSucursalDC
                           {
                               IdCliente = r.SUC_ClienteCredito,
                               Direccion = r.SUC_Direccion,
                               Fax = r.SUC_Fax,
                               Ciudad = new PALocalidadDC { IdLocalidad = r.SUC_Municipio, Nombre = r.LOC_Nombre },
                               Pais = new PALocalidadDC { },
                               Telefono = r.SUC_Telefono,
                               Agencia = r.SUC_AgenciaEncargada,
                               Nombre = r.SUC_Nombre,
                               IdSucursal = r.SUC_IdSucursal,
                               Localidad = r.SUC_Municipio,
                               NombreLocalidad = localidad.LOC_Nombre,
                               ListaZonas = ConsultarZonasDeLocalidadXLocalidad(r.SUC_Municipio),
                               IdBodega = r.SUC_IdBodega
                           };

                           switch (localidad.LOC_IdTipo)
                           {
                               case ConstantesClientes.TIPO_LOCALIDAD_1:
                                   propi.Pais.IdLocalidad = localidad.LOC_IdLocalidad;
                                   propi.Pais.Nombre = localidad.LOC_Nombre;
                                   break;

                               case ConstantesClientes.TIPO_LOCALIDAD_2:
                                   propi.Pais.IdLocalidad = localidad.LOC_IdAncestroPrimerGrado;
                                   propi.Pais.Nombre = localidad.LOC_NombrePrimero;
                                   break;

                               case ConstantesClientes.TIPO_LOCALIDAD_3:
                                   propi.Pais.IdLocalidad = localidad.LOC_IdAncestroSegundoGrado;
                                   propi.Pais.Nombre = localidad.LOC_NombreSegundo;
                                   break;

                               default:
                                   propi.Pais.IdLocalidad = localidad.LOC_IdAncestroTercerGrado;
                                   propi.Pais.Nombre = localidad.LOC_NombreTercero;
                                   break;
                           }
                           return propi;
                       });
                }
                else
                    return
                      new List<CLSucursalDC>();
            }
        }

        #endregion Sucursales del contrato

        #region Facturas

        /// <summary>
        /// Metodo encargado de traer los tipos de notacion para la factura
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CLTipoNotacionDC> ObtenerTipoNotacion()
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TipoNotacionFacturacion_CLI
                  .OrderBy(o => o.TNF_IdTipoNotacion)
                  .ToList()
                  .ConvertAll<CLTipoNotacionDC>
                  (r => new CLTipoNotacionDC()
                  {
                      Idtipo = r.TNF_IdTipoNotacion,
                      Descripcion = r.TNF_Descripcion,
                  });
            }
        }

        /// <summary>
        /// Obtiene una lista con las sucursales para filtrar a partir de un contrato de un cliente
        /// </summary>
        /// <returns>Colección con las sucursales configuradas en la base de datos</returns>
        public IEnumerable<CLFacturaDC> ObtenerFacturasContratoFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, int idContrato)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Dictionary<LambdaExpression, OperadorLogico> where = new Dictionary<LambdaExpression, OperadorLogico>();
                LambdaExpression lamda = contexto.CrearExpresionLambda<ParametrosFactura_CLI>("PAF_Contrato", idContrato.ToString(), OperadorComparacion.Equal);
                where.Add(lamda, OperadorLogico.And);

                return contexto.ConsultarParametrosFactura_CLI(filtro, where, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                .ToList()
                .ConvertAll<CLFacturaDC>
                (r =>
                {
                    NotacionesFacturacion_CLI diaFacturacion = contexto.NotacionesFacturacion_CLI.Where(d1 => d1.NOF_IdNotacion == r.PAF_DiaFacturacion).FirstOrDefault();
                    NotacionesFacturacion_CLI diaPago = contexto.NotacionesFacturacion_CLI.Where(d1 => d1.NOF_IdNotacion == r.PAF_DiaPagoFactura).FirstOrDefault();
                    NotacionesFacturacion_CLI diaRadicacion = contexto.NotacionesFacturacion_CLI.Where(d1 => d1.NOF_IdNotacion == r.PAF_DiaRadicacion).FirstOrDefault();
                    Localidades_VPAR localidadRadicacion = contexto.Localidades_VPAR.Where(t => t.LOC_IdLocalidad == r.PAF_CiudadRadicacion).Single();

                    CLFacturaDC factura = new CLFacturaDC
                    {
                        DiaFacturacion = new CLFacturaNotacionDC()
                        {
                            DiaOrdinal = diaFacturacion.NOF_DiaOrdinal,
                            IdDia = diaFacturacion.NOF_DiaSemana.Trim(),
                            IdNotacion = r.PAF_DiaFacturacion,
                            IdSemana = diaFacturacion.NOF_Semana,
                            IdTipo = diaFacturacion.NOF_TipoNotacion
                        },
                        DiaPagoFactura = new CLFacturaNotacionDC()
                        {
                            DiaOrdinal = diaPago.NOF_DiaOrdinal,
                            IdDia = diaPago.NOF_DiaSemana.Trim(),
                            IdNotacion = r.PAF_DiaPagoFactura,
                            IdSemana = diaPago.NOF_Semana,
                            IdTipo = diaPago.NOF_TipoNotacion
                        },
                        DiaRadicacion = new CLFacturaNotacionDC()
                        {
                            DiaOrdinal = diaRadicacion.NOF_DiaOrdinal,
                            IdDia = diaRadicacion.NOF_DiaSemana.Trim(),
                            IdNotacion = r.PAF_DiaRadicacion,
                            IdSemana = diaRadicacion.NOF_Semana,
                            IdTipo = diaRadicacion.NOF_TipoNotacion
                        },
                        DireccionRadicacion = r.PAF_DireccionRadicacion,
                        DirigidoA = r.PAF_DirigidoA,
                        FinalCorte = r.PAF_DiaFinalCorte,
                        InicioCorte = r.PAF_DiaInicialCorte,
                        LocalidadRadicacion = new PALocalidadDC { IdLocalidad = r.PAF_CiudadRadicacion, Nombre = localidadRadicacion.LOC_Nombre },
                        PaisRadicacion = new PALocalidadDC(),
                        NombreFactura = r.PAF_NombreFactura,
                        PlazoPago = r.PAF_PlazoPago,
                        PorDescuentoProntoPago = r.PAF_PorcDescProntoPago,
                        TelefonoRadicacion = r.PAF_TelefonoRadicacion,
                        IdFormaPago = r.PAF_FormaPago,
                        IdFactura = r.PAF_IdParFacturacion,
                        IdContrato = r.PAF_Contrato,
                    };

                    switch (localidadRadicacion.LOC_IdTipo)
                    {
                        case "1":
                            factura.PaisRadicacion.IdLocalidad = localidadRadicacion.LOC_IdLocalidad;
                            factura.PaisRadicacion.Nombre = localidadRadicacion.LOC_Nombre;
                            break;

                        case "2":
                            factura.PaisRadicacion.IdLocalidad = localidadRadicacion.LOC_IdAncestroPrimerGrado;
                            factura.PaisRadicacion.Nombre = localidadRadicacion.LOC_NombrePrimero;
                            break;

                        case "3":
                            factura.PaisRadicacion.IdLocalidad = localidadRadicacion.LOC_IdAncestroSegundoGrado;
                            factura.PaisRadicacion.Nombre = localidadRadicacion.LOC_NombreSegundo;
                            break;

                        default:
                            factura.PaisRadicacion.IdLocalidad = localidadRadicacion.LOC_IdAncestroTercerGrado;
                            factura.PaisRadicacion.Nombre = localidadRadicacion.LOC_NombreTercero;
                            break;
                    }

                    return factura;
                }).ToList();
            }
        }

        /// <summary>
        /// Metodo para insertar contratos en el repositorio
        /// </summary>
        /// <param name="sucursal">objeto de tipo contrato</param>
        public int AdicionarFactura(CLFacturaDC factura)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ParametrosFactura_CLI dato = new ParametrosFactura_CLI()
                {
                    PAF_CiudadRadicacion = factura.LocalidadRadicacion.IdLocalidad,
                    PAF_Contrato = factura.ContratoSeleccionado.IdContrato,
                    PAF_DiaFacturacion = AdicionarNotacion(factura.DiaFacturacion),
                    PAF_DiaPagoFactura = AdicionarNotacion(factura.DiaPagoFactura),
                    PAF_DiaRadicacion = AdicionarNotacion(factura.DiaRadicacion),
                    PAF_DiaInicialCorte = factura.InicioCorte,
                    PAF_DiaFinalCorte = factura.FinalCorte,
                    PAF_TelefonoRadicacion = factura.TelefonoRadicacion,
                    PAF_DireccionRadicacion = factura.DireccionRadicacion,
                    PAF_DirigidoA = factura.DirigidoA,
                    PAF_NombreFactura = factura.NombreFactura,
                    PAF_FormaPago = factura.IdFormaPago,
                    PAF_PlazoPago = factura.PlazoPago,
                    PAF_PorcDescProntoPago = factura.PorDescuentoProntoPago,
                    PAF_FechaGrabacion = DateTime.Now,
                    PAF_CreadoPor = ControllerContext.Current.Usuario,
                };
                contexto.ParametrosFactura_CLI.Add(dato);
                contexto.SaveChanges();
                return dato.PAF_IdParFacturacion;
            }
        }

        /// <summary>
        /// Metodo para eliminar una factura
        /// </summary>
        /// <param name="factura"></param>
        public void EliminarFatura(CLFacturaDC factura)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ParametrosFactura_CLI facturaBorrar = contexto.ParametrosFactura_CLI.FirstOrDefault(fac => fac.PAF_IdParFacturacion == factura.IdFactura);

                if (facturaBorrar != null)
                {
                    contexto.ParametrosFactura_CLI.Remove(facturaBorrar);
                    CLRepositorioAudit.MapearAuditModificarParametrosFactura(contexto);
                    contexto.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Borra la info en la tabla
        /// agrupacion factura
        /// </summary>
        /// <param name="idParFactura">identificador </param>
        public void EliminarAgrupacionFactura(int idParFactura)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                List<AgrupacionFactura_CLI> lstAgruBorrar = contexto.AgrupacionFactura_CLI.Where(agr => agr.AGF_IdParFacturacion == idParFactura).ToList();

                if (lstAgruBorrar != null && lstAgruBorrar.Count > 0)
                {
                    lstAgruBorrar.ForEach(agr =>
                    {
                        contexto.AgrupacionFactura_CLI.Remove(agr);
                        contexto.SaveChanges();
                    });
                }
            }
        }

        /// <summary>
        /// Metodo para modificar facturas de un contrato
        /// </summary>
        /// <param name="sucursal">objeto de tipo contrato</param>
        public void ModificarFactura(CLFacturaDC factura)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ParametrosFactura_CLI dato = contexto.ParametrosFactura_CLI
                  .Where(r => r.PAF_IdParFacturacion == factura.IdFactura)
                  .FirstOrDefault();

                if (dato == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.CLIENTES, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }
                dato.PAF_Contrato = factura.ContratoSeleccionado.IdContrato;
                dato.PAF_CiudadRadicacion = factura.LocalidadRadicacion.IdLocalidad;
                dato.PAF_DiaFacturacion = factura.DiaFacturacion.IdNotacion;
                dato.PAF_DiaPagoFactura = factura.DiaPagoFactura.IdNotacion;
                dato.PAF_DiaRadicacion = factura.DiaRadicacion.IdNotacion;
                dato.PAF_DiaInicialCorte = factura.InicioCorte;
                dato.PAF_DiaFinalCorte = factura.FinalCorte;
                dato.PAF_TelefonoRadicacion = factura.TelefonoRadicacion;
                dato.PAF_DireccionRadicacion = factura.DireccionRadicacion;
                dato.PAF_DirigidoA = factura.DirigidoA;
                dato.PAF_NombreFactura = factura.NombreFactura;
                dato.PAF_FormaPago = factura.IdFormaPago;
                dato.PAF_PlazoPago = factura.PlazoPago;
                dato.PAF_PorcDescProntoPago = factura.PorDescuentoProntoPago;
                CLRepositorioAudit.MapearAuditModificarParametrosFactura(contexto);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Metodo para adicionar notaciones a una factura de un contrato
        /// </summary>
        /// <param name="notacion"></param>
        /// <returns></returns>
        public short AdicionarNotacion(CLFacturaNotacionDC notacion)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                NotacionesFacturacion_CLI dato = new NotacionesFacturacion_CLI()
                {
                    NOF_DiaSemana = notacion.IdDia,
                    NOF_TipoNotacion = notacion.IdTipo,
                    NOF_DiaOrdinal = notacion.DiaOrdinal,
                    NOF_Semana = notacion.IdSemana,
                    NOF_FechaGrabacion = DateTime.Now,
                    NOF_CreadoPor = ControllerContext.Current.Usuario,
                };
                contexto.NotacionesFacturacion_CLI.Add(dato);
                contexto.SaveChanges();
                return dato.NOF_IdNotacion;
            }
        }

        /// <summary>
        /// Metodo para eliminar notaciones a una factura  de un contrato
        /// </summary>
        /// <param name="notacion"></param>
        public void ModificarNotacion(CLFacturaNotacionDC notacion)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var dato = contexto.NotacionesFacturacion_CLI
                  .Where(r => r.NOF_IdNotacion == notacion.IdNotacion)
                  .FirstOrDefault();
                dato.NOF_DiaOrdinal = notacion.DiaOrdinal;
                dato.NOF_DiaSemana = notacion.IdDia;
                dato.NOF_Semana = notacion.IdSemana;
                dato.NOF_TipoNotacion = notacion.IdTipo;
                CLRepositorioAudit.MapearAuditModificarNotacionFactura(contexto);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Metodo para obtener la fecha de corte de la facturacion
        /// </summary>
        /// <param name="IdContrato"></param>
        /// <returns>datetime con la fecha de facturacion</returns>
        public DateTime ObtenerFechaCorteFacturacion(int IdContrato)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                IQueryable<ParametrosFactura_CLI> listaNotacion;
                DateTime fecha;

                listaNotacion = contexto.ParametrosFactura_CLI
                    .Where(e => e.PAF_Contrato == IdContrato);

                int month = DateTime.Now.Month;
                int day = Convert.ToInt32(listaNotacion.Min(c => c.PAF_DiaFinalCorte));

                // Con la siguiente validación, aseguro que no se intente convertir una fecha en forma inapropiada
                if (month == 2)
                {
                    if (DateTime.IsLeapYear(DateTime.Now.Year) && day > 29)
                    {
                        day = 29;
                    }
                    else if (day > 28)
                    {
                        day = 28;
                    }
                }
                if ((month == 4 || month == 6 || month == 8 || month == 9 || month == 11) && day > 30)
                {
                    day = 30;
                }
                if ((month == 1 || month == 3 || month == 5 || month == 7 || month == 10 || month == 12) && day > 31)
                {
                    day = 31;
                }
                fecha = new DateTime(DateTime.Now.Year, month, day);

                if (DateTime.Now > fecha)
                {
                    return fecha;
                }
                else
                {
                    return fecha.AddMonths(-1);
                }
            }
        }

        #endregion Facturas

        #region Requisitos de la factura

        /// <summary>
        /// Obtiene una lista con los requisitos para filtrar a partir de una factura de un contrato
        /// </summary>
        /// <returns>Colección con los requisitos configuradas en la base de datos</returns>
        public IEnumerable<CLFacturaRequisitosDC> ObtenerRequisitosFacturas(int idFactura)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                IEnumerable<CLFacturaRequisitosDC> infoRequi = contexto.RequisitosRadFactura_CLI.Where(r => r.RRF_IdParFacturacion == idFactura)
                 .ToList()
                 .ConvertAll<CLFacturaRequisitosDC>
                 (r =>
                 {
                     CLFacturaRequisitosDC requisito = new CLFacturaRequisitosDC
                     {
                         Descripcion = r.RRF_Descripcion,
                         IdRequisito = r.RRF_IdRequisito,
                         IdFactura = r.RRF_IdParFacturacion,
                     };

                     return requisito;
                 }).ToList();

                return infoRequi;
            }
        }

        /// <summary>
        /// Metodo para adicionar requisitos a una factura
        /// </summary>
        /// <param name="requisito"></param>
        public void AdicionarRequisitosFacturas(CLFacturaRequisitosDC requisito)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                RequisitosRadFactura_CLI dato = new RequisitosRadFactura_CLI()
                {
                    RRF_Descripcion = requisito.Descripcion,
                    RRF_IdParFacturacion = requisito.IdFactura,
                    RRF_FechaGrabacion = DateTime.Now,
                    RRF_CreadoPor = ControllerContext.Current.Usuario,
                };
                contexto.RequisitosRadFactura_CLI.Add(dato);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Metodo para eliminar requisitos de una factura
        /// </summary>
        /// <param name="requisito"></param>
        public void EliminarRequisitosFacturas(CLFacturaRequisitosDC requisito)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                RequisitosRadFactura_CLI dato = contexto.RequisitosRadFactura_CLI.Where(r => r.RRF_IdRequisito == requisito.IdRequisito).FirstOrDefault();
                if (dato != null)
                {
                    contexto.RequisitosRadFactura_CLI.Remove(dato);
                    contexto.SaveChanges();
                }
                else
                {
                    var x = new ControllerException
                      (
                      COConstantesModulos.CLIENTES,
                      CLEnumTipoErrorCliente.EX_FALLO_ELIMINAR_REQUISITO.ToString(),
                      CLMensajesClientes.CargarMensaje(CLEnumTipoErrorCliente.EX_FALLO_ELIMINAR_REQUISITO)
                      );
                    throw new FaultException<ControllerException>(x);
                }
            }
        }

        /// <summary>
        // Metodo para eliminar requisitos de una factura
        //por la factura
        /// </summary>
        /// <param name="idParFactura">factura</param>
        public void EliminarRequisitosFacturasXFactura(int idParFactura)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                RequisitosRadFactura_CLI requisitoBorrar = contexto.RequisitosRadFactura_CLI.Where(r => r.RRF_IdParFacturacion == idParFactura).FirstOrDefault();
                if (requisitoBorrar != null)
                {
                    contexto.RequisitosRadFactura_CLI.Remove(requisitoBorrar);
                    contexto.SaveChanges();
                }
            }
        }

        #endregion Requisitos de la factura

        #region Descuentos de la factura

        /// <summary>
        /// Obtiene una lista con los descuentos para filtrar a partir de una factura de un contrato
        /// </summary>
        /// <returns>Colección con los descuentos configuradas en la base de datos</returns>
        public IEnumerable<CLFacturaDescuentoDC> ObtenerDescuentosFacturas(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, int idFactura)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Dictionary<LambdaExpression, OperadorLogico> where = new Dictionary<LambdaExpression, OperadorLogico>();
                LambdaExpression lamda = contexto.CrearExpresionLambda<DescuentosFactura_CLI>("DEF_IdParFacturacion", idFactura.ToString(), OperadorComparacion.Equal);
                where.Add(lamda, OperadorLogico.And);

                return contexto.ConsultarDescuentosFactura_CLI(filtro, where, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                     .ToList()

                .ConvertAll<CLFacturaDescuentoDC>
                (r =>
                {
                    CLFacturaDescuentoDC descuento = new CLFacturaDescuentoDC
                    {
                        IdDescuento = r.DEF_IdDescuento,
                        IdFactura = r.DEF_IdParFacturacion,
                        Motivo = r.DEF_Motivo,
                        FechaAplicacion = r.DEF_FechaAplicacion,
                        Ano = r.DEF_FechaAplicacion.Year,
                        Mes = Convert.ToInt16(r.DEF_FechaAplicacion.Month),
                        Valor = r.DEF_Valor,
                    };

                    return descuento;
                }).ToList();
            }
        }

        /// <summary>
        /// Obtiene una lista con los descuentos para filtrar a partir de una factura de un contrato
        /// </summary>
        /// <returns>Colección con los descuentos configuradas en la base de datos</returns>
        public List<CLFacturaDescuentoDC> ObtenerDescuentosFacturas(int idFactura)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.DescuentosFactura_CLI.Where(d => d.DEF_IdParFacturacion == idFactura)
                     .ToList()

                .ConvertAll<CLFacturaDescuentoDC>
                (r =>
                {
                    CLFacturaDescuentoDC descuento = new CLFacturaDescuentoDC
                    {
                        IdDescuento = r.DEF_IdDescuento,
                        IdFactura = r.DEF_IdParFacturacion,
                        Motivo = r.DEF_Motivo,
                        FechaAplicacion = r.DEF_FechaAplicacion,
                        Ano = r.DEF_FechaAplicacion.Year,
                        Mes = Convert.ToInt16(r.DEF_FechaAplicacion.Month),
                        Valor = r.DEF_Valor,
                    };

                    return descuento;
                }).ToList();
            }
        }

        /// <summary>
        /// Metodo para adicionar descuentos a una factura
        /// </summary>
        /// <param name="descuento"></param>
        public void AdicionarDescuentoFactura(CLFacturaDescuentoDC descuento)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                DescuentosFactura_CLI dato = new DescuentosFactura_CLI()
                {
                    DEF_IdParFacturacion = descuento.IdFactura,
                    DEF_Motivo = descuento.Motivo,
                    DEF_Valor = descuento.Valor,
                    DEF_FechaAplicacion = descuento.FechaAplicacion,
                    DEF_FechaGrabacion = DateTime.Now,
                    DEF_CreadoPor = ControllerContext.Current.Usuario,
                };
                contexto.DescuentosFactura_CLI.Add(dato);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Metodo para modificar descuentos a una factura
        /// </summary>
        /// <param name="descuento"></param>
        public void ModificarDescuentoFactura(CLFacturaDescuentoDC descuento)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var dato = contexto.DescuentosFactura_CLI
              .Where(r => r.DEF_IdDescuento == descuento.IdDescuento)
              .FirstOrDefault();
                dato.DEF_IdParFacturacion = descuento.IdFactura;
                dato.DEF_Motivo = descuento.Motivo;
                dato.DEF_Valor = descuento.Valor;
                dato.DEF_FechaAplicacion = descuento.FechaAplicacion;
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Elimina el descuento de una factura por
        /// el numero de la factura
        /// </summary>
        /// <param name="idParFactura">factura</param>
        public void EliminarDescuentoFacturaPorFactura(int idParFactura)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                DescuentosFactura_CLI descuBorrar = contexto.DescuentosFactura_CLI.FirstOrDefault(desc => desc.DEF_IdParFacturacion == idParFactura);

                if (descuBorrar != null)
                {
                    contexto.DescuentosFactura_CLI.Remove(descuBorrar);
                    contexto.SaveChanges();
                }
            }
        }

        #endregion Descuentos de la factura

        #region Servicios de la factura

        /// <summary>
        /// Obtiene los servicios de una factura
        /// </summary>
        /// <param name="factura"></param>
        /// <returns></returns>
        public IEnumerable<CLFacturaServiciosDC> ObtenerServiciosFactura(CLFacturaDC factura)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var servicios = contexto.AgrupacionFacturaSvcSucur_VCLI
                  .Where(r => r.AGF_IdParFacturacion == factura.IdFactura)
                  .ToList();

                var serviciosFactura = servicios.GroupBy(s => s.AGF_IdServicio).Select(s => s.First())
                  .ToList()
                  .ConvertAll<CLFacturaServiciosDC>
                   (r =>
                   {
                       CLFacturaServiciosDC servicio = new CLFacturaServiciosDC
                       {
                           IdContrato = r.AGF_IdContrato,
                           IdFactura = r.AGF_IdParFacturacion,
                           IdServicio = r.AGF_IdServicio,
                           Nombre = r.SER_Nombre,
                       };
                       return servicio;
                   }).ToList();

                return serviciosFactura;
            }
        }

        /// <summary>
        /// Obtiene una lista con las sucursales para filtrar a partir de un servicio de un contrato
        /// </summary>
        /// <returns>Colección con las sucursales configuradas en la base de datos</returns>
        public IEnumerable<CLSucursalServicioDC> ObtenerSucursalesServicioFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, CLFacturaServiciosDC servicioFactura)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Dictionary<LambdaExpression, OperadorLogico> where = new Dictionary<LambdaExpression, OperadorLogico>();
                LambdaExpression lamda = contexto.CrearExpresionLambda<AgrupacionFacturaSvcSucur_VCLI>("AGF_IdContrato", servicioFactura.IdContrato.ToString(), OperadorComparacion.Equal);
                where.Add(lamda, OperadorLogico.And);

                lamda = contexto.CrearExpresionLambda<AgrupacionFacturaSvcSucur_VCLI>("AGF_IdServicio", servicioFactura.IdServicio.ToString(), OperadorComparacion.Equal);
                where.Add(lamda, OperadorLogico.And);

                lamda = contexto.CrearExpresionLambda<AgrupacionFacturaSvcSucur_VCLI>("AGF_IdParFacturacion", servicioFactura.IdFactura.ToString(), OperadorComparacion.Equal);
                where.Add(lamda, OperadorLogico.And);

                return contexto.ConsultarAgrupacionFacturaSvcSucur_VCLI(filtro, where, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                .ToList()
                .ConvertAll<CLSucursalServicioDC>(r =>
                {
                    CLSucursalServicioDC succontra = new CLSucursalServicioDC
                    {
                        IdSucursal = r.AGF_IdSucursal,
                        Nombre = r.SUC_Nombre,
                        Localidad = r.LOC_Nombre,
                        Direccion = r.SUC_Direccion,
                        Telefono = r.SUC_Telefono,
                    };
                    return succontra;
                }).ToList();
            }
        }

        /// <summary>
        /// Método para obtener las sucursales excluidas de una factura
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="servicioFactura"></param>
        /// <returns></returns>
        public IEnumerable<CLSucursalServicioDC> ObtenerSucursalesServicioExcluidosFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, CLFacturaServiciosDC servicioFactura)
        {
            string idSucursal;
            string nombreSucursal;

            filtro.TryGetValue("idSucursal", out idSucursal);
            filtro.TryGetValue("nombreSucursal", out nombreSucursal);

            if (nombreSucursal != null)
                nombreSucursal = "%" + nombreSucursal + "%";
            if (idSucursal == null)
                idSucursal = "0";

            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var listaSucursales = contexto.paObtenerSucursalesFacExcluidas_CLI(indicePagina, registrosPorPagina, servicioFactura.IdContrato, servicioFactura.IdFactura, servicioFactura.IdServicio, Convert.ToInt32(idSucursal), nombreSucursal).ToList();

                if (listaSucursales.Any())
                {
                    return listaSucursales.ConvertAll<CLSucursalServicioDC>(r =>
                    {
                        CLSucursalServicioDC succontra = new CLSucursalServicioDC
                        {
                            IdSucursal = r.SUC_Sucursal,
                            Nombre = r.SUC_Nombre,
                        };
                        return succontra;
                    }).ToList();
                }
                else
                    return new List<CLSucursalServicioDC>();
            }
        }

        public void AdicionarSucursalesServicio(CLFacturaServiciosDC servicioSucursal)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                AgrupacionFactura_CLI dato = new AgrupacionFactura_CLI()
                {
                    AGF_IdContrato = servicioSucursal.IdContrato,
                    AGF_IdParFacturacion = servicioSucursal.IdFactura,
                    AGF_IdServicio = servicioSucursal.IdServicio,
                    AGF_IdSucursal = servicioSucursal.IdSucursal,
                    AGF_FechaGrabacion = DateTime.Now,
                    AGF_CreadoPor = ControllerContext.Current.Usuario,
                };
                contexto.AgrupacionFactura_CLI.Add(dato);
                contexto.SaveChanges();
            }
        }

        public void EliminarSucursalesServicio(CLFacturaServiciosDC servicioSucursal)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                AgrupacionFactura_CLI dato = contexto.AgrupacionFactura_CLI
                 .Where(r => r.AGF_IdContrato == servicioSucursal.IdContrato && r.AGF_IdParFacturacion == servicioSucursal.IdFactura && r.AGF_IdServicio == servicioSucursal.IdServicio && r.AGF_IdSucursal == servicioSucursal.IdSucursal)
                 .First();
                contexto.AgrupacionFactura_CLI.Remove(dato);
                contexto.SaveChanges();
            }
        }

        public void EliminarServicio(CLFacturaServiciosDC servicioSucursal)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                List<AgrupacionFactura_CLI> dato = contexto.AgrupacionFactura_CLI
                  .Where(r => r.AGF_IdContrato == servicioSucursal.IdContrato && r.AGF_IdParFacturacion == servicioSucursal.IdFactura && r.AGF_IdServicio == servicioSucursal.IdServicio)
                  .ToList();
                dato.ForEach(r =>
                contexto.AgrupacionFactura_CLI.Remove(r));
                contexto.SaveChanges();
            }
        }

        #endregion Servicios de la factura

        #region Consultas Suministros

        /// <summary>
        /// Metodo que valida si un suministro esta asignado a una sucursal
        /// </summary>
        /// <param name="sucursal"></param>
        /// <param name="suministro"></param>
        /// <returns></returns>
        public int ObtenerSuministrosSucursal(int sucursal, int suministro)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var a = contexto.paObtenerSucursalSuminist_CLI(sucursal, suministro).FirstOrDefault();
                return a == null ? 0 : a.SUS_IdSucursal;
            }
        }

        /// <summary>
        /// Metodo  que valida si un suministro con un serial esta asignado
        /// </summary>
        /// <param name="Serial"></param>
        /// <param name="suministro"></param>
        /// <returns></returns>
        public int ObtenerSucursalSuministroSerial(long Serial, int suministro)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var a = contexto.paObtenerSucursalSumSerial_CLI(Serial, suministro).FirstOrDefault();
                return a == null ? 0 : a.Value;
            }
        }

        #endregion Consultas Suministros

        #region Validaciones

        /// <summary>
        /// Valida si una identificacion se encuentra en listas restrictivas
        /// </summary>
        /// <param name="identificacion"></param>
        /// <returns></returns>
        public bool ValidarNitExistente(string identificacion)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ClienteCredito_CLI lista = contexto.ClienteCredito_CLI.Where(l => l.CLI_Nit == identificacion).FirstOrDefault();
                if (lista != null)
                {
                    return true;
                }
                else
                    return false;
            }
        }

        #endregion Validaciones

        #region Cliente Agencias Contrato

        /// <summary>
        /// Obtiene los clientes y sus contratos por agencia
        /// </summary>
        /// <param name="idAgencia">Identificador Agencia</param>
        /// <returns>Colección clientes y contratos</returns>
        public List<CLClientesDC> ObtenerClientesContratosXAgencia(long idAgencia)
        {   // todo:??
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var consulta = contexto.paObtenerCliContratoAgen_CLI(idAgencia)
                  .GroupBy(g => g.SUC_IdSucursal)
                    .Select(s => new CLClientesDC()
                    {
                        IdCliente = s.First().CLI_IdCliente,
                        RazonSocial = s.First().CLI_RazonSocial,
                        Nit = s.First().CLI_Nit,
                        Sucursales = s.GroupBy(g => g.CON_IdContrato).Select(c => new CLSucursalDC()
                        {
                            ContratosSucursal = c.ToList()
                                                  .ConvertAll(contrato => new CLContratosDC
                                                  {
                                                      IdContrato = contrato.CON_IdContrato,
                                                      NombreContrato = contrato.CON_NombreContrato,
                                                      ListaPrecios = contrato.CON_ListaPrecios
                                                  }),
                            IdSucursal = c.First().SUC_IdSucursal,
                            Nombre = c.First().SUC_Nombre,
                            Agencia = c.First().SUC_AgenciaEncargada
                        }).ToList()
                    });

                if (consulta != null)
                    return consulta.ToList();
                else
                    return new List<CLClientesDC>();
            }
        }

        // todo:id para Masivos. (1)Obtiene ClientesCredito
        public List<CLClientesDC> ObtenerClientesCreditoXAgencia(long idAgencia)
        {
            List<CLClientesDC> lst = null;

            using (SqlConnection cnn = new SqlConnection(CadCnxController))
            {
                string cadSql = @"SELECT DISTINCT ClienteCredito_CLI.CLI_IdCliente, ClienteCredito_CLI.CLI_RazonSocial, ClienteCredito_CLI.CLI_Nit"
                            + " FROM ClienteCredito_CLI WITH (NOLOCK) "
                            + "     INNER JOIN Sucursal_CLI WITH (NOLOCK) ON ClienteCredito_CLI.CLI_IdCliente = Sucursal_CLI.SUC_ClienteCredito"
                            + "     INNER JOIN SucursalesContrato_CLI WITH (NOLOCK) ON Sucursal_CLI.SUC_IdSucursal = SucursalesContrato_CLI.SUC_Sucursal"
                            + "     INNER JOIN Contrato_CLI WITH (NOLOCK) ON ClienteCredito_CLI.CLI_IdCliente = Contrato_CLI.CON_ClienteCredito"
                            + "                                     AND SucursalesContrato_CLI.SUC_Contrato = Contrato_CLI.CON_IdContrato"
                            + " WHERE (Sucursal_CLI.SUC_AgenciaEncargada = @IdAgencia) AND ClienteCredito_CLI.CLI_Estado = 'ACT' AND Contrato_CLI.CON_FechaFinConExtensiones > GETDATE()";

                SqlCommand comando = new SqlCommand();
                SqlDataAdapter daTablasSync = new SqlDataAdapter();
                comando.CommandText = cadSql;
                comando.Connection = cnn;
                comando.CommandType = CommandType.Text;

                comando.Parameters.Add(new SqlParameter("@IdAgencia", idAgencia));

                DataSet ds = new DataSet();
                daTablasSync.SelectCommand = comando;
                daTablasSync.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    lst = ds.Tables[0].AsEnumerable().ToList().ConvertAll(t =>
                         new CLClientesDC()
                         {
                             IdCliente = t.Field<int>("CLI_IdCliente", DataRowVersion.Current),
                             RazonSocial = t.Field<string>("CLI_RazonSocial", DataRowVersion.Current),
                             Nit = t.Field<string>("CLI_Nit", DataRowVersion.Current)
                         }).OrderBy(ord => ord.RazonSocial).ToList();
                }
            }
            return lst;
        }

        // todo:id para Masivos. (2)Obtiene Contratos de Cliente 
        public List<CLContratosDC> ObtenerContratosActivos_ClienteCredito(long idAgencia, int idCliente)
        {
            List<CLContratosDC> lst = null;

            using (SqlConnection cnn = new SqlConnection(CadCnxController))
            {
                string cadSql = @"SELECT DISTINCT ClienteCredito_CLI.CLI_IdCliente, ClienteCredito_CLI.CLI_RazonSocial, ClienteCredito_CLI.CLI_Nit"
                                + "     , Contrato_CLI.CON_IdContrato, Contrato_CLI.CON_NumeroContrato, Contrato_CLI.CON_NombreContrato, Contrato_CLI.CON_ListaPrecios"

                                + " FROM ClienteCredito_CLI WITH (NOLOCK) "
                                + "	   INNER JOIN Sucursal_CLI WITH (NOLOCK) ON ClienteCredito_CLI.CLI_IdCliente = Sucursal_CLI.SUC_ClienteCredito "
                                + "	   INNER JOIN SucursalesContrato_CLI WITH (NOLOCK) ON Sucursal_CLI.SUC_IdSucursal = SucursalesContrato_CLI.SUC_Sucursal "
                                + "	   INNER JOIN Contrato_CLI WITH (NOLOCK) ON ClienteCredito_CLI.CLI_IdCliente = Contrato_CLI.CON_ClienteCredito "
                                + "                 AND SucursalesContrato_CLI.SUC_Contrato = Contrato_CLI.CON_IdContrato"
                                + " WHERE Sucursal_CLI.SUC_AgenciaEncargada = @idAgencia AND ClienteCredito_CLI.CLI_Estado = 'ACT' AND Contrato_CLI.CON_FechaFinConExtensiones > GETDATE()"
                                + "	    AND ClienteCredito_CLI.CLI_IdCliente =  @IdCliente";

                SqlCommand comando = new SqlCommand();
                SqlDataAdapter daTablasSync = new SqlDataAdapter();
                comando.CommandText = cadSql;
                comando.Connection = cnn;
                comando.CommandType = CommandType.Text;

                comando.Parameters.Add(new SqlParameter("@IdCliente", idCliente));
                comando.Parameters.Add(new SqlParameter("@idAgencia", idAgencia));

                DataSet ds = new DataSet();
                daTablasSync.SelectCommand = comando;
                daTablasSync.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    lst = ds.Tables[0].AsEnumerable().ToList().ConvertAll(t =>
                         new CLContratosDC()
                         {
                             NumeroContrato = t.Field<string>("CON_NumeroContrato", DataRowVersion.Current),
                             IdContrato = t.Field<int>("CON_IdContrato", DataRowVersion.Current),
                             ListaPrecios = t.Field<int>("CON_ListaPrecios", DataRowVersion.Current),
                             NombreContrato = t.Field<string>("CON_NombreContrato", DataRowVersion.Current)
                         });
                }
            }

            if (lst != null)
                lst.ForEach(con => con.ServiciosHabilitados = ObtenerServiciosdeContrato(con.IdContrato));

            return lst;
        }

        // todo:id para Masivos. (3)Obtiene Sucursales
        public List<CLSucursalContratoDC> ObtenerSucursalesContrato_CliCredito(long IdAgencia, int IdContrato)
        {
            List<CLSucursalContratoDC> lst = null;

            using (SqlConnection cnn = new SqlConnection(CadCnxController))
            {
                //string cadSql = @"SELECT DISTINCT ClienteCredito_CLI.CLI_IdCliente, ClienteCredito_CLI.CLI_RazonSocial, ClienteCredito_CLI.CLI_Nit"
                //        + "     , Contrato_CLI.CON_IdContrato, Contrato_CLI.CON_NumeroContrato, Contrato_CLI.CON_NombreContrato, Contrato_CLI.CON_ListaPrecios"
                //        + "     , SucursalesContrato_CLI.SUC_IdSucursalContrato, Sucursal_CLI.SUC_AgenciaEncargada, Sucursal_CLI.SUC_IdSucursal, Sucursal_CLI.SUC_Nombre"
                //        +"      , Sucursal_CLI.SUC_Direccion, Sucursal_CLI.SUC_Telefono, Sucursal_CLI.SUC_Email"

                //        + " FROM ClienteCredito_CLI WITH (NOLOCK) "
                //        + "     INNER JOIN Sucursal_CLI WITH (NOLOCK) ON ClienteCredito_CLI.CLI_IdCliente = Sucursal_CLI.SUC_ClienteCredito"
                //        + "     INNER JOIN SucursalesContrato_CLI WITH (NOLOCK) ON Sucursal_CLI.SUC_IdSucursal = SucursalesContrato_CLI.SUC_Sucursal"
                //        + "     INNER JOIN Contrato_CLI WITH (NOLOCK) ON ClienteCredito_CLI.CLI_IdCliente = Contrato_CLI.CON_ClienteCredito"
                //        + "                                     AND SucursalesContrato_CLI.SUC_Contrato = Contrato_CLI.CON_IdContrato"
                //        + "     INNER JOIN MunicipioCentroLogistico_PUA ON MCL_IdLocalidad = SUC_Municipio AND MCL_IdCentroLogistico = @IdAgencia"
                //        + " WHERE ClienteCredito_CLI.CLI_Estado = 'ACT' AND Contrato_CLI.CON_FechaFinConExtensiones > GETDATE()"
                //        + "     AND Contrato_CLI.CON_IdContrato = @IdContrato";


                string cadSql = @"SELECT DISTINCT ClienteCredito_CLI.CLI_IdCliente, ClienteCredito_CLI.CLI_RazonSocial, ClienteCredito_CLI.CLI_Nit"
                            + "     , Contrato_CLI.CON_IdContrato, Contrato_CLI.CON_NumeroContrato, Contrato_CLI.CON_NombreContrato, Contrato_CLI.CON_ListaPrecios"
                            + "     , SucursalesContrato_CLI.SUC_IdSucursalContrato, Sucursal_CLI.SUC_AgenciaEncargada, Sucursal_CLI.SUC_IdSucursal, Sucursal_CLI.SUC_Nombre"
                            + "     , Sucursal_CLI.SUC_Direccion, Sucursal_CLI.SUC_Telefono,Sucursal_CLI.SUC_Email,Sucursal_CLI.SUC_CodigoPostal"
                            + "     FROM ClienteCredito_CLI WITH (NOLOCK) "
                            + "     INNER JOIN Sucursal_CLI WITH (NOLOCK) ON ClienteCredito_CLI.CLI_IdCliente = Sucursal_CLI.SUC_ClienteCredito "
                            + "     INNER JOIN SucursalesContrato_CLI WITH (NOLOCK) ON Sucursal_CLI.SUC_IdSucursal = SucursalesContrato_CLI.SUC_Sucursal "
                            + "     INNER JOIN Contrato_CLI WITH (NOLOCK) ON ClienteCredito_CLI.CLI_IdCliente = Contrato_CLI.CON_ClienteCredito "
                            + "     AND SucursalesContrato_CLI.SUC_Contrato = Contrato_CLI.CON_IdContrato    "
                            + "     WHERE (Sucursal_CLI.SUC_AgenciaEncargada = @IdAgencia) AND (ClienteCredito_CLI.CLI_Estado = 'ACT' AND Contrato_CLI.CON_FechaFinConExtensiones > GETDATE())"
                            + "     AND Contrato_CLI.CON_IdContrato = @IdContrato";



                SqlCommand comando = new SqlCommand();
                SqlDataAdapter daTablasSync = new SqlDataAdapter();
                comando.CommandText = cadSql;
                comando.Connection = cnn;
                comando.CommandType = CommandType.Text;

                comando.Parameters.Add(new SqlParameter("@IdAgencia", IdAgencia));
                comando.Parameters.Add(new SqlParameter("@IdContrato", IdContrato));

                DataSet ds = new DataSet();
                daTablasSync.SelectCommand = comando;
                daTablasSync.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    lst = ds.Tables[0].AsEnumerable().ToList().ConvertAll(t =>
                         new CLSucursalContratoDC()
                         {
                             IdSucursal = t.Field<int>("SUC_IdSucursal", DataRowVersion.Current),
                             Nombre = t.Field<string>("SUC_Nombre", DataRowVersion.Current),
                             Direccion = t.Field<string>("SUC_Direccion", DataRowVersion.Current),
                             Telefono = t.Field<string>("SUC_Telefono", DataRowVersion.Current),
                             CodigoPostal = t.Field<string>("SUC_CodigoPostal", DataRowVersion.Current)
                         });
                }
            }
            return lst;

        }




        /// <summary>
        /// Consulta para Obtener los Servicios habilitados de un Contrato
        /// </summary>
        /// <returns>Colección clientes y contratos</returns>
        public List<TAServicioDC> ObtenerServiciosdeContrato(long IdContrato)
        {
            //todo:id Metodo para obtener Servicios habilitados de un Contrato
            List<TAServicioDC> lst = null;

            using (SqlConnection cnn = new SqlConnection(CadCnxController))
            {
                string cadSql = @"SELECT DISTINCT ser.SER_IdServicio, ser.SER_Nombre, ser.SER_IdUnidadNegocio, ser.SER_IdConceptoCaja "
                        + " FROM Contrato_CLI"
                        + "  INNER JOIN ListaPrecios_TAR ON ListaPrecios_TAR.LIP_IdListaPrecios = Contrato_CLI.CON_ListaPrecios"
                        + "  INNER JOIN ListaPrecioServicio_TAR lpst ON lpst.LPS_IdListaPrecios = ListaPrecios_TAR.LIP_IdListaPrecios"
                        + "  INNER JOIN Servicio_TAR AS ser ON lpst.LPS_IdServicio = ser.SER_IdServicio"
                        + " WHERE CON_IdContrato = " + IdContrato.ToString().Trim() + " AND lpst.LPS_Estado='ACT'";

                SqlCommand cmdTablasSync = new SqlCommand();
                SqlDataAdapter daTablasSync = new SqlDataAdapter();
                cmdTablasSync.CommandText = cadSql;
                cmdTablasSync.Connection = cnn;
                cmdTablasSync.CommandType = CommandType.Text;

                DataSet ds = new DataSet();
                daTablasSync.SelectCommand = cmdTablasSync;
                daTablasSync.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    lst = ds.Tables[0].AsEnumerable().ToList().ConvertAll(t =>
                         new TAServicioDC()
                         {
                             IdServicio = t.Field<int>("SER_IdServicio", DataRowVersion.Current),
                             Nombre = t.Field<string>("SER_Nombre", DataRowVersion.Current),
                             IdUnidadNegocio = t.Field<string>("SER_IdUnidadNegocio", DataRowVersion.Current),
                             IdConceptoCaja = t.Field<int>("SER_IdConceptoCaja", DataRowVersion.Current)
                         });
                }
            }
            return lst;
        }


        /// <summary>
        /// Consulta los cliente creditos con sus respectivos contratos, y Si puede realizar giros convenio o dispersion de fondos
        /// </summary>
        /// <returns>Colección clientes y contratos</returns>
        public List<CLClientesDC> ObtenerClientesContratosGiros()
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var lstClientes = contexto.paObtenerCliCreditoContratCondicionGiro_CLI(true, null).ToList()
                  .GroupBy(global => global.CLI_IdCliente).ToList().Select(
                  cliente => new CLClientesDC()
                  {
                      IdCliente = cliente.First().CON_IdContrato,
                      RazonSocial = cliente.First().CLI_RazonSocial,
                      Nit = cliente.First().CLI_Nit,
                      Telefono = cliente.First().CLI_Telefono,
                      Direccion = cliente.First().CLI_Direccion,
                      IdPais = cliente.First().IdPais,
                      Localidad = cliente.First().LOC_IdLocalidad,
                      NombrePais = cliente.First().NombrePais,
                      IdCiudad = cliente.First().IdCiudad,
                      NombreCiudad = cliente.First().LOC_Nombre,
                      CodigoPostal = cliente.First().LOC_CodigoPostal,
                      Contratos = cliente.ToList().ConvertAll(contrato => new CLContratosDC()
                      {
                          IdContrato = contrato.CON_IdContrato,
                          NombreContrato = contrato.CON_NombreContrato,
                          ClienteCondicionGiro = new CLClienteCondicionGiroDC()
                          {
                              IdContrato = contrato.CON_IdContrato,
                              ConvenioPagaPorte = contrato.CCG_ConvenioPagaPorte,
                              PermiteDispersion = contrato.CCG_PermiteDispersion,
                              PermiteGiroConvenio = contrato.CCG_PermiteGiroConvenio,
                              EstadoRegistro = contrato.CCG_IdContrato == 0 ? EnumEstadoRegistro.ADICIONADO : EnumEstadoRegistro.MODIFICADO
                          }
                      }),
                  }).ToList();

                if (lstClientes != null)
                    return lstClientes.ToList();
                else
                    return null;
            }
        }

        /// <summary>
        /// Consulta los cliente creditos con sus respectivos contratos, y Si puede realizar giros convenio o dispersion de fondos
        /// y Cupo de Dispersion Aprobado
        /// </summary>
        /// <returns>Colección clientes y contratos</returns>
        public List<CLClientesDC> ObtenerTodosClientesContratosGiros()
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var lstClientes = contexto.paObtenerCliCreditoDispersionFondosGiro_CLI().ToList()
                  .GroupBy(global => global.CLI_IdCliente).ToList().Select(
                  cliente => new CLClientesDC()
                  {
                      IdCliente = cliente.First().CLI_IdCliente,
                      RazonSocial = cliente.First().CLI_RazonSocial,
                      Nit = cliente.First().CLI_Nit,
                      Telefono = cliente.First().CLI_Telefono,
                      Direccion = cliente.First().CLI_Direccion,
                      Contratos = cliente.ToList().ConvertAll(contrato => new CLContratosDC()
                      {
                          IdContrato = contrato.CON_IdContrato,
                          IdCliente = cliente.First().CLI_IdCliente,
                          NombreContrato = contrato.CON_NombreContrato,
                          ClienteCondicionGiro = new CLClienteCondicionGiroDC()
                          {
                              IdContrato = contrato.CON_IdContrato,
                              ConvenioPagaPorte = contrato.CCG_ConvenioPagaPorte != null ? contrato.CCG_ConvenioPagaPorte.Value : false,
                              PermiteDispersion = contrato.CCG_ConvenioPagaPorte != null ? contrato.CCG_ConvenioPagaPorte.Value : false,
                              PermiteGiroConvenio = contrato.CCG_PermiteGiroConvenio != null ? contrato.CCG_PermiteGiroConvenio.Value : false,
                              EstadoRegistro = contrato.CCG_IdContrato == null ? EnumEstadoRegistro.ADICIONADO : EnumEstadoRegistro.MODIFICADO
                          },
                          CupoDispersionCliente = new CLCupoDispersionClienteDC()
                          {
                              CupoDispersionAprobado = contrato.CDC_CupoDispersionAprobado != null ? contrato.CDC_CupoDispersionAprobado.Value : 0
                          }
                      }),
                  }).ToList();

                if (lstClientes != null)
                    return lstClientes.ToList();
                else
                    return null;
            }
        }

        #endregion Cliente Agencias Contrato

        #region TraducirNotacionDiaInter

        /// <summary>
        /// Traduce a texto un id de notación consultando en la base de datos los valores de la misma
        /// </summary>
        /// <param name="idNotacionDia"></param>
        /// <returns></returns>
        public string TraducirNotacionDiaInter(short idNotacionDia)
        {
            string notacionTraducida = "";
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                NotacionesFacturacion_CLI notacion = contexto.NotacionesFacturacion_CLI.Where(not => not.NOF_IdNotacion == idNotacionDia).FirstOrDefault();
                if (notacion != null)
                {
                    if (notacion.NOF_TipoNotacion == 1)
                    {
                        notacionTraducida = "Día " + notacion.NOF_DiaSemana + " de la semana " + notacion.NOF_DiaSemana + " de cada mes";
                    }
                    else if (notacion.NOF_TipoNotacion == 2)
                    {
                        notacionTraducida = "Día " + notacion.NOF_DiaOrdinal + " de cada mes";
                    }
                }
            }

            return notacionTraducida;
        }

        #endregion TraducirNotacionDiaInter

        #region PBX

        public string ObtenerNombreyDireccionCliente(string Telefono)
        {
            CLClientesDC cliente = new CLClientesDC();
            PATipoIdentificacion TipoIdentificacion = new PATipoIdentificacion();
            using (SqlConnection cnn = new SqlConnection(CadCnxController))
            {
                cnn.Open();
                using (SqlCommand cmd = new SqlCommand("paObtenerDatosSolicitudRecogida_CLI", cnn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Telefono", Telefono);
                    string validacion = string.Empty;

                    SqlDataReader reader = cmd.ExecuteReader();
                    {
                        if (reader.Read())
                        {
                            TipoIdentificacion.IdTipoIdentificacion = Convert.ToString(reader["TipoIdentificacion"]);
                            cliente.Nit = Convert.ToString(reader["Identificacion"]);
                            cliente.RazonSocial = Convert.ToString(reader["PersonaQueSolicita"]);
                            cliente.Direccion = Convert.ToString(reader["Direccion"]);
                            cliente.Telefono = Convert.ToString(reader["Telefono"]);
                            cliente.IdCiudad = Convert.ToString(reader["IdCiudad"]);
                            //                            cliente.NombreCiudad = Convert.ToString(reader["Ciudad"]);


                            string longitud = "0", latitud = "0";

                            if (reader["SOR_Longitud"] != System.DBNull.Value)
                            {
                                longitud = Convert.ToString(reader["SOR_Longitud"]);
                            }

                            if (reader["SOR_Latitud"] != System.DBNull.Value)
                            {
                                latitud = Convert.ToString(reader["SOR_Latitud"]);
                            }

                            validacion = string.Join(",", new string[] { TipoIdentificacion.IdTipoIdentificacion, cliente.Nit, cliente.RazonSocial, cliente.Direccion, cliente.Telefono, cliente.IdCiudad, longitud, latitud });
                        }
                        cnn.Close();
                        cnn.Dispose();
                    }
                    return validacion;
                }

            }
        }



        #endregion

        /// <summary>
        /// Consulta el listado de sucursales que pertenecen a un agrupamiento de una factura
        /// </summary>
        /// <param name="idAgrupamiento"></param>
        /// <returns></returns>
        public IEnumerable<CLSucursalDC> ConsultarSucursalesAgrupamientoFactura(int idAgrupamiento)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.AgrupacionFactura_CLI.Include("Sucursal_CLI").Where(ag => ag.AGF_IdParFacturacion == idAgrupamiento).GroupBy(gr => new { gr.AGF_IdSucursal, gr.Sucursal_CLI.SUC_Nombre }).
                 ToList().ConvertAll<CLSucursalDC>(suc =>
                 {
                     return new CLSucursalDC()
                     {
                         IdSucursal = suc.Key.AGF_IdSucursal,
                         Nombre = suc.Key.SUC_Nombre
                     };
                 });
            }
        }

        /// <summary>
        /// Consulta el listado de servicios que pertenecen a un agrupamiento de una factura
        /// </summary>
        /// <param name="idAgrupamiento"></param>
        /// <returns></returns>
        public IEnumerable<TAServicioDC> ConsultarServiciosAgrupamientoFactura(int idAgrupamiento)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.AgrupacionFactura_CLI.Include("Servicio_TAR").Where(ag => ag.AGF_IdParFacturacion == idAgrupamiento).GroupBy(gr => new { gr.AGF_IdServicio, gr.Servicio_TAR.SER_Nombre }).
                 ToList().ConvertAll<TAServicioDC>(ser =>
                 {
                     return new TAServicioDC()
                     {
                         IdServicio = ser.Key.AGF_IdServicio,
                         Nombre = ser.Key.SER_Nombre
                     };
                 });
            }
        }

        /// <summary>
        /// Consulta los requisitos asociados a un agrupamiento de una factura configurada
        /// </summary>
        /// <param name="idAgrupamiento"></param>
        /// <returns></returns>
        public IEnumerable<string> ConsultarRequisitosAgrupamientoFactura(int idAgrupamiento)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.RequisitosRadFactura_CLI.Where(req => req.RRF_IdParFacturacion == idAgrupamiento).ToList().ConvertAll<string>(r =>
                {
                    return r.RRF_Descripcion;
                });
            }
        }

        /// <summary>
        /// Valida que un cliente crédito pueda realizar venta de servicios y retorna  la lista de servicios habilitados.
        /// Para aquellos servicios que aparecen en más de un contrato, coge el primer contrato que se encuentre
        /// </summary>
        /// <param name="idCentroServicios">Identificador del centro de servicios</param>
        /// <returns>Lista de servicios ofrecidos</returns>
        public IEnumerable<CLServiciosSucursal> ObtenerServiciosPorUnidadesDeNegocio(int idSucursal, string idUnidadMensajeria, string idUnidadCarga)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                List<paObtenerServActPorSuc_CLI_Result> sSucursal = contexto.paObtenerServActPorSuc_CLI((int)idSucursal, idUnidadMensajeria, idUnidadCarga).ToList();
                List<CLServiciosSucursal> serviciosSucursal = sSucursal
                                                                    .GroupBy(servSuc => servSuc.SER_IdServicio)
                                                                    .ToList()
                                                                    .ConvertAll<CLServiciosSucursal>(servSuc => new CLServiciosSucursal
                                                                    {
                                                                        Contrato = new CLContratosDC
                                                                        {
                                                                            IdContrato = servSuc.First().CON_IdContrato,
                                                                            NombreContrato = servSuc.First().CON_NombreContrato,
                                                                            NumeroContrato = servSuc.First().CON_NumeroContrato,
                                                                            ListaPrecios = servSuc.First().LIP_IdListaPrecios
                                                                        },
                                                                        Servicio = new Servicios.ContratoDatos.Tarifas.TAServicioDC
                                                                        {
                                                                            IdServicio = servSuc.First().SER_IdServicio,
                                                                            Nombre = servSuc.First().SER_Nombre,
                                                                            IdConceptoCaja = servSuc.First().SER_IdConceptoCaja,
                                                                            PesoMaximo = servSuc.First().SME_PesoMaximo,
                                                                            PesoMinimo = servSuc.First().SME_PesoMínimo,
                                                                            IdUnidadNegocio = servSuc.First().SER_IdUnidadNegocio
                                                                        },
                                                                        IdSucursal = servSuc.First().SUC_IdSucursal
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

        #region Referencia uso guia

        /// <summary>
        /// Inserta una nueva referencia uso guia interna
        /// </summary>
        /// <param name="referencia"></param>
        public void AgregarReferenciaUsoGuia(CLReferenciaUsoGuiaDC referencia)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ReferenciaUsoGuia_CLI refUsoGuia = new ReferenciaUsoGuia_CLI()
                {
                    RUG_CiudadDestino = referencia.CiudadDestinoLoc.IdLocalidad,
                    RUG_CiudadOrigen = referencia.CiudadOrigenLoc.IdLocalidad,
                    RUG_CodigoPostalDestino = referencia.CodigoPostalDestino,
                    RUG_IdSucursal = referencia.IdSucursal,
                    RUG_CodigoPostalOrigen = referencia.CodigoPostalOrigen,
                    RUG_CreadoPor = ControllerContext.Current.Usuario,
                    RUG_DireccionDestino = referencia.DireccionDestino,
                    RUG_DireccionOrigen = referencia.DireccionOrigen,
                    RUG_EsDestionoAbierto = referencia.EsDestionoAbierto,
                    RUG_EsOrigenAbierto = referencia.EsOrigenAbierto,
                    RUG_FechaGrabacion = DateTime.Now,
                    RUG_IdentificacionDestino = referencia.IdentificacionDestino,
                    RUG_IdentificacionOrigen = referencia.IdentificacionOrigen,
                    RUG_NombreDestino = referencia.NombreDestino,
                    RUG_NombreOrigen = referencia.NombreOrigen,
                    RUG_PaisDestino = referencia.PaisDestinoLoc.IdLocalidad,
                    RUG_PaisOrigen = referencia.PaisOrigenLoc.IdLocalidad,
                    RUG_TelefonoDestino = referencia.TelefonoDestino,
                    RUG_TelefonoOrigen = referencia.TelefonoOrigen,
                    RUG_TipoIdentificacionDestino = referencia.TipoIdentificacionDestino,
                    RUG_TipoIdentificacionOrigen = referencia.TipoIdentificacionOrigen
                };

                contexto.ReferenciaUsoGuia_CLI.Add(refUsoGuia);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Modifica una referencia uso guia interna
        /// </summary>
        /// <param name="referencia"></param>
        public void EditarReferenciaUsoGuia(CLReferenciaUsoGuiaDC referencia)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ReferenciaUsoGuia_CLI refUsoGuia = contexto.ReferenciaUsoGuia_CLI.Where(r => r.RUG_IdSucursal == referencia.IdSucursal).SingleOrDefault();

                if (refUsoGuia == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.CLIENTES, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }

                refUsoGuia.RUG_CiudadDestino = referencia.CiudadDestinoLoc != null ? referencia.CiudadDestinoLoc.IdLocalidad : null;
                refUsoGuia.RUG_CiudadOrigen = referencia.CiudadOrigenLoc.IdLocalidad;
                refUsoGuia.RUG_CodigoPostalDestino = referencia.CodigoPostalDestino;
                refUsoGuia.RUG_IdSucursal = referencia.IdSucursal;
                refUsoGuia.RUG_CodigoPostalOrigen = referencia.CodigoPostalOrigen;

                refUsoGuia.RUG_DireccionDestino = referencia.DireccionDestino;
                refUsoGuia.RUG_DireccionOrigen = referencia.DireccionOrigen;
                refUsoGuia.RUG_EsDestionoAbierto = referencia.EsDestionoAbierto;
                refUsoGuia.RUG_EsOrigenAbierto = referencia.EsOrigenAbierto;

                refUsoGuia.RUG_IdentificacionDestino = referencia.IdentificacionDestino;
                refUsoGuia.RUG_IdentificacionOrigen = referencia.IdentificacionOrigen;
                refUsoGuia.RUG_NombreDestino = referencia.NombreDestino;
                refUsoGuia.RUG_NombreOrigen = referencia.NombreOrigen;

                refUsoGuia.RUG_PaisDestino = referencia.PaisDestinoLoc.IdLocalidad;
                refUsoGuia.RUG_PaisOrigen = referencia.PaisOrigenLoc.IdLocalidad;

                refUsoGuia.RUG_TelefonoDestino = referencia.TelefonoDestino;
                refUsoGuia.RUG_TelefonoOrigen = referencia.TelefonoOrigen;
                refUsoGuia.RUG_TipoIdentificacionDestino = referencia.TipoIdentificacionDestino;
                refUsoGuia.RUG_TipoIdentificacionOrigen = referencia.TipoIdentificacionOrigen;

                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Borra una referencia uso guia interna
        /// </summary>
        /// <param name="referencia"></param>
        public void BorrarReferenciaUsoGuia(CLReferenciaUsoGuiaDC referencia)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ReferenciaUsoGuia_CLI refUsoGuia = contexto.ReferenciaUsoGuia_CLI.Where(r => r.RUG_IdSucursal == referencia.IdSucursal).SingleOrDefault();

                if (refUsoGuia == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.CLIENTES, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }

                contexto.ReferenciaUsoGuia_CLI.Remove(refUsoGuia);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Obtiene  las referencias de uso de una guia interna
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consult</param>
        /// <param name="idTipoMensajero">Id de la ruta por la cual se filtraran las estaciones</param>
        /// <returns>Lista  con los conductores y auxiliares en un objeto tipo mensajero</returns>
        public IList<CLReferenciaUsoGuiaDC> ObtenerReferenciaUsoGuia(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ConsultarContainsReferenciaUsoGuia_VCLI(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente).ToList()
                  .ConvertAll<CLReferenciaUsoGuiaDC>(r =>
                    new CLReferenciaUsoGuiaDC()
                    {
                        CiudadDestino = r.RUG_CiudadDestino,
                        CiudadOrigen = r.RUG_CiudadOrigen,
                        CodigoPostalDestino = r.RUG_CodigoPostalDestino,
                        IdSucursal = r.RUG_IdSucursal,
                        CodigoPostalOrigen = r.RUG_CodigoPostalOrigen,
                        DireccionDestino = r.RUG_DireccionDestino,
                        DireccionOrigen = r.RUG_DireccionOrigen,
                        EsDestionoAbierto = r.RUG_EsDestionoAbierto,
                        EsOrigenAbierto = r.RUG_EsOrigenAbierto,
                        IdentificacionDestino = r.RUG_IdentificacionDestino,
                        IdentificacionOrigen = r.RUG_IdentificacionOrigen,
                        NombreDestino = r.RUG_NombreDestino,
                        NombreOrigen = r.RUG_NombreOrigen,
                        PaisDestino = r.RUG_PaisDestino,
                        PaisOrigen = r.RUG_PaisOrigen,
                        TelefonoDestino = r.RUG_TelefonoDestino,
                        TelefonoOrigen = r.RUG_TelefonoOrigen,
                        TipoIdentificacionDestino = r.RUG_TipoIdentificacionDestino,
                        TipoIdentificacionOrigen = r.RUG_TipoIdentificacionOrigen,
                        CiudadDestinoLoc = new PALocalidadDC()
                        {
                            IdLocalidad = r.IdCiudadDestino,
                            Nombre = r.NombreCiudadDestino
                        },
                        CiudadOrigenLoc = new PALocalidadDC()
                        {
                            IdLocalidad = r.IdCiudadOrigen,
                            Nombre = r.NombreCiudadOrigen
                        },
                        PaisDestinoLoc = new PALocalidadDC()
                        {
                            IdLocalidad = r.IdPaisDestino,
                            Nombre = r.NombrePaisDestino
                        },
                        PaisOrigenLoc = new PALocalidadDC()
                        {
                            IdLocalidad = r.IdPaisOrigen,
                            Nombre = r.NombrePaisOrigen
                        }
                    }).ToList();
            }
        }

        /// <summary>
        /// Obtiene la referencia de uso guia por sucursal
        /// </summary>
        /// <param name="idSucursal"></param>
        /// <returns></returns>
        public CLReferenciaUsoGuiaDC ObtenerReferenciaUsoGuiaPorSucursal(int idSucursal)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ReferenciaUsoGuia_VCLI refUsoGuia = contexto.ReferenciaUsoGuia_VCLI.Where(r => r.RUG_IdSucursal == idSucursal).SingleOrDefault();

                if (refUsoGuia == null)
                {
                    return null;
                }

                return new CLReferenciaUsoGuiaDC()
                {
                    CiudadDestino = refUsoGuia.RUG_CiudadDestino,
                    CiudadOrigen = refUsoGuia.RUG_CiudadOrigen,
                    CodigoPostalDestino = refUsoGuia.RUG_CodigoPostalDestino,
                    IdSucursal = refUsoGuia.RUG_IdSucursal,
                    CodigoPostalOrigen = refUsoGuia.RUG_CodigoPostalOrigen,
                    DireccionDestino = refUsoGuia.RUG_DireccionDestino,
                    DireccionOrigen = refUsoGuia.RUG_DireccionOrigen,
                    EsDestionoAbierto = refUsoGuia.RUG_EsDestionoAbierto,
                    EsOrigenAbierto = refUsoGuia.RUG_EsOrigenAbierto,
                    IdentificacionDestino = refUsoGuia.RUG_IdentificacionDestino,
                    IdentificacionOrigen = refUsoGuia.RUG_IdentificacionOrigen,
                    NombreDestino = refUsoGuia.RUG_NombreDestino,
                    NombreOrigen = refUsoGuia.RUG_NombreOrigen,
                    PaisDestino = refUsoGuia.RUG_PaisDestino,
                    PaisOrigen = refUsoGuia.RUG_PaisOrigen,
                    TelefonoDestino = refUsoGuia.RUG_TelefonoDestino,
                    TelefonoOrigen = refUsoGuia.RUG_TelefonoOrigen,
                    TipoIdentificacionDestino = refUsoGuia.RUG_TipoIdentificacionDestino,
                    TipoIdentificacionOrigen = refUsoGuia.RUG_TipoIdentificacionOrigen,
                    CiudadDestinoLoc = new PALocalidadDC()
                    {
                        IdLocalidad = refUsoGuia.IdCiudadDestino,
                        Nombre = refUsoGuia.NombreCiudadDestino
                    },
                    CiudadOrigenLoc = new PALocalidadDC()
                    {
                        IdLocalidad = refUsoGuia.IdCiudadOrigen,
                        Nombre = refUsoGuia.NombreCiudadOrigen
                    },
                    PaisDestinoLoc = new PALocalidadDC()
                    {
                        IdLocalidad = refUsoGuia.IdPaisDestino,
                        Nombre = refUsoGuia.NombrePaisDestino
                    },
                    PaisOrigenLoc = new PALocalidadDC()
                    {
                        IdLocalidad = refUsoGuia.IdPaisOrigen,
                        Nombre = refUsoGuia.NombrePaisOrigen
                    }
                };
            }
        }

        #endregion Referencia uso guia

        #region Cliente convenio

        /// <summary>
        /// Método para obtener las localidades origen de un cliente convenio
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public IEnumerable<CLConvenioLocalidadDC> ObtenerLocalidadesConvenio(int idCliente)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                List<ClienteConvenioNal_CLI> listaConvenio = contexto.ClienteConvenioNal_CLI.Include("Localidad_PAR").Where(con => con.CCN_IdCliente == idCliente)
                    .ToList();
                if (listaConvenio != null && listaConvenio.Any())
                {
                    return listaConvenio.ConvertAll<CLConvenioLocalidadDC>(con => new CLConvenioLocalidadDC
                    {
                        IdCliente = idCliente,
                        EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS,
                        Localidad = new PALocalidadDC { IdLocalidad = con.CCN_IdLocalidad, Nombre = con.Localidad_PAR.LOC_Nombre }
                    });
                }
                else
                    return new List<CLConvenioLocalidadDC>();
            }
        }

        /// <summary>
        /// Método para adicionar una localidad de origen a un cliente convenio
        /// </summary>
        /// <param name="localidadConvenio"></param>
        public void AdicionarLocalidadConvenio(CLConvenioLocalidadDC localidadConvenio)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ClienteConvenioNal_CLI clienteConvenio = new ClienteConvenioNal_CLI
                {
                    CCN_IdCliente = localidadConvenio.IdCliente,
                    CCN_IdLocalidad = localidadConvenio.Localidad.IdLocalidad,
                    CCN_CreadoPor = ControllerContext.Current.Usuario,
                    CCN_FechaGrabacion = DateTime.Now
                };
                contexto.ClienteConvenioNal_CLI.Add(clienteConvenio);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Método para eliminar una localidad de origen de un cliente convenio
        /// </summary>
        /// <param name="localidadConvenio"></param>
        public void EliminarLocalidadConvenio(CLConvenioLocalidadDC localidadConvenio)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ClienteConvenioNal_CLI clienteConvenio = contexto.ClienteConvenioNal_CLI
                    .Where(cl => cl.CCN_IdLocalidad == localidadConvenio.Localidad.IdLocalidad
                        && cl.CCN_IdCliente == localidadConvenio.IdCliente).FirstOrDefault();
                if (clienteConvenio != null)
                {
                    contexto.ClienteConvenioNal_CLI.Remove(clienteConvenio);
                    contexto.SaveChanges();
                }
            }
        }

        #endregion Cliente convenio

        /// <summary>
        /// Obtiene un cliente a partir del id
        /// </summary>
        /// <returns>Cliente</returns>
        //public CLClientesDC ObtenerCliente(long idCliente)
        //{            
        //    using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
        //    {
        //        CLClientesDC cliente = new CLClientesDC();
        //        SqlCommand cmd = new SqlCommand("paObtenerDatosClienteCredito_CLI", sqlConn);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@idCliente", idCliente);
        //        sqlConn.Open();
        //        SqlDataReader reader = cmd.ExecuteReader();
        //        while (reader.Read())
        //        {
        //            cliente.IdCliente = Convert.ToInt32(reader["CLI_IdCliente"]);
        //            cliente.DigitoVerificacion = reader["CLI_DigitoVerificacion"].ToString().Trim();
        //            cliente.Direccion = reader["CLI_Direccion"].ToString();
        //            cliente.Fax = reader["CLI_Fax"].ToString();
        //            cliente.FechaConstitucion = Convert.ToDateTime(reader["CLI_FechaConstitucion"]);
        //            cliente.FechaVinculacion = Convert.ToDateTime(reader["CLI_FechaVinculacion"]);
        //            cliente.IdRepresentanteLegal = Convert.ToInt64(reader["CLI_IdRepresentanteLegal"]);
        //            cliente.Localidad = reader["CLI_Municipio"].ToString();
        //            cliente.Nit = reader["CLI_Nit"].ToString();
        //            cliente.NombreGerente = reader["CLI_NombreGerenteGeneral"].ToString();
        //            cliente.RazonSocial = reader["CLI_RazonSocial"].ToString();
        //            cliente.Telefono = reader["CLI_Telefono"].ToString();
        //        }
        //        sqlConn.Close();
        //        return cliente;

        //        //ClienteCredito_CLI cliente = contexto.ClienteCredito_CLI.FirstOrDefault(cli => cli.CLI_IdCliente == idCliente);
        //        /*  if (cliente != null)
        //          {
        //              return new CLClientesDC
        //              {
        //                  IdCliente = cliente.CLI_IdCliente,
        //                  DigitoVerificacion = cliente.CLI_DigitoVerificacion.Trim(),
        //                  Direccion = cliente.CLI_Direccion,
        //                  Fax = cliente.CLI_Fax,
        //                  FechaConstitucion = cliente.CLI_FechaConstitucion,
        //                  FechaVinculacion = cliente.CLI_FechaVinculacion,
        //                  IdRepresentanteLegal = cliente.CLI_IdRepresentanteLegal,
        //                  Localidad = cliente.CLI_Municipio,
        //                  Nit = cliente.CLI_Nit,
        //                  NombreGerente = cliente.CLI_NombreGerenteGeneral,
        //                  RazonSocial = cliente.CLI_RazonSocial,
        //                  Telefono = cliente.CLI_Telefono,
        //              };
        //          }
        //          else
        //              return new CLClientesDC();*/
        //    }
        //}

        //public bool ValidarCupoContrato(int idContrato, decimal valorTransaccion)





        /// <summary>
        /// Obtiene un cliente a partir del id
        /// </summary>
        /// <returns>Cliente</returns>
        public CLClientesDC ObtenerCliente(long idCliente)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ClienteCredito_CLI cliente = contexto.ClienteCredito_CLI.FirstOrDefault(cli => cli.CLI_IdCliente == idCliente);
                if (cliente != null)
                {
                    return new CLClientesDC
                    {
                        IdCliente = cliente.CLI_IdCliente,
                        DigitoVerificacion = cliente.CLI_DigitoVerificacion.Trim(),
                        Direccion = cliente.CLI_Direccion,
                        Fax = cliente.CLI_Fax,
                        FechaConstitucion = cliente.CLI_FechaConstitucion,
                        FechaVinculacion = cliente.CLI_FechaVinculacion,
                        IdRepresentanteLegal = cliente.CLI_IdRepresentanteLegal,
                        Localidad = cliente.CLI_Municipio,
                        Nit = cliente.CLI_Nit,
                        NombreGerente = cliente.CLI_NombreGerenteGeneral,
                        RazonSocial = cliente.CLI_RazonSocial,
                        Telefono = cliente.CLI_Telefono,
                    };
                }
                else
                    return new CLClientesDC();
            }
        }

        public bool ValidarCupoContrato(int idContrato, decimal valorTransaccion)
        {
            using (EntidadesClientes contexto = new EntidadesClientes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                bool respuesta = false;
                ObjectParameter resultado = new ObjectParameter("resultado", typeof(int));
                contexto.PaValidarCupoContrato_CLI(idContrato, (long)valorTransaccion, resultado);

                if ((int)resultado.Value == 0)
                    respuesta = false;
                if ((int)resultado.Value == 2)
                {
                    var x = new ControllerException
                      (
                      COConstantesModulos.CLIENTES,
                      CLEnumTipoErrorCliente.EX_FALLO_VALOR_CONTRATO.ToString(),
                      CLMensajesClientes.CargarMensaje(CLEnumTipoErrorCliente.EX_FALLO_VALOR_CONTRATO)
                      );
                    throw new FaultException<ControllerException>(x);
                }

                if ((int)resultado.Value == 1)
                    respuesta = true;

                return respuesta;
            }
        }


        //{
        //    using (SqlConnection sqlConn = new SqlConnection(conexionStringTransaccional))
        //    {
        //        bool respuesta = false;
        //        SqlCommand cmd = new SqlCommand("PaValidarCupoContrato_CLI", sqlConn);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@ACC_IdContrato", idContrato);
        //        cmd.Parameters.AddWithValue("@valorTransaccion", (long)valorTransaccion);
        //        SqlParameter resultado = new SqlParameter("@resultado", 0);
        //        resultado.Direction = ParameterDirection.Output;
        //        cmd.Parameters.Add(resultado);
        //        sqlConn.Open();
        //        cmd.ExecuteNonQuery();
        //        sqlConn.Close();
        //        if (Convert.ToInt32(resultado.Value) == 0)
        //            respuesta = false;
        //        if (Convert.ToInt32(resultado.Value) == 2)
        //        {
        //            var x = new ControllerException
        //              (
        //              COConstantesModulos.CLIENTES,
        //              CLEnumTipoErrorCliente.EX_FALLO_VALOR_CONTRATO.ToString(),
        //              CLMensajesClientes.CargarMensaje(CLEnumTipoErrorCliente.EX_FALLO_VALOR_CONTRATO)
        //              );
        //            throw new FaultException<ControllerException>(x);
        //        }
        //        if (Convert.ToInt32(resultado.Value) == 1)
        //            respuesta = true;

        //        return respuesta;
        //    }
        //}
    }//fin namespace
}//fin de la clase