using System.Collections.Generic;
using CO.Servidor.Admisiones.Giros.Pago;
using CO.Servidor.Admisiones.Giros.Venta;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros.Venta;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using CO.Servidor.Admisiones.Giros.Datos;

namespace CO.Servidor.Admisiones.Giros
{
  /// <summary>
  /// Fachada para la administracion de las Admisiones de Giros
  /// </summary>
  public class GIAdministracionGiros
  {
    private static readonly GIAdministracionGiros instancia = new GIAdministracionGiros();

    /// <summary>
    /// Retorna una instancia de la fachada de administracion de giros
    /// /// </summary>
    public static GIAdministracionGiros Instancia
    {
      get { return GIAdministracionGiros.instancia; }
    }

    /// <summary>
    /// Obtiene el valor total,servicio,tarifas de un giro dirigido a un cliente contado a partir de un contrato
    /// </summary>
    /// <returns>Colección de precio rango para una lista de precio servicio</returns>
    public TAPrecioDC ObtenerValorGiroClienteContadoGiro(int idContrato, decimal valor)
    {
      return GIAdmisionGiroConvenio.Instancia.ObtenerValorGiroClienteContadoGiro(idContrato, valor);
    }

    /// <summary>
    ///  Obtiene el valor a cobrar a un servicio segun su  valor total.
    /// </summary>
    public TAPrecioDC CalcularValorServicoAPartirValorTotal(int idContrato, decimal valorTotal)
    {
      return GIAdmisionGiroConvenio.Instancia.CalcularValorServicoAPartirValorTotal(idContrato, valorTotal);
    }

    /// <summary>
    /// Obtener los impuestos de el servicio de giros
    /// </summary>
    /// <returns></returns>
    public TAServicioImpuestosDC ObtenerImpuestosGiros()
    {
      return GIAdmisionGiroConvenio.Instancia.ObtenerImpuestosGiros();
    }

    /// <summary>
    /// Consulta los cliente creditos con sus respectivos contratos, y Si puede realizar giros convenio o dispersion de fondos
    /// </summary>
    /// <returns>Colección clientes y contratos</returns>
    public List<CLClientesDC> ObtenerClientesContratosGiros()
    {
      return GIAdmisionGiroConvenio.Instancia.ObtenerClientesContratosGiros();
    }

    /// <summary>
    /// Guardar los giros peaton convenio.
    /// </summary>
    public GINumeroGiro GuardarGiroPeatonConvenio(GIAdmisionGirosDC giro)
    {
      return GIAdmisionGiroConvenio.Instancia.GuardarGiroPeatonConvenio(giro);
    }

    /// <summary>
    /// Obtiene los estados de un giro a traves del tiempo
    /// </summary>
    /// <param name="idAdminGiro">Es el idAdmin del Giro</param>
    /// <returns>Lista de estados y Fecha</returns>
    public List<GIEstadosGirosDC> ObtenerEstadosGiro(long idAdminGiro)
    {
      return GIAdministracionGiros.Instancia.ObtenerEstadosGiro(idAdminGiro);
    }

    /// <summary>
    /// Actualiza la Informacion del Giro
    /// por una Solicitud
    /// </summary>
    /// <param name="giroUpdate">info del giro a Actualizar</param>
    public void ActualizarInfoGiro(GIAdmisionGirosDC giroUpdate)
    {
      PGPagosGiros.Instancia.ActualizarInfoGiro(giroUpdate);
    }


    /// <summary>
    /// Consulta la tabla Parametros Giros
    /// </summary>
    /// <param name="idParametro"></param>
    /// <returns></returns>
    public GIParametrosGirosDC ConsultarParametrosGiros(string idParametro)
    {
        return GIRepositorio.Instancia.ConsultarParametrosGiros(idParametro);
    }

    }
}