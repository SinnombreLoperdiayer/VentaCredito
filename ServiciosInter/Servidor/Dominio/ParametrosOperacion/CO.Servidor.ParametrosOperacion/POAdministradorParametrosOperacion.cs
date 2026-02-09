using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion;
using Framework.Servidor.Comun;
using Framework.Servidor.Servicios.ContratoDatos.Seguridad;

namespace CO.Servidor.ParametrosOperacion
{
    /// <summary>
    /// Administrador para los parametros de la operacion
    /// </summary>
    public class POAdministradorParametrosOperacion
    {
        private static readonly POAdministradorParametrosOperacion instancia = new POAdministradorParametrosOperacion();

        public static POAdministradorParametrosOperacion Instancia
        {
            get { return POAdministradorParametrosOperacion.instancia; }
        }

        /// <summary>
        /// constructor
        /// </summary>
        private POAdministradorParametrosOperacion() { }

        #region Conductores

        /// <summary>
        /// Consulta todos los tipos de vehiculos
        /// </summary>
        /// <returns></returns>
        public IList<POTipoVehiculo> ObtenerTodosTipoVehiculo()
        {
            return POParametrosOperacion.Instancia.ObtenerTodosTipoVehiculo();
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
        public IList<POConductores> ObtenerConductores(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return POParametrosOperacion.Instancia.ObtenerConductores(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Obtiene todos los conductores
        /// </summary>
        public IList<POConductores> ObtenerTodosConductores()
        {
            return POParametrosOperacion.Instancia.ObtenerTodosConductores();
        }

        /// <summary>
        /// Adiciona y edita un conductor
        /// </summary>
        /// <param name="conductor"></param>
        public void ActualizarConductores(POConductores conductor)
        {
            switch (conductor.EstadoRegistro)
            {
                case EnumEstadoRegistro.ADICIONADO:
                    POParametrosOperacion.Instancia.AdicionarConductor(conductor);
                    break;
                case EnumEstadoRegistro.MODIFICADO:
                    POParametrosOperacion.Instancia.EditarConductor(conductor);
                    break;
            }
        }

        /// <summary>
        /// Obtiene los estados para los centros de servicio
        /// </summary>
        /// <returns></returns>
        public IList<POEstado> ObtenerEstados()
        {
            return POParametrosOperacion.Instancia.ObtenerEstados();
        }

        /// <summary>
        /// Consulta la informacion de un conductor desde novasoft
        /// </summary>
        /// <param name="cedula">Numero de cedula del conductor</param>
        /// <param name="conductor">Bandera que indica si el conductor es un empleado o un contratista</param>
        /// <returns>Objeto con la informacion del conductor</returns>
        public POConductores ObtenerConductorNovasoft(string cedula, bool esContratista)
        {
            return POParametrosOperacion.Instancia.ObtenerConductorNovasoft(cedula, esContratista);
        }

        /// <summary>
        /// Obtiene la lista de racol
        /// </summary>
        /// <returns></returns>
        public IList<PURegionalAdministrativa> ObtenerRacol()
        {
            return POParametrosOperacion.Instancia.ObtenerRacol();
        }

        public List<POTerritorial> ObtenerTodasTerritoriales()
        {
            return POParametrosOperacion.Instancia.ObtenerTodasTerritoriales();
        }

        /// <summary>
        /// Obtiene una lista de los conductores activos y con pase vigente asignados a un vehiculo
        /// </summary>
        /// <param name="idVehiculo"></param>
        /// <returns></returns>
        public IList<POConductores> ObtenerConductoresActivosVehiculos(int idVehiculo)
        {
            return POParametrosOperacion.Instancia.ObtenerConductoresActivosVehiculos(idVehiculo);
        }

        #endregion Conductores

        #region Vehiculos

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
        public IList<POVehiculo> ObtenerVehiculos(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return POParametrosOperacion.Instancia.ObtenerVehiculos(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Valida si ya existe un vehiculo creado a partir de la placa
        /// </summary>
        /// <param name="placa"></param>
        /// <returns></returns>
        public bool ValidarExisteVehiculoPlaca(string placa)
        {
            return POParametrosOperacion.Instancia.ValidarExisteVehiculoPlaca(placa);
        }

        /// <summary>
        /// Obtiene los estados para los vehiculos, solo activo e inactivo
        /// </summary>
        /// <returns></returns>
        public IList<POEstado> ObtenerEstadosSoloActIna()
        {
            return POParametrosOperacion.Instancia.ObtenerEstadosSoloActIna();
        }

        /// <summary>
        /// Obtiene las lineas de los vehiculos filtradas por la marca
        /// </summary>
        /// <returns></returns>
        public IList<POLinea> ObtenerLineaVehiculo(int idMarcaVehiculo)
        {
            return POParametrosOperacion.Instancia.ObtenerLineaVehiculo(idMarcaVehiculo);
        }

        /// <summary>
        /// Obtiene todas las listas requeridas para la configuracion de un vehiculo
        /// </summary>
        /// <returns></returns>
        public POListasDatosVehiculos ObtenerListasConfiguracionVehiculo()
        {
            return POParametrosOperacion.Instancia.ObtenerListasConfiguracionVehiculo();
        }

        /// <summary>
        /// Obtiene todos los tipos de contrato
        /// </summary>
        /// <returns></returns>
        public IList<POTipoContrato> ObtenerTodosTipoContrato()
        {
            return POParametrosOperacion.Instancia.ObtenerTodosTipoContrato();
        }

        /// <summary>
        /// Obtiene todas las categorias de licencia de conduccion
        /// </summary>
        /// <returns></returns>
        public IList<POCategoriaLicencia> ObtenerTodosCategoriaLicencia()
        {
            return POParametrosOperacion.Instancia.ObtenerTodosCategoriaLicencia();
        }

        /// <summary>
        /// Obtiene todos los tipos de poliza de seguro
        /// </summary>
        /// <returns></returns>
        public IList<POTipoPolizaSeguro> ObtenerTodosTipoPolizaSeguro()
        {
            return POParametrosOperacion.Instancia.ObtenerTodosTipoPolizaSeguro();
        }

        /// <summary>
        /// Obtiene todas las aseguradoras
        /// </summary>
        /// <returns></returns>
        public IList<POAseguradora> ObtenerTodosAseguradora()
        {
            return POParametrosOperacion.Instancia.ObtenerTodosAseguradora();
        }

        /// <summary>
        /// Inserta y edita la informacion de un vehiculo
        /// </summary>
        /// <param name="vehiculo"></param>
        public void ActualizarVehiculo(POVehiculo vehiculo)
        {
            POParametrosOperacion.Instancia.ActualizarVehiculo(vehiculo);
        }

        /// <summary>
        /// Obtiene los tipos de mensajero vehicular
        /// </summary>
        /// <returns></returns>
        public IList<POTipoMensajero> ObtenerTiposMensajeroVehicular(int idTipoVehiculo)
        {
            return POParametrosOperacion.Instancia.ObtenerTiposMensajeroVehicular(idTipoVehiculo);
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
        public IList<POMensajero> ObtenerMensajerosVehicular(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return POParametrosOperacion.Instancia.ObtenerMensajerosVehicular(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Obtiene el propietario del vehiculo apartir de la cedula y el tipo de contrato
        /// </summary>
        /// <param name="cedula">Numero de cedula del propietario</param>
        /// <returns>Objeto con la informacion del propietario</returns>
        public POPropietarioVehiculo ObtenerPropietarioVehiculo(string cedula, int tipoContrato)
        {
            return POParametrosOperacion.Instancia.ObtenerPropietarioVehiculo(cedula, tipoContrato);
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
        public IList<POMensajero> ObtenerConductoresAuxiliares(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return POParametrosOperacion.Instancia.ObtenerConductoresAuxiliares(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Obtiene los vehiculos del racol del usuario
        /// </summary>
        /// <param name="idRacol">id del racol</param>
        /// <returns>Lista de los vehiculo del racol</returns>
        public List<POVehiculo> ObtenerVehiculosRacol(long idRacol)
        {
            return POParametrosOperacion.Instancia.ObtenerVehiculosRacol(idRacol);
        }

        /// <summary>
        /// Obtiene los vehiculos del racol del usuario
        /// </summary>
        /// <param name="idRacol">id del racol</param>
        /// <returns>Lista de los vehiculo del racol</returns>
        public List<POVehiculo> ObtenerVehiculosPuntoServicio(long idPuntoServicio)
        {

            return POParametrosOperacion.Instancia.ObtenerVehiculosPuntoServicio(idPuntoServicio);
        }

        #endregion Vehiculos

        #region Creacion Mensajero

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
        public IEnumerable<POMensajero> ObtenerMensajerosConductores(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina,
                                                                              int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return POParametrosOperacion.Instancia.ObtenerMensajerosConductores(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Obtiene el listado de los mensajeros
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <returns></returns>
        public IList<OUMensajeroDC> ObtenerMensajero(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return POParametrosOperacion.Instancia.ObtenerMensajero(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Obtiene la informacion de un mensajero a partir del nombre de usuario, esto aplica
        /// para los usuario que tambien son mensajeros en el caso de los puntos moviles
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns></returns>
        public long ObtenerIdMensajeroNomUsuario(string usuario)
        {
            return POParametrosOperacion.Instancia.ObtenerIdMensajeroNomUsuario(usuario);
        }

        /// <summary>
        /// Obtiene la informacion de un mensajero a partir del id del mensajero
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public OUMensajeroDC ObtenerMensajeroIdMensajero(long idMensajero)
        {
            return POParametrosOperacion.Instancia.ObtenerMensajeroIdMensajero(idMensajero);
        }



        /// <summary>
        /// Obtiene la informacion de un mensajero a partir del id del mensajero
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public OUMensajeroDC ObtenerMensajeroIdMensajeroPAM(long idMensajero)
        {
            return POParametrosOperacion.Instancia.ObtenerMensajeroIdMensajeroPAM(idMensajero);
        }


        /// <summary>
        /// COnsulta los mensajeros activso del sistema activos y pertenecientes a agencias activas
        /// </summary>
        /// <returns></returns>
        public List<OUMensajeroDC> ObtenerMensajerosActivos()
        {
            return POParametrosOperacion.Instancia.ObtenerMensajerosActivos();
        }

        /// <summary>
        /// Consulta si el mensajero existe
        /// </summary>
        /// <param name="identificacion"></param>
        /// <returns></returns>
        public OUMensajeroDC ConsultaExisteMensajero(string identificacion, bool contratista)
        {
            return POParametrosOperacion.Instancia.ConsultaExisteMensajero(identificacion, contratista);
        }

        /// <summary>
        /// Obtiene los tipos de mensajeros
        /// </summary>
        /// <returns></returns>
        public IEnumerable<OUTipoMensajeroDC> ObtenerTiposMensajero()
        {
            return POParametrosOperacion.Instancia.ObtenerTiposMensajero();
        }

        /// <summary>
        /// Adiciona, edita o elimina un mensajero
        /// </summary>
        /// <param name="mensajero"></param>
        public void ActualizarMensajero(OUMensajeroDC mensajero)
        {
            POParametrosOperacion.Instancia.ActualizarMensajero(mensajero);
        }

        /// <summary>
        /// Retorna los estados de los mensajeros
        /// </summary>
        /// <returns></returns>
        public IList<OUEstadosMensajeroDC> ObtenerEstadosMensajero()
        {
            return POParametrosOperacion.Instancia.ObtenerEstadosMensajero();
        }


        /// <summary>
        /// Obtiene el vehiculo asociado a un mensajero
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public POVehiculo ObtenerVehiculoMensajero(long idMensajero)
        {
            return POParametrosOperacion.Instancia.ObtenerVehiculoMensajero(idMensajero);
        }

        /// <summary>
        /// Guarda un mensajero
        /// </summary>
        /// <param name="mensajero"></param>
        public void GuardarMensajero(POMensajero mensajero)
        {
            POParametrosOperacion.Instancia.GuardarMensajero(mensajero);
        }

        #endregion Creacion Mensajero

        /// <summary>
        /// Obtene los datos del mensajero de la agencia.
        /// </summary>
        /// <param name="idAgencia">Es el id agencia.</param>
        /// <returns>la lista de mensajeros de una agencia</returns>
        public IEnumerable<POMensajero> ObtenerMensajerosAgencia(long idAgencia)
        {
            return POParametrosOperacion.Instancia.ObtenerMensajerosAgencia(idAgencia);
        }

        /// <summary>
        /// Retorna un usuario interno dado su número de cédula
        /// </summary>
        /// <param name="idcedula"></param>
        /// <returns></returns>
        public SEAdminUsuario ObtenerUsuarioInternoPorCedula(string idcedula)
        {
            return POParametrosOperacion.Instancia.ObtenerUsuarioInternoPorCedula(idcedula);
        }

        /// <summary>
        /// Agregar una nueva posicion (longitud laitud) de un mensajero
        /// </summary>
        /// <param name="posicionMensajero"></param>        
        public void AgregarPosicionMensajero(POUbicacionMensajero posicionMensajero)
        {
            POParametrosOperacion.Instancia.AgregarPosicionMensajero(posicionMensajero);
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
            return POParametrosOperacion.Instancia.ObtenerUbicacionesMensajero(idMensajero, fechaInicial, fechaFinal);
        }



        /// <summary>
        /// Obtiene la ultima posicion registrada de un mensajero en el dia actual
        /// </summary>
        /// <param name="idMensajero"></param>        
        /// <returns></returns>
        public List<POUbicacionMensajero> ObtenerUltimaUbicacionMensajeroDiaActual(long idMensajero)
        {

            return POParametrosOperacion.Instancia.ObtenerUltimaUbicacionMensajeroDiaActual(idMensajero);
        }

        /// <summary>
        /// Obtiene la ultima posicion (del dia actual) de todos los mensajeros
        /// </summary>        
        /// <returns></returns>
        public List<POUbicacionMensajero> ObtenerUltimaPosicionTodosMensajeros()
        {
            return POParametrosOperacion.Instancia.ObtenerUltimaPosicionTodosMensajeros();
        }

        public List<POPersonaExterna> ObtenerPersonasExternas()
        {
            return POParametrosOperacion.Instancia.ObtenerPersonasExternas();
        }

        public void ActualizarPersonaExterna(POPersonaExterna persona)
        {
            POParametrosOperacion.Instancia.ActualizarPersonaExterna(persona);
        }

        public void AdicionarPersonaExterna(POPersonaExterna persona)
        {
            POParametrosOperacion.Instancia.AdicionarPersonaExterna(persona);
        }

        public void EliminarPersonaExterna(long idPersona)
        {
            POParametrosOperacion.Instancia.EliminarPersonaExterna(idPersona);
        }
        /// <summary>
        /// Metodo para obtener informacion del mensajero
        /// </summary>
        /// <returns></returns>
        public OUMensajeroDC ObtenerInformacionMensajeroNomUsuarioPAM(string nomUsuario)
        {
            return POParametrosOperacion.Instancia.ObtenerInformacionMensajeroNomUsuarioPAM(nomUsuario);
        }

        /// <summary>
        /// Metodo para obtener la ultima posicion del mensajero segun numero de guia 
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public POUbicacionMensajero ObtenerUltimaPosicionMensajeroPorNumeroGuia(long numeroGuia)
        {
            return POParametrosOperacion.Instancia.ObtenerUltimaPosicionMensajeroPorNumeroGuia(numeroGuia);
        }
    }
}