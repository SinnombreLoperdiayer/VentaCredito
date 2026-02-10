using System.Collections.ObjectModel;
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
  /// Clase de negocio para el servicio rapi envíos contra pago
  /// </summary>
  internal class TAServicioRapiEnviosContraPago : TAServicio
  {
    #region Campos

    private static readonly TAServicioRapiEnviosContraPago instancia = (TAServicioRapiEnviosContraPago)FabricaInterceptores.GetProxy(new TAServicioRapiEnviosContraPago(), COConstantesModulos.TARIFAS);
    private int idServicio = TAConstantesServicios.SERVICIO_RAPI_ENVIOS_CONTRAPAGO;

    #endregion Campos

    #region Propiedades

    public static TAServicioRapiEnviosContraPago Instancia
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
    /// Obtiene el precio del servicio rapi envíos contra pago
    /// </summary>
    /// <param name="idListaPrecios">Identificador lista de precios</param>
    /// <returns>Valores de precios</returns>
    public TAPrecioMensajeriaDC CalcularPrecio(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorContraPago, decimal valorDeclarado, bool esPrimeraAdmision, string idTipoEntrega = "-1")
    {
      string idListaPrecioServicio = TARepositorio.Instancia.ObtenerIdentificadorListaPrecioServicio(IdServicio, idListaPrecio);
      int idLp = int.Parse(idListaPrecioServicio);
      TAPrecioMensajeriaDC valorPeso = TARepositorio.Instancia.ObtenerPrecioMensajeria(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision,idTipoEntrega);

      TAPrecioMensajeriaDC precio = new TAPrecioMensajeriaDC()
      {
        Impuestos = TARepositorio.Instancia.ObtenerValorImpuestosServicio(IdServicio).ToList(),
        ValorKiloInicial = valorPeso.ValorKiloInicial,
        ValorKiloAdicional = valorPeso.ValorKiloAdicional,
        Valor = valorPeso.Valor,
        ValorPrimaSeguro = valorPeso.ValorPrimaSeguro,
        ValorContraPago = TARepositorio.Instancia.ObtenerPrecioContrapago(idLp, valorContraPago)
      };

      return precio;
    }

    /// <summary>
    /// Guarda tarifa
    /// </summary>
    /// <param name="tarifaMensajeria">Objeto tarifa mensajería</param>
    public void GuardarTarifaRapiEnviosContraPago(TATarifaRapiEnviosContraPagoDC tarifaRapiEnvio)
    {
      using (TransactionScope transaccion = new TransactionScope())
      {
        TAAdministradorTarifas.Instancia.ModificarFormaPagoServicio(tarifaRapiEnvio.FormasPago, IdServicio);
        tarifaRapiEnvio.ServicioPeso.IdServicio = IdServicio;
        TARepositorio.Instancia.EditarServicioPeso(tarifaRapiEnvio.ServicioPeso);
        TARepositorio.Instancia.EditarParametrosListaPrecioServicio(tarifaRapiEnvio.ListaPrecioParametros);
        TAAdministradorTarifas.Instancia.ModificarImpuestosPorServicio(tarifaRapiEnvio.Impuestos, IdServicio);
      //  ActualizarPrecioRango(tarifaRapiEnvio.PrecioRango, tarifaRapiEnvio.IdListaPrecio);

        transaccion.Complete();
      }
    }

    #endregion Métodos Públicos

    #region Métodos Privados

    /// <summary>
    /// Adiciona, Edita o elimina un precio rango
    /// </summary>
    /// <param name="precioRango">Objeto Precio Rango</param>
    private void ActualizarPrecioRango(ObservableCollection<TAPrecioRangoDC> precioRango, int idListaPrecio)
    {
      string idListaPrecioServicio = TARepositorio.Instancia.ObtenerIdentificadorListaPrecioServicio(IdServicio, idListaPrecio);
      int idLps = int.Parse(idListaPrecioServicio);

      precioRango.ToList().ForEach(pr =>
      {
        if (pr.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
          TARepositorio.Instancia.AdicionarPrecioRango(pr, idLps);
        else if (pr.EstadoRegistro == EnumEstadoRegistro.MODIFICADO)
          TARepositorio.Instancia.EditarPrecioRango(pr);
        else if (pr.EstadoRegistro == EnumEstadoRegistro.BORRADO)
          TARepositorio.Instancia.EliminarPrecioRango(pr);
      });
    }

    #endregion Métodos Privados
  }
}