using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using Framework.Servidor.Servicios.ContratoDatos.Seguridad;
using Framework.Servidor.Servicios.ContratoDatos.Versionamiento;

namespace Framework.Servidor.Servicios.Contratos
{
    [ServiceContract(Namespace = "http://contrologis.com")]
    public interface IPAParametrosFramework
    {
        /// <summary>
        /// Retorna el id del operador postal de la localidad dada
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        PAOperadorPostal ObtenerOperadorPostalLocalidad(string idLocalidad);

        /// <summary>
        /// Retorna el porcentaje del recargo de combustible para el operador postal
        /// </summary>
        /// <param name="idZona"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        decimal ObtenerPorcentajeRecargoCombustibleOPxZona(string idZona);

        /// <summary>
        /// Retorna el valor de un dólar en pesos
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        double? ObtenerValorDolarEnPesos();

        /// <summary>
        /// Retorna la lista de condiciones dado el operador postal
        /// </summary>
        /// <param name="idOperadorPostal"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<PACondicionOperadorPostalDC> ObtenerCondicionesPorOperadorPostal(int idOperadorPostal);

        /// <summary>
        /// Obtiene todos los medios de transporte
        /// </summary>
        /// <returns>Lista con los medios de transporte</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<PAMedioTransporte> ObtenerTodosMediosTrasporte();

        /// <summary>
        /// Consulta los parametros del framework
        /// </summary>
        /// <param name="llave">Llave del parametro</param>
        /// <returns>Valor del parametro</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        PAParametro ConsultarParametrosFramework(string llave);

        /// <summary>
        /// Consulta los parametros generales del Framework
        /// </summary>
        /// <param name="llave">Nombre del parametro</param>
        /// <returns>Valor el parametro</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        Dictionary<string, string> ConsultarListaParametrosFramework();

        /// <summary>
        /// Consulta los tipos de identificacion
        /// </summary>
        /// <returns>Lista de tipos de identificacion</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<PATipoIdentificacion> ConsultarTiposIdentificacion();

        /// <summary>
        /// Consultar las ocupaciones
        /// </summary>
        /// <returns>Lista de ocupaciones</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<PAOcupacionDC> ConsultarOcupacion();

        /// <summary>
        /// Obtiene una lista con los regimenes contributivos
        /// </summary>
        /// <returns>objeto lista de regimenes contributivos</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<PATipoRegimenDC> ObtenerRegimenContributivo();

        /// <summary>
        /// Obtiene una lista con los segmentos de mercado
        /// </summary>
        /// <returns>objeto lista de los segmentos de mercado</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<PATipoSegmentoDC> ObtenerSegmentoMercado();

        /// <summary>
        /// Obtiene una lista con los tipos de sociedad
        /// </summary>
        /// <returns>objeto lista de los tipos de sociedad</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<PATipoSociedadDC> ObtenerTipoSociedad();

        /// <summary>
        /// Consulta todas las zonas
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GenericoConsultasFramework<Framework.Servidor.Servicios.ContratoDatos.Parametros.PAZonaDC> ObtenerZonas(System.Collections.Generic.IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool esAscendente);

        /// <summary>
        /// Consulta las localidades en zona por el id de zona
        /// </summary>
        /// <param name="IdZona"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GenericoConsultasFramework<Framework.Servidor.Servicios.ContratoDatos.Parametros.PALocalidadDC> ConsultarLocalidadEnZonaXZona(string idZona, System.Collections.Generic.IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool esAscendente);

        /// <summary>
        /// Inserta una nueva zona
        /// </summary>
        /// <param name="zona"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void InsertarZona(Framework.Servidor.Servicios.ContratoDatos.Parametros.PAZonaDC zona);

        /// <summary>
        /// Inserta una nueva localidad perteneciente a una zona
        /// </summary>
        /// <param name="localidadEnZona"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GuardarCambiosLocalidadEnZona(List<PAZonaLocalidad> localidadesEnZona);

        /// <summary>
        /// Modifica una zona
        /// </summary>
        /// <param name="zona"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ModificarZona(Framework.Servidor.Servicios.ContratoDatos.Parametros.PAZonaDC zona);

        /// <summary>
        /// Consulta la zona a la que está asociada una localidad
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        PAZonaDC ConsultarZonaDeLocalidad(string idLocalidad);


        /// <summary>
        /// Obtiene el radio de busqueda de las reqcogidas en una localidad
        /// </summary>
        /// <param name="Idlocalidad"></param>
        /// <returns></returns>

        int ObtenerRadioBusquedaRecogidaLocalidad(string Idlocalidad);

