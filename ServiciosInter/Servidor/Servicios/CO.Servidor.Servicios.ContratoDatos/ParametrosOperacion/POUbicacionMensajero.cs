using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion
{
    public class POUbicacionMensajero
    {
        public long IdMensajero { get; set; }

        public long IdDispositivo { get; set; }

        public decimal Longitud { get; set; }

        public decimal Latitud { get; set; }

        public POMensajero Mensajero { get; set; }

        public string IdLocalidad { get; set; }

        public int IdOrden { get; set; }
    }
}
