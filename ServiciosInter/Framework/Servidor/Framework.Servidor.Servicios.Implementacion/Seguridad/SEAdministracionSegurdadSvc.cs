using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Threading;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Seguridad;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using Framework.Servidor.Servicios.ContratoDatos.Reportes;
using Framework.Servidor.Servicios.ContratoDatos.Seguridad;
using Framework.Servidor.Servicios.Contratos;

namespace Framework.Servidor.Servicios.Implementacion.Seguridad
{
    /// <summary>
    /// Clase para los servicios de administración de seguridad
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    internal class SEAdministracionSegurdadSvc : ISEAdministracionSeguridadSvc
    {
        public SEAdministracionSegurdadSvc()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(PAAdministrador.Instancia.ConsultarParametrosFramework("Cultura"));
        }

        /// <summary>
        /// Obtener todos los roles que estan configurados en la base de datos
        /// </summary>
        /// <returns>Colección con los roles configurados en la base de datos</returns>
        public Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<ContratoDatos.Seguridad.SERol> ObtenerRoles(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente)
        {
            int totalRegistros = 0;
            return new ContratoDatos.GenericoConsultasFramework<SERol>()
            {
                Lista = SEAdministradorSeguridad.Instancia.ObtenerRoles(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Borra Roles
        /// </summary>
        /// <param name="rol"></param>
        public void BorrarRol(ContratoDatos.Seguridad.SERol rol)
        {
            SEAdministradorSeguridad.Instancia.BorrarRol(rol);
        }

        /// <summary>
        /// Cambia password de un usuario en la DB
        /// </summary>
        /// <param name="credencial"></param>
        public void CambiarPassword(SECredencialUsuario credencial)
        {
            SEAdministradorSeguridad.Instancia.CambiarPassword(credencial);
        }

        /// <summary>
        /// Obtiene una lista de roles por menu
        /// </summary>
        /// <param name="rol"></param>
        /// <returns></returns>
        public ContratoDatos.Seguridad.SEMenuRolAccion ObtenerMenuRol(SERol rol)
        {
            return SEAdministradorSeguridad.Instancia.ObtenerMenuRol(rol);
        }

        /// <summary>
        /// Cargar las acciones desde la configuración de base de datos
        /// </summary>
        /// <returns>Lista con las acciones</returns>
        public List<ContratoDatos.Seguridad.SEAccion> ObtenerAcciones()
        {
            return SEAdministradorSeguridad.Instancia.ObtenerAcciones();
        }

        /// <summary>
        /// Obtiene los usuarios internos activos
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SEAdminUsuario> ObtenerUsuariosInternosActivos()
        {
            return SEAdministradorSeguridad.Instancia.ObtenerUsuariosInternosActivos();
        }

        /// <summary>
        /// Guarda Menu Rol Accion
        /// </summary>
        /// <param name="menuRolAccion"></param>
        public void GuardarMenuRolAccion(SEMenuRolAccionConsolidado menuRolAccion)
        {
            SEAdministradorSeguridad.Instancia.GuardarMenuRolAccion(menuRolAccion);
        }

        /// <summary>
        /// Obtiene una lista de roles por usuario
        /// </summary>
        /// <param name="credencial"></param>
        /// <returns></returns>
        public ContratoDatos.Seguridad.SERolUsuario ObtenerRolesUsuario(string IdUsuario)
        {
            return SEAdministradorSeguridad.Instancia.ObtenerRolesUsuario(IdUsuario);
        }

        /// <summary>
        /// Guarda los roles asignados y sin asignar de un usuario
        /// </summary>
        /// <param name="rolUsuario"></param>
        public void GuardarRolesUsuario(SERolUsuario rolUsuario)
        {
            SEAdministradorSeguridad.Instancia.GuardarRolesUsuario(rolUsuario);
        }

        /// <summary>
        /// Obtiene los usuarios activos del sistema
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <returns></returns>
        public IEnumerable<SEAdminUsuario> ObtenerUsuariosAdmin(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente)
        {
            return SEAdministradorSeguridad.Instancia.ObtenerUsuariosAdmin(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente);
        }

        /// <summary>
        /// Desactiva usuarios
        /// </summary>
        /// <param name="credencial"></param>
        public void EliminarUsuario(ContratoDatos.Seguridad.SEAdminUsuario credencial)
        {
            SEAdministradorSeguridad.Instancia.EliminarUsuario(credencial);
        }

        /// <summary>
        /// Obtiene los menus de la DB
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <returns></returns>
        public Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<ContratoDatos.Seguridad.SEMenu> ObtenerMenus(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente)
        {
            int totalRegistros = 0;
            return new ContratoDatos.GenericoConsultasFramework<SEMenu>()
            {
                Lista = SEAdministradorSeguridad.Instancia.ObtenerMenus(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Elimina Menus de la base de datos
        /// </summary>
        /// <param name="menu">Objeto menu</param>
        public void EliminarMenu(ContratoDatos.Seguridad.SEMenu menu)
        {
            SEAdministradorSeguridad.Instancia.EliminarMenu(menu);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public SEAdminUsuario ListasAdminUsuario()
        {
            return SEAdministradorSeguridad.Instancia.ListasAdminUsuario();
        }

        /// <summary>
        /// retorna la lista de centros de servicios de tipo PAS por ciudad
        /// </summary>
        /// <param name="idCiudad"></param>
        /// <returns></returns>
        public List<SECentroServicio> ObtenerCentroServicioPASPorRacol(long idRacol)
        {
            return SEAdministradorSeguridad.Instancia.ObtenerCentroServicioPASPorRacol(idRacol);
        }

        /// <summary>
        /// Obtiene los modulos de la base de datos
        /// </summary>
        /// <returns></returns>
        public List<SEModulo> ObtenerModulos()
        {
            return SEAdministradorSeguridad.Instancia.ObtenerModulos();
        }

        /// <summary>
        /// Método para obtener la localidad de un usuario
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        public PALocalidadDC ObtenerLocalidadPorUsuario(string idUsuario)
        {
            return SEAdministradorSeguridad.Instancia.ObtenerLocalidadPorUsuario(idUsuario);
        }

        /// <summary>
        /// Adiciona o elimina menús
        /// </summary>
        /// <param name="menu">Objeto menú</param>
        public void ActualizarAdminMenu(SEMenu menu)
        {
            SEAdministradorSeguridad.Instancia.ActualizarAdminMenu(menu);
        }

        /// <summary>
        /// Adiciona o elimina roles
        /// </summary>
        /// <param name="rol">Objeto rol</param>
        public void ActualizarAdminRoles(SERol rol)
        {
            SEAdministradorSeguridad.Instancia.ActualizarAdminRoles(rol);
        }

        /// <summary>
        /// Adiciona o elimina usuarios
        /// </summary>
        /// <param name="credencial">Objeto Credencial</param>
        public void ActualizarAdminUsuarios(SEAdminUsuario credencial)
        {
            SEAdministradorSeguridad.Instancia.ActualizarAdminUsuarios(credencial);
        }

        /// <summary>
        /// Obtener pais por la Localidad
        /// </summary>
        /// <param name="idlocalidad"></param>
        /// <returns></returns>
        public PALocalidadDC ObtenerPaisPorLocalidad(string idlocalidad)
        {
            return SEAdministradorSeguridad.Instancia.ObtenerPaisPorLocalidad(idlocalidad);
        }

        /// <summary>
        /// Obteners the id usuario.
        /// </summary>
        /// <param name="Usuario">The usuario.</param>
        /// <returns></returns>
        public long ObtenerIdUsuario(string Usuario)
        {
            return SEAdministradorSeguridad.Instancia.ObtenerIdUsuario(Usuario);
        }

        public List<SEClienteCredito> ObtenerClientesCredito()
        {
            return SEAdministradorSeguridad.Instancia.ObtenerClientesCredito();
        }

        public List<SESucursal> ObtenerSucursalesClientesCredito(int IdCliente)
        {
            return SEAdministradorSeguridad.Instancia.ObtenerSucursalesClientesCredito(IdCliente);
        }

        public List<SECasaMatriz> ObtenerCasasMatriz()
        {
            return SEAdministradorSeguridad.Instancia.ObtenerCasasMatriz();
        }

        public List<SEMacroproceso> ObtenerMacroProcesos(short idCasaMatriz)
        {
            return SEAdministradorSeguridad.Instancia.ObtenerMacroProcesos(idCasaMatriz);
        }

        public List<SEGestion> ObtenerGestiones(string idMacroProceso)
        {
            return SEAdministradorSeguridad.Instancia.ObtenerGestiones(idMacroProceso);
        }

        public List<SECentroServicio> ObtenerCentrosDeServicioxCiudad(string idCiudad)
        {
            return SEAdministradorSeguridad.Instancia.ObtenerCentrosDeServicioxCiudad(idCiudad);
        }

        /// <summary>
        /// Consulta los reportes asociados a un rol específico.
        /// </summary>
        /// <param name="rol"></param>
        /// <returns></returns>
        public SERolReportes ConsultarReportesxRol(SERol rol)
        {
            return SEAdministradorSeguridad.Instancia.ConsultarReportesxRol(rol);
        }

        /// <summary>
        /// Almacena en la base de datos los reportes asociados a un rol específico
        /// </summary>
        /// <param name="reporte"></param>
        /// <param name="rol"></param>
        public void GuardarReportesRol(SERolReportes reportesRol)
        {
            SEAdministradorSeguridad.Instancia.GuardarReportesRol(reportesRol);
        }

        /// <summary>
        /// Obtiene la info de la persona interna por el codigo del
        /// usuario
        /// </summary>
        /// <param name="idCodigoUsuario"></param>
        /// <returns>Info del usuario</returns>
        public SEUsuarioPorCodigoDC ObtenerUsuarioPorCodigo(long idCodigoUsuario)
        {
            return SEAdministradorSeguridad.Instancia.ObtenerUsuarioPorCodigo(idCodigoUsuario);
        }

        /// <summary>
        /// Obtiene la info de la persona interna por el id del
        /// usuario
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns>Info del Usuario</returns>
        public void ValidarUsuario(string idUsuario, string identificacion)
        {
            SEAdministradorSeguridad.Instancia.ValidarUsuario(idUsuario, identificacion);
        }

        /// <summary>
        /// se consulta la persona por la identificacion
        /// </summary>
        /// <param name="identificacion"></param>
        /// <returns>datos del usuario encontrado</returns>
        public SEPersonaInternaDC ObtenerPersonaInternaPorIdentificacion(string identificacion)
        {
            return SEAdministradorSeguridad.Instancia.ObtenerPersonaInternaPorIdentificacion(identificacion);
        }

        /// <summary>
        /// Consulta las cajas con los usuarios de un punto trayendo tambien
        ///	las cajas no utilizadas
        /// </summary>
        /// <param name="idCentroSvc">id punto centro servicio</param>
        /// <returns>lista de cajas del punto centro servicio</returns>
        public List<SEUsuarioCentroServicioDC> ObtenerCajerosCajaPorPunto(long idCentroSvc)
        {
            return SEAdministradorSeguridad.Instancia.ObtenerCajerosCajaPorPunto(idCentroSvc);
        }

        #region Administración de maquinas

        /// <summary>
        /// Obtiene las maquinas de la DB
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <returns></returns>
        public Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<ContratoDatos.Seguridad.SEMaquinaVersion> ObtenerMaquinas(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente)
        {
            int totalRegistros = 0;
            return new ContratoDatos.GenericoConsultasFramework<SEMaquinaVersion>()
            {
                Lista = SEAdministradorSeguridad.Instancia.ObtenerMaquinas(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Edita estado de maquina
        /// </summary>
        /// <param name="maquina">Objeto tipo maquina</param>
        public void EditarMaquina(SEMaquinaVersion maquina)
        {
            SEAdministradorSeguridad.Instancia.EditarMaquina(maquina);
        }

        /// <summary>
        /// obtiene los posibles estados
        /// </summary>
        public List<SEEstadoUsuario> ObtenerTiposEstado()
        {
            return SEAdministradorSeguridad.Instancia.ObtenerTiposEstado();
        }

        #endregion Administración de maquinas

        #region Administración de cargos

        /// <summary>
        /// Obtiene los cargos de la DB
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <returns></returns>
        public Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<ContratoDatos.Seguridad.SECargo> ObtenerCargosFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente)
        {
            int totalRegistros = 0;
            return new ContratoDatos.GenericoConsultasFramework<SECargo>()
            {
                Lista = SEAdministradorSeguridad.Instancia.ObtenerCargosFiltro(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Adiciona, edita o elimina cargos
        /// </summary>
        /// <param name="credencial">Objeto cargo</param>
        public void ActualizarCargos(SECargo cargo)
        {
            SEAdministradorSeguridad.Instancia.ActualizarCargos(cargo);
        }

        /// <summary>
        /// Obtiene los cargos
        /// </summary>
        /// <returns></returns>
        public IList<SECargo> ObtenerCargos()
        {
            return SEAdministradorSeguridad.Instancia.ObtenerCargos();
        }

        /// <summary>
        /// Obtiene los cargos posibles
        /// </summary>
        /// <returns></returns>
        public IList<SECargo> ObtenerCargosPosibles(SECargo cargo)
        {
            var tmp = SEAdministradorSeguridad.Instancia.ObtenerCargosPosibles(cargo);
            return tmp;
        }

        /// <summary>
        /// Obtiene los cargos del organigrama
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<SECargo> ObtenerCargosOrganigrama()
        {
            return SEAdministradorSeguridad.Instancia.ObtenerCargosOrganigrama();
        }

        /// <summary>
        /// Valida si el cargo que va a autorizar la actividad pueda hacerlo
        /// </summary>
        /// <param name="idCargoAutoriza">Id del cargo que va a realizar la autorizacion</param>
        /// <param name="idCargoAutenticado">Id del cargo</param>
        /// <returns>true si esta autorizado, false si no lo esta</returns>
        public bool ValidaCargoParaAutorizar(int idCargoAutoriza, int idCargoAutenticado)
        {
            return SEAdministradorSeguridad.Instancia.ValidaCargoParaAutorizar(idCargoAutoriza, idCargoAutenticado);
        }

        #endregion Administración de cargos
        #region Control Usuarios Integracion 
        ///<summary>
        /// Crear usuario de integracion valida que el usuario no exista y encripta la contraseña
        ///</summary>    
        ///<param name="SEUsuarioIntegracion"></param>
        ///<returns>Objeto que compuesto por valor (Corresponde al id de usuario que fue asignado) y mensaje </returns>
        public SERespuestaProceso CrearUsuarioIntegracion(SEUsuarioIntegracionDC usuario)
        {
            return SEAdministradorSeguridad.Instancia.CrearUsuarioIntegracion(usuario);
        }
        ///<summary>
        /// Modifica las credenciales del usuario 
        ///</summary>    
        ///<param name="SEUsuarioIntegracion"></param>
        ///<returns>Objeto que compuesto por valor y mensaje </returns>
        public SERespuestaProceso EditarUsuarioIntegracion(SEUsuarioIntegracionDC usuario)
        {
            return SEAdministradorSeguridad.Instancia.EditarUsuarioIntegracion(usuario);
        }
        ///<summary>
        /// Deshabilita el usuario de integracion
        ///</summary>    
        ///<param name="SEUsuarioIntegracion"></param>
        ///<returns>Objeto que compuesto por valor y mensaje </returns>
        public SERespuestaProceso DeshabilitarUsuarioIntegracion(SEUsuarioIntegracionDC usuario)
        {
            return SEAdministradorSeguridad.Instancia.DeshabilitarUsuarioIntegracion(usuario);
        }
        ///<summary>
        /// Consulta el usuario validando si el usuario y la contraseña coincide
        ///</summary>    
        ///<param name="SEUsuarioIntegracion"></param>
        ///<returns>Objeto que compuesto por valor (corresponde al id de usuario) y mensaje </returns>
        public SERespuestaProceso ConsultarUsuarioIntegracion(SEUsuarioIntegracionDC usuario)
        {
            return SEAdministradorSeguridad.Instancia.ConsultarUsuarioIntegracion(usuario);
        }
        public List<SEUsuarioIntegracionDC> ObtenerUsuariosIntegracion(int idCliente)
        {
            return SEAdministradorSeguridad.Instancia.ObtenerUsuariosIntegracion(idCliente);
        }
        #endregion

    }
}