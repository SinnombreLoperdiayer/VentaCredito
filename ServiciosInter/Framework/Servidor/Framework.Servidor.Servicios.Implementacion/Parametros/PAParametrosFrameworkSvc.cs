using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using Framework.Servidor.Servicios.ContratoDatos.Seguridad;
using Framework.Servidor.Servicios.Contratos;
using Framework.Servidor.Servicios.ContratoDatos.Versionamiento;

namespace Framework.Servidor.Servicios.Implementacion
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class PAParametrosFrameworkSvc : IPAParametrosFramework
    {
        public PAParametrosFrameworkSvc()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(PAAdministrador.Instancia.ConsultarParametrosFramework("Cultura"));
        }

        /// <summary>
        /// Retorna el valor de un dólar en pesos
        /// </summary>
        /// <returns></returns>
        public double? ObtenerValorDolarEnPesos()
        {
            return PAAdministrador.Instancia.ObtenerValorDolarEnPesos();
        }

        /// <summary>
        /// Retorna la lista de condiciones dado el operador postal
        /// </summary>
        /// <param name="idOperadorPostal"></param>
        /// <returns></returns>
        public IEnumerable<PACondicionOperadorPostalDC> ObtenerCondicionesPorOperadorPostal(int idOperadorPostal)
        {
            return PAAdministrador.Instancia.ObtenerCondicionesPorOperadorPostal(idOperadorPostal);
        }

        /// <summary>
        /// Retorna el id del operador postal de la localidad dada
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <returns></returns>
        public PAOperadorPostal ObtenerOperadorPostalLocalidad(string idLocalidad)
        {
            return PAAdministrador.Instancia.ObtenerOperadorPostalLocalidad(idLocalidad);
        }

        /// <summary>
        /// Retorna el porcentaje del recargo de combustible para el operador postal
        /// </summary>
        /// <param name="idZona"></param>
        /// <returns></returns>
        public decimal ObtenerPorcentajeRecargoCombustibleOPxZona(string idZona)
        {
            return PAAdministrador.Instancia.ObtenerPorcentajeRecargoCombustibleOPxZona(idZona);
        }

        /// <summary>
        /// Obtiene todos los medios de transporte
        /// </summary>
        /// <returns>Lista con los medios de transporte</returns>
        public IList<PAMedioTransporte> ObtenerTodosMediosTrasporte()
        {
            return PAAdministrador.Instancia.ObtenerTodosMediosTrasporte();
        }

        /// <summary>
        /// Consulta los tipos de identificacion
        /// </summary>
        /// <returns>Lista de tipos de identificacion</returns>
        public IList<PATipoIdentificacion> ConsultarTiposIdentificacion()
        {
            return PAAdministrador.Instancia.ConsultarTiposIdentificacion();
        }

        /// <summary>
        /// Consultar las ocupaciones
        /// </summary>
        /// <returns>Lista de ocupaciones</returns>
        public IList<PAOcupacionDC> ConsultarOcupacion()
        {
            return PAAdministrador.Instancia.ConsultarOcupacion();
        }

        /// <summary>
        /// Obtiene los festivos entre dos fechas pero no los agrega a la cache, con el fin de consultar fechas por meses
        /// </summary>
        /// <returns></returns>
        public List<DateTime> ObtenerFestivosSinCache(DateTime fechaDesde, DateTime fechaHasta, string pais)
        {
            return PAAdministrador.Instancia.ObtenerFestivosSinCache(fechaDesde, fechaHasta, pais);
        }

        /// <summary>
        /// Consulta los parametros generales del Framework
        /// </summary>
        /// <param name="llave">Nombre del parametro</param>
        /// <returns>Valor el parametro</returns>
        public PAParametro ConsultarParametrosFramework(string llave)
        {
            return new PAParametro() { ValorParametro = PAAdministrador.Instancia.ConsultarParametrosFramework(llave) };
        }

        /// <summary>
        /// Consulta los parametros generales del Framework
        /// </summary>
        /// <param name="llave">Nombre del parametro</param>
        /// <returns>Valor el parametro</returns>
        public Dictionary<string, string> ConsultarListaParametrosFramework()
        {
            return PAParametros.Instancia.ConsultarListaParametrosFramework();
        }


        /// <summary>
        /// Obtiene una lista con los regimenes contributivos
        /// </summary>
        /// <returns>objeto lista de regimenes contributivos</returns>
        public IList<PATipoRegimenDC> ObtenerRegimenContributivo()
        {
            return PAAdministrador.Instancia.ObtenerRegimenContributivo();
        }

        /// <summary>
        /// Obtiene una lista con los segmentos de mercado
        /// </summary>
        /// <returns>objeto lista de los segmentos de mercado</returns>
        public IList<PATipoSegmentoDC> ObtenerSegmentoMercado()
        {
            return PAAdministrador.Instancia.ObtenerSegmentoMercado();
        }

        /// <summary>
        /// Obtiene una lista con los tipos de sociedad
        /// </summary>
        /// <returns>objeto lista de los tipos de sociedad</returns>
        public IList<PATipoSociedadDC> ObtenerTipoSociedad()
        {
            return PAAdministrador.Instancia.ObtenerTipoSociedad();
        }

        /// <summary>
        /// Consulta todas las zonas
        /// </summary>
        /// <returns></returns>
        public GenericoConsultasFramework<Framework.Servidor.Servicios.ContratoDatos.Parametros.PAZonaDC> ObtenerZonas(System.Collections.Generic.IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool esAscendente)
        {
            int totalRegistros = 0;
            return new GenericoConsultasFramework<PAZonaDC>()
            {
                Lista = Framework.Servidor.ParametrosFW.PAAdministrador.Instancia.ObtenerZonas(filtro, out totalRegistros, campoOrdenamiento, indicePagina, registrosPorPagina, esAscendente),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Consulta las localidades en zona por el id de zona
        /// </summary>
        /// <param name="IdZona"></param>
        /// <returns></returns>
        public GenericoConsultasFramework<Framework.Servidor.Servicios.ContratoDatos.Parametros.PALocalidadDC> ConsultarLocalidadEnZonaXZona(string idZona, System.Collections.Generic.IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool esAscendente)
        {
            int totalRegistros = 0;
            return new GenericoConsultasFramework<Framework.Servidor.Servicios.ContratoDatos.Parametros.PALocalidadDC>()
            {
                Lista = Framework.Servidor.ParametrosFW.PAAdministrador.Instancia.ConsultarLocalidadEnZonaXZona(idZona, filtro, out totalRegistros, campoOrdenamiento, indicePagina, registrosPorPagina, esAscendente),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Consulta la fecha del servidor
        /// </summary>
        /// <returns></returns>
        public DateTime ConsultarFechaServidor()
        {
            return DateTime.Now;
        }

        /// <summary>
        /// Inserta una nueva zona
        /// </summary>
        /// <param name="zona"></param>
        public void InsertarZona(Framework.Servidor.Servicios.ContratoDatos.Parametros.PAZonaDC zona)
        {
            Framework.Servidor.ParametrosFW.PAAdministrador.Instancia.InsertarZona(zona);
        }

        /// <summary>
        /// Inserta una nueva localidad perteneciente a una zona
        /// </summary>
        /// <param name="localidadEnZona"></param>
        public void GuardarCambiosLocalidadEnZona(List<PAZonaLocalidad> localidadesEnZona)
        {
            Framework.Servidor.ParametrosFW.PAAdministrador.Instancia.GuardarCambiosLocalidadEnZona(localidadesEnZona);
        }

        /// <summary>
        /// Modifica una zona
        /// </summary>
        /// <param name="zona"></param>
        public void ModificarZona(Framework.Servidor.Servicios.ContratoDatos.Parametros.PAZonaDC zona)
        {
            Framework.Servidor.ParametrosFW.PAAdministrador.Instancia.ModificarZona(zona);
        }

        /// <summary>
        /// Consulta los tipos de zona
        /// </summary>
        /// <returns>Lista con los tipos de zona</returns>
        public List<PATipoZona> ConsultarTipoZona()
        {
            return Framework.Servidor.ParametrosFW.PAAdministrador.Instancia.ConsultarTipoZona();
        }

        /// <summary>
        /// Elimina la informacion de una zona y sus relaciones
        /// </summary>
        /// <param name="IdZona"></param>
        public void EliminarZona(string idZona)
        {
            PAAdministrador.Instancia.EliminarZona(idZona);
        }

        /// <summary>
        /// Retorna la lista de países
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PALocalidadDC> ObtenerPaises()
        {
            return PAAdministrador.Instancia.ObtenerPaises();
        }

        /// <summary>
        /// Retorna la lista de localidad que no son países ni departamentos.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PALocalidadDC> ObtenerLocalidadesNoPaisNoDepartamento()
        {
            return PAAdministrador.Instancia.ObtenerLocalidadesNoPaisNoDepartamento();
        }

        /// <summary>
        /// Retorna la lista de localidades que no son países ni departamentos para Colombia
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PALocalidadDC> ObtenerLocalidadesNoPaisNoDepartamentoColombia()
        {
            return PAAdministrador.Instancia.ObtenerLocalidadesNoPaisNoDepartamentoColombia();
        }

        /// <summary>
        /// Retorna la lista de localidades que no son países ni departamentos para un país dado, se usa para internacional
        /// </summary>
        /// <param name="idPais"></param>
        /// <returns></returns>
        public IEnumerable<PALocalidadDC> ObtenerLocalidadesNoPaisNoDepartamentoPorPais(string idPais)
        {
            return PAAdministrador.Instancia.ObtenerLocalidadesNoPaisNoDepartamentoPorPais(idPais);
        }

        /// <summary>
        /// Obtiene las localidades aplicando el filtro y la paginacion
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="esAscendente"></param>
        /// <returns></returns>
        public GenericoConsultasFramework<PALocalidadDC> ObtenerLocalidades(System.Collections.Generic.IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool esAscendente)
        {
            int totalRegistros = 0;
            return new GenericoConsultasFramework<PALocalidadDC>()
            {
                Lista = Framework.Servidor.ParametrosFW.PAAdministrador.Instancia.ConsultarLocalidades(filtro, out totalRegistros, campoOrdenamiento, indicePagina, registrosPorPagina, esAscendente),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Elimina la informacion de una localidad y sus relaciones
        /// </summary>
        /// <param name="idLocalidad"></param>
        public void EliminarLocalidad(string idLocalidad)
        {
            PAAdministrador.Instancia.EliminarLocalidad(idLocalidad);
        }

        /// <summary>
        /// Consulta los tipos de localidad
        /// </summary>
        /// <returns>Lista con los tipos de localidad</returns>
        public List<PATipoLocalidad> ConsultarTipoLocalidad()
        {
            return PAAdministrador.Instancia.ConsultarTipoLocalidad();
        }

        /// <summary>
        /// Inserta una nueva localidad
        /// </summary>
        /// <param name="localidad">Objeto con la informacion de la localidad</param>
        public void InsertarLocalidad(PALocalidadDC localidad)
        {
            PAAdministrador.Instancia.InsertarLocalidad(localidad);
        }

        /// <summary>
        /// Modifica una localidad
        /// </summary>
        /// <param name="localidad"></param>
        public void ModificarLocalidad(PALocalidadDC localidad)
        {
            PAAdministrador.Instancia.ModificarLocalidad(localidad);
        }

        /// <summary>
        /// Consulta las zonas de localidad por el id de la localidad
        /// </summary>
        /// <param name="IdLocalidad"></param>
        /// <returns></returns>
        public GenericoConsultasFramework<PAZonaDC> ConsultarZonaDeLocalidadXLocalidad(string idLocalidad, System.Collections.Generic.IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool esAscendente)
        {
            int totalRegistros = 0;
            return new GenericoConsultasFramework<PAZonaDC>()
            {
                Lista = PAAdministrador.Instancia.ConsultarZonaDeLocalidadXLocalidad(idLocalidad, filtro, out totalRegistros, campoOrdenamiento, indicePagina, registrosPorPagina, esAscendente),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Inserta una nueva zona asignada a una localidad
        /// </summary>
        /// <param name="localidadEnZona"></param>
        public void GuardarCambiosZonaDeLocalidad(List<PAZonaLocalidad> zonaDelocalidades)
        {
            PAAdministrador.Instancia.GuardarCambiosZonaDeLocalidad(zonaDelocalidades);
        }

        /// <summary>
        /// Consulta las localidades por tipo de localidad
        /// </summary>
        /// <param name="IdTipoLocalidad"></param>
        /// <returns></returns>
        public List<PALocalidadDC> ConsultarLocalidadesXTipoLocalidad(PAEnumTipoLocalidad tipoLocalidad)
        {
            return PAAdministrador.Instancia.ConsultarLocalidadesXTipoLocalidad(tipoLocalidad);
        }

        /// <summary>
        /// Consulta las localidades por el padre y por el tipo de localidad
        /// </summary>
        /// <param name="IdTipoLocalidad"></param>
        /// <returns></returns>
        public List<PALocalidadDC> ConsultarLocalidadesXidPadreXidTipo(string idPadre, PAEnumTipoLocalidad tipoLocalidad)
        {
            return PAAdministrador.Instancia.ConsultarLocalidadesXidPadreXidTipo(idPadre, tipoLocalidad);
        }

        /// <summary>
        /// Consulta las ciudades por departamento
        /// </summary>
        /// <param name="idDepto">Id del departamento</param>
        /// /// <param name="SoloMunicipios">Indica si solo se selecciona municipios o municipios + corregimientos inspecciones  caserios...</param>
        /// <returns>Lista de localidades</returns>
        public List<PALocalidadDC> ConsultarLocalidadesXDepartamento(string idDepto, bool SoloMunicipios)
        {
            return PAAdministrador.Instancia.ConsultarLocalidadesXDepartamento(idDepto, SoloMunicipios);
        }

        /// <summary>
        /// Consulta las zonas de una localidad
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <returns>lista de zonas</returns>
        public IList<PAZonaDC> ConsultarZonasDeLocalidadXLocalidad(string idLocalidad)
        {
            return PAAdministrador.Instancia.ConsultarZonasDeLocalidadXLocalidad(idLocalidad);
        }

        /// <summary>
        /// Consulta la zona a la que está asociada una localidad
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <returns></returns>
        public PAZonaDC ConsultarZonaDeLocalidad(string idLocalidad)
        {
            return PAAdministrador.Instancia.ConsultarZonaDeLocalidad(idLocalidad);
        }

        /// <summary>
        /// Obtiene el radio de busqueda de las reqcogidas en una localidad
        /// </summary>
        /// <param name="Idlocalidad"></param>
        /// <returns></returns>
        public int ObtenerRadioBusquedaRecogidaLocalidad(string Idlocalidad)
        {
            return PAAdministrador.Instancia.ObtenerRadioBusquedaRecogidaLocalidad(Idlocalidad);
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
            return PAAdministrador.Instancia.ObtenerConOperadorPostal(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Adiciona, Edita o Elimina una condicion del Operador Postal Rafael Ramirez 28-12-2011
        /// </summary>
        /// <param name="conOperadorPostal"></param>
        public void ActualizarCondOperadorPostal(PACondicionOperadorPostalDC conOperadorPostal)
        {
            PAAdministrador.Instancia.ActualizarCondOperadorPostal(conOperadorPostal);
        }

        /// <summary>
        /// Obtiene la lista de los Operadores Postales Rafael Ramirez 29-12-2011
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PAOperadorPostal> ObtenerOperadorPostal()
        {
            return PAAdministrador.Instancia.ObtenerOperadorPostal();
        }

        /// <summary>
        /// Obtiene los grupos para realizar la divulgacion
        /// </summary>
        /// <returns></returns>
        public PADivulgacion ObtenerGruposDivulgacion(int idAlerta)
        {
            return PAAdministrador.Instancia.ObtenerGruposDivulgacion(idAlerta);
        }

        /// <summary>
        /// Obtiene los estados de la aplicación
        /// </summary>
        /// <returns>Colección con los estados</returns>
        public IEnumerable<PAEstadoActivoInactivoDC> ObtenerEstadosAplicacion()
        {
            return PAAdministrador.Instancia.ObtenerEstadosAplicacion();
        }

        #region Representante Legal

        /// <summary>
        /// Obtiene lista de responsable legal
        /// </summary>
        /// <returns>Lista con los responsables legales</returns>
        public GenericoConsultasFramework<PAResponsableLegal> ObtenerResponsableLegal(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente)
        {
            int totalRegistros = 0;
            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<PAResponsableLegal>()
            {
                Lista = PAAdministrador.Instancia.ObtenerResponsableLegal(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Actualiza un responsable legal
        /// </summary>
        /// <param name="responsable">Objeto responsable legal</param>
        /// <returns>Id del responsable legal</returns>
        public long ActualizarResponsableLegal(PAResponsableLegal responsable)
        {
            return PAAdministrador.Instancia.ActualizarResponsableLegal(responsable);
        }

        /// <summary>
        /// Consulta un responsable legal dependiendo de su id
        /// </summary>
        /// <param name="idResponsable">Id del responsable legal</param>
        /// <returns>Responsable legal</returns>
        public PAResponsableLegal ObtenerResponsableLegalXIdResponsable(long idResponsable)
        {
            var d = PAAdministrador.Instancia.ObtenerResponsableLegal(idResponsable);
            return d;
        }

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
        public GenericoConsultasFramework<PATipoActEconomica> ObtenerTiposActividadEconomica(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente)
        {
            int totalRegistros = 0;
            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<PATipoActEconomica>()
            {
                Lista = PAAdministrador.Instancia.ObtenerTiposActividadEconomica(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Adiciona, modifica o elimina un tipo de actividad Economica
        /// </summary>
        /// <param name="docuCentroServ">Objeto tipo de actividad economica</param>
        public void ActualizarTipoActividadEconomica(PATipoActEconomica tipoActEconomica)
        {
            PAAdministrador.Instancia.ActualizarTipoActividadEconomica(tipoActEconomica);
        }

        /// <summary>
        /// Obtiene todos los tipos de actividad economica
        /// </summary>
        /// <returns>Lista con los tipos de actividad economica</returns>
        public IList<PATipoActEconomica> ObtenerTodosTiposActividadEconomica()
        {
            return PAAdministrador.Instancia.ObtenerTiposActividadEconomica();
        }

        #endregion CRUD Tipo de actividad economica

        #region Bancos

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
        public GenericoConsultasFramework<PABanco> ObtenerBancos(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente)
        {
            int totalRegistros = 0;
            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<PABanco>()
            {
                Lista = PAAdministrador.Instancia.ObtenerBancos(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Adiciona modifica o elimina un banco
        /// </summary>
        /// <param name="banco">Objeto banco</param>
        public void ActualizarBanco(PABanco banco)
        {
            PAAdministrador.Instancia.ActualizarBanco(banco);
        }

        /// <summary>
        /// Obtiene todos los bancos
        /// </summary>
        /// <returns>Lista con los bancos</returns>
        public IList<PABanco> ObtenerTodosBancos()
        {
            return PAAdministrador.Instancia.ObtenerTodosBancos();
        }

        /// <summary>
        /// Obtiene todos los tipos de cuenta
        /// </summary>
        /// <returns>Lista con los tipos de cuenta</returns>
        public IList<PATipoCuenta> ObtenerTiposCuentaBanco()
        {
            return PAAdministrador.Instancia.ObtenerTiposCuentaBanco();
        }

        #endregion Bancos

        #region Lista restrictiva

        /// <summary>
        /// Valida si una identificacion se encuentra en listas restrictivas
        /// </summary>
        /// <param name="identificacion"></param>
        /// <returns></returns>
        public bool ValidarListaRestrictiva(string identificacion)
        {
            return PAAdministrador.Instancia.ValidarListaRestrictiva(identificacion);
        }

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
        public GenericoConsultasFramework<PAResponsableServicio> ObtenerResponsableServicio(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente)
        {
            int totalRegistros = 0;
            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<PAResponsableServicio>()
            {
                Lista = PAAdministrador.Instancia.ObtenerResponsableServicio(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Adiciona modifica o elimina un responsable de servicio
        /// </summary>
        /// <param name="banco">Objeto responsable de servicio</param>
        public long ActualizarResponsableServicio(PAResponsableServicio responsable)
        {
            return PAAdministrador.Instancia.ActualizarResponsableServicio(responsable);
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
        public GenericoConsultasFramework<PAPersonaInternaDC> ObtenerPersonasFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente)
        {
            int totalRegistros = 0;
            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<PAPersonaInternaDC>()
            {
                Lista = PAAdministrador.Instancia.ObtenerPersonasFiltro(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros),
                TotalRegistros = totalRegistros
            };
        }

        #endregion persona interna

        #region Dias

        /// <summary>
        /// Obtiene todos los dias
        /// </summary>
        /// <returns>Lista con los dias</returns>
        public IList<PADia> ObtenerTodosDias()
        {
            return PAAdministrador.Instancia.ObtenerTodosDias();
        }

        #endregion Dias

        #region Semanas

        /// <summary>
        /// Obtiene todas las semanas
        /// </summary>
        /// <returns>Lista con las semanas</returns>
        public IList<PASemanaDC> ObtenerTodasSemanas()
        {
            return PAAdministrador.Instancia.ObtenerTodasSemanas();
        }

        #endregion Semanas

        #region Meses

        /// <summary>
        /// Obtiene todos los meses
        /// </summary>
        /// <returns>Lista con los meses</returns>
        public IList<PAMesDC> ObtenerTodosMeses()
        {
            return PAAdministrador.Instancia.ObtenerTodosMeses();
        }

        #endregion Meses

        #region Zonas

        /// <summary>
        /// Obtener las zonas de localidades
        /// </summary>
        /// <returns>Colección con las zonas de localidades</returns>
        public IEnumerable<PAZonaDC> ObtenerZonasDeLocalidad()
        {
            return PAAdministrador.Instancia.ObtenerZonasDeLocalidad();
        }

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
        public Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<PAListaRestrictivaDC> ObtenerListaRestrictiva(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente)
        {
            int totalRegistros = 0;
            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<PAListaRestrictivaDC>()
            {
                Lista = PAAdministrador.Instancia.ObtenerListaRestrictiva(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Adiciona, edita o elimina una lista restrictiva
        /// </summary>
        /// <param name="listaRestricitva">Objeto lista restrictiva</param>
        public void ActualizarListaRestrictiva(PAListaRestrictivaDC listaRestricitva)
        {
            PAAdministrador.Instancia.ActualizarListaRestrictiva(listaRestricitva);
        }

        /// <summary>
        /// Obtiene los tipos de lista restrictiva
        /// </summary>
        /// <returns>Colección tipo de lista restrictiva</returns>
        public IEnumerable<PATipoListaRestrictivaDC> ObtenerTiposListaRestrictiva()
        {
            return PAAdministrador.Instancia.ObtenerTiposListaRestrictiva();
        }

        #endregion Lista Restricitva

        #region Consecutivo

        ///// <summary>
        ///// Obtiene
        ///// </summary>
        ///// <param name="idConsecutivo">Identificador consecutivo</param>
        ///// <returns>Objeto Consecutivo</returns>
        //public PAConsecutivoDC ObtenerConsecutivo(short idConsecutivo)
        //{
        //  return PAAdministrador.Instancia.ObtenerConsecutivo(idConsecutivo);
        //}

        #endregion Consecutivo

        #region Parientes

        /// <summary>
        /// Método para obtener los parentezcos configurados
        /// </summary>
        /// <returns></returns>
        public IList<PAParienteDC> ObtenerParientes()
        {
            return PAAdministrador.Instancia.ObtenerParientes();
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
            return PAAdministrador.Instancia.ObtenerEstadosEmpaque(peso);
        }

        /// <summary>
        /// Obtiene los estados del empaque
        /// </summary>
        /// <returns></returns>
        public List<PAEstadoEmpaqueDC> ObtenerTodosEstadosEmpaque()
        {
            return PAAdministrador.Instancia.ObtenerTodosEstadosEmpaque();
        }

        #endregion Estados Empaque

        #region Unidad de MEdida

        /// <summary>
        /// Retorna las unidades de medida
        /// </summary>
        /// <returns></returns>
        public List<PAUnidadMedidaDC> ObtenerUnidadesMedida()
        {
            return PAAdministrador.Instancia.ObtenerUnidadesMedida();
        }

        #endregion Unidad de MEdida

        #region Tipo Sector Cliente

        /// <summary>
        /// Obtiene todos los tipos de sector de cliente
        /// </summary>
        /// <returns></returns>
        public IList<PATipoSectorCliente> ObtenerTodosTipoSectorCliente()
        {
            return PAAdministrador.Instancia.ObtenerTodosTipoSectorCliente();
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
        public GenericoConsultasFramework<PATipoSectorCliente> ObtenerTipoSectorCliente(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente)
        {
            int totalRegistros = 0;
            return new GenericoConsultasFramework<PATipoSectorCliente>()
            {
                Lista = PAAdministrador.Instancia.ObtenerTipoSectorCliente(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// inserta Modifica o elimina un  tipo de sector de cliente
        /// </summary>
        /// <param name="tipoSectorCliente"></param>
        public void ActualizarTipoSectorCliente(PATipoSectorCliente tipoSectorCliente)
        {
            PAAdministrador.Instancia.ActualizarTipoSectorCliente(tipoSectorCliente);
        }

        #endregion Tipo Sector Cliente

        /// <summary>
        ///Obtiene los Operadores Postales de la Zona
        /// </summary>
        /// <returns>lista de operadores Postales de zona con tiempos de entrega</returns>
        public List<PAOperadorPostalZonaDC> ObtenerOperadorPostalZona(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return PAAdministrador.Instancia.ObtenerOperadorPostalZona(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Gestiona el Crud de operador postal por Zona
        /// </summary>
        /// <param name="operadorPostalZona"></param>
        public void GestionOperadorPostalZona(PAOperadorPostalZonaDC operadorPostalZona)
        {
            PAAdministrador.Instancia.GestionOperadorPostalZona(operadorPostalZona);
        }

        /// <summary>
        /// Obtiene la Zona y el tipo de Zona
        /// correspondiente
        /// </summary>
        /// <returns>lista de zonas y su tipo</returns>
        public List<PAZonaDC> ObtenerListadoZonas()
        {
            return PAAdministrador.Instancia.ObtenerListadoZonas();
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
        public GenericoConsultasFramework<PAOperadorPostal> ObtenerOperadorPostalPaginado(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente)
        {
            int totalRegistros = 0;
            return new GenericoConsultasFramework<PAOperadorPostal>()
            {
                Lista = PAAdministrador.Instancia.ObtenerOperadorPostal(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Actualiza un Operador postal
        /// </summary>
        /// <param name="operador"></param>
        public void ActualizarOperadorPostal(PAOperadorPostal operador)
        {
            PAAdministrador.Instancia.ActualizarOperadorPostal(operador);
        }

        /// <summary>
        /// Consulta la url en la cual se encuentra ubicada la app de carga masiva de guias o  facturas
        /// </summary>
        /// <returns></returns>
        public string ConsultarURLAppCargaMasiva()
        {
            return PAParametros.Instancia.ConsultarURLAppCargaMasiva();
        }

        /// <summary>
        /// Obtiene los numeros de guia que no tienen imagen en el servidor
        /// </summary>
        /// <param name="idCol"></param>
        /// <returns></returns>
        public List<string> ConsultarArchivosPendientesSincronizar(long idCol)
        {
            return PAAdministrador.Instancia.ConsultarArchivosPendientesSincronizar(idCol);
        }

        /// <summary>
        /// Retorna la imagen de publicidad asociada al login de la aplicación, si no retorna nada, no requiere visualizar publicidad
        /// </summary>
        /// <returns></returns>
        public ParamPublicidad ConsultarImagenPublicidadLogin()
        {
            return PAAdministrador.Instancia.ConsultarImagenPublicidadLogin();
        }

        /// <summary>
        /// Consulta las variaciones y abreviaciones del campo Direccion
        /// </summary>
        /// <returns></returns>
        public List<PAEstandarDireccionDC> ConsultarAbreviacionesVariacionesDireccion()
        {
            return PAParametros.Instancia.ConsultarAbreviacionesVariacionesDireccion();
        }

        #region menus
        /// <summary>
        /// Otiene los Menus
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consulta</param>
        /// <returns>Lista de tipos de Bancos</returns>
        public GenericoConsultasFramework<VEMenuCapacitacion> ObtenerMenus(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente)
        {
            int totalRegistros = 0;

            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<VEMenuCapacitacion>()
            {
                Lista = PAAdministrador.Instancia.ObtenerMenus(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros),
                TotalRegistros = totalRegistros
            };
        }


        /// <summary>
        /// Adiciona modifica o elimina un menu hijo
        /// </summary>
        /// <param name="menu">Objeto banco</param>
        public void ActualizarMenu(VEMenuCapacitacion menu)
        {
            PAAdministrador.Instancia.ActualizarMenu(menu);
        }

        /// <summary>
        /// Adiciona modifica o elimina un menu padre
        /// </summary>
        /// <param name="menuPadre">Objeto banco</param>
        public void ActualizarMenuPadre(List<VEMenuCapacitacion> menuPadre)
        {
            foreach (VEMenuCapacitacion menu in menuPadre)
            {
                PAAdministrador.Instancia.ActualizarMenu(menu);
            }
        }
        #endregion

        #region Imagen Publicidad Guia
        public void GuardarImagenPublicidad(string rutaImagen, string imagenPublicidad)
        {
            PAAdministrador.Instancia.GuardarImagenPublicidad(rutaImagen, imagenPublicidad);
        }
        #endregion

        #region Notificaciones Moviles

        /// <summary>
        /// Registra un nuevo dispositivo movil dentro de la plataforma
        /// </summary>
        /// <param name="dispositivo"></param>
        public long RegistrarDispositivoMovil(PADispositivoMovil dispositivo)
        {
            return PAParametros.Instancia.RegistrarDispositivoMovil(dispositivo);
        }


        /// <summary>
        /// Inactivar un dispositivo movil registrado
        /// </summary>
        /// <param name="dispositivo"></param>
        public void InactivarDispositivoMovil(PADispositivoMovil dispositivo)
        {
            PAParametros.Instancia.InactivarDispositivoMovil(dispositivo);
        }


        /// <summary>
        /// Obtiene todos los dispositivos de registrados de los peatones o los empleados
        /// </summary>
        /// <param name="tipoDispositivo">Indica si se filtra por los dispositivos de los empleados o los peatones</param>
        /// <returns></returns>
        public List<PADispositivoMovil> ObtenerTodosDispositivosPeatonEmpleados(PAEnumTiposDispositivos tipoDispositivo)
        {
            return PAParametros.Instancia.ObtenerTodosDispositivosPeatonEmpleados(tipoDispositivo);
        }

        /// <summary>
        /// Obtiene los dispositivos moviles de los empleados en una ciudad
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <returns></returns>
        public List<PADispositivoMovil> ObtenerDispositivosMovilesEmpleadosCiudad(string idLocalidad, bool esControllerApp = false)
        {
            return PAParametros.Instancia.ObtenerDispositivosMovilesEmpleadosCiudad(idLocalidad, esControllerApp);
        }

        /// <summary>
        ///  Obtiene los dispositivos el dispositivo movil asociado a la identificacion de un empleado
        /// </summary>
        /// <param name="numeroIdentificacion"></param>
        /// <returns></returns>
        public List<PADispositivoMovil> ObtenerDispositivosMovilesIdentificacionEmpleado(long numeroIdentificacion)
        {
            return PAParametros.Instancia.ObtenerDispositivosMovilesIdentificacionEmpleado(numeroIdentificacion);
        }

        /// <summary>
        /// Obtiene los dispositivos el dispositivo movil asociado a una empleado
        /// </summary>
        /// <param name="nombUsuario"></param>
        /// <returns></returns>
        public PADispositivoMovil ObtenerDispositivoMovilEmpleado(string nombUsuario)
        {
            return PAParametros.Instancia.ObtenerDispositivoMovilEmpleado(nombUsuario);
        }

        /// <summary>
        /// Obtiene un dispositivo movil a partir del token y del sistema operativo
        /// </summary>
        /// <param name="tokenMovil"></param>
        /// <returns></returns>
        public PADispositivoMovil ObtenerDispositivoMovilTokenOs(string tokenMovil, PAEnumOsDispositivo sistemaOperativo)
        {
            return PAParametros.Instancia.ObtenerDispositivoMovilTokenOs(tokenMovil, sistemaOperativo);
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
            return PAAdministrador.Instancia.ObtenerFestivos(fechadesde, fechahasta, idPais);
        }


        /// <summary>
        /// Obtiene la informacion de la entidad segun el rol
        /// </summary>
        /// <param name="codigo"></param>
        /// <param name="rol"></param>
        /// <returns></returns>
        public PAPropiedadesAplicacionDC ConsultarPropiedadesAplicacion(string codigo, int rol)
        {
            return PAAdministrador.Instancia.ConsultarPropiedadesAplicacion(codigo, rol);

        }
        #endregion

        /// <summary>
        /// Verifica si la base de  datos está disponible
        /// </summary>
        /// <returns></returns>
        public bool VerificarConexionBD()
        {
            return PAParametros.Instancia.VerificarConexionBD();
        }

        public byte[] ObtenerArchivoDesdePath(string url)
        {
            return PAParametros.Instancia.ObtenerArchivoDesdePath(url);
        }

    }
}