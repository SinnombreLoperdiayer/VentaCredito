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
  /// Clase para el manejo de tarifa de centros de correspondencia
  /// </summary>
  internal class TAServicioCentroCorrespondencia : TAServicio
  {
    #region Campos

    private static readonly TAServicioCentroCorrespondencia instancia = (TAServicioCentroCorrespondencia)FabricaInterceptores.GetProxy(new TAServicioCentroCorrespondencia(), COConstantesModulos.TARIFAS);

    private int idServicio = TAConstantesServicios.SERVICIO_CENTRO_CORRESPONDENCIA;

    #endregion Campos

    #region Propiedades

    public static TAServicioCentroCorrespondencia Instancia
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
    /// Retorna el precio del servicio
    /// </summary>
    /// <param name="idListaPrecios">Identificador lista de precios</param>
    /// <returns>Colección con precios</returns>
    public TAPrecioCentroCorrespondenciaDC CalcularPrecio(int idListaPrecios)
    {
      string idListaPrecioServicio = TARepositorio.Instancia.ObtenerIdentificadorListaPrecioServicio(IdServicio, idListaPrecios);
      int idLp = int.Parse(idListaPrecioServicio);

      TAPrecioCentroCorrespondenciaDC precio = new TAPrecioCentroCorrespondenciaDC()
      {
        Impuestos = TARepositorio.Instancia.ObtenerValorImpuestosServicio(IdServicio),
        ValoresAdicionales = TARepositorio.Instancia.ObtenerValorValoresAdicionalesServicio(IdServicio),
        PrecioCentrosCorrespondencia = TARepositorio.Instancia.ObtenerPrecioCentroCorrespondencia(idLp)
      };

      return precio;
    }

    /// <summary>
    /// Obtiene los centros de correspondencia de una lista de precio servicio
    /// </summary>
    /// <param name="filtro">Filtro</param>
    /// <param name="campoOrdenamiento">Campo de Ordenamiento</param>
    /// <param name="indicePagina">Indice de página</param>
    /// <param name="registrosPorPagina">Registro por página</param>
    /// <param name="ordenamientoAscendente">Ordenamiento</param>
    /// <param name="totalRegistros">Total de Registros</param>
    /// <param name="idServicio">Identificador Servicio</param>
    /// <param name="idListaPrecio">Identificador lista de precio</param>
    /// <returns>Colección centros de correspondencia</returns>
    public IEnumerable<TAServicioCentroDeCorrespondenciaDC> ObtenerCentrosDeCorrespondencia(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, int idListaPrecio)
    {
      IEnumerable<TAServicioCentroDeCorrespondenciaDC> coleccionServicio = TARepositorio.Instancia.ObtenerCentrosDeCorrespondencia(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, IdServicio, idListaPrecio);

      return coleccionServicio;
    }

    /// <summary>
    /// Guarda Tarifa
    /// </summary>
    public void GuardarTarifaCentroDeCorrespondencia(TATarifaCentroDeCorrespondenciaDC tarifaCentroCorrespondencia)
    {
      using (TransactionScope transaccion = new TransactionScope())
      {
        ActualizarCentroDeCorrespondencia(tarifaCentroCorrespondencia.ServicioCentroDeCorrespondencia, tarifaCentroCorrespondencia.IdListaPrecio);
        TAAdministradorTarifas.Instancia.ModificarFormaPagoServicio(tarifaCentroCorrespondencia.FormasPago, IdServicio);
        TAAdministradorTarifas.Instancia.ModificarImpuestosPorServicio(tarifaCentroCorrespondencia.Impuestos, IdServicio);

        transaccion.Complete();
      }
    }

    #endregion Métodos Públicos

    #region Métodos Privados

    /// <summary>
    /// Adiciona, edita o elimina un centro de servicio
    /// </summary>
    /// <param name="consolidadoCambios">Consolidado de cambios</param>
    /// <param name="idListaPrecio">Identificador lista de precios</param>
    private void ActualizarCentroDeCorrespondencia(ObservableCollection<TAServicioCentroDeCorrespondenciaDC> consolidadoCambios, int idListaPrecio)
    {
      consolidadoCambios.ToList().ForEach(centroDeCorrespondencia =>
        {
          if (centroDeCorrespondencia.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
            TARepositorio.Instancia.AdicionarPrecioCentroDeCorrespondencia(centroDeCorrespondencia, IdServicio, idListaPrecio);
          else if (centroDeCorrespondencia.EstadoRegistro == EnumEstadoRegistro.MODIFICADO)
            TARepositorio.Instancia.EditarPrecioCentroDeCorrespondencia(centroDeCorrespondencia);
          else if (centroDeCorrespondencia.EstadoRegistro == EnumEstadoRegistro.BORRADO)
            TARepositorio.Instancia.EliminarPrecioCentroDeCorrespondencia(centroDeCorrespondencia);
        });
    }

    #endregion Métodos Privados
  }
}