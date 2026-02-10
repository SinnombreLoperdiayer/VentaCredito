using System.Collections.Generic;

namespace ServiciosInter.DatosCompartidos.EntidadesNegocio.Mensajeria
{
    public class ADRangoTrayecto
    {
        public string IdLocalidadOrigen { get; set; }
        public string IdLocalidadDestino { get; set; }
        public List<ADRangoCasillero> Rangos { get; set; }
    }
}