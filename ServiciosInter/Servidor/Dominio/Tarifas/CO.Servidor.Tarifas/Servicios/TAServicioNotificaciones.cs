using System.Transactions;
using System.Linq;
using CO.Servidor.Dominio.Comun.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Precios;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Servicios;
using CO.Servidor.Tarifas.Datos;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;

namespace CO.Servidor.Tarifas.Servicios
{
  /// <summary>
  /// Clase de negocio para el servici ode notificaciones
  /// </summary>
  internal class TAServicioNotificaciones : TAServicio
  {
    #region Campos

    private static readonly TAServicioNotificaciones instancia = (TAServicioNotificaciones)FabricaInterceptores.GetProxy(new TAServicioNotificaciones(), COConstantesModulos.TARIFAS);
    private int idServicio = TAConstantesServicios.SERVICIO_NOTIFICACIONES;

    #endregion Campos

    #region Propiedades

    public static TAServicioNotificaciones Instancia
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
    /// Obtiene el precio del servicio rapi AM
    /// </summary>
    /// <param name="idListaPrecios">Identificador lista de precios</param>
    /// <returns>Valores de precios</returns>
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
    public void GuardarTarifaNotificaciones(TATarifaNotificacionesDC tarifaNotificaciones)
    {
      using (TransactionScope transaccion = new TransactionScope())
      {
        TAAdministradorTarifas.Instancia.ModificarFormaPagoServicio(tarifaNotificaciones.FormasPago, IdServicio);
        tarifaNotificaciones.ServicioPeso.IdServicio = IdServicio;
        TARepositorio.Instancia.EditarServicioPeso(tarifaNotificaciones.ServicioPeso);
        TARepositorio.Instancia.EditarParametrosListaPrecioServicio(tarifaNotificaciones.ListaPrecioParametros);
        TAAdministradorTarifas.Instancia.ModificarImpuestosPorServicio(tarifaNotificaciones.Impuestos, IdServicio);

        transaccion.Complete();
      }
    }

    #endregion Métodos Públicos
  }
}