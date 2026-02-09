using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Consultas
{
    public class RAFallaMensajeroDC
    {
        public string TipoFalla { get; set; }

        public string ValorDeParametro { get; set; }

        public DateTime? FechaCrecionSolicitud { get; set; }

        public long IdParametrizacionRaps { get; set; } 

        public int NivelGravedad { get; set; } 
    }
}
