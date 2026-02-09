using CO.Servidor.Dominio.Comun.Middleware;

namespace CO.Servidor.Dominio.Middleware.Helper
{
  /// <summary>
  /// Clase para crear una instancia a través de la fabrica de dominio y romper cualquier referencia circular entre los módulos de dominio y la fabrica
  /// </summary>
  public class COFabricaDominioHelper
  {
    private COFabricaDominioHelper instancia = new COFabricaDominioHelper();

    /// <summary>
    /// Retorna una instancia de la clase helper para acceso a la fabrica de objetos del dominio
    /// </summary>
    public COFabricaDominioHelper Instancia
    {
      get { return instancia; }
      set { instancia = value; }
    }

    /// <summary>
    /// Crear una instancia de un objeto a través de la fabrica del dominio
    /// </summary>
    /// <typeparam name="TModulo">Tipo del objeto que se desea crear</typeparam>
    /// <returns>Instancia del objeto</returns>
    public TModulo CrearInstancia<TModulo>()
    {
      return COFabricaDominio.Instancia.CrearInstancia<TModulo>();
    }
  }
}