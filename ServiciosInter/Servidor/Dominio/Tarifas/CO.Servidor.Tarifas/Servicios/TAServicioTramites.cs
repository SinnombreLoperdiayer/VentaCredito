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
  /// Clase de negocio para el servicio trámites
  /// </summary>
  internal class TAServicioTramites : TAServicio
  {
    #region Campos

    private static readonly TAServicioTramites instancia = (TAServicioTramites)FabricaInterceptores.GetProxy(new TAServicioTramites(), COConstantesModulos.TARIFAS);
    private int idServicio = TAConstantesServicios.SERVICIO_TRAMITES;

    #endregion Campos

    #region Propiedades

    public static TAServicioTramites Instancia
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
    /// Obtiene el precio del servicio trámites
    /// </summary>
    /// <param name="idListaPrecios">Identificador lista de precios</param>
    /// <returns>Valores de precios</returns>
    public TAPrecioTramiteDC CalcularPrecio(int idListaPrecios, int idTramite)
    {
      string idListaPrecioServicio = TARepositorio.Instancia.ObtenerIdentificadorListaPrecioServicio(IdServicio, idListaPrecios);
      int idLp = int.Parse(idListaPrecioServicio);
      TAPrecioTramiteDC valoresTramite = TARepositorio.Instancia.ObtenerPrecioTramite(idLp, idTramite);

      TAPrecioTramiteDC precio = new TAPrecioTramiteDC()
      {
        ValoresAdicionales = TARepositorio.Instancia.ObtenerValorValoresAdicionalesServicio(IdServicio),
        Valor = valoresTramite.Valor,
        ValorAdicionalLocal = valoresTramite.ValorAdicionalLocal,
        ValorAdicionalDocumento = valoresTramite.ValorAdicionalDocumento,
        ImpuestosTramite = valoresTramite.ImpuestosTramite
      };

      return precio;
    }

     

    /// <summary>
    /// Obtiene trámites
    /// </summary>
    /// <param name="filtro">Filtro</param>
    /// <param name="campoOrdenamiento">Campo de Ordenamiento</param>
    /// <param name="indicePagina">Indice de página</param>
    /// <param name="registrosPorPagina">Registro por página</param>
    /// <param name="ordenamientoAscendente">Ordenamiento</param>
    /// <param name="totalRegistros">Total de Registros</param>
    /// <param name="idListaPrecio">Identificador lista de precio</param>
    /// <returns>Colección trámites</returns>
    public IEnumerable<TAServicioTramiteDC> ObtenerTramites(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, int idListaPrecio)
    {
      IEnumerable<TAServicioTramiteDC> coleccionServicio = TARepositorio.Instancia.ObtenerTramites(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, IdServicio, idListaPrecio);

      return coleccionServicio;
    }

    /// <summary>
    /// Guarda Tarifa
    /// </summary>
    /// <param name="tarifaTramites">Objeto Tarifa</param>
    public void GuardarTarifaTramites(TATarifaTramitesDC tarifaTramites)
    {
      using (TransactionScope transaccion = new TransactionScope())
      {
        ActualizarTramites(tarifaTramites.ServicioTramites, tarifaTramites.IdListaPrecio);
        TAAdministradorTarifas.Instancia.ModificarFormaPagoServicio(tarifaTramites.FormasPago, IdServicio);

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
    private void ActualizarTramites(ObservableCollection<TAServicioTramiteDC> tramites, int idListaPrecio)
    {
      string idListaPrecioServicio = TARepositorio.Instancia.ObtenerIdentificadorListaPrecioServicio(IdServicio, idListaPrecio);
      int idLp = int.Parse(idListaPrecioServicio);

      tramites.ToList().ForEach(tramite =>
        {
          if (tramite.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
            TARepositorio.Instancia.AdicionarTramite(tramite, IdServicio, idLp);
          else if (tramite.EstadoRegistro == EnumEstadoRegistro.MODIFICADO)
            TARepositorio.Instancia.EditarTramite(tramite, idLp);
          else if (tramite.EstadoRegistro == EnumEstadoRegistro.BORRADO)
            TARepositorio.Instancia.EliminarTramite(tramite, idLp);
        });
    }

    #endregion Métodos Privados
  }
}