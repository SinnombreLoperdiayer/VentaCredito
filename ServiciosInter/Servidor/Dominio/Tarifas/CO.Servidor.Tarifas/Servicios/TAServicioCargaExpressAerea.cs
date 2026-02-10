using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Transactions;
using CO.Servidor.Dominio.Comun.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Precios;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Servicios;
using CO.Servidor.Tarifas.Comun;
using CO.Servidor.Tarifas.Datos;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;

namespace CO.Servidor.Tarifas.Servicios
{
  internal class TAServicioCargaExpressAerea : TAServicio
  {
    #region Campos

    private static readonly TAServicioCargaExpressAerea instancia = (TAServicioCargaExpressAerea)FabricaInterceptores.GetProxy(new TAServicioCargaExpressAerea(), COConstantesModulos.TARIFAS);

    private int idServicio = TAConstantesServicios.SERVICIO_CARGA_AEREA;

    #endregion Campos

    #region Propiedades

    public static TAServicioCargaExpressAerea Instancia
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
      TAPrecioMensajeriaDC valorPeso = TARepositorio.Instancia.ObtenerPrecioMensajeria(TAConstantesServicios.SERVICIO_CARGA_AEREA, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision,idTipoEntrega);

      TAPrecioMensajeriaDC precio = new TAPrecioMensajeriaDC()
      {
        Impuestos = TARepositorio.Instancia.ObtenerValorImpuestosServicio(TAConstantesServicios.SERVICIO_CARGA_AEREA).ToList(),
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
    /// <param name="tarifaCargaExpress">Objeto tarifa mensajería</param>
    public void GuardarTarifaCargaExpressAerea(TATarifaMensajeriaDC tarifaCargaExpress)
    {
      using (TransactionScope transaccion = new TransactionScope())
      {
        TAAdministradorTarifas.Instancia.ModificarFormaPagoServicio(tarifaCargaExpress.FormasPago, IdServicio);
        tarifaCargaExpress.ServicioPeso.IdServicio = IdServicio;
        TARepositorio.Instancia.EditarServicioPeso(tarifaCargaExpress.ServicioPeso);
        TARepositorio.Instancia.EditarParametrosListaPrecioServicio(tarifaCargaExpress.ListaPrecioParametros);
        TAAdministradorTarifas.Instancia.ModificarImpuestosPorServicio(tarifaCargaExpress.Impuestos, IdServicio);

        transaccion.Complete();
      }
    }

    #endregion Métodos Públicos
  }
}