using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Transactions;
using CO.Servidor.ExploradorGiros.Datos;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;

namespace CO.Servidor.ExploradorGiros.Consulta
{
  public class GIOperacionesGiros : ControllerBase
  {
    #region CrearInstancia

    private static readonly GIOperacionesGiros instancia = (GIOperacionesGiros)FabricaInterceptores.GetProxy(new GIOperacionesGiros(), COConstantesModulos.GIROS);

    /// <summary>
    /// Retorna una instancia de centro Servicios
    /// /// </summary>
    public static GIOperacionesGiros Instancia
    {
      get { return GIOperacionesGiros.instancia; }
    }

    #endregion CrearInstancia

    #region Metodos

    #region Consultas externas

    /// <summary>
    /// Metodo que obtiene el id de la admision a partir del numero del giro
    /// </summary>
    /// <param name="numeroGuia">Numero del giro</param>
    /// <returns>Identificador de la admisión del giro</returns>
    public long ValidarGiro(long numeroGiro)
    {
      return GIRepositorioExploradorGiros.Instancia.ValidarGiro(numeroGiro);
    }

    /// <summary>
    /// Metodo que obtiene el id de la admision a partir del pago
    /// </summary>
    /// <param name="numeroGuia">Numero de la guía</param>
    public long ValidarPago(long numeroPago)
    {
      return GIRepositorioExploradorGiros.Instancia.ValidarPago(numeroPago);
    }

    #endregion Consultas externas

    #endregion Metodos
  }
}