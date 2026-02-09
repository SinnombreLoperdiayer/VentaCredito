using CO.Servidor.Servicios.ContratoDatos.Produccion;
using Framework.Servidor.Excepciones;
using System.Collections.Generic;
using System.ServiceModel;

namespace CO.Servidor.Servicios.Contratos
{
    /// <summary>
    /// Contratos WCF de centros de servicios
    /// </summary>
    [ServiceContract(Namespace = "http://contrologis.com")]
  public interface IPRProduccionSvc
  {      
      #region Administración de motivos de novedades
      [OperationContract]
      [FaultContract(typeof(ControllerException))]
      void GuardarMotivoNovedad(PRMotivoNovedadDC motivoNovedad);

      [OperationContract]
      [FaultContract(typeof(ControllerException))]
      List<PRMotivoNovedadDC> ConsultarMotivosNovedad();

      [OperationContract]
      [FaultContract(typeof(ControllerException))]
      void BorrarMotivoNovedad(PRMotivoNovedadDC motivoNovedad);
      #endregion


      #region Administración de retenciones
      [OperationContract]
      [FaultContract(typeof(ControllerException))]
      void GuardarValoresRetencion(PRRetencionProduccionDC retencion);

      [OperationContract]
      [FaultContract(typeof(ControllerException))]
      List<PRRetencionProduccionDC> ConsultarValoresRetenciones();

      [OperationContract]
      [FaultContract(typeof(ControllerException))]
      void BorrarRetencion(PRRetencionProduccionDC retencion);

      [OperationContract]
      [FaultContract(typeof(ControllerException))]
      void GuardarRetencionXCiudad(PRRetencionXCiudadDC retencionXCiudad);

      [OperationContract]
      [FaultContract(typeof(ControllerException))]
      void BorrarRetencionXCiudad(PRRetencionXCiudadDC retencionXCiudad);

      [OperationContract]
      [FaultContract(typeof(ControllerException))]
      List<PRRetencionXCiudadDC> ConsultarRetencionesXCiudad();

      [OperationContract]
      [FaultContract(typeof(ControllerException))]
      List<PRRetencionDC> ConsultarTiposRetencion();
        #endregion

      #region Administrar Novedades

      [OperationContract]
      [FaultContract(typeof(ControllerException))]
      void GuardarNovedadesProduccion(List<PRNovedadProduccionDC> novedadesProduccion);

      [OperationContract]
      [FaultContract(typeof(ControllerException))]
      List<PRNovedadProduccionDC> ConsultarNovedadesNoCargadas(int ano, int mes, long idCentroServicios);

      [OperationContract]
      [FaultContract(typeof(ControllerException))]
      void EliminarNovedad(long Idnovedad);
      #endregion

      #region Administrar liquidaciones

      [OperationContract]
      [FaultContract(typeof(ControllerException))]
      void GenerarLiquidacion(long idCentroServicio, int mes, int ano);

      [OperationContract]
      [FaultContract(typeof(ControllerException))]
      void AprobarLiquidaciones(long idRacol, string idCiudad, long idCentroServicio, int mes, int ano, long idLiqDesde, long idLiqHasta);

      [OperationContract]
      [FaultContract(typeof(ControllerException))]
      void EliminarLiquidacionProduccion(long idLiqProduccion);

      [OperationContract]
      [FaultContract(typeof(ControllerException))]
      void CargarLiquidacionEnCaja(int mes, int ano);

      [OperationContract]
      [FaultContract(typeof(ControllerException))]
      List<PRLiquidacionProduccionDC> GenerarGuiasLiquidaciones(long idRacol, string idCiudad, long idCentroServicio, int mes, int ano, long idLiqDesde, long idLiqHasta);

      [OperationContract]
      [FaultContract(typeof(ControllerException))]
      List<PRLiquidacionProduccionDC> ConsultarLiquidacionProduccion(long idRacol, string idCiudad, long idCentroServicio, int mes, int ano, long idLiqDesde, long idLiqHasta);
      
      #endregion

      [OperationContract]
      [FaultContract(typeof(ControllerException))]
      List<PRCiudadDC> ConsultarCiudades();
      
  }
}