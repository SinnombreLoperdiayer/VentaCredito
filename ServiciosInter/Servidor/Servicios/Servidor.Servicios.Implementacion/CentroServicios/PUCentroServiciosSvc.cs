using System.Collections.Generic;
using System.Globalization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Threading;
using CO.Servidor.CentroServicios;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using CO.Servidor.Servicios.Contratos;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;

namespace CO.Servidor.Servicios.Implementacion.CentroServicios
{
    /// <summary>
    /// Clase para los servicios WCF del modulo Centros de servicios
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class PUCentroServiciosSvc : IPUCentroServiciosSvc
    {
        public PUCentroServiciosSvc()
        {
        }

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
        public GenericoConsultasFramework<PUTipoComisionFija> ObtenerTiposComisionFija(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente)
        {
            int totalRegistros = 0;

            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<PUTipoComisionFija>()
            {
                Lista = PUCentroServicios.Instancia.ObtenerTiposComisionFija(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Adiciona Modifica o elimina un tipo de comision fija
        /// </summary>
        /// <param name="tipoComisionFija">Objeto tipo de comision fija</param>
        public void ActualizarTipoComisionFija(PUTipoComisionFija tipoComisionFija)
        {
            PUCentroServicios.Instancia.ActualizarTipoComisionFija(tipoComisionFija);
        }

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
        public GenericoConsultasFramework<PUTiposDescuento> ObtenerTiposDescuento(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente)
        {
            int totalRegistros = 0;
            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<PUTiposDescuento>()
            {
                Lista = PUCentroServicios.Instancia.ObtenerTiposDescuento(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Adiciona Modifica o elimina un tipo de descuento
        /// </summary>
        /// <param name="tipoDescuento">Objeto tipo de descuento</param>
        public void ActializarTipoDescuento(PUTiposDescuento tipoDescuento)
        {
            PUCentroServicios.Instancia.ActializarTipoDescuento(tipoDescuento);
        }

        /// <summary>
        /// Obtiene el numero total de envios en custodia
        /// </summary>
        /// <param name="idTipoMovimiento"></param>
        /// <param name="idEstadoGuia"></param>
        /// <returns></returns>
        public int ObtenerConteoGuiasCustodia(int idTipoMovimiento, int idEstadoGuia)
        {
            return PUCentroServicios.Instancia.ObtenerConteoGuiasCustodia(idTipoMovimiento, idEstadoGuia);
        }

        /// <summary>
        /// Obtiene el numero total de envios en pendientyes por ingr a custodia
        /// </summary>
        /// <param name="idTipoMovimiento"></param>
        /// <param name="idEstadoGuia"></param>
        /// <returns></returns>
        public int ObtenerConteoPendIngrCustodia(int idTipoMovimiento, int idEstadoGuia)
        {
            return PUCentroServicios.Instancia.ObtenerConteoPendIngrCustodia(idTipoMovimiento, idEstadoGuia);
        }

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
        public GenericoConsultasFramework<PUDocuCentroServicio> ObtenerDocuCentrosServicio(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente)
        {
            int totalRegistros = 0;
            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<PUDocuCentroServicio>()
            {
                Lista = PUCentroServicios.Instancia.ObtenerDocuCentrosServicio(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Adiciona modifica o elimina un documento de referencia de los centros de servicio
        /// </summary>
        /// <param name="docuCentroServ">Objeto documento centro de servicio</param>
        public void ActualizarDocuCentrosServicio(PUDocuCentroServicio docuCentroServ)
        {
            PUCentroServicios.Instancia.ActualizarDocuCentrosServicio(docuCentroServ);
        }

        /// <summary>
        /// Obtiene los destinos de para la creacion de los documentos de referencia en centros de servicio
        /// </summary>
        /// <returns></returns>
        public IList<PUTipoDocumentosCentrosServicios> ObtenerTiposDocumentosCentroServicios()
        {
            return PUCentroServicios.Instancia.ObtenerTiposDocumentosCentroServicios();
        }

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
        public GenericoConsultasFramework<PUSuministro> ObtenerSuministros(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente)
        {
            int totalRegistros = 0;
            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<PUSuministro>()
            {
                Lista = PUCentroServicios.Instancia.ObtenerSuministros(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Adiciona modifica o elimina  un suministro
        /// </summary>
        /// <param name="suministro">Objeto suministro</param>
        public void ActualizarSuministro(PUSuministro suministro)
        {
            PUCentroServicios.Instancia.ActualizarSuministro(suministro);
        }

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
        public GenericoConsultasFramework<PUServicioDescuentoRef> ObtenerDescuentoReferencia(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente)
        {
            int totalRegistros = 0;
            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<PUServicioDescuentoRef>()
            {
                Lista = PUCentroServicios.Instancia.ObtenerDescuentoReferencia(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Adiciona, modifica o elimina un descuento referencia de un servicio
        /// </summary>
        /// <param name="descuentoRef">Objeto descuento referencia</param>
        public void ActualizarDescuentoRef(PUServicioDescuentoRef descuentoRef)
        {
            PUCentroServicios.Instancia.ActualizarDescuentoRef(descuentoRef);
        }

        #endregion CRUD Servicio descuento Referencia

        #region CRUD Servicio comision Referencia

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
        public GenericoConsultasFramework<PUServicioComisionRef> ObtenerComisionReferencia(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente)
        {
            int totalRegistros = 0;
            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<PUServicioComisionRef>()
            {
                Lista = PUCentroServicios.Instancia.ObtenerComisionReferencia(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Adiciona, modifica o elimina una comision referencia a un servicio
        /// </summary>
        /// <param name="descuentoRef">Objeto comision referencia</param>
        public void ActualizarComisionReferencia(PUServicioComisionRef comisionRef)
        {
            PUCentroServicios.Instancia.ActualizarComisionReferencia(comisionRef);
        }

        #endregion CRUD Servicio comision Referencia

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
        public GenericoConsultasFramework<PUPropietario> ObtenerPropietarios(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente)
        {
            int totalRegistros = 0;
            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<PUPropietario>()
            {
                Lista = PUCentroServicios.Instancia.ObtenerPropietarios(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Adiciona, modifica o elimina un propietario
        /// </summary>
        /// <param name="propietario">Objeto propietario</param>
        public void ActualizarPropietario(PUPropietario propietario)
        {
            PUCentroServicios.Instancia.ActualizarPropietario(propietario);
        }

        #region Archivos

        /// <summary>
        /// Obtiene lista con los archivos de los propietarios
        /// </summary>
        /// <returns>lista con los archivos de los propietarios</returns>
        public IEnumerable<PUArchivosPropietario> ObtenerArchivosPropietarios(PUPropietario propietario)
        {
            return PUCentroServicios.Instancia.ObtenerArchivosPropietarios(propietario);
        }

        /// <summary>
        ///Metodo para obtener un archivo asociado a un propietario
        /// </summary>
        /// <param name="archivo"></param>
        /// <returns></returns>
        public string ObtenerArchivoPropietario(PUArchivosPropietario archivo)
        {
            return PUCentroServicios.Instancia.ObtenerArchivoPropietario(archivo);
        }

        #endregion Archivos

        #endregion Propietarios (Concesionarios)

        #region Codeudor

        /// <summary>
        /// Obtiene lista de codeudores de un Centro de servicio
        /// </summary>
        /// <returns>Lista con los codeudores de un  Centro de servicio</returns>
        public GenericoConsultasFramework<PUCodeudor> ObtenerCodeudoresXCentroServicio(long idCentroServicio, IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente)
        {
            int totalRegistros = 0;
            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<PUCodeudor>()
            {
                Lista = PUCentroServicios.Instancia.ObtenerCodeudoresXCentroServicio(idCentroServicio, filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Actualiza un codeudor
        /// </summary>
        /// <param name="codeudor">Objeto codeudor</param>
        public void ActualizarCodeudor(PUCodeudor codeudor)
        {
            PUCentroServicios.Instancia.ActualizarCodeudor(codeudor);
        }

        #endregion Codeudor

        #region Consultas

        /// <summary>
        /// Obtiene el centro de servicio.
        /// para el valor de la BaseInicial
        /// </summary>
        /// <param name="idCentroServicio">El idcentroservicio.</param>
        /// <returns>El centro de Servicio con la BaseInicial</returns>
        public PUCentroServiciosDC ObtenerCentroServicio(long idCentroServicio)
        {
            return PUAdministradorCentroServicios.Instancia.ObtenerCentroServicio(idCentroServicio);
        }

        /// <summary>
        /// Retorna la lista de centros de servicios activos en el sistema
        /// </summary>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerCentrosServiciosActivos()
        {
            return PUAdministradorCentroServicios.Instancia.ObtenerCentrosServiciosActivos();
        }


        /// <summary>
        /// Retorna la lista con todos los centros de servicios del sistema
        /// </summary>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerTodosCentrosServicios()
        {
            return PUAdministradorCentroServicios.Instancia.ObtenerTodosCentrosServicios();
        }

        /// <summary>
        /// Retorna la lista con todos los centros de servicios del sistema
        /// </summary>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerTodosCentrosServiciosXEstado(PAEnumEstados estado)
        {
            return PUAdministradorCentroServicios.Instancia.ObtenerTodosCentrosServiciosXEstado(estado);
        }

        public List<PUCentroServiciosDC> ObtenerTodosCentrosServiciosNoInactivos()
        {
            return PUAdministradorCentroServicios.Instancia.ObtenerTodosCentrosServiciosNoInactivos();
        }

        /// <summary>
        /// Obtener todos los coles activos
        /// </summary>
        /// <returns>Colección con los coles activos</returns>
        public List<PUCentroServiciosDC> ObtenerTodosColes()
        {
            return PUAdministradorCentroServicios.Instancia.ObtenerTodosColes();
        }

        /// <summary>
        /// Método para obtener los puntos y agencias de un col
        /// </summary>
        /// <param name="idCol"></param>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerPuntosAgenciasCol(long idCol)
        {
            return PUAdministradorCentroServicios.Instancia.ObtenerPuntosAgenciasCol(idCol);
        }

        /// <summary>
        /// Obtiene todos los col con sus puntos y agencias
        /// </summary>
        /// <returns></returns>
        public List<PUCentroLogistico> ObtenerColConPuntosAgencias()
        {
            return PUAdministradorCentroServicios.Instancia.ObtenerColConPuntosAgencias();
        }


        /// <summary>
        /// Otiene todos los tipos de descuento
        /// </summary>
        /// <returns>Lista con los tipos de descuento</returns>
        public IList<PUTiposDescuento> ObtenerTodosTiposDescuento()
        {
            return PUCentroServicios.Instancia.ObtenerTiposDescuento();
        }

        /// <summary>
        /// Obtiene todos los servicios
        /// </summary>
        /// <returns>Lista con los servicios</returns>
        public IList<PUServicio> ObtenerListaServicios()
        {
            return PUCentroServicios.Instancia.ObtenerServicios();
        }

        /// <summary>
        /// Otiene todos los tipos de comision
        /// </summary>
        /// <returns>Lista con los tipos de comision</returns>
        public IList<PUTiposComision> ObtenerTiposComision()
        {
            return PUCentroServicios.Instancia.ObtenerTiposComision();
        }

        /// <summary>
        /// Obtiene todos los tipos de sociedad
        /// </summary>
        /// <returns>Lista con los tipos de sociedad</returns>
        public IList<PUTipoSociedad> ObtenerTipoSociedad()
        {
            return PUCentroServicios.Instancia.ObtenerTipoSociedad();
        }

        /// <summary>
        /// Obtiene todos los tipos de regimen tributario
        /// </summary>
        /// <returns>Lista con los tipos de regimen tributario</returns>
        public IList<PUTipoRegimen> ObtenerTiposRegimen()
        {
            return PUCentroServicios.Instancia.ObtenerTiposRegimen();
        }

        /// <summary>
        /// Valida que una agencia pueda realizar venta de mensajería y retorna  la lista de servicios de mensajería habilitados
        /// </summary>
        /// <param name="idCentroServicios"></param>
        public IEnumerable<TAServicioDC> ObtenerServiciosMensajeria(long idCentroServicios, int idListaPrecios)
        {
            return PUAdministradorCentroServicios.Instancia.ObtenerServiciosMensajeria(idCentroServicios, idListaPrecios);
        }

        /// <summary>
        /// Valida que una agencia pueda realizar venta de mensajería y retorna  la lista de servicios de mensajería habilitados
        /// </summary>
        /// <param name="idCentroServicios"></param>
        public IEnumerable<TAServicioDC> ObtenerServiciosMensajeriaSinValidacionHorario(long idCentroServicios, int idListaPrecios)
        {
            return PUAdministradorCentroServicios.Instancia.ObtenerServiciosMensajeriaSinValidacionHorario(idCentroServicios, idListaPrecios);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerAgenciasBodegas(IDictionary<string, string> filtro)//, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return PUAdministradorCentroServicios.Instancia.ObtenerAgenciasBodegas(filtro);
        }



        /// <summary>
        /// Obtiene las agencias de la aplicación
        /// </summary>
        public Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<PUCentroServiciosDC> ObtenerAgencias(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente)
        {
            int totalRegistros = 0;
            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<PUCentroServiciosDC>()
            {
                Lista = PUAdministradorCentroServicios.Instancia.ObtenerAgencias(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Obtiene todas las opciones de clasificador canal ventas
        /// </summary>
        /// <returns></returns>
        public IList<PUClasificadorCanalVenta> ObtenerTodosClasificadorCanalVenta()
        {
            return PUAdministradorCentroServicios.Instancia.ObtenerTodosClasificadorCanalVenta();
        }

        /// <summary>
        /// Retorna los centro de servicio que reportan a un centro de servicio dado
        /// </summary>
        /// <param name="idCentroServicio">Id del centro de servicio</param>
        /// <returns></returns>
        public List<PUCentroServicioReporte> ObtenerCentrosServicioReportanCentroServicio(long idCentroServicio)
        {
            return PUAdministradorCentroServicios.Instancia.ObtenerCentrosServicioReportanCentroServicio(idCentroServicio);
        }

        #endregion Consultas

        #region Datos Bancarios

        /// <summary>
        /// Adiciona o modifica los datos bancarios de un agente comercial (propietario)
        /// </summary>
        /// <param name="datosBanca">Objeto datos bancarios</param>
        public void ActualizaInfoBancaria(PUPropietarioBanco datosBanca)
        {
            PUCentroServicios.Instancia.ActualizaInfoBancaria(datosBanca);
        }

        /// <summary>
        /// Obtiene la informacion bancaria de un agente comercial(propietario)
        /// </summary>
        /// <param name="idPropietario"></param>
        /// <returns></returns>
        public PUPropietarioBanco ObtenerDatosBancariosPropietario(int idPropietario)
        {
            return PUCentroServicios.Instancia.ObtenerDatosBancariosPropietario(idPropietario);
        }

        #endregion Datos Bancarios

        #region Centro de servicios

        /// <summary>
        /// Metodo que consulta todas las agencias
        /// y puntos de atención de los Racoles
        /// </summary>
        /// <param name="idRacol"></param>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerCentrosServicios(long idRacol)
        {
            return PUAdministradorCentroServicios.Instancia.ObtenerCentrosServicios(idRacol);
        }

        /// <summary>
        /// Obtiene los Centros de Servicios Activos e Inactivos de una Racol
        /// </summary>
        /// <param name="idRacol"></param>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerCentrosServiciosTodos(long idRacol)
        {
            return PUAdministradorCentroServicios.Instancia.ObtenerCentrosServiciosTodos(idRacol);
        }


        /// <summary>
        /// Método que devuelve una lista con todos los centros de servicio
        /// </summary>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerCentrosServicioActivosLocalidad(string idMunicipio)
        {
            return PUAdministradorCentroServicios.Instancia.ObtenerCentrosServicioActivosLocalidad(idMunicipio);
        }


        /// <summary>
        /// Método para obtener las agencias de un COL que sean de tipo ARO, mas los puntos de la ciudad del COL
        /// </summary>
        /// <param name="idCol"></param>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerCentrosServiciosAsignacionTulas(long idCol)
        {
            return PUAdministradorCentroServicios.Instancia.ObtenerCentrosServiciosAsignacionTulas(idCol);
        }

        /// <summary>
        /// Método que devuelve una lista con todos los centros de servicio de una actividad
        /// </summary>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerTodosCentrosServicioPorLocalidad(string idMunicipio)
        {
            return PUAdministradorCentroServicios.Instancia.ObtenerTodosCentrosServicioPorLocalidad(idMunicipio);
        }

        public List<PUTipoCiudad> ObtenerTiposCiudades()
        {
            return PUAdministradorCentroServicios.Instancia.ObtenerTiposCiudades();
        }

        public List<PUTipoZona> ObtenerTiposZona()
        {
            return PUAdministradorCentroServicios.Instancia.ObtenerTiposZona();
        }

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
        public GenericoConsultasFramework<PUCentroServiciosDC> ObtenerCentrosServicio(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, int? idPropietario)
        {
            int totalRegistros = 0;
            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<PUCentroServiciosDC>()
            {
                Lista = PUCentroServicios.Instancia.ObtenerCentrosServicio(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idPropietario),
                TotalRegistros = totalRegistros
            };
        }

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
        public GenericoConsultasFramework<PUCentroServiciosDC> ObtenerCentrosServicioPorLocalidad(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, string IdLocalidad, PUEnumTipoCentroServicioDC tipoCES)
        {
            int totalRegistros = 0;
            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<PUCentroServiciosDC>()
            {
                Lista = PUCentroServicios.Instancia.ObtenerCentrosServicioPorLocalidad(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, IdLocalidad, tipoCES),
                TotalRegistros = totalRegistros
            };
        }

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
        public GenericoConsultasFramework<PUCentroServicioReporte> ObtenerCentrosServicioReportan(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, long idCentroServicioAQuienReportan)
        {
            int totalRegistros = 0;
            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<PUCentroServicioReporte>()
            {
                Lista = PUAdministradorCentroServicios.Instancia.ObtenerCentrosServicioReportan(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idCentroServicioAQuienReportan),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Adiciona modifica o elimina un centro de servicios
        /// </summary>
        /// <param name="descuentoRef">Objeto centro de servicios</param>
        public void ActualizarCentrosServicio(PUCentroServiciosDC centroServicios)
        {
            PUCentroServicios.Instancia.ActualizarCentrosServicio(centroServicios);
        }

        public void InhabilitarUsuariosCentroServicio(long idCentroServicio)
        {
            PUCentroServicios.Instancia.InhabilitarUsuariosCentroServicio(idCentroServicio);
        }

        /// <summary>
        /// Consulta los tipos de agencia
        /// </summary>
        /// <returns>Lista con los tipos de agencia</returns>
        public IList<PUTipoAgencia> ObtenerTiposAgencia()
        {
            return PUCentroServicios.Instancia.ObtenerTiposAgencia();
        }

        /// <summary>
        /// Obtiene los tipos de propiedad
        /// </summary>
        /// <returns></returns>
        public IList<PUTipoPropiedad> ObtenerTiposPropiedad()
        {
            return PUCentroServicios.Instancia.ObtenerTiposPropiedad();
        }

        /// <summary>
        /// Obtiene los estados para los centros de servicio
        /// </summary>
        /// <returns></returns>
        public IList<PUEstadoDC> ObtenerEstados()
        {
            return PUCentroServicios.Instancia.ObtenerEstados();
        }

        /// <summary>
        /// Obtiene todas las listas necesarias para parametrizar los centros de servicio
        /// </summary>
        /// <returns>Objeto con las lista requeridas en los centros de servicio</returns>
        public PUListasCentrosServicio ObtenerListasCentrosServicio()
        {
            return PUCentroServicios.Instancia.ObtenerListasCentrosServicio();
        }

        /// <summary>
        /// Obtiene la lista de Centros de Costo activo Novasoft para aosciar a un centro de costo.
        /// </summary>
        /// <returns>Objeto con las lista requeridas en los centros de servicio</returns>
        public IList<string> ObtenerListaCenCostoNovasoft()
        {
            return PUCentroServicios.Instancia.ObtenerCenCostoNovasoft();
        }

        /// <summary>
        /// Obtiene la lista de las agencias que pueden realizar pagos de giros
        /// </summary>
        public IList<PUCentroServiciosDC> ObtenerAgenciasPuedenPagarGiros()
        {
            return PUCentroServicios.Instancia.ObtenerAgenciasPuedenPagarGiros();
        }

        /// <summary>
        /// Obtiene lista con los archivos de los centros de servicio
        /// </summary>
        /// <returns>objeto de centro de servicio</returns>
        public IEnumerable<PUArchivoCentroServicios> ObtenerArchivosCentroServicios(PUCentroServiciosDC centroServicios)
        {
            return PUCentroServicios.Instancia.ObtenerArchivosCentroServicios(centroServicios);
        }

        /// <summary>
        ///Metodo para obtener un archivo asociado a un centro Servicio
        /// </summary>
        /// <param name="archivo"></param>
        /// <returns></returns>
        public string ObtenerArchivoCentroServicio(PUArchivoCentroServicios archivo)
        {
            return PUCentroServicios.Instancia.ObtenerArchivoCentroServicio(archivo);
        }

        #region Suministros de centros de servicio

        /// <summary>
        /// Obtiene todos los suministros
        /// </summary>
        /// <returns></returns>
        public IList<PUSuministro> ObtenerTodosSuministros()
        {
            return PUCentroServicios.Instancia.ObtenerTodosSuministros();
        }

        /// <summary>
        /// Adiciona modifica o elimina un centro de servicios
        /// </summary>
        /// <param name="descuentoRef">Objeto centro de servicios</param>
        public void ActualizarCentroServiciosSuministros(PUCentroServiciosSuministro centroServiciosSuministro)
        {
            PUCentroServicios.Instancia.ActualizarCentroServiciosSuministros(centroServiciosSuministro);
        }

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
        public GenericoConsultasFramework<PUCentroServiciosSuministro> ObtenerSuministrosPorCentrosServicio(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, long IdCentroServicios)
        {
            int totalRegistros = 0;
            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<PUCentroServiciosSuministro>()
            {
                Lista = PUCentroServicios.Instancia.ObtenerSuministrosPorCentrosServicio(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, IdCentroServicios),
                TotalRegistros = totalRegistros
            };
        }

        #endregion Suministros de centros de servicio

        /// <summary>
        /// Envia la divulgacion de una agencia
        /// </summary>
        /// <param name="idCentroServicios"></param>
        /// <param name="divulgacion">Objeto con la informacion de los contactos a divulgar la agencia</param>
        public void DivulgarAgencia(long idCentroServicios, PADivulgacion divulgacion)
        {
            PUCentroServicios.Instancia.DivulgarAgencia(idCentroServicios, divulgacion);
        }

        /// <summary>
        /// Obtiene la Agencia responsable del Punto
        /// </summary>
        /// <param name="idPuntoServicio">el id punto servicio.</param>
        /// <returns></returns>
        public PUAgenciaDeRacolDC ObtenerAgenciaResponsable(long idPuntoServicio)
        {
            return PUCentroServicios.Instancia.ObtenerAgenciaResponsable(idPuntoServicio);
        }

        /// <summary>
        /// Obtiene el Racol responsable de la Agencia.
        /// </summary>
        /// <param name="idAgencia">el id agencia.</param>
        /// <returns></returns>
        public PUAgenciaDeRacolDC ObtenerRacolResponsable(long idAgencia)
        {
            return PUCentroServicios.Instancia.ObtenerRacolResponsable(idAgencia);
        }

        /// <summary>
        /// Metodo que consulta todas las agencias y puntos de un RACOL
        /// </summary>
        /// <param name="idRacol"></param>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerAgenciasYPuntosRacolActivos(long idRacol)
        {
            return PUAdministradorCentroServicios.Instancia.ObtenerAgenciasYPuntosRacolActivos(idRacol);
        }

        /// <summary>
        /// Metodo que consulta todas las agencias
        /// </summary>
        /// <param name="idRacol"></param>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerAgenciasRacolActivos(long idRacol)
        {
            return PUAdministradorCentroServicios.Instancia.ObtenerAgenciasRacolActivos(idRacol);
        }

        /// <summary>
        /// Retorna las agencias creadas en el sistemas que se encuentran activas
        /// </summary>
        /// <returns></returns>
        public List<PUAgencia> ObtenerAgenciasActivas()
        {
            return PUAdministradorCentroServicios.Instancia.ObtenerAgenciasActivas();
        }

        /// <summary>
        /// Obtiene los puntosde atencion
        /// de una agencia centro Servicio.
        /// </summary>
        /// <param name="idCentroServicio">The id centro servicio.</param>
        /// <returns>Lista de Puntos de Una Agencia</returns>
        public List<PUCentroServiciosDC> ObtenerPuntosDeAgencia(long idCentroServicio)
        {
            return PUAdministradorCentroServicios.Instancia.ObtenerPuntosDeAgenciaActivos(idCentroServicio);
        }

        /// <summary>
        /// Metodo para consultar las localidades donde existen centros logisticos
        /// </summary>
        /// <returns></returns>
        public IList<LILocalidadColDC> ObtenerLocalidadesCol()
        {
            return PUAdministradorCentroServicios.Instancia.ObtenerLocalidadesCol();
        }

        /// <summary>
        /// Metodo para consultar las agencias que dependen de un COL
        /// </summary>
        /// <returns></returns>
        public IList<LILocalidadColDC> ObtenerAgenciasCol(long idCol)
        {
            return PUAdministradorCentroServicios.Instancia.ObtenerAgenciasCol(idCol);
        }

        /// <summary>
        /// Obtiene los datos básicos de los centros de servivios de giros
        /// </summary>
        /// <returns>Colección centros de servicio</returns>
        public IEnumerable<PUCentroServiciosDC> ObtenerCentrosServicioGiros()
        {
            return PUAdministradorCentroServicios.Instancia.ObtenerCentrosServicioGiros();
        }

        /// <summary>
        /// Obtiene los puntos del centro logistico
        /// </summary>
        /// <param name="idCol"></param>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerPuntosServiciosCol(long idCol)
        {
            return PUAdministradorCentroServicios.Instancia.ObtenerPuntosServiciosCol(idCol);
        }

        /// <summary>
        /// Obtiene el id y la descripcion de todos los centros logisticos activos y racol activos
        /// </summary>
        /// <returns>lista de centros logisticos y racol </returns>
        public IList<PUCentroServicioApoyo> ObtenerCentrosServicioApoyo()
        {
            return PUAdministradorCentroServicios.Instancia.ObtenerCentrosServicioApoyo();
        }

        /// <summary>
        /// Metodo para Obtener el RACOL
        /// </summary>
        /// <returns></returns>
        public List<PURegionalAdministrativa> ObtenerRegionalAdministrativa()
        {
            return PUAdministradorCentroServicios.Instancia.ObtenerRegionalAdministrativa();
        }

        /// <summary>
        /// Metodo para Obtener la RACOL de un municipio
        /// </summary>
        /// <returns></returns>
        public PURegionalAdministrativa ObtenerRegionalAdministrativaPorMunicipio(string idMunicipio)
        {
            return PUAdministradorCentroServicios.Instancia.ObtenerRegionalAdministrativa(idMunicipio);
        }

        /// <summary>
        /// Consulta los municipios  de su centro logístico asociado
        /// </summary>
        /// <param name="IdDepartamento">Id del COL por el cual se quiere filtrar</param>
        /// <returns></returns>
        public List<PALocalidadDC> ObtenerMunicipiosXCol(long IdCol)
        {
            return PUAdministradorCentroServicios.Instancia.ObtenerMunicipiosXCol(IdCol);
        }

        /// <summary>
        /// Consulta los municipios y su respectivo centro logístico asociado
        /// </summary>
        /// <param name="IdDepartamento">Id del departamento por el cual se quiere filtrar</param>
        /// <returns></returns>
        public List<PUMunicipioCentroLogisticoDC> ConsultarMunicipiosCol(string IdDepartamento)
        {
            return PUAdministradorCentroServicios.Instancia.ConsultarMunicipiosCol(IdDepartamento);
        }

        /// <summary>
        /// Guarda en la base de datos el municipio con su respectivo centro logistico de apoyo
        /// </summary>
        /// <param name="municipioCol"></param>
        public void GuardarMunicipioCol(PUMunicipioCentroLogisticoDC municipioCol)
        {
            PUAdministradorCentroServicios.Instancia.GuardarMunicipioCol(municipioCol);
        }

        /// <summary>
        /// Consultar todos los centros de servicios de un RACOL y todos los RACOL activos
        /// </summary>
        /// <param name="idRacol">Identificador del RACOL</param>
        /// <returns>Colección de los centros de servicios de un RACOL y todos los RACOL activos</returns>
        public IList<PUCentroServiciosDC> ObtenerCentrosServiciosDeRacolYTodosRacol(long idRacol)
        {
            return PUAdministradorCentroServicios.Instancia.ObtenerObtenerCentrosServiciosDeRacolYTodosRacol(idRacol);
        }

        /// <summary>
        /// Adiciona el centro de servicios al que se le reporta
        /// </summary>
        /// <param name="idCentroServicioAQuienReporta"></param>
        /// <param name="idCentroServicioReporta"></param>
        public void AdicionarCentroServicioReporte(long idCentroServicioAQuienReporta, long idCentroServicioReporta)
        {
            PUAdministradorCentroServicios.Instancia.AdicionarCentroServicioReporte(idCentroServicioAQuienReporta, idCentroServicioReporta);
        }

        /// <summary>
        /// Eliminar el centro de servicios al que se le reporta
        /// </summary>
        /// <param name="idCentroServicioAQuienReporta"></param>
        /// <param name="idCentroServicioReporta"></param>
        public void EliminarCentroServicioReporte(long idCentroServicioAQuienReporta, long idCentroServicioReporta)
        {
            PUAdministradorCentroServicios.Instancia.EliminarCentroServicioReporte(idCentroServicioAQuienReporta, idCentroServicioReporta);
        }

        /// <summary>
        /// Obtiene el horario de la recogida de un centro de Servicio
        /// </summary>
        /// <param name="idCentroSvc">es le id del centro svc</param>
        /// <returns>info de la recogida</returns>
        public IList<PUHorarioRecogidaCentroSvcDC> ObtenerHorariosRecogidasCentroSvc(long idCentroSvc)
        {
            return PUAdministradorCentroServicios.Instancia.ObtenerHorariosRecogidasCentroSvc(idCentroSvc);
        }

        /// <summary>
        /// obtiene todos los centros servicio
        /// </summary>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerCentrosServicio()
        {
            return PUAdministradorCentroServicios.Instancia.ObtenerCentrosServicio();
        }

        public List<PUCentroServiciosDC> ObtenerCentrosServicioTipo()
        {
            return PUAdministradorCentroServicios.Instancia.ObtenerCentrosServicioTipo();
        }
        /// <summary>
        /// Adiciona los Horarios de las recogidas
        /// de los centros de Svc
        /// </summary>
        /// <param name="centroServicios">info del Centro de Servicio</param>
        public void AdicionarHorariosRecogidasCentroSvc(PUCentroServiciosDC centroServicios)
        {
            PUAdministradorCentroServicios.Instancia.AdicionarHorariosRecogidasCentroSvc(centroServicios);
        }

        /// <summary>
        /// Obtiene el col responsable de la agencia de una localidad
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <returns></returns>
        public long ObtieneIdCOLResponsableAgenciaLocalidad(string idLocalidad)
        {
            return PUAdministradorCentroServicios.Instancia.ObtieneIdCOLResponsableAgenciaLocalidad(idLocalidad);
        }

        /// <summary>
        /// Obtiene todas las agencias y puntos en el sistema
        /// </summary>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerTodosAgenciayPuntosActivos()
        {
            return PUAdministradorCentroServicios.Instancia.ObtenerTodosAgenciayPuntosActivos();
        }

        /// <summary>
        /// Obtiene todas las agencias, col y puntos en el sistema
        /// </summary>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerTodosAgenciaColPuntosActivos()
        {
            return PUAdministradorCentroServicios.Instancia.ObtenerTodosAgenciaColPuntosActivos();

        }


        #endregion Centro de servicios

        #region Clasificador canal de venta

        /// <summary>
        /// Inserta Modifica o Elimina un registro de clasificador de canal de venta
        /// </summary>
        /// <param name="clasificadorCanalVenta"></param>
        public void ActualizarClasificarCanalVenta(PUClasificadorCanalVenta clasificadorCanalVenta)
        {
            PUAdministradorCentroServicios.Instancia.ActualizarClasificarCanalVenta(clasificadorCanalVenta);
        }

        /// <summary>
        /// Obtiene los clasificadores del canal de ventas
        /// </summary>
        public GenericoConsultasFramework<PUClasificadorCanalVenta> ObtenerClasificadorCanalVenta(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente)
        {
            int totalRegistros = 0;
            return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<PUClasificadorCanalVenta>()
            {
                Lista = PUAdministradorCentroServicios.Instancia.ObtenerClasificadorCanalVenta(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros),
                TotalRegistros = totalRegistros
            };
        }

        /// <summary>
        /// Selecciona todos los tipos de centros de servicio
        /// </summary>
        /// <returns></returns>
        public List<PUTipoCentroServicio> ObtenerTodosTipoCentroServicio()
        {
            return PUAdministradorCentroServicios.Instancia.ObtenerTodosTipoCentroServicio();
        }

        #endregion Clasificador canal de venta

        #region Autorizacion Suministros

        /// <summary>
        /// Guardar los suministros que posee un centro de servicio
        /// </summary>
        /// <param name="suministroCentroServicio"></param>
        public void GuardarSuministroCentroServicio(SUSuministroCentroServicioDC suministroCentroServicio)
        {
            PUCentroServicios.Instancia.GuardarSuministroCentroServicio(suministroCentroServicio);
        }

        #endregion Autorizacion Suministros

        /// <summary>
        /// Obtienen todos los municipios de un racol
        /// </summary>
        /// <param name="idRacol">id Racol</param>
        /// <returns>municipios del racol</returns>
        public List<PALocalidadDC> ObtenerMunicipiosDeRacol(long idRacol)
        {
            return PUAdministradorCentroServicios.Instancia.ObtenerMunicipiosDeRacol(idRacol);
        }

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
        public GenericoConsultasFramework<PUMunicipiosSinAlCobro> ObtenerMunicipiosSinFormaPagoAlCobro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente)
        {
            int totalRegistros = 0;
            return new GenericoConsultasFramework<PUMunicipiosSinAlCobro>()
            {
                Lista = PUAdministradorCentroServicios.Instancia.ObtenerMunicipiosSinFormaPagoAlCobro(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros),
                TotalRegistros = totalRegistros
            };
        }

        #region Casa Matriz

        /// <summary>
        /// Obtener la información basica de las Regionales Administrativas activas de una Casa Matriz
        /// </summary>
        /// <param name="idCasaMatriz">Identificador de la Casa Matriz</param>
        /// <returns>Colección con la información básica de las regionales</returns>
        public IList<PURegionalAdministrativa> ObtenerRegionalesDeCasaMatriz(short idCasaMatriz)
        {
            return PUAdministradorCentroServicios.Instancia.ObtenerRegionalesDeCasaMatriz(idCasaMatriz);
        }

        #endregion Casa Matriz

        /// <summary>
        /// Agrega municipio a la lista de municipios que no admiten forma de pago "al cobro"
        /// </summary>
        /// <param name="municipio"></param>
        public void RegistrarMunicipioSinFormaPagoAlCobro(PALocalidadDC municipio)
        {
            PUAdministradorCentroServicios.Instancia.RegistrarMunicipioSinFormaPagoAlCobro(municipio);
        }

        /// <summary>
        /// Quita municipio de la lista de municipios que no admiten forma de pago "al cobro"
        /// </summary>
        /// <param name="municipio"></param>
        public void RemoverMunicipioSinFormaPagoAlCobro(PALocalidadDC municipio)
        {
            PUAdministradorCentroServicios.Instancia.RemoverMunicipioSinFormaPagoAlCobro(municipio);
        }

        public List<string> ObtenerDireccionPuntosSegunLocalidadDestinatario(int idLocalidadDestino)
        {
            return PUCentroServicios.Instancia.ObtenerDireccionPuntosSegunLocalidadDestinatario(idLocalidadDestino);
        }

        public List<PUCentroServicioApoyo> ObtenerPuntosREOSegunUbicacionDestino(int idLocalidadDestino)
        {
            return PUCentroServicios.Instancia.ObtenerPuntosREOSegunUbicacionDestino(idLocalidadDestino);
        }

        public List<PUCustodia> ObtenerGuiasCustodia(int idTipoMovimiento, Int16 idEstadoGuia, long? numeroGuia, bool muestraReportemuestraTodosreporte)
        {
            return PUCentroServicios.Instancia.ObtenerGuiasCustodia(idTipoMovimiento, idEstadoGuia, numeroGuia, muestraReportemuestraTodosreporte);
        }

        /// <summary>
        /// Obtiene los horarios de un centro de servicios para la app de recogidas
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public List<string> ObtenerHorariosCentroServicioAppRecogidas(long idCentroServicio)
        {
            return PUCentroServicios.Instancia.ObtenerHorariosCentroServicioAppRecogidas(idCentroServicio);
        }

        /// <summary>
        /// Método para obtener los puntos y agencias de un col
        /// </summary>
        /// <param name="idCol"></param>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerPuntosAgenciasColReclamaOficina(long idCol)
        {
            return PUAdministradorCentroServicios.Instancia.ObtenerPuntosAgenciasColReclamaOficina(idCol);
        }

        /// <summary>
        /// Obtiene toda la info basica de los centros de servicio
        /// </summary>
        /// <returns></returns>
        public List<PUCentroServicioInfoGeneral> ObtenerInformacionGeneralCentrosServicio()
        {
            return PUAdministradorCentroServicios.Instancia.ObtenerInformacionGeneralCentrosServicio();
        }

        /// <summary>
        /// Obtiene toda la info basica de los centros de servicio
        /// </summary>
        /// <returns></returns>
        public List<PUCentroServicioInfoGeneral> ObtenerPosicionesCanalesVenta(DateTime fechaInicial, DateTime fechaFinal, string idMensajero, string idCentroServicio, int idEstado)
        {
            return PUAdministradorCentroServicios.Instancia.ObtenerPosicionesCanalesVenta(fechaInicial, fechaFinal, idMensajero, idCentroServicio, idEstado);
        }

        /// <summary>
        /// Obtiene los servicios junto con sus horarios de venta de un centro de servicio
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public List<PUServicio> ObtenerServiciosCentroServicio(long idCentroServicio)
        {
            return PUAdministradorCentroServicios.Instancia.ObtenerServiciosCentroServicio(idCentroServicio);
        }

        /// <summary>
        /// Obtiene los servicios junto con sus horarios de venta de un centro de servicio
        /// </summary>
        /// <param name="idServicio"></param>
        /// <returns></returns>
        public List<PUServicio> ObtenerCentrosServicioPorServicio(int idServicio)
        {
            return PUAdministradorCentroServicios.Instancia.ObtenerCentrosServicioPorServicio(idServicio);
        }

        /// <summary>
        /// Metodo para obtener la bodega de custodia
        /// </summary>
        /// <returns></returns>
        public PUCentroServiciosDC ObtenerBodegaCustodia()
        {
            return PUAdministradorCentroServicios.Instancia.ObtenerBodegaCustodia();
        }



        /// <summary>
        /// Obtiene el centro de acopio respectivo a una bodega.
        /// </summary>
        /// <param name="idBodega"></param>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        public PUCentroServiciosDC ObtenerCentroDeAcopioBodega(long idBodega, long idUsuario)
        {
            return PUAdministradorCentroServicios.Instancia.ObtenerCentroDeAcopioBodega(idBodega, idUsuario);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="localidad"></param>
        /// <returns></returns>
        public PUCentroServiciosDC ObtenerCentroConfirmacionesDevoluciones(PALocalidadDC localidad)
        {
            return PUAdministradorCentroServicios.Instancia.ObtenerCentroConfirmacionesDevoluciones(localidad);
        }

        /// <summary>
        /// Metodo para adicionar movimiento inventario 
        /// </summary>
        /// <param name="movimientoInventario"></param>
        /// <returns></returns>
        public long AdicionarMovimientoInventario(PUMovimientoInventario movimientoInventario)
        {
            return PUAdministradorCentroServicios.Instancia.AdicionarMovimientoInventario(movimientoInventario);
        }


        /// <summary>
        /// Obtiene la lista de las Territoriales
        /// </summary>
        /// <returns></returns>
        public List<PUTerritorialDC> ObtenerTerritoriales()
        {
            return PUAdministradorCentroServicios.Instancia.ObtenerTerritoriales();
        }

        /// <summary>
        /// Obtiene los centros de servicio a los cuales tiene acceso el usuario
        /// </summary>
        /// <param name="identificacionUsuario"></param>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerLocacionesAutorizadas(string usuario)
        {
            return PUAdministradorCentroServicios.Instancia.ObtenerLocacionesAutorizadas(usuario);
        }
    }
}