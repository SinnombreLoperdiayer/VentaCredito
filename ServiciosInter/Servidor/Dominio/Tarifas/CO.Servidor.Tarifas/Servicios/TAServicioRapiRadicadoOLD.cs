using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using CO.Servidor.Dominio.Comun.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Precios;
using CO.Servidor.Tarifas.Comun;
using CO.Servidor.Tarifas.Datos;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;

namespace CO.Servidor.Tarifas.Servicios
{
  /// <summary>
  /// Clase de negocio para el servicio rapi radicado
  /// </summary>
  internal class TAServicioRapiRadicadoOLD : TAServicio
  {
    #region Campos

    private static readonly TAServicioRapiRadicadoOLD instancia = (TAServicioRapiRadicadoOLD)FabricaInterceptores.GetProxy(new TAServicioRapiRadicadoOLD(), COConstantesModulos.TARIFAS);

    private int idServicio = TAConstantesServicios.SERVICIO_RAPIRADICADO;

    #endregion Campos

    #region Propiedades

    public static TAServicioRapiRadicadoOLD Instancia
    {
      get { return TAServicioRapiRadicadoOLD.instancia; }
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
      TAPrecioMensajeriaDC valorPeso = TARepositorio.Instancia.ObtenerPrecioMensajeria(TAConstantesServicios.SERVICIO_MENSAJERIA, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision,idTipoEntrega);

      // Se define en solicitud de cambio generada por doña Fidela  el día 17 de septiembre de 2012 que:
      // * Todos los valores de rapiradicado, son los mismos de mensajería pero por dos
      TAPrecioMensajeriaDC precio = new TAPrecioMensajeriaDC()
      {
        Impuestos = TARepositorio.Instancia.ObtenerValorImpuestosServicio(TAConstantesServicios.SERVICIO_MENSAJERIA).ToList(),
        ValorKiloInicial = valorPeso.ValorKiloInicial * 2,
        ValorKiloAdicional = valorPeso.ValorKiloAdicional * 2,
        Valor = valorPeso.Valor * 2,
        ValorPrimaSeguro = valorPeso.ValorPrimaSeguro * 2
      };

      return precio;
    }

    #endregion Métodos Públicos
  }
}