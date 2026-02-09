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
  /// Clase de negocio para el manejo de mensajería
  /// </summary>
  internal class TAServicioMensajeria : TAServicio
  {
    #region Campos

    private static readonly TAServicioMensajeria instancia = (TAServicioMensajeria)FabricaInterceptores.GetProxy(new TAServicioMensajeria(), COConstantesModulos.TARIFAS);

    private int idServicio = TAConstantesServicios.SERVICIO_MENSAJERIA;

    #endregion Campos

    #region Propiedades

    public static TAServicioMensajeria Instancia
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
    /// Calcula precio
    /// </summary>
    /// <param name="idServicio">Identificador servicio</param>
    /// <param name="idListaPrecio">Identificador id lista de precio</param>
    /// <param name="idLocalidadOrigen">Identificador ciudad de origen</param>
    /// <param name="idLocalidadDestino">Identificador ciudad de destino</param>
    /// <param name="peso">Peso</param>
    /// <returns>Objeto valor</returns>
    public TAPrecioMensajeriaDC CalcularPrecio(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision, string idTipoEntrega = "-1")
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
    public void GuardarTarifaMensajeria(TATarifaMensajeriaDC tarifaMensajeria)
    {
      using (TransactionScope transaccion = new TransactionScope())
      {
        TAAdministradorTarifas.Instancia.ModificarFormaPagoServicio(tarifaMensajeria.FormasPago, IdServicio);
        tarifaMensajeria.ServicioPeso.IdServicio = IdServicio;
        TARepositorio.Instancia.EditarServicioPeso(tarifaMensajeria.ServicioPeso);
        TARepositorio.Instancia.EditarParametrosListaPrecioServicio(tarifaMensajeria.ListaPrecioParametros);
        TAAdministradorTarifas.Instancia.ModificarImpuestosPorServicio(tarifaMensajeria.Impuestos, IdServicio);

        transaccion.Complete();
      }
    }

    #endregion Métodos Públicos
  }
}