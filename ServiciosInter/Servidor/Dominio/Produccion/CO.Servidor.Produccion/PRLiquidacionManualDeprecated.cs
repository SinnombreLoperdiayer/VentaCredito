using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using CO.Servidor.Dominio.Comun.Cajas;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Comisiones;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Produccion.Comun;
using CO.Servidor.Produccion.Datos;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Comisiones;
using CO.Servidor.Servicios.ContratoDatos.Produccion;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW;

namespace CO.Servidor.Produccion
{
  public class PRLiquidacionManualDeprecated : ControllerBase
  {
    #region creacion Instancias

    private static readonly PRLiquidacionManualDeprecated instancia = (PRLiquidacionManualDeprecated)FabricaInterceptores.GetProxy(new PRLiquidacionManualDeprecated(), COConstantesModulos.MODULO_PRODUCCION);

    /// <summary>
    /// Retorna una instancia de administracion de produccion
    /// /// </summary>
    public static PRLiquidacionManualDeprecated Instancia
    {
      get { return PRLiquidacionManualDeprecated.instancia; }
    }

    #endregion creacion Instancias

    #region Métodos

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
      return COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerCentrosServicioAgenciasPuntos(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
    }

    /// <summary>
    /// Obtiene los valores para la liquidacion de un centro de servicio
    /// </summary>
    /// <param name="idCentroServicios"></param>
    public PRLiquidacionManualDCDeprecated ObtenerValoresLiquidacionCentroSvc(PRLiquidacionManualDCDeprecated liquidacion)
    {
      ///Obtiene las comisiones por concepto
      liquidacion.ComisionXConcepto = PRRepositorioDeprecated.Instancia.ObtenerComisionXConcepto(liquidacion.IdCentroServicios, liquidacion.FechaCorte.AddDays(1));
      ///Obtiene las comisiones por punto
      liquidacion.ComisionesPuntos = PRRepositorioDeprecated.Instancia.ObtenerComisionXPuntosACargo(liquidacion.IdCentroServicios, liquidacion.FechaCorte.AddDays(1));
      ///Obtiene las novedades por reintegro
      liquidacion.NovedadReintegro = PRRepositorioDeprecated.Instancia.ObtenerDetalleNovedad(liquidacion.IdCentroServicios, liquidacion.FechaCorte.AddDays(1), PRConstantesProduccion.ID_TIPO_NOVEDAD_REINTEGRO);
      ///Obtiene las novedades por descuentos
      liquidacion.NovedadDescuento = PRRepositorioDeprecated.Instancia.ObtenerDetalleNovedad(liquidacion.IdCentroServicios, liquidacion.FechaCorte.AddDays(1), PRConstantesProduccion.ID_TIPO_NOVEDAD_DESCUENTO);
      ///Obtiene la liquidacion de la comision fija
      liquidacion.ComisionesFijas = ObtenerLiquidacionComisionFija(liquidacion);

      ///Obtiene las retenciones por concepto
      liquidacion.RetencionesConcepto = PRRepositorioDeprecated.Instancia.ObtenerRetencionConcepto(liquidacion.IdCentroServicios, liquidacion.FechaCorte.AddDays(1));
      ///Obtiene las retenciones de las novedades por reintegro
      liquidacion.RetencionesNovedad = PRRepositorioDeprecated.Instancia.ObtenerRetencionNovedadReintegro(liquidacion.IdCentroServicios, liquidacion.FechaCorte.AddDays(1));

      ///Obtiene el saldo en caja del centro de servicios
      liquidacion.SaldoCaja = ObtenerSaldoCaja(liquidacion.IdCentroServicios);

      return liquidacion;
    }

    /// <summary>
    /// Obtiene el saldo en caja del centro de servicios
    /// </summary>
    /// <param name="idCentroServicios"></param>
    /// <returns></returns>
    public decimal ObtenerSaldoCaja(long idCentroServicios)
    {
      return COFabricaDominio.Instancia.CrearInstancia<ICAFachadaCajas>()
        .ObtenerSaldoActualCaja(idCentroServicios);
    }

