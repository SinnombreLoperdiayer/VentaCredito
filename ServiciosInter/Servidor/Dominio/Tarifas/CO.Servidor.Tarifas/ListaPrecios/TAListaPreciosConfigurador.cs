using System.Collections.Generic;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.ListaPrecios;
using CO.Servidor.Tarifas.Datos;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;

namespace CO.Servidor.Tarifas.ListaPrecios
{
  /// <summary>
  /// Administrador de la lista de precios para controller
  /// </summary>
  internal class TAListaPreciosConfigurador : ControllerBase
  {
    public static TAListaPreciosConfigurador CrearInstancia()
    {
      TAListaPreciosConfigurador instancia = (TAListaPreciosConfigurador)FabricaInterceptores.GetProxy(new TAListaPreciosConfigurador(), COConstantesModulos.TARIFAS);

      return instancia;
    }

    public TAPrecioMensajariaExpresaDC ObtenerListaPreciosMesajeriaExpresa(int idListaPrecios)
    {
      return TARepositorioListaPrecios.Instancia.ObtenerListaPreciosMesajeriaExpresa(idListaPrecios);
    }

    public IEnumerable<TAListaPrecioDC> ObtenerListasPrecios()
    {
      return TARepositorioListaPrecios.Instancia.ObtenerListasPrecio();
    }
  }
}