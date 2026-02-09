using System;

namespace ServiciosInter.DatosCompartidos.EntidadesNegocio.OperacionNacional
{
    public class ONNovedadesTransporteDC
    {
        /// <summary>
        /// Es el id del Manifiesto Operacion Nacional  (Autonumerico)
        /// </summary>
        public long IdManifiestoOperacionNacio { get; set; }

        /// <summary>
        /// Tipo de Novedad
        /// </summary>
        public string NombreNovedad { get; set; }

        public string LugarIncidente { get; set; }

        /// <summary>
        /// Observaciones
        /// </summary>
        public string Descripcion { get; set; }

        /// <summary>
        /// Fecha y hora del Incidente
        /// </summary>
        public DateTime FechaNovedad { get; set; }

        /// <summary>
        /// Tiempo de duración
        /// </summary>
        public string Tiempo { get; set; }

        /// <summary>
        /// Fecha Estimada de Entrega
        /// </summary>
        public DateTime FechaEstimadaEntrega { get; set; }
    }
}