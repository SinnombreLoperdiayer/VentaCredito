using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Mensajero
{
    public class METipoPenalidad
    {
        public int IdPenalidad { get; set; }
        public string Penalidad { get; set; } 
        public string IdPenalidadRaps { get; set; }

        public int IdParametroRaps { get; set; }
        public string DescripcionPenalidad { get; set; } 
        public int IdTipoUsuario { get; set; }
        public string TipoUsuario { get; set; }
        public double ValorPorcentual { get; set; }

        public double Porcentaje { get; set; }
        public METipoCuenta TipoCuenta { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

        public bool Estado { get; set; }

        public int TotalPaginas { get; set; }
    }
}
