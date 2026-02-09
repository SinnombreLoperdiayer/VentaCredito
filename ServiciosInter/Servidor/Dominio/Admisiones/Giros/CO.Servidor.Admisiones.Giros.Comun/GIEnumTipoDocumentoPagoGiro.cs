namespace CO.Servidor.Admisiones.Giros.Comun
{
  /// <summary>
  /// Identifica el tipo de documento solicitado a la hora de hacer un pago de un giro
  /// </summary>
  public enum GIEnumTipoDocumentoPagoGiro : int
  {
    /// <summary>
    /// DOCUMENTO DE IDENTIDAD
    /// </summary>
    DOCUMENTOIDENTIDAD = 1,

    /// <summary>
    /// AUTORIZACION DE PAGO
    /// </summary>
    AUTORIZACIONPAGO = 2,

    /// <summary>
    /// CERTIFICADO EMPRESARIAL
    /// </summary>
    CERTIFICADOEMPRESARIAL = 3,

    /// <summary>
    /// DECLARACIO VOLUNTARIA PAGO
    /// </summary>
    DECLARACIOVOLUNTARIAPAGO = 4
  }
}