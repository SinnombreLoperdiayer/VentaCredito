using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Mensajero
{
    public class METipoTransporte
    {
        public string IdTipoTransporte { get; set; }

        public string TipoTranpsorte { get; set; }

        public double MinimoVital { get; set; }

        public DateTime FechaCreado { get; set; }

        public string CreadoPor { get; set; }
    }
}
