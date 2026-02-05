namespace VentaCredito.Sitio.Dominio.Comun
{
    public class FabricaServicios
    {

        private static VentaCredito.Negocio.ADAdmisionMensajeria servicioVentaCredito = new VentaCredito.Negocio.ADAdmisionMensajeria();
        public static VentaCredito.Negocio.ADAdmisionMensajeria ServicioVentaCredito
        {
            get { return FabricaServicios.servicioVentaCredito; }

        }

        private static VentaCredito.Negocio.ConsultaClientes servicioClientes = new VentaCredito.Negocio.ConsultaClientes();
        public static VentaCredito.Negocio.ConsultaClientes ServicioClientes
        {
            get { return FabricaServicios.servicioClientes; }
        }
    }
}
