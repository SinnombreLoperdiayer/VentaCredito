using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Seguridad.Datos.Modelo;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using Framework.Servidor.Servicios.ContratoDatos.Reportes;
using Framework.Servidor.Servicios.ContratoDatos.Seguridad;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.Transactions;

namespace Framework.Servidor.Seguridad.Datos
{
    /// <summary>
    /// Clase que representa el repositorio para seguridad
    /// </summary>
    public class SERepositorio
    {
        #region Campos

        private static readonly SERepositorio instancia = new SERepositorio();
        private const string NombreModelo = "ModeloSeguridad";
        private const string FORMATO_CLAVE_HASHED = "hashed";
        private const string FORMATO_CLAVE_PLANA = "plana";

        #endregion Campos

        #region Propiedades

        private string CadCnxController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;

        /// <summary>
        /// Retorna la instancia de la clase SERepositorio
        /// </summary>
        public static SERepositorio Instancia
        {
            get { return SERepositorio.instancia; }
        }


        public bool ValidadUsuarioExiste(string usuario)
        {
            bool respuesta = false;
            string query = @"SELECT USU_TipoUsuario FROM Usuario_SEG WHERE USU_IdUsuario = @usuario";
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand(query, sqlConn);
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.AddWithValue("@usuario", usuario);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    respuesta = true;
                }
            }
            return respuesta;
        }

        #endregion Propiedades

        #region Constructor

        /// <summary>
        /// Crea una nueva instancia del repositorio de seguridad
        /// </summary>
        private SERepositorio()
        {
        }

        #endregion Constructor

         #region Publicos

        /// <summary>
        /// Retorna los menus y los modulos dependiendo del usuario
        /// </summary>
        /// <param name="credencial">Credencial con la informacion del usuario</param>
        /// <returns>Credencial con la informacion del usuario</returns>
        public SECredencialUsuario ObtenerMenusModulosXUsuario(SECredencialUsuario credencial)
        {
            // TODO: ID, Se evita el uso de Entityframework, por que esta sobrecargando el servidor en la autenticacion
            // MODULOS - MENUS - Acciones
            DataSet dsRes = new DataSet();
            SqlDataAdapter da;
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerModulosMenuRol_SEG", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@USU_IdUsuario", credencial.Usuario));

                da = new SqlDataAdapter(cmd);
                da.Fill(dsRes);
            }

            SEModulo Modulo = null;
            SEMenu men = null;

            credencial.Modulos = new List<SEModulo>();
            foreach (DataRow iteRowModulo in dsRes.Tables[0].Rows)
            {
                Modulo = new SEModulo();
                Modulo.Descripcion = iteRowModulo["MOD_Descripcion"].ToString();
                Modulo.IdModulo = iteRowModulo["MOD_IdModulo"].ToString();

                // Se asignan los Menus
                DataRow[] dtFiltroMenu = dsRes.Tables[1].Select("MEN_IdModulo = '" + Modulo.IdModulo + "'");
                Modulo.Menus = new List<SEMenu>();
                foreach (DataRow iteRowMenu in dtFiltroMenu)
                {
                    men = new SEMenu()
                    {
                        Assembly = iteRowMenu["MEN_Assembly"].ToString(),
                        Comentarios = iteRowMenu["MEN_Comentarios"].ToString(),
                        Etiqueta = iteRowMenu["MEN_Etiqueta"].ToString(),
                        IdMenu = Convert.ToInt32(iteRowMenu["MEN_IdMenu"]),
                        NameSpace = iteRowMenu["MEN_NameSpace"].ToString(),
                        UserControl = iteRowMenu["MEN_UserControl"].ToString(),
                        UrlRelativa = iteRowMenu["MEN_UrlRelativa"].ToString(),
                        AplicaAgencia = Convert.ToBoolean(iteRowMenu["MEN_AplicaAgencia"]),
                        AplicaCol = Convert.ToBoolean(iteRowMenu["MEN_AplicaCol"]),
                        AplicaPunto = Convert.ToBoolean(iteRowMenu["MEN_AplicaPunto"]),
                        AplicaRacol = Convert.ToBoolean(iteRowMenu["MEN_AplicaRacol"]),
                        AplicaGestion = Convert.ToBoolean(iteRowMenu["MEN_AplicaGestion"]),
                        AplicaClienteCredito = Convert.ToBoolean(iteRowMenu["MEN_AplicaCliente"]),
                        MenuWPF = iteRowMenu["MEN_MenuWPF"].ToString(),
                        Imagen = (iteRowMenu["MEN_UrlImagenPub"] != null) ? iteRowMenu["MEN_UrlImagenPub"].ToString() : null
                    };

                    Modulo.Menus.Add(men);
                }

                // Agregar imagenes
                Modulo.Menus.ForEach(m =>
                  {
                      if (m.Imagen != null)
                      {
                          try
                          {
                              m.Imagen = Convert.ToBase64String(System.IO.File.ReadAllBytes(m.Imagen));
                          }
                          catch
                          {
                              m.Imagen = null;
                          }
                      }
                  });

                // Se asignan las Acciones a los Menus
                foreach (SEMenu iteMenu in Modulo.Menus)
                {
                    // Se recorren las Acciones para asignarlas al Menu Actual
                    DataRow[] dtfiltroAcciones = dsRes.Tables[2].Select("MER_IdMenu = " + iteMenu.IdMenu);
                    iteMenu.Acciones = new List<string>();
                    foreach (DataRow iteRowAccion in dtfiltroAcciones)
                        iteMenu.Acciones.Add(iteRowAccion["MRA_IdAccion"].ToString());
                }

                credencial.Modulos.Add(Modulo);
            }

            return credencial;

            #region CodigoAnterior

            //using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //    var menus = (from usu in contexto.UsuarioRol_SEG
            //                 join rol in contexto.Rol_SEG on usu.USR_IdRol equals rol.ROL_IdRol
            //                 join menuRol in contexto.MenuRol_SEG on rol.ROL_IdRol equals menuRol.MER_IdRol
            //                 join menu in contexto.Menu_SEG on menuRol.MER_IdMenu equals menu.MEN_IdMenu
            //                 join usuario in contexto.Usuario_SEG on usu.USR_IdCodigoUsuario equals usuario.USU_IdCodigoUsuario
            //                 where usuario.USU_IdUsuario == credencial.Usuario
            //                 select new { menu, menuRol }).ToList();

            //    var modulos = (from menu in menus
            //                   join modulo in contexto.Modulo_VER on menu.menu.MEN_IdModulo equals modulo.MOD_IdModulo
            //                   select new { modulo, menu.menuRol }).GroupBy(m => m.modulo.MOD_IdModulo).Select(m => m.First()).ToList();

            //    credencial.Modulos = modulos.ConvertAll<SEModulo>(obj =>
            //      {
            //          SEModulo modulo = new SEModulo();

            //          modulo.Descripcion = obj.modulo.MOD_Descripcion;
            //          modulo.IdModulo = obj.modulo.MOD_IdModulo;
            //          modulo.Menus = (from m in menus
            //                          where m.menu.MEN_IdModulo == obj.modulo.MOD_IdModulo
            //                          select m).GroupBy(m => m.menu.MEN_IdMenu).Select(m => m.First()).ToList().ConvertAll<SEMenu>(menu =>
            //                     {
            //                         SEMenu men = new SEMenu()
            //                           {
            //                               Assembly = menu.menu.MEN_Assembly,
            //                               Comentarios = menu.menu.MEN_Comentarios,
            //                               Etiqueta = menu.menu.MEN_Etiqueta,
            //                               IdMenu = menu.menu.MEN_IdMenu,
            //                               NameSpace = menu.menu.MEN_NameSpace,
            //                               UserControl = menu.menu.MEN_UserControl,
            //                               UrlRelativa = menu.menu.MEN_UrlRelativa,
            //                               AplicaAgencia = menu.menu.MEN_AplicaAgencia,
            //                               AplicaCol = menu.menu.MEN_AplicaCol,
            //                               AplicaPunto = menu.menu.MEN_AplicaPunto,
            //                               AplicaRacol = menu.menu.MEN_AplicaRacol,
            //                               AplicaGestion = menu.menu.MEN_AplicaGestion,
            //                               AplicaClienteCredito = menu.menu.MEN_AplicaCliente
            //                           };

            //                         men.Acciones = contexto.MenuRolAccion_SEG.Where(mra => mra.MRA_IdMenuRol == menu.menuRol.MER_IdMenuRol).Select(mra => mra.MRA_IdAccion).ToList();

            //                         return men;
            //                     });

            //          return modulo;
            //      });

            //    return credencial;
            //}

            #endregion CodigoAnterior
        }

        /// <summary>
        /// Retorna los menus y los modulos dependiendo del usuario y el sistema de información que está utilizando
        /// </summary>
        /// <param name="credencial">Credencial con la informacion del usuario</param>
        /// <returns>Credencial con la informacion del usuario</returns>
        public SECredencialUsuario ObtenerMenusModulosXUsuarioYSInformacion(SECredencialUsuario credencial, SEEnumSistemaInformacion sistemaInformacion)
        {
            // TODO: ID, Se evita el uso de Entityframework, por que esta sobrecargando el servidor en la autenticacion
            // MODULOS - MENUS - Acciones
            DataSet dsRes = new DataSet();
            SqlDataAdapter da;
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerModulosMenuRolXSistema_SEG", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@USU_IdUsuario", credencial.Usuario));
                cmd.Parameters.Add(new SqlParameter("@IdSistemaInformacion", (short)sistemaInformacion));

                da = new SqlDataAdapter(cmd);
                da.Fill(dsRes);
            }

            SEModulo Modulo = null;
            SEMenu men = null;

            //credencial.Modulos = new List<SEModulo>();

            credencial.Modulos = dsRes.Tables[0].Select().GroupBy(g => new { IdModulo = g["MEN_IdModulo"], NombreModulo = g["MOD_Descripcion"] }).ToList().ConvertAll<SEModulo>((m) =>
            {
                return new SEModulo()
                {
                    IdModulo = m.Key.IdModulo.ToString(),
                    Descripcion = m.Key.NombreModulo.ToString()
                };
            });
            //List<SEModulo> modulos = dsRes.Tables[0].Select().GroupBy(g => g["MOD_Descripcion"]).ToList().ConvertAll<SEModulo>((m) =>
            //{
            //    return new SEModulo()
            //    {
            //        Descripcion = m.["MOD_Descripcion"].ToString(),
            //        IdModulo = m["MOD_IdModulo"].ToString()
            //    };
            //});

            foreach (SEModulo iteRowModulo in credencial.Modulos)
            {
                // Se asignan los Menus
                DataRow[] dtFiltroMenu = dsRes.Tables[0].Select("MEN_IdModulo = '" + iteRowModulo.IdModulo + "'");
                iteRowModulo.Menus = new List<SEMenu>();
                foreach (DataRow iteRowMenu in dtFiltroMenu)
                {
                    men = new SEMenu()
                    {
                        Assembly = iteRowMenu["MEN_Assembly"].ToString(),
                        Comentarios = iteRowMenu["MEN_Comentarios"].ToString(),
                        Etiqueta = iteRowMenu["MEN_Etiqueta"].ToString(),
                        IdMenu = Convert.ToInt32(iteRowMenu["MEN_IdMenu"]),
                        NameSpace = iteRowMenu["MEN_NameSpace"].ToString(),
                        UserControl = iteRowMenu["MEN_UserControl"].ToString(),
                        UrlRelativa = iteRowMenu["MEN_UrlRelativa"].ToString(),
                        AplicaAgencia = Convert.ToBoolean(iteRowMenu["MEN_AplicaAgencia"]),
                        AplicaCol = Convert.ToBoolean(iteRowMenu["MEN_AplicaCol"]),
                        AplicaPunto = Convert.ToBoolean(iteRowMenu["MEN_AplicaPunto"]),
                        AplicaRacol = Convert.ToBoolean(iteRowMenu["MEN_AplicaRacol"]),
                        AplicaGestion = Convert.ToBoolean(iteRowMenu["MEN_AplicaGestion"]),
                        AplicaClienteCredito = Convert.ToBoolean(iteRowMenu["MEN_AplicaCliente"]),
                        MenuWPF = iteRowMenu["MEN_MenuWPF"].ToString()
                    };

                    iteRowModulo.Menus.Add(men);
                }

                // Se asignan las Acciones a los Menus
                foreach (SEMenu iteMenu in iteRowModulo.Menus)
                {
                    // Se recorren las Acciones para asignarlas al Menu Actual
                    DataRow[] dtfiltroAcciones = dsRes.Tables[1].Select("MER_IdMenu = " + iteMenu.IdMenu);
                    iteMenu.Acciones = new List<string>();
                    foreach (DataRow iteRowAccion in dtfiltroAcciones)
                        iteMenu.Acciones.Add(iteRowAccion["MRA_IdAccion"].ToString());
                }
            }

            return credencial;
        }

        /// <summary>
        /// Obtiene los roles por usuario asignados y sin asignar
        /// </summary>
        /// <param name="credencial">Objeto con la información del usuario</param>
        /// <returns>Retorna la lista de los roles asignados y sin asignar</returns>
        public SERolUsuario ObtenerRolesUsuario(string IdUsuario)
        {
            SERolUsuario rolUsuario = new SERolUsuario();

            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                IEnumerable<Rol_SEG> rolesAsignados = contexto.UsuarioRol_SEG
                  .Where(usuarioRol => usuarioRol.Usuario_SEG.USU_IdUsuario == IdUsuario)
                  .Select(usuarioRol => usuarioRol.Rol_SEG);

                rolUsuario.RolesAsignados = rolesAsignados
                  .ToList()
                  .ConvertAll<SERol>(rol => new SERol()
                  {
                      IdRol = rol.ROL_IdRol,
                      Descripcion = rol.ROL_Descripcion,
                      RequiereIdMaquina = rol.ROL_RequiereIdMaquina,
                      EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS
                  });

                List<Rol_SEG> rolesSinAsignar = contexto.Rol_SEG
                  .Except(rolesAsignados)
                  .ToList();

                rolUsuario.RolesSinAsignar = rolesSinAsignar
                  .ConvertAll<SERol>(rol => new SERol()
                  {
                      IdRol = rol.ROL_IdRol,
                      Descripcion = rol.ROL_Descripcion,
                      EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS
                  });

                return rolUsuario;
            }
        }

        /// <summary>
        /// Obtiene los roles por usuario asignados
        /// </summary>
        /// <param name="credencial">Objeto con la información del usuario</param>
        /// <returns>Retorna la lista de los roles asignados</returns>
        public SERolUsuario ObtenerRolesAsignadosUsuario(string IdUsuario)
        {
            // TODO: ID, Se evita el uso de Entityframework, por que esta sobrecargando el servidor en la autenticacion
            SERolUsuario rolUsuario = new SERolUsuario();
            rolUsuario.RolesAsignados = new List<SERol>();
            SERol newRol = null;

            string query = @"SELECT ROL_IdRol, ROL_Descripcion, ROL_RequiereIdMaquina"
                        + " FROM UsuarioRol_SEG AS USROL INNER JOIN Usuario_SEG AS USU  ON USROL.USR_IdCodigoUsuario = USU.USU_IdCodigoUsuario"
                        + " LEFT OUTER JOIN Rol_SEG AS ROL ON USROL.USR_IdRol = ROL.ROL_IdRol"
                        + " WHERE USU.USU_IdUsuario = '" + IdUsuario + "'";

            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand(query, sqlConn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    newRol = new SERol();
                    newRol.IdRol = reader["ROL_IdRol"].ToString();
                    newRol.Descripcion = reader["ROL_Descripcion"].ToString();
                    newRol.RequiereIdMaquina = Convert.ToBoolean(reader["ROL_RequiereIdMaquina"]);
                    newRol.EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS;

                    rolUsuario.RolesAsignados.Add(newRol);
                }
            }

            return rolUsuario;

            //using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //    IEnumerable<Rol_SEG> rolesAsignados = contexto.UsuarioRol_SEG
            //      .Where(usuarioRol => usuarioRol.Usuario_SEG.USU_IdUsuario == IdUsuario)
            //      .Select(usuarioRol => usuarioRol.Rol_SEG);

            //    rolUsuario.RolesAsignados = rolesAsignados
            //      .ToList()
            //      .ConvertAll<SERol>(rol => new SERol()
            //      {
            //          IdRol = rol.ROL_IdRol,
            //          Descripcion = rol.ROL_Descripcion,
            //          RequiereIdMaquina = rol.ROL_RequiereIdMaquina,
            //          EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS
            //      });

            //    return rolUsuario;
            //}
        }

        /// <summary>
        /// Elimina o guarda la relacion entre usuario y roles dentro de una transaccion
        /// </summary>
        /// <param name="rolAgregado"></param>
        /// <param name="rolEliminado"></param>
        public void ModificarRolesUsuarioDB(SERolUsuario rolAgregado, SERolUsuario rolEliminado)
        {
            using (TransactionScope trans = new TransactionScope())
            {
                GuardarRolesUsuario(rolAgregado);
                EliminarRolUsuario(rolEliminado);
                trans.Complete();
            }
        }

        /// <summary>
        /// Retorna la informacion basica del usuario
        /// </summary>
        /// <param name="credencial">credencial con el nombre de usuario de la cuenta</param>
        /// <returns>Credencial con la informacion basica del usuario</returns>
        public SECredencialUsuario ObtieneInformacionBasicaUsuario(SECredencialUsuario credencial)
        {
            // TODO: ID, Se evita el uso de Entityframework, por que esta sobrecargando el servidor en la autenticacion
            DataSet dsRes = new DataSet();
            SqlDataAdapter da;
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerAutenticacion_SEG", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@USU_IdUsuario", credencial.Usuario));             // usuario
                cmd.Parameters.Add(new SqlParameter("@IdCodigoUsuario", credencial.IdCodigoUsuario));   // Id de la tabla
                cmd.Parameters.Add(new SqlParameter("@USU_Estado", ConstantesFramework.ESTADO_ACTIVO));

                da = new SqlDataAdapter(cmd);
                da.Fill(dsRes);
            }

            #region Consultas del DataSet

            SECredencialUsuario secredencial = null;

            // 1
            DataRow dr1 = dsRes.Tables[0].Rows[0];

            credencial.EsUsuarioInterno = Convert.ToBoolean(dr1["USU_EsUsuarioInterno"]);
            credencial.TipoUsuario = dr1["USU_TipoUsuario"].ToString();
            credencial.IdCodigoUsuario = Convert.ToInt64(dr1["USU_IdCodigoUsuario"]);     //
            credencial.Estado = dr1["USU_Estado"].ToString();

            // Se obtiene información de la tabla PersonaInterna o Externa de acuerdo a la propiedad "EsUsuarioInterno"
            if (credencial.EsUsuarioInterno || credencial.TipoUsuario == COConstantesModulos.USUARIO_LDAP)
            {
                // 2
                if (!(dsRes.Tables[1].Rows.Count > 0))
                {
                    ControllerException excepcion = new ControllerException(ConstantesFramework.MODULO_FW_SEGURIDAD, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }

                DataRow dr2 = dsRes.Tables[1].Rows[0];
                secredencial = new SECredencialUsuario()
                {
                    IdPersonaInterna = Convert.ToInt64(dr2["PEI_IdPersonaInterna"]),
                    Usuario = dr2["USU_IdUsuario"].ToString(),
                    Apellido1 = dr2["PEI_PrimerApellido"].ToString(),
                    Apellido2 = dr2["PEI_SegundoApellido"].ToString(),
                    Direccion = dr2["PEI_Direccion"].ToString(),
                    Email = dr2["PEI_Email"].ToString(),
                    Nombre = dr2["PEI_Nombre"].ToString(),
                    RequiereIdentificadorMaquina = Convert.ToBoolean(dr2["USU_RequiereIdMaquina"]),
                    Telefono = dr2["PEI_Telefono"].ToString(),
                    Identificacion = dr2["PEI_Identificacion"].ToString(),
                    TipoIdentificacion = dr2["TII_Descripcion"].ToString(),
                    Password = string.Empty,
                    EsUsuarioInterno = credencial.EsUsuarioInterno,
                    IdCodigoUsuario = Convert.ToInt64(dr2["USU_IdCodigoUsuario"]),
                    AutorizaCargaMasiva = Convert.ToBoolean(dr2["USU_AutorizaCargaMasiva"]),
                    AutorizaCargaMasivaICA = Convert.ToBoolean(dr2["USU_AutorizaCargaMasivaICA"])
                };

                secredencial.LocacionesAutorizadas = new List<SEUbicacionAutorizada>();
                // 3
                foreach (DataRow iteRow in dsRes.Tables[2].Rows)
                {
                    secredencial.LocacionesAutorizadas.Add(new SEUbicacionAutorizada()
                    {
                        IdLocacion = iteRow["UCS_IdCentroServicios"].ToString(),
                        TipoLocacion = TipoLocacionAutorizada.CentroServicios,
                        DescripcionLocacion = iteRow["UCS_NombreCentroServicios"].ToString(),
                        IdCaja = Convert.ToInt32(iteRow["UCS_Caja"]),
                        IdCiudad = iteRow["CES_IdMunicipio"].ToString(),
                        DescripcionCiudad = iteRow["LOC_Nombre"].ToString(),
                        IdCentroCostos = iteRow["CES_IdCentroCostos"].ToString(),
                        TipoCentroServicio = iteRow["CES_Tipo"].ToString(),
                        ImpresionPos = Convert.ToBoolean(iteRow["UCS_ImpresionPOS"]),
                        Operacional = Convert.ToBoolean(iteRow["AGE_Operacional"]),
                        IdCentroServiciosOrigen = Convert.ToInt64(iteRow["IdCol"]),
                        EsCustodia = Convert.ToBoolean(iteRow["EsCustodia"])
                    });
                }

                // 4
                foreach (DataRow iteRow in dsRes.Tables[3].Rows)
                {
                    secredencial.LocacionesAutorizadas.Add(new SEUbicacionAutorizada()
                    {
                        IdLocacion = iteRow["USG_CodigoGestion"].ToString(),
                        TipoLocacion = TipoLocacionAutorizada.Gestion,
                        DescripcionLocacion = iteRow["USG_Descripcion"].ToString()
                    });
                }
            }
            else
            {
                using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
                {
                    // 5
                    UsuarioClienteCredito_VSEG clienteCredito = contexto.UsuarioClienteCredito_VSEG
                      .Where(cliente => cliente.USU_IdCodigoUsuario == credencial.IdCodigoUsuario
                            && cliente.USU_Estado == ConstantesFramework.ESTADO_ACTIVO
                            && cliente.CLI_Estado == ConstantesFramework.ESTADO_ACTIVO)
                     .FirstOrDefault();
                    if (clienteCredito == null)
                    {
                        ControllerException excepcion = new ControllerException(ConstantesFramework.MODULO_FW_SEGURIDAD, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                        throw new FaultException<ControllerException>(excepcion);
                    }

                    secredencial = new SECredencialUsuario()
                    {
                        Usuario = clienteCredito.USU_IdUsuario,
                        Apellido1 = clienteCredito.PEE_PrimerApellido,
                        Apellido2 = clienteCredito.PEE_SegundoApellido,
                        Direccion = clienteCredito.PEE_Direccion,
                        Nombre = clienteCredito.PEE_PrimerNombre,
                        RequiereIdentificadorMaquina = clienteCredito.USU_RequiereIdMaquina,
                        Telefono = clienteCredito.PEE_Telefono,
                        Identificacion = clienteCredito.PEE_Identificacion,
                        TipoIdentificacion = clienteCredito.TII_Descripcion,
                        Password = string.Empty,
                        AutorizaCargaMasiva = clienteCredito.USU_AutorizaCargaMasiva,
                        IdClienteCredito = clienteCredito.CLI_IdCliente
                    };

                    secredencial.LocacionesAutorizadas = new List<SEUbicacionAutorizada>();
                    // 6
                    foreach (UsuarioSucursal_SEG usuGS in contexto.UsuarioSucursal_SEG.Include("Sucursal_CLI").Include("Sucursal_CLI.ClienteCredito_CLI").Where(us => us.USS_IdCodigoUsuario == credencial.IdCodigoUsuario && us.Sucursal_CLI.ClienteCredito_CLI.CLI_Estado == "ACT").ToList())
                    {
                        secredencial.LocacionesAutorizadas.Add(new SEUbicacionAutorizada()
                        {
                            IdLocacion = usuGS.USS_IdSucursal.ToString(),
                            TipoLocacion = TipoLocacionAutorizada.Sucursal,
                            DescripcionLocacion = usuGS.USS_Nombre,
                            IdCiudad = usuGS.Sucursal_CLI.SUC_Municipio,
                            DescripcionCiudad = usuGS.Sucursal_CLI.SUC_Nombre
                        });
                    }
                }
            }

            SERolUsuario rolesusuarios = ObtenerRolesAsignadosUsuario(secredencial.Usuario);
            if (rolesusuarios.RolesAsignados.Exists(rol => rol.RequiereIdMaquina))
            {
                secredencial.RequiereIdentificadorMaquina = true;
            }

            return secredencial;

            #endregion Consultas del DataSet
        }

        // todo:id Consulta de datos Basicos para consumir desde afuera de controller ()

        /// <summary>
        /// Retorna la informacion basica del usuario  (Se utiliza desde Afuera de Controller)
        /// </summary>
        /// <param name="credencial">credencial con el nombre de usuario de la cuenta</param>
        /// <returns>Credencial con la informacion basica del usuario</returns>
        public SECredencialUsuario ObtieneInformacionBasicaUsuarioXUsuario(string usuario)
        {
            if (string.IsNullOrEmpty(usuario))
                return null;

            DataSet dsRes = new DataSet();
            SqlDataAdapter da;
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerInformacionBasicaUsuario_SEG", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@USU_IdUsuario", usuario));
                cmd.Parameters.Add(new SqlParameter("@USU_Estado", ConstantesFramework.ESTADO_ACTIVO));

                da = new SqlDataAdapter(cmd);
                da.Fill(dsRes);
            }

            SECredencialUsuario secredencial = null;

            if (!(dsRes.Tables[0].Rows.Count > 0))
            {
                ControllerException excepcion = new ControllerException(ConstantesFramework.MODULO_FW_SEGURIDAD, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                throw new FaultException<ControllerException>(excepcion);
            }

            DataRow dr1 = dsRes.Tables[0].Rows[0];
            secredencial = new SECredencialUsuario()
            {
                Usuario = dr1["USU_IdUsuario"].ToString(),                      // usuario
                IdCodigoUsuario = Convert.ToInt64(dr1["USU_IdCodigoUsuario"]),
                IdPersonaInterna = Convert.ToInt64(dr1["PEI_IdPersonaInterna"]),
                TipoIdentificacion = dr1["PEI_IdTipoIdentificacion"].ToString(),
                Identificacion = dr1["PEI_Identificacion"].ToString(),
                Nombre = dr1["PEI_Nombre"].ToString(),
                Apellido1 = dr1["PEI_PrimerApellido"].ToString(),
                Apellido2 = dr1["PEI_SegundoApellido"].ToString(),
                Direccion = dr1["PEI_Direccion"].ToString(),
                Email = dr1["PEI_Email"].ToString(),
                Telefono = dr1["PEI_Telefono"].ToString(),
                Cargo = dr1["CargoNova"].ToString(),

                Password = string.Empty,
            };

            return secredencial;
        }

        /// <summary>
        ///retorna el tipo de usuario
        /// </summary>
        /// <param name="credencial">Credencial con la informacion del usuario</param>
        public string ObtenerTipoUsuario(SECredencialUsuario credencial)
        {
            // TODO: ID, Se evita el uso de Entityframework, por que esta sobrecargando el servidor en la autenticacion
            string strTipoUsuario = null;
            string query = @"SELECT USU_TipoUsuario FROM dbo.Usuario_SEG WHERE USU_IdUsuario = '" + credencial.Usuario + "' AND USU_Estado = '" + ConstantesFramework.ESTADO_ACTIVO + "'";

            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand(query, sqlConn);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                    strTipoUsuario = reader["USU_TipoUsuario"].ToString();
            }

            return strTipoUsuario;

            //using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //    Usuario_SEG ObjUsuario = contexto.Usuario_SEG
            //      .Where(cre => cre.USU_IdUsuario == credencial.Usuario && cre.USU_Estado == ConstantesFramework.ESTADO_ACTIVO)
            //      .FirstOrDefault();

            //    if (ObjUsuario == null)
            //    {
            //        return null;
            //    }
            //    else
            //    {
            //        return ObjUsuario.USU_TipoUsuario;
            //    }
            //}
        }

        /// <summary>
        /// Autentica un usuario contra la base de datos
        /// </summary>
        /// <param name="credencial">Credencial con la informacion del usuario</param>
        public SECredencialUsuario AutenticarUsuario(SECredencialUsuario credencial)
        {
            // TODO: ID, Se evita el uso de Entityframework, por que esta sobrecargando el servidor en la autenticacion
            SECredencialUsuario ObjUsuarioCredencial = null;

            string query = @"SELECT CRU_Clave, CRU_FormatoClave
                        , CRU_CantIntentosFallidosClave, CRU_ClaveBloqueada
                        , CRU_ClaveAnterior, CRU_DiasVencimiento
                        , CRU_InicioIntentosFallidos, CRU_IdCodigoUsuario
                        , CRU_FechaUltimoCambioClave, CRU_FechaUltimoBloqueoClave
                        , USU_IdUsuario, USU_TipoUsuario
                        , USU_EsUsuarioInterno, USU_RequiereIdMaquina
                        , USU_Estado, USU_AutorizaCargaMasiva
                        , USU_AutorizaCargaMasivaICA
                         FROM CredencialUsuario_VSEG WHERE USU_IdUsuario = @usuario AND USU_Estado = @estado";

            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand(query, sqlConn);
                cmd.Parameters.AddWithValue("@usuario", credencial.Usuario);
                cmd.Parameters.AddWithValue("@estado", ConstantesFramework.ESTADO_ACTIVO);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    ObjUsuarioCredencial = new SECredencialUsuario();

                    ObjUsuarioCredencial.Usuario = reader["USU_IdUsuario"].ToString();
                    ObjUsuarioCredencial.Password = reader["CRU_Clave"].ToString();
                    ObjUsuarioCredencial.FormatoClave = reader["CRU_FormatoClave"].ToString().ToLower();
                    ObjUsuarioCredencial.CantidadIntentosFallidos = Convert.ToInt16(reader["CRU_CantIntentosFallidosClave"]);
                    ObjUsuarioCredencial.ClaveBloqueada = Convert.ToBoolean(reader["CRU_ClaveBloqueada"]);
                    ObjUsuarioCredencial.PasswordAnterior = reader["CRU_ClaveAnterior"].ToString();
                    ObjUsuarioCredencial.DiasVencimiento = Convert.ToInt32(reader["CRU_DiasVencimiento"]);
                    ObjUsuarioCredencial.TipoUsuario = reader["USU_TipoUsuario"].ToString();
                    ObjUsuarioCredencial.EsUsuarioInterno = Convert.ToBoolean(reader["USU_EsUsuarioInterno"]);
                    ObjUsuarioCredencial.IdCodigoUsuario = Convert.ToInt64(reader["CRU_IdCodigoUsuario"]);
                    ObjUsuarioCredencial.AutorizaCargaMasiva = Convert.ToBoolean(reader["USU_AutorizaCargaMasiva"]);
                    ObjUsuarioCredencial.AutorizaCargaMasivaICA = Convert.ToBoolean(reader["USU_AutorizaCargaMasivaICA"]);
                    if (reader["CRU_InicioIntentosFallidos"] != DBNull.Value) ObjUsuarioCredencial.InicioIntentosFallidos = Convert.ToDateTime(reader["CRU_InicioIntentosFallidos"]);
                    if (reader["CRU_FechaUltimoCambioClave"] != DBNull.Value) ObjUsuarioCredencial.FechaUltimoCambioClave = Convert.ToDateTime(reader["CRU_FechaUltimoCambioClave"]);
                }
            }

            return ObjUsuarioCredencial;


        }

        /// <summary>
        /// Inicia el tiempo para los intetos de autenticacion fallidos
        /// </summary>
        /// <param name="credencial">Credencial con la informacion del usuario</param>
        public void IniciarTiempoIntentosFallidos(SECredencialUsuario credencial)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                CredencialUsuario_SEG ObjUsuario = contexto.CredencialUsuario_SEG
                  .Where(cred => cred.CRU_IdCodigoUsuario == credencial.IdCodigoUsuario)
                  .SingleOrDefault();

                ObjUsuario.CRU_InicioIntentosFallidos = DateTime.Now;
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Incrementa el numero de intentos fallidos y bloquea al usuario si es requerido
        /// </summary>
        /// <param name="credencial">Credencial con la informacion del usuario.</param>
        public void IncrementarIntentosFallidos(SECredencialUsuario credencial)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                CredencialUsuario_SEG ObjUsuario = contexto.CredencialUsuario_SEG
                  .Where(cred => cred.CRU_IdCodigoUsuario == credencial.IdCodigoUsuario)
                  .SingleOrDefault();

                ObjUsuario.CRU_CantIntentosFallidosClave++;
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Consulta los parametros generales del modulo de seguridad
        /// </summary>
        /// <param name="llave">Nombre del parametro</param>
        /// <returns>Valor el parametro</returns>
        public string ConsultarParametrosSeguridad(string llave)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                ParametrosFramework ObjParametro = (from obj in contexto.ParametrosFramework
                                                    where obj.PAR_IdParametro == llave
                                                    select obj).SingleOrDefault();

                if (ObjParametro == null)
                {
                    return String.Empty;
                }
                else
                {
                    return ObjParametro.PAR_ValorParametro;
                }
            }
        }

        /// <summary>
        /// Guarda y actualiza el valor del dolar
        /// por defecto el cual se utiliza en caso de no tener
        /// acceso a internet
        /// </summary>
        /// <param name="valorDolar"></param>
        public void ActualizarValorDolarPorDefecto(string valorDolar)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                ParametrosFramework valorActualizar = contexto.ParametrosFramework.FirstOrDefault(val => val.PAR_IdParametro == "ValorDolar");

                if (valorActualizar != null && !string.IsNullOrEmpty(valorActualizar.PAR_ValorParametro))
                {
                    valorActualizar.PAR_ValorParametro = valorDolar;
                    contexto.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Reinicia los intentos de autenticacion.
        /// </summary>
        public void ReiniciarIntentosAutenticacion(SECredencialUsuario credencial)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                CredencialUsuario_SEG ObjUsuario = contexto.CredencialUsuario_SEG
                  .Where(cred => cred.CRU_IdCodigoUsuario == credencial.IdCodigoUsuario)
                  .SingleOrDefault();

                ObjUsuario.CRU_CantIntentosFallidosClave = 0;
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Bloquea un usuario
        /// </summary>
        /// <param name="credencial">Credencial con la informacion del usuario</param>
        public void BloquearUsuario(SECredencialUsuario credencial)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                CredencialUsuario_SEG ObjUsuario =
                  contexto.CredencialUsuario_SEG
                  .Where(cred => cred.CRU_IdCodigoUsuario == credencial.IdCodigoUsuario)
                  .SingleOrDefault();

                ObjUsuario.CRU_ClaveBloqueada = true;
                ObjUsuario.CRU_FechaUltimoBloqueoClave = DateTime.Now;
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Desactiva un usuario de la base de datos
        /// </summary>
        /// <param name="credencial">Credencial con la informacion del usuario</param>
        public void DesactivarUsuario(SECredencialUsuario credencial)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Usuario_SEG ObjUsuario = (from obj in contexto.Usuario_SEG
                                          where obj.USU_IdUsuario == credencial.Usuario
                                          select obj).SingleOrDefault();
                if (ObjUsuario != null)
                {
                    ObjUsuario.USU_Estado = ConstantesFramework.ESTADO_INACTIVO;
                    contexto.SaveChanges();
                }
                else
                {
                    ControllerException excepcion = new ControllerException(ConstantesFramework.MODULO_FW_SEGURIDAD, ETipoErrorFramework.EX_USUARIO_CLAVE_NO_VALIDO.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_USUARIO_CLAVE_NO_VALIDO));
                    throw new FaultException<ControllerException>(excepcion);
                }
            }
        }

        /// <summary>
        /// Activa una cuenta de usuario
        /// </summary>
        /// <param name="credencial">Credencial con la inforamcion del usuario</param>
        public void ActivarUsuario(SECredencialUsuario credencial)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Usuario_SEG ObjUsuario = (from obj in contexto.Usuario_SEG
                                          where obj.USU_IdUsuario == credencial.Usuario
                                          select obj).FirstOrDefault();

                CredencialUsuario_SEG ObjCredencial = contexto.CredencialUsuario_SEG.
                  Include("USUARIO_SEG").Where(cred => cred.Usuario_SEG != null && cred.Usuario_SEG.USU_IdUsuario == credencial.Usuario)
                  .FirstOrDefault();

                if (ObjUsuario != null)
                {
                    ObjUsuario.USU_Estado = "ACT";

                    if (ObjCredencial != null)
                        ObjCredencial.CRU_CantIntentosFallidosClave = 0;

                    contexto.SaveChanges();
                }
                else
                    throw new Exception("El usuario no existe");
            }
        }

        /// <summary>
        /// Cambia la contraseña de un usuario, valida la contraseña actual y guarda la nueva
        /// </summary>
        /// <param name="credencial">Credencial con la informacion del usuario</param>
        public void CambiarPassword(SECredencialUsuario credencial)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                CredencialUsuario_SEG ObjUsuario = contexto.CredencialUsuario_SEG
                  .Include("USUARIO_SEG")
                  .Where(cred => cred.Usuario_SEG != null && cred.Usuario_SEG.USU_IdUsuario == credencial.Usuario)
                  .FirstOrDefault();

                if (ObjUsuario != null)
                {
                    if (!string.IsNullOrWhiteSpace(credencial.PasswordNuevo))
                    {
                        string claveHash = credencial.PasswordNuevo;
                        string claveActualHash = ObjUsuario.CRU_Clave;

                        if (ObjUsuario.CRU_FormatoClave.Trim().Equals(FORMATO_CLAVE_HASHED, StringComparison.OrdinalIgnoreCase))
                        {
                            claveHash = COEncripcion.ObtieneHash(credencial.PasswordNuevo);
                            claveActualHash = COEncripcion.ObtieneHash(credencial.PasswordAnterior);
                        }

                        // validar que la clave actual sea correcta
                        if (claveActualHash == ObjUsuario.CRU_Clave)
                        {
                            if (claveHash != ObjUsuario.CRU_Clave)
                            {
                                ObjUsuario.CRU_FechaUltimoCambioClave = DateTime.Now;
                                ObjUsuario.CRU_ClaveAnterior = claveActualHash;
                                ObjUsuario.CRU_Clave = claveHash;
                                ObjUsuario.CRU_DiasVencimiento = Convert.ToInt32(ParametrosFW.PAAdministrador.Instancia.ConsultarParametrosFramework("DiasVencimiento"));
                                string cargoPwdVence = ParametrosFW.PAAdministrador.Instancia.ConsultarParametrosFramework("CargoPwdVenceRapido");
                                if (!string.IsNullOrWhiteSpace(cargoPwdVence))
                                {
                                    string[] cargos = cargoPwdVence.Split(',');
                                    UsuarioPersonaInterna_VSEG usu = contexto.UsuarioPersonaInterna_VSEG.FirstOrDefault(u => u.USU_IdUsuario == credencial.Usuario);
                                    if (usu != null)
                                    {
                                        if (cargos.Count(c => int.Parse(c) == usu.PEI_IdCargo) > 0)
                                        {
                                            ObjUsuario.CRU_DiasVencimiento = 1;
                                        }
                                    }
                                }
                                ObjUsuario.CRU_FormatoClave = ObjUsuario.CRU_FormatoClave;
                                ObjUsuario.CRU_ClaveBloqueada = credencial.ClaveBloqueada;
                                SERepositorioAudit.MapeoAuditCambiarPassword(contexto, credencial.Usuario);
                                contexto.SaveChanges();
                            }
                            else
                            {
                                string mensaje = "La nueva clave debe ser diferente a la clave anterior";
                                ControllerException excepcion = new ControllerException(ConstantesFramework.MODULO_FW_SEGURIDAD, "Error", mensaje);
                                throw new FaultException<ControllerException>(excepcion);
                            }
                        }
                        else
                        {
                            string mensaje = "La clave actual no coincide con la clave actual ingresada";
                            ControllerException excepcion = new ControllerException(ConstantesFramework.MODULO_FW_SEGURIDAD, "Error", mensaje);
                            throw new FaultException<ControllerException>(excepcion);
                        }
                    }
                    else
                    {
                        ObjUsuario.CRU_ClaveBloqueada = credencial.ClaveBloqueada;
                        contexto.SaveChanges();
                    }
                }
                else if (ObjUsuario == null)
                {
                    AdicionarPassword(credencial);
                }
            }
        }

        /// <summary>
        /// Cambia la contraseña de un usuario, valida la contraseña actual y guarda la nueva
        /// </summary>
        /// <param name="credencial">Credencial con la informacion del usuario</param>
        public void ResetearPassword(SECredencialUsuario credencial)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                CredencialUsuario_SEG ObjUsuario = contexto.CredencialUsuario_SEG
                  .Include("USUARIO_SEG")
                  .Where(cred => cred.Usuario_SEG != null && cred.Usuario_SEG.USU_IdUsuario == credencial.Usuario)
                  .FirstOrDefault();

                if (ObjUsuario != null)
                {
                    if (!string.IsNullOrWhiteSpace(credencial.PasswordNuevo))
                    {
                        string claveHash = credencial.PasswordNuevo;
                        string claveActualHash = ObjUsuario.CRU_Clave;

                        if (ObjUsuario.CRU_FormatoClave.Trim().Equals(FORMATO_CLAVE_HASHED, StringComparison.OrdinalIgnoreCase))
                        {
                            claveHash = COEncripcion.ObtieneHash(credencial.PasswordNuevo);
                        }

                        ObjUsuario.CRU_FechaUltimoCambioClave = DateTime.Now;
                        ObjUsuario.CRU_ClaveAnterior = claveActualHash;
                        ObjUsuario.CRU_Clave = claveHash;
                        ObjUsuario.CRU_DiasVencimiento = Convert.ToInt32(ParametrosFW.PAAdministrador.Instancia.ConsultarParametrosFramework("DiasVencimiento"));
                        ObjUsuario.CRU_FormatoClave = ObjUsuario.CRU_FormatoClave;
                        ObjUsuario.CRU_ClaveBloqueada = credencial.ClaveBloqueada;
                        SERepositorioAudit.MapeoAuditCambiarPassword(contexto, credencial.Usuario);
                        contexto.SaveChanges();

                        ObjUsuario.CRU_ClaveBloqueada = credencial.ClaveBloqueada;
                        contexto.SaveChanges();
                    }

                    // Se bloquea o desbloque la clave
                    ObjUsuario.CRU_ClaveBloqueada = credencial.ClaveBloqueada;
                    SERepositorioAudit.MapeoAuditCambiarPassword(contexto, credencial.Usuario);
                    contexto.SaveChanges();
                }
            }
        }

        #endregion Publicos

        #region Consultas

        /// <summary>
        /// Obtiene los usuarios internos activos
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SEAdminUsuario> ObtenerUsuariosInternosActivos()
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                return contexto.UsuarioInterno_VSEG.Where(r => r.USU_EsUsuarioInterno == true && r.USU_Estado == ConstantesFramework.ESTADO_ACTIVO)
                  .ToList()
                  .ConvertAll(r => new SEAdminUsuario()
                  {
                      Usuario = r.USU_IdUsuario,
                      Nombre = r.PEI_Nombre.Trim() + " " + r.PEI_PrimerApellido.Trim(),
                      Apellido1 = r.PEI_PrimerApellido,
                      Apellido2 = r.PEI_SegundoApellido == null ? string.Empty : r.PEI_SegundoApellido,
                      Identificacion = r.PEI_Identificacion
                  });
            }
        }

        /// <summary>
        /// Método para obtener la localidad de un usuario
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        public PALocalidadDC ObtenerLocalidadPorUsuario(string idUsuario)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                Localidad_PAR Localidad;
                UsuarioInterno_VSEG usuario = contexto.UsuarioInterno_VSEG.Where(us => us.USU_IdUsuario == idUsuario).FirstOrDefault();
                if (usuario == null)
                    return new PALocalidadDC();
                else
                {
                    Localidad = contexto.Localidad_PAR.Where(loc => loc.LOC_IdLocalidad == usuario.CES_IdMunicipio).FirstOrDefault();
                    return new PALocalidadDC { IdLocalidad = usuario.CES_IdMunicipio, Nombre = Localidad.LOC_Nombre };
                }
            }
        }

        /// <summary>
        /// Retorna los tipos de autenticación
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SETipoAutenticacion> ObtenerTiposAutenticacion()
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TiposAutenticacion_VSEG.ToList().ConvertAll(tipoAut => new SETipoAutenticacion() { Id = tipoAut.Id, Descripcion = tipoAut.Descripcion });
            }
        }

        /// <summary>
        /// Obtiene a los Cajeros Auxiliares de un Centro de Servicio.
        /// </summary>
        /// <param name="idCentroServicio">The id centro servicio.</param>
        /// <returns></returns>
        public List<SEUsuarioCentroServicioDC> ObtenerCajeroCentroServicio(long idCentroServicio, string idRol)
        {
            DataSet dsRes = new DataSet();
            SqlDataAdapter da;
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerCajerosPorPuntoYPerfil_SEG", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdCentroSvc", idCentroServicio));
                cmd.Parameters.Add(new SqlParameter("@IdRol", idRol));
                da = new SqlDataAdapter(cmd);
                da.Fill(dsRes);

                var cajeros = dsRes.Tables[0].AsEnumerable().ToList().ConvertAll<SEUsuarioCentroServicioDC>(r =>
                    {
                        SEUsuarioCentroServicioDC cajero = new SEUsuarioCentroServicioDC()
                        {
                            IdCaja = r.Field<int>("UCS_Caja"),
                            IdUsuario = r.Field<long>("UCS_IdCodigoUsuario"),
                            NombreCentroServicio = r.Field<string>("UCS_NombreCentroServicios"),
                            NombreCajero = String.Format("| {0} {1} {2}", r.Field<string>("PEI_Nombre"), r.Field<string>("PEI_PrimerApellido"), r.Field<string>("PEI_SegundoApellido")),
                            NumeroDocumento = r.Field<string>("PEI_Identificacion"),
                            Perfil = r.Field<string>("ROL_IdRol"),
                            Usuario = r.Field<string>("USU_IdUsuario")
                        };
                        return cajero;
                    });

                sqlConn.Close();
                return cajeros;
            }
        }

        /// <summary>
        /// Consulta las cajas con los usuarios de un punto trayendo tambien
        ///	las cajas no utilizadas
        /// </summary>
        /// <param name="idCentroSvc">id punto centro servicio</param>
        /// <returns>lista de cajas del punto centro servicio</returns>
        public List<SEUsuarioCentroServicioDC> ObtenerCajerosCajaPorPunto(long idCentroSvc)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                var ListCajas = contexto.paObtenerCajasYCajerosPorPunto_CAJ(idCentroSvc);

                if (ListCajas != null)
                {
                    return ListCajas.ToList().ConvertAll<SEUsuarioCentroServicioDC>(caj => new SEUsuarioCentroServicioDC()
                    {
                        IdCaja = caj.UCS_Caja,
                        IdUsuario = caj.UCS_IdCodigoUsuario.Value,
                        IdCentroServicio = caj.UCS_IdCentroServicios.Value,
                        NombreCentroServicio = caj.UCS_NombreCentroServicios,
                        NombreCajero = caj.nombreCompleto,
                        NumeroDocumento = caj.PEI_Identificacion,
                        Perfil = caj.ROL_IdRol
                    }).ToList();
                }
                else
                {
                    List<SEUsuarioCentroServicioDC> lista = new List<SEUsuarioCentroServicioDC>();
                    return lista;
                }
            }
        }

        #endregion Consultas

        #region Administración Roles-Menú-Acción

        /// <summary>
        /// /Obtiene los menus por rol asignados y sin asignar
        /// </summary>
        /// <param name="rol">Recibe rol para hacer las consultas</param>
        /// <returns>Retorna la lista de menús que estan asignados y los que estan disponibles</returns>
        public SEMenuRolAccion ObtenerMenuRol(SERol rol)
        {
            SEMenuRolAccion rolMenu = new SEMenuRolAccion() { IdRol = rol.IdRol };

            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                //
                var menusAsignados = contexto.MenuRol_SEG
                  .Where(menuRol => menuRol.MER_IdRol == rol.IdRol)
                  .Select(menuRol =>
                    new
                    {
                        menu = menuRol.Menu_SEG,
                        idMenuRol = menuRol.MER_IdMenuRol
                    });

                //Se asignan los menús asignados de acuerdo al rol
                rolMenu.MenusAsignados = menusAsignados
               .ToList()
               .ConvertAll<SEMenuAccion>(menu => new SEMenuAccion()
               {
                   IdMenuRol = menu.idMenuRol,
                   IdMenu = menu.menu.MEN_IdMenu,
                   Etiqueta = menu.menu.MEN_IdModulo + "-" + menu.menu.MEN_Etiqueta,
                   Acciones = contexto.MenuRolAccion_SEG.Where(menuId => menuId.MRA_IdMenuRol == menu.idMenuRol)
                   .OrderBy(acc => acc.MRA_IdAccion)
                   .Select(accion => accion.Accion_SEG)
                   .ToList()
                   .ConvertAll<SEAccion>(accion => new SEAccion()
                   {
                       IdAccion = accion.ACC_IdAccion,
                       DescripcionAccion = accion.ACC_Descripcion,
                       EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS
                   })
               });

                //Se asignan los menús disponible (aquellos que no estan asignados) de acuerdo al rol
                rolMenu.Menus = contexto.Menu_SEG
                   .Except(contexto.MenuRol_SEG
                            .Where(menuRol => menuRol.MER_IdRol == rol.IdRol)
                            .Select(menu => menu.Menu_SEG))
                   .ToList()
                   .ConvertAll<SEMenuAccion>(menuAccion => new SEMenuAccion()
                   {
                       IdMenu = menuAccion.MEN_IdMenu,
                       Etiqueta = menuAccion.MEN_IdModulo + "-" + menuAccion.MEN_Etiqueta
                   });

                return rolMenu;
            }
        }

        /// <summary>
        /// Obtener las acciones de la base de datos
        /// </summary>
        /// <returns></returns>
        public List<SEAccion> ObtenerAcciones()
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.Accion_SEG
                  .OrderBy(acc => acc.ACC_IdAccion)
                  .ToList()
                  .ConvertAll<SEAccion>(accion => new SEAccion()
                  {
                      IdAccion = accion.ACC_IdAccion,
                      DescripcionAccion = accion.ACC_Descripcion,
                      EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS
                  });
            }
        }

        /// <summary>
        /// Guarda el menú asignado al Rol
        /// </summary>
        /// <param name="rolMenuGuardar"></param>
        private void AdicionarMenuRol(SEMenuAccion menu, string idRol)
        {
            try
            {
                using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
                {
                    DateTime fecha = DateTime.Now;

                    contexto.MenuRol_SEG.Add(new MenuRol_SEG()
                    {
                        MER_IdMenu = menu.IdMenu,
                        MER_IdRol = idRol,
                        MER_FechaGrabacion = fecha,
                        MER_CreadoPor = ControllerContext.Current.Usuario
                    });
                    if (menu.Acciones.Count != 0)
                    {
                        AplicarCambiosAcciones(menu, contexto, "1");
                    }

                    // TODO ID: Se adiciona Auditoria a Administracion de Menu-Rol
                    contexto.MenuRolHis_SEG.Add(new MenuRolHis_SEG()
                    {
                        MER_IdAuditoria = 1,
                        MER_IdMenu = menu.IdMenu,
                        MER_IdRol = idRol,
                        MER_FechaGrabacion = fecha,
                        MER_CreadoPor = ControllerContext.Current.Usuario,

                        MER_CambiadoPor = ControllerContext.Current.Usuario,
                        MER_FechaCambio = fecha,
                        MER_TipoCambio = "Adicionado"
                    });

                    contexto.SaveChanges();
                    menu = null;
                }
            }
            catch (Exception )
            {
                throw;
            }
        }

        private void ModificarAcciones(SEMenuAccion menu)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                if (menu.Acciones.Count != 0)
                {
                    AplicarCambiosAcciones(menu, contexto, "0");
                    contexto.SaveChanges();
                    contexto.Dispose();
                    menu = null;
                }
            }
        }

        /// <summary>
        /// Aplica los cambios de las acciones de un menú
        /// </summary>
        /// <param name="menu"></param>
        /// <param name="contexto"></param>
        private static void AplicarCambiosAcciones(SEMenuAccion menu, EntidadesSeguridad contexto, string tipo)
        {
            DateTime fecha = DateTime.Now;

            //aplicar los cambios de las acciones del menú

            menu.Acciones.ForEach(acc =>
            {
                if (acc.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
                {
                    if (tipo == "0")
                    {
                        MenuRolAccion_SEG accionCambiada = new MenuRolAccion_SEG()
                        {
                            MRA_IdAccion = acc.IdAccion,
                            MRA_FechaGrabacion = fecha,
                            MRA_IdMenuRol = menu.IdMenuRol,
                            MRA_CreadoPor = ControllerContext.Current.Usuario
                        };
                        contexto.MenuRolAccion_SEG.Add(accionCambiada);
                    }
                    else
                        if (tipo == "1")
                    {
                        MenuRolAccion_SEG accionCambiada = new MenuRolAccion_SEG()
                        {
                            MRA_IdAccion = acc.IdAccion,
                            MRA_FechaGrabacion = fecha,
                            MRA_CreadoPor = ControllerContext.Current.Usuario
                        };
                        contexto.MenuRolAccion_SEG.Add(accionCambiada);
                    }
                }
                else if (acc.EstadoRegistro == EnumEstadoRegistro.BORRADO)
                {
                    MenuRolAccion_SEG accionBorrada = contexto.MenuRolAccion_SEG.SingleOrDefault(m => m.MRA_IdAccion == acc.IdAccion && m.MRA_IdMenuRol == menu.IdMenuRol);
                    if (accionBorrada != null)
                    {
                        contexto.MenuRolAccion_SEG.Remove(accionBorrada);
                    }
                }
            });
        }

        /// <summary>
        /// Elimina menus de roles
        /// </summary>
        /// <param name="rolMenuEliminar"></param>
        private void EliminarMenuRol(int idMenuRol)
        {
            DateTime fecha = DateTime.Now;

            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                MenuRol_SEG menu = contexto.MenuRol_SEG.Where(mr => mr.MER_IdMenuRol == idMenuRol).SingleOrDefault();
                if (menu == null) return;

                // TODO ID: Se adiciona Auditoria a Administracion de Menu-Rol
                contexto.MenuRolHis_SEG.Add(new MenuRolHis_SEG()
                {
                    MER_IdMenu = menu.MER_IdMenu,
                    MER_IdRol = menu.MER_IdRol,
                    MER_FechaGrabacion = fecha,
                    MER_CreadoPor = menu.MER_CreadoPor,

                    MER_CambiadoPor = ControllerContext.Current.Usuario,
                    MER_FechaCambio = fecha,
                    MER_TipoCambio = "Eliminado"
                });

                contexto.MenuRol_SEG.Remove(menu);

                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Transacción para guardar Rol Menu Accion
        /// </summary>
        /// <param name="accionAgregada"></param>
        /// <param name="accionEliminada"></param>
        public void ModificarMenuRolAccion(SEMenuRolAccionConsolidado menuRolAccion)
        {
            using (TransactionScope trans = new TransactionScope())
            {
                menuRolAccion.Menus.ForEach(menu =>
                {
                    if (menu.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
                    {
                        //Insertar menú y sus acciones
                        AdicionarMenuRol(menu, menuRolAccion.IdRol);
                    }
                    else if (menu.EstadoRegistro == EnumEstadoRegistro.SIN_CAMBIOS)
                    {
                        //Insertar y Eliminar las acciones modificadas
                        ModificarAcciones(menu);
                    }
                    else if (menu.EstadoRegistro == EnumEstadoRegistro.BORRADO)
                    {
                        //Borrar Acciones y el menú respectivo
                        ModificarAcciones(menu);
                        EliminarMenuRol(menu.IdMenuRol);
                    }
                });

                trans.Complete();
            }
        }

        #endregion Administración Roles-Menú-Acción

        #region Administración Usuarios

        /// <summary>
        /// Obtener los usuarios activos de la base de datos
        /// </summary>
        /// <param name="filtro">Recibe filtro</param>
        /// <param name="campoOrdenamiento">Recibe campo de ordenamiento</param>
        /// <param name="indicePagina">Recibe Indice de Página</param>
        /// <param name="registrosPorPagina">Recibe registros por página</param>
        /// <param name="ordenamientoAscendente">True o false para ordenamiento Ascendente</param>
        /// <param name="totalRegistros">Recibe el total de registros</param>
        /// <returns></returns>
        public IEnumerable<SEAdminUsuario> ObtenerUsuariosAdmin(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente)
        {
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                string nombre;
                string usuario;
                string Doc;
                string RACOL;
                string Estado;

                filtro.TryGetValue("PEI_Nombre", out nombre);
                filtro.TryGetValue("USU_IdUsuario", out usuario);
                filtro.TryGetValue("PEI_Identificacion", out Doc);
                filtro.TryGetValue("REA_Descripcion", out RACOL);
                filtro.TryGetValue("USU_Estado", out Estado);


                SqlCommand cmd = new SqlCommand("paObtenerUsrCtrSvcCargo_SEG", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PageIndex", indicePagina);
                cmd.Parameters.AddWithValue("@PageSize", registrosPorPagina);
                cmd.Parameters.AddWithValue("@FiltroUsuario", usuario);
                cmd.Parameters.AddWithValue("@FiltroIdentificacion", Doc);
                cmd.Parameters.AddWithValue("@FiltroRacol", RACOL);
                cmd.Parameters.AddWithValue("@FiltroNombre", nombre);
                cmd.Parameters.AddWithValue("@FiltroEstado", Estado);
                DataTable dt = new DataTable();
                sqlConn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                sqlConn.Close();

                var usuarios = dt.AsEnumerable().ToList().ConvertAll(r =>
                    {
                        SEAdminUsuario AdminUsuario = new SEAdminUsuario()
                        {
                            Usuario = r.Field<string>("USU_IdUsuario"),
                            Nombre = r.Field<string>("PEI_Nombre"),
                            Apellido1 = r.Field<string>("PEI_PrimerApellido"),
                            Apellido2 = r.Field<string>("PEI_SegundoApellido"),
                            Identificacion = r.Field<string>("PEI_Identificacion"),
                            Direccion = r.Field<string>("PEI_Direccion"),
                            Email = r.Field<string>("PEI_Email"),
                            Cargo = r.Field<string>("CAR_Descripcion"),
                            IdCargo = r.Field<int>("CAR_IdCargo"),
                            Regional = r.Field<string>("REA_Descripcion"),
                            IdRegional = r.Field<long>("PEI_IdRegionalAdm") == null ? 0 : (long)r.Field<long>("PEI_IdRegionalAdm"),
                            AutorizaCargaMasiva = r.Field<bool>("USU_AutorizaCargaMasiva"),
                            AutorizaCargaMasivaICA = r.Field<bool>("USU_AutorizaCargaMasivaICA"),
                            EsUsuarioInterno = r.Field<bool>("PersonaInterna"),
                            TipoUsuario = r.Field<string>("USU_TipoUsuario"),
                            TipoIdentificacion = r.Field<string>("PEI_IdTipoIdentificacion"),
                            Telefono = r.Field<string>("PEI_Telefono"),
                            Comentarios = r.Field<string>("USU_Comentarios"),
                            RequiereIdentificadorMaquina = r.Field<bool>("USU_RequiereIdMaquina"),
                            Estado = r.Field<string>("USU_Estado"),
                            Municipio = r.Field<string>("PEI_Municipio"),
                            IdCodigoUsuario = r.Field<long>("USP_IdCodigoUsuario"),
                            IdPersonaInterna = r.Field<long>("USP_IdPersonaInterna"),
                            ClaveBloqueada = Convert.ToBoolean(r.Field<int>("CRU_ClaveBloqueada")),
                            CiudadUsuario = new PALocalidadDC()
                            {
                                IdLocalidad = r.Field<string>("PEI_Municipio"),
                                Nombre = r.Field<string>("LOC_Nombre"),
                            },
                            PaisUsuario = ObtenerPaisPorLocalidad(r.Field<string>("PEI_Municipio")),
                            AplicaPAM = r.Field<bool>("USU_AplicaPAM")
                        };

                        AdminUsuario.CentrosDeServicioAutorizados = new ObservableCollection<SECentroServicio>();
                        SqlCommand cmdUsuarioCsv = new SqlCommand("paObtenerInfoUsuarioCentroServicios_SEG", sqlConn);
                        cmdUsuarioCsv.CommandType = CommandType.StoredProcedure;
                        cmdUsuarioCsv.Parameters.AddWithValue("@idCodigoUsuario", r.Field<long>("USP_IdCodigoUsuario"));
                        sqlConn.Open();
                        SqlDataReader reader = cmdUsuarioCsv.ExecuteReader();
                        while (reader.Read())
                        {
                            AdminUsuario.CentrosDeServicioAutorizados.Add(new SECentroServicio()
                            {
                                Id = Convert.ToInt64(reader["UCS_IdCentroServicios"]),
                                Descripcion = reader["UCS_NombreCentroServicios"].ToString(),
                                Caja = Convert.ToInt32(reader["UCS_Caja"]),
                                ImpresionPos = Convert.ToBoolean(reader["UCS_ImpresionPOS"])
                            });
                        }
                        sqlConn.Close();

                        AdminUsuario.GestionesAutorizadas = new ObservableCollection<SEGestion>();
                        SqlCommand cmdUsuarioGestion = new SqlCommand("paObtenerGestionesDeUnUsuario_SEG", sqlConn);
                        cmdUsuarioGestion.CommandType = CommandType.StoredProcedure;
                        cmdUsuarioGestion.Parameters.AddWithValue("@idCodigoUsuario", r.Field<long>("USP_IdCodigoUsuario"));
                        sqlConn.Open();
                        SqlDataReader readerUsuarioGestion = cmdUsuarioGestion.ExecuteReader();
                        while (readerUsuarioGestion.Read())
                        {
                            AdminUsuario.GestionesAutorizadas.Add(new SEGestion()
                            {
                                Id = Convert.ToInt64(readerUsuarioGestion["USG_CodigoGestion"]),
                                Descripcion = readerUsuarioGestion["USG_Descripcion"].ToString()
                            });
                        }
                        sqlConn.Close();

                        AdminUsuario.SucursalesAutorizadas = new ObservableCollection<SESucursal>();
                        SqlCommand cmdUsuarioSucursal = new SqlCommand("paObtenerSucursalesAsignadasaUnUsuario_SEG", sqlConn);
                        cmdUsuarioSucursal.CommandType = CommandType.StoredProcedure;
                        cmdUsuarioSucursal.Parameters.AddWithValue("@idCodigoUsuario", r.Field<long>("USP_IdCodigoUsuario"));
                        sqlConn.Open();
                        SqlDataReader readerSucursales = cmdUsuarioSucursal.ExecuteReader();
                        while (readerSucursales.Read())
                        {
                            AdminUsuario.SucursalesAutorizadas.Add(new SESucursal()
                            {
                                Id = Convert.ToInt32(readerSucursales["USS_IdSucursal"]),
                                Descripcion = readerSucursales["USS_Nombre"].ToString()
                            });
                        }
                        sqlConn.Close();
                        return AdminUsuario;

                    });
                return usuarios;
            }
        }

        /// <summary>
        /// metodo para Obtener el Pais
        /// por medio de una localidad
        /// </summary>
        /// <param name="idlocalidad"></param>
        /// <returns></returns>
        public PALocalidadDC ObtenerPaisPorLocalidad(string idlocalidad)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Localidades_VPAR localidadOrigen = contexto.Localidades_VPAR.Where(t => t.LOC_IdLocalidad == idlocalidad).Single();
                PALocalidadDC Pais = new PALocalidadDC();
                switch (localidadOrigen.LOC_IdTipo)
                {
                    case "1":
                        Pais.IdLocalidad = localidadOrigen.LOC_IdLocalidad;
                        Pais.Nombre = localidadOrigen.LOC_Nombre;
                        Pais.NombreAncestroPGrado = localidadOrigen.LOC_Nombre;
                        break;

                    case "2":
                        Pais.IdLocalidad = localidadOrigen.LOC_IdAncestroPrimerGrado;
                        Pais.Nombre = localidadOrigen.LOC_NombrePrimero;
                        Pais.NombreAncestroPGrado = localidadOrigen.LOC_Nombre;
                        break;

                    case "3":
                        Pais.IdLocalidad = localidadOrigen.LOC_IdAncestroSegundoGrado;
                        Pais.Nombre = localidadOrigen.LOC_NombreSegundo;
                        Pais.NombreAncestroPGrado = localidadOrigen.LOC_Nombre;
                        break;

                    default:
                        Pais.IdLocalidad = localidadOrigen.LOC_IdAncestroTercerGrado;
                        Pais.Nombre = localidadOrigen.LOC_NombreTercero;
                        Pais.NombreAncestroPGrado = localidadOrigen.LOC_Nombre;
                        break;
                }

                return Pais;
            }
        }

        /// <summary>
        /// Establece el estado INA al Usuario
        /// </summary>
        /// <param name="credencial">Recibe la información del uuario</param>
        public void EliminarUsuario(SEAdminUsuario credencial)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Usuario_SEG usuario = contexto.Usuario_SEG
                  .Where(r => r.USU_IdUsuario == credencial.Usuario)
                  .FirstOrDefault();
                DateTime fecha = DateTime.Now;
                usuario.USU_Estado = "INA";
                SERepositorioAudit.MapeoAuditUsuario(contexto);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Adiciona una contraseña
        /// </summary>
        /// <param name="credencial"></param>
        public void AdicionarPassword(SECredencialUsuario credencial)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                string clave = "";
                clave = COEncripcion.ObtieneHash(credencial.PasswordNuevo);

                DateTime fecha = DateTime.Now;

                Usuario_SEG idUsuario = ObtenerUsuarioPorIdUsuario(credencial.Usuario);

                CredencialUsuario_SEG usuario = new CredencialUsuario_SEG()
                {
                    CRU_IdCodigoUsuario = idUsuario.USU_IdCodigoUsuario,
                    CRU_Clave = clave,
                    CRU_FormatoClave = FORMATO_CLAVE_HASHED,
                    CRU_ClaveBloqueada = false,
                    CRU_ClaveAnterior = null,
                    CRU_FechaUltimoCambioClave = fecha,
                    CRU_CantIntentosFallidosClave = 0,
                    CRU_DiasVencimiento = Convert.ToInt32(ParametrosFW.PAAdministrador.Instancia.ConsultarParametrosFramework("DiasVencimiento")),
                    CRU_FechaGrabacion = fecha,
                    CUR_CreadoPor = ControllerContext.Current.Usuario
                };
                contexto.CredencialUsuario_SEG.Add(usuario);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Adiciona un Persona Interna
        /// </summary>
        /// <param name="credencial">Recibe la información del usuario</param>
        public long AdicionarPersonaInterna(SEAdminUsuario credencial)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                long idPersonaInterna;
                PersonaInterna_PAR per = contexto.PersonaInterna_PAR.FirstOrDefault(r => r.PEI_Identificacion == credencial.Identificacion);

                if (per == null)
                {
                    PersonaInterna_PAR personaInterna = new PersonaInterna_PAR()
                    {
                        PEI_Nombre = credencial.Nombre,
                        PEI_PrimerApellido = credencial.Apellido1,
                        PEI_SegundoApellido = credencial.Apellido2,
                        PEI_IdTipoIdentificacion = credencial.TipoIdentificacion,
                        PEI_Identificacion = credencial.Identificacion,
                        PEI_Direccion = credencial.Direccion,
                        PEI_Telefono = credencial.Telefono,
                        PEI_Email = credencial.Email,
                        PEI_IdRegionalAdm = credencial.IdRegional,
                        PEI_IdCargo = credencial.IdCargo,
                        PEI_Municipio = credencial.CiudadUsuario.IdLocalidad,
                        PEI_Comentarios = credencial.Comentarios,
                        PEI_CreadoPor = ControllerContext.Current.Usuario,
                        PEI_FechaGrabacion = DateTime.Now
                    };
                    contexto.PersonaInterna_PAR.Add(personaInterna);
                    this.AuditPersonaInterna(contexto, personaInterna, "Add");
                    contexto.SaveChanges();
                    idPersonaInterna = personaInterna.PEI_IdPersonaInterna;
                }
                else
                {
                    per.PEI_Nombre = credencial.Nombre;
                    per.PEI_PrimerApellido = credencial.Apellido1;
                    per.PEI_SegundoApellido = credencial.Apellido2;
                    per.PEI_IdTipoIdentificacion = credencial.TipoIdentificacion;
                    per.PEI_Direccion = credencial.Direccion;
                    per.PEI_Telefono = credencial.Telefono;
                    per.PEI_Email = credencial.Email;
                    per.PEI_IdRegionalAdm = credencial.IdRegional;
                    per.PEI_IdCargo = credencial.IdCargo;
                    per.PEI_Municipio = credencial.CiudadUsuario.IdLocalidad;
                    per.PEI_Comentarios = credencial.Comentarios;
                    per.PEI_CreadoPor = ControllerContext.Current.Usuario;
                    per.PEI_FechaGrabacion = DateTime.Now;
                    idPersonaInterna = per.PEI_IdPersonaInterna;
                }

                return idPersonaInterna;
            }
        }

        /// <summary>
        /// Adiciona un Persona Externa
        /// </summary>
        /// <param name="credencial">Recibe la información del usuario</param>
        public long AdicionarPersonaExterna(SEAdminUsuario credencial)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PersonaExterna_PAR personaExterna = new PersonaExterna_PAR()
                {
                    PEE_DigitoVerificacion = "0",
                    PEE_Direccion = credencial.Direccion,
                    PEE_FechaExpedicionDocumento = DateTime.Now,
                    PEE_Identificacion = credencial.Identificacion,
                    PEE_IdTipoIdentificacion = credencial.TipoIdentificacion,
                    PEE_Municipio = credencial.CiudadUsuario.IdLocalidad,
                    PEE_NumeroCelular = string.Empty,
                    PEE_PrimerApellido = credencial.Apellido1,
                    PEE_PrimerNombre = credencial.Nombre,
                    PEE_SegundoApellido = credencial.Apellido2,
                    PEE_SegundoNombre = string.Empty,
                    PEE_Telefono = credencial.Telefono,
                    PEE_CreadoPor = ControllerContext.Current.Usuario,
                    PEE_FechaGrabacion = DateTime.Now
                };
                contexto.PersonaExterna_PAR.Add(personaExterna);
                contexto.SaveChanges();
                return personaExterna.PEE_IdPersonaExterna;
            }
        }

        /// <summary>
        /// Adiciona el Usuario a la Tabla Usuario_Seg
        /// </summary>
        /// <param name="credencial"></param>
        /// <returns></returns>
        public long AdicionarUsuarioSeg(SEAdminUsuario credencial)
        {
             using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paInsertarUsuario_SEG", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdUsuario", credencial.Usuario);
                cmd.Parameters.AddWithValue("@Estado", credencial.Estado);
                cmd.Parameters.AddWithValue("@TipoUsuario", credencial.TipoUsuario);
                cmd.Parameters.AddWithValue("@RequiereIdMaquina", credencial.RequiereIdentificadorMaquina);
                cmd.Parameters.AddWithValue("@Comentarios", credencial.Comentarios);
                cmd.Parameters.AddWithValue("@EsUsuarioInterno", credencial.EsUsuarioInterno);
                cmd.Parameters.AddWithValue("@FechaGrabacion", DateTime.Now);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);
                cmd.Parameters.AddWithValue("@AutorizaCargaMasiva", credencial.AutorizaCargaMasiva);
                cmd.Parameters.AddWithValue("@AutorizaCargaMasivaICA", credencial.AutorizaCargaMasivaICA);
                cmd.Parameters.AddWithValue("@AplicaPAM", credencial.AplicaPAM);

                sqlConn.Open();
                long idCodigoUsuario = Convert.ToInt64(cmd.ExecuteScalar());
                sqlConn.Close();
                return idCodigoUsuario;
            }
        }

        /// <summary>
        /// Adiciona la relacion de entre
        /// un usuario y la persona Interna
        /// </summary>
        /// <param name="credencial"></param>
        public void AdicionarUsuarioPersonaInterna(long idCodigoUsuario, long IdPersonaInterna)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                UsuarioPersonaInterna_SEG usuarioPersonaInterna = new UsuarioPersonaInterna_SEG()
                {
                    USP_IdPersonaInterna = IdPersonaInterna,
                    USP_IdCodigoUsuario = idCodigoUsuario,
                    USP_CreadoPor = ControllerContext.Current.Usuario,
                    USP_FechaGrabacion = DateTime.Now
                };
                contexto.UsuarioPersonaInterna_SEG.Add(usuarioPersonaInterna);
                AuditUsuarioPersonaInterna(contexto, usuarioPersonaInterna, "CREADO");
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Adiciona la relacion entre
        /// un Usuario y la persona Externa
        /// </summary>
        /// <param name="idCodigoUsuario"></param>
        /// <param name="IdPersonaExterna"></param>
        public void AdicionarUsuarioPersonaExterna(long idCodigoUsuario, long IdPersonaExterna)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                UsuarioPersonaExterna_SEG usuarioPersonaExterna = new UsuarioPersonaExterna_SEG()
                {
                    USP_IdPersonaExterna = IdPersonaExterna,
                    USP_IdCodigoUsuario = idCodigoUsuario,
                    USP_CreadoPor = ControllerContext.Current.Usuario,
                    USP_FechaGrabacion = DateTime.Now
                };
                contexto.UsuarioPersonaExterna_SEG.Add(usuarioPersonaExterna);
                AuditUsuarioPersonaExterna(contexto, usuarioPersonaExterna, "CREADO");
                contexto.SaveChanges();
            }
        }

        public void GestionarCredencialUsuario(SEAdminUsuario credencial)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                if (!string.IsNullOrWhiteSpace(credencial.PasswordNuevo))
                {
                    CredencialUsuario_SEG usuario = contexto.CredencialUsuario_SEG.FirstOrDefault(c => c.CRU_IdCodigoUsuario == credencial.IdCodigoUsuario);
                    if (usuario == null)
                    {
                        CredencialUsuario_SEG usuarioPassword = new CredencialUsuario_SEG()
                        {
                            CRU_IdCodigoUsuario = credencial.IdCodigoUsuario,
                            CRU_Clave = COEncripcion.ObtieneHash(credencial.PasswordNuevo),
                            CRU_FormatoClave = FORMATO_CLAVE_HASHED,
                            CRU_ClaveBloqueada = credencial.ClaveBloqueada,
                            CRU_ClaveAnterior = null,
                            CRU_FechaUltimoCambioClave = DateTime.Now,
                            CRU_CantIntentosFallidosClave = 0,
                            CRU_DiasVencimiento = Convert.ToInt32(ParametrosFW.PAAdministrador.Instancia.ConsultarParametrosFramework("DiasVencimiento")),
                            CRU_FechaGrabacion = DateTime.Now,
                            CUR_CreadoPor = ControllerContext.Current.Usuario
                        };
                        contexto.CredencialUsuario_SEG.Add(usuarioPassword);
                    }
                    else
                    {
                        usuario.CRU_CantIntentosFallidosClave = 0;
                        usuario.CRU_Clave = COEncripcion.ObtieneHash(credencial.PasswordNuevo);
                        usuario.CRU_ClaveAnterior = null;
                        usuario.CRU_ClaveBloqueada = credencial.ClaveBloqueada;
                        usuario.CRU_DiasVencimiento = Convert.ToInt32(ParametrosFW.PAAdministrador.Instancia.ConsultarParametrosFramework("DiasVencimiento"));
                        usuario.CRU_FechaGrabacion = DateTime.Now;
                        usuario.CRU_FechaUltimoCambioClave = DateTime.Now;
                        usuario.CRU_FormatoClave = FORMATO_CLAVE_HASHED;
                        usuario.CUR_CreadoPor = ControllerContext.Current.Usuario;
                    }
                    contexto.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Edita la informacion de un usuario
        /// </summary>
        /// <param name="credencial">Recibe la información del usuario</param>
        public void EditarUsuario(SEAdminUsuario credencial)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                //hallar el usuario
                Usuario_SEG usuario = contexto.Usuario_SEG
                  .Where(r => r.USU_IdUsuario == credencial.Usuario)
                  .SingleOrDefault();

                if (usuario == null)
                {
                    ControllerException excepcion = new ControllerException(ConstantesFramework.MODULO_FW_SEGURIDAD, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }

                usuario.USU_AutorizaCargaMasiva = credencial.AutorizaCargaMasiva;
                usuario.USU_AutorizaCargaMasivaICA = credencial.AutorizaCargaMasivaICA;
                usuario.USU_Comentarios = credencial.Comentarios;
                usuario.USU_Estado = credencial.Estado;
                usuario.USU_RequiereIdMaquina = credencial.RequiereIdentificadorMaquina;
                this.AuditUsuario(contexto, usuario);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Edita los Datos de la Persona Externa
        /// por accion de asignacion del Usuario
        /// </summary>
        /// <param name="credencial"></param>
        public void EditarPersonaExternaPorUsuario(SEAdminUsuario credencial)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                //hallar los datos demográficos del usuario
                PersonaExterna_PAR personaUsuario = contexto.PersonaExterna_PAR
                  .Where(r => r.PEE_Identificacion == credencial.Identificacion)
                  .SingleOrDefault();

                long idPersonaExterna = 0;

                if (personaUsuario != null)
                {
                    personaUsuario.PEE_Direccion = credencial.Direccion;
                    personaUsuario.PEE_Identificacion = credencial.Identificacion;
                    personaUsuario.PEE_Municipio = credencial.Municipio;
                    personaUsuario.PEE_PrimerApellido = credencial.Apellido1;
                    personaUsuario.PEE_PrimerNombre = credencial.Nombre;
                    personaUsuario.PEE_SegundoApellido = credencial.Apellido2;
                    personaUsuario.PEE_Telefono = credencial.Telefono;
                    personaUsuario.PEE_IdTipoIdentificacion = credencial.TipoIdentificacion;
                    personaUsuario.PEE_Email = credencial.Email;
                    personaUsuario.PEE_SegundoNombre = string.Empty;
                    idPersonaExterna = personaUsuario.PEE_IdPersonaExterna;
                }
                else
                {
                    credencial.IdPersonaInterna = AdicionarPersonaExterna(credencial);
                    idPersonaExterna = credencial.IdPersonaInterna;
                }
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Elimino relacion de usuario persona Interna
        /// por cambio de permisos en usuario
        /// </summary>
        /// <param name="idCodigoUsuario"></param>
        public void BorrarUsuarioPersonaInternaXUsuario(long idCodigoUsuario)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                UsuarioPersonaInterna_SEG usuarioInterno = contexto.UsuarioPersonaInterna_SEG
                                                                .FirstOrDefault(user => user.USP_IdCodigoUsuario == idCodigoUsuario);
                if (usuarioInterno != null)
                {
                    AuditUsuarioPersonaInterna(contexto, usuarioInterno, "ELIMINAR");
                    contexto.UsuarioPersonaInterna_SEG.Remove(usuarioInterno);
                    contexto.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Elimino relacion de usuario persona Externa
        /// por cambio de permisos en usuario
        /// </summary>
        /// <param name="idCodigoUsuario"></param>
        public void BorrarUsuarioPersonaExternaXUsuario(long idCodigoUsuario)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                UsuarioPersonaExterna_SEG usuarioExterno = contexto.UsuarioPersonaExterna_SEG
                                                                .FirstOrDefault(user => user.USP_IdCodigoUsuario == idCodigoUsuario);
                if (usuarioExterno != null)
                {
                    AuditUsuarioPersonaExterna(contexto, usuarioExterno, "ELIMINAR");
                    contexto.UsuarioPersonaExterna_SEG.Remove(usuarioExterno);
                    contexto.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Elimino relacion de usuario persona Interna
        /// por cambio de permisos en usuario
        /// </summary>
        /// <param name="idCodigoUsuario"></param>
        public void BorrarUsuarioPersonaInternaXIdPersona(long idPersona)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                UsuarioPersonaInterna_SEG usuarioInterno = contexto.UsuarioPersonaInterna_SEG
                                                                .FirstOrDefault(user => user.USP_IdPersonaInterna == idPersona);
                if (usuarioInterno != null)
                {
                    AuditUsuarioPersonaInterna(contexto, usuarioInterno, "MODIFICADO");
                    contexto.UsuarioPersonaInterna_SEG.Remove(usuarioInterno);
                    contexto.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Elimino relacion de usuario persona Interna
        /// por cambio de permisos en usuario
        /// </summary>
        /// <param name="idCodigoUsuario"></param>
        public void BorrarUsuarioPersonaExternaXIdPersona(long idPersona)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                UsuarioPersonaExterna_SEG usuarioExterno = contexto.UsuarioPersonaExterna_SEG
                                                                .FirstOrDefault(user => user.USP_IdPersonaExterna == idPersona);
                if (usuarioExterno != null)
                {
                    AuditUsuarioPersonaExterna(contexto, usuarioExterno, "ELIMINAR");
                    contexto.UsuarioPersonaExterna_SEG.Remove(usuarioExterno);
                    contexto.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Edita los Datos de la Persona Interna por accion de
        /// asignacion del Usuario
        /// </summary>
        /// <param name="credencial"></param>
        public void EditarPersonaInternaPorUsuario(SEAdminUsuario credencial)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                long idPersonaInterna = 0;

                PersonaInterna_PAR personaUsuario = contexto.PersonaInterna_PAR
                    .FirstOrDefault(r => r.PEI_Identificacion == credencial.Identificacion);

                credencial.IdPersonaInterna = personaUsuario.PEI_IdPersonaInterna;

                if (personaUsuario != null)
                {
                    personaUsuario.PEI_Nombre = credencial.Nombre;
                    personaUsuario.PEI_PrimerApellido = credencial.Apellido1;
                    personaUsuario.PEI_SegundoApellido = credencial.Apellido2;
                    personaUsuario.PEI_Identificacion = credencial.Identificacion;
                    personaUsuario.PEI_IdTipoIdentificacion = credencial.TipoIdentificacion;
                    personaUsuario.PEI_IdRegionalAdm = credencial.IdRegional;
                    personaUsuario.PEI_IdCargo = credencial.IdCargo;
                    personaUsuario.PEI_Direccion = credencial.Direccion;
                    personaUsuario.PEI_Email = credencial.Email;
                    personaUsuario.PEI_Telefono = credencial.Telefono;
                    personaUsuario.PEI_Comentarios = credencial.Comentarios;
                    personaUsuario.PEI_Municipio = credencial.CiudadUsuario.IdLocalidad;
                    idPersonaInterna = personaUsuario.PEI_IdPersonaInterna;
                    this.AuditPersonaInterna(contexto, personaUsuario, "MODIFICADO");
                    contexto.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Guarda Auditoria de Usuarios
        /// </summary>
        /// <param name="contexto"></param>
        private void AuditUsuario(EntidadesSeguridad contexto, Usuario_SEG usuario)
        {
            UsuarioHistorico_SEG us = new UsuarioHistorico_SEG()
            {
                USU_IdUsuario = usuario.USU_IdUsuario,
                USU_IdCodigoUsuario = usuario.USU_IdCodigoUsuario,
                USU_TipoUsuario = usuario.USU_TipoUsuario,
                USU_RequiereIdMaquina = usuario.USU_RequiereIdMaquina,
                USU_Comentarios = usuario.USU_Comentarios,
                USU_FechaGrabacion = usuario.USU_FechaGrabacion,
                USU_CreadoPor = usuario.USU_CreadoPor,
                USU_FechaCambio = DateTime.Now,
                USU_CambiadoPor = ControllerContext.Current.Usuario,
                USU_TipoCambio = "MODIFICADO",
                USU_Estado = usuario.USU_Estado,
                USU_AutorizaCargaMasiva = usuario.USU_AutorizaCargaMasiva,
                USU_AutorizaCargaMasivaICA = usuario.USU_AutorizaCargaMasivaICA,
                USU_EsCajeroPpal = usuario.USU_EsCajeroPpal,
                USU_EsUsuarioInterno = usuario.USU_EsUsuarioInterno
            };
            contexto.UsuarioHistorico_SEG.Add(us);
            contexto.SaveChanges();
        }

        /// <summary>
        /// Guarda la Auditoria de los Usuarios PersonaInterna
        /// </summary>
        private void AuditUsuarioPersonaInterna(EntidadesSeguridad contexto, UsuarioPersonaInterna_SEG usuario, string tipoCambio)
        {
            UsuarioPersonaInternaHist_SEG userPersonInter = new UsuarioPersonaInternaHist_SEG()
            {
                USP_IdCodigoUsuario = usuario.USP_IdCodigoUsuario,
                USP_IdPersonaInterna = usuario.USP_IdPersonaInterna,
                USP_CambiadoPor = ControllerContext.Current.Usuario,
                USP_CreadoPor = usuario.USP_CreadoPor,
                USP_FechaGrabacion = usuario.USP_FechaGrabacion,
                USP_TipoCambio = tipoCambio,
                USP_FechaCambio = DateTime.Now
            };
            contexto.UsuarioPersonaInternaHist_SEG.Add(userPersonInter);
            contexto.SaveChanges();
        }

        /// <summary>
        /// Guarda la Auditoria de los
        /// usuarios de la PersonaExterna
        /// </summary>
        /// <param name="contexto"></param>
        /// <param name="usuario"></param>
        /// <param name="tipoCambio"></param>
        private void AuditUsuarioPersonaExterna(EntidadesSeguridad contexto, UsuarioPersonaExterna_SEG usuario, string tipoCambio)
        {
            UsuarioPersonaExternaHist_SEG userPersonExter = new UsuarioPersonaExternaHist_SEG()
            {
                USP_IdCodigoUsuario = usuario.USP_IdCodigoUsuario,
                USP_IdPersonaExterna = usuario.USP_IdPersonaExterna,
                USP_CambiadoPor = ControllerContext.Current.Usuario,
                USP_CreadoPor = usuario.USP_CreadoPor,
                USP_FechaGrabacion = usuario.USP_FechaGrabacion,
                USP_TipoCambio = tipoCambio,
                USP_FechaCambio = DateTime.Now,
            };
            contexto.UsuarioPersonaExternaHist_SEG.Add(userPersonExter);
            contexto.SaveChanges();
        }

        /// <summary>
        /// Guarda Auditoria de Usuarios
        /// </summary>
        /// <param name="contexto"></param>
        private void AuditPersonaInterna(EntidadesSeguridad contexto, PersonaInterna_PAR persona, string tipoCambio)
        {
            PersonaInternaHistorico_PAR per = new PersonaInternaHistorico_PAR()
            {
                PEI_IdPersonaInterna = persona.PEI_IdPersonaInterna,
                PEI_Direccion = persona.PEI_Direccion,
                PEI_Email = persona.PEI_Email,
                PEI_Comentarios = persona.PEI_Comentarios,
                PEI_Nombre = persona.PEI_Nombre,
                PEI_PrimerApellido = persona.PEI_PrimerApellido,
                PEI_SegundoApellido = persona.PEI_SegundoApellido,
                PEI_CreadoPor = persona.PEI_CreadoPor,
                PEI_FechaGrabacion = persona.PEI_FechaGrabacion,
                PEI_IdCargo = persona.PEI_IdCargo,
                PEI_Identificacion = persona.PEI_Identificacion,
                PEI_IdTipoIdentificacion = persona.PEI_IdTipoIdentificacion,
                PEI_IdRegionalAdm = persona.PEI_IdRegionalAdm,
                PEI_Municipio = persona.PEI_Municipio,
                PEI_Telefono = persona.PEI_Telefono,
                PEI_FechaCambio = DateTime.Now,
                PEI_CambiadoPor = ControllerContext.Current.Usuario,
                PEI_TipoCambio = tipoCambio,
            };
            contexto.PersonaInternaHistorico_PAR.Add(per);
            contexto.SaveChanges();
        }

        /// <summary>
        /// Modifica un usuario de la base de datos
        /// </summary>
        /// <param name="credencial"></param>
        public void ModificarUsuarioBD(SEAdminUsuario credencial, bool esModificacion)
        {
            EditarUsuario(credencial);
            ResetearPassword(credencial);
        }

        /// <summary>
        /// Valida si una persona interna existe
        /// </summary>
        /// <param name="idTipoIden"></param>
        /// <param name="numIdentifi"></param>
        /// <returns></returns>
        public bool ValidarPersonaInternaExiste(string idTipoIden, string numIdentifi)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PersonaInterna_PAR persona = contexto.PersonaInterna_PAR.Where(p => p.PEI_IdTipoIdentificacion == idTipoIden && p.PEI_Identificacion == numIdentifi).FirstOrDefault();
                if (persona != null)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Valida si una persona externa existe
        /// </summary>
        /// <param name="idTipoIden"></param>
        /// <param name="numIdentifi"></param>
        /// <returns></returns>
        public bool ValidarPersonaExternaExiste(string idTipoIden, string numIdentifi)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PersonaExterna_PAR persona = contexto.PersonaExterna_PAR.Where(p => p.PEE_IdTipoIdentificacion == idTipoIden && p.PEE_Identificacion == numIdentifi).FirstOrDefault();
                if (persona != null)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// valida si existe el Usuario
        /// </summary>
        /// <param name="idUsuario">Usuario</param>
        /// <returns>true si Existe, False si no</returns>
        public SEUsuarioDC ObtenerUsuario(string idUsuario)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Usuario_SEG usuario = contexto.Usuario_SEG.FirstOrDefault(use => use.USU_IdUsuario == idUsuario);
                SEUsuarioDC user;

                if (usuario != null)
                {
                    user = new SEUsuarioDC()
                    {
                        IdCodigoUsuario = usuario.USU_IdCodigoUsuario,
                        Usuario = usuario.USU_IdUsuario,
                        EstadoUsuario = usuario.USU_Estado,
                        UsuarioInterno = usuario.USU_EsUsuarioInterno,
                    };

                    return user;
                }
                else
                    return null;
            }
        }

        /// <summary>
        /// Obtiene una lista de los tipos de identificación
        /// </summary>
        /// <returns>Retorna una lista con los tipos de identifiación</returns>
        public List<SETipoIdentificacion> ObtenerTiposIdentificacion()
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TipoIdentificacion_PAR.ToList().ConvertAll<SETipoIdentificacion>(tipo => new SETipoIdentificacion()
                {
                    IdTipoIdentificacion = tipo.TII_IdTipoIdentificacion,
                    DescripcionIdentificacion = tipo.TII_Descripcion
                });
            }
        }

        /// <summary>
        /// Obtiene una lista con las Regionales
        /// </summary>
        /// <returns></returns>
        public List<SERegional> ObtenerRegionales()
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.RegionalAdministrativa_PUA.ToList().ConvertAll<SERegional>(reg => new SERegional()
                {
                    IdRegional = reg.REA_IdRegionalAdm,
                    DescripcionRegional = reg.REA_Descripcion
                });
            }
        }

        public List<SECentroServicio> ObtenerCentroServicioPASPorRacol(long idRacol)
        {
            List<SECentroServicio> lstCentrosServicioPAS = new List<SECentroServicio>();
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerAgenciasPuntosPorRacol_PUA", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idRegional", idRacol);
                cmd.Parameters.AddWithValue("@tipoAgencia", ConstantesFramework.TIPO_CENTRO_SERVICIO_AGENCIA);
                cmd.Parameters.AddWithValue("@tipoPunto", ConstantesFramework.TIPO_CENTRO_SERVICIO_PUNTO);
                cmd.Parameters.AddWithValue("@estado", ConstantesFramework.ESTADO_ACTIVO);
                cmd.Parameters.AddWithValue("@aplicaPAM", 1);
                sqlConn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    SECentroServicio centroSErvicioPAS = new SECentroServicio();
                    centroSErvicioPAS.Id = Convert.ToInt64(reader["CES_IdCentroServicios"]);
                    centroSErvicioPAS.Descripcion = reader["CES_Nombre"].ToString();
                    lstCentrosServicioPAS.Add(centroSErvicioPAS);
                }
                sqlConn.Close();
            }
            return lstCentrosServicioPAS;
        }

        public List<SEClienteCredito> ObtenerClientesCredito()
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ClienteCredito_CLI.Where(cli => cli.CLI_Estado == "ACT").ToList().ConvertAll<SEClienteCredito>(cli => new SEClienteCredito()
                {
                    Id = cli.CLI_IdCliente,
                    Descripcion = cli.CLI_RazonSocial
                });
            }
        }

        public List<SESucursal> ObtenerSucursalesXClientesCredito(int idCliente)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.Sucursal_CLI.Where(suc => suc.SUC_ClienteCredito == idCliente).ToList().ConvertAll<SESucursal>(suc => new SESucursal()
                {
                    Id = suc.SUC_IdSucursal,
                    Descripcion = suc.SUC_Nombre
                });
            }
        }

        public List<SECasaMatriz> ObtenerCasasMatriz()
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.CasaMatriz_ARE.ToList().ConvertAll<SECasaMatriz>(cas => new SECasaMatriz()
                {
                    Id = cas.CAM_IdCasaMatriz,
                    Descripcion = cas.CAM_Nombre
                });
            }
        }

        public List<SEMacroproceso> ObtenerMacroProcesos(short idCasaMatriz)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.MacroProceso_ARE.Where(mac => mac.MAP_IdCasaMatriz == idCasaMatriz).ToList().ConvertAll<SEMacroproceso>(mac => new SEMacroproceso()
                {
                    Id = mac.MAP_IdMacroProceso,
                    Descripcion = mac.MAP_Descripcion
                });
            }
        }

        public List<SEGestion> ObtenerGestiones(string idMacroProceso)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.Gestion_ARE.Where(ges => ges.GES_IdMacroProceso == idMacroProceso).ToList().ConvertAll<SEGestion>(ges => new SEGestion()
                {
                    Id = ges.GES_CodigoGestion,
                    Descripcion = ges.GES_Descripcion
                });
            }
        }

        public List<SECentroServicio> ObtenerCentrosDeServicioxCiudad(string idCiudad)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.CentroServicios_PUA.Where(cen => cen.CES_IdMunicipio == idCiudad && cen.CES_Estado == "ACT").ToList().ConvertAll<SECentroServicio>(cen => new SECentroServicio()
                {
                    Id = cen.CES_IdCentroServicios,
                    Descripcion = cen.CES_Nombre
                });
            }
        }

        /// <summary>
        /// se consulta la persona por la identificacion
        /// </summary>
        /// <param name="identificacion"></param>
        /// <returns>datos del usuario encontrado</returns>
        public SEPersonaInternaDC ObtenerPersonaInternaPorIdentificacion(string identificacion)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                SEPersonaInternaDC infoUsuario = contexto.PersonaInterna_PAR
                                        .Where(person => person.PEI_Identificacion == identificacion)
                                        .ToList()
                                        .ConvertAll<SEPersonaInternaDC>(per => new SEPersonaInternaDC()
                                        {
                                            Nombre = per.PEI_Nombre,
                                            PrimerApellido = per.PEI_PrimerApellido,
                                            SegundoApellido = per.PEI_SegundoApellido,
                                            Identificacion = per.PEI_Identificacion,
                                            IdTipoIdentificacion = per.PEI_IdTipoIdentificacion,
                                            NombreCompleto = string.Format("{0} {1} {2}", per.PEI_Nombre, per.PEI_PrimerApellido, per.PEI_SegundoApellido),
                                            IdRegional = per.PEI_IdRegionalAdm,
                                            Comentarios = per.PEI_Comentarios,
                                            Email = per.PEI_Email,
                                            Direccion = per.PEI_Direccion,
                                            Telefono = per.PEI_Telefono,
                                            IdCargo = per.PEI_IdCargo,
                                            Municipio = per.PEI_Municipio,
                                            IdPersonaInterna = per.PEI_IdPersonaInterna
                                        }).FirstOrDefault();

                return infoUsuario;
            }
        }

        /// <summary>
        /// se consulta la persona por la identificacion
        /// </summary>
        /// <param name="identificacion">doc de Identificacion</param>
        /// <returns>datos del Usuario Encontrado</returns>
        public SEPersonaInternaDC ObtenerPersonaExternaPorIdentificacion(string identificacion)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                SEPersonaInternaDC infoUsuario = contexto.PersonaExterna_PAR
                                        .Where(person => person.PEE_Identificacion == identificacion)
                                        .ToList()
                                        .ConvertAll<SEPersonaInternaDC>(per => new SEPersonaInternaDC()
                                        {
                                            Nombre = string.Format("{0} {1}", per.PEE_PrimerNombre, per.PEE_SegundoNombre),
                                            PrimerApellido = per.PEE_PrimerApellido,
                                            SegundoApellido = per.PEE_SegundoApellido,
                                            Identificacion = per.PEE_Identificacion,
                                            IdTipoIdentificacion = per.PEE_IdTipoIdentificacion,
                                            NombreCompleto = string.Format("{0} {1} {2} {3}", per.PEE_PrimerNombre, per.PEE_SegundoNombre,
                                                                                                per.PEE_PrimerApellido, per.PEE_SegundoApellido),
                                            IdRegional = 0,
                                            Comentarios = string.Empty,
                                            Email = string.Empty,
                                            Direccion = per.PEE_Direccion,
                                            Telefono = per.PEE_Telefono,
                                            IdCargo = 0,
                                            Municipio = per.PEE_Municipio,
                                            IdPersonaInterna = per.PEE_IdPersonaExterna
                                        }).FirstOrDefault();

                return infoUsuario;
            }
        }

        /// <summary>
        /// Autoriza el ingreso de un usuaario a través de un centro de servicio específico
        /// </summary>
        /// <param name="centroServicio">Centro de servioc</param>
        /// <param name="codigoUsuario">Codigo del usuario autorizado</param>
        public void AutorizarCentroServicio(SECentroServicio centroServicio, long codigoUsuario)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                UsuarioCentroServicio_SEG usuCS = new UsuarioCentroServicio_SEG()
                {
                    UCS_Caja = centroServicio.Caja,
                    UCS_CreadoPor = ControllerContext.Current.Usuario,
                    UCS_FechaGrabacion = DateTime.Now,
                    UCS_IdCentroServicios = centroServicio.Id,
                    UCS_IdCodigoUsuario = codigoUsuario,
                    UCS_NombreCentroServicios = centroServicio.Descripcion,
                    UCS_ImpresionPOS = centroServicio.ImpresionPos
                };

                contexto.UsuarioCentroServicio_SEG.Add(usuCS);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Desautoriza el ingreso de un usuario al sistema a través de un centro de servicio específico
        /// </summary>
        /// <param name="centroServicio">Centro de servicio</param>
        /// <param name="codigoUsuario">Codigo del usuario</param>
        public void DesautorizarCentrosServicio(long codigoUsuario)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                foreach (UsuarioCentroServicio_SEG usuCS in contexto.UsuarioCentroServicio_SEG.Where(us => us.UCS_IdCodigoUsuario == codigoUsuario).ToList())
                {
                    contexto.UsuarioCentroServicio_SEG.Remove(usuCS);
                }
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Autoriza el ingreso de un usuaario a través de una gestion específica
        /// </summary>
        /// <param name="gestion">gestion</param>
        /// <param name="codigoUsuario">Codigo del usuario autorizado</param>
        public void AutorizarGestion(SEGestion gestion, long codigoUsuario)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                UsuarioGestion_SEG usuGS = new UsuarioGestion_SEG()
                {
                    USG_CreadoPor = ControllerContext.Current.Usuario,
                    USG_Descripcion = gestion.Descripcion,
                    USG_FechaGrabacion = DateTime.Now,
                    USG_IdCodigoUsuario = codigoUsuario,
                    USG_CodigoGestion = gestion.Id,
                };

                contexto.UsuarioGestion_SEG.Add(usuGS);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Desautoriza el ingreso de un usuario al sistema a través de una gesion específica
        /// </summary>
        /// <param name="codigoUsuario">Codigo del usuario</param>
        public void DesautorizarGestiones(long codigoUsuario)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                foreach (UsuarioGestion_SEG usuGS in contexto.UsuarioGestion_SEG.Where(us => us.USG_IdCodigoUsuario == codigoUsuario).ToList())
                {
                    contexto.UsuarioGestion_SEG.Remove(usuGS);
                }
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Autoriza el ingreso de un usuaario a través de una sucursal de un cliente específico
        /// </summary>
        /// <param name="sucursal">sucursal</param>
        /// <param name="codigoUsuario">Codigo del usuario autorizado</param>
        public void AutorizarSucursal(SESucursal sucursal, long codigoUsuario)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                UsuarioSucursal_SEG usuSC = new UsuarioSucursal_SEG()
                {
                    USS_CreadoPor = ControllerContext.Current.Usuario,
                    USS_FechaGrabacion = DateTime.Now,
                    USS_IdCodigoUsuario = codigoUsuario,
                    USS_IdSucursal = sucursal.Id,
                    USS_Nombre = sucursal.Descripcion
                };

                contexto.UsuarioSucursal_SEG.Add(usuSC);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Desautoriza el ingreso de un usuario al sistema a través de una sucursal específica
        /// </summary>
        /// <param name="codigoUsuario">Codigo del usuario</param>
        public void DesautorizarSucursales(long codigoUsuario)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                foreach (UsuarioSucursal_SEG usuGS in contexto.UsuarioSucursal_SEG.Where(us => us.USS_IdCodigoUsuario == codigoUsuario).ToList())
                {
                    contexto.UsuarioSucursal_SEG.Remove(usuGS);
                    contexto.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Obtiene una lista con los tipos de usuario
        /// </summary>
        /// <returns></returns>
        public List<SETipoUsuario> ObtenerTipoUsuario()
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TipoUsuario_VSEG
                  .OrderBy(o => o.Descripcion)
                  .ToList()
                  .ConvertAll<SETipoUsuario>(r => new SETipoUsuario()
                  {
                      IdTipoUsuario = r.IdTipoUsuario,
                      DescripcionTipoUsuario = r.Descripcion
                  });
            }
        }

        /// <summary>
        /// Obtiene una lista con los estados del usuario
        /// </summary>
        /// <returns></returns>
        public List<SEEstadoUsuario> ObtenerEstadoUsuario()
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.EstadoActivoInactivo_VFRM.ToList().ConvertAll<SEEstadoUsuario>(r => new SEEstadoUsuario()
                {
                    IdEstado = r.IdEstado,
                    EstadoUsuario = r.Estado
                });
            }
        }

        /// <summary>
        /// Obtiene los datos necesarios para cambiar el password
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        public string ObtenerDatosCambioPassword(string idUsuario)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                Usuario_SEG tipoUsuario = contexto.Usuario_SEG.Where(r => r.USU_IdUsuario == idUsuario).SingleOrDefault();
                if (tipoUsuario != null)
                {
                    return tipoUsuario.USU_TipoUsuario;
                }
                else
                    return "";
            }
        }

        /// <summary>
        /// Obteners the id usuario.
        /// </summary>
        /// <param name="Usuario">The usuario.</param>
        /// <returns></returns>
        public long ObtenerIdUsuario(string Usuario)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                return contexto.Usuario_SEG.First(u => u.USU_IdUsuario == Usuario).USU_IdCodigoUsuario;
            }
        }

        #endregion Administración Usuarios

        #region Administración Roles

        /// <summary>
        /// Obtiene todos los roles que estan configurados en la base de datos
        /// </summary>
        /// <returns>Colección con los roles configurados en la base de datos</returns>
        public IEnumerable<SERol> ObtenerRoles(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ConsultarContainsRol_SEG(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                  .ToList()
                  .ConvertAll(r => new SERol
                  {
                      IdRol = r.ROL_IdRol,
                      Descripcion = r.ROL_Descripcion,
                      RequiereIdMaquina = r.ROL_RequiereIdMaquina
                  });
            }
        }

        /// <summary>
        ///  Inserta Roles en la Base de Datos
        /// </summary>
        /// <param name="rol">Objeto rol con la informacion del rol</param>
        public void InsertarRoles(SERol rol)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                DateTime fecha = DateTime.Now;
                Rol_SEG RolEn = new Rol_SEG()
                {
                    ROL_Descripcion = rol.Descripcion.Trim(),
                    ROL_IdRol = rol.IdRol.ToUpper().Trim(),
                    ROL_RequiereIdMaquina = rol.RequiereIdMaquina,
                    ROL_FechaGrabacion = fecha,
                    ROL_CreadoPor = ControllerContext.Current.Usuario
                };
                contexto.Rol_SEG.Add(RolEn);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Edita Roles en la base de datos
        /// </summary>
        /// <param name="rol">Objeto rol con la informacion del rol</param>
        public void EditarRol(SERol rol)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var rolEn = contexto.Rol_SEG
                  .Where(r => r.ROL_IdRol == rol.IdRol)
                  .SingleOrDefault();

                DateTime fecha = DateTime.Now;
                rolEn.ROL_Descripcion = rol.Descripcion;
                rolEn.ROL_FechaGrabacion = fecha;
                rolEn.ROL_RequiereIdMaquina = rol.RequiereIdMaquina;
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Borra Roles en la base de datos
        /// </summary>
        /// <param name="rol">Objeto rol con la informacion del rol</param>
        public void BorrarRol(SERol rol)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Rol_SEG RolEn = contexto.Rol_SEG.Where(r => r.ROL_IdRol == rol.IdRol).First();
                contexto.Rol_SEG.Remove(RolEn);
                contexto.SaveChanges();
            }
        }

        #endregion Administración Roles

        #region Administración Menus

        /// <summary>
        /// Obtiene los menus de la base de datos
        /// </summary>
        /// <returns>Colección con los menus de la base de datos</returns>
        public IEnumerable<SEMenu> ObtenerMenus(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ConsultarContainsMenu_SEG(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                  .ToList()
                  .ConvertAll(r => new SEMenu
                  {
                      IdMenu = r.MEN_IdMenu,
                      Etiqueta = r.MEN_Etiqueta,
                      Assembly = r.MEN_Assembly,
                      NameSpace = r.MEN_NameSpace,
                      UserControl = r.MEN_UserControl,
                      UrlRelativa = r.MEN_UrlRelativa,
                      Comentarios = r.MEN_Comentarios,
                      IdModulo = r.MEN_IdModulo,
                      NomModulo = contexto.Modulo_VER.Where(mod => mod.MOD_IdModulo == r.MEN_IdModulo).SingleOrDefault().MOD_Descripcion,
                      AplicaAgencia = r.MEN_AplicaAgencia,
                      AplicaClienteCredito = r.MEN_AplicaCliente,
                      AplicaCol = r.MEN_AplicaCol,
                      AplicaGestion = r.MEN_AplicaGestion,
                      AplicaPunto = r.MEN_AplicaPunto,
                      AplicaRacol = r.MEN_AplicaRacol
                  });
            }
        }

        /// <summary>
        /// Borra menus en la base de datos
        /// </summary>
        /// <param name="rol">Objeto con la informacion del menu</param>
        public void EliminarMenu(SEMenu menu)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                DateTime fecha = DateTime.Now;

                Menu_SEG menuEn = contexto.Menu_SEG
                  .Where(r => r.MEN_IdMenu == menu.IdMenu).First();

                // TODO ID: Se Agrega Auditoria a la Administracion de Menus
                contexto.MenuHis_SEG.Add(new MenuHis_SEG()
                {
                    MEN_IdMenu = menuEn.MEN_IdMenu,
                    MEN_IdModulo = menuEn.MEN_IdModulo,
                    MEN_Etiqueta = menuEn.MEN_Etiqueta,
                    MEN_Assembly = menuEn.MEN_Assembly,
                    MEN_NameSpace = menuEn.MEN_NameSpace,
                    MEN_UserControl = menuEn.MEN_UserControl,
                    MEN_UrlRelativa = menuEn.MEN_UrlRelativa,
                    MEN_Comentarios = menuEn.MEN_Comentarios,
                    MEN_FechaGrabacion = menuEn.MEN_FechaGrabacion,
                    MEN_AplicaAgencia = menuEn.MEN_AplicaAgencia,
                    MEN_AplicaCliente = menuEn.MEN_AplicaCliente,
                    MEN_AplicaCol = menuEn.MEN_AplicaCol,
                    MEN_AplicaGestion = menuEn.MEN_AplicaGestion,
                    MEN_AplicaPunto = menuEn.MEN_AplicaPunto,
                    MEN_AplicaRacol = menuEn.MEN_AplicaRacol,
                    MEN_CreadoPor = menuEn.MEN_CreadoPor,

                    MEN_CambiadoPor = ControllerContext.Current.Usuario,
                    MEN_FechaCambio = fecha,
                    MEN_TipoCambio = "Eliminado"
                });

                contexto.Menu_SEG.Remove(menuEn);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        ///  Inserta menus en la Base de Datos
        /// </summary>
        /// <param name="menu">Objeto rol con la informacion del rol</param>
        public void AdicionarMenu(SEMenu menu)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                DateTime fecha = DateTime.Now;

                Menu_SEG menuEn = new Menu_SEG()
                {
                    MEN_IdModulo = menu.IdModulo,
                    MEN_Etiqueta = menu.Etiqueta,
                    MEN_Assembly = menu.Assembly,
                    MEN_NameSpace = menu.NameSpace,
                    MEN_UserControl = menu.UserControl,
                    MEN_UrlRelativa = menu.UrlRelativa,
                    MEN_Comentarios = menu.Comentarios,
                    MEN_FechaGrabacion = fecha,
                    MEN_AplicaAgencia = menu.AplicaAgencia,
                    MEN_AplicaCliente = menu.AplicaClienteCredito,
                    MEN_AplicaCol = menu.AplicaCol,
                    MEN_AplicaGestion = menu.AplicaGestion,
                    MEN_AplicaPunto = menu.AplicaPunto,
                    MEN_AplicaRacol = menu.AplicaRacol,
                    MEN_CreadoPor = ControllerContext.Current.Usuario,
                };
                contexto.Menu_SEG.Add(menuEn);

                // TODO ID: Se Agrega Auditoria a la Administracion de Menus
                contexto.MenuHis_SEG.Add(new MenuHis_SEG()
                {
                    MEN_IdMenu = menuEn.MEN_IdMenu,
                    MEN_IdModulo = menu.IdModulo,
                    MEN_Etiqueta = menu.Etiqueta,
                    MEN_Assembly = menu.Assembly,
                    MEN_NameSpace = menu.NameSpace,
                    MEN_UserControl = menu.UserControl,
                    MEN_UrlRelativa = menu.UrlRelativa,
                    MEN_Comentarios = menu.Comentarios,
                    MEN_FechaGrabacion = fecha,
                    MEN_AplicaAgencia = menu.AplicaAgencia,
                    MEN_AplicaCliente = menu.AplicaClienteCredito,
                    MEN_AplicaCol = menu.AplicaCol,
                    MEN_AplicaGestion = menu.AplicaGestion,
                    MEN_AplicaPunto = menu.AplicaPunto,
                    MEN_AplicaRacol = menu.AplicaRacol,
                    MEN_CreadoPor = ControllerContext.Current.Usuario,

                    MEN_CambiadoPor = ControllerContext.Current.Usuario,
                    MEN_FechaCambio = fecha,
                    MEN_TipoCambio = "Adicionado"
                });

                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Edita menus en la base de datos
        /// </summary>
        /// <param name="menu">Objeto menu</param>
        public void EditarMenu(SEMenu menu)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                DateTime fecha = DateTime.Now;

                var menuEn = contexto.Menu_SEG
                  .Where(r => r.MEN_IdMenu == menu.IdMenu)
                  .FirstOrDefault();

                menuEn.MEN_IdModulo = menu.IdModulo;
                menuEn.MEN_Etiqueta = menu.Etiqueta;
                menuEn.MEN_Assembly = menu.Assembly;
                menuEn.MEN_NameSpace = menu.NameSpace;
                menuEn.MEN_UserControl = menu.UserControl;
                menuEn.MEN_UrlRelativa = menu.UrlRelativa;
                menuEn.MEN_Comentarios = menu.Comentarios;
                menuEn.MEN_AplicaAgencia = menu.AplicaAgencia;
                menuEn.MEN_AplicaCliente = menu.AplicaClienteCredito;
                menuEn.MEN_AplicaCol = menu.AplicaCol;
                menuEn.MEN_AplicaGestion = menu.AplicaGestion;
                menuEn.MEN_AplicaPunto = menu.AplicaPunto;
                menuEn.MEN_AplicaRacol = menu.AplicaRacol;

                // TODO ID: Se Agrega Auditoria a la Administracion de Menus
                contexto.MenuHis_SEG.Add(new MenuHis_SEG()
                {
                    MEN_IdMenu = menu.IdMenu,
                    MEN_IdModulo = menu.IdModulo,
                    MEN_Etiqueta = menu.Etiqueta,
                    MEN_Assembly = menu.Assembly,
                    MEN_NameSpace = menu.NameSpace,
                    MEN_UserControl = menu.UserControl,
                    MEN_UrlRelativa = menu.UrlRelativa,
                    MEN_Comentarios = menu.Comentarios,
                    MEN_FechaGrabacion = menuEn.MEN_FechaGrabacion,
                    MEN_AplicaAgencia = menu.AplicaAgencia,
                    MEN_AplicaCliente = menu.AplicaClienteCredito,
                    MEN_AplicaCol = menu.AplicaCol,
                    MEN_AplicaGestion = menu.AplicaGestion,
                    MEN_AplicaPunto = menu.AplicaPunto,
                    MEN_AplicaRacol = menu.AplicaRacol,
                    MEN_CreadoPor = menuEn.MEN_CreadoPor,

                    MEN_CambiadoPor = ControllerContext.Current.Usuario,
                    MEN_FechaCambio = fecha,
                    MEN_TipoCambio = "Editado"
                });

                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Obtiene una lista de los modulos
        /// </summary>
        /// <returns>Retorna una lista con los modulos</returns>
        public List<SEModulo> ObtenerModulos()
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.Modulo_VER.ToList().ConvertAll<SEModulo>(r => new SEModulo()
                {
                    IdModulo = r.MOD_IdModulo,
                    Descripcion = r.MOD_Descripcion
                });
            }
        }

        /// <summary>
        /// Consulta los reportes asociados a un rol específico.
        /// </summary>
        /// <param name="rol"></param>
        /// <returns></returns>
        public SERolReportes ConsultarReportesxRol(SERol rol)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                List<REPInfoReporte> reportesRolbd = contexto.ReportesRol_REP.Include("AdministradorReportes_REP").Where(rep => rep.RER_IdRol == rol.IdRol).ToList().ConvertAll<REPInfoReporte>(re =>
                  {
                      return new REPInfoReporte()
                      {
                          IdModulo = re.AdministradorReportes_REP.ADR_IdModulo,
                          IdReporte = re.AdministradorReportes_REP.ADR_IdReporte,
                          NombreReporte = re.AdministradorReportes_REP.ADR_IdModulo + "-" + re.AdministradorReportes_REP.ADR_NombreReporte,
                          ReportPath = re.AdministradorReportes_REP.ADR_ReportPath,
                          ReportServerUrl = re.AdministradorReportes_REP.ADR_ReportServerUrl
                      };
                  }
                  );

                SERolReportes reportesRolReturn = new SERolReportes()
                {
                    Rol = rol,
                    Reportes = new ObservableCollection<REPInfoReporte>(reportesRolbd)
                };

                return reportesRolReturn;
            }
        }

        /// <summary>
        /// Almacena en la base de datos un reporte asociado a un rol específico
        /// </summary>
        /// <param name="reporte"></param>
        /// <param name="rol"></param>
        public void GuardarReporteRol(REPInfoReporte reporte, SERol rol)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ReportesRol_REP repRol = contexto.ReportesRol_REP.Where(rep => rep.RER_IdReporte == reporte.IdReporte && rep.RER_IdRol == rol.IdRol).FirstOrDefault();

                if (repRol == null)
                {
                    repRol = new ReportesRol_REP()
                    {
                        RER_CreadoPor = ControllerContext.Current.Usuario,
                        RER_FechaGrabacion = DateTime.Now,
                        RER_IdReporte = reporte.IdReporte,
                        RER_IdRol = rol.IdRol
                    };

                    contexto.ReportesRol_REP.Add(repRol);

                    contexto.SaveChanges();
                }
            }
        }

        /// <summary>
        /// remueve los reportes asociados a un rol específico.
        /// </summary>
        /// <param name="reporte"></param>
        /// <param name="rol"></param>
        public void RemoverReportesRol(SERol rol)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                List<ReportesRol_REP> reportesRol = contexto.ReportesRol_REP.Where(rep => rep.RER_IdRol == rol.IdRol).ToList();

                foreach (ReportesRol_REP repRol in reportesRol)
                {
                    contexto.ReportesRol_REP.Remove(repRol);
                }
                contexto.SaveChanges();
            }
        }

        #endregion Administración Menus

        #region Administración Maquinas

        /// <summary>
        /// Obtiene todos las maquinas registradas en el sistema
        /// </summary>
        /// <returns>Colección con las maquinas configuradas en la base de datos</returns>
        public IEnumerable<SEMaquinaVersion> ObtenerMaquinas(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            totalRegistros = 0;

            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ConsultarContainsMaqVersion_VER(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                  .ToList()
                  .ConvertAll(r => new SEMaquinaVersion
                  {
                      MaquinaId = r.MAV_MaquinaId,
                      MaquinaVersionId = r.MAV_MaquinaVersionId,
                      Estado = r.MAV_Estado,
                      Usuario = r.MAV_CreadoPor,
                      Fecha = r.MAV_FechaGrabacion
                  });
            }
        }

        /// <summary>
        /// Actualiza el estado de la maquina
        /// </summary>
        /// <param name="maquina">Objeto maquina con la información de la maquina</param>
        public void EditarMaquina(SEMaquinaVersion maquina)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var maquinaIn = contexto.MaqVersion_VER
                  .Where(r => r.MAV_MaquinaVersionId == maquina.MaquinaVersionId)
                  .SingleOrDefault();
                maquinaIn.MAV_Estado = maquina.Estado;
                SERepositorioAudit.MapeoAuditMaquinaEstado(contexto);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Elimina el registro de la Caja del Punto
        /// por Id de Caja y por Centro de Servicio.
        /// </summary>
        /// <param name="idPunto">The id punto.</param>
        /// <param name="idCaja">The id caja.</param>
        public void EliminarCajaPunto(long idPunto, int idCaja)
        {
            //using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //  CajaPuntoAtencion_VER CajaPunto = contexto.CajaPuntoAtencion_VER.FirstOrDefault(caja => caja.CPA_IdCaja == idCaja && caja.CPA_IdPuntoAtencion == idPunto);
            //  if (CajaPunto != null)
            //  {
            //    contexto.CajaPuntoAtencion_VER.Remove(CajaPunto);
            //    contexto.SaveChanges();
            //  }
            //}
        }

        #endregion Administración Maquinas

        #region Administración Cargos

        /// <summary>
        /// Obtiene una lista con los cargos
        /// </summary>
        /// <returns></returns>
        public List<SECargo> ObtenerCargos()
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.Cargo_SEG
                  .OrderBy(o => o.CAR_Descripcion)
                  .ToList()
                  .ConvertAll<SECargo>
                  (r => new SECargo()
                  {
                      IdCargo = r.CAR_IdCargo,
                      DescripcionCargo = r.CAR_Descripcion,
                      CarIdCargoReporta = r.CAR_IdCargoReporta,
                  }
                  );
            }
        }

        /// <summary>
        /// Obtiene los hijos de un cargo
        /// </summary>
        /// <param name="cargo"></param>
        /// <returns>lista de cargos</returns>
        public List<SECargo> ObtenerHijo(SECargo cargo)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var cargoSE =
                   contexto.Cargo_SEG
                  .Where(r => r.CAR_IdCargoReporta == cargo.IdCargo)
                  .ToList()
                  .ConvertAll<SECargo>
                  (r => new SECargo()
                  {
                      IdCargo = r.CAR_IdCargo,
                      DescripcionCargo = r.CAR_Descripcion,
                      CarIdCargoReporta = r.CAR_IdCargoReporta
                  }
                  );
                if (cargoSE != null)
                    return cargoSE;
                else
                    return null;
            }
        }

        /// <summary>
        /// Ontiene el apdre de la coleccion
        /// </summary>
        /// <returns></returns>
        public SECargo ObtenerPadre()
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var cargoSE =
                   contexto.Cargo_SEG
                  .Where(r => r.CAR_IdCargoReporta == -1)
                  .SingleOrDefault();

                SECargo f = new SECargo()
                {
                    IdCargo = cargoSE.CAR_IdCargo,
                    DescripcionCargo = cargoSE.CAR_Descripcion,
                    CarIdCargoReporta = cargoSE.CAR_IdCargoReporta
                };

                return f;
            }
        }

        /// <summary>
        /// Obtiene una lista con los cargos para filtrar
        /// </summary>
        /// <returns>Colección con los cargos configuradas en la base de datos</returns>
        public IEnumerable<SECargo> ObtenerCargosFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ConsultarContainsCargos_VSEG(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                  .ToList()
                  .ConvertAll(r => new SECargo
                  {
                      IdCargo = r.CAR_IdCargo,
                      DescripcionCargo = r.CAR_Descripcion,
                      CarIdCargoReporta = r.CAR_IdCargoReporta,
                      DescripcionReporta = r.CAR_DescripcionReporta
                  });
            }
        }

        /// <summary>
        /// Borra cargos en la base de datos
        /// </summary>
        /// <param name="rol">Objeto con la informacion del cargo</param>
        public void EliminarCargo(SECargo cargo)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Cargo_SEG cargoEn = contexto.Cargo_SEG
                  .Where(r => r.CAR_IdCargo == cargo.IdCargo).First();
                contexto.Cargo_SEG.Remove(cargoEn);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        ///  Inserta cargos en la Base de Datos
        /// </summary>
        /// <param name="cargo">Objeto cargo con la informacion del cargo</param>
        public void AdicionarCargo(SECargo cargo)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                DateTime fecha = DateTime.Now;

                Cargo_SEG cargoEn = new Cargo_SEG()
                {
                    CAR_Descripcion = cargo.DescripcionCargo,
                    CAR_IdCargoReporta = cargo.CarIdCargoReporta,
                    CAR_FechaGrabacion = fecha,
                    CAR_CreadoPor = ControllerContext.Current.Usuario
                };
                contexto.Cargo_SEG.Add(cargoEn);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Actualiza el cargo
        /// </summary>
        /// <param name="maquina">Objeto cargo</param>
        public void EditarCargo(SECargo cargo)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var cargoIn = contexto.Cargo_SEG
                  .Where(r => r.CAR_IdCargo == cargo.IdCargo)
                  .SingleOrDefault();
                cargoIn.CAR_Descripcion = cargo.DescripcionCargo;
                cargoIn.CAR_IdCargoReporta = cargo.CarIdCargoReporta;
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Valida si el cargo que va a autorizar la actividad pueda hacerlo
        /// </summary>
        /// <param name="idCargoAutoriza">Id del cargo que va a realizar la autorizacion</param>
        /// <param name="idCargoAutenticado">Id del cargo</param>
        /// <returns>true si esta autorizado, false si no lo esta</returns>
        public bool ValidaCargoParaAutorizar(int idCargoAutoriza, int idCargoAutenticado)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                int idCargo;
                bool Autorizado = false;
                var cargoReporta = contexto.Cargo_SEG.Where(r => r.CAR_IdCargo == idCargoAutenticado).FirstOrDefault();
                if (cargoReporta.CAR_IdCargoReporta != -1)
                {
                    if (cargoReporta.CAR_IdCargoReporta == idCargoAutoriza)
                        Autorizado = true;
                    else
                    {
                        if (int.TryParse(cargoReporta.CAR_IdCargoReporta.ToString(), out idCargo))
                        {
                            return ValidaCargoParaAutorizar(idCargoAutoriza, idCargo);
                        }
                    }
                }
                else if (cargoReporta.CAR_IdCargo == idCargoAutoriza)
                {
                    Autorizado = true;
                }
                else
                    Autorizado = false;

                return Autorizado;
            }
        }

        #endregion Administración Cargos

        #region Privados

        /// <summary>
        /// Cargar la información del usuario a partir de su id de usuario (Login)
        /// </summary>
        /// <param name="idUsuario">Identificación (Login) del usuario</param>
        /// <returns>Entidad con el usuario, en caso de no encontralo se retorna null</returns>
        public Usuario_SEG ObtenerUsuarioPorIdUsuario(string idUsuario)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                //consultar la información del usuario
                Usuario_SEG usuario = contexto.Usuario_SEG
                  .Where(o => o.USU_IdUsuario.Equals(idUsuario))
                  .SingleOrDefault();

                return usuario;
            }
        }

        /// <summary>
        /// Obtiene el cod del Usuario dela
        /// caja principal
        /// </summary>
        /// <param name="idCentroSrv"></param>
        /// <returns></returns>
        public SEUsuarioPorCodigoDC ObtenerUsuarioCajaPpalCentroSvc(long idCentroSrv)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                SEUsuarioPorCodigoDC usuarioCajaPpal = new SEUsuarioPorCodigoDC();

                var usuario = contexto.UsuarioCentroServicio_SEG.FirstOrDefault(idcentro => idcentro.UCS_IdCentroServicios == idCentroSrv && idcentro.UCS_Caja == 0);

                if (usuario != null)
                {
                    usuarioCajaPpal = new SEUsuarioPorCodigoDC()
                    {
                        IdCodigoUsuario = usuario.UCS_IdCodigoUsuario,
                    };
                }

                return usuarioCajaPpal;
            }
        }

        /// <summary>
        /// Obtiene la info de la persona interna por el codigo del
        /// usuario
        /// </summary>
        /// <param name="idCodigoUsuario"></param>
        /// <returns></returns>
        public SEUsuarioPorCodigoDC ObtenerUsuarioPorCodigo(long idCodigoUsuario)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                SEUsuarioPorCodigoDC infoUsuario = contexto.UsuarioPersonaInterna_SEG.Include("PersonaInterna_PAR").Where(perso => perso.USP_IdCodigoUsuario == idCodigoUsuario)
                                        .ToList()
                                        .ConvertAll<SEUsuarioPorCodigoDC>(per => new SEUsuarioPorCodigoDC()
                                        {
                                            IdCodigoUsuario = per.USP_IdPersonaInterna,
                                            NombreUsuario = per.PersonaInterna_PAR.PEI_Nombre,
                                            PrimerApellido = per.PersonaInterna_PAR.PEI_PrimerApellido,
                                            SegundoApellido = per.PersonaInterna_PAR.PEI_SegundoApellido,
                                            Documento = per.PersonaInterna_PAR.PEI_Identificacion,
                                            NombreCompleto = string.Format("{0} {1} {2}", per.PersonaInterna_PAR.PEI_Nombre, per.PersonaInterna_PAR.PEI_PrimerApellido, per.PersonaInterna_PAR.PEI_SegundoApellido),
                                            Usuario = contexto.Usuario_SEG.FirstOrDefault(a => a.USU_IdCodigoUsuario == idCodigoUsuario).USU_IdUsuario,
                                        }).FirstOrDefault();

                return infoUsuario;
            }
        }

        /// <summary>
        /// Obtiene la info de la persona interna por el id del
        /// usuario
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns>Info del Usuario</returns>
        public SEUsuarioPorCodigoDC ObtenerUsuarioPorIdUsuarioData(string idUsuario)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                SEUsuarioPorCodigoDC infoUsuario;

                infoUsuario = contexto.UsuarioPersonaInterna_SEG.Include("PersonaInterna_PAR").Include("Usuario_SEG")
                    .Where(usuari => usuari.Usuario_SEG.USU_IdUsuario == idUsuario)
                                        .ToList()
                                        .ConvertAll<SEUsuarioPorCodigoDC>(per => new SEUsuarioPorCodigoDC()
                                        {
                                            IdCodigoUsuario = per.USP_IdPersonaInterna,
                                            NombreUsuario = per.PersonaInterna_PAR.PEI_Nombre,
                                            PrimerApellido = per.PersonaInterna_PAR.PEI_PrimerApellido,
                                            SegundoApellido = per.PersonaInterna_PAR.PEI_SegundoApellido,
                                            Documento = per.PersonaInterna_PAR.PEI_Identificacion,
                                            NombreCompleto = string.Format("{0} {1} {2}", per.PersonaInterna_PAR.PEI_Nombre, per.PersonaInterna_PAR.PEI_PrimerApellido, per.PersonaInterna_PAR.PEI_SegundoApellido),
                                            Usuario = per.Usuario_SEG.USU_IdUsuario,
                                            EstadoUsuario = per.Usuario_SEG.USU_Estado,
                                        }).FirstOrDefault();

                if (infoUsuario == null)
                {
                    infoUsuario = contexto.UsuarioPersonaExterna_SEG.Include("PersonaExterna_PAR").Include("Usuario_SEG")
                      .Where(usuari => usuari.Usuario_SEG.USU_IdUsuario == idUsuario)
                                          .ToList()
                                          .ConvertAll<SEUsuarioPorCodigoDC>(per => new SEUsuarioPorCodigoDC()
                                          {
                                              IdCodigoUsuario = per.USP_IdPersonaExterna,
                                              NombreUsuario = string.Format("{0} {1}", per.PersonaExterna_PAR.PEE_PrimerNombre, per.PersonaExterna_PAR.PEE_SegundoNombre),
                                              PrimerApellido = per.PersonaExterna_PAR.PEE_PrimerApellido,
                                              SegundoApellido = per.PersonaExterna_PAR.PEE_SegundoApellido,
                                              Documento = per.PersonaExterna_PAR.PEE_Identificacion,
                                              NombreCompleto = string.Format("{0} {1} {2} {3}", per.PersonaExterna_PAR.PEE_PrimerNombre, per.PersonaExterna_PAR.PEE_SegundoNombre, per.PersonaExterna_PAR.PEE_PrimerApellido, per.PersonaExterna_PAR.PEE_SegundoApellido),
                                              Usuario = per.Usuario_SEG.USU_IdUsuario,
                                              EstadoUsuario = per.Usuario_SEG.USU_Estado,
                                          }).FirstOrDefault();
                }

                return infoUsuario;
            }
        }

        /// <summary>
        /// Consulta el usuario activo
        /// por el id de la persona interna
        /// </summary>
        /// <param name="idPersonaInterna"></param>
        /// <returns></returns>
        public SEUsuarioPorCodigoDC ObtenerUsuarioPersonaInterna(long idPersonaInterna)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PersonaInterna_PAR infoPersona = contexto.PersonaInterna_PAR.FirstOrDefault(per => per.PEI_IdPersonaInterna == idPersonaInterna);

                SEUsuarioPorCodigoDC infoUsuario = contexto.UsuarioPersonaInterna_SEG.Include("Usuario_SEG")
                                        .Where(perso => perso.USP_IdPersonaInterna == idPersonaInterna)
                                        .ToList()
                                        .ConvertAll<SEUsuarioPorCodigoDC>(per => new SEUsuarioPorCodigoDC()
                                        {
                                            IdCodigoUsuario = per.Usuario_SEG.USU_IdCodigoUsuario,
                                            Usuario = per.Usuario_SEG.USU_IdUsuario,
                                            EstadoUsuario = per.Usuario_SEG.USU_Estado,
                                            NombreCompleto = string.Format("{0} {1} {2}", infoPersona.PEI_Nombre, infoPersona.PEI_PrimerApellido, infoPersona.PEI_SegundoApellido),
                                            Documento = infoPersona.PEI_Identificacion
                                        }).FirstOrDefault();

                return infoUsuario;
            }
        }

        /// <summary>
        /// Consulta el usuario
        /// por el id de la Persona Externa
        /// </summary>
        /// <param name="IdPersonaExterna"></param>
        /// <returns></returns>
        public SEUsuarioPorCodigoDC ObtenerUsuarioPersonaExterna(long IdPersonaExterna)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PersonaExterna_PAR infoPersona = contexto.PersonaExterna_PAR.FirstOrDefault(per => per.PEE_IdPersonaExterna == IdPersonaExterna);

                SEUsuarioPorCodigoDC infoUsuario = contexto.UsuarioPersonaExterna_SEG.Include("Usuario_SEG")
                                        .Where(perso => perso.USP_IdPersonaExterna == IdPersonaExterna)
                                        .ToList()
                                        .ConvertAll<SEUsuarioPorCodigoDC>(per => new SEUsuarioPorCodigoDC()
                                        {
                                            IdCodigoUsuario = per.Usuario_SEG.USU_IdCodigoUsuario,
                                            Usuario = per.Usuario_SEG.USU_IdUsuario,
                                            EstadoUsuario = per.Usuario_SEG.USU_Estado,
                                            NombreCompleto = string.Format("{0} {1} {2} {3}", infoPersona.PEE_PrimerNombre, infoPersona.PEE_SegundoNombre,
                                                                                            infoPersona.PEE_PrimerApellido, infoPersona.PEE_SegundoApellido),
                                            Documento = infoPersona.PEE_Identificacion
                                        }).FirstOrDefault();
                return infoUsuario;
            }
        }

        /// <summary>
        /// Guarda los roles asignados y sin asignar de un usuario
        /// </summary>
        /// <param name="rolUsuario"></param>
        private void GuardarRolesUsuario(SERolUsuario rol)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Usuario_SEG usuario = ObtenerUsuarioPorIdUsuario(rol.IdUsuario);

                if (usuario != null)
                {
                    DateTime fecha = DateTime.Now;

                    rol.Roles.ForEach(r =>
                    {
                        UsuarioRol_SEG Rol = contexto.UsuarioRol_SEG
                       .Where(ur => ur.USR_IdRol == r.IdRol && ur.USR_IdCodigoUsuario == usuario.USU_IdCodigoUsuario)
                       .SingleOrDefault();

                        if (Rol == null)
                            contexto.UsuarioRol_SEG.Add(new UsuarioRol_SEG()
                            {
                                USR_IdRol = r.IdRol,
                                USR_IdCodigoUsuario = usuario.USU_IdCodigoUsuario,
                                USR_CreadoPor = ControllerContext.Current.Usuario,
                                USR_FechaGrabacion = fecha
                            });
                    });

                    // TODO ID: Se agrega Auditoria a la Administracion de Usuario-Roles
                    rol.Roles.ForEach(r =>
                    {
                        UsuarioRol_SEG Rol = contexto.UsuarioRol_SEG
                       .Where(ur => ur.USR_IdRol == r.IdRol && ur.USR_IdCodigoUsuario == usuario.USU_IdCodigoUsuario)
                       .SingleOrDefault();

                        if (Rol == null)
                            contexto.UsuarioRolHistorico_SEG.Add(new UsuarioRolHistorico_SEG()
                            {
                                USR_IdRol = r.IdRol,
                                USR_IdCodigoUsuario = usuario.USU_IdCodigoUsuario,
                                USR_CreadoPor = ControllerContext.Current.Usuario,
                                USR_FechaGrabacion = fecha,

                                USR_CambiadoPor = ControllerContext.Current.Usuario,
                                USR_FechaCambio = fecha,
                                USR_TipoCambio = "Adicionado"
                            });
                    });

                    contexto.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Elimina una relacion de usuario con rol
        /// </summary>
        /// <param name="rolUsuario"></param>
        private void EliminarRolUsuario(SERolUsuario rolUsuario)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Usuario_SEG usuario = ObtenerUsuarioPorIdUsuario(rolUsuario.IdUsuario);

                rolUsuario.Roles.ForEach(rol =>
                {
                    UsuarioRol_SEG Rol = contexto.UsuarioRol_SEG
                      .Where(ur => ur.USR_IdRol == rol.IdRol && ur.USR_IdCodigoUsuario == usuario.USU_IdCodigoUsuario)
                      .SingleOrDefault();
                    if (Rol != null)
                        contexto.UsuarioRol_SEG.Remove(Rol);
                });

                // Auditoria al Eliminar Usuarios Roles
                SERepositorioAudit.MapeoAuditRolUsuario(contexto);

                contexto.SaveChanges();
            }
        }

        #endregion Privados

        public string ConsultarNombreUsuarioPorCedula(int numCedula)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                string numero = numCedula.ToString();
                var usuarioIterno = contexto.UsuarioPersonaInterna_SEG.Include("Usuario_SEG").FirstOrDefault(u => u.PersonaInterna_PAR.PEI_Identificacion == numero);
                if (usuarioIterno != null)
                {
                    return string.Join(",", new string[] { usuarioIterno.Usuario_SEG.USU_IdUsuario, usuarioIterno.Usuario_SEG.USU_Estado });
                }
                else
                {
                    var usuarioExterno = contexto.UsuarioPersonaExterna_SEG.Include("Usuario_SEG").FirstOrDefault(u => u.PersonaExterna_PAR.PEE_Identificacion == numCedula.ToString());
                    if (usuarioExterno != null)
                    {
                        return string.Join(",", new string[] { usuarioExterno.Usuario_SEG.USU_IdUsuario, usuarioExterno.Usuario_SEG.USU_Estado });
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }
        }

        public List<string> COnsultarUsuariosRacol(int idRacol)
        {
            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.UsuarioPersonaInterna_SEG
                  .Include("Usuario_SEG")
                  .Include("PersonaInterna_PAR")
                  .Where(u => u.PersonaInterna_PAR.PEI_IdRegionalAdm == idRacol)
                  .ToList()
                  .ConvertAll(u => string.Join(";", new string[] { u.PersonaInterna_PAR.PEI_Identificacion, u.PersonaInterna_PAR.PEI_Nombre + " "
                + u.PersonaInterna_PAR.PEI_PrimerApellido, u.Usuario_SEG.USU_Estado, u.Usuario_SEG.USU_IdUsuario}));
            }
        }

        #region CRUD USUARIO INTEGRACION
        /// <summary>
        /// Inserta en Base de datos un usuario en la tabla UsuarioIntegracion_SEG
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns></returns>
        public int InsertarUsuarioIntegracion(SEUsuarioIntegracionDC usuario)
        {
            int codUsuario = 0;
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paCrearUsuarioIntegracion_SEG", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdUsuario", usuario.IdUsuario);
                cmd.Parameters.AddWithValue("@Usuario", usuario.Usuario);
                cmd.Parameters.AddWithValue("@Contrasena", usuario.Contrasena);
                cmd.Parameters.AddWithValue("@IdCliente", usuario.IdCliente);
                cmd.Parameters.AddWithValue("@CreadoPor", usuario.CreadoPor);
                cmd.Parameters.AddWithValue("@FechaGabacion", usuario.FechaGrabacion);
                cmd.Parameters.AddWithValue("@Estado", usuario.Estado);
                sqlConn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    codUsuario = Convert.ToInt32(reader["Return Value"]);
                }
                sqlConn.Close();
                return codUsuario;
            }

        }
        /// <summary>
        /// Valida el usuario integracion por usuario y contraseña 
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns></returns>
        public int ConsultarUsuarioIntegracion(SEUsuarioIntegracionDC usuario)
        {
            int idCliente = 0;
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paConsultarUsuarioIntegracion_SEG", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Usuario", usuario.Usuario);
                cmd.Parameters.AddWithValue("@Contrasena", usuario.Contrasena);
                sqlConn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    idCliente = Convert.ToInt32(reader["USI_IdCliente"]);
                }
                sqlConn.Close();
                return idCliente;
            }
        }
        /// <summary>
        /// Consulta si el usuario existe en la base de datos validando unicamente el usuario
        /// </summary>
        /// <param name="Usuario"></param>
        /// <returns></returns>
        public int ConsultarUsuarioExisteIntegracion(string Usuario)
        {
            int codUsuario = 0;
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paConsultarUsuarioTextoIntegracion_SEG", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Usuario", Usuario);
                sqlConn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    codUsuario = Convert.ToInt32(reader["USI_IdUsuario"]);
                }
                sqlConn.Close();
                return codUsuario;
            }
        }
        /// <summary>
        /// Edita el usuario en la base de datos 
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns></returns>
        public int EditarUsuarioIntegracion(SEUsuarioIntegracionDC usuario)
        {
            int respuestaQuery = 0;
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paModificarUsuarioIntegracion_SEG", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idUsuario", usuario.IdUsuario);
                cmd.Parameters.AddWithValue("@Usuario", usuario.Usuario);
                cmd.Parameters.AddWithValue("@Contrasena", usuario.Contrasena);
                cmd.Parameters.AddWithValue("@idcliente", usuario.idClienteInt);
                sqlConn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    respuestaQuery = Convert.ToInt32(reader["Return Value"]);
                }
                sqlConn.Close();
                return respuestaQuery;
            }
        }
        /// <summary>
        /// Elimina un usuario de la tabla usuario integracion(Cambia el estado a deshabilitado)
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns></returns>
        public int EliminarUsuarioIntegracion(SEUsuarioIntegracionDC usuario)
        {
            int respuestaQuery = 0;
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paDeshabilitarUsuarioIntegracion_SEG", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idUsuario", usuario.IdUsuario);
                sqlConn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    respuestaQuery = Convert.ToInt32(reader["Return Value"]);
                }
                sqlConn.Close();
                return respuestaQuery;
            }
        }
        /// <summary>
        /// Consulta todos los usuarios activos de la tabla
        /// </summary>
        /// <returns></returns>
        public List<SEUsuarioIntegracionDC> ConsultarUsuariosActivosIntegracion()
        {
            List<SEUsuarioIntegracionDC> respuesta = new List<SEUsuarioIntegracionDC>();
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerUsuariosIntegracion_SEG", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                sqlConn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                sqlConn.Close();

                for (int filas = 0; filas < dt.Rows.Count; filas++)
                {
                    SEUsuarioIntegracionDC usuario = new SEUsuarioIntegracionDC();
                    usuario.IdUsuario = Convert.ToInt32(dt.Rows[filas][0]);
                    usuario.Usuario = dt.Rows[filas][1].ToString();
                    usuario.Contrasena = dt.Rows[filas][2].ToString();
                    usuario.IdCliente = dt.Rows[filas][3].ToString();
                    usuario.Cliente = dt.Rows[filas][4].ToString();
                    usuario.CreadoPor = dt.Rows[filas][5].ToString();
                    usuario.FechaGrabacion = Convert.ToDateTime(dt.Rows[filas][6]);
                    usuario.Estado = Convert.ToBoolean(dt.Rows[filas][7]);
                    respuesta.Add(usuario);
                }
            }
            return respuesta;
        }
        #endregion 

        #region APP
        public void InsertarMensajeroPam(SEAdminUsuario credencial)
        {
            //credencial.CentrosDeServicioAutorizados.FirstOrDefault().Id;
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paInsertarMensajero_CPO", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdTipoMensajero", 7);
                cmd.Parameters.AddWithValue("@IdAgencia", credencial.CentrosDeServicioAutorizados.FirstOrDefault().Id);
                cmd.Parameters.AddWithValue("@Telefono2", credencial.Telefono);
                cmd.Parameters.AddWithValue("@FechaIngreso", DateTime.Now);
                cmd.Parameters.AddWithValue("@FechaTerminacionContrato", DateTime.Now.AddYears(1));
                cmd.Parameters.AddWithValue("@NumeroPase", " ");
                cmd.Parameters.AddWithValue("@FechaVencimientoPase", DateTime.Now.AddYears(1));
                cmd.Parameters.AddWithValue("@Estado", "ACT");
                cmd.Parameters.AddWithValue("@EsContratista", 1);
                cmd.Parameters.AddWithValue("@TipoContrato", 2);
                cmd.Parameters.AddWithValue("@FechaGrabacion", DateTime.Now);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);
                cmd.Parameters.AddWithValue("@IdPersonaInterna", credencial.IdPersonaInterna);
                cmd.Parameters.AddWithValue("@EsMensajeroUrbano", 1);
                sqlConn.Open();
                cmd.ExecuteNonQuery();
                sqlConn.Close();
            }
        }

        /// <summary>
        /// Método para validar los usuarios creados por el app
        /// </summary>
        /// <param name="numeroCedula"></param>
        /// <returns></returns>
        public int ValidarUsuarioPersonaInternaAPP(string numeroCedula)
        {
            int codUsuario = 0;   
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("PaValidarUsuarioPersonaInternaAPP_SEG", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@identificacion", numeroCedula);
                sqlConn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                { 
                    codUsuario = Convert.ToInt32 (reader["USU_TipoUsuario"]);
                }
                sqlConn.Close();
                return codUsuario;
            }
        }

        #endregion
        #region Versionamiento usuarios cliente credito
        public void VersionarCliente(SEAdminUsuario credencial)
        {
            List<SESucursal> sucursales = new List<SESucursal>();
            sucursales = credencial.SucursalesAutorizadas.ToList();


            using (EntidadesSeguridad contexto = new EntidadesSeguridad(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {


                var version = contexto.ParametrosFramework.Where(p => p.PAR_IdParametro == "VersionClientCredit").FirstOrDefault();
                if (version != null)
                {
                    string versionClientecredito = version.PAR_ValorParametro;

                    sucursales.ToList().ForEach(s =>
                    {
                        var cliente = contexto.Sucursal_CLI.Where(su => su.SUC_IdSucursal == s.Id).FirstOrDefault();
                        if (cliente != null)
                        {
                            int idClientCredit = cliente.SUC_ClienteCredito;

                            var versionClienteExist = contexto.VersionCliente_VER.Where(ve => ve.VEC_IdCliente == idClientCredit).FirstOrDefault();

                            if (versionClienteExist == null)
                            {
                                VersionCliente_VER VersionCliente = new VersionCliente_VER()
                                {
                                    VEC_IdCliente = idClientCredit,
                                    VEC_IdVersion = versionClientecredito,
                                    VEC_FechaGrabacion = DateTime.Now,
                                    VEC_CreadoPor = ControllerContext.Current.Usuario
                                };
                                contexto.VersionCliente_VER.Add(VersionCliente);
                                contexto.SaveChanges();
                            }

                        }

                    });
                }



            }
        }
        #endregion


    }
}