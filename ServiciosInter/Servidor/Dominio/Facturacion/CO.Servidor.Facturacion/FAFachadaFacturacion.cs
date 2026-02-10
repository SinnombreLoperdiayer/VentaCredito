using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Dominio.Comun.Facturacion;
using CO.Servidor.Servicios.ContratoDatos.Facturacion;

namespace CO.Servidor.Facturacion
{
  public class FAFachadaFacturacion : IFAFachadaFacturacion
  {
    #region SingleToN

    private static readonly FAFachadaFacturacion instancia = new FAFachadaFacturacion();

    /// <summary>
    /// Retorna una instancia de la fachada de admisiones
    /// /// </summary>
    public static FAFachadaFacturacion Instancia
    {
      get { return FAFachadaFacturacion.instancia; }
    }

    #endregion SingleToN

    #region Metodos

    /// <summary>
    /// Consulta la informacion basica de una factura por el numero de Operacion( Numero de giro)
    /// </summary>
    /// <param name="numeroOperacion"></param>
    /// <returns></returns>
    public FAOperacionFacturadaDC ConsultarFacturaPorNumeroOperacion(long numeroOperacion)
    {
      return FAConsultafacturas.Instancia.ConsultarFacturaPorNumeroOperacion(numeroOperacion);
    }

    /// <summary>
    /// Indica si una guía de un cliente crédito ya se encuentra facturada
    /// </summary>
    /// <param name="numeroGuia">Número de la guía a verificar</param>
    /// <returns></returns>
    public bool GuiaYaFacturada(long numeroGuia)
    {
      if (FAConsultafacturas.Instancia.ConsultarFacturaPorNumeroOperacion(numeroGuia) == null)
        return false;

      return true;
    }

    #endregion Metodos
  }
}