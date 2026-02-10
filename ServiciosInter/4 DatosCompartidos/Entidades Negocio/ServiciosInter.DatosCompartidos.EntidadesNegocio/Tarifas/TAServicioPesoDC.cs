namespace ServiciosInter.DatosCompartidos.EntidadesNegocio.Tarifas
{
    /// <summary>
    /// Clase que contiene la información de los Parámetros de un Servicio
    /// </summary>
    public class TAServicioPesoDC
    {
        public int IdServicio { get; set; }
        public decimal PesoMinimo { get; set; }
        public decimal PesoMaximo { get; set; }
    }
}