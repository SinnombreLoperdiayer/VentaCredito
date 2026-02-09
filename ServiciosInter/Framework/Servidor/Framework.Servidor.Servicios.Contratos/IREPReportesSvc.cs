using System.Collections.Generic;
using System.ServiceModel;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Importacion;
using Framework.Servidor.Servicios.ContratoDatos.Reportes;

namespace Framework.Servidor.Servicios.Contratos
{
  [ServiceContract(Namespace = "http://contrologis.com")]
  public interface IREPReportesSvc
  {
    /// <summary>
    /// Consulta los reportes asociadaos a un modulo
    /// </summary>
    /// <param name="modulo">modulo sobre el cual se hará a consulta</param>
    /// <returns></returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    IEnumerable<REPInfoReporte> ConsultarReportesXModulo(string modulo, string idCentroServicio);

    /// <summary>
    /// Consulta la url en la cual se encuentran ubicados los reportes del sistema
    /// </summary>
    /// <returns></returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    string ConsultarUrlReportes();

    /// <summary>
    /// Consulta los reportes disponibles en el sistema independientemente del módulo
    /// </summary>
    /// <returns></returns>
    [OperationContract]
    [FaultContract(typeof(ControllerException))]
    IEnumerable<REPInfoReporte> ConsultarReportesDisponibles();
  }
}