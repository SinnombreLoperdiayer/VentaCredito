using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Web;
using System.Web.Configuration;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.Util;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW.Datos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using Framework.Servidor.Servicios.ContratoDatos.Parametros.Consecutivos;
using Framework.Servidor.Servicios.ContratoDatos.Seguridad;
using Framework.Servidor.Servicios.ContratoDatos.Versionamiento;
using System.Threading.Tasks;
using System.IO;

namespace Framework.Servidor.ParametrosFW
{
    public class PAParametros : ControllerBase
    {
        public static readonly PAParametros Instancia = (PAParametros)FabricaInterceptores.GetProxy(new PAParametros(), ConstantesFramework.PARAMETROS_FRAMEWORK);


        private static bool SistemaEnLinea =false;

        private static bool VerificadorLineaEjecutando = false;

        private Lazy<List<DateTime>> lstDiasNoHabiles = null;

        public List<DateTime> LstDiasNoHabiles
        {
            get
            {
                return lstDiasNoHabiles.Value;
            }

        }

        private PAParametros()
        {
            lstDiasNoHabiles = new Lazy<List<DateTime>>(() => PARepositorio.Instancia.ObtenerDiasNoHabiles());
        }

        /// <summary>
        /// Obtiene la lista de programas Congeladores de Disco duro
        /// </summary>
        /// <returns></returns>
        public List<PAFreezersDiscoDC> ObtenerListaFreezersDisco()
        {
            return PARepositorio.Instancia.ObtenerListaFreezersDisco();
        }

        /// <summary>
        /// Retorna el id del operador postal de la localidad dada
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <returns></returns>
        public PAOperadorPostal ObtenerOperadorPostalLocalidad(string idLocalidad)
        {
            return PARepositorio.Instancia.ObtenerOperadorPostalLocalidad(idLocalidad);
        }

        /// <summary>
        /// Retorna el porcentaje del recargo de combustible para el operador postal
        /// </summary>
        /// <param name="idZona"></param>
        /// <returns></returns>
        public decimal ObtenerPorcentajeRecargoCombustibleOPxZona(string idZona)
        {
            return PARepositorio.Instancia.ObtenerPorcentajeRecargoCombustibleOPxZona(idZona);
        }

        /// <summary>
        /// Retorna el valor del dólar parametrizado en la tabla ParámetrosFramework
        /// </summary>
        /// <returns></returns>
        public double? ObtenerValorDolarEnPesos()
        {
            return PARepositorio.Instancia.ObtenerValorDolarEnPesos();
        }

        /// <summary>
        /// Retorna el valor del parametro para el movimiento de la caja en Controller
        /// </summary>
        /// <returns></returns>
        public bool ObtenerValorMovCajaController()
        {
            return PARepositorio.Instancia.ObtenerValorMovCajaController();
        }

        /// <summary>
        /// Retorna el valor del parametro para el movimiento de la caja en API
        /// </summary>
        /// <returns></returns>
        public bool ObtenerValorMovCajaApi()
        {
            return PARepositorio.Instancia.ObtenerValorMovCajaApi();
        }

        /// <summary>
        /// Retorna la lista de condiciones dado el operador postal
        /// </summary>
        /// <param name="idOperadorPostal"></param>
        /// <returns></returns>
        public IEnumerable<PACondicionOperadorPostalDC> ObtenerCondicionesPorOperadorPostal(int idOperadorPostal)
        {
            return PARepositorio.Instancia.ObtenerCondicionesPorOperadorPostal(idOperadorPostal);
        }

        /// <summary>
        /// Obtiene todos los medios de transporte
        /// </summary>
        /// <returns>Lista con los medios de transporte</returns>
        public IList<PAMedioTransporte> ObtenerTodosMediosTrasporte()
        {
            return PARepositorio.Instancia.ObtenerTodosMediosTrasporte();
        }

        /// <summary>
        /// Consulta los parametros generales del Framework
        /// </summary>
        /// <param name="llave">Nombre del parametro</param>
        /// <returns>Valor el parametro</returns>
        public string ConsultarParametrosFramework(string llave)
        {
            return PARepositorio.Instancia.ConsultarParametrosFramework(llave);
        }

        /// <summary>
        /// Consulta los parametros generales del Framework
        /// </summary>
        /// <param name="llave">Nombre del parametro</param>
        /// <returns>Valor el parametro</returns>
        public Dictionary<string, string> ConsultarListaParametrosFramework()
        {
            return PARepositorio.Instancia.ConsultarListaParametrosFramework();
        }


        /// <summary>
        /// Consulta los tipos de identificacion
        /// </summary>
        /// <returns>Lista de tipos de identificacion</returns>
        public IList<PATipoIdentificacion> ConsultarTiposIdentificacion()
        {
            return PARepositorio.Instancia.ConsultarTiposIdentificacion();
        }

        /// <summary>
        /// Obtiene los festivos entre dos fechas pero no los agrega a la cache, con el fin de consultar fechas por meses
        /// </summary>
        /// <returns></returns>
        public List<DateTime> ObtenerFestivosSinCache(DateTime fechaDesde, DateTime fechaHasta, string pais)
        {
            return PARepositorio.Instancia.ObtenerFestivosSinCache(fechaDesde, fechaHasta, pais);
        }

