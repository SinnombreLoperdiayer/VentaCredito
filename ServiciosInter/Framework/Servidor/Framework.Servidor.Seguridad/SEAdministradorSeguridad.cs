using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Transactions;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Seguridad.Datos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using Framework.Servidor.Servicios.ContratoDatos.Reportes;
using Framework.Servidor.Servicios.ContratoDatos.Seguridad;
using Framework.Servidor.Seguridad.LDAP;
using System.Web.Configuration;
using System.DirectoryServices.AccountManagement;

namespace Framework.Servidor.Seguridad
{
    /// <summary>
    /// Clase para exponer la logica de administraciòn de seguridad
    /// </summary>

    public class SEAdministradorSeguridad : ControllerBase
    {
        /// <summary>
        /// Campo para guardar la lista de cargos hijo de un cargo
        /// </summary>
        private List<SECargo> CargosHijo;

        private ObservableCollection<SECargo> lista = new ObservableCollection<SECargo>();
        private SECargo padre = new SECargo();

        public static readonly SEAdministradorSeguridad Instancia = (SEAdministradorSeguridad)FabricaInterceptores.GetProxy(new SEAdministradorSeguridad(), ConstantesFramework.MODULO_FW_SEGURIDAD);

        internal SEAdministradorSeguridad()
        {
        }

