namespace ServiciosInter.DatosCompartidos.EntidadesNegocio.Parametros
{
    public class PAOperadorPostal
    {
        public int Id { get; set; }

        public string Nombre { get; set; }

        //public string Descripcion { get; set; }

        public string IdZona { get; set; }

        //public string FormulaPesoVolumetrico { get; set; }

        /// <summary>
        /// Tiempo de entrega para la zona designada
        /// </summary>

        public int TiempoEntrega { get; set; }

        /// <summary>
        /// Porcentaje al recargo de combustible
        /// </summary>
        //public decimal PorcentajeCombustible { get; set; }

        //public EnumEstadoRegistro EstadoRegistro { get; set; }
    }
}