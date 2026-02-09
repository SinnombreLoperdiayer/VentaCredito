using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Escalonamiento
{
    public class RACargoEscalonamientoDC
    {
        public string IdCargo { get; set; }

        public string IdProceso { get; set; }

        public string IdProcedimiento { get; set; }

        public int Orden { get; set; }
    }
}