        /// <summary>
        /// Consulta los tipos de zona
        /// </summary>
        /// <returns>Lista con los tipos de zona</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<PATipoZona> ConsultarTipoZona();

        /// <summary>
        /// Elimina la informacion de una zona y sus relaciones
        /// </summary>
        /// <param name="IdZona"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void EliminarZona(string idZona);

        /// <summary>
        /// Obtiene las localidades aplicando el filtro y la paginacion
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="esAscendente"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GenericoConsultasFramework<PALocalidadDC> ObtenerLocalidades(System.Collections.Generic.IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool esAscendente);

        /// <summary>
        /// Elimina la informacion de una localidad y sus relaciones
        /// </summary>
        /// <param name="IdLocalidad"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void EliminarLocalidad(string idLocalidad);

        /// <summary>
        /// Consulta los tipos de localidad
        /// </summary>
        /// <returns>Lista con los tipos de localidad</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<PATipoLocalidad> ConsultarTipoLocalidad();

        /// <summary>
        /// Inserta una nueva localidad
        /// </summary>
        /// <param name="localidad">Objeto con la informacion de la localidad</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void InsertarLocalidad(PALocalidadDC localidad);

        /// <summary>
        /// Modifica una localidad
        /// </summary>
        /// <param name="localidad"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ModificarLocalidad(PALocalidadDC localidad);

        /// <summary>
        /// Consulta las zonas de localidad por el id de la localidad
        /// </summary>
        /// <param name="IdLocalidad"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GenericoConsultasFramework<PAZonaDC> ConsultarZonaDeLocalidadXLocalidad(string idLocalidad, System.Collections.Generic.IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool esAscendente);

        /// <summary>
        /// Inserta una nueva zona asignada a una localidad
        /// </summary>
        /// <param name="localidadEnZona"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GuardarCambiosZonaDeLocalidad(List<PAZonaLocalidad> zonaDelocalidades);

        /// <summary>
        /// Consulta las localidades por tipo de localidad
        /// </summary>
        /// <param name="IdTipoLocalidad"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<PALocalidadDC> ConsultarLocalidadesXTipoLocalidad(PAEnumTipoLocalidad tipoLocalidad);

        /// <summary>
        /// Consulta las localidades por el padre y por el tipo de localidad
        /// </summary>
        /// <param name="IdTipoLocalidad"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<PALocalidadDC> ConsultarLocalidadesXidPadreXidTipo(string idPadre, PAEnumTipoLocalidad tipoLocalidad);

        /// <summary>
        /// Consulta las ciudades por departamento
        /// </summary>
        /// <param name="idDepto">Id del departamento</param>
        /// /// <param name="SoloMunicipios">Indica si solo se selecciona municipios o municipios + corregimientos inspecciones  caserios...</param>
        /// <returns>Lista de localidades</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<PALocalidadDC> ConsultarLocalidadesXDepartamento(string idDepto, bool SoloMunicipios);

        /// <summary>
        /// Retorna la lista de países
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<PALocalidadDC> ObtenerPaises();

        /// <summary>
        /// Retorna la lista de localidad que no son países ni departamentos.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<PALocalidadDC> ObtenerLocalidadesNoPaisNoDepartamento();

        /// <summary>
        /// Retorna la lista de localidades que no son países ni departamentos para Colombia
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<PALocalidadDC> ObtenerLocalidadesNoPaisNoDepartamentoColombia();

        /// <summary>
        /// Retorna la lista de localidades que no son países ni departamentos para un país dado, se usa para internacional
        /// </summary>
        /// <param name="idPais"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<PALocalidadDC> ObtenerLocalidadesNoPaisNoDepartamentoPorPais(string idPais);

        /// <summary>
        /// Consulta las zonas de una localidad
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <returns>lista de zonas</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<PAZonaDC> ConsultarZonasDeLocalidadXLocalidad(string idLocalidad);

        /// <summary>
        /// Consulta las Condiciones del Operador Postal Rafael Ramirez 28-12-2011
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
        IEnumerable<PACondicionOperadorPostalDC> ObtenerConOperadorPostal(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros);

        /// <summary>
        /// Adiciona, Edita o Elimina una condicion del Operador Postal Rafael Ramirez 28-12-2011
        /// </summary>
        /// <param name="conOperadorPostal"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizarCondOperadorPostal(PACondicionOperadorPostalDC conOperadorPostal);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<PAOperadorPostal> ObtenerOperadorPostal();

