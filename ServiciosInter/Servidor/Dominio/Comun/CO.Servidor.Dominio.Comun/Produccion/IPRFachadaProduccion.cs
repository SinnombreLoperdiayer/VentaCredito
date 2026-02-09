using CO.Servidor.Servicios.ContratoDatos.Produccion;

namespace CO.Servidor.Dominio.Comun.Produccion
{
  public interface IPRFachadaProduccion
  {
    /// <summary>
    /// Adicionar novedad de Forma de Pago al Centro Servicio
    /// </summary>
    /// <param name="novedad"></param>
    void AdicionarNovedadCentroServicioFormaPago(PANovedadCentroServicioDCDeprecated novedad);

    /// <summary>
    /// Adicionar novedad de Cambio de destiono al Centro Servicio
    /// </summary>
    /// <param name="novedad"></param>
    void AdicionarNovedadCentroServicioCambioDestino(PANovedadCentroServicioDCDeprecated novedad);

    /// <summary>
    /// Adiciona la Novedad del Centro de Servicio
    /// </summary>
    /// <param name="novedad">Data de la Novedad</param>
    void AdicionarNovedadCentroServicio(PANovedadCentroServicioDCDeprecated novedad);
  }
}