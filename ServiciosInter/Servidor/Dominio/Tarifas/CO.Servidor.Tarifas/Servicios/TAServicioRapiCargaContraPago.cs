using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Transactions;
using CO.Servidor.Dominio.Comun.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Precios;
using CO.Servidor.Tarifas.Datos;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;

namespace CO.Servidor.Tarifas.Servicios
{
  /// <summary>
  /// Clase para el manejo de tarifa Rapi Carga Contra Pago
  /// </summary>
  internal class TAServicioRapiCargaContraPago : TAServicio
  {
    #region Campos

    private static readonly TAServicioRapiCargaContraPago instancia = (TAServicioRapiCargaContraPago)FabricaInterceptores.GetProxy(new TAServicioRapiCargaContraPago(), COConstantesModulos.TARIFAS);
    private int idServicio = TAConstantesServicios.SERVICIO_RAPI_CARGA_CONTRA_PAGO;

    #endregion Campos

    #region Propiedades

    public static TAServicioRapiCargaContraPago Instancia
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
    /// Obtiene el precio del servicio rapi carga contra pago
    /// </summary>
    /// <param name="idListaPrecios">Identificador lista de precios</param>
    /// <returns>Valores de precios</returns>
    public TAPrecioCargaDC CalcularPrecio(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorContraPago, decimal valorDeclarado, bool esPrimeraAdmision, string idTipoEntrega = "-1")
    {
      string idListaPrecioServicio = TARepositorio.Instancia.ObtenerIdentificadorListaPrecioServicio(IdServicio, idListaPrecio);
      int idLp = int.Parse(idListaPrecioServicio);

      TAPrecioCargaDC precioValor = TARepositorio.Instancia.ObtenerPrecioRapiCarga(idServicio, idListaPrecio, idLp, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);

      TAPrecioCargaDC precio = new TAPrecioCargaDC()
      {
        Impuestos = TARepositorio.Instancia.ObtenerValorImpuestosServicio(IdServicio).ToList(),
        ValorKiloAdicional = precioValor.ValorKiloAdicional,
        ValorServicioRetorno = precioValor.ValorServicioRetorno,
        Valor = precioValor.Valor,
        ValorContraPago = TARepositorio.Instancia.ObtenerPrecioContrapago(idLp, valorContraPago)
      };

      return precio;
    }

    /// <summary>
    /// Obtiene precio rapicarga
    /// </summary>
    /// <param name="filtro">Filtro</param>
    /// <param name="campoOrdenamiento">Campo de Ordenamiento</param>
    /// <param name="indicePagina">Indice de página</param>
    /// <param name="registrosPorPagina">Registro por página</param>
    /// <param name="ordenamientoAscendente">Ordenamiento</param>
    /// <param name="totalRegistros">Total de Registros</param>
    /// <returns>Listado de precio internacional</returns>
    public IEnumerable<TAPrecioTrayectoDC> ObtenerPrecioTrayectoSubTrayectoRangoRapCarContraPago(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, int idListaPrecio)
    {
      IEnumerable<TAPrecioTrayectoDC> coleccionServicioRapiCarga = TARepositorio.Instancia.ObtenerPrecioTrayectoSubTrayectoRango(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, IdServicio, idListaPrecio);

      return coleccionServicioRapiCarga;
    }

    /// <summary>
    /// Guarda Tarifa
    /// </summary>
    public void GuardarTarifaRapiCargaContraPago(TATarifaRapiCargaContraPagoDC tarifaRapiCargaContraPago)
    {
      using (TransactionScope transaccion = new TransactionScope())
      {
        ActualizarTrayectoPrecioRango(tarifaRapiCargaContraPago.ServicioRapiCargaContraPago);
        TAAdministradorTarifas.Instancia.ModificarFormaPagoServicio(tarifaRapiCargaContraPago.FormasPago, IdServicio);
        tarifaRapiCargaContraPago.ServicioPeso.IdServicio = IdServicio;
        TARepositorio.Instancia.EditarServicioPeso(tarifaRapiCargaContraPago.ServicioPeso);
        TARepositorio.Instancia.EditarParametrosListaPrecioServicio(tarifaRapiCargaContraPago.ListaPrecioParametros);
        ActualizarPrecioRango(tarifaRapiCargaContraPago.PrecioRango, tarifaRapiCargaContraPago.IdListaPrecio);
        TAAdministradorTarifas.Instancia.ModificarImpuestosPorServicio(tarifaRapiCargaContraPago.Impuestos, IdServicio);

        transaccion.Complete();
      }
    }

    #endregion Métodos Públicos

    #region Métodos Privados

    /// <summary>
    /// Adiciona, Edita o Elimina un precio rango trayecto subtrayecto
    /// </summary>
    /// <param name="consolidadoCambios">Colección consolidado cambios</param>
    private void ActualizarTrayectoPrecioRango(ObservableCollection<TAPrecioTrayectoDC> consolidadoCambios)
    {
      consolidadoCambios.ToList().ForEach(precioTrayecto =>
      {
        if (precioTrayecto.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
          TARepositorio.Instancia.AdicionarPrecioTrayectoSubTrayectoRango(precioTrayecto, IdServicio);
        else if (precioTrayecto.EstadoRegistro == EnumEstadoRegistro.MODIFICADO)
          TARepositorio.Instancia.EditarPrecioTrayectoSubTrayectoRango(precioTrayecto, IdServicio);
        else if (precioTrayecto.EstadoRegistro == EnumEstadoRegistro.BORRADO)
          TARepositorio.Instancia.EliminarPrecioTrayectoSubTrayectoRango(precioTrayecto);
      });
    }

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