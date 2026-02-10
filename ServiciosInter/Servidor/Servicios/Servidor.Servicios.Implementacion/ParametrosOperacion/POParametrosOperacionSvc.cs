using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading;
using CO.Servidor.ParametrosOperacion;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion;
using CO.Servidor.Servicios.Contratos;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Seguridad;

namespace CO.Servidor.Servicios.Implementacion.ParametrosOperacion
{
    /// <summary>
    /// Clase para los servicios de administración de Clientes
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class POParametrosOperacionSvc : IPOParametrosOperacionSvc
    {
        public POParametrosOperacionSvc()
        {

        }

        #region Conductores

        /// <summary>
        /// Consulta todos los tipos de vehiculos
        /// </summary>
        /// <returns></returns>
        public IList<POTipoVehiculo> ObtenerTodosTipoVehiculo()
        {
            return POAdministradorParametrosOperacion.Instancia.ObtenerTodosTipoVehiculo();
        }

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
        public GenericoConsultasFramework<POConductores> ObtenerConductores(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente)
        {
            int totalRegistros = 0;
            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<POConductores>()
            {
                Lista = POAdministradorParametrosOperacion.Instancia.ObtenerConductores(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Adiciona y edita un conductor
        /// </summary>
        /// <param name="conductor"></param>
        public void ActualizarConductores(POConductores conductor)
        {
            POAdministradorParametrosOperacion.Instancia.ActualizarConductores(conductor);
        }

        /// <summary>
        /// Obtiene los estados para los centros de servicio
        /// </summary>
        /// <returns></returns>
        public IList<POEstado> ObtenerEstados()
        {
            return POAdministradorParametrosOperacion.Instancia.ObtenerEstados();
        }

        /// <summary>
        /// Consulta la informacion de un conductor desde novasoft
        /// </summary>
        /// <param name="cedula">Numero de cedula del conductor</param>
        /// <param name="conductor">Bandera que indica si el conductor es un empleado o un contratista</param>
        /// <returns>Objeto con la informacion del conductor</returns>
        public POConductores ObtenerConductorNovasoft(string cedula, bool esContratista)
        {
            return POAdministradorParametrosOperacion.Instancia.ObtenerConductorNovasoft(cedula, esContratista);
        }

        /// <summary>
        /// Obtiene la lista de racol
        /// </summary>
        /// <returns></returns>
        public IList<PURegionalAdministrativa> ObtenerRacol()
        {
            return POAdministradorParametrosOperacion.Instancia.ObtenerRacol();
        }

        public List<POTerritorial> ObtenerTodasTerritoriales()
        {
            return POAdministradorParametrosOperacion.Instancia.ObtenerTodasTerritoriales();
        }

        /// <summary>
        /// Obtiene una lista de los conductores activos y con pase vigente asignados a un vehiculo
        /// </summary>
        /// <param name="idVehiculo"></param>
        /// <returns></returns>
        public IList<POConductores> ObtenerConductoresActivosVehiculos(int idVehiculo)
        {
            return POAdministradorParametrosOperacion.Instancia.ObtenerConductoresActivosVehiculos(idVehiculo);
        }

        #endregion Conductores

        #region Vehiculos

        /// <summary>
        /// Obtiene los vehiculos del racol del usuario
        /// </summary>
        /// <param name="idRacol">id del racol</param>
        /// <returns>Lista de los vehiculo del racol</returns>
        public List<POVehiculo> ObtenerVehiculosRacol(long idRacol)
        {
            return POAdministradorParametrosOperacion.Instancia.ObtenerVehiculosRacol(idRacol);
        }



        /// <summary>
        /// Obtiene los vehiculos del racol del punto de servicio
        /// </summary>
        /// <param name="idRacol">id del racol</param>
        /// <returns>Lista de los vehiculo del racol</returns>
        public List<POVehiculo> ObtenerVehiculosPuntoServicio(long idPuntoServicio)
        {

            return POAdministradorParametrosOperacion.Instancia.ObtenerVehiculosPuntoServicio(idPuntoServicio);
        }

        /// <summary>
        /// Valida si ya existe un vehiculo creado a partir de la placa
        /// </summary>
        /// <param name="placa"></param>
        /// <returns></returns>
        public bool ValidarExisteVehiculoPlaca(string placa)
        {
            return POAdministradorParametrosOperacion.Instancia.ValidarExisteVehiculoPlaca(placa);
        }

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
        public GenericoConsultasFramework<POVehiculo> ObtenerVehiculos(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente)
        {
            int totalRegistros = 0;
            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<POVehiculo>()
            {
                Lista = POAdministradorParametrosOperacion.Instancia.ObtenerVehiculos(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Obtiene los estados para los vehiculos, solo activo e inactivo
        /// </summary>
        /// <returns></returns>
        public IList<POEstado> ObtenerEstadosSoloActIna()
        {
            return POAdministradorParametrosOperacion.Instancia.ObtenerEstadosSoloActIna();
        }

        /// <summary>
        /// Obtiene las lineas de los vehiculos filtrado por la marca del vehiculo
        /// </summary>
        /// <returns></returns>
        public IList<POLinea> ObtenerLineaVehiculo(int idMarcaVehiculo)
        {
            return POAdministradorParametrosOperacion.Instancia.ObtenerLineaVehiculo(idMarcaVehiculo);
        }

        /// <summary>
        /// Obtiene todas las listas requeridas para la configuracion de un vehiculo
        /// </summary>
        /// <returns></returns>
        public POListasDatosVehiculos ObtenerListasConfiguracionVehiculo()
        {
            return POAdministradorParametrosOperacion.Instancia.ObtenerListasConfiguracionVehiculo();
        }

        /// <summary>
        /// Obtiene todos los tipos de contrato
        /// </summary>
        /// <returns></returns>
        public IList<POTipoContrato> ObtenerTodosTipoContrato()
        {
            return POAdministradorParametrosOperacion.Instancia.ObtenerTodosTipoContrato();
        }

        /// <summary>
        /// Obtiene todas las categorias de licencia de conduccion
        /// </summary>
        /// <returns></returns>
        public IList<POCategoriaLicencia> ObtenerTodosCategoriaLicencia()
        {
            return POAdministradorParametrosOperacion.Instancia.ObtenerTodosCategoriaLicencia();
        }

        /// <summary>
        /// Obtiene todos los tipos de poliza de seguro
        /// </summary>
        /// <returns></returns>
        public IList<POTipoPolizaSeguro> ObtenerTodosTipoPolizaSeguro()
        {
            return POAdministradorParametrosOperacion.Instancia.ObtenerTodosTipoPolizaSeguro();
        }

        /// <summary>
        /// Obtiene todas las aseguradoras
        /// </summary>
        /// <returns></returns>
        public IList<POAseguradora> ObtenerTodosAseguradora()
        {
            return POAdministradorParametrosOperacion.Instancia.ObtenerTodosAseguradora();
        }

        /// <summary>
        /// Inserta y edita la informacion de un vehiculo
        /// </summary>
        /// <param name="vehiculo"></param>
        public void ActualizarVehiculo(POVehiculo vehiculo)
        {
            POAdministradorParametrosOperacion.Instancia.ActualizarVehiculo(vehiculo);
        }

        /// <summary>
        /// Obtiene los tipos de mensajero vehicular
        /// </summary>
        /// <returns></returns>
        public IList<POTipoMensajero> ObtenerTiposMensajeroVehicular(int idTipoVehiculo)
        {
            return POAdministradorParametrosOperacion.Instancia.ObtenerTiposMensajeroVehicular(idTipoVehiculo);
        }

        /// <summary>
        /// Obtiene  los mensajeros tipo vehicular
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consult</param>
        /// <param name="idTipoMensajero">Id de la ruta por la cual se filtraran las estaciones</param>
        /// <returns>Lista  con los mensajeros</returns>
        public GenericoConsultasFramework<POMensajero> ObtenerMensajerosVehicular(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente)
        {
            int totalRegistros = 0;
            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<POMensajero>()
            {
                Lista = POAdministradorParametrosOperacion.Instancia.ObtenerMensajerosVehicular(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Obtiene el propietario del vehiculo apartir de la cedula y el tipo de contrato
        /// </summary>
        /// <param name="cedula">Numero de cedula del propietario</param>
        /// <returns>Objeto con la informacion del propietario</returns>
        public POPropietarioVehiculo ObtenerPropietarioVehiculo(string cedula, int tipoContrato)
        {
            return POAdministradorParametrosOperacion.Instancia.ObtenerPropietarioVehiculo(cedula, tipoContrato);
        }

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
        public GenericoConsultasFramework<POMensajero> ObtenerConductoresAuxiliares(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente)
        {
            int totalRegistros = 0;
            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<POMensajero>()
            {
                Lista = POAdministradorParametrosOperacion.Instancia.ObtenerConductoresAuxiliares(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros),
                TotalRegistros = totalRegistros
            };
        }

        #endregion Vehiculos

        #region Creacion de mensajeros

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
        public GenericoConsultasFramework<POMensajero> ObtenerMensajerosConductores(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina,
                                                                              int registrosPorPagina, bool ordenamientoAscendente)
        {
            int totalRegistros = 0;
            return new GenericoConsultasFramework<POMensajero>()
            {
                Lista = POAdministradorParametrosOperacion.Instancia.ObtenerMensajerosConductores(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Obtiene el listado de los mensajeros
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <returns></returns>
        public GenericoConsultasFramework<OUMensajeroDC> ObtenerMensajero(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente)
        {
            int totalRegistros = 0;
            return new GenericoConsultasFramework<OUMensajeroDC>()
            {
                Lista = POAdministradorParametrosOperacion.Instancia.ObtenerMensajero(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Obtiene la informacion de un mensajero a partir del nombre de usuario, esto aplica
        /// para los usuario que tambien son mensajeros en el caso de los puntos moviles
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns></returns>
        public long ObtenerIdMensajeroNomUsuario(string usuario)
        {
            return POAdministradorParametrosOperacion.Instancia.ObtenerIdMensajeroNomUsuario(usuario);
        }

        /// <summary>
        /// Obtiene la informacion de un mensajero a partir del id del mensajero
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public OUMensajeroDC ObtenerMensajeroIdMensajero(long idMensajero)
        {
            return POAdministradorParametrosOperacion.Instancia.ObtenerMensajeroIdMensajero(idMensajero);
        }

        /// <summary>
        /// Obtiene la informacion de un mensajero a partir del id del mensajero PAM
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public OUMensajeroDC ObtenerMensajeroIdMensajeroPAM(long idMensajero)
        {
            return POAdministradorParametrosOperacion.Instancia.ObtenerMensajeroIdMensajeroPAM(idMensajero);
        }


        /// <summary>
        /// Consulta si el mensajero existe
        /// </summary>
        /// <param name="identificacion"></param>
        /// <returns></returns>
        public OUMensajeroDC ConsultaExisteMensajero(string identificacion, bool contratista)
        {
            return POAdministradorParametrosOperacion.Instancia.ConsultaExisteMensajero(identificacion, contratista);
        }

        /// <summary>
        /// Obtiene los tipos de mensajeros
        /// </summary>
        /// <returns></returns>
        public IEnumerable<OUTipoMensajeroDC> ObtenerTiposMensajero()
        {
            return POAdministradorParametrosOperacion.Instancia.ObtenerTiposMensajero();
        }

        /// <summary>
        /// Adiciona, edita o elimina un mensajero
        /// </summary>
        /// <param name="mensajero"></param>
        public void ActualizarMensajero(OUMensajeroDC mensajero)
        {
            POAdministradorParametrosOperacion.Instancia.ActualizarMensajero(mensajero);
        }

        /// <summary>
        /// Retorna los estados de los mensajeros
        /// </summary>
        /// <returns></returns>
        public IList<OUEstadosMensajeroDC> ObtenerEstadosMensajero()
        {
            return POAdministradorParametrosOperacion.Instancia.ObtenerEstadosMensajero();
        }

        /// <summary>
        /// Obtene los datos del mensajero de la agencia.
        /// </summary>
        /// <param name="idAgencia">Es el id agencia.</param>
        /// <returns>la lista de mensajeros de una agencia</returns>
        public IEnumerable<POMensajero> ObtenerMensajerosAgencia(long idAgencia)
        {
            return POAdministradorParametrosOperacion.Instancia.ObtenerMensajerosAgencia(idAgencia);
        }


        /// <summary>
        /// Obtiene el vehiculo asociado a un mensajero
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public POVehiculo ObtenerVehiculoMensajero(long idMensajero)
        {
            return POAdministradorParametrosOperacion.Instancia.ObtenerVehiculoMensajero(idMensajero);
        }

        /// <summary>
        /// Guarda un mensajero
        /// </summary>
        /// <param name="mensajero"></param>
        public void GuardarMensajero(POMensajero mensajero)
        {
            POAdministradorParametrosOperacion.Instancia.GuardarMensajero(mensajero);
        }

        #endregion Creacion de mensajeros

        /// <summary>
        /// Consulta los mensajeros activso del sistema activos y pertenecientes a agencias activas
        /// </summary>
        /// <returns></returns>
        public List<OUMensajeroDC> ObtenerMensajerosActivos()
        {
            return POAdministradorParametrosOperacion.Instancia.ObtenerMensajerosActivos();
        }

        /// <summary>
        /// Retorna un usuario interno dado su número de cédula
        /// </summary>
        /// <param name="idcedula"></param>
        /// <returns></returns>
        public SEAdminUsuario ObtenerUsuarioInternoPorCedula(string idcedula)
        {
            return POAdministradorParametrosOperacion.Instancia.ObtenerUsuarioInternoPorCedula(idcedula);
        }

        /// <summary>
        /// Agregar una nueva posicion (longitud laitud) de un mensajero
        /// </summary>
        /// <param name="posicionMensajero"></param>        
        public void AgregarPosicionMensajero(POUbicacionMensajero posicionMensajero)
        {
            POAdministradorParametrosOperacion.Instancia.AgregarPosicionMensajero(posicionMensajero);
        }

        /// <summary>
        /// Obtiene las ubicaciones de un mensajero en un rango de fechas determinado
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <param name="fechaInicial"></param>
        /// <param name="fechaFinal"></param>
        /// <returns></returns>
        public List<POUbicacionMensajero> ObtenerUbicacionesMensajero(long idMensajero, DateTime fechaInicial, DateTime fechaFinal)
        {
            return POAdministradorParametrosOperacion.Instancia.ObtenerUbicacionesMensajero(idMensajero, fechaInicial, fechaFinal);
        }

        /// <summary>
        /// Obtiene la ultima posicion registrada de un mensajero en el dia actual
        /// </summary>
        /// <param name="idMensajero"></param>        
        /// <returns></returns>
        public List<POUbicacionMensajero> ObtenerUltimaUbicacionMensajeroDiaActual(long idMensajero)
        {

            return POAdministradorParametrosOperacion.Instancia.ObtenerUltimaUbicacionMensajeroDiaActual(idMensajero);
        }

        /// <summary>
        /// Obtiene la ultima posicion (del dia actual) de todos los mensajeros
        /// </summary>        
        /// <returns></returns>
        public List<POUbicacionMensajero> ObtenerUltimaPosicionTodosMensajeros()
        {
            return POAdministradorParametrosOperacion.Instancia.ObtenerUltimaPosicionTodosMensajeros();
        }

        public List<POPersonaExterna> paObtenerPersonasExternas()
        {
            return POAdministradorParametrosOperacion.Instancia.ObtenerPersonasExternas();
        }

        public void ActualizarPersonaExterna(POPersonaExterna persona)
        {
            POAdministradorParametrosOperacion.Instancia.ActualizarPersonaExterna(persona);
        }

        public void AdicionarPersonaExterna(POPersonaExterna persona)
        {
            POAdministradorParametrosOperacion.Instancia.AdicionarPersonaExterna(persona);
        }

        public void EliminarPersonaExterna(long idPersona)
        {
            POAdministradorParametrosOperacion.Instancia.EliminarPersonaExterna(idPersona);
        }

        /// <summary>
        /// Metodo para obtener informacion del mensajero segun usuario
        /// </summary>
        /// <param name="nomUsuario"></param>
        /// <returns></returns>
        public OUMensajeroDC ObtenerInformacionMensajeroNomUsuarioPAM(string nomUsuario)
        {
            return POAdministradorParametrosOperacion.Instancia.ObtenerInformacionMensajeroNomUsuarioPAM(nomUsuario);
        }

        /// <summary>
        /// Metodo para obtener la ultima posicion del mensajero
        /// </summary>
        /// <param name="NumeroGuia"></param>
        /// <returns></returns>
        public POUbicacionMensajero ObtenerUltimaPosicionMensajeroPorNumeroGuia(long NumeroGuia) {
            return POAdministradorParametrosOperacion.Instancia.ObtenerUltimaPosicionMensajeroPorNumeroGuia(NumeroGuia);
        }
    }
}