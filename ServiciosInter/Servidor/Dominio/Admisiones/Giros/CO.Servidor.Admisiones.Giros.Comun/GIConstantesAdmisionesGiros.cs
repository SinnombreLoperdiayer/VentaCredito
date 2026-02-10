using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Admisiones.Giros.Comun
{
  /// <summary>
  /// Constantes Admison de giros
  /// </summary>
  public class GIConstantesAdmisionesGiros
  {
    #region Patrametros Generales

    public const string VALORMAXIMOACUMULADOCLIENTEGIRO = "ValorMaxCliente";
    public const string VALORMAXIMODECLARACIONFONDOS = "VlrMaxDeclaraFondos";
    public const string VALORMAXIMODECLARACIONFONDOSDESTINATARIO = "VlrMaxDeclFondoDest";
    public const string IDCONCEPTOPAGO = "IdConceptoPago";

    #endregion Patrametros Generales

    public const string GIROCONVENIOAPEATON = "CP";
    public const string GIROPEATONACONVENIO = "PC";
    public const string GIROPEATONAPEATON = "PP";
    public const string GIROPRODUCCION = "PR";
    public const string IDPEATON = "P";
    public const string NOMBREPEATON = "Peatón";
    public const string IDCONVENIO = "C";
    public const string NOMBRECONVENIO = "Convenio";
    public const int IDSUMINISTROGIROS = 5;
    public const int IDSUMINISTROPAGOS = 6;
    public const string TIPOIDCONVENIO = "NIT";

    /// <summary>
    /// Id Servicio de Giros
    /// </summary>
    public const int SERVICIO_GIRO = 8;
  }
}