        /// <summary>
        /// Obtiene los grupos para realizar la divulgacion
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        PADivulgacion ObtenerGruposDivulgacion(int idAlerta);

        /// <summary>
        /// Obtiene los estados de la aplicación
        /// </summary>
        /// <returns>Colección con los estados</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<PAEstadoActivoInactivoDC> ObtenerEstadosAplicacion();

        /// <summary>
        /// Consulta la fecha del servidor
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        DateTime ConsultarFechaServidor();

        #region Representante Legal

        /// <summary>
        /// Actualiza un responsable legal
        /// </summary>
        /// <param name="responsable">Objeto responsable legal</param>
        /// <returns>Id del responsable legal</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        long ActualizarResponsableLegal(PAResponsableLegal responsable);

        /// <summary>
        /// Obtiene lista de responsable legal
        /// </summary>
        /// <returns>Lista con los responsables legales</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GenericoConsultasFramework<PAResponsableLegal> ObtenerResponsableLegal(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente);

        /// <summary>
        /// Consulta un responsable legal dependiendo de su id
        /// </summary>
        /// <param name="idResponsable">Id del responsable legal</param>
        /// <returns>Responsable legal</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        PAResponsableLegal ObtenerResponsableLegalXIdResponsable(long idResponsable);

        #endregion Representante Legal

        #region CRUD Tipo de actividad economica

        /// <summary>
        /// Otiene los tipos de actividad
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consulta</param>
        /// <returns>Lista de tipos de actividad economica</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GenericoConsultasFramework<PATipoActEconomica> ObtenerTiposActividadEconomica(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente);

        /// <summary>
        /// Adiciona, modifica o elimina un tipo de actividad Economica
        /// </summary>
        /// <param name="docuCentroServ">Objeto tipo de actividad economica</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizarTipoActividadEconomica(PATipoActEconomica tipoActEconomica);

        /// <summary>
        /// Obtiene todos los tipos de actividad economica
        /// </summary>
        /// <returns>Lista con los tipos de actividad economica</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<PATipoActEconomica> ObtenerTodosTiposActividadEconomica();

        #endregion CRUD Tipo de actividad economica

        #region Bancos

        /// <summary>
        /// Obtiene los bancos
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consulta</param>
        /// <returns>Lista de tipos de Bancos</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GenericoConsultasFramework<PABanco> ObtenerBancos(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente);

        /// <summary>
        /// Adiciona modifica o elimina un banco
        /// </summary>
        /// <param name="banco">Objeto banco</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizarBanco(PABanco banco);

        /// <summary>
        /// Obtiene todos los bancos
        /// </summary>
        /// <returns>Lista con los bancos</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<PABanco> ObtenerTodosBancos();

        /// <summary>
        /// Obtiene todos los tipos de cuenta
        /// </summary>
        /// <returns>Lista con los tipos de cuenta</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<PATipoCuenta> ObtenerTiposCuentaBanco();

        #endregion Bancos

        #region Lista restrictiva

        /// <summary>
        /// Valida si una identificacion se encuentra en listas restrictivas
        /// </summary>
        /// <param name="identificacion"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        bool ValidarListaRestrictiva(string identificacion);

        #endregion Lista restrictiva

        #region Responsable Servicio

        /// <summary>
        /// Obtiene los Responsables de los servicios
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consulta</param>
        /// <returns>Lista con los responsables de los servicios</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GenericoConsultasFramework<PAResponsableServicio> ObtenerResponsableServicio(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente);

        /// <summary>
        /// Adiciona modifica o elimina un responsable de servicio
        /// </summary>
        /// <param name="banco">Objeto responsable de servicio</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        long ActualizarResponsableServicio(PAResponsableServicio responsable);

        #endregion Responsable Servicio

        #region persona interna

        /// <summary>
        /// Obtiene las personas internas
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consulta</param>
        /// <returns>Lista con los responsables de los servicios</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GenericoConsultasFramework<PAPersonaInternaDC> ObtenerPersonasFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente);

        #endregion persona interna

        #region Dias

        /// <summary>
        /// Obtiene todos los dias
        /// </summary>
        /// <returns>Lista con los dias</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<PADia> ObtenerTodosDias();

        #endregion Dias

        #region Semanas

        /// <summary>
        /// Obtiene todas las semanas
        /// </summary>
        /// <returns>Lista con las semanas</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<PASemanaDC> ObtenerTodasSemanas();

        #endregion Semanas

        #region Meses

        /// <summary>
        /// Obtiene todos los meses
        /// </summary>
        /// <returns>Lista con los meses</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<PAMesDC> ObtenerTodosMeses();

