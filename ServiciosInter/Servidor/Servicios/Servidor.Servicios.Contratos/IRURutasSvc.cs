using CO.Servidor.Servicios.ContratoDatos.Rutas;
using CO.Servidor.Servicios.ContratoDatos.Rutas.Optimizacion;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.ServiceModel;


namespace CO.Servidor.Servicios.Contratos
{
    /// <summary>
    /// Contratos WCF de centros de servicios
    /// </summary>
    [ServiceContract(Namespace = "http://contrologis.com")]
    public interface IRURutasSvc
    {
        /// <summary>
        /// Obtiene  las rutas
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consult</param>
        /// <returns>Lista  con las rutas</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GenericoConsultasFramework<RURutaDC> ObtenerRutas(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente);

        

        /// <summary>
        /// Obtiene una lista con todos los tipos de ruta, para llenar un comboBox
        /// </summary>
        /// <returns>Lista con los tipos de ruta</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<RUTipoRuta> ObtenerTodosTipoRuta();

        /// <summary>
        /// Obtiene las empresas trasportadoras filtradas por el tipo de transporte
        /// </summary>
        /// <param name="idTipoTransporte"></param>
        /// <returns>Lista de empresas transportadoras filtradas por el tipo de transporte</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<RUEmpresaTransportadora> ObtieneEmpresaTransportadoraTipoTransporte(int idTipoTransporte);

        /// <summary>
        /// Obtiene lista de tipos de transporte
        /// </summary>
        /// <returns>lista con los tipos de transporte</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<RUTipoTransporte> ObtieneTodosTipoTransporte();

        /// <summary>
        /// Obtiene  las ciudades estacion de una ruta
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consult</param>
        /// <param name="idRuta">Id de la ruta por la cual se filtraran las estaciones</param>
        /// <returns>Lista  con las rutas</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GenericoConsultasFramework<RUEstacionRuta> ObtenerEstacionesRuta(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, int idRuta);

        /// <summary>
        /// Obtiene  las ciudades hijas (cobertura) de una estacion de una ruta
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="idCiudadEstacion">Id de ciudad estacion para la cual se retornara la cobertura</param>
        /// <returns>Lista  con las ciudades hijas de la ciudad estacion (cobertura de la ciudad estacion)</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GenericoConsultasFramework<RUCoberturaEstacion> ObtenerCiudadesHijasEstacionesRuta(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, int idCiudadEstacion);

        /// <summary>
        /// Obtiene  las ciudades que se manifiestan por una ruta
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consult</param>
        /// <param name="idRuta">Id de la ruta por la cual se manifiestan las ciudades</param>
        /// <returns>Lista  con las ciudades que se manifiestan por una ruta</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GenericoConsultasFramework<RUCiudadManifestadaEnRuta> ObtenerCiudadesManifiestanEnRuta(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, int idRuta);

        /// <summary>
        /// Actualiza la informacion de una ciudad que se manifiesta en una ruta
        /// </summary>
        /// <param name="ciudad">Objeto con la informacion de la ciudad que se manifiesta en la ruta</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizarCiudadQueManifiestaenRuta(RUCiudadManifestadaEnRuta ciudad);

        /// <summary>
        /// Obtiene  la cobertura de una ciudad que se manifiestan por una ruta
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consult</param>
        /// <param name="idCiudadManifiestaRuta">Id de la localidadManifiestaenRuta</param>
        /// <returns>cobertura de una ciudad que se manifiesta en una ruta</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GenericoConsultasFramework<RUCoberturaCiudadManifiestaPorRuta> ObtenerCoberturaCiudadManifiestaEnRuta(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, int idCiudadManifiestaRuta);

        /// <summary>
        /// Inserta o borra la cobertura de la ciudades que manifiesta por ruta
        /// </summary>
        /// <param name="ciudad"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizarCoberturaCiudadManifiestaEnRuta(List<RUCoberturaCiudadManifiestaPorRuta> ciudades);

