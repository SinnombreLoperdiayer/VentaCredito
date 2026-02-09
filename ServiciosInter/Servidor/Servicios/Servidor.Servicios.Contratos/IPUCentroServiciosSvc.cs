using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Servicios.Contratos
{
    /// <summary>
    /// Contratos WCF de centros de servicios
    /// </summary>
    [ServiceContract(Namespace = "http://contrologis.com")]
    public interface IPUCentroServiciosSvc
    {
        #region CRUD Tipo de comision fija

        /// <summary>
        /// Otiene los tipos de comision fija
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consult</param>
        /// <returns>Lista de tipos de comision fija</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GenericoConsultasFramework<PUTipoComisionFija> ObtenerTiposComisionFija(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente);

        /// <summary>
        /// Adiciona Modifica o elimina un tipo de comision fija
        /// </summary>
        /// <param name="tipoComisionFija">Objeto tipo de comision fija</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizarTipoComisionFija(PUTipoComisionFija tipoComisionFija);

        #endregion CRUD Tipo de comision fija

        #region CRUD Tipo de descuento

        /// <summary>
        /// Otiene los tipos de descuento
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consult</param>
        /// <returns>Lista de tipos de descuento</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GenericoConsultasFramework<PUTiposDescuento> ObtenerTiposDescuento(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente);

        /// <summary>
        /// Adiciona Modifica o elimina un tipo de descuento
        /// </summary>
        /// <param name="tipoDescuento">Objeto tipo de descuento</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActializarTipoDescuento(PUTiposDescuento tipoDescuento);

        #endregion CRUD Tipo de descuento

        #region CRUD Documentos centro de servicio

        /// <summary>
        /// Otiene los documentos de referencia de los centros de servicio
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consulta</param>
        /// <returns>Lista de documentos de referencia de los centros de servicio</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GenericoConsultasFramework<PUDocuCentroServicio> ObtenerDocuCentrosServicio(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente);

        /// <summary>
        /// Adiciona modifica o elimina un documento de referencia de los centros de servicio
        /// </summary>
        /// <param name="docuCentroServ">Objeto documento centro de servicio</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizarDocuCentrosServicio(PUDocuCentroServicio docuCentroServ);

        /// <summary>
        /// Obtiene los destinos de para la creacion de los documentos de referencia en centros de servicio
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<PUTipoDocumentosCentrosServicios> ObtenerTiposDocumentosCentroServicios();

        #endregion CRUD Documentos centro de servicio

        #region CRUD Suministros

        /// <summary>
        /// Otiene los suministros
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consulta</param>
        /// <returns>Lista de suministros</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GenericoConsultasFramework<PUSuministro> ObtenerSuministros(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente);

        /// <summary>
        /// Adiciona modifica o elimina  un suministro
        /// </summary>
        /// <param name="suministro">Objeto suministro</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizarSuministro(PUSuministro suministro);

        #endregion CRUD Suministros

        #region CRUD Servicio descuento Referencia

        /// <summary>
        /// Otiene los descuentos referencia de un servicio
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consulta</param>
        /// <returns>Lista de descuentos </returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GenericoConsultasFramework<PUServicioDescuentoRef> ObtenerDescuentoReferencia(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente);

        /// <summary>
        /// Adiciona, modifica o elimina un descuento referencia de un servicio
        /// </summary>
        /// <param name="descuentoRef">Objeto descuento referencia</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizarDescuentoRef(PUServicioDescuentoRef descuentoRef);

        #endregion CRUD Servicio descuento Referencia

        #region Propietarios (Concesionarios)

        /// <summary>
        /// Obtiene los propietarios(concesionarios)
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consult</param>
        /// <returns>Lista con los concesionarios</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GenericoConsultasFramework<PUPropietario> ObtenerPropietarios(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente);

        /// <summary>
        /// Adiciona, modifica o elimina un propietario
        /// </summary>
        /// <param name="propietario">Objeto propietario</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizarPropietario(PUPropietario propietario);

        #region Archivos

        /// <summary>
        /// Obtiene lista con los archivos de los propietarios
        /// </summary>
        /// <returns>lista con los archivos de los propietarios</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<PUArchivosPropietario> ObtenerArchivosPropietarios(PUPropietario propietario);

        /// <summary>
        ///Metodo para obtener un archivo asociado a un propietario
        /// </summary>
        /// <param name="archivo"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        string ObtenerArchivoPropietario(PUArchivosPropietario archivo);

        #endregion Archivos

        #endregion Propietarios (Concesionarios)

        #region Consultas

        /// <summary>
        /// Obtiene el centro de servicio.
        /// para el valor de la BaseInicial
        /// </summary>
        /// <param name="idCentroServicio">El idcentroservicio.</param>
        /// <returns>El centro de Servicio con la BaseInicial</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        PUCentroServiciosDC ObtenerCentroServicio(long idCentroServicio);

        /// <summary>
        /// Retorna la lista de centros de servicios activos en el sistema
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<PUCentroServiciosDC> ObtenerCentrosServiciosActivos();

        /// <summary>
        /// Retorna la lista con todos los centros de servicios del sistema
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<PUCentroServiciosDC> ObtenerTodosCentrosServicios();

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<PUCentroServiciosDC> ObtenerTodosCentrosServiciosXEstado(PAEnumEstados estado);

        /// <summary>
        /// Obtener todos los coles activos
        /// </summary>
        /// <returns>Colección con los coles activos</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<PUCentroServiciosDC> ObtenerTodosColes();

        /// <summary>
        /// Obtiene todos los col con sus puntos y agencias
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<PUCentroLogistico> ObtenerColConPuntosAgencias();

        /// <summary>
        /// Valida que una agencia pueda realizar venta de mensajería y retorna  la lista de servicios de mensajería habilitados
        /// </summary>
        /// <param name="idCentroServicios"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<TAServicioDC> ObtenerServiciosMensajeria(long idCentroServicios, int idListaPrecios);

        /// <summary>
        /// Retorna la lista de centro de servicios que reportan dinero a un centro de servicio dado
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <param name="idCentroServicioAQuienReportan"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GenericoConsultasFramework<PUCentroServicioReporte> ObtenerCentrosServicioReportan(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, long idCentroServicioAQuienReportan);

        /// <summary>
        /// Valida que una agencia pueda realizar venta de mensajería y retorna  la lista de servicios de mensajería habilitados
        /// </summary>
        /// <param name="idCentroServicios"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<TAServicioDC> ObtenerServiciosMensajeriaSinValidacionHorario(long idCentroServicios, int idListaPrecios);

        /// <summary>
        /// Otiene todos los tipos de descuento
        /// </summary>
        /// <returns>Lista con los tipos de descuento</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<PUTiposDescuento> ObtenerTodosTiposDescuento();

        /// <summary>
        /// Obtiene todos los servicios
        /// </summary>
        /// <returns>Lista con los servicios</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<PUServicio> ObtenerListaServicios();

        /// <summary>
        /// Otiene todos los tipos de comision
        /// </summary>
        /// <returns>Lista con los tipos de comision</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<PUTiposComision> ObtenerTiposComision();

        /// <summary>
        /// Obtiene las comisiones de referencia
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consulta</param>
        /// <returns>Lista de comisiones </returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GenericoConsultasFramework<PUServicioComisionRef> ObtenerComisionReferencia(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente);

        /// <summary>
        /// Adiciona, modifica o elimina una comision referencia a un servicio
        /// </summary>
        /// <param name="descuentoRef">Objeto comision referencia</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizarComisionReferencia(PUServicioComisionRef comisionRef);

        /// <summary>
        /// Obtiene todos los tipos de sociedad
        /// </summary>
        /// <returns>Lista con los tipos de sociedad</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<PUTipoSociedad> ObtenerTipoSociedad();

        /// <summary>
        /// Obtiene todos los tipos de regimen tributario
        /// </summary>
        /// <returns>Lista con los tipos de regimen tributario</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<PUTipoRegimen> ObtenerTiposRegimen();

        /// <summary>
        /// Obtiene las agencias de la aplicación
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<CO.Servidor.Servicios.ContratoDatos.CentroServicios.PUCentroServiciosDC> ObtenerAgencias(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente);


        /// <summary>
        /// Obtiene las Agencias y Bodegas para la Validacion y Archivo - Control de Cuentas
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<PUCentroServiciosDC> ObtenerAgenciasBodegas(IDictionary<string, string> filtro);//, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)


        /// <summary>
        /// Retorna las agencias creadas en el sistemas que se encuentran activas
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<PUAgencia> ObtenerAgenciasActivas();

        /// <summary>
        /// Obtiene los puntosde atencion
        /// de una agencia centro Servicio.
        /// </summary>
        /// <param name="idCentroServicio">The id centro servicio.</param>
        /// <returns>Lista de Puntos de Una Agencia</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<PUCentroServiciosDC> ObtenerPuntosDeAgencia(long idCentroServicio);

        /// <summary>
        /// Obtiene todas las opciones de clasificador canal ventas
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<PUClasificadorCanalVenta> ObtenerTodosClasificadorCanalVenta();

        /// <summary>
        /// Adiciona el centro de servicios al que se le reporta
        /// </summary>
        /// <param name="idCentroServicioAQuienReporta"></param>
        /// <param name="idCentroServicioReporta"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void AdicionarCentroServicioReporte(long idCentroServicioAQuienReporta, long idCentroServicioReporta);

        /// <summary>
        /// Eliminar el centro de servicios al que se le reporta
        /// </summary>
        /// <param name="idCentroServicioAQuienReporta"></param>
        /// <param name="idCentroServicioReporta"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void EliminarCentroServicioReporte(long idCentroServicioAQuienReporta, long idCentroServicioReporta);

        /// <summary>
        /// Retorna los centro de servicio que reportan a un centro de servicio dado
        /// </summary>
        /// <param name="idCentroServicio">Id del centro de servicio</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<PUCentroServicioReporte> ObtenerCentrosServicioReportanCentroServicio(long idCentroServicio);

        #endregion Consultas

        #region Codeudor

        /// <summary>
        /// Obtiene lista de codeudores de un Centro de servicio
        /// </summary>
        /// <returns>Lista con los codeudores de un  Centro de servicio</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GenericoConsultasFramework<PUCodeudor> ObtenerCodeudoresXCentroServicio(long idCentroServicio, IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente);

        /// <summary>
        /// Actualiza un codeudor
        /// </summary>
        /// <param name="codeudor">Objeto codeudor</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizarCodeudor(PUCodeudor codeudor);

        #endregion Codeudor

        #region Datos Bancarios

        /// <summary>
        /// Adiciona o modifica los datos bancarios de un agente comercial (propietario)
        /// </summary>
        /// <param name="datosBanca">Objeto datos bancarios</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizaInfoBancaria(PUPropietarioBanco datosBanca);

        /// <summary>
        /// Obtiene la informacion bancaria de un agente comercial(propietario)
        /// </summary>
        /// <param name="idPropietario"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        PUPropietarioBanco ObtenerDatosBancariosPropietario(int idPropietario);

        #endregion Datos Bancarios

        #region Centro de servicios


        /// <summary>
        /// Obtiene el col responsable de la agencia de una localidad
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        long ObtieneIdCOLResponsableAgenciaLocalidad(string idLocalidad);

        /// <summary>
        /// Obtiene todas las agencias y puntos en el sistema
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<PUCentroServiciosDC> ObtenerTodosAgenciayPuntosActivos();

        /// <summary>
        /// Obtiene todas las agencias, col y puntos en el sistema
        /// </summary>
        /// <returns></returns>
        List<PUCentroServiciosDC> ObtenerTodosAgenciaColPuntosActivos();

        /// <summary>
        /// Metodo que consulta todas las agencias
        /// y puntos de atención de los Racoles
        /// </summary>
        /// <param name="idRacol"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<PUCentroServiciosDC> ObtenerCentrosServicios(long idRacol);

        /// <summary>
        /// Método para obtener las agencias de un COL que sean de tipo ARO, mas los puntos de la ciudad del COL
        /// </summary>
        /// <param name="idCol"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<PUCentroServiciosDC> ObtenerCentrosServiciosAsignacionTulas(long idCol);

        /// <summary>
        /// Obtiene los Centros de servicio
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consult</param>
        /// <returns>Lista con los centros de servicio</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GenericoConsultasFramework<PUCentroServiciosDC> ObtenerCentrosServicio(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, int? idPropietario);

        /// <summary>
        /// Método que devuelve una lista con todos los centros de servicio
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<PUCentroServiciosDC> ObtenerCentrosServicioActivosLocalidad(string idMunicipio);

        /// <summary>
        /// Método que devuelve una lista con todos los centros de servicio de una actividad
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<PUCentroServiciosDC> ObtenerTodosCentrosServicioPorLocalidad(string idMunicipio);

        /// <summary>
        /// Obtiene los Centros de servicio de acuerdo a una localidad
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consult</param>
        /// <returns>Lista con los centros de servicio</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GenericoConsultasFramework<PUCentroServiciosDC> ObtenerCentrosServicioPorLocalidad(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, string IdLocalidad, PUEnumTipoCentroServicioDC tipoCES);

        /// <summary>
        /// Adiciona modifica o elimina un centro de servicios
        /// </summary>
        /// <param name="descuentoRef">Objeto centro de servicios</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizarCentrosServicio(PUCentroServiciosDC centroServicios);

        /// <summary>
        /// Inhabilita los ususrios que pertenezcan solo al centro de servicio seleccionado
        /// </summary>
        /// <param name="centroServicios"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void InhabilitarUsuariosCentroServicio(long idCentroServicio);

        /// <summary>
        /// Consulta los tipos de agencia
        /// </summary>
        /// <returns>Lista con los tipos de agencia</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<PUTipoAgencia> ObtenerTiposAgencia();

        /// <summary>
        /// Obtiene los tipos de propiedad
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<PUTipoPropiedad> ObtenerTiposPropiedad();

        /// <summary>
        /// Obtiene los estados para los centros de servicio
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<PUEstadoDC> ObtenerEstados();

        /// <summary>
        /// Obtiene todas las listas necesarias para parametrizar los centros de servicio
        /// </summary>
        /// <returns>Objeto con las lista requeridas en los centros de servicio</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        PUListasCentrosServicio ObtenerListasCentrosServicio();

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<string> ObtenerListaCenCostoNovasoft();

        /// <summary>
        /// Obtiene la lista de las agencias que pueden realizar pagos de giros
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<PUCentroServiciosDC> ObtenerAgenciasPuedenPagarGiros();

        /// <summary>
        /// Obtiene lista con los archivos de los centros de servicio
        /// </summary>
        /// <returns>objeto de centro de servicio</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<PUArchivoCentroServicios> ObtenerArchivosCentroServicios(PUCentroServiciosDC centroServicios);

        /// <summary>
        ///Metodo para obtener un archivo asociado a un centro Servicio
        /// </summary>
        /// <param name="archivo"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        string ObtenerArchivoCentroServicio(PUArchivoCentroServicios archivo);

        #region Suministros de centros de servicio

        /// <summary>
        /// Obtiene todos los suministros
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<PUSuministro> ObtenerTodosSuministros();

        /// <summary>
        /// Adiciona modifica o elimina un centro de servicios
        /// </summary>
        /// <param name="descuentoRef">Objeto centro de servicios</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizarCentroServiciosSuministros(PUCentroServiciosSuministro centroServiciosSuministro);

        /// <summary>
        /// Obtiene suministros de un centro de servicio
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consult</param>
        /// <param name="IdCentroServicios">Id del centro de servicio</param>
        /// <returns>Lista suministros del centro de servicio</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GenericoConsultasFramework<PUCentroServiciosSuministro> ObtenerSuministrosPorCentrosServicio(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, long IdCentroServicios);

        #endregion Suministros de centros de servicio

        /// <summary>
        /// Envia la divulgacion de una agencia
        /// </summary>
        /// <param name="idCentroServicios"></param>
        /// <param name="divulgacion">Objeto con la informacion de los contactos a divulgar la agencia</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void DivulgarAgencia(long idCentroServicios, PADivulgacion divulgacion);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        PUAgenciaDeRacolDC ObtenerAgenciaResponsable(long idPuntoServicio);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        PUAgenciaDeRacolDC ObtenerRacolResponsable(long idAgencia);

        /// <summary>
        /// Metodo para consultar las localidades donde existen centros logisticos
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<LILocalidadColDC> ObtenerLocalidadesCol();

        /// <summary>
        /// Metodo que consulta todas las agencias y puntos de un RACOL
        /// </summary>
        /// <param name="idRacol"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<PUCentroServiciosDC> ObtenerAgenciasYPuntosRacolActivos(long idRacol);

        /// <summary>
        /// Metodo que consulta todas las agencias
        /// </summary>
        /// <param name="idRacol"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<PUCentroServiciosDC> ObtenerAgenciasRacolActivos(long idRacol);

        /// <summary>
        /// Metodo para consultar las agencias que dependen de un COL
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<LILocalidadColDC> ObtenerAgenciasCol(long idCol);

        /// <summary>
        /// Obtiene los datos básicos de los centros de servivios de giros
        /// </summary>
        /// <returns>Colección centros de servicio</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IEnumerable<PUCentroServiciosDC> ObtenerCentrosServicioGiros();

        /// <summary>
        /// Obtiene los puntos del centro logistico
        /// </summary>
        /// <param name="idCol"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<PUCentroServiciosDC> ObtenerPuntosServiciosCol(long idCol);

        /// <summary>
        /// Obtiene el id y la descripcion de todos los centros logisticos activos y racol activos
        /// </summary>
        /// <returns>lista de centros logisticos y racol </returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<PUCentroServicioApoyo> ObtenerCentrosServicioApoyo();

        /// <summary>
        /// Metodo para Obtener el RACOL
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<PURegionalAdministrativa> ObtenerRegionalAdministrativa();

        /// <summary>
        /// Metodo para Obtener la RACOL de un municipio
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        PURegionalAdministrativa ObtenerRegionalAdministrativaPorMunicipio(string idMunicipio);


        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<PALocalidadDC> ObtenerMunicipiosXCol(long IdCol);

        /// <summary>
        /// Consulta los municipios y su respectivo centro logístico asociado
        /// </summary>
        /// <param name="IdDepartamento">Id del departamento por el cual se quiere filtrar</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<PUMunicipioCentroLogisticoDC> ConsultarMunicipiosCol(string IdDepartamento);

        /// <summary>
        /// Guarda en la base de datos el municipio con su respectivo centro logistico de apoyo
        /// </summary>
        /// <param name="municipioCol"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GuardarMunicipioCol(PUMunicipioCentroLogisticoDC municipioCol);

        /// <summary>
        /// Consultar todos los centros de servicios de un RACOL y todos los RACOL activos
        /// </summary>
        /// <param name="idRacol">Identificador del RACOL</param>
        /// <returns>Colección de los centros de servicios de un RACOL y todos los RACOL activos</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<PUCentroServiciosDC> ObtenerCentrosServiciosDeRacolYTodosRacol(long idRacol);

        /// <summary>
        /// Obtiene el horario de la recogida de un centro de Servicio
        /// </summary>
        /// <param name="idCentroSvc">es le id del centro svc</param>
        /// <returns>info de la recogida</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<PUHorarioRecogidaCentroSvcDC> ObtenerHorariosRecogidasCentroSvc(long idCentroSvc);

        /// <summary>
        /// Adiciona los Horarios de las recogidas
        /// de los centros de Svc
        /// </summary>
        /// <param name="centroServicios">info del Centro de Servicio</param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void AdicionarHorariosRecogidasCentroSvc(PUCentroServiciosDC centroServicios);


        /// <summary>
        /// Método para obtener los puntos y agencias de un col
        /// </summary>
        /// <param name="idCol"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<PUCentroServiciosDC> ObtenerPuntosAgenciasCol(long idCol);




        #endregion Centro de servicios

        #region Autorizacion Suministros

        /// <summary>
        /// Guardar los suministros que posee un centro de servicio
        /// </summary>
        /// <param name="suministroCentroServicio"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GuardarSuministroCentroServicio(SUSuministroCentroServicioDC suministroCentroServicio);

        #endregion Autorizacion Suministros

        #region Clasificador canal de venta

        /// <summary>
        /// Inserta Modifica o Elimina un registro de clasificador de canal de venta
        /// </summary>
        /// <param name="clasificadorCanalVenta"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizarClasificarCanalVenta(PUClasificadorCanalVenta clasificadorCanalVenta);

        /// <summary>
        /// Obtiene los clasificadores del canal de ventas
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        GenericoConsultasFramework<PUClasificadorCanalVenta> ObtenerClasificadorCanalVenta(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente);

        /// <summary>
        /// Selecciona todos los tipos de centros de servicio
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<PUTipoCentroServicio> ObtenerTodosTipoCentroServicio();

        #endregion Clasificador canal de venta

        /// <summary>
        /// Obtienen todos los municipios de un racol
        /// </summary>
        /// <param name="idRacol">id Racol</param>
        /// <returns>municipios del racol</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<PALocalidadDC> ObtenerMunicipiosDeRacol(long idRacol);

        /// <summary>
        /// Retorna los municipios que no permiten forma de pago "Al Cobro"
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
        GenericoConsultasFramework<PUMunicipiosSinAlCobro> ObtenerMunicipiosSinFormaPagoAlCobro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente);

        #region Casa Matriz

        /// <summary>
        /// Obtener la información basica de las Regionales Administrativas activas de una Casa Matriz
        /// </summary>
        /// <param name="idCasaMatriz">Identificador de la Casa Matriz</param>
        /// <returns>Colección con la información básica de las regionales</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        IList<PURegionalAdministrativa> ObtenerRegionalesDeCasaMatriz(short idCasaMatriz);

        #endregion Casa Matriz

        /// <summary>
        /// Agrega municipio a la lista de municipios que no admiten forma de pago "al cobro"
        /// </summary>
        /// <param name="municipio"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void RegistrarMunicipioSinFormaPagoAlCobro(PALocalidadDC municipio);

        /// <summary>
        /// Quita municipio de la lista de municipios que no admiten forma de pago "al cobro"
        /// </summary>
        /// <param name="municipio"></param>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void RemoverMunicipioSinFormaPagoAlCobro(PALocalidadDC municipio);

        /// <summary>
        /// Obtener Direccion Puntos Segun localidad Destino que tenga habilitado "Reclame en oficina"
        /// </summary>
        /// <param name="idLocalidadDestino"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<string> ObtenerDireccionPuntosSegunLocalidadDestinatario(int idLocalidadDestino);

        /// <summary>
        /// Obtener Direccion Puntos Segun localidad Destino que tenga habilitado "Reclame en oficina"
        /// </summary>
        /// <param name="idLocalidadDestino"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<PUCentroServicioApoyo> ObtenerPuntosREOSegunUbicacionDestino(int idLocalidadDestino);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<PUCentroServiciosDC> ObtenerTodosCentrosServiciosNoInactivos();


        /// <summary>
        /// Obtiene los horarios de un centro de servicios para la app de recogidas
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<string> ObtenerHorariosCentroServicioAppRecogidas(long idCentroServicio);



        /// <summary>
        /// Método para obtener los puntos y agencias de un col
        /// </summary>
        /// <param name="idCol"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<PUCentroServiciosDC> ObtenerPuntosAgenciasColReclamaOficina(long idCol);

        /// <summary>
        /// Obtiene la bodega de custodia
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        PUCentroServiciosDC ObtenerBodegaCustodia();

        /// <summary>
        /// Obtiene los servicios junto con sus horarios de venta de un centro de servicio
        /// </summary>
        /// <param name="idServicio"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<PUServicio> ObtenerCentrosServicioPorServicio(int idServicio);

        /// <summary>
        /// Obtiene el centro de acopio de determinada bodega
        /// </summary>
        /// <param name="idBodega"></param>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        PUCentroServiciosDC ObtenerCentroDeAcopioBodega(long idBodega, long idUsuario);

        /// <summary>
        /// Obtiene la bodega de Logistica Inversa
        /// </summary>
        /// <param name="localidad"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        PUCentroServiciosDC ObtenerCentroConfirmacionesDevoluciones(PALocalidadDC localidad);

        /// <summary>
        /// Asigna y da salida a centro de acopio
        /// </summary>
        /// <param name="movimientoInventario"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        long AdicionarMovimientoInventario(PUMovimientoInventario movimientoInventario);


        /// <summary>
        /// Obtiene las Territoriales
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<PUTerritorialDC> ObtenerTerritoriales();

        /// <summary>
        /// Obtiene los centros de servicio a los cuales tiene acceso el usuario
        /// </summary>
        /// <param name="identificacionUsuario"></param>
        /// <returns></returns>
        /// 
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<PUCentroServiciosDC> ObtenerLocacionesAutorizadas(string usuario);

        /// <summary>
        /// Obtiene el numero total de envios en custodia
        /// </summary>
        /// <param name="idTipoMovimiento"></param>
        /// <param name="idEstadoGuia"></param>
        /// <returns></returns>
        /// 
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        int ObtenerConteoGuiasCustodia(int idTipoMovimiento, int idEstadoGuia);


        /// <summary>
        /// Obtiene el numero total de envios en pendientyes por ingr a custodia
        /// </summary>
        /// <param name="idTipoMovimiento"></param>
        /// <param name="idEstadoGuia"></param>
        /// <returns></returns>
        /// 
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        int ObtenerConteoPendIngrCustodia(int idTipoMovimiento, int idEstadoGuia);


        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<PUCentroServiciosDC> ObtenerCentrosServicioTipo();

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<PUTipoZona> ObtenerTiposZona();

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<PUTipoCiudad> ObtenerTiposCiudades();
    }
}