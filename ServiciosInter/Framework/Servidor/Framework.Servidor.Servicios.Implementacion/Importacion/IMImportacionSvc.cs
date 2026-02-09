using System.Collections.Generic;
using System.Globalization;
using System.ServiceModel.Activation;
using System.Threading;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Importacion;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Servicios.ContratoDatos.Importacion;
using Framework.Servidor.Servicios.Contratos;
using System.ServiceModel;

namespace Framework.Servidor.Servicios.Implementacion.Importacion
{
  [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
  public class IMImportacionSvc : IIMImportacionSvc
  {
    public IMImportacionSvc()
    {
      Thread.CurrentThread.CurrentCulture = new CultureInfo(PAAdministrador.Instancia.ConsultarParametrosFramework("Cultura"));
    }

    /// <summary>
    /// Consulta la lista de plantillas disponibles
    /// </summary>
    /// <returns></returns>
    public IEnumerable<IMPlantillaImportacion> ObtenerPlantillas()
    {
      return IMConsultaPlantillas.Instancia.ObtenerPlantillasDisponibles();
    }

    /// <summary>
    /// Registra plantilla en el sistema
    /// </summary>
    /// <param name="plantilla"></param>
    public void RegistrarPlantilla(IMPlantillaImportacion plantilla)
    {
      IMGestionPlantillas.Instancia.RegistrarPlantilla(plantilla, ControllerContext.Current.Usuario);
    }

    /// <summary>
    /// Retorna la primera plantilla uqe encuentre que tenga el nombre pasado como parámetro
    /// </summary>
    /// <param name="nombre">Nombre de la plantilla a consultar</param>
    /// <returns></returns>
    public IMPlantillaImportacion ObtenerPlantilla(string nombre)
    {
      return IMConsultaPlantillas.Instancia.ObtenerPlantilla(nombre);
    }
  }
}