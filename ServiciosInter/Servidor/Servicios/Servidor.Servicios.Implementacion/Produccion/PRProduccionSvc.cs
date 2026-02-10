using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using CO.Servidor.Produccion;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Produccion;
using CO.Servidor.Servicios.Contratos;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.Implementacion.Produccion
{
  /// <summary>
  /// Clase para los servicios de administración de Clientes
  /// </summary>
  [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
  public class PRProduccionSvc : IPRProduccionSvc
  {
      #region Administración de motivos de novedades
      public void GuardarMotivoNovedad(PRMotivoNovedadDC motivoNovedad)
      {
          PRProduccion.Instancia.GuardarMotivoNovedad(motivoNovedad);
      }

      public List<PRMotivoNovedadDC> ConsultarMotivosNovedad()
      {
          return PRProduccion.Instancia.ConsultarMotivosNovedad();
      }

      public void BorrarMotivoNovedad(PRMotivoNovedadDC motivoNovedad)
      {
          PRProduccion.Instancia.BorrarMotivoNovedad(motivoNovedad);
      }      
      #endregion


      #region Administración de retenciones
      public void GuardarValoresRetencion(PRRetencionProduccionDC retencion)
      {
          PRProduccion.Instancia.GuardarValoresRetencion(retencion);
      }

      public List<PRRetencionProduccionDC> ConsultarValoresRetenciones()
      {
          return PRProduccion.Instancia.ConsultarValoresRetenciones();
      }

      public void BorrarRetencion(PRRetencionProduccionDC retencion)
        {
            PRProduccion.Instancia.BorrarRetencion(retencion);
        }

      public void GuardarRetencionXCiudad(PRRetencionXCiudadDC retencionXCiudad)
      {
          PRProduccion.Instancia.GuardarRetencionXCiudad(retencionXCiudad);
      }

      public void BorrarRetencionXCiudad(PRRetencionXCiudadDC retencionXCiudad)
      {
          PRProduccion.Instancia.BorrarRetencionXCiudad(retencionXCiudad);
      }

      public List<PRRetencionXCiudadDC> ConsultarRetencionesXCiudad()
      {
          return PRProduccion.Instancia.ConsultarRetencionesXCiudad();
      }

      public List<PRRetencionDC> ConsultarTiposRetencion()
      {
          return PRProduccion.Instancia.ConsultarTiposRetencion();
      }
      #endregion

      #region Administrar Novedades

      public void GuardarNovedadesProduccion(List<PRNovedadProduccionDC> novedadesProduccion)
      {
          PRProduccion.Instancia.GuardarNovedadesProduccion(novedadesProduccion);
      }

      public List<PRNovedadProduccionDC> ConsultarNovedadesNoCargadas(int ano, int mes, long idCentroServicios)
      {
          return PRProduccion.Instancia.ConsultarNovedadesNoCargadas(ano, mes, idCentroServicios);
      }

      public void EliminarNovedad(long Idnovedad)
      {
          PRProduccion.Instancia.EliminarNovedad(Idnovedad);
      }
      #endregion

      #region Liquidaciones
      public void GenerarLiquidacion(long idCentroServicio, int mes, int ano)
      {
          PRProduccion.Instancia.GenerarLiquidacion(idCentroServicio, mes, ano);
      }

      public void AprobarLiquidaciones(long idRacol, string idCiudad, long idCentroServicio, int mes, int ano, long idLiqDesde, long idLiqHasta)
      {
          PRProduccion.Instancia.AprobarLiquidaciones(idRacol, idCiudad, idCentroServicio, mes, ano, idLiqDesde, idLiqHasta);
      }

      public void EliminarLiquidacionProduccion(long idLiqProduccion)
      {
          PRProduccion.Instancia.EliminarLiquidacionProduccion(idLiqProduccion);
      }

      public void CargarLiquidacionEnCaja(int mes, int ano)
      {
          PRProduccion.Instancia.CargarLiquidacionEnCaja(mes, ano);
      }

      public List<PRLiquidacionProduccionDC> GenerarGuiasLiquidaciones(long idRacol, string idCiudad, long idCentroServicio, int mes, int ano, long idLiqDesde, long idLiqHasta)
      {
          return PRProduccion.Instancia.GenerarGuiasLiquidaciones(idRacol, idCiudad, idCentroServicio, mes, ano, idLiqDesde, idLiqHasta);
      }

      public List<PRLiquidacionProduccionDC> ConsultarLiquidacionProduccion(long idRacol, string idCiudad, long idCentroServicio, int mes, int ano, long idLiqDesde, long idLiqHasta)
      {
          return PRProduccion.Instancia.ConsultarLiquidacionProduccion(idRacol, idCiudad, idCentroServicio, mes, ano,idLiqDesde,idLiqHasta);
      }
      #endregion

      public List<PRCiudadDC> ConsultarCiudades()
      {
          return Framework.Servidor.ParametrosFW.PAAdministrador.Instancia.ObtenerLocalidadesNoPaisNoDepartamentoColombia().ToList().ConvertAll<PRCiudadDC>(
              (c) =>
              {
                  return new PRCiudadDC()
                  {
                       IdLocalidad=c.IdLocalidad,
                       NombreLocalidad=c.NombreCompleto
                  };
              });
      }
  }
}