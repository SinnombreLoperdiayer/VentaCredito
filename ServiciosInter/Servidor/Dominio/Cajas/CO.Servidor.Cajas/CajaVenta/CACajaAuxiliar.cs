using CO.Servidor.Cajas.Datos;
using CO.Servidor.Servicios.ContratoDatos.Cajas;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;

namespace CO.Servidor.Cajas.CajaVenta
{
  /// <summary>
  /// Clase que maneja las operaciones de la caja
  /// de un punto, su apertura cierre y consultas y validacion
  /// </summary>
  internal class CACajaAuxiliar : ControllerBase
  {
    #region Atributos

    private static readonly CACajaAuxiliar instancia = (CACajaAuxiliar)FabricaInterceptores.GetProxy(new CACajaAuxiliar(), COConstantesModulos.CAJA);

    #endregion Atributos

    #region Instancia

    public static CACajaAuxiliar Instancia
    {
      get { return CACajaAuxiliar.instancia; }
    }

    #endregion Instancia

    /// <summary>
    /// Obtiene la caja por apertura.
    /// </summary>
    /// <param name="idApertura">The id apertura.</param>
    /// <returns></returns>
    public CAAperturaCajaDC ObtenerCajaPorApertura(long idApertura)
    {
      return CARepositorioCaja.Instancia.ObtenerCajaPorApertura(idApertura);
    }
  }
}