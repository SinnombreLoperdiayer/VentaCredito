using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using Framework.Servidor.Servicios.ContratoDatos.Reportes;
using Framework.Servidor.Servicios.ContratoDatos.Seguridad;

namespace Framework.Servidor.Servicios.Contratos
{
    [ServiceContract(Namespace = "http://contrologis.com")]
    public interface ISEAdministracionSeguridadSvc
    {
        /// <summary>
        /// Obtener todos los roles que estan configurados en la base de datos
        /// </summary>
        /// <returns>Colección con los roles configurados en la base de datos</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<ContratoDatos.Seguridad.SERol> ObtenerRoles(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente);

        /// <summary>
        /// Borra Roles en la Base de Datos
        /// </summary>
        /// <param name="rol">Objeto rol</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void BorrarRol(SERol rol);

        /// <summary>
        /// Cambia Contraseña de la DB
        /// </summary>
        /// <param name="credencial">Objeto Credencial</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void CambiarPassword(SECredencialUsuario credencial);

        /// <summary>
        /// Obtener los menús asignados a los roles
        /// </summary>
        /// <param name="rol">Objeto rol</param>
        /// <returns>Retorna una lista con los roles asignados y sin asignar</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        SEMenuRolAccion ObtenerMenuRol(SERol rol);

        /// <summary>
        /// Cargar las acciones desde la configuración de base de datos
        /// </summary>
        /// <returns>Lista con las acciones</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ContratoDatos.Seguridad.SEAccion> ObtenerAcciones();

        /// <summary>
        /// Guarda Menu Rol Accion
        /// </summary>
        /// <param name="menuRolAccion"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GuardarMenuRolAccion(SEMenuRolAccionConsolidado menuRolAccion);

        /// <summary>
        ///Obtiene los modulos y menus por usuario
        /// </summary>
        /// <param name="credencial">Objeto credencial</param>
        /// <returns>Retorna una lista con los modulos y menús asignados al usuario</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        SERolUsuario ObtenerRolesUsuario(string IdUsuario);

        /// <summary>
        /// Guarda los roles asignados y sin asignar de un usuario
        /// </summary>
        /// <param name="rolUsuario"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GuardarRolesUsuario(SERolUsuario rolUsuario);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        PALocalidadDC ObtenerLocalidadPorUsuario(string idUsuario);

        /// <summary>
        ///Obtiene una lista con los usuarios activos
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<SEAdminUsuario> ObtenerUsuariosAdmin(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente);

        /// <summary>
        ///Elimina usuarios
        /// </summary>
        /// <param name="credencial"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void EliminarUsuario(SEAdminUsuario credencial);

        /// <summary>
        /// Obtener los menus que estan configurados en la base de datos
        /// </summary>
        /// <returns>Colección con los menus configurados en la base de datos</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<ContratoDatos.Seguridad.SEMenu> ObtenerMenus(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente);

        /// <summary>
        /// Borra menus en la Base de Datos
        /// </summary>
        /// <param name="menu">Objeto menu</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void EliminarMenu(SEMenu menu);

        /// <summary>
        ///Obtiene las listas para la administración de usuarios
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        SEAdminUsuario ListasAdminUsuario();

        /// <summary>
        /// Obtiene los centros de servicio que esta denominados como PAS
        /// </summary>
        /// <param name="idCiudad"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<SECentroServicio> ObtenerCentroServicioPASPorRacol(long idRacol);

        /// <summary>
        /// Obtiene los modulos de la DB
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<SEModulo> ObtenerModulos();

        /// <summary>
        /// Adiciona o edita menus
        /// </summary>
        /// <param name="menu"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizarAdminMenu(SEMenu menu);

        /// <summary>
        /// Adiciona o edita roles
        /// </summary>
        /// <param name="rol"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizarAdminRoles(SERol rol);

        /// <summary>
        /// Adiciona o edita usuarios
        /// </summary>
        /// <param name="credencial"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizarAdminUsuarios(SEAdminUsuario credencial);
        
        /// <summary>
        /// Obtener las maquinas que estan configuradas en la base de datos
        /// </summary>
        /// <returns>Colección con las maquinas configuradas en la base de datos</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<ContratoDatos.Seguridad.SEMaquinaVersion> ObtenerMaquinas(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente);

        /// <summary>
        /// Edita estado de la maquina
        /// </summary>
        /// <param name="maquina"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void EditarMaquina(SEMaquinaVersion maquina);

        /// <summary>
        /// estados posibles de la maquina
        /// </summary>
        /// <param name="maquina"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<SEEstadoUsuario> ObtenerTiposEstado();

