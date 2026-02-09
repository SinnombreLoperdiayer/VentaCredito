using System.Collections.Generic;

namespace ServiciosInter.DatosCompartidos.EntidadesNegocio.Tarifas
{
    public class TAPrecioCargaDC
    {
        public List<TAImpuestosDC> Impuestos { get; set; }
        public decimal ValorServicioRetorno { get; set; }
        public decimal ValorKiloAdicional { get; set; }
        public decimal Valor { get; set; }
        public decimal ValorContraPago { get; set; }
        public decimal ValorPrimaSeguro { get; set; }
    }
}