        /// <summary>
        /// Consultar las ocupaciones
        /// </summary>
        /// <returns>Lista de ocupaciones</returns>
        public IList<PAOcupacionDC> ConsultarOcupacion()
        {
            return PARepositorio.Instancia.ConsultarOcupacion();
        }

        /// <summary>
        /// Obtiene una lista con los regimenes contributivos
        /// </summary>
        /// <returns>objeto lista de regimenes contributivos</returns>
        public IList<PATipoRegimenDC> ObtenerRegimenContributivo()
        {
            return PARepositorio.Instancia.ObtenerRegimenContributivo();
        }

        /// <summary>
        /// Obtiene una lista con los segmentos de mercado
        /// </summary>
        /// <returns>objeto lista de los segmentos de mercado</returns>
        public IList<PATipoSegmentoDC> ObtenerSegmentoMercado()
        {
            return PARepositorio.Instancia.ObtenerSegmentoMercado();
        }

        /// <summary>
        /// Obtiene una lista con los tipos de sociedad
        /// </summary>
        /// <returns>objeto lista de los tipos de sociedad</returns>
        public IList<PATipoSociedadDC> ObtenerTipoSociedad()
        {
            return PARepositorio.Instancia.ObtenerTipoSociedad();
        }

        /// <summary>
        /// Obtiene los grupos para realizar la divulgacion
        /// </summary>
        /// <returns></returns>
        public PADivulgacion ObtenerGruposDivulgacion(int idAlerta)
        {
            return PARepositorio.Instancia.ObtenerGruposDivulgacion(idAlerta);
        }

        #region Representante Legal

        /// <summary>
        /// Obtiene lista de responsable legal
        /// </summary>
        /// <returns>Lista con los responsables legales</returns>
        public IList<PAResponsableLegal> ObtenerResponsableLegal(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return PARepositorio.Instancia.ObtenerResponsableLegal(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Actualiza un responsable legal
        /// </summary>
        /// <param name="responsable">Objeto responsable legal</param>
        /// <returns>Id del responsable legal</returns>
        public long ActualizarResponsableLegal(PAResponsableLegal responsable)
        {
            if (PARepositorio.Instancia.ValidarListaRestrictiva(responsable.PersonaExterna.Identificacion))
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, ETipoErrorFramework.EX_ALERTA_LISTAS_RESTRICTIVAS.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_ALERTA_LISTAS_RESTRICTIVAS)));

            switch (responsable.EstadoRegistro)
            {
                case EnumEstadoRegistro.ADICIONADO:
                    return PARepositorio.Instancia.AdicionarResponsableLegal(responsable);

                case EnumEstadoRegistro.BORRADO:
                    return 0;

                case EnumEstadoRegistro.MODIFICADO:
                    PARepositorio.Instancia.EditarResponsableLegal(responsable);
                    return 0;

                default:
                    return 0;
            }
        }

        /// <summary>
        /// Consulta un responsable legal dependiendo de su id
        /// </summary>
        /// <param name="idResponsable">Id del responsable legal</param>
        /// <returns>Responsable legal</returns>
        public PAResponsableLegal ObtenerResponsableLegal(long idResponsable)
        {
            return PARepositorio.Instancia.ObtenerResponsableLegal(idResponsable);
        }

        #endregion Representante Legal

        /// <summary>
        /// Valida si una identificacion se encuentra en listas restrictivas
        /// </summary>
        /// <param name="identificacion"></param>
        /// <returns></returns>
        public bool ValidarListaRestrictiva(string identificacion)
        {
            return PARepositorio.Instancia.ValidarListaRestrictiva(identificacion);
        }

        /// <summary>
        /// Enviar correo electronico cuando un cliente se encuentre en listas restrictivas
        /// </summary>
        /// <param name="identificacion"></param>
        /// <param name="idCentroServicios"></param>
        /// <param name="nombreCentroServicios"></param>
        public void EnviarCorreoListasRestrictivas(string identificacion, long idCentroServicios, string nombreCentroServicios)
        {
            InformacionAlerta informacionAlerta = PARepositorio.Instancia.ConsultarInformacionAlerta(PAConstantesParametros.ALERTA_LISTA_RESTRICTIVA);
            string mensaje = String.Format(informacionAlerta.Mensaje, identificacion, idCentroServicios, nombreCentroServicios);
            CorreoElectronico.Instancia.Enviar(informacionAlerta.Destinatario, informacionAlerta.Asunto, mensaje);
        }

        /// <summary>
        /// Envia correo al remitente de un giro
        /// </summary>
        /// <param name="destinoGiro"></param>
        /// <param name="ciudad"></param>
        /// <param name="direccion"></param>
        /// <param name="destinatario"></param>
        public void EnviarCorreoTelemercadeoGiroRemitente(string destinoGiro, string ciudad, string direccion, string destinatario)
        {
            InformacionAlerta informacionAlerta = PARepositorio.Instancia.ConsultarInformacionAlerta(PAConstantesParametros.ALERTA_TELEMERCADEO_GIRO_REMITENTE);
            string mensaje = String.Format(informacionAlerta.Mensaje, destinoGiro, ciudad, direccion);
            CorreoElectronico.Instancia.Enviar(destinatario, informacionAlerta.Asunto, mensaje);
        }

