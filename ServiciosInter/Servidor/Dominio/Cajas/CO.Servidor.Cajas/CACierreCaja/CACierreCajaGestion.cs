using System.Collections.Generic;
using CO.Servidor.Cajas.Datos;
using CO.Servidor.Servicios.ContratoDatos.GestionCajas.Cierres;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;

namespace CO.Servidor.Cajas.CACierreCaja
{
  /// <summary>
  /// Cierre de caja de gestión: Caja Casa Matriz, Operación Nacional y Banco
  /// </summary>
  internal class CACierreCajaGestion : ControllerBase
  {
    #region Campos

    private static readonly CACierreCajaGestion instancia = (CACierreCajaGestion)FabricaInterceptores.GetProxy(new CACierreCajaGestion(), COConstantesModulos.CAJA);

    #endregion Campos

    #region ctor

    private CACierreCajaGestion()
    {
    }

    #endregion ctor

    #region Propiedades

    /// <summary>
    /// Retorna la instancia de la clase
    /// </summary>
    internal static CACierreCajaGestion Instancia
    {
      get { return CACierreCajaGestion.instancia; }
    }

    #endregion Propiedades

    #region Métodos Públicos

    internal void CerrarCajaGestionConCentroServicios(short idCasaMatriz, long idCentroServicos, long idCodigoUsuario)
    {
      //1. Cerrar CS
      //2. Cerrar OPN, validar CS
      //3. Cerrar Banco, validar CS
    }

    /// <summary>
    /// Cerrar las aperturas de caja de Casa Matriz, Operación Naciona, Bancos y Centros de Servicios que el usuario ha hecho
    /// </summary>
    /// <param name="idCasaMatriz">Identificación de la casa matriz sobre la cual se hacen las aperturas</param>
    /// <param name="idCodigoUsuario">Código del usuario que hizo las aperturas</param>
    /// <param name="idRacol">Identificación del RACOL desde donde se hacen las operaciones</param>
    /// <remarks>Las aperturas sobre centros de servicos se hacen sobre la caja 0</remarks>
    internal void CerrarCajaGestion(short idCasaMatriz, long idCodigoUsuario, long idRacol)
    {
      //1. Cerrar CM
      //2. Cerrar OPN, validar CS
      //3. Cerrar Banco, validar CS
      CARepositorioGestionCajas.Instancia.CerrarCajasGestion(idCasaMatriz, idCodigoUsuario, idRacol);
    }

    /// <summary>
    /// Obtener la información del cierre de las cajas de gestión
    /// </summary>
    /// <param name="idCasaMatriz">Identificador de la casa matriz donde se hace el cierre</param>
    /// <param name="idCodigoUsuario">Identificador del usuairo que hace el cierre</param>
    /// <returns>Colección con la información del cierre</returns>
    internal IList<CACierreCajaGestionDC> ObtenerCierreCajaGestion(short idCasaMatriz, long idCodigoUsuario)
    {
      return CARepositorioGestionCajas.Instancia.ObtenerCierreCajasGestion(idCasaMatriz, idCodigoUsuario);
    }

    #endregion Métodos Públicos
  }
}