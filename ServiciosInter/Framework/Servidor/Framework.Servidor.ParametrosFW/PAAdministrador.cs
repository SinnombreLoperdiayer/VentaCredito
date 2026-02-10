using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.Servidor.Comun;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using Framework.Servidor.Servicios.ContratoDatos.Parametros.Consecutivos;
using Framework.Servidor.Servicios.ContratoDatos.Versionamiento;
using System.Xml;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Web.Script.Serialization;

namespace Framework.Servidor.ParametrosFW
{
    /// <summary>
    /// Fachada de parámetros
    /// </summary>
    public class PAAdministrador
    {
        #region Instancia Singleton

        /// <summary>
        /// Instancia de la clase
        /// </summary>
        private static readonly PAAdministrador instancia = new PAAdministrador();

        /// <summary>
        /// Instancia de la clase
        /// </summary>
        public static PAAdministrador Instancia
        {
            get { return PAAdministrador.instancia; }
        }

        #endregion Instancia Singleton

        #region Constructor

        public PAAdministrador()
        {
        }

        #endregion Constructor

        public const string RutaArchivos = "Controller.RutaDescargaArchivos";
        public const string RutaArchivosDivulgacion = "Controller.RutaArchivosDivulgacion";

        #region Operaciones de Localidad

        /// <summary>
        /// Retorna la lista de países
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PALocalidadDC> ObtenerPaises()
        {
            return PALocalidad.Instancia.ObtenerPaises();
        }

        /// <summary>
        /// Consulta las localidades aplicando el filtro y la paginacion
        /// </summary>
        /// <param name="IdTipoLocalidad"></param>
        /// <returns></returns>
        public List<PALocalidadDC> ConsultarLocalidades(IDictionary<string, string> filtro, out int totalRegistros, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool esAscendente)
        {
            return PALocalidad.Instancia.ConsultarLocalidades(filtro, out totalRegistros, campoOrdenamiento, indicePagina, registrosPorPagina, esAscendente);
        }

        /// <summary>
        /// Elimina la informacion de una localidad y sus relaciones
        /// </summary>
        /// <param name="IdLocalidad"></param>
        public void EliminarLocalidad(string idLocalidad)
        {
            PALocalidad.Instancia.EliminarLocalidad(idLocalidad);
        }

        /// <summary>
        /// Consulta los tipos de localidad
        /// </summary>
        /// <returns>Lista con los tipos de localidad</returns>
        public List<PATipoLocalidad> ConsultarTipoLocalidad()
        {
            return PALocalidad.Instancia.ConsultarTipoLocalidad();
        }

        /// <summary>
        /// Inserta una nueva localidad
        /// </summary>
        /// <param name="localidad">Objeto con la informacion de la localidad</param>
        public void InsertarLocalidad(PALocalidadDC localidad)
        {
            PALocalidad.Instancia.InsertarLocalidad(localidad);
        }

        /// <summary>
        /// Modifica una localidad
        /// </summary>
        /// <param name="localidad"></param>
        public void ModificarLocalidad(PALocalidadDC localidad)
        {
            PALocalidad.Instancia.ModificarLocalidad(localidad);
        }

        /// <summary>
        /// Consulta las zonas de localidad por el id de la localidad
        /// </summary>
        /// <param name="IdLocalidad"></param>
        /// <returns></returns>
        public List<PAZonaDC> ConsultarZonaDeLocalidadXLocalidad(string idLocalidad, IDictionary<string, string> filtro, out int totalRegistros, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool esAscendente)
        {
            return PALocalidad.Instancia.ConsultarZonaDeLocalidadXLocalidad(idLocalidad, filtro, out totalRegistros, campoOrdenamiento, indicePagina, registrosPorPagina, esAscendente);
        }

        /// <summary>
        /// Obtiene los festivos entre dos fechas pero no los agrega a la cache, con el fin de consultar fechas por meses
        /// </summary>
        /// <returns></returns>
        public List<DateTime> ObtenerFestivosSinCache(DateTime fechaDesde, DateTime fechaHasta, string pais)
        {
            return PAParametros.Instancia.ObtenerFestivosSinCache(fechaDesde, fechaHasta, pais);
        }

        /// <summary>
        /// Inserta una nueva localidad perteneciente a una zona
        /// </summary>
        /// <param name="localidadEnZona"></param>
        public void GuardarCambiosZonaDeLocalidad(List<PAZonaLocalidad> zonaDelocalidades)
        {
            PALocalidad.Instancia.GuardarCambiosZonaDeLocalidad(zonaDelocalidades);
        }

        /// <summary>
        /// Consulta las localidades por tipo de localidad
        /// </summary>
        /// <param name="IdTipoLocalidad"></param>
        /// <returns></returns>
        public List<PALocalidadDC> ConsultarLocalidadesXTipoLocalidad(PAEnumTipoLocalidad tipoLocalidad)
        {
            return PALocalidad.Instancia.ConsultarLocalidadesXTipoLocalidad(tipoLocalidad);
        }

        /// <summary>
        /// Retorna la lista de localidad que no son países ni departamentos.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PALocalidadDC> ObtenerLocalidadesNoPaisNoDepartamento()
        {
            return PALocalidad.Instancia.ObtenerLocalidadesNoPaisNoDepartamento();
        }