        /// <summary>
        /// Envia un correo electronico a un destinatario de un giro
        /// </summary>
        /// <param name="remitenteGiro"></param>
        /// <param name="ciudad"></param>
        /// <param name="direccion"></param>
        /// <param name="fechaDisponibilidad"></param>
        /// <param name="destinatario"></param>
        public void EnviarCorreoTelemercadeoGiroDestinatario(string remitenteGiro, string ciudad, string direccion, DateTime fechaDisponibilidad, string destinatario)
        {
            InformacionAlerta informacionAlerta = PARepositorio.Instancia.ConsultarInformacionAlerta(PAConstantesParametros.ALERTA_TELEMERCADEO_GIRO_DESTINATARIO);
            string mensaje = String.Format(informacionAlerta.Mensaje, remitenteGiro, ciudad, direccion, fechaDisponibilidad);
            CorreoElectronico.Instancia.Enviar(destinatario, informacionAlerta.Asunto, mensaje);
        }

        /// <summary>
        /// Enviar correo electronico cuando una agencia supero el tope maximo de operacion
        /// </summary>
        /// <param name="identificacion"></param>
        /// <param name="idCentroServicios"></param>
        /// <param name="nombreCentroServicios"></param>
        public void EnviarCorreoAgenciaSuperaMaxOpe(long idCentroServicio, string nombreCentroServicios)
        {
            InformacionAlerta informacionAlerta = PARepositorio.Instancia.ConsultarInformacionAlerta(PAConstantesParametros.ALERTA_AGENCIA_TOPES_MAX);
            string mensaje = String.Format(informacionAlerta.Mensaje, idCentroServicio, nombreCentroServicios);
            string asunto = String.Format(informacionAlerta.Asunto, idCentroServicio);
            CorreoElectronico.Instancia.Enviar(informacionAlerta.Destinatario, asunto, mensaje);
        }

        public void EnviarCorreoUnicoDestinatarioDosVariables(int tipoMensaje, string destinatario, string valor01, string valor02)
        {
            InformacionAlerta informacionAlerta = PARepositorio.Instancia.ConsultarInformacionEnvioCorreo(tipoMensaje);
            string contenidoMensaje = String.Format(informacionAlerta.Mensaje, valor01);
            string asuntoMail = String.Format(informacionAlerta.Asunto, valor02);
            CorreoElectronico.Instancia.Enviar(destinatario, asuntoMail, contenidoMensaje);
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
            CorreoElectronico.Instancia.Enviar(destinatario, asunto, mensaje);
        }

        /// <summary>
        /// Metodo Para obtener un Consecutivo
        /// enviando el tipo de consecutivo
        /// </summary>
        /// <param name="tipoConsecutivo"></param>
        /// <returns></returns>
        public long ObtenerConsecutivo(PAEnumConsecutivos idConsecutivo)
        {
            return PARepositorio.Instancia.ObtenerConsecutivo(idConsecutivo);
        }

        /// <summary>
        /// Metodo Para obtener un Consecutivo Por Col
        /// enviando el tipo de consecutivo
        /// </summary>
        /// <param name="tipoConsecutivo"></param>
        /// <returns></returns>
        public long ObtenerConsecutivoPorCol(PAEnumConsecutivos idConsecutivo, long idCol)
        {
            return PARepositorio.Instancia.ObtenerConsecutivoPorCol(idConsecutivo, idCol);
        }

        /// <summary>
        /// Obtiene la caja actual
        /// </summary>
        /// <param name="idTipoConsecutivo">Tipo de consecutivo</param>
        /// <returns>Objeto Consecutivo</returns>
        public PAConsecutivoDC ObtenerDatosConsecutivoxCol(PAEnumConsecutivos idConsecutivo, long idCentroLogistico)
        {
            return PARepositorio.Instancia.ObtenerDatosConsecutivoxCol(idConsecutivo, idCentroLogistico);
        }

        /// <summary>
        /// Obtiene la caja actual
        /// </summary>
        /// <param name="idTipoConsecutivo">Tipo de consecutivo</param>
        /// <returns>Objeto Consecutivo</returns>
        public PAConsecutivoDC ObtenerDatosConsecutivoIntentoEntregaxCol(PAEnumConsecutivos idConsecutivo, long idCentroLogistico)
        {
            return PARepositorio.Instancia.ObtenerDatosConsecutivoIntentoEntregaxCol(idConsecutivo, idCentroLogistico);
        }

        /// <summary>
        ///Metodo para obtener una plantilla del framework
        /// </summary>
        /// <param name="archivo"></param>
        /// <returns>Archivo</returns>
        public byte[] ObtenerPlantillaFramework(long idPlantilla)
        {
            return PARepositorio.Instancia.ObtenerPlantillaFramework(idPlantilla);
        }

        /// <summary>
        /// Obtiene los estados de la aplicación
        /// </summary>
        /// <returns>Colección con los estados</returns>
        public IEnumerable<PAEstadoActivoInactivoDC> ObtenerEstadosAplicacion()
        {
            return PARepositorio.Instancia.ObtenerEstadosAplicacion();
        }

        /// <summary>
        /// Obtiene la informacion de una persona externa
        /// </summary>
        /// <param name="idPersonaExterna"></param>
        /// <returns></returns>
        public PAPersonaExterna ObtenerPersonaExterna(long idPersonaExterna)
        {
            return PARepositorio.Instancia.ObtenerPersonaExterna(idPersonaExterna);
        }

