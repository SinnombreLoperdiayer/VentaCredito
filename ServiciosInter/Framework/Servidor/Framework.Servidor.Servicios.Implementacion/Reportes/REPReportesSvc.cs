using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Reportes;
using Framework.Servidor.Servicios.ContratoDatos.Reportes;
using Framework.Servidor.Servicios.Contratos;

namespace Framework.Servidor.Servicios.Implementacion.Reportes
{
  [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
  public class REPReportesSvc : IREPReportesSvc
  {
    public REPReportesSvc()
    {
      Thread.CurrentThread.CurrentCulture = new CultureInfo(PAAdministrador.Instancia.ConsultarParametrosFramework("Cultura"));
    }

    /// <summary>
    /// Consulta los reportes asociadaos a un modulo
    /// </summary>
    /// <param name="modulo">modulo sobre el cual se hará a consulta</param>
    /// <returns></returns>
    public IEnumerable<REPInfoReporte> ConsultarReportesXModulo(string modulo, string idCentroServicio)
    {
      return REPAdministradorReportes.Instancia.ConsultarReportesXModulo(modulo, idCentroServicio);
    }

    /// <summary>
    /// Consulta la url en la cual se encuentran ubicados los reportes del sistema
    /// </summary>
    /// <returns></returns>
    public string ConsultarUrlReportes()
    {
      return REPAdministradorReportes.Instancia.ConsultarUrlReportes();
    }

    /// <summary>
    /// Consulta los reportes disponibles en el sistema independientemente del módulo
    /// </summary>
    /// <returns></returns>
    public IEnumerable<REPInfoReporte> ConsultarReportesDisponibles()
    {
      return REPAdministradorReportes.Instancia.ConsultarReportesDisponibles();
    }
  }
}