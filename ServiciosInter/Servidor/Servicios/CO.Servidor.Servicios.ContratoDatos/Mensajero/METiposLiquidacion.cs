using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Mensajero
{
    public class METiposLiquidacion
    {
        public long IdTipoLiq { get; set; }
        public int IdGrupoLiq { get; set; }

        public string IdUnidad { get; set; }
        
        public string UnidadNegocio { get; set; }
        public int IdTipoAccion { get; set; }

        public string TipoAccion { get; set; }

        public string FormaPago { get; set; }

        public DateTime FechaModificacion { get; set; }

        public double ValorPorcentual { get; set; }

        public int TotalPaginas { get; set; }

        public int IdFormaPago { get; set; }

    }
}
