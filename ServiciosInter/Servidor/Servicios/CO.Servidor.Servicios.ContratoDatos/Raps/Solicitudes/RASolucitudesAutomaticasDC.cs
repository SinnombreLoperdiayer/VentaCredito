using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes
{
    public class RASolucitudesAutomaticasDC
    {
        public long IdSolicitud { get; set; }
        public string Descripcion { get; set; }
        public string NombreTipo { get; set; }
        public int Cuenta { get; set; }
        public string ValorParametroAgrupamiento { get; set; }
    }
}
