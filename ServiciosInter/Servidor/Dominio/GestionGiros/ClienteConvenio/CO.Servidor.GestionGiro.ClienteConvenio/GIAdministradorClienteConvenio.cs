using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Dominio.Comun.Clientes;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Servicios.ContratoDatos.Clientes;

namespace CO.Servidor.GestionGiro.ClienteConvenio
{
  public class GIAdministradorClienteConvenio
  {
    #region Campos

    private static readonly GIAdministradorClienteConvenio instancia = new GIAdministradorClienteConvenio();

    #endregion Campos

    #region Propiedades

    /// <summary>
    /// Instancia de acceso
    /// </summary>
    public static GIAdministradorClienteConvenio Instancia
    {
      get { return GIAdministradorClienteConvenio.instancia; }
    }

    #endregion Propiedades

    #region Métodos Públicos

    /// <summary>
    /// Consulta los cliente creditos con sus respectivos contratos, y Si puede realizar giros convenio o dispersion de fondos
    /// </summary>
    /// <returns>Colección clientes y contratos</returns>
    public List<CLClientesDC> ObtenerClientesContratosGiros()
    {
      return COFabricaDominio.Instancia.CrearInstancia<ICLFachadaClientes>().ObtenerClientesContratosGiros();
    }

    /// <summary>
    /// Consulta los cliente creditos con sus respectivos contratos, y Si puede realizar giros convenio o dispersion de fondos
    /// y Cupo de Dispersion Aprobado
    /// </summary>
    /// <returns>Colección clientes y contratos</returns>
    public List<CLClientesDC> ObtenerTodosClientesContratosGiros()
    {
      return COFabricaDominio.Instancia.CrearInstancia<ICLFachadaClientes>().ObtenerTodosClientesContratosGiros();
    }

    /// <summary>
    /// Adiciona, edita o elimina una de las condiciones para el servicio de giros para un cliente
    /// </summary>
    /// <param name="cuentaExterna">Objeto cuenta externa</param>
    public void AdministrarClienteCondicionGiro(CLContratosDC contrato)
    {
      COFabricaDominio.Instancia.CrearInstancia<ICLFachadaClientes>().AdministrarClienteCondicionGiro(contrato);
    }

    #endregion Métodos Públicos
  }
}