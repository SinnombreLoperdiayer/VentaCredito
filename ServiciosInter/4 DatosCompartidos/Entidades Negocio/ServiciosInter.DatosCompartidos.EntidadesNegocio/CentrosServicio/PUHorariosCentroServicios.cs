using System;

namespace ServiciosInter.DatosCompartidos.EntidadesNegocio.CentrosServicio
{
    /// <summary>
    /// Clase que contiene la informacion de los horarios de los centros de servicios
    /// </summary>
    public class PUHorariosCentroServicios
    {
        public string IdLocalidad { get; set; }
        public int IdHorarioCentroServicios { get; set; }
        public long IdCentroServicios { get; set; }
        public string IdDia { get; set; }
        public string NombreDia { get; set; }
        public DateTime HoraInicio { get; set; }
        public DateTime HoraFin { get; set; }
    }
}