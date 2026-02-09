using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using CO.Servidor.Dominio.Comun.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Precios;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Servicios;
using CO.Servidor.Tarifas.Datos;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;

namespace CO.Servidor.Tarifas.Servicios
{
  internal class TAServicioRapiTulas : TAServicio
  {
    #region Campos

    private static readonly TAServicioRapiTulas instancia = (TAServicioRapiTulas)FabricaInterceptores.GetProxy(new TAServicioRapiTulas(), COConstantesModulos.TARIFAS);

    private int idServicio = TAConstantesServicios.SERVICIO_RAPI_TULAS;

    #endregion Campos

    #region Propiedades

    public static TAServicioRapiTulas Instancia
    {
      get { return instancia; }
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
    public TAPrecioMensajeriaDC CalcularPrecio(int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision, string idTipoEntrega = "-1")
    {
      TAPrecioMensajeriaDC valorPeso = TARepositorio.Instancia.ObtenerPrecioMensajeria(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);

      TAPrecioMensajeriaDC precio = new TAPrecioMensajeriaDC()
      {
        Impuestos = TARepositorio.Instancia.ObtenerValorImpuestosServicio(idServicio).ToList(),
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
    public void GuardarTarifa(TATarifaMensajeriaDC tarifa)
    {
      using (TransactionScope transaccion = new TransactionScope())
      {
        TAAdministradorTarifas.Instancia.ModificarFormaPagoServicio(tarifa.FormasPago, idServicio);
        tarifa.ServicioPeso.IdServicio = idServicio;
        TARepositorio.Instancia.EditarServicioPeso(tarifa.ServicioPeso);
        TARepositorio.Instancia.EditarParametrosListaPrecioServicio(tarifa.ListaPrecioParametros);
        TAAdministradorTarifas.Instancia.ModificarImpuestosPorServicio(tarifa.Impuestos, idServicio);

        transaccion.Complete();
      }
    }

    #endregion Métodos Públicos
  }
}
