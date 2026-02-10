using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Produccion;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos;
using System.Collections.Generic;
using System.ServiceModel;

namespace CO.Servidor.Servicios.Contratos
{
    /// <summary>
    /// Contratos WCF de centros de servicios
    /// </summary>
    [ServiceContract(Namespace = "http://contrologis.com")]
  public interface IPRProduccionSvcDeprecated
  {
    /// <summary>
    /// Obtiene los valores para la liquidacion de un centro de servicios
    /// </summary>
    /// <param name="liquidacion"></param>
    /// <returns></returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    PRLiquidacionManualDCDeprecated ObtenerObtenerValoresLiquidacionCentroSvc(PRLiquidacionManualDCDeprecated liquidacion);

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
    GenericoConsultasFramework<PUCentroServiciosDC> ObtenerCentroServicios(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente);

    /// <summary>
    /// Guarda la liquidacion de produccion
    /// </summary>
    /// <param name="liquidacion"></param>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    void GuardarLiquidacionCentroServicios(PRLiquidacionManualDCDeprecated liquidacion);

    /// <summary>
    /// Obtiene las liquidaciones de las agencias/puntos
    /// </summary>
    /// <param name="filtro"></param>
    /// <param name="indicePagina"></param>
    /// <param name="registrosPorPagina"></param>
    /// <returns></returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    List<PRLiquidacionManualDCDeprecated> ObtenerLiquidaciones(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina);

    /// <summary>
    /// Obtiene el detalle de la liquidacion de la produccion
    /// </summary>
    /// <param name="liquidacion"></param>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    PRLiquidacionManualDCDeprecated ObtenerDetalleLiquidacionProduccion(PRLiquidacionManualDCDeprecated liquidacion);

    /// <summary>
    /// Realiza la aprobacion de la liquidacion de produccion
    /// </summary>
    /// <param name="liquidacion"></param>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    void AprobarLiquidacionProduccion(PRLiquidacionManualDCDeprecated liquidacion);

    /// <summary>
    /// Anula la liquidacion de produccion
    /// </summary>
    /// <param name="liquidacion"></param>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    void AnularLiquidacionProduccion(long liquidacion);

    /// <summary>
    /// Obtiene la programacion de las liquidaciones realizadas
    /// </summary>
    /// <param name="filtro"></param>
    /// <param name="campoOrdenamiento"></param>
    /// <param name="indicePagina"></param>
    /// <param name="registrosPorPagina"></param>
    /// <param name="ordenamientoAscendente"></param>
    /// <returns></returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    GenericoConsultasFramework<PRProgramacionLiquidacionDCDeprecated> ObtenerProgramacionLiquidaciones(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente);

    /// <summary>
    /// Obtener centros de servicios y racol
    /// </summary>
    /// <returns></returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    List<PUAgenciaDeRacolDC> ObtenerCentroServiciosRacol();

    /// <summary>
    /// Obtiene los centros de servicios sin programar
    /// </summary>
    /// <param name="idRacol"></param>
    /// <param name="idCentroServicios"></param>
    /// <returns></returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    List<PUAgenciaDeRacolDC> ObtenerCentroServiciosSinPromagramar(long? idRacol, long? idCentroServicios);

    /// <summary>
    /// Guarda la programacion de la liquidacion
    /// </summary>
    /// <param name="programacion"></param>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    void GuardarProgramacionLiquidacion(PRProgramacionLiquidacionDCDeprecated programacion);

    /// <summary>
    /// Obtiene las liquidaciones de las agencias/puntos
    /// </summary>
    /// <param name="filtro"></param>
    /// <param name="indicePagina"></param>
    /// <param name="registrosPorPagina"></param>
    /// <returns></returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    List<PRLiquidacionManualDCDeprecated> ObtenerLiquidacionesAprobadas(IDictionary<string, string> filtro);

    /// <summary>
    /// Obtener centros logisticos activos
    /// </summary>
    /// <returns></returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    List<PUCentroServicioApoyo> ObtenerCentroLogistico();

    /// <summary>
    /// Retona las novedades de los centros de servicios
    /// </summary>
    /// <param name="filtro"></param>
    /// <param name="campoOrdenamiento"></param>
    /// <param name="indicePagina"></param>
    /// <param name="registrosPorPagina"></param>
    /// <param name="ordenamientoAscendente"></param>
    /// <param name="totalRegistros"></param>
    /// <param name="idCentroServicio"></param>
    /// <returns></returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    GenericoConsultasFramework<PANovedadCentroServicioDCDeprecated> ObtenerNovedadesCentroSvc(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, long idCentroServicio);

    /// <summary>
    /// Retorna los tipos de novedad
    /// </summary>
    /// <returns></returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    List<PRTipoNovedadDCDeprecated> ObtenerTiposNovedad();

    /// <summary>
    /// Retorna los tipos de novedad
    /// </summary>
    /// <returns></returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    List<PRMotivoNovedadDCDeprecated> ObtenerMotivoNovedadTipo(int idTipoNovedad);

    /// <summary>
    /// Adicionar novedad de Cambio de destiono al Centro Servicio
    /// </summary>
    /// <param name="novedad"></param>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    void GuardarNovedadCentroServicio(PANovedadCentroServicioDCDeprecated novedad);
  }
}