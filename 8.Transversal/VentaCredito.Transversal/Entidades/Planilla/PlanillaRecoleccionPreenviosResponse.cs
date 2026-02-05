using System;
using System.Collections.Generic;

namespace VentaCredito.Transversal.Entidades.Planilla
{
    public class PlanillaRecoleccionPreenviosResponse
    {
        public int NumeroPlanilla { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int CantidadPreenvios { get; set; }
        public string MensajeCantidaMaximaPreenvios { get; set; }
        public List<long> NumerosPreenviosNoIncluidos { get; set; }
        public string MensajePreenviosInvalidos { get; set; }
        public List<long> NumerosPreenviosInvalidos { get; set; }
        public byte[] ArregloBytesPlanilla { get; set; }
    }
}
