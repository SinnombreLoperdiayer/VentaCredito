using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Transactions;
using CO.Servidor.Dominio.Comun.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Precios;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Servicios;
using CO.Servidor.Tarifas.Datos;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;

namespace CO.Servidor.Tarifas.Servicios
{
  /// <summary>
  /// Clase de negocio para el servicio rapi promocional
  /// </summary>
  internal class TAServicioRapiPromocional : TAServicio
  {
    #region Campos

    private static readonly TAServicioRapiPromocional instancia = (TAServicioRapiPromocional)FabricaInterceptores.GetProxy(new TAServicioRapiPromocional(), COConstantesModulos.TARIFAS);
    private int idServicio = TAConstantesServicios.SERVICIO_RAPI_PROMOCIONAL;

    #endregion Campos

    #region Propiedades

    public static TAServicioRapiPromocional Instancia
    {
      get { return instancia; }
    }

    /// <summary>
    /// Retorna el identificador del servicio
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
    /// Obtiene el precio del servicio rapi promocional
    /// </summary>
    /// <param name="idListaPrecios">Identificador lista de precios</param>
    /// <returns>Valores de precios</returns>
    public TAPrecioServicioDC CalcularPrecio(int idListaPrecios, decimal cantidad, string idTipoEntrega = "-1")
    {
      string idListaPrecioServicio = TARepositorio.Instancia.ObtenerIdentificadorListaPrecioServicio(IdServicio, idListaPrecios);
      int idLp = int.Parse(idListaPrecioServicio);

      TAPrecioServicioDC precio = new TAPrecioServicioDC()
      {
        Impuestos = TARepositorio.Instancia.ObtenerValorImpuestosServicio(IdServicio),
        ValoresAdicionales = TARepositorio.Instancia.ObtenerValorValoresAdicionalesServicio(IdServicio),
        Valor = TARepositorio.Instancia.ObtenerPrecioRapiPromocional(idLp, cantidad)
      };

      return precio;
    }

    /// <summary>
    /// Obtiene rapi promocional
    /// </summary>
    /// <param name="filtro">Filtro</param>
    /// <param name="campoOrdenamiento">Campo de Ordenamiento</param>
    /// <param name="indicePagina">Indice de página</param>
    /// <param name="registrosPorPagina">Registro por página</param>
    /// <param name="ordenamientoAscendente">Ordenamiento</param>
    /// <param name="totalRegistros">Total de Registros</param>
    /// <param name="idListaPrecio">Identificador lista de precio</param>
    /// <returns>Colección rapi promocional</returns>
    public IEnumerable<TAServicioRapiPromocionalDC> ObtenerRapiPromocional(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, int idListaPrecio)
    {
      IEnumerable<TAServicioRapiPromocionalDC> coleccionServicio = TARepositorio.Instancia.ObtenerRapiPromocional(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, IdServicio, idListaPrecio);

      return coleccionServicio;
    }

    /// <summary>
    /// Guarda Tarifa
    /// </summary>
    /// <param name="tarifaTramites">Objeto Tarifa</param>
    public void GuardarTarifaRapiPromocional(TATarifaRapiPromocionalDC tarifaRapiPromocional)
    {
      using (TransactionScope transaccion = new TransactionScope())
      {
        ActualizarRapiPromocional(tarifaRapiPromocional.ServicioRapiPromocional, tarifaRapiPromocional.IdListaPrecio);
        TAAdministradorTarifas.Instancia.ModificarFormaPagoServicio(tarifaRapiPromocional.FormasPago, IdServicio);
        tarifaRapiPromocional.ServicioPeso.IdServicio = IdServicio;
        TARepositorio.Instancia.EditarServicioPeso(tarifaRapiPromocional.ServicioPeso);
        TAAdministradorTarifas.Instancia.ModificarImpuestosPorServicio(tarifaRapiPromocional.Impuestos, IdServicio);

        transaccion.Complete();
      }
    }

    #endregion Métodos Públicos

    #region Métodos Privados

    /// <summary>
    /// Adiciona, edita o elimina trámites
    /// </summary>
    /// <param name="tramites">Colección con los trámites</param>
    /// <param name="idListaPrecio">Identificador Lista de precios</param>
    private void ActualizarRapiPromocional(ObservableCollection<TAServicioRapiPromocionalDC> rapiPromocional, int idListaPrecio)
    {
      string idListaPrecioServicio = TARepositorio.Instancia.ObtenerIdentificadorListaPrecioServicio(IdServicio, idListaPrecio);
      int idLp = int.Parse(idListaPrecioServicio);

      rapiPromocional.ToList().ForEach(r =>
        {
          if (r.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
            TARepositorio.Instancia.AdicionarRapiPromocional(r, idLp);
          else if (r.EstadoRegistro == EnumEstadoRegistro.MODIFICADO)
            TARepositorio.Instancia.EditarRapiPromocional(r);
          else if (r.EstadoRegistro == EnumEstadoRegistro.BORRADO)
            TARepositorio.Instancia.EliminarRapiPromocional(r);
        });
    }

    #endregion Métodos Privados
  }
}