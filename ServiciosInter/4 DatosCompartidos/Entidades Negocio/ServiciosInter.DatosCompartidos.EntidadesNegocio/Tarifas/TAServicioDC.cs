using System;

namespace ServiciosInter.DatosCompartidos.EntidadesNegocio.Tarifas
{
    public class TAServicioDC
    {
        public int IdServicio { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string UnidadNegocio { get; set; }

        public string IdUnidadNegocio { get; set; }

        public int? TiempoEntrega { get; set; }

        public int IdConceptoCaja { get; set; }

        public bool AplicaTodoElDia { get; set; }

        public DateTime? HoraInicio { get; set; }

        public DateTime? HoraFin { get; set; }

        public string FranjaServicio => AplicaTodoElDia || HoraFin is null || HoraInicio is null ? "Durante el dia" : $"{HoraInicio:h:mm tt} - {HoraFin:h:mm tt}".ToLower();

    }
}