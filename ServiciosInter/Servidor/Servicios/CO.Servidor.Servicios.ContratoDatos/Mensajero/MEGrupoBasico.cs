using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Mensajero
{
    public class MEGrupoBasico
    {
        public long IdGrupoBasico { get; set; }
        public string GrupoBasico { get; set; }
        public double ValorInicial { get; set; }
        public double ValorFinal { get; set; }
        public int NumeroCuotas { get; set; }
        public DateTime FechaInicial { get; set; }
        public DateTime FechaFinal { get; set; }
        public bool Estado { get; set; }

        public List<MEBasicoLiquidacion> Diferido { get; set; }

        public int TotalPaginas { get; set; }
    }
}
