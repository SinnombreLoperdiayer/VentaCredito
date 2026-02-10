using System;

namespace ServiciosInter.DatosCompartidos.EntidadesNegocio.Giros
{
    /// <summary>
    /// Entidad encargada del mapping de la tabla EstadosGiro_GIR en IController
    /// </summary>
    public class EstadosGiro_GIR
    {
        /// <summary>
        /// Es el Numero del Giro
        /// </summary>
        public long idGiro { get; set; }

        /// <summary>
        /// Es el id interno del numero del giro
        /// </summary>
        public long IdAdminGiro { get; set; }

        /// <summary>
        /// Es el estado del giro
        /// </summary>
        public string EstadoGiro { get; set; }

        /// <summary>
        /// Es la fecha en la que cambio el estado el giro
        /// </summary>
        public DateTime FechaCambioEstado { get; set; }

        /// <summary>
        /// Es el usuario que actualizo el estado
        /// </summary>
        public string Usuario { get; set; }
    }
}