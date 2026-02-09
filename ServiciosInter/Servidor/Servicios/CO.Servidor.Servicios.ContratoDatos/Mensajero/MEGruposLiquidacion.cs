using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Mensajero
{
    public class MEGruposLiquidacion
    {
        public long IdGrupoLiq { get; set; }

        public string GrupoLiq { get; set; }

        public DateTime FechaInicio { get; set; }

        public DateTime FechaFinal { get; set; }

        public DateTime FechaCreacion { get; set; }

        public string CreadoPor { get; set; }

        public List<METiposLiquidacion> TiposLiquidacion { get; set; }

        public bool Estado { get; set; }

        public int TotalPaginas { get; set; }

    }
}
