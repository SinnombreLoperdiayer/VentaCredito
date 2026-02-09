using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Produccion.Datos;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Produccion;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;

namespace CO.Servidor.Produccion
{
  public class PRAdmProduccionDeprecated : ControllerBase
  {
    #region creacion Instancias

    private static readonly PRAdmProduccionDeprecated instancia = (PRAdmProduccionDeprecated)FabricaInterceptores.GetProxy(new PRAdmProduccionDeprecated(), COConstantesModulos.MODULO_PRODUCCION);

    /// <summary>
    /// Retorna una instancia de administracion de produccion
    /// /// </summary>
    public static PRAdmProduccionDeprecated Instancia
    {
      get { return PRAdmProduccionDeprecated.instancia; }
    }

    #endregion creacion Instancias

    #region Metodos

    /// <summary>
    /// Adicionar novedad de Forma de Pago al Centro Servicio
    /// </summary>
    /// <param name="novedad"></param>
    public void AdicionarNovedadCentroServicioFormaPago(PANovedadCentroServicioDCDeprecated novedad)
    {
      PRProduccionDeprecated.Instancia.AdicionarNovedadCentroServicioFormaPago(novedad);
    }

    /// <summary>
    /// Adicionar novedad de Cambio de destiono al Centro Servicio
    /// </summary>
    /// <param name="novedad"></param>
    public void AdicionarNovedadCentroServicioCambioDestino(PANovedadCentroServicioDCDeprecated novedad)
    {
      PRProduccionDeprecated.Instancia.AdicionarNovedadCentroServicioCambioDestino(novedad);
    }

    #region Liquidacion Manual