        #endregion Meses

        #region Zonas

        /// <summary>
        /// Obtener las zonas de localidades
        /// </summary>
        /// <returns>Colección con las zonas de localidades</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<PAZonaDC> ObtenerZonasDeLocalidad();

        #endregion Zonas

        #region Lista Restricitva

        /// <summary>
        /// Obtiene los datos de la lista restrictiva
        /// </summary>
        /// <param name="filtro">Filtro</param>
        /// <param name="campoOrdenamiento">Campo de Ordenamiento</param>
        /// <param name="indicePagina">Indice de página</param>
        /// <param name="registrosPorPagina">Registro por página</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de Registros</param>
        /// <returns>Colección lista restrictiva</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GenericoConsultasFramework<PAListaRestrictivaDC> ObtenerListaRestrictiva(System.Collections.Generic.IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool esAscendente);

        /// <summary>
        /// Adiciona, edita o elimina una lista restrictiva
        /// </summary>
        /// <param name="listaRestricitva">Objeto lista restrictiva</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizarListaRestrictiva(PAListaRestrictivaDC listaRestricitva);

        /// <summary>
        /// Obtiene los tipos de lista restrictiva
        /// </summary>
        /// <returns>Colección tipo de lista restrictiva</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<PATipoListaRestrictivaDC> ObtenerTiposListaRestrictiva();

        #endregion Lista Restricitva

        #region Consecutivo

        /// <summary>
        /// Obtiene
        /// </summary>
        /// <param name="idConsecutivo">Identificador consecutivo</param>
        /// <returns>Objeto Consecutivo</returns>
        //[OperationContract]
        //[FaultContract(typeof(ControllerException))]
        //PAConsecutivoDC ObtenerConsecutivo(short idConsecutivo);

        #endregion Consecutivo

        #region Parientes

        /// <summary>
        /// Método para obtener los parentezcos configurados
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<PAParienteDC> ObtenerParientes();

        #endregion Parientes

        #region Estado empaque

        /// <summary>
        /// Obtiene los estados del empaque para un peso dado
        /// </summary>
        /// <param name="peso"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<PAEstadoEmpaqueDC> ObtenerEstadosEmpaque(decimal peso);

        /// <summary>
        /// Obtiene los estados del empaque
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<PAEstadoEmpaqueDC> ObtenerTodosEstadosEmpaque();

        #endregion Estado empaque

        #region Unidades de medida

        /// <summary>
        /// Retorna las unidades de medida
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<PAUnidadMedidaDC> ObtenerUnidadesMedida();

        #endregion Unidades de medida

        #region Tipo Sector Cliente

        /// <summary>
        /// Obtiene todos los tipos de sector de cliente
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<Framework.Servidor.Servicios.ContratoDatos.Parametros.PATipoSectorCliente> ObtenerTodosTipoSectorCliente();

        /// <summary>
        /// Obtener tipos de sector de cliente
        /// </summary>
        /// <typeparam name="?"></typeparam>
        /// <typeparam name="?"></typeparam>
        /// <param name="?"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GenericoConsultasFramework<Framework.Servidor.Servicios.ContratoDatos.Parametros.PATipoSectorCliente> ObtenerTipoSectorCliente(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente);

        /// <summary>
        /// inserta Modifica o elimina un  tipo de sector de cliente
        /// </summary>
        /// <param name="tipoSectorCliente"></param>
        ///
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizarTipoSectorCliente(Framework.Servidor.Servicios.ContratoDatos.Parametros.PATipoSectorCliente tipoSectorCliente);

        #endregion Tipo Sector Cliente

        /// <summary>
        ///Obtiene los Operadores Postales de la Zona
        /// </summary>
        /// <returns>lista de operadores Postales de zona con tiempos de entrega</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<PAOperadorPostalZonaDC> ObtenerOperadorPostalZona(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros);

        /// <summary>
        /// Gestiona el Crud de operador postal por Zona
        /// </summary>
        /// <param name="operadorPostalZona"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GestionOperadorPostalZona(PAOperadorPostalZonaDC operadorPostalZona);

        /// <summary>
        /// Obtiene la Zona y el tipo de Zona
        /// correspondiente
        /// </summary>
        /// <returns>lista de zonas y su tipo</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<PAZonaDC> ObtenerListadoZonas();

        /// <summary>
        /// Actualiza un Operador postal
        /// </summary>
        /// <param name="operador"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizarOperadorPostal(PAOperadorPostal operador);

