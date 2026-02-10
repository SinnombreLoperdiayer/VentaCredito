using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Reportes;

namespace Framework.Servidor.Reportes
{
  public class REPAdministradorReportes : ControllerBase
  {
    #region Instancia Singleton

    /// <summary>
    /// Instancia de la clase
    /// </summary>
    private static readonly REPAdministradorReportes instancia = (REPAdministradorReportes)FabricaInterceptores.GetProxy(new REPAdministradorReportes(), Framework.Servidor.Comun.ConstantesFramework.MODULO_FW_REPORTES);

    /// <summary>
    /// Instancia de la clase
    /// </summary>
    public static REPAdministradorReportes Instancia
    {
      get { return REPAdministradorReportes.instancia; }
    }

    #endregion Instancia Singleton

    /// <summary>
    /// Consulta los reportes asociadaos a un modulo
    /// </summary>
    /// <param name="modulo">modulo sobre el cual se hará a consulta</param>
    /// <returns></returns>
    public IEnumerable<REPInfoReporte> ConsultarReportesXModulo(string modulo, string idCentroServicio)
    {
      return Reportes.Datos.REPRepositorio.Instancia.ConsultarReportesXModulo(modulo, idCentroServicio);
    }

    /// <summary>
    /// Consulta la url en la cual se encuentran ubicados los reportes del sistema
    /// </summary>
    /// <returns></returns>
    public string ConsultarUrlReportes()
    {
      return Reportes.Datos.REPRepositorio.Instancia.ConsultarUrlReportes();
    }

    /// <summary>
    /// Consulta la llave con la cual se encriptan las urls de los reportes cuando se pasan por parámetro
    /// </summary>
    /// <returns>llave de encripcion</returns>
    public string ObtenerLlaveEncripcionUrlReportes()
    {
      return Reportes.Datos.REPRepositorio.Instancia.ObtenerLlaveEncripcionUrlReportes();
    }

    /// <summary>
    /// Consulta los reportes disponibles en el sistema independientemente del módulo
    /// </summary>
    /// <returns></returns>
    public IEnumerable<REPInfoReporte> ConsultarReportesDisponibles()
    {
      return Reportes.Datos.REPRepositorio.Instancia.ConsultarReportesDisponibles();
    }
  }
}