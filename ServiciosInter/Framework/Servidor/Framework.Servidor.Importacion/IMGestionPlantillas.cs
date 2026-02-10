using Framework.Servidor.Excepciones;
using Framework.Servidor.Importacion.Datos;
using Framework.Servidor.Servicios.ContratoDatos.Importacion;

namespace Framework.Servidor.Importacion
{
  public class IMGestionPlantillas : System.MarshalByRefObject
  {
    #region Instancia Singleton

    /// <summary>
    /// Instancia de la clase
    /// </summary>
    private static readonly IMGestionPlantillas instancia = (IMGestionPlantillas)FabricaInterceptores.GetProxy(new IMGestionPlantillas(), Framework.Servidor.Comun.ConstantesFramework.MODULO_FW_PLANTILLAS);

    /// <summary>
    /// Instancia de la clase
    /// </summary>
    public static IMGestionPlantillas Instancia
    {
      get { return IMGestionPlantillas.instancia; }
    }

    #endregion Instancia Singleton

    #region Métodos

    /// <summary>
    /// Registra plantilla en el sistema
    /// </summary>
    /// <param name="plantilla"></param>
    /// <param name="usuario"></param>
    public void RegistrarPlantilla(IMPlantillaImportacion plantilla, string usuario)
    {
      IMRepositorio.Instancia.RegistrarPlantilla(plantilla, usuario);
    }

    #endregion Métodos
  }
}