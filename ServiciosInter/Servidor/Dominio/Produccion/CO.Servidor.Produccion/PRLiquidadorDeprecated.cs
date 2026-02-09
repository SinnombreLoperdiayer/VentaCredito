using Framework.Servidor.Comun;

namespace CO.Servidor.Produccion
{
  /// <summary>
  ///  Clase encargada de hacer la liquidación de las comisiones para la Producción
  /// </summary>
  internal class PRLiquidadorDeprecated : ControllerBase
  {
    /// <summary>
    /// Retorna la instancia del liquidador de comisiones fijas
    /// </summary>
    /// <value>
    /// The liquidador comision fija.
    /// </value>
    internal LiquidadorComisionFijaDeprecated LiquidadorComisionFija
    {
      get
      {
        throw new System.NotImplementedException();
      }
      set
      {
      }
    }
  }
}