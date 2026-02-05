using System;
using System.Collections.Generic;

namespace VentaCredito.Transversal.Entidades.AdmisionPreenvio
{
    public class ResponseRecogidas
    {
        public long IdRecogida { get; set; }
        public int CantidadPreenvios { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public decimal PesoTotal { get; set; }
        public string MensajePreenviosAsociados { get; set; }
        public List<long> PreenviosAsociados { get; set; }
        public string MensajePreenviosNoIncluidos { get; set; }
        public List<long> PreenviosNoIncluidos { get; set; }
        public string MensajeCantidaMaximaPreenvios { get; set; }
    }
}
