using CO.Servidor.Servicios.ContratoDatos.ServicioalCliente;
using Framework.Servidor.Excepciones;
using System.Collections.Generic;
using System.ServiceModel;

namespace CO.Servidor.Servicios.Contratos
{
    [ServiceContract(Namespace = "http://contrologis.com")]
    public interface ISCServicioalClienteSvc
    {
       #region Consultas

       /// <summary>
       /// Método para obtener una lista con los tipos de solicitud y subtipos asociados
       /// </summary>
       /// <returns></returns>
       [OperationContract]
       [FaultContract(typeof(ControllerException))]
       IEnumerable<SACTipoSolicitudDC> ObtenerTiposSolicitud();



       /// <summary>
       /// Método para obtener una lista con los posibles estados de una solicitud
       /// </summary>
       /// <returns></returns>
       [OperationContract]
       [FaultContract(typeof(ControllerException))]
        IEnumerable<SACEstadosSolicitudDC> ObtenerEstados();


       /// <summary>
       /// Método para obtener una lista con los tipos de seguimiento
       /// </summary>
       /// <returns></returns>
       [OperationContract]
       [FaultContract(typeof(ControllerException))]
       IEnumerable<SACTipoSeguimientoDC> ObtenerTiposSeguimiento();



       /// <summary>
       /// Método para obtener una lista con los medios de recepción
       /// </summary>
       /// <returns></returns>
       [OperationContract]
       [FaultContract(typeof(ControllerException))]
        IEnumerable<SACMedioRecepcionDC> ObtenerMediosRecepcion();
 

       /// <summary>
       /// Método para obtener una lista con los medios de recepción
       /// </summary>
       /// <returns></returns>
       [OperationContract]
       [FaultContract(typeof(ControllerException))]
       SACSolicitudDC ObtenerSolicitud(long numeroGuia);


       #endregion


       #region Adicionar

       /// <summary>
       /// Método para adicionar una solicitud
       /// </summary>
       /// <param name="solicitud"></param>
       /// <returns></returns>
       [OperationContract]
       [FaultContract(typeof(ControllerException))]
        SACSolicitudDC GuardarCambiosSolicitud(SACSolicitudDC solicitud);


       /// <summary>
       /// Método para adicionar un estado de una solicitud
       /// </summary>
       /// <param name="solicitud"></param>
       /// <returns></returns>
       [OperationContract]
       [FaultContract(typeof(ControllerException))]
        SACSeguimientoSolicitudDC GuardarSeguimiento(SACSeguimientoSolicitudDC seguimiento);
  


       #endregion
    }
}
