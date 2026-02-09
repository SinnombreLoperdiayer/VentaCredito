using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.Solicitudes;

namespace CO.Servidor.Dominio.Comun.ExploradorGiros
{
  public interface IGIFachadaExploradorGiros
  {
    /// <summary>
    /// Metodo que obtiene el id de la admision a partir del numero del giro
    /// </summary>
    /// <param name="numeroGuia">Numero del giro</param>
    /// <returns>Identificador de la admisión del giro</returns>
    long ValidarGiro(long numeroGiro);

    /// <summary>
    /// Metodo que obtiene el id de la admision a partir del numero del giro
    /// </summary>
    /// <param name="numeroGuia">Numero del giro</param>
    /// <returns>Identificador de la admisión del giro</returns>
    long ValidarPago(long numeroPago);
  }
}