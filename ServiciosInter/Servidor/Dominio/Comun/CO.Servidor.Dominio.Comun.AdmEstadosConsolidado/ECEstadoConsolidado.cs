using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Dominio.Comun.AdmEstadosConsolidado
{
    public class ECEstadoConsolidado
    {
        public long IdEstadoConsolidado { get; set; }

        public string NoTula { get; set; }

        public EnumEstadosConsolidados Estado { get; set; }

        public string Observaciones { get; set; }

        public long IdCentroServicios { get; set; }

    }
}
