using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Dominio.Comun.Produccion;
using CO.Servidor.Servicios.ContratoDatos.Produccion;

namespace CO.Servidor.Produccion
{
  public class PRFachadaProduccion : IPRFachadaProduccion
  {
    /// <summary>
    /// Adicionar novedad de Forma de Pago al Centro Servicio
    /// </summary>
    /// <param name="novedad"></param>
    public void AdicionarNovedadCentroServicioFormaPago(PANovedadCentroServicioDCDeprecated novedad)
    {
      PRAdmProduccionDeprecated.Instancia.AdicionarNovedadCentroServicioFormaPago(novedad);
    }

    /// <summary>
    /// Adicionar novedad de Cambio de destiono al Centro Servicio
    /// </summary>
    /// <param name="novedad"></param>
    public void AdicionarNovedadCentroServicioCambioDestino(PANovedadCentroServicioDCDeprecated novedad)
    {
      PRAdmProduccionDeprecated.Instancia.AdicionarNovedadCentroServicioCambioDestino(novedad);
    }

    /// <summary>
    /// Adiciona la Novedad del Centro de Servicio
    /// </summary>
    /// <param name="novedad">Data de la Novedad</param>
    public void AdicionarNovedadCentroServicio(PANovedadCentroServicioDCDeprecated novedad)
    {
      PRAdmProduccionDeprecated.Instancia.AdicionarNovedadCentroServicio(novedad);
    }
  }
}