        /// <summary>
        /// Retorna la lista de localidad que no son países ni departamentos para Colombia
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PALocalidadDC> ObtenerLocalidadesNoPaisNoDepartamentoColombia()
        {
            return PALocalidad.Instancia.ObtenerLocalidadesNoPaisNoDepartamentoColombia();
        }

        /// <summary>
        /// Retorna la lista de localidades que no son países ni departamentos para un país dado, se usa para internacional
        /// </summary>
        /// <param name="idPais"></param>
        /// <returns></returns>
        public IEnumerable<PALocalidadDC> ObtenerLocalidadesNoPaisNoDepartamentoPorPais(string idPais)
        {
            return PALocalidad.Instancia.ObtenerLocalidadesNoPaisNoDepartamentoPorPais(idPais);
        }

        /// <summary>
        /// Retorna la informacion de una localidad por el id de ella misma
        /// </summary>
        /// <returns></returns>
        public PALocalidadDC ObtenerInformacionLocalidad(string idLocalidad)
        {
            return PALocalidad.Instancia.ObtenerInformacionLocalidad(idLocalidad);
        }

        /// <summary>
        /// Consulta las localidades por el padre y por el tipo de localidad
        /// </summary>
        /// <param name="IdTipoLocalidad"></param>
        /// <returns></returns>
        public List<PALocalidadDC> ConsultarLocalidadesXidPadreXidTipo(string idPadre, PAEnumTipoLocalidad tipoLocalidad)
        {
            return PALocalidad.Instancia.ConsultarLocalidadesXidPadreXidTipo(idPadre, tipoLocalidad);
        }

        /// <summary>
        /// Consulta las ciudades por departamento
        /// </summary>
        /// <param name="idDepto">Id del departamento</param>
        /// <param name="SoloMunicipios">Indica si solo se selecciona municipios o municipios + corregimientos inspecciones  caserios...</param>
        /// <returns>Lista de localidades</returns>
        public List<PALocalidadDC> ConsultarLocalidadesXDepartamento(string idDepto, bool SoloMunicipios)
        {
            return PALocalidad.Instancia.ConsultarLocalidadesXDepartamento(idDepto, SoloMunicipios);
        }

        /// <summary>
        /// Obtener la localidad por su identificación
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <returns></returns>
        public PALocalidadDC ObtenerLocalidadPorId(string idLocalidad)
        {
            return PALocalidad.Instancia.ObtenerLocalidadPorId(idLocalidad);
        }


        /// <summary>
        /// Obtiene el radio de busqueda de las reqcogidas en una localidad
        /// </summary>
        /// <param name="Idlocalidad"></param>
        /// <returns></returns>
        public int ObtenerRadioBusquedaRecogidaLocalidad(string Idlocalidad)
        {
            return PALocalidad.Instancia.ObtenerRadioBusquedaRecogidaLocalidad(Idlocalidad);
        }


        #endregion Operaciones de Localidad

        #region Operaciones de Zona

        /// <summary>
        /// Consulta todas las zonas
        /// </summary>
        /// <returns></returns>
        public List<Framework.Servidor.Servicios.ContratoDatos.Parametros.PAZonaDC> ObtenerZonas(System.Collections.Generic.IDictionary<string, string> filtro, out int totalRegistros, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool esAscendente)
        {
            return PAZona.Instancia.ObtenerZonas(filtro, out totalRegistros, campoOrdenamiento, indicePagina, registrosPorPagina, esAscendente);
        }

        /// <summary>
        /// Consulta las localidades en zona por el id de zona
        /// </summary>
        /// <param name="IdZona"></param>
        /// <returns></returns>
        public List<Framework.Servidor.Servicios.ContratoDatos.Parametros.PALocalidadDC> ConsultarLocalidadEnZonaXZona(string idZona, IDictionary<string, string> filtro, out int totalRegistros, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool esAscendente)
        {
            return PAZona.Instancia.ConsultarLocalidadEnZonaXZona(idZona, filtro, out totalRegistros, campoOrdenamiento, indicePagina, registrosPorPagina, esAscendente);
        }

        /// <summary>
        /// Inserta una nueva zona
        /// </summary>
        /// <param name="zona"></param>
        public void InsertarZona(Framework.Servidor.Servicios.ContratoDatos.Parametros.PAZonaDC zona)
        {
            PAZona.Instancia.InsertarZona(zona);
        }

        /// <summary>
        /// Inserta una nueva localidad perteneciente a una zona
        /// </summary>
        /// <param name="localidadEnZona"></param>
        public void GuardarCambiosLocalidadEnZona(List<PAZonaLocalidad> localidadesEnZona)
        {
            PAZona.Instancia.GuardarCambiosLocalidadEnZona(localidadesEnZona);
        }

        /// <summary>
        /// Modifica una zona
        /// </summary>
        /// <param name="zona"></param>
        public void ModificarZona(Framework.Servidor.Servicios.ContratoDatos.Parametros.PAZonaDC zona)
        {
            PAZona.Instancia.ModificarZona(zona);
        }

        /// <summary>
        /// Consulta los tipos de zona
        /// </summary>
        /// <returns>Lista con los tipos de zona</returns>
        public List<PATipoZona> ConsultarTipoZona()
        {
            return PAZona.Instancia.ConsultarTipoZona();
        }

