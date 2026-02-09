using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using Framework.Servidor.Excepciones;
using System.Collections.Generic;
using System.ServiceModel;
namespace CO.Servidor.Servicios.Contratos
{
    [ServiceContract(Namespace = "http://contrologis.com")]
    public interface IServiciosMovilesSvc
    {
        #region Operacion Urbana

         [OperationContract]
         [FaultContract(typeof(ControllerException))]
         IList<OUMensajeroDC> ObtenerMensajeroCentroLogistico(long centroLogistico);

         [OperationContract]
         [FaultContract(typeof(ControllerException))]
         List<OUNovedadIngresoDC> ObtenerNovedadesIngreso();

         [OperationContract]
         [FaultContract(typeof(ControllerException))]
         OUPlanillaVentaGuiasDC IngresarGuiaCentroAcopio(string numeroGuia, string idCiudadOrigen, string idPuntoOrigen, string idMensajero, string idNovedad, string usuario);
     


        #endregion
    }
}
