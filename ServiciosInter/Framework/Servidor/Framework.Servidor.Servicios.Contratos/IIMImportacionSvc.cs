using System.Collections.Generic;
using System.ServiceModel;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Importacion;

namespace Framework.Servidor.Servicios.Contratos
{
  [ServiceContract(Namespace = "http://contrologis.com")]
  public interface IIMImportacionSvc
  {
    /// <summary>
    /// Consulta la lista de plantillas disponibles
    /// </summary>
    /// <returns></returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    IEnumerable<IMPlantillaImportacion> ObtenerPlantillas();

    /// <summary>
    /// Registra plantilla en el sistema
    /// </summary>
    /// <param name="plantilla"></param>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    void RegistrarPlantilla(IMPlantillaImportacion plantilla);

    /// <summary>
    /// Retorna la primera plantilla uqe encuentre que tenga el nombre pasado como parámetro
    /// </summary>
    /// <param name="nombre">Nombre de la plantilla a consultar</param>
    /// <returns></returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    IMPlantillaImportacion ObtenerPlantilla(string nombre);
  }
}