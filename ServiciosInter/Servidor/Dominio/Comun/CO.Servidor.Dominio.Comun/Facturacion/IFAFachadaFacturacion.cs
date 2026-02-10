using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.Facturacion;

namespace CO.Servidor.Dominio.Comun.Facturacion
{
  public interface IFAFachadaFacturacion
  {
    /// <summary>
    /// Consulta la informacion basica de una factura por el numero de Operacion( Numero de giro)
    /// </summary>
    /// <param name="numeroOperacion"></param>
    /// <returns></returns>
    FAOperacionFacturadaDC ConsultarFacturaPorNumeroOperacion(long numeroOperacion);

    /// <summary>
    /// Indica si una guía de un cliente crédito ya se encuentra facturada
    /// </summary>
    /// <param name="numeroGuia">Número de la guía a verificar</param>
    /// <returns></returns>
    bool GuiaYaFacturada(long numeroGuia);
  }
}