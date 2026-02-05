using System.Collections.Generic;

namespace VentaCredito.Transversal.Entidades.Planilla
{
    public class PlanillaRecoleccionPreenviosRequest
    {
        public int IdCliente { get; set; }
        public int IdSucursal { get; set; }
        public List<long> ListaNumPreenvios { get; set; }
    }
}