        /// <summary>
        /// Elimina la informacion de una zona y sus relaciones
        /// </summary>
        /// <param name="IdZona"></param>
        public void EliminarZona(string idZona)
        {
            PAZona.Instancia.EliminarZona(idZona);
        }

        /// <summary>
        /// Consulta las zonas de una localidad
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <returns>lista de zonas</returns>
        public IList<PAZonaDC> ConsultarZonasDeLocalidadXLocalidad(string idLocalidad)
        {
            return PAZona.Instancia.ConsultarZonasDeLocalidadXLocalidad(idLocalidad);
        }

        /// <summary>
        /// Consulta la zona a la que está asociada una localidad
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <returns></returns>
        public PAZonaDC ConsultarZonaDeLocalidad(string idLocalidad)
        {
            return PAZona.Instancia.ConsultarZonaDeLocalidad(idLocalidad);
        }

        #endregion Operaciones de Zona

        #region Operaciones de Parámetros

        /// <summary>
        /// Obtiene todos los medios de transporte
        /// </summary>
        /// <returns>Lista con los medios de transporte</returns>
        public IList<PAMedioTransporte> ObtenerTodosMediosTrasporte()
        {
            return PAParametros.Instancia.ObtenerTodosMediosTrasporte();
        }

        /// <summary>
        /// Consulta los parametros generales del Framework
        /// </summary>
        /// <param name="llave">Nombre del parametro</param>
        /// <returns>Valor el parametro</returns>
        public string ConsultarParametrosFramework(string llave)
        {
            return PAParametros.Instancia.ConsultarParametrosFramework(llave);
        }



        /// <summary>
        /// Consulta los tipos de identificacion
        /// </summary>
        /// <returns>Lista de tipos de identificacion</returns>
        public IList<PATipoIdentificacion> ConsultarTiposIdentificacion()
        {
            return PAParametros.Instancia.ConsultarTiposIdentificacion();
        }

        /// <summary>
        /// Consultar las ocupaciones
        /// </summary>
        /// <returns>Lista de ocupaciones</returns>
        public IList<PAOcupacionDC> ConsultarOcupacion()
        {
            return PAParametros.Instancia.ConsultarOcupacion();
        }

        /// <summary>
        /// Obtiene una lista con los regimenes contributivos
        /// </summary>
        /// <returns>objeto lista de regimenes contributivos</returns>
        public IList<PATipoRegimenDC> ObtenerRegimenContributivo()
        {
            return PAParametros.Instancia.ObtenerRegimenContributivo();
        }

        /// <summary>
        /// Obtiene una lista con los segmentos de mercado
        /// </summary>
        /// <returns>objeto lista de los segmentos de mercado</returns>
        public IList<PATipoSegmentoDC> ObtenerSegmentoMercado()
        {
            return PAParametros.Instancia.ObtenerSegmentoMercado();
        }

        /// <summary>
        /// Obtiene una lista con los tipos de sociedad
        /// </summary>
        /// <returns>objeto lista de los tipos de sociedad</returns>
        public IList<PATipoSociedadDC> ObtenerTipoSociedad()
        {
            return PAParametros.Instancia.ObtenerTipoSociedad();
        }

