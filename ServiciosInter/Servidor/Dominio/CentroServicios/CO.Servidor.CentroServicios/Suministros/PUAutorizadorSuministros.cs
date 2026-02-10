using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.Suministros;
using CO.Servidor.Servicios.ContratoDatos.Suministros;

namespace CO.Servidor.CentroServicios.Suministros
{
  /// <summary>
  /// Clase para el manejo de los suministros autorizados para un centros de servicio
  /// </summary>
  public class PUAutorizadorSuministros
  {
    #region SingleToN
    private static readonly PUAutorizadorSuministros instancia = new PUAutorizadorSuministros();

    public static PUAutorizadorSuministros Instancia
    {
      get
      {
        return instancia;
      }
    }
    #endregion SingleToN

    #region Metodos
    /// <summary>
    /// Guardar los suministros que posee un centro de servicio
    /// </summary>
    /// <param name="suministroCentroServicio"></param>
    public void GuardarSuministroCentroServicio(SUSuministroCentroServicioDC suministroCentroServicio)
    {
      COFabricaDominio.Instancia.CrearInstancia<ISUFachadaSuministros>().GuardarSuministroCentroServicio(suministroCentroServicio);
    }

    #endregion
  }
}