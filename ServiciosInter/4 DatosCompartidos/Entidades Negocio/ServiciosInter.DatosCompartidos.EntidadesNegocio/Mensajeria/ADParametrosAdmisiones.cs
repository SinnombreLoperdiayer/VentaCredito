namespace ServiciosInter.DatosCompartidos.EntidadesNegocio.Mensajeria
{
    public class ADParametrosAdmisiones
    {
        public string UnidadMedidaPorDefecto { get; set; }
        public decimal PorcentajePrimaSeguro { get; set; }
        public decimal TopeMaxValorDeclarado { get; set; }
        public decimal TopeMinVlrDeclRapiCarga { get; set; }
        public decimal PesoPorDefecto { get; set; }
        public string TipoMonedaPorDefecto { get; set; }
        public bool TipoMonedaModificable { get; set; }
        public int PesoMinimoRotulo { get; set; }
        public double PorcentajeRecargo { get; set; }
        public int NumeroPiezasAplicaRotulo { get; set; }
        public string ImagenPublicidadGuia { get; set; }
        public string ValorReimpresionCertificacion { get; set; }
    }
}