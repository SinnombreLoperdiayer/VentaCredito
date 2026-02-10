using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using CO.Servidor.Tarifas.Datos;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;

namespace CO.Servidor.Tarifas
{
  internal class TAListaPrecios : ControllerBase
  {
    private static readonly TAListaPrecios instancia = (TAListaPrecios)FabricaInterceptores.GetProxy(new TAListaPrecios(), COConstantesModulos.TARIFAS);

    public static TAListaPrecios Instancia
    {
      get { return TAListaPrecios.instancia; }
    }

    /// <summary>
    /// Metodo encargado de devolver el id de la lista de precios vigente
    /// </summary>
    /// <returns>int con el id de la lista de precio</returns>
    public int ObtenerIdListaPrecioVigente()
    {
      return TARepositorio.Instancia.ObtenerIdListaPrecioVigente();
    }

    /// <summary>
    /// Metodo utilizado para conocer la lista de precios para determinado cliente credito
    /// </summary>
    /// <param name="IdClienteCredito"></param>
    /// <returns></returns>
    internal int ObtenerIdListaPrecioClienteCredito(int IdClienteCredito)
    {
        return TARepositorio.Instancia.ObtenerIdListaPrecioClienteCredito(IdClienteCredito);
    }
  }
}