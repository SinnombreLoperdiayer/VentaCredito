using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.GestionGiros.Configuracion.Datos;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;

namespace CO.Servidor.GestionGiro.ConfiguracionGiros
{
  public class GIAdministradorConfiguracionGiros
  {
    #region Singleton

    private static readonly GIAdministradorConfiguracionGiros instancia = new GIAdministradorConfiguracionGiros();

    /// <summary>
    /// Instancia de acceso
    /// </summary>
    public static GIAdministradorConfiguracionGiros Instancia
    {
      get { return GIAdministradorConfiguracionGiros.instancia; }
    }

    #endregion Singleton

    #region Metodos

    /// <summary>
    /// consulta los diferentes clasificaciones que existen para un agencia por el servicio de giros
    /// superhabitarias
    /// deficitarias
    /// compensadas
    /// </summary>
    /// <param name="idCentroServicio"></param>
    /// <returns></returns>
    public List<PUClasificacionPorIngresosDC> ConsultarClasificacionPorIngreso()
    {
      return GIRepositorioConfiguracionGiros.Instancia.ConsultarClasificacionPorIngreso();
    }

    /// <summary>
    /// Consultar los tipos de motivo  para la inactivacion de un giro
    /// </summary>
    /// <returns></returns>
    public List<string> ConsultarTiposMotivos()
    {
      return GIRepositorioConfiguracionGiros.Instancia.ConsultarTiposMotivos();
    }

    /// <summary>
    /// Guarda la nueva informacion de la configuracion de giros
    /// </summary>
    /// <param name="centroServicio"></param>
    public void GuardarConfiguracionGiro(PUCentroServiciosDC centroServicio)
    {
      COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ActualizarConfiguracionGiros(centroServicio);
    }

    /// <summary>
    /// Obtener las observaciones de un centro de servicio
    /// </summary>
    /// <param name="idCentroServicio"></param>
    /// <returns></returns>
    public IList<PUObservacionCentroServicioDC> ObtenerObservacionCentroServicio(long idCentroServicio)
    {
      return COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerObservacionCentroServicio(idCentroServicio);
    }

    #endregion Metodos
  }
}