using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Mensajero
{
    public class METipoRodamiento
    {
        public int IdGrupoRodamiento { get; set; }
        public string IdTipoTransporte { get; set; }
        public string TipoTransporte { get; set; }
        public double Valor { get; set; }

        public double MinimoVital { get; set; }
        
        public bool EstadoTpoRod { get; set; }

        public string CreadoPor { get; set; }

        public DateTime FechaCreacion { get; set; }
    }
}
