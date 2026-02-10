using System.Globalization;

namespace Framework.Servidor.Comun
{
  /// <summary>
  /// Aplicación en servidor
  /// </summary>
  public class AppServidor
  {
    /// <summary>
    /// Almacena el nombre de la cultura del servidor de base de datos, se debe asignar desde la configuración
    /// </summary>
    public static string NOMBRE_CULTURA { get; set; }

    /// <summary>
    /// Almacena la clase de CultureInfo del servidor de base de datos
    /// </summary>
    public static CultureInfo CultureInfoServer { get; set; }
  }
}