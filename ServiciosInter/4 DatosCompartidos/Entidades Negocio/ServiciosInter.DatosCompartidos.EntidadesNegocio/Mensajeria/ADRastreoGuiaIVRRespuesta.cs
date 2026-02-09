using System;

namespace ServiciosInter.DatosCompartidos.EntidadesNegocio.Mensajeria
{
    /// <summary>
    /// Entidad de respuesta a la solicitud
    /// </summary>
    public class ADRastreoGuiaIVRRespuesta
    {
        /// <summary>
        /// retorna o asigna el objeto con la traza de la guia.
        /// </summary>
        public ADTrazaGuiaEstadoGuia TrazaGuia { get; set; }
        /// <summary>
        /// retorna o asigna el EstadoEvaluacionEnvio.
        /// </summary>
        public string EstadoEvaluacionEnvio { get; set; }
        /// <summary>
        /// retorna o asigna la FechaEstimadaEntregaNew.
        /// </summary>
        public DateTime FechaEstimadaEntregaNew { get; set; }
        /// <summary>
        /// retorna o asigna la FechaEstimadaEntrega.
        /// </summary>
        public DateTime FechaEstimadaEntrega { get; set; }
        /// <summary>
        /// retorna o asigna el CentroServicioEstado.
        /// </summary>
        public string CentroServicioEstado { get; set; }
        /// <summary>
        /// retorna o asigna el TipoEntrega.
        /// </summary>
        public string TipoEntrega { get; set; }
        /// <summary>
        /// retorna o asigna el CentroServicioDestino.
        /// </summary>
        public string CentroServicioDestino { get; set; }
        /// <summary>
        /// retorna o asigna la FechaEntrega.
        /// </summary>
        public DateTime? FechaEntrega { get; set; }
    }
}
