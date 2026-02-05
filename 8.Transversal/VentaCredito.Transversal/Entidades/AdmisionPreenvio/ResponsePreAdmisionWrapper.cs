using System;

namespace VentaCredito.Transversal.Entidades.AdmisionPreenvio
{
    public class ResponsePreAdmisionWrapper
    {
        public long IdPreenvio { get; set; }
        public long NumeroPreenvio { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public DateTime FechaCreacion { get; set; }
        public decimal ValorFlete { get; set; }
        public decimal ValorSobreFlete { get; set; }
        public decimal ValorServicioContraPago { get; set; }
    }
}
