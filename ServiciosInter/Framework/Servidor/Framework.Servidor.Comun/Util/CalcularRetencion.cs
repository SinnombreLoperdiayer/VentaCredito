using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Servidor.Comun.Util
{
  /// <summary>
  /// Clase que calcula el valor de la
  /// retencion Rafael Ramirez
  /// </summary>
  public class CalcularRetencion
  {
    #region Singleton

    private static readonly CalcularRetencion instancia = new CalcularRetencion();

    public static CalcularRetencion Instancia
    {
      get { return CalcularRetencion.instancia; }
    }

    #endregion Singleton

    /// <summary>
    /// Calcula el valor de la retencion.
    /// </summary>
    /// <param name="valorBase">The valor base.</param>
    /// <param name="tarifaPorcentual">The tarifa porcentual.</param>
    /// <param name="valorFijo">The valor fijo.</param>
    /// <returns>el valor Calculado</returns>
    public decimal CalcularValorRetencion(decimal valorBase, decimal tarifaPorcentual, decimal valorFijo)
    {
      decimal ValorCalculado = 0;

      if (valorBase != 0)
      {
        if (valorFijo != 0 && valorBase >= valorFijo)
        {
          ValorCalculado = valorFijo;
        }

        if (tarifaPorcentual != 0)
        {
          ValorCalculado = (valorBase * tarifaPorcentual) / 100;
        }
      }
      return ValorCalculado;
    }
  }
}