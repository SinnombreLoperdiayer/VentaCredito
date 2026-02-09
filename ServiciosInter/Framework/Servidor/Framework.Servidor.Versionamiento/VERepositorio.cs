using System.Collections.Generic;
using System.Linq;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos;

namespace Framework.Servidor.Versionamiento
{
  public class VERepositorio : ControllerBase
  {
    #region Instancia Singleton

    /// <summary>
    /// Instancia de VERepositorio
    /// </summary>
    private static readonly VERepositorio instancia = (VERepositorio)FabricaInterceptores.GetProxy(new VERepositorio(), Framework.Servidor.Comun.ConstantesFramework.MODULO_FW_VERSIONAMIENTO);

    /// <summary>
    /// Propiedad de la instancia de la clase
    /// </summary>
    public static VERepositorio Instancia
    {
      get { return VERepositorio.instancia; }
    }

    #endregion Instancia Singleton

    #region Atributos

    private const string nombreModelo = "ModeloVersion";

    #endregion Atributos

    #region Consultas

    /// <summary>
    /// Consulta la lista de módulos del sistema
    /// </summary>
    /// <returns></returns>
    public IEnumerable<VEModulo> ObtenerModulos()
    {
      using (Modelo.VersionEntidades contexto = new Modelo.VersionEntidades(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
      {
        return contexto.Modulo_VER.ToList().ConvertAll(modulo => new VEModulo()
        {
          IdModulo = modulo.MOD_IdModulo,
          Descripcion = modulo.MOD_Descripcion
        });
      }
    }    

    #endregion Consultas
  }
}