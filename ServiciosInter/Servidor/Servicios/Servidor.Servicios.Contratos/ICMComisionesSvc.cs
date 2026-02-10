using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Comisiones;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos;
using System.Collections.Generic;
using System.ServiceModel;

namespace CO.Servidor.Servicios.Contratos
{
    [ServiceContract(Namespace = "http://contrologis.com")]
  public interface ICMComisionesSvc
  {
    #region Comisiones por centros de servicio administrados

    /// <summary>
    /// Obtiene los puntos de una agencia
    /// </summary>
    /// <param name="filtro">Diccionario con los parametros del filtro</param>
    /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
    /// <param name="indicePagina">Indice de la pagina</param>
    /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
    /// <param name="ordenamientoAscendente">Ordenamiento</param>
    /// <param name="IdCentroServicios">Id del centro de servicio</param>
    /// <returns>Lista puntos de la agencia</returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    GenericoConsultasFramework<CMCentroServicioAdministrado> ObtenerPuntosDeAgencia(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, long IdAgencia);

    /// <summary>
    /// Obtiene las agencias de un col
    /// </summary>
    /// <param name="filtro">Diccionario con los parametros del filtro</param>
    /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
    /// <param name="indicePagina">Indice de la pagina</param>
    /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
    /// <param name="ordenamientoAscendente">Ordenamiento</param>
    /// <param name="IdCentroServicios">Id del centro de servicio</param>
    /// <returns>Lista agencias de un col</returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    GenericoConsultasFramework<CMCentroServicioAdministrado> ObtenerAgenciasDeCol(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, long IdCol);

    /// <summary>
    /// Obtiene los col de un racol
    /// </summary>
    /// <param name="filtro">Diccionario con los parametros del filtro</param>
    /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
    /// <param name="indicePagina">Indice de la pagina</param>
    /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
    /// <param name="ordenamientoAscendente">Ordenamiento</param>
    /// <param name="IdCentroServicios">Id del centro de servicio</param>
    /// <returns>Lista col de un racol</returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    GenericoConsultasFramework<CMCentroServicioAdministrado> ObtenerColDeRacol(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, long IdRacol);

    /// <summary>
    /// Obtiene  las comisiones de los servicios asignados a un centro de servicios administrado
    /// </summary>
    /// <param name="filtro">Diccionario con los parametros del filtro</param>
    /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
    /// <param name="indicePagina">Indice de la pagina</param>
    /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
    /// <param name="ordenamientoAscendente">Ordenamiento</param>
    /// <param name="totalRegistros">Total de registros de la consult</param>
    /// <param name="IdCentroServicios">Id del centro de servicio</param>
    /// <returns>Lista  de comisiones por servicios de los centros de servicio</returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    GenericoConsultasFramework<CMComisionesServiciosCentroServiciosAdmin> ObtenerComisionesServiciosCentroServiciosAdmin(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, long IdCentroServicios);

    /// <summary>
    /// Obtiene todos los tipos de comision
    /// </summary>
    /// <returns>Lista con los tipos de comision</returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    IList<PUTiposComision> ObtenerTiposComision();

    /// <summary>
    /// Guarda o edita comisiones de los servicios por un centro de servicios
    /// </summary>
    /// <param name="comisionesServicios"></param>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    void ActualizarComisionesServiciosAsignadosCentrosServiciosAdmin(CMComisionesServiciosCentroServiciosAdmin comisionesServicios);

    /// <summary>
    /// Obtiene todos los tipos de comision fija
    /// </summary>
    /// <returns>Lista con los tipos de comision fija</returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    IList<PUTipoComisionFija> ObtenerTiposComisionFija();

    /// <summary>
    /// Obtiene  los conceptos adicionales (comisiones fijas) de un centro de servicio
    /// </summary>
    /// <param name="filtro">Diccionario con los parametros del filtro</param>
    /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
    /// <param name="indicePagina">Indice de la pagina</param>
    /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
    /// <param name="ordenamientoAscendente">Ordenamiento</param>
    /// <param name="totalRegistros">Total de registros de la consult</param>
    /// <param name="IdCentroServicios">Id del centro de servicio</param>
    /// <returns>Lista  de los conceptos adicionales (comisiones fijas) de un centro de servicio</returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    GenericoConsultasFramework<CMComisionesConceptosAdicionales> ObtenerComisionesConceptosAdicionalesCentroServicio(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, long IdCentroServicios);

    /// <summary>
    /// Inserta, Modifica o Elimina una comision por concepto adicional (comision fija) a un centro de servicio sin un contrato
    /// </summary>
    /// <param name="comisionFija"></param>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    void ActualizarComisionesConceptosAdicionalesCentroServicio(CMComisionesConceptosAdicionales comisionFija);

    #endregion Comisiones por centros de servicio administrados

    /// <summary>
    /// Adiciona modifica o elimina un servicios a un centro de servicios junto con los descuentos, comisiones y horarios
    /// </summary>
    /// <param name="servicios">Objeto con la informacion de los servicios</param>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    void ActualizarServiciosCentroServicios(CMServiciosCentroServicios servicios);

    /// <summary>
    /// Obtiene  los servicios asignados a un centro de servicios junto con sus comisiones y sus descuentos
    /// </summary>
    /// <param name="filtro">Diccionario con los parametros del filtro</param>
    /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
    /// <param name="indicePagina">Indice de la pagina</param>
    /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
    /// <param name="ordenamientoAscendente">Ordenamiento</param>
    /// <param name="totalRegistros">Total de registros de la consult</param>
    /// <param name="IdCentroServicios">Id del centro de servicio</param>
    /// <returns>Lista  de comisiones por servicios de los centros de servicio</returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    GenericoConsultasFramework<CMServiciosCentroServicios> ObtenerServiciosCentroServicios(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, long IdCentroServicios);

    /// <summary>
    /// Obtiene  las comisiones y sus descuentos de un servicio asignado a un centro de servicios
    /// </summary>
    /// <param name="filtro">Diccionario con los parametros del filtro</param>
    /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
    /// <param name="indicePagina">Indice de la pagina</param>
    /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
    /// <param name="ordenamientoAscendente">Ordenamiento</param>
    /// <param name="totalRegistros">Total de registros de la consult</param>
    /// <param name="IdCentroServiciosServicios">Id del servicio asignado a un  centro de servicio</param>
    /// <returns>Objeto servicioCentroServicios con la lista de comisiones y descuentos</returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    CMServiciosCentroServicios ObtenerComiDescServiciosCentroServicios(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long IdCentroServiciosServicios, int idServicio);

    #region Servicios de Centro servicios

    #endregion Servicios de Centro servicios
  }
}