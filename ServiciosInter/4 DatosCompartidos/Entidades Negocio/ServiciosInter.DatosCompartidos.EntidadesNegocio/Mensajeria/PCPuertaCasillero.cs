namespace ServiciosInter.DatosCompartidos.EntidadesNegocio.Mensajeria
{
    public class PCPuertaCasillero
    {
        public long IdCentroServicioOrigen { get; set; }
        public long IdCentroServicioDestino { get; set; }
        public string IdCiudadDestino { get; set; }
        public string Puerta { get; set; }
        public string Casillero { get; set; }
        public string NombreCiudadCorto { get; set; }
        public string RutaPuertaCasillero { get; set; }
        public string IdCiudadOrigen { get; set; }
        public long NumeroGuia { get; set; }
    }
}
