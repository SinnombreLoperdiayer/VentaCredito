namespace CO.Servidor.Produccion.Comun
{
  public class PRConstantesProduccion
  {
    /// <summary>
    /// IdTipoNovedadPoduccion de la tabla TipoNovedad_PRO
    /// NOVEDAD CAMBIO FORMA DE PAGO
    /// </summary>
    public const string ID_NOVEDAD_CAMBIO_FORMA_PAGO_PRODUCCION = "MOTI_CAMBIOFORMAPAGO";

    /// <summary>
    /// IdTipoNovedadPoduccion de la tabla TipoNovedad_PRO
    /// CAMBIO DE DESTINO
    /// </summary>
    public const string ID_NOVEDAD_CAMBIO_DESTINO_PRODUCCION = "MOTI_CAMBIO_DESTINO";

    /// <summary>
    /// IdTipoNovedadPoduccion de la tabla TipoNovedad_PRO
    /// CAMBIO DE DESTINO
    /// </summary>
    public const string ID_PARAMETRO_DIAS_HABILES = "DiasHabilesMes";

    /// <summary>
    /// Id novedad por descuento
    /// </summary>
    public const int ID_TIPO_NOVEDAD_DESCUENTO = 1;

    /// <summary>
    /// Id novedad por reintegro
    /// </summary>
    public const int ID_TIPO_NOVEDAD_REINTEGRO = 2;

    /// <summary>
    /// Estado liquidacion anulado
    /// </summary>
    public const string ESTADO_LIQUIDACION_ANULADA = "ANU";

    /// <summary>
    /// Parametro para la impresion del detalle de la liquidacion
    /// </summary>
    public const string PARAMETRO_LIQUIDACIONES = "Liquidaciones";

    /// <summary>
    /// Parametro para la impresion del detalle de la liquidacion
    /// </summary>
    public const string PARAMETRO_USUARIO_IMPRIME = "Usuario";

    /// <summary>
    /// Parametro para la impresion del detalle de la liquidacion
    /// </summary>
    public const string REPORTE_DETALLE_LIQUIDACIONES = "/Produccion/DetalleLiquidacion.aspx";
  }
}