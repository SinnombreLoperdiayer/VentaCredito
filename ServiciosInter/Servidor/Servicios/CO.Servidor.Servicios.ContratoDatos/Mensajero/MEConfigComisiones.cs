using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Mensajero
{
    public class MEConfigComisiones
    {
        public MEGrupoRodamiento GrupoRodamiento { get; set; }

        public MEGrupoBasico GrupoBasico { get; set; }

        public MEGruposLiquidacion GrupoLiquidacion { get; set; }

        public METipoPenalidad GrupoPenalidad { get; set; }

        public int IdPersona { get; set; }

        public int TotalPaginas { get; set; }

        
    }
}
