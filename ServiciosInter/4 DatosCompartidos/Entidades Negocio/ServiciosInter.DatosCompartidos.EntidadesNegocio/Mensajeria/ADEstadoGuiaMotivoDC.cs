using System;

namespace ServiciosInter.DatosCompartidos.EntidadesNegocio.Mensajeria
{
    public class ADEstadoGuiaMotivoDC
    {
        /// <summary>
        /// retorna o asigna el id de la traza del cambio de estado
        /// </summary>
        public long? IdTrazaGuia { get; set; }

        /// <summary>
        /// retorna o asigna el motivo del cambio de estado
        /// </summary>
        public ADMotivoGuiaDC Motivo { get; set; }

        /// <summary>
        /// retorna o asigna el estado de la guia
        /// </summary>
        public ADTrazaGuia EstadoGuia { get; set; }

        /// <summary>
        /// Retorna o asigna las observaciones de la guia
        /// </summary>
        public string Observaciones { get; set; }

        /// <summary>
        /// Retorna o asigna las observaciones de la guia
        /// </summary>
        public DateTime FechaMotivo { get; set; }
    }
}