    /// <summary>
    /// Obtiene la liquidacion por comision fija
    /// </summary>
    /// <param name="liquidacion"></param>
    public List<CMComisionesConceptosAdicionales> ObtenerLiquidacionComisionFija(PRLiquidacionManualDCDeprecated liquidacion)
    {
      PRLiquidacionManualDCDeprecated ultimaLiquidacion = new PRLiquidacionManualDCDeprecated();
      string diasHabilesMes;
      int diasMes;
      decimal valorDia;

      ///Obtiene la ultima liquidacion aprobada para consultar la fecha de la liquidacion
      ultimaLiquidacion = PRRepositorioDeprecated.Instancia.ObtenerUltimaLiquidacionAprobada(liquidacion.IdCentroServicios);

      diasHabilesMes = PRRepositorioDeprecated.Instancia.ObtenerParametrosProduccion(PRConstantesProduccion.ID_PARAMETRO_DIAS_HABILES);

      int.TryParse(diasHabilesMes, out diasMes);

      List<CMComisionesConceptosAdicionales> comisionesFijas = COFabricaDominio.Instancia.CrearInstancia<ICMFachadaComisiones>()
                  .ObtenerComisionesFijasCentroSvcContrato(liquidacion.IdCentroServicios);

      foreach (CMComisionesConceptosAdicionales comision in comisionesFijas)
      {
        ///Valida que la fecha de inicio sea mayor que la fecha de corte para realizar los calculos correspondientes
        if (comision.FechaInicio < liquidacion.FechaCorte)
        {
          ///Si la comision es comision de un contrato
          if (comision.IdContrato != 0)
          {
            if (comision.FechaTeminacionContrato < liquidacion.FechaCorte)
              liquidacion.FechaCorte = comision.FechaTeminacionContrato;
          }

          ///Si la ultima liquidacion es igual a 0, es porque la agencia no tiene liquidaciones aprobadas
          ///en este caso se toma la fecha de inicio de la comision para el calculo de los dias que se debe
          ///pagar a la agencia por la comision fija
          if (ultimaLiquidacion.IdLiquidacion == 0)
          {
            ultimaLiquidacion.FechaLiquidacion = comision.FechaInicio;
          }

          if (ultimaLiquidacion.FechaLiquidacion < liquidacion.FechaCorte)
          {
            comision.DiasHabiles = ObtenerDiasHabilesComision(ultimaLiquidacion.FechaLiquidacion, liquidacion.FechaCorte);
            valorDia = comision.Valor / diasMes;
            comision.ValorComision = valorDia * comision.DiasHabiles;
          }
        }
      }

      return comisionesFijas;
    }

    /// <summary>
    /// Obtiene los dias habiles de un rango de fechas
    /// </summary>
    /// <param name="fechaInicio"></param>
    /// <param name="fechaFin"></param>
    /// <returns></returns>
    public int ObtenerDiasHabilesComision(DateTime fechaInicio, DateTime fechaFin)
    {
      return PAAdministrador.Instancia.ObtenerDiasHabiles(fechaInicio, fechaFin, ConstantesFramework.PARA_PAIS_DEFAULT);
    }

    /// <summary>
    /// Guarda la liquidacion de produccion
    /// </summary>
    /// <param name="liquidacion"></param>
    public void GuardarLiquidacionCentroServicios(PRLiquidacionManualDCDeprecated liquidacion)
    {
      using (TransactionScope scope = new TransactionScope())
      {
        ///Obtiene la ultima liquidacion del centro de servicios no aprobada, si el centro de servicios tiene
        ///liquidacion sin aprobar retorna 0
        long idLiquidacion = PRRepositorioDeprecated.Instancia.ObtenerUltimaLiquidacionCentroSvc(liquidacion.IdCentroServicios);

        ///Si la liquidacion es mayor que 0, es porq existe una liquidacion sin aprobar para el centro de servicios
        ///y anula la liquidacion existente
        if (idLiquidacion > 0)
        {
          PRRepositorioDeprecated.Instancia.ActualizarEstadoLiquidacion(idLiquidacion, PRConstantesProduccion.ESTADO_LIQUIDACION_ANULADA);
        }

        ///Guarda la liquidacion de produccion
        liquidacion.IdLiquidacion = PRRepositorioDeprecated.Instancia.GuardarLiquidacionManual(liquidacion);

        ///Actualiza y guarda las comisiones pertenecientes a la liquidacion manual
        PRRepositorioDeprecated.Instancia.GuardaLiquidacionAgrupada(liquidacion);

        ///guarda la liquidacion de ingresos fijos
        liquidacion.ComisionesFijas
          .ForEach(ingreso =>
            {
              liquidacion.ComisionFija = ingreso;
              PRRepositorioDeprecated.Instancia.GuardarIngresosFijos(liquidacion);
            });

        scope.Complete();
      }
    }

