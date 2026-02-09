using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiciosInter.DatosCompartidos.EntidadesNegocio.Tarifas
{
    public class TAPrecioServicioDto
    {
        public long IdCliente { get; set; }
        public string IdLocalidadOrigen { get; set; }
        public string IdLocalidadDestino { get; set; }
        public decimal Peso { get; set; }
        public decimal ValorDeclarado { get; set; }
        public string IdTipoEntrega { get; set; }
        public string Fecha { get; set; }
        public int IdServicio { get; set; }
        public int IdContrato { get; set; }
        public int IdListaPrecios { get; set; }
        public long IdCentroServicioOrigen { get; set; }
        public bool? EsMarketPlace { get; set; }
    }
}
