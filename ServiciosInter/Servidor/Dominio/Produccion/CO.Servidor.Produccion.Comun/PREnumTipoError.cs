namespace CO.Servidor.Produccion.Comun
{
  /// <summary>
  /// Enumeración con los códigos de mensajes de Operacion Urbana
  /// </summary>
  public enum PREnumTipoError : int
  {
    #region Mensajes de Excepción

    /// <summary>
    /// Mensaje de error {0} no se encuentra configurado
    /// </summary>
    EX_MENSAJE_NO_CONFIGURADO,

    /// <summary>
    /// No se encuentra configurado en base de datos el parametro {0}
    /// </summary>
    EX_NO_CONFIGURADO_PARAMETRO,

    #endregion Mensajes de Excepción

    #region Mensajes

    /// <summary>
    /// Mensaje ejecutado
    /// </summary>
    IN_EJECUTADO,

    /// <summary>
    /// Mensaje no ejecutado
    /// </summary>
    IN_NO_EJECUTADO,

    #endregion Mensajes
  }
}