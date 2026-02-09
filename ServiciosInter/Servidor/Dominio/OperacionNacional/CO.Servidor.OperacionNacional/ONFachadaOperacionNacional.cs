using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Dominio.Comun.OperacionNacional;
using CO.Servidor.Servicios.ContratoDatos.OperacionNacional;

namespace CO.Servidor.OperacionNacional
{
  public class ONFachadaOperacionNacional : IONFachadaOperacionNacional
  {
    /// <summary>
    /// Instancia Singleton
    /// </summary>
    private static readonly ONFachadaOperacionNacional instancia = new ONFachadaOperacionNacional();

    #region Propiedades

    /// <summary>
    /// Retorna una instancia de la fabrica de Dominio
    /// </summary>
    public static ONFachadaOperacionNacional Instancia
    {
      get { return ONFachadaOperacionNacional.instancia; }
    }

    #endregion Propiedades

    /// <summary>
    /// Indica si una guía, dado su número y el id de la agencia, ya ha sido ingresada a centro de acopio pero no había sido creada la guía como tal al momento del ingreso
    /// </summary>
    /// <param name="numeroGuia"></param>.
    /// <param name="idAgencia"></param>
    /// <returns>True si la guía ya fué ingresada</returns>
    public bool GuiaYaFueIngresadaACentroDeAcopio(long numeroGuia, long idAgencia)
    {
      return ONAdministradorOperacionNacional.Instancia.GuiaYaFueIngresadaACentroDeAcopio(numeroGuia, idAgencia);
    }

    /// <summary>
    /// Indica si una guía, dado su número ya ha sido ingresasda a centro de copio antes de haberla creado en el sistema.
    /// </summary>
    /// <param name="numeroGuia">Retorna el id del centro de servicio que ingresó a centro de acopio la guía</param>
    /// <returns></returns>
    public long GuiaYaFueIngresadaACentroDeAcopioRetornaCS(long numeroGuia)
    {
      return ONAdministradorOperacionNacional.Instancia.GuiaYaFueIngresadaACentroDeAcopioRetornaCS(numeroGuia);
    }

    public ONManifiestoGuia ConsultarUltimoManifiestoGuia(long idAdmision)
    {
        return ONOperacionNacional.Instancia.ConsultarUltimoManifiestoGuia(idAdmision);
    }
        
        /// <summary>
        /// Obtiene manifiesto de la guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ONManifiestoOperacionNacional ObtenerResponsableGuiaManifiestoPorNGuia(long numeroGuia)
        {
            return ONOperacionNacional.Instancia.ObtenerResponsableGuiaManifiestoPorNGuia(numeroGuia);
        }
    }
}