        /// <summary>
        /// Obtiene la informacion de una persona interna
        /// </summary>
        /// <param name="idPersonaInterna"></param>
        /// <returns></returns>
        public PAPersonaInternaDC ObtenerPersonaInterna(long idPersonaInterna)
        {
            return PARepositorio.Instancia.ObtenerPersonaInterna(idPersonaInterna);
        }

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
        public IList<PATipoActEconomica> ObtenerTiposActividadEconomica(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return PARepositorio.Instancia.ObtenerTiposActividadEconomica(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Adiciona, modifica o elimina un tipo de actividad Economica
        /// </summary>
        /// <param name="docuCentroServ">Objeto tipo de actividad economica</param>
        public void ActualizarTipoActividadEconomica(PATipoActEconomica tipoActEconomica)
        {
            switch (tipoActEconomica.EstadoRegistro)
            {
                case EnumEstadoRegistro.ADICIONADO:
                    PARepositorio.Instancia.AdicionarTipoActividadEconomica(tipoActEconomica);
                    break;

                case EnumEstadoRegistro.BORRADO:
                    PARepositorio.Instancia.EliminarTipoActividadEconomica(tipoActEconomica);
                    break;

                case EnumEstadoRegistro.MODIFICADO:
                    PARepositorio.Instancia.EditarTipoActividadEconomica(tipoActEconomica);
                    break;
            }
        }

        /// <summary>
        /// Obtiene todos los tipos de actividad economica
        /// </summary>
        /// <returns>Lista con los tipos de actividad economica</returns>
        public IList<PATipoActEconomica> ObtenerTiposActividadEconomica()
        {
            return PARepositorio.Instancia.ObtenerTiposActividadEconomica();
        }

        /// <summary>
        /// Obtiene el valor del AppSetting de acuerdo al key suministrado
        /// </summary>
        /// <param name="key"></param>
        /// <returns>String con el parametro conrrespondiente al key del AppSettings</returns>
        public string ObtenerParametrosAplicacion(string key)
        {
            return WebConfigurationManager.AppSettings[key];
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
        public IList<PABanco> ObtenerBancos(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return PARepositorio.Instancia.ObtenerBancos(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Adiciona modifica o elimina un banco
        /// </summary>
        /// <param name="banco">Objeto banco</param>
        public void ActualizarBanco(PABanco banco)
        {
            switch (banco.EstadoRegistro)
            {
                case EnumEstadoRegistro.ADICIONADO:
                    PARepositorio.Instancia.AdicionarBanco(banco);
                    break;

                case EnumEstadoRegistro.BORRADO:
                    PARepositorio.Instancia.EliminarBanco(banco);
                    break;

                case EnumEstadoRegistro.MODIFICADO:
                    PARepositorio.Instancia.EditarBanco(banco);
                    break;
            }
        }

        /// <summary>
        /// Obtiene todos los bancos
        /// </summary>
        /// <returns>Lista con los bancos</returns>
        public IList<PABanco> ObtenerTodosBancos()
        {
            return PARepositorio.Instancia.ObtenerTodosBancos();
        }

        /// <summary>
        /// Obtiene todos los tipos de cuenta
        /// </summary>
        /// <returns>Lista con los tipos de cuenta</returns>
        public IList<PATipoCuenta> ObtenerTiposCuentaBanco()
        {
            return PARepositorio.Instancia.ObtenerTiposCuentaBanco();
        }

        /// <summary>
        /// Obtiene los Tipos de Documentos de Banco
        /// </summary>
        /// <returns>lista de los Tipos de Doc Banco</returns>
        public IList<PATipoDocumBancoDC> ObtenerTiposDocumentosBanco()
        {
            return PARepositorio.Instancia.ObtenerTiposDocumentosBanco();
        }

        #endregion Bancos

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
            return PARepositorio.Instancia.ObtenerResponsableServicio(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Adiciona modifica o elimina un responsable de servicio
        /// </summary>
        /// <param name="banco">Objeto responsable de servicio</param>
        public long ActualizarResponsableServicio(PAResponsableServicio responsable)
        {
            switch (responsable.EstadoRegistro)
            {
                case EnumEstadoRegistro.ADICIONADO:
                    return PARepositorio.Instancia.AdicionarResponsableServicio(responsable);

                case EnumEstadoRegistro.BORRADO:
                    PARepositorio.Instancia.EliminarResponsableServicio(responsable);
                    return 0;

                case EnumEstadoRegistro.MODIFICADO:
                    PARepositorio.Instancia.EditarResponsableServicio(responsable);
                    return 0;
                default:
                    return 0;
            }
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
            return PARepositorio.Instancia.ObtenerPersonasFiltro(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        #endregion persona interna

        #region Dias

        /// <summary>
        /// Obtiene todos los dias
        /// </summary>
        /// <returns>Lista con los dias</returns>
        public IList<PADia> ObtenerTodosDias()
        {
            return PARepositorio.Instancia.ObtenerTodosDias();
        }

        #endregion Dias

        #region Semanas

        /// <summary>
        /// Obtiene todas las semanas
        /// </summary>
        /// <returns>Lista con las semanas</returns>
        public IList<PASemanaDC> ObtenerTodasSemanas()
        {
            return PARepositorio.Instancia.ObtenerTodasSemanas();
        }

        #endregion Semanas

        #region Meses

        /// <summary>
        /// Obtiene todos los meses
        /// </summary>
        /// <returns>Lista con los meses</returns>
        public IList<PAMesDC> ObtenerTodosMeses()
        {
            return PARepositorio.Instancia.ObtenerTodosMeses();
        }

        #endregion Meses

        #region Zonas

        /// <summary>
        /// Obtener las zonas de localidades
        /// </summary>
        /// <returns>Colección con las zonas de localidades</returns>
        public IEnumerable<PAZonaDC> ObtenerZonasDeLocalidad()
        {
            return PARepositorio.Instancia.ObtenerZonasDeLocalidad();
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
            return PARepositorio.Instancia.ObtenerDiasHabiles(fechadesde, fechahasta, idPais);
        }

        /// <summary>
        /// Obtiene el listado de días festivos que hay entre una fecha y otra
        /// </summary>
        /// <param name="fechadesde">Fecha desde la cual se hará la consulta</param>
        /// <param name="fechahasta">Fecha hasta la cua se hará la consulta</param>
        /// <returns></returns>
        public List<DateTime> ObtenerFestivos(DateTime fechadesde, DateTime fechahasta, string idPais)
        {
            return PARepositorio.Instancia.ObtenerFestivos(fechadesde, fechahasta, idPais);
        }

        /// <summary>
        /// Obtiene la fecha hábil más próxima desde una fecha inicial sumando un número de días específicos teniendo en cuenta los sábados como días hábiles
        /// </summary>
        /// <param name="fechadesde">Fecha desde la cual se hará la consulta</param>
        /// <param name="numerodias">Número de días que se quiere sumar a la fecha desde</param>
        public DateTime ObtenerFechaFinalHabil(DateTime fechadesde, double numerodias, string idPais)
        {
            int numeroDiasNoHabiles = 0;
            DateTime fechaHasta = fechadesde.AddDays(numerodias);
            numeroDiasNoHabiles = LstDiasNoHabiles.Where(d => d.Date > fechadesde && d.Date <= fechaHasta).Count();
            if (numerodias == 0 && fechadesde.Date == fechaHasta.Date)
            {
                numeroDiasNoHabiles = LstDiasNoHabiles.Where(d => d.Date >= fechadesde.Date && d.Date <= fechaHasta.Date).Count();
            }
            if (numeroDiasNoHabiles == 0)
                return fechaHasta;
            else
                return ObtenerFechaFinalHabil(fechaHasta, (numeroDiasNoHabiles), idPais);
        }

        /// <summary>
        /// Obtiene la fecha hábil más próxima desde una fecha inicial sumando un número de días específicos teniendo en cuenta los sábados como días hábiles
        /// </summary>
        /// <param name="fechadesde">Fecha desde la cual se hará la consulta</param>
        /// <param name="numerodias">Número de días que se quiere sumar a la fecha desde</param>
        public DateTime ObtenerFechaFinalHabilSinSabados(DateTime fechadesde, double numerodias, string idPais, List<DateTime> fechasH = null)
        {
            int numeroDiasNoHabiles = 0;
            DateTime fechaHasta = fechadesde.AddDays(numerodias);           

            for (int i = 0; i <= numerodias; i++)
            {
                if (fechadesde.AddDays(i).DayOfWeek == DayOfWeek.Saturday)
                {
                    fechaHasta = fechaHasta.AddDays(1);
                }
            }

            numeroDiasNoHabiles = LstDiasNoHabiles.Where(d => d.Date > fechadesde.Date && d.Date <= fechaHasta.Date).Count();

            if (numerodias == 0 && fechadesde.Date == fechaHasta.Date)
            {
                numeroDiasNoHabiles = LstDiasNoHabiles.Where(d => d.Date >= fechadesde.Date && d.Date <= fechaHasta.Date).Count();
            }

            if (numeroDiasNoHabiles == 0)
            {
                return fechaHasta;
            }
            else
            {
                return ObtenerFechaFinalHabilSinSabados(fechaHasta, (numeroDiasNoHabiles), idPais, fechasH);
            }
        }

        /// <summary>
        /// Obtiene el consecutivo para el archivo de intento de entrega
        /// </summary>
        /// <param name="idConsecutivo">identificador consecutivo</param>
        /// <param name="idCol">identificador del col</param>
        /// <returns>consecutivo</returns>
        internal long ObtenerConsecutivoIntentoEntregaPorCol(PAEnumConsecutivos idConsecutivo, long idCol)
        {
            return PARepositorio.Instancia.ObtenerConsecutivoIntentoEntregaPorCol(idConsecutivo, idCol);
        }


        /// <summary>
        /// Retorn la cantidad de domingos que hay entre una fecha y otra
        /// </summary>
        /// <param name="fechadesde">Fecha desde la cual se hará la consulta</param>
        /// <param name="fechahasta">Fecha hasta la cua se hará la consulta</param>
        /// <returns></returns>
        public int ObtenerNumeroDeDomingos(DateTime fechadesde, DateTime fechahasta)
        {
            return PARepositorio.Instancia.ObtenerNumeroDeDomingos(fechadesde, fechahasta);
        }

        internal PAConsecutivoDC CrearRangoConsecutivoIntentoEntrega(PAEnumConsecutivos cajas_PruebasEntrega, long idCentroLogistico, string creadoPor)
        {
            return PARepositorio.Instancia.CrearRangoConsecutivoIntentoEntrega(cajas_PruebasEntrega, idCentroLogistico, creadoPor);
        }

        /// <summary>
        /// Método para obtener los agregar los dias laborales a una fecha predeterminada
        /// </summary>
        /// <param name="fechaOriginal"></param>
        /// <param name="diasLaborables"></param>
        /// <returns></returns>
        public DateTime AgregarDiasLaborales(DateTime fechaOriginal, int diasLaborables)
        {
            return PARepositorio.Instancia.AgregarDiasLaborales(fechaOriginal, diasLaborables);
        }

        /// <summary>
        /// Método para obtener los agregar los dias laborales entre dos fechas
        /// </summary>
        /// <param name="fechaOriginal"></param>
        /// <param name="diasLaborables"></param>
        /// <returns></returns>
        public int ConsultarDiasLaborales(DateTime fechaInicial, DateTime fechaFinal)
        {
            return PARepositorio.Instancia.ConsultarDiasLaborales(fechaInicial, fechaFinal);
        }

        #endregion Calendario Festivos x Pais

        #region Consecutivo

        /// <summary>
        /// Obtiene la caja actual
        /// </summary>
        /// <param name="idTipoConsecutivo">Tipo de consecutivo</param>
        /// <returns>Objeto Consecutivo</returns>
        public PAConsecutivoDC ObtenerDatosConsecutivo(PAEnumConsecutivos idConsecutivo)
        {
            return PARepositorio.Instancia.ObtenerDatosConsecutivo(idConsecutivo);
        }

        #endregion Consecutivo

        #region Parientes

        /// <summary>
        /// Método para obtener los parentezcos configurados
        /// </summary>
        /// <returns></returns>
        public IList<PAParienteDC> ObtenerParientes()
        {
            return PARepositorio.Instancia.ObtenerParientes();
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
            return PARepositorio.Instancia.ObtenerEstadosEmpaque(peso);
        }

        /// <summary>
        /// Obtiene los estados del empaque
        /// </summary>
        /// <returns></returns>
        public List<PAEstadoEmpaqueDC> ObtenerTodosEstadosEmpaque()
        {
            return PARepositorio.Instancia.ObtenerTodosEstadosEmpaque();
        }

        #endregion Estados Empaque

        #region Unidad de medida

        /// <summary>
        /// Retorna las unidades de medida
        /// </summary>
        /// <returns></returns>
        public List<PAUnidadMedidaDC> ObtenerUnidadesMedida()
        {
            return PARepositorio.Instancia.ObtenerUnidadesMedida();
        }

        #endregion Unidad de medida

        #region Tipo Sector Cliente

        /// <summary>
        /// Obtiene todos los tipos de sector de cliente
        /// </summary>
        /// <returns></returns>
        public IList<PATipoSectorCliente> ObtenerTodosTipoSectorCliente()
        {
            return PARepositorio.Instancia.ObtenerTodosTipoSectorCliente();
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
            return PARepositorio.Instancia.ObtenerTipoSectorCliente(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// inserta Modifica o elimina un  tipo de sector de cliente
        /// </summary>
        /// <param name="tipoSectorCliente"></param>
        public void ActualizarTipoSectorCliente(PATipoSectorCliente tipoSectorCliente)
        {
            switch (tipoSectorCliente.EstadoRegistro)
            {
                case EnumEstadoRegistro.ADICIONADO:
                    PARepositorio.Instancia.AdicionarTipoSectorCliente(tipoSectorCliente);
                    break;

                case EnumEstadoRegistro.BORRADO:
                    PARepositorio.Instancia.EliminarTipoSectorCliente(tipoSectorCliente);
                    break;

                case EnumEstadoRegistro.MODIFICADO:
                    PARepositorio.Instancia.EditarTipoSectorCliente(tipoSectorCliente);
                    break;
            }
        }

        #endregion Tipo Sector Cliente

        #region Operador Postal

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
            return PARepositorio.Instancia.ObtenerCondicionOperadorPostal(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Adiciona, Edita o Elimina una condicion del Operador Postal Rafael Ramirez 28-12-2011
        /// </summary>
        public void ActualizarCondOperadorPostal(PACondicionOperadorPostalDC conOperadorPostal)
        {
            if (conOperadorPostal.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
            {
                PARepositorio.Instancia.InsertaCondicionOperadorPostal(conOperadorPostal);
            }

            if (conOperadorPostal.EstadoRegistro == EnumEstadoRegistro.MODIFICADO)
            {
                PARepositorio.Instancia.ModificarCondicionOperadorPostal(conOperadorPostal);
            }

            if (conOperadorPostal.EstadoRegistro == EnumEstadoRegistro.BORRADO)
            {
                PARepositorio.Instancia.EliminarConOperadorPostal(conOperadorPostal);
            }
        }

        /// <summary>
        /// Obtiene la lista de los Operadores Postales Rafael Ramirez 29-12-2011
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PAOperadorPostal> ObtenerOperadorPostal()
        {
            return PARepositorio.Instancia.ObtenerOperadorPostal();
        }

        /// <summary>
        ///Obtiene los Operadores Postales de la Zona
        /// </summary>
        /// <returns>lista de operadores Postales de zona con tiempos de entrega</returns>
        public List<PAOperadorPostalZonaDC> ObtenerOperadorPostalZona(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return PARepositorio.Instancia.ObtenerOperadorPostalZona(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Gestiona el Crud de operador postal por Zona
        /// </summary>
        /// <param name="operadorPostalZona"></param>
        public void GestionOperadorPostalZona(PAOperadorPostalZonaDC operadorPostalZona)
        {
            switch (operadorPostalZona.EstadoRegistro)
            {
                case EnumEstadoRegistro.ADICIONADO:
                    PARepositorio.Instancia.InsertarOperadorPostalZona(operadorPostalZona);
                    break;

                case EnumEstadoRegistro.MODIFICADO:
                    PARepositorio.Instancia.ModificarOperadorPostalZona(operadorPostalZona);
                    break;

                case EnumEstadoRegistro.BORRADO:
                    PARepositorio.Instancia.EliminarOperadorPostalZona(operadorPostalZona);
                    break;

                default:

                    break;
            }
        }

        #endregion Operador Postal

        /// <summary>
        /// Consulta la informacion de la alerta para el envio de correo electronico
        /// </summary>
        /// <param name="idAlerta"></param>
        /// <returns></returns>
        public InformacionAlerta ConsultarInformacionAlerta(int idAlerta)
        {
            return PARepositorio.Instancia.ConsultarInformacionAlerta(idAlerta);
        }

        #region Operador Postal

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
            return PARepositorio.Instancia.ObtenerOperadorPostal(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Actualiza un Operador postal
        /// </summary>
        /// <param name="operador"></param>
        public void ActualizarOperadorPostal(PAOperadorPostal operador)
        {
            switch (operador.EstadoRegistro)
            {
                case EnumEstadoRegistro.ADICIONADO:
                    PARepositorio.Instancia.AdicionarOperadorPostal(operador);
                    break;

                case EnumEstadoRegistro.BORRADO:
                    PARepositorio.Instancia.EliminarOperadorPostal(operador);
                    break;

                case EnumEstadoRegistro.MODIFICADO:
                    PARepositorio.Instancia.EditarOperadorPostal(operador);
                    break;
            }
        }

        #endregion Operador Postal

        /// <summary>
        /// Consulta la url en la cual se encuentra ubicada la app de carga masiva de guias o  facturas
        /// </summary>
        /// <returns></returns>
        public string ConsultarURLAppCargaMasiva()
        {
            return PARepositorio.Instancia.ConsultarURLAppCargaMasiva();
        }
        /// <summary>
        /// Obtiene los numeros de guia que no tienen imagen en el servidor
        /// </summary>
        /// <param name="idCol"></param>
        /// <returns></returns>
        public List<string> ConsultarArchivosPendientesSincronizar(long idCol)
        {
            return PARepositorio.Instancia.ConsultarArchivosPendientesSincronizar(idCol);
        }

        /// <summary>
        /// Obtiene los centros de servicios que deberian estar en linea
        /// </summary>        
        /// <returns></returns>
        public List<Framework.Servidor.Servicios.ContratoDatos.Notificador.CentrosServiciosLinea> ConsultarCentrosServiciosDeberianEstarLinea()
        {
            return PARepositorio.Instancia.ConsultarCentrosServiciosDeberianEstarLinea();
        }

        internal ParamPublicidad ConsultarImagenPublicidadLogin()
        {
            return PARepositorio.Instancia.ConsultarImagenPublicidadLogin();
        }

        /// <summary>
        /// Consulta las variaciones y abreviaciones del campo Direccion
        /// </summary>
        /// <returns></returns>
        public List<PAEstandarDireccionDC> ConsultarAbreviacionesVariacionesDireccion()
        {
            return PARepositorio.Instancia.ConsultarAbreviacionesVariacionesDireccion();
        }

        /// <summary>
        /// Consulta informacion de la entidad segun el rol
        /// </summary>
        /// <param name="codigo"></param>
        /// <param name="rol"></param>
        /// <returns></returns>
        public PAPropiedadesAplicacionDC ConsultarPropiedadesAplicacion(string codigo, int rol)
        {
            PAPropiedadesAplicacionDC paPropiedadesAplicacion;
            switch (rol)
            {
                case (int)PAEnumTipoRolAplicacion.MENSAJERO:
                    paPropiedadesAplicacion = PARepositorio.Instancia.ConsultarPropiedadesMensajeroAplicacion(codigo);
                    break;
                case (int)PAEnumTipoRolAplicacion.AUDITOR:
                    paPropiedadesAplicacion = PARepositorio.Instancia.ConsultarPropiedadesAuditorAplicacion(codigo);
                    break;
                case (int)PAEnumTipoRolAplicacion.MENSAJERO_PAM:
                    paPropiedadesAplicacion = PARepositorio.Instancia.ConsultarPropiedadesMensajeroPAMAplicacion(codigo);
                    break;
                case (int)PAEnumTipoRolAplicacion.AGENCIA:
                    paPropiedadesAplicacion = PARepositorio.Instancia.ConsultarPropiedadesAgenciaAplicacion(codigo);
                    break;
                default:
                    paPropiedadesAplicacion = null;
                    break;
            }

            return paPropiedadesAplicacion;
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
        /// <returns>Lista de tipos de Bancos</returns>
        public IList<VEMenuCapacitacion> ObtenerMenus(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return PARepositorio.Instancia.ObtenerMenus(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Adiciona modifica o elimina un Menu
        /// </summary>
        /// <param name="menu">Objeto Menu</param>
        public void ActualizarMenu(VEMenuCapacitacion menu)
        {
            switch (menu.EstadoRegistro)
            {
                case EnumEstadoRegistro.ADICIONADO:
                    PARepositorio.Instancia.AdicionarMenu(menu);
                    break;

                case EnumEstadoRegistro.BORRADO:
                    PARepositorio.Instancia.EliminarMenu(menu);
                    break;

                case EnumEstadoRegistro.MODIFICADO:
                    PARepositorio.Instancia.EditarMenu(menu);
                    break;
            }
        }
        #endregion

        #region Imagen Publicidad guia
        public void GuardarImagenPublicidadGuia(string rutaImagen, string imagenPublicidad)
        {
            PARepositorio.Instancia.GuardarImagenPub(rutaImagen, imagenPublicidad);
        }
        #endregion


        #region Notificaciones Moviles

        /// <summary>
        /// Registra un nuevo dispositivo movil dentro de la plataforma
        /// </summary>
        /// <param name="dispositivo"></param>
        public long RegistrarDispositivoMovil(PADispositivoMovil dispositivo)
        {
            return PARepositorio.Instancia.RegistrarDispositivoMovil(dispositivo);
        }

        /// <summary>
        ///Inactivar un dispositivo movil registrado
        /// </summary>
        /// <param name="dispositivo"></param>
        public void InactivarDispositivoMovil(PADispositivoMovil dispositivo)
        {
            PARepositorio.Instancia.InactivarDispositivoMovil(dispositivo);
        }

        /// <summary>
        /// Obtiene todos los dispositivos de registrados de los peatones o los empleados
        /// </summary>
        /// <param name="tipoDispositivo">Indica si se filtra por los dispositivos de los empleados o los peatones</param>
        /// <returns></returns>
        public List<PADispositivoMovil> ObtenerTodosDispositivosPeatonEmpleados(PAEnumTiposDispositivos tipoDispositivo)
        {
            return PARepositorio.Instancia.ObtenerTodosDispositivosPeatonEmpleados(tipoDispositivo);
        }
        /// <summary>
        /// Obtiene los dispositivos moviles de los empleados en una ciudad
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <returns></returns>
        public List<PADispositivoMovil> ObtenerDispositivosMovilesEmpleadosCiudad(string idLocalidad, bool esControllerApp = false)
        {
            return PARepositorio.Instancia.ObtenerDispositivosMovilesEmpleadosCiudad(idLocalidad, esControllerApp);
        }

        /// <summary>
        ///  Obtiene los dispositivos el dispositivo movil asociado a la identificacion de un empleado
        /// </summary>
        /// <param name="numeroIdentificacion"></param>
        /// <returns></returns>
        public List<PADispositivoMovil> ObtenerDispositivosMovilesIdentificacionEmpleado(long numeroIdentificacion)
        {
            return PARepositorio.Instancia.ObtenerDispositivosMovilesIdentificacionEmpleado(numeroIdentificacion);
        }

        /// <summary>
        /// Obtiene los dispositivos el dispositivo movil asociado a una empleado
        /// </summary>
        /// <param name="nombUsuario"></param>
        /// <returns></returns>
        public PADispositivoMovil ObtenerDispositivoMovilEmpleado(string nombUsuario)
        {
            return PARepositorio.Instancia.ObtenerDispositivoMovilEmpleado(nombUsuario);
        }
        /// <summary>
        /// Obtiene un dispositivo movil a partir del token y del sistema operativo
        /// </summary>
        /// <param name="tokenMovil"></param>
        /// <returns></returns>
        public PADispositivoMovil ObtenerDispositivoMovilTokenOs(string tokenMovil, PAEnumOsDispositivo sistemaOperativo)
        {
            return PARepositorio.Instancia.ObtenerDispositivoMovilTokenOs(tokenMovil, sistemaOperativo);
        }



        #endregion


        /// <summary>
        /// Verifica si la base de  datos está disponible
        /// </summary>
        /// <returns></returns>
        public bool VerificarConexionBD()
        {
            if (!PAParametros.VerificadorLineaEjecutando)
            {
                lock (this)
                {
                    if (!PAParametros.VerificadorLineaEjecutando)
                    {
                        PAParametros.SistemaEnLinea = PARepositorio.Instancia.VerificarConexionBD();
                        
                        Task.Factory.StartNew(() =>
                            {
                                while (true)
                                {
                                    System.Threading.Thread.Sleep(3000);                                   
                                    PAParametros.SistemaEnLinea = PARepositorio.Instancia.VerificarConexionBD();
                                }
                            });
                        PAParametros.VerificadorLineaEjecutando = true;
                    }
                }

            }

            return PAParametros.SistemaEnLinea;

            
        }

        public byte[] ObtenerArchivoDesdePath(string url)
        {

            try
            {
                FileStream fs = File.OpenRead(url);
                byte[] imagenArray = null;
                using (MemoryStream ms = new MemoryStream())
                {
                    fs.CopyTo(ms);
                    imagenArray = ms.ToArray();
                }

                fs.Close();
                return imagenArray;
            }
            catch (UnauthorizedAccessException ue)
            {
                throw new FaultException<ControllerException>(new ControllerException("Controller", "100", "Error: No se tienen permisos para acceder a la ruta."));
            }
            catch (FileNotFoundException fe)
            {
                throw new FaultException<ControllerException>(new ControllerException("Controller", "100", "Error: El archivo no se encuentra en el repositorio."));
            }


        }

    }
}