        /// <summary>
        /// Adiciona edita o elimina una estacion ruta
        /// </summary>
        /// <param name="estacionRuta"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizarEstacionRuta(RUEstacionRuta estacionRuta);

        /// <summary>
        /// Adiciona o edita la informacion de una ruta
        /// </summary>
        /// <param name="ruta"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        int ActualizarRuta(RURutaDC ruta);

        /// <summary>
        /// Obtiene todas las empresas transportadoras dependiendo de un medio de transporte
        /// </summary>
        /// <param name="idMedioTransporte">Identificador del medio de transporte</param>
        /// <returns>Lista de empresas transportadoras</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<RUEmpresaTransportadora> ObtenerEmpresaTransportadoraXMedioTransporte(int idMedioTransporte, int tipoTransporte);

        /// <summary>
        ///  Obtiene las rutas filtradas por la ciudad de origen o destino
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <param name="idLocalidadAgencia"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GenericoConsultasFramework<RURutaDC> ObtenerRutasXCiudadOrigenDestino(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, string idLocalidadAgencia);

        /// <summary>
        /// Calcula la ruta más optima y la ruta menos óptima desde un origen a un destino
        /// </summary>
        /// <param name="idLocalidadOrigen">Identificador de la localidad origen del envío</param>
        /// <param name="idLocalidadDestino">Identificador de la localidad destino del envío</param>
        /// <returns>ruta óptima calculada</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        RURutaOptimaCalculada CalcularRutaOptimaConFechaAdmision(string idLocalidadOrigen, string idLocalidadDestino, DateTime fechadmisionEnvio);

        #region Empresa Transportadora

        /// <summary>
        /// Metodo que consulta las empresas
        /// transportadoras
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <returns>lista de empresas transportadoras</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RUEmpresaTransportadora> ObtenerEmpresaTransportadora(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros);

        /// <summary>
        /// Metodo para Adicionar - Editar - Eliminar Empresas Transportadoras
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GestionarEmpresaTransportadora(RUEmpresaTransportadora empresaTransportadora);

        /// <summary>
        /// Obtiene la Info Inicial para la pantalla de Empresa Trasnportadora
        /// </summary>
        /// <returns>Listas de Inicializacion</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        RUEmpresaTransporteInfoInicialDC ObtenerInfoInicialEmpresaTransportadora();

        #endregion Empresa Transportadora

        #region rutas
        /// <summary>
        /// Obtiene rutas
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RURutaICWeb> ObtenerRuta();

        /// <summary>
        /// obtiene centros de servicios de la ruta indicada
        /// </summary>
        /// <param name="idRuta"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RURutaCWebDetalleCentrosServicios> ObtenerRutaDetalleCentroServiciosRuta(int idruta, int id);

        /// <summary>
        /// agrega un punto a la ruta indicada
        /// </summary>
        /// <param name="datosPunto"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void AgregarPtoRuta(PtoRuta datosPunto);

        /// <summary>
        /// elimina punto de ruta indicada
        /// </summary>
        /// <param name="datosPunto"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void EliminarPtoRuta(PtoRuta datosPunto);

        /// <summary>
        /// crear punto
        /// </summary>
        /// <param name="datosPunto"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void CrearPunto(PtoRuta datosPunto);

        /// <summary>
        /// asigna posicion en ruta a punto indicado
        /// </summary>
        /// <param name="datosPunto"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void OrganizarPtos(PtoRuta datosPunto);

        /// <summary>
        /// obtiene todos los medios de transporte
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RUMedioTransporte> ObtenerMediosTransporte();

        /// <summary>
        /// obtiene todos lod tipos de vehiculos
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RUTipoVehiculo> ObtenerTiposVehiculos();

        /// <summary>
        /// obtiene todos los tipos de ruta
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<RUTipoRuta> ObtenerTiposRuta();

        /// <summary>
        /// crea nueva ruta
        /// </summary>
        /// <param name="ruta"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        int CrearRuta(RURutaICWeb ruta);

        #endregion
    }
}