    /// <summary>
    /// Obtiene los valores para la liquidacion de un centro de servicios
    /// </summary>
    /// <param name="liquidacion"></param>
    /// <returns></returns>
    public PRLiquidacionManualDCDeprecated ObtenerValoresLiquidacionCentroSvc(PRLiquidacionManualDCDeprecated liquidacion)
    {
      return PRLiquidacionManualDeprecated.Instancia.ObtenerValoresLiquidacionCentroSvc(liquidacion);
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
    public IList<PUCentroServiciosDC> ObtenerCentroServicios(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
    {
      return PRLiquidacionManualDeprecated.Instancia.ObtenerCentroServicios(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
    }

    /// <summary>
    /// Guarda la liquidacion de produccion
    /// </summary>
    /// <param name="liquidacion"></param>
    public void GuardarLiquidacionCentroServicios(PRLiquidacionManualDCDeprecated liquidacion)
    {
      PRLiquidacionManualDeprecated.Instancia.GuardarLiquidacionCentroServicios(liquidacion);
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
      return PRLiquidacionManualDeprecated.Instancia.ObtenerLiquidaciones(filtro, indicePagina, registrosPorPagina);
    }

    /// <summary>
    /// Obtiene el detalle de la liquidacion de la produccion
    /// </summary>
    /// <param name="liquidacion"></param>
    public PRLiquidacionManualDCDeprecated ObtenerDetalleLiquidacionProduccion(PRLiquidacionManualDCDeprecated liquidacion)
    {
      return PRLiquidacionManualDeprecated.Instancia.ObtenerDetalleLiquidacionProduccion(liquidacion);
    }

    /// <summary>
    /// Realiza la aprobacion de la liquidacion de produccion
    /// </summary>
    /// <param name="liquidacion"></param>
    public void AprobarLiquidacionProduccion(PRLiquidacionManualDCDeprecated liquidacion)
    {
      PRProduccionDeprecated.Instancia.AprobarLiquidacionProduccion(liquidacion);
    }

    /// <summary>
    /// Anula la liquidacion de produccion
    /// </summary>
    /// <param name="liquidacion"></param>
    public void AnularLiquidacionProduccion(long liquidacion)
    {
      PRProduccionDeprecated.Instancia.AnularLiquidacionProduccion(liquidacion);
    }

    #endregion AdminLiquidacion

    #region Programacion

    /// <summary>
    /// Obtiene la programacion de las liquidaciones
    /// </summary>
    public List<PRProgramacionLiquidacionDCDeprecated> ObtenerProgramacionLiquidaciones(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
    {
      return PRProduccionDeprecated.Instancia.ObtenerProgramacionLiquidaciones(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
    }

    /// <summary>
    /// Obtener centros de servicios y racol
    /// </summary>
    /// <returns></returns>
    public List<PUAgenciaDeRacolDC> ObtenerCentroServiciosRacol()
    {
      return PRProduccionDeprecated.Instancia.ObtenerCentroServiciosRacol();
    }

    /// <summary>
    /// Obtener centros logisticos activos
    /// </summary>
    /// <returns></returns>
    public List<PUCentroServicioApoyo> ObtenerCentroLogistico()
    {
      return PRProduccionDeprecated.Instancia.ObtenerCentroLogistico();
    }

    /// <summary>
    /// Obtiene los centros de servicios sin programar
    /// </summary>
    /// <param name="idRacol"></param>
    /// <param name="idCentroServicios"></param>
    /// <returns></returns>
    public List<PUAgenciaDeRacolDC> ObtenerCentroServiciosSinPromagramar(long idRacol, long idCentroServicios)
    {
      return PRProduccionDeprecated.Instancia.ObtenerCentroServiciosSinPromagramar(idRacol, idCentroServicios);
    }

    /// <summary>
    /// Guarda la programacion de la liquidacion
    /// </summary>
    /// <param name="programacion"></param>
    public void GuardarProgramacionLiquidacion(PRProgramacionLiquidacionDCDeprecated programacion)
    {
      if (programacion.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
        PRProduccionDeprecated.Instancia.GuardarProgramacionLiquidacion(programacion);
      else if (programacion.EstadoRegistro == EnumEstadoRegistro.MODIFICADO)
        PRProduccionDeprecated.Instancia.ModificarProgramacion(programacion);
    }

    #endregion Programacion

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
      return PRProduccionDeprecated.Instancia.ObtenerLiquidacionesAprobadas(filtro);
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
    public List<PANovedadCentroServicioDCDeprecated> ObtenerNovedadesCentroSvc(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long idCentroServicio)
    {
      return PRProduccionDeprecated.Instancia.ObtenerNovedadesCentroSvc(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idCentroServicio);
    }

    /// <summary>
    /// Retorna los tipos de novedad
    /// </summary>
    /// <returns></returns>
    public List<PRTipoNovedadDCDeprecated> ObtenerTiposNovedad()
    {
      return PRProduccionDeprecated.Instancia.ObtenerTiposNovedad();
    }

    /// <summary>
    /// Retorna los tipos de novedad
    /// </summary>
    /// <returns></returns>
    public List<PRMotivoNovedadDCDeprecated> ObtenerMotivoNovedadTipo(int idTipoNovedad)
    {
      return PRProduccionDeprecated.Instancia.ObtenerMotivoNovedadTipo(idTipoNovedad);
    }

    /// <summary>
    /// Adicionar novedad de Cambio de destiono al Centro Servicio
    /// </summary>
    /// <param name="novedad"></param>
    public void GuardarNovedadCentroServicio(PANovedadCentroServicioDCDeprecated novedad)
    {
      PRProduccionDeprecated.Instancia.GuardarNovedadCentroServicio(novedad);
    }

    /// <summary>
    /// Adiciona la Novedad del Centro de Servicio
    /// </summary>
    /// <param name="novedad">Data de la Novedad</param>
    public void AdicionarNovedadCentroServicio(PANovedadCentroServicioDCDeprecated novedad)
    {
      PRProduccionDeprecated.Instancia.AdicionarNovedadCentroServicio(novedad);
    }

    #endregion Novedades

    #endregion Metodos
  }
}