using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Dominio.Comun.ExploradorGiros;
using CO.Servidor.Dominio.Comun.LogisticaInversa;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.ExploradorGiros.Consulta;

namespace CO.Servidor.ExploradorGiros
{
  public class GIFachadaExploradorGiros : IGIFachadaExploradorGiros
  {
    #region Consultas externas

    /// <summary>
    /// Metodo que obtiene el id de la admision a partir del numero del giro
    /// </summary>
    /// <param name="numeroGuia">Numero del giro</param>
    /// <returns>Identificador de la admisión del giro</returns>
    public long ValidarGiro(long numeroGiro)
    {
      return GIOperacionesGiros.Instancia.ValidarGiro(numeroGiro);
    }

    /// <summary>
    /// Metodo que obtiene el id de la admision a partir del pago
    /// </summary>
    /// <param name="numeroGuia">Numero de la guía</param>
    public long ValidarPago(long numeroPago)
    {
      return GIOperacionesGiros.Instancia.ValidarPago(numeroPago);
    }

    #endregion Consultas externas
  }
}