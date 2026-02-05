using System;

namespace VentaCredito.Datos.Repositorio
{
    public class HorarioServicioAgil
    {
        public int IdHorario { get; set; }
        public int IdServicio { get; set; }
        public string Alias { get; set; }
        public DateTime HoraInicio { get; set; }
        public DateTime HoraFin { get; set; }
        public bool AplicaTodoDia { get; set; }
    }
}