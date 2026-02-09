using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.OperacionUrbana.Comun
{
  public enum OUEnumEstadoSolicitudRecogidas : int
  {
    // <summary>
    /// Estado Programada.
    /// </summary>
    IN_PROGRAMADA = 1,

    /// <summary>
    /// Estado pendiente por programar
    /// </summary>
    IN_PENDIENTE_PROGRAMAR = 2,

      /// <summary>
      /// estado programada no recogida
      /// </summary>
    IN_PROGRAMADA_NO_RECOGIDA = 3,


    /// <summary>
    /// Estado realizada
    /// </summary>
    IN_REALIZADA = 4,

  
    /// <summary>
    /// Estado programada no planillada
    /// </summary>
    IN_PROGRAMADA_SIN_PLANILLAR = 5,

    /// <summary>
    /// Estado para la solicitud descargada que permite reprogramar
    /// </summary>
    IN_DESCARGADA_POR_REPROGRAMAR = 6,

    /// <summary>
    /// Estado solicitud reprogramar
    /// </summary>
    IN_REPROGRAMAR = 7,

    /// <summary>
    /// Estado solicitud recogida disponible
    /// </summary>
    IN_DISPONIBLE = 8,

    /// <summary>
    /// Estado solicitud recogida pendiente por coordenadas
    /// </summary>
    IN_PENDIENTE_COORDENADAS = 9,

      /// <summary>
      /// Estado indicando que la recogida para el area de operaciones, este tipo de programacion no vence
      /// </summary>
    IN_PROGRAMADA_OPERACION = 10,

      /// <summary>
      /// Estado indicando que la recogida fue cancelada
      /// </summary>
     IN_CANCELADA = 11

  }
}