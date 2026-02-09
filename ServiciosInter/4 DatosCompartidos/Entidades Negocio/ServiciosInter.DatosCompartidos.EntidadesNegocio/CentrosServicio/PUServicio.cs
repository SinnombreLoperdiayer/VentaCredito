using System.Collections.Generic;

namespace ServiciosInter.DatosCompartidos.EntidadesNegocio.CentrosServicio
{
    public class PUServicio
    {
        public int IdServicio { get; set; }
        public string NombreServicio { get; set; }
        public string IdUnidadNegocio { get; set; }
        public string NombreUnidadNegocio { get; set; }
        public long IdCentroServicio { get; set; }
        public long IdCentroServicioServicio { get; set; }
        public string NombreLocalidad { get; set; }
        public string NombreCompletoLocalidad { get; set; }
        public string IdLocalidad { get; set; }
        public List<PUHorariosServiciosCentroServicios> HorariosServicios { get; set; }
    }
}