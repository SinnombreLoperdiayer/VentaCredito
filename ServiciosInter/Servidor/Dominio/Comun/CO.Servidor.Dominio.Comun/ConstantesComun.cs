using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Dominio.Comun
{
  public class ConstantesComun
  {
    /// <summary>
    /// Condición de uso de la guía, indica que el origen es cerrado y el destino abierto
    /// </summary>
    public const string CONDICION_USO_ORIGEN_CERRADO_DESTINO_ABIERTO = "OC-DA";

    /// <summary>
    /// Condición de uso de la guía, indica que el origen es cerrado y el destino cerrado
    /// </summary>
    public const string CONDICION_USO_ORIGEN_CERRADO_DESTINO_CERRADO = "OC-DC";

    /// <summary>
    /// Condición de uso de la guía, indica que el origen es abierto y el destino cerrado
    /// </summary>
    public const string CONDICION_USO_ORIGEN_ABIERTO_DESTINO_CERRADO = "OA-DC";
  }
}