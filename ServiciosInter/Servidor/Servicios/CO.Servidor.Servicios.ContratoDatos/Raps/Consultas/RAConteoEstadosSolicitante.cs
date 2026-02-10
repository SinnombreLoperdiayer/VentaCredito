using CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Consultas
{
    public class RAConteoEstadosSolicitante
    {
        /// <summary>
        /// Estado
        /// </summary>
        public EnumEstadosAplicacion Estado { get; set; }
        /// <summary>
        /// Descripcion
        /// </summary>
        public string Descripcion { get; set; }
        /// <summary>
        /// Cantidad
        /// </summary>
        public decimal Cantidad { get; set; }

        /// <summary>
        /// Tipo de Conteo
        /// P = Propio
        /// E = Enviados
        /// G = Grupo (mi equipo de trabajo)
        /// </summary>
        public string Tipo { get; set; }
    }
}
