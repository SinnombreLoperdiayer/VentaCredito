using System.Linq;
using System.Transactions;
using CO.Servidor.Dominio.Comun.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Precios;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Servicios;
using CO.Servidor.Tarifas.Datos;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;

namespace CO.Servidor.Tarifas.Servicios
{
  /// <summary>
  /// Clase de negocio para el manejo del servicio rapi personalizado
  /// </summary>
  internal class TAServicioRapiPersonalizado : TAServicio
  {
    #region Campos

    private static readonly TAServicioRapiPersonalizado instancia = (TAServicioRapiPersonalizado)FabricaInterceptores.GetProxy(new TAServicioRapiPersonalizado(), COConstantesModulos.TARIFAS);
    private int idServicio = TAConstantesServicios.SERVICIO_RAPI_PERSONALIZADO;

    #endregion Campos

    #region Propiedades

    public static TAServicioRapiPersonalizado Instancia
    {
      get { return instancia; }
    }

    /// <summary>
    /// Retorna el identificador del servicio a partir de su identificador interno
    /// </summary>
    public int IdServicio
    {
      get
      {
        return this.idServicio;
      }
      set
      {
        this.idServicio = value;
      }
    }

    #endregion Propiedades

    #region Métodos Públicos

    /// <summary>
    /// Obtiene el precio del servicio rapi personalizado
    /// </summary>
    /// <param name="idListaPrecios">Identificador lista de precios</param>
    /// <returns>Valores de precios</returns>
    public TAPrecioMensajeriaDC CalcularPrecio(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision,string idTipoEntrega = "-1")
    {
      TAPrecioMensajeriaDC valorPeso = TARepositorio.Instancia.ObtenerPrecioMensajeria(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision,idTipoEntrega);

      TAPrecioMensajeriaDC precio = new TAPrecioMensajeriaDC()
      {
        Impuestos = TARepositorio.Instancia.ObtenerValorImpuestosServicio(IdServicio).ToList(),
        ValorKiloInicial = valorPeso.ValorKiloInicial,
        ValorKiloAdicional = valorPeso.ValorKiloAdicional,
        Valor = valorPeso.Valor,
        ValorPrimaSeguro = valorPeso.ValorPrimaSeguro
      };

      return precio;
    }

    /// <summary>
    /// Guarda tarifa
    /// </summary>
    /// <param name="tarifaMensajeria">Objeto tarifa mensajería</param>
    public void GuardarTarifaRapiPersonalizado(TATarifaMensajeriaDC tarifaRapiPersonalizado)
    {
      using (TransactionScope transaccion = new TransactionScope())
      {
        TAAdministradorTarifas.Instancia.ModificarFormaPagoServicio(tarifaRapiPersonalizado.FormasPago, IdServicio);
        tarifaRapiPersonalizado.ServicioPeso.IdServicio = IdServicio;
        TARepositorio.Instancia.EditarServicioPeso(tarifaRapiPersonalizado.ServicioPeso);
        TARepositorio.Instancia.EditarParametrosListaPrecioServicio(tarifaRapiPersonalizado.ListaPrecioParametros);
        TAAdministradorTarifas.Instancia.ModificarImpuestosPorServicio(tarifaRapiPersonalizado.Impuestos, IdServicio);

        transaccion.Complete();
      }
    }

    #endregion Métodos Públicos
  }
}