        /// <summary>
        /// Obtiene los Roles del repositorio
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SERol> ObtenerRoles(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return SERepositorio.Instancia.ObtenerRoles(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        ///  Insertar Roles en la Base de Datos
        /// </summary>
        /// <param name="rol">Objeto rol con la informacion del rol</param>
        public void InsertarRoles(SERol rol)
        {
            SERepositorio.Instancia.InsertarRoles(rol);
        }

        /// <summary>
        /// Edita Roles en la Base de Datos
        /// </summary>
        /// <param name="rol">Objeto rol con la informacion del rol</param>
        public void EditarRol(SERol rol)
        {
            SERepositorio.Instancia.EditarRol(rol);
        }

        /// <summary>
        /// Borra Roles en la Base de Datos
        /// </summary>
        /// <param name="rol">Objeto rol con la informacion del rol</param>
        public void BorrarRol(SERol rol)
        {
            SERepositorio.Instancia.BorrarRol(rol);
        }

        /// <summary>
        /// Valida si el usuario pertenece a DB o LDAP para cambiar la contraseña
        /// </summary>
        /// <param name="credencial">Objeto credencial con la información del usuario</param>
        public void CambiarPassword(SECredencialUsuario credencial)
        {
            if (!String.IsNullOrEmpty(credencial.PasswordNuevo))
            {
                string tipoUsuario = SERepositorio.Instancia.ObtenerDatosCambioPassword(credencial.Usuario);
                if (!String.IsNullOrEmpty(tipoUsuario))
                    if (tipoUsuario == COConstantesModulos.USUARIO_APLICACION)
                    {
                        SERepositorio.Instancia.CambiarPassword(credencial);
                    }
                    else if (tipoUsuario == COConstantesModulos.USUARIO_LDAP)
                    {
                        if (SEProveedorLDAP.Instancia.CambiarPassword(credencial) == SEEnumMensajesSeguridad.ERRORCAMBIANDOPASSWORD)
                            throw new FaultException<ControllerException>(new ControllerException(ConstantesFramework.MODULO_FRAMEWORK, ETipoErrorFramework.EX_ERROR_CAMBIANDO_PASSWORD.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_ERROR_CAMBIANDO_PASSWORD)));
                    }
            }
        }

        /// <summary>
        /// Obtiene los menus por roles asignados y sin asignar
        /// </summary>
        /// <param name="rol"></param>
        /// <returns></returns>
        public SEMenuRolAccion ObtenerMenuRol(SERol rol)
        {
            return SERepositorio.Instancia.ObtenerMenuRol(rol);
        }

        /// <summary>
        /// Método para obtener la localidad de un usuario
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        public PALocalidadDC ObtenerLocalidadPorUsuario(string idUsuario)
        {
            return SERepositorio.Instancia.ObtenerLocalidadPorUsuario(idUsuario);
        }

        /// <summary>
        /// Guarda Rol Menu Acción
        /// </summary>
        /// <param name="menuRolAccion"></param>
        public void GuardarMenuRolAccion(SEMenuRolAccionConsolidado menuRolAccion)
        {
            SERepositorio.Instancia.ModificarMenuRolAccion(menuRolAccion);
        }

        /// <summary>
        /// Obtiene los roles por usuario asignados y sin asignar
        /// </summary>
        /// <param name="credencial"></param>
        /// <returns></returns>
        public SERolUsuario ObtenerRolesUsuario(string IdUsuario)
        {
            return SERepositorio.Instancia.ObtenerRolesUsuario(IdUsuario);
        }

        /// <summary>
        /// Obtiene las Acciones de la DB
        /// </summary>
        /// <returns></returns>
        public List<SEAccion> ObtenerAcciones()
        {
            return SERepositorio.Instancia.ObtenerAcciones();
        }

        /// <summary>
        /// Guarda los roles asignados y sin asignar de un usuario
        /// </summary>
        /// <param name="rolUsuario"></param>
        public void GuardarRolesUsuario(SERolUsuario rolUsuario)
        {
            SERolUsuario agregados = new SERolUsuario() { IdUsuario = rolUsuario.IdUsuario, Roles = new List<SERol>() };

            SERolUsuario borrados = new SERolUsuario() { IdUsuario = rolUsuario.IdUsuario, Roles = new List<SERol>() };

            rolUsuario.Roles.ForEach(rol =>
              {
                  if (rol.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
                  {
                      agregados.Roles.Add(rol);
                  }
                  else if (rol.EstadoRegistro == EnumEstadoRegistro.BORRADO)
                  {
                      borrados.Roles.Add(rol);
                  }
              });

            SERepositorio.Instancia.ModificarRolesUsuarioDB(agregados, borrados);
        }

        /// <summary>
        /// Obtener los usuarios activos
        /// </summary>
        /// <param name="filtro">Filtro</param>
        /// <param name="campoOrdenamiento">Campo de ordenamiento de la grilla</param>
        /// <param name="indicePagina">Indice de pagina de la grilla</param>
        /// <param name="registrosPorPagina">Registros por pagina de la grilla</param>
        /// <param name="ordenamientoAscendente">Ordenamiento ascendente</param>
        /// <param name="totalRegistros">Total de registros</param>
        /// <returns></returns>
        public IEnumerable<SEAdminUsuario> ObtenerUsuariosAdmin(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente)
        {
            return SERepositorio.Instancia.ObtenerUsuariosAdmin(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente);
        }

        /// <summary>
        /// Establece el estado INA al Usuario
        /// </summary>
        /// <param name="credencial"></param>
        /// <returns></returns>
        public void EliminarUsuario(SEAdminUsuario credencial)
        {
            SERepositorio.Instancia.EliminarUsuario(credencial);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public SEAdminUsuario ListasAdminUsuario()
        {
            SEAdminUsuario admUsuario = new SEAdminUsuario();

            //admUsuario.TipoIdentificacionColeccion = SERepositorio.Instancia.ObtenerTiposIdentificacion();
            admUsuario.TipoIdentificacionColeccion = SERepositorio.Instancia.ObtenerTiposIdentificacion();
            admUsuario.RegionalColeccion = SERepositorio.Instancia.ObtenerRegionales();
            admUsuario.CargoColleccion = SERepositorio.Instancia.ObtenerCargos();

            //admUsuario.CentroLogisticoColleccion = SERepositorio.Instancia.ObtenerCentrosLogisticos();
            admUsuario.TipoUsuarioColeccion = SERepositorio.Instancia.ObtenerTipoUsuario();
            admUsuario.EstadoUsuarioColeccion = SERepositorio.Instancia.ObtenerEstadoUsuario();

            return admUsuario;
        }

        // <summary>
        /// Obtiene los centros de servicio que esta denominados como PAS
        /// </summary>
        /// <param name="idCiudad"></param>
        /// <returns></returns>
        public List<SECentroServicio> ObtenerCentroServicioPASPorRacol(long idRacol)
        {
            List<SECentroServicio> lstCentrosServiciosPas = new List<SECentroServicio>();
            lstCentrosServiciosPas = SERepositorio.Instancia.ObtenerCentroServicioPASPorRacol(idRacol);
            return lstCentrosServiciosPas;
        }



        /// <summary>
        /// Obtiene los Menus del repositorio
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SEMenu> ObtenerMenus(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return SERepositorio.Instancia.ObtenerMenus(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Borra menus en la Base de Datos
        /// </summary>
        /// <param name="menu">Objeto con la informacion del menu</param>
        public void EliminarMenu(SEMenu menu)
        {
            SERepositorio.Instancia.EliminarMenu(menu);
        }

        /// <summary>
        /// Adiciona menus en la Base de Datos
        /// </summary>
        /// <param name="menu">Objeto con la informacion del menu</param>
        public void AdicionarMenu(SEMenu menu)
        {
            SERepositorio.Instancia.AdicionarMenu(menu);
        }

        /// <summary>
        /// Edita menus en la Base de Datos
        /// </summary>
        /// <param name="menu">Objeto con la informacion del menu</param>
        public void EditarMenu(SEMenu menu)
        {
            SERepositorio.Instancia.EditarMenu(menu);
        }

        /// <summary>
        ///Obtiene los tipos de identificación de la base de datos
        /// </summary>
        /// <returns></returns>
        public List<SETipoIdentificacion> ObtenerTiposIdentificacion()
        {
            return SERepositorio.Instancia.ObtenerTiposIdentificacion();
        }

        /// <summary>
        /// Obtiene los modulos de la base de datos
        /// </summary>
        /// <returns></returns>
        public List<SEModulo> ObtenerModulos()
        {
            return SERepositorio.Instancia.ObtenerModulos();
        }

        public List<SEClienteCredito> ObtenerClientesCredito()
        {
            return SERepositorio.Instancia.ObtenerClientesCredito();
        }

        public List<SESucursal> ObtenerSucursalesClientesCredito(int IdCliente)
        {
            return SERepositorio.Instancia.ObtenerSucursalesXClientesCredito(IdCliente);
        }

        public List<SECasaMatriz> ObtenerCasasMatriz()
        {
            return SERepositorio.Instancia.ObtenerCasasMatriz();
        }

        public List<SEMacroproceso> ObtenerMacroProcesos(short idCasaMatriz)
        {
            return SERepositorio.Instancia.ObtenerMacroProcesos(idCasaMatriz);
        }

        public List<SEGestion> ObtenerGestiones(string idMacroProceso)
        {
            return SERepositorio.Instancia.ObtenerGestiones(idMacroProceso);
        }

        public List<SECentroServicio> ObtenerCentrosDeServicioxCiudad(string idCiudad)
        {
            return SERepositorio.Instancia.ObtenerCentrosDeServicioxCiudad(idCiudad);
        }

        /// <summary>
        /// Adicionar o Eliminar menús
        /// </summary>
        /// <param name="menu">Objeto menú</param>
        public void ActualizarAdminMenu(SEMenu menu)
        {
            if (menu.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
            {
                SERepositorio.Instancia.AdicionarMenu(menu);
            }
            else if (menu.EstadoRegistro == EnumEstadoRegistro.MODIFICADO)
            {
                SERepositorio.Instancia.EditarMenu(menu);
            }
        }

        /// <summary>
        /// Adicionar o eliminar roles
        /// </summary>
        /// <param name="rol">Objeto rol</param>
        public void ActualizarAdminRoles(SERol rol)
        {
            if (rol.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
            {
                SERepositorio.Instancia.InsertarRoles(rol);
            }
            else if (rol.EstadoRegistro == EnumEstadoRegistro.MODIFICADO)
            {
                SERepositorio.Instancia.EditarRol(rol);
            }
        }

        /// <summary>
        /// Adiciona o edita usuarios
        /// </summary>
        /// <param name="credencial"></param>
        public void ActualizarAdminUsuarios(SEAdminUsuario credencial)
        {
            SEUsuario.Instancia.GestionarAdminUsuarios(credencial);

            #region Anterior

            //if (credencial.SucursalesAutorizadas != null)
            //{
            //    if (credencial.SucursalesAutorizadas.Count > 0)
            //    {
            //        credencial.EsUsuarioInterno = false;
            //    }
            //    else
            //    {
            //        credencial.EsUsuarioInterno = true;
            //    }
            //}
            //else { credencial.EsUsuarioInterno = true; }

            //#region Adicionar un Usuario

            //if (credencial.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
            //{
            //    CrearNuevoUsuario(credencial);
            //}

            //#endregion Adicionar un Usuario

            //#region Modificar usuario

            //else if (credencial.EstadoRegistro == EnumEstadoRegistro.MODIFICADO)
            //{
            //    SEPersonaInternaDC validarPersonaInterna = ObtenerPersonaInternaPorIdentificacion(credencial.Identificacion);

            //    if (validarPersonaInterna == null)
            //    {
            //        //desactivo usuario
            //        SERepositorio.Instancia.DesactivarUsuario(credencial);

            //        //creo nuevo usuario
            //        CrearNuevoUsuario(credencial);
            //    }
            //    else
            //    {
            //        SEUsuarioPorCodigoDC usuarioModificar = SERepositorio.Instancia.ObtenerUsuarioPersonaInterna(validarPersonaInterna.IdPersonaInterna);
            //        if (usuarioModificar != null && usuarioModificar.Usuario != credencial.Usuario && credencial.Estado == ConstantesFramework.ESTADO_ACTIVO)
            //        {
            //            ControllerException excepcion = new ControllerException(ConstantesFramework.MODULO_FW_SEGURIDAD,
            //                ETipoErrorFramework.EX_DOCUMENTO_PERSONA_ACTIVO_USUARIO_ACTIVO.ToString(),
            //                string.Format(MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_DOCUMENTO_PERSONA_ACTIVO_USUARIO_ACTIVO),
            //                usuarioModificar.Documento, usuarioModificar.NombreCompleto, usuarioModificar.Usuario));
            //            throw new FaultException<ControllerException>(excepcion);
            //        }

            //        else
            //        {
            //            //este usuario existe pero no tiene usuario asignado,
            //            //desactivo usuario
            //            SERepositorio.Instancia.DesactivarUsuario(credencial);
            //        }
            //    }

            //    using (TransactionScope trans = new TransactionScope())
            //    {
            //        SERepositorio.Instancia.ModificarUsuarioBD(credencial, true);
            //        if (credencial.TipoUsuario == COConstantesModulos.USUARIO_LDAP)
            //        {
            //            LDAP.SEAdminUsuarioLDAP.Instancia.EditarUsuario(credencial);
            //        }

            //        SERepositorio.Instancia.DesautorizarCentrosServicio(credencial.IdCodigoUsuario);
            //        if (credencial.CentrosDeServicioAutorizados != null)
            //        {
            //            foreach (SECentroServicio centroServicio in credencial.CentrosDeServicioAutorizados)
            //            {
            //                SERepositorio.Instancia.AutorizarCentroServicio(centroServicio, credencial.IdCodigoUsuario);
            //            }
            //        }

            //        SERepositorio.Instancia.DesautorizarGestiones(credencial.IdCodigoUsuario);
            //        if (credencial.GestionesAutorizadas != null)
            //        {
            //            foreach (SEGestion gestion in credencial.GestionesAutorizadas)
            //            {
            //                SERepositorio.Instancia.AutorizarGestion(gestion, credencial.IdCodigoUsuario);
            //            }
            //        }

            //        SERepositorio.Instancia.DesautorizarSucursales(credencial.IdCodigoUsuario);
            //        if (credencial.SucursalesAutorizadas != null)
            //        {
            //            foreach (SESucursal sucursal in credencial.SucursalesAutorizadas)
            //            {
            //                SERepositorio.Instancia.AutorizarSucursal(sucursal, credencial.IdCodigoUsuario);
            //            }
            //            if (credencial.SucursalesAutorizadas.Count > 0)
            //            {
            //                credencial.EsUsuarioInterno = false;
            //            }
            //            else
            //            {
            //                credencial.EsUsuarioInterno = true;
            //            }
            //        }
            //        else { credencial.EsUsuarioInterno = true; }
            //        trans.Complete();
            //    }
            //}

            //#endregion Modificar usuario

            #endregion Anterior
        }
        
        /// <summary>
        /// Metodo de Creacion de Un Usuario Nuevo
        /// </summary>
        /// <param name="credencial"></param>
        private void CrearNuevoUsuario(SEAdminUsuario credencial)
        {
            //using (TransactionScope trans = new TransactionScope())
            //{
            //    if (credencial.EsUsuarioInterno)
            //    {
            //        credencial.IdPersonaInterna = SERepositorio.Instancia.AdicionarPersonaInterna(credencial);
            //    }
            //    else
            //    {
            //        credencial.IdPersonaInterna = SERepositorio.Instancia.AdicionarPersonaExterna(credencial);
            //    }
            //    credencial.IdCodigoUsuario = SERepositorio.Instancia.AdicionarUsuarioSeg(credencial);

            //    if (credencial.EsUsuarioInterno)
            //    {
            //        if (SERepositorio.Instancia.ValidarPersonaInternaExiste(credencial.TipoIdentificacion, credencial.Identificacion))
            //            SERepositorio.Instancia.ModificarUsuarioBD(credencial, false);
            //        else
            //            SERepositorio.Instancia.AdicionarUsuarioPersonaInterna(credencial, false);
            //    }
            //    else
            //    {
            //        if (SERepositorio.Instancia.ValidarPersonaInternaExiste(credencial.TipoIdentificacion, credencial.Identificacion))
            //            SERepositorio.Instancia.ModificarUsuarioBD(credencial, false);
            //        else
            //            SERepositorio.Instancia.AdicionarUsuarioPersonaExterna(credencial);
            //    }
            //    if (credencial.TipoUsuario == COConstantesModulos.USUARIO_LDAP)
            //    {
            //        LDAP.SEAdminUsuarioLDAP.Instancia.CrearUsuario(credencial);
            //    }
            //    else if (credencial.TipoUsuario == COConstantesModulos.USUARIO_APLICACION)
            //    {
            //        SERepositorio.Instancia.AdicionarCredencialUsuario(credencial);
            //    }

            //    SERepositorio.Instancia.DesautorizarCentrosServicio(credencial.IdCodigoUsuario);
            //    if (credencial.CentrosDeServicioAutorizados != null)
            //    {
            //        foreach (SECentroServicio centroServicio in credencial.CentrosDeServicioAutorizados)
            //        {
            //            SERepositorio.Instancia.AutorizarCentroServicio(centroServicio, credencial.IdCodigoUsuario);
            //        }
            //    }

            //    SERepositorio.Instancia.DesautorizarGestiones(credencial.IdCodigoUsuario);
            //    if (credencial.GestionesAutorizadas != null)
            //    {
            //        foreach (SEGestion gestion in credencial.GestionesAutorizadas)
            //        {
            //            SERepositorio.Instancia.AutorizarGestion(gestion, credencial.IdCodigoUsuario);
            //        }
            //    }

            //    SERepositorio.Instancia.DesautorizarSucursales(credencial.IdCodigoUsuario);
            //    if (credencial.SucursalesAutorizadas != null)
            //    {
            //        foreach (SESucursal sucursal in credencial.SucursalesAutorizadas)
            //        {
            //            SERepositorio.Instancia.AutorizarSucursal(sucursal, credencial.IdCodigoUsuario);
            //        }
            //        if (credencial.SucursalesAutorizadas.Count > 0)
            //        {
            //            credencial.EsUsuarioInterno = false;
            //        }
            //        else
            //        {
            //            credencial.EsUsuarioInterno = true;
            //        }
            //    }
            //    else { credencial.EsUsuarioInterno = true; }

            //    trans.Complete();
            //}
        }

        /// <summary>
        /// Obtiene los usuarios internos activos
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SEAdminUsuario> ObtenerUsuariosInternosActivos()
        {
            return SEConsultas.Instancia.ObtenerUsuariosInternosActivos();
        }

        /// <summary>
        /// Valida si un usuario existe en el directorio activo
        /// </summary>
        /// <param name="credencial"></param>
        /// <returns></returns>
        public bool ValidarUsuario(SECredencialUsuario credencial)
        {
            return SEProveedorLDAP.Instancia.ValidarUsuario(credencial);
        }

        /// <summary>
        /// Obtener pais por la Localidad
        /// </summary>
        /// <param name="idlocalidad"></param>
        /// <returns></returns>
        public PALocalidadDC ObtenerPaisPorLocalidad(string idlocalidad)
        {
            return SERepositorio.Instancia.ObtenerPaisPorLocalidad(idlocalidad);
        }

        /// <summary>
        /// Obteners the id usuario.
        /// </summary>
        /// <param name="Usuario">The usuario.</param>
        /// <returns></returns>
        public long ObtenerIdUsuario(string Usuario)
        {
            return SERepositorio.Instancia.ObtenerIdUsuario(Usuario);
        }

        /// <summary>
        /// Consulta los reportes asociados a un rol específico.
        /// </summary>
        /// <param name="rol"></param>
        /// <returns></returns>
        public SERolReportes ConsultarReportesxRol(SERol rol)
        {
            return SERepositorio.Instancia.ConsultarReportesxRol(rol);
        }

        /// <summary>
        /// Almacena en la base de datos los reportes asociados a un rol específico
        /// </summary>
        /// <param name="reporte"></param>
        /// <param name="rol"></param>
        public void GuardarReportesRol(SERolReportes reportesRol)
        {
            using (TransactionScope transaccion = new TransactionScope())
            {
                RemoverReportesRol(reportesRol.Rol);
                foreach (REPInfoReporte repInfo in reportesRol.Reportes)
                {
                    SERepositorio.Instancia.GuardarReporteRol(repInfo, reportesRol.Rol);
                }
                transaccion.Complete();
            }
        }

        /// <summary>
        /// remueve los reportes asociados a un rol específico.
        /// </summary>
        /// <param name="reporte"></param>
        /// <param name="rol"></param>
        private void RemoverReportesRol(SERol rol)
        {
            SERepositorio.Instancia.RemoverReportesRol(rol);
        }

        #region Administración Maquinas

        /// <summary>
        /// Obtener las maquinas
        /// </summary>
        /// <param name="filtro">Filtro</param>
        /// <param name="campoOrdenamiento">Campo de ordenamiento de la grilla</param>
        /// <param name="indicePagina">Indice de pagina de la grilla</param>
        /// <param name="registrosPorPagina">Registros por pagina de la grilla</param>
        /// <param name="ordenamientoAscendente">Ordenamiento ascendente</param>
        /// <param name="totalRegistros">Total de registros</param>
        /// <returns></returns>
        public IEnumerable<SEMaquinaVersion> ObtenerMaquinas(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return SERepositorio.Instancia.ObtenerMaquinas(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Edita estado de las maquinas
        /// </summary>
        /// <param name="credencial">Objeto tipo maquina</param>
        public void EditarMaquina(SEMaquinaVersion maquina)
        {
            SERepositorio.Instancia.EditarMaquina(maquina);
        }

        /// <summary>
        /// Obtiene posibles estados
        /// </summary>
        /// <returns></returns>
        public List<SEEstadoUsuario> ObtenerTiposEstado()
        {
            return SERepositorio.Instancia.ObtenerEstadoUsuario();
        }

        /// <summary>
        /// Elimina el registro de la Caja del Punto
        /// por Id de Caja y por Centro de Servicio.
        /// </summary>
        /// <param name="idPunto">The id punto.</param>
        /// <param name="idCaja">The id caja.</param>
        public void EliminarCajaPunto(long idPunto, int idCaja)
        {
            SERepositorio.Instancia.EliminarCajaPunto(idPunto, idCaja);
        }

        #endregion Administración Maquinas

        #region Administración Cargos

        /// <summary>
        /// Obtener los cargos para el filtro
        /// </summary>
        /// <param name="filtro">Filtro</param>
        /// <param name="campoOrdenamiento">Campo de ordenamiento de la grilla</param>
        /// <param name="indicePagina">Indice de pagina de la grilla</param>
        /// <param name="registrosPorPagina">Registros por pagina de la grilla</param>
        /// <param name="ordenamientoAscendente">Ordenamiento ascendente</param>
        /// <param name="totalRegistros">Total de registros</param>
        /// <returns></returns>
        public IEnumerable<SECargo> ObtenerCargosFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return SERepositorio.Instancia.ObtenerCargosFiltro(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Adicionar, modificar o eliminar cargos
        /// </summary>
        /// <param name="rol">Objeto cargo</param>
        public void ActualizarCargos(SECargo cargo)
        {
            if (cargo.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
            {
                SERepositorio.Instancia.AdicionarCargo(cargo);
            }
            else if (cargo.EstadoRegistro == EnumEstadoRegistro.MODIFICADO)
            {
                SERepositorio.Instancia.EditarCargo(cargo);
            }
            else if (cargo.EstadoRegistro == EnumEstadoRegistro.BORRADO)
            {
                SERepositorio.Instancia.EliminarCargo(cargo);
            }
            else
            {
                //
            }
        }

        /// <summary>
        /// Obtener lista con los cargos
        /// </summary>
        /// <returns>Objeto cargo de tipo lista</returns>
        public IList<SECargo> ObtenerCargos()
        {
            return SERepositorio.Instancia.ObtenerCargos();
        }

        /// <summary>
        /// Metodo para obtener los hijos de un cargo
        /// </summary>
        /// <param name="cargo"></param>
        /// <returns>Objeto cargo de tipo lista</returns>
        private List<SECargo> ObtenerHijos(SECargo cargo)
        {
            if (CargosHijo == null)
                CargosHijo = new List<SECargo>();
            var Lista = SERepositorio.Instancia.ObtenerHijo(cargo).ToList();
            if (Lista.Count != 0)
            {
                CargosHijo.AddRange(Lista);
                Lista.ForEach(r =>
                  {
                      this.ObtenerHijos(r);
                  });
            }
            else
            {
                return Lista;
            }
            CargosHijo.Add(cargo);
            return CargosHijo;
        }

        /// <summary>
        /// Metodo para obtener el padre de la lista
        /// </summary>
        /// <param name="cargo"></param>
        /// <returns>Objeto cargo padre de la lista</returns>
        private SECargo ObtenerPadre()
        {
            var padre = SERepositorio.Instancia.ObtenerPadre();

            return padre;
        }

        /// <summary>
        /// Metodo para obtener los posibles cargos padre de un cargo
        /// </summary>
        /// <param name="cargo"></param>
        /// <returns>Objeto cargo de tipo lista</returns
        public IList<SECargo> ObtenerCargosPosibles(SECargo cargo)
        {
            var Hijos = ObtenerHijos(cargo).Distinct();
            var Todos = ObtenerCargos();
            CargosHijo = null;
            return Todos.Except(Hijos, new COLambdaComparer<SECargo>((x, y) => x.IdCargo == y.IdCargo)).ToList();
        }

        /// <summary>
        /// Metodo que obtiene los subordinados de  los hijos del padre
        /// </summary>
        /// <param name="padre"></param>
        /// <returns>Objeto tipo cargo con toda la informacion</returns>
        private SECargo ObtieneSubordinados(SECargo padre)
        {
            if (padre.Subordinados != null)
            {
                padre.Subordinados.ForEach(obj =>
                {
                    obj.Subordinados = SERepositorio.Instancia.ObtenerHijo(obj);
                    ObtieneSubordinados(obj);
                });
            }
            return padre;
        }

        /// <summary>
        /// Metodo para obtener los cargos en una lista para alimentar el organigrama
        /// </summary>
        /// <returns>Objeto tipo cargo con toda la informacion</returns>
        public ObservableCollection<SECargo> ObtenerCargosOrganigrama()
        {
            lista.Clear();
            if (padre.IdCargo == 0)
            {
                padre = ObtenerPadre();
            }
            padre.Subordinados = SERepositorio.Instancia.ObtenerHijo(padre);
            lista.Add(ObtieneSubordinados(padre));
            return lista;
        }

        /// <summary>
        /// Valida si el cargo que va a autorizar la actividad pueda hacerlo
        /// </summary>
        /// <param name="idCargoAutoriza">Id del cargo que va a realizar la autorizacion</param>
        /// <param name="idCargoAutenticado">Id del cargo</param>
        /// <returns>true si esta autorizado, false si no lo esta</returns>
        public bool ValidaCargoParaAutorizar(int idCargoAutoriza, int idCargoAutenticado)
        {
            return SERepositorio.Instancia.ValidaCargoParaAutorizar(idCargoAutoriza, idCargoAutenticado);
        }

        #endregion Administración Cargos

        #region Consultas

        /// <summary>
        /// Retorna los tipos de autenticación
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SETipoAutenticacion> ObtenerTiposAutenticacion()
        {
            return SEConsultas.Instancia.ObtenerTiposAutenticacion();
        }

        /// <summary>
        /// Obtiene a los Cajeros Auxiliares de un Centro de Servicio.
        /// </summary>
        /// <param name="idCentroServicio">The id centro servicio.</param>
        /// <returns></returns>
        public List<SEUsuarioCentroServicioDC> ObtenerCajeroCentroServicio(long idCentroServicio, string idRol)
        {
            return SEConsultas.Instancia.ObtenerCajeroCentroServicio(idCentroServicio, idRol);
        }

        /// <summary>
        /// Consulta las cajas con los usuarios de un punto trayendo tambien
        ///	las cajas no utilizadas
        /// </summary>
        /// <param name="idCentroSvc">id punto centro servicio</param>
        /// <returns>lista de cajas del punto centro servicio</returns>
        public List<SEUsuarioCentroServicioDC> ObtenerCajerosCajaPorPunto(long idCentroSvc)
        {
            return SERepositorio.Instancia.ObtenerCajerosCajaPorPunto(idCentroSvc);
        }

        /// <summary>
        /// Obtiene la info de la persona interna por el codigo del
        /// usuario
        /// </summary>
        /// <param name="idCodigoUsuario"></param>
        /// <returns></returns>
        public SEUsuarioPorCodigoDC ObtenerUsuarioPorCodigo(long idCodigoUsuario)
        {
            return SEConsultas.Instancia.ObtenerUsuarioPorCodigo(idCodigoUsuario);
        }

        /// <summary>
        /// Valida la persona interna si
        /// tiene usuario asignado
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns>Info del Usuario</returns>
        public void ValidarUsuario(string idUsuario, string identificacion)
        {
            SEConsultas.Instancia.ValidarUsuario(idUsuario, identificacion);
        }

        /// <summary>
        /// se consulta la persona por la identificacion
        /// </summary>
        /// <param name="identificacion"></param>
        /// <returns>datos del usuario encontrado</returns>
        public SEPersonaInternaDC ObtenerPersonaInternaPorIdentificacion(string identificacion)
        {
            return SEConsultas.Instancia.ObtenerPersonaInternaPorIdentificacion(identificacion);
        }

        /// <summary>
        /// Método para validar los usuarios creados por el app
        /// </summary>
        /// <param name="numeroCedula"></param>
        /// <returns></returns>
        public int ValidarUsuarioPersonaInternaAPP(string numeroCedula)
        {
            return SERepositorio.Instancia.ValidarUsuarioPersonaInternaAPP(numeroCedula);
        }



        #endregion Consultas
        #region Control Usuarios Integracion 
        ///<summary>
        /// Crear usuario de integracion valida que el usuario no exista y encripta la contraseña
        ///</summary>    
        ///<param name="SEUsuarioIntegracion"></param>
        ///<returns>Objeto que compuesto por valor (Corresponde al id de usuario que fue asignado) y mensaje </returns>
        public SERespuestaProceso CrearUsuarioIntegracion(SEUsuarioIntegracionDC usuario)
        {
            return SEIntegracion.Instancia.CrearUsuarioIntegracion(usuario);
        }
        ///<summary>
        /// Modifica las credenciales del usuario 
        ///</summary>    
        ///<param name="SEUsuarioIntegracion"></param>
        ///<returns>Objeto que compuesto por valor y mensaje </returns>
        public SERespuestaProceso EditarUsuarioIntegracion(SEUsuarioIntegracionDC usuario)
        {
            return SEIntegracion.Instancia.EditarUsuarioIntegracion(usuario);
        }
        ///<summary>
        /// Deshabilita el usuario de integracion
        ///</summary>    
        ///<param name="SEUsuarioIntegracion"></param>
        ///<returns>Objeto que compuesto por valor y mensaje </returns>
        public SERespuestaProceso DeshabilitarUsuarioIntegracion(SEUsuarioIntegracionDC usuario)
        {
            return SEIntegracion.Instancia.DeshabilitarUsuarioIntegracion(usuario);
        }
        ///<summary>
        /// Consulta el usuario validando si el usuario y la contraseña coincide
        ///</summary>    
        ///<param name="SEUsuarioIntegracion"></param>
        ///<returns>Objeto que compuesto por valor (corresponde al id de usuario) y mensaje </returns>
        public SERespuestaProceso ConsultarUsuarioIntegracion(SEUsuarioIntegracionDC usuario)
        {
            return SEIntegracion.Instancia.ConsultarUsuarioIntegracion(usuario);
        }
        public List<SEUsuarioIntegracionDC> ObtenerUsuariosIntegracion(int idCliente)
        {
            return SEIntegracion.Instancia.ObtenerUsuariosIntegracion(idCliente);
        }
        #endregion
    }
}