        /// <summary>
        /// Obtiene el operador postal
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
        GenericoConsultasFramework<PAOperadorPostal> ObtenerOperadorPostalPaginado(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente);

        /// <summary>
        /// Consulta la url en la cual se encuentra ubicada la app de carga masiva de guias o  facturas
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        string ConsultarURLAppCargaMasiva();

        /// <summary>
        /// Obtiene los numeros de guia que no tienen imagen en el servidor
        /// </summary>
        /// <param name="idCol"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<string> ConsultarArchivosPendientesSincronizar(long idCol);

        /// <summary>
        /// Retorna la imagen de publicidad asociada al login de la aplicación, si no retorna nada, no requiere visualizar publicidad
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ParamPublicidad ConsultarImagenPublicidadLogin();

        /// <summary>
        /// Consulta las variaciones y abreviaciones del campo Direccion
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<PAEstandarDireccionDC> ConsultarAbreviacionesVariacionesDireccion();

        /// <summary>
        /// Consulta la informacion de la entidad segun rol
        /// </summary>
        /// <param name="codigo"></param>
        /// <param name="rol"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        PAPropiedadesAplicacionDC ConsultarPropiedadesAplicacion(string codigo, int rol);

        #region Menus
        /// <summary>
        /// Obtiene los menus
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consulta</param>
        /// <returns>Lista de tipos de Menus</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GenericoConsultasFramework<VEMenuCapacitacion> ObtenerMenus(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente);

        /// <summary>
        /// Adiciona modifica o elimina un menu
        /// </summary>
        /// <param name="menu">Objeto banco</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizarMenu(VEMenuCapacitacion menu);

        /// <summary>
        /// Adiciona modifica o elimina un menu padre
        /// </summary>
        /// <param name="menuPadre">Objeto banco</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizarMenuPadre(List<VEMenuCapacitacion> menuPadre);
        #endregion

        #region Imagen Publicidad Guia
        /// <summary>
        /// Envia los parametrosd para guardar la imagen de publicidad en la url que se encuentre en la bd de parametros admisiones
        /// </summary>
        /// <param name="rutaImagen"></param>
        /// <param name="imagenPublicidad"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GuardarImagenPublicidad(string rutaImagen, string imagenPublicidad);
        #endregion

       

        /// <summary>
        /// Obtiene todos los dispositivos de registrados de los peatones o los empleados
        /// </summary>
        /// <param name="tipoDispositivo">Indica si se filtra por los dispositivos de los empleados o los peatones</param>
        /// <returns></returns>
        List<PADispositivoMovil> ObtenerTodosDispositivosPeatonEmpleados(PAEnumTiposDispositivos tipoDispositivo);

        /// <summary>
        /// Obtiene los dispositivos moviles de los empleados en una ciudad
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <returns></returns>
        List<PADispositivoMovil> ObtenerDispositivosMovilesEmpleadosCiudad(string idLocalidad, bool esControllerApp = false);

        /// <summary>
        ///  Obtiene los dispositivos el dispositivo movil asociado a la identificacion de un empleado
        /// </summary>
        /// <param name="numeroIdentificacion"></param>
        /// <returns></returns>
        List<PADispositivoMovil> ObtenerDispositivosMovilesIdentificacionEmpleado(long numeroIdentificacion);

        /// <summary>
        /// Obtiene los dispositivos el dispositivo movil asociado a una empleado
        /// </summary>
        /// <param name="nombUsuario"></param>
        /// <returns></returns>
        PADispositivoMovil ObtenerDispositivoMovilEmpleado(string nombUsuario);

        /// <summary>
        /// Obtiene un dispositivo movil a partir del token y del sistema operativo
        /// </summary>
        /// <param name="tokenMovil"></param>
        /// <returns></returns>
        PADispositivoMovil ObtenerDispositivoMovilTokenOs(string tokenMovil, PAEnumOsDispositivo sistemaOperativo);



        /// <summary>
        /// Obtiene la lista de programas Congeladores de Disco duro
        /// </summary>
        /// <returns></returns>

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<PAFreezersDiscoDC> ObtenerListaFreezersDisco();


        /// <summary>
        /// Obtiene el listado de días festivos que hay entre una fecha y otra
        /// </summary>
        /// <param name="fechadesde">Fecha desde la cual se hará la consulta</param>
        /// <param name="fechahasta">Fecha hasta la cua se hará la consulta</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<DateTime> ObtenerFestivos(DateTime fechadesde, DateTime fechahasta, string idPais);

        


        /// <summary>
        /// Verifica si la base de  datos está disponible
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        bool VerificarConexionBD();
    }
}