        /// <summary>
        /// Obtener los cargos que estan configuradas en la base de datos
        /// </summary>
        /// <returns>Colección con las cargos configuradas en la base de datos</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<ContratoDatos.Seguridad.SECargo> ObtenerCargosFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente);

        /// <summary>
        /// Adiciona, edita o elimina cargos
        /// </summary>
        /// <param name="cargo"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizarCargos(SECargo cargo);

        /// <summary>
        /// Obtiene los cargos
        /// </summary>
        /// <param name="cargo"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<SECargo> ObtenerCargos();

        /// <summary>
        /// Obtiene los cargos posibles
        /// </summary>
        /// <param name="cargo"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<SECargo> ObtenerCargosPosibles(SECargo cargo);

        /// <summary>
        /// Obtiene los cargos del organigrama
        /// </summary>
        /// <param name="cargo"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ObservableCollection<SECargo> ObtenerCargosOrganigrama();

        /// <summary>
        /// Metodo que obtiene el pais segun la
        /// localidad
        /// </summary>
        /// <param name="idlocalidad"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        PALocalidadDC ObtenerPaisPorLocalidad(string idlocalidad);

        /// <summary>
        /// Obteners the id usuario.
        /// </summary>
        /// <param name="Usuario">The usuario.</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        long ObtenerIdUsuario(string Usuario);

        /// <summary>
        /// Valida si el cargo que va a autorizar la actividad pueda hacerlo
        /// </summary>
        /// <param name="idCargoAutoriza">Id del cargo que va a realizar la autorizacion</param>
        /// <param name="idCargoAutenticado">Id del cargo</param>
        /// <returns>true si esta autorizado, false si no lo esta</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        bool ValidaCargoParaAutorizar(int idCargoAutoriza, int idCargoAutenticado);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<SEClienteCredito> ObtenerClientesCredito();

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<SESucursal> ObtenerSucursalesClientesCredito(int IdCliente);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<SECasaMatriz> ObtenerCasasMatriz();

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<SEMacroproceso> ObtenerMacroProcesos(short idMacroproceso);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<SEGestion> ObtenerGestiones(string idMacroProceso);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<SECentroServicio> ObtenerCentrosDeServicioxCiudad(string idCiudad);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]

        /// <summary>
        /// Consulta los reportes asociados a un rol específico.
        /// </summary>
        /// <param name="rol"></param>
        /// <returns></returns>
        SERolReportes ConsultarReportesxRol(SERol rol);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]

        /// <summary>
        /// Almacena en la base de datos los reportes asociados a un rol específico
        /// </summary>
        /// <param name="reporte"></param>
        /// <param name="rol"></param>
        void GuardarReportesRol(SERolReportes reportesRol);

        /// <summary>
        /// Obtiene los usuarios internos activos
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<SEAdminUsuario> ObtenerUsuariosInternosActivos();

        /// <summary>
        /// Obtiene la info de la persona interna por el codigo del
        /// usuario
        /// </summary>
        /// <param name="idCodigoUsuario"></param>
        /// <returns>Info del usuario</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        SEUsuarioPorCodigoDC ObtenerUsuarioPorCodigo(long idCodigoUsuario);

        /// <summary>
        /// Obtiene la info de la persona interna por el id del
        /// usuario
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns>Info del Usuario</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ValidarUsuario(string idUsuario, string identificacion);

        /// <summary>
        /// se consulta la persona por la identificacion
        /// </summary>
        /// <param name="identificacion"></param>
        /// <returns>datos del usuario encontrado</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        SEPersonaInternaDC ObtenerPersonaInternaPorIdentificacion(string identificacion);

        /// <summary>
        /// Consulta las cajas con los usuarios de un punto trayendo tambien
        ///	las cajas no utilizadas
        /// </summary>
        /// <param name="idCentroSvc">id punto centro servicio</param>
        /// <returns>lista de cajas del punto centro servicio</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<SEUsuarioCentroServicioDC> ObtenerCajerosCajaPorPunto(long idCentroSvc);

        ///<sumary>
        ///Control de usuarios de integracion Crear, editar, deshabilitar, consultar(Loggear)
        /// </sumary>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        SERespuestaProceso CrearUsuarioIntegracion(SEUsuarioIntegracionDC usuario);
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        SERespuestaProceso EditarUsuarioIntegracion(SEUsuarioIntegracionDC usuario);
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        SERespuestaProceso DeshabilitarUsuarioIntegracion(SEUsuarioIntegracionDC usuario);
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        SERespuestaProceso ConsultarUsuarioIntegracion(SEUsuarioIntegracionDC usuario);
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<SEUsuarioIntegracionDC> ObtenerUsuariosIntegracion(int idCliente);

    }
}