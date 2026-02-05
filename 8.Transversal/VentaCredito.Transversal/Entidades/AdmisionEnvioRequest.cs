using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VentaCredito.Transversal.Entidades
{
    public class AdmisionEnvioRequest
    {
        public int IdCliente { get; set; }
        public int IdServicio { get; set; }
        public int IdTipoEntrega { get; set; }
        public string IdPaisOrigen { get; set; }
        public string IdCiudadOrigen { get; set; }
        public string CodigoPostalOrigen { get; set; }
        public string IdPaisDestino { get; set; }
        public string IdCiudadDestino { get; set; }
        public string CodigoPostalDestino { get; set; }
        public decimal ValorDeclarado { get; set; }
        public string DiceContener { get; set; }
        public string Observaciones { get; set; }
        public int IdUnidadMedidaPeso { get; set; }
        public int Peso { get; set; }
        public int IdUnidadMedidaLongitud { get; set; }
        public decimal Largo { get; set; }
        public decimal Alto { get; set; }
        public decimal Ancho { get; set; }
        public short IdTipoEnvio { get; set; }
        public Destinatario Destinatario { get; set; }
        public bool NotificarEntregaPorEmail { get; set; }
        public string NoPedido { get; set; }
        public int IdSucursal { get; set; }
        public int IdTipoEmpaque {get;set;}
       


    }
}
