using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VentaCredito.Transversal.Entidades.AdmisionPreenvio
{
    public class ZNTurnoOperacion
    {
        public int IdTurno { get; set; }
        public int TipoTurno { get; set; }
        public string IdLocalidad { get; set; }
        public int IdCentroServicio { get; set; }
        public int IdDIa { get; set; }
        public string HorarioInicio { get; set; }
        public string HorarioFin { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string CreadoPor { get; set; }
        public bool Estado { get; set; }
    }
}
