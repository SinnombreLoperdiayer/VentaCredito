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
  internal class TAServicioGiro : TAServicio
  {
    #region Campos

    private static readonly TAServicioGiro instancia = (TAServicioGiro)FabricaInterceptores.GetProxy(new TAServicioGiro(), COConstantesModulos.TARIFAS);
    private int idServicio = TAConstantesServicios.SERVICIO_GIRO;

    #endregion Campos

    #region Propiedades

    public static TAServicioGiro Instancia
    {
      get { return TAServicioGiro.instancia; }
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
    /// Guarda Tarifa
    /// </summary>
    public void GuardarTarifaGiros(TATarifaGirosDC tarifaGiros)
    {
      using (TransactionScope transaccion = new TransactionScope())
      {
        ActualizarPrecioRango(tarifaGiros.PrecioRango, tarifaGiros.IdListaPrecio);
        TAAdministradorTarifas.Instancia.ModificarFormaPagoServicio(tarifaGiros.FormasPago, IdServicio);
        TAAdministradorTarifas.Instancia.ModificarImpuestosPorServicio(tarifaGiros.Impuestos, IdServicio);
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