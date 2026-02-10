using System.Collections.Generic;

namespace ServiciosInter.DatosCompartidos.EntidadesNegocio.Tarifas
{
    public class TAFormaPagoServicio
    {
        public int IdServicio { get; set; }
        public List<TAFormaPago> FormaPago { get; set; }
    }
}