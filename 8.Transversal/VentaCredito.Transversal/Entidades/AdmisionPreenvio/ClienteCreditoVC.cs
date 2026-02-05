namespace VentaCredito.Transversal.Entidades.AdmisionPreenvio
{
    public class ClienteCreditoVC
    {
        public int IdCliente { get; set; }
        public string Nit { get; set; }
        public string RazonSocial { get; set; }
        public string Direccion { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string IdLocalidad { get; set; }
        public string NombreLocalidad { get; set; }
        public int IdContrato { get; set; }
        public int IdListaPrecios { get; set; }
        public decimal ValorPresupuesto { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
    }
}
