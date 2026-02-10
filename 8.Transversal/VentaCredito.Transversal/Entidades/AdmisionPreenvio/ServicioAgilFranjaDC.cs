namespace VentaCredito.Transversal.Entidades.AdmisionPreenvio
{
    public class ServicioAgilFranjaDC
    {
        public int IdHorario { get; set; }
        public int IdServicio { get; set; }
        public string NombreServicio { get; set; }
        public string Alias { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFin { get; set; }
        public string FranjaHoraria { get; set; }
        public bool AplicaTodoDia { get; set; }
    }
}
