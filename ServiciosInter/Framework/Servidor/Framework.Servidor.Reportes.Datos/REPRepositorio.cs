using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.EncryptQueryStrings;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Reportes.Datos.Modelo;
using Framework.Servidor.Servicios.ContratoDatos.Reportes;

namespace Framework.Servidor.Reportes.Datos
{
  public class REPRepositorio
  {
    #region Instancia Singleton de la clase

    /// <summary>
    /// Instancia de la clase ASRepositorio
    /// </summary>
    public static readonly REPRepositorio Instancia = new REPRepositorio();

    #endregion Instancia Singleton de la clase

    #region Atributos

    private const string nombreModelo = "ModeloReportes";

    #endregion Atributos

    #region metodos publicos

    /// <summary>
    /// Consulta los reportes asociadaos a un modulo
    /// </summary>
    /// <param name="modulo">modulo sobre el cual se hará a consulta</param>
    /// <returns></returns>
    public IEnumerable<REPInfoReporte> ConsultarReportesXModulo(string modulo, string idCentroServicio)
    {
      using (EntidadesReportes contexto = new EntidadesReportes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
      {
        string usuario = "Prueba";
        if (ControllerContext.Current != null)
          usuario = ControllerContext.Current.Usuario;
        return contexto.ReportesRol_REP.Include("AdministradorReportes_REP").Include("Rol_SEG.UsuarioRol_SEG.Usuario_SEG").
          Where(rep => rep.AdministradorReportes_REP.ADR_IdModulo == modulo && rep.Rol_SEG.UsuarioRol_SEG.Count(usu => usu.Usuario_SEG.USU_IdUsuario == usuario) > 0)
           .GroupBy(x => new
           {
             x.AdministradorReportes_REP.ADR_IdReporte,
             x.AdministradorReportes_REP.ADR_NombreReporte,
             x.AdministradorReportes_REP.ADR_ReportPath,
             x.AdministradorReportes_REP.ADR_ReportServerUrl
           }).Select(group => group.Key)
          .ToList().ConvertAll(reporte =>
          {
            return new REPInfoReporte()
            {
              IdModulo = modulo,
              IdReporte = reporte.ADR_IdReporte,
              NombreReporte = reporte.ADR_NombreReporte,
              ReportPath = reporte.ADR_ReportPath,
              ReportServerUrl = reporte.ADR_ReportServerUrl,
              ReportPathYUrlEncriptados = EncriptarRutaReporte(reporte.ADR_ReportPath, reporte.ADR_ReportServerUrl, usuario, idCentroServicio)
            };
          });
      }
    }

    /// <summary>
    /// Consulta la url en la cual se encuentran ubicados los reportes del sistema
    /// </summary>
    /// <returns></returns>
    public string ConsultarUrlReportes()
    {
      using (EntidadesReportes contexto = new EntidadesReportes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
      {
        return contexto.ParametrosFramework.Where(par => par.PAR_IdParametro == "UrlReportes").FirstOrDefault().PAR_ValorParametro;
      }
    }

    /// <summary>
    /// Consulta los reportes disponibles en el sistema independientemente del módulo
    /// </summary>
    /// <returns></returns>
    public IEnumerable<REPInfoReporte> ConsultarReportesDisponibles()
    {
      using (EntidadesReportes contexto = new EntidadesReportes(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
      {
        return contexto.AdministradorReportes_REP.ToList().ConvertAll<REPInfoReporte>(rep =>
        {
          return new REPInfoReporte()
          {
            IdModulo = rep.ADR_IdModulo,
            IdReporte = rep.ADR_IdReporte,
            NombreReporte = rep.ADR_IdModulo + "-" + rep.ADR_NombreReporte,
            ReportPath = rep.ADR_ReportPath,
            ReportServerUrl = rep.ADR_ReportServerUrl
          };
        });
      }
    }

    /// <summary>
    /// Consulta la llave con la cual se encriptan las urls de los reportes cuando se pasan por parámetro
    /// </summary>
    /// <returns>llave de encripcion</returns>
    public string ObtenerLlaveEncripcionUrlReportes()
    {
      string conexion = COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo);
      using (EntidadesReportes contexto = new EntidadesReportes(conexion))
      {
        return contexto.ParametrosFramework.Where(par => par.PAR_IdParametro == "CryptoKeyReportes").FirstOrDefault().PAR_ValorParametro;
      }
    }

    #endregion metodos publicos

    #region metodos privados

    private string EncriptarRutaReporte(string ReportPath, string ReportServerUrl, string usuario, string idCentroServicio)
    {
      NameValueCollection queryStrings = new NameValueCollection();
      queryStrings.Add("ReportPath", ReportPath);
      queryStrings.Add("ReportServerUrl", ReportServerUrl);
      queryStrings.Add("Usuario", usuario);
      queryStrings.Add("IdCentroServicio", idCentroServicio);

      //return CryptoQueryStringHandler.EncryptQueryStrings(queryStrings,
      //                                      Encryption64.ObtenerLlaveEncripcion());

      return CryptoQueryStringHandler.EncryptQueryStrings(queryStrings,
                                             ObtenerLlaveEncripcionUrlReportes());
    }

    #endregion metodos privados
  }
}