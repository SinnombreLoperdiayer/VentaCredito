namespace ServiciosInter.DatosCompartidos.EntidadesNegocio.Mensajeria
{
    public class DIIntegracionFactura
    {
        public int IdServicio { get; set; }
        public int IdTiposuministro { get; set; }
        public long IdCentroServicio { get; set; }
        public long? IdTercero { get; set; }
        public string NumeroIdentificacion { get; set; }
        public int IdAplicacion { get; set; }
        public long NumeroDocumento { get; set; }
        public bool EsManual { get; set; }
        public string NumeroCelular { get; set; }
        public string CreadoPor { get; set; }
    }
}
