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
  /// Clase para el manejo de tarifa Rapi Carga
  /// </summary>
  internal class TAServicioRapiCarga : TAServicio
  {
    #region Campos

    private static readonly TAServicioRapiCarga instancia = (TAServicioRapiCarga)FabricaInterceptores.GetProxy(new TAServicioRapiCarga(), COConstantesModulos.TARIFAS);

    private int idServicio = TAConstantesServicios.SERVICIO_RAPI_CARGA;

    #endregion Campos

    #region Propiedades

    public static TAServicioRapiCarga Instancia
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
    /// Obtiene el precio del servicio rapi carga
    /// </summary>
    /// <param name="idListaPrecios">Identificador lista de precios</param>
    /// <returns>Valores de precios</returns>
    public TAPrecioCargaDC CalcularPrecio(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision, string idTipoEntrega = "-1")
    {
      string idListaPrecioServicio = TARepositorio.Instancia.ObtenerIdentificadorListaPrecioServicio(IdServicio, idListaPrecio);
      int idLp = int.Parse(idListaPrecioServicio);

      TAPrecioCargaDC precioValor = TARepositorio.Instancia.ObtenerPrecioRapiCarga(idServicio, idListaPrecio, idLp, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision,idTipoEntrega);

      TAPrecioCargaDC precio = new TAPrecioCargaDC()
      {
        Impuestos = TARepositorio.Instancia.ObtenerValorImpuestosServicio(IdServicio).ToList(),
        ValorKiloAdicional = precioValor.ValorKiloAdicional,
        ValorServicioRetorno = precioValor.ValorServicioRetorno,
        Valor = precioValor.Valor,
        ValorPrimaSeguro = precioValor.ValorPrimaSeguro
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
    public IEnumerable<TAPrecioTrayectoDC> ObtenerPrecioTrayectoSubTrayectoRango(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, int idListaPrecio)
    {
      IEnumerable<TAPrecioTrayectoDC> coleccionServicioRapiCarga = TARepositorio.Instancia.ObtenerPrecioTrayectoSubTrayectoRango(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, IdServicio, idListaPrecio);
      return coleccionServicioRapiCarga;
    }

    /// <summary>
    /// Obtiene los precios de los trayectos por servicio
    /// </summary>
    /// <param name="idServicio"></param>
    /// <param name="idListaPrecio"></param>
    /// <returns></returns>
    public IEnumerable<TAPrecioTrayectoDC> ObtenerPrecioTraySubTrayectoRango(int idListaPrecio)
    {
      return TARepositorio.Instancia.ObtenerPrecioTraySubTrayectoRango(IdServicio, idListaPrecio);
    }

    /// <summary>
    /// Guarda los cambios realizados en RapiCarga
    /// </summary>
    /// <param name="consolidadoCambios">Objeto Consolidado de cambios</param>
    public void GuardarTarifa(TATarifaRapiCargaDC tarifaRapiCarga)
    {
      using (TransactionScope transaccion = new TransactionScope())
      {
        ActualizarPrecioRango(tarifaRapiCarga.ServicioRapiCarga);
        TAAdministradorTarifas.Instancia.ModificarFormaPagoServicio(tarifaRapiCarga.FormasPago, IdServicio);
        tarifaRapiCarga.ServicioPeso.IdServicio = IdServicio;
        TARepositorio.Instancia.EditarServicioPeso(tarifaRapiCarga.ServicioPeso);
        TARepositorio.Instancia.EditarParametrosListaPrecioServicio(tarifaRapiCarga.ListaPrecioParametros);
        TAAdministradorTarifas.Instancia.ModificarImpuestosPorServicio(tarifaRapiCarga.Impuestos, IdServicio);

        transaccion.Complete();
      }
    }

    #endregion Métodos Públicos

    #region Métodos Privados

    /// <summary>
    /// Adiciona, Edita o Elimina un precio rango trayecto subtrayecto
    /// </summary>
    /// <param name="consolidadoCambios">Colección consolidado cambios</param>
    private void ActualizarPrecioRango(ObservableCollection<TAPrecioTrayectoDC> consolidadoCambios)
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

    #endregion Métodos Privados
  }
}