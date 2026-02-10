using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Integraciones;
using CO.Servidor.Servicios.ContratoDatos.Integraciones.Satrack;
using Framework.Servidor.Excepciones;
using System.Collections.Generic;
using System.ServiceModel;

namespace CO.Servidor.Servicios.Contratos
{


    [ServiceContract(Namespace = "http://contrologis.com")]
    public interface IIntegracionesControllerSvc
    {
        #region Leonisa
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        string ConsultarGuiaLeonisa(long numeroGuia);
        #endregion

        #region 472

        //[OperationContract]
        //[FaultContract(typeof(ControllerException))]
        //respuestaWSRiesgoLiquidezDTO consultaIngresosEgresosPuntosDeAtencion(credencialDTO credencial, string codigoRed, DateTime fecha, string horaInicial, string horaFinal);

        //[OperationContract]
        //[FaultContract(typeof(ControllerException))]
        //respuestaWSRiesgoLiquidezDTO consultaValorRealPorPuntosDeAtencion(credencialDTO credencial, string codigoRed);

        //[OperationContract]
        //[FaultContract(typeof(ControllerException))]
        //respuestaWSRiesgoLiquidezDTO consultaValorRealPuntoDeAtencion(credencialDTO credencial, string codigoRed, string idCentroServicio);

        #endregion

        #region Sispostal
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        long ADmisionSispostal(credencialDTO credencial, ADGuiaSisPostal admision);

        //[OperationContract]
        //[FaultContract(typeof(ControllerException))]
        //void NotificarEntregasClaro();

        /// <summary>
        /// Metodo para determinar si la guía fue descargada por la AppMasivos
        /// </summary>
        /// <param name="NGuia"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        bool ValidarGuiaDescargadaXAppMasivos(long NGuia);

        #endregion

        #region Tracking

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        INTrackingGuiaDC ConsultarTrackingGuia(credencialDTO credencial, long numeroGuia);

        #endregion
        #region Yanbal
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        INTRespuestaProceso SolicitarToken();
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ADEstadoHomologacionYanbal> ConsultaTrazaEventos();
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        INTRespuestaProceso RegistraEventos(ADEstadoHomologacionYanbal[] EventoRegistra, INTRespuestaProceso token);
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        int ConsultarFrecuenciaEjecucion();
        #endregion


        #region ecapture

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        bool InsertarLecturaEcaptureArchivoPruebaEntrega(INTArchivoPruebaEntrega archivoPruebaEntrega);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        bool ValidarRecepcionHistoricoEcapture(long numeroGuia, string codigoProceso);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        int ConsultarOrigenGuia(long numeroGuia);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizarArchivoVolanteSincronizado(ArchivoVolante archivoVolante);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ArchivoVolante> VerificarVolante(string numeroVolante);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void InsertaHistoricoArchivoGuiaDigitalizada(ArchivoGuia archivoGuia);


        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void ActualizarArchivoGuiaDigitalizada(ArchivoGuia archivoGuia);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ArchivoGuia> VerificarGuia(string numeroGuia);
        #endregion

        #region Satrack

        /// <summary>
        /// Consume el servicio de satrack para la programacion de un itinerario
        /// </summary>
        /// <param name="Programacion">Objeto de prigrmacion</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        string ProgramarItinerario(List<INItinerarioDC> Programacion);

        #endregion
    }

}