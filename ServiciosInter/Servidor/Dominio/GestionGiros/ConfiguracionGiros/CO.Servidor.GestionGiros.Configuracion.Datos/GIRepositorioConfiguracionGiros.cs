using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.GestionGiros.Configuracion.Datos.Modelo;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using Framework.Servidor.Comun;

namespace CO.Servidor.GestionGiros.Configuracion.Datos
{
  public class GIRepositorioConfiguracionGiros
  {
    #region Atributos

    /// <summary>
    /// Nombre del modelo
    /// </summary>
    private const string NombreModelo = "ModeloConfiguracionGiros";

    #endregion Atributos

    #region Crear Instancia

    private static readonly GIRepositorioConfiguracionGiros instancia = new GIRepositorioConfiguracionGiros();

    /// <summary>
    /// Retorna la instancia de la clase GIRepositorioConfiguracionGiros
    /// </summary>
    public static GIRepositorioConfiguracionGiros Instancia
    {
      get { return GIRepositorioConfiguracionGiros.instancia; }
    }

    #endregion Crear Instancia

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
      using (ModeloConfiguracionGiros contexto = new ModeloConfiguracionGiros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
      {
        return contexto.ClasificacionPorIngresos_GIR.ToList().ConvertAll(
          cla => new PUClasificacionPorIngresosDC()
          {
            IdClasificacion = cla.CXI_IdClasificacion,
            Descripcion = cla.CXI_Descripcion
          });
      }
    }

    /// <summary>
    /// Consultar los tipos de motivo  para la inactivacion de un giro
    /// </summary>
    /// <returns></returns>
    public List<string> ConsultarTiposMotivos()
    {
      using (ModeloConfiguracionGiros contexto = new ModeloConfiguracionGiros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
      {
        return contexto.MotivoInactivacionGiros_GIR.Select(c => c.MIG_Descripcion).ToList();
      }
    }

    #endregion Metodos
  }
}