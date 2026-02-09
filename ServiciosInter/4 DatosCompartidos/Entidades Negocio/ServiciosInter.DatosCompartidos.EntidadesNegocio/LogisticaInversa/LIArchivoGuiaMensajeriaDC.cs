namespace ServiciosInter.DatosCompartidos.EntidadesNegocio.LogisticaInversa
{
    public class LIArchivoGuiaMensajeriaDC
    {
        public long NumeroGuia { get; set; }

        public string RutaServidor { get; set; }

        public string ValorDecodificado { get; set; }

        public long IdAdmisionMensajeria { get; set; }

        public bool Sincronizada { get; set; }
    }
}