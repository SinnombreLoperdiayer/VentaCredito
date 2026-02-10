using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.OperacionNacional;

namespace CO.Servidor.Dominio.Comun.OperacionNacional
{
  public interface IONFachadaOperacionNacional
  {
    /// <summary>
    /// Indica si una guía, dado su número y el id de la agencia, ya ha sido ingresada a centro de acopio pero no había sido creada la guía como tal al momento del ingreso
    /// </summary>
    /// <param name="numeroGuia"></param>.
    /// <param name="idAgencia"></param>
    /// <returns>True si la guía ya fué ingresada</returns>
    bool GuiaYaFueIngresadaACentroDeAcopio(long numeroGuia, long idAgencia);

    /// <summary>
    /// Indica si una guía, dado su número ya ha sido ingresasda a centro de copio antes de haberla creado en el sistema.
    /// </summary>
    /// <param name="numeroGuia">Retorna el id del centro de servicio que ingresó a centro de acopio la guía</param>
    /// <returns></returns>
    long GuiaYaFueIngresadaACentroDeAcopioRetornaCS(long numeroGuia);

      /// <summary>
      /// Consulta el útimo manifiesto en el cual fué incluida una guía
      /// </summary>
      /// <param name="idAdmision"></param>
      /// <returns></returns>
    ONManifiestoGuia ConsultarUltimoManifiestoGuia(long idAdmision);

        /// <summary>
        /// Obtiene manifiesto de la guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        ONManifiestoOperacionNacional ObtenerResponsableGuiaManifiestoPorNGuia(long numeroGuia);        
    }
}
