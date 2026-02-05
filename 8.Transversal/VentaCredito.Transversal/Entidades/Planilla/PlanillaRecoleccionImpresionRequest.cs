using System;
using System.Collections.Generic;
using VentaCredito.Transversal.Entidades.AdmisionPreenvio;

namespace VentaCredito.Transversal.Entidades.Planilla
{
    public class PlanillaRecoleccionImpresionRequest
    {
        public int IdCliente { get; set; }
        public int IdSucursal { get; set; }
        public string NombreSucursal { get; set; }
        public int NumeroPlanilla { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string FechaRegistro { get; set; }
        public string HoraCreacion { get; set; }
        public int CantidadPreenvios { get; set; }
        public List<long> NumerosPreenviosValidos { get; set; }
        public string MensajeCantidaMaximaPreenvios { get; set; }
        public List<long> NumerosPreenviosNoIncluidos { get; set; }
        public string mensajePreenviosInvalidos { get; set; }
        public List<long> numerosPreenviosInvalidos { get; set; }
        public ClienteCreditoVC Cliente { get; set; }
    }
}
