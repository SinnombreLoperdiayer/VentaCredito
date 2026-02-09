using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Raps
{

    public class ParametrosReglasEstadosRaps
    {
        public long IdParametrizacionReglasEstados { get; set; }
        public short IdMotivoController { get; set; }
        public long IdParametrizacion { get; set; }
        public int IdParametroAsociado { get; set; }
        public int IdFuncion { get; set; }
        public string DescripcionFuncion { get; set; }
        public string DescripcionParametrizacion { get; set; }
    }
}