        /// <summary>
        /// Obtiene lista de responsable legal
        /// </summary>
        /// <returns>Lista con los responsables legales</returns>
        public IList<PAResponsableLegal> ObtenerResponsableLegal(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return PAParametros.Instancia.ObtenerResponsableLegal(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Actualiza un responsable legal
        /// </summary>
        /// <param name="responsable">Objeto responsable legal</param>
        /// <returns>Id del responsable legal</returns>
        public long ActualizarResponsableLegal(PAResponsableLegal responsable)
        {
            return PAParametros.Instancia.ActualizarResponsableLegal(responsable);
        }

        /// <summary>
        /// Consulta un responsable legal dependiendo de su id
        /// </summary>
        /// <param name="idResponsable">Id del responsable legal</param>
        /// <returns>Responsable legal</returns>
        public PAResponsableLegal ObtenerResponsableLegal(long idResponsable)
        {
            return PAParametros.Instancia.ObtenerResponsableLegal(idResponsable);
        }

        /// <summary>
        /// Valida si una identificacion se encuentra en listas restrictivas
        /// </summary>
        /// <param name="identificacion"></param>
        /// <returns></returns>
        public bool ValidarListaRestrictiva(string identificacion)
        {
            return PAParametros.Instancia.ValidarListaRestrictiva(identificacion);
        }

        /// <summary>
        /// Enviar correo electronico cuando un cliente se encuentre en listas restrictivas
        /// </summary>
        /// <param name="identificacion"></param>
        /// <param name="idCentroServicios"></param>
        /// <param name="nombreCentroServicios"></param>
        public void EnviarCorreoListasRestrictivas(string identificacion, long idCentroServicios, string nombreCentroServicios)
        {
            PAParametros.Instancia.EnviarCorreoListasRestrictivas(identificacion, idCentroServicios, nombreCentroServicios);
        }

        /// <summary>
        /// Enviar correo electronico cuando una agencia supero el tope maximo de operacion
        /// </summary>
        /// <param name="identificacion"></param>
        /// <param name="idCentroServicios"></param>
        /// <param name="nombreCentroServicios"></param>
        public void EnviarCorreoAgenciaSuperaMaxOpe(long idCentroServicio, string nombreCentroServicios)
        {
            PAParametros.Instancia.EnviarCorreoAgenciaSuperaMaxOpe(idCentroServicio, nombreCentroServicios);
        }

        /// <summary>
        /// Envia Correo a un solo destinatario con dos valores
        /// diferentes en contenido y asunto
        /// </summary>
        /// <param name="tipoMensaje"></param>
        /// <param name="destinatario"></param>
        /// <param name="valor01"></param>
        /// <param name="valor02"></param>
        public void EnviarCorreoUnicoDestinatarioDosVariables(int tipoMensaje, string destinatario, string valor01, string valor02)
        {
            PAParametros.Instancia.EnviarCorreoUnicoDestinatarioDosVariables(tipoMensaje, destinatario, valor01, valor02);
        }

        /// <summary>
        /// Enviar correo electronico cuando un cliente se encuentre en listas restrictivas
        /// </summary>
        /// <param name="identificacion"></param>
        /// <param name="idCentroServicios"></param>
        /// <param name="nombreCentroServicios"></param>
        public void EnviarCorreoTelemercadeoGiroRemitente(string destinoGiro, string ciudad, string direccion, string destinatario)
        {
            PAParametros.Instancia.EnviarCorreoTelemercadeoGiroRemitente(destinoGiro, ciudad, direccion, destinatario);
        }

        /// <summary>
        /// Enviar correo electronico cuando un cliente se encuentre en listas restrictivas
        /// </summary>
        /// <param name="identificacion"></param>
        /// <param name="idCentroServicios"></param>
        /// <param name="nombreCentroServicios"></param>
        public void EnviarCorreoTelemercadeoGiroDetinatario(string remitenteGiro, string ciudadDestino, string direccionDestino, DateTime fechadisponibilidad, string destinatario)
        {
            PAParametros.Instancia.EnviarCorreoTelemercadeoGiroDestinatario(remitenteGiro, ciudadDestino, direccionDestino, fechadisponibilidad, destinatario);
        }

        /// <summary>
        /// Envia correo al remitente de un envio en custodia
        /// </summary>
        /// <param name="destinoGiro"></param>
        /// <param name="ciudad"></param>
        /// <param name="direccion"></param>
        /// <param name="destinatario"></param>
        public void EnviarCorreologisticaInversa(string destinatario, string asunto, string mensaje)
        {
            PAParametros.Instancia.EnviarCorreologisticaInversa(destinatario, asunto, mensaje);
        }

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
        public IList<PATipoActEconomica> ObtenerTiposActividadEconomica(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return PAParametros.Instancia.ObtenerTiposActividadEconomica(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Adiciona, modifica o elimina un tipo de actividad Economica
        /// </summary>
        /// <param name="docuCentroServ">Objeto tipo de actividad economica</param>
        public void ActualizarTipoActividadEconomica(PATipoActEconomica tipoActEconomica)
        {
            PAParametros.Instancia.ActualizarTipoActividadEconomica(tipoActEconomica);
        }

        /// <summary>
        /// Obtiene todos los tipos de actividad economica
        /// </summary>
        /// <returns>Lista con los tipos de actividad economica</returns>
        public IList<PATipoActEconomica> ObtenerTiposActividadEconomica()
        {
            return PAParametros.Instancia.ObtenerTiposActividadEconomica();
        }

        /// <summary>
        /// Obtiene el valor del AppSetting de acuerdo al key suministrado
        /// </summary>
        /// <param name="key"></param>
        /// <returns>String con el parametro conrrespondiente al key del AppSettings</returns>
        public string ObtenerParametrosAplicacion(string key)
        {
            return PAParametros.Instancia.ObtenerParametrosAplicacion(key);
        }

        /// <summary>
        /// Otiene los bancos
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consulta</param>
        /// <returns>Lista de tipos de Bancos</returns>
        public IList<PABanco> ObtenerBancos(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return PAParametros.Instancia.ObtenerBancos(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Adiciona modifica o elimina un banco
        /// </summary>
        /// <param name="banco">Objeto banco</param>
        public void ActualizarBanco(PABanco banco)
        {
            PAParametros.Instancia.ActualizarBanco(banco);
        }

        /// <summary>
        /// Obtiene todos los bancos
        /// </summary>
        /// <returns>Lista con los bancos</returns>
        public IList<PABanco> ObtenerTodosBancos()
        {
            return PAParametros.Instancia.ObtenerTodosBancos();
        }

        /// <summary>
        /// Obtiene todos los tipos de cuenta
        /// </summary>
        /// <returns>Lista con los tipos de cuenta</returns>
        public IList<PATipoCuenta> ObtenerTiposCuentaBanco()
        {
            return PAParametros.Instancia.ObtenerTiposCuentaBanco();
        }

        /// <summary>
        /// Obtiene los Tipos de Documentos de Banco
        /// </summary>
        /// <returns>lista de los Tipos de Doc Banco</returns>
        public IList<PATipoDocumBancoDC> ObtenerTiposDocumentosBanco()
        {
            return PAParametros.Instancia.ObtenerTiposDocumentosBanco();
        }

        /// <summary>
        /// Obtiene la lista de las condiciones del Operador Postal Rafael Ramirez 28-12-2011
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <returns></returns>
        public IEnumerable<PACondicionOperadorPostalDC> ObtenerConOperadorPostal(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return PAParametros.Instancia.ObtenerConOperadorPostal(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Adiciona, Edita o Elimina una condicion del Operador Postal Rafael Ramirez 28-12-2011
        /// </summary>
        /// <param name="conOperadorPostal"></param>
        public void ActualizarCondOperadorPostal(PACondicionOperadorPostalDC conOperadorPostal)
        {
            PAParametros.Instancia.ActualizarCondOperadorPostal(conOperadorPostal);
        }

        /// <summary>
        /// Obtiene la lista de los Operadores Postales Rafael Ramirez 29-12-2011
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PAOperadorPostal> ObtenerOperadorPostal()
        {
            return PAParametros.Instancia.ObtenerOperadorPostal();
        }

        /// <summary>
        /// Metodo Para obtener un Consecutivo
        /// enviando el tipo de consecutivo
        /// </summary>
        /// <param name="tipoConsecutivo"></param>
        /// <returns></returns>
        public long ObtenerConsecutivo(PAEnumConsecutivos idConsecutivo)
        {
            return PAParametros.Instancia.ObtenerConsecutivo(idConsecutivo);
        }

        /// <summary>
        /// Metodo Para obtener un Consecutivo Por Col
        /// enviando el tipo de consecutivo
        /// </summary>
        /// <param name="tipoConsecutivo"></param>
        /// <returns></returns>
        public long ObtenerConsecutivoPorCol(PAEnumConsecutivos idConsecutivo, long idCol)
        {
            return PAParametros.Instancia.ObtenerConsecutivoPorCol(idConsecutivo, idCol);
        }

        /// <summary>
        ///Metodo para obtener una plantilla del framework
        /// </summary>
        /// <param name="archivo"></param>
        /// <returns>Archivo</returns>
        public byte[] ObtenerPlantillaFramework(long idPlantilla)
        {
            return PAParametros.Instancia.ObtenerPlantillaFramework(idPlantilla);
        }

        /// <summary>
        /// Obtiene los grupos para realizar la divulgacion
        /// </summary>
        /// <returns></returns>
        public PADivulgacion ObtenerGruposDivulgacion(int idAlerta)
        {
            return PAParametros.Instancia.ObtenerGruposDivulgacion(idAlerta);
        }

        #endregion Operaciones de Parámetros

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
        public IList<PAResponsableServicio> ObtenerResponsableServicio(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return PAParametros.Instancia.ObtenerResponsableServicio(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Adiciona modifica o elimina un responsable de servicio
        /// </summary>
        /// <param name="banco">Objeto responsable de servicio</param>
        public long ActualizarResponsableServicio(PAResponsableServicio responsable)
        {
            return PAParametros.Instancia.ActualizarResponsableServicio(responsable);
        }

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
        public IEnumerable<PAPersonaInternaDC> ObtenerPersonasFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return PAParametros.Instancia.ObtenerPersonasFiltro(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Obtiene el consecutivo para el archivo de intento de entrega
        /// </summary>
        /// <param name="idConsecutivo">identificador consecutivo</param>
        /// <param name="idCol">identificador del col</param>
        /// <returns>consecutivo</returns>
        public long ObtenerConsecutivoIntentoEntregaPorCol(PAEnumConsecutivos idConsecutivo, long idCol)
        {
            return PAParametros.Instancia.ObtenerConsecutivoIntentoEntregaPorCol(idConsecutivo, idCol);
        }

        #endregion persona interna

        #region Dias

        /// <summary>
        /// Obtiene todos los dias
        /// </summary>
        /// <returns>Lista con los dias</returns>
        public IList<PADia> ObtenerTodosDias()
        {
            return PAParametros.Instancia.ObtenerTodosDias();
        }

        #endregion Dias

        #region Semanas

        /// <summary>
        /// Obtiene todas las semanas
        /// </summary>
        /// <returns>Lista con las semanas</returns>
        public IList<PASemanaDC> ObtenerTodasSemanas()
        {
            return PAParametros.Instancia.ObtenerTodasSemanas();
        }

        #endregion Semanas

        #region Meses

        /// <summary>
        /// Obtiene todos los meses
        /// </summary>
        /// <returns>Lista con los meses</returns>
        public IList<PAMesDC> ObtenerTodosMeses()
        {
            return PAParametros.Instancia.ObtenerTodosMeses();
        }

        public PAConsecutivoDC CrearRangoConsecutivoIntentoEntrega(PAEnumConsecutivos cajas_PruebasEntrega, long idCentroLogistico, string creadoPor)
        {
            return PAParametros.Instancia.CrearRangoConsecutivoIntentoEntrega(cajas_PruebasEntrega, idCentroLogistico, creadoPor);
        }

        #endregion Meses

        #region Zonas

        /// <summary>
        /// Obtener las zonas de localidades
        /// </summary>
        /// <returns>Colección con las zonas de localidades</returns>
        public IEnumerable<PAZonaDC> ObtenerZonasDeLocalidad()
        {
            return PAParametros.Instancia.ObtenerZonasDeLocalidad();
        }

        #endregion Zonas

        #region Calendario Festivos x Pais

        /// <summary>
        /// Obtiene el número de días hábiles que hay entre una fecha y otra teniendo en cuenta el sábado como día hábil
        /// </summary>
        /// <param name="fechadesde">Fecha desde la cual se hará la consulta</param>
        /// <param name="fechahasta">Fecha hasta la cua se hará la consulta</param>
        /// <returns></returns>
        public int ObtenerDiasHabiles(DateTime fechadesde, DateTime fechahasta, string idPais)
        {
            return PAParametros.Instancia.ObtenerDiasHabiles(fechadesde, fechahasta, idPais);
        }

        /// <summary>
        /// Obtiene la lista de programas Congeladores de Disco duro
        /// </summary>
        /// <returns></returns>
        public List<PAFreezersDiscoDC> ObtenerListaFreezersDisco()
        {
            return PAParametros.Instancia.ObtenerListaFreezersDisco();
        }

        /// <summary>
        /// Obtiene el listado de días festivos que hay entre una fecha y otra
        /// </summary>
        /// <param name="fechadesde">Fecha desde la cual se hará la consulta</param>
        /// <param name="fechahasta">Fecha hasta la cua se hará la consulta</param>
        /// <returns></returns>
        public List<DateTime> ObtenerFestivos(DateTime fechadesde, DateTime fechahasta, string idPais)
        {
            return PAParametros.Instancia.ObtenerFestivos(fechadesde, fechahasta, idPais);
        }

        /// <summary>
        /// Obtiene la fecha hábil más próxima desde una fecha inicial sumando un número de días específicos teniendo en cuenta los sábados como días hábiles
        /// </summary>
        /// <param name="fechadesde">Fecha desde la cual se hará la consulta</param>
        /// <param name="numerodias">Número de días que se quiere sumar a la fecha desde</param>
        public DateTime ObtenerFechaFinalHabil(DateTime fechadesde, double numerodias, string idPais)
        {
            return PAParametros.Instancia.ObtenerFechaFinalHabil(fechadesde, numerodias, idPais);
        }

        /// <summary>
        /// Obtiene la fecha hábil más próxima desde una fecha inicial sumando un número de días específicos teniendo en cuenta los sábados como días hábiles
        /// </summary>
        /// <param name="fechadesde">Fecha desde la cual se hará la consulta</param>
        /// <param name="numerodias">Número de días que se quiere sumar a la fecha desde</param>
        public DateTime ObtenerFechaFinalHabilSinSabados(DateTime fechadesde, double numerodias, string idPais)
        {
            return PAParametros.Instancia.ObtenerFechaFinalHabilSinSabados(fechadesde, numerodias, idPais);
        }

        /// <summary>
        /// Retorn la cantidad de domingos que hay entre una fecha y otra
        /// </summary>
        /// <param name="fechadesde">Fecha desde la cual se hará la consulta</param>
        /// <param name="fechahasta">Fecha hasta la cua se hará la consulta</param>
        /// <returns></returns>
        public int ObtenerNumeroDeDomingos(DateTime fechadesde, DateTime fechahasta)
        {
            return PAParametros.Instancia.ObtenerNumeroDeDomingos(fechadesde, fechahasta);
        }

        /// <summary>
        /// Método para obtener los agregar los dias laborales a una fecha predeterminada
        /// </summary>
        /// <param name="fechaOriginal"></param>
        /// <param name="diasLaborables"></param>
        /// <returns></returns>
        public DateTime AgregarDiasLaborales(DateTime fechaOriginal, int diasLaborables)
        {
            return PAParametros.Instancia.AgregarDiasLaborales(fechaOriginal, diasLaborables);
        }

        /// <summary>
        /// Método para obtener los agregar los dias laborales entre dos fechas
        /// </summary>
        /// <param name="fechaOriginal"></param>
        /// <param name="diasLaborables"></param>
        /// <returns></returns>
        public int ConsultarDiasLaborales(DateTime fechaInicial, DateTime fechaFinal)
        {
            return PAParametros.Instancia.ConsultarDiasLaborales(fechaInicial, fechaFinal);
        }

        #endregion Calendario Festivos x Pais

        #region Lista Restrictiva

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
        public IEnumerable<PAListaRestrictivaDC> ObtenerListaRestrictiva(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return PAListaNegra.Instancia.ObtenerListaRestrictiva(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Adiciona, edita o elimina una lista restrictiva
        /// </summary>
        /// <param name="listaRestricitva">Objeto lista restrictiva</param>
        public void ActualizarListaRestrictiva(PAListaRestrictivaDC listaRestricitva)
        {
            PAListaNegra.Instancia.ActualizarListaRestrictiva(listaRestricitva);
        }

        /// <summary>
        /// Obtiene los tipos de lista restrictiva
        /// </summary>
        /// <returns>Colección tipo de lista restrictiva</returns>
        public IEnumerable<PATipoListaRestrictivaDC> ObtenerTiposListaRestrictiva()
        {
            return PAListaNegra.Instancia.ObtenerTiposListaRestrictiva();
        }

        /// <summary>
        /// Obtiene los estados de la aplicación
        /// </summary>
        /// <returns>Colección con los estados</returns>
        public IEnumerable<PAEstadoActivoInactivoDC> ObtenerEstadosAplicacion()
        {
            return PAParametros.Instancia.ObtenerEstadosAplicacion();
        }

        #endregion Lista Restrictiva

        #region Consecutivo

        /// <summary>
        /// Obtiene
        /// </summary>
        /// <param name="idConsecutivo">Identificador consecutivo</param>
        /// <returns>Objeto Consecutivo</returns>
        //public PAConsecutivoDC ObtenerConsecutivo(short idConsecutivo)
        //{
        //  return PAParametros.Instancia.ObtenerConsecutivo(idConsecutivo);
        //}

        /// <summary>
        /// Obtiene la caja actual
        /// </summary>
        /// <param name="idTipoConsecutivo">Tipo de consecutivo</param>
        /// <returns>Objeto Consecutivo</returns>
        public PAConsecutivoDC ObtenerDatosConsecutivo(PAEnumConsecutivos idConsecutivo)
        {
            return PAParametros.Instancia.ObtenerDatosConsecutivo(idConsecutivo);
        }

        /// <summary>
        /// Obtiene la caja actual
        /// </summary>
        /// <param name="idTipoConsecutivo">Tipo de consecutivo</param>
        /// <returns>Objeto Consecutivo</returns>
        public PAConsecutivoDC ObtenerDatosConsecutivoxCol(PAEnumConsecutivos idConsecutivo, long idCentroLogistico)
        {
            return PAParametros.Instancia.ObtenerDatosConsecutivoxCol(idConsecutivo, idCentroLogistico);
        }

        /// <summary>
        /// Obtiene la caja actual para el intento de entrega
        /// </summary>
        /// <param name="idTipoConsecutivo">Tipo de consecutivo</param>
        /// <returns>Objeto Consecutivo</returns>
        public PAConsecutivoDC ObtenerDatosConsecutivoIntentoEntregaxCol(PAEnumConsecutivos idConsecutivo, long idCentroLogistico)
        {
            return PAParametros.Instancia.ObtenerDatosConsecutivoIntentoEntregaxCol(idConsecutivo, idCentroLogistico);
        }

        #endregion Consecutivo

        #region Parientes

        /// <summary>
        /// Método para obtener los parentezcos configurados
        /// </summary>
        /// <returns></returns>
        public IList<PAParienteDC> ObtenerParientes()
        {
            return PAParametros.Instancia.ObtenerParientes();
        }

        #endregion Parientes

        #region Estados Empaque

        /// <summary>
        /// Obtiene los estados del empaque para un peso dado
        /// </summary>
        /// <param name="peso"></param>
        /// <returns></returns>
        public List<PAEstadoEmpaqueDC> ObtenerEstadosEmpaque(decimal peso)
        {
            return PAParametros.Instancia.ObtenerEstadosEmpaque(peso);
        }

        /// <summary>
        /// Obtiene los estados del empaque
        /// </summary>
        /// <returns></returns>
        public List<PAEstadoEmpaqueDC> ObtenerTodosEstadosEmpaque()
        {
            return PAParametros.Instancia.ObtenerTodosEstadosEmpaque();
        }

        #endregion Estados Empaque

        #region Datos empresa

        #endregion Datos empresa

        /// <summary>
        /// Retorna la lista de condiciones dado el operador postal
        /// </summary>
        /// <param name="idOperadorPostal"></param>
        /// <returns></returns>
        public IEnumerable<PACondicionOperadorPostalDC> ObtenerCondicionesPorOperadorPostal(int idOperadorPostal)
        {
            return PAParametros.Instancia.ObtenerCondicionesPorOperadorPostal(idOperadorPostal);
        }

        /// <summary>
        /// Retorna el id del operador postal de la localidad dada
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <returns></returns>
        public PAOperadorPostal ObtenerOperadorPostalLocalidad(string idLocalidad)
        {
            return PAParametros.Instancia.ObtenerOperadorPostalLocalidad(idLocalidad);
        }

        /// <summary>
        /// Retorna el porcentaje del recargo de combustible para el operador postal
        /// </summary>
        /// <param name="idZona"></param>
        /// <returns></returns>
        public decimal ObtenerPorcentajeRecargoCombustibleOPxZona(string idZona)
        {
            return PAParametros.Instancia.ObtenerPorcentajeRecargoCombustibleOPxZona(idZona);
        }

        /// <summary>
        /// Retorna el valor de un dólar en pesos
        /// </summary>
        /// <returns></returns>
        public double? ObtenerValorDolarEnPesos()
        {
            double valor = 0;
            if (Cache.Instancia.ContainsKey(Framework.Servidor.Comun.ConstantesFramework.CACHE_DOLAR_EN_PESOS))
            {
                double.TryParse(Cache.Instancia[ConstantesFramework.CACHE_DOLAR_EN_PESOS].ToString(), out valor);
            }
            else
            {
                // Si no está en caché, entonces consultar el valor, primero intentar por el webservice
                try
                {
                    //valor = Integraciones.ConversionMoneda.ConversorMoneda.Instancia.ObtenerValorDolar();
                    XmlDocument xmlDoc = new XmlDocument();
                    XmlDocument xmlResponse = new XmlDocument();

                    var url = "http://www.datos.gov.co/api/id/32sa-8pi3.json?$select=`valor`,`vigenciadesde`&$order=`vigenciadesde`+DESC&$limit=1";
                    var webrequest = (HttpWebRequest)System.Net.WebRequest.Create(url);

                    using (var response = webrequest.GetResponse())
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        var streamResult = reader.ReadToEnd();
                        var objetoJson = JsonConvert.DeserializeXmlNode(streamResult).InnerXml;
                        var paValorDolar = new JavaScriptSerializer().Deserialize<PAValorDolar>(objetoJson);
                        valor = paValorDolar.Valor;
                    }
                }
                catch (Exception e)
                {
                    Framework.Servidor.Excepciones.AuditoriaTrace.EscribirAuditoria(Framework.Servidor.Excepciones.ETipoAuditoria.Error, Framework.Servidor.Excepciones.MensajesFramework.CargarMensaje(Framework.Servidor.Excepciones.ETipoErrorFramework.EX_ERROR_NO_SE_PUDO_CONECTAR_WS_DOLAR), ConstantesFramework.PARAMETROS_FRAMEWORK, e);
                }

                // Si el valor no se pudo obtener del webservice, entonces obtenerlo de parámetros de Framework
                if (valor == 0)
                {
                    double? valorParametro = PAParametros.Instancia.ObtenerValorDolarEnPesos();
                    if (valorParametro.HasValue)
                    {
                        valor = valorParametro.Value;
                    }
                    else
                    {
                        return null;
                    }
                }

                Cache.Instancia.Add(ConstantesFramework.CACHE_DOLAR_EN_PESOS, valor);
            }
            return valor;
        }

        public List<PAUnidadMedidaDC> ObtenerUnidadesMedida()
        {
            return PAParametros.Instancia.ObtenerUnidadesMedida();
        }

        #region Tipo Sector Cliente

        /// <summary>
        /// Obtiene todos los tipos de sector de cliente
        /// </summary>
        /// <returns></returns>
        public IList<PATipoSectorCliente> ObtenerTodosTipoSectorCliente()
        {
            return PAParametros.Instancia.ObtenerTodosTipoSectorCliente();
        }

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
        public IList<PATipoSectorCliente> ObtenerTipoSectorCliente(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return PAParametros.Instancia.ObtenerTipoSectorCliente(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// inserta Modifica o elimina un  tipo de sector de cliente
        /// </summary>
        /// <param name="tipoSectorCliente"></param>
        public void ActualizarTipoSectorCliente(PATipoSectorCliente tipoSectorCliente)
        {
            PAParametros.Instancia.ActualizarTipoSectorCliente(tipoSectorCliente);
        }

        #endregion Tipo Sector Cliente

        /// <summary>
        ///Obtiene los Operadores Postales de la Zona
        /// </summary>
        /// <returns>lista de operadores Postales de zona con tiempos de entrega</returns>
        public List<PAOperadorPostalZonaDC> ObtenerOperadorPostalZona(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return PAParametros.Instancia.ObtenerOperadorPostalZona(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Gestiona el Crud de operador postal por Zona
        /// </summary>
        /// <param name="operadorPostalZona"></param>
        public void GestionOperadorPostalZona(PAOperadorPostalZonaDC operadorPostalZona)
        {
            PAParametros.Instancia.GestionOperadorPostalZona(operadorPostalZona);
        }

        /// <summary>
        /// Obtiene la Zona y el tipo de Zona
        /// correspondiente
        /// </summary>
        /// <returns>lista de zonas y su tipo</returns>
        public List<PAZonaDC> ObtenerListadoZonas()
        {
            return PAZona.Instancia.ObtenerListadoZonas();
        }

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
        public List<PAOperadorPostal> ObtenerOperadorPostal(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return PAParametros.Instancia.ObtenerOperadorPostal(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Actualiza un Operador postal
        /// </summary>
        /// <param name="operador"></param>
        public void ActualizarOperadorPostal(PAOperadorPostal operador)
        {
            PAParametros.Instancia.ActualizarOperadorPostal(operador);
        }
        /// <summary>
        /// Obtiene los numeros de guia que no tienen imagen en el servidor
        /// </summary>
        /// <param name="idCol"></param>
        /// <returns></returns>
        public List<string> ConsultarArchivosPendientesSincronizar(long idCol)
        {
            return PAParametros.Instancia.ConsultarArchivosPendientesSincronizar(idCol);
        }

        public ParamPublicidad ConsultarImagenPublicidadLogin()
        {
            return PAParametros.Instancia.ConsultarImagenPublicidadLogin();
        }

        /// <summary>
        /// Consulta la informacion de la entidad segun el rol
        /// </summary>
        /// <param name="codigo"></param>
        /// <param name="rol"></param>
        /// <returns></returns>
        public PAPropiedadesAplicacionDC ConsultarPropiedadesAplicacion(string codigo, int rol)
        {
            return PAParametros.Instancia.ConsultarPropiedadesAplicacion(codigo, rol);
        }

        #region Menus
        /// <summary>
        /// Otiene los Menus
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consulta</param>
        /// <returns>Lista de tipos de Menus</returns>
        public IList<VEMenuCapacitacion> ObtenerMenus(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return PAParametros.Instancia.ObtenerMenus(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Adiciona modifica o elimina un Menu
        /// </summary>
        /// <param name="Menu">Objeto Menu</param>
        public void ActualizarMenu(VEMenuCapacitacion menu)
        {
            PAParametros.Instancia.ActualizarMenu(menu);
        }
        #endregion

        #region Imagen Publicidad guia
        public void GuardarImagenPublicidad(string rutaImagen, string imagenPublicidad)
        {
            PAParametros.Instancia.GuardarImagenPublicidadGuia(rutaImagen, imagenPublicidad);
        }
        #endregion

    }
}