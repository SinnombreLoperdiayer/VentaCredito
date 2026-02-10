using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Integraciones.GirosRest
{
    public class ResultadoBiometrico
    {

        public int CodigoMensaje { get; set; }
        public string Message { get; set; }
        public DateTime Fecha { get; set; }
        public string SessionId { get; set; }
    }
}
