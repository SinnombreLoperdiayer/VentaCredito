using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros;
using CO.Servidor.Servicios.ContratoDatos.GestionGiros.Telemercadeo;

namespace CO.Servidor.Telemercadeo.Giros
{
  public class GIAdministradorTelemercadeoGiros
  {
    private static GIAdministradorTelemercadeoGiros instancia = new GIAdministradorTelemercadeoGiros();

    /// <summary>
    /// Instancia de acceso
    /// </summary>
    public static GIAdministradorTelemercadeoGiros Instancia
    {
      get { return GIAdministradorTelemercadeoGiros.instancia; }
    }

    /// <summary>
    /// Retorna los resultados de la gestion de telemercadeo
    /// </summary>
    /// <returns></returns>
    public List<GIResultadoGestionTelemercadeoDC> ObtenerResultadoGestionTelemercadeo()
    {
      return GITelemercadeoGiros.Instancia.ObtenerResultadoGestionTelemercadeo();
    }

    /// <summary>
    /// Obtiene los giros que cumplen con los tiempos para estar en telemercadeo
    /// </summary>
    /// <returns></returns>
    public List<GIAdmisionGirosDC> ObtenerGirosTelemercadeo(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina, long idRacol)
    {
      return GITelemercadeoGiros.Instancia.ObtenerGirosTelemercadeo(filtro, indicePagina, registrosPorPagina, idRacol);
    }

    /// <summary>
    /// Obtiene la informacion del giro
    /// </summary>
    /// <param name="giro"></param>
    public GIAdmisionGirosDC ObtenerInformacionGiroTelemercadeo(GIAdmisionGirosDC giro)
    {
      return GITelemercadeoGiros.Instancia.ObtenerInformacionGiroTelemercadeo(giro);
    }

    /// <summary>
    /// Obtiene el historico de telemercadeo de un giro
    /// </summary>
    /// <param name="idGiro"></param>
    public List<GITelemercadeoGiroDC> ObtenerHistoricoTelemercadeoGiro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long idGiro)
    {
      return GITelemercadeoGiros.Instancia.ObtenerHistoricoTelemercadeoGiro(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idGiro);
    }

    /// <summary>
    /// Guardar telemercadeo giro
    /// </summary>
    /// <param name="telemercadeo"></param>
    public void GuardarTelemercadeoGiro(GITelemercadeoGiroDC telemercadeo)
    {
      GITelemercadeoGiros.Instancia.GuardarTelemercadeoGiro(telemercadeo);
    }

    /// <summary>
    /// Retorna la informacion de telemercadeo
    /// </summary>
    /// <param name="idTelemercadeoGiro"></param>
    /// <returns></returns>
    public GITelemercadeoGiroDC ObtenerDetalleTelemercadeoGiro(long idTelemercadeoGiro)
    {
      return GITelemercadeoGiros.Instancia.ObtenerDetalleTelemercadeoGiro(idTelemercadeoGiro);
    }

    /// <summary>
    /// Obtiene la informacion de Telemercadeo de
    /// un giro especifico
    /// </summary>
    /// <param name="idAdmisionGiro"></param>
    /// <returns>la info del telemercadeo de un giro</returns>
    public GITelemercadeoGiroDC ObtenerTelemercadeoDeGiro(long idAdmisionGiro)
    {
      return GITelemercadeoGiros.Instancia.ObtenerTelemercadeoDeGiro(idAdmisionGiro);
    }
  }
}