    #endregion Métodos

    /// <summary>
    /// Obtiene las liquidaciones de las agencias/puntos
    /// </summary>
    /// <param name="filtro"></param>
    /// <param name="indicePagina"></param>
    /// <param name="registrosPorPagina"></param>
    /// <returns></returns>
    public List<PRLiquidacionManualDCDeprecated> ObtenerLiquidaciones(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina)
    {
      return PRRepositorioDeprecated.Instancia.ObtenerLiquidaciones(filtro, indicePagina, registrosPorPagina);
    }

    /// <summary>
    /// Obtiene el detalle de la liquidacion de la produccion
    /// </summary>
    /// <param name="liquidacion"></param>
    public PRLiquidacionManualDCDeprecated ObtenerDetalleLiquidacionProduccion(PRLiquidacionManualDCDeprecated liquidacion)
    {
      ///Obtiene las comisiones por concepto
      liquidacion.ComisionXConcepto = PRRepositorioDeprecated.Instancia.ObtenerComisionesLiquidacionProduccion(liquidacion.IdLiquidacion);

      if (liquidacion.ComisionXConcepto != null)
        liquidacion.ComisionXConcepto.ForEach(comision =>
        {
          liquidacion.TotalComisiones += comision.ValorComision;
        });

      ///Obtiene las comisiones por punto
      liquidacion.ComisionesPuntos = PRRepositorioDeprecated.Instancia.ObtenerLiquidacionPuntosaCargo(liquidacion.IdLiquidacion);
      ///Totaliza las comisiones por punto
      if (liquidacion.ComisionesPuntos != null)
        liquidacion.ComisionesPuntos.ForEach(comision =>
        {
          liquidacion.TotalComisionesPuntos += comision.ValorComision;
        });

      ///Obtiene las novedades por reintegro
      liquidacion.NovedadReintegro = PRRepositorioDeprecated.Instancia.ObtenerLiquidacionNovedad(liquidacion.IdLiquidacion, PRConstantesProduccion.ID_TIPO_NOVEDAD_REINTEGRO);
      ///Totaliza las novedades por reintegro
      if (liquidacion.NovedadReintegro != null)
        liquidacion.NovedadReintegro.ForEach(comision =>
        {
          liquidacion.TotalNovedadReintegro += comision.Valor;
        });

      ///Obtiene las novedades por descuentos
      liquidacion.NovedadDescuento = PRRepositorioDeprecated.Instancia.ObtenerLiquidacionNovedad(liquidacion.IdLiquidacion, PRConstantesProduccion.ID_TIPO_NOVEDAD_DESCUENTO);
      ///Totaliza las Novedades por descuento
      if (liquidacion.NovedadDescuento != null)
        liquidacion.NovedadDescuento.ForEach(comision =>
        {
          liquidacion.TotalNovedadDescuento += comision.Valor;
        });

      ///Obtiene la liquidacion de la comision fija
      liquidacion.ComisionesFijas = PRRepositorioDeprecated.Instancia.ObtenerLiquidacionIngresosFijos(liquidacion.IdLiquidacion);
      ///Totaliza las comisiones fijas
      if (liquidacion.ComisionesFijas != null)
        liquidacion.ComisionesFijas.ForEach(comision =>
        {
          liquidacion.TotalComisionesFijas += comision.ValorComision;
        });

      ///Obtiene las retenciones por concepto
      liquidacion.RetencionesConcepto = PRRepositorioDeprecated.Instancia.ObtenerLiquidacionRetencionConcepto(liquidacion.IdLiquidacion);
      ///Totaliza  las retenciones por concepto
      if (liquidacion.RetencionesConcepto != null)
        liquidacion.RetencionesConcepto.ForEach(comision =>
        {
          liquidacion.TotalRetencionesConcepto += comision.ValorRetencion;
        });

      ///Obtiene las retenciones de las novedades por reintegro
      liquidacion.RetencionesNovedad = PRRepositorioDeprecated.Instancia.ObtenerRetencionNovedadReintegroLiquidacion(liquidacion.IdLiquidacion, PRConstantesProduccion.ID_TIPO_NOVEDAD_REINTEGRO);
      ///Totaliza las retenciones por novedad
      if (liquidacion.RetencionesNovedad != null)
        liquidacion.RetencionesNovedad.ForEach(comision =>
        {
          liquidacion.TotalRetencionesNovedad += comision.ValorRetencion;
        });

      return liquidacion;
    }
  }
}