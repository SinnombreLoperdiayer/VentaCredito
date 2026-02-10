using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Seguridad;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace CO.Servidor.Servicios.Contratos
{
    #region Conductores

    /// <summary>
    /// Contratos WCF de centros de servicios
    /// </summary>
    [ServiceContract(Namespace = "http://contrologis.com")]
    public interface IPOParametrosOperacionSvc
    {
        /// <summary>
        /// Consulta todos los tipos de vehiculos
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<POTipoVehiculo> ObtenerTodosTipoVehiculo();

        /// <summary>
        /// Obtiene  los conductores
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consult</param>
        /// <returns>Lista con los conductores</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GenericoConsultasFramework<POConductores> ObtenerConductores(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente);

        /// <summary>
        /// Adiciona y edita un conductor
        /// </summary>
        /// <param name="conductor"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizarConductores(POConductores conductor);

        /// <summary>
        /// Obtiene los estados para los centros de servicio
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<POEstado> ObtenerEstados();

        /// <summary>
        /// Consulta la informacion de un conductor desde novasoft
        /// </summary>
        /// <param name="cedula">Numero de cedula del conductor</param>
        /// <param name="conductor">Bandera que indica si el conductor es un empleado o un contratista</param>
        /// <returns>Objeto con la informacion del conductor</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        POConductores ObtenerConductorNovasoft(string cedula, bool esContratista);

        /// <summary>
        /// Obtiene la lista de racol
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<PURegionalAdministrativa> ObtenerRacol();

        /// <summary>
        /// Obtiene una lista de los conductores activos y con pase vigente asignados a un vehiculo
        /// </summary>
        /// <param name="idVehiculo"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<POConductores> ObtenerConductoresActivosVehiculos(int idVehiculo);

    #endregion Conductores

        #region Vehiculos

        /// <summary>
        /// Obtiene los vehiculos del racol del usuario
        /// </summary>
        /// <param name="idRacol">id del racol</param>
        /// <returns>Lista de los vehiculo del racol</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<POVehiculo> ObtenerVehiculosRacol(long idRacol);

        /// <summary>
        /// Obtiene los vehiculos del racol del punto de servicio
        /// </summary>
        /// <param name="idRacol">id del racol</param>
        /// <returns>Lista de los vehiculo del racol</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<POVehiculo> ObtenerVehiculosPuntoServicio(long idPuntoServicio);

        /// <summary>
        /// Valida si ya existe un vehiculo creado a partir de la placa
        /// </summary>
        /// <param name="placa"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        bool ValidarExisteVehiculoPlaca(string placa);

        /// <summary>
        /// Obtiene  los vehiculos
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consult</param>
        /// <returns>Lista con los vehiculos</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GenericoConsultasFramework<POVehiculo> ObtenerVehiculos(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente);

        /// <summary>
        /// Obtiene los estados para los vehiculos, solo activo e inactivo
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<POEstado> ObtenerEstadosSoloActIna();

        /// <summary>
        /// Obtiene las lineas de los vehiculos filtrado por la marca del vehiculo
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<POLinea> ObtenerLineaVehiculo(int idMarcaVehiculo);

        /// <summary>
        /// Obtiene todas las listas requeridas para la configuracion de un vehiculo
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        POListasDatosVehiculos ObtenerListasConfiguracionVehiculo();

        /// <summary>
        /// Obtiene todos los tipos de contrato
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<POTipoContrato> ObtenerTodosTipoContrato();

        /// <summary>
        /// Obtiene todas las categorias de licencia de conduccion
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<POCategoriaLicencia> ObtenerTodosCategoriaLicencia();

        /// <summary>
        /// Obtiene todos los tipos de poliza de seguro
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<POTipoPolizaSeguro> ObtenerTodosTipoPolizaSeguro();

        /// <summary>
        /// Obtiene todas las aseguradoras
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<POAseguradora> ObtenerTodosAseguradora();

        /// <summary>
        /// Inserta y edita la informacion de un vehiculo
        /// </summary>
        /// <param name="vehiculo"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizarVehiculo(POVehiculo vehiculo);

        /// <summary>
        /// Obtiene los tipos de mensajero vehicular
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<POTipoMensajero> ObtenerTiposMensajeroVehicular(int idTipoVehiculo);

        /// <summary>
        /// Obtiene los tipos de mensajero vehicular
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GenericoConsultasFramework<POMensajero> ObtenerMensajerosVehicular(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente);

        /// <summary>
        /// Obtiene el propietario del vehiculo apartir de la cedula y el tipo de contrato
        /// </summary>
        /// <param name="cedula">Numero de cedula del propietario</param>
        /// <returns>Objeto con la informacion del propietario</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        POPropietarioVehiculo ObtenerPropietarioVehiculo(string cedula, int tipoContrato);

        /// <summary>
        /// Obtiene  los conductores y auxiliares de camion (mensajeros tipo auxiliar) activos
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consult</param>
        /// <param name="idTipoMensajero">Id de la ruta por la cual se filtraran las estaciones</param>
        /// <returns>Lista  con los conductores y auxiliares en un objeto tipo mensajero</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GenericoConsultasFramework<POMensajero> ObtenerConductoresAuxiliares(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente);

        #endregion Vehiculos

        #region Creación de mensajeros

        /// <summary>
        /// Obtiene los mensajeros y conductores configurados
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GenericoConsultasFramework<POMensajero> ObtenerMensajerosConductores(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina,
                                                                              int registrosPorPagina, bool ordenamientoAscendente);

        /// <summary>
        /// Obtiene el listado de los mensajeros
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GenericoConsultasFramework<OUMensajeroDC> ObtenerMensajero(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente);


        /// <summary>
        /// Obtiene la informacion de un mensajero a partir del nombre de usuario, esto aplica
        /// para los usuario que tambien son mensajeros en el caso de los puntos moviles
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns></returns>
        long ObtenerIdMensajeroNomUsuario(string usuario);

        /// <summary>
        /// Obtiene informacion de un mensajero a partir de un nombre de usuario pam
        /// </summary>
        /// <param name="nomUsuario"></param>
        /// <returns></returns>
        OUMensajeroDC ObtenerInformacionMensajeroNomUsuarioPAM(string nomUsuario);
        /// <summary>
        /// Obtiene la informacion de un mensajero a partir del id del mensajero
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        OUMensajeroDC ObtenerMensajeroIdMensajero(long idMensajero);


        /// <summary>
        /// Obtiene la informacion de un mensajero a partir del id del mensajero PAM
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        OUMensajeroDC ObtenerMensajeroIdMensajeroPAM(long idMensajero);


        /// <summary>
        /// Consulta si el mensajero existe
        /// </summary>
        /// <param name="identificacion"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        OUMensajeroDC ConsultaExisteMensajero(string identificacion, bool contratista);

        /// <summary>
        /// Obtene los datos del mensajero de la agencia.
        /// </summary>
        /// <param name="idAgencia">Es el id agencia.</param>
        /// <returns>la lista de mensajeros de una agencia</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<POMensajero> ObtenerMensajerosAgencia(long idAgencia);

        /// <summary>
        /// Obtiene los tipos de mensajeros
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<OUTipoMensajeroDC> ObtenerTiposMensajero();

        /// <summary>
        /// Adiciona, edita o elimina un mensajero
        /// </summary>
        /// <param name="mensajero"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizarMensajero(OUMensajeroDC mensajero);

        /// <summary>
        /// Retorna los estados de los mensajeros
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<OUEstadosMensajeroDC> ObtenerEstadosMensajero();

        /// <summary>
        /// Obtiene el vehiculo asociado a un mensajero
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        POVehiculo ObtenerVehiculoMensajero(long idMensajero);

        /// <summary>
        /// Guarda un mensajero
        /// </summary>
        /// <param name="mensajero"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GuardarMensajero(POMensajero mensajero);

        #endregion Creación de mensajeros

        /// <summary>
        /// COnsulta los mensajeros activso del sistema activos y pertenecientes a agencias activas
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<OUMensajeroDC> ObtenerMensajerosActivos();

        /// <summary>
        /// Retorna un usuario interno dado su número de cédula
        /// </summary>
        /// <param name="idcedula"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        SEAdminUsuario ObtenerUsuarioInternoPorCedula(string idcedula);


        /// <summary>
        /// Agregar una nueva posicion (longitud laitud) de un mensajero
        /// </summary>
        /// <param name="posicionMensajero"></param>        
        void AgregarPosicionMensajero(POUbicacionMensajero posicionMensajero);

        /// <summary>
        /// Obtiene las ubicaciones de un mensajero en un rango de fechas determinado
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <param name="fechaInicial"></param>
        /// <param name="fechaFinal"></param>
        /// <returns></returns>
        List<POUbicacionMensajero> ObtenerUbicacionesMensajero(long idMensajero, DateTime fechaInicial, DateTime fechaFinal);

        /// <summary>
        /// Obtiene la ultima posicion registrada de un mensajero en el dia actual
        /// </summary>
        /// <param name="idMensajero"></param>        
        /// <returns></returns>
        List<POUbicacionMensajero> ObtenerUltimaUbicacionMensajeroDiaActual(long idMensajero);

        /// <summary>
        /// Obtiene la ultima posicion (del dia actual) de todos los mensajeros
        /// </summary>        
        /// <returns></returns>
        List<POUbicacionMensajero> ObtenerUltimaPosicionTodosMensajeros();

        /// <summary>
        /// Metodo para obtener ultima posicion mensajero por numero de guia
        /// </summary>
        /// <param name="NumeroGuia"></param>
        /// <returns></returns>
        POUbicacionMensajero ObtenerUltimaPosicionMensajeroPorNumeroGuia(long NumeroGuia);
    }
}