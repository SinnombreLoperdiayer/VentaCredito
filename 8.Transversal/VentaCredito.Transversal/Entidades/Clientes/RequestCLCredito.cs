using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VentaCredito.Transversal.Entidades.Clientes
{
    public class RequestCLCredito
    {
        public long IdCliente { get; set; }
        public string NombreCliente { get; set; }
        public string NitCliente { get; set; }
        public string DigitoCliente { get; set; }
        public string DireccionCliente { get; set; }
        public string LocalidadCliente { get; set; }
        public string TelefonoCliente { get; set; }
        public string RepresentanteCliente { get; set; }
        public string CorreoCliente { get; set; }
        public List<int> ServiciosCliente { get; set; }
        public string urlToken { get; set; }
        public string urlNotificacion { get; set; }
        public string key { get; set; }
        public string Secret { get; set; }
        public int IdSucursal { get; set; }
        public string IdZona { get; set; }
        public List<int> EstadosCliente { get; set; }
    }
}
