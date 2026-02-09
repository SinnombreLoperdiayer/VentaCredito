namespace ServiciosInter.DatosCompartidos.EntidadesNegocio.Giros
{
    public class TAPrecioDC
    {
        public decimal ValorServicio { get; set; }

        public decimal ValorTotalServicio { get; set; }

        public decimal ValorGiro { get; set; }

        public decimal? TarifaPorcPorte { get; set; }

        public decimal? TarifaFijaPorte { get; set; }

        public decimal ValorAdicionales { get; set; }

        public decimal ValorImpuestos { get; set; }

        public decimal ValorTotal { get; set; }
    }
}
