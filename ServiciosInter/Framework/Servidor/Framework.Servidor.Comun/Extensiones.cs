using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Servidor.Comun
{
  /// <summary>
  /// Extensiones sobre las clases base
  /// </summary>
  public static class Extensiones
  {
    /// <summary>
    /// Calcula una fecha Agregandole una cantidad de días laborables a partir de otra fecha, por tanto,
    /// a diferencia del DateTime.AddDays excluye los sábados y domingos.
    /// </summary>
    /// <param name="fechaOriginal">Fecha a partir de la cual se hace la suma</param>
    /// <param name="diasLaborables">Número de días laborables a calcular</param>
    /// <returns></returns>
    // public static DateTime AgregarDiasLaborales(this DateTime fechaOriginal, int diasLaborables)
    //{
    //  DateTime fechaTemporal = fechaOriginal;
    //  while (diasLaborables > 0)
    //  {
    //    fechaTemporal = fechaTemporal.AddDays(1);
    //    if (fechaTemporal.DayOfWeek < DayOfWeek.Saturday && fechaTemporal.DayOfWeek > DayOfWeek.Sunday)
    //    {
    //      diasLaborables--;
    //    }
    //  }
    //  return fechaTemporal;
    //}

    /// <summary>
    /// Obtiene los días laborales existentes entre dos fechas.
    /// </summary>
    /// <param name="fechaInicio"></param>
    /// <param name="fechaFinal"></param>
    /// <returns></returns>
    public static double ObtenerDiasLaborales(this DateTime fechaInicio, DateTime fechaFin)
    {
      if (fechaInicio.DayOfWeek == DayOfWeek.Saturday)
      {
        fechaInicio = fechaInicio.AddDays(2);
      }
      else if (fechaInicio.DayOfWeek == DayOfWeek.Sunday)
      {
        fechaInicio = fechaInicio.AddDays(1);
      }

      if (fechaFin.DayOfWeek == DayOfWeek.Saturday)
      {
        fechaFin = fechaFin.AddDays(-1);
      }
      else if (fechaFin.DayOfWeek == DayOfWeek.Sunday)
      {
        fechaFin = fechaFin.AddDays(-2);
      }

      int diff = (int)fechaFin.Subtract(fechaInicio).Days;

      double result = diff / 7 * 5 + diff % 7;

      if (fechaFin.DayOfWeek < fechaInicio.DayOfWeek)
      {
        return result - 2;
      }
      else
      {
        return result;
      }
    }
  }
}