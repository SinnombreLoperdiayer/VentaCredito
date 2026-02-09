using System;

namespace ServiciosInter.DatosCompartidos.EntidadesNegocio.CentrosServicio
{
    public class PUHorariosServiciosCentroServicios
    {
        public long IdCentroServicioSrvDia { get; set; }
        public long IdCentroServiciosServicio { get; set; }
        public string IdDia { get; set; }
        public DateTime HoraInicio { get; set; }
        public DateTime HoraFin { get; set; }
        public string NombreDia { get; set; }
    }
}