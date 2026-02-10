using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Servidor.Comun
{
  /// <summary>
  /// Usuarios del sistema
  /// </summary>
  public class COUsuariosSistema
  {
    #region Instancia singleton de la clase

    private static readonly COUsuariosSistema instancia = new COUsuariosSistema();

    /// <summary>
    /// Retorna instancia de la clase de constantes de los móudulos de controller
    /// </summary>
    public static COUsuariosSistema Instancia
    {
      get { return COUsuariosSistema.instancia; }
    }

    #endregion Instancia singleton de la clase

    #region Usuarios Sistema

    /// <summary>
    /// Para las operaciones ejecutadas por el sistema
    /// </summary>
    public const string USUARIO_SISTEMA = "SISTEMA";

    #endregion Usuarios Sistema
  }
}