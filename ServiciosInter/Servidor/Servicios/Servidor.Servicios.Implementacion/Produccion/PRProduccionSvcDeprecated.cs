using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using CO.Servidor.Produccion;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Produccion;
using CO.Servidor.Servicios.Contratos;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.Implementacion.Produccion
{
  /// <summary>
  /// Clase para los servicios de administración de Clientes
  /// </summary>
  [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
  public class PRProduccionSvcDeprecated : IPRProduccionSvcDeprecated
  {
    #region Liquidacion Manual

    /// <summary>
    /// Obtiene los valores para la liquidacion de un centro de servicios
    /// </summary>
    /// <param name="liquidacion"></param>
    /// <returns></returns>
    public PRLiquidacionManualDCDeprecated ObtenerObtenerValoresLiquidacionCentroSvc(PRLiquidacionManualDCDeprecated liquidacion)
    {
      return PRAdmProduccionDeprecated.Instancia.ObtenerValoresLiquidacionCentroSvc(liquidacion);
    }

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
    public GenericoConsultasFramework<PUCentroServiciosDC> ObtenerCentroServicios(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente)
    {
      int totalRegistros = 0;
      return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<PUCentroServiciosDC>()
      {
        Lista = PRAdmProduccionDeprecated.Instancia.ObtenerCentroServicios(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros),
        TotalRegistros = totalRegistros
      };
    }

    /// <summary>
    /// Guarda la liquidacion de produccion
    /// </summary>
    /// <param name="liquidacion"></param>
    public void GuardarLiquidacionCentroServicios(PRLiquidacionManualDCDeprecated liquidacion)
    {
      PRAdmProduccionDeprecated.Instancia.GuardarLiquidacionCentroServicios(liquidacion);
    }

    #endregion Liquidacion Manual

    #region AdminLiquidacion

    /// <summary>
    /// Obtiene las liquidaciones de las agencias/puntos
    /// </summary>
    /// <param name="filtro"></param>
    /// <param name="indicePagina"></param>
    /// <param name="registrosPorPagina"></param>
    /// <returns></returns>
    public List<PRLiquidacionManualDCDeprecated> ObtenerLiquidaciones(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina)
    {
      return PRAdmProduccionDeprecated.Instancia.ObtenerLiquidaciones(filtro, indicePagina, registrosPorPagina);
    }

    /// <summary>
    /// Obtiene el detalle de la liquidacion de la produccion
    /// </summary>
    /// <param name="liquidacion"></param>
    public PRLiquidacionManualDCDeprecated ObtenerDetalleLiquidacionProduccion(PRLiquidacionManualDCDeprecated liquidacion)
    {
      return PRAdmProduccionDeprecated.Instancia.ObtenerDetalleLiquidacionProduccion(liquidacion);
    }

    /// <summary>
    /// Realiza la aprobacion de la liquidacion de produccion
    /// </summary>
    /// <param name="liquidacion"></param>
    public void AprobarLiquidacionProduccion(PRLiquidacionManualDCDeprecated liquidacion)
    {
      PRAdmProduccionDeprecated.Instancia.AprobarLiquidacionProduccion(liquidacion);
    }

    /// <summary>
    /// Anula la liquidacion de produccion
    /// </summary>
    /// <param name="liquidacion"></param>
    public void AnularLiquidacionProduccion(long liquidacion)
    {
      PRAdmProduccionDeprecated.Instancia.AnularLiquidacionProduccion(liquidacion);
    }

    /// <summary>
    /// Obtiene la programacion de las liquidaciones realizadas
    /// </summary>
    /// <param name="filtro"></param>
    /// <param name="campoOrdenamiento"></param>
    /// <param name="indicePagina"></param>
    /// <param name="registrosPorPagina"></param>
    /// <param name="ordenamientoAscendente"></param>
    /// <returns></returns>
    public GenericoConsultasFramework<PRProgramacionLiquidacionDCDeprecated> ObtenerProgramacionLiquidaciones(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente)
    {
      int totalRegistros = 0;
      return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<PRProgramacionLiquidacionDCDeprecated>()
      {
        Lista = PRAdmProduccionDeprecated.Instancia.ObtenerProgramacionLiquidaciones(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros),
        TotalRegistros = totalRegistros
      };
    }

    /// <summary>
    /// Obtener centros de servicios y racol
    /// </summary>
    /// <returns></returns>
    public List<PUAgenciaDeRacolDC> ObtenerCentroServiciosRacol()
    {
      return PRAdmProduccionDeprecated.Instancia.ObtenerCentroServiciosRacol();
    }

    /// <summary>
    /// Obtiene los centros de servicios sin programar
    /// </summary>
    /// <param name="idRacol"></param>
    /// <param name="idCentroServicios"></param>
    /// <returns></returns>
    public List<PUAgenciaDeRacolDC> ObtenerCentroServiciosSinPromagramar(long? idRacol, long? idCentroServicios)
    {
      return PRAdmProduccionDeprecated.Instancia.ObtenerCentroServiciosSinPromagramar(idRacol.Value, idCentroServicios.Value);
    }

    /// <summary>
    /// Guarda la programacion de la liquidacion
    /// </summary>
    /// <param name="programacion"></param>
    public void GuardarProgramacionLiquidacion(PRProgramacionLiquidacionDCDeprecated programacion)
    {
      PRAdmProduccionDeprecated.Instancia.GuardarProgramacionLiquidacion(programacion);
    }

    #endregion AdminLiquidacion

    #region Imprimir

    /// <summary>
    /// Obtiene las liquidaciones de las agencias/puntos
    /// </summary>
    /// <param name="filtro"></param>
    /// <param name="indicePagina"></param>
    /// <param name="registrosPorPagina"></param>
    /// <returns></returns>
    public List<PRLiquidacionManualDCDeprecated> ObtenerLiquidacionesAprobadas(IDictionary<string, string> filtro)
    {
      return PRAdmProduccionDeprecated.Instancia.ObtenerLiquidacionesAprobadas(filtro);
    }

    /// <summary>
    /// Obtener centros logisticos activos
    /// </summary>
    /// <returns></returns>
    public List<PUCentroServicioApoyo> ObtenerCentroLogistico()
    {
      return PRAdmProduccionDeprecated.Instancia.ObtenerCentroLogistico();
    }

    #endregion Imprimir

    #region Novedades

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
    public GenericoConsultasFramework<PANovedadCentroServicioDCDeprecated> ObtenerNovedadesCentroSvc(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, long idCentroServicio)
    {
      int totalRegistros = 0;
      return new Framework.Servidor.Servicios.ContratoDatos.GenericoConsultasFramework<PANovedadCentroServicioDCDeprecated>()
      {
        Lista = PRAdmProduccionDeprecated.Instancia.ObtenerNovedadesCentroSvc(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idCentroServicio),
        TotalRegistros = totalRegistros
      };
    }

    /// <summary>
    /// Retorna los tipos de novedad
    /// </summary>
    /// <returns></returns>
    public List<PRTipoNovedadDCDeprecated> ObtenerTiposNovedad()
    {
      return PRAdmProduccionDeprecated.Instancia.ObtenerTiposNovedad();
    }

    /// <summary>
    /// Retorna los tipos de novedad
    /// </summary>
    /// <returns></returns>
    public List<PRMotivoNovedadDCDeprecated> ObtenerMotivoNovedadTipo(int idTipoNovedad)
    {
      return PRAdmProduccionDeprecated.Instancia.ObtenerMotivoNovedadTipo(idTipoNovedad);
    }

    /// <summary>
    /// Adicionar novedad de Cambio de destiono al Centro Servicio
    /// </summary>
    /// <param name="novedad"></param>
    public void GuardarNovedadCentroServicio(PANovedadCentroServicioDCDeprecated novedad)
    {
      PRAdmProduccionDeprecated.Instancia.GuardarNovedadCentroServicio(novedad);
    }

    #endregion Novedades
  }
}