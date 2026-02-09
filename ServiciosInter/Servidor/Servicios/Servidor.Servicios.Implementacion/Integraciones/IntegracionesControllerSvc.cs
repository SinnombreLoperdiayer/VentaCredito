using CO.Servidor.Integraciones;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Integraciones;
using CO.Servidor.Servicios.Contratos;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System;
using CO.Servidor.Integraciones.Satrack;
using CO.Servidor.Servicios.ContratoDatos.Integraciones.Satrack;

namespace CO.Servidor.Servicios.Implementacion.Integraciones
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class IntegracionesControllerSvc : IIntegracionesControllerSvc
    {
        #region Leonisa
        public string ConsultarGuiaLeonisa(long numeroGuia)
        {
            return IntegracionesController.Instancia.ConsultarGuiaLeonisa(numeroGuia);
        }
        #endregion

        #region SisPostal
        public long ADmisionSispostal(credencialDTO credencial, ADGuiaSisPostal admision)
        {
            return IntegracionSISPostal.Instancia.AdicionarAdmisionSisPostal(credencial, admision);
        }

        public void ConsultarEntregasClaro()
        {
            IntegracionSISPostal.Instancia.ConsultarEntregasClaro();
        }

        public List<INEstadosSWSispostal> ObtenerEstadosGuiaSispostal(long NGuia)
        {
            return IntegracionSISPostal.Instancia.ObtenerEstadosGuiaSispostal(NGuia);
        }

        /// <summary>
        /// Metodo para determinar si la guía fue descargada por la AppMasivos
        /// </summary>
        /// <param name="NGuia"></param>
        /// <returns></returns>
        public bool ValidarGuiaDescargadaXAppMasivos(long NGuia)
        {
            return IntegracionSISPostal.Instancia.ValidarGuiaDescargadaXAppMasivos(NGuia);
        }

        #endregion

        #region Tracking

        public INTrackingGuiaDC ConsultarTrackingGuia(credencialDTO credencial, long numeroGuia)
        {
            return IntegracionesController.Instancia.ConsultarTrackingGuia(credencial, numeroGuia);
        }

        #endregion
        #region Yanbal
        /// <summary>
        /// Metodo que consume el servicio de seguridad de yanbal para solicitar token
        /// </summary>
        /// <returns>Token y key de seguridad </returns>
        public INTRespuestaProceso SolicitarToken()
        {
            return IntegracionesController.Instancia.SolicitarToken();
        }
        /// <summary>
        /// Consume servicio de eventos de yanbal(Envia estados y motivos homologados)
        /// </summary>
        /// <param name="EventoRegistra"></param>
        /// <param name="token"></param>
        /// <returns>Respuesta del proceso </returns>
        public INTRespuestaProceso RegistraEventos(ADEstadoHomologacionYanbal[] EventoRegistra, INTRespuestaProceso token)
        {
            return IntegracionesController.Instancia.RegistrarEventos(EventoRegistra, token);
        }

        /// <summary>
        /// Consume el servicio de satrack para la programacion de un itinerario
        /// </summary>
        /// <param name="Programacion">Objeto de prigrmacion</param>
        /// <returns>resultado programacion</returns>
        public string ProgramarItinerario(List<INItinerarioDC> Programacion)
        {
            return IntegracionSatrack.Instancia.ProgramarItinerario(Programacion);
        }
        
        /// <summary>
        /// Consulta los movimientos en las admisiones de yanbal trayendo estados y motivos homologados
        /// </summary>
        /// <returns>Lista de estados y motivos homologados</returns>
        public List<ADEstadoHomologacionYanbal> ConsultaTrazaEventos()
        {
            return IntegracionesController.Instancia.ConsultarTrazaEventos();
        }
        public int ConsultarFrecuenciaEjecucion()
        {
            return IntegracionesController.Instancia.ConsultarFrecuenciaEjecucion();
        }
        #endregion

        #region Ecapture
        public bool InsertarLecturaEcaptureArchivoPruebaEntrega(INTArchivoPruebaEntrega archivoPruebaEntrega)
        {
            return IntegracionesController.Instancia.InsertarLecturaEcaptureArchivoPruebaEntrega(archivoPruebaEntrega);
        }

        public bool ValidarRecepcionHistoricoEcapture(long numeroGuia, string codigoProceso)
        {
            return IntegracionesController.Instancia.ValidarRecepcionHistoricoEcapture(numeroGuia, codigoProceso);
        }

        public int ConsultarOrigenGuia(long numeroGuia)
        {
            return IntegracionesController.Instancia.ConsultarOrigenGuia(numeroGuia);
        }

        public void ActualizarArchivoVolanteSincronizado(ArchivoVolante archivoVolante)
        {
            IntegracionesController.Instancia.ActualizarArchivoVolanteSincronizado(archivoVolante);
        }

        public List<ArchivoVolante> VerificarVolante(string numeroVolante)
        {
            return IntegracionesController.Instancia.VerificarVolante(numeroVolante);
        }

        public void InsertaHistoricoArchivoGuiaDigitalizada(ArchivoGuia archivoGuia)
        {
            IntegracionesController.Instancia.InsertaHistoricoArchivoGuiaDigitalizada(archivoGuia);
        }

        public void ActualizarArchivoGuiaDigitalizada(ArchivoGuia archivoGuia)
        {
            IntegracionesController.Instancia.ActualizarArchivoGuiaDigitalizada(archivoGuia);
        }

        public List<ArchivoGuia> VerificarGuia(string numeroGuia)
        {
            return IntegracionesController.Instancia.VerificarGuia(numeroGuia);
        }
        #endregion

    }



}
